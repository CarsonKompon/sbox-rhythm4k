using Sandbox;
using Sandbox.UI;
using System;

[UseTemplate]
public partial class MainMenu : Panel
{
    public bool Visible = true;
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
        if(Visible)
        {
            Logo.Style.FilterHueRotate = -((RealTime.Now * 140f) % 360f);
        }
    }

    public void buttonSingleplayer(Button button)
    {
        Hud.Instance.ChangeMenuState((int)MainMenuState.SongSelect);
    }

    public void buttonMultiplayer(Button button)
    {
        Hud.Instance.ChangeMenuState((int)MainMenuState.SearchingForLobby);
    }

    public void buttonSettings(Button button)
    {
        Hud.Instance.ChangeMenuState((int)MainMenuState.Settings);
    }

    public void buttonHover(Button button)
    {

    }
}