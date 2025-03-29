using System;
using System.Collections.Generic;

namespace FiveDChess 
{
	public class GameState 
	{
	public static readonly bool WHITE = true;
	public static readonly bool BLACK = false;

	public List < Timeline > Multiverse;
	public bool EvenStart;
	public bool Color;
	public int StartPresent;
	public int Present;
	public int Width;
	public int Height;
	public int MaxTL;
	public int MinTL;
	public int MinActiveTL;
	public int MaxActiveTL;

	protected List < Move > TurnMoves;
	protected List < int > TurnTLS;

	public int TLHandicap;

	public GameState(Timeline[] origins, int width, int height, bool evenStart, bool color, int minTL) : 
	this(origins, width, height,evenStart,color,minTL,null){ }

	public GameState(Timeline[] origins, int width, int height, bool evenStart, bool color, int minTL,
					Move[] moves) 
	{
		Multiverse = new List < Timeline > ();
		this.Width = width;
		this.Height = height;
		this.MinTL = minTL;
		this.MaxTL = minTL + origins.Length - 1;
		this.Color = color;
		this.EvenStart = evenStart;
		if (evenStart) 
		{
			TLHandicap = 1;
		}
		else 
		{
			TLHandicap = 0;
		}
		foreach(Timeline t in origins) 
		{
			Multiverse.Add(t);
		}
		if (moves == null) 
		{
			InitVals();
			return;
		}
		//XXX Dont even think this constructor is used...
		foreach(Move m in moves) {
			if(!MakeSilentGenericMove(m))
			{
				throw new Exception("Moves not properly added to gamestate.");
			}
		}
		InitVals();
	}

	private void InitVals() 
	{
	  TurnMoves = new List < Move > ();
	  TurnTLS = new List < int > ();
	  DetermineActiveTLS();
	  CalcPresent();
	  StartPresent = Present;
	}

	public int GetTimelineIndex(int layer)
	{
		return layer + (-1 * MinTL);
	}


	/// <summary>
	/// Gets timeline of index. Does bounds checking.
	/// </summary>
	/// <param name="layer">layer of timeline to get</param>
	/// <returns>timeline of the index layer</returns>
	public Timeline GetTimeline(int layer) 
	{
		int index = layer + (-1 * MinTL);
		if (index < 0 || index >= Multiverse.Count)
		{
			return null;
		}
		return Multiverse[index];
	}

	/// <summary>
	/// Gets board at temporal coordinate c Does Bounds checking.
	/// </summary>
	/// <param name="c">Coordinate to get</param>
	/// <returns>Board which matches coord.</returns>
	public Board GetBoard(CoordFive c)
	{
		if(IsInBounds(c))
		{
			return GetTimeline(c.L).GetBoard(c);
		}
		return null;
	}

	/// <summary>
	/// Returns the piece int at given square
	/// </summary>
	/// <param name="c">Coord of piece to get</param>
	/// <returns>piece int of the square or error square if out of bounds</returns>
	public int GetSquare(CoordFive c) {
		if(IsInBounds(c))
		{
			return GetTimeline(c.L).GetSquare(c);
		}
		return Board.ERRORSQUARE;
	}

	/// <summary>
	/// Determines the active timelines and sets internal variables.
	/// should calculate after every branching move.
	/// </summary>
	public void DetermineActiveTLS() {
		// case 1 -- black has branched more.
		if (MaxTL < Math.Abs(MinTL)) 
		{
			MaxActiveTL = MaxTL;
			MinActiveTL = Math.Max(-1 - MaxActiveTL + TLHandicap, MinTL);
		}
		// case 2 -- white has branched more.
		else if (Math.Abs(MinTL) < MaxTL) 
		{
			MinActiveTL = MinTL;
			MaxActiveTL = Math.Min(1 + Math.Abs(MinActiveTL) + TLHandicap, MaxTL);
		}
		// case 3 -- they have branched the same # of times
		else 
		{
			MinActiveTL = MinTL;
			MaxActiveTL = MaxTL;
		}
	}

	/// <summary>
	/// Calculates the value of the present and sets the present variable in the gameState.
	/// </summary>
	/// <returns>returns the color which the present rests on.</returns>
	public bool CalcPresent() 
	{
		int presentTime = GetTimeline(MinActiveTL).TEnd;
		bool presentColor = GetTimeline(MinActiveTL).ColorPlayable;
		for (int i = MinActiveTL; i <= MaxActiveTL; i++) 
		{
			Timeline t = GetTimeline(i);
			if (t.TEnd < presentTime) 
			{
				presentTime = t.TEnd;
				presentColor = t.ColorPlayable;
			}
			if (t.TEnd == presentTime && !presentColor && t.ColorPlayable == GameState.WHITE) 
			{
				presentTime = t.TEnd;
				presentColor = t.ColorPlayable;
			}
		}
		this.Present = presentTime;
		return presentColor;
	}

	/// <summary>
	/// Checks whether coordinate is in bounds
	/// </summary>
	/// <param name="c">c to check</param>
	/// <returns>true if in bounds otherwise false</returns>
	public bool IsInBounds(CoordFive c) 
	{
		if (c.L > MaxTL || c.L < MinTL) 
		{
			return false;
		}
		if (!Multiverse[c.L + (-1 * MinTL)].TimeExists(c)) 
		{
			return false;
		}
		if (c.X < 0 || c.X >= Width)
		{
			return false;
		}
		if (c.Y < 0 || c.Y >= Height)
		{
			return false;
		}
		return true;
	}

