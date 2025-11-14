# Design Document - Expense Tracking Dashboard

## 1. Introduction

### 1.1 Purpose

This document describes the design for an Expense Tracking Dashboard that provides users with visual budget monitoring through a thermometer chart and comprehensive transaction filtering capabilities. The dashboard integrates with the existing Project Budget Management system to display real-time expense data.

### 1.2 Scope

The dashboard will be implemented as a Vue 3 component within the front-end application, utilizing the existing back-end API infrastructure. It focuses on read-only visualization and filtering of expense data without modifying transaction records.

### 1.3 Definitions and Acronyms

- **SPA**: Single Page Application
- **API**: Application Programming Interface
- **WCAG**: Web Content Accessibility Guidelines
- **ARIA**: Accessible Rich Internet Applications
- **UI**: User Interface
- **DTO**: Data Transfer Object

### 1.4 References

- Requirements Document: `.kiro/specs/expense-tracking-dashboard/requirements.md`
- Vue 3 Standards: `.kiro/steering/front-end/vue-standards.md`
- Tailwind Standards: `.kiro/steering/front-end/tailwind-standards.md`
- Accessibility Standards: `.kiro/steering/front-end/accessibility-wcag.md`
- API Design Standards: `.kiro/steering/back-end/api-design.md`

## 2. System Overview

The Expense Tracking Dashboard is a front-end feature that provides visual and tabular representations of project expenses. It consists of three main areas:

1. **Budget Visualization**: Horizontal thermometer chart showing consumed, remaining, and yield amounts
2. **Filter Controls**: Interactive inputs for project selection, search, date, status, and category filtering
3. **Transaction List**: Paginated table displaying filtered transaction records

The dashboard communicates with the back-end API to fetch project data, transaction records, and filter options.

## 3. Design Considerations

### 3.1 Assumptions and Dependencies

- The back-end API provides endpoints for fetching projects, transactions, and dashboard metrics
- Entity Framework Core repositories (IProjectRepository, ITransactionRepository) are already implemented
- The front-end uses Vue 3 Composition API with TypeScript
- Tailwind CSS is configured with zinc color palette for dark theme
- Users have appropriate permissions to view project expense data

### 3.2 General Constraints

- Must maintain < 100ms API response time for dashboard data
- Must support modern browsers (Chrome, Firefox, Safari, Edge)
- Must be fully responsive across mobile, tablet, and desktop devices
- Must comply with WCAG 2.0 Level AA accessibility standards
- All monetary values must use Brazilian Real (BRL) currency format

### 3.3 Goals and Guidelines

- **Performance**: Minimize API calls through efficient data fetching and caching
- **Usability**: Provide intuitive filtering with immediate visual feedback
- **Accessibility**: Ensure keyboard navigation and screen reader compatibility
- **Maintainability**: Use composables for reusable logic and clear component structure
- **Consistency**: Follow existing project patterns for API integration and styling

### 3.4 Development Methods

- Component-driven development with Vue 3 Composition API
- TypeScript for type safety and better developer experience
- Tailwind CSS utility-first approach for styling
- Composables for business logic separation
- Mock API for development and testing before back-end integration

## 4. Architectural Design

### 4.1 System Architecture

