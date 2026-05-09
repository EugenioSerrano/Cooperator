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
using System.Windows.Forms;

namespace CooperatorModeler
{
    public partial class GenerationDialog : Form
    {

        public Snapshot currentSnapshot;

        public GenerationDialog()
        {
            InitializeComponent();
        }

        private void Close_button_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FolderBrowser_button_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                DeployFolder_textBox.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void GenerationDialog_Load(object sender, EventArgs e)
        {
            SPPrefix_textBox.Text = currentSnapshot.SPPrefix;
            if (currentSnapshot.Language == "CS")
            {
                CS_radioButton.Checked = true;
            }
            else
            {
                VB_radioButton.Checked = true;
            }

            AppProjectName_textBox.Text = currentSnapshot.AppProjectName;
            DataProjectName_textBox.Text = currentSnapshot.DataProjectName;
            RulesProjectName_textBox.Text = currentSnapshot.RulesProjectName;
            EntitiesProjectName_textBox.Text = currentSnapshot.EntitiesProjectName;
            DeployFolder_textBox.Text = currentSnapshot.DeployFolder;
            connectionStringTextBox.Text = currentSnapshot.ConnectionString;
            StampDateAndTimeCheckBox.Checked = currentSnapshot.StampDateAndTimeOnAutoFiles;
            CheckForCallerTokenCheckBox.Checked = currentSnapshot.GenerateCheckForToken;

            if (string.IsNullOrEmpty(SPPrefix_textBox.Text))
            {
                SPPrefix_textBox.Text = "Coop_";
            }

            if (string.IsNullOrEmpty(AppProjectName_textBox.Text))
            {
                AppProjectName_textBox.Text = currentSnapshot.DataBaseName;
            }

            if (string.IsNullOrEmpty(DataProjectName_textBox.Text))
            {
                DataProjectName_textBox.Text = currentSnapshot.DataBaseName + "Data";
            }

            if (string.IsNullOrEmpty(RulesProjectName_textBox.Text))
            {
                RulesProjectName_textBox.Text = currentSnapshot.DataBaseName + "Rules";
            }

            if (string.IsNullOrEmpty(EntitiesProjectName_textBox.Text))
            {
                EntitiesProjectName_textBox.Text = currentSnapshot.DataBaseName + "Entities";
            }

            if (string.IsNullOrEmpty(DeployFolder_textBox.Text))
            {
                DeployFolder_textBox.Text = "Select folder...";
            }
        }

        private void Ok_button_Click(object sender, EventArgs e)
        {
            currentSnapshot.SPPrefix = SPPrefix_textBox.Text;
            currentSnapshot.AppProjectName = AppProjectName_textBox.Text;
            currentSnapshot.DataProjectName = DataProjectName_textBox.Text;
            currentSnapshot.RulesProjectName = RulesProjectName_textBox.Text;
            currentSnapshot.EntitiesProjectName = EntitiesProjectName_textBox.Text;
            currentSnapshot.StampDateAndTimeOnAutoFiles = StampDateAndTimeCheckBox.Checked;
            currentSnapshot.GenerateCheckForToken = CheckForCallerTokenCheckBox.Checked;

            currentSnapshot.Language = CS_radioButton.Checked 
                ? "CS" 
                : "VB";

            currentSnapshot.DeployFolder = DeployFolder_textBox.Text;

            Close();
        }

        private void GenerateSP_button_Click(object sender, EventArgs e)
        {
            currentSnapshot.SPPrefix = this.SPPrefix_textBox.Text;
            // Generate SPs
            UI.StatusLabel = "Generating stored procedures...";
            Cooperator.Framework.Utility.DBReverseHelper.DomainTreeHelper.ResetNamespaceCollection();
            Application.DoEvents();
            CodeGenerator.GenerateStoredProcedures(this.currentSnapshot);
        }

        private void GenerateSourceCode_button_Click(object sender, EventArgs e)
        {
            currentSnapshot.AppProjectName = AppProjectName_textBox.Text;
            currentSnapshot.DataProjectName = DataProjectName_textBox.Text;
            currentSnapshot.RulesProjectName = RulesProjectName_textBox.Text;
            currentSnapshot.EntitiesProjectName = EntitiesProjectName_textBox.Text;
            currentSnapshot.StampDateAndTimeOnAutoFiles = StampDateAndTimeCheckBox.Checked;
            currentSnapshot.GenerateCheckForToken = CheckForCallerTokenCheckBox.Checked;

            currentSnapshot.Language = CS_radioButton.Checked 
                ? "CS" 
                : "VB";

            // Generate clases and projects
            UI.StatusLabel = "Generating source code...";
            Application.DoEvents();
            Cooperator.Framework.Utility.DBReverseHelper.DomainTreeHelper.ResetNamespaceCollection();
            CodeGenerator.GenerateClasses(currentSnapshot);
            Deploy_groupBox.Enabled = true;
        }

        private void CreateSolution_button_Click(object sender, EventArgs e)
        {
            currentSnapshot.DeployFolder = DeployFolder_textBox.Text;
            UI.ProgessBar.Maximum =2;
            UI.ProgessBar.Value = 1;
            UI.StatusLabel = "Creating solution...";
            Application.DoEvents();

            // Create solution
            if (CodeGenerator.GenerateSolution(currentSnapshot))
            {
                UI.StatusLabel = "Solution created";
                UI.ProgessBar.Value = UI.ProgessBar.Maximum;
                if (MessageBox.Show("Solution created successfully. Do you like open it ?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2 ) == DialogResult.Yes)
                {
                    var solFile = currentSnapshot.DeployFolder + "\\" + currentSnapshot.AppProjectName + ".sln";
                    System.Diagnostics.Process.Start(solFile);
                }
            }
            else
            {
                UI.StatusLabel = "Error creating solution";
                UI.ProgessBar.Value = UI.ProgessBar.Maximum;
            }

        }

        private void Update_button_Click(object sender, EventArgs e)
        {
            // Update solution
            UI.ProgessBar.Maximum = 2;
            UI.ProgessBar.Value = 1;
            Application.DoEvents();
            this.currentSnapshot.DeployFolder = DeployFolder_textBox.Text;
            if (CodeGenerator.UpdateSolution(currentSnapshot, UpdateOnlyAutoFile_checkBox.Checked))
            {
                UI.StatusLabel = "Solution updated successfully";
                UI.ProgessBar.Value = 2;
                if (MessageBox.Show("Solution updated successfully. Do you like open it ?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2 ) == DialogResult.Yes)
                {
                    string solFile = this.currentSnapshot.DeployFolder + "\\" + this.currentSnapshot.AppProjectName + ".sln";
                    if (!System.IO.File.Exists(solFile))
                    {
                        MessageBox.Show(
                            $"The file [{solFile}] can't be found. \r\nIf you solution file was renamed, please change the Application name.");
                        return;
                    }
                    System.Diagnostics.Process.Start(solFile);
                }
            }
            else
            {
                UI.StatusLabel = "Error updating solution";
                UI.ProgessBar.Value = UI.ProgessBar.Maximum;
            }
        }

        private void CS_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            Deploy_groupBox.Enabled = false;
        }

        private void VB_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            Deploy_groupBox.Enabled = false;
        }

        private void PostGenerationScriptButton_Click(object sender, EventArgs e)
        {
            var scriptDialog = new PostGenerationScriptDialog
            {
                CurrentSnapshot = currentSnapshot            
            };
            scriptDialog.ShowDialog(this);
        }


    }
}
