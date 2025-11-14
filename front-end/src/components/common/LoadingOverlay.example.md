# LoadingOverlay Component

A full-page loading overlay component with accessibility features and focus management.

## Features

- Full-page overlay with backdrop blur
- ARIA live region for screen reader announcements
- Prevents body scroll during loading
- Restores focus after loading completes
- Optional background click to close
- Escape key support
- Smooth fade transition
- Tailwind CSS styling
- No focus trap - maintains keyboard accessibility

## Props

| Prop | Type | Default | Description |
|------|------|---------|-------------|
| `show` | `boolean` | required | Controls overlay visibility |
| `message` | `string` | `'Loading...'` | Loading message displayed to users |
| `spinnerSize` | `'sm' \| 'md' \| 'lg' \| 'xl'` | `'lg'` | Size of the loading spinner |
| `allowBackgroundClick` | `boolean` | `false` | Allow closing by clicking background or pressing Escape |

## Events

| Event | Payload | Description |
|-------|---------|-------------|
| `close` | none | Emitted when user attempts to close (background click or Escape key) |

## Usage Examples

### Basic Usage

```vue
<template>
  <div>
    <button @click="loading = true">Start Loading</button>
    <LoadingOverlay :show="loading" @close="loading = false" />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import LoadingOverlay from '@/components/common/LoadingOverlay.vue';

const loading = ref(false);
</script>
```

### With Custom Message

```vue
<template>
  <LoadingOverlay 
    :show="isSubmitting" 
    message="Saving your changes..." 
  />
</template>

<script setup lang="ts">
import { ref } from 'vue';
import LoadingOverlay from '@/components/common/LoadingOverlay.vue';

const isSubmitting = ref(false);
</script>
```

### With API Call

```vue
<template>
  <div>
    <button @click="fetchData">Load Data</button>
    <LoadingOverlay 
      :show="loading" 
      message="Fetching projects..." 
      spinner-size="xl"
    />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import LoadingOverlay from '@/components/common/LoadingOverlay.vue';
import { projectService } from '@/services/api/projectService';

const loading = ref(false);

const fetchData = async () => {
  loading.value = true;
  try {
    await projectService.getProjects();
  } finally {
    loading.value = false;
  }
};
</script>
```

### Allow User to Cancel

```vue
<template>
  <LoadingOverlay 
    :show="loading" 
    message="Processing... Click outside or press Escape to cancel"
    allow-background-click
    @close="handleCancel"
  />
</template>

<script setup lang="ts">
import { ref } from 'vue';
import LoadingOverlay from '@/components/common/LoadingOverlay.vue';

const loading = ref(false);

const handleCancel = () => {
  loading.value = false;
  // Cancel any ongoing operations
};
</script>
```

### With Composable

```vue
<template>
  <div>
    <button @click="createProject">Create Project</button>
    <LoadingOverlay 
      :show="loading" 
      message="Creating project..." 
    />
  </div>
</template>

<script setup lang="ts">
import LoadingOverlay from '@/components/common/LoadingOverlay.vue';
import { useProjects } from '@/composables/useProjects';

const { loading, createProject: createProjectFn } = useProjects();

const createProject = async () => {
  await createProjectFn({
    name: 'New Project',
    // ... other fields
  });
};
</script>
```

## Accessibility

- Uses `role="dialog"` with `aria-modal="false"` to indicate overlay without trapping focus
- Implements `aria-live="polite"` for screen reader announcements
- Sets `aria-busy="true"` to indicate loading state
- Announces loading message to screen readers
- Prevents body scroll during loading
- Restores focus to previously focused element after loading
- Does NOT trap focus - users can still navigate with keyboard
- Supports Escape key to close (when `allowBackgroundClick` is true)
- Prevents focus loss by maintaining focus context

## Focus Management

The component implements smart focus management:

1. **On Show**: Stores the currently focused element
2. **During Loading**: Does NOT trap focus (maintains keyboard accessibility)
3. **On Hide**: Restores focus to the previously focused element

This approach ensures:
- Users are not trapped in the overlay
- Keyboard navigation remains functional
- Focus context is preserved
- Screen readers can navigate normally

## Styling

The component uses Tailwind CSS for styling:
- Semi-transparent black backdrop (50% opacity)
- Backdrop blur effect for visual depth
- White content card with rounded corners
- Box shadow for elevation
- Smooth fade transition
- Responsive padding and sizing
- High z-index (9999) to appear above all content

## Best Practices

1. **Use for long operations**: Only show overlay for operations taking > 500ms
2. **Provide context**: Use descriptive messages ("Saving project..." not just "Loading...")
3. **Don't overuse**: Consider inline spinners for quick operations
4. **Allow cancellation**: Set `allowBackgroundClick` for non-critical operations
5. **Handle errors**: Always hide overlay in finally block or error handlers
6. **Test accessibility**: Verify with screen readers and keyboard navigation

## Common Patterns

### Form Submission

```vue
<template>
  <form @submit.prevent="handleSubmit">
    <!-- form fields -->
    <button type="submit">Submit</button>
  </form>
  <LoadingOverlay :show="submitting" message="Submitting form..." />
</template>
```

### Data Fetching

```vue
<template>
  <div>
    <div v-if="!loading && data">
      <!-- display data -->
    </div>
    <LoadingOverlay :show="loading" message="Loading data..." />
  </div>
</template>
```

### Multi-step Process

```vue
<template>
  <LoadingOverlay 
    :show="processing" 
    :message="currentStep" 
  />
</template>

<script setup lang="ts">
const currentStep = ref('Step 1: Validating...');
// Update currentStep as process progresses
</script>
```
