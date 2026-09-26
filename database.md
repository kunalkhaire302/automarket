# AutoMarket — Database Specification

## 1. Database Platform

**Supabase PostgreSQL** is the authoritative production database for AutoMarket.

Application access:

```text
ASP.NET Core → EF Core → Npgsql → Supabase PostgreSQL
```

The browser never connects directly to PostgreSQL.

## 2. Database Responsibilities

PostgreSQL stores all authoritative transactional and marketplace data, including:

- users;
- roles;
- cars;
- images metadata;
- features;
- inspections;
- seller submissions;
- appointments;
- bookings;
- favourites;
- comparison lists;
- notifications;
- AI result metadata;
- search events;
- audit logs.

Use Supabase Storage for large binary assets where appropriate rather than storing image bytes inside ordinary relational tables.

## 3. PostgreSQL Conventions

- Prefer `uuid` identifiers.
- Use `timestamptz` for timestamps.
- Use `numeric` for INR monetary values.
- Use foreign keys for relationships.
- Use explicit unique constraints for business keys.
- Prefer normalized relational tables for core business data.
- Use `jsonb` only for genuinely semi-structured attributes.
- Add indexes based on actual query patterns.
- Use transactions for booking/appointment state transitions.
- Avoid destructive hard deletes where history/audit is needed.

## 4. Core Schema

### 4.1 users

| Column | Type | Rules |
|---|---|---|
| id | uuid | PK |
| name | text | required |
| email | text | required, unique after normalization |
| phone | text | optional/required by flow |
| password_hash | text | never plaintext |
| role | text/enum | BUYER/SELLER/ADMIN |
| email_verified | boolean | default false |
| is_active | boolean | default true |
| created_at | timestamptz | required |
| updated_at | timestamptz | required |

### 4.2 cars

| Column | Type | Rules |
|---|---|---|
| id | uuid | PK |
| seller_id | uuid | FK users.id where applicable |
| brand | text | required |
| model | text | required |
| variant | text | optional depending on inventory |
| manufacturing_year | int | constrained |
| registration_year | int | constrained |
| fuel_type | text/enum | constrained |
| transmission | text/enum | constrained |
| body_type | text/enum | constrained |
| kilometers | int | >= 0 |
| price | numeric | > 0 |
| city | text | required for listed cars |
| state | text | supported geography |
| ownership_count | int | >= 0 |
| description | text | sanitized |
| status | text/enum | lifecycle state |
| created_at | timestamptz | required |
| updated_at | timestamptz | required |

### 4.3 car_images

- id uuid PK
- car_id FK
- storage_bucket
- storage_key
- public_url or signed-url metadata strategy
- image_type
- sort_order
- width
- height
- moderation_status
- created_at

### 4.4 car_features

A normalized many-to-many-friendly representation of vehicle features.

Recommended fields:

- id;
- car_id;
- feature_key;
- feature_value;
- created_at.

For controlled features, prefer a reference table instead of unconstrained text.

### 4.5 car_inspections

- id;
- car_id;
- inspector/user reference where appropriate;
- inspection date;
- odometer reading;
- exterior score;
- interior score;
- mechanical score if validated through an operational inspection process;
- notes;
- status;
- created_at;
- updated_at.

AI image inference must not be treated as a mechanical inspection record by default.

### 4.6 seller_submissions

- id;
- seller_id;
- car_id;
- submission_status;
- expected_price numeric;
- valuation_id nullable;
- review_notes;
- submitted_at;
- reviewed_at;
- reviewed_by.

State machine:

```text
DRAFT → SUBMITTED → UNDER_REVIEW
                     ├→ APPROVED → LISTED
                     ├→ REJECTED
                     └→ NEEDS_INFORMATION → SUBMITTED
```

### 4.7 appointments

- id;
- user_id;
- car_id;
- appointment_type;
- appointment_date;
- start_time;
- end_time;
- location;
- status;
- notes;
- created_at;
- updated_at.

### 4.8 bookings

