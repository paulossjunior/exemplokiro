import { ref, computed } from 'vue'
import type { Ref } from 'vue'
import {
  fetchDashboardData,
  fetchTransactions,
  type DashboardData,
  type Transaction,
  type TransactionFilters
} from '@/services/mockApi'

/**
 * Composable for managing dashboard state and operations
 */
export function useDashboard() {
  // State
  const dashboardData = ref<DashboardData | null>(null)
  const transactions = ref<Transaction[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)
  
  // Pagination
  const currentPage = ref(1)
  const totalPages = ref(1)
  const totalResults = ref(0)
  const pageSize = ref(10)
  
  // Filters
  const selectedProjectId = ref<string>('')
  const searchQuery = ref<string>('')
  const selectedDate = ref<string>('')
  const selectedStatus = ref<string>('')
  const selectedCategory = ref<string>('')
  
  // Computed
  const budgetSummary = computed(() => dashboardData.value?.budgetSummary)
  const projects = computed(() => dashboardData.value?.projects || [])
  const categories = computed(() => dashboardData.value?.categories || [])
  const statuses = computed(() => dashboardData.value?.statuses || [])
  
  const hasFilters = computed(() => {
    return !!(
      selectedProjectId.value ||
      searchQuery.value ||
      selectedDate.value ||
      selectedStatus.value ||
      selectedCategory.value
    )
  })
  
  /**
   * Loads initial dashboard data
   */
  const loadDashboardData = async () => {
    loading.value = true
    error.value = null
    
    try {
      dashboardData.value = await fetchDashboardData()
    } catch (e) {
      error.value = 'Falha ao carregar dados do dashboard'
      console.error(e)
    } finally {
      loading.value = false
    }
  }
  
  /**
   * Loads transactions with current filters
   */
  const loadTransactions = async () => {
    loading.value = true
    error.value = null
    
    try {
      const filters: TransactionFilters = {
        projectId: selectedProjectId.value || undefined,
        search: searchQuery.value || undefined,
        date: selectedDate.value || undefined,
        status: selectedStatus.value || undefined,
        category: selectedCategory.value || undefined,
        page: currentPage.value,
        pageSize: pageSize.value
      }
      
      const response = await fetchTransactions(filters)
      transactions.value = response.transactions
      currentPage.value = response.pagination.currentPage
      totalPages.value = response.pagination.totalPages
      totalResults.value = response.pagination.totalResults
    } catch (e) {
      error.value = 'Falha ao carregar transações'
      console.error(e)
    } finally {
      loading.value = false
    }
  }
  
  /**
   * Applies all filters and reloads transactions
   */
  const applyFilters = async () => {
    currentPage.value = 1 // Reset to first page
    await loadTransactions()
  }
  
  /**
   * Clears all filters
   */
  const clearFilters = () => {
    selectedProjectId.value = ''
    searchQuery.value = ''
    selectedDate.value = ''
    selectedStatus.value = ''
    selectedCategory.value = ''
    currentPage.value = 1
  }
  
  /**
   * Goes to next page
   */
  const nextPage = async () => {
    if (currentPage.value < totalPages.value) {
      currentPage.value++
      await loadTransactions()
    }
  }
  
  /**
   * Goes to previous page
   */
  const previousPage = async () => {
    if (currentPage.value > 1) {
      currentPage.value--
      await loadTransactions()
    }
  }
  
  /**
   * Goes to specific page
   */
  const goToPage = async (page: number) => {
    if (page >= 1 && page <= totalPages.value) {
      currentPage.value = page
      await loadTransactions()
    }
  }
  
  return {
    // State
    dashboardData,
    transactions,
    loading,
    error,
    
    // Pagination
    currentPage,
    totalPages,
    totalResults,
    pageSize,
    
    // Filters
    selectedProjectId,
    searchQuery,
    selectedDate,
    selectedStatus,
    selectedCategory,
    
    // Computed
    budgetSummary,
    projects,
    categories,
    statuses,
    hasFilters,
    
    // Methods
    loadDashboardData,
    loadTransactions,
    applyFilters,
    clearFilters,
    nextPage,
    previousPage,
    goToPage
  }
}
