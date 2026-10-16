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
using System.IO;
using System.Net.Mail;
using Cooperator.Framework.Core.Configuration;

namespace Cooperator.Framework.Library
{

	/// <summary>
	/// Envío de correo del framework.
	/// <para>
	/// ADR-004 §3.2 (F-22): el servidor sale del <b>catálogo de configuración</b>
	/// (<c>Cooperator:Mail:SmtpServer</c>) con lectura <b>perezosa y validada</b> —no de un inicializador estático
	/// que explota con <see cref="NullReferenceException"/> al cargar el tipo— y su ausencia produce el error
	/// tipado del framework.
	/// </para>
	/// <para>
	/// El método muerto <c>SendMailViaCDONTS</c> no se conserva: ADR-004 §3.2 y ADR-001 §3.1 ya lo declaraban
	/// saliente (su firma pública arrastraba <c>System.Web.HttpContext</c>).
	/// </para>
	/// </summary>
	public static class SmtpMail
	{
		private static SmtpClient? _SmtpServer;// = null; Microsoft.Performance CA1805

        /// <summary>
        ///  Envia un mail usando CDSYS. El servidor se toma de la configuración
        ///  (<c>Cooperator:Mail:SmtpServer</c>) en el momento de usarlo.
        /// </summary>
        /// <param name="from">Direccion de correo de quien envia</param>
        /// <param name="to">Direccion de correo del destinatario</param>
        /// <param name="subject">Asunto del mensajes</param>
        /// <param name="body">Cuerpo del mensaje</param>
        /// <param name="attachments">Lista de archivos adjuntos.</param>
        public static void SendMailViaCDSYS(string from, string to, string subject, string body, params FileInfo[] attachments)
		{
			MailMessage oMessage = GetNewMessage(from, to);
			oMessage.Subject = subject;
			oMessage.Body = body;
			foreach (System.IO.FileInfo Att in attachments)
				oMessage.Attachments.Add(new System.Net.Mail.Attachment(Att.FullName));
            SendMessage(oMessage);
		}

		private static MailMessage GetNewMessage(string from, string to)
		{
			MailMessage oM = new MailMessage(from, to);
			return oM;
		}

		private static void SendMessage(MailMessage message)
		{
			// Lectura perezosa y validada: si la clave falta, sale el error tipado del framework tal cual,
			// no envuelto en el error de envío.
			string smtpServerName = SmtpServerName;

			try
			{
				if (_SmtpServer == null) _SmtpServer = new SmtpClient(smtpServerName);
				_SmtpServer.Send(message);
			}
			catch (Exception ex)
			{
				throw new Exceptions.SmtpErrorException(ex.Message);
			}
		}

		/// <summary>
		/// Servidor SMTP del catálogo de configuración, leído en el punto de uso.
		/// </summary>
		private static string SmtpServerName
		{
			get { return CooperatorSettings.Current.GetString(ConfigurationKeys.Mail.SmtpServer); }
		}

	}
}
