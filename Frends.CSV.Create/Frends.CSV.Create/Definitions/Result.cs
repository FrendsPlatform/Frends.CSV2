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
    public bool Success { get; init; }

    /// <summary>
    /// CSV string.
    /// </summary>
    /// <example>First;Second;"foo";"bar"</example>
    public string CSV { get; init; }

    /// <summary>
    /// Error information. Null when Success is true.
    /// </summary>
    public Error Error { get; init; }
}