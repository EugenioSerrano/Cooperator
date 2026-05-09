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

namespace Cooperator.Framework.Library.IO
{
    /// <summary>
    /// 
    /// </summary>
    public static class File
    {

        /// <summary>
        /// Graba un string en un archivo en la carpeta de la aplicacación.
        /// </summary>
        /// <param name="fileName">Nombre del archivo sin path. Ej: MyFile.txt</param>
        /// <param name="text">Texto a guardar</param>
        /// <param name="replace">Si se reemplaza o no el archivo. Si no se reemplaza y el archivo existe se dispara un error.</param>
        public static void SaveStringToFileInAppFolder(string fileName, string text, bool replace)
        {
            string newFileName = Cooperator.Framework.Library.IO.Path.AppPath(true) + fileName;
            SaveStringToFile(newFileName, text, replace);
        }

        /// <summary>
        /// Graba un string en un archivo.
        /// </summary>
        /// <param name="fullFileName">Nombre del archivo con path. Ej: C:\MyFiles\MyFile.txt</param>
        /// <param name="text">Texto a guardar</param>
        /// <param name="replace">Si se reemplaza o no el archivo. Si no se reemplaza y el archivo existe se dispara un error.</param>
        public static void SaveStringToFile(string fullFileName, string text, bool replace)
        {
            if (System.IO.File.Exists(fullFileName))
            {
                if (replace)
                {
                    System.IO.File.Delete(fullFileName);
                }
                else 
                {
                    throw new Exceptions.FileAlreadyExist(String.Format("El archivo {0} ya existe", fullFileName));
                }
            }

            // Ahora grabamos...
            using (System.IO.StreamWriter sw = System.IO.File.CreateText(fullFileName))
            {
                sw.Write(text);
                sw.Close();
            }
        }

    }
}
