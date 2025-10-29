using System.Runtime.InteropServices;
using Frends.CSV.Create.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.CSV.Create.UnitTests;

[TestClass]
public class UnitTests
{
    private const string Xml = @"<?xml version=""1.0""?>
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

    private const string JsonString = @"
[
    {
        ""user_data"":{
         ""login"":""user1"",
         ""phone"":""123321111"",
         ""contact"":{
            ""emails"":[
               ""user11@frends.com"",
               ""user12@frends.com""
            ]
         }
      },
      ""roles"":[
         {
            ""roles_list1"":[
               ""role1_1"",
               ""role1_2""
            ]
         },
         {
            ""roles_list2"":[
               ""role2_1"",
               ""role2_2""
            ]
         }
      ],
      ""activation_type"":""password""
   },
   {
      ""user_data"":{
         ""login"":""user2"",
         ""phone"":""123322222"",
         ""contact"":{
            ""emails"":[
               ""user21@frends.com"",
               ""user22@frends.com"",
               ""user23@frends.com""
            ]
         }
      },
      ""roles"":[
         {
            ""roles_list1"":null
         },
         {
            ""roles_list2"":[
               ""role2_1""
            ]
         }
      ],
      ""activation_type"":""password""
   }
]";

    [TestMethod]
    public async Task MassiveJsonLoad()
    {
        var jsonTemp = await MassiveTempFileFactory.CreateTempJsonFileAsync(600_000);
        var csvTemp = await MassiveTempFileFactory.CreateTempCsvFileAsync(600_000);
        var json = await File.ReadAllTextAsync(jsonTemp);
        var expected = await File.ReadAllTextAsync(csvTemp);

        var input = new Input
        {
            InputType = CreateInputType.Json,
            Delimiter = ";",
            Json = json,
        };
        var options = new Options();

        var result = CSV.Create(input, options, CancellationToken.None);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(expected, result.CSV);

        File.Delete(jsonTemp);
        File.Delete(csvTemp);
    }

    [TestMethod]
    public void CreateTest_WriteFromListTable()
    {
        var date = new DateTime(2000, 1, 1);
        var headers = new List<string>
        {
            "Dosage",
            "Drug",
            "Patient",
            "Date"
        };

        var data = new List<List<object>>
        {
            new() { 25, "Indocin", "David", date },
            new() { 50, "Enebrel", "Sam", date },
            new() { 10, "Hydralazine", "Christoff", date },
            new() { 21, "Combiv;ent", "Janet", date },
            new() { 100, "Dilantin", "Melanie", date }
        };

        var input = new Input
        {
            InputType = CreateInputType.List,
            Delimiter = ";",
            Data = data,
            Headers = headers
        };

        var options = new Options
        {
            CultureInfo = "fi-FI"
        };

        var result = CSV.Create(input, options, CancellationToken.None);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(
            $"Dosage;Drug;Patient;Date{Environment.NewLine}25;Indocin;David;1.1.2000 0.00.00{Environment.NewLine}50;Enebrel;Sam;1.1.2000 0.00.00{Environment.NewLine}10;Hydralazine;Christoff;1.1.2000 0.00.00{Environment.NewLine}21;\"Combiv;ent\";Janet;1.1.2000 0.00.00{Environment.NewLine}100;Dilantin;Melanie;1.1.2000 0.00.00{Environment.NewLine}",
            result.CSV);
    }

    [TestMethod]
    public void CreateTest_WriteFromJSON()
    {
        const string correctResult =
            @"user_data.login;user_data.phone;user_data.contact.emails[0];user_data.contact.emails[1];user_data.contact.emails[2];roles[0].roles_list1[0];roles[0].roles_list1[1];roles[1].roles_list2[0];roles[1].roles_list2[1];activation_type
user1;123321111;user11@frends.com;user12@frends.com;;role1_1;role1_2;role2_1;role2_2;password
user2;123322222;user21@frends.com;user22@frends.com;user23@frends.com;;;role2_1;;password
";

        var input = new Input
        {
            InputType = CreateInputType.Json,
            Delimiter = ";",
            Json = JsonString
        };

        var options = new Options();

        var result = CSV.Create(input, options, CancellationToken.None);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(correctResult, result.CSV);
    }

