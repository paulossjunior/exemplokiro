<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useProjects } from '@/composables/useProjects'
import type { CreateProjectRequest, Project } from '@/types/api'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import ErrorAlert from '@/components/common/ErrorAlert.vue'

interface Props {
  projectId?: string
  mode?: 'create' | 'edit'
}

const props = withDefaults(defineProps<Props>(), {
  mode: 'create',
})

const router = useRouter()
const { currentProject, loading, error, fetchProject, createProject, updateProject } = useProjects()

// Form data
const formData = reactive<CreateProjectRequest>({
  name: '',
  description: null,
  startDate: '',
  endDate: '',
  budgetAmount: 0,
  coordinatorId: '',
  bankAccount: {
    accountNumber: '',
    bankName: '',
    branchNumber: '',
    accountHolderName: '',
  },
})

// Validation errors
const validationErrors = reactive<Record<string, string>>({})

// Form submission state
const isSubmitting = ref(false)
const submitError = ref<string | null>(null)

// Computed properties
const isEditMode = computed(() => props.mode === 'edit')
const formTitle = computed(() => isEditMode.value ? 'Edit Project' : 'Create New Project')
const submitButtonText = computed(() => isEditMode.value ? 'Update Project' : 'Create Project')

// Load project data for edit mode
const loadProjectData = async () => {
  if (isEditMode.value && props.projectId) {
    try {
      await fetchProject(props.projectId)
      if (currentProject.value) {
        populateForm(currentProject.value)
      }
    } catch (err) {
      // Error is handled by the composable
    }
  }
}

// Populate form with project data
const populateForm = (project: Project) => {
  formData.name = project.name
  formData.description = project.description
  formData.startDate = project.startDate.split('T')[0] // Extract date part
  formData.endDate = project.endDate.split('T')[0]
  formData.budgetAmount = project.budgetAmount
  formData.coordinatorId = project.coordinatorId
  
  if (project.bankAccount) {
    formData.bankAccount = {
      accountNumber: project.bankAccount.accountNumber,
      bankName: project.bankAccount.bankName,
      branchNumber: project.bankAccount.branchNumber,
      accountHolderName: project.bankAccount.accountHolderName,
    }
  }
}

// Validate form
const validateForm = (): boolean => {
  // Clear previous errors
  Object.keys(validationErrors).forEach(key => delete validationErrors[key])
  
  let isValid = true
  
  // Name validation
  if (!formData.name || formData.name.trim().length === 0) {
    validationErrors.name = 'Project name is required'
    isValid = false
  } else if (formData.name.length > 200) {
    validationErrors.name = 'Project name must not exceed 200 characters'
    isValid = false
  }
  
  // Start date validation
  if (!formData.startDate) {
    validationErrors.startDate = 'Start date is required'
    isValid = false
  }
  
  // End date validation
  if (!formData.endDate) {
    validationErrors.endDate = 'End date is required'
    isValid = false
  } else if (formData.startDate && formData.endDate < formData.startDate) {
    validationErrors.endDate = 'End date must be after start date'
    isValid = false
  }
  
  // Budget validation
  if (formData.budgetAmount <= 0) {
    validationErrors.budgetAmount = 'Budget amount must be greater than 0'
    isValid = false
  }
  
  // Coordinator ID validation
  if (!formData.coordinatorId || formData.coordinatorId.trim().length === 0) {
    validationErrors.coordinatorId = 'Coordinator ID is required'
    isValid = false
  }
  
  // Bank account validation (if provided)
  if (formData.bankAccount) {
    if (formData.bankAccount.accountNumber && !formData.bankAccount.bankName) {
      validationErrors.bankName = 'Bank name is required when account number is provided'
      isValid = false
    }
    if (formData.bankAccount.accountNumber && !formData.bankAccount.branchNumber) {
      validationErrors.branchNumber = 'Branch number is required when account number is provided'
      isValid = false
    }
    if (formData.bankAccount.accountNumber && !formData.bankAccount.accountHolderName) {
      validationErrors.accountHolderName = 'Account holder name is required when account number is provided'
      isValid = false
    }
  }
  
  return isValid
}

// Handle form submission
const handleSubmit = async () => {
  submitError.value = null
  
  // Validate form
  if (!validateForm()) {
    return
  }
  
  isSubmitting.value = true
  
  try {
    let result
    
    if (isEditMode.value && props.projectId) {
      // Update existing project
      result = await updateProject(props.projectId, {
        name: formData.name,
        description: formData.description,
        startDate: formData.startDate,
        endDate: formData.endDate,
        budgetAmount: formData.budgetAmount,
        coordinatorId: formData.coordinatorId,
      })
    } else {
      // Create new project
      result = await createProject(formData)
    }
    
    // Navigate to project details on success
    if (result) {
      router.push({ name: 'ProjectDetails', params: { id: result.id } })
    }
  } catch (err: any) {
    // Handle API validation errors
    if (err.errors) {
      Object.keys(err.errors).forEach(field => {
        validationErrors[field] = err.errors[field].join(', ')
      })
    }
    submitError.value = err.message || 'Failed to save project'
  } finally {
    isSubmitting.value = false
  }
}

