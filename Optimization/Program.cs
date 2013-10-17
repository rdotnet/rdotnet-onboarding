using System;
using RDotNet;
using RDotNetSetup;
using System.Linq;

namespace Optimization
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            /* This sample code is around the use case of mathematical optimization
             * It is derived from a real use case in environmental modelling, the calibration 
             * of spatial/temporal models. Coding the model in pure R is usually too slow. 
             * See https://r2clr.codeplex.com/wikipage?title=Spatial-temporal%20model&referringTitle=Documentation
             * 
             * This sample is much simplified, using a canonical mathematical function, but 
             * the patterns are applicable to pracitcal cases.
             * 
             **/

            SetupHelper.SetupPath();
            using (REngine engine = REngine.CreateInstance("RDotNet")) {
                // From v1.5, REngine requires explicit initialization.
                // You can set some parameters.
                engine.Initialize();


            // http://en.wikipedia.org/wiki/Rosenbrock_function

                // the 'optimization' engine is in C#, the objective function is in R
                TestOptimCsharp(engine);
                // the optimization engine is in R, the objective function is in C#
                TestOptimR (engine);
            }
        }

        static double RosenbrockFunc (double x, double y)
        {
            var a = 1-x;
            var b = (y - x * x);
            return a * a + 100 * b * b;
        }

        static void TestOptimCsharp (REngine engine)
        {
            var rand = new Random (0);
            int n = 10000;
            double x, y, r, xb, yb, rb;
            rb = double.MaxValue; xb = yb = double.MaxValue;
            engine.Evaluate ("rosen <- function(x, y) { (1-x)**2 + 100*(y-x*x)**2 }");
            Console.WriteLine("*** Try a basic way to call the function in R ***");
            for (int i = 0; i < n; i++) {
                x = -1 + rand.NextDouble () * (3 - (-1));
                y = -1 + rand.NextDouble () * (3 - (-1));
                r = engine.Evaluate(string.Format ("rosen({0}, {1})", x, y)).AsNumeric().ToArrayFast()[0];
                if(r < rb)
                {
                    rb = r;
                    xb = x;
                    yb = y;
                }
            }
            Console.WriteLine("The best score r={0} is for x={1}, y={2}", rb, xb, yb);
            Console.WriteLine("*** Try an R function 'pointer' with a vectorized function call. Faster, if you can do it this way***");

            var f = engine.GetSymbol ("rosen").AsFunction ();
            double[] xa = new double[n], ya = new double[n];
            rand = new Random (0);
            for (int i = 0; i < n; i++) {
                xa[i] = -1 + rand.NextDouble () * (3 - (-1));
                ya[i] = -1 + rand.NextDouble () * (3 - (-1));
            }
            double[] ra = f.Invoke (new[]{ engine.CreateNumericVector(xa), engine.CreateNumericVector(ya) })
                .AsNumeric ().ToArrayFast ();
            rb = ra.Min ();
            int indBest = -1;
            for (int i = 0; i < ra.Length; i++) { // no which.min in C#. Should call R here too...
                if( ra[i] <= rb )
                    indBest = i;
            }
            Console.WriteLine("The best score r={0} is for x={1}, y={2}", rb, xa[indBest], ya[indBest]);
        }

        static void TestOptimR (REngine engine)
        {
            Console.WriteLine("*** The use case optimisation in R; engine in C# is not yet implemented");
            Console.WriteLine("*** For a similar use case see: https://r2clr.codeplex.com/wikipage?title=Spatial-temporal%20model&referringTitle=Documentation");
        }
    }
}
