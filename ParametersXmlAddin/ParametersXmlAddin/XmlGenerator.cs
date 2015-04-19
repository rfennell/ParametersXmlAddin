using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace BlackMarble.ParametersXmlAddin
{
    internal static class XmlGenerator
    {
        /// <summary>
        /// Generates a new file from a config file using the XSLT transform stored as an embedded resource
        /// </summary>
        /// <param name="inFile">The source web.config file</param>
        /// <param name="outFile">The target parameters.xml</param>
        internal static void GenerateParametersXmlFile(string inFile, string outFile)
        {
            using (var strm = Assembly.GetExecutingAssembly().GetManifestResourceStream("BlackMarble.ParametersXmlAddin.Resources.ParametersTransform.xslt"))
            {
                using (XmlReader reader = XmlReader.Create(strm))
                {
                    XslCompiledTransform transform = new XslCompiledTransform();
                    transform.Load(reader);

                    GenerateParametersXmlFile(inFile, outFile, transform);
                }
            }
        }

        /// <summary>
        /// Generates a new file from a config file using the XSLT transform stored as an embedded resource
        /// </summary>
        /// <param name="inFile">The source web.config file</param>
        /// <param name="outFile">The target parameters.xml</param>
        /// <param name="transform">The transform loaded to apply to the files</param>
        private static void GenerateParametersXmlFile(string inFile, string outFile, XslCompiledTransform transform)
        {
            XPathDocument xPathDoc = new XPathDocument(inFile);
            using (XmlTextWriter writer = new XmlTextWriter(outFile, System.Text.Encoding.UTF8))
            {
                transform.Transform(xPathDoc, null, writer);
            }
        }
    }
}
