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

namespace WsSCCI
{
    public class ScciRefertiDWH : IScciRefertiDWH
    {

        #region Public Methods

        public List<RefertoDWH> RicercaRefertiDWH(string idsac, DateTime datainizio, DateTime datafine, string tipoevidenzaclinica, string statoevidenzaclinica)
        {
            DataSet dsReferti = null;
            UnicodeSrl.Scci.Statics.Database.ConnectionString = Common.ConnString;

            List<RefertoDWH> oListRefertiDWH = new List<RefertoDWH>();

            string datainiziostring = string.Empty;
            string datafinestring = string.Empty;

                        List<string> codicidwh = new List<string>();

                        DataSet dsfiltri = this.CaricaTipiStatiEvidenzeCliniche();

                        bool bAdd = true;

                        string sfiltrotipoDWH = string.Empty;
            string sfiltrostatoDWH = string.Empty;

            try
            {

                                if (dsfiltri != null)
                {
                    try
                    { sfiltrotipoDWH = UnicodeSrl.Scci.Statics.Database.DataTableFindValue(dsfiltri.Tables[Common.GC_TBTIPI], "CodDWH", "CodTipoEvidenzaClinica ='" + tipoevidenzaclinica + "'").ToString(); }
                    catch
                    { sfiltrotipoDWH = string.Empty; }

                    try
                    { sfiltrostatoDWH = UnicodeSrl.Scci.Statics.Database.DataTableFindValue(dsfiltri.Tables[Common.GC_TBSTATI], "CodDWH", "CodStatoEvidenzaClinica ='" + statoevidenzaclinica + "'").ToString(); }
                    catch
                    { sfiltrostatoDWH = string.Empty; }
                }
                else
                {
                    sfiltrotipoDWH = string.Empty;
                    sfiltrostatoDWH = string.Empty;
                }

                                net.asmn.dwhclinico.DataAccessTdsV2 dwh = new net.asmn.dwhclinico.DataAccessTdsV2();
                dwh.Url = UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceDWH);

                                dwh.UseDefaultCredentials = false;
                dwh.Credentials = Credentials.CredenzialiScci(Credentials.enumTipoCredenziali.DWH);

                                                if (datainizio != null && datainizio != DateTime.MinValue)
                    datainiziostring = datainizio.ToString("yyyy-MM-ddTHH:mm:ss");
                else
                    datainiziostring = System.Data.SqlTypes.SqlDateTime.MinValue.Value.ToString("yyyy-MM-ddTHH:mm:ss");

                if (datafine != null && datafine != DateTime.MinValue)
                    datafinestring = datafine.ToString("yyyy-MM-ddTHH:mm:ss");
                else
                    datafinestring = System.Data.SqlTypes.SqlDateTime.MaxValue.Value.ToString("yyyy-MM-ddTHH:mm:ss");

                                dsReferti = dwh.RefertiPerPaziente(idsac, datainiziostring, datafinestring);

                                if (dsReferti != null)
                {

                    foreach (DataRow oRowReferto in dsReferti.Tables["Referti"].Rows)
                    {

                                                bAdd = true;
                        if (sfiltrotipoDWH != string.Empty)
                            bAdd = (sfiltrotipoDWH == oRowReferto["SistemaErogante"].ToString());

                        if (sfiltrostatoDWH != string.Empty)
                            if (bAdd) bAdd = (sfiltrostatoDWH == oRowReferto["StatoRichiestaCodice"].ToString());

                        if (bAdd)
                        {
                            RefertoDWH oRefertoDWH = new RefertoDWH();

                            oRefertoDWH.IDReferto = oRowReferto["ID"].ToString();
                            oRefertoDWH.DataReferto = (DateTime)oRowReferto["DataReferto"];
                            oRefertoDWH.NumeroNosologico = oRowReferto["NumeroNosologico"].ToString();
                            oRefertoDWH.NumeroPrenotazione = oRowReferto["NumeroPrenotazione"].ToString();
                            oRefertoDWH.NumeroReferto = oRowReferto["NumeroReferto"].ToString();

                            oRefertoDWH.AziendaErogante = oRowReferto["AziendaErogante"].ToString();
                            oRefertoDWH.RepartoErogante = oRowReferto["RepartoErogante"].ToString();
                            oRefertoDWH.SistemaErogante = oRowReferto["SistemaErogante"].ToString();

                            oRefertoDWH.DWHCodRepartoRichiedente = oRowReferto["RepartoRichiedenteCodice"].ToString();
                            oRefertoDWH.DWHDescRepartoRichiedente = oRowReferto["RepartoRichiedenteDescr"].ToString();

                            oRefertoDWH.DWHCodStatoRichiesta = oRowReferto["StatoRichiestaCodice"].ToString();
                            oRefertoDWH.DWHDescStatoRichiesta = oRowReferto["StatoRichiestaDescr"].ToString();

                            oRefertoDWH.DWHCodTipoRichiesta = oRowReferto["TipoRichiestaCodice"].ToString();
                            oRefertoDWH.DWHDescTipoRichiesta = oRowReferto["TipoRichiestaDescr"].ToString();

                            if (dsfiltri != null)
                            {
                                oRefertoDWH.CodTipoEvidenzaClinica = UnicodeSrl.Scci.Statics.Database.DataTableFindValue(dsfiltri.Tables[Common.GC_TBTIPI], "CodTipoEvidenzaClinica", "CodDWH ='" + oRowReferto["SistemaErogante"].ToString() + "'").ToString();
                                oRefertoDWH.DescTipoEvidenzaClinica = UnicodeSrl.Scci.Statics.Database.DataTableFindValue(dsfiltri.Tables[Common.GC_TBTIPI], "Descrizione", "CodDWH ='" + oRowReferto["SistemaErogante"].ToString() + "'").ToString();

                                oRefertoDWH.CodStatoEvidenzaClinica = UnicodeSrl.Scci.Statics.Database.DataTableFindValue(dsfiltri.Tables[Common.GC_TBSTATI], "CodStatoEvidenzaClinica", "CodDWH ='" + oRowReferto["StatoRichiestaCodice"].ToString() + "'").ToString();
                                oRefertoDWH.DescStatoEvidenzaClinica = UnicodeSrl.Scci.Statics.Database.DataTableFindValue(dsfiltri.Tables[Common.GC_TBSTATI], "Descrizione", "CodDWH ='" + oRowReferto["StatoRichiestaCodice"].ToString() + "'").ToString();
                            }
                            else
                            {
                                oRefertoDWH.CodTipoEvidenzaClinica = string.Empty;
                                oRefertoDWH.DescTipoEvidenzaClinica = string.Empty;

                                oRefertoDWH.CodStatoEvidenzaClinica = string.Empty;
                                oRefertoDWH.DescStatoEvidenzaClinica = string.Empty;
                            }

                            oRefertoDWH.TestoAnteprima = oRowReferto["Anteprima"].ToString();

                            oRefertoDWH.DataEventoDWH = (DateTime)oRowReferto["DataEvento"];

                            oRefertoDWH.Firmato = oRowReferto["Firmato"].ToString();

                            oListRefertiDWH.Add(oRefertoDWH);

                                                        if (oRowReferto["SistemaErogante"].ToString() != "" && !codicidwh.Contains(oRowReferto["SistemaErogante"].ToString()))
                                codicidwh.Add(oRowReferto["SistemaErogante"].ToString());
                        }

                    }

                                        if (codicidwh.Count > 0) this.AlimentazioneTipiEvidenzaClinica(codicidwh);

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
                oListRefertiDWH = new List<RefertoDWH>();
            }

            return oListRefertiDWH;
        }