    [TestMethod]
    public void CreateTest_WriteFromJSONWithManualColumns()
    {
        var headers = new List<string>
        {
            "Login",
            "Phone",
            "Primary Email",
            "Secondary Email",
            "Role List 1",
            "Role List 2",
            "Activation Type"
        };

        var columns = new List<string>
        {
            "user_data.login",
            "user_data.phone",
            "user_data.contact.emails[0]",
            "user_data.contact.emails[1]",
            "roles[0].roles_list1[0]",
            "roles[1].roles_list2[0]",
            "activation_type"
        };

        var input = new Input
        {
            InputType = CreateInputType.Json,
            Delimiter = ";",
            Json = JsonString,
            SpecifyColumnsManually = true,
            Headers = headers,
            Columns = columns
        };

        var options = new Options
        {
            IncludeHeaderRow = true
        };

        var result = CSV.Create(input, options, CancellationToken.None);

        var expectedCsv =
            $"Login;Phone;Primary Email;Secondary Email;Role List 1;Role List 2;Activation Type{Environment.NewLine}" +
            $"user1;123321111;user11@frends.com;user12@frends.com;role1_1;role2_1;password{Environment.NewLine}" +
            $"user2;123322222;user21@frends.com;user22@frends.com;;role2_1;password{Environment.NewLine}";


        Assert.IsTrue(result.Success);
        Assert.AreEqual(expectedCsv, result.CSV);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateExceptionTest_EmptyManualColumns()
    {
        var input = new Input
        {
            InputType = CreateInputType.Json,
            Delimiter = ";",
            Json = JsonString,
            SpecifyColumnsManually = true,
            Columns = null
        };

        var options = new Options();

        var result = CSV.Create(input, options, CancellationToken.None);

        Assert.IsFalse(result.Success);
    }

    [TestMethod]
    public void CreateTest_WriteFromXML()
    {
        var input = new Input
        {
            InputType = CreateInputType.Xml,
            Delimiter = ";",
            Xml = Xml,
            XmlNodeElementName = "book"
        };

        var options = new Options();

        var result = CSV.Create(input, options, CancellationToken.None);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(
            $"author;title;genre;price;publish_date;description{Environment.NewLine}Gambardella, Matthew;XML Developer's Guide;Computer;44.95;2000-10-01;An in-depth look at creating applications with XML.{Environment.NewLine}Ralls, Kim;Midnight Rain;Fantasy;5.95;2000-12-16;A former architect battles corporate zombies, an evil sorceress, and her own childhood to become queen of the world.{Environment.NewLine}",
            result.CSV);
    }

    [TestMethod]
    public void CreateTest_WriteFromXMLWithoutHeaders()
    {
        var input = new Input
        {
            InputType = CreateInputType.Xml,
            Delimiter = ";",
            Xml = Xml,
            XmlNodeElementName = "book"
        };

        var options = new Options { IncludeHeaderRow = false };

        var result = CSV.Create(input, options, CancellationToken.None);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(
            $"Gambardella, Matthew;XML Developer's Guide;Computer;44.95;2000-10-01;An in-depth look at creating applications with XML.{Environment.NewLine}Ralls, Kim;Midnight Rain;Fantasy;5.95;2000-12-16;A former architect battles corporate zombies, an evil sorceress, and her own childhood to become queen of the world.{Environment.NewLine}",
            result.CSV);
    }

    [TestMethod]
    public void CreateTest_WriteFromXMLWithRoot()
    {
        const string xml = @"<?xml version=""1.0""?>
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
        var input = new Input
        {
            InputType = CreateInputType.Xml,
            Delimiter = ";",
            Xml = xml,
        };

        var options = new Options();

        var result = CSV.Create(input, options, CancellationToken.None);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(
            $"author;title;genre;price;publish_date;description{Environment.NewLine}Gambardella, Matthew;XML Developer's Guide;Computer;44.95;2000-10-01;An in-depth look at creating applications with XML.{Environment.NewLine}Ralls, Kim;Midnight Rain;Fantasy;5.95;2000-12-16;A former architect battles corporate zombies, an evil sorceress, and her own childhood to become queen of the world.{Environment.NewLine}",
            result.CSV);
    }

    [TestMethod]
    public void CreateTest_WriteFromXMLWithoutHeadersWithRoot()
    {
        const string xml = @"<?xml version=""1.0""?>
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
        var input = new Input
        {
            InputType = CreateInputType.Xml,
            Delimiter = ";",
            Xml = xml
        };

        var options = new Options { IncludeHeaderRow = false };

        var result = CSV.Create(input, options, CancellationToken.None);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(
            $"Gambardella, Matthew;XML Developer's Guide;Computer;44.95;2000-10-01;An in-depth look at creating applications with XML.{Environment.NewLine}Ralls, Kim;Midnight Rain;Fantasy;5.95;2000-12-16;A former architect battles corporate zombies, an evil sorceress, and her own childhood to become queen of the world.{Environment.NewLine}",
            result.CSV);
    }

    [TestMethod]
    public void CreateTest_NullInputValue()
    {
        const string json = @"[{""ShouldStayNull"":""null"", ""ShouldBeReplaced"": null}]";
        var input = new Input
        {
            InputType = CreateInputType.Json,
            Delimiter = ";",
            Json = json
        };

        var options = new Options
        {
            ReplaceNullsWith = "replacedvalue"
        };

        var result = CSV.Create(input, options, CancellationToken.None);
        Assert.IsTrue(result.Success);
        Assert.AreEqual($"ShouldStayNull;ShouldBeReplaced{Environment.NewLine}null;replacedvalue{Environment.NewLine}",
            result.CSV);
    }

    [TestMethod]
    public void CreateTest_NeverAddQuotesAroundValues_false()
    {
        const string json = @"[{
""foo"" : "" Normally I would have quotes "",
""bar"" : ""I would not""
}]";

