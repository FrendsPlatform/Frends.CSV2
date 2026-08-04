namespace Frends.CSV.Create.Definitions;

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
    /// CSV string.
    /// </summary>
    /// <example>First;Second;"foo";"bar"</example>
    public string CSV { get; internal set; }

    /// <summary>
    /// Error information. Null when Success is true.
    /// </summary>
    public Error Error { get; internal set; }

    internal Result(bool success, string cSV)
    {
        Success = success;
        CSV = cSV;
    }

    internal Result() { }
}