using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.ScciResource;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinSchedule;
using Infragistics.Win.UltraWinListView;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.DatiClinici.DC;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci.PluginClient;
using UnicodeSrl.Scci;
using Infragistics.Win.UltraWinGrid;
using UnicodeSrl.Framework.Data;

namespace UnicodeSrl.ScciCore
{
    public partial class frmSelezionaAppuntamento : frmBaseModale, Interfacce.IViewFormlModal
    {

        #region ucDcViewer

        private WpfControls40.ucDcViewer ucDcViewer;

                                private void InitializeDCViewerComponent()
        {
            this.ucDcViewer = new UnicodeSrl.WpfControls40.ucDcViewer();
            this.ehViewer.Child = this.ucDcViewer;
        }

        #endregion

        public frmSelezionaAppuntamento()
        {
            InitializeComponent();
            InitializeDCViewerComponent();
        }

        #region Declare

        private Gestore oGestore = null;
        private ucEasyListBox _ucEasyListBox = null;
                private ucSegnalibri _ucSegnalibri = null;

                private const string _C_SINGOLO = @"SN";
        private const string _C_MULTIPLO = @"ML";
        private const string _C_SETTIMANALE = @"ST";
        private const string _C_DAPIANIFICARE = @"DP";

        private bool _bruntime = false;

        private bool _brefreshappuntamenti = true;

        private int _minOccorrenze = 0;
        private int _MAXOccorrenze = 0;

        #endregion

        #region Interface

        public void Carica()
        {

            try
            {
                _brefreshappuntamenti = true;

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = Risorse.GetIconFromResource(Risorse.GC_APPUNTAMENTO_16);

                this.ucEasyCheckEditorDC.Appearance.ImageBackground = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_MODIFICA_256);
                this.ucEasyCheckEditorDC.CheckedAppearance.ImageBackground = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_MODIFICACHECK_256);

                this.InizializzaGestore();

                this.ucEasyLabelAgende.Text = "";
                foreach (MovAppuntamentoAgende oMaa in CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Elementi)
                {
                    if (oMaa.Selezionata == true)
                    {
                        if (this.ucEasyLabelAgende.Text != string.Empty) { this.ucEasyLabelAgende.Text += ", "; }
                        this.ucEasyLabelAgende.Text += oMaa.Descrizione;
                    }
                }


                                _bruntime = true;
                if (getEscludiFestivitaDefault())
                {
                    this.ucEasyCheckEditorFestivita_ST.Checked = true;
                    this.ucEasyCheckEditorFestivita_ML.Checked = true;
                }
                else
                {
                    this.ucEasyCheckEditorFestivita_ST.Checked = false;
                    this.ucEasyCheckEditorFestivita_ML.Checked = false;
                }
                                if (getEscludiSovrapposizioniDefault())
                {
                    this.ucEasyCheckEditorEscludiSovrapposizioni_ST.Checked = true;
                    this.ucEasyCheckEditorEscludiSovrapposizioni_ML.Checked = true;
                }
                else
                {
                    this.ucEasyCheckEditorEscludiSovrapposizioni_ST.Checked = false;
                    this.ucEasyCheckEditorEscludiSovrapposizioni_ML.Checked = false;
                }

                _bruntime = false;