        public List<AllegatoRefertoDWH> CaricaRefertoDWH(string idreferto)
        {
            DataSet dsAllegati = null;
            UnicodeSrl.Scci.Statics.Database.ConnectionString = Common.ConnString;

            List<AllegatoRefertoDWH> oListaAllegatiDWH = new List<AllegatoRefertoDWH>();

            try
            {

                net.asmn.dwhclinico.DataAccessTdsV2 dwh = new net.asmn.dwhclinico.DataAccessTdsV2();
                dwh.Url = UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceDWH);

                                dwh.UseDefaultCredentials = false;
                dwh.Credentials = Credentials.CredenzialiScci(Credentials.enumTipoCredenziali.DWH);

                dsAllegati = dwh.RefertoPerId(idreferto);

                if (dsAllegati != null)
                {

                                        string sIdOrderEntry = "";
                    try
                    {
                        if (dsAllegati.Tables.Contains("RefertoAttributi") && dsAllegati.Tables["RefertoAttributi"].Rows.Count > 0)
                        {
                            DataRow[] oDr = dsAllegati.Tables["RefertoAttributi"].Select(@"Nome = 'IdOrderEntry'");
                            if (oDr.Length > 0) sIdOrderEntry = oDr[0]["Valore"].ToString();
                        }
                    }
                    catch
                    {
                    }

                    foreach (DataRow oRowReferto in dsAllegati.Tables["Referto"].Rows)
                    {
                        DataRow[] allegatifiler = dsAllegati.Tables["Allegati"].Select("IdRefertiBase = '" + oRowReferto["ID"].ToString() + "'");

                        foreach (DataRow oRowAllegato in allegatifiler)
                        {
                            AllegatoRefertoDWH oAllegatoRefertoDWH = new AllegatoRefertoDWH();

                            oAllegatoRefertoDWH.DataReferto = (DateTime)oRowReferto["DataReferto"];
                            oAllegatoRefertoDWH.IDReferto = oRowReferto["ID"].ToString();
                            oAllegatoRefertoDWH.NumeroReferto = oRowReferto["NumeroReferto"].ToString();
                            oAllegatoRefertoDWH.IDAllegato = oRowAllegato["Id"].ToString();
                            oAllegatoRefertoDWH.IdPaziente = oRowReferto["IdPaziente"].ToString();
                            oAllegatoRefertoDWH.DataInserimento = (DateTime)oRowReferto["DataInserimento"];
                            oAllegatoRefertoDWH.DataModifica = (DateTime)oRowReferto["DataModifica"];
                            oAllegatoRefertoDWH.NomeFile = oRowAllegato["NomeFile"].ToString();
                            oAllegatoRefertoDWH.DescrizioneFile = oRowAllegato["Descrizione"].ToString();
                            oAllegatoRefertoDWH.CodStatoAllegato = oRowAllegato["StatoCodice"].ToString();
                            oAllegatoRefertoDWH.DescrStatoAllegato = oRowAllegato["StatoDescrizione"].ToString();
                            oAllegatoRefertoDWH.MimeType = oRowAllegato["MimeType"].ToString();
                            oAllegatoRefertoDWH.FileData = (byte[])oRowAllegato["MimeData"];
                            oAllegatoRefertoDWH.IdOrderEntry = sIdOrderEntry;

                            
                            oListaAllegatiDWH.Add(oAllegatoRefertoDWH);
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
                oListaAllegatiDWH = new List<AllegatoRefertoDWH>();
            }

            return oListaAllegatiDWH;
        }

        public RefertoDWHDetailed CaricaRefertoDWHDettaglio(string idreferto)
        {
            DataSet dsReferto = null;
            UnicodeSrl.Scci.Statics.Database.ConnectionString = Common.ConnString;

            RefertoDWHDetailed oRefertoDWH = null;

            try
            {

                net.asmn.dwhclinico.DataAccessTdsV2 dwh = new net.asmn.dwhclinico.DataAccessTdsV2();
                dwh.Url = UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceDWH);

                                dwh.UseDefaultCredentials = false;
                dwh.Credentials = Credentials.CredenzialiScci(Credentials.enumTipoCredenziali.DWH);
                dsReferto = dwh.RefertoPerId(idreferto);

                if (dsReferto != null && dsReferto.Tables.Contains("Referto") && dsReferto.Tables["Referto"].Rows.Count > 0)
                {

                                        DataSet dsfiltri = this.CaricaTipiStatiEvidenzeCliniche();

                    DataRow oRowReferto = dsReferto.Tables["Referto"].Rows[0];

                    oRefertoDWH = new RefertoDWHDetailed();

                    oRefertoDWH.IDReferto = oRowReferto["ID"].ToString();
                    oRefertoDWH.DataReferto = (DateTime)oRowReferto["DataReferto"];
                    oRefertoDWH.NumeroNosologico = oRowReferto["NumeroNosologico"].ToString();
                    oRefertoDWH.NumeroPrenotazione = oRowReferto["NumeroPrenotazione"].ToString();
                    oRefertoDWH.NumeroReferto = oRowReferto["NumeroReferto"].ToString();

                    oRefertoDWH.AziendaErogante = oRowReferto["AziendaErogante"].ToString();
                    oRefertoDWH.RepartoErogante = oRowReferto["RepartoErogante"].ToString();
                    oRefertoDWH.SistemaErogante = oRowReferto["SistemaErogante"].ToString();

                    oRefertoDWH.DWHCodRepartoRichiedente = oRowReferto["RepartoRichiedenteCodice"].ToString();
                    oRefertoDWH.DWHDescRepartoRichiedente = oRowReferto["RepartoRichiedenteDescr"].ToString();

                    oRefertoDWH.DWHCodStatoRichiesta = oRowReferto["StatoRichiestaCodice"].ToString();
                    oRefertoDWH.DWHDescStatoRichiesta = oRowReferto["StatoRichiestaDescr"].ToString();

                    oRefertoDWH.DWHCodTipoRichiesta = oRowReferto["TipoRichiestaCodice"].ToString();
                    oRefertoDWH.DWHDescTipoRichiesta = oRowReferto["TipoRichiestaDescr"].ToString();

                    if (dsfiltri != null)
                    {
                        oRefertoDWH.CodTipoEvidenzaClinica = UnicodeSrl.Scci.Statics.Database.DataTableFindValue(dsfiltri.Tables[Common.GC_TBTIPI], "CodTipoEvidenzaClinica", "CodDWH ='" + oRowReferto["SistemaErogante"].ToString() + "'").ToString();
                        oRefertoDWH.DescTipoEvidenzaClinica = UnicodeSrl.Scci.Statics.Database.DataTableFindValue(dsfiltri.Tables[Common.GC_TBTIPI], "Descrizione", "CodDWH ='" + oRowReferto["SistemaErogante"].ToString() + "'").ToString();

                        oRefertoDWH.CodStatoEvidenzaClinica = UnicodeSrl.Scci.Statics.Database.DataTableFindValue(dsfiltri.Tables[Common.GC_TBSTATI], "CodStatoEvidenzaClinica", "CodDWH ='" + oRowReferto["StatoRichiestaCodice"].ToString() + "'").ToString();
                        oRefertoDWH.DescStatoEvidenzaClinica = UnicodeSrl.Scci.Statics.Database.DataTableFindValue(dsfiltri.Tables[Common.GC_TBSTATI], "Descrizione", "CodDWH ='" + oRowReferto["StatoRichiestaCodice"].ToString() + "'").ToString();
                    }
                    else
                    {
                        oRefertoDWH.CodTipoEvidenzaClinica = string.Empty;
                        oRefertoDWH.DescTipoEvidenzaClinica = string.Empty;

                        oRefertoDWH.CodStatoEvidenzaClinica = string.Empty;
                        oRefertoDWH.DescStatoEvidenzaClinica = string.Empty;
                    }

                    oRefertoDWH.TestoAnteprima = oRowReferto["Referto"].ToString();

                    if (!oRowReferto.IsNull("Sesso")) oRefertoDWH.Sesso = oRowReferto["Sesso"].ToString();
                    if (!oRowReferto.IsNull("CodiceSAUB")) oRefertoDWH.CodiceSAUB = oRowReferto["CodiceSAUB"].ToString();
                    if (!oRowReferto.IsNull("CodiceSanitario")) oRefertoDWH.CodiceSanitario = oRowReferto["CodiceSanitario"].ToString();
                    if (!oRowReferto.IsNull("PrioritaDescr")) oRefertoDWH.Priorita = oRowReferto["PrioritaDescr"].ToString();

                                        if (dsReferto.Tables.Contains("RefertoAttributi") && dsReferto.Tables["RefertoAttributi"].Rows.Count > 0)
                    {
                        DataRow[] oDr = dsReferto.Tables["RefertoAttributi"].Select(@"Nome = 'AccessNumber'");
                        if (oDr.Length > 0) oRefertoDWH.AccessNumber = oDr[0]["Valore"].ToString();
                    }
                                        if (dsReferto.Tables.Contains("RefertoAttributi") && dsReferto.Tables["RefertoAttributi"].Rows.Count > 0)
                    {
                        DataRow[] oDr = dsReferto.Tables["RefertoAttributi"].Select(@"Nome = 'IdOrderEntry'");
                        if (oDr.Length > 0) oRefertoDWH.IdOrderEntry = oDr[0]["Valore"].ToString();
                    }

                                                                                oRefertoDWH.PrestazioniReferto = new List<PrestazioneReferto>();
                    if (dsReferto.Tables.Contains("Prestazioni") && dsReferto.Tables["Prestazioni"].Rows.Count > 0)
                    {
                        foreach (DataRow drPrest in dsReferto.Tables["Prestazioni"].Rows)
                        {
                            try
                            {
                                PrestazioneReferto oPrest = new PrestazioneReferto();
                                oPrest.ID = drPrest["ID"].ToString();
                                oPrest.PrestazioneCodice = drPrest["PrestazioneCodice"].ToString();
                                oPrest.PrestazioneDescrizione = drPrest["PrestazioneDescrizione"].ToString();
                                oPrest.SezioneDescrizione = drPrest["SezioneDescrizione"].ToString();
                                int iPos = 0;
                                if (!drPrest.IsNull("SezionePosizione") && int.TryParse(drPrest["SezionePosizione"].ToString(), out iPos)) oPrest.SezionePosizione = iPos;
                                iPos = 0;
                                if (!drPrest.IsNull("PrestazionePosizione") && int.TryParse(drPrest["PrestazionePosizione"].ToString(), out iPos)) oPrest.PrestazionePosizione = iPos;

                                if (!drPrest.IsNull("Risultato")) oPrest.Risultato = drPrest["Risultato"].ToString();
                                if (!drPrest.IsNull("ValoriRiferimento")) oPrest.ValoriRiferimento = drPrest["ValoriRiferimento"].ToString();
                                if (!drPrest.IsNull("GravitaDescrizione")) oPrest.GravitaDescrizione = drPrest["GravitaDescrizione"].ToString();

                                                                if (dsReferto.Tables.Contains("PrestazioniAttributi") && dsReferto.Tables["PrestazioniAttributi"].Rows.Count > 0)
                                {
                                    try
                                    {
                                        DataRow[] oDr = dsReferto.Tables["PrestazioniAttributi"].Select(@"IdPrestazioniBase = '" + oPrest.ID + "' And Nome = 'AccessionNumber'");
                                        if (oDr.Length > 0) oPrest.AccessNumber = oDr[0]["Valore"].ToString().Split(';')[0];
                                    }
                                    catch
                                    {
                                    }
                                }


                                oRefertoDWH.PrestazioniReferto.Add(oPrest);
                            }
                            catch
                            {
                            }
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
                oRefertoDWH = null;
            }

            return oRefertoDWH;
        }

        public Dictionary<string, string> RicercaContenutiReferto(string idreferto, EnumTipoContenutiReferto tipo)
        {
                        UnicodeSrl.Scci.Statics.Database.ConnectionString = Common.ConnString;
            Dictionary<string, string> oListaContenutiReferto = new Dictionary<string, string>();

            net.asmn.dwhclinicolab.LayerFormatoType[] formati = null;

                        net.asmn.dwhclinicolab.DataAccessV2 dwhlab = new net.asmn.dwhclinicolab.DataAccessV2();
            dwhlab.Url = UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceDWHLAB);

                        dwhlab.UseDefaultCredentials = false;
            dwhlab.Credentials = Credentials.CredenzialiScci(Credentials.enumTipoCredenziali.DWH);

            try
            {
                formati = dwhlab.LayerRefertoFormatiPerIdReferto(idreferto);

                if (formati != null && formati.Length > 0)
                {
                    if (oListaContenutiReferto == null) oListaContenutiReferto = new Dictionary<string,string>();

                    foreach (net.asmn.dwhclinicolab.LayerFormatoType formato in formati)
                    {
                        if (formato.TipiFormato.Contains(TraduciTipoContenutoADWH(tipo)))
                        {
                            oListaContenutiReferto.Add(formato.Id, formato.Descrizione);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
                oListaContenutiReferto = new Dictionary<string, string>();
            }
            finally
            {
                if (formati != null) formati = null;

                if (dwhlab != null)
                {
                    dwhlab.Dispose();
                    dwhlab = null;
                }
            }

            return oListaContenutiReferto;

        }

        public string[] CaricaContenutiDaListaID(string idreferto, string[] idcontenuti, EnumTipoContenutiReferto tipo)
        {
            
            UnicodeSrl.Scci.Statics.Database.ConnectionString = Common.ConnString;

            net.asmn.dwhclinicolab.LayerRefertoFormattatoType refertoformattato = null;

            List<string> contenutiperid = new List<string>();
            
                        net.asmn.dwhclinicolab.DataAccessV2 dwhlab = new net.asmn.dwhclinicolab.DataAccessV2();
            dwhlab.Url = UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceDWHLAB);

                        dwhlab.UseDefaultCredentials = false;
            dwhlab.Credentials = Credentials.CredenzialiScci(Credentials.enumTipoCredenziali.DWH);

            try
            {
                                                                                

                                                
                refertoformattato = dwhlab.LayerRefertoFormattatoPerFormatiTipo(idreferto, idcontenuti, TraduciTipoContenutoADWH(tipo));

                if (refertoformattato != null)
                {
                    foreach (net.asmn.dwhclinicolab.LayerFormattazioneType formattazione in refertoformattato.Formattazioni)
                    {
                        if (formattazione.TipoFormato == TraduciTipoContenutoADWH(tipo))
                        {
                                                        contenutiperid.Add(System.Text.Encoding.UTF8.GetString(formattazione.Contenuto));
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
                contenutiperid = new List<string>();
            }
            finally
            {

                if (refertoformattato != null) refertoformattato = null;

                if (dwhlab != null)
                {
                    dwhlab.Dispose();
                    dwhlab = null;
                }
            }

            return contenutiperid.ToArray();
        }

                                                                                public List<RefertoDWH> RicercaRefertiDWHTipi(string idsac, DateTime datainizio, DateTime datafine, List<string> tipievidenzaclinica, List<string> statievidenzaclinica)
        {
            DataSet dsReferti = null;
            UnicodeSrl.Scci.Statics.Database.ConnectionString = Common.ConnString;

            List<RefertoDWH> oListRefertiDWH = new List<RefertoDWH>();

            string datainiziostring = string.Empty;
            string datafinestring = string.Empty;

                        List<string> codicidwh = new List<string>();

                        DataSet dsAllineaStatiETipi = this.CaricaTipiStatiEvidenzeCliniche();

                        bool bAdd = true;

                        List<string> lstfiltrotipiDWH = new List<string>();
            List<string> lstfiltrostatiDWH = new List<string>();

            try
            {

                                if (dsAllineaStatiETipi != null)
                {
                    if (tipievidenzaclinica != null && tipievidenzaclinica.Count > 0)
                    {
                        foreach (string codTipoSCCI in tipievidenzaclinica)
                        {
                            if (codTipoSCCI != null && codTipoSCCI.Trim() != "")
                            {

                                try
                                {
                                    string codTipoDWH = UnicodeSrl.Scci.Statics.Database.DataTableFindValue(dsAllineaStatiETipi.Tables[Common.GC_TBTIPI], "CodDWH", "CodTipoEvidenzaClinica ='" + codTipoSCCI + "'").ToString();
                                    if (codTipoDWH != null && codTipoDWH.Trim() != "") lstfiltrotipiDWH.Add(codTipoDWH);
                                }
                                catch
                                {
                                }
                            }
                        }
                    }

                    if (statievidenzaclinica != null && statievidenzaclinica.Count > 0)
                    {
                        foreach (string codStatoSCCI in statievidenzaclinica)
                        {
                            if (codStatoSCCI != null && codStatoSCCI.Trim() != "")
                            {
                                try
                                {
                                    string codStatoDWH = UnicodeSrl.Scci.Statics.Database.DataTableFindValue(dsAllineaStatiETipi.Tables[Common.GC_TBSTATI], "CodDWH", "CodStatoEvidenzaClinica ='" + codStatoSCCI + "'").ToString();
                                    if (codStatoDWH != null && codStatoDWH.Trim() != "") lstfiltrostatiDWH.Add(codStatoDWH);
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                    
                }
                else
                {
                    lstfiltrotipiDWH = new List<string>();
                    lstfiltrostatiDWH = new List<string>();

                }

                                net.asmn.dwhclinico.DataAccessTdsV2 dwh = new net.asmn.dwhclinico.DataAccessTdsV2();
                dwh.Url = UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceDWH);

                                dwh.UseDefaultCredentials = false;
                dwh.Credentials = Credentials.CredenzialiScci(Credentials.enumTipoCredenziali.DWH);

                                                if (datainizio != null && datainizio != DateTime.MinValue)
                    datainiziostring = datainizio.ToString("yyyy-MM-ddTHH:mm:ss");
                else
                    datainiziostring = System.Data.SqlTypes.SqlDateTime.MinValue.Value.ToString("yyyy-MM-ddTHH:mm:ss");

                if (datafine != null && datafine != DateTime.MinValue)
                    datafinestring = datafine.ToString("yyyy-MM-ddTHH:mm:ss");
                else
                    datafinestring = System.Data.SqlTypes.SqlDateTime.MaxValue.Value.ToString("yyyy-MM-ddTHH:mm:ss");

                                dsReferti = dwh.RefertiPerPaziente(idsac, datainiziostring, datafinestring);

                                if (dsReferti != null)
                {

                    foreach (DataRow oRowReferto in dsReferti.Tables["Referti"].Rows)
                    {

                                                bAdd = true;

                                                if (bAdd)
                        {
                            if (lstfiltrotipiDWH != null && lstfiltrotipiDWH.Count > 0)
                            {
                                bAdd = false;
                                foreach (string codTipoDWH in lstfiltrotipiDWH)
                                {
                                    if (codTipoDWH.Trim().ToUpper() == oRowReferto["SistemaErogante"].ToString().Trim().ToUpper()) bAdd = true;
                                }
                            }
                        }

                                                if (bAdd)
                        {
                            if (lstfiltrostatiDWH != null && lstfiltrostatiDWH.Count > 0)
                            {
                                bAdd = false;
                                foreach (string codStatoDWH in lstfiltrostatiDWH)
                                {
                                    if (codStatoDWH.Trim().ToUpper() == oRowReferto["StatoRichiestaCodice"].ToString().Trim().ToUpper()) bAdd = true;
                                }
                            }
                        }
                        

                        if (bAdd)
                        {
                            RefertoDWH oRefertoDWH = new RefertoDWH();

                            oRefertoDWH.IDReferto = oRowReferto["ID"].ToString();
                            oRefertoDWH.DataReferto = (DateTime)oRowReferto["DataReferto"];
                            oRefertoDWH.NumeroNosologico = oRowReferto["NumeroNosologico"].ToString();
                            oRefertoDWH.NumeroPrenotazione = oRowReferto["NumeroPrenotazione"].ToString();
                            oRefertoDWH.NumeroReferto = oRowReferto["NumeroReferto"].ToString();

                            oRefertoDWH.AziendaErogante = oRowReferto["AziendaErogante"].ToString();
                            oRefertoDWH.RepartoErogante = oRowReferto["RepartoErogante"].ToString();
                            oRefertoDWH.SistemaErogante = oRowReferto["SistemaErogante"].ToString();

                            oRefertoDWH.DWHCodRepartoRichiedente = oRowReferto["RepartoRichiedenteCodice"].ToString();
                            oRefertoDWH.DWHDescRepartoRichiedente = oRowReferto["RepartoRichiedenteDescr"].ToString();

                            oRefertoDWH.DWHCodStatoRichiesta = oRowReferto["StatoRichiestaCodice"].ToString();
                            oRefertoDWH.DWHDescStatoRichiesta = oRowReferto["StatoRichiestaDescr"].ToString();

                            oRefertoDWH.DWHCodTipoRichiesta = oRowReferto["TipoRichiestaCodice"].ToString();
                            oRefertoDWH.DWHDescTipoRichiesta = oRowReferto["TipoRichiestaDescr"].ToString();

                            if (dsAllineaStatiETipi != null)
                            {
                                oRefertoDWH.CodTipoEvidenzaClinica = UnicodeSrl.Scci.Statics.Database.DataTableFindValue(dsAllineaStatiETipi.Tables[Common.GC_TBTIPI], "CodTipoEvidenzaClinica", "CodDWH ='" + oRowReferto["SistemaErogante"].ToString() + "'").ToString();
                                oRefertoDWH.DescTipoEvidenzaClinica = UnicodeSrl.Scci.Statics.Database.DataTableFindValue(dsAllineaStatiETipi.Tables[Common.GC_TBTIPI], "Descrizione", "CodDWH ='" + oRowReferto["SistemaErogante"].ToString() + "'").ToString();

                                oRefertoDWH.CodStatoEvidenzaClinica = UnicodeSrl.Scci.Statics.Database.DataTableFindValue(dsAllineaStatiETipi.Tables[Common.GC_TBSTATI], "CodStatoEvidenzaClinica", "CodDWH ='" + oRowReferto["StatoRichiestaCodice"].ToString() + "'").ToString();
                                oRefertoDWH.DescStatoEvidenzaClinica = UnicodeSrl.Scci.Statics.Database.DataTableFindValue(dsAllineaStatiETipi.Tables[Common.GC_TBSTATI], "Descrizione", "CodDWH ='" + oRowReferto["StatoRichiestaCodice"].ToString() + "'").ToString();
                            }
                            else
                            {
                                oRefertoDWH.CodTipoEvidenzaClinica = string.Empty;
                                oRefertoDWH.DescTipoEvidenzaClinica = string.Empty;

                                oRefertoDWH.CodStatoEvidenzaClinica = string.Empty;
                                oRefertoDWH.DescStatoEvidenzaClinica = string.Empty;
                            }

                            oRefertoDWH.TestoAnteprima = oRowReferto["Anteprima"].ToString();

                            oRefertoDWH.DataEventoDWH = (DateTime)oRowReferto["DataEvento"];

                            oRefertoDWH.Firmato = oRowReferto["Firmato"].ToString();

                            oListRefertiDWH.Add(oRefertoDWH);

                                                        if (oRowReferto["SistemaErogante"].ToString() != "" && !codicidwh.Contains(oRowReferto["SistemaErogante"].ToString()))
                                codicidwh.Add(oRowReferto["SistemaErogante"].ToString());
                        }

                    }

                                        if (codicidwh.Count > 0) this.AlimentazioneTipiEvidenzaClinica(codicidwh);

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
                oListRefertiDWH = new List<RefertoDWH>();
            }

            return oListRefertiDWH;
        }

        #endregion

        #region Private Methods

        private void AlimentazioneTipiEvidenzaClinica(List<string> codicidwh)
        {
            try
            {
                if (codicidwh.Count > 0)
                {
                    Parametri op = new Parametri(new ScciAmbiente());
                    op.ParametroRipetibile.Add("CodTipoEvidenzaClinicaDWH", codicidwh.ToArray());

                                        SqlParameterExt[] spcoll = new SqlParameterExt[1];

                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    UnicodeSrl.Scci.Statics.Database.ExecStoredProc("MSP_ControlloTipoEvidenzaClinica", spcoll);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
            }

        }

        private DataSet CaricaTipiStatiEvidenzeCliniche()
        {
            DataSet dsRet = new DataSet();
            string sql = string.Empty;

            try
            {

                                sql = @"
                    SELECT 
	                    ALLDWH.CodDWH,
	                    ISNULL(SE.Codice, '') AS CodStatoEvidenzaClinica,
	                    SE.Descrizione
                    FROM 
	                    T_AllDWHStatoEvidenzaClinica ALLDWH LEFT JOIN T_StatoEvidenzaClinica SE ON
		                    ALLDWH.CodStatoEvidenzaClinica = SE.Codice 
                    ";

                dsRet.Tables.Add(UnicodeSrl.Scci.Statics.Database.GetDatatable(sql));
                dsRet.Tables[dsRet.Tables.Count - 1].TableName = Common.GC_TBSTATI;


                                sql = @"
                        SELECT
	                        ALLDWH.CodDWH,
	                        ISNULL(TE.Codice, '') AS CodTipoEvidenzaClinica,
	                        TE.Descrizione
                        FROM 
	                        T_AllDWHTipoEvidenzaClinica ALLDWH LEFT JOIN T_TipoEvidenzaClinica TE ON
		                        ALLDWH.CodTipoEvidenzaClinica = TE.Codice
                    ";

                dsRet.Tables.Add(UnicodeSrl.Scci.Statics.Database.GetDatatable(sql));
                dsRet.Tables[dsRet.Tables.Count - 1].TableName = Common.GC_TBTIPI;
            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
                dsRet = null;
            }

            return dsRet;

        }

        private EnumTipoContenutiReferto TraduciTipoContenutoDaDWH(WsSCCI.net.asmn.dwhclinicolab.LayerTipoFormatoEnum tipodwh)
        {
            switch (tipodwh)
            {
                case net.asmn.dwhclinicolab.LayerTipoFormatoEnum.RTF:
                    return EnumTipoContenutiReferto.RTF;

                case net.asmn.dwhclinicolab.LayerTipoFormatoEnum.TEXT:
                    return EnumTipoContenutiReferto.Testo;

                default:
                    return EnumTipoContenutiReferto.Testo;
            }
        }

        private WsSCCI.net.asmn.dwhclinicolab.LayerTipoFormatoEnum TraduciTipoContenutoADWH(EnumTipoContenutiReferto tiposcci)
        {
            switch (tiposcci)
            {
                case EnumTipoContenutiReferto.RTF:
                    return net.asmn.dwhclinicolab.LayerTipoFormatoEnum.RTF;

                case EnumTipoContenutiReferto.Testo:
                    return net.asmn.dwhclinicolab.LayerTipoFormatoEnum.TEXT;

                default:
                    return net.asmn.dwhclinicolab.LayerTipoFormatoEnum.TEXT;
            }
        }

        #endregion
    }
}
