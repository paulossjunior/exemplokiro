<template>
  <Teleport to="body">
    <Transition name="fade">
      <div
        v-if="show"
        class="loading-overlay"
        role="dialog"
        aria-modal="false"
        aria-live="polite"
        aria-busy="true"
        @click.self="handleBackgroundClick"
      >
        <div class="loading-overlay-content" tabindex="-1">
          <LoadingSpinner :size="spinnerSize" :message="message" />
          <p v-if="message" class="loading-overlay-message">
            {{ message }}
          </p>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
import { onMounted, onUnmounted, watch } from 'vue';
import LoadingSpinner from './LoadingSpinner.vue';

interface Props {
  show: boolean;
  message?: string;
  spinnerSize?: 'sm' | 'md' | 'lg' | 'xl';
  allowBackgroundClick?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  message: 'Loading...',
  spinnerSize: 'lg',
  allowBackgroundClick: false,
});

const emit = defineEmits<{
  (e: 'close'): void;
}>();

// Store the previously focused element
let previouslyFocusedElement: HTMLElement | null = null;

// Handle background click
const handleBackgroundClick = () => {
  if (props.allowBackgroundClick) {
    emit('close');
  }
};

// Prevent focus trap by not trapping focus during loading
// This ensures keyboard accessibility and prevents users from being stuck
watch(
  () => props.show,
  (isShowing) => {
    if (isShowing) {
      // Store the currently focused element
      previouslyFocusedElement = document.activeElement as HTMLElement;
      
      // Prevent body scroll
      document.body.style.overflow = 'hidden';
      
      // Announce to screen readers
      const announcement = document.createElement('div');
      announcement.setAttribute('role', 'status');
      announcement.setAttribute('aria-live', 'polite');
      announcement.className = 'sr-only';
      announcement.textContent = props.message || 'Loading...';
      document.body.appendChild(announcement);
      
      setTimeout(() => {
        announcement.remove();
      }, 1000);
    } else {
      // Restore body scroll
      document.body.style.overflow = '';
      
      // Restore focus to previously focused element
      if (previouslyFocusedElement && typeof previouslyFocusedElement.focus === 'function') {
        previouslyFocusedElement.focus();
      }
      previouslyFocusedElement = null;
    }
  }
);

// Cleanup on unmount
onUnmounted(() => {
  if (props.show) {
    document.body.style.overflow = '';
  }
});

// Handle escape key to close (if allowed)
const handleKeydown = (event: KeyboardEvent) => {
  if (event.key === 'Escape' && props.allowBackgroundClick) {
    emit('close');
  }
};

onMounted(() => {
  document.addEventListener('keydown', handleKeydown);
});

onUnmounted(() => {
  document.removeEventListener('keydown', handleKeydown);
});
</script>

<style scoped>
.loading-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999;
  backdrop-filter: blur(2px);
}

.loading-overlay-content {
  background-color: white;
  border-radius: 0.5rem;
  padding: 2rem;
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
  min-width: 200px;
  max-width: 400px;
}

.loading-overlay-message {
  margin: 0;
  color: #374151;
  font-size: 1rem;
  font-weight: 500;
  text-align: center;
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

/* Fade transition */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
