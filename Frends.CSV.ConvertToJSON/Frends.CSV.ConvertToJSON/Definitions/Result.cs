namespace Frends.CSV.ConvertToJSON.Definitions;

using Newtonsoft.Json.Linq;

/// <summary>
/// Result class with private setters.
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates whether the operation completed successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; internal set; }

    /// <summary>
    /// Result as JToken.
    /// </summary>
    /// <example>{[ {"value": "1", "foos": "foo", "bars": "bar"} ]}</example>
    public dynamic Json { get; internal set; }

    /// <summary>
    /// Error details. Null when Success is true.
    /// </summary>
    /// <example>null</example>
    public Error Error { get; internal set; }
}
