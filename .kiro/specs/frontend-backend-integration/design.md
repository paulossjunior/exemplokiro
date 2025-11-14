# Design Document: Front-End and Back-End Integration

## 1. Overview

This design document describes the integration architecture between the Vue 3 front-end application and the .NET 8 back-end API for the Project Budget Management System. The integration follows industry best practices for RESTful API communication, type safety, error handling, and user experience.

### 1.1 Design Goals

- **Type Safety**: Ensure compile-time type checking for all API interactions
- **Separation of Concerns**: Isolate API communication logic from UI components
- **Error Resilience**: Handle network failures and API errors gracefully
- **Performance**: Minimize unnecessary API calls and optimize data transfer
- **Developer Experience**: Provide clear patterns and reusable code for API integration
- **User Experience**: Provide immediate feedback and smooth interactions

### 1.2 Technology Stack

**Front-End:**
- Vue 3 (Composition API)
- TypeScript
- Axios (HTTP client)
- Pinia (State management)
- Vite (Build tool)

**Back-End:**
- .NET 8 Web API
- ASP.NET Core CORS middleware
- JWT Authentication

## 2. Architecture

### 2.1 High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     Vue 3 Application                        │
│                                                              │
│  ┌────────────┐  ┌────────────┐  ┌────────────┐           │
│  │ Components │  │   Stores   │  │ Composables│           │
│  └──────┬─────┘  └──────┬─────┘  └──────┬─────┘           │
│         │               │               │                   │
│         └───────────────┴───────────────┘                   │
│                         │                                    │
│                  ┌──────▼──────┐                            │
│                  │ API Services│                            │
│                  └──────┬──────┘                            │
│                         │                                    │
│                  ┌──────▼──────┐                            │
│                  │ HTTP Client │                            │
│                  │   (Axios)   │                            │
│                  └──────┬──────┘                            │
└─────────────────────────┼────────────────────────────────────┘
                          │ HTTP/REST
                          │ JSON
┌─────────────────────────▼────────────────────────────────────┐
│                    .NET 8 Web API                            │
│                                                              │
│  ┌────────────┐  ┌────────────┐  ┌────────────┐           │
│  │Controllers │  │   CORS     │  │    JWT     │           │
│  │            │  │ Middleware │  │   Auth     │           │
│  └────────────┘  └────────────┘  └────────────┘           │
└─────────────────────────────────────────────────────────────┘
```

### 2.2 Layer Responsibilities

**Components Layer:**
- Render UI elements
- Handle user interactions
- Display data from stores
- Trigger API operations through services

**Stores Layer (Pinia):**
- Manage application state
- Cache API responses
- Coordinate multiple API calls
- Provide reactive data to components

**Composables Layer:**
- Reusable composition functions
- Shared logic for API interactions
- Loading state management
- Error handling utilities

**API Services Layer:**
- Encapsulate HTTP communication
- Provide typed methods for each endpoint
- Handle request/response transformation
- Abstract API details from components

**HTTP Client Layer:**
- Configure Axios instance
- Implement request/response interceptors
- Handle authentication tokens
- Manage base URL and headers

## 3. Components and Interfaces

### 3.1 HTTP Client Configuration

**File:** `src/services/api/httpClient.ts`

```typescript
import axios, { AxiosInstance, AxiosError, InternalAxiosRequestConfig } from 'axios';
import { useAuthStore } from '@/stores/authStore';
import router from '@/router';

// Create Axios instance
const httpClient: AxiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000',
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor - attach auth token
httpClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const authStore = useAuthStore();
    const token = authStore.token;
    
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    
    // Log requests in development
    if (import.meta.env.DEV) {
      console.log(`[API Request] ${config.method?.toUpperCase()} ${config.url}`, config.data);
    }
    
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor - handle errors
httpClient.interceptors.response.use(
  (response) => {
    // Log responses in development
    if (import.meta.env.DEV) {
      console.log(`[API Response] ${response.config.url}`, response.data);
    }
    return response;
  },
  (error: AxiosError) => {
    // Handle authentication errors
    if (error.response?.status === 401) {
      const authStore = useAuthStore();
      authStore.logout();
      router.push({ name: 'login', query: { redirect: router.currentRoute.value.fullPath } });
    }
    
    // Log errors in development
    if (import.meta.env.DEV) {
      console.error('[API Error]', error.response?.data || error.message);
    }
    
    return Promise.reject(error);
  }
);

