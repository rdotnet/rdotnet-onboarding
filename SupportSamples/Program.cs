using System;
using RDotNet;

namespace SupportSamples
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			REngine.SetEnvironmentVariables();
			REngine engine = REngine.GetInstance();
			//stackoverflow_27689786_2752565 (engine);
			stackoverflow_27597542_2752565 (engine);
			// you should always dispose of the REngine properly.
			// After disposing of the engine, you cannot reinitialize nor reuse it
			engine.Dispose();
		}

		/// <summary>
		/// http://stackoverflow.com/q/27689786/2752565
		/// </summary>
		static void stackoverflow_27689786_2752565 (REngine engine)
		{
			var rand = new System.Random (0);
			double[] randValues;

			for (int i = 0; i < 10; i++) {
				randValues = mkValues (rand, 100);
				Console.WriteLine ("std dev iteration {0} = {1}", i + 1, calcSDev (engine, randValues));
			}
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

		/// <summary>
		/// http://stackoverflow.com/q/27597542/2752565
		/// </summary>
		static void stackoverflow_27597542_2752565 (REngine engine)
		{
			var createModel = @"
			set.seed(0)
			x <- ts(rnorm(100))
			library(forecast)
			blah <- ets(x)
			# str(blah)
			";
			engine.Evaluate (createModel);
			var m = engine.GetSymbol ("blah").AsList ();
			var components = m ["components"].AsCharacter ().ToArray ();
			for (int i = 0; i < components.Length; i++) {
				Console.WriteLine ("m$components[{0}] = {1}", i + 1, components [i]);
			}
		}
	}
}
