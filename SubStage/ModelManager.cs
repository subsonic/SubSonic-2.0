/*
 * SubSonic - http://subsonicproject.com
 * 
 * The contents of this file are subject to the Mozilla Public
 * License Version 1.1 (the "License"); you may not use this file
 * except in compliance with the License. You may obtain a copy of
 * the License at http://www.mozilla.org/MPL/
 * 
 * Software distributed under the License is distributed on an 
 * "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or
 * implied. See the License for the specific language governing
 * rights and limitations under the License.
*/

using System;
using System.IO;

namespace SubSonic.SubStage
{
    public sealed class ModelManager : MasterStore
    {
        private static readonly ModelManager instance = new ModelManager();
        private static string _storePath;

        private ModelManager()
        {
            lock(this)
            {
                Clear();
                bool isNew = false;
                EnforceConstraints = false;
                if(!File.Exists(StorePath))
                {
                    ReadXml(AppDomain.CurrentDomain.BaseDirectory + "\\ProviderTypes.xml");
                    isNew = true;
                }
                else
                    ReadXml(StorePath);
                EnforceConstraints = true;

                if(isNew)
                    Save();
            }
        }

        public static ModelManager Instance
        {
            get { return instance; }
        }

        public static string StorePath
        {
            get
            {
                if(String.IsNullOrEmpty(_storePath))
                    _storePath = AppDomain.CurrentDomain.BaseDirectory + "\\MasterStore.xml";
                return _storePath;
            }
            set { _storePath = value; }
        }

        public void Save()
        {
            //if(System.IO.File.Exists(StorePath))
            //{
            lock(this)
            {
                AcceptChanges();
                WriteXml(StorePath);
            }
            //}
        }
    }
}