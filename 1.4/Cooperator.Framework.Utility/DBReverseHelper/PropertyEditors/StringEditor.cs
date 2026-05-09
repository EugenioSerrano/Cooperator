using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Drawing.Design;

namespace Cooperator.Framework.Utility.DBReverseHelper.PropertyEditors
{
    /// <summary>
    /// 
    /// </summary>
    public class StringEditor : System.Drawing.Design.UITypeEditor
    {
        //this is a container for strings, which can be edited in another form
        StringEditorForm myForm = new StringEditorForm();
        IWindowsFormsEditorService myEditorService;

        /// <summary>
        /// 
        /// </summary>
        public StringEditor()
        {
            //myForm.BorderStyle = BorderStyle.None;
            //add event handler for drop-down box when item will be selected
            //myForm.Click += new EventHandler(myDropDownList_Click);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        /// <summary>
        /// Displays the UI for value selection.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="provider"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            //myForm.Items.Clear();
            //myForm.Items.AddRange(stringList);
            //myForm.Height = myForm.PreferredHeight;
            // Uses the IWindowsFormsEditorService to display the
            // form  as modal of Properties window.
            myEditorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (myEditorService != null)
            {
                myForm.Text = context.PropertyDescriptor.Name + " Editor";
                myForm.StringTextBox.Text = Convert.ToString(value);
                if (myEditorService.ShowDialog(myForm) == DialogResult.OK)
                    return myForm.StringTextBox.Text;
            }
            return value;
        }

        private void myDropDownList_Click(object sender, EventArgs e)
        {
            myEditorService.CloseDropDown();
        }

    }
}
