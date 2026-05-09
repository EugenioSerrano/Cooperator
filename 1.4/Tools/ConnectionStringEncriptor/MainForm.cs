using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ConnectionStringEncriptor
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }


        private void ConnectionStringTextBox_Enter(object sender, EventArgs e)
        {
            ConnectionStringTextBox.SelectAll();
        }

        private void CopyClipboardButton_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Clipboard.SetText(EncryptedConnectionStringTextBox.Text);
        }

        private void EncryptButton_Click(object sender, EventArgs e)
        {
            EncryptedConnectionStringTextBox.Text = Cooperator.Framework.Library.Cryptography.Encrypt(ConnectionStringTextBox.Text);
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}