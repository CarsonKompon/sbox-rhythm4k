using Sandbox;
using System;
using System.Linq;

namespace Sandbox;

partial class RhythmPlayer : AnimatedEntity
{
	[Net] public int LobbyIdent {get;set;} = -1;

	[Net, Predicted] public int Score {get;set;} = 0;
	[Net, Predicted] public int MaxCombo {get;set;} = 0;
	public int Combo = 0;
	/// <summary>
	/// Called when the entity is first created 
	/// </summary>
	public override void Spawn()
	{
		base.Spawn();

		//
		// Use a watermelon model
		//
		// SetModel( "models/sbox_props/watermelon/watermelon.vmdl" );

		// EnableDrawing = false;
		// EnableHideInFirstPerson = true;
		// EnableShadowInFirstPerson = true;
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		if(Input.Pressed(InputButton.Left))
		{
			
		}
	}

}
