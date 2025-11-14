# Offline Handling Implementation

## Overview

This document describes the offline handling implementation for the Project Budget Management System front-end application. The system provides robust network error detection, automatic retry with exponential backoff, and user-friendly offline indicators.

## Architecture

### Components

1. **Network Store** (`src/stores/networkStore.ts`)
   - Tracks online/offline state
   - Records last successful fetch timestamp
   - Counts failed request attempts
   - Listens to browser online/offline events

2. **HTTP Client** (`src/services/api/httpClient.ts`)
   - Detects network errors in response interceptor
   - Marks application as offline after 2+ consecutive failures
   - Records successful fetches to update network state
   - Prevents requests when offline (unless retrying)

3. **Retry Utility** (`src/utils/retryWithBackoff.ts`)
   - Implements exponential backoff algorithm
   - Retries network errors and 5xx server errors
   - Configurable retry attempts (default: 3)
   - Configurable delays (1s → 2s → 4s → 8s, max 10s)

4. **Offline Indicator** (`src/components/common/OfflineIndicator.vue`)
   - Displays banner when offline
   - Shows last successful connection time
   - Indicates reconnection attempts
   - Accessible with ARIA attributes

5. **Network Status Composable** (`src/composables/useNetworkStatus.ts`)
   - Provides reactive network status to components
   - Exposes online/offline state
   - Provides formatted time since last fetch

## Features

### 1. Network Status Detection

The system detects offline status through multiple mechanisms:

- **Browser Events**: Listens to `window.online` and `window.offline` events
- **API Failures**: Marks offline after 2+ consecutive failed requests
- **Network Errors**: Detects when requests fail without a response

### 2. Automatic Retry with Exponential Backoff

Failed requests are automatically retried with increasing delays:

```
Attempt 1: Immediate
Attempt 2: Wait 1 second
Attempt 3: Wait 2 seconds
Attempt 4: Wait 4 seconds (final attempt)
```

**Retryable Errors:**
- Network errors (no response from server)
- Server errors (5xx status codes)
- Timeout errors

**Non-Retryable Errors:**
- Client errors (4xx status codes)
- Validation errors (400)
- Authentication errors (401)
- Authorization errors (403)
- Not found errors (404)

### 3. Offline Indicator

A prominent banner appears at the top of the page when offline:

- **Red background** for high visibility
- **Last connection time** to inform users
- **Reconnection status** with animated spinner
- **Smooth animations** for better UX
- **Accessible** with ARIA live regions

### 4. Last Successful Fetch Timestamp

The system tracks and displays when the last successful API request occurred:

- Stored in network store
- Updated on every successful response
- Displayed in human-readable format (e.g., "5 minutes ago")
- Shown in offline indicator

### 5. Auto-Resume on Connectivity Restoration

When connectivity is restored:

- Browser `online` event triggers state update
- Failed request count resets
- Offline indicator disappears
- Pending requests can proceed
- Normal operation resumes automatically

## Usage

### In Components

Access network status in any component:

```typescript
import { useNetworkStatus } from '@/composables/useNetworkStatus';

const { isOnline, isOffline, timeSinceLastFetch } = useNetworkStatus();
```

### In API Services

The retry logic is automatically applied to all HTTP requests through the `httpClientWithRetry` wrapper:

```typescript
import httpClient, { httpClientWithRetry } from '@/services/api/httpClient';

// Automatic retry enabled
const result = await httpClientWithRetry(() => 
  httpClient.get('/api/projects')
);

// Disable retry for specific requests
const result = await httpClientWithRetry(
  () => httpClient.get('/api/projects'),
  false // disable retry
);
```

### Manual Network Status Check

```typescript
import { useNetworkStore } from '@/stores/networkStore';

const networkStore = useNetworkStore();

if (networkStore.isOffline) {
  console.log('Application is offline');
  console.log('Last fetch:', networkStore.lastFetchTimestamp);
}
```

## Configuration

### Retry Configuration

Modify retry behavior in `src/utils/retryWithBackoff.ts`:

```typescript
const DEFAULT_CONFIG = {
  maxRetries: 3,           // Maximum retry attempts
  initialDelay: 1000,      // Initial delay in ms
  maxDelay: 10000,         // Maximum delay in ms
  backoffMultiplier: 2,    // Exponential multiplier
};
```

### Offline Detection Threshold

Modify the failed request threshold in `src/services/api/httpClient.ts`:

```typescript
// Mark as offline if we have multiple failed requests
if (networkStore.failedRequestCount >= 2) {
  networkStore.setOnline(false);
}
```

## Testing

### Simulate Offline Mode

1. **Browser DevTools**: 
   - Open DevTools → Network tab
   - Select "Offline" from throttling dropdown

2. **Stop Back-End Server**:
   - Stop the API server to simulate unreachable back-end

3. **Programmatically**:
   ```typescript
   const networkStore = useNetworkStore();
   networkStore.setOnline(false);
   ```

### Test Scenarios

1. **Offline Detection**:
   - Go offline → Verify banner appears
   - Make API request → Verify immediate rejection

2. **Retry Logic**:
   - Simulate intermittent failures
   - Verify exponential backoff delays
   - Verify max 3 retry attempts

3. **Auto-Resume**:
   - Go offline → Go online
   - Verify banner disappears
   - Verify requests succeed

4. **Last Fetch Timestamp**:
   - Make successful request
   - Go offline
   - Verify timestamp displayed correctly

## Accessibility

The offline indicator follows WCAG 2.0 Level AA guidelines:

- **ARIA Attributes**: `role="alert"` and `aria-live="assertive"`
- **Screen Reader Support**: Status changes announced immediately
- **Keyboard Navigation**: No focus traps during offline state
- **Visual Contrast**: Red banner with white text (high contrast)
- **Semantic HTML**: Proper heading and paragraph structure

## Performance Considerations

- **Minimal Overhead**: Network status checks are lightweight
- **Event-Driven**: Uses browser events instead of polling
- **Efficient Storage**: Only stores essential state
- **Optimized Retries**: Exponential backoff prevents request flooding

## Limitations

1. **Browser Support**: Requires modern browsers with `navigator.onLine` support
2. **False Positives**: Browser may report online when network is actually unreachable
3. **Retry Delays**: Users may experience delays during retry attempts
4. **No Offline Queue**: Failed requests are not queued for later retry

## Future Enhancements

- **Offline Queue**: Store failed requests and retry when online
- **Service Worker**: Cache responses for offline access
- **Progressive Web App**: Enable full offline functionality
- **Configurable Retry**: Allow per-request retry configuration
- **Network Quality**: Detect slow connections and adjust behavior

## Related Files

- `front-end/src/stores/networkStore.ts` - Network state management
- `front-end/src/services/api/httpClient.ts` - HTTP client with retry logic
- `front-end/src/utils/retryWithBackoff.ts` - Retry utility
- `front-end/src/components/common/OfflineIndicator.vue` - Offline banner
- `front-end/src/composables/useNetworkStatus.ts` - Network status composable

## Requirements Fulfilled

This implementation fulfills the following requirements from the specification:

- **14.1**: Display offline indicator when back-end is unreachable ✓
- **14.2**: Retry failed requests automatically with exponential backoff ✓
- **14.3**: Limit retry attempts to 3 times ✓
- **14.4**: Auto-resume when connectivity is restored ✓
- **14.5**: Display last successful data fetch timestamp ✓
