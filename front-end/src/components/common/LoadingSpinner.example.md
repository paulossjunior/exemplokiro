# LoadingSpinner Component

A reusable loading spinner component with accessibility features and customizable sizes.

## Features

- Multiple size options (sm, md, lg, xl)
- ARIA live region for screen reader announcements
- Customizable loading message
- Tailwind CSS styling
- Smooth animation

## Props

| Prop | Type | Default | Description |
|------|------|---------|-------------|
| `size` | `'sm' \| 'md' \| 'lg' \| 'xl'` | `'md'` | Size of the spinner |
| `message` | `string` | `'Loading...'` | Message announced to screen readers |

## Usage Examples

### Basic Usage

```vue
<template>
  <LoadingSpinner />
</template>

<script setup lang="ts">
import LoadingSpinner from '@/components/common/LoadingSpinner.vue';
</script>
```

### Custom Size

```vue
<template>
  <div>
    <LoadingSpinner size="sm" />
    <LoadingSpinner size="md" />
    <LoadingSpinner size="lg" />
    <LoadingSpinner size="xl" />
  </div>
</template>
```

### Custom Message

```vue
<template>
  <LoadingSpinner 
    size="lg" 
    message="Loading projects..." 
  />
</template>
```

### In a Button

```vue
<template>
  <button 
    :disabled="loading" 
    class="btn btn-primary"
  >
    <LoadingSpinner v-if="loading" size="sm" />
    <span v-else>Submit</span>
  </button>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import LoadingSpinner from '@/components/common/LoadingSpinner.vue';

const loading = ref(false);
</script>
```

### Centered in Container

```vue
<template>
  <div class="flex items-center justify-center min-h-screen">
    <LoadingSpinner size="xl" message="Loading application..." />
  </div>
</template>
```

## Accessibility

- Uses `role="status"` for screen reader announcements
- Implements `aria-live="polite"` for non-intrusive updates
- Provides visually hidden text for screen readers via `sr-only` class
- SVG has `aria-hidden="true"` to prevent redundant announcements
- Customizable message for context-specific announcements

## Styling

The component uses Tailwind CSS for styling:
- Blue color scheme (customizable via color classes)
- Smooth spin animation
- Responsive sizing
- Inline-flex display for easy integration

## Size Reference

- `sm`: 16px (h-4 w-4) - For inline or button usage
- `md`: 32px (h-8 w-8) - Default size for general use
- `lg`: 48px (h-12 w-12) - For prominent loading states
- `xl`: 64px (h-16 w-16) - For full-page or hero sections
