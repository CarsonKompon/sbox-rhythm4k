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
    /// The total amount of notes/chain in the chart. This is calculated on game launch and is not stored in the chart file.
    /// </summary>
    public int TotalChain;

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
    /// Returns a baked time (in seconds) based on BPM changes given an offset in steps.
    /// </summary>
    public float GetTimeFromOffset(float offset)
    {
        float currentOffset = 0f;
        float currentTime = 0f;
        float bpm = BpmChanges[0].BPM;
        float offsetChange = 0f;
        foreach(BpmChange bpmChange in BpmChanges.OrderBy(o=>o.Offset))
        {
            if(bpmChange.Offset > offset) break;
            offsetChange = bpmChange.Offset - currentOffset;
            currentOffset += offsetChange;
            currentTime += (offsetChange/1000f) * ((60f/bpm)*4f);
            bpm = bpmChange.BPM;
        }
        offsetChange = offset - currentOffset;
        currentTime += (offsetChange/1000f) * ((60f/bpm)*4f);
        return currentTime;
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