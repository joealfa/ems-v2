# Authentication Cookie Migration

**Date:** January 25, 2026  
**Status:** ✅ Completed

## Problem

The frontend was storing refresh tokens in `localStorage`, which is insecure and redundant since the backend was already setting refresh tokens as HttpOnly cookies.

### Security Issue

- **Refresh tokens** are long-lived and sensitive credentials
- Storing them in `localStorage` exposes them to XSS attacks
- JavaScript can read `localStorage`, making tokens vulnerable

## Solution

Refactored the authentication system to properly use **HttpOnly cookies** for refresh tokens:

1. **Access tokens**: Stored in `localStorage` (short-lived, needed for API calls)
2. **Refresh tokens**: Stored in HttpOnly cookies (long-lived, managed by browser)

## Changes Made

### 1. API Configuration ([src/api/config.ts](../application/src/api/config.ts))

- ✅ Added `withCredentials: true` to axios instance (enables cookie sending)
- ✅ Removed `REFRESH_TOKEN_KEY` constant
- ✅ Updated token refresh to send empty body (cookie auto-sent)
- ✅ Removed refresh token from `clearAuthStorage()`

### 2. Auth Context ([src/contexts/AuthContext.tsx](../application/src/contexts/AuthContext.tsx))

- ✅ Removed `REFRESH_TOKEN_KEY` constant
- ✅ Updated `saveAuthData()` - no longer stores refresh token
- ✅ Updated `clearAuthData()` - no longer removes refresh token
- ✅ Updated `refreshToken()` - sends empty body, cookie auto-sent
- ✅ Updated `logout()` - sends empty body for revoke, cookie auto-sent

## How It Works Now

### Login Flow

1. User logs in via Google OAuth
2. Backend responds with:
   - `accessToken` in response body ➜ stored in localStorage
   - `refreshToken` as HttpOnly cookie ➜ automatically stored by browser
   - `user` info in response body ➜ stored in localStorage
3. Frontend only stores `accessToken`, `user`, and `tokenExpiry` in localStorage

### API Request Flow

1. Request is made with `Authorization: Bearer <accessToken>`
2. Browser automatically includes `refreshToken` cookie
3. If access token expires (401), interceptor triggers refresh
4. Refresh request sent with cookie (no body needed)
5. New access token received and stored

### Logout Flow

1. Logout endpoint called with empty body
2. Browser automatically sends refresh token cookie
3. Backend revokes token and clears cookie
4. Frontend clears localStorage

## Backend Configuration

The backend ([AuthController.cs](../server/EmployeeManagementSystem.Api/v1/Controllers/AuthController.cs)) already had proper cookie support:

```csharp
private void SetRefreshTokenCookie(string refreshToken)
{
    var cookieOptions = new CookieOptions
    {
        HttpOnly = true,        // JavaScript cannot access
        Secure = true,          // HTTPS only
        SameSite = SameSiteMode.Strict,  // CSRF protection
        Expires = DateTime.UtcNow.AddDays(7)
    };
    Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
}
```

## Testing

To verify the changes:

1. **Clear existing data:**
   ```javascript
   localStorage.clear()
   ```

2. **Login again** - check Application tab in DevTools:
   - ✅ `localStorage`: Should have `accessToken`, `user`, `tokenExpiry`
   - ❌ `localStorage`: Should NOT have `refreshToken`
   - ✅ `Cookies`: Should have `refreshToken` cookie (HttpOnly)

3. **Make API requests** - cookies sent automatically
4. **Logout** - cookie cleared by server

## Benefits

- 🔒 **More Secure**: Refresh tokens protected from XSS attacks
- 🍪 **Best Practice**: Following OAuth2 security recommendations
- 🎯 **Simpler Code**: Browser handles cookie management
- 🚀 **No Breaking Changes**: API already supported this pattern

## Dev Auth Utility

The development authentication utility ([devAuth.ts](../application/src/utils/devAuth.ts)) has been updated to align with the cookie-based authentication:

- ✅ `generateAndStoreDevToken()` only stores `accessToken`, `user`, and `tokenExpiry`
- ✅ Does NOT store refresh tokens in localStorage
- ✅ Follows same storage pattern as production OAuth flow
- ✅ Console helper `window.devLogin()` available in dev mode

Dev tokens are short-lived and used only for local testing, so they don't use refresh tokens.

## References

- [OWASP Token Storage Best Practices](https://cheatsheetseries.owasp.org/cheatsheets/JSON_Web_Token_for_Java_Cheat_Sheet.html#token-storage-on-client-side)
- [OAuth2 Security Best Current Practice](https://datatracker.ietf.org/doc/html/draft-ietf-oauth-security-topics)
