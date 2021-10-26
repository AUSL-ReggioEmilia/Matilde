using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.ScciCore.ViewController;
using UnicodeSrl.ScciResource;
using UnicodeSrl.Framework.Data;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinListView;
using Infragistics.Win.Misc;
using UnicodeSrl.Framework.Threading;
using System.Threading;
using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.DatiClinici.DC;

namespace UnicodeSrl.ScciCore
{
    public partial class frmPUMultiTaskInfermieristici : frmBaseModale, Interfacce.IViewFormlModal, IViewControllerMultiTaskInfermieristico
    {
        public frmPUMultiTaskInfermieristici()
        {
            InitializeComponent();
        }

        #region Declare

        private bool _bruntime = false;
        private ucEasyListBox _ucEasyListBox = null;
        private ucEasyGrid _ucEasyGrid = null;
                private ucSegnalibri _ucSegnalibri = null;

        #endregion

        #region Interface ViewFormlModal

        public void Carica()
        {

            try
            {

                this.Icon = Risorse.GetIconFromResource(Risorse.GC_WORKLIST_16);

                this.LoadViewController();

                this.InizializzaControlli();

                this.Aggiorna();

                this.ShowDialog();

            }
            catch (Exception)
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }

        }

        #endregion

        #region Interface ViewController

        public ViewControllerMultiTaskInfermieristico ViewController { get; set; }

        public Maschera Maschera { get; set; }

        public void InitViewController(IViewController viewcontroller)
        {
            this.ViewController = (ViewControllerMultiTaskInfermieristico)viewcontroller;
        }

        public void LoadViewController()
        {
            this.Text = this.Maschera.Descrizione;
        }

        public IViewController SaveViewController()
        {
            this.ViewController.DialogResultReturn = this.DialogResult;
            return this.ViewController;
        }

        #endregion

        #region Functions

