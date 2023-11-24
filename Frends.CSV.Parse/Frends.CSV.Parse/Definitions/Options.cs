using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.CSV.Parse.Definitions;

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
    public string ReplaceHeaderWhitespaceWith { get; set; }

    /// <summary>
    /// The culture info to read/write the entries with, e.g. for decimal separators.
    /// InvariantCulture will be used by default.
    /// See list of cultures here: https://learn.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo.name?view=net-7.0
    /// NOTE: Due to an issue with the CsvHelpers library, all CSV tasks will use the culture info setting of the first CSV task in the process; you cannot use different cultures for reading and parsing CSV files in the same process.|
    /// </summary>
    /// <example>fi-FI</example>
    public string CultureInfo { get; set; } = "";

    /// <summary>
    /// The flag for reader to treat missing fields as nulls instead of throwing a MissingFieldException.
    /// In case of providing column specifications manually the value of empty field defined as string will be empty instead of null
    /// </summary>
    [DefaultValue("false")]
    public bool TreatMissingFieldsAsNulls { get; set; } = false;
}