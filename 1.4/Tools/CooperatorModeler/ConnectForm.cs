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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.Common;
using System.Data.Sql;
using System.Data.SqlClient;
using Cooperator.Framework.Library.Repository;
using Cooperator.Framework.Library.Exceptions;

namespace CooperatorModeler
{
	public partial class ConnectForm : Form
	{
		public ConnectForm()
		{
			InitializeComponent();
		}

        const string baseCon = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog={1};Data Source={0}";


		private void lbBases_SelectedIndexChanged(object sender, EventArgs e)
		{
			ConnectionStringTextBox.Text = string.Format(baseCon, ServersComboBox.Text, DataBasesListBox.SelectedValue.ToString());
		}

		private void txtConnectionString_TextChanged(object sender, EventArgs e)
		{
			Ok_Button.Enabled = ConnectionStringTextBox.Text != "";

		}

        // Se ejecuta cuando se muestra el formulario por primera vez
        private void ConnectForm_Shown(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void GetDatabasesbutton_Click(object sender, EventArgs e)
        {
            try
            {
                string con = string.Format(baseCon, ServersComboBox.Text, "master");
                SqlConnection Connect = new SqlConnection(con);
                Connect.Open();
                DataTable tbdb = Connect.GetSchema("Databases");
                Connect.Close();
                DataBasesListBox.DisplayMember = tbdb.Columns[0].ColumnName;
                DataBasesListBox.ValueMember = DataBasesListBox.DisplayMember;
                DataBasesListBox.DataSource = tbdb.DefaultView;
                Ok_Button.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BuscarMasbutton_Click(object sender, EventArgs e)
        {
            this.UseWaitCursor = true;
            ServersComboBox.Items.Clear();
            this.Refresh();
            System.Windows.Forms.Application.DoEvents();
            System.Threading.Thread.Sleep(200);

            SqlDataSourceEnumerator DbServer = SqlDataSourceEnumerator.Instance;
            DataTable TServ = DbServer.GetDataSources();
            foreach (DataRow r in TServ.Rows)
            {
                if (r["InstanceName"].ToString().Length > 0)
                {
                    ServersComboBox.Items.Add(r["ServerName"].ToString() + @"\" + r["InstanceName"].ToString());
                }
                else
                {
                    ServersComboBox.Items.Add(r[0].ToString());
                }
            }
            this.UseWaitCursor = false;
        }

        private void Ok_Button_Click(object sender, EventArgs e)
        {
            RegistryRepository.SaveItem(RegistryTarget.CurrentUser, "Cooperator.Modeler", "CurrentServer", this.ServersComboBox.Text);
            RegistryRepository.SaveItem(RegistryTarget.CurrentUser, "Cooperator.Modeler", "CurrentConnectionString", this.ConnectionStringTextBox.Text);
            this.DialogResult = DialogResult.OK;
        }

        private void ConnectForm_Load(object sender, EventArgs e)
        {
            try
            {
                this.ServersComboBox.Text = RegistryRepository.GetItemAsString(RegistryTarget.CurrentUser, "Cooperator.Modeler", "CurrentServer");
                this.ConnectionStringTextBox.Text = RegistryRepository.GetItemAsString(RegistryTarget.CurrentUser, "Cooperator.Modeler", "CurrentConnectionString");
            }
            catch (RegistryIOErrorException)
            {
                this.ServersComboBox.Text = "";
            }

        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
	}
}