using System;
using System.Collections.Generic;

namespace FiveDChess
{
	public class MoveGenerator
	{
		public static readonly int EMPTYSQUARE = (int)Board.Piece.EMPTY;
		public static readonly int WKING = (int)Board.Piece.WKING;
		public static readonly int BKING = (int)Board.Piece.BKING;
		public static readonly int UNMOVEDROOK = (int)Board.Piece.WROOK * -1;

        public static readonly CoordFive[] NULLMOVESET = { };

        public static readonly CoordFive[] ROOKMOVESET = {
            // spatial
            new CoordFive(1, 0, 0, 0),
            new CoordFive(0, 1, 0, 0),
            new CoordFive(-1, 0, 0, 0),
            new CoordFive(0, -1, 0, 0),
            // temporal
            new CoordFive(0, 0, 0, 1),
            new CoordFive(0, 0, -1, 0),
            new CoordFive(0, 0, 0, -1)
            };

            public static readonly CoordFive[] BISHOPMOVESET = {
                // pure spatial
                new CoordFive(1, 1, 0, 0),
                new CoordFive(1, -1, 0, 0),
                new CoordFive(-1, 1, 0, 0),
                new CoordFive(-1, -1, 0, 0),
                // pure Temporal
                new CoordFive(0, 0, 1, 1),
                new CoordFive(0, 0, 1, -1),
                new CoordFive(0, 0, -1, 1),
                new CoordFive(0, 0, -1, -1),
                // +L
                new CoordFive(1, 0, 0, 1),
                new CoordFive(-1, 0, 0, 1),
                new CoordFive(0, 1, 0, 1),
                new CoordFive(0, -1, 0, 1),
                // -L
                new CoordFive(1, 0, 0, -1),
                new CoordFive(-1, 0, 0, -1),
                new CoordFive(0, 1, 0, -1),
                new CoordFive(0, -1, 0, -1),
                // -T
                new CoordFive(1, 0, -1, 0),
                new CoordFive(-1, 0, -1, 0),
                new CoordFive(0, 1, -1, 0),
                new CoordFive(0, -1, -1, 0)
            };

            public static readonly CoordFive[] UnicornMoveset = {
                new CoordFive(1, 1, 0, 1),
                new CoordFive(1, 0, 1, 1),
                new CoordFive(0, 1, 1, 1),
                new CoordFive(-1, 1, 0, 1),
                new CoordFive(-1, 0, 1, 1),
                new CoordFive(0, -1, 1, 1),
                new CoordFive(1, -1, 0, 1),
                new CoordFive(1, 0, -1, 1),
                new CoordFive(0, 1, -1, 1),
                new CoordFive(1, 1, -1, 0),
                new CoordFive(1, 1, 0, -1),
                new CoordFive(1, 0, 1, -1),
                new CoordFive(0, 1, 1, -1),
                new CoordFive(-1, -1, 0, 1),
                new CoordFive(-1, 0, -1, 1),
                new CoordFive(0, -1, -1, 1),
                new CoordFive(-1, 1, -1, 0),
                new CoordFive(-1, 1, 0, -1),
                new CoordFive(-1, 0, 1, -1),
                new CoordFive(0, -1, 1, -1),
                new CoordFive(1, -1, -1, 0),
                new CoordFive(1, -1, 0, -1),
                new CoordFive(1, 0, -1, -1),
                new CoordFive(0, 1, -1, -1),
                new CoordFive(-1, -1, -1, 0),
                new CoordFive(-1, -1, 0, -1),
                new CoordFive(-1, 0, -1, -1),
                new CoordFive(0, -1, -1, -1)
            };

            public static readonly CoordFive[] DragonMoveset = {
                new CoordFive(1, 1, 1, 1),
                new CoordFive(-1, 1, 1, 1),
                new CoordFive(1, -1, 1, 1),
                new CoordFive(1, 1, -1, 1),
                new CoordFive(1, 1, 1, -1),
                new CoordFive(-1, -1, 1, 1),
                new CoordFive(-1, 1, -1, 1),
                new CoordFive(-1, 1, 1, -1),
                new CoordFive(1, -1, -1, 1),
                new CoordFive(1, -1, 1, -1),
                new CoordFive(1, 1, -1, -1),
                new CoordFive(1, -1, -1, -1),
                new CoordFive(-1, 1, -1, -1),
                new CoordFive(-1, -1, 1, -1),
                new CoordFive(-1, -1, -1, 1),
                new CoordFive(-1, -1, -1, -1)
            };

