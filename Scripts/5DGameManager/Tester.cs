using Godot;
using System;
using Test;
using FileIO5D;
using Engine;

public partial class Tester : Node
{
	public override void _Ready()
	{
		//if (Engine.IsDebugBuild())
		//{ TODO ADD THIS not sure the rigth way to implement.
			//QueueFree();
		//}
		TurnTester.TestTurnEquals();
		CoordTester.TestAllCoordFourFuncs();
		FENParserTest.TestMoveParser();
		FENParserTest.TestSANParser();
		FENParserTest.TestShadParser();
		FENParserTest.TestFENFileParser();
		FENParserTest.TestShadFEN();
		FENParserTest.TestAmbiguityInfoParser();
		GameStateManager gsm = FENParser.ShadSTDGSM("res://PGN/Variations/Standard-T0.PGN5.txt");
		long time1 = Benchmarker.Measure(gsm, x => x.isMated());
		long time2 = Benchmarker.MeasureAverage(gsm, x => x.isMated(),10);
		Console.WriteLine(time1);
		Console.WriteLine(time2);
	}
}
