# Project Structure

This document describes the technical structure of the PriceTracker project
and how its source code is organized.

## Technical Projects

- PriceTracker/ — ASP.NET Core backend (.NET Core 8)
- PriceTrackerWebInterface/ — Angular frontend

## Backend Project Structure

- Modules/ — feature-oriented modules
- wwwroot/ — frontend static assets
- Migrations/ — EF Core migrations

### Key files
- `Program.cs` – entry point, configures services and middleware.
- `appsettings.json` – default configuration (logging, adjusting upsertion, etc.).
- `PriceTracker.csproj` – project definition.

## Frontend Project Structure

- src/app/ — application logic (angular modules and components)
- src/assets/ — static assets

---