```
┌─────────────────────────────────────────────────────────┐
│                    Vue 3 Front-End                      │
├─────────────────────────────────────────────────────────┤
│  ┌───────────────────────────────────────────────────┐  │
│  │         ExpenseTrackingDashboard.vue              │  │
│  │  ┌─────────────────────────────────────────────┐  │  │
│  │  │  ThermometerChart.vue                       │  │  │
│  │  └─────────────────────────────────────────────┘  │  │
│  │  ┌─────────────────────────────────────────────┐  │  │
│  │  │  DashboardFilters.vue                       │  │  │
│  │  │  - ProjectSelect                            │  │  │
│  │  │  - SearchInput                              │  │  │
│  │  │  - DatePicker                               │  │  │
│  │  │  - StatusSelect                             │  │  │
│  │  │  - CategorySelect                           │  │  │
│  │  └─────────────────────────────────────────────┘  │  │
│  │  ┌─────────────────────────────────────────────┐  │  │
│  │  │  TransactionTable.vue                       │  │  │
│  │  │  - TransactionRow                           │  │  │
│  │  │  - StatusBadge                              │  │  │
│  │  │  - Pagination                               │  │  │
│  │  └─────────────────────────────────────────────┘  │  │
│  └───────────────────────────────────────────────────┘  │
│                          │                              │
│                          ▼                              │
│  ┌───────────────────────────────────────────────────┐  │
│  │      useDashboard.ts (Composable)                 │  │
│  │  - State management                               │  │
│  │  - Filter logic                                   │  │
│  │  - Pagination logic                               │  │
│  └───────────────────────────────────────────────────┘  │
│                          │                              │
│                          ▼                              │
│  ┌───────────────────────────────────────────────────┐  │
│  │      dashboardService.ts                          │  │
│  │  - API communication                              │  │
│  │  - Data transformation                            │  │
│  └───────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────┐
│                  .NET 8 Back-End API                    │
├─────────────────────────────────────────────────────────┤
│  ┌───────────────────────────────────────────────────┐  │
│  │      DashboardController                          │  │
│  │  - GET /api/dashboard/metrics/{projectId}        │  │
│  │  - GET /api/dashboard/transactions               │  │
│  │  - GET /api/projects                             │  │
│  └───────────────────────────────────────────────────┘  │
│                          │                              │
│                          ▼                              │
│  ┌───────────────────────────────────────────────────┐  │
│  │      DashboardService (Application Layer)         │  │
│  │  - Business logic                                 │  │
│  │  - Data aggregation                               │  │
│  └───────────────────────────────────────────────────┘  │
│                          │                              │
│                          ▼                              │
│  ┌───────────────────────────────────────────────────┐  │
│  │      Repositories (Infrastructure Layer)          │  │
│  │  - IProjectRepository                             │  │
│  │  - ITransactionRepository                         │  │
│  │  - IAccountingAccountRepository                   │  │
│  └───────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
```

### 4.2 Component Hierarchy

**Design Decision**: Break the dashboard into smaller, focused components for better maintainability and reusability.

**Rationale**: Following Vue 3 best practices and the single responsibility principle, each component handles a specific concern. This approach enables easier testing, better code organization, and component reuse across the application.



## 5. Detailed Design

### 5.1 Component Design

#### 5.1.1 ExpenseTrackingDashboard.vue (Container Component)

**Purpose**: Main container component that orchestrates the dashboard layout and manages overall state.

**Responsibilities**:
- Initialize dashboard data on mount
- Coordinate communication between child components
- Handle loading and error states
- Provide layout structure

**Props**: None (route-based)

**Emits**: None

**Composables Used**:
- `useDashboard()` - Main dashboard logic and state

**Template Structure**:
```vue
<template>
  <div class="min-h-screen bg-zinc-900 p-6">
    <div class="max-w-7xl mx-auto space-y-6">
      <ThermometerChart :data="dashboardMetrics" />
      <DashboardFilters 
        :projects="projects"
        :categories="categories"
        @filter="handleFilter"
      />
      <TransactionTable 
        :transactions="paginatedTransactions"
        :pagination="paginationData"
        @page-change="handlePageChange"
      />
    </div>
  </div>
</template>
```

**Design Decision**: Use a container/presentational component pattern.

**Rationale**: Separates business logic (container) from presentation (child components), making components more testable and reusable.

---

#### 5.1.2 ThermometerChart.vue

**Purpose**: Display budget consumption progress as a horizontal thermometer visualization.

**Props**:
```typescript
interface ThermometerChartProps {
  data: {
    consumed: number;
    remaining: number;
    yield: number;
    total: number;
  };
}
```

**Visual Design**:
- Horizontal bar with gradient fill representing consumption percentage
- Three metric cards displaying consumed, remaining, and yield amounts
- Responsive layout: stacked on mobile, horizontal on desktop
- Color scheme: cyan accent (text-cyan-400) on zinc background

