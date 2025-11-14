<script setup lang="ts">
import { useRoute } from 'vue-router'
import ProjectForm from '@/components/projects/ProjectForm.vue'

const route = useRoute()
const projectId = route.params.id as string
</script>

<template>
  <div class="container mx-auto px-4 py-8">
    <ProjectForm mode="edit" :project-id="projectId" />
  </div>
</template>
