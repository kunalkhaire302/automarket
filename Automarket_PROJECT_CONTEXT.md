# Cars24-Style Real Car Marketplace — PROJECT_CONTEXT.md

> **Context Version:** 1.1.0  
> **Created:** 2026-09-25  
> **Status:** Development / Production-oriented academic project baseline  
> **Project Type:** Full-stack used-car marketplace and vehicle transaction workflow platform  
> **Primary Frontend:** Next.js + TypeScript  
> **Primary Backend:** ASP.NET Core / .NET + C#  
> **Primary Database:** Supabase PostgreSQL (managed PostgreSQL)  
> **Reference Product:** Cars24-style marketplace workflow; use original branding, content, imagery, copy, and UI assets.

---

## 1. Purpose of This Project Context

This file is the **single source of truth for the project implementation context**. Any developer, AI coding agent, reviewer, tester, or maintainer working on the project should read this document before making architectural, feature, data-model, API, UI/UX, AI/ML, security, or deployment changes.

The project is not intended to be a simple static clone. It is a **real full-stack automobile marketplace** that demonstrates a complete digital journey for discovering cars, viewing vehicle information, selling a vehicle, scheduling appointments, creating bookings, authenticating users, managing profiles, and persisting business data through backend services.

The project is inspired by the functional patterns of modern used-car marketplaces, including Cars24, but must maintain its **own product identity and implementation**.

### Core principle

> **Build the complete product workflow, not only the UI.**

Every important screen should be connected to a realistic data model, API contract, validation layer, business rule, loading state, empty state, error state, security boundary, and test case where applicable.

---

# 2. Project Identity

## 2.1 Working Project Name

**AutoMarket — Cars24-Style Real Car Marketplace**

The project may use another final brand name. The functional scope in this context remains authoritative unless a newer project context is explicitly supplied.

## 2.2 Product Category

- Used-car marketplace
- Vehicle buying platform
- Vehicle selling platform
- Appointment scheduling system
- Car booking workflow
- User account and profile management
- Full-stack marketplace demonstration
- AI-assisted vehicle discovery and decision support extension

## 2.3 Product Vision

Create a modern, trusted, responsive marketplace where a user can move from **car discovery → vehicle evaluation → appointment → booking → account management** without leaving the platform, while sellers can move from **vehicle submission → valuation → inspection appointment → listing lifecycle** through a structured workflow.

## 2.4 Product Mission

Reduce the friction involved in finding, evaluating, selling, and booking used cars by bringing vehicle discovery, seller intake, appointments, transaction workflows, and intelligent assistance into one coherent digital platform.

---

# 3. Learning Context and Course Scope

The project is based on the following learning modules and must demonstrate practical application of the concepts taught in them.

## 3.1 Introduction and Basics — 6/6 Completed

