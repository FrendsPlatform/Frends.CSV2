﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.CSV.Parse.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Input csv string
    /// </summary>
    /// <example>1;Foo;Bar</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string Csv { get; set; }

    /// <summary>
    /// Delimiter.
    /// </summary>
    /// <example>;</example>
    [DefaultValue("\";\"")]
    public string Delimiter { get; set; }

    /// <summary>
    /// You can map columns to specific types. 
    /// The order of the columns are used for mapping, that means that the ColumnSpecification elements need to be created in the same order as the CSV fields.
    /// </summary>
    /// <example>[ foo, String ]</example>
    public ColumnSpecification[] ColumnSpecifications { get; set; }
}

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