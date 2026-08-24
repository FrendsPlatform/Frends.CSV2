using System;

namespace Frends.CSV.ConvertToXML.Definitions;

/// <summary>
/// Error details for a failed operation.
/// </summary>
public class Error
{
    /// <summary>
    /// Error message describing the failure.
    /// </summary>
    /// <example>CSV input can not be empty</example>
    public string Message { get; set; }

    /// <summary>
    /// Additional information about the error, typically the original exception.
    /// </summary>
    /// <example>null</example>
    public Exception AdditionalInfo { get; set; }
}
