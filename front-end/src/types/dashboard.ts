/**
 * TypeScript type definitions for the Expense Tracking Dashboard
 */

/**
 * Project entity
 */
export interface Project {
  id: string
  name: string
  code: string
  status: string
}

/**
 * Dashboard metrics showing budget consumption
 */
export interface DashboardMetrics {
  consumed: number
  remaining: number
  yield: number
  total: number
  projectId: string
  projectName: string
}

/**
 * Transaction status type
 */
export type TransactionStatus = 'Em Validação' | 'Pendente' | 'Validado' | 'Revisar'

/**
 * Transaction entity
 */
export interface Transaction {
  id: string
  paymentMethod: string
  amount: number
  date: string // ISO 8601 format
  cnpj: string
  companyName?: string
  status: TransactionStatus
  category: string
  projectId: string
}

/**
 * Filter criteria for transactions
 */
export interface FilterCriteria {
  projectId?: string
  searchTerm?: string
  date?: string
  status?: TransactionStatus
  category?: string
  page: number
  pageSize: number
}

/**
 * Paginated response wrapper
 */
export interface PaginatedResponse<T> {
  data: T[]
  currentPage: number
  totalPages: number
  totalResults: number
  pageSize: number
}

/**
 * Pagination state
 */
export interface PaginationState {
  currentPage: number
  totalPages: number
  totalResults: number
  pageSize: number
}
