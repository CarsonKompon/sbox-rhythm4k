using Sandbox;
using Sandbox.UI;
using System;

[UseTemplate]
public partial class SongSelect : Panel
{
    public Chart SelectedChart = null;
    public Panel RootBody {get;set;}
    public Panel ScrollBody {get;set;}

    public Image AlbumArt {get;set;}
    public Panel DifficultyScrollBody {get;set;}
    public Label SongTitle {get;set;}
    public Label SongArtist {get;set;}
    public Label SongCharter {get;set;}
    public Label SongBpm {get;set;}
    public Button ButtonStart {get;set;}
    public Button ButtonBack {get;set;}
    public Label LabelSelectedChart {get;set;}
    public int LobbyIdent;

    private bool SinglePlayer = true;

    public static SongSelect Instance;

    public SongSelect()
    {
        SetClass("hide", true);
        Instance = this;
    }

    public void InitSongs()
    {
        if(ScrollBody.ChildrenCount == 0)
        {
            foreach(var rhythmSong in Rhythm4KSong.All)
            {
                var button = ScrollBody.AddChild<SongButton>();
                button.SetSong(rhythmSong.Song);
            }
        }
    }

    public static void SelectSong(Song song)
    {
        Instance.AlbumArt.SetTexture(song.AlbumArt);
        Instance.SongTitle.Text = song.Name;
        Instance.SongArtist.Text = song.Artist;
        Instance.SongCharter.Text = "Unknown";
        Instance.SongBpm.Text = song.BPM.ToString();
        Instance.DifficultyScrollBody.DeleteChildren();

        foreach(var chart in song.Charts)
        {
            var button = Instance.DifficultyScrollBody.AddChild<DifficultyButton>();
            button.SetChart(chart);
        }
        SelectChart(song.Charts[0]);
    }

    public static void SelectChart(Chart chart)
    {
        Instance.SelectedChart = chart;
        RhythmPlayer.SetChart(chart.Song.Name, chart.Name);
        RhythmLobby.SetChart(Instance.LobbyIdent, chart.Song.Name, chart.Name);
        foreach(var panel in Instance.DifficultyScrollBody.Children)
        {
            if(panel is DifficultyButton button)
            {
                button.SetClass("selected", button.Chart == chart);
            }
        }
        if(!Instance.ButtonStart.HasClass("hidden"))
        {
            Instance.ButtonStart.SetClass("active", true);
        }

        Instance.LabelSelectedChart.Text = $"Selected Chart: {chart.Song.Name} - {chart.Song.Artist} | {chart.Name} ({chart.Difficulty})";
    }

    public void buttonStart(Button button)
    {
        if(button.HasClass("active"))
        {
            if(SinglePlayer)
            {
                Hud.Instance.ChangeMenuState(MainMenuState.None);
                if(Local.Pawn is RhythmPlayer player)
                {
                    RhythmLobby.StartGame(player.LobbyIdent);
                }
            }
            else
            {
                RhythmLobby.SetReady(Local.PlayerId.ToString());
                Hud.Instance.ChangeMenuState(MainMenuState.Lobby);
                if(Local.Pawn is RhythmPlayer player)
                {
                    RhythmLobby.SetChart(player.LobbyIdent, SelectedChart.Song.Name, SelectedChart.Name);
                }
            }
        }
    }

    public void buttonBack(Button button)
    {
        if(SinglePlayer)
        {
            RhythmGame.LeaveLobby(Local.PlayerId.ToString());
            Hud.Instance.ChangeMenuState(MainMenuState.Title);
        }
        else
        {
            Hud.Instance.ChangeMenuState(MainMenuState.Lobby);
        }
    }

    public void Deselect()
    {
        Instance.AlbumArt.SetTexture("");
        Instance.SongTitle.Text = "";
        Instance.SongArtist.Text = "";
        Instance.SongCharter.Text = "";
        Instance.SongBpm.Text = "";
        Instance.DifficultyScrollBody.DeleteChildren();
        Instance.SelectedChart = null;
        Instance.ButtonStart.SetClass("active", false);
    }

    public void Show(bool visible = true)
    {
        SetClass("hide", !visible);
        if(visible)
        {
            Deselect();
            if(Local.Pawn is RhythmPlayer player)
            {
                RhythmLobby lobby = RhythmLobby.FindByIndex(player.LobbyIdent) as RhythmLobby;
                SinglePlayer = lobby.MaxPlayerCount == 1;
            }
            if(SinglePlayer)
            {
                ButtonStart.Text = "Start";
            }
            else
            {
                ButtonStart.Text = "Select";
            }
        }
    }
}

public partial class SongButton : Button
{
    public Song Song;

    public void SetSong(Song song)
    {
        Song = song;
        Text = Song.Name;
    }

    protected override void OnClick(MousePanelEvent e)
    {
        SongSelect.SelectSong(Song);
    }
}

public partial class DifficultyButton : Button
{
    public Chart Chart;

    public void SetChart(Chart chart)
    {
        Chart = chart;
        Text = $"{Chart.Name} ({Chart.Difficulty})";
    }

    protected override void OnClick(MousePanelEvent e)
    {
        SongSelect.SelectChart(Chart);
    }
}