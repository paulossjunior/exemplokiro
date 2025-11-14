# Vue Composables

This directory contains reusable Vue 3 composables for the Project Budget Management System front-end.

## Available Composables

### useApi

Generic composable for managing API calls with loading, error, and data states.

**Features:**
- Automatic loading state management
- Error handling and transformation
- Success/error callbacks
- Support for parameterized API calls

**Usage:**
```typescript
import { useApi } from '@/composables';

const { data, error, loading, execute } = useApi(
  async (id: string) => {
    return await someApiCall(id);
  },
  {
    immediate: false,
    onSuccess: (data) => console.log('Success!', data),
    onError: (error) => console.error('Error:', error)
  }
);

// Execute the API call
await execute('some-id');
```

### useProjects

Composable for project management operations.

**Features:**
- Fetch projects with pagination and filtering
- Fetch single project details
- Create new projects
- Update existing projects
- Update project status
- Reactive state management
- Automatic state synchronization

**Usage:**
```typescript
import { useProjects } from '@/composables';

const { 
  projects, 
  currentProject, 
  loading, 
  error,
  fetchProjects, 
  fetchProject,
  createProject,
  updateProject,
  updateStatus
} = useProjects();

// Fetch all projects
await fetchProjects({ status: 'InProgress', pageNumber: 1, pageSize: 10 });

// Fetch single project
await fetchProject('project-id');

// Create project
await createProject({
  name: 'New Project',
  description: 'Project description',
  startDate: '2024-01-01',
  endDate: '2024-12-31',
  budgetAmount: 100000,
  coordinatorName: 'John Doe',
  coordinatorIdentification: '123456789',
  bankAccountNumber: '1234567890',
  bankName: 'Bank Name',
  branchNumber: '001'
});

// Update project status
await updateStatus('project-id', 'Completed');
```

### useTransactions

Composable for transaction management operations.

**Features:**
- Fetch transaction history with filtering
- Create new transactions
- Fetch account balance
- Reactive state management
- Separate loading states for each operation

**Usage:**
```typescript
import { useTransactions } from '@/composables';

const { 
  transactions, 
  balance,
  loadingTransactions,
  creatingTransaction,
  loadingBalance,
  transactionsError,
  createError,
  balanceError,
  fetchTransactions,
  createTransaction,
  fetchBalance
} = useTransactions();

// Fetch transactions with filters
await fetchTransactions('project-id', {
  startDate: '2024-01-01',
  endDate: '2024-12-31',
  classification: 'Debit'
});

// Create transaction
await createTransaction('project-id', {
  amount: 1000,
  date: '2024-11-14',
  classification: 'Debit',
  accountingAccountId: 'account-id',
  description: 'Office supplies'
});

// Get balance
await fetchBalance('project-id');
```

### useDebounce

Composable for debouncing reactive values.

**Features:**
- Configurable delay
- Automatic cleanup
- Type-safe
- Perfect for search inputs and form validation

**Usage:**
```typescript
import { ref, watch } from 'vue';
import { useDebounce } from '@/composables';

const searchQuery = ref('');
const debouncedQuery = useDebounce(searchQuery, 500);

// Watch the debounced value
watch(debouncedQuery, (newValue) => {
  // This will only fire 500ms after the user stops typing
  searchAPI(newValue);
});
```

## Best Practices

### Error Handling

All composables provide error states. Always handle errors in your components:

```typescript
const { error, execute } = useApi(apiCall);

try {
  await execute();
} catch (err) {
  // Error is already captured in error.value
  console.error('Operation failed:', error.value);
}
```

### Loading States

Use loading states to provide user feedback:

```vue
<template>
  <div>
    <LoadingSpinner v-if="loading" />
    <div v-else>
      <!-- Your content -->
    </div>
  </div>
</template>

<script setup lang="ts">
const { loading, fetchProjects } = useProjects();
</script>
```

### State Management

Composables maintain reactive state that automatically updates your UI:

```vue
<template>
  <ul>
    <li v-for="project in projects" :key="project.id">
      {{ project.name }}
    </li>
  </ul>
</template>

<script setup lang="ts">
import { onMounted } from 'vue';
import { useProjects } from '@/composables';

const { projects, fetchProjects } = useProjects();

onMounted(() => {
  fetchProjects();
});
</script>
```

### Combining Composables

Composables can be combined for complex workflows:

```typescript
import { useProjects, useTransactions } from '@/composables';

const { currentProject, fetchProject } = useProjects();
const { transactions, fetchTransactions } = useTransactions();

async function loadProjectData(projectId: string) {
  await fetchProject(projectId);
  await fetchTransactions(projectId);
}
```

## Testing

When testing components that use composables, you can mock them:

```typescript
import { vi } from 'vitest';
import { useProjects } from '@/composables';

vi.mock('@/composables', () => ({
  useProjects: vi.fn(() => ({
    projects: ref([]),
    loading: ref(false),
    fetchProjects: vi.fn()
  }))
}));
```

## Type Safety

All composables are fully typed with TypeScript. Import types from `@/types/api`:

```typescript
import type { Project, CreateProjectRequest } from '@/types/api';
import { useProjects } from '@/composables';

const { createProject } = useProjects();

const newProject: CreateProjectRequest = {
  // TypeScript will enforce the correct structure
};

await createProject(newProject);
```
