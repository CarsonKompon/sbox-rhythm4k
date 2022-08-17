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

	[ConCmd.Server]
	public static void CreateLobby()
	{
		
	}

	public override bool CanHearPlayerVoice( Client source, Client dest )
	{
		// TODO: Make it so you can only hear people in your lobby
		return base.CanHearPlayerVoice( source, dest );
	}
}
