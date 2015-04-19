using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ParametersXmlAddin_UnitTests
{
    internal static class XmlHelper
    {
        /// <summary>
        /// Compares two string of XML 
        /// </summary>
        /// <param name="string1">Xml Block</param>
        /// <param name="string2">Xml Block</param>
        /// <returns>True if the same</returns>
        internal static bool StringsAreEquivalent(string string1, string string2)
        {
            var docA = XDocument.Parse(string1);
            var docB = XDocument.Parse(string2);
            return XNode.DeepEquals(docA.Root, docB.Root);
        }

        /// <summary>
        /// Compares two Xml Files
        /// </summary>
        /// <param name="file1">Xml file</param>
        /// <param name="file2">Xml file</param>
        /// <returns>True if the same</returns>
        internal static bool FilesAreEquivalent(string file1, string file2)
        {
            return StringsAreEquivalent(
                new StreamReader(file1, System.Text.Encoding.UTF8).ReadToEnd(),
                new StreamReader(file2, System.Text.Encoding.UTF8).ReadToEnd());
        }
    }
}
