using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.CSV.Create.Definitions;

/// <summary>
/// Options parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// The culture info to read/write the entries with, e.g. for decimal separators. 
    /// InvariantCulture will be used by default. 
    /// See list of cultures here: https://learn.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo.name?view=net-7.0
    /// NOTE: Due to an issue with the CsvHelpers library, all CSV tasks will use the culture info setting of the first CSV task in the process; you cannot use different cultures for reading and parsing CSV files in the same process.|
    /// </summary>
    /// <example>fi-FI</example>
    public string CultureInfo { get; set; } = "";

    /// <summary>
    /// This flag tells the writer if the header row should be written.
    /// </summary>
    /// <example>true</example>
    [DefaultValue("true")]
    public bool IncludeHeaderRow { get; set; } = true;

    /// <summary>
    /// If set true, CSV's fields are never put in quotes
    /// </summary>
    /// <example>false</example>
    [DefaultValue("false")]
    public bool NeverAddQuotesAroundValues { get; set; }

    /// <summary>
    /// Input's NULL values will be replaced with this value.
    /// </summary>
    /// <example>foo</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string ReplaceNullsWith { get; set; }

    /// <summary>
    /// Specifies the string representation format for boolean values in the CSV output.
    /// </summary>
    /// <example>BooleanFormat.Lowercase</example>
    [DefaultValue(BooleanFormat.Lowercase)]
    public BooleanFormat BooleanFormat { get; set; } = BooleanFormat.Lowercase;
}