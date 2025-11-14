# Requirements Document - Expense Tracking Dashboard

## Introduction

This document specifies the requirements for an Expense Tracking Dashboard that enables users to monitor their project expenses through a visual thermometer chart and filter transactions using various criteria. The dashboard provides real-time visibility into consumed budget, remaining balance, and account yield.

## Glossary

- **System**: The Expense Tracking Dashboard
- **User**: A person accessing the dashboard to monitor project expenses
- **Thermometer Chart**: A visual gauge displaying budget consumption progress
- **Consumed Amount**: The total amount of money spent from the project budget
- **Remaining Amount**: The available balance left in the project budget
- **Yield Amount**: The financial return or interest earned on the account
- **Transaction**: A financial record with payment method, value, date, CNPJ, and status
- **Filter**: A mechanism to narrow down displayed transactions based on criteria
- **Status**: The current state of a transaction (Em Validação, Pendente, Validado, Revisar)
- **Dropdown**: A UI component that expands to show a list of selectable options
- **Date Picker**: A UI component for selecting dates
- **Hover Effect**: A visual change when the mouse cursor is over an element
- **Zinc Theme**: Tailwind CSS color palette using zinc shades for dark mode

## Requirements

### Requirement 1

**User Story:** As a user, I want to see a thermometer chart displaying my budget consumption, so that I can quickly understand my spending status at a glance.

#### Acceptance Criteria

1. THE System SHALL display a horizontal thermometer chart showing budget consumption progress
2. THE System SHALL display the consumed amount with label "Consumido" in cyan color (text-cyan-400)
3. THE System SHALL display the remaining amount with label "Restante" in cyan color (text-cyan-400)
4. THE System SHALL display the yield amount with label "Rendimento" in cyan color (text-cyan-400)
5. THE System SHALL format all monetary values with Brazilian Real currency format (R$ X.XXX,XX)

### Requirement 2

**User Story:** As a user, I want to select a project from a dropdown list, so that I can view expenses specific to that project.

#### Acceptance Criteria

1. THE System SHALL provide a dropdown input labeled "Selecione seu projeto"
2. WHEN the user clicks the project dropdown, THE System SHALL display a list of available projects
3. THE System SHALL allow the user to select one project from the list
4. THE System SHALL update the dashboard data when a project is selected
5. THE System SHALL use zinc color scheme for the dropdown (bg-zinc-800, border-zinc-700)

### Requirement 3

**User Story:** As a user, I want to search for transactions using a text input, so that I can quickly find specific expenses.

#### Acceptance Criteria

1. THE System SHALL provide a search input field with placeholder "Pesquisar"
2. THE System SHALL display a search icon inside the input field
3. WHEN the user types in the search field, THE System SHALL filter transactions in real-time
4. THE System SHALL search across payment method, CNPJ, and other transaction fields
5. THE System SHALL use zinc color scheme for the search input (bg-zinc-800, border-zinc-700)

### Requirement 4

**User Story:** As a user, I want to select a date using a date picker, so that I can filter transactions by specific dates or date ranges.

#### Acceptance Criteria

1. THE System SHALL provide a date picker input with label "Data"
2. WHEN the user clicks the date input, THE System SHALL display a calendar interface
3. THE System SHALL allow the user to select a date from the calendar
4. THE System SHALL filter transactions based on the selected date
5. THE System SHALL use zinc color scheme for the date picker (bg-zinc-800, border-zinc-700)

### Requirement 5

**User Story:** As a user, I want to filter transactions by status, so that I can focus on transactions in specific states.

#### Acceptance Criteria

1. THE System SHALL provide a status dropdown with label "Status"
2. THE System SHALL display status options: "Em Validação", "Pendente", "Validado", "Revisar"
3. WHEN the user selects a status, THE System SHALL filter transactions to show only those with the selected status
4. THE System SHALL display status badges with appropriate colors (blue for Em Validação, red for Pendente, green for Validado, orange for Revisar)
5. THE System SHALL use zinc color scheme for the dropdown (bg-zinc-800, border-zinc-700)

### Requirement 6

**User Story:** As a user, I want to filter transactions by category, so that I can analyze expenses by type.

#### Acceptance Criteria

