using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public partial class RhythmGame : Sandbox.Game
{
	public Hud Hud {get;set;}
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

		// Get all of the spawnpoints
		var spawnpoints = Entity.All.OfType<SpawnPoint>();

		// chose a random one
		var randomSpawnPoint = spawnpoints.OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

		// if it exists, place the pawn there
		if ( randomSpawnPoint != null )
		{
			var tx = randomSpawnPoint.Transform;
			tx.Position = tx.Position + Vector3.Up * 50.0f; // raise it up
			pawn.Transform = tx;
		}

		// Make voice chat 2D
		client.VoiceStereo = false;
	}

	public override bool CanHearPlayerVoice( Client source, Client dest )
	{
		// TODO: Make it so you can only hear people in your lobby
		return base.CanHearPlayerVoice( source, dest );
	}
}
