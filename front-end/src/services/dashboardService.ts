/**
 * Dashboard API Service
 * Handles all API communication for dashboard data
 */

import type {
  Project,
  DashboardMetrics,
  Transaction,
  FilterCriteria,
  PaginatedResponse
} from '@/types/dashboard'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || '/api'

/**
 * Fetches the list of available projects
 */
export async function getProjects(): Promise<Project[]> {
  try {
    const response = await fetch(`${API_BASE_URL}/projects`)
    
    if (!response.ok) {
      throw new Error(`Failed to fetch projects: ${response.statusText}`)
    }
    
    return await response.json()
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
    const response = await fetch(`${API_BASE_URL}/dashboard/metrics/${projectId}`)
    
    if (!response.ok) {
      throw new Error(`Failed to fetch dashboard metrics: ${response.statusText}`)
    }
    
    return await response.json()
  } catch (error) {
    console.error('Error fetching dashboard metrics:', error)
    throw new Error('Falha ao carregar métricas do dashboard')
  }
}

/**
 * Fetches transactions with filtering and pagination
 */
export async function getTransactions(
  params: FilterCriteria
): Promise<PaginatedResponse<Transaction>> {
  try {
    const queryParams = new URLSearchParams()
    
    if (params.projectId) queryParams.append('projectId', params.projectId)
    if (params.searchTerm) queryParams.append('searchTerm', params.searchTerm)
    if (params.date) queryParams.append('date', params.date)
    if (params.status) queryParams.append('status', params.status)
    if (params.category) queryParams.append('category', params.category)
    queryParams.append('page', params.page.toString())
    queryParams.append('pageSize', params.pageSize.toString())
    
    const response = await fetch(
      `${API_BASE_URL}/dashboard/transactions?${queryParams.toString()}`
    )
    
    if (!response.ok) {
      throw new Error(`Failed to fetch transactions: ${response.statusText}`)
    }
    
    return await response.json()
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
    const response = await fetch(`${API_BASE_URL}/dashboard/categories`)
    
    if (!response.ok) {
      throw new Error(`Failed to fetch categories: ${response.statusText}`)
    }
    
    return await response.json()
  } catch (error) {
    console.error('Error fetching categories:', error)
    throw new Error('Falha ao carregar categorias')
  }
}
