# Upsertion Module

This document describes the **Upsertion** subsystem of PriceTracker.

---

## Purpose

The Upsertion module is responsible for:
- Periodically extracting product data from online shops
- Processing and validating extracted data
- Saving data into the repository (PostgreSQL)
- Maintaining **ExecutionState** (progress of last run)

---

## Core Concepts

### ScheduledUpserter
- The main orchestrator for shop data collection  
- Responsible for:
  - Running extraction on schedule
  - Coordinating **Extractor** and **Consumer**
  - Providing **ExecutionState** to dependent modules

⚠️ **Current flaw:**  
`ScheduledUpserter` both manages scheduling **and** owns execution state (SRP violation).  
Future refactor → move state management into a separate provider.

---

### Extractor
- Implements `IMerchDataExtractor<MerchParsedDto, ExecutionState>`
- Handles fetching and parsing product data from shop source
- Can be HTTP parser or API client depending on shop

### Consumer
- Implements `IMerchDataConsumer<MerchParsedDto>`
- Saves extracted data into repository (EF Core + PostgreSQL)
- Validates incoming products, timestamps, prices

---

## Execution State

- Tracks progress of extraction (e.g., last page visited, last product processed)
- Enables **resuming** after app restart or crash
- Currently managed inside `ScheduledUpserter`
- Saved into DB via Repository

Tip:  
When persisting execution state, always ensure it stays consistent between **Extractor** and **Consumer**, otherwise data may desynchronize.

---

## Lifecycle

1. Application starts  
2. `Modules/MerchDataUpserter/MerchDataProviderFacade` creates `ScheduledUpserter` instances (one per shop)  
3. Each upserter runs independently on its schedule  
4. After upsertion cycle/subcycle completes, event `UpsertionCycleCompleted` is triggered  
5. Updated data is committed into repository  

⚠️ **Current flaw:**  
`MerchDataProviderFacade` constructs necessary classes at StartAsync function whose primary responsibilty is to run background process (SRP violation).  
Future refactor → move construction of ScheduledUpserters into separate provider.

---

## Adding New Shop Support

1. Implement new **Extractor** (shop-specific)  
2. Implement new **Consumer** (or reuse base one if possible)  
3. Register new `ScheduledUpserter` for this shop in `MerchDataProviderFacade`  
4. Add shop-specific configuration (schedule, intervals, etc.)

---

## Planned Improvements

- Extract `ExecutionStateProvider` into its own module → repository handles only storage, not creation  
- Support parallel upsertion for multiple shops 
- Improve logging and monitoring
- Refactor towards DI-based service creation – reduce manual wiring, improve modularity
- Split repository layer into business-case-specific sub-repositories – improve separation of concerns and maintainability
