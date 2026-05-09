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
using Microsoft.VisualBasic;
using Microsoft.CSharp;
using System.CodeDom;
using System.CodeDom.Compiler;


namespace Cooperator.Framework.Utility.CodeGeneratorHelper
{
    /// <summary>
    /// Clase base para la creación de 
    /// interpretes de distintos lenguajes
    /// </summary>
    public abstract class InterpreterFactory
    {
        /// <summary>
        /// Instancia el parser que se aplicará
        /// al script a ejecutar
        /// </summary>
        /// <returns></returns>
        public abstract ScriptParser createParser();
        /// <summary>
        /// Instancia el interprete que se corresponde
        /// con el lenguaje en que esta escrito el script
        /// </summary>
        /// <returns></returns>
        public abstract Interpreter createInterpreter();
    }

    /// <summary>
    /// Clase base para los parsers
    /// </summary>
    public abstract class ScriptParser
    {
        /// <summary>
        /// Realiza el parseado del script
        /// </summary>
        /// <param name="scriptText">Texto del script original</param>
        /// <returns>Texto procesado por el parser</returns>
        public abstract string Parse(string scriptText);
    }

    /// <summary>
    /// Clase base para cualquier Clase Interprete que se quiera 
    /// crear.
    /// </summary>
    public abstract class Interpreter
    {

        private CompilerResults results = null;
        /// <summary>
        /// Resultado de la compilaci{on del script
        /// </summary>
        public CompilerResults Results { get { return results; } }

        /// <summary>
        /// Ejecuta el script ya procesado por el parser correspondiente
        /// </summary>
        /// <param name="nativeCode">Código ya parseado</param>
        /// <param name="assemblies">Lista de assemblies necesarios para compilar el script</param>
        /// <returns>Verdadero si el resultado es exitoso</returns>
        public abstract bool Compile(string nativeCode, string[] assemblies);
        /// <summary>
        /// Compila un assembly en memoria que contiene el c{odigo del script.
        /// </summary>
        /// <param name="provider">CodeDomProvider según lenguaje seleccionado</param>
        /// <param name="sourceCode">Script parseado a ejecutar</param>
        /// <returns></returns>
        protected bool CompileExecutable(CodeDomProvider provider, String sourceCode)
        {
            return CompileExecutable(provider, null, sourceCode);
        }
        /// <summary>
        /// Compila un assembly en memoria que contiene el código del script.
        /// </summary>
        /// <param name="provider">CodeDomProvider según lenguaje seleccionado</param>
        /// <param name="assemblies">Lista de assemblies necesarios para compilar el script</param>
        /// <param name="sourceCode">Script parseado a ejecutar</param>
        /// <returns>Verdadero si el resultado es exitoso</returns>
        protected bool CompileExecutable(CodeDomProvider provider,
                                        string[] assemblies,
                                        String sourceCode)
        {
            bool compileOk = false;
            CompilerParameters cp = null;
            if (assemblies == null)
                cp = new CompilerParameters();
            else
                cp = new CompilerParameters(assemblies);
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;
            cp.TreatWarningsAsErrors = false;
            results = provider.CompileAssemblyFromSource(cp, sourceCode);

            if (results.Errors.Count > 0)
                compileOk = false;
            else
                compileOk = true;
            return compileOk;
        }
    }

    /// <summary>
    /// Factoria de interprete y parser para VBNet
    /// </summary>
    public class VBNetFactory : InterpreterFactory
    {
        /// <summary>
        /// Instancia el parser que se aplicará
        /// al script a ejecutar
        /// </summary>
        /// <returns>Parser</returns>
        public override ScriptParser createParser()
        {
            return new ASPStyleParser();
        }
        /// <summary>
        /// Instancia el interprete que se corresponde
        /// con el lenguaje en que esta escrito el script
        /// </summary>
        /// <returns>Interprete</returns>
        public override Interpreter createInterpreter()
        {
            return new VBNetInterpreter();
        }
    }

