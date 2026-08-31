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
    public bool Success { get; internal set; }

    /// <summary>
    /// Processed data.
    /// </summary>
    /// <example>{ {foo, bar}, {bar, foo} }</example>
    public List<List<object>> Data { get; internal set; }

    /// <summary>
    /// Headers.
    /// </summary>
    /// <example>{ foos, bars }</example>
    public List<string> Headers { get; internal set; }

    /// <summary>
    /// The culture info used to read/write the entries.
    /// </summary>
    /// <example>fi-FI</example>
    public string ConfigurationCultureInfo { get; internal set; }

    /// <summary>
    /// Error details. Null when Success is true.
    /// </summary>
    /// <example>null</example>
    public Error Error { get; internal set; }
}