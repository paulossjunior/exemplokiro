<script setup lang="ts">
import { computed } from 'vue'
import type { Transaction, PaginationState } from '@/types/dashboard'
import { formatCurrency, formatDateTime } from '@/utils/formatters'
import StatusBadge from '@/components/common/StatusBadge.vue'

interface Props {
  transactions: Transaction[]
  pagination: PaginationState
}

interface Emits {
  (e: 'pageChange', page: number): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const canGoPrevious = computed(() => props.pagination.currentPage > 1)
const canGoNext = computed(() => props.pagination.currentPage < props.pagination.totalPages)

const visiblePages = computed(() => {
  const pages: number[] = []
  const maxVisible = 5
  const current = props.pagination.currentPage
  const total = props.pagination.totalPages
  
  let start = Math.max(1, current - Math.floor(maxVisible / 2))
  let end = Math.min(total, start + maxVisible - 1)
  
  if (end - start < maxVisible - 1) {
    start = Math.max(1, end - maxVisible + 1)
  }
  
  for (let i = start; i <= end; i++) {
    pages.push(i)
  }
  
  return pages
})

const handlePrevious = () => {
  if (canGoPrevious.value) {
    emit('pageChange', props.pagination.currentPage - 1)
  }
}

const handleNext = () => {
  if (canGoNext.value) {
    emit('pageChange', props.pagination.currentPage + 1)
  }
}

const handleGoToPage = (page: number) => {
  emit('pageChange', page)
}
</script>

<template>
  <div class="bg-zinc-800 rounded-lg border border-zinc-700 overflow-hidden">
    <!-- Table -->
    <div class="overflow-x-auto">
      <table class="w-full">
        <thead class="bg-zinc-900 sticky top-0">
          <tr>
            <th class="px-6 py-4 text-left text-sm font-medium text-zinc-400">Pagamento</th>
            <th class="px-6 py-4 text-left text-sm font-medium text-zinc-400">Valor</th>
            <th class="px-6 py-4 text-left text-sm font-medium text-zinc-400">Data</th>
            <th class="px-6 py-4 text-left text-sm font-medium text-zinc-400">CNPJ</th>
            <th class="px-6 py-4 text-left text-sm font-medium text-zinc-400">Status</th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="transaction in transactions"
            :key="transaction.id"
            class="border-t border-zinc-700 hover:bg-zinc-800/50 transition-colors duration-200"
          >
            <td class="px-6 py-4 text-sm text-white">
              {{ transaction.paymentMethod }}
            </td>
            <td class="px-6 py-4 text-sm text-white font-medium">
              {{ formatCurrency(transaction.amount) }}
            </td>
            <td class="px-6 py-4 text-sm text-zinc-400">
              {{ formatDateTime(transaction.date) }}
            </td>
            <td class="px-6 py-4 text-sm text-white">
              {{ transaction.companyName || transaction.cnpj }}
            </td>
            <td class="px-6 py-4">
              <StatusBadge :status="transaction.status" />
            </td>
          </tr>
          
          <!-- Empty State -->
          <tr v-if="transactions.length === 0">
            <td colspan="5" class="px-6 py-12 text-center text-zinc-400">
              Nenhuma transação encontrada
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Pagination -->
    <div class="flex items-center justify-between px-6 py-4 border-t border-zinc-700 bg-zinc-900">
      <!-- Results Count -->
      <div class="text-sm text-zinc-400">
        Exibindo {{ transactions.length }} resultados de {{ pagination.totalResults }}
      </div>
      
      <!-- Pagination Controls -->
      <nav aria-label="Paginação de transações">
        <div class="flex items-center gap-2">
          <!-- Previous Button -->
          <button
            @click="handlePrevious"
            :disabled="!canGoPrevious"
            class="p-2 rounded-lg text-zinc-400 hover:bg-zinc-800 hover:text-white disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
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
            @click="handleGoToPage(page)"
            :class="[
              'min-w-[2.5rem] px-3 py-2 rounded-lg text-sm font-medium transition-colors',
              page === pagination.currentPage
                ? 'bg-cyan-500 text-white'
                : 'text-zinc-400 hover:bg-zinc-800 hover:text-white'
            ]"
            :aria-label="`Página ${page}`"
            :aria-current="page === pagination.currentPage ? 'page' : undefined"
          >
            {{ page }}
          </button>
          
          <!-- Next Button -->
          <button
            @click="handleNext"
            :disabled="!canGoNext"
            class="p-2 rounded-lg text-zinc-400 hover:bg-zinc-800 hover:text-white disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
            aria-label="Próxima página"
          >
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
            </svg>
          </button>
        </div>
      </nav>
    </div>
  </div>
</template>
