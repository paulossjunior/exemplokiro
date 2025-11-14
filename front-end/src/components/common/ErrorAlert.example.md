# ErrorAlert Component Usage

## Overview

The `ErrorAlert` component displays API errors with support for field-specific validation errors and retry functionality. It follows WCAG 2.0 Level AA accessibility standards.

## Features

- Prominent error message display
- Field-specific validation errors
- Optional retry button
- ARIA role="alert" for screen reader announcements
- Tailwind CSS styling
- Responsive design

## Props

```typescript
interface Props {
  error: ApiError | null
  onRetry?: () => void
}
```

### ApiError Type

```typescript
interface ApiError {
  message: string
  errors?: Record<string, string[]>
  statusCode: number
}
```

## Usage Examples

### Basic Error Display

```vue
<script setup lang="ts">
import { ref } from 'vue'
import ErrorAlert from '@/components/common/ErrorAlert.vue'
import type { ApiError } from '@/types/api'

const error = ref<ApiError | null>({
  message: 'Failed to load data',
  statusCode: 500
})
</script>

<template>
  <ErrorAlert :error="error" />
</template>
```

### Error with Validation Details

```vue
<script setup lang="ts">
import { ref } from 'vue'
import ErrorAlert from '@/components/common/ErrorAlert.vue'
import type { ApiError } from '@/types/api'

const error = ref<ApiError | null>({
  message: 'Validation failed',
  errors: {
    Name: ['The Name field is required.'],
    BudgetAmount: ['Budget amount must be greater than 0.']
  },
  statusCode: 400
})
</script>

<template>
  <ErrorAlert :error="error" />
</template>
```

### Error with Retry Button

```vue
<script setup lang="ts">
import { ref } from 'vue'
import ErrorAlert from '@/components/common/ErrorAlert.vue'
import type { ApiError } from '@/types/api'

const error = ref<ApiError | null>(null)

const fetchData = async () => {
  try {
    // API call here
  } catch (err) {
    error.value = {
      message: 'Failed to fetch data',
      statusCode: 500
    }
  }
}

const handleRetry = () => {
  error.value = null
  fetchData()
}
</script>

<template>
  <ErrorAlert :error="error" :on-retry="handleRetry" />
</template>
```

### Integration with useApi Composable

```vue
<script setup lang="ts">
import { ref } from 'vue'
import ErrorAlert from '@/components/common/ErrorAlert.vue'
import { useApi } from '@/composables/useApi'
import { projectService } from '@/services/api/projectService'

const { data, error, loading, execute } = useApi(
  () => projectService.getProjects()
)

const handleRetry = () => {
  execute()
}
</script>

<template>
  <div>
    <ErrorAlert :error="error" :on-retry="handleRetry" />
    
    <div v-if="loading">Loading...</div>
    <div v-else-if="data">
      <!-- Display data -->
    </div>
  </div>
</template>
```

## Accessibility Features

- **role="alert"**: Announces errors to screen readers immediately
- **aria-live="assertive"**: Ensures screen readers interrupt to announce errors
- **aria-label**: Provides descriptive labels for interactive elements
- **Keyboard Navigation**: Retry button is fully keyboard accessible
- **Focus Management**: Proper focus indicators on interactive elements
- **Color Contrast**: Meets WCAG 2.0 Level AA contrast requirements

## Styling

The component uses Tailwind CSS with the following color scheme:

- **Background**: `bg-red-50` (light red background)
- **Border**: `border-red-300` (red border)
- **Text**: `text-red-800` (dark red for main message), `text-red-700` (medium red for details)
- **Icon**: `text-red-600` (red icon)
- **Button**: `bg-red-600` with `hover:bg-red-700` (red button with hover state)

## Best Practices

1. **Clear Error Messages**: Provide user-friendly error messages
2. **Field-Specific Errors**: Include validation errors for form fields
3. **Retry Logic**: Implement retry functionality for transient errors
4. **Error Clearing**: Clear errors when retrying or navigating away
5. **Logging**: Log errors for debugging (already handled in httpClient)

## Error Handling Flow

```
API Call → Error Occurs → useApi Catches Error → 
Transforms to ApiError → Component Displays Error → 
User Clicks Retry → execute() Called → Error Cleared
```
