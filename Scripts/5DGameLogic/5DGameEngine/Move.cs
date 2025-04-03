using System;
using FileIO5D;

namespace FiveDChess
{
	public class Move : IComparable<Move>
	{
		public CoordFive Origin;
		public CoordFive Dest;
		public int PieceMoved;
		// 1 for spatial, 2 for jumping(unrecognized branching), 3 for branching.
		public int Type;
		public int SpecialType;

		public static readonly int NULLMOVE = -1;
		public static readonly int NORMALMOVE = 0;
		public static readonly int CASTLE = 1;
		public static readonly int PROMOTION = 2;

		public static readonly int SPATIALMOVE = 1;
		public static readonly int JUMPINGMOVE = 2;
		public static readonly int BRANCHINGMOVE = 3;
		public static readonly Move NULL = new Move(new CoordFive(0,0,0,0,true),new CoordFive(0,0,0,0,true),1,-1);
		// 0 is normal, 1 is castling, and 2+ is promotion with the type being promoted.
		// For castling, origin in the king and dest is the rook.

		public Move(CoordFive coordOrigin, CoordFive coordDest)
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

		public Move(CoordFive coordOrigin, CoordFive coordDest, int type, int specialType = 0)
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
			SpecialType = specialType;
		}

        public Move(CoordFive boardOrigin)
		{
			Origin = boardOrigin;
			Dest = new CoordFive(0, 0, 0, 0,boardOrigin.Color);
			Type = SPATIALMOVE;
			SpecialType = NULLMOVE;
		}

		public Move Clone()
		{
			Move newMove = new Move(Origin.Clone(),Dest.Clone(),SpecialType);
			newMove.Type = Type;
			newMove.PieceMoved = PieceMoved;
			newMove.SpecialType = SpecialType;
			return newMove;
		}

		public void SwapColor()
		{
			Origin.Color = !Origin.Color;
			Dest.Color = !Dest.Color;
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
				move += "(" + Origin.L + "T" + Origin.T + ")" + piece + StringUtils.SANString(Dest);
			}
			else if (Type == JUMPINGMOVE)
			{
				move += "(" + Origin.L + "T" + Origin.T + ")" + piece + StringUtils.SANString(Origin) + ">("
						+ Dest.L + "T" + Dest.T + ")" + StringUtils.SANString(Dest);
			}
			else if (Type == BRANCHINGMOVE)
			{
				move += "(" + Origin.L + "T" + Origin.T + ")" + piece + StringUtils.SANString(Origin) + ">>("
						+ Dest.L + "T" + Dest.T + ")" + StringUtils.SANString(Dest);
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
	}
}
