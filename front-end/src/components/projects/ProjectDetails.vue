<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useProjects } from '@/composables/useProjects'
import { ProjectStatus } from '@/types/api'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import ErrorAlert from '@/components/common/ErrorAlert.vue'

interface Props {
  projectId: string
}

const props = defineProps<Props>()
const router = useRouter()
const { currentProject, loading, error, fetchProject } = useProjects()

// Load project details
const loadProject = async () => {
  try {
    await fetchProject(props.projectId)
  } catch (err) {
    // Error is handled by the composable
  }
}

// Navigate to edit form
const editProject = () => {
  router.push({ name: 'ProjectEdit', params: { id: props.projectId } })
}

// Navigate back to list
const goBack = () => {
  router.push({ name: 'ProjectList' })
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
    month: 'long',
    day: 'numeric',
  })
}

// Format datetime
const formatDateTime = (dateString: string) => {
  return new Date(dateString).toLocaleString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })
}

// Get status badge color
const getStatusColor = (status: ProjectStatus) => {
  const colors = {
    [ProjectStatus.NotStarted]: 'bg-gray-100 text-gray-800 border-gray-300',
    [ProjectStatus.InProgress]: 'bg-blue-100 text-blue-800 border-blue-300',
    [ProjectStatus.Completed]: 'bg-green-100 text-green-800 border-green-300',
    [ProjectStatus.Cancelled]: 'bg-red-100 text-red-800 border-red-300',
  }
  return colors[status] || 'bg-gray-100 text-gray-800 border-gray-300'
}

// Get status label
const getStatusLabel = (status: ProjectStatus) => {
  const labels = {
    [ProjectStatus.NotStarted]: 'Not Started',
    [ProjectStatus.InProgress]: 'In Progress',
    [ProjectStatus.Completed]: 'Completed',
    [ProjectStatus.Cancelled]: 'Cancelled',
  }
  return labels[status] || status
}

// Get status icon
const getStatusIcon = (status: ProjectStatus) => {
  const icons = {
    [ProjectStatus.NotStarted]: 'M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z',
    [ProjectStatus.InProgress]: 'M13 10V3L4 14h7v7l9-11h-7z',
    [ProjectStatus.Completed]: 'M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z',
    [ProjectStatus.Cancelled]: 'M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z',
  }
  return icons[status] || icons[ProjectStatus.NotStarted]
}

// Load project on mount
onMounted(() => {
  loadProject()
})

// Reload project when projectId changes
watch(() => props.projectId, () => {
  loadProject()
})
</script>

