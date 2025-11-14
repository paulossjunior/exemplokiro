# Transaction Components

This directory contains Vue components for transaction management in the Project Budget Management System.

## Components

### TransactionList

Displays a list of transactions with filtering and pagination capabilities.

**Props:**
- `projectId` (string, required): The ID of the project to fetch transactions for

**Features:**
- Fetch and display transactions in a table format
- Filter by date range, classification, and accounting account
- Pagination controls for large datasets
- Display digital signatures and data hashes for each transaction
- Loading and error states
- WCAG 2.0 AA compliant

**Usage:**
```vue
<script setup>
import { TransactionList } from '@/components/transactions'
</script>

<template>
  <TransactionList project-id="project-123" />
</template>
```

### TransactionForm

Form component for creating new transactions.

**Props:**
- `projectId` (string, required): The ID of the project to create the transaction for

**Events:**
- `success`: Emitted when a transaction is successfully created, passes the created transaction
- `cancel`: Emitted when the user cancels the form

**Features:**
- Form validation matching back-end rules
- Accounting account dropdown selection
- Real-time validation feedback
- Loading state during submission
- Success message display
- Authentication requirement notice
- WCAG 2.0 AA compliant

**Usage:**
```vue
<script setup>
import { TransactionForm } from '@/components/transactions'

const handleSuccess = (transaction) => {
  console.log('Transaction created:', transaction)
}

const handleCancel = () => {
  console.log('Form cancelled')
}
</script>

<template>
  <TransactionForm 
    project-id="project-123"
    @success="handleSuccess"
    @cancel="handleCancel"
  />
</template>
```

### AccountBalance

Displays the current account balance with budget tracking.

**Props:**
- `projectId` (string, required): The ID of the project to fetch balance for
- `budgetAmount` (number, required): The project's budget amount
- `autoRefresh` (boolean, optional, default: true): Whether to auto-refresh when transactions change

**Features:**
- Display budget amount, current balance, total credits, and total debits
- Visual budget utilization progress bar
- Over budget warning indicator
- Near budget warning (80%+)
- Auto-refresh when transactions change
- Manual refresh button
- Calculation timestamp display
- WCAG 2.0 AA compliant

**Usage:**
```vue
<script setup>
import { AccountBalance } from '@/components/transactions'
import { ref } from 'vue'

const projectId = ref('project-123')
const budgetAmount = ref(50000)
</script>

<template>
  <AccountBalance 
    :project-id="projectId"
    :budget-amount="budgetAmount"
    :auto-refresh="true"
  />
</template>
```

## Integration Example

Here's a complete example showing how to use all transaction components together:

```vue
<script setup>
import { ref } from 'vue'
import { useRoute } from 'vue-router'
import { 
  TransactionList, 
  TransactionForm, 
  AccountBalance 
} from '@/components/transactions'

const route = useRoute()
const projectId = ref(route.params.id as string)
const budgetAmount = ref(50000) // This should come from project data
const showForm = ref(false)

const handleTransactionCreated = (transaction) => {
  console.log('New transaction created:', transaction)
  showForm.value = false
  // The TransactionList will auto-refresh via the composable
}

const handleFormCancel = () => {
  showForm.value = false
}
</script>

<template>
  <div class="container mx-auto px-4 py-8">
    <h1 class="text-3xl font-bold text-gray-900 mb-8">
      Transaction Management
    </h1>

    <!-- Account Balance -->
    <AccountBalance 
      :project-id="projectId"
      :budget-amount="budgetAmount"
      class="mb-8"
    />

    <!-- Create Transaction Button -->
    <div class="mb-6">
      <button
        @click="showForm = !showForm"
        class="inline-flex items-center gap-2 rounded-md bg-blue-600 px-4 py-2 text-sm font-semibold text-white shadow-sm hover:bg-blue-700"
      >
        {{ showForm ? 'Hide Form' : 'Create Transaction' }}
      </button>
    </div>

    <!-- Transaction Form -->
    <TransactionForm
      v-if="showForm"
      :project-id="projectId"
      @success="handleTransactionCreated"
      @cancel="handleFormCancel"
      class="mb-8"
    />

    <!-- Transaction List -->
    <TransactionList :project-id="projectId" />
  </div>
</template>
```

## Accessibility

All transaction components follow WCAG 2.0 Level AA guidelines:

- Semantic HTML elements
- ARIA labels and roles
- Keyboard navigation support
- Screen reader announcements for dynamic content
- Sufficient color contrast
- Focus indicators
- Error messages associated with form fields

## Styling

Components use Tailwind CSS utility classes and follow the project's design system:

- Consistent spacing and typography
- Responsive design (mobile-first)
- Hover and focus states
- Loading and error states
- Color-coded status indicators

## Dependencies

- Vue 3 Composition API
- `useTransactions` composable
- `useAccountingAccountStore` Pinia store
- Common components: `LoadingSpinner`, `ErrorAlert`
- Type definitions from `@/types/api`
