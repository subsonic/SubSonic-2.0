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

using System.Data;
using System.Windows.Forms;
using DM = SubSonic.SubStage.MasterStore;

namespace SubSonic.SubStage
{
    public enum StageNodeType
    {
        ColumnNode,
        ColumnPrimaryKeyNode,
        ConnectionString,
        ConnectionStringFolder,
        Project,
        ProjectFolder,
        Provider,
        StoredProcedure,
        StoredProcedureExcluded,
        StoredProcedureFolder,
        StoredProcedureParameter,
        SubStageConfiguration,
        Table,
        TableFolder,
        TableWithoutPrimaryKey,
        TableExcluded,
        View,
        ViewExcluded,
        ViewFolder
    }

    public class StageNode : TreeNode
    {
        private MasterStore.ConnectionStringsRow _connectionString;
        private string _databaseName;
        private bool _isPrimaryKeyColumn;
        private string _itemKey;
        private StageNodeType _nodeType;
        private MasterStore.ProjectsRow _project;
        private MasterStore.ProvidersRow _provider;
        private int _rowId;
        private string _subSonicName;
        private bool _tableHasPrimaryKey = true;

        public StageNode(string nodeText, DataRow row)
        {
            Text = nodeText;
            if(row != null)
            {
                if(row is MasterStore.ProjectsRow)
                    Project = (MasterStore.ProjectsRow)row;
                else if(row is MasterStore.ProvidersRow)
                {
                    Provider = (MasterStore.ProvidersRow)row;
                    Project = Provider.Project;
                }
                else if(row is MasterStore.ConnectionStringsRow)
                    ConnectionString = (MasterStore.ConnectionStringsRow)row;
            }
        }

        public MasterStore.ConnectionStringsRow ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        public MasterStore.ProvidersRow Provider
        {
            get { return _provider; }
            set { _provider = value; }
        }

        public MasterStore.ProjectsRow Project
        {
            get { return _project; }
            set { _project = value; }
        }

        public StageNodeType NodeType
        {
            get { return _nodeType; }
            set { _nodeType = value; }
        }

        public int RowId
        {
            get { return _rowId; }
            set { _rowId = value; }
        }

        public string ItemKey
        {
            get { return _itemKey; }
            set { _itemKey = value; }
        }

        public string DatabaseName
        {
            get { return _databaseName; }
            set { _databaseName = value; }
        }

        public string SubSonicName
        {
            get { return _subSonicName; }
            set { _subSonicName = value; }
        }

        public bool IsProviderNode
        {
            get { return NodeType == StageNodeType.Provider; }
        }

        public bool IsProjectNode
        {
            get { return NodeType == StageNodeType.Project; }
        }

        public bool IsConnectionStringNode
        {
            get { return NodeType == StageNodeType.ConnectionString; }
        }

        public bool IsConnectionStringFolderNode
        {
            get { return NodeType == StageNodeType.ConnectionStringFolder; }
        }

        public bool IsTableNode
        {
            get { return (NodeType == StageNodeType.Table || NodeType == StageNodeType.TableWithoutPrimaryKey || NodeType == StageNodeType.TableExcluded); }
        }

        public bool IsViewNode
        {
            get { return (NodeType == StageNodeType.View || NodeType == StageNodeType.ViewExcluded); }
        }

        public bool IsColumnNode
        {
            get { return (NodeType == StageNodeType.ColumnNode || NodeType == StageNodeType.ColumnPrimaryKeyNode); }
        }

        public bool IsPrimaryKeyColumn
        {
            get { return _isPrimaryKeyColumn; }
            set { _isPrimaryKeyColumn = value; }
        }

        public bool TableHasPrimaryKey
        {
            get { return _tableHasPrimaryKey; }
            set { _tableHasPrimaryKey = value; }
        }
    }
}