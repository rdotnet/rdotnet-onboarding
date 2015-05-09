using System;
using RDotNet;

namespace ReportInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            REngine.SetEnvironmentVariables();
            // and/or
            // REngine engine = REngine.GetInstance();


            var logInfo = RDotNet.NativeLibrary.NativeUtility.SetEnvironmentVariablesLog;

            Console.WriteLine("Is this process 64 bits? {0}", System.Environment.Is64BitProcess);
            Console.WriteLine(logInfo);

            // engine.Dispose();

        }
    }
}