- id;
- user_id;
- car_id;
- appointment_id;
- booking_reference unique;
- booking_status;
- amount numeric;
- payment_status if enabled;
- created_at;
- updated_at.

### 4.9 favourites

Use a unique composite constraint such as `(user_id, car_id)`.

### 4.10 comparison_items

Track user-selected vehicles for comparison with a unique constraint preventing duplicates.

### 4.11 notifications

- id;
- user_id;
- type;
- title;
- message;
- deep_link;
- read_at;
- created_at.

### 4.12 audit_logs

Recommended:

- id;
- actor_user_id;
- action;
- entity_type;
- entity_id;
- previous_state jsonb where justified;
- new_state jsonb where justified;
- request_id;
- timestamp;

Do not store secrets in audit records.

### 4.13 AI tables

Recommended tables:

`ai_models`

- model_name;
- model_version;
- model_type;
- artifact_uri;
- feature_schema_version;
- metrics jsonb;
- status;
- approved_at;
- created_at.

`ai_predictions`

- id;
- model_id;
- user_id nullable;
- car_id nullable;
- prediction_type;
- input_snapshot_hash;
- output jsonb;
- confidence numeric;
- created_at.

`ai_recommendations`

- id;
- user_id;
- car_id;
- score;
- reasons jsonb;
- model_id;
- created_at.

## 5. Relationships

```text
users
 ├──< cars
 ├──< appointments
 ├──< bookings
 ├──< favourites
 ├──< seller_submissions
 ├──< notifications
 └──< audit_logs

cars
 ├──< car_images
 ├──< car_features
 ├──< car_inspections
 ├──< appointments
 ├──< bookings
 └──< ai_predictions

seller_submissions ──> cars
bookings ──> appointments
```

## 6. Index Strategy

At minimum evaluate indexes for:

- cars(status);
- cars(brand, model);
- cars(city);
- cars(price);
- cars(manufacturing_year);
- cars(fuel_type);
- cars(transmission);
- cars(created_at);
- appointments(car_id, appointment_date);
- appointments(user_id, appointment_date);
- bookings(user_id, created_at);
- bookings(car_id, booking_status);
- seller_submissions(seller_id, submission_status);
- notifications(user_id, read_at, created_at);

Do not create excessive indexes without measuring write and storage cost.

## 7. Transactions

Required transactional workflows:

### Appointment creation

1. Begin transaction.
2. Validate car availability.
3. Validate time slot.
4. Check conflicts.
5. Create appointment.
6. Record audit event.
7. Commit.

### Booking creation

1. Begin transaction.
2. Validate authenticated user.
3. Validate car state.
4. Validate appointment.
5. Validate conflict constraints.
6. Create booking.
7. Update relevant availability state if required.
8. Record audit event.
9. Commit.

## 8. EF Core Migrations

EF Core migrations with Npgsql are the version-controlled schema migration mechanism.

Recommended commands:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Production deployment should apply approved migrations through a controlled release process.

## 9. Supabase Environment Strategy

Use separate Supabase projects for development, staging, and production where possible.

Backend configuration example:

```text
ConnectionStrings__SupabasePostgres=...
```

The connection string must remain server-side.

Public frontend environment variables must never contain the database password or Supabase service-role key.

## 10. Supabase Storage

Recommended buckets may include:

```text
car-images
seller-documents
inspection-images
```

Use generated storage keys and authorization checks. Do not trust user-provided paths.

## 11. pgvector

If enabled, store embeddings separately from transactional vehicle fields.

Potential table:

`car_embeddings`

- id;
- car_id;
- embedding vector(...);
- model_name;
- model_version;
- source_hash;
- created_at.

Use vector search for semantic retrieval and combine it with deterministic filters.

## 12. Backup and Recovery

The operational backup and point-in-time recovery strategy must be documented against the selected Supabase plan. Restore procedures must be tested in a non-production environment.

## 13. Data Retention

Define retention rules for:

- audit logs;
- abandoned bookings;
- inactive seller submissions;
- uploaded documents;
- AI prediction history;
- analytics events.

Retention must be implemented intentionally, not by accidental deletion.
