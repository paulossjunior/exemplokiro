<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue'
import { useTransactions } from '@/composables/useTransactions'
import { TransactionClassification } from '@/types/api'
import { useAccountingAccountStore } from '@/stores/accountingAccountStore'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import ErrorAlert from '@/components/common/ErrorAlert.vue'

interface Props {
  projectId: string
}

const props = defineProps<Props>()

const { 
  transactions, 
  loadingTransactions, 
  transactionsError, 
  fetchTransactions 
} = useTransactions()

const accountingAccountStore = useAccountingAccountStore()

// Filter state
const startDate = ref<string>('')
const endDate = ref<string>('')
const selectedClassification = ref<string>('')
const selectedAccountId = ref<string>('')

// Pagination state
const currentPage = ref(1)
const pageSize = ref(10)

// Classification options
const classificationOptions = [
  { value: '', label: 'All Classifications' },
  { value: TransactionClassification.Credit, label: 'Credit' },
  { value: TransactionClassification.Debit, label: 'Debit' },
]

// Computed paginated transactions
const paginatedTransactions = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  const end = start + pageSize.value
  return transactions.value.slice(start, end)
})

const totalPages = computed(() => {
  return Math.ceil(transactions.value.length / pageSize.value)
})

const totalCount = computed(() => {
  return transactions.value.length
})

// Load transactions
const loadTransactions = async () => {
  try {
    const params: any = {}
    
    if (startDate.value) {
      params.startDate = startDate.value
    }
    
    if (endDate.value) {
      params.endDate = endDate.value
    }
    
    if (selectedClassification.value) {
      params.classification = selectedClassification.value
    }
    
    if (selectedAccountId.value) {
      params.accountingAccountId = selectedAccountId.value
    }
    
    await fetchTransactions(props.projectId, params)
    currentPage.value = 1 // Reset to first page
  } catch (err) {
    // Error is handled by the composable
  }
}

// Load accounting accounts for filter
const loadAccountingAccounts = async () => {
  try {
    await accountingAccountStore.fetchAccounts()
  } catch (err) {
    // Error handling can be added if needed
  }
}

// Handle filter changes
const onFilterChange = () => {
  loadTransactions()
}

// Handle page change
const goToPage = (page: number) => {
  if (page >= 1 && page <= totalPages.value) {
    currentPage.value = page
  }
}

// Format currency
const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
  }).format(amount)
}

// Format date
const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  })
}

// Format date and time
const formatDateTime = (dateString: string) => {
  return new Date(dateString).toLocaleString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })
}

// Get classification badge color
const getClassificationColor = (classification: string) => {
  return classification === TransactionClassification.Credit
    ? 'bg-green-100 text-green-800'
    : 'bg-red-100 text-red-800'
}

// Get classification label
const getClassificationLabel = (classification: string) => {
  return classification === TransactionClassification.Credit ? 'Credit' : 'Debit'
}

// Get accounting account name
const getAccountName = (accountId: string) => {
  const account = accountingAccountStore.accounts.find(a => a.id === accountId)
  return account ? account.name : accountId
}

// Pagination range
const paginationRange = computed(() => {
  const range = []
  const delta = 2
  const left = Math.max(2, currentPage.value - delta)
  const right = Math.min(totalPages.value - 1, currentPage.value + delta)
  
  range.push(1)
  
  if (left > 2) {
    range.push(-1) // Ellipsis
  }
  
  for (let i = left; i <= right; i++) {
    range.push(i)
  }
  
  if (right < totalPages.value - 1) {
    range.push(-1) // Ellipsis
  }
  
  if (totalPages.value > 1) {
    range.push(totalPages.value)
  }
  
  return range
})

// Watch for projectId changes
watch(() => props.projectId, () => {
  loadTransactions()
}, { immediate: false })

// Load data on mount
onMounted(() => {
  loadAccountingAccounts()
  loadTransactions()
})
</script>

