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
    public List<Arrow> Arrows = new();
    public bool Active = false;

    public float ScreenTime = 1f;

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
        // Instantiate new notes
        List<Note> notes = GetNextNotes();
        foreach(Note note in notes)
        {
            Lane lane = Lanes[note.Lane];
            Arrow arrow = lane.AddChild<Arrow>();
            arrow.SetNote(note);
            Arrows.Add(arrow);
            Notes.Remove(note);
        }

        foreach(Lane lane in Lanes)
        {
            foreach(Panel child in lane.Children)
            {
                if(child is Arrow arrow)
                {
                    float percent = 100f * ((StepsToTime(arrow.Note.Offset) - CurrentSound.ElapsedTime) / ScreenTime);
                    arrow.Style.Top = Length.Percent(percent);
                    if(percent <= -50f)
                    {
                        Arrows.Remove(arrow);
                        arrow.Delete();
                    }
                }
            }
        }

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

    public async void StartSong(Chart chart)
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
        Log.Info(StepsToTime(1000));
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

    public List<Arrow> GetArrowsToHit()
    {
        List<Arrow> arrows = new();
        foreach(Arrow arrow in Arrows)
        {
            float noteTime = StepsToTime(arrow.Note.Offset);
            float time = CurrentSound.ElapsedTime + Song.Offset;
            if(noteTime > time - NoteTimings.Error && noteTime < time + NoteTimings.Error)
            {
                arrows.Add(arrow);
            }
        }
        return arrows;
    }

    public List<Note> GetNextNotes()
    {
        List<Note> notes = new();
        foreach(Note note in Notes)
        {
            float time = StepsToTime(note.Offset);
            if(CurrentSound.ElapsedTime + Song.Offset >= time - ScreenTime)
            {
                notes.Add(note);
            }
        }
        return notes;
    }

    public float StepsToTime(float steps)
    {
        return (steps / 250) * (60 / CurrentBPM);
    }

    public void Show()
    {
        SetClass("hide", false);
        Log.Info("HELP");
    }

    public void Hide()
    {
        SetClass("hide", true);
    }

}