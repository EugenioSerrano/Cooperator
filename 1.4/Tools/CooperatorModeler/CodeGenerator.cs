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
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using System.Reflection;
using Cooperator.Framework.Utility.DBReverseHelper;
using Cooperator.Framework.Utility.CodeGeneratorHelper;
using Cooperator.Framework.Utility.Exceptions;

namespace CooperatorModeler
{

    public class CodeGenerator
    {
        private string AssemblyName()
        {
            return GetType().Assembly.ManifestModule.Name;
        }

        private static int CountLines(string script)
        {
            int count = 0;
            for (int i = 0; i < script.Length; i++)
            {
                if (script[i] == '\r') count++;
            }
            return count;
        }

        /// <summary>
        /// Generate Stored Procedures
        /// </summary>
        /// <param name="snapshot"></param>
        public static void GenerateStoredProcedures(Snapshot snapshot)
        {

            DomainTreeHelper.CorrectGenerationNamesForRelatedFields(snapshot.DomainTree);

            FolderHelper.CreateStoredProcedureFolders();


            string currentScript = "";
            int numberOfFiles = 0;
            int numberOfLines = 0;

            try
            {
                GeneratorRules.Instance().SetConnectionString(snapshot.ConnectionString);
                Dictionary<string, string> buffers;
                var scripts = new List<string>
                {
                    "StoredProcedures\\DescriptionFunction.cs",
                    "StoredProcedures\\GetAll.cs",
                    "StoredProcedures\\Insert.cs",
                    "StoredProcedures\\GetOne.cs",
                    "StoredProcedures\\Delete.cs",
                    "StoredProcedures\\Update.cs",
                    "StoredProcedures\\GetByParent.cs",
                    "StoredProcedures\\GetByParents.cs",
                    "StoredProcedures\\DeleteByParents.cs",
                    "StoredProcedures\\Queries.cs"
                };


                UI.ProgessBar.Value = UI.ProgessBar.Minimum;
                UI.ProgessBar.Maximum = scripts.Count;

                foreach (string script in scripts)
                {
                    // Limpio porque si hay un error en GenerateFromTemplate(..)
                    // buffers contendría el script inmediato anterior.
                    currentScript = "";
                    buffers = null;

                    bool skipGeneration = true;
                    if (script == "StoredProcedures\\DescriptionFunction.cs")
                    {
                        // check if generation are needful
                        foreach (EntityNode currentEntity in snapshot.DomainTree.Children)
                        {
                            foreach (PropertyNode currentProperty in currentEntity.Children)
                            {
                                if (currentProperty.GenerateProperty && currentProperty.IsDescriptionField)
                                    skipGeneration = false;
                            }
                        }
                    }
                    else
                    {
                        skipGeneration = false;
                    }

                    if (!skipGeneration)
                    {
                        buffers = GenerateFromTemplate("SqlServerScriptor", script, snapshot, false);

                        string tempsp = "";
                        foreach (string key in buffers.Keys)
                        {
                            // Ejecutamos el script
                            currentScript = buffers[key];
                            GeneratorRules.Instance().ExecuteNoQueryBySQLText(buffers[key]);

                            if (key.ToLower().Contains("drop"))
                            {
                                tempsp = buffers[key];
                            }
                            else
                            {
                                // Grabamos el StoredProcedure en un archivo
                                string fileName = Cooperator.Framework.Library.IO.Path.AppPath(true) + "GeneratedCode\\StoredProcedures\\" + snapshot.SPPrefix + key + ".sql";
                                string completeScript = tempsp + "\r\n\r\n" + buffers[key];
                                Cooperator.Framework.Library.IO.File.SaveStringToFile(fileName, completeScript, true);
                                numberOfLines += CountLines(completeScript);
                                numberOfFiles++;
                            }
                        }
                    }
                    UI.ProgessBar.Value++;
                    System.Windows.Forms.Application.DoEvents();
                }
                //foreach (string key in buffers.Keys) System.Windows.Forms.MessageBox.Show(buffers[key]);

                if (!String.IsNullOrEmpty(snapshot.PostGenerationScript))
                {
                    string[] separator = { "GO" };
                    string[] postGenerationScripts = snapshot.PostGenerationScript.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    // Ejecutamos el script final
                    foreach (string s in postGenerationScripts)
                        GeneratorRules.Instance().ExecuteNoQueryBySQLText(s);
                    // Grabamos el StoredProcedure final en un archivo
                    string fileName2 = Cooperator.Framework.Library.IO.Path.AppPath(true) + "GeneratedCode\\StoredProcedures\\PostGenerationScript.sql";
                    Cooperator.Framework.Library.IO.File.SaveStringToFile(fileName2, snapshot.PostGenerationScript, true);
                    numberOfLines += CountLines(snapshot.PostGenerationScript);
                    numberOfFiles++;
                }
            }
            catch (Exception ex)
            {
                ShowError errorForm = new ShowError();
                errorForm.LogTextBox.Text = ex.Message + "\r\n\r\n" + ex.StackTrace + "\r\n\r\n" + currentScript;
                errorForm.ShowDialog();
            }
            finally
            {
                UI.ProgessBar.Value = UI.ProgessBar.Maximum;
                UI.StatusLabel = $"Ready. {numberOfFiles} files, {numberOfLines} lines generated.";
            }
        }



