using Godot;
using System;
using FiveDChess;

public partial class GameInterface
{	
	public static MoveGD MovetoGD(Move m)
	{
		var newMove = new MoveGD();
		newMove.Origin = CoordFivetoVector(m.Origin);
		newMove.Dest = CoordFivetoVector(m.Dest);
		newMove.Color = m.Origin.Color;
		return newMove;
	}
	
	public static Vector4 CoordFivetoVector(CoordFive coord)
	{
		// Create a Vector4 from your custom C# object
		Vector4 godotVector = new Vector4(coord.X, coord.Y, coord.L, coord.T);
		return godotVector;
	}
	
	public static Coord5 CoordFivetoGD(CoordFive CF)
	{
		Vector4 godotVector = new Vector4(CF.X, CF.Y, CF.L, CF.T);
		var coord5GD = new Coord5(godotVector,CF.Color);
		return coord5GD;
	}
	
	public static CoordFive C5toCoordFive(Coord5 c)
	{
		CoordFive new_cf = new CoordFive((int)c.v[0],(int)c.v[1],(int)c.v[3],(int)c.v[2],c.color);
		return new_cf;
	}
}
