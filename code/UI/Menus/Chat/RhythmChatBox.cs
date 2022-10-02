using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

public partial class RhythmChatBox : ChatBox
{
	public static RhythmChatBox Instance;
	public RhythmChatBox()
	{
		StyleSheet.Load( "/ui/menus/chat/rhythmchatbox.scss" );

		Canvas.PreferScrollToBottom = true;
		Input.Placeholder = "Press Enter to chat";

		// Canvas = Add.Panel( "chat_canvas" );

		// Input = Add.TextEntry( "" );
		// Input.AddEventListener( "onsubmit", () => Submit() );
		// Input.AddEventListener( "onblur", () => Close() );
		// Input.AcceptsFocus = true;
		// Input.AllowEmojiReplace = true;

		// Sandbox.Hooks.Chat.OnOpenChat += Open;
		Instance = this;
	}

	void Open()
	{
		AddClass( "open" );
		Input.Focus();
	}

	void Close()
	{
		RemoveClass( "open" );
		Input.Blur();
	}

	void Submit()
	{
		Close();

		var msg = Input.Text.Trim();
		Input.Text = "";

		if ( string.IsNullOrWhiteSpace( msg ) )
			return;

		Say( msg );
	}

	public void AddEntry( string name, string message, string avatar, string lobbyState = null )
	{
		var e = Canvas.AddChild<RhythmChatEntry>();

		e.Message.Text = message;
		e.NameLabel.Text = name;
		e.Avatar.SetTexture( avatar );

		e.SetClass( "noname", string.IsNullOrEmpty( name ) );
		e.SetClass( "noavatar", string.IsNullOrEmpty( avatar ) );

		if ( lobbyState == "ready" || lobbyState == "staging" )
		{
			e.SetClass( "is-lobby", true );
		}
	}


	[ConCmd.Client( "chat_add", CanBeCalledFromServer = true )]
	public static void AddChatEntry( string name, string message, string avatar = null, string lobbyState = null )
	{
		Instance?.AddEntry( name, message, avatar, lobbyState );

		// Only log clientside if we're not the listen server host
		if ( !Global.IsListenServer )
		{
			Log.Info( $"{name}: {message}" );
		}
	}

	[ConCmd.Client( "chat_addinfo", CanBeCalledFromServer = true )]
	public static void AddInformation( string message, string avatar = null )
	{
		// Instance?.AddEntry( null, message, avatar );
	}

	[ConCmd.Server( "say" )]
	public static void Say( string message )
	{
		Assert.NotNull( ConsoleSystem.Caller );
		
		if ( message.Contains( '\n' ) || message.Contains( '\r' ) )
			return;

		Log.Info( $"{ConsoleSystem.Caller}: {message}" );
		AddChatEntry( To.Everyone, ConsoleSystem.Caller.Name, message, $"avatar:{ConsoleSystem.Caller.PlayerId}" );
	}
}