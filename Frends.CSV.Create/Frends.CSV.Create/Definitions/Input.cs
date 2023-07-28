using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.CSV.Create.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Select input type to show correct editor for input
    /// </summary>
    /// <example>CreateInputType.List</example>
    [DefaultValue(CreateInputType.List)]
    public CreateInputType InputType { get; set; }

    /// <summary>
    /// Delimiter.
    /// </summary>
    /// <example>;</example>
    [DefaultValue("\";\"")]
    public string Delimiter { get; set; }

    /// <summary>
    /// Json string to write to CSV. 
    /// Must be an array of objects.
    /// </summary>
    /// <example>[{\"Column1\": \"row1Val1\",\"Column2\": \"row1Val2\"},{\"Column1\": \"row2Val1\",\"Column2\": \"row2Val2\"}]</example>
    [UIHint(nameof(InputType), "", CreateInputType.Json)]
    [DisplayFormat(DataFormatString = "Json")]
    public string Json { get; set; }

    /// <summary>
    /// Xml string to write to CSV. 
    /// </summary>
    /// <example>
    /// &lt;?xml version="1.0" encoding="utf-8"?&gt;
    /// &lt;Root&gt;
    ///    &lt;Row&gt;
    ///      &lt;Name&gt;Veijo&lt;/Name&gt;
    ///      &lt;Street&gt;FrendsStr&lt;/Street&gt;
    ///      &lt;StartDate&gt;2018-05-27T00:00:00&lt;/StartDate&gt;
    ///    &lt;/Row&gt;
    ///    &lt;Row&gt;
    ///      &lt;Name&gt;Hodor&lt;/Name&gt;
    ///      &lt;Street&gt;HodorsStr&lt;/Street&gt;
    ///      &lt;StartDate&gt;2018-01-01T00:00:00&lt;/StartDate&gt;
    ///    &lt;/Row&gt;
    /// &lt;/Root&gt;
    /// </example>
    [UIHint(nameof(InputType), "", CreateInputType.Xml)]
    [DisplayFormat(DataFormatString = "Xml")]
    public string Xml { get; set; }

    /// <summary>
    /// Name of the node from which the data is to be queried.
    /// </summary>
    /// <example>book</example>
    [UIHint(nameof(InputType), "", CreateInputType.Xml)]
    [DisplayFormat(DataFormatString = "Text")]
    public string XmlNodeElementName { get; set; }

    /// <summary>
    /// Headers for the data. 
    /// Need to be in the same order as the underlying data
    /// </summary>
    /// <example>{ Values, Foos, Bars, Dates }</example>
    [UIHint(nameof(InputType), "", CreateInputType.List)]
    public List<string> Headers { get; set; }

    /// <summary>
    /// Data to write to the CSV string. 
    /// Needs to be of type List&lt;List&lt;object&gt;&gt;. 
    /// The order of the nested list objects need to be in the same order as the header list.
    /// </summary>
    /// <example>{ 1, "foo", "bar", 2023-02-08 }</example>
    [UIHint(nameof(InputType), "", CreateInputType.List)]
    [DisplayFormat(DataFormatString = "Expression")]
    public List<List<object>> Data { get; set; }

}