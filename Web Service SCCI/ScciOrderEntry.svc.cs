
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WsSCCI.net.asmn.orderentry;
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Framework.Data;
using System.Data;
using UnicodeSrl.Scci;
using System.ServiceModel.Channels;
using System.Globalization;
using UnicodeSrl.ScciCore.Debugger;

namespace WsSCCI
{

    public class OEGruppoPrestazioneComparer : IComparer<OEGruppoPrestazione>
    {
        public int Compare(OEGruppoPrestazione x, OEGruppoPrestazione y)
        {
                                                
            int retval = 0;

            if (x == null)
            {
                if (y == null)
                {
                                        retval = 0;
                }
                else
                {
                                        retval = -1;
                }
            }
            else
            {
                                if (y == null)
                {
                                        retval = 1;
                }
                else
                {
                                        retval = String.Compare(x.Descrizione, y.Descrizione);
                }
            }

            return retval;
        }
    }

    public class OEDatiAccessoriComparer : IComparer<OEDatiAccessoriDescrittore>
    {
        public int Compare(OEDatiAccessoriDescrittore x, OEDatiAccessoriDescrittore y)
        {
                                                
            int retval = 0;

            if (x == null)
            {
                if (y == null)
                {
                                        retval = 0;
                }
                else
                {
                                        retval = -1;
                }
            }
            else
            {
                                if (y == null)
                {
                                        retval = 1;
                }
                else
                {
                                        if (x.Ordinamento == y.Ordinamento)
                    {
                        retval = 0;
                    }
                    else
                    {
                        if (x.Ordinamento > y.Ordinamento)
                        {
                            retval = 1;
                        }
                        else
                        {
                            retval = -1;
                        }
                    }
                }
            }

            return retval;
        }
    }

    public class ScciOrderEntry : IScciOrderEntry
    {
        OrderEntryV1Client oe;

        Exception eStored;

        #region DIZIONARI

        public List<OESistemaErogante> GetEroganti(string s_user)
        {

            List<OESistemaErogante> risultato;
            DizionariType diz;
            risultato = new List<OESistemaErogante>();
            if (checkConnectionOE())
                try
                {
                    diz = oe.OttieniListeDizionari(creaToken(s_user));

                    foreach (SistemaType p in diz.SistemiEroganti)
                        risultato.Add(TranslateSistemaFromOE(p));
                }
                catch (Exception ex)
                {
                    ErrorHandler.AddException(ex);
                }
            return risultato;
        }