export default httpClient;
```

### 3.2 TypeScript Type Definitions

**File:** `src/types/api.ts`

```typescript
// Common types
export interface ApiResponse<T> {
  data: T;
  message?: string;
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

export interface ApiError {
  message: string;
  errors?: Record<string, string[]>;
  statusCode: number;
}

// Project types
export interface Project {
  id: string;
  name: string;
  description: string;
  startDate: string;
  endDate: string;
  status: ProjectStatus;
  budgetAmount: number;
  coordinator: Person;
  bankAccount: BankAccount;
}

export enum ProjectStatus {
  NotStarted = 'NotStarted',
  InProgress = 'InProgress',
  Completed = 'Completed',
  Cancelled = 'Cancelled',
}

export interface Person {
  id: string;
  name: string;
  identificationNumber: string;
}

export interface BankAccount {
  accountNumber: string;
  bankName: string;
  branchNumber: string;
  accountHolderName: string;
}

export interface CreateProjectRequest {
  name: string;
  description: string;
  startDate: string;
  endDate: string;
  budgetAmount: number;
  coordinatorName: string;
  coordinatorIdentification: string;
  bankAccountNumber: string;
  bankName: string;
  branchNumber: string;
}

export interface UpdateProjectRequest {
  name?: string;
  description?: string;
  endDate?: string;
}

export interface UpdateProjectStatusRequest {
  status: ProjectStatus;
}

// Transaction types
export interface Transaction {
  id: string;
  amount: number;
  date: string;
  classification: TransactionClassification;
  accountingAccount: AccountingAccount;
  description?: string;
  digitalSignature: string;
  createdAt: string;
  createdBy: string;
}

export enum TransactionClassification {
  Debit = 'Debit',
  Credit = 'Credit',
}

export interface AccountingAccount {
  id: string;
  name: string;
  identifier: string;
}

export interface CreateTransactionRequest {
  amount: number;
  date: string;
  classification: TransactionClassification;
  accountingAccountId: string;
  description?: string;
}

export interface AccountBalance {
  projectId: string;
  budgetAmount: number;
  currentBalance: number;
  totalCredits: number;
  totalDebits: number;
  isOverBudget: boolean;
  calculatedAt: string;
}

// Audit types
export interface AuditEntry {
  id: string;
  userId: string;
  actionType: string;
  entityType: string;
  entityId: string;
  timestamp: string;
  previousValue: string | null;
  newValue: string;
  digitalSignature: string;
  dataHash: string;
}

export interface IntegrityVerificationResult {
  isValid: boolean;
  totalRecordsChecked: number;
  invalidRecords: InvalidRecord[];
  verifiedAt: string;
}

export interface InvalidRecord {
  entityType: string;
  entityId: string;
  issue: string;
}

// Report types
export interface GenerateReportRequest {
  includeAuditTrail: boolean;
  startDate?: string;
  endDate?: string;
}

export interface ReportResponse {
  reportId: string;
  projectId: string;
  generatedAt: string;
  downloadUrl: string;
}
```

### 3.3 API Service Classes

**File:** `src/services/api/projectService.ts`

```typescript
import httpClient from './httpClient';
import type {
  Project,
  CreateProjectRequest,
  UpdateProjectRequest,
  UpdateProjectStatusRequest,
  PaginatedResponse,
} from '@/types/api';

export class ProjectService {
  private readonly basePath = '/api/projects';

  async getProjects(params?: {
    status?: string;
    pageNumber?: number;
    pageSize?: number;
  }): Promise<PaginatedResponse<Project>> {
    const response = await httpClient.get<PaginatedResponse<Project>>(this.basePath, { params });
    return response.data;
  }

  async getProject(id: string): Promise<Project> {
    const response = await httpClient.get<Project>(`${this.basePath}/${id}`);
    return response.data;
  }

