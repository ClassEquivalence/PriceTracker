# Project Structure

This document explains the high-level structure of the PriceTracker project.

---

## Root Directories

- **PriceTracker/** – Backend project written in ASP.NET Core (.NET 8).
- **PriceTrackerWebInterface/** – Angular frontend project.

---

## Backend (`PriceTracker/`)

The backend is an ASP.NET Core application.
It exposes REST endpoints, serves the Angular frontend from wwwroot/, and includes a background worker (Upserter) that uses HttpClient to periodically fetch and update shop data.

### Key directories
- **Modules/**  
  Organized by feature (modular monolith style):
  - `MerchDataUpserter/` – background service that periodically fetches and updates product data from shops.
  - `Repository/` – data access layer built on Entity Framework Core (PostgreSQL).
  - `WebInterface/` – controllers and HTTP endpoints.

### Upsertion
Upsertion module needs particular attention as it's the only non-completely-standard one across 3 modules.
For detailed information on **Upsertion module**, see [Upsertion.md](./Upsertion.md).

- **wwwroot/**  
  Contains built Angular static files after frontend build.

- **Migrations/**  
  Entity Framework Core migrations for database schema.

### Key files
- `Program.cs` – entry point, configures services and middleware.
- `appsettings.json` – default configuration (logging, adjusting upsertion, etc.).
- `PriceTracker.csproj` – project definition.

---

## Frontend (`PriceTrackerWebInterface/`)

Angular 20 application. Provides UI for browsing tracked products and their price history.

### Key directories
- **src/app/** – Angular modules and components.
- **src/assets/** – static resources (logos, icons, etc.).

### Build
- Output directory: `dist/price-tracker-web-interface/browser/`  
- Its contents need to be copied to backend directory `PriceTracker/wwwroot/`.

---

## Data Flow Overview

1. **Scraper (MerchDataUpserter)** fetches product data from external shop (e.g., Citilink).  
2. **Repository** persists new data into PostgreSQL via EF Core.  
3. **WebInterface** exposes stored data to the frontend via REST endpoints.  
4. **Frontend (Angular)** requests data and renders charts, tables, and product pages.  

---

## Notes

- Environment variables are used for DB connection (`ProductionDbOptions__*`).  
- Background workers may consume significant CPU; deployment should account for resource limits.  
- Static files in `wwwroot/` should not be committed to VCS (add to `.gitignore`).
