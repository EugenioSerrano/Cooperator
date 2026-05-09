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
using System.Runtime.Serialization;
using Cooperator.Framework.Utility.DBReverseHelper;
using System.Reflection;


namespace CooperatorModeler
{
    [Serializable] public class Snapshot
    {

        public Snapshot()
        {
            _ModelerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        private string _ModelerVersion = "";
        public string ModelerVersion
        {
            get { return _ModelerVersion; }
            set
            {
                if (_ModelerVersion != value) _haveChanges = true;
                _ModelerVersion = value;
            }
        }

        private string _ConnectionString = "";
        public string ConnectionString
        {
            get { return _ConnectionString; }
            set
            {
                if (_ConnectionString != value) _haveChanges = true;
                _ConnectionString = value;
            }
        }

        private string _DataBaseName = "";
        public string DataBaseName
        {
            get { return _DataBaseName; }
            set
            {
                if (_DataBaseName != value) _haveChanges = true;
                _DataBaseName = value;
            }
        }

        private string _FileName = "";
        public string FileName
        {
            get { return _FileName; }
            set
            {
                if (_FileName != value) _haveChanges = true;
                _FileName = value;
            }
        }

        private string _SPPrefix = "";
        public string SPPrefix
        {
            get { return _SPPrefix; }
            set
            {
                if (_SPPrefix != value) _haveChanges = true;
                _SPPrefix = value;
            }
        }

        private string _RulesProjectName = "";
        public string RulesProjectName
        {
            get { return _RulesProjectName; }
            set
            {
                if (_RulesProjectName != value) _haveChanges = true;
                _RulesProjectName = value;
            }
        }

        private string _DataProjectName = "";
        public string DataProjectName
        {
            get { return _DataProjectName; }
            set
            {
                if (_DataProjectName != value) _haveChanges = true;
                _DataProjectName = value;
            }
        }

        private string _EntitiesProjectName = "";
        public string EntitiesProjectName
        {
            get { return _EntitiesProjectName; }
            set
            {
                if (_EntitiesProjectName != value) _haveChanges = true;
                _EntitiesProjectName = value;
            }
        }

        private string _AppProjectName = "";
        public string AppProjectName
        {
            get { return _AppProjectName; }
            set
            {
                if (_AppProjectName != value) _haveChanges = true;
                _AppProjectName = value;
            }
        }

        private string _DeployFolder = "";
        public string DeployFolder
        {
            get { return _DeployFolder; }
            set
            {
                if (_DeployFolder != value) _haveChanges = true;
                _DeployFolder = value;
            }
        }

        private string _Language = "";
        public string Language
        {
            get { return _Language; }
            set
            {
                if (_Language != value) _haveChanges = true;
                _Language = value;
            }
        }

        private bool _GenerateAutoFileOnly = false;
        public bool GenerateAutoFileOnly
        {
            get { return _GenerateAutoFileOnly; }
            set
            {
                if (_GenerateAutoFileOnly != value) _haveChanges = true;
                _GenerateAutoFileOnly = value;
            }
        }

        private bool _StampDateAndTimeOnAutoFiles = true;
        public bool StampDateAndTimeOnAutoFiles
        {
            get
            {
                return _StampDateAndTimeOnAutoFiles;
            }
            set
            {
                if (_StampDateAndTimeOnAutoFiles != value) _haveChanges = true;
                _StampDateAndTimeOnAutoFiles = value;
            }
        }


        private bool _GenerateCheckForToken = true;
        public bool GenerateCheckForToken
        {
            get { return _GenerateCheckForToken; }
            set
            {
                if (_GenerateCheckForToken != value) _haveChanges = true;
                _GenerateCheckForToken = value;
            }
        }

        private string _PostGenerationScript = "";
        public string PostGenerationScript
        {
            get { return _PostGenerationScript; }
            set
            {
                if (_PostGenerationScript != value) _haveChanges = true;
                _PostGenerationScript = value;
            }
        }



        private BaseTreeNode _DomainTree = null;
        public BaseTreeNode DomainTree
        {
            get { return _DomainTree; }
            set
            {
                _haveChanges = true;
                _DomainTree = value;
            }
        }

        private bool _haveChanges;
        public bool HaveChanges
        {
            get
            {
                if (_haveChanges) return true;
                if (this._DomainTree != null)
                {
                    foreach (EntityNode currentEntity in this._DomainTree.Children)
                    {
                        if (currentEntity.HaveChanges) return true;
                        foreach (PropertyNode currentProperty in currentEntity.Children)
                        {
                            if (currentProperty.HaveChanges) return true;
                        }
                    }
                }
                return false;
            }
            set
            {
                if (value)
                {
                    _haveChanges = value;
                }
                else
                {
                    _haveChanges = false;
                    if (this._DomainTree != null)
                    {
                        foreach (EntityNode currentEntity in this._DomainTree.Children)
                        {
                            currentEntity.HaveChanges = false;
                            foreach (PropertyNode currentProperty in currentEntity.Children)
                            {
                                currentProperty.HaveChanges = false;
                            }
                        }
                    }
                }
            }
        }

    }
}
