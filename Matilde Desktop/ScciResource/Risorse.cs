using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Media;

namespace UnicodeSrl.ScciResource
{
    public partial class Risorse
    {

        #region Dichiarazioni

        public const string GC_LOGO = @"Logo";
        public const string GC_SCCI = @"Hospital_256";
        public const string GC_SCCIMANAGEMENT = @"Hospital_256";
        public const string GC_SCCIMANAGEMENTSPLASH = @"SplashManagement";
        public const string GC_SCCIEASY = @"Matilde_128";
        public const string GC_UPDATER_128 = @"Updater_128";

        public const string GC_HOME_256 = @"Home_256";

        public const string GC_CURSORWAIT = @"Wait";

        public const string GC_WAIT_16 = @"Wait_16";
        public const string GC_WAIT_32 = @"Wait_32";
        public const string GC_WAIT_256 = @"Wait_256";

        public const string GC_AMMINISTRAZIONE_16 = @"Administration_16";
        public const string GC_AMMINISTRAZIONE_32 = @"Administration_32";
        public const string GC_AMMINISTRAZIONE_256 = @"Administration_256";

        public const string GC_ESCI_16 = @"Log_Out_16";
        public const string GC_ESCI_32 = @"Log_Out_32";
        public const string GC_ESCI_256 = @"Log_Out_256";

        public const string GC_CLOSE_ALL_16 = @"Close_All_16";
        public const string GC_CLOSE_ALL_32 = @"Close_All_32";
        public const string GC_CLOSE_ALL_256 = @"Close_All_256";

        public const string GC_NUOVO_16 = "Add_16";
        public const string GC_NUOVO_32 = "Add_32";
        public const string GC_NUOVO_256 = "Add_256";

        public const string GC_MODIFICA_16 = "Modifica_16";
        public const string GC_MODIFICA_32 = "Modifica_32";
        public const string GC_MODIFICA_256 = "Modifica_256";
        public const string GC_MODIFICACHECK_256 = "ModificaCheck_256";

        public const string GC_MODIFICAVERSIONE_16 = "ModificaVersione_16";
        public const string GC_MODIFICAVERSIONE_32 = "ModificaVersione_32";
        public const string GC_MODIFICAVERSIONE_256 = "ModificaVersione_256";

        public const string GC_EBM_16 = "EBM_16";
        public const string GC_EBM_32 = "EBM_32";
        public const string GC_EBM_256 = "EBM_256";

        public const string GC_ELIMINA_16 = "Delete_16";
        public const string GC_ELIMINA_32 = "Delete_32";
        public const string GC_ELIMINA_256 = "Delete_256";

        public const string GC_VISUALIZZA_16 = "View_16";
        public const string GC_VISUALIZZA_32 = "View_32";
        public const string GC_VISUALIZZA_256 = "View_256";

        public const string GC_STAMPA_16 = "Printer_16";
        public const string GC_STAMPA_32 = "Printer_32";
        public const string GC_STAMPA_256 = "Printer_256";

        public const string GC_AGGIORNA_16 = "Refresh_16";
        public const string GC_AGGIORNA_32 = "Refresh_32";
        public const string GC_AGGIORNA_256 = "Refresh_256";

        public const string GC_ESPORTA_16 = "Export_16";
        public const string GC_ESPORTA_32 = "Export_32";
        public const string GC_ESPORTA_256 = "Export_256";

        public const string GC_IMPORTA_16 = "Import_16";
        public const string GC_IMPORTA_32 = "Import_32";
        public const string GC_IMPORTA_256 = "Import_256";

        public const string GC_GENERA_16 = "Genera_16";
        public const string GC_GENERA_32 = "Genera_32";
        public const string GC_GENERA_256 = "Genera_256";

        public const string GC_FILTRO_16 = "Filtro_16";
        public const string GC_FILTRO_32 = "Filtro_32";
        public const string GC_FILTRO_256 = "Filtro_256";

        public const string GC_FILTROAPPLICATO_16 = "FiltroApplicato_16";
        public const string GC_FILTROAPPLICATO_32 = "FiltroApplicato_32";
        public const string GC_FILTROAPPLICATO_256 = "FiltroApplicato_256";

        public const string GC_FILTROCANCELLA_256 = "FiltroCancella_256";

        public const string GC_FOLDER_16 = "Folder_16";
        public const string GC_FOLDER_32 = "Folder_32";
        public const string GC_FOLDER_256 = "Folder_256";
        public const string GC_FOLDERAGGIUNGI_256 = "FolderAggiungi_256";

        public const string GC_FOLDERCARTELLA_16 = "FolderCartella_16";
        public const string GC_FOLDERCARTELLA_32 = "FolderCartella_32";
        public const string GC_FOLDERCARTELLA_256 = "FolderCartella_256";

        public const string GC_FOLDERPAZIENTE_16 = "FolderPaziente_16";
        public const string GC_FOLDERPAZIENTE_32 = "FolderPaziente_32";
        public const string GC_FOLDERPAZIENTE_256 = "FolderPaziente_256";

        public const string GC_SYMBOL_CHECK_16 = "Symbol Check_16";
        public const string GC_SYMBOL_RESTRICTED_16 = "Symbol Restricted_16";

        public const string GC_NO_16 = @"No_16";
        public const string GC_NO_32 = @"No_32";
        public const string GC_NO_256 = @"No_256";

        public const string GC_SI_16 = @"Si_16";
        public const string GC_SI_32 = @"Si_32";
        public const string GC_SI_256 = @"Si_256";

        public const string GC_FRECCIADX_16 = "FrecciaDx_16";
        public const string GC_FRECCIADX_32 = "FrecciaDx_32";
        public const string GC_FRECCIADX_256 = "FrecciaDx_256";

        public const string GC_FRECCIASX_16 = "FrecciaSx_16";
        public const string GC_FRECCIASX_32 = "FrecciaSx_32";
        public const string GC_FRECCIASX_256 = "FrecciaSx_256";

