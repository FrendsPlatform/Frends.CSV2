using Frends.CSV.Create.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.CSV.Create.UnitTests;

[TestClass]
public class UnitTests
{
    private readonly string _xml =
        @"<?xml version=""1.0""?>
<catalog>
   <info>Books</info>
   <date>2023-07-27</date>
   <book id = ""bk101"">
       <author>Gambardella, Matthew</author>
             <title>XML Developer's Guide</title>
             <genre>Computer</genre>
             <price>44.95</price>  
             <publish_date>2000-10-01</publish_date>      
             <description>An in-depth look at creating applications with XML.</description>
   </book>
   <book id = ""bk102"">
       <author>Ralls, Kim</author>
          <title>Midnight Rain</title>
             <genre>Fantasy</genre>
             <price>5.95</price>     
             <publish_date>2000-12-16</publish_date> 
             <description>A former architect battles corporate zombies, an evil sorceress, and her own childhood to become queen of the world.</description>
   </book>
</catalog>
";

    private readonly string _jsonString =
        @"
[
	{
		""user_data"": {
		  ""login"": ""user1"",
		  ""phone"": ""123321111"",
		  ""contact"": {
			""emails"": [
				""user11@frends.com"",
                ""user12@frends.com""
			]
          },
		},
		""roles"": [
            {""roles_list1"": [
                ""role1_1"",
                ""role1_2"",
             ]
            },
            {""roles_list2"": [
                ""role2_1"",
                ""role2_2"",
             ]
            },

		],
		""activation_type"": ""password""
	},
		{
		""user_data"": {
		  ""login"": ""user2"",
		  ""phone"": ""123322222"",
		  ""contact"": {
			""emails"": [
				""user21@frends.com"",
                ""user22@frends.com"",
				""user23@frends.com""
			]
          },
		},
		""roles"": [
            {""roles_list1"": null
            },
            {""roles_list2"": [
                ""role2_1"",
             ]
            },

		],
		""activation_type"": ""password""
	},
]";

