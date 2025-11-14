# Front-End Docker Build Guide

## Overview

This document describes the production-ready Docker setup for the Vue 3 front-end application. The Dockerfile uses a multi-stage build process to create an optimized, secure nginx-based container.

## Dockerfile Features

### Multi-Stage Build

The Dockerfile uses two stages:

1. **Build Stage** (node:20-alpine)
   - Installs dependencies
   - Builds the Vue application
   - Uses build arguments for environment configuration

2. **Production Stage** (nginx:alpine)
   - Serves the built static files
   - Runs as non-root user for security
   - Includes health checks
   - Optimized for production

### Security Features

- **Non-root user**: Container runs as `nginx` user (not root)
- **Minimal base image**: Uses Alpine Linux for smaller attack surface
- **Security headers**: Configured in nginx.conf
- **No unnecessary packages**: Only includes curl for health checks

### Performance Optimizations

- **Layer caching**: Optimized layer order for faster rebuilds
- **Gzip compression**: Enabled for all text-based assets
- **Static asset caching**: 1-year cache for immutable assets
- **No caching for index.html**: Ensures users get latest version

## Building the Image

### Basic Build

```bash
docker build -t projectbudget-frontend:latest ./front-end
```

### Build with Custom API URL

```bash
docker build \
  --build-arg VITE_API_BASE_URL=https://api.example.com \
  -t projectbudget-frontend:latest \
  ./front-end
```

### Build Arguments

| Argument | Description | Default |
|----------|-------------|---------|
| `VITE_API_BASE_URL` | Back-end API base URL | (none) |
| `VITE_ENABLE_API_LOGGING` | Enable API request logging | `false` |

## Running the Container

### Basic Run

```bash
docker run -d \
  --name projectbudget-frontend \
  -p 80:80 \
  projectbudget-frontend:latest
```

### Run with Environment Variables

```bash
docker run -d \
  --name projectbudget-frontend \
  -p 80:80 \
  -e VITE_API_BASE_URL=https://api.example.com \
  projectbudget-frontend:latest
```

**Note**: Environment variables must be set at build time, not runtime, because Vite bundles them into the static files.

## Health Checks

The container includes a health check that:
- Runs every 30 seconds
- Has a 3-second timeout
- Allows 5 seconds for startup
- Retries 3 times before marking unhealthy

Check health status:

```bash
docker inspect projectbudget-frontend --format='{{.State.Health.Status}}'
```

## Nginx Configuration

### SPA Routing

The nginx configuration supports Vue Router by serving `index.html` for all routes:

```nginx
location / {
    try_files $uri $uri/ /index.html;
}
```

### Caching Strategy

- **Static assets** (JS, CSS, images): Cached for 1 year
- **index.html**: No caching (always fresh)
- **Gzip compression**: Enabled for all text-based content

### Security Headers

The following security headers are automatically added:

- `X-Frame-Options: SAMEORIGIN`
- `X-Content-Type-Options: nosniff`
- `X-XSS-Protection: 1; mode=block`
- `Referrer-Policy: no-referrer-when-downgrade`

## Docker Compose Integration

The front-end service is configured in the main `docker-compose.yml`:

```yaml
frontend:
  build:
    context: ./front-end
    dockerfile: Dockerfile
    args:
      VITE_API_BASE_URL: http://api:5000
  container_name: projectbudget-frontend
  ports:
    - "80:80"
  depends_on:
    - api
  networks:
    - projectbudget-network
```

Start with Docker Compose:

```bash
docker-compose up -d frontend
```

## Development vs Production

### Development (Dockerfile.dev)

- Uses Vite dev server
- Hot module replacement
- Source maps enabled
- Port 5173

### Production (Dockerfile)

- Static files served by nginx
- Optimized bundle
- No source maps
- Port 80

## Troubleshooting

### Build Fails with TypeScript Errors

The Dockerfile skips type checking during build (`npx vite build` instead of `npm run build`). Type checking should be done in CI/CD separately:

```bash
npm run build  # Includes type checking
```

### Container Starts but Shows 404

Ensure the build completed successfully and `dist` folder was created:

```bash
docker run --rm projectbudget-frontend:latest ls -la /usr/share/nginx/html
```

### Health Check Fails

Check nginx logs:

```bash
docker logs projectbudget-frontend
```

### SPA Routes Return 404

Verify nginx configuration is correctly copied:

```bash
docker exec projectbudget-frontend cat /etc/nginx/conf.d/default.conf
```

## Best Practices

1. **Build Arguments**: Set API URL at build time for different environments
2. **Image Tagging**: Use semantic versioning for production images
3. **Security Scanning**: Scan images for vulnerabilities before deployment
4. **Resource Limits**: Set memory and CPU limits in production
5. **Logging**: Configure nginx to output logs to stdout/stderr for container logging

## CI/CD Integration

Example GitHub Actions workflow:

```yaml
- name: Build Docker image
  run: |
    docker build \
      --build-arg VITE_API_BASE_URL=${{ secrets.API_URL }} \
      -t projectbudget-frontend:${{ github.sha }} \
      ./front-end

- name: Run security scan
  run: |
    docker scan projectbudget-frontend:${{ github.sha }}

- name: Push to registry
  run: |
    docker push projectbudget-frontend:${{ github.sha }}
```

## File Structure

```
front-end/
├── Dockerfile              # Production multi-stage build
├── Dockerfile.dev          # Development build
├── nginx.conf              # Nginx configuration for SPA
├── .dockerignore           # Files to exclude from build
└── DOCKER_BUILD.md         # This documentation
```

## Additional Resources

- [Docker Multi-Stage Builds](https://docs.docker.com/build/building/multi-stage/)
- [Nginx Configuration](https://nginx.org/en/docs/)
- [Vite Build Options](https://vitejs.dev/guide/build.html)
- [Vue Router History Mode](https://router.vuejs.org/guide/essentials/history-mode.html)
