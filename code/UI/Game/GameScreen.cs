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
    public RealTimeSince CurrentTime = 0f;
    public List<Note> Notes = new();
    public List<Arrow> Arrows = new();
    public List<BpmChange> BpmChanges = new();
    public bool Active = false;
    public int CritValue = 1;

    public float ScreenTime = 0.7f;

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
            if(Active)
            {
                // Check for BPM Change
                if(BpmChanges.Count > 0)
                {
                    foreach(BpmChange bpmchange in BpmChanges)
                    {
                        if(CurrentTime >= bpmchange.BakedTime)
                        {
                            CurrentBPM = bpmchange.BPM;
                            Log.Info($"NEW BPM JUST DROPPED: {CurrentBPM}");
                            Log.Info(BpmChanges.Count);
                            BpmChanges.Remove(bpmchange);
                            break;
                        }
                    }
                }

                // Instantiate new notes
                if(Notes.Count > 0)
                {
                    List<Note> notes = GetNextNotes();
                    foreach(Note note in notes)
                    {
                        Lane lane = Lanes[note.Lane];
                        Arrow arrow = lane.AddChild<Arrow>();
                        arrow.SetNote(note);
                        Arrows.Add(arrow);
                        Notes.Remove(note);
                    }
                }

                foreach(Lane lane in Lanes)
                {
                    foreach(Panel child in lane.Children)
                    {
                        if(child is Arrow arrow)
                        {
                            float noteTime = arrow.Note.BakedTime;
                            float percent = 100f * ((noteTime - CurrentTime) / ScreenTime);
                            arrow.Style.Top = Length.Percent(percent);
                            if(!arrow.Missed && CurrentTime > noteTime + NoteTimings.Error)
                            {
                                player.ResetCombo();
                                arrow.Missed = true;
                            }
                            if(percent <= -50f)
                            {
                                Arrows.Remove(arrow);
                                arrow.Delete();
                            }
                        }
                    }
                }

                TimeSpan time = TimeSpan.FromSeconds(CurrentTime);
                SongTime.Text = string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
                ProgressBar.Style.Width = Length.Percent((CurrentTime / SongLength) * 100f);
            }

            string scoreText = string.Format("{0:D8}", player.Score);
            ScoreBig.Text = scoreText.Substring(0, 4);
            ScoreSmall.Text = scoreText.Substring(4, 4);
            MaxComboLabel.Text = player.MaxCombo.ToString();
            ComboContainer.SetClass("hide", player.Combo < 10);
            ComboLabel.Text = player.Combo.ToString();
        }
    }

    public async void StartSong(Chart chart)
    {
        SetClass("hide", false);
        SetChart(chart);

        await GameTask.DelayRealtimeSeconds(2);

        StartChart();

        await GameTask.DelayRealtimeSeconds(SongLength + 3f);

        FinishedSong();
    }

    public void FinishedSong()
    {
        Active = false;
        Lobby.SetFinished(Local.PlayerId.ToString());
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
        SongLength = Chart.GetSongLength();
        CritValue = (int)MathF.Floor(10000000f/Chart.Notes.Count);

        foreach(Note note in Chart.Notes)
        {
            Notes.Add(note);
        }

        foreach(BpmChange bpmchange in Chart.BpmChanges)
        {
            BpmChanges.Add(bpmchange);
        }

        Log.Info(CritValue);
    }

    public void StartChart()
    {
        CurrentSound = Sound.FromScreen(Song.Sound);
        CurrentTime = Song.Offset;
        Active = true;
    }

    public void EndChart()
    {
        CurrentSound.Stop();
        Active = false;
    }

    public List<Arrow> GetArrowsToHit()
    {
        float[] noteTimes = {1000, 1000, 1000, 1000};
        Arrow[] distantArrows = {null, null, null, null};
        List<Arrow> arrows = new();
        foreach(Arrow arrow in Arrows)
        {
            float noteTime = arrow.Note.BakedTime;
            float time = CurrentTime;
            float distance = MathF.Abs(time - noteTime);
            if(distance < NoteTimings.Error && noteTime < noteTimes[arrow.Note.Lane])
            {
                if(distantArrows[arrow.Note.Lane] != null) arrows.Remove(distantArrows[arrow.Note.Lane]);
                distantArrows[arrow.Note.Lane] = arrow;
                noteTimes[arrow.Note.Lane] = distance;
                arrow.Points = (int)MathF.Floor((float)CritValue / (distance > NoteTimings.Critical ? 2f : 1f));
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
            if(CurrentTime >= note.BakedTime - ScreenTime)
            {
                notes.Add(note);
            }
        }
        return notes;
    }

    public void ClearArrows()
    {
        foreach(Lane lane in Lanes)
        {
            foreach(Panel child in lane.Children)
            {
                if(child is Arrow arrow)
                {
                    arrow.Delete();
                }
            }
        }
    }

    public void StopMusic()
    {
        CurrentSound.Stop();
        Chart = null;
        Active = false;
    }

    public void Show(bool visible)
    {
        SetClass("hide", !visible);
        if(!visible)
        {
            // Clear pre-existing arrows
            ClearArrows();
        } 
    }

}