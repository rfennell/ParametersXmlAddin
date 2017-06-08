using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Xml.Linq;
using System.Linq;

namespace ParametersXmlAddin_UnitTests
{
    /// <summary>
    /// Summary description for XmltTest
    /// </summary>
    [TestClass]
    public class XmlTest
    {
        public XmlTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        private string GetWorkingFolder()
        {
            //get the full location of the assembly with DaoTests in it
            string fullPath = System.Reflection.Assembly.GetAssembly(typeof(XmlTest)).Location;
            //get the folder that's in
            return Path.GetDirectoryName(fullPath);

        }

        [TestMethod]
        public void Can_generate_a_new_parameters_file_in_uppercase_with_description()
        {
            // Arrange 
            var sourceFile = Path.Combine(GetWorkingFolder(), @"testdata\web.config");
            var requiredFile = Path.Combine(GetWorkingFolder(), @"testdata\ParametersUppercaseWithDescription.xml");
            var actualFile = "results.xml";

            // act
            BlackMarble.ParametersXmlAddin.XmlGenerator.GenerateParametersXmlFile(
                sourceFile, 
                actualFile, 
                true, 
                true,
                BlackMarble.ParametersXmlAddin.OptionPageGrid.DEFAULTDELIMITER);

            // Assert
            XmlAssert.AreEqual(
                File.ReadAllText(requiredFile, System.Text.Encoding.UTF8),
                File.ReadAllText(actualFile, System.Text.Encoding.UTF8));
        }

        [TestMethod]
        public void Can_generate_a_new_parameters_file_in_mixedcase_with_description()
        {
            // Arrange 
            var sourceFile = Path.Combine(GetWorkingFolder(), @"testdata\web.config");
            var requiredFile = Path.Combine(GetWorkingFolder(), @"testdata\ParametersmixedcaseWithDescription.xml");
            var actualFile = "results.xml";

            // act
            BlackMarble.ParametersXmlAddin.XmlGenerator.GenerateParametersXmlFile(
                sourceFile, 
                actualFile, 
                false, 
                true,
                BlackMarble.ParametersXmlAddin.OptionPageGrid.DEFAULTDELIMITER);

            // Assert
            XmlAssert.AreEqual(
                File.ReadAllText(requiredFile, System.Text.Encoding.UTF8),
                File.ReadAllText(actualFile, System.Text.Encoding.UTF8));
        }

        [TestMethod]
        public void Can_generate_a_new_parameters_file_in_uppercase_with_no_description()
        {
            // Arrange 
            var sourceFile = Path.Combine(GetWorkingFolder(), @"testdata\web.config");
            var requiredFile = Path.Combine(GetWorkingFolder(), @"testdata\ParametersUppercaseWithNoDescription.xml");
            var actualFile = "results.xml";

            // act
            BlackMarble.ParametersXmlAddin.XmlGenerator.GenerateParametersXmlFile(
                sourceFile, 
                actualFile, 
                true, 
                false,
                BlackMarble.ParametersXmlAddin.OptionPageGrid.DEFAULTDELIMITER);

            // Assert
            XmlAssert.AreEqual(
                File.ReadAllText(requiredFile, System.Text.Encoding.UTF8),
                File.ReadAllText(actualFile, System.Text.Encoding.UTF8));
        }

        [TestMethod]
        public void Can_generate_a_new_parameters_file_in_mixedcase_with_no_description()
        {
            // Arrange 
            var sourceFile = Path.Combine(GetWorkingFolder(), @"testdata\web.config");
            var requiredFile = Path.Combine(GetWorkingFolder(), @"testdata\ParametersmixedcaseWithNoDescription.xml");
            var actualFile = "results.xml";

            // act
            BlackMarble.ParametersXmlAddin.XmlGenerator.GenerateParametersXmlFile(
                sourceFile, 
                actualFile, 
                false,
                false,
                BlackMarble.ParametersXmlAddin.OptionPageGrid.DEFAULTDELIMITER);

            // Assert
            XmlAssert.AreEqual(
                File.ReadAllText(requiredFile, System.Text.Encoding.UTF8),
                File.ReadAllText(actualFile, System.Text.Encoding.UTF8));
        }

        [TestMethod]
        public void Can_generate_a_new_parameters_file_in_mixedcase_with_description_swap_delimiter()
        {
            // Arrange 
            var sourceFile = Path.Combine(GetWorkingFolder(), @"testdata\web.config");
            var requiredFile = Path.Combine(GetWorkingFolder(), @"testdata\ParametersMixedcaseWithDescriptionDelimiterSwapped.xml");
            var actualFile = "results.xml";

            // act
            BlackMarble.ParametersXmlAddin.XmlGenerator.GenerateParametersXmlFile(
                sourceFile,
                actualFile,
                false,
                true,
                "##");

            Console.WriteLine(File.ReadAllText(actualFile, System.Text.Encoding.UTF8));
            // Assert
            XmlAssert.AreEqual(
                File.ReadAllText(requiredFile, System.Text.Encoding.UTF8),
                File.ReadAllText(actualFile, System.Text.Encoding.UTF8));
        }

