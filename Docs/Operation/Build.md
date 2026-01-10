# Operations

This document explains build pipeline with possible nuances and subtleties.

---

# Build

There are 2 technical projects to build:
- PriceTracker/ — ASP.NET Core backend (.NET Core 8)
- PriceTrackerWebInterface/ — Angular frontend

To build Angular frontend, you'll need...

### Angular Build
- Output directory: `dist/price-tracker-web-interface/browser/`  
- Its contents need to be copied to backend directory `PriceTracker/wwwroot/`.

To build ASP.NET Core backend, you'll need...

