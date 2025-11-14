<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  currentPage: number
  totalPages: number
  totalResults: number
}

interface Emits {
  (e: 'previous'): void
  (e: 'next'): void
  (e: 'goToPage', page: number): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const canGoPrevious = computed(() => props.currentPage > 1)
const canGoNext = computed(() => props.currentPage < props.totalPages)

const visiblePages = computed(() => {
  const pages: number[] = []
  const maxVisible = 3
  
  let start = Math.max(1, props.currentPage - 1)
  let end = Math.min(props.totalPages, start + maxVisible - 1)
  
  if (end - start < maxVisible - 1) {
    start = Math.max(1, end - maxVisible + 1)
  }
  
  for (let i = start; i <= end; i++) {
    pages.push(i)
  }
  
  return pages
})
</script>

<template>
  <div class="flex items-center justify-between mt-6 px-6">
    <!-- Results Count -->
    <div class="text-sm text-zinc-400">
      Exibindo {{ totalResults }} resultados de {{ totalResults }}
    </div>
    
    <!-- Pagination Controls -->
    <div class="flex items-center gap-2">
      <!-- Previous Button -->
      <button
        @click="emit('previous')"
        :disabled="!canGoPrevious"
        class="p-2 rounded-lg hover:bg-zinc-800 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
        aria-label="Página anterior"
      >
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7" />
        </svg>
      </button>
      
      <!-- Page Numbers -->
      <button
        v-for="page in visiblePages"
        :key="page"
        @click="emit('goToPage', page)"
        :class="[
          'min-w-[2.5rem] px-3 py-2 rounded-lg text-sm font-medium transition-colors',
          page === currentPage
            ? 'bg-zinc-700 text-white'
            : 'text-zinc-400 hover:bg-zinc-800 hover:text-white'
        ]"
        :aria-label="`Página ${page}`"
        :aria-current="page === currentPage ? 'page' : undefined"
      >
        {{ page }}
      </button>
      
      <!-- Next Button -->
      <button
        @click="emit('next')"
        :disabled="!canGoNext"
        class="p-2 rounded-lg hover:bg-zinc-800 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
        aria-label="Próxima página"
      >
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
        </svg>
      </button>
    </div>
  </div>
</template>
