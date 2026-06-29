using Microsoft.AspNetCore.Mvc.Filters;
using SkippableFilters.Core;
using SkippableFilters.Enums;

namespace SkippableFilters.Exception;

/// <summary>
/// Provides an asynchronous base implementation of <see cref="IAsyncExceptionFilter"/>
/// with built-in support for conditional execution skipping.
/// </summary>
/// <remarks>
/// This filter allows derived classes to implement only the core exception logic
/// without manually handling skip conditions.
/// </remarks>
/// /// <example>
/// Example of derived filter:
/// <code>
/// public class MyFilter : AsyncSkippableExceptionFilter
/// {
///     public MyResourceFilter(SkipMode skipMode = SkipMode.Never)
///         : base(skipMode) { }
///     protected override async Task OnExceptionAsync(ExceptionContext context)
///     {
///         // logic here
///     }
/// }
/// </code>
/// </example>
public abstract class AsyncSkippableExceptionFilter
    : SkippableFilterBase<ExceptionContext>, IAsyncExceptionFilter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncSkippableExceptionFilter"/> class.
    /// </summary>
    /// <param name="skipMode">
    /// Determines how the filter decides whether it should be skipped.
    /// </param>
    protected AsyncSkippableExceptionFilter(SkipMode skipMode = SkipMode.Never)
        : base(skipMode) { }
    
    /// <summary>
    /// This method is invoked by ASP.NET Core during the exception phase.
    /// </summary>
    /// <param name="context">
    /// The current <see cref="ExceptionContext"/>.
    /// </param>
    async Task IAsyncExceptionFilter.OnExceptionAsync(ExceptionContext context)
    {
        if (SkipExecution(context))
            return;

        await this.OnExceptionAsync(context);
    }

    /// <summary>
    /// Executes the custom asynchronous exception filter logic.
    /// </summary>
    /// <param name="context">
    /// The current <see cref="ExceptionContext"/>.
    /// </param>
    protected abstract Task OnExceptionAsync(ExceptionContext context);
}