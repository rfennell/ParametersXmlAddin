using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace BlackMarble.ParametersXmlAddin
{
    internal static class XmlGenerator
    {
        /// <summary>
        /// The resource that holds the XSLT template
        /// </summary>
        private const string XSLTFILENAME = "BlackMarble.ParametersXmlAddin.Resources.ParametersTransform.xslt";

        /// <summary>
        /// Generates a new file from a config file using the XSLT transform stored as an embedded resource
        /// </summary>
        /// <param name="inFile">The source web.config file</param>
        /// <param name="outFile">The target parameters.xml</param>
        internal static void GenerateParametersXmlFile(string inFile, string outFile)
        {
            using (var strm = Assembly.GetExecutingAssembly().GetManifestResourceStream(XSLTFILENAME))
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
        /// This replaces any existing parameters file
        /// </summary>
        /// <param name="webConfigPath">The source web.config file</param>
        /// <param name="parametersXmlPath">The target parameters.xml</param>
        /// <param name="transform">The transform loaded to apply to the files</param>
        private static void GenerateParametersXmlFile(string webConfigPath, string parametersXmlPath, XslCompiledTransform transform)
        {
            XPathDocument xPathDoc = new XPathDocument(webConfigPath);
            using (XmlTextWriter writer = new XmlTextWriter(parametersXmlPath, System.Text.Encoding.UTF8))
            {
                transform.Transform(xPathDoc, null, writer);
            }
        }

        /// <summary>
        /// Updates an existing file from a config file using the XSLT transform stored as an embedded resource
        /// Any new entries are added, any existing ones are not touch, neither updated or removed
        /// </summary>
        /// <param name="webConfigPath">The source web.config file</param>
        /// <param name="parametersXmlPath">The target parameters.xml</param>

        internal static void UpdateParametersXmlFile(string webConfigPath, string parametersXmlPath)
        {
            UpdateParametersXmlFile(webConfigPath, parametersXmlPath, System.IO.Path.GetTempFileName());
        }

        /// <summary>
        /// Updates an existing file from a config file using the XSLT transform stored as an embedded resource
        /// Any new entries are added, any existing ones are not touch, neither updated or removed
        /// </summary>
        /// <param name="webConfigPath">The source web.config file</param>
        /// <param name="existingParametersXmlPath">The target parameters.xml</param>
        /// <param name="newParametersXmlPath">The temporary location to generate the file into</param>

        internal static void UpdateParametersXmlFile(string webConfigPath, string existingParametersXmlPath, string newParametersXmlPath)
        {
            using (var strm = Assembly.GetExecutingAssembly().GetManifestResourceStream(XSLTFILENAME))
            {
                using (XmlReader reader = XmlReader.Create(strm))
                {
                    XslCompiledTransform transform = new XslCompiledTransform();
                    transform.Load(reader);

                    UpdateParametersXmlFile(webConfigPath, existingParametersXmlPath, newParametersXmlPath, transform);
                }
            }
        }

        /// <summary>
        /// Updates an existing file from a config file using the XSLT transform stored as an embedded resource
        /// Any new entries are added, any existing ones are not touch, neither updated or removed
        /// </summary>
        /// <param name="webConfigPath">The source web.config file</param>
        /// <param name="parametersXmlPath">The target parameters.xml</param>
        /// <param name="transform">The transform loaded to apply to the files</param>

        internal static void UpdateParametersXmlFile(string webConfigPath, string existingParametersXmlPath, string newParametersXmlPath, XslCompiledTransform transform)
        {
            GenerateParametersXmlFile(webConfigPath, newParametersXmlPath, transform);

            // what we think the xml should be
            var generatedXml = LoadNormalizedXml(newParametersXmlPath);

            // what it is currently
            var existingXml = LoadNormalizedXml(existingParametersXmlPath);

            // compare each element
            foreach (XElement element in generatedXml.Elements())
            {
                if (existingXml.Elements().SingleOrDefault(e => e.Attribute("name").Value == element.Attribute("name").Value) == null)
                {
                    // can't find the elememt in the target as add it
                    Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "BlackMarble.ParametersXmlAddin.XmlGenerator: Adding element {0}", element.Attribute("name").Value));
                    existingXml.Add(element);
                }
            }

            // normalise it to make it neat after everything is update
            var updatedXml = Normalize(existingXml);

            // write it out by swapping the files
            updatedXml.Save(existingParametersXmlPath);
        }

        private static XElement LoadNormalizedXml(string fileName)
        {
            using (var reader = new StreamReader(fileName, System.Text.Encoding.UTF8))
            {
                return Normalize(XDocument.Parse(reader.ReadToEnd()).Root);
            }
        }


        /// <summary>
        /// Sorts the XElement to allow an easy means to compare. To allow the sorting of elements of the same
        /// type we provide an extra attribute of 'name' to provide a secondary search
        /// </summary>
        /// <param name="element">The root element</param>
        /// <returns>An ordered object</returns>
        public static XElement Normalize(XElement element)
        {
            return Normalize(element, "name");
        }

        /// <summary>
        /// Sorts the XElement to allow an easy means to compare. To allow the sorting of elements of the same
        /// type we provide an extra attribute to provide a secondary search
        /// </summary>
        /// <param name="element">The root element</param>
        /// <returns>An order object</returns>
        /// <param name="extraAttribute">The extra attribute to check (if present)</param>
        /// <returns>An ordered object</returns>
        public static XElement Normalize(XElement element, string extraAttribute)
        {
            if (element.HasElements)
            {
                return new XElement(
                    element.Name,
                    element.Attributes()
                           .OrderBy(a => a.Name.ToString()),
                    element.Elements()
                           .OrderBy(a => a.Name.ToString())
                           .ThenBy(a => a.Attribute(extraAttribute) != null ? a.Attribute(extraAttribute).Value : string.Empty)
                           .Select(e => Normalize(e, extraAttribute)));
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
