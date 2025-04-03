using System;
using FileIO5D;

namespace FiveDChess
{
	public class Board{
		/// <summary>
		/// Board Array of integers held in the position. Each piece has a value, going from 0-24. 0 is empty and -63 is ERROR.
		/// Negative values are unmoved pieces(kings, pawns). 1-12 is white 13-24 are black pieces.
		/// </summary>
		private int[] Brd;
		public int Height;
		public int Width;
		/// <summary>
		/// Square that holds the en passent. is not copied over on a clone.
		/// </summary>
		public CoordFive EnPassentSquare;

		public static readonly int NUMTYPES = 12;
		public static readonly int ERRORSQUARE = -63;
		public static readonly int EMPTYSQUARE = 0;

		/// <summary>
		/// Enums for the pieces.
		/// </summary>
		public enum Piece
		{
			EMPTY, WPAWN, WKNIGHT, WBISHOP, WROOK, WPRINCESS, WQUEEN, WKING, WUNICORN, WDRAGON, WBRAWN, WROYALQUEEN, WCOMMONKING, BPAWN, BKNIGHT,
			BBISHOP, BROOK, BPRINCESS, BQUEEN, BKING, BUNICORN, BDRAGON, BBRAWN, BROYALQUEEN, BCOMMONKING
		}

		/// <summary>
		/// Chars used to print the pieces and some other functions with translating characters to ints
		/// </summary>
		public static readonly char[] PieceChars = 
		{
			'_', 'P', 'N', 'B', 'R', 'S', 'Q', 'K', 'U', 'D', 'W', 'Y', 'C', 'p', 'n', 'b', 'r', 's', 'q', 'k', 'u', 'd', 'w', 'y', 'c'
		};

		/// <summary>
		/// Board constructor to clone a board
		/// </summary>
		/// <param name="b">board to clone</param>
		public Board(Board b)
		{
			Brd = new int[b.Width * b.Height];
			for(int i = 0; i < b.Width * b.Height; i++)
			{
				this.Brd[i] = b.Brd[i];
			}
			Height = b.Height;
			Width = b.Width;
		}

		/// <summary>
		/// Creates an empty board of size heigth and width
		/// </summary>
		/// <param name="height">board heigth</param>
		/// <param name="width">board width</param>
		public Board(int height, int width)
		{
			Brd = new int[width * height];
			Height = height;
			Width = width;
		}

		/// <summary>
		/// Gets a piece code at the coordinate specified.
		/// </summary>
		/// <param name="file">The 'file' to get. Starts at 0, so a=0, h=7.</param>
		/// <param name="rank">The 'rank' to get. Starts at 0, so 0 would be the first rank.</param>
		/// <returns>An integer piece defined in the enum above, or ErrorSquare if the coordinate is out of bounds.</returns>
		public int GetSquare(int file, int rank)
		{
			return Brd[rank * Width + file];
		}

		/// <summary>
		/// Gets the code of the square location.
		/// </summary>
		/// <param name="c">Coordinate to get piece code.</param>
		/// <returns>An integer piece defined in the enum above, or ErrorSquare if the coordinate is out of bounds.</returns>
		public int GetSquare(CoordFive c)
		{
			return Brd[c.Y * Width + c.X];
		}

		/// <summary>
		/// Gets the code of the square location. Does bounds checking on the coordinate generated.
		/// </summary>
		/// <param name="c">Coordinate to get piece code.</param>
		/// <returns>An integer piece defined in the enum above, or ErrorSquare if the coordinate is out of bounds.</returns>
		public int GetSquareSafe(CoordFive c)
		{
			if(IsInBounds(c))
			{
				return Brd[c.Y * Width + c.X];
			}
			return ERRORSQUARE;
		}

		/// <summary>
		/// Gets a piece code at the coordinate specified. Does bounds checking.
		/// </summary>
		/// <param name="file">The 'file' to get. Starts at 0, so a=0, h=7.</param>
		/// <param name="rank">The 'rank' to get. Starts at 0, so 0 would be the first rank.</param>
		/// <returns>An integer piece defined in the enum above, or ErrorSquare if the coordinate is out of bounds.</returns>
		public int GetSquareSafe(int file, int rank)
		{
			if(IsInBounds(file,rank))
			{
				return Brd[rank * Width + file];
			}
			return ERRORSQUARE;
		}

		/// <summary>
		/// Sets specified square to specified piece. Currently does not bounds checking.
		/// </summary>
		/// <param name="c">coord of piece to get</param>
		/// <param name="piece">piece to add to the board</param>
		public void SetSquare(CoordFive c, int piece)
		{
			Brd[c.Y * Width + c.X] = piece;
		}

		/// <summary>
		/// Sets specified square to specified piece. Currently does not bounds checking.
		/// </summary>
		/// <param name="file">file of piece</param>
		/// <param name="rank">rank of piece</param>
		/// <param name="piece">piece to add to the board</param>
		public void SetSquare(int file, int rank, int piece)
		{
			Brd[rank * Width + file] = piece;
		}

		/// <summary>
		/// Determine whether a coordinate is in bounds.
		/// </summary>
		/// <param name="file">The 'file' to check. Starts at 0, so a=0, h=7.</param>
		/// <param name="rank">The 'rank' to check. Starts at 0, so 0 would be the first rank.</param>
		/// <returns>Boolean indicating whether the x,y was in bounds.</returns>
		public bool IsInBounds(int file, int rank)
		{
			if (file < 0 || file >= Width)
				return false;
			if (rank < 0 || rank >= Height)
				return false;
			return true;
		}

		/// <summary>
		/// Determines whether a coordinate is in bounds, according to spatial dimensions.
		/// </summary>
		/// <param name="cf">Coordinate to test.</param>
		/// <returns>Boolean indicating whether the coordinate is in bounds spatially.</returns>
		public bool IsInBounds(CoordFive cf)
		{
			if (cf.X < 0 || cf.X >= Width)
				return false;
			if (cf.Y < 0 || cf.Y >= Height)
				return false;
			return true;
		}

		/// <summary>
		/// Gets a string representation of the board. This string starts with the 1st rank, and each rank is a line of text.
		/// </summary>
		public override string ToString()
		{
			string temp = "  ";
			for (int x = 0; x < Height; x++)
			{
				temp += StringUtils. IntToFile(x);
			}
			temp += "\n";
			for (int y = Height-1; y >= 0; y--)
			{
				temp += (y + 1).ToString() + " ";
				for (int x = 0; x < Width; x++)
				{
					int piece = GetSquare(x,y);
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
			return true;//GameState.WHITE; TODO Add this global back in.
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
