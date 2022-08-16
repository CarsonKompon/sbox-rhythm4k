using Sandbox;
using Sandbox.UI;
using System;

[UseTemplate]
public partial class SongSelect : Panel
{
    public Panel RootBody {get;set;}
    public Panel Menu {get;set;}

    public SongSelect()
    {
        InitSongs();
    }

    public void InitSongs()
    {
        Menu.DeleteChildren();

        
    }
}