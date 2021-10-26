using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.SqlClient;
using UnicodeSrl.Framework.Data;
using Infragistics.Win.UltraWinGrid;
using System.Collections;
using UnicodeSrl.Scci.RTF;
using UnicodeSrl.DatiClinici.DC;
using UnicodeSrl.DatiClinici.Common.Enums;
using UnicodeSrl.ScciCommon2.DataModel;
using UnicodeSrl.DatiClinici.Common;
using System.Windows.Forms;

namespace UnicodeSrl.ScciManagement
{
    public partial class DataBase
    {

        #region Accesso al database

        internal static bool ExecuteSql(string sql)
        {

            bool bRet = false;
            SqlConnection oSqlConn = null;
            SqlCommand oCmd = null;

            try
            {
                oSqlConn = new SqlConnection(MyStatics.Configurazione.ConnectionString);
                oSqlConn.Open();
                oCmd = new SqlCommand(sql, oSqlConn);
                oCmd.CommandTimeout = 60;
                oCmd.CommandType = CommandType.Text;

                oCmd.ExecuteReader();

                bRet = true;
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            finally
            {
                if (oCmd != null)
                {
                    oCmd.Dispose();
                    oCmd = null;
                }
                if (oSqlConn != null)
                {
                    if (oSqlConn.State == ConnectionState.Open) oSqlConn.Close();
                    oSqlConn.Dispose();
                    oSqlConn = null;
                }
            }

            return bRet;

        }
        internal static bool ExecuteSql(string sql, string connectionstring)
        {

            bool bRet = false;
            SqlConnection oSqlConn = null;
            SqlCommand oCmd = null;

            try
            {
                oSqlConn = new SqlConnection(connectionstring);
                oSqlConn.Open();
                oCmd = new SqlCommand(sql, oSqlConn);
                oCmd.CommandTimeout = 60;
                oCmd.CommandType = CommandType.Text;

                oCmd.ExecuteReader();

                bRet = true;
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            finally
            {
                if (oCmd != null)
                {
                    oCmd.Dispose();
                    oCmd = null;
                }
                if (oSqlConn != null)
                {
                    if (oSqlConn.State == ConnectionState.Open) oSqlConn.Close();
                    oSqlConn.Dispose();
                    oSqlConn = null;
                }
            }

            return bRet;

        }

        internal static string FindValue(string field, string table, string where, string defValue)
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

        internal static DataSet GetDataSet(string sql)
        {

            DataSet dsRet = null;

            try
            {
                SqlDataObject sd = new SqlDataObject(MyStatics.Configurazione.ConnectionString, sql);
                try
                {
                    dsRet = sd.GetData();
                }
                catch (Exception)
                {
                    dsRet = sd.GetData(MissingSchemaAction.Add);
                }
                sd.Dispose();
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            return dsRet;
        }
        internal static DataSet GetDataSet(string sql, string connectionstring)
        {

            DataSet dsRet = null;

            try
            {
                SqlDataObject sd = new SqlDataObject(connectionstring, sql);
                try
                {
                    dsRet = sd.GetData();
                }
                catch (Exception)
                {
                    dsRet = sd.GetData(MissingSchemaAction.Add);
                }
                sd.Dispose();
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            return dsRet;
        }

        internal static DataTable GetDataTable(string sql)
        {
            return DataBase.GetDataTable(sql, MyStatics.Configurazione.ConnectionString);
        }
        internal static DataTable GetDataTable(string sql, string connectionstring)
        {
            SqlConnection cnn = null;
            SqlCommand cmd = null;
            SqlDataReader dbReader = null;
            DataTable dt = null;

            try
            {
                cnn = new SqlConnection(MyStatics.Configurazione.ConnectionString);
                cmd = new SqlCommand(sql, cnn);
                cnn.Open();

                dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                dt = new DataTable();
                dt.Load(dbReader);
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                dt = null;
            }
            finally
            {
                cmd.Dispose();
                cmd = null;

                dbReader.Close();
                dbReader.Dispose();
                dbReader = null;

                if (cnn.State == ConnectionState.Open) cnn.Close();
                cnn.Dispose();
                cnn = null;
            }

            return dt;
        }

        internal static bool SaveDataSet(DataSet oDs, string sql)
        {
            try
            {
                SqlDataObject sd = new SqlDataObject(MyStatics.Configurazione.ConnectionString, sql);
                sd.SaveData(oDs);
                return true;
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                return false;
            }
        }
        internal static bool SaveDataSet(DataSet oDs, string sql, string connectionstring)
        {
            try
            {
                SqlDataObject sd = new SqlDataObject(connectionstring, sql);
                sd.SaveData(oDs);
                return true;
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                return false;
            }
        }

        internal static void ExecStoredProcNQ(string procName, SqlParameter[] sqlParams, ref SqlParameterCollection outParams)
        {
            SqlConnection oSqlConn = null;
            SqlCommand oCmd = new SqlCommand();

            oSqlConn = new SqlConnection(MyStatics.Configurazione.ConnectionString);
            oSqlConn.Open();

            oCmd.Connection = oSqlConn;
            oCmd.CommandTimeout = 0;
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = procName;

            if (outParams != null && outParams.Count > 0) { outParams.Clear(); outParams = null; }

            if (sqlParams != null)
            {
                foreach (SqlParameter par in sqlParams)
                {
                    oCmd.Parameters.Add(par);
                }

            }

            oCmd.ExecuteNonQuery();
            outParams = oCmd.Parameters;
            oCmd.Dispose();

        }
        internal static void ExecStoredProcNQ(string procName, ref SqlParameter[] sqlParams)
        {
            SqlConnection oSqlConn = null;
            SqlCommand oCmd = new SqlCommand();

            oSqlConn = new SqlConnection(MyStatics.Configurazione.ConnectionString);
            oSqlConn.Open();

            oCmd.Connection = oSqlConn;
            oCmd.CommandTimeout = 0;
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = procName;

            if (sqlParams != null)
            {
                oCmd.Parameters.AddRange(sqlParams);
            }

            oCmd.ExecuteNonQuery();

            oCmd.Connection.Close();
            oCmd.Connection = null;
            oCmd.Dispose();

        }
        internal static DataSet ExecStoredProc(string procName, SqlParameter[] sqlParams, out SqlParameterCollection outParams)
        {
            SqlConnection oSqlConn = null;
            SqlCommand oCmd = new SqlCommand();

            oSqlConn = new SqlConnection(MyStatics.Configurazione.ConnectionString);
            oSqlConn.Open();

            oCmd.Connection = oSqlConn;
            oCmd.CommandTimeout = 0;
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = procName;

            if (sqlParams != null)
            {
                foreach (SqlParameter par in sqlParams)
                {
                    oCmd.Parameters.Add(par);
                }
            }

            SqlDataAdapter _da = new SqlDataAdapter(oCmd);
            DataSet _ds = new DataSet();
            _da.Fill(_ds);
            outParams = _da.SelectCommand.Parameters;

            oCmd.Dispose();

            return _ds;
        }

        internal static long GetLastIdentityForTable(string vsTableName)
        {
            long lastId = -1;

            try
            {

                if (vsTableName != null && vsTableName.Trim() != "")
                {
                    string sSql = @"SELECT IDENT_CURRENT('" + vsTableName + @"')";
                    DataTable dt = GetDataTable(sSql);
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0 && !dt.Rows[0].IsNull(0))
                        {
                            lastId = Convert.ToInt64(dt.Rows[0][0]);
                        }

                        dt.Dispose();
                    }
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                lastId = -1;
            }
            return lastId;
        }

        #endregion

        #region Formattazione SQL

        internal static string Ax2(string Sql)
        {
            return Sql.Replace("'", "''");
        }

        internal static string SQLDate(DateTime vDate)
        {

            string sRet = string.Format("Convert(DateTime,'{0}-{1}-{2}',120)",
                                            vDate.ToString(@"yyyy"),
                                            vDate.ToString(@"MM"),
                                            vDate.ToString(@"dd"));
            return sRet;

        }

        internal static string SQLDateTime(DateTime vDate)
        {
            string sRet = vDate.ToString(@"yyyy/MM/dd HH:mm").Replace(@".", @":");
            sRet = @"Convert(DateTime,'" + sRet + @"',120)";
            return sRet;
        }

        internal static string SQLDateTimeInsert(DateTime vDate)
        {
            string sRet = vDate.ToString(@"yyyy-MM-dd HH:mm:ss").Replace(@".", @":");
            sRet = @"Convert(DateTime,'" + sRet + @"',120)";
            return sRet;
        }

        #endregion

        #region Sql e DataBinding

        internal static string GetViewDataName(ref Infragistics.Win.UltraWinToolbars.ToolBase roTool)
        {
            return GetViewDataName(roTool.Key.ToString());
        }
        internal static string GetViewDataName(string rsToolKey)
        {
            string sRet = "";

            switch (rsToolKey)
            {
                case MyStatics.GC_LOGIN:
                    sRet = @"T_Login";
                    break;

                case MyStatics.GC_MODULI:
                    sRet = @"T_Moduli";
                    break;

                case MyStatics.GC_RUOLI:
                    sRet = @"T_Ruoli";
                    break;

                case MyStatics.GC_AZIENDE:
                    sRet = @"T_Aziende";
                    break;

                case MyStatics.GC_UNITAATOMICHE:
                    sRet = @"T_UnitaAtomiche";
                    break;
                case MyStatics.GC_UNITAOPERATIVE:
                    sRet = @"T_UnitaOperative";
                    break;

                case MyStatics.GC_DIARIOMEDICO:
                    sRet = @"T_DiarioMedico";
                    break;

                case MyStatics.GC_DIARIOINFERMIERISTICO:
                    sRet = @"T_DiarioInfermieristico";
                    break;

                case MyStatics.GC_PARAMETRIVITALITIPO:
                    sRet = @"T_TipoParametroVitale";
                    break;

                case MyStatics.GC_PARAMETRIVITALISTATO:
                    sRet = @"T_StatoParametroVitale";
                    break;

                case MyStatics.GC_CONSEGNATIPO:
                    sRet = @"T_TipoConsegna";
                    break;

                case MyStatics.GC_CONSEGNASTATO:
                    sRet = @"T_StatoConsegna";
                    break;

                case MyStatics.GC_CONSEGNAPAZIENTETIPO:
                    sRet = @"T_TipoConsegnaPaziente";
                    break;

                case MyStatics.GC_CONSEGNAPAZIENTESTATO:
                    sRet = @"T_StatoConsegnaPaziente";
                    break;

                case MyStatics.GC_CONSEGNAPAZIENTESTATORUOLI:
                    sRet = @"T_StatoConsegnaPazienteRuoli";
                    break;

                case MyStatics.GC_CONSENSOSTATICALCOLATI:
                    sRet = @"T_StatoConsensoCalcolato";
                    break;

                case MyStatics.GC_EVIDENZACLINICA:
                    sRet = @"T_TipoEvidenzaClinica";
                    break;

                case MyStatics.GC_REPORT:
                    sRet = @"T_Report";
                    break;

                case MyStatics.GC_MASCHERE:
                    sRet = @"T_Maschere";
                    break;

                case MyStatics.GC_TESTIPREDEFINITI:
                    sRet = @"T_TestiPredefiniti";
                    break;

                case MyStatics.GC_TESTINOTEPREDEFINITI:
                    sRet = @"T_TestiNotePredefiniti";
                    break;

                case MyStatics.GC_NEWS:
                    sRet = @"T_MovNews";
                    break;

                case MyStatics.GC_VIESOMMINISTRAZIONE:
                    sRet = @"T_ViaSomministrazione";
                    break;

                case MyStatics.GC_PROTOCOLLI:
                    sRet = @"T_Protocolli";
                    break;

                case MyStatics.GC_PROTOCOLLITEMPI:
                    sRet = @"T_ProtocolliTempi";
                    break;

                case MyStatics.GC_TIPOPRESCRIZIONE:
                    sRet = @"T_TipoPrescrizione";
                    break;

                case MyStatics.GC_ALLEGATITIPO:
                    sRet = @"T_TipoAllegato";
                    break;

                case MyStatics.GC_EVIDENZACLINICASTATI:
                    sRet = @"T_StatoEvidenzaClinica";
                    break;

                case MyStatics.GC_APPUNTAMENTITIPO:
                    sRet = @"T_TipoAppuntamento";
                    break;

                case MyStatics.GC_APPUNTAMENTISTATO:
                    sRet = @"T_StatoAppuntamento";
                    break;

                case MyStatics.GC_TASKINFERMIERISTICISTATO:
                    sRet = @"T_StatoTaskInfermieristico";
                    break;

                case MyStatics.GC_TASKINFERMIERISTICITIPO:
                    sRet = @"T_TipoTaskInfermieristico";
                    break;

                case MyStatics.GC_SCHEDELISTA:
                    sRet = @"T_Schede";
                    break;

                case MyStatics.GC_STANZE:
                    sRet = @"T_Stanze";
                    break;

                case MyStatics.GC_FORMATOREPORT:
                    sRet = @"T_FormatoReport";
                    break;

                case MyStatics.GC_STATOPRESCRIZIONE:
                    sRet = @"T_StatoPrescrizione";
                    break;

                case MyStatics.GC_TIPOEPISODIO:
                    sRet = @"T_TipoEpisodio";
                    break;

                case MyStatics.GC_STATOTRASFERIMENTO:
                    sRet = @"T_StatoTrasferimento";
                    break;

                case MyStatics.GC_STATODIARIO:
                    sRet = @"T_StatoDiario";
                    break;

                case MyStatics.GC_TIPODIARIO:
                    sRet = @"T_TipoDiario";
                    break;

                case MyStatics.GC_LETTI:
                    sRet = @"T_Letti";
                    break;

                case MyStatics.GC_SETTORI:
                    sRet = @"T_Settori";
                    break;

                case MyStatics.GC_SCHEDETIPO:
                    sRet = @"T_TipoScheda";
                    break;

                case MyStatics.GC_DIZIONARI:
                    sRet = @"T_DCDecodifiche";
                    break;

                case MyStatics.GC_ASSLETTIUA:
                    sRet = @"T_AssUAUOLetti";
                    break;

                case MyStatics.GC_CDSSAZIONI:
                    sRet = @"T_CDSSAzioni";
                    break;

                case MyStatics.GC_CDSSPLUGINS:
                    sRet = @"T_CDSSPlugins";
                    break;

                case MyStatics.GC_INTEGRAZIONIDESTINATARI:
                    sRet = @"T_Integra_Destinatari";
                    break;

                case MyStatics.GC_INTEGRAZIONICAMPI:
                    sRet = @"T_Integra_Campi";
                    break;

                case MyStatics.GC_CDSSSTRUTTURA:
                    sRet = @"T_CDSSStruttura";
                    break;

                case MyStatics.GC_CDSSSTRUTTURARUOLI:
                    sRet = @"T_CDSSStrutturaRuoli";
                    break;

                case MyStatics.GC_AGENDE:
                    sRet = @"T_Agende";
                    break;

                case MyStatics.GC_TIPOAGENDA:
                    sRet = @"T_TipoAgenda";
                    break;

                case MyStatics.GC_FESTIVITA:
                    sRet = @"T_Festivita";
                    break;

                case MyStatics.GC_EVIDENZACLINICASTATIVISIONE:
                    sRet = @"T_StatoEvidenzaClinicaVisione";
                    break;

                case MyStatics.GC_ALERTALLERGIEANAMNESITIPO:
                    sRet = @"T_TipoAlertAllergiaAnamnesi";
                    break;

                case MyStatics.GC_ALERTALLERGIEANAMNESISTATO:
                    sRet = @"T_StatoAlertAllergiaAnamnesi";
                    break;

                case MyStatics.GC_ALERTGENERICITIPO:
                    sRet = @"T_TipoAlertGenerico";
                    break;

                case MyStatics.GC_ALERTGENERICISTATO:
                    sRet = @"T_StatoAlertGenerico";
                    break;

                case MyStatics.GC_SCHEDESTATO:
                    sRet = @"T_StatoScheda";
                    break;

                case MyStatics.GC_SCHEDESTATOCALCOLATO:
                    sRet = @"T_StatoSchedaCalcolato";
                    break;

                case MyStatics.GC_SISTEMI:
                    sRet = @"T_Sistemi";
                    break;

                case MyStatics.GC_ALLEGATIFORMATO:
                    sRet = @"T_FormatoAllegati";
                    break;

                case MyStatics.GC_ORDINITIPO:
                    sRet = @"T_TipoOrdine";
                    break;

                case MyStatics.GC_ORDINIFORMULE:
                    sRet = @"T_OEFormule";
                    break;

                case MyStatics.GC_ORDINIATTRIBUTI:
                    sRet = @"T_OEAttributi";
                    break;

                case MyStatics.GC_ORDINISTATO:
                    sRet = @"T_StatoOrdine";
                    break;

                case MyStatics.GC_STATOPRESCRIZIONETEMPI:
                    sRet = @"T_StatoPrescrizioneTempi";
                    break;

                case MyStatics.GC_STATOCONTINUAZIONE:
                    sRet = @"T_StatoContinuazione";
                    break;

                case MyStatics.GC_APPUNTAMENTISTATOAGENDE:
                    sRet = @"T_StatoAppuntamentoAgende";
                    break;

                case MyStatics.GC_ALLEGATISTATO:
                    sRet = @"T_StatoAllegato";
                    break;

                case MyStatics.GC_STATOCARTELLA:
                    sRet = @"T_StatoCartella";
                    break;

                case MyStatics.GC_STATOCARTELLAINFO:
                    sRet = @"T_StatoCartellaInfo";
                    break;

                case MyStatics.GC_STATOEPISODIO:
                    sRet = @"T_StatoEpisodio";
                    break;

                case MyStatics.GC_SEZIONIFUT:
                    sRet = @"T_SezioniFUT";
                    break;

                case MyStatics.GC_STATOCARTELLAINVISIONE:
                    sRet = @"T_StatoCartellaInVisione";
                    break;

                case MyStatics.GC_BANCHEDATILISTA:
                    sRet = @"T_EBM";
                    break;

                case MyStatics.GC_CONFIGURAZIONE_PC:
                    sRet = @"T_ConfigPC";
                    break;

                case MyStatics.GC_CONTATORI:
                    sRet = @"T_Contatori";
                    break;

                case MyStatics.GC_FILTRISPECIALI:
                    sRet = @"T_FiltriSpeciali";
                    break;

                case MyStatics.GC_PROTOCOLLIATTIVITA:
                    sRet = @"T_ProtocolliAttivita";
                    break;

                case MyStatics.GC_SELEZIONI:
                    sRet = @"T_Selezioni";
                    break;

                case MyStatics.GC_SCREEN:
                    sRet = @"T_Screen";
                    break;

                case MyStatics.GC_PROFILITERAPIEFARMACOLOGICHE:
                    sRet = @"T_ProtocolliPrescrizioni";
                    break;

                case MyStatics.GC_ENTITAALLEGATI:
                    sRet = @"T_EntitaAllegato";
                    break;
            }

            return sRet;
        }

        internal static string GetSqlView(Enums.EnumDataNames dn)
        {

            switch (dn)
            {

                case Enums.EnumDataNames.T_Login:
                    return "select Codice,Descrizione,Cognome,Nome,Note,FlagAdmin,FlagObsoleto,FlagSistema,CodiceFiscale From T_Login";

                case Enums.EnumDataNames.T_Moduli:
                    return "Select * From T_Moduli Order By Descrizione";

                case Enums.EnumDataNames.T_Ruoli:
                    return @"Select * From T_Ruoli Order By Descrizione";

                case Enums.EnumDataNames.T_Aziende:
                    return @"Select Codice, Descrizione From T_Aziende Order By Descrizione";

                case Enums.EnumDataNames.T_UnitaAtomiche:
                    return @"Select * From T_UnitaAtomiche Order By Descrizione";

                case Enums.EnumDataNames.T_UnitaOperative:
                    return @"Select * From T_UnitaOperative Order By Descrizione";

                case Enums.EnumDataNames.T_DiarioMedico:
                    return @"Select * From T_TipoVoceDiario where CodTipoDiario = 'M'";

                case Enums.EnumDataNames.T_DiarioInfermieristico:
                    return @"Select * From T_TipoVoceDiario where CodTipoDiario = 'I'";

                case Enums.EnumDataNames.T_TipoParametroVitale:
                    return @"Select Codice, Descrizione, CodScheda, Ordine, Colore as CodColore, '' AS Colore, Icona From T_TipoParametroVitale";

                case Enums.EnumDataNames.T_TipoConsegna:
                    return @"Select Codice, Descrizione, CodScheda, Colore as CodColore, '' AS Colore, Icona From T_TipoConsegna";

                case Enums.EnumDataNames.T_TipoConsegnaPaziente:
                    return @"Select Codice, Descrizione, CodScheda, Colore as CodColore, '' AS Colore, Icona From T_TipoConsegnaPaziente";

                case Enums.EnumDataNames.T_TipoEvidenzaClinica:
                    return @"Select * From T_TipoEvidenzaClinica";

                case Enums.EnumDataNames.T_Report:
                    return @"Select F.Icona, R.Codice, R.Descrizione, R.CodFormatoReport, F.Descrizione As Formato
	                            , R.DaStoricizzare, R.[Path], R.Note, V.Descrizione As Vista, R.CodReportVista, R.NomePlugin
                                , R.FlagSistema, R.ApriBrowser, R.ApriIE, R.FlagRichiediStampante
                            From T_Report R
                                Left Join T_FormatoReport F On R.CodFormatoReport = F.Codice
	                            Left Join T_ReportViste V On R.CodReportVista = V.Codice";

                case Enums.EnumDataNames.T_Maschere:
                    return @"Select * From T_Maschere";

                case Enums.EnumDataNames.T_FormatoReport:
                    return @"Select * From T_FormatoReport";

                case Enums.EnumDataNames.T_TestiPredefiniti:
                    return @"Select * From T_TestiPredefiniti Order By Descrizione";

                case Enums.EnumDataNames.T_TestiNotePredefiniti:
                    return @"Select * From T_TestiNotePredefiniti Order By Descrizione";

                case Enums.EnumDataNames.T_ViaSomministrazione:
                    return @"Select * From T_ViaSomministrazione Order By Descrizione";

                case Enums.EnumDataNames.T_TipoPrescrizione:
                    return @"Select P.*, '' As ColoreGraf, S.Descrizione As Scheda, SS.Descrizione As [Scheda Posologia], V.Descrizione As ViaSomministrazione From T_TipoPrescrizione P " +
                                "Left Join T_Schede S On P.CodScheda = S.Codice " +
                                "Left Join T_Schede SS On P.CodSchedaPosologia = SS.Codice " +
                                "Left Join T_ViaSomministrazione V On P.CodViaSomministrazione = V.Codice Order By P.Descrizione";

                case Enums.EnumDataNames.T_StatoPrescrizione:
                    return @"Select Codice, Descrizione, Colore as CodColore, '' AS Colore, Icona From T_StatoPrescrizione Order By Descrizione";

                case Enums.EnumDataNames.T_TipoAllegato:
                    return @"Select Codice, Descrizione, Colore as CodColore, '' AS Colore, Icona From T_TipoAllegato Order By Descrizione";

                case Enums.EnumDataNames.T_MovNews:
                    return @"Select * From T_MovNews Order By Rilevante, DataInizioPubblicazione, DataFinePubblicazione";

                case Enums.EnumDataNames.T_StatoEvidenzaClinica:
                    return @"Select Codice, Descrizione, Colore as CodColore, '' AS Colore, Icona From T_StatoEvidenzaClinica Order By Descrizione";

                case Enums.EnumDataNames.T_TipoAppuntamento:
                    return @"Select Codice, Descrizione, CodScheda, IsNull(SenzaData, 0) As SenzaData, IsNull(SenzaDataSempre, 0) As SenzaDataSempre, IsNull(Settimanale, 0) As Settimanale, IsNull(Multiplo, 0) As Multiplo, Colore as CodColore, '' AS Colore, Icona, IsNull(TimeSlotInterval,0) As [Time Slot], IsNull(Ripianificazione,0) As Ripianificazione From T_TipoAppuntamento Order By Descrizione";

                case Enums.EnumDataNames.T_StatoAppuntamento:
                    return @"Select Codice, Descrizione, Ordine, Icona, Colore as CodColore, '' AS Colore, Riservato From T_StatoAppuntamento Order By Descrizione";

                case Enums.EnumDataNames.T_TipoTaskInfermieristico:
                    return @"Select Codice, Descrizione, CodScheda, Sigla, Anticipo, Ripianificazione, ErogazioneDiretta, Colore as CodColore, '' AS Colore,  Icona From T_TipoTaskInfermieristico Order By Descrizione";

                case Enums.EnumDataNames.T_StatoTaskInfermieristico:
                    return @"Select Codice, Descrizione, Visibile, Colore as CodColore, '' AS Colore, Icona From T_StatoTaskInfermieristico Order By Descrizione";

                case Enums.EnumDataNames.T_Schede:
                    return @"Select S.[Codice],S.[Descrizione],S.[SchedaSemplice]
                                  ,S.[CodTipoScheda], T.Descrizione As Tipo
                                  ,S.[CodEntita], E.Descrizione As Entita
                                  ,S.[Path],S.[Ordine], S.CreaDefault, S.NumerositaMinima,S.NumerositaMassima, S.[Note]
                            From T_Schede S
	                            Left Join T_Entita E On S.CodEntita = E.Codice
	                            Left Join T_TipoScheda T On S.CodTipoScheda = T.Codice
                            Order By S.Descrizione";

                case Enums.EnumDataNames.T_Stanze:
                    return @"Select * From T_Stanze Order By Descrizione";

                case Enums.EnumDataNames.T_SchedeVersioni:
                    return @"Select * From T_SchedeVersioni";

                case Enums.EnumDataNames.T_TipoEpisodio:
                    return @"Select * From T_TipoEpisodio Order By Descrizione";

                case Enums.EnumDataNames.T_StatoTrasferimento:
                    return @"Select Codice, Descrizione, Colore as CodColore, '' AS Colore, Icona, Ordine From T_StatoTrasferimento Order By Descrizione";

                case Enums.EnumDataNames.T_StatoDiario:
                    return @"Select Codice, Descrizione, Colore as CodColore, '' AS Colore, Icona From T_StatoDiario Order By Descrizione";

                case Enums.EnumDataNames.T_TipoDiario:
                    return @"Select * From T_TipoDiario Order By Descrizione";

                case Enums.EnumDataNames.T_Letti:
                    return @"Select 
	                            IsNull(A.Descrizione, '') As Azienda
	                            , IsNull(S.Descrizione, '') As Settore
	                            , IsNull(Z.Descrizione, '') As Stanza
                                , L.*
                            From T_Letti L
	                            Left Join T_Aziende A On L.CodAzi = A.Codice
	                            Left Join T_Settori S On L.CodAzi = S.CodAzi And L.CodSettore = S.Codice
	                            Left Join T_Stanze Z On L.CodAzi = Z.CodAzi And L.CodSettore = Z.Codice
                            Order By A.Descrizione, S.Descrizione, Z.Descrizione, L.Descrizione";

                case Enums.EnumDataNames.T_Settori:
                    return @"Select 
	                            IsNull(A.Descrizione, '') As Azienda
                                , L.*
                            From T_Settori L
	                            Left Join T_Aziende A On L.CodAzi = A.Codice
                            Order By A.Descrizione, L.Descrizione";

                case Enums.EnumDataNames.T_TipoScheda:
                    return @"Select * From T_TipoScheda Order By Descrizione";

                case Enums.EnumDataNames.T_DCDecodifiche:
                    return @"Select * From T_DCDecodifiche Order By Descrizione";

                case Enums.EnumDataNames.T_DCDecodificheValori:
                    return @"Select * From T_DCDecodificheValori";

                case Enums.EnumDataNames.T_AssUAUOLetti:
                    return @"SELECT 
	                            ASS.CodUA,
	                            UA.Descrizione AS DescUA,
	                            ASS.CodAzi,
	                            AZI.Descrizione AS DescAzi,
	                            ASS.CodUO,
	                            CASE WHEN ASS.CodUO <> '*' THEN ISNULL(UO.Descrizione, '') ELSE 'Tutte' END AS DescUO,
	                            ASS.CodSettore,
	                            CASE WHEN ASS.CodSettore <> '*' THEN ISNULL(S.Descrizione, '') ELSE 'Tutti' END AS DescSettore,	
	                            ASS.Codletto,
	                            CASE WHEN ASS.Codletto <> '*' THEN ISNULL(L.Descrizione, '') ELSE 'Tutti' END AS DescLetto
                            FROM 
	                            T_AssUAUOLetti ASS LEFT JOIN T_Aziende AZI ON
		                            ASS.CodAzi = AZI.Codice
	                            LEFT JOIN T_UnitaAtomiche UA ON
		                            ASS.CodUA = UA.Codice
	                            LEFT JOIN T_UnitaOperative UO ON
		                            ASS.CodUO = UO.Codice AND ASS.CodAzi = UO.CodAzi
	                            LEFT JOIN T_Settori S ON
		                            ASS.CodSettore = S.Codice AND ASS.CodAzi = S.CodAzi
	                            LEFT JOIN T_Letti L ON
		                            ASS.CodSettore = L.CodSettore AND ASS.CodAzi = L.CodAzi AND ASS.CodLetto = L.CodLetto";

                case Enums.EnumDataNames.T_CDSSAzioni:
                    return @"Select * From T_CDSSAzioni Order By Descrizione";

                case Enums.EnumDataNames.T_CDSSPlugins:
                    return @"Select * From T_CDSSPlugins Order By Descrizione";

                case Enums.EnumDataNames.T_CDSSStruttura:
                    return @"Select S.ID, S.CodUA, UA.Descrizione As [Unità Atomica]," + Environment.NewLine +
                                "S.CodAzione, A.Descrizione As [Azione]," + Environment.NewLine +
                                "S.CodPlugin, P.Descrizione As [Plugin], S.Parametri" + Environment.NewLine +
                            "From T_CDSSStruttura S" + Environment.NewLine +
                                "Inner Join T_UnitaAtomiche UA ON S.CodUA = UA.Codice" + Environment.NewLine +
                                "Inner Join T_CDSSAzioni A ON S.CodAzione = A.Codice" + Environment.NewLine +
                                "Inner Join T_CDSSPlugins P ON S.CodPlugin = P.Codice" + Environment.NewLine +
                            "Order By S.CodUA, S.CodAzione, S.CodPlugin";

                case Enums.EnumDataNames.T_CDSSStrutturaRuoli:
                    return @"Select S.ID, S.CodRuolo, R.Descrizione As Ruolo," + Environment.NewLine +
                                "S.CodAzione, A.Descrizione As [Azione]," + Environment.NewLine +
                                "S.CodPlugin, P.Descrizione As [Plugin], S.Parametri" + Environment.NewLine +
                            "From T_CDSSStrutturaRuoli S" + Environment.NewLine +
                                "Inner Join T_Ruoli R ON S.CodRuolo = R.Codice" + Environment.NewLine +
                                "Inner Join T_CDSSAzioni A ON S.CodAzione = A.Codice" + Environment.NewLine +
                                "Inner Join T_CDSSPlugins P ON S.CodPlugin = P.Codice" + Environment.NewLine +
                            " Order By S.CodRuolo, S.CodAzione, S.CodPlugin";

                case Enums.EnumDataNames.T_Integra_Destinatari:
                    return @"Select * From T_Integra_Destinatari Order By Descrizione";

                case Enums.EnumDataNames.T_Integra_Campi:
                    return @"Select * From T_Integra_Campi Order By Codice";

                case Enums.EnumDataNames.T_TipoAgenda:
                    return @"Select * From T_TipoAgenda Order By Descrizione";

                case Enums.EnumDataNames.T_Festivita:
                    return @"Select * From T_Festivita Order By Data";

                case Enums.EnumDataNames.T_FestivitaAgende:
                    return @"Select * From T_FestivitaAgende";

                case Enums.EnumDataNames.T_Agende:
                    return @"Select A.[Codice], A.[Descrizione], A.DescrizioneAlternativa As [Descrizione Alternativa]
                                  ,A.[CodTipoAgenda], TA.Descrizione As Tipo
                                  ,A.Lista
                                  ,A.[CodEntita], E.Descrizione As Entita
                                  ,A.[Colore] As CodColore, '' AS Colore
                                  ,A.[ElencoCampi],A.[IntervalloSlot],A.[OrariLavoro],A.[Ordine]
                                  ,A.[UsaColoreTipoAppuntamento],A.[EscludiFestivita]
                            From T_Agende A
	                            Left Join T_Entita E On A.CodEntita = E.Codice
	                            Left Join T_TipoAgenda TA On A.CodTipoAgenda = TA.Codice
                            Order By A.Descrizione";

                case Enums.EnumDataNames.T_StatoEvidenzaClinicaVisione:
                    return @"Select Codice, Descrizione, Colore as CodColore, '' AS Colore, Icona From T_StatoEvidenzaClinicaVisione Order By Descrizione";

                case Enums.EnumDataNames.T_TipoAlertAllergiaAnamnesi:
                    return @"Select * From T_TipoAlertAllergiaAnamnesi Order By Descrizione";

                case Enums.EnumDataNames.T_TipoAlertGenerico:
                    return @"Select * From T_TipoAlertGenerico Order By Descrizione";

                case Enums.EnumDataNames.T_StatoAlertGenerico:
                    return @"Select Codice, Descrizione, Colore as CodColore, '' AS Colore, Icona From T_StatoAlertGenerico Order By Descrizione";

                case Enums.EnumDataNames.T_StatoAlertAllergiaAnamnesi:
                    return @"Select Codice, Descrizione, Colore as CodColore, '' AS Colore, Icona From T_StatoAlertAllergiaAnamnesi Order By Descrizione";

                case Enums.EnumDataNames.T_StatoParametroVitale:
                    return @"Select Codice, Descrizione, Colore as CodColore, '' AS Colore, Icona From T_StatoParametroVitale Order By Descrizione";

                case Enums.EnumDataNames.T_StatoConsegna:
                    return @"Select Codice, Descrizione, Colore as CodColore, '' AS Colore, Icona From T_StatoConsegna Order By Descrizione";

                case Enums.EnumDataNames.T_StatoConsegnaPaziente:
                    return @"Select Codice, Descrizione, Colore as CodColore, '' AS Colore, Icona From T_StatoConsegnaPaziente Order By Descrizione";

                case Enums.EnumDataNames.T_StatoConsegnaPazienteRuoli:
                    return @"Select Codice, Descrizione, Colore as CodColore, '' AS Colore, Icona From T_StatoConsegnaPazienteRuoli Order By Descrizione";

                case Enums.EnumDataNames.T_StatoConsensoCalcolato:
                    return @"Select Codice, Descrizione, Icona From T_StatoConsensoCalcolato Order By Descrizione";

                case Enums.EnumDataNames.T_StatoScheda:
                    return @"Select Codice, Descrizione, Colore as CodColore, '' AS Colore, Icona From T_StatoScheda Order By Descrizione";

                case Enums.EnumDataNames.T_StatoSchedaCalcolato:
                    return @"Select CodScheda, Codice, Descrizione, Colore as CodColore, '' AS Colore, Icona From T_StatoSchedaCalcolato Order By Descrizione";

                case Enums.EnumDataNames.T_Sistemi:
                    return @"Select Codice, Descrizione, Colore as CodColore, '' AS Colore, Icona, FlagSistemaRiservato AS Riservato, CodDestinatario From T_Sistemi Order By Descrizione";

                case Enums.EnumDataNames.T_FormatoAllegati:
                    return @"Select Codice, Descrizione, Colore as CodColore, '' AS Colore, Icona From T_FormatoAllegati Order By Descrizione";

                case Enums.EnumDataNames.T_TipoOrdine:
                    return @"Select * From T_TipoOrdine Order By Descrizione";

                case Enums.EnumDataNames.T_OEFormule:
                    return @"Select * From T_OEFormule Order By CodUA";

                case Enums.EnumDataNames.T_OEAttributi:
                    return @"Select * From T_OEAttributi Order By CodEntita";

                case Enums.EnumDataNames.T_StatoOrdine:
                    return @"Select Codice, Descrizione, Colore as CodColore, '' AS Colore, Icona From T_StatoOrdine Order By Descrizione";

                case Enums.EnumDataNames.T_StatoPrescrizioneTempi:
                    return @"Select Codice, Descrizione, Colore as CodColore, '' AS Colore, Icona From T_StatoPrescrizioneTempi Order By Descrizione";

                case Enums.EnumDataNames.T_StatoContinuazione:
                    return @"Select Codice, Descrizione, Colore as CodColore, '' AS Colore, Icona From T_StatoContinuazione Order By Descrizione";

                case Enums.EnumDataNames.T_StatoAppuntamentoAgende:
                    return @"Select Codice, Descrizione, Colore as CodColore, '' AS Colore, Ordine, Icona From T_StatoAppuntamentoAgende Order By Descrizione";

                case Enums.EnumDataNames.T_StatoAllegato:
                    return @"Select Codice, Descrizione, Colore as CodColore, '' AS Colore, Ordine, Icona From T_StatoAllegato Order By Descrizione";

                case Enums.EnumDataNames.T_StatoCartella:
                    return @"Select Codice, Descrizione, Colore as CodColore, '' AS Colore, Icona From T_StatoCartella Order By Descrizione";

                case Enums.EnumDataNames.T_StatoCartellaInfo:
                    return @"Select Codice, Descrizione, Colore as CodColore, '' AS Colore, Icona From T_StatoCartellaInfo Order By Descrizione";

                case Enums.EnumDataNames.T_StatoEpisodio:
                    return @"Select Codice, Descrizione, Colore as CodColore, '' AS Colore, Icona From T_StatoEpisodio Order By Descrizione";

                case Enums.EnumDataNames.T_Protocolli:
                    return @"Select Codice, Descrizione, Continuita, Durata, CodTipoProtocollo From T_Protocolli Order By Descrizione";

                case Enums.EnumDataNames.T_ProtocolliTempi:
                    return @"Select T.Codice, T.Descrizione, T.CodProtocollo, T.Delta, T.Ora, P.CodTipoProtocollo" + Environment.NewLine +
                                "From T_ProtocolliTempi T" + Environment.NewLine +
                                        "Inner Join T_Protocolli P ON P.Codice = T.CodProtocollo";

                case Enums.EnumDataNames.T_SezioniFUT:
                    return @"Select T_SezioniFUT.Codice, T_SezioniFUT.Descrizione, CONVERT(VARCHAR, T_SezioniFUT.Ordine) AS Ordine, T_SezioniFUT.CodEntita, T_Entita.Descrizione AS Entita, T_SezioniFUT.Colore as CodColore, '' AS Colore, T_SezioniFUT.Icona From T_SezioniFUT INNER JOIN T_Entita ON T_SezioniFUT.CodEntita = T_Entita.Codice Order By T_SezioniFUT.Ordine";

                case Enums.EnumDataNames.T_StatoCartellaInVisione:
                    return @"Select Codice, Descrizione, Colore as CodColore, '' AS Colore, Icona From T_StatoCartellaInVisione Order By Descrizione";

                case Enums.EnumDataNames.T_EBM:
                    return @"Select Codice, Descrizione From T_EBM Order by Descrizione";

                case Enums.EnumDataNames.T_ConfigPC:
                    return @"Select CodPC, Min(Descrizione) As Descrizione From T_ConfigPC Group By CodPC Order by Descrizione";

                case Enums.EnumDataNames.T_Contatori:
                    return @"Select Codice, Descrizione, Valore, DataScadenza, CodUnitaScadenza, Sistema From T_Contatori Order By Descrizione";

                case Enums.EnumDataNames.T_FiltriSpeciali:
                    return @"Select F.Codice, F.Descrizione, F.CodTipoFiltroSpeciale As [Cod Tipo], IsNull(T.Descrizione, '') As Tipo
                             From T_FiltriSpeciali F
                                Left Join T_TipoFiltroSpeciale T On T.Codice = F.CodTipoFiltroSpeciale
                             Order By F.Codice";

                case Enums.EnumDataNames.T_ProtocolliAttivita:
                    return @"Select Codice, Descrizione, Colore as CodColore, '' AS Colore, Icona From T_ProtocolliAttivita Order By Descrizione";

                case Enums.EnumDataNames.T_ProtocolliAttivitaTempi:
                    return @"Select Codice, Descrizione, DeltaGiorni AS [Delta Giorni], DeltaOre AS [Delta Ore], DeltaMinuti AS [Delta Minuti] From T_ProtocolliAttivitaTempi";

                case Enums.EnumDataNames.T_ProtocolliAttivitaTempiTipoTask:
                    return @"Select TP.Codice, TP.Descrizione, TP.Colore AS CodColore, '' AS Colore, TP.Icona " + Environment.NewLine +
                            "from T_ProtocolliAttivitaTempiTipoTask TT INNER JOIN T_TipoTaskInfermieristico TP ON" + Environment.NewLine +
                            "TT.CodTipoTaskInfermieristico = TP.Codice ";

                case Enums.EnumDataNames.T_TipoSelezione:
                    return @"Select * From T_TipoSelezione";

                case Enums.EnumDataNames.T_TipoFiltroSpeciale:
                    return @"Select * From T_TipoFiltroSpeciale";

                case Enums.EnumDataNames.T_Selezioni:
                    return @"Select TS.Codice As CodTipoSel, TS.Descrizione As Tipo
                                , S.Codice, S.Descrizione, S.FlagSistema
                                , S.[CodUtenteInserimento], S.[CodRuoloInserimento], S.[DataInserimento]
                                , S.[CodUtenteUltimaModifica], S.[CodRuoloUltimaModifica], S.[DataUltimaModifica]
                            From T_Selezioni S INNER JOIN T_TipoSelezione TS ON S.CodTipoSelezione = TS.Codice
                            Order By TS.Codice, S.Codice ";

                case Enums.EnumDataNames.T_AgendePeriodi:
                    return @"Select * From T_AgendePeriodi";

                case Enums.EnumDataNames.T_Screen:
                    return @"SELECT S.Codice, S.Descrizione, S.Righe, S.Colonne, T.Descrizione AS TipoScreen, S.AltezzaRigaGrid, S.LarghezzaColonnaGrid
                                , CONVERT(bit, CASE WHEN ISNULL(S.CodTipoScreen, '') = '" + Scci.Model.en_TipoScreen.EPIGRID.ToString() + @"' THEN CaricaPerRiga ELSE 0 END) As CaricaPerRiga, S.AdattaAltezzaRighe
                                , Predefinito
                             FROM T_Screen S LEFT JOIN T_TipoScreen T ON S.CodTipoScreen = T.Codice ORDER BY S.Descrizione";

                case Enums.EnumDataNames.T_ScreenTile:
                    return @"SELECT S.ID, S.NomeTile, S.CodScreen, S.Riga, S.Colonna, S.Altezza, S.Larghezza, P.Descrizione AS Plugin
                                , S.InEvidenza, S.Fissa, S.NonCollassabile, S.Collassata
                             FROM SCCI.dbo.T_ScreenTile S LEFT JOIN T_PlugIn P ON P.Codice = S.CodPlugin";

                case Enums.EnumDataNames.T_ProtocolliPrescrizioni:
                    return @"SELECT Codice, Descrizione, Colore as CodColore, '' AS Colore, Icona FROM T_ProtocolliPrescrizioni ORDER BY Descrizione, Codice";

                case Enums.EnumDataNames.T_EntitaAllegato:
                    return @"Select Codice, Descrizione, Colore as CodColore, '' AS Colore, Icona From T_EntitaAllegato Order By Descrizione";

                case Enums.EnumDataNames.T_AssUAIntestazioni:
                    return @"Select * From T_AssUAIntestazioni";

                default:
                    return "";

            }

        }

        internal static string GetSqlPUView(Enums.EnumDataNames dn)
        {

            switch (dn)
            {

                case Enums.EnumDataNames.T_Login:
                    return "Select * From T_Login";

                case Enums.EnumDataNames.T_Moduli:
                    return "Select * From T_Moduli";

                case Enums.EnumDataNames.T_Ruoli:
                    return @"Select * From T_Ruoli";

                case Enums.EnumDataNames.T_Aziende:
                    return @"Select * From T_Aziende";

                case Enums.EnumDataNames.T_UnitaAtomiche:
                    return @"Select * From T_UnitaAtomiche";

                case Enums.EnumDataNames.T_UnitaOperative:
                    return @"Select * From T_UnitaOperative";

                case Enums.EnumDataNames.T_DiarioInfermieristico:
                case Enums.EnumDataNames.T_DiarioMedico:
                    return @"Select * From T_TipoVoceDiario";

                case Enums.EnumDataNames.T_TipoParametroVitale:
                    return @"Select * From T_TipoParametroVitale";

                case Enums.EnumDataNames.T_TipoConsegna:
                    return @"Select * From T_TipoConsegna";

                case Enums.EnumDataNames.T_TipoConsegnaPaziente:
                    return @"Select * From T_TipoConsegnaPaziente";

                case Enums.EnumDataNames.T_TipoEvidenzaClinica:
                    return @"Select * FROM T_TipoEvidenzaClinica";

                case Enums.EnumDataNames.T_Report:
                    return @"Select * From T_Report";

                case Enums.EnumDataNames.T_Maschere:
                    return @"Select * FROM T_Maschere";

                case Enums.EnumDataNames.T_FormatoReport:
                    return @"Select * FROM T_FormatoReport";

                case Enums.EnumDataNames.T_TestiPredefiniti:
                    return @"Select * From T_TestiPredefiniti";

                case Enums.EnumDataNames.T_TestiNotePredefiniti:
                    return @"Select * From T_TestiNotePredefiniti";

                case Enums.EnumDataNames.T_ViaSomministrazione:
                    return @"Select * From T_ViaSomministrazione";

                case Enums.EnumDataNames.T_TipoPrescrizione:
                    return @"Select * From T_TipoPrescrizione";

                case Enums.EnumDataNames.T_StatoPrescrizione:
                    return @"Select * From T_StatoPrescrizione";

                case Enums.EnumDataNames.T_TipoAllegato:
                    return @"Select * From T_TipoAllegato";

                case Enums.EnumDataNames.T_MovNews:
                    return @"Select * From T_MovNews";

                case Enums.EnumDataNames.T_StatoEvidenzaClinica:
                    return @"Select * From T_StatoEvidenzaClinica";

                case Enums.EnumDataNames.T_TipoAppuntamento:
                    return @"Select * From T_TipoAppuntamento";

                case Enums.EnumDataNames.T_StatoAppuntamento:
                    return @"Select * From T_StatoAppuntamento";

                case Enums.EnumDataNames.T_TipoTaskInfermieristico:
                    return @"Select * From T_TipoTaskInfermieristico";

                case Enums.EnumDataNames.T_StatoTaskInfermieristico:
                    return @"Select * From T_StatoTaskInfermieristico";

                case Enums.EnumDataNames.T_Schede:
                    return @"Select * From T_Schede";

                case Enums.EnumDataNames.T_Stanze:
                    return @"Select * From T_Stanze";

                case Enums.EnumDataNames.T_SchedeVersioni:
                    return @"Select * From T_SchedeVersioni";

                case Enums.EnumDataNames.T_TipoEpisodio:
                    return @"Select * From T_TipoEpisodio";

                case Enums.EnumDataNames.T_StatoTrasferimento:
                    return @"Select * From T_StatoTrasferimento";

                case Enums.EnumDataNames.T_TipoDiario:
                    return @"Select * From T_TipoDiario";

                case Enums.EnumDataNames.T_Letti:
                    return @"Select * From T_Letti";

                case Enums.EnumDataNames.T_Settori:
                    return @"Select * From T_Settori";

                case Enums.EnumDataNames.T_TipoScheda:
                    return @"Select * From T_TipoScheda";

                case Enums.EnumDataNames.T_DCDecodifiche:
                    return @"Select * From T_DCDecodifiche";

                case Enums.EnumDataNames.T_DCDecodificheValori:
                    return @"Select * From T_DCDecodificheValori";

                case Enums.EnumDataNames.T_AssUAUOLetti:
                    return @"Select * From T_AssUAUOLetti";

                case Enums.EnumDataNames.T_CDSSAzioni:
                    return @"Select * From T_CDSSAzioni";

                case Enums.EnumDataNames.T_CDSSPlugins:
                    return @"Select * From T_CDSSPlugins";

                case Enums.EnumDataNames.T_CDSSStruttura:
                    return @"Select * From T_CDSSStruttura";

                case Enums.EnumDataNames.T_CDSSStrutturaRuoli:
                    return @"Select * From T_CDSSStrutturaRuoli";

                case Enums.EnumDataNames.T_Integra_Destinatari:
                    return @"Select * From T_Integra_Destinatari";

                case Enums.EnumDataNames.T_Integra_Campi:
                    return @"Select * From T_Integra_Campi";

                case Enums.EnumDataNames.T_TipoAgenda:
                    return @"Select * From T_TipoAgenda";

                case Enums.EnumDataNames.T_Festivita:
                    return @"Select * From T_Festivita";

                case Enums.EnumDataNames.T_FestivitaAgende:
                    return @"Select * From T_FestivitaAgende";

                case Enums.EnumDataNames.T_Agende:
                    return @"Select * From T_Agende";

                case Enums.EnumDataNames.T_StatoDiario:
                    return @"Select * From T_StatoDiario";

                case Enums.EnumDataNames.T_StatoEvidenzaClinicaVisione:
                    return @"Select * from T_StatoEvidenzaClinicaVisione";

                case Enums.EnumDataNames.T_TipoAlertAllergiaAnamnesi:
                    return @"Select * From T_TipoAlertAllergiaAnamnesi";

                case Enums.EnumDataNames.T_TipoAlertGenerico:
                    return @"Select * From T_TipoAlertGenerico";

                case Enums.EnumDataNames.T_StatoAlertGenerico:
                    return @"Select * From T_StatoAlertGenerico";

                case Enums.EnumDataNames.T_StatoAlertAllergiaAnamnesi:
                    return @"Select * From T_StatoAlertAllergiaAnamnesi";

                case Enums.EnumDataNames.T_StatoParametroVitale:
                    return @"Select * From T_StatoParametroVitale";

                case Enums.EnumDataNames.T_StatoConsegna:
                    return @"Select * From T_StatoConsegna";

                case Enums.EnumDataNames.T_StatoConsegnaPaziente:
                    return @"Select * From T_StatoConsegnaPaziente";

                case Enums.EnumDataNames.T_StatoConsegnaPazienteRuoli:
                    return @"Select * From T_StatoConsegnaPazienteRuoli";

                case Enums.EnumDataNames.T_StatoConsensoCalcolato:
                    return @"Select * From T_StatoConsensoCalcolato";

                case Enums.EnumDataNames.T_StatoScheda:
                    return @"Select * From T_StatoScheda";

                case Enums.EnumDataNames.T_StatoSchedaCalcolato:
                    return @"Select * From T_StatoSchedaCalcolato";

                case Enums.EnumDataNames.T_Sistemi:
                    return @"Select * From T_Sistemi";

                case Enums.EnumDataNames.T_FormatoAllegati:
                    return @"Select * From T_FormatoAllegati";

                case Enums.EnumDataNames.T_TipoOrdine:
                    return @"Select * From T_TipoOrdine";

                case Enums.EnumDataNames.T_OEFormule:
                    return @"Select * From T_OEFormule";

                case Enums.EnumDataNames.T_OEAttributi:
                    return @"Select * From T_OEAttributi";

                case Enums.EnumDataNames.T_StatoOrdine:
                    return @"Select * From T_StatoOrdine";

                case Enums.EnumDataNames.T_StatoPrescrizioneTempi:
                    return @"Select * From T_StatoPrescrizioneTempi";

                case Enums.EnumDataNames.T_StatoContinuazione:
                    return @"Select * From T_StatoContinuazione";

                case Enums.EnumDataNames.T_StatoAppuntamentoAgende:
                    return @"Select * From T_StatoAppuntamentoAgende";

                case Enums.EnumDataNames.T_StatoAllegato:
                    return @"Select * From T_StatoAllegato";

                case Enums.EnumDataNames.T_StatoCartella:
                    return @"Select * From T_StatoCartella";

                case Enums.EnumDataNames.T_StatoCartellaInfo:
                    return @"Select * From T_StatoCartellaInfo";

                case Enums.EnumDataNames.T_StatoEpisodio:
                    return @"Select * From T_StatoEpisodio";

                case Enums.EnumDataNames.T_Protocolli:
                    return @"Select * From T_Protocolli";

                case Enums.EnumDataNames.T_ProtocolliTempi:
                    return @"Select * From T_ProtocolliTempi";

                case Enums.EnumDataNames.T_SezioniFUT:
                    return @"Select * From T_SezioniFUT";

                case Enums.EnumDataNames.T_StatoCartellaInVisione:
                    return @"Select * From T_StatoCartellaInVisione";

                case Enums.EnumDataNames.T_EBM:
                    return @"Select * From T_EBM";

                case Enums.EnumDataNames.T_ConfigPC:
                    return @"Select * From T_ConfigPC";

                case Enums.EnumDataNames.T_Contatori:
                    return @"Select * From T_Contatori";

                case Enums.EnumDataNames.T_FiltriSpeciali:
                    return @"Select * From T_FiltriSpeciali";

                case Enums.EnumDataNames.T_ProtocolliAttivita:
                case Enums.EnumDataNames.T_ProtocolliAttivitaTempi:
                case Enums.EnumDataNames.T_ProtocolliAttivitaTempiTipoTask:
                    return @"Select * From " + dn.ToString();

                case Enums.EnumDataNames.T_TipoSelezione:
                    return @"Select * From T_TipoSelezione";

                case Enums.EnumDataNames.T_Selezioni:
                    return @"Select * From T_Selezioni";

                case Enums.EnumDataNames.T_TipoFiltroSpeciale:
                    return @"Select * From T_TipoFiltroSpeciale";

                case Enums.EnumDataNames.T_AgendePeriodi:
                    return @"Select * From T_AgendePeriodi";

                case Enums.EnumDataNames.T_ModalitaCopiaPrecedente:
                    return @"Select * From T_ModalitaCopiaPrecedente";

                case Enums.EnumDataNames.T_Screen:
                    return @"Select * From T_Screen";

                case Enums.EnumDataNames.T_ScreenTile:
                    return @"Select * From T_ScreenTile";

                case Enums.EnumDataNames.T_ProtocolliPrescrizioni:
                    return @"SELECT * FROM T_ProtocolliPrescrizioni";

                case Enums.EnumDataNames.T_EntitaAllegato:
                    return @"Select * From T_EntitaAllegato";

                case Enums.EnumDataNames.T_AssUAIntestazioni:
                    return @"Select * From T_AssUAIntestazioni";

                default:
                    return "";

            }

        }

        internal static SortedList GetFieldsKey(Enums.EnumDataNames dn)
        {

            SortedList oSl = new SortedList();

            switch (dn)
            {

                case Enums.EnumDataNames.T_Agende:
                case Enums.EnumDataNames.T_TipoAgenda:
                case Enums.EnumDataNames.T_Login:
                case Enums.EnumDataNames.T_Moduli:
                case Enums.EnumDataNames.T_Ruoli:
                case Enums.EnumDataNames.T_Aziende:
                case Enums.EnumDataNames.T_UnitaAtomiche:
                case Enums.EnumDataNames.T_DiarioMedico:
                case Enums.EnumDataNames.T_DiarioInfermieristico:
                case Enums.EnumDataNames.T_TipoParametroVitale:
                case Enums.EnumDataNames.T_TipoConsegna:
                case Enums.EnumDataNames.T_TipoConsegnaPaziente:
                case Enums.EnumDataNames.T_TipoEvidenzaClinica:
                case Enums.EnumDataNames.T_Report:
                case Enums.EnumDataNames.T_Maschere:
                case Enums.EnumDataNames.T_FormatoReport:
                case Enums.EnumDataNames.T_TestiPredefiniti:
                case Enums.EnumDataNames.T_TestiNotePredefiniti:
                case Enums.EnumDataNames.T_ViaSomministrazione:
                case Enums.EnumDataNames.T_TipoPrescrizione:
                case Enums.EnumDataNames.T_StatoPrescrizione:
                case Enums.EnumDataNames.T_TipoAllegato:
                case Enums.EnumDataNames.T_UnitaOperative:
                case Enums.EnumDataNames.T_StatoEvidenzaClinica:
                case Enums.EnumDataNames.T_TipoAppuntamento:
                case Enums.EnumDataNames.T_StatoAppuntamento:
                case Enums.EnumDataNames.T_TipoTaskInfermieristico:
                case Enums.EnumDataNames.T_StatoTaskInfermieristico:
                case Enums.EnumDataNames.T_Schede:
                case Enums.EnumDataNames.T_TipoEpisodio:
                case Enums.EnumDataNames.T_StatoTrasferimento:
                case Enums.EnumDataNames.T_TipoDiario:
                case Enums.EnumDataNames.T_TipoScheda:
                case Enums.EnumDataNames.T_DCDecodifiche:
                case Enums.EnumDataNames.T_StatoDiario:
                case Enums.EnumDataNames.T_StatoEvidenzaClinicaVisione:
                case Enums.EnumDataNames.T_TipoAlertAllergiaAnamnesi:
                case Enums.EnumDataNames.T_TipoAlertGenerico:
                case Enums.EnumDataNames.T_StatoAlertGenerico:
                case Enums.EnumDataNames.T_StatoAlertAllergiaAnamnesi:
                case Enums.EnumDataNames.T_StatoParametroVitale:
                case Enums.EnumDataNames.T_StatoConsegna:
                case Enums.EnumDataNames.T_StatoConsegnaPaziente:
                case Enums.EnumDataNames.T_StatoConsegnaPazienteRuoli:
                case Enums.EnumDataNames.T_StatoConsensoCalcolato:
                case Enums.EnumDataNames.T_StatoScheda:
                case Enums.EnumDataNames.T_Sistemi:
                case Enums.EnumDataNames.T_FormatoAllegati:
                case Enums.EnumDataNames.T_TipoOrdine:
                case Enums.EnumDataNames.T_StatoOrdine:
                case Enums.EnumDataNames.T_StatoPrescrizioneTempi:
                case Enums.EnumDataNames.T_StatoContinuazione:
                case Enums.EnumDataNames.T_StatoAppuntamentoAgende:
                case Enums.EnumDataNames.T_StatoAllegato:
                case Enums.EnumDataNames.T_StatoCartella:
                case Enums.EnumDataNames.T_StatoCartellaInfo:
                case Enums.EnumDataNames.T_StatoEpisodio:
                case Enums.EnumDataNames.T_Protocolli:
                case Enums.EnumDataNames.T_ProtocolliTempi:
                case Enums.EnumDataNames.T_SezioniFUT:
                case Enums.EnumDataNames.T_CDSSAzioni:
                case Enums.EnumDataNames.T_CDSSPlugins:
                case Enums.EnumDataNames.T_StatoCartellaInVisione:
                case Enums.EnumDataNames.T_EBM:
                case Enums.EnumDataNames.T_Integra_Destinatari:
                case Enums.EnumDataNames.T_Integra_Campi:
                case Enums.EnumDataNames.T_Contatori:
                case Enums.EnumDataNames.T_FiltriSpeciali:
                case Enums.EnumDataNames.T_ProtocolliAttivita:
                case Enums.EnumDataNames.T_ProtocolliAttivitaTempi:
                case Enums.EnumDataNames.T_Selezioni:
                case Enums.EnumDataNames.T_TipoSelezione:
                case Enums.EnumDataNames.T_TipoFiltroSpeciale:
                case Enums.EnumDataNames.T_Screen:
                case Enums.EnumDataNames.T_ProtocolliPrescrizioni:
                case Enums.EnumDataNames.T_EntitaAllegato:
                    oSl.Add("Codice", "S");
                    break;

                case Enums.EnumDataNames.T_OEFormule:
                case Enums.EnumDataNames.T_OEAttributi:
                    oSl.Add("ID", "N");
                    break;

                case Enums.EnumDataNames.T_StatoSchedaCalcolato:
                    oSl.Add("CodScheda", "S");
                    oSl.Add("Codice", "S");
                    break;

                case Enums.EnumDataNames.T_AgendePeriodi:
                    oSl.Add("CodAgenda", "S");
                    oSl.Add("DataInizio", "D");
                    oSl.Add("DataFine", "D");
                    break;

                case Enums.EnumDataNames.T_CDSSStruttura:
                case Enums.EnumDataNames.T_CDSSStrutturaRuoli:
                    oSl.Add("ID", "N");
                    break;

                case Enums.EnumDataNames.T_MovNews:
                    oSl.Add("Codice", "S");
                    break;

                case Enums.EnumDataNames.T_Settori:
                case Enums.EnumDataNames.T_Stanze:
                    oSl.Add("CodAzi", "S");
                    oSl.Add("Codice", "S");
                    break;

                case Enums.EnumDataNames.T_ConfigPC:
                    oSl.Add("CodPC", "S");
                    break;

                case Enums.EnumDataNames.T_Letti:
                    oSl.Add("CodAzi", "S");
                    oSl.Add("CodLetto", "S");
                    oSl.Add("CodSettore", "S");
                    break;

                case Enums.EnumDataNames.T_SchedeVersioni:
                    oSl.Add("CodScheda", "S");
                    oSl.Add("Versione", "N");
                    break;

                case Enums.EnumDataNames.T_DCDecodificheValori:
                    oSl.Add("CodDec", "S");
                    oSl.Add("Codice", "S");
                    break;

                case Enums.EnumDataNames.T_AssUAUOLetti:
                    oSl.Add("CodUA", "S");
                    oSl.Add("CodAzi", "S");
                    oSl.Add("CodUO", "S");
                    oSl.Add("CodSettore", "S");
                    oSl.Add("CodLetto", "S");
                    break;

                case Enums.EnumDataNames.T_ProtocolliAttivitaTempiTipoTask:
                    oSl.Add("CodProtocolloAttivitaTempi", "S");
                    oSl.Add("CodTipoTaskInfermieristico", "S");
                    break;

                case Enums.EnumDataNames.T_Festivita:
                    oSl.Add("Data", "D");
                    break;

                case Enums.EnumDataNames.T_FestivitaAgende:
                    oSl.Add("Data", "D");
                    oSl.Add("CodAgenda", "S");
                    break;

                case Enums.EnumDataNames.T_ScreenTile:
                    oSl.Add("ID", "N");
                    break;

                case Enums.EnumDataNames.T_AssUAIntestazioni:
                    oSl.Add("CodUA", "S");
                    oSl.Add("CodIntestazione", "S");
                    oSl.Add("DataInizio", "T");
                    break;

                default:
                    break;
            }

            return oSl;

        }

        internal static string GetSqlPUDelete(Enums.EnumDataNames dn)
        {

            switch (dn)
            {

                case Enums.EnumDataNames.T_Login:
                    return "Delete From T_Login";

                case Enums.EnumDataNames.T_Moduli:
                    return "Delete From T_Moduli";

                case Enums.EnumDataNames.T_Ruoli:
                    return @"Delete From T_Ruoli";

                case Enums.EnumDataNames.T_Aziende:
                    return @"Delete From T_Aziende";

                case Enums.EnumDataNames.T_UnitaAtomiche:
                    return @"Delete From T_UnitaAtomiche";

                case Enums.EnumDataNames.T_UnitaOperative:
                    return @"Delete From T_UnitaOperative";

                case Enums.EnumDataNames.T_DiarioInfermieristico:
                case Enums.EnumDataNames.T_DiarioMedico:
                    return @"Delete From T_TipoVoceDiario";

                case Enums.EnumDataNames.T_TipoParametroVitale:
                    return @"Delete From T_TipoParametroVitale";

                case Enums.EnumDataNames.T_TipoConsegna:
                    return @"Delete From T_TipoConsegna";

                case Enums.EnumDataNames.T_TipoConsegnaPaziente:
                    return @"Delete From T_TipoConsegnaPaziente";

                case Enums.EnumDataNames.T_TipoEvidenzaClinica:
                    return @"Delete From T_TipoEvidenzaClinica";

                case Enums.EnumDataNames.T_Report:
                    return @"Delete From T_Report";

                case Enums.EnumDataNames.T_Maschere:
                    return @"Delete From T_Maschere";

                case Enums.EnumDataNames.T_FormatoReport:
                    return @"Delete From T_FormatoReport";

                case Enums.EnumDataNames.T_TestiPredefiniti:
                    return @"Delete From T_TestiPredefiniti";

                case Enums.EnumDataNames.T_TestiNotePredefiniti:
                    return @"Delete From T_TestiNotePredefiniti";

                case Enums.EnumDataNames.T_TipoAllegato:
                    return @"Delete From T_TipoAllegato";

                case Enums.EnumDataNames.T_TipoPrescrizione:
                    return @"Delete From T_TipoPrescrizione";

                case Enums.EnumDataNames.T_StatoPrescrizione:
                    return @"Delete From T_StatoPrescrizione";

                case Enums.EnumDataNames.T_ViaSomministrazione:
                    return @"Delete From T_ViaSomministrazione";

                case Enums.EnumDataNames.T_MovNews:
                    return @"Delete From T_MovNews";

                case Enums.EnumDataNames.T_StatoEvidenzaClinica:
                    return @"Delete From T_StatoEvidenzaClinica";

                case Enums.EnumDataNames.T_TipoAppuntamento:
                    return @"Delete From T_TipoAppuntamento";

                case Enums.EnumDataNames.T_TipoTaskInfermieristico:
                    return @"Delete From T_TipoTaskInfermieristico";

                case Enums.EnumDataNames.T_StatoTaskInfermieristico:
                    return @"Delete From T_StatoTaskInfermieristico";

                case Enums.EnumDataNames.T_Schede:
                    return @"Delete From T_Schede";

                case Enums.EnumDataNames.T_Stanze:
                    return @"Delete From T_Stanze";

                case Enums.EnumDataNames.T_SchedeVersioni:
                    return @"Delete From T_SchedeVersioni";

                case Enums.EnumDataNames.T_TipoEpisodio:
                    return @"Delete From T_TipoEpisodio";

                case Enums.EnumDataNames.T_StatoTrasferimento:
                    return @"Delete From T_StatoTrasferimento";

                case Enums.EnumDataNames.T_TipoDiario:
                    return @"Delete From T_TipoDiario";

                case Enums.EnumDataNames.T_Letti:
                    return @"Delete From T_Letti";

                case Enums.EnumDataNames.T_Settori:
                    return @"Delete From T_Settori";

                case Enums.EnumDataNames.T_TipoScheda:
                    return @"Delete From T_TipoScheda";

                case Enums.EnumDataNames.T_DCDecodifiche:
                    return @"Delete From T_DCDecodifiche";

                case Enums.EnumDataNames.T_DCDecodificheValori:
                    return @"Delete From T_DCDecodificheValori";

                case Enums.EnumDataNames.T_AssUAUOLetti:
                    return @"Delete From T_AssUAUOLetti";

                case Enums.EnumDataNames.T_CDSSAzioni:
                    return @"Delete From T_CDSSAzioni";

                case Enums.EnumDataNames.T_CDSSPlugins:
                    return @"Delete From T_CDSSPlugins";

                case Enums.EnumDataNames.T_CDSSStruttura:
                    return @"Delete From T_CDSSStruttura";

                case Enums.EnumDataNames.T_CDSSStrutturaRuoli:
                    return @"Delete From T_CDSSStrutturaRuoli";

                case Enums.EnumDataNames.T_Integra_Destinatari:
                    return @"Delete From T_Integra_Destinatari";

                case Enums.EnumDataNames.T_Integra_Campi:
                    return @"Delete From T_Integra_Campi";

                case Enums.EnumDataNames.T_TipoAgenda:
                    return @"Delete From T_TipoAgenda";

                case Enums.EnumDataNames.T_Festivita:
                    return @"Delete From T_Festivita";

                case Enums.EnumDataNames.T_FestivitaAgende:
                    return @"Delete From T_FestivitaAgende";

                case Enums.EnumDataNames.T_Agende:
                    return @"Delete From T_Agende";

                case Enums.EnumDataNames.T_StatoDiario:
                    return @"Delete From T_StatoDiario";

                case Enums.EnumDataNames.T_StatoEvidenzaClinicaVisione:
                    return @"Delete from T_StatoEvidenzaClinicaVisione";

                case Enums.EnumDataNames.T_TipoAlertAllergiaAnamnesi:
                    return @"Delete From T_TipoAlertAllergiaAnamnesi";

                case Enums.EnumDataNames.T_TipoAlertGenerico:
                    return @"Delete From T_TipoAlertGenerico";

                case Enums.EnumDataNames.T_StatoAlertGenerico:
                    return @"Delete From T_StatoAlertGenerico";

                case Enums.EnumDataNames.T_StatoAlertAllergiaAnamnesi:
                    return @"Delete From T_StatoAlertAllergiaAnamnesi";

                case Enums.EnumDataNames.T_StatoParametroVitale:
                    return @"Delete From T_StatoParametroVitale";

                case Enums.EnumDataNames.T_StatoConsegna:
                    return @"Delete From T_StatoConsegna";

                case Enums.EnumDataNames.T_StatoConsegnaPaziente:
                    return @"Delete From T_StatoConsegnaPaziente";

                case Enums.EnumDataNames.T_StatoConsegnaPazienteRuoli:
                    return @"Delete From T_StatoConsegnaPazienteRuoli";

                case Enums.EnumDataNames.T_StatoConsensoCalcolato:
                    return @"Delete From T_StatoConsensoCalcolato";

                case Enums.EnumDataNames.T_StatoScheda:
                    return @"Delete From T_StatoScheda";

                case Enums.EnumDataNames.T_StatoSchedaCalcolato:
                    return @"Delete From T_StatoSchedaCalcolato";

                case Enums.EnumDataNames.T_Sistemi:
                    return @"Delete From T_Sistemi";

                case Enums.EnumDataNames.T_FormatoAllegati:
                    return @"Delete From T_FormatoAllegati";

                case Enums.EnumDataNames.T_TipoOrdine:
                    return @"Delete From T_TipoOrdine";

                case Enums.EnumDataNames.T_OEFormule:
                    return @"Delete From T_OEFormule";

                case Enums.EnumDataNames.T_OEAttributi:
                    return @"Delete From T_OEAttributi";

                case Enums.EnumDataNames.T_StatoOrdine:
                    return @"Delete From T_StatoOrdine";

                case Enums.EnumDataNames.T_StatoPrescrizioneTempi:
                    return @"Delete From T_StatoPrescrizioneTempi";

                case Enums.EnumDataNames.T_StatoContinuazione:
                    return @"Delete From T_StatoContinuazione";

                case Enums.EnumDataNames.T_StatoAppuntamentoAgende:
                    return @"Delete From T_StatoAppuntamentoAgende";

                case Enums.EnumDataNames.T_StatoAllegato:
                    return @"Delete From T_StatoAllegato";

                case Enums.EnumDataNames.T_StatoCartella:
                    return @"Delete From T_StatoCartella";

                case Enums.EnumDataNames.T_StatoCartellaInfo:
                    return @"Delete From T_StatoCartellaInfo";

                case Enums.EnumDataNames.T_StatoEpisodio:
                    return @"Delete From T_StatoEpisodio";

                case Enums.EnumDataNames.T_Protocolli:
                    return @"Delete From T_Protocolli";

                case Enums.EnumDataNames.T_ProtocolliTempi:
                    return @"Delete From T_ProtocolliTempi";

                case Enums.EnumDataNames.T_SezioniFUT:
                    return @"Delete  From T_SezioniFUT";

                case Enums.EnumDataNames.T_StatoCartellaInVisione:
                    return @"Delete From T_StatoCartellaInVisione";

                case Enums.EnumDataNames.T_EBM:
                    return @"Delete From T_EBM";

                case Enums.EnumDataNames.T_ConfigPC:
                    return @"Delete From T_ConfigPC";

                case Enums.EnumDataNames.T_Contatori:
                    return @"Delete From T_Contatori";

                case Enums.EnumDataNames.T_FiltriSpeciali:
                    return @"Delete From T_FiltriSpeciali";

                case Enums.EnumDataNames.T_ProtocolliAttivita:
                case Enums.EnumDataNames.T_ProtocolliAttivitaTempi:
                case Enums.EnumDataNames.T_ProtocolliAttivitaTempiTipoTask:
                    return @"Delete From " + dn.ToString();

                case Enums.EnumDataNames.T_Selezioni:
                    return @"Delete From T_Selezioni";

                case Enums.EnumDataNames.T_TipoSelezione:
                    return @"Delete From T_TipoSelezione";

                case Enums.EnumDataNames.T_TipoFiltroSpeciale:
                    return @"Delete From T_TipoFiltroSpeciale";

                case Enums.EnumDataNames.T_AgendePeriodi:
                    return @"Delete From T_AgendePeriodi";

                case Enums.EnumDataNames.T_Screen:
                    return @"Delete From T_Screen";

                case Enums.EnumDataNames.T_ScreenTile:
                    return @"Delete From T_ScreenTile";

                case Enums.EnumDataNames.T_ProtocolliPrescrizioni:
                    return @"Delete FROM T_ProtocolliPrescrizioni";

                case Enums.EnumDataNames.T_EntitaAllegato:
                    return @"Delete From T_EntitaAllegato";

                case Enums.EnumDataNames.T_AssUAIntestazioni:
                    return @"Delete From T_AssUAIntestazioni";

                default:
                    return "";

            }

        }

        internal static void GetDefultValues(ref DataRow dr, Enums.EnumDataNames dn)
        {

            switch (dn)
            {

                case Enums.EnumDataNames.T_Login:
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    dr["Note"] = "";
                    dr["FlagAdmin"] = 0;
                    dr["FlagObsoleto"] = 0;
                    break;

                case Enums.EnumDataNames.T_Moduli:
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    dr["Note"] = "";
                    dr["Path"] = "";
                    break;

                case Enums.EnumDataNames.T_Ruoli:
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    dr["Note"] = "";
                    break;

                case Enums.EnumDataNames.T_TipoAllegato:
                case Enums.EnumDataNames.T_ViaSomministrazione:
                case Enums.EnumDataNames.T_FormatoReport:
                case Enums.EnumDataNames.T_Maschere:
                case Enums.EnumDataNames.T_Aziende:
                case Enums.EnumDataNames.T_DCDecodifiche:
                case Enums.EnumDataNames.T_TipoAlertAllergiaAnamnesi:
                case Enums.EnumDataNames.T_TipoAlertGenerico:
                case Enums.EnumDataNames.T_Agende:
                case Enums.EnumDataNames.T_CDSSAzioni:
                case Enums.EnumDataNames.T_CDSSPlugins:
                case Enums.EnumDataNames.T_TipoSelezione:
                case Enums.EnumDataNames.T_TipoFiltroSpeciale:
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    break;

                case Enums.EnumDataNames.T_CDSSStruttura:
                    dr["CodUA"] = "";
                    dr["CodAzione"] = "";
                    dr["CodPlugin"] = "";
                    dr["Parametri"] = "";
                    break;

                case Enums.EnumDataNames.T_CDSSStrutturaRuoli:
                    dr["CodRuolo"] = "";
                    dr["CodAzione"] = "";
                    dr["CodPlugin"] = "";
                    dr["Parametri"] = "";
                    break;

                case Enums.EnumDataNames.T_Integra_Destinatari:
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    dr["Indirizzo"] = "";
                    dr["Dominio"] = "";
                    dr["Utente"] = "";
                    dr["Password"] = "";
                    dr["Note"] = "";
                    dr["Https"] = 0;
                    break;

                case Enums.EnumDataNames.T_Integra_Campi:
                    dr["Codice"] = "";
                    dr["CodEntita"] = "";
                    dr["CodTipoEntita"] = "";
                    dr["Campo"] = "";
                    dr["Note"] = "";
                    break;

                case Enums.EnumDataNames.T_Protocolli:
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    dr["Continuita"] = false;
                    dr["Durata"] = 0;
                    break;

                case Enums.EnumDataNames.T_ProtocolliTempi:
                    dr["Codice"] = "";
                    dr["CodProtocollo"] = "";
                    dr["Descrizione"] = "";
                    dr["Delta"] = 0;
                    dr["Ora"] = DateTime.MinValue;
                    break;

                case Enums.EnumDataNames.T_UnitaAtomiche:
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    dr["Note"] = "";
                    dr["CodPadre"] = "";
                    break;

                case Enums.EnumDataNames.T_UnitaOperative:
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    dr["CodAzi"] = "";
                    break;

                case Enums.EnumDataNames.T_Settori:
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    dr["CodAzi"] = "";
                    break;

                case Enums.EnumDataNames.T_DiarioMedico:
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    dr["CodTipoDiario"] = "M";
                    dr["CodScheda"] = "";
                    break;

                case Enums.EnumDataNames.T_DiarioInfermieristico:
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    dr["CodTipoDiario"] = "I";
                    dr["CodScheda"] = "";
                    break;

                case Enums.EnumDataNames.T_TipoParametroVitale:
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    dr["Icona"] = null;
                    dr["Colore"] = "";
                    dr["CampiFUT"] = "";
                    dr["CampiGrafici"] = "";
                    dr["CodScheda"] = "";
                    break;

                case Enums.EnumDataNames.T_TipoConsegna:
                case Enums.EnumDataNames.T_TipoConsegnaPaziente:
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    dr["Icona"] = null;
                    dr["Colore"] = "";
                    dr["CodScheda"] = "";
                    break;

                case Enums.EnumDataNames.T_TipoEvidenzaClinica:
                case Enums.EnumDataNames.T_TipoOrdine:
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    dr["Icona"] = null;
                    break;

                case Enums.EnumDataNames.T_OEFormule:
                    dr["CodUA"] = "";
                    dr["CodAzienda"] = "";
                    dr["CodErogante"] = "";
                    dr["CodPrestazione"] = "";
                    dr["CodDatoAccessorio"] = "";
                    dr["Formula"] = "";
                    break;

                case Enums.EnumDataNames.T_OEAttributi:
                    dr["CodEntita"] = "";
                    dr["CodSistemaRichiedente"] = "";
                    dr["CodAgendaRichiedente"] = "";
                    dr["MappaturaOE"] = "<Parametri></Parametri>";
                    dr["Note"] = "";
                    break;

                case Enums.EnumDataNames.T_Report:
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    dr["CodFormatoReport"] = "CAB";
                    dr["DaStoricizzare"] = false;
                    dr["Path"] = "";
                    dr["Note"] = "";
                    dr["ApriBrowser"] = false;
                    dr["ApriIE"] = false;
                    break;

                case Enums.EnumDataNames.T_TestiPredefiniti:
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    RtfFiles rtf = new RtfFiles();
                    System.Drawing.Font f = rtf.getFontFromString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoRTF), false, false);
                    dr["TestoRTF"] = rtf.initRtf(f);
                    rtf = null;
                    dr["Path"] = "";
                    dr["CodEntita"] = "";
                    break;

                case Enums.EnumDataNames.T_TestiNotePredefiniti:
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    dr["OggettoNota"] = "";
                    dr["DescrizioneNota"] = "";
                    dr["Path"] = "";
                    dr["Colore"] = "";
                    break;

                case Enums.EnumDataNames.T_MovNews:
                    dr["Codice"] = "";
                    dr["DataOra"] = DBNull.Value;
                    dr["DataInizioPubblicazione"] = DBNull.Value;
                    dr["DataFinePubblicazione"] = DBNull.Value;
                    dr["Rilevante"] = false;
                    dr["TestoRTF"] = "";
                    break;

                case Enums.EnumDataNames.T_TipoAppuntamento:
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    dr["Colore"] = "";
                    dr["Icona"] = null;
                    dr["CodScheda"] = "";
                    dr["TimeSlotInterval"] = 0;
                    break;

                case Enums.EnumDataNames.T_TipoPrescrizione:
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    dr["Icona"] = null;
                    dr["CodScheda"] = "";
                    dr["PrescrizioneASchema"] = false;
                    dr["CodSchedaPosologia"] = "";
                    break;

                case Enums.EnumDataNames.T_StatoAppuntamento:
                case Enums.EnumDataNames.T_StatoAppuntamentoAgende:
                case Enums.EnumDataNames.T_StatoAllegato:
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    dr["Colore"] = "";
                    dr["Ordine"] = 0;
                    dr["Icona"] = null;
                    break;

                case Enums.EnumDataNames.T_TipoTaskInfermieristico:
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    dr["Colore"] = "";
                    dr["Icona"] = null;
                    dr["CodScheda"] = "";
                    dr["Anticipo"] = 0;
                    break;

                case Enums.EnumDataNames.T_StatoTaskInfermieristico:
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    dr["Colore"] = "";
                    dr["Icona"] = null;
                    dr["Visibile"] = false;
                    break;

                case Enums.EnumDataNames.T_Schede:
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    dr["SchedaSemplice"] = false;
                    dr["EsportaDWH"] = false;
                    dr["EsportaDWHSingola"] = false;
                    dr["IgnoraStampaCartella"] = false;
                    dr["CodModalitaCopiaPrecedente"] = "CREA";
                    break;

                case Enums.EnumDataNames.T_Stanze:
                    dr["CodAzi"] = "";
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    break;

                case Enums.EnumDataNames.T_SchedeVersioni:
                    dr["CodScheda"] = "";
                    dr["Versione"] = 0;
                    dr["Descrizione"] = "";
                    break;

                case Enums.EnumDataNames.T_StatoEvidenzaClinica:
                case Enums.EnumDataNames.T_StatoPrescrizione:
                case Enums.EnumDataNames.T_StatoDiario:
                case Enums.EnumDataNames.T_StatoEvidenzaClinicaVisione:
                case Enums.EnumDataNames.T_StatoAlertGenerico:
                case Enums.EnumDataNames.T_StatoAlertAllergiaAnamnesi:
                case Enums.EnumDataNames.T_StatoParametroVitale:
                case Enums.EnumDataNames.T_StatoConsegna:
                case Enums.EnumDataNames.T_StatoConsegnaPaziente:
                case Enums.EnumDataNames.T_StatoConsegnaPazienteRuoli:
                case Enums.EnumDataNames.T_StatoScheda:
                case Enums.EnumDataNames.T_Sistemi:
                case Enums.EnumDataNames.T_FormatoAllegati:
                case Enums.EnumDataNames.T_StatoOrdine:
                case Enums.EnumDataNames.T_StatoPrescrizioneTempi:
                case Enums.EnumDataNames.T_StatoContinuazione:
                case Enums.EnumDataNames.T_StatoCartella:
                case Enums.EnumDataNames.T_StatoCartellaInfo:
                case Enums.EnumDataNames.T_StatoEpisodio:
                case Enums.EnumDataNames.T_StatoCartellaInVisione:
                case Enums.EnumDataNames.T_ProtocolliAttivita:
                case Enums.EnumDataNames.T_EntitaAllegato:
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    dr["Colore"] = "";
                    dr["Icona"] = null;
                    break;

                case Enums.EnumDataNames.T_StatoSchedaCalcolato:
                    dr["CodScheda"] = "";
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    dr["Colore"] = "";
                    dr["Icona"] = null;
                    break;

                case Enums.EnumDataNames.T_StatoTrasferimento:
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    dr["Colore"] = "";
                    dr["Icona"] = null;
                    dr["Ordine"] = 0;
                    break;

                case Enums.EnumDataNames.T_Contatori:
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    dr["Valore"] = 0;
                    break;

                case Enums.EnumDataNames.T_FiltriSpeciali:
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    dr["SQL"] = "";
                    break;

                case Enums.EnumDataNames.T_TipoEpisodio:
                case Enums.EnumDataNames.T_TipoDiario:
                case Enums.EnumDataNames.T_TipoScheda:
                case Enums.EnumDataNames.T_TipoAgenda:
                case Enums.EnumDataNames.T_StatoConsensoCalcolato:
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    dr["Icona"] = null;
                    break;

                case Enums.EnumDataNames.T_Festivita:
                    dr["Data"] = DateTime.Now.Date;
                    dr["Descrizione"] = "";
                    break;

                case Enums.EnumDataNames.T_FestivitaAgende:
                    dr["Data"] = DateTime.Now.Date;
                    dr["CodAgenda"] = "";
                    break;

                case Enums.EnumDataNames.T_Letti:
                    dr["CodAzi"] = "";
                    dr["CodLetto"] = "";
                    dr["CodSettore"] = "";
                    dr["Descrizione"] = "";
                    dr["CodStanza"] = "";
                    break;

                case Enums.EnumDataNames.T_DCDecodificheValori:
                    dr["CodDec"] = "";
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    dr["Ordine"] = 0;
                    dr["DtValI"] = DateTime.Now;
                    dr["DtValF"] = DBNull.Value;
                    break;

                case Enums.EnumDataNames.T_AssUAUOLetti:
                    dr["CodUA"] = "";
                    dr["CodAzi"] = "";
                    dr["CodUO"] = "";
                    dr["CodSettore"] = "";
                    dr["CodLetto"] = "";
                    break;

                case Enums.EnumDataNames.T_SezioniFUT:
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    dr["Colore"] = "";
                    dr["Icona"] = null;
                    dr["Ordine"] = 0;
                    dr["CodEntita"] = "";
                    break;

                case Enums.EnumDataNames.T_EBM:
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    dr["Note"] = "";
                    dr["Url"] = "";
                    dr["Ordine"] = 0;
                    break;

                case Enums.EnumDataNames.T_ConfigPC:
                    dr["CodPC"] = "";
                    dr["Codice"] = "";
                    dr["Descrizione"] = "";
                    break;

                case Enums.EnumDataNames.T_ProtocolliAttivitaTempi:
                    dr["Codice"] = "";
                    dr["CodProtocolloAttivita"] = "";
                    dr["Descrizione"] = "";
                    dr["DeltaGiorni"] = 0;
                    dr["DeltaOre"] = 0;
                    dr["DeltaMinuti"] = 0;
                    break;

                case Enums.EnumDataNames.T_ProtocolliAttivitaTempiTipoTask:
                    dr["CodProtocolloAttivitaTempi"] = "";
                    dr["CodTipoTaskInfermieristico"] = "";
                    dr["Ordine"] = 0;

                    break;

                case Enums.EnumDataNames.T_Selezioni:
                    dr["Codice"] = DateTime.Now.ToString(@"yyyyMMddHHmmssfff") + "SYS";
                    dr["Descrizione"] = "";
                    dr["CodTipoSelezione"] = "";
                    dr["FlagSistema"] = true;
                    dr["CodUtenteInserimento"] = Environment.UserDomainName + @"\" + Environment.UserName;
                    dr["CodRuoloInserimento"] = "";
                    dr["DataInserimento"] = DateTime.Now;
                    dr["DataInserimentoUTC"] = DateTime.Now.ToUniversalTime();
                    dr["CodUtenteUltimaModifica"] = Environment.UserDomainName + @"\" + Environment.UserName;
                    dr["CodRuoloUltimaModifica"] = "";
                    dr["DataUltimaModifica"] = DateTime.Now;
                    dr["DataUltimaModificaUTC"] = DateTime.Now.ToUniversalTime();

                    break;

                case Enums.EnumDataNames.T_AgendePeriodi:
                    dr["CodAgenda"] = "";
                    dr["DataInizio"] = DateTime.Now.Date;
                    dr["DataFine"] = DateTime.Now.Date;
                    break;

                case Enums.EnumDataNames.T_Screen:
                    dr["Codice"] = "";
                    dr["Descrizione"] = string.Empty;
                    dr["Attributi"] = null;
                    dr["Righe"] = 0;
                    dr["Colonne"] = 0;
                    dr["CodTipoScreen"] = string.Empty;
                    dr["AltezzaRigaGrid"] = 0;
                    dr["LarghezzaColonnaGrid"] = 0;
                    dr["CaricaPerRiga"] = false;
                    dr["AdattaAltezzaRighe"] = false;
                    dr["Predefinito"] = false;
                    break;

                case Enums.EnumDataNames.T_ScreenTile:
                    dr["CodScreen"] = string.Empty;
                    dr["Riga"] = 0;
                    dr["Colonna"] = 0;
                    dr["Altezza"] = 0;
                    dr["Larghezza"] = 0;
                    dr["InEvidenza"] = false;
                    dr["CodPlugin"] = string.Empty;
                    dr["Attributi"] = null;
                    dr["NomeTile"] = string.Empty;
                    dr["Fissa"] = false;
                    dr["NonCollassabile"] = false;
                    dr["Collassata"] = false;
                    break;

                case Enums.EnumDataNames.T_ProtocolliPrescrizioni:
                    dr["Codice"] = string.Empty;
                    dr["Descrizione"] = string.Empty;
                    dr["Colore"] = string.Empty;
                    dr["Icona"] = null;
                    dr["ModelliPrescrizioni"] = null;
                    dr["DataOraInizioObbligatoria"] = false;
                    break;

                case Enums.EnumDataNames.T_AssUAIntestazioni:
                    dr["CodUA"] = "";
                    dr["CodIntestazione"] = "";
                    dr["DataInizio"] = DateTime.Now.Date;
                    break;

                default:
                    break;

            }

        }

        internal static void SetDataBinding(ref PUDataBindings ViewDataBindings,
                                            Enums.EnumDataNames DataName, Enums.EnumModalityPopUp Modality,
                                            ref Infragistics.Win.UltraWinGrid.UltraGridRow UltraRow)
        {

            try
            {

                ViewDataBindings.SqlSelect.Sql = GetSqlPUView(DataName);
                switch (Modality)
                {

                    case Enums.EnumModalityPopUp.mpNuovo:
                        ViewDataBindings.SqlSelect.Where = @"0=1";
                        break;

                    default:
                        SortedList oSl = GetFieldsKey(DataName);
                        for (int i = 0; i < oSl.Count; i++)
                        {

                            if (ViewDataBindings.SqlSelect.Where != "") ViewDataBindings.SqlSelect.Where += @" And ";
                            ViewDataBindings.SqlSelect.Where += oSl.GetKey(i).ToString() + @" = ";

                            switch (oSl.GetByIndex(i).ToString())
                            {

                                case "S":
                                    ViewDataBindings.SqlSelect.Where += @"'" + Ax2(UltraRow.Cells[oSl.GetKey(i).ToString()].Text) + @"'";
                                    break;

                                case "N":
                                    if (UltraRow.Cells[oSl.GetKey(i).ToString()].Text.Trim() == "")
                                        ViewDataBindings.SqlSelect.Where += @"''";
                                    else
                                        ViewDataBindings.SqlSelect.Where += UltraRow.Cells[oSl.GetKey(i).ToString()].Text;
                                    break;

                                case "D":
                                    ViewDataBindings.SqlSelect.Where += SQLDate((DateTime)UltraRow.Cells[oSl.GetKey(i).ToString()].Value);
                                    break;

                                case "G":
                                    ViewDataBindings.SqlSelect.Where += @"'" + UltraRow.Cells[oSl.GetKey(i).ToString()].Value.ToString() + @"'";
                                    break;

                                default:
                                    break;

                            }
                        }
                        break;
                }
                ViewDataBindings.SqlDelete.Sql = GetSqlPUDelete(DataName);
                ViewDataBindings.SqlDelete.Where = ViewDataBindings.SqlSelect.Where;
                ViewDataBindings.DataBindings.DataSet = DataBase.GetDataSet(ViewDataBindings.SqlSelect.Sql);
                ViewDataBindings.DataBindings.DatasourceType = Sys.Data2008.DS_TYPE.SqlServer2005;

                if (Modality == Enums.EnumModalityPopUp.mpNuovo)
                {
                    DataRow _dr = ViewDataBindings.DataBindings.DataSet.Tables[0].NewRow();
                    GetDefultValues(ref _dr, DataName);
                    ViewDataBindings.DataBindings.DataSet.Tables[0].Rows.Add(_dr);
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        internal static void SetDataBinding(ref PUDataBindings ViewDataBindings,
                                            Enums.EnumDataNames DataName, Enums.EnumModalityPopUp Modality,
                                            params string[] vsCodiciRecord)
        {

            try
            {

                ViewDataBindings.SqlSelect.Sql = GetSqlPUView(DataName);
                switch (Modality)
                {

                    case Enums.EnumModalityPopUp.mpNuovo:
                        ViewDataBindings.SqlSelect.Where = @"0=1";
                        break;

                    default:
                        SortedList oSl = GetFieldsKey(DataName);
                        for (int i = 0; i < oSl.Count; i++)
                        {

                            if (ViewDataBindings.SqlSelect.Where != "") ViewDataBindings.SqlSelect.Where += @" And ";
                            ViewDataBindings.SqlSelect.Where += oSl.GetKey(i).ToString() + @" = ";

                            switch (oSl.GetByIndex(i).ToString())
                            {

                                case "S":
                                    ViewDataBindings.SqlSelect.Where += @"'" + Ax2(vsCodiciRecord[i]) + @"'";
                                    break;

                                case "N":
                                    if (vsCodiciRecord[i].Trim() == "")
                                        ViewDataBindings.SqlSelect.Where += @"''";
                                    else
                                        ViewDataBindings.SqlSelect.Where += vsCodiciRecord[i];
                                    break;

                                case "D":
                                    if (vsCodiciRecord[i].Trim() == "")
                                        ViewDataBindings.SqlSelect.Where += "Null";
                                    else
                                    {
                                        DateTime dt;
                                        if (DateTime.TryParse(vsCodiciRecord[i], out dt))
                                            ViewDataBindings.SqlSelect.Where += SQLDate(dt);
                                        else
                                            ViewDataBindings.SqlSelect.Where += "Null";

                                    }
                                    break;

                                case "T":
                                    if (vsCodiciRecord[i].Trim() == "")
                                        ViewDataBindings.SqlSelect.Where += "Null";
                                    else
                                    {
                                        DateTime dt = new DateTime(long.Parse(vsCodiciRecord[i]));
                                        ViewDataBindings.SqlSelect.Where += SQLDateTime(dt);
                                    }
                                    break;

                                case "G":
                                    ViewDataBindings.SqlSelect.Where += @"'" + vsCodiciRecord[i] + @"'";
                                    break;

                                default:
                                    break;

                            }
                        }
                        break;
                }
                ViewDataBindings.SqlDelete.Sql = GetSqlPUDelete(DataName);
                ViewDataBindings.SqlDelete.Where = ViewDataBindings.SqlSelect.Where;
                ViewDataBindings.DataBindings.DataSet = DataBase.GetDataSet(ViewDataBindings.SqlSelect.Sql);
                ViewDataBindings.DataBindings.DatasourceType = Sys.Data2008.DS_TYPE.SqlServer2005;

                if (Modality == Enums.EnumModalityPopUp.mpNuovo)
                {
                    DataRow _dr = ViewDataBindings.DataBindings.DataSet.Tables[0].NewRow();
                    GetDefultValues(ref _dr, DataName);
                    SortedList oSl = null;
                    switch (DataName)
                    {

                        case Enums.EnumDataNames.T_Schede:
                        case Enums.EnumDataNames.T_TestiPredefiniti:
                        case Enums.EnumDataNames.T_TestiNotePredefiniti:
                        case Enums.EnumDataNames.T_Report:
                            oSl = GetFieldsKey(DataName);
                            if (vsCodiciRecord.Length > oSl.Count)
                            {
                                _dr["Path"] = vsCodiciRecord[oSl.Count];
                            }
                            break;

                        case Enums.EnumDataNames.T_UnitaAtomiche:
                            oSl = GetFieldsKey(DataName);
                            if (vsCodiciRecord.Length > oSl.Count)
                            {
                                _dr["CodPadre"] = vsCodiciRecord[oSl.Count];
                            }
                            break;

                        case Enums.EnumDataNames.T_SchedeVersioni:
                            oSl = GetFieldsKey(DataName);
                            if (vsCodiciRecord.Length > oSl.Count)
                            {
                                _dr["CodScheda"] = vsCodiciRecord[oSl.Count];
                            }
                            break;

                        case Enums.EnumDataNames.T_AssUAIntestazioni:
                            _dr["CodUA"] = vsCodiciRecord[0];
                            _dr["CodIntestazione"] = vsCodiciRecord[1];
                            break;

                        default:
                            break;

                    }
                    ViewDataBindings.DataBindings.DataSet.Tables[0].Rows.Add(_dr);
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }


        internal static string CopiaDCDecodifiche(string vsCodDCDecodifiche)
        {
            try
            {
                string sNuovoCodice = "";
                const string C_LETTERE = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

                string root = vsCodDCDecodifiche;
                if (root.Length >= 19) root = root.Substring(0, 18);
                string tmp = "";
                bool bFound = false;
                int i = 0;
                while (!bFound)
                {
                    if (i < C_LETTERE.Length)
                        tmp = root + C_LETTERE.Substring(i, 1);
                    else
                        tmp = root + i.ToString("00");

                    i += 1;

                    DataTable dt = GetDataTable(GetSqlPUView(Enums.EnumDataNames.T_DCDecodifiche) + @" WHERE Codice = '" + Ax2(tmp) + @"'");
                    bFound = dt.Rows.Count <= 0;
                    dt.Dispose();

                    if (i > 99)
                    {
                        break;
                    }
                }

                if (bFound && tmp != "")
                {


                    frmPUDizionarioWiz oPUDizW = new frmPUDizionarioWiz();
                    oPUDizW.ViewDataNamePU = Enums.EnumDataNames.T_DCDecodifiche;
                    oPUDizW.ViewModality = Enums.EnumModalityPopUp.mpCopia;
                    oPUDizW.ViewText = @"Copia Dizionario";
                    oPUDizW.ViewIcon = ScciResource.Risorse.GetIconFromResource(MyStatics.GetNameResource(MyStatics.GC_DIZIONARI, Enums.EnumImageSize.isz16)); ;
                    oPUDizW.ViewImage = ScciResource.Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_DIZIONARI, Enums.EnumImageSize.isz256));
                    oPUDizW.ViewDizionarioWiz = frmPUDizionarioWiz.enumDizionarioWiz.copia;
                    oPUDizW.codiceNewDizionario = tmp;
                    oPUDizW.descrizioneNewDizionario = FindValue(@"IsNull(Descrizione,'') + ' (copia)'", @"T_DCDecodifiche", @"Codice = '" + Ax2(vsCodDCDecodifiche) + @"'", "");
                    oPUDizW.ViewInit();
                    oPUDizW.ShowDialog();

                    if (oPUDizW.DialogResult == System.Windows.Forms.DialogResult.OK)
                    {

                        tmp = oPUDizW.codiceNewDizionario;
                        string descr = oPUDizW.descrizioneNewDizionario;

                        string sSql = @" INSERT INTO T_DCDecodifiche (Codice, Descrizione)
                                    VALUES ( '" + Ax2(tmp) + @"', '" + Ax2(descr) + @"') " + Environment.NewLine + Environment.NewLine;

                        sSql += @"  INSERT INTO T_DCDecodificheValori ([CodDec],[Codice],[Descrizione],[Ordine],[DtValI],[DtValF],[Icona],[InfoRTF],[Path])
                                Select '" + Ax2(tmp) + @"' As [CodDec],[Codice],[Descrizione],[Ordine],[DtValI],[DtValF],[Icona],[InfoRTF],[Path]
                                From T_DCDecodificheValori
                                Where CodDec = '" + Ax2(vsCodDCDecodifiche) + @"'";

                        if (ExecuteSql(sSql)) sNuovoCodice = tmp;

                    }



                }


                return sNuovoCodice;
            }
            catch (Exception)
            {
                throw;
            }
        }

        internal static string DCDecodificheQuick()
        {
            try
            {
                string sNuovoCodice = "";


                frmPUDizionarioWiz oPUDizW = new frmPUDizionarioWiz();
                oPUDizW.ViewDataNamePU = Enums.EnumDataNames.T_DCDecodifiche;
                oPUDizW.ViewModality = Enums.EnumModalityPopUp.mpNuovo;
                oPUDizW.ViewText = @"Dizionario Quick";
                oPUDizW.ViewIcon = ScciResource.Risorse.GetIconFromResource(MyStatics.GetNameResource(MyStatics.GC_DIZIONARIOQUICK, Enums.EnumImageSize.isz16)); ;
                oPUDizW.ViewImage = ScciResource.Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_DIZIONARIOQUICK, Enums.EnumImageSize.isz256));
                oPUDizW.ViewDizionarioWiz = frmPUDizionarioWiz.enumDizionarioWiz.quick;
                oPUDizW.codiceNewDizionario = "";
                oPUDizW.descrizioneNewDizionario = "";
                oPUDizW.ViewInit();
                oPUDizW.ShowDialog();

                if (oPUDizW.DialogResult == System.Windows.Forms.DialogResult.OK)
                {

                    string codice = oPUDizW.codiceNewDizionario;
                    string descr = oPUDizW.descrizioneNewDizionario;

                    Dictionary<string, string> elencoVoci = new Dictionary<string, string>();
                    string[] sep = new string[] { Environment.NewLine };
                    string[] arrVociQ = oPUDizW.vociQuickNewDizionario.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                    for (int v = 0; v < arrVociQ.Length; v++)
                    {
                        if (arrVociQ[v].Trim() != "" && arrVociQ[v].Trim() != @"|")
                        {
                            string codVoce = "";
                            string descrVoce = "";

                            string[] arrVoce = arrVociQ[v].Split('|');
                            codVoce = arrVoce[0].Trim();
                            descrVoce = arrVoce[0].Trim();
                            if (arrVoce.Length > 1) descrVoce = arrVoce[1].Trim();
                            if (codVoce.Trim() == "") codVoce = descrVoce;
                            if (descrVoce.Trim() == "") descrVoce = codVoce;
                            codVoce = codVoce.Replace(@" ", "");

                            if (codVoce.Length > 50) codVoce.Substring(0, 50);
                            if (descrVoce.Length > 255) descrVoce.Substring(0, 255);

                            bool bContinuaRicerca = true;
                            int cont = 0;
                            string tmp = codVoce;
                            while (bContinuaRicerca)
                            {
                                bool bTrovato = false;
                                cont += 1;
                                foreach (var item in elencoVoci)
                                {
                                    if (item.Key.ToUpper() == tmp.ToUpper())
                                    {
                                        bTrovato = true;
                                    }
                                }

                                if (bTrovato)
                                {
                                    if (codVoce.Length > 48) tmp = codVoce.Substring(0, 48);
                                    tmp = codVoce + cont.ToString("00");
                                    bContinuaRicerca = true;
                                }
                                else
                                {
                                    bContinuaRicerca = false;
                                    codVoce = tmp;
                                }
                            }


                            elencoVoci.Add(codVoce, descrVoce);

                        }
                    }

                    if (elencoVoci.Count > 0)
                    {

                        string sSql = @" INSERT INTO T_DCDecodifiche (Codice, Descrizione)
                                VALUES ( '" + Ax2(codice) + @"', '" + Ax2(descr) + @"') " + Environment.NewLine + Environment.NewLine;

                        int cont = 1;
                        foreach (var voce in elencoVoci)
                        {
                            sSql += @"  INSERT INTO T_DCDecodificheValori ([CodDec],[Codice],[Descrizione],[Ordine],[DtValI])
                                    VALUES('" + Ax2(codice) + @"','" + Ax2(voce.Key) + @"','" + Ax2(voce.Value) + @"', " + cont.ToString() + @", " + SQLDate(DateTime.Now.Date) + @") " + Environment.NewLine + Environment.NewLine;
                            cont += 1;
                        }


                        if (ExecuteSql(sSql)) sNuovoCodice = codice;
                    }


                }

                return sNuovoCodice;
            }
            catch (Exception)
            {
                throw;
            }
        }

        internal static string[] CheckInputScheda(string s_struttura, string s_layout)
        {

            List<string> lst_decodifiche = new List<string>();

            try
            {


                DcScheda oScheda = (DcScheda)Serializer.FromXmlString(s_struttura, typeof(DcScheda));
                DcSchedaLayouts oSchedaLayouts = (DcSchedaLayouts)Serializer.FromXmlString(s_layout, typeof(DcSchedaLayouts));

                foreach (DcSezione oDcSezione in oScheda.Sezioni.Values)
                {

                    foreach (DcVoce oDcVoce in oDcSezione.Voci.Values)
                    {

                        if (oSchedaLayouts.Layouts.ContainsKey(oDcVoce.Key) == true)
                        {

                            switch (oSchedaLayouts.Layouts[oDcVoce.Key].TipoVoce)
                            {

                                case enumTipoVoce.Domanda:
                                case enumTipoVoce.Testo:
                                case enumTipoVoce.Numerico:
                                case enumTipoVoce.Decimale:
                                case enumTipoVoce.Data:
                                case enumTipoVoce.Ora:
                                case enumTipoVoce.Immagine:
                                case enumTipoVoce.Etichetta:
                                case enumTipoVoce.Logo:
                                case enumTipoVoce.TestoRtf:
                                case enumTipoVoce.DataOra:
                                case enumTipoVoce.Bottone:
                                case enumTipoVoce.Scheda:
                                case enumTipoVoce.Marker:
                                case enumTipoVoce.Allegato:
                                    break;

                                case enumTipoVoce.Combo:
                                case enumTipoVoce.Multipla:
                                case enumTipoVoce.ListaSingola:
                                case enumTipoVoce.Zoom:
                                    T_DCDecodifiche dcDecod = T_DCDecodificheBuffer.SelectFirst(MyStatics.Configurazione.ConnectionString, oDcVoce.Decodifica);
                                    if (dcDecod == null) { lst_decodifiche.Add($"ERR01: Manca decodifica ({oDcVoce.Decodifica}) alla voce {oDcVoce.ID}-{oDcVoce.Descrizione} (Sezione: {oDcSezione.ID}-{oDcSezione.Descrizione})"); }
                                    break;

                            }

                        }

                    }

                }

                if (lst_decodifiche.Count > 0)
                {
                    string s_des = $"== Scheda: {oScheda.ID}-{oScheda.Descrizione} ==";
                    lst_decodifiche.Insert(0, string.Concat(Enumerable.Repeat("=", s_des.Length)));
                    lst_decodifiche.Insert(1, s_des);
                    lst_decodifiche.Insert(2, string.Concat(Enumerable.Repeat("=", s_des.Length)));
                }

            }
            catch (Exception ex)
            {
                lst_decodifiche.Add(ex.Message);
            }

            return lst_decodifiche.ToArray();

        }

        #endregion

    }
}
