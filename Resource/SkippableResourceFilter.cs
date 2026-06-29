using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using SkippableFilters.Core;
using SkippableFilters.Enums;

namespace SkippableFilters.Resource;

/// <summary>
/// Provides a base implementation of <see cref="IResourceFilter"/> with built-in support
/// for conditional execution skipping.
/// </summary>
/// <remarks>
/// This filter allows derived classes to implement only the relevant action logic
/// without manually handling skip conditions or pipeline continuation.
/// <para>
/// When a filter execution is skipped during <see cref="IResourceFilter.OnResourceExecuting(ResourceExecutingContext)"/>,
/// a marker is stored in <see cref="HttpContext.Items"/> to ensure the corresponding
/// <see cref="OnResourceExecuted(ResourceExecutedContext)"/> phase is also skipped.
/// </para>
/// <example>
/// Example of derived filter constructor:
/// <code>
/// public class MyFilter : SkippableResourceFilter
/// {
///     public MyFilter(SkipMode skipMode = SkipMode.Never) 
///         : base(skipMode) { }
/// 
///     protected override void OnResourceExecuting(ResourceExecutingContext context)
///     {
///         // logic here
///     }
/// }
/// </code>
/// </example>
/// </remarks>
public abstract class SkippableResourceFilter
    : SkippableFilterBase<ResourceExecutingContext>, IResourceFilter
{
    /// <summary>
    /// A private key used to track whether the current filter execution was skipped.
    /// </summary>
    /// <remarks>
    /// Stored in <see cref="HttpContext.Items"/> to ensure per-request isolation
    /// between <see cref="OnResourceExecuting(ResourceExecutingContext)"/> and
    /// <see cref="OnResourceExecuted(ResourceExecutedContext)"/>.
    /// </remarks>
    private static readonly object SkipKey = new();
    
    /// <summary>
    /// Initializes a new instance of the <see cref="SkippableResourceFilter"/> class.
    /// </summary>
    /// <param name="skipMode">
    /// Determines how the filter decides whether it should be skipped.
    /// </param>
    protected SkippableResourceFilter(SkipMode skipMode)
        : base(skipMode) { }
    
    /// <summary>
    /// Called by the ASP.NET Core pipeline before the resource pipeline executes.
    /// </summary>
    /// <param name="context">
    /// The current <see cref="ResourceExecutingContext"/> for the HTTP request.
    /// </param>
    /// <remarks>
    /// If the filter is skipped, a marker is stored in
    /// <see cref="HttpContext.Items"/> to ensure that the
    /// <see cref="OnResourceExecuted(ResourceExecutedContext)"/> phase is also skipped.
    /// </remarks>
    void IResourceFilter.OnResourceExecuting(ResourceExecutingContext context)
    {
        if (SkipExecution(context))
        {
            context.HttpContext.Items[SkipKey] = true;
            return;
        }
        
        this.OnResourceExecuting(context);
    }

    /// <summary>
    /// Called by the ASP.NET Core pipeline after the resource pipeline has executed.
    /// </summary>
    /// <param name="context">
    /// The current <see cref="ResourceExecutedContext"/> for the HTTP request.
    /// </param>
    /// <remarks>
    /// This method is not executed if the corresponding
    /// <see cref="OnResourceExecuting(ResourceExecutingContext)"/> phase was skipped.
    /// </remarks>
    void IResourceFilter.OnResourceExecuted(ResourceExecutedContext context)
    {
        if ((bool?)context.HttpContext.Items[SkipKey] == true)
            return;
        
        this.OnResourceExecuted(context);
    }

    /// <summary>
    /// Executes logic before the resource pipeline continues.
    /// </summary>
    /// <param name="context">
    /// The current <see cref="ResourceExecutingContext"/>.
    /// </param>
    protected abstract void OnResourceExecuting(ResourceExecutingContext context);
    
    /// <summary>
    /// Executes logic after the resource pipeline completes.
    /// </summary>
    /// <param name="context">
    /// The current <see cref="ActionExecutedContext"/>.
    /// </param>
    protected virtual void OnResourceExecuted(ResourceExecutedContext context) { }
}