        public const string GC_FRECCIASU_48 = "Navigation 2 Up";
        public const string GC_FRECCIAGIU_48 = "Navigation 2 Down";

        public const string GC_MENUPOPUP_32 = "MenuPopUp_32";

        public const string GC_STATUSFLAGRED_16 = "StatusFlagRed_16";
        public const string GC_STATUSFLAGRED_32 = "StatusFlagRed_32";
        public const string GC_STATUSFLAGRED_256 = "StatusFlagRed_256";

        public const string GC_FORMATOPDF_16 = "FormatoPDF_16";
        public const string GC_FORMATOPDF_32 = "FormatoPDF_32";
        public const string GC_FORMATOPDF_256 = "FormatoPDF_256";

        public const string GC_TESSERA_256 = "Tessera_256";
        public const string GC_TESSERAATTESA_256 = "TesseraAttesa_256";
        public const string GC_TESSERAERRORE_256 = "TesseraErrore_256";
        public const string GC_TESSERAMANCANTE_256 = "TesseraMancante_256";
        public const string GC_TESSERAOK_256 = "TesseraOk_256";

        public const string GC_TESSERAFIRMA_32 = "TesseraFirma_32";
        public const string GC_TESSERAFIRMA_256 = "TesseraFirma_256";
        public const string GC_TESSERAFIRMATUTTI_256 = "TesseraFirmaTutti_256";
        public const string GC_TESSERAFIRMATUTTIRAP_256 = "TesseraFirmaTuttiRAP_256";
        public const string GC_FIRMAMULTIPLAFILTRO_256 = "FirmaMultiplaFiltro_256";
        public const string GC_FIRMAMULTIPLAFILTROSELEZIONATO_256 = "FirmaMultiplaFiltroSelezionato_256";

        public const string GC_FIRMAMULTIPLAFILTROINFO_256 = "FirmaMultiplaFiltroRAP_256";
        public const string GC_FIRMAMULTIPLAFILTROINFOSELEZIONATO_256 = "FirmaMultiplaFiltroSelezionatoRAP_256";

        public const string GC_TESSERAFIRMAESEGUITA_32 = "FirmaEseguitaTessera_32";
        public const string GC_TESSERAFIRMAESEGUITA_256 = "FirmaEseguitaTessera_256";

        public const string GC_AGENDE_1 = "Agende_1";
        public const string GC_AGENDE_2 = "Agende_2";
        public const string GC_AGENDE_3 = "Agende_3";
        public const string GC_AGENDE_4 = "Agende_4";
        public const string GC_AGENDE_6 = "Agende_6";

        public const string GC_AGENDESINCRONIZZA_256 = "AgendeSincronizza_256";

        public const string GC_EROGAZIONERAPIDA_32 = @"ErogazioneRapida_32";

        public const string GC_APPUNTAMENTOAGGIUNGI = "AppuntamentoAggiungi_256";
        public const string GC_APPUNTAMENTOANNULLA = "AppuntamentoAnnulla_256";
        public const string GC_APPUNTAMENTOCANCELLA = "AppuntamentoCancella_256";
        public const string GC_APPUNTAMENTOMODIFICA = "AppuntamentoModifica_256";

        public const string GC_AGGIUNGICONTINUA_256 = "Aggiungi_Continua_256";
        public const string GC_AGGIUNGISINGOLA_256 = "Aggiungi_Singola_256";
        public const string GC_AGGIUNGISOMMINISTRAZIONE_256 = "Aggiungi_Somministrazione_256";
        public const string GC_AGGIUNGITERAPIA_256 = "Aggiungi_Terapia_256";
        public const string GC_AGGIUNGITERAPIARAPIDA_256 = "Aggiungi_TerapiaRapida_256";

        public const string GC_NOTAAGGIUNGI = "NotaAggiungi_256";
        public const string GC_NOTACANCELLA = "NotaCancella_256";
        public const string GC_NOTAMODIFICA = "NotaModifica_256";

        public const string GC_CONFIGURAZIONE_16 = @"Configuration_16";
        public const string GC_CONFIGURAZIONE_32 = @"Configuration_32";
        public const string GC_CONFIGURAZIONE_256 = @"Configuration_256";

        public const string GC_CONFIGURAZIONE_TABLE_16 = @"Configuration_Tools_16";
        public const string GC_CONFIGURAZIONE_TABLE_32 = @"Configuration_Tools_32";
        public const string GC_CONFIGURAZIONE_TABLE_256 = @"Configuration_Tools_256";

        public const string GC_CONTATORI_16 = @"Contatori_16";
        public const string GC_CONTATORI_32 = @"Contatori_32";
        public const string GC_CONTATORI_256 = @"Contatori_256";

        public const string GC_FILTRISPECIALI_16 = @"FiltriSpeciali_16";
        public const string GC_FILTRISPECIALI_32 = @"FiltriSpeciali_32";
        public const string GC_FILTRISPECIALI_256 = @"FiltriSpeciali_256";

        public const string GC_INPROGRESS_16 = @"InProgress_16";
        public const string GC_INPROGRESS_32 = @"InProgress_32";
        public const string GC_INPROGRESS_256 = @"InProgress_256";

        public const string GC_LOGIN_16 = @"Login_16";
        public const string GC_LOGIN_32 = @"Login_32";
        public const string GC_LOGIN_256 = @"Login_256";

        public const string GC_MODULI_16 = @"Modulo_16";
        public const string GC_MODULI_32 = @"Modulo_32";
        public const string GC_MODULI_256 = @"Modulo_256";

        public const string GC_RUOLI_16 = @"Ruolo_16";
        public const string GC_RUOLI_32 = @"Ruolo_32";
        public const string GC_RUOLI_256 = @"Ruolo_256";

        public const string GC_RUOLIAGGIUNGI_16 = @"RuoloAggiungi_16";
        public const string GC_RUOLIAGGIUNGI_32 = @"RuoloAggiungi_32";
        public const string GC_RUOLIAGGIUNGI_256 = @"RuoloAggiungi_256";

