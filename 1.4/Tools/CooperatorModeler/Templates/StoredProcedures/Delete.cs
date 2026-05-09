<%
foreach (BaseTreeNode entityNode in Model.Children) {
    EntityNode currentEntity = (EntityNode)entityNode;
    if (currentEntity.GenerateSpDelete) {
    if (currentEntity.GenerateObject && currentEntity.PrimaryKeyFields.Count != 0) {
%>
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[<%Response.Write(parameters["SpPrefix"]+currentEntity.Name);%>_Delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
drop procedure [dbo].[<%Response.Write(parameters["SpPrefix"]+currentEntity.Name);%>_Delete]
<%Response.SaveBuffer(currentEntity.Name+"_DeleteDrop");%>
--------------------------------------------------
-- Author: Cooperator Modeler, Version <%Response.Write(parameters["GeneratorVersion"]);%>
-- Created: <%Response.Write(DateTime.Today.ToShortDateString());%> - <%Response.Write(DateTime.Now.ToShortTimeString());%>
--------------------------------------------------
CREATE PROCEDURE [dbo].[<%Response.Write(parameters["SpPrefix"]+currentEntity.Name);%>_Delete]
<%Response.Write(PK_AS_PARAMETERS_PLUS_ROWVERSION(currentEntity));%>

AS

DELETE FROM <%Response.Write(CORRECT_TABLENAME(currentEntity.Name));%>
WHERE 
<%Response.Write(PK_AS_WHERE_PLUS_ROWVERSION(currentEntity));%>
<%if (currentEntity.GenerateAsVersionable) { %>
IF @@ROWCOUNT = 0 BEGIN
  IF EXISTS(SELECT <%Response.Write(PK_FIELDS_FOR_SELECT(currentEntity));%> FROM <%Response.Write(CORRECT_TABLENAME(currentEntity.Name));%> WHERE <%Response.Write(PK_AS_WHERE(currentEntity));%> )
    RAISERROR ('CONCURRENCE ERROR', 16, 1) 
  ELSE
    RAISERROR ('ROW DO NOT EXIST', 16, 1) 
END
<%}%>
<%Response.SaveBuffer(currentEntity.Name + "_Delete");
}
}
}
%>
