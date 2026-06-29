using Microsoft.AspNetCore.Mvc.Filters;
using SkippableFilters.Core;
using SkippableFilters.Enums;

namespace SkippableFilters.Action;

/// <summary>
/// Provides an asynchronous base implementation of <see cref="IAsyncActionFilter"/>
/// with built-in support for conditional execution skipping.
/// </summary>
/// <remarks>
/// This filter allows derived classes to implement only the core action pipeline logic
/// without manually handling skip conditions or pipeline continuation.
/// <para>
/// When execution is not skipped, the filter participates in the ASP.NET Core action
/// execution pipeline and is responsible for invoking next to
/// continue execution.
/// </para>
/// <para>
/// If execution is skipped, the filter bypasses the derived logic and immediately
/// invokes next to continue the pipeline.
/// </para>
/// </remarks>
public abstract class AsyncSkippableActionFilter 
    : SkippableFilterBase<ActionExecutingContext>, IAsyncActionFilter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncSkippableActionFilter"/> class.
    /// </summary>
    /// <param name="skipMode">
    /// Determines how the filter decides whether it should be skipped.
    /// </param>
    protected AsyncSkippableActionFilter(SkipMode skipMode = SkipMode.Never)
        : base(skipMode) { }

    /// <summary>
    /// Executes the action filter asynchronously as part of the ASP.NET Core pipeline.
    /// </summary>
    /// <param name="context">
    /// The current <see cref="ActionExecutingContext"/>.
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
    async Task IAsyncActionFilter.OnActionExecutionAsync(
        ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (SkipExecution(context))
        {
            await next();
            return;
        }

        await this.OnActionExecutionAsync(context, next);
    }

    /// <summary>
    /// Executes the custom asynchronous action filter logic.
    /// </summary>
    /// <param name="context">
    /// The current <see cref="ActionExecutingContext"/>.
    /// </param>
    /// <param name="next">
    /// A delegate representing the next step in the filter pipeline.
    /// It must be invoked to continue execution.
    /// </param>
    protected abstract Task OnActionExecutionAsync(ActionExecutingContext context, 
        ActionExecutionDelegate next);
}