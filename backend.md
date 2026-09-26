# AutoMarket — Backend Specification

## 1. Backend Mission

The ASP.NET Core backend is the authoritative business application layer for AutoMarket. It exposes REST APIs, enforces security, applies marketplace rules, persists data to Supabase PostgreSQL, coordinates storage, and integrates with AI services.

## 2. Core Technology

- .NET / ASP.NET Core Web API
- C#
- Entity Framework Core
- Npgsql
- PostgreSQL on Supabase
- JWT-based application authentication
- OpenAPI/Swagger
- FluentValidation or equivalent
- Structured logging
- xUnit/NUnit for tests

## 3. Layered Structure

```text
backend/
├── src/
│   ├── Api/
│   │   ├── Controllers/
│   │   ├── Middleware/
│   │   ├── Filters/
│   │   └── Extensions/
│   ├── Application/
│   │   ├── DTOs/
│   │   ├── Services/
│   │   ├── Interfaces/
│   │   └── Validators/
│   ├── Domain/
│   │   ├── Entities/
│   │   ├── Enums/
│   │   ├── ValueObjects/
│   │   └── Rules/
│   ├── Infrastructure/
│   │   ├── Persistence/
│   │   ├── Repositories/
│   │   ├── Storage/
│   │   ├── AI/
│   │   └── Notifications/
│   └── Common/
│       ├── Errors/
│       ├── Security/
│       └── Observability/
└── tests/
    ├── Unit/
    ├── Integration/
    └── Contract/
```

## 4. Dependency Direction

```text
Api → Application → Domain
Infrastructure → Application/Domain
```

Domain should not depend on infrastructure implementations.

## 5. Controllers

Controllers should:

- authenticate/authorize through framework policy;
- bind DTOs;
- delegate to application services;
- map results to HTTP responses.

Controllers should not contain complex business logic.

## 6. Application Services

Examples:

```text
AuthService
CarService
FavouriteService
CompareService
AppointmentService
BookingService
SellerSubmissionService
NotificationService
AiOrchestrationService
AdminService
```

Each service represents a business capability/use-case boundary.

## 7. Domain Rules

Important rules belong in reusable domain/application logic, including:

- booking eligibility;
- appointment conflict checks;
- listing publication rules;
- seller submission state transitions;
- ownership checks;
- allowable cancellation rules.

## 8. DTOs

Use request/response DTOs.

Do not expose EF entities directly because that can:

- couple API contracts to persistence;
- expose unintended fields;
- create mass-assignment risks.

## 9. Authentication Flow

```text
Register/Login
  ↓
credentials validated
  ↓
password hash verification
  ↓
access token + refresh strategy
  ↓
authenticated request
  ↓
claims parsed
  ↓
authorization policy
```

## 10. Authorization Policies

Examples:

```text
RequireAuthenticatedUser
RequireBuyer
RequireSeller
RequireAdmin
RequireResourceOwner
```

Combine role and ownership checks when necessary.

## 11. PostgreSQL Access

Configure EF Core against Supabase PostgreSQL through Npgsql.

Example conceptual configuration:

```csharp
services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("SupabasePostgres")));
```

The actual production configuration must be adapted to deployment secret management and connection pooling.

## 12. Transaction Strategy

Use explicit EF Core database transactions for multi-step operations.

Example booking transaction:

```text
Begin transaction
 ↓
load car
 ↓
validate car state
 ↓
load/validate appointment
 ↓
check conflicts
 ↓
create booking
 ↓
write audit record
 ↓
commit
```

## 13. Concurrency

Booking and appointment workflows need concurrency protection.

Options include:

- transaction isolation;
- optimistic concurrency tokens;
- unique database constraints;
- short critical sections.

Database constraints are the last line of defense against duplicate/invalid state.

## 14. Validation

Server validation must cover:

- required fields;
- enum values;
- price ranges;
- mileage ranges;
- year ranges;
- slot availability;
- resource ownership;
- state transitions.

## 15. Error Handling

Use centralized exception handling middleware.

Map known domain/application exceptions to controlled responses.

Example internal categories:

```text
ValidationException
UnauthorizedException
ForbiddenException
NotFoundException
ConflictException
ExternalDependencyException
```

Do not leak internal implementation details.

## 16. Logging

Use structured logs.

Recommended fields:

- timestamp;
- level;
- request ID;
- trace ID;
- endpoint;
- status code;
- duration;
- user ID where appropriate;
- operation name;
- exception code.

## 17. Audit Logging

Record critical mutations:

- role changes;
- seller approval/rejection;
- booking creation/cancellation;
- appointment changes;
- listing publication;
- high-impact profile/security events.

## 18. Storage Integration

Use Supabase Storage or approved object storage for large image/document files.

Backend responsibilities:

- authorize upload;
- validate metadata;
- generate storage key;
- persist metadata;
- revoke/delete when business rules require.

## 19. AI Integration

The .NET backend should expose a typed AI client abstraction.

Example:

```text
IAiRecommendationClient
IAiValuationClient
IAiVisionClient
ISemanticSearchClient
```

Do not spread raw HTTP calls to FastAPI throughout controllers.

## 20. AI Reliability

AI client should implement:

- timeouts;
- bounded retries for safe operations;
- response schema validation;
- model-version capture;
- confidence capture;
- fallback behavior.

## 21. Background Jobs

For non-blocking operations consider a background queue for:

- image processing;
- AI inference;
- notifications;
- analytics aggregation;
- recommendation refresh.

The chosen queue implementation is deployment-dependent. Start with a simple hosted-worker approach if scale does not justify Redis-backed queues.

## 22. Caching

Backend cache candidates:

- public car metadata;
- brand/model dictionaries;
- expensive public queries.

Do not cache per-user authorization decisions beyond carefully scoped, short-lived mechanisms.

## 23. Testing

### Unit

Test:

- domain rules;
- validators;
- services;
- state transitions;
- pricing calculations that are deterministic.

### Integration

Test against a disposable/test PostgreSQL environment and/or an isolated Supabase environment.

### Contract

Verify frontend expectations against OpenAPI/API response contracts.

### Security

Test ownership checks and role restrictions explicitly.

## 24. API Documentation

Generate OpenAPI documentation from the API implementation. Keep example payloads and error models aligned with the actual contract.

## 25. Backend Acceptance Criteria

- No core business rule exists only in the frontend.
- Protected resources enforce role and ownership.
- All transactional workflows use appropriate database consistency controls.
- APIs return stable error envelopes.
- AI is integrated through a bounded service abstraction.
- Supabase PostgreSQL is used as the authoritative database.
- Tests cover critical business workflows.
