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
using Cooperator.Framework.Library.Exceptions;
using Cooperator.Framework.Library.IO;

namespace Cooperator.Framework.Library.Repository
{
	/// <summary>
	/// 
	/// </summary>
    public static class RegistryRepository
    {

		private enum RepositoryValueType
		{
			Date,
			Number,
			Boolean,
			Text
		}

		private static Dictionary<string, RepositoryEntity> _RepositoryValues = new Dictionary<string, RepositoryEntity>();

		private static void SaveItem(RegistryTarget target, string ownerName, string parameterName, object value, RepositoryValueType type)
		{
			RepositoryEntity Obj;
			bool exist = false;
			try
			{
				Obj = RepositoryMapper.GetInstance.GetByOwnerAndParam(target, ownerName, parameterName);
				exist = true;
				//Update
			}
			catch (RegistryIOErrorException)
			{
				//New
				Obj = new RepositoryEntity();
				Obj.Target = target;
				Obj.OwnerName = ownerName;
				Obj.ParameterName = parameterName;
			}
			switch (type)
			{
				case RepositoryValueType.Date:
					if (exist && !Obj.DateValue.HasValue) throw new ItemExistButDiferentTypeException();
					Obj.DateValue = Convert.ToDateTime(value);
					break;
				case RepositoryValueType.Number:
					if (exist && !Obj.NumberValue.HasValue) throw new ItemExistButDiferentTypeException();
					Obj.NumberValue = Convert.ToDecimal(value);
					break;
				case RepositoryValueType.Boolean:
					if (exist && !Obj.BooleanValue.HasValue) throw new ItemExistButDiferentTypeException();
					Obj.BooleanValue = Convert.ToBoolean(value);
					break;
				case RepositoryValueType.Text:
					if (exist && String.IsNullOrEmpty(Obj.TextValue)) throw new ItemExistButDiferentTypeException();
					Obj.TextValue = Convert.ToString(value);
					break;
			}

			RepositoryMapper.GetInstance.Update(target, Obj);

			string key = GenKey(ownerName, parameterName);
			if (_RepositoryValues.ContainsKey(key))
			{
				//Si esta la elimino, para que se carge de nuevo en la hash table.
				_RepositoryValues.Remove(key);
			}
		}

		private static string GenKey(string ownerName, string parameterName)
		{
			return ownerName.Trim().ToLower() + "¡" + parameterName.Trim().ToLower();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="target"></param>
		/// <param name="ownerName"></param>
		/// <param name="parameterName"></param>
		/// <param name="number"></param>
		public static void SaveItem(RegistryTarget target, string ownerName, string parameterName, decimal number)
		{
			SaveItem(target, ownerName, parameterName, number, RepositoryValueType.Number);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="target"></param>
		/// <param name="ownerName"></param>
		/// <param name="parameterName"></param>
		/// <param name="date"></param>
		public static void SaveItem(RegistryTarget target, string ownerName, string parameterName, DateTime date)
		{
			SaveItem(target, ownerName, parameterName, date, RepositoryValueType.Date);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="target"></param>
		/// <param name="ownerName"></param>
		/// <param name="parameterName"></param>
		/// <param name="str"></param>
		public static void SaveItem(RegistryTarget target, string ownerName, string parameterName, string str)
		{
			if (String.IsNullOrEmpty(str))
			{
				throw new NullValueNotAllowedException();
			}
			SaveItem(target, ownerName, parameterName, str, RepositoryValueType.Text);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="target"></param>
		/// <param name="ownerName"></param>
		/// <param name="parameterName"></param>
		/// <param name="boolean"></param>
		public static void SaveItem(RegistryTarget target, string ownerName, string parameterName, bool boolean)
		{
			SaveItem(target, ownerName, parameterName, boolean, RepositoryValueType.Boolean);
		}


		private static RepositoryEntity GetItem(RegistryTarget target, string ownerName, string parameterName, RepositoryValueType type)
		{
			RepositoryEntity Obj;

			// Primero lo buscamos en la hash
			string key = GenKey(ownerName, parameterName);
			if (_RepositoryValues.ContainsKey(key))
			{
				Obj = _RepositoryValues[key];
			}
			else
			{
				//No existe el la hash, lo buscamos en la tabla
				// Si no existe da error de RegistryIOErrorException
				Obj = RepositoryMapper.GetInstance.GetByOwnerAndParam(target, ownerName, parameterName);

				// lo agregamos a la hash
				_RepositoryValues.Add(key, Obj);
			}

			switch (type)
			{
				case RepositoryValueType.Date:
					if (!Obj.DateValue.HasValue) throw new ItemExistButDiferentTypeException();
					break;
				case RepositoryValueType.Number:
					if (!Obj.NumberValue.HasValue) throw new ItemExistButDiferentTypeException();
					break;
				case RepositoryValueType.Boolean:
					if (!Obj.BooleanValue.HasValue) throw new ItemExistButDiferentTypeException();
					break;
				case RepositoryValueType.Text:
					if (String.IsNullOrEmpty(Obj.TextValue)) throw new ItemExistButDiferentTypeException();
					break;
			}

			return Obj;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="target"></param>
		/// <param name="ownerName"></param>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		public static DateTime GetItemAsDateTime(RegistryTarget target, string ownerName, string parameterName)
		{
			return GetItem(target, ownerName, parameterName, RepositoryValueType.Date).DateValue.Value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="target"></param>
		/// <param name="ownerName"></param>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		public static decimal GetItemAsDecimal(RegistryTarget target, string ownerName, string parameterName)
		{
			return GetItem(target, ownerName, parameterName, RepositoryValueType.Number).NumberValue.Value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="target"></param>
		/// <param name="ownerName"></param>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		public static string GetItemAsString(RegistryTarget target, string ownerName, string parameterName)
		{
			return GetItem(target, ownerName, parameterName, RepositoryValueType.Text).TextValue;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="target"></param>
		/// <param name="ownerName"></param>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		public static bool GetItemAsBoolean(RegistryTarget target, string ownerName, string parameterName)
		{
			return GetItem(target, ownerName, parameterName, RepositoryValueType.Boolean).BooleanValue.Value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="target"></param>
		/// <param name="ownerName"></param>
		/// <param name="parameterName"></param>
		public static void DeleteItem(RegistryTarget target, string ownerName, string parameterName)
		{
			RepositoryEntity Obj;
			// Si no existe da error de RegistryIOError
			Obj = RepositoryMapper.GetInstance.GetByOwnerAndParam(target, ownerName, parameterName);

			RepositoryMapper.GetInstance.Delete(target, Obj);

			string key = GenKey(ownerName, parameterName);
			if (_RepositoryValues.ContainsKey(key))
			{
				_RepositoryValues.Remove(key);
			}
		}

    }
}