        [TestMethod]
        public void Can_update_and_existing_parameters_file_with_uppercase_and_description()
        {
            // Arrange 
            var sourceFile = Path.Combine(GetWorkingFolder(), @"testdata\web.config");
            var requiredFile = Path.Combine(GetWorkingFolder(), @"testdata\ParametersUppercaseWithDescription.xml");
            var existingFile = Path.Combine(GetWorkingFolder(), @"testdata\TestFile.XML");
            File.Copy(Path.Combine(GetWorkingFolder(), @"testdata\ParametersMissingEntries.XML"), existingFile,true);

            // act
            BlackMarble.ParametersXmlAddin.XmlGenerator.UpdateParametersXmlFile(
                sourceFile, 
                existingFile, 
                true, 
                true,
                BlackMarble.ParametersXmlAddin.OptionPageGrid.DEFAULTDELIMITER);

            // assert
            XmlAssert.AreEqual(
                File.ReadAllText(requiredFile, System.Text.Encoding.UTF8),
                File.ReadAllText(existingFile, System.Text.Encoding.UTF8));
        }

        [TestMethod]
        public void Can_update_and_existing_parameters_file_with_uppercase_and_description_swap_delimiter()
        {
            // Arrange 
            var sourceFile = Path.Combine(GetWorkingFolder(), @"testdata\web.config");
            var requiredFile = Path.Combine(GetWorkingFolder(), @"testdata\ParametersUppercaseWithDescriptionDelimiterSwapper - Update.xml");
            var existingFile = Path.Combine(GetWorkingFolder(), @"testdata\TestFile.XML");
            File.Copy(Path.Combine(GetWorkingFolder(), @"testdata\ParametersMissingEntries.XML"), existingFile, true);

            // act
            BlackMarble.ParametersXmlAddin.XmlGenerator.UpdateParametersXmlFile(
                sourceFile,
                existingFile,
                true,
                true,
                "##");

            // assert
            XmlAssert.AreEqual(
                File.ReadAllText(requiredFile, System.Text.Encoding.UTF8),
                File.ReadAllText(existingFile, System.Text.Encoding.UTF8));
        }

        [TestMethod]
        public void Can_update_and_existing_parameters_file_with_original_transformmixedcase_and_description()
        {
            // Arrange 
            var sourceFile = Path.Combine(GetWorkingFolder(), @"testdata\web.config");
            var requiredFile = Path.Combine(GetWorkingFolder(), @"testdata\ParametersMixedcaseWithDescription - Update.xml");
            var existingFile = Path.Combine(GetWorkingFolder(), @"testdata\TestFile.XML");
            File.Copy(Path.Combine(GetWorkingFolder(), @"testdata\ParametersMissingEntries.XML"), existingFile, true);

            // act
            BlackMarble.ParametersXmlAddin.XmlGenerator.UpdateParametersXmlFile(
                sourceFile, 
                existingFile, 
                false, 
                true,
                BlackMarble.ParametersXmlAddin.OptionPageGrid.DEFAULTDELIMITER);

            // assert
            XmlAssert.AreEqual(
                File.ReadAllText(requiredFile, System.Text.Encoding.UTF8),
                File.ReadAllText(existingFile, System.Text.Encoding.UTF8));

        }

        [TestMethod]
        public void Can_update_and_existing_parameters_file_with_uppercase_and_no_description()
        {
            // Arrange 
            var sourceFile = Path.Combine(GetWorkingFolder(), @"testdata\web.config");
            var requiredFile = Path.Combine(GetWorkingFolder(), @"testdata\ParametersUppercaseWithNoDescription - update.xml");
            var existingFile = Path.Combine(GetWorkingFolder(), @"testdata\TestFile.XML");
            File.Copy(Path.Combine(GetWorkingFolder(), @"testdata\ParametersMissingEntries.XML"), existingFile, true);

            // act
            BlackMarble.ParametersXmlAddin.XmlGenerator.UpdateParametersXmlFile(
                sourceFile, 
                existingFile, 
                true, 
                false,
                BlackMarble.ParametersXmlAddin.OptionPageGrid.DEFAULTDELIMITER);

            // assert
            XmlAssert.AreEqual(
                File.ReadAllText(requiredFile, System.Text.Encoding.UTF8),
                File.ReadAllText(existingFile, System.Text.Encoding.UTF8));
        }

        [TestMethod]
        public void Can_update_and_existing_parameters_file_with_original_transformmixedcase_and_no_description()
        {
            // Arrange 
            var sourceFile = Path.Combine(GetWorkingFolder(), @"testdata\web.config");
            var requiredFile = Path.Combine(GetWorkingFolder(), @"testdata\ParametersMixedcaseWithNoDescription - Update.xml");
            var existingFile = Path.Combine(GetWorkingFolder(), @"testdata\TestFile.XML");
            File.Copy(Path.Combine(GetWorkingFolder(), @"testdata\ParametersMissingEntries.XML"), existingFile, true);

            // act
            BlackMarble.ParametersXmlAddin.XmlGenerator.UpdateParametersXmlFile(
                sourceFile, 
                existingFile, 
                false, 
                false,
                BlackMarble.ParametersXmlAddin.OptionPageGrid.DEFAULTDELIMITER);

            // assert
            XmlAssert.AreEqual(
                File.ReadAllText(requiredFile, System.Text.Encoding.UTF8),
                File.ReadAllText(existingFile, System.Text.Encoding.UTF8));

        }


    }
}
