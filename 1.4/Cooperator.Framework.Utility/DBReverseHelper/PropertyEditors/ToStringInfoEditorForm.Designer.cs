namespace Cooperator.Framework.Utility.DBReverseHelper.PropertyEditors
{
    partial class ToStringInfoEditorForm
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
            this.Ok_button = new System.Windows.Forms.Button();
            this.Cancel_button = new System.Windows.Forms.Button();
            this.ToStringParams_listbox = new System.Windows.Forms.ListBox();
            this.Properties_listbox = new System.Windows.Forms.ListBox();
            this.Add_button = new System.Windows.Forms.Button();
            this.Remove_button = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.StringFormat_textbox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.OverrideToString_checkBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // Ok_button
            // 
            this.Ok_button.Location = new System.Drawing.Point(319, 236);
            this.Ok_button.Name = "Ok_button";
            this.Ok_button.Size = new System.Drawing.Size(75, 23);
            this.Ok_button.TabIndex = 3;
            this.Ok_button.Text = "Ok";
            this.Ok_button.UseVisualStyleBackColor = true;
            this.Ok_button.Click += new System.EventHandler(this.Ok_button_Click);
            // 
            // Cancel_button
            // 
            this.Cancel_button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel_button.Location = new System.Drawing.Point(400, 236);
            this.Cancel_button.Name = "Cancel_button";
            this.Cancel_button.Size = new System.Drawing.Size(75, 23);
            this.Cancel_button.TabIndex = 2;
            this.Cancel_button.Text = "Cancel";
            this.Cancel_button.UseVisualStyleBackColor = true;
            // 
            // ToStringParams_listbox
            // 
            this.ToStringParams_listbox.FormattingEnabled = true;
            this.ToStringParams_listbox.Location = new System.Drawing.Point(273, 70);
            this.ToStringParams_listbox.Name = "ToStringParams_listbox";
            this.ToStringParams_listbox.Size = new System.Drawing.Size(202, 160);
            this.ToStringParams_listbox.TabIndex = 4;
            // 
            // Properties_listbox
            // 
            this.Properties_listbox.FormattingEnabled = true;
            this.Properties_listbox.Location = new System.Drawing.Point(12, 70);
            this.Properties_listbox.Name = "Properties_listbox";
            this.Properties_listbox.Size = new System.Drawing.Size(202, 160);
            this.Properties_listbox.TabIndex = 5;
            // 
            // Add_button
            // 
            this.Add_button.Location = new System.Drawing.Point(220, 82);
            this.Add_button.Name = "Add_button";
            this.Add_button.Size = new System.Drawing.Size(47, 27);
            this.Add_button.TabIndex = 6;
            this.Add_button.Text = "&>";
            this.Add_button.UseVisualStyleBackColor = true;
            this.Add_button.Click += new System.EventHandler(this.Add_button_Click);
            // 
            // Remove_button
            // 
            this.Remove_button.Location = new System.Drawing.Point(220, 137);
            this.Remove_button.Name = "Remove_button";
            this.Remove_button.Size = new System.Drawing.Size(47, 27);
            this.Remove_button.TabIndex = 7;
            this.Remove_button.Text = "&<";
            this.Remove_button.UseVisualStyleBackColor = true;
            this.Remove_button.Click += new System.EventHandler(this.Remove_button_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "StringFormat";
            // 
            // StringFormat_textbox
            // 
            this.StringFormat_textbox.Location = new System.Drawing.Point(84, 28);
            this.StringFormat_textbox.Name = "StringFormat_textbox";
            this.StringFormat_textbox.Size = new System.Drawing.Size(391, 20);
            this.StringFormat_textbox.TabIndex = 9;
            this.StringFormat_textbox.TextChanged += new System.EventHandler(this.StrinFormat_textbox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Properties";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(270, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(119, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Params for StringFormat";
            // 
            // OverrideToString_checkBox
            // 
            this.OverrideToString_checkBox.AutoSize = true;
            this.OverrideToString_checkBox.Location = new System.Drawing.Point(13, 5);
            this.OverrideToString_checkBox.Name = "OverrideToString_checkBox";
            this.OverrideToString_checkBox.Size = new System.Drawing.Size(157, 17);
            this.OverrideToString_checkBox.TabIndex = 12;
            this.OverrideToString_checkBox.Text = "Override ToString() Method.";
            this.OverrideToString_checkBox.UseVisualStyleBackColor = true;
            this.OverrideToString_checkBox.CheckedChanged += new System.EventHandler(this.OverrideToString_checkBox_CheckedChanged);
            // 
            // ToStringInfoEditorForm
            // 
            this.AcceptButton = this.Ok_button;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel_button;
            this.ClientSize = new System.Drawing.Size(487, 271);
            this.Controls.Add(this.OverrideToString_checkBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.StringFormat_textbox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Remove_button);
            this.Controls.Add(this.Add_button);
            this.Controls.Add(this.Properties_listbox);
            this.Controls.Add(this.ToStringParams_listbox);
            this.Controls.Add(this.Ok_button);
            this.Controls.Add(this.Cancel_button);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ToStringInfoEditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ToStringInfo Editor";
            this.Load += new System.EventHandler(this.ToStringInfoEditorForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Ok_button;
        private System.Windows.Forms.Button Cancel_button;
        private System.Windows.Forms.Button Add_button;
        private System.Windows.Forms.Button Remove_button;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;

        /// <summary></summary>
        protected System.Windows.Forms.ListBox ToStringParams_listbox;

        /// <summary></summary>
        protected System.Windows.Forms.ListBox Properties_listbox;

        /// <summary></summary>
        protected System.Windows.Forms.TextBox StringFormat_textbox;

        private System.Windows.Forms.CheckBox OverrideToString_checkBox;
    }
}