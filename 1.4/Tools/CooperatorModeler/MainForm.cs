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
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Cooperator.Framework.Utility.DBReverseHelper;
using Cooperator.Framework.Utility.DBReverseHelper.Providers.MSSQL;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;

namespace CooperatorModeler
{

    public partial class MainForm : Form
    {

        internal Snapshot currentSnapshot = new Snapshot();

        public MainForm()
        {
            InitializeComponent();
            UI.mainForm = this;
        }


        // save the command arguments
        public string[] args;



        private void Exit_toolStripButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Connect_toolStripButton1_Click(object sender, EventArgs e)
        {
            if (this.currentSnapshot.HaveChanges)
            {
                if (MessageBox.Show("The current model will be lost. Do you want to continue ?", "New model", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes )
                {
                    return;
                }
            }

            // Pedimos el nuevo string de conexion.
            UI.StatusLabel = "Select database";
            this.Refresh();
            ConnectForm myDialog = new ConnectForm();
            if (myDialog.ShowDialog(this) == DialogResult.OK)
            {
                this.currentSnapshot = new Snapshot();
                this.currentSnapshot.ConnectionString = myDialog.ConnectionStringTextBox.Text;

                try
                {
                    //Con el nuevo string de conexion, pedimos el esquema de la base.
                    this.currentSnapshot.DomainTree = DomainTreeHelper.GetDomainTree(this.currentSnapshot.ConnectionString, null);
                }
                catch (Exception ex) {
                   
                    ShowError errorForm = new ShowError();
                    errorForm.LogTextBox.Text = ex.Message;
                    errorForm.ShowDialog();
                    return;
                }

                System.Data.Common.DbConnectionStringBuilder csb = new System.Data.Common.DbConnectionStringBuilder();
                csb.ConnectionString = this.currentSnapshot.ConnectionString ;

                if (csb.ContainsKey("Initial Catalog"))
                    this.currentSnapshot.DataBaseName = csb["Initial Catalog"].ToString();

                if (csb.ContainsKey("Database"))
                    this.currentSnapshot.DataBaseName = csb["Database"].ToString();

                this.currentSnapshot.FileName = "none";

                UI.DBNameLabel = this.currentSnapshot.DataBaseName;
                UI.ModelFileNameLabel = this.currentSnapshot.FileName;

                //this.DomainTree.Display(0);
                RecreateTreeView();

            }
            myDialog.Dispose();

            MainStatus_toolStripStatusLabel.Text = "Ready.";
        }

        private void RecreateTreeView()
        {
            treeView1.Nodes.Clear();
            TreeNode treeNode = new TreeNode("Model");
            treeNode.Checked = true;
            treeView1.Nodes.Add(treeNode);
            PopulateNode(this.currentSnapshot.DomainTree, treeNode);

            treeView1.Nodes[0].Expand();

        }

        private void PopulateNode(BaseTreeNode domainNode, TreeNode treeViewNode)
        {
            foreach (BaseTreeNode domNode in domainNode.Children)
            {
                TreeNode treeNode = new TreeNode(domNode.Name);
                treeNode.Tag = domNode;
                if ((domNode as PropertyNode) == null)
                {
                    // Es una entidad, la pinto
                    treeNode.NodeFont = new Font(this.treeView1.Font, FontStyle.Bold);
                    treeNode.Checked = ((EntityNode)domNode).GenerateObject;
                }
                else
                {
                    // Es propiedad
                    treeNode.Checked = ((PropertyNode)domNode).GenerateProperty;
                }


                treeViewNode.Nodes.Add(treeNode);
                PopulateNode(domNode, treeNode);
            }
        }



        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            propertyGrid1.SelectedObject = ((TreeNode)e.Node).Tag;
            if (mustCorrectCheckedNodes) CorrectCheckedNodes();
        }

