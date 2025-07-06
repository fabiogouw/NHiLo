# Architectural Decision Record: Split NHiLo into Repository-Specific Libraries

## Status
Accepted

## Context
The NHiLo project originally provided a single library implementing the HiLo algorithm for .NET, supporting multiple database providers (SQL Server, MySQL, Oracle, SQLite, etc.) within the same package. This approach led to several issues:

- Increased package size and unnecessary dependencies for consumers who only needed support for a specific database.
- Difficulty in maintaining and updating provider-specific code independently.
- Challenges in aligning with best practices for modularity and NuGet package management.

## Decision
The project has been refactored to split the original NHiLo library into multiple repository-specific libraries. Each supported database provider now has its own dedicated NuGet package and project:

- `NHiLo` (core logic, provider-agnostic)
- `NHilLo.Repository.MsSql`
- `NHilLo.Repository.MySql`
- `NHilLo.Repository.Oracle`
- `NHilLo.Repository.SQLite`

Each repository package contains only the code and dependencies required for its respective database provider. The core NHiLo package remains lightweight and free of direct database dependencies.

## Consequences
- Consumers can now reference only the packages relevant to their database technology, reducing bloat and potential conflicts.
- Provider-specific code can be maintained, tested, and released independently, improving maintainability and release agility.
- NuGet package generation is now controlled per repository project, with appropriate metadata and assets.
- The core NHiLo library is now more modular and easier to extend for new providers in the future.

## Related Changes
- Each repository project includes its own `.csproj` with NuGet packaging metadata.
- The solution structure has been updated to reflect the new modular organization.
- Tests and integration tests have been updated or added for each provider as appropriate.

---
Date: 2024-06-28
