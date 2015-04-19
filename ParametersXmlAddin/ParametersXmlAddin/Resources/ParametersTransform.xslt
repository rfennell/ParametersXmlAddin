<!-- every StyleSheet starts with this tag -->
<xsl:stylesheet
      xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
      version="1.0" xml:space="preserve">
  <xsl:output encoding="utf-8"/>
  <!-- indicates what our output type is going to be -->
  <xsl:template match="/"><parameters>
  <xsl:for-each select="/configuration/appSettings/add"><parameter name="{@key}" description="Description for {@key}" defaultvalue="{@value}" tags="">
    <parameterentry kind="XmlFile" scope="\\web.config$" match="/configuration/appSettings/add[@key='{@key}']/@value">
    </parameterentry>
  </parameter>
  </xsl:for-each>
  <xsl:for-each select="/configuration/applicationSettings">
    <!--<xsl:value-of select="current()"/>-->
    <xsl:for-each select="current()/setting">
      <parameter name="{@name}" description="Description for {@name}" defaultvalue="{value}" tags="">
      <parameterentry kind="XmlFile" scope="\\web.config$" match="/configuration/{current()}/add[@key='{@key}']/@value">
      </parameterentry>
      </parameter>
  </xsl:for-each>  
  </xsl:for-each>  
</parameters></xsl:template>
</xsl:stylesheet>
