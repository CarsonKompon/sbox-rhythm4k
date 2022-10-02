using Sandbox;
using System.Collections.Generic;

public partial class RhythmLobby : Entity
{
    [Net] public int MaxPlayerCount {get;set;} = 8;
    [Net] public List<Client> Clients {get;set;} = new();
    [Net] public Client Host {get;set;}
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
        Host = RhythmGame.GetClientFromId(host);
        Name = name;
        MaxPlayerCount = maxPlayerCount;
        Hidden = hidden;
        AddPlayer(Host);
        
        if(Host.Pawn is RhythmPlayer player) player.SetLobby(this);
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
            foreach(Client cl in lobby.Clients)
            {
                if(cl.Pawn is RhythmPlayer player)
                {
                    player.StartGame(To.Single(cl));
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
            player.Ready = false;
            RhythmLobby lobby = RhythmGame.GetLobbyFromIdent(player.LobbyIdent);
            if(lobby != null)
            {
                Log.Info($"Rhythm4K: Player {client.Name} finished in Lobby #{player.LobbyIdent}");
                lobby.Finished++;

                if(lobby.Finished >= lobby.Clients.Count)
                {
                    lobby.ReturnToLobby();
                }
            }
        }
    }

    [ConCmd.Server]
    public static void SetReady(string idString, bool ready = true)
    {
        long id = long.Parse(idString);
        Client client = RhythmGame.GetClientFromId(id);
        if(client?.Pawn is RhythmPlayer player)
        {
            player.Ready = ready;
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
        foreach(Client cl in Clients)
        {
            if(cl.Pawn is RhythmPlayer player)
            {
                player.ReturnToLobby(To.Single(cl));
            }
        }
    }

    public void ReturnToSongSelect(Client cl)
    {
        if(cl?.Pawn is RhythmPlayer player)
        {
            player.ReturnToSongSelect(To.Single(cl));
        }
    }

    public void SetSong(Song song)
    {
        SongName = song.Name;
        foreach(Client client in Clients)
        {
            UpdateLobbySong(To.Single(client));
        }
    }

    [ClientRpc]
    public void UpdateLobbySong()
    {
        Hud.Instance.LobbyMenu.SetSong(Song);
    }

    public void AddPlayer(Client cl)
    {
        if(HasPlayer(cl)) return;

        RhythmLobby.SetReady(cl.PlayerId.ToString(), false);

        Clients.Add(cl);
    }

    public bool RemovePlayer(Client cl)
    {
        if(!HasPlayer(cl)) return false;

        if(cl.Pawn is RhythmPlayer player)
        {
            player.LobbyIdent = -1;
        }
        Clients.Remove(cl);

        if(cl.PlayerId == Host.PlayerId && Clients.Count > 1)
        {
            // Migrate Hosts
            SetHost(Clients[0]);
        }

        if(Clients.Count == 0) return true;
        return false;
    }

    public void PlayerQuit(Client cl)
    {
        if(!HasPlayer(cl)) return;

        if(cl.PlayerId == Host.PlayerId && MaxPlayerCount == 1)
        {
            Log.Info($"Rhythm4K: Player {cl.PlayerId} returned to song select");
            // Return to song select
            ReturnToSongSelect(cl);
            return;
        }

        RemovePlayer(cl);
    }

    public bool HasPlayer(long id)
    {
        foreach(Client client in Clients)
        {
            if(client.PlayerId == id) return true;
        }
        return false;
    }

    public bool HasPlayer(Client cl)
    {
        return Clients.Contains(cl);
    }

    public void SetHost(Client cl)
    {
        Host = cl;
    }
}