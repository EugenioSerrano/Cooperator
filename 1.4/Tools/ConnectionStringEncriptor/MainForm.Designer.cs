namespace ConnectionStringEncriptor
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
            this.ConnectionStringTextBox = new System.Windows.Forms.TextBox();
            this.EncryptedConnectionStringTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.EncryptButton = new System.Windows.Forms.Button();
            this.CopyClipboardButton = new System.Windows.Forms.Button();
            this.ExitButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ConnectionStringTextBox
            // 
            this.ConnectionStringTextBox.Location = new System.Drawing.Point(12, 24);
            this.ConnectionStringTextBox.Multiline = true;
            this.ConnectionStringTextBox.Name = "ConnectionStringTextBox";
            this.ConnectionStringTextBox.Size = new System.Drawing.Size(558, 97);
            this.ConnectionStringTextBox.TabIndex = 0;
            this.ConnectionStringTextBox.Text = "Paste your connection string here.";
            this.ConnectionStringTextBox.Enter += new System.EventHandler(this.ConnectionStringTextBox_Enter);
            // 
            // EncryptedConnectionStringTextBox
            // 
            this.EncryptedConnectionStringTextBox.Location = new System.Drawing.Point(12, 156);
            this.EncryptedConnectionStringTextBox.Multiline = true;
            this.EncryptedConnectionStringTextBox.Name = "EncryptedConnectionStringTextBox";
            this.EncryptedConnectionStringTextBox.ReadOnly = true;
            this.EncryptedConnectionStringTextBox.Size = new System.Drawing.Size(558, 115);
            this.EncryptedConnectionStringTextBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "ConnectionString:";
            // 
            // EncryptButton
            // 
            this.EncryptButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.EncryptButton.Location = new System.Drawing.Point(12, 127);
            this.EncryptButton.Name = "EncryptButton";
            this.EncryptButton.Size = new System.Drawing.Size(75, 23);
            this.EncryptButton.TabIndex = 3;
            this.EncryptButton.Text = "Encrypt";
            this.EncryptButton.UseVisualStyleBackColor = true;
            this.EncryptButton.Click += new System.EventHandler(this.EncryptButton_Click);
            // 
            // CopyClipboardButton
            // 
            this.CopyClipboardButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CopyClipboardButton.Location = new System.Drawing.Point(93, 127);
            this.CopyClipboardButton.Name = "CopyClipboardButton";
            this.CopyClipboardButton.Size = new System.Drawing.Size(116, 23);
            this.CopyClipboardButton.TabIndex = 4;
            this.CopyClipboardButton.Text = "Copy to clipboard";
            this.CopyClipboardButton.UseVisualStyleBackColor = true;
            this.CopyClipboardButton.Click += new System.EventHandler(this.CopyClipboardButton_Click);
            // 
            // ExitButton
            // 
            this.ExitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ExitButton.Location = new System.Drawing.Point(495, 277);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(75, 23);
            this.ExitButton.TabIndex = 5;
            this.ExitButton.Text = "Exit";
            this.ExitButton.UseVisualStyleBackColor = true;
            this.ExitButton.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ExitButton;
            this.ClientSize = new System.Drawing.Size(582, 308);
            this.Controls.Add(this.ExitButton);
            this.Controls.Add(this.CopyClipboardButton);
            this.Controls.Add(this.EncryptButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.EncryptedConnectionStringTextBox);
            this.Controls.Add(this.ConnectionStringTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "Connection String Encriptor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ConnectionStringTextBox;
        private System.Windows.Forms.TextBox EncryptedConnectionStringTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button EncryptButton;
        private System.Windows.Forms.Button CopyClipboardButton;
        private System.Windows.Forms.Button ExitButton;
    }
}

