# AutoMarket implementation context

The supplied [CARS24_PROJECT_CONTEXT.md](CARS24_PROJECT_CONTEXT.md) remains the full authoritative product specification. Read it alongside this file and the root-level specialized specifications. The newer user request also makes all six advanced capability groups mandatory.

## Implementation decisions

- Monorepo: `frontend`, `backend`, `ai-service`, `scripts`, and `docs`.
- Next.js App Router and TypeScript; browser requests use a same-origin Next.js backend-for-frontend proxy. Tokens remain in HttpOnly cookies; state-changing browser requests require a matching Origin.
- ASP.NET Core 10, application services, domain entities/rules, EF Core/Npgsql infrastructure. Supabase PostgreSQL remains the only application database.
- API contracts retain `/api/v1` and the documented response envelopes.
- All business timestamps use UTC. Currency is INR. Money uses decimal/numeric.
- No production inventory, geographic locations, maintenance reference costs, or pricing signals are fabricated. Empty configured datasets return honest empty/unavailable states.
- Financial payments are outside the implemented booking-intent workflow; booking never reports a payment as settled.

See [implementation status](docs/implementation-status.md) for verified delivery and external requirements. A documented target is not a completion claim.
