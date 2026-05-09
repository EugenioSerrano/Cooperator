namespace CooperatorModeler
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.MainStatus_toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.SnapshotName_StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.DatabaseName_StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.Progress_toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.Connect_toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.Server_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.Refresh_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.CodeGeneration_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.QuickGenerationToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.Load_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.Save_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.ChangeSelection_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.FileAssociation_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.About_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.Exit_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.panel1 = new System.Windows.Forms.Panel();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MainStatus_toolStripStatusLabel,
            this.SnapshotName_StatusLabel,
            this.DatabaseName_StatusLabel,
            this.toolStripStatusLabel2,
            this.Progress_toolStripProgressBar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 582);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(884, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // MainStatus_toolStripStatusLabel
            // 
            this.MainStatus_toolStripStatusLabel.Margin = new System.Windows.Forms.Padding(0, 3, 15, 2);
            this.MainStatus_toolStripStatusLabel.Name = "MainStatus_toolStripStatusLabel";
            this.MainStatus_toolStripStatusLabel.Size = new System.Drawing.Size(538, 17);
            this.MainStatus_toolStripStatusLabel.Spring = true;
            this.MainStatus_toolStripStatusLabel.Text = "Ready.";
            this.MainStatus_toolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SnapshotName_StatusLabel
            // 
            this.SnapshotName_StatusLabel.Margin = new System.Windows.Forms.Padding(0, 3, 15, 2);
            this.SnapshotName_StatusLabel.Name = "SnapshotName_StatusLabel";
            this.SnapshotName_StatusLabel.Size = new System.Drawing.Size(66, 17);
            this.SnapshotName_StatusLabel.Text = "Model: none";
            // 
            // DatabaseName_StatusLabel
            // 
            this.DatabaseName_StatusLabel.Margin = new System.Windows.Forms.Padding(0, 3, 15, 2);
            this.DatabaseName_StatusLabel.Name = "DatabaseName_StatusLabel";
            this.DatabaseName_StatusLabel.Size = new System.Drawing.Size(51, 17);
            this.DatabaseName_StatusLabel.Text = "DB: none";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(53, 17);
            this.toolStripStatusLabel2.Text = "Progress:";
            // 
            // Progress_toolStripProgressBar
            // 
            this.Progress_toolStripProgressBar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.Progress_toolStripProgressBar.Margin = new System.Windows.Forms.Padding(1, 3, 15, 3);
            this.Progress_toolStripProgressBar.Name = "Progress_toolStripProgressBar";
            this.Progress_toolStripProgressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Connect_toolStripButton1,
            this.Server_toolStripButton,
            this.Refresh_toolStripButton,
            this.toolStripSeparator4,
            this.CodeGeneration_toolStripButton,
            this.QuickGenerationToolStripButton,
            this.toolStripSeparator3,
            this.Load_toolStripButton,
            this.Save_toolStripButton,
            this.toolStripSeparator2,
            this.ChangeSelection_toolStripButton,
            this.toolStripSeparator1,
            this.FileAssociation_toolStripButton,
            this.toolStripSeparator6,
            this.About_toolStripButton,
            this.toolStripSeparator5,
            this.Exit_toolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(884, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // Connect_toolStripButton1
            // 
            this.Connect_toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.Connect_toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("Connect_toolStripButton1.Image")));
            this.Connect_toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Connect_toolStripButton1.Name = "Connect_toolStripButton1";
            this.Connect_toolStripButton1.Size = new System.Drawing.Size(63, 22);
            this.Connect_toolStripButton1.Text = "New model";
            this.Connect_toolStripButton1.Click += new System.EventHandler(this.Connect_toolStripButton1_Click);
            // 
            // Server_toolStripButton
            // 
            this.Server_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.Server_toolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("Server_toolStripButton.Image")));
            this.Server_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Server_toolStripButton.Name = "Server_toolStripButton";
            this.Server_toolStripButton.Size = new System.Drawing.Size(62, 22);
            this.Server_toolStripButton.Text = "Reconnect";
            this.Server_toolStripButton.Click += new System.EventHandler(this.Server_toolStripButton_Click);
            // 
            // Refresh_toolStripButton
            // 
            this.Refresh_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.Refresh_toolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("Refresh_toolStripButton.Image")));
            this.Refresh_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Refresh_toolStripButton.Name = "Refresh_toolStripButton";
            this.Refresh_toolStripButton.Size = new System.Drawing.Size(49, 22);
            this.Refresh_toolStripButton.Text = "Refresh";
            this.Refresh_toolStripButton.Click += new System.EventHandler(this.Refresh_toolStripButton_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // CodeGeneration_toolStripButton
            // 
            this.CodeGeneration_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.CodeGeneration_toolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("CodeGeneration_toolStripButton.Image")));
            this.CodeGeneration_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.CodeGeneration_toolStripButton.Name = "CodeGeneration_toolStripButton";
            this.CodeGeneration_toolStripButton.Size = new System.Drawing.Size(91, 22);
            this.CodeGeneration_toolStripButton.Text = "Code generation";
            this.CodeGeneration_toolStripButton.Click += new System.EventHandler(this.CodeGeneration_toolStripButton_Click);
            // 
            // QuickGenerationToolStripButton
            // 
            this.QuickGenerationToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.QuickGenerationToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("QuickGenerationToolStripButton.Image")));
            this.QuickGenerationToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.QuickGenerationToolStripButton.Name = "QuickGenerationToolStripButton";
            this.QuickGenerationToolStripButton.Size = new System.Drawing.Size(92, 22);
            this.QuickGenerationToolStripButton.Text = "Quick generation";
            this.QuickGenerationToolStripButton.Click += new System.EventHandler(this.QuickGenerationToolStripButton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // Load_toolStripButton
            // 
            this.Load_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.Load_toolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("Load_toolStripButton.Image")));
            this.Load_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Load_toolStripButton.Name = "Load_toolStripButton";
            this.Load_toolStripButton.Size = new System.Drawing.Size(65, 22);
            this.Load_toolStripButton.Text = "Load model";
            this.Load_toolStripButton.Click += new System.EventHandler(this.Load_toolStripButton_Click);
            // 
            // Save_toolStripButton
            // 
            this.Save_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.Save_toolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("Save_toolStripButton.Image")));
            this.Save_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Save_toolStripButton.Name = "Save_toolStripButton";
            this.Save_toolStripButton.Size = new System.Drawing.Size(66, 22);
            this.Save_toolStripButton.Text = "Save model";
            this.Save_toolStripButton.Click += new System.EventHandler(this.Save_toolStripButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // ChangeSelection_toolStripButton
            // 
            this.ChangeSelection_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ChangeSelection_toolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("ChangeSelection_toolStripButton.Image")));
            this.ChangeSelection_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ChangeSelection_toolStripButton.Name = "ChangeSelection_toolStripButton";
            this.ChangeSelection_toolStripButton.Size = new System.Drawing.Size(93, 22);
            this.ChangeSelection_toolStripButton.Text = "Change selection";
            this.ChangeSelection_toolStripButton.Click += new System.EventHandler(this.ChangeSelection_toolStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // FileAssociation_toolStripButton
            // 
            this.FileAssociation_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.FileAssociation_toolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("FileAssociation_toolStripButton.Image")));
            this.FileAssociation_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.FileAssociation_toolStripButton.Name = "FileAssociation_toolStripButton";
            this.FileAssociation_toolStripButton.Size = new System.Drawing.Size(84, 22);
            this.FileAssociation_toolStripButton.Text = "File Association";
            this.FileAssociation_toolStripButton.Click += new System.EventHandler(this.FileAssociation_toolStripButton_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
            // 
            // About_toolStripButton
            // 
            this.About_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.About_toolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("About_toolStripButton.Image")));
            this.About_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.About_toolStripButton.Name = "About_toolStripButton";
            this.About_toolStripButton.Size = new System.Drawing.Size(40, 22);
            this.About_toolStripButton.Text = "About";
            this.About_toolStripButton.Click += new System.EventHandler(this.About_toolStripButton_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // Exit_toolStripButton
            // 
            this.Exit_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.Exit_toolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("Exit_toolStripButton.Image")));
            this.Exit_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Exit_toolStripButton.Name = "Exit_toolStripButton";
            this.Exit_toolStripButton.Size = new System.Drawing.Size(29, 22);
            this.Exit_toolStripButton.Text = "Exit";
            this.Exit_toolStripButton.Click += new System.EventHandler(this.Exit_toolStripButton_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 26);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.propertyGrid1);
            this.splitContainer1.Size = new System.Drawing.Size(884, 556);
            this.splitContainer1.SplitterDistance = 395;
            this.splitContainer1.TabIndex = 4;
            // 
            // treeView1
            // 
            this.treeView1.CheckBoxes = true;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.HideSelection = false;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(395, 556);
            this.treeView1.TabIndex = 1;
            this.treeView1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCheck);
            this.treeView1.NodeMouseHover += new System.Windows.Forms.TreeNodeMouseHoverEventHandler(this.treeView1_NodeMouseHover);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(485, 556);
            this.propertyGrid1.TabIndex = 2;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(884, 26);
            this.panel1.TabIndex = 5;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 604);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cooperator Modeler";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton Connect_toolStripButton1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.ToolStripButton Load_toolStripButton;
        private System.Windows.Forms.ToolStripButton Save_toolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton Exit_toolStripButton;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripButton CodeGeneration_toolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripButton ChangeSelection_toolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton Refresh_toolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton Server_toolStripButton;
        private System.Windows.Forms.ToolStripButton About_toolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        internal System.Windows.Forms.ToolStripStatusLabel MainStatus_toolStripStatusLabel;
        internal System.Windows.Forms.ToolStripProgressBar Progress_toolStripProgressBar;
        internal System.Windows.Forms.ToolStripStatusLabel DatabaseName_StatusLabel;
        internal System.Windows.Forms.ToolStripStatusLabel SnapshotName_StatusLabel;
        private System.Windows.Forms.ToolStripButton QuickGenerationToolStripButton;
        private System.Windows.Forms.ToolStripButton FileAssociation_toolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;



    }
}

