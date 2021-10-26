using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnicodeSrl.Scci.Enums;

using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.DatiClinici.DC;
using UnicodeSrl.DatiClinici.Common;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci.PluginClient;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinListView;
using Infragistics.Win.Misc;
using UnicodeSrl.Framework.Data;

namespace UnicodeSrl.ScciCore
{
    public partial class frmPUTaskInfermieristici : frmBaseModale, Interfacce.IViewFormlModal
    {

        public frmPUTaskInfermieristici()
        {
            InitializeComponent();
        }

        #region Declare

        private bool _bruntime = false;
        private ucEasyListBox _ucEasyListBox = null;
        private ucEasyGrid _ucEasyGrid = null;
                private ucSegnalibri _ucSegnalibri = null;

        #endregion

        #region Interface

        public void Carica()
        {
            try
            {
                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_WORKLIST_32);

                this.InizializzaControlli();

                this.CaricaScheda();

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

                                this.ubZoomTipoTaskInfermieristico.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_ZOOM_256);
                this.ubZoomTipoTaskInfermieristico.PercImageFill = 0.75F;

                this.ucDcViewer.VisualizzaTitoloScheda = false;

                                switch (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Azione)
                {
                    case EnumAzioni.INS:
                        this.ubZoomTipoTaskInfermieristico.Visible = true;
                                                                                                                                                                                                                                                break;

                    case EnumAzioni.VIS:
                        this.ubZoomTipoTaskInfermieristico.Visible = false;
                                                                                                                                                                                                                        
                                                                                                this.ucDcViewer.IsEnabled = false;

                        break;
                    case EnumAzioni.MOD:
                        this.ubZoomTipoTaskInfermieristico.Visible = false;
                                                                                                                                                                                                                                                break;
                }

                                                
                this.ucEasyHourRange.RangeOre = ScciCore.ucEasyHourRange.EnumHourRange.Next24;
                this.ucEasyHourRange.DataInizio = DateTime.Now;

                                
                                this.lblZoomTipoTaskInfermieristico.Text = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DescrTipoTaskInfermieristico;

                                                                

                if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.SoloTestata)
                {
                                        this.ucEasyTabControlOrari.Visible = false;

                    this.ucEasyTableLayoutPanel.RowStyles[3].Height = 0F;
                    this.ucEasyTableLayoutPanel.RowStyles[2].Height = 91F;
                }
                else
                {
                                        this.ucEasyTableLayoutPanel.RowStyles[2].Height = 56F;
                    this.ucEasyTableLayoutPanel.RowStyles[3].Height = 35F;

                    this.ucEasyTabControlOrari.Visible = true;

                }

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
                op.Parametro.Add("CodTipoTaskInfermieristico", CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodTipoTaskInfermieristico);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataTable dt = Database.GetDataTableStoredProc("MSP_SelTipoTaskInfermieristiciTempi", spcoll);

