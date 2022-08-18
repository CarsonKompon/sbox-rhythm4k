using Sandbox;
using Sandbox.UI;
using System;

[UseTemplate]
public partial class MainMenu : Panel
{
    public Panel RootBody {get;set;}
    public Image Logo {get;set;}

    public MainMenu()
    {
        if(RootBody == null) Log.Warning("NO ROOTBODY!");
        if(Logo != null) Logo.SetTexture("/sprites/logo.png");
        var q = new Package.Query();
        q.Type = Package.Type.Content;
    }

    [Event.Frame]
    public void OnFrame()
    {
        if(!HasClass("hide"))
        {
            Logo.Style.FilterHueRotate = -((RealTime.Now * 140f) % 360f);
        }
    }

    public void buttonSingleplayer(Button button)
    {
        // TODO: Fix this.
        RhythmGame.CreateLobby(Local.PlayerId.ToString(), Local.DisplayName + "'s Lobby", 1, true);
        Hud.Instance.ChangeMenuState(MainMenuState.SongSelect);
    }

    public void buttonMultiplayer(Button button)
    {
        Hud.Instance.ChangeMenuState(MainMenuState.SearchingForLobby);
    }

    public void buttonSettings(Button button)
    {
        Hud.Instance.ChangeMenuState(MainMenuState.Settings);
    }

    public void buttonHover(Button button)
    {

    }
}