        /// <summary>
        /// Generate Classes
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static bool GenerateClasses(Snapshot snapshot)
        {
            DomainTreeHelper.CorrectGenerationNamesForRelatedFields(snapshot.DomainTree);

            FolderHelper.CreateCodeFolders();

            string currentScript = "";
            int numberOfFiles = 0;
            int numberOfLines = 0;

            bool isOk = true;
            try
            {
                Dictionary<string, string> buffers;
                List<string> scripts = new List<string>();
                string templatePattern = "";
                switch (snapshot.Language)
                {
                    case "VB":
                        templatePattern = "VisualBasic";
                        break;
                    case "CS":
                        templatePattern = "CSharp";
                        break;
                    default:
                        throw new ApplicationException("Model file are corrupt.");
                }


                if (!snapshot.GenerateAutoFileOnly) scripts.Add(templatePattern + "Classes\\Object.cs");
                scripts.Add(templatePattern + "Classes\\Object.Auto.cs");
                if (!snapshot.GenerateAutoFileOnly) scripts.Add(templatePattern + "Classes\\Gateway.cs");
                scripts.Add(templatePattern + "Classes\\Gateway.Auto.cs");
                if (!snapshot.GenerateAutoFileOnly) scripts.Add(templatePattern + "Classes\\Entity.cs");
                scripts.Add(templatePattern + "Classes\\Entity.Auto.cs");
                if (!snapshot.GenerateAutoFileOnly) scripts.Add(templatePattern + "Classes\\Mapper.cs");
                scripts.Add(templatePattern + "Classes\\Mapper.Auto.cs");

                scripts.Add(templatePattern + "Classes\\RuleExample.cs");
                scripts.Add(templatePattern + "Classes\\DefaultLazyProvider.cs");

                scripts.Add("ConfigFiles\\App.config.cs");

                switch (templatePattern)
                {
                    case "CSharp":
                        scripts.Add("CSharpProjects\\AppProject.csproj");
                        scripts.Add("CSharpProjects\\EntitiesProject.csproj");
                        scripts.Add("CSharpProjects\\RulesProject.csproj");
                        scripts.Add("CSharpProjects\\DataProject.csproj");
                        scripts.Add("CSharpProjects\\Solution.sln");
                        scripts.Add("CSharpProjects\\AppProjectFiles\\Form1.cs");
                        scripts.Add("CSharpProjects\\AppProjectFiles\\Form1.Designer.cs");
                        scripts.Add("CSharpProjects\\AppProjectFiles\\Program.cs");

                        break;
                    case "VisualBasic":
                        scripts.Add("VisualBasicProjects\\AppProject.vbproj");
                        scripts.Add("VisualBasicProjects\\EntitiesProject.vbproj");
                        scripts.Add("VisualBasicProjects\\RulesProject.vbproj");
                        scripts.Add("VisualBasicProjects\\DataProject.vbproj");
                        scripts.Add("VisualBasicProjects\\Solution.sln");
                        scripts.Add("VisualBasicProjects\\AppProjectFiles\\Form1.vb");
                        scripts.Add("VisualBasicProjects\\AppProjectFiles\\Form1.Designer.vb");

                        break;
                    default:
                        break;
                }

                UI.ProgessBar.Value = UI.ProgessBar.Minimum;
                UI.ProgessBar.Maximum = scripts.Count;

                bool resetGuids = true;
                foreach (string script in scripts)
                {
                    // Limpio porque si hay un error en GenerateFromTemplate(..)
                    // buffers contendría el script inmediato anterior.
                    buffers = null;
                    buffers = GenerateFromTemplate(templatePattern + "Scriptor", script, snapshot, resetGuids);
                    resetGuids = false;
                    foreach (string key in buffers.Keys)
                    {
                        // Grabamos el archivo
                        string fileName = Cooperator.Framework.Library.IO.Path.AppPath(true) + "GeneratedCode" + key;
                        string fileContent = buffers[key];
                        if (fileName.Contains("GeneratedCode\\ProjectFiles") || fileName.Contains("GeneratedCode\\ConfigFiles"))
                        {
                            //Substring(10) eliminates the "\r\n" at start of file
                            fileContent = fileContent.Substring(10);
                        }
                        Cooperator.Framework.Library.IO.File.SaveStringToFile(fileName, fileContent, true);
                        numberOfLines += CountLines(buffers[key]);
                        numberOfFiles++;
                    }
                    UI.ProgessBar.Value++;
                    System.Windows.Forms.Application.DoEvents();
                }
                //foreach (string key in buffers.Keys) System.Windows.Forms.MessageBox.Show(buffers[key]);

                FolderHelper.CopyFolder(Cooperator.Framework.Library.IO.Path.AppPath(true) + "Templates\\Assemblies\\", Cooperator.Framework.Library.IO.Path.AppPath(true) + "GeneratedCode\\Assemblies\\", false, true);

            }
            catch (Exception ex)
            {
                ShowError errorForm = new ShowError();
                errorForm.LogTextBox.Text = ex.Message + "\r\n\r\n" + ex.StackTrace + "\r\n\r\n" + currentScript;
                errorForm.ShowDialog();
                isOk = false;
            }
            finally
            {
                UI.ProgessBar.Value = UI.ProgessBar.Maximum;
                UI.StatusLabel = string.Format("Ready. {0} files, {1} lines generated.", numberOfFiles, numberOfLines);
            }
            return isOk;
        }





