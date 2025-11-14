<script setup lang="ts">
import { onMounted } from 'vue'
import { useDashboard } from '@/composables/useDashboard'
import { formatCurrency, formatDate, getStatusColorClass } from '@/services/mockApi'
import ThermometerChart from '@/components/ThermometerChart.vue'
import FilterBar from '@/components/FilterBar.vue'
import TransactionTable from '@/components/TransactionTable.vue'
import Pagination from '@/components/Pagination.vue'

const {
  budgetSummary,
  transactions,
  projects,
  categories,
  statuses,
  loading,
  selectedProjectId,
  searchQuery,
  selectedDate,
  selectedStatus,
  selectedCategory,
  currentPage,
  totalPages,
  totalResults,
  loadDashboardData,
  loadTransactions,
  applyFilters,
  previousPage,
  nextPage,
  goToPage
} = useDashboard()

onMounted(async () => {
  await loadDashboardData()
  await loadTransactions()
})
</script>

<template>
  <div class="min-h-screen bg-zinc-900 text-white">
    <!-- Header -->
    <header class="border-b border-zinc-800 px-6 py-4">
      <div class="flex items-center justify-between">
        <button class="p-2 hover:bg-zinc-800 rounded-lg transition-colors">
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
          </svg>
        </button>
        
        <div class="flex items-center gap-4">
          <button class="p-2 hover:bg-zinc-800 rounded-lg transition-colors" aria-label="Toggle theme">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20.354 15.354A9 9 0 018.646 3.646 9.003 9.003 0 0012 21a9.003 9.003 0 008.354-5.646z" />
            </svg>
          </button>
          <button class="p-2 hover:bg-zinc-800 rounded-lg transition-colors" aria-label="Documents">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
            </svg>
          </button>
          <button class="p-2 hover:bg-zinc-800 rounded-lg transition-colors" aria-label="User menu">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
            </svg>
          </button>
        </div>
      </div>
    </header>

    <!-- Main Content -->
    <main class="px-6 py-8 max-w-7xl mx-auto">
      <!-- Breadcrumb -->
      <nav class="flex items-center gap-2 text-sm text-zinc-400 mb-6" aria-label="Breadcrumb">
        <a href="#" class="hover:text-white transition-colors">
          <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
            <path d="M10.707 2.293a1 1 0 00-1.414 0l-7 7a1 1 0 001.414 1.414L4 10.414V17a1 1 0 001 1h2a1 1 0 001-1v-2a1 1 0 011-1h2a1 1 0 011 1v2a1 1 0 001 1h2a1 1 0 001-1v-6.586l.293.293a1 1 0 001.414-1.414l-7-7z" />
          </svg>
        </a>
        <span>/</span>
        <a href="#" class="hover:text-white transition-colors">Financeiro</a>
        <span>/</span>
        <span class="text-white">Prestação de Contas</span>
      </nav>

      <!-- Page Title -->
      <div class="mb-8">
        <div class="flex items-center gap-2 mb-2">
          <h1 class="text-2xl font-semibold">Prestação de Contas</h1>
          <button class="p-1 hover:bg-zinc-800 rounded transition-colors" aria-label="Information">
            <svg class="w-5 h-5 text-zinc-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </button>
        </div>
        <p class="text-sm text-zinc-400 leading-relaxed">
          Aqui você tem acesso ao Extrato Bancário de Rubricas da conta do seu projeto. Nele você associa a Nota Fiscal e a saída para prestar conta do gasto.
          Se você deseja fazer uma compra de um item não presente na sua solicitação primeiro faça a 
          <a href="#" class="text-cyan-400 hover:text-cyan-300 transition-colors">Solicitação de Remanejamento de Recursos</a>.
        </p>
      </div>

      <!-- Thermometer Chart Section -->
      <section class="mb-8">
        <h2 class="text-lg font-medium mb-3">Controle de Gastos</h2>
        <p class="text-sm text-zinc-400 mb-4">
          Acompanhe o valor total que você tem disponível para cada categoria, o valor que já gastou e o valor que ainda possui.
        </p>
        
        <ThermometerChart
          v-if="budgetSummary"
          :consumed="budgetSummary.consumed"
          :remaining="budgetSummary.remaining"
          :yield="budgetSummary.yield"
          :percentage="budgetSummary.consumedPercentage"
        />
      </section>

      <!-- Filters Section -->
      <section class="mb-6">
        <h3 class="text-sm font-medium text-zinc-400 mb-3">Selecione seu projeto</h3>
        
        <FilterBar
          v-model:project-id="selectedProjectId"
          v-model:search="searchQuery"
          v-model:date="selectedDate"
          v-model:status="selectedStatus"
          v-model:category="selectedCategory"
          :projects="projects"
          :categories="categories"
          :statuses="statuses"
          :loading="loading"
          @apply="applyFilters"
        />
      </section>

      <!-- Transactions Table -->
      <section>
        <TransactionTable
          :transactions="transactions"
          :loading="loading"
        />
        
        <!-- Pagination -->
        <Pagination
          :current-page="currentPage"
          :total-pages="totalPages"
          :total-results="totalResults"
          @previous="previousPage"
          @next="nextPage"
          @go-to-page="goToPage"
        />
      </section>
    </main>
  </div>
</template>
