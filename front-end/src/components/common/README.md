# Common Components

This directory contains reusable UI components used throughout the application.

## Components

### ErrorAlert
Displays API error messages with field-specific validation errors.

**Usage:**
```vue
<ErrorAlert :error="error" :onRetry="handleRetry" />
```

**Props:**
- `error: ApiError | null` - Error object to display
- `onRetry?: () => void` - Optional callback for retry button

**Features:**
- Displays error message prominently
- Shows field-specific validation errors
- Optional retry button
- ARIA role="alert" for accessibility

### LoadingSpinner
Displays a loading indicator during async operations.

**Usage:**
```vue
<LoadingSpinner :loading="isLoading" message="Loading projects..." />
```

**Props:**
- `loading: boolean` - Whether to show the spinner
- `message?: string` - Optional loading message

**Features:**
- Animated spinner
- ARIA live region for screen readers
- Customizable message

### LoadingOverlay
Full-page loading overlay for blocking operations.

**Usage:**
```vue
<LoadingOverlay :loading="isLoading" message="Saving..." />
```

**Props:**
- `loading: boolean` - Whether to show the overlay
- `message?: string` - Optional loading message

**Features:**
- Full-page overlay with backdrop
- Prevents user interaction during loading
- Accessible with ARIA attributes
- Prevents focus trap

### OfflineIndicator
Displays a banner when the application is offline or the back-end is unreachable.

**Usage:**
```vue
<OfflineIndicator />
```

**Props:** None (uses global network store)

**Features:**
- Automatic network status detection
- Shows last successful connection time
- Displays reconnection status
- Smooth slide-down animations
- WCAG 2.0 Level AA accessible

**Note:** This component is automatically included in `App.vue` and doesn't need to be added to individual pages.

### StatusBadge
Displays a colored badge for status values (e.g., project status).

**Usage:**
```vue
<StatusBadge :status="project.status" />
```

**Props:**
- `status: string` - Status value to display

**Features:**
- Color-coded badges
- Supports multiple status types
- Accessible with proper contrast

## Styling

All components use Tailwind CSS utility classes for styling. They follow the application's design system and are responsive by default.

## Accessibility

All components follow WCAG 2.0 Level AA guidelines:
- Proper ARIA attributes
- Keyboard navigation support
- Screen reader compatibility
- Sufficient color contrast
- Semantic HTML

## Examples

See individual `.example.md` files for detailed usage examples:
- `ErrorAlert.example.md`
- `LoadingSpinner.example.md`
- `LoadingOverlay.example.md`
- `OfflineIndicator.example.md`
