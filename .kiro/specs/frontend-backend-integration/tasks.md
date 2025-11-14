# Implementation Plan: Front-End and Back-End Integration

- [x] 1. Set up HTTP client and base configuration
  - Create Axios HTTP client instance with base URL configuration
  - Implement request interceptor to attach authentication tokens
  - Implement response interceptor for error handling and logging
  - Configure timeout and default headers
  - Add development mode logging for requests and responses
  - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.5_

- [x] 2. Define TypeScript type definitions
  - Create common API types (ApiResponse, PaginatedResponse, ApiError)
  - Define Project-related types (Project, CreateProjectRequest, UpdateProjectRequest, ProjectStatus)
  - Define Transaction-related types (Transaction, CreateTransactionRequest, TransactionClassification, AccountBalance)
  - Define AccountingAccount types
  - Define Audit-related types (AuditEntry, IntegrityVerificationResult)
  - Define Report-related types (GenerateReportRequest, ReportResponse)
  - _Requirements: 7.1, 7.2, 7.3, 7.4, 7.5_

- [x] 3. Implement API service classes
- [x] 3.1 Create ProjectService
  - Implement getProjects method with pagination and filtering
  - Implement getProject method for single project retrieval
  - Implement createProject method
  - Implement updateProject method
  - Implement updateProjectStatus method
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5, 8.1, 8.2, 8.3, 8.4, 8.5_

- [x] 3.2 Create TransactionService
  - Implement getTransactions method with filtering and pagination
  - Implement createTransaction method with authentication
  - Implement getBalance method for account balance retrieval
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5, 9.1, 9.2, 9.3, 9.4, 9.5_

- [x] 3.3 Create AccountingAccountService
  - Implement getAccounts method to fetch all accounting accounts
  - Implement getAccount method for single account retrieval
  - Implement createAccount method
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5_

- [x] 3.4 Create AuditService
  - Implement getAuditTrail method with filtering and pagination
  - Implement verifyIntegrity method for data integrity verification
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5, 11.1, 11.2, 11.3, 11.4, 11.5_

- [x] 3.5 Create ReportService
  - Implement generateAccountabilityReport method
  - Implement downloadReport method with blob response handling
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5, 10.1, 10.2, 10.3, 10.4, 10.5_

- [x] 4. Create reusable composables
- [x] 4.1 Implement useApi composable
  - Create generic useApi composable for API call management
  - Implement loading state management
  - Implement error state management
  - Implement data state management
  - Add support for immediate execution option
  - Add support for success and error callbacks
  - Implement error transformation from Axios errors to ApiError
  - _Requirements: 5.1, 5.2, 5.3, 5.4, 5.5, 6.1, 6.2, 6.3, 6.4_

- [x] 4.2 Implement useProjects composable
  - Create useProjects composable wrapping ProjectService
  - Implement fetchProjects method with loading state
  - Implement fetchProject method for single project
  - Implement createProject method
  - Implement updateProject method
  - Implement updateStatus method
  - Manage projects array state
  - Manage currentProject state
  - Provide computed loading state
  - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5_

- [x] 4.3 Implement useTransactions composable
  - Create useTransactions composable wrapping TransactionService
  - Implement fetchTransactions method with filtering
  - Implement createTransaction method
  - Implement fetchBalance method
  - Manage transactions array state
  - Manage balance state
  - _Requirements: 9.1, 9.2, 9.3, 9.4, 9.5_

- [x] 4.4 Implement useDebounce composable
  - Create useDebounce composable for search input debouncing
  - Implement configurable delay parameter
  - Use Vue watch to track value changes
  - Return debounced value ref
  - _Requirements: 13.3_

- [x] 5. Implement Pinia stores
- [x] 5.1 Create stores directory and configure Pinia
  - Create front-end/src/stores directory
  - Ensure Pinia is installed and configured in main.ts
  - _Requirements: 4.1, 4.2, 4.3, 4.4, 4.5_

- [x] 5.2 Create authStore
  - Implement token state management
  - Implement user state management
  - Implement isAuthenticated computed property
  - Implement setToken method with localStorage persistence
  - Implement setUser method
  - Implement logout method with cleanup
  - _Requirements: 4.1, 4.2, 4.3, 4.4, 4.5_

