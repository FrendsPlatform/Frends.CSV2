using System.Collections.Generic;
using System.IO;
using System.Threading;
using CsvHelper;
using CsvHelper.Configuration;
using Frends.CSV.Create.Definitions;

namespace Frends.CSV.Create.Parsers;

internal static class ListParser
{
    internal static string ListToCsvString(List<List<object>> inputData, List<string> inputHeaders,
        CsvConfiguration config, Options options, CancellationToken cancellationToken)
    {
        using var csvString = new StringWriter();
        using var csv = new CsvWriter(csvString, config);

        //Write the header row
        if (config.HasHeaderRecord && inputHeaders is { Count: > 0 })
        {
            foreach (var column in inputHeaders)
            {
                cancellationToken.ThrowIfCancellationRequested();
                csv.WriteField(column);
            }

            csv.NextRecord();
        }

        foreach (var row in inputData)
        {
            foreach (var cell in row)
            {
                cancellationToken.ThrowIfCancellationRequested();
                csv.WriteField(cell ?? options.ReplaceNullsWith);
            }

            csv.NextRecord();
        }

        return csvString.ToString();
    }
}