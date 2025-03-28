using System;
using System.Collections.Generic;

namespace Engine
{
	public class Timeline : IComparable<Timeline>
	{
		/* the white and black boards on the timeline. */
		public List<Board> WBoards;
		public List<Board> BBoards;
		public int Layer;
		/* the absolute start and end time of both white or black */
		public int TStart;
		public int TEnd;
		/* white start time, end time */
		public int WhiteStart;
		public int WhiteEnd;
		/* black start time, end time */
		public int BlackStart;
		public int BlackEnd;
		/* the color of the current playable board */
		public bool ColorPlayable;
		/*
		 * the color of the first board of the timeline, white(true) for any timeline <
		 * 0. and black(false) for any timeline above 0 XXX Dont think this variable is used.
		 */
		public bool ColorStart;

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


		/// <summary>
		/// Determines if a time exists. 
		/// </summary>
		/// <param name="t">time to check</param>
		/// <param name="boardColor">Color to check</param>
		/// <returns>true if board exists, and false if it is out of bounds.</returns>
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

		/// <summary>
		/// Gets whether or not the most recent time, or playable board is there.
		/// </summary>
		/// <param name="t">Time to check</param>
		/// <param name="color">color to check</param>
		/// <returns>true if the provided coordinate is the most recent time, otherwise false</returns>
		public bool IsMostRecentTime(int t, bool color)
		{
			return ColorPlayable == color && t == TEnd;
		}

		/// <summary>
		/// Gets board based on time and color
		/// </summary>
		/// <param name="t">time to get</param>
		/// <param name="boardColor">color of board to get</param>
		/// <returns></returns>
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

		/// <summary>
		/// Gets the Playable board on the Timeline. (End and right color)
		/// </summary>
		/// <returns>Playable board.</returns>
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

		public int getSquare(CoordFive c, bool color) //TODO make this not use boardcolor XXX remove this when no longer needed.
		{
			Board b = GetBoard(c.T, color);
			if (b != null)
			{
				return b.GetSquare(c);
			}
			return Board.ERRORSQUARE;
		}

		/// <summary>
		/// Gets a square in the timeline. does not validate layer
		/// </summary>
		/// <param name="c">coord to get</param>
		/// <returns>square integer, or errorsquare if not found</returns>
		public int GetSquare(CoordFive c)
		{
			Board b = GetBoard(c.T,c.Color);
			if (b != null)
			{
				return b.GetSquare(c);
			}
			return Board.ERRORSQUARE;
		}

		public bool AddSpatialMove(Move m, bool moveColor) //TODO make this not use boardcolor XXX Remove this when no longer needed.
		{
			if (moveColor != ColorPlayable) // XXX move this validation up the chain.
				return false;
			Board b = GetPlayableBoard();
			Board newBoard = new Board(b);
			int piece = newBoard.GetSquare(m.Origin);
			piece = piece < 0 ? piece * -1 : piece;
			m.PieceMoved = piece;
			newBoard.SetSquare(m.Origin, (int)Board.Piece.EMPTY);
			newBoard.SetSquare(m.Dest, piece);
			if ((piece == (int)Board.Piece.WPAWN || piece == (int)Board.Piece.BPAWN || piece == (int)Board.Piece.WBRAWN || piece == (int)Board.Piece.BBRAWN) && Math.Abs(m.Dest.Y - m.Origin.Y) == 2)
			{
				newBoard.EnPassentSquare = m.Dest.Clone();
				newBoard.EnPassentSquare.Y = (m.Dest.Y + m.Origin.Y) / 2;
			}
			if ((piece == (int)Board.Piece.WPAWN || piece == (int)Board.Piece.BPAWN || piece == (int)Board.Piece.WBRAWN || piece == (int)Board.Piece.BBRAWN) && b.EnPassentSquare != null && b.EnPassentSquare.SpatialEquals(m.Dest))
			{
				CoordFive pawnSquare = m.Origin.Clone();
				pawnSquare.X = m.Dest.X;
				newBoard.SetSquare(pawnSquare, Board.EMPTYSQUARE);
			}
			return AddMove(newBoard);
		}