        private void Save_toolStripButton_Click(object sender, EventArgs e)
        {

            // Save model to file
            if (String.IsNullOrEmpty(this.currentSnapshot.DataBaseName))
            {
                this.MainStatus_toolStripStatusLabel.Text = "Nothing to save.";
                return;
            }

            if (this.currentSnapshot.FileName == "none")
            {

                // Show save file dialog
                SaveFileDialog a = new SaveFileDialog();
                a.Title = "Save model";
                if (String.IsNullOrEmpty(this.currentSnapshot.FileName) || this.currentSnapshot.FileName == "none")
                    a.FileName = this.currentSnapshot.DataBaseName + ".Coop";
                else
                    a.FileName = this.currentSnapshot.FileName;

                a.Filter = "Cooperator's generator model files (*.Coop)|";
                a.RestoreDirectory = true; //Muy importante sino el generador no encuentra los archivos.

                if (a.ShowDialog() != DialogResult.Cancel)
                {
                    this.currentSnapshot.FileName = a.FileName;
                    savemodel(a.FileName);
                }
            }
            else
                savemodel(this.currentSnapshot.FileName);
        }



        private void savemodel(string fileName)
        {
            if (String.IsNullOrEmpty(fileName) || fileName == "none") {

                Save_toolStripButton_Click(this, new EventArgs());
                return;
            }

            this.currentSnapshot.FileName = fileName;

            // Serialize objects to file
            FileStream fstream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(fstream, this.currentSnapshot);
            fstream.Close();
            this.currentSnapshot.HaveChanges = false;
            UI.ModelFileNameLabel = fileName;
            UI.StatusLabel = "Model saved";
        }




