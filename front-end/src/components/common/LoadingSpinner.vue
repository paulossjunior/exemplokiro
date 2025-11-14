<template>
  <div class="loading-spinner" :class="sizeClass" role="status" aria-live="polite">
    <svg
      class="animate-spin"
      :class="spinnerSizeClass"
      xmlns="http://www.w3.org/2000/svg"
      fill="none"
      viewBox="0 0 24 24"
      aria-hidden="true"
    >
      <circle
        class="opacity-25"
        cx="12"
        cy="12"
        r="10"
        stroke="currentColor"
        stroke-width="4"
      ></circle>
      <path
        class="opacity-75"
        fill="currentColor"
        d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
      ></path>
    </svg>
    <span class="sr-only">{{ message }}</span>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';

interface Props {
  size?: 'sm' | 'md' | 'lg' | 'xl';
  message?: string;
}

const props = withDefaults(defineProps<Props>(), {
  size: 'md',
  message: 'Loading...',
});

const sizeClass = computed(() => {
  const classes = {
    sm: 'text-blue-500',
    md: 'text-blue-600',
    lg: 'text-blue-600',
    xl: 'text-blue-700',
  };
  return classes[props.size];
});

const spinnerSizeClass = computed(() => {
  const classes = {
    sm: 'h-4 w-4',
    md: 'h-8 w-8',
    lg: 'h-12 w-12',
    xl: 'h-16 w-16',
  };
  return classes[props.size];
});
</script>

<style scoped>
.loading-spinner {
  display: inline-flex;
  align-items: center;
  justify-content: center;
}

.sr-only {
  position: absolute;
  width: 1px;
  height: 1px;
  padding: 0;
  margin: -1px;
  overflow: hidden;
  clip: rect(0, 0, 0, 0);
  white-space: nowrap;
  border-width: 0;
}
</style>
