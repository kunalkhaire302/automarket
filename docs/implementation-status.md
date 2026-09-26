# Implementation status

Last verified: 26 September 2026.

## Complete and exercised

- Responsive marketplace pages for home, search, details, selling, appointments, bookings, account, compare, locations, rewards, notifications, and administration.
- ASP.NET Core REST API with input validation, consistent envelopes, authorization, ownership checks, role checks, rate limiting, structured errors, health checks, and audit logging.
- Supabase PostgreSQL migrations for marketplace, identity, workflow, notification, referral, and wallet records.
- Transactional lifecycle rules and unique database constraints for active appointment and booking conflicts.
- Password hashing, access tokens, refresh-token rotation, secure BFF cookies, origin checks, and automatic expired-access renewal.
- Deterministic price rules, maintenance reference calculations, fuzzy catalogue search, and recommendation scoring.
- Internal-key protected FastAPI service and bounded .NET orchestration. Valuation refuses to infer unless a locally approved artifact is configured.
- Unit, safety, build, dependency-audit, database-advisor, and live end-to-end checks.
- Development inventory seeder with 80 realistic Indian-market listings, 18 cities, 18 service centres, 12 featured listings, 480 local illustrative image records, and rerunnable internal inventory keys.

## Deliberately unavailable until credentials or approved data are supplied

- Supabase Storage uploads: image metadata and empty-photo states exist, but no storage bucket/service credentials were supplied.
- Push delivery: notification records and user preferences exist, but FCM credentials and device-token registration are not configured.
- Learned valuation, semantic search, and image-condition models: no approved dataset or model artifact was supplied. The product shows an honest fallback and keeps core transactions operational.
- Production email delivery, email verification, and password-reset delivery require a transactional email provider.

## Supabase security posture

Application data lives in the private `automarket` schema. `PUBLIC`, `anon`, and `authenticated` were denied schema and table privileges; the browser reaches data only through the backend. A direct privilege check confirmed both Supabase client roles lack schema usage and user-table read access. The Supabase security advisor reports no security lints.

RLS is not enabled on these private tables because they are not in an exposed API schema and the backend owns authorization. RLS may be added later as defense in depth if the schema is ever exposed through Supabase Data APIs.

## Production launch checklist

1. Rotate all development secrets and store production values in the deployment secret manager.
2. Configure storage, email, and push providers if those optional capabilities are required.
3. Load only evaluated, approved model artifacts and record their manifests and monitoring thresholds.
4. Run migrations through CI, then repeat security/performance advisors and the end-to-end smoke test.
5. Configure TLS, domain allowlists, telemetry, backups, retention, and alerting described in `deployment.md` and `security.md`.
