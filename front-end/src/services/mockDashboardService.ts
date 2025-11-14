/**
 * Mock Dashboard Service
 * Uses local JSON files for development and demonstration
 */

import type {
  Project,
  DashboardMetrics,
  Transaction,
  FilterCriteria,
  PaginatedResponse,
  TransactionStatus
} from '@/types/dashboard'

/**
 * Fetches the list of available projects
 */
export async function getProjects(): Promise<Project[]> {
  try {
    const response = await fetch('/mock-api/dashboard-data.json')
    const data = await response.json()
    
    return data.projects.map((p: any) => ({
      id: p.id,
      name: p.name,
      code: `PRJ-${p.id}`,
      status: 'Active'
    }))
  } catch (error) {
    console.error('Error fetching projects:', error)
    throw new Error('Falha ao carregar projetos')
  }
}

/**
 * Fetches dashboard metrics for a specific project
 */
export async function getDashboardMetrics(projectId: string): Promise<DashboardMetrics> {
  try {
    const response = await fetch('/mock-api/dashboard-data.json')
    const data = await response.json()
    
    const project = data.projects.find((p: any) => p.id === projectId)
    
    return {
      consumed: data.budgetSummary.consumed,
      remaining: data.budgetSummary.remaining,
      yield: data.budgetSummary.yield,
      total: data.budgetSummary.total,
      projectId: projectId,
      projectName: project?.name || 'Projeto Desconhecido'
    }
  } catch (error) {
    console.error('Error fetching dashboard metrics:', error)
    throw new Error('Falha ao carregar métricas do dashboard')
  }
}

/**
 * Maps mock status to TransactionStatus type
 */
function mapStatus(status: string): TransactionStatus {
  const statusMap: Record<string, TransactionStatus> = {
    'em-validacao': 'Em Validação',
    'pendente': 'Pendente',
    'validado': 'Validado',
    'revisar': 'Revisar'
  }
  return statusMap[status] || 'Pendente'
}

/**
 * Fetches transactions with filtering and pagination
 */
export async function getTransactions(
  params: FilterCriteria
): Promise<PaginatedResponse<Transaction>> {
  try {
    console.log('Fetching transactions with params:', params)
    const response = await fetch('/mock-api/transactions.json')
    const data = await response.json()
    console.log('Raw transaction data:', data)
    
    // Convert mock transactions to our format
    let transactions: Transaction[] = data.transactions.map((t: any) => ({
      id: t.id,
      paymentMethod: t.paymentMethod,
      amount: t.value,
      date: t.date,
      cnpj: t.cnpj,
      companyName: t.cnpj,
      status: mapStatus(t.status),
      category: t.category,
      projectId: t.projectId
    }))
    
    // Apply filters
    if (params.projectId) {
      transactions = transactions.filter(t => t.projectId === params.projectId)
    }
    
    if (params.searchTerm) {
      const searchLower = params.searchTerm.toLowerCase()
      transactions = transactions.filter(t =>
        t.paymentMethod.toLowerCase().includes(searchLower) ||
        t.cnpj.toLowerCase().includes(searchLower) ||
        (t.companyName && t.companyName.toLowerCase().includes(searchLower))
      )
    }
    
    if (params.date) {
      transactions = transactions.filter(t => t.date.startsWith(params.date!))
    }
    
    if (params.status) {
      transactions = transactions.filter(t => t.status === params.status)
    }
    
    if (params.category) {
      transactions = transactions.filter(t => t.category === params.category)
    }
    
    // Pagination
    const page = params.page || 1
    const pageSize = params.pageSize || 20
    const startIndex = (page - 1) * pageSize
    const endIndex = startIndex + pageSize
    
    const paginatedTransactions = transactions.slice(startIndex, endIndex)
    
    const result = {
      data: paginatedTransactions,
      currentPage: page,
      totalPages: Math.ceil(transactions.length / pageSize),
      totalResults: transactions.length,
      pageSize: pageSize
    }
    
    console.log('Returning transactions:', result)
    return result
  } catch (error) {
    console.error('Error fetching transactions:', error)
    throw new Error('Falha ao carregar transações')
  }
}

/**
 * Fetches the list of available categories
 */
export async function getCategories(): Promise<string[]> {
  try {
    const response = await fetch('/mock-api/dashboard-data.json')
    const data = await response.json()
    
    return data.categories.map((c: any) => c.name)
  } catch (error) {
    console.error('Error fetching categories:', error)
    throw new Error('Falha ao carregar categorias')
  }
}
