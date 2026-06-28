using Microsoft.AspNetCore.Mvc.Filters;
using SkippableFilters.Enums;

namespace SkippableFilters.Authorization;

public abstract class AsyncSkippableAuthorizationFilter : IAsyncAuthorizationFilter
{
    protected SkipMode SkipMode { get; }

    protected AsyncSkippableAuthorizationFilter(SkipMode skipMode = SkipMode.Never)
    {
        SkipMode = skipMode;
    }
    
    async Task IAsyncAuthorizationFilter.OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (SkipExecution(context))
            return;

        await OnAuthorizeAsync(context);
    }

    private bool SkipExecution(AuthorizationFilterContext context)
    {
        return SkipMode switch
        {
            SkipMode.Never => false,
            SkipMode.Always => true,
            SkipMode.Custom => ShouldSkip(context),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    protected virtual bool ShouldSkip(AuthorizationFilterContext context)
    {
        throw new InvalidOperationException(
            $"{GetType().Name} uses SkipMode.Custom but does not override ShouldSkip().");
    }

    protected abstract Task OnAuthorizeAsync(AuthorizationFilterContext context);
}