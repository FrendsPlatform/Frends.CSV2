namespace Frends.CSV.ConvertToJSON.Definitions;

/// <summary>
/// Column types.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "Self explanatory Enum values")]
public enum ColumnType
{
#pragma warning disable CS1591 // self explanatory
    String,
    Int,
    Long,
    Decimal,
    Double,
    Boolean,
    DateTime,
    Char,
#pragma warning restore CS1591 // self explanatory
}
