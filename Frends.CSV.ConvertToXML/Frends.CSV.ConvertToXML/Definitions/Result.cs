namespace Frends.CSV.ConvertToXML.Definitions;

/// <summary>
/// Task's result.
/// </summary>
public class Result
{
    /// <summary>
    /// Operation complete without errors.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// Result as XML.
    /// </summary>
    /// <example>"&lt;?xml version=\"1.0\" encoding=\"utf-8\"?&gt;\r\n&lt;Root&gt;\r\n  &lt;Row&gt;\r\n    &lt;value&gt;1&lt;/value&gt;\r\n    &lt;foos&gt;foo&lt;/foos&gt;\r\n    &lt;bars&gt;bar&lt;/bars&gt;\r\n&lt;/Row&gt;\r\n&lt;/Root&gt;"</example>
    public string Xml { get; private set; }

    internal Result(bool success, string xml)
    {
        Success = success;
        Xml = xml;
    }
}