using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WsSCCI.net.asmn.orderentry;
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Scci.Enums;

namespace WsSCCI
{
        [ServiceContract]
    public interface IScciOrderEntry
    {

        #region DIZIONARI

        [OperationContract]
        List<OESistemaErogante> GetEroganti(string s_user);

        [OperationContract]
        List<OESistemaErogante> GetEroganti2(string s_user);

        [OperationContract]
        List<OEPrestazione> GetPrestazioni(string s_user, string codiceregime, OEPrioritaOrdine priorita, string codiceAzienda, string codiceUnita, string codiceAziendaSistemaErogante, string codiceSistemaErogante, string filtro);

        [OperationContract]
        List<OEPrestazione> GetProfili(string s_user, string codiceregime, OEPrioritaOrdine priorita, string codiceAzienda, string codiceUnita, string codiceAziendaSistemaErogante, string codiceSistemaErogante, string filtro);

        [OperationContract]
        List<OEPrestazione> GetProfiliUtente(string s_user, string codiceregime, OEPrioritaOrdine priorita, string codiceAzienda, string codiceUnita, string codiceAziendaSistemaErogante, string codiceSistemaErogante, string filtro);

        [OperationContract]
        List<OEPrestazione> GetPrestazioniInProfilo(string s_user, OEPrestazione prestazione);

        #endregion

        #region ORDINI

        [OperationContract]
        OEOrdineTestata CreaOrdine(string s_user, string s_nome, string s_cognome, string idOrdine, string nosologico, string s_listaattesa, string regime,
                                          OEPrioritaOrdine priorita, string codiceFiscale, string idPaziente, string cognome,
                                          string nome, DateTime dataNascita, string codiceAzienda,
                                          string codiceUnitaOperativa, string s_workstation);

        [OperationContract]
        OEOrdineDettaglio CreaOrdineEsteso(string s_user, string s_nome, string s_cognome, string idOrdine, string nosologico, string s_listaattesa, string regime,
                                           OEPrioritaOrdine priorita, string codiceFiscale, string idPaziente, string cognome,
                                           string nome, DateTime dataNascita, string codiceAzienda,
                                           string codiceUnitaOperativa, string s_workstation);
        [OperationContract]
        bool SalvaOrdine(string s_user, string s_nome, string s_cognome, string idOrdine, string nosologico, string s_listaattesa, string regime,
                                          OEPrioritaOrdine priorita, DateTime? dataPrenotazione, string codiceAzienda,
                                          string codiceUnitaOperativa, string s_workstation);
        [OperationContract]
        OEOrdineDettaglio SalvaOrdineEsteso(string s_user, string s_nome, string s_cognome, string idOrdine, string nosologico, string s_listaattesa, string regime,
                                            OEPrioritaOrdine priorita, DateTime? dataPrenotazione, string codiceAzienda,
                                            string codiceUnitaOperativa, string s_workstation);
        [OperationContract]
        string getLastException();
        [OperationContract]
        OEOrdineTestata CreaOrdineCopia(string s_user, string idOEorigine, string idOrdineNuovo, string s_workstation);
        [OperationContract]
        OEOrdineTestata CreaOrdineCopia2(string s_user, string s_nome, string s_cognome, string idOEorigine, string idOrdineNuovo, string s_workstation);
        [OperationContract]
        OEOrdineTestata CreaOrdineCopia3(string s_user, string idOEorigine, string idOrdineNuovo, string s_nosologiconuovo, string s_aziendauo, string s_codiceuo, bool b_copiadatiaccessori, string s_workstation);

        [OperationContract]
        bool CancellaOrdine(string s_user, string idOE);

        [OperationContract]
        OEOrdineTestata GetOrdineTestata(string s_user, string idOE);

        [OperationContract]
        OEOrdineDettaglio GetOrdineDettaglioPerTestata(string s_user, OEOrdineTestata o_testata);

        [OperationContract]
        OEOrdineDettaglio GetOrdineDettaglio(string s_user, string idOE);


        [OperationContract]
        List<OEOrdineTestata> CercaOrdiniPerPaziente(string s_user, DateTime dataInizio, DateTime dataFine, string idSacPaziente);

        [OperationContract]
        List<OEOrdineTestata> CercaOrdiniPerNosologico(string s_user, DateTime dataInizio, DateTime dataFine, string nosologico, string s_listaattesa);

        [OperationContract]
        bool InoltraOrdine(string s_user, string s_nome, string s_cognome, string idOE);

        [OperationContract]
        bool InoltraOrdineDaWorkstation(string s_user, string s_nome, string s_cognome, string idOE, string s_workstation);

