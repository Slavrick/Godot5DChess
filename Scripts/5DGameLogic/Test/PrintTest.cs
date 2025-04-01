using System;
using FiveDChess;
using FileIO5D;

namespace Test
{
	public class PrintTester
    {
        public static void TimeLinePrintTest()
        {
            Console.WriteLine("    Testing timeline Print:");
			string filePath = "res://PGN/Puzzles/RookTactics4.PGN5.txt";
			GameState g = FENParser.ShadSTDGSM(filePath);
            Console.WriteLine(g.GetTimeline(0).ToString());
			Console.WriteLine(" passed.");
        }
    }
}