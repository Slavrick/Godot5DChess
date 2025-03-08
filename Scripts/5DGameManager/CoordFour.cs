using System;
using Godot;
using System.Collections.Generic;

namespace Engine
{
	public class CoordFour : IEquatable<CoordFour>
	{
		/*
		 * X represents file ie. a,b,c... files y represents rank T/L represent their
		 * raw coordinates as per 5d chess rules
		 */
		public int X { get; set; }
		public int Y { get; set; }
		public int T { get; set; }
		public int L { get; set; }
		
		public CoordFour()
		{
			X = 0;
			Y = 0;
			T = 0;
			L = 0;
		}
		
		public CoordFour(int x, int y, int t, int l)
		{
			X = x;
			Y = y;
			T = t;
			L = l;
		}

		public CoordFour(int x, int y) : this(x, y, 0, 0) { }

		public CoordFour Clone()
		{
			return new CoordFour(X, Y, T, L);
		}

		/// <summary>
		/// A comparison function to compare this coordinate and another.
		/// </summary>
		/// <param name="c">Coordinate to compare to.</param>
		/// <returns>True if the two coordinates are the same.</returns>
		public bool Equals(CoordFour c)
		{
			return X == c.X && Y == c.Y && T == c.T && L == c.L;
		}


		public override bool Equals(object o)
		{
			if(o == null){
				return false;
			}
			CoordFour c = o as CoordFour;
			return X == c.X && Y == c.Y && T == c.T && L == c.L;
		}
		/// <summary>
		/// A comparison function to compare this coordinate and another.
		/// This only checks spatially.
		/// </summary>
		/// <param name="c">Coordinate to compare to.</param>
		/// <returns>True if the two coordinates are the same.</returns>
		public bool SpatialEquals(CoordFour c)
		{
			return X == c.X && Y == c.Y;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns>True if the coordinate is pure spatial, and false otherwise.</returns>
		public bool IsSpatial()
		{
			return T == 0 && L == 0;
		}

		/// <summary>
		/// This function takes a 5D coord and adds another coordinate's values to it.
		/// </summary>
		/// <param name="c">Five Dimensional Coordinate to add to this Coordinate.</param>
		public void Add(CoordFour c)
		{
			X += c.X;
			Y += c.Y;
			T += c.T;
			L += c.L;
		}

		public void Sub(CoordFour c)
		{
			X -= c.X;
			Y -= c.Y;
			T -= c.T;
			L -= c.L;
		}

		// Turns a coord into a vector, or a coord with only 1/0's
		public void MakeVector()
		{
			if (X != 0) X = 1;
			if (Y != 0) Y = 1;
			if (T != 0) T = 1;
			if (L != 0) L = 1;
		}

		public void Flatten()
		{
			int gcd = GCD(GCD(Math.Abs(X), Math.Abs(Y)), GCD(Math.Abs(T), Math.Abs(L)));
			if (gcd == 0) return;
			X /= gcd;
			Y /= gcd;
			T /= gcd;
			L /= gcd;
		}

		// Gets the n-diagonal that a vector is
		public int GetNagonal()
		{
			int nagonal = 0;
			if (X != 0) nagonal++;
			if (Y != 0) nagonal++;
			if (T != 0) nagonal++;
			if (L != 0) nagonal++;
			return nagonal;
		}

		public override string ToString()
		{
			return $"({L}L.T{T}.{IntToFile(X)}{Y + 1})";
		}

		/// <summary>
		/// Gets a string raw representation of this.
		/// </summary>
		/// <returns>Raw coord string.</returns>
		public string RawCoordString()
		{
			return $"({X},{Y},{T},{L})";
		}

		/// <summary>
		/// Get a SAN 2D coord of the given object such as a1 e4 ....
		/// </summary>
		/// <returns>String SAN representation.</returns>
		public string SANString()
		{
			return $"{IntToFile(X)}{Y + 1}";
		}

		public static int GCD(int num1, int num2)
		{
			while (num1 > 0 && num2 > 0)
			{
				if (num1 > num2)
				{
					num1 -= num2;
				}
				else
				{
					num2 -= num1;
				}
			}
			return num1 + num2;
		}

		// Sub
		public static CoordFour Sub(CoordFour c1, CoordFour c2)
		{
			return new CoordFour(c1.X - c2.X, c1.Y - c2.Y, c1.T - c2.T, c1.L - c2.L);
		}

		// Add
		public static CoordFour Add(CoordFour c1, CoordFour c2)
		{
			return new CoordFour(c2.X + c1.X, c2.Y + c1.Y, c2.T + c1.T, c2.L + c1.L);
		}

		/// <summary>
		/// Returns the corresponding file from the int file sent, 0 indexed so a is 0 b is 1 and so on.
		/// </summary>
		/// <param name="file">File to get char for.</param>
		/// <returns>Char corresponding to sent file.</returns>
		protected static char IntToFile(int file)
		{
			return (char)(file + 97);
		}
	}
}
