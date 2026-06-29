using System.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using SkippableFilters.Core;
using SkippableFilters.Enums;

namespace SkippableFilters.Exception;

public abstract class SkippableExceptionFilter
    : SkippableFilterBase<ExceptionContext>, IExceptionFilter
{
    protected SkippableExceptionFilter(SkipMode skipMode = SkipMode.Never)
        : base(skipMode) { }
    
    void IExceptionFilter.OnException(ExceptionContext context)
    {
        if (SkipExecution(context))
            return;

        this.OnException(context);
    }

    protected abstract void OnException(ExceptionContext context);
}