using Godot;
using System;
using Engine;

namespace FileIO5D {
	public class StringUtils
	{
		/// <summary>
		/// Returns the corresponding file from the int file sent, 0 indexed so a is 0 b is 1 and so on.
		/// </summary>
		/// <param name="file">File to get char for.</param>
		/// <returns>Char corresponding to sent file.</returns>
		public static char IntToFile(int file)
		{
			return (char)(file + 97);
		}

		/// <summary>
		/// Gets a string raw representation of this.
		/// </summary>
		/// <param name="c">Coord to make</param>
		/// <returns> Raw coord string.</returns>
		public static string RawCoordString(CoordFive c)
		{
			char colorch = c.Color ? 'w' : 'b';
			return $"({c.X},{c.Y},{c.T},{c.L},{colorch})";
		}
	}
}