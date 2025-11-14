import { ref } from 'vue';
import { transactionService } from '@/services/api/transactionService';
import { useApi } from './useApi';
import type { 
  Transaction, 
  CreateTransactionRequest,
  AccountBalance 
} from '@/types/api';

/**
 * useTransactions - Composable for transaction management operations
 * 
 * Provides reactive state and methods for managing transactions including
 * fetching transaction history, creating transactions, and retrieving account balance.
 * 
 * @example
 * ```typescript
 * const { transactions, balance, loading, fetchTransactions, createTransaction } = useTransactions();
 * 
 * // Fetch transactions for a project
 * await fetchTransactions('project-id', {
 *   startDate: '2024-01-01',
 *   endDate: '2024-12-31'
 * });
 * 
 * // Create a new transaction
 * await createTransaction('project-id', {
 *   amount: 1000,
 *   date: '2024-11-14',
 *   classification: 'Debit',
 *   accountingAccountId: 'account-id',
 *   description: 'Office supplies'
 * });
 * 
 * // Get account balance
 * await fetchBalance('project-id');
 * ```
 */
export function useTransactions() {
  // State
  const transactions = ref<Transaction[]>([]);
  const balance = ref<AccountBalance | null>(null);

  // Fetch transactions with filtering
  const { 
    loading: loadingTransactions, 
    error: transactionsError,
    execute: executeFetchTransactions 
  } = useApi(
    async (
      projectId: string,
      params?: {
        startDate?: string;
        endDate?: string;
        classification?: string;
        accountingAccountId?: string;
      }
    ) => {
      const data = await transactionService.getTransactions(projectId, params);
      transactions.value = data;
      return data;
    }
  );

  // Create transaction
  const { 
    loading: creatingTransaction, 
    error: createError,
    execute: executeCreateTransaction 
  } = useApi(
    async (projectId: string, data: CreateTransactionRequest) => {
      const transaction = await transactionService.createTransaction(projectId, data);
      
      // Add to transactions array (prepend for chronological order)
      transactions.value.unshift(transaction);
      
      return transaction;
    }
  );

  // Fetch balance
  const { 
    loading: loadingBalance, 
    error: balanceError,
    execute: executeFetchBalance 
  } = useApi(
    async (projectId: string) => {
      const data = await transactionService.getBalance(projectId);
      balance.value = data;
      return data;
    }
  );

  // Public methods
  const fetchTransactions = (
    projectId: string,
    params?: {
      startDate?: string;
      endDate?: string;
      classification?: string;
      accountingAccountId?: string;
    }
  ) => {
    return executeFetchTransactions(projectId, params);
  };

  const createTransaction = (projectId: string, data: CreateTransactionRequest) => {
    return executeCreateTransaction(projectId, data);
  };

  const fetchBalance = (projectId: string) => {
    return executeFetchBalance(projectId);
  };

  return {
    // State
    transactions,
    balance,
    
    // Loading states
    loadingTransactions,
    creatingTransaction,
    loadingBalance,
    
    // Error states
    transactionsError,
    createError,
    balanceError,
    
    // Methods
    fetchTransactions,
    createTransaction,
    fetchBalance,
  };
}
