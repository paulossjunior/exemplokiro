<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { auditService } from '@/services/api/auditService'
import type { AuditEntry, PaginatedResponse, ApiError } from '@/types/api'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import ErrorAlert from '@/components/common/ErrorAlert.vue'

// State
const auditEntries = ref<AuditEntry[]>([])
const loading = ref(false)
const error = ref<ApiError | null>(null)

// Filter state
const entityType = ref<string>('')
const userId = ref<string>('')
const startDate = ref<string>('')
const endDate = ref<string>('')

// Pagination state
const currentPage = ref(1)
const pageSize = ref(10)
const totalCount = ref(0)
const totalPages = ref(0)

// Entity type options
const entityTypeOptions = [
  { value: '', label: 'All Entity Types' },
  { value: 'Project', label: 'Project' },
  { value: 'Transaction', label: 'Transaction' },
  { value: 'AccountingAccount', label: 'Accounting Account' },
  { value: 'Person', label: 'Person' },
]

// Load audit trail
const loadAuditTrail = async () => {
  loading.value = true
  error.value = null

  try {
    const params: any = {
      pageNumber: currentPage.value,
      pageSize: pageSize.value,
    }

    if (entityType.value) {
      params.entityType = entityType.value
    }

    if (userId.value) {
      params.userId = userId.value
    }

    if (startDate.value) {
      params.startDate = startDate.value
    }

    if (endDate.value) {
      params.endDate = endDate.value
    }

    const response: PaginatedResponse<AuditEntry> = await auditService.getAuditTrail(params)
    
    auditEntries.value = response.items
    totalCount.value = response.totalCount
    totalPages.value = response.totalPages
  } catch (err: any) {
    error.value = {
      message: err.response?.data?.message || err.message || 'Failed to load audit trail',
      errors: err.response?.data?.errors,
      statusCode: err.response?.status || 500,
    }
  } finally {
    loading.value = false
  }
}

// Handle filter changes
const onFilterChange = () => {
  currentPage.value = 1 // Reset to first page
  loadAuditTrail()
}

// Handle page change
const goToPage = (page: number) => {
  if (page >= 1 && page <= totalPages.value) {
    currentPage.value = page
    loadAuditTrail()
  }
}

// Format date and time
const formatDateTime = (dateString: string) => {
  return new Date(dateString).toLocaleString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit',
  })
}

