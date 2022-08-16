using System;
using System.Collections.Generic;

public class SongPackBase : Attribute
{
    /// <summary>
    /// A string path to the .r4k file
    /// </summary>
    public virtual string JsonPath {get;set;} = "";

    /// <summary>
    /// A string path to the .r4k file
    /// </summary>
    public virtual List<string> JsonPaths {get;set;} = new();
}