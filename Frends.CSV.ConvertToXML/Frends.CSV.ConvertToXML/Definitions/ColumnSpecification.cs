namespace Frends.CSV.ConvertToXML.Definitions;

/// <summary>
/// ColumnSpecification values
/// </summary>
public class ColumnSpecification
{
    /// <summary>
    /// Name of the resulting column
    /// </summary>
    /// <example>foo</example>
    public string Name { get; set; }

    /// <summary>
    /// Type for the resulting column.
    /// </summary>
    /// <example>String</example>
    public ColumnType Type { get; set; }
}
