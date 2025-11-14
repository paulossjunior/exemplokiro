---
inclusion: manual
---

# Tailwind CSS Standards

## Utility-First Approach

Use Tailwind utility classes directly in templates:

```vue
<template>
  <!-- Good: Utility classes -->
  <div class="flex items-center justify-between p-4 bg-white rounded-lg shadow-md">
    <h2 class="text-2xl font-bold text-gray-900">Project Name</h2>
    <button class="px-4 py-2 text-white bg-blue-600 rounded hover:bg-blue-700">
      Edit
    </button>
  </div>
</template>
```

## Responsive Design

Use responsive prefixes for mobile-first design:

```vue
<template>
  <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
    <!-- Mobile: 1 column, Tablet: 2 columns, Desktop: 3 columns -->
  </div>

  <h1 class="text-xl md:text-2xl lg:text-3xl">
    <!-- Responsive text sizes -->
  </h1>
</template>
```

## Component Extraction

Extract repeated patterns into components:

```vue
<!-- BaseButton.vue -->
<script setup lang="ts">
interface Props {
  variant?: 'primary' | 'secondary' | 'danger'
  size?: 'sm' | 'md' | 'lg'
  disabled?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  variant: 'primary',
  size: 'md',
  disabled: false
})

const baseClasses = 'font-medium rounded focus:outline-none focus:ring-2 focus:ring-offset-2 transition-colors'

const variantClasses = {
  primary: 'bg-blue-600 text-white hover:bg-blue-700 focus:ring-blue-500',
  secondary: 'bg-gray-200 text-gray-900 hover:bg-gray-300 focus:ring-gray-500',
  danger: 'bg-red-600 text-white hover:bg-red-700 focus:ring-red-500'
}

const sizeClasses = {
  sm: 'px-3 py-1.5 text-sm',
  md: 'px-4 py-2 text-base',
  lg: 'px-6 py-3 text-lg'
}

const disabledClasses = 'opacity-50 cursor-not-allowed'

const buttonClasses = computed(() => [
  baseClasses,
  variantClasses[props.variant],
  sizeClasses[props.size],
  props.disabled && disabledClasses
].filter(Boolean).join(' '))
</script>

<template>
  <button :class="buttonClasses" :disabled="disabled">
    <slot />
  </button>
</template>
```

## Custom Configuration

Extend Tailwind with project-specific values:

```javascript
// tailwind.config.js
module.exports = {
  theme: {
    extend: {
      colors: {
        primary: {
          50: '#eff6ff',
          100: '#dbeafe',
          500: '#3b82f6',
          600: '#2563eb',
          700: '#1d4ed8'
        }
      },
      spacing: {
        '128': '32rem',
        '144': '36rem'
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif']
      }
    }
  }
}
```

## Best Practices

1. **Use semantic color names** in config, not in templates
2. **Group related utilities** for readability
3. **Use @apply sparingly** - prefer utility classes
4. **Leverage JIT mode** for custom values
5. **Use arbitrary values** when needed: `w-[137px]`
6. **Maintain consistent spacing** scale
7. **Use Tailwind plugins** for forms, typography

## Common Patterns

### Card Component

```vue
<template>
  <div class="bg-white rounded-lg shadow-md overflow-hidden">
    <div class="p-6">
      <h3 class="text-lg font-semibold text-gray-900 mb-2">
        {{ title }}
      </h3>
      <p class="text-gray-600">
        {{ description }}
      </p>
    </div>
  </div>
</template>
```

### Form Input

```vue
<template>
  <div class="mb-4">
    <label class="block text-sm font-medium text-gray-700 mb-2">
      {{ label }}
    </label>
    <input
      type="text"
      class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
      :class="{ 'border-red-500': error }"
    />
    <p v-if="error" class="mt-1 text-sm text-red-600">
      {{ error }}
    </p>
  </div>
</template>
```
