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
#if ALLPROVIDERS
using System;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace SubSonic
{
    /// <summary>
    /// Summary for the ELib3DataProvider class
    /// </summary>
    public class ELib3DataProvider : SqlDataProvider
    {
        #region Query Execution overrides

        /// <summary>
        /// Gets the reader.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public override IDataReader GetReader(QueryCommand qry)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand cmd = qry.CommandType == CommandType.Text ? db.GetSqlStringCommand(qry.CommandSql) : db.GetStoredProcCommand(qry.CommandSql);

            foreach(QueryParameter param in qry.Parameters)
                db.AddInParameter(cmd, param.ParameterName, param.DataType, param.ParameterValue);

            return db.ExecuteReader(cmd);
        }

        /// <summary>
        /// Gets the data set command.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        private static DbCommand GetDataSetCommand(Database db, QueryCommand qry)
        {
            DbCommand cmd = qry.CommandType == CommandType.Text ? db.GetSqlStringCommand(qry.CommandSql) : db.GetStoredProcCommand(qry.CommandSql);

            foreach(QueryParameter param in qry.Parameters)
                db.AddInParameter(cmd, param.ParameterName, param.DataType, param.ParameterValue);

            return cmd;
        }

        /// <summary>
        /// Gets the data set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public override T GetDataSet<T>(QueryCommand qry)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand cmd = GetDataSetCommand(db, qry);

            T ds;
            if(typeof(T) == typeof(DataSet)) // LoadDataSet doesn't accept empty table names.
                ds = (T)db.ExecuteDataSet(cmd);
            else
            {
                ds = new T();
                db.LoadDataSet(cmd, ds, GetTableNames(ds));
            }
            return ds;
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public override object ExecuteScalar(QueryCommand qry)
        {
            Database db = DatabaseFactory.CreateDatabase();

            DbCommand cmd = qry.CommandType == CommandType.Text ? db.GetSqlStringCommand(qry.CommandSql) : db.GetStoredProcCommand(qry.CommandSql);
            cmd.CommandType = qry.CommandType;

            foreach(QueryParameter param in qry.Parameters)
                db.AddInParameter(cmd, param.ParameterName, param.DataType, param.ParameterValue);

            return db.ExecuteScalar(cmd);
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public override int ExecuteQuery(QueryCommand qry)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand cmd = qry.CommandType == CommandType.Text ? db.GetSqlStringCommand(qry.CommandSql) : db.GetStoredProcCommand(qry.CommandSql);

            cmd.CommandType = qry.CommandType;

            foreach(QueryParameter param in qry.Parameters)
                db.AddInParameter(cmd, param.ParameterName, param.DataType, param.ParameterValue);

            return db.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Executes a transaction using the passed-commands
        /// </summary>
        /// <param name="commands"></param>
        public override void ExecuteTransaction(QueryCommandCollection commands)
        {
            Database db = DatabaseFactory.CreateDatabase();

            //make sure we have at least one
            if(commands.Count > 0)
            {
                DbCommand cmd = null;

                //a using statement will make sure we close off the connection
                using(DbConnection conn = db.CreateConnection())
                {
                    //open up the connection and start the transaction
                    conn.Open();

                    DbTransaction trans = conn.BeginTransaction();

                    foreach(QueryCommand qry in commands)
                    {
                        cmd = qry.CommandType == CommandType.Text ? db.GetSqlStringCommand(qry.CommandSql) : db.GetStoredProcCommand(qry.CommandSql);

                        try
                        {
                            foreach(QueryParameter param in qry.Parameters)
                                db.AddInParameter(cmd, param.ParameterName, param.DataType, param.ParameterValue);

                            db.ExecuteNonQuery(cmd, trans);
                        }
                        catch(DbException x)
                        {
                            //if there's an error, roll everything back
                            trans.Rollback();

                            //clean up
                            conn.Close();
                            cmd.Dispose();

                            //throw the error retaining the stack.
                            throw new Exception(x.Message);
                        }
                    }
                    //if we get to this point, we're good to go
                    trans.Commit();

                    //close off the connection
                    conn.Close();
                    if(cmd != null)
                        cmd.Dispose();
                }
            }
            else
                throw new Exception("No commands present");
        }

        #endregion


        #region Command Builder Overrides

        /// <summary>
        /// Gets the db command.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public override DbCommand GetDbCommand(QueryCommand qry)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand cmd = qry.CommandType == CommandType.StoredProcedure ? db.GetStoredProcCommand(qry.CommandSql) : db.GetSqlStringCommand(qry.CommandSql);

            foreach(QueryParameter par in qry.Parameters)
                db.AddInParameter(cmd, par.ParameterName, par.DataType, par.ParameterValue);

            return cmd;
        }

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public override IDbCommand GetCommand(QueryCommand qry)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand cmd = db.GetSqlStringCommand(qry.CommandSql);
            foreach(QueryParameter param in qry.Parameters)
                db.AddInParameter(cmd, param.ParameterName, param.DataType, param.ParameterValue);

            return cmd;
        }

        #endregion
    }

    /// <summary>
    /// This is class is for backwards compatibility only. Please update your references to ELib3DataProvider
    /// </summary>
    public class ELib2DataProvider : ELib3DataProvider {}
}

#endif