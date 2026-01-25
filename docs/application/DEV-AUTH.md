# Development Authentication

This document describes how to use the development JWT token generator to bypass Google OAuth during development.

## Overview

In development mode, you can generate JWT tokens without going through the Google OAuth flow. This is useful for:
- Quick testing without setting up OAuth
- Automated testing
- Local development when OAuth is not available

⚠️ **Important**: The dev token endpoint is only available when the backend is running in DEBUG mode and will not be compiled into production builds.

## Backend Setup

### DevAuthController

The `DevAuthController.cs` is wrapped in `#if DEBUG` directives, ensuring it's only compiled in DEBUG builds.

**Endpoint**: `POST /api/v1/dev/devauth/token`

**Request Body** (all optional):
```json
{
  "userId": "custom-user-id",
  "email": "test@example.com",
  "name": "Test User"
}
```

**Response**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2026-01-26T08:00:00Z",
  "userId": "custom-user-id",
  "email": "test@example.com",
  "name": "Test User"
}
```

### Default Values

If you don't provide user information, the following defaults are used:
- **userId**: `dev-user-123`
- **email**: `dev@example.com`
- **name**: `Dev User`

## Frontend Usage

### Method 1: Quick Console Login (Easiest)

In your browser console, run:
```javascript
await window.devLogin()
```

Then refresh the page. You'll be automatically authenticated!

### Method 2: Programmatic Usage

```typescript
import { generateAndStoreDevToken } from '@/utils/devAuth';

// Generate with default values
await generateAndStoreDevToken();

// Generate with custom user info
await generateAndStoreDevToken({
  userId: 'custom-123',
  email: 'custom@example.com',
  name: 'Custom User'
});
```

### Method 3: Direct API Call

Using curl or any HTTP client:
```bash
curl -X POST https://localhost:7001/api/v1/dev/devauth/token \
  -H "Content-Type: application/json" \
  -d '{}'
```

Then use the returned token in the Authorization header:
```bash
curl -X GET https://localhost:7001/api/v1/persons \
  -H "Authorization: Bearer <your-token>"
```

## Testing in Swagger

1. Start your backend in DEBUG mode
2. Navigate to Swagger UI (https://localhost:7001/swagger)
3. Find the `dev/DevAuth` section
4. Use `POST /api/v1/dev/devauth/token` to generate a token
5. Copy the token from the response
6. Click the "Authorize" button at the top
7. Enter `Bearer <your-token>` in the Bearer authentication field
8. Test your authenticated endpoints!

## Security Notes

✅ **Safe for Development**:
- Only compiled in DEBUG builds
- Not available in production
- Clearly marked in logs and Swagger

❌ **Never Use in Production**:
- The `#if DEBUG` directive ensures this code is completely removed from Release builds
- Even if someone tries to call the endpoint in production, it won't exist

## Troubleshooting

### "Failed to generate dev token: 404 Not Found"

The backend might be running in Release mode. Ensure you're running in DEBUG/Development mode:

```bash
dotnet run --project EmployeeManagementSystem.Api --launch-profile https
```

Or in Visual Studio: Select "Debug" configuration instead of "Release".

### "Dev token generation is only available in development mode"

The frontend is running in production mode. Ensure:
- You're using `npm run dev` (not `npm run build` + `npm run preview`)
- Environment variable `NODE_ENV` is not set to `production`

### Token Expires Quickly

Development tokens expire after 8 hours by default. You can:
- Generate a new token when it expires
- Modify the expiry time in `DevAuthController.cs` (line with `AddHours(8)`)

## Example Workflow

1. **Start Backend**: `cd server && dotnet run --project EmployeeManagementSystem.Api`
2. **Start Frontend**: `cd application && npm run dev`
3. **Open Browser**: Navigate to `http://localhost:5173`
4. **Open Console**: Press F12
5. **Login**: Type `await window.devLogin()` and press Enter
6. **Refresh**: Reload the page
7. **You're In**: Start testing! 🎉
