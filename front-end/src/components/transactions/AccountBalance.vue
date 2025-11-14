<script setup lang="ts">
import { onMounted, watch, computed } from 'vue'
import { useTransactions } from '@/composables/useTransactions'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import ErrorAlert from '@/components/common/ErrorAlert.vue'

interface Props {
  projectId: string
  budgetAmount: number
  autoRefresh?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  autoRefresh: true
})

const { 
  balance, 
  loadingBalance, 
  balanceError, 
  fetchBalance,
  transactions
} = useTransactions()

// Load balance
const loadBalance = async () => {
  try {
    await fetchBalance(props.projectId)
  } catch (err) {
    // Error is handled by the composable
  }
}

// Format currency
const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
  }).format(amount)
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

// Computed: Is over budget
const isOverBudget = computed(() => {
  if (!balance.value) return false
  return balance.value.balance < 0
})

// Computed: Budget utilization percentage
const budgetUtilization = computed(() => {
  if (!balance.value || props.budgetAmount === 0) return 0
  const spent = balance.value.totalDebits - balance.value.totalCredits
  return Math.min((spent / props.budgetAmount) * 100, 100)
})

// Computed: Budget status color
const budgetStatusColor = computed(() => {
  const utilization = budgetUtilization.value
  if (utilization >= 100) return 'text-red-600'
  if (utilization >= 80) return 'text-yellow-600'
  return 'text-green-600'
})

// Computed: Budget status background
const budgetStatusBg = computed(() => {
  const utilization = budgetUtilization.value
  if (utilization >= 100) return 'bg-red-50 border-red-200'
  if (utilization >= 80) return 'bg-yellow-50 border-yellow-200'
  return 'bg-green-50 border-green-200'
})

// Computed: Progress bar color
const progressBarColor = computed(() => {
  const utilization = budgetUtilization.value
  if (utilization >= 100) return 'bg-red-600'
  if (utilization >= 80) return 'bg-yellow-600'
  return 'bg-green-600'
})

// Watch for projectId changes
watch(() => props.projectId, () => {
  loadBalance()
}, { immediate: false })

// Watch for transaction changes to auto-refresh
watch(() => transactions.value.length, () => {
  if (props.autoRefresh) {
    loadBalance()
  }
}, { immediate: false })

// Load balance on mount
onMounted(() => {
  loadBalance()
})
</script>

