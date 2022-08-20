using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sandbox;

partial class RhythmPlayer : AnimatedEntity
{
	[Net] public int LobbyIdent {get;set;} = -1;
	public bool InGame = false;
	public int Score= 0;
	public int Combo = 0;
	public int MaxCombo  = 0;
	public float QuitTime = 0f;
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
			// Get Inputs
			bool[] pressed = {
				Input.Pressed(InputButton.Left),
				Input.Pressed(InputButton.Back),
				Input.Pressed(InputButton.Forward),
				Input.Pressed(InputButton.Right),
			};

			// Hit Arrows
			List<Arrow> arrows = Hud.Instance.GameScreen.GetArrowsToHit();
			float lowestOffset = -1f;
			foreach(Arrow arrow in arrows)
			{
				if(arrow.Missed) continue;
				if(pressed[arrow.Note.Lane])
				{
					if(lowestOffset == -1f || arrow.Note.Offset < lowestOffset) lowestOffset = arrow.Note.Offset;
					Score += arrow.Points;
					Combo += 1;
					if(Combo > MaxCombo) MaxCombo = Combo;

					Receptor rec = Hud.Instance.GameScreen.Lanes[arrow.Note.Lane].Receptor;
					rec.Glow();

					Hud.Instance.GameScreen.Arrows.Remove(arrow);
					arrow.Delete();
				}
			}

			// Remove any arrows that were skipped (if any)
			foreach(Arrow arrow in arrows)
			{
				if(arrow != null && !arrow.Missed && arrow.Note.Offset < lowestOffset)
				{
					arrow.Missed = true;
					ResetCombo();
				}
			}

			if(Input.Down(InputButton.Menu))
			{
				QuitTime += Time.Delta;
				if(QuitTime >= 3f)
				{
					RhythmGame.QuitLobby(Client.PlayerId.ToString());
					QuitTime = -100f;
				}
			}
			else
			{
				QuitTime = 0f;
			}
		}
	}

	public void ResetCombo()
	{
		Combo = 0;
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
        Chart chart = RhythmGame.GetChartFromString(name, difficulty);
        if(chart != null)
        {
            Hud.Instance.ChangeMenuState(MainMenuState.Game);
            Hud.Instance.GameScreen.StartSong(chart);
			InGame = true;
        }
    }

	[ClientRpc]
	public void ReturnToLobby()
	{
		Hud.Instance.ChangeMenuState(MainMenuState.Lobby);
		InGame = false;
	}

	[ClientRpc]
	public void ReturnToSongSelect()
	{
		Hud.Instance.GameScreen.StopMusic();
		Hud.Instance.ChangeMenuState(MainMenuState.SongSelect);
		InGame = false;
	}

}
