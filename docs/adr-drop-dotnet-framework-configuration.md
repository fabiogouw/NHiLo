# Architectural Decision Record: Drop of .NET Framework Configuration Support

## Status
Accepted

## Context
Historically, the NHiLo library supported configuration via the legacy .NET Framework configuration system (System.Configuration, app.config/web.config). This allowed users to configure NHiLo using XML-based configuration files, which was standard in .NET Framework applications.

With the evolution of .NET, modern applications (including .NET Core, .NET 5+, and .NET Standard projects) have adopted the Microsoft.Extensions.Configuration model, which is more flexible, supports multiple sources (JSON, environment variables, etc.), and is the recommended approach for new development.

Maintaining support for both configuration models increased code complexity and maintenance burden, and limited the ability to modernize the codebase.

## Decision
Support for .NET Framework configuration (System.Configuration) has been dropped from the NHiLo library. The library now exclusively supports configuration via Microsoft.Extensions.Configuration and related abstractions.

## Consequences
- Simplifies the codebase by removing legacy configuration code and dependencies.
- Reduces maintenance overhead and potential for configuration-related bugs.
- Encourages users to migrate to modern .NET configuration practices.
- NHiLo is now focused on .NET Standard 2.0+, .NET 6+, and .NET 8+ projects, and is not intended for legacy .NET Framework-only applications.
- Last version supporting .NET Framework configuration will be 2.4.0. Existing users relying on app.config/web.config must migrate their configuration to the supported model.

## Related Changes
- Removal of System.Configuration and related code from the core library.
- All configuration is now expected to be provided via Microsoft.Extensions.Configuration.
- Documentation and samples have been updated to reflect this change.

---
Date: 2024-06-28
