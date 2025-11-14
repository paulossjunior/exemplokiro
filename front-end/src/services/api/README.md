# API Services

This directory contains the HTTP client configuration and API service classes for communicating with the back-end API.

## HTTP Client (`httpClient.ts`)

The HTTP client is a configured Axios instance with the following features:

### Configuration

- **Base URL**: Configured via `VITE_API_BASE_URL` environment variable (defaults to `http://localhost:5000`)
- **Timeout**: 30 seconds (30000ms)
- **Default Headers**: `Content-Type: application/json`

### Request Interceptor

Automatically attaches authentication tokens to all requests:

- Reads JWT token from `localStorage` (key: `auth_token`)
- Adds `Authorization: Bearer <token>` header if token exists
- Logs requests in development mode (when `VITE_ENABLE_API_LOGGING` is not `false`)

### Response Interceptor

Handles responses and errors:

- Logs successful responses in development mode
- Handles 401 (Unauthorized) errors by:
  - Clearing the auth token from localStorage
  - Redirecting to `/login` with return URL
- Logs all errors in development mode
- Provides detailed error information for debugging

### Error Handling

The `handleApiError` function transforms Axios errors into typed `ApiError` objects:

```typescript
interface ApiError {
  message: string;
  errors?: Record<string, string[]>; // Field-specific validation errors
  statusCode: number;
}
```

## Usage

### Basic Usage

```typescript
import httpClient from '@/services/api/httpClient';

// GET request
const response = await httpClient.get('/api/projects');

// POST request
const response = await httpClient.post('/api/projects', {
  name: 'New Project',
  description: 'Project description',
});

// PUT request
const response = await httpClient.put('/api/projects/123', {
  name: 'Updated Project',
});

// DELETE request
const response = await httpClient.delete('/api/projects/123');
```

### Error Handling

```typescript
import httpClient, { handleApiError } from '@/services/api/httpClient';

try {
  const response = await httpClient.get('/api/projects');
  console.log(response.data);
} catch (error) {
  const apiError = handleApiError(error);
  console.error(apiError.message);
  
  // Display field-specific errors
  if (apiError.errors) {
    Object.entries(apiError.errors).forEach(([field, messages]) => {
      console.error(`${field}: ${messages.join(', ')}`);
    });
  }
}
```

### With Authentication

The HTTP client automatically includes the authentication token if it exists in localStorage:

```typescript
// Login and store token
localStorage.setItem('auth_token', 'your-jwt-token');

// All subsequent requests will include the token
const response = await httpClient.get('/api/projects');
// Request headers will include: Authorization: Bearer your-jwt-token
```

## Environment Variables

Configure the HTTP client using environment variables:

### Development (`.env.development`)

```env
VITE_API_BASE_URL=http://localhost:5000
VITE_ENABLE_API_LOGGING=true
```

### Production (`.env.production`)

```env
VITE_API_BASE_URL=https://api.projectbudget.com
VITE_ENABLE_API_LOGGING=false
```

## Development Logging

When `VITE_ENABLE_API_LOGGING` is `true` (or not set to `false`) in development mode, the HTTP client logs:

- **Request logs**: Method, URL, params, data, headers
- **Response logs**: Status, data
- **Error logs**: URL, method, status, error data, message

Example console output:

```
[API Request] GET /api/projects { params: { status: 'InProgress' }, ... }
[API Response] GET /api/projects { status: 200, data: [...] }
```

## Security Considerations

- JWT tokens are stored in `localStorage` for persistence across sessions
- Tokens are automatically cleared on 401 (Unauthorized) responses
- All requests use HTTPS in production (configured via `VITE_API_BASE_URL`)
- CORS is handled by the back-end API

## Next Steps

After setting up the HTTP client, you can:

1. Create API service classes (e.g., `projectService.ts`, `transactionService.ts`)
2. Implement composables for state management (e.g., `useProjects`, `useTransactions`)
3. Create Pinia stores for application-wide state
4. Build UI components that consume the API services
