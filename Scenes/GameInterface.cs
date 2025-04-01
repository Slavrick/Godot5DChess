using Godot;
using System;
using FiveDChess;

public partial class GameInterface : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public static MoveGD MovetoGD(Move m)
	{
		var newMove = new MoveGD();
		newMove.Origin = CoordFivetoVector(m.Origin);
		newMove.Dest = CoordFivetoVector(m.Dest);
		return newMove;
	}
	
	
	public static Vector4 CoordFivetoVector(CoordFive coord)
	{
		// Create a Vector4 from your custom C# object
		Vector4 godotVector = new Vector4(coord.X, coord.Y, coord.L, coord.T);
		return godotVector;
	}
	
	public static Coord5 CoordFivetoCoord5(CoordFive coord)
	{
		Vector4 godotVector = new Vector4(coord.X, coord.Y, coord.L, coord.T);
		var coord5GD = new Coord5();
		coord5GD.v = godotVector;
		coord5GD.color = true;
		return coord5GD;
	}
	
	
	public static Coord5 CoordFivetoCoord5(CoordFive coord, bool color)
	{
		Vector4 godotVector = new Vector4(coord.X, coord.Y, coord.L, coord.T);
		var coord5GD = new Coord5();
		coord5GD.v = godotVector;
		coord5GD.color = color;
		return coord5GD;
	}
	
	public static Coord5 CoordFivetoGD(CoordFive CF)
	{
		Vector4 godotVector = new Vector4(CF.X, CF.Y, CF.L, CF.T);
		var coord5GD = new Coord5();
		coord5GD.v = godotVector;
		coord5GD.color = CF.Color;
		return coord5GD;
	}
	

}
