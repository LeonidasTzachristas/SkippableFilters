using Microsoft.AspNetCore.Mvc.Filters;
using SkippableFilters.Core;
using SkippableFilters.Enums;

namespace SkippableFilters.Authorization;

/// <summary>
/// Provides an asynchronous base implementation of <see cref="IAsyncAuthorizationFilter"/>
/// with built-in support for conditional execution skipping.
/// </summary>
/// <remarks>
/// This filter allows derived classes to implement only the core authorization logic
/// without manually handling skip conditions.
/// </remarks>
/// /// <example>
/// Example of derived filter:
/// <code>
/// public class MyFilter : AsyncSkippableAuthorizationFilter
/// {
///     public MyResourceFilter(SkipMode skipMode = SkipMode.Never)
///         : base(skipMode) { }
///     protected override async Task OnAuthorizationAsync(AuthorizationFilterContext context)
///     {
///         // logic here
///     }
/// }
/// </code>
/// </example>
public abstract class AsyncSkippableAuthorizationFilter 
    : SkippableFilterBase<AuthorizationFilterContext>, IAsyncAuthorizationFilter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncSkippableAuthorizationFilter"/> class.
    /// </summary>
    /// <param name="skipMode">
    /// Determines how the filter decides whether it should be skipped.
    /// </param>
    protected AsyncSkippableAuthorizationFilter(SkipMode skipMode = SkipMode.Never)
        :base(skipMode) { }
    
    /// <summary>
    /// This method is invoked by ASP.NET Core during the authorization phase.
    /// </summary>
    /// <param name="context">
    /// The current <see cref="AuthorizationFilterContext"/>.
    /// </param>
    async Task IAsyncAuthorizationFilter.OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (SkipExecution(context))
            return;
        
        await this.OnAuthorizationAsync(context);
    }

    /// <summary>
    /// Executes the custom asynchronous authorization filter logic.
    /// </summary>
    /// <param name="context">
    /// The current <see cref="AuthorizationFilterContext"/>.
    /// </param>
    protected abstract Task OnAuthorizationAsync(AuthorizationFilterContext context);
}