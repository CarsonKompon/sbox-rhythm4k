public enum NoteType
{
    Normal,
    Mine,
    Hold = 999,
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

    /// <summary>
    /// The length of the note/trail in seconds. This is calculated on game launch and is not stored in the chart file.
    /// </summary>
    public float BakedLength;

    /// <summary>
    /// The amount of points the note is worth. This is calculated on game launch and is not stored in the chart file.
    /// </summary>
    public int Points = 1;

    /// <summary>
    /// The Arrow associated with the Note (if there is one)
    /// </summary>
    public Arrow Arrow = null;
}