        public List<OESistemaErogante> GetEroganti2(string s_user)
        {

            List<OESistemaErogante> risultato;
            DizionariType diz;
            risultato = new List<OESistemaErogante>();
            if (checkConnectionOE())
                try
                {
                    diz = oe.OttieniListeDizionari(creaToken(s_user));

                    foreach (SistemaErogante p in diz.SistemiEroganti2)
                    {
                        if (p.AccessoLettura)
                        {
                            risultato.Add(TranslateSistemaFromOE(p));
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorHandler.AddException(ex);
                }
            return risultato;
        }

        public List<OEPrestazione> GetPrestazioni(string s_user, string codiceregime, OEPrioritaOrdine priorita,
                                                  string codiceAzienda, string codiceUnita, string codiceAziendaSistemaErogante,
                                                  string codiceSistemaErogante, string filtro)
        {
            PrestazioniListaType prestazioni;
            List<OEPrestazione> risultato;

            risultato = new List<OEPrestazione>();

            if (checkConnectionOE())
                try
                {

                    prestazioni = oe.CercaPrestazioniPerCodiceODescrizione(
                        creaToken(s_user),
                        TranslateRegimeToOE(codiceregime),
                        TranslatePrioritaToOE(priorita),
                        codiceAzienda,
                        codiceUnita,
                        "ASMN",
                        "SCCI",
                        codiceAziendaSistemaErogante,
                        codiceSistemaErogante,
                        filtro);

                    if (prestazioni != null)
                    {
                        foreach (PrestazioneListaType prestazione in prestazioni)
                        {
                            risultato.Add(TranslatePrestazioneErogabileFromOE(prestazione));
                        }
                    }

                }
                catch (Exception ex)
                {
                    ErrorHandler.AddException(ex);
                }
            return risultato;
        }

        public List<OEPrestazione> GetProfili(string s_user, string codiceregime, OEPrioritaOrdine priorita,
                                              string codiceAzienda, string codiceUnita, string codiceAziendaSistemaErogante,
                                              string codiceSistemaErogante, string filtro)
        {
            PrestazioniListaType profili;
            List<OEPrestazione> risultato;

            risultato = new List<OEPrestazione>();

            if (checkConnectionOE())
                try
                {

                    List<TipoPrestazioneErogabileEnum> array = new List<TipoPrestazioneErogabileEnum>();
                    array.Add(TipoPrestazioneErogabileEnum.ProfiloBlindato);
                    array.Add(TipoPrestazioneErogabileEnum.ProfiloScomponibile);
                    array.Add(TipoPrestazioneErogabileEnum.ProfiloUtente);


                    profili = oe.CercaProfiliPerCodiceODescrizione(
                        creaToken(s_user),
                        TranslateRegimeToOE(codiceregime),
                        TranslatePrioritaToOE(priorita),
                        codiceAzienda,
                        codiceUnita,
                        "ASMN",
                        "SCCI",
                        filtro,
                            array.ToArray()
                                    );

                    if (profili != null)
                    {
                        foreach (PrestazioneListaType profilo in profili)
                            risultato.Add(TranslatePrestazioneErogabileFromOE(profilo));
                    }
                }
                catch (Exception ex)
                {
                    ErrorHandler.AddException(ex);

                }
            return risultato;

        }

        public List<OEPrestazione> GetProfiliUtente(string s_user, string codiceregime, OEPrioritaOrdine priorita,
                                                    string codiceAzienda, string codiceUnita, string codiceAziendaSistemaErogante,
                                                    string codiceSistemaErogante, string filtro)
        {
            PrestazioniListaType profili;
            List<OEPrestazione> risultato;


            risultato = new List<OEPrestazione>();

            if (checkConnectionOE())
                try
                {

                    List<TipoPrestazioneErogabileEnum> array = new List<TipoPrestazioneErogabileEnum>();
                    array.Add(TipoPrestazioneErogabileEnum.ProfiloUtente);


                    profili = oe.CercaProfiliPerCodiceODescrizione(
                        creaToken(s_user),
                        TranslateRegimeToOE(codiceregime),
                        TranslatePrioritaToOE(priorita),
                        codiceAzienda,
                        codiceUnita,
                        "ASMN",
                        "SCCI",
                       filtro,
                       array.ToArray()
                                    );

                    if (profili != null)
                    {
                        foreach (PrestazioneListaType profilo in profili)
                            risultato.Add(TranslatePrestazioneErogabileFromOE(profilo));
                    };

                }
                catch (Exception ex)
                {
                    ErrorHandler.AddException(ex);

                }
            return risultato;
        }

        public List<OEPrestazione> GetPrestazioniInProfilo(string s_user, OEPrestazione prestazione)
        {
            PrestazioneErogabileType prestazioneoe;
            List<OEPrestazione> risultato;

            risultato = new List<OEPrestazione>();

            if (checkConnectionOE() && prestazione != null)
                try
                {
                    prestazioneoe = oe.OttieniPrestazionePerCodice(creaToken(s_user), prestazione.Erogante.CodiceAzienda, prestazione.Erogante.Codice, prestazione.Codice);

                    if (prestazioneoe == null)
                        return null;

                    if (prestazioneoe.Tipo == TipoPrestazioneErogabileEnum.Prestazione)
                        return null;

                    foreach (PrestazioneErogabileType prestazioneinprofilo in prestazioneoe.Prestazioni)
                        risultato.Add(TranslatePrestazioneErogabileFromOE(prestazioneinprofilo));

                }
                catch (Exception ex)
                {
                    ErrorHandler.AddException(ex);

                }
            return risultato;
        }

        public List<OEGruppoPrestazione> GetGruppiPreferenziali(string s_user, string codiceregime, OEPrioritaOrdine priorita,
                                                                string codiceAzienda, string codiceUnita, string codiceAziendaSistemaRichiedente,
                                                                string codiceSistemaRichiedente, string filtro)
        {

            List<OEGruppoPrestazione> oRet = new List<OEGruppoPrestazione>();
            GruppiPrestazioniListaType gruppi;

            if (checkConnectionOE())
                try
                {

                    gruppi = oe.CercaGruppiPrestazioniPerDescrizione(creaToken(s_user), TranslateRegimeToOE(codiceregime),
                                                                    TranslatePrioritaToOE(priorita), codiceAzienda, codiceUnita,
                                                                    codiceAziendaSistemaRichiedente, codiceSistemaRichiedente, filtro);
                    if (gruppi != null)
                    {
                        foreach (GruppoPrestazioniListaType gruppo in gruppi)
                        {
                            oRet.Add(this.TranslateGruppoPrestazioniFromOE(gruppo));
                        }
                                                oRet.Sort(new OEGruppoPrestazioneComparer());
                    }
                    else
                    {
                        oRet = null;
                    }

                }
                catch (Exception ex)
                {
                    ErrorHandler.AddException(ex);
                }

            return oRet;

        }

        public List<OEPrestazione> GetPrestazioniInGruppo(string s_user, string idgruppo, string filtroprestazioni)
        {
            GruppoPrestazioniType prestazioneoe;
            List<OEPrestazione> oRet = new List<OEPrestazione>();

            if (checkConnectionOE() && idgruppo != null && idgruppo != string.Empty)
                try
                {

                    prestazioneoe = oe.OttieniGruppoPrestazioniPerId(creaToken(s_user), idgruppo);

                    if (prestazioneoe != null && prestazioneoe.Prestazioni != null)
                    {
                        foreach (PrestazioneListaType prestazione in prestazioneoe.Prestazioni)
                        {
                            if (filtroprestazioni == string.Empty || prestazione.Descrizione.ToLower().Contains(filtroprestazioni.ToLower()))
                                oRet.Add(TranslatePrestazioneErogabileFromOE(prestazione));
                        }
                    }
                    else
                    {
                        oRet = new List<OEPrestazione>();
                    }
                }
                catch (Exception ex)
                {
                    ErrorHandler.AddException(ex);
                    oRet = new List<OEPrestazione>();
                }

            return oRet;
        }

        public List<OEPrestazione> GetPrestazioniInGruppo2(string s_user, string codiceregime, OEPrioritaOrdine priorita,
                                                           string codiceAzienda, string codiceUnita, string codiceAziendaSistemaRichiedente,
                                                           string codiceSistemaRichiedente, string idgruppo, string filtroprestazioni)
        {
            PrestazioniListaType prestazionilista = null;
            List<OEPrestazione> oRet = new List<OEPrestazione>();

            if (checkConnectionOE() && idgruppo != null && idgruppo != string.Empty)
                try
                {

                    prestazionilista = oe.CercaPrestazioniPerGruppoPrestazioni(creaToken(s_user), TranslateRegimeToOE(codiceregime),
                                                                               TranslatePrioritaToOE(priorita), codiceAzienda, codiceUnita,
                                                                               codiceAziendaSistemaRichiedente, codiceSistemaRichiedente,
                                                                               null, null, idgruppo, filtroprestazioni);

                    if (prestazionilista != null && prestazionilista.Count > 0)
                    {
                        foreach (PrestazioneListaType prestazione in prestazionilista)
                        {
                            if (filtroprestazioni == string.Empty || prestazione.Descrizione.ToLower().Contains(filtroprestazioni.ToLower()))
                                oRet.Add(TranslatePrestazioneErogabileFromOE(prestazione));
                        }
                    }
                    else
                    {
                        oRet = new List<OEPrestazione>();
                    }
                }
                catch (Exception ex)
                {
                    ErrorHandler.AddException(ex);
                    oRet = new List<OEPrestazione>();
                }

            return oRet;
        }

        #endregion

        #region ORDINI

        public OEOrdineTestata CreaOrdine(string s_user, string s_nome, string s_cognome, string idOrdine, string nosologico, string s_listaattesa, string regime,
                                          OEPrioritaOrdine priorita, string codiceFiscale, string idPaziente, string cognome,
                                          string nome, DateTime dataNascita, string codiceAzienda,
                                          string codiceUnitaOperativa, string s_workstation)
        {
            OrdineType ordine;
            StatoType stato;
            DatoNomeValoreType datoaggiuntivo;

            OEOrdineTestata oRet = new OEOrdineTestata();
            TokenAccessoType token = null;

            
            if (checkConnectionOE())
                try
                {
                                        token = creaToken(s_user);
                    
                    ordine = new OrdineType();

                    ordine.IdRichiestaRichiedente = idOrdine;
                    ordine.Regime = new RegimeType();
                    ordine.Regime.Codice = TranslateRegimeToOE(regime).ToString();
                    ordine.Priorita = new PrioritaType();
                    ordine.Priorita.Codice = TranslatePrioritaToOE(priorita).ToString();
                    if (nosologico.Length > 0)
                        ordine.NumeroNosologico = nosologico;
                    else
                        ordine.NumeroNosologico = s_listaattesa;
                    ordine.DataRichiesta = DateTime.Now;
                    ordine.DataPrenotazione = null;
                    ordine.SistemaRichiedente = new SistemaType();

                    ordine.SistemaRichiedente.Azienda = new CodiceDescrizioneType();
                    ordine.SistemaRichiedente.Azienda.Codice = "ASMN";
                    ordine.SistemaRichiedente.Sistema = new CodiceDescrizioneType();
                    ordine.SistemaRichiedente.Sistema.Codice = "SCCI";
                    ordine.UnitaOperativaRichiedente = new StrutturaType();
                    ordine.UnitaOperativaRichiedente.Azienda = new CodiceDescrizioneType();
                    ordine.UnitaOperativaRichiedente.Azienda.Codice = codiceAzienda;
                    ordine.UnitaOperativaRichiedente.UnitaOperativa = new CodiceDescrizioneType();
                    ordine.UnitaOperativaRichiedente.UnitaOperativa.Codice = "MAT" + codiceUnitaOperativa;

                    if (ordine.Operatore == null)
                        ordine.Operatore = new OperatoreType();

                    ordine.Operatore.ID = s_user;
                    ordine.Operatore.Nome = s_nome;
                    ordine.Operatore.Cognome = s_cognome;

                    ordine.RigheRichieste = new RigheRichiesteType();

                    
                    datoaggiuntivo = new DatoNomeValoreType();
                    datoaggiuntivo.Nome = "PCInvioRichiesta";
                    datoaggiuntivo.TipoDato = "xs:string";
                    datoaggiuntivo.ValoreDato = s_workstation;

                    ordine.DatiAggiuntivi = new DatiAggiuntiviType();
                    ordine.DatiAggiuntivi.Add(datoaggiuntivo);

                    ordine.Paziente = new PazienteType();
                    
                                        ordine = oe.CompilaPazienteSac(token, idPaziente, ordine);
                    
                                        stato = oe.AggiungiOppureModificaOrdine(token, ordine);
                    
                    if (stato == null)
                        oRet = new OEOrdineTestata();
                    else
                        oRet = TranslateOrdineTestataFromOE(stato);

                                                        }
                catch (Exception ex)
                {
                    ErrorHandler.AddException(ex);
                    oRet = new OEOrdineTestata();
                }
            else
                oRet = new OEOrdineTestata();

            return oRet;
        }

        public OEOrdineDettaglio CreaOrdineEsteso(string s_user, string s_nome, string s_cognome, string idOrdine, string nosologico, string s_listaattesa, string regime,
                                                  OEPrioritaOrdine priorita, string codiceFiscale, string idPaziente, string cognome,
                                                  string nome, DateTime dataNascita, string codiceAzienda,
                                                  string codiceUnitaOperativa, string s_workstation)
        {

            OEOrdineDettaglio oRet = new OEOrdineDettaglio();

            try
            {
                if (checkConnectionOE())
                {

                    OrdineType ordine;
                    StatoType stato;
                    DatoNomeValoreType datoaggiuntivo;

                    TokenAccessoType token = null;

                    token = creaToken(s_user);

                    ordine = new OrdineType();

                    ordine.IdRichiestaRichiedente = idOrdine;
                    ordine.Regime = new RegimeType();
                    ordine.Regime.Codice = TranslateRegimeToOE(regime).ToString();
                    ordine.Priorita = new PrioritaType();
                    ordine.Priorita.Codice = TranslatePrioritaToOE(priorita).ToString();
                    if (nosologico.Length > 0)
                        ordine.NumeroNosologico = nosologico;
                    else
                        ordine.NumeroNosologico = s_listaattesa;
                    ordine.DataRichiesta = DateTime.Now;
                    ordine.DataPrenotazione = null;
                    ordine.SistemaRichiedente = new SistemaType();

                    ordine.SistemaRichiedente.Azienda = new CodiceDescrizioneType();
                    ordine.SistemaRichiedente.Azienda.Codice = "ASMN";
                    ordine.SistemaRichiedente.Sistema = new CodiceDescrizioneType();
                    ordine.SistemaRichiedente.Sistema.Codice = "SCCI";
                    ordine.UnitaOperativaRichiedente = new StrutturaType();
                    ordine.UnitaOperativaRichiedente.Azienda = new CodiceDescrizioneType();
                    ordine.UnitaOperativaRichiedente.Azienda.Codice = codiceAzienda;
                    ordine.UnitaOperativaRichiedente.UnitaOperativa = new CodiceDescrizioneType();
                    ordine.UnitaOperativaRichiedente.UnitaOperativa.Codice = "MAT" + codiceUnitaOperativa;

                    if (ordine.Operatore == null)
                        ordine.Operatore = new OperatoreType();

                    ordine.Operatore.ID = s_user;
                    ordine.Operatore.Nome = s_nome;
                    ordine.Operatore.Cognome = s_cognome;

                    ordine.RigheRichieste = new RigheRichiesteType();

                    datoaggiuntivo = new DatoNomeValoreType();
                    datoaggiuntivo.Nome = "PCInvioRichiesta";
                    datoaggiuntivo.TipoDato = "xs:string";
                    datoaggiuntivo.ValoreDato = s_workstation;

                    ordine.DatiAggiuntivi = new DatiAggiuntiviType();
                    ordine.DatiAggiuntivi.Add(datoaggiuntivo);

                    ordine.Paziente = new PazienteType();

                    ordine = oe.CompilaPazienteSac(token, idPaziente, ordine);

                    stato = oe.AggiungiOppureModificaOrdine(token, ordine);

                    if (stato == null)
                        oRet = new OEOrdineDettaglio();
                    else
                        oRet = TranslateOrdineDettaglioFromOE(stato, TranslateOrdineTestataFromOE(stato));
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
                oRet = new OEOrdineDettaglio();
            }

            return oRet;
        }

        public bool SalvaOrdine(string s_user, string s_nome, string s_cognome, string idOrdine, string nosologico, string s_listaattesa, string regime,
                                          OEPrioritaOrdine priorita, DateTime? dataPrenotazione, string codiceAzienda,
                                          string codiceUnitaOperativa, string s_workstation)
        {
            OrdineType ordine;
            StatoType stato;
            DatoNomeValoreType datoaggiuntivo;

            TokenAccessoType token = null;

            
            if (checkConnectionOE())
                try
                {
                                        token = creaToken(s_user);
                    
                                        stato = oe.OttieniOrdinePerIdRichiesta(token, idOrdine);
                    

                    if (stato == null)
                        return false;

                    ordine = stato.Ordine;

                    ordine.Regime = new RegimeType();
                    ordine.Regime.Codice = TranslateRegimeToOE(regime).ToString();
                    ordine.Priorita = new PrioritaType();
                    ordine.Priorita.Codice = TranslatePrioritaToOE(priorita).ToString();
                    if (nosologico.Length > 0)
                        ordine.NumeroNosologico = nosologico;
                    else
                        ordine.NumeroNosologico = s_listaattesa;
                    ordine.DataPrenotazione = dataPrenotazione;

                    ordine.UnitaOperativaRichiedente = new StrutturaType();
                    ordine.UnitaOperativaRichiedente.Azienda = new CodiceDescrizioneType();
                    ordine.UnitaOperativaRichiedente.Azienda.Codice = codiceAzienda;
                    ordine.UnitaOperativaRichiedente.UnitaOperativa = new CodiceDescrizioneType();
                    ordine.UnitaOperativaRichiedente.UnitaOperativa.Codice = "MAT" + codiceUnitaOperativa;
                    
                    
                    if (ordine.DatiAggiuntivi == null)
                        ordine.DatiAggiuntivi = new DatiAggiuntiviType();
                    datoaggiuntivo = null;
                    if (ordine.DatiAggiuntivi != null)
                        foreach (DatoNomeValoreType o_dato in ordine.DatiAggiuntivi)
                            if (o_dato.Nome == "PCInvioRichiesta")
                                datoaggiuntivo = o_dato;
                    if (datoaggiuntivo == null)
                    {
                        datoaggiuntivo = new DatoNomeValoreType();
                        datoaggiuntivo.Nome = "PCInvioRichiesta";
                        datoaggiuntivo.TipoDato = "xs:string";
                        ordine.DatiAggiuntivi.Add(datoaggiuntivo);
                    };
                    datoaggiuntivo.ValoreDato = s_workstation;

                    
                    if (ordine.Operatore == null)
                        ordine.Operatore = new OperatoreType();

                    ordine.Operatore.ID = s_user;
                    ordine.Operatore.Nome = s_nome;
                    ordine.Operatore.Cognome = s_cognome;

                    
                    stato = oe.AggiungiOppureModificaOrdine(token, ordine);

                    
                    
                    
                    if (stato == null)
                        return false;
                    else
                        return true;

                }
                catch (Exception ex)
                {
                    ErrorHandler.AddException(ex);
                    eStored = ex;

                    return false;
                }
            else
                return false;
        }

        public OEOrdineDettaglio SalvaOrdineEsteso(string s_user, string s_nome, string s_cognome, string idOrdine, string nosologico, string s_listaattesa, string regime,
                                                   OEPrioritaOrdine priorita, DateTime? dataPrenotazione, string codiceAzienda,
                                                   string codiceUnitaOperativa, string s_workstation)
        {

            OEOrdineDettaglio retOrdine = null;

            try
            {
                OrdineType ordine;
                StatoType stato = null;
                DatoNomeValoreType datoaggiuntivo;

                TokenAccessoType token = null;

                if (checkConnectionOE())
                {
                    token = creaToken(s_user);

                    stato = oe.OttieniOrdinePerIdRichiesta(token, idOrdine);

                    if (stato != null)
                    {

                        ordine = stato.Ordine;

                        ordine.Regime = new RegimeType();
                        ordine.Regime.Codice = TranslateRegimeToOE(regime).ToString();
                        ordine.Priorita = new PrioritaType();
                        ordine.Priorita.Codice = TranslatePrioritaToOE(priorita).ToString();
                        if (nosologico.Length > 0)
                            ordine.NumeroNosologico = nosologico;
                        else
                            ordine.NumeroNosologico = s_listaattesa;
                        ordine.DataPrenotazione = dataPrenotazione;

                        ordine.UnitaOperativaRichiedente = new StrutturaType();
                        ordine.UnitaOperativaRichiedente.Azienda = new CodiceDescrizioneType();
                        ordine.UnitaOperativaRichiedente.Azienda.Codice = codiceAzienda;
                        ordine.UnitaOperativaRichiedente.UnitaOperativa = new CodiceDescrizioneType();
                        ordine.UnitaOperativaRichiedente.UnitaOperativa.Codice = "MAT" + codiceUnitaOperativa;
                        
                        if (ordine.DatiAggiuntivi == null)
                            ordine.DatiAggiuntivi = new DatiAggiuntiviType();
                        datoaggiuntivo = null;
                        if (ordine.DatiAggiuntivi != null)
                            foreach (DatoNomeValoreType o_dato in ordine.DatiAggiuntivi)
                                if (o_dato.Nome == "PCInvioRichiesta")
                                    datoaggiuntivo = o_dato;
                        if (datoaggiuntivo == null)
                        {
                            datoaggiuntivo = new DatoNomeValoreType();
                            datoaggiuntivo.Nome = "PCInvioRichiesta";
                            datoaggiuntivo.TipoDato = "xs:string";
                            ordine.DatiAggiuntivi.Add(datoaggiuntivo);
                        };
                        datoaggiuntivo.ValoreDato = s_workstation;

                        if (ordine.Operatore == null)
                            ordine.Operatore = new OperatoreType();

                        ordine.Operatore.ID = s_user;
                        ordine.Operatore.Nome = s_nome;
                        ordine.Operatore.Cognome = s_cognome;

                        stato = oe.AggiungiOppureModificaOrdine(token, ordine);


                        if (stato != null)
                            retOrdine = TranslateOrdineDettaglioFromOE(stato, TranslateOrdineTestataFromOE(stato));

                    }
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
                eStored = ex;
            }

            return retOrdine;
        }


        public string getLastException()
        {
            if (eStored != null)
                return eStored.ToString();
            else return "no exception";
        }

        public OEOrdineTestata CreaOrdineCopia(string s_user, string idOEorigine, string idOrdineNuovo, string s_workstation)
        {
            return CreaOrdineCopia2(s_user, s_user, s_user, idOEorigine, idOrdineNuovo, s_workstation);
        }

        public OEOrdineTestata CreaOrdineCopia2(string s_user, string s_nome, string s_cognome, string idOEorigine, string idOrdineNuovo, string s_workstation)
        {
            StatoType ordine;
            StatoType stato;
            DatoNomeValoreType datoaggiuntivo;

            OEOrdineTestata oRet = new OEOrdineTestata();
            TokenAccessoType token = null;

            if (checkConnectionOE())
                try
                {
                    token = creaToken(s_user);
                    ordine = oe.OttieniOrdinePerIdRichiesta(token, idOEorigine);

                    ordine.Ordine.DatiAggiuntivi = null;
                    ordine.Ordine.DatiAggiuntivi = new DatiAggiuntiviType();

                    datoaggiuntivo = new DatoNomeValoreType();
                    datoaggiuntivo.Nome = "PCInvioRichiesta";
                    datoaggiuntivo.TipoDato = "xs:string";
                    ordine.Ordine.DatiAggiuntivi.Add(datoaggiuntivo);
                    datoaggiuntivo.ValoreDato = s_workstation;

                    foreach (RigaRichiestaType o_riga in ordine.Ordine.RigheRichieste)
                    {
                        o_riga.DatiAggiuntivi = null;
                        if (ordine.DescrizioneStato == StatoDescrizioneEnum.Cancellato) o_riga.OperazioneOrderEntry = OperazioneRigaRichiestaOrderEntryEnum.IS;
                    }

                    ordine.Ordine.Operatore.ID = s_user;
                    ordine.Ordine.Operatore.Nome = s_nome;
                    ordine.Ordine.Operatore.Cognome = s_cognome;

                    ordine.Ordine.DataRichiesta = DateTime.Now;
                    ordine.Ordine.Data = DateTime.Now;
                    ordine.Ordine.IdGuidOrderEntry = "";
                    ordine.Ordine.IdRichiestaRichiedente = idOrdineNuovo;
                    ordine.Ordine.IdRichiestaOrderEntry = "";

                    string idPaziente = ordine.Ordine.Paziente.IdSac;
                    ordine.Ordine.Paziente = new PazienteType();
                    ordine.Ordine = oe.CompilaPazienteSac(token, idPaziente, ordine.Ordine);

                    stato = oe.AggiungiOppureModificaOrdine(creaToken(s_user), ordine.Ordine);

                    if (stato == null)
                        oRet = new OEOrdineTestata();
                    else
                        oRet = TranslateOrdineTestataFromOE(stato);
                }
                catch (Exception ex)
                {
                    ErrorHandler.AddException(ex);
                    oRet = new OEOrdineTestata();

                }
            return oRet;

        }

        public OEOrdineTestata CreaOrdineCopia3(string s_user, string idOEorigine, string idOrdineNuovo, string s_nosologiconuovo, string s_aziendauo, string s_codiceuo, bool b_copiadatiaccessori, string s_workstation)
        {

            StatoType ordine;
            StatoType stato;
            OEOrdineTestata oRet = new OEOrdineTestata();
            TokenAccessoType token = null;

            if (checkConnectionOE())
            {

                try
                {

                    token = creaToken(s_user);
                    ordine = oe.OttieniOrdinePerIdRichiesta(token, idOEorigine);

                    stato = oe.CopiaOrdinePerIdRichiesta(token,
                                                           idOEorigine,
                                                           ordine.Ordine.Paziente.IdSac,
                                                           idOrdineNuovo,
                                                           s_nosologiconuovo,
                                                           s_aziendauo,
                                                           s_codiceuo,
                                                           b_copiadatiaccessori);
                    if (stato == null)
                    {
                        oRet = new OEOrdineTestata();
                    }
                    else
                    {
                        oRet = TranslateOrdineTestataFromOE(stato);
                    }

                }
                catch (Exception ex)
                {
                    ErrorHandler.AddException(ex);
                    oRet = new OEOrdineTestata();
                }

            }

            return oRet;

        }

        public bool CancellaOrdine(string s_user, string idOE)
        {
            if (checkConnectionOE())
                try
                {
                    oe.CancellaOrdinePerIdRichiesta(creaToken(s_user), idOE);
                    return true;
                }
                catch (Exception ex)
                {
                    ErrorHandler.AddException(ex);

                }
            return false;
        }

        public OEOrdineTestata GetOrdineTestata(string s_user, string idOE)
        {
            StatoType ordine;
            OEOrdineTestata oRet = new OEOrdineTestata();

            if (checkConnectionOE())
                try
                {
                    ordine = oe.OttieniOrdinePerIdRichiesta(creaToken(s_user), idOE);

                    if (ordine == null)
                        oRet = new OEOrdineTestata();

                    oRet = TranslateOrdineTestataFromOE(ordine);
                }
                catch (Exception ex)
                {
                    ErrorHandler.AddException(ex);

                }
            return oRet;
        }

        public OEOrdineDettaglio GetOrdineDettaglioPerTestata(string s_user, OEOrdineTestata o_testata)
        {
            StatoType ordine;
            OEOrdineDettaglio oRet = new OEOrdineDettaglio();
            if (checkConnectionOE())
                try
                {
                    ordine = oe.OttieniOrdinePerIdRichiesta(creaToken(s_user), o_testata.NumeroOrdine);

                    if (ordine == null)
                        oRet = new OEOrdineDettaglio();

                    oRet = TranslateOrdineDettaglioFromOE(ordine, o_testata);
                }
                catch (Exception ex)
                {
                    ErrorHandler.AddException(ex);

                }
            return oRet;
        }

        public OEOrdineDettaglio GetOrdineDettaglio(string s_user, string idOE)
        {
            StatoType ordine = null;

            
            if (checkConnectionOE())
                try
                {

                                        ordine = oe.OttieniOrdinePerIdRichiesta(creaToken(s_user), idOE);
                    
                                                        }

                catch (Exception ex)
                {
                    ErrorHandler.AddException(ex);
                    ordine = null;
                }
            else
                ordine = null;

            return TranslateOrdineDettaglioFromOE(ordine, GetOrdineTestata(s_user, idOE));

        }

        public List<OEOrdineTestata> CercaOrdiniPerPaziente(string s_user, DateTime dataInizio, DateTime dataFine, string idSacPaziente)
        {
            List<OEOrdineTestata> listaOrdini;
            OrdiniListaType ordini;
            listaOrdini = new List<OEOrdineTestata>();
            if (checkConnectionOE())
                try
                {
                    try
                    { ordini = oe.CercaOrdiniPerPaziente(creaToken(s_user), dataInizio, dataFine, idSacPaziente); }
                    catch
                    { ordini = null; }

                    if (ordini != null)
                    {
                        listaOrdini = new List<OEOrdineTestata>();
                        foreach (OrdineListaType ordine in ordini)
                            listaOrdini.Add(TranslateOrdineTestataFromOE(ordine));
                                            }
                }
                catch (Exception ex)
                {
                    ErrorHandler.AddException(ex);
                    listaOrdini = new List<OEOrdineTestata>();
                }
            else
                listaOrdini = new List<OEOrdineTestata>();

            return listaOrdini;
        }

        public List<OEOrdineTestata> CercaOrdiniPerNosologico(string s_user, DateTime dataInizio, DateTime dataFine, string nosologico, string s_listaattesa)
        {
            List<OEOrdineTestata> listaOrdini = new List<OEOrdineTestata>();
            OrdiniListaType ordini = null;
            TokenAccessoType token = null;

            if (checkConnectionOE())
                try
                {
                    listaOrdini = new List<OEOrdineTestata>();

                    if (nosologico == s_listaattesa) s_listaattesa = "";


                    Parametri op = new Parametri(new ScciAmbiente());
                    op.Parametro.Add("NumeroNosologico", nosologico);
                    op.Parametro.Add("NumeroListaAttesa", s_listaattesa);

                                        SqlParameterExt[] spcoll = new SqlParameterExt[1];

                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    DataTable dt = UnicodeSrl.Scci.Statics.Database.GetDataTableStoredProc("MSP_SelNumeroEpisodioDaNosologicoLA", spcoll);

                    if (dt != null && dt.Rows.Count > 0)
                    {

                        token = creaToken(s_user);

                        foreach (DataRow orow in dt.Rows)
                        {

                            ordini = null;

                            if (orow["NumeroNosologico"] != DBNull.Value && orow["NumeroNosologico"].ToString() != String.Empty)
                            {
                                try
                                {
                                    ordini = oe.CercaOrdiniPerNosologico(token, dataInizio, dataFine, orow["NumeroNosologico"].ToString());
                                }
                                catch
                                {
                                    ordini = null;
                                }

                                if (ordini != null)
                                {
                                    foreach (OrdineListaType ordine in ordini)
                                        listaOrdini.Add(TranslateOrdineTestataFromOE(ordine));
                                }
                            }

                            ordini = null;

                            if (orow["NumeroListaAttesa"] != DBNull.Value && orow["NumeroListaAttesa"].ToString() != String.Empty)
                            {
                                try
                                {
                                    ordini = oe.CercaOrdiniPerNosologico(token, dataInizio, dataFine, orow["NumeroListaAttesa"].ToString());
                                }
                                catch
                                {
                                    ordini = null;
                                }

                                if (ordini != null)
                                {
                                    foreach (OrdineListaType ordine in ordini)
                                        listaOrdini.Add(TranslateOrdineTestataFromOE(ordine));
                                }
                            }

                        }
                    }
                    else
                    {
                        listaOrdini = new List<OEOrdineTestata>();
                    }

                }
                catch (Exception ex)
                {
                    ErrorHandler.AddException(ex);
                    listaOrdini = new List<OEOrdineTestata>();
                }
            else
                listaOrdini = new List<OEOrdineTestata>();

            return listaOrdini;
        }

        public bool InoltraOrdine(string s_user, string s_nome, string s_cognome, string idOE)
        {
            if (!checkConnectionOE())
                return false;

            StatoType stato;

            OrdineType ordine;
            try
            {

                stato = oe.OttieniOrdinePerIdRichiesta(creaToken(s_user), idOE);
                ordine = stato.Ordine;

                if (ordine.Operatore == null)
                    ordine.Operatore = new OperatoreType();

                ordine.Operatore.ID = s_user;
                ordine.Operatore.Nome = s_nome;
                ordine.Operatore.Cognome = s_cognome;

                                ordine.DataRichiesta = DateTime.Now;
                stato = oe.AggiungiOppureModificaOrdine(creaToken(s_user), ordine);

                oe.InoltraOrdinePerIdRichiesta(creaToken(s_user), idOE);

                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);

            }
            return false;
        }

        public bool InoltraOrdineDaWorkstation(string s_user, string s_nome, string s_cognome, string idOE, string s_workstation)
        {
            if (!checkConnectionOE())
                return false;

            StatoType stato;
            DatoNomeValoreType datoaggiuntivo;

            OrdineType ordine;

            TokenAccessoType token = null;

            try
            {

                token = creaToken(s_user);
                stato = oe.OttieniOrdinePerIdRichiesta(token, idOE);
                ordine = stato.Ordine;

                if (ordine.Operatore == null)
                    ordine.Operatore = new OperatoreType();

                ordine.Operatore.ID = s_user;
                ordine.Operatore.Nome = s_nome;
                ordine.Operatore.Cognome = s_cognome;

                if (ordine.DatiAggiuntivi == null)
                    ordine.DatiAggiuntivi = new DatiAggiuntiviType();
                datoaggiuntivo = null;
                if (ordine.DatiAggiuntivi != null)
                    foreach (DatoNomeValoreType o_dato in ordine.DatiAggiuntivi)
                        if (o_dato.Nome == "PCInvioRichiesta")
                            datoaggiuntivo = o_dato;
                if (datoaggiuntivo == null)
                {
                    datoaggiuntivo = new DatoNomeValoreType();
                    datoaggiuntivo.Nome = "PCInvioRichiesta";
                    datoaggiuntivo.TipoDato = "xs:string";
                    ordine.DatiAggiuntivi.Add(datoaggiuntivo);
                };
                datoaggiuntivo.ValoreDato = s_workstation;

                                bool b_rad = false;
                DateTime dt_dataoraprogrammata;
                DatoNomeValoreType o_datoaccessorio;
                DatoNomeValoreType o_dato_damodificare;

                #region Personalizzazione Radiologia Disattivato
                                                                
                                                                                                
                                                                                                                                                
                                                                                                                                                
                                

                
                                                                                                                                                                                                                                                                                                                                                
                                                
                
                                                                                                                                                                                                                
                                                                                                                                                                
                
                                                                                                                                                                                                                
                                                                                
                
                                                                                                                                                                                                                                
                                                                                                                                                                                                                                
                                                                                                                                                                                                                                
                                                #endregion (Disattivato)

                
                o_dato_damodificare = null;
                foreach (DatoNomeValoreType o_datoaggiuntivo in ordine.DatiAggiuntivi)
                    if (o_datoaggiuntivo.Nome == "MedicoRichiedenteCodice")
                        o_dato_damodificare = o_datoaggiuntivo;
                if (o_dato_damodificare == null)
                {
                    o_datoaccessorio = new DatoNomeValoreType();
                    o_datoaccessorio.Nome = "MedicoRichiedenteCodice";
                    o_datoaccessorio.TipoDato = "xs:string";
                    o_datoaccessorio.ValoreDato = "";
                    ordine.DatiAggiuntivi.Add(o_datoaccessorio);
                    o_dato_damodificare = o_datoaccessorio;
                };
                o_dato_damodificare.ValoreDato = s_user;

                o_dato_damodificare = null;
                foreach (DatoNomeValoreType o_datoaggiuntivo in ordine.DatiAggiuntivi)
                    if (o_datoaggiuntivo.Nome == "MedicoRichiedenteCognome")
                        o_dato_damodificare = o_datoaggiuntivo;
                if (o_dato_damodificare == null)
                {
                    o_datoaccessorio = new DatoNomeValoreType();
                    o_datoaccessorio.Nome = "MedicoRichiedenteCognome";
                    o_datoaccessorio.TipoDato = "xs:string";
                    o_datoaccessorio.ValoreDato = "";
                    ordine.DatiAggiuntivi.Add(o_datoaccessorio);
                    o_dato_damodificare = o_datoaccessorio;
                };
                o_dato_damodificare.ValoreDato = s_cognome;

                o_dato_damodificare = null;
                foreach (DatoNomeValoreType o_datoaggiuntivo in ordine.DatiAggiuntivi)
                    if (o_datoaggiuntivo.Nome == "MedicoRichiedenteNome")
                        o_dato_damodificare = o_datoaggiuntivo;
                if (o_dato_damodificare == null)
                {
                    o_datoaccessorio = new DatoNomeValoreType();
                    o_datoaccessorio.Nome = "MedicoRichiedenteNome";
                    o_datoaccessorio.TipoDato = "xs:string";
                    o_datoaccessorio.ValoreDato = "";
                    ordine.DatiAggiuntivi.Add(o_datoaccessorio);
                    o_dato_damodificare = o_datoaccessorio;
                };
                o_dato_damodificare.ValoreDato = s_nome;

                string idPaziente = ordine.Paziente.IdSac;
                ordine.Paziente = new PazienteType();
                ordine = oe.CompilaPazienteSac(token, idPaziente, ordine);

                                ordine.DataRichiesta = DateTime.Now;

                stato = oe.AggiungiOppureModificaOrdine(token, ordine);

                oe.InoltraOrdinePerIdRichiesta(token, idOE);


                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);

            }
            return false;
        }

        public OEOrdineDettaglio InoltraOrdineDaWorkstationEsteso(string s_user, string s_nome, string s_cognome, string idOE, string s_workstation)
        {
            OEOrdineDettaglio retOrdine = null;

            try
            {
                if (checkConnectionOE())
                {
                    StatoType stato;
                    DatoNomeValoreType datoaggiuntivo;
                    OrdineType ordine;
                    TokenAccessoType token = null;


                    token = creaToken(s_user);
                    stato = oe.OttieniOrdinePerIdRichiesta(token, idOE);
                    ordine = stato.Ordine;

                    if (ordine.Operatore == null)
                        ordine.Operatore = new OperatoreType();

                    ordine.Operatore.ID = s_user;
                    ordine.Operatore.Nome = s_nome;
                    ordine.Operatore.Cognome = s_cognome;

                    if (ordine.DatiAggiuntivi == null)
                        ordine.DatiAggiuntivi = new DatiAggiuntiviType();
                    datoaggiuntivo = null;
                    if (ordine.DatiAggiuntivi != null)
                        foreach (DatoNomeValoreType o_dato in ordine.DatiAggiuntivi)
                            if (o_dato.Nome == "PCInvioRichiesta")
                                datoaggiuntivo = o_dato;
                    if (datoaggiuntivo == null)
                    {
                        datoaggiuntivo = new DatoNomeValoreType();
                        datoaggiuntivo.Nome = "PCInvioRichiesta";
                        datoaggiuntivo.TipoDato = "xs:string";
                        ordine.DatiAggiuntivi.Add(datoaggiuntivo);
                    };
                    datoaggiuntivo.ValoreDato = s_workstation;

                                        DatoNomeValoreType o_datoaccessorio;
                    DatoNomeValoreType o_dato_damodificare;

                    
                    o_dato_damodificare = null;
                    foreach (DatoNomeValoreType o_datoaggiuntivo in ordine.DatiAggiuntivi)
                        if (o_datoaggiuntivo.Nome == "MedicoRichiedenteCodice")
                            o_dato_damodificare = o_datoaggiuntivo;
                    if (o_dato_damodificare == null)
                    {
                        o_datoaccessorio = new DatoNomeValoreType();
                        o_datoaccessorio.Nome = "MedicoRichiedenteCodice";
                        o_datoaccessorio.TipoDato = "xs:string";
                        o_datoaccessorio.ValoreDato = "";
                        ordine.DatiAggiuntivi.Add(o_datoaccessorio);
                        o_dato_damodificare = o_datoaccessorio;
                    };
                    o_dato_damodificare.ValoreDato = s_user;

                    o_dato_damodificare = null;
                    foreach (DatoNomeValoreType o_datoaggiuntivo in ordine.DatiAggiuntivi)
                        if (o_datoaggiuntivo.Nome == "MedicoRichiedenteCognome")
                            o_dato_damodificare = o_datoaggiuntivo;
                    if (o_dato_damodificare == null)
                    {
                        o_datoaccessorio = new DatoNomeValoreType();
                        o_datoaccessorio.Nome = "MedicoRichiedenteCognome";
                        o_datoaccessorio.TipoDato = "xs:string";
                        o_datoaccessorio.ValoreDato = "";
                        ordine.DatiAggiuntivi.Add(o_datoaccessorio);
                        o_dato_damodificare = o_datoaccessorio;
                    };
                    o_dato_damodificare.ValoreDato = s_cognome;

                    o_dato_damodificare = null;
                    foreach (DatoNomeValoreType o_datoaggiuntivo in ordine.DatiAggiuntivi)
                        if (o_datoaggiuntivo.Nome == "MedicoRichiedenteNome")
                            o_dato_damodificare = o_datoaggiuntivo;
                    if (o_dato_damodificare == null)
                    {
                        o_datoaccessorio = new DatoNomeValoreType();
                        o_datoaccessorio.Nome = "MedicoRichiedenteNome";
                        o_datoaccessorio.TipoDato = "xs:string";
                        o_datoaccessorio.ValoreDato = "";
                        ordine.DatiAggiuntivi.Add(o_datoaccessorio);
                        o_dato_damodificare = o_datoaccessorio;
                    };
                    o_dato_damodificare.ValoreDato = s_nome;

                    string idPaziente = ordine.Paziente.IdSac;
                    ordine.Paziente = new PazienteType();
                    ordine = oe.CompilaPazienteSac(token, idPaziente, ordine);

                                        ordine.DataRichiesta = DateTime.Now;

                    stato = oe.AggiungiOppureModificaOrdine(token, ordine);

                    stato = oe.InoltraOrdinePerIdRichiesta(token, idOE);

                    if (stato != null)
                        retOrdine = TranslateOrdineDettaglioFromOE(stato, TranslateOrdineTestataFromOE(stato));
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);

            }
            return retOrdine;
        }

        public List<OEOrdineTestata> CercaOrdiniPerStatoPianificato(string s_user, DateTime dataInizio, DateTime dataFine,
                                                                    string CodAziendaSistemaErogante, string CodSistemaErogante,
                                                                    List<KeyValuePair<string, string>> unitaoperative,
                                                                    string nosologico, string cognome, string nome, DateTime? datanascita, int maxrecord)
        {

            List<OEOrdineTestata> listaOrdini;
            OrdiniListaType ordini;
            listaOrdini = new List<OEOrdineTestata>();

            if (checkConnectionOE())
            {

                try
                {

                    try
                    {
                        ordini = oe.CercaOrdiniPerStatoPianificato(creaToken(s_user),
                                                                    dataInizio,
                                                                    dataFine,
                                                                    null,
                                                                    CodAziendaSistemaErogante,
                                                                    CodSistemaErogante,
                                                                    creaunitaoperative(unitaoperative),
                                                                    nosologico,
                                                                    cognome,
                                                                    nome,
                                                                    datanascita,
                                                                    null,
                                                                    maxrecord);
                    }
                    catch
                    {
                        ordini = null;
                    }

                    if (ordini != null)
                    {
                        listaOrdini = new List<OEOrdineTestata>();
                        foreach (OrdineListaType ordine in ordini)
                        {
                            listaOrdini.Add(TranslateOrdineTestataFromOE(ordine));
                        }
                    }

                }
                catch (Exception ex)
                {
                    ErrorHandler.AddException(ex);
                    listaOrdini = new List<OEOrdineTestata>();
                }

            }
            else
            {
                listaOrdini = new List<OEOrdineTestata>();
            }

            return listaOrdini;

        }

        private StrutturaType[] creaunitaoperative(List<KeyValuePair<string, string>> uo)
        {

            StrutturaType[] arvRet = null;

            try
            {

                if (uo != null && uo.Count > 0)
                {

                    List<StrutturaType> lst_StrutturaType = new List<StrutturaType>();

                    foreach (KeyValuePair<string, string> pair in uo)
                    {
                        StrutturaType oStrutturaType = new StrutturaType();
                        oStrutturaType.Azienda = new CodiceDescrizioneType();
                        oStrutturaType.Azienda.Codice = pair.Key;
                        oStrutturaType.UnitaOperativa = new CodiceDescrizioneType();
                        oStrutturaType.UnitaOperativa.Codice = pair.Value;
                        lst_StrutturaType.Add(oStrutturaType);
                    }

                    arvRet = lst_StrutturaType.ToArray();

                }

            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
            }

            return arvRet;

        }

        #endregion

        #region PRESTAZIONI

        public bool InserisciPrestazioni(string s_user, string idOE, List<OEPrestazione> prestazioni)
        {
            bool bret = false;

            if (checkConnectionOE() && prestazioni != null && prestazioni.Count > 0)
                try
                {
                    OEOrdineDettaglio ordine = this.GetOrdineDettaglio(s_user, idOE);

                    foreach (OEPrestazione prestazione in prestazioni)
                    {
                        if (ordine.Prestazioni.Find(o => o.Prestazione.Codice == prestazione.Codice &&
                                                          o.Prestazione.Erogante.Codice == prestazione.Erogante.Codice &&
                                                          o.Prestazione.Erogante.CodiceAzienda == prestazione.Erogante.CodiceAzienda) == null)
                        {
                            this.InserisciPrestazioneNoCheck(s_user, idOE, prestazione);
                        }
                    }

                    bret = true;
                }
                catch (Exception ex)
                {
                    ErrorHandler.AddException(ex);
                    bret = false;
                }
            else
                bret = false;

            return bret;
        }

        private bool InserisciPrestazioneNoCheck(string s_user, string idOE, OEPrestazione prestazione)
        {

            bool bRet = false;

            try
            {
                if (checkConnectionOE() && prestazione != null)
                {
                    oe.AggiungiPrestazionePerIdRichiestaPerCodicePrestazione(creaToken(s_user), idOE, prestazione.Codice, prestazione.Erogante.CodiceAzienda, prestazione.Erogante.Codice);
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
                bRet = false;
            }

            return bRet;
        }

        public bool InserisciPrestazione(string s_user, string idOE, OEPrestazione prestazione)
        {
                                                
            if (!checkConnectionOE() || prestazione == null)
                return false;

            try
            {

                OEOrdineDettaglio ordine = this.GetOrdineDettaglio(s_user, idOE);

                if (ordine.Prestazioni.Find(o => o.Prestazione.Codice == prestazione.Codice &&
                                                  o.Prestazione.Erogante.Codice == prestazione.Erogante.Codice &&
                                                  o.Prestazione.Erogante.CodiceAzienda == prestazione.Erogante.CodiceAzienda) == null)
                {
                    oe.AggiungiPrestazionePerIdRichiestaPerCodicePrestazione(creaToken(s_user), idOE, prestazione.Codice, prestazione.Erogante.CodiceAzienda, prestazione.Erogante.Codice);
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
            }
            return false;
        }

        public bool CancellaPrestazioni(string s_user, string idOE, List<OEPrestazione> prestazioni)
        {
            bool bret = false;

            if (checkConnectionOE() && prestazioni != null && prestazioni.Count > 0)
                try
                {
                    foreach (OEPrestazione prestazione in prestazioni)
                        this.CancellaPrestazione(s_user, idOE, prestazione);

                    bret = true;
                }
                catch (Exception ex)
                {
                    ErrorHandler.AddException(ex);
                    bret = false;
                }
            else
                bret = false;

            return bret;
        }

        public bool CancellaPrestazione(string s_user, string idOE, OEPrestazione prestazione)
        {
                        
            if (!checkConnectionOE() || prestazione == null)
                return false;


            try
            {

                oe.RimuoviPrestazionePerIdRichiestaPerCodicePrestazione(creaToken(s_user), idOE, prestazione.Codice, prestazione.Erogante.CodiceAzienda, prestazione.Erogante.Codice);

                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);

            }
            return false;
        }

        public bool EsplodiProfilo(string s_user, string idOE, OEPrestazione prestazione)
        {
            List<OEPrestazione> prestazioniinprofilo;

            if (!checkConnectionOE() || prestazione == null)
                return false;

            try
            {
                prestazioniinprofilo = GetPrestazioniInProfilo(s_user, prestazione);

                if (prestazioniinprofilo == null)
                    return false;

                this.InserisciPrestazioni(s_user, idOE, prestazioniinprofilo);

                CancellaPrestazione(s_user, idOE, prestazione);

                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);

            }
            return false;
        }

        public List<OEPrestazione> PrestazioniRecenti(string s_user, string regime, OEPrioritaOrdine priorita, string codAzienda,
                                                      string codUnita, string codAziendaRichiedente, string codSistRichiedente,
                                                      string codAziendaSistErogante, string codSistErogante, string idSacPaziente,
                                                      string filtroCodiceDescrizione)
        {
            List<OEPrestazione> prestazioniOE;
            PrestazioniListaType prestazioni;
            if (!checkConnectionOE())
                return null;


            try
            {

                prestazioni = new PrestazioniListaType();
                prestazioni = oe.CercaPrestazioniPerPaziente(creaToken(s_user), TranslateRegimeToOE(regime),
                                TranslatePrioritaToOE(priorita), codAzienda, codUnita, codAziendaRichiedente,
                                codSistRichiedente, codAziendaSistErogante, codSistErogante, idSacPaziente,
                                filtroCodiceDescrizione);
                prestazioniOE = new List<OEPrestazione>();

                if (prestazioni != null)
                    foreach (PrestazioneListaType pres in prestazioni)
                        prestazioniOE.Add(TranslatePrestazioneErogabileFromOE(pres));

                return prestazioniOE;
            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);

            }
            return null;

        }

        #endregion

        #region PROFILI

        public bool CreaProfilo(string s_user, string descrizione, List<OEPrestazione> prestazioni)
        {
            ProfiloUtenteType profilo;
            ProfiloUtenteType profiloNew;
            ProfiloUtentePrestazioneType pres;

            if (!checkConnectionOE())
                return false;


            try
            {
                profilo = new ProfiloUtenteType();
                profilo.Prestazioni = new ProfiloUtentePrestazioniType();

                profilo.Descrizione = descrizione;


                foreach (OEPrestazione prestazione in prestazioni)                     {
                    pres = new ProfiloUtentePrestazioneType();

                    pres.Codice = prestazione.Codice;
                    pres.Descrizione = prestazione.Descrizione;

                    pres.SistemaErogante = TranslateSistemaToOE(prestazione.Erogante);
                    pres.Tipo = TranslatePrestazioneTipoToOE(prestazione.Tipo);
                    pres.Id = "";

                    profilo.Prestazioni.Add(pres);
                }

                profilo.Id = "";

                profiloNew = oe.AggiungiOppureModificaProfiloUtente(creaToken(s_user), profilo);

                if (profiloNew == null)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);

            }
            return false;
        }

