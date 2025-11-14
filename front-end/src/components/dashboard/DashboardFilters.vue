<script setup lang="ts">
import { ref } from 'vue'
import type { Project, TransactionStatus, FilterCriteria } from '@/types/dashboard'

interface Props {
  projects: Project[]
  categories: string[]
}

interface Emits {
  (e: 'filter', filters: Partial<FilterCriteria>): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

// Local filter state
const selectedProjectId = ref<string>('')
const searchTerm = ref<string>('')
const selectedDate = ref<string>('')
const selectedStatus = ref<TransactionStatus | ''>('')
const selectedCategory = ref<string>('')

const statusOptions: TransactionStatus[] = ['Em Validação', 'Pendente', 'Validado', 'Revisar']

/**
 * Handles filter application
 */
const handleApplyFilters = () => {
  const filters: Partial<FilterCriteria> = {}
  
  if (selectedProjectId.value) filters.projectId = selectedProjectId.value
  if (searchTerm.value) filters.searchTerm = searchTerm.value
  if (selectedDate.value) filters.date = selectedDate.value
  if (selectedStatus.value) filters.status = selectedStatus.value as TransactionStatus
  if (selectedCategory.value) filters.category = selectedCategory.value
  
  emit('filter', filters)
}
</script>

<template>
  <div class="bg-zinc-800 rounded-lg p-6 border border-zinc-700">
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
      <!-- Project Select -->
      <div>
        <label for="project-select" class="block text-sm text-zinc-400 mb-2">
          Selecione seu projeto
        </label>
        <select
          id="project-select"
          v-model="selectedProjectId"
          class="w-full px-4 py-2.5 bg-zinc-800 border border-zinc-700 rounded-lg text-white text-sm focus:outline-none focus:ring-2 focus:ring-cyan-500 focus:border-transparent transition-all"
          aria-label="Selecione seu projeto"
        >
          <option value="">Todos os projetos</option>
          <option v-for="project in projects" :key="project.id" :value="project.id">
            {{ project.name }}
          </option>
        </select>
      </div>

      <!-- Search Input -->
      <div>
        <label for="search-input" class="block text-sm text-zinc-400 mb-2">
          Pesquisar
        </label>
        <div class="relative">
          <input
            id="search-input"
            v-model="searchTerm"
            type="text"
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
      </div>

      <!-- Date Picker -->
      <div>
        <label for="date-picker" class="block text-sm text-zinc-400 mb-2">
          Data
        </label>
        <input
          id="date-picker"
          v-model="selectedDate"
          type="date"
          class="w-full px-4 py-2.5 bg-zinc-800 border border-zinc-700 rounded-lg text-white text-sm focus:outline-none focus:ring-2 focus:ring-cyan-500 focus:border-transparent transition-all [&::-webkit-calendar-picker-indicator]:invert [&::-webkit-calendar-picker-indicator]:opacity-50"
          aria-label="Selecionar data"
        />
      </div>

      <!-- Status Select -->
      <div>
        <label for="status-select" class="block text-sm text-zinc-400 mb-2">
          Status
        </label>
        <select
          id="status-select"
          v-model="selectedStatus"
          class="w-full px-4 py-2.5 bg-zinc-800 border border-zinc-700 rounded-lg text-white text-sm focus:outline-none focus:ring-2 focus:ring-cyan-500 focus:border-transparent transition-all"
          aria-label="Selecione o status"
        >
          <option value="">Todos os status</option>
          <option v-for="status in statusOptions" :key="status" :value="status">
            {{ status }}
          </option>
        </select>
      </div>

      <!-- Category Select -->
      <div>
        <label for="category-select" class="block text-sm text-zinc-400 mb-2">
          Categoria
        </label>
        <select
          id="category-select"
          v-model="selectedCategory"
          class="w-full px-4 py-2.5 bg-zinc-800 border border-zinc-700 rounded-lg text-white text-sm focus:outline-none focus:ring-2 focus:ring-cyan-500 focus:border-transparent transition-all"
          aria-label="Selecione a categoria"
        >
          <option value="">Todas as categorias</option>
          <option v-for="category in categories" :key="category" :value="category">
            {{ category }}
          </option>
        </select>
      </div>

      <!-- Search Button -->
      <div class="flex items-end">
        <button
          @click="handleApplyFilters"
          class="w-full px-6 py-2.5 bg-cyan-500 hover:bg-cyan-600 text-white text-sm font-medium rounded-lg transition-colors focus:outline-none focus:ring-2 focus:ring-cyan-500 focus:ring-offset-2 focus:ring-offset-zinc-900"
          aria-label="Buscar transações"
        >
          Buscar
        </button>
      </div>
    </div>
  </div>
</template>
