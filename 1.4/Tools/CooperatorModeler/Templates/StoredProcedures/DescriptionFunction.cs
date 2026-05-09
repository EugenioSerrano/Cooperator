<%
foreach (BaseTreeNode entityNode in Model.Children) {
    EntityNode currentEntity = (EntityNode)entityNode;
    foreach (BaseTreeNode propNode in currentEntity.Children)
    { 
        PropertyNode currentProperty = (PropertyNode)propNode;
        // Verifico si es una propiedad que hace referencia a una funcion que devuelve una descripcion
        if (currentProperty.RelatedFields.Count != 0 && currentProperty.NativeType != "None" && currentProperty.GenerateProperty)
        {
            string functionName = currentProperty.RelatedTableName + "_GetDescription";
            if (!Response.OuputBuffers.ContainsKey(functionName)) {
%>
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[<%Response.Write(parameters["SpPrefix"]+currentProperty.RelatedTableName);%>_GetDescription]') and xtype in (N'FN', N'IF', N'TF'))
drop function [dbo].[<%Response.Write(parameters["SpPrefix"]+currentProperty.RelatedTableName);%>_GetDescription]

<%Response.SaveBuffer(functionName+"Drop");%>
--------------------------------------------------
-- Author: Cooperator Modeler, Version <%Response.Write(parameters["GeneratorVersion"]);%>
-- Created: <%Response.Write(DateTime.Today.ToShortDateString());%> - <%Response.Write(DateTime.Now.ToShortTimeString());%>
--------------------------------------------------
CREATE FUNCTION [dbo].[<%Response.Write(parameters["SpPrefix"]+currentProperty.RelatedTableName);%>_GetDescription]
(
<%Response.Write(PK_AS_PARAMETERS_FROM_RELATED_FIELDS(currentProperty.RelatedFields)); %>

)
<%
    string whatField = "";
    string whatLength = "";
    foreach (BaseTreeNode tableNode in Model.Children)
    {
        EntityNode table = (EntityNode)tableNode;
        if (table.Name == currentProperty.RelatedTableName) {
            foreach (BaseTreeNode columnNode in table.Children)
            {
                PropertyNode column = (PropertyNode)columnNode;
                if (column.GenerateAsDescriptionField) 
                {
                    whatField = column.Name;
                    if (column.NativeLength <= 0) 
                        whatLength = "36";
                    else
                        whatLength = column.NativeLength.ToString();
                    break;
                }
            }
        }
    }

%>
RETURNS varchar(<%Response.Write(whatLength);%>)
AS
BEGIN
RETURN (
SELECT ISNULL(CONVERT(varchar(<%Response.Write(whatLength);%>), [<%Response.Write(whatField);%>]), '')
FROM <%Response.Write(CORRECT_TABLENAME(currentProperty.RelatedTableName));%>
WHERE 
<%Response.Write(PK_AS_WHERE_FROM_RELATED_FIELDS(currentProperty.RelatedFields)); %>

)
END                
<%Response.SaveBuffer(functionName);%>
<%
            }
        }
    }
}
%>
