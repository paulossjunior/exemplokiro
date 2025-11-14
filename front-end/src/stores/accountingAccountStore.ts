import { defineStore } from 'pinia';
import { ref } from 'vue';
import { accountingAccountService } from '@/services/api/accountingAccountService';
import type { AccountingAccount } from '@/types/api';

/**
 * Accounting Account store for managing accounting accounts with caching
 * Implements 5-minute cache duration to reduce unnecessary API calls
 */
export const useAccountingAccountStore = defineStore('accountingAccounts', () => {
  // State
  const accounts = ref<AccountingAccount[]>([]);
  const lastFetch = ref<number | null>(null);
  
  // Cache duration: 5 minutes in milliseconds
  const CACHE_DURATION = 5 * 60 * 1000;

  // Actions
  /**
   * Fetch accounting accounts with caching logic
   * Returns cached data if still valid, otherwise fetches fresh data from API
   * 
   * @param forceRefresh - If true, bypass cache and fetch fresh data
   * @returns Promise with array of accounting accounts
   */
  async function fetchAccounts(forceRefresh = false): Promise<AccountingAccount[]> {
    const now = Date.now();
    
    // Return cached data if still valid
    if (
      !forceRefresh &&
      accounts.value.length > 0 &&
      lastFetch.value &&
      now - lastFetch.value < CACHE_DURATION
    ) {
      return accounts.value;
    }

    // Fetch fresh data from API
    const data = await accountingAccountService.getAccounts();
    accounts.value = data;
    lastFetch.value = now;
    
    return data;
  }

  /**
   * Clear cache and reset state
   * Useful when accounts are modified and cache needs to be invalidated
   */
  function clearCache() {
    accounts.value = [];
    lastFetch.value = null;
  }

  return {
    // State
    accounts,
    lastFetch,
    // Actions
    fetchAccounts,
    clearCache,
  };
});
