<%
foreach (BaseTreeNode entityNode in Model.Children) {
    EntityNode currentEntity = (EntityNode)entityNode;
    if (currentEntity.GenerateSpGetAll) {
    if (currentEntity.GenerateObject ) {
%>
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[<%Response.Write(parameters["SpPrefix"]+currentEntity.Name);%>_GetAll]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
drop procedure [dbo].[<%Response.Write(parameters["SpPrefix"]+currentEntity.Name);%>_GetAll]
<%Response.SaveBuffer(currentEntity.Name+"_GetAllDrop");%>
--------------------------------------------------
-- Author: Cooperator Modeler, Version <%Response.Write(parameters["GeneratorVersion"]);%>
-- Created: <%Response.Write(DateTime.Today.ToShortDateString());%> - <%Response.Write(DateTime.Now.ToShortTimeString());%>
--------------------------------------------------
CREATE PROCEDURE [dbo].[<%Response.Write(parameters["SpPrefix"]+currentEntity.Name);%>_GetAll] AS

SELECT 
<%Response.Write(ALL_FIELDS_FOR_SELECT(currentEntity, parameters));%>

FROM <%Response.Write(CORRECT_TABLENAME(currentEntity.Name));%>

<%if (currentEntity.OrderBy.Trim() != String.Empty ) {
   Response.Write("ORDER BY " + currentEntity.OrderBy.Trim());
}%>

<%
Response.SaveBuffer(currentEntity.Name + "_GetAll");
}
}
}%>
