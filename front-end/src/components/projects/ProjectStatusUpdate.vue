<script setup lang="ts">
import { ref, computed } from 'vue'
import { useProjects } from '@/composables/useProjects'
import { ProjectStatus } from '@/types/api'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import ErrorAlert from '@/components/common/ErrorAlert.vue'

interface Props {
  projectId: string
  currentStatus: ProjectStatus
}

const props = defineProps<Props>()

const emit = defineEmits<{
  (e: 'statusUpdated', status: ProjectStatus): void
}>()

const { updateStatus } = useProjects()

// Local state
const selectedStatus = ref<ProjectStatus>(props.currentStatus)
const isUpdating = ref(false)
const updateError = ref<string | null>(null)
const showSuccessMessage = ref(false)

// Status options
const statusOptions = [
  {
    value: ProjectStatus.NotStarted,
    label: 'Not Started',
    description: 'Project has not begun yet',
    color: 'text-gray-700',
    icon: 'M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z',
  },
  {
    value: ProjectStatus.InProgress,
    label: 'In Progress',
    description: 'Project is currently active',
    color: 'text-blue-700',
    icon: 'M13 10V3L4 14h7v7l9-11h-7z',
  },
  {
    value: ProjectStatus.Completed,
    label: 'Completed',
    description: 'Project has been finished',
    color: 'text-green-700',
    icon: 'M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z',
  },
  {
    value: ProjectStatus.Cancelled,
    label: 'Cancelled',
    description: 'Project has been cancelled',
    color: 'text-red-700',
    icon: 'M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z',
  },
]

// Computed
const hasStatusChanged = computed(() => selectedStatus.value !== props.currentStatus)

const selectedStatusOption = computed(() => {
  return statusOptions.find(option => option.value === selectedStatus.value)
})

// Handle status update
const handleUpdateStatus = async () => {
  if (!hasStatusChanged.value) {
    return
  }

  updateError.value = null
  showSuccessMessage.value = false
  isUpdating.value = true

  try {
    await updateStatus(props.projectId, selectedStatus.value)
    
    // Show success message
    showSuccessMessage.value = true
    
    // Emit event
    emit('statusUpdated', selectedStatus.value)
    
    // Hide success message after 3 seconds
    setTimeout(() => {
      showSuccessMessage.value = false
    }, 3000)
  } catch (err: any) {
    updateError.value = err.message || 'Failed to update project status'
    // Revert to current status on error
    selectedStatus.value = props.currentStatus
  } finally {
    isUpdating.value = false
  }
}

// Handle cancel
const handleCancel = () => {
  selectedStatus.value = props.currentStatus
  updateError.value = null
  showSuccessMessage.value = false
}
</script>

<template>
  <div class="project-status-update">
    <!-- Success Message -->
    <div
      v-if="showSuccessMessage"
      class="mb-4 rounded-lg border border-green-300 bg-green-50 p-4"
      role="alert"
      aria-live="polite"
    >
      <div class="flex items-center gap-3">
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
        <p class="text-sm font-medium text-green-800">
          Project status updated successfully!
        </p>
      </div>
    </div>

    <!-- Error Alert -->
    <ErrorAlert
      v-if="updateError"
      :error="{ message: updateError, statusCode: 400 }"
      :on-retry="handleUpdateStatus"
      class="mb-4"
    />

    <!-- Status Update Form -->
    <div class="rounded-lg border border-gray-200 bg-white p-6 shadow">
      <h3 class="mb-4 text-lg font-semibold text-gray-900">
        Update Project Status
      </h3>

      <!-- Status Options -->
      <fieldset class="space-y-3">
        <legend class="sr-only">Select project status</legend>
        
        <div
          v-for="option in statusOptions"
          :key="option.value"
          class="relative"
        >
          <label
            :for="`status-${option.value}`"
            class="flex cursor-pointer items-start gap-3 rounded-lg border-2 p-4 transition-colors hover:bg-gray-50"
            :class="[
              selectedStatus === option.value
                ? 'border-blue-600 bg-blue-50'
                : 'border-gray-200',
            ]"
          >
            <!-- Radio Input -->
            <input
              :id="`status-${option.value}`"
              v-model="selectedStatus"
              type="radio"
              :value="option.value"
              :disabled="isUpdating"
              class="mt-1 h-4 w-4 border-gray-300 text-blue-600 focus:ring-2 focus:ring-blue-500 disabled:cursor-not-allowed disabled:opacity-50"
              :aria-describedby="`status-${option.value}-description`"
            />

            <!-- Status Icon and Info -->
            <div class="flex flex-1 items-start gap-3">
              <svg
                :class="option.color"
                class="h-6 w-6 flex-shrink-0"
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
                  :d="option.icon"
                />
              </svg>

              <div class="flex-1">
                <div class="flex items-center gap-2">
                  <span class="text-sm font-semibold text-gray-900">
                    {{ option.label }}
                  </span>
                  <span
                    v-if="option.value === currentStatus"
                    class="rounded-full bg-gray-100 px-2 py-0.5 text-xs font-medium text-gray-600"
                  >
                    Current
                  </span>
                </div>
                <p
                  :id="`status-${option.value}-description`"
                  class="mt-1 text-sm text-gray-600"
                >
                  {{ option.description }}
                </p>
              </div>
            </div>

            <!-- Selected Indicator -->
            <svg
              v-if="selectedStatus === option.value"
              class="h-5 w-5 flex-shrink-0 text-blue-600"
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
          </label>
        </div>
      </fieldset>

      <!-- Action Buttons -->
      <div class="mt-6 flex items-center justify-end gap-3">
        <button
          v-if="hasStatusChanged"
          type="button"
          @click="handleCancel"
          :disabled="isUpdating"
          class="rounded-md border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 shadow-sm transition-colors hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
        >
          Cancel
        </button>
        <button
          type="button"
          @click="handleUpdateStatus"
          :disabled="!hasStatusChanged || isUpdating"
          class="inline-flex items-center gap-2 rounded-md bg-blue-600 px-4 py-2 text-sm font-semibold text-white shadow-sm transition-colors hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
          :aria-label="`Update status to ${selectedStatusOption?.label}`"
        >
          <LoadingSpinner v-if="isUpdating" size="sm" />
          <span v-else>Update Status</span>
        </button>
      </div>

      <!-- Help Text -->
      <p
        v-if="!hasStatusChanged"
        class="mt-4 text-sm text-gray-500"
      >
        Select a different status to update the project.
      </p>
    </div>
  </div>
</template>
