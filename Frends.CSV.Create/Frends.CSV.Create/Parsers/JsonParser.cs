using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using CsvHelper;
using CsvHelper.Configuration;
using Frends.CSV.Create.Definitions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Frends.CSV.Create.Parsers;

internal static class JsonParser
{
    internal static string JsonToCsvString(Input input, CsvConfiguration config, Options options,
        CancellationToken cancellationToken)
    {
        var json = StringifyJsonNumbers(input.Json);
        var jToken = JsonConvert.DeserializeObject<JToken>(
            json,
            new JsonSerializerSettings { DateParseHandling = DateParseHandling.None, }
        );
        var inputArray = jToken is JObject ? new JArray { jToken } : (JArray)jToken;

        // CSV part
        using var csvString = new StringWriter();
        using var csv = new CsvWriter(csvString, config);

        // If columns are specified manually
        if (input.SpecifyColumnsManually)
        {
            // Validate if columns are null or empty
            if (input.Columns == null || !input.Columns.Any())
            {
                throw new ArgumentException("Manual columns are specified but no columns are provided.");
            }

            // Write the manually specified header row
            if (config.HasHeaderRecord)
            {
                foreach (var header in input.Headers)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    csv.WriteField(header);
                }

                csv.NextRecord();
            }

            // Write the data rows using the specified JSON paths
            foreach (var row in inputArray)
            {
                foreach (var column in input.Columns)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var value = row.SelectToken(column)?.ToString() ?? options.ReplaceNullsWith;
                    csv.WriteField(value);
                }

                csv.NextRecord();
            }
        }
        else
        {
            var limits = JsonArraysLimits(inputArray);
            var data = FlattenJson(inputArray, limits);

            // Write the automatically generated header row
            if (config.HasHeaderRecord && data.Any())
            {
                foreach (var column in data.First().Keys)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    csv.WriteField(column);
                }

                csv.NextRecord();
            }

            // Write the data rows
            foreach (var row in data)
            {
                foreach (var cell in row)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    csv.WriteField(cell.Value ?? options.ReplaceNullsWith);
                }

                csv.NextRecord();
            }
        }

        return csvString.ToString();
    }


    private static string StringifyJsonNumbers(string json)
    {
        var jsonElement = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(
            json,
            new JsonSerializerOptions { AllowTrailingCommas = true, }
        );
        string stringifiedJson = System.Text.Json.JsonSerializer.Serialize(
            jsonElement,
            new JsonSerializerOptions { Converters = { new NumberToStringConverter() } }
        );
        return stringifiedJson;
    }

    private static Dictionary<string, int> JsonArraysLimits(JArray jArray)
    {
        var limits = new Dictionary<string, int>();
        foreach (var row in jArray)
        {
            IEnumerable<JToken> jTokens = JObject.FromObject(row).Descendants().Where(p => !p.HasValues);
            JToken previousToken = null;

            foreach (JToken jToken in jTokens)
            {
                //checking only one element of the same parent
                if (previousToken == null || previousToken?.Parent?.Path != jToken?.Parent?.Path)
                {
                    if (jToken?.Parent?.Count > 1)
                    {
                        if (limits.TryGetValue(jToken.Parent.Path, out int limit))
                        {
                            if (limit < jToken.Parent.Count)
                            {
                                limits[jToken.Parent.Path] = jToken.Parent.Count;
                            }
                        }
                        else
                        {
                            limits.Add(jToken.Parent.Path, jToken.Parent.Count);
                        }
                    }
                }
            }
        }

        return limits;
    }

    private static List<Dictionary<string, string>> FlattenJson(JArray input, Dictionary<string, int> limits)
    {
        List<Dictionary<string, string>> data = new();
        foreach (var row in input)
        {
            JObject rowObject = JObject.FromObject(row);
            IEnumerable<JToken> jTokens = rowObject.Descendants().Where(p => !p.HasValues);
            JToken previousToken = null;

            Dictionary<string, string> results = jTokens.Aggregate(
                new Dictionary<string, string>(),
                (properties, jToken) =>
                {
                    //skip adding if same parent, because we added all elements of array with the first occurance
                    if (
                        previousToken == null
                        || previousToken?.Parent?.Path != jToken?.Parent?.Path
                    )
                    {
                        //adding array properties
                        if (limits.TryGetValue(jToken!.Parent!.Path, out int limit))
                        {
                            //adding first value for empty array
                            if (jToken.Type == JTokenType.Null)
                            {
                                properties.Add(jToken.Parent.Path + "[0]", "");
                            }
                            else
                            {
                                //adding all children of an array to properties
                                foreach (JToken child in jToken.Parent!.Children())
                                {
                                    properties.Add(
                                        child.Path.Replace("['", "").Replace("']", ""),
                                        child.Type == JTokenType.Null ? null : child.ToString()
                                    );
                                }
                            }

                            //add empty values to even size of all arrays
                            for (int i = jToken.Parent.Count; i < limit; i++)
                            {
                                properties.Add(jToken.Parent.Path + "[" + i + "]", "");
                            }
                        }
                        else
                        {
                            //adding non-array properties
                            properties.Add(
                                jToken.Path.Replace("['", "").Replace("']", ""),
                                jToken.Type == JTokenType.Null ? null : jToken.ToString()
                            );
                        }
                    }

                    previousToken = jToken;
                    return properties;
                }
            );
            data.Add(results);
        }

        return data;
    }
}