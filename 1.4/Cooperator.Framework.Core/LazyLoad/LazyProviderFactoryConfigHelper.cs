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
using System.Configuration;

namespace Cooperator.Framework.Core.LazyLoad
{
    /// <summary>
    /// 
    /// </summary>
    public class Provider : ConfigurationElement
    {
        /// <summary>
        /// 
        /// </summary>
        internal Provider() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        internal Provider(string entityName)
            : this()
        {
            this["entityname"] = entityName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="assemblyName"></param>
        internal Provider(string entityName, string assemblyName)
            : this(entityName)
        {
            this["assemblyname"] = assemblyName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="assemblyName"></param>
        /// <param name="className"></param>
        public Provider(string entityName, string assemblyName, string className)
            : this(entityName, assemblyName)
        {
            this["classname"] = className;
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("entityname", IsKey = true, IsRequired = true)]
        public string EntityName { get { return (string)this["entityname"]; } set { this["entityname"] = value; } }
        
        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("classname", IsRequired = true)]
        public string ClassName { get { return (string)this["classname"]; } set { this["classname"] = value; } }
        
        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("assemblyname", IsRequired = true)]
        public string AssemblyName { get { return (string)this["assemblyname"]; } set { this["assemblyname"] = value; } }

    }

    /// <summary>
    /// 
    /// </summary>
    public class ProvidersCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// 
        /// </summary>
        protected internal ProvidersCollection()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.AddRemoveClearMap;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new Provider();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement(string elementName)
        {
            return new Provider(elementName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return $"{((Provider) element).EntityName}";
        }

        /// <summary>
        /// 
        /// </summary>
        public new string AddElementName
        {
            get
            { return base.AddElementName; }

            set
            { base.AddElementName = value; }

        }

        /// <summary>
        /// 
        /// </summary>
        public new string ClearElementName
        {
            get
            { return base.ClearElementName; }

            set
            { base.AddElementName = value; }

        }

        /// <summary>
        /// 
        /// </summary>
        public new string RemoveElementName
        {
            get
            { return base.RemoveElementName; }


        }

        /// <summary>
        /// 
        /// </summary>
        public new int Count
        {

            get { return base.Count; }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Provider this[int index]
        {
            get
            {
                return (Provider)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        new public Provider this[string Name]
        {
            get
            {
                return (Provider)BaseGet(Name);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="psh"></param>
        /// <returns></returns>
        public int IndexOf(Provider psh)
        {
            return BaseIndexOf(psh);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="psh"></param>
        public void Add(Provider psh)
        {
            BaseAdd(psh);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="psh"></param>
        public void Remove(Provider psh)
        {
            if (BaseIndexOf(psh) >= 0)
                BaseRemove(psh.EntityName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void Remove(string name)
        {
            BaseRemove(name);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            BaseClear();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ProvidersSection : ConfigurationSection
    {
        /// <summary>
        /// 
        /// </summary>
        protected internal ProvidersSection()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("providers", IsDefaultCollection = false)]
        public ProvidersCollection Providers
        {
            get
            {
                ProvidersCollection providers =
                (ProvidersCollection)base["providers"];
                return providers;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected override void DeserializeSection(
            System.Xml.XmlReader reader)
        {
            base.DeserializeSection(reader);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="name"></param>
        /// <param name="saveMode"></param>
        /// <returns></returns>
        protected override string SerializeSection(
            ConfigurationElement parentElement,
            string name, ConfigurationSaveMode saveMode)
        {
            string s =
                base.SerializeSection(parentElement,
                name, saveMode);
            return s;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class LazyConfigurationHelper
    {
        private static ProvidersSection providers;

        /// <summary>
        /// 
        /// </summary>
        public static ProvidersSection ProvidersSection { get { return providers; } }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool Read()
        {
            return Read(GetFileName());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool Read(string fileName)
        {
            ConfigurationSectionGroup csg = null;
            ConfigurationSection cs = null;

            Configuration cfg = GetConfiguration(fileName);
            if (cfg.HasFile)
            {
                csg = cfg.SectionGroups["LazyLoad"];
                if (csg != null)
                {
                    cs = cfg.SectionGroups["LazyLoad"].Sections["ProvidersSection"];
                    if (cs != null)
                        providers = cfg.SectionGroups["LazyLoad"].Sections["ProvidersSection"] as ProvidersSection;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void CreateEmpty()
        {
            CreateEmpty(GetFileName());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public static void CreateEmpty(string fileName)
        {
            Configuration cfg = GetConfiguration(fileName);
            cfg.SectionGroups.Remove("LazyLoad");
            ProvidersSection lps = new ProvidersSection();
            lps.Providers.Add(new Provider("Provider1", "miAssembly", "myProvider01"));
            lps.Providers.Add(new Provider("Provider2", "miAssembly", "myProvider02"));
            lps.Providers.Add(new Provider("Provider3", "miAssembly", "MyProvider03"));
            cfg.SectionGroups.Add("LazyLoad", new ConfigurationSectionGroup());
            cfg.SectionGroups["LazyLoad"].Sections.Add("ProvidersSection", lps);
            cfg.Save(ConfigurationSaveMode.Full);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static Configuration GetConfiguration(string fileName)
        {
            Configuration cfg = ConfigurationManager.OpenMappedExeConfiguration(
                GetExeConfigurationFileMap(fileName),
                ConfigurationUserLevel.None);
            return cfg;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string GetFileName()
        {
            return GetFileName(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string GetFileName(string fileName)
        {
            if (fileName == null || fileName.Trim().Length == 0)
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                string file = AppDomain.CurrentDomain.FriendlyName;
                fileName = string.Format("{0}{1}.config", path, file);
            }
            return fileName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static ExeConfigurationFileMap GetExeConfigurationFileMap(string fileName)
        {
            ExeConfigurationFileMap eCnfFile = new ExeConfigurationFileMap();
            eCnfFile.ExeConfigFilename = fileName;
            return eCnfFile;
        }

    }

}
