using Godot;
using System;
using Test;
using FileIO5D;
using FiveDChess;


public partial class Tester : Node
{
	/// <summary>
	/// Tests various functions
	/// </summary>
	public void _on_timer_timeout()
	{
		//PrintTester.TimeLinePrintTest();
		TurnTester.TestTurnEquals();
		CoordTester.TestAllCoordFiveFuncs();
		FENParserTest.TestMoveParser();
		FENParserTest.TestSANParser();
		FENParserTest.TestShadParser();
		FENParserTest.TestFENFileParser();
		FENParserTest.TestShadFEN();
		FENParserTest.TestAmbiguityInfoParser();
		MateTest.BenchmarkMates();
	}
}
