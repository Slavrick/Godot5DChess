using System;
using System.Collections.Generic;
//using GUI;

namespace FiveDChess
{
    public class GameStateManager : GameState
    {
        public Move[] PreMoves;
        public Timeline[] OriginsTL;
        public int StartMinTL;
        public AnnotationTree ATR;
        public AnnotationTree.Node Index;
        public int CurrTurn;


        public GameStateManager(Timeline[] origins, int width, int height, bool evenStart, bool color, int minTL, Move[] moves)
            : base(origins, width, height, evenStart, color, minTL, moves)
        {
            this.PreMoves = moves;
            OriginsTL = origins;
            CurrTurn = 1;
            ATR = new AnnotationTree(new Turn());
            Index = ATR.Root;
        }

        public bool SubmitMoves()
        {
            bool presColor = CalcPresent();
            if (!OpponentCanCaptureKing() && !(presColor == Color))
            {
                Turn newTurn = new Turn(TurnMoves, TurnTLS);
                newTurn.TurnNum = CurrTurn;
                if (!AnnotationTree.ContainsChild(Index, newTurn))
                {
                    Index = Index.AddChild(newTurn);
                }
                else
                {
                    Index = AnnotationTree.FindNode(Index, newTurn);
                }
                CurrTurn++;
                TurnTLS.Clear();
                TurnMoves.Clear();
                Color = !Color;
                StartPresent = Present;
                return true;
            }
            return false;
        }

        public bool NavigateToTurn(int indexClicked)
        {
            UndoTempMoves();
            if (indexClicked == -1)
            {
                return true;
            }
            if (indexClicked == 0)
            {
                while (Index != ATR.Root)
                {
                    UndoTree();
                }
                return true;
            }
            List<AnnotationTree.Node> nodeList = AnnotationTree.GetNodesLinear(ATR.Root);
            AnnotationTree.Node target = nodeList[indexClicked];
            while (!AnnotationTree.Contains(Index, target.NodeID))
            {
                UndoTree();
            }
            List<int> navPath = AnnotationTree.NavPath(Index, target.NodeID);
            if (navPath == null)
            {
                return true;
            }
            foreach (int i in navPath)
            {
                ProgressTree(i);
            }
            return true;
        }

        private void UndoTree()
        {
            if (Index.Parent != null)
            {
                UndoTurn(Index.AT.T.TLs);
                CurrTurn--;
                Index = Index.Parent;
            }
        }

        private bool ProgressTree(int childIndex)
        {
            if (Index.Children.Count <= childIndex)
            {
                return false;
            }
            Index = Index.Children[childIndex];
            IncrementTurn(Index.AT.T);
            return true;
        }

        public bool MakeTurn(Turn turn)
        {
            if (turn == null)
            {
                return false;
            }
            foreach (Move m in turn.Moves)
            {
                if (!this.MakeMove(m,false))
                {
                    //UndoTempMoves(); TODO not sure if this is needed, I think it is though.
                    return false;
                }
            }
            this.SubmitMoves();
            return true;
        }

        /// <summary>
        /// Increments the turn without validation. Used with turntree
        /// </summary>
        /// <param name="t"></param>
        /// <returns>true always(indicates the turn worked)</returns>
        private bool IncrementTurn(Turn t)
        {
            CurrTurn++;
            foreach (Move m in t.Moves)
            {
                this.MakeSilentMove(m);
            }
            DetermineActiveTLS();
            Color = !Color;
            StartPresent = Present;
            return true;
        }

        public static void PrintTree(AnnotationTree t)
        {
            List<AnnotationTree.Node> nodes = AnnotationTree.GetNodesLinear(t.Root);
            foreach (AnnotationTree.Node n in nodes)
            {
                Console.Write(n);
                foreach (AnnotationTree.Node child in n.Children)
                {
                    Console.Write(" : " + child);
                }
                Console.WriteLine();
            }
        }
    }
}