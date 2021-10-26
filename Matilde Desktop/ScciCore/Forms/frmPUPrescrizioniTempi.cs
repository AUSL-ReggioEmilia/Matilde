using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;

using Microsoft.Data.SqlClient;
using System.Globalization;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.ScciResource;
using UnicodeSrl.ScciCore.CustomControls.Infra;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Statics;
using Infragistics.Win.UltraWinListView;
using Infragistics.Win.Misc;
using UnicodeSrl.Scci.PluginClient;
using UnicodeSrl.DatiClinici.DC;
using UnicodeSrl.DatiClinici.Gestore;

namespace UnicodeSrl.ScciCore
{
    public partial class frmPUPrescrizioniTempi : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmPUPrescrizioniTempi()
        {
            InitializeComponent();
        }

        #region Declare

        private bool _bruntime = false;
        private bool _baddtempo = false;

        private ucEasyListBox _ucEasyListBox = null;
        private ucEasyGrid _ucEasyGrid = null;
        private ucRichTextBox _ucRichTextBox = null;
        private ucDatiAggiuntiviPopUp _ucDatiAggiuntiviPopUp = null;
                private ucSegnalibri _ucSegnalibri = null;

        private bool _bFirmaDigitale = false;

        private Gestore oGestore = null;

        private bool _bMostraValidazione = true;

        #endregion

        #region Interface

