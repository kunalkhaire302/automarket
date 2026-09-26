# AutoMarket — API Specification

## 1. API Purpose

The API is the authoritative application interface between the Next.js frontend, operational services, Supabase PostgreSQL, storage, and AI/ML services.

Base path:

```text
/api/v1
```

## 2. API Standards

- HTTPS only in non-local environments.
- JSON request/response bodies unless multipart upload is explicitly required.
- UTC timestamps in transport payloads.
- Stable machine-readable error codes.
- Correlation/request ID on every response.
- Pagination for collection endpoints.
- Server-side validation for every mutation.
- Explicit authorization on every protected resource.
- DTOs are the API contract; do not expose EF entities directly.

## 3. Authentication Endpoints

```http
POST /api/v1/auth/register
POST /api/v1/auth/login
POST /api/v1/auth/refresh
POST /api/v1/auth/logout
POST /api/v1/auth/forgot-password
POST /api/v1/auth/reset-password
GET  /api/v1/auth/me
```

### Register

Request:

```json
{
  "name": "Kunal",
  "email": "user@example.com",
  "phone": "+91XXXXXXXXXX",
  "password": "strong-password"
}
```

Rules:

- Email normalized before uniqueness check.
- Password never returned.
- Password hash generated server-side.
- Verification workflow triggered if enabled.

## 4. User Endpoints

```http
GET   /api/v1/users/me
PATCH /api/v1/users/me
PATCH /api/v1/users/me/password
GET   /api/v1/users/me/activity
```

## 5. Vehicle Endpoints

```http
GET    /api/v1/cars
GET    /api/v1/cars/{id}
POST   /api/v1/cars
PATCH  /api/v1/cars/{id}
DELETE /api/v1/cars/{id}
POST   /api/v1/cars/{id}/publish
POST   /api/v1/cars/{id}/unpublish
```

### Query parameters

```text
q
brand
model
variant
minPrice
maxPrice
minYear
maxYear
fuelType
transmission
bodyType
minKilometers
maxKilometers
ownershipCount
city
state
status
sort
page
pageSize
```

Example:

```http
GET /api/v1/cars?brand=Honda&fuelType=Petrol&transmission=Automatic&minPrice=500000&maxPrice=1000000&city=Pune&page=1&pageSize=20
```

## 6. Vehicle Image Endpoints

```http
POST   /api/v1/cars/{id}/images
DELETE /api/v1/cars/{id}/images/{imageId}
PATCH  /api/v1/cars/{id}/images/order
```

Recommended flow:

1. Authenticate user.
2. Validate vehicle ownership/administrative access.
3. Validate MIME type and size.
4. Generate server-side storage key.
5. Upload to approved storage.
6. Persist metadata in PostgreSQL.
7. Optionally trigger image moderation/condition inference asynchronously.

## 7. Favourite Endpoints

```http
GET    /api/v1/favourites
POST   /api/v1/favourites/{carId}
DELETE /api/v1/favourites/{carId}
```

## 8. Compare Endpoints

```http
GET    /api/v1/compare
POST   /api/v1/compare/{carId}
DELETE /api/v1/compare/{carId}
```

Enforce a configurable comparison limit on the server.

## 9. Appointment Endpoints

```http
POST   /api/v1/appointments
GET    /api/v1/appointments/me
GET    /api/v1/appointments/{id}
PATCH  /api/v1/appointments/{id}
POST   /api/v1/appointments/{id}/cancel
```

### Appointment creation

Server must validate:

- authenticated user;
- eligible vehicle;
- requested slot availability;
- permitted appointment type;
- location rules;
- conflict with existing active appointment;
- time in permitted booking window.

## 10. Booking Endpoints

```http
POST   /api/v1/bookings
GET    /api/v1/bookings/me
GET    /api/v1/bookings/{id}
PATCH  /api/v1/bookings/{id}
POST   /api/v1/bookings/{id}/cancel
```

Booking creation should be transactional. Where required, lock or otherwise serialize conflicting vehicle/slot state so two requests cannot both create invalid active bookings.

## 11. Seller Endpoints

```http
POST  /api/v1/seller/submissions
GET   /api/v1/seller/submissions
GET   /api/v1/seller/submissions/{id}
PATCH /api/v1/seller/submissions/{id}
POST  /api/v1/seller/submissions/{id}/submit
```

## 12. Notification Endpoints

```http
GET  /api/v1/notifications
POST /api/v1/notifications/{id}/read
POST /api/v1/notifications/read-all
```

## 13. Admin Endpoints

```http
GET   /api/v1/admin/users
GET   /api/v1/admin/cars
GET   /api/v1/admin/sellers
GET   /api/v1/admin/appointments
GET   /api/v1/admin/bookings
GET   /api/v1/admin/analytics
GET   /api/v1/admin/audit-logs
PATCH /api/v1/admin/users/{id}/status
PATCH /api/v1/admin/users/{id}/role
POST  /api/v1/admin/cars/{id}/approve
POST  /api/v1/admin/cars/{id}/reject
```

## 14. AI Endpoints

Public clients normally call the .NET API, not FastAPI directly.

```http
GET  /api/v1/ai/cars/{carId}/recommendation
POST /api/v1/ai/recommendations
POST /api/v1/ai/valuation
POST /api/v1/ai/image-condition-check
POST /api/v1/ai/semantic-search
GET  /api/v1/ai/models/status
```

The backend must enforce authentication, request limits, schema validation, and response policy before exposing AI results.

## 15. Pagination

Recommended request:

```text
?page=1&pageSize=20
```

Recommended response:

```json
{
  "success": true,
  "data": [],
  "meta": {
    "page": 1,
    "pageSize": 20,
    "totalItems": 142,
    "totalPages": 8,
    "hasNext": true
  },
  "requestId": "req_123"
}
```

## 16. Success Envelope

```json
{
  "success": true,
  "data": {},
  "message": "Operation completed successfully",
  "meta": null,
  "requestId": "req_123"
}
```

## 17. Error Envelope

```json
{
  "success": false,
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "One or more fields are invalid",
    "fields": {
      "email": "A valid email is required"
    }
  },
  "requestId": "req_123"
}
```

## 18. Status Code Standards

| Code | Use |
|---|---|
| 200 | Successful read/update |
| 201 | Successful resource creation |
| 204 | Successful no-content operation |
| 400 | Invalid request |
| 401 | Missing/invalid authentication |
| 403 | Authenticated but not authorized |
| 404 | Resource not found or not exposed |
| 409 | Conflict / duplicate / invalid state transition |
| 422 | Semantically invalid input where chosen by API policy |
| 429 | Rate limited |
| 500 | Unexpected server error |
| 503 | Dependency unavailable |

## 19. Idempotency

Use idempotency keys for operations that may be retried and must not create duplicates, particularly:

- booking creation;
- payment initiation if introduced;
- seller submission finalization;
- selected outbound notification jobs.

## 20. API Security

- Validate request body, query, route parameters, and headers.
- Authorize resource ownership.
- Avoid mass-assignment binding.
- Return DTOs.
- Rate-limit authentication and AI endpoints.
- Do not leak whether sensitive resources exist when enumeration is a concern.
- Include audit events for consequential mutations.

## 21. API Versioning

Use URL or header versioning. The initial contract uses `/api/v1`.

Breaking changes require a new version rather than silently changing response semantics.
