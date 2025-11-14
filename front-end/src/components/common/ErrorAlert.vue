<script setup lang="ts">
import type { ApiError } from '@/types/api'

interface Props {
  error: ApiError | null
  onRetry?: () => void
}

defineProps<Props>()
</script>

<template>
  <div
    v-if="error"
    class="rounded-lg border border-red-300 bg-red-50 p-4 shadow-sm"
    role="alert"
    aria-live="assertive"
  >
    <!-- Error Header -->
    <div class="flex items-start gap-3">
      <!-- Error Icon -->
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

      <!-- Error Content -->
      <div class="flex-1">
        <!-- Main Error Message -->
        <h3 class="text-sm font-semibold text-red-800">
          {{ error.message }}
        </h3>

        <!-- Field-Specific Validation Errors -->
        <div v-if="error.errors && Object.keys(error.errors).length > 0" class="mt-3">
          <ul class="space-y-2 text-sm text-red-700">
            <li
              v-for="(messages, field) in error.errors"
              :key="field"
              class="flex flex-col gap-1"
            >
              <strong class="font-medium">{{ field }}:</strong>
              <ul class="ml-4 list-disc space-y-1">
                <li v-for="(message, index) in messages" :key="index">
                  {{ message }}
                </li>
              </ul>
            </li>
          </ul>
        </div>

        <!-- Status Code (for debugging in development) -->
        <p v-if="error.statusCode" class="mt-2 text-xs text-red-600">
          Error Code: {{ error.statusCode }}
        </p>
      </div>
    </div>

    <!-- Retry Button -->
    <div v-if="onRetry" class="mt-4 flex justify-end">
      <button
        type="button"
        @click="onRetry"
        class="inline-flex items-center gap-2 rounded-md bg-red-600 px-3 py-2 text-sm font-medium text-white shadow-sm transition-colors hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-red-500 focus:ring-offset-2"
        aria-label="Retry the failed operation"
      >
        <svg
          class="h-4 w-4"
          xmlns="http://www.w3.org/2000/svg"
          viewBox="0 0 20 20"
          fill="currentColor"
          aria-hidden="true"
        >
          <path
            fill-rule="evenodd"
            d="M15.312 11.424a5.5 5.5 0 01-9.201 2.466l-.312-.311h2.433a.75.75 0 000-1.5H3.989a.75.75 0 00-.75.75v4.242a.75.75 0 001.5 0v-2.43l.31.31a7 7 0 0011.712-3.138.75.75 0 00-1.449-.39zm1.23-3.723a.75.75 0 00.219-.53V2.929a.75.75 0 00-1.5 0V5.36l-.31-.31A7 7 0 003.239 8.188a.75.75 0 101.448.389A5.5 5.5 0 0113.89 6.11l.311.31h-2.432a.75.75 0 000 1.5h4.243a.75.75 0 00.53-.219z"
            clip-rule="evenodd"
          />
        </svg>
        Retry
      </button>
    </div>
  </div>
</template>
