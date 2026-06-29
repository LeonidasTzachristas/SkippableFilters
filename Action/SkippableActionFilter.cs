using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using SkippableFilters.Core;
using SkippableFilters.Enums;

namespace SkippableFilters.Action;

/// <summary>
/// Provides a base implementation of <see cref="IActionFilter"/> with built-in support
/// for conditional execution skipping.
/// </summary>
/// <remarks>
/// This filter allows derived classes to implement only the relevant action logic
/// without manually handling skip conditions or pipeline continuation.
/// <para>
/// When a filter execution is skipped during <see cref="IActionFilter.OnActionExecuting(ActionExecutingContext)"/>,
/// a marker is stored in <see cref="HttpContext.Items"/> to ensure the corresponding
/// <see cref="IActionFilter.OnActionExecuted(ActionExecutedContext)"/> phase is also skipped.
/// </para>
/// <example>
/// Example of derived filter:
/// <code>
/// public class MyFilter : SkippableActionFilter
/// {
///     public MyFilter(SkipMode skipMode = SkipMode.Never) 
///         : base(skipMode) { }
///
///     protected override void OnActionExecuting(ActionExecutingContext context)
///     {
///         // logic here
///     }
/// }
/// </code>
/// </example>
/// </remarks>
public abstract class SkippableActionFilter 
    : SkippableFilterBase<ActionExecutingContext>, IActionFilter
{
    /// <summary>
    /// A private key used to track whether the current filter execution was skipped.
    /// </summary>
    /// <remarks>
    /// Stored in <see cref="HttpContext.Items"/> to ensure per-request isolation
    /// between <see cref="OnActionExecuting(ActionExecutingContext)"/> and
    /// <see cref="OnActionExecuted(ActionExecutedContext)"/>.
    /// </remarks>
    private static readonly object SkipKey = new();
    
    /// <summary>
    /// Initializes a new instance of the <see cref="SkippableActionFilter"/> class.
    /// </summary>
    /// <param name="skipMode">
    /// Determines how the filter decides whether it should be skipped.
    /// </param>
    protected SkippableActionFilter(SkipMode skipMode = SkipMode.Never)
        :base(skipMode) { }
    
    /// <summary>
    /// Called by the ASP.NET Core pipeline before the action executes.
    /// </summary>
    /// <param name="context">
    /// The current <see cref="ActionExecutingContext"/>.
    /// </param>
    /// <remarks>
    /// If the filter is skipped, a marker is stored in
    /// <see cref="HttpContext.Items"/> to ensure that the
    /// <see cref="OnActionExecuted(ActionExecutedContext)"/> phase is also skipped.
    /// </remarks>
    void IActionFilter.OnActionExecuting(ActionExecutingContext context)
    {
        if (SkipExecution(context))
        {
            context.HttpContext.Items[SkipKey] = true;
            return;
        }
        
        this.OnActionExecuting(context);
    }

    /// <summary>
    /// Called by the ASP.NET Core pipeline after the action executes.
    /// </summary>
    /// <param name="context">
    /// The current <see cref="ActionExecutedContext"/>.
    /// </param>
    /// <remarks>
    /// This method is not executed if the corresponding
    /// <see cref="OnActionExecuting(ActionExecutingContext)"/> phase was skipped.
    /// </remarks>
    void IActionFilter.OnActionExecuted(ActionExecutedContext context)
    {
        if ((bool?)context.HttpContext.Items[SkipKey] == true)
            return;
        
        this.OnActionExecuted(context);
    }

    /// <summary>
    /// Executes logic before the action pipeline continues.
    /// </summary>
    /// <param name="context">
    /// The current <see cref="ActionExecutingContext"/>.
    /// </param>
    protected abstract void OnActionExecuting(ActionExecutingContext context);
    
    /// <summary>
    /// Executes logic after the action pipeline completes.
    /// </summary>
    /// <param name="context">
    /// The current <see cref="ActionExecutedContext"/>.
    /// </param>
    protected virtual void OnActionExecuted(ActionExecutedContext context) { }
}