| Module | Duration / Status |
|---|---|
| HTML Basics | 7:55 min — Completed |
| CSS: Styling with Elegance — Part 1 | 88 min — Completed |
| JavaScript Basics | 17:43 min — Completed |
| CSS: Styling with Elegance — Part 2 | 23 min — Completed |
| Basics of .NET (C# / C-Sharp) | 7:10 min — Completed |
| Learn Next.js with TypeScript | 7:48 min — Completed |

## 3.2 Creating Real-Time Cars24 Project — 21/21 Completed

| Module | Duration / Status |
|---|---|
| Setting Up Project | 5 min — Completed |
| Creating Header and Footer | 17:19 min — Completed |
| Home Page | 7:20 min — Completed |
| Buy Car Page | 19:57 min — Completed |
| Car Detail Page | 29:49 min — Completed |
| Sell Car Page — Part 1 | 36:22 min — Completed |
| Sell Car Page — Part 2 | 16:26 min — Completed |
| Book Appointment Page | 8:12 min — Completed |
| Booking Page | 17:14 min — Completed |
| Profile Page | 5:31 min — Completed |
| Login / Signup Page | 11:47 min — Completed |
| Backend Setup | 12:13 min — Completed |
| User Model / Service | 12:09 min — Completed |
| Adding Frontend Car API Request | 20:04 min — Completed |
| Creating User API and Frontend Integration | 34:32 min — Completed |
| Car Model / Service | 20:32 min — Completed |
| Appointment Model / Service | 26:28 min — Completed |
| Booking Model and Service | 6:28 min — Completed |
| Frontend Appointment API Integration | 18:06 min — Completed |
| Frontend Booking API Integration and Booking Model Changes | 8:20 min — Completed |
| Deployment | Completed |

### Course-to-product expectation

The course modules provide the foundation. The project context extends that foundation into a maintainable, production-oriented architecture with explicit contracts, security, testing, AI/ML capabilities, observability, and deployment discipline.

---

# 4. Problem Statement

Used-car discovery and selling often require users to combine search portals, phone calls, messaging, physical inspection scheduling, and manual paperwork. Vehicle data may be inconsistent, decisions may be difficult for inexperienced buyers, and seller workflows can be fragmented.

The platform addresses these issues through a unified application that supports:

1. Vehicle discovery and search.
2. Structured vehicle specifications.
3. Advanced filtering and sorting.
4. Vehicle detail and media presentation.
5. User registration and authentication.
6. Seller vehicle submission.
7. Appointment scheduling.
8. Vehicle booking workflow.
9. Profile and account activity.
10. Administrative and operational controls.
11. AI-assisted discovery, recommendation, valuation, and decision support.
12. Auditable backend APIs and business rules.

---

# 5. Target Users and Roles

## 5.1 Guest

A visitor who has not authenticated.

### Allowed capabilities

- Browse home page.
- Browse public car listings.
- Search and filter cars.
- Open public car details.
- View public marketplace information.
- Start login/signup.

### Restricted capabilities

- Submit a car for sale.
- Create a booking.
- Create a protected appointment.
- Access personal profile.
- Access private recommendations based on account history.

## 5.2 Buyer

A registered user looking for a vehicle.

### Capabilities

- Search and filter cars.
- View vehicle details.
- Save/favourite cars.
- Compare vehicles.
- Create appointments.
- Create and manage bookings.
- View booking history.
- Receive notifications.
- Manage profile.
- Use AI-assisted recommendations and explanations.

## 5.3 Seller

A registered user submitting one or more vehicles for sale.

### Capabilities

- Create seller profile.
- Submit vehicle information.
- Upload vehicle images.
- Enter expected selling price.
- Request valuation.
- Schedule inspection appointment.
- Track submission status.
- View seller activity.

## 5.4 Administrator

Operational platform role for the production extension.

### Capabilities

- Manage users.
- Manage listings.
- Approve/reject seller submissions.
- Manage appointments.
- Manage bookings.
- Manage marketplace metadata.
- Review reports and analytics.
- Review AI model outputs and confidence.
- Access audit logs.
- Manage system settings.

### Authorization rule

Role-based authorization must be enforced **server-side**. The frontend may hide controls for convenience but must never be treated as the security boundary.

---

# 6. Product Scope

## 6.1 Core MVP

The MVP must contain:

- Responsive landing page.
- Header and footer.
- Buy-car listing page.
- Car detail page.
- Search/filter/sort.
- Sell-car workflow.
- Appointment workflow.
- Booking workflow.
- Login/signup.
- User profile.
- Backend API.
- Database persistence.
- Authentication.
- Validation.
- Error handling.
- Deployment.

## 6.2 Production-Oriented Extensions

The following are recommended after the baseline course implementation:

- Favourite/saved cars.
- Compare cars.
- Seller dashboard.
- Admin dashboard.
- Vehicle inspection records.
- Image/object storage.
- Notifications.
- Email/SMS integration.
- Search indexing.
- Location support.
- Payments where appropriate.
- Audit logs.
- Analytics.
- AI/ML service.
- Automated CI/CD.
- Observability.
- Rate limiting.
- Feature flags.

---

# 7. Functional Modules

## 7.1 Authentication

### Features

- Signup.
- Login.
- Logout.
- Password hashing.
- JWT-based authentication.
- Refresh-token strategy for production.
- Password reset.
- Email verification.
- Role-based access control.
- Session expiration.
- Security event logging.

### Routes

```text
/login
/signup
/forgot-password
/reset-password
/verify-email
```

## 7.2 Home

### Sections

- Global header.
- Hero section.
- Search experience.
- Buy CTA.
- Sell CTA.
- Featured vehicles.
- Popular brands.
- Budget/category shortcuts.
- How it works.
- Trust indicators.
- FAQ.
- Footer.

### Route

```text
/
```

## 7.3 Buy Cars

### Required features

- Listing grid/list.
- Search.
- Filters.
- Sort.
- Pagination or infinite scroll.
- Price range.
- Brand/model filters.
- Fuel type.
- Transmission.
- Year.
- Kilometres.
- City/location.
- Vehicle body type.
- Ownership count.
- Availability status.
- Favourite action.
- Compare action.

### Route

```text
/buy
```

## 7.4 Car Details

### Required information

- Gallery.
- Primary image.
- Price.
- Make.
- Model.
- Variant.
- Registration year.
- Manufacturing year.
- Kilometres driven.
- Fuel type.
- Transmission.
- Ownership.
- Location.
- Registration state.
- Insurance status if available.
- Service information if available.
- Description.
- Key features.
- Inspection details.
- Similar cars.
- AI recommendation/explanation section.
- Appointment CTA.
- Booking CTA.

### Route

```text
/buy/[carId]
```

## 7.5 Sell Car

A multi-step form is preferred over a single large form.

### Step 1 — Vehicle basics

- Brand.
- Model.
- Variant.
- Registration year.
- Fuel type.
- Transmission.

### Step 2 — Vehicle usage

- Kilometres driven.
- Ownership count.
- Registration state.
- City.
- Service history availability.

### Step 3 — Condition

- Overall condition.
- Exterior condition.
- Interior condition.
- Tyre condition.
- Accident history.
- Known issues.
- Repair history.

### Step 4 — Media

- Vehicle images.
- Front.
- Rear.
- Left.
- Right.
- Interior.
- Odometer.
- Tyres.
- Damage areas where applicable.

### Step 5 — Price and contact

- Expected price.
- Preferred contact method.
- Preferred appointment slot.
- Seller notes.

### Step 6 — Review and submit

- Summary.
- Terms acknowledgement.
- Submission.
- Confirmation.

### Route

```text
/sell
```

## 7.6 Appointment

Appointments are used for vehicle inspection, viewing, evaluation, test-drive coordination, or seller intake depending on the workflow.

### Fields

- User.
- Car.
- Appointment type.
- Date.
- Time slot.
- Location.
- Notes.
- Status.
- Created timestamp.
- Updated timestamp.

### Appointment statuses

```text
PENDING
CONFIRMED
RESCHEDULED
COMPLETED
CANCELLED
NO_SHOW
```

### Route

```text
/appointment
```

## 7.7 Booking

### Features

- Select vehicle.
- Confirm customer details.
- Select appointment.
- Review booking summary.
- Submit booking.
- Show confirmation.
- Show booking status.
- Cancel or reschedule when allowed.

### Booking statuses

```text
PENDING
CONFIRMED
PAYMENT_PENDING
PAID
CANCELLED
COMPLETED
EXPIRED
```

### Route

```text
/booking
```

## 7.8 Profile

### Sections

- Personal details.
- Contact details.
- Account security.
- Saved cars.
- Compared cars.
- Appointment history.
- Booking history.
- Seller submissions.
- Notifications.
- Preferences.

### Route

```text
/profile
```

## 7.9 Admin

Suggested routes:

```text
/admin
/admin/users
/admin/cars
/admin/sellers
/admin/appointments
/admin/bookings
/admin/analytics
/admin/ai
/admin/audit-logs
/admin/settings
```

---

# 8. End-to-End Workflows

## 8.1 Buyer Journey

```text
Landing Page
    ↓
Search / Buy Cars
    ↓
Apply Filters
    ↓
Open Car Details
    ↓
Review Vehicle
    ↓
Compare / Favourite (optional)
    ↓
AI Recommendation / Decision Support
    ↓
Select Appointment
    ↓
Confirm Booking
    ↓
Booking Created
    ↓
Notification / Confirmation
    ↓
Profile → My Bookings
```

## 8.2 Seller Journey

```text
Landing Page
    ↓
Sell Car
    ↓
Vehicle Information
    ↓
Condition + Usage
    ↓
Upload Images
    ↓
AI Valuation / Quality Assistance
    ↓
Expected Price
    ↓
Choose Inspection Slot
    ↓
Review Submission
    ↓
Submit
    ↓
Seller Submission Created
    ↓
Admin Review / Operational Processing
    ↓
Approved / Rejected / More Information Required
```

## 8.3 Authentication Journey

```text
Signup
  ↓
Validate request
  ↓
Hash password
  ↓
Create user
  ↓
Issue auth credentials
  ↓
Authenticated frontend session
  ↓
Protected API access
```

---

# 9. Technical Architecture

## 9.1 Baseline Architecture

```text
                           ┌─────────────────────────┐
                           │       User Browser      │
                           │ Desktop / Tablet / PWA  │
                           └────────────┬────────────┘
                                        │ HTTPS
                                        ▼
                    ┌───────────────────────────────────┐
                    │      Next.js + TypeScript          │
                    │ UI / SSR / RSC / Routing / Forms  │
                    └────────────────┬──────────────────┘
                                     │ REST / HTTPS
                                     ▼
                    ┌───────────────────────────────────┐
                    │      ASP.NET Core Web API          │
                    │ Controllers / Services / Auth      │
                    └───────────────┬───────────────────┘
                                    │
                   ┌────────────────┼─────────────────┐
                   │                │                 │
                   ▼                ▼                 ▼
             ┌──────────┐    ┌────────────┐    ┌─────────────┐
             │ Supabase  │    │ Object     │    │ AI/ML       │
             │PostgreSQL│    │ Storage    │    │ Service     │
             └──────────┘    └────────────┘    └──────┬──────┘
                                                       │
                                              ┌────────┴────────┐
                                              │ Model Registry  │
                                              │ + Feature Store │
                                              └─────────────────┘
```

## 9.2 Architectural Principles

1. Separation of concerns.
2. API-first data contracts.
3. Server-side authorization.
4. Centralized validation.
5. Immutable audit events for critical actions.
6. Idempotent operations where duplicate requests are possible.
7. Observable services.
8. Explicit model versioning for AI features.
9. Graceful degradation when AI services are unavailable.
10. No business-critical workflow may depend on an untrusted AI output without validation and defined fallback behavior.

---

# 10. Technology Stack

## 10.1 Frontend

| Technology | Role |
|---|---|
| Next.js | Application framework and routing |
| React | Component architecture |
| TypeScript | Static typing |
| HTML5 | Semantic structure |
| CSS3 | Styling |
| Tailwind CSS or CSS Modules | UI implementation |
| React Hook Form | Complex forms |
| Zod | Client-side schema validation |
| TanStack Query | Server-state management |
| Axios or Fetch | API communication |
| Zustand or Context | Lightweight global state where needed |
| Recharts / equivalent | Dashboard analytics |

## 10.2 Backend

| Technology | Role |
|---|---|
| C# | Backend language |
| ASP.NET Core / .NET | REST API |
| Entity Framework Core + Npgsql | ORM and PostgreSQL database access |
| FluentValidation or equivalent | Request validation |
| JWT | Access token authentication |
| BCrypt / PBKDF2 / ASP.NET Identity | Password hashing strategy |
| Serilog or equivalent | Structured logging |
| Swagger / OpenAPI | API contract/documentation |
| xUnit / NUnit | Automated testing |

## 10.3 Database

### Mandatory Database Platform

**Supabase PostgreSQL** is the primary and authoritative database for the project. Supabase is used as the managed PostgreSQL platform; the application must treat PostgreSQL as the underlying relational database engine.

### Database Access Pattern

```text
Next.js
   ↓ HTTPS
ASP.NET Core API
   ↓
Application Services
   ↓
EF Core + Npgsql
   ↓
Supabase PostgreSQL
```

The frontend must not connect directly to the database. All business-critical writes, authorization checks, booking transitions, appointment validation, seller workflows, and AI-result persistence must pass through the ASP.NET Core API.

### Supabase Responsibilities

- Managed PostgreSQL database hosting.
- Production backups / recovery features available through the selected Supabase plan.
- SQL database administration and monitoring through Supabase tooling.
- Optional Row Level Security (RLS) as a defense-in-depth control.
- Optional Supabase Storage for vehicle images and documents.
- Optional `pgvector` support for AI embeddings and semantic retrieval.
- Optional Supabase Realtime for narrowly scoped realtime use cases if the project later requires it.

### Authentication Boundary

The project retains **ASP.NET Core authentication/authorization as the application security boundary** unless a future approved context explicitly migrates authentication to Supabase Auth. Merely using Supabase PostgreSQL does not imply that Supabase Auth is enabled.

### Migration Source of Truth

Use **Entity Framework Core migrations with Npgsql** as the version-controlled application schema migration mechanism. Avoid undocumented manual production schema changes in the Supabase dashboard. Any emergency manual change must be reconciled into a migration immediately.

### PostgreSQL Standards

- Prefer `uuid` primary keys for distributed-safe identifiers.
- Use `timestamptz` for event timestamps and cross-time-zone scheduling data.
- Use `numeric` for financial values such as vehicle price, booking amount, and valuation outputs.
- Use foreign keys for relationship integrity.
- Add unique constraints for business identifiers such as normalized user email where applicable.
- Add targeted indexes for high-frequency filters including make, model, city, fuel type, transmission, price, year, mileage, status, and created date.
- Use `jsonb` only where the data is genuinely semi-structured; do not use it as a replacement for relational design.
- Use PostgreSQL transactions for multi-step booking, appointment, and seller workflow operations.
- Do not store secrets, raw passwords, or sensitive tokens in ordinary business tables.

### Environment Separation

Create separate Supabase projects/environments for development, staging, and production where practical. Never point local development or test automation at production data.

### Connection Management

- Store the Supabase PostgreSQL connection string only in backend secret configuration.
- Use the connection/pooling option appropriate for the deployed workload.
- Configure EF Core connection resiliency and bounded connection-pool settings for the deployment environment.
- Never expose the database connection string in Next.js public environment variables.

Recommended database configuration variable:

```text
ConnectionStrings__SupabasePostgres=
```

### Supabase Database Acceptance Criteria

- All core entities persist successfully in Supabase PostgreSQL.
- EF Core migrations apply cleanly to a fresh Supabase project.
- Foreign-key and unique constraints are enforced.
- Appointment and booking transactions prevent invalid state transitions.
- Backup/recovery expectations are documented for the selected Supabase plan.
- Production credentials are stored outside source control.

### Legacy SQL Server Rule

SQL Server is **not** part of the approved production database architecture for this project. Do not introduce SQL Server-specific types, queries, connection strings, or deployment dependencies unless a future project context explicitly changes the database platform.


## 10.4 AI/ML Service

Recommended separation:

- Python.
- FastAPI.
- pandas.
- scikit-learn.
- XGBoost.
- sentence-transformers.
- spaCy where NLP preprocessing is required.
- Pillow/OpenCV for image preprocessing.
- ONNX Runtime where model portability/inference efficiency is useful.

The AI layer should be independently deployable and should communicate with the .NET backend through authenticated internal APIs.

---

# 11. Repository Structure

Recommended monorepo layout:

```text
automarket/
├── PROJECT_CONTEXT.md
├── README.md
├── LICENSE
├── docs/
│   ├── architecture.md
│   ├── api.md
│   ├── database.md
│   ├── ai-models.md
│   ├── security.md
│   └── deployment.md
├── frontend/
│   ├── app/
│   ├── components/
│   ├── features/
│   ├── lib/
│   ├── hooks/
│   ├── services/
│   ├── schemas/
│   ├── stores/
│   ├── types/
│   └── tests/
├── backend/
│   ├── Migrations/
│   ├── src/
│   │   ├── Api/
│   │   ├── Application/
│   │   ├── Domain/
│   │   ├── Infrastructure/
│   │   └── Common/
│   └── tests/
├── ai-service/
│   ├── app/
│   │   ├── api/
│   │   ├── models/
│   │   ├── services/
│   │   ├── pipelines/
│   │   ├── schemas/
│   │   └── utils/
│   ├── models/
│   ├── tests/
│   └── requirements.txt
├── infra/
│   ├── docker/
│   ├── nginx/
│   └── deployment/
└── scripts/
```

---

# 12. Frontend Architecture

## 12.1 Next.js Responsibilities

- Route rendering.
- Page composition.
- SEO metadata.
- Server-side rendering where useful.
- Client interactions.
- Form handling.
- API integration.
- Loading/error/empty states.
- Authentication-aware UI.
- Responsive layout.

## 12.2 Frontend Rules

- Prefer server components where no client interactivity is required.
- Use client components only where state, browser APIs, or user interaction require them.
- Keep API logic out of visual components.
- Centralize API clients.
- Centralize schema validation.
- Do not duplicate business calculations across components.
- Avoid hardcoded marketplace data once API integration exists.
- Every data-dependent view needs loading, error, empty, and success states.

## 12.3 Suggested Feature Structure

```text
features/
├── auth/
├── cars/
├── search/
├── favourites/
├── compare/
├── sell/
├── appointments/
├── bookings/
├── profile/
├── notifications/
├── admin/
└── ai/
```

---

# 13. Backend Architecture

Use a layered architecture:

```text
HTTP Request
   ↓
Controller
   ↓
Application Service
   ↓
Domain / Business Rules
   ↓
Repository / EF Core + Npgsql
   ↓
Supabase PostgreSQL
```

## 13.1 Layer Responsibilities

### API

- Routing.
- Authentication middleware.
- Authorization.
- Request/response DTOs.
- HTTP status codes.
- API versioning.

### Application

- Use cases.
- Transaction coordination.
- DTO mapping.
- Business orchestration.

### Domain

- Business entities.
- Value objects.
- Domain rules.
- Status transitions.

### Infrastructure

- EF Core.
- Database configuration.
- Repositories.
- External services.
- AI client.
- Storage integration.

---

# 14. Database Context

## 14.1 Core Entities

The production-oriented baseline should include at least:

```text
User
Role
Car
CarImage
CarFeature
CarInspection
SellerSubmission
Appointment
Booking
Favourite
ComparisonList
Notification
AuditLog
AiRecommendation
AiPrediction
SearchEvent
```

## 14.2 User

| Field | Type | Rules |
|---|---|---|
| Id | GUID/UUID | Primary key |
| Name | string | Required |
| Email | string | Required, unique |
| Phone | string | Required where business flow requires |
| PasswordHash | string | Never store plaintext password |
| Role | enum | Buyer/Seller/Admin |
| IsEmailVerified | bool | Default false |
| IsActive | bool | Default true |
| CreatedAt | datetime | Required |
| UpdatedAt | datetime | Required |

## 14.3 Car

| Field | Type | Rules |
|---|---|---|
| Id | GUID/UUID | Primary key |
| SellerId | FK | Optional for platform inventory, required for user-submitted cars |
| Brand | string | Required |
| Model | string | Required |
| Variant | string | Optional/required depending on listing type |
| ManufacturingYear | int | Valid range |
| RegistrationYear | int | Valid range |
| FuelType | enum | Petrol/Diesel/CNG/EV/Hybrid/Other |
| Transmission | enum | Manual/Automatic/AMT/CVT/DCT/Other |
| Kilometers | int | Non-negative |
| Price | decimal | Positive |
| City | string | Required |
| State | string | Optional/required based on market |
| OwnershipCount | int | Non-negative |
| BodyType | enum | Hatchback/Sedan/SUV/MUV/Coupe/Other |
| Status | enum | Draft/Pending/Listed/Booked/Sold/Rejected/Inactive |
| Description | text | Sanitized |
| CreatedAt | datetime | Required |
| UpdatedAt | datetime | Required |

## 14.4 CarImage

- Id.
- CarId.
- StorageKey.
- Public/secure URL.
- ImageType.
- SortOrder.
- Width.
- Height.
- ModerationStatus.
- CreatedAt.

## 14.5 Appointment

- Id.
- UserId.
- CarId.
- AppointmentType.
- AppointmentDate.
- StartTime.
- EndTime.
- Location.
- Status.
- Notes.
- CreatedAt.
- UpdatedAt.

## 14.6 Booking

- Id.
- UserId.
- CarId.
- AppointmentId.
- BookingReference.
- BookingStatus.
- Amount.
- PaymentStatus if payment is enabled.
- CreatedAt.
- UpdatedAt.

## 14.7 SellerSubmission

- Id.
- SellerId.
- CarId.
- SubmissionStatus.
- ExpectedPrice.
- ValuationId.
- ReviewNotes.
- SubmittedAt.
- ReviewedAt.
- ReviewedBy.

## 14.8 AuditLog

Critical mutation events should be logged.

Fields:

- Id.
- ActorUserId.
- Action.
- EntityType.
- EntityId.
- PreviousValue snapshot where justified.
- NewValue snapshot where justified.
- IP metadata where legally and operationally appropriate.
- Timestamp.

---

# 15. Entity Relationships

```text
User
 ├──< Cars
 ├──< Appointments
 ├──< Bookings
 ├──< Favourites
 ├──< SellerSubmissions
 ├──< Notifications
 ├──< AuditLogs
 └──< SearchEvents

Car
 ├──< CarImages
 ├──< CarFeatures
 ├──< CarInspections
 ├──< Appointments
 ├──< Bookings
 └──< AiPredictions / AiRecommendations

Appointment
 └──< Booking (typically one active booking association per business workflow)

SellerSubmission
 └── Car
```

---

# 16. REST API Contract

Base URL example:

```text
/api/v1
```

## 16.1 Authentication

```http
POST /auth/register
POST /auth/login
POST /auth/refresh
POST /auth/logout
POST /auth/forgot-password
POST /auth/reset-password
GET  /auth/me
```

## 16.2 Users

```http
GET   /users/me
PATCH /users/me
PATCH /users/me/password
GET   /users/me/activity
```

Admin:

```http
GET    /admin/users
GET    /admin/users/{id}
PATCH  /admin/users/{id}/status
PATCH  /admin/users/{id}/role
```

## 16.3 Cars

```http
GET    /cars
GET    /cars/{id}
POST   /cars
PATCH  /cars/{id}
DELETE /cars/{id}
POST   /cars/{id}/publish
POST   /cars/{id}/unpublish
```

### Filtering example

```http
GET /cars?brand=Honda&fuelType=Petrol&transmission=Automatic&minPrice=500000&maxPrice=1000000&city=Pune&page=1&pageSize=20
```

## 16.4 Images

```http
POST   /cars/{id}/images
DELETE /cars/{id}/images/{imageId}
PATCH  /cars/{id}/images/order
```

## 16.5 Appointments

```http
POST   /appointments
GET    /appointments/me
GET    /appointments/{id}
PATCH  /appointments/{id}
POST   /appointments/{id}/cancel
```

## 16.6 Bookings

```http
POST   /bookings
GET    /bookings/me
GET    /bookings/{id}
PATCH  /bookings/{id}
POST   /bookings/{id}/cancel
```

## 16.7 Favourites

```http
GET    /favourites
POST   /favourites/{carId}
DELETE /favourites/{carId}
```

## 16.8 Compare

```http
GET    /compare
POST   /compare/{carId}
DELETE /compare/{carId}
```

## 16.9 Seller

```http
POST /seller/submissions
GET  /seller/submissions
GET  /seller/submissions/{id}
PATCH /seller/submissions/{id}
POST /seller/submissions/{id}/submit
```

## 16.10 AI

```http
GET  /ai/cars/{carId}/recommendation
POST /ai/recommendations
POST /ai/valuation
POST /ai/image-condition-check
POST /ai/search/semantic
GET  /ai/models/status
```

The frontend should normally call the .NET backend, not the AI service directly. The backend acts as the authenticated orchestration boundary.

---

# 17. API Response Standards

## Success response

```json
{
  "success": true,
  "data": {},
  "message": "Operation completed successfully",
  "meta": null,
  "requestId": "..."
}
```

## Error response

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
  "requestId": "..."
}
```

## Required API standards

- Consistent status codes.
- Stable error codes.
- Request correlation ID.
- Pagination metadata.
- Validation errors by field.
- No secret values in responses.
- No stack traces in production responses.

---

# 18. Frontend Data and State Management

## UI state

Use local component state for:

- Modal visibility.
- Drawer state.
- Temporary form state.
- Visual toggles.

## Server state

Use TanStack Query or equivalent for:

- Cars.
- User profile.
- Appointments.
- Bookings.
- Seller submissions.
- AI responses.

## Authentication state

Authentication state should be persisted securely according to the chosen auth model. Prefer secure, HttpOnly cookies for long-lived production sessions where architecture permits. Avoid storing sensitive long-lived credentials in insecure browser storage.

---

# 19. Search and Discovery System

## 19.1 Baseline search

Support:

- Keyword matching.
- Brand.
- Model.
- City.
- Price range.
- Year range.
- Fuel.
- Transmission.
- Body type.
- Kilometres.

## 19.2 Ranking signals

A future ranking layer may consider:

- Text relevance.
- Availability.
- User-selected filters.
- Recency.
- Price fit.
- Distance.
- Engagement.
- Personal preferences.

Ranking must be explainable enough to debug and audit.

---

# 20. Full AI / ML Model Layer

> This section interprets the requested “full AI model” as the complete AI/ML system around the marketplace, including model training, inference, evaluation, monitoring, and AI alignment/guardrails.

The AI layer is an **extension of the marketplace**, not a replacement for deterministic business logic.

## 20.1 AI Objectives

The AI system should help with:

1. Car recommendations.
2. Semantic search.
3. Used-car price estimation.
4. Vehicle condition assistance from images.
5. Seller lead/quality scoring.
6. Personalization.
7. Similar-car retrieval.
8. Marketplace analytics and forecasting.
9. Natural-language car discovery.
10. Explainable decision support.

## 20.2 AI Architecture

```text
                         ┌─────────────────────────┐
                         │      Next.js UI         │
                         └────────────┬────────────┘
                                      │
                                      ▼
                         ┌─────────────────────────┐
                         │    ASP.NET Core API     │
                         │ Auth + Policy + Rules   │
                         └────────────┬────────────┘
                                      │ Internal API
                                      ▼
                  ┌────────────────────────────────────┐
                  │         AI / ML Service             │
                  │ FastAPI + Python                   │
                  ├────────────────────────────────────┤
                  │ Recommendation                     │
                  │ Semantic Search                     │
                  │ Price Valuation                     │
                  │ Image Condition                     │
                  │ Personalization                     │
                  │ Ranking                              │
                  └──────────────┬─────────────────────┘
                                 │
             ┌───────────────────┼──────────────────────┐
             ▼                   ▼                      ▼
       Feature Store       Vector Index           Model Registry
             │                   │                      │
             └───────────────────┼──────────────────────┘
                                 ▼
                          Evaluation Store
                                 │
                                 ▼
                        Monitoring / Alerts
