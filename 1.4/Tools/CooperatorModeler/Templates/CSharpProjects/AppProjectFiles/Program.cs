
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace <%Response.Write(parameters["AppProjectName"]);%>
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
<%Response.SaveBuffer("\\AppProjectFiles\\Program.cs");%>
