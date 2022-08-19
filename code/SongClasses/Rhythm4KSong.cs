using Sandbox;
using System;
using System.Collections.Generic;

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
        }

		if ( !_all.Contains( this ) )
			_all.Add( this );
	}
}