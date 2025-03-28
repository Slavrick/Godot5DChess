using System;
using System.Collections.Generic;

namespace Engine
{
	public class MoveGenerator
	{
		public static readonly int EMPTYSQUARE = (int)Board.Piece.EMPTY;
		public static readonly int WKING = (int)Board.Piece.WKING;
		public static readonly int BKING = (int)Board.Piece.BKING;
		public static readonly int UNMOVEDROOK = (int)Board.Piece.WROOK * -1;

		public bool CanCaptureSquare(GameState g, bool color, CoordFive origin, CoordFive target, int pieceType)
		{
			bool rider = Board.GetColorBool(pieceType);
			if (pieceType <= 0 || pieceType > (Board.numTypes) * 3)
			{
				return false;
			}
			CoordFive vectorTo = CoordFive.Sub(target, origin);
			// FIXME finish this function.
			return false;
		}

		public static List<CoordFive> GetAllCheckingPieces(GameState g)
		{
			List<CoordFive> attackingPieces = new List<CoordFive>();
			foreach (Timeline t in g.Multiverse)
			{
				if (t.ColorPlayable != g.Color)
				{
					attackingPieces.AddRange(GetCheckingPieces(g, new CoordFive(0, 0, t.TEnd, t.Layer, !g.Color)));
				}
			}
			return attackingPieces;
		}

		public static List<CoordFive> GetCheckingPieces(GameState g, CoordFive spatialCoord)
		{
			List<CoordFive> attackingPieces = new List<CoordFive>();
			Board b = g.GetBoard(spatialCoord);
			if (b == null)
			{
				Console.WriteLine("Error: " + spatialCoord);
				return attackingPieces;
			}
			for (int x = 0; x < g.Width; x++)
			{
				for (int y = 0; y < g.Height; y++)
				{
					int piece = b.GetSquare(x, y);
					if (piece != 0 && Board.GetColorBool(piece) == spatialCoord.Color)
					{
						CoordFive currSquare = new CoordFive(x, y, spatialCoord.T, spatialCoord.L, spatialCoord.Color);
						List<CoordFive> currSquareCaps = GetCaptures(piece, g, currSquare);
						foreach (CoordFive square in currSquareCaps)
						{
							int attackedPiece = g.GetSquare(square, spatialCoord.Color);
							attackedPiece = attackedPiece < 0 ? attackedPiece * -1 : attackedPiece;
							if (MoveNotation.pieceIsRoyal(attackedPiece))
							{
								attackingPieces.Add(currSquare);
							}
						}
					}
				}
			}

			return attackingPieces;
		}

		//for all the check indicator.
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g">gamestate to check</param>
		/// <param name="color">color to check. For example if true(white) you are checking who is checking white</param>
		/// <returns></returns>
		public static List<Move> GetCurrentThreats(GameState g, bool defender){
			List<Move> moves = new List<Move>();
			for(int i = g.MinTL; i <= g.MaxTL; i++){
				Timeline t = g.GetTimeline(i);
				bool NullAdded = false;
				if(t.ColorPlayable == defender){
					//if its the defenders turn check if they need to move.
					if(i < g.MinActiveTL || i > g.MaxActiveTL || t.TEnd > g.Present){
						continue;
					}
					//The defender has to move, so add a 'ghost' board
					t.AddNullMove();
					NullAdded = true;
				}
				Board b = t.GetPlayableBoard();
				for (int x = 0; x < g.Width; x++)
				{
					for (int y = 0; y < g.Height; y++)
					{
						int piece = b.GetSquare(x, y);
						if (Board.GetColorBool(piece) != defender)
						{
							CoordFive srcLocation = new CoordFive(x, y, t.TEnd, i);
							List<CoordFive> captures = GetCaptures(piece, g, new CoordFive(srcLocation, !defender));
							if (captures == null)
							{
								continue;
							}
							foreach (CoordFive dest in captures)
							{
								if(MoveNotation.pieceIsRoyal(g.GetSquare(dest,!defender))){
									moves.Add(new Move(srcLocation, dest));
								}
							}
						}
					}
				}
				if (NullAdded){
					t.UndoMove();
				}
			}
			return moves;
		}

