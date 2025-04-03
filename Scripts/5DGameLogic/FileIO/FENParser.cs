using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using FiveDChess;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;
using Godot;

//TODO fix style for this.
namespace FileIO5D
{
	public enum TokenType
	{
		UNKNOWN, TURNINDICATOR, HALFTURNINDICATOR, SAN, TEMPORALCOORD, TURNTERMINATOR, PROMOTION, FULLCOORDINATE, HALFCOORDINATE, 
		CASTLE, LONGCASTLE, NULLMOVE, COMMENT, PRESENTSHIFTED, LAYERCREATED
	}

	public class FENParser
	{
		public static readonly string STDBOARDFEN = "[r*nbqk*bnr*/p*p*p*p*p*p*p*p*/8/8/8/8/P*P*P*P*P*P*P*P*/R*NBQK*BNR*:0:1:w]";
		public static readonly string STD_PRINCESS_BOARDFEN = "[r*nbsk*bnr*/p*p*p*p*p*p*p*p*/8/8/8/8/P*P*P*P*P*P*P*P*/R*NBSK*BNR*:0:1:w]";
		public static readonly string STD_DEFENDEDPAWN_BOARDFEN = "[r*qbnk*bnr*/p*p*p*p*p*p*p*p*/8/8/8/8/P*P*P*P*P*P*P*P*/R*QBNK*BNR*:0:1:w]";

		public static string[] FileToLines(string filePath)
		{
			//If this is a godot res path, needs to get it properly.
			if(filePath.Contains("res://"))
			{
				if (!Godot.FileAccess.FileExists(filePath))
				{
					Console.WriteLine($"File not found: {filePath}");
					return null;
				}
				using var file = Godot.FileAccess.Open(filePath, Godot.FileAccess.ModeFlags.Read);
				if (file == null)
				{
					Console.WriteLine("Failed to open file.");
					return null;
				}
				string gdContent = file.GetAsText();
				return gdContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
			}
			if (!File.Exists(filePath))
			{
				Console.WriteLine($"File not found: {filePath}");
				return null;
			}
			string content = File.ReadAllText(filePath);
			string[] linesarray = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
			return linesarray;
		}

