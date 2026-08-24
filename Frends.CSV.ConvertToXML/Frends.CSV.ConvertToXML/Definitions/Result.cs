namespace Frends.CSV.ConvertToXML.Definitions;

/// <summary>
/// Task's result.
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates whether the operation completed successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; internal set; }

    /// <summary>
    /// Result as XML.
    /// </summary>
    /// <example>&lt;?xml version="1.0" encoding="utf-8"?&gt;&lt;Root&gt;&lt;Row&gt;&lt;value&gt;1&lt;/value&gt;&lt;/Row&gt;&lt;/Root&gt;</example>
    public string Xml { get; internal set; }

    /// <summary>
    /// Error details. Null when Success is true.
    /// </summary>
    /// <example>null</example>
    public Error Error { get; internal set; }

    internal Result(bool success, string xml, Error error = null)
    {
        Success = success;
        Xml = xml;
        Error = error;
    }
}