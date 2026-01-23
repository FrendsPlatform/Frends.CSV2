namespace Frends.CSV.ConvertToXML.Definitions;

/// <summary>
/// Action to perform when illegal character is encountered in converted Xml node names
/// </summary>
public enum IllegalNodeNameAction
{

    /// <summary>
    /// If any illegal node name is encountered, a task will throw an error
    /// </summary>
    ThrowError = 1,

    /// <summary>
    /// Any illegal character will be replaced with "_{ASCII hexcode}_" string
    /// Node name started with number will have a prefix added 
    /// </summary>
    Overwrite = 2,
}