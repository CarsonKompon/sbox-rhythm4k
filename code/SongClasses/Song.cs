using System.Collections.Generic;
using System.Text.Json.Serialization;

public class Song
{
    /// <summary>
    /// The name of the song
    /// </summary>
    public string Name {get; set;}

    /// <summary>
    /// The artist(s) of the song
    /// </summary>
    public string Artist {get;set;}

    /// <summary>
    /// The sound file for the song
    /// </summary>
    public string Sound;

    /// <summary>
    /// The sound file for the song
    /// </summary>
    public string AlbumArt;

    /// <summary>
    /// The song's offset from the start in seconds
    /// </summary>
    public float Offset {get;set;}

    /// <summary>
    /// The start of the song's sample
    /// </summary>
    public float SampleStart {get;set;}

    /// <summary>
    /// The length of the song's sample
    /// </summary>
    public float SampleLength {get;set;}

    /// <summary>
    /// The BPM (or average BPM) of the song
    /// </summary>
    public float BPM {get;set;}

    /// <summary>
    /// A list of all charts/difficulties associated with the song
    /// </summary>
    public List<Chart> Charts {get;set;}

    /// <summary>
    /// Check if the song is valid
    /// </summary>
    public bool IsValid()
    {
        if(Charts.Count == 0) return false;
        if(Sound == "") return false;

        return true;
    }

    /// <summary>
    /// Check how many of the charts/difficulties are valid
    /// </summary>
    public int ValidCharts()
    {
        int amount = 0;
        foreach(var chart in Charts)
        {
            if(chart.IsValid()) amount++;
        }
        return amount;
    }
}