		/// <summary>
		///  adds a spatial move to the timeline.
		/// </summary>
		/// <param name="m">Move to add</param>
		/// <returns>true if the move worked, false if it failed</returns>
		public bool AddSpatialMove(Move m) 
		{
			if (m.Origin.Color != ColorPlayable) // XXX move this validation up the chain.
				return false;
			Board b = GetPlayableBoard();
			Board newBoard = new Board(b);
			int piece = newBoard.GetSquare(m.Origin);
			piece = piece < 0 ? piece * -1 : piece;
			m.PieceMoved = piece;
			newBoard.SetSquare(m.Origin, (int)Board.Piece.EMPTY);
			newBoard.SetSquare(m.Dest, piece);
			if ((piece == (int)Board.Piece.WPAWN || piece == (int)Board.Piece.BPAWN || piece == (int)Board.Piece.WBRAWN || piece == (int)Board.Piece.BBRAWN) && Math.Abs(m.Dest.Y - m.Origin.Y) == 2)
			{
				newBoard.EnPassentSquare = m.Dest.Clone();
				newBoard.EnPassentSquare.Y = (m.Dest.Y + m.Origin.Y) / 2;
			}
			if ((piece == (int)Board.Piece.WPAWN || piece == (int)Board.Piece.BPAWN || piece == (int)Board.Piece.WBRAWN || piece == (int)Board.Piece.BBRAWN) && b.EnPassentSquare != null && b.EnPassentSquare.SpatialEquals(m.Dest))
			{
				CoordFive pawnSquare = m.Origin.Clone();
				pawnSquare.X = m.Dest.X;
				newBoard.SetSquare(pawnSquare, Board.EMPTYSQUARE);
			}
			return AddMove(newBoard);
		}


		/// <summary>
		/// Castle the king. Does validation that is moving king and rook, marks them as moved.
		/// </summary>
		/// <param name="m">Castleing Move</param>
		/// <returns>true if move worked.</returns>
		public bool CastleKing(Move m)
		{
			Board b = GetPlayableBoard();
			Board newBoard = new Board(b);
			int king = newBoard.GetSquare(m.Origin) * -1;
			CoordFive direction = CoordFive.Sub(m.Dest, m.Origin);
			direction.Flatten();
			CoordFive index = CoordFive.Add(direction, m.Origin);
			while (b.GetSquare(index) != (int)Board.Piece.WROOK * -1 && b.GetSquare(index) != (int)Board.Piece.BROOK * -1)
			{
				index.Add(direction);
			}
			m.PieceMoved = king;
			int rook = newBoard.GetSquare(index) * -1;
			newBoard.SetSquare(m.Origin, Board.EMPTYSQUARE);
			newBoard.SetSquare(index, Board.EMPTYSQUARE);
			newBoard.SetSquare(m.Dest, king);
			newBoard.SetSquare(CoordFive.Sub(m.Dest, direction), rook);
			return AddMove(newBoard);
		}

		/// <summary>
		/// Promote a piece. uses SpecialType Variable to determine which move to promote to.
		/// </summary>
		/// <param name="m">Promotion Move.</param>
		/// <returns>true if move worked</returns>
		public bool Promote(Move m)
		{
			Board b = GetPlayableBoard();
			Board newBoard = new Board(b);
			newBoard.SetSquare(m.Origin, (int)Board.Piece.EMPTY);
			newBoard.SetSquare(m.Dest, m.SpecialType);
			return AddMove(newBoard);
		}


		/// <summary>
		/// adds jumping origin move, basically just removes that piece from the board.
		/// </summary>
		/// <param name="m">move to be made.</param>
		/// <param name="moveColor">Color of the player moving.</param> 
		/// <returns>piece that moved. for example if a white knight jumps will return 2</returns>
		public int AddJumpingMove(Move m, bool moveColor)//TODO make this not need color XXX Remove this when no longer needed
		{
			if (moveColor != ColorPlayable)
				return -1;
			CoordFive origin = m.Origin;
			Board b = GetPlayableBoard();
			Board newBoard = new Board(b);
			int piece = newBoard.GetSquare(origin);
			piece = piece < 0 ? piece * -1 : piece;
			m.PieceMoved = piece;
			newBoard.SetSquare(origin, (int)Board.Piece.EMPTY);
			AddMove(newBoard);
			return piece;
		}

