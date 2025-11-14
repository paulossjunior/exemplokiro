import { ref, watch, type Ref } from 'vue';

/**
 * useDebounce - Composable for debouncing reactive values
 * 
 * Delays updating the debounced value until after the specified delay has elapsed
 * since the last change to the input value. Useful for search inputs, form validation,
 * and reducing unnecessary API calls.
 * 
 * @param value - Reactive value to debounce
 * @param delay - Delay in milliseconds (default: 300ms)
 * @returns Debounced reactive value
 * 
 * @example
 * ```typescript
 * const searchQuery = ref('');
 * const debouncedQuery = useDebounce(searchQuery, 500);
 * 
 * // Watch the debounced value for API calls
 * watch(debouncedQuery, (newValue) => {
 *   // This will only fire 500ms after the user stops typing
 *   searchAPI(newValue);
 * });
 * ```
 * 
 * @example
 * ```typescript
 * // In a component
 * const filterText = ref('');
 * const debouncedFilter = useDebounce(filterText, 300);
 * 
 * // Use in computed or watch
 * const filteredItems = computed(() => {
 *   return items.value.filter(item => 
 *     item.name.includes(debouncedFilter.value)
 *   );
 * });
 * ```
 */
export function useDebounce<T>(value: Ref<T>, delay: number = 300): Ref<T> {
  // Initialize with the current value
  const debouncedValue = ref<T>(value.value) as Ref<T>;
  
  // Store timeout reference
  let timeout: ReturnType<typeof setTimeout> | null = null;

  // Watch for changes to the input value
  watch(
    value,
    (newValue) => {
      // Clear existing timeout
      if (timeout !== null) {
        clearTimeout(timeout);
      }

      // Set new timeout
      timeout = setTimeout(() => {
        debouncedValue.value = newValue;
        timeout = null;
      }, delay);
    },
    { immediate: false }
  );

  return debouncedValue;
}
