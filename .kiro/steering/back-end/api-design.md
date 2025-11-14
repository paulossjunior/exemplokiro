---
inclusion: manual
---

# API Design and Documentation

## API First Approach

- **Design APIs before implementation**
- Define OpenAPI/Swagger specification first
- Review and validate API contracts with stakeholders
- Generate client SDKs from specifications
- Use specification as source of truth

## Performance Requirements

### Critical Performance Target

- **All API requests must respond in less than 100ms**
- Monitor and measure response times for all endpoints
- Optimize database queries and use appropriate indexes
- Implement caching strategies where appropriate
- Use async/await patterns for I/O operations
- Profile and optimize slow endpoints immediately

### Performance Testing

- Include performance tests in test suite
- Fail tests if response time exceeds 100ms threshold
- Monitor response times in production
- Set up alerts for performance degradation

### Optimization Strategies

- Use Entity Framework query optimization (AsNoTracking, projections)
- Implement database indexes on frequently queried fields
- Use caching for read-heavy operations
- Minimize database round trips
- Use compiled queries where appropriate
- Consider pagination for large result sets

## Swagger/OpenAPI Documentation

- All APIs must be documented with Swagger/OpenAPI
- Use Swashbuckle.AspNetCore for .NET integration
- Include detailed descriptions for endpoints, parameters, and responses
- Document all possible HTTP status codes
- Provide example requests and responses
- Keep documentation synchronized with implementation

### Swagger Configuration

```bash
# Access Swagger UI
http://localhost:5000/swagger

# Access OpenAPI specification
http://localhost:5000/swagger/v1/swagger.json
```

### Documentation Requirements

- Summary and description for each endpoint
- Parameter descriptions and constraints
- Response schemas with examples
- Authentication/authorization requirements
- Error response formats
- Expected response times

## RESTful API Standards

### HTTP Methods

- **GET**: Retrieve resources (idempotent, safe)
- **POST**: Create new resources
- **PUT**: Update entire resource (idempotent)
- **PATCH**: Partial update of resource
- **DELETE**: Remove resource (idempotent)

### Status Codes

- **200 OK**: Successful GET, PUT, PATCH
- **201 Created**: Successful POST
- **204 No Content**: Successful DELETE
- **400 Bad Request**: Invalid input
- **401 Unauthorized**: Missing or invalid authentication
- **403 Forbidden**: Authenticated but not authorized
- **404 Not Found**: Resource doesn't exist
- **409 Conflict**: Duplicate or constraint violation
- **422 Unprocessable Entity**: Validation errors
- **500 Internal Server Error**: Server-side error

### URL Conventions

- Use nouns, not verbs: `/api/projects` not `/api/getProjects`
- Use plural nouns: `/api/projects` not `/api/project`
- Use kebab-case for multi-word resources: `/api/accounting-accounts`
- Nest resources logically: `/api/projects/{id}/transactions`
- Use query parameters for filtering: `/api/projects?status=active`

### Response Format

```json
{
  "data": {
    "id": "guid",
    "name": "Project Name",
    "status": "InProgress"
  },
  "meta": {
    "timestamp": "2025-11-13T10:30:00Z",
    "version": "1.0"
  }
}
```

### Error Response Format

```json
{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "The transaction amount must be greater than zero",
    "details": [
      {
        "field": "amount",
        "issue": "Value must be positive"
      }
    ],
    "timestamp": "2025-11-13T10:30:00Z",
    "traceId": "abc123-def456"
  }
}
```
