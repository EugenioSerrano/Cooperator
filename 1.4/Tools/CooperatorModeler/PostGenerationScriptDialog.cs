using System;
using System.Windows.Forms;

namespace CooperatorModeler
{
    public partial class PostGenerationScriptDialog : Form
    {
        public PostGenerationScriptDialog()
        {
            InitializeComponent();
        }


        public Snapshot CurrentSnapshot;

        private void OkButtonClick(object sender, EventArgs e)
        {
            CurrentSnapshot.PostGenerationScript = this.ScriptTextBox.Text;
            Close();
        }

        private void PostGenerationScriptDialogLoad(object sender, EventArgs e)
        {
            ScriptTextBox.Text = CurrentSnapshot.PostGenerationScript;
        }



    }
}