        public const string GC_RUOLICANCELLA_16 = @"RuoloCancella_16";
        public const string GC_RUOLICANCELLA_32 = @"RuoloCancella_32";
        public const string GC_RUOLICANCELLA_256 = @"RuoloCancella_256";

        public const string GC_AZIENDE_16 = @"Azienda_16";
        public const string GC_AZIENDE_32 = @"Azienda_32";
        public const string GC_AZIENDE_256 = @"Azienda_256";

        public const string GC_UNITAATOMICHE_16 = @"UnitaAtomica_16";
        public const string GC_UNITAATOMICHE_32 = @"UnitaAtomica_32";
        public const string GC_UNITAATOMICHE_256 = @"UnitaAtomica_256";

        public const string GC_DIARIOMEDICO_16 = @"Diario_Clinico_16";
        public const string GC_DIARIOMEDICO_32 = @"Diario_Clinico_32";
        public const string GC_DIARIOMEDICO_256 = @"Diario_Clinico_256";

        public const string GC_DIARIOINFERMIERISTICO_16 = @"Diario_Infermieristico_16";
        public const string GC_DIARIOINFERMIERISTICO_32 = @"Diario_Infermieristico_32";
        public const string GC_DIARIOINFERMIERISTICO_256 = @"Diario_Infermieristico_256";

        public const string GC_PARAMETRIVITALI_16 = @"ParametroVitale_16";
        public const string GC_PARAMETRIVITALI_32 = @"ParametroVitale_32";
        public const string GC_PARAMETRIVITALI_256 = @"ParametroVitale_256";

        public const string GC_EVIDENZACLINICA_16 = @"EvidenzaClinica_16";
        public const string GC_EVIDENZACLINICA_32 = @"EvidenzaClinica_32";
        public const string GC_EVIDENZACLINICA_256 = @"EvidenzaClinica_256";
        public const string GC_EVIDENZACLINICAIMPORTA_256 = @"EvidenzaClinicaImporta_256";
        public const string GC_EVIDENZACLINICAIMPORTA_32 = @"EvidenzaClinicaImporta_32";

        public const string GC_EVIDENZACLINICAALERT_256 = @"EvidenzaClinicaAlert_256";
        public const string GC_EVIDENZACLINICAALERTDISABLE_256 = @"EvidenzaClinicaAlertDisable_256";

        public const string GC_REPORT_16 = @"Report_16";
        public const string GC_REPORT_32 = @"Report_32";
        public const string GC_REPORT_256 = @"Report_256";

        public const string GC_REPORTHISTORY_128 = @"ReportHistory_128";

        public const string GC_TESTOPREDEFINITO_16 = @"TestoPredefinito_16";
        public const string GC_TESTOPREDEFINITO_32 = @"TestoPredefinito_32";
        public const string GC_TESTOPREDEFINITO_256 = @"TestoPredefinito_256";

        public const string GC_MASCHERA_16 = @"Maschera_16";
        public const string GC_MASCHERA_32 = @"Maschera_32";
        public const string GC_MASCHERA_256 = @"Maschera_256";

        public const string GC_NEWS_16 = @"News_16";
        public const string GC_NEWS_32 = @"News_32";
        public const string GC_NEWS_256 = @"News_256";

        public const string GC_ICONA_16 = @"Icona_16";
        public const string GC_ICONA_32 = @"Icona_32";
        public const string GC_ICONA_256 = @"Icona_256";

        public const string GC_DIFFERENZE_16 = @"Differenze_16";
        public const string GC_DIFFERENZE_32 = @"Differenze_32";
        public const string GC_DIFFERENZE_256 = @"Differenze_256";

        public const string GC_UNITAOPERATIVA_16 = @"UnitaOperativa_16";
        public const string GC_UNITAOPERATIVA_32 = @"UnitaOperativa_32";
        public const string GC_UNITAOPERATIVA_256 = @"UnitaOperativa_256";

        public const string GC_TASKINFERMIERISTICO_16 = @"TaskInfermieristico_16";
        public const string GC_TASKINFERMIERISTICO_32 = @"TaskInfermieristico_32";
        public const string GC_TASKINFERMIERISTICO_256 = @"TaskInfermieristico_256";

        public const string GC_TASKINFERMIERISTICO_ADD_16 = @"TaskInfermieristicoAggiungi_16";
        public const string GC_TASKINFERMIERISTICO_ADD_32 = @"TaskInfermieristicoAggiungi_32";
        public const string GC_TASKINFERMIERISTICO_ADD_256 = @"TaskInfermieristicoAggiungi_256";

        public const string GC_PRESCRIZIONE_16 = @"Prescrizione_16";
        public const string GC_PRESCRIZIONE_32 = @"Prescrizione_32";
        public const string GC_PRESCRIZIONE_256 = @"Prescrizione_256";

        public const string GC_COPIAPRESCRIZIONI_256 = @"CopiaPrescrizioni_256";

        public const string GC_STAMPACARTELLECHIUSE_256 = @"StampaCartelleChiuse_256";
        public const string GC_STAMPACARTELLA_256 = @"StampaCartella_256";
        public const string GC_STAMPACARTELLAREFERTI_256 = @"StampaCartellaReferti_256";

        public const string GC_VIASOMMINISTRAZIONE_16 = @"ViaDiSomministrazione_16";
        public const string GC_VIASOMMINISTRAZIONE_32 = @"ViaDiSomministrazione_32";
        public const string GC_VIASOMMINISTRAZIONE_256 = @"ViaDiSomministrazione_256";

        public const string GC_ALLEGATI_16 = @"Allegato_16";
        public const string GC_ALLEGATI_32 = @"Allegato_32";
        public const string GC_ALLEGATI_256 = @"Allegato_256";
        public const string GC_ALLEGATI_ACQ = @"Scanner";

