using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

[GameResource("Rhythm4K Song", "chart", "A song to be used in Rhythm4K.")]
public class Rhythm4KSong : GameResource
{
    /// <summary>
    /// The .r4k chart file path from root
    /// </summary>
    public string ChartFile {get;set;}

    /// <summary>
    /// The sound file
    /// </summary>
    [ResourceType("sound")]
    public string Sound {get;set;}

    /// <summary>
    /// The cover art
    /// </summary>
    [ResourceType("png")]
    public string AlbumArt {get;set;}

    /// <summary>
    /// The deserialized chart file
    /// </summary>
    [HideInEditor]
    public Song Song {get;set;}



    // Access these statically with Rhythm4KSong.All

    /// <summary>
    /// All loaded songs
    /// </summary>
	public static IReadOnlyList<Rhythm4KSong> All => _all;
	internal static List<Rhythm4KSong> _all = new();

	protected override void PostLoad()
	{
		base.PostLoad();

        Song = FileSystem.Mounted.ReadJson<Song>(ChartFile);
        Song.Sound = Sound;
        Song.AlbumArt = AlbumArt;

        foreach(Chart chart in Song.Charts)
        {
            chart.Song = Song;
            
            List<BpmChange> bpmchanges = new();
            foreach(BpmChange bpmchange in chart.BpmChanges.OrderBy(o=>o.Offset))
            {
                bpmchanges.Add(bpmchange);
            }

            float bpm = 0;
            float time = 0;
            float realTime = 0;
            foreach(Note note in chart.Notes.OrderBy(o=>o.Offset))
            {
                float timeChange = note.Offset - time;
                time = note.Offset;

                // Check for BPM Changes
                foreach(BpmChange bpmchange in bpmchanges)
                {
                    if(time >= bpmchange.Offset)
                    {
                        bpm = bpmchange.BPM;
                        bpmchange.BakedTime = realTime + ((timeChange / 250f) * (60f / bpm)); 
                        bpmchanges.Remove(bpmchange);
                        break;
                    }
                }

                realTime += (timeChange / 250f) * (60f / bpm);
                note.BakedTime = realTime;
            }
        }

		if ( !_all.Contains( this ) )
			_all.Add( this );
	}
}