using RDotNet;
using RDotNetSetup;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample2
{
    class Program
    {
        static void Main(string[] args)
        {
            SetupHelper.SetupPath();
            using (REngine engine = REngine.CreateInstance("RDotNet"))
            {
                // Looking at the issues reported in https://rdotnet.codeplex.com/discussions/458547
                // indeed the R.NET documentation page as of 22 Sept 2013 is off as to what Defer method there is.
                // The following memory stream is a workaround out of curiosity: not something intended for ongoing use.
                // With the memory stream workaround, it bombs. 
                // Not sure whether this is because of the conversion to byte, but unlikely to be a false positive.
                string deferedStatement = "x <- 3";
                var byteArr = Array.ConvertAll(deferedStatement.ToArray(), x => Convert.ToByte(x));
                using (var stream = new MemoryStream(byteArr))
                {
                    // Defer method delays an effect on the R environment.
                    var e = engine.Defer(stream);
                    // Error: GetSymbol method returns null at the moment.
                    // NumericVector x = engine.GetSymbol("x").AsNumeric();

                    // Evaluates the statement.
                    e.ToArray();
                    // You can now access x defined in the R environment.
                    NumericVector x = engine.GetSymbol("x").AsNumeric();

                    // Evaluate method evaluates the statement as soon as you call it.
                    engine.Evaluate("y <- 1:10");
                    NumericVector y = engine.GetSymbol("y").AsNumeric();
                }
            }
        }
    }
}
