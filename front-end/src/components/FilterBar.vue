<script setup lang="ts">
import type { Project, Category, Status } from '@/services/mockApi'

interface Props {
  projectId: string
  search: string
  date: string
  status: string
  category: string
  projects: Project[]
  categories: Category[]
  statuses: Status[]
  loading: boolean
}

interface Emits {
  (e: 'update:projectId', value: string): void
  (e: 'update:search', value: string): void
  (e: 'update:date', value: string): void
  (e: 'update:status', value: string): void
  (e: 'update:category', value: string): void
  (e: 'apply'): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()
</script>

<template>
  <div class="flex items-end gap-3">
    <!-- Project Select -->
    <div class="flex-1">
      <select
        :value="projectId"
        @change="emit('update:projectId', ($event.target as HTMLSelectElement).value)"
        class="w-full px-4 py-2.5 bg-zinc-800 border border-zinc-700 rounded-lg text-white text-sm focus:outline-none focus:ring-2 focus:ring-cyan-500 focus:border-transparent transition-all"
        aria-label="Selecione o projeto"
      >
        <option value="">Conecta Fapes</option>
        <option v-for="project in projects" :key="project.id" :value="project.id">
          {{ project.name }}
        </option>
      </select>
    </div>

    <!-- Search Input -->
    <div class="flex-1 relative">
      <input
        type="text"
        :value="search"
        @input="emit('update:search', ($event.target as HTMLInputElement).value)"
        placeholder="Pesquisar"
        class="w-full px-4 py-2.5 pl-10 bg-zinc-800 border border-zinc-700 rounded-lg text-white text-sm placeholder-zinc-500 focus:outline-none focus:ring-2 focus:ring-cyan-500 focus:border-transparent transition-all"
        aria-label="Pesquisar transações"
      />
      <svg 
        class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-zinc-500" 
        fill="none" 
        stroke="currentColor" 
        viewBox="0 0 24 24"
        aria-hidden="true"
      >
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
      </svg>
    </div>

    <!-- Date Picker -->
    <div class="flex-1 relative">
      <input
        type="date"
        :value="date"
        @change="emit('update:date', ($event.target as HTMLInputElement).value)"
        class="w-full px-4 py-2.5 bg-zinc-800 border border-zinc-700 rounded-lg text-white text-sm focus:outline-none focus:ring-2 focus:ring-cyan-500 focus:border-transparent transition-all [&::-webkit-calendar-picker-indicator]:invert [&::-webkit-calendar-picker-indicator]:opacity-50"
        aria-label="Selecione a data"
      />
    </div>

    <!-- Status Select -->
    <div class="flex-1">
      <select
        :value="status"
        @change="emit('update:status', ($event.target as HTMLSelectElement).value)"
        class="w-full px-4 py-2.5 bg-zinc-800 border border-zinc-700 rounded-lg text-white text-sm focus:outline-none focus:ring-2 focus:ring-cyan-500 focus:border-transparent transition-all"
        aria-label="Selecione o status"
      >
        <option value="">Status</option>
        <option v-for="s in statuses" :key="s.id" :value="s.id">
          {{ s.name }}
        </option>
      </select>
    </div>

    <!-- Category Select -->
    <div class="flex-1">
      <select
        :value="category"
        @change="emit('update:category', ($event.target as HTMLSelectElement).value)"
        class="w-full px-4 py-2.5 bg-zinc-800 border border-zinc-700 rounded-lg text-white text-sm focus:outline-none focus:ring-2 focus:ring-cyan-500 focus:border-transparent transition-all"
        aria-label="Selecione a categoria"
      >
        <option value="">Categoria</option>
        <option v-for="cat in categories" :key="cat.id" :value="cat.name">
          {{ cat.name }}
        </option>
      </select>
    </div>

    <!-- Search Button -->
    <button
      @click="emit('apply')"
      :disabled="loading"
      class="px-6 py-2.5 bg-cyan-500 hover:bg-cyan-600 disabled:bg-cyan-500/50 text-white text-sm font-medium rounded-lg transition-colors focus:outline-none focus:ring-2 focus:ring-cyan-500 focus:ring-offset-2 focus:ring-offset-zinc-900"
      aria-label="Buscar transações"
    >
      Buscar
    </button>
  </div>
</template>
