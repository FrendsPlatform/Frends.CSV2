using Frends.CSV.Parse.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Xml;

namespace Frends.CSV.Parse.Tests;

[TestClass]
public class UnitTests
{
    [TestMethod]
    public void ParseTest_SkipRowsWithAutomaticHeaders()
    {
        var csv = $@"asdasd
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

        var result = CSV.Parse(input, options, default);
        dynamic resultJArray = result.Jtoken;

        Assert.AreEqual(2, result.Data.Count);
        Assert.AreEqual(2, resultJArray.Count);
        Assert.IsNotNull(result.Xml);
        Assert.IsTrue(ValidateXml(result.Xml));
        Assert.IsTrue(result.Xml.Contains("<year>2000</year>"));
        Assert.AreEqual("2,34", resultJArray[0].price.ToString());
    }

    [TestMethod]
    public void ParseTest_WithColumnSpecAndMissingHeader()
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

        var result = CSV.Parse(input, options, default);
        var resultJArray = (JArray)result.Jtoken;

        Assert.AreEqual(2, result.Data.Count);
        Assert.AreEqual(2, resultJArray.Count);
        Assert.IsNotNull(result.Xml);
        Assert.IsTrue(ValidateXml(result.Xml));
        Assert.IsTrue(result.Xml.Contains("<Year>2000</Year>"));
        Assert.AreEqual(2.34, resultJArray[0]["Price"].Value<double>());
    }

    [TestMethod]
    public void ParseTest_WithNoColumnSpecAndNoHeader()
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

        var result = CSV.Parse(input, options, default);
        var resultJArray = (JArray)result.Jtoken;

        Assert.AreEqual(2, result.Data.Count);
        Assert.AreEqual(2, resultJArray.Count);
        Assert.IsNotNull(result.Xml);
        Assert.IsTrue(ValidateXml(result.Xml));
        Assert.IsTrue(result.Xml.Contains("<Column1>2000</Column1>"));
        Assert.AreEqual("2,34", resultJArray[0]["Column4"].Value<string>());
    }

    [TestMethod]
    public void ParseTest_WillAllKindOfDataTypes()
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

        var result = CSV.Parse(input, options, default);
        var resultJArray = (JArray)result.Jtoken;
        var resultData = result.Data;
        var itemArray = resultData[0];

        Assert.AreEqual(2, result.Data.Count);
        Assert.AreEqual(2, resultJArray.Count);
        Assert.IsNotNull(result.Xml);
        Assert.IsTrue(ValidateXml(result.Xml));
        Assert.AreEqual(4294967296, resultJArray[0]["Long"].Value<long>());
        Assert.AreEqual(typeof(int), itemArray[0].GetType());
        Assert.AreEqual(1997, itemArray[0]);
        Assert.AreEqual(typeof(string), itemArray[1].GetType());
        Assert.AreEqual("Fo;rd", itemArray[1]);
        Assert.AreEqual(typeof(decimal), itemArray[2].GetType());
        Assert.AreEqual((decimal)2.34, itemArray[2]);
        Assert.AreEqual(typeof(bool), itemArray[3].GetType());
        Assert.AreEqual(true, itemArray[3]);
        Assert.AreEqual(typeof(bool), itemArray[4].GetType());
        Assert.AreEqual(true, itemArray[4]);
        Assert.AreEqual(typeof(long), itemArray[5].GetType());
        Assert.AreEqual(4294967296, itemArray[5]);
        Assert.AreEqual(typeof(char), itemArray[6].GetType());
        Assert.AreEqual('f', itemArray[6]);
        Assert.AreEqual(typeof(DateTime), itemArray[7].GetType());
        Assert.AreEqual(typeof(DateTime), itemArray[8].GetType());
    }

    [TestMethod]
    public void TestParseTreatMissingFieldsAsNullSetToTrue()
    {
        var csv =
            @"header1,header2,header3
                value1,value2,value3
                value1,value2,value3
                value1,value2";

        var input = new Input()
        {
            ColumnSpecifications = Array.Empty<ColumnSpecification>(),
            Delimiter = ",",
            Csv = csv
        };

        var options = new Options()
        {
            ContainsHeaderRow = true,
            CultureInfo = "fi-FI",
            TreatMissingFieldsAsNulls = true
        };

        var result = CSV.Parse(input, options, default);

        var resultJson = (JArray)result.Jtoken;
        Assert.IsNull(resultJson[2].Value<string>("header3"));

        Assert.IsTrue(ValidateXml(result.Xml));
        Assert.IsTrue(result.Xml.Contains("<header3 />"));

        var resultData = result.Data;
        var nullItem = resultData[2][2];

        Assert.IsNull(nullItem);
    }

    [TestMethod]
    public void TestParseTreatMissingFieldsAsNullSetToTrueNoHeader()
    {
        var csv =
            @"value1,value2,value3
              value1,value2,value3
              value1,value2";

        var input = new Input()
        {
            ColumnSpecifications = Array.Empty<ColumnSpecification>(),
            Delimiter = ",",
            Csv = csv
        };

        var options = new Options()
        {
            ContainsHeaderRow = false,
            CultureInfo = "fi-FI",
            TreatMissingFieldsAsNulls = true
        };

        var result = CSV.Parse(input, options, default);

        var resultJson = (JArray)result.Jtoken;
        Assert.IsNull(resultJson[2].Value<string>("Column3"));

        Assert.IsTrue(ValidateXml(result.Xml));
        Assert.IsTrue(result.Xml.Contains("<Column3 />"));

        var resultData = result.Data;
        var nullItem = resultData[2][2];

        Assert.IsNull(nullItem);
    }

    [TestMethod]
    public void TestParseTreatMissingFieldsAsNullSetToTrueNoHeaderWithColumnSpecifications()
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

        var result = CSV.Parse(input, options, default);
        var resultData = result.Data;
        var itemArray = resultData[1];

        Assert.IsNull(itemArray[1]);
        Assert.IsNull(itemArray[2]);
        Assert.IsNull(itemArray[3]);
        Assert.IsNull(itemArray[4]);
        Assert.IsNull(itemArray[5]);
        Assert.IsNull(itemArray[6]);
        Assert.IsNull(itemArray[7]);
    }

    [TestMethod]
    [ExpectedException(typeof(CsvHelper.MissingFieldException))]
    public void TestParseTreatMissingFieldsAsNullSetToFalse()
    {
        var csv =
            @"header1,header2,header3
                value1,value2,value3
                value1,value2,value3
                value1,value2";
        var input = new Input()
        {
            ColumnSpecifications = Array.Empty<ColumnSpecification>(),
            Delimiter = ",",
            Csv = csv
        };

        var options = new Options()
        {
            ContainsHeaderRow = true,
            CultureInfo = "fi-FI",
            TreatMissingFieldsAsNulls = false
        };

        CSV.Parse(input, options, default);
    }

    [TestMethod]
    public void TestParseTreatMissingFieldsAsNullDefaultValue()
    {
        var options = new Options();

        Assert.IsFalse(options.TreatMissingFieldsAsNulls);
    }

    [TestMethod]
    public void TestParse_WithoutTrimOption()
    {
        var options = new Options()
        {
            ContainsHeaderRow = true,
            CultureInfo = "fi-FI",
            TreatMissingFieldsAsNulls = false,
            TrimOutput = false
        };

        var csv = @"First; Second; Number; Date
Foo; bar; 100; 2000-01-01";

        var input = new Input
        {
            ColumnSpecifications = new[]
            {
                new ColumnSpecification() { Name = "First", Type = ColumnType.String },
                new ColumnSpecification() { Name = "Second", Type = ColumnType.String },
                new ColumnSpecification() { Name = "Number", Type = ColumnType.Int },
                new ColumnSpecification() { Name = "Date", Type = ColumnType.DateTime }
            },
            Delimiter = ";",
            Csv = csv
        };

        var result = CSV.Parse(input, options, default);
        Assert.AreEqual(" bar", result.Data[0][1]);
    }

    [TestMethod]
    public void TestParse_ReplaceEmptyAndWhitespaceHeaders()
    {
        var options = new Options()
        {
            ContainsHeaderRow = true,
            CultureInfo = "fi-FI",
            TreatMissingFieldsAsNulls = false,
            TrimOutput = false,
            ReplaceHeaderWhitespaceWith = "empty"
        };

        var csv = "first;; \nfirst;row;data\nsecond;row;data";

        var input = new Input
        {
            ColumnSpecifications = Array.Empty<ColumnSpecification>(),
            Delimiter = ";",
            Csv = csv
        };

        var result = CSV.Parse(input, options, default);
        Assert.AreEqual("empty", result.Headers[1]);
        Assert.AreEqual("empty", result.Headers[2]);
    }

    [TestMethod]
    public void ParseTest_WithIgnoreQuotesSetToFalse()
    {
        const string csv = @"col1;col2
""first;value"";secondValue
thirdValue;fourthValue";

        var input = new Input { Csv = csv };
        var options = new Options { IgnoreQuotes = false };

        var result = CSV.Parse(input, options, CancellationToken.None);
        var resultJArray = (JArray)result.Jtoken;

        Assert.AreEqual(2, result.Data.Count);
        Assert.AreEqual(2, resultJArray.Count);
        Assert.IsNotNull(result.Xml);
        Assert.IsTrue(ValidateXml(result.Xml));
        Assert.IsTrue(result.Xml.Contains("<col1>first;value</col1>"));
        Assert.AreEqual("fourthValue", resultJArray[1]["col2"]);
    }

    [TestMethod]
    public void ParseTest_WithIgnoreQuotesSetToTrue()
    {
        const string csv = @"col1;col2;col3
""first;value"";secondValue
thirdValue;fourthValue;fifthValue";

        var input = new Input { Csv = csv };
        var options = new Options { IgnoreQuotes = true };

        var result = CSV.Parse(input, options, CancellationToken.None);
        var resultJArray = (JArray)result.Jtoken;

        Assert.AreEqual(2, result.Data.Count);
        Assert.AreEqual(2, resultJArray.Count);
        Assert.IsNotNull(result.Xml);
        Assert.IsTrue(ValidateXml(result.Xml));
        Assert.IsTrue(result.Xml.Contains("<col1>\"first</col1>"));
        Assert.AreEqual("fifthValue", resultJArray[1]["col3"]);
    }

    private static bool ValidateXml(string xml)
    {
        var doc = new XmlDocument();
        doc.LoadXml(xml);
        return true;
    }
}