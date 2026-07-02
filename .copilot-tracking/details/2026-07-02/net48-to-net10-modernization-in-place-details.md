---
title: Net48 to Net10 In-Place Modernization Details
description: Phase-level implementation details for LegacyInventory net48 to net10 in-place migration.
author: GitHub Copilot
ms.date: 2026-07-02
ms.topic: reference
---
<!-- markdownlint-disable-file -->

## Context Reference

Sources:

* .copilot-tracking/research/2026-07-02/net48-to-net10-modernization-research.md
* .copilot-tracking/plans/2026-07-02/net48-to-net10-modernization-in-place-plan.instructions.md
* LegacyInventory.sln
* src/LegacyInventory.Core/LegacyInventory.Core.csproj
* src/LegacyInventory.ConsoleApp/LegacyInventory.ConsoleApp.csproj
* src/LegacyInventory.ConsoleApp/App.config
* tests/LegacyInventory.Tests/LegacyInventory.Tests.csproj
* tests/LegacyInventory.Tests/InventoryServiceTests.cs
* tests/LegacyInventory.Tests/PricingCalculatorTests.cs
* README.md

## Implementation Phase 1: Baseline and Environment Gate

<!-- parallelizable: false -->

### Step 1.1: Capture baseline net48 build and test evidence

Run the current legacy commands exactly as documented to establish pre-migration baseline artifacts.

Files:

* README.md - Legacy command source of truth
* LegacyInventory.sln - Baseline build target

Discrepancy references:

* Addresses DR-02 by preserving baseline evidence before changing project system.

Success criteria:

* Legacy MSBuild command succeeds for Debug build.
* Legacy vstest command succeeds for the test assembly.
* Baseline output logs are archived under implementation change artifact.

Context references:

* .copilot-tracking/research/2026-07-02/net48-to-net10-modernization-research.md (Lines 67-83) - Build and test tooling constraints
* README.md (Lines 20-38) - Legacy build and test commands

Dependencies:

* Windows host with Visual Studio MSBuild and vstest paths available.

### Step 1.2: Verify SDK toolchain and pin SDK version

Validate target SDK availability and add global.json with selected net10 SDK feature band.

Files:

* global.json - New SDK pin file at repository root

Discrepancy references:

* Addresses DR-01 prerequisite to avoid implementation stalls due to missing SDK pinning.

Success criteria:

* dotnet --info shows expected net10 SDK.
* global.json is created and dotnet --version resolves pinned feature band.

Context references:

* .copilot-tracking/research/2026-07-02/net48-to-net10-modernization-research.md (Lines 70-74, 111-114) - Missing SDK pin and prerequisite

Dependencies:

* Step 1.1 completion.

### Step 1.3: Validate phase changes

Validation commands:

* dotnet --info - Verify SDK inventory
* dotnet --version - Verify pinned SDK resolution

## Implementation Phase 2: Convert Projects to SDK Style While Staying on net48

<!-- parallelizable: false -->

### Step 2.1: Convert Core project to SDK-style net48

Replace classic MSBuild project XML with SDK-style format and preserve compile includes through SDK defaults.

Files:

* src/LegacyInventory.Core/LegacyInventory.Core.csproj - Convert format, keep target net48, keep assembly identity

Discrepancy references:

* Mitigates B1 and addresses DR-03 selected migration path.

Success criteria:

* Core project builds under dotnet build.
* No source files are dropped from compile set.

Context references:

* src/LegacyInventory.Core/LegacyInventory.Core.csproj (Lines 1-45) - Classic format baseline

Dependencies:

* Phase 1 complete.

### Step 2.2: Convert ConsoleApp project to SDK-style net48

Preserve executable output type and Core project reference while removing explicit framework assembly references.

Files:

* src/LegacyInventory.ConsoleApp/LegacyInventory.ConsoleApp.csproj - Convert format and preserve project reference

Discrepancy references:

* Mitigates B1 by converting second project in dependency order.

Success criteria:

* Console project builds under dotnet build.
* Project reference to Core resolves correctly.

Context references:

* src/LegacyInventory.ConsoleApp/LegacyInventory.ConsoleApp.csproj (Lines 1-53) - Classic format and project reference

Dependencies:

* Step 2.1 complete.

