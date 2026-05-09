/*-
*       Copyright (c) 2006-2007 Eugenio Serrano, Daniel Calvin.
*       All rights reserved.
*
*       Redistribution and use in source and binary forms, with or without
*       modification, are permitted provided that the following conditions
*       are met:
*       1. Redistributions of source code must retain the above copyright
*          notice, this list of conditions and the following disclaimer.
*       2. Redistributions in binary form must reproduce the above copyright
*          notice, this list of conditions and the following disclaimer in the
*          documentation and/or other materials provided with the distribution.
*       3. Neither the name of copyright holders nor the names of its
*          contributors may be used to endorse or promote products derived
*          from this software without specific prior written permission.
*
*       THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
*       "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
*       TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
*       PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL COPYRIGHT HOLDERS OR CONTRIBUTORS
*       BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
*       CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
*       SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
*       INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
*       CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
*       ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
*       POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace CooperatorModeler
{
    public static class UI
    {
        public static MainForm mainForm;

        public static string StatusLabel
        {
            get { return mainForm.MainStatus_toolStripStatusLabel.Text; }
            set { mainForm.MainStatus_toolStripStatusLabel.Text = value; }
        }

        public static string ModelFileNameLabel        
        {
            get { return mainForm.SnapshotName_StatusLabel.Text; }
            set
            {
                int pos = value.LastIndexOf('\\');
                string newvalue ;
                if (pos != -1)
                    newvalue = value.Substring(pos + 1);
                else
                    newvalue = value;
                mainForm.SnapshotName_StatusLabel.Text = string.Format("Model: {0}", newvalue);
            }
        }

        public static string DBNameLabel
        {
            get { return mainForm.DatabaseName_StatusLabel.Text; }
            set { mainForm.DatabaseName_StatusLabel.Text = string.Format("DB: {0}", value); }
        }

        public static System.Windows.Forms.ToolStripProgressBar ProgessBar
        {
            get { return mainForm.Progress_toolStripProgressBar; }
        }
    }
}
