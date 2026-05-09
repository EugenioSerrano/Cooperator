using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Cooperator.Framework.Utility.DBReverseHelper.PropertyEditors
{
    /// <summary>
    /// 
    /// </summary>
    public partial class StringEditorForm : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public StringEditorForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ok_button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