### Step 2.3: Convert test project skeleton to SDK-style net48

Move test project to SDK-style while temporarily preserving net48 target before test framework package migration.

Files:

* tests/LegacyInventory.Tests/LegacyInventory.Tests.csproj - Convert format and preserve Core reference

Discrepancy references:

* Mitigates B1 and sets prerequisite for B2 mitigation.

Success criteria:

* Test project restores and builds with SDK-style structure.
* Legacy test framework reference is still visible for controlled removal in Phase 3.

Context references:

* tests/LegacyInventory.Tests/LegacyInventory.Tests.csproj (Lines 1-56) - Legacy baseline with test project GUIDs

Dependencies:

* Step 2.2 complete.

### Step 2.4: Validate phase changes

Validation commands:

* dotnet restore LegacyInventory.sln
* dotnet build LegacyInventory.sln -c Debug

## Implementation Phase 3: Migrate Test Framework Infrastructure

<!-- parallelizable: false -->

### Step 3.1: Replace MSTest v1 GAC dependency with NuGet packages

Remove Microsoft.VisualStudio.QualityTools.UnitTestFramework reference and add modern package references.

Files:

* tests/LegacyInventory.Tests/LegacyInventory.Tests.csproj - Add Microsoft.NET.Test.Sdk, MSTest.TestFramework, MSTest.TestAdapter

Discrepancy references:

* Mitigates B2 directly.

Success criteria:

* dotnet restore succeeds without VsInstallRoot dependency.
* Test discovery works under dotnet test.

Context references:

* tests/LegacyInventory.Tests/LegacyInventory.Tests.csproj (Lines 34-39) - GAC dependency evidence

Dependencies:

* Phase 2 complete.

### Step 3.2: Update test sources for modern MSTest compatibility

Adjust test code if package version differences require attribute or assertion updates.

Files:

* tests/LegacyInventory.Tests/InventoryServiceTests.cs - Update using and attributes if required
* tests/LegacyInventory.Tests/PricingCalculatorTests.cs - Update using and attributes if required

Discrepancy references:

* Mitigates B2 and supports parity objective.

Success criteria:

* Existing 20 tests are discovered and pass.

Context references:

* tests/LegacyInventory.Tests/InventoryServiceTests.cs (Lines 1-122) - Current test style
* tests/LegacyInventory.Tests/PricingCalculatorTests.cs (Lines 1-84) - Current test style

Dependencies:

* Step 3.1 complete.

### Step 3.3: Validate phase changes

Validation commands:

* dotnet test tests/LegacyInventory.Tests/LegacyInventory.Tests.csproj -c Debug

## Implementation Phase 4: Retarget Runtime and Projects to net10

<!-- parallelizable: false -->

### Step 4.1: Retarget projects in dependency order

Retarget Core to net10 first, then ConsoleApp, then Tests to reduce compounding errors.

Files:

* src/LegacyInventory.Core/LegacyInventory.Core.csproj - Set TargetFramework to net10.0
* src/LegacyInventory.ConsoleApp/LegacyInventory.ConsoleApp.csproj - Set TargetFramework to net10.0
* tests/LegacyInventory.Tests/LegacyInventory.Tests.csproj - Set TargetFramework to net10.0

Discrepancy references:

* Mitigates B1 completion and addresses B5 incrementally.

Success criteria:

* Each project builds after its retarget before moving to next project.

Context references:

* .copilot-tracking/research/2026-07-02/net48-to-net10-modernization-research.md (Lines 116-120) - Recommended project retarget order

Dependencies:

* Phase 3 complete.

### Step 4.2: Replace framework-specific runtime startup configuration

Remove or transform App.config startup metadata that is tied to .NET Framework runtime selection.

Files:

* src/LegacyInventory.ConsoleApp/App.config - Remove supportedRuntime block or migrate required settings
* src/LegacyInventory.ConsoleApp/Program.cs - Add configuration loading only if app settings must persist

Discrepancy references:

* Mitigates B3 directly.

Success criteria:

* Console app runs under dotnet run without framework startup config errors.

Context references:

* src/LegacyInventory.ConsoleApp/App.config (Lines 1-6) - Framework-specific startup declaration

Dependencies:

* Step 4.1 complete for ConsoleApp.

