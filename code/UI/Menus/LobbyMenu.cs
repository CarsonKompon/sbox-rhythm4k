using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;
using System.Linq;

[UseTemplate]
public partial class LobbyMenu : Panel
{
    public Label LobbyName {get;set;}
    public Image AlbumArt {get;set;}
    public Label SongTitle {get;set;}
    public Label SongArtist {get;set;}
    public Panel DifficultiesList {get;set;}
    public Button ReadyButton {get;set;}
    public Panel PlayerList {get;set;}
    public RhythmLobby Lobby;
    public LobbyMenu()
    {
        SetClass("hide", true);
    }

    [Event.Frame]
    public void OnFrame()
    {
        if(Lobby != null)
        {
            if(Local.PlayerId == Lobby.Host.PlayerId)
            {
                bool canStart = true;
                foreach(Client cl in Lobby.Clients)
                {
                    if(cl.Pawn is RhythmPlayer player && !player.Ready)
                    {
                        canStart = false;
                        break;
                    }
                }
                ReadyButton.SetClass("active", canStart);
            }
        }
    }

    public void SetLobby(RhythmLobby lobby)
    {
        Lobby = lobby;
        LobbyName.Text = lobby.Name;
    }

    public void UpdatePlayerList()
    {
        List<long> existingPlayers = new();
        foreach(Panel child in PlayerList.Children.ToList())
        {
            if(child is PlayerButton button)
            {
                if(!Lobby.HasPlayer(button.PlayerId))
                {
                    button.Delete();
                }
                else
                {
                    existingPlayers.Add(button.PlayerId);
                }
            }
        }

        foreach(Client cl in Lobby.Clients)
        {
            if(!existingPlayers.Contains(cl.PlayerId))
            {
                PlayerButton playerButton = PlayerList.AddChild<PlayerButton>();
                playerButton.SetPlayer(cl);
            }
        }
    }

    public void SetSong(Song song)
    {
        AlbumArt.SetTexture(song.AlbumArt);
        SongTitle.Text = song.Name;
        SongArtist.Text = song.Artist;
    }

    public void Show(bool show = true)
    {
        if(show)
        {
            UpdatePlayerList();
        }
        SetClass("hide", !show);
    }

    public void buttonReady(Button button)
    {
        if(button.HasClass("active"))
        {
            RhythmLobby.StartGame(Lobby.NetworkIdent);
        }
    }

    public void buttonSongSelect()
    {
        if(Local.PlayerId == Lobby.Host.PlayerId)
        {
            Hud.Instance.ChangeMenuState(MainMenuState.SongSelect);
        }
    }

    public void buttonBack()
    {
        RhythmGame.LeaveLobby(Local.PlayerId.ToString());
        Hud.Instance.ChangeMenuState(MainMenuState.SearchingForLobby);
    }

    // [Event.Frame]
    // public void OnFrame()
    // {
        
    // }
}

public partial class PlayerButton : Panel
{
    public Panel Background;
    public Image Pfp;
    public Label Name;
    public Label Difficulty;
    public Image Status;
    public long PlayerId;

    public PlayerButton()
    {
        Pfp = AddChild<Image>();
        Name = AddChild<Label>();
        Difficulty = AddChild<Label>();
        Status = AddChild<Image>();

        Pfp.AddClass("pfp");
        Name.AddClass("name");
        Difficulty.AddClass("difficulty");
        Status.AddClass("status");
    }

    public void SetPlayer(Client cl)
    {
        PlayerId = cl.PlayerId;
        Name.Text = cl.Name;
        Pfp.SetTexture($"avatar:{PlayerId}");
    }

    public void SetChart(Chart chart)
    {
        Difficulty.Text = chart.Difficulty.ToString();
    }

	protected override void OnClick( MousePanelEvent e )
	{
        Log.Info("nfbnfsunfdsi");
	}
}