
[Build Status](https://richardfennell.visualstudio.com/DefaultCollection/_apis/public/build/definitions/670b3a60-2021-47ab-a88b-d76ebd888a2f/10/badge)
# MSDeploy Parameters.Xml Generator
A tool to generate parameters.xml files for MSdeploy from existing web.config files.

Once the VSIX package is installed, to use right click on a web.config file in Solution Explorer and the parameters.xml file will be generated using the current web.config entries as default values for both configuration/applicationSettings and configuration/AppSettings.

If the parameters.xml already exists in the folder (even if it is not a file in the project) you will prompted before it is overwritten.

For more details see the [wiki](https://github.com/rfennell/ParametersXmlAddin/wiki/Using-Parameters-XML-Addin-for-Visual-Studio). You can also install the package from [Visual Studio gallery](https://visualstudiogallery.msdn.microsoft.com/cbf2764d-d205-49d6-810f-25324402c3a9?SRC=Home)
