using Godot;
using System;
using Test;

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
	}
}
