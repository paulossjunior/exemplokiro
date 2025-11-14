<script setup lang="ts">
import { ref, computed } from 'vue'
import { auditService } from '@/services/api/auditService'
import type { IntegrityVerificationResult, ApiError } from '@/types/api'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import ErrorAlert from '@/components/common/ErrorAlert.vue'

// State
const verificationResult = ref<IntegrityVerificationResult | null>(null)
const loading = ref(false)
const error = ref<ApiError | null>(null)

// Verify integrity
const verifyIntegrity = async () => {
  loading.value = true
  error.value = null
  verificationResult.value = null

  try {
    const result = await auditService.verifyIntegrity()
    verificationResult.value = result
  } catch (err: any) {
    error.value = {
      message: err.response?.data?.message || err.message || 'Failed to verify data integrity',
      errors: err.response?.data?.errors,
      statusCode: err.response?.status || 500,
    }
  } finally {
    loading.value = false
  }
}

// Format date and time
const formatDateTime = (dateString: string) => {
  return new Date(dateString).toLocaleString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit',
  })
}

// Computed properties for easier access
const isValid = computed(() => verificationResult.value?.isIntegrityValid ?? false)
const totalRecordsChecked = computed(() => {
  if (!verificationResult.value) return 0
  return verificationResult.value.totalTransactionsChecked + verificationResult.value.totalAuditEntriesChecked
})
const invalidRecords = computed(() => {
  if (!verificationResult.value) return []
  const records = []
  
  // Add tampered transactions
  for (const id of verificationResult.value.tamperedTransactionIds) {
    records.push({
      entityType: 'Transaction',
      entityId: id,
      issue: 'Digital signature verification failed - data may have been tampered with',
    })
  }
  
  // Add tampered audit entries
  for (const id of verificationResult.value.tamperedAuditEntryIds) {
    records.push({
      entityType: 'AuditEntry',
      entityId: id,
      issue: 'Digital signature verification failed - data may have been tampered with',
    })
  }
  
  return records
})
const verifiedAt = computed(() => verificationResult.value?.verificationTimestamp ?? '')
</script>