        public const string GC_ALLEGATOELETTRONICO_AGGIUNGI_16 = @"AllegatoElettronico_Aggiungi_16";
        public const string GC_ALLEGATOELETTRONICO_AGGIUNGI_32 = @"AllegatoElettronico_Aggiungi_32";
        public const string GC_ALLEGATOELETTRONICO_AGGIUNGI_256 = @"AllegatoElettronico_Aggiungi_256";

        public const string GC_ALLEGATOELETTRONICO_IMPORTA_16 = @"AllegatoElettronicoImporta_16";
        public const string GC_ALLEGATOELETTRONICO_IMPORTA_32 = @"AllegatoElettronicoImporta_32";
        public const string GC_ALLEGATOELETTRONICO_IMPORTA_256 = @"AllegatoElettronicoImporta_256";

        public const string GC_ALLEGATOVIRTUALE_AGGIUNGI_16 = @"AllegatoVirtuale_Aggiungi_16";
        public const string GC_ALLEGATOVIRTUALE_AGGIUNGI_32 = @"AllegatoVirtuale_Aggiungi_32";
        public const string GC_ALLEGATOVIRTUALE_AGGIUNGI_256 = @"AllegatoVirtuale_Aggiungi_256";

        public const string GC_STANZA_16 = @"Stanza_16";
        public const string GC_STANZA_32 = @"Stanza_32";
        public const string GC_STANZA_256 = @"Stanza_256";

        public const string GC_APPUNTAMENTO_16 = @"Appuntamento_16";
        public const string GC_APPUNTAMENTO_32 = @"Appuntamento_32";
        public const string GC_APPUNTAMENTO_256 = @"Appuntamento_256";

        public const string GC_SCHEDA_16 = @"Scheda_16";
        public const string GC_SCHEDA_32 = @"Scheda_32";
        public const string GC_SCHEDA_256 = @"Scheda_256";

        public const string GC_SCHEDACARTELLAAMBULATORIALE_16 = @"SchedaCartellaAmbulatorialeAperta_16";
        public const string GC_SCHEDACARTELLAAMBULATORIALE_32 = @"SchedaCartellaAmbulatorialeAperta_32";
        public const string GC_SCHEDACARTELLAAMBULATORIALE_256 = @"SchedaCartellaAmbulatorialeAperta_256";

        public const string GC_SCHEDACARTELLAAMBULATORIALECHIUSA_32 = @"SchedaCartellaAmbulatorialeChiusa_32";
        public const string GC_SCHEDACARTELLAAMBULATORIALECHIUSAD_32 = @"SchedaCartellaAmbulatorialeChiusaD_32";

        public const string GC_EPISODIO_16 = @"Episodio_16";
        public const string GC_EPISODIO_32 = @"Episodio_32";
        public const string GC_EPISODIO_256 = @"Episodio_256";

        public const string GC_LETTO_16 = @"Letto_16";
        public const string GC_LETTO_32 = @"Letto_32";
        public const string GC_LETTO_256 = @"Letto_256";

        public const string GC_LETTOD_16 = @"LettoD_16";
        public const string GC_LETTOD_32 = @"LettoD_32";
        public const string GC_LETTOD_256 = @"LettoD_256";

        public const string GC_LETTODANNULLATO_16 = @"LettoDAnnullato_16";
        public const string GC_LETTODANNULLATO_32 = @"LettoDAnnullato_32";
        public const string GC_LETTODANNULLATO_256 = @"LettoDAnnullato_256";

        public const string GC_SETTORI_16 = @"Settori_16";
        public const string GC_SETTORI_32 = @"Settori_32";
        public const string GC_SETTORI_256 = @"Settori_256";

        public const string GC_DIZIONARI_16 = @"Dizionari_16";
        public const string GC_DIZIONARI_32 = @"Dizionari_32";
        public const string GC_DIZIONARI_256 = @"Dizionari_256";

        public const string GC_DIZIONARICSV_16 = @"DizionariCSV_16";
        public const string GC_DIZIONARICSV_32 = @"DizionariCSV_32";
        public const string GC_DIZIONARICSV_256 = @"DizionariCSV_256";

        public const string GC_DIZIONARIQUICK_16 = @"DizionariQuick_16";
        public const string GC_DIZIONARIQUICK_32 = @"DizionariQuick_32";
        public const string GC_DIZIONARIQUICK_256 = @"DizionariQuick_256";

        public const string GC_AGENDA_16 = "Agenda_16";
        public const string GC_AGENDA_32 = "Agenda_32";
        public const string GC_AGENDA_256 = "Agenda_256";
        public const string GC_AGENDAELENCO_32 = "AgendaElenco_32";

        public const string GC_ALERTGENERICO_16 = "AlertGenerico_16";
        public const string GC_ALERTGENERICO_32 = "AlertGenerico_32";
        public const string GC_ALERTGENERICO_256 = "AlertGenerico_256";

        public const string GC_ALERTGENERICO_DISABLED_16 = "AlertGenericoD_16";
        public const string GC_ALERTGENERICO_DISABLED_32 = "AlertGenericoD_32";
        public const string GC_ALERTGENERICO_DISABLED_256 = "AlertGenericoD_256";

        public const string GC_ALERTALLERGIA_16 = "AlertAllergia_16";
        public const string GC_ALERTALLERGIA_32 = "AlertAllergia_32";
        public const string GC_ALERTALLERGIA_256 = "AlertAllergia_256";

        public const string GC_ALERTALLERGIA_DISABLED_16 = "AlertAllergiaD_16";
        public const string GC_ALERTALLERGIA_DISABLED_32 = "AlertAllergiaD_32";
        public const string GC_ALERTALLERGIA_DISABLED_256 = "AlertAllergiaD_256";

        public const string GC_PAZIENTI_16 = "Pazienti_16";
        public const string GC_PAZIENTI_32 = "Pazienti_32";
        public const string GC_PAZIENTI_256 = "Pazienti_256";

        public const string GC_WORKLIST_16 = "Worklist_16";
        public const string GC_WORKLIST_32 = "Worklist_32";
        public const string GC_WORKLIST_256 = "Worklist_256";