        public bool EliminaProfilo(string s_user, string CodiceProfilo)
        {

            if (!checkConnectionOE())
                return false;
            try
            {
                oe.CancellaProfiloUtentePerCodice(creaToken(s_user), CodiceProfilo);

            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);

            }
            return false;
        }

        #endregion

        #region DATIACCESSORI


        private string MappaDatiAccessoriIngresso(DatoNomeValoreType dato)
        {

            if (dato == null || dato.DatoAccessorio == null)
            {
                return string.Empty;
            }
            else
            {
                string sOutput = string.Empty;
                string sValoreDato = string.Empty;

                sValoreDato = dato.ValoreDato;

                switch (dato.DatoAccessorio.Tipo)
                {
                    case TipoDatoAccessorioEnum.DateBox:
                        DateTime dttemp = DateTime.MinValue;
                        string stemp = string.Empty;
                        string sformat = string.Empty;

                        sformat = "yyyy-MM-dd";
                        if (DateTime.TryParseExact(sValoreDato, sformat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dttemp))
                        {
                            stemp = dttemp.ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            stemp = "Mario";
                        }

                        sOutput = stemp;
                        break;

                    case TipoDatoAccessorioEnum.DateTimeBox:
                        DateTime dttemp2 = DateTime.MinValue;
                        string stemp2 = string.Empty;
                        string sformat2 = string.Empty;

                        sformat2 = "yyyy-MM-ddTHH:mm:ss";
                        if (DateTime.TryParseExact(sValoreDato, sformat2, CultureInfo.InvariantCulture, DateTimeStyles.None, out dttemp2))
                        {
                            stemp2 = dttemp2.ToString("dd/MM/yyyy HH:mm");
                        }
                        else
                        {
                            stemp2 = "gianni";
                        }

                        sOutput = stemp2;
                        break;

                    case TipoDatoAccessorioEnum.FloatBox:
                        sOutput = sValoreDato.Replace(".", ",");
                        break;

                    case TipoDatoAccessorioEnum.ComboBox:
                    case TipoDatoAccessorioEnum.ListBox:
                    case TipoDatoAccessorioEnum.ListMultiBox:
                    case TipoDatoAccessorioEnum.NumberBox:
                    case TipoDatoAccessorioEnum.TextBox:
                    case TipoDatoAccessorioEnum.TimeBox:
                        sOutput = sValoreDato;
                        break;
                    default:
                        sOutput = sValoreDato;
                        break;

                }
                return sOutput;
            }

        }

