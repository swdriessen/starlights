# MTP browser fixture paths and loopback scheme

- **What happened:** An opt-in Kestrel fixture was responsive, but its expected
  repository-relative readiness sentinel appeared missing. An external
  antiforgery probe also returned 500 over loopback HTTP.
- **Impact:** Several repeated readiness checks were spent on the wrong path.
- **Evidence/root cause:** Native MTP executed the test with its output directory
  as the current directory. Relative fixture paths resolved beneath `bin`, not
  the agent's repository working directory. The real Identity configuration
  deliberately uses `AntiforgeryOptions.Cookie.SecurePolicy = Always`, which
  rejects token generation on an HTTP request.
- **Candidate improvements:** Resolve explicitly reusable fixture paths against
  a discovered repository root and report the canonical path. For a strictly
  opted-in localhost HTTP fixture, override cookie/antiforgery secure policy in
  the fixture only; never weaken application configuration to accommodate tests.
  Probe both OpenAPI and the antiforgery endpoint before declaring browser
  readiness.
