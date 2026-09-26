# AutoMarket — Deployment Specification

## 1. Deployment Goal

Deploy the application as a production-oriented multi-service system while preserving a simple development workflow.

## 2. Components

```text
Frontend: Next.js
Backend: ASP.NET Core / .NET
Database: Supabase PostgreSQL
Storage: Supabase Storage (optional but recommended)
AI: Python/FastAPI
```

## 3. Recommended Environments

### Development

Local frontend + local .NET API + development Supabase project + optional local AI service.

### Staging

Production-like cloud services using staging-only data.

### Production

Separate cloud resources, credentials, database project, storage buckets, and monitoring configuration.

Never run tests that mutate production data unless explicitly designed as safe production smoke tests.

## 4. Frontend Deployment

Next.js can be deployed to Vercel or an equivalent platform.

Required production variables may include:

```text
NEXT_PUBLIC_API_BASE_URL=https://api.example.com/api/v1
```

Do not place secrets in `NEXT_PUBLIC_*` variables.

Production checks:

- build succeeds;
- environment variables present;
- API base URL correct;
- image domains/configuration valid;
- no development logging/debug UI;
- SEO metadata valid;
- no secret values in generated bundles.

## 5. Backend Deployment

ASP.NET Core may be deployed to a container or managed .NET hosting platform.

Required server-side variables may include:

```text
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__SupabasePostgres=...
Jwt__SigningKey=...
Jwt__Issuer=...
Jwt__Audience=...
AI__BaseUrl=...
```

The connection string and signing secret must be managed through the platform secret store.

## 6. Database Deployment

Supabase project setup:

1. Create production Supabase project.
2. Configure PostgreSQL settings.
3. Create required storage buckets if used.
4. Enable pgvector if semantic search is enabled.
5. Apply approved EF Core migrations.
6. Create required indexes and constraints.
7. Validate backups/recovery configuration.

Migration source of truth:

```text
backend/Migrations
```

Manual dashboard changes must be reconciled into version-controlled migrations.

## 7. Supabase Connection Strategy

The backend uses:

```text
EF Core + Npgsql
```

to access Supabase PostgreSQL.

Use the connection configuration appropriate for the workload, including connection pooling/managed pooling when available and appropriate.

## 8. AI Service Deployment

FastAPI service requirements:

- Python runtime pinned to a supported version;
- dependency lock file;
- model artifact packaging or registry reference;
- health endpoint;
- readiness/liveness checks;
- authenticated internal API communication;
- resource limits;
- model warm-up strategy where inference startup is expensive.

Example:

```text
GET /health
GET /ready
GET /models
POST /recommendations
POST /valuation
```

## 9. Containerization

Recommended Docker services:

```text
frontend/
backend/
ai-service/
```

Each image should:

- use a minimal base image;
- run as a non-root user where practical;
- pin major runtime versions;
- avoid unnecessary build artifacts;
- have a health check where appropriate.

## 10. CI/CD Pipeline

Recommended flow:

```text
Pull Request
   ↓
Lint
   ↓
Type Check / Build
   ↓
Frontend Tests
   ↓
Backend Unit Tests
   ↓
API Integration Tests
   ↓
AI Tests
   ↓
Security / Dependency Scan
   ↓
Build Artifacts
   ↓
Deploy Staging
   ↓
Smoke Tests
   ↓
Manual/controlled approval
   ↓
Production
```

## 11. Database Migration Pipeline

Migration release process:

```text
Schema change
 ↓
EF migration generated
 ↓
Review SQL / migration code
 ↓
CI validation on fresh database
 ↓
Staging migration
 ↓
Integration tests
 ↓
Production migration
```

Avoid destructive migrations without a backward-compatible rollout plan.

## 12. Health Checks

### Frontend

- build/deployment health;
- external API reachability checks where appropriate.

### Backend

```http
GET /health
GET /ready
```

Health checks should not reveal secrets or internal topology.

### AI

```http
GET /health
GET /ready
```

Readiness should verify that model artifacts/configuration required for service startup are available.

## 13. Observability

Track:

- request count;
- latency;
- error rate;
- dependency latency;
- database failures;
- booking errors;
- appointment conflicts;
- AI inference errors;
- model latency;
- deployment health.

Use structured logs with correlation IDs.

## 14. Monitoring and Alerts

Alert on:

- API 5xx spikes;
- database connectivity failures;
- high booking conflict rates;
- authentication abuse;
- AI service downtime;
- model latency degradation;
- storage failures;
- unusual error patterns.

## 15. Backup and Recovery

Use the backup/recovery capabilities of the selected Supabase plan and document:

- backup frequency;
- retention;
- recovery owner;
- restore process;
- restoration test cadence.

Test restoration in staging/non-production periodically.

## 16. Rollback

Frontend rollback: redeploy previous known-good build.

Backend rollback: redeploy previous application artifact.

Database rollback: prefer forward-compatible corrective migrations; use destructive rollback only with a tested recovery plan.

AI rollback: switch model registry deployment pointer to previous approved model version.

## 17. Production Configuration Checklist

- production Supabase project selected;
- production connection string configured;
- JWT secret configured;
- API origin/CORS configured;
- frontend API URL configured;
- storage buckets configured;
- AI internal URL configured;
- rate limits enabled;
- logs/metrics enabled;
- HTTPS enabled;
- error tracking enabled;
- database backups validated;
- migration applied;
- smoke tests passed.

## 18. Domain and HTTPS

Recommended:

```text
www.example.com       → Next.js
api.example.com       → ASP.NET Core API
ai.example.com/private→ FastAPI internal/private network when possible
```

Use HTTPS for every public endpoint.

## 19. Deployment Acceptance Criteria

The release is ready only when:

- frontend build is successful;
- backend build/tests pass;
- AI service health checks pass if enabled;
- Supabase migration succeeds;
- core browse/search/login/appointment/booking flows pass smoke tests;
- security secrets are not exposed;
- logs and monitoring are active;
- rollback procedure is documented.
