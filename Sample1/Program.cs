using System;
using System.Linq;
using RDotNet;

namespace Sample1
{
   class Program
   {
      static void Main(string[] args)
      {
         REngine.SetEnvironmentVariables();
         REngine engine = REngine.GetInstance();
         // REngine requires explicit initialization.
         // You can set some parameters.
         engine.Initialize();

         // .NET Framework array to R vector.
         NumericVector group1 = engine.CreateNumericVector(new double[] { 30.02, 29.99, 30.11, 29.97, 30.01, 29.99 });
         engine.SetSymbol("group1", group1);
         // Direct parsing from R script.
         NumericVector group2 = engine.Evaluate("group2 <- c(29.89, 29.93, 29.72, 29.98, 30.02, 29.98)").AsNumeric();

         // Test difference of mean and get the P-value.
         GenericVector testResult = engine.Evaluate("t.test(group1, group2)").AsList();
         double p = testResult["p.value"].AsNumeric().First();

         Console.WriteLine("Group1: [{0}]", string.Join(", ", group1));
         Console.WriteLine("Group2: [{0}]", string.Join(", ", group2));
         Console.WriteLine("P-value = {0:0.000}", p);

         // you should always dispose of the REngine properly.
         // After disposing of the engine, you cannot reinitialize nor reuse it
         engine.Dispose();

      }
   }
}
