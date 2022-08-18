using Sandbox;
using Sandbox.UI;
using System;

[UseTemplate]
public partial class SongSelect : Panel
{
    public Chart SelectedChart;
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
            chart.Song = song;
            var button = Instance.DifficultyScrollBody.AddChild<DifficultyButton>();
            button.SetChart(chart);
        }
        SelectChart(song.Charts[0]);
    }

    public static void SelectChart(Chart chart)
    {
        Instance.SelectedChart = chart;
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
            Hud.Instance.ChangeMenuState(MainMenuState.None);
            Hud.Instance.GameScreen.StartSongClient(SelectedChart);
        }
    }

    public void buttonBack(Button button)
    {
        RhythmGame.LeaveLobby(Local.PlayerId.ToString());
        Hud.Instance.ChangeMenuState(MainMenuState.Title);
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