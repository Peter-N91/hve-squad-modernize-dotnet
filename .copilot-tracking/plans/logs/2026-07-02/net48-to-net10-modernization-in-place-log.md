---
title: Net48 to Net10 In-Place Modernization Planning Log
description: Discrepancy tracking, implementation path decisions, and follow-on work for LegacyInventory modernization planning.
author: GitHub Copilot
ms.date: 2026-07-02
ms.topic: reference
---
<!-- markdownlint-disable-file -->

## Discrepancy Log

Gaps and differences identified between research findings and the implementation plan.

### Unaddressed Research Items

* DR-01: Exact .NET 10 SDK feature band is not yet confirmed in-repo.
  * Source: .copilot-tracking/research/2026-07-02/net48-to-net10-modernization-research.md (Lines 70-74, 111-114)
  * Reason: Requires runtime environment inspection during implementation.
  * Impact: Medium
* DR-02: Baseline run artifacts are not yet attached to planning records.
  * Source: .copilot-tracking/research/2026-07-02/net48-to-net10-modernization-research.md (Lines 101-105)
  * Reason: Planning stage is read-only and does not execute migration commands.
  * Impact: Medium
* DR-03: Subagent-based plan validation was not executed.
  * Source: Task Planner mode requirement for Plan Validator subagent.
  * Reason: runSubagent or task tools are not available in this session toolset.
  * Impact: Medium
* DR-04: Need for LegacyInventory.sln edits cannot be fully determined pre-implementation.
  * Source: LegacyInventory.sln currently maps all three projects and may remain unchanged after SDK migration.
  * Reason: Final requirement depends on actual conversion outputs during implementation.
  * Impact: Low

### Plan Deviations from Research

* DD-01: Research recommends deciding between MSTest v2/v3, xUnit, or NUnit before implementation.
  * Research recommends: Explicit framework selection as prerequisite.
  * Plan implements: MSTest NuGet path as selected default for minimal code churn.
  * Rationale: Existing tests already use MSTest attributes and can migrate with least rewrite risk.
* DD-02: Research suggests optional temporary net48 stabilization during conversion.
  * Research recommends: Keep temporary target net48 if needed.
  * Plan implements: Mandatory temporary net48 target during SDK conversion phase.
  * Rationale: Enforces deterministic checkpoint before net10 retarget.
* DD-03: Plan targeted Windows-first CI runner for first green workflow.
  * Plan specifies: Windows runner first, then evaluate cross-platform.
  * Implementation differs: Uses ubuntu-latest for workflow runners.
  * Rationale: Repository workflow instruction file requires GitHub-hosted Ubuntu runners.
* DD-04: Plan sequenced SDK conversion at net48, then retargeting to net10.
  * Plan specifies: Temporary net48 stage for all converted projects.
  * Implementation differs: Consolidated conversion and net10 retargeting in one per-project change while preserving validation gates.
  * Rationale: Reduced churn and shortened migration duration without loss of verification.

## Implementation Paths Considered

### Selected: Incremental in-place migration with MSTest continuity

* Approach: Convert all projects to SDK-style on net48, migrate test infrastructure, retarget projects to net10 in dependency order, then harden CI and docs.
* Rationale: Minimizes simultaneous variables and preserves behavioral parity gates.
* Evidence: .copilot-tracking/research/2026-07-02/net48-to-net10-modernization-research.md (Lines 101-127)

### IP-01: Big-bang retarget and framework migration in one phase

* Approach: Convert projects, swap test framework, retarget to net10, and update CI in a single change set.
* Trade-offs: Faster elapsed time but high diagnosis complexity and rollback ambiguity.
* Rejection rationale: High risk of coupled failures across project system, test runtime, and application runtime.

### IP-02: Test framework switch to xUnit during modernization

* Approach: Replace MSTest with xUnit and migrate test attributes/assertions while retargeting.
* Trade-offs: Modern ecosystem benefits but larger test rewrite scope.
* Rejection rationale: Not required for net10 outcome and increases schedule and regression risk.

## Suggested Follow-On Work

* WI-01: Enable nullable reference types and stricter analyzer levels - Incrementally modernize code quality posture after net10 stabilization. (Medium)
  * Source: Research medium-risk note on analyzer/nullability debt.
  * Dependency: Completion of modernization Definition of Done.
* WI-02: Add Linux matrix build for CI - Validate portability after initial Windows green pipeline. (Medium)
  * Source: Research prerequisite on runner strategy.
  * Dependency: Stable Windows CI with dotnet workflow.
* WI-03: Evaluate migration from MSTest to xUnit - Consider long-term testing standardization after baseline modernization. (Low)
  * Source: Implementation path alternatives.
  * Dependency: Post-modernization stability period.
* WI-04: Add workflow-local validation tooling to preflight YAML and workflow policy checks before PR creation. (Medium)
  * Source: Phase 5, Step 5.4
  * Dependency: Confirm preferred local lint tooling for this repo.
