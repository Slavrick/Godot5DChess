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

		public bool CanCaptureSquare(GameState g, bool color, CoordFour origin, CoordFour target, int pieceType)
		{
			bool rider = Board.GetColorBool(pieceType);
			if (pieceType <= 0 || pieceType > (Board.numTypes) * 3)
			{
				return false;
			}
			CoordFour vectorTo = CoordFour.Sub(target, origin);
			// FIXME finish this function.
			return false;
		}

		public static List<CoordFour> GetAllCheckingPieces(GameState g)
		{
			List<CoordFour> attackingPieces = new List<CoordFour>();
			foreach (Timeline t in g.Multiverse)
			{
				if (t.ColorPlayable != g.Color)
				{
					attackingPieces.AddRange(GetCheckingPieces(g, new CoordFive(0, 0, t.TEnd, t.Layer, !g.Color)));
				}
			}
			return attackingPieces;
		}

		public static List<CoordFour> GetCheckingPieces(GameState g, CoordFive spatialCoord)
		{
			List<CoordFour> attackingPieces = new List<CoordFour>();
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
					int piece = b.getSquare(x, y);
					if (piece != 0 && Board.GetColorBool(piece) == spatialCoord.Color)
					{
						CoordFive currSquare = new CoordFive(x, y, spatialCoord.T, spatialCoord.L, spatialCoord.Color);
						List<CoordFour> currSquareCaps = GetCaptures(piece, g, currSquare);
						foreach (CoordFour square in currSquareCaps)
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

		public static List<Move> GetAllMoves(GameState g, bool color, int T, int L)
		{
			List<Move> moves = new List<Move>();
			Board b = g.GetBoard(new CoordFive(0, 0, T, L, color));
			for (int x = 0; x < g.Width; x++)
			{
				for (int y = 0; y < g.Height; y++)
				{
					int piece = b.getSquare(x, y);
					if (Board.GetColorBool(piece) == color)
					{
						CoordFour srcLocation = new CoordFour(x, y, T, L);
						List<CoordFour> moveLocations = GetMoves(piece, g, new CoordFive(srcLocation, color));
						if (moveLocations == null)
						{
							continue;
						}
						foreach (CoordFour dest in moveLocations)
						{
							moves.Add(new Move(srcLocation, dest));
						}
					}
				}
			}
			return moves;
		}

		public static List<CoordFour> GetMoves(int piece, GameState g, CoordFive source)
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
				List<CoordFour> moves = GetPawnMoves(piece, g, source, unMoved);
				moves.AddRange(GetCaptures(piece, g, source));
				return moves;
			}
			if (piece == WKING || piece == BKING)
			{
				List<CoordFour> moves = new List<CoordFour>();
				if (unMoved)
				{
					CoordFour rookLocq = kingCanCastle(g.GetBoard(source), source, true);
					CoordFour rookLock = kingCanCastle(g.GetBoard(source), source, false);
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

		public static List<CoordFour> GetCaptures(int piece, GameState g, CoordFive source)
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

		private static List<CoordFour> GetPawnMoves(int piece, GameState g, CoordFive source, bool unmoved)
		{
			List<CoordFour> destCoords = new List<CoordFour>();
			if (unmoved)
			{
				destCoords.AddRange(GetSliderMoves(g, source.Color, source, MoveNotation.getMoveVectors(piece), 2));
			}
			else
			{
				destCoords.AddRange(GetLeaperMoves(g, source.Color, source, MoveNotation.getMoveVectors(piece)));
			}
			CoordFour[] Movementvec;
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
				CoordFour enPassent = g.GetBoard(source).EnPassentSquare;
				CoordFour left;
				CoordFour right;
				if (source.Color)
				{
					left = CoordFour.Add(source, MoveNotation.whitePawnAttack[0]);
					right = CoordFour.Add(source, MoveNotation.whitePawnAttack[1]);
				}
				else
				{
					left = CoordFour.Add(source, MoveNotation.blackPawnattack[0]);
					right = CoordFour.Add(source, MoveNotation.blackPawnattack[1]);
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

		public static List<CoordFour> GetLeaperMovesAndCaptures(GameState g, bool color, CoordFour sourceCoord, CoordFour[] movementVec)
		{
			List<CoordFour> destCoords = new List<CoordFour>();
			foreach (CoordFour leap in movementVec)
			{
				int piece = g.GetSquare(CoordFour.Add(sourceCoord, leap), color);
				if (piece == Board.ERRORSQUARE)
				{
					continue;
				}
				if (piece == EMPTYSQUARE)
				{
					destCoords.Add(CoordFour.Add(sourceCoord, leap));
				}
				else if (Board.GetColorBool(piece) != color)
				{
					destCoords.Add(CoordFour.Add(sourceCoord, leap));
				}
			}
			return destCoords;
		}

		public static List<CoordFour> GetLeaperCaptures(GameState g, bool color, CoordFour sourceCoord, CoordFour[] movementVec)
		{
			List<CoordFour> destCoords = new List<CoordFour>();
			foreach (CoordFour leap in movementVec)
			{
				int piece = g.GetSquare(CoordFour.Add(sourceCoord, leap), color);
				if (piece == EMPTYSQUARE || piece == Board.ERRORSQUARE)
				{
					continue;
				}
				else if (Board.GetColorBool(piece) != color)
				{
					destCoords.Add(CoordFour.Add(sourceCoord, leap));
				}
			}
			return destCoords;
		}

		public static List<CoordFour> GetLeaperMoves(GameState g, bool color, CoordFour sourceCoord, CoordFour[] movementVec)
		{
			List<CoordFour> destCoords = new List<CoordFour>();
			foreach (CoordFour leap in movementVec)
			{
				int piece = g.GetSquare(CoordFour.Add(sourceCoord, leap), color);
				if (piece == Board.ERRORSQUARE)
				{
					continue;
				}
				if (piece == EMPTYSQUARE)
				{
					destCoords.Add(CoordFour.Add(sourceCoord, leap));
				}
			}
			return destCoords;
		}

		public static List<CoordFour> GetRiderMoves(GameState g, bool color, CoordFour sourceCoord, CoordFour[] movementVec)
		{
			List<CoordFour> moveList = new List<CoordFour>();
			foreach (CoordFour cf in movementVec)
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

		private static List<CoordFour> GetRiderCaptures(GameState g, bool color, CoordFive source, CoordFour[] moveVectors)
		{
			List<CoordFour> moveList = new List<CoordFour>();
			foreach (CoordFour cf in moveVectors)
			{
				if (cf.IsSpatial())
				{
					CoordFour capture = MoveGenerator.GetSpatialRiderCapture(g, color, source, cf);
					if (capture != null)
					{
						moveList.Add(capture);
					}
				}
				else
				{
					CoordFour capture = MoveGenerator.GetTemporalRiderCaptures(g, color, source, cf);
					if (capture != null)
					{
						moveList.Add(capture);
					}
				}
			}
			return moveList;
		}

		private static CoordFour GetSpatialRiderCapture(GameState g, bool color, CoordFive source, CoordFour movementVec)
		{
			if (!movementVec.IsSpatial())
			{
				return null;
			}
			Board b = g.GetBoard(source, color);
			CoordFour currSquare = CoordFour.Add(source, movementVec);
			while (true)
			{
				int currPiece = b.getSquare(currSquare);
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

		private static CoordFour GetTemporalRiderCaptures(GameState g, bool color, CoordFive source, CoordFour movementVec)
		{
			CoordFour currSquare = CoordFour.Add(source, movementVec);
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

		public static List<CoordFour>[] GetRiderMovesAndCaps(GameState g, bool color, CoordFour sourceCoord, CoordFour[] movementVec)
		{
			List<CoordFour> moveList = new List<CoordFour>();
			List<CoordFour> capList = new List<CoordFour>();
			foreach (CoordFour cf in movementVec)
			{
				if (cf.IsSpatial())
				{
					List<CoordFour>[] list = MoveGenerator.GetSpatialRiderMovesAndCaptures(g, color, sourceCoord, cf);
					moveList.AddRange(list[0]);
					capList.AddRange(list[1]);
				}
				else
				{
					List<CoordFour>[] list = MoveGenerator.GetTemporalRiderMovesAndCaptures(g, color, sourceCoord, cf);
					moveList.AddRange(list[0]);
					capList.AddRange(list[1]);
				}
			}
			return new List<CoordFour>[] { moveList, capList };
		}

		public static List<CoordFour> GetSpatialRiderMoves(GameState g, bool color, CoordFour sourceCoord, CoordFour movementVec)
		{
			List<CoordFour> destCoords = new List<CoordFour>();
			if (!movementVec.IsSpatial())
			{
				return null;
			}
			Board b = g.GetBoard(sourceCoord, color);
			CoordFour currSquare = CoordFour.Add(sourceCoord, movementVec);
			while (b.IsInBounds(currSquare))
			{
				int currPiece = b.getSquare(currSquare);
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

		public static List<CoordFour>[] GetSpatialRiderMovesAndCaptures(GameState g, bool color, CoordFour sourceCoord, CoordFour movementVec)
		{
			List<CoordFour> destCoords = new List<CoordFour>();
			List<CoordFour> capCoords = new List<CoordFour>();
			if (!movementVec.IsSpatial())
			{
				return null;
			}
			Board b = g.GetBoard(sourceCoord, color);
			CoordFour currSquare = CoordFour.Add(sourceCoord, movementVec);
			while (b.IsInBounds(currSquare))
			{
				int currPiece = b.getSquare(currSquare);
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
			return new List<CoordFour>[] { destCoords, capCoords };
		}

		public static List<CoordFour> GetTemporalRiderMoves(GameState g, bool color, CoordFour sourceCoord, CoordFour movementVec)
		{
			List<CoordFour> destCoords = new List<CoordFour>();
			CoordFour currSquare = CoordFour.Add(sourceCoord, movementVec);
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

		private static List<CoordFour>[] GetTemporalRiderMovesAndCaptures(GameState g, bool color, CoordFour sourceCoord, CoordFour movementVec)
		{
			List<CoordFour> destCoords = new List<CoordFour>();
			List<CoordFour> capCoords = new List<CoordFour>();
			CoordFour currSquare = CoordFour.Add(sourceCoord, movementVec);
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
			return new List<CoordFour>[] { destCoords, capCoords };
		}

		private static List<CoordFour> GetSliderMoves(GameState g, bool color, CoordFour sourceCoord, CoordFour[] movementVec, int range)
		{
			if (range <= 0)
				return null;
			List<CoordFour> destCoords = new List<CoordFour>();
			foreach (CoordFour vec in movementVec)
			{
				CoordFour newsrc = CoordFour.Add(vec, sourceCoord);
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

	   public static CoordFour kingCanCastle(Board b, CoordFive kingSquare, bool kside) {
		int unmvdRk = UNMOVEDROOK;
		if (!kingSquare.Color) {
			unmvdRk -= Board.numTypes;
		}
		if (kside) {
			// Check For Clearance.
			CoordFour left = new CoordFour(1, 0, 0, 0);
			CoordFour index = CoordFour.Add(kingSquare, left);
			while (b.getSquare(index) == EMPTYSQUARE) {
				index.Add(left);
			}
			int firstNonEmpty = b.getSquare(index);
			if (firstNonEmpty != unmvdRk) {
				return null;
			}
			// Check For check
			CoordFive target = kingSquare.clone();
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
			return CoordFour.Add(kingSquare, CoordFour.Add(left,left));
		} else {
			// Check For Clearance.
			CoordFour right = new CoordFour(-1, 0, 0, 0);
			CoordFour index = CoordFour.Add(kingSquare, right);
			while (b.getSquare(index) == EMPTYSQUARE) {
				index.Add(right);
			}
			int firstNonEmpty = b.getSquare(index);
			if (firstNonEmpty != unmvdRk) {
				return null;
			}
			// Check For check
			CoordFive target = kingSquare.clone();
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
			return CoordFour.Add(kingSquare, CoordFour.Add(right,right));
		}
	}
	
	// For now, somewhat counterIntuitively this checks for pieces of opposite CF
	// color,
	//TODO fix this, it doesnt work but look around given square on a queen/knight basis rather than searching like this.
	private static bool isSquareAttacked(Board b, CoordFive target) {
		for (int x = 0; x < b.Width; x++) {
			for (int y = 0; y < b.Height; y++) {
				int piece = b.getSquare(x, y);
				if (piece != EMPTYSQUARE && Board.GetColorBool(piece) != target.Color) {
					Move attack = new Move(new CoordFour(x, y, target.T, target.L), target);
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
		CoordFour attackVector = CoordFour.Sub(attack.Dest, attack.Origin);
		attackVector.Flatten();
		if (!arrContains(MoveNotation.getMoveVectors(piece), attackVector)) {
			return false;
		}
		CoordFour index = attack.Origin.Clone();
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
			if (b.getSquare(index) != EMPTYSQUARE) {
				return false;
			}
			index.Add(attackVector);
		}
		return true;
	}

	public static bool arrContains(CoordFour[] array, CoordFour target) {
		foreach (CoordFour element in array) {
			if (element.Equals(target)) {
				return true;
			}
		}
		return false;
	}
	
	//this will return the origin square given a coord, used for parsing the moves such as Nf3
	//where you would not know the destination and would have to search for it.
	//this only works for spatial moves, really full coordinates should be entered for anything other than a spatial move.
	public static CoordFour reverseLookup(GameState g, CoordFive destSquare, int pieceType, int rank, int file) {
		Board b = g.GetBoard(destSquare);
		if(b == null) {
			return null;
		}
		if(MoveNotation.pieceIsRider(pieceType)) {
			CoordFour[] moveVecs = MoveNotation.getMoveVectors(pieceType);
			foreach(CoordFour vector in moveVecs) {
				if(vector.IsSpatial()) {
					CoordFour result = destSquare.clone();
					while(true) {
						result = CoordFour.Sub(result, vector);
						int square = b.getSquare(result);
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
			CoordFour[] moveVecs = MoveNotation.getMoveVectors(pieceType);
			if(pieceType == (int) Board.Piece.WPAWN) {
				moveVecs = MoveNotation.whitePawnRLkup;
			}else if(pieceType == (int) Board.Piece.WPAWN + Board.numTypes) {
				moveVecs = MoveNotation.blackPawnRLkup;
			}else if(pieceType == (int) Board.Piece.WBRAWN) {
				moveVecs = MoveNotation.whiteBrawnRLkup;
			}else if(pieceType == (int) Board.Piece.WBRAWN + Board.numTypes) {
				moveVecs = MoveNotation.blackBrawnRLkup;
			}
			foreach(CoordFour vector in moveVecs) {
				if(vector.IsSpatial()) {
					CoordFour result = CoordFour.Sub(destSquare, vector);
					if(b.getSquare(result) == pieceType || b.getSquare(result) * -1 == pieceType) {
						if( (file == -1 || result.X == file) && (rank == -1 || result.Y == rank)) {
							return result;
						}
					}
				}
			}
		}
		//Parse For castling
		if(pieceType == WKING || pieceType == BKING) {
			CoordFour[] CastleLkup = {
					new CoordFour(2,0,0,0),
					new CoordFour(-2,0,0,0)
			};
			foreach(CoordFour vector in CastleLkup) {
				CoordFour result = CoordFour.Sub(destSquare, vector);
				if(b.getSquare(result) == pieceType || b.getSquare(result) * -1 == pieceType) {
					if( (file == -1 || result.X == file) && (rank == -1 || result.Y == rank)) {
							return result;
					}
				}
			}		
		}
		return null;
	}
	
	//search for piece, returns first index of the piece, searching from 0, width 0, height
	public static CoordFour findPiece(Board b, int target) {
		for (int x = 0; x < b.Width; x++) {
			for (int y = 0; y < b.Height; y++) {
				if(b.getSquare(x,y) == target) {
					return new CoordFour(x,y,0,0);
				}
			}
		}
		return null;
	}
}
}
