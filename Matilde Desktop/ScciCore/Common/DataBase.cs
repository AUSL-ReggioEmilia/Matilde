using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using Microsoft.Data.SqlClient;
using UnicodeSrl.Framework.Data;
using Infragistics.Win.UltraWinGrid;
using System.Collections;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;

using UnicodeSrl.ScciCore.WebSvc;
using UnicodeSrl.Scci.DataContracts;

using System.Net.Sockets;
using UnicodeSrl.ScciCore.Common.TimersCB;

using System.Threading;
using UnicodeSrl.Framework.Diagnostics;
using UnicodeSrl.Scci.Model;

namespace UnicodeSrl.ScciCore
{
    public static class DBUtils
    {

        #region Declare

        public static int nTotCicliNewsHard = 0;
        public static int nCicliNewsHard = 0;

        #endregion

        #region Subroutine specifiche

        internal static void get_Controllo(ref TimersCB_Controllo_Data cbData)
        {

            nCicliNewsHard += 1;

            try
            {
                if (Interlocked.Equals(Maschere._navigare, 0))
                {
                    cbData.Connettivita = true;
                    return;
                }

                cbData.Connettivita = Database.GetConnettivita();
                cbData.Connettivita = Database.GetConnettivitaDB();

                if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato != null)
                {
                    string idPaziente = null;
                    string idEpisodio = null;
                    string idCartella = null;

                    if (CoreStatics.CoreApplication.Paziente != null) idPaziente = CoreStatics.CoreApplication.Paziente.ID;
                    if (CoreStatics.CoreApplication.Episodio != null) idEpisodio = CoreStatics.CoreApplication.Episodio.ID;
                    if (CoreStatics.CoreApplication.Cartella != null) idCartella = CoreStatics.CoreApplication.Cartella.ID;

                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);

                    op.Parametro.Add("CodUtente", CoreStatics.CoreApplication.Sessione.Utente.Codice);
                    op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                    if (CoreStatics.CoreApplication.Paziente != null)
                    {
                        op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);
                    }
                    if (CoreStatics.CoreApplication.Episodio != null)
                    {
                        op.Parametro.Add("IDEpisodio", CoreStatics.CoreApplication.Episodio.ID);
                    }
                    if (CoreStatics.CoreApplication.Cartella != null)
                    {
                        op.Parametro.Add("IDCartella", CoreStatics.CoreApplication.Cartella.ID);
                    }

                    if (cbData.Connettivita == true)
                    {

                        MSP_SelIndicatori data = MSP_SelIndicatori.Select(CoreStatics.CoreApplication.Sessione.Utente.Codice, CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice
                            , idPaziente
                            , idEpisodio
                            , idCartella
                            );

                        if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.DiarioC_Valida))
                            cbData.DiarioClinico = data.DiarioClinico;

