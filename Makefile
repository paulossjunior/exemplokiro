.PHONY: help up down rebuild logs clean test migrate-add migrate-up

# Default target
help:
	@echo "Project Budget Management System - Docker Commands"
	@echo ""
	@echo "Available targets:"
	@echo "  make up           - Build and start all services"
	@echo "  make down         - Stop all services"
	@echo "  make rebuild      - Rebuild and restart all services"
	@echo "  make logs         - View logs from all services"
	@echo "  make clean        - Stop services and remove volumes"
	@echo "  make test         - Run tests in Docker"
	@echo "  make migrate-add  - Add a new migration (use NAME=MigrationName)"
	@echo "  make migrate-up   - Apply pending migrations"

# Build and start services
up:
	docker-compose up -d --build
	@echo "Services started. API available at http://localhost:5000"
	@echo "Swagger UI available at http://localhost:5000/swagger"

# Stop services
down:
	docker-compose down

# Rebuild and restart services
rebuild:
	docker-compose down
	docker-compose up -d --build

# View logs
logs:
	docker-compose logs -f

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