	public bool LayerExists(int layer) 
	{
		return (layer >= MinTL && layer <= MaxTL);
	}

	public bool LayerIsActive(int layer) 
	{
		if (layer < MinActiveTL || layer > MaxActiveTL) 
		{
			return false;
		}
		return true;
	}

	public bool CoordIsPlayable(CoordFive c) 
	{
		if (c == null || !LayerExists(c.L) || c.X < 0 || c.X > this.Width || c.Y < 0 || c.Y > this.Height)
			return false;
		return GetTimeline(c.L).IsMostRecentTime(c.T, c.Color);
	}

	private bool CanActiveBranch() {
		if (Color) 
		{
			return MaxActiveTL <= (TLHandicap - MinActiveTL);
		} 
		else 
		{
			return (MinActiveTL * -1) <= (MaxActiveTL + TLHandicap);
		}
	}

	protected bool ValidateMove(Move m) {
		if (!IsInBounds(m.Origin) || !IsInBounds(m.Dest)) 
		{
			return false;
		}
		Timeline originTL = GetTimeline(m.Origin.L);
		if (originTL.ColorPlayable != this.Color) 
		{
			return false;
		}
		int pieceMoved = this.GetSquare(m.Origin);
		int movedTo = this.GetSquare(m.Dest);
		if (pieceMoved == Board.EMPTYSQUARE || this.Color != Board.GetColorBool(pieceMoved)) 
		{
			return false;
		}
		if (movedTo != Board.EMPTYSQUARE && Board.GetColorBool(movedTo) == this.Color && m.SpecialType == Move.NORMALMOVE) 
		{
			return false;
		}
		if (m.SpecialType != Move.NORMALMOVE) 
		{
			if (m.SpecialType >= Move.PROMOTION) 
			{
				pieceMoved = pieceMoved < 0 ? pieceMoved * -1 : pieceMoved;
				if (m.SpecialType == (int)Board.Piece.WKING || m.SpecialType == (int)Board.Piece.BKING || m.SpecialType > (Board.NUMTYPES * 2)) 
				{
					return false;
				}
				if (pieceMoved == (int)Board.Piece.WPAWN || pieceMoved == (int)Board.Piece.BPAWN ||
					pieceMoved == (int)Board.Piece.WBRAWN || pieceMoved == (int)Board.Piece.BBRAWN) 
				{
					return true;
				} 
				else 
				{
					return false;
				}
			} 
			else if (m.SpecialType == Move.CASTLE) 
			{
				if ((pieceMoved == -1 * (int) Board.Piece.WKING && movedTo == -1 * (int) Board.Piece.WROOK) || (pieceMoved == -1 * (int) Board.Piece.BKING && movedTo == -1 * (int) Board.Piece.BROOK)) 
				{
					return true;
				} 
				else
				{
					return false;
				}
			}
		}
		return MoveGenerator.ValidatePath(this,m,pieceMoved);
	}

	/// <summary>
	/// make a move without submitting turn, or effecting the temp move data
	/// structures. (In other words, it becomes up to the caller to handle undo's and
	/// other features. This will do no validation as it should only be called for
	/// setting up positions.
	/// </summary>
	/// <param name="m">Move to Make</param>
	/// <returns>Bool if the move was made.</returns>
	public bool MakeSilentMove(Move m) 
	{
		if(!IsInBounds(m.Origin) || !IsInBounds(m.Dest))
		{
			return false;
		}
		if(m.Type >= Move.PROMOTION)
		{
			return GetTimeline(m.Origin.L).Promote(m);
		}
		if (m.Type == Move.SPATIALMOVE) 
		{
			int pieceMoved = GetSquare(m.Origin);
			pieceMoved = pieceMoved < 0 ? pieceMoved * -1 : pieceMoved;
			if (pieceMoved == (int) Board.Piece.WKING || pieceMoved == (int) Board.Piece.BKING) 
			{
				if (Math.Abs(m.Origin.X - m.Dest.X) == 2) 
				{
					return this.GetTimeline(m.Origin.L).CastleKing(m);
				}
			}
			bool moveResult = GetTimeline(m.Origin.L).AddSpatialMove(m);
			return moveResult;
		}
		Timeline originT = GetTimeline(m.Origin.L);
		Timeline destT = GetTimeline(m.Dest.L);
		if (!originT.ColorPlayable == this.Color) 
		{
			return false;
		}
		int jumpPieceMoved = originT.AddJumpingMove(m);
		Board b = destT.AddJumpingMoveDest(m.Dest, jumpPieceMoved);
		if (b == null) 
		{
			m.Type = Move.JUMPINGMOVE;
		} 
		else 
		{
			this.AddTimeline(b, m.Dest);
			m.Type = Move.BRANCHINGMOVE;
			DetermineActiveTLS();
		}
		return true;
	}


