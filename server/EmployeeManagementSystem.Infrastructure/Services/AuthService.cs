using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EmployeeManagementSystem.Application.DTOs.Auth;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Infrastructure.Data;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManagementSystem.Infrastructure.Services;

/// <summary>
/// Service for handling authentication operations using Google OAuth2 and JWT.
/// </summary>
public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private static readonly HttpClient _httpClient = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthService"/> class.
    /// </summary>
    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    /// <inheritdoc />
    public async Task<AuthResponseDto> AuthenticateWithGoogleAsync(string idToken, string ipAddress)
    {
        // Validate the Google ID token
        var payload = await ValidateGoogleTokenAsync(idToken);

        // Find or create the user - use IgnoreQueryFilters to find soft-deleted users too
        var user = await _context.Users
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.GoogleId == payload.Subject);

        if (user == null)
        {
            // Create new user
            user = new User
            {
                Id = Guid.NewGuid(),
                GoogleId = payload.Subject,
                Email = payload.Email,
                FirstName = payload.GivenName ?? string.Empty,
                LastName = payload.FamilyName ?? string.Empty,
                ProfilePictureUrl = payload.Picture,
                CreatedOn = DateTime.UtcNow,
                IsActive = true,
                Role = "User"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        else
        {
            // Update user directly in database using ExecuteUpdateAsync to bypass change tracking issues
            await _context.Users
                .IgnoreQueryFilters()
                .Where(u => u.Id == user.Id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(u => u.Email, payload.Email)
                    .SetProperty(u => u.FirstName, payload.GivenName ?? user.FirstName)
                    .SetProperty(u => u.LastName, payload.FamilyName ?? user.LastName)
                    .SetProperty(u => u.ProfilePictureUrl, payload.Picture ?? user.ProfilePictureUrl)
                    .SetProperty(u => u.LastLoginOn, DateTime.UtcNow)
                    .SetProperty(u => u.ModifiedOn, DateTime.UtcNow)
                    .SetProperty(u => u.IsDeleted, false));

            // Revoke all active refresh tokens for this user directly in database
            await _context.RefreshTokens
                .IgnoreQueryFilters()
                .Where(rt => rt.UserId == user.Id && rt.RevokedOn == null && rt.ExpiresOn > DateTime.UtcNow)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(rt => rt.RevokedOn, DateTime.UtcNow)
                    .SetProperty(rt => rt.RevokedByIp, ipAddress)
                    .SetProperty(rt => rt.ReasonRevoked, "Replaced by new token on login"));

            // Refresh user data after update
            user.Email = payload.Email;
            user.FirstName = payload.GivenName ?? user.FirstName;
            user.LastName = payload.FamilyName ?? user.LastName;
            user.ProfilePictureUrl = payload.Picture ?? user.ProfilePictureUrl;
            user.IsDeleted = false;
        }

        // Generate JWT and refresh token
        var accessToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken(ipAddress);
        refreshToken.UserId = user.Id;

        // Add new refresh token directly
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresOn = DateTime.UtcNow.AddMinutes(GetAccessTokenExpirationMinutes()),
            User = MapToUserDto(user)
        };
    }

    /// <inheritdoc />
    public async Task<AuthResponseDto> AuthenticateWithGoogleAccessTokenAsync(string accessToken, string ipAddress)
    {
        // Fetch user info from Google using the access token
        var userInfo = await GetGoogleUserInfoAsync(accessToken);

        // Find or create the user - use IgnoreQueryFilters to find soft-deleted users too
        var user = await _context.Users
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.GoogleId == userInfo.Sub);

        if (user == null)
        {
            // Create new user
            user = new User
            {
                Id = Guid.NewGuid(),
                GoogleId = userInfo.Sub,
                Email = userInfo.Email,
                FirstName = userInfo.GivenName ?? string.Empty,
                LastName = userInfo.FamilyName ?? string.Empty,
                ProfilePictureUrl = userInfo.Picture,
                CreatedOn = DateTime.UtcNow,
                IsActive = true,
                Role = "User"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        else
        {
            // Update user directly in database using ExecuteUpdateAsync to bypass change tracking issues
            await _context.Users
                .IgnoreQueryFilters()
                .Where(u => u.Id == user.Id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(u => u.Email, userInfo.Email)
                    .SetProperty(u => u.FirstName, userInfo.GivenName ?? user.FirstName)
                    .SetProperty(u => u.LastName, userInfo.FamilyName ?? user.LastName)
                    .SetProperty(u => u.ProfilePictureUrl, userInfo.Picture ?? user.ProfilePictureUrl)
                    .SetProperty(u => u.LastLoginOn, DateTime.UtcNow)
                    .SetProperty(u => u.ModifiedOn, DateTime.UtcNow)
                    .SetProperty(u => u.IsDeleted, false));

            // Revoke all active refresh tokens for this user directly in database
            await _context.RefreshTokens
                .IgnoreQueryFilters()
                .Where(rt => rt.UserId == user.Id && rt.RevokedOn == null && rt.ExpiresOn > DateTime.UtcNow)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(rt => rt.RevokedOn, DateTime.UtcNow)
                    .SetProperty(rt => rt.RevokedByIp, ipAddress)
                    .SetProperty(rt => rt.ReasonRevoked, "Replaced by new token on login"));

            // Refresh user data after update
            user.Email = userInfo.Email;
            user.FirstName = userInfo.GivenName ?? user.FirstName;
            user.LastName = userInfo.FamilyName ?? user.LastName;
            user.ProfilePictureUrl = userInfo.Picture ?? user.ProfilePictureUrl;
            user.IsDeleted = false;
        }

        // Generate JWT and refresh token
        var jwtToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken(ipAddress);
        refreshToken.UserId = user.Id;

        // Add new refresh token directly
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = jwtToken,
            RefreshToken = refreshToken.Token,
            ExpiresOn = DateTime.UtcNow.AddMinutes(GetAccessTokenExpirationMinutes()),
            User = MapToUserDto(user)
        };
    }

    /// <inheritdoc />
    public async Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken, string ipAddress)
    {
        var user = await _context.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));

        if (user == null)
        {
            return null;
        }

        var existingToken = user.RefreshTokens.Single(t => t.Token == refreshToken);

        if (!existingToken.IsActive)
        {
            // Token has been revoked or expired - revoke all tokens for security
            if (existingToken.IsRevoked)
            {
                // Potential token reuse - revoke all descendant tokens
                RevokeDescendantTokens(existingToken, user.RefreshTokens, ipAddress, "Attempted reuse of revoked token");
            }

            return null;
        }

        // Generate new tokens
        var newRefreshToken = GenerateRefreshToken(ipAddress);
        existingToken.RevokedOn = DateTime.UtcNow;
        existingToken.RevokedByIp = ipAddress;
        existingToken.ReplacedByToken = newRefreshToken.Token;
        existingToken.ReasonRevoked = "Rotated";

        user.RefreshTokens.Add(newRefreshToken);
        user.LastLoginOn = DateTime.UtcNow;
        user.ModifiedOn = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var accessToken = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken.Token,
            ExpiresOn = DateTime.UtcNow.AddMinutes(GetAccessTokenExpirationMinutes()),
            User = MapToUserDto(user)
        };
    }

    /// <inheritdoc />
    public async Task<bool> RevokeTokenAsync(string refreshToken, string ipAddress)
    {
        var user = await _context.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));

        if (user == null)
        {
            return false;
        }

        var token = user.RefreshTokens.Single(t => t.Token == refreshToken);

        if (!token.IsActive)
        {
            return false;
        }

        token.RevokedOn = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        token.ReasonRevoked = "Revoked by user";

        await _context.SaveChangesAsync();

        return true;
    }

    /// <inheritdoc />
    public async Task<UserDto?> GetUserByIdAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        return user == null ? null : MapToUserDto(user);
    }

    /// <summary>
    /// Validates a Google ID token and returns the payload.
    /// </summary>
    private async Task<GoogleJsonWebSignature.Payload> ValidateGoogleTokenAsync(string idToken)
    {
        var clientId = _configuration["Authentication:Google:ClientId"]
            ?? throw new InvalidOperationException("Google ClientId is not configured");

        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new[] { clientId }
        };

        return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
    }

    /// <summary>
    /// Generates a JWT access token for the user.
    /// </summary>
    private string GenerateJwtToken(User user)
    {
        var jwtSecret = _configuration["Authentication:Jwt:Secret"]
            ?? throw new InvalidOperationException("JWT Secret is not configured");
        var issuer = _configuration["Authentication:Jwt:Issuer"]
            ?? throw new InvalidOperationException("JWT Issuer is not configured");
        var audience = _configuration["Authentication:Jwt:Audience"]
            ?? throw new InvalidOperationException("JWT Audience is not configured");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(GetAccessTokenExpirationMinutes()),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Generates a new refresh token.
    /// </summary>
    private RefreshToken GenerateRefreshToken(string ipAddress)
    {
        var refreshTokenDays = int.Parse(_configuration["Authentication:Jwt:RefreshTokenExpirationDays"] ?? "7");

        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            ExpiresOn = DateTime.UtcNow.AddDays(refreshTokenDays),
            CreatedOn = DateTime.UtcNow,
            CreatedByIp = ipAddress
        };
    }

    /// <summary>
    /// Gets the access token expiration time in minutes.
    /// </summary>
    private int GetAccessTokenExpirationMinutes()
    {
        return int.Parse(_configuration["Authentication:Jwt:ExpiresMinutes"] ?? "60");
    }

    /// <summary>
    /// Revokes all descendant tokens of a token family.
    /// </summary>
    private void RevokeDescendantTokens(RefreshToken token, ICollection<RefreshToken> allTokens, string ipAddress, string reason)
    {
        if (string.IsNullOrEmpty(token.ReplacedByToken))
        {
            return;
        }

        var childToken = allTokens.FirstOrDefault(t => t.Token == token.ReplacedByToken);

        if (childToken != null)
        {
            if (childToken.IsActive)
            {
                childToken.RevokedOn = DateTime.UtcNow;
                childToken.RevokedByIp = ipAddress;
                childToken.ReasonRevoked = reason;
            }

            RevokeDescendantTokens(childToken, allTokens, ipAddress, reason);
        }
    }

    /// <summary>
    /// Maps a User entity to a UserDto.
    /// </summary>
    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ProfilePictureUrl = user.ProfilePictureUrl,
            Role = user.Role
        };
    }

    /// <summary>
    /// Fetches user information from Google using an access token.
    /// </summary>
    private static async Task<GoogleUserInfo> GetGoogleUserInfoAsync(string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v3/userinfo");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Failed to get user info from Google: {error}");
        }

        var content = await response.Content.ReadAsStringAsync();
        var userInfo = JsonSerializer.Deserialize<GoogleUserInfo>(content)
            ?? throw new InvalidOperationException("Failed to deserialize Google user info");

        return userInfo;
    }

    /// <summary>
    /// Internal class representing Google userinfo response.
    /// </summary>
    private class GoogleUserInfo
    {
        [System.Text.Json.Serialization.JsonPropertyName("sub")]
        public string Sub { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("email_verified")]
        public bool EmailVerified { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("name")]
        public string? Name { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("given_name")]
        public string? GivenName { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("family_name")]
        public string? FamilyName { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("picture")]
        public string? Picture { get; set; }
    }
}
