using Microsoft.AspNetCore.Mvc.Filters;
using SkippableFilters.Core;
using SkippableFilters.Enums;

namespace SkippableFilters.Exception;

public abstract class AsyncSkippableExceptionFilter
    : SkippableFilterBase<ExceptionContext>, IAsyncExceptionFilter
{
    protected AsyncSkippableExceptionFilter(SkipMode skipMode = SkipMode.Never)
        : base(skipMode) { }
    
    async Task IAsyncExceptionFilter.OnExceptionAsync(ExceptionContext context)
    {
        if (SkipExecution(context))
            return;

        await this.OnExceptionAsync(context);
    }

    protected abstract Task OnExceptionAsync(ExceptionContext context);
}