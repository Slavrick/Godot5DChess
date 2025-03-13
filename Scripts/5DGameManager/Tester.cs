using Godot;
using System;
using Test;
using FileIO5D;
using Engine;

public partial class Tester : Node
{
	public override void _Ready()
	{
		TurnTester.TestTurnEquals();
		CoordTester.TestAllCoordFourFuncs();
		FENParserTest.TestMoveParser();
		FENParserTest.TestSANParser();
		FENParserTest.TestShadParser();
		FENParserTest.TestFENFileParser();
		FENParserTest.TestShadFEN();
		FENParserTest.TestAmbiguityInfoParser();
		GameStateManager gsm = FENParser.ShadSTDGSM("res://PGN/testPGNs/ShadTestGame2.txt");
		long time2 = Benchmarker.MeasureAverage(gsm, x => x.isMated(),10);
		Console.Write(time2);
		Console.WriteLine("ns ShadTestGame2.txt");
		GameStateManager gsm2 = FENParser.ShadSTDGSM("res://PGN/NehemiagurlVsQxyzpkS2.txt");
		long time3 = Benchmarker.MeasureAverage(gsm, x => x.isMated(),10);
		Console.Write(time3);
		Console.WriteLine("ns NehemiavsQxyzpkS2");
	}
}
