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
using System.Configuration;
using System.Net.Mail;
using System.IO;

namespace Cooperator.Framework.Library
{

	/// <summary>
	/// 
	/// </summary>
	public static class SmtpMail
	{
		private static string _SmtpServerName = ConfigurationManager.AppSettings["SMTPServerName"].ToString();
		private static SmtpClient _SmtpServer;// = null; Microsoft.Performance CA1805

        /// <summary>
        ///  Envia un mail usando CDSYS. Recuerde configurar la entrada SMTPServerName en su archivo de configuracion.
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
            try
            {
                SendMessage(oMessage);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
		}

		private static MailMessage GetNewMessage(string from, string to)
		{
			MailMessage oM = new MailMessage(from, to);
			return oM;
		}

		private static void SendMessage(MailMessage message)
		{
			try
			{
				if (_SmtpServer == null) _SmtpServer = new SmtpClient(_SmtpServerName);
				_SmtpServer.Send(message);
			}
			catch (Exception ex)
			{
				throw new Exceptions.SmtpErrorException(ex.Message);
			}
		}






        /// <summary>
        ///  Envia un mail usando CDONTS. No requiere configurar la entrada SMTPServerName en su archivo de configuracion.
        /// </summary>
        /// <param name="from">Direccion de correo de quien envia</param>
        /// <param name="to">Direccion de correo del destinatario</param>
        /// <param name="subject">Asunto del mensajes</param>
        /// <param name="body">Cuerpo del mensaje</param>
        /// <param name="currentHttpContext">El objeto HttpContext actual</param>
        /// <param name="attachments">Lista de archivos adjuntos.</param>        
        public static void SendMailViaCDONTS(string from, string to, string subject, string body, System.Web.HttpContext currentHttpContext, params FileInfo[] attachments)
        {

            try
            {

                throw new System.NotImplementedException("Esta funcion no esta implementada en esa version");

                //TODO: C# no soporta late binding asi nomas. Tengo que usar reflexion para usarlo..

                //object ObjMail;

                //ObjMail = currentHttpContext.Server.CreateObject("CDONTS.NewMail");
                //ObjMail.From = from;
                //ObjMail.To = to;

                //ObjMail.BodyFormat = 0;
                //ObjMail.BodyFormat = 1;

                //ObjMail.Subject = subject;

                //ObjMail.Body = body;

                //foreach (System.IO.FileInfo Att in attachments) 
                //    ObjMail.AttachFile(Att.FullName);

                //ObjMail.Send();

                //ObjMail = null;
            }
            catch (Exception ex)
            {
                throw new Exceptions.SmtpErrorException(ex.Message.ToString() + ex.ToString());
            }

        }

	}
}
