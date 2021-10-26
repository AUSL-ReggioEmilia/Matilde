using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using UnicodeSrl.ScciResource;
using UnicodeSrl.ScciCore;
using UnicodeSrl.Scci;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinGrid;
using UnicodeSrl.Framework.Diagnostics;
using Microsoft.VisualBasic;
using System.Xml.Serialization;
using System.Xml;

using Infragistics.Win.UltraWinSchedule;
using UnicodeSrl.ScciManagement;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.ScciManagement
{
    public static class MyStatics
    {

        #region Dichiarazioni

        private static Configurazione m_Configurazione;
        private static Scci.DataContracts.ScciAmbiente m_Ambiente;


        #region Costanti x ToolBar Main

        public const string GC_CHIUDIFINESTRE = @"Chiudi Finestre";
        public const string GC_ESCI = @"Esci";
        public const string GC_CONFIGURAZIONE = @"Configurazione";
        public const string GC_CONFIGCE = @"ConfigCE";
        public const string GC_CONFIGURAZIONE_AMBIENTE = @"ConfigurazioneAmbiente";
        public const string GC_CONFIGURAZIONE_PC = @"ConfigurazionePC";
        public const string GC_AMMINISTRAZIONE = @"Amministrazione";
        public const string GC_ELABORAZIONI = @"Elaborazioni";
        public const string GC_SEZIONIFUT = @"SezioniFUT";
        public const string GC_NORMALIZZAZIONE = @"Normalizzazione";
        public const string GC_DOCUMENTIFIRMATI = @"DocumentiFirmati";
        public const string GC_REPORTSTORICIZZATI = @"ReportStoricizzati";
        public const string GC_CONTATORI = @"Contatori";
        public const string GC_FILTRISPECIALI = @"FiltriSpeciali";
        public const string GC_SELEZIONI = @"Selezioni";

        public const string GC_SYMBOL_CHECK = "Symbol_Check";
        public const string GC_SYMBOL_RESTRICTED = "Symbol_Restricted";

        public const string GC_LOGIN = @"Login";
        public const string GC_MODULI = @"Moduli";
        public const string GC_RUOLI = @"Ruoli";
        public const string GC_UNITAATOMICHE = @"UnitaAtomiche";
        public const string GC_UNITAATOMICHEMENU = @"UnitaAtomicheMenu";
        public const string GC_UNITAATOMICHETREEVIEW = @"UnitaAtomicheTreeview";
        public const string GC_UNITAOPERATIVE = @"UnitaOperative";
        public const string GC_AZIENDE = @"Aziende";
        public const string GC_DIARI = @"DiarioClinicoMenu";
        public const string GC_DIARIOMEDICO = @"Diario Medico";
        public const string GC_DIARIOINFERMIERISTICO = @"Diario Infermieristico";
        public const string GC_STATODIARIO = @"StatoDiario";
        public const string GC_TIPODIARIO = @"TipoDiario";
        public const string GC_PARAMETRIVITALI = @"Parametri Vitali";
        public const string GC_PARAMETRIVITALITIPO = @"ParametriVitaliTipo";
        public const string GC_PARAMETRIVITALISTATO = @"ParametriVitaliStato";
        public const string GC_EVIDENZACLINICA = @"Evidenza Clinica";
        public const string GC_EVIDENZACLINICAMENU = @"EvidenzaClinicaMenu";
        public const string GC_EVIDENZACLINICASTATI = @"EvidenzaClinicaStati";
        public const string GC_EVIDENZACLINICASTATIVISIONE = @"EvidenzaClinicaStatiVisione";
        public const string GC_REPORT = @"Report";
        public const string GC_REPORTMENU = @"ReportMenu";
        public const string GC_REPORTTREEVIEW = @"ReportTreeview";
        public const string GC_FORMATOREPORT = @"FormatoReport";
        public const string GC_MASCHERE = @"Maschere";
        public const string GC_TESTIPREDEFINITI = @"Testi Predefiniti";
        public const string GC_TESTINOTEPREDEFINITI = @"Testi Note Predefiniti";
        public const string GC_TESTIPREDEFINITIMENU = @"Testi PredefinitiMenu";
        public const string GC_TESTIPREDEFINITITREEVIEW = @"Testi PredefinitiTreeview";
        public const string GC_ICONE = @"Icone";
        public const string GC_NEWS = @"News";
        public const string GC_APPUNTAMENTI = @"Appuntamenti";
        public const string GC_APPUNTAMENTITIPO = @"AppuntamentiTipo";
        public const string GC_APPUNTAMENTISTATO = @"AppuntamentiStato";
        public const string GC_APPUNTAMENTISTATOAGENDE = @"AppuntamentiStatoAgende";
        public const string GC_TASKINFERMIERISTICI = @"TaskInfermieristici";
        public const string GC_TASKINFERMIERISTICITIPO = @"TaskInfermieristiciTipo";
        public const string GC_TASKINFERMIERISTICISTATO = @"TaskInfermieristiciStato";
        public const string GC_SCHEDE = @"Schede";
        public const string GC_SCHEDELISTA = @"Lista Schede";
        public const string GC_SCHEDETIPO = @"Tipo Schede";
        public const string GC_SCHEDESTATO = @"Stato Schede";
        public const string GC_SCHEDESTATOCALCOLATO = @"Stato Schede Calcolato";
        public const string GC_SCHEDEESPORTA = @"Esporta Schede";
        public const string GC_TIPOEPISODIOMENU = @"TipoEpisodioMenu";
        public const string GC_TIPOEPISODIO = @"TipoEpisodio";
        public const string GC_STATOTRASFERIMENTO = @"StatoTrasferimento";
        public const string GC_CARTELLAMENU = @"CartellaMenu";
        public const string GC_STATOCARTELLA = @"StatoCartella";
        public const string GC_STATOCARTELLAINFO = @"StatoCartellaInfo";
        public const string GC_STATOCARTELLAINVISIONE = @"StatoCartellaInVisione";
        public const string GC_STATOEPISODIO = @"StatoEpisodio";
        public const string GC_LETTIGESTIONE = @"Gestione Letti";
        public const string GC_LETTI = @"Letti";
        public const string GC_SETTORI = @"Settori";
        public const string GC_DIZIONARI = @"Dizionari";
        public const string GC_SCHEDETREEVIEW = @"SchedeTreeview";
        public const string GC_AGENDE = @"Agende";
        public const string GC_AGENDEMENU = @"AgendeMenu";
        public const string GC_TIPOAGENDA = @"TipoAgenda";
        public const string GC_FESTIVITA = @"Festività";
        public const string GC_ORDINIMENU = @"OrdiniMenu";
        public const string GC_ORDINITIPO = @"OrdiniTipo";
        public const string GC_ORDINIFORMULE = @"OrdiniFormule";
        public const string GC_ORDINIATTRIBUTI = @"OrdiniAttributi";
        public const string GC_ORDINISTATO = @"OrdiniStato";
        public const string GC_CONSEGNE = @"Consegne";
        public const string GC_CONSEGNATIPO = @"ConsegnaTipo";
        public const string GC_CONSEGNASTATO = @"ConsegnaStato";
        public const string GC_CONSEGNEPAZIENTE = @"ConsegnePaziente";
        public const string GC_CONSEGNAPAZIENTETIPO = @"ConsegnaPazienteTipo";
        public const string GC_CONSEGNAPAZIENTESTATO = @"ConsegnaPazienteStato";
        public const string GC_CONSEGNAPAZIENTESTATORUOLI = @"ConsegnaPazienteStatoRuoli";
        public const string GC_CONSENSI = @"Consensi";
        public const string GC_CONSENSOSTATICALCOLATI = @"StatiConsensoCalcolati";

        public const string GC_TIPOPRESCRIZIONEMENU = @"TipoPrescrizioneMenu";
        public const string GC_TIPOPRESCRIZIONE = @"TipoPrescrizione";
        public const string GC_STATOPRESCRIZIONE = @"StatoPrescrizione";
        public const string GC_STATOPRESCRIZIONETEMPI = @"StatoPrescrizioneTempi";
        public const string GC_STATOCONTINUAZIONE = @"StatoContinuazione";
        public const string GC_VIESOMMINISTRAZIONE = @"ViaSomministrazione";

        public const string GC_PROTOCOLLI = @"Protocolli";
        public const string GC_PROFILITERAPIEFARMACOLOGICHE = @"ProfiliTerapieFarmacologiche";
        public const string GC_PROTOCOLLITEMPI = @"ProtocolliTempi";
        public const string GC_PROTOCOLLIATTIVITA = @"Protocolli Attivita";
        public const string GC_ASSTIPOPRESCRIZIONEPROTOCOLLI = @"AssTipoPrescrizioneProtocolli";

        public const string GC_ALLEGATIMENU = @"AllegatiMenu";
        public const string GC_ALLEGATITIPO = @"Tipo Allegato";
        public const string GC_ALLEGATIFORMATO = @"Formato Allegato";
        public const string GC_ALLEGATISTATO = @"Stato Allegato";
        public const string GC_STANZE = @"Stanze";

        public const string GC_ASSLETTIUA = @"Associazione Letti UA";
        public const string GC_ALERTALLERGIEANAMNESI = @"AlertAllergieAnamnesi";
        public const string GC_ALERTALLERGIEANAMNESITIPO = @"AlertAllergieAnamnesiTipo";
        public const string GC_ALERTALLERGIEANAMNESISTATO = @"AlertAllergieAnamnesiStato";
        public const string GC_ALERTGENERICI = @"AlertGenerici";
        public const string GC_ALERTGENERICITIPO = @"AlertGenericiTipo";
        public const string GC_ALERTGENERICISTATO = @"AlertGenericiStato";

        public const string GC_CDSSGESTIONE = @"Gestione CDSS";
        public const string GC_CDSSAZIONI = @"CDSS Azioni";
        public const string GC_CDSSPLUGINS = @"CDSS Plugin";
        public const string GC_CDSSSTRUTTURA = @"CDSS Struttura";
        public const string GC_CDSSSTRUTTURARUOLI = @"CDSS Struttura Ruoli";

        public const string GC_INTEGRAZIONIGESTIONE = @"Gestione Integrazioni";
        public const string GC_INTEGRAZIONIDESTINATARI = @"Integrazioni Destinatari";
        public const string GC_INTEGRAZIONICAMPI = @"Integrazioni Campi";
        public const string GC_INTEGRAZIONIALTRE = @"Altre Integrazioni";

        public const string GC_MANUTENZIONEDATI = @"ManutenzioneDati";
        public const string GC_CONSULTAEPISODIO = @"ConsultaEpisodio";

        public const string GC_WORD = @"Word";

        public const string GC_SISTEMI = @"Sistemi";

        public const string GC_BANCHEDATI = "BancheDati";
        public const string GC_BANCHEDATILISTA = "BancheDatiLista";

        public const string GC_SCREEN = @"Screen";

        public const string GC_ENTITAALLEGATI = @"Entita Allegati";

        #endregion

        #region Costanti x ToolBar View
        public const string GC_NUOVO = "Nuovo";
        public const string GC_MODIFICA = "Modifica";
        public const string GC_ELIMINA = "Elimina";
        public const string GC_VISUALIZZA = "Visualizza";
        public const string GC_STAMPA = "Stampa";
        public const string GC_AGGIORNA = "Aggiorna";
        public const string GC_ESPORTA = "Esporta";
        public const string GC_GENERA = "Genera";
        public const string GC_COPIA = "Copia";
        public const string GC_CONVERSIONE = "Conversione";
        public const string GC_FOLDER = "Folder";
        public const string GC_MODIFICAVERSIONE = "ModificaVersione";
        public const string GC_MODIFICAVERSIONECORRENTE = "ModificaVersioneCorrente";
        public const string GC_CONVERSIONESCHEDE = "ConversioneSchede";
        public const string GC_ESPORTAXML = "ExpXML";
        public const string GC_IMPORTAXML = "ImpXML";
        public const string GC_DIZIONARIOCSV = "DizionarioCSV";
        public const string GC_DIZIONARIOQUICK = "DizionarioQuick";

        #endregion

        #region Costanti Treeview
        public const string TV_ROOT = "Root";
        public const string TV_PATH = "Path";
        public const string TV_MODULI = "Moduli";
        public const string TV_UNITAATOMICHE = "UnitaAtomiche";
        public const string TV_REPORT = "Report";
        public const string TV_TESTOPREDEFINITO = "Testo Predefinito";
        public const string TV_SCHEDA = "Scheda";
        public const string TV_INTESTAZIONE = "Intestazione";

        public const string TV_ENTITA = "Entita";
        public const string TV_AZIONE = "Azione";
        public const string TV_VOCE = "Voce";

        public const string TV_SPOSTASINGOLOMENU = "SpostaSingoloMenu";
        public const string TV_SPOSTASINGOLOAPPPAZAMB = "SpostaSingoloAppPazAmb";
        public const string TV_SPOSTASINGOLOAPPPAZRO = "SpostaSingoloAppPazRO";
        public const string TV_SPOSTASINGOLODIARIOCLINICO = "SpostaSingoloDiarioClinico";
        public const string TV_SPOSTASINGOLOEVIDENZACLINICA = "SpostaSingoloEvidenzaClinica";
        public const string TV_SPOSTASINGOLOPRESCRIZIONE = "SpostaSingoloPrescrizione";
        public const string TV_SPOSTASINGOLOPARAMETRIVITALI = "SpostaSingoloParametriVitali";
        public const string TV_SPOSTASINGOLOSCHEDAPAZ = "SpostaSingoloSchedaPaz";
        public const string TV_SPOSTASINGOLOSCHEDAEPI = "SpostaSingoloSchedaEpi";
        public const string TV_SPOSTASINGOLOTASKINF = "SpostaSingoloTaskInf";
        public const string TV_SPOSTASINGOLOALERTGENERICO = "SpostaSingoloAlertGenerico";
        public const string TV_SPOSTASINGOLOAPPAMBTOEPI = "SpostaSingoloAppAmbToEpi";
        public const string TV_SPOSTASINGOLOAPPEPITOAMB = "SpostaSingoloAppEpiToAmb";
        public const string TV_ALLINEAADT = "AllineaADT";

        public const string TV_SPOSTAGRUPPOMENU = "SpostaGruppoMenu";
        public const string TV_SPOSTAGRUPPOAPPPAZAMB = "SpostaGruppoPazAmb";
        public const string TV_SPOSTAGRUPPOAPPPAZRO = "SpostaGruppoPazRO";
        public const string TV_SPOSTAGRUPPODIARIOCLINICO = "SpostaGruppoDiarioClinico";
        public const string TV_SPOSTAGRUPPOEVIDENZACLINICA = "SpostaGruppoEvidenzaClinica";
        public const string TV_SPOSTAGRUPPOPRESCRIZIONE = "SpostaGruppoPrescrizione";
        public const string TV_SPOSTAGRUPPOPARAMETRIVITALI = "SpostaGruppoParametriVitali";
        public const string TV_SPOSTAGRUPPOSCHEDAPAZ = "SpostaGruppoSchedaPaz";
        public const string TV_SPOSTAGRUPPOSCHEDAEPI = "SpostaGruppoSchedaEpi";
        public const string TV_SPOSTAGRUPPOTASKINF = "SpostaGruppoTaskInf";
        public const string TV_SPOSTAGRUPPOALERTGENRICO = "SpostaGruppoAlertGenerico";
        public const string TV_SBLOCCOSCHEDA = "SbloccoScheda";
        public const string TV_ANNULLACANCSCHEDA = "AnnullaCancellazioneScheda";
        public const string TV_CANCELLAEVIDENZACLINICA = "CancellaEvidenzaClinica";
        public const string TV_RIPRISTINOREVISIONESCHEDA = "RipristinoRevisioneScheda";

        public const string TV_SCHEDAGENERARTFGRUPPO = "GruppoSchedaMenu";
        public const string TV_SCHEDAGENERARTFSINGOLO = "SchedaGeneraRTF";
        public const string TV_SCHEDAGENERARTFMULTIPLO = "SchedaGeneraRTFMultiplo";

        public const string TV_CARTELLAGRUPPO = "GruppoCartellaMenu";
        public const string TV_MODIFICANUMEROCARTELLA = "ModificaNumeroCartella";
        public const string TV_CARTELLARIAPRI = "CartellaRiapri";
        public const string TV_CARTELLACANCELLA = "CartellaCancella";
        public const string TV_CARTELLASCOLLEGA = "CartellaScollega";

        public const string TV_CARTELLAAMBULATORIALEGRUPPO = "GruppoCartellaAmbulatorialeMenu";
        public const string TV_MODIFICANUMEROCARTELLAAMBULATORIALE = "ModificaNumeroCartellaAmbulatoriale";
        public const string TV_CARTELLAAMBULATORIALERIAPRI = "CartellaAmbulatorialeRiapri";

        #endregion

        #region COSTANTI FORMATI REPORT
        public const string FR_REM = "REM";
        public const string FR_PDF = "PDF";
        public const string FR_WORD = "WORD";
        public const string FR_CABLATO = "CAB";

        #endregion

        public static Array g_split;

        #endregion

        #region Proprietà x configurazione

        public static Configurazione Configurazione
        {
            get
            {
                if (m_Configurazione == null) m_Configurazione = new Configurazione();
                return m_Configurazione;
            }
            set
            {
                m_Configurazione = value;
            }
        }

        public static Scci.DataContracts.ScciAmbiente Ambiente
        {
            get
            {
                if (m_Ambiente == null) m_Ambiente = new Scci.DataContracts.ScciAmbiente();
                return m_Ambiente;
            }
            set
            {
                m_Ambiente = value;
            }
        }


        #endregion

        #region Infragistics

        internal static void SetUltraToolbarsManager(UltraToolbarsManager oToolbarsManager)
        {

            oToolbarsManager.FormDisplayStyle = FormDisplayStyle.RoundedSizable;
            oToolbarsManager.LockToolbars = true;
            oToolbarsManager.RuntimeCustomizationOptions = Infragistics.Win.UltraWinToolbars.RuntimeCustomizationOptions.None;
            oToolbarsManager.Style = Infragistics.Win.UltraWinToolbars.ToolbarStyle.Office2007;

        }

        internal static void SetUltraGroupBox(Infragistics.Win.Misc.UltraGroupBox GroupBox)
        {
            GroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
        }

        internal static void SetUltraTree(Infragistics.Win.UltraWinTree.UltraTree UltraTree)
        {
            SetUltraTree(UltraTree, new Size(20, 20), false);
        }
        internal static void SetUltraTree(Infragistics.Win.UltraWinTree.UltraTree UltraTree, Size leftImageSize, bool hideLines)
        {
            UltraTree.FullRowSelect = true;
            UltraTree.HideSelection = false;
            UltraTree.LeftImagesSize = leftImageSize;
            UltraTree.Override.NodeSpacingAfter = 1;
            UltraTree.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
            UltraTree.ShowLines = !hideLines;
            UltraTree.ShowRootLines = !hideLines;
        }

        internal static void SetUltraGridLayout(ref Infragistics.Win.UltraWinGrid.UltraGrid roGrid, bool vAddFilterRow, bool vOperatorStartWith)
        {

            roGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.None;
            roGrid.DisplayLayout.ScrollStyle = ScrollStyle.Immediate;

            roGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.True;
            roGrid.DisplayLayout.CaptionAppearance.BackColor = Color.LightSteelBlue;
            roGrid.DisplayLayout.CaptionAppearance.BackColor2 = Color.Lavender;
            roGrid.DisplayLayout.CaptionAppearance.BackGradientStyle = Infragistics.Win.GradientStyle.ForwardDiagonal;

            roGrid.DisplayLayout.GroupByBox.Hidden = false;
            roGrid.DisplayLayout.GroupByBox.Prompt = @"Trascina un'intestazione della colonna qui per raggrupparla.";
            roGrid.DisplayLayout.GroupByBox.Appearance.BackColor = Color.LightSteelBlue;
            roGrid.DisplayLayout.GroupByBox.Appearance.BackColor2 = Color.Lavender;
            roGrid.DisplayLayout.GroupByBox.Appearance.BackGradientStyle = Infragistics.Win.GradientStyle.ForwardDiagonal;
            roGrid.DisplayLayout.GroupByBox.PromptAppearance.BackColor = Color.Lavender;
            roGrid.DisplayLayout.GroupByBox.PromptAppearance.BackColor2 = Color.Lavender;
            roGrid.DisplayLayout.GroupByBox.PromptAppearance.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            roGrid.DisplayLayout.GroupByBox.PromptAppearance.ForeColor = Color.Black;

            roGrid.DisplayLayout.Override.AllowAddNew = AllowAddNew.No;
            roGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            roGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            roGrid.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            roGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            roGrid.DisplayLayout.Override.ColumnAutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.VisibleRows;
            roGrid.DisplayLayout.Override.HeaderClickAction = HeaderClickAction.SortSingle;
            roGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            roGrid.DisplayLayout.Override.RowSizing = Infragistics.Win.UltraWinGrid.RowSizing.Sychronized;
            roGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.Extended;
            roGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Extended;

            roGrid.DisplayLayout.Override.RowAlternateAppearance.BackColor = Color.WhiteSmoke;
            roGrid.DisplayLayout.Override.RowAlternateAppearance.ForeColor = Color.DarkBlue;

            roGrid.DisplayLayout.Override.HeaderAppearance.BackColor = Color.LightSteelBlue;
            roGrid.DisplayLayout.Override.HeaderAppearance.BackColor2 = Color.Lavender;
            roGrid.DisplayLayout.Override.HeaderAppearance.BackGradientStyle = Infragistics.Win.GradientStyle.ForwardDiagonal;
            roGrid.DisplayLayout.Override.HeaderAppearance.TextHAlign = Infragistics.Win.HAlign.Center;

            roGrid.DisplayLayout.Override.AllowColMoving = AllowColMoving.NotAllowed;

            if (vAddFilterRow)
            {
                UltraGridAddFilterRow(ref roGrid, vOperatorStartWith);
            }

        }

        internal static void UltraGridAddFilterRow(ref Infragistics.Win.UltraWinGrid.UltraGrid roGrid, bool vOperatorStartWith)
        {

            roGrid.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            roGrid.DisplayLayout.Override.RowFilterMode = RowFilterMode.AllRowsInBand;
            roGrid.DisplayLayout.Override.RowFilterMode = RowFilterMode.Default;
            roGrid.DisplayLayout.Override.FilterUIType = FilterUIType.FilterRow;

            roGrid.DisplayLayout.Override.FilterClearButtonLocation = FilterClearButtonLocation.Row;

            roGrid.DisplayLayout.Override.FilterRowAppearance.BackColor = Color.LightYellow;
            roGrid.DisplayLayout.Override.FilterComparisonType = FilterComparisonType.CaseInsensitive;

            if (vOperatorStartWith)
                roGrid.DisplayLayout.Override.FilterOperatorDefaultValue = FilterOperatorDefaultValue.StartsWith;
            else
                roGrid.DisplayLayout.Override.FilterOperatorDefaultValue = FilterOperatorDefaultValue.Contains;
            roGrid.DisplayLayout.Override.FilterOperatorLocation = FilterOperatorLocation.Hidden;
            roGrid.DisplayLayout.Override.FilterOperandStyle = FilterOperandStyle.Edit;

            roGrid.DisplayLayout.Override.FilterRowPrompt = "";
            roGrid.DisplayLayout.Override.FilterRowPromptAppearance.BackColorAlpha = Infragistics.Win.Alpha.Opaque;
            roGrid.DisplayLayout.Override.SpecialRowSeparator = SpecialRowSeparator.FilterRow;

        }

        internal static void SetUltraComboEditorLayout(ref Infragistics.Win.UltraWinEditors.UltraComboEditor roCombo)
        {
            roCombo.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
            roCombo.HideSelection = false;
            roCombo.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            roCombo.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.Office2007;
        }

        internal static void SetGridWizardFilter(ref Infragistics.Win.UltraWinGrid.UltraGrid roGrid,
                                                   string vsFilterColumnKey, string vsFilterText,
                                                   string vsCaptionColumnKey = "", string vsCaption = "records",
                                                   string vsFilterCaption = "filtrati")
        {

            try
            {

                roGrid.Rows.ColumnFilters[vsFilterColumnKey].FilterConditions.Clear();
                if (vsFilterText != "")
                {
                    roGrid.Rows.ColumnFilters[vsFilterColumnKey].FilterConditions.Add(Infragistics.Win.UltraWinGrid.FilterComparisionOperator.Like, @"*" + vsFilterText + "*");
                }
                if (roGrid.Rows.FilteredInRowCount > 0 & roGrid.Rows.ColumnFilters[vsFilterColumnKey].FilterConditions.Count > 0)
                {
                    roGrid.ActiveRow.Selected = false;
                    roGrid.ActiveRow = roGrid.Rows.GetRowAtVisibleIndex(0);
                    roGrid.ActiveRow.Selected = true;
                }
                else if (roGrid.Rows.FilteredInRowCount == 0 & roGrid.Rows.ColumnFilters[vsFilterColumnKey].FilterConditions.Count > 0)
                {
                    roGrid.ActiveRow.Selected = false;
                    roGrid.ActiveRow = null;
                }
                SetGridWizardCaption(ref roGrid, vsCaptionColumnKey, vsCaption, vsFilterCaption);

            }
            catch (Exception)
            {

            }

        }

        internal static void SetGridWizardFilter(ref ucEasyGrid roGrid,
                                                   string vsFilterColumnKey, string vsFilterText,
                                                   string vsCaptionColumnKey = "", string vsCaption = "records",
                                                   string vsFilterCaption = "filtrati")
        {

            try
            {

                roGrid.Rows.ColumnFilters[vsFilterColumnKey].FilterConditions.Clear();
                if (vsFilterText != "")
                {
                    roGrid.Rows.ColumnFilters[vsFilterColumnKey].FilterConditions.Add(Infragistics.Win.UltraWinGrid.FilterComparisionOperator.Like, @"*" + vsFilterText + "*");
                }
                if (roGrid.Rows.FilteredInRowCount > 0 & roGrid.Rows.ColumnFilters[vsFilterColumnKey].FilterConditions.Count > 0)
                {
                    roGrid.ActiveRow.Selected = false;
                    roGrid.ActiveRow = roGrid.Rows.GetRowAtVisibleIndex(0);
                    roGrid.ActiveRow.Selected = true;
                }
                else if (roGrid.Rows.FilteredInRowCount == 0 & roGrid.Rows.ColumnFilters[vsFilterColumnKey].FilterConditions.Count > 0)
                {
                    roGrid.ActiveRow.Selected = false;
                    roGrid.ActiveRow = null;
                }
                SetGridWizardCaption(ref roGrid, vsCaptionColumnKey, vsCaption, vsFilterCaption);

            }
            catch (Exception)
            {

            }

        }

        internal static void SetGridWizardCaption(ref Infragistics.Win.UltraWinGrid.UltraGrid roGrid,
                                                   string vsCaptionColumnKey,
                                                   string vsCaption = "",
                                                   string vsFilterCaption = "filtrati")
        {

            try
            {

                if (roGrid.DisplayLayout.Bands[0].Columns.Exists(vsCaptionColumnKey) == true)
                {
                    roGrid.DisplayLayout.Bands[0].Columns[vsCaptionColumnKey].Header.Caption = roGrid.Rows.Count.ToString("#,##0");
                    if (vsCaption != "")
                    {
                        roGrid.DisplayLayout.Bands[0].Columns[vsCaptionColumnKey].Header.Caption += " " + vsCaption;
                    }
                    if (roGrid.Rows.FilteredInRowCount != roGrid.Rows.Count)
                    {
                        roGrid.DisplayLayout.Bands[0].Columns[vsCaptionColumnKey].Header.Caption += " (" + roGrid.Rows.FilteredInRowCount.ToString("#,##0");
                        if (vsFilterCaption != "")
                        {
                            roGrid.DisplayLayout.Bands[0].Columns[vsCaptionColumnKey].Header.Caption += " " + vsFilterCaption;
                        }
                        roGrid.DisplayLayout.Bands[0].Columns[vsCaptionColumnKey].Header.Caption += ")";
                    }
                }

            }
            catch (Exception)
            {

            }

        }
        internal static void SetGridWizardCaption(ref ucEasyGrid roGrid,
                                                   string vsCaptionColumnKey,
                                                   string vsCaption = "",
                                                   string vsFilterCaption = "filtrati")
        {

            try
            {

                if (roGrid.DisplayLayout.Bands[0].Columns.Exists(vsCaptionColumnKey) == true)
                {
                    roGrid.DisplayLayout.Bands[0].Columns[vsCaptionColumnKey].Header.Caption = roGrid.Rows.Count.ToString("#,##0");
                    if (vsCaption != "")
                    {
                        roGrid.DisplayLayout.Bands[0].Columns[vsCaptionColumnKey].Header.Caption += " " + vsCaption;
                    }
                    if (roGrid.Rows.FilteredInRowCount != roGrid.Rows.Count)
                    {
                        roGrid.DisplayLayout.Bands[0].Columns[vsCaptionColumnKey].Header.Caption += " (" + roGrid.Rows.FilteredInRowCount.ToString("#,##0");
                        if (vsFilterCaption != "")
                        {
                            roGrid.DisplayLayout.Bands[0].Columns[vsCaptionColumnKey].Header.Caption += " " + vsFilterCaption;
                        }
                        roGrid.DisplayLayout.Bands[0].Columns[vsCaptionColumnKey].Header.Caption += ")";
                    }
                }

            }
            catch (Exception)
            {

            }

        }

        internal static void InitUltraComboEditorTVCheck(ref Infragistics.Win.UltraWinEditors.UltraComboEditor uce)
        {
            SetUltraComboEditorLayout(ref uce);
            uce.SortStyle = Infragistics.Win.ValueListSortStyle.AscendingByValue;
            uce.Items.Add(Enums.EnumCheckTVSelezione.tutti, "Tutti");
            uce.Items.Add(Enums.EnumCheckTVSelezione.selezionati, "Selezionati");
            uce.Items.Add(Enums.EnumCheckTVSelezione.non_selezionati, "NON Selezionati");
            uce.SelectedIndex = 0;
        }

        internal static void FiltraTVCheck(ref Infragistics.Win.UltraWinEditors.UltraComboEditor uce,
   ref Infragistics.Win.UltraWinTree.UltraTree utv)
        {
            Enums.EnumCheckTVSelezione sel = Enums.EnumCheckTVSelezione.tutti;
            if (uce.SelectedItem != null)
                sel = (Enums.EnumCheckTVSelezione)uce.SelectedItem.DataValue;

            foreach (var tvnode in utv.Nodes)
            {
                switch (sel)
                {
                    case Enums.EnumCheckTVSelezione.selezionati:
                        tvnode.Visible = (tvnode.CheckedState == CheckState.Checked);
                        break;
                    case Enums.EnumCheckTVSelezione.non_selezionati:
                        tvnode.Visible = (tvnode.CheckedState != CheckState.Checked);
                        break;

                    case Enums.EnumCheckTVSelezione.tutti:
                    default:
                        tvnode.Visible = true;
                        break;
                }
            }
        }

        #endregion

        #region System.Windows.Forms

        internal static void InitializeSaveFileDialog(ref System.Windows.Forms.SaveFileDialog Dialogo)
        {
            Dialogo.Filter = @"Microsoft Office Excel Workbook (*.xls)|*.xls";
            Dialogo.FilterIndex = 1;
        }

        internal static void SetControls(Control.ControlCollection oControls, bool bEnable)
        {

            foreach (Control c in oControls)
            {

                try
                {

                    if (c.GetType() == typeof(TextBox))
                        ((TextBox)c).ReadOnly = !bEnable;

                    if (c.GetType() == typeof(CheckBox))
                        c.Enabled = bEnable;

                    if (c.GetType() == typeof(DateTimePicker))
                        c.Enabled = bEnable;

                    if (c.GetType() == typeof(RadioButton))
                        c.Enabled = bEnable;

                    if (c.GetType() == typeof(Infragistics.Win.UltraWinEditors.UltraComboEditor))
                        ((Infragistics.Win.UltraWinEditors.UltraComboEditor)c).ReadOnly = !bEnable;

                    if (c.GetType() == typeof(Infragistics.Win.UltraWinEditors.UltraCheckEditor))
                        ((Infragistics.Win.UltraWinEditors.UltraCheckEditor)c).Enabled = bEnable;

                    if (c.GetType() == typeof(ReportManager.ReportHandler.ReportLinkConfig))
                        ((ReportManager.ReportHandler.ReportLinkConfig)c).ReadOnly = !bEnable;

                    if (c.GetType() == typeof(Infragistics.Win.UltraWinEditors.UltraDateTimeEditor))
                        ((Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)c).ReadOnly = !bEnable;

                    if (c.GetType() == typeof(Infragistics.Win.UltraWinEditors.UltraColorPicker))
                    {
                        Infragistics.Win.UltraWinEditors.UltraColorPicker ucp = (Infragistics.Win.UltraWinEditors.UltraColorPicker)c;
                        ucp.ReadOnly = !bEnable;
                        if (ucp.ButtonsRight.Count > 0)
                        {
                            foreach (Infragistics.Win.UltraWinEditors.EditorButton ucpbutton in ucp.ButtonsRight)
                            {
                                ucpbutton.Enabled = bEnable;
                            }
                        }
                        if (ucp.ButtonsLeft.Count > 0)
                        {
                            foreach (Infragistics.Win.UltraWinEditors.EditorButton ucpbutton in ucp.ButtonsLeft)
                            {
                                ucpbutton.Enabled = bEnable;
                            }
                        }
                    }

                    if (c.GetType() == typeof(Infragistics.Win.UltraWinEditors.UltraTextEditor))
                        ((Infragistics.Win.UltraWinEditors.UltraTextEditor)c).Enabled = bEnable;

                    if (c.GetType() == typeof(Infragistics.Win.UltraWinMaskedEdit.UltraMaskedEdit))
                        ((Infragistics.Win.UltraWinMaskedEdit.UltraMaskedEdit)c).Enabled = bEnable;

                    if (c.GetType() == typeof(Infragistics.Win.Misc.UltraGroupBox))
                        SetControls(c.Controls, bEnable);

                    if (c.GetType() == typeof(Infragistics.Win.UltraWinTabControl.UltraTabControl))
                    {
                        SetControls(c.Controls, bEnable);
                    }

                    if (c.GetType() == typeof(Infragistics.Win.UltraWinTabControl.UltraTabPageControl))
                        SetControls(c.Controls, bEnable);

                    if (c.GetType() == typeof(GroupBox))
                        SetControls(c.Controls, bEnable);

                    if (c.GetType() == typeof(ucEasyTableLayoutPanel))
                        SetControls(c.Controls, bEnable);

                    if (c.GetType() == typeof(TableLayoutPanel))
                        SetControls(c.Controls, bEnable);

                    if (c.GetType() == typeof(UnicodeSrl.ScciCore.ucMultiSelect))
                        c.Enabled = bEnable;

                    if (c.GetType() == typeof(UnicodeSrl.ScciManagement.ucMultiSelectPlus))
                        c.Enabled = bEnable;

                    if (c.GetType() == typeof(UnicodeSrl.ScciCore.ucPictureSelect))
                        c.Enabled = bEnable;

                    if (c.GetType() == typeof(Panel))
                        c.Enabled = bEnable;

                    if (c.GetType() == typeof(UnicodeSrl.ScciCore.ucRichTextBox))
                        c.Enabled = bEnable;

                    if (c.GetType() == typeof(XmlEditor))
                        ((XmlEditor)c).ReadOnly = !bEnable;

                }
                catch (Exception)
                {
                }

            }

        }

        #endregion

        #region Resource e Image

        internal static string GetNameResource(string KeyName, Enums.EnumImageSize vImageSize)
        {

            switch (KeyName)
            {
                case GC_WORD:
                    return Risorse.GC_WORD;

                case GC_ESCI:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_ESCI_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_ESCI_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_ESCI_256;
                        default: return Risorse.GC_ESCI_256;
                    }

                case GC_CHIUDIFINESTRE:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_CLOSE_ALL_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_CLOSE_ALL_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_CLOSE_ALL_256;
                        default: return Risorse.GC_CLOSE_ALL_256;
                    }

                case GC_FOLDER:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_FOLDER_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_FOLDER_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_FOLDER_256;
                        default: return Risorse.GC_FOLDER_256;
                    }

                case GC_SYMBOL_CHECK:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_SYMBOL_CHECK_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_SYMBOL_CHECK_16;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_SYMBOL_CHECK_16;
                        default: return Risorse.GC_SYMBOL_CHECK_16;
                    }

                case GC_SYMBOL_RESTRICTED:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_SYMBOL_RESTRICTED_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_SYMBOL_RESTRICTED_16;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_SYMBOL_RESTRICTED_16;
                        default: return Risorse.GC_SYMBOL_RESTRICTED_16;
                    }

                case GC_NUOVO:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_NUOVO_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_NUOVO_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_NUOVO_256;
                        default: return Risorse.GC_NUOVO_256;
                    }

                case GC_MODIFICA:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_MODIFICA_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_MODIFICA_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_MODIFICA_256;
                        default: return Risorse.GC_MODIFICA_256;
                    }

                case GC_MODIFICAVERSIONE:
                case GC_MODIFICAVERSIONECORRENTE:
                case GC_CONVERSIONESCHEDE:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_MODIFICAVERSIONE_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_MODIFICAVERSIONE_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_MODIFICAVERSIONE_256;
                        default: return Risorse.GC_MODIFICAVERSIONE_256;
                    }

                case GC_ELIMINA:
                case TV_CARTELLACANCELLA:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_ELIMINA_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_ELIMINA_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_ELIMINA_256;
                        default: return Risorse.GC_ELIMINA_256;
                    }

                case GC_VISUALIZZA:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_VISUALIZZA_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_VISUALIZZA_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_VISUALIZZA_256;
                        default: return Risorse.GC_VISUALIZZA_256;
                    }

                case GC_STAMPA:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_STAMPA_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_STAMPA_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_STAMPA_256;
                        default: return Risorse.GC_STAMPA_256;
                    }

                case GC_COPIA:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_COPIA_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_COPIA_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_COPIA_256;
                        default: return Risorse.GC_COPIA_256;
                    }

                case GC_AGGIORNA:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_AGGIORNA_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_AGGIORNA_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_AGGIORNA_256;
                        default: return Risorse.GC_AGGIORNA_256;
                    }

                case GC_ESPORTA:
                case GC_ESPORTAXML:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_ESPORTA_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_ESPORTA_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_ESPORTA_256;
                        default: return Risorse.GC_ESPORTA_256;
                    }

                case GC_IMPORTAXML:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_IMPORTA_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_IMPORTA_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_IMPORTA_256;
                        default: return Risorse.GC_ESPORTA_256;
                    }

                case GC_GENERA:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_GENERA_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_GENERA_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_GENERA_256;
                        default: return Risorse.GC_GENERA_256;
                    }

                case GC_CONFIGURAZIONE:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_CONFIGURAZIONE_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_CONFIGURAZIONE_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_CONFIGURAZIONE_256;
                        default: return Risorse.GC_CONFIGURAZIONE_256;
                    }

                case GC_CONFIGCE:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_LOGOCE_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_LOGOCE_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_LOGOCE_256;
                        default: return Risorse.GC_LOGOCE_256;

                    }
                case GC_CONTATORI:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_CONTATORI_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_CONTATORI_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_CONTATORI_256;
                        default: return Risorse.GC_CONTATORI_256;
                    }

                case GC_FILTRISPECIALI:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_FILTRISPECIALI_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_FILTRISPECIALI_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_FILTRISPECIALI_256;
                        default: return Risorse.GC_FILTRISPECIALI_256;
                    }

                case GC_CONFIGURAZIONE_AMBIENTE:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_CONFIGURAZIONE_TABLE_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_CONFIGURAZIONE_TABLE_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_CONFIGURAZIONE_TABLE_256;
                        default: return Risorse.GC_CONFIGURAZIONE_TABLE_256;
                    }

                case GC_AMMINISTRAZIONE:
                case GC_CONSULTAEPISODIO:
                case GC_MANUTENZIONEDATI:
                case GC_ELABORAZIONI:
                case GC_NORMALIZZAZIONE:
                case GC_DOCUMENTIFIRMATI:
                case GC_REPORTSTORICIZZATI:
                case TV_SPOSTASINGOLOMENU:
                case TV_SCHEDAGENERARTFGRUPPO:
                case TV_SPOSTAGRUPPOMENU:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_AMMINISTRAZIONE_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_AMMINISTRAZIONE_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_AMMINISTRAZIONE_256;
                        default: return Risorse.GC_AMMINISTRAZIONE_256;
                    }

                case GC_LOGIN:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_LOGIN_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_LOGIN_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_LOGIN_256;
                        default: return Risorse.GC_LOGIN_256;
                    }

                case GC_MODULI:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_MODULI_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_MODULI_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_MODULI_256;
                        default: return Risorse.GC_MODULI_256;
                    }

                case GC_RUOLI:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_RUOLI_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_RUOLI_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_RUOLI_256;
                        default: return Risorse.GC_RUOLI_256;
                    }

                case GC_UNITAATOMICHE:
                case GC_UNITAATOMICHEMENU:
                case GC_UNITAATOMICHETREEVIEW:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_UNITAATOMICHE_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_UNITAATOMICHE_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_UNITAATOMICHE_256;
                        default: return Risorse.GC_UNITAATOMICHE_256;
                    }

                case GC_UNITAOPERATIVE:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_UNITAOPERATIVA_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_UNITAOPERATIVA_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_UNITAOPERATIVA_256;
                        default: return Risorse.GC_UNITAATOMICHE_256;
                    }

                case GC_AZIENDE:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_AZIENDE_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_AZIENDE_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_AZIENDE_256;
                        default: return Risorse.GC_AZIENDE_256;
                    }

                case GC_DIARI:
                case GC_TIPODIARIO:
                case GC_STATODIARIO:
                case GC_DIARIOMEDICO:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_DIARIOMEDICO_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_DIARIOMEDICO_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_DIARIOMEDICO_256;
                        default: return Risorse.GC_DIARIOMEDICO_256;
                    }

                case GC_DIARIOINFERMIERISTICO:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_DIARIOINFERMIERISTICO_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_DIARIOINFERMIERISTICO_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_DIARIOINFERMIERISTICO_256;
                        default: return Risorse.GC_DIARIOINFERMIERISTICO_256;
                    }

                case GC_PARAMETRIVITALI:
                case GC_PARAMETRIVITALISTATO:
                case GC_PARAMETRIVITALITIPO:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_PARAMETRIVITALI_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_PARAMETRIVITALI_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_PARAMETRIVITALI_256;
                        default: return Risorse.GC_PARAMETRIVITALI_256;
                    }

                case GC_CONSEGNE:
                case GC_CONSEGNASTATO:
                case GC_CONSEGNATIPO:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_CONSEGNE_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_CONSEGNE_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_CONSEGNE_256;
                        default: return Risorse.GC_CONSEGNE_256;
                    }

                case GC_CONSEGNEPAZIENTE:
                case GC_CONSEGNAPAZIENTESTATO:
                case GC_CONSEGNAPAZIENTESTATORUOLI:
                case GC_CONSEGNAPAZIENTETIPO:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_CONSEGNEPAZIENTE_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_CONSEGNEPAZIENTE_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_CONSEGNEPAZIENTE_256;
                        default: return Risorse.GC_CONSEGNEPAZIENTE_256;
                    }

                case GC_CONSENSI:
                case GC_CONSENSOSTATICALCOLATI:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_CONSENSI_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_CONSENSI_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_CONSENSI_256;
                        default: return Risorse.GC_CONSENSI_256;
                    }

                case GC_EVIDENZACLINICA:
                case GC_EVIDENZACLINICAMENU:
                case GC_EVIDENZACLINICASTATI:
                case GC_EVIDENZACLINICASTATIVISIONE:
                case TV_CANCELLAEVIDENZACLINICA:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_EVIDENZACLINICA_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_EVIDENZACLINICA_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_EVIDENZACLINICA_256;
                        default: return Risorse.GC_EVIDENZACLINICA_256;
                    }

                case GC_TESTIPREDEFINITI:
                case GC_TESTINOTEPREDEFINITI:
                case GC_TESTIPREDEFINITIMENU:
                case GC_TESTIPREDEFINITITREEVIEW:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_TESTOPREDEFINITO_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_TESTOPREDEFINITO_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_TESTOPREDEFINITO_256;
                        default: return Risorse.GC_TESTOPREDEFINITO_256;
                    }

                case GC_REPORT:
                case GC_REPORTMENU:
                case GC_REPORTTREEVIEW:
                case GC_FORMATOREPORT:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_REPORT_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_REPORT_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_REPORT_256;
                        default: return Risorse.GC_REPORT_256;
                    }

                case GC_MASCHERE:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_MASCHERA_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_MASCHERA_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_MASCHERA_256;
                        default: return Risorse.GC_MASCHERA_256;
                    }

                case GC_ICONE:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_ICONA_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_ICONA_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_ICONA_256;
                        default: return Risorse.GC_ICONA_256;
                    }

                case GC_NEWS:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_NEWS_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_NEWS_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_NEWS_256;
                        default: return Risorse.GC_ICONA_256;
                    }

                case TV_AZIONE:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_MODIFICA_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_MODIFICA_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_MODIFICA_256;
                        default: return Risorse.GC_MODIFICA_256;
                    }

                case TV_VOCE:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_VISUALIZZA_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_VISUALIZZA_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_VISUALIZZA_256;
                        default: return Risorse.GC_VISUALIZZA_256;
                    }

                case GC_VIESOMMINISTRAZIONE:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_VIASOMMINISTRAZIONE_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_VIASOMMINISTRAZIONE_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_VIASOMMINISTRAZIONE_256;
                        default: return Risorse.GC_VIASOMMINISTRAZIONE_256;
                    }

                case GC_PROTOCOLLIATTIVITA:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_PROTOCOLLOATTIVITA_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_PROTOCOLLOATTIVITA_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_PROTOCOLLOATTIVITA_256;
                        default: return Risorse.GC_PROTOCOLLOATTIVITA_256;
                    }

                case GC_PROTOCOLLI:
                case GC_PROTOCOLLITEMPI:
                case GC_PROFILITERAPIEFARMACOLOGICHE:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_PRESCRIZIONE_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_PRESCRIZIONE_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_PRESCRIZIONE_256;
                        default: return Risorse.GC_PRESCRIZIONE_256;
                    }

                case GC_TIPOPRESCRIZIONE:
                case GC_TIPOPRESCRIZIONEMENU:
                case GC_STATOPRESCRIZIONE:
                case GC_STATOPRESCRIZIONETEMPI:
                case GC_STATOCONTINUAZIONE:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_PRESCRIZIONE_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_PRESCRIZIONE_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_PRESCRIZIONE_256;
                        default: return Risorse.GC_PRESCRIZIONE_256;
                    }

                case GC_ALLEGATIMENU:
                case GC_ALLEGATITIPO:
                case GC_ALLEGATIFORMATO:
                case GC_ALLEGATISTATO:
                case GC_ENTITAALLEGATI:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_ALLEGATI_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_ALLEGATI_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_ALLEGATI_256;
                        default: return Risorse.GC_ALLEGATI_256;
                    }

                case GC_APPUNTAMENTI:
                case GC_APPUNTAMENTISTATO:
                case GC_APPUNTAMENTITIPO:
                case GC_APPUNTAMENTISTATOAGENDE:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_APPUNTAMENTO_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_APPUNTAMENTO_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_APPUNTAMENTO_256;
                        default: return Risorse.GC_APPUNTAMENTO_256;
                    }

                case GC_TASKINFERMIERISTICI:
                case GC_TASKINFERMIERISTICITIPO:
                case GC_TASKINFERMIERISTICISTATO:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_TASKINFERMIERISTICO_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_TASKINFERMIERISTICO_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_TASKINFERMIERISTICO_256;
                        default: return Risorse.GC_TASKINFERMIERISTICO_256;
                    }

                case GC_STANZE:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_STANZA_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_STANZA_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_STANZA_256;
                        default: return Risorse.GC_STANZA_256;
                    }

                case GC_SCHEDE:
                case GC_SCHEDELISTA:
                case GC_SCHEDETIPO:
                case GC_SCHEDESTATO:
                case GC_SCHEDESTATOCALCOLATO:
                case GC_SCHEDETREEVIEW:
                case TV_SBLOCCOSCHEDA:
                case TV_ANNULLACANCSCHEDA:
                case TV_RIPRISTINOREVISIONESCHEDA:
                case TV_ALLINEAADT:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_SCHEDA_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_SCHEDA_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_SCHEDA_256;
                        default: return Risorse.GC_SCHEDA_256;
                    }

                case GC_SCHEDEESPORTA:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_ESPORTA_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_ESPORTA_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_SCHEDA_256;
                        default: return Risorse.GC_ESPORTA_256;
                    }

                case GC_CARTELLAMENU:
                case GC_STATOCARTELLA:
                case GC_STATOCARTELLAINFO:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_CARTELLACLINICA_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_CARTELLACLINICA_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_CARTELLACLINICA_256;
                        default: return Risorse.GC_EPISODIO_256;
                    }


                case GC_STATOCARTELLAINVISIONE:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_OCCHIOAPERTO_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_OCCHIOAPERTO_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_OCCHIOAPERTO_256;
                        default: return Risorse.GC_OCCHIOAPERTO_256;
                    }

                case GC_TIPOEPISODIO:
                case GC_TIPOEPISODIOMENU:
                case GC_STATOTRASFERIMENTO:
                case GC_STATOEPISODIO:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_EPISODIO_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_EPISODIO_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_EPISODIO_256;
                        default: return Risorse.GC_EPISODIO_256;
                    }

                case GC_LETTIGESTIONE:
                case GC_LETTI:
                case GC_ASSLETTIUA:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_LETTO_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_LETTO_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_LETTO_256;
                        default: return Risorse.GC_LETTO_256;
                    }

                case GC_CDSSGESTIONE:
                case GC_CDSSAZIONI:
                case GC_CDSSPLUGINS:
                case GC_CDSSSTRUTTURA:
                case GC_CDSSSTRUTTURARUOLI:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_CDSSPLUGIN_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_CDSSPLUGIN_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_CDSSPLUGIN_256;
                        default: return Risorse.GC_CDSSPLUGIN_256;
                    }

                case GC_INTEGRAZIONIGESTIONE:
                case GC_INTEGRAZIONIDESTINATARI:
                case GC_INTEGRAZIONICAMPI:
                case GC_INTEGRAZIONIALTRE:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_INTEGRAZIONI_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_INTEGRAZIONI_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_INTEGRAZIONI_256;
                        default: return Risorse.GC_INTEGRAZIONI_256;
                    }


                case GC_SETTORI:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_SETTORI_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_SETTORI_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_SETTORI_256;
                        default: return Risorse.GC_SETTORI_256;
                    }

                case GC_DIZIONARI:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_DIZIONARI_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_DIZIONARI_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_DIZIONARI_256;
                        default: return Risorse.GC_DIZIONARI_256;
                    }

                case GC_DIZIONARIOQUICK:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_DIZIONARIQUICK_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_DIZIONARIQUICK_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_DIZIONARIQUICK_256;
                        default: return Risorse.GC_DIZIONARIQUICK_256;
                    }

                case GC_DIZIONARIOCSV:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_DIZIONARICSV_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_DIZIONARICSV_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_DIZIONARICSV_256;
                        default: return Risorse.GC_DIZIONARICSV_256;
                    }

                case GC_AGENDE:
                case GC_AGENDEMENU:
                case GC_TIPOAGENDA:
                case GC_FESTIVITA:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_AGENDA_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_AGENDA_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_AGENDA_256;
                        default: return Risorse.GC_AGENDA_256;
                    }

                case GC_ALERTALLERGIEANAMNESI:
                case GC_ALERTALLERGIEANAMNESITIPO:
                case GC_ALERTALLERGIEANAMNESISTATO:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_ALERTALLERGIA_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_ALERTALLERGIA_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_ALERTALLERGIA_256;
                        default: return Risorse.GC_ALERTALLERGIA_256;
                    }

                case GC_ALERTGENERICI:
                case GC_ALERTGENERICITIPO:
                case GC_ALERTGENERICISTATO:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_ALERTGENERICO_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_ALERTGENERICO_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_ALERTGENERICO_256;
                        default: return Risorse.GC_ALERTGENERICO_256;
                    }

                case GC_SISTEMI:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_SISTEMI_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_SISTEMI_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_SISTEMI_256;
                        default: return Risorse.GC_SISTEMI_256;
                    }

                case GC_ORDINIMENU:
                case GC_ORDINITIPO:
                case GC_ORDINISTATO:
                case GC_ORDINIFORMULE:
                case GC_ORDINIATTRIBUTI:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_ORDINE_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_ORDINE_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_ORDINE_256;
                        default: return Risorse.GC_ORDINE_256;
                    }

                case GC_SEZIONIFUT:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_FOGLIOUNICO_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_FOGLIOUNICO_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_FOGLIOUNICO_256;
                        default: return Risorse.GC_FOGLIOUNICO_256;
                    }

                case TV_SPOSTASINGOLOAPPPAZAMB:
                case TV_SPOSTASINGOLOAPPPAZRO:
                case TV_SPOSTASINGOLOAPPAMBTOEPI:
                case TV_SPOSTASINGOLOAPPEPITOAMB:
                case TV_SPOSTASINGOLODIARIOCLINICO:
                case TV_SPOSTASINGOLOEVIDENZACLINICA:
                case TV_SPOSTASINGOLOPRESCRIZIONE:
                case TV_SPOSTASINGOLOPARAMETRIVITALI:
                case TV_SPOSTASINGOLOSCHEDAPAZ:
                case TV_SPOSTASINGOLOSCHEDAEPI:
                case TV_SPOSTASINGOLOTASKINF:
                case TV_SPOSTASINGOLOALERTGENERICO:
                case TV_SPOSTAGRUPPOAPPPAZAMB:
                case TV_SPOSTAGRUPPOAPPPAZRO:
                case TV_SPOSTAGRUPPODIARIOCLINICO:
                case TV_SPOSTAGRUPPOEVIDENZACLINICA:
                case TV_SPOSTAGRUPPOPRESCRIZIONE:
                case TV_SPOSTAGRUPPOPARAMETRIVITALI:
                case TV_SPOSTAGRUPPOSCHEDAPAZ:
                case TV_SPOSTAGRUPPOSCHEDAEPI:
                case TV_SPOSTAGRUPPOTASKINF:
                case TV_SPOSTAGRUPPOALERTGENRICO:
                case TV_SCHEDAGENERARTFSINGOLO:
                case TV_SCHEDAGENERARTFMULTIPLO:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_FRECCIA_DX_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_FRECCIA_DX_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_FRECCIA_DX_256;
                        default: return Risorse.GC_FRECCIA_DX_256;
                    }

                case TV_CARTELLARIAPRI:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_CARTELLACLINICA_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_CARTELLACLINICA_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_CARTELLACLINICA_256;
                        default: return Risorse.GC_CARTELLACLINICA_256;
                    }

                case GC_BANCHEDATI:
                case GC_BANCHEDATILISTA:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_EBM_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_EBM_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_EBM_256;
                        default: return Risorse.GC_EBM_256;
                    }

                case GC_CONFIGURAZIONE_PC:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_CONFIG_PC_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_CONFIG_PC_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_CONFIG_PC_256;
                        default: return Risorse.GC_CONFIG_PC_256;
                    }

                case GC_SELEZIONI:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_SELEZIONI_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_SELEZIONI_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_SELEZIONI_256;
                        default: return Risorse.GC_SELEZIONI_256;
                    }

                case GC_SCREEN:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_SCREEN_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_SCREEN_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_SCREEN_256;
                        default: return Risorse.GC_SCREEN_256;
                    }

                case GC_CONVERSIONE:
                    switch (vImageSize)
                    {
                        case Enums.EnumImageSize.isz16: return Risorse.GC_CONVERSIONE_16;
                        case Enums.EnumImageSize.isz32: return Risorse.GC_CONVERSIONE_32;
                        case Enums.EnumImageSize.isz256: return Risorse.GC_CONVERSIONE_256;
                        default: return Risorse.GC_CONVERSIONE_256;
                    }

                default:
                    return "";

            }

        }

        public static string GetKeyNameFromEntitaCode(string codiceEntita)
        {
            return GetKeyNameFromEntitaCode((Scci.Enums.EnumEntita)Enum.Parse(typeof(Scci.Enums.EnumEntita), codiceEntita));
        }
        public static string GetKeyNameFromEntitaCode(Scci.Enums.EnumEntita entita)
        {
            string sReturn = GC_FOLDER;
            switch (entita)
            {
                case Scci.Enums.EnumEntita.DCL:
                    sReturn = GC_DIARIOMEDICO;
                    break;
                case Scci.Enums.EnumEntita.PRF:
                    sReturn = GC_TIPOPRESCRIZIONE;
                    break;
                case Scci.Enums.EnumEntita.PVT:
                    sReturn = GC_PARAMETRIVITALI;
                    break;
                case Scci.Enums.EnumEntita.CSG:
                    sReturn = GC_CONSEGNE;
                    break;
                case Scci.Enums.EnumEntita.RPT:
                    sReturn = GC_REPORT;
                    break;
                case Scci.Enums.EnumEntita.WKI:
                    sReturn = GC_TASKINFERMIERISTICI;
                    break;
                case Scci.Enums.EnumEntita.SCH:
                    sReturn = GC_SCHEDE;
                    break;
                case Scci.Enums.EnumEntita.SCR:
                    sReturn = GC_SCREEN;
                    break;
                case Scci.Enums.EnumEntita.FLS:
                    sReturn = GC_FILTRISPECIALI;
                    break;
                default:
                    sReturn = GC_VISUALIZZA;
                    break;
            }

            return sReturn;
        }

        internal static Bitmap MergeTwoImages(Image firstImage, Image secondImage, Enums.EnumImageSize eSize)
        {

            Bitmap target = null;

            try
            {

                Bitmap source1 = null;
                Bitmap source2 = null;

                switch (eSize)
                {
                    case Enums.EnumImageSize.isz16:
                        source1 = MyStatics.ResizeBitmap(firstImage, 16, 16);
                        source2 = MyStatics.ResizeBitmap(secondImage, 16, 16);
                        target = new Bitmap(16, 16, PixelFormat.Format32bppArgb);
                        break;

                    case Enums.EnumImageSize.isz32:
                        source1 = MyStatics.ResizeBitmap(firstImage, 32, 32);
                        source2 = MyStatics.ResizeBitmap(secondImage, 32, 32);
                        target = new Bitmap(32, 32, PixelFormat.Format32bppArgb);
                        break;

                    case Enums.EnumImageSize.isz48:
                        source1 = MyStatics.ResizeBitmap(firstImage, 48, 48);
                        source2 = MyStatics.ResizeBitmap(secondImage, 48, 48);
                        target = new Bitmap(48, 48, PixelFormat.Format32bppArgb);
                        break;

                    case Enums.EnumImageSize.isz256:
                        source1 = MyStatics.ResizeBitmap(firstImage, 256, 256);
                        source2 = MyStatics.ResizeBitmap(secondImage, 256, 256);
                        target = new Bitmap(256, 256, PixelFormat.Format32bppArgb);
                        break;

                }

                var graphics = Graphics.FromImage(target);
                graphics.CompositingMode = CompositingMode.SourceOver;
                graphics.DrawImage(source1, 0, 0);
                graphics.DrawImage(source2, 0, 0);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            return target;

        }

        internal static Bitmap ResizeBitmap(Image b, int nWidth, int nHeight)
        {
            Bitmap result = new Bitmap(nWidth, nHeight);
            using (Graphics g = Graphics.FromImage((Image)result))
                g.DrawImage(b, 0, 0, nWidth, nHeight);
            return result;
        }

        #endregion

        #region PopUp e Zoom

        public static string GetDataNameModalityFormPU(Enums.EnumModalityPopUp Modality)
        {

            switch (Modality)
            {

                case Enums.EnumModalityPopUp.mpNuovo:
                    return @"Nuovo";

                case Enums.EnumModalityPopUp.mpModifica:
                    return @"Modifica";

                case Enums.EnumModalityPopUp.mpCancella:
                    return @"Cancella";

                case Enums.EnumModalityPopUp.mpVisualizza:
                    return @"Visualizza";

                case Enums.EnumModalityPopUp.mpCopia:
                    return @"Copia";

                default:
                    return @"";

            }

        }

        public static DialogResult ActionDataNameFormPU(Enums.EnumDataNames DataName, Enums.EnumModalityPopUp Modality,
                                                        Icon vIcon, Image vImage, string vsText,
                                                        ref Infragistics.Win.UltraWinGrid.UltraGridRow UltraRow)
        {

            DialogResult dialogResult = DialogResult.Cancel;

            try
            {

                switch (DataName)
                {

                    case Enums.EnumDataNames.T_Schede:

                        using (frmPUSchede oPUSchede = new frmPUSchede())
                        {
                            oPUSchede.ViewDataNamePU = DataName;
                            oPUSchede.ViewModality = Modality;
                            oPUSchede.ViewText = vsText;
                            oPUSchede.ViewIcon = vIcon;
                            oPUSchede.ViewImage = vImage;
                            PUDataBindings oPUDataBindingsSchede = oPUSchede.ViewDataBindings;
                            DataBase.SetDataBinding(ref oPUDataBindingsSchede, DataName, Modality, ref UltraRow);
                            oPUSchede.ViewInit();
                            oPUSchede.ShowDialog();
                        }

                        return DialogResult.OK;

                    case Enums.EnumDataNames.T_SchedeVersioni:

                        using (frmPUSchedeVersioni oPUSchedeVersioni = new frmPUSchedeVersioni())
                        {
                            oPUSchedeVersioni.ViewDataNamePU = DataName;
                            oPUSchedeVersioni.ViewModality = Modality;
                            oPUSchedeVersioni.ViewText = vsText;
                            oPUSchedeVersioni.ViewIcon = vIcon;
                            oPUSchedeVersioni.ViewImage = vImage;
                            PUDataBindings oPUDataBindingsSchedeVersioni = oPUSchedeVersioni.ViewDataBindings;
                            DataBase.SetDataBinding(ref oPUDataBindingsSchedeVersioni, DataName, Modality, ref UltraRow);
                            oPUSchedeVersioni.ViewInit();
                            oPUSchedeVersioni.ShowDialog();
                        }

                        return DialogResult.OK;

                    case Enums.EnumDataNames.T_ProtocolliAttivita:
                        using (frmPUProtocolliAttivita oPUProtocolliAttivita = new frmPUProtocolliAttivita())
                        {
                            oPUProtocolliAttivita.ViewDataNamePU = DataName;
                            oPUProtocolliAttivita.ViewModality = Modality;
                            oPUProtocolliAttivita.ViewText = vsText;
                            oPUProtocolliAttivita.ViewIcon = vIcon;
                            oPUProtocolliAttivita.ViewImage = vImage;
                            PUDataBindings oPUDataBindingsProtocolliAttivita = oPUProtocolliAttivita.ViewDataBindings;
                            DataBase.SetDataBinding(ref oPUDataBindingsProtocolliAttivita, DataName, Modality, ref UltraRow);

                            oPUProtocolliAttivita.ViewInit();
                            oPUProtocolliAttivita.ShowDialog();

                            dialogResult = oPUProtocolliAttivita.DialogResult;
                        }

                        return dialogResult;

                    case Enums.EnumDataNames.T_EntitaAllegato:

                        using (frmEntitaAllegato oPUEntitaAllegato = new frmEntitaAllegato())
                        {
                            oPUEntitaAllegato.ViewDataNamePU = DataName;
                            oPUEntitaAllegato.ViewModality = Modality;
                            oPUEntitaAllegato.ViewText = vsText;
                            oPUEntitaAllegato.ViewIcon = vIcon;
                            oPUEntitaAllegato.ViewImage = vImage;
                            PUDataBindings oPUDataBindingsEntitaAllegato = oPUEntitaAllegato.ViewDataBindings;
                            DataBase.SetDataBinding(ref oPUDataBindingsEntitaAllegato, DataName, Modality, ref UltraRow);

                            oPUEntitaAllegato.ViewInit();
                            dialogResult = oPUEntitaAllegato.ShowDialog();
                        }

                        return dialogResult;

                    case Enums.EnumDataNames.T_TipoAgenda:
                    case Enums.EnumDataNames.T_Festivita:
                    case Enums.EnumDataNames.T_Agende:
                    case Enums.EnumDataNames.T_DCDecodifiche:
                    case Enums.EnumDataNames.T_TipoTaskInfermieristico:
                    case Enums.EnumDataNames.T_TipoAppuntamento:
                    case Enums.EnumDataNames.T_AssUAUOLetti:
                    case Enums.EnumDataNames.T_TipoAlertAllergiaAnamnesi:
                    case Enums.EnumDataNames.T_TipoAlertGenerico:
                    case Enums.EnumDataNames.T_Protocolli:
                    case Enums.EnumDataNames.T_SezioniFUT:
                    case Enums.EnumDataNames.T_CDSSPlugins:
                    case Enums.EnumDataNames.T_CDSSStruttura:
                    case Enums.EnumDataNames.T_CDSSStrutturaRuoli:
                    case Enums.EnumDataNames.T_EBM:
                    case Enums.EnumDataNames.T_Integra_Destinatari:
                    case Enums.EnumDataNames.T_Integra_Campi:
                    case Enums.EnumDataNames.T_FiltriSpeciali:
                    case Enums.EnumDataNames.T_AgendePeriodi:

                        using (frmPUView2 oPUView2 = new frmPUView2())
                        {
                            oPUView2.ViewDataNamePU = DataName;
                            oPUView2.ViewModality = Modality;
                            oPUView2.ViewText = vsText;
                            oPUView2.ViewIcon = vIcon;
                            oPUView2.ViewImage = vImage;
                            PUDataBindings oPUDataBindings2 = oPUView2.ViewDataBindings;
                            DataBase.SetDataBinding(ref oPUDataBindings2, DataName, Modality, ref UltraRow);
                            oPUView2.ViewInit();
                            dialogResult = oPUView2.ShowDialog();
                        }

                        return dialogResult;

                    case Enums.EnumDataNames.T_ConfigPC:

                        using (frmPUConfigPC oPUConfigPC = new frmPUConfigPC())
                        {
                            oPUConfigPC.ViewDataNamePU = DataName;
                            oPUConfigPC.ViewModality = Modality;
                            oPUConfigPC.ViewText = vsText;
                            oPUConfigPC.ViewIcon = vIcon;
                            oPUConfigPC.ViewImage = vImage;
                            PUDataBindings oPUDataBindingsConfigPC = oPUConfigPC.ViewDataBindings;
                            DataBase.SetDataBinding(ref oPUDataBindingsConfigPC, DataName, Modality, ref UltraRow);
                            oPUConfigPC.ViewInit();
                            dialogResult = oPUConfigPC.ShowDialog();
                        }

                        return dialogResult;

                    case Enums.EnumDataNames.T_Selezioni:


                        using (frmPUSelezioni oPUSel = new frmPUSelezioni())
                        {
                            oPUSel.ViewDataNamePU = DataName;
                            oPUSel.ViewModality = Modality;
                            oPUSel.ViewText = vsText;
                            oPUSel.ViewIcon = vIcon;
                            oPUSel.ViewImage = vImage;
                            PUDataBindings oPUSelDB = oPUSel.ViewDataBindings;
                            DataBase.SetDataBinding(ref oPUSelDB, DataName, Modality, ref UltraRow);
                            oPUSel.ViewInit();
                            dialogResult = oPUSel.ShowDialog();
                        }

                        return dialogResult;

                    case Enums.EnumDataNames.T_Screen:


                        using (frmPUScreen oPUScreen = new frmPUScreen())
                        {
                            oPUScreen.ViewDataNamePU = DataName;
                            oPUScreen.ViewModality = Modality;
                            oPUScreen.ViewText = vsText;
                            oPUScreen.ViewIcon = vIcon;
                            oPUScreen.ViewImage = vImage;
                            PUDataBindings oPUDataBindingsScreen = oPUScreen.ViewDataBindings;
                            DataBase.SetDataBinding(ref oPUDataBindingsScreen, DataName, Modality, ref UltraRow);

                            oPUScreen.ViewInit();
                            dialogResult = oPUScreen.ShowDialog();
                        }

                        return dialogResult;

                    case Enums.EnumDataNames.T_ProtocolliPrescrizioni:

                        using (frmPUProtocolliPrescrizioni oPUProtocolliPrescrizioni = new frmPUProtocolliPrescrizioni())
                        {
                            oPUProtocolliPrescrizioni.ViewDataNamePU = DataName;
                            oPUProtocolliPrescrizioni.ViewModality = Modality;
                            oPUProtocolliPrescrizioni.ViewText = vsText;
                            oPUProtocolliPrescrizioni.ViewIcon = vIcon;
                            oPUProtocolliPrescrizioni.ViewImage = vImage;
                            PUDataBindings oPUDataBindingsProtocolliPrescrizioni = oPUProtocolliPrescrizioni.ViewDataBindings;

                            DataBase.SetDataBinding(ref oPUDataBindingsProtocolliPrescrizioni, DataName, Modality, ref UltraRow);

                            oPUProtocolliPrescrizioni.ViewInit();
                            dialogResult = oPUProtocolliPrescrizioni.ShowDialog();
                        }

                        return dialogResult;

                    case Enums.EnumDataNames.T_TipoConsegna:
                    case Enums.EnumDataNames.T_TipoConsegnaPaziente:

                        using (frmPUTipoConsegna oPUTC = new frmPUTipoConsegna())
                        {
                            oPUTC.ViewDataNamePU = DataName;
                            oPUTC.ViewModality = Modality;
                            oPUTC.ViewText = vsText;
                            oPUTC.ViewIcon = vIcon;
                            oPUTC.ViewImage = vImage;
                            PUDataBindings oPUDataBindingsTC = oPUTC.ViewDataBindings;
                            DataBase.SetDataBinding(ref oPUDataBindingsTC, DataName, Modality, ref UltraRow);
                            oPUTC.ViewInit();
                            dialogResult = oPUTC.ShowDialog();
                        }


                        return dialogResult;

                    default:

                        using (frmPUView oPUView = new frmPUView())
                        {
                            oPUView.ViewDataNamePU = DataName;
                            oPUView.ViewModality = Modality;
                            oPUView.ViewText = vsText;
                            oPUView.ViewIcon = vIcon;
                            oPUView.ViewImage = vImage;
                            PUDataBindings oPUDataBindings = oPUView.ViewDataBindings;
                            DataBase.SetDataBinding(ref oPUDataBindings, DataName, Modality, ref UltraRow);
                            oPUView.ViewInit();
                            dialogResult = oPUView.ShowDialog();
                        }

                        return dialogResult;

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                return DialogResult.Cancel;
            }

        }

        public static DialogResult ActionDataNameFormPU(Enums.EnumDataNames DataName, Enums.EnumModalityPopUp Modality,
                                                        Icon vIcon, Image vImage, string vsText,
                                                        params string[] vsCodiciRecord)
        {

            try
            {
                DialogResult dialogResult = DialogResult.Cancel;

                switch (DataName)
                {
                    case Enums.EnumDataNames.T_Schede:

                        using (frmPUSchede oPUSchede = new frmPUSchede())
                        {
                            oPUSchede.ViewDataNamePU = DataName;
                            oPUSchede.ViewModality = Modality;
                            oPUSchede.ViewText = vsText;
                            oPUSchede.ViewIcon = vIcon;
                            oPUSchede.ViewImage = vImage;
                            PUDataBindings oPUDataBindingsSchede = oPUSchede.ViewDataBindings;
                            DataBase.SetDataBinding(ref oPUDataBindingsSchede, DataName, Modality, vsCodiciRecord);
                            oPUSchede.ViewInit();
                            oPUSchede.ShowDialog();
                        }

                        return DialogResult.OK;

                    case Enums.EnumDataNames.T_SchedeVersioni:

                        using (frmPUSchedeVersioni oPUSchedeVersioni = new frmPUSchedeVersioni())
                        {
                            oPUSchedeVersioni.ViewDataNamePU = DataName;
                            oPUSchedeVersioni.ViewModality = Modality;
                            oPUSchedeVersioni.ViewText = vsText;
                            oPUSchedeVersioni.ViewIcon = vIcon;
                            oPUSchedeVersioni.ViewImage = vImage;
                            PUDataBindings oPUDataBindingsSchedeVersioni = oPUSchedeVersioni.ViewDataBindings;
                            DataBase.SetDataBinding(ref oPUDataBindingsSchedeVersioni, DataName, Modality, vsCodiciRecord);
                            oPUSchedeVersioni.ViewInit();
                            dialogResult = oPUSchedeVersioni.ShowDialog();
                        }

                        return DialogResult.OK;

                    case Enums.EnumDataNames.T_AssUAIntestazioni:

                        using (frmPUView2 oPUView2 = new frmPUView2())
                        {
                            oPUView2.ViewDataNamePU = DataName;
                            oPUView2.ViewModality = Modality;
                            oPUView2.ViewText = vsText;
                            oPUView2.ViewIcon = vIcon;
                            oPUView2.ViewImage = vImage;
                            PUDataBindings oPUDataBindingsIntestazioni = oPUView2.ViewDataBindings;
                            DataBase.SetDataBinding(ref oPUDataBindingsIntestazioni, DataName, Modality, vsCodiciRecord);
                            oPUView2.ViewInit();
                            oPUView2.ShowDialog();
                        }
                        return DialogResult.OK;

                    default:
                        using (frmPUView oPUView = new frmPUView())
                        {
                            oPUView.ViewDataNamePU = DataName;
                            oPUView.ViewModality = Modality;
                            oPUView.ViewText = vsText;
                            oPUView.ViewIcon = vIcon;
                            oPUView.ViewImage = vImage;
                            PUDataBindings oPUDataBindings = oPUView.ViewDataBindings;
                            DataBase.SetDataBinding(ref oPUDataBindings, DataName, Modality, vsCodiciRecord);
                            oPUView.ViewInit();
                            dialogResult = oPUView.ShowDialog();
                        }

                        return dialogResult;

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                return DialogResult.Cancel;
            }

        }

        public static DialogResult ActionDataNameFormPU(Enums.EnumDataNames DataName, Enums.EnumModalityPopUp Modality,
                                                Icon vIcon, Image vImage, string vsText,
                                                ref Infragistics.Win.UltraWinGrid.UltraGridRow UltraRow, String CodPadre)
        {
            try
            {
                DialogResult dialogResult = DialogResult.Cancel;

                switch (DataName)
                {

                    case Enums.EnumDataNames.T_DCDecodificheValori:
                        using (frmPUView2 oPUViewDCDecodificheValori = new frmPUView2())
                        {
                            oPUViewDCDecodificheValori.ViewDataNamePU = DataName;
                            oPUViewDCDecodificheValori.ViewModality = Modality;
                            oPUViewDCDecodificheValori.ViewText = vsText;
                            oPUViewDCDecodificheValori.ViewIcon = vIcon;
                            oPUViewDCDecodificheValori.ViewImage = vImage;
                            PUDataBindings oPUDataBindingsDCDecodificheValori = oPUViewDCDecodificheValori.ViewDataBindings;
                            DataBase.SetDataBinding(ref oPUDataBindingsDCDecodificheValori, DataName, Modality, ref UltraRow);
                            if (Modality == Enums.EnumModalityPopUp.mpNuovo) oPUDataBindingsDCDecodificheValori.DataBindings.DataSet.Tables[0].Rows[0]["CodDec"] = CodPadre;
                            oPUViewDCDecodificheValori.ViewInit();
                            oPUViewDCDecodificheValori.ShowDialog();
                        }

                        return DialogResult.OK;

                    case Enums.EnumDataNames.T_ProtocolliTempi:

                        using (frmPUView2 oPUViewProtocolliTempi = new frmPUView2())
                        {
                            oPUViewProtocolliTempi.ViewDataNamePU = DataName;
                            oPUViewProtocolliTempi.ViewModality = Modality;
                            oPUViewProtocolliTempi.ViewText = vsText;
                            oPUViewProtocolliTempi.ViewIcon = vIcon;
                            oPUViewProtocolliTempi.ViewImage = vImage;
                            PUDataBindings oPUDataBindingsProtocolliTempi = oPUViewProtocolliTempi.ViewDataBindings;
                            DataBase.SetDataBinding(ref oPUDataBindingsProtocolliTempi, DataName, Modality, ref UltraRow);
                            if (Modality == Enums.EnumModalityPopUp.mpNuovo) oPUDataBindingsProtocolliTempi.DataBindings.DataSet.Tables[0].Rows[0]["CodProtocollo"] = CodPadre;
                            oPUViewProtocolliTempi.ViewInit();
                            oPUViewProtocolliTempi.ShowDialog();
                        }

                        return DialogResult.OK;

                    case Enums.EnumDataNames.T_ProtocolliAttivitaTempi:

                        using (frmPUProtocolliAttivitaTempi oPUProtocolliAttivitaTempi = new frmPUProtocolliAttivitaTempi())
                        {
                            oPUProtocolliAttivitaTempi.ViewDataNamePU = DataName;
                            oPUProtocolliAttivitaTempi.ViewModality = Modality;
                            oPUProtocolliAttivitaTempi.ViewText = vsText;
                            oPUProtocolliAttivitaTempi.ViewIcon = vIcon;
                            oPUProtocolliAttivitaTempi.ViewImage = vImage;
                            PUDataBindings oPUDataBindingsProtocolliAttivitaTempi = oPUProtocolliAttivitaTempi.ViewDataBindings;
                            DataBase.SetDataBinding(ref oPUDataBindingsProtocolliAttivitaTempi, DataName, Modality, ref UltraRow);

                            if (Modality == Enums.EnumModalityPopUp.mpNuovo)
                                oPUProtocolliAttivitaTempi.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["CodProtocolloAttivita"] = CodPadre;

                            oPUProtocolliAttivitaTempi.ViewInit();
                            dialogResult = oPUProtocolliAttivitaTempi.ShowDialog();
                        }

                        return dialogResult;

                    case Enums.EnumDataNames.T_AgendePeriodi:

                        using (frmPUView2 oPUViewAgendePeriodi = new frmPUView2())
                        {
                            oPUViewAgendePeriodi.ViewDataNamePU = DataName;
                            oPUViewAgendePeriodi.ViewModality = Modality;
                            oPUViewAgendePeriodi.ViewText = vsText;
                            oPUViewAgendePeriodi.ViewIcon = vIcon;
                            oPUViewAgendePeriodi.ViewImage = vImage;
                            PUDataBindings oPUDataBindingsAgendePeriodi = oPUViewAgendePeriodi.ViewDataBindings;
                            DataBase.SetDataBinding(ref oPUDataBindingsAgendePeriodi, DataName, Modality, ref UltraRow);
                            if (Modality == Enums.EnumModalityPopUp.mpNuovo) oPUDataBindingsAgendePeriodi.DataBindings.DataSet.Tables[0].Rows[0]["CodAgenda"] = CodPadre;
                            oPUViewAgendePeriodi.ViewInit();
                            oPUViewAgendePeriodi.ShowDialog();
                        }

                        return DialogResult.OK;

                    case Enums.EnumDataNames.T_ScreenTile:

                        using (frmPUScreentile oPUScreenTile = new frmPUScreentile())
                        {
                            oPUScreenTile.ViewDataNamePU = DataName;
                            oPUScreenTile.ViewModality = Modality;
                            oPUScreenTile.ViewText = vsText;
                            oPUScreenTile.ViewIcon = vIcon;
                            oPUScreenTile.ViewImage = vImage;
                            PUDataBindings oPUDataBindingsScreenTile = oPUScreenTile.ViewDataBindings;
                            DataBase.SetDataBinding(ref oPUDataBindingsScreenTile, DataName, Modality, ref UltraRow);
                            string[] codiciScreen = CodPadre.Split('§');
                            if (Modality == Enums.EnumModalityPopUp.mpNuovo) oPUDataBindingsScreenTile.DataBindings.DataSet.Tables[0].Rows[0]["CodScreen"] = codiciScreen[0];
                            if (codiciScreen.Length < 2 || codiciScreen[1] == null || codiciScreen[1] == string.Empty)
                            {
                                string tmp = DataBase.FindValue("CodTipoScreen", "T_Screen", @"Codice = '" + DataBase.Ax2(codiciScreen[0]) + @"'", "");
                                if (tmp != null && tmp != string.Empty)
                                {
                                    Scci.Model.en_TipoScreen tipoScreen = (Scci.Model.en_TipoScreen)Enum.Parse(typeof(Scci.Model.en_TipoScreen), tmp);
                                    oPUScreenTile.TipoScreen = tipoScreen;
                                }

                            }
                            else
                                oPUScreenTile.TipoScreen = (Scci.Model.en_TipoScreen)Enum.Parse(typeof(Scci.Model.en_TipoScreen), codiciScreen[1]);

                            oPUScreenTile.ViewInit();
                            oPUScreenTile.ShowDialog();
                        }

                        return DialogResult.OK;


                    default:
                        return DialogResult.Cancel;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                return DialogResult.Cancel;
            }
        }

        public static void ApriDOCX(string docxFullPath)
        {
            try
            {
                ApriDOCX(docxFullPath, "");
            }
            catch (Exception ex)
            {
                ExGest(ref ex, @"ApriDOCX", @"MyStatics");
            }
        }
        public static void ApriDOCX(string docxFullPath, string titolo)
        {
            frmPUDocViewer frm = null;
            try
            {
                frm = new frmPUDocViewer();

                if (titolo != null && titolo != string.Empty && titolo != "")
                    frm.ViewText = titolo;
                else
                {
                    if (docxFullPath != null && docxFullPath != string.Empty && docxFullPath != "")
                        frm.ViewText = System.IO.Path.GetFileName(docxFullPath);
                    else
                        frm.ViewText = "Documento";
                }

                frm.ViewIcon = Risorse.GetIconFromResource(MyStatics.GetNameResource(MyStatics.GC_WORD, Enums.EnumImageSize.isz32));

                frm.DOCXFileFullPath = docxFullPath;

                frm.ViewInit();

            }
            catch (Exception ex)
            {
                ExGest(ref ex, @"ApriDOCX", @"MyStatics");
            }
            finally
            {
                if (frm != null) frm.Dispose();
            }
        }

        public static void ApriPDF(string pdfFullPath)
        {
            try
            {
                ApriPDF(pdfFullPath, "");
            }
            catch (Exception ex)
            {
                ExGest(ref ex, @"ApriPDF", @"MyStatics");
            }
        }
        public static void ApriPDF(string pdfFullPath, string titolo)
        {

            frmPUPdfViewer frm = null;
            try
            {
                frm = new frmPUPdfViewer();

                if (titolo != null && titolo != string.Empty && titolo != "")
                    frm.ViewText = titolo;
                else
                {
                    if (pdfFullPath != null && pdfFullPath != string.Empty && pdfFullPath != "")
                        frm.ViewText = System.IO.Path.GetFileName(pdfFullPath);
                    else
                        frm.ViewText = "Documento";
                }

                frm.ViewIcon = Risorse.GetIconFromResource(MyStatics.GetNameResource(MyStatics.GC_WORD, Enums.EnumImageSize.isz32));

                frm.PDFFileFullPath = pdfFullPath;

                frm.ViewInit();

            }
            catch (Exception ex)
            {
                ExGest(ref ex, @"ApriPDF", @"MyStatics");
            }
            finally
            {
                if (frm != null) frm.Dispose();
            }

        }

        #endregion

        #region Utilità

        internal static Array SetSplit(string Key)
        {
            return SetSplit(Key, "|");
        }
        internal static Array SetSplit(string Key, string Separator)
        {
            Array ar = Key.Split(Separator.ToCharArray());
            return ar;
        }

        internal static string IndentXMLString(string xml)
        {

            String outXml = String.Empty;

            MemoryStream ms = new MemoryStream();

            XmlTextWriter xtw = new XmlTextWriter(ms, System.Text.Encoding.Unicode);
            XmlDocument doc = new XmlDocument();

            try
            {

                doc.LoadXml(xml);

                xtw.Formatting = Formatting.Indented;

                doc.WriteContentTo(xtw);
                xtw.Flush();

                ms.Seek(0, SeekOrigin.Begin);
                StreamReader sr = new StreamReader(ms);
                return sr.ReadToEnd();

            }
            catch (Exception)
            {
                return String.Empty;
            }

        }

        #endregion

        #region Errori

        internal static DialogResult ExGest(ref Exception roEx, string vsSubRoutine, string vsModuleName)
        {
            return ExGest(ref roEx, vsSubRoutine, vsModuleName, true, vsModuleName, @"Errore", MessageBoxIcon.Error, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);
        }
        internal static DialogResult ExGest(ref Exception roEx, string vsSubRoutine, string vsModuleName, bool vAddDebugInfo, string vsMessage, string vsMessageTitle, MessageBoxIcon vMessageIcon, MessageBoxButtons vMessageButtons, MessageBoxDefaultButton vMessageDefButton)
        {

            DialogResult vRet = DialogResult.OK;
            string sMsg = "";
            string sTitle = "";
            try
            {
                sMsg += roEx.Message;
                if (roEx.Source != null && roEx.Source.Trim() != "")
                    sMsg += Environment.NewLine + @"Source: " + roEx.Source;

                if (vsSubRoutine != null && vsSubRoutine.Trim() != "")
                    sMsg += Environment.NewLine + @"Sub: " + vsSubRoutine;

                if (vsModuleName != null && vsModuleName.Trim() != "")
                    sMsg += Environment.NewLine + @"Module: " + vsModuleName;

                if (vsMessage != null && vsMessage.Trim() != "")
                    sMsg += Environment.NewLine + vsMessage;

                if (vsMessageTitle == null || vsMessageTitle.Trim() == "")
                    sTitle = @"Errore";
                else
                    sTitle = vsMessageTitle;

                vRet = MessageBox.Show(sMsg, sTitle, vMessageButtons, vMessageIcon, vMessageDefButton);

            }
            catch (Exception ex)
            {
                Statics.AddDebugInfo(ex);
            }

            try
            {
                if (vAddDebugInfo) Statics.AddDebugInfo(roEx);
            }
            catch (Exception ex)
            {
                Statics.AddDebugInfo(ex);
            }

            return vRet;
        }

        #endregion

        #region Colori

        public static Color GetColorFromString(string Stringa)
        {

            Color Colore = default(Color);
            string sColore = Stringa;
            char[] splitchar1 = ",".ToCharArray();
            char[] splitchar2 = "=".ToCharArray();

            if (sColore.LastIndexOf("[") > 0)
            {
                sColore = sColore.Substring(sColore.LastIndexOf("[") + 1, sColore.LastIndexOf("]") - sColore.LastIndexOf("[") - 1);

                if (!Information.IsDBNull(sColore) & sColore != "Empty")
                {
                    Colore = Color.FromName(sColore);
                    if (Colore.ToArgb() == 0 && Information.IsArray(sColore.Split(splitchar1)))
                    {
                        int A = 0;
                        int R = 0;
                        int G = 0;
                        int B = 0;
                        if (Information.IsNumeric(sColore.Split(splitchar1)[0].Split(splitchar2)[1])) A = Convert.ToInt32(sColore.Split(splitchar1)[0].Split(splitchar2)[1]);
                        if (Information.IsNumeric(sColore.Split(splitchar1)[1].Split(splitchar2)[1])) R = Convert.ToInt32(sColore.Split(splitchar1)[1].Split(splitchar2)[1]);
                        if (Information.IsNumeric(sColore.Split(splitchar1)[2].Split(splitchar2)[1])) G = Convert.ToInt32(sColore.Split(splitchar1)[2].Split(splitchar2)[1]);
                        if (Information.IsNumeric(sColore.Split(splitchar1)[3].Split(splitchar2)[1])) B = Convert.ToInt32(sColore.Split(splitchar1)[3].Split(splitchar2)[1]);

                        Colore = Color.FromArgb(A, R, G, B);
                    }
                }
                else
                {
                    Colore = Color.FromArgb(0, 0, 0, 0);
                }

            }

            if (Colore.ToArgb() == 0 & Information.IsNumeric(sColore))
            {
                Colore = Color.FromArgb(Convert.ToInt32(sColore));
            }

            return Colore;

        }

        public static Bitmap CreateSolidBitmap(Color Colore, int x = 16, int y = 16)
        {

            Bitmap oRet = new Bitmap(x, y);

            for (int nx = 0; nx <= x - 1; nx++)
            {
                for (int ny = 0; ny <= y - 1; ny++)
                {
                    if (nx == 0 || nx == x - 1 || ny == 0 || ny == y - 1)
                    {
                        oRet.SetPixel(nx, ny, Color.Black);
                    }
                    else
                    {
                        oRet.SetPixel(nx, ny, Colore);
                    }
                }
            }

            return oRet;

        }

        #endregion

        #region Gestione Log

        public static void LogManager(Enums.EnumModalityPopUp modality, Enums.EnumEntitaLog entita, DataSet logprima, DataSet logdopo)
        {

            string sLogPrima = string.Empty;
            string sLogDopo = string.Empty;

            try
            {

                if (entita != Enums.EnumEntitaLog.Nessuna)
                {

                    Ambiente.Codlogin = UnicodeSrl.Framework.Utility.Windows.CurrentUser();
                    Ambiente.Indirizzoip = CoreStatics.CoreApplication.Ambiente.Indirizzoip;
                    Ambiente.Nomepc = CoreStatics.CoreApplication.Ambiente.Nomepc;

                    Parametri op = new Parametri(Ambiente);
                    op.TimeStamp.CodEntita = entita.ToString();

                    switch (modality)
                    {

                        case Enums.EnumModalityPopUp.mpNuovo:
                            if (logdopo != null)
                            {
                                LogSetDataSet(ref logdopo);
                                sLogDopo = logdopo.GetXml();
                                op.TimeStamp.CodAzione = "INS";
                            }
                            break;

                        case Enums.EnumModalityPopUp.mpModifica:
                            if (logprima != null)
                            {
                                LogSetDataSet(ref logprima);
                                sLogPrima = logprima.GetXml();
                            }
                            if (logdopo != null)
                            {
                                LogSetDataSet(ref logdopo);
                                sLogDopo = logdopo.GetXml();
                            }
                            op.TimeStamp.CodAzione = "MOD";
                            break;

                        case Enums.EnumModalityPopUp.mpCancella:
                            if (logprima != null)
                            {
                                LogSetDataSet(ref logprima);
                                sLogPrima = logprima.GetXml();
                            }
                            op.TimeStamp.CodAzione = "CAN";
                            break;

                    }

                    op.Parametro.Add("LogPrima", sLogPrima);
                    op.Parametro.Add("LogDopo", sLogDopo);

                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    xmlParam = XmlProcs.CheckXMLDati(xmlParam);
                    FwDataParametersList fplist = new FwDataParametersList
                    {
                        { "xParametri", xmlParam, ParameterDirection.Input, DbType.Xml }
                    };

                    Database.GetDataTableStoredProc("MSP_InsMovLog", fplist);

                }

            }
            catch (Exception)
            {

            }

        }

        private static void LogSetDataSet(ref DataSet ds)
        {

            if (ds != null)
            {
                ds.DataSetName = "DataSet";
                if (ds.Tables.Count > 0)
                {
                    ds.Tables[0].TableName = "Table";
                }
            }

        }

        #endregion

    }

}
