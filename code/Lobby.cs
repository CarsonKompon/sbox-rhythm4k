using Sandbox;
using System.Collections.Generic;

public partial class Lobby : Entity
{
    [Net] public string Name {get;set;} = "Unnamed Lobby";
    [Net] public int MaxPlayerCount {get;set;} = 8;
    [Net] public List<long> PlayerIds {get;set;} = new();
    [Net] public long Host {get;set;}
    [Net] public bool Hidden {get;set;} = false;
    [Net] public bool InProgress {get;set;} = false;
    public Chart Chart;

    public Lobby(){}
    public Lobby(long host, string name = "Unnamed Lobby", int maxPlayerCount = 8, bool hidden = false)
    {
        Host = host;
        Name = name;
        MaxPlayerCount = maxPlayerCount;
        Hidden = hidden;
        AddPlayer(host);
    }

    [ConCmd.Server]
    public static void SetChart(int lobbyIdent, string name, string difficulty)
    {
        Chart chart = RhythmGame.GetChartFromString(name, difficulty);
        if(chart != null)
        {
            Lobby lobby =  RhythmGame.GetLobbyFromIdent(lobbyIdent);
            if(lobby != null)
            {
                lobby.Chart = chart;
            }
        }
    }

    [ConCmd.Server]
    public static void StartGame(int lobbyIdent)
    {
        Lobby lobby = RhythmGame.GetLobbyFromIdent(lobbyIdent);
        if(lobby != null)
        {
            lobby.InProgress = true;
            foreach(Client cl in Client.All)
            {
                foreach(long id in lobby.PlayerIds)
                {
                    if(cl.PlayerId == id && cl.Pawn is RhythmPlayer player)
                    {
                        Log.Info(lobby.Chart.Song);
                        player.StartGame(To.Single(cl), lobby.Chart.Song.Name, lobby.Chart.Name);
                    }
                }
            }
        }
    }

    public void AddPlayer(long id)
    {
        if(HasPlayer(id)) return;

        PlayerIds.Add(id);
    }

    public void RemovePlayer(long id)
    {
        if(!HasPlayer(id)) return;

        PlayerIds.Remove(id);

        // Migrate hosts
        if(PlayerIds.Count >= 1)
        {
            SetHost(PlayerIds[0]);
        }
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