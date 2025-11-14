# Reports Components

This directory contains Vue components for report generation and management functionality.

## Components

### ReportGenerator.vue

A comprehensive component for generating and downloading accountability reports.

**Features:**
- Form for report parameters (date range, audit trail inclusion)
- Real-time form validation
- Loading states during report generation
- Error handling with retry functionality
- Report summary display
- PDF download functionality
- WCAG 2.0 Level AA compliant
- Responsive design with Tailwind CSS

**Props:**
- `projectId` (string, required): The ID of the project to generate report for

**Usage:**
```vue
<script setup>
import ReportGenerator from '@/components/reports/ReportGenerator.vue'
</script>

<template>
  <ReportGenerator :project-id="projectId" />
</template>
```

**Accessibility Features:**
- Semantic HTML structure
- ARIA labels and descriptions
- Keyboard navigation support
- Screen reader announcements for loading states
- Error alerts with role="alert"
- Form validation with aria-invalid and aria-describedby

**API Integration:**
- Uses `reportService.generateAccountabilityReport()` for report generation
- Uses `reportService.downloadReport()` for PDF download
- Handles blob responses for file downloads
- Comprehensive error handling

## Report Generation Flow

1. User fills in report parameters (optional date range, audit trail inclusion)
2. Form validates input (date range validation)
3. User submits form
4. Loading state displayed with progress indicator
5. Report generated via API call
6. Success message displayed with report summary
7. User can download PDF or generate new report

## Error Handling

- Network errors
- Validation errors
- Report generation failures
- Download failures
- Retry functionality for all error scenarios

## Styling

All components use Tailwind CSS utility classes following the project's design system:
- Consistent color scheme (blue for primary actions, green for success, red for errors)
- Responsive grid layouts
- Proper spacing and typography
- Accessible focus states
- Hover and active states for interactive elements