  async createProject(data: CreateProjectRequest): Promise<Project> {
    const response = await httpClient.post<Project>(this.basePath, data);
    return response.data;
  }

  async updateProject(id: string, data: UpdateProjectRequest): Promise<Project> {
    const response = await httpClient.put<Project>(`${this.basePath}/${id}`, data);
    return response.data;
  }

  async updateProjectStatus(id: string, data: UpdateProjectStatusRequest): Promise<Project> {
    const response = await httpClient.put<Project>(`${this.basePath}/${id}/status`, data);
    return response.data;
  }
}

export const projectService = new ProjectService();
```

**File:** `src/services/api/transactionService.ts`

```typescript
import httpClient from './httpClient';
import type {
  Transaction,
  CreateTransactionRequest,
  AccountBalance,
  PaginatedResponse,
} from '@/types/api';

export class TransactionService {
  async getTransactions(
    projectId: string,
    params?: {
      startDate?: string;
      endDate?: string;
      classification?: string;
      pageNumber?: number;
      pageSize?: number;
    }
  ): Promise<PaginatedResponse<Transaction>> {
    const response = await httpClient.get<PaginatedResponse<Transaction>>(
      `/api/projects/${projectId}/transactions`,
      { params }
    );
    return response.data;
  }

  async createTransaction(projectId: string, data: CreateTransactionRequest): Promise<Transaction> {
    const response = await httpClient.post<Transaction>(
      `/api/projects/${projectId}/transactions`,
      data
    );
    return response.data;
  }

  async getBalance(projectId: string): Promise<AccountBalance> {
    const response = await httpClient.get<AccountBalance>(`/api/projects/${projectId}/balance`);
    return response.data;
  }
}

export const transactionService = new TransactionService();
```

**File:** `src/services/api/accountingAccountService.ts`

```typescript
import httpClient from './httpClient';
import type { AccountingAccount } from '@/types/api';

export class AccountingAccountService {
  private readonly basePath = '/api/accounting-accounts';

  async getAccounts(): Promise<AccountingAccount[]> {
    const response = await httpClient.get<AccountingAccount[]>(this.basePath);
    return response.data;
  }

  async getAccount(id: string): Promise<AccountingAccount> {
    const response = await httpClient.get<AccountingAccount>(`${this.basePath}/${id}`);
    return response.data;
  }

  async createAccount(data: { name: string; identifier: string }): Promise<AccountingAccount> {
    const response = await httpClient.post<AccountingAccount>(this.basePath, data);
    return response.data;
  }
}

export const accountingAccountService = new AccountingAccountService();
```

**File:** `src/services/api/auditService.ts`

```typescript
import httpClient from './httpClient';
import type { AuditEntry, IntegrityVerificationResult, PaginatedResponse } from '@/types/api';

export class AuditService {
  private readonly basePath = '/api/audit';

  async getAuditTrail(params?: {
    entityType?: string;
    startDate?: string;
    endDate?: string;
    pageNumber?: number;
    pageSize?: number;
  }): Promise<PaginatedResponse<AuditEntry>> {
    const response = await httpClient.get<PaginatedResponse<AuditEntry>>(`${this.basePath}/trail`, {
      params,
    });
    return response.data;
  }

  async verifyIntegrity(): Promise<IntegrityVerificationResult> {
    const response = await httpClient.get<IntegrityVerificationResult>(
      `${this.basePath}/integrity`
    );
    return response.data;
  }
}

export const auditService = new AuditService();
```

**File:** `src/services/api/reportService.ts`

```typescript
import httpClient from './httpClient';
import type { GenerateReportRequest, ReportResponse } from '@/types/api';

export class ReportService {
  private readonly basePath = '/api/reports';

  async generateAccountabilityReport(
    projectId: string,
    data: GenerateReportRequest
  ): Promise<ReportResponse> {
    const response = await httpClient.post<ReportResponse>(
      `${this.basePath}/accountability/${projectId}`,
      data
    );
    return response.data;
  }