		public static List<Move> GetAllMoves(GameState g, bool color, int T, int L)
		{
			List<Move> moves = new List<Move>();
			Board b = g.GetBoard(new CoordFive(0, 0, T, L, color));
			for (int x = 0; x < g.Width; x++)
			{
				for (int y = 0; y < g.Height; y++)
				{
					int piece = b.GetSquare(x, y);
					if (Board.GetColorBool(piece) == color)
					{
						CoordFive srcLocation = new CoordFive(x, y, T, L);
						List<CoordFive> moveLocations = GetMoves(piece, g, new CoordFive(srcLocation, color));
						if (moveLocations == null)
						{
							continue;
						}
						foreach (CoordFive dest in moveLocations)
						{
							moves.Add(new Move(srcLocation, dest));
						}
					}
				}
			}
			return moves;
		}

		public static List<CoordFive> GetMoves(int piece, GameState g, CoordFive source)
		{
			bool unMoved = false;
			if (piece < 0)
			{
				unMoved = true;
				piece *= -1;
			}
			if (piece == Board.EMPTYSQUARE)
				return null;
			if (piece == (int)Board.Piece.WPAWN || piece == (int)Board.Piece.BPAWN)
			{
				return GetPawnMoves(piece, g, source, unMoved);
			}
			if (piece == (int)Board.Piece.WBRAWN || piece == (int)Board.Piece.BBRAWN)
			{
				List<CoordFive> moves = GetPawnMoves(piece, g, source, unMoved);
				moves.AddRange(GetCaptures(piece, g, source));
				return moves;
			}
			if (piece == WKING || piece == BKING)
			{
				List<CoordFive> moves = new List<CoordFive>();
				if (unMoved)
				{
					CoordFive rookLocq = kingCanCastle(g.GetBoard(source), source, true);
					CoordFive rookLock = kingCanCastle(g.GetBoard(source), source, false);
					if (rookLocq != null)
					{
						moves.Add(rookLocq);
					}
					if (rookLock != null)
					{
						moves.Add(rookLock);
					}
				}
				moves.AddRange(MoveGenerator.GetLeaperMovesAndCaptures(g, source.Color, source, MoveNotation.getMoveVectors(piece)));
				return moves;
			}
			if (MoveNotation.pieceIsRider(piece))
			{
				return MoveGenerator.GetRiderMoves(g, source.Color, source, MoveNotation.getMoveVectors(piece));
			}
			else
			{
				return MoveGenerator.GetLeaperMovesAndCaptures(g, source.Color, source, MoveNotation.getMoveVectors(piece));
			}
		}

		public static List<CoordFive> GetCaptures(int piece, GameState g, CoordFive source)
		{
			if (piece < 0)
			{
				piece *= -1;
			}
			if (piece == (int)Board.Piece.WPAWN)
			{
				return GetLeaperCaptures(g, source.Color, source, MoveNotation.whitePawnAttack);
			}
			if (piece == (int)Board.Piece.BPAWN)
			{
				return GetLeaperCaptures(g, source.Color, source, MoveNotation.blackPawnattack);
			}
			if (piece == (int)Board.Piece.WBRAWN)
			{
				return GetLeaperCaptures(g, source.Color, source, MoveNotation.whiteBrawnattack);
			}
			if (piece == (int)Board.Piece.BBRAWN)
			{
				return GetLeaperCaptures(g, source.Color, source, MoveNotation.blackBrawnattack);
			}
			if (MoveNotation.pieceIsRider(piece))
			{
				return MoveGenerator.GetRiderCaptures(g, source.Color, source, MoveNotation.getMoveVectors(piece));
			}
			else
			{
				return MoveGenerator.GetLeaperCaptures(g, source.Color, source, MoveNotation.getMoveVectors(piece));
			}
		}

		private static List<CoordFive> GetPawnMoves(int piece, GameState g, CoordFive source, bool unmoved)
		{
			List<CoordFive> destCoords = new List<CoordFive>();
			if (unmoved)
			{
				destCoords.AddRange(GetSliderMoves(g, source.Color, source, MoveNotation.getMoveVectors(piece), 2));
			}
			else
			{
				destCoords.AddRange(GetLeaperMoves(g, source.Color, source, MoveNotation.getMoveVectors(piece)));
			}
			CoordFive[] Movementvec;
			if (Board.GetColorBool(piece))
			{
				Movementvec = MoveNotation.whitePawnAttack;
			}
			else
			{
				Movementvec = MoveNotation.blackPawnattack;
			}
			destCoords.AddRange(GetLeaperCaptures(g, source.Color, source, Movementvec));
			if (g.GetBoard(source).EnPassentSquare != null)
			{
				CoordFive enPassent = g.GetBoard(source).EnPassentSquare;
				CoordFive left;
				CoordFive right;
				if (source.Color)
				{
					left = CoordFive.Add(source, MoveNotation.whitePawnAttack[0]);
					right = CoordFive.Add(source, MoveNotation.whitePawnAttack[1]);
				}
				else
				{
					left = CoordFive.Add(source, MoveNotation.blackPawnattack[0]);
					right = CoordFive.Add(source, MoveNotation.blackPawnattack[1]);
				}
				if (left.SpatialEquals(enPassent))
				{
					destCoords.Add(left);
				}
				else if (right.SpatialEquals(enPassent))
				{
					destCoords.Add(right);
				}
			}
			return destCoords;
		}

