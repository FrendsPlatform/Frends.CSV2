namespace Frends.CSV.ConvertToJSON.Tests;

using Frends.CSV.ConvertToJSON.Definitions;
using NUnit.Framework;
using System;

/// <summary>
/// Test class.
/// </summary>
[TestFixture]
internal class TestClass
{
    [Test]
    public void Test_WithEmptyFields()
    {
        var csv = @"string,1,2023-01-01,2,200,3,true,N
              ,,,,,,,";

        var input = new Input()
        {
            ColumnSpecifications = Array.Empty<ColumnSpecification>(),
            Delimiter = ",",
            Csv = csv,
        };

        var options = new Options()
        {
            ContainsHeaderRow = false,
            SkipRowsFromTop = 0,
            SkipEmptyRows = false,
        };

        var result = CSV.ConvertToJSON(input, options, default);

    }

    [Test]
    public void ConvertToJSONTest_SkipRowsWithAutomaticHeaders()
    {
        var csv = @"asdasd
Coolio
year;car;mark;price
1997;Ford;E350;2,34
2000;Mercury;Cougar;2,38";

        var input = new Input()
        {
            ColumnSpecifications = Array.Empty<ColumnSpecification>(),
            Delimiter = ";",
            Csv = csv,
        };

        var options = new Options()
        {
            ContainsHeaderRow = true,
            SkipRowsFromTop = 2,
            SkipEmptyRows = false,
        };

        var result = CSV.ConvertToJSON(input, options, default);

    }

    [Test]
    public void ConvertToJSONTest_WithColumnSpecAndMissingHeader()
    {
        var csv = @"1997;Ford;E350;2,34
2000;Mercury;Cougar;2,38";

        var input = new Input()
        {
            ColumnSpecifications = new[]
                {
                    new ColumnSpecification() { Name = "Year", Type = ColumnType.Int },
                    new ColumnSpecification() { Name = "Car", Type = ColumnType.String },
                    new ColumnSpecification() { Name = "Mark", Type = ColumnType.String },
                    new ColumnSpecification() { Name = "Price", Type = ColumnType.Decimal },
                },
            Delimiter = ";",
            Csv = csv,
        };

        var options = new Options()
        {
            ContainsHeaderRow = false,
            CultureInfo = "fi-FI",
        };

        var result = CSV.ConvertToJSON(input, options, default);

    }

    [Test]
    public void ConvertToJSONTest_WithNoColumnSpecAndNoHeader()
    {
        var csv = @"1997;Ford;E350;2,34
2000;Mercury;Cougar;2,38";

        var input = new Input()
        {
            ColumnSpecifications = Array.Empty<ColumnSpecification>(),
            Delimiter = ";",
            Csv = csv,
        };

        var options = new Options()
        {
            ContainsHeaderRow = false,
            CultureInfo = "fi-FI",
        };

        var result = CSV.ConvertToJSON(input, options, default);

    }

    [Test]
    public void ConvertToJSONTest_WillAllKindOfDataTypes()
    {
        var csv =
@"THIS;is;header;row;with;some;random;stuff ;yes
1997;""Fo;rd"";2,34;true;1;4294967296;f;2008-09-15;2008-05-01 7:34:42Z
2000;Mercury;2,38;false;0;4294967296;g;2009-09-15T06:30:41.7752486;Thu, 01 May 2008 07:34:42 GMT";

        var input = new Input()
        {
            ColumnSpecifications = new[]
                {
                    new ColumnSpecification() { Name = "Int", Type = ColumnType.Int },
                    new ColumnSpecification() { Name = "String", Type = ColumnType.String },
                    new ColumnSpecification() { Name = "Decimal", Type = ColumnType.Decimal },
                    new ColumnSpecification() { Name = "Bool", Type = ColumnType.Boolean },
                    new ColumnSpecification() { Name = "Bool2", Type = ColumnType.Boolean },
                    new ColumnSpecification() { Name = "Long", Type = ColumnType.Long },
                    new ColumnSpecification() { Name = "Char", Type = ColumnType.Char },
                    new ColumnSpecification() { Name = "DateTime", Type = ColumnType.DateTime },
                    new ColumnSpecification() { Name = "DateTime2", Type = ColumnType.DateTime },
                },
            Delimiter = ";",
            Csv = csv,
        };

        var options = new Options()
        {
            ContainsHeaderRow = true,
            CultureInfo = "fi-FI",
        };

        var result = CSV.ConvertToJSON(input, options, default);

    }

