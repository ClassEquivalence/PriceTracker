
## Data Flow Overview

1. **Scraper (MerchDataUpserter)** fetches product data from external shop (e.g., Citilink).  
2. **Repository** persists new data into PostgreSQL via EF Core.  
3. **WebInterface** exposes stored data to the frontend via REST endpoints.  
4. **Frontend (Angular)** requests data and renders charts, tables, and product pages.  


- **Modules/**  
  Organized by feature (modular monolith style):
  - `MerchDataUpserter/` – background service that periodically fetches and updates product data from shops.
  - `Repository/` – data access layer built on Entity Framework Core (PostgreSQL).
  - `WebInterface/` – controllers and HTTP endpoints.