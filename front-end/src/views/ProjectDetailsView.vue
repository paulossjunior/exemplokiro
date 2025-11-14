<script setup lang="ts">
import { useRoute } from 'vue-router'
import ProjectDetails from '@/components/projects/ProjectDetails.vue'
import ProjectStatusUpdate from '@/components/projects/ProjectStatusUpdate.vue'

const route = useRoute()
const projectId = route.params.id as string
</script>

<template>
  <div class="container mx-auto px-4 py-8">
    <div class="grid grid-cols-1 gap-8 lg:grid-cols-3">
      <!-- Main Content -->
      <div class="lg:col-span-2">
        <ProjectDetails :project-id="projectId" />
      </div>

      <!-- Sidebar -->
      <aside class="lg:col-span-1">
        <ProjectStatusUpdate
          v-if="projectId"
          :project-id="projectId"
          :current-status="$route.meta.currentStatus as any"
        />
      </aside>
    </div>
  </div>
</template>