    [Test]
    public void TestConvertToJSONTreatMissingFieldsAsNullSetToTrue()
    {
        var csv =
            @"header1,header2,header3
                value1,value2,value3
                value1,value2,value3
                value1,value2";

        var input = new Input()
        {
            ColumnSpecifications = new ColumnSpecification[0],
            Delimiter = ",",
            Csv = csv,
        };

        var options = new Options()
        {
            ContainsHeaderRow = true,
            CultureInfo = "fi-FI",
            TreatMissingFieldsAsNulls = true,
        };

        var result = CSV.ConvertToJSON(input, options, default);

    }

    [Test]
    public void TestConvertToJSONTreatMissingFieldsAsNullSetToTrueNoHeader()
    {
        var csv =
            @"value1,value2,value3
              value1,value2,value3
              value1,value2";

        var input = new Input()
        {
            ColumnSpecifications = new ColumnSpecification[0],
            Delimiter = ",",
            Csv = csv,
        };

        var options = new Options()
        {
            ContainsHeaderRow = false,
            CultureInfo = "fi-FI",
            TreatMissingFieldsAsNulls = true,
        };

        var result = CSV.ConvertToJSON(input, options, default);

    }

    [Test]
    public void TestConvertToJSONTreatMissingFieldsAsNullSetToTrueNoHeaderWithColumnSpecifications()
    {
        var csv =
            @"string,1,2023-01-01,2,200,3,true,N
              ,,,,,,,";

        var input = new Input()
        {
            ColumnSpecifications = new[]
            {
                new ColumnSpecification() { Name = "String", Type = ColumnType.String },
                new ColumnSpecification() { Name = "Decimal", Type = ColumnType.Decimal },
                new ColumnSpecification() { Name = "DateTime", Type = ColumnType.DateTime },
                new ColumnSpecification() { Name = "Int", Type = ColumnType.Int },
                new ColumnSpecification() { Name = "Long", Type = ColumnType.Long },
                new ColumnSpecification() { Name = "Double", Type = ColumnType.Double },
                new ColumnSpecification() { Name = "Boolean", Type = ColumnType.Boolean },
                new ColumnSpecification() { Name = "Char", Type = ColumnType.Char },
            },
            Delimiter = ",",
            Csv = csv,
        };

        var options = new Options()
        {
            ContainsHeaderRow = false,
            CultureInfo = "fi-FI",
            TreatMissingFieldsAsNulls = true,
        };

        var result = CSV.ConvertToJSON(input, options, default).Json.ToString();
        Console.WriteLine(result);
        Assert.AreEqual(
@"[
  {
    ""String"": ""string"",
    ""Decimal"": 1.0,
    ""DateTime"": ""2023-01-01T00:00:00"",
    ""Int"": 2,
    ""Long"": 200,
    ""Double"": 3.0,
    ""Boolean"": true,
    ""Char"": ""N""
  },
  {
    ""String"": ""              "",
    ""Decimal"": null,
    ""DateTime"": null,
    ""Int"": null,
    ""Long"": null,
    ""Double"": null,
    ""Boolean"": null,
    ""Char"": null
  }
]",
result);
    }

    [Test]
    public void TestConvertToJSONTreatMissingFieldsAsNullSetToFalse()
    {
        var csv =
            @"header1,header2,header3
                value1,value2,value3
                value1,value2,value3
                value1,value2";
        var input = new Input()
        {
            ColumnSpecifications = new ColumnSpecification[0],
            Delimiter = ",",
            Csv = csv,
        };

        var options = new Options()
        {
            ContainsHeaderRow = true,
            CultureInfo = "fi-FI",
            TreatMissingFieldsAsNulls = false,
        };

        var ex = Assert.Throws<CsvHelper.MissingFieldException>(() => CSV.ConvertToJSON(input, options, default));
        Assert.IsTrue(ex.Message.StartsWith("Field at index '2' does not exist. You can ignore missing fields by setting MissingFieldFound to null."));
    }
}