        private void Load_toolStripButton_Click(object sender, EventArgs e)
        {
            // Open model file

            if (this.currentSnapshot.HaveChanges)
            {
                if (MessageBox.Show("The current model will be lost. Do you want to continue ?", "New model", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
            }
            
            
            // Show open file dialog
            OpenFileDialog a =  new OpenFileDialog();
            a.Title = "Load model";
            a.FileName = "*.Coop";
            a.Filter = "Cooperator's generator model files (*.Coop)|";
            a.RestoreDirectory = true; //Muy importante sino el generador no encuentra los archivos.

            if (a.ShowDialog() != DialogResult.Cancel)
            {

                // Deserialize the file
                OpenCoopFile(a.FileName);
                UI.StatusLabel = "Model loaded.";
            }
            else
            {
                UI.StatusLabel = "Ready.";
            }
        }


        private void OpenCoopFile(string file)
        {
            try
            {

                Snapshot MySnapshot = null; ;
                try
                {
                    FileStream fstream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None);
                    BinaryFormatter formatter = new BinaryFormatter();
                    MySnapshot = formatter.Deserialize(fstream) as Snapshot;
                    fstream.Close();
                }
                catch
                {
                    throw new System.ApplicationException("File not valid or file corrupt.");
                }

                // Update older repositories to this version
                if (MySnapshot.ModelerVersion != "1.4.5.0")
                    MySnapshot.ModelerVersion = "1.4.5.0";

                //if (!MySnapshot.ModelerVersion.Trim().StartsWith("1.3"))
                //    throw new System.ApplicationException(string.Format("Incorrect version. This file was created with Cooperator Modeler {0}.", MySnapshot.ModelerVersion));

                this.currentSnapshot = MySnapshot;
                this.currentSnapshot.FileName = file; // if the filename has changed.                   

                UI.DBNameLabel = this.currentSnapshot.DataBaseName;
                UI.ModelFileNameLabel = this.currentSnapshot.FileName;

                RecreateTreeView();

                this.currentSnapshot.HaveChanges = false;

            }
            catch (Exception ex)
            {
                this.currentSnapshot.FileName = "none";
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {

            //if (e.ChangedItem.Label.ToLower() == "generateobject" || e.ChangedItem.Label.ToLower() == "generateentity" || (e.ChangedItem.Label.ToLower() == "generateproperty" && (bool)e.ChangedItem.Value))  // propuesto por popov
            if (e.ChangedItem.Label.ToLower() == "generateobject" || e.ChangedItem.Label.ToLower() == "generateentity" || e.ChangedItem.Label.ToLower() == "generateproperty")
            { 
                // Cambiaron la propiedad generate. Debo actualizar el treeview
                foreach (TreeNode node in treeView1.Nodes[0].Nodes)
                {
                    if (node.Tag.Equals(propertyGrid1.SelectedObject))
                    {
                        // Es este nodo.
                        node.Checked = (bool)e.ChangedItem.Value;
                    }
                    // Como este arbol tiene 2 niveles, reviso el segundo nivel
                    foreach (TreeNode node2 in node.Nodes)
                    {
                        if (node2.Tag.Equals(propertyGrid1.SelectedObject))
                        {
                            // Es este nodo.
                            node2.Checked = (bool)e.ChangedItem.Value;
                        }
                    }
                }
            }

            if (e.ChangedItem.Label.ToLower() == "generateaschildof")
            {
                CorrectCheckedNodes();
            }
        }

        private void ChangeSelection_toolStripButton_Click(object sender, EventArgs e)
        {

            if (treeView1.Nodes.Count ==0) return;
            foreach (TreeNode node in treeView1.Nodes[0].Nodes)
            {
                node.Checked = !node.Checked;
            }

        }


        private void Refresh_toolStripButton_Click(object sender, EventArgs e)
        {

            if (String.IsNullOrEmpty(this.currentSnapshot.DataBaseName))
            {
                MainStatus_toolStripStatusLabel.Text = "Nothing to refresh.";
                return;
            }


            if (this.currentSnapshot.HaveChanges)
            {
                if (MessageBox.Show("The current model will be modified. Do you want to continue ?", "New model", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
            }
            
            try
            {

                //actualizamos el arbol de entidades leyendo de nuevo la base
                this.currentSnapshot.DomainTree = DomainTreeHelper.UpdateDomainTree(this.currentSnapshot.ConnectionString, this.currentSnapshot.DomainTree);

                //refrescamos el treeview
                RecreateTreeView();

            }

            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Server_toolStripButton_Click(object sender, EventArgs e)
        {

            if (String.IsNullOrEmpty(this.currentSnapshot.DataBaseName))
            {
                MainStatus_toolStripStatusLabel.Text = "Nothing to reconnect.";
                return;
            }
                
            
            // Pedimos el nuevo string de conexion.
            UI.StatusLabel = "Reconnect to database.";
            this.Refresh();
            ConnectForm myDialog = new ConnectForm();
            if (myDialog.ShowDialog(this) == DialogResult.OK)
            {
                this.currentSnapshot.ConnectionString = myDialog.ConnectionStringTextBox.Text;

                System.Data.Common.DbConnectionStringBuilder csb = new System.Data.Common.DbConnectionStringBuilder();
                csb.ConnectionString = this.currentSnapshot.ConnectionString;

                if (csb.ContainsKey("Initial Catalog"))
                    this.currentSnapshot.DataBaseName = csb["Initial Catalog"].ToString();

                if (csb.ContainsKey("Database"))
                    this.currentSnapshot.DataBaseName = csb["Database"].ToString();

                DatabaseName_StatusLabel.Text = "DB: " + this.currentSnapshot.DataBaseName;

                if (MessageBox.Show("Do you want to refresh the model ?", "Reconnect", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes )
                {
                    //Con el nuevo string de conexion, pedimos actualizar la base
                    this.currentSnapshot.DomainTree = DomainTreeHelper.UpdateDomainTree(this.currentSnapshot.ConnectionString, this.currentSnapshot.DomainTree);

                    //this.DomainTree.Display(0);
                    RecreateTreeView();
                }
            }

            myDialog.Dispose();
            UI.DBNameLabel = this.currentSnapshot.DataBaseName;
            UI.StatusLabel= "Ready.";

        }

        private void CodeGeneration_toolStripButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(this.currentSnapshot.DataBaseName))
            {
                UI.StatusLabel = "Nothing to generate.";
                return;
            }
            
            GenerationDialog objForm = new GenerationDialog();
            objForm.currentSnapshot = this.currentSnapshot;
            objForm.ShowDialog();
            return;

        }

        private void SPPrefix_textBox_Validated(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Text.ToLowerInvariant() == "sp_")
                tb.Text = "coop_";
        }


        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            // Si es el nodo raiz, no hacemos nada
            if (e.Node.Level == 0) return;

            // Cambio el estado de la propiedad Generate
            BaseTreeNode domNode = (BaseTreeNode)e.Node.Tag;

            if ((domNode as PropertyNode) == null)
            {
                // Es una entidad
                ((EntityNode)domNode).GenerateObject = e.Node.Checked;
                propertyGrid1.Refresh();
            }
            else
            {
                // Es una propiedad
                ((PropertyNode)domNode).GenerateProperty = e.Node.Checked;
                propertyGrid1.Refresh();
                mustCorrectCheckedNodes = true;
            }
        }


        bool mustCorrectCheckedNodes;
        private void treeView1_NodeMouseHover(object sender, TreeNodeMouseHoverEventArgs e)
        {
            if (mustCorrectCheckedNodes) CorrectCheckedNodes();
        }


        private void CorrectCheckedNodes()
        {
            this.treeView1.AfterCheck -= new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCheck);

            foreach (TreeNode entity in treeView1.Nodes[0].Nodes)
            {
                foreach (TreeNode property in entity.Nodes)
                {
                    PropertyNode propertyNode = (PropertyNode)property.Tag;
                    property.Checked = propertyNode.GenerateProperty;
                }
            }

            this.treeView1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCheck);
            mustCorrectCheckedNodes = false;
        }

        private void About_toolStripButton_Click(object sender, EventArgs e)
        {
            About objForm = new About();
            objForm.ShowDialog();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (args.Length == 1)
            {
                OpenCoopFile(Path.GetFullPath(args[0]));
            }
            // When the program is loaded by double click, the current diretory is not the original path.
            System.IO.Directory.SetCurrentDirectory(System.Windows.Forms.Application.StartupPath);
            this.Text = Application.ProductName + " - Version: " + Application.ProductVersion.ToString();
        }

        private void QuickGenerationToolStripButton_Click(object sender, EventArgs e)
        {

            if (String.IsNullOrEmpty(this.currentSnapshot.DataBaseName) || !Directory.Exists(this.currentSnapshot.DeployFolder))
            {
                UI.StatusLabel = "Nothing to generate.";
                return;
            }

            try
            {
                this.UseWaitCursor = true;
                System.Windows.Forms.Application.DoEvents();

                // Generate stored procedures
                UI.StatusLabel = "Generating stored procedures...";
                System.Windows.Forms.Application.DoEvents();
                CodeGenerator.GenerateStoredProcedures(this.currentSnapshot);
                System.Windows.Forms.Application.DoEvents();


                // Generate clases and projects
                UI.StatusLabel = "Generating source code...";
                System.Windows.Forms.Application.DoEvents();
                CodeGenerator.GenerateClasses(this.currentSnapshot);
                System.Windows.Forms.Application.DoEvents();

                // Update solution
                CodeGenerator.UpdateSolution(this.currentSnapshot, true);
                UI.StatusLabel = "Solution updated successfully";

            }
            catch (Exception)
            {
                UI.StatusLabel = "Quick generation error.";
            }
            finally
            {
                this.UseWaitCursor = false;
            }
        }

        private void FileAssociation_toolStripButton_Click(object sender, EventArgs e)
        {

            FileExtensionAssociation.RegisterFileExtension();

            UI.StatusLabel = ".Coop file extension associated successfully to Cooperator Modeler.";
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.currentSnapshot.HaveChanges)
            {
                switch (MessageBox.Show("Do you want to save the model ?", "Exit", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        savemodel(this.currentSnapshot.FileName);
                        if (this.currentSnapshot.HaveChanges) return; //Press cancel, abort exit
                        break;
                    case DialogResult.No:
                        // exit without save
                        break;
                    default:
                        // cancel exit      
                        e.Cancel = true;
                        break;
                }
            }
        }
    }
}
