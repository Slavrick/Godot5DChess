using System;
using System.Collections.Generic;
using FiveDChess;

namespace Test
{
	public class TurnTester
	{
		public static void TestTurnEquals()
		{
			Move m1 = new Move(new CoordFive(0, 1, 2, 3), new CoordFive(1, 1, 2, 4));
			Move m2 = new Move(new CoordFive(0, 1, 2, 2), new CoordFive(1, 1, 2, 1));
			Move m3 = new Move(new CoordFive(0, 0, 0, 0), new CoordFive(0, 0, 0, 0));

			Move m4 = new Move(new CoordFive(0, 1, 2, 3), new CoordFive(1, 1, 2, 4));
			Move m5 = new Move(new CoordFive(0, 1, 2, 2), new CoordFive(1, 1, 2, 1));
			Move m6 = new Move(new CoordFive(0, 0, 0, 0), new CoordFive(0, 0, 0, 0));

			Move m7 = new Move(new CoordFive(0, 1, 2, 3), new CoordFive(1, 1, 2, 4));
			Move m8 = new Move(new CoordFive(0, 1, 2, 2), new CoordFive(1, 1, 2, 1));
			Move m9 = new Move(new CoordFive(0, 0, 0, 0), new CoordFive(1, 1, 1, 1));

			List<Move> moves1 = new List<Move> { m1, m2, m3 };
			List<Move> moves2 = new List<Move> { m6, m4, m5 };
			List<Move> moves3 = new List<Move> { m9, m7, m8 };

			List<int> in1 = new List<int> { 3, 4, 0, 1 };
			List<int> in2 = new List<int> { 3, 4, 0, 1 };

			Turn t1 = new Turn(moves1, in1);
			Turn t2 = new Turn(moves2, in2);
			Turn t3 = new Turn(moves3, in2);
			if (!t1.Equals(t2)) 
			{
				throw new Exception("Assertion failed: t1 should equal t2");
			}

			if (t1.Equals(t3)) 
			{
				throw new Exception("Assertion failed: t1 should not equal t3");
			}
			
		}
	}
}