            public static readonly CoordFive[] KNIGHTMOVESET = {
                // pure Spatial
                new CoordFive(1, 2, 0, 0),
                new CoordFive(2, 1, 0, 0),
                new CoordFive(-1, 2, 0, 0),
                new CoordFive(-2, 1, 0, 0),
                new CoordFive(-2, -1, 0, 0),
                new CoordFive(-1, -2, 0, 0),
                new CoordFive(1, -2, 0, 0),
                new CoordFive(2, -1, 0, 0),
                // pure temporal
                new CoordFive(0, 0, 1, 2),
                new CoordFive(0, 0, 2, 1),
                new CoordFive(0, 0, -1, 2),
                new CoordFive(0, 0, -2, 1),
                new CoordFive(0, 0, -2, -1),
                new CoordFive(0, 0, -1, -2),
                new CoordFive(0, 0, 1, -2),
                new CoordFive(0, 0, 2, -1),
                // Half Spatial/temporal
                // +L
                new CoordFive(2, 0, 0, 1),
                new CoordFive(-2, 0, 0, 1),
                new CoordFive(0, 2, 0, 1),
                new CoordFive(0, -2, 0, 1),
                new CoordFive(1, 0, 0, 2),
                new CoordFive(-1, 0, 0, 2),
                new CoordFive(0, 1, 0, 2),
                new CoordFive(0, -1, 0, 2),
                // -L
                new CoordFive(2, 0, 0, -1),
                new CoordFive(-2, 0, 0, -1),
                new CoordFive(0, 2, 0, -1),
                new CoordFive(0, -2, 0, -1),
                new CoordFive(1, 0, 0, -2),
                new CoordFive(-1, 0, 0, -2),
                new CoordFive(0, 1, 0, -2),
                new CoordFive(0, -1, 0, -2),
                // -T
                new CoordFive(2, 0, -1, 0),
                new CoordFive(-2, 0, -1, 0),
                new CoordFive(0, 2, -1, 0),
                new CoordFive(0, -2, -1, 0),
                new CoordFive(1, 0, -2, 0),
                new CoordFive(-1, 0, -2, 0),
                new CoordFive(0, 1, -2, 0),
                new CoordFive(0, -1, -2, 0)
            };

            public static readonly CoordFive[] PRINCESSMOVESET = {
                // ROOK
                // spatial
                new CoordFive(1, 0, 0, 0),
                new CoordFive(0, 1, 0, 0),
                new CoordFive(-1, 0, 0, 0),
                new CoordFive(0, -1, 0, 0),
                // temporal
                new CoordFive(0, 0, 0, 1),
                new CoordFive(0, 0, -1, 0),
                new CoordFive(0, 0, 0, -1),
                // Bishop
                // pure spatial
                new CoordFive(1, 1, 0, 0),
                new CoordFive(1, -1, 0, 0),
                new CoordFive(-1, 1, 0, 0),
                new CoordFive(-1, -1, 0, 0),
                // pure Temporal
                new CoordFive(0, 0, 1, 1),
                new CoordFive(0, 0, 1, -1),
                new CoordFive(0, 0, -1, 1),
                new CoordFive(0, 0, -1, -1),
                // +L
                new CoordFive(1, 0, 0, 1),
                new CoordFive(-1, 0, 0, 1),
                new CoordFive(0, 1, 0, 1),
                new CoordFive(0, -1, 0, 1),
                // -L
                new CoordFive(1, 0, 0, -1),
                new CoordFive(-1, 0, 0, -1),
                new CoordFive(0, 1, 0, -1),
                new CoordFive(0, -1, 0, -1),
                // -T
                new CoordFive(1, 0, -1, 0),
                new CoordFive(-1, 0, -1, 0),
                new CoordFive(0, 1, -1, 0),
                new CoordFive(0, -1, -1, 0)
            };

            public static readonly CoordFive[] KINGMOVESET = {
                // pure Spatial
                new CoordFive(1, 0, 0, 0),
                new CoordFive(1, 1, 0, 0),
                new CoordFive(1, -1, 0, 0),
                new CoordFive(-1, 0, 0, 0),
                new CoordFive(-1, 1, 0, 0),
                new CoordFive(-1, -1, 0, 0),
                new CoordFive(0, 1, 0, 0),
                new CoordFive(0, -1, 0, 0),
                // +L board
                new CoordFive(1, 0, 0, 1),
                new CoordFive(1, 1, 0, 1),
                new CoordFive(1, -1, 0, 1),
                new CoordFive(-1, 0, 0, 1),
                new CoordFive(-1, 1, 0, 1),
                new CoordFive(-1, -1, 0, 1),
                new CoordFive(0, 1, 0, 1),
                new CoordFive(0, -1, 0, 1),
                new CoordFive(0, 0, 0, 1),
                // -L board
                new CoordFive(1, 0, 0, -1),
                new CoordFive(1, 1, 0, -1),
                new CoordFive(1, -1, 0, -1),
                new CoordFive(-1, 0, 0, -1),
                new CoordFive(-1, 1, 0, -1),
                new CoordFive(-1, -1, 0, -1),
                new CoordFive(0, 1, 0, -1),
                new CoordFive(0, -1, 0, -1),
                new CoordFive(0, 0, 0, -1),
                // -T board
                new CoordFive(1, 0, -1, 0),
                new CoordFive(1, 1, -1, 0),
                new CoordFive(1, -1, -1, 0),
                new CoordFive(-1, 0, -1, 0),
                new CoordFive(-1, 1, -1, 0),
                new CoordFive(-1, -1, -1, 0),
                new CoordFive(0, 1, -1, 0),
                new CoordFive(0, -1, -1, 0),
                new CoordFive(0, 0, -1, 0),
                // -T,-L
                new CoordFive(1, 0, -1, -1),
                new CoordFive(1, 1, -1, -1),
                new CoordFive(1, -1, -1, -1),
                new CoordFive(-1, 0, -1, -1),
                new CoordFive(-1, 1, -1, -1),
                new CoordFive(-1, -1, -1, -1),
                new CoordFive(0, 1, -1, -1),
                new CoordFive(0, -1, -1, -1),
                new CoordFive(0, 0, -1, -1),
                // -T,+L
                new CoordFive(1, 0, -1, 1),
                new CoordFive(1, 1, -1, 1),
                new CoordFive(1, -1, -1, 1),
                new CoordFive(-1, 0, -1, 1),
                new CoordFive(-1, 1, -1, 1),
                new CoordFive(-1, -1, -1, 1),
                new CoordFive(0, 1, -1, 1),
                new CoordFive(0, -1, -1, 1),
                new CoordFive(0, 0, -1, 1),
                // +T,+L
                new CoordFive(1, 0, 1, 1),
                new CoordFive(1, 1, 1, 1),
                new CoordFive(1, -1, 1, 1),
                new CoordFive(-1, 0, 1, 1),
                new CoordFive(-1, 1, 1, 1),
                new CoordFive(-1, -1, 1, 1),
                new CoordFive(0, 1, 1, 1),
                new CoordFive(0, -1, 1, 1),
                new CoordFive(0, 0, 1, 1),
                // +T,-L
                new CoordFive(1, 0, 1, -1),
                new CoordFive(1, 1, 1, -1),
                new CoordFive(1, -1, 1, -1),
                new CoordFive(-1, 0, 1, -1),
                new CoordFive(-1, 1, 1, -1),
                new CoordFive(-1, -1, 1, -1),
                new CoordFive(0, 1, 1, -1),
                new CoordFive(0, -1, 1, -1),
                new CoordFive(0, 0, 1, -1)
            };

