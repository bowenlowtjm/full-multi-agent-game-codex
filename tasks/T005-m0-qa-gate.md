---
id: T005
title: QA gate for M0 scaffold (compile/tests/ci preflight)
status: done
milestone: M0
owner: qa
depends_on: [T001, T002]
branch: chore/T005-m0-qa
---

## Goal
Run independent M0 preflight checks before entering milestone feature implementation.

## Acceptance
- [ ] Headless compile check is clean.
- [ ] Test asmdef references resolve with com.unity.test-framework present.
- [ ] CI workflow definitions present and valid.
- [ ] Known risks and assumptions documented in run log.

## Notes
- QA ownership is independent from implementation tasks.
