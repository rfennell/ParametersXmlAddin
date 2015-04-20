# MSDeploy Parameters.Xml Generator
A tool to generate parameters.xml files for MSdeploy from existing web.config files.

Once the VSIX package is installed, to use right click on a web.config file in Solution Explorer and the parameters.xml file will be generated using the current web.config entries as default values for both configuration/applicationSettings and configuration/AppSettings.

If the parameters.xml already exists in the folder (even if it is not a file in the project) you will prompted before it is overwritten.
