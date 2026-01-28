namespace Frends.CSV.Create.Definitions;

/// <summary>
/// Input types.
/// </summary>
public enum CreateInputType
{
#pragma warning disable CS1591 // self explanatory
    List,
    Json,
    Xml
#pragma warning restore CS1591 // self explanatory
}

/// <summary>
/// Defines the string representation formats for boolean values in CSV output.
/// </summary>
public enum BooleanFormat
{
    /// <summary>
    /// Outputs as "true"/"false" (JSON standard, lowercase)
    /// </summary>
    Lowercase,

    /// <summary>
    /// Outputs as "True"/"False" (C#/Python style, PascalCase)
    /// </summary>
    PascalCase,

    /// <summary>
    /// Outputs as "1"/"0" (numeric representation)
    /// </summary>
    Numeric
}
