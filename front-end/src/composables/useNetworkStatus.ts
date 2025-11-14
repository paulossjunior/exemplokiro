import { computed } from 'vue';
import { useNetworkStore } from '@/stores/networkStore';

/**
 * Composable for accessing network status
 */
export function useNetworkStatus() {
  const networkStore = useNetworkStore();

  const isOnline = computed(() => networkStore.isOnline);
  const isOffline = computed(() => networkStore.isOffline);
  const lastFetchTimestamp = computed(() => networkStore.lastFetchTimestamp);
  const timeSinceLastFetch = computed(() => networkStore.timeSinceLastFetch);

  return {
    isOnline,
    isOffline,
    lastFetchTimestamp,
    timeSinceLastFetch,
  };
}
