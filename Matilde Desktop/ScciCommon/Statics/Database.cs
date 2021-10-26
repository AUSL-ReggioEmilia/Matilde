using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.Enums;
using System.Net.NetworkInformation;
using System.Xml.Serialization;
using System.IO;
using UnicodeSrl.Scci.Model;

namespace UnicodeSrl.Scci.Statics
{
    public static class Database
    {

        private static long _mbminconnettivita = 0;
        private static string m_ConnectionString = null;

        public static string ConnectionString
        {
            get { return m_ConnectionString; }
            set
            {
                bool bChanged = (value != m_ConnectionString);

                if (bChanged)
                {
                    m_ConnectionString = value;
                }

            }
        }

        public static bool ExecuteSql(string sql)
        {
            bool bRet = false;

            try
            {
                SqlServerCmdMT cmd = new SqlServerCmdMT(Database.ConnectionString, sql, System.Threading.ThreadPriority.Normal, false, "");
                cmd.Execute();
                cmd.Wait();

                bRet = (cmd.LastException == null);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }


            return bRet;

        }

        public static string FindValue(string connString, string field, string table, string where, string defValue)
        {

            string sql = "Select " + field;
            if (table != "") { sql += " From " + table; }
            if (where != "") { sql += " Where " + where; }

            using (FwDataConnection fdc = new FwDataConnection(connString))
            {
                Dictionary<string, object> result = fdc.QueryFirst(sql);

                if (result != null)
                    return Convert.ToString(result.First().Value);
                else
                    return defValue;
            }

        }

        public static string FindValue(string field, string table, string where, string defValue)
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
        internal static object FindValue(string field, string table, string where)
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

