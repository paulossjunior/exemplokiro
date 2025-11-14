import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'

const routes: RouteRecordRaw[] = [
  {
    path: '/dashboard/expenses',
    name: 'ExpenseTrackingDashboard',
    component: () => import('@/views/ExpenseTrackingDashboard.vue'),
    meta: {
      title: 'Prestação de Contas - Dashboard',
      requiresAuth: true
    }
  },
  {
    path: '/',
    redirect: '/dashboard/expenses'
  }
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes
})

// Navigation guard for page titles
router.beforeEach((to, from, next) => {
  if (to.meta.title) {
    document.title = to.meta.title as string
  }
  next()
})

export default router
