<!-- markdownlint-disable-file -->
# Release Changes: net48-to-net10-modernization-in-place

**Related Plan**: net48-to-net10-modernization-in-place-plan.instructions.md
**Implementation Date**: 2026-07-02

## Summary

Modernized LegacyInventory in-place from .NET Framework 4.8 to .NET 10 by converting all projects to SDK-style, migrating MSTest v1/GAC test infrastructure to NuGet-based MSTest, replacing Visual Studio path-bound build and test instructions with dotnet CLI workflows, and adding SDK governance and security governance artifacts.

## Changes

### Added

* global.json - Pins .NET SDK feature band for deterministic CLI builds.
* .github/workflows/dotnet-ci.yml - Adds restore/build/test CI workflow using pinned GitHub Actions SHAs.
* .github/workflows/security-scanning.yml - Adds SAST (CodeQL) and SCA (Dependency Review) workflow to satisfy council conditions.
* docs/security/dependency-trust-policy.md - Documents dependency trust and review controls.

### Modified

* src/LegacyInventory.Core/LegacyInventory.Core.csproj - Converted from classic non-SDK format to SDK-style and retargeted to net10.0.
* src/LegacyInventory.ConsoleApp/LegacyInventory.ConsoleApp.csproj - Converted from classic non-SDK format to SDK-style, preserved Core project reference, retargeted to net10.0, and removed empty App.config metadata.
* tests/LegacyInventory.Tests/LegacyInventory.Tests.csproj - Converted from classic non-SDK format to SDK-style, removed MSTest v1 GAC dependency, added modern MSTest package references, retargeted to net10.0.
* src/LegacyInventory.ConsoleApp/App.config - Removed .NET Framework startup metadata incompatible with .NET 10.
* src/LegacyInventory.ConsoleApp/Program.cs - Updated runtime banner to .NET 10.
* tests/LegacyInventory.Tests/InventoryServiceTests.cs - Replaced legacy ExpectedException attributes with Assert.ThrowsExactly for modern MSTest compatibility.
* tests/LegacyInventory.Tests/PricingCalculatorTests.cs - Replaced legacy ExpectedException attributes with Assert.ThrowsExactly for modern MSTest compatibility.
* README.md - Updated build/run/test documentation to dotnet CLI and modernized stack description.
* .github/workflows/dotnet-ci.yml - Added push trigger coverage for main branch CI validation.
* .github/workflows/security-scanning.yml - Pinned .NET SDK setup via global.json before CodeQL build.

### Removed

* None.

## Additional or Deviating Changes

* Plan called for temporary SDK-style conversion at net48 before retargeting to net10.
  * Implementation consolidated conversion + retarget in one controlled change per project to reduce churn, while preserving validation at each phase boundary.
* Plan suggested Windows-first CI runner.
  * Workflow uses ubuntu-latest to align with repository workflow conventions requiring GitHub-hosted Ubuntu runners.
* Council conditions required security scanning and dependency trust policy before implementation completion.
  * Added dedicated security workflow and policy document during implementation to close those conditions.
* Post-review remediation closed the remaining workflow governance and legacy metadata findings.
  * Added main push trigger coverage to CI, aligned CodeQL build with global.json SDK pinning, and removed the leftover empty App.config project include.

## Validation Results

Baseline (pre-migration):

* Legacy MSBuild command from README: Passed.
* Legacy vstest.console.exe command from README: Passed, 20/20 tests.

Post-migration validation:

* dotnet --info: Passed, .NET SDK 10.0.301 available.
* dotnet --version: Passed, 10.0.301.
* dotnet restore LegacyInventory.sln: Passed.
* dotnet build LegacyInventory.sln -c Debug: Passed.
* dotnet test tests/LegacyInventory.Tests/LegacyInventory.Tests.csproj -c Debug: Passed, 20/20 tests.
* dotnet build LegacyInventory.sln -c Release: Passed.
* dotnet test LegacyInventory.sln -c Release --no-build: Passed, 20/20 tests.
* dotnet test LegacyInventory.sln -c Debug --no-build: Passed, 20/20 tests.

## Release Summary

Total files affected: 11

* Added: 4 files
* Modified: 7 files
* Removed: 0 files

Key outcomes:

* All projects now target net10.0 using SDK-style csproj format.
* Legacy MSTest v1 GAC dependency removed and replaced with NuGet-based MSTest tooling.
* Build and test workflows now use dotnet CLI locally and in CI.
* SDK governance and security governance conditions from council verdict are implemented in-repo.