        public const string GC_CONSULENZA_16 = "Consulenza_16";
        public const string GC_CONSULENZA_32 = "Consulenza_32";
        public const string GC_CONSULENZA_256 = "Consulenza_256";

        public const string GC_CONSULENZE_16 = "Consulenze_16";
        public const string GC_CONSULENZE_32 = "Consulenze_32";
        public const string GC_CONSULENZE_256 = "Consulenze_256";

        public const string GC_CARTELLACLINICA_16 = "CartellaClinica_16";
        public const string GC_CARTELLACLINICA_32 = "CartellaClinica_32";
        public const string GC_CARTELLACLINICA_256 = "CartellaClinica_256";

        public const string GC_CHIUSURACARTELLA_16 = "ChiusuraCartella_16";
        public const string GC_CHIUSURACARTELLA_32 = "ChiusuraCartella_32";
        public const string GC_CHIUSURACARTELLA_256 = "ChiusuraCartella_256";

        public const string GC_CARTELLEINVISIONE_16 = "CartelleInVisione_16";
        public const string GC_CARTELLEINVISIONE_32 = "CartelleInVisione_32";
        public const string GC_CARTELLEINVISIONE_256 = "CartelleInVisione_256";

        public const string GC_MATHOME_16 = "MatildeHome_16";
        public const string GC_MATHOME_32 = "MatildeHome_32";
        public const string GC_MATHOME_256 = "MatildeHome_256";

        public const string GC_CONTATTI_16 = "MatildeHome_16";
        public const string GC_CONTATTI_32 = "MatildeHome_32";
        public const string GC_CONTATTI_256 = "MatildeHome_256";

        public const string GC_KEY_16 = "Key_16";
        public const string GC_KEY_32 = "Key_32";
        public const string GC_KEY_256 = "Key_256";

        public const string GC_WIZARD_16 = "Wizard_16";
        public const string GC_WIZARD_32 = "Wizard_32";
        public const string GC_WIZARD_256 = "Wizard_256";

        public const string GC_SISTEMI_16 = "Sistemi_16";
        public const string GC_SISTEMI_32 = "Sistemi_32";
        public const string GC_SISTEMI_256 = "Sistemi_256";

        public const string GC_COPIA_16 = "Copia_16";
        public const string GC_COPIA_32 = "Copia_32";
        public const string GC_COPIA_256 = "Copia_256";

        public const string GC_PAZIENTEFEMMINA_16 = "PazienteFemmina_16";
        public const string GC_PAZIENTEFEMMINA_32 = "PazienteFemmina_32";
        public const string GC_PAZIENTEFEMMINA_256 = "PazienteFemmina_256";

        public const string GC_PAZIENTEMASCHIO_16 = "PazienteMaschio_16";
        public const string GC_PAZIENTEMASCHIO_32 = "PazienteMaschio_32";
        public const string GC_PAZIENTEMASCHIO_256 = "PazienteMaschio_256";

        public const string GC_INRILIEVO_16 = "InRilievo_16";
        public const string GC_INRILIEVO_32 = "InRilievo_32";
        public const string GC_INRILIEVO_256 = "InRilievo_256";

        public const string GC_INRILIEVORIMUOVI_256 = "InRilievoRimuovi_256";

        public const string GC_FIRMA_16 = "Firma_16";
        public const string GC_FIRMA_32 = "Firma_32";
        public const string GC_FIRMA_256 = "Firma_256";

        public const string GC_FIRMAMULTIPLA_16 = "FirmaMultipla_16";
        public const string GC_FIRMAMULTIPLA_32 = "FirmaMultipla_32";
        public const string GC_FIRMAMULTIPLA_256 = "FirmaMultipla_256";
        public const string GC_FIRMAMULTIPLARAP_256 = "FirmaMultiplaRAP_256";

        public const string GC_FIRMAAGGIUNGI_256 = "FirmaAggiungi_256";
        public const string GC_FIRMARIMUOVI_256 = "FirmaRimuovi_256";

        public const string GC_CONNESSOSI_16 = "ConnessoSi_16";
        public const string GC_CONNESSOSI_32 = "ConnessoSi_32";
        public const string GC_CONNESSOSI_256 = "ConnessoSi_256";

        public const string GC_CONNESSONO_256 = "ConnessoNo_256";

        public const string GC_SEGNALIBRI_16 = "Segnalibri_16";
        public const string GC_SEGNALIBRI_32 = "Segnalibri_32";
        public const string GC_SEGNALIBRI_256 = "Segnalibri_256";

        public const string GC_SEGNALIBROADD_32 = "Segnalibro_Aggiungi_32";
        public const string GC_SEGNALIBROADD_256 = "Segnalibro_Aggiungi_256";

        public const string GC_SEGNALIBRODEL_32 = "Segnalibro_Cancella_32";
        public const string GC_SEGNALIBRODEL_256 = "Segnalibro_Cancella_256";

        public const string GC_DISTACCO_256 = "Distacco_256";

        public const string GC_DATOMANCANTE_16 = "DatoMancante_16";
        public const string GC_DATOMANCANTE_32 = "DatoMancante_32";
        public const string GC_DATOMANCANTE_256 = "DatoMancante_256";

        public const string GC_ORDINE_16 = "Ordine_16";
        public const string GC_ORDINE_32 = "Ordine_32";
        public const string GC_ORDINE_256 = "Ordine_256";

        public const string GC_ORDINEMONITOR_16 = "OrdineMonitor_16";
        public const string GC_ORDINEMONITOR_32 = "OrdineMonitor_32";
        public const string GC_ORDINEMONITOR_256 = "OrdineMonitor_256";

        public const string GC_PARAMETRIVITALIGRAFICO_16 = "ParametriVitaliGrafico_16";
        public const string GC_PARAMETRIVITALIGRAFICO_32 = "ParametriVitaliGrafico_32";
        public const string GC_PARAMETRIVITALIGRAFICO_256 = "ParametriVitaliGrafico_256";

