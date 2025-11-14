import httpClient from './httpClient';
import type { AuditEntry, IntegrityVerificationResult, PaginatedResponse } from '@/types/api';

/**
 * AuditService - API service for audit trail and integrity verification operations
 * 
 * Provides methods to interact with the back-end Audit API endpoints
 * for viewing audit logs and verifying data integrity.
 */
export class AuditService {
  private readonly basePath = '/api/audit';

  /**
   * Get paginated audit trail with optional filtering
   * 
   * Retrieves audit entries that track all system actions and changes.
   * Useful for compliance, debugging, and security monitoring.
   * 
   * @param params - Query parameters for filtering and pagination
   * @param params.entityType - Filter by entity type (e.g., "Project", "Transaction")
   * @param params.entityId - Filter by specific entity ID
   * @param params.userId - Filter by user who performed the action
   * @param params.startDate - Filter entries from this date (ISO 8601 format)
   * @param params.endDate - Filter entries until this date (ISO 8601 format)
   * @param params.pageNumber - Page number for pagination (default: 1)
   * @param params.pageSize - Number of items per page (default: 10)
   * @returns Promise with paginated audit entry list
   */
  async getAuditTrail(params?: {
    entityType?: string;
    entityId?: string;
    userId?: string;
    startDate?: string;
    endDate?: string;
    pageNumber?: number;
    pageSize?: number;
  }): Promise<PaginatedResponse<AuditEntry>> {
    const response = await httpClient.get<PaginatedResponse<AuditEntry>>(`${this.basePath}/trail`, {
      params,
    });
    return response.data;
  }

  /**
   * Verify data integrity across the system
   * 
   * Performs cryptographic verification of all transactions and audit entries
   * to detect any tampering or data corruption. This is a critical security
   * feature for maintaining accountability.
   * 
   * @returns Promise with integrity verification results
   * @throws ApiError if verification process fails or other errors
   */
  async verifyIntegrity(): Promise<IntegrityVerificationResult> {
    const response = await httpClient.get<IntegrityVerificationResult>(
      `${this.basePath}/integrity`
    );
    return response.data;
  }
}

// Export singleton instance
export const auditService = new AuditService();
