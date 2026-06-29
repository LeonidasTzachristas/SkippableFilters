using Microsoft.AspNetCore.Mvc.Filters;
using SkippableFilters.Core;
using SkippableFilters.Enums;

namespace SkippableFilters.Result;

public abstract class AsyncSkippableResultFilter
    : SkippableFilterBase<ResultExecutingContext>, IAsyncResultFilter
{
    protected AsyncSkippableResultFilter(SkipMode skipMode = SkipMode.Never)
        : base(skipMode) { }
    
    async Task IAsyncResultFilter.OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (SkipExecution(context))
        {
            await next();
            return;
        }

        await this.OnResultExecutionAsync(context, next);
    }

    protected abstract Task OnResultExecutionAsync(ResultExecutingContext context,
        ResultExecutionDelegate next);
}