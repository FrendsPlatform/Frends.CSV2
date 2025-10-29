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
            columns = CollectAllColumns(jsonBytes, cancellationToken);
            headers = columns;
        }

        if (columns is null || columns.Count == 0) throw new ArgumentException("No columns found in JSON.");

        using var csvString = new StringWriter();
        using var csv = new CsvWriter(csvString, config);

        if (config.HasHeaderRecord)
        {
            foreach (var header in headers)
            {
                csv.WriteField(header);
            }

            csv.NextRecord();
        }

        WriteDataRows(jsonBytes, columns, csv, options.ReplaceNullsWith, cancellationToken);
        return csvString.ToString();
    }

    private static List<string> CollectAllColumns(byte[] jsonBytes, CancellationToken cancellationToken)
    {
        var allColumns = new List<string>();
        var columns = new List<string>();
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
            ReadObjectColumns(ref reader, ref columns, string.Empty);
            InsertColumns(ref allColumns, ref columns);
        } while (reader.Read());

        return allColumns;
    }

    private static void InsertColumns(ref List<string> allColumns, ref List<string> columns)
    {
        foreach (var column in columns)
        {
            var prevColumn = DecrementArrayIndex(column);
            if (allColumns.Contains(column) || allColumns.Contains($"{column}[0]")) continue;
            var prevIndex = allColumns.IndexOf(prevColumn);
            if (prevIndex == -1) allColumns.Add(column);
            else allColumns.Insert(prevIndex + 1, column);
        }

        columns.Clear();
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

    private static void ReadObjectColumns(ref Utf8JsonReader reader,
        ref List<string> columns, string prefix)
    {
        var depth = reader.CurrentDepth;
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject && reader.CurrentDepth == depth) break;
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var newPrefix = string.IsNullOrEmpty(prefix)
                    ? reader.GetString()
                    : $"{prefix}.{reader.GetString()}";
                reader.Read();
                switch (reader.TokenType)
                {
                    case JsonTokenType.StartObject:
                        ReadObjectColumns(ref reader, ref columns, newPrefix);
                        break;
                    case JsonTokenType.StartArray:
                        ReadArrayColumns(ref reader, ref columns, newPrefix);
                        break;
                    default:
                        columns.Add(newPrefix);
                        break;
                }
            }
        }
    }

    private static void ReadArrayColumns(ref Utf8JsonReader reader,
        ref List<string> columns, string prefix)
    {
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
                    ReadObjectColumns(ref reader, ref columns, arrayPrefix);
                    break;
                case JsonTokenType.StartArray:
                    ReadArrayColumns(ref reader, ref columns, arrayPrefix);
                    break;
                case JsonTokenType.Null:
                    break;
                default:
                    columns.Add(arrayPrefix);
                    break;
            }

            index++;
        }
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

        var rowData = new Dictionary<string, string>();
        var data = new Dictionary<string, string>();
        do
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (reader.TokenType != JsonTokenType.StartObject) continue;
            ReadObjectData(ref reader, ref rowData, ref data, string.Empty);
            foreach (var column in columns)
            {
                csv.WriteField(rowData.GetValueOrDefault(column, defaultValue));
            }

            rowData.Clear();
            data.Clear();
            csv.NextRecord();
        } while (reader.Read());
    }

    private static void ReadObjectData(ref Utf8JsonReader reader, ref Dictionary<string, string> allData,
        ref Dictionary<string, string> data, string prefix)
    {
        var depth = reader.CurrentDepth;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject && reader.CurrentDepth == depth) break;

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var newPrefix = string.IsNullOrEmpty(prefix)
                    ? reader.GetString()
                    : $"{prefix}.{reader.GetString()}";
                reader.Read();
                switch (reader.TokenType)
                {
                    case JsonTokenType.StartObject:
                        ReadObjectData(ref reader, ref allData, ref data, newPrefix);
                        InsertData(ref allData, ref data);
                        break;
                    case JsonTokenType.StartArray:
                        ReadArrayData(ref reader, ref allData, ref data, newPrefix);
                        InsertData(ref allData, ref data);
                        break;
                    case JsonTokenType.String:
                        allData[newPrefix] = reader.GetString() ?? string.Empty;
                        break;
                    case JsonTokenType.Number:
                        allData[newPrefix] = Encoding.UTF8.GetString(reader.ValueSpan);
                        break;
                    case JsonTokenType.True:
                    case JsonTokenType.False:
                        allData[newPrefix] = reader.GetBoolean().ToString().ToLower();
                        break;
                    case JsonTokenType.Null:
                        break;
                }
            }
        }
    }

    private static void ReadArrayData(ref Utf8JsonReader reader, ref Dictionary<string, string> allData,
        ref Dictionary<string, string> data, string prefix)
    {
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
                    ReadObjectData(ref reader, ref allData, ref data, arrayPrefix);
                    InsertData(ref allData, ref data);

                    break;
                case JsonTokenType.StartArray:
                    ReadArrayData(ref reader, ref allData, ref data, arrayPrefix);
                    InsertData(ref allData, ref data);
                    break;
                case JsonTokenType.String:
                    allData[arrayPrefix] = reader.GetString() ?? string.Empty;
                    break;
                case JsonTokenType.Number:
                    allData[arrayPrefix] = Encoding.UTF8.GetString(reader.ValueSpan);
                    break;
                case JsonTokenType.True:
                case JsonTokenType.False:
                    allData[arrayPrefix] = reader.GetBoolean().ToString().ToLower();
                    break;
                case JsonTokenType.Null:
                    break;
            }

            index++;
        }
    }

    private static void InsertData(ref Dictionary<string, string> allData,
        ref Dictionary<string, string> data)
    {
        foreach (var kvp in data)
        {
            allData[kvp.Key] = kvp.Value;
        }

        data.Clear();
    }
}