# AutoMarket — Architecture Specification

## 1. Purpose

This document defines the target production-oriented architecture for **AutoMarket**, a Cars24-style used-car marketplace built as a real full-stack application. It is derived from `PROJECT_CONTEXT.md` and is subordinate to any newer approved project context.

The system must support vehicle discovery, detailed vehicle views, seller intake, appointments, bookings, accounts, administrative operations, and an AI/ML decision-support layer.

## 2. Architectural Goals

- Keep the frontend, business logic, persistence, and AI layers independently maintainable.
- Make backend authorization the primary security boundary.
- Make Supabase PostgreSQL the authoritative relational datastore.
- Keep AI optional for core transaction flows and provide deterministic fallbacks.
- Support local development, staging, and production environments without changing business logic.
- Provide observable, testable, versioned interfaces.
- Avoid coupling UI components directly to persistence concerns.

## 3. High-Level Architecture

```text
                         ┌──────────────────────────────┐
                         │           Users              │
                         │ Desktop / Tablet / Mobile   │
                         └──────────────┬───────────────┘
                                        │ HTTPS
                                        ▼
                         ┌──────────────────────────────┐
                         │ Next.js + TypeScript         │
                         │ UI / Routing / SSR / Forms   │
                         └──────────────┬───────────────┘
                                        │ REST/HTTPS
                                        ▼
                    ┌────────────────────────────────────────┐
                    │ ASP.NET Core Web API / .NET + C#        │
                    │ Auth | Authorization | Business Rules  │
                    │ Validation | API orchestration         │
                    └───────────┬───────────────┬────────────┘
                                │               │
                     EF Core + Npgsql       Internal HTTPS
                                │               │
                                ▼               ▼
                   ┌───────────────────┐  ┌──────────────────┐
                   │ Supabase           │  │ Python/FastAPI   │
                   │ PostgreSQL        │  │ AI/ML Service     │
                   │ + optional pgvector│  │ Model Inference   │
                   └─────────┬─────────┘  └────────┬─────────┘
                             │                      │
                             ▼                      ▼
                   ┌──────────────────┐    ┌──────────────────┐
                   │ Supabase Storage │    │ Model Registry   │
                   │ images/docs      │    │ evaluation/store │
                   └──────────────────┘    └──────────────────┘
```

## 4. Logical Components

### 4.1 Frontend — Next.js

Responsibilities:

- Page routing and layouts.
- Server-side rendering and SEO where appropriate.
- Client-side interactions.
- Form composition and client validation.
- Search/filter experience.
- Authentication-aware rendering.
- Loading, empty, error, retry, and success states.
- Presentation of AI outputs with confidence and limitations.

The frontend must not contain authoritative business rules for bookings, appointment conflicts, permissions, prices, or user ownership.

### 4.2 Backend — ASP.NET Core

Responsibilities:

- Authentication and authorization.
- REST API exposure.
- Request validation.
- Domain/business rules.
- Transaction coordination.
- Database access.
- File/storage orchestration.
- AI-service orchestration.
- Notifications and audit logging.
- Rate limiting and security policy enforcement.

### 4.3 Database — Supabase PostgreSQL

Supabase PostgreSQL is the **single source of truth** for marketplace business data.

Use EF Core + Npgsql for application access. The browser must never connect directly to PostgreSQL.

Optional capabilities:

- `pgvector` for embeddings.
- Supabase Storage for vehicle images and user-submitted documents.
- Supabase Realtime for explicitly approved realtime features.

### 4.4 AI/ML Service

Python/FastAPI service responsible for:

- Vehicle recommendations.
- Semantic search.
- Price valuation.
- Image condition assistance.
- Personalization.
- Similar-car retrieval.
- Demand forecasting.

The .NET API remains the public orchestration boundary.

## 5. Deployment Topology

Recommended:

```text
Vercel / equivalent
    └── Next.js frontend

Cloud host / container platform
    └── ASP.NET Core API

Cloud host / container platform
    └── FastAPI AI service

Supabase
    ├── PostgreSQL
    ├── optional pgvector
    └── optional Storage
```

Exact vendors can vary, but the interfaces and security boundaries remain stable.

## 6. Request Lifecycle

```text
Browser
  ↓
HTTPS
  ↓
Next.js
  ↓
API client
  ↓
ASP.NET Core middleware
  ↓
Authentication
  ↓
Authorization
  ↓
Validation
  ↓
Controller
  ↓
Application Service
  ↓
Domain rules
  ↓
EF Core transaction
  ↓
Supabase PostgreSQL
  ↓
DTO mapping
  ↓
HTTP response
  ↓
UI state update
```

For AI:

```text
Client request
  ↓
.NET auth + validation
  ↓
AI orchestration service
  ↓
FastAPI model endpoint
  ↓
model inference
  ↓
confidence + metadata
  ↓
policy/guardrail validation
  ↓
persist AI result if required
  ↓
client response
```

## 7. Architectural Boundaries

### Browser boundary

Never expose:

- Database credentials.
- Supabase service-role key.
- JWT signing secret.
- AI service credentials.
- Internal API keys.

### Backend boundary

All consequential operations pass through authorization and deterministic business rules.

### AI boundary

AI is advisory unless an explicit product rule says otherwise. AI cannot bypass authorization, availability, financial, or state-transition rules.

## 8. Scalability Strategy

Start with a modular monolith for the .NET API. Split services only when load, team structure, or operational requirements justify it.

Scale independently:

- Next.js horizontally through the hosting platform.
- ASP.NET Core API horizontally behind a load balancer.
- AI service horizontally for inference throughput.
- PostgreSQL through appropriate Supabase capacity and query optimization.
- Storage independently from database rows.

## 9. Resilience

Required resilience patterns:

- Request timeouts.
- Retries only for safe/idempotent downstream operations.
- Circuit breaking around optional AI integrations where useful.
- Idempotency for booking/submission creation where duplicate requests are possible.
- Graceful AI degradation.
- Database transaction boundaries for multi-write workflows.

## 10. Caching

Candidate cacheable resources:

- Public car catalog queries with short TTLs.
- Brand/model metadata.
- Popular search facets.
- Read-heavy public vehicle details.

Do not cache mutable user-private data without correct ownership scoping.

## 11. Architecture Acceptance Criteria

- Frontend cannot access PostgreSQL directly.
- All protected actions require server-side authorization.
- Supabase PostgreSQL is the authoritative source for transactional state.
- AI failures do not prevent basic browsing, appointments, or booking flows from functioning where deterministic data is sufficient.
- APIs are documented and versioned.
- Database transactions protect booking and appointment consistency.
- Every major component can be tested independently.
