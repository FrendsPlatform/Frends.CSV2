using Frends.CSV.Create.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.CSV.Create.UnitTests;

[TestClass]
public class UnitTests
{
    [TestMethod]
    public void CreateTest_WriteFromListTable()
    {
        var date = new DateTime(2000, 1, 1);
        var headers = new List<string>()
        {
            "Dosage",
            "Drug",
            "Patient",
            "Date"
        };

        var data = new List<List<object>>()
        {
            new List<object>() {25, "Indocin", "David", date},
            new List<object>() {50, "Enebrel", "Sam", date},
            new List<object>() {10, "Hydralazine", "Christoff", date},
            new List<object>() {21, "Combiv;ent", "Janet", date},
            new List<object>() {100, "Dilantin", "Melanie", date}
        };

        var input = new Input()
        {
            InputType = CreateInputType.List,
            Delimiter = ";",
            Data = data,
            Headers = headers
        };

        var options = new Options()
        {
            CultureInfo = "fi-FI"
        };

        var result = CSV.Create(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual($"Dosage;Drug;Patient;Date{Environment.NewLine}25;Indocin;David;1.1.2000 0.00.00{Environment.NewLine}50;Enebrel;Sam;1.1.2000 0.00.00{Environment.NewLine}10;Hydralazine;Christoff;1.1.2000 0.00.00{Environment.NewLine}21;\"Combiv;ent\";Janet;1.1.2000 0.00.00{Environment.NewLine}100;Dilantin;Melanie;1.1.2000 0.00.00{Environment.NewLine}", result.CSV);
    }

    [TestMethod]
    public void CreateTest_WriteFromJSON()
    {
        var json = @"[{""cool"":""nice"", ""what"": ""no""}, {""cool"":""not"", ""what"": ""yes""}, {""cool"":""maybe"", ""what"": ""never""}]";

        var input = new Input()
        {
            InputType = CreateInputType.Json,
            Delimiter = ";",
            Json = json
        };

        var options = new Options() { };

        var result = CSV.Create(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual($"cool;what{Environment.NewLine}nice;no{Environment.NewLine}not;yes{Environment.NewLine}maybe;never{Environment.NewLine}", result.CSV);
    }

    [TestMethod]
    public void CreateTest_NullInputValue()
    {
        var json = @"[{""ShouldStayNull"":""null"", ""ShouldBeReplaced"": null}]"; ;

        var input = new Input()
        {
            InputType = CreateInputType.Json,
            Delimiter = ";",
            Json = json
        };

        var options = new Options()
        {
            ReplaceNullsWith = "replacedvalue"
        };

        var result = CSV.Create(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual($"ShouldStayNull;ShouldBeReplaced{Environment.NewLine}null;replacedvalue{Environment.NewLine}", result.CSV);
    }

    [TestMethod]
    public void CreateTest_NeverAddQuotesAroundValues_false()
    {
        var json = @"[{
""foo"" : "" Normally I would have quotes "",
""bar"" : ""I would not""
}]";

        var input = new Input()
        {
            InputType = CreateInputType.Json,
            Delimiter = ";",
            Json = json
        };

        var options = new Options()
        {
            NeverAddQuotesAroundValues = false
        };

        var result = CSV.Create(input, options, default);
        Assert.AreEqual($@"foo;bar{Environment.NewLine}"" Normally I would have quotes "";I would not{Environment.NewLine}", result.CSV);
    }

    [TestMethod]
    public void CreateTest_NeverAddQuotesAroundValues_true()
    {
        var json = @"[{
""foo"" : "" Normally I would have quotes "",
""bar"" : ""I would not""
}]";

        var input = new Input()
        {
            InputType = CreateInputType.Json,
            Delimiter = ";",
            Json = json
        };

        var options = new Options()
        {
            NeverAddQuotesAroundValues = true
        };

        var result = CSV.Create(input, options, default);
        Assert.AreEqual($"foo;bar{Environment.NewLine} Normally I would have quotes ;I would not{Environment.NewLine}", result.CSV);
    }

    [TestMethod]
    public void CreateTest_DatetimeValue()
    {
        var json = @"[{
""datetime"" : ""2018-11-22T10:30:55"",
""string"" : ""foo""
}]";

        var input = new Input()
        {
            InputType = CreateInputType.Json,
            Delimiter = ";",
            Json = json
        };

        var options = new Options() { };

        var result = CSV.Create(input, options, default);
        Assert.AreEqual($"datetime;string{Environment.NewLine}2018-11-22T10:30:55;foo{Environment.NewLine}", result.CSV);
    }

    [TestMethod]
    public void CreateTest_DecimalValues()
    {
        var json = @"[{
""foo"" : 0.1,
""bar"" : 1.00,
""baz"" : 0.000000000000000000000000000000000000000000000000000000001
}]";

        var input = new Input()
        {
            InputType = CreateInputType.Json,
            Delimiter = ";",
            Json = json
        };

        var options = new Options() { };

        var result = CSV.Create(input, options, default);
        Assert.AreEqual($"foo;bar;baz{Environment.NewLine}0.1;1.00;0.000000000000000000000000000000000000000000000000000000001{Environment.NewLine}", result.CSV);
    }
}
