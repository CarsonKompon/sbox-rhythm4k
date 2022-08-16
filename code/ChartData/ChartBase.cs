using System;
using System.Collections.Generic;

public class ChartBase : Attribute
{
    /// <summary>
    /// The identifier of the song, should contain no whitespace.
    /// </summary>
    public virtual string Name {get;set;} = "unnamed_song";

    /// <summary>
    /// A string path to the .r4k file
    /// </summary>
    public virtual string JsonPath {get;set;} = null;

    /// <summary>
    /// The loaded song
    /// </summary>
    public virtual Song Song {get;set;}
}