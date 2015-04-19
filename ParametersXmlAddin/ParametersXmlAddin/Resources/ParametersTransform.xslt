<!-- every StyleSheet starts with this tag -->
<xsl:stylesheet
      xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
      version="1.0" xml:space="preserve">
  <xsl:output encoding="utf-8"/>
  <xsl:template match="/"><parameters>
  <!-- the appSettings -->
  <xsl:for-each select="/configuration/appSettings/add"><parameter name="{@key}" description="Description for {@key}" defaultvalue="{@value}" tags="">
    <parameterentry kind="XmlFile" scope="\\web.config$" match="/configuration/appSettings/add[@key='{@key}']/@value" />
  </parameter>
  </xsl:for-each>
  <!-- any custom settings-->  
  <xsl:for-each select="/configuration/applicationSettings/*">
    <xsl:variable name="settingblockname" select="name()"/>
    <xsl:for-each select="current()/setting">
      <parameter name="{@name}" description="Description for {@name}" defaultvalue="{value}" tags="">
      <parameterentry kind="XmlFile" scope="\\web.config$" match="/configuration/applicationSettings/{$settingblockname}/setting[@name='{@name}']/value/text()" />
      </parameter>
  </xsl:for-each>  
  </xsl:for-each>  
</parameters></xsl:template>
</xsl:stylesheet>