```

---

# 21. AI Model Inventory

## 21.1 Model A — Car Recommendation Model

### Purpose

Recommend vehicles that best match a user's requirements.

### Inputs

- Budget.
- Preferred brand.
- Preferred body type.
- Fuel type.
- Transmission.
- Location.
- Minimum/maximum year.
- Maximum kilometres.
- Feature preferences.
- Favourite/history signals when consent and availability permit.

### Candidate retrieval

Use hybrid retrieval:

1. Structured filter retrieval.
2. Keyword retrieval.
3. Embedding similarity retrieval.

### Ranking model

Initial baseline:

```text
Weighted ranking score
= 0.30 × budget_fit
+ 0.20 × preference_fit
+ 0.15 × feature_fit
+ 0.15 × location_fit
+ 0.10 × recency
+ 0.10 × engagement_quality
```

Weights are configurable and must not be hardcoded into frontend logic.

Production evolution:

- Logistic regression / learning-to-rank baseline.
- Gradient boosting ranker.
- Pairwise ranking model.
- Neural retrieval + ranking architecture where sufficient training data exists.

### Output

```json
{
  "carId": "...",
  "score": 0.91,
  "reasons": [
    "Fits your selected budget",
    "Matches automatic transmission preference",
    "Located in your selected city"
  ],
  "modelVersion": "recommendation-v1.0"
}
```

## 21.2 Model B — Semantic Car Search

### Purpose

Allow users to search naturally, for example:

> “Automatic SUV under 10 lakh for city and occasional highway driving.”

### Pipeline

```text
Natural language query
      ↓