<template>
  <div class="project-details-container">
    <!-- Header with Back Button -->
    <header class="mb-6">
      <button
        type="button"
        @click="goBack"
        class="mb-4 inline-flex items-center gap-2 text-sm font-medium text-gray-600 transition-colors hover:text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
        aria-label="Go back to projects list"
      >
        <svg
          class="h-5 w-5"
          xmlns="http://www.w3.org/2000/svg"
          viewBox="0 0 20 20"
          fill="currentColor"
          aria-hidden="true"
        >
          <path
            fill-rule="evenodd"
            d="M17 10a.75.75 0 01-.75.75H5.612l4.158 3.96a.75.75 0 11-1.04 1.08l-5.5-5.25a.75.75 0 010-1.08l5.5-5.25a.75.75 0 111.04 1.08L5.612 9.25H16.25A.75.75 0 0117 10z"
            clip-rule="evenodd"
          />
        </svg>
        Back to Projects
      </button>
    </header>

    <!-- Error Alert -->
    <ErrorAlert
      v-if="error"
      :error="error"
      :on-retry="loadProject"
      class="mb-6"
    />

    <!-- Loading State -->
    <div
      v-if="loading && !currentProject"
      class="flex items-center justify-center py-12"
      aria-live="polite"
      aria-busy="true"
    >
      <LoadingSpinner size="lg" message="Loading project details..." />
    </div>

    <!-- Project Details -->
    <article
      v-else-if="currentProject"
      class="overflow-hidden rounded-lg border border-gray-200 bg-white shadow"
    >
      <!-- Project Header -->
      <header class="border-b border-gray-200 bg-gray-50 px-6 py-4">
        <div class="flex items-start justify-between">
          <div class="flex-1">
            <h1 class="text-2xl font-bold text-gray-900">
              {{ currentProject.name }}
            </h1>
            <p
              v-if="currentProject.description"
              class="mt-2 text-sm text-gray-600"
            >
              {{ currentProject.description }}
            </p>
          </div>
          <button
            type="button"
            @click="editProject"
            class="ml-4 inline-flex items-center gap-2 rounded-md bg-blue-600 px-4 py-2 text-sm font-semibold text-white shadow-sm transition-colors hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
            aria-label="Edit project"
          >
            <svg
              class="h-4 w-4"
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 20 20"
              fill="currentColor"
              aria-hidden="true"
            >
              <path
                d="M5.433 13.917l1.262-3.155A4 4 0 017.58 9.42l6.92-6.918a2.121 2.121 0 013 3l-6.92 6.918c-.383.383-.84.685-1.343.886l-3.154 1.262a.5.5 0 01-.65-.65z"
              />
              <path
                d="M3.5 5.75c0-.69.56-1.25 1.25-1.25H10A.75.75 0 0010 3H4.75A2.75 2.75 0 002 5.75v9.5A2.75 2.75 0 004.75 18h9.5A2.75 2.75 0 0017 15.25V10a.75.75 0 00-1.5 0v5.25c0 .69-.56 1.25-1.25 1.25h-9.5c-.69 0-1.25-.56-1.25-1.25v-9.5z"
              />
            </svg>
            Edit Project
          </button>
        </div>
      </header>

      <!-- Project Information -->
      <section class="px-6 py-6">
        <!-- Status Badge -->
        <div class="mb-6">
          <h2 class="mb-2 text-sm font-medium text-gray-500">Status</h2>
          <div
            :class="getStatusColor(currentProject.status)"
            class="inline-flex items-center gap-2 rounded-lg border px-3 py-2"
            role="status"
            :aria-label="`Project status: ${getStatusLabel(currentProject.status)}`"
          >
            <svg
              class="h-5 w-5"
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
                :d="getStatusIcon(currentProject.status)"
              />
            </svg>
            <span class="text-sm font-semibold">
              {{ getStatusLabel(currentProject.status) }}
            </span>
          </div>
        </div>

        <!-- Project Details Grid -->
        <dl class="grid grid-cols-1 gap-6 sm:grid-cols-2">
          <!-- Budget -->
          <div>
            <dt class="text-sm font-medium text-gray-500">Budget Amount</dt>
            <dd class="mt-1 text-lg font-semibold text-gray-900">
              {{ formatCurrency(currentProject.budgetAmount) }}
            </dd>
          </div>

          <!-- Start Date -->
          <div>
            <dt class="text-sm font-medium text-gray-500">Start Date</dt>
            <dd class="mt-1 text-lg font-semibold text-gray-900">
              {{ formatDate(currentProject.startDate) }}
            </dd>
          </div>

          <!-- End Date -->
          <div>
            <dt class="text-sm font-medium text-gray-500">End Date</dt>
            <dd class="mt-1 text-lg font-semibold text-gray-900">
              {{ formatDate(currentProject.endDate) }}
            </dd>
          </div>

          <!-- Coordinator ID -->
          <div>
            <dt class="text-sm font-medium text-gray-500">Coordinator ID</dt>
            <dd class="mt-1 text-lg font-semibold text-gray-900">
              {{ currentProject.coordinatorId }}
            </dd>
          </div>
        </dl>

        <!-- Bank Account Information -->
        <div v-if="currentProject.bankAccount" class="mt-8">
          <h2 class="mb-4 text-lg font-semibold text-gray-900">
            Bank Account Information
          </h2>
          <div
            class="rounded-lg border border-gray-200 bg-gray-50 p-4"
            role="region"
            aria-label="Bank account details"
          >
            <dl class="grid grid-cols-1 gap-4 sm:grid-cols-2">
              <!-- Account Holder -->
              <div>
                <dt class="text-sm font-medium text-gray-500">
                  Account Holder
                </dt>
                <dd class="mt-1 text-sm font-semibold text-gray-900">
                  {{ currentProject.bankAccount.accountHolderName }}
                </dd>
              </div>

              <!-- Bank Name -->
              <div>
                <dt class="text-sm font-medium text-gray-500">Bank Name</dt>
                <dd class="mt-1 text-sm font-semibold text-gray-900">
                  {{ currentProject.bankAccount.bankName }}
                </dd>
              </div>

              <!-- Account Number -->
              <div>
                <dt class="text-sm font-medium text-gray-500">
                  Account Number
                </dt>
                <dd class="mt-1 text-sm font-semibold text-gray-900">
                  {{ currentProject.bankAccount.accountNumber }}
                </dd>
              </div>

              <!-- Branch Number -->
              <div>
                <dt class="text-sm font-medium text-gray-500">
                  Branch Number
                </dt>
                <dd class="mt-1 text-sm font-semibold text-gray-900">
                  {{ currentProject.bankAccount.branchNumber }}
                </dd>
              </div>
            </dl>
          </div>
        </div>

        <!-- No Bank Account -->
        <div v-else class="mt-8">
          <h2 class="mb-4 text-lg font-semibold text-gray-900">
            Bank Account Information
          </h2>
          <div
            class="rounded-lg border-2 border-dashed border-gray-300 bg-gray-50 p-6 text-center"
          >
            <svg
              class="mx-auto h-8 w-8 text-gray-400"
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
                d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z"
              />
            </svg>
            <p class="mt-2 text-sm text-gray-500">
              No bank account information available
            </p>
          </div>
        </div>

        <!-- Metadata -->
        <div class="mt-8 border-t border-gray-200 pt-6">
          <h2 class="mb-4 text-sm font-medium text-gray-500">Metadata</h2>
          <dl class="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <!-- Created At -->
            <div>
              <dt class="text-xs font-medium text-gray-500">Created At</dt>
              <dd class="mt-1 text-sm text-gray-900">
                {{ formatDateTime(currentProject.createdAt) }}
              </dd>
            </div>

            <!-- Updated At -->
            <div>
              <dt class="text-xs font-medium text-gray-500">Last Updated</dt>
              <dd class="mt-1 text-sm text-gray-900">
                {{ formatDateTime(currentProject.updatedAt) }}
              </dd>
            </div>

            <!-- Project ID -->
            <div class="sm:col-span-2">
              <dt class="text-xs font-medium text-gray-500">Project ID</dt>
              <dd class="mt-1 font-mono text-xs text-gray-900">
                {{ currentProject.id }}
              </dd>
            </div>
          </dl>
        </div>
      </section>
    </article>
  </div>
</template>