		public static GameStateManager ShadSTDGSM(string fileLocation)
		{
			//Read in the file, initialize variables.
			string[] lines = FileToLines(fileLocation);
			List<string> fenBoards = new List<string>();
			List<string> SANLines = new List<string>();
			bool evenStarters = false;

			//Separate moves by header, FEN, SAN
			Dictionary<string, string> headers = new Dictionary<string, string>();
			foreach (string line in lines)
			{
				string trimmmedLine = line.Trim();
				if (trimmmedLine.Length == 0)
				{
					continue;
				}
				if (trimmmedLine[0] == '[')
				{ 
					if (trimmmedLine.Contains(":") && !trimmmedLine.Contains("\""))
					{
						fenBoards.Add(trimmmedLine);
						if (trimmmedLine.Contains("-0"))
						{
							evenStarters = true;
						}
						continue;
					}
					int bracketIndex = trimmmedLine.IndexOf('[');
					int quoteIndex = trimmmedLine.IndexOf("\"");
					int lastQuoteIndex = trimmmedLine.LastIndexOf("\"");
					string headerKey = trimmmedLine.Substring(bracketIndex + 1,quoteIndex - (bracketIndex + 1)).Trim().ToLower();
					string headerValue = trimmmedLine.Substring(quoteIndex + 1, lastQuoteIndex - quoteIndex - 1).Trim();
					headers.Add(headerKey, headerValue);
				}
				else
				{
					SANLines.Add(line);
				}
			}

			//Parse Headers.
			string size = null;
			string variant = null;
			string color = null;
			if (headers.ContainsKey("size")) size = headers["size"];
			if (headers.ContainsKey("board")) variant = headers["board"];
			if (headers.ContainsKey("color")) color = headers["color"];

			//Determine if there is a variant that can be pulled from.
			GameStateManager gsm = null;
			Timeline[] starters;
			if (variant != null)
			{
				string boardChosen = variant.Trim();
				if (boardChosen.Equals("custom", StringComparison.OrdinalIgnoreCase))
				{
					if (size == null)
					{
						return null;
					}
					string[] dimensions = size.Split('x');
					int width = int.Parse(dimensions[0]);
					int height = int.Parse(dimensions[1]);
					int count = 0;
					starters = new Timeline[fenBoards.Count];
					foreach (string FEN in fenBoards)//TODO make sure the file is not passing in mutliple of the same timeline.
					{
						starters[count++] = GetTimelineFromString(FEN, width, height, evenStarters);
					}
					Array.Sort(starters);
					gsm = new GameStateManager(starters, width, height, evenStarters, true, starters[0].Layer, null);
				}
				else if (boardChosen.Equals("standard", StringComparison.OrdinalIgnoreCase))
				{
					starters = new Timeline[1];
					starters[0] = GetTimelineFromString(STDBOARDFEN, 8, 8, false);
					gsm = new GameStateManager(starters, 8, 8, false, true, starters[0].Layer, null);
				}
				else if (boardChosen.Equals("standard-princess", StringComparison.OrdinalIgnoreCase))
				{
					starters = new Timeline[1];
					starters[0] = GetTimelineFromString(STD_PRINCESS_BOARDFEN, 8, 8, false);
					gsm = new GameStateManager(starters, 8, 8, false, true, starters[0].Layer, null);
				}
			}
			else
			{
				return null;
			}

			//reads color header.
			if (color != null)
			{
				gsm.Color = color.Contains("white");
			}

			//Turns into tokens.
			List<string> tokenString = new List<string>();
			List<TokenType> tokenTypes = new List<TokenType>();
			foreach(string s in SANLines)
			{
				string sanLine = s;
				Match turnIndicatorMatch = Regex.Match(sanLine, "^\\d*(w|W|b|B)?\\.");
				if(turnIndicatorMatch.Success)
				{
					tokenString.Add(turnIndicatorMatch.Value);
					tokenTypes.Add(TokenType.TURNINDICATOR);
					sanLine = sanLine.Substring(turnIndicatorMatch.Length);
				}
				string[] splitLine = sanLine.Trim().Split(' ');
				bool commentStatus = false;
				string comment = "";
				foreach(string partial in splitLine)
				{
					string part = partial;
					if(commentStatus)
					{
						if (part.Contains("}"))
						{
							commentStatus = false;
							comment += part.Substring(0, part.IndexOf("}") + 1);
							tokenString.Add(comment);
							tokenTypes.Add(TokenType.COMMENT);
							part = part.Substring(part.IndexOf("}") + 1);
							if (part.Length == 0){ continue; }
						}
						else
						{
							comment += ' ' + part;
							continue;
						}
					}
					if (part.Contains("{"))
					{
						commentStatus = true;
						comment += part.Substring(part.IndexOf("{"));
						part = part.Substring(0, part.IndexOf("{"));
						if (part.Length == 0) { continue; }
					}
					if(part.Length == 0) { continue; }
					if(part.Equals("/"))
					{
						tokenString.Add(part);
						tokenTypes.Add(TokenType.TURNTERMINATOR);
						continue;
					}
					Match halfCoordinateMatch = Regex.Match(part, "\\([+\\-]?\\d+T-?\\d+\\)[NBRQSKCYUDPW]?[a-z]?\\d{0,2}x?[a-z]\\d{1,2}(=[NBRQSKCYUDPW])?");
					Match fullCoordinateMatch = Regex.Match(part, "\\([+\\-]?\\d+T-?\\d+\\)[NBRQSKCYUDPW]?[a-z]?\\d{0,2}[a-z]\\d{1,2}(>+)?x?\\([+\\-]?\\d+T-?\\d+\\)\\S+(=[NBRQSKCYUDPW])?");
					Match simpleSANMatch = Regex.Match(part, "[NBRQSKCYUDPW]?[a-z]?\\d{0,2}x?[a-z]\\d{1,2}(=[NBRQSKCYUDPW])?");//TODO rework this.
					Match nullMoveMatch = Regex.Match(part, "(\\([+\\-]?\\d+T-?\\d+\\))?0000");
					Match castleMatch = Regex.Match(part, "(\\([+\\-]?\\d+T-?\\d+\\))?O-O");
					Match longCastleMatch = Regex.Match(part, "(\\([+\\-]?\\d+T-?\\d+\\))?O-O-O");
					Match timeshiftMatch = Regex.Match(part, "\\(~T[+\\-]?(\\d+)\\)");
					Match timelineCreatedMatch = Regex.Match(part, "\\(>L([+\\-]?\\d+)\\)");
					Match promotionMatch = Regex.Match(part, "=[NBRQSKCYUDPW]");
					if (promotionMatch.Success)
					{
						if(fullCoordinateMatch.Success)
						{
							tokenString.Add(fullCoordinateMatch.Value);
							tokenTypes.Add(TokenType.PROMOTION);
							continue;
						}
						if(halfCoordinateMatch.Success)
						{
							tokenString.Add(halfCoordinateMatch.Value);
							tokenTypes.Add(TokenType.PROMOTION);
							continue;
						}
						if (simpleSANMatch.Success)
						{
							tokenString.Add(simpleSANMatch.Value);
							tokenTypes.Add(TokenType.PROMOTION);
							continue;
						}
					}
					if(nullMoveMatch.Success)
					{
						tokenString.Add(nullMoveMatch.Value);
						tokenTypes.Add(TokenType.NULLMOVE);
						continue;
					}
					if (longCastleMatch.Success)
					{
						tokenString.Add(longCastleMatch.Value);
						tokenTypes.Add(TokenType.LONGCASTLE);
						continue;
					}
					if(castleMatch.Success)
					{
						tokenString.Add(castleMatch.Value);
						tokenTypes.Add(TokenType.CASTLE);
						continue;
					}
					if (fullCoordinateMatch.Success)
					{
						tokenString.Add(fullCoordinateMatch.Value);
						tokenTypes.Add(TokenType.FULLCOORDINATE);
						continue;
					}
					else if (halfCoordinateMatch.Success)
					{
						tokenString.Add(halfCoordinateMatch.Value);
						tokenTypes.Add(TokenType.HALFCOORDINATE);
						continue;
					}
					else if (simpleSANMatch.Success)
					{
						tokenString.Add(simpleSANMatch.Value);
						tokenTypes.Add(TokenType.SAN);//TODO make sure this is the right token.
						continue;
					}
					else if(timeshiftMatch.Success)
					{
						tokenString.Add(timeshiftMatch.Value);
						tokenTypes.Add(TokenType.PRESENTSHIFTED);
						continue;
					}
					else if (timelineCreatedMatch.Success)
					{
						tokenString.Add(timelineCreatedMatch.Value);
						tokenTypes.Add(TokenType.LAYERCREATED);
						continue;
					}
					tokenString.Add(part);
					tokenTypes.Add(TokenType.UNKNOWN);
				}
			}

			//Parse the tokens.
			List<Turn> compiledTurns = new List<Turn>();
			List<Move> compiledMoves = new List<Move>();
			for(int i = 0; i < tokenString.Count; i++)
			{
				switch(tokenTypes[i])
				{
					case TokenType.COMMENT:
					case TokenType.PRESENTSHIFTED: 
					case TokenType.LAYERCREATED:
						//eventually use this for analysis, right now throw it away.
						break;
					case TokenType.TURNINDICATOR:
					case TokenType.TURNTERMINATOR:
						goto case TokenType.HALFTURNINDICATOR;
					case TokenType.HALFTURNINDICATOR:
						if(compiledMoves.Count > 0)
						{
							compiledTurns.Add(new Turn(compiledMoves.ToArray()));
							compiledMoves.Clear();
							bool turnStatus =  gsm.MakeTurn(compiledTurns[compiledTurns.Count - 1]);
							if (!turnStatus)
							{
								throw new Exception("Turn not properly added to gamestate.");
							}
						}
						break;
					case TokenType.FULLCOORDINATE:
					case TokenType.HALFCOORDINATE:
					case TokenType.SAN:
						Move m = GetShadMove(gsm, tokenString[i], evenStarters);
						compiledMoves.Add(m);
						break;
					case TokenType.NULLMOVE://TODO NEED NULL MOVES.
						Move nullMove = GetNullMove(tokenString[i], evenStarters);
						if(!gsm.Color)
						{
							nullMove.SwapColor();
						}
						compiledMoves.Add(nullMove);
						break;
					case TokenType.PROMOTION:
						Move promotionMove = PromotionStringToMove(gsm, tokenString[i], evenStarters);
						compiledMoves.Add(promotionMove);
						break;
					case TokenType.CASTLE:
					case TokenType.LONGCASTLE:
						Move castleMove = FindCastleMove(gsm, tokenString[i], evenStarters);
						compiledMoves.Add(castleMove);
						break;
					case TokenType.UNKNOWN:
						throw new Exception("Unknown token detected");
						break;
				}
			}
			//Add Final Move if needed.
			if (compiledMoves.Count > 0)
			{
				compiledTurns.Add(new Turn(compiledMoves.ToArray()));
				compiledMoves.Clear();
				bool turnStatus = gsm.MakeTurn(compiledTurns[compiledTurns.Count - 1]);
				if (!turnStatus)
				{
					throw new Exception("Turn not properly added to gamestate.");
				}
			}
			return gsm;
		}

