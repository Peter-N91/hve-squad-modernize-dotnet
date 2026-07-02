---
title: LegacyInventory net48 to net10 modernization research
description: Read-only baseline assessment for in-place modernization from .NET Framework 4.8 to .NET 10.
author: GitHub Copilot
ms.date: 2026-07-02
ms.topic: reference
---

<!-- markdownlint-disable-file -->

## Scope and Method

This artifact captures a read-only baseline analysis for in-place modernization of LegacyInventory from .NET Framework 4.8 to .NET 10. Findings are based on direct inspection of solution, project, source, and repository configuration files.

Inspected evidence:

* LegacyInventory.sln:1-38
* src/LegacyInventory.Core/LegacyInventory.Core.csproj:1-45
* src/LegacyInventory.ConsoleApp/LegacyInventory.ConsoleApp.csproj:1-53
* tests/LegacyInventory.Tests/LegacyInventory.Tests.csproj:1-56
* tests/LegacyInventory.Tests/InventoryServiceTests.cs:1-122
* tests/LegacyInventory.Tests/PricingCalculatorTests.cs:1-84
* src/LegacyInventory.ConsoleApp/App.config:1-6
* src/LegacyInventory.ConsoleApp/Program.cs:1-184
* src/LegacyInventory.Core/Services/InventoryService.cs:1-107
* src/LegacyInventory.Core/Services/PricingCalculator.cs:1-81
* README.md:1-46

## 1) Solution and Project Inventory

### Solution

* Solution file: LegacyInventory.sln
* Projects in scope: 3

### Project inventory and current target frameworks

| Project path | Type | Current framework | Project format |
| --- | --- | --- | --- |
| src/LegacyInventory.Core/LegacyInventory.Core.csproj | Class library | .NET Framework 4.8 (`<TargetFrameworkVersion>v4.8</TargetFrameworkVersion>`) | Classic non-SDK style (`ToolsVersion="15.0"`) |
| src/LegacyInventory.ConsoleApp/LegacyInventory.ConsoleApp.csproj | Console executable | .NET Framework 4.8 (`<TargetFrameworkVersion>v4.8</TargetFrameworkVersion>`) | Classic non-SDK style (`ToolsVersion="15.0"`) |
| tests/LegacyInventory.Tests/LegacyInventory.Tests.csproj | Test library | .NET Framework 4.8 (`<TargetFrameworkVersion>v4.8</TargetFrameworkVersion>`) | Classic non-SDK style with legacy test project GUIDs |

## 2) Package and Dependency Inventory with net10 Compatibility Signals

### External package inventory

* No `PackageReference` entries found in any project.
* No `packages.config` or `NuGet.config` found at repository level.
* Dependency posture is mostly framework/BCL references and one Visual Studio GAC-style test dependency.

### Current references and likely net10 compatibility

| Dependency/reference | Where found | Likely net10 compatibility | Notes |
| --- | --- | --- | --- |
| `System`, `System.Core`, `System.Data`, `System.Xml` assembly references | All 3 `.csproj` files | Medium to High | Code usage appears basic and should map to modern BCL APIs after SDK-style conversion. Manual reference items should be removed in favor of SDK defaults. |
| `Microsoft.VisualStudio.QualityTools.UnitTestFramework` (GAC/VS install path) | tests/LegacyInventory.Tests/LegacyInventory.Tests.csproj:36-39 | Low | Not suitable for .NET 10. Must migrate to NuGet-based framework (MSTest v2/v3, xUnit, or NUnit) with `Microsoft.NET.Test.Sdk`. |
| Project reference (`ConsoleApp` -> `Core`) | src/LegacyInventory.ConsoleApp/LegacyInventory.ConsoleApp.csproj:46-51 | High | Compatible pattern, needs SDK-style `ProjectReference` conversion. |
| `App.config` startup runtime declaration for `.NETFramework,Version=v4.8` | src/LegacyInventory.ConsoleApp/App.config:1-6 | Low | Legacy runtime declaration is not valid for .NET 10 target model. Requires config strategy update or removal. |

## 3) Build, Test, and Tooling Constraints

### Build model constraints

* Projects use classic MSBuild XML format with `ToolsVersion="15.0"` and `Microsoft.CSharp.targets` imports.
* Current documented build path is Visual Studio MSBuild executable with explicit `VsInstallRoot` (README.md:21-26).
* No `global.json` exists, so SDK pinning is absent.
* No repository-level `Directory.Build.props` or `Directory.Build.targets` exists for shared modernization settings.

### Test model constraints

