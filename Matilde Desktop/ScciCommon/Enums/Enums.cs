using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace UnicodeSrl.Scci.Enums
{

    public enum EnumAzioni
    {
        [Description("Annullamento")]
        ANN = 10,
        [Description("Cancella")]
        CAN = 20,
        [Description("Completa")]
        COM = 30,
        [Description("Inserisci")]
        INS = 40,
        [Description("Modifica")]
        MOD = 50,
        [Description("Valida")]
        VAL = 60,
        [Description("Visualizza")]
        VIS = 70,
        [Description("Visualizza Schede Riservate")]
        VSR = 80,
        [Description("Visualizza Schede Revisionabili")]
        REV = 90,
        [Description("Condividi")]
        CND = 100
    }

    public enum EnumConfigTable
    {
        [Description("WS x Diagnostics")]
        Diagnostics = 0,
        [Description("Report Manager : Viewer")]
        ReMViewer = 1,
        [Description("Report Manager : Stringa DB")]
        ReMConnectionString = 2,
        [Description("Blocco Uscita per Firma/Validazione")]
        BloccoUscita = 3,
        [Description("News: N° registrazione da visualizzare")]
        NumeroRecordNews = 4,
        [Description("Parametri Vitali : N° di registrazioni da visualizzare")]
        NumeroRecordParametri = 5,
        [Description("Font Predefinito RTF (tipo + dimensione)")]
        FontPredefinitoRTF = 6,
        [Description("Web Service Scci")]
        WebServiceSCCI = 7,
        [Description("Logo Applicativo Easy")]
        LogoEasy = 8,
        [Description("Logo Fornitore")]
        LogoFornitore = 9,
        [Description("Numero Record Allergie")]
        NumeroRecordAllergie = 10,
        [Description("Numero Record Warning")]
        NumeroRecordWarning = 11,
        [Description("Numero Record Parametri Vitali")]
        NumeroRecordParametriVitali = 12,
        [Description("Numero Record Task Infermieristici")]
        NumeroRecordTaskInfermieristici = 13,
        [Description("Numero Record Ordini")]
        NumeroRecordOrdini = 14,
        [Description("Numero Record Appuntamenti")]
        NumeroRecordAppuntamenti = 15,
        [Description("Font Predefinito Maschere")]
        FontPredefinitoForm = 17,
        [Description("Numero Record Evidenza Clinica")]
        NumeroRecordEvidenzaClinica = 18,


        [Description("Referto DWH da Escludere")]
        RefertiDWHdaEscludere = 20,
        [Description("URL Accesso Web Referti DWH")]
        URLAccessoRefertiDWH = 25,

        [Description("Minuti ricerca parametri vitali trasversali")]
        MinutiRicercaParametriVitaliTrasversali = 30,
        [Description("Tipo Scheda Testata Paziente")]
        TipoSchedaTestataPaziente = 31,
        [Description("Tipo Scheda Testata Episodio")]
        TipoSchedaTestataEpisodio = 32,
        [Description("Tipo Scheda Task da Prescrizione")]
        TipoSchedaTaskDaPrescrizione = 33,
        [Description("Foto Paziente: Larghezza MAX")]
        FotoPazienteMaxWidth = 34,
        [Description("Foto Paziente: Altezza MAX")]
        FotoPazienteMaxHeight = 35,
        [Description("Dimensione massima Mb Allegati")]
        AllegatiDimensioneMassimaMb = 36,
        [Description("Allegati : prossimo ID per Allegato Cartaceo")]
        AllegatiProssimoID = 37,
        [Description("Gestione Automatica numero cartella")]
        GestAutoNumCartella = 38,
        [Description("Assegnazione stesso numero cartella in caso di trasferimento")]
        AssStessoNumCartella = 39,
        [Description("Ruolo per Consulenze Esterne")]
        RuoloConsulente = 40,
        [Description("Cartella Ambulatoriale: n° Giorni Evidenza Clinica")]
        NumeroGiorniEvidenzaClinica = 41,
        [Description("Grafici: fattore di curvatura (da 0 a 1)")]
        FattoreCurvaturaGrafici = 42,
        [Description("RTF Intestazione Stampe")]
        RTFIntestazioneStampe = 43,
        [Description("Mb minimi per test connettività")]
        MbMINNetworkAvailable = 44,
        [Description("Lettera Dimissione: URL da aprire")]
        LetteraDimissioneURL = 45,
        [Description("Lettera Dimissione: URL da aprire AUSL")]
        LetteraDimissioneURLAUSL = 1045,
        [Description("Codice Azienda")]
        Azienda = 46,
        [Description("Tipo Voce Diario Medico")]
        TipoVoceDiarioMedico = 47,
        [Description("Tipo Voce Diario Infermieristico")]
        TIpoVoceDiarioInfermieristico = 48,

        [Description("PSC: URL da aprire")]
        PscURL = 49,
        [Description("PSC: URL da aprire AUSL")]
        PscURLAUSL = 1049,

        [Description("Apri DOCX tramite shell di Windows")]
        ApriDOCXtramiteshell = 50,
        [Description("Apri PDF tramite shell di Windows")]
        ApriPDFtramiteshell = 51,
        [Description("Stringa da cercare per forzare apertura WORD")]
        ParolaChiaveAperturaWord = 52,
        [Description("Stringa da cercare per forzare apertura ACROBAT")]
        ParolaChiaveAperturaAcrobat = 53,

        [Description("Delta Ore Erogazione Task")]
        DeltaOreErogazioneTask = 54,

        [Description("FUT Colore Cambio Giorno")]
        FUTColoreCambioGiorno = 55,

        [Description("FUT Colore Ultima Somministrazione Programmata")]
        FUTColoreUltimaSommProgrammata = 56,
        [Description("FUT Carattere Ultima Somministrazione Programmata")]
        FUTCarattereUltimaSommProgrammata = 57,

        [Description("FUT Colore Ultima Somministrazione Erogata")]
        FUTColoreUltimaSommErogata = 58,
        [Description("FUT Carattere Ultima Somministrazione Erogata")]
        FUTCarattereUltimaSommErogata = 59,

        [Description("RTF Intestazione Stampe Sintetica")]
        RTFIntestazioneStampeSintetica = 60,

        [Description("Aggiorna dati estesi SAC su apertura cartella")]
        AggiornaSACAperturaCartella = 61,

        [Description("Tipo Scheda Task da Prescrizione Semplice")]
        TipoPrescrizioneSemplice = 62,

        [Description("Campo Scheda Task da Prescrizione Semplice")]
        CampoSchedaDaPrescrizioneSemplice = 63,

        [Description("FUT Colore Ultima Somministrazione Programmata Chiusa")]
        FUTColoreUltimaSommProgrChiusa = 64,

        [Description("Grafici: n. max giorni per mostrare Tickmark (0=NO Tickmark)")]
        GraficiMaxGiorniTickmark = 66,

        [Description("Dominio Predefinito")]
        DominioPredefinito = 70,

        [Description("Diagnostics Client Percorso")]
        DiagnosticsClientPercorso = 71,
        [Description("Diagnostics Client Giorni")]
        DiagnosticsClientGiorni = 72,
        [Description("Diagnostics Client Eventi Log")]
        DiagnosticsClientEventiLog = 73,

        [Description("Colore Festività Calendari")]
        ColoreFestivitaCalendari = 74,

        [Description("Elenco Campi Appuntamenti Episodio")]
        ElencoCampiAppuntamentoEpisodio = 80,
        [Description("Elenco Campi Appuntamenti Paziente")]
        ElencoCampiAppuntamentoPaziente = 90,

        [Description("Appuntamenti Multipli : N° min Occorrenze")]
        AppuntamentiMinOccorrenze = 91,
        [Description("Appuntamenti Multipli : N° MAX Occorrenze")]
        AppuntamentiMAXOccorrenze = 92,


        [Description("Timer Applicazione")]
        TimerApplicazione = 100,
        [Description("Timer Ambiente")]
        TimerAmbiente = 101,

        [Description("Versione SCCI")]
        VersioneSCCI = 110,
        [Description("Abilita Controllo Versione SCCI")]
        AbilitaControlloVersioneSCCI = 111,

        [Description("Versione SCCI Management")]
        VersioneSCCIManagement = 112,
        [Description("Abilita Controllo Versione SCCI Management")]
        AbilitaControlloVersioneSCCIManagement = 113,

        [Description("Splash Easy")]
        SplashEasy = 115,
        [Description("Splash Management")]
        SplashManagement = 116,

        [Description("Timer Foglio Unico")]
        TimerFUT = 150,
        [Description("Numero Massimo Owner Foglio Unico")]
        NumeroMassimoOwnerFUT = 160,
        [Description("Numero Massimo Caratteri Owner Foglio Unico")]
        NumeroMassimoCaratteriOwnerFUT = 161,

        [Description("FUT Percentuale altezza righe Normale")]
        PercentualeAltezzaRighaNormaleFUT = 162,
        [Description("FUT Percentuale altezza righe Compatta")]
        PercentualeAltezzaRighaCompattaFUT = 163,

        [Description("Ricerca Ambulatoriali n° ultime ricerche")]
        NumeroUltimeRicercheAmbulatoriali = 170,
        [Description("Pazienti Seguiti: Colore sfondo")]
        ColorePazienteSeguito = 180,

        [Description("Chiusura Cartelle: Colore selezione")]
        ColoreChiusuraCartelle = 185,

        [Description("Tempo Dimessi Trasferiti")]
        TempoDimessiTrasferiti = 200,

        [Description("Tipo Salvataggio Scheda")]
        TipoSalvataggioScheda = 190,
        [Description("Path Salvataggio Scheda")]
        PathSalvataggioScheda = 191,

        [Description("Allegati Schede: Larghezza Antemprima")]
        AllegatiSchedeAntemprimaWidth = 192,
        [Description("Allegati Schede: Altezza Antemprima")]
        AllegatiSchedeAntemprimaHeight = 193,

        [Description("Allegati Schede: Larghezza Stampa")]
        AllegatiSchedeStampaWidth = 194,
        [Description("Allegati Schede: Altezza Stampa")]
        AllegatiSchedeStampaHeight = 195,

        [Description("Tipo Consegna da DCL")]
        TipoConsegnaDaDCL = 196,

        [Description("Web Service SAC")]
        WebServiceSAC = 500,
        [Description("Web Service OE")]
        WebServiceOE = 501,
        [Description("Web Service DWH")]
        WebServiceDWH = 502,
        [Description("Web Service DWH Laboratorio")]
        WebServiceDWHLAB = 503,
        [Description("Web Service AntiBlastica")]
        WebServiceAB = 504,
        [Description("Web Service VNA")]
        WebServiceVNA = 505,
        [Description("Web Service UFA")]
        WebServiceUFA = 506,

        [Description("Web Service SAC Consensi")]
        WebServiceSACConsensi = 507,
        [Description("Abilita SAC Consensi")]
        SACConsensiAbilita = 508,
        [Description("Giorni pregressi EVC in consultazione")]
        GGPregressiEVCConsultazione = 509,


        [Description("Web Service SAC UserName")]
        WebServiceSACUserName = 510,
        [Description("Web Service OE UserName")]
        WebServiceOEUserName = 511,
        [Description("Web Service DWH UserName")]
        WebServiceDWHUserName = 512,
        [Description("Web Service AntiBlastica UserName")]
        WebServiceABUserName = 514,
        [Description("Web Service VNA UserName")]
        WebServiceVNAUserName = 515,
        [Description("Web Service UFA UserName")]
        WebServiceUFAUserName = 516,
        [Description("Web Service SAC Password")]
        WebServiceSACPassword = 520,
        [Description("Web Service OE Password")]
        WebServiceOEPassword = 521,
        [Description("Web Service DWH Password")]
        WebServiceDWHPassword = 522,
        [Description("Web Service AntiBlastica Password")]
        WebServiceABPassword = 524,
        [Description("Web Service VNA Password")]
        WebServiceVNAPassword = 525,
        [Description("Web Service UFA Password")]
        WebServiceUFAPassword = 526,
        [Description("Web Service SAC numero massimo record")]
        WebServiceSACNumRecords = 530,
        [Description("Web Service OE numero massimo record")]
        WebServiceOENumRecords = 531,
        [Description("Web Service DWH numero massimo record")]
        WebServiceDWHNumRecords = 532,
        [Description("Web Service MOW")]
        WebServiceMOW = 533,
        [Description("Web Service MOW AUSL")]
        WebServiceMOWAUSL = 1533,
        [Description("Web Service OE RR")]
        WebServiceOERR = 534,
        [Description("Web Service OE OSU")]
        WebServiceOEOSU = 535,
        [Description("Web Service PSC")]
        WebServicePSC = 536,
        [Description("Web Service PSC UserName")]
        WebServicePSCUserName = 537,
        [Description("Web Service PSC Password")]
        WebServicePSCPassword = 538,
        [Description("Web Service PSC AUSL")]
        WebServicePSCAUSL = 1536,
        [Description("Web Service PSC UserName AUSL")]
        WebServicePSCUserNameAUSL = 1537,
        [Description("Web Service PSC Password AUSL")]
        WebServicePSCPasswordAUSL = 1538,

        [Description("URL Web Site FastDRG2")]
        WebSiteFastDRG2 = 539,

        [Description("Web Service OE RR Username")]
        WebServiceOERRUserName = 541,
        [Description("Web Service OE RR Password")]
        WebServiceOERRPassword = 542,

        [Description("PACS RDP: Server")]
        PACSRDPServer = 543,
        [Description("PACS RDP: UserName")]
        PACSRDPUserName = 544,
        [Description("PACS RDP: Password")]
        PACSRDPPassword = 545,

        [Description("WS Updater: Indirizzo URL del server di update")]
        WSUpdaterAddress = 600,
        [Description("WS Updater: Dominio Windows dell’ utente che esegue la procedura di update")]
        WSUpdaterDomain = 601,
        [Description("WS Updater: Username dell’ utente che esegue la procedura di update")]
        WSUpdaterUserName = 602,
        [Description("WS Updater: Password criptata dell’ utente che esegue la procedura di update")]
        WSUpdaterPassword = 603,
        [Description("WS Updater: Indica se visualizzare l’ interfaccia grafica di aggiornamento")]
        WSUpdaterShowUI = 604,
        [Description("WS Updater: Dominio Windows di un utente abitato ad accedere al servizio")]
        WSUpdaterSvcSecDomain = 605,
        [Description("WS Updater: Username Windows di un utente abitato ad accedere al servizio")]
        WSUpdaterSvcSecUserName = 606,
        [Description("WS Updater: Password criptata dell'utente abitato ad accedere al servizio")]
        WSUpdaterSvcSecPassword = 607,
        [Description("WS Updater: Indica se utilizzare il folder dell'utente")]
        WSUpdaterUseUserFolder = 608,
        [Description("WS Updater: Indica se confrontare sempre tutti i files")]
        WSUpdaterAlwaysCheckFiles = 609,

        [Description("Web Service Scci: Dominio+Username")]
        WebServiceSCCIUserName = 670,
        [Description("Web Service Scci: Password")]
        WebServiceSCCIPassword = 671,
        [Description("Web Service Scci: HTTPS")]
        WebServiceSCCIHTTPS = 672,

        [Description("Elaborazione Sistemi: Codice Utente di elaborazione")]
        ElabSistemiUserName = 680,
        [Description("Elaborazione Sistemi: Codice Ruolo utente di elaborazione")]
        ElabSistemiCodRuolo = 681,

        [Description("Analisi : Stringa DB")]
        AnalisiConnectionString = 700,

        [Description("Timer Inattività")]
        TimerInattivita = 800,

        [Description("Moltiplicatore x lettura News Hard")]
        MoltiplicatoreNewsHard = 801,

        [Description("Debug")]
        Debug = 999,

        [Description("Diagnostics: stringa di connessione db")]
        DiagnosticsDbConn = 1540,

        [Description("Diagnostics: stringa di connessione db")]
        ScciUnsDbConn = 1541,

        [Description("ScciWeb: Path 1 icone")]
        ScciWebPathIcone1 = 1600,
        [Description("ScciWeb: Path 2 icone")]
        ScciWebPathIcone2 = 1605,
        [Description("ScciWeb: Path icone Utente")]
        ScciWebPathIconeUtente = 1610,
        [Description("ScciWeb: Path icone Password")]
        ScciWebPathIconePassword = 1625

    }

    public enum EnumConfigCETable
    {
        [Description("Logo Fabbricatore")]
        LogoFabbricatore = 100,
        [Description("Descrizione Fabbricatore")]
        DescrizioneFabbricatore = 110,
        [Description("Logo Prodotto")]
        LogoProdotto = 120,
        [Description("Descrizione Prodotto")]
        DescrizioneProdotto = 130,
        [Description("Logo Manuale")]
        LogoManuale = 140,
        [Description("Descrizione Manuale")]
        DescrizioneManuale = 150
    }

    public enum EnumEntita
    {
        [Description("Agende")]
        AGE = 10,
        [Description("Alert Anamnestici e Allergie")]
        ALA = 20,
        [Description("Alert Generici")]
        ALG = 30,
        [Description("Allegati")]
        ALL = 40,
        [Description("Appuntamenti")]
        APP = 50,
        [Description("Diario Clinico")]
        DCL = 60,
        [Description("Episodio")]
        EPI = 70,
        [Description("Evidenza Clinica")]
        EVC = 80,
        [Description("Order Entry")]
        OE = 90,
        [Description("Paziente Movimento")]
        PAZ = 100,
        [Description("Paziente Anagrafica")]
        ANA = 105,
        [Description("Prescrizioni")]
        PRF = 110,
        [Description("Parametri Vitali")]
        PVT = 120,
        [Description("Report")]
        RPT = 130,
        [Description("Schede")]
        SCH = 140,
        [Description("Testi Predefiniti")]
        TST = 150,
        [Description("Work List Infermieristica")]
        WKI = 160,
        [Description("Note Agende")]
        NTE = 170,
        [Description("Cartella")]
        CAR = 180,
        [Description("Trasferimenti")]
        TRA = 190,
        [Description("ADT")]
        ADT = 195,
        [Description("PRT")]
        PRT = 205,
        [Description("Note")]
        NTG = 111,
        [Description("Segnalibri")]
        SGL = 145,
        [Description("Cartelle in Visione")]
        CIV = 146,
        [Description("Pazienti Seguiti")]
        PZS = 147,
        [Description("Documenti Firmati")]
        DCF = 148,
        [Description("Banche Dati Medicali")]
        EBM = 149,
        [Description("Pazienti in Visione")]
        PIV = 150,
        [Description("Filtri Speciali")]
        FLS = 206,
        [Description("Protocollo Attività")]
        PRA = 207,
        [Description("Gestione Code")]
        CDA = 210,
        [Description("Screen")]
        SCR = 215,
        [Description("News")]
        NWS = 218,
        [Description("Profili Prescrizione")]
        PRP = 219,
        [Description("Consensi")]
        CNS = 220,
        [Description("Consensi Calcolati")]
        CNC = 221,
        [Description("Consegne")]
        CSG = 222,
        [Description("Consegne Paziente")]
        CSP = 223,
        [Description("Consegne Paziente Ruoli")]
        CSR = 224,
        [Description("Cartella Ambulatoriale")]
        CAC = 225,
        [Description("Non Definita")]
        XXX = 200
    }

    public enum EnumModules
    {
        [Description("Pazienti Menù")]
        Pazienti_Menu = 0,
        [Description("Parametri Vitali Menù")]
        ParamV_Menu = 1,
        [Description("Worklist Menù")]
        WorkL_Menu = 2,
        [Description("Agende Menù")]
        Agende_Menu = 3,
        [Description("Consulenza Menù")]
        Consulenza_Menu = 4,
        [Description("Cartella Ambulatoriale Menù")]
        CartellaAmbulatoriale_Menu = 5,
        [Description("Chiusura Cartella Menù")]
        ChiusuraCartella_Menu = 6,
        [Description("Diario Clinico Inserisci")]
        DiarioC_Inserisci = 7,
        [Description("Diario Clinico Valida")]
        DiarioC_Valida = 8,
        [Description("Schede Menù")]
        Schede_Menu = 9,
        [Description("Diario Clinico Menù")]
        DiarioC_Menu = 10,
        [Description("Cartelle In Visione")]
        CartelleIV_Menu = 11,
        [Description("Prescrizioni Menù")]
        Prescr_Menu = 12,
        [Description("Percorso Ambulatoriale Menù")]
        PercorsoAmb_Menu = 14,
        [Description("Evidenza Clinica Menù")]
        EvidenzaC_Menu = 15,
        [Description("Ordini Menù")]
        Ordini_Menu = 16,
        [Description("Lettera Dimissione Menù")]
        LetteraD_Menu = 17,
        [Description("Allegati Menù")]
        Allegati_Menu = 18,
        [Description("EBM Menù")]
        Ebm_Menu = 19,
        [Description("Foglio Unico Terapia Menù")]
        FoglioUT_Menu = 20,
        [Description("Parametri Vitali Inserisci")]
        ParamV_Inserisci = 21,
        [Description("Schede Inserisci")]
        Schede_Inserisci = 22,
        [Description("Schede Modifica")]
        Schede_Modifica = 23,
        [Description("Schede Visualizza")]
        Schede_Visualizza = 24,
        [Description("Schede Cancella")]
        Schede_Cancella = 25,
        [Description("Task Infermieristici Inserisci")]
        WorkL_Inserisci = 26,
        [Description("Agende Inserisci")]
        Agende_Inserisci = 27,
        [Description("Agende Modifica")]
        Agende_Modifica = 28,
        [Description("Agende Visualizza")]
        Agende_Visualizza = 29,
        [Description("Agende Cancella")]
        Agende_Cancella = 30,
        [Description("Terapie Farmacologiche/Prescrizioni Inserisci")]
        Prescr_Inserisci = 31,
        [Description("Terapie Farmacologiche/Prescrizioni Modifica")]
        Prescr_Modifica = 32,
        [Description("Terapie Farmacologiche/Prescrizioni Valida")]
        Prescr_Valida = 33,
        [Description("Terapie Farmacologiche/Prescrizioni Cancella")]
        Prescr_Cancella = 34,
        [Description("Terapie Farmacologiche/Prescrizioni Sospendi")]
        Prescr_Annulla = 35,
        [Description("Alert Allergie e Anamnesi Inserisci")]
        AlertAA_Inserisci = 36,
        [Description("Alert Allergie e Anamnesi Visualizza")]
        AlertAA_Visualizza = 37,
        [Description("Alert Allergie e Anamnesi Cancella")]
        AlertAA_Cancella = 38,
        [Description("Alert Allergie e Anamnesi Menu")]
        AlertAA_Menu = 39,
        [Description("Alert Generici Inserisci")]
        AlertG_Inserisci = 40,
        [Description("Alert Generici Visualizza")]
        AlertG_Visualizza = 41,
        [Description("Alert Generici Cancella")]
        AlertG_Cancella = 42,
        [Description("Alert Generici Menu")]
        AlertG_Menu = 43,
        [Description("Alert Generici Vista")]
        AlertG_Vista = 44,
        [Description("Evidenza Clinica Visualizza")]
        EvidenzaC_Visualizza = 45,
        [Description("Evidenza Clinica Includi")]
        EvidenzaC_Includi = 46,
        [Description("Evidenza Clinica Vista")]
        EvidenzaC_Vista = 48,
        [Description("Schede Dettaglio Paziente")]
        Schede_Dettaglio_Paziente = 49,
        [Description("Schede Dettaglio Episodio")]
        Schede_Dettaglio_Episodio = 50,
        [Description("Modifica Foto Pazienti")]
        Pazienti_Modifica_Foto = 51,
        [Description("Visualizza Foto Pazienti")]
        Pazienti_Visualizza_Foto = 52,
        [Description("Visualizza Allegati ")]
        Allegati_Visualizza = 53,
        [Description("Inserisci Allegati")]
        Allegati_Inserisci = 54,
        [Description("Cancella Allegati")]
        Allegati_Cancella = 55,
        [Description("Modifica Allegati")]
        Allegati_Modifica = 56,
        [Description("Apri Cartella")]
        Cartella_Apri = 57,
        [Description("Chiudi Cartella")]
        Cartella_Chiudi = 58,
        [Description("Agende Annulla")]
        Agende_Annulla = 59,
        [Description("Ordini Visualizza")]
        Ordini_Visualizza = 60,
        [Description("Ordini Modifica")]
        Ordini_Modifica = 61,
        [Description("Ordini Cancella")]
        Ordini_Cancella = 62,
        [Description("Ordini Inserisci")]
        Ordini_Inserisci = 63,
        [Description("Ordini Inoltra")]
        Ordini_Inoltra = 64,
        [Description("Mow Menù")]
        Mow_Menu = 65,
        [Description("Task Infermieristici Amministrazione")]
        WorkL_Admin = 66,
        [Description("Psc Menù")]
        Psc_Menu = 67,
        [Description("Cartella In Visione Visualizza")]
        CartellaIV_Visualizza = 68,
        [Description("Cartella In Visione Inserisci")]
        CartellaIV_Inserisci = 69,
        [Description("Cartella In Visione Modifica")]
        CartellaIV_Modifica = 70,
        [Description("Cartella In Visione Cancella")]
        CartellaIV_Cancella = 71,
        [Description("Pazienti Seguiti Visualizza")]
        PazientiSeguiti_Visualizza = 72,
        [Description("Pazienti Seguiti Inserisci")]
        PazientiSeguiti_Inserisci = 73,
        [Description("Pazienti Seguiti Modifica")]
        PazientiSeguiti_Modifica = 74,
        [Description("Pazienti Seguiti Cancella")]
        PazientiSeguiti_Cancella = 75,
        [Description("Stampa Cartelle Chiuse Menu")]
        StampaCartelleChiuse_Menu = 76,
        [Description("Cartella Firma Multipla Elenco")]
        Cartella_FMElenco = 77,
        [Description("Altre Funzioni")]
        AltreFunzioni_Menu = 78,
        [Description("Pre-Traferimento")]
        PreTrasferimento_Menu = 79,
        [Description("Agende Trasversali")]
        Agende_Home = 80,
        [Description("Evidenza Clinica Trasversale")]
        EvidenzaC_Home = 81,
        [Description("Ordini Trasversale")]
        Ordini_Home = 82,
        [Description("Parametri Vitali Trasversali")]
        ParamV_Home = 83,
        [Description("Worklist Trasversale")]
        WorkL_Home = 84,
        [Description("Menù Principale CDSS")]
        Menu_Principale_CDSS = 85,
        [Description("Matilde Home Menù")]
        MatHome_Menu = 86,
        [Description("Firma Cartelle Aperte Menù")]
        FirmaCartelleAperte_Menu = 87,
        [Description("Firma Cartella Aperta")]
        Firma_CartellaAperta = 88,
        [Description("Consegne Menù")]
        Consegne_Menu = 89,
        [Description("Consegne Inserisci")]
        Consegne_Inserisci = 90,
        [Description("Consegne Annulla")]
        Consegne_Annulla = 91,
        [Description("Consulenze Trasversali")]
        Consulenze_Home = 92,
        [Description("Consegne Paziente Menù")]
        ConsegneP_Menu = 93,
        [Description("Consegne Paziente Inserisci")]
        ConsegneP_Inserisci = 94,
        [Description("Consegne Paziente Annulla")]
        ConsegneP_Annulla = 95,
        [Description("Consegne Paziente Vista")]
        ConsegneP_Visto = 96,
        [Description("Consegne Paziente Cancella")]
        ConsegneP_Cancella = 97,
        [Description("Diario Clinico Invia Consegne Paziente")]
        DiarioC_InviaConsegne = 98,
        [Description("Monitor Ordini Trasversale")]
        OrdiniMonitor_Home = 99
    }

    public enum EnumPluginClient
    {
        [Description("Worklist - Erogazione Task - Prima")]
        WKI_EROGA_PRIMA = 1,
        [Description("Worklist - Erogazione Task - Dopo")]
        WKI_EROGA_DOPO = 2,
        [Description("Worklist - Erogazione Task - Altrimenti")]
        WKI_EROGA_ALTRIMENTI = 3,
        [Description("Worklist - Controllo Annulla Task AntiBlastica [da maschera]")]
        WKI_ANNULLA_PRIMA_PU = 4,
        [Description("Worklist - Controlla Cancellazione Task AntiBlastica")]
        WKI_CANCELLA_PRIMA = 5,
        [Description("Worklist - Controllo Eroga Task AntiBlastica [da maschera]")]
        WKI_EROGA_PRIMA_PU = 6,
        [Description("Worklist - Controllo Modifica (orario) Task AntiBlastica")]
        WKI_MODIFICA_PRIMA = 7,
        [Description("Worklist - Controllo Modifica Task AntiBlastica [da maschera]")]
        WKI_MODIFICA_PRIMA_PU = 8,
        [Description("Prescrizione Tempi - Sospendi Prescrizione Tempi: controlla task antiblastica")]
        PRT_SOSPENDI_PRIMA = 9,
        [Description("Prescrizione Tempi - Controllo Validazione Antiblastica [da maschera]")]
        PRT_VALIDA_DOPO_PU = 10,

        [Description("Agende - Annulla Appuntamento - Prima")]
        APP_ANNULLA_PRIMA_PU = 27,
        [Description("Agende - Nuovo Appuntamento - Dopo")]
        APP_NUOVO_DOPO = 11,
        [Description("Agende - Modifica Appuntamento - Dopo ")]
        APP_MODIFICA_DOPO = 19,
        [Description("Agende - Cancella Appuntamento - Dopo ")]
        APP_CANCELLA_DOPO = 20,
        [Description("Agende - Annulla Appuntamento - Dopo")]
        APP_ANNULLA_DOPO = 21,
        [Description("Appuntamenti - Pre Salvataggio")]
        APP_PRE_SALVA = 26,

        [Description("Prescrizione Tempi - Cancella Prescrizione Tempi: controlla task antiblastica")]
        PRT_CANCELLA_PRIMA = 12,

        [Description("Menu Cartella")]
        MENU_CARTELLA = 13,
        [Description("Menu Ambulatoriale")]
        MENU_AMBULATORIALE = 14,

        [Description("Schede - Validazione - Dopo")]
        SCH_VALIDA_DOPO = 15,

        [Description("Schede - Svalidazione - Dopo")]
        SCH_SVALIDA_DOPO = 16,

        [Description("Integrazioni da Management")]
        MANAGEMENT_INTEGRA = 17,

        [Description("Ordini - Inoltra - Dopo")]
        OE_INOLTRA_DOPO = 18,

        [Description("Schede - Nuova - Dopo")]
        SCH_NUOVA_DOPO = 22,

        [Description("Schede - Modifica - Dopo")]
        SCH_MODIFICA_DOPO = 23,

        [Description("Schede - Cancella - Dopo")]
        SCH_CANCELLA_DOPO = 24,

        [Description("Azione generica di avvio CDSS da menù generici")]
        CDSS_A_RICHIESTA = 25,

        [Description("Diario clinico - Valida - Prima PU")]
        DCL_VALIDA_PRIMA_PU = 30,
        [Description("Diario clinico - Valida - Prima")]
        DCL_VALIDA_PRIMA = 31,
        [Description("Diario clinico - Valida - Dopo")]
        DCL_VALIDA_DOPO = 32,
        [Description("Diario clinico - Valida - Altrimenti")]
        DCL_VALIDA_ALTRIMENTI = 33

    }

    public enum enumZoomfactor
    {
        zf25 = 25,
        zf50 = 50,
        zf75 = 75,
        zf100 = 100,
        zf125 = 125,
        zf150 = 150,
        zf175 = 175,
        zf200 = 200,
        zf225 = 225,
        zf250 = 250,
        zf275 = 275,
        zf300 = 300,
        zf325 = 325,
        zf350 = 350,
        zf375 = 375,
        zf400 = 400,
        zf425 = 425,
        zf450 = 450,
        zf475 = 475,
        zf500 = 500
    }

    public enum EnumMaschere
    {
        [Description("Menu Principale")]
        MenuPrincipale = 0,
        [Description("Dettaglio News")]
        DettaglioNews = -1,
        [Description("Seleziona Ruolo")]
        SelezionaRuolo = -3,
        [Description("Cambia Utente")]
        CambiaUtente = -4,
        [Description("Richiedi Password")]
        RichiediPassword = -5,

        [Description("Pazienti")]
        RicercaPazienti = 1,
        [Description("SelezionaCartella")]
        SelezionaCartella = 9,

        [Description("Cartella Paziente")]
        CartellaPaziente = 11,
        [Description("Cartella in Visione")]
        CartellaInVisione = 110,

        [Description("Schede")]
        Schede = 111,
        [Description("Schede Modale")]
        SchedeModal = 911,

        [Description("Diario Clinico")]
        DiarioClinico = 112,
        [Description("Validazione Voci di Diario")]
        ValidazioneVociDiDiario = 1122,
        [Description("Editing Voce di Diario")]
        EditingVoceDiDiario = 1123,

        [Description("Voce di Diario")]
        VoceDiDiarioNM = 11231,

        [Description("Seleziona Tipo Nuova Voce di Diario")]
        SelezionaTipoVoceDiario = 1124,
        [Description("Parametri Vitali")]
        ParametriVitali = 113,
        [Description("Grafico Parametri Vitali")]
        GraficoParametriVitali = 1132,
        [Description("Terapie Farmacologiche")]
        TerapieFarmacologiche = 114,
        [Description("Worklist")]
        WorklistInfermieristica = 115,

        [Description("Agende Paziente")]
        AgendePaziente = 116,
        [Description("Selezione Tipo Appuntamento")]
        SelezioneTipoAppuntamento = 1160,
        [Description("Selezione Agende Appuntamento")]
        SelezioneAgendeAppuntamento = 1161,
        [Description("Selezione Stato Appuntamento")]
        SelezioneStatoAppuntamento = 1162,
        [Description("Selezione Appuntamento")]
        SelezioneAppuntamento = 11611,
        [Description("Annulla Appuntamento")]
        AnnullaAppuntamento = 11612,

        [Description("Cartella Paziente Chiusa")]
        CartellaPazienteChiusa = 12,

        [Description("Agende Trasversali")]
        AgendeTrasversali = 4,
        [Description("Selezione Paziente SAC")]
        RicercaSAC = 41,
        [Description("Selezione Nota")]
        SelezioneNota = 42,
        [Description("Ricorrenza Nota")]
        RicorrenzaNota = 421,

        [Description("Selezione Testi Note Predefiniti")]
        TestiNotePredefiniti = 422,

        [Description("Tracker Paziente")]
        TrackerPaziente = 44,


        [Description("Evidenza Clinica")]
        EvidenzaClinica = 117,
        [Description("Visualizza Evidenza Clinica")]
        EditingEvidenzaClinica = 1171,
        [Description("Visualizzazione Grafica Lab Referto")]
        GraficiEvidenzaClinica = 1172,
        [Description("Visualizza Referto PDF")]
        EvidenzaClinicaPDF = 1173,
        [Description("Evidenza Clinica: apertura PACS da Access Number")]
        EvidenzaClinicaPACS = 1174,

        [Description("Gestione Ordini")]
        GestioneOrdini = 118,
        [Description("Modifica Ordini")]
        EditingOrdine = 1181,
        [Description("Modifica Ordini Dati Aggiuntivi")]
        EditingOrdineDatiAggiuntivi = 11811,
        [Description("Modifica Ordini Dati Aggiuntivi per Trasfusionale")]
        EditingOrdineDatiAggiuntiviTrasfusionale = 11812,
        [Description("Visualizza Ordine")]
        VisualizzaOrdine = 1182,

        [Description("Lettera Di Dimissione")]
        LetteraDiDimissione = 119,
        [Description("Editing Parametri Vitali")]
        EditingParametriVitali = 120,
        [Description("Selezione Parametri Vitali")]
        SelezioneTipoParametriVitali = 121,
        [Description("Editing Task Infermieristici")]
        EditingTaskInfermieristici = 122,
        [Description("Selezione Task Infermieristici")]
        SelezioneTipoTaskInfermieristici = 123,
        [Description("Erogazione Task Infermieristici")]
        ErogazioneTaskInfermieristici = 124,
        [Description("Selezione Tipo Prescrizione")]
        SelezioneTipoPrescrizione = 125,
        [Description("Editing Prescrizione")]
        EditingPrescrizione = 126,
        [Description("Editing Prescrizione Tempi")]
        EditingPrescrizioneTempi = 127,
        [Description("Selezione Via di Somministrazione")]
        SelezioneViaSomministrazione = 128,
        [Description("Elenco task infermieristici in annullamento da prestazione")]
        AnnullaPrescrizioneTempi = 129,
        [Description("Conferma Presa in Carico")]
        ConfermaPresainCarico = 130,
        [Description("Selezione Cartella Collegabile")]
        SelezionaCartellaCollegabile = -1071,
        [Description("Copia Prescrizioni")]
        CopiaPrescrizioni = 131,
        [Description("Sposta Task Infermieristici in altra Cartella")]
        SpostaTaskInAltraCartella = 132,
        [Description("Prosegui Terapia")]
        ProseguiTerapia = 133,
        [Description("Selezione Task Infermieristici da worklist trasversale")]
        SelezioneTipoTaskInfermieristiciDaWKITrasversale = 134,
        [Description("Editing Task Infermieristici da Protocollo")]
        EditingTaskInfermieristiciProtocollo = 135,
        [Description("Multi Task Infermieristici")]
        MultiTaskInfermieristici = 136,
        [Description("Editing Prescrizioni da Protocollo")]
        EditingPrescrizioniProtocollo = 137,
        [Description("Conferma Presa in Carico Ambulatoriale")]
        ConfermaPresainCaricoAmbulatoriale = 138,
        [Description("Allegati")]
        Allegati = 1110,
        [Description("Selezione Tipo Allegato")]
        SelezioneTipoAllegato = 11100,
        [Description("Allegati Inserisci Virtuale")]
        AllegatiInserisciVirtuale = 11101,
        [Description("Allegati Inserisci Elettronico")]
        AllegatiInserisciElettronico = 11102,
        [Description("Allegati Modifica")]
        AllegatiEditing = 11103,
        [Description("Allegati - Acquisizione")]
        AllegatiAcquisizione = 11104,

        [Description("EBM (Evidence Based Medicine)")]
        EBM = 1111,
        [Description("Foglio Unico")]
        FoglioUnico = 1112,

        [Description("Scheda")]
        Scheda = 11111,
        [Description("Selezione Tipo Scheda")]
        SelezioneTipoScheda = 111112,
        [Description("Zoom Anteprima RTF Scheda")]
        ZoomAnteprimaRTFScheda = 1118,
        [Description("Scheda")]
        SchedaNM = 111111,


        [Description("Refertazione")]
        Mow = 1113,

        [Description("Prescrizione Farmaci (PSC)")]
        Psc = 1114,

        [Description("Importazione Testi DWH")]
        ImportaDWH = 1115,

        [Description("Immagini VNA")]
        ImmaginiVNA = 1116,

        [Description("Report ReM")]
        ReportReM = 1117,

        [Description("Immagine")]
        Immagine = 1119,

        [Description("Rilevazione Parametri Vitali Trasversali")]
        ParametriVitaliTrasversali = 2,
        [Description("Identificazione Iterata Paziente")]
        IdentificazioneIterataPaziente = 21,

        [Description("Refertazione Consulenze Esterne - Selezione Paziente")]
        Consulenze_RicercaPaziente = 5,
        [Description("Refertazione Consulenze Esterne")]
        Consulenze_Refertazione = 51,
        [Description("Verifica LDAP del Consulente")]
        Consulenze_Login = 511,

        [Description("Worklist Trasversale")]
        WorklistInfermieristicaTrasversale = 3,

        [Description("Chiusura Cartelle")]
        ChiusuraCartelle = 6,

        [Description("Evidenza Clinica Trasversale")]
        EvidenzaClinicaTrasversale = 8,

        [Description("Order Entry Trasversale")]
        OrderEntryTrasversale = 15,

        [Description("Monitor Order Entry Trasversale")]
        OrderEntryMonitorTrasversale = 13,

        [Description("Stampa Cartelle Chiuse")]
        StampaCartelleChiuse = 10,

        [Description("Cartelle in Visione")]
        CartelleInVisione = 16,
        [Description("Paziente in Visione")]
        PazienteInVisione = 161,

        [Description("Firma Cartelle Aperte")]
        FirmaCartelleAperte = 17,

        [Description("Consegne")]
        Consegne = 18,
        [Description("Editing Consegne")]
        EditingConsegna = 181,

        [Description("Selezione Tipo Consegna")]
        SelezioneTipoConsegna = 182,

        [Description("Selezione Struttura")]
        SelezioneUAConsegna = 183,

        [Description("Consegne Paziente")]
        ConsegnePaziente = 20,
        [Description("Editing Consegne Paziente")]
        EditingConsegnaPaziente = 201,
        [Description("Selezione Tipo Consegna Paziente")]
        SelezioneTipoConsegnaPaziente = 202,
        [Description("Selezione Struttura Consegna Paziente")]
        SelezioneUAConsegnaPaziente = 203,

        [Description("Consegne Paziente Cartella")]
        ConsegnePazienteCartella = 204,

        [Description("Pre-Trasferimento")]
        PreTrasferimento_RicercaPaziente = 14,
        [Description("Selezione Unità Atomica di Competenza - Pre-Trasferimento")]
        PreTrasferimento_SelezioneUAUO = 140,

        [Description("Selezione Unità Atomica di Competenza - Ambulatoriale")]
        Ambulatoriale_SelezioneUA = 70,
        [Description("Ricerca Paziente Ambulatoriale")]
        Ambulatoriale_RicercaPaziente = 7,
        [Description("Cartella Ambulatoriale")]
        Ambulatoriale_Cartella = 71,
        [Description("Evidenza Clinica")]
        Ambulatoriale_EvidenzaClinica = 7117,
        [Description("Allegati")]
        Ambulatoriale_Allegati = 71110,
        [Description("Schede")]
        Ambulatoriale_Schede = 7111,

        [Description("Percorso Ambulatoriale")]
        PercorsoAmbulatoriale_RicercaPaziente = 72,

        [Description("Scheda")]
        Ambulatoriale_Scheda = 711111,
        [Description("Selezione Tipo Scheda")]
        Ambulatoriale_SelezioneTipoScheda = 7111112,

        [Description("EBM (Evidence Based Medicine)")]
        Ambulatoriale_EBM = 71111,

        [Description("Gestione Ordini Ambulatoriali")]
        Ambulatoriale_GestioneOrdini = 7118,
        [Description("Selezione Unità Operativa su Ordine Ambulatoriale")]
        Ambulatoriale_SelezioneUOOrdini = 71181,
        [Description("Nuovo Ordine Edit Ordine Ambulatoriale")]
        Ambulatoriale_EditingOrdine = 711811,
        [Description("Nuovo Ordine Dati Aggiuntivi Ambulatoriali")]
        Ambulatoriale_EditingOrdineDatiAggiuntivi = 711812,
        [Description("Visualizza Ordine Ambulatoriale")]
        Ambulatoriale_VisualizzaOrdine = 71182,

        [Description("Modifica Ordini Dati Aggiuntivi Ambulatoriali per Trasfusionale")]
        Ambulatoriale_EditingOrdineDatiAggiuntiviTrasfusionale = 711813,

        [Description("Agende Paziente")]
        Ambulatoriale_AgendePaziente = 7116,
        [Description("Selezione Tipo Appuntamento")]
        Ambulatoriale_SelezioneTipoAppuntamento = 71160,
        [Description("Selezione Agende Appuntamento")]
        Ambulatoriale_SelezioneAgendeAppuntamento = 71161,
        [Description("Selezione Stato Appuntamento")]
        Ambulatoriale_SelezioneStatoAppuntamento = 71162,
        [Description("Selezione Appuntamento")]
        Ambulatoriale_SelezioneAppuntamento = 711611,

        [Description("Refertazione")]
        Ambulatoriale_Mow = 7113,

        [Description("Ricerca Paziente MatHome")]
        MatHome_RicercaPaziente = 80,
        [Description("Gestione Account")]
        MatHome_GestioneAccount = 801,

        [Description("Testi Predefiniti")]
        TestiPredefiniti = -102,

        [Description("Dati Anagrafici Paziente")]
        DatiAnagraficiPaziente = -103,

        [Description("Dati Episodio")]
        DatiEpisodio = -104,

        [Description("Tracker")]
        Tracker = -1041,

        [Description("Note Anamnestiche - Alert Allergie")]
        NoteAnamnesticheAlertAllergie = -105,
        [Description("Selezione Tipo Note Anamnestiche - Alert Allergie")]
        SelezioneTipoNoteAnamnesticheAlertAllergie = -1050,
        [Description("Edit Note Anamnestiche - Alert Allergie")]
        EditingNoteAnamnesticheAlertAllergie = -1051,

        [Description("Alert Generici")]
        AlertGenerici = -106,
        [Description("Selezione Tipo Alert Generico")]
        SelezioneTipoAlertGenerico = -1060,
        [Description("Edit Alert Generico")]
        EditingAlertGenerico = -1061,

        [Description("Acquisizione Immagine Paziente")]
        AcquisizioneImmaginePaziente = -108,

        [Description("Seleziona Report")]
        SelezionaReport = -1010,
        [Description("Report")]
        Report = -1011,
        [Description("Web Browser")]
        WebBrowser = -1012,
        [Description("Storico Report")]
        StoricoReport = -1013,

        [Description("Richiesta Consenso")]
        RichiestaConsenso = -1014,

        [Description("Uscita")]
        Uscita = 99,

        [Description("Selezione Unità Atomica di Competenza - Consegne ")]
        Consegne_SelezioneUA = 1830,

        [Description("Consulenze Trasversali")]
        ConsulenzeTrasversali = 19,

    }

    public enum EnumImmagineTop
    {

        Utente = 1,
        VociDiarioClinico = 2,
        Paziente = 3,
        Allergie = 4,
        Alert = 5,
        EvidenzaClinica = 6,
        Connettivita = 7,
        InfoPaziente = 8,
        InfoEpisodio = 9,
        Refresh = 10,
        Segnalibri = 11,
        SegnalibroAdd = 12,
        CartelleInVisione = 13,
        PazientiSeguiti = 14,
        Distacco = 15,
        Consensi = 16,
        Consegne = 17,
        Help = 18

    }

    public enum EnumImmagineBottom
    {

        Home = 1,
        ElencoPazienti = 2,
        CartellaPaziente = 3,
        Stampe = 4,
        CartelleChiuse = 5

    }

    public enum EnumPulsanteBottom
    {

        Indietro = 1,
        Avanti = 2

    }

    public enum EnumPulsante
    {

        PulsanteIndietroBottom = 0,
        PulsanteAvantiBottom = 1,
        PulsanteAvantiMenuBottom = 2,
        PulsanteHomeBottom = 3,
        PulsanteElencoPazientiBottom = 4,
        PulsanteCartellaPazienteBottom = 5,
        PulsanteStampeBottom = 6,
        PulsanteChiusuraCartelleBottom = 16,

        PulsanteUtenteTop = 7,
        PulsanteVociDiarioClinicoTop = 8,
        PulsantePazienteTop = 9,
        PulsanteAllergieTop = 10,
        PulsanteAlertTop = 11,
        PulsanteEvidenzaClinicaTop = 12,
        PulsanteConnettivitaTop = 13,
        PulsanteInfoPazienteTop = 14,
        PulsanteInfoEpisodioTop = 15,
        PulsanteConsensiTop = 17,
        PulsanteConsegneTop = 18

    }

    public enum EnumPulsanteSegnalibri
    {
        Modifica = 1
    }

    public enum EnumPulsanteCartelleInVisione
    {
        Nuovo = 0,
        Modifica = 1,
        Cancella = 2
    }

    public enum EnumStatoCartelleInVisione
    {
        [Description("Cancellata")]
        CA = 10,
        [Description("Attiva")]
        IC = 20,
        [Description("Scaduta")]
        SS = 30
    }

    public enum EnumStatoPazientiInVisione
    {
        [Description("Cancellata")]
        CA = 10,
        [Description("Attiva")]
        IC = 20,
        [Description("Scaduta")]
        SS = 30
    }

    public enum EnumStatoPazientiSeguiti
    {
        [Description("Cancellata")]
        CA = 10,
        [Description("Attiva")]
        IC = 20,
    }

    public enum EnumPulsantePazientiSeguiti
    {
        Nuovo = 0,
        Cancella = 2
    }

    public enum EnumTipoFormattazione
    {
        Grassetto = 0,
        Sottolineato = 1,
        Italico = 2,
        Barrato = 3,
        GrasItalico = 4
    }

    public enum enum_app_cursors
    {
        DefaultCursor = 0,
        WaitCursor = 1
    }

    public enum EnumLock
    {
        [Description("Lock")]
        LOCK = 10,
        [Description("Unlock")]
        UNLOCK = 20,
        [Description("Informazioni")]
        INFO = 30,
        [Description("UnlockAll")]
        UNLOCKALL = 40,
        [Description("Successiva")]
        SUCCESSIVA = 50,
        [Description("Precedente")]
        PRECEDENTE = 60
    }

    public enum EnumTipoRegistrazione
    {
        [Description("Regitrazione Manuale")]
        M = 10,
        [Description("Regitrazione Automatica")]
        A = 20
    }

    public enum EnumCodSistema
    {
        [Description("Order Entry")]
        OE = 10,
        [Description("Prescrizione da Cartella")]
        PRF = 20,
        [Description("Prescrizione PSC")]
        PSC = 30,
        [Description("Worklist da Cartella")]
        WKI = 40,
        [Description("Worklist da Appuntamento")]
        APP = 50,
        [Description("Worklist da Scheda")]
        SCH = 60,
        [Description("Prescrizione Tempo")]
        PRT = 70

    }

    public enum EnumStatoEpisodio
    {
        [Description("Annullato")]
        AN = 10,
        [Description("Cancellato")]
        CA = 20,
        [Description("Attivo")]
        AT = 30,
        [Description("Dimesso")]
        DM = 40
    }

    public enum EnumStatoTaskInfermieristico
    {
        [Description("Annullato")]
        AN = 10,
        [Description("Cancellato")]
        CA = 20,
        [Description("Eseguito")]
        ER = 30,
        [Description("Pianificato")]
        PR = 40,
        [Description("In Corso")]
        IC = 50,
        [Description("Trascritto")]
        TR = 60
    }

    public enum EnumStatoTrasferimento
    {
        [Description("Dimesso")]
        DM = 10,
        [Description("Trasferito")]
        TR = 40,
        [Description("Attivo")]
        AT = 50,
        [Description("Sospeso")]
        SS = 60,
        [Description("Cancellato")]
        CA = 70,
        [Description("Prenotato")]
        PR = 80,
        [Description("Prenotazione Chiusa")]
        PC = 90,
        [Description("PreTrasferimento")]
        PT = 100,
        [Description("Prenotazione Annullata")]
        PA = 110
    }

    public enum EnumStatoCartella
    {
        [Description("Aperta")]
        AP = 10,
        [Description("Chiusa")]
        CH = 20,
        [Description("Da Aprire")]
        DA = 30,
        [Description("Cancellata")]
        CA = 40
    }

    public enum EnumStatoCartellaInfo
    {
        [Description("Riapertura")]
        RCA = 10
    }

    public enum EnumStatoScheda
    {
        [Description("Annullata")]
        AN = 10,
        [Description("Cancellata")]
        CA = 20,
        [Description("Chiusa")]
        CH = 30,
        [Description("Valida")]
        IC = 40
    }

    public enum EnumStatoAppuntamento
    {
        [Description("Annullato")]
        AN = 10,
        [Description("Cancellato")]
        CA = 20,
        [Description("Erogato")]
        ER = 30,
        [Description("In Corso")]
        IC = 40,
        [Description("Pronto")]
        PR = 50,
        [Description("Sospeso")]
        SS = 60,
        [Description("Trascritto")]
        TR = 70,
        [Description("Da Pianificare")]
        DP = 80
    }

    public enum EnumStatoAppuntamentoAgenda
    {
        [Description("Cancellato")]
        CA = 20,
        [Description("Pronto")]
        PR = 40
    }

    public enum EnumStatoNotaAgenda
    {
        [Description("Cancellata")]
        CA = 20,
        [Description("Inserita")]
        PR = 40
    }

    public enum EnumStatoNota
    {
        [Description("Cancellata")]
        CA = 20,
        [Description("Inserita")]
        PR = 40
    }

    public enum EnumStatoPrescrizione
    {
        [Description("Cancellato")]
        CA = 10,
        [Description("Terminata")]
        ER = 20,
        [Description("Attiva")]
        IC = 30,
        [Description("Sospesa")]
        SS = 40,
        [Description("Validata")]
        VA = 50
    }

    public enum EnumStatoContinuazione
    {
        [Description("Apri")]
        AP = 10,
        [Description("Chiudi")]
        CH = 20
    }

    public enum EnumStatoPrescrizioneTempi
    {
        [Description("Cancellato")]
        CA = 10,
        [Description("Terminata")]
        ER = 20,
        [Description("Attiva")]
        IC = 30,
        [Description("Sospesa")]
        SS = 40,
        [Description("Validata")]
        VA = 50
    }

    public enum EnumTipoPrescrizioneTempi
    {
        [Description("Singola")]
        SN = 10,
        [Description("Ripetuta")]
        RP = 20,
        [Description("Continua")]
        CN = 30,
        [Description("Protocollo Orario")]
        PO = 40,
        [Description("Protocollo Orario Continuo")]
        POC = 45,
        [Description("Protocollo Giornaliero")]
        PG = 50,
        [Description("Al Bisogno")]
        AB = 60
    }

    public enum EnumTipoTaskInfermieristicoTempi
    {
        [Description("Singola")]
        SN = 10,
        [Description("Ripetuta")]
        RP = 20,
        [Description("Protocollo Orario")]
        PO = 40,
        [Description("Protocollo Giornaliero")]
        PG = 50
    }

    public enum EnumStatoParametroVitale
    {
        [Description("Annulato")]
        AN = 10,
        [Description("Rilevato")]
        ER = 20,
        [Description("Cancellato")]
        CA = 30
    }

    public enum EnumSistemi
    {
        [Description("Order Entry")]
        OE = 10,
        [Description("Prescrizione da Cartella")]
        PRF = 20,
        [Description("Prescrizione PSC")]
        PSC = 30,
        [Description("Worklist da Cartella")]
        WKI = 40,
        [Description("Prescrizione Tempo")]
        PRT = 50,
        [Description("Appuntamenti")]
        APP = 60
    }

    public enum EnumXMLEncoding
    {
        UTF_7 = 7,
        UTF_8 = 8,
        UTF_16 = 16,
        UTF_32 = 32
    }

    public enum EnumFormatoReport
    {
        [Description("Cablato")]
        CAB = 10,
        [Description("Formato PDF")]
        PDF = 20,
        [Description("Report Manager")]
        REM = 30,
        [Description("Formato Word 2007/2010")]
        WORD = 40
    }

    public enum EnumCodTipoEvidenzaClinica
    {
        [Description("Defualt (non cancellare)")]
        _DEFAULT_ = 1,
        [Description("Anatomia Patologica")]
        AP = 2,
        [Description("Cardiologia (Coronarografia)")]
        AV1 = 3,
        [Description("Referti Ambulatoriali")]
        EIM = 4,
        [Description("EIM-RO")]
        EIM_RO = 5,
        [Description("EIM-SP")]
        EIM_SP = 6,
        [Description("Endoscopia Digestiva o Pneumologia")]
        ESD = 7,
        [Description("Laboratorio")]
        LAB = 8,
        [Description("Lettera di Dimissione")]
        LD1 = 9,
        [Description("Lettera di Dimissione Cardiologia")]
        LD2 = 10,
        [Description("Medicina Nucleare")]
        MN = 11,
        [Description("Oncologia")]
        ONC = 12,
        [Description("Cardiologia")]
        PRG_CAR = 13,
        [Description("CC Neurologia")]
        PRG_NEU = 14,
        [Description("Pronto Soccorso")]
        PS = 15,
        [Description("Radioterapia")]
        RT = 16,
        [Description("Radiologia")]
        RX = 17,
        [Description("SDO")]
        SDO = 18,
        [Description("Anatomia Patologica")]
        SINFO = 19,
        [Description("Trasfusionale")]
        SIT = 20,
        [Description("Sala Operatoria")]
        SO = 21
    }

    public enum EnumStatoOrdine
    {
        [Description("Cancellato")]
        CA = 10,
        [Description("Inoltrato")]
        VA = 20,
        [Description("Accettato")]
        AC = 30,
        [Description("Annullato")]
        AN = 40,
        [Description("Erogato")]
        ER = 50,
        [Description("Errato")]
        ET = 60,
        [Description("InCarico")]
        IC = 70,
        [Description("Inserito")]
        IS = 80,
        [Description("Programmato")]
        PR = 90,
        [Description("Sconosciuto")]
        NN = 100,
        [Description("Rifiutato")]
        RI = 110,
        [Description("Stato Esteso")]
        SE = 120,
    }

    public enum EnumUAModuli
    {
        [Description("Firma Digitale Chiusura Cartella")]
        FirmaD_ChCartella = 0,
        [Description("Firma Digitale Diario Clinico")]
        FirmaD_Diario = 1,
        [Description("Firma Digitale Prescrizioni")]
        FirmaD_Prescrizioni = 2,
        [Description("Firma Digitale Allegati")]
        FirmaD_Allegati = 3

    }

    public enum EnumTipoDocumentoFirmato
    {
        [Description("Cartella Clinica Firmata")]
        CARFM01 = 10,
        [Description("Diario Clinico Firmato")]
        DCLFM01 = 20,
        [Description("Prescrizione Tempi Firmato")]
        PRTFM01 = 30,
        [Description("Allegato Firmato")]
        ALLFM01 = 40,
        [Description("Scheda Firmata")]
        SCHFM01 = 50
    }

    public enum EnumTipoDatoAggiuntivo
    {
        Undefined = 0,
        ComboBox = 10,
        DateBox = 20,
        DateTimeBox = 30,
        FloatBox = 40,
        ListBox = 50,
        ListMultiBox = 60,
        NumberBox = 70,
        TextBox = 80,
        TimeBox = 90,
        Tempi = 100,
        Titolo = 110
    }

    public enum OEPrestazioneTipo
    {
        [Description("Non Definito")]
        NN = 10,
        [Description("Prestazione")]
        Prestazione = 20,
        [Description("Profilo")]
        Profilo = 30,
        [Description("Profilo Scomponibile")]
        ProfiloScomponibile = 40,
        [Description("Profilo Utente")]
        ProfiloUtente = 50
    }

    public enum OEPrioritaOrdine
    {
        [Description("Non Definita")]
        NN = -1,
        [Description("Programmata")]
        P = 0,
        [Description("Ordinaria")]
        O = 1,
        [Description("Urgente Differibile")]
        UD = 2,
        [Description("Urgente")]
        U = 3,
        [Description("Urgente 2H")]
        U2 = 4,
    }

    public enum OEStato
    {
        NN = -1,
        Inserito = 10,
        Inoltrato = 20,
        InCarico = 30,
        Programmato = 40,
        Erogato = 50,
        Cancellato = 60,
        Errato = 70,
        Accettato = 80,
        Annullato = 90
    }

    public enum OEValiditaOrdine
    {
        [Description("Non Definito")]
        NN = -1,
        [Description("Valido")]
        Valido = 10,
        [Description("Non Valido")]
        NonValido = 20
    }

    public enum OEStatoRichiedente
    {
        NN = -1,
        Inserita = 10,
        Cancellata = 20,
        Modificata = 30
    }

    public enum OEStatoErogante
    {
        [Description("Non Definita")]
        NN = -1,
        [Description("Cancellata")]
        Cancellata = 10,
        [Description("Erogata")]
        Erogata = 20,
        [Description("In Corso")]
        InCorso = 30,
        [Description("Programmata")]
        Programmata = 40
    }

    public enum EnumTipoProtocollo
    {
        [Description("Delta")]
        DELTA = 10,
        [Description("Ora")]
        ORA = 20
    }

    public enum EnumTipoSalvataggioScheda
    {
        [Description("No")]
        N = 10,
        [Description("Locale")]
        L = 20,
        [Description("Rete")]
        R = 30
    }

    public enum EnumTipoContenutiReferto
    {
        [Description("Testo semplice")]
        Testo = 10,
        [Description("Rich Text Format")]
        RTF = 20
    }

    public enum EnumTipoAllegatoScheda
    {
        [Description("Immagine")]
        IMG = 10
    }

    public enum EnumStatoAllegatoScheda
    {
        [Description("Cancellato")]
        CA = 10,
        [Description("Inserito")]
        IC = 20
    }

    public enum EnumTipoRichiestaAllegatoScheda
    {
        [Description("Lista")]
        LISTA = 10,
        [Description("Thumb")]
        THUMB = 20,
        [Description("Doc")]
        DOC = 30
    }

    public enum EnumStatoCoda
    {
        [Description("Assegnato Numero")]
        AS = 10,
        [Description("Cancellato")]
        CA = 20,
        [Description("Chiamato Numero")]
        CH = 30
    }

    public enum EnumStatoEvidenzaClinica
    {
        [Description("Annullato")]
        AN = 10,
        [Description("Cancellato")]
        CA = 20,
        [Description("Completato")]
        CM = 30,
        [Description("In Corso")]
        IC = 40
    }

    public enum EnumStatoEvidenzaClinicaVisione
    {
        [Description("Non Vistabile")]
        DV = 10,
        [Description("Non Visionabie")]
        NV = 20,
        [Description("Vistato")]
        VS = 30
    }

    public enum EnumCommandListener
    {
        [Description("Apri Maschera")]
        ApriMaschera = 10,
        [Description("Chiudi tutte le maschere")]
        ChiudiMaschere = 20,
        [Description("Chiudi Listener")]
        ChiudiListener = 30
    }

    public enum EnumTipoSelezione
    {
        [Description("Grafici PVT e LAB")]
        GRAF = 10
    }

    public enum EnumTipoNews
    {
        [Description("Standard")]
        STD = 10,
        [Description("Push Lite")]
        LITE = 20,
        [Description("Push Hard")]
        HARD = 30
    }

    public enum EnumTipoFiltroSpeciale
    {
        [Description("Ricerca Pazienti percorso Cartella")]
        PAZCAR = 10,
        [Description("Ricerca Pazienti percorso Ambulatoriale")]
        PAZAMB = 20,
        [Description("Worklist Trasversale")]
        WKITRA = 30,
        [Description("Evidenza Clinica Trasversale")]
        EVCTRA = 40,
        [Description("Taglia code - Assegnazione")]
        TCDASS = 50,
        [Description("Taglia code - Chiamata")]
        TCDCHI = 60,
        [Description("Cartella Paziente")]
        EPICAR = 70,
        [Description("Cartella Paziente Ambulatoriale")]
        AMBCAR = 80,
        [Description("Ricerca Paziente Totem")]
        TOTEMASS = 90,
        [Description("Order Entry Trasversale")]
        OETRA = 100,
        [Description("Consegne Pazienti Trasversale")]
        CSPTRA = 110
    }

    public enum EnumTipoAzienda
    {
        [Description("Azienda ASMN")]
        ASMN = 0,
        [Description("Azienda AUSL")]
        AUSL = 1
    }

    public enum EnumTipoConsenso
    {
        [Description("Consenso Generico")]
        Generico = 0,
        [Description("Consenso Dossier")]
        Dossier = 1,
        [Description("Consenso Dossier Storico")]
        DossierStorico = 2
    }

    public enum EnumStatoConsenso
    {
        [Description("Non dichiarato")]
        ND = 10,
        [Description("Consenso negato")]
        NO = 20,
        [Description("Consenso acconsentito")]
        SI = 30
    }

    public enum EnumStatoConsensoCalcolato
    {
        [Description("Nessuno")]
        ND = 10,
        [Description("Solo Generico")]
        GN = 20,
        [Description("Generico e Dossier")]
        DO = 30,
        [Description("Generico, Dossier e Dossier Storico")]
        DS = 40,
        [Description("Negato")]
        NO = 50
    }

    public enum EnumTipoIntestazione
    {
        [Description("Firma Cartella")]
        CARFIRMA = 0,
        [Description("Cartella Sintetica")]
        CARTSINT = 1,
        [Description("Cartella")]
        CARTSTD = 2,
        [Description("Spalla Sinistra - Stampa Schede")]
        SPALLASX = 3
    }

    public enum EnumStatoConsegna
    {
        [Description("Annullato")]
        AN = 10,
        [Description("Cancellato")]
        CA = 20,
        [Description("Inserto")]
        IC = 30

    }

    public enum EnumTipoRaggruppamentoAgenda
    {
        [Description("Nessuno")]
        Nessuno = 10,
        [Description("Campo")]
        Campo = 20,
        [Description("Dizionario")]
        Dizionario = 30,
        [Description("Scheda")]
        Scheda = 40
    }

    public enum EnumCommandLineModules
    {
        [Description("Sezione principale Cartella Paziente/Ambulatoriale")]
        Cartella = 10,

        [Description("Schede")]
        Schede = 20,
    }

    public enum EnumUnitaScadenza
    {

        [Description("Scadenza Giornaliera")]
        G_1 = 10,

        [Description("Scadenza Annuale")]
        A_1 = 20,

    }

    public enum EnumAttributiScheda
    {
        [Description("Gestione Layer per DWH")]
        LayerDWH = 10,
        [Description("Descrizione Layer per DWH")]
        LayerDescrizioneDWH = 20,
    }

}
