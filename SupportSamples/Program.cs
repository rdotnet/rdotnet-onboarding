using System;
using RDotNet;

namespace SupportSamples
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			stackoverflow_27689786_2752565 ();
		}

		/// <summary>
		/// http://stackoverflow.com/q/27689786/2752565
		/// </summary>
		static void stackoverflow_27689786_2752565 ()
		{
			REngine.SetEnvironmentVariables();
			REngine engine = REngine.GetInstance();

			var rand = new System.Random (0);
			double[] randValues;

			for (int i = 0; i < 10; i++) {
				randValues = mkValues (rand, 100);
				Console.WriteLine ("std dev iteration {0} = {1}", i + 1, calcSDev (engine, randValues));
			}
			// you should always dispose of the REngine properly.
			// After disposing of the engine, you cannot reinitialize nor reuse it
			engine.Dispose();
		}

		static double[] mkValues (Random rand, int n)
		{
			double[] res = new double[n];
			for (int i = 0; i < n; i++) {
				var v = rand.NextDouble ();
				res [i] = (v < 0 ? -1 : 1) * v * v;
			}
			return res;
		}

		static double calcSDev (REngine engine, double[] arr)
		{
			// Note: only one quick and slightly dirty way to do it
			NumericVector rVector = engine.CreateNumericVector(arr);
			engine.SetSymbol ("x", rVector);
			return engine.Evaluate ("sd(x)").AsNumeric () [0];
		}
	}
}
