using Sandbox;
using Sandbox.UI;

public enum MainMenuState {Title, SearchingForLobby, Lobby, SongSelect, Settings, Game, None}

public partial class Hud : HudEntity<RootPanel>
{
    public MainMenuState MenuState = MainMenuState.Title;
    public MenuBackground MenuBackground {get;set;}
    public MainMenu MainMenu {get;set;}
    public SongSelect SongSelect {get;set;}
    public LobbyBrowser LobbyBrowser {get;set;}
    public LobbyMenu LobbyMenu {get;set;}
    public SettingsMenu SettingsMenu {get;set;}
    public GameScreen GameScreen {get;set;}
    public static Hud Instance {get;set;}
    public Hud()
    {
        if(!IsClient) return;

        RootPanel.StyleSheet.Load("/ui/hud.scss");

        MenuBackground = RootPanel.AddChild<MenuBackground>();
        MainMenu = RootPanel.AddChild<MainMenu>();
        SongSelect = RootPanel.AddChild<SongSelect>();
        LobbyBrowser = RootPanel.AddChild<LobbyBrowser>();
        LobbyMenu = RootPanel.AddChild<LobbyMenu>();
        SettingsMenu = RootPanel.AddChild<SettingsMenu>();
        GameScreen = RootPanel.AddChild<GameScreen>();

        Instance = this;
    }

    public void ChangeMenuState(MainMenuState state)
    {
        switch(state)
        {
            case MainMenuState.Title:
                MainMenu.SetClass("hide", false);
                SongSelect.Show(false);
                LobbyBrowser.SetClass("hide", true);
                LobbyMenu.Show(false);
                SettingsMenu.SetClass("hide", true);
                MenuBackground.SetClass("hide", false);
                MenuBackground.SetHue(0);
                break;
            case MainMenuState.SongSelect:
                SongSelect.InitSongs();
                MainMenu.SetClass("hide", true);
                SongSelect.Show();
                LobbyBrowser.SetClass("hide", true);
                LobbyMenu.Show(false);
                MenuBackground.SetClass("hide", false);
                MenuBackground.SetHue(-10);
                break;
            case MainMenuState.SearchingForLobby:
                LobbyBrowser.SetClass("hide", false);
                MainMenu.SetClass("hide", true);
                LobbyMenu.Show(false);
                MenuBackground.SetClass("hide", false);
                MenuBackground.SetHue(-20);
                break;
            case MainMenuState.Lobby:
                LobbyMenu.Show();
                MainMenu.SetClass("hide", true);
                LobbyBrowser.SetClass("hide", true);
                SongSelect.Show(false);
                MenuBackground.SetClass("hide", false);
                MenuBackground.SetHue(70);
                break;
            case MainMenuState.Settings:
                MainMenu.SetClass("hide", true);
                SettingsMenu.SetClass("hide", false);
                MenuBackground.SetHue(10);
                break;
            case MainMenuState.Game:
            case MainMenuState.None:
                MainMenu.SetClass("hide", true);
                SongSelect.Show(false);
                LobbyMenu.Show(false);
                MenuBackground.SetClass("hide", true);
                break;
        }
        MenuState = state;
        GameScreen.Show(MenuState == MainMenuState.Game);
    }

    public void SetLobby(RhythmLobby lobby)
    {
        SongSelect.LobbyIdent = lobby.NetworkIdent;
        LobbyMenu.SetLobby(lobby);
        if(lobby.MaxPlayerCount == 1)
        {
            ChangeMenuState(MainMenuState.SongSelect);
        }
        else
        {
            ChangeMenuState(MainMenuState.Lobby);
        }
    }
}