        private void InizializzaControlli()
        {

            try
            {

                this._bruntime = true;

                this.ucEasyHourRange.RangeOre = ScciCore.ucEasyHourRange.EnumHourRange.Next24;
                this.ucEasyHourRange.DataInizio = DateTime.Now;

                this.LoadOptionSet();

                this.lblElencoSomministrazioni.Text = "Elenco Task";

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
                op.Parametro.Add("CodTipoTaskInfermieristico", this.ViewController.MovTaskInfermieristico.CodTipoTaskInfermieristico);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataTable dt = Database.GetDataTableStoredProc("MSP_SelTipoTaskInfermieristiciTempi", spcoll);

                if (dt != null)
                {
                    foreach (DataRow oRow in dt.Rows)
                    {
                        if (oRow["CodTipoTaskInfermieristicoTempi"].ToString() == EnumTipoTaskInfermieristicoTempi.SN.ToString())
                        {
                            Infragistics.Win.ValueListItem oVal = new Infragistics.Win.ValueListItem(oRow["CodTipoTaskInfermieristicoTempi"].ToString(), oRow["DescTipoTaskInfermieristicoTempi"].ToString());
                            this.ucEasyOptionSet.Items.Add(oVal);
                        }
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

            switch ((EnumTipoTaskInfermieristicoTempi)Enum.Parse(typeof(EnumTipoTaskInfermieristicoTempi), key))
            {

                case EnumTipoTaskInfermieristicoTempi.SN:                     this.ucEasyTabControlSelezioni.Tabs[key].Visible = true;
                    this.ucEasyTabControlSelezioni.ActiveTab = this.ucEasyTabControlSelezioni.Tabs[key];
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoTaskInfermieristicoTempi.RP.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoTaskInfermieristicoTempi.PO.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoTaskInfermieristicoTempi.PG.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs["CN"].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs["POC"].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs["AB"].Visible = false;
                    break;

                case EnumTipoTaskInfermieristicoTempi.RP:                      this.ucEasyTabControlSelezioni.Tabs[key].Visible = true;
                    this.ucEasyTabControlSelezioni.ActiveTab = this.ucEasyTabControlSelezioni.Tabs[key];
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoTaskInfermieristicoTempi.SN.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoTaskInfermieristicoTempi.PO.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoTaskInfermieristicoTempi.PG.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs["CN"].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs["POC"].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs["AB"].Visible = false;
                    break;

                case EnumTipoTaskInfermieristicoTempi.PO:                      this.ucEasyTabControlSelezioni.Tabs[key].Visible = true;
                    this.ucEasyTabControlSelezioni.ActiveTab = this.ucEasyTabControlSelezioni.Tabs[key];
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoTaskInfermieristicoTempi.SN.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoTaskInfermieristicoTempi.RP.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoTaskInfermieristicoTempi.PG.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs["CN"].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs["POC"].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs["AB"].Visible = false;
                    break;

                case EnumTipoTaskInfermieristicoTempi.PG:                      this.ucEasyTabControlSelezioni.Tabs[key].Visible = true;
                    this.ucEasyTabControlSelezioni.ActiveTab = this.ucEasyTabControlSelezioni.Tabs[key];
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoTaskInfermieristicoTempi.SN.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoTaskInfermieristicoTempi.RP.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs[EnumTipoTaskInfermieristicoTempi.PO.ToString()].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs["CN"].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs["POC"].Visible = false;
                    this.ucEasyTabControlSelezioni.Tabs["AB"].Visible = false;
                    break;

            }

        }

        private void GetOptionSet()
        {

            foreach (Infragistics.Win.ValueListItem item in this.ucEasyOptionSet.Items)
            {

                if (item.DataValue.ToString() == this.ViewController.MovTaskInfermieristico.CodTipoTaskInfermieristicoTempi)
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

            if (this.ViewController.MovTaskInfermieristico.DataProgrammata == DateTime.MinValue)
            {
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
            }
            else
            {
                this.udteDataOraInizio_SN.Value = this.ViewController.MovTaskInfermieristico.DataProgrammata;
                this.udteDataOraInizio_RP.Value = this.ViewController.MovTaskInfermieristico.DataProgrammata;
                this.udteDataOraInizio_CN.Value = this.ViewController.MovTaskInfermieristico.DataProgrammata;
                this.udteDataOraInizio_PO.Value = this.ViewController.MovTaskInfermieristico.DataProgrammata;
                this.udteDataOraInizio_POC.Value = this.ViewController.MovTaskInfermieristico.DataProgrammata;
                this.udteDataOraInizio_PG.Value = this.ViewController.MovTaskInfermieristico.DataProgrammata;
                this.udteDataOraFine_RP.Value = this.ViewController.MovTaskInfermieristico.DataProgrammata;
                this.udteDataOraFine_CN.Value = this.ViewController.MovTaskInfermieristico.DataProgrammata;
                this.udteDataOraFine_PO.Value = this.ViewController.MovTaskInfermieristico.DataProgrammata;
                this.udteDataOraFine_POC.Value = this.ViewController.MovTaskInfermieristico.DataProgrammata;
            }

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

            this.ViewController.MovTaskInfermieristico.CodProtocollo = string.Empty;
            this.ViewController.MovTaskInfermieristico.CodTipoProtocollo = string.Empty;
            this.ViewController.MovTaskInfermieristico.CodTipoTaskInfermieristicoTempi = key;
            this.ViewController.MovTaskInfermieristico.Continuita = false;

            switch ((EnumTipoTaskInfermieristicoTempi)Enum.Parse(typeof(EnumTipoTaskInfermieristicoTempi), key))
            {

                case EnumTipoTaskInfermieristicoTempi.SN:                                         break;

                case EnumTipoTaskInfermieristicoTempi.RP:                                          break;

                case EnumTipoTaskInfermieristicoTempi.PO:                      this.LoadProtocollo_PO();
                                        break;

                case EnumTipoTaskInfermieristicoTempi.PG:                      this.LoadProtocollo_PG();
                                        break;

            }

            this._bruntime = false;

        }

        public void Aggiorna()
        {
            try
            {
                if (this.ViewController.MovTaskInfermieristico.Azione == EnumAzioni.INS)
                {
                    
                    this.ucEasyOptionSet.CheckedIndex = 0;

                    _bruntime = true;

                    if (this.ViewController.MovTaskInfermieristico.DataProgrammata > DateTime.MinValue)
                    {
                        this.udteDataOraInizio_SN.Value = this.ViewController.MovTaskInfermieristico.DataProgrammata;
                        this.udteDataOraInizio_RP.Value = this.ViewController.MovTaskInfermieristico.DataProgrammata;
                        this.udteDataOraInizio_CN.Value = this.ViewController.MovTaskInfermieristico.DataProgrammata;
                        this.udteDataOraInizio_PO.Value = this.ViewController.MovTaskInfermieristico.DataProgrammata;
                        this.udteDataOraInizio_POC.Value = this.ViewController.MovTaskInfermieristico.DataProgrammata;
                        this.udteDataOraInizio_PG.Value = this.ViewController.MovTaskInfermieristico.DataProgrammata;
                    }
                    if (this.ViewController.MovTaskInfermieristico.PeriodicitaDataFine > DateTime.MinValue)
                    {
                        this.udteDataOraFine_RP.Value = this.ViewController.MovTaskInfermieristico.PeriodicitaDataFine;
                        this.udteDataOraFine_CN.Value = this.ViewController.MovTaskInfermieristico.PeriodicitaDataFine;
                        this.udteDataOraFine_PO.Value = this.ViewController.MovTaskInfermieristico.PeriodicitaDataFine;
                        this.udteDataOraFine_POC.Value = this.ViewController.MovTaskInfermieristico.PeriodicitaDataFine;
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

                    this.GetOptionSet();

                    if (this.ViewController.MovTaskInfermieristico.DataProgrammata > DateTime.MinValue)
                    {
                        this.udteDataOraInizio_SN.Value = this.ViewController.MovTaskInfermieristico.DataProgrammata;
                        this.udteDataOraInizio_RP.Value = this.ViewController.MovTaskInfermieristico.DataProgrammata;
                        this.udteDataOraInizio_CN.Value = this.ViewController.MovTaskInfermieristico.DataProgrammata;
                        this.udteDataOraInizio_PO.Value = this.ViewController.MovTaskInfermieristico.DataProgrammata;
                        this.udteDataOraInizio_POC.Value = this.ViewController.MovTaskInfermieristico.DataProgrammata;
                        this.udteDataOraInizio_PG.Value = this.ViewController.MovTaskInfermieristico.DataProgrammata;
                    }
                    if (this.ViewController.MovTaskInfermieristico.PeriodicitaDataFine > DateTime.MinValue)
                    {
                        this.udteDataOraFine_RP.Value = this.ViewController.MovTaskInfermieristico.PeriodicitaDataFine;
                        this.udteDataOraFine_CN.Value = this.ViewController.MovTaskInfermieristico.PeriodicitaDataFine;
                        this.udteDataOraFine_PO.Value = this.ViewController.MovTaskInfermieristico.PeriodicitaDataFine;
                        this.udteDataOraFine_POC.Value = this.ViewController.MovTaskInfermieristico.PeriodicitaDataFine;
                    }

                    this.ucSelPeriodicitaGiorni_RP.Value = this.ViewController.MovTaskInfermieristico.PeriodicitaGiorni;
                    this.ucSelPeriodicitaOre_RP.Value = this.ViewController.MovTaskInfermieristico.PeriodicitaOre;
                    this.ucSelPeriodicitaMinuti_RP.Value = this.ViewController.MovTaskInfermieristico.PeriodicitaMinuti;

                    this.ucSelPeriodicitaGiorni_CN.Value = this.ViewController.MovTaskInfermieristico.PeriodicitaGiorni;
                    this.ucSelPeriodicitaOre_CN.Value = this.ViewController.MovTaskInfermieristico.PeriodicitaOre;
                    this.ucSelPeriodicitaMinuti_CN.Value = this.ViewController.MovTaskInfermieristico.PeriodicitaMinuti;

                                                                                                                                                                                                        
                    this.LoadProtocollo_PO();
                    this.LoadProtocollo_PG();

                    this.uceProtocollo_PO.Value = this.ViewController.MovTaskInfermieristico.CodProtocollo;
                    this.uceProtocollo_POC.Value = this.ViewController.MovTaskInfermieristico.CodProtocollo;
                    this.uceProtocollo_PG.Value = this.ViewController.MovTaskInfermieristico.CodProtocollo;

                    this.ucEasyOptionSet.Enabled = false;

                    _bruntime = false;
                }

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "Aggiorna", this.Text);
            }

        }

        private void LoadProtocollo_PO()
        {
            try
            {
                
                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodTipoTaskInfermieristico", this.ViewController.MovTaskInfermieristico.CodTipoTaskInfermieristico);
                op.Parametro.Add("Continuita", "0");

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataTable dt = Database.GetDataTableStoredProc("MSP_SelProtocolliTaskInfermieristici", spcoll);

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
        private void LoadProtocollo_PG()
        {
            try
            {
                
                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodTipoTaskInfermieristico", this.ViewController.MovTaskInfermieristico.CodTipoTaskInfermieristico);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataTable dt = Database.GetDataTableStoredProc("MSP_SelProtocolliTaskInfermieristici", spcoll);

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

        private void MemorizzaValori()
        {

            DateTime dtI = DateTime.MinValue;
            DateTime dtF = DateTime.MinValue;

            this.ViewController.CodTipoProtocollo = this.ucEasyOptionSet.CheckedItem.DataValue.ToString();

            switch ((EnumTipoTaskInfermieristicoTempi)Enum.Parse(typeof(EnumTipoTaskInfermieristicoTempi), this.ucEasyOptionSet.CheckedItem.DataValue.ToString()))
            {

                case EnumTipoTaskInfermieristicoTempi.SN:                      if (this.udteDataOraInizio_SN.Value != null)
                    {
                        dtI = (DateTime)this.udteDataOraInizio_SN.Value;
                        dtF = (DateTime)this.udteDataOraInizio_SN.Value;
                    }
                    this.ViewController.MovTaskInfermieristico.Periodicita = false;
                    break;

                case EnumTipoTaskInfermieristicoTempi.RP:                      if (this.udteDataOraInizio_RP.Value != null && this.udteDataOraFine_RP.Value != null &&
                        this.ucSelPeriodicitaGiorni_RP.Value != null && this.ucSelPeriodicitaOre_RP.Value != null && this.ucSelPeriodicitaMinuti_RP.Value != null)
                    {
                        dtI = (DateTime)this.udteDataOraInizio_RP.Value;
                        dtF = this.udteDataOraFine_RP.Value != null ? (DateTime)this.udteDataOraFine_RP.Value : DateTime.MinValue;
                        this.ViewController.MovTaskInfermieristico.PeriodicitaGiorni = (int)this.ucSelPeriodicitaGiorni_RP.Value;
                        this.ViewController.MovTaskInfermieristico.PeriodicitaOre = (int)this.ucSelPeriodicitaOre_RP.Value;
                        this.ViewController.MovTaskInfermieristico.PeriodicitaMinuti = (int)this.ucSelPeriodicitaMinuti_RP.Value;
                    }
                    this.ViewController.MovTaskInfermieristico.Periodicita = true;
                    break;

                case EnumTipoTaskInfermieristicoTempi.PO:                      if (this.udteDataOraInizio_PO.Value != null && this.udteDataOraFine_PO.Value != null &&
                        this.uceProtocollo_PO.SelectedItem != null)
                    {
                        dtI = (DateTime)this.udteDataOraInizio_PO.Value;
                        dtF = this.udteDataOraFine_PO.Value != null ? (DateTime)this.udteDataOraFine_PO.Value : DateTime.MinValue;
                        this.ViewController.MovTaskInfermieristico.CodProtocollo = this.uceProtocollo_PO.SelectedItem != null ? this.uceProtocollo_PO.SelectedItem.DataValue.ToString() : string.Empty;
                    }
                    this.ViewController.MovTaskInfermieristico.Periodicita = true;
                    break;

                case EnumTipoTaskInfermieristicoTempi.PG:                      if (this.udteDataOraInizio_PG.Value != null &&
                        this.uceProtocollo_PG.SelectedItem != null)
                    {
                        dtI = (DateTime)this.udteDataOraInizio_PG.Value;
                        dtF = (DateTime)this.udteDataOraInizio_PG.Value;
                        this.ViewController.MovTaskInfermieristico.CodProtocollo = this.uceProtocollo_PG.SelectedItem != null ? this.uceProtocollo_PG.SelectedItem.DataValue.ToString() : string.Empty;
                    }
                    this.ViewController.MovTaskInfermieristico.Periodicita = true;
                    break;

            }

            this.ViewController.MovTaskInfermieristico.DataProgrammata = new DateTime(dtI.Year, dtI.Month, dtI.Day, dtI.Hour, dtI.Minute, 0);
            this.ViewController.MovTaskInfermieristico.PeriodicitaDataFine =
                            this.udteDataOraFine_RP.Value != null ? new DateTime(dtF.Year, dtF.Month, dtF.Day, dtF.Hour, dtF.Minute, 0) : DateTime.MinValue;

        }

        private void CaricaSomministrazioni()
        {

            List<IntervalloTempi> tempi = this.ViewController.MovTaskInfermieristico.AnteprimaPeriodicita();
            this.lblElencoSomministrazioni.Text = "Elenco Task (" + tempi.Count + ")";
            if (tempi.Count > 0)
                this.ucEasyGrid.DataSource = GetIntervalloTempiDT(tempi);
            else
                this.ucEasyGrid.DataSource = null;
            this.ucEasyGrid.Refresh();

        }

        private DataTable GetIntervalloTempiDT(List<IntervalloTempi> tempi)
        {

            DataTable dtReturn = new DataTable("Table");

            dtReturn.Columns.Add("CodTipo", typeof(string));
            dtReturn.Columns.Add("Descrizione", typeof(string));

            dtReturn.Columns.Add("DataOraInizio", typeof(DateTime));
            dtReturn.Columns.Add("DataOraFine", typeof(DateTime));

            dtReturn.Columns.Add("NomeProtocollo", typeof(string));
            dtReturn.Columns.Add("EtichettaTempo", typeof(string));

            foreach (string tipo in this.ViewController.MovTaskInfermieristico.List_CodTipoTaskInfermieristico)
            {

                foreach (IntervalloTempi oIt in tempi)
                {

                    DataRow oDr = dtReturn.NewRow();

                    oDr["CodTipo"] = tipo;
                    oDr["Descrizione"] = GetDescrizioneTipoTask(tipo, this.ViewController.MovTaskInfermieristico.CodUA);
                    oDr["DataOraInizio"] = oIt.DataOraInizio;
                    oDr["DataOraFine"] = oIt.DataOraFine;
                    oDr["NomeProtocollo"] = oIt.NomeProtocollo;
                    oDr["EtichettaTempo"] = oIt.EtichettaTempo;

                    dtReturn.Rows.Add(oDr);

                }

            }

            return dtReturn;

        }

        private string GetDescrizioneTipoTask(string tipo, string codUA)
        {

            string sret = string.Empty;

                        Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
            op.Parametro.Add("CodUA", codUA);
            op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
            op.Parametro.Add("CodTipoTaskInfermieristico", tipo);
            op.Parametro.Add("SoloFiltroTipoTaskInfermieristico", "1");
            SqlParameterExt[] spcoll = new SqlParameterExt[1];
            string xmlParam = XmlProcs.XmlSerializeToString(op);
            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
            DataTable oDt = Database.GetDataTableStoredProc("MSP_SelTipoTaskInfermieristico", spcoll);
            if (oDt != null && oDt.Rows.Count == 1 && !oDt.Rows[0].IsNull("Descrizione"))
            {
                sret = oDt.Rows[0]["Descrizione"].ToString();
            }
            return sret;

        }

        private string GetCodSchedaTipoTask(string tipo, string codUA)
        {

            string sret = string.Empty;

                        Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
            op.Parametro.Add("CodUA", codUA);
            op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
            op.Parametro.Add("CodTipoTaskInfermieristico", tipo);
            op.Parametro.Add("SoloFiltroTipoTaskInfermieristico", "1");
            SqlParameterExt[] spcoll = new SqlParameterExt[1];
            string xmlParam = XmlProcs.XmlSerializeToString(op);
            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
            DataTable oDt = Database.GetDataTableStoredProc("MSP_SelTipoTaskInfermieristico", spcoll);
            if (oDt != null && oDt.Rows.Count == 1 && !oDt.Rows[0].IsNull("CodScheda"))
            {
                sret = oDt.Rows[0]["CodScheda"].ToString();
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
                op.Parametro.Add("CodTipoTaskInfermieristico", this.ViewController.MovTaskInfermieristico.CodTipoTaskInfermieristico);
                op.Parametro.Add("CodTipoProtocollo", CodTipoProtocollo);

                switch ((EnumTipoTaskInfermieristicoTempi)Enum.Parse(typeof(EnumTipoTaskInfermieristicoTempi), this.ucEasyOptionSet.CheckedItem.DataValue.ToString()))
                {

                    case EnumTipoTaskInfermieristicoTempi.SN:                          break;

                    case EnumTipoTaskInfermieristicoTempi.RP:                          break;

                    case EnumTipoTaskInfermieristicoTempi.PO:                          op.Parametro.Add("Continuita", "0");
                        break;

                    case EnumTipoTaskInfermieristicoTempi.PG:                          break;

                }

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataTable dt = Database.GetDataTableStoredProc("MSP_SelProtocolliTaskInfermieristici", spcoll);

                _ucEasyGrid.DataSource = dt;
                _ucEasyGrid.Refresh();

            }
            catch (Exception)
            {

            }

            return _ucEasyGrid;

        }

        public bool Salva()
        {
            bool bReturn = false;
            try
            {

                if (this.ViewController.MovTaskInfermieristico.Azione != EnumAzioni.VIS)
                {
                    this.ImpostaCursore(enum_app_cursors.WaitCursor);
                                        enableControls(false);

                    if (ControllaValori())
                    {
                        MemorizzaValori();
                    }

                }
                else
                    bReturn = true;
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Salva", this.Text);
            }
            finally
            {
                                enableControls(true);
            }

            this.ImpostaCursore(enum_app_cursors.DefaultCursor);

            return bReturn;
        }

        private bool ControllaValori()
        {

            bool bOK = true;

            switch ((EnumTipoTaskInfermieristicoTempi)Enum.Parse(typeof(EnumTipoTaskInfermieristicoTempi), this.ucEasyOptionSet.CheckedItem.DataValue.ToString()))
            {

                case EnumTipoTaskInfermieristicoTempi.SN:
                                        if (bOK && this.udteDataOraInizio_SN.Value == null)
                    {
                        easyStatics.EasyMessageBox("E' necessario definire Data/Ora Inizio.", "Tempi Task Infermieristico", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.udteDataOraInizio_SN.Focus();
                        bOK = false;
                    }
                    break;

                case EnumTipoTaskInfermieristicoTempi.RP:
                                        if (bOK && this.udteDataOraInizio_RP.Value == null)
                    {
                        easyStatics.EasyMessageBox("E' necessario definire Data/Ora Inizio.", "Tempi Task Infermieristico", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.udteDataOraInizio_RP.Focus();
                        bOK = false;
                    }
                    if (bOK && this.udteDataOraFine_RP.Value == null)
                    {
                        easyStatics.EasyMessageBox("E' necessario definire Data/Ora Fine.", "Tempi Task Infermieristico", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.udteDataOraFine_RP.Focus();
                        bOK = false;
                    }
                    if (bOK && (DateTime)this.udteDataOraFine_RP.Value <= (DateTime)this.udteDataOraInizio_RP.Value)
                    {
                        easyStatics.EasyMessageBox("Date non coerenti: Data/Ora Inizio maggiore o uguale di Data/Ora Fine.", "Tempi Task Infermieristico", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.udteDataOraFine_RP.Focus();
                        bOK = false;
                    }
                    if (bOK && this.ucSelPeriodicitaGiorni_RP.Value != null && (int)this.ucSelPeriodicitaGiorni_RP.Value == 0
                            && this.ucSelPeriodicitaOre_RP.Value != null && (int)this.ucSelPeriodicitaOre_RP.Value == 0
                            && this.ucSelPeriodicitaMinuti_RP.Value != null && (int)this.ucSelPeriodicitaMinuti_RP.Value == 0
                        )
                    {
                        easyStatics.EasyMessageBox("E' necessario definire la periodicità.", "Tempi Task Infermieristico", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.ucSelPeriodicitaGiorni_RP.Focus();
                        bOK = false;
                    }

                    break;

                case EnumTipoTaskInfermieristicoTempi.PO:
                                        if (bOK && this.udteDataOraInizio_PO.Value == null)
                    {
                        easyStatics.EasyMessageBox("E' necessario definire Data/Ora Inizio.", "Tempi Task Infermieristico", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.udteDataOraInizio_PO.Focus();
                        bOK = false;
                    }
                    if (bOK && this.udteDataOraFine_PO.Value == null)
                    {
                        easyStatics.EasyMessageBox("E' necessario definire Data/Ora Fine.", "Tempi Task Infermieristico", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.udteDataOraFine_PO.Focus();
                        bOK = false;
                    }
                    if (bOK && (DateTime)this.udteDataOraFine_PO.Value <= (DateTime)this.udteDataOraInizio_PO.Value)
                    {
                        easyStatics.EasyMessageBox("Date non coerenti: Data/Ora Inizio maggiore di Data/Ora Fine.", "Tempi Task Infermieristico", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.udteDataOraFine_PO.Focus();
                        bOK = false;
                    }
                    if (this.uceProtocollo_PO.Value == null || this.uceProtocollo_PO.Value.ToString() == string.Empty)
                    {
                        easyStatics.EasyMessageBox("E' necessario definire il protocollo.", "Tempi Task Infermieristico", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uceProtocollo_PO.Focus();
                        bOK = false;
                    }
                    break;

                case EnumTipoTaskInfermieristicoTempi.PG:
                                        if (bOK && this.udteDataOraInizio_PG.Value == null)
                    {
                        easyStatics.EasyMessageBox("E' necessario definire Data/Ora Inizio.", "Tempi Task Infermieristico", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.udteDataOraInizio_PG.Focus();
                        bOK = false;
                    }
                    if (this.uceProtocollo_PG.Value == null || this.uceProtocollo_PG.Value.ToString() == string.Empty)
                    {
                        easyStatics.EasyMessageBox("E' necessario definire il protocollo.", "Tempi Task Infermieristico", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uceProtocollo_PG.Focus();
                        bOK = false;
                    }
                    break;

            }

                        if (bOK && this.ViewController.MovTaskInfermieristico.AnteprimaPeriodicita().Count == 0)
            {
                easyStatics.EasyMessageBox("E' necessario definire Data/Ora Inizio!", "Tempi Task Infermieristico", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                bOK = false;
            }

            return bOK;
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

        private void enableControls(bool vEnable)
        {
            try
            {
                this.PulsanteAvantiAbilitato = vEnable;
                this.PulsanteIndietroAbilitato = vEnable;
                this.ucTopModale.Enabled = vEnable;
                this.ucEasyTabControlOrari.Enabled = vEnable;
            }
            catch
            {
            }
        }

        #endregion

        #region Multi Thread

        private bool GeneraMultiTask()
        {

            bool bReturn = false;

            ClrThreadPool ctp = new ClrThreadPool();
            ctp.SynchronizationContext = SynchronizationContext.Current;
            ctp.ThreadStarted += ctp_ThreadStarted;
            ctp.ThreadCompleted += ctp_ThreadCompleted;
            ctp.ThreadException += ctp_ThreadException;

            try
            {

                this.ImpostaCursore(enum_app_cursors.WaitCursor);

                foreach (UltraGridRow ugr in this.ucEasyGrid.Rows)
                {

                                                                                List<object> oList = new List<object>();
                    oList.Add(ugr.Cells["CodTipo"].Value.ToString());
                    oList.Add(this.ViewController);
                    ctp.QueueWorker(this.ThreadProcNewTask_CLR, oList,
                                    string.Format("{0}|{1:yyyyMMddHHmmss}", ugr.Cells["CodTipo"].Value, ugr.Cells["DataOraInizio"].Value));
                }

                try
                {
                    ctp.WaitAll();
                }
                catch (Exception ex)
                {
                    CoreStatics.ExGest(ref ex, "Genera Multi Task ClrThreadPool", this.Name);
                }

                bReturn = true;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Genera Multi Task", this.Name);
            }
            finally
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }

            return bReturn;

        }

        private void ctp_ThreadException(object sender, ThreadingExceptionEventArgs args)
        {

            throw new Exception(args.ExceptionMessage, args.Exception.InnerException);

        }

        private void ctp_ThreadCompleted(object sender, ThreadingEventArgs args)
        {

        }

        private void ctp_ThreadStarted(object sender, ThreadingEventArgs args)
        {

        }

        private void ThreadProcNewTask_CLR(object parameter)
        {

            try
            {

                string codtipo = ((List<object>)parameter)[0] as string;
                ViewControllerMultiTaskInfermieristico oVc = ((List<object>)parameter)[1] as ViewControllerMultiTaskInfermieristico;

                                MovTaskInfermieristico oMov = new MovTaskInfermieristico(oVc.Trasferimento.CodUA,
                                                                            oVc.Paziente.ID,
                                                                            oVc.Episodio.ID,
                                                                            oVc.Trasferimento.ID,
                                                                            EnumCodSistema.WKI, EnumTipoRegistrazione.M,
                                                                            CoreStatics.CoreApplication.Ambiente);

                oMov.Azione = EnumAzioni.INS;
                oMov.DataEvento = DateTime.Now;

                oMov.CodTipoTaskInfermieristico = codtipo;
                oMov.DescrTipoTaskInfermieristico = GetDescrizioneTipoTask(codtipo, oVc.Trasferimento.CodUA);
                oMov.CodScheda = GetCodSchedaTipoTask(codtipo, oVc.Trasferimento.CodUA);

                Gestore oGestore = GetGestore();
                oGestore.Contesto = new Dictionary<string, object>();
                oGestore.Contesto.Add("Paziente", oVc.Paziente);
                oGestore.Contesto.Add("Episodio", oVc.Episodio);
                oGestore.Contesto.Add("Trasferimento", oVc.Trasferimento);
                oGestore.Contesto.Add("Cartella", oVc.Cartella);

                                oGestore.SchedaXML = oMov.MovScheda.Scheda.StrutturaXML;
                                oGestore.SchedaLayoutsXML = oMov.MovScheda.Scheda.LayoutXML;
                                oGestore.Decodifiche = oMov.MovScheda.Scheda.DizionarioValori();
                                oGestore.SchedaDati = new DcSchedaDati();
                oGestore.NuovaScheda();
                oMov.MovScheda.DatiXML = oGestore.SchedaDatiXML;

                switch ((EnumTipoTaskInfermieristicoTempi)Enum.Parse(typeof(EnumTipoTaskInfermieristicoTempi), oVc.CodTipoProtocollo))
                {

                    case EnumTipoTaskInfermieristicoTempi.SN:                          oMov.DataProgrammata = oVc.MovTaskInfermieristico.DataProgrammata;
                        oMov.PeriodicitaDataFine = oVc.MovTaskInfermieristico.PeriodicitaDataFine;
                        break;

                    case EnumTipoTaskInfermieristicoTempi.RP:                      case EnumTipoTaskInfermieristicoTempi.PO:                      case EnumTipoTaskInfermieristicoTempi.PG:                          break;

                }

                oMov.Salva();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ThreadProcNewTask_CLR", this.Name);
            }

        }

        private Gestore GetGestore()
        {

            Gestore oGestore = new Gestore();

                                                oGestore.Valutatore.Fillers.Add("FillerSistema", new EvaluatorFillerSistema());
            oGestore.Valutatore.Fillers.Add("FillerScci", new EvaluatorFillerScci());
            oGestore.Valutatore.Fillers.Add("FillerScheda", new EvaluatorFillerScheda());
            oGestore.Valutatore.Fillers.Add("FillerAltraScheda", new EvaluatorFillerAltraScheda());
            oGestore.Valutatore.Fillers.Add("FillerDLookUp", new EvaluatorFillerDLookUp());
            oGestore.Valutatore.Fillers.Add("FillerGeneric", new UnicodeSrl.Evaluator.EvaluatorFillerGeneric());

            return oGestore;

        }

        #endregion

        #region Events Form

        private void frmPUMultiTaskInfermieristici_ImmagineClick(object sender, ImmagineTopClickEventArgs e)
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

        private void frmPUMultiTaskInfermieristici_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            try
            {
                if (ControllaValori())
                {
                    MemorizzaValori();
                    if (GeneraMultiTask())
                    {
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                        this.SaveViewController();
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private void frmPUMultiTaskInfermieristici_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.SaveViewController();
            this.Close();
        }

        #endregion

        #region Events

        private void ucEasyGrid_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            int refWidth = (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XLarge) * 3;
                        e.Layout.Bands[0].HeaderVisible = false;
                        foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
            {
                oCol.SortIndicator = SortIndicator.Disabled;
                switch (oCol.Key)
                {

                    case "Descrizione":
                        oCol.Header.Caption = "Tipo Task";
                        oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
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
                        oCol.Hidden = this.ViewController.MovTaskInfermieristico.Durata == 0;
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
                        oCol.Hidden = this.ViewController.MovTaskInfermieristico.CodProtocollo == string.Empty;
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

        #endregion

        #region Events Controlli di Selezione

        private void ucEasyOptionSet_ValueChanged(object sender, EventArgs e)
        {

            if (ucEasyOptionSet.CheckedItem != null)
            {
                SetOptionSet(ucEasyOptionSet.CheckedItem.DataValue.ToString());
                if (!_bruntime)
                {
                    this.ResetOptionSet(ucEasyOptionSet.CheckedItem.DataValue.ToString());
                    this.MemorizzaValori();
                    this.CaricaSomministrazioni();
                }
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

            switch ((EnumTipoTaskInfermieristicoTempi)Enum.Parse(typeof(EnumTipoTaskInfermieristicoTempi), this.ucEasyOptionSet.CheckedItem.DataValue.ToString()))
            {

                case EnumTipoTaskInfermieristicoTempi.SN:
                    break;

                case EnumTipoTaskInfermieristicoTempi.RP:
                                        if (udteDataOraInizio_RP.Value != null && udteDataOraFine_RP.Value != null &&
                        Convert.ToDateTime(udteDataOraInizio_RP.Value.ToString()) > Convert.ToDateTime(udteDataOraFine_RP.Value))
                        this.udteDataOraFine_RP.Value = this.udteDataOraInizio_RP.Value;
                    break;

                case EnumTipoTaskInfermieristicoTempi.PO:
                                        if (udteDataOraInizio_PO.Value != null && udteDataOraFine_PO.Value != null &&
                        Convert.ToDateTime(udteDataOraInizio_PO.Value.ToString()) > Convert.ToDateTime(udteDataOraFine_PO.Value))
                        this.udteDataOraFine_PO.Value = this.udteDataOraInizio_PO.Value;
                    break;

                case EnumTipoTaskInfermieristicoTempi.PG:
                    break;

            }

        }

        private void ucEasyHourRange_ValueChanged(object sender, EventArgs e)
        {
            this.udteDataOraInizio_SN.Value = (DateTime)ucEasyHourRange.Value;
            this.udteDataOraInizio_SN.Value = this.udteDataOraInizio_SN.Value;
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

            switch ((EnumTipoTaskInfermieristicoTempi)Enum.Parse(typeof(EnumTipoTaskInfermieristicoTempi), this.ucEasyOptionSet.CheckedItem.DataValue.ToString()))
            {

                case EnumTipoTaskInfermieristicoTempi.SN:
                    break;

                case EnumTipoTaskInfermieristicoTempi.RP:
                                        if (udteDataOraInizio_RP.Value != null && udteDataOraFine_RP.Value != null &&
                                Convert.ToDateTime(udteDataOraInizio_RP.Value.ToString()) > Convert.ToDateTime(udteDataOraFine_RP.Value))
                        this.udteDataOraInizio_RP.Value = this.udteDataOraFine_RP.Value;
                    break;

                case EnumTipoTaskInfermieristicoTempi.PO:
                                        if (udteDataOraInizio_PO.Value != null && udteDataOraFine_PO.Value != null &&
                                Convert.ToDateTime(udteDataOraInizio_PO.Value.ToString()) > Convert.ToDateTime(udteDataOraFine_PO.Value))
                        this.udteDataOraInizio_PO.Value = this.udteDataOraFine_PO.Value;
                    break;

                case EnumTipoTaskInfermieristicoTempi.PG:
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

                switch ((EnumTipoTaskInfermieristicoTempi)Enum.Parse(typeof(EnumTipoTaskInfermieristicoTempi), this.ucEasyOptionSet.CheckedItem.DataValue.ToString()))
                {

                    case EnumTipoTaskInfermieristicoTempi.SN:
                        break;

                    case EnumTipoTaskInfermieristicoTempi.RP:
                        break;

                    case EnumTipoTaskInfermieristicoTempi.PO:
                        if (this.uceProtocollo_PO.SelectedItem != null && ((DataRowView)this.uceProtocollo_PO.SelectedItem.ListObject).Row["CodTipoProtocollo"].ToString() == EnumTipoProtocollo.DELTA.ToString())
                        {
                            this.udteDataOraFine_PO.Enabled = false;
                            this.udteDataOraFine_PO.Value = null;
                        }
                        else
                        {
                            this.udteDataOraFine_PO.Enabled = true;
                        }
                        this.ViewController.MovTaskInfermieristico.Continuita = GetAttivazioneContinuita_PO();
                        this.ViewController.MovTaskInfermieristico.CodTipoProtocollo = GetTipoProtocollo_PO();
                        break;

                    case EnumTipoTaskInfermieristicoTempi.PG:
                        this.ViewController.MovTaskInfermieristico.Continuita = GetAttivazioneContinuita_PG();
                        this.ViewController.MovTaskInfermieristico.CodTipoProtocollo = GetTipoProtocollo_PG();
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
                switch ((EnumTipoTaskInfermieristicoTempi)Enum.Parse(typeof(EnumTipoTaskInfermieristicoTempi), this.ucEasyOptionSet.CheckedItem.DataValue.ToString()))
                {

                    case EnumTipoTaskInfermieristicoTempi.SN:                          break;

                    case EnumTipoTaskInfermieristicoTempi.RP:                          break;

                    case EnumTipoTaskInfermieristicoTempi.PO:                          this.uceProtocollo_PO.Value = e.Cell.Row.Cells["Codice"].Value;
                        break;

                    case EnumTipoTaskInfermieristicoTempi.PG:                          this.uceProtocollo_PG.Value = e.Cell.Row.Cells["Codice"].Value;
                        break;

                }

                this.ViewController.MovTaskInfermieristico.CodTipoProtocollo = e.Cell.Row.Cells["CodTipoProtocollo"].Text;
                this.UltraPopupControlContainerProtocollo.Close();
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
