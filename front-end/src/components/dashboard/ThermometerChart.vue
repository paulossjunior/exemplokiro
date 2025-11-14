<script setup lang="ts">
import { computed } from 'vue'
import type { DashboardMetrics } from '@/types/dashboard'
import { formatCurrency } from '@/utils/formatters'

interface Props {
  data: DashboardMetrics | null
}

const props = defineProps<Props>()

/**
 * Calculate consumption percentage
 */
const consumedPercentage = computed(() => {
  if (!props.data || props.data.total === 0) return 0
  return Math.round((props.data.consumed / props.data.total) * 100)
})

const consumedWidth = computed(() => `${consumedPercentage.value}%`)
</script>

<template>
  <div class="bg-zinc-800 rounded-lg p-6 border border-zinc-700">
    <!-- Metric Cards -->
    <div class="grid grid-cols-1 sm:grid-cols-3 gap-4 mb-6">
      <!-- Consumido -->
      <div class="bg-zinc-900 rounded-lg p-4 border border-zinc-700">
        <div class="text-sm text-cyan-400 mb-2">Consumido</div>
        <div class="text-2xl font-semibold text-white">
          {{ data ? formatCurrency(data.consumed) : 'R$ 0,00' }}
        </div>
      </div>

      <!-- Restante -->
      <div class="bg-zinc-900 rounded-lg p-4 border border-zinc-700">
        <div class="text-sm text-cyan-400 mb-2">Restante</div>
        <div class="text-2xl font-semibold text-white">
          {{ data ? formatCurrency(data.remaining) : 'R$ 0,00' }}
        </div>
      </div>

      <!-- Rendimento -->
      <div class="bg-zinc-900 rounded-lg p-4 border border-zinc-700">
        <div class="text-sm text-cyan-400 mb-2">Rendimento</div>
        <div class="text-2xl font-semibold text-white">
          {{ data ? formatCurrency(data.yield) : 'R$ 0,00' }}
        </div>
      </div>
    </div>

    <!-- Thermometer Bar -->
    <div>
      <div class="flex items-center justify-between mb-2">
        <span class="text-sm text-zinc-400">Progresso de Consumo</span>
        <span class="text-sm font-medium text-cyan-400">{{ consumedPercentage }}%</span>
      </div>
      <div class="h-3 bg-zinc-700 rounded-full overflow-hidden">
        <div 
          class="h-full bg-gradient-to-r from-cyan-500 to-cyan-400 transition-all duration-500 ease-out"
          :style="{ width: consumedWidth }"
          role="progressbar"
          :aria-valuenow="consumedPercentage"
          aria-valuemin="0"
          aria-valuemax="100"
          :aria-label="`${consumedPercentage}% do orçamento consumido`"
        />
      </div>
    </div>
  </div>
</template>
