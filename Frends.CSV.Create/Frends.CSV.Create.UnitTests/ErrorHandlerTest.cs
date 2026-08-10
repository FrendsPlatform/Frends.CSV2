using System;
using System.Threading;
using Frends.CSV.Create.Definitions;
using NUnit.Framework;

namespace Frends.CSV.Create.UnitTests;

[TestFixture]
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

    [Test]
    public void Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        Assert.That(() =>
            CSV.Create(InvalidInput(), DefaultOptions(), CancellationToken.None),
            Throws.Exception);
    }

    [Test]
    public void Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = DefaultOptions();
        options.ThrowErrorOnFailure = false;
        var result = CSV.Create(InvalidInput(), options, CancellationToken.None);
        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
    }

    [Test]
    public void Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = DefaultOptions();
        options.ErrorMessageOnFailure = CustomErrorMessage;
        var ex = Assert.Throws<Exception>(() =>
            CSV.Create(InvalidInput(), options, CancellationToken.None));
        Assert.That(ex!.Message, Does.Contain(CustomErrorMessage));
    }
}
