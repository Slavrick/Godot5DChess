using System;
using System.Collections.Generic;

namespace Engine
{
	public class Timeline : IComparable<Timeline>
	{
		/* the white and black boards on the timeline. */
		public List<Board> WBoards { get; set; }
		public List<Board> BBoards { get; set; }
		public int Layer { get; set; }
		/* the absolute start and end time of both white or black */
		public int TStart { get; set; }
		public int TEnd { get; set; }
		/* white start time, end time */
		public int WhiteStart { get; set; }
		public int WhiteEnd { get; set; }
		/* black start time, end time */
		public int BlackStart { get; set; }
		public int BlackEnd { get; set; }
		/* the color of the current playable board */
		public bool ColorPlayable { get; set; }
		/*
		 * the color of the first board of the timeline, white(true) for any timeline <
		 * 0. and black(false) for any timeline above 0
		 */
		public bool ColorStart { get; set; }
		/* the "activeness" of the timeline, not sure if this fits here */
		public bool Active { get; set; }

		// starts the timeline with a timestart and layer
		public Timeline(Board origin, bool boardColor, int startTime, int layer)
		{
			WBoards = new List<Board>();
			BBoards = new List<Board>();
			if (boardColor)
			{
				WBoards.Add(origin);
				WhiteStart = startTime;
				WhiteEnd = startTime;
				BlackStart = startTime;
				BlackEnd = startTime - 1;
			}
			else
			{
				BBoards.Add(origin);
				WhiteStart = startTime + 1;
				WhiteEnd = startTime;
				BlackStart = startTime;
				BlackEnd = startTime;
			}
			Layer = layer;
			ColorPlayable = boardColor;
			ColorStart = boardColor;
			TStart = startTime;
			TEnd = startTime;
		}

		// returns if the time exists or not, ie if the time is in bounds
		// @TODO fix this, not necessarily true for instance if white branches to t1,
		// a black t1 exists but not white t1, but timestart would be 1,
		public bool TimeExists(int t, bool boardColor)
		{
			if ((boardColor == GameState.WHITE && (t < WhiteStart || t > WhiteEnd)))
			{
				return false;
			}
			else if ((boardColor == GameState.BLACK && (t < BlackStart || t > BlackEnd)))
			{
				return false;
			}
			return true;
		}

		public bool IsMostRecentTime(int t, bool color)
		{
			return ColorPlayable == color && t == TEnd;
		}

		// gets the board on the timeline at time T and color C
		public Board GetBoard(int t, bool boardColor)
		{
			if (!TimeExists(t, boardColor))
			{
				return null;
			}
			if (boardColor == GameState.WHITE)
			{
				return WBoards[t - WhiteStart];
			}
			else
			{
				return BBoards[t - BlackStart];
			}
		}

		// gets the playable board on the timeline
		public Board GetPlayableBoard()
		{
			if (ColorPlayable)
			{
				return WBoards[WBoards.Count - 1];
			}
			else
			{
				return BBoards[BBoards.Count - 1];
			}
		}

		public int getSquare(CoordFour c, bool color)
		{
			Board b = GetBoard(c.T, color);
			if (b != null)
			{
				return b.getSquare(c);
			}
			return Board.ERRORSQUARE;
		}

		public bool AddSpatialMove(Move m, bool moveColor)
		{
			if (moveColor != ColorPlayable) // XXX move this validation up the chain.
				return false;
			Board b = GetPlayableBoard();
			Board newBoard = new Board(b);
			int piece = newBoard.getSquare(m.Origin);
			piece = piece < 0 ? piece * -1 : piece;
			m.PieceMoved = piece;
			newBoard.setSquare(m.Origin, (int)Board.Piece.EMPTY);
			newBoard.setSquare(m.Dest, piece);
			if ((piece == (int)Board.Piece.WPAWN || piece == (int)Board.Piece.BPAWN || piece == (int)Board.Piece.WBRAWN || piece == (int)Board.Piece.BBRAWN) && Math.Abs(m.Dest.Y - m.Origin.Y) == 2)
			{
				newBoard.EnPassentSquare = m.Dest.Clone();
				newBoard.EnPassentSquare.Y = (m.Dest.Y + m.Origin.Y) / 2;
			}
			if ((piece == (int)Board.Piece.WPAWN || piece == (int)Board.Piece.BPAWN || piece == (int)Board.Piece.WBRAWN || piece == (int)Board.Piece.BBRAWN) && b.EnPassentSquare != null && b.EnPassentSquare.Equals(m.Dest))
			{
				CoordFour pawnSquare = m.Origin.Clone();
				pawnSquare.X = m.Dest.X;
				newBoard.setSquare(pawnSquare, Board.EMPTYSQUARE);
			}
			return AddMove(newBoard);
		}

