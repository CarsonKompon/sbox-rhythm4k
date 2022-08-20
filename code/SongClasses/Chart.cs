using System.Collections.Generic;
using System.Linq;

public class Chart
{
    /// <summary>
    /// The name of the chart/difficulty
    /// </summary>
    public string Name {get;set;} = "Unnamed Chart";

    /// <summary>
    /// The name of the chart creator
    /// </summary>
    public string CharterName {get;set;} = "Unknown";

    /// <summary>
    /// The difficulty number of the song
    /// </summary>
    public int Difficulty {get;set;} = 8;

    /// <summary>
    /// A list of all BPM changes in the chart
    /// </summary>
    public List<BpmChange> BpmChanges {get;set;} = new();

    /// <summary>
    /// A list of all notes in the chart
    /// </summary>
    public List<Note> Notes {get;set;} = new();

    /// <summary>
    /// The song this chart is associated with
    /// </summary>
    public Song Song;

    /// <summary>
    /// Returns the length of the song in seconds
    /// </summary>
    public float GetSongLength()
    {
        Note lastNote = Notes.OrderBy(o=>-o.BakedTime).ToList()[0];
        return lastNote.BakedTime;
    }

    /// <summary>
    /// Returns the length of the song in beats
    /// </summary>
    public float GetSongLengthBeats()
    {
        Note lastNote = Notes.OrderBy(o=>-o.Offset).ToList()[0];
        return lastNote.Offset;
    }

    /// <summary>
    /// Check if the chart is valid
    /// </summary>
    public bool IsValid()
    {
        if(BpmChanges.Count == 0) return false;

        return true;
    }
}