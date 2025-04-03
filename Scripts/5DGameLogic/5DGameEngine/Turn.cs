using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using FileIO5D;

namespace FiveDChess
{
    public class Turn
    {
        //Moves to make
        public Move[] Moves;
        //Timelines Moved
        public int[] TLs;
        // This is ply, not actual turn num, so 2 would be blacks first turn and 3 would be whites 2nd
        public int TurnNum;
        //whether or not this can be undoable. does nothing so far.
        public bool Undoable = false;

        public Turn(List<Move> turnMoves, List<int> tlEffected, int turnNum)
        {
            TurnNum = turnNum;
            if( turnMoves == null ) { turnMoves = new List<Move>(); }
            if( tlEffected == null) { tlEffected = new List<int>(); }

            this.Moves = turnMoves.ToArray();
            this.TLs = tlEffected.ToArray();
            if(tlEffected != null)
            {
                Undoable = true;
            }
        }

        public Turn(List<Move> turnMoves, List<int> tlEffected) : this(turnMoves,tlEffected,0) { }

        public Turn() : this(null ,null,0) { }//TODO this will 

        /// <summary>
        /// This renders this non-undoable since we aren't adding in the timelines that are branching
        /// </summary>
        /// <param name="tmoves"></param>
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
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
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

        public override string ToString()
        {
            if (Moves == null)
            {
                return "";
            }
            string temp = "";
            temp += ((TurnNum + 1) / 2) + "" + (TurnNum % 2 == 1 ? 'w' : 'b') + ".";
            foreach (Move m in Moves)
            {
                temp += StringUtils.ToShadString(m);
                temp += " ";
            }
            return temp;
        }

        /// <summary>
        /// Compares based on comparator. 1 for greater than, 0 for equal, -1 for less than
        /// </summary>
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