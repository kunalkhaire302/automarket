# AutoMarket — User Flow Specification

## 1. Scope

This document defines the end-to-end user journeys for the Cars24-style marketplace.

Core journeys:

1. Guest discovery.
2. Buyer search and booking.
3. Seller submission and appointment.
4. Authentication.
5. Profile management.
6. Admin operations.
7. AI-assisted discovery.

## 2. Guest Discovery Flow

```text
Landing Page
  ↓
Hero/Search
  ↓
Buy Cars
  ↓
Search / Filter / Sort
  ↓
Car Details
  ├── View gallery
  ├── View specifications
  ├── View inspection details
  └── Similar cars
        ↓
     Login/Signup when protected action required
```

Guests can browse public marketplace data but protected actions require authentication.

## 3. Buyer Journey

```text
Home
 ↓
Buy Cars
 ↓
Set budget + preferences
 ↓
Browse results
 ↓
Open car
 ↓
Review vehicle information
 ↓
Optional: Favourite / Compare
 ↓
AI recommendations / similar cars
 ↓
Book appointment
 ↓
Select date/time
 ↓
Confirm appointment
 ↓
Booking review
 ↓
Create booking
 ↓
Confirmation reference
 ↓
Profile → My Bookings
```

## 4. Buyer Search Flow

### Step 1
User enters text query or selects filters.

### Step 2
Frontend serializes filter state into URL/query parameters when possible.

### Step 3
Backend validates and executes search.

### Step 4
Results show loading/success/empty/error states.

### Step 5
User adjusts filters without losing the current context.

## 5. AI Search Flow

```text
Natural-language query
 ↓
Parse intent
 ↓
Extract constraints
 ↓
Validate constraints
 ↓
Structured filter search
 ↓
Semantic similarity
 ↓
Rank results
 ↓
Show explanation
```

If parsing confidence is low, show interpreted filters and let the user correct them.

## 6. Car Detail Flow

```text
Result Card
 ↓
Car Detail
 ├── Gallery
 ├── Price
 ├── Specs
 ├── Features
 ├── Inspection
 ├── AI assistance
 └── Similar cars
      ↓
Choose appointment/booking
```

## 7. Appointment Flow

```text
Car Detail
 ↓
Book Appointment
 ↓
Authenticate
 ↓
Choose appointment type
 ↓
Select date
 ↓
Select available slot
 ↓
Review
 ↓
Confirm
 ↓
Appointment Created
```

### Failure states

- slot taken before submit;
- car no longer available;
- invalid date/time;
- service unavailable.

The backend must reject stale/conflicting requests safely.

## 8. Booking Flow

```text
Appointment
 ↓
Booking Review
 ↓
Validate user + car + appointment
 ↓
Transaction
 ↓
Booking Created
 ↓
Confirmation
 ↓
Notification
 ↓
Profile history
```

## 9. Seller Flow

```text
Home
 ↓
Sell Car
 ↓
Vehicle basics
 ↓
Usage
 ↓
Condition
 ↓
Upload images
 ↓
AI valuation assistance
 ↓
Contact + expected price
 ↓
Select inspection slot
 ↓
Review
 ↓
Submit
 ↓
Submission status
 ↓
Admin/operations review
 ↓
Approved / rejected / needs information
```

## 10. Seller AI Valuation Flow

```text
Vehicle details
 ↓
Validate inputs
 ↓
Valuation model
 ↓
Low / Mid / High estimate
 ↓
Confidence + factors
 ↓
Seller review
 ↓
Optional modification of expected price
 ↓
Submit
```

AI output never forces the seller's price.

## 11. Image Condition Flow

```text
Upload image
 ↓
Validate file
 ↓
Store image
 ↓
AI image analysis
 ↓
Visible-condition result
 ↓
Confidence check
 ├── high confidence → display assistance
 └── low confidence → request better image / human review
```

## 12. Authentication Flow

### Signup

```text
Signup form
 ↓
Client validation
 ↓
API validation
 ↓
Create user
 ↓
Hash password
 ↓
Verification where configured
 ↓
Session
 ↓
Return to intended page
```

### Login

```text
Email + password
 ↓
API
 ↓
Credential verification
 ↓
Issue session/token
 ↓
Redirect to intended destination
```

## 13. Profile Flow

```text
Profile
 ├── Personal information
 ├── Security
 ├── Favourites
 ├── Compare
 ├── Appointments
 ├── Bookings
 ├── Seller submissions
 └── Notifications
```

## 14. Booking Cancellation Flow

```text
My Booking
 ↓
Open booking
 ↓
Cancel
 ↓
Show policy/confirmation
 ↓
Confirm
 ↓
Server validates cancellability
 ↓
State transition
 ↓
Audit event
 ↓
Notification
```

## 15. Admin Flow

```text
Admin Login
 ↓
Dashboard
 ├── Users
 ├── Cars
 ├── Seller Submissions
 ├── Appointments
 ├── Bookings
 ├── Analytics
 ├── AI Models
 └── Audit Logs
```

### Seller review

```text
Submission Queue
 ↓
Open submission
 ↓
Review data/images
 ↓
Review AI assistance if present
 ↓
Approve / Reject / Request information
 ↓
Audit
 ↓
Notification
```

AI must not silently approve/reject sellers.

## 16. Notification Flow

Events that may trigger notifications:

- signup verification;
- appointment confirmation;
- appointment reschedule;
- appointment cancellation;
- booking confirmation;
- booking cancellation;
- seller submission status change;
- operational messages.

## 17. Error Recovery Flows

### Network failure

```text
Request
 ↓
Timeout/failure
 ↓
Preserve user input
 ↓
Show retry
 ↓
Retry safely if idempotent
```

### AI unavailable

```text
AI request
 ↓
Failure
 ↓
Show non-blocking message
 ↓
Continue deterministic workflow
```

## 18. State Machines

### Car

```text
DRAFT → PENDING_REVIEW → LISTED → BOOKED → SOLD
                      └→ REJECTED
LISTED → INACTIVE
```

### Appointment

```text
PENDING → CONFIRMED → COMPLETED
        ├→ RESCHEDULED → CONFIRMED
        ├→ CANCELLED
        └→ NO_SHOW
```

### Booking

```text
PENDING → CONFIRMED → COMPLETED
        ├→ PAYMENT_PENDING → PAID → COMPLETED
        ├→ CANCELLED
        └→ EXPIRED
```

## 19. Conversion/Trust Principles

The product should reduce friction while remaining transparent:

- never hide important fees;
- clearly label estimates;
- show availability accurately;
- provide cancellation rules before commitment;
- distinguish verified inspection data from user-provided data and AI assistance.

## 20. User Flow Acceptance Criteria

- Every primary CTA leads to a complete path.
- Protected actions prompt for authentication at the correct point.
- Forms preserve progress on recoverable errors.
- Booking and appointment conflicts are handled safely.
- AI assistance is optional and transparent.
- Users can recover from errors without restarting unnecessarily.
