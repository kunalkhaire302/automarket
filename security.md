# AutoMarket — Security Specification

## 1. Security Goals

Protect users, vehicle data, appointments, bookings, seller submissions, AI services, and infrastructure from unauthorized access, manipulation, data leakage, abuse, and accidental exposure.

Security must be implemented as defense in depth.

## 2. Threat Model Areas

Threats to consider:

- credential theft;
- brute-force login;
- session abuse;
- IDOR/BOLA resource-access vulnerabilities;
- injection attacks;
- malicious uploads;
- API abuse;
- privilege escalation;
- CSRF where cookie-auth patterns apply;
- XSS;
- SSRF through unsafe integrations;
- secret leakage;
- model abuse/prompt injection;
- data scraping;
- automated booking abuse.

## 3. Authentication

Recommended baseline:

- strong password hashing;
- short-lived access tokens;
- refresh-token rotation where applicable;
- email verification;
- password reset with short-lived single-use tokens;
- authentication rate limits;
- secure session invalidation on password/security changes.

Never store plaintext passwords.

## 4. Authorization

Server-side RBAC is mandatory.

Roles:

```text
GUEST
BUYER
SELLER
ADMIN
```

The server must validate both:

1. role permission;
2. resource ownership or explicit administrative access.

Example: a seller must not be able to edit another seller's car by changing a route parameter.

## 5. Database Security

- Use least-privilege database credentials.
- Keep database credentials server-side.
- Use parameterized ORM queries.
- Restrict administrative database access.
- Use database constraints in addition to application validation.
- Review RLS configuration if Supabase RLS is enabled.
- Never expose the Supabase service-role key in the browser.

## 6. Supabase Security

The project uses Supabase PostgreSQL as the database platform.

Rules:

- public frontend must not contain privileged database keys;
- server-side connection strings are secrets;
- Storage access must be authorized;
- private buckets require signed or controlled access mechanisms;
- production and development projects should be separated;
- migration changes must be reviewed and version controlled.

## 7. API Security

### Input validation

Validate:

- body;
- query parameters;
- route parameters;
- headers where relevant;
- file metadata.

### Rate limiting

At minimum apply stronger limits to:

- login;
- password reset;
- registration;
- image upload;
- AI requests;
- booking creation.

### CORS

Allow only approved application origins in production.

## 8. XSS Prevention

- React escaping is the default.
- Sanitize user-generated HTML if rich content is ever allowed.
- Do not render arbitrary HTML from AI responses.
- Apply a restrictive Content Security Policy where compatible.

## 9. CSRF

If cookie-based authentication is used for state-changing browser requests, apply appropriate CSRF protection and SameSite policy.

## 10. File Upload Security

Vehicle image upload requirements:

- allowlist image MIME types;
- size limits;
- pixel dimension limits;
- server-generated storage keys;
- reject executable/content-sniffing anomalies;
- optional malware scanning;
- image transformation before public serving where appropriate.

Never trust the extension alone.

## 11. IDOR/BOLA Prevention

Every protected resource lookup must be ownership-aware.

Bad:

```text
GET /bookings/{id}
```

with no ownership check.

Good:

```text
Find booking by id AND authorized user/admin scope.
```

## 12. Secrets Management

Secrets include:

- JWT signing keys;
- Supabase/PostgreSQL credentials;
- storage credentials;
- SMTP credentials;
- AI service credentials;
- external API keys.

Rules:

- never commit secrets;
- use environment/secret stores;
- rotate compromised credentials;
- do not expose secrets through logs.

## 13. Logging Security

Never log:

- passwords;
- access tokens;
- refresh tokens;
- API keys;
- database credentials;
- private documents.

Log security events with appropriate metadata and privacy controls.

## 14. AI Security

AI endpoints require:

- authentication where user-specific;
- rate limits;
- input length/size constraints;
- schema validation;
- prompt/model isolation where LLM components exist;
- output validation;
- tool/use restrictions;
- protection against prompt injection and indirect instruction attacks.

AI output must never be directly executed as code, SQL, shell commands, or privileged API requests.

## 15. Privacy

Collect only data required for the product flow.

Consider retention and deletion controls for:

- profile data;
- uploaded documents;
- appointment history;
- booking history;
- telemetry;
- AI interaction history.

Do not use sensitive personal attributes in recommendation models without a documented, lawful, necessary purpose and explicit governance approval.

## 16. Security Headers

Consider:

- Content-Security-Policy;
- Strict-Transport-Security;
- X-Content-Type-Options;
- Referrer-Policy;
- Permissions-Policy;
- frame-ancestors protection / clickjacking protection.

## 17. Dependency Security

- Lock dependency versions where appropriate.
- Run dependency vulnerability scanning.
- Update critical security fixes promptly.
- Remove unused dependencies.

## 18. Secure State Transitions

Booking, appointment, seller approval, and listing publication states must be validated against an explicit state machine.

Do not allow client-provided status values to bypass permitted transitions.

## 19. Security Testing

Minimum test categories:

- authentication tests;
- authorization tests;
- IDOR tests;
- validation tests;
- rate-limit tests;
- file-upload tests;
- XSS tests;
- SQL injection regression tests;
- secret exposure checks;
- dependency scans;
- API fuzzing for critical endpoints.

## 20. Incident Response

Document:

1. detection;
2. containment;
3. credential rotation;
4. impact assessment;
5. remediation;
6. audit trail;
7. recovery;
8. post-incident review.

## 21. Security Acceptance Criteria

- No plaintext passwords.
- No privileged Supabase keys in frontend bundles.
- All protected resources enforce ownership/role checks.
- Critical mutations are audited.
- Production logs contain no secrets.
- Uploads are validated.
- Authentication and AI endpoints are rate limited.
- Secrets are outside source control.
