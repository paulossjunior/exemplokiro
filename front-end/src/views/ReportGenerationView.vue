<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ReportGenerator } from '@/components/reports'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'

const route = useRoute()
const router = useRouter()

const projectId = ref<string>('')
const loading = ref(true)

onMounted(() => {
  // Get project ID from route params
  const id = route.params.projectId as string
  
  if (!id) {
    // Redirect to project list if no project ID provided
    router.push({ name: 'projects' })
    return
  }
  
  projectId.value = id
  loading.value = false
})

function goBack() {
  router.back()
}
</script>

<template>
  <div class="min-h-screen bg-gray-50 py-8">
    <div class="mx-auto max-w-4xl px-4 sm:px-6 lg:px-8">
      <!-- Back Button -->
      <div class="mb-6">
        <button
          type="button"
          @click="goBack"
          class="inline-flex items-center gap-2 text-sm font-medium text-gray-600 transition-colors hover:text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
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
          Back to Project
        </button>
      </div>

      <!-- Loading State -->
      <div
        v-if="loading"
        class="flex items-center justify-center py-12"
      >
        <LoadingSpinner size="lg" message="Loading report generator..." />
      </div>

      <!-- Report Generator -->
      <ReportGenerator v-else :project-id="projectId" />
    </div>
  </div>
</template>