Intent extraction
      ↓
Structured constraints
      ↓
Embedding generation
      ↓
Vector search
      ↓
Rule-based filters
      ↓
Ranking
      ↓
Human-readable explanation
```

### Embedding model

Use a production-supported sentence-transformer model selected through benchmarking. The exact model is configurable and must be recorded in the model registry.

### Safety rule

The language understanding layer may interpret user intent but must not bypass deterministic price, availability, authorization, or transaction rules.

## 21.3 Model C — Used-Car Price Valuation

### Purpose

Estimate a reasonable market range for a vehicle.

### Features

- Brand.
- Model.
- Variant.
- Registration year.
- Manufacturing year.
- Kilometres.
- Fuel type.
- Transmission.
- Ownership count.
- City.
- Region.
- Body type.
- Service history.
- Insurance information where legally/operationally appropriate.
- Condition score.
- Market trends.

### Recommended model

Baseline:

- XGBoost Regressor or Gradient Boosting Regressor.

Comparison baselines:

- Linear regression.
- Random forest.
- CatBoost if categorical-heavy data benefits from it.

### Output

The model should return a **range**, not only a single number:

```json
{
  "estimatedLow": 615000,
  "estimatedMid": 665000,
  "estimatedHigh": 720000,
  "currency": "INR",
  "confidence": 0.82,
  "modelVersion": "valuation-v1.0",
  "topFactors": [
    "Vehicle age",
    "Kilometres driven",
    "Variant",
    "Local market conditions"
  ]
}
```

### Critical rule

Valuation is decision support. It must be labeled as an estimate and must not be represented as a guaranteed sale price.

## 21.4 Model D — Vehicle Image Condition Model

### Purpose

Assist with identifying visible vehicle-condition characteristics from submitted images.

### Possible classifications

- Clean/normal.
- Minor visible damage.
- Moderate visible damage.
- Major visible damage.
- Unclear image.
- Insufficient evidence.

### Pipeline

```text
Upload
  ↓
