using System;
using System.Collections.Generic;

namespace Engine
{
    public class Turn
    {
        public Move[] Moves { get; set; }
        public int[] TLs { get; set; }
        // This is ply, not actual turn num, so 2 would be blacks first turn and 3 would be whites 2nd
        public int TurnNum { get; set; }

        // XXX implement or delete these fields
        public int NumTL { get; set; }
        public bool Color { get; set; }
        public int TPresent { get; set; }

        public static NotationMode Mode { get; set; } = NotationMode.SHAD;
        public static PrefixMode Pre { get; set; } = PrefixMode.TURN;

        public enum PrefixMode
        {
            NONE, TURN, TURNANDPRESENT
        }

        public enum NotationMode
        {
            COORDINATE, SHAD, SHADRAW
        }

        // This will do no validation as of yet.
        public Turn(List<Move> turnMoves, List<int> tlEffected)
        {
            this.Moves = turnMoves.ToArray();
            this.TLs = tlEffected.ToArray();
        }

        public Turn(Move[] tmoves)
        {
            List<Move> removedNull = new List<Move>();
            foreach (Move m in tmoves)
            {
                if (m != null)
                {
                    removedNull.Add(m);
                }
            }
            this.Moves = removedNull.ToArray();
            List<int> tlList = new List<int>();
            foreach (Move m in this.Moves)
            {
                tlList.Add(m.Origin.L);
                if (m.Type != Move.SPATIALMOVE)
                {
                    tlList.Add(m.Dest.L);
                }
            }
            this.TLs = tlList.ToArray();
            Mode = NotationMode.SHAD;
            Pre = PrefixMode.TURN;
        }

        public Turn()
        {
            Moves = null;
            TLs = null;
            TurnNum = 0;
        }

        public Move[] GetMoves()
        {
            return this.Moves;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Turn t = (Turn)obj;
            if (this.Moves == null && t.Moves == null)
            {
                return true;
            }
            if (this.Moves == null || t.Moves == null)
            {
                return false;
            }
            if (this.Moves.Length != t.Moves.Length)
            {
                return false;
            }
            foreach (Move ogMove in this.Moves)
            {
                bool found = false;
                foreach (Move compareTo in t.Moves)
                {
                    if (ogMove.Equals(compareTo))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            if (Moves == null)
            {
                return "";
            }
            string temp = "";
            switch (Pre)
            {
                case PrefixMode.TURN:
                    temp += ((TurnNum + 1) / 2) + "" + (TurnNum % 2 == 1 ? 'w' : 'b') + ".";
                    break;
                case PrefixMode.NONE:
                default:
                    break;
            }
            switch (Mode)
            {
                case NotationMode.SHAD:
                    foreach (Move m in Moves)
                    {
                        temp += m.ToShadString();
                        temp += " ";
                    }
                    break;
                case NotationMode.SHADRAW:
                    foreach (Move m in Moves)
                    {
                        temp += m.ToRawShadString();
                        temp += " ";
                    }
                    break;
                case NotationMode.COORDINATE:
                default:
                    foreach (Move m in Moves)
                    {
                        temp += m.RawMoveNotation();
                        temp += "; ";
                    }
                    break;
            }
            return temp;
        }

        private static class ShadNotation
        {
            public static string MovesToString(Turn t)
            {
                string temp = "";
                foreach (Move m in t.GetMoves())
                {
                    // TODO: Implement the method body
                }
                return temp;
            }
        }

        private class TLMoveComparator : IComparer<Move>
        {
            public int Compare(Move o1, Move o2)
            {
                if (o1.Origin.L > o2.Origin.L)
                {
                    return 1;
                }
                if (o1.Origin.L < o2.Origin.L)
                {
                    return -1;
                }
                return 0;
            }
        }
    }
}