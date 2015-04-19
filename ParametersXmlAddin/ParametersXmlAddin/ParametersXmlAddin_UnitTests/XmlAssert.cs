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
            var e = Normalize(expected).ToString();
            var a = Normalize(actual).ToString();
            for (int i = 0; i < e.Length; i++)
            {
                Assert.AreEqual(e[i], a[i], "Mismatch at Index {0}", i);
            }

        }

        private static XElement Normalize(XElement element)
        {
            if (element.HasElements)
            {
                return new XElement(
                    element.Name,
                    element.Attributes()
                           .OrderBy(a => a.Name.ToString()),
                    element.Elements()
                           .OrderBy(a => a.Name.ToString())
                           .Select(e => Normalize(e)));
            }

            if (element.IsEmpty)
            {
                return new XElement(
                    element.Name,
                    element.Attributes()
                           .OrderBy(a => a.Name.ToString()));
            }

            return new XElement(element.Name, element.Attributes()
                .OrderBy(a => a.Name.ToString()), element.Value);
        }
    }
}
