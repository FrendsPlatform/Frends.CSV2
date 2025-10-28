using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using CsvHelper;
using CsvHelper.Configuration;
using Frends.CSV.Create.Definitions;

namespace Frends.CSV.Create.Parsers;

internal static class JsonParserNew
{
    internal static string JsonToCsvString(Input input, CsvConfiguration config, Options options,
        CancellationToken cancellationToken)
    {
        var jsonBytes = Encoding.UTF8.GetBytes(input.Json);
        List<string> columns, headers;

        if (input.SpecifyColumnsManually)
        {
            if (input.Columns is null || input.Columns.Count == 0)
                throw new ArgumentException("Manual columns are specified but no columns are provided.");

            columns = input.Columns;
            headers = input.Headers;
        }
        else
        {
            columns = CollectAllColumns(jsonBytes);
            headers = columns;
        }

        if (columns is null || columns.Count == 0) throw new ArgumentException("No columns found in JSON.");

        using var csvString = new StringWriter();
        using var csv = new CsvWriter(csvString, config);

        // Write headers
        if (config.HasHeaderRecord)
        {
            foreach (var header in headers)
            {
                csv.WriteField(header);
            }

            csv.NextRecord();
        }

        // Write data rows
        WriteDataRows(jsonBytes, columns, csv, options.ReplaceNullsWith, cancellationToken);
        return csvString.ToString();
    }

    private static List<string> CollectAllColumns(byte[] jsonBytes)
    {
        var allColumns = new List<string>();
        var reader = new Utf8JsonReader(jsonBytes, new JsonReaderOptions { AllowTrailingCommas = true });
        if (!reader.Read() || (reader.TokenType != JsonTokenType.StartArray &&
                               reader.TokenType != JsonTokenType.StartObject))
        {
            throw new InvalidOperationException("Expected Json array or Json object at root");
        }

        do
        {
            if (reader.TokenType != JsonTokenType.StartObject) continue;
            var columns = ReadObjectColumns(ref reader, string.Empty);
            foreach (var column in columns)
            {
                var prevColumn = DecrementArrayIndex(column);
                if (allColumns.Contains(column) || allColumns.Contains($"{column}[0]")) continue;
                var prevIndex = allColumns.IndexOf(prevColumn);
                if (prevIndex == -1) allColumns.Add(column);
                else allColumns.Insert(prevIndex + 1, column);
            }
        } while (reader.Read());

        return allColumns;
    }

    private static string DecrementArrayIndex(string input)
    {
        return Regex.Replace(input, @"\[(\d+)\]$", match =>
        {
            var index = int.Parse(match.Groups[1].Value);
            var newIndex = index > 0 ? index - 1 : 0;
            return $"[{newIndex}]";
        });
    }

