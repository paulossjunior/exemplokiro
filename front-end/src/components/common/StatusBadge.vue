<script setup lang="ts">
import { computed } from 'vue'
import type { TransactionStatus } from '@/types/dashboard'

interface Props {
  status: TransactionStatus
}

const props = defineProps<Props>()

/**
 * Maps status to color classes
 */
const statusConfig = computed(() => {
  const configs: Record<TransactionStatus, { bg: string; text: string; label: string }> = {
    'Em Validação': {
      bg: 'bg-blue-500/20',
      text: 'text-blue-400',
      label: 'Status: Em Validação'
    },
    'Pendente': {
      bg: 'bg-red-500/20',
      text: 'text-red-400',
      label: 'Status: Pendente'
    },
    'Validado': {
      bg: 'bg-green-500/20',
      text: 'text-green-400',
      label: 'Status: Validado'
    },
    'Revisar': {
      bg: 'bg-orange-500/20',
      text: 'text-orange-400',
      label: 'Status: Revisar'
    }
  }
  
  return configs[props.status]
})
</script>

<template>
  <span
    :class="[statusConfig.bg, statusConfig.text]"
    class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-medium"
    role="status"
    :aria-label="statusConfig.label"
  >
    <span class="w-1.5 h-1.5 rounded-full bg-current"></span>
    {{ status }}
  </span>
</template>
