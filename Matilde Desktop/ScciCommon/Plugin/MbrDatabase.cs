using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Model;

namespace UnicodeSrl.Scci.Plugin
{

    public class MbrDatabase
        : MarshalByRefObject
    {
        public string ConnectionString { get; set; }

        public MbrDatabase(string connString)
        {
            this.ConnectionString = connString;
        }

        private long _mbminconnettivita = 0;


        internal bool ExecuteSql(string sql)
        {

            try
            {
                using (FwDataConnection cnn = new FwDataConnection(this.ConnectionString))
                {
                    cnn.Execute(sql);
                }

                return true;

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            return false;

        }

        public string FindValue(string field, string table, string where, string defValue)
        {

            string sql = "Select " + field;
            if (table != "") { sql += " From " + table; }
            if (where != "") { sql += " Where " + where; }
            string ret = "";
            DataSet ds = GetDataSet(sql);

            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                ret = dr[0].ToString();
            }
            else
            {
                ret = defValue;
            }

            return ret;

        }
        public object FindValue(string field, string table, string where)
        {

            string sql = "Select " + field;
            if (table != "") { sql += " From " + table; }
            if (where != "") { sql += " Where " + where; }
            object ret = null;
            DataSet ds = GetDataSet(sql);

            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                ret = dr[0];
            }
            else
            {
                ret = null;
            }

            return ret;

        }

        public DataSet GetDataSet(string sql)
        {
            using (FwDataConnection cnn = new FwDataConnection(this.ConnectionString))
            {
                DataSet result = cnn.Query<DataSet>(sql);
                cnn.Close();
                return result;
            }

        }

        public DataTable GetDatatable(string sql)
        {
            using (FwDataConnection cnn = new FwDataConnection(this.ConnectionString))
            {
                DataTable result = cnn.Query<DataTable>(sql);
                cnn.Close();
                return result;
            }
        }

        public bool SaveDataSet(DataSet oDs, string sql)
        {
            SqlDataObject sd = new SqlDataObject(this.ConnectionString, sql);
            sd.SaveData(oDs);
            return true;
        }

        public DataTable GetDataTableStoredProc(string storedProcedure, SqlParameterExt[] sqlParams)
        {

            FwDataParametersList fplist = new FwDataParametersList();
            fplist.FromSqlParameterExt(sqlParams);

            return GetDataTableStoredProc(storedProcedure, fplist);

        }

        public DataTable GetDataTableStoredProc(string storedProcedure, FwDataParametersList fplist)
        {
            using (FwDataConnection conn = new FwDataConnection(this.ConnectionString))
            {
                return conn.Query<DataTable>(storedProcedure, fplist, CommandType.StoredProcedure);
            }

        }

