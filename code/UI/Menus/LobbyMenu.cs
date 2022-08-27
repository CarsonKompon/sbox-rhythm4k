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

        foreach(long id in Lobby.PlayerIds)
        {
            if(!existingPlayers.Contains(id))
            {
                PlayerButton playerButton = PlayerList.AddChild<PlayerButton>();
                playerButton.SetPlayer(RhythmGame.GetClientFromId(id));
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

    public void buttonReady()
    {
        // TODO: Ready up or start+
    }

    public void buttonSongSelect()
    {
        if(Local.PlayerId == Lobby.Host)
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
    public Image Status;
    public long PlayerId;

    public PlayerButton()
    {
        Pfp = AddChild<Image>();
        Name = AddChild<Label>();
        Status = AddChild<Image>();

        Pfp.AddClass("pfp");
        Name.AddClass("name");
        Status.AddClass("status");
    }

    public void SetPlayer(Client cl)
    {
        PlayerId = cl.PlayerId;
        Name.Text = cl.Name;
        Pfp.SetTexture($"avatar:{PlayerId}");
    }

	protected override void OnClick( MousePanelEvent e )
	{
        Log.Info("nfbnfsunfdsi");
	}
}