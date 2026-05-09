<%
foreach (BaseTreeNode entityNode in Model.Children) {
    EntityNode currentEntity = (EntityNode)entityNode;
    if (currentEntity.GenerateSpGetOne) {
    if (currentEntity.GenerateObject && currentEntity.PrimaryKeyFields.Count != 0) {
%>
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[<%Response.Write(parameters["SpPrefix"]+currentEntity.Name);%>_GetOne]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
drop procedure [dbo].[<%Response.Write(parameters["SpPrefix"]+currentEntity.Name);%>_GetOne]
<%Response.SaveBuffer(currentEntity.Name+"_GetOneDrop");%>
--------------------------------------------------
-- Author: Cooperator Modeler, Version <%Response.Write(parameters["GeneratorVersion"]);%>
-- Created: <%Response.Write(DateTime.Today.ToShortDateString());%> - <%Response.Write(DateTime.Now.ToShortTimeString());%>
--------------------------------------------------
CREATE PROCEDURE [dbo].[<%Response.Write(parameters["SpPrefix"]+currentEntity.Name);%>_GetOne]
<%Response.Write(PK_AS_PARAMETERS(currentEntity));%>

AS

SELECT 
<%Response.Write(ALL_FIELDS_FOR_SELECT(currentEntity, parameters));%>

FROM <%Response.Write(CORRECT_TABLENAME(currentEntity.Name));%>
WHERE 
<%Response.Write(PK_AS_WHERE(currentEntity));%>

<%
Response.SaveBuffer(currentEntity.Name + "_GetOne");
}
}
}%>
