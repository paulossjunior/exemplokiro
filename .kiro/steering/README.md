# Steering Rules Overview

This directory contains AI assistant steering rules organized by concern.

## Structure

### Back-End (`back-end/`)

Rules specific to the .NET 8 API back-end:

- **api-design.md**: RESTful API design, OpenAPI/Swagger documentation, performance requirements
- **architecture.md**: Hexagonal architecture, dependency injection, layer organization
- **code-quality.md**: SOLID principles, clean code practices, documentation standards
- **tech-stack.md**: .NET 8, Entity Framework Core, SQL Server, Docker
- **testing.md**: TDD, integration testing with real database, test strategies

### Front-End (`front-end/`)

Rules specific to the Vue 3 + TypeScript front-end:

- **accessibility-wcag.md**: WCAG 2.0 Level AA compliance, ARIA attributes, keyboard navigation
- **semantic-html.md**: HTML5 semantic elements, proper document structure
- **tailwind-standards.md**: Tailwind CSS utility-first approach, responsive design, component patterns
- **tech-stack.md**: Vue 3, TypeScript, Vite, Pinia, project structure
- **typescript-standards.md**: Type safety, interfaces vs types, generics, best practices
- **vue-standards.md**: Composition API, component structure, composables, state management

### General

- **product.md**: Product overview and domain concepts
- **documentation.md**: IEEE documentation standards

## Usage

These steering rules are automatically included when working on relevant files. You can also reference them manually using the `#` context in chat.

## Key Principles

### Back-End
- Hexagonal architecture with clear layer separation
- SOLID principles and dependency injection
- API-first design with OpenAPI/Swagger
- < 100ms response time requirement
- Real database testing (no in-memory)

### Front-End
- Vue 3 Composition API with TypeScript
- Semantic HTML5 elements
- Tailwind CSS utility-first styling
- WCAG 2.0 Level AA accessibility
- Mobile-first responsive design

### Both
- All code and documentation in English
- Comprehensive documentation
- Test-driven development
- Clean code practices
