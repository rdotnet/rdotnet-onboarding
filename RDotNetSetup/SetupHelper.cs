using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDotNetSetup
{
    public class SetupHelper
    {
        public static void SetupPath()
        {
            var oldPath = System.Environment.GetEnvironmentVariable("PATH");

            var rPath = System.Environment.Is64BitProcess ? @"C:\Program Files\R\R-3.0.1\bin\x64" : @"C:\Program Files\R\R-3.0.1\bin\i386";

            if (Directory.Exists(rPath) == false)
                throw new DirectoryNotFoundException(string.Format("Could not found the specified path to the directory containing R.dll: {0}", rPath));

            var newPath = string.Format("{0}{1}{2}", rPath, System.IO.Path.PathSeparator, oldPath);
            System.Environment.SetEnvironmentVariable("PATH", newPath);
        }
    }
}
