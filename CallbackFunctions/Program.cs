using DynamicInterop;
using RDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CallbackFunctions
{
    /// <summary>
    /// A sample code that was written to answer the question http://rdotnet.codeplex.com/discussions/646729
    /// </summary>
    class Program
    {
        public static void Main(string[] args)
        {
            REngine.SetEnvironmentVariables();
            REngine engine = REngine.GetInstance();
            rdotnet_discussions_646729(engine);
            // you should always dispose of the REngine properly.
            // After disposing of the engine, you cannot reinitialize nor reuse it
            engine.Dispose();
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void ProgressNotificationHandler([In] [MarshalAs(UnmanagedType.LPStr)] string buffer, double percentage);

        private class CallBackHandlers
        {
            public void ProcessProgress(string buffer, double percentage)
            {
                Console.WriteLine(string.Format("C# progress handler: at {0}% - {1}", percentage, buffer));
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        class TestCallback
        {
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ProgressNotificationHandler MyHandler;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void register_default_progress_handler(ProgressNotificationHandler delegatePtr);

        static void rdotnet_discussions_646729(REngine engine)
        {
            var setup = @"library(rdotnetsamples)
rdotnetsamples::register_default_progress_handler()
";
            engine.Evaluate(setup);
            var myRFunction = @"
my_r_calculation <- function()
{
  for (i in seq(0, 100, by=20)) {
    rdotnetsamples::broadcast_progress_update(paste0('Some Update Message for ', i), i);
  }
}
";
            engine.Evaluate(myRFunction);
            engine.Evaluate("my_r_calculation()");

            var unixDllPath = engine.Evaluate("getLoadedDLLs()$rdotnetsamples[['path']]").AsCharacter()[0];
            var dllPath = unixDllPath.Replace("/", "\\");
            var dll = new DynamicInterop.UnmanagedDll(dllPath);
            TestCallback cback = new TestCallback();
            CallBackHandlers cbh = new CallBackHandlers();
            cback.MyHandler = cbh.ProcessProgress;

            string cFunctionRegisterCallback = "register_progress_handler";
            register_default_progress_handler registerHandlerFun = dll.GetFunction<register_default_progress_handler>(cFunctionRegisterCallback);
            registerHandlerFun(cback.MyHandler);

            Console.WriteLine();
            Console.WriteLine("After registering the callback with a function pointer to a C# function:");
            Console.WriteLine();
            engine.Evaluate("my_r_calculation()");

        }
    }
}