        private string MappaDatiAccessoriIngresso(DatoAccessorioValoreType dato, DatoAccessorioListaType datoType)
        {

            if (dato == null || datoType == null)
            {
                return string.Empty;
            }
            else
            {
                string sOutput = string.Empty;
                string sValoreDato = string.Empty;

                sValoreDato = dato.ValoreDato;

                switch (datoType.DatoAccessorio.Tipo)
                {
                    case TipoDatoAccessorioEnum.DateBox:
                        DateTime dttemp = DateTime.MinValue;
                        string stemp = string.Empty;
                        string sformat = string.Empty;

                        sformat = "yyyy-MM-dd";
                        if (DateTime.TryParseExact(sValoreDato, sformat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dttemp))
                        {
                            stemp = dttemp.ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            stemp = "";
                        }

                        sOutput = stemp;
                        break;

                    case TipoDatoAccessorioEnum.DateTimeBox:
                        DateTime dttemp2 = DateTime.MinValue;
                        string stemp2 = string.Empty;
                        string sformat2 = string.Empty;

                        sformat2 = "yyyy-MM-ddTHH:mm:ss";
                        if (DateTime.TryParseExact(sValoreDato, sformat2, CultureInfo.InvariantCulture, DateTimeStyles.None, out dttemp2))
                        {
                            stemp2 = dttemp2.ToString("dd/MM/yyyy HH:mm");
                        }
                        else
                        {
                            stemp2 = "";
                        }

                        sOutput = stemp2;
                        break;

                    case TipoDatoAccessorioEnum.FloatBox:
                        sOutput = sValoreDato.Replace(".", ",");
                        break;

                    case TipoDatoAccessorioEnum.ComboBox:
                    case TipoDatoAccessorioEnum.ListBox:
                    case TipoDatoAccessorioEnum.ListMultiBox:
                    case TipoDatoAccessorioEnum.NumberBox:
                    case TipoDatoAccessorioEnum.TextBox:
                    case TipoDatoAccessorioEnum.TimeBox:
                        sOutput = sValoreDato;
                        break;
                    default:
                        sOutput = sValoreDato;
                        break;

                }
                return sOutput;
            }

        }

