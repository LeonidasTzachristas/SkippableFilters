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

    /// <summary>
    /// Determines whether the filter should be skipped.
    /// </summary>
    /// <param name="context">
    /// The current action execution context.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the filter should be skipped; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// This method is only invoked when <see cref="SkipMode.Custom"/> is specified.
    /// The default implementation throws an <see cref="InvalidOperationException"/>
    /// to indicate that derived classes must override this method when using
    /// <see cref="SkipMode.Custom"/>.
    /// </remarks>
    protected virtual bool ShouldSkip(TContext context)
    {
        throw new InvalidOperationException(
            $"{GetType().Name} uses SkipMode.Custom but does not override ShouldSkip().");
    }
}