# Project Management Components

This directory contains Vue 3 components for managing projects in the Project Budget Management System.

## Components

### ProjectList.vue

Displays a paginated, filterable list of projects.

**Features:**
- Fetches projects using `useProjects` composable
- Displays projects in a responsive table layout
- Shows loading state while fetching data
- Displays error messages with retry functionality
- Implements pagination controls
- Provides status filter dropdown
- Styled with Tailwind CSS
- WCAG 2.0 AA compliant

**Usage:**
```vue
<script setup>
import ProjectList from '@/components/projects/ProjectList.vue'
</script>

<template>
  <ProjectList />
</template>
```

**Navigation:**
- Clicking "View Details" navigates to project details page
- Clicking "Create Project" navigates to project creation form

---

### ProjectDetails.vue

Displays detailed information about a single project.

**Props:**
- `projectId` (string, required): The ID of the project to display

**Features:**
- Fetches single project using `useProjects` composable
- Displays project information (name, description, dates, budget, coordinator, bank account)
- Shows loading state while fetching
- Displays error messages with retry functionality
- Provides edit button to navigate to edit form
- Displays project status with visual indicator
- Styled with Tailwind CSS
- WCAG 2.0 AA compliant

**Usage:**
```vue
<script setup>
import ProjectDetails from '@/components/projects/ProjectDetails.vue'

const projectId = 'abc-123'
</script>

<template>
  <ProjectDetails :project-id="projectId" />
</template>
```

---

### ProjectForm.vue

Form component for creating and editing projects.

**Props:**
- `projectId` (string, optional): The ID of the project to edit (required in edit mode)
- `mode` ('create' | 'edit', default: 'create'): Form mode

**Features:**
- Creates or updates projects using `useProjects` composable
- Implements client-side form validation matching back-end rules
- Shows loading state during submission
- Displays validation errors inline with form fields
- Handles API errors and displays error messages
- Redirects to project details on success
- Styled with Tailwind CSS
- WCAG 2.0 AA compliant

**Validation Rules:**
- Project name: Required, max 200 characters
- Start date: Required
- End date: Required, must be after start date
- Budget amount: Required, must be greater than 0
- Coordinator ID: Required
- Bank account fields: Optional, but if account number is provided, all other bank fields are required

**Usage:**

Create mode:
```vue
<script setup>
import ProjectForm from '@/components/projects/ProjectForm.vue'
</script>

<template>
  <ProjectForm mode="create" />
</template>
```

Edit mode:
```vue
<script setup>
import ProjectForm from '@/components/projects/ProjectForm.vue'

const projectId = 'abc-123'
</script>

<template>
  <ProjectForm mode="edit" :project-id="projectId" />
</template>
```

---

### ProjectStatusUpdate.vue

Component for updating project status with visual feedback.

**Props:**
- `projectId` (string, required): The ID of the project to update
- `currentStatus` (ProjectStatus, required): The current status of the project

**Events:**
- `statusUpdated`: Emitted when status is successfully updated, passes the new status

**Features:**
- Updates project status using `useProjects` composable
- Displays status options as radio buttons with descriptions
- Shows loading state during update
- Displays success message on status change
- Handles errors and displays error messages
- Styled with Tailwind CSS
- WCAG 2.0 AA compliant

**Status Options:**
- Not Started
- In Progress
- Completed
- Cancelled

**Usage:**
```vue
<script setup>
import { ref } from 'vue'
import ProjectStatusUpdate from '@/components/projects/ProjectStatusUpdate.vue'
import { ProjectStatus } from '@/types/api'

const projectId = 'abc-123'
const currentStatus = ref(ProjectStatus.InProgress)

const handleStatusUpdated = (newStatus) => {
  console.log('Status updated to:', newStatus)
  currentStatus.value = newStatus
}
</script>

<template>
  <ProjectStatusUpdate
    :project-id="projectId"
    :current-status="currentStatus"
    @status-updated="handleStatusUpdated"
  />
</template>
```

---

## Views

The following views are provided to use these components:

### ProjectListView.vue
- Route: `/projects`
- Displays the ProjectList component

### ProjectDetailsView.vue
- Route: `/projects/:id`
- Displays ProjectDetails and ProjectStatusUpdate components side by side

### ProjectCreateView.vue
- Route: `/projects/create`
- Displays ProjectForm in create mode

### ProjectEditView.vue
- Route: `/projects/:id/edit`
- Displays ProjectForm in edit mode

---

## Accessibility

All components follow WCAG 2.0 Level AA guidelines:

- Semantic HTML elements
- Proper ARIA attributes (role, aria-label, aria-describedby, aria-invalid, aria-live)
- Keyboard navigation support
- Focus management
- Screen reader announcements for loading states and errors
- Sufficient color contrast
- Clear error messages

---

## Dependencies

These components depend on:

- `@/composables/useProjects`: Composable for project operations
- `@/types/api`: TypeScript type definitions
- `@/components/common/LoadingSpinner`: Loading indicator component
- `@/components/common/ErrorAlert`: Error display component
- Vue Router: For navigation

---

## Styling

All components use Tailwind CSS utility classes for styling. The design follows a consistent pattern:

- Primary color: Blue (blue-600, blue-700)
- Success color: Green (green-600, green-700)
- Error color: Red (red-600, red-700)
- Neutral colors: Gray scale
- Rounded corners: rounded-md, rounded-lg
- Shadows: shadow, shadow-sm
- Transitions: transition-colors

---

## Testing

To test these components:

1. Ensure the back-end API is running
2. Navigate to `/projects` to see the project list
3. Click "Create Project" to test the creation form
4. Click "View Details" on a project to see details
5. Click "Edit Project" to test the edit form
6. Use the status update component to change project status

---

## Future Enhancements

Potential improvements:

- Add search functionality to ProjectList
- Add sorting options for the project table
- Implement bulk actions (delete, status update)
- Add project duplication feature
- Add export functionality (CSV, PDF)
- Implement real-time updates using WebSockets
- Add project archiving functionality
