using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace BlackMarble.ParametersXmlAddin
{
    internal static class XmlGenerator
    {
        internal static void GenerateParametersXmlFile(string sourceFile, string transformFile, string actualFile)
        {
            XPathDocument xPathDoc = new XPathDocument(sourceFile);
            XslCompiledTransform xslTrans = new XslCompiledTransform();
            xslTrans.Load(transformFile);
            using (XmlTextWriter writer = new XmlTextWriter(actualFile, System.Text.Encoding.UTF8))
            {
                xslTrans.Transform(xPathDoc, null, writer);
            }
        }
    }
}