    [TestMethod]
    public void CreateTest_WriteFromListTable()
    {
        var date = new DateTime(2000, 1, 1);
        var headers = new List<string>() { "Dosage", "Drug", "Patient", "Date" };

        var data = new List<List<object>>()
        {
            new List<object>() { 25, "Indocin", "David", date },
            new List<object>() { 50, "Enebrel", "Sam", date },
            new List<object>() { 10, "Hydralazine", "Christoff", date },
            new List<object>() { 21, "Combiv;ent", "Janet", date },
            new List<object>() { 100, "Dilantin", "Melanie", date }
        };

        var input = new Input()
        {
            InputType = CreateInputType.List,
            Delimiter = ";",
            Data = data,
            Headers = headers
        };

        var options = new Options() { CultureInfo = "fi-FI" };

        var result = CSV.Create(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(
            $"Dosage;Drug;Patient;Date{Environment.NewLine}25;Indocin;David;1.1.2000 0.00.00{Environment.NewLine}50;Enebrel;Sam;1.1.2000 0.00.00{Environment.NewLine}10;Hydralazine;Christoff;1.1.2000 0.00.00{Environment.NewLine}21;\"Combiv;ent\";Janet;1.1.2000 0.00.00{Environment.NewLine}100;Dilantin;Melanie;1.1.2000 0.00.00{Environment.NewLine}",
            result.CSV
        );
    }

    [TestMethod]
    public void CreateTest_WriteFromJSON()
    {
        var correct_result =
            @"user_data.login;user_data.phone;user_data.contact.emails[0];user_data.contact.emails[1];user_data.contact.emails[2];roles[0].roles_list1[0];roles[0].roles_list1[1];roles[1].roles_list2[0];roles[1].roles_list2[1];activation_type
user1;123321111;user11@frends.com;user12@frends.com;;role1_1;role1_2;role2_1;role2_2;password
user2;123322222;user21@frends.com;user22@frends.com;user23@frends.com;;;role2_1;;password
";

        var input = new Input()
        {
            InputType = CreateInputType.Json,
            Delimiter = ";",
            Json = _jsonString
        };

        var options = new Options() { };

        var result = CSV.Create(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(correct_result, result.CSV);
    }

    [TestMethod]
    public void CreateTest_WriteFromXML()
    {
        var input = new Input()
        {
            InputType = CreateInputType.Xml,
            Delimiter = ";",
            Xml = _xml,
            XmlNodeElementName = "book"
        };

        var options = new Options() { };

        var result = CSV.Create(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(
            $"author;title;genre;price;publish_date;description{Environment.NewLine}Gambardella, Matthew;XML Developer's Guide;Computer;44.95;2000-10-01;An in-depth look at creating applications with XML.{Environment.NewLine}Ralls, Kim;Midnight Rain;Fantasy;5.95;2000-12-16;A former architect battles corporate zombies, an evil sorceress, and her own childhood to become queen of the world.{Environment.NewLine}",
            result.CSV
        );
    }

    [TestMethod]
    public void CreateTest_WriteFromXMLWithoutHeaders()
    {
        var input = new Input()
        {
            InputType = CreateInputType.Xml,
            Delimiter = ";",
            Xml = _xml,
            XmlNodeElementName = "book"
        };

        var options = new Options() { IncludeHeaderRow = false };

        var result = CSV.Create(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(
            $"Gambardella, Matthew;XML Developer's Guide;Computer;44.95;2000-10-01;An in-depth look at creating applications with XML.{Environment.NewLine}Ralls, Kim;Midnight Rain;Fantasy;5.95;2000-12-16;A former architect battles corporate zombies, an evil sorceress, and her own childhood to become queen of the world.{Environment.NewLine}",
            result.CSV
        );
    }

    [TestMethod]
    public void CreateTest_WriteFromXMLWithRoot()
    {
        var xml =
            @"<?xml version=""1.0""?>
<catalog>
    <book id = ""bk101"">
        <author>Gambardella, Matthew</author>
        <title>XML Developer's Guide</title>
        <genre>Computer</genre>
        <price>44.95</price>  
        <publish_date>2000-10-01</publish_date>      
        <description>An in-depth look at creating applications with XML.</description>
    </book>
    <book id = ""bk102"">
        <author>Ralls, Kim</author>
        <title>Midnight Rain</title>
        <genre>Fantasy</genre>
        <price>5.95</price>     
        <publish_date>2000-12-16</publish_date> 
        <description>A former architect battles corporate zombies, an evil sorceress, and her own childhood to become queen of the world.</description>
    </book>
</catalog>
";
        var input = new Input()
        {
            InputType = CreateInputType.Xml,
            Delimiter = ";",
            Xml = xml,
        };

        var options = new Options() { };

        var result = CSV.Create(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(
            $"author;title;genre;price;publish_date;description{Environment.NewLine}Gambardella, Matthew;XML Developer's Guide;Computer;44.95;2000-10-01;An in-depth look at creating applications with XML.{Environment.NewLine}Ralls, Kim;Midnight Rain;Fantasy;5.95;2000-12-16;A former architect battles corporate zombies, an evil sorceress, and her own childhood to become queen of the world.{Environment.NewLine}",
            result.CSV
        );
    }

    [TestMethod]
    public void CreateTest_WriteFromXMLWithoutHeadersWithRoot()
    {
        var xml =
            @"<?xml version=""1.0""?>
<catalog>
    <book id = ""bk101"">
        <author>Gambardella, Matthew</author>
        <title>XML Developer's Guide</title>
        <genre>Computer</genre>
        <price>44.95</price>  
        <publish_date>2000-10-01</publish_date>      
        <description>An in-depth look at creating applications with XML.</description>
    </book>
    <book id = ""bk102"">
        <author>Ralls, Kim</author>
        <title>Midnight Rain</title>
        <genre>Fantasy</genre>
        <price>5.95</price>     
        <publish_date>2000-12-16</publish_date> 
        <description>A former architect battles corporate zombies, an evil sorceress, and her own childhood to become queen of the world.</description>
    </book>
</catalog>
";
        var input = new Input()
        {
            InputType = CreateInputType.Xml,
            Delimiter = ";",
            Xml = xml
        };

        var options = new Options() { IncludeHeaderRow = false };

        var result = CSV.Create(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(
            $"Gambardella, Matthew;XML Developer's Guide;Computer;44.95;2000-10-01;An in-depth look at creating applications with XML.{Environment.NewLine}Ralls, Kim;Midnight Rain;Fantasy;5.95;2000-12-16;A former architect battles corporate zombies, an evil sorceress, and her own childhood to become queen of the world.{Environment.NewLine}",
            result.CSV
        );
    }

    [TestMethod]
    public void CreateTest_NullInputValue()
    {
        var json =
            @"[{""ShouldStayNull"":""null"", ""ShouldBeReplaced"": null, ""ShouldStayEmpty"": """"}]";
        ;

        var input = new Input()
        {
            InputType = CreateInputType.Json,
            Delimiter = ";",
            Json = json
        };

        var options = new Options() { ReplaceNullsWith = "replacedvalue" };

        var result = CSV.Create(input, options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(
            $"ShouldStayNull;ShouldBeReplaced;ShouldStayEmpty{Environment.NewLine}null;replacedvalue;{Environment.NewLine}",
            result.CSV
        );
    }

    [TestMethod]
    public void CreateTest_NeverAddQuotesAroundValues_false()
    {
        var json =
            @"[{
""foo"" : "" Normally I would have quotes "",
""bar"" : ""I would not""
}]";

        var input = new Input()
        {
            InputType = CreateInputType.Json,
            Delimiter = ";",
            Json = json
        };

        var options = new Options() { NeverAddQuotesAroundValues = false };

        var result = CSV.Create(input, options, default);
        Assert.AreEqual(
            $@"foo;bar{Environment.NewLine}"" Normally I would have quotes "";I would not{Environment.NewLine}",
            result.CSV
        );
    }

    [TestMethod]
    public void CreateTest_NeverAddQuotesAroundValues_true()
    {
        var json =
            @"[{
""foo"" : "" Normally I would have quotes "",
""bar"" : ""I would not""
}]";

        var input = new Input()
        {
            InputType = CreateInputType.Json,
            Delimiter = ";",
            Json = json
        };

        var options = new Options() { NeverAddQuotesAroundValues = true };

        var result = CSV.Create(input, options, default);
        Assert.AreEqual(
            $"foo;bar{Environment.NewLine} Normally I would have quotes ;I would not{Environment.NewLine}",
            result.CSV
        );
    }

    [TestMethod]
    public void CreateTest_DatetimeValue()
    {
        var json =
            @"[{
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
        Assert.AreEqual(
            $"datetime;string{Environment.NewLine}2018-11-22T10:30:55;foo{Environment.NewLine}",
            result.CSV
        );
    }

    [TestMethod]
    public void CreateTest_DecimalValues()
    {
        var json =
            @"[{
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
        Assert.AreEqual(
            $"foo;bar;baz{Environment.NewLine}0.1;1.00;0.000000000000000000000000000000000000000000000000000000001{Environment.NewLine}",
            result.CSV
        );
    }
}
