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
    class ToStringInfoEditor : System.Drawing.Design.UITypeEditor
    {
        ToStringInfoEditorForm myForm = new ToStringInfoEditorForm();
        IWindowsFormsEditorService myEditorService;

        public ToStringInfoEditor() { }
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        // Displays the UI for value selection.
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            ToStringStruct tss = value as ToStringStruct;
            tss = tss == null ? new ToStringStruct() : tss;
            myEditorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (myEditorService != null)
            {
                myForm.Text = context.PropertyDescriptor.Name + " Editor";
                myForm.StringFormat = tss.StringFotmat;
                myForm.SetProperties( tss.Properties,tss.ToStringParams);
                myForm.OverrideToString = tss.OverrideToString;
                if (myEditorService.ShowDialog(myForm) == DialogResult.OK)
                {
                    tss = new ToStringStruct();
                    tss.StringFotmat = myForm.StringFormat;
                    tss.ToStringParams = myForm.ToStringParams;
                    tss.Properties = myForm.Properties;
                    tss.OverrideToString=myForm.OverrideToString;
                    return tss;
                }
            }
            return tss;
        }

        private void myDropDownList_Click(object sender, EventArgs e)
        {
            myEditorService.CloseDropDown();
        }


    }
}
