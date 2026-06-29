using Microsoft.AspNetCore.Mvc.Filters;
using SkippableFilters.Core;
using SkippableFilters.Enums;

namespace SkippableFilters.Resource;

public abstract class AsyncSkippableResourceFilter
    : SkippableFilterBase<ResourceExecutingContext>, IAsyncResourceFilter
{
    protected AsyncSkippableResourceFilter(SkipMode skipMode = SkipMode.Never)
        : base(skipMode) { }
    
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

    protected abstract Task OnResourceAsync(ResourceExecutingContext context,
        ResourceExecutionDelegate next);
}