		/// <summary>
		/// Parses a FEN, returns a timeline TODO must ensure that fen boards for the same timeline are properly parsed.
		/// </summary>
		/// <param name="board"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="evenStarters"></param>
		/// <returns></returns>
		public static Timeline GetTimelineFromString(string board, int width, int height, bool evenStarters)
		{
			board = board.Substring(1, board.Length - 2);
			Board b = new Board(width, height);
			string[] fields = board.Split(':');
			string[] rows = fields[0].Split('/');
			int row = 0;
			int col = 0;
			try
			{
				foreach (string s in rows)
				{
					for (int charat = 0; charat < s.Length; charat++)
					{
						char c = s[charat];
						if (c >= 'A')
						{
							int piece = Array.IndexOf(Board.PieceChars, c);
							if (charat < s.Length - 1 && s[charat + 1] == '*')
							{
								piece *= -1;
								charat++;
							}
							b.SetSquare(col, height - row - 1, piece);
							col++;
						}
						else if (c <= '9' && c >= '1')
						{
							for (int i = 0; i < c - '0'; i++)
							{
								b.SetSquare(col, height - row - 1, Board.EMPTYSQUARE);
								col++;
							}
						}
						else
						{
							Console.WriteLine("There was an error reading the FEN provided");
							col++;
						}
					}
					row++;
					col = 0;
				}
			}
			catch (IndexOutOfRangeException)
			{
				Console.WriteLine("There was an indexOutOfBounds -- this probably means the height or width was incorrectly provided");
				Console.WriteLine("Height of: " + (height - row - 1) + "Width of: " + (col - height - 1));
				return null;
			}

			Timeline t = new Timeline(b, fields[3][0] == 'w', int.Parse(fields[2]), ParseLayer(fields[1], evenStarters));
			return t;
		}

