using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParametersXmlAddin_UnitTests
{
    internal class HelperMethods
    {
        internal static string GetWorkingFolder()
        {
            //get the full location of the assembly with DaoTests in it
            string fullPath = System.Reflection.Assembly.GetAssembly(typeof(ProcessWebConfigTests)).Location;
            //get the folder that's in
            return Path.GetDirectoryName(fullPath);
        }
    }
}
