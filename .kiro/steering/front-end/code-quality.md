---
inclusion: manual
---

# Front-End Code Quality Standards

## Clean Code Principles

### Component Design

1. **Single Responsibility**: Each component should do one thing well
2. **Small Components**: Keep components under 200 lines
3. **Reusability**: Extract common patterns into composables
4. **Props Down, Events Up**: Follow unidirectional data flow

### Naming Conventions

```typescript
// Components: PascalCase
ProjectCard.vue
UserProfile.vue
BaseButton.vue

// Composables: camelCase with 'use' prefix
useProjects.ts
useAuth.ts
useForm.ts

// Utilities: camelCase
formatCurrency.ts
validateEmail.ts

// Types: PascalCase
interface Project { }
type ProjectStatus = 'Active' | 'Completed'

// Constants: UPPER_SNAKE_CASE
const API_BASE_URL = 'http://localhost:5000'
const MAX_RETRIES = 3
```

### Code Organization

```vue
<script setup lang="ts">
// 1. Imports
import { ref, computed, onMounted } from 'vue'
import type { Project } from '@/types/project'

// 2. Props
interface Props {
  projectId: string
}
const props = defineProps<Props>()

// 3. Emits
interface Emits {
  (e: 'update', value: Project): void
}
const emit = defineEmits<Emits>()

// 4. Composables
const { projects, loadProjects } = useProjects()

// 5. State
const loading = ref(false)
const error = ref<string | null>(null)

// 6. Computed
const isValid = computed(() => projects.value.length > 0)

// 7. Methods
const handleSubmit = async () => {
  // Implementation
}

// 8. Lifecycle
onMounted(() => {
  loadProjects()
})
</script>

<template>
  <!-- Template -->
</template>

<style scoped>
/* Styles (prefer Tailwind) */
</style>
```

## Best Practices

### Template Best Practices

```vue
<template>
  <!-- Good: Use semantic HTML -->
  <article>
    <header>
      <h2>{{ title }}</h2>
    </header>
  </article>

  <!-- Good: Use v-for with :key -->
  <ul>
    <li v-for="item in items" :key="item.id">
      {{ item.name }}
    </li>
  </ul>

  <!-- Good: Use computed for complex logic -->
  <p>{{ formattedDate }}</p>

  <!-- Bad: Complex logic in template -->
  <!-- <p>{{ new Date(date).toLocaleDateString('en-US') }}</p> -->

  <!-- Good: Extract to method -->
  <button @click="handleClick">Click</button>

  <!-- Bad: Inline arrow function -->
  <!-- <button @click="() => doSomething()">Click</button> -->
</template>
```

### Performance Best Practices

```vue
<script setup lang="ts">
// Good: Use computed for derived state
const filteredProjects = computed(() =>
  projects.value.filter(p => p.status === 'Active')
)

// Bad: Filter in template
// <div v-for="project in projects.filter(p => p.status === 'Active')">

// Good: Use v-show for frequent toggles
const showDetails = ref(false)

// Good: Use v-once for static content
// <div v-once>{{ staticContent }}</div>

// Good: Lazy load components
const HeavyComponent = defineAsyncComponent(() =>
  import('./HeavyComponent.vue')
)
</script>
```

### Error Handling

```typescript
// Good: Comprehensive error handling
const loadData = async () => {
  loading.value = true
  error.value = null

  try {
    const data = await api.fetchProjects()
    projects.value = data
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'An error occurred'
    console.error('Failed to load projects:', e)
  } finally {
    loading.value = false
  }
}

// Good: User-friendly error messages
const getErrorMessage = (error: unknown): string => {
  if (error instanceof ApiError) {
    return error.message
  }
  if (error instanceof Error) {
    return error.message
  }
  return 'An unexpected error occurred'
}
```

## Anti-Patterns to Avoid

### Don't Mutate Props

```vue
<script setup lang="ts">
interface Props {
  modelValue: string
}
const props = defineProps<Props>()

// Bad: Mutating prop
// props.modelValue = 'new value'

// Good: Emit event
const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
}>()

const updateValue = (newValue: string) => {
  emit('update:modelValue', newValue)
}
</script>
```

### Don't Use v-if with v-for

```vue
<template>
  <!-- Bad: v-if with v-for -->
  <!-- <div v-for="item in items" v-if="item.active" :key="item.id"> -->

  <!-- Good: Filter first -->
  <div v-for="item in activeItems" :key="item.id">
    {{ item.name }}
  </div>
</template>

<script setup lang="ts">
const activeItems = computed(() =>
  items.value.filter(item => item.active)
)
</script>
```

### Avoid Deep Nesting

```vue
<!-- Bad: Deep nesting -->
<div>
  <div>
    <div>
      <div>
        <p>Too deep!</p>
      </div>
    </div>
  </div>
</div>

<!-- Good: Extract to components -->
<ProjectCard>
  <ProjectHeader />
  <ProjectContent />
</ProjectCard>
```

## Testing

### Component Testing

```typescript
import { mount } from '@vue/test-utils'
import { describe, it, expect } from 'vitest'
import ProjectCard from './ProjectCard.vue'

describe('ProjectCard', () => {
  it('renders project name', () => {
    const wrapper = mount(ProjectCard, {
      props: {
        project: {
          id: '1',
          name: 'Test Project',
          status: 'Active'
        }
      }
    })

    expect(wrapper.text()).toContain('Test Project')
  })

  it('emits update event on button click', async () => {
    const wrapper = mount(ProjectCard, {
      props: { project: mockProject }
    })

    await wrapper.find('button').trigger('click')

    expect(wrapper.emitted('update')).toBeTruthy()
  })
})
```

## Documentation

### Component Documentation

```vue
<script setup lang="ts">
/**
 * ProjectCard component displays project information in a card format.
 *
 * @example
 * <ProjectCard
 *   :project="project"
 *   @update="handleUpdate"
 * />
 */

/**
 * Props for ProjectCard component
 */
interface Props {
  /** The project to display */
  project: Project
  /** Whether the card is in readonly mode */
  readonly?: boolean
}

/**
 * Events emitted by ProjectCard
 */
interface Emits {
  /** Emitted when project is updated */
  (e: 'update', value: Project): void
  /** Emitted when project is deleted */
  (e: 'delete', id: string): void
}
</script>
```

## Code Review Checklist

- [ ] Component follows single responsibility principle
- [ ] Props and emits are properly typed
- [ ] No prop mutations
- [ ] Computed properties used for derived state
- [ ] No v-if with v-for on same element
- [ ] Semantic HTML used
- [ ] Accessibility attributes present
- [ ] Error handling implemented
- [ ] Loading states handled
- [ ] TypeScript types are strict (no `any`)
- [ ] Component is tested
- [ ] Documentation is complete
