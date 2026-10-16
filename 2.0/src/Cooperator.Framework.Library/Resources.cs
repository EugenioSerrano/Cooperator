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

namespace Cooperator.Framework.Library
{
	/// <summary>
	/// 
	/// </summary>
	public static class Resources
	{

		/// <summary>
		/// 
		/// </summary>
		/// <param name="objectName"></param>
		/// <param name="sourceType"></param>
		/// <returns></returns>
		static public System.IO.Stream GetResourceStream(string objectName, Type sourceType)
		{
			System.Reflection.Assembly _assembly = System.Reflection.Assembly.GetAssembly(sourceType);
			string[] names = _assembly.GetManifestResourceNames();
			string resName = String.Format("{0}.{1}", System.Reflection.Assembly.GetAssembly(sourceType).GetName().Name.ToString(), objectName);
			System.IO.Stream retValue = _assembly.GetManifestResourceStream(resName);
			return retValue;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="objectName"></param>
		/// <returns></returns>
		static public System.IO.Stream GetResourceStream(string objectName)
		{
			System.Reflection.Assembly _assembly = System.Reflection.Assembly.GetCallingAssembly();
			System.IO.Stream retValue = _assembly.GetManifestResourceStream(String.Format("{0}.{1}", System.Reflection.Assembly.GetCallingAssembly().GetName().Name.ToString(), objectName));
			return retValue;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="sourceType"></param>
		/// <returns></returns>
		static public string GetResourceString(string name, Type sourceType)
		{
			System.IO.Stream _stringStream = GetResourceStream(name, sourceType);
			System.IO.StreamReader Tr = new System.IO.StreamReader(_stringStream, System.Text.Encoding.Default);
			string s = Tr.ReadToEnd();
			Tr.Close();
			_stringStream.Close();
			return s;
		}

	}
}
