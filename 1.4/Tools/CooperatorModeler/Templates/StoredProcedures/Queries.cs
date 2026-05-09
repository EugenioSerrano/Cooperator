<%
foreach (BaseTreeNode entityNode in Model.Children) {
    EntityNode currentEntity = (EntityNode)entityNode;

    foreach (DatabaseQuery currentQuery in currentEntity.Queries) {
        if (currentEntity.GenerateObject ) {
        if (currentQuery.GenerateQuery) {
%>
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[<%Response.Write(parameters["SpPrefix"]+currentEntity.Name);%>_<%Response.Write(currentQuery.QueryName);%>]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
drop procedure [dbo].[<%Response.Write(parameters["SpPrefix"]+currentEntity.Name);%>_<%Response.Write(currentQuery.QueryName);%>]
<%Response.SaveBuffer(currentEntity.Name + "_" + currentQuery.QueryName + "Drop");%>
--------------------------------------------------
-- Author: Cooperator Modeler, Version <%Response.Write(parameters["GeneratorVersion"]);%>
-- Created: <%Response.Write(DateTime.Today.ToShortDateString());%> - <%Response.Write(DateTime.Now.ToShortTimeString());%>
--------------------------------------------------
CREATE PROCEDURE [dbo].[<%Response.Write(parameters["SpPrefix"]+currentEntity.Name);%>_<%Response.Write(currentQuery.QueryName);%>]

<%Response.Write(currentQuery.Parameters); %>

AS

<%Response.Write(currentQuery.QueryText.Replace("<FIELDS>", ALL_FIELDS_FOR_SELECT(currentEntity, parameters, true)).Replace("<TABLE>", CORRECT_TABLENAME(currentEntity.Name))) ; %>

<%
Response.SaveBuffer(currentEntity.Name + "_" + currentQuery.QueryName);
}
}
}
}%>
