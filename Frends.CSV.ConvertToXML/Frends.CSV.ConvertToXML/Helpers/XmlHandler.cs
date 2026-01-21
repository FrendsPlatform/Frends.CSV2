using System;
using Frends.CSV.ConvertToXML.Definitions;

namespace Frends.CSV.ConvertToXML.Helpers;

internal static class XmlHandler
{
    public static string FixedXmlNodeName(
        string nodeName,
        string nodeNamePrefix,
        IllegalNodeNameAction illegalNodeNameAction)
    {
        if (nodeNamePrefix == string.Empty) nodeNamePrefix = "_";
        
        // illegal characters
        for (var i = 0; i < nodeName.Length; i++)
        {
            var c = nodeName[i];

            if (char.IsLetterOrDigit(c) || c == '-' || c == '_' || c == '.')
            {
                continue;
            }

            if (illegalNodeNameAction == IllegalNodeNameAction.ThrowError)
                throw new Exception($"Illegal character '{c}' in nodeName \"{nodeName}\" at position {i}.");
            nodeName = $"{nodeName[..i]}_{(int)c:X2}_{nodeName[(i + 1)..]}" ;
        }
        
        // first character
        var first = nodeName[0];

        if (!(char.IsLetter(first) || first == '_'))
        {
            if (illegalNodeNameAction == IllegalNodeNameAction.ThrowError)
                throw new Exception($"Illegal first character '{first}' in nodeName \"{nodeName}\"");

            nodeName = $"{nodeNamePrefix}{nodeName}";
        }

        return nodeName;
    }
}