        /// <summary>
        /// Generate Solution
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        internal static bool GenerateSolution(Snapshot snapshot)
        {
            bool isOk = true;
            try
            {
                Snapshot s = snapshot;

                if (!Directory.Exists(s.DeployFolder))
                    throw new ApplicationException("The folder '" + s.DeployFolder + "' do not exists.");

                if (Directory.GetFiles(s.DeployFolder, "*.*", SearchOption.AllDirectories).Length != 0)
                    throw new ApplicationException("The folder '" + s.DeployFolder + "' is not empty.");

                string appProjectFolder = s.DeployFolder + "\\" + s.AppProjectName;
                string rulesProjectFolder = s.DeployFolder + "\\" + s.RulesProjectName;
                string dataProjectFolder = s.DeployFolder + "\\" + s.DataProjectName;
                string entitiesProjectFolder = s.DeployFolder + "\\" + s.EntitiesProjectName;
                string assembliesFolder = s.DeployFolder + "\\CooperatorAssemblies";

                string entitiesFolder = entitiesProjectFolder + "\\Entities";
                string objectsFolder = entitiesProjectFolder + "\\Objects";

                string gatewaysFolder = dataProjectFolder + "\\Gateways";
                string mappersFolder = dataProjectFolder + "\\Mappers";
                string lazyProvidersFolder = dataProjectFolder + "\\LazyProviders";

                Directory.CreateDirectory(dataProjectFolder);
                Directory.CreateDirectory(rulesProjectFolder);
                Directory.CreateDirectory(entitiesProjectFolder);
                Directory.CreateDirectory(appProjectFolder);

                System.Windows.Forms.Application.DoEvents();

                Directory.CreateDirectory(entitiesFolder);
                Directory.CreateDirectory(objectsFolder);

                Directory.CreateDirectory(gatewaysFolder);
                Directory.CreateDirectory(mappersFolder);
                Directory.CreateDirectory(lazyProvidersFolder);

                Directory.CreateDirectory(assembliesFolder);

                System.Windows.Forms.Application.DoEvents();

                string GeneratedCodeFolder = Cooperator.Framework.Library.IO.Path.AppPath(true) + "GeneratedCode";

                FolderHelper.CopyFolder(GeneratedCodeFolder + "\\Entities", entitiesFolder, false, false);
                FolderHelper.CopyFolder(GeneratedCodeFolder + "\\Objects", objectsFolder, false, false);
                FolderHelper.CopyFolder(GeneratedCodeFolder + "\\Rules", rulesProjectFolder, false, false);

                FolderHelper.CopyFolder(GeneratedCodeFolder + "\\Mappers", mappersFolder, false, false);
                FolderHelper.CopyFolder(GeneratedCodeFolder + "\\Gateways", gatewaysFolder, false, false);
                FolderHelper.CopyFolder(GeneratedCodeFolder + "\\LazyProviders", lazyProvidersFolder, false, false);

                FolderHelper.CopyFolder(GeneratedCodeFolder + "\\Assemblies", assembliesFolder, false, false);
                FolderHelper.CopyFolder(GeneratedCodeFolder + "\\AppProjectFiles", appProjectFolder, false, false);
                FolderHelper.CopyFolder(GeneratedCodeFolder + "\\ConfigFiles", appProjectFolder, false, false);

                System.Windows.Forms.Application.DoEvents();

                string solutionFileName = "";
                string dataProjectFileName = "";
                string rulesProjectFileName = "";
                string entitiesProjectFileName = "";
                string appProjectFileName = "";

                switch (s.Language)
                {
                    case "CS":
                        solutionFileName = GeneratedCodeFolder + "\\ProjectFiles\\" + snapshot.AppProjectName + ".sln";
                        dataProjectFileName = GeneratedCodeFolder + "\\ProjectFiles\\" + snapshot.DataProjectName + ".csproj";
                        rulesProjectFileName = GeneratedCodeFolder + "\\ProjectFiles\\" + snapshot.RulesProjectName + ".csproj";
                        entitiesProjectFileName = GeneratedCodeFolder + "\\ProjectFiles\\" + snapshot.EntitiesProjectName + ".csproj";
                        appProjectFileName = GeneratedCodeFolder + "\\ProjectFiles\\" + snapshot.AppProjectName + ".csproj";

                        break;

                    case "VB":
                        solutionFileName = GeneratedCodeFolder + "\\ProjectFiles\\" + snapshot.AppProjectName + ".sln";
                        dataProjectFileName = GeneratedCodeFolder + "\\ProjectFiles\\" + snapshot.DataProjectName + ".vbproj";
                        rulesProjectFileName = GeneratedCodeFolder + "\\ProjectFiles\\" + snapshot.RulesProjectName + ".vbproj";
                        entitiesProjectFileName = GeneratedCodeFolder + "\\ProjectFiles\\" + snapshot.EntitiesProjectName + ".vbproj";
                        appProjectFileName = GeneratedCodeFolder + "\\ProjectFiles\\" + snapshot.AppProjectName + ".vbproj";

                        break;
                    default:
                        break;
                }

                File.Copy(solutionFileName, solutionFileName.Replace(GeneratedCodeFolder, s.DeployFolder).Replace("\\ProjectFiles", ""));
                File.Copy(rulesProjectFileName, rulesProjectFileName.Replace(GeneratedCodeFolder, s.DeployFolder + "\\" + s.RulesProjectName).Replace("\\ProjectFiles", ""));
                File.Copy(dataProjectFileName, dataProjectFileName.Replace(GeneratedCodeFolder, s.DeployFolder + "\\" + s.DataProjectName).Replace("\\ProjectFiles", ""));
                File.Copy(entitiesProjectFileName, entitiesProjectFileName.Replace(GeneratedCodeFolder, s.DeployFolder + "\\" + s.EntitiesProjectName).Replace("\\ProjectFiles", ""));
                File.Copy(appProjectFileName, appProjectFileName.Replace(GeneratedCodeFolder, s.DeployFolder + "\\" + s.AppProjectName).Replace("\\ProjectFiles", ""));

                System.Windows.Forms.Application.DoEvents();

            }
            catch (Exception ex)
            {
                ShowError errorForm = new ShowError();
                errorForm.LogTextBox.Text = ex.Message;
                errorForm.ShowDialog();
                isOk = false;
            }
            return isOk;
        }