        var input = new Input
        {
            InputType = CreateInputType.Json,
            Delimiter = ";",
            Json = json
        };

        var options = new Options
        {
            NeverAddQuotesAroundValues = false
        };

        var result = CSV.Create(input, options, CancellationToken.None);
        Assert.AreEqual(
            $@"foo;bar{Environment.NewLine}"" Normally I would have quotes "";I would not{Environment.NewLine}",
            result.CSV);
    }

    [TestMethod]
    public void CreateTest_NeverAddQuotesAroundValues_true()
    {
        const string json = @"[{
""foo"" : "" Normally I would have quotes "",
""bar"" : ""I would not""
}]";

        var input = new Input
        {
            InputType = CreateInputType.Json,
            Delimiter = ";",
            Json = json
        };

        var options = new Options
        {
            NeverAddQuotesAroundValues = true
        };

        var result = CSV.Create(input, options, CancellationToken.None);
        Assert.AreEqual($"foo;bar{Environment.NewLine} Normally I would have quotes ;I would not{Environment.NewLine}",
            result.CSV);
    }

    [TestMethod]
    public void CreateTest_DatetimeValue()
    {
        const string json = @"[{
""datetime"" : ""2018-11-22T10:30:55"",
""string"" : ""foo""
}]";

        var input = new Input
        {
            InputType = CreateInputType.Json,
            Delimiter = ";",
            Json = json
        };

        var options = new Options();

        var result = CSV.Create(input, options, CancellationToken.None);
        Assert.AreEqual($"datetime;string{Environment.NewLine}2018-11-22T10:30:55;foo{Environment.NewLine}",
            result.CSV);
    }

    [TestMethod]
    public void CreateTest_DecimalValues()
    {
        const string json = @"[{
""foo"" : 0.1,
""bar"" : 1.00,
""baz"" : 0.000000000000000000000000000000000000000000000000000000001
}]";

        var input = new Input
        {
            InputType = CreateInputType.Json,
            Delimiter = ";",
            Json = json
        };

        var options = new Options();

        var result = CSV.Create(input, options, CancellationToken.None);
        Assert.AreEqual(
            $"foo;bar;baz{Environment.NewLine}0.1;1.00;0.000000000000000000000000000000000000000000000000000000001{Environment.NewLine}",
            result.CSV);
    }

    [TestMethod]
    public void CreateTest_SingleObject()
    {
        var json =
            @"{
""foo"" : ""bar"",
}";

        var input = new Input
        {
            InputType = CreateInputType.Json,
            Delimiter = ";",
            Json = json
        };

        var options = new Options();

        var result = CSV.Create(input, options, CancellationToken.None);
        Assert.AreEqual(
            @"foo
bar
",
            result.CSV
        );
    }

    [TestMethod]
    public void CreateTest_JSONWithSpacesInPropertyNames()
    {
        const string json = @"{ ""property with spaces"": ""test test"" }";

        var input = new Input
        {
            InputType = CreateInputType.Json,
            Delimiter = ";",
            Json = json
        };

        var options = new Options();

        var expectedCsv = $"property with spaces{Environment.NewLine}test test{Environment.NewLine}";

        var result = CSV.Create(input, options, CancellationToken.None);

        Console.WriteLine(result.CSV);
        Assert.AreEqual(expectedCsv, result.CSV);
    }

    [TestMethod]
    public void CreateTest_JSONWithSpacesInPropertyNames2()
    {
        const string json = @"{
        ""property with spaces"": ""test test"",
        ""normal_property"": ""value"",
        ""another property with spaces"": 123
    }";

        var input = new Input
        {
            InputType = CreateInputType.Json,
            Delimiter = ";",
            Json = json
        };

        var options = new Options();

        var result = CSV.Create(input, options, CancellationToken.None);

        Assert.IsTrue(result.Success);

        var expectedCsv =
            $"property with spaces;normal_property;another property with spaces{Environment.NewLine}test test;value;123{Environment.NewLine}";

        Assert.AreEqual(expectedCsv, result.CSV);
    }
}