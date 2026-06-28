namespace SkippableFilters.Enums;

/// <summary>
/// Defines how a skippable filter determines whether it should execute or be skipped.
/// </summary>
public enum SkipMode
{
    /// <summary>
    /// The filter will always execute and will never be skipped.
    /// </summary>
    Never,
    /// <summary>
    /// The filter will always be skipped and its execution logic will not run.
    /// </summary>
    Always,
    /// <summary>
    /// The skip decision is determined by custom logic implemented in the filter,
    /// typically by overriding <c>ShouldSkip</c> in the base filter class.
    /// </summary>
    Custom
}