using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using SkippableFilters.Core;
using SkippableFilters.Enums;

namespace SkippableFilters.Result;

/// <summary>
/// Provides a base implementation of <see cref="IResultFilter"/> with built-in support
/// for conditional execution skipping.
/// </summary>
/// <remarks>
/// This filter allows derived classes to implement only the relevant result logic
/// without manually handling skip conditions or pipeline continuation.
/// <para>
/// When a filter execution is skipped during <see cref="IResourceFilter.OnResourceExecuting(ResourceExecutingContext)"/>,
/// a marker is stored in <see cref="HttpContext.Items"/> to ensure the corresponding
/// <see cref="OnResultExecuted(ResultExecutedContext)"/> phase is also skipped.
/// </para>
/// <example>
/// Example of derived filter constructor:
/// <code>
/// public class MyFilter : SkippableResultFilter
/// {
///     public MyFilter(SkipMode skipMode = SkipMode.Never) 
///         : base(skipMode) { }
/// 
///     protected override void OnResultExecuting(ResultExecutingContext context)
///     {
///         // logic here
///     }
/// }
/// </code>
/// </example>
/// </remarks>
public abstract class SkippableResultFilter
    : SkippableFilterBase<ResultExecutingContext>, IResultFilter
{
    /// <summary>
    /// A private key used to track whether the current filter execution was skipped.
    /// </summary>
    /// <remarks>
    /// Stored in <see cref="HttpContext.Items"/> to ensure per-request isolation
    /// between <see cref="OnResultExecuting(ResultExecutingContext)"/> and
    /// <see cref="OnResultExecuted(ResultExecutedContext)"/>.
    /// </remarks>
    private static readonly object SkipKey = new();
    
    /// <summary>
    /// Initializes a new instance of the <see cref="SkippableResultFilter"/> class.
    /// </summary>
    /// <param name="skipMode">
    /// Determines how the filter decides whether it should be skipped.
    /// </param>
    protected SkippableResultFilter(SkipMode skipMode = SkipMode.Never)
        : base(skipMode) { }

    /// <summary>
    /// Called by the ASP.NET Core pipeline before the result pipeline executes.
    /// </summary>
    /// <param name="context">
    /// The current <see cref="ResultExecutingContext"/> for the HTTP request.
    /// </param>
    /// <remarks>
    /// If the filter is skipped, a marker is stored in
    /// <see cref="HttpContext.Items"/> to ensure that the
    /// <see cref="OnResultExecuted(ResultExecutedContext)"/> phase is also skipped.
    /// </remarks>
    void IResultFilter.OnResultExecuting(ResultExecutingContext context)
    {
        if (SkipExecution(context))
        {
            context.HttpContext.Items[SkipKey] = true;
            return;
        }

        this.OnResultExecuting(context);
    }

    /// <summary>
    /// Called by the ASP.NET Core pipeline after the result pipeline has executed.
    /// </summary>
    /// <param name="context">
    /// The current <see cref="ResultExecutedContext"/> for the HTTP request.
    /// </param>
    /// <remarks>
    /// This method is not executed if the corresponding
    /// <see cref="OnResultExecuting(ResultExecutingContext)"/> phase was skipped.
    /// </remarks>
    void IResultFilter.OnResultExecuted(ResultExecutedContext context)
    {
        if ((bool?)context.HttpContext.Items[SkipKey] == true)
            return;
        
        this.OnResultExecuted(context);
    }

    /// <summary>
    /// Executes logic before the result pipeline continues.
    /// </summary>
    /// <param name="context">
    /// The current <see cref="ResultExecutingContext"/>.
    /// </param>
    protected abstract void OnResultExecuting(ResultExecutingContext context);
    
    /// <summary>
    /// Executes logic after the result pipeline completes.
    /// </summary>
    /// <param name="context">
    /// The current <see cref="ResultExecutedContext"/>.
    /// </param>
    protected virtual void OnResultExecuted(ResultExecutedContext context) { }
}