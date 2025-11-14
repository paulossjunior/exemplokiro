---
inclusion: manual
---

# Vue 3 Development Standards

## Component Structure

Use Vue 3 Composition API with `<script setup>` syntax:

```vue
<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import type { Project } from '@/types/project'

// Props
interface Props {
  projectId: string
  readonly?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  readonly: false
})

// Emits
interface Emits {
  (e: 'update', value: Project): void
  (e: 'delete', id: string): void
}

const emit = defineEmits<Emits>()

// State
const project = ref<Project | null>(null)
const loading = ref(false)

// Computed
const isValid = computed(() => {
  return project.value !== null && project.value.name.length > 0
})

// Methods
const loadProject = async () => {
  loading.value = true
  try {
    // Load project logic
  } finally {
    loading.value = false
  }
}

// Lifecycle
onMounted(() => {
  loadProject()
})
</script>

<template>
  <div class="project-card">
    <h2 class="text-2xl font-bold">{{ project?.name }}</h2>
    <!-- Template content -->
  </div>
</template>

<style scoped>
/* Component-specific styles if needed */
/* Prefer Tailwind utility classes */
</style>
```

## Component Naming Conventions

- Use PascalCase for component names: `ProjectCard.vue`, `UserProfile.vue`
- Use multi-word names to avoid conflicts with HTML elements
- Prefix base components with `Base`: `BaseButton.vue`, `BaseInput.vue`
- Prefix single-instance components with `The`: `TheHeader.vue`, `TheSidebar.vue`

## Props and Events

### Props

```typescript
// Define props with TypeScript interface
interface Props {
  title: string
  count?: number
  items: string[]
  disabled?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  count: 0,
  disabled: false
})
```

### Events

```typescript
// Define emits with TypeScript
interface Emits {
  (e: 'update:modelValue', value: string): void
  (e: 'submit', data: FormData): void
  (e: 'cancel'): void
}

const emit = defineEmits<Emits>()

// Emit events
emit('update:modelValue', newValue)
emit('submit', formData)
```

## Composables

Create reusable logic with composables:

```typescript
// composables/useProjects.ts
import { ref, computed } from 'vue'
import type { Project } from '@/types/project'
import { projectService } from '@/services/projectService'

export function useProjects() {
  const projects = ref<Project[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  const activeProjects = computed(() => 
    projects.value.filter(p => p.status === 'InProgress')
  )

  const loadProjects = async () => {
    loading.value = true
    error.value = null
    try {
      projects.value = await projectService.getAll()
    } catch (e) {
      error.value = 'Failed to load projects'
      console.error(e)
    } finally {
      loading.value = false
    }
  }

  return {
    projects,
    loading,
    error,
    activeProjects,
    loadProjects
  }
}
```

## State Management with Pinia

```typescript
// stores/projectStore.ts
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { Project } from '@/types/project'

export const useProjectStore = defineStore('project', () => {
  // State
  const projects = ref<Project[]>([])
  const currentProject = ref<Project | null>(null)

  // Getters
  const activeProjects = computed(() => 
    projects.value.filter(p => p.status === 'InProgress')
  )

  // Actions
  const fetchProjects = async () => {
    // Fetch logic
  }

  const setCurrentProject = (project: Project) => {
    currentProject.value = project
  }

  return {
    projects,
    currentProject,
    activeProjects,
    fetchProjects,
    setCurrentProject
  }
})
```

## Best Practices

### Component Organization

1. **Single Responsibility**: Each component should do one thing well
2. **Small Components**: Keep components under 200 lines
3. **Reusability**: Extract common patterns into composables
4. **Props Down, Events Up**: Follow unidirectional data flow

### Performance

1. **Use `v-show` for frequent toggles**, `v-if` for conditional rendering
2. **Use `v-once` for static content**
3. **Lazy load routes and components**
4. **Use `computed` for derived state**
5. **Avoid inline functions in templates**

### Template Best Practices

```vue
<template>
  <!-- Good: Use semantic HTML -->
  <article class="project-card">
    <header>
      <h2>{{ project.name }}</h2>
    </header>
    <main>
      <p>{{ project.description }}</p>
    </main>
  </article>

  <!-- Good: Use v-for with :key -->
  <ul>
    <li v-for="item in items" :key="item.id">
      {{ item.name }}
    </li>
  </ul>

  <!-- Good: Use v-bind shorthand -->
  <BaseButton
    :disabled="loading"
    :aria-label="buttonLabel"
    @click="handleClick"
  >
    Submit
  </BaseButton>
</template>
```

### Avoid

- Don't mutate props directly
- Don't use `v-if` with `v-for` on the same element
- Don't access `$refs` in computed properties
- Don't use arrow functions for methods (breaks `this` context)
- Avoid deep component nesting (max 3-4 levels)
