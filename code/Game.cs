using Sandbox;
using System;
using System.Linq;
using System.Collections.Generic;

public static class NoteTimings
{
	public const float Error = 0.150f;
	public const float Critical = 0.046f;
}

public partial class RhythmGame : Sandbox.Game
{
	public Hud Hud {get;set;}
	[Net] public static List<Lobby> Lobbies {get;set;} = new();
	public RhythmGame()
	{
		if(IsServer)
		{
			Hud = new Hud();
		}
	}

	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );
		
		// Create a pawn for this client to play with
		var pawn = new RhythmPlayer();
		client.Pawn = pawn;

		// Make voice chat 2D
		client.VoiceStereo = false;
	}

	public override void ClientDisconnect( Client cl, NetworkDisconnectionReason reason )
	{
		base.ClientDisconnect( cl, reason );

		if(InLobby(cl.PlayerId))
		{
			LeaveLobby( cl.PlayerId.ToString() );
		}
	}

	[ConCmd.Server]
	public static void CreateLobby(string host, string name = "Unnamed Lobby", int maxPlayerCount = 8, bool hidden = false)
	{
		long id = long.Parse(host);
		Lobby lobby = new Lobby(id, name, maxPlayerCount, hidden);
		Lobbies.Add(lobby);
		
		Log.Info($"Rhythm4K: Creating Lobby '{name}' as #{lobby.NetworkIdent}");
		foreach(Client client in Client.All)
		{
			if(client.PlayerId == id)
			{
				if(client.Pawn is RhythmPlayer player)
				{
					player.SetLobby(lobby.NetworkIdent);
				}
				break;
			}
		}
	}

	[ConCmd.Server]
	public static void JoinLobby(string idString, int lobbyIdent)
	{
		long id = long.Parse(idString);
		Lobby lobby = GetLobbyFromIdent(lobbyIdent);
		if(lobby != null)
		{
			Client client = GetClientFromId(id);
			if(client != null && client.Pawn is RhythmPlayer player)
			{
				Log.Info($"Rhythm4K: Player {id} is joining lobby #{lobbyIdent}");
				lobby.AddPlayer(id);
				player.LobbyIdent = lobby.NetworkIdent;
			}
		}
	}

	[ConCmd.Server]
	public static void LeaveLobby(string idString)
	{
		long id = long.Parse(idString);
		Client client = GetClientFromId(id);
		if(client != null && client.Pawn is RhythmPlayer player && player.LobbyIdent != -1)
		{
			Lobby lobby = GetLobbyFromIdent(player.LobbyIdent);
			if(lobby != null)
			{
				Log.Info($"Rhythm4K: Player {id} is leaving Lobby #{player.LobbyIdent}");
				if(lobby.RemovePlayer(id))
				{
					// If the lobby is empty
					Log.Info($"Rhythm4K: Lobby #{lobby.NetworkIdent} has been destroyed");
					Lobbies.Remove(lobby);
					lobby.Delete();
				}
			}
		}
	}

	[ConCmd.Server]
	public static void QuitLobby(string idString)
	{
		long id = long.Parse(idString);
		Client client = GetClientFromId(id);
		if(client?.Pawn is RhythmPlayer player && player.LobbyIdent != -1)
		{
			Lobby lobby = GetLobbyFromIdent(player.LobbyIdent);
			if(lobby != null)
			{
				Log.Info($"Rhythm4K: Player {id} is quitting Lobby #{player.LobbyIdent}");
				lobby.PlayerQuit(id);
			}
		}
	}

	// [ConCmd.Server]
	// public static void QuitGame(string idString)
	// {
	// 	long id = long.Parse(idString);
	// 	Client client = GetClientFromId(id);
	// 	if(client != null && client.Pawn is RhythmPlayer player && player.LobbyIdent != -1)
	// 	{
	// 		lobby.QuitPlayer(id);
	// 		pla
	// 	}
	// }

	public static bool InLobby(long id)
	{
		foreach(Lobby lobby in Lobbies)
		{
			foreach(long player in lobby.PlayerIds)
			{
				if(player == id) return true;
			}
		}
		return false;
	}

	public static Lobby GetLobbyFromIdent(int lobbyIdent)
	{
		foreach(Lobby lobby in Lobbies)
		{
			if(lobby.NetworkIdent == lobbyIdent)
			{
				return lobby;
			}
		}
		return null;
	}

	public static Client GetClientFromId(long id)
	{
		foreach(Client client in Client.All)
		{
			if(client.PlayerId == id) return client;
		}
		return null;
	}

	public static Chart GetChartFromString(string songName, string chartName)
	{
		foreach(Rhythm4KSong r4kSong in Rhythm4KSong.All)
		{
			if(r4kSong.Song.Name == songName)
			{
				foreach(Chart chart in r4kSong.Song.Charts)
				{
					if(chart.Name == chartName)
					{
						return chart;
					}
				}
			}
		}
		return null;
	}

	public override bool CanHearPlayerVoice( Client source, Client dest )
	{
		// TODO: Make it so you can only hear people in your lobby
		return base.CanHearPlayerVoice( source, dest );
	}
}
