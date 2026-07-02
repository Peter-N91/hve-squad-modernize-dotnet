---
title: Dependency Trust Policy
description: Baseline dependency trust and review controls for LegacyInventory modernization.
author: GitHub Copilot
ms.date: 2026-07-02
ms.topic: reference
---

## Scope

This policy applies to all dependencies used by this repository, including direct and transitive NuGet packages and GitHub Actions used in workflows.

## Trust Requirements

* Dependencies must come from trusted upstreams.
* NuGet dependencies must resolve from official sources configured by the .NET tooling in this environment.
* GitHub Actions dependencies must be pinned to full commit SHAs.
* New dependencies must be explicitly reviewed in pull requests.

## Review Controls

* Pull requests must pass dependency review checks.
* Pull requests must pass static analysis checks for C# code and workflow security posture.
* Package upgrades must document rationale when they introduce new transitive dependencies.

## Approved Baseline for This Repository

* Test stack: Microsoft.NET.Test.Sdk, MSTest.TestFramework, MSTest.TestAdapter.
* Build stack: .NET SDK pinned in global.json.
* CI actions: actions/checkout and actions/setup-dotnet pinned to full SHAs.

## Exceptions

Any temporary exception requires a pull request note that includes:

1. Exception reason.
2. Risk assessment.
3. Expiration date.
4. Mitigation plan.