* Tests are MSTest v1 attribute-based (`[TestClass]`, `[TestMethod]`, `[ExpectedException]`, `[TestInitialize]`) and import `Microsoft.VisualStudio.TestTools.UnitTesting`.
* Test project depends on Visual Studio PublicAssemblies path and `VsInstallRoot` semantics.
* Current test execution is documented through `vstest.console.exe` with `.NETFramework,Version=v4.8` (README.md:34-38).
* Approximate baseline test count: 20 methods across two files.

### CI assumptions

* No `.github/workflows` definitions found in repository root.
* Current runbook assumes Windows + Visual Studio full install, not portable `dotnet build` and `dotnet test` flows.

## 4) Ranked Blockers and Prerequisites

### High

1. Legacy project system conversion required before retargeting.
   * Evidence: all `.csproj` files are non-SDK style with `TargetFrameworkVersion` and `ToolsVersion="15.0"`.
   * Why high: `.NET 10` targeting requires SDK-style projects and `TargetFramework` semantics.
2. Test framework hard dependency on MSTest v1 GAC assembly.
   * Evidence: tests/LegacyInventory.Tests/LegacyInventory.Tests.csproj:36-39.
   * Why high: blocks `dotnet test` and cross-platform modern pipelines until replaced.
3. Runtime config tied to .NET Framework 4.8.
   * Evidence: src/LegacyInventory.ConsoleApp/App.config:1-6.
   * Why high: startup runtime entry is framework-specific and not portable to net10 runtime model.

### Medium

1. Build process currently Visual Studio path-dependent.
   * Evidence: README.md build and test commands rely on fixed VS installation paths.
   * Why medium: migration can proceed, but reproducibility and automation are constrained until modern CLI workflow is introduced.
2. Legacy language patterns may generate analyzers/nullability debt when enabling modern defaults.
   * Evidence: code uses pre-C# 8 style null handling and string exception argument naming.
   * Why medium: not a blocker for first compile, but relevant for quality gates in .NET 10 adoption.

### Low

1. No third-party NuGet package dependency graph to untangle.
   * Evidence: no `PackageReference` and no `packages.config` found.
   * Why low: reduces compatibility matrix complexity.
2. Core business logic appears API-conservative and not dependent on deprecated framework-only subsystems.
   * Evidence: source scan found no `System.Web`, remoting, registry, or binary formatter usage in business and console paths inspected.

### Prerequisites before implementation

* Decide test framework destination (MSTest v2/v3 recommended for lowest rewrite, xUnit acceptable).
* Decide migration strategy for `App.config` settings and runtime startup metadata.
* Establish minimum supported .NET SDK and pin with `global.json` once target SDK is finalized.
* Define pipeline baseline (Windows-only first pass or immediate cross-platform build validation).

## 5) Recommended Migration Sequence for In-Place Modernization

1. Baseline safety net and branch policy.
   * Capture current green build and test run on net48.
   * Export test inventory and expected outcomes as migration acceptance baseline.
2. Convert project files to SDK style without changing runtime behavior.
   * Convert `Core`, `ConsoleApp`, and `Tests` projects to SDK-style format.
   * Keep temporary target at `net48` if needed for incremental confidence.
3. Modernize test infrastructure first.
   * Replace MSTest v1 GAC reference with NuGet-based test framework and `Microsoft.NET.Test.Sdk`.
   * Update assertions/attributes where needed and validate test pass parity.
4. Introduce modern CLI build contract.
   * Add `global.json` for SDK pinning.
   * Validate `dotnet restore`, `dotnet build`, and `dotnet test` for entire solution.
5. Retarget to net10 in phases.
   * Retarget `LegacyInventory.Core` then `LegacyInventory.ConsoleApp`, then tests.
   * Resolve compile-time API and analyzer changes per project in isolation.
6. Replace framework-specific runtime configuration.
   * Remove or migrate `App.config` startup metadata and port settings to modern configuration pattern where required.
7. Harden for production readiness.
   * Enable nullable and modern analyzer levels in controlled steps.
   * Add CI workflow for deterministic restore/build/test on hosted agents.

## 6) Executive Summary for Planning Stage

The repository is a focused, low-complexity modernization candidate because it has only three projects and no third-party NuGet dependency graph. The migration is feasible in-place, but there are two mandatory structural transitions before any net10 retarget can succeed: convert all projects from classic to SDK-style, and replace MSTest v1 GAC-based testing with a NuGet-based test stack.

Risk is concentrated in build and test infrastructure rather than business logic. The source itself uses conservative APIs and should port with limited code churn after project and test scaffolding are modernized. Recommended plan is phased, beginning with project system conversion and test framework migration, followed by net10 retargeting and CI hardening.

Planning readiness verdict: Go for planning with preconditions. Do not start implementation until test framework destination and SDK pinning approach are confirmed.