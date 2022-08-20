using Sandbox;
using Sandbox.UI;

[UseTemplate]
public partial class SettingsMenu : Panel
{
    private Switch Downscroll {get;set;}
    public float ScrollSpeed {get;set;}
    public SettingsData Settings;
    public SettingsMenu()
    {
        SetClass("hide", true);

        Load();
    }

    public void Load()
    {
        Settings = FileSystem.Data.ReadJsonOrDefault<SettingsData>("settings.json", new SettingsData());
        Downscroll.Checked = Settings.Downscroll;
        ScrollSpeed = Settings.ScrollSpeed;
    }

    public void Save()
    {
        Settings.Downscroll = Downscroll.Checked;
        Settings.ScrollSpeed = ScrollSpeed;
        FileSystem.Data.WriteJson("settings.json", Settings);
    }

    public void buttonBack()
    {
        Save();
        Hud.Instance.ChangeMenuState(MainMenuState.Title);
    }

    // [Event.Frame]
    // public void OnFrame()
    // {
        
    // }
}

public class SettingsData
{
    public bool Downscroll {get;set;} = false;
    public float ScrollSpeed {get;set;} = 40f;
}