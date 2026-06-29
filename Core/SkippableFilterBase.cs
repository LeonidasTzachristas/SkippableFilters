using Microsoft.AspNetCore.Mvc.Filters;
using SkippableFilters.Enums;

namespace SkippableFilters.Core;

/// <summary>
/// Provides a common base implementation for skippable ASP.NET Core filters.
/// </summary>
/// <typeparam name="TContext">
/// The type of filter context associated with the derived filter.
/// </typeparam>
public abstract class SkippableFilterBase<TContext>
    where TContext : FilterContext
{
    /// <summary>
    /// Gets the strategy used to determine whether the filter should be skipped.
    /// </summary>
    protected SkipMode SkipMode { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="SkippableFilterBase{TContext}"/> class.
    /// </summary>
    /// <param name="skipMode">
    /// Determines how the filter decides whether to execute or be skipped.
    /// </param>
    protected SkippableFilterBase(SkipMode skipMode = SkipMode.Never)
    {
        SkipMode = skipMode;
    }
    
    /// <summary>
    /// Determines whether the current filter execution should be skipped.
    /// </summary>
    /// <param name="context">
    /// The current filter context.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the filter should be skipped; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// The result is determined by the configured <see cref="SkipMode"/>.
    /// When <see cref="SkipMode.Custom"/> is specified, the decision is delegated
    /// to <see cref="ShouldSkip(TContext)"/>.
    /// </remarks>
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
    /// Determines whether the current filter execution should be skipped.
    /// </summary>
    /// <param name="context">
    /// The current filter context.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the filter should be skipped; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// This method is only invoked when <see cref="SkipMode.Custom"/> is used.
    /// Derived classes should override this method to provide custom skip logic.
    /// The default implementation throws an <see cref="InvalidOperationException"/>
    /// to indicate that a custom skip strategy was requested but not implemented.
    /// </remarks>
    protected virtual bool ShouldSkip(TContext context)
    {
        throw new InvalidOperationException(
            $"{GetType().Name} uses SkipMode.Custom but does not override ShouldSkip().");
    }
}