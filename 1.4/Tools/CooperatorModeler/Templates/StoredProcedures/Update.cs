<%
foreach (BaseTreeNode entityNode in Model.Children) {
    EntityNode currentEntity = (EntityNode)entityNode;
    if (currentEntity.GenerateSpUpdate) {
    if (currentEntity.GenerateObject && currentEntity.PrimaryKeyFields.Count != 0) {
%>
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[<%Response.Write(parameters["SpPrefix"]+currentEntity.Name);%>_Update]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
drop procedure [dbo].[<%Response.Write(parameters["SpPrefix"]+currentEntity.Name);%>_Update]
<%Response.SaveBuffer(currentEntity.Name+"_UpdateDrop");%>
--------------------------------------------------
-- Author: Cooperator Modeler, Version <%Response.Write(parameters["GeneratorVersion"]);%>
-- Created: <%Response.Write(DateTime.Today.ToShortDateString());%> - <%Response.Write(DateTime.Now.ToShortTimeString());%>
--------------------------------------------------
CREATE PROCEDURE [dbo].[<%Response.Write(parameters["SpPrefix"]+currentEntity.Name);%>_Update]
<%Response.Write(ALL_FIELDS_MORE_PK_AS_PARAMETERS(currentEntity));%>

AS

UPDATE <%Response.Write(CORRECT_TABLENAME(currentEntity.Name));%> SET
<%Response.Write(SET_ALL_FIELD_BY_VARIABLES(currentEntity));%>

WHERE 
<%Response.Write(PK_PK_AS_WHERE(currentEntity));%>
<%if (currentEntity.GenerateAsVersionable) { %>
IF @@ROWCOUNT = 0 BEGIN
  IF EXISTS(SELECT <%Response.Write(PK_FIELDS_FOR_SELECT(currentEntity));%> FROM <%Response.Write(CORRECT_TABLENAME(currentEntity.Name));%> WHERE <%Response.Write(PK_AS_WHERE(currentEntity));%> )
    RAISERROR ('CONCURRENCE ERROR', 16, 1) 
  ELSE
    RAISERROR ('ROW DO NOT EXIST', 16, 1) 
END
<%Response.Write(SET_ROWVERSION(currentEntity));%>
<%}%>
<%Response.SaveBuffer(currentEntity.Name + "_Update");
}
}
}%>
