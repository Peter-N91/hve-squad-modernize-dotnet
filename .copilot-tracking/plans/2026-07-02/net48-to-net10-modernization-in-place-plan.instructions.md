---
applyTo: '.copilot-tracking/changes/2026-07-02/net48-to-net10-modernization-in-place-changes.md'
title: Net48 to Net10 In-Place Modernization Plan
description: Implementation-ready autopilot plan for migrating LegacyInventory from .NET Framework 4.8 to .NET 10.
author: GitHub Copilot
ms.date: 2026-07-02
ms.topic: how-to
---
<!-- markdownlint-disable-file -->

## Overview

Modernize LegacyInventory in place from .NET Framework 4.8 to .NET 10 by converting all projects to SDK-style, migrating test infrastructure off MSTest v1 GAC dependencies, retargeting runtime and tests, and establishing dotnet CLI based validation and CI.

## Objectives

### User Requirements

* Build an implementation-ready plan for in-place modernization from .NET Framework 4.8 to .NET 10. Source: user request dated 2026-07-02.
* Include objectives, non-goals, preconditions, phased tasks, validation strategy, rollback, and definition of done. Source: user request dated 2026-07-02.
* Include autopilot stage sequence for implementation and review with estimated risk per phase. Source: user request dated 2026-07-02.
* Keep the plan concrete to this repository structure and files. Source: user request dated 2026-07-02.

### Derived Objectives

* Preserve behavior parity across business logic while changing project system and runtime model. Derived from: research finding that risk is concentrated in build and test infrastructure.
* Sequence migration to reduce blast radius by validating each project transition before retargeting to net10. Derived from: ranked blockers and recommended migration sequence in research artifact.
* Define blocker-to-mitigation tasks so implementation can proceed without re-planning. Derived from: required explicit mapping and research blocker list.

## Non-Goals

* Introduce architectural redesigns such as dependency injection containers, async-wide refactors, or domain model rewrites.
* Perform large-scale code-style modernization beyond what is required for compilation and test execution on net10.
* Multi-target the solution for long-term dual support; this plan targets in-place completion on net10.
* Add deployment platform changes beyond repository build/test CI validation.

## Context Summary

### Project Files

* LegacyInventory.sln - Contains three projects and build configurations that must remain wired after migration.
* src/LegacyInventory.Core/LegacyInventory.Core.csproj - Classic net48 class library project to convert and retarget.
* src/LegacyInventory.ConsoleApp/LegacyInventory.ConsoleApp.csproj - Classic net48 executable project with App.config reference and project reference.
* src/LegacyInventory.ConsoleApp/App.config - Framework-specific startup runtime declaration to replace or remove.
* tests/LegacyInventory.Tests/LegacyInventory.Tests.csproj - Classic test project with MSTest v1 GAC dependency and legacy test project GUIDs.
* tests/LegacyInventory.Tests/InventoryServiceTests.cs - MSTest v1 tests using ExpectedException and TestInitialize.
* tests/LegacyInventory.Tests/PricingCalculatorTests.cs - MSTest v1 tests requiring framework package migration.
* README.md - Documents legacy Visual Studio specific build and test commands that must be modernized.

### References

* .copilot-tracking/research/2026-07-02/net48-to-net10-modernization-research.md - Primary modernization research baseline and blocker ranking.

### Standards References

* .github/instructions/copilot-tracking.instructions.md - Tracking artifact path, naming, and markdownlint-disable requirements.
* .github/instructions/markdown.instructions.md - Markdown authoring requirements and frontmatter guidance.
* .github/instructions/writing-style.instructions.md - Writing voice and style constraints for markdown artifacts.

## Preconditions and Entry Checklist

* [x] Confirm .NET 10 SDK availability on implementation host with dotnet --info.
* [ ] Create migration branch from current default branch.
* [x] Capture net48 baseline build and test evidence before first project-file change.
* [x] Decide test framework destination: MSTest v2/v3 NuGet path (selected in this plan for minimal rewrite).
* [x] Confirm CI target runner strategy: Windows runner for first green build, then evaluate cross-platform.
* [ ] Record phase checkpoints as separate commits for deterministic rollback.

## Blocker to Mitigation Mapping

