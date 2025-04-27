using System;
using FiveDChess;

namespace FileIO5D {
	public class StringUtils
	{
		public static readonly char[] colorchars =
		{
			'R','B','G','Y','O'
		};


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
			if (m.SpecialType == Move.NULLMOVE)
			{
				return $"({m.Origin.L}T{m.Origin.T})0000";

			}
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
			if (piece > Board.NUMTYPES)
			{
				piece -= Board.NUMTYPES;
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
				if (m.SpecialType > Board.NUMTYPES)
				{
					promoted -= Board.NUMTYPES;
				}
				move += "=" + Board.PieceChars[promoted];
			}
			return move;
		}

		public static string ToShadString(Move m)
		{
			string move = "";
			int piece = m.PieceMoved;
			if (m.SpecialType == Move.NULLMOVE)
			{
				return $"({m.Origin.L}T{m.Origin.T})0000";
			}
			if (m.SpecialType == Move.CASTLE)
			{
				return $"({m.Origin.L}T{m.Origin.T})O-O";
			}
			if (piece > Board.NUMTYPES)
			{
				piece -= Board.NUMTYPES;
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
			if (m.SpecialType > Move.CASTLE)
			{
				int promotionpiece = m.SpecialType;
				if (promotionpiece > Board.NUMTYPES)
				{
					promotionpiece -= Board.NUMTYPES;
				}
				move += "=" + Board.PieceChars[promotionpiece];
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

		public static string TurnExportString(Turn t, int type = 0)
		{
			if (t.Moves == null)
			{
				return "";
			}
			string temp = "";
			foreach (Move m in t.Moves)
			{
				temp += ToRawShadString(m);
				temp += " ";
			}
			return temp;
		}

		public static string AnnotatedTurnExportString(AnnotatedTurn at)
		{
			string exportString = "";
			//exportString += TurnExportString(at.T);
			if (at.Annotation != null && at.Annotation.Length > 0)
			{
				exportString += $" {{%c {at.Annotation} }}";
			}
			if (at.Highlights != null && at.Highlights.Count > 0)
			{
				exportString += $" {{[%csl ";
				for (int i = 0; i < at.Highlights.Count; i++)
				{
					char colorchar = colorchars[at.HighlightColors[i]];
					char turnchar = at.Highlights[i].Color ? 'w' : 'b';
					exportString += $"{colorchar}" + CoordAnnotationString(at.Highlights[i]) +  " ";
				}
				exportString += $"]}} ";
			}
			if (at.Arrows != null && at.Arrows.Count > 0)
			{
				exportString += $" {{[%cal ";
				Console.WriteLine(at.Arrows.Count);
				Console.WriteLine(at.ArrowColors.Count);
				for (int i = 0; i < at.Arrows.Count; i++)
				{
					exportString += AnnotationMoveCoordinate(at.Arrows[i], at.ArrowColors[i]) + " ";                   
				}
				exportString += $"]}} ";
			}
			return exportString;
		}

		public static string AnnotationMoveCoordinate(Move m,int color)
		{
			string annotationString = "";
            char colorchar = colorchars[color];
            char turnchar = m.Origin.Color ? 'w' : 'b';
            annotationString += $"{colorchar}({turnchar}.{m.Origin.L}T{m.Origin.T}){IntToFile(m.Origin.X)}{m.Origin.Y + 1}";
            annotationString += $"({m.Dest.L}T{m.Dest.T}){IntToFile(m.Dest.X)}{m.Dest.Y + 1}";
            return annotationString;

        }

		public static string CoordAnnotationString(CoordFive c)
		{
			char turnchar = c.Color ? 'w' : 'b';
			return $"({turnchar}.{c.L}T{c.T}){IntToFile(c.X)}{c.Y + 1}";
		}

    }
}