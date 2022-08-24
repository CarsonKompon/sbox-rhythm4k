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
            chart.TotalChain = 0;
            
            List<BpmChange> bpmchanges = new();
            foreach(BpmChange bpmchange in chart.BpmChanges.OrderBy(o=>o.Offset))
            {
                bpmchanges.Add(bpmchange);
            }

            List<Note> holds = new();
            foreach(Note note in chart.Notes.ToList())
            {
                note.BakedTime = chart.GetTimeFromOffset(note.Offset);
                chart.TotalChain += 1;
                if(note.Length > 0f)
                {
                    note.BakedLength = chart.GetTimeFromOffset(note.Offset + note.Length) - note.BakedTime;
                    float length = note.Length;
                    float offset = note.Offset;
                    while(length >= 62.5f)
                    {
                        offset += 62.5f;
                        length -= 62.5f;
                        var hold = new Note();
                        hold.Offset = offset;
                        hold.Length = 0f;
                        hold.Lane = note.Lane;
                        hold.Type = (int)NoteType.Hold;
                        hold.BakedTime = chart.GetTimeFromOffset(offset);
                        chart.Notes.Add(hold);
                        chart.TotalChain += 1;
                    } 
                    holds.Add(note);
                }
            }
        }

		if ( !_all.Contains( this ) )
			_all.Add( this );
	}
}