    private static List<string> ReadObjectColumns(ref Utf8JsonReader reader, string prefix)
    {
        var columns = new List<string>();
        var depth = reader.CurrentDepth;
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject && reader.CurrentDepth == depth) break;
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                var newPrefix = string.IsNullOrEmpty(prefix)
                    ? propertyName
                    : $"{prefix}.{propertyName}";
                reader.Read();
                switch (reader.TokenType)
                {
                    case JsonTokenType.StartObject:
                        var nestedColumns = ReadObjectColumns(ref reader, newPrefix);
                        columns.AddRange(nestedColumns);
                        break;
                    case JsonTokenType.StartArray:
                        var arrayColumns = ReadArrayColumns(ref reader, newPrefix);
                        columns.AddRange(arrayColumns);
                        break;
                    default:
                        columns.Add(newPrefix);
                        break;
                }
            }
        }

        return columns;
    }

    private static List<string> ReadArrayColumns(ref Utf8JsonReader reader, string prefix)
    {
        var columns = new List<string>();
        var depth = reader.CurrentDepth;
        var index = 0;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray && reader.CurrentDepth == depth) break;

            if (reader.CurrentDepth != depth + 1) continue;
            var arrayPrefix = $"{prefix}[{index}]";

            switch (reader.TokenType)
            {
                case JsonTokenType.StartObject:
                    var nestedColumns = ReadObjectColumns(ref reader, arrayPrefix);
                    columns.AddRange(nestedColumns);
                    break;
                case JsonTokenType.StartArray:
                    var arrayColumns = ReadArrayColumns(ref reader, arrayPrefix);
                    columns.AddRange(arrayColumns);
                    break;
                case JsonTokenType.Null:
                    break;
                default:
                    columns.Add(arrayPrefix);
                    break;
            }

            index++;
        }

        return columns;
    }

    private static void WriteDataRows(byte[] jsonBytes, List<string> columns,
        CsvWriter csv, string defaultValue, CancellationToken cancellationToken)
    {
        var reader = new Utf8JsonReader(jsonBytes, new JsonReaderOptions { AllowTrailingCommas = true });

        if (!reader.Read() || (reader.TokenType != JsonTokenType.StartArray &&
                               reader.TokenType != JsonTokenType.StartObject))
        {
            throw new InvalidOperationException("Expected Json array or Json object at root");
        }

        do
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (reader.TokenType != JsonTokenType.StartObject) continue;

            var rowData = ReadObjectData(ref reader, string.Empty);
            // Write row
            foreach (var column in columns)
            {
                csv.WriteField(rowData.GetValueOrDefault(column, defaultValue));
            }

            csv.NextRecord();
        } while (reader.Read());
    }

    private static Dictionary<string, string> ReadObjectData(ref Utf8JsonReader reader, string prefix)
    {
        var data = new Dictionary<string, string>();
        var depth = reader.CurrentDepth;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject && reader.CurrentDepth == depth) break;

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString() ?? string.Empty;
                var newPrefix = string.IsNullOrEmpty(prefix)
                    ? propertyName
                    : $"{prefix}.{propertyName}";
                reader.Read();
                switch (reader.TokenType)
                {
                    case JsonTokenType.StartObject:
                        var nestedData = ReadObjectData(ref reader, newPrefix);
                        foreach (var kvp in nestedData)
                        {
                            data[kvp.Key] = kvp.Value;
                        }

                        break;
                    case JsonTokenType.StartArray:
                        var arrayData = ReadArrayData(ref reader, newPrefix);
                        foreach (var kvp in arrayData)
                        {
                            data[kvp.Key] = kvp.Value;
                        }

                        break;
                    case JsonTokenType.String:
                        data[newPrefix] = reader.GetString() ?? string.Empty;
                        break;
                    case JsonTokenType.Number:
                        data[newPrefix] = Encoding.UTF8.GetString(reader.ValueSpan);
                        break;
                    case JsonTokenType.True:
                    case JsonTokenType.False:
                        data[newPrefix] = reader.GetBoolean().ToString().ToLower();
                        break;
                    case JsonTokenType.Null:
                        break;
                }
            }
        }

        return data;
    }

    private static Dictionary<string, string> ReadArrayData(ref Utf8JsonReader reader, string prefix)
    {
        var data = new Dictionary<string, string>();
        var depth = reader.CurrentDepth;
        var index = 0;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray && reader.CurrentDepth == depth)
            {
                break;
            }

            if (reader.CurrentDepth != depth + 1) continue;
            var arrayPrefix = $"{prefix}[{index}]";

            switch (reader.TokenType)
            {
                case JsonTokenType.StartObject:
                    var nestedData = ReadObjectData(ref reader, arrayPrefix);
                    foreach (var kvp in nestedData)
                    {
                        data[kvp.Key] = kvp.Value;
                    }

                    break;
                case JsonTokenType.StartArray:
                    var arrayData = ReadArrayData(ref reader, arrayPrefix);
                    foreach (var kvp in arrayData)
                    {
                        data[kvp.Key] = kvp.Value;
                    }

                    break;
                case JsonTokenType.String:
                    data[arrayPrefix] = reader.GetString() ?? string.Empty;
                    break;
                case JsonTokenType.Number:
                    data[arrayPrefix] = Encoding.UTF8.GetString(reader.ValueSpan);
                    break;
                case JsonTokenType.True:
                case JsonTokenType.False:
                    data[arrayPrefix] = reader.GetBoolean().ToString().ToLower();
                    break;
                case JsonTokenType.Null:
                    break;
            }

            index++;
        }

        return data;
    }
}