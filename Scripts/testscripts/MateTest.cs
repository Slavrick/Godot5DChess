using Godot;
using System;
using Engine;
using FileIO5D;

namespace Test{
	public partial class MateTest : Node
	{
		
		public static void BenchmarkMates(){
			TestMate("res://PGN/NehemiagurlVsQxyzpkS2.txt");
			TestMate("res://PGN/MateTest/AquaBabyVsAndrey 6-14-2021.txt");
			TestMate("res://PGN/MateTest/AquaBabyVsSamet 6-12-2021.txt");
			TestMate("res://PGN/MateTest/AquaBabyVsWritenWrong 6-13-2021.txt");
			TestMate("res://PGN/MateTest/test1.txt");
			TestMate("res://PGN/MateTest/test2.txt");
		}

		public static void TestMate(String FilePath)
		{
			GameStateManager gsm = FENParser.ShadSTDGSM(FilePath);
			long time2 = Benchmarker.MeasureAverage(gsm, x => x.isMated(),10);
			Console.Write(time2);
			Console.WriteLine(" ns/run for: " + FilePath);
		}
	}
}
