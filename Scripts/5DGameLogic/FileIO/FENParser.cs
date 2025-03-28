using System;
using System.Collections.Generic;
using Engine;
using Godot;

namespace FileIO5D {
  public class FENParser {
	public static readonly string STDBOARDFEN = "[r*nbqk*bnr*/p*p*p*p*p*p*p*p*/8/8/8/8/P*P*P*P*P*P*P*P*/R*NBQK*BNR*:0:1:w]";
	public static readonly string STD_PRINCESS_BOARDFEN = "[r*nbsk*bnr*/p*p*p*p*p*p*p*p*/8/8/8/8/P*P*P*P*P*P*P*P*/R*NBSK*BNR*:0:1:w]";
	public static readonly string STD_DEFENDEDPAWN_BOARDFEN = "[r*qbnk*bnr*/p*p*p*p*p*p*p*p*/8/8/8/8/P*P*P*P*P*P*P*P*/R*QBNK*BNR*:0:1:w]";

	public static GameStateManager ShadSTDGSM(string fileLocation) {
		if (!FileAccess.FileExists(fileLocation))
		{
			Console.WriteLine($"File not found: {fileLocation}");
			return null;
		}
		using var file = FileAccess.Open(fileLocation, FileAccess.ModeFlags.Read);
		if (file == null)
		{
			Console.WriteLine("Failed to open file.");
			return null;
		}
		string content = file.GetAsText();
		string[] linesarray = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
		List<string> lines =  new List<string>(linesarray);
		string size = null;
		string variant = null;
		string color = null;
		List < string > fenBoards = new List < string > ();
		List < string > moves = new List < string > ();
		bool evenStarters = false;
		foreach(string line in lines) {
		if (line.Length == 0){
			continue;
		}
		if (line[0] == '[') {
			string line2 = line;
			if (line.Contains("\"")) {
				line2 = line.ToLower();
		  	}
		  if (line2.Contains("size")) {
			size = line2;
		  }
		  if (line2.Contains("variant") || line2.Contains("board")) {
			variant = line2;
		  }
		  if (line.Contains("color")) {
			color = line2;
		  }
		  if (line2.Contains(":") && !line2.Contains("\"")) {
			fenBoards.Add(line2);
			if (line2.Contains("-0")) {
			  evenStarters = true;
			}
		  }
		} else {
		  moves.Add(line);
		}
	  }

	  Timeline[] starters;
	  GameStateManager gsm = null;

	  if (variant != null) {
		string boardChosen = variant.Substring(variant.IndexOf("\"") + 1, variant.LastIndexOf("\"") - variant.IndexOf("\"") - 1);
		if (boardChosen.Equals("custom", StringComparison.OrdinalIgnoreCase)) {
		  if (size == null) {
			return null;
		  }
		  int width = int.Parse(size.Substring(size.IndexOf('\"') + 1, size.IndexOf('x') - size.IndexOf('\"') - 1));
		  int height = int.Parse(size.Substring(size.IndexOf('x') + 1, size.LastIndexOf('\"') - size.IndexOf('x') - 1));
		  int count = 0;
		  starters = new Timeline[fenBoards.Count];
		  foreach(string FEN in fenBoards) {
			starters[count++] = GetTimelineFromString(FEN, width, height, evenStarters);
		  }
		  Array.Sort(starters);
		  gsm = new GameStateManager(starters, width, height, evenStarters, true, starters[0].Layer, null);
		} else if (boardChosen.Equals("standard", StringComparison.OrdinalIgnoreCase)) {
		  starters = new Timeline[1];
		  starters[0] = GetTimelineFromString(STDBOARDFEN, 8, 8, false);
		  gsm = new GameStateManager(starters, 8, 8, false, true, starters[0].Layer, null);
		} else if (boardChosen.Equals("standard-princess", StringComparison.OrdinalIgnoreCase)) {
		  starters = new Timeline[1];
		  starters[0] = GetTimelineFromString(STD_PRINCESS_BOARDFEN, 8, 8, false);
		  gsm = new GameStateManager(starters, 8, 8, false, true, starters[0].Layer, null);
		}
	  } else {
		return null;
	  }

	  if (color != null) {
		gsm.Color = color.Contains("white");
	  }

	  List < string > turns = new List < string > ();
	  foreach(string s in moves) {
		if (s.Contains("/")) {
		  turns.AddRange(s.Split('/'));
		} else {
		  turns.Add(s);
		}
	  }

	  foreach(string strturn in turns) {
		gsm.MakeTurn(StringToTurn(gsm, strturn, evenStarters));
	  }

	  return gsm;
	}

