import { defineStore } from 'pinia';
import { ref, computed } from 'vue';

export const useNetworkStore = defineStore('network', () => {
  const isOnline = ref(navigator.onLine);
  const lastSuccessfulFetch = ref<Date | null>(null);
  const failedRequestCount = ref(0);

  const isOffline = computed(() => !isOnline.value);
  
  const lastFetchTimestamp = computed(() => {
    if (!lastSuccessfulFetch.value) return null;
    return lastSuccessfulFetch.value.toISOString();
  });

  const timeSinceLastFetch = computed(() => {
    if (!lastSuccessfulFetch.value) return null;
    const now = new Date();
    const diff = now.getTime() - lastSuccessfulFetch.value.getTime();
    const seconds = Math.floor(diff / 1000);
    const minutes = Math.floor(seconds / 60);
    const hours = Math.floor(minutes / 60);
    
    if (hours > 0) {
      return `${hours} hour${hours > 1 ? 's' : ''} ago`;
    } else if (minutes > 0) {
      return `${minutes} minute${minutes > 1 ? 's' : ''} ago`;
    } else {
      return `${seconds} second${seconds !== 1 ? 's' : ''} ago`;
    }
  });

  function setOnline(online: boolean) {
    isOnline.value = online;
    if (online) {
      failedRequestCount.value = 0;
    }
  }

  function recordSuccessfulFetch() {
    lastSuccessfulFetch.value = new Date();
    failedRequestCount.value = 0;
  }

  function recordFailedRequest() {
    failedRequestCount.value++;
  }

  function resetFailedCount() {
    failedRequestCount.value = 0;
  }

  // Set up event listeners for online/offline events
  if (typeof window !== 'undefined') {
    window.addEventListener('online', () => setOnline(true));
    window.addEventListener('offline', () => setOnline(false));
  }

  return {
    isOnline,
    isOffline,
    lastSuccessfulFetch,
    lastFetchTimestamp,
    timeSinceLastFetch,
    failedRequestCount,
    setOnline,
    recordSuccessfulFetch,
    recordFailedRequest,
    resetFailedCount,
  };
});
