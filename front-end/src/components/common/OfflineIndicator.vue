<template>
  <Transition name="slide-down">
    <div
      v-if="isOffline"
      class="fixed top-0 left-0 right-0 z-50 bg-red-600 text-white px-4 py-3 shadow-lg"
      role="alert"
      aria-live="assertive"
    >
      <div class="container mx-auto flex items-center justify-between">
        <div class="flex items-center space-x-3">
          <svg
            class="w-6 h-6 flex-shrink-0"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
            xmlns="http://www.w3.org/2000/svg"
            aria-hidden="true"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M18.364 5.636a9 9 0 010 12.728m0 0l-2.829-2.829m2.829 2.829L21 21M15.536 8.464a5 5 0 010 7.072m0 0l-2.829-2.829m-4.243 2.829a4.978 4.978 0 01-1.414-2.83m-1.414 5.658a9 9 0 01-2.167-9.238m7.824 2.167a1 1 0 111.414 1.414m-1.414-1.414L3 3m8.293 8.293l1.414 1.414"
            />
          </svg>
          <div>
            <p class="font-semibold">You are currently offline</p>
            <p v-if="timeSinceLastFetch" class="text-sm text-red-100">
              Last successful connection: {{ timeSinceLastFetch }}
            </p>
            <p v-else class="text-sm text-red-100">
              Unable to connect to the server
            </p>
          </div>
        </div>
        <div class="flex items-center space-x-2">
          <span class="text-sm">Attempting to reconnect...</span>
          <div class="animate-spin rounded-full h-5 w-5 border-b-2 border-white"></div>
        </div>
      </div>
    </div>
  </Transition>
</template>

<script setup lang="ts">
import { useNetworkStatus } from '@/composables/useNetworkStatus';

const { isOffline, timeSinceLastFetch } = useNetworkStatus();
</script>

<style scoped>
.slide-down-enter-active,
.slide-down-leave-active {
  transition: transform 0.3s ease-out, opacity 0.3s ease-out;
}

.slide-down-enter-from {
  transform: translateY(-100%);
  opacity: 0;
}

.slide-down-leave-to {
  transform: translateY(-100%);
  opacity: 0;
}
</style>
