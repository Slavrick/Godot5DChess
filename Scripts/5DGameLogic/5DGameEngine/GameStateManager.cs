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
        public List<Turn> Turns;
        public TurnTree TurnTree;
        public TurnTree.Node Index;
        public int CurrTurn;

        public GameStateManager(Timeline[] origins, int width, int height, bool evenStart, bool color, int minTL, Move[] moves)
            : base(origins, width, height, evenStart, color, minTL, moves)
        {
            this.PreMoves = moves;
            OriginsTL = origins;
            this.Turns = new List<Turn>();
            CurrTurn = 1;
            TurnTree = new TurnTree(new Turn());
            Index = TurnTree.Root;
        }

        public bool SubmitMoves()
        {
            bool presColor = CalcPresent();
            if (!OpponentCanCaptureKing() && !(presColor == Color))
            {
                if (CurrTurn + 1 < Turns.Count)
                {
                    ClearFutureTurns();
                }
                Turn newTurn = new Turn(TurnMoves, TurnTLS);
                newTurn.TurnNum = CurrTurn;
                if (!TurnTree.Contains(Index, newTurn))
                {
                    Index = Index.AddChild(newTurn);
                }
                else
                {
                    Index = TurnTree.FindNode(Index, newTurn);
                }
                CurrTurn++;
                Turns.Add(new Turn(TurnMoves, TurnTLS));
                Turns[Turns.Count - 1].TurnNum = CurrTurn / 2 + 1;
                TurnTLS.Clear();
                TurnMoves.Clear();
                Color = !Color;
                StartPresent = Present;
                return true;
            }
            return false;
        }

        public bool SetTurn(int targetTurn)
        {
            UndoTempMoves();
            if (targetTurn < -1 || targetTurn > Turns.Count)
            {
                return false;
            }
            if (CurrTurn == targetTurn)
            {
                return true;
            }
            if (targetTurn > CurrTurn)
            {
                while (CurrTurn < targetTurn)
                {
                    IncrementTurn(Turns[CurrTurn + 1]);
                }
            }
            else
            {
                while (CurrTurn > targetTurn)
                {
                    if (Index.Parent != null)
                    {
                        Index = Index.Parent;
                    }
                    UndoTurn(Turns[CurrTurn].TLs);
                    CurrTurn--;
                }
            }
            return true;
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
                while (Index != TurnTree.Root)
                {
                    UndoTree();
                }
                return true;
            }
            List<TurnTree.Node> nodeList = TurnTree.GetNodesLinear();
            TurnTree.Node target = nodeList[indexClicked];
            while (!TurnTree.Contains(Index, target.NodeID))
            {
                UndoTree();
            }
            List<int> navPath = TurnTree.NavPath(Index, target.NodeID);
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
                UndoTurn(((Turn)Index.Data).TLs);
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
            IncrementTurn((Turn)Index.Data);
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
                    //UndoTempMoves(); not sure if this is neede.
                    return false;
                }
            }
            this.SubmitMoves();
            return true;
        }

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

        private void ClearFutureTurns()
        {
            for (int i = Turns.Count - 1; i > CurrTurn; i--)
            {
                Turns.RemoveAt(i);
            }
        }

        public static void PrintTree(TurnTree t)
        {
            List<TurnTree.Node> nodes = t.GetNodesLinear();
            foreach (TurnTree.Node n in nodes)
            {
                Console.Write(n);
                foreach (TurnTree.Node child in n.Children)
                {
                    Console.Write(" : " + child);
                }
                Console.WriteLine();
            }
        }
    }
}