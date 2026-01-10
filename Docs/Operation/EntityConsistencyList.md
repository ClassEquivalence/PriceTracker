# Dependency Consistency List

This document lists entities and their dependencies that require synchronous modification.

## List maintenance principles

When modifying the code or documentation, we always make sure to check modifiable or added entity or dependency 
for consistent change with others described below.

An entity should be included in the consistency list if **both** of the following conditions are met:
1. Changing the entity requires synchronous updates to other entities or references, such as:
   - references in documentation;
   - references in code outside the current technical project.
2. Entity's dependencies cannot be **fully traced** by IDE tooling.
Typically, this applies to dependencies crossing technical project boundaries,
even when they belong to the same functional module.

For clarity, we distinguish different concepts:
- **technical projects** as build/deployment units;
- **functional modules** as business or responsibility boundaries.
Canonical definition for these terms lives [here](Glossary.md).

Most likely technical projects to watch for consistency are:
1. Assembly and DI process;
2. Unit test;
3. Other related modules support;
4. Frontend components

## List itself