### Step 4.3: Resolve compatibility warnings required for green build

Fix blocking compiler and runtime issues needed for pass criteria while deferring broad analyzer tightening.

Files:

* src/LegacyInventory.Core/Services/InventoryService.cs - Apply targeted compatibility fixes if required
* src/LegacyInventory.Core/Services/PricingCalculator.cs - Apply targeted compatibility fixes if required
* src/LegacyInventory.ConsoleApp/Program.cs - Apply targeted compatibility fixes if required

Discrepancy references:

* Mitigates B5 without expanding scope beyond required compile/runtime stability.

Success criteria:

* dotnet build passes with no errors.
* dotnet test passes with current test inventory.

Context references:

* .copilot-tracking/research/2026-07-02/net48-to-net10-modernization-research.md (Lines 94-100) - Analyzer/nullability debt risk

Dependencies:

* Step 4.2 complete.

### Step 4.4: Validate phase changes

Validation commands:

* dotnet build LegacyInventory.sln -c Debug
* dotnet test LegacyInventory.sln -c Debug

## Implementation Phase 5: End-to-End CLI and CI Hardening

<!-- parallelizable: true -->

### Step 5.1: Ensure solution and project metadata are CLI stable

Confirm solution traversal and project references work consistently in Debug and Release.

Files:

* LegacyInventory.sln - Update only if required by SDK migration output

Discrepancy references:

* Addresses DR-04 by preferring minimal solution file edits.

Success criteria:

* dotnet build LegacyInventory.sln -c Release succeeds.

Context references:

* LegacyInventory.sln (Lines 1-38) - Current project mapping

Dependencies:

* Phase 4 complete.

### Step 5.2: Add CI workflow for restore build and test

Create first-pass Windows workflow to align with in-repo constraints and reduce migration risk.

Files:

* .github/workflows/dotnet-ci.yml - New workflow for dotnet restore, build, and test

Discrepancy references:

* Mitigates B4 and implements end-to-end modernization gate.

Success criteria:

* Workflow validates on pull request trigger and manual run.

Context references:

* .copilot-tracking/research/2026-07-02/net48-to-net10-modernization-research.md (Lines 84-86) - No current GitHub workflow baseline

Dependencies:

* Phase 4 complete.

### Step 5.3: Update README to modern CLI commands

Replace Visual Studio path-bound commands with dotnet CLI commands and preserve run instructions.

Files:

* README.md - Update build and test sections for dotnet workflow

Discrepancy references:

* Mitigates B4 directly.

Success criteria:

* README commands run successfully on the validated implementation environment.

Context references:

* README.md (Lines 20-38) - Legacy build/test commands to replace

Dependencies:

* Steps 5.1 and 5.2 complete.

### Step 5.4: Validate phase changes

Validation commands:

* dotnet restore LegacyInventory.sln
* dotnet build LegacyInventory.sln -c Release
* dotnet test LegacyInventory.sln -c Release --no-build

## Implementation Phase 6: Final Validation and Review Gate

<!-- parallelizable: false -->

### Step 6.1: Run full validation matrix

Execute final build and test matrix for Debug and Release and compare with baseline counts.

Validation commands:

* dotnet restore LegacyInventory.sln
* dotnet build LegacyInventory.sln -c Debug
* dotnet test LegacyInventory.sln -c Debug --no-build
* dotnet build LegacyInventory.sln -c Release
* dotnet test LegacyInventory.sln -c Release --no-build

Success criteria:

* Full matrix passes.
* Test count parity with baseline inventory remains intact.

### Step 6.2: Fix minor validation issues

Apply isolated fixes that do not alter architecture or migration scope.

Success criteria:

* Remaining issues are either resolved or explicitly escalated as blockers.

### Step 6.3: Review and implementation handoff

Compile final phase risk outcomes, unresolved follow-on work, and acceptance evidence for review stage.

Success criteria:

* Change log contains traceable evidence to each Definition of Done item.

## Dependencies

* .NET 10 SDK installed and pinned
* Ability to run dotnet CLI locally and in CI

## Success Criteria

* Each phase has actionable steps, concrete files, and validation commands.
* Blocker mitigations map one-to-one to implementation actions.
* End-to-end validation gate is executable without Visual Studio path-bound tooling.
