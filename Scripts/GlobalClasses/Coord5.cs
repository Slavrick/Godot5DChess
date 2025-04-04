using Godot;
using System;

[GlobalClass]
public partial class Coord5 : Resource
{
	[Export]
	public Vector4 v;
	
	[Export]
	public bool color;

	public Coord5(Vector4 vector, bool coord_color)
	{
		this.v = vector;
		this.color = coord_color;
	}

	public static Coord5 Create(Vector4 vector, bool coord_color)
	{
		return new Coord5(vector,coord_color);
	}
	
	public bool Equals(Coord5 compare)
	{
		return compare.v[0] == v[0] 
			&& compare.v[1] == v[1] 
			&& compare.v[2] == v[2] 
			&& compare.v[3] == v[3]
			&& compare.color == this.color;
	}
}