<template>
  <div class="transaction-list-container">
    <!-- Filters -->
    <section class="mb-6 rounded-lg border border-gray-200 bg-white p-4 shadow-sm" aria-label="Transaction filters">
      <h2 class="mb-4 text-lg font-semibold text-gray-900">Filter Transactions</h2>
      <div class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <!-- Start Date Filter -->
        <div>
          <label for="start-date" class="block text-sm font-medium text-gray-700 mb-1">
            Start Date
          </label>
          <input
            id="start-date"
            v-model="startDate"
            type="date"
            @change="onFilterChange"
            class="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-1 focus:ring-blue-500 sm:text-sm"
            aria-label="Filter by start date"
          />
        </div>

        <!-- End Date Filter -->
        <div>
          <label for="end-date" class="block text-sm font-medium text-gray-700 mb-1">
            End Date
          </label>
          <input
            id="end-date"
            v-model="endDate"
            type="date"
            @change="onFilterChange"
            class="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-1 focus:ring-blue-500 sm:text-sm"
            aria-label="Filter by end date"
          />
        </div>

        <!-- Classification Filter -->
        <div>
          <label for="classification-filter" class="block text-sm font-medium text-gray-700 mb-1">
            Classification
          </label>
          <select
            id="classification-filter"
            v-model="selectedClassification"
            @change="onFilterChange"
            class="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-1 focus:ring-blue-500 sm:text-sm"
            aria-label="Filter by classification"
          >
            <option
              v-for="option in classificationOptions"
              :key="option.value"
              :value="option.value"
            >
              {{ option.label }}
            </option>
          </select>
        </div>

        <!-- Accounting Account Filter -->
        <div>
          <label for="account-filter" class="block text-sm font-medium text-gray-700 mb-1">
            Accounting Account
          </label>
          <select
            id="account-filter"
            v-model="selectedAccountId"
            @change="onFilterChange"
            class="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-1 focus:ring-blue-500 sm:text-sm"
            aria-label="Filter by accounting account"
          >
            <option value="">All Accounts</option>
            <option
              v-for="account in accountingAccountStore.accounts"
              :key="account.id"
              :value="account.id"
            >
              {{ account.name }} ({{ account.identifier }})
            </option>
          </select>
        </div>
      </div>
    </section>

    <!-- Error Alert -->
    <ErrorAlert
      v-if="transactionsError"
      :error="transactionsError"
      :on-retry="loadTransactions"
      class="mb-6"
    />

    <!-- Loading State -->
    <div
      v-if="loadingTransactions && transactions.length === 0"
      class="flex items-center justify-center py-12"
      aria-live="polite"
      aria-busy="true"
    >
      <LoadingSpinner size="lg" message="Loading transactions..." />
    </div>

    <!-- Transactions Table -->
    <section
      v-else-if="!loadingTransactions && paginatedTransactions.length > 0"
      class="overflow-hidden rounded-lg border border-gray-200 bg-white shadow"
      aria-label="Transactions table"
    >
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th
                scope="col"
                class="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500"
              >
                Date
              </th>
              <th
                scope="col"
                class="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500"
              >
                Account
              </th>
              <th
                scope="col"
                class="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500"
              >
                Classification
              </th>
              <th
                scope="col"
                class="px-6 py-3 text-right text-xs font-medium uppercase tracking-wider text-gray-500"
              >
                Amount
              </th>
              <th
                scope="col"
                class="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500"
              >
                Created By
              </th>
              <th
                scope="col"
                class="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500"
              >
                Digital Signature
              </th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-200 bg-white">
            <tr
              v-for="transaction in paginatedTransactions"
              :key="transaction.id"
              class="transition-colors hover:bg-gray-50"
            >
              <td class="whitespace-nowrap px-6 py-4 text-sm text-gray-900">
                {{ formatDate(transaction.date) }}
              </td>
              <td class="px-6 py-4">
                <div class="text-sm font-medium text-gray-900">
                  {{ getAccountName(transaction.accountingAccountId) }}
                </div>
                <div class="text-xs text-gray-500">
                  Created: {{ formatDateTime(transaction.createdAt) }}
                </div>
              </td>
              <td class="whitespace-nowrap px-6 py-4">
                <span
                  :class="getClassificationColor(transaction.classification)"
                  class="inline-flex rounded-full px-2 py-1 text-xs font-semibold leading-5"
                >
                  {{ getClassificationLabel(transaction.classification) }}
                </span>
              </td>
              <td class="whitespace-nowrap px-6 py-4 text-right text-sm font-medium text-gray-900">
                {{ formatCurrency(transaction.amount) }}
              </td>
              <td class="whitespace-nowrap px-6 py-4 text-sm text-gray-500">
                {{ transaction.createdBy }}
              </td>
              <td class="px-6 py-4">
                <div class="max-w-xs">
                  <div class="text-xs font-mono text-gray-600 truncate" :title="transaction.digitalSignature">
                    {{ transaction.digitalSignature }}
                  </div>
                  <div class="text-xs text-gray-400 mt-1">
                    Hash: <span class="font-mono truncate" :title="transaction.dataHash">{{ transaction.dataHash.substring(0, 16) }}...</span>
                  </div>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Pagination -->
      <nav
        v-if="totalPages > 1"
        class="flex items-center justify-between border-t border-gray-200 bg-white px-4 py-3 sm:px-6"
        aria-label="Pagination"
      >
        <div class="hidden sm:block">
          <p class="text-sm text-gray-700">
            Showing
            <span class="font-medium">{{ (currentPage - 1) * pageSize + 1 }}</span>
            to
            <span class="font-medium">{{
              Math.min(currentPage * pageSize, totalCount)
            }}</span>
            of
            <span class="font-medium">{{ totalCount }}</span>
            results
          </p>
        </div>
        <div class="flex flex-1 justify-between sm:justify-end">
          <button
            type="button"
            @click="goToPage(currentPage - 1)"
            :disabled="currentPage === 1"
            class="relative inline-flex items-center rounded-md border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 transition-colors hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
            aria-label="Go to previous page"
          >
            Previous
          </button>
          <div class="hidden sm:flex sm:gap-1 sm:mx-4">
            <button
              v-for="page in paginationRange"
              :key="page"
              type="button"
              @click="page > 0 ? goToPage(page) : null"
              :disabled="page === -1"
              :class="[
                page === currentPage
                  ? 'bg-blue-600 text-white'
                  : 'bg-white text-gray-700 hover:bg-gray-50',
                page === -1 ? 'cursor-default' : '',
              ]"
              class="relative inline-flex items-center rounded-md border border-gray-300 px-4 py-2 text-sm font-medium transition-colors focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:cursor-not-allowed"
              :aria-label="page === -1 ? 'More pages' : `Go to page ${page}`"
              :aria-current="page === currentPage ? 'page' : undefined"
            >
              {{ page === -1 ? '...' : page }}
            </button>
          </div>
          <button
            type="button"
            @click="goToPage(currentPage + 1)"
            :disabled="currentPage === totalPages"
            class="relative ml-3 inline-flex items-center rounded-md border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 transition-colors hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
            aria-label="Go to next page"
          >
            Next
          </button>
        </div>
      </nav>
    </section>

    <!-- Empty State -->
    <div
      v-else-if="!loadingTransactions && transactions.length === 0"
      class="rounded-lg border-2 border-dashed border-gray-300 bg-white p-12 text-center"
    >
      <svg
        class="mx-auto h-12 w-12 text-gray-400"
        xmlns="http://www.w3.org/2000/svg"
        fill="none"
        viewBox="0 0 24 24"
        stroke="currentColor"
        aria-hidden="true"
      >
        <path
          stroke-linecap="round"
          stroke-linejoin="round"
          stroke-width="2"
          d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-3 7h3m-3 4h3m-6-4h.01M9 16h.01"
        />
      </svg>
      <h3 class="mt-2 text-sm font-medium text-gray-900">No transactions found</h3>
      <p class="mt-1 text-sm text-gray-500">
        No transactions match the current filters.
      </p>
    </div>
  </div>
</template>