            public static readonly CoordFive[] whitePawnMovement = {
                new CoordFive(0, 1, 0, 0),
                new CoordFive(0, 0, 0, -1)
            };

            public static readonly CoordFive[] whitePawnAttack = {
                new CoordFive(1, 1, 0, 0),
                new CoordFive(-1, 1, 0, 0),
                new CoordFive(0, 0, 1, -1),
                new CoordFive(0, 0, -1, -1)
            };

            public static readonly CoordFive[] blackPawnMovement = {
                new CoordFive(0, -1, 0, 0),
                new CoordFive(0, 0, 0, 1)
            };

            public static readonly CoordFive[] blackPawnattack = {
                new CoordFive(1,-1,0,0),
                new CoordFive(-1,-1,0,0),
                new CoordFive(0,0,1,1),
                new CoordFive(0,0,-1,1)
            };

        
        public static readonly CoordFive[] whiteBrawnattack = {
                //Pawn Captures
                new CoordFive(1,1,0,0),
                new CoordFive(-1,1,0,0),
                new CoordFive(0,0,1,-1),
                new CoordFive(0,0,-1,-1),
                //Brawn Specific Captures
                new CoordFive(0,1,-1,0),
                //new CoordFive(0,1,1,0),
                new CoordFive(0,1,0,-1),
                new CoordFive(1,0,0,-1),
                new CoordFive(-1,0,0,-1),
        };
        
        public static readonly CoordFive[] blackBrawnattack = {
                //Pawn Captures
                new CoordFive(1,-1,0,0),
                new CoordFive(-1,-1,0,0),
                new CoordFive(0,0,1,1),
                new CoordFive(0,0,-1,1),
                //Brawn Captures
                new CoordFive(0,-1,-1,0),
                new CoordFive(0,-1,1,0),
                new CoordFive(0,-1,0,1),
                new CoordFive(1,0,0,1),
                new CoordFive(-1,0,0,1),
                
        };
        
        
        //Jank Solution to the fact that pawns and brawns can move in so many ways, for reverse lookup(Searching for a source from a destination)
        //Notice the 1 or two movement. This could fail if there is an inproper configuration, ie. the reverse lookup is destined to fail(this doesnt have any validations)
        //However say 2 pawns are in a line, This code will always find the one in front based of this description(so it should work fine, given that the reverse lookup is
        //non ambiguous and actually exitst)
        public static readonly CoordFive[] whitePawnRLkup = {
                //Pawn Movement
                new CoordFive(0,1,0,0),
                new CoordFive(0,0,0,-1),
                new CoordFive(0,2,0,0),
                new CoordFive(0,0,0,-2),
                //Pawn Attack
                new CoordFive(1,1,0,0),
                new CoordFive(-1,1,0,0),
                new CoordFive(0,0,1,-1),
                new CoordFive(0,0,-1,-1)
        };
        
        public static readonly CoordFive[] blackPawnRLkup = {
                //Pawn Movement
                new CoordFive(0,-1,0,0),
                new CoordFive(0,0,0,1),
                new CoordFive(0,-2,0,0),
                new CoordFive(0,0,0,2),
                //Pawn Attack
                new CoordFive(1,-1,0,0),
                new CoordFive(-1,-1,0,0),
                new CoordFive(0,0,1,1),
                new CoordFive(0,0,-1,1)
        };
        
        public static readonly CoordFive[] whiteBrawnRLkup = {
                //Pawn Movement
                new CoordFive(0,1,0,0),
                new CoordFive(0,0,0,-1),
                new CoordFive(0,2,0,0),
                new CoordFive(0,0,0,-2),
                //Pawn Captures
                new CoordFive(1,1,0,0),
                new CoordFive(-1,1,0,0),
                new CoordFive(0,0,1,-1),
                new CoordFive(0,0,-1,-1),
                //Brawn Specific Captures
                new CoordFive(0,1,-1,0),
                new CoordFive(0,1,1,0),
                new CoordFive(0,1,0,-1),
                new CoordFive(1,0,0,-1),
                new CoordFive(-1,0,0,-1),
        };
        
