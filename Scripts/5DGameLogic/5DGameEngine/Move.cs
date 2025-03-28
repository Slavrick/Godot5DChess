using System;

namespace Engine
{
	public class Move : IComparable<Move>
	{
		public CoordFour Origin { get; set; }
		public CoordFour Dest { get; set; }
		public int PieceMoved { get; set; }
		// 1 for spatial, 2 for jumping(unrecognized branching, 3 for branching.
		public int Type { get; set; }

		public static readonly int NULLMOVE = -1;
		public static readonly int NORMALMOVE = 0;
		public static readonly int CASTLE = 1;
		public static readonly int PROMOTION = 2;
		public static readonly int SPATIALMOVE = 1;
		public static readonly int JUMPINGMOVE = 2;
		public static readonly int BRANCHINGMOVE = 3;

		// 0 is normal, 1 is castling, and 2+ is promotion with the type being promoted.
		// For castling, origin in the king and dest is the rook.
		public int SpecialType { get; set; }

		public Move(CoordFour coordOrigin, CoordFour coordDest)
		{
			SpecialType = 0;
			Origin = coordOrigin;
			Dest = coordDest;
			if (Origin.L == Dest.L && Origin.T == Dest.T)
			{
				Type = SPATIALMOVE;
			}
			else
			{
				Type = JUMPINGMOVE;
			}
			PieceMoved = 0;
		}

		public Move(CoordFour coordOrigin, CoordFour coordDest, int type)
		{
			SpecialType = type;
			Origin = coordOrigin;
			Dest = coordDest;
			if (Origin.L == Dest.L && Origin.T == Dest.T)
			{
				Type = SPATIALMOVE;
			}
			else
			{
				Type = JUMPINGMOVE;
			}
			PieceMoved = 0;
		}

		public Move(CoordFour boardOrigin)
		{
			Origin = boardOrigin;
			Dest = new CoordFour(0, 0, 0, 0);
			Type = SPATIALMOVE;
			SpecialType = NULLMOVE;
		}

		public string RawMoveNotation()
		{
			return Origin.RawCoordString() + Dest.RawCoordString();
		}

		public override string ToString()
		{
			string moveStr = "";
			moveStr += Origin.ToString();
			moveStr += ">>";
			moveStr += Dest.ToString();
			return moveStr;
		}

		// This implements shads notation, mainly used for debugging(The eclipse debugger uses toString to show objects)
		public string ToString(char piece)
		{
			string move = "";
			if (Type == SPATIALMOVE)
			{
				move += "(" + Origin.L + "T" + Origin.T + ")" + piece + Dest.SANString();
			}
			else if (Type == JUMPINGMOVE)
			{
				move += "(" + Origin.L + "T" + Origin.T + ")" + piece + Origin.SANString() + ">("
						+ Dest.L + "T" + Dest.T + ")" + Dest.SANString();
			}
			else if (Type == BRANCHINGMOVE)
			{
				move += "(" + Origin.L + "T" + Origin.T + ")" + piece + Origin.SANString() + ">>("
						+ Dest.L + "T" + Dest.T + ")" + Dest.SANString();
			}
			return move;
		}

		public string ToShadString()
		{
			string move = "";
			int piece = PieceMoved;
			if (piece > Board.numTypes)
			{
				piece -= Board.numTypes;
			}
			if (PieceMoved != Board.EMPTYSQUARE && piece != 1)
			{
				move += Board.PieceChars[piece];
			}
			move = "(" + Origin.L + "T" + Origin.T + ")" + move;
			if (Type == SPATIALMOVE)
			{
				move += Dest.SANString();
			}
			else if (Type == JUMPINGMOVE)
			{
				move += Origin.SANString() + ">(" + Dest.L + "T" + Dest.T + ")" + Dest.SANString();
			}
			else if (Type == BRANCHINGMOVE)
			{
				move += ">>(" + Dest.L + "T" + Dest.T + ")" + Dest.SANString();
			}
			return move;
		}

		public int CompareTo(Move m2)
		{
			if (m2.Type > this.Type)
			{
				return -1;
			}
			if (m2.Type == this.Type)
			{
				return 0;
			}
			return 1;
		}

		public bool Equals(Move m)
		{
			return this.Origin.Equals(m.Origin) && this.Dest.Equals(m.Dest);
		}

		public string ToRawShadString()
		{
			if (this.SpecialType == CASTLE)
			{
				if (Dest.X > Origin.X)
				{
					return "O-O";
				}
				else
				{
					return "O-O-O";
				}
			}
			string move = "";
			string temporalOrigin = "(" + Origin.L + "T" + Origin.T + ")";
			string sanOrigin = Origin.SANString();
			string destStr = "(" + Dest.L + "T" + Dest.T + ")" + Dest.SANString();
			int piece = this.PieceMoved;
			if (piece > Board.numTypes)
			{
				piece -= Board.numTypes;
			}
			move = temporalOrigin;
			if (PieceMoved != Board.EMPTYSQUARE && piece != 1)
			{
				move += Board.PieceChars[piece];
			}
			move += sanOrigin;
			if (Type == SPATIALMOVE)
			{
				move += Dest.SANString();
			}
			else if (Type == JUMPINGMOVE)
			{
				move += ">" + destStr;
			}
			else if (Type == BRANCHINGMOVE)
			{
				move += ">>" + destStr;
			}
			if (this.SpecialType > CASTLE)
			{
				int promoted = SpecialType;
				if (SpecialType > Board.numTypes)
				{
					promoted -= Board.numTypes;
				}
				move += "=" + Board.PieceChars[promoted];
			}
			return move;
		}
	}
}
