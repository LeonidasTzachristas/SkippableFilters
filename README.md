# SkippableFilters

A lightweight abstraction layer over ASP.NET Core filters that adds **conditional execution (skipping)** while keeping the original MVC pipeline behavior intact.

This library helps reduce boilerplate in filters by centralizing skip logic and providing clean base classes for all ASP.NET Core filter types.

---

## ✨ Features

- Conditional filter execution via `SkipMode`
- Consistent abstraction across all ASP.NET Core filter types:
    - Action filters
    - Authorization filters
    - Resource filters
    - Exception filters
    - Result filters
- Async and sync support
- Clean separation between pipeline logic and custom logic
- Minimal boilerplate for derived filters

---

## 📦 Installation

```bash
dotnet add package SkippableFilters