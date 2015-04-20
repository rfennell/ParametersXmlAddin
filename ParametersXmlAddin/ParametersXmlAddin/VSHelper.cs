using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BlackMarble.ParametersXmlAddin
{
    internal static  class VSHelper
    {
        internal static void AddFileToProject(IVsProject vsProject, string parametersXmlPath)
        {
            string[] files = new string[1] { parametersXmlPath };
            VSADDRESULT[] result = new VSADDRESULT[1];
            vsProject.AddItem(
                VSConstants.VSITEMID_ROOT,
                VSADDITEMOPERATION.VSADDITEMOP_OPENFILE,
                parametersXmlPath,
                1,
                files,
                IntPtr.Zero,
                result);
        }


        /// <summary>
        /// Gets the current items
        /// Taken from https://github.com/oncheckin/oncheckin-transformer/blob/master/OnCheckinTransformer.VisualStudio/OnCheckinTransforms.VisualStudioPackage.cs
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <param name="itemid"></param>
        /// <returns></returns>
        internal static bool IsSingleProjectItemSelection(out IVsHierarchy hierarchy, out uint itemid)
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

      



        //internal static ProjectItem GetProjectItemFromHierarchy(IVsHierarchy pHierarchy, uint itemID)
        //{
        //    object propertyValue;
        //    ErrorHandler.ThrowOnFailure(pHierarchy.GetProperty(itemID, (int)__VSHPROPID.VSHPROPID_ExtObject, out propertyValue));

        //    var projectItem = propertyValue as ProjectItem;
        //    if (projectItem == null) return null;

        //    return projectItem;
        //}

    }
}
