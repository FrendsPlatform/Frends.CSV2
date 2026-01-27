using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using Frends.CSV.Create.Definitions;

namespace Frends.CSV.Create.Parsers;

internal static class XmlParser
{
    internal static string XmlToCsvString(string xml, string node, CsvConfiguration config, Options options,
        CancellationToken cancellationToken)
    {
        using var csvString = new StringWriter();
        using var csv = new CsvWriter(csvString, config);

        XDocument xdoc = XDocument.Parse(xml);
        IEnumerable<XElement> nodes;
        if (string.IsNullOrEmpty(node))
            nodes = xdoc.Root.Elements();
        else
            nodes = xdoc.Descendants().Where(p => p.Name.LocalName.Equals(node));

        //Write the header row
        if (config.HasHeaderRecord && nodes.Any())
        {
            var headers = nodes.Descendants().Select(n => n.Name.LocalName).Distinct().ToList();

            foreach (var column in headers)
            {
                cancellationToken.ThrowIfCancellationRequested();
                csv.WriteField(column.ToString());
            }

            csv.NextRecord();
        }

        foreach (var column in nodes)
        {
            foreach (var element in column.Elements())
            {
                cancellationToken.ThrowIfCancellationRequested();

                var cell = element.Value;
                var value = cell switch
                {
                    "true" or "True" or "TRUE" => Helpers.FormatBoolean(true, options.BooleanFormat),
                    "false" or "False" or "FALSE" => Helpers.FormatBoolean(false, options.BooleanFormat),
                    null => options.ReplaceNullsWith,
                    _ => cell
                };

                csv.WriteField(value);
            }

            csv.NextRecord();
        }

        return csvString.ToString();
    }
}