        public const string GC_CDSSPLUGIN_16 = "Analyze_16";
        public const string GC_CDSSPLUGIN_32 = "Analyze_32";
        public const string GC_CDSSPLUGIN_256 = "Analyze_256";

        public const string GC_ZOOM_256 = "Zoom_256";

        public const string GC_LUCCHETTOAPERTO_16 = "Aperto_16";
        public const string GC_LUCCHETTOAPERTO_32 = "Aperto_32";
        public const string GC_LUCCHETTOAPERTO_256 = "Aperto_256";

        public const string GC_LUCCHETTOCHIUSO_16 = "Chiuso_16";
        public const string GC_LUCCHETTOCHIUSO_32 = "Chiuso_32";
        public const string GC_LUCCHETTOCHIUSO_256 = "Chiuso_256";

        public const string GC_LUCCHETTOCHIUSOD_16 = "ChiusoD_16";
        public const string GC_LUCCHETTOCHIUSOD_32 = "ChiusoD_32";
        public const string GC_LUCCHETTOCHIUSOD_256 = "ChiusoD_256";

        public const string GC_LUCCHETTOAPERTOFIRMA_16 = "ApertoFirma_16";
        public const string GC_LUCCHETTOAPERTOFIRMA_32 = "ApertoFirma_32";
        public const string GC_LUCCHETTOAPERTOFIRMA_256 = "ApertoFirma_256";

        public const string GC_LUCCHETTOAPERTODFIRMA_16 = "ApertoDFirma_16";
        public const string GC_LUCCHETTOAPERTODFIRMA_32 = "ApertoDFirma_32";
        public const string GC_LUCCHETTOAPERTODFIRMA_256 = "ApertoDFirma_256";

        public const string GC_LUCCHETTOCHIUSOFIRMA_16 = "ChiusoFirma_16";
        public const string GC_LUCCHETTOCHIUSOFIRMA_32 = "ChiusoFirma_32";
        public const string GC_LUCCHETTOCHIUSOFIRMA_256 = "ChiusoFirma_256";

        public const string GC_LUCCHETTOCHIUSODFIRMA_16 = "ChiusoDFirma_16";
        public const string GC_LUCCHETTOCHIUSODFIRMA_32 = "ChiusoDFirma_32";
        public const string GC_LUCCHETTOCHIUSODFIRMA_256 = "ChiusoDFirma_256";

        public const string GC_NUOVAREVISIONE_16 = "NuovaRevisione_16";
        public const string GC_NUOVAREVISIONE_32 = "NuovaRevisione_32";
        public const string GC_NUOVAREVISIONE_256 = "NuovaRevisione_256";

        public const string GC_SOSPENDI_16 = "Sospendi_16";
        public const string GC_SOSPENDI_32 = "Sospendi_32";
        public const string GC_SOSPENDI_256 = "Sospendi_256";

        public const string GC_AGENDAGIORNALIERA_16 = "AgendaGiornaliera_16";
        public const string GC_AGENDAGIORNALIERA_32 = "AgendaGiornaliera_32";
        public const string GC_AGENDAGIORNALIERA_256 = "AgendaGiornaliera_256";

        public const string GC_AGENDAGIORNALIERALAV_16 = "AgendaGiornalieraLav_16";
        public const string GC_AGENDAGIORNALIERALAV_32 = "AgendaGiornalieraLav_32";
        public const string GC_AGENDAGIORNALIERALAV_256 = "AgendaGiornalieraLav_256";

        public const string GC_AGENDAGIORNALIERALAV5_16 = "AgendaGiornalieraLav5_16";
        public const string GC_AGENDAGIORNALIERALAV5_32 = "AgendaGiornalieraLav5_32";
        public const string GC_AGENDAGIORNALIERALAV5_256 = "AgendaGiornalieraLav5_256";

        public const string GC_AGENDAMENSILE_16 = "AgendaMensile_16";
        public const string GC_AGENDAMENSILE_32 = "AgendaMensile_32";
        public const string GC_AGENDAMENSILE_256 = "AgendaMensile_256";

        public const string GC_AGENDAOGGI_16 = "AgendaOggi_16";
        public const string GC_AGENDAOGGI_32 = "AgendaOggi_32";
        public const string GC_AGENDAOGGI_256 = "AgendaOggi_256";

        public const string GC_AGENDASETTIMANALE_16 = "AgendaSettimanale_16";
        public const string GC_AGENDASETTIMANALE_32 = "AgendaSettimanale_32";
        public const string GC_AGENDASETTIMANALE_256 = "AgendaSettimanale_256";

        public const string GC_TRACKER_256 = "Tracker_256";

        public const string GC_FOGLIOUNICO_16 = "FUT_16";
        public const string GC_FOGLIOUNICO_32 = "FUT_32";
        public const string GC_FOGLIOUNICO_256 = "FUT_256";

        public const string GC_ALLEGATOSTAMPAETICHETTE_256 = "AllegatoStampaEtichette_256";

        public const string GC_DOWNLOADDOCUMENTO_16 = "DownloadDocumneto_16";
        public const string GC_DOWNLOADDOCUMENTO_32 = "DownloadDocumento_32";
        public const string GC_DOWNLOADDOCUMENTO_256 = "DownloadDocumento_256";

        public const string GC_RTF_16 = "RTF_16";
        public const string GC_RTF_32 = "RTF_32";
        public const string GC_RTF_256 = "RTF_256";

        public const string GC_GRIGLIACONTINUA_16 = "GrigliaContinua_16";
        public const string GC_GRIGLIACONTINUA_32 = "GrigliaContinua_32";

        public const string GC_ZOOMIN_256 = "ZoomIn_256";
        public const string GC_ZOOMOUT_256 = "ZoomOut_256";
        public const string GC_PAGGIU_256 = "PagGiu_256";
        public const string GC_PAGSU_256 = "PagSu_256";
        public const string GC_FIT_H_256 = "FitH_256";
        public const string GC_FIT_PAGE_256 = "FitToPage_256";

