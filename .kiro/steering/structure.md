# Project Structure

## Overview

This project is divided into two main parts:
- **Back-End**: .NET 8 API with hexagonal architecture
- **Front-End**: Vue 3 + TypeScript + Tailwind CSS

## Back-End Structure

```
src/
├── ProjectBudgetManagement.Domain/
│   ├── Entities/          # Domain entities
│   ├── ValueObjects/      # Value objects
│   └── Services/          # Domain services
├── ProjectBudgetManagement.Application/
│   ├── Commands/          # Command handlers
│   ├── Queries/           # Query handlers
│   ├── Services/          # Application services
│   └── Ports/             # Repository interfaces
├── ProjectBudgetManagement.Infrastructure/
│   ├── Persistence/       # DbContext and migrations
│   └── Repositories/      # Repository implementations
└── ProjectBudgetManagement.Api/
    ├── Controllers/       # API controllers
    ├── Models/            # DTOs
    └── Middleware/        # Custom middleware
```

## Front-End Structure

```
front-end/
├── src/
│   ├── assets/            # Static assets
│   ├── components/        # Vue components
│   │   ├── common/        # Reusable UI components
│   │   ├── forms/         # Form components
│   │   └── layout/        # Layout components
│   ├── composables/       # Vue composables
│   ├── views/             # Page components
│   ├── router/            # Vue Router
│   ├── stores/            # Pinia stores
│   ├── services/          # API services
│   ├── types/             # TypeScript types
│   └── utils/             # Utility functions
├── public/                # Public static files
└── index.html             # Entry point
```

## Steering Organization

Steering files are organized by concern:

```
.kiro/steering/
├── back-end/
│   ├── api-design.md      # API design standards
│   ├── architecture.md    # Hexagonal architecture
│   ├── code-quality.md    # Code quality standards
│   ├── tech-stack.md      # Technology stack
│   └── testing.md         # Testing standards
├── front-end/
│   ├── accessibility-wcag.md  # WCAG 2.0 standards
│   ├── semantic-html.md       # Semantic HTML
│   ├── tailwind-standards.md  # Tailwind CSS
│   ├── tech-stack.md          # Front-end stack
│   ├── typescript-standards.md # TypeScript
│   └── vue-standards.md       # Vue 3 standards
├── product.md             # Product overview
└── documentation.md       # Documentation standards
```

## Conventions

- **Back-End**: Follow hexagonal architecture principles
- **Front-End**: Use Vue 3 Composition API with TypeScript
- **Styling**: Tailwind CSS utility-first approach
- **Accessibility**: WCAG 2.0 Level AA compliance
- **Language**: All code and documentation in English
