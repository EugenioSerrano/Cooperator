<%
foreach (BaseTreeNode entityNode in Model.Children) {
    EntityNode currentEntity = (EntityNode)entityNode;
    if (currentEntity.GenerateSpDeleteByParents) {
    if (currentEntity.GenerateObject && currentEntity.PrimaryKeyFields.Count != 0) {
    foreach (BaseTreeNode propNode in currentEntity.Children) {
        PropertyNode currentProperty = (PropertyNode)propNode;
        if (currentProperty.IsEntity) {
%>
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[<%Response.Write(parameters["SpPrefix"]+currentEntity.Name+"_DeleteBy"+currentProperty.GenerateAs.Replace("Entity","").Replace("Aggregation",""));%>]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
drop procedure [dbo].[<%Response.Write(parameters["SpPrefix"]+currentEntity.Name+"_DeleteBy"+currentProperty.GenerateAs.Replace("Entity","").Replace("Aggregation",""));%>]
<%
string scriptName = currentEntity.Name+"_DeleteBy"+currentProperty.GenerateAs.Replace("Entity","").Replace("Aggregation","")+"Drop";
if (!Response.OuputBuffers.ContainsKey(scriptName)) Response.SaveBuffer(scriptName);
else Response.Clear();
%>
--------------------------------------------------
-- Author: Cooperator Modeler, Version <%Response.Write(parameters["GeneratorVersion"]);%>
-- Created: <%Response.Write(DateTime.Today.ToShortDateString());%> - <%Response.Write(DateTime.Now.ToShortTimeString());%>
--------------------------------------------------
CREATE PROCEDURE [dbo].[<%Response.Write(parameters["SpPrefix"]+currentEntity.Name+"_DeleteBy"+currentProperty.GenerateAs.Replace("Entity","").Replace("Aggregation",""));%>] 
<%
        string sep = "";
        foreach (RelatedField relField in currentProperty.RelatedFields)
        {
            string type = "";
            if (relField.NativeLength == 0 || relField.NativeType.ToLower() == "ntext" || relField.NativeType.ToLower() == "text" || relField.NativeType.ToLower() == "timestamp")
                type = relField.NativeType;
            else
                type = relField.NativeType + "(" + relField.NativeLength + ")";
            Response.Write(sep + "@" + CORRECT_NAME(relField.Name) + " " + type);
            sep = ",\r\n";
        }
%>
AS

DELETE <%Response.Write(CORRECT_TABLENAME(currentEntity.Name));%>

WHERE 
<%
            sep = "";
            foreach (RelatedField relField in currentProperty.RelatedFields)
            {
                Response.Write(sep + "[" +relField.Name + "] = @" + CORRECT_NAME(relField.Name));
                sep = " AND\r\n";
            }
scriptName = currentEntity.Name+"_DeleteBy"+currentProperty.GenerateAs.Replace("Entity","").Replace("Aggregation","");
if (!Response.OuputBuffers.ContainsKey(scriptName)) Response.SaveBuffer(scriptName);
else Response.Clear();
}
}
}
}
}
%>
