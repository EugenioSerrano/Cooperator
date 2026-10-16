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

namespace Cooperator.Framework.Core.Factory
{
    /// <summary>
    /// Construcción de objetos de tipos <b>conocidos</b>: la vía tipada que no carga por
    /// nombre.
    /// <para>
    /// ADR-006 §3.1 elimina la carga por <b>nombre o ruta</b> —la superficie que convertía
    /// un valor de configuración en ejecución de código— y el sondeo implícito de
    /// directorios; <b>no</b> elimina la construcción genérica de tipos ya referenciados,
    /// y retirarla habría sido una ruptura que ningún ADR ordena. La carga de providers por
    /// nombre vive ahora en <c>Cooperator.Framework.Core.LazyLoad</c>.
    /// </para>
    /// </summary>
    public static class SingleFactory
    {
        /// <summary>
        /// Construye una instancia del tipo indicado por su constructor sin parámetros.
        /// </summary>
        /// <param name="type">Tipo ya conocido (referenciado por la aplicación).</param>
        /// <returns>La instancia; nunca <c>null</c>.</returns>
        public static object CreateObject(Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.Assembly.CreateInstance(type.FullName ?? type.Name)
                ?? throw new InvalidOperationException(
                    $"No se pudo construir una instancia de '{type.FullName}': el tipo necesita un constructor público sin parámetros.");
        }

        /// <summary>
        /// Construye una instancia de <typeparamref name="T"/> por su constructor sin
        /// parámetros.
        /// </summary>
        /// <typeparam name="T">Tipo ya conocido (referenciado por la aplicación).</typeparam>
        /// <returns>La instancia; nunca <c>null</c>.</returns>
        public static T CreateObject<T>() => (T)CreateObject(typeof(T));
    }
}
