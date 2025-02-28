using System;
using Godot;

namespace Engine
{
	public class CoordFive : CoordFour
	{
		public bool Color { get; set; }

		public CoordFive(int x, int y, bool color) : base(x, y)
		{
			Color = color;
		}

		public CoordFive(CoordFour c, bool color) : base(c.X, c.Y, c.T, c.L)
		{
			Color = color;
		}

		public CoordFive(int x, int y, int t, int l, bool color) : base(x, y, t, l)
		{
			Color = color;
		}

		public new CoordFive clone()
		{
			return new CoordFive(base.Clone(), Color);
		}

		public bool Equals(CoordFive compare)
		{
			return compare.X == X && compare.Y == Y && compare.T == T && compare.L == L && compare.Color == Color;
		}

		public override string ToString()
		{
			char colorch = Color ? 'w' : 'b';
			return $"({colorch}.{L}L.T{T}.{IntToFile(X)}{Y + 1})";
		}
	}
}
