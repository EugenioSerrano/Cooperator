<%
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cooperator.Framework.Utility.DBReverseHelper;
using Cooperator.Framework.Utility.CodeGeneratorHelper;

public class RealScript: //{PROVIDERCLASSNAME}// 
{
    public RealScript():base()
	{
	}

    public override object Main(BaseTreeNode Model, Dictionary<string,string> parameters)
    {        
        %>
        //{INSERTCODE}//
        <%
        return null;
    }
}
%>
