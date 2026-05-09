<%
foreach (BaseTreeNode entityNode in Model.Children) {
    EntityNode currentEntity = (EntityNode)entityNode;
    if (currentEntity.GenerateSpGetByParents) {
    if (currentEntity.GenerateObject  ) {
    foreach (BaseTreeNode propNode in currentEntity.Children) {
        PropertyNode currentProperty = (PropertyNode)propNode;
        if (currentProperty.IsEntity) {
%>
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[<%Response.Write(parameters["SpPrefix"]+currentEntity.Name+"_GetBy"+currentProperty.GenerateAs.Replace("Entity","").Replace("Aggregation",""));%>]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
drop procedure [dbo].[<%Response.Write(parameters["SpPrefix"]+currentEntity.Name+"_GetBy"+currentProperty.GenerateAs.Replace("Entity","").Replace("Aggregation",""));%>]
<%
string scriptName = currentEntity.Name+"_GetBy"+currentProperty.GenerateAs.Replace("Entity","").Replace("Aggregation","")+"Drop";
if (!Response.OuputBuffers.ContainsKey(scriptName)) Response.SaveBuffer(scriptName);
else Response.Clear();
%>
--------------------------------------------------
-- Author: Cooperator Modeler, Version <%Response.Write(parameters["GeneratorVersion"]);%>
-- Created: <%Response.Write(DateTime.Today.ToShortDateString());%> - <%Response.Write(DateTime.Now.ToShortTimeString());%>
--------------------------------------------------
CREATE PROCEDURE [dbo].[<%Response.Write(parameters["SpPrefix"]+currentEntity.Name+"_GetBy"+currentProperty.GenerateAs.Replace("Entity","").Replace("Aggregation",""));%>] 
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

SELECT 
<%
    sep = @"";
    foreach (BaseTreeNode propNode2 in currentEntity.Children)
    { 
        PropertyNode currentProperty2 = (PropertyNode)propNode2;
        if (currentProperty2.GenerateProperty || currentProperty2.IsPrimaryKey || currentProperty2.IsForeignKey) {
            if (currentProperty2.NativeType != "None") {
                if (currentProperty2.RelatedFields.Count == 0 ) {
                    Response.Write(sep + "[" + currentProperty2.Name + "]" );
                } else {
                    string sep2 = "";
                    string funcParams = "";
                    foreach (RelatedField relField in currentProperty2.RelatedFields )
                    {
                        funcParams += sep2 + "[" + relField.Name + "]";
                        sep2 = ", ";
                    }
                    Response.Write(sep + "[dbo].[" + parameters["SpPrefix"] + currentProperty2.RelatedTableName + "_GetDescription](" + funcParams + ") AS " + currentProperty2.Name);
                }
                sep = ",\r\n";
            }
        }
    }
%>

FROM <%Response.Write(CORRECT_TABLENAME(currentEntity.Name));%>

WHERE 
<%
            sep = "";
            foreach (RelatedField relField in currentProperty.RelatedFields)
            {
                Response.Write(sep + "[" +relField.Name + "] = @" + CORRECT_NAME(relField.Name));
                sep = " AND\r\n";
            }

%>
<%if (currentEntity.OrderBy.Trim() != String.Empty ) {
   Response.Write("ORDER BY " + currentEntity.OrderBy.Trim());
}%>
<%
scriptName = currentEntity.Name+"_GetBy"+currentProperty.GenerateAs.Replace("Entity","").Replace("Aggregation","");
if (!Response.OuputBuffers.ContainsKey(scriptName)) Response.SaveBuffer(scriptName);
else Response.Clear();
}
}
}
}
}%>
