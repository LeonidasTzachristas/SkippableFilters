using Microsoft.AspNetCore.Mvc.Filters;
using SkippableFilters.Core;
using SkippableFilters.Enums;

namespace SkippableFilters.Resource;

public abstract class SkippableResourceFilter
    : SkippableFilterBase<ResourceExecutingContext>, IResourceFilter
{
    private static readonly object SkipKey = new();
    
    protected SkippableResourceFilter(SkipMode skipMode = SkipMode.Never)
        : base(skipMode) { }
    
    void IResourceFilter.OnResourceExecuting(ResourceExecutingContext context)
    {
        if (SkipExecution(context))
        {
            context.HttpContext.Items[SkipKey] = true;
            return;
        }
        
        this.OnResourceExecuting(context);
    }

    void IResourceFilter.OnResourceExecuted(ResourceExecutedContext context)
    {
        if ((bool?)context.HttpContext.Items[SkipKey] == true)
            return;
        
        this.OnResourceExecuted(context);
    }

    protected abstract void OnResourceExecuting(ResourceExecutingContext context);
    
    protected virtual void OnResourceExecuted(ResourceExecutedContext context) { }
}