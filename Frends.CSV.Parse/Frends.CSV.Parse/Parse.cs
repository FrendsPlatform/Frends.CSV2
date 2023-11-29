﻿using CsvHelper;
using CsvHelper.Configuration;
using Frends.CSV.Parse.Definitions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;

namespace Frends.CSV.Parse;

/// <summary>
/// CSV Task.
/// </summary>
public class CSV
{
    /// <summary>
    /// Frends Task for parsing CSV string.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.CSV.Parse)
    /// </summary>
    /// <param name="input">Input parameters</param>
    /// <param name="options">Optional parameters</param>
    /// <param name="cancellationToken">Token generated by Frends to stop this Task.</param>
    /// <returns>Object { bool Success, List&lt;List&lt;object&gt;&gt; Data, List&gt;string&lt; Headers, string ConfigurationCultureInfo, object Jtoken, object Xml }</returns>
    public static Result Parse([PropertyTab] Input input, [PropertyTab] Options options, CancellationToken cancellationToken)
    {
        var cultureInfo = new CultureInfo(options.CultureInfo);
        var resultData = new List<List<object>>();
        var headers = new List<string>();

        var configuration = new CsvConfiguration(cultureInfo)
        {
            HasHeaderRecord = options.ContainsHeaderRow,
            Delimiter = input.Delimiter,
            TrimOptions = options.TrimOutput ? TrimOptions.Trim : TrimOptions.None,
            IgnoreBlankLines = options.SkipEmptyRows
        };

        // Setting the MissingFieldFound -delegate property of configuration to null when
        // option.TreatMissingFieldsAsNulls is set to true for returning null values for missing fields.
        // Otherwise the default setting which throws a MissingFieldException is used
        if (options.TreatMissingFieldsAsNulls)
            configuration.MissingFieldFound = null;

        using TextReader sr = new StringReader(input.Csv);
        //Read rows before passing textreader to csvreader for so that header row would be in the correct place
        for (var i = 0; i < options.SkipRowsFromTop; i++)
            sr.ReadLine();

        using var csvReader = new CsvReader(sr, configuration);
        if (options.ContainsHeaderRow)
        {
            csvReader.Read();
            csvReader.ReadHeader();
        }

        if (input.ColumnSpecifications.Any())
        {
            var typeList = new List<Type>();

            foreach (var columnSpec in input.ColumnSpecifications)
            {
                typeList.Add(ToType(columnSpec.Type, options.TreatMissingFieldsAsNulls));
                headers.Add(columnSpec.Name);
            }

            while (csvReader.Read())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var innerList = new List<object>();
                for (var index = 0; index < input.ColumnSpecifications.Length; index++)
                {
                    var obj = csvReader.GetField(typeList[index], index);
                    innerList.Add(obj);
                }
                resultData.Add(innerList);
            }
        }
        else if (options.ContainsHeaderRow && !input.ColumnSpecifications.Any())
        {
            if (string.Equals(options.ReplaceHeaderWhitespaceWith, " "))
                headers = csvReader.HeaderRecord.ToList();
            else
                headers = csvReader.HeaderRecord.Select(x => x.Replace(" ", options.ReplaceHeaderWhitespaceWith)).ToList();

            while (csvReader.Read())
            {
                cancellationToken.ThrowIfCancellationRequested();

                var innerList = new List<object>();
                for (var index = 0; index < csvReader.HeaderRecord.Length; index++)
                {
                    var obj = csvReader.GetField(index);
                    innerList.Add(obj);
                }
                resultData.Add(innerList);
            }
        }
        else if (!options.ContainsHeaderRow && !input.ColumnSpecifications.Any())
        {
            if (!csvReader.Read())
                throw new ArgumentException("CSV input can not be empty");

            headers = csvReader.Parser.Record.Select((x, index) => index.ToString()).ToList();
            resultData.Add(new List<object>(csvReader.Parser.Record));
            while (csvReader.Read())
            {
                cancellationToken.ThrowIfCancellationRequested();

                var innerList = new List<object>();
                for (var index = 0; index < headers.Count; index++)
                {
                    var obj = csvReader.GetField(index);
                    innerList.Add(obj);
                }
                resultData.Add(innerList);
            }
        }

        var xmlResult = WriteXmlString(resultData, cultureInfo, headers);
        var jtokenResult = WriteJToken(resultData, cultureInfo, headers);

        return new Result(true, resultData, headers, cultureInfo.Name, jtokenResult, xmlResult);
    }

    /// <summary>
    /// Converts enum to type definition, set useNullables to true to return nullable types
    /// </summary>
    /// <param name="code">ColumnType enum</param>
    /// <param name="useNullables">If set to true, returns types as nullables. Used for TreatMissingFieldsAsNulls option</param>
    /// <returns></returns>
    private static Type ToType(ColumnType code, bool useNullables)
    {
        return code switch
        {
            ColumnType.Boolean => useNullables ? typeof(bool?) : typeof(bool),
            ColumnType.Char => useNullables ? typeof(char?) : typeof(char),
            ColumnType.DateTime => useNullables ? typeof(DateTime?) : typeof(DateTime),
            ColumnType.Decimal => useNullables ? typeof(decimal?) : typeof(decimal),
            ColumnType.Double => useNullables ? typeof(double?) : typeof(double),
            ColumnType.Int => useNullables ? typeof(int?) : typeof(int),
            ColumnType.Long => useNullables ? typeof(long?) : typeof(long),
            ColumnType.String => typeof(string),
            _ => null,
        };
    }

    private static string WriteXmlString(IEnumerable<List<object>> data, CultureInfo culture, IReadOnlyList<string> headers)
    {
        using var ms = new MemoryStream();
        using (var writer = new XmlTextWriter(ms, new UTF8Encoding(false)) { Formatting = Formatting.Indented })
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("Root");

            foreach (var row in data)
            {
                writer.WriteStartElement("Row");

                for (var i = 0; i < headers.Count; i++)
                    writer.WriteElementString(headers[i], Convert.ToString(row[i], culture));

                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }
        return Encoding.UTF8.GetString(ms.ToArray());
    }

    private static object WriteJToken(IEnumerable<List<object>> data, CultureInfo culture, IReadOnlyList<string> headers)
    {
        using var writer = new JTokenWriter();
        writer.Formatting = (Newtonsoft.Json.Formatting)Formatting.Indented;
        writer.Culture = culture;
        writer.WriteStartArray();
        foreach (var row in data)
        {
            writer.WriteStartObject();

            for (var i = 0; i < headers.Count; i++)
            {
                writer.WritePropertyName(headers[i]);
                writer.WriteValue(row[i]);
            }
            writer.WriteEndObject();
        }
        writer.WriteEndArray();
        return writer.Token;
    }
}