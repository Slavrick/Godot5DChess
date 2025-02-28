using Godot;
using System;
using Test;

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
	}
}