| Blocker ID | Blocker | Evidence | Mitigation Task | Plan Step |
| --- | --- | --- | --- | --- |
| B1 | Non-SDK project format blocks net10 retargeting | Three classic csproj files with TargetFrameworkVersion v4.8 | Convert each project to SDK-style while temporarily retaining net48 to preserve behavior | Phase 2, Steps 2.1-2.3 |
| B2 | MSTest v1 GAC dependency blocks dotnet test | tests/LegacyInventory.Tests/LegacyInventory.Tests.csproj references Microsoft.VisualStudio.QualityTools.UnitTestFramework | Replace with Microsoft.NET.Test.Sdk + MSTest.TestFramework + MSTest.TestAdapter packages and update test code attributes/usings | Phase 3, Steps 3.1-3.2 |
| B3 | App.config startup metadata is .NET Framework specific | src/LegacyInventory.ConsoleApp/App.config supportedRuntime v4.8 | Remove framework startup metadata and move needed settings to runtime-neutral config pattern | Phase 4, Step 4.2 |
| B4 | Build/test process is Visual Studio path dependent | README build and test commands call MSBuild.exe and vstest.console.exe by fixed install paths | Introduce dotnet restore/build/test workflow and update README runbook | Phase 5, Steps 5.2-5.3 |
| B5 | Potential analyzer and nullability debt on retarget | Research medium blocker on modern compiler defaults | Retarget in project order and defer nullable/analyzer strictness escalation to follow-on tasks | Phase 4, Step 4.3 and Log WI-01 |

## Autopilot Stage Execution Sequence

| Stage | Phase | Scope | Estimated Risk | Why |
| --- | --- | --- | --- | --- |
| Implement | Phase 1 | Baseline and toolchain gate | Medium | Environment mismatch can block all downstream work.
| Implement | Phase 2 | SDK-style conversion at net48 | High | Project format changes affect all build entry points.
| Implement | Phase 3 | Test infrastructure migration | High | Test adapter/framework transition can silently reduce coverage if misconfigured.
| Implement | Phase 4 | net10 retarget and runtime config migration | High | Runtime/API compatibility and config behavior changes converge here.
| Implement | Phase 5 | Solution-level CLI and CI hardening | Medium | Workflow and docs can fail despite successful local builds.
| Review | Phase 6 | End-to-end validation and acceptance review | Medium | Final integration may uncover cross-phase regressions.

## Validation Strategy

* Phase 1: Run existing net48 baseline build and test commands from README to capture parity baseline.
* Phase 2: After each SDK conversion step, run dotnet restore and dotnet build for impacted project and then solution.
* Phase 3: Run dotnet test tests/LegacyInventory.Tests/LegacyInventory.Tests.csproj after package migration and then solution-level dotnet test.
* Phase 4: Retarget one project at a time and run dotnet build or dotnet test after each retarget.
* Phase 5: Run CI-equivalent command set locally: dotnet restore, dotnet build -c Release, dotnet test -c Release --no-build.
* Phase 6: Re-run full validation and compare test count and pass/fail parity against baseline inventory.

## Rollback Strategy

* Commit at phase boundaries with explicit checkpoint labels.
* If a phase fails and minor fix is not immediate, revert only the phase commit(s), not the whole branch.
* Keep a tagged baseline commit before first conversion change.
* Rollback order:
  * Revert CI/documentation changes first.
  * Revert retargeting and config changes next.
  * Revert test framework migration next.
  * Revert SDK-style conversion last.
* Recovery criteria after rollback:
  * Legacy net48 build command succeeds.
  * Legacy vstest command succeeds with original test assembly.

## Definition of Done

* All three projects are SDK-style and target net10.
* Test project uses NuGet-based modern test stack and no GAC Visual Studio test assembly reference remains.
* dotnet restore, dotnet build, and dotnet test succeed at solution level in Debug and Release.
* README reflects modernized build and test commands.
* CI workflow exists and passes on repository default branch policy target.
* No open High or Critical discrepancies remain in planning log and implementation change log.

## Implementation Checklist

### [x] Implementation Phase 1: Baseline and Environment Gate

<!-- parallelizable: false -->

* [x] Step 1.1: Capture baseline net48 build and test evidence using current README commands.
  * Details: .copilot-tracking/details/2026-07-02/net48-to-net10-modernization-in-place-details.md (Lines 29-56)
* [x] Step 1.2: Verify .NET 10 SDK availability and create global.json pin candidate.
  * Details: .copilot-tracking/details/2026-07-02/net48-to-net10-modernization-in-place-details.md (Lines 57-80)
* [x] Step 1.3: Validate phase changes.
  * Run baseline commands and archive outputs for parity comparison.

### [x] Implementation Phase 2: Convert Projects to SDK Style While Staying on net48

<!-- parallelizable: false -->

* [x] Step 2.1: Convert src/LegacyInventory.Core/LegacyInventory.Core.csproj to SDK-style net48.
  * Details: .copilot-tracking/details/2026-07-02/net48-to-net10-modernization-in-place-details.md (Lines 93-117)