		/// <summary>
		/// Takes a turn string and retuns Turn structure object.
		/// </summary>
		/// <param name="g">gamestate that is being parsed for. Required since certain moves are ambigious and you must search for the piece.</param>
		/// <param name="turnstr">Turn to parse</param>
		/// <param name="evenStarters">whether there are an even number of Starters</param> 
		/// <returns>Parsed Turn Object</returns>
		public static Turn StringToTurn(GameState g, string turnstr, bool evenStarters)
		{
			if (turnstr.Contains("."))
			{
				turnstr = turnstr.Substring(turnstr.IndexOf('.') + 1);
			}
			turnstr = turnstr.Trim();
			if (turnstr.Length <= 1)
			{
				return null;
			}
			string[] movesStr = turnstr.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			Move[] moves = new Move[movesStr.Length];
			for (int i = 0; i < movesStr.Length; i++)
			{
				string token = movesStr[i];
				if (ShouldIgnoreToken(token))
				{
					moves[i] = null;
					continue;
				}
				if (IsSpecialMove(token))
				{
					if (token.Contains("O-O"))
					{
						moves[i] = FindCastleMove(g, token, evenStarters);
					}
					else if (token.Contains("="))
					{
						int promotion = Board.PieceCharToInt(token[token.Length - 1]);
						if (!g.Color)
						{
							promotion += Board.NUMTYPES;
						}
						moves[i] = GetShadMove(g, token.Substring(0, token.Length - 2), evenStarters);
						moves[i].SpecialType = promotion;

					}
					else if (token.Contains("0000"))
					{
						CoordFive boardOrigin = new CoordFive();
						boardOrigin.Color = g.Color;
						if (token.Contains("("))
						{
							boardOrigin.Add(TemporalToCoord(token.Substring(token.IndexOf('('), token.IndexOf(')') + 1), evenStarters));
						}
						else
						{
							boardOrigin.T = g.GetTimeline(0).TEnd;
						}
						Move nullMove = new Move(boardOrigin, boardOrigin);
						nullMove.SpecialType = Move.NULLMOVE;
						moves[i] = nullMove;
					}
				}
				else
				{
					moves[i] = GetShadMove(g, token, evenStarters);
				}
			}
			return new Turn(moves);
		}