        public const string GC_CTRL_INGRANDISCI_16 = "CtrlIngrandisci_16";
        public const string GC_CTRL_INGRANDISCI_32 = "CtrlIngrandisci_32";
        public const string GC_CTRL_INGRANDISCI_256 = "CtrlIngrandisci_256";

        public const string GC_CTRL_RIDUCI_16 = "CtrlRiduci_16";
        public const string GC_CTRL_RIDUCI_32 = "CtrlRiduci_32";
        public const string GC_CTRL_RIDUCI_256 = "CtrlRiduci_256";

        public const string GC_LETTERA_DIMISSIONE_16 = "LetteraDimissione_16";
        public const string GC_LETTERA_DIMISSIONE_32 = "LetteraDimissione_32";
        public const string GC_LETTERA_DIMISSIONE_256 = "LetteraDimissioni_256";

        public const string GC_MODIFICA_ORARIO_32 = "ModificaOrario_32";
        public const string GC_MODIFICA_ORARIO_256 = "ModificaOrario_256";

        public const string GC_PROFILO_ESPANDI_256 = "ProfiloEspandi_256";
        public const string GC_PROFILO_AGGIUNGI_256 = "ProfiloAggiungi_256";
        public const string GC_PROFILO_CANCELLA_256 = "ProfiloCancella_256";

        public const string GC_MOW_16 = @"Mow_16";
        public const string GC_MOW_32 = @"Mow_32";
        public const string GC_MOW_256 = @"Mow_256";

        public const string GC_FARMACI_16 = "Pills_16";
        public const string GC_FARMACI_32 = "Pills_32";
        public const string GC_FARMACI_256 = "Pills_256";

        public const string GC_OBBLIGATORIO_256 = "Obbligatorio_256";

        public const string GC_RX_256 = "RX_256";

        public const string GC_WORD = "Word";

        public const string GC_SB_SU = "SB_Su";
        public const string GC_SB_PAGSU = "SB_PagSu";
        public const string GC_SB_GIU = "SB_Giu";
        public const string GC_SB_PAGGIU = "SB_PagGiu";
        public const string GC_SB_PRIMO = "SB_Primo";
        public const string GC_SB_ULTIMO = "SB_Ultimo";

        public const string GC_FRECCIA_DX_16 = "FrecciaDx_16";
        public const string GC_FRECCIA_DX_32 = "FrecciaDx_32";
        public const string GC_FRECCIA_DX_256 = "FrecciaDx_256";

        public const string GC_SALVA_16 = "Salva_16";
        public const string GC_SALVA_32 = "Salva_32";
        public const string GC_SALVA_256 = "Salva_256";

        public const string GC_SPOSTA_IN_ALTRA_CARTELLA_256 = "SpostaInAltraCartella_256";

        public const string GC_OCCHIOCHIUSO_16 = "OcchioChiuso_16";
        public const string GC_OCCHIOCHIUSO_32 = "OcchioChiuso_32";
        public const string GC_OCCHIOCHIUSO_256 = "OcchioChiuso_256";

        public const string GC_OCCHIOAPERTO_16 = "OcchioAperto_16";
        public const string GC_OCCHIOAPERTO_32 = "OcchioAperto_32";
        public const string GC_OCCHIOAPERTO_256 = "OcchioAperto_256";

        public const string GC_OCCHIOAPERTOFRECCIA_16 = "OcchioApertoFreccia_16";
        public const string GC_OCCHIOAPERTOFRECCIA_32 = "OcchioApertoFreccia_32";

        public const string GC_PREFERITIAGGIUNGI_256 = "PreferitiAggiungi_256";
        public const string GC_PREFERITIALTRI_256 = "PreferitiAltri_256";
        public const string GC_PREFERITIMIEI_256 = "PreferitiMiei_256";

        public const string GC_CONFIG_PC_16 = "PC Desktop_16";
        public const string GC_CONFIG_PC_32 = "PC Desktop_32";
        public const string GC_CONFIG_PC_256 = "PC Desktop_256";

        public const string GC_CANCELLATONDO_16 = "CancellaTondo_16";
        public const string GC_CANCELLATONDO_32 = "CancellaTondo_32";
        public const string GC_CANCELLATONDO_256 = "CancellaTondo_256";

        public const string GC_CANCELLATESSERA_32 = "CancellaTessera_32";
        public const string GC_CANCELLATESSERA_256 = "CancellaTessera_256";

        public const string GC_SOSPENDITESSERA_32 = "SospendiTessera_32";
        public const string GC_SOSPENDITESSERA_256 = "SospendiTessera_256";

        public const string GC_INTEGRAZIONI_16 = "Integrazioni_16";
        public const string GC_INTEGRAZIONI_32 = "Integrazioni_32";
        public const string GC_INTEGRAZIONI_256 = "Integrazioni_256";

        public const string GC_PROSEGUITERAPIA_256 = "ProseguiTerapia_256";

        public const string GC_PRETRASFERIMENTO_256 = "PreTrasferimento_256";

        public const string GC_PROTOCOLLOATTIVITA_16 = "ProtocolloAttivita_16";
        public const string GC_PROTOCOLLOATTIVITA_32 = "ProtocolloAttivita_32";
        public const string GC_PROTOCOLLOATTIVITA_256 = "ProtocolloAttivita_256";

        public const string GC_SELEZIONATUTTI_256 = "SelezionaTutti_256";
        public const string GC_DESELEZIONATUTTI_256 = "DeSelezionaTutti_256";

        public const string GC_ASSEGNANUMERO_256 = "AssegnaNumero_256";
        public const string GC_MODIFICANUMERO_256 = "ModificaNumero_256";

        public const string GC_CHIAMATA_256 = "Chiamata_256";
        public const string GC_CHIAMATAANNULLA_256 = "ChiamataAnnulla_256";

        public const string GC_SELEZIONI_16 = @"Selezioni_16";
        public const string GC_SELEZIONI_32 = @"Selezioni_32";
        public const string GC_SELEZIONI_256 = @"Selezioni_256";

