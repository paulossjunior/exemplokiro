import httpClient from './httpClient';
import type { AccountingAccount, CreateAccountingAccountRequest } from '@/types/api';

/**
 * AccountingAccountService - API service for accounting account operations
 * 
 * Provides methods to interact with the back-end Accounting Accounts API endpoints
 * for managing chart of accounts.
 */
export class AccountingAccountService {
  private readonly basePath = '/api/accounting-accounts';

  /**
   * Get all accounting accounts
   * 
   * Retrieves the complete list of accounting accounts available in the system.
   * This is typically used to populate dropdowns and selection lists.
   * 
   * @returns Promise with array of accounting accounts
   */
  async getAccounts(): Promise<AccountingAccount[]> {
    const response = await httpClient.get<AccountingAccount[]>(this.basePath);
    return response.data;
  }

  /**
   * Get a single accounting account by ID
   * 
   * @param id - Accounting account ID
   * @returns Promise with accounting account details
   * @throws ApiError if account not found (404) or other errors
   */
  async getAccount(id: string): Promise<AccountingAccount> {
    const response = await httpClient.get<AccountingAccount>(`${this.basePath}/${id}`);
    return response.data;
  }

  /**
   * Create a new accounting account
   * 
   * @param data - Accounting account creation data
   * @param data.name - Account name
   * @param data.identifier - Unique account identifier/code
   * @returns Promise with created accounting account
   * @throws ApiError if validation fails (400), duplicate identifier (409), or other errors
   */
  async createAccount(data: CreateAccountingAccountRequest): Promise<AccountingAccount> {
    const response = await httpClient.post<AccountingAccount>(this.basePath, data);
    return response.data;
  }
}

// Export singleton instance
export const accountingAccountService = new AccountingAccountService();
