using Microsoft.AspNetCore.Mvc.Filters;
using SkippableFilters.Core;
using SkippableFilters.Enums;

namespace SkippableFilters.Result;

/// <summary>
/// Provides an asynchronous base implementation of <see cref="IAsyncResultFilter"/>
/// with built-in support for conditional execution skipping.
/// </summary>
/// <remarks>
/// This filter allows derived classes to implement only the core resource pipeline logic
/// without manually handling skip conditions or pipeline continuation.
/// <para>
/// When execution is not skipped, the filter participates in the ASP.NET Core result
/// execution pipeline and is responsible for invoking next to continue execution.
/// </para>
/// <para>
/// If execution is skipped, the filter bypasses the derived logic and immediately
/// invokes next to continue the pipeline.
/// </para>
/// </remarks>
public abstract class AsyncSkippableResultFilter
    : SkippableFilterBase<ResultExecutingContext>, IAsyncResultFilter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncSkippableResultFilter"/> class.
    /// </summary>
    /// <param name="skipMode">
    /// Determines how the filter decides whether it should be skipped.
    /// </param>
    protected AsyncSkippableResultFilter(SkipMode skipMode = SkipMode.Never)
        : base(skipMode) { }
    
    /// <summary>
    /// Executes the result filter asynchronously as part of the ASP.NET Core pipeline.
    /// </summary>
    /// <param name="context">
    /// The current <see cref="ResultExecutingContext"/>.
    /// </param>
    /// <param name="next">
    /// A delegate representing the next step in the filter pipeline.
    /// </param>
    /// <remarks>
    /// This method is invoked only when the filter is not skipped.
    /// <para>
    /// Derived implementations must invoke <paramref name="next"/> to continue the pipeline.
    /// Failure to do so will short-circuit the request pipeline.
    /// </para>
    /// </remarks>
    async Task IAsyncResultFilter.OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (SkipExecution(context))
        {
            await next();
            return;
        }

        await this.OnResultExecutionAsync(context, next);
    }

    /// <summary>
    /// Executes the custom asynchronous result filter logic.
    /// </summary>
    /// <param name="context">
    /// The current <see cref="ResultExecutingContext"/>.
    /// </param>
    /// <param name="next">
    /// A delegate representing the next step in the filter pipeline.
    /// It must be invoked to continue execution.
    /// </param>
    protected abstract Task OnResultExecutionAsync(ResultExecutingContext context,
        ResultExecutionDelegate next);
}