	public static Timeline GetTimelineFromString(string board, int width, int height, bool evenStarters) {
	  board = board.Substring(1, board.Length - 2);
	  Board b = new Board(width, height);
	  string[] fields = board.Split(':');
	  string[] rows = fields[0].Split('/');
	  int row = 0;
	  int col = 0;

	  try {
		foreach(string s in rows) {
		  for (int charat = 0; charat < s.Length; charat++) {
			char c = s[charat];
			if (c >= 'A') {
			  int piece = Array.IndexOf(Board.PieceChars, c);
			  if (charat < s.Length - 1 && s[charat + 1] == '*') {
				piece *= -1;
				charat++;
			  }
			  b.SetSquare(col, height - row - 1, piece);
			  col++;
			} else if (c <= '9' && c >= '1') {
			  for (int i = 0; i < c - '0'; i++) {
				b.SetSquare(col,height - row - 1, Board.EMPTYSQUARE);
				col++;
			  }
			} else {
			  Console.WriteLine("There was an error reading the FEN provided");
			  col++;
			}
		  }
		  row++;
		  col = 0;
		}
	  } catch (IndexOutOfRangeException) {
		Console.WriteLine("There was an indexOutOfBounds -- this probably means the height or width was incorrectly provided");
		Console.WriteLine("Height of: " + (height - row - 1) + "Width of: " + (col - height - 1));
		return null;
	  }

	  Timeline t = new Timeline(b, fields[3][0] == 'w', int.Parse(fields[2]), ParseLayer(fields[1], evenStarters));
	  return t;
	}
/* XXX Slated for Removal. This is not used ever since eventimelines were implemented.
	public static Timeline GetTimelineFromString(string timelinestr, int layer, int width, int height) {
	  string[] fields = timelinestr.Split(';');
	  string[] rows = fields[0].Split('/');
	  Board b = new Board(width, height);
	  int row = 0;
	  int col = 0;

	  try {
		foreach(string s in rows) {
		  for (int charat = 0; charat < s.Length; charat++) {
			char c = s[charat];
			if (c >= 'A') {
			  int piece = Array.IndexOf(Board.PieceChars, c);
			  if (charat < s.Length - 1 && s[charat + 1] == '*') {
				piece *= -1;
				charat++;
			  }
			  b.SetSquare(height - row - 1,col, piece);
			  col++;
			} else if (c <= '9' && c >= '1') {
			  for (int i = 0; i < c - '0'; i++) {
				b.SetSquare(col,height - row - 1, Board.EMPTYSQUARE);
				col++;
			  }
			} else {
			  Console.WriteLine("There was an error reading the FEN provided");
			  col++;
			}
		  }
		  row++;
		  col = 0;
		}
	  } catch (IndexOutOfRangeException) {
		Console.WriteLine("There was an indexOutOfBounds -- this probably means the height or width was incorrectly provided");
		Console.WriteLine("Height of: " + (height - row - 1) + "Width of: " + (col - height - 1));
		return null;
	  }

	  if (fields[2] == "-") {
		b.EnPassentSquare = null;
	  } else {
		int file = fields[2][0] - 'a';
		int rank = fields[2][1] - '1';
		b.EnPassentSquare = new CoordFive(file, rank, 0, 0);
	  }

	  bool color = fields[3][0] == 'w';
	  int timeStart = int.Parse(fields[3].Substring(1));
	  Timeline t = new Timeline(b, color, timeStart, layer);
	  return t;
	}
 */
	public static Turn StringToTurn(GameState g, string turnstr, bool evenStarters) {
	  if (turnstr.Contains(".")) {
		turnstr = turnstr.Substring(turnstr.IndexOf('.') + 1);
	  }
	  turnstr = turnstr.Trim();
	  if (turnstr.Length <= 1) {
		return null;
	  }
	  string[] movesStr = turnstr.Split(' ', StringSplitOptions.RemoveEmptyEntries);
	  Move[] moves = new Move[movesStr.Length];
	  for (int i = 0; i < movesStr.Length; i++) {
		string token = movesStr[i];
		if (ShouldIgnoreToken(token)) {
		  moves[i] = null;
		  continue;
		}
		if (IsSpecialMove(token)) {
		  if (token.Contains("O-O")) {
			moves[i] = FindCastleMove(g, token, evenStarters);
		  } else if (token.Contains("=")) {
			int promotion = Board.PieceCharToInt(token[token.Length - 1]);
			if (!g.Color) {
			  promotion += Board.numTypes;
			}
			moves[i] = GetShadMove(g, token.Substring(0, token.Length - 2), evenStarters);
			moves[i].SpecialType = promotion;

		  } else if (token.Contains("0000")) {
			CoordFive boardOrigin = new CoordFive(0, 0, 0, 0);
			if (token.Contains("(")) {
			  boardOrigin.Add(TemporalToCoord(token.Substring(token.IndexOf('('), token.IndexOf(')') + 1), evenStarters));
			} else {
			  boardOrigin.T = g.GetTimeline(0).TEnd;
			}
			moves[i] = new Move(boardOrigin);
		  }
		} else {
		  moves[i] = GetShadMove(g, token, evenStarters);
		}
	  }
	  return new Turn(moves);
	}

