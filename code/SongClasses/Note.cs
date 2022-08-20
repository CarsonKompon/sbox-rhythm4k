public enum NoteType
{
    Normal = 0,
    Mine = 1,
}

public class Note
{
    /// <summary>
    /// When the note should appear in ticks. 1000 ticks == 1 measure.
    /// </summary>
    public float Offset {get;set;}

    /// <summary>
    /// The length of the note/trail in ticks. 1000 ticks == 1 measure.
    /// </summary>
    public float Length {get;set;}

    /// <summary>
    /// The note's type (NoteType.Normal, Mine, ect)
    /// </summary>
    public int Type {get;set;}

    /// <summary>
    /// The note's lane
    /// </summary>
    public int Lane {get;set;}

    /// <summary>
    /// The time in seconds that the note appears. This is calculated on game launch and is not stored in the chart file.
    /// </summary>
    public float BakedTime;
}