                                if (!int.TryParse(UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.AppuntamentiMinOccorrenze), out _minOccorrenze)) _minOccorrenze = 0;
                if (!int.TryParse(UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.AppuntamentiMAXOccorrenze), out _MAXOccorrenze)) _MAXOccorrenze = 0;

                SetMinMAXOccorrenze(true, true);

                CoreStatics.CoreApplication.MovAppuntamentiGenerati = new List<MovAppuntamento>();

                LoadOptionSetTipoAppuntamento();

                                this.ucEasyTextBoxOggetto.ReadOnly = true;
                this.ucAnteprimaRtf.rtbRichTextBox.ReadOnly = true;
                if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Azione == EnumAzioni.VIS || CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Azione == EnumAzioni.CAN)
                {
                    this.ucEasyOptionSet.Enabled = false;
                                        this.udteDataInizio.ReadOnly = true;
                    this.udteDataFine.ReadOnly = true;
                                        this.udteDataOraInizio_ML.ReadOnly = true;
                    this.udteDataOraFine_ML.ReadOnly = true;
                    this.ucSelPeriodicitaGiorni_ML.Enabled = false;
                    this.ucSelPeriodicitaOre_ML.Enabled = false;
                    this.ucSelPeriodicitaMinuti_ML.Enabled = false;
                                        this.ucEasyCheckEditorDC.Enabled = false;
                    this.SetDC(false);
                }
                else
                {
                    this.ucEasyCheckEditorDC.Checked = true;
                }

                this.ShowDialog();

            }
            catch (Exception)
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }

        }

        #endregion

        #region Private Properties

        private bool TipoAppuntamentoSettimanale
        {
            get
            {
                return CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Settimanale;
            }
        }

        private bool TipoAppuntamentoMultiplo
        {
            get
            {
                return CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Multiplo;
            }
        }

        private bool TipoAppuntamentoSenzaData
        {
            get
            {
                return CoreStatics.CoreApplication.MovAppuntamentoSelezionato.SenzaData;
            }
        }

        private bool TipoAppuntamentoSenzaDataSempre
        {
            get
            {
                return CoreStatics.CoreApplication.MovAppuntamentoSelezionato.SenzaDataSempre;
            }
        }

        #endregion

        #region Tipi Appuntamenti Multipli / Settimanali / Da Pianificare

                                private void LoadOptionSetTipoAppuntamentoOld()
        {

            try
            {
                ucEasyTabControlOrari.Tabs["Pianificazione"].Active = true;
                ucEasyTabControlOrari.Tabs["Pianificazione"].Selected = true;
                                ucEasyTabControlOrari.Tabs["Anteprima"].Visible = false;
                                this.ucEasyOptionSet.Items.Clear();
                                Infragistics.Win.ValueListItem oVal = new Infragistics.Win.ValueListItem(_C_SINGOLO, "Singolo");
                this.ucEasyOptionSet.Items.Add(oVal);

                switch (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Azione)
                {
                    case EnumAzioni.INS:
                                                                                                                        if (TipoAppuntamentoSettimanale)
                        {
                            this.ucEasyOptionSet.Visible = true;
                                                        this.ucEasyTableLayoutPanelSelezioni.ColumnStyles[1].SizeType = SizeType.Percent;
                            this.ucEasyTableLayoutPanelSelezioni.ColumnStyles[1].Width = 82;
                                                        this.ucEasyTableLayoutPanelSelezioni.ColumnStyles[0].SizeType = SizeType.Percent;
                            this.ucEasyTableLayoutPanelSelezioni.ColumnStyles[0].Width = 18;

                            Infragistics.Win.ValueListItem oValML = new Infragistics.Win.ValueListItem(_C_SETTIMANALE, "Settimanale");
                            this.ucEasyOptionSet.Items.Add(oValML);
                            ucEasyTabControlOrari.Tabs["Anteprima"].Visible = true;
                            ucEasyTabControlOrari.Tabs["Anteprima"].Enabled = false;
                        }
                        else if (TipoAppuntamentoMultiplo)
                        {
                            this.ucEasyOptionSet.Visible = true;
                                                        this.ucEasyTableLayoutPanelSelezioni.ColumnStyles[1].SizeType = SizeType.Percent;
                            this.ucEasyTableLayoutPanelSelezioni.ColumnStyles[1].Width = 82;
                                                        this.ucEasyTableLayoutPanelSelezioni.ColumnStyles[0].SizeType = SizeType.Percent;
                            this.ucEasyTableLayoutPanelSelezioni.ColumnStyles[0].Width = 18;

                            Infragistics.Win.ValueListItem oValML = new Infragistics.Win.ValueListItem(_C_MULTIPLO, "Multiplo");
                            this.ucEasyOptionSet.Items.Add(oValML);
                            ucEasyTabControlOrari.Tabs["Anteprima"].Visible = true;
                            ucEasyTabControlOrari.Tabs["Anteprima"].Enabled = false;
                        }
                        else
                        {
                                                        this.ucEasyTableLayoutPanelSelezioni.ColumnStyles[0].SizeType = SizeType.Percent;
                            this.ucEasyTableLayoutPanelSelezioni.ColumnStyles[0].Width = 0;
                                                        this.ucEasyTableLayoutPanelSelezioni.ColumnStyles[1].SizeType = SizeType.Percent;
                            this.ucEasyTableLayoutPanelSelezioni.ColumnStyles[1].Width = 100;
                                                        this.ucEasyOptionSet.Visible = false;
                        }

                                                this.ucEasyOptionSet.CheckedIndex = 0;


                                                if (TipoAppuntamentoSenzaData)
                        {
                            this.ucEasyCheckEditorDP.Visible = true;
                            this.ucEasyCheckEditorDP.Checked = (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento == EnumStatoAppuntamento.DP.ToString());
                            this.ucEasyLabelDP.Visible = true;
                            if (this.TipoAppuntamentoSenzaDataSempre)
                            {
                                this.ucEasyCheckEditorDP.Checked = true;
                                this.ucEasyCheckEditorDP.Enabled = false;


                                                                NascondiSezioneDate(true);
                            }
                            else
                            {
                                                                NascondiSezioneDate(false);
                            }
                        }

                                                this.udteDataInizio.Value = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio;
                        this.udteDataFine.Value = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine;
                        if (TipoAppuntamentoSettimanale)
                        {
                            this.udteDataInizio_ST.Value = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio.Date;
                            this.udteDataFine_ST.Value = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine.Date;

                            this.udteOraInizio_ST.Value = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio;
                            this.udteOraFine_ST.Value = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine;

                            initTabSettimanale();

                        }
                        else if (TipoAppuntamentoMultiplo)
                        {
                            this.udteDataOraInizio_ML.Value = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio;
                            this.udteDataOraFine_ML.Value = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine;
                        }

                        break;

                    case EnumAzioni.ANN:
                    case EnumAzioni.CAN:
                    case EnumAzioni.VIS:
                    case EnumAzioni.MOD:


                                                                        
                                                                                                
                                                                        

                                                                        this.ucEasyTableLayoutPanelSelezioni.ColumnStyles[0].SizeType = SizeType.Percent;
                        this.ucEasyTableLayoutPanelSelezioni.ColumnStyles[0].Width = 0;
                                                this.ucEasyTableLayoutPanelSelezioni.ColumnStyles[1].SizeType = SizeType.Percent;
                        this.ucEasyTableLayoutPanelSelezioni.ColumnStyles[1].Width = 100;
                                                this.ucEasyOptionSet.Visible = false;
                                                                                                                                                                                                this.ucEasyLabelMultiplo.Visible = false;


                        if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento == EnumStatoAppuntamento.DP.ToString()
                            || (TipoAppuntamentoSenzaData && CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento == EnumStatoAppuntamento.PR.ToString()))
                        {
                            this.ucEasyLabelDP.Visible = true;
                            this.ucEasyCheckEditorDP.Visible = true;
                            this.ucEasyCheckEditorDP.Checked = (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento == EnumStatoAppuntamento.DP.ToString());
                            if (this.TipoAppuntamentoSenzaDataSempre)
                            {
                                this.ucEasyCheckEditorDP.Checked = true;
                                this.ucEasyCheckEditorDP.Enabled = false;

                                NascondiSezioneDate(true);
                            }
                            else
                                NascondiSezioneDate(false);
                        }

                                                this.udteDataInizio.Value = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio;
                        this.udteDataFine.Value = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine;
                        if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento == EnumStatoAppuntamento.DP.ToString())
                        {
                                                                                                                this.udteDataInizio.Enabled = false;
                            this.udteDataFine.Enabled = false;
                        }
                                                                                                                        
                        break;

                }


            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "LoadOptionSetTipoAppuntamento", this.Text);
            }

        }
        private void LoadOptionSetTipoAppuntamento()
        {

            try
            {

                                ucEasyTabControlOrari.Tabs["Pianificazione"].Visible = true;
                ucEasyTabControlOrari.Tabs["Pianificazione"].Active = true;
                ucEasyTabControlOrari.Tabs["Pianificazione"].Selected = true;
                ucEasyTabControlOrari.Tabs["Anteprima"].Visible = false;
                ucEasyTabControlOrari.Tabs["Anteprima"].Enabled = false;

                                this.ucEasyTableLayoutPanelSelezioni.ColumnStyles[0].SizeType = SizeType.Percent;
                this.ucEasyTableLayoutPanelSelezioni.ColumnStyles[0].Width = 0;
                this.ucEasyTableLayoutPanelSelezioni.ColumnStyles[1].SizeType = SizeType.Percent;
                this.ucEasyTableLayoutPanelSelezioni.ColumnStyles[1].Width = 100;

                                                                this.ucEasyOptionSet.Items.Clear();
                switch (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Azione)
                {

                                                                                case EnumAzioni.INS:
                                                if (TipoAppuntamentoSenzaDataSempre == false)
                        {
                            Infragistics.Win.ValueListItem oValSI = new Infragistics.Win.ValueListItem(_C_SINGOLO, "Singolo");
                            this.ucEasyOptionSet.Items.Add(oValSI);
                        }
                        else
                        {
                                                        NascondiSezioneDate(true);
                        }

                                                if (TipoAppuntamentoSettimanale)
                        {
                            Infragistics.Win.ValueListItem oValSE = new Infragistics.Win.ValueListItem(_C_SETTIMANALE, "Settimanale");
                            this.ucEasyOptionSet.Items.Add(oValSE);
                            ucEasyTabControlOrari.Tabs["Anteprima"].Visible = true;
                            ucEasyTabControlOrari.Tabs["Anteprima"].Enabled = false;
                        }

                                                if (TipoAppuntamentoMultiplo)
                        {
                            Infragistics.Win.ValueListItem oValML = new Infragistics.Win.ValueListItem(_C_MULTIPLO, "Multiplo");
                            this.ucEasyOptionSet.Items.Add(oValML);
                            ucEasyTabControlOrari.Tabs["Anteprima"].Visible = true;
                            ucEasyTabControlOrari.Tabs["Anteprima"].Enabled = false;
                        }

                                                if (TipoAppuntamentoSenzaData)
                        {
                            Infragistics.Win.ValueListItem oValDP = new Infragistics.Win.ValueListItem(_C_DAPIANIFICARE, "Da Pianificare");
                            this.ucEasyOptionSet.Items.Add(oValDP);
                        }
                        this.ucEasyOptionSet.CheckedIndex = 0;
                        if (this.ucEasyOptionSet.Items.Count > 1)
                        {
                            this.ucEasyTableLayoutPanelSelezioni.ColumnStyles[1].SizeType = SizeType.Percent;
                            this.ucEasyTableLayoutPanelSelezioni.ColumnStyles[1].Width = 82;
                            this.ucEasyTableLayoutPanelSelezioni.ColumnStyles[0].SizeType = SizeType.Percent;
                            this.ucEasyTableLayoutPanelSelezioni.ColumnStyles[0].Width = 18;
                        }

                        break;

                    case EnumAzioni.ANN:
                    case EnumAzioni.CAN:
                    case EnumAzioni.VIS:
                    case EnumAzioni.MOD:


                                                                        
                                                                                                
                                                                        

                                                                        this.ucEasyTableLayoutPanelSelezioni.ColumnStyles[0].SizeType = SizeType.Percent;
                        this.ucEasyTableLayoutPanelSelezioni.ColumnStyles[0].Width = 0;
                                                this.ucEasyTableLayoutPanelSelezioni.ColumnStyles[1].SizeType = SizeType.Percent;
                        this.ucEasyTableLayoutPanelSelezioni.ColumnStyles[1].Width = 100;
                                                this.ucEasyOptionSet.Visible = false;
                                                                                                                                                                                                this.ucEasyLabelMultiplo.Visible = false;


                        if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento == EnumStatoAppuntamento.DP.ToString()
                            || (TipoAppuntamentoSenzaData && CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento == EnumStatoAppuntamento.PR.ToString()))
                        {
                            this.ucEasyLabelDP.Visible = true;
                            this.ucEasyCheckEditorDP.Visible = true;
                            this.ucEasyCheckEditorDP.Checked = (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento == EnumStatoAppuntamento.DP.ToString());
                            if (this.TipoAppuntamentoSenzaDataSempre)
                            {
                                this.ucEasyCheckEditorDP.Checked = true;
                                this.ucEasyCheckEditorDP.Enabled = false;

                                NascondiSezioneDate(true);
                            }
                            else
                                NascondiSezioneDate(false);
                        }

                                                if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio != DateTime.MinValue)
                            this.udteDataInizio.Value = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio;

                        if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine != DateTime.MinValue)
                            this.udteDataFine.Value = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine;

                        if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento == EnumStatoAppuntamento.DP.ToString())
                        {
                                                                                                                this.udteDataInizio.Enabled = false;
                            this.udteDataFine.Enabled = false;
                        }
                                                                                                                        
                        break;

                }


            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "LoadOptionSetTipoAppuntamento", this.Text);
            }

        }

        private void ucEasyCheckEditorDP_CheckedChanged(object sender, EventArgs e)
        {
            if (TipoAppuntamentoSenzaData && this.ucEasyCheckEditorDP.Checked)
            {
                this.udteDataInizio.Enabled = false;
                this.udteDataFine.Enabled = false;

                                if (TipoAppuntamentoSenzaDataSempre)
                {
                    this.udteDataInizio.Visible = false;
                    this.udteDataFine.Visible = false;
                    this.lblDataProgrammataInizio.Visible = false;
                    this.lblDataProgrammataFine.Visible = false;

                                        NascondiSezioneDate(true);
                }
                else
                {
                                        NascondiSezioneDate(false);
                }
            }
            else
            {
                this.udteDataInizio.Enabled = !(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Azione == EnumAzioni.VIS || CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Azione == EnumAzioni.CAN || CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Azione == EnumAzioni.ANN);
                this.udteDataFine.Enabled = !(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Azione == EnumAzioni.VIS || CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Azione == EnumAzioni.CAN || CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Azione == EnumAzioni.ANN);
            }
        }

        private void ucEasyOptionSet_ValueChanged(object sender, EventArgs e)
        {

            if (ucEasyOptionSet.CheckedItem != null)
            {
                SetOptionSetTabsAndControls(ucEasyOptionSet.CheckedItem.DataValue.ToString());

                if (!_bruntime)
                {
                                                            _brefreshappuntamenti = true;
                }
            }
        }

        private void SetOptionSetTabsAndControls(string key)
        {
            switch (key)
            {

                case _C_SINGOLO:
                    this.ucEasyTabControlSelezioni.Tabs[key].Visible = true;
                    this.ucEasyTabControlSelezioni.ActiveTab = this.ucEasyTabControlSelezioni.Tabs[key];
                    this.ucEasyTabControlSelezioni.Tabs[_C_MULTIPLO].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[_C_SETTIMANALE].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[_C_DAPIANIFICARE].Visible = false;
                    this.ucEasyTabControlOrari.Tabs["Anteprima"].Enabled = false;

                    this.udteDataInizio.Value = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio;
                    this.udteDataFine.Value = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine;

                    break;

                case _C_MULTIPLO:                      this.ucEasyTabControlSelezioni.Tabs[key].Visible = true;
                    this.ucEasyTabControlSelezioni.ActiveTab = this.ucEasyTabControlSelezioni.Tabs[key];
                    this.ucEasyTabControlSelezioni.Tabs[_C_SINGOLO].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[_C_SETTIMANALE].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[_C_DAPIANIFICARE].Visible = false;
                    this.ucEasyTabControlOrari.Tabs["Anteprima"].Enabled = true;

                    this.udteDataOraInizio_ML.Value = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio;
                    this.udteDataOraFine_ML.Value = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine;

                    break;

                case _C_SETTIMANALE:                      this.ucEasyTabControlSelezioni.Tabs[key].Visible = true;
                    this.ucEasyTabControlSelezioni.ActiveTab = this.ucEasyTabControlSelezioni.Tabs[key];
                    this.ucEasyTabControlSelezioni.Tabs[_C_SINGOLO].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[_C_MULTIPLO].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[_C_DAPIANIFICARE].Visible = false;
                    this.ucEasyTabControlOrari.Tabs["Anteprima"].Enabled = true;

                    this.udteDataInizio_ST.Value = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio.Date;
                    this.udteDataFine_ST.Value = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine.Date;
                    this.udteOraInizio_ST.Value = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio;
                    this.udteOraFine_ST.Value = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine;
                    initTabSettimanale();

                    break;

                case _C_DAPIANIFICARE:                      this.ucEasyTabControlSelezioni.Tabs[key].Visible = true;
                    this.ucEasyTabControlSelezioni.ActiveTab = this.ucEasyTabControlSelezioni.Tabs[key];
                    this.ucEasyTabControlSelezioni.Tabs[_C_SINGOLO].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[_C_MULTIPLO].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[_C_SETTIMANALE].Visible = false;
                    this.ucEasyTabControlOrari.Tabs["Anteprima"].Enabled = false;

                    this.udteDataInizio.Value = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio;
                    this.udteDataFine.Value = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine;

                                                                                                                                                                                                        

                                                                                                                                                                                    
                    break;

            }

        }

        private void NascondiSezioneDate(bool nascondi)
        {
            if (nascondi)
            {

                                this.ucEasyTabControlOrari.Visible = false;
                this.ucEasyTableLayoutPanel.RowStyles[5].Height = 0;
                this.ucEasyTableLayoutPanel.RowStyles[4].Height = 85;
            }
            else
            {

                                this.ucEasyTableLayoutPanel.RowStyles[4].Height = 55;
                this.ucEasyTableLayoutPanel.RowStyles[5].Height = 30;
                this.ucEasyTabControlOrari.Visible = true;
            }
        }

        private void udteDataOraFine_ML_Validating(object sender, CancelEventArgs e)
        {
                        if (udteDataOraInizio_ML.Value != null && udteDataOraFine_ML.Value != null &&
                        Convert.ToDateTime(udteDataOraInizio_ML.Value.ToString()) > Convert.ToDateTime(udteDataOraFine_ML.Value))
                this.udteDataOraInizio_ML.Value = this.udteDataOraFine_ML.Value;

        }
        private void udteDataOraInizio_ML_Validating(object sender, CancelEventArgs e)
        {
                        if (udteDataOraInizio_ML.Value != null && udteDataOraFine_ML.Value != null &&
                Convert.ToDateTime(udteDataOraInizio_ML.Value.ToString()) > Convert.ToDateTime(udteDataOraFine_ML.Value))
                this.udteDataOraFine_ML.Value = this.udteDataOraInizio_ML.Value;

        }

        private void udteDataFine_ST_Validating(object sender, CancelEventArgs e)
        {
                        if (udteDataInizio_ST.Value != null && udteDataFine_ST.Value != null &&
                        Convert.ToDateTime(udteDataInizio_ST.Value.ToString()) > Convert.ToDateTime(udteDataFine_ST.Value))
                this.udteDataInizio_ST.Value = this.udteDataFine_ST.Value;

        }
        private void udteDataInizio_ST_Validating(object sender, CancelEventArgs e)
        {
                        if (udteDataInizio_ST.Value != null && udteDataFine_ST.Value != null &&
                Convert.ToDateTime(udteDataInizio_ST.Value.ToString()) > Convert.ToDateTime(udteDataFine_ST.Value))
                this.udteDataFine_ST.Value = this.udteDataInizio_ST.Value;

        }

        private void udteOraFine_ST_Validating(object sender, CancelEventArgs e)
        {
                        if (udteOraInizio_ST.Value != null && udteOraFine_ST.Value != null &&
                        Convert.ToDateTime(udteOraInizio_ST.Value.ToString()) > Convert.ToDateTime(udteOraFine_ST.Value))
                this.udteOraInizio_ST.Value = this.udteOraFine_ST.Value;

        }
        private void udteOraInizio_ST_Validating(object sender, CancelEventArgs e)
        {
                        if (udteOraInizio_ST.Value != null && udteOraFine_ST.Value != null &&
                Convert.ToDateTime(udteOraInizio_ST.Value.ToString()) > Convert.ToDateTime(udteOraFine_ST.Value))
                this.udteOraFine_ST.Value = this.udteOraInizio_ST.Value;

        }

        private void uceDaysOfWeek_CheckedChanged(object sender, EventArgs e)
        {
            if (!_bruntime)
            {
                                                _brefreshappuntamenti = true;
            }
        }

        private void GestValueChange(object sender, EventArgs e)
        {
            if (!_bruntime)
            {
                                                _brefreshappuntamenti = true;
            }
        }

        private void CaricaAppuntamenti()
        {
            this.ImpostaCursore(enum_app_cursors.WaitCursor);
            try
            {
                List<IntervalloTempi> listaappuntamenti = new List<IntervalloTempi>();

                switch (ucEasyOptionSet.CheckedItem.DataValue.ToString())
                {
                    case _C_MULTIPLO:
                        listaappuntamenti = getListaDatePerAppuntamentoMultiplo(true);
                        break;
                    default:
                        listaappuntamenti = getListaDatePerAppuntamentoSettimanale(true);
                        break;
                }

                
                this.lblElencoAppuntamenti.Text = "Elenco Appuntamenti (" + listaappuntamenti.Count + ")";
                if (listaappuntamenti.Count > 0)
                {
                    this.ucEasyGrid.DataSource = listaappuntamenti;
                                    }
                else
                    this.ucEasyGrid.DataSource = null;
                this.ucEasyGrid.Refresh();
                _brefreshappuntamenti = false;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }
        }

        private List<IntervalloTempi> getListaDatePerAppuntamentoMultiplo(bool checkMaxOccorrenze)
        {

            List<IntervalloTempi> listaDateAppuntamenti = new List<IntervalloTempi>();
            DateTime dataperiodicita = DateTime.MinValue;
            List<DateTime> dateDaEscludere = getListDateDaEscludere();
            int iMaxOccorrenze = 0;
            if (checkMaxOccorrenze && this.ucSelOccorrenze_ML.Value != null) iMaxOccorrenze = (int)this.ucSelOccorrenze_ML.Value;

            if (this.udteDataOraInizio_ML.Value != null && this.udteDataOraInizio_ML.DateTime != DateTime.MinValue
                && this.udteDataOraFine_ML.Value != null && this.udteDataOraFine_ML.DateTime != DateTime.MinValue)
            {
                DateTime orainizio = new DateTime(this.udteDataOraInizio_ML.DateTime.Year, this.udteDataOraInizio_ML.DateTime.Month, this.udteDataOraInizio_ML.DateTime.Day, this.udteDataOraInizio_ML.DateTime.Hour, this.udteDataOraInizio_ML.DateTime.Minute, 0);
                DateTime orafine = new DateTime(this.udteDataOraFine_ML.DateTime.Year, this.udteDataOraFine_ML.DateTime.Month, this.udteDataOraFine_ML.DateTime.Day, this.udteDataOraFine_ML.DateTime.Hour, this.udteDataOraFine_ML.DateTime.Minute, 0);

                if (!dateDaEscludere.Contains(orainizio.Date)
                    && !AppuntamentoHaSovrapposizioni(orainizio, orainizio.AddMinutes(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.TimeSlotInterval)))
                    listaDateAppuntamenti.Add(new IntervalloTempi(orainizio, orainizio.AddMinutes(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.TimeSlotInterval), "", ""));

                                if ((int)this.ucSelPeriodicitaGiorni_ML.Value > 0 || (int)this.ucSelPeriodicitaOre_ML.Value > 0 || (int)this.ucSelPeriodicitaMinuti_ML.Value > 0)
                {
                    if (iMaxOccorrenze <= 0 || iMaxOccorrenze > listaDateAppuntamenti.Count)
                    {
                        dataperiodicita = orainizio.AddDays((int)this.ucSelPeriodicitaGiorni_ML.Value).AddHours((int)this.ucSelPeriodicitaOre_ML.Value).AddMinutes((int)this.ucSelPeriodicitaMinuti_ML.Value);

                        while (dataperiodicita <= orafine)
                        {
                            if (!dateDaEscludere.Contains(dataperiodicita.Date)
                                && !AppuntamentoHaSovrapposizioni(dataperiodicita, dataperiodicita.AddMinutes(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.TimeSlotInterval)))
                            {
                                listaDateAppuntamenti.Add(new IntervalloTempi(dataperiodicita, dataperiodicita.AddMinutes(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.TimeSlotInterval), "", ""));
                            }

                            dataperiodicita = dataperiodicita.AddDays((int)this.ucSelPeriodicitaGiorni_ML.Value).AddHours((int)this.ucSelPeriodicitaOre_ML.Value).AddMinutes((int)this.ucSelPeriodicitaMinuti_ML.Value);

                            if (iMaxOccorrenze > 0 && iMaxOccorrenze == listaDateAppuntamenti.Count) break;
                        }
                    }
                }
            }

            return listaDateAppuntamenti;
        }

        private List<IntervalloTempi> getListaDatePerAppuntamentoSettimanale(bool checkMaxOccorrenze)
        {
            List<IntervalloTempi> listaDateAppuntamenti = new List<IntervalloTempi>();
            int iMaxOccorrenze = 0;
            if (checkMaxOccorrenze && this.ucSelOccorrenze_ST.Value != null) iMaxOccorrenze = (int)this.ucSelOccorrenze_ST.Value;

            if ((int)this.ucSelPeriodicitaSettimane_ST.Value > 0
                && (this.uceDaysOfWeekLun.Checked || this.uceDaysOfWeekMar.Checked || this.uceDaysOfWeekMer.Checked || this.uceDaysOfWeekGio.Checked || this.uceDaysOfWeekVen.Checked || this.uceDaysOfWeekSab.Checked || this.uceDaysOfWeekDom.Checked))
            {

                using (AppointmentRecurrence oRecurrence = new AppointmentRecurrence())
                {
                    oRecurrence.PatternFrequency = RecurrencePatternFrequency.Weekly;
                    oRecurrence.PatternInterval = (int)this.ucSelPeriodicitaSettimane_ST.Value;

                    if (this.uceDaysOfWeekLun.Checked == true)
                    {
                        oRecurrence.PatternDaysOfWeek = oRecurrence.PatternDaysOfWeek | RecurrencePatternDaysOfWeek.Monday;
                    }
                    if (this.uceDaysOfWeekMar.Checked == true)
                    {
                        oRecurrence.PatternDaysOfWeek = oRecurrence.PatternDaysOfWeek | RecurrencePatternDaysOfWeek.Tuesday;
                    }
                    if (this.uceDaysOfWeekMer.Checked == true)
                    {
                        oRecurrence.PatternDaysOfWeek = oRecurrence.PatternDaysOfWeek | RecurrencePatternDaysOfWeek.Wednesday;
                    }
                    if (this.uceDaysOfWeekGio.Checked == true)
                    {
                        oRecurrence.PatternDaysOfWeek = oRecurrence.PatternDaysOfWeek | RecurrencePatternDaysOfWeek.Thursday;
                    }
                    if (this.uceDaysOfWeekVen.Checked == true)
                    {
                        oRecurrence.PatternDaysOfWeek = oRecurrence.PatternDaysOfWeek | RecurrencePatternDaysOfWeek.Friday;
                    }
                    if (this.uceDaysOfWeekSab.Checked == true)
                    {
                        oRecurrence.PatternDaysOfWeek = oRecurrence.PatternDaysOfWeek | RecurrencePatternDaysOfWeek.Saturday;
                    }
                    if (this.uceDaysOfWeekDom.Checked == true)
                    {
                        oRecurrence.PatternDaysOfWeek = oRecurrence.PatternDaysOfWeek | RecurrencePatternDaysOfWeek.Sunday;
                    }

                    oRecurrence.RangeLimit = RecurrenceRangeLimit.LimitByDate;
                    oRecurrence.RangeStartDate = (DateTime)this.udteDataInizio_ST.Value;
                    oRecurrence.RangeEndDate = (DateTime)this.udteDataFine_ST.Value;


                                                                                List<DateTime> dateDaEscludere = getListDateDaEscludere();
                    DateTime dataperiodicita = ((DateTime)this.udteDataInizio_ST.Value).AddDays(-1);
                    while (dataperiodicita <= oRecurrence.RangeEndDate)
                    {
                                                                                                Boolean bCheckDay = false;
                        while (!bCheckDay)
                        {

                            dataperiodicita = dataperiodicita.AddDays(1);

                            if ((dataperiodicita.DayOfWeek == System.DayOfWeek.Monday) & (oRecurrence.PatternDaysOfWeek & RecurrencePatternDaysOfWeek.Monday) == RecurrencePatternDaysOfWeek.Monday)
                            {
                                bCheckDay = true;
                            }
                            if ((dataperiodicita.DayOfWeek == System.DayOfWeek.Tuesday) & (oRecurrence.PatternDaysOfWeek & RecurrencePatternDaysOfWeek.Tuesday) == RecurrencePatternDaysOfWeek.Tuesday)
                            {
                                bCheckDay = true;
                            }
                            if ((dataperiodicita.DayOfWeek == System.DayOfWeek.Wednesday) & (oRecurrence.PatternDaysOfWeek & RecurrencePatternDaysOfWeek.Wednesday) == RecurrencePatternDaysOfWeek.Wednesday)
                            {
                                bCheckDay = true;
                            }
                            if ((dataperiodicita.DayOfWeek == System.DayOfWeek.Thursday) & (oRecurrence.PatternDaysOfWeek & RecurrencePatternDaysOfWeek.Thursday) == RecurrencePatternDaysOfWeek.Thursday)
                            {
                                bCheckDay = true;
                            }
                            if ((dataperiodicita.DayOfWeek == System.DayOfWeek.Friday) & (oRecurrence.PatternDaysOfWeek & RecurrencePatternDaysOfWeek.Friday) == RecurrencePatternDaysOfWeek.Friday)
                            {
                                bCheckDay = true;
                            }
                            if ((dataperiodicita.DayOfWeek == System.DayOfWeek.Saturday) & (oRecurrence.PatternDaysOfWeek & RecurrencePatternDaysOfWeek.Saturday) == RecurrencePatternDaysOfWeek.Saturday)
                            {
                                bCheckDay = true;
                            }
                            if ((dataperiodicita.DayOfWeek == System.DayOfWeek.Sunday) & (oRecurrence.PatternDaysOfWeek & RecurrencePatternDaysOfWeek.Sunday) == RecurrencePatternDaysOfWeek.Sunday)
                            {
                                bCheckDay = true;
                            }

                            if (bCheckDay == true)
                            {
                                
                                DateTime dataorainizio = new DateTime(dataperiodicita.Year, dataperiodicita.Month, dataperiodicita.Day, ((DateTime)this.udteOraInizio_ST.Value).Hour, ((DateTime)this.udteOraInizio_ST.Value).Minute, 0);
                                DateTime dataorafine = new DateTime(dataperiodicita.Year, dataperiodicita.Month, dataperiodicita.Day, ((DateTime)this.udteOraFine_ST.Value).Hour, ((DateTime)this.udteOraFine_ST.Value).Minute, 0);

                                if (!dateDaEscludere.Contains(dataorainizio.Date)
                                    && !AppuntamentoHaSovrapposizioni(dataorainizio, dataorafine))
                                {
                                    listaDateAppuntamenti.Add(new IntervalloTempi(dataorainizio, dataorafine, "", ""));
                                }
                            }

                            if (dataperiodicita.DayOfWeek == System.DayOfWeek.Sunday)
                            {
                                dataperiodicita = dataperiodicita.AddDays(7 * (oRecurrence.PatternInterval - 1));
                            }

                            if (dataperiodicita.Date >= oRecurrence.RangeEndDate)
                            {
                                bCheckDay = true;
                            }

                            if (iMaxOccorrenze > 0 && iMaxOccorrenze == listaDateAppuntamenti.Count) break;
                        }

                        if (iMaxOccorrenze > 0 && iMaxOccorrenze == listaDateAppuntamenti.Count) break;
                    }
                }
            }

            return listaDateAppuntamenti;
        }

                                        private List<DateTime> getListDateDaEscludere()
        {
            List<DateTime> ret = new List<DateTime>();

            bool bEscludiFestivita = false;
            switch (ucEasyOptionSet.CheckedItem.DataValue.ToString())
            {
                case _C_MULTIPLO:
                    bEscludiFestivita = this.ucEasyCheckEditorFestivita_ML.Checked;
                    break;
                default:
                    bEscludiFestivita = this.ucEasyCheckEditorFestivita_ST.Checked;
                    break;
            }

            if (bEscludiFestivita)
            {
                                List<string> codiciAgende = new List<string>();
                foreach (var item in CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Elementi)
                {
                    if (item.Selezionata && !codiciAgende.Contains(item.CodAgenda)) codiciAgende.Add(item.CodAgenda);
                }

                                using (DataTable dt = CoreStatics.getDataTableFestivita(codiciAgende))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (!ret.Contains(((DateTime)dr["Data"]).Date)) ret.Add(((DateTime)dr["Data"]).Date);
                    }
                }
            }

            return ret;
        }

                                        private bool getEscludiFestivitaDefault()
        {
            bool bEscludiFestivita = false;
            try
            {
                string sWhere = "";
                foreach (var item in CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Elementi)
                {
                    if (item.Selezionata)
                    {
                        if (sWhere != "") sWhere += @", ";
                        sWhere += @"'" + Database.testoSQL(item.CodAgenda) + @"'";
                    }
                }
                if (sWhere != "")
                {
                    string sql = @"Select EscludiFestivita From T_Agende Where EscludiFestivita = 1 And Codice IN (" + sWhere + ")";
                    using (DataTable dtTmp = Database.GetDatatable(sql))
                    {
                        if (dtTmp != null && dtTmp.Rows.Count > 0) bEscludiFestivita = true;
                    }
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            return bEscludiFestivita;
        }

                                        private bool getEscludiSovrapposizioniDefault()
        {
            bool bEscludiSovrapposizioni = false;
            try
            {
                string sWhere = "";
                foreach (var item in CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Elementi)
                {
                    if (item.Selezionata)
                    {
                        if (sWhere != "") sWhere += @", ";
                        sWhere += @"'" + Database.testoSQL(item.CodAgenda) + @"'";
                    }
                }
                if (sWhere != "")
                {
                    string sql = @"Select EscludiSovrapposizioni From T_AssAgendeTipoAppuntamenti Where EscludiSovrapposizioni = 1 And CodAgenda IN (" + sWhere + ") And CodTipoApp = '" + CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodTipoAppuntamento + @"'";
                    using (DataTable dtTmp = Database.GetDatatable(sql))
                    {
                        if (dtTmp != null && dtTmp.Rows.Count > 0) bEscludiSovrapposizioni = true;
                    }
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            return bEscludiSovrapposizioni;
        }

                                                        private bool AppuntamentoHaSovrapposizioni(DateTime dtInizioAppuntamento, DateTime dtFineAppuntamento)
        {
            bool bSovrapposizioni = false;
            bool bVerificaSovrapposizioni = false;

            switch (ucEasyOptionSet.CheckedItem.DataValue.ToString())
            {
                case _C_MULTIPLO:
                    bVerificaSovrapposizioni = this.ucEasyCheckEditorEscludiSovrapposizioni_ML.Checked;
                    break;
                default:
                    bVerificaSovrapposizioni = this.ucEasyCheckEditorEscludiSovrapposizioni_ST.Checked;
                    break;
            }

            if (bVerificaSovrapposizioni)
            {
                                List<string> agende = new List<string>();
                foreach (var item in CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Elementi)
                {
                    if (item.Selezionata)
                    {
                        agende.Add(item.CodAgenda);
                    }
                }


                                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.ParametroRipetibile.Add("CodAgenda", agende.ToArray());
                op.Parametro.Add("DataInizio", dtInizioAppuntamento.ToString(@"yyyy-MM-dd HH:mm:ss").Replace(@".", @":"));
                op.Parametro.Add("DataFine", dtFineAppuntamento.ToString(@"yyyy-MM-dd HH:mm:ss").Replace(@".", @":"));

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                using (DataSet oDs = Database.GetDatasetStoredProc("MSP_ControlloSovrapposizioni", spcoll))
                {
                    if (oDs != null && oDs.Tables.Count == 1 && oDs.Tables[0].Rows.Count == 1)
                    {
                        if (!oDs.Tables[0].Rows[0].IsNull(0) && (int)oDs.Tables[0].Rows[0][0] > 0) bSovrapposizioni = true;
                    }
                }
            }
            else
                bSovrapposizioni = false;

            return bSovrapposizioni;
        }

        private void ucEasyGrid_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            int refWidth = (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XLarge) * 3;
                        e.Layout.Bands[0].HeaderVisible = false;
                        foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
            {
                oCol.SortIndicator = SortIndicator.Disabled;
                switch (oCol.Key)
                {
                    case "DataOraInizio":
                        oCol.Header.Caption = "Data/Ora Inizio";
                        oCol.Format = "dddd dd/MM/yyyy HH:mm";
                        oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                        try
                        {
                            oCol.MaxWidth = refWidth * 3;
                            oCol.MinWidth = oCol.MaxWidth;
                            oCol.Width = oCol.MaxWidth;
                        }
                        catch (Exception)
                        {
                        }
                        break;

                    case "DataOraFine":
                        oCol.Hidden = (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.TimeSlotInterval == 0);
                        oCol.Header.Caption = "Data/Ora Fine";
                        oCol.Format = "dddd dd/MM/yyyy HH:mm";
                        oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                        try
                        {
                            oCol.MaxWidth = refWidth * 3;
                            oCol.MinWidth = oCol.MaxWidth;
                            oCol.Width = oCol.MaxWidth;
                        }
                        catch (Exception)
                        {
                        }
                        break;

                    case "EtichettaTempo":
                        oCol.Hidden = true;
                        oCol.Header.Caption = "Descrizione Somministrazione";
                        oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;

                                                break;
                    default:
                        oCol.Hidden = true;
                        break;
                }

                                e.Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            }
        }

        private void initTabSettimanale()
        {
            this.uceDaysOfWeekLun.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_NO_32);
            this.uceDaysOfWeekMar.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_NO_32);
            this.uceDaysOfWeekMer.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_NO_32);
            this.uceDaysOfWeekGio.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_NO_32);
            this.uceDaysOfWeekVen.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_NO_32);
            this.uceDaysOfWeekSab.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_NO_32);
            this.uceDaysOfWeekDom.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_NO_32);

            this.uceDaysOfWeekLun.CheckedAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_SI_32);
            this.uceDaysOfWeekMar.CheckedAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_SI_32);
            this.uceDaysOfWeekMer.CheckedAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_SI_32);
            this.uceDaysOfWeekGio.CheckedAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_SI_32);
            this.uceDaysOfWeekVen.CheckedAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_SI_32);
            this.uceDaysOfWeekSab.CheckedAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_SI_32);
            this.uceDaysOfWeekDom.CheckedAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_SI_32);

            this.ucSelPeriodicitaSettimane_ST.Value = 1;

            this.uceDaysOfWeekLun.Checked = false;             this.uceDaysOfWeekMar.Checked = false;             this.uceDaysOfWeekMer.Checked = false;             this.uceDaysOfWeekGio.Checked = false;             this.uceDaysOfWeekVen.Checked = false;             this.uceDaysOfWeekSab.Checked = false;             this.uceDaysOfWeekDom.Checked = false; 
        }

        #endregion

        #region SubRoutine

                
        
                                                        
                                                        
                                                                                                
        
                                
                                                                                                                                                                                                                                                        
                                                                                                
        
                                
                                                                                                                                                                                                                                
                
                        
        
                        
                
                                                                
                                                
        
        
                                                                                                                
        
        
        public bool Salva()
        {

            bool bReturn = false;

            try
            {
                if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Azione != EnumAzioni.VIS)
                {
                                        if (ControllaValori(out List<IntervalloTempi> listadateappuntamenti))
                    {
                        this.ImpostaCursore(enum_app_cursors.WaitCursor);

                        if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Azione == EnumAzioni.INS)
                        {
                            
                                                        if (TipoAppuntamentoSettimanale && ucEasyOptionSet.CheckedItem.DataValue.ToString() == _C_SETTIMANALE)
                            {
                                #region creazione appuntamenti settimanali 
                                                                Guid idgruppo = Guid.NewGuid();
                                
                                if (listadateappuntamenti.Count > 0)
                                {
                                                                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Oggetto = this.ucEasyLabelOggetto.Text;
                                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.ElencoRisorse = this.ucEasyLabelAgende.Text;
                                                                        var dataapp1 = listadateappuntamenti[0];
                                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio = dataapp1.DataOraInizio;
                                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine = dataapp1.DataOraFine;
                                                                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodSistema = EnumSistemi.APP.ToString();
                                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.IDGruppo = idgruppo.ToString();

                                                                        PluginClientStatics.PluginClient(EnumPluginClient.APP_PRE_SALVA.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                                                                        bReturn = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Salva();

                                                                        if (bReturn)
                                    {
                                        CoreStatics.CoreApplication.MovAppuntamentiGenerati.Add(CoreStatics.CoreApplication.MovAppuntamentoSelezionato);

                                        if (listadateappuntamenti.Count > 1)
                                        {

                                                                                        int iMaxOccorrenze = 0;
                                            if (this.ucSelOccorrenze_ST.Value != null) iMaxOccorrenze = (int)this.ucSelOccorrenze_ST.Value;
                                            if (iMaxOccorrenze <= 0 || iMaxOccorrenze > listadateappuntamenti.Count) iMaxOccorrenze = listadateappuntamenti.Count;

                                            MovAppuntamento primo = CoreStatics.CoreApplication.MovAppuntamentoSelezionato;
                                            for (int i = 1; i < iMaxOccorrenze; i++)
                                            {
                                                var dataappuntamento = listadateappuntamenti[i];
                                                MovAppuntamento newMovAppuntamento = new MovAppuntamento(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.MovAppuntamentoSelezionato.IDPaziente, CoreStatics.CoreApplication.MovAppuntamentoSelezionato.IDEpisodio, CoreStatics.CoreApplication.MovAppuntamentoSelezionato.IDTrasferimento);
                                                newMovAppuntamento.CopiaDaOrigine(ref primo);
                                                                                                newMovAppuntamento.DataInizio = dataappuntamento.DataOraInizio;
                                                newMovAppuntamento.DataFine = dataappuntamento.DataOraFine;
                                                                                                newMovAppuntamento.CodSistema = EnumSistemi.APP.ToString();
                                                newMovAppuntamento.IDGruppo = idgruppo.ToString();
                                                                                                                                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato = newMovAppuntamento;
                                                PluginClientStatics.PluginClient(EnumPluginClient.APP_PRE_SALVA.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                                                newMovAppuntamento = CoreStatics.CoreApplication.MovAppuntamentoSelezionato;
                                                                                                if (newMovAppuntamento.Salva())
                                                {
                                                    CoreStatics.CoreApplication.MovAppuntamentiGenerati.Add(newMovAppuntamento);
                                                }
                                            }
                                            CoreStatics.CoreApplication.MovAppuntamentoSelezionato = primo;
                                        }
                                    }
                                }
                                #endregion
                            }

                                                        else if (TipoAppuntamentoMultiplo && ucEasyOptionSet.CheckedItem.DataValue.ToString() == _C_MULTIPLO)
                            {
                                #region creazione appuntamenti multipli
                                                                Guid idgruppo = Guid.NewGuid();
                                
                                if (listadateappuntamenti.Count > 0)
                                {
                                                                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Oggetto = this.ucEasyLabelOggetto.Text;
                                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.ElencoRisorse = this.ucEasyLabelAgende.Text;
                                                                        var dataapp1 = listadateappuntamenti[0];
                                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio = dataapp1.DataOraInizio;
                                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine = dataapp1.DataOraFine;
                                                                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodSistema = EnumSistemi.APP.ToString();
                                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.IDGruppo = idgruppo.ToString();

                                                                        PluginClientStatics.PluginClient(EnumPluginClient.APP_PRE_SALVA.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                                                                        bReturn = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Salva();

                                                                        if (bReturn)
                                    {
                                        CoreStatics.CoreApplication.MovAppuntamentiGenerati.Add(CoreStatics.CoreApplication.MovAppuntamentoSelezionato);

                                        if (listadateappuntamenti.Count > 1)
                                        {
                                                                                        int iMaxOccorrenze = 0;
                                            if (this.ucSelOccorrenze_ML.Value != null) iMaxOccorrenze = (int)this.ucSelOccorrenze_ML.Value;
                                            if (iMaxOccorrenze <= 0 || iMaxOccorrenze > listadateappuntamenti.Count) iMaxOccorrenze = listadateappuntamenti.Count;

                                            MovAppuntamento primo = CoreStatics.CoreApplication.MovAppuntamentoSelezionato;
                                            for (int i = 1; i < iMaxOccorrenze; i++)
                                            {
                                                var dataappuntamento = listadateappuntamenti[i];
                                                MovAppuntamento newMovAppuntamento = new MovAppuntamento(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.MovAppuntamentoSelezionato.IDPaziente, CoreStatics.CoreApplication.MovAppuntamentoSelezionato.IDEpisodio, CoreStatics.CoreApplication.MovAppuntamentoSelezionato.IDTrasferimento);
                                                newMovAppuntamento.CopiaDaOrigine(ref primo);
                                                                                                newMovAppuntamento.DataInizio = dataappuntamento.DataOraInizio;
                                                newMovAppuntamento.DataFine = dataappuntamento.DataOraFine;
                                                                                                newMovAppuntamento.CodSistema = EnumSistemi.APP.ToString();
                                                newMovAppuntamento.IDGruppo = idgruppo.ToString();
                                                                                                                                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato = newMovAppuntamento;
                                                PluginClientStatics.PluginClient(EnumPluginClient.APP_PRE_SALVA.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                                                newMovAppuntamento = CoreStatics.CoreApplication.MovAppuntamentoSelezionato;
                                                                                                if (newMovAppuntamento.Salva())
                                                {
                                                    CoreStatics.CoreApplication.MovAppuntamentiGenerati.Add(newMovAppuntamento);
                                                }
                                            }
                                            CoreStatics.CoreApplication.MovAppuntamentoSelezionato = primo;
                                        }
                                    }
                                }
                                #endregion
                            }

                                                        else if (TipoAppuntamentoSenzaData && ucEasyOptionSet.CheckedItem.DataValue.ToString() == _C_DAPIANIFICARE)
                            {

                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Oggetto = this.ucEasyLabelOggetto.Text;
                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato.ElencoRisorse = this.ucEasyLabelAgende.Text;

                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento = EnumStatoAppuntamento.DP.ToString();
                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio = DateTime.MinValue;
                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine = DateTime.MinValue;

                                                                Guid idgruppo = Guid.Empty;
                                int iNumAppuntamenti = 0;
                                if (this.ucSelOccorrenze_DP.Value != null) iNumAppuntamenti = (int)this.ucSelOccorrenze_DP.Value;
                                if (iNumAppuntamenti > 1)
                                {
                                    idgruppo = Guid.NewGuid();
                                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.IDGruppo = idgruppo.ToString();
                                }

                                                                PluginClientStatics.PluginClient(EnumPluginClient.APP_PRE_SALVA.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                                                                bReturn = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Salva();

                                if (bReturn && iNumAppuntamenti > 1)
                                {
                                                                        CoreStatics.CoreApplication.MovAppuntamentiGenerati.Add(CoreStatics.CoreApplication.MovAppuntamentoSelezionato);
                                    MovAppuntamento primo = CoreStatics.CoreApplication.MovAppuntamentoSelezionato;
                                    for (int i = 1; i < iNumAppuntamenti; i++)
                                    {
                                        MovAppuntamento newMovAppuntamento = new MovAppuntamento(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.MovAppuntamentoSelezionato.IDPaziente, CoreStatics.CoreApplication.MovAppuntamentoSelezionato.IDEpisodio, CoreStatics.CoreApplication.MovAppuntamentoSelezionato.IDTrasferimento);
                                        newMovAppuntamento.CopiaDaOrigine(ref primo);
                                                                                newMovAppuntamento.CodSistema = EnumSistemi.APP.ToString();
                                        newMovAppuntamento.IDGruppo = idgruppo.ToString();
                                                                                                                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato = newMovAppuntamento;
                                        PluginClientStatics.PluginClient(EnumPluginClient.APP_PRE_SALVA.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                                        newMovAppuntamento = CoreStatics.CoreApplication.MovAppuntamentoSelezionato;
                                                                                if (newMovAppuntamento.Salva())
                                        {
                                            CoreStatics.CoreApplication.MovAppuntamentiGenerati.Add(newMovAppuntamento);
                                        }
                                    }
                                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato = primo;
                                }

                            }

                                                        else if (ucEasyOptionSet.CheckedItem.DataValue.ToString() == _C_SINGOLO)
                            {

                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Oggetto = this.ucEasyLabelOggetto.Text;
                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato.ElencoRisorse = this.ucEasyLabelAgende.Text;

                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento = EnumStatoAppuntamento.PR.ToString();
                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio = (DateTime)this.udteDataInizio.Value;
                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine = (DateTime)this.udteDataFine.Value;

                                if (ControllaMassimali())
                                {
                                                                        PluginClientStatics.PluginClient(EnumPluginClient.APP_PRE_SALVA.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                                                                        bReturn = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Salva();
                                }
                                else
                                {
                                    bReturn = false;
                                }
                            }
                        }
                        else
                        {
                                                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Oggetto = this.ucEasyLabelOggetto.Text;
                            CoreStatics.CoreApplication.MovAppuntamentoSelezionato.ElencoRisorse = this.ucEasyLabelAgende.Text;

                                                        if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento == EnumStatoAppuntamento.DP.ToString())
                            {
                                                                if (TipoAppuntamentoSenzaDataSempre == true)
                                {
                                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio = DateTime.MinValue;
                                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine = DateTime.MinValue;
                                }
                                else
                                {
                                                                        if (this.ucEasyCheckEditorDP.Visible && this.ucEasyCheckEditorDP.Checked == false)
                                    {
                                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento = EnumStatoAppuntamento.PR.ToString();

                                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio = (DateTime)this.udteDataInizio.Value;
                                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine = (DateTime)this.udteDataFine.Value;
                                    }
                                    else
                                    {
                                                                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio = DateTime.MinValue;
                                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine = DateTime.MinValue;
                                    }
                                }
                            }
                            else
                            {
                                                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio = (DateTime)this.udteDataInizio.Value;
                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine = (DateTime)this.udteDataFine.Value;
                            }

                            if (ControllaMassimali())
                            {
                                Risposta oRispostaElaboraAnnullaPrimaPU = new Risposta();
                                oRispostaElaboraAnnullaPrimaPU.Successo = true;
                                if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento == EnumStatoAppuntamento.AN.ToString())
                                {
                                    oRispostaElaboraAnnullaPrimaPU = PluginClientStatics.PluginClient(EnumPluginClient.APP_ANNULLA_PRIMA_PU.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                                }
                                if (oRispostaElaboraAnnullaPrimaPU.Successo)
                                {
                                                                        PluginClientStatics.PluginClient(EnumPluginClient.APP_PRE_SALVA.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                                                                        bReturn = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Salva();
                                }
                            }
                            else
                            {
                                bReturn = false;
                            }

                        }
                    }
                    else
                        bReturn = false;
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Salva", this.Text);
            }
            finally
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }

            return bReturn;

        }

                                                private void SetMinMAXOccorrenze(bool bAppMultipli, bool bAppSettimanali)
        {
            _bruntime = true;

            if (bAppMultipli)
            {
                if (this.ucEasyCheckEditorEscludiSovrapposizioni_ML.Checked)
                {
                    if ((int)this.ucSelOccorrenze_ML.Value > _MAXOccorrenze || ((int)this.ucSelOccorrenze_ML.Value == 0 && _MAXOccorrenze > 0))
                        this.ucSelOccorrenze_ML.Value = _MAXOccorrenze;

                    this.ucSelOccorrenze_ML.MaxValue = _MAXOccorrenze;
                    this.ucSelOccorrenze_ML.MinValue = _minOccorrenze;
                }
                else
                {
                    this.ucSelOccorrenze_ML.MaxValue = 9999;
                    this.ucSelOccorrenze_ML.MinValue = 0;
                    this.ucSelOccorrenze_ML.Value = 0;
                }
            }

            if (bAppSettimanali)
            {
                if (this.ucEasyCheckEditorEscludiSovrapposizioni_ST.Checked)
                {
                    if ((int)this.ucSelOccorrenze_ST.Value > _MAXOccorrenze || ((int)this.ucSelOccorrenze_ST.Value == 0 && _MAXOccorrenze > 0))
                        this.ucSelOccorrenze_ST.Value = _MAXOccorrenze;

                    this.ucSelOccorrenze_ST.MaxValue = _MAXOccorrenze;
                    this.ucSelOccorrenze_ST.MinValue = _minOccorrenze;
                }
                else
                {
                    this.ucSelOccorrenze_ST.MaxValue = 9999;
                    this.ucSelOccorrenze_ST.MinValue = 0;
                    this.ucSelOccorrenze_ST.Value = 0;
                }
            }

            _bruntime = false;
        }

        private void CaricaScheda()
        {
            try
            {

                this.ucAnteprimaRtf.MovScheda = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.MovScheda;
                this.ucAnteprimaRtf.MovScheda.GeneraRTF();
                this.ucAnteprimaRtf.RefreshRTF();

            }
            catch (Exception ex)
            {
                throw new Exception(@"CaricaScheda()" + Environment.NewLine + ex.Message, ex);
            }
        }

        private bool ControllaValori(out List<IntervalloTempi> outListaDateAppuntamenti)
        {
            this.ImpostaCursore(enum_app_cursors.WaitCursor);
            try
            {
                outListaDateAppuntamenti = null;

                bool bOK = true;

                                if (bOK
                    && !(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Azione == EnumAzioni.INS
                        && TipoAppuntamentoMultiplo
                        && ucEasyOptionSet.CheckedItem.DataValue.ToString() == _C_MULTIPLO)
                    && !(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Azione == EnumAzioni.INS
                        && TipoAppuntamentoSettimanale
                        && ucEasyOptionSet.CheckedItem.DataValue.ToString() == _C_SETTIMANALE)
                    && !(this.ucEasyCheckEditorDP.Visible && this.ucEasyCheckEditorDP.Checked)
                    && TipoAppuntamentoSenzaDataSempre == false
                    )
                {
                    int result = DateTime.Compare((DateTime)this.udteDataInizio.Value, (DateTime)this.udteDataFine.Value);
                    if (result >= 0)
                    {
                        easyStatics.EasyMessageBox("Data e ora Inizio/Fine NON corrette !", "Appuntamento", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.udteDataInizio.Focus();
                        bOK = false;
                    }

                }

                if (bOK && CoreStatics.CoreApplication.MovAppuntamentoSelezionato.MovScheda.DatiObbligatoriMancantiRTF != string.Empty)
                {
                    easyStatics.EasyMessageBox(@"Non sono stati compilati alcuni valori obbligatori della scheda!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this.ucDcViewer.Focus();
                    bOK = false;
                }

                if (bOK
                    && CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Azione == EnumAzioni.INS
                    && TipoAppuntamentoMultiplo
                    && ucEasyOptionSet.CheckedItem.DataValue.ToString() == _C_MULTIPLO)
                {
                    outListaDateAppuntamenti = getListaDatePerAppuntamentoMultiplo(true);
                    if (outListaDateAppuntamenti.Count <= 0)
                    {
                        easyStatics.EasyMessageBox("Data e ora Inizio/Fine NON corrette !", "Appuntamento Multiplo", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.udteDataOraInizio_ML.Focus();
                        bOK = false;
                    }
                    else
                    {
                        if (!this.ucEasyCheckEditorEscludiSovrapposizioni_ML.Checked && _MAXOccorrenze > 0 && _MAXOccorrenze < outListaDateAppuntamenti.Count)
                        {
                            if (easyStatics.EasyMessageBox(@"Stai cercando di prenotare " + outListaDateAppuntamenti.Count.ToString("#,##0") + @" appuntamenti," + Environment.NewLine
                                                        + @"il massimo previsto è " + _MAXOccorrenze.ToString("#,##0") + @"." + Environment.NewLine
                                                        + @"Sei sicuro di voler continuare?", "Appuntamento Multiplo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                            {
                                this.ucSelOccorrenze_ML.Focus();
                                bOK = false;
                            }
                        }
                    }
                }

                if (bOK
                    && CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Azione == EnumAzioni.INS
                    && TipoAppuntamentoSettimanale
                    && ucEasyOptionSet.CheckedItem.DataValue.ToString() == _C_SETTIMANALE)
                {
                    if ((int)this.ucSelPeriodicitaSettimane_ST.Value <= 0)
                    {
                        easyStatics.EasyMessageBox("Inserire intervallo!", "Appuntamento Settimanale", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.ucSelPeriodicitaSettimane_ST.Focus();
                        bOK = false;
                    }
                }

                if (bOK
                    && CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Azione == EnumAzioni.INS
                    && TipoAppuntamentoSettimanale
                    && ucEasyOptionSet.CheckedItem.DataValue.ToString() == _C_SETTIMANALE)
                {
                    outListaDateAppuntamenti = getListaDatePerAppuntamentoSettimanale(true);
                    if (outListaDateAppuntamenti.Count <= 0)
                    {
                        easyStatics.EasyMessageBox("Date Inizio/Fine e/o giorni settimana NON corretti!", "Appuntamento Settimanale", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.udteDataInizio_ST.Focus();
                        bOK = false;
                    }
                    else
                    {
                        if (!this.ucEasyCheckEditorEscludiSovrapposizioni_ST.Checked && _MAXOccorrenze > 0 && _MAXOccorrenze < outListaDateAppuntamenti.Count)
                        {
                            if (easyStatics.EasyMessageBox(@"Stai cercando di prenotare " + outListaDateAppuntamenti.Count.ToString("#,##0") + @" appuntamenti," + Environment.NewLine
                                                        + @"il massimo previsto è " + _MAXOccorrenze.ToString("#,##0") + @"." + Environment.NewLine
                                                        + @"Sei sicuro di voler continuare?", "Appuntamento Settimanale", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                            {
                                this.ucSelOccorrenze_ST.Focus();
                                bOK = false;
                            }
                        }
                    }
                }

                return bOK;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }

        }

        private bool ControllaMassimali()
        {

            StringBuilder sb_agendenondisponibili = new StringBuilder();

            try
            {

                Dictionary<string, MassimaliAgenda> dict_massimaliagenda = new Dictionary<string, MassimaliAgenda>();
                List<string> list_agende = new List<string>();
                List<IntervalloTempi> list_intervallotempi = new List<IntervalloTempi>();

                                foreach (MovAppuntamentoAgende maa in CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Elementi)
                {
                    if (maa.Selezionata)
                    {
                        dict_massimaliagenda.Add(maa.CodAgenda, getMassimaleAgenda(maa.CodAgenda));
                        list_agende.Add(maa.CodAgenda);
                    }
                }

                                if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Azione == EnumAzioni.INS)
                {
                                                            if (TipoAppuntamentoSettimanale && ucEasyOptionSet.CheckedItem.DataValue.ToString() == _C_SETTIMANALE)
                    {
                        list_intervallotempi = (List<IntervalloTempi>)this.ucEasyGrid.DataSource;
                    }
                                        else if (TipoAppuntamentoMultiplo && ucEasyOptionSet.CheckedItem.DataValue.ToString() == _C_MULTIPLO)
                    {
                        list_intervallotempi = (List<IntervalloTempi>)this.ucEasyGrid.DataSource;
                    }
                                        else if (TipoAppuntamentoSenzaData && ucEasyOptionSet.CheckedItem.DataValue.ToString() == _C_DAPIANIFICARE)
                    {
                        return true;
                    }
                                        else if (ucEasyOptionSet.CheckedItem.DataValue.ToString() == _C_SINGOLO)
                    {

                    }
                }
                else
                {
                                        if (TipoAppuntamentoSenzaDataSempre == true)
                    {
                        return true;
                    }
                    else
                    {
                                                MovAppuntamento oMovApp = new MovAppuntamento(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.IDAppuntamento);
                        if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine.Date.Equals(oMovApp.DataFine.Date))
                        {
                            oMovApp = null;
                            return true;
                        }
                        oMovApp = null;
                    }
                }

                                DateTime dtinizio = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio.Date;
                DateTime dtfine = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine.Date.AddDays(1);
                if (list_intervallotempi.Count > 0)
                {
                    dtfine = (from x in list_intervallotempi select x).Max(c => c.DataOraFine.Date).AddDays(1);
                }

                                DataTable dtdisponibilita = getDataTableDisponibilita(dtinizio, dtfine, list_agende);

                                foreach (KeyValuePair<string, MassimaliAgenda> oMa in dict_massimaliagenda)
                {

                                        if (list_intervallotempi.Count() == 0)
                    {
                                                if (dict_massimaliagenda[oMa.Key] != null && dict_massimaliagenda[oMa.Key].Massimale[(int)dtinizio.DayOfWeek] > 0)
                        {
                            int nDisp = getDataTableDisponibilitaAgendaGiorno(dtdisponibilita, oMa.Key, dtinizio);
                            if (dict_massimaliagenda[oMa.Key].Massimale[(int)dtinizio.DayOfWeek] <= nDisp)
                            {
                                sb_agendenondisponibili.Append(string.Format("{0} {1:dd/MM/yyyy} ({2}/{3}) NON disponibile.\n",
                                                                                getDescrizioneAgenda(oMa.Key), dtinizio, nDisp, dict_massimaliagenda[oMa.Key].Massimale[(int)dtinizio.DayOfWeek]));
                            }
                        }
                    }
                    else
                    {
                                                foreach (IntervalloTempi oIntervalloTempi in list_intervallotempi)
                        {
                            if (dict_massimaliagenda[oMa.Key] != null && dict_massimaliagenda[oMa.Key].Massimale[(int)oIntervalloTempi.DataOraInizio.Date.DayOfWeek] > 0)
                            {
                                int nDisp = getDataTableDisponibilitaAgendaGiorno(dtdisponibilita, oMa.Key, oIntervalloTempi.DataOraInizio.Date);
                                if (dict_massimaliagenda[oMa.Key].Massimale[(int)oIntervalloTempi.DataOraInizio.Date.DayOfWeek] <= nDisp)
                                {
                                    sb_agendenondisponibili.Append(string.Format("{0} {1:dd/MM/yyyy} ({2}/{3}) NON disponibile.\n",
                                                                                getDescrizioneAgenda(oMa.Key), oIntervalloTempi.DataOraInizio, nDisp, dict_massimaliagenda[oMa.Key].Massimale[(int)oIntervalloTempi.DataOraInizio.DayOfWeek]));
                                }
                            }
                        }
                    }

                }

            }
            catch (Exception)
            {
                sb_agendenondisponibili.Append("Errore recupero massimali.");
            }

            if (sb_agendenondisponibili.Length > 0)
            {
                easyStatics.EasyMessageBox(sb_agendenondisponibili.ToString(), "Massimali", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return (sb_agendenondisponibili.Length == 0);

        }

        private MassimaliAgenda getMassimaleAgenda(string codagenda)
        {

            MassimaliAgenda _MassimaliAgenda = null;

            try
            {

                                                                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodAgenda", codagenda);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                op.Parametro.Add("DatiEstesi", "1");
                op.Parametro.Add("SoloFiltroAgenda", "0");

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataSet oDs = Database.GetDatasetStoredProc("MSP_SelAgende", spcoll);

                if (oDs != null && oDs.Tables.Count == 1 && oDs.Tables[0].Rows.Count == 1 && !oDs.Tables[0].Rows[0].IsNull("Risorse"))
                {
                    _MassimaliAgenda = XmlProcs.XmlDeserializeFromString<MassimaliAgenda>(oDs.Tables[0].Rows[0]["Risorse"].ToString());
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "getMassimaleAgenda", "Common");
            }

            return _MassimaliAgenda;

        }

        private DataTable getDataTableDisponibilita(DateTime datainizio, DateTime datafine, List<string> listagende)
        {
                                                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
            op.Parametro.Add("DataInizio", Database.dataOra105PerParametri(datainizio));
            op.Parametro.Add("DataFine", Database.dataOra105PerParametri(datafine));


            op.ParametroRipetibile.Add("CodAgenda", listagende.ToArray());


                        SqlParameterExt[] spcoll = new SqlParameterExt[1];

            string xmlParam = XmlProcs.XmlSerializeToString(op);
            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

            DataTable dt = Database.GetDataTableStoredProc("MSP_SelInfoAppuntamentiAgendeGiorni", spcoll);

                        dt = dt.DefaultView.ToTable();

            return dt;
        }

        private int getDataTableDisponibilitaAgendaGiorno(DataTable dt, string agenda, DateTime giorno)
        {

            int nRet = 0;

            try
            {

                var query = from p in dt.AsEnumerable()
                            where p.Field<string>("CodAgenda") == agenda && p.Field<DateTime>("Data").Equals(giorno)
                            select p;
                if (query != null && query.Count() == 1)
                {
                    nRet = (int)query.ToList()[0]["Qta"];
                }

            }
            catch (Exception)
            {


            }

            return nRet;

        }

        private string getDescrizioneAgenda(string codagenda)
        {

            string sret = codagenda;

            try
            {

                sret = (from x in CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Elementi select x).FirstOrDefault(c => c.CodAgenda == codagenda).Descrizione;

            }
            catch (Exception)
            {

            }

            return sret;

        }

        private ucEasyListBox GetEasyListBoxForPopupControlContainer(object sender)
        {

            DateTime dt = (DateTime)((ucEasyDateTimeEditor)sender).Value;

            ucEasyListBox _ucEasyListBox = new ucEasyListBox();

            try
            {

                _ucEasyListBox.Size = new Size(150, 300);
                _ucEasyListBox.ItemSettings.SelectionType = Infragistics.Win.UltraWinListView.SelectionType.Single;
                _ucEasyListBox.TextFontRelativeDimension = ((ucEasyDateTimeEditor)sender).TextFontRelativeDimension;
                _ucEasyListBox.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
                _ucEasyListBox.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Details;
                _ucEasyListBox.ViewSettingsDetails.AllowColumnMoving = false;
                _ucEasyListBox.ViewSettingsDetails.AllowColumnSizing = false;
                _ucEasyListBox.ViewSettingsDetails.AllowColumnSorting = false;
                _ucEasyListBox.ViewSettingsDetails.AutoFitColumns = Infragistics.Win.UltraWinListView.AutoFitColumns.ResizeAllColumns;
                _ucEasyListBox.ViewSettingsDetails.FullRowSelect = true;
                _ucEasyListBox.ViewSettingsDetails.ImageSize = new System.Drawing.Size(0, 0);
                _ucEasyListBox.ViewSettingsList.ImageSize = new System.Drawing.Size(0, 0);

                UltraListViewItem oVal = null;
                DateTime valoreitem = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);

                _ucEasyListBox.Items.Clear();

                for (int i = 0; i < 24; i++)
                {
                    oVal = new Infragistics.Win.UltraWinListView.UltraListViewItem(i.ToString());
                    oVal.Value = valoreitem.ToString("HH:mm");
                    _ucEasyListBox.Items.Add(oVal);
                    valoreitem = valoreitem.AddHours(1);
                }

            }
            catch (Exception)
            {

            }

            return _ucEasyListBox;

        }

        private void SetDC(bool bDc)
        {

            this.ucEasyTableLayoutPanelDC.Visible = false;

            switch (bDc)
            {

                case false:
                    this.ucEasyTableLayoutPanelDC.ColumnStyles[0].Width = 90;
                    this.ucEasyTableLayoutPanelDC.ColumnStyles[1].Width = 0;
                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.MovScheda.DatiXML = this.ucDcViewer.Gestore.SchedaDatiXML;
                    this.CaricaScheda();
                    break;

                case true:
                    this.ucEasyTableLayoutPanelDC.ColumnStyles[0].Width = 0;
                    this.ucEasyTableLayoutPanelDC.ColumnStyles[1].Width = 90;
                    this.ucDcViewer.CaricaDati();
                    break;
            }

            this.ucEasyTableLayoutPanelDC.Visible = true;

        }

        private void InizializzaGestore()
        {

            try
            {

                                                                oGestore = CoreStatics.GetGestore();

                this.CaricaGestore();

                                this.ucDcViewer.VisualizzaTitoloScheda = false;

                this.ucDcViewer.CaricaDati(oGestore);

                this.ucDcViewer.RtfEvent -= ucDcViewer_RtfEvent;
                this.ucDcViewer.KeyEvent -= ucDcViewer_KeyEvent;
                this.ucDcViewer.ButtonEvent -= ucDcViewer_ButtonEvent;
                this.ucDcViewer.RtfEvent += ucDcViewer_RtfEvent;
                this.ucDcViewer.KeyEvent += ucDcViewer_KeyEvent;
                this.ucDcViewer.ButtonEvent += ucDcViewer_ButtonEvent;


            }
            catch (Exception ex)
            {
                throw new Exception(@"InizializzaGestore()" + Environment.NewLine + ex.Message, ex);
            }

        }

        private void CaricaGestore()
        {

            try
            {

                                oGestore.SchedaXML = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.MovScheda.Scheda.StrutturaXML;
                oGestore.SchedaLayoutsXML = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.MovScheda.Scheda.LayoutXML;
                                oGestore.Decodifiche = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.MovScheda.Scheda.DizionarioValori();

                                if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.MovScheda.DatiXML == string.Empty)
                {
                    oGestore.SchedaDati = new DcSchedaDati();
                }
                else
                {
                    try
                    {
                        oGestore.SchedaDatiXML = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.MovScheda.DatiXML;
                    }
                    catch (Exception)
                    {
                        oGestore.SchedaDati = new DcSchedaDati();
                    }
                }
                if (oGestore.SchedaDati.Dati.Count == 0) { oGestore.NuovaScheda(); }

            }
            catch (Exception ex)
            {
                throw new Exception(@"CaricaGestore()" + Environment.NewLine + ex.Message, ex);
            }

        }

        private void SalvaElementi()
        {

            try
            {
                foreach (MovAppuntamentoAgende maa in CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Elementi)
                {

                    if (maa.Selezionata)
                    {

                        Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                        op.Parametro.Add("CodAgenda", maa.CodAgenda);
                        op.Parametro.Add("DatiEstesi", "1");
                        op.Parametro.Add("Lista", "1");
                        SqlParameterExt[] spcoll = new SqlParameterExt[1];
                        string xmlParam = XmlProcs.XmlSerializeToString(op);
                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                        DataTable oDt = Database.GetDataTableStoredProc("MSP_SelMovAppuntamentiAgende", spcoll);

                        if (oDt.Rows.Count == 1)
                        {

                            ParametriListaAgenda mo_ParametriListaAgenda = new ParametriListaAgenda();
                            mo_ParametriListaAgenda = UnicodeSrl.Scci.Statics.XmlProcs.XmlDeserializeFromString<ParametriListaAgenda>(oDt.Rows[0]["ParametriLista"].ToString());

                            switch (mo_ParametriListaAgenda.TipoRaggruppamentoAgenda1)
                            {

                                case EnumTipoRaggruppamentoAgenda.Nessuno:
                                case EnumTipoRaggruppamentoAgenda.Campo:
                                case EnumTipoRaggruppamentoAgenda.Dizionario:
                                    break;

                                case EnumTipoRaggruppamentoAgenda.Scheda:
                                    KeyValuePair<string, string> okvp = mo_ParametriListaAgenda.RaggruppamentoAgenda1.FirstOrDefault(kvp => kvp.Key == CoreStatics.CoreApplication.MovAppuntamentoSelezionato.MovScheda.CodScheda);
                                    if (okvp.Key != null && okvp.Value != null)
                                    {
                                        string sval = this.ucDcViewer.Gestore.LeggeValore(okvp.Value, 1).ToString();
                                        string stra = this.ucDcViewer.Gestore.LeggeTranscodifica(okvp.Value, 1).ToString();
                                        CoreStatics.OrdinaSelezioneMultipla(ref sval, ref stra);
                                        if (maa.CodRaggr1 != sval || maa.DescrRaggr1 != stra)
                                        {
                                            maa.CodRaggr1 = sval;
                                            maa.DescrRaggr1 = stra;
                                            maa.Modificata = true;
                                        }
                                    }
                                    break;

                            }

                            switch (mo_ParametriListaAgenda.TipoRaggruppamentoAgenda2)
                            {

                                case EnumTipoRaggruppamentoAgenda.Nessuno:
                                case EnumTipoRaggruppamentoAgenda.Campo:
                                case EnumTipoRaggruppamentoAgenda.Dizionario:
                                    break;

                                case EnumTipoRaggruppamentoAgenda.Scheda:
                                    KeyValuePair<string, string> okvp = mo_ParametriListaAgenda.RaggruppamentoAgenda2.FirstOrDefault(kvp => kvp.Key == CoreStatics.CoreApplication.MovAppuntamentoSelezionato.MovScheda.CodScheda);
                                    if (okvp.Key != null && okvp.Value != null)
                                    {
                                        string sval = this.ucDcViewer.Gestore.LeggeValore(okvp.Value, 1).ToString();
                                        if (maa.CodRaggr2 != sval)
                                        {
                                            maa.CodRaggr2 = sval;
                                            maa.DescrRaggr2 = maa.CodRaggr2;
                                            maa.Modificata = true;
                                        }
                                    }
                                    break;

                            }

                            switch (mo_ParametriListaAgenda.TipoRaggruppamentoAgenda3)
                            {

                                case EnumTipoRaggruppamentoAgenda.Nessuno:
                                case EnumTipoRaggruppamentoAgenda.Campo:
                                case EnumTipoRaggruppamentoAgenda.Dizionario:
                                    break;

                                case EnumTipoRaggruppamentoAgenda.Scheda:
                                    KeyValuePair<string, string> okvp = mo_ParametriListaAgenda.RaggruppamentoAgenda3.FirstOrDefault(kvp => kvp.Key == CoreStatics.CoreApplication.MovAppuntamentoSelezionato.MovScheda.CodScheda);
                                    if (okvp.Key != null && okvp.Value != null)
                                    {
                                        string sval = this.ucDcViewer.Gestore.LeggeValore(okvp.Value, 1).ToString();
                                        if (maa.CodRaggr3 != sval)
                                        {
                                            maa.CodRaggr3 = sval;
                                            maa.DescrRaggr3 = maa.CodRaggr3;
                                            maa.Modificata = true;
                                        }
                                    }
                                    break;

                            }

                        }

                    }

                }

            }
            catch (Exception)
            {

            }

        }

        #endregion

        #region Events Form

        private void frmSelezionaAppuntamento_Shown(object sender, EventArgs e)
        {
            this.SetDC(this.ucEasyCheckEditorDC.Checked);
                    }

        private void frmSelezionaAppuntamento_ImmagineClick(object sender, ImmagineTopClickEventArgs e)
        {

            try
            {

                switch (e.Tipo)
                {

                    case EnumImmagineTop.Segnalibri:
                        if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Bookmarks > 0)
                        {
                            CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerSegnalibri);
                            _ucSegnalibri = new ucSegnalibri();
                            int iWidth = Convert.ToInt32(easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large)) * 40;
                            int iHeight = Convert.ToInt32((double)iWidth / 1.52D);
                            _ucSegnalibri.Size = new Size(iWidth, iHeight);
                            _ucSegnalibri.ViewInit();
                            _ucSegnalibri.Focus();
                            this.UltraPopupControlContainerSegnalibri.Show();
                        }
                        break;

                    case EnumImmagineTop.SegnalibroAdd:
                        break;

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void frmSelezionaAppuntamento_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {

            try
            {
                CoreStatics.CoreApplication.MovAppuntamentoSelezionato.MovScheda.DatiXML = this.ucDcViewer.Gestore.SchedaDatiXML;
                SalvaElementi();

                if (this.Salva() == true)
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void frmSelezionaAppuntamento_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion

        #region Events

        private void ucEasyCheckEditorDC_CheckedChanged(object sender, EventArgs e)
        {
            this.SetDC(this.ucEasyCheckEditorDC.Checked);
        }

        private void udteDataInizio_ValueChanged(object sender, EventArgs e)
        {

            if (this.udteDataInizio.Value != null)
                this.udteDataFine.Value = ((DateTime)this.udteDataInizio.Value).AddMinutes(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.TimeSlotInterval);


        }

        private void udteDataInizio_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "Orari" && ((ucEasyDateTimeEditor)sender).Value != null)
            {
                CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainer);
                _ucEasyListBox = GetEasyListBoxForPopupControlContainer(sender);
                this.UltraPopupControlContainer.Show((ucEasyDateTimeEditor)sender);
            }
        }

        private void udteDataFine_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "Orari" && ((ucEasyDateTimeEditor)sender).Value != null)
            {

                CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainer);
                _ucEasyListBox = GetEasyListBoxForPopupControlContainer(sender);
                this.UltraPopupControlContainer.Show((ucEasyDateTimeEditor)sender);
            }
        }

        private void ucEasyCheckEditorFestivitaSovrapp_CheckedChanged(object sender, EventArgs e)
        {
                        if (!_bruntime)
            {
                if (sender == this.ucEasyCheckEditorEscludiSovrapposizioni_ML)
                    SetMinMAXOccorrenze(true, false);
                else if (sender == this.ucEasyCheckEditorEscludiSovrapposizioni_ST)
                    SetMinMAXOccorrenze(false, true);

                                                _brefreshappuntamenti = true;
            }
        }


        private void ucEasyTabControlOrari_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
        {
            if (!_bruntime && _brefreshappuntamenti && e.Tab.Key == "Anteprima")
            {
                this.CaricaAppuntamenti();
            }
        }

        #endregion

        #region UltraPopupControlContainer ucEasyListBox

        private void UltraPopupControlContainer_Closed(object sender, EventArgs e)
        {
            _ucEasyListBox.ItemSelectionChanged -= ucEasyListBox_ItemSelectionChanged;
        }

        private void UltraPopupControlContainer_Opened(object sender, EventArgs e)
        {
            _ucEasyListBox.ItemSelectionChanged += ucEasyListBox_ItemSelectionChanged;
        }

        private void UltraPopupControlContainer_Opening(object sender, CancelEventArgs e)
        {
            Infragistics.Win.Misc.UltraPopupControlContainer popup = sender as Infragistics.Win.Misc.UltraPopupControlContainer;
            popup.PopupControl = _ucEasyListBox;
        }

        private void ucEasyListBox_ItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {

            ucEasyDateTimeEditor source = this.UltraPopupControlContainer.SourceControl as ucEasyDateTimeEditor;
            ucEasyListBox popup = this.UltraPopupControlContainer.PopupControl as ucEasyListBox;

            DateTime dt = (DateTime)source.Value;
            if (popup.ActiveItem != null)
            {
                string[] orari = popup.ActiveItem.Text.Split(':');
                DateTime newdt = new DateTime(dt.Year, dt.Month, dt.Day, int.Parse(orari[0]), int.Parse(orari[1]), 0);
                source.Value = newdt;
            }


            this.UltraPopupControlContainer.Close();
        }

        #endregion

        #region Events UserControl

        void ucDcViewer_KeyEvent(System.Windows.Input.Key key)
        {

            try
            {

                if (key == System.Windows.Input.Key.F1)
                {
                    frmSelezionaAppuntamento_PulsanteIndietroClick(null, new PulsanteBottomClickEventArgs(EnumPulsanteBottom.Indietro));
                }
                else if (key == System.Windows.Input.Key.F12)
                {
                    frmSelezionaAppuntamento_PulsanteAvantiClick(null, new PulsanteBottomClickEventArgs(EnumPulsanteBottom.Avanti));
                }

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ucDcViewer_KeyEvent", this.Text);
            }

        }

        void ucDcViewer_RtfEvent(string id)
        {

            try
            {

                string codua = "";

                                CoreStatics.CoreApplication.MovTestoPredefinitoSelezionato = new MovTestoPredefinito(UnicodeSrl.Scci.Enums.EnumEntita.SCH.ToString(), codua, CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice,
                                                                                                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodTipoAppuntamento, id);
                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(Scci.Enums.EnumMaschere.TestiPredefiniti) == System.Windows.Forms.DialogResult.OK)
                {
                                                            
                    string sRTFOriginale = this.ucDcViewer.Gestore.LeggeValore(id).ToString();
                    string sRTFDaAccodare = CoreStatics.CoreApplication.MovTestoPredefinitoSelezionato.RitornoRTF;
                    UnicodeSrl.Scci.RTF.RtfFiles rtf = new UnicodeSrl.Scci.RTF.RtfFiles();
                    sRTFOriginale = rtf.joinRtf(sRTFDaAccodare, sRTFOriginale, true);
                    rtf = null;
                    this.ucDcViewer.Gestore.ModificaValore(id, sRTFOriginale);
                }

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ucDcViewer_RtfEvent", this.Text);
            }

        }

        void ucDcViewer_ButtonEvent(string id)
        {

            string _codua = string.Empty;

            try
            {

                this.ImpostaCursore(enum_app_cursors.WaitCursor);
                this.Tag = id;
                if (CoreStatics.CoreApplication.Trasferimento != null)
                {
                    _codua = CoreStatics.CoreApplication.Trasferimento.CodUA;
                }
                else
                {
                    _codua = CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.CodUAAmbulatorialeSelezionata;
                }
                string[] azioni = id.Split('.');
                string[] campo = azioni[2].Split('_');
                string azione = string.Format("APP{0}.{1}", CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodTipoAppuntamento, campo[0]);
                                object[] myparam = new object[5] { this, campo[0], int.Parse(campo[1]), this.ucDcViewer.Gestore, azioni[0] };

                Risposta oRisposta = PluginClientStatics.PluginClient(azione, myparam, CommonStatics.UAPadri(_codua, CoreStatics.CoreApplication.Ambiente));
                if (oRisposta.Successo == true)
                {
                }
                else
                {
                    if (oRisposta.ex != null)
                    {
                        Exception rex = oRisposta.ex;
                        CoreStatics.ExGest(ref rex, @"Si è verificato un errore nell'elaborazione della procedura.", "ucDcViewer_ButtonEvent", this.Text, true, this.Name, "Errore", MessageBoxIcon.Warning, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);
                    }
                    else
                    {
                        easyStatics.EasyMessageBox(oRisposta.Parameters[0].ToString(), azione, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                this.Tag = null;


            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ucDcViewer_ButtonEvent", this.Text);
            }
            finally
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }

        }

        #endregion

        #region UltraPopupControlContainerSegnalibri

        private void UltraPopupControlContainerSegnalibri_Closed(object sender, EventArgs e)
        {
            _ucSegnalibri.SegnalibriClick -= UltraPopupControlContainerSegnalibri_ModificaClick;
        }

        private void UltraPopupControlContainerSegnalibri_Opened(object sender, EventArgs e)
        {
            _ucSegnalibri.SegnalibriClick += UltraPopupControlContainerSegnalibri_ModificaClick;
            _ucSegnalibri.Focus();
        }

        private void UltraPopupControlContainerSegnalibri_Opening(object sender, CancelEventArgs e)
        {
            UltraPopupControlContainer popup = sender as UltraPopupControlContainer;
            popup.PopupControl = _ucSegnalibri;
        }

        private void UltraPopupControlContainerSegnalibri_ModificaClick(object sender, SegnalibriClickEventArgs e)
        {

            try
            {

                switch (e.Pulsante)
                {

                    case EnumPulsanteSegnalibri.Modifica:
                        this.UltraPopupControlContainerSegnalibri.Close();
                        this.ucTopModale.Focus();
                        CoreStatics.CaricaPopup(EnumMaschere.Scheda, EnumEntita.SCH, e.ID);
                        break;

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        #endregion

    }
}
