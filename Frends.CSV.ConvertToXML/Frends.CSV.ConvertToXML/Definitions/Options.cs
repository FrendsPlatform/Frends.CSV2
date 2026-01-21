using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.CSV.ConvertToXML.Definitions;

/// <summary>
/// Options parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// This flag tells the reader if there is a header row in the CSV string.
    /// </summary>
    /// <example>true</example>
    [DefaultValue("true")]
    public bool ContainsHeaderRow { get; set; } = true;

    /// <summary>
    /// This flag tells the reader to trim whitespace from the beginning and ending of the field value when reading.
    /// </summary>
    /// <example>true</example>
    [DefaultValue("true")]
    public bool TrimOutput { get; set; } = true;

    /// <summary>
    /// If the CSV string contains metadata before the header row you can set this value to ignore a specific amount of rows from the beginning of the csv string.
    /// </summary>
    /// <example>2</example>
    [DefaultValue(0)]
    public int SkipRowsFromTop { get; set; }

    /// <summary>
    /// A flag to let the reader know if a record should be skipped when reading if it's empty.
    /// A record is considered empty if all fields are empty.
    /// </summary>
    /// <example>true</example>
    [DefaultValue("true")]
    public bool SkipEmptyRows { get; set; }

    /// <summary>
    /// If intended header value contains whitespaces replace it(them) with this string, default action is to do nothing.
    /// </summary>
    /// <example>" "</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue(" ")]
    public string ReplaceHeaderWhitespaceWith { get; set; } = " ";

    /// <summary>
    /// The culture info to read/write the entries with, e.g. for decimal separators.
    /// InvariantCulture will be used by default.
    /// See list of cultures here: https://learn.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo.name?view=net-7.0
    /// NOTE: Due to an issue with the CsvHelpers library, all CSV tasks will use the culture info setting of the first CSV task in the process; you cannot use different cultures for reading and parsing CSV files in the same process.|
    /// </summary>
    /// <example>fi-FI</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string CultureInfo { get; set; } = "";

    /// <summary>
    /// The flag for reader to treat missing fields as nulls instead of throwing a MissingFieldException.
    /// In case of providing column specifications manually the value of empty field defined as string will be empty instead of null
    /// </summary>
    /// <example>true</example>
    [DefaultValue("true")]
    public bool TreatMissingFieldsAsNulls { get; set; } = false;

    /// <summary>
    /// Specifies the name for the XML root element.
    /// </summary>
    /// <example>Root</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("Root")]
    public string XmlRootElementName { get; set; }

    /// <summary>
    /// Specifies the name for the XML row element.
    /// </summary>
    /// <example>Row</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("Row")]
    public string XmlRowElementName { get; set; }

    /// <summary>
    /// A flag to let the reader know if quotes should be ignored.
    /// </summary>
    /// <example>false</example>
    [DefaultValue("false")]
    public bool IgnoreQuotes { get; set; }

    /// <summary>
    /// If a converted node name is illegal, a selected action will be taken.
    /// </summary>
    /// <example>ThrowError</example>
    [DefaultValue(IllegalNodeNameAction.ThrowError)]
    public IllegalNodeNameAction IllegalNodeNameAction { get; set; } = IllegalNodeNameAction.ThrowError;

    /// <summary>
    /// A prefix that will be added to illegal node names. If it's empty, the prefix will be "_"
    /// </summary>
    /// <example>ThrowError</example>
    [DefaultValue("")]
    [UIHint(nameof(IllegalNodeNameAction), "", IllegalNodeNameAction.Overwrite)]
    public string IllegalNodeNamePrefix { get; set; } = string.Empty;
}