		public static List<CoordFive> GetLeaperMovesAndCaptures(GameState g, bool color, CoordFive sourceCoord, CoordFive[] movementVec)
		{
			List<CoordFive> destCoords = new List<CoordFive>();
			foreach (CoordFive leap in movementVec)
			{
				int piece = g.GetSquare(CoordFive.Add(sourceCoord, leap), color);
				if (piece == Board.ERRORSQUARE)
				{
					continue;
				}
				if (piece == EMPTYSQUARE)
				{
					destCoords.Add(CoordFive.Add(sourceCoord, leap));
				}
				else if (Board.GetColorBool(piece) != color)
				{
					destCoords.Add(CoordFive.Add(sourceCoord, leap));
				}
			}
			return destCoords;
		}

		public static List<CoordFive> GetLeaperCaptures(GameState g, bool color, CoordFive sourceCoord, CoordFive[] movementVec)
		{
			List<CoordFive> destCoords = new List<CoordFive>();
			foreach (CoordFive leap in movementVec)
			{
				int piece = g.GetSquare(CoordFive.Add(sourceCoord, leap), color);
				if (piece == EMPTYSQUARE || piece == Board.ERRORSQUARE)
				{
					continue;
				}
				else if (Board.GetColorBool(piece) != color)
				{
					destCoords.Add(CoordFive.Add(sourceCoord, leap));
				}
			}
			return destCoords;
		}

		public static List<CoordFive> GetLeaperMoves(GameState g, bool color, CoordFive sourceCoord, CoordFive[] movementVec)
		{
			List<CoordFive> destCoords = new List<CoordFive>();
			foreach (CoordFive leap in movementVec)
			{
				int piece = g.GetSquare(CoordFive.Add(sourceCoord, leap), color);
				if (piece == Board.ERRORSQUARE)
				{
					continue;
				}
				if (piece == EMPTYSQUARE)
				{
					destCoords.Add(CoordFive.Add(sourceCoord, leap));
				}
			}
			return destCoords;
		}

		public static List<CoordFive> GetRiderMoves(GameState g, bool color, CoordFive sourceCoord, CoordFive[] movementVec)
		{
			List<CoordFive> moveList = new List<CoordFive>();
			foreach (CoordFive cf in movementVec)
			{
				if (cf.IsSpatial())
				{
					moveList.AddRange(MoveGenerator.GetSpatialRiderMoves(g, color, sourceCoord, cf));
				}
				else
				{
					moveList.AddRange(MoveGenerator.GetTemporalRiderMoves(g, color, sourceCoord, cf));
				}
			}
			return moveList;
		}

		private static List<CoordFive> GetRiderCaptures(GameState g, bool color, CoordFive source, CoordFive[] moveVectors)
		{
			List<CoordFive> moveList = new List<CoordFive>();
			foreach (CoordFive cf in moveVectors)
			{
				if (cf.IsSpatial())
				{
					CoordFive capture = MoveGenerator.GetSpatialRiderCapture(g, color, source, cf);
					if (capture != null)
					{
						moveList.Add(capture);
					}
				}
				else
				{
					CoordFive capture = MoveGenerator.GetTemporalRiderCaptures(g, color, source, cf);
					if (capture != null)
					{
						moveList.Add(capture);
					}
				}
			}
			return moveList;
		}

		private static CoordFive GetSpatialRiderCapture(GameState g, bool color, CoordFive source, CoordFive movementVec)
		{
			if (!movementVec.IsSpatial())
			{
				return null;
			}
			Board b = g.GetBoard(source, color);
			CoordFive currSquare = CoordFive.Add(source, movementVec);
			while (true)
			{
				int currPiece = b.GetSquare(currSquare);
				if (currPiece == Board.ERRORSQUARE)
				{
					return null;
				}
				if (currPiece != EMPTYSQUARE)
				{
					bool currColor = Board.GetColorBool(currPiece);
					if (currColor == color)
					{
						return null;
					}
					return currSquare;
				}
				currSquare.Add(movementVec);
			}
		}