                        if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.AlertAA_Visualizza))
                            cbData.Allergie = data.Allergie;

                        if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.AlertG_Visualizza))
                            cbData.Alert = data.Alert;

                        if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.EvidenzaC_Visualizza))
                            cbData.EvidenzaClinica = data.EvidenzaClinica;

                        cbData.Segnalibri = data.Segnalibri;
                        cbData.CartelleInVisione = data.CartelleInVisione;
                        cbData.PazientiInVisione = data.PazientiInVisione;

                        cbData.PazientiSeguiti = data.PazientiSeguiti;
                        cbData.PazienteSeguito = data.PazienteSeguito;
                        cbData.PazientiSeguitiDaAltri = data.PazientiSeguitiDaAltri;

                        cbData.MatHome = data.MatHome;

                        if (nTotCicliNewsHard != 0 && nCicliNewsHard >= nTotCicliNewsHard)
                        {

                            op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                            op.Parametro.Add("CodTipoNews", EnumTipoNews.HARD.ToString());
                            op.Parametro.Add("NonVisionate", "1");
                            op.Parametro.Add("ContaNews", "1");
                            SqlParameterExt[] spcoll = new SqlParameterExt[1];
                            string xmlParam = XmlProcs.XmlSerializeToString(op);
                            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                            DataTable dtnewshard = Database.GetDataTableStoredProc("MSP_SelMovNews", spcoll);
                            if (dtnewshard != null && dtnewshard.Rows.Count == 1)
                            {
                                cbData.NewsHard = int.Parse(dtnewshard.Rows[0]["Qta"].ToString());
                            }
                            dtnewshard.Dispose();
                            dtnewshard = null;

                            nCicliNewsHard = 0;
                        }

                        if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.NewsLiteChange == true)
                        {
                            op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                            op.Parametro.Add("CodTipoNews", EnumTipoNews.LITE.ToString());
                            op.Parametro.Add("NonVisionate", "1");
                            op.Parametro.Add("ContaNews", "1");
                            SqlParameterExt[] spcoll = new SqlParameterExt[1];
                            string xmlParam = XmlProcs.XmlSerializeToString(op);
                            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                            DataTable dtnewslite = Database.GetDataTableStoredProc("MSP_SelMovNews", spcoll);
                            if (dtnewslite != null && dtnewslite.Rows.Count == 1)
                            {
                                cbData.NewsLite = int.Parse(dtnewslite.Rows[0]["Qta"].ToString());
                            }
                            dtnewslite.Dispose();
                            dtnewslite = null;
                            if (cbData.NewsLite == 0) { CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.NewsLiteChange = false; }
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }

        }




        internal static List<PazienteSac> get_RicercaPazientiSAC(string cognome, string nome, DateTime datanascita, string luogonascita, string codicefiscale)
        {

            List<PazienteSac> oPazRet = new List<PazienteSac>();
            SvcRicercaSAC.ScciRicercaSACClient sac = null;

            try
            {

                sac = ScciSvcRef.GetSvcRicercaSAC();
                oPazRet.AddRange(sac.RicercaPazientiSAC(cognome, nome, datanascita, luogonascita, codicefiscale));

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }
            finally
            {
                if (sac != null)
                    sac.Close();

                sac = null;
            }

            return oPazRet;

        }

        public static PazienteSac get_RicercaPazientiSACByID(string idsac)
        {

            PazienteSac oPazRet = new PazienteSac();
            SvcRicercaSAC.ScciRicercaSACClient sac = null;

            try
            {
                sac = ScciSvcRef.GetSvcRicercaSAC();
                oPazRet = sac.RicercaPazientiSACByID(idsac);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }
            finally
            {
                if (sac != null)
                    sac.Close();

                sac = null;
            }

            return oPazRet;

        }

        public static PazienteSacDatiAggiuntivi get_PazienteSacDatiAggiuntivi(string idsac)
        {

            PazienteSacDatiAggiuntivi oPazRet = new PazienteSacDatiAggiuntivi();
            SvcRicercaSAC.ScciRicercaSACClient sac = null;

            try
            {

                sac = ScciSvcRef.GetSvcRicercaSAC();
                oPazRet = sac.PazienteSacDatiAggiuntiviByID(idsac);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }
            finally
            {
                if (sac != null)
                    sac.Close();

                sac = null;
            }

            return oPazRet;

        }

        public static string CaricaPazienteDaSAC(PazienteSac pazientesac, PazienteSacDatiAggiuntivi pazientesacdatiaggiuntivi)
        {

            string idpaziente = string.Empty;

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodSAC", Database.testoSQL(pazientesac.CodSAC));
                op.Parametro.Add("Cognome", Database.testoSQL(pazientesac.Cognome));
                op.Parametro.Add("Nome", Database.testoSQL(pazientesac.Nome));
                op.Parametro.Add("Sesso", Database.testoSQL(pazientesac.Sesso));
                op.Parametro.Add("CodiceFiscale", Database.testoSQL(pazientesac.CodiceFiscale));

                if (pazientesac.DataNascita != DateTime.MinValue)
                {
                    op.Parametro.Add("DataNascita", Database.data105PerParametri(pazientesac.DataNascita));
                }
                op.Parametro.Add("CodComuneNascita", Database.testoSQL(pazientesac.CodComuneNascita));
                op.Parametro.Add("ComuneNascita", Database.testoSQL(pazientesac.ComuneNascita));
                op.Parametro.Add("LocalitaNascita", Database.testoSQL(pazientesac.LocalitaNascita));
                op.Parametro.Add("CodProvinciaNascita", Database.testoSQL(pazientesac.CodProvinciaNascita));
                op.Parametro.Add("ProvinciaNascita", Database.testoSQL(pazientesac.ProvinciaNascita));

                op.Parametro.Add("CAPResidenza", Database.testoSQL(pazientesac.CAPResidenza));
                op.Parametro.Add("CodComuneResidenza", Database.testoSQL(pazientesac.CodComuneResidenza));
                op.Parametro.Add("ComuneResidenza", Database.testoSQL(pazientesac.ComuneResidenza));
                op.Parametro.Add("IndirizzoResidenza", Database.testoSQL(pazientesac.IndirizzoResidenza));
                op.Parametro.Add("LocalitaResidenza", Database.testoSQL(pazientesac.LocalitaResidenza));
                op.Parametro.Add("CodProvinciaResidenza", Database.testoSQL(pazientesac.CodProvinciaResidenza));
                op.Parametro.Add("ProvinciaResidenza", Database.testoSQL(pazientesac.ProvinciaResidenza));
                op.Parametro.Add("CodRegioneResidenza", Database.testoSQL(pazientesac.CodRegioneResidenza));
                op.Parametro.Add("RegioneResidenza", Database.testoSQL(pazientesac.RegioneResidenza));

                op.Parametro.Add("CAPDomicilio", Database.testoSQL(pazientesac.CAPDomicilio));
                op.Parametro.Add("CodComuneDomicilio", Database.testoSQL(pazientesac.CodComuneDomicilio));
                op.Parametro.Add("ComuneDomicilio", Database.testoSQL(pazientesac.ComuneDomicilio));
                op.Parametro.Add("IndirizzoDomicilio", Database.testoSQL(pazientesac.IndirizzoDomicilio));
                op.Parametro.Add("LocalitaDomicilio", Database.testoSQL(pazientesac.LocalitaDomicilio));
                op.Parametro.Add("CodProvinciaDomicilio", Database.testoSQL(pazientesac.CodProvinciaDomicilio));
                op.Parametro.Add("ProvinciaDomicilio", Database.testoSQL(pazientesac.ProvinciaDomicilio));

                op.Parametro.Add("CodRegioneDomicilio", "");
                op.Parametro.Add("RegioneDomicilio", "");

                if (pazientesacdatiaggiuntivi != null)
                {
                    op.Parametro.Add("CodMedicoBase", pazientesacdatiaggiuntivi.CodiceMedicoDiBase.ToString());
                    op.Parametro.Add("CodFiscMedicoBase", Database.testoSQL(pazientesacdatiaggiuntivi.CodiceFiscaleMedicoDiBase));
                    op.Parametro.Add("CognomeNomeMedicoBase", Database.testoSQL(pazientesacdatiaggiuntivi.CognomeNomeMedicoDiBase));
                    op.Parametro.Add("DistrettoMedicoBase", Database.testoSQL(pazientesacdatiaggiuntivi.DistrettoMedicoDiBase));
                    if (pazientesacdatiaggiuntivi.DataSceltaMedicoDiBase != DateTime.MinValue)
                    {
                        op.Parametro.Add("DataSceltaMedicoBase", Database.dataOra105PerParametri(pazientesacdatiaggiuntivi.DataSceltaMedicoDiBase));
                    }
                    op.Parametro.Add("ElencoEsenzioni", Database.testoSQL(pazientesacdatiaggiuntivi.DescrizioniEsenzioni(DateTime.Now)));
                }

                if (pazientesac.DataDecesso != DateTime.MinValue) op.Parametro.Add("DataDecesso", Database.dataOra105PerParametri(pazientesac.DataDecesso));
                else op.Parametro.Add("DataDecesso", "");

                op.TimeStamp.CodAzione = EnumAzioni.INS.ToString();
                op.TimeStamp.CodEntita = EnumEntita.PAZ.ToString();

                op.Parametro.Add("CreaPaziente", "1");

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_ControlloPazienteDaSAC", spcoll);

                if (dt.Rows.Count == 1)
                {
                    idpaziente = dt.Rows[0]["IDPaziente"].ToString();
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            return idpaziente;

        }

        public static DataSet getEvidenzaClinicaDataset(bool datiEpisodio,
                                                        bool datiDWH,
                                                        bool datiEstesiEpisodio,
                                                        bool datiEstesiDWH,
                                                        string idPaziente,
                                                        string idEpisodio,
                                                        string idSAC,
                                                        bool soloDefinitivi,
                                                        bool soloDaVistare,
                                                        List<string> tipi,
                                                        DateTime datainizio,
                                                        DateTime datafine,
                                                        DataTable dtTipi)
        {
            try
            {
                return getEvidenzaClinicaDataset(datiEpisodio,
                                                 datiDWH,
                                                 datiEstesiEpisodio,
                                                 datiEstesiDWH,
                                                 idPaziente,
                                                 idEpisodio,
                                                 idSAC,
                                                 soloDefinitivi,
                                                 soloDaVistare,
                                                 tipi,
                                                 datainizio,
                                                 datafine,
                                                 dtTipi,
                                                 false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static DataSet getEvidenzaClinicaDataset(bool datiEpisodio,
                                                        bool datiDWH,
                                                        bool datiEstesiEpisodio,
                                                        bool datiEstesiDWH,
                                                        string idPaziente,
                                                        string idEpisodio,
                                                        string idSAC,
                                                        bool soloDefinitivi,
                                                        bool soloDaVistare,
                                                        List<string> tipi,
                                                        DateTime datainizio,
                                                        DateTime datafine,
                                                        DataTable dtTipi,
                                                        bool throwException)
        {
            try
            {
                return getEvidenzaClinicaDataset(datiEpisodio,
                                                 datiDWH,
                                                 datiEstesiEpisodio,
                                                 datiEstesiDWH,
                                                 idPaziente,
                                                 idEpisodio,
                                                 idSAC,
                                                 soloDefinitivi,
                                                 soloDaVistare,
                                                 tipi,
                                                 datainizio,
                                                 datafine,
                                                 dtTipi,
                                                 false,
                                                 false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static DataSet getEvidenzaClinicaDataset(bool datiEpisodio,
                                                        bool datiDWH,
                                                        bool datiEstesiEpisodio,
                                                        bool datiEstesiDWH,
                                                        string idPaziente,
                                                        string idEpisodio,
                                                        string idSAC,
                                                        bool soloDefinitivi,
                                                        bool soloDaVistare,
                                                        List<string> tipi,
                                                        DateTime datainizio, DateTime datafine,
                                                        DataTable dtTipi,
                                                        bool throwException,
                                                        bool checknosologico,
                                                        DateTime? dataconsensodossier = null)
        {

            try
            {
                DataSet dsReturn = creaEvidenzaClinicaDataset(dtTipi, (datiEstesiEpisodio || datiEstesiDWH));

                if (datiEpisodio)
                {
                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    if (idEpisodio != null && idEpisodio != string.Empty && idEpisodio.Trim() != "")
                        op.Parametro.Add("IDEpisodio", idEpisodio);

                    if (tipi != null && tipi.Count > 0 && !tipi.Contains(CoreStatics.GC_TUTTI))
                        op.ParametroRipetibile.Add("CodTipoEvidenzaClinica", tipi.ToArray());

                    if (soloDefinitivi) op.Parametro.Add("CodStatoEvidenzaClinica", "CM");
                    if (soloDaVistare) op.Parametro.Add("CodStatoEvidenzaClinicaVisione", "DV");
                    if (datainizio > DateTime.MinValue)
                        op.Parametro.Add("DataInizio", Database.dataOra105PerParametri(datainizio));

                    if (datafine > DateTime.MinValue)
                        op.Parametro.Add("DataFine", Database.dataOra105PerParametri(datafine));

                    if (datiEstesiEpisodio) op.Parametro.Add("DatiEstesi", "1");

                    op.TimeStamp.CodEntita = EnumEntita.EVC.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                    SqlParameterExt[] spcoll = new SqlParameterExt[1];

                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    DataSet dsOrigine = Database.GetDatasetStoredProc("MSP_SelMovEvidenzaClinica", spcoll);

                    if (dsOrigine != null && dsOrigine.Tables.Count > 0)
                    {
                        foreach (DataRow drOrigine in dsOrigine.Tables[0].Rows)
                        {

                            DataRow drReferto = dsReturn.Tables[0].NewRow();

                            drReferto["IDSCCI"] = drOrigine["ID"];
                            drReferto["IDPaziente"] = drOrigine["IDPaziente"];
                            drReferto["IDEpisodio"] = drOrigine["IDEpisodio"];
                            drReferto["IDTrasferimento"] = drOrigine["IDTrasferimento"];
                            if (!drOrigine.IsNull("IDRefertoDWH")) drReferto["IDRefertoDWH"] = drOrigine["IDRefertoDWH"].ToString();
                            drReferto["DataReferto"] = drOrigine["DataEvento"];
                            drReferto["Anteprima"] = drOrigine["Anteprima"];

                            drReferto["CodTipo"] = drOrigine["CodTipo"];
                            drReferto["DescrTipo"] = drOrigine["DescrTipo"];

                            drReferto["CodStato"] = drOrigine["CodStato"];
                            drReferto["DescrStato"] = drOrigine["DescrStato"];

                            drReferto["CodStatoVisione"] = drOrigine["CodStatoVisione"];
                            drReferto["DescrStatoVisione"] = drOrigine["DescrStatoVisione"];

                            drReferto["CodUtenteVisione"] = drOrigine["CodUtenteVisione"];
                            drReferto["DescrUtenteVisione"] = drOrigine["DescrUtenteVisione"];
                            drReferto["DataVisione"] = drOrigine["DataVisione"];

                            drReferto["PermessoVista"] = drOrigine["PermessoVista"];
                            drReferto["PermessoGrafico"] = drOrigine["PermessoGrafico"];
                            drReferto["PermessoCancella"] = drOrigine["PermessoCancella"];

                            drReferto["EsistePDFDWH"] = drOrigine["EsistePDFDWH"];

                            drReferto["PDFDWH"] = drOrigine["PDFDWH"];
                            drReferto["IDIcona"] = drOrigine["IDIcona"];

                            drReferto["DataEventoDWH"] = drOrigine["DataEventoDWH"];

                            if (drOrigine.IsNull("DataEventoDWH"))
                            {
                                drReferto["DataEventoDWHGriglia"] = drOrigine["DataEvento"];
                            }
                            else
                            {
                                drReferto["DataEventoDWHGriglia"] = drOrigine["DataEventoDWH"];
                            }


                            drReferto["DWHCodRepartoRichiedente"] = "";
                            drReferto["DWHDescRepartoRichiedente"] = "";

                            drReferto["Firmato"] = "DV";

                            dsReturn.Tables[0].Rows.Add(drReferto);

                        }

                        if (datiEstesiEpisodio && dsOrigine.Tables.Count > 1)
                        {
                            foreach (DataRow drTipoOrigine in dsOrigine.Tables[1].Rows)
                            {
                                bool bAddTipo = true;

                                dsReturn.Tables[1].DefaultView.RowFilter = @"CodTipo = '" + drTipoOrigine["CodTipo"].ToString() + @"'";
                                bAddTipo = (dsReturn.Tables[1].DefaultView.Count <= 0);
                                dsReturn.Tables[1].DefaultView.RowFilter = @"";

                                if (bAddTipo)
                                {
                                    DataRow drTipo = dsReturn.Tables[1].NewRow();

                                    drTipo["CodTipo"] = drTipoOrigine["CodTipo"];
                                    drTipo["DescTipo"] = drTipoOrigine["DescTipo"];

                                    dsReturn.Tables[1].Rows.Add(drTipo);
                                }
                            }

                        }
                    }

                }

                if ((datiDWH && !soloDaVistare && idSAC != null && idSAC != string.Empty && idSAC.Trim() != "") || checknosologico)
                {
                    List<RefertoDWH> oDwhList = new List<RefertoDWH>();
                    SvcRefertiDWH.ScciRefertiDWHClient dwh = null;

                    try
                    {
                        dwh = ScciSvcRef.GetSvcRefertiDWH();


                        List<string> stati = new List<string>();
                        if (soloDefinitivi) stati.Add("CM");
                        if (tipi != null && tipi.Count > 0 && tipi.Contains(CoreStatics.GC_TUTTI)) tipi.Remove(CoreStatics.GC_TUTTI);

                        DateTime datada = datainizio;

                        if (dataconsensodossier.HasValue)
                            datada = dataconsensodossier.Value;

                        UnicodeSrl.Scci.DataContracts.RefertoDWH[] resultDWH = dwh.RicercaRefertiDWHTipi(idSAC, datada, datafine, tipi.ToArray(), stati.ToArray());

                        oDwhList.AddRange(resultDWH);

                        DataTable oDtDWH = CoreStatics.CreateDataTable<RefertoDWH>();
                        CoreStatics.FillDataTable<RefertoDWH>(oDtDWH, oDwhList);

                        string sTipoDaEscludere = "";
                        try
                        {
                            sTipoDaEscludere = Database.GetConfigTable(EnumConfigTable.RefertiDWHdaEscludere);
                            if (sTipoDaEscludere == null || sTipoDaEscludere.Trim() == "")
                                sTipoDaEscludere = "";
                            else
                            {

                                string[] arrCodici = sTipoDaEscludere.Split('|');
                                sTipoDaEscludere = "";
                                for (int i = arrCodici.GetLowerBound(0); i <= arrCodici.GetUpperBound(0); i++)
                                {
                                    if (arrCodici[i] != null && arrCodici[i].Trim() != "")
                                    {
                                        if (sTipoDaEscludere != "") sTipoDaEscludere += @",";
                                        sTipoDaEscludere += @"'" + Database.testoSQL(arrCodici[i]) + @"'";
                                    }
                                }
                                if (sTipoDaEscludere != "") sTipoDaEscludere = @"CodTipoEvidenzaClinica NOT IN (" + sTipoDaEscludere + @")";
                            }
                        }
                        catch
                        {
                        }

                        oDtDWH.DefaultView.RowFilter = sTipoDaEscludere;

                        Episodio oEpisodio = null;
                        foreach (DataRowView drvDWH in oDtDWH.DefaultView)
                        {
                            DataRow drDWH = drvDWH.Row;
                            bool bAddReferto = true;

                            dsReturn.Tables[0].DefaultView.RowFilter = @"IDRefertoDWH = '" + drDWH["IDReferto"].ToString() + @"'";
                            bAddReferto = (dsReturn.Tables[0].DefaultView.Count <= 0);
                            dsReturn.Tables[0].DefaultView.RowFilter = @"";

                            if (checknosologico)
                            {
                                if (drvDWH["NumeroNosologico"].ToString() != string.Empty)
                                {
                                    if (oEpisodio == null) { oEpisodio = new Episodio(idEpisodio); }
                                    if ((oEpisodio.NumeroListaAttesa == drvDWH["NumeroNosologico"].ToString()) ||
                                        (oEpisodio.NumeroEpisodio == drvDWH["NumeroNosologico"].ToString())
                                        )
                                    {
                                        bAddReferto = true;
                                    }
                                    else
                                    {
                                        bAddReferto = false;
                                    }
                                }
                                else
                                {
                                    bAddReferto = false;
                                }
                            }

                            if (bAddReferto)
                            {
                                DataRow drReferto = dsReturn.Tables[0].NewRow();

                                if (idPaziente != null && idPaziente != string.Empty && idPaziente.Trim() != "") drReferto["IDPaziente"] = new Guid(idPaziente);
                                if (idEpisodio != null && idEpisodio != string.Empty && idEpisodio.Trim() != "") drReferto["IDEpisodio"] = new Guid(idEpisodio);
                                drReferto["IDRefertoDWH"] = drDWH["IDReferto"];
                                drReferto["DataReferto"] = drDWH["DataReferto"];
                                drReferto["Anteprima"] = drDWH["TestoAnteprima"];

                                drReferto["CodTipo"] = drDWH["CodTipoEvidenzaClinica"];
                                drReferto["DescrTipo"] = drDWH["DescTipoEvidenzaClinica"];

                                drReferto["CodStato"] = drDWH["CodStatoEvidenzaClinica"];
                                drReferto["DescrStato"] = drDWH["DescStatoEvidenzaClinica"];

                                drReferto["CodStatoVisione"] = "";
                                drReferto["DescrStatoVisione"] = "";

                                drReferto["PermessoVista"] = 0;
                                drReferto["PermessoGrafico"] = 0;
                                if (!drDWH.IsNull("CodTipoEvidenzaClinica") && drDWH["CodTipoEvidenzaClinica"].ToString().Trim().ToUpper() == EnumCodTipoEvidenzaClinica.LAB.ToString().ToUpper())
                                    drReferto["PermessoGrafico"] = 1;
                                drReferto["PermessoCancella"] = 0;

                                drReferto["EsistePDFDWH"] = 1;

                                drReferto["Icona"] = null;
                                drReferto["IDIcona"] = getIDIconaByTipoStato(EnumEntita.EVC, drDWH["CodTipoEvidenzaClinica"].ToString(), drDWH["CodStatoEvidenzaClinica"].ToString());

                                drReferto["SistemaEroganteDWH"] = drDWH["SistemaErogante"];
                                drReferto["NumeroRefertoDWH"] = drDWH["NumeroReferto"];

                                drReferto["DataEventoDWH"] = drDWH["DataEventoDWH"];

                                if (drDWH.IsNull("DataEventoDWH"))
                                {
                                    drReferto["DataEventoDWHGriglia"] = drDWH["DataReferto"];
                                }
                                else
                                {
                                    drReferto["DataEventoDWHGriglia"] = drDWH["DataEventoDWH"];
                                }

                                if (drDWH["Firmato"].ToString() == "True")
                                {
                                    drReferto["Firmato"] = "DV";
                                }
                                else
                                {
                                    drReferto["Firmato"] = "NV";
                                }

                                drReferto["DWHCodRepartoRichiedente"] = drDWH["DWHCodRepartoRichiedente"];
                                drReferto["DWHDescRepartoRichiedente"] = drDWH["DWHDescRepartoRichiedente"];

                                dsReturn.Tables[0].Rows.Add(drReferto);
                            }


                            if (datiEstesiDWH && !drDWH.IsNull("CodTipoEvidenzaClinica") && drDWH["CodTipoEvidenzaClinica"].ToString().Trim() != "" && bAddReferto == true)
                            {
                                bool bAddTipo = true;

                                dsReturn.Tables[1].DefaultView.RowFilter = @"CodTipo = '" + drDWH["CodTipoEvidenzaClinica"].ToString() + @"'";
                                bAddTipo = (dsReturn.Tables[1].DefaultView.Count <= 0);
                                dsReturn.Tables[1].DefaultView.RowFilter = @"";

                                if (bAddTipo)
                                {
                                    DataRow drTipo = dsReturn.Tables[1].NewRow();

                                    drTipo["CodTipo"] = drDWH["CodTipoEvidenzaClinica"];
                                    drTipo["DescTipo"] = drDWH["DescTipoEvidenzaClinica"];

                                    dsReturn.Tables[1].Rows.Add(drTipo);
                                }
                            }
                        }
                        oEpisodio = null;

                    }
                    catch (Exception ex)
                    {
                        UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                        if (throwException) throw ex;
                    }
                    finally
                    {
                        if (dwh != null)
                            dwh.Close();

                        dwh = null;
                    }

                }
                if (dsReturn != null && dsReturn.Tables.Count > 0) dsReturn.Tables[0].DefaultView.Sort = "DataReferto DESC, DataEventoDWH DESC";
                if (dsReturn != null && dsReturn.Tables.Count > 1) dsReturn.Tables[1].DefaultView.Sort = "DescTipo DESC";

                return dsReturn;
            }
            catch (Exception)
            {
                throw;
            }

        }

        private static DataSet creaEvidenzaClinicaDataset(DataTable dtTipi, bool datiestesi)
        {
            try
            {
                DataSet dsReturn = new DataSet();

                DataTable dtReferti = new DataTable("tableReferti");
                dtReferti.Columns.Add("IDSCCI", typeof(Guid));
                dtReferti.Columns.Add("IDPaziente", typeof(Guid));
                dtReferti.Columns.Add("IDEpisodio", typeof(Guid));
                dtReferti.Columns.Add("IDTrasferimento", typeof(Guid));
                dtReferti.Columns.Add("IDRefertoDWH", typeof(string));
                dtReferti.Columns.Add("DataReferto", typeof(DateTime));
                dtReferti.Columns.Add("Anteprima", typeof(string));

                dtReferti.Columns.Add("CodTipo", typeof(string));
                dtReferti.Columns.Add("DescrTipo", typeof(string));

                dtReferti.Columns.Add("CodStato", typeof(string));
                dtReferti.Columns.Add("DescrStato", typeof(string));

                dtReferti.Columns.Add("CodStatoVisione", typeof(string));
                dtReferti.Columns.Add("DescrStatoVisione", typeof(string));

                dtReferti.Columns.Add("CodUtenteVisione", typeof(string));
                dtReferti.Columns.Add("DescrUtenteVisione", typeof(string));
                dtReferti.Columns.Add("DataVisione", typeof(DateTime));

                dtReferti.Columns.Add("PermessoVista", typeof(int));
                dtReferti.Columns.Add("PermessoGrafico", typeof(int));
                dtReferti.Columns.Add("PermessoCancella", typeof(int));

                dtReferti.Columns.Add("EsistePDFDWH", typeof(int));

                dtReferti.Columns.Add("PDFDWH", typeof(byte[]));
                dtReferti.Columns.Add("Icona", typeof(byte[]));
                dtReferti.Columns.Add("IDIcona", typeof(int));

                dtReferti.Columns.Add("NumeroRefertoDWH", typeof(string));
                dtReferti.Columns.Add("SistemaEroganteDWH", typeof(string));

                dtReferti.Columns.Add("DataEventoDWH", typeof(DateTime));

                dtReferti.Columns.Add("DataEventoDWHGriglia", typeof(DateTime));

                dtReferti.Columns.Add("Firmato", typeof(string));

                dtReferti.Columns.Add("DWHCodRepartoRichiedente", typeof(string));
                dtReferti.Columns.Add("DWHDescRepartoRichiedente", typeof(string));

                dsReturn.Tables.Add(dtReferti);


                if (datiestesi)
                {
                    DataTable tableTipi = null;
                    if (dtTipi != null)
                        tableTipi = dtTipi.Copy();
                    else
                    {
                        tableTipi = new DataTable("tableTipi");
                        tableTipi.Columns.Add("CodTipo", typeof(string));
                        tableTipi.Columns.Add("DescTipo", typeof(string));
                    }

                    dsReturn.Tables.Add(tableTipi);
                }


                return dsReturn;
            }
            catch (Exception)
            {
                throw;
            }

        }



        public static DataTable getDatatableTipiEVCRaggruppato(ref DataTable rdtTipi, bool addTutti)
        {
            DataTable dtRet = new DataTable();

            dtRet.Columns.Add("DescTipo", typeof(string));
            dtRet.Columns["DescTipo"].AllowDBNull = false;
            dtRet.Columns["DescTipo"].Unique = true;

            if (addTutti)
            {
                DataRow dr = dtRet.NewRow();
                dr[0] = " " + CoreStatics.GC_TUTTI;
                dtRet.Rows.Add(dr);
            }

            if (rdtTipi != null && rdtTipi.Rows.Count > 0)
            {
                foreach (DataRow drOrig in rdtTipi.Rows)
                {
                    try
                    {
                        if (drOrig.IsNull("DescTipo") || drOrig["DescTipo"].ToString().Trim().ToUpper() != CoreStatics.GC_TUTTI.Trim().ToUpper())
                        {
                            bool bAdd = true;
                            string sDescr = "";
                            if (!drOrig.IsNull("DescTipo")) sDescr = drOrig["DescTipo"].ToString();

                            dtRet.DefaultView.RowFilter = @"DescTipo = '" + Database.testoSQL(sDescr) + @"'";
                            if (dtRet.DefaultView.Count > 0) bAdd = false;
                            dtRet.DefaultView.RowFilter = "";

                            if (bAdd)
                            {
                                DataRow dr = dtRet.NewRow();
                                dr[0] = sDescr;
                                dtRet.Rows.Add(dr);
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }

            if (dtRet.Rows.Count > 1)
            {
                dtRet.DefaultView.Sort = "DescTipo ASC";
                dtRet = dtRet.DefaultView.ToTable(true);
            }

            return dtRet;
        }

        internal static byte[] getIcona16ByTipoStato(EnumEntita entita, string codtipo, string codstato)
        {
            byte[] breturn = null;
            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodEntita", entita.ToString());
                op.Parametro.Add("CodTipo", codtipo);
                op.Parametro.Add("CodStato", codstato);
                op.Parametro.Add("DatiEstesi", "1");

                op.TimeStamp.CodEntita = entita.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dtOrigine = Database.GetDataTableStoredProc("MSP_SelIcona", spcoll);

                if (dtOrigine != null && dtOrigine.Rows.Count > 0 && !dtOrigine.Rows[0].IsNull("Icona16"))
                {
                    breturn = (byte[])dtOrigine.Rows[0]["Icona16"];
                }
            }
            catch (Exception)
            {
            }

            return breturn;
        }

        public static byte[] getIcona256ByTipoStato(EnumEntita entita, string codtipo, string codstato)
        {
            byte[] breturn = null;
            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodEntita", entita.ToString());
                op.Parametro.Add("CodTipo", codtipo);
                op.Parametro.Add("CodStato", codstato);
                op.Parametro.Add("DatiEstesi", "1");

                op.TimeStamp.CodEntita = entita.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dtOrigine = Database.GetDataTableStoredProc("MSP_SelIcona", spcoll);

                if (dtOrigine != null && dtOrigine.Rows.Count > 0 && !dtOrigine.Rows[0].IsNull("Icona256"))
                {
                    breturn = (byte[])dtOrigine.Rows[0]["Icona256"];
                }
            }
            catch (Exception)
            {
            }

            return breturn;
        }

        internal static int getIDIconaByTipoStato(EnumEntita entita, string codtipo, string codstato)
        {

            int ireturn = 0;

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodEntita", entita.ToString());
                op.Parametro.Add("CodTipo", codtipo);
                op.Parametro.Add("CodStato", codstato);
                op.Parametro.Add("DatiEstesi", "0");

                op.TimeStamp.CodEntita = entita.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dtOrigine = Database.GetDataTableStoredProc("MSP_SelIcona", spcoll);

                if (dtOrigine != null && dtOrigine.Rows.Count > 0 && !dtOrigine.Rows[0].IsNull("IDNum"))
                {
                    ireturn = Convert.ToInt32(dtOrigine.Rows[0]["IDNum"]);
                }
            }
            catch (Exception)
            {
            }

            return ireturn;
        }

        internal static byte[] getIconaByViaSomministrazione(string codice)
        {

            byte[] breturn = null;

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("Codice", codice);
                op.Parametro.Add("DatiEstesi", "1");

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dtOrigine = Database.GetDataTableStoredProc("MSP_SelViaSomministrazione", spcoll);

                if (dtOrigine != null && dtOrigine.Rows.Count > 0 && !dtOrigine.Rows[0].IsNull("Icona"))
                {
                    breturn = (byte[])dtOrigine.Rows[0]["Icona"];
                }
            }
            catch (Exception)
            {
            }

            return breturn;
        }

        internal static byte[] getIconaBySelStatoPrescrizione(string codice)
        {

            byte[] breturn = null;

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("Codice", codice);
                op.Parametro.Add("DatiEstesi", "1");

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dtOrigine = Database.GetDataTableStoredProc("MSP_SelStatoPrescrizione", spcoll);

                if (dtOrigine != null && dtOrigine.Rows.Count > 0 && !dtOrigine.Rows[0].IsNull("Icona"))
                {
                    breturn = (byte[])dtOrigine.Rows[0]["Icona"];
                }
            }
            catch (Exception)
            {
            }

            return breturn;
        }

        public static DataTable getEpisodiPazienteDatatable(string idSAC, DateTime datainizio, DateTime datafine)
        {
            SvcRicoveriDWH.ScciRicoveriDWHClient dwhclnt = null;

            try
            {
                List<RicoveroDWHSintetico> oDwhList = new List<RicoveroDWHSintetico>();
                DataTable dtReturn = null;

                dwhclnt = ScciSvcRef.GetSvcRicoveriDWH();
                oDwhList.AddRange(dwhclnt.RicercaRicoveriDWH(idSAC, datainizio, datafine));

                dtReturn = CoreStatics.CreateDataTable<RicoveroDWHSintetico>();
                CoreStatics.FillDataTable<RicoveroDWHSintetico>(dtReturn, oDwhList);

                return dtReturn;
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (dwhclnt != null)
                    dwhclnt.Close();

                dwhclnt = null;
            }
        }

        public static RicoveroDWH getRicoveroDWH(string idRicovero)
        {
            SvcRicoveriDWH.ScciRicoveriDWHClient dwhclnt = null;

            try
            {
                RicoveroDWH objReturn = new RicoveroDWH();

                dwhclnt = ScciSvcRef.GetSvcRicoveriDWH();
                objReturn = dwhclnt.RicoveroPerId(idRicovero);

                return objReturn;
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (dwhclnt != null)
                    dwhclnt.Close();

                dwhclnt = null;
            }
        }

        internal static DataTable getRisultatiLaboratorioPazienteDatatable(string idSAC, DateTime datainizio, DateTime datafine)
        {
            SvcRicoveriDWH.ScciRicoveriDWHClient dwhclnt = null;

            try
            {
                List<RisultatiLab> oDwhList = new List<RisultatiLab>();
                DataTable dtReturn = null;

                dwhclnt = ScciSvcRef.GetSvcRicoveriDWH();
                oDwhList.AddRange(dwhclnt.RicercaDatiLabDWH(idSAC, datainizio, datafine));

                dtReturn = CoreStatics.CreateDataTable<RisultatiLab>();
                CoreStatics.FillDataTable<RisultatiLab>(dtReturn, oDwhList);

                return dtReturn;
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (dwhclnt != null)
                    dwhclnt.Close();

                dwhclnt = null;
            }
        }

        internal static bool storicizzaReport(string codreport, string documentogeneratofullpath)
        {
            return storicizzaReport(codreport, documentogeneratofullpath, true);
        }
        internal static bool storicizzaReport(string codreport, string documentogeneratofullpath, bool storicizza)
        {
            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);

                if (CoreStatics.CoreApplication.Paziente != null) op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);
                if (CoreStatics.CoreApplication.Episodio != null) op.Parametro.Add("IDEpisodio", CoreStatics.CoreApplication.Episodio.ID);
                if (CoreStatics.CoreApplication.Trasferimento != null) op.Parametro.Add("IDTrasferimento", CoreStatics.CoreApplication.Trasferimento.ID);
                op.Parametro.Add("CodReport", codreport);
                if (storicizza)
                {
                    byte[] report = UnicodeSrl.Framework.Utility.FileSystem.FileToByteArray(documentogeneratofullpath);
                    op.Parametro.Add("Documento", Convert.ToBase64String(report));
                }
                op.TimeStamp.CodAzione = EnumAzioni.INS.ToString();
                op.TimeStamp.CodEntita = EnumEntita.RPT.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                Database.ExecStoredProc("MSP_InsMovReport", spcoll);

                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static bool ModuloUAAbilitato(string codUA, EnumUAModuli uaModulo)
        {
            bool bModuloAbilitato = false;
            DataTable dt = null;
            try
            {
                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodUA", codUA);
                op.Parametro.Add("CodModulo", uaModulo.ToString());

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                dt = Database.GetDataTableStoredProc("MSP_SelUAModuli", spcoll);
                if (dt != null && dt.Rows.Count > 0) bModuloAbilitato = true;
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }
            finally
            {
                if (dt != null)
                {
                    dt.Dispose();
                    dt = null;
                }
            }

            return bModuloAbilitato;
        }

        public static bool ControllaAnticipoErogazione(DateTime dtDataOraErogazione)
        {
            try
            {
                bool bContinua = true;
                if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata != null)
                {
                    int ndeltaminuti = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.AnticipoMinutiTipoTaskInfermieristico;
                    if (ndeltaminuti != 0)
                    {
                        TimeSpan timespan = (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata - dtDataOraErogazione);
                        if (timespan.TotalMinutes > ndeltaminuti)
                        {
                            if (easyStatics.EasyMessageBox("Attività pianificata nel futuro." + Environment.NewLine +
                                                            "Sei sicuro di voler erogare ?", "Erogazione", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                            {
                                bContinua = false;
                            }
                        }
                    }
                }
                return bContinua;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool ControllaAlertErogazione(MovTaskInfermieristico movTask, bool verificaAzioneVIS)
        {
            bool bAlertAccettato = true;

            try
            {
                if (movTask != null)
                {

                    if (!verificaAzioneVIS || movTask.Azione == EnumAzioni.VAL)
                    {
                        string sAlert = movTask.Alert;


                        if (sAlert != null && sAlert.Trim() != "")
                        {
                            string stitolo = "Per procedere con l'Erogazione occorre prima leggere e confermare il seguente avviso:" + Environment.NewLine;
                            sAlert = CoreStatics.FixNewLine(sAlert);

                            if (easyStatics.EasyMessageBoxInfo(sAlert
                                                               , stitolo
                                                               , "Erogazione Task"
                                                               , System.Windows.Forms.MessageBoxButtons.OKCancel
                                                               , System.Windows.Forms.MessageBoxIcon.Warning
                                                               , "CONFERMA"
                                                               , "ANNULLA"
                                                               , easyStatics.easyRelativeDimensions.XLarge
                                                               , easyStatics.easyRelativeDimensions.XLarge
                                                               , true) == System.Windows.Forms.DialogResult.Cancel)
                            {
                                bAlertAccettato = false;
                            }
                        }

                    }


                }
            }
            catch (Exception ex)
            {
                DiagnosticStatics.AddDebugInfo(ex);
            }
            return bAlertAccettato;
        }

        #endregion

    }
}