  async downloadReport(reportId: string): Promise<Blob> {
    const response = await httpClient.get(`${this.basePath}/${reportId}/download`, {
      responseType: 'blob',
    });
    return response.data;
  }
}

export const reportService = new ReportService();
```

### 3.4 Composables for API Integration

**File:** `src/composables/useApi.ts`

```typescript
import { ref, type Ref } from 'vue';
import { AxiosError } from 'axios';
import type { ApiError } from '@/types/api';

export interface UseApiOptions {
  immediate?: boolean;
  onSuccess?: (data: any) => void;
  onError?: (error: ApiError) => void;
}

export function useApi<T>(
  apiCall: () => Promise<T>,
  options: UseApiOptions = {}
) {
  const data: Ref<T | null> = ref(null);
  const error: Ref<ApiError | null> = ref(null);
  const loading = ref(false);

  const execute = async () => {
    loading.value = true;
    error.value = null;

    try {
      const result = await apiCall();
      data.value = result;
      
      if (options.onSuccess) {
        options.onSuccess(result);
      }
      
      return result;
    } catch (err) {
      const apiError = handleApiError(err);
      error.value = apiError;
      
      if (options.onError) {
        options.onError(apiError);
      }
      
      throw apiError;
    } finally {
      loading.value = false;
    }
  };

  if (options.immediate) {
    execute();
  }

  return {
    data,
    error,
    loading,
    execute,
  };
}

function handleApiError(err: unknown): ApiError {
  if (err instanceof AxiosError) {
    return {
      message: err.response?.data?.message || err.message || 'An error occurred',
      errors: err.response?.data?.errors,
      statusCode: err.response?.status || 500,
    };
  }
  
  return {
    message: 'An unexpected error occurred',
    statusCode: 500,
  };
}
```

**File:** `src/composables/useProjects.ts`

```typescript
import { ref, computed } from 'vue';
import { projectService } from '@/services/api/projectService';
import { useApi } from './useApi';
import type { Project, CreateProjectRequest, ProjectStatus } from '@/types/api';

export function useProjects() {
  const projects = ref<Project[]>([]);
  const currentProject = ref<Project | null>(null);

  const { loading: loadingProjects, execute: fetchProjects } = useApi(
    async () => {
      const response = await projectService.getProjects();
      projects.value = response.items;
      return response;
    }
  );

  const { loading: loadingProject, execute: fetchProject } = useApi(
    async (id: string) => {
      const project = await projectService.getProject(id);
      currentProject.value = project;
      return project;
    }
  );

  const { loading: creatingProject, execute: createProject } = useApi(
    async (data: CreateProjectRequest) => {
      const project = await projectService.createProject(data);
      projects.value.push(project);
      return project;
    }
  );

  const { loading: updatingStatus, execute: updateStatus } = useApi(
    async (id: string, status: ProjectStatus) => {
      const project = await projectService.updateProjectStatus(id, { status });
      const index = projects.value.findIndex((p) => p.id === id);
      if (index !== -1) {
        projects.value[index] = project;
      }
      if (currentProject.value?.id === id) {
        currentProject.value = project;
      }
      return project;
    }
  );

  const loading = computed(
    () => loadingProjects.value || loadingProject.value || creatingProject.value || updatingStatus.value
  );

  return {
    projects,
    currentProject,
    loading,
    fetchProjects,
    fetchProject,
    createProject,
    updateStatus,
  };
}
```

### 3.5 Pinia Store for State Management

**File:** `src/stores/authStore.ts`

```typescript
import { defineStore } from 'pinia';
import { ref, computed } from 'vue';

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem('auth_token'));
  const user = ref<{ id: string; name: string } | null>(null);

  const isAuthenticated = computed(() => !!token.value);

  function setToken(newToken: string) {
    token.value = newToken;
    localStorage.setItem('auth_token', newToken);
  }

  function setUser(userData: { id: string; name: string }) {
    user.value = userData;
  }

  function logout() {
    token.value = null;
    user.value = null;
    localStorage.removeItem('auth_token');
  }

  return {
    token,
    user,
    isAuthenticated,
    setToken,
    setUser,
    logout,
  };
});
```

## 4. Data Models

### 4.1 Request/Response Flow

```
Component
   │
   ├─> Call composable method (e.g., createProject)
   │
   ▼
Composable
   │
   ├─> Call API service method
   │
   ▼
