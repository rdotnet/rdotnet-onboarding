using RDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRCode
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(@"Usage:");
                Console.WriteLine(@"SourceRCode.exe c:/path/to/source.r");
                Console.WriteLine(@" (!) Do note that you should use forward slashes (simpler than backslashes here)");
                return;
            }
            REngine.SetEnvironmentVariables();
            REngine engine = REngine.GetInstance();
            engine.Evaluate("source('"+ args[0] + "')");
            /*
             * // Say your R file contains:

sqr <- function(x) {
    return(x*x)
}
             */
            Console.WriteLine("By default, autoprint on the console");
            double[] a = engine.Evaluate("sqr(0:5)").AsNumeric().ToArray();

            Console.WriteLine("However, for manipulation of larger data, autoprint on the console is probably not a good idea");
            engine.AutoPrint = false;
            a = engine.Evaluate("sqr(0:1000)").AsNumeric().ToArray();

            Console.WriteLine("Length(a) is "+a.Length+", but the vector has not been written out to the console");                
            Console.WriteLine("Press any key to exit the program");
            Console.ReadKey();
            engine.Dispose();
        }
    }
}
