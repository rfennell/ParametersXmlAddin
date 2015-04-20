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
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
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
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
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
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering BeforeQueryStatus()"));

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
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering MenuItemCallback()"));
    
            // gets access to the item in the IDE, but no access 
            // to underling files 
            /*
            DTE dte = (DTE)GetService(typeof(SDTE));
            SelectedItems selectedItems = dte.SelectedItems;
            */
            
            IVsHierarchy hierarchy = null;
            uint itemid = VSConstants.VSITEMID_NIL;

            if (!IsSingleProjectItemSelection(out hierarchy, out itemid)) return;

            var vsProject = (IVsProject)hierarchy;
         
            string projectFullPath = null;
            if (ErrorHandler.Failed(vsProject.GetMkDocument(VSConstants.VSITEMID_ROOT, out projectFullPath))) return;

            // get the name of the item
            string itemFullPath = null;
            if (ErrorHandler.Failed(vsProject.GetMkDocument(itemid, out itemFullPath))) return;

            var newFile = Path.Combine(Path.GetDirectoryName(itemFullPath), "parameters.xml");
            XmlGenerator.GenerateParametersXmlFile(
                itemFullPath, 
                newFile);

            var selectedProjectItem = GetProjectItemFromHierarchy(hierarchy, itemid);
            ((ProjectItem)selectedProjectItem.ProjectItems.Parent).ProjectItems.AddFromFile(newFile);

            ShowMessageBox(
                "ParametersXmlAddin", 
                string.Format("Added {0} to project", newFile));
        }

        private ProjectItem GetProjectItemFromHierarchy(IVsHierarchy pHierarchy, uint itemID)
        {
            object propertyValue;
            ErrorHandler.ThrowOnFailure(pHierarchy.GetProperty(itemID, (int)__VSHPROPID.VSHPROPID_ExtObject, out propertyValue));

            var projectItem = propertyValue as ProjectItem;
            if (projectItem == null) return null;

            return projectItem;
        }

        /// <summary>
        /// Gets the current items
        /// Taken from https://github.com/oncheckin/oncheckin-transformer/blob/master/OnCheckinTransformer.VisualStudio/OnCheckinTransforms.VisualStudioPackage.cs
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <param name="itemid"></param>
        /// <returns></returns>
        private static bool IsSingleProjectItemSelection(out IVsHierarchy hierarchy, out uint itemid)
        {
            hierarchy = null;
            itemid = VSConstants.VSITEMID_NIL;
            int hr = VSConstants.S_OK;

            var monitorSelection = Package.GetGlobalService(typeof(SVsShellMonitorSelection)) as IVsMonitorSelection;
            var solution = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;
            if (monitorSelection == null || solution == null)
            {
                return false;
            }

            IVsMultiItemSelect multiItemSelect = null;
            IntPtr hierarchyPtr = IntPtr.Zero;
            IntPtr selectionContainerPtr = IntPtr.Zero;

            try
            {
                hr = monitorSelection.GetCurrentSelection(out hierarchyPtr, out itemid, out multiItemSelect, out selectionContainerPtr);

                if (ErrorHandler.Failed(hr) || hierarchyPtr == IntPtr.Zero || itemid == VSConstants.VSITEMID_NIL)
                {
                    // there is no selection
                    return false;
                }

                // multiple items are selected
                if (multiItemSelect != null) return false;

                // there is a hierarchy root node selected, thus it is not a single item inside a project

                if (itemid == VSConstants.VSITEMID_ROOT) return false;

                hierarchy = Marshal.GetObjectForIUnknown(hierarchyPtr) as IVsHierarchy;
                if (hierarchy == null) return false;

                Guid guidProjectID = Guid.Empty;

                if (ErrorHandler.Failed(solution.GetGuidOfProject(hierarchy, out guidProjectID)))
                {
                    return false; // hierarchy is not a project inside the Solution if it does not have a ProjectID Guid
                }

                // if we got this far then there is a single project item selected
                return true;
            }
            finally
            {
                if (selectionContainerPtr != IntPtr.Zero)
                {
                    Marshal.Release(selectionContainerPtr);
                }

                if (hierarchyPtr != IntPtr.Zero)
                {
                    Marshal.Release(hierarchyPtr);
                }
            }
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
        /// Checks if the selected node has a given file name
        /// </summary>
        /// <param name="extension">The extensions e.g. myfile.cs</param>
        /// <returns>true of it matches</returns>
        private bool CurrentSelectionHasName(string name)
        {
            DTE dte = (DTE)GetService(typeof(SDTE));
            SelectedItems selectedItems = dte.SelectedItems;
            if (selectedItems.Count==1)
            {
                // note we index from 1 not zero
                if (selectedItems.Item(1).Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

    }
}
