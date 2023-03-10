using Frends.CSV.Parse.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Frends.CSV.Parse.UnitTests;

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
        var xmlToString = result.Xml.ToString();

        Assert.AreEqual(2, result.Data.Count);
        Assert.AreEqual(2, resultJArray.Count);
        Assert.IsNotNull(xmlToString);
        Assert.IsTrue(xmlToString.Contains("<year>2000</year>"));
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
                    new ColumnSpecification() {Name = "Year", Type = ColumnType.Int},
                    new ColumnSpecification() {Name = "Car", Type = ColumnType.String},
                    new ColumnSpecification() {Name = "Mark", Type = ColumnType.String},
                    new ColumnSpecification() {Name = "Price", Type = ColumnType.Decimal}
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
        var xmlToString = result.Xml.ToString();

        Assert.AreEqual(2, result.Data.Count);
        Assert.AreEqual(2, resultJArray.Count);
        Assert.IsNotNull(xmlToString);
        Assert.IsTrue(xmlToString.Contains("<Year>2000</Year>"));
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
        var xmlToString = result.Xml.ToString();

        Assert.AreEqual(2, result.Data.Count);
        Assert.AreEqual(2, resultJArray.Count);
        Assert.IsNotNull(xmlToString);
        Assert.IsTrue(xmlToString.Contains("<0>2000</0>"));
        Assert.AreEqual("2,34", resultJArray[0]["3"].Value<string>());
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
                    new ColumnSpecification() {Name = "Int", Type = ColumnType.Int},
                    new ColumnSpecification() {Name = "String", Type = ColumnType.String},
                    new ColumnSpecification() {Name = "Decimal", Type = ColumnType.Decimal},
                    new ColumnSpecification() {Name = "Bool", Type = ColumnType.Boolean},
                    new ColumnSpecification() {Name = "Bool2", Type = ColumnType.Boolean},
                    new ColumnSpecification() {Name = "Long", Type = ColumnType.Long},
                    new ColumnSpecification() {Name = "Char", Type = ColumnType.Char},
                    new ColumnSpecification() {Name = "DateTime", Type = ColumnType.DateTime},
                    new ColumnSpecification() {Name = "DateTime2", Type = ColumnType.DateTime},
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
        var xmlToString = result.Xml.ToString();
        var resultData = result.Data;
        var itemArray = resultData[0];

        Assert.AreEqual(2, result.Data.Count);
        Assert.AreEqual(2, resultJArray.Count);
        Assert.IsNotNull(xmlToString);
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
}