        [OperationContract]
        OEOrdineDettaglio InoltraOrdineDaWorkstationEsteso(string s_user, string s_nome, string s_cognome, string idOE, string s_workstation);

        [OperationContract]
        List<OEGruppoPrestazione> GetGruppiPreferenziali(string s_user, string codiceregime, OEPrioritaOrdine priorita,
                                                                string codiceAzienda, string codiceUnita, string codiceAziendaSistemaRichiedente,
                                                                string codiceSistemaRichiedente, string filtro);

        [OperationContract]
        List<OEPrestazione> GetPrestazioniInGruppo(string s_user, string idgruppo, string filtroprestazioni);

        [OperationContract]
        List<OEPrestazione> GetPrestazioniInGruppo2(string s_user, string codiceregime, OEPrioritaOrdine priorita,
                                                           string codiceAzienda, string codiceUnita, string codiceAziendaSistemaRichiedente,
                                                           string codiceSistemaRichiedente, string idgruppo, string filtroprestazioni);

        [OperationContract]
        List<OEOrdineTestata> CercaOrdiniPerStatoPianificato(string s_user, DateTime dataInizio, DateTime dataFine, 
                                                                string CodAziendaSistemaErogante, string CodSistemaErogante,
                                                                List<KeyValuePair<string, string>> unitaoperative,
                                                                string nosologico, string cognome, string nome, DateTime? datanascita, 
                                                                int maxrecord);

        #endregion

        #region PRESTAZIONI

        [OperationContract]
        bool InserisciPrestazioni(string s_user, string idOE, List<OEPrestazione> prestazioni);

        [OperationContract]
        bool InserisciPrestazione(string s_user, string idOE, OEPrestazione prestazione);

        [OperationContract]
        bool CancellaPrestazioni(string s_user, string idOE, List<OEPrestazione> prestazioni);

        [OperationContract]
        bool CancellaPrestazione(string s_user, string idOE, OEPrestazione prestazione);

        [OperationContract]
        bool EsplodiProfilo(string s_user, string idOE, OEPrestazione prestazione);

        [OperationContract]
        List<OEPrestazione> PrestazioniRecenti(string s_user, string regime, OEPrioritaOrdine priorita, string codAzienda, string codUnita, string conAziendaRichiedente, string codSistRichiedente, string codAziendaSistErogante, string codSistErogante, string idSacPaziente, string filtroCodiceDescrizione);

        [OperationContract]
        string TranslatePrioritaDescFromOE(OEPrioritaOrdine oepri);

        #endregion

        #region PROFILI

        [OperationContract]
        bool CreaProfilo(string s_user, string descrizione, List<OEPrestazione> prestazioni);

        [OperationContract]
        bool EliminaProfilo(string s_user, string CodiceProfilo);

        #endregion

        #region DATIACCESSORI

        [OperationContract]
        List<OEDatiAccessoriDescrittore> GetDatiAccessorNecessari(string s_user, string idOE);

        [OperationContract]
        List<OEDatoAccessorio> GetDatiAccessoriOld(string s_user, string idOE);

        [OperationContract]
        List<OEDatoAccessorio> GetDatiAccessori(string s_user, string idOE);

        [OperationContract]
        bool FillDatiAccessori(string s_user, string idOE, List<OEDatoAccessorio> datiAggiuntivi);

        [OperationContract]
        OEOrdineDettaglio FillDatiAccessoriEsteso(string s_user, string idOE, List<OEDatoAccessorio> datiAggiuntivi);

        [OperationContract]
        List<OEDatoAccessorio> GetDatiAccessoriTestata(string s_user, string idOE);

        [OperationContract]
        List<OEDatoAccessorio> GetDatiAccessoriPrestazione(string s_user, string idOE, OEPrestazione o_prestazione);

        [OperationContract]
        List<OEDatoAccessorio> GetDatiAccessoriErogante(string s_user, string idOE, OESistemaErogante o_erogante);

        [OperationContract]
        List<OEDatoAccessorio> GetDatiAccessoriErogante2(string s_user, string idOE);

        [OperationContract]
        List<OEDatoAccessorio> GetDatiAccessoriEroganteTestata(string s_user, string idOE, OESistemaErogante o_erogante);

        [OperationContract]
        List<OEDatoAccessorio> GetDatiAccessoriErogantePrestazione(string s_user, string idOE, OEPrestazione o_prestazione);

        #endregion

        #region COMPARISON

        [OperationContract]
        bool ComparePrestazione(OEPrestazione o_prestazione, RigaRichiestaType o_prestazione_oe);

        #endregion

    }
}
