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
         // Sample code used for updating the documentation at the codeplex web site.
         SetupHelper.SetupPath();
         using (REngine engine = REngine.CreateInstance("RDotNet"))
         {
            engine.Initialize();
            var e = engine.Evaluate("x <- 3");
            // You can now access x defined in the R environment.
            NumericVector x = engine.GetSymbol("x").AsNumeric();
            engine.Evaluate("y <- 1:10");
            NumericVector y = engine.GetSymbol("y").AsNumeric();

            // Invoking functions
            // invoking expand.grid directly would not work as of R.NET 1.5.5, 
            // because it has a '...' pairlist argument. We need a wrapper function.
            var expandGrid = engine.Evaluate("function(x, y) { expand.grid(x=x, y=y) }").AsFunction();
            var v1 = engine.CreateIntegerVector(new[] { 1, 2, 3 });
            var v2 = engine.CreateCharacterVector(new[] { "a", "b", "c" });
            var df = expandGrid.Invoke(new SymbolicExpression[] { v1, v2 }).AsDataFrame();
            engine.SetSymbol("cases", df);
            // Not correct: the 'y' column is a "factor". This returns "1", "1", "1", "2", "2", etc. 
            var letterCases = engine.Evaluate("cases[,'y']").AsCharacter().ToArray();
            // This returns something more intuitive for C# 
            letterCases = engine.Evaluate("as.character(cases[,'y'])").AsCharacter().ToArray();

            // invoking a whole script
            // engine.Evaluate("source('c:/src/path/to/myscript.r')");

         }
      }


      // Investigate use of the Defer method. Deprecated?
      private static string DeferedStatementRepro(REngine engine)
      {
         // Looking at the issues reported in https://rdotnet.codeplex.com/discussions/458547
         // indeed the R.NET documentation page as of 22 Sept 2013 is off as to what Defer method there is.
         // The following memory stream is a workaround out of curiosity: not something intended for ongoing use.
         // With the memory stream workaround, it bombs. 
         // Not sure whether this is because of the conversion to byte, but unlikely to be a false positive.
         string deferedStatement = "x <- 3";

         var byteArr = Array.ConvertAll(deferedStatement.ToArray(), c => Convert.ToByte(c));
         using (var stream = new MemoryStream(byteArr))
         {
            // Defer method delays an effect on the R environment.
            var e = engine.Defer(stream);
            // Error: GetSymbol method returns null at the moment.
            // NumericVector x = engine.GetSymbol("x").AsNumeric();

            // Evaluates the statement.
            e.ToArray();
         }
         return deferedStatement;
      }
   }
}
