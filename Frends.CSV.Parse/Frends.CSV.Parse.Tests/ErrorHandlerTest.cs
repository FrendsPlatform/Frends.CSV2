using Frends.CSV.Parse.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.CSV.Parse.Tests;

[TestClass]
public class ErrorHandlerTest
{
    private const string CustomErrorMessage = "CustomErrorMessage";

    private static Input InvalidInput() => new Input
    {
        ColumnSpecifications = Array.Empty<ColumnSpecification>(),
        Delimiter = ";",
        Csv = null,
    };

    private static Options DefaultOptions() => new Options
    {
        ThrowErrorOnFailure = true,
    };

    [TestMethod]
    public void Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        var ex = Assert.ThrowsException<ArgumentNullException>(() =>
            CSV.Parse(InvalidInput(), DefaultOptions(), CancellationToken.None));
        Assert.IsNotNull(ex);
    }

    [TestMethod]
    public void Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = DefaultOptions();
        options.ThrowErrorOnFailure = false;
        var result = CSV.Parse(InvalidInput(), options, CancellationToken.None);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.IsNotNull(result.Error.Message);
    }

    [TestMethod]
    public void Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = DefaultOptions();
        options.ErrorMessageOnFailure = CustomErrorMessage;
        var ex = Assert.ThrowsException<Exception>(() =>
            CSV.Parse(InvalidInput(), options, CancellationToken.None));
        Assert.IsNotNull(ex);
        Assert.IsTrue(ex.Message.Contains(CustomErrorMessage));
    }
}
