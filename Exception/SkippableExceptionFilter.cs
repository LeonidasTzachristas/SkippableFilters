using System.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using SkippableFilters.Core;
using SkippableFilters.Enums;

namespace SkippableFilters.Exception;

/// <summary>
/// Provides a base implementation of <see cref="IExceptionFilter"/> with built-in support
/// for conditional execution skipping.
/// </summary>
/// <remarks>
/// <para>
/// This filter allows derived classes to implement only the relevant exception logic
/// without manually handling skip conditions or pipeline continuation.
/// </para>
/// <para>
/// When execution is skipped, the derived exception logic is not executed and
/// the exception is allowed to continue through the pipeline.
/// </para>
/// </remarks>
/// <example>
/// Example of derived filter:
/// <code>
/// public class MyFilter : SkippableExceptionFilter
/// {
///     public MyFilter(SkipMode skipMode = SkipMode.Never) 
///         : base(skipMode) { }
///
///     protected override void OnException(ExceptionContext context)
///     {
///         // logic here
///     }
/// }
/// </code>
/// </example>
public abstract class SkippableExceptionFilter
    : SkippableFilterBase<ExceptionContext>, IExceptionFilter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SkippableExceptionFilter"/> class.
    /// </summary>
    /// <param name="skipMode">
    /// Determines how the filter decides whether it should be skipped.
    /// </param>
    protected SkippableExceptionFilter(SkipMode skipMode = SkipMode.Never)
        : base(skipMode) { }
    
    /// <summary>
    /// Called by ASP.NET Core during exception phase of the request pipeline.
    /// </summary>
    /// <param name="context">
    /// The current <see cref="ExceptionContext"/>.
    /// </param>
    void IExceptionFilter.OnException(ExceptionContext context)
    {
        if (SkipExecution(context))
            return;

        this.OnException(context);
    }

    /// <summary>
    /// Executes the exception logic.
    /// </summary>
    /// <param name="context">
    /// The current <see cref="ExceptionContext"/>.
    /// </param>
    protected abstract void OnException(ExceptionContext context);
}