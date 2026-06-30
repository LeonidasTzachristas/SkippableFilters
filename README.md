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

## 🛠️ Basic Usage

### 1. Create your own filter by inheriting from one of the provided base classes.


#### Tips:
- Each filter should expose a constructor that accepts a `SkipMode` and forwards it to the base class. Using `SkipMode.Never` as the default is recommended.
- Each skippable filter follows the same pattern as the corresponding ASP.NET Core filter. The only addition is the configurable `SkipMode` and optional custom skip logic through `ShouldSkip()`.


### Sync version:
```csharp
public class MyFilter : SkippableActionFilter
{
    public MyFilter(SkipMode skipMode = SkipMode.Never)
        : base(skipMode)
    {
    }

    protected override void OnActionExecuting(ActionExecutingContext context)
    {
        // Custom logic to execute before action
    }

    protected override void OnActionExecuted(ActionExecutedContext context)
    {
        // Custom logic to execute after action
    }
    
    protected override bool ShouldSkip(ActionExecutingContext context)
    {
        // Custom logic for skipping
    }
}
```
> **Note**
> 
> - `ShouldSkip()` is only required when using `SkipMode.Custom`
> - `OnActionExecuting()` overriding is required
> - `OnActionExecuted()` overriding is optional

### Async version:
```csharp
public class MyFilter : AsyncSkippableActionFilter
{
    public MyFilter(SkipMode skipMode = SkipMode.Never)
        : base(skipMode)
    {
    }

    protected override async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        // Before action logic
        
        await next();

        // After action logic
    }
    
    protected override bool ShouldSkip(ActionExecutingContext context)
    {
        // Custom logic for skipping
    }
}
```
> **Important**
>
> - Always invoke `next()` unless you intentionally want to short-circuit the request pipeline.
> - The async filter follows the same `SkipMode` behavior as the sync version.

### 2. Apply the filter

- Example with no arguments using the default `SkipMode`
```csharp
[TypeFilter<MyFilter>]
public IActionResult Index()
{
    return Ok();
}
```

- Example with arguments and Custom skip logic
```csharp
// Additional arguments are passed to the filter constructor
[TypeFilter<MyFilter>(Arguments = [SkipMode.Custom, arg2, arg3])]
public IActionResult Index()
{
    return Ok();
}
```

---

## 📦 Installation

```bash
dotnet add package SkippableFilters

