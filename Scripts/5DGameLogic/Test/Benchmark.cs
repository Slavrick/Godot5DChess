using FiveDChess;
using System;
using System.Diagnostics;

namespace Test  {
	public class Benchmarker {

		public static long Measure<T>(T obj, Action<T> method)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			method(obj);
			stopwatch.Stop();
			
			long nanoseconds = stopwatch.ElapsedTicks * (1_000_000_000L / Stopwatch.Frequency);
			return nanoseconds;
		}

		public static long MeasureAverage<T>(T obj, Action<T> method, int iterations)
		{

			long nanoseconds = 0;
			for(int i = 0; i < iterations; i++){
				var stopwatch = new Stopwatch();
				stopwatch.Start();
				method(obj);
				stopwatch.Stop();
				nanoseconds += stopwatch.ElapsedTicks * (1_000_000_000L / Stopwatch.Frequency);
			}
			nanoseconds /= iterations;
			return nanoseconds;
		}

	}
}
