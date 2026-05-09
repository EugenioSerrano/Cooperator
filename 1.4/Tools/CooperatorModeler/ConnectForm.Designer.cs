namespace CooperatorModeler
{
	partial class ConnectForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectForm));
            this.label1 = new System.Windows.Forms.Label();
            this.ServersComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.DataBasesListBox = new System.Windows.Forms.ListBox();
            this.Ok_Button = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.ConnectionStringTextBox = new System.Windows.Forms.TextBox();
            this.Cancel_Button = new System.Windows.Forms.Button();
            this.GetDatabasesbutton = new System.Windows.Forms.Button();
            this.FindServersbutton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Server:";
            // 
            // ServersComboBox
            // 
            this.ServersComboBox.FormattingEnabled = true;
            this.ServersComboBox.Location = new System.Drawing.Point(86, 9);
            this.ServersComboBox.Name = "ServersComboBox";
            this.ServersComboBox.Size = new System.Drawing.Size(163, 21);
            this.ServersComboBox.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "&Database:";
            // 
            // DataBasesListBox
            // 
            this.DataBasesListBox.FormattingEnabled = true;
            this.DataBasesListBox.Location = new System.Drawing.Point(86, 36);
            this.DataBasesListBox.Name = "DataBasesListBox";
            this.DataBasesListBox.Size = new System.Drawing.Size(163, 82);
            this.DataBasesListBox.TabIndex = 1;
            this.DataBasesListBox.SelectedIndexChanged += new System.EventHandler(this.lbBases_SelectedIndexChanged);
            // 
            // Ok_Button
            // 
            this.Ok_Button.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Ok_Button.Enabled = false;
            this.Ok_Button.Location = new System.Drawing.Point(265, 63);
            this.Ok_Button.Name = "Ok_Button";
            this.Ok_Button.Size = new System.Drawing.Size(91, 26);
            this.Ok_Button.TabIndex = 4;
            this.Ok_Button.Text = "&Ok";
            this.Ok_Button.Click += new System.EventHandler(this.Ok_Button_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 131);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Connection String:";
            // 
            // ConnectionStringTextBox
            // 
            this.ConnectionStringTextBox.Location = new System.Drawing.Point(18, 147);
            this.ConnectionStringTextBox.Multiline = true;
            this.ConnectionStringTextBox.Name = "ConnectionStringTextBox";
            this.ConnectionStringTextBox.Size = new System.Drawing.Size(338, 60);
            this.ConnectionStringTextBox.TabIndex = 6;
            this.ConnectionStringTextBox.TextChanged += new System.EventHandler(this.txtConnectionString_TextChanged);
            // 
            // Cancel_Button
            // 
            this.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel_Button.Location = new System.Drawing.Point(265, 91);
            this.Cancel_Button.Name = "Cancel_Button";
            this.Cancel_Button.Size = new System.Drawing.Size(91, 26);
            this.Cancel_Button.TabIndex = 5;
            this.Cancel_Button.Text = "Cancel";
            this.Cancel_Button.UseVisualStyleBackColor = true;
            this.Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
            // 
            // GetDatabasesbutton
            // 
            this.GetDatabasesbutton.Location = new System.Drawing.Point(265, 35);
            this.GetDatabasesbutton.Name = "GetDatabasesbutton";
            this.GetDatabasesbutton.Size = new System.Drawing.Size(91, 26);
            this.GetDatabasesbutton.TabIndex = 3;
            this.GetDatabasesbutton.Text = "Get Databases";
            this.GetDatabasesbutton.UseVisualStyleBackColor = true;
            this.GetDatabasesbutton.Click += new System.EventHandler(this.GetDatabasesbutton_Click);
            // 
            // FindServersbutton
            // 
            this.FindServersbutton.Location = new System.Drawing.Point(265, 7);
            this.FindServersbutton.Name = "FindServersbutton";
            this.FindServersbutton.Size = new System.Drawing.Size(91, 26);
            this.FindServersbutton.TabIndex = 2;
            this.FindServersbutton.Text = "Find Servers...";
            this.FindServersbutton.Click += new System.EventHandler(this.BuscarMasbutton_Click);
            // 
            // ConnectForm
            // 
            this.AcceptButton = this.Ok_Button;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel_Button;
            this.ClientSize = new System.Drawing.Size(375, 227);
            this.Controls.Add(this.FindServersbutton);
            this.Controls.Add(this.GetDatabasesbutton);
            this.Controls.Add(this.Cancel_Button);
            this.Controls.Add(this.ConnectionStringTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Ok_Button);
            this.Controls.Add(this.DataBasesListBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ServersComboBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConnectForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Connection";
            this.Shown += new System.EventHandler(this.ConnectForm_Shown);
            this.Load += new System.EventHandler(this.ConnectForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox ServersComboBox;
        private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button Ok_Button;
		private System.Windows.Forms.Label label3;
		internal System.Windows.Forms.TextBox ConnectionStringTextBox;
        internal System.Windows.Forms.ListBox DataBasesListBox;
        private System.Windows.Forms.Button Cancel_Button;
        private System.Windows.Forms.Button GetDatabasesbutton;
        private System.Windows.Forms.Button FindServersbutton;
	}
}