        /// <summary>
        /// Update Solution
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="onlyAutoFiles"></param>
        internal static bool UpdateSolution(Snapshot snapshot, bool onlyAutoFiles)
        {
            UI.StatusLabel = "Updating solution...";
            System.Windows.Forms.Application.DoEvents();
            bool isOk = true;
            try
            {
                Snapshot s = snapshot;

                if (!Directory.Exists(s.DeployFolder))
                    throw new ApplicationException("The folder '" + s.DeployFolder + "' do not exists.");

                if (Directory.GetFiles(s.DeployFolder, "*.*", SearchOption.AllDirectories).Length == 0)
                    throw new ApplicationException("The folder '" + s.DeployFolder + "' is empty.");

                System.Windows.Forms.Application.DoEvents();

                string appProjectFolder = s.DeployFolder + "\\" + s.AppProjectName;
                string rulesProjectFolder = s.DeployFolder + "\\" + s.RulesProjectName;
                string dataProjectFolder = s.DeployFolder + "\\" + s.DataProjectName;
                string entitiesProjectFolder = s.DeployFolder + "\\" + s.EntitiesProjectName;
                string assembliesFolder = s.DeployFolder + "\\CooperatorAssemblies";

                string entitiesFolder = entitiesProjectFolder + "\\Entities";
                string objectsFolder = entitiesProjectFolder + "\\Objects";

                string gatewaysFolder = dataProjectFolder + "\\Gateways";
                string mappersFolder = dataProjectFolder + "\\Mappers";
                string lazyProvidersFolder = dataProjectFolder + "\\LazyProviders";

                string GeneratedCodeFolder = Cooperator.Framework.Library.IO.Path.AppPath(true) + "GeneratedCode";

                FolderHelper.CopyFolder(GeneratedCodeFolder + "\\Entities", entitiesFolder, onlyAutoFiles, true);
                FolderHelper.CopyFolder(GeneratedCodeFolder + "\\Objects", objectsFolder, onlyAutoFiles, true);

                System.Windows.Forms.Application.DoEvents();

                FolderHelper.CopyFolder(GeneratedCodeFolder + "\\Mappers", mappersFolder, onlyAutoFiles, true);
                FolderHelper.CopyFolder(GeneratedCodeFolder + "\\Gateways", gatewaysFolder, onlyAutoFiles, true);
                FolderHelper.CopyFolder(GeneratedCodeFolder + "\\LazyProviders", lazyProvidersFolder, false, true);

                System.Windows.Forms.Application.DoEvents();

            }
            catch (Exception ex)
            {
                ShowError errorForm = new ShowError();
                errorForm.LogTextBox.Text = ex.Message;
                errorForm.ShowDialog();
                isOk = false;
            }
            return isOk;
        }



