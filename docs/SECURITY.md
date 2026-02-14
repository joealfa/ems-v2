# Security Best Practices & Guidelines

## Table of Contents
1. [Authentication & Authorization](#authentication--authorization)
2. [Security Vulnerabilities Found & Recommendations](#security-vulnerabilities-found--recommendations)
3. [Token Management](#token-management)
4. [API Security](#api-security)
5. [CORS Configuration](#cors-configuration)
6. [Security Headers](#security-headers)
7. [Secrets Management](#secrets-management)
8. [Input Validation](#input-validation)
9. [Database Security](#database-security)
10. [Caching Security](#caching-security)
11. [Security Checklist](#security-checklist)

---

## Authentication & Authorization

### Current Implementation

**Authentication Flow:**
```
1. User logs in with Google OAuth2
2. Backend validates Google ID token
3. Backend generates JWT access token (15 min) + refresh token (7 days)
4. Access token stored in localStorage
5. Refresh token stored in HttpOnly cookie
6. Frontend includes access token in Authorization header
7. Token automatically refreshed when expired
```

**Token Structure:**
- **Access Token**: JWT with 15-minute expiration, stored in `localStorage`
- **Refresh Token**: Opaque token with 7-day expiration, stored in HttpOnly cookie
- **Cookie Settings**: HttpOnly, Secure, SameSite=Strict

### JWT Configuration

Located in `server/EmployeeManagementSystem.Api/Program.cs`:

```csharp
TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = configuration["Authentication:Jwt:Issuer"],
    ValidAudience = configuration["Authentication:Jwt:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(configuration["Authentication:Jwt:Secret"] ?? throw new InvalidOperationException())),
    ClockSkew = TimeSpan.Zero  // Strict expiration enforcement
}
```

**Security Features:**
✅ All validation flags enabled
✅ Clock skew set to zero (strict expiration)
✅ Signing key from secure configuration
✅ Issuer and audience validation

---

## Security Vulnerabilities Found & Recommendations

### ✅ RESOLVED: Environment File Template Created

**Previous Issue:** `.env` file potentially exposed in git

**Status:** ✅ **RESOLVED** (Feb 5, 2026)

**Actions Taken:**
1. ✅ Verified `.env` is not tracked in git (already in `.gitignore`)
2. ✅ Created `.env.example` template file
3. ✅ Updated `application/README.md` with setup instructions

**Current Setup:**
- `.env.example` contains placeholder values
- Developers copy `.env.example` to `.env` and add actual credentials
- `.env` is ignored by git
- Documentation updated with clear setup instructions

---

### ✅ VERIFIED: Refresh Token Rotation Already Implemented

**Status:** ✅ **ALREADY IMPLEMENTED** (Verified Feb 5, 2026)

**Implementation Details:**
The `AuthService.RefreshTokenAsync()` method already implements industry-standard token rotation:

1. ✅ **Validates old token** - Checks if token is active
2. ✅ **Generates new token** - Creates fresh refresh token
3. ✅ **Revokes old token** - Marks old token as revoked with timestamp and IP
4. ✅ **Tracks token family** - Sets `ReplacedByToken` for audit trail
5. ✅ **Detects token reuse** - Revokes all descendant tokens on security breach

**Key Features:**
```csharp
// From AuthService.cs:194-240
public async Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken, string ipAddress)
{
    // Token validation and user lookup
    RefreshToken existingToken = user.RefreshTokens.Single(t => t.Token == refreshToken);

    if (!existingToken.IsActive)
    {
        if (existingToken.IsRevoked)
        {
            // Security: Revoke all descendant tokens on reuse attempt
            RevokeDescendantTokens(existingToken, user.RefreshTokens, ipAddress,
                "Attempted reuse of revoked token");
        }
        return null;
    }

    // Generate new token and revoke old
    RefreshToken newRefreshToken = GenerateRefreshToken(ipAddress);
    existingToken.RevokedOn = DateTime.UtcNow;
    existingToken.RevokedByIp = ipAddress;
    existingToken.ReplacedByToken = newRefreshToken.Token;  // Track token family
    existingToken.ReasonRevoked = "Rotated";

    user.RefreshTokens.Add(newRefreshToken);
    await _context.SaveChangesAsync();

    return new AuthResponseDto { ... };
}
```

**Security Benefits:**
- ✅ Limits exposure window (new token on each refresh)
- ✅ Detects token theft (reuse of revoked token = security breach)
- ✅ Industry standard practice
- ✅ Full audit trail with IP tracking

---

### ✅ IMPLEMENTED: Rate Limiting Added

**Status:** ✅ **IMPLEMENTED** (Feb 5, 2026)

**Package Installed:** `AspNetCoreRateLimit 5.0.0`

**Configuration:**

**Development (`appsettings.Development.json`):**
```json
{
  "IpRateLimiting": {
    "EnableEndpointRateLimiting": true,
    "StackBlockedRequests": false,
    "RealIpHeader": "X-Forwarded-For",
    "HttpStatusCode": 429,
    "GeneralRules": [
      {
        "Endpoint": "POST:/api/v1/auth/*",
        "Period": "1m",
        "Limit": 10
      },
      {
        "Endpoint": "*",
        "Period": "1m",
        "Limit": 200
      }
    ]
  }
}
```

**Production (`appsettings.json`):**
```json
{
  "IpRateLimiting": {
    "EnableEndpointRateLimiting": true,
    "StackBlockedRequests": false,
    "RealIpHeader": "X-Forwarded-For",
    "HttpStatusCode": 429,
    "GeneralRules": [
      {
        "Endpoint": "POST:/api/v1/auth/*",
        "Period": "1m",
        "Limit": 5
      },
      {
        "Endpoint": "*",
        "Period": "1m",
        "Limit": 100
      }
    ]
  }
}
```

**Implementation (`Program.cs`):**
```csharp
// Configure IP Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// Use IP Rate Limiting (after CORS, before authentication)
app.UseIpRateLimiting();
```

**Protection Provided:**
- ✅ Authentication endpoints: Max 5 requests/minute (production), 10 requests/minute (development)
- ✅ General endpoints: Max 100 requests/minute (production), 200 requests/minute (development)
- ✅ Returns HTTP 429 (Too Many Requests) when limit exceeded
- ✅ Uses `X-Forwarded-For` header for accurate IP detection behind proxies

---

### ✅ RESOLVED: CORS Configuration Optimized

**Status:** ✅ **OPTIMIZED** (Feb 5, 2026)

**Previous Issue:** Too many localhost origins, including unused ports

**Current Configuration:**

**Development (`appsettings.Development.json`):**
```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:5173",   // Frontend (Vite)
      "https://localhost:5003"   // Gateway
    ]
  }
}
```

**Production (`appsettings.json`):**
```json
{
  "Cors": {
    "AllowedOrigins": [
      "https://your-production-domain.com"
    ]
  }
}
```

**Changes Made:**
- ✅ Removed `http://localhost:5001` (unused)
- ✅ Removed `https://localhost:7009` (non-standard port, unused)
- ✅ Kept only active origins: Frontend (5173) and Gateway (5003)
- ✅ Production config uses exact domain (no wildcards)

**Security Improvements:**
- Reduced attack surface
- Only necessary origins allowed
- Clear separation between development and production
- No wildcard CORS in production

---

### 🟢 LOW: SQL Injection Risk Mitigated

**Status:** ✅ Using Entity Framework Core with parameterized queries

**Verification:**
- All database queries use EF Core LINQ
- No raw SQL with string concatenation
- Repository pattern enforces safe queries

**Example (Safe):**
```csharp
public async Task<Person?> GetByDisplayIdAsync(long displayId)
{
    // EF Core automatically parameterizes
    return await _dbContext.Persons
        .FirstOrDefaultAsync(p => p.DisplayId == displayId);
}
```

---

### 🟢 LOW: XSS Protection

**Status:** ✅ Frontend uses React (auto-escapes by default)

**Verification:**
- React escapes all text content automatically
- No use of `dangerouslySetInnerHTML`
- GraphQL responses are JSON (not HTML)

**Ensure continued protection:**
- Never use `dangerouslySetInnerHTML` without sanitization
- Validate file uploads (check MIME types, scan for malware)
- Use Content Security Policy headers in production

---

### 🟢 LOW: HTTPS Enforcement

**Status:** ✅ HTTPS enforced in production

**Configuration:**
```csharp
// Program.cs
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}
```

**Cookie Security:**
```csharp
cookieOptions = new()
{
    HttpOnly = true,      // ✅ Prevents XSS access
    Secure = true,        // ✅ HTTPS only
    SameSite = SameSiteMode.Strict  // ✅ Prevents CSRF
};
```

---

## Token Management

### Access Tokens

**Storage:** `localStorage` (Frontend)
```typescript
// src/contexts/AuthContext.tsx
localStorage.setItem('access_token', response.accessToken);
```

**Why localStorage?**
- Needs to be accessible to JavaScript for GraphQL requests
- Short expiration (15 minutes) limits exposure
- Cleared on logout

**Security Considerations:**
- ⚠️ Vulnerable to XSS attacks
- ✅ Mitigated by React's auto-escaping
- ✅ Short expiration limits damage

### Refresh Tokens

**Storage:** HttpOnly Cookie (Backend)
```csharp
cookieOptions = new()
{
    HttpOnly = true,      // JavaScript cannot access
    Secure = true,        // HTTPS only
    SameSite = SameSiteMode.Strict,  // CSRF protection
    Expires = DateTime.UtcNow.AddDays(7)
};
```

**Why HttpOnly Cookie?**
- Not accessible to JavaScript (XSS protection)
- Automatically included in requests to same domain
- Server-side validation

### Token Refresh Flow

```
1. Frontend detects expired access token
2. Frontend calls /api/v1/auth/refresh (refresh token in cookie)
3. Backend validates refresh token from cookie
4. Backend generates new access token
5. Backend returns new access token
6. Frontend stores new access token in localStorage
```

**Implementation:**
```typescript
// Frontend token refresh
const refreshAccessToken = async () => {
  const response = await fetch('/api/v1/auth/refresh', {
    method: 'POST',
    credentials: 'include'  // Include cookies
  });

  if (response.ok) {
    const { accessToken } = await response.json();
    localStorage.setItem('access_token', accessToken);
    return accessToken;
  }

  // Refresh failed, redirect to login
  logout();
};
```

---

## API Security

### Authorization Attributes

All protected endpoints use `[Authorize]` attribute:

```csharp
[HttpGet("{displayId}")]
[Authorize]  // Requires valid JWT
[ProducesResponseType(typeof(PersonResponseDto), StatusCodes.Status200OK)]
public async Task<ActionResult<PersonResponseDto>> GetPersonAsync(long displayId)
{
    // Only authenticated users can access
}
```

### User Identity

Access current user ID in controllers:

```csharp
public abstract class ApiControllerBase : ControllerBase
{
    protected string? CurrentUser => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
}
```

### API Versioning

**URL-based versioning:**
- v1: `/api/v1/[controller]`
- v2: `/api/v2/[controller]` (future)

**Benefits:**
- Clear version in URL
- Easy to maintain multiple versions
- Simple routing

---

## CORS Configuration

### Development

**Allowed Origins:**
```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:5173",   // Vite dev server
      "https://localhost:5003"   // GraphQL Gateway
    ]
  }
}
```

### Production

**Recommendation:**
```json
{
  "Cors": {
    "AllowedOrigins": [
      "https://yourdomain.com"
    ]
  }
}
```

**Configuration (`Program.cs`):**
```csharp
string[] allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? throw new InvalidOperationException();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowHost", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();  // Required for cookies
    });
});
```

---

## Security Headers

The application implements comprehensive security headers to provide defense-in-depth protection.

### Implemented Headers

| Header | Purpose | Status |
|--------|---------|--------|
| Content-Security-Policy | XSS protection via resource restrictions | ✅ Enabled |
| Strict-Transport-Security | Force HTTPS (production only) | ✅ Enabled |
| X-Content-Type-Options | Prevent MIME sniffing | ✅ Enabled |
| X-Frame-Options | Clickjacking protection | ✅ Enabled |
| X-XSS-Protection | Legacy XSS filter | ✅ Enabled |
| Referrer-Policy | Control referrer information | ✅ Enabled |
| Permissions-Policy | Feature/API restrictions | ✅ Enabled |

### Implementation

Security headers are applied via middleware:
- Backend: `server/EmployeeManagementSystem.Api/Middleware/SecurityHeadersMiddleware.cs`
- Gateway: `gateway/EmployeeManagementSystem.Gateway/Middleware/SecurityHeadersMiddleware.cs`

### Environment-Specific Policies

**Development:** Relaxed CSP for developer tools (Swagger, Banana Cake Pop)  
**Production:** Strict CSP with no `unsafe-inline` or `unsafe-eval`

### Testing

Verify headers are present:
```powershell
curl -I https://localhost:7166/api/v1/persons
```

### Documentation

See [SECURITY-HEADERS.md](SECURITY-HEADERS.md) for complete documentation including:
- All header values and configurations
- Testing procedures
- Browser compatibility
- Troubleshooting guide
- Best practices

---

## Secrets Management

### User Secrets (Development)

**Backend API:**
```bash
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "your-secret-key-here"
dotnet user-secrets set "Jwt:Issuer" "EmployeeManagementSystem"
dotnet user-secrets set "Jwt:Audience" "EmployeeManagementSystem"
dotnet user-secrets set "GoogleAuth:ClientId" "your-client-id"
dotnet user-secrets set "GoogleAuth:ClientSecret" "your-client-secret"
```

**Storage Location:**
- Windows: `%APPDATA%\Microsoft\UserSecrets\<user-secrets-id>\secrets.json`
- Linux/macOS: `~/.microsoft/usersecrets/<user-secrets-id>/secrets.json`

**User Secrets IDs:**
- Backend API: `6d3bfc92-af45-453d-aa90-b6da41f650cf`
- Gateway: `d5f56e0f-1368-4b36-991d-815b1521dbd8`

### Environment Variables (Production)

**Azure App Service:**
```bash
az webapp config appsettings set \
  --resource-group <resource-group> \
  --name <app-name> \
  --settings \
    Jwt__Key="<production-jwt-key>" \
    Jwt__Issuer="EmployeeManagementSystem" \
    Jwt__Audience="EmployeeManagementSystem" \
    GoogleAuth__ClientId="<prod-client-id>" \
    GoogleAuth__ClientSecret="<prod-client-secret>"
```

### Secret Rotation

**Best Practices:**
1. Rotate JWT signing keys every 90 days
2. Use different keys for dev/staging/production
3. Store secrets in Azure Key Vault for production
4. Never commit secrets to git

---

## Input Validation

### DTO Validation

All input DTOs use Data Annotations:

```csharp
public class CreatePersonDto
{
    [Required(ErrorMessage = "First name is required")]
    [MaxLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? Email { get; set; }

    [Range(1, 150, ErrorMessage = "Age must be between 1 and 150")]
    public int? Age { get; set; }
}
```

### Validation Pipeline

```csharp
// Automatic validation in ASP.NET Core
[ApiController]  // Enables automatic model validation
public class PersonsController : ApiControllerBase
{
    [HttpPost]
    public async Task<ActionResult<PersonResponseDto>> CreateAsync(
        [FromBody] CreatePersonDto dto)  // Validated automatically
    {
        // If validation fails, 400 BadRequest returned automatically
        // If validation passes, this code executes
    }
}
```

### File Upload Validation

**Required Checks:**
1. File size limit (10 MB recommended)
2. Allowed MIME types
3. File extension whitelist
4. Virus scanning (production)

```csharp
public async Task<IActionResult> UploadDocument(IFormFile file)
{
    // 1. Check file size
    if (file.Length > 10 * 1024 * 1024)  // 10 MB
    {
        return BadRequest("File size exceeds 10 MB");
    }

    // 2. Check content type
    string[] allowedTypes = { "image/jpeg", "image/png", "application/pdf" };
    if (!allowedTypes.Contains(file.ContentType))
    {
        return BadRequest("Invalid file type");
    }

    // 3. Check extension
    string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
    if (extension != ".jpg" && extension != ".png" && extension != ".pdf")
    {
        return BadRequest("Invalid file extension");
    }

    // 4. Process file
    await ProcessFileAsync(file);
}
```

---

## Database Security

### Soft Deletes

All entities use soft delete pattern:

```csharp
public abstract class BaseEntity
{
    public bool IsDeleted { get; set; }
}

// Global query filter
modelBuilder.Entity<Person>()
    .HasQueryFilter(p => !p.IsDeleted);
```

**Benefits:**
- Prevents accidental data loss
- Audit trail preserved
- Can recover deleted data

### Audit Trail

All entities track:
- `CreatedBy` - User who created the record
- `CreatedAt` - Timestamp of creation
- `ModifiedBy` - User who last modified
- `ModifiedAt` - Timestamp of last modification

```csharp
public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
{
    foreach (var entry in ChangeTracker.Entries<BaseEntity>())
    {
        if (entry.State == EntityState.Added)
        {
            entry.Entity.CreatedAt = DateTime.UtcNow;
            entry.Entity.CreatedBy = _currentUserService.UserId;
        }

        if (entry.State == EntityState.Modified)
        {
            entry.Entity.ModifiedAt = DateTime.UtcNow;
            entry.Entity.ModifiedBy = _currentUserService.UserId;
        }
    }

    return await base.SaveChangesAsync(ct);
}
```

### Connection String Security

**Development:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EmployeeManagementSystem;Trusted_Connection=true;TrustServerCertificate=true"
  }
}
```

**Production (Azure SQL):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:yourserver.database.windows.net,1433;Database=EmployeeManagementSystem;User ID=yourusername;Password=yourpassword;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```

**Best Practice:** Use Managed Identity in Azure to avoid storing credentials

---

## Caching Security

### Redis Cache Keys

**Current Implementation (Hash-based):**
```csharp
private static string GenerateHashedKey(string prefix, object parameters)
{
    string json = JsonSerializer.Serialize(parameters);
    byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(json));
    string hash = Convert.ToHexString(hashBytes).ToLowerInvariant();
    return $"{prefix}{hash[..16]}";
}
```

**Security Benefits:**
- No sensitive data in cache keys
- Deterministic hashing
- Short hash (16 chars) for performance

### Cache Invalidation

**On Data Modification:**
```csharp
// After creating/updating a person
await _cache.RemoveByPrefixAsync(CacheKeys.PersonsListPrefix);
await _cache.RemoveAsync(CacheKeys.Person(displayId));
```

**TTL Strategy:**
- Individual entities: 10 minutes
- List queries: 2 minutes
- Dashboard stats: 1 minute

### Redis Security

**Configuration (`appsettings.json`):**
```json
{
  "Redis": {
    "ConnectionString": "localhost:6379",
    "InstanceName": "EmsGateway:"
  }
}
```

**Production Recommendations:**
1. Use TLS/SSL for Redis connection
2. Enable Redis AUTH with strong password
3. Use Azure Cache for Redis with private endpoint
4. Restrict network access to Redis

```json
{
  "Redis": {
    "ConnectionString": "your-redis.redis.cache.windows.net:6380,password=your-password,ssl=True,abortConnect=False",
    "InstanceName": "EmsGateway_Prod:"
  }
}
```

---

## Security Checklist

### Development Environment

- ✅ HTTPS enabled for all services (localhost certificates)
- ✅ User secrets configured for sensitive data
- ✅ `.env` file in `.gitignore`
- ✅ `.env.example` template created
- ✅ Development authentication bypass (`#if DEBUG` only)
- ✅ Soft deletes enabled
- ✅ Audit trail tracking

### Authentication & Authorization

- ✅ JWT validation configured correctly
- ✅ Clock skew set to zero (strict expiration)
- ✅ HttpOnly cookies for refresh tokens
- ✅ Secure and SameSite flags on cookies
- ✅ Refresh token rotation implemented (verified)
- ✅ Rate limiting implemented (AspNetCoreRateLimit)
- ✅ Google OAuth2 integration

### API Security

- ✅ CORS configured with specific origins (optimized)
- ✅ Input validation with Data Annotations
- ✅ Entity Framework Core (parameterized queries)
- ✅ Authorization attributes on endpoints
- ✅ API versioning implemented
- ✅ Rate limiting on all endpoints (AspNetCoreRateLimit)
- ✅ Content Security Policy (CSP) headers
- ✅ HSTS (HTTP Strict Transport Security)
- ✅ Security headers (X-Frame-Options, X-Content-Type-Options, etc.)

### Data Protection

- ✅ Soft deletes (no hard deletes)
- ✅ Audit trail (CreatedBy, ModifiedBy, timestamps)
- ✅ EF Core query filters
- ✅ Azure Blob Storage for files
- ✅ Redis caching with TTL

### Frontend Security

- ✅ React auto-escaping (XSS protection)
- ✅ No use of `dangerouslySetInnerHTML`
- ✅ TypeScript for type safety
- ✅ Token auto-refresh implemented
- ✅ Logout clears tokens

### Production Readiness

- ✅ Deployment guide created (docs/DEPLOYMENT.md)
- ⚠️ Monitoring/logging strategy needs documentation
- ✅ Rate limiting implemented
- ✅ Content Security Policy headers implemented (docs/SECURITY-HEADERS.md)
- ✅ HSTS enabled for production
- ✅ Security headers suite implemented
- ⚠️ Disaster recovery plan needs documentation
- ✅ Refresh token rotation implemented
- ⚠️ Redis AUTH recommended for production

---

## Immediate Action Items

### ✅ Completed

1. ✅ **Environment File Template** (Feb 5, 2026) - Created `.env.example` and updated documentation
2. ✅ **Refresh Token Rotation** (Feb 5, 2026) - Verified already implemented with token reuse detection
3. ✅ **Rate Limiting** (Feb 5, 2026) - Installed `AspNetCoreRateLimit` and configured auth endpoint limits
4. ✅ **CORS Configuration** (Feb 5, 2026) - Removed unused origins, optimized for security
5. ✅ **Content Security Policy** (Feb 5, 2026) - Configured CSP headers in middleware with environment-specific policies (see [SECURITY-HEADERS.md](SECURITY-HEADERS.md))

### Priority 1 (Remaining High Priority Items)

6. **Redis Security (Production)**
   - Enable AUTH with strong password
   - Use TLS for Redis connections
   - Configure in Azure Cache for Redis

### Priority 2 (Medium Priority)

7. **Security Monitoring**
   - Configure Application Insights alerts for failed auth attempts
   - Monitor rate limiting violations
   - Track token reuse detection events

8. **Security Testing**
   - Penetration testing
   - Dependency vulnerability scanning
   - OWASP compliance verification

### Priority 3 (Low Priority)

9. **Security Documentation**
   - Document incident response procedures
   - Create security testing guidelines
   - Add security training materials

---

## References

- [OWASP Top 10 Security Risks](https://owasp.org/www-project-top-ten/)
- [JWT Best Practices](https://tools.ietf.org/html/rfc8725)
- [ASP.NET Core Security](https://learn.microsoft.com/en-us/aspnet/core/security/)
- [CORS Specification](https://developer.mozilla.org/en-US/docs/Web/HTTP/CORS)
- [OAuth 2.0 Security Best Practices](https://datatracker.ietf.org/doc/html/draft-ietf-oauth-security-topics)