<template>
  <div class="integrity-verification-container">
    <!-- Verification Trigger -->
    <section
      class="mb-6 rounded-lg border border-gray-200 bg-white p-6 shadow-sm"
      aria-label="Data integrity verification"
    >
      <div class="flex items-start justify-between">
        <div class="flex-1">
          <h2 class="text-lg font-semibold text-gray-900">Data Integrity Verification</h2>
          <p class="mt-2 text-sm text-gray-600">
            Verify the cryptographic integrity of all transactions and audit entries to detect any
            tampering or data corruption. This process checks digital signatures and data hashes
            across the entire system.
          </p>
        </div>
        <button
          type="button"
          @click="verifyIntegrity"
          :disabled="loading"
          class="ml-4 inline-flex items-center gap-2 rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white shadow-sm transition-colors hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
          aria-label="Start integrity verification"
        >
          <svg
            v-if="!loading"
            class="h-5 w-5"
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 20 20"
            fill="currentColor"
            aria-hidden="true"
          >
            <path
              fill-rule="evenodd"
              d="M16.403 12.652a3 3 0 000-5.304 3 3 0 00-3.75-3.751 3 3 0 00-5.305 0 3 3 0 00-3.751 3.75 3 3 0 000 5.305 3 3 0 003.75 3.751 3 3 0 005.305 0 3 3 0 003.751-3.75zm-2.546-4.46a.75.75 0 00-1.214-.883l-3.483 4.79-1.88-1.88a.75.75 0 10-1.06 1.061l2.5 2.5a.75.75 0 001.137-.089l4-5.5z"
              clip-rule="evenodd"
            />
          </svg>
          <LoadingSpinner v-else size="sm" />
          <span>{{ loading ? 'Verifying...' : 'Verify Integrity' }}</span>
        </button>
      </div>
    </section>

    <!-- Error Alert -->
    <ErrorAlert v-if="error" :error="error" :on-retry="verifyIntegrity" class="mb-6" />

    <!-- Verification Results -->
    <section
      v-if="verificationResult"
      class="rounded-lg border bg-white shadow-sm"
      :class="isValid ? 'border-green-200' : 'border-red-200'"
      aria-label="Verification results"
    >
      <!-- Results Header -->
      <div
        class="border-b px-6 py-4"
        :class="isValid ? 'border-green-200 bg-green-50' : 'border-red-200 bg-red-50'"
      >
        <div class="flex items-center gap-3">
          <!-- Status Icon -->
          <div class="flex-shrink-0">
            <svg
              v-if="isValid"
              class="h-8 w-8 text-green-600"
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
            <svg
              v-else
              class="h-8 w-8 text-red-600"
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

          <!-- Status Message -->
          <div class="flex-1">
            <h3
              class="text-lg font-semibold"
              :class="isValid ? 'text-green-900' : 'text-red-900'"
            >
              {{ isValid ? 'Data Integrity Verified' : 'Integrity Issues Detected' }}
            </h3>
            <p
              class="mt-1 text-sm"
              :class="isValid ? 'text-green-700' : 'text-red-700'"
            >
              {{
                isValid
                  ? 'All records passed cryptographic verification. No tampering detected.'
                  : 'Some records failed verification. Please review the issues below.'
              }}
            </p>
          </div>
        </div>
      </div>

      <!-- Verification Statistics -->
      <div class="border-b border-gray-200 bg-gray-50 px-6 py-4">
        <dl class="grid grid-cols-1 gap-4 sm:grid-cols-3">
          <div>
            <dt class="text-sm font-medium text-gray-500">Total Records Checked</dt>
            <dd class="mt-1 text-2xl font-semibold text-gray-900">
              {{ totalRecordsChecked.toLocaleString() }}
            </dd>
          </div>
          <div>
            <dt class="text-sm font-medium text-gray-500">Valid Records</dt>
            <dd class="mt-1 text-2xl font-semibold text-green-600">
              {{
                (totalRecordsChecked - invalidRecords.length).toLocaleString()
              }}
            </dd>
          </div>
          <div>
            <dt class="text-sm font-medium text-gray-500">Invalid Records</dt>
            <dd class="mt-1 text-2xl font-semibold text-red-600">
              {{ invalidRecords.length.toLocaleString() }}
            </dd>
          </div>
        </dl>
      </div>

      <!-- Verification Timestamp -->
      <div class="border-b border-gray-200 px-6 py-3">
        <p class="text-sm text-gray-600">
          <span class="font-medium">Verified at:</span>
          {{ formatDateTime(verifiedAt) }}
        </p>
      </div>

      <!-- Invalid Records List -->
      <div v-if="invalidRecords.length > 0" class="px-6 py-4">
        <h4 class="mb-4 text-base font-semibold text-gray-900">Invalid Records</h4>
        <div class="overflow-hidden rounded-lg border border-red-200">
          <table class="min-w-full divide-y divide-red-200">
            <thead class="bg-red-50">
              <tr>
                <th
                  scope="col"
                  class="px-4 py-3 text-left text-xs font-medium uppercase tracking-wider text-red-900"
                >
                  Entity Type
                </th>
                <th
                  scope="col"
                  class="px-4 py-3 text-left text-xs font-medium uppercase tracking-wider text-red-900"
                >
                  Entity ID
                </th>
                <th
                  scope="col"
                  class="px-4 py-3 text-left text-xs font-medium uppercase tracking-wider text-red-900"
                >
                  Issue
                </th>
              </tr>
            </thead>
            <tbody class="divide-y divide-red-100 bg-white">
              <tr
                v-for="(record, index) in invalidRecords"
                :key="index"
                class="transition-colors hover:bg-red-50"
              >
                <td class="whitespace-nowrap px-4 py-3 text-sm font-medium text-gray-900">
                  {{ record.entityType }}
                </td>
                <td class="whitespace-nowrap px-4 py-3 text-sm text-gray-700">
                  {{ record.entityId }}
                </td>
                <td class="px-4 py-3 text-sm text-red-700">
                  {{ record.issue }}
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Success Message (No Invalid Records) -->
      <div v-else class="px-6 py-8 text-center">
        <svg
          class="mx-auto h-12 w-12 text-green-500"
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
            d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"
          />
        </svg>
        <h3 class="mt-2 text-sm font-medium text-gray-900">All Records Valid</h3>
        <p class="mt-1 text-sm text-gray-500">
          All {{ totalRecordsChecked }} records passed cryptographic verification.
        </p>
      </div>
    </section>

    <!-- Initial State (No Verification Run Yet) -->
    <div
      v-if="!verificationResult && !loading && !error"
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
          d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z"
        />
      </svg>
      <h3 class="mt-2 text-sm font-medium text-gray-900">No Verification Run Yet</h3>
      <p class="mt-1 text-sm text-gray-500">
        Click the "Verify Integrity" button above to check data integrity.
      </p>
    </div>
  </div>
</template>
