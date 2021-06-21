using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace BlackMarble.ParametersXmlAddin
{
     public class OptionPageGrid : DialogPage
    {
        public const string DEFAULTDELIMITER = "__";
        private bool makeTokenUpperCase = true;
        private bool addDefaultDescription = true;
        private string delimiter = DEFAULTDELIMITER;

        [Category("Options")]
        [DisplayName("Make Tokens Uppercase")]
        [Description("If True adds token in form __TOKEN__, if false the token refects the variable name")]
        public bool MakeTokenUpperCase 
        {
            get { return makeTokenUpperCase; }
            set { makeTokenUpperCase = value; }
        }

        [Category("Options")]
        [DisplayName("Add a default description")]
        [Description("If True adds a default description to the parameters.xml file")]
        public bool AddDefaultDescription
        {
            get { return addDefaultDescription; }
            set { addDefaultDescription = value; }
        }

        [Category("Options")]
        [DisplayName("Replacement Delimiter")]
        [Description("The delimiter to be used to find items to be replaced in later processing")]
        public string Delimiter
        {
            get { return delimiter; }
            set { delimiter = value; }
        }

    }
}
