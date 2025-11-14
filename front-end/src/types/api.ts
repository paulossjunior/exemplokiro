// Common API types
export interface ApiResponse<T> {
  data: T;
  message?: string;
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

export interface ApiError {
  message: string;
  errors?: Record<string, string[]>;
  statusCode: number;
}

// Project types
export interface Project {
  id: string;
  name: string;
  description: string | null;
  startDate: string;
  endDate: string;
  status: ProjectStatus;
  budgetAmount: number;
  coordinatorId: string;
  bankAccount: BankAccount | null;
  createdAt: string;
  updatedAt: string;
}

export enum ProjectStatus {
  NotStarted = 'NotStarted',
  InProgress = 'InProgress',
  Completed = 'Completed',
  Cancelled = 'Cancelled',
}

export interface BankAccount {
  id: string;
  accountNumber: string;
  bankName: string;
  branchNumber: string;
  accountHolderName: string;
}

export interface BankAccountRequest {
  accountNumber: string;
  bankName: string;
  branchNumber: string;
  accountHolderName: string;
}

export interface CreateProjectRequest {
  name: string;
  description?: string | null;
  startDate: string;
  endDate: string;
  budgetAmount: number;
  coordinatorId: string;
  bankAccount?: BankAccountRequest | null;
}

export interface UpdateProjectRequest {
  name: string;
  description?: string | null;
  startDate: string;
  endDate: string;
  budgetAmount: number;
  coordinatorId: string;
}

export interface UpdateProjectStatusRequest {
  status: string;
}

// Transaction types
export interface Transaction {
  id: string;
  amount: number;
  date: string;
  classification: string;
  digitalSignature: string;
  dataHash: string;
  bankAccountId: string;
  accountingAccountId: string;
  createdAt: string;
  createdBy: string;
}

export enum TransactionClassification {
  Debit = 'Debit',
  Credit = 'Credit',
}

export interface AccountingAccount {
  id: string;
  name: string;
  identifier: string;
  createdAt: string;
}

export interface CreateAccountingAccountRequest {
  name: string;
  identifier: string;
}

export interface CreateTransactionRequest {
  amount: number;
  date: string;
  classification: string;
  accountingAccountId: string;
}

export interface AccountBalance {
  bankAccountId: string;
  balance: number;
  totalCredits: number;
  totalDebits: number;
  transactionCount: number;
  calculatedAt: string;
}

// Audit types
export interface AuditEntry {
  id: string;
  userId: string;
  actionType: string;
  entityType: string;
  entityId: string;
  timestamp: string;
  previousValue: string | null;
  newValue: string | null;
  digitalSignature: string;
  dataHash: string;
}

export interface IntegrityVerificationResult {
  verificationTimestamp: string;
  totalTransactionsChecked: number;
  tamperedTransactionIds: string[];
  totalAuditEntriesChecked: number;
  tamperedAuditEntryIds: string[];
  isIntegrityValid: boolean;
}

// Report types
export interface GenerateReportRequest {
  includeAuditTrail: boolean;
  startDate?: string;
  endDate?: string;
}

export interface AccountabilityReportResponse {
  id: string;
  reportIdentifier: string;
  projectId: string;
  projectName: string;
  projectDescription: string | null;
  projectCoordinator: string;
  budgetAmount: number;
  currentBalance: number;
  bankAccountNumber: string;
  bankName: string;
  branchNumber: string;
  accountHolderName: string;
  transactions: Transaction[];
  auditEntries: AuditEntry[];
  integrityReport: IntegrityVerificationResult;
  generatedAt: string;
}
