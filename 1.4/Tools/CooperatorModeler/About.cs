/*-
*       Copyright (c) 2006-2007 Eugenio Serrano, Daniel Calvin.
*       All rights reserved.
*
*       Redistribution and use in source and binary forms, with or without
*       modification, are permitted provided that the following conditions
*       are met:
*       1. Redistributions of source code must retain the above copyright
*          notice, this list of conditions and the following disclaimer.
*       2. Redistributions in binary form must reproduce the above copyright
*          notice, this list of conditions and the following disclaimer in the
*          documentation and/or other materials provided with the distribution.
*       3. Neither the name of copyright holders nor the names of its
*          contributors may be used to endorse or promote products derived
*          from this software without specific prior written permission.
*
*       THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
*       "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
*       TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
*       PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL COPYRIGHT HOLDERS OR CONTRIBUTORS
*       BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
*       CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
*       SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
*       INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
*       CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
*       ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
*       POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace CooperatorModeler
{
    partial class About : Form
    {
        public About()
        {
            InitializeComponent();

            //  Inicializar AboutBox para mostrar la información del producto desde la información del ensamblado.
            //  Cambiar la configuración de la información del ensamblado correspondiente a su aplicación desde:
            //  - Proyecto->Propiedades->Aplicación->Información del ensamblado
            //  - AssemblyInfo.cs
            this.Text = String.Format("About {0}", AssemblyTitle);
            this.labelProductName.Text = AssemblyProduct;
            this.labelVersion.Text = String.Format("Version {0}", AssemblyVersion);
            this.labelCopyright.Text = AssemblyCopyright;
            this.labelCompanyName.Text = AssemblyCompany;

            string licenseFileName = Cooperator.Framework.Library.IO.Path.AppPath(true) + "License.txt";
            StreamReader licenseReader = File.OpenText(licenseFileName);
            string licenseText = licenseReader.ReadToEnd();
            licenseReader.Close();
            licenseReader.Dispose();

            this.textBoxDescription.Text = licenseText;


        }

        #region Descriptores de acceso de atributos de ensamblado

        public string AssemblyTitle
        {
            get
            {
                // Obtener todos los atributos Title en este ensamblado
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                // Si hay al menos un atributo Title
                if (attributes.Length > 0)
                {
                    // Seleccione el primero
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    // Si no es una cadena vacía, devuélvalo
                    if (titleAttribute.Title != "")
                        return titleAttribute.Title;
                }
                // Si no había ningún atributo Title, o si el atributo Title era una cadena vacía, devuelva el nombre del archivo .exe
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                // Obtener todos los atributos de descripción de este ensamblado
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                // Si no hay ningún atributo de descripción, devuelva una cadena vacía
                if (attributes.Length == 0)
                    return "";
                // Si hay un atributo de descripción, devuelva su valor
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                // Obtenga todos los atributos del producto de este ensamblado
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                // Si no hay atributos del producto, devuelva una cadena vacía
                if (attributes.Length == 0)
                    return "";
                // Si hay un atributo de producto, devuelva su valor
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                // Obtenga todos los atributos de copyright de este ensamblado
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                // Si no hay atributos de copyright, devuelva una cadena vacía
                if (attributes.Length == 0)
                    return "";
                // Si hay un atributo de copyright, devuelva su valor
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                // Obtenga todos los atributos de compañía en este ensamblado
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                // Si no hay ningún atributo de compañía, devuelva una cadena vacía
                if (attributes.Length == 0)
                    return "";
                // Si hay un atributo de compañía, devuelva su valor
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(linkLabel1.Text.Trim());
        }
    }
}
