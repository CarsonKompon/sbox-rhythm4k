using System.Collections.Generic;

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
    /// Check if the chart is valid
    /// </summary>
    public bool IsValid()
    {
        if(BpmChanges.Count == 0) return false;

        return true;
    }
}