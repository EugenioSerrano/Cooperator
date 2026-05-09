<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <connectionStrings>
        <add name="CooperatorConnectionString"
            connectionString="<%Response.Write(parameters["ConnectionString"]+"; Application Name=" + parameters["AppProjectName"] + ";" );%>" 
            providerName="System.Data.SqlClient" />
    </connectionStrings>
  <appSettings>
    <add key="Enterprisename" value="<%Response.Write(parameters["AppProjectName"]);%>" />
  </appSettings>
</configuration>

<%Response.SaveBuffer("\\ConfigFiles\\App.Config");%>
