using System;
using System.Collections.Generic;

namespace FiveDChess
{
	public class CoordFive : IEquatable<CoordFive>
	{
		/*
		 * X represents file ie. a,b,c... files y represents rank T/L represent their
		 * raw coordinates as per 5d chess rules
         * Color represents white or black board.
		 */
		public int X;
		public int Y;
		public int T;
		public int L;
		public bool Color;
		
		public CoordFive(int x, int y, int t, int l, bool c)
		{
			X = x;
			Y = y;
			T = t;
			L = l;
			Color = c;
		}
		
		public CoordFive(int x, int y, int t, int l) : this(x,y,t,l,true) { }
		
		public CoordFive(int x, int y) : this(x, y, 0, 0) { }

		public CoordFive() : this(0,0,0,0,true) {}

		public CoordFive( CoordFive c , bool color) : this(c.X,c.Y,c.T,c.L,color) { }

		public CoordFive Clone()
		{
			return new CoordFive(X, Y, T, L, Color);
		}

		/// <summary>
		/// A comparison function to compare this coordinate and another.
		/// </summary>
		/// <param name="c">Coordinate to compare to.</param>
		/// <returns>True if the two coordinates are the same.</returns>
		public bool Equals(CoordFive c)
		{
			return X == c.X && Y == c.Y && T == c.T && L == c.L;
		}

		public override bool Equals(object o)
		{
			if(o == null){
				return false;
			}
			CoordFive c = o as CoordFive;
			return X == c.X && Y == c.Y && T == c.T && L == c.L;
		}

		/// <summary>
		/// A comparison function to compare this coordinate and another.
		/// This only checks spatially.
		/// </summary>
		/// <param name="c">Coordinate to compare to.</param>
		/// <returns>True if the two coordinates are the same.</returns>
		public bool SpatialEquals(CoordFive c)
		{
			return X == c.X && Y == c.Y;
		}

        /// <summary>
        /// Does an Equals, including color.
        /// </summary>
		/// <param name="c">Coordinate to compare to.</param>
		/// <returns>True if the two coordinates are the same includes color.</returns>
		public bool EqualsFull(CoordFive c)
		{
			return X == c.X && Y == c.Y && T == c.T && L == c.L && Color == c.Color;
		}

		/// <summary>
		/// Test if the move is only a spatial move.
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
		public void Add(CoordFive c)
		{
			X += c.X;
			Y += c.Y;
			T += c.T;
			L += c.L;
		}

        /// <summary>
        /// Function takes a coordfive and subtracts it from the current coordfive
        /// </summary>
        /// <param name="c">coord5 to subtract</param>
		public void Sub(CoordFive c)
		{
			X -= c.X;
			Y -= c.Y;
			T -= c.T;
			L -= c.L;
		}

        /// <summary>
        /// Flattens the current Coord. For example,  (2,2,4,4) = (1,1,2,2)
        /// </summary>
		public void Flatten()
		{
			int gcd = GCD(GCD(Math.Abs(X), Math.Abs(Y)), GCD(Math.Abs(T), Math.Abs(L)));
			if (gcd == 0) return;
			X /= gcd;
			Y /= gcd;
			T /= gcd;
			L /= gcd;
		}

		/// <summary>
        /// Gets Nagonal of the CoordFive. rook for example moves on Nagonal of 1
        /// </summary>
        /// <returns>Nagonal 0-4</returns>
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
			char colorch = Color ? 'w' : 'b';
			return $"({colorch}.{L}L.T{T}.{IntToFile(X)}{Y + 1})";
		}

        /// <summary>
        /// Greatest common denominator. From Stackoverflow.
        /// </summary>
        /// <param name="num1"></param>
        /// <param name="num2"></param>
        /// <returns>GCD of num 1 and num 2</returns>
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

		/// <summary>
		/// Subtracts the coords. Keeps the color of the first argument.
		/// </summary>
		/// <param name="c1">Coord to subtract</param>
		/// <param name="c2">Coord Subtracting from the first coord</param>
		/// <returns>Subratction of c1-c2 for all components.</returns>
		public static CoordFive Sub(CoordFive c1, CoordFive c2)
		{
			return new CoordFive(c1.X - c2.X, c1.Y - c2.Y, c1.T - c2.T, c1.L - c2.L,c1.Color);
		}

		/// <summary>
		/// Add Coords. Keeps the color of the first argument.
		/// </summary>
		/// <param name="c1">c1 to sum</param>
		/// <param name="c2">c2 to sum</param>
		/// <returns>Coord Sum with all the components added</returns>
		public static CoordFive Add(CoordFive c1, CoordFive c2)
		{
			return new CoordFive(c2.X + c1.X, c2.Y + c1.Y, c2.T + c1.T, c2.L + c1.L,c1.Color);
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
