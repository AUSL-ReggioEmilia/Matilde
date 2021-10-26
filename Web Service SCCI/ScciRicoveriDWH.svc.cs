using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;

using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Framework.Diagnostics;
using System.Data.SqlClient;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;
using System.Data.SqlTypes;

namespace WsSCCI
{
    public class ScciRicoveriDWH : IScciRicoveriDWH
    {

        public RicoveroDWH RicoveroPerId(string idricovero)
        {
            DataSet dsRicoveri = null;
            UnicodeSrl.Scci.Statics.Database.ConnectionString = Common.ConnString;

            RicoveroDWH oRicoveroDWH = new RicoveroDWH();

            try
            {
                                net.asmn.dwhclinico.DataAccessTdsV2 dwh = new net.asmn.dwhclinico.DataAccessTdsV2();
                dwh.Url = UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceDWH);

                                dwh.UseDefaultCredentials = false;
                dwh.Credentials = Credentials.CredenzialiScci(Credentials.enumTipoCredenziali.DWH);

                                dsRicoveri = dwh.RicoveroPerId(idricovero);

                if (dsRicoveri != null && dsRicoveri.Tables["Ricovero"].Rows.Count > 0)
                {

                    oRicoveroDWH.IDRicovero = dsRicoveri.Tables["Ricovero"].Rows[0]["Id"].ToString();
                    oRicoveroDWH.IDPaziente = dsRicoveri.Tables["Ricovero"].Rows[0]["IdPaziente"].ToString();
                    oRicoveroDWH.Nosologico = dsRicoveri.Tables["Ricovero"].Rows[0]["NumeroNosologico"].ToString();
                    oRicoveroDWH.AziendaErogante = dsRicoveri.Tables["Ricovero"].Rows[0]["AziendaErogante"].ToString();
                    if (dsRicoveri.Tables["Ricovero"].Rows[0]["DataInizioEpisodio"].ToString() != "") oRicoveroDWH.DataInizioRicovero = (DateTime)dsRicoveri.Tables["Ricovero"].Rows[0]["DataInizioEpisodio"];
                    if (dsRicoveri.Tables["Ricovero"].Rows[0]["DataFineEpisodio"].ToString() != "") oRicoveroDWH.DataFineRicovero = (DateTime)dsRicoveri.Tables["Ricovero"].Rows[0]["DataFineEpisodio"];
                    oRicoveroDWH.Diagnosi = dsRicoveri.Tables["Ricovero"].Rows[0]["Diagnosi"].ToString();
                    oRicoveroDWH.DescTipoEpisodio = dsRicoveri.Tables["Ricovero"].Rows[0]["TipoEpisodioDescr"].ToString();
                    oRicoveroDWH.DescRepartoAmmissione = dsRicoveri.Tables["Ricovero"].Rows[0]["RepartoRicoveroAccettazioneDescr"].ToString();
                    oRicoveroDWH.DescRepartoDimissione = dsRicoveri.Tables["Ricovero"].Rows[0]["RepartoRicoveroUltimoEventoDescr"].ToString();
                    
                    oRicoveroDWH.UltimoEvento = dsRicoveri.Tables["Ricovero"].Rows[0]["UltimoEventoDescr"].ToString();
                                        
                                        if (dsRicoveri.Tables.Contains("RicoveroAttributi") && dsRicoveri.Tables["RicoveroAttributi"].Rows.Count > 0)
                    {
                                                foreach (DataRow rowAttr in dsRicoveri.Tables["RicoveroAttributi"].Rows)
                        {
                            try
                            {
                                if (!rowAttr.IsNull("Nome") && rowAttr["Nome"].ToString().Trim().ToUpper() == @"RepartoDimissioneDescr".ToUpper() && !rowAttr.IsNull("Valore"))
                                    oRicoveroDWH.DescRepartoDimissione = rowAttr["Valore"].ToString().Trim();

                                                                
                                                                                            }
                            catch
                            {
                            }
                        }
                    }

                                        if (dsRicoveri.Tables["RicoveroEventi"].Rows.Count > 0)
                    {

                        foreach (DataRow oRowEventoRicovero in dsRicoveri.Tables["RicoveroEventi"].Rows)
                        {
                            EventoDWH oEventoRicoveroDWH = new EventoDWH();

                            oEventoRicoveroDWH.IDEvento = oRowEventoRicovero["Id"].ToString();
                            oEventoRicoveroDWH.Nosologico = oRowEventoRicovero["NumeroNosologico"].ToString();
                            oEventoRicoveroDWH.AziendaErogante = oRowEventoRicovero["AziendaErogante"].ToString();
                            oEventoRicoveroDWH.SistemaErogante = oRowEventoRicovero["SistemaErogante"].ToString();
                            oEventoRicoveroDWH.RepartoErogante = oRowEventoRicovero["RepartoErogante"].ToString();
                            if (oRowEventoRicovero["DataEvento"].ToString() != "") oEventoRicoveroDWH.DataEvento = (DateTime)oRowEventoRicovero["DataEvento"];
                            oEventoRicoveroDWH.CodTipoEvento = oRowEventoRicovero["TipoEventoCodice"].ToString();
                            oEventoRicoveroDWH.DescTipoEvento = oRowEventoRicovero["TipoEventoDescr"].ToString();
                            oEventoRicoveroDWH.CodTipoEpisodio = oRowEventoRicovero["TipoEpisodio"].ToString();
                            oEventoRicoveroDWH.DescTipoEpisodio = oRowEventoRicovero["TipoEpisodioDescr"].ToString();
                            oEventoRicoveroDWH.CodReparto = oRowEventoRicovero["RepartoRicoveroCodice"].ToString();
                            oEventoRicoveroDWH.DescReparto = oRowEventoRicovero["RepartoRicoveroDescr"].ToString();
                            oEventoRicoveroDWH.Diagnosi = oRowEventoRicovero["Diagnosi"].ToString();

                            oEventoRicoveroDWH.CodSettore = oRowEventoRicovero["SettoreRicoveroCodice"].ToString();
                            oEventoRicoveroDWH.DescSettore = oRowEventoRicovero["SettoreRicoveroDescr"].ToString();
                            oEventoRicoveroDWH.CodLetto = oRowEventoRicovero["LettoRicoveroCodice"].ToString();
                            oEventoRicoveroDWH.DescLetto = oRowEventoRicovero["LettoRicoveroDescr"].ToString();

                                                        if (dsRicoveri.Tables.Contains("RicoveroEventiAttributi") && dsRicoveri.Tables["RicoveroEventiAttributi"].Rows.Count > 0)
                            {
                                dsRicoveri.Tables["RicoveroEventiAttributi"].DefaultView.RowFilter = @"IdEventiBase='" + oEventoRicoveroDWH.IDEvento + @"'";
                                if (dsRicoveri.Tables["RicoveroEventiAttributi"].DefaultView.Count > 0)
                                {
                                                                        foreach (DataRowView rowAttr in dsRicoveri.Tables["RicoveroEventiAttributi"].DefaultView)
                                    {
                                        try
                                        {
                                            if (!rowAttr.Row.IsNull("Nome") && rowAttr["Nome"].ToString().Trim().ToUpper() == @"SettoreDescr".ToUpper() && !rowAttr.Row.IsNull("Valore"))
                                                oEventoRicoveroDWH.DescSettore = rowAttr["Valore"].ToString().Trim();

                                            if (!rowAttr.Row.IsNull("Nome") && rowAttr["Nome"].ToString().Trim().ToUpper() == @"SettoreCodice".ToUpper() && !rowAttr.Row.IsNull("Valore"))
                                                oEventoRicoveroDWH.CodSettore = rowAttr["Valore"].ToString().Trim();

                                            if (!rowAttr.Row.IsNull("Nome") && rowAttr["Nome"].ToString().Trim().ToUpper() == @"LettoCodice".ToUpper() && !rowAttr.Row.IsNull("Valore"))
                                            {
                                                oEventoRicoveroDWH.CodLetto = rowAttr["Valore"].ToString().Trim();
                                                if (oEventoRicoveroDWH.DescLetto == string.Empty) oEventoRicoveroDWH.DescLetto = oEventoRicoveroDWH.CodLetto;
                                            }

                                            if (!rowAttr.Row.IsNull("Nome") && rowAttr["Nome"].ToString().Trim().ToUpper() == @"LettoDescr".ToUpper() && !rowAttr.Row.IsNull("Valore"))
                                                oEventoRicoveroDWH.DescLetto = rowAttr["Valore"].ToString().Trim();
                                        }
                                        catch
                                        {
                                        }
                                    }
                                }
                                
                                dsRicoveri.Tables["RicoveroEventiAttributi"].DefaultView.RowFilter = @"";
                            }

                            oRicoveroDWH.EventiDWH.Add(oEventoRicoveroDWH);
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
                oRicoveroDWH = new RicoveroDWH();
            }

            return oRicoveroDWH;
        }

        public List<RicoveroDWHSintetico> RicercaRicoveriDWH(string idsac, DateTime datainizio, DateTime datafine)
        {

            DataSet dsRicoveri = null;
            UnicodeSrl.Scci.Statics.Database.ConnectionString = Common.ConnString;

            List<RicoveroDWHSintetico> oListRicoveriDWH = new List<RicoveroDWHSintetico>();

            string datainiziostring = string.Empty;
            string datafinestring = string.Empty;

            try
            {
                                net.asmn.dwhclinico.DataAccessTdsV2 dwh = new net.asmn.dwhclinico.DataAccessTdsV2();
                dwh.Url = UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceDWH);

                                dwh.UseDefaultCredentials = false;
                dwh.Credentials = Credentials.CredenzialiScci(Credentials.enumTipoCredenziali.DWH);

                                if (datainizio != null && datainizio != DateTime.MinValue)
                    datainiziostring = datainizio.ToString("yyyy-MM-dd");
                else
                    datainiziostring = System.Data.SqlTypes.SqlDateTime.MinValue.Value.ToString("yyyy-MM-dd");

                if (datafine != null && datafine != DateTime.MinValue)
                    datafinestring = datafine.ToString("yyyy-MM-dd");
                else
                    datafinestring = System.Data.SqlTypes.SqlDateTime.MaxValue.Value.ToString("yyyy-MM-dd");

                                dsRicoveri = dwh.RicoveriPerPaziente(idsac, datainiziostring, datafinestring);

                if (dsRicoveri != null)
                {
                    foreach (DataRow oRowRicovero in dsRicoveri.Tables["Ricoveri"].Rows)
                    {
                        RicoveroDWHSintetico oRicoveroDWH = new RicoveroDWHSintetico();

                        oRicoveroDWH.IDRicovero = oRowRicovero["IdRicovero"].ToString();
                        oRicoveroDWH.IDPaziente = oRowRicovero["IdPaziente"].ToString();
                        oRicoveroDWH.Nosologico = oRowRicovero["NumeroNosologico"].ToString();
                        oRicoveroDWH.AziendaErogante = oRowRicovero["AziendaErogante"].ToString();
                        if (oRowRicovero["DataInizioEpisodio"].ToString() != "") oRicoveroDWH.DataInizioRicovero = (DateTime)oRowRicovero["DataInizioEpisodio"];
                        if (oRowRicovero["DataFineEpisodio"].ToString() != "") oRicoveroDWH.DataFineRicovero = (DateTime)oRowRicovero["DataFineEpisodio"];
                        oRicoveroDWH.Diagnosi = oRowRicovero["Diagnosi"].ToString();
                        oRicoveroDWH.DescTipoEpisodio = oRowRicovero["TipoEpisodioDescr"].ToString();
                        oRicoveroDWH.DescRepartoAmmissione = oRowRicovero["RepartoRicoveroAccettazioneDescr"].ToString();
                        oRicoveroDWH.DescRepartoDimissione = oRowRicovero["RepartoRicoveroUltimoEventoDescr"].ToString();

                        
                        oListRicoveriDWH.Add(oRicoveroDWH);
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
                oListRicoveriDWH = new List<RicoveroDWHSintetico>();
            }

            return oListRicoveriDWH;

        }

        public List<RisultatiLab> RicercaDatiLabDWH(string idsac, DateTime datainizio, DateTime datafine)
        {


            UnicodeSrl.Scci.Statics.Database.ConnectionString = Common.ConnString;


            List<RisultatiLab> oRisultatiLab = new List<RisultatiLab>();

            string datainiziostring = string.Empty;
            string datafinestring = string.Empty;

            net.asmn.dwhclinicolab.PrestazioniMatriceLab2PerPazienteResult result = new net.asmn.dwhclinicolab.PrestazioniMatriceLab2PerPazienteResult();
            
            try
            {
                                net.asmn.dwhclinicolab.DataAccessV2 dwh = new net.asmn.dwhclinicolab.DataAccessV2();
                dwh.Url = UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceDWHLAB);

                                dwh.UseDefaultCredentials = false;
                dwh.Credentials = Credentials.CredenzialiScci(Credentials.enumTipoCredenziali.DWH);

                                if (datainizio != null && datainizio != DateTime.MinValue)
                    datainiziostring = datainizio.ToString("yyyy-MM-ddTHH:mm:ss");
                
                else
                    datainiziostring = System.Data.SqlTypes.SqlDateTime.MinValue.Value.ToString("yyyy-MM-dd");

                if (datafine != null && datafine != DateTime.MinValue)
                    datafinestring = datafine.ToString("yyyy-MM-ddTHH:mm:ss");
                else
                    datafinestring = System.Data.SqlTypes.SqlDateTime.MaxValue.Value.ToString("yyyy-MM-dd");

                                result = dwh.PrestazioniMatriceLab2PerPaziente(idsac, datainiziostring, datafinestring);

                if (result != null)
                {
                    foreach (net.asmn.dwhclinicolab.PrestazioniMatriceLab2PerPazienteResultPrestazioniMatriceLab singleresult in result.PrestazioniMatriceLab)
                    {

                        try
                        {
                            RisultatiLab risultatolab = new RisultatiLab();

                            risultatolab.IdReferto = singleresult.IdRefertiBase;
                            risultatolab.CodSezione = singleresult.SezioneCodice;
                            risultatolab.DescrSezione = singleresult.SezioneDescrizione;
                            risultatolab.CodPrescrizione = singleresult.PrestazioneCodice;
                            risultatolab.DescPrescrizione = singleresult.PrestazioneDescrizione;

                            risultatolab.Quantita = 0;
                            if (singleresult.Quantita != null)
                            {
                                double dbltmp = 0;
                                if (double.TryParse(singleresult.Quantita, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out dbltmp)) risultatolab.Quantita = dbltmp;
                            }

                            if (singleresult.DataEvento != null && singleresult.DataEvento > DateTime.MinValue && singleresult.DataEvento < DateTime.MaxValue)
                                risultatolab.Data = singleresult.DataEvento;
                            else
                                risultatolab.Data = singleresult.DataReferto;

                            oRisultatiLab.Add(risultatolab);

                        }
                        catch (Exception ex)
                        {
                            ErrorHandler.AddException(ex);
                        }


                    }
                }


            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
                oRisultatiLab = new List<RisultatiLab>();
            }

            return oRisultatiLab;

        }

                                                                public List<RisultatiLabUM> RicercaDatiLabDWHUM(string idsac, DateTime datainizio, DateTime datafine)
        {


            UnicodeSrl.Scci.Statics.Database.ConnectionString = Common.ConnString;


            List<RisultatiLabUM> oRisultatiLabUM = new List<RisultatiLabUM>();

            string datainiziostring = string.Empty;
            string datafinestring = string.Empty;

            net.asmn.dwhclinicolab.PrestazioniMatriceLab2PerPazienteResult result = new net.asmn.dwhclinicolab.PrestazioniMatriceLab2PerPazienteResult();

            try
            {
                                net.asmn.dwhclinicolab.DataAccessV2 dwh = new net.asmn.dwhclinicolab.DataAccessV2();
                dwh.Url = UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceDWHLAB);

                                dwh.UseDefaultCredentials = false;
                dwh.Credentials = Credentials.CredenzialiScci(Credentials.enumTipoCredenziali.DWH);

                                if (datainizio != null && datainizio != DateTime.MinValue)
                    datainiziostring = datainizio.ToString("yyyy-MM-ddTHH:mm:ss");
                else
                    datainiziostring = System.Data.SqlTypes.SqlDateTime.MinValue.Value.ToString("yyyy-MM-dd");

                if (datafine != null && datafine != DateTime.MinValue)
                    datafinestring = datafine.ToString("yyyy-MM-ddTHH:mm:ss");
                else
                    datafinestring = System.Data.SqlTypes.SqlDateTime.MaxValue.Value.ToString("yyyy-MM-dd");

                                result = dwh.PrestazioniMatriceLab2PerPaziente(idsac, datainiziostring, datafinestring);

                if (result != null)
                {
                    foreach (net.asmn.dwhclinicolab.PrestazioniMatriceLab2PerPazienteResultPrestazioniMatriceLab singleresult in result.PrestazioniMatriceLab)
                    {

                        try
                        {

                                                                                                                                                                        bool bAddRisultato = true;

                            if (singleresult.DataEvento != null && singleresult.DataEvento > DateTime.MinValue && singleresult.DataEvento < DateTime.MaxValue)
                            {
                                                                if (singleresult.DataEvento > datafine || singleresult.DataEvento < datainizio) bAddRisultato = false;
                            }

                                if (bAddRisultato)
                            {
                                RisultatiLabUM risultatolab = new RisultatiLabUM();

                                risultatolab.IdReferto = singleresult.IdRefertiBase;
                                risultatolab.CodSezione = singleresult.SezioneCodice;
                                risultatolab.DescrSezione = singleresult.SezioneDescrizione;
                                risultatolab.CodPrescrizione = singleresult.PrestazioneCodice;
                                risultatolab.DescPrescrizione = singleresult.PrestazioneDescrizione;

                                risultatolab.Quantita = 0;
                                if (singleresult.Quantita != null)
                                {
                                    double dbltmp = 0;
                                    if (double.TryParse(singleresult.Quantita, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out dbltmp)) risultatolab.Quantita = dbltmp;
                                }

                                if (singleresult.DataEvento != null && singleresult.DataEvento > DateTime.MinValue && singleresult.DataEvento < DateTime.MaxValue)
                                    risultatolab.Data = singleresult.DataEvento;
                                else
                                    risultatolab.Data = singleresult.DataReferto;

                                risultatolab.UM = singleresult.UnitaDiMisuraDescrizione;

                                oRisultatiLabUM.Add(risultatolab);
                            }

                        }
                        catch (Exception ex)
                        {
                            ErrorHandler.AddException(ex);
                        }


                    }
                }


            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
                oRisultatiLabUM = new List<RisultatiLabUM>();
            }

            return oRisultatiLabUM;

        }

                                                                public List<RisultatiLabAll> RicercaDatiLabDWHAll(string idsac, DateTime datainizio, DateTime datafine)
        {


            UnicodeSrl.Scci.Statics.Database.ConnectionString = Common.ConnString;


            List<RisultatiLabAll> oRisultatiLabAll = new List<RisultatiLabAll>();

            string datainiziostring = string.Empty;
            string datafinestring = string.Empty;

            net.asmn.dwhclinicolab.PrestazioniMatriceLab2PerPazienteResult result = new net.asmn.dwhclinicolab.PrestazioniMatriceLab2PerPazienteResult();

            try
            {
                                net.asmn.dwhclinicolab.DataAccessV2 dwh = new net.asmn.dwhclinicolab.DataAccessV2();
                dwh.Url = UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceDWHLAB);

                                dwh.UseDefaultCredentials = false;
                dwh.Credentials = Credentials.CredenzialiScci(Credentials.enumTipoCredenziali.DWH);

                                if (datainizio != null && datainizio != DateTime.MinValue)
                    datainiziostring = datainizio.ToString("yyyy-MM-ddTHH:mm:ss");
                else
                    datainiziostring = System.Data.SqlTypes.SqlDateTime.MinValue.Value.ToString("yyyy-MM-dd");

                if (datafine != null && datafine != DateTime.MinValue)
                    datafinestring = datafine.ToString("yyyy-MM-ddTHH:mm:ss");
                else
                    datafinestring = System.Data.SqlTypes.SqlDateTime.MaxValue.Value.ToString("yyyy-MM-dd");

                                result = dwh.PrestazioniMatriceLab2PerPaziente(idsac, datainiziostring, datafinestring);

                if (result != null)
                {
                    foreach (net.asmn.dwhclinicolab.PrestazioniMatriceLab2PerPazienteResultPrestazioniMatriceLab singleresult in result.PrestazioniMatriceLab)
                    {

                        try
                        {

                                                                                                                                                                        bool bAddRisultato = true;

                            if (singleresult.DataEvento != null && singleresult.DataEvento > DateTime.MinValue && singleresult.DataEvento < DateTime.MaxValue)
                            {
                                                                if (singleresult.DataEvento > datafine || singleresult.DataEvento < datainizio) bAddRisultato = false;
                            }

                            if (bAddRisultato)
                            {
                                RisultatiLabAll risultatolaball = new RisultatiLabAll();

                                risultatolaball.IdReferto = singleresult.IdRefertiBase;
                                risultatolaball.CodSezione = singleresult.SezioneCodice;
                                risultatolaball.DescrSezione = singleresult.SezioneDescrizione;
                                risultatolaball.CodPrescrizione = singleresult.PrestazioneCodice;
                                risultatolaball.DescPrescrizione = singleresult.PrestazioneDescrizione;

                                risultatolaball.RisultatoNumericoAssente = false;
                                risultatolaball.Quantita = 0;
                                if (singleresult.Quantita != null)
                                {
                                    double dbltmp = 0;
                                    if (double.TryParse(singleresult.Quantita, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out dbltmp)) risultatolaball.Quantita = dbltmp;
                                }
                                else
                                    risultatolaball.RisultatoNumericoAssente = true;

                                if (singleresult.DataEvento != null && singleresult.DataEvento > DateTime.MinValue && singleresult.DataEvento < DateTime.MaxValue)
                                    risultatolaball.Data = singleresult.DataEvento;
                                else
                                    risultatolaball.Data = singleresult.DataReferto;

                                risultatolaball.UM = singleresult.UnitaDiMisuraDescrizione;

                                                                risultatolaball.Risultato = singleresult.Risultato;
                                risultatolaball.Commenti = singleresult.Commenti;
                                
                                oRisultatiLabAll.Add(risultatolaball);
                            }

                        }
                        catch (Exception ex)
                        {
                            ErrorHandler.AddException(ex);
                        }


                    }
                }


            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
                oRisultatiLabAll = new List<RisultatiLabAll>();
            }

            return oRisultatiLabAll;

        }

    }
}
