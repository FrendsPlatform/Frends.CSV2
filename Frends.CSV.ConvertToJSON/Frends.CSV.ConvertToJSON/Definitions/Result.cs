namespace Frends.CSV.ConvertToJSON.Definitions;

using Newtonsoft.Json.Linq;

/// <summary>
/// Result class..
/// </summary>
public class Result
{
    internal Result(bool success, JToken json)
    {
        Success = success;
        Json = json;
    }

    /// <summary>
    /// Operation complete without errors.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// Result as JToken.
    /// </summary>
    /// <example>{[ {"value": "1", "foos": "foo", "bars": "bar"} ]}</example>
    public dynamic Json { get; private set; }
}