Image validation
  ↓
Resize / normalize
  ↓
Quality check
  ↓
Vision model
  ↓
Classification
  ↓
Confidence threshold
  ↓
Human review when uncertain
```

### Safety and quality rules

- Never claim hidden mechanical condition from a photograph.
- Never infer accident history solely from a low-confidence image signal.
- Clearly distinguish “visible damage detected” from “vehicle has structural/mechanical damage.”
- Low-confidence outputs must be marked uncertain.

## 21.5 Model E — Personalization Model

### Purpose

Adjust recommendations based on non-sensitive product behavior and explicit user preferences.

### Signals

- Search filters.
- Viewed vehicles.
- Favourite vehicles.
- Compare actions.
- Appointment actions.
- Budget selections.
- Body-type preferences.
- Fuel/transmission preferences.

### Exclusions

Do not use sensitive personal attributes for ranking or recommendations.

## 21.6 Model F — Lead / Seller Quality Score

### Purpose

Prioritize operational workflows such as seller follow-up or review.

### Inputs

- Completeness of listing.
- Image quality.
- Required field completion.
- Appointment readiness.
- Seller interaction metrics.
- Previous platform activity where appropriate.

### Rule

This score should support workflow prioritization, not make irreversible decisions automatically.

## 21.7 Model G — Marketplace Forecasting

### Purpose

Support internal analytics such as:

- Demand forecasting.
- Inventory trends.
- Category demand.
- Search demand.
- Booking volume forecasting.

Possible models:

- Moving averages.
- Exponential smoothing.
- Prophet-style models where appropriate.
- Gradient boosting with temporal features.

---

# 22. AI Data Pipeline

## 22.1 Training Pipeline

```text
Raw Data
  ↓
Validation
  ↓
Cleaning
  ↓
Deduplication
  ↓
Feature Engineering
  ↓
Train / Validation / Test Split
  ↓
Baseline Model
  ↓
Experiment Tracking
  ↓
Evaluation
  ↓
Model Approval
  ↓
Registry
  ↓
Deployment
```

## 22.2 Data Quality Checks

Every training pipeline must validate:

- Missing-value rate.
- Duplicate rate.
- Invalid ranges.
- Outliers.
- Label quality.
- Category drift.
- Distribution drift.
- Leakage risks.

## 22.3 Train/Test Rules

Avoid random leakage across vehicles or sellers when records are temporally or entity-correlated.

For valuation and forecasting, use time-aware splits where appropriate.

---

# 23. AI Model Evaluation

## Recommendation

Metrics:

- Precision@K.
- Recall@K.
- NDCG@K.
- CTR on recommended listings.
- Save/favourite rate.
- Booking conversion where valid.

## Semantic Search

Metrics:

- Retrieval recall.
- Precision@K.
- Query intent accuracy.
- Filter extraction accuracy.
- Human relevance score.

## Valuation

Metrics:

- MAE.
- RMSE.
- Median absolute percentage error.
- Prediction interval coverage.
- Segment-level performance.

## Image condition

Metrics:

- Accuracy.
- Precision.
- Recall.
- F1.
- Confusion matrix.
- Calibration / confidence reliability.

## Forecasting

Metrics:

- MAE.
- RMSE.
- MAPE or sMAPE where suitable.

---

# 24. AI Alignment, Guardrails, and Human Oversight

This section defines the **AI alignment requirements** for the platform.

## 24.1 Core alignment principles

1. AI must assist, not silently override deterministic business rules.
2. AI outputs must be attributable to a model version.
3. AI confidence must be available for machine-generated judgments.
4. Low-confidence outputs must trigger fallback or human review.
5. User-facing explanations must not invent reasons unsupported by model features or platform data.
6. Sensitive attributes must not be used for marketplace ranking unless explicitly approved, legally justified, and documented.
7. The system must avoid deceptive certainty.
8. Critical transaction states must be controlled by backend business logic.
9. AI outages must not make basic marketplace functions unavailable.
10. Every consequential AI action must have an auditable trail.

## 24.2 Human-in-the-loop boundaries

Human review should be mandatory or available for:

- Seller submission rejection.
- Fraud/suspicious listing escalation.
- Major condition claims.
- Disputed valuation cases.
- Policy-sensitive content.
- Low-confidence AI results.
- Administrative decisions affecting users materially.

## 24.3 Explanation format

Every consequential AI response should expose:

- What was predicted/recommended.
- Confidence.
- Key factors.
- Model version.
- Timestamp.
- Whether human review is required.

## 24.4 AI refusal/fallback behavior

When the model cannot provide a reliable result:

```text
Do not guess
   ↓
Return “insufficient confidence / insufficient data”
   ↓
Use deterministic fallback
   ↓
Request more information or human review
```

## 24.5 AI governance

Maintain an AI model registry with:

- Model name.
- Model version.
- Training dataset version.
- Feature schema version.
- Code commit/version.
- Evaluation metrics.
- Approval status.
- Deployment status.
- Owner.
- Rollback version.
- Created/updated timestamps.

---

# 25. AI Service API Design

Example internal endpoints:

```http
GET  /health
GET  /models
GET  /models/{name}/status
POST /recommendations
POST /semantic-search
POST /valuation
POST /image-condition
POST /forecast
```

Example valuation request:

```json
{
  "brand": "ExampleBrand",
  "model": "ExampleModel",
  "variant": "ExampleVariant",
  "registrationYear": 2023,
  "kilometers": 28000,
  "fuelType": "PETROL",
  "transmission": "AUTOMATIC",
  "city": "Pune",
  "ownershipCount": 1,
  "conditionScore": 0.86
}
```

The AI service must validate schemas independently. It must never trust arbitrary model inputs from external clients.

---

# 26. Business Rules

## 26.1 Car Listing

- Required vehicle fields must be present before publication.
- Price must be greater than zero.
- Kilometres cannot be negative.
- Registration year cannot be in an invalid future range for the selected business rules.
- A sold/inactive vehicle cannot accept new bookings.
- Deleted records should be soft-deleted when audit/history requires them.

## 26.2 Appointment

- Appointment must reference an active eligible car.
- Time slot must be available.
- Cancelled appointments cannot be completed without an explicit state transition policy.
- Rescheduling must preserve audit history.
- Duplicate conflicting appointments must be prevented transactionally.

## 26.3 Booking

- Booking requires an authenticated user.
- Booking requires an eligible vehicle.
- Booking requires an available appointment when appointment-based booking is used.
- Duplicate active booking for the same user and car must be prevented where business rules require it.
- Booking cancellation must respect configured cancellation rules.
- Booking state transitions must be validated server-side.

## 26.4 Seller Submission

```text
DRAFT
  ↓
SUBMITTED
  ↓
UNDER_REVIEW
  ├── APPROVED → LISTED
  ├── REJECTED
  └── NEEDS_INFORMATION → SUBMITTED
