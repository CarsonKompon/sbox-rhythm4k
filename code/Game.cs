using Sandbox;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class RhythmGame : Sandbox.Game
{
	public Hud Hud {get;set;}
	[Net] public static List<Song> Songs {get;set;}
	public RhythmGame()
	{
		if(IsServer)
		{
			Hud = new Hud();
		}
		LoadContent();
	}

	[Event.Hotload]
	public static void LoadContent()
	{
		Songs = new();
		foreach(TypeDescription _td in TypeLibrary.GetDescriptions<ChartBase>())
		{
			ChartBase chart = TypeLibrary.Create<ChartBase>(_td.TargetType);
			if(chart.JsonPaths.Count > 0)
			{
				foreach(var jsonPath in chart.JsonPaths)
				{
					Songs.Add(FileSystem.Mounted.ReadJson<Song>(jsonPath));
				}
			}
			else if(chart.JsonPath.Length > 0)
			{
				Song song = FileSystem.Mounted.ReadJson<Song>(chart.JsonPath);
				Log.Info(song.Name);
				Songs.Add(song);
			}
		}
		Log.Info(Songs[0]);
	}

	/// <summary>
	/// A client has joined the server. Make them a pawn to play with
	/// </summary>
	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		// Create a pawn for this client to play with
		var pawn = new Pawn();
		client.Pawn = pawn;

		// Make voice chat 2D
		client.VoiceStereo = false;
	}

	public override bool CanHearPlayerVoice( Client source, Client dest )
	{
		// TODO: Make it so you can only hear people in your lobby
		return base.CanHearPlayerVoice( source, dest );
	}
}
