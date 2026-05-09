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
    public partial class ToStringInfoEditorForm : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public bool OverrideToString
        { 
            get { return this.OverrideToString_checkBox.Checked; }
            set { 
                this.OverrideToString_checkBox.Checked= value;
                OverrideToString_checkBox_CheckedChanged(null, null);

            }
        }
        /// <summary>
        /// 
        /// </summary>
        public String StringFormat
        {
            get { return this.StringFormat_textbox.Text; }
            set { 
                this.StringFormat_textbox.Text = value;
                
            }
        }

        private List<String> toStringParams;
        /// <summary>
        /// 
        /// </summary>
        public List<String> ToStringParams
        {
            get { return toStringParams; }
            
        }
	
        private List<String> properties;
        /// <summary>
        /// 
        /// </summary>
        public List<String> Properties
        {
            get { return properties; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="toStringParams"></param>
        public void SetProperties(List<String> properties, List<String> toStringParams)
        {
            List<String> ppties=new List<string>(properties);
            List<String> tsprms=new List<string>(toStringParams);

            this.properties = properties;
            this.toStringParams = toStringParams;

            this.Properties_listbox.Items.Clear();
            this.ToStringParams_listbox.Items.Clear();

            
            foreach(String s in toStringParams)
            {
                if(!ppties.Remove(s))
                    tsprms.Remove(s);
            }                
            this.Properties_listbox.Items.AddRange(ppties.ToArray());
            this.ToStringParams_listbox.Items.AddRange(tsprms.ToArray());
        }
        /// <summary>
        /// 
        /// </summary>
        public ToStringInfoEditorForm()
        {
            InitializeComponent();
        }

        private void ToStringInfoEditorForm_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ok_button_Click(object sender, EventArgs e)
        {

            
            if (this.OverrideToString_checkBox.Checked && ToStringInfoEditorForm_Validating())
            {
                MessageBox.Show("The number of parameters declared, not match with the number of items in String.Format .", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {

                this.toStringParams.Clear();
                foreach (string s in this.ToStringParams_listbox.Items)
                    this.toStringParams.Add(s);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void Add_button_Click(object sender, EventArgs e)
        {
            if (this.Properties_listbox.SelectedItem != null)
            {
                this.ToStringParams_listbox.Items.Add(this.Properties_listbox.SelectedItem);
                this.Properties_listbox.Items.Remove(this.Properties_listbox.SelectedItem);
            }
        }

        private void Remove_button_Click(object sender, EventArgs e)
        {
            if (this.ToStringParams_listbox.SelectedItem != null)
            {
                this.Properties_listbox.Items.Add(this.ToStringParams_listbox.SelectedItem);
                this.ToStringParams_listbox.Items.Remove(this.ToStringParams_listbox.SelectedItem);
            }

        }

        private void StrinFormat_textbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void OverrideToString_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!OverrideToString_checkBox.Checked)
            {
                this.Properties_listbox.Enabled = false;
                this.ToStringParams_listbox.Enabled = false;
                this.StringFormat_textbox.Enabled = false;
            }
            else
            {
                this.Properties_listbox.Enabled = true;
                this.ToStringParams_listbox.Enabled = true;
                this.StringFormat_textbox.Enabled = true;
            }

        }


        private bool ToStringInfoEditorForm_Validating()
        {
            bool cancel = false;
            for (int strParams = 0; strParams < this.ToStringParams_listbox.Items.Count; strParams++)
            {
                if (this.StringFormat_textbox.Text.IndexOf("{"+string.Format("{0}", strParams)+"}")==-1)
                {
                    cancel = true;
                    break;
                }
            }
            int i = 0;
            int j = 0;
            while((i=this.StringFormat_textbox.Text.IndexOf("{",i))!=-1)
            {
                
                j++;
                i++;
            }
            if(this.ToStringParams_listbox.Items.Count!=j)
                cancel = true;

            return cancel;
        }

    }
}
