using Microsoft.AspNetCore.Mvc.Filters;
using SkippableFilters.Core;
using SkippableFilters.Enums;

namespace SkippableFilters.Authorization;

public abstract class SkippableAuthorizationFilter 
    : SkippableFilterBase<AuthorizationFilterContext>, IAuthorizationFilter
{
    protected SkippableAuthorizationFilter(SkipMode skipMode = SkipMode.Never)
        : base(skipMode) { }
    
    void IAuthorizationFilter.OnAuthorization(AuthorizationFilterContext context)
    {
        if (SkipExecution(context))
            return;
        
        this.OnAuthorization(context);
    }

    protected abstract void OnAuthorization(AuthorizationFilterContext context);
}