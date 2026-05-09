namespace CooperatorModeler
{
    partial class GenerationDialog
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GenerationDialog));
            this.Deploy_groupBox = new System.Windows.Forms.GroupBox();
            this.FolderBrowser_button = new System.Windows.Forms.Button();
            this.Update_button = new System.Windows.Forms.Button();
            this.UpdateOnlyAutoFile_checkBox = new System.Windows.Forms.CheckBox();
            this.CreateSolution_button = new System.Windows.Forms.Button();
            this.DeployFolder_textBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SourceCode_groupBox = new System.Windows.Forms.GroupBox();
            this.StampDateAndTimeCheckBox = new System.Windows.Forms.CheckBox();
            this.EntitiesProjectName_textBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.RulesProjectName_textBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.DataProjectName_textBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.AppProjectName_textBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.GenerateSourceCode_button = new System.Windows.Forms.Button();
            this.VB_radioButton = new System.Windows.Forms.RadioButton();
            this.CS_radioButton = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.StoredProcedures_groupBox = new System.Windows.Forms.GroupBox();
            this.PostGenerationScriptButton = new System.Windows.Forms.Button();
            this.connectionStringTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.GenerateSP_button = new System.Windows.Forms.Button();
            this.SPPrefix_textBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.Cancel_button = new System.Windows.Forms.Button();
            this.Ok_button = new System.Windows.Forms.Button();
            this.CheckForCallerTokenCheckBox = new System.Windows.Forms.CheckBox();
            this.Deploy_groupBox.SuspendLayout();
            this.SourceCode_groupBox.SuspendLayout();
            this.StoredProcedures_groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // Deploy_groupBox
            // 
            this.Deploy_groupBox.Controls.Add(this.FolderBrowser_button);
            this.Deploy_groupBox.Controls.Add(this.Update_button);
            this.Deploy_groupBox.Controls.Add(this.UpdateOnlyAutoFile_checkBox);
            this.Deploy_groupBox.Controls.Add(this.CreateSolution_button);
            this.Deploy_groupBox.Controls.Add(this.DeployFolder_textBox);
            this.Deploy_groupBox.Controls.Add(this.label3);
            this.Deploy_groupBox.Enabled = false;
            this.Deploy_groupBox.Location = new System.Drawing.Point(7, 276);
            this.Deploy_groupBox.Name = "Deploy_groupBox";
            this.Deploy_groupBox.Size = new System.Drawing.Size(369, 107);
            this.Deploy_groupBox.TabIndex = 20;
            this.Deploy_groupBox.TabStop = false;
            this.Deploy_groupBox.Text = "3 - Deployment";
            // 
            // FolderBrowser_button
            // 
            this.FolderBrowser_button.Location = new System.Drawing.Point(229, 39);
            this.FolderBrowser_button.Name = "FolderBrowser_button";
            this.FolderBrowser_button.Size = new System.Drawing.Size(25, 23);
            this.FolderBrowser_button.TabIndex = 10;
            this.FolderBrowser_button.Text = "...";
            this.FolderBrowser_button.UseVisualStyleBackColor = true;
            this.FolderBrowser_button.Click += new System.EventHandler(this.FolderBrowser_button_Click);
            // 
            // Update_button
            // 
            this.Update_button.Location = new System.Drawing.Point(273, 61);
            this.Update_button.Name = "Update_button";
            this.Update_button.Size = new System.Drawing.Size(90, 40);
            this.Update_button.TabIndex = 12;
            this.Update_button.Text = "Update solution";
            this.Update_button.UseVisualStyleBackColor = true;
            this.Update_button.Click += new System.EventHandler(this.Update_button_Click);
            // 
            // UpdateOnlyAutoFile_checkBox
            // 
            this.UpdateOnlyAutoFile_checkBox.AutoSize = true;
            this.UpdateOnlyAutoFile_checkBox.Checked = true;
            this.UpdateOnlyAutoFile_checkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.UpdateOnlyAutoFile_checkBox.Location = new System.Drawing.Point(10, 74);
            this.UpdateOnlyAutoFile_checkBox.Name = "UpdateOnlyAutoFile_checkBox";
            this.UpdateOnlyAutoFile_checkBox.Size = new System.Drawing.Size(132, 17);
            this.UpdateOnlyAutoFile_checkBox.TabIndex = 11;
            this.UpdateOnlyAutoFile_checkBox.Text = "Update only .Auto files";
            this.UpdateOnlyAutoFile_checkBox.UseVisualStyleBackColor = true;
            // 
            // CreateSolution_button
            // 
            this.CreateSolution_button.Location = new System.Drawing.Point(273, 15);
            this.CreateSolution_button.Name = "CreateSolution_button";
            this.CreateSolution_button.Size = new System.Drawing.Size(90, 40);
            this.CreateSolution_button.TabIndex = 11;
            this.CreateSolution_button.Text = "Create solution";
            this.CreateSolution_button.UseVisualStyleBackColor = true;
            this.CreateSolution_button.Click += new System.EventHandler(this.CreateSolution_button_Click);
            // 
            // DeployFolder_textBox
            // 
            this.DeployFolder_textBox.Location = new System.Drawing.Point(9, 41);
            this.DeployFolder_textBox.Name = "DeployFolder_textBox";
            this.DeployFolder_textBox.Size = new System.Drawing.Size(217, 20);
            this.DeployFolder_textBox.TabIndex = 9;
            this.DeployFolder_textBox.Text = "Select folder...";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Folder:";
            // 
            // SourceCode_groupBox
            // 
            this.SourceCode_groupBox.Controls.Add(this.CheckForCallerTokenCheckBox);
            this.SourceCode_groupBox.Controls.Add(this.StampDateAndTimeCheckBox);
            this.SourceCode_groupBox.Controls.Add(this.EntitiesProjectName_textBox);
            this.SourceCode_groupBox.Controls.Add(this.label7);
            this.SourceCode_groupBox.Controls.Add(this.RulesProjectName_textBox);
            this.SourceCode_groupBox.Controls.Add(this.label6);
            this.SourceCode_groupBox.Controls.Add(this.DataProjectName_textBox);
            this.SourceCode_groupBox.Controls.Add(this.label5);
            this.SourceCode_groupBox.Controls.Add(this.AppProjectName_textBox);
            this.SourceCode_groupBox.Controls.Add(this.label4);
            this.SourceCode_groupBox.Controls.Add(this.GenerateSourceCode_button);
            this.SourceCode_groupBox.Controls.Add(this.VB_radioButton);
            this.SourceCode_groupBox.Controls.Add(this.CS_radioButton);
            this.SourceCode_groupBox.Controls.Add(this.label1);
            this.SourceCode_groupBox.Location = new System.Drawing.Point(7, 118);
            this.SourceCode_groupBox.Name = "SourceCode_groupBox";
            this.SourceCode_groupBox.Size = new System.Drawing.Size(369, 152);
            this.SourceCode_groupBox.TabIndex = 19;
            this.SourceCode_groupBox.TabStop = false;
            this.SourceCode_groupBox.Text = "2 - Source code generation";
            // 
            // StampDateAndTimeCheckBox
            // 
            this.StampDateAndTimeCheckBox.Checked = true;
            this.StampDateAndTimeCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.StampDateAndTimeCheckBox.Location = new System.Drawing.Point(275, 10);
            this.StampDateAndTimeCheckBox.Name = "StampDateAndTimeCheckBox";
            this.StampDateAndTimeCheckBox.Size = new System.Drawing.Size(90, 52);
            this.StampDateAndTimeCheckBox.TabIndex = 13;
            this.StampDateAndTimeCheckBox.Text = "Stamp date and time on .Auto files";
            this.StampDateAndTimeCheckBox.UseVisualStyleBackColor = true;
            // 
            // EntitiesProjectName_textBox
            // 
            this.EntitiesProjectName_textBox.Location = new System.Drawing.Point(115, 125);
            this.EntitiesProjectName_textBox.Name = "EntitiesProjectName_textBox";
            this.EntitiesProjectName_textBox.Size = new System.Drawing.Size(139, 20);
            this.EntitiesProjectName_textBox.TabIndex = 7;
            this.EntitiesProjectName_textBox.Text = "EntitiesProjectName";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 128);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(108, 13);
            this.label7.TabIndex = 20;
            this.label7.Text = "Entities project name:";
            // 
            // RulesProjectName_textBox
            // 
            this.RulesProjectName_textBox.Location = new System.Drawing.Point(115, 73);
            this.RulesProjectName_textBox.Name = "RulesProjectName_textBox";
            this.RulesProjectName_textBox.Size = new System.Drawing.Size(139, 20);
            this.RulesProjectName_textBox.TabIndex = 5;
            this.RulesProjectName_textBox.Text = "RulesProjectName";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 76);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(101, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "Rules project name:";
            // 
            // DataProjectName_textBox
            // 
            this.DataProjectName_textBox.Location = new System.Drawing.Point(115, 99);
            this.DataProjectName_textBox.Name = "DataProjectName_textBox";
            this.DataProjectName_textBox.Size = new System.Drawing.Size(139, 20);
            this.DataProjectName_textBox.TabIndex = 6;
            this.DataProjectName_textBox.Text = "DataProjectName";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 102);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Data project name:";
            // 
            // AppProjectName_textBox
            // 
            this.AppProjectName_textBox.Location = new System.Drawing.Point(115, 47);
            this.AppProjectName_textBox.Name = "AppProjectName_textBox";
            this.AppProjectName_textBox.Size = new System.Drawing.Size(139, 20);
            this.AppProjectName_textBox.TabIndex = 4;
            this.AppProjectName_textBox.Text = "AppProjectName";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 50);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Application name:";
            // 
            // GenerateSourceCode_button
            // 
            this.GenerateSourceCode_button.Location = new System.Drawing.Point(273, 105);
            this.GenerateSourceCode_button.Name = "GenerateSourceCode_button";
            this.GenerateSourceCode_button.Size = new System.Drawing.Size(90, 40);
            this.GenerateSourceCode_button.TabIndex = 8;
            this.GenerateSourceCode_button.Text = "Generate source code";
            this.GenerateSourceCode_button.UseVisualStyleBackColor = true;
            this.GenerateSourceCode_button.Click += new System.EventHandler(this.GenerateSourceCode_button_Click);
            // 
            // VB_radioButton
            // 
            this.VB_radioButton.AutoSize = true;
            this.VB_radioButton.Checked = true;
            this.VB_radioButton.Location = new System.Drawing.Point(172, 23);
            this.VB_radioButton.Name = "VB_radioButton";
            this.VB_radioButton.Size = new System.Drawing.Size(82, 17);
            this.VB_radioButton.TabIndex = 3;
            this.VB_radioButton.TabStop = true;
            this.VB_radioButton.Text = "Visual Basic";
            this.VB_radioButton.UseVisualStyleBackColor = true;
            this.VB_radioButton.CheckedChanged += new System.EventHandler(this.VB_radioButton_CheckedChanged);
            // 
            // CS_radioButton
            // 
            this.CS_radioButton.AutoSize = true;
            this.CS_radioButton.Location = new System.Drawing.Point(106, 23);
            this.CS_radioButton.Name = "CS_radioButton";
            this.CS_radioButton.Size = new System.Drawing.Size(39, 17);
            this.CS_radioButton.TabIndex = 2;
            this.CS_radioButton.Text = "C#";
            this.CS_radioButton.UseVisualStyleBackColor = true;
            this.CS_radioButton.CheckedChanged += new System.EventHandler(this.CS_radioButton_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Language:";
            // 
            // StoredProcedures_groupBox
            // 
            this.StoredProcedures_groupBox.Controls.Add(this.PostGenerationScriptButton);
            this.StoredProcedures_groupBox.Controls.Add(this.connectionStringTextBox);
            this.StoredProcedures_groupBox.Controls.Add(this.label8);
            this.StoredProcedures_groupBox.Controls.Add(this.GenerateSP_button);
            this.StoredProcedures_groupBox.Controls.Add(this.SPPrefix_textBox);
            this.StoredProcedures_groupBox.Controls.Add(this.label2);
            this.StoredProcedures_groupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StoredProcedures_groupBox.Location = new System.Drawing.Point(7, 5);
            this.StoredProcedures_groupBox.Name = "StoredProcedures_groupBox";
            this.StoredProcedures_groupBox.Size = new System.Drawing.Size(369, 107);
            this.StoredProcedures_groupBox.TabIndex = 18;
            this.StoredProcedures_groupBox.TabStop = false;
            this.StoredProcedures_groupBox.Text = "1 - Stored procedures generation";
            // 
            // PostGenerationScriptButton
            // 
            this.PostGenerationScriptButton.Location = new System.Drawing.Point(273, 15);
            this.PostGenerationScriptButton.Name = "PostGenerationScriptButton";
            this.PostGenerationScriptButton.Size = new System.Drawing.Size(90, 40);
            this.PostGenerationScriptButton.TabIndex = 13;
            this.PostGenerationScriptButton.Text = "Post-generation script ...";
            this.PostGenerationScriptButton.UseVisualStyleBackColor = true;
            this.PostGenerationScriptButton.Click += new System.EventHandler(this.PostGenerationScriptButton_Click);
            // 
            // connectionStringTextBox
            // 
            this.connectionStringTextBox.Enabled = false;
            this.connectionStringTextBox.Location = new System.Drawing.Point(9, 62);
            this.connectionStringTextBox.Multiline = true;
            this.connectionStringTextBox.Name = "connectionStringTextBox";
            this.connectionStringTextBox.Size = new System.Drawing.Size(245, 39);
            this.connectionStringTextBox.TabIndex = 12;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 46);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(92, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "Connection string:";
            // 
            // GenerateSP_button
            // 
            this.GenerateSP_button.Location = new System.Drawing.Point(273, 61);
            this.GenerateSP_button.Name = "GenerateSP_button";
            this.GenerateSP_button.Size = new System.Drawing.Size(90, 40);
            this.GenerateSP_button.TabIndex = 1;
            this.GenerateSP_button.Text = "Generate SPs";
            this.GenerateSP_button.UseVisualStyleBackColor = true;
            this.GenerateSP_button.Click += new System.EventHandler(this.GenerateSP_button_Click);
            // 
            // SPPrefix_textBox
            // 
            this.SPPrefix_textBox.Location = new System.Drawing.Point(137, 22);
            this.SPPrefix_textBox.Name = "SPPrefix_textBox";
            this.SPPrefix_textBox.Size = new System.Drawing.Size(117, 20);
            this.SPPrefix_textBox.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Stored procedures prefix:";
            // 
            // Cancel_button
            // 
            this.Cancel_button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel_button.Location = new System.Drawing.Point(295, 389);
            this.Cancel_button.Name = "Cancel_button";
            this.Cancel_button.Size = new System.Drawing.Size(75, 23);
            this.Cancel_button.TabIndex = 14;
            this.Cancel_button.Text = "Cancel";
            this.Cancel_button.UseVisualStyleBackColor = true;
            // 
            // Ok_button
            // 
            this.Ok_button.Location = new System.Drawing.Point(214, 389);
            this.Ok_button.Name = "Ok_button";
            this.Ok_button.Size = new System.Drawing.Size(75, 23);
            this.Ok_button.TabIndex = 13;
            this.Ok_button.Text = "Ok";
            this.Ok_button.UseVisualStyleBackColor = true;
            this.Ok_button.Click += new System.EventHandler(this.Ok_button_Click);
            // 
            // CheckForCallerTokenCheckBox
            // 
            this.CheckForCallerTokenCheckBox.Checked = true;
            this.CheckForCallerTokenCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckForCallerTokenCheckBox.Location = new System.Drawing.Point(273, 62);
            this.CheckForCallerTokenCheckBox.Name = "CheckForCallerTokenCheckBox";
            this.CheckForCallerTokenCheckBox.Size = new System.Drawing.Size(90, 37);
            this.CheckForCallerTokenCheckBox.TabIndex = 21;
            this.CheckForCallerTokenCheckBox.Text = "Check for caller\'s token";
            this.CheckForCallerTokenCheckBox.UseVisualStyleBackColor = true;
            // 
            // GenerationDialog
            // 
            this.AcceptButton = this.Ok_button;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel_button;
            this.ClientSize = new System.Drawing.Size(382, 424);
            this.Controls.Add(this.Ok_button);
            this.Controls.Add(this.Cancel_button);
            this.Controls.Add(this.Deploy_groupBox);
            this.Controls.Add(this.SourceCode_groupBox);
            this.Controls.Add(this.StoredProcedures_groupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GenerationDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Code generation";
            this.Load += new System.EventHandler(this.GenerationDialog_Load);
            this.Deploy_groupBox.ResumeLayout(false);
            this.Deploy_groupBox.PerformLayout();
            this.SourceCode_groupBox.ResumeLayout(false);
            this.SourceCode_groupBox.PerformLayout();
            this.StoredProcedures_groupBox.ResumeLayout(false);
            this.StoredProcedures_groupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox Deploy_groupBox;
        private System.Windows.Forms.Button FolderBrowser_button;
        private System.Windows.Forms.Button Update_button;
        private System.Windows.Forms.CheckBox UpdateOnlyAutoFile_checkBox;
        private System.Windows.Forms.Button CreateSolution_button;
        private System.Windows.Forms.TextBox DeployFolder_textBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox SourceCode_groupBox;
        private System.Windows.Forms.TextBox AppProjectName_textBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button GenerateSourceCode_button;
        private System.Windows.Forms.RadioButton VB_radioButton;
        private System.Windows.Forms.RadioButton CS_radioButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox StoredProcedures_groupBox;
        private System.Windows.Forms.Button GenerateSP_button;
        private System.Windows.Forms.TextBox SPPrefix_textBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox RulesProjectName_textBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox DataProjectName_textBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button Cancel_button;
        private System.Windows.Forms.Button Ok_button;
        private System.Windows.Forms.TextBox EntitiesProjectName_textBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox connectionStringTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox StampDateAndTimeCheckBox;
        private System.Windows.Forms.Button PostGenerationScriptButton;
        private System.Windows.Forms.CheckBox CheckForCallerTokenCheckBox;
    }
}