using Microsoft.AspNetCore.Mvc.Filters;
using SkippableFilters.Core;
using SkippableFilters.Enums;

namespace SkippableFilters.Authorization;

public abstract class AsyncSkippableAuthorizationFilter 
    : SkippableFilterBase<AuthorizationFilterContext>, IAsyncAuthorizationFilter
{
    protected AsyncSkippableAuthorizationFilter(SkipMode skipMode = SkipMode.Never)
        :base(skipMode) { }
    
    async Task IAsyncAuthorizationFilter.OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (SkipExecution(context))
            return;
        
        await this.OnAuthorizationAsync(context);
    }

    protected abstract Task OnAuthorizationAsync(AuthorizationFilterContext context);
}