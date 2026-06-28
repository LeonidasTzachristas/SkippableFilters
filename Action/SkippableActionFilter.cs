using Microsoft.AspNetCore.Mvc.Filters;
using SkippableFilters.Core;
using SkippableFilters.Enums;

namespace SkippableFilters.Action;

public abstract class SkippableActionFilter 
    : SkippableFilterBase<ActionExecutingContext>, IActionFilter
{
    protected SkippableActionFilter(SkipMode skipMode = SkipMode.Never)
        :base(skipMode) { }
    
    void IActionFilter.OnActionExecuting(ActionExecutingContext context)
    {
        if (SkipExecution(context))
            return;
        
        OnExecuting(context);
    }

    void IActionFilter.OnActionExecuted(ActionExecutedContext context)
    {
        OnExecuted(context);
    }

    protected abstract void OnExecuting(ActionExecutingContext context);
    
    protected virtual void OnExecuted(ActionExecutedContext context) { }
}