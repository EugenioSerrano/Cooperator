namespace Cooperator.Framework.Utility.DBReverseHelper.PropertyEditors
{
    partial class StringEditorForm
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
            this.Cancel_button = new System.Windows.Forms.Button();
            this.Ok_button = new System.Windows.Forms.Button();
            this.StringTextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // Cancel_button
            // 
            this.Cancel_button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel_button.Location = new System.Drawing.Point(345, 248);
            this.Cancel_button.Name = "Cancel_button";
            this.Cancel_button.Size = new System.Drawing.Size(75, 23);
            this.Cancel_button.TabIndex = 0;
            this.Cancel_button.Text = "Cancel";
            this.Cancel_button.UseVisualStyleBackColor = true;
            // 
            // Ok_button
            // 
            this.Ok_button.Location = new System.Drawing.Point(264, 248);
            this.Ok_button.Name = "Ok_button";
            this.Ok_button.Size = new System.Drawing.Size(75, 23);
            this.Ok_button.TabIndex = 1;
            this.Ok_button.Text = "Ok";
            this.Ok_button.UseVisualStyleBackColor = true;
            this.Ok_button.Click += new System.EventHandler(this.Ok_button_Click);
            // 
            // StringTextBox
            // 
            this.StringTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.StringTextBox.DetectUrls = false;
            this.StringTextBox.HideSelection = false;
            this.StringTextBox.Location = new System.Drawing.Point(13, 13);
            this.StringTextBox.Name = "StringTextBox";
            this.StringTextBox.Size = new System.Drawing.Size(407, 229);
            this.StringTextBox.TabIndex = 2;
            this.StringTextBox.Text = "";
            this.StringTextBox.WordWrap = false;
            // 
            // StringEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel_button;
            this.ClientSize = new System.Drawing.Size(432, 283);
            this.Controls.Add(this.StringTextBox);
            this.Controls.Add(this.Ok_button);
            this.Controls.Add(this.Cancel_button);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StringEditorForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Property Editor";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Cancel_button;
        private System.Windows.Forms.Button Ok_button;

        /// <summary></summary>
        public System.Windows.Forms.RichTextBox StringTextBox;
    }
}