API Service
   │
   ├─> Transform data to match API contract
   ├─> Call httpClient with typed request
   │
   ▼
HTTP Client (Axios)
   │
   ├─> Add auth token (request interceptor)
   ├─> Send HTTP request
   │
   ▼
Back-End API
   │
   ├─> Process request
   ├─> Return response
   │
   ▼
HTTP Client (Axios)
   │
   ├─> Handle errors (response interceptor)
   ├─> Return typed response
   │
   ▼
API Service
   │
   ├─> Extract data from response
   ├─> Return typed data
   │
   ▼
Composable
   │
   ├─> Update reactive state
   ├─> Call success/error callbacks
   │
   ▼
Component
   │
   └─> Update UI
```

### 4.2 Error Response Structure

```typescript
// Back-end validation error response
{
  "message": "Validation failed",
  "errors": {
    "Name": ["The Name field is required."],
    "BudgetAmount": ["Budget amount must be greater than 0."]
  }
}

// Front-end ApiError type
interface ApiError {
  message: string;
  errors?: Record<string, string[]>;
  statusCode: number;
}
```

## 5. Error Handling

### 5.1 Error Handling Strategy

**Levels of Error Handling:**

1. **HTTP Client Level** (Interceptors)
   - Handle 401 (redirect to login)
   - Log errors in development
   - Transform Axios errors to ApiError

2. **API Service Level**
   - Let errors propagate to composables
   - No error handling at this level

3. **Composable Level**
   - Catch errors from API services
   - Update error state
   - Call error callbacks
   - Provide error to components

4. **Component Level**
   - Display error messages to users
   - Handle specific error cases
   - Provide retry mechanisms

### 5.2 Error Display Component

**File:** `src/components/common/ErrorAlert.vue`

```vue
<template>
  <div v-if="error" class="error-alert" role="alert">
    <div class="error-header">
      <span class="error-icon">⚠️</span>
      <h3>{{ error.message }}</h3>
    </div>
    
    <div v-if="error.errors" class="error-details">
      <ul>
        <li v-for="(messages, field) in error.errors" :key="field">
          <strong>{{ field }}:</strong>
          <span v-for="(message, index) in messages" :key="index">
            {{ message }}
          </span>
        </li>
      </ul>
    </div>
    
    <button v-if="onRetry" @click="onRetry" class="retry-button">
      Retry
    </button>
  </div>
</template>

<script setup lang="ts">
import type { ApiError } from '@/types/api';

defineProps<{
  error: ApiError | null;
  onRetry?: () => void;
}>();
</script>
```

## 6. Testing Strategy

### 6.1 Unit Tests

**Test API Services:**
- Mock Axios responses
- Test request/response transformation
- Test error handling

**Test Composables:**
- Mock API services
- Test state management
- Test loading states
- Test error handling

**Test Components:**
- Mock composables
- Test user interactions
- Test error display
- Test loading states

### 6.2 Integration Tests

**Test API Integration:**
- Use real back-end API (test environment)
- Test complete request/response flow
- Test authentication
- Test error scenarios

### 6.3 E2E Tests

**Test User Workflows:**
- Create project end-to-end
- Record transaction end-to-end
- Generate report end-to-end
- Test error recovery

## 7. CORS Configuration

### 7.1 Back-End CORS Setup

**File:** `src/ProjectBudgetManagement.Api/Program.cs`

```csharp
// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontEnd", policy =>
    {
        policy.WithOrigins(
                builder.Configuration["FrontEndUrl"] ?? "http://localhost:5173"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Use CORS middleware
app.UseCors("AllowFrontEnd");
```

**File:** `src/ProjectBudgetManagement.Api/appsettings.json`

```json
{
  "FrontEndUrl": "http://localhost:5173",
  "ConnectionStrings": {
    "DefaultConnection": "..."
  }
}
```

### 7.2 Development Proxy (Vite)

**File:** `front-end/vite.config.ts`

```typescript
import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';

export default defineConfig({
  plugins: [vue()],
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true,
      },
    },
  },
});
```

## 8. Performance Optimization

### 8.1 Caching Strategy

**Implement cache in Pinia store:**

```typescript
import { defineStore } from 'pinia';
import { ref } from 'vue';
import { accountingAccountService } from '@/services/api/accountingAccountService';
import type { AccountingAccount } from '@/types/api';

