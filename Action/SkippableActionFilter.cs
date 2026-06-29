using Microsoft.AspNetCore.Mvc.Filters;
using SkippableFilters.Core;
using SkippableFilters.Enums;

namespace SkippableFilters.Action;

public abstract class SkippableActionFilter 
    : SkippableFilterBase<ActionExecutingContext>, IActionFilter
{
    private static readonly object SkipKey = new();
    
    protected SkippableActionFilter(SkipMode skipMode = SkipMode.Never)
        :base(skipMode) { }
    
    void IActionFilter.OnActionExecuting(ActionExecutingContext context)
    {
        if (SkipExecution(context))
        {
            context.HttpContext.Items[SkipKey] = true;
            return;
        }
        
        this.OnActionExecuting(context);
    }

    void IActionFilter.OnActionExecuted(ActionExecutedContext context)
    {
        if ((bool?)context.HttpContext.Items[SkipKey] == true)
            return;
        
        this.OnActionExecuted(context);
    }

    protected abstract void OnActionExecuting(ActionExecutingContext context);
    
    protected virtual void OnActionExecuted(ActionExecutedContext context) { }
}