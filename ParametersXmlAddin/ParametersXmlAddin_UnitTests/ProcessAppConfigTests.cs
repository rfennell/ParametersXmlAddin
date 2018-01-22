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
    public class ProcessAppConfigTests
    {
        public ProcessAppConfigTests()
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

        [TestMethod]
        public void Can_generate_a_new_parameters_file_in_uppercase_with_description()
        {
            // Arrange 
            var sourceFile = Path.Combine(HelperMethods.GetWorkingFolder(), @"testdata\dummy.app.config");
            var requiredFile = Path.Combine(HelperMethods.GetWorkingFolder(), @"testdata\ParametersUppercaseWithDescription - AppConfig.XML");
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
    }
}
