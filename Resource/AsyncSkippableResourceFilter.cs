using Microsoft.AspNetCore.Mvc.Filters;
using SkippableFilters.Core;
using SkippableFilters.Enums;

namespace SkippableFilters.Resource;

/// <summary>
/// Provides an asynchronous base implementation of <see cref="IAsyncResourceFilter"/>
/// with built-in support for conditional execution skipping.
/// </summary>
/// <remarks>
/// This filter allows derived classes to implement only the core resource pipeline logic
/// without manually handling skip conditions or pipeline continuation.
/// <para>
/// When execution is not skipped, the filter participates in the ASP.NET Core resource
/// execution pipeline and is responsible for invoking next to continue execution.
/// </para>
/// <para>
/// If execution is skipped, the filter bypasses the derived logic and immediately
/// invokes next to continue the pipeline.
/// </para>
/// </remarks>
public abstract class AsyncSkippableResourceFilter
    : SkippableFilterBase<ResourceExecutingContext>, IAsyncResourceFilter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncSkippableResourceFilter"/> class.
    /// </summary>
    /// <param name="skipMode">
    /// Determines how the filter decides whether it should be skipped.
    /// </param>
    protected AsyncSkippableResourceFilter(SkipMode skipMode = SkipMode.Never)
        : base(skipMode) { }
    
    /// <summary>
    /// Executes the resource filter asynchronously as part of the ASP.NET Core pipeline.
    /// </summary>
    /// <param name="context">
    /// The current <see cref="ResourceExecutingContext"/>.
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
    async Task IAsyncResourceFilter.OnResourceExecutionAsync(
        ResourceExecutingContext context, ResourceExecutionDelegate next)
    {
        if (SkipExecution(context))
        {
            await next();
            return;
        }

        await OnResourceAsync(context, next);
    }

    /// <summary>
    /// Executes the custom asynchronous resource filter logic.
    /// </summary>
    /// <param name="context">
    /// The current <see cref="ResourceExecutingContext"/>.
    /// </param>
    /// <param name="next">
    /// A delegate representing the next step in the filter pipeline.
    /// It must be invoked to continue execution.
    /// </param>
    protected abstract Task OnResourceAsync(ResourceExecutingContext context,
        ResourceExecutionDelegate next);
}