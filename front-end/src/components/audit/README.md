# Audit Components

This directory contains Vue components for audit trail viewing and data integrity verification.

## Components

### AuditTrail.vue

Displays a paginated, filterable table of audit entries that track all system actions and changes.

**Features:**
- Fetch and display audit entries from the back-end API
- Filter by entity type, user ID, and date range
- Pagination controls for large datasets
- Display audit entry details including user, action, timestamp, and entity information
- Show digital signatures and data hashes for each entry
- Loading states and error handling
- WCAG 2.0 AA compliant with proper ARIA attributes

**Usage:**
```vue
<script setup>
import { AuditTrail } from '@/components/audit'
</script>

<template>
  <AuditTrail />
</template>
```

**Props:**
None - component manages its own state

**API Integration:**
- Uses `auditService.getAuditTrail()` to fetch audit entries
- Supports filtering by:
  - `entityType`: Filter by entity type (Project, Transaction, etc.)
  - `userId`: Filter by user who performed the action
  - `startDate`: Filter entries from this date
  - `endDate`: Filter entries until this date
  - `pageNumber`: Current page number
  - `pageSize`: Number of items per page

**Accessibility:**
- Semantic HTML with proper table structure
- ARIA labels for filters and pagination
- Screen reader announcements for loading states
- Keyboard navigation support
- Focus management

---

### IntegrityVerification.vue

Provides a button to trigger data integrity verification and displays the results with clear indicators.

**Features:**
- Trigger cryptographic verification of all transactions and audit entries
- Display verification results with success/failure indicators
- Show statistics (total records checked, valid, invalid)
- List invalid records with details if any issues are detected
- Display verification timestamp
- Loading states and error handling
- WCAG 2.0 AA compliant with proper ARIA attributes

**Usage:**
```vue
<script setup>
import { IntegrityVerification } from '@/components/audit'
</script>

<template>
  <IntegrityVerification />
</template>
```

**Props:**
None - component manages its own state

**API Integration:**
- Uses `auditService.verifyIntegrity()` to perform verification
- Returns:
  - `isValid`: Boolean indicating if all records are valid
  - `totalRecordsChecked`: Total number of records verified
  - `invalidRecords`: Array of invalid records with details
  - `verifiedAt`: Timestamp of verification

**Accessibility:**
- Semantic HTML with proper heading structure
- ARIA labels for buttons and status indicators
- Screen reader announcements for verification results
- Color-coded indicators with text labels (not relying on color alone)
- Keyboard navigation support

---

## Implementation Notes

### Requirements Addressed

**Requirement 11.1, 11.2, 11.5 (Audit Trail):**
- Fetch audit entries from the back-end API
- Display audit entry details including user, action, timestamp, and entity information
- Allow filtering audit entries by entity type, date range, and user

**Requirement 11.3, 11.4 (Integrity Verification):**
- Call the integrity verification endpoint on the back-end API
- Display integrity verification results with clear indicators for valid and invalid records

**Requirement 6 (Loading States):**
- Display loading indicators during API operations
- Accessible loading states with ARIA live regions

**Requirement 5 (Error Handling):**
- Display error messages when API calls fail
- Provide retry functionality

### Design Patterns

Both components follow the established patterns in the project:

1. **Composition API**: Using Vue 3 Composition API with `<script setup>`
2. **Type Safety**: TypeScript interfaces for all data structures
3. **API Integration**: Direct use of `auditService` for API calls
4. **Error Handling**: Consistent error handling with `ErrorAlert` component
5. **Loading States**: Consistent loading indicators with `LoadingSpinner` component
6. **Styling**: Tailwind CSS utility classes for responsive design
7. **Accessibility**: WCAG 2.0 AA compliance with semantic HTML and ARIA attributes

### Styling

Components use Tailwind CSS with the following color schemes:

**AuditTrail:**
- Blue for primary actions and active states
- Gray for neutral elements
- Green/Blue/Red/Yellow badges for action types

**IntegrityVerification:**
- Green for success states (valid data)
- Red for error states (invalid data)
- Blue for primary actions
- Gray for neutral elements

### Future Enhancements

Potential improvements for future iterations:

1. **Export Functionality**: Add ability to export audit trail to CSV/PDF
2. **Real-time Updates**: WebSocket integration for live audit trail updates
3. **Advanced Filtering**: More filter options (action type, entity ID)
4. **Audit Entry Details**: Modal or expandable rows for full audit entry details
5. **Scheduled Verification**: Automatic periodic integrity verification
6. **Verification History**: Track and display past verification results
7. **Notifications**: Alert users when integrity issues are detected

## Testing

### Manual Testing Checklist

**AuditTrail:**
- [ ] Component loads and displays audit entries
- [ ] Filters work correctly (entity type, user, date range)
- [ ] Pagination works correctly
- [ ] Loading state displays during fetch
- [ ] Error state displays on API failure
- [ ] Retry button works after error
- [ ] Empty state displays when no entries found
- [ ] Table is responsive on mobile devices
- [ ] Keyboard navigation works
- [ ] Screen reader announces loading and errors

**IntegrityVerification:**
- [ ] Verify button triggers verification
- [ ] Loading state displays during verification
- [ ] Success state displays when all records valid
- [ ] Error state displays when issues detected
- [ ] Invalid records table displays correctly
- [ ] Statistics display correctly
- [ ] Timestamp displays correctly
- [ ] Retry button works after error
- [ ] Component is responsive on mobile devices
- [ ] Keyboard navigation works
- [ ] Screen reader announces verification results

### Integration Testing

Test with real back-end API:
1. Verify audit trail fetches real data
2. Verify filters work with back-end API
3. Verify pagination works with back-end API
4. Verify integrity verification returns real results
5. Test error scenarios (network failure, API errors)

## Related Files

- `front-end/src/services/api/auditService.ts` - API service for audit operations
- `front-end/src/types/api.ts` - TypeScript type definitions
- `front-end/src/components/common/LoadingSpinner.vue` - Loading indicator component
- `front-end/src/components/common/ErrorAlert.vue` - Error display component
