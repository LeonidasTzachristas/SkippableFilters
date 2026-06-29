using SkippableFilters.Enums;

namespace SkippableFilters.Core;

public abstract class SkippableFilterBase<TContext>
{
    protected SkipMode SkipMode { get; }
    
    protected SkippableFilterBase(SkipMode skipMode = SkipMode.Never)
    {
        SkipMode = skipMode;
    }
    
    protected bool SkipExecution(TContext context)
    {
        return SkipMode switch
        {
            SkipMode.Never => false,
            SkipMode.Always => true,
            SkipMode.Custom => ShouldSkip(context),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    protected virtual bool ShouldSkip(TContext context)
    {
        throw new InvalidOperationException(
            $"{GetType().Name} uses SkipMode.Custom but does not override ShouldSkip().");
    }
}