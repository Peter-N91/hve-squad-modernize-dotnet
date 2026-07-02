# Legacy Inventory Manager

A small, intentionally **legacy** .NET application to serve as a modernization target.

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
