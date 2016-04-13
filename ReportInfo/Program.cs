using System;
using RDotNet.NativeLibrary;

namespace ReportInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            string rHome = string.Empty;
            string rPath = string.Empty;
            if (args.Length > 0)
                rPath = args[0];
            if (args.Length > 1)
                rHome = args[1];

            var logInfo = NativeUtility.FindRPaths(ref rPath, ref rHome);

            Console.WriteLine("Is this process 64 bits? {0}", System.Environment.Is64BitProcess);
            Console.WriteLine(logInfo);
        }
    }
}
