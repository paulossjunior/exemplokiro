<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useTransactions } from '@/composables/useTransactions'
import { useAccountingAccountStore } from '@/stores/accountingAccountStore'
import { TransactionClassification, type CreateTransactionRequest } from '@/types/api'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import ErrorAlert from '@/components/common/ErrorAlert.vue'

interface Props {
  projectId: string
}

interface Emits {
  (e: 'success', transaction: any): void
  (e: 'cancel'): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const { 
  creatingTransaction, 
  createError, 
  createTransaction 
} = useTransactions()

const accountingAccountStore = useAccountingAccountStore()

// Form state
const formData = ref<CreateTransactionRequest>({
  amount: 0,
  date: new Date().toISOString().split('T')[0], // Today's date in YYYY-MM-DD format
  classification: TransactionClassification.Debit,
  accountingAccountId: '',
})

// Validation errors
const validationErrors = ref<Record<string, string>>({})

// Success message
const showSuccess = ref(false)
const successMessage = ref('')

// Classification options
const classificationOptions = [
  { value: TransactionClassification.Credit, label: 'Credit (Income)' },
  { value: TransactionClassification.Debit, label: 'Debit (Expense)' },
]

// Load accounting accounts
const loadAccountingAccounts = async () => {
  try {
    await accountingAccountStore.fetchAccounts()
  } catch (err) {
    // Error handling can be added if needed
  }
}

// Validate form
const validateForm = (): boolean => {
  validationErrors.value = {}
  
  // Amount validation
  if (!formData.value.amount || formData.value.amount <= 0) {
    validationErrors.value.amount = 'Amount must be greater than 0'
  }
  
  // Date validation
  if (!formData.value.date) {
    validationErrors.value.date = 'Date is required'
  } else {
    const selectedDate = new Date(formData.value.date)
    const today = new Date()
    today.setHours(0, 0, 0, 0)
    
    if (selectedDate > today) {
      validationErrors.value.date = 'Date cannot be in the future'
    }
  }
  
  // Classification validation
  if (!formData.value.classification) {
    validationErrors.value.classification = 'Classification is required'
  }
  
  // Accounting account validation
  if (!formData.value.accountingAccountId) {
    validationErrors.value.accountingAccountId = 'Accounting account is required'
  }
  
  return Object.keys(validationErrors.value).length === 0
}

// Handle form submission
const handleSubmit = async () => {
  // Reset success message
  showSuccess.value = false
  
  // Validate form
  if (!validateForm()) {
    return
  }
  
  try {
    const transaction = await createTransaction(props.projectId, formData.value)
    
    // Show success message
    successMessage.value = 'Transaction created successfully!'
    showSuccess.value = true
    
    // Reset form
    formData.value = {
      amount: 0,
      date: new Date().toISOString().split('T')[0],
      classification: TransactionClassification.Debit,
      accountingAccountId: '',
    }
    
    // Emit success event
    emit('success', transaction)
    
    // Hide success message after 3 seconds
    setTimeout(() => {
      showSuccess.value = false
    }, 3000)
  } catch (err) {
    // Error is handled by the composable and displayed via ErrorAlert
  }
}

// Handle cancel
const handleCancel = () => {
  emit('cancel')
}

// Computed: Is form valid
const isFormValid = computed(() => {
  return (
    formData.value.amount > 0 &&
    formData.value.date &&
    formData.value.classification &&
    formData.value.accountingAccountId
  )
})

// Load accounting accounts on mount
onMounted(() => {
  loadAccountingAccounts()
})
</script>

<template>
  <div class="transaction-form-container">
    <!-- Success Message -->
    <div
      v-if="showSuccess"
      class="mb-6 rounded-lg border border-green-300 bg-green-50 p-4 shadow-sm"
      role="alert"
      aria-live="polite"
    >
      <div class="flex items-start gap-3">
        <div class="flex-shrink-0">
          <svg
            class="h-5 w-5 text-green-600"
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 20 20"
            fill="currentColor"
            aria-hidden="true"
          >
            <path
              fill-rule="evenodd"
              d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.857-9.809a.75.75 0 00-1.214-.882l-3.483 4.79-1.88-1.88a.75.75 0 10-1.06 1.061l2.5 2.5a.75.75 0 001.137-.089l4-5.5z"
              clip-rule="evenodd"
            />
          </svg>
        </div>
        <div class="flex-1">
          <h3 class="text-sm font-semibold text-green-800">
            {{ successMessage }}
          </h3>
        </div>
      </div>
    </div>

    <!-- Error Alert -->
    <ErrorAlert
      v-if="createError"
      :error="createError"
      class="mb-6"
    />

    <!-- Form -->
    <form @submit.prevent="handleSubmit" class="rounded-lg border border-gray-200 bg-white p-6 shadow-sm">
      <h2 class="mb-6 text-xl font-semibold text-gray-900">Create Transaction</h2>

