<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useProjects } from '@/composables/useProjects'
import { ProjectStatus } from '@/types/api'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import ErrorAlert from '@/components/common/ErrorAlert.vue'

const router = useRouter()
const { projects, loading, error, fetchProjects } = useProjects()

// Pagination state
const currentPage = ref(1)
const pageSize = ref(10)
const totalPages = ref(1)
const totalCount = ref(0)

// Filter state
const selectedStatus = ref<string>('')

// Status options for filter dropdown
const statusOptions = [
  { value: '', label: 'All Statuses' },
  { value: ProjectStatus.NotStarted, label: 'Not Started' },
  { value: ProjectStatus.InProgress, label: 'In Progress' },
  { value: ProjectStatus.Completed, label: 'Completed' },
  { value: ProjectStatus.Cancelled, label: 'Cancelled' },
]

// Load projects
const loadProjects = async () => {
  try {
    const params: any = {
      pageNumber: currentPage.value,
      pageSize: pageSize.value,
    }
    
    if (selectedStatus.value) {
      params.status = selectedStatus.value
    }
    
    const response = await fetchProjects(params)
    if (response) {
      totalPages.value = response.totalPages
      totalCount.value = response.totalCount
    }
  } catch (err) {
    // Error is handled by the composable
  }
}

// Handle status filter change
const onStatusChange = () => {
  currentPage.value = 1 // Reset to first page
  loadProjects()
}

// Handle page change
const goToPage = (page: number) => {
  if (page >= 1 && page <= totalPages.value) {
    currentPage.value = page
    loadProjects()
  }
}

// Navigate to project details
const viewProject = (projectId: string) => {
  router.push({ name: 'ProjectDetails', params: { id: projectId } })
}

// Navigate to create project
const createNewProject = () => {
  router.push({ name: 'ProjectCreate' })
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

// Get status badge color
const getStatusColor = (status: ProjectStatus) => {
  const colors = {
    [ProjectStatus.NotStarted]: 'bg-gray-100 text-gray-800',
    [ProjectStatus.InProgress]: 'bg-blue-100 text-blue-800',
    [ProjectStatus.Completed]: 'bg-green-100 text-green-800',
    [ProjectStatus.Cancelled]: 'bg-red-100 text-red-800',
  }
  return colors[status] || 'bg-gray-100 text-gray-800'
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

// Load projects on mount
onMounted(() => {
  loadProjects()
})
</script>

<template>
  <div class="project-list-container">
    <!-- Header -->
    <header class="mb-6">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-3xl font-bold text-gray-900">Projects</h1>
          <p class="mt-1 text-sm text-gray-600">
            Manage and track all your projects
          </p>
        </div>
        <button
          type="button"
          @click="createNewProject"
          class="inline-flex items-center gap-2 rounded-md bg-blue-600 px-4 py-2 text-sm font-semibold text-white shadow-sm transition-colors hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
          aria-label="Create new project"
        >
          <svg
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
          Create Project
        </button>
      </div>
    </header>

    <!-- Filters -->
    <section class="mb-6" aria-label="Project filters">
      <div class="flex items-center gap-4">
        <label for="status-filter" class="text-sm font-medium text-gray-700">
          Filter by Status:
        </label>
        <select
          id="status-filter"
          v-model="selectedStatus"
          @change="onStatusChange"
          class="block rounded-md border-gray-300 py-2 pl-3 pr-10 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
          aria-label="Filter projects by status"
        >
          <option
            v-for="option in statusOptions"
            :key="option.value"
            :value="option.value"
          >
            {{ option.label }}
          </option>
        </select>
      </div>
    </section>

    <!-- Error Alert -->
    <ErrorAlert
      v-if="error"
      :error="error"
      :on-retry="loadProjects"
      class="mb-6"
    />

    <!-- Loading State -->
    <div
      v-if="loading && projects.length === 0"
      class="flex items-center justify-center py-12"
      aria-live="polite"
      aria-busy="true"
    >
      <LoadingSpinner size="lg" message="Loading projects..." />
    </div>

    <!-- Projects Table -->
    <section
      v-else-if="!loading && projects.length > 0"
      class="overflow-hidden rounded-lg border border-gray-200 bg-white shadow"
      aria-label="Projects table"
    >
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th
                scope="col"
                class="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500"
              >
                Project Name
              </th>
              <th
                scope="col"
                class="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500"
              >
                Status
              </th>
              <th
                scope="col"
                class="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500"
              >
                Budget
              </th>
              <th
                scope="col"
                class="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500"
              >
                Start Date
              </th>
              <th
                scope="col"
                class="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500"
              >
                End Date
              </th>
              <th scope="col" class="relative px-6 py-3">
                <span class="sr-only">Actions</span>
              </th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-200 bg-white">
            <tr
              v-for="project in projects"
              :key="project.id"
              class="transition-colors hover:bg-gray-50"
            >
              <td class="whitespace-nowrap px-6 py-4">
                <div class="flex flex-col">
                  <div class="text-sm font-medium text-gray-900">
                    {{ project.name }}
                  </div>
                  <div
                    v-if="project.description"
                    class="text-sm text-gray-500 line-clamp-1"
                  >
                    {{ project.description }}
                  </div>
                </div>
              </td>
              <td class="whitespace-nowrap px-6 py-4">
                <span
                  :class="getStatusColor(project.status)"
                  class="inline-flex rounded-full px-2 py-1 text-xs font-semibold leading-5"
                >
                  {{ getStatusLabel(project.status) }}
                </span>
              </td>
              <td class="whitespace-nowrap px-6 py-4 text-sm text-gray-900">
                {{ formatCurrency(project.budgetAmount) }}
              </td>
              <td class="whitespace-nowrap px-6 py-4 text-sm text-gray-500">
                {{ formatDate(project.startDate) }}
              </td>
              <td class="whitespace-nowrap px-6 py-4 text-sm text-gray-500">
                {{ formatDate(project.endDate) }}
              </td>
              <td
                class="whitespace-nowrap px-6 py-4 text-right text-sm font-medium"
              >
                <button
                  type="button"
                  @click="viewProject(project.id)"
                  class="text-blue-600 transition-colors hover:text-blue-900 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
                  :aria-label="`View details for ${project.name}`"
                >
                  View Details
                </button>
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
      v-else-if="!loading && projects.length === 0"
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
      <h3 class="mt-2 text-sm font-medium text-gray-900">No projects found</h3>
      <p class="mt-1 text-sm text-gray-500">
        Get started by creating a new project.
      </p>
      <div class="mt-6">
        <button
          type="button"
          @click="createNewProject"
          class="inline-flex items-center gap-2 rounded-md bg-blue-600 px-4 py-2 text-sm font-semibold text-white shadow-sm transition-colors hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
        >
          <svg
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
          Create Project
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.line-clamp-1 {
  display: -webkit-box;
  -webkit-line-clamp: 1;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>
