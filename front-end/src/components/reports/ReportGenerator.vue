<script setup lang="ts">
import { ref, computed } from 'vue'
import { reportService } from '@/services/api/reportService'
import type { GenerateReportRequest, AccountabilityReportResponse, ApiError } from '@/types/api'
import ErrorAlert from '@/components/common/ErrorAlert.vue'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'

interface Props {
  projectId: string
}

const props = defineProps<Props>()

// Form state
const includeAuditTrail = ref(true)
const startDate = ref('')
const endDate = ref('')

// Component state
const loading = ref(false)
const error = ref<ApiError | null>(null)
const generatedReport = ref<AccountabilityReportResponse | null>(null)
const downloadingReport = ref(false)
const downloadError = ref<ApiError | null>(null)

// Validation
const isFormValid = computed(() => {
  // If dates are provided, start date must be before end date
  if (startDate.value && endDate.value) {
    return new Date(startDate.value) <= new Date(endDate.value)
  }
  return true
})

const dateRangeError = computed(() => {
  if (startDate.value && endDate.value && new Date(startDate.value) > new Date(endDate.value)) {
    return 'Start date must be before or equal to end date'
  }
  return null
})

/**
 * Generate accountability report
 */
async function generateReport() {
  if (!isFormValid.value) {
    return
  }

  loading.value = true
  error.value = null
  generatedReport.value = null

  try {
    const request: GenerateReportRequest = {
      includeAuditTrail: includeAuditTrail.value,
      startDate: startDate.value || undefined,
      endDate: endDate.value || undefined,
    }

    const report = await reportService.generateAccountabilityReport(props.projectId, request)
    generatedReport.value = report
  } catch (err: any) {
    error.value = {
      message: err.response?.data?.message || err.message || 'Failed to generate report',
      errors: err.response?.data?.errors,
      statusCode: err.response?.status || 500,
    }
  } finally {
    loading.value = false
  }
}

/**
 * Download generated report as PDF
 */
async function downloadReport() {
  if (!generatedReport.value) {
    return
  }

  downloadingReport.value = true
  downloadError.value = null

  try {
    const blob = await reportService.downloadReport(generatedReport.value.id)
    
    // Create download link and trigger download
    const url = URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `accountability-report-${generatedReport.value.reportIdentifier}.pdf`
    link.click()
    
    // Clean up
    URL.revokeObjectURL(url)
  } catch (err: any) {
    downloadError.value = {
      message: err.response?.data?.message || err.message || 'Failed to download report',
      errors: err.response?.data?.errors,
      statusCode: err.response?.status || 500,
    }
  } finally {
    downloadingReport.value = false
  }
}

/**
 * Reset form and generated report
 */
function resetForm() {
  includeAuditTrail.value = true
  startDate.value = ''
  endDate.value = ''
  generatedReport.value = null
  error.value = null
  downloadError.value = null
}

/**
 * Retry report generation
 */
function retryGeneration() {
  generateReport()
}
</script>

