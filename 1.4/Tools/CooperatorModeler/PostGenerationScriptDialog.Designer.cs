namespace CooperatorModeler
{
    partial class PostGenerationScriptDialog
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
			this.Ok_Button = new System.Windows.Forms.Button();
			this.Cancel_Button = new System.Windows.Forms.Button();
			this.ScriptTextBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// Ok_Button
			// 
			this.Ok_Button.Location = new System.Drawing.Point(501, 428);
			this.Ok_Button.Name = "Ok_Button";
			this.Ok_Button.Size = new System.Drawing.Size(75, 23);
			this.Ok_Button.TabIndex = 0;
			this.Ok_Button.Text = "&Ok";
			this.Ok_Button.UseVisualStyleBackColor = true;
			this.Ok_Button.Click += new System.EventHandler(this.OkButtonClick);
			// 
			// Cancel_Button
			// 
			this.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Cancel_Button.Location = new System.Drawing.Point(583, 428);
			this.Cancel_Button.Name = "Cancel_Button";
			this.Cancel_Button.Size = new System.Drawing.Size(75, 23);
			this.Cancel_Button.TabIndex = 1;
			this.Cancel_Button.Text = "&Cancel";
			this.Cancel_Button.UseVisualStyleBackColor = true;
			// 
			// ScriptTextBox
			// 
			this.ScriptTextBox.Location = new System.Drawing.Point(13, 13);
			this.ScriptTextBox.MaxLength = 65536;
			this.ScriptTextBox.Multiline = true;
			this.ScriptTextBox.Name = "ScriptTextBox";
			this.ScriptTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.ScriptTextBox.Size = new System.Drawing.Size(645, 409);
			this.ScriptTextBox.TabIndex = 2;
			// 
			// PostGenerationScriptDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(670, 463);
			this.Controls.Add(this.ScriptTextBox);
			this.Controls.Add(this.Cancel_Button);
			this.Controls.Add(this.Ok_Button);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PostGenerationScriptDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Post-generation SQL Script";
			this.Load += new System.EventHandler(this.PostGenerationScriptDialogLoad);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Ok_Button;
        private System.Windows.Forms.Button Cancel_Button;
        private System.Windows.Forms.TextBox ScriptTextBox;
    }
}