	/// <summary>
	/// make a move without submitting turn, or effecting the temp move data
	/// structures. (In other words, it becomes up to the caller to handle undo's and
	/// other features. This will do no validation as it should only be called for
	/// setting up positions. This also doesn't care whose turn it is when moving
	/// </summary>
	/// <param name="m">Move to Make</param>
	/// <returns>Bool if the move was made.</returns>
	public bool MakeSilentGenericMove(Move m) 
	{
		if(!IsInBounds(m.Origin) || !IsInBounds(m.Dest))
		{
			return false;
		}
		if(m.Type >= Move.PROMOTION)
		{
			return GetTimeline(m.Origin.L).Promote(m);
		}
		if (m.Type == Move.SPATIALMOVE) 
		{
			int pieceMoved = GetSquare(m.Origin);
			pieceMoved = pieceMoved < 0 ? pieceMoved * -1 : pieceMoved;
			if (pieceMoved == (int) Board.Piece.WKING || pieceMoved == (int) Board.Piece.BKING) 
			{
				if (Math.Abs(m.Origin.X - m.Dest.X) == 2) 
				{
					return this.GetTimeline(m.Origin.L).CastleKing(m);
				}
			}
			bool moveResult = GetTimeline(m.Origin.L).AddSpatialMove(m);
			return moveResult;
		}
		Timeline originT = GetTimeline(m.Origin.L);
		Timeline destT = GetTimeline(m.Dest.L);
		int jumpPieceMoved = originT.AddJumpingMove(m);
		Board b = destT.AddJumpingMoveDest(m.Dest, jumpPieceMoved);
		if (b == null) 
		{
			m.Type = Move.JUMPINGMOVE;
		} 
		else 
		{
			this.AddTimeline(b, m.Dest);
			m.Type = Move.BRANCHINGMOVE;
		}
		return true;
	}


	/// <summary>
	/// Makes a move using MakeSilentMove and then updates the present
	/// Validates the move completely, This move should be used externally.
	/// </summary>
	/// <param name="m">move to make</param>
	/// <returns>true if the move worked, false if it failed.</returns>
	public bool MakeMove(Move m, bool validate = true) {//TODO validate Pawns moving to first or last rank and not promoting.
		if( m == null )
		{
			return false;
		}
		if(validate)
		{
			if(!ValidateMove(m))
			{
				return false;
			}
		}
		if(MakeSilentMove(m))
		{
			TurnMoves.Add(m);
			TurnTLS.Add(m.Origin.L);
			if(m.Type == Move.JUMPINGMOVE)
			{
				TurnTLS.Add(m.Dest.L);
			}
			else if(m.Type == Move.BRANCHINGMOVE)
			{
				int layerAdded = m.Dest.Color ? MaxTL : MinTL;
				TurnTLS.Add(layerAdded);
			}
		}
		CalcPresent();
		return true;
	}

	public bool MakeTurn(Move[] moves) 
	{
		foreach(Move m in moves) 
		{
			MakeMove(m,true);
		}
		if (!SubmitMoves()) 
		{
			UndoTempMoves();
			return false;
		}
		Color = !Color;
		return true;
	}

	public bool SubmitMoves() 
	{
		bool presColor = CalcPresent();
		if( !(presColor == Color) ) //TODO(!OpponentCanCaptureKing() && !(presColor == Color)) 
		{
			TurnTLS.Clear();
			TurnMoves.Clear();
			Color = !Color;
			StartPresent = Present;
			return true;
	  	}
		return false;
	}

	public void UndoTempMoves() 
	{
		if (TurnTLS.Count <= 0)
		{
			return;
		}
		for (int index = TurnTLS.Count - 1; index >= 0; index--) 
		{
			if (GetTimeline(TurnTLS[index]).UndoMove()) 
			{
				// this means that the timeline had only one board.
				Multiverse.RemoveAt(GetTimelineIndex(TurnTLS[index]));
				if (this.Color) 
				{
					MaxTL--;
				} 
				else 
				{
					MinTL++;
				}
			}
		}
		CalcPresent();
		DetermineActiveTLS();
		TurnTLS.Clear();
		TurnMoves.Clear();
	}

	public bool UndoTurn(int[] tlmoved) {
		if (tlmoved.Length == 0)
		{
			return false;
		}
		for (int index = tlmoved.Length - 1; index >= 0; index--) 
		{
			if (GetTimeline(TurnTLS[index]).UndoMove()) 
			{
				// this means that the timeline had only one board.
				Multiverse.RemoveAt(GetTimelineIndex(TurnTLS[index]));
				if (this.Color) 
				{
					MaxTL--;
				} 
				else 
				{
					MinTL++;
				}
			}
		}
		Color = !Color;
		CalcPresent();
		DetermineActiveTLS();
		StartPresent = Present;
		return true;
	}

	public bool UndoTurn(List < int > tlmoved) 
	{
		return UndoTurn(tlmoved.ToArray());
	}

	/// <summary>
	/// Adds a timeline, updated min and maxTL, updates active timelines.
	/// </summary>
	/// <param name="b">Board to add as a new timeline</param>
	/// <param name="c">move destination that led to the timeline</param>
	/// <returns></returns>
	private int AddTimeline(Board b, CoordFive c) 
	{
		if (c.Color) 
		{
			MaxTL++;
			Timeline branch = new Timeline(b, !c.Color, c.T, MaxTL);
			Multiverse.Add(branch);
			return MaxTL;
		} 
		else 
		{
			MinTL--;
			Timeline branch = new Timeline(b, !c.Color, c.T + 1, MaxTL);
			Multiverse.Insert(0, branch);
			return MinTL;
		}
	}


