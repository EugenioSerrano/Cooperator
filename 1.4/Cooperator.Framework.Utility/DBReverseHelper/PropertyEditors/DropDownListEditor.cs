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
    public class DropDownListEditor : System.Drawing.Design.UITypeEditor
    {
        //this is a container for strings, which can be picked-out
        ListBox myDropDownList = new ListBox();
        IWindowsFormsEditorService myEditorService;
        
        /// <summary>
        /// this is a string array for drop-down list
        /// </summary>
        public static string[] stringList;

        /// <summary>
        /// 
        /// </summary>
        public DropDownListEditor()
        {
            myDropDownList.BorderStyle = BorderStyle.None;
            //add event handler for drop-down box when item will be selected
            myDropDownList.Click += new EventHandler(myDropDownList_Click);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
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
            myDropDownList.Items.Clear();
            myDropDownList.Items.AddRange(stringList);
            myDropDownList.Height = myDropDownList.PreferredHeight;
            // Uses the IWindowsFormsEditorService to display a 
            // drop-down UI in the Properties window.
            myEditorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (myEditorService != null)
            {
                myEditorService.DropDownControl(myDropDownList);
                return myDropDownList.SelectedItem;
            }
            return value;
        }

        private void myDropDownList_Click(object sender, EventArgs e)
        {
            myEditorService.CloseDropDown();
        }
    }

}