	private static Move FindCastleMove(GameState g, string token, bool evenStarters) {
	  bool side = true;
	  CoordFive temporalOrigin;
	  if (token.Contains("O-O-O")) {
		side = false;
	  } else if (token.Contains("O-O")) {
		side = true;
	  }
	  if (token.Contains("(")) {
		temporalOrigin = new CoordFive(TemporalToCoord(token.Substring(token.IndexOf('('), token.IndexOf(')') + 1), evenStarters), g.Color);
	  } else {
		temporalOrigin = new CoordFive(0, 0, g.GetTimeline(0).TEnd, 0, g.Color);
	  }
	  Board origin = g.GetBoard(temporalOrigin);
	  int target = g.Color ? -1 * (int) Board.Piece.WKING : -1 * (int) Board.Piece.BKING;
	  CoordFive spatialOrigin = MoveGenerator.findPiece(origin, target);
	  temporalOrigin.Add(spatialOrigin);
	  Move castle = new Move(temporalOrigin, MoveGenerator.kingCanCastle(origin, temporalOrigin, side));
	  return castle;
	}

	private static bool IsSpecialMove(string str) {
	  return str.Contains("O-O") || str.Contains("=") || str.Contains("0000");
	}

	private static bool ShouldIgnoreToken(string str) {
	  return str[0] == '(' && str[str.Length - 1] == ')';
	}

	public static Move GetShadMove(GameState g, string move, bool evenStarters) {
	  move = System.Text.RegularExpressions.Regex.Replace(move, "[~!#]", "");
	  if (move.Contains("(") && move.IndexOf("(") != move.LastIndexOf("(")) {
		return FullStringToCoord(move, evenStarters);
	  }
	  return AmbiguousStringToMove(g, move, evenStarters);
	}

	public static Move FullStringToCoord(string move, bool evenStarters) {
	  string coord1;
	  if (move.Contains(">")) {
		coord1 = move.Substring(0, move.IndexOf('>'));
	  } else {
		coord1 = move.Substring(0, move.LastIndexOf('('));
	  }
	  string coord2 = move.Substring(move.LastIndexOf('('));
	  return new Move(HalfStringToCoord(coord1, evenStarters), HalfStringToCoord(coord2, evenStarters));
	}

	public static Move AmbiguousStringToMove(GameState g, string move, bool evenStarters) {
	  CoordFive dest = new CoordFive(HalfStringToCoord(move, evenStarters), g.Color);
	  if (dest.T == -1) {
		dest.T = g.GetTimeline(0).TEnd;
	  }
	  char piecechar;
	  int piece;
	  if (move.Contains(")")) {
		piecechar = move[move.IndexOf(')') + 1];
	  } else {
		piecechar = move[0];
	  }
	  if (piecechar <= 'Z') {
		piece = Array.IndexOf(Board.PieceChars, piecechar);
	  } else {
		piece = (int) Board.Piece.WPAWN;
	  }
	  piece = g.Color ? piece : piece + Board.numTypes;
	  CoordFive ambiguity = GetAmbiguityInfo(move);
	  int file = ambiguity.X;
	  int rank = ambiguity.Y;
	  CoordFive origin = MoveGenerator.reverseLookup(g, dest, piece, rank, file);
	  if (origin == null) {
		Console.WriteLine("Could not find ReverseLookup for this move: " + move);
		return null;
	  }
	  return new Move(origin, dest);
	}

