// Export the HTTP client and error handler
export { default as httpClient, handleApiError } from './httpClient';

// Export API service instances
export { projectService } from './projectService';
export { transactionService } from './transactionService';
export { accountingAccountService } from './accountingAccountService';
export { auditService } from './auditService';
export { reportService } from './reportService';

// Re-export types for convenience
export type { ApiError } from '@/types/api';
