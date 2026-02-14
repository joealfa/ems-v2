## Verification
- docker compose --env-file .env.docker build — All 3 images build successfully
- docker compose --env-file .env.docker up -d — All containers start
- https://localhost — Frontend loads (SPA)
- https://localhost/graphql — GraphQL Banana Cake Pop / playground
- https://localhost/api/v1/health or any backend endpoint — Backend responds
- https://localhost/swagger — Swagger UI loads
- WebSocket subscriptions work through wss://localhost/graphql

## Files Created

| File | Purpose |
|------|---------|
| `.dockerignore` | Excludes build artifacts, node_modules, etc. |
| `docker/backend/Dockerfile` | Multi-stage build for Backend API (.NET 10) |
| `docker/gateway/Dockerfile` | Multi-stage build for Gateway (includes local NuGet package) |
| `docker/nginx/Dockerfile` | Multi-stage: builds React frontend + serves via nginx |
| `docker/nginx/nginx.conf` | Reverse proxy with path-based routing + HTTPS |
| `.env.docker` | Template for Docker environment variables (fill in your secrets) |
| `docker-compose.yml` | Orchestrates all 3 services + nginx |

## Path-Based Routing

| URL Path | Routes To |
|----------|-----------|
| `/api/v1/*` | Backend API |
| `/api/persons/*` | Gateway (profile image proxy) |
| `/api/dev/*` | Gateway (dev auth) |
| `/graphql` | Gateway (GraphQL + WebSocket) |
| `/swagger`, `/openapi/*` | Backend API (docs) |
| `/*` | Frontend (SPA static files) |

## How to Use

1. Edit .env.docker — fill in your actual SQL Server connection string, JWT secret, Google OAuth credentials
2. Build: docker compose --env-file .env.docker build
3. Run: docker compose --env-file .env.docker up -d
4. Access: https://localhost (only port 443 exposed)

**Key design choices:**
- Internal services communicate over HTTP — nginx handles all HTTPS termination
- Frontend is baked into the nginx container (no separate container)
- SSL certs from application/certs/ are mounted as a read-only volume
- External services (SQL Server, Redis, RabbitMQ, Seq) accessed via host.docker.internal