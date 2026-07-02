---
title: Legacy Inventory Manager
description: Legacy Inventory modernization sample upgraded from .NET Framework 4.8 to .NET 10 with squad autopilot.
---

# Legacy Inventory Manager

A small, intentionally **legacy** .NET application that was modernized in place.

## Modernization summary

This repository started on **.NET Framework 4.8** and was migrated to **.NET 10** using:

```text
/squad mode=autopilot request=Modernize this repository from .NET Framework 4.8 to .NET 10. Assess the current projects and dependencies, define the target state, and carry out the in-place upgrade end-to-end.
```

### Outcome

* Classification and dispatch matched modernize and ran with `mode=autopilot`, beginning in Init Mode.
* The squad dispatched researcher, lead, a pre-implementation council (architect, security, RAI), developer, and tester, while Squad Scribe recorded decisions and history.
* One Risk Gate was triggered when Security reported high-risk process blockers.
* Continuation was approved with conditions in advisory mode, and execution proceeded.
* The repository is modernized in place to .NET 10.
* Core projects are SDK-style and target `net10.0`.
* Tests were migrated from legacy MSTest v1/GAC wiring to NuGet-based MSTest.
* Build and test workflows moved to `dotnet` CLI with SDK pinning in `global.json`.
* Stale App.config project metadata was removed.
* CI and security workflow updates were added.

### Key upgraded files

* `src/LegacyInventory.Core/LegacyInventory.Core.csproj`
* `src/LegacyInventory.ConsoleApp/LegacyInventory.ConsoleApp.csproj`
* `tests/LegacyInventory.Tests/LegacyInventory.Tests.csproj`
* `README.md`
* `dotnet-ci.yml`
* `security-scanning.yml`

### Validation and review

* Implementation validation reported successful `dotnet restore`, Debug and Release builds, and 20/20 passing tests.
* Workspace diagnostics reported no errors.
* Review initially found two medium workflow gaps and one low metadata issue.
* Those findings were remediated.
* Final re-review returned no active findings.
* Final recommendation: **Go** for final-outcome validation.

### Supporting artifacts

Supporting squad artifacts were written under `.copilot-tracking`, including research, plan, changes, review, and autopilot history records.

## Stack (legacy on purpose)

- **.NET 10**
- **SDK-style `.csproj`** with modern `dotnet` CLI workflow
- **MSTest (NuGet-based)** via `Microsoft.NET.Test.Sdk`, `MSTest.TestFramework`, and `MSTest.TestAdapter`
- Built and tested with **dotnet CLI**

## Projects

| Project | Type | Description |
| --- | --- | --- |
| `src/LegacyInventory.Core` | Class library | Domain models and business logic (`Product`, `InventoryService`, `PricingCalculator`) |
| `src/LegacyInventory.ConsoleApp` | Console EXE | Interactive menu-driven front-end |
| `tests/LegacyInventory.Tests` | Test library | 20 MSTest unit tests |

## Build

```powershell
dotnet restore LegacyInventory.sln
dotnet build LegacyInventory.sln -c Debug
```

## Run the app

```powershell
dotnet run --project src/LegacyInventory.ConsoleApp/LegacyInventory.ConsoleApp.csproj
```

## Run the tests

```powershell
dotnet test LegacyInventory.sln -c Debug
```

## Ideas for modernization (for the hve-squad)

- Enable nullable reference types and incrementally harden analyzers
- Introduce dependency injection, async, nullable reference types, and structured logging