    /// <summary>
    /// /// Interprete para VBNet
    /// </summary>
    public class VBNetInterpreter : Interpreter
    {
        /// <summary>
        /// Ejecuta el script ya procesado por el parser correspondiente
        /// </summary>
        /// <param name="nativeCode">Código ya parseado</param>
        /// <param name="assemblies">Lista de assemblies necesarios para compilar el script</param>
        /// <returns>Verdadero si el resultado es exitoso</returns>
        public override bool Compile(string nativeCode, string[] assemblies)
        {
            if (assemblies.Length == 0)
                return base.CompileExecutable(new Microsoft.VisualBasic.VBCodeProvider(), nativeCode);
            else
                return base.CompileExecutable(new Microsoft.VisualBasic.VBCodeProvider(), assemblies, nativeCode);
        }
    }

    /// <summary>
    /// Factoria de interprete y parser para C Sharp
    /// </summary>
    public class CSharpFactory : InterpreterFactory
    {
        /// <summary>
        /// Instancia el parser que se aplicará
        /// al script a ejecutar
        /// </summary>
        /// <returns>Parser</returns>
        public override ScriptParser createParser()
        {
            return new ASPStyleParser();
        }
        /// <summary>
        /// Instancia el interprete que se corresponde
        /// con el lenguaje en que esta escrito el script
        /// </summary>
        /// <returns>Interprete</returns>
        public override Interpreter createInterpreter()
        {

            return new CSharpInterpreter();
        }
    }

    /// <summary>
    /// Parser estilo ASP
    /// &gt;% Inicio de script
    /// &lt;%> Fin de script
    /// </summary>
    public class ASPStyleParser : ScriptParser
    {
        /// <summary>
        /// Parser
        /// </summary>
        /// <param name="scriptText">Texto a parsear</param>
        /// <returns>Texto parseado</returns>
        public override string Parse(string scriptText)
        {
            string result = "";

            StringParser sp = new StringParser("<%", "%>");
            List<StringParser.CodeLine> cls = sp.GetScript(scriptText);

            int indexNoCode = 0;
            foreach (StringParser.CodeLine match in cls)
            {
                if (match.From > indexNoCode)
                    result += ParseLines(scriptText.Substring(indexNoCode, match.From - indexNoCode), false);
                indexNoCode = match.From + match.Length;
                result += ParseLines(match.Value.Replace("<%", "").Replace("%>", ""), true);
            }
            //if (indexNoCode < scriptText.Length) result += ParseLines(scriptText.Substring(indexNoCode), false);
            return result;
        }

        private string ParseLines(string scriptText, bool code)
        {
            string result = "";
            string[] lines = scriptText.Replace(string.Format("\r\n"), string.Format("\n")).Split(string.Format("\n").ToCharArray());
            for (int i = 0; i < lines.Length; i++)
            {
                if (i == lines.Length - 1 & lines[i] != "")
                {
                    if (!code)
                        result += string.Format("Response.Write(\"{0}\");\r\n", lines[i].Replace("\"", "\\\""));
                    else
                        result += string.Format("{0}\r\n", lines[i]);
                }
                else
                {
                    if (!code)
                        result += string.Format("Response.WriteLine(\"{0}\");\r\n", lines[i].Replace("\"", "\\\""));
                    else
                        result += string.Format("{0}\r\n", lines[i]);
                }
            }
            return result;
        }
    }

    /// <summary>
    /// /// Interprete para C Sharp
    /// </summary>
    public class CSharpInterpreter : Interpreter
    {
        /// <summary>
        /// Ejecuta el script ya procesado por el parser correspondiente
        /// </summary>
        /// <param name="nativeCode">Código ya parseado</param>
        /// <param name="assemblies">Lista de assemblies necesarios para compilar el script</param>
        /// <returns>Verdadero si el resultado es exitoso</returns>
        public override bool Compile(string nativeCode, string[] assemblies)
        {
            if (assemblies.Length == 0)
                return base.CompileExecutable(new Microsoft.CSharp.CSharpCodeProvider(), nativeCode);
            else
                return base.CompileExecutable(new Microsoft.CSharp.CSharpCodeProvider(), assemblies, nativeCode);
        }
    }

    /// <summary>
    /// Motor de scripting
    /// </summary>
    public class ScriptingHost
    {
        private ScriptParser sp = null;
        private Interpreter itr = null;

        private string scriptText = "";
        private string nativeCode = "";
        private string[] assemblies = new string[] { };

