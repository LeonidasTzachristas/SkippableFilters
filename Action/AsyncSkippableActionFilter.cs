using Microsoft.AspNetCore.Mvc.Filters;
using SkippableFilters.Core;
using SkippableFilters.Enums;

namespace SkippableFilters.Action;

public abstract class AsyncSkippableActionFilter 
    : SkippableFilterBase<ActionExecutingContext>, IAsyncActionFilter
{
    protected AsyncSkippableActionFilter(SkipMode skipMode = SkipMode.Never)
        : base(skipMode) { }

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

    protected abstract Task OnActionExecutionAsync(ActionExecutingContext context, 
        ActionExecutionDelegate next);
}