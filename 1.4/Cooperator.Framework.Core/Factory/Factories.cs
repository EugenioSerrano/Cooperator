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
using System.Reflection;

namespace Cooperator.Framework.Core.Factory
{
    /// <summary>
    /// 
    /// </summary>
    public static class SingleFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static object CreateObject(string assemblyName, string className)
        {
            //string assemblyPath = AppDomain.CurrentDomain.BaseDirectory;
            //if (!assemblyPath.EndsWith("\\")) assemblyPath += "\\";
            //Assembly a = Assembly.LoadFrom(assemblyPath + assemblyName);
            var a = Assembly.Load(assemblyName);
            return a.CreateInstance(className, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <param name="assemblyName"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static object CreateObject(string assemblyPath,string assemblyName, string className)
        {
            if (!assemblyPath.EndsWith("\\"))
            {
                assemblyPath += "\\";
            }
            var a = Assembly.LoadFrom(assemblyPath + assemblyName);
            return a.CreateInstance(className, true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object CreateObject(Type type)
        {
            return type.Assembly.CreateInstance(type.FullName);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateObject<T>()
        {
            return (T)CreateObject(typeof(T));
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public static class FactoryCache
    {
        static readonly Dictionary<string, object> Objects = new Dictionary<string, object>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static object Get(string assemblyName, string className)
        {
            var ko = $"{assemblyName}.{className}";
            if (!Objects.ContainsKey(ko))
            {
                Objects.Add(ko, SingleFactory.CreateObject(assemblyName, className));
            }
            return Objects[ko];
        }
    }
}
