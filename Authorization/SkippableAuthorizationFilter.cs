using Microsoft.AspNetCore.Mvc.Filters;
using SkippableFilters.Enums;

namespace SkippableFilters.Authorization;

public abstract class SkippableAuthorizationFilter : IAuthorizationFilter
{
    protected SkipMode SkipMode { get; }

    protected SkippableAuthorizationFilter(SkipMode skipMode = SkipMode.Never)
    {
        SkipMode = skipMode;
    }

    void IAuthorizationFilter.OnAuthorization(AuthorizationFilterContext context)
    {
        if (SkipExecution(context))
            return;

        OnAuthorize(context);
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

    protected abstract void OnAuthorize(AuthorizationFilterContext context);
}