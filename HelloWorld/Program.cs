using RDotNet;
using RDotNet.NativeLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorld
{
   class Program
   {
      static void Main(string[] args)
      {
         REngine.SetEnvironmentVariables();
         REngine engine = REngine.GetInstance();
         engine.Initialize(); // required since v1.5
         CharacterVector charVec = engine.CreateCharacterVector(new[] { "Hello, R world!, .NET speaking" });
         engine.SetSymbol("greetings", charVec);
         engine.Evaluate("str(greetings)"); // print out in the console
         string[] a = engine.Evaluate("'Hi there .NET, from the R engine'").AsCharacter().ToArray();
         Console.WriteLine("R answered: '{0}'", a[0]);
         Console.WriteLine("Press any key to exit the program");
         Console.ReadKey();
         engine.Dispose();
      }
   }
}
