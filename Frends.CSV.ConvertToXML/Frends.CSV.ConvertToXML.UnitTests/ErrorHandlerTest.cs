using System;
using System.Threading;
using Frends.CSV.ConvertToXML.Definitions;
using NUnit.Framework;

namespace Frends.CSV.ConvertToXML.Tests;

[TestFixture]
internal class ErrorHandlerTest
{
    private const string CustomErrorMessage = "CustomErrorMessage";

    private static Input InvalidInput() => new Input
    {
        Csv = null,
        Delimiter = ";",
        ColumnSpecifications = Array.Empty<ColumnSpecification>(),
    };

    private static Options DefaultOptions() => new Options
    {
        ContainsHeaderRow = false,
        ThrowErrorOnFailure = true,
        ErrorMessageOnFailure = string.Empty,
    };

    [Test]
    public void Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        var ex = Assert.Throws<Exception>(() =>
            CSV.ConvertToXML(InvalidInput(), DefaultOptions(), CancellationToken.None));
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public void Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = DefaultOptions();
        options.ThrowErrorOnFailure = false;
        var result = CSV.ConvertToXML(InvalidInput(), options, CancellationToken.None);
        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
        Assert.That(result.Error.Message, Is.Not.Empty);
    }

    [Test]
    public void Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = DefaultOptions();
        options.ErrorMessageOnFailure = CustomErrorMessage;
        var ex = Assert.Throws<Exception>(() =>
            CSV.ConvertToXML(InvalidInput(), options, CancellationToken.None));
        Assert.That(ex, Is.Not.Null);
        Assert.That(ex.Message, Contains.Substring(CustomErrorMessage));
    }

    [Test]
    public void Should_Use_Custom_ErrorMessageOnFailure_In_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = DefaultOptions();
        options.ThrowErrorOnFailure = false;
        options.ErrorMessageOnFailure = CustomErrorMessage;
        var result = CSV.ConvertToXML(InvalidInput(), options, CancellationToken.None);
        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
        Assert.That(result.Error.Message, Contains.Substring(CustomErrorMessage));
    }
}
