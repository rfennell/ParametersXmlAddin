﻿<!-- every StyleSheet starts with this tag -->
<xsl:stylesheet
      xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
      version="1.0" xml:space="preserve">
  <xsl:output encoding="utf-8"/>
  <xsl:param name="file" />
  <xsl:template match="/"><parameters>
  <!-- the appSettings -->
  <xsl:for-each select="/configuration/appSettings/add">
    <xsl:variable name="defaultValue" select="translate(@key, 'abcdefghijklmnopqrstuvwxyz', 'ABCDEFGHIJKLMNOPQRSTUVWXYZ')"/>
    <parameter name="{@key}" description="" defaultvalue="__{$defaultValue}__" tags="">
    <parameterentry kind="XmlFile" scope="\\{$file}$" match="/configuration/appSettings/add[@key='{@key}']/@value" />
    </parameter>
  </xsl:for-each>
  <!-- any custom settings-->  
  <xsl:for-each select="/configuration/applicationSettings/*">
    <xsl:variable name="settingblockname" select="name()"/>
    <xsl:for-each select="current()/setting">
      <xsl:variable name="defaultValue" select="translate(@name, 'abcdefghijklmnopqrstuvwxyz', 'ABCDEFGHIJKLMNOPQRSTUVWXYZ')"/>
      <parameter name="{@name}" description="" defaultvalue="__{$defaultValue}__" tags="">
      <parameterentry kind="XmlFile" scope="\\{$file}$" match="/configuration/applicationSettings/{$settingblockname}/setting[@name='{@name}']/value/text()" />
      </parameter>
  </xsl:for-each>  
  </xsl:for-each>  
</parameters></xsl:template>
</xsl:stylesheet>
