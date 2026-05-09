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

namespace Cooperator.Framework.Library.Repository
{

	/// <summary>
	/// 
	/// </summary>
	public enum RegistryTarget
	{
		/// <summary>
		/// 
		/// </summary>
		CurrentUser,

		/// <summary>
		/// 
		/// </summary>
		LocalMachine
	}

	/// <summary>
	/// 
	/// </summary>
	internal class RepositoryEntity
	{
		private RegistryTarget _Target;
		private string _OwnerName;
		private string _ParameterName;
		private string _TextValue;
		private decimal? _NumberValue;
		private bool? _BooleanValue;
		private DateTime? _DateValue;

		const string separatorTemplate = "[!?|]";
		const string nullValueTemplate = "[+*|null|*+]";

		public RepositoryEntity()
		{
			//_TextValue = null; Microsoft.Performance CA1805
			_NumberValue = null;
			_BooleanValue = null;
			_DateValue = null;
		}

		public RepositoryEntity(string str, string ownerName, string parameterName)
		{
			_OwnerName = ownerName;
			_ParameterName = parameterName;
			_TextValue = null;
			_NumberValue = null;
			_BooleanValue = null;
			_DateValue = null;

			if (String.IsNullOrEmpty(str)) return;

			int end, start = 0;
			for (int i = 0; i <= 4; i++)
			{
				end = str.IndexOf(separatorTemplate, start);
				string value = str.Substring(start, (end-start));
				switch (i)
				{
					case 0:
						switch (value)
						{
							case "CurrentUser":
								_Target = RegistryTarget.CurrentUser;
								break;
							case "LocalMachine":
								_Target = RegistryTarget.LocalMachine;
								break;
						}
						break;
					case 1:
						_TextValue = value;
						if ( ! String.IsNullOrEmpty(_TextValue )) return;
						break;
					case 2:
						if (value != nullValueTemplate)
						{
							_NumberValue = Convert.ToInt32(value) / Convert.ToDecimal(100000);
							return;
						}
						break;
					case 3:
						if (value != nullValueTemplate)
						{
							if (value == "0")
							{
								_BooleanValue = false;
							}
							else
							{
								_BooleanValue = true;
							}
							return;
						}
						break;
					case 4:
						if (value != nullValueTemplate)
						{
							int year, month, day;
							year = Convert.ToInt16(value.Substring(0, 4));
							month = Convert.ToInt16(value.Substring(4, 2));
							day = Convert.ToInt16(value.Substring(6, 2));
							_DateValue = new DateTime(year, month, day);
							return;
						}
						break;
				}
				start = end+5;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			string Snumbervalue = nullValueTemplate;
			string Sboolvalue = nullValueTemplate;
			string Sdatevalue = nullValueTemplate;

			if (_NumberValue.HasValue)
				Snumbervalue = Convert.ToInt32(_NumberValue.Value * 100000).ToString();

			if (_BooleanValue.HasValue)
			{
				if (_BooleanValue.Value)
				{
					Sboolvalue = "1";
				}
				else
				{
					Sboolvalue = "0";
				}

			}
			if (_DateValue.HasValue)
			{
				Sdatevalue = _DateValue.Value.Year.ToString("0000")+ _DateValue.Value.Month.ToString("00")+_DateValue.Value.Day.ToString("00");
			}

			sb.Append(_Target.ToString());
			sb.Append(separatorTemplate);
			sb.Append(_TextValue);
			sb.Append(separatorTemplate);
			sb.Append(Snumbervalue);
			sb.Append(separatorTemplate);
			sb.Append(Sboolvalue);
			sb.Append(separatorTemplate);
			sb.Append(Sdatevalue);
			sb.Append(separatorTemplate);

			return sb.ToString();

		}


		public RegistryTarget Target
		{
			get { return _Target; }
			set { _Target = value; }
		}

		public string OwnerName
		{
			get { return _OwnerName; }
			set { _OwnerName = value; }
		}

		public string ParameterName
		{
			get { return _ParameterName; }
			set { _ParameterName = value; }
		}

		public string TextValue
		{
			get { return _TextValue; }
			set { _TextValue = value; }
		}

		public decimal? NumberValue
		{
			get { return _NumberValue; }
			set { _NumberValue = value; }
		}

		public bool? BooleanValue
		{
			get { return _BooleanValue; }
			set { _BooleanValue = value; }
		}


		public DateTime? DateValue
		{
			get { return _DateValue; }
			set { _DateValue = value; }
		}

	}
}