// Get action type badge color
const getActionTypeColor = (actionType: string) => {
  const colors: Record<string, string> = {
    Create: 'bg-green-100 text-green-800',
    Update: 'bg-blue-100 text-blue-800',
    Delete: 'bg-red-100 text-red-800',
    StatusChange: 'bg-yellow-100 text-yellow-800',
  }
  return colors[actionType] || 'bg-gray-100 text-gray-800'
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

// Load data on mount
onMounted(() => {
  loadAuditTrail()
})
</script>

<template>
  <div class="audit-trail-container">
    <!-- Filters -->
    <section
      class="mb-6 rounded-lg border border-gray-200 bg-white p-4 shadow-sm"
      aria-label="Audit trail filters"
    >
      <h2 class="mb-4 text-lg font-semibold text-gray-900">Filter Audit Entries</h2>
      <div class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <!-- Entity Type Filter -->
        <div>
          <label for="entity-type-filter" class="mb-1 block text-sm font-medium text-gray-700">
            Entity Type
          </label>
          <select
            id="entity-type-filter"
            v-model="entityType"
            @change="onFilterChange"
            class="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-1 focus:ring-blue-500 sm:text-sm"
            aria-label="Filter by entity type"
          >
            <option v-for="option in entityTypeOptions" :key="option.value" :value="option.value">
              {{ option.label }}
            </option>
          </select>
        </div>

        <!-- User ID Filter -->
        <div>
          <label for="user-id-filter" class="mb-1 block text-sm font-medium text-gray-700">
            User ID
          </label>
          <input
            id="user-id-filter"
            v-model="userId"
            type="text"
            @input="onFilterChange"
            placeholder="Enter user ID"
            class="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-1 focus:ring-blue-500 sm:text-sm"
            aria-label="Filter by user ID"
          />
        </div>

        <!-- Start Date Filter -->
        <div>
          <label for="start-date-filter" class="mb-1 block text-sm font-medium text-gray-700">
            Start Date
          </label>
          <input
            id="start-date-filter"
            v-model="startDate"
            type="date"
            @change="onFilterChange"
            class="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-1 focus:ring-blue-500 sm:text-sm"
            aria-label="Filter by start date"
          />
        </div>

        <!-- End Date Filter -->
        <div>
          <label for="end-date-filter" class="mb-1 block text-sm font-medium text-gray-700">
            End Date
          </label>
          <input
            id="end-date-filter"
            v-model="endDate"
            type="date"
            @change="onFilterChange"
            class="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-1 focus:ring-blue-500 sm:text-sm"
            aria-label="Filter by end date"
          />
        </div>
      </div>
    </section>

    <!-- Error Alert -->
    <ErrorAlert v-if="error" :error="error" :on-retry="loadAuditTrail" class="mb-6" />

    <!-- Loading State -->
    <div
      v-if="loading && auditEntries.length === 0"
      class="flex items-center justify-center py-12"
      aria-live="polite"
      aria-busy="true"
    >
      <LoadingSpinner size="lg" message="Loading audit trail..." />
    </div>

    <!-- Audit Trail Table -->
    <section
      v-else-if="!loading && auditEntries.length > 0"
      class="overflow-hidden rounded-lg border border-gray-200 bg-white shadow"
      aria-label="Audit trail table"
    >
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th
                scope="col"
                class="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500"
              >
                Timestamp
              </th>
              <th
                scope="col"
                class="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500"
              >
                User
              </th>
              <th
                scope="col"
                class="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500"
              >
                Action
              </th>
              <th
                scope="col"
                class="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500"
              >
                Entity
              </th>
              <th
                scope="col"
                class="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500"
              >
                Changes
              </th>
              <th
                scope="col"
                class="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500"
              >
                Signature
              </th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-200 bg-white">
            <tr
              v-for="entry in auditEntries"
              :key="entry.id"
              class="transition-colors hover:bg-gray-50"
            >
              <td class="whitespace-nowrap px-6 py-4 text-sm text-gray-900">
                {{ formatDateTime(entry.timestamp) }}
              </td>
              <td class="whitespace-nowrap px-6 py-4 text-sm text-gray-700">
                {{ entry.userId }}
              </td>
              <td class="whitespace-nowrap px-6 py-4">
                <span
                  :class="getActionTypeColor(entry.actionType)"
                  class="inline-flex rounded-full px-2 py-1 text-xs font-semibold leading-5"
                >
                  {{ entry.actionType }}
                </span>
              </td>
              <td class="px-6 py-4">
                <div class="text-sm font-medium text-gray-900">
                  {{ entry.entityType }}
                </div>
                <div class="text-xs text-gray-500">ID: {{ entry.entityId }}</div>
              </td>
              <td class="px-6 py-4">
                <div class="max-w-md">
                  <div v-if="entry.previousValue" class="mb-2">
                    <span class="text-xs font-medium text-gray-500">Previous:</span>
                    <div class="mt-1 text-xs text-gray-600">
                      {{ entry.previousValue.substring(0, 100)
                      }}{{ entry.previousValue.length > 100 ? '...' : '' }}
                    </div>
                  </div>
                  <div>
                    <span class="text-xs font-medium text-gray-500">New:</span>
                    <div class="mt-1 text-xs text-gray-600">
                      {{ entry.newValue.substring(0, 100)
                      }}{{ entry.newValue.length > 100 ? '...' : '' }}
                    </div>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4">
                <div class="max-w-xs">
                  <div
                    class="truncate font-mono text-xs text-gray-600"
                    :title="entry.digitalSignature"
                  >
                    {{ entry.digitalSignature }}
                  </div>
                  <div class="mt-1 text-xs text-gray-400">
                    Hash:
                    <span class="truncate font-mono" :title="entry.dataHash">{{
                      entry.dataHash.substring(0, 16)
                    }}...</span>
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
            <span class="font-medium">{{ Math.min(currentPage * pageSize, totalCount) }}</span>
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
          <div class="mx-4 hidden gap-1 sm:flex">
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
      v-else-if="!loading && auditEntries.length === 0"
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
          d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"
        />
      </svg>
      <h3 class="mt-2 text-sm font-medium text-gray-900">No audit entries found</h3>
      <p class="mt-1 text-sm text-gray-500">No audit entries match the current filters.</p>
    </div>
  </div>
</template>
