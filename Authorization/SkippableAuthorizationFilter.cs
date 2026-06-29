using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using SkippableFilters.Core;
using SkippableFilters.Enums;

namespace SkippableFilters.Authorization;

/// <summary>
/// Provides a base implementation of <see cref="IAuthorizationFilter"/> with built-in support
/// for conditional execution skipping.
/// </summary>
/// <remarks>
/// This filter allows derived classes to implement only the relevant action logic
/// without manually handling skip conditions or pipeline continuation.
/// </remarks>
/// <example>
/// Example of derived filter:
/// <code>
/// public class MyFilter : SkippableAuthorizationFilter
/// {
///     public MyFilter(SkipMode skipMode = SkipMode.Never) 
///         : base(skipMode) { }
///
///     protected override void OnAuthorization(AuthorizationFilterContext context)
///     {
///         // logic here
///     }
/// }
/// </code>
/// </example>
public abstract class SkippableAuthorizationFilter 
    : SkippableFilterBase<AuthorizationFilterContext>, IAuthorizationFilter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SkippableAuthorizationFilter"/> class.
    /// </summary>
    /// <param name="skipMode">
    /// Determines how the filter decides whether it should be skipped.
    /// </param>
    protected SkippableAuthorizationFilter(SkipMode skipMode = SkipMode.Never)
        : base(skipMode) { }
    
    /// <summary>
    /// Called by ASP.NET Core during authorization phase of the request pipeline.
    /// </summary>
    /// <param name="context">
    /// The current <see cref="AuthorizationFilterContext"/>.
    /// </param>
    void IAuthorizationFilter.OnAuthorization(AuthorizationFilterContext context)
    {
        if (SkipExecution(context))
            return;
        
        this.OnAuthorization(context);
    }
    
    /// <summary>
    /// Executes the authorization logic.
    /// </summary>
    /// <param name="context">
    /// The current <see cref="AuthorizationFilterContext"/>.
    /// </param>
    protected abstract void OnAuthorization(AuthorizationFilterContext context);
}