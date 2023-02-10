using System.Collections.Generic;

namespace Frends.CSV.Parse.Definitions;

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
    /// Processed data.
    /// </summary>
    /// <example>{ {foo, bar}, {bar, foo} }</example>
    public List<List<object>> Data { get; private set; }

    /// <summary>
    /// Headers.
    /// </summary>
    /// <example>{ foos, bars }</example>
    public List<string> Headers { get; private set; }

    /// <summary>
    /// The culture info used to read/write the entries. 
    /// </summary>
    /// <example>fi-FI</example>
    public string ConfigurationCultureInfo { get; private set; }

    /// <summary>
    /// Result as JToken.
    /// </summary>
    /// <example>{[ {"value": "1", "foos": "foo", "bars": "bar"} ]}</example>
    public object Jtoken { get; private set; }

    /// <summary>
    /// Result as XML.
    /// </summary>
    /// <example>"&lt;?xml version=\"1.0\" encoding=\"utf-8\"?&gt;\r\n&lt;Root&gt;\r\n  &lt;Row&gt;\r\n    &lt;value&gt;1&lt;/value&gt;\r\n    &lt;foos&gt;foo&lt;/foos&gt;\r\n    &lt;bars&gt;bar&lt;/bars&gt;\r\n&lt;/Row&gt;\r\n&lt;/Root&gt;"</example>
    public object Xml { get; private set; }

    internal Result(bool success, List<List<object>> data, List<string> headers, string configurationCultureInfo, object jtoken, object xml)
    {
        Success = success;
        Data = data;
        Headers = headers;
        ConfigurationCultureInfo = configurationCultureInfo;
        Jtoken = jtoken;
        Xml = xml;
    }
}