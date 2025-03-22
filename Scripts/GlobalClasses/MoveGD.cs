using Godot;
using System;

[GlobalClass]
public partial class MoveGD : Resource
{
	[Export]
	public Vector4 Origin;
	
	[Export]
	public Vector4 Dest;
}