		private static CoordFive GetTemporalRiderCaptures(GameState g, bool color, CoordFive source, CoordFive movementVec)
		{
			CoordFive currSquare = CoordFive.Add(source, movementVec);
			while (true)
			{
				int currPiece = g.GetSquare(currSquare, color);
				if (currPiece == Board.ERRORSQUARE)
				{
					break;
				}
				if (currPiece != EMPTYSQUARE)
				{
					if (Board.GetColorBool(currPiece) != color)
					{
						return currSquare;
					}
					else
					{
						return null;
					}
				}
				currSquare.Add(movementVec);
			}
			return null;
		}

		public static List<CoordFive>[] GetRiderMovesAndCaps(GameState g, bool color, CoordFive sourceCoord, CoordFive[] movementVec)
		{
			List<CoordFive> moveList = new List<CoordFive>();
			List<CoordFive> capList = new List<CoordFive>();
			foreach (CoordFive cf in movementVec)
			{
				if (cf.IsSpatial())
				{
					List<CoordFive>[] list = MoveGenerator.GetSpatialRiderMovesAndCaptures(g, color, sourceCoord, cf);
					moveList.AddRange(list[0]);
					capList.AddRange(list[1]);
				}
				else
				{
					List<CoordFive>[] list = MoveGenerator.GetTemporalRiderMovesAndCaptures(g, color, sourceCoord, cf);
					moveList.AddRange(list[0]);
					capList.AddRange(list[1]);
				}
			}
			return new List<CoordFive>[] { moveList, capList };
		}

		public static List<CoordFive> GetSpatialRiderMoves(GameState g, bool color, CoordFive sourceCoord, CoordFive movementVec)
		{
			List<CoordFive> destCoords = new List<CoordFive>();
			if (!movementVec.IsSpatial())
			{
				return null;
			}
			Board b = g.GetBoard(sourceCoord, color);
			CoordFive currSquare = CoordFive.Add(sourceCoord, movementVec);
			while (b.IsInBounds(currSquare))
			{
				int currPiece = b.GetSquare(currSquare);
				if (currPiece != EMPTYSQUARE)
				{
					bool currColor = Board.GetColorBool(currPiece);
					if (currColor == color)
					{
						break;
					}
					destCoords.Add(currSquare);
					break;
				}
				destCoords.Add(currSquare.Clone());
				currSquare.Add(movementVec);
			}
			return destCoords;
		}

		public static List<CoordFive>[] GetSpatialRiderMovesAndCaptures(GameState g, bool color, CoordFive sourceCoord, CoordFive movementVec)
		{
			List<CoordFive> destCoords = new List<CoordFive>();
			List<CoordFive> capCoords = new List<CoordFive>();
			if (!movementVec.IsSpatial())
			{
				return null;
			}
			Board b = g.GetBoard(sourceCoord, color);
			CoordFive currSquare = CoordFive.Add(sourceCoord, movementVec);
			while (b.IsInBounds(currSquare))
			{
				int currPiece = b.GetSquare(currSquare);
				if (currPiece != EMPTYSQUARE)
				{
					bool currColor = Board.GetColorBool(currPiece);
					if (currColor == color)
					{
						break;
					}
					destCoords.Add(currSquare.Clone());
					capCoords.Add(currSquare.Clone());
					break;
				}
				destCoords.Add(currSquare.Clone());
				currSquare.Add(movementVec);
			}
			return new List<CoordFive>[] { destCoords, capCoords };
		}

		public static List<CoordFive> GetTemporalRiderMoves(GameState g, bool color, CoordFive sourceCoord, CoordFive movementVec)
		{
			List<CoordFive> destCoords = new List<CoordFive>();
			CoordFive currSquare = CoordFive.Add(sourceCoord, movementVec);
			int currPiece = g.GetSquare(currSquare, color);
			while (currPiece != Board.ERRORSQUARE)
			{
				if (currPiece == EMPTYSQUARE)
				{
					destCoords.Add(currSquare.Clone());
				}
				else
				{
					bool currColor = Board.GetColorBool(currPiece);
					if (currColor == color)
					{
						break;
					}
					destCoords.Add(currSquare);
					break;
				}
				currSquare.Add(movementVec);
				currPiece = g.GetSquare(currSquare, color);
			}
			return destCoords;
		}

