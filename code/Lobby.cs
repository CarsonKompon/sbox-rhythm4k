using Sandbox;
using System.Collections.Generic;

public partial class RhythmLobby : Entity
{
    [Net] public int MaxPlayerCount {get;set;} = 8;
    [Net] public List<long> PlayerIds {get;set;} = new();
    [Net] public long Host {get;set;}
    [Net] public bool Hidden {get;set;} = false;
    [Net] public bool Open {get;set;} = false;
    [Net] public bool InProgress {get;set;} = false;
    [Net] public int Finished {get;set;} = 0;
    [Net] private string SongName {get;set;} = "";
    private Song __Song;
    public Song Song
    {
        get
        {
            if(__Song == null || __Song.Name != SongName)
            {
                __Song = RhythmGame.GetSongFromString(SongName);
            }
            return __Song;
        }
        set
        {
            __Song = value;
        }
    }

    public RhythmLobby(){}
    public RhythmLobby(long host, string name = "Unnamed Lobby", int maxPlayerCount = 8, bool hidden = false)
    {
        Host = host;
        Name = name;
        MaxPlayerCount = maxPlayerCount;
        Hidden = hidden;
        AddPlayer(host);
        
        RhythmPlayer player = RhythmGame.GetPlayerFromId(Host);
        player.SetLobby(this);
    }

    public override void Spawn()
    {
        base.Spawn();
        Transmit = TransmitType.Always;
    }

    [ConCmd.Server]
    public static void SetChart(int lobbyIdent, string name, string difficulty)
    {
        Chart chart = RhythmGame.GetChartFromString(name, difficulty);
        if(chart != null)
        {
            RhythmLobby lobby =  RhythmGame.GetLobbyFromIdent(lobbyIdent);
            if(lobby != null)
            {
                Log.Info($"Rhythm4K: Loaded Chart '{chart.Name} ({chart.Difficulty})'");
                lobby.SetSong(chart.Song);
            }
        }
    }

    [ConCmd.Server]
    public static void StartGame(int lobbyIdent)
    {
        RhythmLobby lobby = RhythmGame.GetLobbyFromIdent(lobbyIdent);
        if(lobby != null)
        {
            Log.Info($"Rhythm4K: Starting Game in Lobby #{lobbyIdent}");
            lobby.InProgress = true;
            lobby.Finished = 0;
            foreach(Client cl in Client.All)
            {
                foreach(long id in lobby.PlayerIds)
                {
                    if(cl.PlayerId == id && cl.Pawn is RhythmPlayer player)
                    {
                        player.StartGame(To.Single(cl));
                    }
                }
            }
        }
    }

    [ConCmd.Server]
    public static void SetFinished(string idString)
    {
        long id = long.Parse(idString);
        Client client = RhythmGame.GetClientFromId(id);
        if(client?.Pawn is RhythmPlayer player)
        {
            RhythmLobby lobby = RhythmGame.GetLobbyFromIdent(player.LobbyIdent);
            if(lobby != null)
            {
                Log.Info($"Rhythm4K: Player {client.Name} finished in Lobby #{player.LobbyIdent}");
                lobby.Finished++;

                if(lobby.Finished >= lobby.PlayerIds.Count)
                {
                    lobby.ReturnToLobby();
                }
            }
        }
    }

    public void ReturnToLobby()
    {
        Log.Info($"Rhythm4K: Lobby #{NetworkIdent} returned to lobby menu");
        if(MaxPlayerCount == 1)
        {
            ReturnToSongSelect(Host);
            return;
        }
        foreach(Client cl in Client.All)
        {
            foreach(long id in PlayerIds)
            {
                if(cl.PlayerId == id && cl.Pawn is RhythmPlayer player)
                {
                    player.ReturnToLobby(To.Single(cl));
                }
            }
        }
    }

    public void ReturnToSongSelect(long id)
    {
        Client client = RhythmGame.GetClientFromId(id);
        if(client?.Pawn is RhythmPlayer player)
        {
            player.ReturnToSongSelect(To.Single(client));
        }
    }

    public void SetSong(Song song)
    {
        SongName = song.Name;
        foreach(Client client in Client.All)
        {
            if(PlayerIds.Contains(client.PlayerId))
            {
                UpdateLobbySong(To.Single(client));
            }
        }
    }

    [ClientRpc]
    public void UpdateLobbySong()
    {
        Hud.Instance.LobbyMenu.SetSong(Song);
    }

    public void AddPlayer(long id)
    {
        if(HasPlayer(id)) return;

        PlayerIds.Add(id);
    }

    public bool RemovePlayer(long id)
    {
        if(!HasPlayer(id)) return false;

        if(id == Host && PlayerIds.Count > 1)
        {
            // Migrate Hosts
            SetHost(PlayerIds[0]);
        }

        Client client = RhythmGame.GetClientFromId(id);
        if(client.Pawn is RhythmPlayer player)
        {
            player.LobbyIdent = -1;
        }

        PlayerIds.Remove(id);

        if(PlayerIds.Count == 0) return true;
        return false;
    }

    public void PlayerQuit(long id)
    {
        if(!HasPlayer(id)) return;

        if(id == Host && MaxPlayerCount == 1)
        {
            Log.Info($"Rhythm4K: Player {id} returned to song select");
            // Return to song select
            ReturnToSongSelect(id);
            return;
        }

        RemovePlayer(id);
    }

    public bool HasPlayer(long id)
    {
        foreach(long player in PlayerIds)
        {
            if(player == id) return true;
        }
        return false;
    }

    public void SetHost(long id)
    {
        Host = id;
    }
}