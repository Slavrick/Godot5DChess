using System;



namespace Engine
{
	public class MoveNotation
	{
		// There are 2 types of pieces, riders and leapers. A rider will "ride" along some sort of line, indefinitely until they hit an edge or other piece.
		// a leaper is a piece who "Leaps" a specific length and stays there.
		// rooks, bishops, queens, unicorns, dragons, are all riders
		// Kings, Rooks, and pawns are leapers. however Pawn 2 move on 2nd/7th rank is not a leap, but pseudo leap/ride.
/* 
		public bool Rider { get; set; } XXX Delete this is Slated for deletion. Originally I had intended to have a rider be determined by something like this. However, i decided to use the below arrays
		public int[] MovNote { get; set; } */ 

/* 		public static readonly int[] ROOK = { 1 }; XXX this is slated for deletion. Originally I wanted to notetate based on the movement potentials
		public static readonly int[] BISHOP = { 2 }; Since each would have to generate the below movesets anyway, just hvaing them there is a little more clearn imo
		public static readonly int[] UNICORN = { 3 };
		public static readonly int[] DRAGON = { 4 };
		public static readonly int[] KING = { 1, 2, 3, 4 };
		public static readonly int[] QUEEN = { 1, 2, 3, 4 }; */

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
	public static CoordFive[] getMoveVectors(int piece) {
		piece = piece < 0 ? piece * -1 : piece;
		switch(piece) {
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
	public static bool pieceIsRider(int piece) {
		piece = piece < 0 ? piece * -1 : piece;
		switch(piece) {
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
	public static bool pieceIsRoyal(int piece) { //TODO replace this with Board.Piece for clarity.
		piece = piece < 0 ? piece * -1 : piece;
		switch(piece) {
		case (int)Board.Piece.WKING + 12://King
		case 7:
		case 11 + 12://Royal Queen
		case 11:
			return true;
		default: 
			return false;
		}
	}
	}
}