* [x] Step 2.2: Convert src/LegacyInventory.ConsoleApp/LegacyInventory.ConsoleApp.csproj and preserve project reference to Core.
  * Details: .copilot-tracking/details/2026-07-02/net48-to-net10-modernization-in-place-details.md (Lines 118-141)
* [x] Step 2.3: Convert tests/LegacyInventory.Tests/LegacyInventory.Tests.csproj to SDK-style net48 test project skeleton.
  * Details: .copilot-tracking/details/2026-07-02/net48-to-net10-modernization-in-place-details.md (Lines 143-166)
* [x] Step 2.4: Validate phase changes.
  * Run dotnet restore and dotnet build LegacyInventory.sln.

### [x] Implementation Phase 3: Migrate Test Framework Infrastructure

<!-- parallelizable: false -->

* [x] Step 3.1: Add Microsoft.NET.Test.Sdk, MSTest.TestFramework, and MSTest.TestAdapter package references.
  * Details: .copilot-tracking/details/2026-07-02/net48-to-net10-modernization-in-place-details.md (Lines 179-203)
* [x] Step 3.2: Update test source files for modern MSTest compatibility and remove legacy dependencies.
  * Details: .copilot-tracking/details/2026-07-02/net48-to-net10-modernization-in-place-details.md (Lines 204-228)
* [x] Step 3.3: Validate phase changes.
  * Run dotnet test tests/LegacyInventory.Tests/LegacyInventory.Tests.csproj.

### [x] Implementation Phase 4: Retarget Runtime and Projects to net10

<!-- parallelizable: false -->

* [x] Step 4.1: Retarget Core, then ConsoleApp, then Tests to net10 in that order.
  * Details: .copilot-tracking/details/2026-07-02/net48-to-net10-modernization-in-place-details.md (Lines 240-264)
* [x] Step 4.2: Replace framework-specific runtime startup configuration from App.config.
  * Details: .copilot-tracking/details/2026-07-02/net48-to-net10-modernization-in-place-details.md (Lines 266-289)
* [x] Step 4.3: Resolve compatibility warnings required for green build without broad refactoring.
  * Details: .copilot-tracking/details/2026-07-02/net48-to-net10-modernization-in-place-details.md (Lines 291-316)
* [x] Step 4.4: Validate phase changes.
  * Run dotnet build and dotnet test after each retarget step.

### [x] Implementation Phase 5: End-to-End CLI and CI Hardening

<!-- parallelizable: true -->

* [x] Step 5.1: Update LegacyInventory.sln and project metadata as needed for stable dotnet CLI traversal.
  * Details: .copilot-tracking/details/2026-07-02/net48-to-net10-modernization-in-place-details.md (Lines 329-351)
* [x] Step 5.2: Add .github/workflows/dotnet-ci.yml for restore, build, and test on Windows.
  * Details: .copilot-tracking/details/2026-07-02/net48-to-net10-modernization-in-place-details.md (Lines 353-375)
* [x] Step 5.3: Update README build and test instructions to dotnet CLI flows.
  * Details: .copilot-tracking/details/2026-07-02/net48-to-net10-modernization-in-place-details.md (Lines 377-399)
* [x] Step 5.4: Validate phase changes.
  * Run local CI-equivalent command set and workflow lint if available.

### [x] Implementation Phase 6: Final Validation and Review Gate

<!-- parallelizable: false -->

* [x] Step 6.1: Run full project validation.
  * Execute dotnet restore, dotnet build -c Release, and dotnet test -c Release --no-build.
* [x] Step 6.2: Fix minor validation issues.
  * Resolve straightforward compile, test, and workflow issues discovered in final pass.
* [x] Step 6.3: Review against definition of done and blocker mapping.
  * Confirm each blocker mitigation task is complete and evidenced in change log.
* [x] Step 6.4: Prepare implementation handoff for review stage.
  * Capture risk outcomes by phase and unresolved follow-on items.

## Planning Log

See .copilot-tracking/plans/logs/2026-07-02/net48-to-net10-modernization-in-place-log.md for discrepancy tracking, implementation paths considered, and suggested follow-on work.

## Dependencies

* .NET 10 SDK installed on implementation runner
* dotnet CLI available on PATH
* Git branch with checkpoint commit workflow
* Access to update CI workflow files and repository documentation

## Success Criteria

* Plan includes all required sections from user request and maps each blocker to mitigation task. Traces to: user requirement and research blocker ranking.
* Ordered phases define executable implementation sequence with per-phase validation commands. Traces to: modernization research migration sequence.
* Autopilot implementation and review stages include explicit risk estimates. Traces to: user requirement for autopilot stage execution sequence.
* Rollback and definition of done provide objective go/no-go criteria for implementation gate. Traces to: user request for implementation readiness.
