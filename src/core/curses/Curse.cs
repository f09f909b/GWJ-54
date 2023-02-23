using Godot;
using System;

public partial class Curse : Node
{
	[Export] public CurseResource CurseData;
	
	public void PlaceOnPlayer()
	{
		//var limbNode = GetNode();
		//var targetLimb = limbNode.RemoveChild();
	}

	protected virtual void OnEquipped()
	{
		GD.Print("sometihn");
	}
}
