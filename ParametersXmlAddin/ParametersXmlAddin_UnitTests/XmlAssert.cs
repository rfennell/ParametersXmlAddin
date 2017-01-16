using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ParametersXmlAddin_UnitTests
{
    public static class XmlAssert
    {

        /// <summary>
        /// Compares two string of XML 
        /// </summary>
        /// <param name="expected">Xml Block</param>
        /// <param name="actual">Xml Block</param>
        /// <returns>True if the same</returns>
        public static void AreEqual(string expected, string actual)
        {
            var docA = XDocument.Parse(expected);
            var docB = XDocument.Parse(actual);

            XmlAssert.AreEqual(docA.Root, docB.Root);
        }


        public static void AreEqual(
            XElement expected,
            XElement actual)
        {

            // This is char by char as it gives me an index where the failure is
            // if you just compare the whole string it takes forever to hunt down the issue
            var e = BlackMarble.ParametersXmlAddin.XmlGenerator.Normalize(expected).ToString();
            var a = BlackMarble.ParametersXmlAddin.XmlGenerator.Normalize(actual).ToString();
            Console.WriteLine("\nXmlAssert.AreEqual: Characters compared, last character is the one with a mismatch");
            for (int i = 0; i < e.Length; i++)
            {
                Console.Write(e[i]);
                Assert.AreEqual(e[i], a[i], "Mismatch at Index {0} of {1} characters");
            }

        }

    }
}