**Implementation Approach**:
- Use CSS flexbox for layout
- Calculate percentage: `(consumed / total) * 100`
- Use Tailwind gradient utilities for thermometer fill
- Format currency using Brazilian Real format helper

**Design Decision**: Implement as a pure presentational component without internal state.

**Rationale**: Makes the component highly reusable and testable. All data comes from props, ensuring single source of truth.

---

#### 5.1.3 DashboardFilters.vue

**Purpose**: Provide filtering controls for transactions.

**Props**:
```typescript
interface DashboardFiltersProps {
  projects: Project[];
  categories: string[];
}
```

**Emits**:
```typescript
interface DashboardFiltersEmits {
  filter: (filters: FilterCriteria) => void;
}
```

**Filter Controls**:
1. **Project Select**: Dropdown with project list
2. **Search Input**: Text input with search icon
3. **Date Picker**: Calendar input for date selection
4. **Status Select**: Dropdown with predefined status options
5. **Category Select**: Dropdown with category list
6. **Search Button**: Cyan button to apply filters

**Layout**:
- Responsive grid: 1 column on mobile, 2-3 columns on tablet/desktop
- All inputs use zinc-800 background with zinc-700 borders
- Consistent spacing and alignment

**Design Decision**: Emit filter event only when "Buscar" button is clicked, not on every input change.

**Rationale**: Prevents excessive API calls and gives users control over when to apply filters. Improves performance and user experience.

---

#### 5.1.4 TransactionTable.vue

**Purpose**: Display paginated list of transactions with status indicators.

**Props**:
```typescript
interface TransactionTableProps {
  transactions: Transaction[];
  pagination: {
    currentPage: number;
    totalPages: number;
    totalResults: number;
    resultsPerPage: number;
  };
}
```

**Emits**:
```typescript
interface TransactionTableEmits {
  pageChange: (page: number) => void;
}
```

**Table Columns**:
1. Pagamento (Payment Method)
2. Valor (Amount)
3. Data (Date/Time)
4. CNPJ (Tax ID)
5. Status (Status Badge)

**Features**:
- Hover effect on rows (bg-zinc-800/50)
- Smooth transitions
- Horizontal scroll on mobile
- Sticky header on scroll
- Status badges with color coding

**Design Decision**: Implement pagination on the front-end with server-side data fetching.

**Rationale**: Reduces initial load time and network bandwidth. Server returns paginated results, and front-end manages page state.

---

#### 5.1.5 StatusBadge.vue

**Purpose**: Display transaction status with appropriate color and icon.

**Props**:
```typescript
interface StatusBadgeProps {
  status: 'Em Validação' | 'Pendente' | 'Validado' | 'Revisar';
}
```

**Status Mapping**:
- **Em Validação**: Blue background (bg-blue-500/20), blue text (text-blue-400), dot icon
- **Pendente**: Red background (bg-red-500/20), red text (text-red-400), dot icon
- **Validado**: Green background (bg-green-500/20), green text (text-green-400), dot icon
- **Revisar**: Orange background (bg-orange-500/20), orange text (text-orange-400), dot icon

**Design Decision**: Use semi-transparent backgrounds with solid text colors.

**Rationale**: Provides visual distinction while maintaining readability on dark backgrounds. Follows modern UI design patterns.

---

### 5.2 Composable Design

#### 5.2.1 useDashboard.ts

**Purpose**: Centralize dashboard state management and business logic.

**Exported State**:
```typescript
interface UseDashboardReturn {
  // State
  projects: Ref<Project[]>;
  selectedProject: Ref<Project | null>;
  dashboardMetrics: Ref<DashboardMetrics | null>;
  transactions: Ref<Transaction[]>;
  categories: Ref<string[]>;
  filters: Ref<FilterCriteria>;
  pagination: Ref<PaginationState>;
  loading: Ref<boolean>;
  error: Ref<string | null>;
  
  // Methods
  fetchProjects: () => Promise<void>;
  fetchDashboardMetrics: (projectId: string) => Promise<void>;
  fetchTransactions: (filters: FilterCriteria) => Promise<void>;
  applyFilters: (newFilters: FilterCriteria) => Promise<void>;
  changePage: (page: number) => Promise<void>;
  resetFilters: () => void;
}
```

