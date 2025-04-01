using System;
using FiveDChess;

namespace TestRewrite
{
	public class CoordTester
	{
		public static void TestAllCoordFiveFuncs()
		{
			ArithmaticTest();
			VecTest();
		}

		public static void ArithmaticTest()
		{
			Console.Write("    Arithmatic testing:");
			CoordFive add1 = new CoordFive(2, 2, 3, 4);
			CoordFive sum = CoordFive.Add(add1, add1);
			TestCoord(sum, 4, 4, 6, 8);
			CoordFive add2 = new CoordFive(2, 3, 7, 10);
			add2.Add(add1);
			TestCoord(add2, 4, 5, 10, 14);
			CoordFive sub1 = new CoordFive(4, 4, 1, 0);
			sub1.Sub(new CoordFive(5, 5, 1, 0));
			TestCoord(sub1, -1, -1, 0, 0);
			TestCoord(CoordFive.Sub(new CoordFive(4, 4, 1, 0), new CoordFive(2, 6, 2, -1)), 2, -2, -1, 1);

			if (sub1.GetNagonal() != 2)
			{
				throw new Exception("Assertion failed: sub1.GetNagonal() should equal 2");
			}
			Console.WriteLine(" passed.");
		}

		public static void VecTest()
		{
			Console.Write("    Flatten Testing:");
			CoordFive test = new CoordFive(3, 9, 6, 18);
			test.Flatten();
			TestCoord(test, 1, 3, 2, 6);
			CoordFive test2 = new CoordFive(3, 9, 0, 0);
			test2.Flatten();
			TestCoord(test2, 1, 3, 0, 0);
			CoordFive test3 = new CoordFive(-2, -2, 0, -4);
			test3.Flatten();
			TestCoord(test3, -1, -1, 0, -2);
			CoordFive test4 = new CoordFive(0, 0, 0, 0);
			test4.Flatten();
			TestCoord(test4, 0, 0, 0, 0);
			Console.WriteLine(" passed.");
		}

		public static void TestCoord(CoordFive test, int x, int y, int T, int L)
		{
			if (test.X != x || test.Y != y || test.T != T || test.L != L)
			{
				throw new Exception("Assertion failed: Coord values do not match");
			}
		}
	}
}
