<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { useProjects } from '@/composables/useProjects'
import { 
  TransactionList, 
  TransactionForm, 
  AccountBalance 
} from '@/components/transactions'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import ErrorAlert from '@/components/common/ErrorAlert.vue'

const route = useRoute()
const projectId = ref(route.params.id as string)

const { currentProject, loading, error, fetchProject } = useProjects()

const showForm = ref(false)

// Load project data
const loadProject = async () => {
  try {
    await fetchProject(projectId.value)
  } catch (err) {
    // Error is handled by the composable
  }
}

// Handle transaction created
const handleTransactionCreated = () => {
  showForm.value = false
  // The TransactionList and AccountBalance will auto-refresh via the composable
}

// Handle form cancel
const handleFormCancel = () => {
  showForm.value = false
}

// Load project on mount
onMounted(() => {
  loadProject()
})
</script>

<template>
  <div class="container mx-auto px-4 py-8">
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
      <LoadingSpinner size="lg" message="Loading project..." />
    </div>

    <!-- Content -->
    <div v-else-if="currentProject">
      <!-- Header -->
      <header class="mb-8">
        <div class="flex items-center justify-between">
          <div>
            <h1 class="text-3xl font-bold text-gray-900">Transaction Management</h1>
            <p class="mt-1 text-sm text-gray-600">
              {{ currentProject.name }}
            </p>
          </div>
          <button
            type="button"
            @click="showForm = !showForm"
            class="inline-flex items-center gap-2 rounded-md bg-blue-600 px-4 py-2 text-sm font-semibold text-white shadow-sm transition-colors hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
            :aria-label="showForm ? 'Hide transaction form' : 'Show transaction form'"
          >
            <svg
              v-if="!showForm"
              class="h-5 w-5"
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 20 20"
              fill="currentColor"
              aria-hidden="true"
            >
              <path
                d="M10.75 4.75a.75.75 0 00-1.5 0v4.5h-4.5a.75.75 0 000 1.5h4.5v4.5a.75.75 0 001.5 0v-4.5h4.5a.75.75 0 000-1.5h-4.5v-4.5z"
              />
            </svg>
            {{ showForm ? 'Hide Form' : 'Create Transaction' }}
          </button>
        </div>
      </header>

      <!-- Account Balance -->
      <AccountBalance 
        :project-id="projectId"
        :budget-amount="currentProject.budgetAmount"
        class="mb-8"
      />

      <!-- Transaction Form -->
      <TransactionForm
        v-if="showForm"
        :project-id="projectId"
        @success="handleTransactionCreated"
        @cancel="handleFormCancel"
        class="mb-8"
      />

      <!-- Transaction List -->
      <TransactionList :project-id="projectId" />
    </div>
  </div>
</template>
