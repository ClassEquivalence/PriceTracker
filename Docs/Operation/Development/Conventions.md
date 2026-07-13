# Conventions

Below you'll find:

1. [Project conventions](#conventions)
2. [C# backend conventions](#csharp-backend-conventions)
3. [Documentation conventions](#docs-conventions)
4. What should be documented [in-code](#in-code) and what [in-docs](#in-docs)
5. [Main documentation categories](#docs-categories)
6. [Architectural category](#docs-arch-category)
7. [Technical category](#docs-tech-category)
8. [Operational category](#docs-oper-category)

# Project conventions

<a name="conventions"></a>

The design pattern chosen for this app is the *modular monolith*.

<a name="csharp-backend-conventions"></a>

# C# Backend conventions

*Functional modules* (see Glossary.md for definition) interact with each other through classes and interfaces named after the pattern:
- SomeForeignModuleName_Adapter - for using other modules within the current one,
- ThisModule_Facade - for using current module within other ones.

<a name="docs-conventions"></a>

# Documentation conventions

Info related to organizing specific document should be placed in respective document's header.

## Code or doc files?

<a name="in-code"></a>

What should be documented in-code:
1. TODO-Lists with plans related to specific module
2. Modules behaviour (speaking of facade methods and adapters)
3. Behaviour of entities within module, within core etc
4. Minor code notes

<a name="in-docs"></a>

What should be documented in Docs section:
1. App architecture (including data flow, modules responsibility boundaries)
2. Folders and files location with their technical purpose
3. Code and docs writing and organization approach
4. Consistency lists
5. Change guidelines
6. TODO-Lists related to docs or general app development plan

<a name="docs-categories"></a>

## Documentation categories

There are 3 main categories of documentation: Operational, Architectural and Technical.
Definitions for these categories and their subcatigories live in [Glossary](../Glossary.md).

<a name="docs-arch-category"></a>

## Architectural category asnwers:
1. How are system's functional modules segregated and organized
2. How do system's functional modules communicate with each other
3. How should developers view functional modules organization

<a name="docs-tech-category"></a>

## Technical category answers:
1. How are system's components organized technically (what folders do different 
technical modules lie in?)
2. Technical nuances and subtleties of application

<a name="docs-oper-category"></a>

## Operational category 
Operational category answers how to properly operate with app on different lifecycle stages.

Operational category is split into 3 subcategories based on lifecycle stages:
- Development
- Build
- Deploy and utilization (it can be splitted into admin and user guides)

### Development subcategory

Development subcategory contains guidelines, nuances, subtleties and tips needed for proper development of project.

### Build subcategory

Similarly, build subcategory - for proper organization and build of app's components. 

### Deploy and utilization subcategory

Similarly, deploy and utilization - for proper app deployment and utilizing.
Deploy and utilization subcategory may be splitted into admin and user intended guides.