        public static readonly CoordFive[] blackBrawnRLkup = {
                //Pawn Movement
                new CoordFive(0,-1,0,0),
                new CoordFive(0,0,0,1),
                new CoordFive(0,-2,0,0),
                new CoordFive(0,0,0,2),
                //Pawn Captures
                new CoordFive(1,-1,0,0),
                new CoordFive(-1,-1,0,0),
                new CoordFive(0,0,1,1),
                new CoordFive(0,0,-1,1),
                //Brawn Captures
                new CoordFive(0,-1,-1,0),
                new CoordFive(0,-1,1,0),
                new CoordFive(0,-1,0,1),
                new CoordFive(1,0,0,1),
                new CoordFive(-1,0,0,1),
        };

        //Impossible to avoid magic number antipattern below (or so i think)
        //If I were to do for instantce case board.piece.pawn.ordinal()
        //java complains its no constant :(
        //Perhaps changing this to an if else is something I want to do
        //However, as long as I strictly add new pieces in a predictable pattern, this is fine
        //TODO re add the numpieces. (although i dont imagine ever needing to add in more fairy pieces.)
        /// <summary>
        /// Take a piece and turns it into an array of movement vectors
        /// </summary>
        /// <param name="piece">ordinal integer relating to the piece enum defined in board</param>
        /// <returns>an array of vectors, or empty array if nothing is found.</returns>
        public static CoordFive[] GetMoveVectors(int piece) 
        {
            piece = piece < 0 ? piece * -1 : piece;
            switch(piece) 
            {
            case 1:
                return whitePawnMovement;
            case 13:
                return blackPawnMovement;
            case 2:
            case 14:
                return KNIGHTMOVESET;
            case 3:
            case 15:
                return BISHOPMOVESET;
            case 4:
            case 16:
                return ROOKMOVESET;
            case 5:
            case 17:
                return PRINCESSMOVESET;
            case 6:
            case 18:
                //this case is the queen, but it has the same movement vectors as a king.
                return KINGMOVESET;
            case 7:
            case 19:
                return KINGMOVESET;
            case 8:
            case 20:
                return UnicornMoveset;
            case 9:
            case 21:
                return DragonMoveset;
            case 10:
                return whitePawnMovement;
            case 22:
                return blackPawnMovement;
            case 11:
            case 23://Royal Queen
            case 12:
            case 24://Common King
                return KINGMOVESET;
            case 0:
            default:
                return NULLMOVESET;
            }
        }
        
        /// <summary>
        /// Takes in a piece code and returns whether the piece is a rider.
        /// </summary>
        /// <param name="piece">Piece to check</param>
        /// <returns>returns trus if the piece is a rider and otherwise returns false</returns>
        public static bool PieceIsRider(int piece) 
        {
            piece = piece < 0 ? piece * -1 : piece;
            switch(piece) 
            {
            case 1 + 12://Pawn
            case 1:
            case 2 + 12://Knight
            case 2:
            case 7 + 12://King
            case 7:
            case 10 + 12://Brawn
            case 10:
            case 12 + 12://common king
            case 12:
                return false;
            default: 
                return true;
            }
        }
        
        //Determines who or what piece counts toward checkmate. currently only the king and royal queen but flexable if you want to add more royal pieces.
        /// <summary>
        /// Takes in a piece and returns whether it is royal. Any Royal piece counts toward checkmate. Technically for example can change this to any piece being royal.
        /// </summary>
        /// <param name="piece">piece to check whether not it is considered royal</param>
        /// <returns>boolean, true if royal false otherwise</returns>
        public static bool PieceIsRoyal(int piece) 
        { //TODO replace this with Board.Piece for clarity.
            piece = piece < 0 ? piece * -1 : piece;
            switch(piece) 
            {
            case (int)Board.Piece.WKING + 12://King
            case 7:
            case 11 + 12://Royal Queen
            case 11:
                return true;
            default: 
                return false;
            }
        }

        public bool CanCaptureSquare(GameState g, bool color, CoordFive origin, CoordFive target, int pieceType)
		{
			bool rider = Board.GetColorBool(pieceType);
			if (pieceType <= 0 || pieceType > (Board.NUMTYPES) * 3)
			{
				return false;
			}
			CoordFive vectorTo = CoordFive.Sub(target, origin);
			// TODO finish this function.
			return false;
		}

        public static bool ValidatePath(GameState g, Move m, int piece)
        {
            return true; //TODO This needs to be completed.
        }

        //TODO not sure this works with pawns/brawns. -- it doesnt.
        private static bool ValidateSpatialPath(Board b, int piece, Move attack) 
        {
            CoordFive attackVector = CoordFive.Sub(attack.Dest, attack.Origin);
            attackVector.Flatten();
            if (!arrContains(GetMoveVectors(piece), attackVector)) 
            {
                return false;
            }
            CoordFive index = attack.Origin.Clone();
            index.Add(attackVector);
            if(!PieceIsRider(piece)) {
                if(index.Equals(attack.Dest)) {
                    return true;
                }
                else {
                    return false;
                }
            }
            while (!index.Equals(attack.Dest)) {
                if (b.GetSquareSafe(index) != EMPTYSQUARE) {
                    return false;
                }
                index.Add(attackVector);
            }//TODO validate if you didn't capture your own piece.
            return true;
        }