      <div class="space-y-6">
        <!-- Amount Field -->
        <div>
          <label for="amount" class="block text-sm font-medium text-gray-700 mb-1">
            Amount <span class="text-red-500">*</span>
          </label>
          <div class="relative">
            <div class="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-3">
              <span class="text-gray-500 sm:text-sm">$</span>
            </div>
            <input
              id="amount"
              v-model.number="formData.amount"
              type="number"
              step="0.01"
              min="0"
              required
              :disabled="creatingTransaction"
              :class="[
                'block w-full rounded-md border-gray-300 pl-7 pr-12 shadow-sm focus:border-blue-500 focus:ring-1 focus:ring-blue-500 sm:text-sm',
                validationErrors.amount ? 'border-red-300 focus:border-red-500 focus:ring-red-500' : ''
              ]"
              placeholder="0.00"
              aria-label="Transaction amount"
              aria-required="true"
              :aria-invalid="!!validationErrors.amount"
              aria-describedby="amount-error"
            />
          </div>
          <p
            v-if="validationErrors.amount"
            id="amount-error"
            class="mt-1 text-sm text-red-600"
            role="alert"
          >
            {{ validationErrors.amount }}
          </p>
        </div>

        <!-- Date Field -->
        <div>
          <label for="date" class="block text-sm font-medium text-gray-700 mb-1">
            Transaction Date <span class="text-red-500">*</span>
          </label>
          <input
            id="date"
            v-model="formData.date"
            type="date"
            required
            :max="new Date().toISOString().split('T')[0]"
            :disabled="creatingTransaction"
            :class="[
              'block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-1 focus:ring-blue-500 sm:text-sm',
              validationErrors.date ? 'border-red-300 focus:border-red-500 focus:ring-red-500' : ''
            ]"
            aria-label="Transaction date"
            aria-required="true"
            :aria-invalid="!!validationErrors.date"
            aria-describedby="date-error date-help"
          />
          <p
            v-if="validationErrors.date"
            id="date-error"
            class="mt-1 text-sm text-red-600"
            role="alert"
          >
            {{ validationErrors.date }}
          </p>
          <p
            v-else
            id="date-help"
            class="mt-1 text-sm text-gray-500"
          >
            Date cannot be in the future
          </p>
        </div>

        <!-- Classification Field -->
        <div>
          <label for="classification" class="block text-sm font-medium text-gray-700 mb-1">
            Classification <span class="text-red-500">*</span>
          </label>
          <select
            id="classification"
            v-model="formData.classification"
            required
            :disabled="creatingTransaction"
            :class="[
              'block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-1 focus:ring-blue-500 sm:text-sm',
              validationErrors.classification ? 'border-red-300 focus:border-red-500 focus:ring-red-500' : ''
            ]"
            aria-label="Transaction classification"
            aria-required="true"
            :aria-invalid="!!validationErrors.classification"
            aria-describedby="classification-error classification-help"
          >
            <option
              v-for="option in classificationOptions"
              :key="option.value"
              :value="option.value"
            >
              {{ option.label }}
            </option>
          </select>
          <p
            v-if="validationErrors.classification"
            id="classification-error"
            class="mt-1 text-sm text-red-600"
            role="alert"
          >
            {{ validationErrors.classification }}
          </p>
          <p
            v-else
            id="classification-help"
            class="mt-1 text-sm text-gray-500"
          >
            Credit for income, Debit for expenses
          </p>
        </div>

        <!-- Accounting Account Field -->
        <div>
          <label for="accounting-account" class="block text-sm font-medium text-gray-700 mb-1">
            Accounting Account <span class="text-red-500">*</span>
          </label>
          <select
            id="accounting-account"
            v-model="formData.accountingAccountId"
            required
            :disabled="creatingTransaction || accountingAccountStore.accounts.length === 0"
            :class="[
              'block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-1 focus:ring-blue-500 sm:text-sm',
              validationErrors.accountingAccountId ? 'border-red-300 focus:border-red-500 focus:ring-red-500' : ''
            ]"
            aria-label="Accounting account"
            aria-required="true"
            :aria-invalid="!!validationErrors.accountingAccountId"
            aria-describedby="accounting-account-error"
          >
            <option value="">Select an account</option>
            <option
              v-for="account in accountingAccountStore.accounts"
              :key="account.id"
              :value="account.id"
            >
              {{ account.name }} ({{ account.identifier }})
            </option>
          </select>
          <p
            v-if="validationErrors.accountingAccountId"
            id="accounting-account-error"
            class="mt-1 text-sm text-red-600"
            role="alert"
          >
            {{ validationErrors.accountingAccountId }}
          </p>
          <p
            v-else-if="accountingAccountStore.accounts.length === 0"
            class="mt-1 text-sm text-gray-500"
          >
            Loading accounting accounts...
          </p>
        </div>

        <!-- Authentication Notice -->
        <div class="rounded-md bg-blue-50 p-4">
          <div class="flex">
            <div class="flex-shrink-0">
              <svg
                class="h-5 w-5 text-blue-400"
                xmlns="http://www.w3.org/2000/svg"
                viewBox="0 0 20 20"
                fill="currentColor"
                aria-hidden="true"
              >
                <path
                  fill-rule="evenodd"
                  d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a.75.75 0 000 1.5h.253a.25.25 0 01.244.304l-.459 2.066A1.75 1.75 0 0010.747 15H11a.75.75 0 000-1.5h-.253a.25.25 0 01-.244-.304l.459-2.066A1.75 1.75 0 009.253 9H9z"
                  clip-rule="evenodd"
                />
              </svg>
            </div>
            <div class="ml-3 flex-1">
              <p class="text-sm text-blue-700">
                All transactions are digitally signed and require authentication. Only the project coordinator can create transactions.
              </p>
            </div>
          </div>
        </div>
      </div>

      <!-- Form Actions -->
      <div class="mt-8 flex items-center justify-end gap-3">
        <button
          type="button"
          @click="handleCancel"
          :disabled="creatingTransaction"
          class="inline-flex items-center rounded-md border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 shadow-sm transition-colors hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
        >
          Cancel
        </button>
        <button
          type="submit"
          :disabled="creatingTransaction || !isFormValid"
          class="inline-flex items-center gap-2 rounded-md bg-blue-600 px-4 py-2 text-sm font-semibold text-white shadow-sm transition-colors hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
          aria-label="Create transaction"
        >
          <LoadingSpinner v-if="creatingTransaction" size="sm" />
          <span v-if="creatingTransaction">Creating...</span>
          <span v-else>Create Transaction</span>
        </button>
      </div>
    </form>
  </div>
</template>