        /// <summary>
        /// Constructor del motor
        /// </summary>
        /// <param name="ify"></param>
        public ScriptingHost(InterpreterFactory ify)
        {
            this.sp = ify.createParser();
            this.itr = ify.createInterpreter();
        }
        /// <summary>
        /// Constructor del motor
        /// </summary>
        /// <param name="ify"></param>
        /// <param name="assemblies"></param>
        public ScriptingHost(InterpreterFactory ify, string[] assemblies)
        {
            this.sp = ify.createParser();
            this.itr = ify.createInterpreter();
            this.assemblies = assemblies;
        }
        /// <summary>
        /// Recibe el script a ejecutar
        /// </summary>
        /// <param name="scriptText">Script a ejecutar</param>
        public void SetScript(string scriptText)
        {
            this.scriptText = scriptText;
            this.nativeCode = this.sp.Parse(this.scriptText);
        }

        /// <summary>
        /// Retorna el código ya procesado por parser
        /// </summary>
        /// <returns></returns>
        public string GetNativeCode()
        {
            return nativeCode;
        }

        /// <summary>
        /// Metodo ejecutor del script
        /// </summary>
        /// <param name="className">Clase que se debe instanciar- (Es la clase resultado de la compilaci{on del script)</param>
        /// <param name="classInstance"></param>
        /// <param name="assemblyList"></param>
        /// <returns></returns>
        public CompilerResults Compile(string className, out object classInstance, string[] assemblyList)
        {
            //result = null;
            classInstance = null;
            assemblies = new string[assemblyList.Length+1];
            assemblies[0]=this.GetType().Assembly.ManifestModule.Name;
            for (int i = 1; i <= assemblyList.Length; i++)
                assemblies[i] = assemblyList[i - 1];

            this.itr.Compile(this.sp.Parse(this.scriptText), assemblies);
            System.Diagnostics.Debug.WriteLine(this.sp.Parse(this.scriptText));
            if (this.itr.Results.Errors.HasErrors)
                return this.itr.Results;
            else
            {
                //object o = this.itr.Results.CompiledAssembly.CreateInstance(className);
                //result = o.GetType().GetMethod(method).Invoke(o,args);
                classInstance = this.itr.Results.CompiledAssembly.CreateInstance(className);
                return this.itr.Results;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class ScriptingClass
    {
        /// <summary>
        /// 
        /// </summary>
        public event ProcessEvent Info;

        private Response response = new Response();
        /// <summary>
        /// 
        /// </summary>
        public Response Response
        {
            get { return response; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ScriptingClass()
        {
            response.Info += new ProcessEvent(response_Info);
        }

        void response_Info(object sender, EventArgs e)
        {
            if (Info != null)
            {
                Info(sender, e);
            }
        }

        //public abstract object Main();

    }

    /// <summary>
    /// 
    /// </summary>
    public class StringParser
    {
        private string initCode = "<%";
        private string endCode = "%>";

        /// <summary>
        /// 
        /// </summary>
        public StringParser() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initCode"></param>
        /// <param name="endCode"></param>
        public StringParser(string initCode, string endCode)
        {
            this.initCode = initCode;
            this.endCode = endCode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public List<CodeLine> GetScript(string script)
        {
            List<CodeLine> codeLines = new List<CodeLine>();
            int actual = 0;
            int end = 0;

            while (actual != -1)
            {
                actual = script.IndexOf(initCode, actual);
                if (actual != -1)
                {
                    end = script.IndexOf(endCode, actual);
                    if (end != -1)
                    {
                        codeLines.Add(new CodeLine(actual, end + 2, script.Substring(actual, (end + 2) - actual)));
                    }
                    actual++;
                }
            }
            return codeLines;
        }

        /// <summary>
        /// 
        /// </summary>
        public class CodeLine
        {
            private int from = 0;
            private int to = 0;
            private string code = "";

            /// <summary>
            /// 
            /// </summary>
            public int To { get { return to; } }

            /// <summary>
            /// 
            /// </summary>
            public int From { get { return from; } }

            /// <summary>
            /// 
            /// </summary>
            public string Code { get { return code; } }

            /// <summary>
            /// 
            /// </summary>
            public string Value { get { return code; } }

            /// <summary>
            /// 
            /// </summary>
            public int Length { get { return (to) - from; } }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="from"></param>
            /// <param name="to"></param>
            /// <param name="code"></param>
            internal CodeLine(int from, int to, string code)
            {
                this.from = from;
                this.to = to;
                this.code = code;
            }
        }
    }



}
