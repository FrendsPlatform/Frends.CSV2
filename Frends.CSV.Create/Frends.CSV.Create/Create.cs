﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading;
using System.Xml.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using Frends.CSV.Create.Definitions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Frends.CSV.Create;

/// <summary>
/// CSV Task.
/// </summary>
public class CSV
{
    /// <summary>
    /// Create a CSV string from a List, JSON string or XML string.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.CSV.Create)
    /// </summary>
    /// <param name="input">Input parameters</param>
    /// <param name="options">Optional parameters</param>
    /// <param name="cancellationToken">Token generated by Frends to stop this Task.</param>
    /// <returns>Object { bool Success, string CSV }</returns>
    public static Result Create(
        [PropertyTab] Input input,
        [PropertyTab] Options options,
        CancellationToken cancellationToken
    )
    {
        var config = new CsvConfiguration(new CultureInfo(options.CultureInfo))
        {
            Delimiter = input.Delimiter,
            HasHeaderRecord = options.IncludeHeaderRow
        };

        if (options.NeverAddQuotesAroundValues)
        {
            config.Mode = CsvMode.NoEscape;
            // if IgnoreQuotes is true, seems like ShouldQuote function has to return false in all cases
            // if IgnoreQuotes is false ShouldQuote can't have any implementation otherwise it will overwrite IgnoreQuotes statement ( might turn it on again)
            config.ShouldQuote = (field) => (!options.NeverAddQuotesAroundValues);
        }
        var csv = string.Empty;

        switch (input.InputType)
        {
            case CreateInputType.List:
                csv = ListToCsvString(
                    input.Data,
                    input.Headers,
                    config,
                    options,
                    cancellationToken
                );
                break;
            case CreateInputType.Json:
                csv = JsonToCsvString(input.Json, config, options, cancellationToken);
                break;
            case CreateInputType.Xml:
                csv = XmlToCsvString(
                    input.Xml,
                    input.XmlNodeElementName,
                    config,
                    options,
                    cancellationToken
                );
                break;
        }
        return new Result(true, csv);
    }

    private static string ListToCsvString(
        List<List<object>> inputData,
        List<string> inputHeaders,
        CsvConfiguration config,
        Options options,
        CancellationToken cancellationToken
    )
    {
        using var csvString = new StringWriter();
        using var csv = new CsvWriter(csvString, config);

        //Write the header row
        if (config.HasHeaderRecord && inputData.Any())
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

    private static string JsonToCsvString(
        string json,
        CsvConfiguration config,
        Options options,
        CancellationToken cancellationToken
    )
    {
        //stringify numbers
        var jsonElement = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(
            json,
            new JsonSerializerOptions { AllowTrailingCommas = true, }
        );
        string stringifiedJson = System.Text.Json.JsonSerializer.Serialize(
            jsonElement,
            new JsonSerializerOptions { Converters = { new NumberToStringConverter() } }
        );

        JToken input = JsonConvert.DeserializeObject<JToken>(
            stringifiedJson,
            new JsonSerializerSettings { DateParseHandling = DateParseHandling.None, }
        );
        input = input is JObject ? new JArray() { input } : input;

        //compute what are max sizes of arrays in all objects
        var limits = new Dictionary<string, int>();
        foreach (var row in input)
        {
            JObject rowObject = JObject.FromObject(row);
            IEnumerable<JToken> jTokens = rowObject.Descendants().Where(p => !p.HasValues);
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

        //flatten json
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
                                        child.Path,
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
                                jToken.Path,
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

        //CSV part
        using var csvString = new StringWriter();
        using var csv = new CsvWriter(csvString, config);

        //Write the header row
        if (config.HasHeaderRecord && data.Any())
        {
            foreach (var column in data.First().Keys)
            {
                cancellationToken.ThrowIfCancellationRequested();
                csv.WriteField(column);
            }

            csv.NextRecord();
        }

        foreach (var row in data)
        {
            foreach (var cell in row)
            {
                cancellationToken.ThrowIfCancellationRequested();
                csv.WriteField(cell.Value ?? options.ReplaceNullsWith);
            }

            csv.NextRecord();
        }
        var foo = csvString.ToString();
        return csvString.ToString();
    }

    private static string XmlToCsvString(
        string xml,
        string node,
        CsvConfiguration config,
        Options options,
        CancellationToken cancellationToken
    )
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
            foreach (var cell in column.Elements().Select(n => n.Value).ToList())
            {
                cancellationToken.ThrowIfCancellationRequested();
                csv.WriteField(cell ?? options.ReplaceNullsWith);
            }
            csv.NextRecord();
        }

        return csvString.ToString();
    }
}