        public const string GC_TASKAB_256 = @"TaskAB_256";
        public const string GC_TASKABSELEZIONATO_256 = @"TaskABSelezionato_256";

        public const string GC_INFOPAZ_256 = @"infopaz_256";

        public const string GC_TERAPIAORALE_256 = @"TerapiaOrale_256";
        public const string GC_TERAPIAORALEBN_256 = @"TerapiaOraleBN_256";

        public const string GC_SCREEN_16 = @"screen_16";
        public const string GC_SCREEN_32 = @"screen_32";
        public const string GC_SCREEN_128 = @"screen_128";
        public const string GC_SCREEN_256 = @"screen_256";

        public const string GC_CONVERSIONE_16 = @"convert_16";
        public const string GC_CONVERSIONE_32 = @"convert_32";
        public const string GC_CONVERSIONE_128 = @"convert_128";
        public const string GC_CONVERSIONE_256 = @"convert_256";

        public const string GC_SCANNERIMPORTA_256 = @"ScannerImporta_256";

        public const string GC_STAMPACARTELLAAPERTACHIUSA_16 = @"StampaCartellaApertaChiusa_16";
        public const string GC_STAMPACARTELLAAPERTACHIUSA_32 = @"StampaCartellaApertaChiusa_32";
        public const string GC_STAMPACARTELLAAPERTACHIUSA_256 = @"StampaCartellaApertaChiusa_256";

        public const string GC_FIRMACARTELLEAPERTE_16 = @"FirmaCartelleAperte_16";
        public const string GC_FIRMACARTELLEAPERTE_32 = @"FirmaCartelleAperte_32";
        public const string GC_FIRMACARTELLEAPERTE_256 = @"FirmaCartelleAperte_256";


        public const string GC_CONSEGNE_16 = @"Consegne_16";
        public const string GC_CONSEGNE_32 = @"Consegne_32";
        public const string GC_CONSEGNE_256 = @"Consegne_256";

        public const string GC_CONSENSI_16 = @"Consensi_16";
        public const string GC_CONSENSI_32 = @"Consensi_32";
        public const string GC_CONSENSI_256 = @"Consensi_256";

        public const string GC_CONSEGNEPAZIENTE_16 = @"ConsegnePaziente_16";
        public const string GC_CONSEGNEPAZIENTE_32 = @"ConsegnePaziente_32";
        public const string GC_CONSEGNEPAZIENTE_256 = @"ConsegnePaziente_256";

        public const string GC_XCHECKED_48 = @"xChecked_48";

        public const string GC_PERCORSOCARTELLAAMBULATORIALE_16 = @"PercorsoCartellaAmbulatoriale_16";
        public const string GC_PERCORSOCARTELLAAMBULATORIALE_32 = @"PercorsoCartellaAmbulatoriale_32";
        public const string GC_PERCORSOCARTELLAAMBULATORIALE_256 = @"PercorsoCartellaAmbulatoriale_256";

        public const string GC_LOGOCE_16 = @"LogoCE_16";
        public const string GC_LOGOCE_32 = @"LogoCE_32";
        public const string GC_LOGOCE_256 = @"LogoCE_256";

        public const string GC_HELP_16 = @"Help_16";
        public const string GC_HELP_32 = @"Help_32";
        public const string GC_HELP_256 = @"Help_256";


        #endregion

        #region Dichiarazioni Audio

        public const string GC_WAV_APERTURAMATILDE = @"AperturaMatilde";
        public const string GC_WAV_CHIUSURAMATILDE = @"ChiusuraMatilde";
        public const string GC_WAV_ALTREFUNZIONI = @"AltreFunzioni";

        #endregion

        #region Immagini

        public static Image GetImageFromResource(string vsName)
        {

            string sImageName = @"UnicodeSrl.ScciResource.Resources." + vsName + @".png";

            try
            {
                return UnicodeSrl.Framework.GraphicsExt.GraphicsHelper.BmpFromResx(System.Reflection.Assembly.GetExecutingAssembly(), sImageName);
            }
            catch (Exception)
            {
                return null;
            }

        }

        public static Icon GetIconFromResource(string vsName)
        {

            try
            {
                Bitmap oBitmap = new Bitmap(GetImageFromResource(vsName), 32, 32);
                return Icon.FromHandle(oBitmap.GetHicon());
            }
            catch (Exception)
            {
                return null;
            }

        }

        public static Image GetImageFromResourceWithTip(string vsName, string vsTip)
        {

            try
            {

                Image oImage = GetImageFromResource(vsName);
                Graphics oGraphics = Graphics.FromImage(oImage);

                int nWidth = (oImage.Size.Width * 50 / 100);
                int nHeight = (oImage.Size.Height * 50 / 100);

                SolidBrush redBrush = new SolidBrush(Color.Green);
                Rectangle rect = new Rectangle(oImage.Size.Width - (nWidth * vsTip.Length), 0, (nWidth * vsTip.Length), nHeight);
                oGraphics.FillEllipse(redBrush, rect);

                Font oFont = new Font("Calibri", nWidth, FontStyle.Bold, GraphicsUnit.Pixel);
                StringFormat stringFormat = new StringFormat()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                oGraphics.DrawString(vsTip, oFont, Brushes.White, rect, stringFormat);

                return oImage;

            }
            catch (Exception)
            {
                return null;
            }

        }


        #endregion

        #region Audio

        public static Stream GetSoundFromResource(string vsName)
        {

            string sSoundName = @"UnicodeSrl.ScciResource.Resources." + vsName + @".wav";

            try
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(sSoundName);
            }
            catch (Exception)
            {
                return null;
            }

        }

        public static void GetPlaySoundFromResource(string vsName)
        {

            try
            {

                using (SoundPlayer snd = new SoundPlayer())
                {

                    snd.Stream = GetSoundFromResource(vsName);
                    snd.PlaySync();

                }

            }
            catch (Exception)
            {

            }

        }

        #endregion

    }
}
