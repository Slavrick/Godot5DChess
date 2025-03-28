using Godot;
using System;
using Test;
using FileIO5D;
using Engine;


public partial class Tester : Node
{
	public override void _Ready()
	{
		

	}
	
	public void _on_timer_timeout(){
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
