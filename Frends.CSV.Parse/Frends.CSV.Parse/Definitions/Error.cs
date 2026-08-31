using System;

namespace Frends.CSV.Parse.Definitions;

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
    /// <example>System.NullReferenceException: Object reference not set to an instance of an object.</example>
    public Exception AdditionalInfo { get; internal set; }
}
