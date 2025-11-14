# OfflineIndicator Component

## Overview

The `OfflineIndicator` component displays a banner at the top of the page when the application detects that the network is offline or the back-end API is unreachable.

## Features

- **Automatic Detection**: Monitors browser online/offline events and API request failures
- **Visual Feedback**: Displays a prominent red banner with offline status
- **Last Connection Time**: Shows when the last successful API request was made
- **Reconnection Status**: Displays an animated spinner indicating reconnection attempts
- **Accessibility**: Uses ARIA live regions for screen reader announcements
- **Smooth Animations**: Slides in/out with smooth transitions

## Usage

The component is automatically included in the root `App.vue` and doesn't require manual integration in individual pages.

```vue
<template>
  <div>
    <OfflineIndicator />
    <RouterView />
  </div>
</template>
```

## Network Status Detection

The offline indicator works in conjunction with:

1. **Browser Events**: Listens to `online` and `offline` events
2. **API Failures**: Tracks failed API requests (2+ consecutive failures trigger offline state)
3. **Network Store**: Uses Pinia store to manage global network state

## Automatic Retry

When offline, the HTTP client automatically:
- Retries failed requests up to 3 times
- Uses exponential backoff (1s, 2s, 4s delays)
- Resumes normal operation when connectivity is restored

## Network Store

Access network status in any component:

```typescript
import { useNetworkStatus } from '@/composables/useNetworkStatus';

const { isOnline, isOffline, timeSinceLastFetch } = useNetworkStatus();
```

## Styling

The component uses Tailwind CSS classes and includes custom transition animations:
- Red background (`bg-red-600`) for high visibility
- Fixed positioning at the top of the viewport
- Slide-down animation on show/hide
- Responsive container layout

## Accessibility

- Uses `role="alert"` for immediate screen reader announcement
- `aria-live="assertive"` ensures priority announcement
- Semantic HTML with descriptive text
- Keyboard accessible (no interactive elements to trap focus)

## Example States

### Offline with Last Connection Time
```
🚫 You are currently offline
   Last successful connection: 5 minutes ago
   Attempting to reconnect... ⟳
```

### Offline without Previous Connection
```
🚫 You are currently offline
   Unable to connect to the server
   Attempting to reconnect... ⟳
```

## Related Components

- `LoadingSpinner`: Shows loading state during API requests
- `ErrorAlert`: Displays API error messages
- Network Store: Manages global network state
