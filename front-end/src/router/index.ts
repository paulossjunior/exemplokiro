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
    path: '/projects',
    name: 'ProjectList',
    component: () => import('@/views/ProjectListView.vue'),
    meta: {
      title: 'Projects - Project Budget Management',
      requiresAuth: true
    }
  },
  {
    path: '/projects/create',
    name: 'ProjectCreate',
    component: () => import('@/views/ProjectCreateView.vue'),
    meta: {
      title: 'Create Project - Project Budget Management',
      requiresAuth: true
    }
  },
  {
    path: '/projects/:id',
    name: 'ProjectDetails',
    component: () => import('@/views/ProjectDetailsView.vue'),
    meta: {
      title: 'Project Details - Project Budget Management',
      requiresAuth: true
    }
  },
  {
    path: '/projects/:id/edit',
    name: 'ProjectEdit',
    component: () => import('@/views/ProjectEditView.vue'),
    meta: {
      title: 'Edit Project - Project Budget Management',
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