**Key Responsibilities**:
1. Manage all dashboard state
2. Coordinate API calls through dashboardService
3. Handle filter application logic
4. Manage pagination state
5. Provide computed properties for derived data
6. Handle error states

**Design Decision**: Use a single composable for all dashboard logic rather than multiple smaller composables.

**Rationale**: Dashboard state is highly interconnected (filters affect transactions, project selection affects metrics). A single composable provides better cohesion and easier state management.

---

### 5.3 Service Layer Design

#### 5.3.1 dashboardService.ts

**Purpose**: Handle all API communication for dashboard data.

**API Methods**:
```typescript
interface DashboardService {
  getProjects(): Promise<Project[]>;
  getDashboardMetrics(projectId: string): Promise<DashboardMetrics>;
  getTransactions(params: TransactionQueryParams): Promise<PaginatedResponse<Transaction>>;
  getCategories(): Promise<string[]>;
}
```

**Design Decision**: Separate service layer from composable logic.

**Rationale**: Follows separation of concerns. Service layer handles HTTP communication and data transformation, while composables handle state and business logic. Makes testing easier and enables service reuse.

---

## 6. Data Design

### 6.1 Data Models

#### 6.1.1 Front-End TypeScript Interfaces

```typescript
interface Project {
  id: string;
  name: string;
  code: string;
  status: string;
}

interface DashboardMetrics {
  consumed: number;
  remaining: number;
  yield: number;
  total: number;
  projectId: string;
  projectName: string;
}

interface Transaction {
  id: string;
  paymentMethod: string;
  amount: number;
  date: string; // ISO 8601 format
  cnpj: string;
  companyName?: string;
  status: TransactionStatus;
  category: string;
  projectId: string;
}

type TransactionStatus = 'Em Validação' | 'Pendente' | 'Validado' | 'Revisar';

interface FilterCriteria {
  projectId?: string;
  searchTerm?: string;
  date?: string;
  status?: TransactionStatus;
  category?: string;
  page: number;
  pageSize: number;
}

interface PaginatedResponse<T> {
  data: T[];
  currentPage: number;
  totalPages: number;
  totalResults: number;
  pageSize: number;
}
```

#### 6.1.2 Back-End DTOs

```csharp
public class DashboardMetricsDto
{
    public decimal Consumed { get; set; }
    public decimal Remaining { get; set; }
    public decimal Yield { get; set; }
    public decimal Total { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; }
}

public class TransactionDto
{
    public Guid Id { get; set; }
    public string PaymentMethod { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Cnpj { get; set; }
    public string CompanyName { get; set; }
    public string Status { get; set; }
    public string Category { get; set; }
    public Guid ProjectId { get; set; }
}

public class TransactionQueryParams
{
    public Guid? ProjectId { get; set; }
    public string SearchTerm { get; set; }
    public DateTime? Date { get; set; }
    public string Status { get; set; }
    public string Category { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
```

### 6.2 Data Flow

1. **Initial Load**:
   - Fetch projects list → populate project dropdown
   - User selects project → fetch dashboard metrics and transactions
   - Display thermometer chart and transaction table

2. **Filter Application**:
   - User modifies filter inputs
   - User clicks "Buscar" button
   - Composable collects filter criteria
   - Service makes API call with query parameters
   - API returns filtered, paginated results
   - UI updates with new data

3. **Pagination**:
   - User clicks next/previous page
   - Composable updates page number
   - Service fetches new page with existing filters
   - UI updates transaction table

**Design Decision**: Use query parameters for filtering rather than POST body.

**Rationale**: Follows RESTful conventions for GET requests. Enables URL bookmarking and browser history navigation. Aligns with API design standards.

---

## 7. Interface Design

### 7.1 API Endpoints

#### 7.1.1 Get Projects
```
GET /api/projects
Response: 200 OK
[
  {
    "id": "uuid",
    "name": "Project Name",
    "code": "PRJ001",
    "status": "Active"
  }
]
```

