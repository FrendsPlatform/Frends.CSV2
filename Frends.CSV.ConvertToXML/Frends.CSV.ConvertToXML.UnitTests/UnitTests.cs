using Frends.CSV.ConvertToXML.Definitions;
using NUnit.Framework;

namespace Frends.CSV.ConvertToXML.UnitTests;

[TestFixture]
public class UnitTests
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
            Csv = csv
        };

        var options = new Options()
        {
            ContainsHeaderRow = false,
            SkipRowsFromTop = 0,
            SkipEmptyRows = false
        };

        var result = CSV.ConvertToXML(input, options, default);
        Assert.IsNotNull(result.Xml);
    }

    [Test]
    public void ConvertToXMLTest_SkipRowsWithAutomaticHeaders()
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
            Csv = csv
        };

        var options = new Options()
        {
            ContainsHeaderRow = true,
            SkipRowsFromTop = 2,
            SkipEmptyRows = false
        };

        var result = CSV.ConvertToXML(input, options, default);
        Assert.IsNotNull(result.Xml);
        Assert.IsTrue(result.Xml.Contains("<year>2000</year>"));
    }

    [Test]
    public void ConvertToXMLTest_WithColumnSpecAndMissingHeader()
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
                new ColumnSpecification() { Name = "Price", Type = ColumnType.Decimal }
            },
            Delimiter = ";",
            Csv = csv
        };

        var options = new Options()
        {
            ContainsHeaderRow = false,
            CultureInfo = "fi-FI"
        };

        var result = CSV.ConvertToXML(input, options, default);
        Assert.IsNotNull(result.Xml);
        Assert.IsTrue(result.Xml.Contains("<Year>2000</Year>"));
    }

    [Test]
    public void ConvertToXMLTest_WithNoColumnSpecAndNoHeader()
    {
        var csv = @"1997;Ford;E350;2,34
2000;Mercury;Cougar;2,38";

        var input = new Input()
        {
            ColumnSpecifications = Array.Empty<ColumnSpecification>(),
            Delimiter = ";",
            Csv = csv
        };

        var options = new Options()
        {
            ContainsHeaderRow = false,
            CultureInfo = "fi-FI"
        };

        var result = CSV.ConvertToXML(input, options, default);
        Assert.IsNotNull(result.Xml);
        Assert.IsTrue(result.Xml.Contains("<0>2000</0>"));
    }

    [Test]
    public void ConvertToXMLTest_WillAllKindOfDataTypes()
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
            Csv = csv
        };

        var options = new Options()
        {
            ContainsHeaderRow = true,
            CultureInfo = "fi-FI"
        };

        var result = CSV.ConvertToXML(input, options, default);
        Assert.IsNotNull(result.Xml);
    }

    [Test]
    public void TestConvertToXMLTreatMissingFieldsAsNullSetToTrue()
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
            Csv = csv
        };

        var options = new Options()
        {
            ContainsHeaderRow = true,
            CultureInfo = "fi-FI",
            TreatMissingFieldsAsNulls = true
        };

        var result = CSV.ConvertToXML(input, options, default);
        Assert.IsTrue(condition: result.Xml.Contains("<header3 />"));
    }

    [Test]
    public void TestConvertToXMLTreatMissingFieldsAsNullSetToTrueNoHeader()
    {
        var csv =
            @"value1,value2,value3
              value1,value2,value3
              value1,value2";

        var input = new Input()
        {
            ColumnSpecifications = new ColumnSpecification[0],
            Delimiter = ",",
            Csv = csv
        };

        var options = new Options()
        {
            ContainsHeaderRow = false,
            CultureInfo = "fi-FI",
            TreatMissingFieldsAsNulls = true
        };

        var result = CSV.ConvertToXML(input, options, default);
        Assert.IsTrue(condition: result.Xml.Contains("<2 />"));
    }

    [Test]
    public void TestConvertToXMLTreatMissingFieldsAsNullSetToTrueNoHeaderWithColumnSpecifications()
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
                new ColumnSpecification() { Name = "Char", Type = ColumnType.Char }
            },
            Delimiter = ",",
            Csv = csv
        };

        var options = new Options()
        {
            ContainsHeaderRow = false,
            CultureInfo = "fi-FI",
            TreatMissingFieldsAsNulls = true
        };

        var result = CSV.ConvertToXML(input, options, default);
        Assert.IsTrue(result.Xml.Contains("<Decimal />"));
        Assert.IsTrue(result.Xml.Contains("<DateTime />"));
        Assert.IsTrue(result.Xml.Contains("<Int />"));
        Assert.IsTrue(result.Xml.Contains("<Long />"));
        Assert.IsTrue(result.Xml.Contains("<Double />"));
        Assert.IsTrue(result.Xml.Contains("<Boolean />"));
        Assert.IsTrue(result.Xml.Contains("<Char />"));
    }

    [Test]
    public void TestConvertToXMLTreatMissingFieldsAsNullSetToFalse()
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
            Csv = csv
        };

        var options = new Options()
        {
            ContainsHeaderRow = true,
            CultureInfo = "fi-FI",
            TreatMissingFieldsAsNulls = false
        };

        var ex = Assert.Throws<CsvHelper.MissingFieldException>(() => CSV.ConvertToXML(input, options, default));
        Assert.IsTrue(ex.Message.StartsWith("Field at index '2' does not exist. You can ignore missing fields by setting MissingFieldFound to null."));
    }

    [Test]
    public void ConvertToXML_WithIgnoreQuotesSetToFalse()
    {
        const string csv = @"col1;col2
""first;value"";secondValue
thirdValue;fourthValue";
        var input = new Input
        {
            Csv = csv,
            Delimiter = ";",
            ColumnSpecifications = Array.Empty<ColumnSpecification>(),
        };
        var options = new Options { IgnoreQuotes = false };
        var result = CSV.ConvertToXML(input, options, CancellationToken.None);
        Assert.IsTrue(result.Xml.Contains("<col1>first;value</col1>"));
        Assert.IsTrue(result.Xml.Contains("<col1>thirdValue</col1>"));
    }

    [Test]
    public void ConvertToXML_WithIgnoreQuotesSetToTrue()
    {
        const string csv = @"col1;col2;col3
""first;value"";secondValue
thirdValue;fourthValue;fifthValue";
        var input = new Input
        {
            Csv = csv,
            Delimiter = ";",
            ColumnSpecifications = Array.Empty<ColumnSpecification>(),
        };
        var options = new Options { IgnoreQuotes = true };
        var result = CSV.ConvertToXML(input, options, CancellationToken.None);
        Assert.IsTrue(result.Xml.Contains("<col1>\"first</col1>"));
        Assert.IsTrue(result.Xml.Contains("<col1>thirdValue</col1>"));
    }
}