1. THE System SHALL provide a category dropdown with label "Categoria"
2. THE System SHALL display a list of available expense categories
3. WHEN the user selects a category, THE System SHALL filter transactions to show only those in the selected category
4. THE System SHALL allow clearing the category filter to show all transactions
5. THE System SHALL use zinc color scheme for the dropdown (bg-zinc-800, border-zinc-700)

### Requirement 7

**User Story:** As a user, I want to see a list of transactions with their details, so that I can review individual expenses.

#### Acceptance Criteria

1. THE System SHALL display a table with columns: Pagamento, Valor, Data, CNPJ, Status
2. THE System SHALL display transaction payment methods (Pix, Boleto, etc.)
3. THE System SHALL format transaction values in Brazilian Real currency format
4. THE System SHALL display transaction dates in format DD/MM/YYYY - HH:MM:SS
5. THE System SHALL display CNPJ or company names for each transaction

### Requirement 8

**User Story:** As a user, I want to see visual status indicators for each transaction, so that I can quickly identify transaction states.

#### Acceptance Criteria

1. THE System SHALL display status badges with colored backgrounds and icons
2. THE System SHALL use blue background with dot icon for "Em Validação" status
3. THE System SHALL use red background with dot icon for "Pendente" status
4. THE System SHALL use green background with dot icon for "Validado" status
5. THE System SHALL use orange background with dot icon for "Revisar" status

### Requirement 9

**User Story:** As a user, I want table rows to highlight when I hover over them, so that I can easily track which row I'm viewing.

#### Acceptance Criteria

1. WHEN the user hovers the mouse over a table row, THE System SHALL lighten the row background color
2. THE System SHALL change the row background from transparent to zinc-800/50
3. THE System SHALL apply a smooth transition effect to the hover state
4. THE System SHALL maintain the hover effect until the mouse leaves the row
5. THE System SHALL ensure the hover effect does not interfere with text readability

### Requirement 10

**User Story:** As a user, I want to navigate through multiple pages of transactions, so that I can view all expenses without overwhelming the interface.

#### Acceptance Criteria

1. THE System SHALL display pagination controls at the bottom of the transaction list
2. THE System SHALL show the current page number and total pages
3. THE System SHALL provide previous and next page navigation buttons
4. THE System SHALL display "Exibindo X resultados de Y" showing the result count
5. THE System SHALL use zinc color scheme for pagination controls

### Requirement 11

**User Story:** As a user, I want to apply all filters simultaneously with a search button, so that I can refine my transaction view precisely.

#### Acceptance Criteria

1. THE System SHALL provide a "Buscar" button in cyan color (bg-cyan-500)
2. WHEN the user clicks the "Buscar" button, THE System SHALL apply all selected filters
3. THE System SHALL update the transaction list based on combined filter criteria
4. THE System SHALL maintain filter selections after applying them
5. THE System SHALL provide visual feedback when the button is clicked

### Requirement 12

**User Story:** As a user, I want the interface to follow WCAG 2.0 accessibility standards, so that all users can access the dashboard effectively.

#### Acceptance Criteria

1. THE System SHALL provide proper ARIA labels for all interactive elements
2. THE System SHALL ensure color contrast ratios meet WCAG 2.0 Level AA standards
3. THE System SHALL support full keyboard navigation for all controls
4. THE System SHALL provide screen reader announcements for dynamic content updates
5. THE System SHALL include focus indicators for all focusable elements

### Requirement 13

**User Story:** As a user, I want the interface to be responsive, so that I can access the dashboard on different devices.

#### Acceptance Criteria

1. THE System SHALL adapt the layout for mobile, tablet, and desktop screen sizes
2. THE System SHALL stack filter inputs vertically on mobile devices
3. THE System SHALL make the transaction table horizontally scrollable on small screens
4. THE System SHALL maintain readability of the thermometer chart on all screen sizes
5. THE System SHALL use Tailwind responsive prefixes (sm:, md:, lg:) for breakpoints

### Requirement 14

**User Story:** As a user, I want the interface to use a dark theme with zinc colors, so that I have a comfortable viewing experience.

#### Acceptance Criteria

1. THE System SHALL use zinc-900 as the primary background color
2. THE System SHALL use zinc-800 for card and input backgrounds
3. THE System SHALL use zinc-700 for borders and dividers
4. THE System SHALL use white or zinc-100 for primary text
5. THE System SHALL use zinc-400 for secondary text and labels