		private static List<CoordFive>[] GetTemporalRiderMovesAndCaptures(GameState g, bool color, CoordFive sourceCoord, CoordFive movementVec)
		{
			List<CoordFive> destCoords = new List<CoordFive>();
			List<CoordFive> capCoords = new List<CoordFive>();
			CoordFive currSquare = CoordFive.Add(sourceCoord, movementVec);
			int currPiece = g.GetSquare(currSquare, color);
			while (currPiece != Board.ERRORSQUARE)
			{
				if (currPiece == EMPTYSQUARE)
				{
					destCoords.Add(currSquare.Clone());
				}
				else
				{
					bool currColor = Board.GetColorBool(currPiece);
					if (currColor == color)
					{
						break;
					}
					destCoords.Add(currSquare.Clone());
					capCoords.Add(currSquare.Clone());
					break;
				}
				currSquare.Add(movementVec);
				currPiece = g.GetSquare(currSquare, color);
			}
			return new List<CoordFive>[] { destCoords, capCoords };
		}

		private static List<CoordFive> GetSliderMoves(GameState g, bool color, CoordFive sourceCoord, CoordFive[] movementVec, int range)
		{
			if (range <= 0)
				return null;
			List<CoordFive> destCoords = new List<CoordFive>();
			foreach (CoordFive vec in movementVec)
			{
				CoordFive newsrc = CoordFive.Add(vec, sourceCoord);
				int rangeLeft = range - 1;
				while (rangeLeft >= 0 && g.GetSquare(newsrc, color) == EMPTYSQUARE)
				{
					destCoords.Add(newsrc.Clone());
					rangeLeft--;
					newsrc.Add(vec);
				}
			}
			return destCoords;
		}

//START OF JAVA CODE This stuff was not ported by copilot

	   public static CoordFive kingCanCastle(Board b, CoordFive kingSquare, bool kside) {
		int unmvdRk = UNMOVEDROOK;
		if (!kingSquare.Color) {
			unmvdRk -= Board.numTypes;
		}
		if (kside) {
			// Check For Clearance.
			CoordFive left = new CoordFive(1, 0, 0, 0);
			CoordFive index = CoordFive.Add(kingSquare, left);
			while (b.GetSquare(index) == EMPTYSQUARE) {
				index.Add(left);
			}
			int firstNonEmpty = b.GetSquare(index);
			if (firstNonEmpty != unmvdRk) {
				return null;
			}
			// Check For check
			CoordFive target = kingSquare.Clone();
			if (MoveGenerator.isSquareAttacked(b, target)) {
				return null;
			}
			target.Add(left);
			if (MoveGenerator.isSquareAttacked(b, target)) {
				return null;
			}
			target.Add(left);
			if (MoveGenerator.isSquareAttacked(b, target)) {
				return null;
			}
			return CoordFive.Add(kingSquare, CoordFive.Add(left,left));
		} else {
			// Check For Clearance.
			CoordFive right = new CoordFive(-1, 0, 0, 0);
			CoordFive index = CoordFive.Add(kingSquare, right);
			while (b.GetSquare(index) == EMPTYSQUARE) {
				index.Add(right);
			}
			int firstNonEmpty = b.GetSquare(index);
			if (firstNonEmpty != unmvdRk) {
				return null;
			}
			// Check For check
			CoordFive target = kingSquare.Clone();
			if (MoveGenerator.isSquareAttacked(b, target)) {
				return null;
			}
			target.Add(right);
			if (MoveGenerator.isSquareAttacked(b, target)) {
				return null;
			}
			target.Add(right);
			if (MoveGenerator.isSquareAttacked(b, target)) {
				return null;
			}
			return CoordFive.Add(kingSquare, CoordFive.Add(right,right));
		}
	}
	
	// For now, somewhat counterIntuitively this checks for pieces of opposite CF
	// color,
	//TODO fix this, it doesnt work but look around given square on a queen/knight basis rather than searching like this.
	private static bool isSquareAttacked(Board b, CoordFive target) {
		for (int x = 0; x < b.Width; x++) {
			for (int y = 0; y < b.Height; y++) {
				int piece = b.GetSquare(x, y);
				if (piece != EMPTYSQUARE && Board.GetColorBool(piece) != target.Color) {
					Move attack = new Move(new CoordFive(x, y, target.T, target.L), target);
					if (MoveGenerator.validateSpatialPath(b, piece, attack)) {
						return true;
					}
				}
			}
		}
		return false;
	}