#### 7.1.2 Get Dashboard Metrics
```
GET /api/dashboard/metrics/{projectId}
Response: 200 OK
{
  "consumed": 50000.00,
  "remaining": 30000.00,
  "yield": 2000.00,
  "total": 80000.00,
  "projectId": "uuid",
  "projectName": "Project Name"
}
```

#### 7.1.3 Get Transactions
```
GET /api/dashboard/transactions?projectId={id}&searchTerm={term}&date={date}&status={status}&category={category}&page={page}&pageSize={size}
Response: 200 OK
{
  "data": [
    {
      "id": "uuid",
      "paymentMethod": "Pix",
      "amount": 1500.00,
      "date": "2025-11-13T14:30:00Z",
      "cnpj": "12.345.678/0001-90",
      "companyName": "Company Name",
      "status": "Validado",
      "category": "Equipment",
      "projectId": "uuid"
    }
  ],
  "currentPage": 1,
  "totalPages": 5,
  "totalResults": 95,
  "pageSize": 20
}
```

#### 7.1.4 Get Categories
```
GET /api/dashboard/categories
Response: 200 OK
[
  "Equipment",
  "Services",
  "Materials",
  "Personnel"
]
```

### 7.2 User Interface Design

#### 7.2.1 Layout Structure

```
┌─────────────────────────────────────────────────────────┐
│  Expense Tracking Dashboard                             │
├─────────────────────────────────────────────────────────┤
│  ┌───────────────────────────────────────────────────┐  │
│  │  Thermometer Chart                                │  │
│  │  ┌─────────┐  ┌─────────┐  ┌─────────┐          │  │
│  │  │Consumido│  │Restante │  │Rendimento│          │  │
│  │  │R$ 50.000│  │R$ 30.000│  │R$ 2.000  │          │  │
│  │  └─────────┘  └─────────┘  └─────────┘          │  │
│  │  [████████████░░░░░░░░] 62.5%                    │  │
│  └───────────────────────────────────────────────────┘  │
│                                                          │
│  ┌───────────────────────────────────────────────────┐  │
│  │  Filters                                          │  │
│  │  ┌──────────┐ ┌──────────┐ ┌──────────┐         │  │
│  │  │ Project  │ │  Search  │ │   Date   │         │  │
│  │  └──────────┘ └──────────┘ └──────────┘         │  │
│  │  ┌──────────┐ ┌──────────┐ ┌──────────┐         │  │
│  │  │  Status  │ │ Category │ │  Buscar  │         │  │
│  │  └──────────┘ └──────────┘ └──────────┘         │  │
│  └───────────────────────────────────────────────────┘  │
│                                                          │
│  ┌───────────────────────────────────────────────────┐  │
│  │  Transactions                                     │  │
│  │  ┌─────────────────────────────────────────────┐ │  │
│  │  │Pagamento│Valor    │Data      │CNPJ  │Status│ │  │
│  │  ├─────────────────────────────────────────────┤ │  │
│  │  │Pix      │R$ 1.500 │13/11/2025│12... │✓     │ │  │
│  │  │Boleto   │R$ 2.300 │12/11/2025│34... │⚠     │ │  │
│  │  └─────────────────────────────────────────────┘ │  │
│  │  Exibindo 20 resultados de 95    [< 1 2 3 4 5 >]│  │
│  └───────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
```

#### 7.2.2 Color Scheme (Zinc Dark Theme)

