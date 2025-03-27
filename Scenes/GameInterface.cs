using Godot;
using System;
using Engine;

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
		newMove.Origin = CoordFourtoVector(m.Origin);
		newMove.Dest = CoordFourtoVector(m.Dest);
		return newMove;
	}
	
	
	public static Vector4 CoordFourtoVector(CoordFour coord)
	{
		// Create a Vector4 from your custom C# object
		Vector4 godotVector = new Vector4(coord.X, coord.Y, coord.L, coord.T);
		return godotVector;
	}
	
	public static Coord5 CoordFourtoCoord5(CoordFour coord)
	{
		Vector4 godotVector = new Vector4(coord.X, coord.Y, coord.L, coord.T);
		var coord5GD = new Coord5(godotVector,true);
		return coord5GD;
	}
	
	
	public static Coord5 CoordFourtoCoord5(CoordFour coord, bool color)
	{
		Vector4 godotVector = new Vector4(coord.X, coord.Y, coord.L, coord.T);
		var coord5GD = new Coord5(godotVector,color);
		return coord5GD;
	}
	
	public static Coord5 CoordFivetoGD(CoordFive CF)
	{
		Vector4 godotVector = new Vector4(CF.X, CF.Y, CF.L, CF.T);
		var coord5GD = new Coord5(godotVector,CF.Color);
		return coord5GD;
	}
	
	public static CoordFive Coord5toFive(Coord5 c)
	{
		CoordFive new_cf = new CoordFive((int)c.v[0],(int)c.v[1],(int)c.v[3],(int)c.v[2],c.color);
		return new_cf;
	}
}
