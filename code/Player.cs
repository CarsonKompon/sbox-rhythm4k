using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sandbox;

partial class RhythmPlayer : AnimatedEntity
{
	[Net] public int LobbyIdent {get;set;} = -1;

	[Net, Predicted] public int Score {get;set;} = 0;
	[Net, Predicted] public int MaxCombo {get;set;} = 0;
	public bool InGame = false;
	public int Combo = 0;
	/// <summary>
	/// Called when the entity is first created 
	/// </summary>
	public override void Spawn()
	{
		base.Spawn();

		//
		// Use a watermelon model
		//
		// SetModel( "models/sbox_props/watermelon/watermelon.vmdl" );

		// EnableDrawing = false;
		// EnableHideInFirstPerson = true;
		// EnableShadowInFirstPerson = true;
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		if(LobbyIdent != -1 && InGame)
		{
			bool[] pressed = {
				Input.Pressed(InputButton.Left),
				Input.Pressed(InputButton.Back),
				Input.Pressed(InputButton.Forward),
				Input.Pressed(InputButton.Right),
			};

			List<Arrow> arrows = Hud.Instance.GameScreen.GetArrowsToHit();
			Log.Info(arrows.Count);
			foreach(Arrow arrow in arrows)
			{
				if(pressed[arrow.Note.Lane])
				{
					// TODO: Give score and combo
					Hud.Instance.GameScreen.Arrows.Remove(arrow);
					arrow.Delete();
				}
			}
		}
	}

	public void SetLobby(int lobbyIdent)
	{
		LobbyIdent = lobbyIdent;
		SetLobbyClient(To.Single(Client), lobbyIdent);
	}

	[ClientRpc]
	public void SetLobbyClient(int lobbyIdent)
	{
		Hud.Instance.SetLobby(LobbyIdent);
	}

	[ClientRpc]
    public void StartGame(string name, string difficulty)
    {
        Log.Info("ouch");
        Chart chart = RhythmGame.GetChartFromString(name, difficulty);
        if(chart != null)
        {
            Hud.Instance.ChangeMenuState(MainMenuState.Game);
            Hud.Instance.GameScreen.StartSong(chart);
			InGame = true;
        }
    }

}
