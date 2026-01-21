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

    internal Result(bool success, List<List<object>> data, List<string> headers, string configurationCultureInfo)
    {
        Success = success;
        Data = data;
        Headers = headers;
        ConfigurationCultureInfo = configurationCultureInfo;
    }
}