- **Background**: `bg-zinc-900` (#18181b)
- **Cards/Inputs**: `bg-zinc-800` (#27272a)
- **Borders**: `border-zinc-700` (#3f3f46)
- **Primary Text**: `text-white` or `text-zinc-100`
- **Secondary Text**: `text-zinc-400` (#a1a1aa)
- **Accent**: `text-cyan-400` (#22d3ee) and `bg-cyan-500` (#06b6d4)

#### 7.2.3 Responsive Breakpoints

- **Mobile**: < 640px (sm) - Single column layout, stacked filters
- **Tablet**: 640px - 1024px (sm-lg) - Two column filters, scrollable table
- **Desktop**: > 1024px (lg+) - Three column filters, full table width

**Design Decision**: Mobile-first responsive design with Tailwind breakpoints.

**Rationale**: Ensures optimal experience on all devices. Mobile-first approach prioritizes performance and usability on smaller screens.

---

## 8. Accessibility Design

### 8.1 WCAG 2.0 Level AA Compliance

#### 8.1.1 Keyboard Navigation

- All interactive elements must be keyboard accessible
- Tab order follows logical reading flow
- Focus indicators visible on all focusable elements
- Escape key closes dropdowns and date picker
- Enter key submits filters

#### 8.1.2 ARIA Attributes

```html
<!-- Project Select -->
<select 
  aria-label="Selecione seu projeto"
  aria-required="true"
>

<!-- Search Input -->
<input 
  type="text"
  aria-label="Pesquisar transações"
  placeholder="Pesquisar"
>

<!-- Date Picker -->
<input 
  type="date"
  aria-label="Selecionar data"
>

<!-- Status Badge -->
<span 
  role="status"
  aria-label="Status: Validado"
  class="status-badge"
>

<!-- Pagination -->
<nav aria-label="Paginação de transações">
  <button aria-label="Página anterior">
  <button aria-label="Página 1" aria-current="page">
</nav>
```

#### 8.1.3 Screen Reader Announcements

- Announce filter application: "Filtros aplicados. Exibindo X resultados"
- Announce page changes: "Página X de Y carregada"
- Announce loading states: "Carregando dados..."
- Announce errors: "Erro ao carregar dados. Tente novamente"

#### 8.1.4 Color Contrast

All text must meet WCAG AA contrast ratios:
- Normal text: 4.5:1 minimum
- Large text: 3:1 minimum
- Status badges: Verified contrast between background and text

**Design Decision**: Use semantic HTML and ARIA attributes throughout.

**Rationale**: Ensures compatibility with assistive technologies. Improves usability for all users, not just those with disabilities.

---

## 9. Error Handling

### 9.1 Error Scenarios

1. **API Unavailable**: Display error message with retry button
2. **No Projects Found**: Show empty state with guidance
3. **No Transactions Found**: Display "Nenhuma transação encontrada" message
4. **Invalid Filter Criteria**: Show validation errors on filter inputs
5. **Network Timeout**: Display timeout message with retry option

### 9.2 Error Display Strategy

```typescript
interface ErrorState {
  type: 'network' | 'validation' | 'notFound' | 'server';
  message: string;
  retryable: boolean;
}
```

**Error UI Components**:
- Toast notifications for transient errors
- Inline validation messages for filter inputs
- Empty state components for no data scenarios
- Error boundary for unexpected errors

**Design Decision**: Use optimistic UI updates with rollback on error.

**Rationale**: Provides better perceived performance. Users see immediate feedback, and errors are handled gracefully without blocking the UI.

---

## 10. Performance Considerations

### 10.1 Optimization Strategies

1. **Lazy Loading**: Load transaction data only when needed
2. **Debouncing**: Debounce search input (300ms) to reduce API calls
3. **Caching**: Cache project list and categories (rarely change)
4. **Pagination**: Limit results to 20 per page
5. **Virtual Scrolling**: Consider for large transaction lists (future enhancement)

### 10.2 Performance Targets

- Initial page load: < 2 seconds
- Filter application: < 500ms
- Page navigation: < 300ms
- API response time: < 100ms (back-end requirement)

**Design Decision**: Implement client-side caching for static data (projects, categories).

**Rationale**: Reduces unnecessary API calls and improves responsiveness. Static data changes infrequently and can be cached for the session duration.

---

## 11. Testing Strategy

### 11.1 Unit Testing

**Components to Test**:
- `useDashboard.ts`: Filter logic, pagination, state management
- `dashboardService.ts`: API calls, data transformation
- `StatusBadge.vue`: Status mapping and rendering
- Utility functions: Currency formatting, date formatting

**Testing Approach**:
- Use Vitest for unit tests
- Mock API calls with MSW (Mock Service Worker)
- Test composable logic in isolation
- Verify computed properties and reactive state

### 11.2 Component Testing

**Components to Test**:
- `ThermometerChart.vue`: Rendering with different data values
- `DashboardFilters.vue`: Filter input handling and emission
- `TransactionTable.vue`: Pagination and row rendering
- `ExpenseTrackingDashboard.vue`: Integration of child components

**Testing Approach**:
- Use Vue Test Utils
- Test user interactions (clicks, input changes)
- Verify prop passing and event emission
- Test responsive behavior with viewport changes

### 11.3 Accessibility Testing

- Keyboard navigation flow
- Screen reader compatibility (with axe-core)
- Color contrast verification
- Focus management
- ARIA attribute correctness

### 11.4 Integration Testing

- End-to-end filter application workflow
- Project selection and data refresh
- Pagination navigation
- Error handling scenarios

**Design Decision**: Focus on testing business logic in composables rather than UI implementation details.

**Rationale**: Composables contain the core functionality and are easier to test in isolation. UI tests are more brittle and should focus on user interactions rather than implementation.

---

## 12. Requirements Traceability

| Requirement | Design Component | Implementation Location |
|-------------|------------------|-------------------------|
| Req 1: Thermometer Chart | ThermometerChart.vue | Section 5.1.2 |
| Req 2: Project Selection | DashboardFilters.vue (ProjectSelect) | Section 5.1.3 |
| Req 3: Search Input | DashboardFilters.vue (SearchInput) | Section 5.1.3 |
| Req 4: Date Picker | DashboardFilters.vue (DatePicker) | Section 5.1.3 |
| Req 5: Status Filter | DashboardFilters.vue (StatusSelect) | Section 5.1.3 |
| Req 6: Category Filter | DashboardFilters.vue (CategorySelect) | Section 5.1.3 |
| Req 7: Transaction List | TransactionTable.vue | Section 5.1.4 |
| Req 8: Status Indicators | StatusBadge.vue | Section 5.1.5 |
| Req 9: Hover Effects | TransactionTable.vue (row styling) | Section 5.1.4 |
| Req 10: Pagination | TransactionTable.vue (Pagination) | Section 5.1.4 |
| Req 11: Search Button | DashboardFilters.vue | Section 5.1.3 |
| Req 12: Accessibility | All components | Section 8 |
| Req 13: Responsive Design | All components | Section 7.2.3 |
| Req 14: Dark Theme | All components | Section 7.2.2 |

---

## 13. Future Enhancements

### 13.1 Potential Improvements

1. **Export Functionality**: Export filtered transactions to CSV/PDF
2. **Advanced Filters**: Date range selection, amount range filtering
3. **Sorting**: Sort transactions by column headers
4. **Transaction Details**: Modal or drawer for detailed transaction view
5. **Real-time Updates**: WebSocket integration for live transaction updates
6. **Saved Filters**: Save and load filter presets
7. **Dashboard Customization**: User-configurable metrics and charts
8. **Bulk Actions**: Select multiple transactions for batch operations

### 13.2 Scalability Considerations

- Virtual scrolling for large transaction lists (1000+ items)
- Server-side filtering and sorting for better performance
- Caching strategy with cache invalidation
- Progressive Web App (PWA) capabilities for offline access

---

## 14. Appendix

### 14.1 Currency Formatting

```typescript
function formatCurrency(value: number): string {
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL'
  }).format(value);
}
// Example: formatCurrency(1500.50) → "R$ 1.500,50"
```

### 14.2 Date Formatting

```typescript
function formatDateTime(isoDate: string): string {
  const date = new Date(isoDate);
  return new Intl.DateTimeFormat('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit'
  }).format(date);
}
// Example: formatDateTime('2025-11-13T14:30:00Z') → "13/11/2025 - 14:30:00"
```

### 14.3 Technology Stack Summary

**Front-End**:
- Vue 3 (Composition API)
- TypeScript
- Tailwind CSS
- Vite
- Vitest (testing)
- Vue Test Utils (component testing)

**Back-End**:
- .NET 8
- Entity Framework Core
- SQL Server
- Hexagonal Architecture

**Development Tools**:
- ESLint
- Prettier
- Mock Service Worker (API mocking)
