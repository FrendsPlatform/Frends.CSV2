using System;

namespace Frends.CSV.Create.Definitions;

/// <summary>
/// Error information returned when the task fails and ThrowErrorOnFailure is false.
/// </summary>
public class Error
{
    /// <summary>
    /// Error message.
    /// </summary>
    /// <example>Something went wrong.</example>
    public string Message { get; internal set; }

    /// <summary>
    /// The exception that caused the error.
    /// </summary>
    /// <example>System.FormatException: Input string was not in a correct format.</example>
    public Exception AdditionalInfo { get; internal set; }
}