        public DataSet GetDatasetStoredProc(string storedProcedure, SqlParameterExt[] sqlParams)
        {
            using (FwDataConnection cnn = new FwDataConnection(this.ConnectionString))
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

        public object ExecStoredProc(string storedProcedure, SqlParameterExt[] sqlParams)
        {
            FwDataParametersList fplist = new FwDataParametersList();

            foreach (SqlParameterExt item in sqlParams)
            {
                fplist.Add(item.ParameterName, item.Value, item.Direction, item.DbType);
            }

            return ExecStoredProc(storedProcedure, fplist);

        }

        public object ExecStoredProc(string storedProcedure, FwDataParametersList plist)
        {
            try
            {
                using (FwDataConnection cnn = new FwDataConnection(this.ConnectionString))
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

        public DataTable GetConfigUpdater()
        {
            try
            {
                FwDataParametersList plist = new FwDataParametersList();
                return this.GetDataTableStoredProc("MSP_SelConfigUpdater", plist);
            }
            catch (Exception)
            {
                return null;
            }

        }

        public string GetConfigTable(EnumConfigTable nID)
        {

            string sRet = @"";

            try
            {

                using (FwDataConnection conn = new FwDataConnection(this.ConnectionString))
                {
                    T_ConfigRow row = conn.T_Config(nID);
                    if (row != null)
                        return row.Valore;
                    else
                        return "";
                }


            }
            catch (Exception ex)
            {
                sRet = "";
            }

            return sRet;

        }

        public Image GetConfigTableImage(EnumConfigTable nID)
        {

            Image imgRet = null;

            try
            {


                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                spcoll[0] = new SqlParameterExt("nID", (int)nID, ParameterDirection.Input, SqlDbType.Int);

                DataTable dt = this.GetDataTableStoredProc("MSP_SelConfig", spcoll);

                if (dt.Rows.Count != 0 && !dt.Rows[0].IsNull("Immagine"))
                {
                    imgRet = GetImageFromByte(dt.Rows[0]["Immagine"]);
                }

            }
            catch (Exception)
            {
                imgRet = null;
            }

            return imgRet;

        }

        public void SetConfigTable(EnumConfigTable ID, string Valore)
        {

            SetConfigTable(ID, Valore, null);

        }
        public void SetConfigTable(EnumConfigTable ID, string Valore, Image immagine)
        {

            try
            {

                SqlDataObject sd = new SqlDataObject(this.ConnectionString, @"Select * From T_Config Where ID = " + (int)ID);
                DataSet oDs = sd.GetData();
                if (oDs.Tables[0].Rows.Count == 1)
                {
                    oDs.Tables[0].Rows[0]["Valore"] = Valore;
                    if (immagine == null)
                        oDs.Tables[0].Rows[0]["Immagine"] = System.DBNull.Value;
                    else
                        oDs.Tables[0].Rows[0]["Immagine"] = GetByteFromImage(immagine);
                    sd.SaveData(oDs);
                }
                oDs.Dispose();

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        public string DataTableFindValue(DataTable table, string fieldname, string wherestring)
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

        public bool GetConnettivita()
        {

            bool bRet = true;

            try
            {

                bRet = IsNetworkAvailable(getMbMinConnettivita());

            }
            catch (Exception)
            {
                bRet = false;
            }

            return bRet;

        }

        private long getMbMinConnettivita()
        {
            long lReturn = 1000000;
            try
            {
                if (_mbminconnettivita <= 0)
                {
                    string stmp = GetConfigTable(EnumConfigTable.MbMINNetworkAvailable);
                    if (stmp != null && stmp != string.Empty && stmp.Trim() != "")
                    {
                        if (!long.TryParse(stmp, out _mbminconnettivita)) _mbminconnettivita = 0;
                    }
                }

                if (_mbminconnettivita > 0) lReturn = _mbminconnettivita;
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            return lReturn;
        }

        public bool IsNetworkAvailable(long minimumSpeed)
        {

            if (!NetworkInterface.GetIsNetworkAvailable())
                return false;

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if ((ni.OperationalStatus != OperationalStatus.Up) ||
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) ||
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Tunnel))
                    continue;

                if (ni.Speed < minimumSpeed)
                    continue;

                if (ni.Description.Equals("Microsoft Loopback Adapter", StringComparison.OrdinalIgnoreCase))
                    continue;

                return true;
            }

            return false;

        }

        public bool IsDebugEnabled()
        {
            bool bRet = false;
            string sValore = string.Empty;
            try
            {
                sValore = GetConfigTable(EnumConfigTable.Debug);

                if (sValore == "1")
                    bRet = true;
                else
                    bRet = false;

            }
            catch
            {
                bRet = false;
            }

            return bRet;

        }

        public string testoSQL(string testo)
        {
            if (testo != null)
                return testo.Replace(@"'", @"''");
            else
                return string.Empty;
        }
        public string dataOraMinutiSecondi105PerParametri(DateTime dataOra)
        {
            return dataOra.ToString(@"dd-MM-yyyy HH:mm:ssXXXff").Replace(@"/", @"-").Replace(@".", @":").Replace(@"XXX", @".");
        }

        public string dataOra105PerParametri(DateTime dataOra)
        {
            return dataOra.ToString(@"dd-MM-yyyy HH:mm").Replace(@"/", @"-").Replace(@".", @":");
        }

        public string data105PerParametri(DateTime data)
        {
            return data.ToString(@"dd-MM-yyyy").Replace(@"/", @"-");
        }

        public string dataPerRowFilter(DateTime data)
        {
            return @"#" + data.ToString(@"MM/dd/yyyy HH:mm:ss").Replace(@"-", @"/").Replace(@".", @":") + @"#";
        }

        public string CheckStringNull(string vValue, string vRetValueIfDBNull)
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

        public byte[] GetByteFromImage(Image image)
        {

            try
            {

                MemoryStream stream = new MemoryStream();
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                byte[] pic = stream.ToArray();
                return pic;

            }
            catch (Exception)
            {
                return null;
            }

        }
        public Image GetImageFromByte(object dc)
        {

            try
            {

                Byte[] data = new Byte[0];
                data = (Byte[])(dc);
                MemoryStream mem = new MemoryStream(data);
                return Image.FromStream(mem);

            }
            catch (Exception)
            {
                return null;
            }

        }

    }
}