```

---

# 27. UI/UX System

## 27.1 Design Direction

Use a modern marketplace aesthetic inspired by high-quality fintech/e-commerce products, with a distinct automobile identity.

Reference characteristics may include:

- Strong typography.
- Clear hierarchy.
- Large vehicle imagery.
- Generous spacing.
- Card-based information grouping.
- High-quality filters.
- Persistent conversion CTAs.
- Fast interactions.
- Responsive behavior.

Do not copy proprietary branding or layouts one-for-one.

## 27.2 UX Principles

- Minimize user effort.
- Reveal complexity progressively.
- Keep primary CTA obvious.
- Preserve input state when possible.
- Avoid unnecessary form repetition.
- Provide clear validation immediately.
- Use confirmations for consequential actions.
- Provide contextual help.
- Provide accessible keyboard navigation.
- Design for mobile first and scale up.

## 27.3 Required UI States

Every major page and API-driven component must support:

```text
Loading
Empty
Success
Error
Partial data
Offline / retry where meaningful
```

---

# 28. Component Inventory

## Global

- Navbar.
- Footer.
- Button.
- Input.
- Select.
- Checkbox.
- Radio.
- Modal.
- Drawer.
- Toast.
- Skeleton.
- Pagination.
- Breadcrumbs.
- Error boundary.

## Marketplace

- SearchBar.
- FilterPanel.
- FilterDrawer.
- CarCard.
- CarGrid.
- CarGallery.
- PriceDisplay.
- VehicleSpecs.
- FeatureList.
- SimilarCars.
- FavouriteButton.
- CompareButton.
- RecommendationCard.

## Sell flow

- SellProgress.
- VehicleForm.
- ConditionForm.
- ImageUploader.
- ValuationCard.
- SellerSummary.
- SubmissionStatus.

## Appointment

- CalendarPicker.
- SlotPicker.
- AppointmentSummary.
- AppointmentStatusBadge.

## Booking

- BookingSummary.
- BookingConfirmation.
- BookingStatus.

## Profile

- ProfileCard.
- ActivityList.
- BookingList.
- AppointmentList.
- SellerSubmissionList.
- NotificationList.

## Admin

- DataTable.
- KPI cards.
- AuditTimeline.
- ReviewQueue.
- AIModelStatusCard.

---

# 29. Validation Requirements

## Client-side

Validate:

- Required fields.
- Email format.
- Phone format.
- Numeric ranges.
- Date constraints.
- Image type/size.
- Password strength.

## Server-side

The server must repeat all critical validations.

Never rely only on client-side validation.

### Validation examples

```text
Price >= 0
Kilometres >= 0
ManufacturingYear <= allowed maximum
RegistrationYear <= allowed maximum
Email unique
Appointment slot available
Booking state transition valid
Role authorized
```

---

# 30. Security Requirements

## Authentication

- Strong password hashing.
- JWT access tokens with short lifetimes.
- Refresh-token rotation where used.
- Secure cookie strategy where applicable.
- Account lockout/rate limits for repeated failures.
- Email verification.

## Authorization

- Role-based authorization.
- Resource ownership checks.
- Admin-only endpoints protected server-side.
- AI internal endpoints not publicly exposed.

## API security

- HTTPS only.
- CORS restricted to approved origins.
- Rate limiting.
- Input validation.
- Output encoding/sanitization.
- SQL injection protection via parameterized ORM queries.
- File-upload validation.
- Maximum payload sizes.
- Secret management through environment/configuration infrastructure.

## File security

- Restrict image MIME types.
- Restrict maximum image size.
- Generate server-side storage keys.
- Never trust filenames.
- Scan/moderate uploads where infrastructure supports it.
- Store private originals separately from public transformed assets when required.

## Privacy

- Collect only necessary data.
- Avoid storing sensitive information without a defined purpose.
- Apply data-retention rules.
- Provide account/data-management controls where required.

---

# 31. Error Handling

## Frontend

Use human-readable error messages:

```text
Something went wrong. Please retry.
```

For validation:

```text
Please check the highlighted fields.
```

For unavailable AI:

```text
AI assistance is temporarily unavailable. Standard marketplace features remain available.
```

## Backend

Map exceptions to controlled API responses.

Never expose:

- stack traces.
- database connection strings.
- secret keys.
- internal filesystem paths.
- raw SQL errors.

---

# 32. Logging and Observability

Every service should produce structured logs.

## Log fields

- Timestamp.
- Level.
- Service.
- RequestId.
- UserId when allowed.
- Endpoint.
- Duration.
- Status code.
- Error code.
- Model version for AI requests.

## Metrics

Track:

- API latency.
- Error rate.
- Login failures.
- Search latency.
- Listing conversion.
- Appointment creation rate.
- Booking conversion.
- AI inference latency.
- AI error rate.
- Model confidence distribution.

## Tracing

Use a correlation/request ID across:

```text
Browser → Next.js → .NET API → AI Service → Supabase PostgreSQL
```

---

# 33. Notifications

Recommended notification channels:

- In-app.
- Email.
- SMS/WhatsApp only when separately integrated and compliant.

### Events

- Registration.
- Email verification.
- Appointment created.
- Appointment confirmed.
- Appointment changed.
- Appointment cancelled.
- Booking created.
- Booking confirmed.
- Booking cancelled.
- Seller submission status change.
- AI valuation completed.

Notifications should be generated from business events rather than manually duplicated throughout controllers.

---

# 34. Search / Analytics Events

Important events include:

```text
SEARCH_PERFORMED
CAR_VIEWED
CAR_FAVOURITED
CAR_COMPARED
APPOINTMENT_STARTED
APPOINTMENT_CREATED
BOOKING_STARTED
BOOKING_CREATED
BOOKING_CANCELLED
SELL_SUBMISSION_STARTED
SELL_SUBMISSION_SUBMITTED
AI_RECOMMENDATION_VIEWED
AI_VALUATION_REQUESTED
AI_OUTPUT_SHOWN
```

Do not collect unnecessary personal information in analytics events.

---

# 35. Testing Strategy

## 35.1 Frontend Unit Tests

Test:

- Components.
- Utility functions.
- Validation schemas.
- Search filter serialization.
- Booking UI logic.
- Form state transitions.

## 35.2 Backend Unit Tests

Test:

- Business rules.
- Service methods.
- Validation.
- Authorization policies.
- Status transitions.
- Pricing calculation utilities.

## 35.3 Integration Tests

Test:

- API + database.
- Auth + protected endpoints.
- Car CRUD.
- Appointment workflow.
- Booking workflow.
- Seller submission lifecycle.
- AI service integration.

## 35.4 End-to-End Tests

Minimum scenarios:

### Buyer

```text
Open home
→ search car
→ filter
→ open details
→ select appointment
→ create booking
→ verify booking in profile
```

### Seller

```text
Signup/login
→ sell car
→ complete form
→ upload media
→ request valuation
→ select appointment
→ submit
→ verify seller status
```

### Invalid authentication

```text
Invalid email/password
→ controlled error
→ no authenticated session created
```

### Invalid booking

```text
Attempt to book unavailable/sold car
→ backend rejects
→ clear error shown
```

## 35.5 AI Tests

- Schema validation.
- Deterministic feature preprocessing.
- Model loading.
- Inference correctness on test fixtures.
- Confidence threshold behavior.
- Fallback behavior.
- Version consistency.
- Drift monitoring checks.

---

# 36. AI Testing Matrix

| Model | Functional Test | Offline Metric | Safety Check | Fallback |
|---|---|---|---|---|
| Recommendation | Candidate generation/ranking | Precision@K / NDCG | Avoid unsupported reasons | Rule-based popular/relevant cars |
| Semantic Search | Query parsing | Recall / Intent accuracy | No policy bypass | Keyword + structured search |
| Valuation | Prediction schema | MAE / RMSE | No certainty claims | Rule/range baseline |
| Image Condition | Image inference | F1 / recall | Low-confidence handling | Manual review |
| Personalization | Ranking behavior | CTR / save rate | No sensitive attributes | Non-personalized ranking |
| Forecasting | Future estimate | MAE / sMAPE | Confidence intervals | Historical baseline |

---

# 37. Performance Requirements

## Frontend

- Fast initial page load.
- Optimized image sizes.
- Lazy load below-the-fold images.
- Minimize client JavaScript where possible.
- Cache server data appropriately.

## API

- Pagination for large listings.
- Indexed queries.
- Avoid N+1 database queries.
- Async I/O.
- Request cancellation/timeouts.

## AI

- Cache repeat semantic-search and recommendation requests where safe.
- Batch embeddings when processing multiple cars.
- Use model warm-up.
- Record inference latency.
- Use CPU/GPU appropriately based on model size and deployment cost.

---

# 38. Accessibility Requirements

Target accessible UX including:

- Semantic HTML.
- Keyboard navigation.
- Visible focus states.
- Sufficient contrast.
- Correct labels.
- Form error announcements.
- Accessible modals/dialogues.
- Alt text for meaningful car imagery.
- Decorative images marked appropriately.
- Touch targets large enough for mobile use.

---

# 39. Responsive Requirements

The website must work across:

- Mobile.
- Tablet.
- Laptop.
- Desktop.
- Large desktop screens.

Priority:

```text
Mobile first
   ↓
Tablet adaptation
   ↓
Desktop enhancement
```

Do not simply scale the desktop UI down; recompose content for small screens.

---

# 40. SEO Requirements

Public marketplace pages should support:

- Semantic titles.
- Meta descriptions.
- Canonical URLs.
- Open Graph metadata.
- Structured data where appropriate.
- Crawlable listing pages.
- Clean URLs.
- Sitemap.
- Robots configuration.

User-specific pages such as account pages should generally not be publicly indexed.

---

# 41. Deployment Architecture

Recommended production-style topology:

```text
CDN / Edge
    ↓
