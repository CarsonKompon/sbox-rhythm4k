﻿using Sandbox;
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
			bool[] held = {
				Input.Down(InputButton.Left),
				Input.Down(InputButton.Back),
				Input.Down(InputButton.Forward),
				Input.Down(InputButton.Right),
			};

			// Hit Arrows
			List<Note> notes = Hud.Instance.GameScreen.GetNotesToHit();
			float lowestOffset = -1f;
			foreach(Note note in notes.ToList())
			{
				if(note.Arrow != null && note.Arrow.Missed) continue;
				bool hit = false;
				switch((NoteType)note.Type)
				{
					case NoteType.Hold:
						hit = held[note.Lane];
						break;
					default:
						hit = pressed[note.Lane];
						break;
				}
				if(hit)
				{
					if(lowestOffset == -1f || note.Offset < lowestOffset) lowestOffset = note.Offset;
					Score += note.Points;
					if((NoteType)note.Type == NoteType.Normal)
					{
						Combo += 1;
						if(Combo > MaxCombo) MaxCombo = Combo;
					}

					Hud.Instance.GameScreen.LivingNotes.Remove(note);
					if(note.Arrow != null)
					{
						Receptor rec = Hud.Instance.GameScreen.Lanes[note.Lane].Receptor;
						rec.Glow(note.Arrow);

						Hud.Instance.GameScreen.Arrows.Remove(note.Arrow);
						note.Arrow.Delete();
					}

					notes.Remove(note);
				}
			}

			// Remove any arrows that were skipped (if any)
			foreach(Note note in notes)
			{
				if(note.Offset < lowestOffset)
				{
					Hud.Instance.GameScreen.LivingNotes.Remove(note);
					ResetCombo();
					if(note.Arrow != null) note.Arrow.Missed = true;
				}
			}

			// TODO: Re-implement trail scoring
			// // Hit Trails
			// List<Trail> trails = Hud.Instance.GameScreen.GetTrailsToHit();
			// foreach(Trail trail in trails.ToList())
			// {
			// 	if(held[trail.Note.Lane])
			// 	{
			// 		Score += trail.Points;
			// 		Combo += 1;
			// 		if(Combo > MaxCombo) MaxCombo = Combo;

			// 		Receptor rec = Hud.Instance.GameScreen.Lanes[trail.Note.Lane].Receptor;
			// 		rec.Glow(trail);

			// 		trails.Remove(trail);
			// 	}
			// }

			// // Check missed trails
			// if(trails.Count > 0) ResetCombo();

			if(Input.Down(InputButton.Menu))
			{
				QuitTime += Time.Delta;
				if(QuitTime >= 2f)
				{
					if(Hud.Instance.GameScreen.Active)
					{
						RhythmGame.QuitLobby(Client.PlayerId.ToString());
						QuitTime = -100f;
					}
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
		Score = 0;
		Combo = 0;
		MaxCombo = 0;

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
