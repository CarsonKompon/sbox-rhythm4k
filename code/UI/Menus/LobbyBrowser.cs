using Sandbox;
using Sandbox.UI;

[UseTemplate]
public partial class LobbyBrowser : Panel
{
    public string LobbyName {get;set;} = "";
    public TextEntry LobbyNameEntry {get;set;}
    public int MaxPlayerCount {get;set;} = 8;
    public Panel LobbyList {get;set;}
    public LobbyBrowser()
    {
        SetClass("hide", true);
        LobbyNameEntry.Placeholder = $"{Local.DisplayName}'s Lobby";
    }

    public void buttonCreateLobby()
    {
        string lobbyName = LobbyName == "" ? LobbyNameEntry.Placeholder : LobbyName;
        RhythmGame.CreateLobby(Local.PlayerId.ToString(), lobbyName, MaxPlayerCount);
        Hud.Instance.ChangeMenuState(MainMenuState.Lobby);
    }

    public void buttonBack()
    {
        Hud.Instance.ChangeMenuState(MainMenuState.Title);
    }

    // [Event.Frame]
    // public void OnFrame()
    // {
        
    // }
}

public partial class LobbyButton : Panel
{
    public Panel Background;
    public Label Name;
    public Label PlayerCount;

    public LobbyButton()
    {
        Name = AddChild<Label>();
        PlayerCount = AddChild<Label>();

        Name.AddClass("name");
        PlayerCount.AddClass("count");

        Name.Text = "Carson's Lobby";
        PlayerCount.Text = "1/8";
    }

	protected override void OnClick( MousePanelEvent e )
	{
        Log.Info("nfbnfsunfdsi");
	}
}