using System;
using FiveDChess;
using FileIO5D;

namespace Test
{
	/*
	 * A general purpose class for testing the FEN parser.
	 */
	public static class FENParserTest
	{
		public static void TestMoveParser()
		{
			Console.Write("    Testing Move Parser: ");
			CoordFive x = FENParser.ParseRawCoordinate("(0,0,0,0)");
			if (!new CoordFive(0, 0, 0, 0).Equals(x)) throw new Exception(x.ToString());
			x = FENParser.ParseRawCoordinate("(10,5,10,5)");
			if (!new CoordFive(10, 5, 10, 5).Equals(x)) throw new Exception(x.ToString());
			x = FENParser.ParseRawCoordinate("(-10,-5,-2,-1)");
			if (!new CoordFive(-10, -5, -2, -1).Equals(x)) throw new Exception(x.ToString());
			Console.WriteLine("passed.");
		}

		public static void TestFENFileParser()
		{
			Console.Write("    Testing Parsing Rookie.FEN.txt:");
			string filePath = "C:\\Users\\mavmi\\Documents\\5DRewrite\\5DChess\\5DChess\\PGN\\Puzzles\\RookTactics4.PGN5.txt";
			GameState g = FENParser.ShadSTDGSM(filePath);
			if (g == null) throw new Exception("GameState is null");
			if (g.Color != false) throw new Exception("Color mismatch");
			if (g.MinTL != 0) throw new Exception("MinTL mismatch");
			if (g.MaxTL != 1) throw new Exception("MaxTL mismatch");
			if (g.GetTimeline(0).TEnd != 2) throw new Exception("Timeline 0 Tend mismatch");
			if (g.GetTimeline(1).TEnd != 2) throw new Exception("Timeline 1 Tend mismatch");
			Console.WriteLine(" passed.");
		}

		public static void TestSANParser()
		{
			Console.WriteLine("    Testing SAN Parsing.");
			string san1 = "a1";
			string san2 = "h8";
			string san3 = "e4";
			string san4 = "m42";
            CoordFive c1 = FENParser.SANToCoord(san1);
			CoordFive c2 = FENParser.SANToCoord(san2);
			CoordFive c3 = FENParser.SANToCoord(san3);
			CoordFive c4 = FENParser.SANToCoord(san4);
            CoordTester.TestCoord(c1, 0, 0, 0, 0);
			CoordTester.TestCoord(c2, 7, 7, 0, 0);
			CoordTester.TestCoord(c3, 4, 3, 0, 0);
			CoordTester.TestCoord(c4, 12, 41, 0, 0);
		}

		public static void TestShadParser()
		{
			Console.WriteLine("    Testing Full Coord Parser.");
			CoordFive h1 = FENParser.HalfStringToCoord("(0T0)a1", false);
			CoordFive h2 = FENParser.HalfStringToCoord("(0T0)Na1", false);
			CoordFive h3 = FENParser.HalfStringToCoord("(0T1)e3", false);
			CoordFive h4 = FENParser.HalfStringToCoord("(-1T3)Nh7", false);
			CoordFive h5 = FENParser.HalfStringToCoord("(-0T3)Nh7", true);
			CoordFive h6 = FENParser.HalfStringToCoord("(+0T3)Nh7", true);
			CoordFive h7 = FENParser.HalfStringToCoord("(2T3)Nh7", true);
			CoordFive h8 = FENParser.HalfStringToCoord("(-1T3)Nh7", true);
			CoordFive h9 = FENParser.HalfStringToCoord("(-1T12)Bxf6+", false);
            CoordTester.TestCoord(h1, 0, 0, 0, 0);
			CoordTester.TestCoord(h2, 0, 0, 0, 0);
			CoordTester.TestCoord(h3, 4, 2, 1, 0);
			CoordTester.TestCoord(h4, 7, 6, 3, -1);
			CoordTester.TestCoord(h5, 7, 6, 3, 0);
			CoordTester.TestCoord(h6, 7, 6, 3, 1);
			CoordTester.TestCoord(h7, 7, 6, 3, 3);
			CoordTester.TestCoord(h8, 7, 6, 3, -1);
            CoordTester.TestCoord(h9, 5, 5, 12, -1);
            Move f1 = FENParser.FullStringToCoord("(0T1)Ng1(0T1)f3", false);
			Move f2 = FENParser.FullStringToCoord("(0T3)Ng1>(0T2)g3", false);
			Move f3 = FENParser.FullStringToCoord("(0T5)Qc3>>(0T1)f7", false);
			CoordTester.TestCoord(f1.Origin, 6, 0, 1, 0);
			CoordTester.TestCoord(f1.Dest, 5, 2, 1, 0);
			CoordTester.TestCoord(f2.Origin, 6, 0, 3, 0);
			CoordTester.TestCoord(f2.Dest, 6, 2, 2, 0);
			CoordTester.TestCoord(f3.Origin, 2, 2, 5, 0);
			CoordTester.TestCoord(f3.Dest, 5, 6, 1, 0);
		}

		public static void TestShadFEN()
		{
			Console.Write("    Testing Whole Fen Parser.");
			FENParser.ShadSTDGSM("C:\\Users\\mavmi\\Documents\\5DRewrite\\5DChess\\5DChess\\PGN\\Puzzles\\Brawn Tactics 1.5DPGN.txt");
			FENParser.ShadSTDGSM("C:\\Users\\mavmi\\Documents\\5DRewrite\\5DChess\\5DChess\\PGN\\testPGNs\\AmbiguityCheck.txt");
			Console.WriteLine(" passed.");
		}

		public static void TestDefaultGame()
		{
			
		}
		
		public static void TestAmbiguityInfoParser()
		{
			Console.Write("    Testing Ambiguity Getter.");
			CoordFive t1 = FENParser.GetAmbiguityInfo("Ng3");
			CoordTester.TestCoord(t1, -1, -1, -1, -1);
			CoordFive t2 = FENParser.GetAmbiguityInfo("Nfg3");
			CoordTester.TestCoord(t2, 5, -1, -1, -1);
			CoordFive t3 = FENParser.GetAmbiguityInfo("N3g3");
			CoordTester.TestCoord(t3, -1, 2, -1, -1);
			CoordFive t4 = FENParser.GetAmbiguityInfo("Nc5g3");
			CoordTester.TestCoord(t4, 2, 4, -1, -1);
			CoordFive t5 = FENParser.GetAmbiguityInfo("(0T1)N1g3");
			CoordTester.TestCoord(t5, -1, 0, -1, -1);
			Console.WriteLine(" passed.");
		}
	}
}
