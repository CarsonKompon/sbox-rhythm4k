using Sandbox;
using Sandbox.UI;

public enum MainMenuState {Title, SearchingForLobby, Lobby, SongSelect, Settings, None}

public partial class Hud : HudEntity<RootPanel>
{
    public MainMenuState MenuState = MainMenuState.Title;
    public MenuBackground MenuBackground {get;set;}
    public MainMenu MainMenu {get;set;}
    public static Hud Instance {get;set;}
    public Hud()
    {
        if(!IsClient) return;

        RootPanel.StyleSheet.Load("/ui/hud.scss");

        MenuBackground = RootPanel.AddChild<MenuBackground>();
        MainMenu = RootPanel.AddChild<MainMenu>();

        Instance = this;
    }

    [ClientRpc]
    public void ChangeMenuState(int stateInt)
    {
        MenuState = (MainMenuState)stateInt;
        switch(MenuState)
        {
            case MainMenuState.Title:
                MenuBackground.SetHue(0);
                MainMenu.Visible = true;
                break;
            case MainMenuState.SongSelect:
                MenuBackground.SetHue(200);
                break;
            case MainMenuState.SearchingForLobby:
                MenuBackground.SetHue(100);
                break;
        }
    }
}