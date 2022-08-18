using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;

[UseTemplate]
public partial class GameScreen : Panel
{
    public Panel RootBody {get;set;}
    public Image AlbumArt {get;set;}
    public Label DifficultyName {get;set;}
    public Label DifficultyLevel {get;set;}
    public Label SongTitle {get;set;}
    public Label SongArtist {get;set;}
    public Panel ProgressBar {get;set;}
    public Label SongTime {get;set;}
    public Label ScoreBig {get;set;}
    public Label ScoreSmall {get;set;}
    public Panel ChainContainer {get;set;}
    public Label MaxComboLabel {get;set;}
    public Panel LaneContainer {get;set;}
    public Panel ComboContainer {get;set;}
    public Label ComboLabel {get;set;}
    public Panel MultiplayerContainer {get;set;}

    public Song Song;
    public Chart Chart = null;
    public float CurrentBPM = 120f;
    public float SongLength = 120f;
    public Sound CurrentSound;
    public List<Note> Notes = new();
    public bool Active = false;

    public List<Lane> Lanes = new();

    public static GameScreen Instance;

    public GameScreen()
    {
        SetClass("hide", true);
        Instance = this;

        for(int i=0; i<4; i++)
        {
            Lane lane = LaneContainer.AddChild<Lane>();
            lane.SetLane(i);
            Lanes.Add(lane);
        }

        // for(int j=0; j<8; j++)
        // {
        //     MultiplayerScore score = MultiplayerContainer.AddChild<MultiplayerScore>();
        //     score.Position.Text = "#0";
        //     score.Name.Text = "MyUsername";
        //     score.ScoreBig.Text = "0984";
        //     score.ScoreSmall.Text = "5839";
        // }
    }

    [Event.Frame]
    private void OnFrame()
    {
        if(Local.Pawn is RhythmPlayer player)
        {
            string scoreText = string.Format("{0:D8}", player.Score);
            ScoreBig.Text = scoreText.Substring(0, 4);
            ScoreSmall.Text = scoreText.Substring(4, 4);
            MaxComboLabel.Text = player.MaxCombo.ToString();
            int combo = player.Combo;
            ComboLabel.SetClass("hide", combo < 5);
            ComboLabel.Text = combo.ToString();
        }

        if(Active)
        {
            TimeSpan time = TimeSpan.FromSeconds(CurrentSound.ElapsedTime);
            SongTime.Text = string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
            ProgressBar.Style.Width = Length.Percent((CurrentSound.ElapsedTime / SongLength) * 100f);
        }
    }

    public async void StartSongClient(Chart chart)
    {
        SetClass("hide", false);
        SetChart(chart);

        await GameTask.DelayRealtimeSeconds(2);

        StartChart();
    }

    public void SetChart(Chart chart)
    {
        Notes.Clear();

        Chart = chart;
        Song = chart.Song;
        AlbumArt.SetTexture(Song.AlbumArt);
        DifficultyName.Text = Chart.Name;
        DifficultyLevel.Text = Chart.Difficulty.ToString();
        SongTitle.Text = Song.Name;
        SongArtist.Text = Song.Artist;
        ProgressBar.Style.Width = Length.Percent(0);
        SongTime.Text = "00:00";
        ScoreBig.Text = "0000";
        ScoreSmall.Text = "0000";
        MaxComboLabel.Text = "0";
        ComboLabel.Text = "0";
        ComboContainer.SetClass("hide", true);
        CurrentBPM = Song.BPM;
        SongLength = StepsToTime(Chart.GetSongLength());
        Notes = Chart.Notes;
    }

    public void StartChart()
    {
        CurrentSound = Sound.FromScreen(Song.Sound);
        Active = true;
    }

    public void EndChart()
    {
        CurrentSound.Stop();
        Active = false;
    }

    public float TimeToSteps(float time)
    {
        return (25f * time * CurrentBPM) / 3f;
    }

    public float StepsToTime(float steps)
    {
        return (steps / 500) * (60 / CurrentBPM);
    }

}