        public static DataSet GetDataSet(string sql)
        {
            try
            {
                using (FwDataConnection cnn = new FwDataConnection(Database.ConnectionString))
                {
                    DataSet result = cnn.Query<DataSet>(sql);
                    cnn.Close();
                    return result;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo("Errore GetDataset:" + Environment.NewLine + ex.Message);
                throw (ex);
            }

        }

        public static DataTable GetDatatable(string sql)
        {
            try
            {
                using (FwDataConnection cnn = new FwDataConnection(Database.ConnectionString))
                {
                    DataTable result = cnn.Query<DataTable>(sql);
                    cnn.Close();
                    return result;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo("Errore GetDataTable :" + Environment.NewLine + ex.Message);
                throw (ex);
            }


        }

        public static bool SaveDataSet(DataSet oDs, string sql)
        {
            try
            {
                SqlDataObject sd = new SqlDataObject(Database.ConnectionString, sql);
                sd.SaveData(oDs);
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo("Errore SaveDataSet :" + Environment.NewLine + ex.Message);
                throw (ex);
            }

            return true;

        }

        public static DataTable GetDataTableStoredProc(string storedProcedure, SqlParameterExt[] sqlParams)
        {
            FwDataParametersList fplist = new FwDataParametersList();
            fplist.FromSqlParameterExt(sqlParams);

            return GetDataTableStoredProc(storedProcedure, fplist);

        }

        public static DataTable GetDataTableStoredProc(string storedProcedure, FwDataParametersList fplist)
        {
            using (FwDataConnection conn = new FwDataConnection(Database.ConnectionString))
            {
                return conn.Query<DataTable>(storedProcedure, fplist, CommandType.StoredProcedure);
            }

        }

        public static FwDataParametersList GetFwDataParametersList(Parametri op)
        {
            return new FwDataParametersList
            {
                {
                    "xParametri", XmlProcs.XmlSerializeToString(op), ParameterDirection.Input, DbType.Xml
                }
            };
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
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo("Errore GetDataset:" + Environment.NewLine + ex.Message);
                throw (ex);
            }
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

        [Obsolete("Solo per migrazione, usare FwDataParametersList nei parametri")]
        public static object ExecStoredProc(string storedProcedure, SqlParameterExt[] plist)
        {
            FwDataParametersList fplist = new FwDataParametersList();

            foreach (SqlParameterExt item in plist)
            {
                fplist.Add(item.ParameterName, item.Value, item.Direction, item.DbType);
            }

            return ExecStoredProc(storedProcedure, fplist);
        }

        public static List<FwDataParameter> ExecStoredProcOutput(string storedProcedure, FwDataParametersList plist)
        {
            try
            {
                using (FwDataConnection cnn = new FwDataConnection(Database.ConnectionString))
                {
                    plist.Add("RetValue", null, ParameterDirection.ReturnValue);
                    cnn.ExecuteStored(storedProcedure, ref plist);

                    List<FwDataParameter> ret = plist.FindAll(p => p.Direction == ParameterDirection.Output);
                    if (ret != null)
                        return ret;

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

        public static DataTable GetConfigUpdater()
        {
            try
            {
                FwDataParametersList list = new FwDataParametersList();
                return Database.GetDataTableStoredProc("MSP_SelConfigUpdater", list);
            }
            catch (Exception)
            {
                return null;
            }

        }


        public static string GetConfigTable(EnumConfigTable nID)
        {

            string sRet = @"";

            try
            {

                using (FwDataConnection conn = new FwDataConnection(Database.ConnectionString))
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

        public static Image GetConfigTableImage(EnumConfigTable nID)
        {

            Image imgRet = null;

            try
            {


                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                spcoll[0] = new SqlParameterExt("nID", (int)nID, ParameterDirection.Input, SqlDbType.Int);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelConfig", spcoll);

                if (dt.Rows.Count != 0 && !dt.Rows[0].IsNull("Immagine"))
                {
                    imgRet = DrawingProcs.GetImageFromByte(dt.Rows[0]["Immagine"]);
                }

            }
            catch (Exception)
            {
                imgRet = null;
            }

            return imgRet;

        }

        public static void SetConfigTable(EnumConfigTable ID, string Valore)
        {

            SetConfigTable(ID, Valore, null);

        }
        public static void SetConfigTable(EnumConfigTable ID, string Valore, Image immagine)
        {

            try
            {

                SqlDataObject sd = new SqlDataObject(Database.ConnectionString, @"Select * From T_Config Where ID = " + (int)ID);
                DataSet oDs = sd.GetData();
                if (oDs.Tables[0].Rows.Count == 1)
                {
                    oDs.Tables[0].Rows[0]["Valore"] = Valore;
                    if (immagine == null)
                        oDs.Tables[0].Rows[0]["Immagine"] = System.DBNull.Value;
                    else
                        oDs.Tables[0].Rows[0]["Immagine"] = DrawingProcs.GetByteFromImage(immagine);
                    sd.SaveData(oDs);
                }
                oDs.Dispose();

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        public static void SetConfigPCTable(string codPC, ConfigPC configPC)
        {
            try
            {
                string sXML = "";

                sXML = "";
                if (configPC != null && configPC.configDebug != null)
                {
                    sXML = configPC.configDebug.getXML();
                }
                SqlDataObject sd = new SqlDataObject(Database.ConnectionString, @"Select * From T_ConfigPC Where CodPC = '" + testoSQL(codPC) + @"' And Codice = '" + testoSQL(ConfigPC.SECTION_DEBUG) + @"'");
                DataSet oDs = sd.GetData();
                if (oDs.Tables[0].Rows.Count == 1)
                {
                    oDs.Tables[0].Rows[0]["Valore"] = sXML;
                }
                else
                {
                    DataRow newrow = oDs.Tables[0].NewRow();

                    newrow["CodPC"] = codPC;
                    newrow["Codice"] = ConfigPC.SECTION_DEBUG;
                    newrow["Descrizione"] = "Configurazione " + codPC;
                    newrow["Valore"] = sXML;

                    oDs.Tables[0].Rows.Add(newrow);

                }
                sd.SaveData(oDs);
                oDs.Dispose();

                sXML = "";
                if (configPC != null && configPC.configRtf != null)
                {
                    sXML = configPC.configRtf.getXML();
                }
                sd = new SqlDataObject(Database.ConnectionString, @"Select * From T_ConfigPC Where CodPC = '" + testoSQL(codPC) + @"' And Codice = '" + testoSQL(ConfigPC.SECTION_RTF) + @"'");
                oDs = sd.GetData();
                if (oDs.Tables[0].Rows.Count == 1)
                {
                    oDs.Tables[0].Rows[0]["Valore"] = sXML;
                }
                else
                {
                    DataRow newrow = oDs.Tables[0].NewRow();

                    newrow["CodPC"] = codPC;
                    newrow["Codice"] = ConfigPC.SECTION_RTF;
                    newrow["Descrizione"] = "Configurazione " + codPC;
                    newrow["Valore"] = sXML;

                    oDs.Tables[0].Rows.Add(newrow);

                }
                sd.SaveData(oDs);
                oDs.Dispose();


                sXML = "";
                if (configPC != null && configPC.configFont != null)
                {
                    sXML = configPC.configFont.getXML();
                }
                sd = new SqlDataObject(Database.ConnectionString, @"Select * From T_ConfigPC Where CodPC = '" + testoSQL(codPC) + @"' And Codice = '" + testoSQL(ConfigPC.SECTION_FONT) + @"'");
                oDs = sd.GetData();
                if (oDs.Tables[0].Rows.Count == 1)
                {
                    oDs.Tables[0].Rows[0]["Valore"] = sXML;
                }
                else
                {
                    DataRow newrow = oDs.Tables[0].NewRow();

                    newrow["CodPC"] = codPC;
                    newrow["Codice"] = ConfigPC.SECTION_FONT;
                    newrow["Descrizione"] = "Configurazione " + codPC;
                    newrow["Valore"] = sXML;

                    oDs.Tables[0].Rows.Add(newrow);

                }
                sd.SaveData(oDs);
                oDs.Dispose();


                sXML = "";
                if (configPC != null && configPC.configChiamataNumeri != null)
                {
                    sXML = configPC.configChiamataNumeri.getXML();
                }
                sd = new SqlDataObject(Database.ConnectionString, @"Select * From T_ConfigPC Where CodPC = '" + testoSQL(codPC) + @"' And Codice = '" + testoSQL(ConfigPC.SECTION_CHIAMATA_NUM) + @"'");
                oDs = sd.GetData();
                if (oDs.Tables[0].Rows.Count == 1)
                {
                    oDs.Tables[0].Rows[0]["Valore"] = sXML;
                }
                else
                {
                    DataRow newrow = oDs.Tables[0].NewRow();

                    newrow["CodPC"] = codPC;
                    newrow["Codice"] = ConfigPC.SECTION_CHIAMATA_NUM;
                    newrow["Descrizione"] = "Configurazione " + codPC;
                    newrow["Valore"] = sXML;

                    oDs.Tables[0].Rows.Add(newrow);

                }
                sd.SaveData(oDs);
                oDs.Dispose();

                sXML = "";
                if (configPC != null && configPC.configIpovedente != null)
                {
                    sXML = configPC.configIpovedente.getXML();
                }
                sd = new SqlDataObject(Database.ConnectionString, @"Select * From T_ConfigPC Where CodPC = '" + testoSQL(codPC) + @"' And Codice = '" + testoSQL(ConfigPC.SECTION_IPOVEDENTE) + @"'");
                oDs = sd.GetData();
                if (oDs.Tables[0].Rows.Count == 1)
                {
                    oDs.Tables[0].Rows[0]["Valore"] = sXML;
                }
                else
                {
                    DataRow newrow = oDs.Tables[0].NewRow();

                    newrow["CodPC"] = codPC;
                    newrow["Codice"] = ConfigPC.SECTION_IPOVEDENTE;
                    newrow["Descrizione"] = "Configurazione " + codPC;
                    newrow["Valore"] = sXML;

                    oDs.Tables[0].Rows.Add(newrow);

                }
                sd.SaveData(oDs);
                oDs.Dispose();
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        public static ConfigPC GetConfigPCTable(string codPC)
        {
            ConfigPC oRet = new ConfigPC();
            try
            {
                string sXML = "";

                SqlDataObject sd = new SqlDataObject(Database.ConnectionString, @"Select * From T_ConfigPC Where CodPC = '" + testoSQL(codPC) + @"'");
                DataSet oDs = sd.GetData();

                oDs.Tables[0].DefaultView.RowFilter = @"Codice = '" + testoSQL(ConfigPC.SECTION_DEBUG) + @"'";
                if (oDs.Tables[0].DefaultView.Count > 0 && !oDs.Tables[0].DefaultView[0].Row.IsNull("Valore") && oDs.Tables[0].DefaultView[0]["Valore"].ToString().Trim() != "")
                {
                    sXML = oDs.Tables[0].DefaultView[0]["Valore"].ToString();
                    XmlSerializer mySerializer = new XmlSerializer(oRet.configDebug.GetType());
                    MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(sXML));
                    try
                    {
                        oRet.configDebug = (ConfigDebug)mySerializer.Deserialize(ms);
                    }
                    catch (Exception)
                    {
                    }
                }
                oDs.Tables[0].DefaultView.RowFilter = @"";

                oDs.Tables[0].DefaultView.RowFilter = @"Codice = '" + testoSQL(ConfigPC.SECTION_RTF) + @"'";
                if (oDs.Tables[0].DefaultView.Count > 0 && !oDs.Tables[0].DefaultView[0].Row.IsNull("Valore") && oDs.Tables[0].DefaultView[0]["Valore"].ToString().Trim() != "")
                {
                    sXML = oDs.Tables[0].DefaultView[0]["Valore"].ToString();
                    XmlSerializer mySerializer = new XmlSerializer(oRet.configRtf.GetType());
                    MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(sXML));
                    try
                    {
                        oRet.configRtf = (ConfigRtf)mySerializer.Deserialize(ms);
                    }
                    catch (Exception)
                    {
                    }
                }
                oDs.Tables[0].DefaultView.RowFilter = @"";

                oDs.Tables[0].DefaultView.RowFilter = @"Codice = '" + testoSQL(ConfigPC.SECTION_FONT) + @"'";
                if (oDs.Tables[0].DefaultView.Count > 0 && !oDs.Tables[0].DefaultView[0].Row.IsNull("Valore") && oDs.Tables[0].DefaultView[0]["Valore"].ToString().Trim() != "")
                {
                    sXML = oDs.Tables[0].DefaultView[0]["Valore"].ToString();
                    XmlSerializer mySerializer = new XmlSerializer(oRet.configFont.GetType());
                    MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(sXML));
                    try
                    {
                        oRet.configFont = (ConfigFont)mySerializer.Deserialize(ms);
                    }
                    catch (Exception)
                    {
                    }
                }
                oDs.Tables[0].DefaultView.RowFilter = @"";

                oDs.Tables[0].DefaultView.RowFilter = @"Codice = '" + testoSQL(ConfigPC.SECTION_CHIAMATA_NUM) + @"'";
                if (oDs.Tables[0].DefaultView.Count > 0 && !oDs.Tables[0].DefaultView[0].Row.IsNull("Valore") && oDs.Tables[0].DefaultView[0]["Valore"].ToString().Trim() != "")
                {
                    sXML = oDs.Tables[0].DefaultView[0]["Valore"].ToString();
                    XmlSerializer mySerializer = new XmlSerializer(oRet.configChiamataNumeri.GetType());
                    MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(sXML));
                    try
                    {
                        oRet.configChiamataNumeri = (ConfigChiamataNumeri)mySerializer.Deserialize(ms);
                    }
                    catch (Exception)
                    {
                    }
                }
                oDs.Tables[0].DefaultView.RowFilter = @"";

                oDs.Tables[0].DefaultView.RowFilter = @"Codice = '" + testoSQL(ConfigPC.SECTION_IPOVEDENTE) + @"'";
                if (oDs.Tables[0].DefaultView.Count > 0 && !oDs.Tables[0].DefaultView[0].Row.IsNull("Valore") && oDs.Tables[0].DefaultView[0]["Valore"].ToString().Trim() != "")
                {
                    sXML = oDs.Tables[0].DefaultView[0]["Valore"].ToString();
                    XmlSerializer mySerializer = new XmlSerializer(oRet.configIpovedente.GetType());
                    MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(sXML));
                    try
                    {
                        oRet.configIpovedente = (ConfigIpovedente)mySerializer.Deserialize(ms);
                    }
                    catch (Exception)
                    {
                    }
                }
                oDs.Tables[0].DefaultView.RowFilter = @"";

                oDs.Dispose();

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            return oRet;
        }

        public static string GetConfigCETable(EnumConfigCETable nID)
        {

            string sRet = @"";

            try
            {

                using (FwDataConnection conn = new FwDataConnection(Database.ConnectionString))
                {
                    DataTable dt = conn.Query<DataTable>($"Select Valore From T_ConfigCE Where ID = {Convert.ToInt32(nID)}");
                    if (dt.Rows.Count != 0 && !dt.Rows[0].IsNull(0))
                    {
                        sRet = dt.Rows[0][0].ToString();
                    }
                }

            }
            catch (Exception)
            {
                sRet = "";
            }

            return sRet;

        }

        public static Image GetConfigCETableImage(EnumConfigCETable nID)
        {

            Image imgRet = null;

            try
            {

                using (FwDataConnection conn = new FwDataConnection(Database.ConnectionString))
                {
                    DataTable dt = conn.Query<DataTable>($"Select Immagine From T_ConfigCE Where ID = {Convert.ToInt32(nID)}");
                    if (dt.Rows.Count != 0 && !dt.Rows[0].IsNull(0))
                    {
                        imgRet = DrawingProcs.GetImageFromByte(dt.Rows[0][0]);
                    }
                }

            }
            catch (Exception)
            {
                imgRet = null;
            }

            return imgRet;

        }

        public static void SetConfigCETable(EnumConfigCETable ID, string Valore)
        {

            SetConfigCETable(ID, Valore, null);

        }
        public static void SetConfigCETable(EnumConfigCETable ID, string Valore, Image immagine)
        {

            try
            {

                SqlDataObject sd = new SqlDataObject(Database.ConnectionString, @"Select * From T_ConfigCE Where ID = " + (int)ID);
                DataSet oDs = sd.GetData();
                if (oDs.Tables[0].Rows.Count == 1)
                {
                    oDs.Tables[0].Rows[0]["Valore"] = Valore;
                    if (immagine == null)
                        oDs.Tables[0].Rows[0]["Immagine"] = System.DBNull.Value;
                    else
                        oDs.Tables[0].Rows[0]["Immagine"] = DrawingProcs.GetByteFromImage(immagine);
                    sd.SaveData(oDs);
                }
                oDs.Dispose();

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        public static string DataTableFindValue(DataTable table, string fieldname, string wherestring)
        {
            string sret = null;
            DataRow[] rowfilter = null;

            try
            {

                rowfilter = table.Select(wherestring);

                if (rowfilter != null && rowfilter.Count() > 0)
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

        public static bool GetConnettivita()
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

        public static bool GetConnettivitaDB()
        {

            bool bRet = false;

            try
            {

                SqlDataObject sd = new SqlDataObject
                {
                    ConnectionString = Database.ConnectionString + @";Connect Timeout=2"
                };

                bRet = sd.IsValidConnection;

            }
            catch (Exception)
            {
                bRet = false;
            }

            return bRet;

        }

        private static long getMbMinConnettivita()
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

        public static bool IsNetworkAvailable(long minimumSpeed)
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

                if ((ni.Description.IndexOf("VMware Network Adapter", StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (ni.Name.IndexOf("VMware Network Adapter", StringComparison.OrdinalIgnoreCase) >= 0))
                    continue;

                if (ni.Description.Equals("Microsoft Loopback Adapter", StringComparison.OrdinalIgnoreCase))
                    continue;

                return true;
            }

            return false;

        }

        public static bool IsDebugEnabled()
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

        public static bool LineeGiornaliereAbilitate(DateTime dataDa, DateTime dataA)
        {
            bool bLineeGiornaliereAbilitate = false;
            try
            {

                if (!int.TryParse(GetConfigTable(Scci.Enums.EnumConfigTable.GraficiMaxGiorniTickmark), out int iMaxDays))
                    iMaxDays = 0;

                if (dataDa > DateTime.MinValue && dataA > DateTime.MinValue)
                {
                    bLineeGiornaliereAbilitate = (iMaxDays > 0 && dataA.Subtract(dataDa).Days <= iMaxDays);
                }
                else
                {
                    bLineeGiornaliereAbilitate = (iMaxDays > 0);
                }
            }
            catch
            {
                bLineeGiornaliereAbilitate = false;
            }

            return bLineeGiornaliereAbilitate;

        }

        public static string GetNewID()
        {

            string ret = string.Empty;
            DataSet ds = null;
            try
            {
                ds = Database.GetDataSet("SELECT NEWID() ");
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    ret = dr[0].ToString();
                }

            }
            catch (Exception ex)
            {
                Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            finally
            {
                if (ds != null)
                {
                    ds.Dispose();
                    ds = null;
                }
            }


            return ret;
        }

        public static string testoSQL(string testo)
        {
            if (testo != null)
                return testo.Replace(@"'", @"''");
            else
                return string.Empty;
        }

        public static string dataOraSQL(DateTime dataora)
        {
            return @"CONVERT (DateTime, '" + dataOra105PerParametri(dataora) + @"', 105)";
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
