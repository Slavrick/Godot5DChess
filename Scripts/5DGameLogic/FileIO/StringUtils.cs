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

		/// <summary>
		/// Get a SAN 2D coord of the given object such as a1 e4 ....
		/// </summary>
		/// <returns>String SAN representation.</returns>
		public static string SANString(CoordFive c)
		{
			return $"{IntToFile(c.X)}{c.Y + 1}";
		}

		/// <summary>
		/// Raw Coordinate notation for this. 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		public static string RawMoveNotation(Move m)
		{
			return RawCoordString(m.Origin) + RawCoordString(m.Dest);
		}

		public static string ToRawShadString(Move m)
		{
			if (m.SpecialType == Move.CASTLE)
			{
				if (m.Dest.X > m.Origin.X)
				{
					return "O-O";
				}
				else
				{
					return "O-O-O";
				}
			}
			string move = "";
			string temporalOrigin = "(" + m.Origin.L + "T" + m.Origin.T + ")";
			string sanOrigin = SANString(m.Origin);
			string destStr = "(" + m.Dest.L + "T" + m.Dest.T + ")" + SANString(m.Dest);
			int piece = m.PieceMoved;
			if (piece > Board.numTypes)
			{
				piece -= Board.numTypes;
			}
			move = temporalOrigin;
			if (m.PieceMoved != Board.EMPTYSQUARE && piece != 1)
			{
				move += Board.PieceChars[piece];
			}
			move += sanOrigin;
			if (m.Type == Move.SPATIALMOVE)
			{
				move += SANString(m.Dest);
			}
			else if (m.Type == Move.JUMPINGMOVE)
			{
				move += ">" + destStr;
			}
			else if (m.Type == Move.BRANCHINGMOVE)
			{
				move += ">>" + destStr;
			}
			if (m.SpecialType > Move.CASTLE)
			{
				int promoted = m.SpecialType;
				if (m.SpecialType > Board.numTypes)
				{
					promoted -= Board.numTypes;
				}
				move += "=" + Board.PieceChars[promoted];
			}
			return move;
		}

		public static string ToShadString(Move m)
		{
			string move = "";
			int piece = m.PieceMoved;
			if (piece > Board.numTypes)
			{
				piece -= Board.numTypes;
			}
			if (m.PieceMoved != Board.EMPTYSQUARE && piece != 1)
			{
				move += Board.PieceChars[piece];
			}
			move = "(" + m.Origin.L + "T" + m.Origin.T + ")" + move;
			if (m.Type == Move.SPATIALMOVE)
			{
				move += SANString(m.Dest);
			}
			else if (m.Type == Move.JUMPINGMOVE)
			{
				move += SANString(m.Origin) + ">(" + m.Dest.L + "T" + m.Dest.T + ")" + SANString(m.Dest);
			}
			else if (m.Type == Move.BRANCHINGMOVE)
			{
				move += ">>(" + m.Dest.L + "T" + m.Dest.T + ")" + SANString(m.Dest);
			}
			return move;
		}

		public static string TurnString(Turn t, int type = 0)
		{
			if (t.Moves == null)
            {
                return "";
            }
            string temp = "";
            temp += ((t.TurnNum + 1) / 2) + "" + (t.TurnNum % 2 == 1 ? 'w' : 'b') + ".";
            switch (type)
            {
                case 0://Shad
                    foreach (Move m in t.Moves)
                    {
                        temp += ToShadString(m);
                        temp += " ";
                    }
                    break;
                case 1://Shadraw
                    foreach (Move m in t.Moves)
                    {
                        temp += ToRawShadString(m);
                        temp += " ";
                    }
                    break;
                case 2://Coordinate
                default:
                    foreach (Move m in t.Moves)
                    {
                        temp += RawMoveNotation(m);
                        temp += "; ";
                    }
                    break;
            }
            return temp;
		}
	}
}