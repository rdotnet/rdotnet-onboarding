using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDotNet.NativeLibrary;

namespace RDotNetSetup
{
    public class SetupHelper
    {
        public static void SetupPath()
        {
            // as of 2013-10, there is a helper that should work for all platforms:
            NativeUtility.SetEnvironmentVariables ();
            // The following sample code is left as a fallback option if you have unforeseen issues,
            // but you should report issues on the R.NET discussion list.
//            var oldPath = System.Environment.GetEnvironmentVariable("PATH");
//            var rPath = System.Environment.Is64BitProcess ? @"C:\Program Files\R\R-3.0.2\bin\x64" : @"C:\Program Files\R\R-3.0.2\bin\i386";
//            if (Directory.Exists(rPath) == false)
//                throw new DirectoryNotFoundException(string.Format("Could not found the specified path to the directory containing R.dll: {0}", rPath));
//            var newPath = string.Format("{0}{1}{2}", rPath, System.IO.Path.PathSeparator, oldPath);
//            System.Environment.SetEnvironmentVariable("PATH", newPath);
        }
    }
}

