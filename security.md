<div align="center">

# 🛡️ AutoMarket — Security Documentation

**Defense-in-depth practices for a production marketplace handling accounts, payments-adjacent data, and AI orchestration**

</div>

---

## Table of Contents

1. [Security Goals](#1-security-goals)
2. [Threat Model](#2-threat-model)
3. [Authentication](#3-authentication)
4. [Authorization (RBAC)](#4-authorization-rbac)
5. [Database Security](#5-database-security)
6. [Supabase-Specific Controls](#6-supabase-specific-controls)
7. [API Security](#7-api-security)
8. [XSS Prevention](#8-xss-prevention)
9. [CSRF Protection](#9-csrf-protection)
10. [File Upload Security](#10-file-upload-security)
11. [IDOR / BOLA Prevention](#11-idor--bola-prevention)
12. [Secrets Management](#12-secrets-management)
13. [Logging & Privacy-Safe Telemetry](#13-logging--privacy-safe-telemetry)
14. [AI Service Security](#14-ai-service-security)
15. [Data Privacy](#15-data-privacy)
16. [Security Headers](#16-security-headers)
17. [Dependency Security](#17-dependency-security)
18. [Secure State Transitions](#18-secure-state-transitions)
19. [Security Testing](#19-security-testing)
20. [Incident Response](#20-incident-response)
21. [Security Acceptance Criteria](#21-security-acceptance-criteria)
22. [Reporting a Vulnerability](#22-reporting-a-vulnerability)

---

## 1. Security Goals

AutoMarket protects **users, vehicle data, appointments, bookings, seller submissions, the AI service, and infrastructure** from unauthorized access, manipulation, data leakage, and abuse.

Security is implemented as **defense in depth** — no single control (not even authentication) is trusted alone. Every layer, from the database schema to the HTTP response headers, assumes the layer above it can fail.

| Principle | What it means in practice |
|---|---|
| **Backend as the trust boundary** | The browser never talks to PostgreSQL, Supabase Storage, or the AI service directly — only the ASP.NET Core API does. |
| **Least privilege everywhere** | Database roles, service credentials, and user permissions grant the minimum access required. |
| **Fail safe, not fail open** | Missing configuration (e.g. no AI credentials) produces an explicit "unavailable" response, never a silent bypass or fabricated result. |
| **Server-validated state** | Client-supplied values never directly set privileged fields (roles, ownership, prices, booking status). |

---

## 2. Threat Model

| Category | Examples |
|---|---|
| **Identity & session** | Credential theft, brute-force login, session/token replay, refresh-token abuse |
| **Access control** | IDOR/BOLA, privilege escalation, horizontal access between sellers/buyers |
| **Injection** | SQL/NoSQL injection, command injection via uploads or metadata |
| **Web client** | XSS, CSRF (where cookie auth applies), clickjacking |
| **API abuse** | Scraping, credential stuffing, booking/appointment spam, resource exhaustion |
| **Integration** | SSRF via unsafe outbound integrations, secret leakage across services |
| **AI-specific** | Prompt injection, model output misuse, adversarial input to valuation/recommendation models |
| **Supply chain** | Vulnerable dependencies, unpinned transitive packages |

---

## 3. Authentication

**Implemented baseline:**

- Passwords hashed server-side (`Infrastructure/Identity.cs`) — **plaintext passwords are never stored**.
- Short-lived JWT **access tokens** signed with a server-only key (`Jwt__Key`, ≥32 bytes).
- **Rotating**, HttpOnly **refresh tokens** — each use invalidates the previous token, limiting replay value if one leaks.
- Automatic silent renewal of expired access tokens via the refresh flow.
- Rate limiting on `AuthController` endpoints (login, register, password reset) to blunt brute-force and credential-stuffing attempts.
- Secure session invalidation on password change / explicit logout.

```text
POST /api/v1/auth/register        → rate-limited, validated, password hashed server-side
POST /api/v1/auth/login           → rate-limited, generic error on failure (no user enumeration)
POST /api/v1/auth/refresh         → rotates refresh token, revokes the prior one
POST /api/v1/auth/logout          → revokes active session/refresh token
POST /api/v1/auth/forgot-password → short-lived, single-use reset token
POST /api/v1/auth/reset-password  → validates token, forces re-authentication
```

> **Rule:** Access tokens are never persisted anywhere but memory/short-lived storage on the client; refresh tokens are HttpOnly and never readable by JavaScript.

---

## 4. Authorization (RBAC)

Authorization is **mandatory and server-side** — the frontend's UI state is a convenience, never a security control.

```text
GUEST    → public browsing, search, vehicle details
BUYER    → saved cars, compare, appointments, bookings, referrals, wallet
SELLER   → listing intake, own-listing management, submission tracking
ADMIN    → cities, service centres, submissions, appointments, bookings, users
```

Every protected request is checked against **two independent conditions**, not one:

1. **Role permission** — does this role have this capability at all?
2. **Resource ownership** — does this *specific* user own or have explicit administrative rights over this *specific* resource?

```text
❌ Bad:  A seller can PATCH /cars/{id} for any id, because the endpoint only
         checks "is this user a SELLER?"

✅ Good: A seller can PATCH /cars/{id} only when {id} belongs to that seller's
         account, or the caller holds an ADMIN role.
```

This ownership check is applied uniformly across cars, appointments, bookings, saved lists, referrals, and wallet records.

---

## 5. Database Security

- **Least-privilege credentials**: the application connects with a role scoped to exactly what it needs — never a superuser or the Supabase service-role key.
- **Server-side only**: database credentials live in `backend/.env.local` (dev) or the deployment secret manager (prod) — never in frontend code or bundles.
- **Parameterized queries only**: all data access goes through EF Core + Npgsql; there is no raw string-concatenated SQL.
- **Database-enforced constraints** back up application logic — e.g. unique constraints prevent two overlapping bookings for the same slot even under concurrent requests, closing a race condition that application-layer checks alone cannot.
- Administrative database access is restricted outside of migrations and approved tooling (`backend/tools`).

---

## 6. Supabase-Specific Controls

AutoMarket uses Supabase PostgreSQL as its datastore, with a security posture that has been **explicitly verified**, not just declared:

| Control | Status |
|---|---|
| Application data lives in a **private** `automarket` schema | ✅ Implemented |
| `PUBLIC`, `anon`, and `authenticated` roles denied schema/table privileges | ✅ Verified via direct privilege check |
| Browser reaches data only through the backend API | ✅ No direct client-side Supabase data access |
| Supabase security advisor | ✅ Reports no security lints |
| Row-Level Security (RLS) | ⚪ Not enabled — tables are not exposed via Supabase Data APIs, so the backend is the sole authorization layer. RLS is planned as defense-in-depth *if* the schema is ever exposed through those APIs. |
| Production/development project separation | Required before go-live |
| Storage bucket access | Requires signed/controlled access; see [§10](#10-file-upload-security) |

> The Supabase **service-role key** must never be shipped to the browser or committed to source control, under any circumstance.

---

## 7. API Security

**Standards applied to every endpoint:**

- HTTPS only outside local development.
- JSON request/response bodies with UTC timestamps.
- Every response carries a correlation/request ID for traceability.
- Server-side validation on **body, query parameters, route parameters, and relevant headers** — never trust client-side validation alone.
- DTOs are the contract; EF Core entities are never serialized directly to clients (prevents accidental over-exposure of internal fields).

### Rate limiting

Stronger limits are applied to abuse-prone endpoints:

| Endpoint class | Why |
|---|---|
| Login / registration | Brute-force & credential-stuffing resistance |
| Password reset | Prevent token-guessing / enumeration abuse |
| Image upload | Storage & cost abuse prevention |
| AI requests | Inference cost control & prompt-abuse mitigation |
| Booking / appointment creation | Prevent slot-spam and automated scalping |

### CORS

Only approved application origins are allowed in production; local development origins are excluded from production configuration.

---

## 8. XSS Prevention

- React's default output escaping is relied upon for all user-generated content rendering.
- No `dangerouslySetInnerHTML` (or equivalent) for user-generated or AI-generated content.
- **AI responses are never rendered as raw HTML** — they are treated as untrusted text.
- A restrictive Content-Security-Policy is applied wherever compatible with required third-party assets (see [§16](#16-security-headers)).

---

## 9. CSRF Protection

Where cookie-based authentication is used for state-changing browser requests, the API applies:

- `SameSite` cookie policy appropriate to the auth flow (BFF-issued cookies).
- Origin/Referer validation on state-changing requests.
- CSRF tokens where cookie-auth patterns require them beyond `SameSite` alone.

---

## 10. File Upload Security

Vehicle image and document uploads (seller submissions, condition photos) are constrained by:

- **MIME type allowlisting** — only approved image types are accepted; the file extension alone is never trusted.
- **Size and pixel-dimension limits** to prevent resource exhaustion.
- **Server-generated storage keys** — client-supplied filenames never determine storage paths.
- Rejection of content-sniffing anomalies (a file claiming to be a JPEG that isn't).
- Optional malware scanning before a file is made available.
- Image transformation/re-encoding before public serving, where appropriate, to strip embedded payloads and metadata.

> Per [`docs/implementation-status.md`](./docs/implementation-status.md), Supabase Storage upload wiring is currently pending bucket/service credentials — image metadata and empty-state handling exist today, and this section defines the control set that activates once storage is configured.

---

## 11. IDOR / BOLA Prevention

Every protected resource lookup is **ownership-aware by construction**, not by convention.

```text
❌ Bad:   GET /bookings/{id}
          → looks up by id only, trusts the caller

✅ Good:  GET /bookings/{id}
          → looks up by id AND (owner == current user OR caller is ADMIN)
          → returns 404 (not 403) for resources outside scope, to avoid
            confirming the resource's existence to unauthorized callers
```

This pattern is applied to bookings, appointments, saved cars, seller listings, referral records, and wallet history — anywhere a numeric or GUID identifier appears in a route.

---

## 12. Secrets Management

**Classified as secrets:**

- JWT signing key (`Jwt__Key`)
- Supabase/PostgreSQL connection strings
- Supabase service-role / storage credentials
- SMTP / transactional email credentials
- AI service credentials (`AiService__ApiKey`, `AI_API_KEY`)
- Any third-party external API keys

**Rules:**

| Rule | Enforcement |
|---|---|
| Never commit secrets to source control | `.env.local` files are gitignored; only `.env.example` templates are tracked |
| Use environment variables / a secret manager in production | Required before deployment |
| Rotate immediately on suspected compromise | See [§20 Incident Response](#20-incident-response) |
| Never expose secrets through logs, error messages, or stack traces | Enforced in error-handling middleware |
| `AiService__ApiKey` and `AI_API_KEY` must match | Shared secret authenticates the .NET ↔ FastAPI boundary |

---

## 13. Logging & Privacy-Safe Telemetry

**Never logged, under any circumstance:**

- Passwords (hashed or plaintext)
- Access tokens / refresh tokens
- API keys or connection strings
- Private user documents

**What is logged:** structured security events (auth failures, authorization denials, rate-limit triggers, admin actions) with enough metadata for investigation but without exposing the sensitive payloads above. Audit records are retained for accountability on consequential actions (listing approval, booking changes, admin overrides).

---

## 14. AI Service Security

The FastAPI AI service sits **behind** the .NET API — it is never called directly by the browser, and it authenticates every inbound call with an internal shared key (`AI_API_KEY`).

| Control | Purpose |
|---|---|
| Authentication on user-specific AI calls | Prevents anonymous inference abuse |
| Rate limiting | Bounds inference cost and abuse surface |
| Input length/size constraints & schema validation | Prevents oversized or malformed payloads reaching the model layer |
| Model/prompt isolation | Limits blast radius of any single request |
| Output validation | AI output is checked before being trusted or persisted |
| No direct execution of AI output | AI responses are **never** executed as code, SQL, shell commands, or privileged API calls |
| Prompt-injection resistance | Guards against instructions smuggled inside user-supplied or retrieved content |
| Honest unavailability | If no approved model artifact is configured, the service returns an explicit "unavailable" response rather than a fabricated estimate — this is a security property as much as a product one, since a fabricated confident-looking output is a form of deception |

---

## 15. Data Privacy

- **Data minimization**: only data required for the active product flow is collected.
- Retention/deletion controls are considered for: profile data, uploaded documents, appointment history, booking history, telemetry, and AI interaction history.
- **Sensitive personal attributes are never used in recommendation models** without a documented, lawful, necessary purpose and explicit governance approval — this is a hard gate, not a guideline to be revisited later.

---

## 16. Security Headers

Applied at the API/edge layer wherever compatible with required functionality:

| Header | Purpose |
|---|---|
| `Content-Security-Policy` | Restricts script/style/resource origins, mitigates XSS impact |
| `Strict-Transport-Security` | Enforces HTTPS on repeat visits |
| `X-Content-Type-Options: nosniff` | Prevents MIME-sniffing attacks |
| `Referrer-Policy` | Limits referrer leakage to third parties |
| `Permissions-Policy` | Restricts access to sensitive browser APIs |
| Frame-ancestors / clickjacking protection | Prevents the app from being embedded in a malicious frame |

---

## 17. Dependency Security

- Dependency versions are locked (`package-lock.json`, `.csproj` pinned versions, `requirements.txt`).
- Automated vulnerability scanning runs against frontend, backend, and AI-service dependency trees.
- Critical security patches are prioritized for prompt application.
- Unused dependencies are removed rather than left dormant as latent attack surface.

---

## 18. Secure State Transitions

Booking, appointment, seller-approval, and listing-publication states are governed by an **explicit state machine**, enforced server-side and backed by database constraints where conflicts matter (e.g. overlapping appointment slots).

```text
❌ Bad:   Client sends { "status": "APPROVED" } and the server trusts it.
✅ Good:  Server validates the requested transition against the current
          state and the caller's role before applying it; invalid
          transitions are rejected outright.
```

Client-provided status values can never skip, bypass, or force an illegal transition.

---

## 19. Security Testing

Automated and manual coverage includes:

- Authentication tests (valid/invalid credentials, token expiry, refresh rotation)
- Authorization tests (role checks, cross-role access attempts)
- IDOR/BOLA regression tests (ownership bypass attempts)
- Input validation tests (malformed, oversized, boundary-value payloads)
- Rate-limit tests (threshold and lockout behavior)
- File-upload tests (MIME spoofing, oversized files, malformed images)
- XSS regression tests
- SQL-injection regression tests (defense-in-depth, despite parameterized queries)
- Secret-exposure checks (logs, error responses, client bundles)
- Dependency vulnerability scans
- API fuzzing on critical/high-value endpoints (auth, bookings, payments-adjacent flows)

These run alongside the project's standard test suites — see the [README's Testing & Verification section](./README.md#-testing--verification) for exact commands.

---

## 20. Incident Response

| Step | Action |
|---|---|
| 1. Detection | Identify the security event via logs, alerts, or external report |
| 2. Containment | Limit blast radius — revoke sessions/tokens, disable affected endpoints if necessary |
| 3. Credential rotation | Rotate any secret that may have been exposed |
| 4. Impact assessment | Determine what data or systems were affected |
| 5. Remediation | Fix the root cause, not just the symptom |
| 6. Audit trail | Preserve logs and evidence for investigation |
| 7. Recovery | Restore normal operation, verify integrity of affected data |
| 8. Post-incident review | Document lessons learned and update controls/tests to prevent recurrence |

---

## 21. Security Acceptance Criteria

A release is not considered production-ready unless **all** of the following hold:

- [ ] No plaintext passwords anywhere in the system.
- [ ] No privileged Supabase keys present in frontend bundles.
- [ ] All protected resources enforce both role and ownership checks.
- [ ] Critical mutations (approvals, bookings, admin overrides) are audited.
- [ ] Production logs contain no secrets or sensitive payloads.
- [ ] All uploads are validated (type, size, dimensions, storage key generation).
- [ ] Authentication and AI endpoints are rate limited.
- [ ] All secrets are outside source control and rotated for production.

---

## 22. Reporting a Vulnerability

If you discover a security vulnerability in AutoMarket, please report it privately rather than opening a public issue. Include:

- A description of the vulnerability and its potential impact
- Steps to reproduce (proof-of-concept if possible)
- Any relevant logs, requests, or screenshots

Please allow a reasonable window for investigation and remediation before any public disclosure.

---

<div align="center">

Security is a continuous process, not a checklist run once. This document is updated as the threat model, infrastructure, and feature set evolve.

</div>
