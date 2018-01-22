﻿using System;
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
        /// Replaces the delimiter in use in the template
        /// </summary>
        /// <param name="fileName">The file to update</param>
        /// <param name="oldDelimiter">Old delimiter - usually __</param>
        /// <param name="newDelimiter">What ever you need</param>
        /// <returns></returns>
        internal static void SwapDelimiter(string fileName, string oldDelimiter, string newDelimiter)
        {
            if (!oldDelimiter.Equals(newDelimiter))
            {
                var fileContent = File.ReadAllText(fileName, System.Text.Encoding.UTF8);
                File.WriteAllText(
                    fileName,
                    fileContent.Replace(oldDelimiter, newDelimiter),
                    System.Text.Encoding.UTF8);
            }
        }

        /// <summary>
        /// Gets the name of the XSLT transform resource
        /// </summary>
        /// <param name="forceUppercase">Set the token as upper case</param>
        /// <param name="addDescription">Add a description</param>
        /// <returns>The file name</returns>
        private static string GetTransformresourceName(bool forceUppercase, bool addDescription)
        {
            if (forceUppercase == true)
            {
                if (addDescription == true)
                {
                    return "BlackMarble.ParametersXmlAddin.Resources.ParametersUppercaseTransform.xslt";
                }
                else
                {
                    return "BlackMarble.ParametersXmlAddin.Resources.ParametersUppercaseNoDescription.xslt";
                }
            }
            else
            {
                if (addDescription == true)
                {
                    return "BlackMarble.ParametersXmlAddin.Resources.ParametersPreserveTransform.xslt";
                }
                else
                {
                    return "BlackMarble.ParametersXmlAddin.Resources.ParametersPreserveTransformNoDescription.xslt";
                }
            }

        }

        /// <summary>
        /// Generates a new file from a config file using the XSLT transform stored as an embedded resource
        /// </summary>
        /// <param name="inFile">The source app.config or web.config file</param>
        /// <param name="outFile">The target parameters.xml</param>
        /// <param name="forceUppercase">Set the token as upper case</param>
        /// <param name="addDescription">Add a description</param>
        /// <param name="delimiter">Demlimter override</param>

        internal static void GenerateParametersXmlFile(
            string inFile,
            string outFile,
            bool forceUppercase,
            bool addDescription,
            string delimiter)
        {
            var transformFile = XmlGenerator.GetTransformresourceName(forceUppercase, addDescription);
            using (var strm = Assembly.GetExecutingAssembly().GetManifestResourceStream(transformFile))
            {
                using (XmlReader reader = XmlReader.Create(strm))
                {
                    XslCompiledTransform transform = new XslCompiledTransform();
                    transform.Load(reader);

                    GenerateParametersXmlFile(inFile, outFile, transform);
                }
            }
            XmlGenerator.SwapDelimiter(outFile, OptionPageGrid.DEFAULTDELIMITER, delimiter);
        }

        /// <summary>
        /// Generates a new file from a config file using the XSLT transform stored as an embedded resource
        /// This replaces any existing parameters file
        /// </summary>
        /// <param name="configPath">The source app.config or web.config file</param>
        /// <param name="parametersXmlPath">The target parameters.xml</param>
        /// <param name="transform">The transform loaded to apply to the files</param>
        private static void GenerateParametersXmlFile(string configPath, string parametersXmlPath, XslCompiledTransform transform)
        {
            XPathDocument xPathDoc = new XPathDocument(configPath);
         
            // Create the XsltArgumentList.
            XsltArgumentList xslArg = new XsltArgumentList();
            // Create a parameter which represents the file name scope
            xslArg.AddParam("file", "", Path.GetFileName(configPath));

            using (XmlTextWriter writer = new XmlTextWriter(parametersXmlPath, System.Text.Encoding.UTF8))
            {
                transform.Transform(xPathDoc, xslArg, writer);
            }
        }

        /// <summary>
        /// Updates an existing file from a config file using the XSLT transform stored as an embedded resource
        /// Any new entries are added, any existing ones are not touched, neither updated or removed
        /// </summary>
        /// <param name="configPath">The source app.config or web.config file</param>
        /// <param name="parametersXmlPath">The target parameters.xml</param>
        /// <param name="forceUppercase">Set the token as upper case</param>
        /// <param name="addDescription">Add a description</param>
        /// <param name="delimiter">Delimiter to be used for replacement</param>

        internal static void UpdateParametersXmlFile(
            string configPath,
            string parametersXmlPath,
            bool forceUppercase,
            bool addDescription,
            string delimiter)
        {
            UpdateParametersXmlFile(
                configPath, 
                parametersXmlPath, 
                System.IO.Path.GetTempFileName(), 
                forceUppercase, 
                addDescription,
                delimiter);
        }

        /// <summary>
        /// Updates an existing file from a config file using the XSLT transform stored as an embedded resource
        /// Any new entries are added, any existing ones are not touched, neither updated or removed
        /// </summary>
        /// <param name="configPath">The source app.config or web.config file</param>
        /// <param name="existingParametersXmlPath">The target parameters.xml</param>
        /// <param name="newParametersXmlPath">The temporary location to generate the file into</param>
        /// <param name="forceUppercase">Set the token as upper case</param>
        /// <param name="addDescription">Add a description</param>
        /// <param name="delimiter">Delimiter to be used for replacement</param>

        internal static void UpdateParametersXmlFile(
            string configPath,
            string existingParametersXmlPath,
            string newParametersXmlPath,
            bool forceUppercase,
            bool addDescription,
            string delimiter)

        {
            var transformFile = XmlGenerator.GetTransformresourceName(forceUppercase, addDescription);

            using (var strm = Assembly.GetExecutingAssembly().GetManifestResourceStream(transformFile))
            {
                using (XmlReader reader = XmlReader.Create(strm))
                {
                    XslCompiledTransform transform = new XslCompiledTransform();
                    transform.Load(reader);

                    UpdateParametersXmlFile(configPath, existingParametersXmlPath, newParametersXmlPath, transform);
                }
            }
            XmlGenerator.SwapDelimiter(existingParametersXmlPath, OptionPageGrid.DEFAULTDELIMITER, delimiter);
        }

        /// <summary>
        /// Updates an existing file from a config file using the XSLT transform stored as an embedded resource
        /// Any new entries are added, any existing ones are not touch, neither updated or removed
        /// </summary>
        /// <param name="configPath">The source app.config or web.config file</param>
        /// <param name="parametersXmlPath">The target parameters.xml</param>
        /// <param name="transform">The transform loaded to apply to the files</param>

        internal static void UpdateParametersXmlFile(string configPath, string existingParametersXmlPath, string newParametersXmlPath, XslCompiledTransform transform)
        {
            GenerateParametersXmlFile(configPath, newParametersXmlPath, transform);

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

        /// <summary>
        /// Loads a XML file and normalizes the contents
        /// </summary>
        /// <param name="fileName">The file</param>
        /// <returns>Normalised block of XML</returns>
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