<template>
  <div class="account-balance-container">
    <!-- Error Alert -->
    <ErrorAlert
      v-if="balanceError"
      :error="balanceError"
      :on-retry="loadBalance"
      class="mb-6"
    />

    <!-- Loading State -->
    <div
      v-if="loadingBalance && !balance"
      class="flex items-center justify-center rounded-lg border border-gray-200 bg-white p-12 shadow-sm"
      aria-live="polite"
      aria-busy="true"
    >
      <LoadingSpinner size="lg" message="Loading balance..." />
    </div>

    <!-- Balance Display -->
    <div
      v-else-if="balance"
      :class="[
        'rounded-lg border p-6 shadow-sm transition-colors',
        budgetStatusBg
      ]"
      role="region"
      aria-label="Account balance information"
    >
      <!-- Header -->
      <div class="mb-6 flex items-center justify-between">
        <h2 class="text-xl font-semibold text-gray-900">Account Balance</h2>
        <button
          type="button"
          @click="loadBalance"
          :disabled="loadingBalance"
          class="inline-flex items-center gap-2 rounded-md bg-white px-3 py-2 text-sm font-medium text-gray-700 shadow-sm transition-colors hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
          aria-label="Refresh balance"
        >
          <svg
            :class="['h-4 w-4', loadingBalance ? 'animate-spin' : '']"
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
              d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"
            />
          </svg>
          Refresh
        </button>
      </div>

      <!-- Budget Overview -->
      <div class="mb-6 grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <!-- Budget Amount -->
        <div class="rounded-lg bg-white p-4 shadow-sm">
          <div class="text-sm font-medium text-gray-500">Budget Amount</div>
          <div class="mt-1 text-2xl font-semibold text-gray-900">
            {{ formatCurrency(budgetAmount) }}
          </div>
        </div>

        <!-- Current Balance -->
        <div class="rounded-lg bg-white p-4 shadow-sm">
          <div class="text-sm font-medium text-gray-500">Current Balance</div>
          <div :class="['mt-1 text-2xl font-semibold', budgetStatusColor]">
            {{ formatCurrency(balance.balance) }}
          </div>
        </div>

        <!-- Total Credits -->
        <div class="rounded-lg bg-white p-4 shadow-sm">
          <div class="text-sm font-medium text-gray-500">Total Credits</div>
          <div class="mt-1 text-2xl font-semibold text-green-600">
            {{ formatCurrency(balance.totalCredits) }}
          </div>
          <div class="mt-1 text-xs text-gray-500">
            {{ balance.transactionCount }} transaction{{ balance.transactionCount !== 1 ? 's' : '' }}
          </div>
        </div>

        <!-- Total Debits -->
        <div class="rounded-lg bg-white p-4 shadow-sm">
          <div class="text-sm font-medium text-gray-500">Total Debits</div>
          <div class="mt-1 text-2xl font-semibold text-red-600">
            {{ formatCurrency(balance.totalDebits) }}
          </div>
        </div>
      </div>

      <!-- Budget Utilization Progress Bar -->
      <div class="mb-4">
        <div class="mb-2 flex items-center justify-between">
          <span class="text-sm font-medium text-gray-700">Budget Utilization</span>
          <span :class="['text-sm font-semibold', budgetStatusColor]">
            {{ budgetUtilization.toFixed(1) }}%
          </span>
        </div>
        <div class="h-3 w-full overflow-hidden rounded-full bg-gray-200">
          <div
            :class="['h-full transition-all duration-500', progressBarColor]"
            :style="{ width: `${Math.min(budgetUtilization, 100)}%` }"
            role="progressbar"
            :aria-valuenow="budgetUtilization"
            aria-valuemin="0"
            aria-valuemax="100"
            :aria-label="`Budget utilization: ${budgetUtilization.toFixed(1)}%`"
          ></div>
        </div>
      </div>

      <!-- Over Budget Warning -->
      <div
        v-if="isOverBudget"
        class="mb-4 rounded-md bg-red-100 p-4"
        role="alert"
        aria-live="polite"
      >
        <div class="flex">
          <div class="flex-shrink-0">
            <svg
              class="h-5 w-5 text-red-600"
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 20 20"
              fill="currentColor"
              aria-hidden="true"
            >
              <path
                fill-rule="evenodd"
                d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.28 7.22a.75.75 0 00-1.06 1.06L8.94 10l-1.72 1.72a.75.75 0 101.06 1.06L10 11.06l1.72 1.72a.75.75 0 101.06-1.06L11.06 10l1.72-1.72a.75.75 0 00-1.06-1.06L10 8.94 8.28 7.22z"
                clip-rule="evenodd"
              />
            </svg>
          </div>
          <div class="ml-3">
            <h3 class="text-sm font-semibold text-red-800">Over Budget</h3>
            <p class="mt-1 text-sm text-red-700">
              This project has exceeded its budget by {{ formatCurrency(Math.abs(balance.balance)) }}.
            </p>
          </div>
        </div>
      </div>

      <!-- Near Budget Warning -->
      <div
        v-else-if="budgetUtilization >= 80"
        class="mb-4 rounded-md bg-yellow-100 p-4"
        role="alert"
        aria-live="polite"
      >
        <div class="flex">
          <div class="flex-shrink-0">
            <svg
              class="h-5 w-5 text-yellow-600"
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 20 20"
              fill="currentColor"
              aria-hidden="true"
            >
              <path
                fill-rule="evenodd"
                d="M8.485 2.495c.673-1.167 2.357-1.167 3.03 0l6.28 10.875c.673 1.167-.17 2.625-1.516 2.625H3.72c-1.347 0-2.189-1.458-1.515-2.625L8.485 2.495zM10 5a.75.75 0 01.75.75v3.5a.75.75 0 01-1.5 0v-3.5A.75.75 0 0110 5zm0 9a1 1 0 100-2 1 1 0 000 2z"
                clip-rule="evenodd"
              />
            </svg>
          </div>
          <div class="ml-3">
            <h3 class="text-sm font-semibold text-yellow-800">Approaching Budget Limit</h3>
            <p class="mt-1 text-sm text-yellow-700">
              This project has used {{ budgetUtilization.toFixed(1) }}% of its budget. Remaining: {{ formatCurrency(balance.balance) }}.
            </p>
          </div>
        </div>
      </div>

      <!-- Calculation Timestamp -->
      <div class="flex items-center justify-between border-t border-gray-200 pt-4">
        <div class="flex items-center gap-2 text-sm text-gray-500">
          <svg
            class="h-4 w-4"
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
              d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"
            />
          </svg>
          <span>Last calculated: {{ formatDateTime(balance.calculatedAt) }}</span>
        </div>
        <div
          v-if="loadingBalance"
          class="flex items-center gap-2 text-sm text-blue-600"
        >
          <LoadingSpinner size="sm" />
          <span>Updating...</span>
        </div>
      </div>
    </div>
  </div>
</template>
