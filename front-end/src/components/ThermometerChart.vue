<script setup lang="ts">
import { computed } from 'vue'
import { formatCurrency } from '@/services/mockApi'

interface Props {
  consumed: number
  remaining: number
  yield: number
  percentage: number
}

const props = defineProps<Props>()

const consumedWidth = computed(() => `${props.percentage}%`)
</script>

<template>
  <div class="bg-zinc-800/50 rounded-lg p-6 border border-zinc-700">
    <!-- Thermometer Bar -->
    <div class="mb-6">
      <div class="h-2 bg-zinc-700 rounded-full overflow-hidden">
        <div 
          class="h-full bg-gradient-to-r from-cyan-500 to-cyan-400 transition-all duration-500"
          :style="{ width: consumedWidth }"
          role="progressbar"
          :aria-valuenow="percentage"
          aria-valuemin="0"
          aria-valuemax="100"
          :aria-label="`${percentage}% do orçamento consumido`"
        />
      </div>
    </div>

    <!-- Values -->
    <div class="flex items-center justify-center gap-8">
      <!-- Consumed -->
      <div class="flex items-center gap-3">
        <div class="flex items-center gap-2">
          <div class="w-2 h-2 rounded-full bg-cyan-400" aria-hidden="true" />
          <span class="text-sm text-zinc-400">Consumido</span>
        </div>
        <div class="px-4 py-2 bg-zinc-900 border border-cyan-500/30 rounded-lg">
          <span class="text-cyan-400 font-medium">{{ formatCurrency(consumed) }}</span>
        </div>
      </div>

      <!-- Remaining -->
      <div class="flex items-center gap-3">
        <div class="flex items-center gap-2">
          <div class="w-2 h-2 rounded-full bg-cyan-400" aria-hidden="true" />
          <span class="text-sm text-zinc-400">Restante</span>
        </div>
        <div class="px-4 py-2 bg-zinc-900 border border-cyan-500/30 rounded-lg">
          <span class="text-cyan-400 font-medium">{{ formatCurrency(remaining) }}</span>
        </div>
      </div>

      <!-- Yield -->
      <div class="flex items-center gap-3">
        <div class="flex items-center gap-2">
          <div class="w-2 h-2 rounded-full bg-zinc-400" aria-hidden="true" />
          <span class="text-sm text-zinc-400">Rendimento</span>
        </div>
        <div class="px-4 py-2 bg-zinc-900 border border-zinc-700 rounded-lg">
          <span class="text-zinc-300 font-medium">{{ formatCurrency(yield) }}</span>
        </div>
      </div>
    </div>
  </div>
</template>
