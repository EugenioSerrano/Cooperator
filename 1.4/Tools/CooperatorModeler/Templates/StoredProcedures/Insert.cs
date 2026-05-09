<%
foreach (BaseTreeNode entityNode in Model.Children) {
    EntityNode currentEntity = (EntityNode)entityNode;
    if (currentEntity.GenerateSpInsert) {
    if (currentEntity.GenerateObject && currentEntity.PrimaryKeyFields.Count != 0) {
%>
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[<%Response.Write(parameters["SpPrefix"]+currentEntity.Name);%>_Insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
drop procedure [dbo].[<%Response.Write(parameters["SpPrefix"]+currentEntity.Name);%>_Insert]
<%Response.SaveBuffer(currentEntity.Name+"_InsertDrop");%>
--------------------------------------------------
-- Author: Cooperator Modeler, Version <%Response.Write(parameters["GeneratorVersion"]);%>
-- Created: <%Response.Write(DateTime.Today.ToShortDateString());%> - <%Response.Write(DateTime.Now.ToShortTimeString());%>
--------------------------------------------------
CREATE PROCEDURE [dbo].[<%Response.Write(parameters["SpPrefix"]+currentEntity.Name);%>_Insert]
<%Response.Write(ALL_FIELDS_AS_PARAMETERS(currentEntity));%>

AS
<%Response.Write(SET_GUID_VALUES(currentEntity));%>
INSERT INTO <%Response.Write(CORRECT_TABLENAME(currentEntity.Name));%>(
<%Response.Write(ALL_FIELDS_SEPARATED_BY_COMMA(currentEntity));%>

)

VALUES(
<%Response.Write(ALL_VARIABLES_OF_FIELDS_SEPARATED_BY_COMMA(currentEntity));%>
)
<%Response.Write(SET_IDENTITY_VALUES(currentEntity));%>
<%if (currentEntity.GenerateAsVersionable) Response.Write(SET_ROWVERSION(currentEntity));%>
<%Response.SaveBuffer(currentEntity.Name + "_Insert");
}
}
}%>