Next.js Application
    ↓
ASP.NET Core API
    ├── Supabase PostgreSQL
    ├── Supabase Storage / Object Storage
    ├── Notification Provider
    └── AI Service
            ├── Model Registry
            └── Vector Store
```

## Environment separation

At minimum:

```text
development
staging
production
```

The environments must not share production credentials casually.

---

# 42. Environment Variables

## Frontend

```text
NEXT_PUBLIC_API_URL=
NEXT_PUBLIC_APP_URL=
NEXT_PUBLIC_ANALYTICS_ID=
```

Only variables explicitly safe for the browser may use the public prefix.

## Backend

```text
ASPNETCORE_ENVIRONMENT=
ConnectionStrings__SupabasePostgres=
# Supabase project URL is not a database credential; expose it to the backend only when needed for approved Supabase APIs.
Supabase__Url=
Jwt__Issuer=
Jwt__Audience=
Jwt__Key=
AllowedOrigins__0=
Storage__Bucket=
Storage__Endpoint=
AiService__BaseUrl=
AiService__ApiKey=
```

## AI Service

```text
APP_ENV=
MODEL_REGISTRY_PATH=
VECTOR_DB_URL=
EMBEDDING_MODEL=
VALUATION_MODEL_VERSION=
RECOMMENDATION_MODEL_VERSION=
```

Never commit secrets to Git.

---

# 43. CI/CD Requirements

Pipeline should perform:

```text
Checkout
  ↓
Install dependencies
  ↓
Lint
  ↓
Type check / compile
  ↓
Unit tests
  ↓
Integration tests
  ↓
Build frontend
  ↓
Build backend
  ↓
Build AI service
  ↓
Security checks
  ↓
Deploy to staging
  ↓
Smoke tests
  ↓
Production approval
  ↓
Production deploy
```

Production deployment should support rollback to the previous stable version.

---

# 44. Seed Data

Development should include deterministic seed data:

- Users with each role.
- Multiple brands.
- Multiple models.
- Multiple price ranges.
- Multiple fuel types.
- Multiple transmission types.
- Cities.
- Images.
- Appointments.
- Bookings.
- Seller submissions.

Seed records must clearly be marked as development/test data.

---

# 45. Demo Data Strategy

For academic demonstration, create representative listings covering:

- Budget hatchback.
- Mid-range hatchback.
- Sedan.
- Compact SUV.
- Full-size SUV.
- Electric vehicle.
- Automatic car.
- Manual car.
- High-kilometre vehicle.
- Low-kilometre vehicle.
- Premium vehicle.

This ensures the search, filter, recommendation, and comparison systems can be demonstrated meaningfully.

---

# 46. Page Inventory

```text
/
├── /buy
│   └── /buy/[carId]
├── /sell
├── /appointment
├── /booking
├── /login
├── /signup
├── /forgot-password
├── /profile
│   ├── /profile/bookings
│   ├── /profile/appointments
│   ├── /profile/favourites
│   └── /profile/seller-submissions
└── /admin
    ├── /users
    ├── /cars
    ├── /appointments
    ├── /bookings
    ├── /analytics
    ├── /ai
    └── /audit-logs
```

---

# 47. Definition of Done

A feature is complete only when:

1. UI is implemented.
2. Responsive behavior is verified.
3. API contract exists where needed.
4. Validation is implemented.
5. Authorization is implemented where needed.
6. Loading state exists.
7. Empty state exists.
8. Error state exists.
9. Database persistence works.
10. Relevant tests exist.
11. Logging exists for critical failures.
12. Documentation is updated.
13. No secrets are hardcoded.
14. Accessibility requirements are addressed.
15. Production build succeeds.

For an AI feature, add:

16. Model version is registered.
17. Offline evaluation exists.
18. Confidence/fallback behavior is implemented.
19. Explanation is grounded in model-supported features.
20. AI outage does not break the core non-AI workflow.

---

# 48. Development Phases

## Phase 1 — Foundation

- Repository.
- Next.js setup.
- .NET API setup.
- Database.
- Environment configuration.
- Base UI system.

## Phase 2 — Public Marketplace

- Header/footer.
- Home.
- Buy cars.
- Car details.
- Search/filter/sort.

## Phase 3 — Authentication

- Signup.
- Login.
- Logout.
- Profile.
- Authorization.

## Phase 4 — Seller Flow

- Sell form.
- Uploads.
- Submission lifecycle.
- Seller view.

## Phase 5 — Appointment

- Slot selection.
- Appointment API.
- Confirmation.
- Cancellation/reschedule.

## Phase 6 — Booking

- Booking API.
- Confirmation.
- Status.
- Profile integration.

## Phase 7 — Hardening

- Validation.
- Security.
- Error handling.
- Testing.
- Observability.

## Phase 8 — AI/ML

- Data pipeline.
- Semantic search.
- Recommendation.
- Valuation.
- Image condition assistance.
- Evaluation.
- Model registry.
- Guardrails.

## Phase 9 — Deployment

- CI/CD.
- Staging.
- Smoke tests.
- Production deployment.
- Monitoring.

---

# 49. MVP Acceptance Criteria

## Authentication

- User can register.
- User can log in.
- Invalid credentials are rejected.
- Protected endpoints require authentication.

## Marketplace

- Cars load from backend.
- Search works.
- Filters work.
- Car details load by ID.

## Selling

- User can submit vehicle information.
- Validation works.
- Submission persists to database.

## Appointments

- User can select a valid slot.
- Appointment persists.
- Conflicting slot is rejected.

## Booking

- User can create a booking.
- Booking is associated with the correct car/user/appointment.
- Booking appears in profile.
- Invalid booking is rejected.

## AI extension

- Recommendation endpoint returns schema-valid results.
- Valuation endpoint returns an estimated range with confidence.
- AI model version is returned.
- Low-confidence results fall back safely.
- Core buying/selling flows continue if AI service is unavailable.

---

# 50. Project Quality Gates

Every major release should pass:

### Gate A — Build

```text
Frontend build: PASS
Backend build: PASS
AI service build/startup: PASS
```

### Gate B — Quality

```text
Lint: PASS
Type check: PASS
Unit tests: PASS
Integration tests: PASS
```

### Gate C — Security

```text
No committed secrets
Auth verified
Authorization verified
Input validation verified
Upload validation verified
```

### Gate D — UX

```text
Responsive
Accessible
Loading states
Error states
Empty states
```

### Gate E — AI

```text
Model version registered
Offline metrics recorded
Confidence thresholds configured
Fallback validated
Human-review path defined
```

### Gate F — Production

```text
Environment variables configured
Database migrations applied
Health checks passing
Logs available
Rollback plan available
```

---

# 51. Non-Functional Requirements

## Reliability

The core marketplace must remain usable even when optional AI or third-party services fail.

## Scalability

The architecture must allow independent scaling of:

- Frontend.
- API.
- AI inference service.
- Database.
- Search/vector infrastructure.

## Maintainability

- Strong typing.
- Modular code.
- Clear naming.
- Small cohesive functions.
- Centralized configuration.
- Documented API contracts.

## Observability

Production failures must be diagnosable using logs, metrics, correlation IDs, and health endpoints.

---

# 52. Health Checks

## Backend

```http
GET /health
GET /health/ready
GET /health/live
```

## AI

```http
GET /health
GET /ready
```

Readiness should verify required dependencies rather than only process existence.

---

# 53. Production Data Rules

- Never treat seed/demo data as real marketplace inventory.
- Never expose internal IDs unnecessarily in public UI.
- Preserve transactional history where required.
- Use decimal-safe currency handling.
- Store UTC timestamps in backend systems where practical and convert for display.
- Use explicit timezone handling for appointments.
- Validate availability transactionally.

---

# 54. Currency and Localization

Primary target market:

```text
India
Currency: INR (₹)
```

The UI should format currency consistently.

Possible future localization:

- Multiple Indian cities.
- Regional language support.
- Currency abstraction for future expansion.

Localization should not be embedded deep inside business logic.

---

# 55. Example Technical Data Flow — Buy Car

```text
User opens /buy
       ↓
Next.js renders search UI
       ↓
User applies filters
       ↓
Client constructs validated query
       ↓
GET /api/v1/cars
       ↓
ASP.NET validates request
       ↓
Car service applies business rules
       ↓
EF Core + Npgsql executes indexed PostgreSQL query
       ↓
API returns paginated DTO
       ↓
Next.js caches server state
       ↓
Car cards render
```

---

# 56. Example Technical Data Flow — Sell Car + AI Valuation

```text
Seller submits vehicle data
       ↓