	public string ToString() 
	{
		string temp = "";
	  	temp += "Turn: " + Color.ToString();
		int tl = MinTL;
		foreach(Timeline t in Multiverse) {
			temp += "----------------------TL" + tl.ToString() + "----------------------";
			temp += t.ToString();
			tl++;
		}
		return temp;
	}

/*
	public bool MakeTurn(Move move) 
	{
	  Move[] moves = {
		move
	  };
	  return MakeTurn(moves);
	}

	public bool MakeTurn(Move[] moves) {
	  foreach(Move m in moves) {
		MakeMove(m);
	  }
	  if (!SubmitMoves()) {
		undoTempMoves();
		return false;
	  }
	  Color = !Color;
	  return true;
	}

	/**
	 * make a move without submitting turn, or effecting the temp move data
	 * structures. (In other words, it becomes up to the caller to handle undo's and
	 * other features. This will do no validation as it should only be called for
	 * setting up positions.
	 * 
	 * @param m move to submit
	 * @return boolean whether the move was made or not
	 */
	 /*
	protected bool MakeSilentMove(Move m) {
	  if (m.Type == Move.SPATIALMOVE) {
		int pieceMoved = this.GetSquare(m.Origin, this.Color);
		pieceMoved = pieceMoved < 0 ? pieceMoved * -1 : pieceMoved;
		if (pieceMoved == (int) Board.Piece.WKING || pieceMoved == (int) Board.Piece.BKING) {
		  if (Math.Abs(m.Origin.X - m.Dest.X) == 2) {
			return this.GetTimeline(m.Origin.L).CastleKing(m);
		  }
		}
		bool moveResult = GetTimeline(m.Origin.L).AddSpatialMove(m, Color);
		if (moveResult) {
		  return true;
		} else {
		  return false;
		}
	  }
	  Timeline originT = GetTimeline(m.Origin.L);
	  Timeline destT = GetTimeline(m.Dest.L);
	  if (originT == null || destT == null) {
		return false;
	  }
	  if (!originT.ColorPlayable == this.Color) {
		return false;
	  }
	  int pieceMoved2 = originT.AddJumpingMove(m, Color);
	  Board b = destT.AddJumpingMoveDest(m.Dest, Color, pieceMoved2);
	  if (b == null) {
		m.Type = 2;
	  } else {
		this.AddTimeline(b, m.Dest.T);
		m.Type = 3;
	  }
	  determineActiveTLS();
	  return true;
	}

	public bool MakeMove(Move m) {//TODO validate Pawns moving to first or last rank and not promoting.
	  if (m == null) {
		return false;
	  }
	  if (m.SpecialType == Move.NULLMOVE && this.GetTimeline(m.Origin.L).ColorPlayable == this.Color) {
		GetTimeline(m.Origin.L).AddSpatialMove(m, Color);
		return true;
	  }
	  if (!ValidateMove(m)) {
		return false;
	  }
	  if (m.SpecialType != Move.NORMALMOVE) {
		if (m.SpecialType >= Move.PROMOTION) {
		  if (this.Promote(m)) {
			TurnTLS.Add(m.Dest.L);
			TurnMoves.Add(m);
			return true;
		  } else {
			return false;
		  }
		}
	  }
	  int pieceMoved = this.GetSquare(m.Origin, this.Color);
	  pieceMoved = pieceMoved < 0 ? pieceMoved * -1 : pieceMoved;
	  if (m.Type == Move.SPATIALMOVE) {
		if (pieceMoved == (int) Board.Piece.WKING || pieceMoved == (int) Board.Piece.BKING) {
		  if (Math.Abs(m.Origin.X - m.Dest.X) == 2) {
			TurnTLS.Add(m.Origin.L);
			TurnMoves.Add(m);
			return this.GetTimeline(m.Origin.L).CastleKing(m);
		  }
		}
		bool moveResult = GetTimeline(m.Origin.L).AddSpatialMove(m, Color);
		if (moveResult) {
		  TurnTLS.Add(m.Origin.L);
		  TurnMoves.Add(m);
		  return true;
		} else {
		  return false;
		}
	  }
	  Timeline originT = GetTimeline(m.Origin.L);
	  Timeline destT = GetTimeline(m.Dest.L);
	  originT.AddJumpingMove(m, Color);
	  Board b = destT.AddJumpingMoveDest(m.Dest, Color, pieceMoved);
	  if (b == null) {
		TurnTLS.Add(m.Origin.L);
		TurnTLS.Add(m.Dest.L);
		TurnMoves.Add(m);
		m.Type = Move.JUMPINGMOVE;
	  } else {
		TurnTLS.Add(m.Origin.L);
		TurnTLS.Add(this.AddTimeline(b, m.Dest.T));
		TurnMoves.Add(m);
		m.Type = Move.BRANCHINGMOVE;
	  }
	  determineActiveTLS();
	  calcPresent();
	  return true;
	}

	public bool Promote(Move m) {
	  int promotionType = m.SpecialType;
	  if (m.Type != Move.SPATIALMOVE || m.SpecialType < Move.PROMOTION || (m.Dest.Y != 0 && m.Dest.Y != (this.Height - 1)) || Board.GetColorBool(promotionType) != this.Color || promotionType == 0) {
		return false;
	  }
	  return GetTimeline(m.Origin.L).Promote(m);
	}

	public bool SubmitMoves() {
	  bool presColor = calcPresent();
	  if (!OpponentCanCaptureKing() && !(presColor == Color)) {
		TurnTLS.Clear();
		TurnMoves.Clear();
		Color = !Color;
		StartPresent = Present;
		return true;
	  }
	  return false;
	}

	/**
	 * add a timeline to the multiverse. This assumes that the branch is the color
	 * of the current players turn
	 * 
	 * @param b
	 * @param timeStart the time of the destination of the branch. ie if you jump to
	 *                  time 1 as black, you would input 1 not 2 (2 is the first
	 *                  playable board for white and when the timeline actually
	 *                  starts)
	 * @return integer of the timeline created.
	 */ /*
	private int AddTimeline(Board b, int timeStart) {
	  if (Color) {
		MaxTL++;
		Timeline branch = new Timeline(b, !Color, timeStart, MaxTL);
		Multiverse.Add(branch);
		return MaxTL;
	  } else {
		MinTL--;
		Timeline branch = new Timeline(b, !Color, timeStart + 1, MaxTL);
		Multiverse.Insert(0, branch);
		return MinTL;
	  }
	}

	private bool ValidateTurn(Move[] turn) {
	  List < int > affectedTimelines = new List < int > ();
	  foreach(Move m in turn) {
		if (m == null) {
		  continue;
		}
		if (MakeSilentMove(m)) {
		  if (m.Type == 1) {
			affectedTimelines.Add(m.Origin.L);
		  } else if (m.Type == 2) {
			affectedTimelines.Add(m.Origin.L);
			affectedTimelines.Add(m.Dest.L);
		  } else {
			affectedTimelines.Add(m.Origin.L);
			if (Color) {
			  affectedTimelines.Add(MaxTL);
			} else {
			  affectedTimelines.Add(MinTL);
			}
		  }
		} else {
		  continue;
		}
	  }
	  bool result = false;
	  if (calcPresent() != Color && !OpponentCanCaptureKing()) {
		result = true;
	  }
	  if (this.UndoTurn(affectedTimelines)) {
		Color = !Color;
	  }
	  return result;
	}

	protected bool ValidateMove(Move m) {
	  if (!IsInBounds(m.Origin, this.Color) || !IsInBounds(m.Dest, this.Color)) {
		return false;
	  }
	  Timeline originTL = GetTimeline(m.Origin.L);
	  if (originTL.ColorPlayable != this.Color) {
		return false;
	  }
	  int pieceMoved = this.GetSquare(m.Origin, this.Color);
	  int movedTo = this.GetSquare(m.Dest, this.Color);
	  if (pieceMoved == Board.EMPTYSQUARE || pieceMoved == Board.ERRORSQUARE || this.Color != Board.GetColorBool(pieceMoved)) {
		return false;
	  }
	  if (movedTo != Board.EMPTYSQUARE && Board.GetColorBool(movedTo) == Color && m.SpecialType == Move.NORMALMOVE) {
		return false;
	  }
	  if (m.SpecialType != Move.NORMALMOVE) {
		if (m.SpecialType >= Move.PROMOTION) {
		  pieceMoved = pieceMoved < 0 ? pieceMoved * -1 : pieceMoved;
		  if (m.SpecialType == 7 || m.SpecialType > (Board.numTypes * 2)) {
			return false;
		  }
		  if (pieceMoved == (int) Board.Piece.WPAWN || pieceMoved == (int) Board.Piece.BPAWN ||
			pieceMoved == (int) Board.Piece.WBRAWN || pieceMoved == (int) Board.Piece.BBRAWN) {
			return true;
		  } else {
			return false;
		  }
		} else if (m.SpecialType == Move.CASTLE) {
		  if ((pieceMoved == -1 * (int) Board.Piece.WKING && movedTo == -4) || (pieceMoved == -7 - Board.numTypes && movedTo == -4 - Board.numTypes)) {
			return true;
		  } else {
			return false;
		  }
		}
	  }
	  return true;
	}

	protected bool IsInCheck() {
	  List < int > nullmoves = new List < int > ();
	  for (int i = MinActiveTL; i < MaxActiveTL; i++) {
		Timeline t = GetTimeline(i);
		if (t.ColorPlayable == Color && t.TEnd == StartPresent) {
		  t.AddNullMove();
		  nullmoves.Add(i);
		}
	  }
	  bool inCheck = OpponentCanCaptureKing();
	  foreach(int i in nullmoves) {
		GetTimeline(i).UndoMove();
	  }
	  return inCheck;
	}

	protected bool OpponentCanCaptureKing() {
	  for (int i = MinTL; i <= MaxTL; i++) {
		Timeline t = GetTimeline(i);
		if (t.ColorPlayable != Color) {
		  List < CoordFive > attackingPieces = MoveGenerator.GetCheckingPieces(this,
			new CoordFive(0, 0, t.TEnd, i, !this.Color));
		  if (attackingPieces.Count != 0)
			return true;
		}
	  }
	  return false;
	}

	public bool IsInBounds(CoordFive c, bool boardColor) { //TODO make this not use boardcolor
	  if (c.L > MaxTL || c.L < MinTL) {
		return false;
	  }
	  if (!Multiverse[c.L + (-1 * MinTL)].TimeExists(c.T, boardColor)) {
		return false;
	  }
	  if (c.X < 0 || c.X >= Width)
		return false;
	  if (c.Y < 0 || c.Y >= Height)
		return false;
	  return true;
	}

	public bool CoordIsPlayable(CoordFive c) {
	  if (c == null || !LayerExists(c.L) || c.X < 0 || c.X > this.Width || c.Y < 0 || c.Y > this.Height)
		return false;
	  return GetTimeline(c.L).IsMostRecentTime(c.T, c.Color);
	}

	public bool LayerExists(int layer) {
	  return (layer >= MinTL && layer <= MaxTL);
	}

	public bool LayerIsActive(int layer) {
	  if (layer < MinActiveTL || layer > MaxActiveTL) {
		return false;
	  }
	  return true;
	}

	public Timeline GetTimeline(int layer) {
	  if (!LayerExists(layer)) {
		return null;
	  }
	  return Multiverse[layer + (-1 * MinTL)];
	}

	public Board GetBoard(CoordFive c, bool boardColor) {//TODO make this not use boardcolor
	  if (!LayerExists(c.L)) {
		return null;
	  }
	  return Multiverse[c.L + (-1 * MinTL)].GetBoard(c.T, boardColor);
	}

	public Board GetBoard(CoordFive temporalCoord) {
	  return GetBoard(temporalCoord, temporalCoord.Color);
	}

	public int GetSquare(CoordFive C){
		return GetSquare(C, C.Color);
	}
	public int GetSquare(CoordFive c, bool color) {//TODO make this not use boardcolor
	  if (LayerExists(c.L))
		return GetTimeline(c.L).getSquare(c, color);
	  return Board.ERRORSQUARE;
	}

	public void PrintMultiverse() 
	{
	  Console.WriteLine("Turn: " + Color);
	  int tl = MinTL;
	  foreach(Timeline t in Multiverse) {
		Console.WriteLine("----------------------TL" + tl + "----------------------");
		Console.WriteLine(t.ToString());
		tl++;
	  }
	}

	public bool UndoTurn(int[] tlmoved) {
	  if (tlmoved.Length == 0)
		return false;
	  for (int index = tlmoved.Length - 1; index >= 0; index--) {
		if (GetTimeline(tlmoved[index]).UndoMove()) {
		  Multiverse.RemoveAt(GameState.getTLIndex(tlmoved[index], this.MinTL));
		  if (tlmoved[index] == this.MaxTL) {
			MaxTL--;
		  } else {
			MinTL++;
		  }
		}
	  }
	  determineActiveTLS();
	  Color = !Color;
	  calcPresent();
	  StartPresent = Present;
	  return true;
	}

	public bool UndoTurn(List < int > tlmoved) {
	  if (tlmoved.Count == 0)
		return false;
	  for (int index = tlmoved.Count - 1; index >= 0; index--) {
		if (GetTimeline(tlmoved[index]).UndoMove()) {
		  Multiverse.RemoveAt(GameState.getTLIndex(tlmoved[index], this.MinTL));
		  if (tlmoved[index] == this.MaxTL) {
			MaxTL--;
		  } else {
			MinTL++;
		  }
		}
	  }
	  determineActiveTLS();
	  Color = !Color;
	  calcPresent();
	  StartPresent = Present;
	  return true;
	}

	//------------------------------

	public void undoTempMoves() {
	  if (TurnTLS.Count <= 0)
		return;
	  for (int index = TurnTLS.Count - 1; index >= 0; index--) {
		if (GetTimeline(TurnTLS[index]).UndoMove()) {
		  // this means that the timeline had only one board.
		  Multiverse.RemoveAt(GameState.getTLIndex(TurnTLS[index], this.MinTL));
		  if (Color) {
			MaxTL--;
		  } else {
			MinTL++;
		  }
		}
	  }
	  determineActiveTLS();
	  TurnTLS.Clear();
	  TurnMoves.Clear();
	}

	/**
	 * this function changes the object to reflect which TL are 'active' Uses
	 * 'handicap' for even tl things -- May be wrong but we will see.
	 * 
	 * Should calculate this after every move that is non spatial -- and
	 * therefore all calculations should be working with correct numbers 
	 */ /*
	protected void determineActiveTLS() {
	  // case 1 -- black has branched more.
	  if (MaxTL < Math.Abs(MinTL)) {
		MaxActiveTL = MaxTL;
		MinActiveTL = Math.Max(-1 - MaxActiveTL + TLHandicap, MinTL);
	  }
	  // case 2 -- white has branched more.
	  else if (Math.Abs(MinTL) < MaxTL) {
		MinActiveTL = MinTL;
		MaxActiveTL = Math.Min(1 + Math.Abs(MinActiveTL) + TLHandicap, MaxTL);
	  }
	  // case 3 -- they have branched the same # of times
	  else {
		MinActiveTL = MinTL;
		MaxActiveTL = MaxTL;
	  }
	}


	/// <summary>
	/// Calculates the value of the present and sets the present variable in the gameState.
	/// </summary>
	/// <returns>returns the color which the present rests on.</returns>
	public bool calcPresent() {
	  int presentTime = GetTimeline(MinActiveTL).TEnd;
	  bool presentColor = GetTimeline(MinActiveTL).ColorPlayable;
	  for (int i = MinActiveTL; i <= MaxActiveTL; i++) {
		Timeline t = GetTimeline(i);
		if (t.TEnd < presentTime) {
		  presentTime = t.TEnd;
		  presentColor = t.ColorPlayable;
		}
		if (t.TEnd == presentTime && !presentColor && t.ColorPlayable == GameState.WHITE) {
		  presentTime = t.TEnd;
		  presentColor = t.ColorPlayable;
		}
	  }
	  this.Present = presentTime;
	  return presentColor;
	}

	// return true if a branch will be active, this is for checkmate detection.(ie.
	// if we can branch, check for easy moves that may take us out of check)
	private bool canActiveBranch() {
	  if (Color) {
		return MaxActiveTL <= (TLHandicap - MinActiveTL);
	  } else {
		return (MinActiveTL * -1) <= (MaxActiveTL + TLHandicap);
	  }
	}

	public bool isMated() {
	  determineActiveTLS();
	  calcPresent();
	  if (OpponentCanCaptureKing()) {
		return true;
	  }
	  // Get all the moves from the current place 
	  List < List < Move >> allMoves = new List < List < Move >> ();
	  for (int i = MinTL; i <= MaxTL; i++) {
		List < Move > tlMoves = new List < Move > ();
		Timeline t = GetTimeline(i);
		if (t.ColorPlayable != Color) {
		  continue;
		}
		// the null move marks that the timeline is unmoved. Ie this is the case for
		// certain things.(inactive, future timelines etc.)
		tlMoves.Add(null);
		tlMoves.AddRange(MoveGenerator.GetAllMoves(this, Color, t.TEnd, i));
		allMoves.Add(tlMoves);
	  }
	  //DO a first pass to get rid of any moves that immediatly put you in check
	  //TODO this is too reductive, as its possible a branching move checks frome the origin TL to the branched TL(i think) and therefore, a different order would
	  // allow the move to exist, this is simply supposed to look at single moves (like moving the king into the fire of a bishop etc.)
	  foreach(List < Move > tlMoves in allMoves) {
		int i = 0;
		while (i < tlMoves.Count) {
		  Move m = tlMoves[i];
		  if (m == null) {
			i++;
			continue;
		  }
		  this.MakeMove(m);
		  CoordFive loc = new CoordFive(0, 0, m.Origin.T, m.Origin.L, !this.Color);
		  if (!this.Color) {
			loc.T++;
		  }
		  if (MoveGenerator.GetCheckingPieces(this, loc).Count > 0) {
			//System.out.println("removed move");
			tlMoves.RemoveAt(i);
			i--; //I assume I need this, but maybe not.
		  }
		  this.undoTempMoves();
		  i++;
		}
	  }
	  int[] curMove = new int[allMoves.Count];
	  while (curMove[0] < allMoves[0].Count) {
		Move[] moves = new Move[curMove.Length];
		for (int getM = 0; getM < curMove.Length; getM++) {
		  moves[getM] = allMoves[getM][curMove[getM]];
		}
		if (this.validatePermutationsReduced(moves)) {
		  //if(this.validateAllPermutations(moves)) {
		  //System.out.println(Arrays.toString(moves));
		  return false;
		}
		int cursor = 0;
		while (true) {
		  curMove[cursor]++;
		  if (curMove[cursor] >= allMoves[cursor].Count) {
			curMove[cursor] = 0;
			cursor++;
			if (cursor >= curMove.Length) {
			  return true;
			}
		  } else {
			break;
		  }
		}
	  }
	  return true;
	}

	//This is a copy of the mate detection, but returns a move if exists,
	public Turn findLegalTurn() {
	  determineActiveTLS();
	  calcPresent();
	  //if the opponent can already capture the king, then the player has no legal turns from current position
	  if (OpponentCanCaptureKing()) {
		return null;
	  }
	  // Get all the moves from the current place 
	  List < List < Move >> allMoves = new List < List < Move >> ();
	  for (int i = MinTL; i <= MaxTL; i++) {
		List < Move > tlMoves = new List < Move > ();
		Timeline t = GetTimeline(i);
		if (t.ColorPlayable != Color) {
		  continue;
		}
		// the null move marks that the timeline is unmoved. Ie this is the case for
		// certain things.(inactive, future timelines etc.)
		tlMoves.Add(null);
		tlMoves.AddRange(MoveGenerator.GetAllMoves(this, Color, t.TEnd, i));
		allMoves.Add(tlMoves);
	  }
	  //do a first pass to get rid of any moves that immediatly put you in check
	  foreach (List < Move > tlMoves in allMoves) {
		int i = 0;
		while (i < tlMoves.Count) {
		  Move m = tlMoves[i];
		  if (m == null) {
			i++;
			continue;
		  }
		  this.MakeMove(m);
		  CoordFive loc = new CoordFive(0, 0, m.Origin.T, m.Origin.L, !this.Color);
		  if (!this.Color) {
			loc.T++;
		  }
		  if (MoveGenerator.GetCheckingPieces(this, loc).Count > 0) {
			//System.out.println("removed move");
			tlMoves.RemoveAt(i);
			i--; //I assume I need this, but maybe not.
		  }
		  this.undoTempMoves();
		  i++;
		}
	  }
	  int[] curMove = new int[allMoves.Count];
	  while (curMove[0] < allMoves[0].Count) {
		Move[] moves = new Move[curMove.Length];
		for (int getM = 0; getM < curMove.Length; getM++) {
		  moves[getM] = allMoves[getM][curMove[getM]];
		}
		if (this.validateAllPermutations(moves)) {
		  //System.out.println(Arrays.toString(moves));
		  return new Turn(moves);
		}
		int cursor = 0;
		while (true) {
		  curMove[cursor]++;
		  if (curMove[cursor] >= allMoves[cursor].Count) {
			curMove[cursor] = 0;
			cursor++;
			if (cursor >= curMove.Length) {
			  return null;
			}
		  } else {
			break;
		  }
		}
	  }
	  return null;
	}

	// returns true if the position is checkmate.
	public bool bruteForceMateDetection() {
	  determineActiveTLS();
	  calcPresent();
	  if (OpponentCanCaptureKing()) {
		return true;
	  }
	  // This looks like it should be hackey and bad, i should change it.
	  List < List < Move >> allMoves = new List < List < Move >> ();
	  for (int i = MinTL; i <= MaxTL; i++) {
		List < Move > tlMoves = new List < Move > ();
		Timeline t = GetTimeline(i);
		if (t.ColorPlayable != Color) {
		  continue;
		}
		// the null move marks that the timeline is unmoved. Ie this is the case for
		// certain things.(inactive, future timelines etc.)
		tlMoves.Add(null);
		tlMoves.AddRange(MoveGenerator.GetAllMoves(this, Color, t.TEnd, i));
		allMoves.Add(tlMoves);
	  }
	  int[] curMove = new int[allMoves.Count];
	  while (curMove[0] < allMoves[0].Count) {
		Move[] moves = new Move[curMove.Length];
		for (int getM = 0; getM < curMove.Length; getM++) {
		  moves[getM] = allMoves[getM][curMove[getM]];
		}
		if (this.validateAllPermutations(moves)) {
		  //System.out.println(Arrays.toString(moves));
		  return false;
		}
		int cursor = 0;
		while (true) {
		  curMove[cursor]++;
		  if (curMove[cursor] >= allMoves[cursor].Count) {
			curMove[cursor] = 0;
			cursor++;
			if (cursor >= curMove.Length) {
			  return true;
			}
		  } else {
			break;
		  }
		}
	  }
	  return true;
	}

	// Looks at all permutations of a moveset and validates them. this uses a swap
	// permutation algorithm.
	public bool validateAllPermutations(Move[] moves) {
	  if (ValidateTurn(moves)) {
		return true;
	  }
	  Move[] temp = new Move[moves.Length];
	  int counter = 0;
	  foreach (Move m in moves) {
		temp[counter++] = m;
	  }
	  bool plus = false;
	  int[] c = new int[temp.Length];
	  for (int i = 0; i < temp.Length;) {
		if (c[i] < i) {
		  if (i % 2 == 0) {
			swap(temp, 0, i);

		  } else {
			swap(temp, c[i], i);
		  }
		  if (ValidateTurn(temp)) {
			return true;
		  }
		  plus = !plus;
		  c[i]++;
		  i = 0;
		} else {
		  c[i] = 0;
		  i++;
		}
	  }
	  return false;
	}

	//this function simply reduces the amount of turns validated by not permuting spatial moves in relation to each other
	public bool validatePermutationsReduced(Move[] moves) {
	  if (ValidateTurn(moves)) {
		return true;
	  }
	  List < Move > spatialMoves = new List < Move > ();
	  List < Move > nonSpatialMoves = new List < Move > ();
	  foreach (Move m in moves) {
		if (m == null) {
		  continue;
		} else if (m.Type == Move.SPATIALMOVE) {
		  spatialMoves.Add(m);
		} else {
		  nonSpatialMoves.Add(m);
		}
	  }
	  //The null will be a marker, so that we know where to input the spatial moves.
	  //we permute all nonspatial moves as required to generate all unique turns
	  //this optimization reduces duplicate turns createdd
	  nonSpatialMoves.Add(null);
	  Move[] nonSpatialArray = nonSpatialMoves.ToArray();
	  bool plus = false;
	  int[] c = new int[nonSpatialArray.Length];
	  for (int i = 0; i < nonSpatialArray.Length;) {
		if (c[i] < i) {
		  if (i % 2 == 0) {
			swap(nonSpatialArray, 0, i);

		  } else {
			swap(nonSpatialArray, c[i], i);
		  }
		  if (validatePermutation(nonSpatialArray, spatialMoves.ToArray())) {
			return true;
		  }
		  plus = !plus;
		  c[i]++;
		  i = 0;
		} else {
		  c[i] = 0;
		  i++;
		}
	  }
	  return false;
	}

	public bool validatePermutation(Move[] nonSpatial, Move[] spatial) {
	  Move[] allMoves = new Move[nonSpatial.Length + spatial.Length - 1];
	  int nonspatialIndex = 0;
	  for (int i = 0; i < allMoves.Length; i++) {
		if (nonSpatial[nonspatialIndex] == null) {
		  nonspatialIndex++;
		  foreach(Move m in spatial) {
			allMoves[i++] = m;
		  }
		} else {
		  allMoves[i] = nonSpatial[nonspatialIndex++];
		}
	  }
	  return ValidateTurn(allMoves);
	}

	public static void swap(Object[] array, int index1, int index2) {
	  Object obj = array[index1];
	  array[index1] = array[index2];
	  array[index2] = obj;
	}

	public static int getTLIndex(int layer, int MinTL) {
	  return layer + (-1 * MinTL);
	}
	*/
  }
}