        private static Guid _appGuid;
        private static Guid _entitiesGuid;
        private static Guid _dataGuid;
        private static Guid _rulesGuid;


        private static Dictionary<string, string> GenerateFromTemplate(string ScriptorProviderName, string TemplateName, Snapshot snapshot, bool resetGuids)
        {

            if (resetGuids || _appGuid == null)
            {
                _appGuid = Guid.NewGuid();
                _entitiesGuid = Guid.NewGuid();
                _dataGuid = Guid.NewGuid();
                _rulesGuid = Guid.NewGuid();
            }

            string FileName = Cooperator.Framework.Library.IO.Path.AppPath(true) + "ScriptorProviders\\RealScript.cs";
            StreamReader sourceCode = File.OpenText(FileName);
            string ss = sourceCode.ReadToEnd();
            sourceCode.Close();
            sourceCode.Dispose();

            StringBuilder sb = new StringBuilder(ss);
            FileName = Cooperator.Framework.Library.IO.Path.AppPath(true) + "Templates\\" + TemplateName;
            StreamReader txtScript = File.OpenText(FileName);
            sb.Replace(@"//{INSERTCODE}//", txtScript.ReadToEnd());
            txtScript.Close();
            txtScript.Dispose();
            sb.Replace(@"//{PROVIDERCLASSNAME}//", ScriptorProviderName);

            ScriptingHost sh = new ScriptingHost(new CSharpFactory());
            sh.SetScript(sb.ToString());

            string compilatingCode = sh.GetNativeCode();
            object o;

            string[] assemblies = new string[2];
            assemblies[0] = new CodeGenerator().AssemblyName();
            assemblies[1] = new System.Windows.Forms.Form().GetType().Assembly.ManifestModule.Name;

            CompilerResults cr = sh.Compile("RealScript", out o, assemblies);

            if (cr.Errors.HasErrors)
            {
                string ceText = "";
                foreach (CompilerError ce in cr.Errors)
                {
                    ceText += string.Format("Linea {0}, posición {1}  {2}-{3}\r\n", ce.Line, ce.Column, ce.ErrorNumber, ce.ErrorText);
                }
                throw new CompilerException(ceText + "\r\n\r\n" + compilatingCode);
            }

            ScriptorBaseProvider sc = o as ScriptorBaseProvider;

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("GeneratorVersion", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            if (snapshot.StampDateAndTimeOnAutoFiles)
                parameters.Add("AutoFilesDateAndTime", DateTime.Today.ToShortDateString() + " - " + DateTime.Now.ToShortTimeString());
            else
                parameters.Add("AutoFilesDateAndTime", "");

            parameters.Add("DataBaseName", snapshot.DataBaseName);
            parameters.Add("SpPrefix", snapshot.SPPrefix);
            parameters.Add("ConnectionString", snapshot.ConnectionString);
            parameters.Add("GenerateCheckForToken", snapshot.GenerateCheckForToken.ToString().ToUpper());

            parameters.Add("RulesProjectName", snapshot.RulesProjectName);
            parameters.Add("EntitiesProjectName", snapshot.EntitiesProjectName);
            parameters.Add("DataProjectName", snapshot.DataProjectName);
            parameters.Add("AppProjectName", snapshot.AppProjectName);

            parameters.Add("AppProjectGuid", _appGuid.ToString());
            parameters.Add("RulesProjectGuid", _rulesGuid.ToString());
            parameters.Add("EntitiesProjectGuid", _entitiesGuid.ToString());
            parameters.Add("DataProjectGuid", _dataGuid.ToString());

            parameters.Add("SolutionName", snapshot.AppProjectName);

            try
            {
                sc.Main(snapshot.DomainTree, parameters);
            }
            catch (Exception ex)
            {
                throw new System.Exception(ex.Message + "\r\n\r\n" + ex.StackTrace + "\r\n\r\n" + compilatingCode + "\r\n\r\n" + sb.ToString());
            }

            return sc.Response.OuputBuffers;
        }

    }
}
