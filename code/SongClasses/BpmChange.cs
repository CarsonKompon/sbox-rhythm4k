

public class BpmChange
{
    /// <summary>
    /// When the BPM Change appears in ticks. 1000 ticks == 1 measure.
    /// </summary>
    public float Offset {get;set;}

    /// <summary>
    /// The new BPM
    /// </summary>
    public float BPM {get;set;}

    /// <summary>
    /// The time in seconds that the note appears. This is calculated on game launch and is not stored in the chart file.
    /// </summary>
    public float BakedTime;
}