using System;
using Godot;

namespace Engine
{
	public class Board
	{
		// TODO possibly make this hold the position of pieces.
		// TODO Make this board a 1d array and index that way, in order to have all the board contiguous and possibly faster
		// Make this board private, so incorrect indexing doesn't happen
		public int[][] Brd { get; set; }
		public int Height { get; set; }
		public int Width { get; set; }
		// The board has no need for its location within the multiverse
		//TODO possibly make this a 2d coordinate instead.
		public CoordFour EnPassentSquare { get; set; }

		public static readonly int numTypes = 12;
		public static readonly int ERRORSQUARE = -63;
		public static readonly int EMPTYSQUARE = 0;

		public enum Piece
		{
			EMPTY, WPAWN, WKNIGHT, WBISHOP, WROOK, WPRINCESS, WQUEEN, WKING, WUNICORN, WDRAGON, WBRAWN, WROYALQUEEN, WCOMMONKING, BPAWN, BKNIGHT,
			BBISHOP, BROOK, BPRINCESS, BQUEEN, BKING, BUNICORN, BDRAGON, BBRAWN, BROYALQUEEN, BCOMMONKING
		}

		public static readonly char[] PieceChars = {
			'_', 'P', 'N', 'B', 'R', 'S', 'Q', 'K', 'U', 'D', 'W', 'Y', 'C', 'p', 'n', 'b', 'r', 's', 'q', 'k', 'u', 'd', 'w', 'y', 'c'
		};

		public Board(Board b)
		{
			Brd = new int[b.Width][];
			for (int i = 0; i < b.Width; i++)
			{
				Brd[i] = (int[])b.Brd[i].Clone();
			}
			Height = b.Height;
			Width = b.Width;
		}

		public Board(int height, int width)
		{
			Brd = new int[width][];
			for (int i = 0; i < width; i++)
			{
				Brd[i] = new int[height];
			}
			Height = height;
			Width = width;
		}

		/// <summary>
		/// Gets a piece code at the coordinate specified. Does bounds checking.
		/// </summary>
		/// <param name="x">The 'file' to get. Starts at 0, so a=0, h=7.</param>
		/// <param name="y">The 'rank' to get. Starts at 0, so 0 would be the first rank.</param>
		/// <returns>An integer piece defined in the enum above, or ErrorSquare if the coordinate is out of bounds.</returns>
		public int getSquare(int x, int y)
		{
			if (IsInBounds(x, y))
				return Brd[y][x];
			return ERRORSQUARE;
		}

		/// <summary>
		/// Gets the code of the square location. Does bounds checking on the coordinate generated.
		/// </summary>
		/// <param name="c">Coordinate to get piece code.</param>
		/// <returns>An integer piece defined in the enum above, or ErrorSquare if the coordinate is out of bounds.</returns>
		public int getSquare(CoordFour c)
		{
			if (IsInBounds(c))
				return Brd[c.Y][c.X];
			return ERRORSQUARE;
		}

		public void setSquare(CoordFour c, int piece)
		{
			Brd[c.Y][c.X] = piece;
		}

		public void setSquare(int x, int y, int piece)
		{
			Brd[y][x] = piece;
		}

		/// <summary>
		/// Determine whether a coordinate is in bounds.
		/// </summary>
		/// <param name="x">The 'file' to check. Starts at 0, so a=0, h=7.</param>
		/// <param name="y">The 'rank' to check. Starts at 0, so 0 would be the first rank.</param>
		/// <returns>Boolean indicating whether the x,y was in bounds.</returns>
		public bool IsInBounds(int x, int y)
		{
			if (x < 0 || x >= Width)
				return false;
			if (y < 0 || y >= Height)
				return false;
			return true;
		}

		/// <summary>
		/// Determines whether a coordinate is in bounds, according to spatial dimensions.
		/// </summary>
		/// <param name="cf">Coordinate to test.</param>
		/// <returns>Boolean indicating whether the coordinate is in bounds spatially.</returns>
		public bool IsInBounds(CoordFour cf)
		{
			return IsInBounds(cf.X, cf.Y);
		}

		/// <summary>
		/// Gets a string representation of the board. This string starts with the 1st rank, and each rank is a line of text.
		/// </summary>
		public override string ToString()
		{
			string temp = "";
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					int piece = Brd[x][y];
					piece = piece < 0 ? piece * -1 : piece;
					temp += PieceChars[piece];
				}
				temp += "\n";
			}
			return temp;
		}

		/// <summary>
		/// Gets the color of the piece code that was sent (as defined above in the piece enum above).
		/// </summary>
		/// <param name="pieceCode">Integer that matches the enum.</param>
		/// <returns>True for white, false for black and false for empty.</returns>
		public static bool GetColorBool(int pieceCode)
		{
			pieceCode = pieceCode < 0 ? pieceCode * -1 : pieceCode;
			if (pieceCode == (int)Piece.EMPTY)
				return false;
			if (pieceCode >= (int)Piece.BPAWN)
				return false;//GameState.BLACK;
			return true;//GameState.WHITE;
		}

		/// <summary>
		/// Gets ordinal value from char. Example: N = 2.
		/// Error square is returned if not found.
		/// </summary>
		/// <param name="target">Character to find.</param>
		/// <returns>Ordinal value or ErrorSquare if not found.</returns>
		public static int PieceCharToInt(char target)
		{
			for (int i = 0; i < PieceChars.Length; i++)
			{
				if (PieceChars[i] == target)
					return i;
			}
			return ERRORSQUARE;
		}
	}
}
