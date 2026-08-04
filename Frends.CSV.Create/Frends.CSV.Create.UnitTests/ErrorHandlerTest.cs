using System;
using System.Threading;
using Frends.CSV.Create.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.CSV.Create.UnitTests;

[TestClass]
public class ErrorHandlerTest
{
    private const string CustomErrorMessage = "CustomErrorMessage";

    private static Input InvalidInput() => new Input
    {
        InputType = CreateInputType.Json,
        Delimiter = ";",
        Json = "not-valid-json",
    };

    private static Options DefaultOptions() => new Options
    {
        ThrowErrorOnFailure = true,
    };

    [TestMethod]
    public void Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        try
        {
            CSV.Create(InvalidInput(), DefaultOptions(), CancellationToken.None);
            Assert.Fail("Expected exception was not thrown.");
        }
        catch (Exception ex)
        {
            Assert.IsNotNull(ex);
        }
    }

    [TestMethod]
    public void Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = DefaultOptions();
        options.ThrowErrorOnFailure = false;
        var result = CSV.Create(InvalidInput(), options, CancellationToken.None);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
    }

    [TestMethod]
    public void Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = DefaultOptions();
        options.ErrorMessageOnFailure = CustomErrorMessage;
        try
        {
            CSV.Create(InvalidInput(), options, CancellationToken.None);
            Assert.Fail("Expected exception was not thrown.");
        }
        catch (Exception ex)
        {
            Assert.IsNotNull(ex);
            StringAssert.Contains(ex.Message, CustomErrorMessage);
        }
    }
}
