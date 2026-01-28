using Frends.CSV.Create.Definitions;

namespace Frends.CSV.Create;

internal static class Helpers
{
    internal static string FormatBoolean(bool value, BooleanFormat format)
    {
        return format switch
        {
            BooleanFormat.Lowercase => value ? "true" : "false",
            BooleanFormat.PascalCase => value ? "True" : "False",
            BooleanFormat.Numeric => value ? "1" : "0",
            _ => value ? "true" : "false"
        };
    }
}
