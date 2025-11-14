---
inclusion: manual
---

# TypeScript Standards

## Type Definitions

### Interface vs Type

Use **interfaces** for object shapes that may be extended:

```typescript
// Good: Use interface for objects
interface Project {
  id: string
  name: string
  status: ProjectStatus
  budget: number
}

// Extend interfaces
interface DetailedProject extends Project {
  description: string
  coordinator: Person
}
```

Use **type** for unions, intersections, and primitives:

```typescript
// Good: Use type for unions
type ProjectStatus = 'NotStarted' | 'Initiated' | 'InProgress' | 'Completed' | 'Cancelled'

// Good: Use type for intersections
type ProjectWithMeta = Project & {
  createdAt: Date
  updatedAt: Date
}
```

## Type Safety

### Strict Mode

Always use strict TypeScript configuration:

```json
{
  "compilerOptions": {
    "strict": true,
    "noImplicitAny": true,
    "strictNullChecks": true,
    "strictFunctionTypes": true,
    "noUnusedLocals": true,
    "noUnusedParameters": true
  }
}
```

### Avoid `any`

```typescript
// Bad
const data: any = fetchData()

// Good: Use proper types
interface ApiResponse {
  data: Project[]
  meta: {
    total: number
    page: number
  }
}

const response: ApiResponse = await fetchData()
```

### Use Type Guards

```typescript
// Type guard function
function isProject(obj: unknown): obj is Project {
  return (
    typeof obj === 'object' &&
    obj !== null &&
    'id' in obj &&
    'name' in obj
  )
}

// Usage
if (isProject(data)) {
  console.log(data.name) // TypeScript knows data is Project
}
```

## API Types

Define types for API requests and responses:

```typescript
// types/api.ts
export interface ApiError {
  code: string
  message: string
  details?: Array<{
    field: string
    issue: string
  }>
  timestamp: string
  traceId: string
}

export interface ApiResponse<T> {
  data: T
  meta?: {
    timestamp: string
    version: string
  }
}

export interface PaginatedResponse<T> {
  data: T[]
  meta: {
    total: number
    page: number
    pageSize: number
  }
}

// types/project.ts
export interface CreateProjectRequest {
  name: string
  description?: string
  startDate: string
  endDate: string
  budgetAmount: number
  coordinatorId: string
}

export interface ProjectResponse {
  id: string
  name: string
  description?: string
  startDate: string
  endDate: string
  status: ProjectStatus
  budgetAmount: number
  coordinator: PersonResponse
  createdAt: string
  updatedAt: string
}
```

## Generic Types

Use generics for reusable type-safe code:

```typescript
// Generic API service
class ApiService {
  async get<T>(url: string): Promise<ApiResponse<T>> {
    const response = await fetch(url)
    return response.json()
  }

  async post<TRequest, TResponse>(
    url: string,
    data: TRequest
  ): Promise<ApiResponse<TResponse>> {
    const response = await fetch(url, {
      method: 'POST',
      body: JSON.stringify(data)
    })
    return response.json()
  }
}

// Usage
const api = new ApiService()
const project = await api.get<ProjectResponse>('/api/projects/123')
```

## Utility Types

Leverage TypeScript utility types:

```typescript
// Partial - make all properties optional
type PartialProject = Partial<Project>

// Pick - select specific properties
type ProjectSummary = Pick<Project, 'id' | 'name' | 'status'>

// Omit - exclude specific properties
type ProjectWithoutId = Omit<Project, 'id'>

// Required - make all properties required
type RequiredProject = Required<Project>

// Readonly - make all properties readonly
type ImmutableProject = Readonly<Project>

// Record - create object type with specific keys
type ProjectMap = Record<string, Project>
```

## Enums vs Union Types

Prefer **union types** over enums for better type safety:

```typescript
// Good: Union type
type ProjectStatus = 'NotStarted' | 'Initiated' | 'InProgress' | 'Completed' | 'Cancelled'

// Create helper for validation
const PROJECT_STATUSES = ['NotStarted', 'Initiated', 'InProgress', 'Completed', 'Cancelled'] as const

function isValidStatus(status: string): status is ProjectStatus {
  return PROJECT_STATUSES.includes(status as ProjectStatus)
}
```

## Type Assertions

Use type assertions sparingly and safely:

```typescript
// Bad: Unsafe assertion
const project = data as Project

// Good: Use type guard first
if (isProject(data)) {
  const project = data // TypeScript infers Project type
}

// Good: Use assertion with validation
const element = document.getElementById('app')
if (element) {
  const app = element as HTMLDivElement
}
```

## Best Practices

1. **Always define return types** for functions
2. **Use `unknown` instead of `any`** when type is truly unknown
3. **Prefer interfaces for public APIs**
4. **Use type aliases for complex types**
5. **Leverage type inference** when obvious
6. **Document complex types** with JSDoc comments
7. **Use const assertions** for literal types
8. **Avoid type assertions** unless absolutely necessary

```typescript
// Good: Explicit return type
function calculateTotal(items: Item[]): number {
  return items.reduce((sum, item) => sum + item.price, 0)
}

// Good: Const assertion for literal types
const ROUTES = {
  HOME: '/',
  PROJECTS: '/projects',
  TRANSACTIONS: '/transactions'
} as const

type Route = typeof ROUTES[keyof typeof ROUTES]
```
