import httpClient from './httpClient';
import type {
  Transaction,
  CreateTransactionRequest,
  AccountBalance,
} from '@/types/api';

/**
 * TransactionService - API service for transaction management operations
 * 
 * Provides methods to interact with the back-end Transactions API endpoints
 * including transaction creation, history retrieval, and balance calculation.
 * 
 * All transaction operations require authentication via JWT token.
 */
export class TransactionService {
  /**
   * Get transaction history for a project with optional filtering
   * 
   * @param projectId - Project ID
   * @param params - Query parameters for filtering
   * @param params.startDate - Filter by start date (inclusive, ISO format)
   * @param params.endDate - Filter by end date (inclusive, ISO format)
   * @param params.classification - Filter by classification (Debit or Credit)
   * @param params.accountingAccountId - Filter by accounting account ID
   * @returns Promise with array of transactions in chronological order
   * @throws ApiError if project not found (404) or other errors
   */
  async getTransactions(
    projectId: string,
    params?: {
      startDate?: string;
      endDate?: string;
      classification?: string;
      accountingAccountId?: string;
    }
  ): Promise<Transaction[]> {
    const response = await httpClient.get<Transaction[]>(
      `/api/projects/${projectId}/transactions`,
      { params }
    );
    return response.data;
  }

  /**
   * Create a new transaction for a project
   * 
   * Requires authentication. Only the project coordinator can create transactions.
   * All transactions are digitally signed and include cryptographic hashes for integrity.
   * 
   * @param projectId - Project ID
   * @param data - Transaction creation data
   * @returns Promise with created transaction including digital signature
   * @throws ApiError if:
   *   - Authentication fails (401)
   *   - User is not authorized (403)
   *   - Validation fails (400) - negative amount, future date, closed project
   *   - Project or accounting account not found (404)
   */
  async createTransaction(projectId: string, data: CreateTransactionRequest): Promise<Transaction> {
    const response = await httpClient.post<Transaction>(
      `/api/projects/${projectId}/transactions`,
      data
    );
    return response.data;
  }

  /**
   * Get the current balance for a project's bank account
   * 
   * Calculates balance by summing all credits and subtracting all debits.
   * Balance is compared against project budget to identify overspending.
   * 
   * @param projectId - Project ID
   * @returns Promise with account balance details
   * @throws ApiError if project not found (404) or other errors
   */
  async getBalance(projectId: string): Promise<AccountBalance> {
    const response = await httpClient.get<AccountBalance>(
      `/api/projects/${projectId}/transactions/balance`
    );
    return response.data;
  }
}

// Export singleton instance
export const transactionService = new TransactionService();
