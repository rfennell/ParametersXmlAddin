using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using System.IO;

namespace BlackMarble.ParametersXmlAddin
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidParametersXmlAddinPkgString)]
    // You may wonder what that GUID value is, well in this case it represents the 
    // UICONTEXT_SolutionExists constant, which means the the package will auto-load when a solution exists 
    // (so when we create a new one or load one). 
    [ProvideAutoLoad("{f1536ef8-92ec-443c-9ed7-fdadf150da82}")]
    public sealed class ParametersXmlAddinPackage : Package
    {
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public ParametersXmlAddinPackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "BlackMarble.ParametersXmlAddin: Entering constructor for: {0}", this.ToString()));
        }



        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "BlackMarble.ParametersXmlAddin: Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                // Create the command for the menu item.
                var menuCommandID = new CommandID(GuidList.guidParametersXmlAddinCmdSet, (int)PkgCmdIDList.cmdidGenerateParametersXmlFile);
                // Is an OleMenuCommand as opposed to MenuCommand to get assess to BeforeQueryStatus
                var menuItem = new OleMenuCommand(MenuItemCallback, menuCommandID);
                menuItem.BeforeQueryStatus += menuItem_BeforeQueryStatus;
                mcs.AddCommand(menuItem);
            }
        }

        #endregion

        /// <summary>
        /// Called when the right click occures
        /// Used to set if the menu is seen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void menuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "BlackMarble.ParametersXmlAddin: Entering BeforeQueryStatus()"));

            var myCommand = sender as OleMenuCommand;
            myCommand.Visible = CurrentSelectionHasName("web.config");
        }


        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "BlackMarble.ParametersXmlAddin: Entering MenuItemCallback()"));

            // get the project object
            IVsHierarchy hierarchy = null;
            uint itemid = VSConstants.VSITEMID_NIL;
            if (!VSHelper.IsSingleProjectItemSelection(out hierarchy, out itemid))
            {
                // should never get here as we only show the menu items on a single node
                // in a context menu, not on any toolbar etal.
                return;
            }
            var vsProject = (IVsProject)hierarchy;

            // get the name of the web.config file
            string webConfigPath = null;
            if (ErrorHandler.Failed(vsProject.GetMkDocument(itemid, out webConfigPath)))
            {
                return;
            }
            // work out the parameters.xml path
            var parametersXmlPath = Path.Combine(Path.GetDirectoryName(webConfigPath), "parameters.xml");

            if (File.Exists(parametersXmlPath) == false ||
                ShowYesNoBox(
                "Generating parameters.xml",
                "A parameters.xml file already exists in the project folder. Do you wish to replace it?") == MessageBoxReturnCode.IDYES)
            {
                // generate the file
                XmlGenerator.GenerateParametersXmlFile(
                    webConfigPath,
                    parametersXmlPath);
                // add it to the project, this can be run multiple times
                VSHelper.AddFileToProject(vsProject, parametersXmlPath);

                Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "BlackMarble.ParametersXmlAddin: Exiting MenuItemCallback(): The file [{0}] added", parametersXmlPath));


            }
            else if (ShowYesNoBox(
              "Generating parameters.xml",
              "Do you wish to update the existing parameters.xml file with any new parameters?") == MessageBoxReturnCode.IDYES)
            {
                // updated the file
                XmlGenerator.UpdateParametersXmlFile(
                    webConfigPath,
                    parametersXmlPath);
                // add it to the project, this can be run multiple times
                VSHelper.AddFileToProject(vsProject, parametersXmlPath);

                Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "BlackMarble.ParametersXmlAddin: Exiting MenuItemCallback(): The file [{0}] updated", parametersXmlPath));
            }

        }


        /// <summary>
        /// Checks if the selected node has a given file name
        /// </summary>
        /// <param name="extension">The extensions e.g. myfile.cs</param>
        /// <returns>true of it matches</returns>
        private bool CurrentSelectionHasName(string name)
        {
            DTE dte = (DTE)GetService(typeof(SDTE));
            SelectedItems selectedItems = dte.SelectedItems;
            if (selectedItems.Count == 1)
            {
                // note we index from 1 not zero
                if (selectedItems.Item(1).Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Shows a VS messagebox
        /// </summary>
        /// <param name="title">The title of the dialog</param>
        /// <param name="message">The message from the dialog</param>
        private void ShowMessageBox(string title, string message)
        {
            // Show a Message Box to prove we were here
            IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            Guid clsid = Guid.Empty;
            int result;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
                       0,
                       ref clsid,
                       title,
                       message,
                       string.Empty,
                       0,
                       OLEMSGBUTTON.OLEMSGBUTTON_OK,
                       OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                       OLEMSGICON.OLEMSGICON_INFO,
                       0,        // false
                       out result));
        }


        /// <summary>
        /// Shows a VS messagebox
        /// </summary>
        /// <param name="title">The title of the dialog</param>
        /// <param name="message">The message from the dialog</param>
        private MessageBoxReturnCode ShowYesNoBox(string title, string message)
        {
            // Show a Message Box to prove we were here
            IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            Guid clsid = Guid.Empty;
            int result;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
                       0,
                       ref clsid,
                       title,
                       message,
                       string.Empty,
                       0,
                       OLEMSGBUTTON.OLEMSGBUTTON_YESNO,
                       OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                       OLEMSGICON.OLEMSGICON_INFO,
                       0,        // false
                       out result));

            return (MessageBoxReturnCode)result;
        }
    }
}