Next.js validates form
       ↓
POST /api/v1/seller/submissions
       ↓
.NET validates + stores submission
       ↓
.NET requests valuation from AI service
       ↓
AI validates schema
       ↓
Feature engineering
       ↓
Valuation model inference
       ↓
Confidence + range + factors
       ↓
.NET stores AI result + model version
       ↓
Seller sees estimated range
       ↓
Seller selects appointment
       ↓
Appointment created
```

If AI fails:

```text
AI request timeout/error
       ↓
.NET records failure
       ↓
Core submission remains valid
       ↓
Seller sees “valuation unavailable”
       ↓
Seller may continue using standard workflow
```

---

# 57. Recommended AI Model Registry Schema

```text
ModelRegistry
├── Id
├── Name
├── Version
├── Framework
├── ArtifactUri
├── DatasetVersion
├── FeatureSchemaVersion
├── MetricsJson
├── ConfidencePolicyJson
├── Status
├── ApprovedBy
├── ApprovedAt
├── DeployedAt
├── RollbackVersion
├── CreatedAt
└── UpdatedAt
```

Possible statuses:

```text
EXPERIMENTAL
VALIDATING
APPROVED
STAGING
PRODUCTION
DEPRECATED
ROLLED_BACK
```

---

# 58. AI Feature Store Concepts

For scalable recommendation and valuation pipelines, standardize features.

### Example vehicle features

```text
vehicle_age
kilometers_per_year
price_per_km
brand_frequency
model_market_demand
city_price_index
transmission_numeric
fuel_type_encoding
ownership_count
condition_score
```

Features must have:

- Name.
- Type.
- Definition.
- Source.
- Version.
- Null policy.
- Transformation logic.

Training and inference should use compatible feature definitions.

---

# 59. Model Drift Monitoring

Monitor:

- Feature distribution drift.
- Prediction distribution drift.
- Confidence drift.
- Segment-level performance.
- Data missingness.
- Category emergence.

Trigger review when configured thresholds are exceeded.

A drift signal should create a **review workflow**, not automatically retrain and deploy a model without controls.

---

# 60. Responsible AI Requirements

The AI system should:

- Be transparent about uncertainty.
- Avoid unsupported guarantees.
- Minimize unnecessary personal data.
- Avoid sensitive-attribute discrimination.
- Keep humans accountable for consequential decisions.
- Permit model rollback.
- Preserve evidence needed to investigate AI-related incidents.

---

# 61. Documentation Set

The repository should eventually contain:

```text
docs/
├── architecture.md
├── frontend.md
├── backend.md
├── database.md
├── api.md
├── authentication.md
├── security.md
├── testing.md
├── deployment.md
├── ai-models.md
├── ai-evaluation.md
├── ai-governance.md
├── observability.md
└── troubleshooting.md
```

This `PROJECT_CONTEXT.md` remains the high-level authoritative implementation context.

---

# 62. AI Coding-Agent Instructions

Any AI coding agent working on this repository should follow these rules.

## Before changing code

1. Read `PROJECT_CONTEXT.md`.
2. Inspect the current repository structure.
3. Identify the existing implementation before replacing anything.
4. Preserve working functionality unless change is explicitly required.
5. Confirm API/database dependencies before editing contracts.

## When implementing a feature

The agent must provide:

- UI.
- API integration.
- Backend logic.
- Data model/migration where required.
- Validation.
- Error handling.
- Tests.
- Responsive behavior.
- Documentation updates.

## For AI features

The agent must additionally implement:

- Input/output schema.
- Model/version reference.
- Confidence handling.
- Fallback behavior.
- Evaluation test or fixture.
- Logging/observability.
- Explicit user-facing explanation.

## Do not

- Hardcode production secrets.
- Bypass backend authorization.
- Put database credentials in frontend code.
- Let an LLM directly mutate transactional state without deterministic validation.
- Claim an AI output is certain when it is probabilistic.
- Replace an existing data model without assessing migrations and dependencies.
- Remove working features merely to simplify implementation.

---

# 63. Git and Versioning Rules

Recommended branch model:

```text
main
 ├── develop
 ├── feature/*
 ├── fix/*
 └── hotfix/*
```

Commit messages should describe intent:

```text
feat: add car filtering
fix: prevent duplicate appointment slots
feat(ai): add vehicle valuation endpoint
chore: update model registry schema
```

Database migrations must be committed with the related backend changes.

---

# 64. Current Context Baseline

This context is derived from the supplied learning/project scope and the existing Cars24-style project document.

### Known baseline

- Project is a full-stack car marketplace.
- Frontend direction is Next.js with TypeScript.
- Backend direction is .NET/C# using ASP.NET Core.
- Supabase PostgreSQL is the mandatory relational database baseline for this project.
- .NET accesses Supabase PostgreSQL through Entity Framework Core with Npgsql.
- Supabase Storage is the preferred object-storage option when the project needs managed media storage.
- Supabase pgvector may be used for semantic search and AI embeddings instead of introducing a separate vector database unless scale or architectural requirements justify it.
- Core user, car, appointment, and booking models are required.
- Frontend-to-backend API integration is required.
- Deployment is part of the project scope.
- AI/ML is an extension layer for production-oriented functionality.

### Implementation status policy

The presence of a capability in this document means it is part of the **approved project context / target architecture**. It does not automatically mean the feature is already implemented in the repository.

Agents must verify repository code before claiming a feature is complete.

---

# 65. Priority Matrix

## P0 — Core and mandatory

- Authentication.
- Buy cars.
- Car details.
- Search/filter.
- Sell car.
- Appointments.
- Bookings.
- Profile.
- Backend API.
- Database.
- Deployment.
- Security.

## P1 — Production usability

- Favourites.
- Compare.
- Seller dashboard.
- Admin dashboard.
- Notifications.
- File/object storage.
- Audit logging.
- Observability.
- Automated E2E tests.

## P2 — Intelligence

- Semantic search.
- Recommendation engine.
- Price valuation.
- Image-condition assistance.
- Personalization.
- Marketplace forecasting.

## P3 — Advanced platform evolution

- Advanced ranking.
- Real-time notifications.
- Payment integration.
- Advanced fraud detection.
- Multilingual search.
- Mobile app.
- More sophisticated multimodal AI.

---

# 66. Project Success Definition

The project is successful when a reviewer can independently run the platform and demonstrate:

```text
Open website
   ↓
Browse cars
   ↓
Search/filter
   ↓
Inspect a car
   ↓
Sign in
   ↓
Book appointment
   ↓
Create booking
   ↓
See booking in profile
   ↓
Sell a car
   ↓
Complete seller workflow
   ↓
Receive valuation/recommendation assistance
   ↓
Observe persisted data through backend/database
   ↓
Run automated tests
   ↓
Deploy application successfully
```

The platform must feel like **one integrated product**, not separate tutorial exercises.

---

# 67. Final Project Statement

**AutoMarket is a full-stack Cars24-style used-car marketplace that combines modern Next.js frontend engineering, ASP.NET Core backend development, relational data management, end-to-end user workflows, and an optional production-oriented AI/ML intelligence layer.**

The platform supports the complete lifecycle of discovering vehicles, evaluating listings, selling vehicles, scheduling appointments, creating bookings, managing accounts, and providing AI-assisted decision support.

The implementation must prioritize correctness, maintainability, responsive UX, secure APIs, reliable data persistence, testability, and transparent AI behavior.

---

# 68. Change Control

When a future project requirement conflicts with this document:

1. Prefer the newest explicitly approved project context.
2. Update this file with a new version.
3. Record the material architectural change.
4. Update affected API/database/UI/AI documentation.
5. Update tests and deployment configuration.

### Versioning format

```text
MAJOR.MINOR.PATCH
```

- **MAJOR:** architecture/product scope changes.
- **MINOR:** new modules or significant capability additions.
- **PATCH:** corrections or clarifications.

---

# 69. Context Maintenance Log

| Version | Date | Change |
|---|---|---|
| 1.1.0 | 2026-09-25 | Updated the canonical data layer from SQL Server to Supabase PostgreSQL; standardized Npgsql/EF Core access, Supabase storage/vector options, PostgreSQL constraints/indexing, migration strategy, and deployment configuration. |
| 1.0.0 | 2026-09-25 | Initial canonical project context created from Cars24-style project document; expanded with production architecture, database/API standards, AI/ML model layer, AI alignment/guardrails, evaluation, testing, security, deployment, and AI coding-agent rules. |

---

## End of PROJECT_CONTEXT.md

**Authoritative instruction:** Read this file before implementing or reviewing project changes. Verify actual repository state before marking any item as implemented.