                if (dt != null)
                {
                    foreach (DataRow oRow in dt.Rows)
                    {
                        Infragistics.Win.ValueListItem oVal = new Infragistics.Win.ValueListItem(oRow["CodTipoTaskInfermieristicoTempi"].ToString(), oRow["DescTipoTaskInfermieristicoTempi"].ToString());
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

                if (item.DataValue.ToString() == CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodTipoTaskInfermieristicoTempi)
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

            if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata == DateTime.MinValue)
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
                this.udteDataOraInizio_SN.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata;
                this.udteDataOraInizio_RP.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata;
                this.udteDataOraInizio_CN.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata;
                this.udteDataOraInizio_PO.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata;
                this.udteDataOraInizio_POC.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata;
                this.udteDataOraInizio_PG.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata;
                this.udteDataOraFine_RP.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata;
                this.udteDataOraFine_CN.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata;
                this.udteDataOraFine_PO.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata;
                this.udteDataOraFine_POC.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata;
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

            CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodProtocollo = string.Empty;
            CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodTipoProtocollo = string.Empty;
            CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodTipoTaskInfermieristicoTempi = key;
            CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Continuita = false;

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
                if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Azione == EnumAzioni.INS)
                {
                    
                    this.ucEasyOptionSet.CheckedIndex = 0;

                    _bruntime = true;

                    if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata > DateTime.MinValue)
                    {
                        this.udteDataOraInizio_SN.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata;
                        this.udteDataOraInizio_RP.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata;
                        this.udteDataOraInizio_CN.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata;
                        this.udteDataOraInizio_PO.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata;
                        this.udteDataOraInizio_POC.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata;
                        this.udteDataOraInizio_PG.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata;
                    }
                    if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.PeriodicitaDataFine > DateTime.MinValue)
                    {
                        this.udteDataOraFine_RP.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.PeriodicitaDataFine;
                        this.udteDataOraFine_CN.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.PeriodicitaDataFine;
                        this.udteDataOraFine_PO.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.PeriodicitaDataFine;
                        this.udteDataOraFine_POC.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.PeriodicitaDataFine;
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

                    if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata > DateTime.MinValue)
                    {
                        this.udteDataOraInizio_SN.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata;
                        this.udteDataOraInizio_RP.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata;
                        this.udteDataOraInizio_CN.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata;
                        this.udteDataOraInizio_PO.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata;
                        this.udteDataOraInizio_POC.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata;
                        this.udteDataOraInizio_PG.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata;
                    }
                    if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.PeriodicitaDataFine > DateTime.MinValue)
                    {
                        this.udteDataOraFine_RP.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.PeriodicitaDataFine;
                        this.udteDataOraFine_CN.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.PeriodicitaDataFine;
                        this.udteDataOraFine_PO.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.PeriodicitaDataFine;
                        this.udteDataOraFine_POC.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.PeriodicitaDataFine;
                    }

                    this.ucSelPeriodicitaGiorni_RP.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.PeriodicitaGiorni;
                    this.ucSelPeriodicitaOre_RP.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.PeriodicitaOre;
                    this.ucSelPeriodicitaMinuti_RP.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.PeriodicitaMinuti;

                    this.ucSelPeriodicitaGiorni_CN.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.PeriodicitaGiorni;
                    this.ucSelPeriodicitaOre_CN.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.PeriodicitaOre;
                    this.ucSelPeriodicitaMinuti_CN.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.PeriodicitaMinuti;

                                                                                                                                                                                                        
                    this.LoadProtocollo_PO();
                    this.LoadProtocollo_PG();

                    this.uceProtocollo_PO.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodProtocollo;
                    this.uceProtocollo_POC.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodProtocollo;
                    this.uceProtocollo_PG.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodProtocollo;

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
                op.Parametro.Add("CodTipoTaskInfermieristico", CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodTipoTaskInfermieristico);
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
                op.Parametro.Add("CodTipoTaskInfermieristico", CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodTipoTaskInfermieristico);

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

            switch ((EnumTipoTaskInfermieristicoTempi)Enum.Parse(typeof(EnumTipoTaskInfermieristicoTempi), this.ucEasyOptionSet.CheckedItem.DataValue.ToString()))
            {

                case EnumTipoTaskInfermieristicoTempi.SN:                      if (this.udteDataOraInizio_SN.Value != null)
                    {
                        dtI = (DateTime)this.udteDataOraInizio_SN.Value;
                        dtF = (DateTime)this.udteDataOraInizio_SN.Value;
                    }
                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Periodicita = false;
                    break;

                case EnumTipoTaskInfermieristicoTempi.RP:                      if (this.udteDataOraInizio_RP.Value != null && this.udteDataOraFine_RP.Value != null &&
                        this.ucSelPeriodicitaGiorni_RP.Value != null && this.ucSelPeriodicitaOre_RP.Value != null && this.ucSelPeriodicitaMinuti_RP.Value != null)
                    {
                        dtI = (DateTime)this.udteDataOraInizio_RP.Value;
                        dtF = this.udteDataOraFine_RP.Value != null ? (DateTime)this.udteDataOraFine_RP.Value : DateTime.MinValue;
                        CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.PeriodicitaGiorni = (int)this.ucSelPeriodicitaGiorni_RP.Value;
                        CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.PeriodicitaOre = (int)this.ucSelPeriodicitaOre_RP.Value;
                        CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.PeriodicitaMinuti = (int)this.ucSelPeriodicitaMinuti_RP.Value;
                    }
                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Periodicita = true;
                    break;

                case EnumTipoTaskInfermieristicoTempi.PO:                      if (this.udteDataOraInizio_PO.Value != null && this.udteDataOraFine_PO.Value != null &&
                        this.uceProtocollo_PO.SelectedItem != null)
                    {
                        dtI = (DateTime)this.udteDataOraInizio_PO.Value;
                        dtF = this.udteDataOraFine_PO.Value != null ? (DateTime)this.udteDataOraFine_PO.Value : DateTime.MinValue;
                        CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodProtocollo = this.uceProtocollo_PO.SelectedItem != null ? this.uceProtocollo_PO.SelectedItem.DataValue.ToString() : string.Empty;
                    }
                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Periodicita = true;
                    break;

                case EnumTipoTaskInfermieristicoTempi.PG:                      if (this.udteDataOraInizio_PG.Value != null &&
                        this.uceProtocollo_PG.SelectedItem != null)
                    {
                        dtI = (DateTime)this.udteDataOraInizio_PG.Value;
                        dtF = (DateTime)this.udteDataOraInizio_PG.Value;
                        CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodProtocollo = this.uceProtocollo_PG.SelectedItem != null ? this.uceProtocollo_PG.SelectedItem.DataValue.ToString() : string.Empty;
                    }
                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Periodicita = true;
                    break;

            }

            CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata = new DateTime(dtI.Year, dtI.Month, dtI.Day, dtI.Hour, dtI.Minute, 0);
            CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.PeriodicitaDataFine =
                            this.udteDataOraFine_RP.Value != null ? new DateTime(dtF.Year, dtF.Month, dtF.Day, dtF.Hour, dtF.Minute, 0) : DateTime.MinValue;

        }

        private void CaricaSomministrazioni()
        {

            List<IntervalloTempi> tempi = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.AnteprimaPeriodicita();
            this.lblElencoSomministrazioni.Text = "Elenco Task (" + tempi.Count + ")";
            if (tempi.Count > 0)
                this.ucEasyGrid.DataSource = tempi;
            else
                this.ucEasyGrid.DataSource = null;
            this.ucEasyGrid.Refresh();

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
                op.Parametro.Add("CodTipoTaskInfermieristico", CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodTipoTaskInfermieristico);
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

        private void CaricaScheda()
        {
            try
            {

                                                                Gestore oGestore = CoreStatics.GetGestore();

                                oGestore.SchedaXML = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.MovScheda.Scheda.StrutturaXML;

                                oGestore.SchedaLayoutsXML = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.MovScheda.Scheda.LayoutXML;

                                oGestore.Decodifiche = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.MovScheda.Scheda.DizionarioValori();

                                if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.MovScheda.DatiXML == string.Empty)
                {
                    oGestore.SchedaDati = new DcSchedaDati();
                }
                else
                {
                    try
                    {
                        oGestore.SchedaDatiXML = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.MovScheda.DatiXML;
                    }
                    catch (Exception)
                    {
                        oGestore.SchedaDati = new DcSchedaDati();
                    }
                }
                if (oGestore.SchedaDati.Dati.Count == 0) { oGestore.NuovaScheda(); }

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
                ScciCore.CoreStatics.ExGest(ref ex, "CaricaScheda", this.Text);
            }
        }

        public bool Salva()
        {
            bool bReturn = false;
            try
            {

                if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Azione != EnumAzioni.VIS)
                {
                    this.ImpostaCursore(enum_app_cursors.WaitCursor);
                                        enableControls(false);

                    if (ControllaValori())
                    {

                        MemorizzaValori();

                        if (SalvaScheda())
                        {

                                                                                    Risposta oRispostaElaboraPrima = PluginClientStatics.PluginClient(EnumPluginClient.WKI_MODIFICA_PRIMA_PU.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                            if (oRispostaElaboraPrima.Successo)
                            {
                                bReturn = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Salva();
                            }
                        }
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

        private bool SalvaScheda()
        {
            bool bReturn = true;

            try
            {

                CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.MovScheda.DatiXML = this.ucDcViewer.Gestore.SchedaDatiXML;

                if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.MovScheda.DatiObbligatoriMancantiRTF != string.Empty
                && CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.MovScheda.DatiObbligatoriMancantiRTF.Trim() != string.Empty)
                {
                    if (easyStatics.EasyMessageBox(@"Non sono stati compilati alcuni valori obbligatori della scheda!" + Environment.NewLine + @"Vuoi conitinuare col salvataggio?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                        bReturn = false;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return bReturn;
        }

        private bool ControllaValori()
        {
            bool bOK = true;

                        if (bOK && CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodTipoTaskInfermieristico == "")
            {
                easyStatics.EasyMessageBox("Inserire Tipo Task Infermieristico!", "Worklist", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.ubZoomTipoTaskInfermieristico.Focus();
                bOK = false;
            }

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

                        if (bOK && CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.AnteprimaPeriodicita().Count == 0)
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
                this.ucDcViewer.IsEnabled = vEnable;
            }
            catch
            {
            }
        }

        #endregion

        #region Events Form

        private void frmPUTaskInfermieristici_ImmagineClick(object sender, ImmagineTopClickEventArgs e)
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

        private void frmPUTaskInfermieristici_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            try
            {

                if (Salva())
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

        private void frmPUTaskInfermieristici_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void frmPUTaskInfermieristici_Shown(object sender, EventArgs e)
        {
            this.CaricaScheda();
        }

        #endregion

        #region Events

        private void ubZoomTipoTaskInfermieristico_Click(object sender, EventArgs e)
        {
            if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneTipoTaskInfermieristici) == DialogResult.OK)
            {
                this.lblZoomTipoTaskInfermieristico.Text = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DescrTipoTaskInfermieristico;
                this.ImpostaCursore(enum_app_cursors.WaitCursor);
                this.LoadOptionSet();
                this.Aggiorna();
                CaricaScheda();
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }
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
                        oCol.Hidden = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Durata == 0;
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
                        oCol.Hidden = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodProtocollo == string.Empty;
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
                        CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Continuita = GetAttivazioneContinuita_PO();
                        CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodTipoProtocollo = GetTipoProtocollo_PO();
                        break;

                    case EnumTipoTaskInfermieristicoTempi.PG:
                        CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Continuita = GetAttivazioneContinuita_PG();
                        CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodTipoProtocollo = GetTipoProtocollo_PG();
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

        #region Events UserControl

        void ucDcViewer_KeyEvent(System.Windows.Input.Key key)
        {

            try
            {

                if (key == System.Windows.Input.Key.F1)
                {
                    frmPUTaskInfermieristici_PulsanteIndietroClick(null, new PulsanteBottomClickEventArgs(EnumPulsanteBottom.Indietro));
                }
                else if (key == System.Windows.Input.Key.F12)
                {
                    frmPUTaskInfermieristici_PulsanteAvantiClick(null, new PulsanteBottomClickEventArgs(EnumPulsanteBottom.Avanti));
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

                                if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato != null)
                {
                    codua = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodUA;
                }

                                CoreStatics.CoreApplication.MovTestoPredefinitoSelezionato = new MovTestoPredefinito(UnicodeSrl.Scci.Enums.EnumEntita.WKI.ToString(), codua, CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice,
                                                                                                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodTipoTaskInfermieristico, id);
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
                string azione = string.Format("WKI{0}.{1}", CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodTipoTaskInfermieristico, campo[0]);
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

                CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodTipoProtocollo = e.Cell.Row.Cells["CodTipoProtocollo"].Text;
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
