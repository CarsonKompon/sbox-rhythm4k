using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;

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
    public TimeSince CurrentTime = 0f;
    public float LastTime = 0f;
    public List<Note> Notes = new();
    public List<Note> LivingNotes = new();
    public List<Arrow> Arrows = new();
    public List<Trail> Trails = new();
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
                        LivingNotes.Add(note);
                        Notes.Remove(note);
                        if(note.Type == (int)NoteType.Hold) continue;
                        Lane lane = Lanes[note.Lane];
                        Arrow arrow = lane.AddChild<Arrow>();
                        arrow.SetNote(note);
                        Arrows.Add(arrow);

                        if(note.Length > 0f)
                        {
                            Trail trail = lane.AddChild<Trail>();
                            trail.SetNote(note);
                            Trails.Add(trail);
                        }
                        note.Arrow = arrow;
                    }
                }
                else if(Arrows.Count == 0 && Active)
                {
                    FinishedSong();
                }

                foreach(Note note in LivingNotes.ToList())
                {
                    if(note.BakedTime <= CurrentTime - NoteTimings.Error)
                    {
                        LivingNotes.Remove(note);
                    }
                }

                foreach(Lane lane in Lanes)
                {
                    foreach(Panel child in lane.Children)
                    {
                        // Loop through all Arrows
                        if(child is Arrow arrow)
                        {
                            float noteTime = arrow.Note.BakedTime;
                            float percent = 100f * ((noteTime - CurrentTime) / ScreenTime);
                            if(Hud.Instance.SettingsMenu.Settings.Downscroll)
                            {
                                arrow.Style.Top = Length.Percent(100f-percent);
                            }
                            else
                            {
                                arrow.Style.Top = Length.Percent(percent);
                            }
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

                        // Loop through all trails
                        if(child is Trail trail)
                        {
                            float positionPercent = 100f * ((trail.Note.BakedTime - CurrentTime) / ScreenTime);
                            float lengthPercent = 100f * (trail.Note.BakedLength / ScreenTime);
                            if(Hud.Instance.SettingsMenu.Settings.Downscroll)
                            {
                                float downscrollPos = 100f-positionPercent-lengthPercent+6f;
                                if(downscrollPos > 0f && positionPercent < -2.5f)
                                {
                                    lengthPercent += positionPercent + 2.5f;
                                    positionPercent = -2.5f;
                                    downscrollPos = 100f-positionPercent-lengthPercent+6f;
                                }
                                if(downscrollPos <= 0f && lengthPercent >= 100f)
                                {
                                    Log.Info("ayo");
                                    trail.Style.Top = Length.Percent(0f);
                                    trail.Style.Height = Length.Percent(100f-positionPercent+6f);
                                }
                                else
                                {
                                    trail.Style.Top = Length.Percent(downscrollPos);
                                    trail.Style.Height = Length.Percent(lengthPercent);
                                }
                            }
                            else
                            {
                                if(positionPercent < 0f)
                                {
                                    lengthPercent += positionPercent;
                                    positionPercent = 0f;
                                }
                                trail.Style.Top = Length.Percent(positionPercent);
                                trail.Style.Height = Length.Percent(lengthPercent);
                            }
                            if(positionPercent + lengthPercent <= 0f) trail.Delete();
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

        LastTime = CurrentTime;
    }

    public async void StartSong(Chart chart)
    {
        SetClass("hide", false);
        SetChart(chart);

        await GameTask.DelayRealtimeSeconds(2);

        StartChart();
    }

    public async void FinishedSong()
    {
        Active = false;

        await GameTask.DelayRealtimeSeconds(3);

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
        CritValue = (int)MathF.Floor(10000000f/Chart.TotalChain);

        foreach(Note note in Chart.Notes)
        {
            Notes.Add(note);
        }

        foreach(BpmChange bpmchange in Chart.BpmChanges)
        {
            BpmChanges.Add(bpmchange);
        }
    }

    public void StartChart()
    {
        CurrentSound = Sound.FromScreen(Song.Sound);
        CurrentTime = Song.Offset;
        LastTime = CurrentTime;
        // CurrentSteps = SecondsToSteps(CurrentTime);
        Active = true;
    }

    public void EndChart()
    {
        CurrentSound.Stop();
        Active = false;
    }

    public List<Note> GetNotesToHit()
    {
        float[] noteTimes = {1000, 1000, 1000, 1000};
        List<Note> notes = new();
        foreach(Note note in LivingNotes)
        {
            float noteTime = note.BakedTime;
            float time = CurrentTime;
            float distance = MathF.Abs(time - noteTime);
            float timing = NoteTimings.Error;
            if((NoteType)note.Type == NoteType.Hold) timing = NoteTimings.Critical;
            if(distance < timing && noteTime < noteTimes[note.Lane])
            {
                noteTimes[note.Lane] = distance;
                if((NoteType)note.Type == NoteType.Hold)
                {
                    note.Points = CritValue;
                }
                else
                {
                    note.Points = (int)MathF.Floor((float)CritValue / (distance > NoteTimings.Critical ? 2f : 1f));
                }
                notes.Add(note);
            }
        }

        return notes;
    }

    // public List<Trail> GetTrailsToHit()
    // {
    //     List<Trail> trails = new();
    //     foreach(Trail trail in Trails)
    //     {
    //         bool hit = false;
    //         while(CurrentTime >= trail.lastHit)
    //         {
    //             if(trail.lastHit > trail.Note.Offset) hit = true;
    //             trail.lastHit += (60f / CurrentBPM) / 4f;
    //         }
    //         if(hit && CurrentTime < trail.Note.BakedTime + trail.Note.BakedLength)
    //         {
    //             trails.Add(trail);
    //         }
    //     }
    //     return trails;
    // }

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
                if(child is Arrow || child is Trail)
                {
                    child.Delete();
                }
            }
        }
        Arrows.Clear();
        Trails.Clear();
    }

    public void StopMusic()
    {
        CurrentSound.Stop();
        Chart = null;
        Active = false;
    }

    private float SecondsToSteps(float seconds)
    {
        return MathC.Map(seconds, 0, 4f * (60f / CurrentBPM), 0, 1000f);
    }

    public void Show(bool visible)
    {
        SetClass("hide", !visible);
        if(visible)
        {
            ScreenTime = MathC.Map(Hud.Instance.SettingsMenu.Settings.ScrollSpeed, 0, 100, 2, 0.05f);
            if(Hud.Instance.SettingsMenu.Settings.Downscroll)
            {
                LaneContainer.Style.Top = Length.Pixels(-100f);
            }
            else
            {
                LaneContainer.Style.Top = Length.Pixels(100f);
            }
            foreach(Lane lane in Lanes)
            {
                lane.Receptor.Delete();
                lane.Receptor = lane.AddChild<Receptor>();
                lane.Receptor.SetLane(lane.LaneIndex);
                if(Hud.Instance.SettingsMenu.Settings.Downscroll)
                {
                    lane.Style.Top = Length.Pixels(-171f);
                    lane.Receptor.Style.Bottom = Length.Pixels(-171f);
                }
                else
                {
                    lane.Style.Top = Length.Pixels(0f);
                }
            }
        }
        else
        {
            // Clear pre-existing arrows and make sure game is inactive
            ClearArrows();
            Active = false;
        } 
    }

}