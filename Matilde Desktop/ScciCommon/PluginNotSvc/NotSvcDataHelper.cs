using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.Scci.PluginNotSvc
{
    public static class NotSvcDataHelper
    {
        public static DataSet GetDataset(string connectionstring, string sql)
        {
            try
            {
                using (FwDataConnection cnn = new FwDataConnection(connectionstring))
                {
                    DataSet result = cnn.Query<DataSet>(sql);
                    cnn.Close();
                    return result;
                }

            }
            catch (Exception ex)
            {
                Exception exception = new Exception("Errore GetDataset 1", ex);
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(exception);
                return null;
            }

        }

        public static DataSet GetDatasetStoredProc(string connectionString, string storedProcedure, SqlParameterExt[] sqlParams)
        {
            try
            {
                using (FwDataConnection cnn = new FwDataConnection(connectionString))
                {
                    if (sqlParams == null)
                        sqlParams = new SqlParameterExt[0];

                    FwDataParametersList list = new FwDataParametersList();
                    list.FromSqlParameterExt(sqlParams);

                    DataSet result = cnn.Query<DataSet>(storedProcedure, list, CommandType.StoredProcedure);
                    cnn.Close();
                    return result;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo("Errore GetDataset 2:" + Environment.NewLine + ex.Message);
                throw (ex);
            }
        }

        public static DataSet GetDatasetStoredProc(string storedProcedure, SqlParameterExt[] sqlParams)
        {
            try
            {
                using (FwDataConnection cnn = new FwDataConnection(Database.ConnectionString))
                {
                    if (sqlParams == null)
                        sqlParams = new SqlParameterExt[0];

                    FwDataParametersList list = new FwDataParametersList();
                    list.FromSqlParameterExt(sqlParams);

                    DataSet result = cnn.Query<DataSet>(storedProcedure, list, CommandType.StoredProcedure);
                    cnn.Close();
                    return result;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo("Errore GetDataset 3:" + Environment.NewLine + ex.Message);
                throw (ex);
            }
        }

        public static DataTable GetDataTable(string connectionstring, string sql)
        {
            try
            {
                using (FwDataConnection cnn = new FwDataConnection(connectionstring))
                {
                    DataTable result = cnn.Query<DataTable>(sql);
                    cnn.Close();
                    return result;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo("Errore GetDataTable :" + Environment.NewLine + ex.Message);
                return null;
            }

        }


        [Obsolete("Solo per migrazione")]

        public static DataTable GetDataTableStoredProc(string storedProcedure, SqlParameterExt[] sqlParams)
        {
            FwDataParametersList fplist = new FwDataParametersList();

            foreach (SqlParameterExt item in sqlParams)
            {
                fplist.Add(item.ParameterName, item.Value, item.Direction, item.DbType);
            }

            return GetDataTableStoredProc(storedProcedure, fplist);

        }

        public static DataTable GetDataTableStoredProc(string storedProcedure, FwDataParametersList fplist)
        {
            using (FwDataConnection conn = new FwDataConnection(Database.ConnectionString))
            {
                return conn.Query<DataTable>(storedProcedure, fplist, CommandType.StoredProcedure);
            }

        }

        public static DataTable GetDataTableStoredProc(string connectionstring, string storedProcedure, SqlParameterExt[] sqlParams)
        {
            FwDataParametersList fplist = new FwDataParametersList();
            fplist.FromSqlParameterExt(sqlParams);

            using (FwDataConnection conn = new FwDataConnection(connectionstring))
            {
                return conn.Query<DataTable>(storedProcedure, fplist, CommandType.StoredProcedure);
            }

        }

        [Obsolete("Solo per migrazione, usare FwDataParametersList nei parametri")]
        public static object ExecStoredProc(string storedProcedure, SqlParameterExt[] plist)
        {
            FwDataParametersList fplist = new FwDataParametersList();
            fplist.FromSqlParameterExt(plist);

            return ExecStoredProc(storedProcedure, fplist);
        }

        [Obsolete("Solo per migrazione, usare FwDataParametersList nei parametri")]
        public static object ExecStoredProc(string connectionstring, string storedProcedure, SqlParameterExt[] plist)
        {
            FwDataParametersList fplist = new FwDataParametersList();
            fplist.FromSqlParameterExt(plist);

            return ExecStoredProc(connectionstring, storedProcedure, fplist);

        }

        public static object ExecStoredProc(string storedProcedure, FwDataParametersList plist)
        {
            try
            {
                using (FwDataConnection cnn = new FwDataConnection(Database.ConnectionString))
                {
                    plist.Add("RetValue", null, ParameterDirection.ReturnValue);
                    cnn.ExecuteStored(storedProcedure, ref plist);

                    FwDataParameter ret = plist.FirstOrDefault(p => p.ParameterName == "RetValue");
                    if (ret != null)
                        return ret.Value;

                    cnn.Close();

                    return null;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo("Errore ExecStoredProc :" + Environment.NewLine + ex.Message);
                return null;
            }

        }

        public static object ExecStoredProc(string connectionString, string storedProcedure, FwDataParametersList plist)
        {
            try
            {
                using (FwDataConnection cnn = new FwDataConnection(connectionString))
                {
                    plist.Add("RetValue", null, ParameterDirection.ReturnValue);
                    cnn.ExecuteStored(storedProcedure, ref plist);

                    FwDataParameter ret = plist.FirstOrDefault(p => p.ParameterName == "RetValue");
                    if (ret != null)
                        return ret.Value;

                    cnn.Close();

                    return null;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo("Errore ExecStoredProc :" + Environment.NewLine + ex.Message);
                return null;
            }

        }

        public static void ExecuteSQL(string connectionstring, string sql)
        {
            try
            {
                using (FwDataConnection cnn = new FwDataConnection(connectionstring))
                {
                    cnn.Execute(sql);

                    cnn.Close();
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo("Errore ExecStoredProc :" + Environment.NewLine + ex.Message);
            }

        }

        public static void SaveDataSet(DataSet oDs, string connectionstring, string sql)
        {
            SqlDataObject sd = new SqlDataObject(connectionstring, sql);
            sd.SaveData(oDs);
        }

        public static string FindValue(string connectionstring, string field, string table, string where, string defValue)
        {

            string sql = "Select " + field;
            if (table != "") { sql += " From " + table; }
            if (where != "") { sql += " Where " + where; }
            string ret = "";
            DataTable dt = GetDataTable(connectionstring, sql);

            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                ret = dr[0].ToString();
            }
            else
            {
                ret = defValue;
            }

            return ret;

        }

        public static string DataTableFindValue(DataTable table, string fieldname, string wherestring)
        {
            string sret = null;
            DataRow[] rowfilter = null;

            try
            {

                rowfilter = table.Select(wherestring);

                if (rowfilter != null && rowfilter.Length > 0)
                    sret = rowfilter[0][fieldname].ToString();
                else
                    sret = string.Empty;
            }
            catch (Exception)
            {
                sret = string.Empty;
            }

            return sret;

        }

        public static string testoSQL(string testo)
        {
            if (testo != null)
                return testo.Replace(@"'", @"''");
            else
                return string.Empty;
        }
        public static string dataOraMinutiSecondi105PerParametri(DateTime dataOra)
        {
            return dataOra.ToString(@"dd-MM-yyyy HH:mm:ssXXXff").Replace(@"/", @"-").Replace(@".", @":").Replace(@"XXX", @".");
        }

        public static string dataOra105PerParametri(DateTime dataOra)
        {
            return dataOra.ToString(@"dd-MM-yyyy HH:mm").Replace(@"/", @"-").Replace(@".", @":");
        }

        public static string data105PerParametri(DateTime data)
        {
            return data.ToString(@"dd-MM-yyyy").Replace(@"/", @"-");
        }

        public static string dataPerRowFilter(DateTime data)
        {
            return @"#" + data.ToString(@"MM/dd/yyyy HH:mm:ss").Replace(@"-", @"/").Replace(@".", @":") + @"#";
        }

        public static string CheckStringNull(string vValue, string vRetValueIfDBNull)
        {
            if (System.String.IsNullOrEmpty(vValue))
            {
                return vRetValueIfDBNull;
            }
            else
            {
                return vValue;
            }
        }

    }
}