		private static bool IsSpecialMove(string str)
		{
			return str.Contains("O-O") || str.Contains("=") || str.Contains("0000");
		}

		private static bool ShouldIgnoreToken(string str)
		{
			return str[0] == '(' && str[str.Length - 1] == ')';
		}

		private static Move FindCastleMove(GameState g, string token, bool evenStarters)
		{
			bool side = true;
			CoordFive temporalOrigin;
			if (token.Contains("O-O-O"))
			{
				side = false;
			}
			else if (token.Contains("O-O"))
			{
				side = true;
			}
			if (token.Contains("("))
			{
				temporalOrigin = new CoordFive(TemporalToCoord(token.Substring(token.IndexOf('('), token.IndexOf(')') + 1), evenStarters), g.Color);
			}
			else
			{
				temporalOrigin = new CoordFive(0, 0, g.GetTimeline(0).TEnd, 0, g.Color);
			}
			Board origin = g.GetBoard(temporalOrigin);
			int target = g.Color ? -1 * (int)Board.Piece.WKING : -1 * (int)Board.Piece.BKING;
			CoordFive spatialOrigin = MoveGenerator.FindPiece(origin, target);
			temporalOrigin.Add(spatialOrigin);
			Move castle = new Move(temporalOrigin, MoveGenerator.KingCanCastle(origin, temporalOrigin, side));
			return castle;
		}

		public static Move PromotionStringToMove(GameState g, string move, bool evenStarters)
		{
			Move returnMove = GetShadMove(g, move.Substring(0,move.IndexOf("=")), evenStarters);
			int promotionType = Board.PieceCharToInt(move[move.Length-1]);
			if(!g.Color)
			{
				promotionType += Board.NUMTYPES;
			}    
			returnMove.SpecialType = promotionType;
			return returnMove;
		}

		public static Move GetShadMove(GameState g, string move, bool evenStarters)
		{
			move = System.Text.RegularExpressions.Regex.Replace(move, "[~!#+]", "");
			if (move.Contains("(") && move.IndexOf("(") != move.LastIndexOf("("))
			{
				Move parsedMove = FullStringToCoord(move, evenStarters);
				if(!g.Color)
				{
					parsedMove.SwapColor();
				}
				return parsedMove;
			}
			return AmbiguousStringToMove(g, move, evenStarters);
		}

		public static Move GetNullMove(string move, bool evenStarters)
		{
			Move nullMove = Move.NULL.Clone();
			Match temporalPositionMatch = Regex.Match(move, "\\([+\\-]?\\d+T-?\\d+\\)");
			if (temporalPositionMatch.Success)
			{
				CoordFive temporalPosition = TemporalToCoord(temporalPositionMatch.Value, evenStarters);
				nullMove.Origin.Add(temporalPosition);
				nullMove.Dest.Add(temporalPosition);
			}
			return nullMove;
		}

		public static Move FullStringToCoord(string move, bool evenStarters)
		{
			string coord1;
			if (move.Contains(">"))
			{
				coord1 = move.Substring(0, move.IndexOf('>'));
			}
			else
			{
				coord1 = move.Substring(0, move.LastIndexOf('('));
			}
			string coord2 = move.Substring(move.LastIndexOf('('));
			return new Move(HalfStringToCoord(coord1, evenStarters), HalfStringToCoord(coord2, evenStarters));
		}

