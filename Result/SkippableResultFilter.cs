using Microsoft.AspNetCore.Mvc.Filters;
using SkippableFilters.Core;
using SkippableFilters.Enums;

namespace SkippableFilters.Result;

public abstract class SkippableResultFilter
    : SkippableFilterBase<ResultExecutingContext>, IResultFilter
{
    private static readonly object SkipKey = new();
    
    protected SkippableResultFilter(SkipMode skipMode = SkipMode.Never)
        : base(skipMode) { }

    void IResultFilter.OnResultExecuting(ResultExecutingContext context)
    {
        if (SkipExecution(context))
        {
            context.HttpContext.Items[SkipKey] = true;
            return;
        }

        this.OnResultExecuting(context);
    }

    void IResultFilter.OnResultExecuted(ResultExecutedContext context)
    {
        if ((bool?)context.HttpContext.Items[SkipKey] == true)
            return;
        
        this.OnResultExecuted(context);
    }

    protected abstract void OnResultExecuting(ResultExecutingContext context);
    
    protected virtual void OnResultExecuted(ResultExecutedContext context) { }
}