- [x] 5.3 Create accountingAccountStore
  - Implement accounts state management
  - Implement lastFetch timestamp for cache invalidation
  - Implement fetchAccounts method with caching logic
  - Implement 5-minute cache duration
  - Support force refresh parameter
  - _Requirements: 13.1, 13.2_

- [x] 6. Configure CORS on back-end
  - Add CORS policy in Program.cs to allow front-end origin
  - Configure allowed methods (GET, POST, PUT, DELETE, OPTIONS)
  - Configure allowed headers (Authorization, Content-Type)
  - Enable credentials support
  - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5_

- [x] 6.1 Update CORS configuration for specific front-end origin
  - Update Program.cs to use FrontEndUrl from configuration instead of AllowAnyOrigin
  - Add FrontEndUrl configuration in appsettings.json and appsettings.Development.json
  - Enable AllowCredentials for cookie-based authentication
  - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5_

- [x] 7. Create error handling components
  - Create ErrorAlert component for displaying API errors
  - Display error message prominently
  - Display field-specific validation errors when available
  - Add retry button with callback support
  - Implement ARIA role="alert" for accessibility
  - Style error component with Tailwind CSS
  - _Requirements: 5.1, 5.2, 5.3, 5.4, 5.5_

- [x] 8. Create loading state components
  - Create LoadingSpinner component for loading indicators
  - Implement ARIA live region for screen reader announcements
  - Create LoadingOverlay component for full-page loading
  - Ensure loading components are keyboard accessible
  - Prevent focus trap during loading states
  - Style components with Tailwind CSS
  - _Requirements: 6.1, 6.2, 6.3, 6.4, 6.5_

- [x] 9. Implement environment configuration
  - Create .env.development file with local API URL
  - Create .env.production file with production API URL
  - Add VITE_API_BASE_URL environment variable
  - Add VITE_ENABLE_API_LOGGING environment variable
  - Update httpClient to use environment variable for base URL
  - _Requirements: 15.1, 15.2_

- [x] 10. Configure Vite proxy for development
  - Add proxy configuration in vite.config.ts
  - Proxy /api requests to back-end server
  - Enable changeOrigin for proxy
  - Test proxy with local development server
  - _Requirements: 15.3_

- [x] 11. Implement project management UI integration
- [x] 11.1 Create ProjectList component
  - Fetch projects using useProjects composable
  - Display projects in a table or card layout
  - Show loading state while fetching
  - Display error message if fetch fails
  - Implement pagination controls
  - Add filter by status dropdown
  - Style with Tailwind CSS and ensure WCAG 2.0 AA compliance
  - _Requirements: 8.2_

- [x] 11.2 Create ProjectDetails component
  - Fetch single project using useProjects composable
  - Display project information (name, description, dates, budget, coordinator, bank account)
  - Show loading state while fetching
  - Display error message if fetch fails
  - Add edit button to navigate to edit form
  - Display project status with visual indicator
  - Style with Tailwind CSS and ensure WCAG 2.0 AA compliance
  - _Requirements: 8.3_

- [x] 11.3 Create ProjectForm component
  - Create form for project creation with all required fields
  - Implement form validation matching back-end rules
  - Use createProject method from useProjects composable
  - Show loading state during submission
  - Display validation errors inline with form fields
  - Handle API errors and display error messages
  - Redirect to project details on success
  - Style with Tailwind CSS and ensure WCAG 2.0 AA compliance
  - _Requirements: 8.1, 12.1, 12.2, 12.3, 12.4, 12.5_

- [x] 11.4 Create ProjectStatusUpdate component
  - Create dropdown or button group for status selection
  - Use updateStatus method from useProjects composable
  - Show loading state during update
  - Display success message on status change
  - Handle errors and display error messages
  - Style with Tailwind CSS and ensure WCAG 2.0 AA compliance
  - _Requirements: 8.5_

- [x] 12. Implement transaction management UI integration
- [x] 12.1 Create TransactionList component
  - Fetch transactions using useTransactions composable
  - Display transactions in a table with all details
  - Show loading state while fetching
  - Display error message if fetch fails
  - Implement pagination controls
  - Add filters for date range and classification
  - Display digital signature for each transaction
  - Style with Tailwind CSS and ensure WCAG 2.0 AA compliance
  - _Requirements: 9.2, 9.4, 9.5_

