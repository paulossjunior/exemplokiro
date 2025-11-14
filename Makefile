.PHONY: help up down rebuild logs clean test migrate-add migrate-up dev prod status logs-api logs-frontend logs-db

# Default target
help:
	@echo "Project Budget Management System - Docker Commands"
	@echo ""
	@echo "Available targets:"
	@echo "  make dev          - Start all services in development mode (with hot reload)"
	@echo "  make prod         - Start all services in production mode"
	@echo "  make up           - Alias for 'make dev'"
	@echo "  make down         - Stop all services"
	@echo "  make rebuild      - Rebuild and restart all services"
	@echo "  make logs         - View logs from all services"
	@echo "  make logs-api     - View API logs only"
	@echo "  make logs-frontend - View front-end logs only"
	@echo "  make logs-db      - View database logs only"
	@echo "  make status       - Show status of all containers"
	@echo "  make clean        - Stop services and remove volumes"
	@echo "  make test         - Run tests in Docker"
	@echo "  make migrate-add  - Add a new migration (use NAME=MigrationName)"
	@echo "  make migrate-up   - Apply pending migrations"

# Start services in development mode (default)
dev:
	@echo "Starting services in DEVELOPMENT mode..."
	docker-compose up -d --build
	@echo ""
	@echo "✅ Services started successfully!"
	@echo ""
	@echo "📱 Front-end (Vite dev server): http://localhost:5173"
	@echo "🔧 API: http://localhost:5000"
	@echo "📚 Swagger UI: http://localhost:5000/swagger"
	@echo "🗄️  SQL Server: localhost:1433"
	@echo ""
	@echo "Use 'make logs' to view logs or 'make status' to check container status"

# Start services in production mode
prod:
	@echo "Starting services in PRODUCTION mode..."
	docker-compose -f docker-compose.prod.yml up -d --build
	@echo ""
	@echo "✅ Services started successfully!"
	@echo ""
	@echo "📱 Front-end (nginx): http://localhost"
	@echo "🔧 API: http://localhost:5000"
	@echo "📚 Swagger UI: http://localhost:5000/swagger"
	@echo "🗄️  SQL Server: localhost:1433"
	@echo ""
	@echo "Use 'make logs' to view logs or 'make status' to check container status"

# Alias for dev
up: dev

# Stop services
down:
	@echo "Stopping all services..."
	docker-compose down
	@echo "✅ Services stopped"

# Rebuild and restart services
rebuild:
	@echo "Rebuilding and restarting services..."
	docker-compose down
	docker-compose up -d --build
	@echo "✅ Services rebuilt and restarted"

# View logs from all services
logs:
	docker-compose logs -f

# View API logs only
logs-api:
	docker-compose logs -f api

# View front-end logs only
logs-frontend:
	docker-compose logs -f frontend

# View database logs only
logs-db:
	docker-compose logs -f sqlserver

# Show container status
status:
	@echo "Container Status:"
	@docker-compose ps

# Clean up containers and volumes
clean:
	docker-compose down -v
	@echo "All containers and volumes removed"

# Run tests (placeholder for future test implementation)
test:
	@echo "Test infrastructure not yet implemented"
	@echo "Tests will run in Docker containers once test project is created"

# Add a new migration
migrate-add:
ifndef NAME
	@echo "Error: Migration name required. Usage: make migrate-add NAME=MigrationName"
	@exit 1
endif
	dotnet ef migrations add $(NAME) --project src/ProjectBudgetManagement.Infrastructure --startup-project src/ProjectBudgetManagement.Api

# Apply migrations
migrate-up:
	dotnet ef database update --project src/ProjectBudgetManagement.Infrastructure --startup-project src/ProjectBudgetManagement.Api