        public void Carica()
        {
            try
            {
                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_PRESCRIZIONE_256);

                _bFirmaDigitale = DBUtils.ModuloUAAbilitato(CoreStatics.CoreApplication.Trasferimento.CodUA, EnumUAModuli.FirmaD_Prescrizioni);

                                if (this.CustomParamaters != null)
                    _bMostraValidazione = (bool)this.CustomParamaters;
                else
                    _bMostraValidazione = true;

                this.InizializzaControlli();
                if (CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodSchedaPosologia != string.Empty)
                {
                    this.InizializzaGestore();
                }

                this.Aggiorna();

                this.ShowDialog();
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "Carica", this.Text);
            }
        }

        #endregion

        #region Functions

        private void InizializzaControlli()
        {
            try
            {
                this._bruntime = true;

                this.ucAnteprimaRtfPosologia.rtbRichTextBox.ReadOnly = true;
                this.ucAnteprimaRtfPosologia.rtbRichTextBox.Click += ucAnteprimaRtfPosologia_Click;
                this.ucAnteprimaRtf.rtbRichTextBox.ReadOnly = true;
                this.ucAnteprimaRtf.rtbRichTextBox.Click += ucAnteprimaRtf_Click;

                this.LoadOptionSet();

                this.lblElencoSomministrazioni.Text = "Elenco Somministrazioni";

                if (_bFirmaDigitale)
                {
                    this.chkValidate.UNCheckedImage = Risorse.GetImageFromResource(Risorse.GC_TESSERAFIRMA_256);
                    this.chkValidate.CheckedImage = Risorse.GetImageFromResource(Risorse.GC_TESSERAFIRMAESEGUITA_256);
                }
                else
                {
                    this.chkValidate.UNCheckedImage = Risorse.GetImageFromResource(Risorse.GC_FIRMA_256);
                    this.chkValidate.CheckedImage = Properties.Resources.FirmaEseguita_256;
                }

                this.chkValidate.Visible = !CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.InMemoria && _bMostraValidazione;

                this.ubAddTempo.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_NUOVO_256);
                this.ubAddTempo.PercImageFill = 0.75F;

                                this.ucEasyCheckEditorDC.Appearance.ImageBackground = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_MODIFICA_256);
                this.ucEasyCheckEditorDC.CheckedAppearance.ImageBackground = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_MODIFICACHECK_256);
                if (CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodSchedaPosologia == string.Empty)
                {

                    this.ucEasyCheckEditorDC.Visible = false;
                    this.ucEasyTabControlPosologia.Tabs["tab1"].Visible = true;
                    this.ucEasyTabControlPosologia.ActiveTab = this.ucEasyTabControlPosologia.Tabs["tab1"];                   
                    this.ucEasyTabControlPosologia.Tabs["tab2"].Visible = false;

                }
                else
                {
                    this.ucEasyCheckEditorDC.Visible = true;
                    this.ucEasyTabControlPosologia.Tabs["tab2"].Visible = true;
                    this.ucEasyTabControlPosologia.ActiveTab = this.ucEasyTabControlPosologia.Tabs["tab2"];
                    this.ucEasyTabControlPosologia.Tabs["tab1"].Visible = false;
                }

                this._bruntime = false;
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "InizializzaControlli", this.Text);
            }
        }

        private void LoadOptionSet()
        {

            try
            {

                this.ucEasyOptionSet.Items.Clear();

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodTipoPrescrizione", CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CodTipoPrescrizione);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataTable dt = Database.GetDataTableStoredProc("MSP_SelTipoPrescrizioniTempi", spcoll);

                if (dt != null)
                {
                    foreach (DataRow oRow in dt.Rows)
                    {
                        Infragistics.Win.ValueListItem oVal = new Infragistics.Win.ValueListItem(oRow["CodTipoPrescrizioneTempi"].ToString(), oRow["DescTipoPrescrizioneTempi"].ToString());
                        this.ucEasyOptionSet.Items.Add(oVal);
                    }
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "LoadOptionSet", this.Text);
            }

        }

        private void SetOptionSet(string key)
        {

            switch ((EnumTipoPrescrizioneTempi)Enum.Parse(typeof(EnumTipoPrescrizioneTempi), key))
            {

                case EnumTipoPrescrizioneTempi.SN:                     this.ucEasyTabControlSelezioni.Tabs[key].Visible = true;
                    this.ucEasyTabControlSelezioni.ActiveTab = this.ucEasyTabControlSelezioni.Tabs[key];
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.RP.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.CN.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.PO.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.POC.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.PG.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.AB.ToString()].Visible = false;
                    break;

                case EnumTipoPrescrizioneTempi.RP:                      this.ucEasyTabControlSelezioni.Tabs[key].Visible = true;
                    this.ucEasyTabControlSelezioni.ActiveTab = this.ucEasyTabControlSelezioni.Tabs[key];
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.SN.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.CN.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.PO.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.POC.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.PG.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.AB.ToString()].Visible = false;
                    break;

                case EnumTipoPrescrizioneTempi.CN:                      this.ucEasyTabControlSelezioni.Tabs[key].Visible = true;
                    this.ucEasyTabControlSelezioni.ActiveTab = this.ucEasyTabControlSelezioni.Tabs[key];
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.SN.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.RP.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.PO.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.POC.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.PG.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.AB.ToString()].Visible = false;
                    break;

                case EnumTipoPrescrizioneTempi.PO:                      this.ucEasyTabControlSelezioni.Tabs[key].Visible = true;
                    this.ucEasyTabControlSelezioni.ActiveTab = this.ucEasyTabControlSelezioni.Tabs[key];
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.SN.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.RP.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.CN.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.POC.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.PG.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.AB.ToString()].Visible = false;
                    break;

                case EnumTipoPrescrizioneTempi.POC:                      this.ucEasyTabControlSelezioni.Tabs[key].Visible = true;
                    this.ucEasyTabControlSelezioni.ActiveTab = this.ucEasyTabControlSelezioni.Tabs[key];
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.SN.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.RP.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.CN.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.PO.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.PG.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.AB.ToString()].Visible = false;
                    break;

                case EnumTipoPrescrizioneTempi.PG:                      this.ucEasyTabControlSelezioni.Tabs[key].Visible = true;
                    this.ucEasyTabControlSelezioni.ActiveTab = this.ucEasyTabControlSelezioni.Tabs[key];
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.SN.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.RP.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.CN.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.PO.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.POC.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.AB.ToString()].Visible = false;
                    break;

                case EnumTipoPrescrizioneTempi.AB:                      this.ucEasyTabControlSelezioni.Tabs[key].Visible = true;
                    this.ucEasyTabControlSelezioni.ActiveTab = this.ucEasyTabControlSelezioni.Tabs[key];
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.SN.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.RP.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.CN.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.PO.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.POC.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoPrescrizioneTempi.PG.ToString()].Visible = false;
                    break;

            }

        }

        private void GetOptionSet()
        {

            foreach (Infragistics.Win.ValueListItem item in this.ucEasyOptionSet.Items)
            {

                if (item.DataValue.ToString() == CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodTipoPrescrizioneTempi)
                {
                    this.ucEasyOptionSet.CheckedItem = item;
                    break;
                }

            }
            if (this.ucEasyOptionSet.CheckedIndex == -1) { this.ucEasyOptionSet.CheckedIndex = 0; }

        }

        private void ResetOptionSet(string key)
        {

            this._bruntime = true;

            this.udteDataOraInizio_SN.Value = DateTime.Now;
            this.udteDataOraInizio_RP.Value = DateTime.Now;
            this.udteDataOraInizio_CN.Value = DateTime.Now;
            this.udteDataOraInizio_PO.Value = DateTime.Now;
            this.udteDataOraInizio_POC.Value = DateTime.Now;
            this.udteDataOraInizio_PG.Value = DateTime.Now;
            this.udteDataOraFine_RP.Value = DateTime.Now;
            this.udteDataOraFine_CN.Value = DateTime.Now;
            this.udteDataOraFine_PO.Value = DateTime.Now;
            this.udteDataOraFine_POC.Value = DateTime.Now;

            this.ucSelPeriodicitaGiorni_RP.Value = 0;
            this.ucSelPeriodicitaOre_RP.Value = 0;
            this.ucSelPeriodicitaMinuti_RP.Value = 0;

            this.ucSelPeriodicitaGiorni_CN.Value = 0;
            this.ucSelPeriodicitaOre_CN.Value = 0;
            this.ucSelPeriodicitaMinuti_CN.Value = 0;

            this.umeDurata_CN.Reset();

            this.uceProtocollo_PO.Text = string.Empty;
            this.uceProtocollo_POC.Text = string.Empty;
            this.uceProtocollo_PG.Text = string.Empty;

            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodProtocollo = string.Empty;
            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodTipoProtocollo = string.Empty;
            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodTipoPrescrizioneTempi = key;
            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Continuita = false;

            switch ((EnumTipoPrescrizioneTempi)Enum.Parse(typeof(EnumTipoPrescrizioneTempi), key))
            {

                case EnumTipoPrescrizioneTempi.SN:                     CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.AlBisogno = false;
                    break;

                case EnumTipoPrescrizioneTempi.RP:                      CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.AlBisogno = false;
                    break;

                case EnumTipoPrescrizioneTempi.CN:                      CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.AlBisogno = false;
                    CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Continuita = true;
                    break;

                case EnumTipoPrescrizioneTempi.PO:                      this.LoadProtocollo_PO();
                    CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.AlBisogno = false;
                    break;

                case EnumTipoPrescrizioneTempi.POC:                      this.LoadProtocollo_POC();
                    CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.AlBisogno = false;
                    break;

                case EnumTipoPrescrizioneTempi.PG:                      this.LoadProtocollo_PG();
                    CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.AlBisogno = false;
                    break;

                case EnumTipoPrescrizioneTempi.AB:                      CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.AlBisogno = true;
                    break;

            }

            this._bruntime = false;

        }

        public void Aggiorna()
        {
            try
            {
                if (CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Azione == EnumAzioni.INS)
                {

                    if (CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodSchedaPosologia != string.Empty)
                    {
                        this.SetDC(false);
                    }

                                                            if (this._bMostraValidazione)
                        this.chkValidate.Checked = true;
                    else
                        this.chkValidate.Checked = false;

                    this.ucEasyOptionSet.CheckedIndex = 0;

                    _bruntime = true;

                    if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata.PrescrizioneASchema == true)
                    {
                        this.ucEasyTextBoxPosologia.Text = "SCHEMA";
                        this.ucEasyTextBoxPosologia.Enabled = false;
                    }

                    if (CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraInizio > DateTime.MinValue)
                    {
                        this.udteDataOraInizio_SN.Value = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraInizio;
                        this.udteDataOraInizio_RP.Value = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraInizio;
                        this.udteDataOraInizio_CN.Value = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraInizio;
                        this.udteDataOraInizio_PO.Value = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraInizio;
                        this.udteDataOraInizio_POC.Value = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraInizio;
                        this.udteDataOraInizio_PG.Value = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraInizio;
                    }
                    if (CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraFine > DateTime.MinValue)
                    {
                        this.udteDataOraFine_RP.Value = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraFine;
                        this.udteDataOraFine_CN.Value = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraFine;
                        this.udteDataOraFine_PO.Value = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraFine;
                        this.udteDataOraFine_POC.Value = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraFine;
                    }

                    this.ucSelPeriodicitaGiorni_RP.Value = 0;
                    this.ucSelPeriodicitaOre_RP.Value = 0;
                    this.ucSelPeriodicitaMinuti_RP.Value = 0;

                    this.ucSelPeriodicitaGiorni_CN.Value = 0;
                    this.ucSelPeriodicitaOre_CN.Value = 0;
                    this.ucSelPeriodicitaMinuti_CN.Value = 0;

                    this.umeDurata_CN.Reset();

                    _bruntime = false;

                    this.MemorizzaValori();
                    this.CaricaSomministrazioni();

                }
                else
                {
                                        _bruntime = true;

                    if (CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodSchedaPosologia != string.Empty)
                    {
                        this.SetDC(false);
                    }

                    if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata.PrescrizioneASchema == true)
                    {
                        this.ucEasyTextBoxPosologia.Enabled = false;
                    }

                    this.GetOptionSet();

                    if (CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraInizio > DateTime.MinValue)
                    {
                        this.udteDataOraInizio_SN.Value = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraInizio;
                        this.udteDataOraInizio_RP.Value = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraInizio;
                        this.udteDataOraInizio_CN.Value = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraInizio;
                        this.udteDataOraInizio_PO.Value = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraInizio;
                        this.udteDataOraInizio_POC.Value = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraInizio;
                        this.udteDataOraInizio_PG.Value = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraInizio;
                    }
                    if (CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraFine > DateTime.MinValue)
                    {
                        this.udteDataOraFine_RP.Value = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraFine;
                        this.udteDataOraFine_CN.Value = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraFine;
                        this.udteDataOraFine_PO.Value = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraFine;
                        this.udteDataOraFine_POC.Value = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraFine;
                    }

                    this.ucSelPeriodicitaGiorni_RP.Value = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.PeriodicitaGiorni;
                    this.ucSelPeriodicitaOre_RP.Value = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.PeriodicitaOre;
                    this.ucSelPeriodicitaMinuti_RP.Value = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.PeriodicitaMinuti;

                    this.ucSelPeriodicitaGiorni_CN.Value = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.PeriodicitaGiorni;
                    this.ucSelPeriodicitaOre_CN.Value = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.PeriodicitaOre;
                    this.ucSelPeriodicitaMinuti_CN.Value = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.PeriodicitaMinuti;

                    if (CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DurataOrario != DateTime.MinValue)
                    {
                        this.umeDurata_CN.Reset();
                        this.umeDurata_CN.Ore = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DurataOrario.Hour;
                        this.umeDurata_CN.Minuti = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DurataOrario.Minute;
                    }
                    else
                    {
                        this.umeDurata_CN.Reset();
                    }

                    if (this.chkValidate.Visible)
                    {
                        this.chkValidate.Checked = (CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodStatoPrescrizioneTempi == EnumStatoPrescrizioneTempi.VA.ToString());
                        if (this.chkValidate.Checked) this.chkValidate.Enabled = false;

                    }
                    else
                    {
                        this.chkValidate.Checked = false;
                    }

                    this.ucEasyTextBoxPosologia.Text = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Posologia;
                    this.chkManuale.Checked = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Manuale;
                    if (this.chkManuale.Checked == true)
                    {
                        this.CaricaSomministrazioni();
                    }

                    this.LoadProtocollo_PO();
                    this.LoadProtocollo_POC();
                    this.LoadProtocollo_PG();

                    this.uceProtocollo_PO.Value = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodProtocollo;
                    this.uceProtocollo_POC.Value = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodProtocollo;
                    this.uceProtocollo_PG.Value = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodProtocollo;

                    _bruntime = false;
                }

                this.SetManuale();

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "Aggiorna", this.Text);
            }

        }

        private void CaricaSomministrazioni()
        {

            if (CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Manuale == false)
            {
                List<IntervalloTempi> tempi = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.GeneraPeriodicita();
                this.lblElencoSomministrazioni.Text = "Elenco Somministrazioni (" + tempi.Count + ")";
                if (tempi.Count > 0)
                    this.ucEasyGrid.DataSource = tempi;
                else
                    this.ucEasyGrid.DataSource = null;
                this.ucEasyGrid.Refresh();
            }
            else
            {
                DataSet oDs = this.GetListaIntervalloTempiDS();
                oDs.Tables[0].DefaultView.Sort = "DataOraInizio";
                this.ucEasyGridManuale.DisplayLayout.Bands[0].Columns.ClearUnbound();
                this.ucEasyGridManuale.DataSource = oDs;
                this.ucEasyGridManuale.Refresh();
                this.ucEasyGridManuale.DisplayLayout.Bands[0].SortedColumns.Clear();
                this.ucEasyGridManuale.DisplayLayout.Bands[0].SortedColumns.Add("DataOraInizio", false);

            }

        }

        public bool Salva()
        {
            bool bReturn = false;
            frmSmartCardProgress frmSC = null;
            try
            {

                this.Cursor = CoreStatics.setCursor(enum_app_cursors.WaitCursor);

                if (ControllaValori())
                {
                    bool bValida = false;
                    if (this.chkValidate.Checked && this.chkValidate.Enabled) bValida = true;

                    this.MemorizzaValori();

                    if (CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.InMemoria == false)
                    {

                        bReturn = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Salva();

                                                if (bReturn && bValida)
                        {
                            bool bContinua = true;

                            if (_bFirmaDigitale)
                            {
                                bContinua = false;
                                try
                                {

                                                                        setNavigazione(false);
                                    frmSC = new frmSmartCardProgress();
                                    frmSC.InizializzaEMostra(0, 4, this);
                                                                        frmSC.SetCursore(enum_app_cursors.WaitCursor);

                                    frmSC.SetStato(@"Validazione Prescrizione");


                                                                        frmSC.SetStato(@"Generazione Documento...");

                                                                        byte[] pdfContent = CoreStatics.GeneraPDFPrescrizioneTempi(CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.IDPrescrizioneTempi, EnumStatoPrescrizioneTempi.VA, true);

                                    if (pdfContent == null || pdfContent.Length <= 0)
                                    {
                                        frmSC.SetLog(@"Errore Generazione documento", true);

                                    }
                                    else
                                    {
                                        bContinua = frmSC.ProvaAFirmare(ref pdfContent, EnumTipoDocumentoFirmato.PRTFM01, "Firma Digitale...", EnumEntita.PRT, CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.IDPrescrizioneTempi);
                                    }


                                }
                                catch (Exception ex)
                                {
                                    frmSC.SetLog(@"Errore Generazione documento: " + ex.Message, true);
                                    bContinua = false;
                                }
                            }


                            if (bContinua)
                            {
                                                                CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodStatoPrescrizioneTempi = EnumStatoPrescrizioneTempi.VA.ToString();                                 CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Azione = EnumAzioni.VAL;

                                if (CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Salva())
                                    CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CreaTaskInfermieristici(EnumCodSistema.PRF, Database.GetConfigTable(EnumConfigTable.TipoSchedaTaskDaPrescrizione), EnumTipoRegistrazione.A);


                                Risposta oRispostaValidaDopo = new Risposta();
                                oRispostaValidaDopo.Successo = true;
                                oRispostaValidaDopo = PluginClientStatics.PluginClient(EnumPluginClient.PRT_VALIDA_DOPO_PU.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.Trasferimento.CodUA, CoreStatics.CoreApplication.Ambiente));

                                

                            }

                        }

                    }
                    else
                    {
                        bReturn = true;
                    }

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Salva", this.Name);
            }
            finally
            {
                if (frmSC != null)
                {
                    frmSC.Close();
                    frmSC.Dispose();
                }

                                setNavigazione(true);

                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }

            return bReturn;
        }

        private void MemorizzaValori()
        {

            DateTime dtI = DateTime.MinValue;
            DateTime dtF = DateTime.MinValue;

            switch ((EnumTipoPrescrizioneTempi)Enum.Parse(typeof(EnumTipoPrescrizioneTempi), this.ucEasyOptionSet.CheckedItem.DataValue.ToString()))
            {

                case EnumTipoPrescrizioneTempi.SN:                      if (this.udteDataOraInizio_SN.Value != null)
                    {
                        dtI = (DateTime)this.udteDataOraInizio_SN.Value;
                        dtF = (DateTime)this.udteDataOraInizio_SN.Value;
                    }
                    break;

                case EnumTipoPrescrizioneTempi.RP:                      if (this.udteDataOraInizio_RP.Value != null && this.udteDataOraFine_RP.Value != null &&
                        this.ucSelPeriodicitaGiorni_RP.Value != null && this.ucSelPeriodicitaOre_RP.Value != null && this.ucSelPeriodicitaMinuti_RP.Value != null)
                    {
                        dtI = (DateTime)this.udteDataOraInizio_RP.Value;
                        dtF = this.udteDataOraFine_RP.Value != null ? (DateTime)this.udteDataOraFine_RP.Value : DateTime.MinValue;
                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.PeriodicitaGiorni = (int)this.ucSelPeriodicitaGiorni_RP.Value;
                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.PeriodicitaOre = (int)this.ucSelPeriodicitaOre_RP.Value;
                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.PeriodicitaMinuti = (int)this.ucSelPeriodicitaMinuti_RP.Value;
                    }
                    break;

                case EnumTipoPrescrizioneTempi.CN:                      if (this.udteDataOraInizio_CN.Value != null && this.udteDataOraFine_CN.Value != null &&
                        this.ucSelPeriodicitaGiorni_CN.Value != null && this.ucSelPeriodicitaOre_CN.Value != null && this.ucSelPeriodicitaMinuti_CN.Value != null &&
                        this.umeDurata_CN.Value != null)
                    {
                        dtI = (DateTime)this.udteDataOraInizio_CN.Value;
                        dtF = this.udteDataOraFine_CN.Value != null ? (DateTime)this.udteDataOraFine_CN.Value : DateTime.MinValue;
                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.PeriodicitaGiorni = (int)this.ucSelPeriodicitaGiorni_CN.Value;
                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.PeriodicitaOre = (int)this.ucSelPeriodicitaOre_CN.Value;
                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.PeriodicitaMinuti = (int)this.ucSelPeriodicitaMinuti_CN.Value;
                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Durata = (this.umeDurata_CN.Giorni * 24 * 60) + (this.umeDurata_CN.Ore * 60) + this.umeDurata_CN.Minuti;
                    }
                    break;

                case EnumTipoPrescrizioneTempi.PO:                      if (this.udteDataOraInizio_PO.Value != null && this.udteDataOraFine_PO.Value != null &&
                        this.uceProtocollo_PO.SelectedItem != null)
                    {
                        dtI = (DateTime)this.udteDataOraInizio_PO.Value;
                        dtF = this.udteDataOraFine_PO.Value != null ? (DateTime)this.udteDataOraFine_PO.Value : DateTime.MinValue;
                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodProtocollo = this.uceProtocollo_PO.SelectedItem != null ? this.uceProtocollo_PO.SelectedItem.DataValue.ToString() : string.Empty;
                    }
                    break;

                case EnumTipoPrescrizioneTempi.POC:                      if (this.udteDataOraInizio_POC.Value != null && this.udteDataOraFine_POC.Value != null &&
                        this.uceProtocollo_POC.SelectedItem != null)
                    {
                        dtI = (DateTime)this.udteDataOraInizio_POC.Value;
                        dtF = this.udteDataOraFine_POC.Value != null ? (DateTime)this.udteDataOraFine_POC.Value : DateTime.MinValue;
                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodProtocollo = this.uceProtocollo_POC.SelectedItem != null ? this.uceProtocollo_POC.SelectedItem.DataValue.ToString() : string.Empty;
                    }
                    break;

                case EnumTipoPrescrizioneTempi.PG:                      if (this.udteDataOraInizio_PG.Value != null &&
                        this.uceProtocollo_PG.SelectedItem != null)
                    {
                        dtI = (DateTime)this.udteDataOraInizio_PG.Value;
                        dtF = (DateTime)this.udteDataOraInizio_PG.Value;
                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodProtocollo = this.uceProtocollo_PG.SelectedItem != null ? this.uceProtocollo_PG.SelectedItem.DataValue.ToString() : string.Empty;
                    }
                    break;

                case EnumTipoPrescrizioneTempi.AB:                      break;

            }

            if (CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodSchedaPosologia != string.Empty)
            {
                CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.MovScheda.DatiXML = this.ucDcViewer.Gestore.SchedaDatiXML;
                CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Posologia = CoreStatics.CoreApplication.MovPrescrizioneSelezionata.MovScheda.AnteprimaTXT;
            }
            else
            {
                CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Posologia = this.ucEasyTextBoxPosologia.Text;
            }

            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraInizio = new DateTime(dtI.Year, dtI.Month, dtI.Day, dtI.Hour, dtI.Minute, 0);
            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraFine =
                            this.udteDataOraFine_RP.Value != null ? new DateTime(dtF.Year, dtF.Month, dtF.Day, dtF.Hour, dtF.Minute, 0) : DateTime.MinValue;

            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Manuale = this.chkManuale.Checked;
            if (CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Manuale == true && this.ucEasyGridManuale.DataSource != null)
            {
                CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.TempiManuali = ((DataSet)this.ucEasyGridManuale.DataSource).GetXml();
            }
            else
            {
                CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.TempiManuali = string.Empty;
            }

        }

        private bool ControllaValori()
        {

            bool bOK = true;

            switch ((EnumTipoPrescrizioneTempi)Enum.Parse(typeof(EnumTipoPrescrizioneTempi), this.ucEasyOptionSet.CheckedItem.DataValue.ToString()))
            {

                case EnumTipoPrescrizioneTempi.SN:
                                        if (bOK && this.udteDataOraInizio_SN.Value == null)
                    {
                        easyStatics.EasyMessageBox("E' necessario definire Data/Ora Inizio.", "Tempi Prescrizione", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.udteDataOraInizio_SN.Focus();
                        bOK = false;
                    }
                    break;

                case EnumTipoPrescrizioneTempi.RP:
                                        if (bOK && this.udteDataOraInizio_RP.Value == null)
                    {
                        easyStatics.EasyMessageBox("E' necessario definire Data/Ora Inizio.", "Tempi Prescrizione", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.udteDataOraInizio_RP.Focus();
                        bOK = false;
                    }
                    if (bOK && this.udteDataOraFine_RP.Value == null)
                    {
                        easyStatics.EasyMessageBox("E' necessario definire Data/Ora Fine.", "Tempi Prescrizione", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.udteDataOraFine_RP.Focus();
                        bOK = false;
                    }
                    if (bOK && (DateTime)this.udteDataOraFine_RP.Value <= (DateTime)this.udteDataOraInizio_RP.Value)
                    {
                        easyStatics.EasyMessageBox("Date non coerenti: Data/Ora Inizio maggiore o uguale di Data/Ora Fine.", "Tempi Prescrizione", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.udteDataOraFine_RP.Focus();
                        bOK = false;
                    }
                    if (bOK && this.ucSelPeriodicitaGiorni_RP.Value != null && (int)this.ucSelPeriodicitaGiorni_RP.Value == 0
                            && this.ucSelPeriodicitaOre_RP.Value != null && (int)this.ucSelPeriodicitaOre_RP.Value == 0
                            && this.ucSelPeriodicitaMinuti_RP.Value != null && (int)this.ucSelPeriodicitaMinuti_RP.Value == 0
                        )
                    {
                        easyStatics.EasyMessageBox("E' necessario definire la periodicità.", "Tempi Prescrizione", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.ucSelPeriodicitaGiorni_RP.Focus();
                        bOK = false;
                    }

                    break;

                case EnumTipoPrescrizioneTempi.CN:
                                        if (bOK && this.udteDataOraInizio_CN.Value == null)
                    {
                        easyStatics.EasyMessageBox("E' necessario definire Data/Ora Inizio.", "Tempi Prescrizione", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.udteDataOraInizio_CN.Focus();
                        bOK = false;
                    }
                    if (bOK && (this.umeDurata_CN.Value == null || (this.umeDurata_CN.Giorni == 0 && this.umeDurata_CN.Ore == 0 && this.umeDurata_CN.Minuti == 0)))
                    {
                        easyStatics.EasyMessageBox("E' necessario definire la Durata.", "Tempi Prescrizione", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.udteDataOraInizio_CN.Focus();
                        bOK = false;
                    }
                    break;

                case EnumTipoPrescrizioneTempi.PO:
                                        if (bOK && this.udteDataOraInizio_PO.Value == null)
                    {
                        easyStatics.EasyMessageBox("E' necessario definire Data/Ora Inizio.", "Tempi Prescrizione", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.udteDataOraInizio_PO.Focus();
                        bOK = false;
                    }
                    if (bOK && this.udteDataOraFine_PO.Value == null)
                    {
                        easyStatics.EasyMessageBox("E' necessario definire Data/Ora Fine.", "Tempi Prescrizione", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.udteDataOraFine_PO.Focus();
                        bOK = false;
                    }
                    if (bOK && (DateTime)this.udteDataOraFine_PO.Value <= (DateTime)this.udteDataOraInizio_PO.Value)
                    {
                        easyStatics.EasyMessageBox("Date non coerenti: Data/Ora Inizio maggiore di Data/Ora Fine.", "Tempi Prescrizione", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.udteDataOraFine_PO.Focus();
                        bOK = false;
                    }
                    if (this.uceProtocollo_PO.Value == null || this.uceProtocollo_PO.Value.ToString() == string.Empty)
                    {
                        easyStatics.EasyMessageBox("E' necessario definire il protocollo.", "Tempi Prescrizione", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uceProtocollo_PO.Focus();
                        bOK = false;
                    }
                    break;

                case EnumTipoPrescrizioneTempi.POC:
                                        if (bOK && this.udteDataOraInizio_POC.Value == null)
                    {
                        easyStatics.EasyMessageBox("E' necessario definire Data/Ora Inizio!", "Tempi Prescrizione", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.udteDataOraInizio_POC.Focus();
                        bOK = false;
                    }
                    if (bOK && this.udteDataOraFine_POC.Value == null)
                    {
                        easyStatics.EasyMessageBox("E' necessario definire  Data/Ora Fine!", "Tempi Prescrizione", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.udteDataOraFine_POC.Focus();
                        bOK = false;
                    }
                    if (bOK && (DateTime)this.udteDataOraFine_POC.Value <= (DateTime)this.udteDataOraInizio_POC.Value)
                    {
                        easyStatics.EasyMessageBox("Date non coerenti: Data/Ora Inizio maggiore o uguale di Data/Ora Fine.", "Tempi Prescrizione", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.udteDataOraFine_POC.Focus();
                        bOK = false;
                    }
                    if (this.uceProtocollo_POC.Value == null || this.uceProtocollo_POC.Value.ToString() == string.Empty)
                    {
                        easyStatics.EasyMessageBox("E' necessario definire il protocollo.", "Tempi Prescrizione", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uceProtocollo_POC.Focus();
                        bOK = false;
                    }
                    break;

                case EnumTipoPrescrizioneTempi.PG:
                                        if (bOK && this.udteDataOraInizio_PG.Value == null)
                    {
                        easyStatics.EasyMessageBox("E' necessario definire Data/Ora Inizio.", "Tempi Prescrizione", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.udteDataOraInizio_PG.Focus();
                        bOK = false;
                    }
                    if (this.uceProtocollo_PG.Value == null || this.uceProtocollo_PG.Value.ToString() == string.Empty)
                    {
                        easyStatics.EasyMessageBox("E' necessario definire il protocollo.", "Tempi Prescrizione", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uceProtocollo_PG.Focus();
                        bOK = false;
                    }
                    break;

                case EnumTipoPrescrizioneTempi.AB:
                    break;

            }

            return bOK;
        }

        private void LoadProtocollo_PO()
        {
            try
            {
                
                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodTipoPrescrizione", CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CodTipoPrescrizione);
                op.Parametro.Add("Continuita", "0");

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataTable dt = Database.GetDataTableStoredProc("MSP_SelProtocolli", spcoll);

                if (dt != null)
                {

                    this.uceProtocollo_PO.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
                    this.uceProtocollo_PO.ValueMember = "Codice";
                    this.uceProtocollo_PO.DisplayMember = "Descrizione";
                    this.uceProtocollo_PO.DataSource = dt;
                    this.uceProtocollo_PO.Refresh();

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "LoadProtocollo_PO", this.Text);
            }
        }
        private void LoadProtocollo_POC()
        {
            try
            {
                
                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodTipoPrescrizione", CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CodTipoPrescrizione);
                op.Parametro.Add("Continuita", "1");

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataTable dt = Database.GetDataTableStoredProc("MSP_SelProtocolli", spcoll);

                if (dt != null)
                {

                    this.uceProtocollo_POC.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
                    this.uceProtocollo_POC.ValueMember = "Codice";
                    this.uceProtocollo_POC.DisplayMember = "Descrizione";
                    this.uceProtocollo_POC.DataSource = dt;
                    this.uceProtocollo_POC.Refresh();

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "LoadProtocollo_POC", this.Text);
            }
        }
        private void LoadProtocollo_PG()
        {
            try
            {
                
                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodTipoPrescrizione", CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CodTipoPrescrizione);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataTable dt = Database.GetDataTableStoredProc("MSP_SelProtocolli", spcoll);

                if (dt != null)
                {

                    this.uceProtocollo_PG.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
                    this.uceProtocollo_PG.ValueMember = "Codice";
                    this.uceProtocollo_PG.DisplayMember = "Descrizione";
                    this.uceProtocollo_PG.DataSource = dt;
                    this.uceProtocollo_PG.Refresh();

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "LoadProtocollo_PG", this.Text);
            }
        }

        private bool GetAttivazioneContinuita_PO()
        {
            bool bret = false;

            try
            {
                DataTable dtcombo = (DataTable)uceProtocollo_PO.DataSource;
                if (dtcombo != null)
                {
                    DataRow[] dtfilter = dtcombo.Select("Codice = '" + this.uceProtocollo_PO.SelectedItem.DataValue.ToString() + "'");
                    if (dtfilter.Length > 0)
                    {
                                                bret = bool.Parse(dtfilter[0]["Continuita"].ToString());
                    }
                    else
                        bret = false;
                }
                else
                    bret = false;
            }
            catch
            {
                bret = false;
            }

            return bret;
        }
        private string GetTipoProtocollo_PO()
        {

            string sret = string.Empty;

            try
            {

                DataTable dtcombo = (DataTable)uceProtocollo_PO.DataSource;
                if (dtcombo != null)
                {
                    DataRow[] dtfilter = dtcombo.Select("Codice = '" + this.uceProtocollo_PO.SelectedItem.DataValue.ToString() + "'");
                    if (dtfilter.Length > 0)
                    {
                        sret = dtfilter[0]["CodTipoProtocollo"].ToString();
                    }
                }

            }
            catch
            {

            }

            return sret;

        }

        private bool GetAttivazioneContinuita_POC()
        {
            bool bret = false;

            try
            {
                DataTable dtcombo = (DataTable)uceProtocollo_POC.DataSource;
                if (dtcombo != null)
                {
                    DataRow[] dtfilter = dtcombo.Select("Codice = '" + this.uceProtocollo_POC.SelectedItem.DataValue.ToString() + "'");
                    if (dtfilter.Length > 0)
                    {
                                                bret = bool.Parse(dtfilter[0]["Continuita"].ToString());
                    }
                    else
                        bret = false;
                }
                else
                    bret = false;
            }
            catch
            {
                bret = false;
            }

            return bret;
        }
        private int GetValoreDurata_POC()
        {
            int nret = 0;

            try
            {
                DataTable dtcombo = (DataTable)uceProtocollo_POC.DataSource;
                if (dtcombo != null)
                {
                    DataRow[] dtfilter = dtcombo.Select("Codice = '" + this.uceProtocollo_POC.SelectedItem.DataValue.ToString() + "'");
                    if (dtfilter.Length > 0)
                    {
                        nret = int.Parse(dtfilter[0]["Durata"].ToString());
                    }
                    else
                        nret = 0;
                }
                else
                    nret = 0;
            }
            catch
            {
                nret = 0;
            }

            return nret;
        }
        private string GetTipoProtocollo_POC()
        {

            string sret = string.Empty;

            try
            {

                DataTable dtcombo = (DataTable)uceProtocollo_POC.DataSource;
                if (dtcombo != null)
                {
                    DataRow[] dtfilter = dtcombo.Select("Codice = '" + this.uceProtocollo_POC.SelectedItem.DataValue.ToString() + "'");
                    if (dtfilter.Length > 0)
                    {
                        sret = dtfilter[0]["CodTipoProtocollo"].ToString();
                    }
                }

            }
            catch
            {

            }

            return sret;

        }

        private bool GetAttivazioneContinuita_PG()
        {
            bool bret = false;

            try
            {
                DataTable dtcombo = (DataTable)uceProtocollo_PG.DataSource;
                if (dtcombo != null)
                {
                    DataRow[] dtfilter = dtcombo.Select("Codice = '" + this.uceProtocollo_PG.SelectedItem.DataValue.ToString() + "'");
                    if (dtfilter.Length > 0)
                    {
                                                bret = bool.Parse(dtfilter[0]["Continuita"].ToString());
                    }
                    else
                        bret = false;
                }
                else
                    bret = false;
            }
            catch
            {
                bret = false;
            }

            return bret;
        }
        private string GetTipoProtocollo_PG()
        {

            string sret = string.Empty;

            try
            {

                DataTable dtcombo = (DataTable)uceProtocollo_PG.DataSource;
                if (dtcombo != null)
                {
                    DataRow[] dtfilter = dtcombo.Select("Codice = '" + this.uceProtocollo_PG.SelectedItem.DataValue.ToString() + "'");
                    if (dtfilter.Length > 0)
                    {
                        sret = dtfilter[0]["CodTipoProtocollo"].ToString();
                    }
                }

            }
            catch
            {

            }

            return sret;

        }

        private void CalcolaDurata()
        {

            try
            {

                int nGiorni = (int)this.ucSelPeriodicitaGiorni_CN.Value;
                int nOre = (int)this.ucSelPeriodicitaOre_CN.Value;
                int nMinuti = (int)this.ucSelPeriodicitaMinuti_CN.Value;

                TimeSpan timespan = ((DateTime)this.udteDataOraFine_CN.Value - (DateTime)this.udteDataOraInizio_CN.Value);

                if (nGiorni == 0 && nOre == 0 && nMinuti == 0)
                {
                    this.umeDurata_CN.Giorni = (timespan.Days < 0 ? 0 : timespan.Days);
                    this.umeDurata_CN.Ore = (timespan.Hours < 0 ? 0 : timespan.Hours);
                    this.umeDurata_CN.Minuti = (timespan.Minutes < 0 ? 0 : timespan.Minutes);
                }
                else
                {
                    if (this.umeDurata_CN.Giorni == timespan.Days && this.umeDurata_CN.Ore == timespan.Hours && this.umeDurata_CN.Minuti == timespan.Minutes)
                        this.umeDurata_CN.Reset();
                }

            }
            catch (Exception)
            {

            }

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

        private ucEasyGrid GetEasyGridForPopupControlContainer(string CodTipoProtocollo)
        {

            ucEasyGrid _ucEasyGrid = new ucEasyGrid();

            try
            {

                _ucEasyGrid.Size = new Size(500, 400);
                _ucEasyGrid.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Medium;
                
                _ucEasyGrid.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
                _ucEasyGrid.DisplayLayout.Override.FilterUIType = FilterUIType.FilterRow;
                _ucEasyGrid.DisplayLayout.Override.FilterClearButtonLocation = FilterClearButtonLocation.Row;

                _ucEasyGrid.DisplayLayout.Override.FilterRowAppearance.BackColor = Color.Yellow;
                                                                                                
                _ucEasyGrid.DisplayLayout.Override.FilterComparisonType = FilterComparisonType.CaseInsensitive;
                _ucEasyGrid.DisplayLayout.Override.FilterOperatorDefaultValue = FilterOperatorDefaultValue.Contains;
                _ucEasyGrid.DisplayLayout.Override.FilterOperatorLocation = FilterOperatorLocation.Hidden;
                _ucEasyGrid.DisplayLayout.Override.FilterOperandStyle = FilterOperandStyle.Edit;
                _ucEasyGrid.DisplayLayout.Override.FilterRowPrompt = "";
                                _ucEasyGrid.DisplayLayout.Override.SpecialRowSeparator = SpecialRowSeparator.FilterRow;

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodTipoPrescrizione", CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CodTipoPrescrizione);
                op.Parametro.Add("CodTipoProtocollo", CodTipoProtocollo);

                switch ((EnumTipoPrescrizioneTempi)Enum.Parse(typeof(EnumTipoPrescrizioneTempi), this.ucEasyOptionSet.CheckedItem.DataValue.ToString()))
                {

                    case EnumTipoPrescrizioneTempi.SN:                          break;

                    case EnumTipoPrescrizioneTempi.RP:                          break;

                    case EnumTipoPrescrizioneTempi.CN:                          break;

                    case EnumTipoPrescrizioneTempi.PO:                          op.Parametro.Add("Continuita", "0");
                        break;

                    case EnumTipoPrescrizioneTempi.POC:                          op.Parametro.Add("Continuita", "1");
                        break;

                    case EnumTipoPrescrizioneTempi.PG:                          break;

                    case EnumTipoPrescrizioneTempi.AB:                          break;

                }

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataTable dt = Database.GetDataTableStoredProc("MSP_SelProtocolli", spcoll);

                _ucEasyGrid.DataSource = dt;
                _ucEasyGrid.Refresh();

            }
            catch (Exception)
            {

            }

            return _ucEasyGrid;

        }

        private void SetManuale()
        {

            try
            {

                if (CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Manuale == false)
                {
                    this.ucEasyTabControlGrid.Tabs[0].Visible = true;
                    this.ucEasyTabControlGrid.ActiveTab = this.ucEasyTabControlGrid.Tabs[0];
                    this.ucEasyTabControlGrid.Tabs[1].Visible = false;
                    this.ucEasyOptionSet.Enabled = true;
                    this.ucEasyTabControlSelezioni.Enabled = true;
                    uceProtocollo_ValueChanged(this.uceProtocollo_PO, new System.EventArgs());
                    uceProtocollo_ValueChanged(this.uceProtocollo_POC, new System.EventArgs());
                    uceProtocollo_ValueChanged(this.uceProtocollo_PG, new System.EventArgs());
                }
                else
                {
                    this.ucEasyTabControlGrid.Tabs[1].Visible = true;
                    this.ucEasyTabControlGrid.ActiveTab = this.ucEasyTabControlGrid.Tabs[1];
                    this.ucEasyTabControlGrid.Tabs[0].Visible = false;
                    this.ucEasyOptionSet.Enabled = false;
                    this.ucEasyTabControlSelezioni.Enabled = false;
                }

            }
            catch (Exception)
            {

            }

        }

        private DataSet GetListaIntervalloTempiDS()
        {

            DataSet oDs = new DataSet();
            DataTable oDt = new DataTable();
            DataColumn oDc = null;
                        oDc = new DataColumn("DataOraInizio", typeof(DateTime));
            oDt.Columns.Add(oDc);
            oDc = new DataColumn("DataOraFine", typeof(DateTime));
            oDt.Columns.Add(oDc);
            oDc = new DataColumn("NomeProtocollo", typeof(string));
            oDt.Columns.Add(oDc);
            oDc = new DataColumn("EtichettaTempo", typeof(string));
            oDt.Columns.Add(oDc);
                        oDs.Tables.Add(oDt);

            if (this.ucEasyGrid.DataSource != null)
            {
                List<IntervalloTempi> olist = (List<IntervalloTempi>)this.ucEasyGrid.DataSource;
                foreach (IntervalloTempi inttempo in olist)
                {
                    DataRow oDr = oDs.Tables[0].NewRow();
                    oDr["DataOraInizio"] = inttempo.DataOraInizio;
                    oDr["DataOraFine"] = inttempo.DataOraFine;
                    oDr["NomeProtocollo"] = inttempo.NomeProtocollo;
                    oDr["EtichettaTempo"] = inttempo.EtichettaTempo;
                    oDs.Tables[0].Rows.Add(oDr);
                }
            }
            else if (CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.TempiManuali != string.Empty)
            {
                System.IO.StringReader xmlSR = new System.IO.StringReader(CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.TempiManuali);
                oDs.ReadXml(xmlSR, XmlReadMode.IgnoreSchema);
            }

            return oDs;

        }

        private ucDatiAggiuntiviPopUp GetDatiAggiuntiviPopUpForPopupControlContainer()
        {

            ucDatiAggiuntiviPopUp _ucDatiAggiuntiviPopUp = new ucDatiAggiuntiviPopUp();

            try
            {

                _ucDatiAggiuntiviPopUp.Descrizione = "Inserire Tempi";
                _ucDatiAggiuntiviPopUp.TipoDato = EnumTipoDatoAggiuntivo.Tempi;

            }
            catch (Exception)
            {

            }

            return _ucDatiAggiuntiviPopUp;

        }

        private void setNavigazione(bool enable)
        {
            try
            {
                CoreStatics.SetNavigazione(enable);

                this.ucBottomModale.Enabled = enable;
                this.ucEasyTableLayoutPanelPosologia.Enabled = enable;
                this.ucEasyTableLayoutPanelSelezioni.Enabled = enable;
                this.ucEasyTableLayoutPanelGrid.Enabled = enable;

            }
            catch
            {
                CoreStatics.SetNavigazione(true);
                this.ucBottomModale.Enabled = true;
            }
        }

        #endregion

        #region Scheda

        private void InizializzaGestore()
        {

            try
            {

                                                                oGestore = CoreStatics.GetGestore();

                this.CaricaGestore();

                                this.ucDcViewer.VisualizzaTitoloScheda = false;

                this.ucDcViewer.CaricaDati(oGestore);

                this.ucDcViewer.KeyEvent -= ucDcViewer_KeyEvent;
                this.ucDcViewer.KeyEvent += ucDcViewer_KeyEvent;


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
                                oGestore.SchedaXML = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.MovScheda.Scheda.StrutturaXML;
                oGestore.SchedaLayoutsXML = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.MovScheda.Scheda.LayoutXML;
                                oGestore.Decodifiche = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.MovScheda.Scheda.DizionarioValori();

                                if (CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.MovScheda.DatiXML == string.Empty)
                {
                    oGestore.SchedaDati = new DcSchedaDati();
                }
                else
                {
                    try
                    {
                        oGestore.SchedaDatiXML = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.MovScheda.DatiXML;
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

        private void SetDC(bool bDc)
        {

            this.ucEasyTableLayoutPanelDC.Visible = false;

            switch (bDc)
            {

                case false:
                    this.ucEasyTableLayoutPanelDC.ColumnStyles[0].Width = 100;
                    this.ucEasyTableLayoutPanelDC.ColumnStyles[1].Width = 0;
                    CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.MovScheda.DatiXML = this.ucDcViewer.Gestore.SchedaDatiXML;
                    this.CaricaRtf();
                    break;

                case true:
                    this.ucEasyTableLayoutPanelDC.ColumnStyles[0].Width = 0;
                    this.ucEasyTableLayoutPanelDC.ColumnStyles[1].Width = 100;
                    this.ucDcViewer.CaricaDati();
                    break;
            }

            this.ucEasyTableLayoutPanelDC.Visible = true;

        }

        private void CaricaRtf()
        {

            try
            {

                this.ucAnteprimaRtfPosologia.MovScheda = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.MovScheda;
                this.ucAnteprimaRtfPosologia.MovScheda.GeneraRTF();
                this.ucAnteprimaRtfPosologia.RefreshRTF();

            }
            catch (Exception ex)
            {
                throw new Exception(@"CaricaRtf()" + Environment.NewLine + ex.Message, ex);
            }

        }

        private void CaricaScheda()
        {

            try
            {

                this.ucDcViewer.CaricaDati(oGestore);

            }
            catch (Exception ex)
            {
                throw new Exception(@"CaricaScheda()" + Environment.NewLine + ex.Message, ex);
            }

        }

        #endregion

        #region Events Form

        private void frmPUPrescrizioniTempi_ImmagineClick(object sender, ImmagineTopClickEventArgs e)
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

        private void frmPUPrescrizioniTempi_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            try
            {

                if (Salva())
                {
                    this.ucAnteprimaRtfPosologia.rtbRichTextBox.Click -= ucAnteprimaRtfPosologia_Click;
                    this.ucAnteprimaRtf.rtbRichTextBox.Click -= ucAnteprimaRtf_Click;
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "frmPUTaskInfermieristici_PulsanteAvantiClick", this.Text);
            }
        }

        private void frmPUPrescrizioniTempi_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.ucAnteprimaRtfPosologia.rtbRichTextBox.Click -= ucAnteprimaRtfPosologia_Click;
            this.ucAnteprimaRtf.rtbRichTextBox.Click -= ucAnteprimaRtf_Click;
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void frmPUPrescrizioniTempi_Shown(object sender, EventArgs e)
        {

            if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata != null)
            {
                this.ucAnteprimaRtf.MovScheda = CoreStatics.CoreApplication.MovPrescrizioneSelezionata.MovScheda;
                this.ucAnteprimaRtf.RefreshRTF();
            }

            if (CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodSchedaPosologia != string.Empty)
            {
                this.CaricaRtf();
                this.CaricaScheda();
            }

        }

        #endregion

        #region Events

        private void ucAnteprimaRtf_Click(object sender, EventArgs e)
        {

            CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerRtf);
            _ucRichTextBox = CoreStatics.GetRichTextBoxForPopupControlContainer(this.ucAnteprimaRtf.MovScheda.AnteprimaRTF);
            this.UltraPopupControlContainerRtf.Show((RichTextBox)sender);
        }

        private void ucAnteprimaRtfPosologia_Click(object sender, EventArgs e)
        {

            CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerRtf);
            _ucRichTextBox = CoreStatics.GetRichTextBoxForPopupControlContainer(this.ucAnteprimaRtfPosologia.MovScheda.AnteprimaRTF);
            this.UltraPopupControlContainerRtf.Show((RichTextBox)sender);
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
                        oCol.Hidden = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Durata == 0;
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
                        oCol.Hidden = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodProtocollo == string.Empty;
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

                                
                
        
        
        private void chkManuale_CheckedChanged(object sender, EventArgs e)
        {
            if (!_bruntime)
            {
                this.MemorizzaValori();
                this.SetManuale();
                this.CaricaSomministrazioni();
            }
        }

        private void ucEasyGridManuale_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            int refWidth = (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XLarge) * 3;

            Graphics g = this.CreateGraphics();
            int refBtnWidth = Convert.ToInt32(CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.ucEasyGrid.DataRowFontRelativeDimension), g.DpiY) * 3);
            g.Dispose();
            g = null;
                        e.Layout.Bands[0].HeaderVisible = false;
            e.Layout.Bands[0].ColHeadersVisible = false;
            e.Layout.Bands[0].RowLayoutStyle = RowLayoutStyle.GroupLayout;
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
                        oCol.RowLayoutColumnInfo.OriginX = 0;
                        oCol.RowLayoutColumnInfo.OriginY = 0;
                        oCol.RowLayoutColumnInfo.SpanX = 1;
                        oCol.RowLayoutColumnInfo.SpanY = 1;
                        break;

                    case "DataOraFine":
                        oCol.Hidden = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Durata == 0;
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
                        oCol.RowLayoutColumnInfo.OriginX = 1;
                        oCol.RowLayoutColumnInfo.OriginY = 0;
                        oCol.RowLayoutColumnInfo.SpanX = 1;
                        oCol.RowLayoutColumnInfo.SpanY = 1;

                        break;

                    case "EtichettaTempo":
                        oCol.Hidden = CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodProtocollo == string.Empty;
                        oCol.Header.Caption = "Descrizione Somministrazione";
                        oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                        try
                        {
                            oCol.MaxWidth = refWidth * 4;
                            oCol.MinWidth = oCol.MaxWidth;
                            oCol.Width = oCol.MaxWidth;
                        }
                        catch (Exception)
                        {
                        }
                                                oCol.RowLayoutColumnInfo.OriginX = 2;
                        oCol.RowLayoutColumnInfo.OriginY = 0;
                        oCol.RowLayoutColumnInfo.SpanX = 1;
                        oCol.RowLayoutColumnInfo.SpanY = 1;

                        break;

                    default:
                        oCol.Hidden = true;
                        break;

                }

                                
            }

            if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_EDIT))
            {
                UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_EDIT);
                colEdit.Hidden = false;

                                colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                colEdit.CellActivation = Activation.AllowEdit;
                colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_MODIFICA_32);


                colEdit.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                try
                {
                    colEdit.MinWidth = refBtnWidth;
                    colEdit.MaxWidth = colEdit.MinWidth;
                    colEdit.Width = colEdit.MaxWidth;
                }
                catch (Exception)
                {
                }
                colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                colEdit.LockedWidth = true;


                colEdit.RowLayoutColumnInfo.OriginX = 3;
                colEdit.RowLayoutColumnInfo.OriginY = 0;
                colEdit.RowLayoutColumnInfo.SpanX = 1;
                colEdit.RowLayoutColumnInfo.SpanY = 1;
            }
                        if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_EDIT + @"_SP"))
            {
                UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_EDIT + @"_SP");
                colEdit.Hidden = false;
                colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                colEdit.CellActivation = Activation.Disabled;
                colEdit.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                try
                {
                    colEdit.MinWidth = Convert.ToInt32(refBtnWidth * 0.25);
                    colEdit.MaxWidth = colEdit.MinWidth;
                    colEdit.Width = colEdit.MaxWidth;
                }
                catch (Exception)
                {
                }
                colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                colEdit.LockedWidth = true;
                colEdit.RowLayoutColumnInfo.OriginX = 4;
                colEdit.RowLayoutColumnInfo.OriginY = 0;
                colEdit.RowLayoutColumnInfo.SpanX = 1;
                colEdit.RowLayoutColumnInfo.SpanY = 1;
            }
            if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_ANNULLA))
            {
                UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_ANNULLA);
                colEdit.Hidden = false;

                                colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                colEdit.CellActivation = Activation.AllowEdit;
                colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_ELIMINA_32);


                colEdit.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                try
                {
                    colEdit.MinWidth = refBtnWidth;
                    colEdit.MaxWidth = colEdit.MinWidth;
                    colEdit.Width = colEdit.MaxWidth;
                }
                catch (Exception)
                {
                }
                colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                colEdit.LockedWidth = true;


                colEdit.RowLayoutColumnInfo.OriginX = 5;
                colEdit.RowLayoutColumnInfo.OriginY = 0;
                colEdit.RowLayoutColumnInfo.SpanX = 1;
                colEdit.RowLayoutColumnInfo.SpanY = 1;
            }
                        if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_ANNULLA + @"_SP"))
            {
                UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_ANNULLA + @"_SP");
                colEdit.Hidden = false;
                colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                colEdit.CellActivation = Activation.Disabled;
                colEdit.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                try
                {
                    colEdit.MinWidth = Convert.ToInt32(refBtnWidth * 0.1);
                    colEdit.MaxWidth = colEdit.MinWidth;
                    colEdit.Width = colEdit.MaxWidth;
                }
                catch (Exception)
                {
                }
                colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                colEdit.LockedWidth = true;
                colEdit.RowLayoutColumnInfo.OriginX = 6;
                colEdit.RowLayoutColumnInfo.OriginY = 0;
                colEdit.RowLayoutColumnInfo.SpanX = 1;
                colEdit.RowLayoutColumnInfo.SpanY = 1;
            }

        }

        private void ucEasyGridManuale_ClickCellButton(object sender, CellEventArgs e)
        {

            try
            {

                switch (e.Cell.Column.Key)
                {

                    case CoreStatics.C_COL_BTN_EDIT:
                        _baddtempo = false;
                        CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerTempi);
                        _ucDatiAggiuntiviPopUp = GetDatiAggiuntiviPopUpForPopupControlContainer();
                        _ucDatiAggiuntiviPopUp.Valore = string.Format("{0}|{1}|{2}",
                                                        e.Cell.Row.Cells["DataOraInizio"].Text,
                                                        (CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Durata == 0 ? "" : e.Cell.Row.Cells["DataOraFine"].Text),
                                                        e.Cell.Row.Cells["EtichettaTempo"].Text);
                        this.UltraPopupControlContainerTempi.Show((ucEasyGrid)sender);
                        break;

                    case CoreStatics.C_COL_BTN_ANNULLA:
                        _baddtempo = false;
                                                                        this.ucEasyGridManuale.ActiveRow.Delete(false);
                        this.ucEasyGridManuale.UpdateData();
                                                break;

                    default:
                        break;

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGridManuale_ClickCellButton", this.Name);
            }

        }

        private void ubAddTempo_Click(object sender, EventArgs e)
        {

            _baddtempo = true;
            CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerTempi);
            _ucDatiAggiuntiviPopUp = GetDatiAggiuntiviPopUpForPopupControlContainer();
            _ucDatiAggiuntiviPopUp.Valore = string.Format("{0}|{1}|{2}",
                                                            DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                                            (CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Durata == 0 ? "" : DateTime.Now.ToString("dd/MM/yyyy HH:mm")),
                                                            "");
            this.UltraPopupControlContainerTempi.Show((ucEasyButton)sender);

        }

        #endregion

        #region Events Controlli di Selezione

        private void ucEasyOptionSet_ValueChanged(object sender, EventArgs e)
        {


            SetOptionSet(ucEasyOptionSet.CheckedItem.DataValue.ToString());
            if (!_bruntime)
            {
                this.ResetOptionSet(ucEasyOptionSet.CheckedItem.DataValue.ToString());
                this.MemorizzaValori();
                this.CaricaSomministrazioni();
            }

        }

        private void udteDataOraInizio_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            if (e.Button.Key == "Orari" && ((ucEasyDateTimeEditor)sender).Value != null)
            {
                CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainer);
                _ucEasyListBox = GetEasyListBoxForPopupControlContainer(sender);
                this.UltraPopupControlContainer.Show((ucEasyDateTimeEditor)sender);
            }

        }
        private void udteDataOraInizio_Validating(object sender, CancelEventArgs e)
        {

            switch ((EnumTipoPrescrizioneTempi)Enum.Parse(typeof(EnumTipoPrescrizioneTempi), this.ucEasyOptionSet.CheckedItem.DataValue.ToString()))
            {

                case EnumTipoPrescrizioneTempi.SN:
                    break;

                case EnumTipoPrescrizioneTempi.RP:
                                        if (udteDataOraInizio_RP.Value != null && udteDataOraFine_RP.Value != null &&
                        Convert.ToDateTime(udteDataOraInizio_RP.Value.ToString()) > Convert.ToDateTime(udteDataOraFine_RP.Value))
                        this.udteDataOraFine_RP.Value = this.udteDataOraInizio_RP.Value;
                    break;

                case EnumTipoPrescrizioneTempi.CN:
                                        if (udteDataOraInizio_CN.Value != null && udteDataOraFine_CN.Value != null &&
                        Convert.ToDateTime(udteDataOraInizio_CN.Value.ToString()) > Convert.ToDateTime(udteDataOraFine_CN.Value))
                        this.udteDataOraFine_CN.Value = this.udteDataOraInizio_CN.Value;
                    break;

                case EnumTipoPrescrizioneTempi.PO:
                                        if (udteDataOraInizio_PO.Value != null && udteDataOraFine_PO.Value != null &&
                        Convert.ToDateTime(udteDataOraInizio_PO.Value.ToString()) > Convert.ToDateTime(udteDataOraFine_PO.Value))
                        this.udteDataOraFine_PO.Value = this.udteDataOraInizio_PO.Value;
                    break;

                case EnumTipoPrescrizioneTempi.POC:
                                        if (udteDataOraInizio_POC.Value != null && udteDataOraFine_POC.Value != null &&
                        Convert.ToDateTime(udteDataOraInizio_POC.Value.ToString()) > Convert.ToDateTime(udteDataOraFine_POC.Value))
                        this.udteDataOraFine_POC.Value = this.udteDataOraInizio_POC.Value;
                    break;

                case EnumTipoPrescrizioneTempi.PG:
                    break;

                case EnumTipoPrescrizioneTempi.AB:
                    break;

            }

        }

        private void udteDataOraFine_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            if (e.Button.Key == "Orari" && ((ucEasyDateTimeEditor)sender).Value != null)
            {

                CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainer);
                _ucEasyListBox = GetEasyListBoxForPopupControlContainer(sender);
                this.UltraPopupControlContainer.Show((ucEasyDateTimeEditor)sender);
            }

        }
        private void udteDataOraFine_Validating(object sender, CancelEventArgs e)
        {

            switch ((EnumTipoPrescrizioneTempi)Enum.Parse(typeof(EnumTipoPrescrizioneTempi), this.ucEasyOptionSet.CheckedItem.DataValue.ToString()))
            {

                case EnumTipoPrescrizioneTempi.SN:
                    break;

                case EnumTipoPrescrizioneTempi.RP:
                                        if (udteDataOraInizio_RP.Value != null && udteDataOraFine_RP.Value != null &&
                                Convert.ToDateTime(udteDataOraInizio_RP.Value.ToString()) > Convert.ToDateTime(udteDataOraFine_RP.Value))
                        this.udteDataOraInizio_RP.Value = this.udteDataOraFine_RP.Value;
                    break;

                case EnumTipoPrescrizioneTempi.CN:
                                        if (udteDataOraInizio_CN.Value != null && udteDataOraFine_CN.Value != null &&
                                Convert.ToDateTime(udteDataOraInizio_CN.Value.ToString()) > Convert.ToDateTime(udteDataOraFine_CN.Value))
                        this.udteDataOraInizio_CN.Value = this.udteDataOraFine_CN.Value;
                    break;

                case EnumTipoPrescrizioneTempi.PO:
                                        if (udteDataOraInizio_PO.Value != null && udteDataOraFine_PO.Value != null &&
                                Convert.ToDateTime(udteDataOraInizio_PO.Value.ToString()) > Convert.ToDateTime(udteDataOraFine_PO.Value))
                        this.udteDataOraInizio_PO.Value = this.udteDataOraFine_PO.Value;
                    break;

                case EnumTipoPrescrizioneTempi.POC:
                                        if (udteDataOraInizio_POC.Value != null && udteDataOraFine_POC.Value != null &&
                                Convert.ToDateTime(udteDataOraInizio_POC.Value.ToString()) > Convert.ToDateTime(udteDataOraFine_POC.Value))
                        this.udteDataOraInizio_POC.Value = this.udteDataOraFine_POC.Value;
                    break;

                case EnumTipoPrescrizioneTempi.PG:
                    break;

                case EnumTipoPrescrizioneTempi.AB:
                    break;

            }

        }

        private void umeDurata_ValueChanged(object sender, EventArgs e)
        {
            if (!_bruntime)
            {
                this.MemorizzaValori();
                this.CaricaSomministrazioni();
            }
        }

        private void uceProtocollo_ValueChanged(object sender, EventArgs e)
        {
            try
            {

                _bruntime = true;

                switch ((EnumTipoPrescrizioneTempi)Enum.Parse(typeof(EnumTipoPrescrizioneTempi), this.ucEasyOptionSet.CheckedItem.DataValue.ToString()))
                {

                    case EnumTipoPrescrizioneTempi.SN:
                        break;

                    case EnumTipoPrescrizioneTempi.RP:
                        break;

                    case EnumTipoPrescrizioneTempi.CN:
                        break;

                    case EnumTipoPrescrizioneTempi.PO:
                        if (this.uceProtocollo_PO.SelectedItem != null && ((DataRowView)this.uceProtocollo_PO.SelectedItem.ListObject).Row["CodTipoProtocollo"].ToString() == EnumTipoProtocollo.DELTA.ToString())
                        {
                            this.udteDataOraFine_PO.Enabled = false;
                            this.udteDataOraFine_PO.Value = null;
                        }
                        else
                        {
                            this.udteDataOraFine_PO.Enabled = true;
                        }
                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Continuita = GetAttivazioneContinuita_PO();
                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodTipoProtocollo = GetTipoProtocollo_PO();
                        break;

                    case EnumTipoPrescrizioneTempi.POC:
                        if (this.uceProtocollo_POC.SelectedItem != null && ((DataRowView)this.uceProtocollo_POC.SelectedItem.ListObject).Row["CodTipoProtocollo"].ToString() == EnumTipoProtocollo.DELTA.ToString())
                        {
                            this.udteDataOraFine_POC.Enabled = false;
                            this.udteDataOraFine_POC.Value = null;
                        }
                        else
                        {
                            this.udteDataOraFine_POC.Enabled = true;
                        }
                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Continuita = GetAttivazioneContinuita_POC();
                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Durata = GetValoreDurata_POC();
                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodTipoProtocollo = GetTipoProtocollo_POC();
                        break;

                    case EnumTipoPrescrizioneTempi.PG:
                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Continuita = GetAttivazioneContinuita_PG();
                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodTipoProtocollo = GetTipoProtocollo_PG();
                        break;

                    case EnumTipoPrescrizioneTempi.AB:
                        break;

                }

                this.MemorizzaValori();
                this.CaricaSomministrazioni();

                _bruntime = false;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Salva", this.Text);
            }
        }

        private void ubProtocolloD_Click(object sender, EventArgs e)
        {

            CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerProtocollo);
            _ucEasyGrid = GetEasyGridForPopupControlContainer(EnumTipoProtocollo.DELTA.ToString());
            this.UltraPopupControlContainerProtocollo.Show((ucEasyButton)sender);

            _ucEasyGrid.DisplayLayout.Bands[0].ClearGroupByColumns();
            if (_ucEasyGrid.DisplayLayout.Bands[0].Columns.Exists("Codice") == true)
            {
                _ucEasyGrid.DisplayLayout.Bands[0].Columns["Codice"].Hidden = true;
            }
            if (_ucEasyGrid.DisplayLayout.Bands[0].Columns.Exists("Descrizione") == true)
            {
                _ucEasyGrid.DisplayLayout.Bands[0].Columns["Descrizione"].Header.Caption = "Protocolli";
            }
            if (_ucEasyGrid.DisplayLayout.Bands[0].Columns.Exists("Continuita") == true)
            {
                _ucEasyGrid.DisplayLayout.Bands[0].Columns["Continuita"].Hidden = true;
            }
            if (_ucEasyGrid.DisplayLayout.Bands[0].Columns.Exists("Durata") == true)
            {
                _ucEasyGrid.DisplayLayout.Bands[0].Columns["Durata"].Hidden = true;
            }
            if (_ucEasyGrid.DisplayLayout.Bands[0].Columns.Exists("CodTipoProtocollo") == true)
            {
                _ucEasyGrid.DisplayLayout.Bands[0].Columns["CodTipoProtocollo"].Hidden = true;
            }
            _ucEasyGrid.DisplayLayout.Bands[0].Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            _ucEasyGrid.Refresh();

        }

        private void ubProtocolloH_Click(object sender, EventArgs e)
        {

            CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerProtocollo);
            _ucEasyGrid = GetEasyGridForPopupControlContainer(EnumTipoProtocollo.ORA.ToString());
            this.UltraPopupControlContainerProtocollo.Show((ucEasyButton)sender);

            _ucEasyGrid.DisplayLayout.Bands[0].ClearGroupByColumns();
            if (_ucEasyGrid.DisplayLayout.Bands[0].Columns.Exists("Codice") == true)
            {
                _ucEasyGrid.DisplayLayout.Bands[0].Columns["Codice"].Hidden = true;
            }
            if (_ucEasyGrid.DisplayLayout.Bands[0].Columns.Exists("Descrizione") == true)
            {
                _ucEasyGrid.DisplayLayout.Bands[0].Columns["Descrizione"].Header.Caption = "Protocolli";
            }
            if (_ucEasyGrid.DisplayLayout.Bands[0].Columns.Exists("Continuita") == true)
            {
                _ucEasyGrid.DisplayLayout.Bands[0].Columns["Continuita"].Hidden = true;
            }
            if (_ucEasyGrid.DisplayLayout.Bands[0].Columns.Exists("Durata") == true)
            {
                _ucEasyGrid.DisplayLayout.Bands[0].Columns["Durata"].Hidden = true;
            }
            if (_ucEasyGrid.DisplayLayout.Bands[0].Columns.Exists("CodTipoProtocollo") == true)
            {
                _ucEasyGrid.DisplayLayout.Bands[0].Columns["CodTipoProtocollo"].Hidden = true;
            }
            _ucEasyGrid.DisplayLayout.Bands[0].Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            _ucEasyGrid.Refresh();

        }

        private void GestValueChange(object sender, EventArgs e)
        {
            if (!_bruntime)
            {
                this.CalcolaDurata();
                this.MemorizzaValori();
                this.CaricaSomministrazioni();
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

        #region UltraPopupControlContainer Protocollo

        private void UltraPopupControlContainerProtocollo_Closed(object sender, EventArgs e)
        {
            _ucEasyGrid.ClickCell -= ucEasyGrid_ClickCell;
        }

        private void UltraPopupControlContainerProtocollo_Opened(object sender, EventArgs e)
        {
            _ucEasyGrid.ClickCell += ucEasyGrid_ClickCell;
            _ucEasyGrid.Focus();
            _ucEasyGrid.ActiveRow = _ucEasyGrid.Rows.FilterRow;
            _ucEasyGrid.ActiveCell = _ucEasyGrid.Rows.FilterRow.Cells[1];
            _ucEasyGrid.PerformAction(UltraGridAction.EnterEditMode, false, false);
        }

        private void UltraPopupControlContainerProtocollo_Opening(object sender, CancelEventArgs e)
        {
            Infragistics.Win.Misc.UltraPopupControlContainer popup = sender as Infragistics.Win.Misc.UltraPopupControlContainer;
            popup.PopupControl = _ucEasyGrid;
        }

        private void ucEasyGrid_ClickCell(object sender, ClickCellEventArgs e)
        {

            if (e.Cell.IsDataCell == true)
            {
                switch ((EnumTipoPrescrizioneTempi)Enum.Parse(typeof(EnumTipoPrescrizioneTempi), this.ucEasyOptionSet.CheckedItem.DataValue.ToString()))
                {

                    case EnumTipoPrescrizioneTempi.SN:                          break;

                    case EnumTipoPrescrizioneTempi.RP:                          break;

                    case EnumTipoPrescrizioneTempi.CN:                          break;

                    case EnumTipoPrescrizioneTempi.PO:                          this.uceProtocollo_PO.Value = e.Cell.Row.Cells["Codice"].Value;
                        break;

                    case EnumTipoPrescrizioneTempi.POC:                          this.uceProtocollo_POC.Value = e.Cell.Row.Cells["Codice"].Value;
                        break;

                    case EnumTipoPrescrizioneTempi.PG:                          this.uceProtocollo_PG.Value = e.Cell.Row.Cells["Codice"].Value;
                        break;

                    case EnumTipoPrescrizioneTempi.AB:                          break;

                }

                CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodTipoProtocollo = e.Cell.Row.Cells["CodTipoProtocollo"].Text;
                this.UltraPopupControlContainerProtocollo.Close();
            }

        }

        #endregion

        #region UltraPopupControlContainer Rtf

        private void UltraPopupControlContainerRtf_Closed(object sender, EventArgs e)
        {
            _ucRichTextBox.RtfClick -= ucRichTextBox_Click;
        }

        private void UltraPopupControlContainerRtf_Opened(object sender, EventArgs e)
        {
            _ucRichTextBox.RtfClick += ucRichTextBox_Click;
        }

        private void UltraPopupControlContainerRtf_Opening(object sender, CancelEventArgs e)
        {
            Infragistics.Win.Misc.UltraPopupControlContainer popup = sender as Infragistics.Win.Misc.UltraPopupControlContainer;
            popup.PopupControl = _ucRichTextBox;
        }

        private void ucRichTextBox_Click(object sender, EventArgs e)
        {
            this.UltraPopupControlContainerRtf.Close();
        }

        #endregion

        #region UltraPopupControlContainer Tempi

        private void UltraPopupControlContainerTempi_Closed(object sender, EventArgs e)
        {
            _ucDatiAggiuntiviPopUp.Annulla_Click -= ubAnnullaTempi_Click;
            _ucDatiAggiuntiviPopUp.Conferma_Click -= ubConfermaTempi_Click;
        }

        private void UltraPopupControlContainerTempi_Opened(object sender, EventArgs e)
        {
            _ucDatiAggiuntiviPopUp.Annulla_Click += ubAnnullaTempi_Click;
            _ucDatiAggiuntiviPopUp.Conferma_Click += ubConfermaTempi_Click;
            _ucDatiAggiuntiviPopUp.Focus();
        }

        private void UltraPopupControlContainerTempi_Opening(object sender, CancelEventArgs e)
        {
            Infragistics.Win.Misc.UltraPopupControlContainer popup = sender as Infragistics.Win.Misc.UltraPopupControlContainer;
            popup.PopupControl = _ucDatiAggiuntiviPopUp;
        }

        private void ubAnnullaTempi_Click(object sender, EventArgs e)
        {
            this.UltraPopupControlContainerTempi.Close();
            _baddtempo = false;
        }

        private void ubConfermaTempi_Click(object sender, EventArgs e)
        {

            this.UltraPopupControlContainerTempi.Close();

            string[] valori = _ucDatiAggiuntiviPopUp.Valore.Split('|');

            if (_baddtempo == true)
            {
                this.ucEasyGridManuale.DisplayLayout.Override.AllowAddNew = AllowAddNew.Yes;
                UltraGridRow orow = this.ucEasyGridManuale.DisplayLayout.Bands[0].AddNew();
                this.ucEasyGridManuale.ActiveRow = orow;
            }

            DateTime dt = DateTime.MinValue;

            try
            {
                dt = DateTime.Parse(valori[0]);
            }
            catch (Exception)
            {
                dt = DateTime.MinValue;
            }
            this.ucEasyGridManuale.ActiveRow.Cells["DataOraInizio"].Value = dt;

            try
            {
                dt = DateTime.Parse(valori[1]);
                this.ucEasyGridManuale.ActiveRow.Cells["DataOraFine"].Value = dt;
            }
            catch (Exception)
            {

            }

            this.ucEasyGridManuale.ActiveRow.Cells["EtichettaTempo"].Value = valori[2];

            this.ucEasyGridManuale.UpdateData();
            this.ucEasyGridManuale.Refresh();
            this.ucEasyGridManuale.DisplayLayout.Bands[0].SortedColumns.RefreshSort(false);


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

        #region Events UserControl

        private void ucEasyCheckEditorDC_CheckedChanged(object sender, EventArgs e)
        {
            this.SetDC(this.ucEasyCheckEditorDC.Checked);
        }

        void ucDcViewer_KeyEvent(System.Windows.Input.Key key)
        {

            try
            {

                if (key == System.Windows.Input.Key.F1)
                {
                    frmPUPrescrizioniTempi_PulsanteIndietroClick(null, new PulsanteBottomClickEventArgs(EnumPulsanteBottom.Indietro));
                }
                else if (key == System.Windows.Input.Key.F12)
                {
                    frmPUPrescrizioniTempi_PulsanteAvantiClick(null, new PulsanteBottomClickEventArgs(EnumPulsanteBottom.Avanti));
                }

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ucDcViewer_KeyEvent", this.Text);
            }

        }

        #endregion

    }
}