- [x] 12.2 Create TransactionForm component
  - Create form for transaction creation with all required fields
  - Fetch accounting accounts for dropdown selection
  - Implement form validation matching back-end rules
  - Use createTransaction method from useTransactions composable
  - Show loading state during submission
  - Display validation errors inline with form fields
  - Handle authentication requirement
  - Display success message on creation
  - Style with Tailwind CSS and ensure WCAG 2.0 AA compliance
  - _Requirements: 9.1, 12.1, 12.2, 12.3, 12.4, 12.5_

- [x] 12.3 Create AccountBalance component
  - Fetch balance using useTransactions composable
  - Display budget amount, current balance, total credits, and total debits
  - Show visual indicator if over budget
  - Display calculation timestamp
  - Auto-refresh balance when transactions change
  - Style with Tailwind CSS and ensure WCAG 2.0 AA compliance
  - _Requirements: 9.3_

- [x] 13. Implement reporting UI integration
- [x] 13.1 Create ReportGenerator component
  - Create form for report parameters (date range, include audit trail)
  - Use generateAccountabilityReport method from reportService
  - Show loading state during report generation
  - Display progress indicator
  - Handle errors and display error messages
  - Provide download link when report is ready
  - Style with Tailwind CSS and ensure WCAG 2.0 AA compliance
  - _Requirements: 10.1, 10.2, 10.3, 10.4_

- [x] 13.2 Implement report download functionality
  - Use downloadReport method from reportService
  - Handle blob response type
  - Create download link and trigger download
  - Show loading state during download
  - Handle download errors
  - _Requirements: 10.5_

- [x] 14. Implement audit trail UI integration
- [x] 14.1 Create AuditTrail component
  - Fetch audit entries using auditService
  - Display audit entries in a table with all details
  - Show loading state while fetching
  - Display error message if fetch fails
  - Implement pagination controls
  - Add filters for entity type, date range, and user
  - Style with Tailwind CSS and ensure WCAG 2.0 AA compliance
  - _Requirements: 11.1, 11.2, 11.5_

- [x] 14.2 Create IntegrityVerification component
  - Create button to trigger integrity verification
  - Use verifyIntegrity method from auditService
  - Show loading state during verification
  - Display verification results with clear indicators
  - Show list of invalid records if any
  - Display verification timestamp
  - Style with Tailwind CSS and ensure WCAG 2.0 AA compliance
  - _Requirements: 11.3, 11.4_

- [x] 15. Implement offline handling
  - Add network status detection in httpClient
  - Display offline indicator when back-end is unreachable
  - Implement retry logic with exponential backoff
  - Limit retry attempts to 3 times
  - Display last successful fetch timestamp
  - Auto-resume when connectivity is restored
  - _Requirements: 14.1, 14.2, 14.3, 14.4, 14.5_

- [x] 16. Update Docker Compose configuration
  - Add front-end service to docker-compose.yml
  - Configure front-end environment variables for API URL
  - Set up dependency on API service
  - Expose front-end port (5173)
  - Add front-end to project network
  - _Requirements: 15.1, 15.2_

- [x] 17. Create front-end Dockerfile
  - Create multi-stage Dockerfile for front-end
  - Install dependencies in build stage
  - Build production bundle
  - Serve with nginx in production stage
  - Configure nginx for SPA routing
  - _Requirements: 15.1_

- [ ]* 18. Write integration tests
  - Create test utilities for mocking API responses
  - Write tests for HTTP client interceptors
  - Write tests for API service methods
  - Write tests for composables (useApi, useProjects, useTransactions)
  - Write tests for error handling scenarios
  - Write tests for authentication flow
  - _Requirements: All requirements - testing_

- [ ]* 19. Write E2E tests
  - Set up E2E testing framework (Playwright or Cypress)
  - Write E2E test for project creation workflow
  - Write E2E test for transaction recording workflow
  - Write E2E test for report generation workflow
  - Write E2E test for authentication and authorization
  - Write E2E test for error recovery scenarios
  - _Requirements: All requirements - E2E testing_

- [ ] 20. Documentation and examples
  - Document API service usage with examples
  - Document composable usage patterns
  - Create troubleshooting guide for common integration issues
  - Document environment configuration
  - Add inline code comments for complex logic
  - Update main README with front-end setup instructions
  - _Requirements: All requirements - documentation_
