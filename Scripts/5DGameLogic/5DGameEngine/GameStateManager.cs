using System;
using System.Collections.Generic;
//using GUI;

namespace Engine
{
	public class GameStateManager : GameState
	{
		public Move[] PreMoves { get; set; }
		public Timeline[] OriginsTL { get; set; }
		public int StartMinTL { get; set; }
		public List<Turn> Turns { get; set; }
		public TurnTree TurnTree { get; set; }
		public TurnTree.Node Index { get; set; }
		public int CurrTurn { get; set; }

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
			bool presColor = calcPresent();
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
			//if (GUI.Globals.ES != null)
			//{
				//if (presColor == Color)
				//{
					//MessageEvent m = new MessageEvent("The present Still rests on your color.");
					//GUI.Globals.ES.BroadcastEvent(m);
				//}
				//else
				//{
					//List<CoordFive> checkingPiece = MoveGenerator.GetAllCheckingPieces(this);
					//MessageEvent m = new MessageEvent("Submitting now Would Allow your opponent to capture your king. \n For example, the piece on: " + checkingPiece + " can capture your piece");
					//GUI.Globals.ES.BroadcastEvent(m);
				//}
			//}
			return false;
		}

		public bool SetTurn(int targetTurn)
		{
			undoTempMoves();
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
			undoTempMoves();
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
				UndoTurn(Index.Data.TLs);
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
			IncrementTurn(Index.Data);
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
				if (!this.MakeMove(m))
				{
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
			determineActiveTLS();
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

		//this is for the game UI, no functional attributes to this function. this is used to pan the board.
		public CoordFive GetPresentCoordinate(int skip){
			for(int i = MinActiveTL ; i <= MaxActiveTL; i++){
				Timeline t = GetTimeline(i); 
				if(t.ColorPlayable == this.Color && t.TEnd <= Present){
					return new CoordFive(0,0,i,t.TEnd);
				}
			}
			//fallback
			for(int i = MinActiveTL ; i <= MinActiveTL; i++){
				Timeline t = GetTimeline(getTLIndex(i,MinTL));
				return new CoordFive(0,0,i,t.TEnd);
			}
			return null;
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