		public bool CastleKing(Move m)
		{
			Board b = GetPlayableBoard();
			Board newBoard = new Board(b);
			int king = newBoard.getSquare(m.Origin) * -1;
			CoordFour direction = CoordFour.Sub(m.Dest, m.Origin);
			direction.Flatten();
			CoordFour index = CoordFour.Add(direction, m.Origin);
			while (b.getSquare(index) != (int)Board.Piece.WROOK * -1 && b.getSquare(index) != (int)Board.Piece.BROOK * -1)
			{
				index.Add(direction);
			}
			m.PieceMoved = king;
			int rook = newBoard.getSquare(index) * -1;
			newBoard.setSquare(m.Origin, Board.EMPTYSQUARE);
			newBoard.setSquare(index, Board.EMPTYSQUARE);
			newBoard.setSquare(m.Dest, king);
			newBoard.setSquare(CoordFour.Sub(m.Dest, direction), rook);
			return AddMove(newBoard);
		}

		public bool Promote(Move m)
		{
			Board b = GetPlayableBoard();
			Board newBoard = new Board(b);
			newBoard.setSquare(m.Origin, (int)Board.Piece.EMPTY);
			newBoard.setSquare(m.Dest, m.SpecialType);
			return AddMove(newBoard);
		}

		// adds jumping origin move, basically just removes that piece from the board.
		public int AddJumpingMove(Move m, bool moveColor)
		{
			if (moveColor != ColorPlayable)
				return -1;
			CoordFour origin = m.Origin;
			Board b = GetPlayableBoard();
			Board newBoard = new Board(b);
			int piece = newBoard.getSquare(origin);
			piece = piece < 0 ? piece * -1 : piece;
			m.PieceMoved = piece;
			newBoard.setSquare(origin, (int)Board.Piece.EMPTY);
			AddMove(newBoard);
			return piece;
		}

		// add a move jumping, if the move is branching return the branched board,
		// otherwise, add the board onto the end of the timeline.
		public Board AddJumpingMoveDest(CoordFour dest, bool moveColor, int piece)
		{
			Board b = GetBoard(dest.T, moveColor);
			Board newBoard = new Board(b);
			newBoard.setSquare(dest, piece);
			if (dest.T != TEnd || moveColor != ColorPlayable)
			{
				return newBoard;
			}
			AddMove(newBoard);
			return null;
		}

		// Adds a null move to the Timeline. This can be used to check for certain
		// things.
		public void AddNullMove()
		{
			AddMove(GetPlayableBoard());
		}

		// add a board to the end of the timeline.
		private bool AddMove(Board b)
		{
			if (ColorPlayable)
			{
				BBoards.Add(b);
				BlackEnd++;
			}
			else
			{
				WBoards.Add(b);
				WhiteEnd++;
				TEnd++;
			}
			ColorPlayable = !ColorPlayable;
			return true;
		}

		// will pop off the last board to 'undo'
		public bool UndoMove()
		{
			if (ColorPlayable)
			{
				WBoards.RemoveAt(WBoards.Count - 1);
				WhiteEnd--;
				TEnd--;
			}
			else
			{
				BBoards.RemoveAt(BBoards.Count - 1);
				BlackEnd--;
			}
			ColorPlayable = !ColorPlayable;
			if (WBoards.Count == 0 && BBoards.Count == 0)
			{
				return true;
			}
			return false;
		}

		// prints the board in a primitive way
		public void PrintTimeline()
		{
			int lastWIndex = WBoards.Count;
			int lastBIndex = BBoards.Count;
			if (ColorStart)
			{
				for (int i = 0; i < lastBIndex || i < lastWIndex; i++)
				{
					if (i < lastWIndex)
					{
						Console.WriteLine($"__W_T_{i + TStart}__\n");
						Console.WriteLine(WBoards[i]);
					}
					if (i < lastBIndex)
					{
						Console.WriteLine($"__B_T_{i + TStart}__\n");
						Console.WriteLine(BBoards[i]);
					}
				}
			}
			else
			{
				for (int i = 0; i < lastBIndex || i < lastWIndex; i++)
				{
					if (i < lastBIndex)
					{
						Console.WriteLine($"__B_T_{i + TStart}__\n");
						Console.WriteLine(BBoards[i]);
					}
					if (i < lastWIndex)
					{
						Console.WriteLine($"__W_T_{i + TStart}__\n");
						Console.WriteLine(WBoards[i]);
					}
				}
			}
		}

		public int CompareTo(Timeline compareTo)
		{
			if (this.Layer > compareTo.Layer)
			{
				return 1;
			}
			if (this.Layer < compareTo.Layer)
			{
				return -1;
			}
			return 0;
		}
	}
}
