using Sandbox;
using Sandbox.UI;

public enum MainMenuState {Title, SearchingForLobby, Lobby, SongSelect, Settings, Game, None}

public partial class Hud : HudEntity<RootPanel>
{
    public MainMenuState MenuState = MainMenuState.Title;
    public MenuBackground MenuBackground {get;set;}
    public MainMenu MainMenu {get;set;}
    public SongSelect SongSelect {get;set;}
    public GameScreen GameScreen {get;set;}
    public static Hud Instance {get;set;}
    public Hud()
    {
        if(!IsClient) return;

        RootPanel.StyleSheet.Load("/ui/hud.scss");

        MenuBackground = RootPanel.AddChild<MenuBackground>();
        MainMenu = RootPanel.AddChild<MainMenu>();
        SongSelect = RootPanel.AddChild<SongSelect>();
        GameScreen = RootPanel.AddChild<GameScreen>();

        Instance = this;
    }

    public void ChangeMenuState(MainMenuState state)
    {
        MenuState = state;
        switch(MenuState)
        {
            case MainMenuState.Title:
                MainMenu.SetClass("hide", false);
                SongSelect.SetClass("hide", true);
                MenuBackground.SetClass("hide", false);
                MenuBackground.SetHue(0);
                break;
            case MainMenuState.SongSelect:
                SongSelect.InitSongs();
                MainMenu.SetClass("hide", true);
                SongSelect.SetClass("hide", false);
                SongSelect.Deselect();
                MenuBackground.SetClass("hide", false);
                MenuBackground.SetHue(-10);
                break;
            case MainMenuState.SearchingForLobby:
                MenuBackground.SetClass("hide", false);
                MenuBackground.SetHue(-20);
                break;
            case MainMenuState.Game:
            case MainMenuState.None:
                MainMenu.SetClass("hide", true);
                SongSelect.SetClass("hide", true);
                MenuBackground.SetClass("hide", true);
                break;
        }
        GameScreen.Show(MenuState == MainMenuState.Game);
    }

    public void SetLobby(int lobbyIdent)
    {
        SongSelect.LobbyIdent = lobbyIdent;
    }
}