        public static List<CoordFive> GetMoves(int piece, GameState g, CoordFive source)
		{
			bool unMoved = false;
			if (piece < 0)
			{
				unMoved = true;
				piece *= -1;
			}
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
					CoordFive rookLocq = KingCanCastle(g.GetBoard(source), source, true);
					CoordFive rookLock = KingCanCastle(g.GetBoard(source), source, false);
					if (rookLocq != null)
					{
						moves.Add(rookLocq);
					}
					if (rookLock != null)
					{
						moves.Add(rookLock);
					}
				}
				moves.AddRange(GetLeaperMovesAndCaptures(g, source, GetMoveVectors(piece)));
				return moves;
			}
			if (PieceIsRider(piece))
			{
				return GetRiderMoves(g, source, GetMoveVectors(piece));
			}
			else
			{
				return GetLeaperMovesAndCaptures(g, source, GetMoveVectors(piece));
			}
		}

        public static List<CoordFive> GetRiderMoves(GameState g, CoordFive sourceCoord, CoordFive[] movementVec)
		{
			List<CoordFive> moveList = new List<CoordFive>();
			foreach (CoordFive cf in movementVec)
			{
				if (cf.IsSpatial())
				{
					moveList.AddRange(GetSpatialRiderMoves(g, sourceCoord, cf));
				}
				else
				{
					moveList.AddRange(GetTemporalRiderMoves(g, sourceCoord, cf));
				}
			}
			return moveList;
		}

        public static List<CoordFive> GetTemporalRiderMoves(GameState g, CoordFive sourceCoord, CoordFive movementVec)
		{
			List<CoordFive> destCoords = new List<CoordFive>();
			CoordFive currSquare = CoordFive.Add(sourceCoord, movementVec);
			int currPiece = g.GetSquare(currSquare);
			while (currPiece != Board.ERRORSQUARE)
			{
				if (currPiece == EMPTYSQUARE)
				{
					destCoords.Add(currSquare.Clone());
				}
				else
				{
					bool currColor = Board.GetColorBool(currPiece);
					if (currColor == sourceCoord.Color)
					{
						break;
					}
					destCoords.Add(currSquare);
					break;
				}
				currSquare.Add(movementVec);
				currPiece = g.GetSquare(currSquare);
			}
			return destCoords;
		}

        public static List<CoordFive> GetSpatialRiderMoves(GameState g, CoordFive sourceCoord, CoordFive movementVec)//TODO make this say something like GenerateRideMovesSpatial
		{
			List<CoordFive> destCoords = new List<CoordFive>();
			if (!movementVec.IsSpatial())
			{
				return null;
			}
			Board b = g.GetBoard(sourceCoord);
			CoordFive currSquare = CoordFive.Add(sourceCoord, movementVec);
			while (b.IsInBounds(currSquare))
			{
				int currPiece = b.GetSquare(currSquare);
				if (currPiece != EMPTYSQUARE)
				{
					bool currColor = Board.GetColorBool(currPiece);
					if (currColor == sourceCoord.Color)
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

        public static List<CoordFive> GetLeaperMovesAndCaptures(GameState g, CoordFive sourceCoord, CoordFive[] movementVec)
		{
			List<CoordFive> destCoords = new List<CoordFive>();
			foreach (CoordFive leap in movementVec)
			{
                CoordFive leapDestination = CoordFive.Add(sourceCoord, leap);
				int piece = g.GetSquare(leapDestination);
				if (piece == Board.ERRORSQUARE)
				{
					continue;
				}
				if (piece == EMPTYSQUARE)
				{
					destCoords.Add(leapDestination);
				}
				else if (Board.GetColorBool(piece) != sourceCoord.Color)
				{
					destCoords.Add(leapDestination);
				}
			}
			return destCoords;
		}

        public static List<CoordFive> GetLeaperCaptures(GameState g, CoordFive sourceCoord, CoordFive[] movementVec)
		{
			List<CoordFive> destCoords = new List<CoordFive>();
			foreach (CoordFive leap in movementVec)
			{
				int piece = g.GetSquare(CoordFive.Add(sourceCoord, leap));//TODO cache this result.
				if (piece == EMPTYSQUARE || piece == Board.ERRORSQUARE)
				{
					continue;
				}
				else if (Board.GetColorBool(piece) != sourceCoord.Color)
				{
					destCoords.Add(CoordFive.Add(sourceCoord, leap));
				}
			}
			return destCoords;
		}

		public static List<CoordFive> GetLeaperMoves(GameState g, CoordFive sourceCoord, CoordFive[] movementVec)
		{
			List<CoordFive> destCoords = new List<CoordFive>();
			foreach (CoordFive leap in movementVec)
			{
				int piece = g.GetSquare(CoordFive.Add(sourceCoord, leap));//TODO Cache Result
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

        private static List<CoordFive> GetPawnMoves(int piece, GameState g, CoordFive source, bool unmoved)
		{
			List<CoordFive> destCoords = new List<CoordFive>();
			if (unmoved)
			{
				destCoords.AddRange(GetSliderMoves(g, source, GetMoveVectors(piece), 2));
			}
			else
			{
				destCoords.AddRange(GetLeaperMoves(g, source, GetMoveVectors(piece)));
			}
			CoordFive[] Movementvec;
			if (Board.GetColorBool(piece))
			{
				Movementvec = whitePawnAttack;
			}
			else
			{
				Movementvec = blackPawnattack;
			}
			destCoords.AddRange(GetLeaperCaptures(g, source, Movementvec));
			if (g.GetBoard(source).EnPassentSquare != null)
			{
				CoordFive enPassent = g.GetBoard(source).EnPassentSquare;
				CoordFive left;
				CoordFive right;
				if (source.Color)
				{
					left = CoordFive.Add(source, whitePawnAttack[0]);
					right = CoordFive.Add(source, whitePawnAttack[1]);
				}
				else
				{
					left = CoordFive.Add(source, blackPawnattack[0]);
					right = CoordFive.Add(source, blackPawnattack[1]);
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

        private static List<CoordFive> GetSliderMoves(GameState g, CoordFive sourceCoord, CoordFive[] movementVec, int range)
		{
			if (range <= 0)
            {
				return null;
            }
			List<CoordFive> destCoords = new List<CoordFive>();
			foreach (CoordFive vec in movementVec)
			{
				CoordFive newsrc = CoordFive.Add(sourceCoord, vec);
				int rangeLeft = range - 1;
				while (rangeLeft >= 0 && g.GetSquare(newsrc) == EMPTYSQUARE)
				{
					destCoords.Add(newsrc.Clone());
					rangeLeft--;
					newsrc.Add(vec);
				}
			}
			return destCoords;
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
					int piece = b.GetSquareSafe(x, y);
					if (piece != 0 && Board.GetColorBool(piece) == spatialCoord.Color)
					{
						CoordFive currSquare = new CoordFive(x, y, spatialCoord.T, spatialCoord.L, spatialCoord.Color);
						List<CoordFive> currSquareCaps = GetCaptures(piece, g, currSquare);
						foreach (CoordFive square in currSquareCaps)
						{
							int attackedPiece = g.GetSquare(square);
							attackedPiece = attackedPiece < 0 ? attackedPiece * -1 : attackedPiece;
							if (PieceIsRoyal(attackedPiece))
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
		public static List<Move> GetCurrentThreats(GameState g, bool defender)
        {
			List<Move> moves = new List<Move>();
			for(int i = g.MinTL; i <= g.MaxTL; i++)
            {
				Timeline t = g.GetTimeline(i);
				bool NullAdded = false;
				if(t.ColorPlayable == defender)
                {
					//if its the defenders turn check if they need to move.
					if(i < g.MinActiveTL || i > g.MaxActiveTL || t.TEnd > g.Present)
                    {
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
						int piece = b.GetSquareSafe(x, y);
						if (Board.GetColorBool(piece) != defender)
						{
							CoordFive srcLocation = new CoordFive(x, y, t.TEnd, i,!defender);
							List<CoordFive> captures = GetCaptures(piece, g, srcLocation);
							if (captures == null)
							{
								continue;
							}
							foreach (CoordFive dest in captures)
							{
								if(PieceIsRoyal(g.GetSquare(dest)))
                                {
									moves.Add(new Move(srcLocation.Clone(), dest));
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

        //TODO might want to get oponents moves, currently it only gets the moves of the player whose move it is for that board.
		public static List<Move> GetAllMoves(GameState g, CoordFive boardCoord)
		{
			List<Move> moves = new List<Move>();
			Board b = g.GetBoard(boardCoord);
			for (int x = 0; x < g.Width; x++)
			{
				for (int y = 0; y < g.Height; y++)
				{
					int piece = b.GetSquareSafe(x, y);
					if (Board.GetColorBool(piece) == boardCoord.Color)
					{
						CoordFive srcLocation = new CoordFive(x, y, boardCoord.T, boardCoord.L ,boardCoord.Color);
						List<CoordFive> moveLocations = GetMoves(piece, g, srcLocation.Clone());
						if (moveLocations == null)
						{
							continue;
						}
						foreach (CoordFive dest in moveLocations)
						{
							moves.Add(new Move(srcLocation.Clone(), dest));
						}
					}
				}
			}
			return moves;
		}

	

		public static List<CoordFive> GetCaptures(int piece, GameState g, CoordFive source)
		{
			if (piece < 0)
			{
				piece *= -1;
			}
			if (piece == (int)Board.Piece.WPAWN)
			{
				return GetLeaperCaptures(g, source, whitePawnAttack);
			}
			if (piece == (int)Board.Piece.BPAWN)
			{
				return GetLeaperCaptures(g, source, blackPawnattack);
			}
			if (piece == (int)Board.Piece.WBRAWN)
			{
				return GetLeaperCaptures(g, source, whiteBrawnattack);
			}
			if (piece == (int)Board.Piece.BBRAWN)
			{
				return GetLeaperCaptures(g, source, blackBrawnattack);
			}
			if (PieceIsRider(piece))
			{
				return GetRiderCaptures(g, source, GetMoveVectors(piece));
			}
			else
			{
				return GetLeaperCaptures(g, source, GetMoveVectors(piece));
			}
		}

		private static List<CoordFive> GetRiderCaptures(GameState g, CoordFive source, CoordFive[] moveVectors)
		{
			List<CoordFive> moveList = new List<CoordFive>();
			foreach (CoordFive cf in moveVectors)
			{
				if (cf.IsSpatial())
				{
					CoordFive capture = GetSpatialRiderCapture(g, source, cf);
					if (capture != null)
					{
						moveList.Add(capture);
					}
				}
				else
				{
					CoordFive capture = GetTemporalRiderCaptures(g, source, cf);
					if (capture != null)
					{
						moveList.Add(capture);
					}
				}
			}
			return moveList;
		}

		private static CoordFive GetSpatialRiderCapture(GameState g, CoordFive source, CoordFive movementVec)
		{
			if (!movementVec.IsSpatial())
			{
				return null;
			}
			Board b = g.GetBoard(source);
			CoordFive currSquare = CoordFive.Add(source, movementVec);
			while (true)
			{
				int currPiece = b.GetSquareSafe(currSquare);
				if (currPiece == Board.ERRORSQUARE)
				{
					return null;
				}
				if (currPiece != EMPTYSQUARE)
				{
					bool currColor = Board.GetColorBool(currPiece);
					if (currColor == source.Color)
					{
						return null;
					}
					return currSquare;
				}
				currSquare.Add(movementVec);
			}
		}

		private static CoordFive GetTemporalRiderCaptures(GameState g, CoordFive source, CoordFive movementVec)
		{
			CoordFive currSquare = CoordFive.Add(source, movementVec);
			while (true)
			{
				int currPiece = g.GetSquare(currSquare);
				if (currPiece == Board.ERRORSQUARE)
				{
					break;
				}
				if (currPiece != EMPTYSQUARE)
				{
					if (Board.GetColorBool(currPiece) != source.Color)
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

		public static List<CoordFive>[] GetRiderMovesAndCaps(GameState g, CoordFive sourceCoord, CoordFive[] movementVec)
		{
			List<CoordFive> moveList = new List<CoordFive>();
			List<CoordFive> capList = new List<CoordFive>();
			foreach (CoordFive cf in movementVec)
			{
				if (cf.IsSpatial())
				{
					List<CoordFive>[] list = GetSpatialRiderMovesAndCaptures(g, sourceCoord, cf);
					moveList.AddRange(list[0]);
					capList.AddRange(list[1]);
				}
				else
				{
					List<CoordFive>[] list = MoveGenerator.GetTemporalRiderMovesAndCaptures(g, sourceCoord, cf);
					moveList.AddRange(list[0]);
					capList.AddRange(list[1]);
				}
			}
			return new List<CoordFive>[] { moveList, capList };
		}

		public static List<CoordFive>[] GetSpatialRiderMovesAndCaptures(GameState g, CoordFive sourceCoord, CoordFive movementVec)
		{
			List<CoordFive> destCoords = new List<CoordFive>();
			List<CoordFive> capCoords = new List<CoordFive>();
			if (!movementVec.IsSpatial())
			{
				return null;
			}
			Board b = g.GetBoard(sourceCoord);
			CoordFive currSquare = CoordFive.Add(sourceCoord, movementVec);
			while (b.IsInBounds(currSquare))
			{
				int currPiece = b.GetSquare(currSquare);
				if (currPiece != EMPTYSQUARE)
				{
					bool currColor = Board.GetColorBool(currPiece);
					if (currColor == sourceCoord.Color)
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
 
		private static List<CoordFive>[] GetTemporalRiderMovesAndCaptures(GameState g, CoordFive sourceCoord, CoordFive movementVec)
		{
			List<CoordFive> destCoords = new List<CoordFive>();
			List<CoordFive> capCoords = new List<CoordFive>();
			CoordFive currSquare = CoordFive.Add(sourceCoord, movementVec);
			int currPiece = g.GetSquare(currSquare);
			while (currPiece != Board.ERRORSQUARE)
			{
				if (currPiece == EMPTYSQUARE)
				{
					destCoords.Add(currSquare.Clone());
				}
				else
				{
					bool currColor = Board.GetColorBool(currPiece);
					if (currColor == sourceCoord.Color)
					{
						break;
					}
					destCoords.Add(currSquare.Clone());
					capCoords.Add(currSquare.Clone());
					break;
				}
				currSquare.Add(movementVec);
				currPiece = g.GetSquare(currSquare);
			}
			return new List<CoordFive>[] { destCoords, capCoords };
		}

        //TODO Doesnt check if you can castle through check temporally not sure if the base game does.
        public static CoordFive KingCanCastle(Board b, CoordFive kingSquare, bool kside) 
        {
            int unmvdRk = UNMOVEDROOK;
            if (!kingSquare.Color) 
            {
                unmvdRk -= Board.NUMTYPES;
            }
            if (kside) 
            {
                // Check For Clearance.
                CoordFive left = new CoordFive(1, 0, 0, 0);
                CoordFive index = CoordFive.Add(kingSquare, left);
                while (b.GetSquareSafe(index) == EMPTYSQUARE) 
                {
                    index.Add(left);
                }
                int firstNonEmpty = b.GetSquareSafe(index);
                if (firstNonEmpty != unmvdRk) 
                {
                    return null;
                }
                // Check For check
                CoordFive target = kingSquare.Clone();
                if (IsSquareAttacked(b, target)) 
                {
                    return null;
                }
                target.Add(left);
                if (IsSquareAttacked(b, target)) 
                {
                    return null;
                }
                target.Add(left);
                if (IsSquareAttacked(b, target))
                {
                    return null;
                }
                return CoordFive.Add(kingSquare, CoordFive.Add(left,left));
            } 
            else 
            {
                // Check For Clearance.
                CoordFive right = new CoordFive(-1, 0, 0, 0);
                CoordFive index = CoordFive.Add(kingSquare, right);
                while (b.GetSquareSafe(index) == EMPTYSQUARE) 
                {
                    index.Add(right);
                }
                int firstNonEmpty = b.GetSquareSafe(index);
                if (firstNonEmpty != unmvdRk) 
                {
                    return null;
                }
                // Check For check
                CoordFive target = kingSquare.Clone();
                if (IsSquareAttacked(b, target)) 
                {
                    return null;
                }
                target.Add(right);
                if (IsSquareAttacked(b, target)) 
                {
                    return null;
                }
                target.Add(right);
                if (IsSquareAttacked(b, target)) 
                {
                    return null;
                }
                return CoordFive.Add(kingSquare, CoordFive.Add(right,right));
            }
        }
      
        // For now, somewhat counterIntuitively this checks for pieces of opposite CF
        // color,
        //TODO fix this, it doesnt work but look around given square on a queen/knight basis rather than searching like this.
        private static bool IsSquareAttacked(Board b, CoordFive target) 
        {
            for (int x = 0; x < b.Width; x++) 
            {
                for (int y = 0; y < b.Height; y++) 
                {
                    int piece = b.GetSquare(x, y);
                    if (piece != EMPTYSQUARE && Board.GetColorBool(piece) != target.Color) 
                    {
                        Move attack = new Move(new CoordFive(x, y, target.T, target.L), target);
                        if (ValidateSpatialPath(b, piece, attack)) 
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }


        
        //this will return the origin square given a coord, used for parsing the moves such as Nf3
        //where you would not know the destination and would have to search for it.
        //this only works for spatial moves, really full coordinates should be entered for anything other than a spatial move.
        public static CoordFive ReverseLookup(GameState g, CoordFive destSquare, int pieceType, int rank, int file) {
            Board b = g.GetBoard(destSquare);
            if(b == null) {
                return null;
            }
            if(PieceIsRider(pieceType)) 
            {
                CoordFive[] moveVecs = GetMoveVectors(pieceType);
                foreach(CoordFive vector in moveVecs) 
                {
                    if(vector.IsSpatial()) 
                    {
                        CoordFive result = destSquare.Clone();
                        while(true) 
                        {
                            result = CoordFive.Sub(result, vector);
                            int square = b.GetSquareSafe(result);
                            square = square < 0 ? square * -1 : square;
                            if(square == EMPTYSQUARE) 
                            {
                                continue;
                            }
                            if(square == Board.ERRORSQUARE) 
                            {
                                break;
                            }
                            if(square == pieceType) 
                            {
                                if( (file == -1 || result.X == file) && (rank == -1 || result.Y == rank)) 
                                {
                                    return result;
                                }
                            }
                            break;
                        }
                    }
                }
            }else {
                CoordFive[] moveVecs = GetMoveVectors(pieceType);
                if(pieceType == (int) Board.Piece.WPAWN) 
                {
                    moveVecs = whitePawnRLkup;
                }
                else if(pieceType == (int) Board.Piece.WPAWN + Board.NUMTYPES) 
                {
                    moveVecs = blackPawnRLkup;
                }
                else if(pieceType == (int) Board.Piece.WBRAWN) 
                {
                    moveVecs = whiteBrawnRLkup;
                }
                else if(pieceType == (int) Board.Piece.WBRAWN + Board.NUMTYPES) 
                {
                    moveVecs = blackBrawnRLkup;
                }
                foreach(CoordFive vector in moveVecs) 
                {
                    if(vector.IsSpatial()) 
                    {
                        CoordFive result = CoordFive.Sub(destSquare, vector);
                        if(b.GetSquareSafe(result) == pieceType || b.GetSquareSafe(result) * -1 == pieceType) 
                        {
                            if( (file == -1 || result.X == file) && (rank == -1 || result.Y == rank)) 
                            {
                                return result;
                            }
                        }
                    }
                }
            }
            //Parse For castling
            if(pieceType == WKING || pieceType == BKING) 
            {
                CoordFive[] CastleLkup = {
                        new CoordFive(2,0,0,0),
                        new CoordFive(-2,0,0,0)
                };
                foreach(CoordFive vector in CastleLkup) 
                {
                    CoordFive result = CoordFive.Sub(destSquare, vector);
                    if(b.GetSquareSafe(result) == pieceType || b.GetSquareSafe(result) * -1 == pieceType) 
                    {
                        if( (file == -1 || result.X == file) && (rank == -1 || result.Y == rank)) 
                        {
                            return result;
                        }
                    }
                }		
            }
            return null;
        }

    public static bool arrContains(CoordFive[] array, CoordFive target) 
    {
        foreach (CoordFive element in array) 
        {
            if (element.Equals(target)) 
            {
                return true;
            }
        }
        return false;
    }

    public static CoordFive FindPiece(Board b, int target) 
    {
		for (int x = 0; x < b.Width; x++) 
        {
			for (int y = 0; y < b.Height; y++) 
            {
				if(b.GetSquare(x,y) == target) 
                {
					return new CoordFive(x,y,0,0);
				}
			}
		}
		return null;
	}

    }
}
