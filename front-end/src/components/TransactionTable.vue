<script setup lang="ts">
import type { Transaction } from '@/services/mockApi'
import { formatCurrency, formatDate, getStatusColorClass } from '@/services/mockApi'

interface Props {
  transactions: Transaction[]
  loading: boolean
}

const props = defineProps<Props>()

const getStatusLabel = (status: string): string => {
  const labels: Record<string, string> = {
    'em-validacao': 'Em Validação',
    'pendente': 'Pendente',
    'validado': 'Validado',
    'revisar': 'Revisar'
  }
  return labels[status] || status
}
</script>

<template>
  <div class="border border-zinc-800 rounded-lg overflow-hidden">
    <table class="w-full">
      <thead>
        <tr class="border-b border-zinc-800">
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
          class="border-b border-zinc-800 hover:bg-zinc-800/50 transition-colors cursor-pointer"
        >
          <td class="px-6 py-4 text-sm text-white">{{ transaction.paymentMethod }}</td>
          <td class="px-6 py-4 text-sm text-white">{{ formatCurrency(transaction.value) }}</td>
          <td class="px-6 py-4 text-sm text-zinc-400">{{ formatDate(transaction.date) }}</td>
          <td class="px-6 py-4 text-sm text-white">{{ transaction.cnpj }}</td>
          <td class="px-6 py-4">
            <span 
              :class="getStatusColorClass(transaction.status)"
              class="inline-flex items-center gap-1.5 px-3 py-1 rounded-full text-xs font-medium"
            >
              <span class="w-1.5 h-1.5 rounded-full bg-current" aria-hidden="true" />
              {{ getStatusLabel(transaction.status) }}
            </span>
          </td>
        </tr>
        
        <!-- Loading State -->
        <tr v-if="loading">
          <td colspan="5" class="px-6 py-12 text-center text-zinc-400">
            <div class="flex items-center justify-center gap-2">
              <svg class="animate-spin h-5 w-5" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" />
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z" />
              </svg>
              <span>Carregando...</span>
            </div>
          </td>
        </tr>
        
        <!-- Empty State -->
        <tr v-if="!loading && transactions.length === 0">
          <td colspan="5" class="px-6 py-12 text-center text-zinc-400">
            Nenhuma transação encontrada
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