<template>
  <div class="report-generator space-y-6">
    <!-- Header -->
    <div class="border-b border-gray-200 pb-4">
      <h2 class="text-2xl font-semibold text-gray-900">
        Generate Accountability Report
      </h2>
      <p class="mt-1 text-sm text-gray-600">
        Create a comprehensive report with project details, transactions, and integrity verification
      </p>
    </div>

    <!-- Error Display -->
    <ErrorAlert v-if="error" :error="error" :on-retry="retryGeneration" />

    <!-- Report Generation Form -->
    <form
      v-if="!generatedReport"
      @submit.prevent="generateReport"
      class="space-y-6 rounded-lg border border-gray-200 bg-white p-6 shadow-sm"
    >
      <fieldset :disabled="loading" class="space-y-6">
        <legend class="sr-only">Report Parameters</legend>

        <!-- Include Audit Trail -->
        <div class="flex items-start">
          <div class="flex h-5 items-center">
            <input
              id="includeAuditTrail"
              v-model="includeAuditTrail"
              type="checkbox"
              class="h-4 w-4 rounded border-gray-300 text-blue-600 focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
              aria-describedby="includeAuditTrail-description"
            />
          </div>
          <div class="ml-3">
            <label for="includeAuditTrail" class="font-medium text-gray-900">
              Include Audit Trail
            </label>
            <p id="includeAuditTrail-description" class="text-sm text-gray-600">
              Include detailed audit trail information showing all system actions and changes
            </p>
          </div>
        </div>

        <!-- Date Range -->
        <div class="space-y-4">
          <h3 class="text-sm font-medium text-gray-900">
            Date Range (Optional)
          </h3>
          <p class="text-sm text-gray-600">
            Filter transactions by date range. Leave empty to include all transactions.
          </p>

          <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <!-- Start Date -->
            <div>
              <label for="startDate" class="block text-sm font-medium text-gray-700">
                Start Date
              </label>
              <input
                id="startDate"
                v-model="startDate"
                type="date"
                class="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2 shadow-sm focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:cursor-not-allowed disabled:bg-gray-50 disabled:text-gray-500"
                :aria-invalid="!!dateRangeError"
                :aria-describedby="dateRangeError ? 'date-range-error' : undefined"
              />
            </div>

            <!-- End Date -->
            <div>
              <label for="endDate" class="block text-sm font-medium text-gray-700">
                End Date
              </label>
              <input
                id="endDate"
                v-model="endDate"
                type="date"
                class="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2 shadow-sm focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:cursor-not-allowed disabled:bg-gray-50 disabled:text-gray-500"
                :aria-invalid="!!dateRangeError"
                :aria-describedby="dateRangeError ? 'date-range-error' : undefined"
              />
            </div>
          </div>

          <!-- Date Range Validation Error -->
          <p
            v-if="dateRangeError"
            id="date-range-error"
            class="text-sm text-red-600"
            role="alert"
          >
            {{ dateRangeError }}
          </p>
        </div>

        <!-- Submit Button -->
        <div class="flex items-center justify-end gap-3 border-t border-gray-200 pt-4">
          <button
            type="submit"
            :disabled="loading || !isFormValid"
            class="inline-flex items-center gap-2 rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white shadow-sm transition-colors hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
            :aria-busy="loading"
          >
            <LoadingSpinner v-if="loading" size="sm" message="Generating report..." />
            <svg
              v-else
              class="h-5 w-5"
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 20 20"
              fill="currentColor"
              aria-hidden="true"
            >
              <path
                fill-rule="evenodd"
                d="M4.5 2A1.5 1.5 0 003 3.5v13A1.5 1.5 0 004.5 18h11a1.5 1.5 0 001.5-1.5V7.621a1.5 1.5 0 00-.44-1.06l-4.12-4.122A1.5 1.5 0 0011.378 2H4.5zm2.25 8.5a.75.75 0 000 1.5h6.5a.75.75 0 000-1.5h-6.5zm0 3a.75.75 0 000 1.5h6.5a.75.75 0 000-1.5h-6.5z"
                clip-rule="evenodd"
              />
            </svg>
            {{ loading ? 'Generating...' : 'Generate Report' }}
          </button>
        </div>
      </fieldset>
    </form>

    <!-- Generated Report Summary -->
    <div
      v-if="generatedReport"
      class="space-y-6 rounded-lg border border-green-200 bg-green-50 p-6 shadow-sm"
    >
      <!-- Success Header -->
      <div class="flex items-start gap-3">
        <div class="flex-shrink-0">
          <svg
            class="h-6 w-6 text-green-600"
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
          <h3 class="text-lg font-semibold text-green-900">
            Report Generated Successfully
          </h3>
          <p class="mt-1 text-sm text-green-700">
            Your accountability report is ready for download
          </p>
        </div>
      </div>

      <!-- Report Details -->
      <dl class="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <div>
          <dt class="text-sm font-medium text-green-900">Report ID</dt>
          <dd class="mt-1 text-sm text-green-700">{{ generatedReport.reportIdentifier }}</dd>
        </div>
        <div>
          <dt class="text-sm font-medium text-green-900">Project</dt>
          <dd class="mt-1 text-sm text-green-700">{{ generatedReport.projectName }}</dd>
        </div>
        <div>
          <dt class="text-sm font-medium text-green-900">Generated At</dt>
          <dd class="mt-1 text-sm text-green-700">
            {{ new Date(generatedReport.generatedAt).toLocaleString() }}
          </dd>
        </div>
        <div>
          <dt class="text-sm font-medium text-green-900">Transactions</dt>
          <dd class="mt-1 text-sm text-green-700">
            {{ generatedReport.transactions.length }} transaction(s)
          </dd>
        </div>
        <div v-if="generatedReport.auditEntries.length > 0">
          <dt class="text-sm font-medium text-green-900">Audit Entries</dt>
          <dd class="mt-1 text-sm text-green-700">
            {{ generatedReport.auditEntries.length }} entry(ies)
          </dd>
        </div>
        <div>
          <dt class="text-sm font-medium text-green-900">Integrity Status</dt>
          <dd class="mt-1 text-sm text-green-700">
            <span
              :class="[
                'inline-flex items-center gap-1 rounded-full px-2 py-1 text-xs font-medium',
                generatedReport.integrityReport.isIntegrityValid
                  ? 'bg-green-100 text-green-800'
                  : 'bg-red-100 text-red-800',
              ]"
            >
              {{ generatedReport.integrityReport.isIntegrityValid ? 'Valid' : 'Invalid' }}
            </span>
          </dd>
        </div>
      </dl>

      <!-- Download Error -->
      <ErrorAlert v-if="downloadError" :error="downloadError" :on-retry="downloadReport" />

      <!-- Action Buttons -->
      <div class="flex items-center justify-end gap-3 border-t border-green-200 pt-4">
        <button
          type="button"
          @click="resetForm"
          class="inline-flex items-center gap-2 rounded-md border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 shadow-sm transition-colors hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
        >
          Generate New Report
        </button>
        <button
          type="button"
          @click="downloadReport"
          :disabled="downloadingReport"
          class="inline-flex items-center gap-2 rounded-md bg-green-600 px-4 py-2 text-sm font-medium text-white shadow-sm transition-colors hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-green-500 focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
          :aria-busy="downloadingReport"
        >
          <LoadingSpinner
            v-if="downloadingReport"
            size="sm"
            message="Downloading report..."
          />
          <svg
            v-else
            class="h-5 w-5"
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 20 20"
            fill="currentColor"
            aria-hidden="true"
          >
            <path
              d="M10.75 2.75a.75.75 0 00-1.5 0v8.614L6.295 8.235a.75.75 0 10-1.09 1.03l4.25 4.5a.75.75 0 001.09 0l4.25-4.5a.75.75 0 00-1.09-1.03l-2.955 3.129V2.75z"
            />
            <path
              d="M3.5 12.75a.75.75 0 00-1.5 0v2.5A2.75 2.75 0 004.75 18h10.5A2.75 2.75 0 0018 15.25v-2.5a.75.75 0 00-1.5 0v2.5c0 .69-.56 1.25-1.25 1.25H4.75c-.69 0-1.25-.56-1.25-1.25v-2.5z"
            />
          </svg>
          {{ downloadingReport ? 'Downloading...' : 'Download PDF' }}
        </button>
      </div>
    </div>

    <!-- Progress Indicator (shown during generation) -->
    <div
      v-if="loading"
      class="flex flex-col items-center justify-center space-y-4 rounded-lg border border-blue-200 bg-blue-50 p-8"
      role="status"
      aria-live="polite"
    >
      <LoadingSpinner size="lg" message="Generating accountability report..." />
      <div class="text-center">
        <p class="text-sm font-medium text-blue-900">Generating Report</p>
        <p class="mt-1 text-sm text-blue-700">
          Please wait while we compile your accountability report...
        </p>
      </div>
    </div>
  </div>
</template>