	//TODO not sure this works with pawns/brawns. -- it doesnt.
	private static bool validateSpatialPath(Board b, int piece, Move attack) {
		CoordFive attackVector = CoordFive.Sub(attack.Dest, attack.Origin);
		attackVector.Flatten();
		if (!arrContains(MoveNotation.getMoveVectors(piece), attackVector)) {
			return false;
		}
		CoordFive index = attack.Origin.Clone();
		index.Add(attackVector);
		if(!MoveNotation.pieceIsRider(piece)) {
			if(index.Equals(attack.Dest)) {
				return true;
			}
			else {
				return false;
			}
		}
		while (!index.Equals(attack.Dest)) {
			if (b.GetSquare(index) != EMPTYSQUARE) {
				return false;
			}
			index.Add(attackVector);
		}
		return true;
	}

	//Slated for removal. this is probalby a c# function
	public static bool arrContains(CoordFive[] array, CoordFive target) {
		foreach (CoordFive element in array) {
			if (element.Equals(target)) {
				return true;
			}
		}
		return false;
	}
	
	//this will return the origin square given a coord, used for parsing the moves such as Nf3
	//where you would not know the destination and would have to search for it.
	//this only works for spatial moves, really full coordinates should be entered for anything other than a spatial move.
	public static CoordFive reverseLookup(GameState g, CoordFive destSquare, int pieceType, int rank, int file) {
		Board b = g.GetBoard(destSquare);
		if(b == null) {
			return null;
		}
		if(MoveNotation.pieceIsRider(pieceType)) {
			CoordFive[] moveVecs = MoveNotation.getMoveVectors(pieceType);
			foreach(CoordFive vector in moveVecs) {
				if(vector.IsSpatial()) {
					CoordFive result = destSquare.Clone();
					while(true) {
						result = CoordFive.Sub(result, vector);
						int square = b.GetSquare(result);
						square = square < 0 ? square * -1 : square;
						if(square == EMPTYSQUARE) {
							continue;
						}
						if(square == Board.ERRORSQUARE) {
							break;
						}
						if(square == pieceType) {
							if( (file == -1 || result.X == file) && (rank == -1 || result.Y == rank)) {
								return result;
							}
						}
						break;
					}
				}
			}
		}else {
			CoordFive[] moveVecs = MoveNotation.getMoveVectors(pieceType);
			if(pieceType == (int) Board.Piece.WPAWN) {
				moveVecs = MoveNotation.whitePawnRLkup;
			}else if(pieceType == (int) Board.Piece.WPAWN + Board.numTypes) {
				moveVecs = MoveNotation.blackPawnRLkup;
			}else if(pieceType == (int) Board.Piece.WBRAWN) {
				moveVecs = MoveNotation.whiteBrawnRLkup;
			}else if(pieceType == (int) Board.Piece.WBRAWN + Board.numTypes) {
				moveVecs = MoveNotation.blackBrawnRLkup;
			}
			foreach(CoordFive vector in moveVecs) {
				if(vector.IsSpatial()) {
					CoordFive result = CoordFive.Sub(destSquare, vector);
					if(b.GetSquare(result) == pieceType || b.GetSquare(result) * -1 == pieceType) {
						if( (file == -1 || result.X == file) && (rank == -1 || result.Y == rank)) {
							return result;
						}
					}
				}
			}
		}
		//Parse For castling
		if(pieceType == WKING || pieceType == BKING) {
			CoordFive[] CastleLkup = {
					new CoordFive(2,0,0,0),
					new CoordFive(-2,0,0,0)
			};
			foreach(CoordFive vector in CastleLkup) {
				CoordFive result = CoordFive.Sub(destSquare, vector);
				if(b.GetSquare(result) == pieceType || b.GetSquare(result) * -1 == pieceType) {
					if( (file == -1 || result.X == file) && (rank == -1 || result.Y == rank)) {
							return result;
					}
				}
			}		
		}
		return null;
	}
	
	//search for piece, returns first index of the piece, searching from 0, width 0, height
	public static CoordFive findPiece(Board b, int target) {
		for (int x = 0; x < b.Width; x++) {
			for (int y = 0; y < b.Height; y++) {
				if(b.GetSquare(x,y) == target) {
					return new CoordFive(x,y,0,0);
				}
			}
		}
		return null;
	}
}
}
