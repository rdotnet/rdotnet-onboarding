using RDotNet;
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
         using (REngine engine = REngine.GetInstance())
         {
            var e = engine.Evaluate("x <- 3");
            // You can now access x defined in the R environment.
            NumericVector x = engine.GetSymbol("x").AsNumeric();
            engine.Evaluate("y <- 1:10");
            NumericVector y = engine.GetSymbol("y").AsNumeric();

            // Invoking functions; Previously you may have needed custom function definitions
            var myFunc = engine.Evaluate("function(x, y) { expand.grid(x=x, y=y) }").AsFunction();
            var v1 = engine.CreateIntegerVector(new[] { 1, 2, 3 });
            var v2 = engine.CreateCharacterVector(new[] { "a", "b", "c" });
            var df = myFunc.Invoke(new SymbolicExpression[] { v1, v2 }).AsDataFrame();

            // As of R.NET 1.6, more function call syntaxes are supported.
            var expandGrid = engine.Evaluate("expand.grid").AsFunction();
            var d = new Dictionary<string, SymbolicExpression>();
            d["x"] = v1;
            d["y"] = v2;
            df = expandGrid.Invoke(d).AsDataFrame();

            // querying data frames
            engine.SetSymbol("cases", df);
            // As of R.NET 1.6, factor to character expressions work consistently with R

            var letterCases = engine.Evaluate("cases[,'y']").AsCharacter().ToArray();
            // "a","a","a","b","b","b", etc. Same as as.character(cases[,'y']) in R
            // This used to return  "1", "1", "1", "2", "2", etc. with R.NET 1.5.5

            // Equivalent:
            letterCases = df[1].AsCharacter().ToArray();
            letterCases = df["y"].AsCharacter().ToArray();

            // Accessing items by two dimensional indexing
            string s = (string)df[1, 1]; // "a"
            s = (string)df[3, 1]; // "a"
            s = (string)df[3, "y"]; // "b"
            // s = (string)df["4", "y"]; // fails because there are no row names
            df[3, "y"] = "a";
            s = (string)df[3, "y"]; // "a"
            df[3, "y"] = "d";
            s = (string)df[3, "y"]; // null, because we have an <NA> string in R

            // invoking a whole script
            // engine.Evaluate("source('c:/src/path/to/myscript.r')");

            // TODO
            // Date-time objects

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