		/// <summary>
		/// adds jumping origin move, basically just removes that piece from the board.
		/// </summary>
		/// <param name="m">move to be made.</param>
		/// <returns>piece that moved. for example if a white knight jumps will return 2</returns>
		public int AddJumpingMove(Move m)
		{
			if (m.Origin.Color != ColorPlayable)
				return -1;
			CoordFive origin = m.Origin;
			Board b = GetPlayableBoard();
			Board newBoard = new Board(b);
			int piece = newBoard.GetSquare(origin);
			piece = piece < 0 ? piece * -1 : piece;
			m.PieceMoved = piece;
			newBoard.SetSquare(origin, (int)Board.Piece.EMPTY);
			AddMove(newBoard);
			return piece;
		}

		// add a move jumping, if the move is branching return the branched board,
		// otherwise, add the board onto the end of the timeline. 
		public Board AddJumpingMoveDest(CoordFive dest, bool moveColor, int piece)//TODO make this not use boardcolor XXX Remove this when no longer needed.
		{
			Board b = GetBoard(dest.T, moveColor);
			Board newBoard = new Board(b);
			newBoard.SetSquare(dest, piece);
			if (dest.T != TEnd || moveColor != ColorPlayable)
			{
				return newBoard;
			}
			AddMove(newBoard);
			return null;
		}


		/// <summary>
		/// Adds a non spatial move. In the case it is a branching move, return the board that is created.
		/// </summary>
		/// <param name="dest">destination of the move</param>
		/// <param name="piece">Piece to place at the destination, the "mover"</param>
		/// <returns></returns>
		public Board AddJumpingMoveDest(CoordFive dest, int piece)
		{
			Board b = GetBoard(dest.T, dest.Color);
			Board newBoard = new Board(b);
			newBoard.SetSquare(dest, piece);
			if (dest.T != TEnd || dest.Color != ColorPlayable)
			{
				return newBoard;
			}
			AddMove(newBoard);
			return null;
		}

		/// <summary>
		/// Adds a Null move to the timeline. In other words, the playable board is 'passed'
		/// </summary>
		public void AddNullMove()
		{
			AddMove(GetPlayableBoard());
		}

		/// <summary>
		/// Adds a board to the game, increments WhiteEnd,BlackEnd,TEND when needed.
		/// Updates Color playable.
		/// </summary>
		/// <param name="b">Board to add</param>
		/// <returns>true always (a board was added successfully) </returns>
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
			return true; //CONSIDER whether this is needed.
		}

		/// <summary>
		/// Pops off the last board to undo the last move. Modifies the internal variables.
		/// </summary>
		/// <returns>Returns true if the last board on the timeline was removed, or false if the timeline still should exist</returns>
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

		public string ToString()
		{
			string timelineString = "";
			int lastWIndex = WBoards.Count;
			int lastBIndex = BBoards.Count;
			if (ColorStart)
			{
				for (int i = 0; i < lastBIndex || i < lastWIndex; i++)
				{
					if (i < lastWIndex)
					{
						timelineString += ($"__W_T_{i + TStart}__\n");
						timelineString += WBoards[i].ToString();
						timelineString += "\n";
					}
					if (i < lastBIndex)
					{
						timelineString += ($"__B_T_{i + TStart}__\n");
						timelineString += BBoards[i].ToString();
						timelineString += "\n";
					}
				}
			}
			else
			{
				for (int i = 0; i < lastBIndex || i < lastWIndex; i++)
				{
					if (i < lastBIndex)
					{
						timelineString += ($"__B_T_{i + TStart}__\n");
						timelineString += BBoards[i].ToString();
						timelineString += "\n";
					}
					if (i < lastWIndex)
					{
						timelineString += ($"__W_T_{i + TStart}__\n");
						timelineString += WBoards[i].ToString();
						timelineString += "\n";
					}
				}
			}
			return timelineString;
		}

		/// <summary>
		/// Compares based on timeline, ascending. Layer 0 goes before layer 1
		/// </summary>
		/// <param name="compareTo">Timeline to compare to</param>
		/// <returns>0 if they are the same layer, 1 if the layer of this is larger and -1 if this layer is smaller</returns>
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