	public static CoordFive GetAmbiguityInfo(string move) {
	  CoordFive temp = new CoordFive(-1, -1, -1, -1);
	  move = move.Trim();
	  int index = 0;
	  int rindex = move.Length;
	  if (move.Contains(")")) {
		index = move.IndexOf(")") + 1;
	  }
	  if (move[index] > 'A' && move[index] < 'a') {
		index++;
	  }
	  if (move[index] == 'x') {
		index++;
	  }
	  if (rindex - index >= 3 && move[rindex - 3] == 'x') {
		rindex -= 3;
	  } else {
		rindex -= 2;
	  }
	  move = move.Substring(index, rindex - index);
	  if (move.Length == 2) {
		temp = SANToCoord(move);
		temp.T = -1;
		temp.L = -1;
	  } else if (move.Length == 1) {
		if (move[0] >= 'a') {
		  temp.X = move[0] - 'a';
		} else {
		  temp.Y = int.Parse(move) - 1;
		}
	  }
	  return temp;
	}

	public static CoordFive HalfStringToCoord(string halfmove, bool evenStarters) {
	  string sancoord;
	  if (halfmove[halfmove.Length - 1] == 'x') {
		sancoord = halfmove.Substring(halfmove.Length - 3, 2);
	  } else {
		sancoord = halfmove.Substring(halfmove.Length - 2);
	  }
	  CoordFive coord = SANToCoord(sancoord);
	  if (halfmove.Contains("(")) {
		coord.Add(TemporalToCoord(halfmove.Substring(halfmove.IndexOf('('), halfmove.IndexOf(')') + 1), evenStarters));
	  } else {
		coord.L = 0;
		coord.T = -1;
	  }
	  return coord;
	}

	public static CoordFive StringToCoord(string coord) {
	  coord = coord.Substring(1, coord.Length - 2);
	  string[] coords = coord.Split(',');
	  int x = int.Parse(coords[0]);
	  int y = int.Parse(coords[1]);
	  int t = int.Parse(coords[2]);
	  int l = int.Parse(coords[3]);
	  return new CoordFive(x, y, t, l);
	}

	public static Move StringToMove(string move) {
	  CoordFive c1 = StringToCoord(move.Substring(0, move.IndexOf(')') + 1));
	  CoordFive c2 = StringToCoord(move.Substring(move.IndexOf(')') + 1));
	  return new Move(c1, c2);
	}

	public static Move StringToMove(string coord1, string coord2) {
	  CoordFive c1 = StringToCoord(coord1);
	  CoordFive c2 = StringToCoord(coord2);
	  return new Move(c1, c2);
	}

	public static CoordFive TemporalToCoord(string temporal, bool evenStarters) {
	  int T = int.Parse(temporal.Substring(temporal.IndexOf('T') + 1, temporal.IndexOf(')') - temporal.IndexOf('T') - 1));
	  int L = ParseLayer(temporal.Substring(1, temporal.IndexOf('T') - 1), evenStarters);
	  return new CoordFive(0, 0, T, L);
	}

	public static CoordFive SANToCoord(string san) {
	  char file = san[0];
	  string rank = san.Substring(1);
	  return new CoordFive(file - 'a', int.Parse(rank) - 1, 0, 0);
	}

	public static int ParseLayer(string layer, bool evenStarters) {
	  if (!evenStarters) {
		return int.Parse(layer);
	  }
	  if (layer.Contains("-0")) {
		return 0;
	  } else if (layer.Contains("+0")) {
		return 1;
	  }
	  int layerint = int.Parse(layer);
	  if (layerint >= 1) {
		layerint++;
	  }
	  return layerint;
	}
  }
}