		/// <summary>
		/// Gets an ambiguous Move, (only spatial moves work, this will not get an ambigious temporal move)
		/// </summary>
		/// <param name="g">gamestate</param>
		/// <param name="move">ambiguous move string</param>
		/// <param name="evenStarters">whether there are +/- 0 or just 0. if true -0 =0L and +0 =1L </param>
		/// <returns></returns>
		public static Move AmbiguousStringToMove(GameState g, string move, bool evenStarters)
		{
			CoordFive dest = HalfStringToCoord(move, evenStarters);
			dest.Color = g.Color;
			if (dest.T == -1)
			{
				dest.T = g.GetTimeline(0).TEnd;
			}
			char piecechar;
			int piece;
			if (move.Contains(")"))
			{
				piecechar = move[move.IndexOf(')') + 1];
			}
			else
			{
				piecechar = move[0];
			}
			if (piecechar <= 'Z')
			{
				piece = Array.IndexOf(Board.PieceChars, piecechar);
			}
			else
			{
				piece = (int)Board.Piece.WPAWN;
			}
			piece = g.Color ? piece : piece + Board.NUMTYPES;
			CoordFive ambiguity = GetAmbiguityInfo(move);
			int file = ambiguity.X;
			int rank = ambiguity.Y;
			CoordFive origin = MoveGenerator.ReverseLookup(g, dest, piece, rank, file);
			if (origin == null)
			{
				Console.WriteLine("Could not find ReverseLookup for this move: " + move);
				return null;
			}
			return new Move(origin, dest);
		}

		public static CoordFive GetAmbiguityInfo(string move)
		{
			CoordFive temp = new CoordFive(-1, -1, -1, -1);
			move = move.Trim();
			int index = 0;
			int rindex = move.Length;
			if (move.Contains(")"))
			{
				index = move.IndexOf(")") + 1;
			}
			if (move[index] > 'A' && move[index] < 'a')
			{
				index++;
			}
			if (move[index] == 'x')
			{
				index++;
			}
			if (rindex - index >= 3 && move[rindex - 3] == 'x')
			{
				rindex -= 3;
			}
			else
			{
				rindex -= 2;
			}
			move = move.Substring(index, rindex - index);
			if (move.Length == 2)
			{
				temp = SANToCoord(move);
				temp.T = -1;
				temp.L = -1;
			}
			else if (move.Length == 1)
			{
				if (move[0] >= 'a')
				{
					temp.X = move[0] - 'a';
				}
				else
				{
					temp.Y = int.Parse(move) - 1;
				}
			}
			return temp;
		}

		/// <summary>
		/// Gets a coord from half of a move. Does not enter the correct color into the coordinate.
		/// </summary>
		/// <param name="halfmove">string move to parse</param>
		/// <param name="evenStarters">boolean whether there are even amounts of starters, ie. +/- 0</param>
		/// <returns></returns>
		public static CoordFive HalfStringToCoord(string halfmove, bool evenStarters)
		{
			//match a temporal coordinate. and parse it
			Match temporalMatch = Regex.Match(halfmove, "\\(-?\\+?\\d*T-?\\d*\\)");
			string temporalCoordString = "";
			string sanCoordString = halfmove;
			CoordFive temporalCoord;
			if (temporalMatch.Success)
			{
				temporalCoordString = temporalMatch.Value;
				temporalCoord = TemporalToCoord(temporalCoordString,evenStarters);
				sanCoordString = halfmove.Substring(temporalMatch.Index + temporalMatch.Length);
			}
			else
			{
				temporalCoord = new CoordFive(0, 0, -1, 0);
			}

			//the rest is the san Coord.
			Match SANMatch = Regex.Match(sanCoordString, "[a-z]\\d+");
			if(SANMatch.Success)
			{
				//this is needed if for example you have something like Ne7e4
				Match lastSANMatch = Regex.Match(sanCoordString.Substring(SANMatch.Index + 2), "[a-z]\\d+");
				if(lastSANMatch.Success)
				{
					SANMatch = lastSANMatch;
				}
			}

			sanCoordString = SANMatch.Value;
			CoordFive sanCoord = SANToCoord(sanCoordString);
			
			return CoordFive.Add(sanCoord,temporalCoord);
		}