export const useAccountingAccountStore = defineStore('accountingAccounts', () => {
  const accounts = ref<AccountingAccount[]>([]);
  const lastFetch = ref<number | null>(null);
  const CACHE_DURATION = 5 * 60 * 1000; // 5 minutes

  async function fetchAccounts(forceRefresh = false) {
    const now = Date.now();
    
    // Return cached data if still valid
    if (
      !forceRefresh &&
      accounts.value.length > 0 &&
      lastFetch.value &&
      now - lastFetch.value < CACHE_DURATION
    ) {
      return accounts.value;
    }

    // Fetch fresh data
    const data = await accountingAccountService.getAccounts();
    accounts.value = data;
    lastFetch.value = now;
    
    return data;
  }

  return {
    accounts,
    fetchAccounts,
  };
});
```

### 8.2 Debouncing Search

**File:** `src/composables/useDebounce.ts`

```typescript
import { ref, watch } from 'vue';

export function useDebounce<T>(value: Ref<T>, delay: number = 300) {
  const debouncedValue = ref<T>(value.value);
  let timeout: ReturnType<typeof setTimeout>;

  watch(value, (newValue) => {
    clearTimeout(timeout);
    timeout = setTimeout(() => {
      debouncedValue.value = newValue;
    }, delay);
  });

  return debouncedValue;
}
```

## 9. Environment Configuration

### 9.1 Environment Variables

**File:** `front-end/.env.development`

```
VITE_API_BASE_URL=http://localhost:5000
VITE_ENABLE_API_LOGGING=true
```

**File:** `front-end/.env.production`

```
VITE_API_BASE_URL=https://api.projectbudget.com
VITE_ENABLE_API_LOGGING=false
```

### 9.2 Docker Compose Integration

**File:** `docker-compose.yml`

```yaml
version: '3.8'

services:
  sqlserver:
    # ... existing configuration

  api:
    # ... existing configuration
    environment:
      - FrontEndUrl=http://localhost:5173

  frontend:
    build:
      context: ./front-end
      dockerfile: Dockerfile
    container_name: projectbudget-frontend
    environment:
      - VITE_API_BASE_URL=http://api:5000
    ports:
      - "5173:5173"
    depends_on:
      - api
    networks:
      - projectbudget-network
```

## 10. Security Considerations

### 10.1 Token Storage

- Store JWT token in localStorage for persistence
- Consider httpOnly cookies for enhanced security in production
- Implement token refresh mechanism

### 10.2 XSS Protection

- Vue 3 automatically escapes content
- Sanitize user input before sending to API
- Use Content Security Policy headers

### 10.3 CSRF Protection

- Use SameSite cookie attribute
- Implement CSRF tokens for state-changing operations
- Validate origin headers on back-end

## 11. Accessibility

### 11.1 Loading States

- Use ARIA live regions for loading announcements
- Provide keyboard-accessible loading indicators
- Ensure loading states don't trap focus

### 11.2 Error Messages

- Use role="alert" for error messages
- Ensure error messages are announced to screen readers
- Provide clear, actionable error messages

## 12. Monitoring and Logging

### 12.1 API Request Logging

- Log all API requests in development
- Log errors in production
- Include request ID for tracing

### 12.2 Performance Monitoring

- Track API response times
- Monitor failed requests
- Alert on high error rates

## 13. Migration Strategy

### 13.1 Phased Rollout

1. **Phase 1**: Set up HTTP client and basic API services
2. **Phase 2**: Implement project management integration
3. **Phase 3**: Implement transaction management integration
4. **Phase 4**: Implement reporting and audit trail integration
5. **Phase 5**: Performance optimization and caching

### 13.2 Backward Compatibility

- Maintain API versioning
- Support gradual migration of components
- Provide fallback mechanisms

## 14. Documentation

### 14.1 API Service Documentation

- Document all API service methods
- Provide usage examples
- Document error scenarios

### 14.2 Component Integration Guide

- Provide examples of using composables
- Document common patterns
- Provide troubleshooting guide
