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
using Cooperator.Framework.Library.IO;
using Microsoft.Win32;
using Cooperator.Framework.Library.Exceptions;

namespace Cooperator.Framework.Library.Repository
{



	/// <summary>
	/// Esta clase mapea el Objeto RepositoryEntity a la registry
	/// </summary>
	internal class RepositoryMapper
	{

		#region "Singleton"

		private RepositoryMapper()
		{
		}

		private static RepositoryMapper _instance = new RepositoryMapper();

		public static RepositoryMapper GetInstance
		{
			get
			{
				return _instance;
			}
		}

		#endregion


		public RepositoryEntity GetByOwnerAndParam(RegistryTarget target, string ownerName, string parameterName)
		{
			string key = GetRegistryHelper(target).Read(GenKey(ownerName, parameterName));

			if (String.IsNullOrEmpty(key))
			{
				throw new RegistryIOErrorException();
			}

			return new RepositoryEntity(key, ownerName, parameterName);
		}

		public void Update(RegistryTarget target, RepositoryEntity entity)
		{

			GetRegistryHelper(target).Write(GenKey(entity.OwnerName, entity.ParameterName), entity.ToString());
		}

		public void Delete(RegistryTarget target, RepositoryEntity entity)
		{
			GetRegistryHelper(target).DeleteKey(GenKey(entity.OwnerName, entity.ParameterName));
		}

		private static string GenKey(string ownerName, string parameterName)
		{
			return ownerName.Trim().ToLower() + "¡" + parameterName.Trim().ToLower();
		}

		private Library.IO.Registry GetRegistryHelper(RegistryTarget target) {
            Library.IO.Registry myReg = new Library.IO.Registry();
			switch (target)
			{
				case RegistryTarget.CurrentUser:
                    myReg.BaseRegistryKey = Microsoft.Win32.Registry.CurrentUser;
					break;
				case RegistryTarget.LocalMachine:
                    myReg.BaseRegistryKey = Microsoft.Win32.Registry.LocalMachine;
					break;
			}
			myReg.SubKey = "SOFTWARE\\Cooperator Framework\\Repository";
			return myReg;
		}

	}
}
