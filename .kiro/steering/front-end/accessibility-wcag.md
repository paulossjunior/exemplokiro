---
inclusion: manual
---

# WCAG 2.0 Accessibility Standards

## Level AA Compliance

All front-end code must meet WCAG 2.0 Level AA standards.

## Semantic HTML

Use proper HTML5 semantic elements:

```vue
<template>
  <!-- Good: Semantic structure -->
  <article class="project-card">
    <header>
      <h2>{{ project.name }}</h2>
    </header>
    <main>
      <p>{{ project.description }}</p>
    </main>
    <footer>
      <time :datetime="project.createdAt">
        {{ formatDate(project.createdAt) }}
      </time>
    </footer>
  </article>

  <!-- Good: Semantic navigation -->
  <nav aria-label="Main navigation">
    <ul>
      <li><a href="/">Home</a></li>
      <li><a href="/projects">Projects</a></li>
    </ul>
  </nav>

  <!-- Good: Semantic forms -->
  <form @submit.prevent="handleSubmit">
    <fieldset>
      <legend>Project Information</legend>
      <!-- Form fields -->
    </fieldset>
  </form>
</template>
```

## ARIA Attributes

Use ARIA attributes to enhance accessibility:

```vue
<template>
  <!-- Labels and descriptions -->
  <button
    aria-label="Close dialog"
    aria-describedby="close-description"
  >
    <span aria-hidden="true">&times;</span>
  </button>
  <span id="close-description" class="sr-only">
    Closes the dialog and returns to the previous page
  </span>

  <!-- Live regions for dynamic content -->
  <div
    role="alert"
    aria-live="assertive"
    aria-atomic="true"
  >
    {{ errorMessage }}
  </div>

  <!-- Loading states -->
  <button :disabled="loading" :aria-busy="loading">
    <span v-if="loading" role="status">Loading...</span>
    <span v-else>Submit</span>
  </button>

  <!-- Expanded/collapsed states -->
  <button
    :aria-expanded="isOpen"
    aria-controls="menu-content"
    @click="toggleMenu"
  >
    Menu
  </button>
  <div id="menu-content" :hidden="!isOpen">
    <!-- Menu content -->
  </div>
</template>
```

## Keyboard Navigation

Ensure all interactive elements are keyboard accessible:

```vue
<template>
  <!-- Keyboard accessible custom button -->
  <div
    role="button"
    tabindex="0"
    @click="handleClick"
    @keydown.enter="handleClick"
    @keydown.space.prevent="handleClick"
  >
    Custom Button
  </div>

  <!-- Skip to main content link -->
  <a href="#main-content" class="sr-only focus:not-sr-only focus:absolute focus:top-0 focus:left-0 focus:z-50 focus:p-4 focus:bg-white">
    Skip to main content
  </a>

  <main id="main-content">
    <!-- Main content -->
  </main>

  <!-- Focus management in modals -->
  <dialog
    ref="dialogRef"
    @keydown.esc="closeDialog"
    aria-labelledby="dialog-title"
    aria-describedby="dialog-description"
  >
    <h2 id="dialog-title">Dialog Title</h2>
    <p id="dialog-description">Dialog content</p>
    <button @click="closeDialog">Close</button>
  </dialog>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'

const dialogRef = ref<HTMLDialogElement>()
let previousFocus: HTMLElement | null = null

const openDialog = () => {
  previousFocus = document.activeElement as HTMLElement
  dialogRef.value?.showModal()
  // Focus first focusable element
  const firstFocusable = dialogRef.value?.querySelector('button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])')
  ;(firstFocusable as HTMLElement)?.focus()
}

const closeDialog = () => {
  dialogRef.value?.close()
  previousFocus?.focus()
}
</script>
```

## Color Contrast

Ensure sufficient color contrast (WCAG AA: 4.5:1 for normal text, 3:1 for large text):

```vue
<template>
  <!-- Good: High contrast -->
  <p class="text-gray-900 bg-white">
    Normal text with 21:1 contrast ratio
  </p>

  <!-- Good: Large text with sufficient contrast -->
  <h1 class="text-2xl text-gray-700 bg-white">
    Large heading with 4.5:1 contrast ratio
  </h1>

  <!-- Bad: Insufficient contrast -->
  <!-- <p class="text-gray-400 bg-white">Low contrast text</p> -->

  <!-- Good: Error states with sufficient contrast -->
  <p class="text-red-700 bg-red-50 border border-red-200">
    Error message with good contrast
  </p>
</template>
```

## Form Accessibility

Make forms fully accessible:

```vue
<template>
  <form @submit.prevent="handleSubmit">
    <!-- Proper label association -->
    <div class="mb-4">
      <label for="project-name" class="block text-sm font-medium text-gray-700 mb-2">
        Project Name
        <span class="text-red-600" aria-label="required">*</span>
      </label>
      <input
        id="project-name"
        v-model="projectName"
        type="text"
        required
        aria-required="true"
        aria-invalid="!!errors.name"
        aria-describedby="name-error name-hint"
        class="w-full px-3 py-2 border rounded-md"
        :class="errors.name ? 'border-red-500' : 'border-gray-300'"
      />
      <p id="name-hint" class="mt-1 text-sm text-gray-500">
        Enter a descriptive name for your project
      </p>
      <p
        v-if="errors.name"
        id="name-error"
        class="mt-1 text-sm text-red-600"
        role="alert"
      >
        {{ errors.name }}
      </p>
    </div>

    <!-- Fieldset for related inputs -->
    <fieldset class="mb-4">
      <legend class="text-sm font-medium text-gray-700 mb-2">
        Project Dates
      </legend>
      <div class="grid grid-cols-2 gap-4">
        <div>
          <label for="start-date" class="block text-sm text-gray-600 mb-1">
            Start Date
          </label>
          <input
            id="start-date"
            v-model="startDate"
            type="date"
            required
            aria-required="true"
            class="w-full px-3 py-2 border border-gray-300 rounded-md"
          />
        </div>
        <div>
          <label for="end-date" class="block text-sm text-gray-600 mb-1">
            End Date
          </label>
          <input
            id="end-date"
            v-model="endDate"
            type="date"
            required
            aria-required="true"
            class="w-full px-3 py-2 border border-gray-300 rounded-md"
          />
        </div>
      </div>
    </fieldset>

    <!-- Submit button with loading state -->
    <button
      type="submit"
      :disabled="loading"
      :aria-busy="loading"
      class="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50"
    >
      <span v-if="loading" role="status">Saving...</span>
      <span v-else>Save Project</span>
    </button>
  </form>
</template>
```

## Screen Reader Support

Provide screen reader-only content when needed:

```vue
<template>
  <!-- Screen reader only text -->
  <span class="sr-only">
    Current page: Projects
  </span>

  <!-- Icon with accessible label -->
  <button aria-label="Delete project">
    <svg aria-hidden="true" class="w-5 h-5">
      <use href="#trash-icon" />
    </svg>
  </button>

  <!-- Data table with proper headers -->
  <table>
    <caption class="sr-only">
      List of projects with their status and budget
    </caption>
    <thead>
      <tr>
        <th scope="col">Project Name</th>
        <th scope="col">Status</th>
        <th scope="col">Budget</th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="project in projects" :key="project.id">
        <th scope="row">{{ project.name }}</th>
        <td>{{ project.status }}</td>
        <td>{{ formatCurrency(project.budget) }}</td>
      </tr>
    </tbody>
  </table>
</template>

<style scoped>
.sr-only {
  position: absolute;
  width: 1px;
  height: 1px;
  padding: 0;
  margin: -1px;
  overflow: hidden;
  clip: rect(0, 0, 0, 0);
  white-space: nowrap;
  border-width: 0;
}

.sr-only:focus {
  position: static;
  width: auto;
  height: auto;
  padding: inherit;
  margin: inherit;
  overflow: visible;
  clip: auto;
  white-space: normal;
}
</style>
```

## Focus Management

Provide visible focus indicators:

```css
/* Global focus styles */
*:focus {
  outline: 2px solid #3b82f6;
  outline-offset: 2px;
}

/* Custom focus styles with Tailwind */
.focus-visible:focus {
  @apply ring-2 ring-blue-500 ring-offset-2;
}
```

## Testing Accessibility

### Manual Testing

1. **Keyboard Navigation**: Tab through all interactive elements
2. **Screen Reader**: Test with NVDA (Windows) or VoiceOver (Mac)
3. **Color Contrast**: Use browser DevTools or online checkers
4. **Zoom**: Test at 200% zoom level
5. **Focus Indicators**: Verify all focusable elements have visible focus

### Automated Testing

```typescript
// Use axe-core for automated accessibility testing
import { mount } from '@vue/test-utils'
import { axe, toHaveNoViolations } from 'jest-axe'

expect.extend(toHaveNoViolations)

test('component should have no accessibility violations', async () => {
  const wrapper = mount(ProjectCard, {
    props: { project: mockProject }
  })

  const results = await axe(wrapper.element)
  expect(results).toHaveNoViolations()
})
```

## Accessibility Checklist

- [ ] All images have alt text
- [ ] All form inputs have associated labels
- [ ] Color is not the only means of conveying information
- [ ] Sufficient color contrast (4.5:1 for normal text)
- [ ] All interactive elements are keyboard accessible
- [ ] Focus indicators are visible
- [ ] ARIA attributes are used correctly
- [ ] Semantic HTML is used throughout
- [ ] Skip navigation link is provided
- [ ] Error messages are announced to screen readers
- [ ] Loading states are communicated
- [ ] Modal dialogs trap focus
- [ ] Page titles are descriptive and unique
