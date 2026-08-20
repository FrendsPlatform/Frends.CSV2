namespace Frends.CSV.ConvertToJSON.Definitions;

using System;

/// <summary>
/// Error details returned when the Task fails.
/// </summary>
public class Error
{
    /// <summary>
    /// Error message.
    /// </summary>
    /// <example>CSV input can not be empty</example>
    public string Message { get; set; }

    /// <summary>
    /// Additional information about the error, typically the original exception.
    /// </summary>
    /// <example>null</example>
    public Exception AdditionalInfo { get; set; }
}