		/// <summary>
		/// Takes in a temporal coordinate and passes back a CoordFive Temporal coordinate
		/// For example parses a string "(0T0)", "(-1T12)", etc.
		/// </summary>
		/// <param name="temporal"></param>
		/// <param name="evenStarters"></param>
		/// <returns></returns>
		public static CoordFive TemporalToCoord(string temporal, bool evenStarters)
		{
			int T = int.Parse(temporal.Substring(temporal.IndexOf('T') + 1, temporal.IndexOf(')') - temporal.IndexOf('T') - 1));
			int L = ParseLayer(temporal.Substring(1, temporal.IndexOf('T') - 1), evenStarters);
			return new CoordFive(0, 0, T, L);
		}

		/// <summary>
		/// Takes in a SAN and passes back a coordfive that is spatial Depending on the SAN passed.
		/// examples include a4 b3 c20 etc. this only works for files up to the letter z (width of 26 is the max, wont be able to parse aa for example)
		/// </summary>
		/// <param name="san">SAN string to parse</param>
		/// <returns>CoordFive spatial coord. No temporal or color values are set.</returns>
		public static CoordFive SANToCoord(string san)
		{
			char file = san[0];
			string rank = san.Substring(1);
			return new CoordFive(file - 'a', int.Parse(rank) - 1, 0, 0);
		}

		/// <summary>
		/// Parses a layer string, ie, just a string but with the added bonus that +/-L are taken into account
		/// as with the other parts of this program, -0 is technically 0L and +0 is 1L
		/// </summary>
		/// <param name="layer">Layer string to parse</param>
		/// <param name="evenStarters">whther +-0exist</param>
		/// <returns></returns>
		public static int ParseLayer(string layer, bool evenStarters)
		{
			if (!evenStarters)
			{
				return int.Parse(layer);
			}
			if (layer.Contains("-0"))
			{
				return 0;
			}
			else if (layer.Contains("+0"))
			{
				return 1;
			}
			int layerint = int.Parse(layer);
			if (layerint >= 1)
			{
				layerint++;
			}
			return layerint;
		}

		/// <summary>
		/// Parses a raw string ie. 0,0,0,0. (0T1)e4 would be 4,3,1,0
		/// this function was no longer in use after moving to SAN
		/// </summary>
		/// <param name="coord">string to parse</param>
		/// <returns>coordfive of string, minus the color</returns>
		public static CoordFive ParseRawCoordinate(string coord)
		{
			coord = coord.Substring(1, coord.Length - 2);
			string[] coords = coord.Split(',');
			int x = int.Parse(coords[0]);
			int y = int.Parse(coords[1]);
			int t = int.Parse(coords[2]);
			int l = int.Parse(coords[3]);
			return new CoordFive(x, y, t, l);
		}

		/// <summary>
		/// Takes a coordinate move and turns it into a move object.
		/// </summary>
		/// <param name="move">Coordinate move string</param>
		/// <returns>Move representing the string</returns>
		public static Move ParseRawCoordinateMove(string move)
		{
			CoordFive c1 = ParseRawCoordinate(move.Substring(0, move.IndexOf(')') + 1));
			CoordFive c2 = ParseRawCoordinate(move.Substring(move.IndexOf(')') + 1));
			return new Move(c1, c2);
		}

		/// <summary>
		/// Takes 2 coordinate strings and turns it into a move. Does not include Color information.
		/// </summary>
		/// <param name="coord1">Coordinate move origin String</param>
		/// <param name="coord2">Coordinate move Destination String</param>
		/// <returns>Move representing the string</returns>
		public static Move ParseRawCoordinateMove(string coord1, string coord2)
		{
			CoordFive c1 = ParseRawCoordinate(coord1);
			CoordFive c2 = ParseRawCoordinate(coord2);
			return new Move(c1, c2);
		}
	}
}