// Cancel and go back
const handleCancel = () => {
  if (isEditMode.value && props.projectId) {
    router.push({ name: 'ProjectDetails', params: { id: props.projectId } })
  } else {
    router.push({ name: 'ProjectList' })
  }
}

// Load project data on mount (for edit mode)
onMounted(() => {
  loadProjectData()
})
</script>

<template>
  <div class="project-form-container">
    <!-- Header -->
    <header class="mb-6">
      <h1 class="text-3xl font-bold text-gray-900">{{ formTitle }}</h1>
      <p class="mt-1 text-sm text-gray-600">
        {{ isEditMode ? 'Update project information' : 'Fill in the details to create a new project' }}
      </p>
    </header>

    <!-- Loading State (for edit mode) -->
    <div
      v-if="loading && isEditMode && !currentProject"
      class="flex items-center justify-center py-12"
      aria-live="polite"
      aria-busy="true"
    >
      <LoadingSpinner size="lg" message="Loading project data..." />
    </div>

    <!-- Form -->
    <form
      v-else
      @submit.prevent="handleSubmit"
      class="space-y-6"
      novalidate
    >
      <!-- Error Alert -->
      <ErrorAlert
        v-if="error || submitError"
        :error="error || { message: submitError || '', statusCode: 400 }"
        class="mb-6"
      />

      <!-- Form Card -->
      <div class="overflow-hidden rounded-lg border border-gray-200 bg-white shadow">
        <!-- Basic Information Section -->
        <section class="border-b border-gray-200 px-6 py-6">
          <h2 class="mb-4 text-lg font-semibold text-gray-900">
            Basic Information
          </h2>
          
          <div class="space-y-4">
            <!-- Project Name -->
            <div>
              <label
                for="project-name"
                class="block text-sm font-medium text-gray-700"
              >
                Project Name <span class="text-red-600">*</span>
              </label>
              <input
                id="project-name"
                v-model="formData.name"
                type="text"
                required
                maxlength="200"
                class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
                :class="{ 'border-red-500': validationErrors.name }"
                :aria-invalid="!!validationErrors.name"
                :aria-describedby="validationErrors.name ? 'name-error' : undefined"
              />
              <p
                v-if="validationErrors.name"
                id="name-error"
                class="mt-1 text-sm text-red-600"
                role="alert"
              >
                {{ validationErrors.name }}
              </p>
            </div>

            <!-- Description -->
            <div>
              <label
                for="project-description"
                class="block text-sm font-medium text-gray-700"
              >
                Description
              </label>
              <textarea
                id="project-description"
                v-model="formData.description"
                rows="3"
                class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
                placeholder="Enter project description..."
              ></textarea>
            </div>

            <!-- Date Range -->
            <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
              <!-- Start Date -->
              <div>
                <label
                  for="start-date"
                  class="block text-sm font-medium text-gray-700"
                >
                  Start Date <span class="text-red-600">*</span>
                </label>
                <input
                  id="start-date"
                  v-model="formData.startDate"
                  type="date"
                  required
                  class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
                  :class="{ 'border-red-500': validationErrors.startDate }"
                  :aria-invalid="!!validationErrors.startDate"
                  :aria-describedby="validationErrors.startDate ? 'start-date-error' : undefined"
                />
                <p
                  v-if="validationErrors.startDate"
                  id="start-date-error"
                  class="mt-1 text-sm text-red-600"
                  role="alert"
                >
                  {{ validationErrors.startDate }}
                </p>
              </div>

              <!-- End Date -->
              <div>
                <label
                  for="end-date"
                  class="block text-sm font-medium text-gray-700"
                >
                  End Date <span class="text-red-600">*</span>
                </label>
                <input
                  id="end-date"
                  v-model="formData.endDate"
                  type="date"
                  required
                  class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
                  :class="{ 'border-red-500': validationErrors.endDate }"
                  :aria-invalid="!!validationErrors.endDate"
                  :aria-describedby="validationErrors.endDate ? 'end-date-error' : undefined"
                />
                <p
                  v-if="validationErrors.endDate"
                  id="end-date-error"
                  class="mt-1 text-sm text-red-600"
                  role="alert"
                >
                  {{ validationErrors.endDate }}
                </p>
              </div>
            </div>

            <!-- Budget Amount -->
            <div>
              <label
                for="budget-amount"
                class="block text-sm font-medium text-gray-700"
              >
                Budget Amount <span class="text-red-600">*</span>
              </label>
              <div class="relative mt-1 rounded-md shadow-sm">
                <div class="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-3">
                  <span class="text-gray-500 sm:text-sm">$</span>
                </div>
                <input
                  id="budget-amount"
                  v-model.number="formData.budgetAmount"
                  type="number"
                  step="0.01"
                  min="0"
                  required
                  class="block w-full rounded-md border-gray-300 pl-7 pr-12 focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
                  :class="{ 'border-red-500': validationErrors.budgetAmount }"
                  :aria-invalid="!!validationErrors.budgetAmount"
                  :aria-describedby="validationErrors.budgetAmount ? 'budget-error' : undefined"
                  placeholder="0.00"
                />
              </div>
              <p
                v-if="validationErrors.budgetAmount"
                id="budget-error"
                class="mt-1 text-sm text-red-600"
                role="alert"
              >
                {{ validationErrors.budgetAmount }}
              </p>
            </div>

            <!-- Coordinator ID -->
            <div>
              <label
                for="coordinator-id"
                class="block text-sm font-medium text-gray-700"
              >
                Coordinator ID <span class="text-red-600">*</span>
              </label>
              <input
                id="coordinator-id"
                v-model="formData.coordinatorId"
                type="text"
                required
                class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
                :class="{ 'border-red-500': validationErrors.coordinatorId }"
                :aria-invalid="!!validationErrors.coordinatorId"
                :aria-describedby="validationErrors.coordinatorId ? 'coordinator-error' : undefined"
              />
              <p
                v-if="validationErrors.coordinatorId"
                id="coordinator-error"
                class="mt-1 text-sm text-red-600"
                role="alert"
              >
                {{ validationErrors.coordinatorId }}
              </p>
            </div>
          </div>
        </section>

        <!-- Bank Account Section -->
        <section v-if="!isEditMode" class="px-6 py-6">
          <h2 class="mb-4 text-lg font-semibold text-gray-900">
            Bank Account Information
          </h2>
          <p class="mb-4 text-sm text-gray-600">
            Optional: Provide bank account details for this project
          </p>
          
          <div class="space-y-4">
            <!-- Account Holder Name -->
            <div>
              <label
                for="account-holder"
                class="block text-sm font-medium text-gray-700"
              >
                Account Holder Name
              </label>
              <input
                id="account-holder"
                v-model="formData.bankAccount!.accountHolderName"
                type="text"
                class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
                :class="{ 'border-red-500': validationErrors.accountHolderName }"
                :aria-invalid="!!validationErrors.accountHolderName"
                :aria-describedby="validationErrors.accountHolderName ? 'holder-error' : undefined"
              />
              <p
                v-if="validationErrors.accountHolderName"
                id="holder-error"
                class="mt-1 text-sm text-red-600"
                role="alert"
              >
                {{ validationErrors.accountHolderName }}
              </p>
            </div>

            <!-- Bank Name -->
            <div>
              <label
                for="bank-name"
                class="block text-sm font-medium text-gray-700"
              >
                Bank Name
              </label>
              <input
                id="bank-name"
                v-model="formData.bankAccount!.bankName"
                type="text"
                class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
                :class="{ 'border-red-500': validationErrors.bankName }"
                :aria-invalid="!!validationErrors.bankName"
                :aria-describedby="validationErrors.bankName ? 'bank-name-error' : undefined"
              />
              <p
                v-if="validationErrors.bankName"
                id="bank-name-error"
                class="mt-1 text-sm text-red-600"
                role="alert"
              >
                {{ validationErrors.bankName }}
              </p>
            </div>

            <!-- Account and Branch Numbers -->
            <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
              <!-- Account Number -->
              <div>
                <label
                  for="account-number"
                  class="block text-sm font-medium text-gray-700"
                >
                  Account Number
                </label>
                <input
                  id="account-number"
                  v-model="formData.bankAccount!.accountNumber"
                  type="text"
                  class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
                />
              </div>

              <!-- Branch Number -->
              <div>
                <label
                  for="branch-number"
                  class="block text-sm font-medium text-gray-700"
                >
                  Branch Number
                </label>
                <input
                  id="branch-number"
                  v-model="formData.bankAccount!.branchNumber"
                  type="text"
                  class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
                  :class="{ 'border-red-500': validationErrors.branchNumber }"
                  :aria-invalid="!!validationErrors.branchNumber"
                  :aria-describedby="validationErrors.branchNumber ? 'branch-error' : undefined"
                />
                <p
                  v-if="validationErrors.branchNumber"
                  id="branch-error"
                  class="mt-1 text-sm text-red-600"
                  role="alert"
                >
                  {{ validationErrors.branchNumber }}
                </p>
              </div>
            </div>
          </div>
        </section>
      </div>

      <!-- Form Actions -->
      <div class="flex items-center justify-end gap-3">
        <button
          type="button"
          @click="handleCancel"
          :disabled="isSubmitting"
          class="rounded-md border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 shadow-sm transition-colors hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
        >
          Cancel
        </button>
        <button
          type="submit"
          :disabled="isSubmitting"
          class="inline-flex items-center gap-2 rounded-md bg-blue-600 px-4 py-2 text-sm font-semibold text-white shadow-sm transition-colors hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
        >
          <LoadingSpinner v-if="isSubmitting" size="sm" />
          {{ submitButtonText }}
        </button>
      </div>
    </form>
  </div>
</template>
