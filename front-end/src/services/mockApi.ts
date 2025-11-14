/**
 * Mock API service for testing the Expense Tracking Dashboard
 * This service simulates API calls using local JSON files
 */

export interface BudgetSummary {
  consumed: number
  remaining: number
  yield: number
  total: number
  consumedPercentage: number
}

export interface Project {
  id: string
  name: string
}

export interface Category {
  id: string
  name: string
}

export interface Status {
  id: string
  name: string
  color: 'blue' | 'red' | 'green' | 'orange'
}

export interface Transaction {
  id: string
  paymentMethod: string
  value: number
  date: string
  cnpj: string
  status: string
  category: string
  projectId: string
}

export interface DashboardData {
  budgetSummary: BudgetSummary
  projects: Project[]
  categories: Category[]
  statuses: Status[]
}

export interface TransactionsResponse {
  transactions: Transaction[]
  pagination: {
    currentPage: number
    totalPages: number
    pageSize: number
    totalResults: number
  }
}

export interface TransactionFilters {
  projectId?: string
  search?: string
  date?: string
  status?: string
  category?: string
  page?: number
  pageSize?: number
}

/**
 * Fetches dashboard data including budget summary, projects, categories, and statuses
 */
export async function fetchDashboardData(): Promise<DashboardData> {
  try {
    const response = await fetch('/mock-api/dashboard-data.json')
    if (!response.ok) {
      throw new Error('Failed to fetch dashboard data')
    }
    return await response.json()
  } catch (error) {
    console.error('Error fetching dashboard data:', error)
    throw error
  }
}

/**
 * Fetches transactions with optional filters
 */
export async function fetchTransactions(
  filters: TransactionFilters = {}
): Promise<TransactionsResponse> {
  try {
    const response = await fetch('/mock-api/transactions.json')
    if (!response.ok) {
      throw new Error('Failed to fetch transactions')
    }
    
    const data: TransactionsResponse = await response.json()
    
    // Apply filters
    let filteredTransactions = [...data.transactions]
    
    // Filter by project
    if (filters.projectId) {
      filteredTransactions = filteredTransactions.filter(
        t => t.projectId === filters.projectId
      )
    }
    
    // Filter by search (searches in payment method and CNPJ)
    if (filters.search) {
      const searchLower = filters.search.toLowerCase()
      filteredTransactions = filteredTransactions.filter(
        t => 
          t.paymentMethod.toLowerCase().includes(searchLower) ||
          t.cnpj.toLowerCase().includes(searchLower)
      )
    }
    
    // Filter by date
    if (filters.date) {
      filteredTransactions = filteredTransactions.filter(
        t => t.date.startsWith(filters.date!)
      )
    }
    
    // Filter by status
    if (filters.status) {
      filteredTransactions = filteredTransactions.filter(
        t => t.status === filters.status
      )
    }
    
    // Filter by category
    if (filters.category) {
      filteredTransactions = filteredTransactions.filter(
        t => t.category === filters.category
      )
    }
    
    // Pagination
    const page = filters.page || 1
    const pageSize = filters.pageSize || 10
    const startIndex = (page - 1) * pageSize
    const endIndex = startIndex + pageSize
    
    const paginatedTransactions = filteredTransactions.slice(startIndex, endIndex)
    
    return {
      transactions: paginatedTransactions,
      pagination: {
        currentPage: page,
        totalPages: Math.ceil(filteredTransactions.length / pageSize),
        pageSize,
        totalResults: filteredTransactions.length
      }
    }
  } catch (error) {
    console.error('Error fetching transactions:', error)
    throw error
  }
}

/**
 * Formats a number as Brazilian Real currency
 */
export function formatCurrency(value: number): string {
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL'
  }).format(value)
}

/**
 * Formats a date string to Brazilian format
 */
export function formatDate(dateString: string): string {
  const date = new Date(dateString)
  return new Intl.DateTimeFormat('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit'
  }).format(date)
}

/**
 * Gets the status color class for Tailwind
 */
export function getStatusColorClass(statusId: string): string {
  const colorMap: Record<string, string> = {
    'em-validacao': 'bg-blue-500/20 text-blue-400',
    'pendente': 'bg-red-500/20 text-red-400',
    'validado': 'bg-green-500/20 text-green-400',
    'revisar': 'bg-orange-500/20 text-orange-400'
  }
  return colorMap[statusId] || 'bg-zinc-500/20 text-zinc-400'
}
