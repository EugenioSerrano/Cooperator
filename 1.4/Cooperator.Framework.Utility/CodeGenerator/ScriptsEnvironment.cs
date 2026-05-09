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
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Cooperator.Framework.Utility.CodeGeneratorHelper
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ProcessEvent(object sender, System.EventArgs e);

    /// <summary>
    /// Descripción breve de ScriptingEnvironment.
    /// </summary>
    public class ScriptEnvironment
    {
        /// <summary>
        /// 
        /// </summary>
        public ScriptEnvironment()
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Response
    {
        #region privates

        private string buffer = "";
        private string ouputName = "";

        private Dictionary<string, string> ouputBuffers = new Dictionary<string, string>();

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string,string> OuputBuffers
        {
            get { return ouputBuffers; }
        }

        #endregion privates
        
        
        /// <summary>
        /// 
        /// </summary>
        protected internal Response() { }

        /// <summary>
        /// 
        /// </summary>
        public string OutputName
        {
            get { return ouputName; }
            set { ouputName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public event ProcessEvent Info;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg"></param>
        public void Write(string format, params object[] arg)
        {
            if (arg.Length == 0)
            {
                buffer += format;
            }
            else {
                buffer += (string.Format(format, arg));
            }
            EventFire();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg"></param>
        public void WriteLine(string format, params object[] arg)
        {
            Write(format + "\r\n", arg);            
        }
        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            ouputName = "";
            buffer = "";
            EventFire();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetBuffer()
        {
            return buffer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ouputName"></param>
        /// <returns></returns>
        public string GetBuffer(string ouputName)
        {            
            return ouputBuffers[ouputName];
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputName"></param>
        public void SaveBuffer(string outputName)
        {
            OutputName = outputName;
            SaveBuffer();
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void SaveBuffer()
        {
            if(ouputName=="")ouputName=string.Format("Ouput{0}",ouputBuffers.Count.ToString("000"));
            if (ouputBuffers.ContainsKey(ouputName)) throw new ApplicationException(string.Format("An item with the same key ({0}) has already been added.", ouputName));
            ouputBuffers.Add(ouputName, buffer);
            EventFire();
            Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        private void EventFire()
        {
            if (Info != null)
            {
                EventArgs e = new EventArgs();
                Info(null, e);
            }
        }

    }
}