        public List<OEDatiAccessoriDescrittore> GetDatiAccessorNecessari(string s_user, string idOE)
        {
            DatiAccessoriListaType datiAcc;
            List<OEDatiAccessoriDescrittore> listaDati;

            if (!checkConnectionOE())
                return null;
            try
            {
                datiAcc = oe.OttieniDatiAccessoriPerIdRichiesta(creaToken(s_user), idOE);

                if (datiAcc == null)
                    return null;

                listaDati = new List<OEDatiAccessoriDescrittore>();

                foreach (DatoAccessorioListaType dato in datiAcc)
                {
                    listaDati.Add(TranslateDatoAccessorioFromOE(dato));
                }

                if (listaDati.Count > 0)
                {
                    listaDati.Sort(new OEDatiAccessoriComparer());
                }

                return listaDati;
            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);

            }
            return null;
        }

        public List<OEDatoAccessorio> GetDatiAccessoriOld(string s_user, string idOE)
        {

            StatoType ordine;
            OEDatoAccessorio o_nuovodato;
            List<OEDatoAccessorio> datiAccessori;
            Dictionary<string, OEDatoAccessorio> dict_dati;
            string sValoreDato = string.Empty;

            if (!checkConnectionOE())
                return null;
            try
            {
                ordine = oe.OttieniOrdinePerIdRichiesta(creaToken(s_user), idOE);

                if (ordine == null)
                    return null;

                datiAccessori = new List<OEDatoAccessorio>();
                dict_dati = new Dictionary<string, OEDatoAccessorio>();

                foreach (DatoNomeValoreType dato in ordine.Ordine.DatiAggiuntivi)
                {
                    if (dato.DatoAccessorio != null)
                    {
                                                sValoreDato = MappaDatiAccessoriIngresso(dato);
                        o_nuovodato = new OEDatoAccessorio(dato.DatoAccessorio.Codice, sValoreDato, dato.TipoDato, 1);

                        dict_dati.Add(dato.DatoAccessorio.Codice, o_nuovodato);
                        datiAccessori.Add(o_nuovodato);
                    }
                };

                foreach (RigaRichiestaType riga in ordine.Ordine.RigheRichieste)
                {
                    if (riga.DatiAggiuntivi != null)
                        foreach (DatoNomeValoreType dato in riga.DatiAggiuntivi)
                        {
                            if (!dict_dati.ContainsKey(dato.Nome))
                            {
                                                                sValoreDato = MappaDatiAccessoriIngresso(dato);
                                o_nuovodato = new OEDatoAccessorio(dato.Nome, sValoreDato, dato.TipoDato, 1);

                                dict_dati.Add(dato.Nome, o_nuovodato);
                                datiAccessori.Add(o_nuovodato);
                            }
                        }
                }

                return datiAccessori;
            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
            }

            return null;
        }

                                                        public List<OEDatoAccessorio> GetDatiAccessori(string s_user, string idOE)
        {

            DatiAccessoriValoriType dati = null;
            DatiAccessoriListaType datiType = null;

            List<OEDatoAccessorio> result = new List<OEDatoAccessorio>();

            string sValoreDato = string.Empty;

                        bool bConn = checkConnectionOE();

            if (bConn == false)
                return null;


            try
            {
                TokenAccessoType token = creaToken(s_user);
                dati = oe.OttieniDatiAccessoriValoriPerIdRichiesta(token, idOE);
                datiType = oe.OttieniDatiAccessoriPerIdRichiesta(token, idOE);

                if ((dati == null) || (datiType == null))
                    return null;

                                foreach (DatoAccessorioListaType datoType in datiType)
                {
                    List<DatoAccessorioValoreType> datoValori = dati.Where(x => x.Codice == datoType.DatoAccessorio.Codice).ToList();

                    foreach (DatoAccessorioValoreType datoValore in datoValori)
                    {
                        sValoreDato = MappaDatiAccessoriIngresso(datoValore, datoType);

                        int rip = 0;

                        if (datoValore.Ripetizione.HasValue)
                            rip = datoValore.Ripetizione.Value;

                        OEDatoAccessorio oeDato = new OEDatoAccessorio(datoType.DatoAccessorio.Codice, sValoreDato, datoType.DatoAccessorio.Tipo.ToString(), rip);
                        result.Add(oeDato);
                    }

                }

                return result;

            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
            }

            return null;
        }

        public bool FillDatiAccessori(string s_user, string idOE, List<OEDatoAccessorio> datiAggiuntivi)
        {
            DatiAccessoriValoriType o_datiaccessorioe;
            DatoAccessorioValoreType o_datoaccessoriooe;

            if (!checkConnectionOE())
                return false;
            try
            {
                o_datiaccessorioe = new DatiAccessoriValoriType();

                foreach (OEDatoAccessorio o_dato in datiAggiuntivi)
                {
                    o_datoaccessoriooe = new DatoAccessorioValoreType();
                    o_datoaccessoriooe.Codice = o_dato.Codice;
                    o_datoaccessoriooe.Ripetizione = o_dato.Ripetizione;

                    EnumTipoDatoAggiuntivo tipodato = this.TranslateTipoDatoAccessorioToOE(o_dato.Tipo);
                    switch (tipodato)
                    {
                        case EnumTipoDatoAggiuntivo.DateBox:
                            DateTime dttemp = DateTime.MinValue;
                            string stemp = string.Empty;
                            string sformat = string.Empty;
                            sformat = "dd/MM/yyyy";

                                                                                                                                                                                                                                                            if (o_dato.Valore != string.Empty)
                            {
                                try
                                {
                                    dttemp = DateTime.ParseExact(o_dato.Valore, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                    stemp = dttemp.ToString("yyyy-MM-dd");
                                }
                                catch
                                {
                                    stemp = string.Empty;
                                }
                            }
                            o_datoaccessoriooe.ValoreDato = stemp;
                            break;

                        case EnumTipoDatoAggiuntivo.DateTimeBox:
                            DateTime dttemp2 = DateTime.MinValue;
                            string stemp2 = string.Empty;
                            string sformat2 = string.Empty;
                            sformat2 = "dd/MM/yyyy HH:mm";

                                                                                                                                                                                                                                                            if (o_dato.Valore != string.Empty)
                            {
                                try
                                {
                                    dttemp2 = DateTime.ParseExact(o_dato.Valore, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                                    stemp2 = dttemp2.ToString("yyyy-MM-ddTHH:mm:ss");
                                }
                                catch
                                {
                                    stemp2 = string.Empty;
                                }
                            }

                            o_datoaccessoriooe.ValoreDato = stemp2;
                            break;

                        case EnumTipoDatoAggiuntivo.FloatBox:
                            o_datoaccessoriooe.ValoreDato = o_dato.Valore.Replace(",", ".");
                            break;

                        case EnumTipoDatoAggiuntivo.ListMultiBox:

                                                                                                                                                                                                                                                                                                                    

                            if (o_dato.Valore.Length > 2)
                            {
                                if (o_dato.Valore.Substring(o_dato.Valore.Length - 2, 2) == "§;")
                                {
                                    o_datoaccessoriooe.ValoreDato = o_dato.Valore.Substring(0, o_dato.Valore.Length - 2);
                                }
                                else
                                {
                                    o_datoaccessoriooe.ValoreDato = o_dato.Valore;
                                }
                            }
                            else
                            {
                                o_datoaccessoriooe.ValoreDato = o_dato.Valore;
                            }
                            break;

                        case EnumTipoDatoAggiuntivo.TimeBox:
                        case EnumTipoDatoAggiuntivo.ListBox:
                        case EnumTipoDatoAggiuntivo.NumberBox:
                        case EnumTipoDatoAggiuntivo.Tempi:
                        case EnumTipoDatoAggiuntivo.TextBox:
                        case EnumTipoDatoAggiuntivo.ComboBox:
                        case EnumTipoDatoAggiuntivo.Titolo:
                        case EnumTipoDatoAggiuntivo.Undefined:
                        default:
                            o_datoaccessoriooe.ValoreDato = o_dato.Valore;
                            break;
                    }


                    o_datiaccessorioe.Add(o_datoaccessoriooe);
                }

                oe.AggiornaOrdineDatiAccessoriValoriPerIdRichiesta(creaToken(s_user), idOE, o_datiaccessorioe);

                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
            }

            return false;
        }

        public OEOrdineDettaglio FillDatiAccessoriEsteso(string s_user, string idOE, List<OEDatoAccessorio> datiAggiuntivi)
        {
            OEOrdineDettaglio retOrdine = null;

            try
            {
                if (checkConnectionOE())
                {
                    DatiAccessoriValoriType o_datiaccessorioe;
                    DatoAccessorioValoreType o_datoaccessoriooe;

                    o_datiaccessorioe = new DatiAccessoriValoriType();

                    foreach (OEDatoAccessorio o_dato in datiAggiuntivi)
                    {
                        o_datoaccessoriooe = new DatoAccessorioValoreType();
                        o_datoaccessoriooe.Codice = o_dato.Codice;
                        o_datoaccessoriooe.Ripetizione = o_dato.Ripetizione;

                        EnumTipoDatoAggiuntivo tipodato = this.TranslateTipoDatoAccessorioToOE(o_dato.Tipo);
                        switch (tipodato)
                        {
                            case EnumTipoDatoAggiuntivo.DateBox:
                                DateTime dttemp = DateTime.MinValue;
                                string stemp = string.Empty;
                                string sformat = string.Empty;
                                sformat = "dd/MM/yyyy";

                                if (o_dato.Valore != string.Empty)
                                {
                                    try
                                    {
                                        dttemp = DateTime.ParseExact(o_dato.Valore, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                        stemp = dttemp.ToString("yyyy-MM-dd");
                                    }
                                    catch
                                    {
                                        stemp = string.Empty;
                                    }
                                }
                                o_datoaccessoriooe.ValoreDato = stemp;
                                break;

                            case EnumTipoDatoAggiuntivo.DateTimeBox:
                                DateTime dttemp2 = DateTime.MinValue;
                                string stemp2 = string.Empty;
                                string sformat2 = string.Empty;
                                sformat2 = "dd/MM/yyyy HH:mm";

                                if (o_dato.Valore != string.Empty)
                                {
                                    try
                                    {
                                        dttemp2 = DateTime.ParseExact(o_dato.Valore, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                                        stemp2 = dttemp2.ToString("yyyy-MM-ddTHH:mm:ss");
                                    }
                                    catch
                                    {
                                        stemp2 = string.Empty;
                                    }
                                }

                                o_datoaccessoriooe.ValoreDato = stemp2;
                                break;

                            case EnumTipoDatoAggiuntivo.FloatBox:
                                o_datoaccessoriooe.ValoreDato = o_dato.Valore.Replace(",", ".");
                                break;

                            case EnumTipoDatoAggiuntivo.ListMultiBox:

                                                                                                                                                                                                                                                                                                                                                                

                                if (o_dato.Valore.Length > 2)
                                {
                                    if (o_dato.Valore.Substring(o_dato.Valore.Length - 2, 2) == "§;")
                                    {
                                        o_datoaccessoriooe.ValoreDato = o_dato.Valore.Substring(0, o_dato.Valore.Length - 2);
                                    }
                                    else
                                    {
                                        o_datoaccessoriooe.ValoreDato = o_dato.Valore;
                                    }
                                }
                                else
                                {
                                    o_datoaccessoriooe.ValoreDato = o_dato.Valore;
                                }
                                break;

                            case EnumTipoDatoAggiuntivo.TimeBox:
                            case EnumTipoDatoAggiuntivo.ListBox:
                            case EnumTipoDatoAggiuntivo.NumberBox:
                            case EnumTipoDatoAggiuntivo.Tempi:
                            case EnumTipoDatoAggiuntivo.TextBox:
                            case EnumTipoDatoAggiuntivo.ComboBox:
                            case EnumTipoDatoAggiuntivo.Titolo:
                            case EnumTipoDatoAggiuntivo.Undefined:
                            default:
                                o_datoaccessoriooe.ValoreDato = o_dato.Valore;
                                break;
                        }


                        o_datiaccessorioe.Add(o_datoaccessoriooe);
                    }

                    StatoType stato = oe.AggiornaOrdineDatiAccessoriValoriPerIdRichiesta(creaToken(s_user), idOE, o_datiaccessorioe);

                    if (stato != null)
                        retOrdine = TranslateOrdineDettaglioFromOE(stato, TranslateOrdineTestataFromOE(stato));
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
            }
            return retOrdine;
        }

        public List<OEDatoAccessorio> GetDatiAccessoriTestata(string s_user, string idOE)
        {
            StatoType ordine;
            List<OEDatoAccessorio> datiAccessori;

            if (!checkConnectionOE())
                return null;
            try
            {
                ordine = oe.OttieniOrdinePerIdRichiesta(creaToken(s_user), idOE);

                if (ordine == null)
                    return null;

                datiAccessori = new List<OEDatoAccessorio>();
                foreach (DatoNomeValoreType dato in ordine.Ordine.DatiAggiuntivi)
                    datiAccessori.Add(new OEDatoAccessorio(dato.DatoAccessorio.Codice, dato.ValoreDato, dato.TipoDato, 1));

                return datiAccessori;
            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);

            }

            return null;
        }

        public List<OEDatoAccessorio> GetDatiAccessoriPrestazione(string s_user, string idOE, OEPrestazione o_prestazione)
        {
            StatoType ordine;
            List<OEDatoAccessorio> datiAccessori;

            if (!checkConnectionOE())
                return null;
            try
            {
                ordine = oe.OttieniOrdinePerIdRichiesta(creaToken(s_user), idOE);

                if (ordine == null)

                    return null;

                datiAccessori = new List<OEDatoAccessorio>();

                foreach (RigaRichiestaType riga in ordine.Ordine.RigheRichieste)
                {
                    if (
                        riga.SistemaErogante.Azienda.Codice == o_prestazione.Erogante.CodiceAzienda &&
                        riga.SistemaErogante.Sistema.Codice == o_prestazione.Erogante.Codice &&
                        riga.Prestazione.Codice == o_prestazione.Codice
                        )
                        foreach (DatoNomeValoreType dato in riga.DatiAggiuntivi)
                            datiAccessori.Add(new OEDatoAccessorio(dato.Nome, dato.ValoreDato, dato.TipoDato, 1));
                }

                return datiAccessori;
            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);

            }
            return null;
        }

        public List<OEDatoAccessorio> GetDatiAccessoriErogante2(string s_user, string idOE)
        {
            StatoType ordine;
            List<OEDatoAccessorio> datiAccessori;
            OEDatoAccessorio o_nuovodato;
            Dictionary<string, OEDatoAccessorio> dict_dati;

            if (!checkConnectionOE())
                return null;

            try
            {
                ordine = oe.OttieniOrdinePerIdRichiesta(creaToken(s_user), idOE);

                if (ordine == null)
                    return null;

                datiAccessori = new List<OEDatoAccessorio>();
                dict_dati = new Dictionary<string, OEDatoAccessorio>();

                if (ordine.Erogati != null)
                    foreach (TestataErogatoType dato in ordine.Erogati)
                    {
                        if (dato.DatiPersistenti != null)
                            foreach (DatoNomeValoreType datoaggiuntivo in dato.DatiPersistenti)
                            {
                                o_nuovodato = new OEDatoAccessorio(datoaggiuntivo.Nome, datoaggiuntivo.ValoreDato, datoaggiuntivo.TipoDato, 1);
                                datiAccessori.Add(o_nuovodato);
                                if (!dict_dati.ContainsKey(datoaggiuntivo.Nome))
                                    dict_dati.Add(datoaggiuntivo.Nome, o_nuovodato);
                            }

                        if (dato.DatiAggiuntivi != null)
                            foreach (DatoNomeValoreType datoaggiuntivo in dato.DatiAggiuntivi)
                            {
                                o_nuovodato = new OEDatoAccessorio(datoaggiuntivo.Nome, datoaggiuntivo.ValoreDato, datoaggiuntivo.TipoDato, 1);
                                datiAccessori.Add(o_nuovodato);
                                if (!dict_dati.ContainsKey(datoaggiuntivo.Nome))
                                    dict_dati.Add(datoaggiuntivo.Nome, o_nuovodato);
                            };
                    };

                if (ordine.Erogati != null)
                    foreach (TestataErogatoType dato in ordine.Erogati)
                        foreach (RigaErogataType riga in dato.RigheErogate)
                            if (riga.DatiAggiuntivi != null)
                            {
                                foreach (DatoNomeValoreType datoaggiuntivo in riga.DatiAggiuntivi)
                                    if (!dict_dati.ContainsKey(datoaggiuntivo.Nome))
                                    {
                                        o_nuovodato = new OEDatoAccessorio(datoaggiuntivo.Nome, datoaggiuntivo.ValoreDato, datoaggiuntivo.TipoDato, 1);
                                        datiAccessori.Add(o_nuovodato);
                                        if (!dict_dati.ContainsKey(datoaggiuntivo.Nome))
                                            dict_dati.Add(datoaggiuntivo.Nome, o_nuovodato);
                                    }
                            };

                return datiAccessori;
            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);

            }
            return null;

        }

        public List<OEDatoAccessorio> GetDatiAccessoriErogante(string s_user, string idOE, OESistemaErogante o_erogante)
        {
            return GetDatiAccessoriErogante2(s_user, idOE);
        }

        public List<OEDatoAccessorio> GetDatiAccessoriEroganteTestata(string s_user, string idOE, OESistemaErogante o_erogante)
        {
            StatoType ordine;
            List<OEDatoAccessorio> datiAccessori;

            if (!checkConnectionOE())
                return null;
            try
            {
                ordine = oe.OttieniOrdinePerIdRichiesta(creaToken(s_user), idOE);

                if (ordine == null)
                    return null;

                datiAccessori = new List<OEDatoAccessorio>();

                if (ordine.Erogati != null)
                    foreach (TestataErogatoType dato in ordine.Erogati)
                        if (
                            dato.SistemaErogante.Sistema.Codice == o_erogante.Codice &&
                            dato.SistemaErogante.Azienda.Codice == o_erogante.CodiceAzienda
                            )
                            foreach (DatoNomeValoreType datoaggiuntivo in dato.DatiAggiuntivi)
                                datiAccessori.Add(new OEDatoAccessorio(datoaggiuntivo.Nome, datoaggiuntivo.ValoreDato, datoaggiuntivo.TipoDato, 1));

                return datiAccessori;
            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
            }

            return null;

        }

        public List<OEDatoAccessorio> GetDatiAccessoriErogantePrestazione(string s_user, string idOE, OEPrestazione o_prestazione)
        {
            StatoType ordine;
            List<OEDatoAccessorio> datiAccessori;

            if (!checkConnectionOE())
                return null;

            try
            {
                ordine = oe.OttieniOrdinePerIdRichiesta(creaToken(s_user), idOE);

                if (ordine == null)
                    return null;

                datiAccessori = new List<OEDatoAccessorio>();

                if (ordine.Erogati != null)
                    foreach (TestataErogatoType dato in ordine.Erogati)
                    {
                        if (
                            dato.SistemaErogante.Sistema.Codice == o_prestazione.Erogante.Codice &&
                            dato.SistemaErogante.Azienda.Codice == o_prestazione.Erogante.CodiceAzienda
                            )
                            foreach (RigaErogataType rigaerogata in dato.RigheErogate)
                            {
                                if (rigaerogata.Prestazione.Codice == o_prestazione.Codice)
                                    foreach (DatoNomeValoreType d in rigaerogata.DatiAggiuntivi)
                                        datiAccessori.Add(new OEDatoAccessorio(d.Nome, d.ValoreDato, d.TipoDato, 1));
                            }
                    }


                return datiAccessori;
            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);

            }
            return null;

        }

        #endregion

        #region DATAORAPROGRAMMATA

        public DateTime? GetDataOraProgrammata(string s_user, string s_IdOE)
        {
            StatoType ordine;

            try
            {
                ordine = oe.OttieniOrdinePerIdRichiesta(creaToken(s_user), s_IdOE);

                return ordine.Ordine.DataPrenotazione;
            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
            };

            return null;
        }

        public bool SetDataOraProgrammata(string s_user, string s_IdOE, DateTime? dt_dataoraprogrammata)
        {
            StatoType ordine;

            try
            {
                ordine = oe.OttieniOrdinePerIdRichiesta(creaToken(s_user), s_IdOE);

                ordine.Ordine.DataPrenotazione = dt_dataoraprogrammata;

                oe.AggiungiOppureModificaOrdine(creaToken(s_user), ordine.Ordine);

                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
            };

            return false;
        }

        #endregion

        #region MANAGEMENT

        private TokenAccessoType creaToken(string s_user)
        {
                        OperationContext context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;

            return getTokenFromDB(s_user, endpoint.Address);
        }

        private TokenAccessoType getTokenFromDB(string s_user, string s_ipaddress)
        {
            TokenAccessoType oetoken = null;
            SqlParameterExt[] spcoll = new SqlParameterExt[2];
            DataTable dt = null;
            string tokenxml = string.Empty;

            spcoll[0] = new SqlParameterExt("CodUtente", s_user, ParameterDirection.Input, SqlDbType.VarChar);
            spcoll[1] = new SqlParameterExt("IndirizzoIP", s_ipaddress, ParameterDirection.Input, SqlDbType.VarChar);

            try
            {
                dt = Database.GetDataTableStoredProc("MSP_OE_SelMovToken", spcoll);

                if (dt != null && dt.Rows.Count > 0)
                {

                    tokenxml = dt.Rows[0]["Token"].ToString();

                    oetoken = XmlProcs.XmlDeserializeFromString<TokenAccessoType>(tokenxml);

                }
                else
                {
                    oetoken = oe.CreaTokenAccessoDelega(s_user, "ASMN", "SCCI");
                    this.setTokenToDB(s_user, s_ipaddress, oetoken);
                }

            }
            catch
            {
                oetoken = oe.CreaTokenAccessoDelega(s_user, "ASMN", "SCCI");
                this.setTokenToDB(s_user, s_ipaddress, oetoken);
            }
            finally
            {
                if (dt != null)
                {
                    dt.Dispose();
                    dt = null;
                }
            }

            return oetoken;

        }

        private void setTokenToDB(string s_user, string s_ipaddress, TokenAccessoType token)
        {

            try
            {
                SqlParameterExt[] spcoll = new SqlParameterExt[4];
                spcoll[0] = new SqlParameterExt("CodUtente", s_user, ParameterDirection.Input, SqlDbType.VarChar);
                spcoll[1] = new SqlParameterExt("IndirizzoIP", s_ipaddress, ParameterDirection.Input, SqlDbType.VarChar);
                spcoll[2] = new SqlParameterExt("DataScadenza", token.DataScadenza, ParameterDirection.Input, SqlDbType.DateTime);
                spcoll[3] = new SqlParameterExt("Token", XmlProcs.XmlSerializeToString(token), ParameterDirection.Input, SqlDbType.Xml);

                Database.ExecStoredProc("MSP_OE_InsMovToken", spcoll);

            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
            }

        }

        private bool checkConnectionOE()
        {


            if (oe == null || oe.State != CommunicationState.Created)
                connectToOE();
            if (oe.State != CommunicationState.Created)
                return false;

            return true;

        }

        private void connectToOE()
        {

            UnicodeSrl.Scci.Statics.Database.ConnectionString = Common.ConnString;

            CredencialsWCF cred = new CredencialsWCF();

            try
            {

                cred = Credentials.CredenzialiScciWCF(Credentials.enumTipoCredenziali.OE);

                Uri serviceUri = new Uri(cred.Url);
                EndpointAddress endpointAddress = new EndpointAddress(serviceUri);

                                System.ServiceModel.BasicHttpBinding binding = Credentials.CreateBindingInstance(Credentials.enumTipoCredenziali.OE);

                                oe = new OrderEntryV1Client(binding, endpointAddress);

                oe.ClientCredentials.Windows.ClientCredential.Domain = cred.Domain;
                oe.ClientCredentials.Windows.ClientCredential.UserName = cred.UserName;
                oe.ClientCredentials.Windows.ClientCredential.Password = cred.Password;

            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
            }
            finally
            {
                cred = null;
            }

        }

        private string getIdGuidFromIdOE(string s_user, string idOE)
        {

            StatoType ordine;

            if (!checkConnectionOE())
                return null;

            ordine = oe.OttieniOrdinePerIdRichiesta(creaToken(s_user), idOE);
            return ordine.Ordine.IdGuidOrderEntry;


        }

        internal static BasicHttpBinding CreateBindingInstance(Credentials.enumTipoCredenziali tipocredenziali)
        {
            BasicHttpBinding binding = new BasicHttpBinding();

            switch (tipocredenziali)
            {
                case Credentials.enumTipoCredenziali.DWH:
                    break;
                case Credentials.enumTipoCredenziali.OE:
                    binding.Name = "BasicHttpBinding_IOrderEntry";
                    break;
                case Credentials.enumTipoCredenziali.SAC:
                    break;
            }

            binding.MessageEncoding = WSMessageEncoding.Text;
            binding.Security.Mode = BasicHttpSecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;

            binding.CloseTimeout = new TimeSpan(10, 0, 0);
            binding.OpenTimeout = new TimeSpan(10, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(10, 0, 0);
            binding.SendTimeout = new TimeSpan(10, 0, 0);

            binding.ReaderQuotas.MaxDepth = 2147483647;
            binding.ReaderQuotas.MaxStringContentLength = 2147483647;
            binding.ReaderQuotas.MaxArrayLength = 2147483647;
            binding.ReaderQuotas.MaxBytesPerRead = 2147483647;
            binding.ReaderQuotas.MaxNameTableCharCount = 2147483647;
            binding.MaxReceivedMessageSize = 2147483647;
            binding.MaxBufferSize = 2147483647;
            binding.MaxBufferPoolSize = 2147483647;

            binding.UseDefaultWebProxy = true;
            return binding;
        }

        #endregion

        #region TRANSLATION

        #region TRANSLATION - ENUMS

        private OESistemaErogante TranslateSistemaFromOE(SistemaType o_sistema)
        {
            return new OESistemaErogante(o_sistema.Sistema.Codice, o_sistema.Sistema.Descrizione, o_sistema.Azienda.Codice, o_sistema.Azienda.Descrizione);
        }

        private SistemaType TranslateSistemaToOE(OESistemaErogante o_sistema)
        {
            SistemaType sistema;
            sistema = new SistemaType();
            sistema.Azienda = new CodiceDescrizioneType();
            sistema.Sistema = new CodiceDescrizioneType();
            sistema.Azienda.Codice = o_sistema.CodiceAzienda;
            sistema.Azienda.Descrizione = o_sistema.DescrizioneAzienda;
            sistema.Sistema.Codice = o_sistema.Codice;
            sistema.Sistema.Descrizione = o_sistema.Descrizione;

            return sistema;
        }

        public OEPrioritaOrdine TranslatePrioritaFromOE(PrioritaType o_t)
        {
            if (o_t == null)
                return OEPrioritaOrdine.NN;

            return TranslatePrioritaFromOE((PrioritaEnum)Enum.Parse(typeof(PrioritaEnum), o_t.Codice));
        }

        private OEPrioritaOrdine TranslatePrioritaFromOE(PrioritaEnum e_t)
        {
            switch (e_t)
            {
                case PrioritaEnum.O:
                    return OEPrioritaOrdine.O;
                case PrioritaEnum.P:
                    return OEPrioritaOrdine.P;
                case PrioritaEnum.U:
                    return OEPrioritaOrdine.U;
                case PrioritaEnum.U2:
                    return OEPrioritaOrdine.U2;
                case PrioritaEnum.UD:
                    return OEPrioritaOrdine.UD;
            }

            return OEPrioritaOrdine.NN;
        }

        public string TranslatePrioritaDescFromOE(PrioritaType o_t)
        {
            if (o_t == null)
                return CommonConstants.C_DESC_PRI_NONDEFINITA;
            else
                return TranslatePrioritaDescFromOE((PrioritaEnum)Enum.Parse(typeof(PrioritaEnum), o_t.Codice));
        }

        public string TranslatePrioritaDescFromOE(PrioritaEnum o_t)
        {
            string sRet = string.Empty;

            switch (o_t)
            {
                case PrioritaEnum.O:
                    sRet = CommonConstants.C_DESC_PRI_ORDINARIA;
                    break;

                case PrioritaEnum.P:
                    sRet = CommonConstants.C_DESC_PRI_PROGRAMMATA;
                    break;

                case PrioritaEnum.U:
                    sRet = CommonConstants.C_DESC_PRI_URGENTE;
                    break;

                case PrioritaEnum.U2:
                    sRet = CommonConstants.C_DESC_PRI_CRITICA;
                    break;

                case PrioritaEnum.UD:
                    sRet = CommonConstants.C_DESC_PRI_URGENTEDIFFERIBILE;
                    break;

                default:
                    sRet = CommonConstants.C_DESC_PRI_NONDEFINITA;
                    break;
            }

            return sRet;
        }

        public string TranslatePrioritaDescFromOE(OEPrioritaOrdine oepri)
        {
            return TranslatePrioritaDescFromOE(TranslatePrioritaToOE(oepri));
        }

        private PrioritaEnum TranslatePrioritaToOE(OEPrioritaOrdine o_p)
        {
            switch (o_p)
            {
                case OEPrioritaOrdine.O:
                    return PrioritaEnum.O;
                case OEPrioritaOrdine.P:
                    return PrioritaEnum.P;
                case OEPrioritaOrdine.U:
                    return PrioritaEnum.U;
                case OEPrioritaOrdine.U2:
                    return PrioritaEnum.U2;
                case OEPrioritaOrdine.UD:
                    return PrioritaEnum.UD;
            }

            return PrioritaEnum.O;
        }

        private string TranslateRegimeFromOE(string e_r)
        {
            
            switch ((RegimeEnum)Enum.Parse(typeof(RegimeEnum), e_r))
            {
                case RegimeEnum.AMB:
                    return "AMB";
                case RegimeEnum.DH:
                    return "DH";
                case RegimeEnum.DSA:
                    return "DS";
                case RegimeEnum.LP:
                    return "AMB";                 case RegimeEnum.PS:
                    return "PS";
                case RegimeEnum.RO:
                    return "RO";
            }

            return "NN";
        }

        private RegimeEnum TranslateRegimeToOE(string s_regime)
        {
            switch (s_regime)
            {
                case "AMB":
                    return RegimeEnum.AMB;
                case "DH":
                    return RegimeEnum.DH;
                case "DS":
                    return RegimeEnum.DSA;
                case "OB":                  case "PS":
                    return RegimeEnum.PS;
                case "RO":
                    return RegimeEnum.RO;

            }

                        return RegimeEnum.AMB;
        }

        private OEPrestazioneTipo TranslatePrestazioneTipoFromOE(TipoPrestazioneErogabileEnum e_tipo)
        {
            switch (e_tipo)
            {
                case TipoPrestazioneErogabileEnum.Prestazione:
                    return OEPrestazioneTipo.Prestazione;
                case TipoPrestazioneErogabileEnum.ProfiloBlindato:
                    return OEPrestazioneTipo.Profilo;
                case TipoPrestazioneErogabileEnum.ProfiloScomponibile:
                    return OEPrestazioneTipo.ProfiloScomponibile;
                case TipoPrestazioneErogabileEnum.ProfiloUtente:
                    return OEPrestazioneTipo.ProfiloUtente;
            }

            return OEPrestazioneTipo.NN;
        }

        public TipoPrestazioneErogabileEnum TranslatePrestazioneTipoToOE(OEPrestazioneTipo e_tipo)
        {
            switch (e_tipo)
            {
                case OEPrestazioneTipo.Prestazione:
                    return TipoPrestazioneErogabileEnum.Prestazione;
                case OEPrestazioneTipo.Profilo:
                    return TipoPrestazioneErogabileEnum.ProfiloBlindato;
                case OEPrestazioneTipo.ProfiloScomponibile:
                    return TipoPrestazioneErogabileEnum.ProfiloScomponibile;
                case OEPrestazioneTipo.ProfiloUtente:
                    return TipoPrestazioneErogabileEnum.ProfiloUtente;
            }

            return TipoPrestazioneErogabileEnum.Prestazione;
        }

        private OEValiditaOrdine TranslateStatoValidazioneOrdine(StatoValidazioneEnum statoValidazioneEnum)
        {
            switch (statoValidazioneEnum)
            {
                case StatoValidazioneEnum.AA:
                    return OEValiditaOrdine.Valido;
                case StatoValidazioneEnum.AE:
                    return OEValiditaOrdine.NonValido;
                case StatoValidazioneEnum.AR:
                    return OEValiditaOrdine.NonValido;
            }

            return OEValiditaOrdine.NN;
        }

        private OEStato TranslateStatoOrdine(StatoDescrizioneEnum statoDescrizioneEnum)
        {
            switch (statoDescrizioneEnum)
            {
                case StatoDescrizioneEnum.Accettato:
                    return OEStato.Accettato;
                case StatoDescrizioneEnum.Annullato:
                    return OEStato.Annullato;
                case StatoDescrizioneEnum.Cancellato:
                    return OEStato.Cancellato;
                case StatoDescrizioneEnum.Erogato:
                    return OEStato.Erogato;
                case StatoDescrizioneEnum.Errato:
                    return OEStato.Errato;
                case StatoDescrizioneEnum.Incarico:
                    return OEStato.InCarico;
                case StatoDescrizioneEnum.Inoltrato:
                    return OEStato.Inoltrato;
                case StatoDescrizioneEnum.Inserito:
                    return OEStato.Inserito;
                case StatoDescrizioneEnum.Modificato:
                    return OEStato.Inserito;
                case StatoDescrizioneEnum.Programmato:
                    return OEStato.Programmato;
                case StatoDescrizioneEnum.Rifiutato:
                    return OEStato.Errato;
            }

            return OEStato.NN;
        }

        private OEStatoRichiedente TranslateStatoRichiedente(OperazioneRigaRichiestaOrderEntryEnum statoRichiedente)
        {
            switch (statoRichiedente)
            {
                case OperazioneRigaRichiestaOrderEntryEnum.CA:
                    return OEStatoRichiedente.Cancellata;
                case OperazioneRigaRichiestaOrderEntryEnum.IS:
                    return OEStatoRichiedente.Inserita;
                case OperazioneRigaRichiestaOrderEntryEnum.MD:
                    return OEStatoRichiedente.Modificata;
            }

            return OEStatoRichiedente.NN;
        }

        private OEStatoErogante TranslateStatoErogante(StatoRigaErogataOrderEntryEnum statoErogante)
        {
            switch (statoErogante)
            {
                case StatoRigaErogataOrderEntryEnum.CA:
                    return OEStatoErogante.Cancellata;
                case StatoRigaErogataOrderEntryEnum.CM:
                    return OEStatoErogante.Erogata;
                case StatoRigaErogataOrderEntryEnum.IC:
                    return OEStatoErogante.InCorso;
                case StatoRigaErogataOrderEntryEnum.IP:
                    return OEStatoErogante.Programmata;
            }

            return OEStatoErogante.NN;
        }

        public string TranslateTipoDatoAccessorioFromOE(TipoDatoAccessorioEnum o_tipoDato)
        {
            switch (o_tipoDato)
            {
                case TipoDatoAccessorioEnum.ComboBox:
                    return "ComboBox";
                case TipoDatoAccessorioEnum.DateBox:
                    return "DateBox";
                case TipoDatoAccessorioEnum.DateTimeBox:
                    return "DateTimeBox";
                case TipoDatoAccessorioEnum.FloatBox:
                    return "FloatBox";
                case TipoDatoAccessorioEnum.ListBox:
                    return "ListBox";
                case TipoDatoAccessorioEnum.ListMultiBox:
                    return "ListMultiBox";
                case TipoDatoAccessorioEnum.NumberBox:
                    return "NumberBox";
                case TipoDatoAccessorioEnum.TextBox:
                    return "TextBox";
                case TipoDatoAccessorioEnum.TimeBox:
                    return "TimeBox";
            }

            return "";
        }

        private EnumTipoDatoAggiuntivo TranslateTipoDatoAccessorioToOE(string tipodatoOE)
        {
            EnumTipoDatoAggiuntivo ret = EnumTipoDatoAggiuntivo.Undefined;

            switch (tipodatoOE.ToUpper())
            {
                case "COMBOBOX":
                    ret = EnumTipoDatoAggiuntivo.ComboBox;
                    break;
                case "DATEBOX":
                    ret = EnumTipoDatoAggiuntivo.DateBox;
                    break;
                case "DATETIMEBOX":
                    ret = EnumTipoDatoAggiuntivo.DateTimeBox;
                    break;
                case "FLOATBOX":
                    ret = EnumTipoDatoAggiuntivo.FloatBox;
                    break;
                case "LISTBOX":
                    ret = EnumTipoDatoAggiuntivo.ListBox;
                    break;
                case "LISTMULTIBOX":
                    ret = EnumTipoDatoAggiuntivo.ListMultiBox;
                    break;
                case "NUMBERBOX":
                    ret = EnumTipoDatoAggiuntivo.NumberBox;
                    break;
                case "TEXTBOX":
                    ret = EnumTipoDatoAggiuntivo.TextBox;
                    break;
                case "TIMEBOX":
                    ret = EnumTipoDatoAggiuntivo.TimeBox;
                    break;
                case "TITOLO":
                    ret = EnumTipoDatoAggiuntivo.Titolo;
                    break;
                default:
                    ret = EnumTipoDatoAggiuntivo.Undefined;
                    break;
            }

            return ret;
        }

        #endregion

        private OEPrestazione TranslatePrestazioneErogabileFromOE(PrestazioneListaType o_prestazione)
        {
            return new OEPrestazione(
                        o_prestazione.Codice,
                        o_prestazione.Descrizione,
                        TranslatePrestazioneTipoFromOE(o_prestazione.Tipo),
                        new OESistemaErogante(
                            o_prestazione.SistemaErogante.Sistema.Codice,
                            o_prestazione.SistemaErogante.Sistema.Descrizione,
                            o_prestazione.SistemaErogante.Azienda.Codice,
                            o_prestazione.SistemaErogante.Azienda.Descrizione
                            )
                            );
        }

        private OEPrestazione TranslatePrestazioneErogabileFromOE(PrestazioneErogabileType o_prestazione)
        {
            return new OEPrestazione(
                        o_prestazione.Codice,
                        o_prestazione.Descrizione,
                        TranslatePrestazioneTipoFromOE(o_prestazione.Tipo),
                        new OESistemaErogante(
                            o_prestazione.SistemaErogante.Sistema.Codice,
                            o_prestazione.SistemaErogante.Sistema.Descrizione,
                            o_prestazione.SistemaErogante.Azienda.Codice,
                            o_prestazione.SistemaErogante.Azienda.Descrizione
                            )
                            );
        }

        private OEOrdineTestata TranslateOrdineTestataFromOE(StatoType o_ordineoe)
        {
            OEOrdineTestata ordinetestata;

            if (o_ordineoe == null)

                return null;

            ordinetestata = new OEOrdineTestata();

            ordinetestata.DataOrdine = (DateTime)(o_ordineoe.Ordine.Data);

            if (o_ordineoe.Ordine.Operatore != null)
                ordinetestata.Operatore = o_ordineoe.Ordine.Operatore.Cognome + " " + o_ordineoe.Ordine.Operatore.Nome + " (" + o_ordineoe.Ordine.Operatore.CodiceFiscale + ")";

            ordinetestata.NumeroOrdine = o_ordineoe.Ordine.IdRichiestaOrderEntry;
            ordinetestata.IdOrdine = o_ordineoe.Ordine.IdRichiestaRichiedente;

            ordinetestata.DescrizioneSistemaRichiedente = o_ordineoe.Ordine.SistemaRichiedente.Azienda.Descrizione + " - " + o_ordineoe.Ordine.SistemaRichiedente.Sistema.Descrizione;

            ordinetestata.Nosologico = o_ordineoe.Ordine.NumeroNosologico;

            ordinetestata.Priorita = TranslatePrioritaFromOE(o_ordineoe.Ordine.Priorita);

            ordinetestata.Regime = TranslateRegimeFromOE(o_ordineoe.Ordine.Regime.Codice);

                        ordinetestata.DataOraProgrammata = GrabDataOraProgrammata(o_ordineoe);

            if (o_ordineoe.Erogati != null)
                foreach (TestataErogatoType o_testata in o_ordineoe.Erogati)
                {
                    if ((o_testata.DataPrenotazione != null) && (o_testata.DataPrenotazione != DateTime.MinValue) && ((o_testata.DataPrenotazione < ordinetestata.DataOraProgrammata) || (ordinetestata.DataOraProgrammata == DateTime.MinValue)))
                        ordinetestata.DataOraProgrammata = (DateTime)o_testata.DataPrenotazione;
                };

            if (ordinetestata.DataOraProgrammata == DateTime.MinValue)
            {
                if (o_ordineoe.Ordine.DataPrenotazione != null)
                    ordinetestata.DataOraProgrammata = (DateTime)o_ordineoe.Ordine.DataPrenotazione;
                else
                    ordinetestata.DataOraProgrammata = DateTime.MinValue;
            };

            ordinetestata.PazienteId = o_ordineoe.Ordine.Paziente.IdSac;

            ordinetestata.PazienteNome = o_ordineoe.Ordine.Paziente.Nome;
            ordinetestata.PazienteCognome = o_ordineoe.Ordine.Paziente.Cognome;

            if (o_ordineoe.Ordine.Paziente.DataNascita != null)
                ordinetestata.PazienteDataNascita = (DateTime)(o_ordineoe.Ordine.Paziente.DataNascita);
            else
                ordinetestata.PazienteDataNascita = DateTime.MinValue;

            ordinetestata.PazienteCF = o_ordineoe.Ordine.Paziente.CodiceFiscale;

            
            if (o_ordineoe.Ordine.UnitaOperativaRichiedente.UnitaOperativa.Codice.StartsWith("MAT"))
                o_ordineoe.Ordine.UnitaOperativaRichiedente.UnitaOperativa.Codice = o_ordineoe.Ordine.UnitaOperativaRichiedente.UnitaOperativa.Codice.Remove(0, 3);

            ordinetestata.UnitaOperativaCodice = o_ordineoe.Ordine.UnitaOperativaRichiedente.UnitaOperativa.Codice;

            ordinetestata.UnitaOperativaDescrizione = o_ordineoe.Ordine.UnitaOperativaRichiedente.UnitaOperativa.Descrizione;
            ordinetestata.UnitaOperativaAziendaCodice = o_ordineoe.Ordine.UnitaOperativaRichiedente.Azienda.Codice;
            ordinetestata.UnitaOperativaAziendaDescrizione = o_ordineoe.Ordine.UnitaOperativaRichiedente.Azienda.Descrizione;

            ordinetestata.StatoValidazione = TranslateStatoValidazioneOrdine(o_ordineoe.StatoValidazione.Stato);
            ordinetestata.DescrizioneStatoValidazione = o_ordineoe.StatoValidazione.Descrizione;

            ordinetestata.Stato = TranslateStatoOrdine(o_ordineoe.DescrizioneStato);

            ordinetestata.Eroganti = TranslateElencoEroganti(o_ordineoe);
            ordinetestata.ErogantiLista = TranslateElencoErogantiLista(o_ordineoe);



                                                                                                                                                                                                                                    
            ordinetestata.NumeroPrestazioni = o_ordineoe.Ordine.RigheRichieste.Count;

            if (o_ordineoe.Ordine.Operatore != null)
                ordinetestata.Operatore = o_ordineoe.Ordine.Operatore.Cognome + " " + o_ordineoe.Ordine.Operatore.Nome;

                        if (o_ordineoe.Ordine.Cancellabile != null)
            {
                ordinetestata.Cancellabile = (bool)o_ordineoe.Ordine.Cancellabile;
            }
            else
            {
                ordinetestata.Cancellabile = false;
            }


            return ordinetestata;
        }

        private OEOrdineTestata TranslateOrdineTestataFromOE(OrdineListaType o_ordineoe)
        {
            OEOrdineTestata ordinetestata;

            if (o_ordineoe == null)

                return null;

            ordinetestata = new OEOrdineTestata();

            ordinetestata.DataOrdine = (DateTime)(o_ordineoe.Data);
            ordinetestata.DataModifica = (DateTime)(o_ordineoe.DataModifica);

            if (o_ordineoe.Operatore != null)
                ordinetestata.Operatore = o_ordineoe.Operatore.Cognome + " " + o_ordineoe.Operatore.Nome + " (" + o_ordineoe.Operatore.CodiceFiscale + ")";
            ordinetestata.NumeroOrdine = o_ordineoe.IdRichiestaOrderEntry;
            ordinetestata.IdOrdine = o_ordineoe.IdRichiestaRichiedente;

            ordinetestata.DescrizioneSistemaRichiedente = o_ordineoe.SistemaRichiedente.Azienda.Codice + " - " + o_ordineoe.SistemaRichiedente.Sistema.Descrizione;

            ordinetestata.Nosologico = o_ordineoe.NumeroNosologico;

            ordinetestata.Priorita = TranslatePrioritaFromOE(o_ordineoe.Priorita);
            ordinetestata.PrioritaDesc = TranslatePrioritaDescFromOE(o_ordineoe.Priorita);

            ordinetestata.Regime = TranslateRegimeFromOE(o_ordineoe.Regime.Codice);

            if (o_ordineoe.DataPrenotazione != null)
                ordinetestata.DataOraProgrammata = (DateTime)o_ordineoe.DataPrenotazione;
            else
                ordinetestata.DataOraProgrammata = DateTime.MinValue;

            if (o_ordineoe.DataProgrammataCalcolata != null)
                ordinetestata.DataOraProgrammataCalcolata = (DateTime)o_ordineoe.DataProgrammataCalcolata;
            else
                ordinetestata.DataOraProgrammataCalcolata = DateTime.MinValue;

            if (o_ordineoe.DataPrenotazioneRichiesta != null)
                ordinetestata.DataOraPreferita = (DateTime)o_ordineoe.DataPrenotazioneRichiesta;
            else
                ordinetestata.DataOraPreferita = DateTime.MinValue;

            ordinetestata.PazienteId = o_ordineoe.Paziente.IdSac;

            ordinetestata.PazienteNome = o_ordineoe.Paziente.Nome;
            ordinetestata.PazienteCognome = o_ordineoe.Paziente.Cognome;

            if (o_ordineoe.Paziente.DataNascita != null)
                ordinetestata.PazienteDataNascita = (DateTime)(o_ordineoe.Paziente.DataNascita);
            else
                ordinetestata.PazienteDataNascita = DateTime.MinValue;

            ordinetestata.PazienteCF = o_ordineoe.Paziente.CodiceFiscale;
            ordinetestata.PazienteSesso = o_ordineoe.Paziente.Sesso;

            
            if (o_ordineoe.UnitaOperativaRichiedente.UnitaOperativa.Codice.StartsWith("MAT"))
                o_ordineoe.UnitaOperativaRichiedente.UnitaOperativa.Codice = o_ordineoe.UnitaOperativaRichiedente.UnitaOperativa.Codice.Remove(0, 3);

            ordinetestata.UnitaOperativaCodice = o_ordineoe.UnitaOperativaRichiedente.UnitaOperativa.Codice;
            ordinetestata.UnitaOperativaDescrizione = o_ordineoe.UnitaOperativaRichiedente.UnitaOperativa.Descrizione;
            ordinetestata.UnitaOperativaAziendaCodice = o_ordineoe.UnitaOperativaRichiedente.Azienda.Codice;
            ordinetestata.UnitaOperativaAziendaDescrizione = o_ordineoe.UnitaOperativaRichiedente.Azienda.Descrizione;

            ordinetestata.StatoValidazione = TranslateStatoValidazioneOrdine(o_ordineoe.StatoValidazione.Stato);
            ordinetestata.DescrizioneStatoValidazione = o_ordineoe.StatoValidazione.Descrizione;

            ordinetestata.Stato = TranslateStatoOrdine(o_ordineoe.DescrizioneStato);

            ordinetestata.Eroganti = o_ordineoe.SistemiEroganti;


                                                                                                                        
                                                                                                                                    
            ordinetestata.NumeroPrestazioni = o_ordineoe.NumeroRighe;
            ordinetestata.AnteprimaPrestazioni = o_ordineoe.AnteprimaPrestazioni;
            if (o_ordineoe.Operatore != null)
                ordinetestata.Operatore = o_ordineoe.Operatore.Cognome + " " + o_ordineoe.Operatore.Nome;

            ordinetestata.Cancellabile = o_ordineoe.Cancellabile;

            return ordinetestata;
        }

        private OEOrdineDettaglio TranslateOrdineDettaglioFromOE(StatoType o_ordineoe)
        {
            OEOrdineTestata o_testata;

            if (o_ordineoe == null)
                return null;

            o_testata = TranslateOrdineTestataFromOE(o_ordineoe);

            return TranslateOrdineDettaglioFromOE(o_ordineoe, o_testata);
        }

        private OEOrdineDettaglio TranslateOrdineDettaglioFromOE(StatoType o_ordineoe, OEOrdineTestata o_testata)
        {
            OEOrdineDettaglio o_ordinedettaglio;
            OEOrdinePrestazione o_nuovaprestazione;

            if (o_ordineoe == null)
                return new OEOrdineDettaglio();

            o_ordinedettaglio = new OEOrdineDettaglio();

            o_ordinedettaglio.OrdineTestata = o_testata;

            o_ordinedettaglio.Prestazioni.Clear();

            foreach (RigaRichiestaType o_prestazionerichiesta in o_ordineoe.Ordine.RigheRichieste)
            {
                                o_nuovaprestazione = o_ordinedettaglio.GetPrestazione(
                    o_prestazionerichiesta.SistemaErogante.Azienda.Codice,
                    o_prestazionerichiesta.SistemaErogante.Sistema.Codice,
                    o_prestazionerichiesta.Prestazione.Codice);
                if (o_nuovaprestazione == null)
                {
                    o_nuovaprestazione = new OEOrdinePrestazione();
                    o_ordinedettaglio.Prestazioni.Add(o_nuovaprestazione);
                };

                TranslatePrestazioneRichestaFromOE(o_nuovaprestazione, o_prestazionerichiesta);
            };

            TranslateStatoValidazionePrestazioni(o_ordineoe, o_ordinedettaglio);

            if (o_ordineoe.Erogati != null)
            {
                foreach (TestataErogatoType o_erogante in o_ordineoe.Erogati)
                {
                    if (o_erogante.RigheErogate != null)
                        foreach (RigaErogataType o_prestazioneerogata in o_erogante.RigheErogate)
                        {
                            o_nuovaprestazione = o_ordinedettaglio.GetPrestazione(
                                o_erogante.SistemaErogante.Azienda.Codice,
                                o_erogante.SistemaErogante.Sistema.Codice,
                                o_prestazioneerogata.Prestazione.Codice);

                            if (o_nuovaprestazione == null)
                            {
                                o_nuovaprestazione = new OEOrdinePrestazione();
                                o_ordinedettaglio.Prestazioni.Add(o_nuovaprestazione);
                            };

                            TranslatePrestazioneErogataFromOE(o_nuovaprestazione, o_prestazioneerogata);
                            TranslatePrestazioneErogataFromOEDatiAccessoriPersistenti(o_nuovaprestazione, o_erogante.DatiPersistenti);
                        };
                };
            };

            return o_ordinedettaglio;
        }

        private void TranslatePrestazioneErogataFromOEDatiAccessoriPersistenti(OEOrdinePrestazione o_prestazione, DatiPersistentiType datipersistenti)
        {
            if (o_prestazione == null || datipersistenti == null)
                return;

            foreach (DatoNomeValoreType o_datoaccessorioerogante in datipersistenti)
                o_prestazione.DatiAccessoriErogante.Add(
                    new OEDatoAccessorio(
                        o_datoaccessorioerogante.Nome,
                        o_datoaccessorioerogante.ValoreDato,
                        o_datoaccessorioerogante.TipoDato,
                        1)                         );
        }

        private void TranslateStatoValidazionePrestazioni(StatoType o_ordineoe, OEOrdineDettaglio o_ordinedettaglio)
        {
            int i_index;
            RigaRichiestaType o_rigarichiesta;
            OEOrdinePrestazione o_prestazione;

            if (o_ordineoe.StatoValidazione.Righe != null)
            {
                foreach (StatoRigaValidazioneType o_statoivalidazioneriga in o_ordineoe.StatoValidazione.Righe)
                {
                    i_index = o_statoivalidazioneriga.Index;
                    o_rigarichiesta = o_ordineoe.Ordine.RigheRichieste[i_index - 1];
                    o_prestazione = o_ordinedettaglio.GetPrestazione(
                        o_rigarichiesta.SistemaErogante.Azienda.Codice,
                        o_rigarichiesta.SistemaErogante.Sistema.Codice,
                        o_rigarichiesta.Prestazione.Codice);
                    if (o_prestazione != null)
                    {
                        o_prestazione.StatoValidazione = TranslateStatoValidazioneOrdine(o_statoivalidazioneriga.Stato);
                        o_prestazione.DescrizioneValidazione = o_statoivalidazioneriga.Descrizione;
                    }
                }
            }
        }

        private void TranslatePrestazioneRichestaFromOE(OEOrdinePrestazione o_prestazione, RigaRichiestaType o_prestazioneoe)
        {
            if (o_prestazione == null || o_prestazioneoe == null)
                return;

            o_prestazione.StatoRichiedente = TranslateStatoRichiedente(o_prestazioneoe.OperazioneOrderEntry);

            if (o_prestazioneoe.DatiAggiuntivi != null)
                foreach (DatoNomeValoreType o_datoaccessoriorichiedente in o_prestazioneoe.DatiAggiuntivi)
                    if (o_datoaccessoriorichiedente.DatoAccessorio != null)
                        o_prestazione.DatiAccessoriRichiedente.Add(
                            new OEDatoAccessorio(
                                o_datoaccessoriorichiedente.DatoAccessorio.Codice,
                                o_datoaccessoriorichiedente.ValoreDato,
                                o_datoaccessoriorichiedente.TipoDato,
                                1)                                 );

            o_prestazione.Prestazione.Codice = o_prestazioneoe.Prestazione.Codice;
            o_prestazione.Prestazione.Descrizione = o_prestazioneoe.Prestazione.Descrizione;
            o_prestazione.Prestazione.Tipo = TranslatePrestazioneTipoFromOE(o_prestazioneoe.PrestazioneTipo);

            o_prestazione.Prestazione.Erogante.Codice = o_prestazioneoe.SistemaErogante.Sistema.Codice;
            o_prestazione.Prestazione.Erogante.Descrizione = o_prestazioneoe.SistemaErogante.Sistema.Descrizione;
            o_prestazione.Prestazione.Erogante.CodiceAzienda = o_prestazioneoe.SistemaErogante.Azienda.Codice;
            o_prestazione.Prestazione.Erogante.DescrizioneAzienda = o_prestazioneoe.SistemaErogante.Azienda.Descrizione;

        }

        private void TranslatePrestazioneErogataFromOE(OEOrdinePrestazione o_prestazione, RigaErogataType o_prestazioneoe)
        {
            if (o_prestazione == null || o_prestazioneoe == null)
                return;

            o_prestazione.StatoErogante = TranslateStatoErogante(o_prestazioneoe.StatoOrderEntry);

                        o_prestazione.DataPianificata = o_prestazioneoe.DataPianificata;

            if (o_prestazioneoe.DatiAggiuntivi != null)
                foreach (DatoNomeValoreType o_datoaccessorioerogante in o_prestazioneoe.DatiAggiuntivi)
                    o_prestazione.DatiAccessoriErogante.Add(
                           new OEDatoAccessorio(
                               o_datoaccessorioerogante.Nome,
                               o_datoaccessorioerogante.ValoreDato,
                               o_datoaccessorioerogante.TipoDato,
                               1)                                );
        }

        private OEDatiAccessoriDescrittore TranslateDatoAccessorioFromOE(DatoAccessorioListaType o_datooe)
        {
            OEDatiAccessoriDescrittore OEDato;

            OEDato = new OEDatiAccessoriDescrittore();

            OEDato.Codice = o_datooe.DatoAccessorio.Codice;
            OEDato.Descrizione = o_datooe.DatoAccessorio.Descrizione;
            OEDato.Etichetta = o_datooe.DatoAccessorio.Etichetta;
            OEDato.Gruppo = o_datooe.DatoAccessorio.Gruppo;
            OEDato.Obbligatorio = o_datooe.DatoAccessorio.Obbligatorio;
            OEDato.Ordinamento = o_datooe.DatoAccessorio.Ordinamento;
            OEDato.Ripetibile = o_datooe.DatoAccessorio.Ripetibile;
            OEDato.Testata = o_datooe.DatoAccessorioRichiesta;
            OEDato.Tipo = TranslateTipoDatoAccessorioFromOE(o_datooe.DatoAccessorio.Tipo);
            OEDato.ValidazioneMessaggio = o_datooe.DatoAccessorio.ValidazioneMessaggio;
            OEDato.ValidazioneRegEx = o_datooe.DatoAccessorio.ValidazioneRegex;
            OEDato.Valori = o_datooe.DatoAccessorio.Valori;

            return OEDato;
        }

        private string TranslateElencoEroganti(StatoType o_ordineoe)
        {
            string s_eroganti = "";
            string s_erogante;
            List<string> l_eroganti;

            l_eroganti = new List<string>();
            foreach (var o_prestazione in o_ordineoe.Ordine.RigheRichieste)
            {
                if (o_prestazione.SistemaErogante.Sistema.Descrizione != null)
                {
                    s_erogante = o_prestazione.SistemaErogante.Sistema.Descrizione + " (" + o_prestazione.SistemaErogante.Azienda.Codice + ")";
                    if (!l_eroganti.Contains(s_erogante))
                    {
                        l_eroganti.Add(s_erogante);
                        s_eroganti += s_erogante + ", ";
                    }
                };
            }
            s_eroganti = s_eroganti.TrimEnd(' ');
            s_eroganti = s_eroganti.TrimEnd(',');

            return s_eroganti;
        }

        private List<OESistemaErogante> TranslateElencoErogantiLista(StatoType o_ordineoe)
        {
            List<OESistemaErogante> l_eroganti;
            List<string> l_eroganti_descrittori;
            string s_descrittore;
            OESistemaErogante o_sistemaerogante;

            l_eroganti = new List<OESistemaErogante>();
            l_eroganti_descrittori = new List<string>();
            foreach (var o_prestazione in o_ordineoe.Ordine.RigheRichieste)
            {
                o_sistemaerogante = TranslateSistemaFromOE(o_prestazione.SistemaErogante);
                s_descrittore = o_sistemaerogante.ToString();
                if (!l_eroganti_descrittori.Contains(s_descrittore))
                {
                    l_eroganti.Add(o_sistemaerogante);
                    l_eroganti_descrittori.Add(s_descrittore);
                };
            }

            return l_eroganti;
        }

        private DateTime GrabDataOraProgrammata(StatoType o_ordineoe)
        {
            try
            {
                foreach (DatoNomeValoreType o_datoaccessorio in o_ordineoe.Ordine.DatiAggiuntivi)
                {
                    if (o_datoaccessorio.Nome == "MATDP")
                        return DateTime.Parse(o_datoaccessorio.ValoreDato);
                };
            }
            catch             {
                return DateTime.MinValue;
            }

            return DateTime.MinValue;
        }

        private OEGruppoPrestazione TranslateGruppoPrestazioniFromOE(GruppoPrestazioniListaType o_gruppo)
        {
            return new OEGruppoPrestazione(o_gruppo.Id, o_gruppo.Descrizione, o_gruppo.NumeroPrestazioni, o_gruppo.SistemiEroganti);
        }

        #endregion

        #region COMPARISON

        public bool ComparePrestazione(OEPrestazione o_prestazione, RigaRichiestaType o_prestazione_oe)
        {
            return
                o_prestazione_oe.Prestazione.Codice == o_prestazione.Codice &&
                o_prestazione_oe.SistemaErogante.Azienda.Codice == o_prestazione.Erogante.CodiceAzienda &&
                o_prestazione_oe.SistemaErogante.Sistema.Codice == o_prestazione.Erogante.Codice;
        }

        #endregion

    }

}
