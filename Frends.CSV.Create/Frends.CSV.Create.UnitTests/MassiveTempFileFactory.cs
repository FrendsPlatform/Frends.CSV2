using System.Text.Json;

namespace Frends.CSV.Create.UnitTests;

public static class MassiveTempFileFactory
{
    private static readonly JsonSerializerOptions Opts = new JsonSerializerOptions
    {
        WriteIndented = false
    };

    private static readonly object JsonTemplate = new
    {
        ColA = "123",
        ColB = "3",
        ColC = "123",
        ColD = "1998-07-04",
        ColE = "3",
        ColF = "2005",
        ColG = "3",
        ColH = "mjuk",
        ColI = "Day1/Day2",
        ColJ = "8,00",
        ColK = "1998-07-04",
        ColL = "1998-07-04",
        ColM = "mjuk",
        ColN = "8,000000"
    };

    private const string CsvHeader =
        "ColA;ColB;ColC;ColD;ColE;ColF;ColG;ColH;ColI;ColJ;ColK;ColL;ColM;ColN";

    private const string CsvRow =
        "123;3;123;1998-07-04;3;2005;3;mjuk;Day1/Day2;8,00;1998-07-04;1998-07-04;mjuk;8,000000";

    public static async Task<string> CreateTempCsvFileAsync(int count)
    {
        var path = Path.GetTempFileName();

        await using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
        await using var writer = new StreamWriter(fs);

        // Write header
        await writer.WriteLineAsync(CsvHeader);

        // Write rows
        for (var i = 0; i < count; i++)
        {
            await writer.WriteLineAsync(CsvRow);
        }

        await writer.FlushAsync();
        return path;
    }

    public static async Task<string> CreateTempJsonFileAsync(int count)
    {
        var tempPath = Path.GetTempFileName();

        await using var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await using var writer = new StreamWriter(fs);

        await writer.WriteAsync("[");
        var json = JsonSerializer.Serialize(JsonTemplate, Opts);

        for (var i = 0; i < count; i++)
        {
            if (i > 0)
                await writer.WriteAsync(",");

            await writer.WriteAsync(json);
        }

        await writer.WriteAsync("]");
        await writer.FlushAsync();

        return tempPath;
    }
}