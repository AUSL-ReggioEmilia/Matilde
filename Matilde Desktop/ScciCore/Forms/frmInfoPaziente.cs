using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win.UltraWinGrid;
using UnicodeSrl.Scci.DataContracts;

namespace UnicodeSrl.ScciCore
{
    public partial class frmInfoPaziente : frmBaseModale, Interfacce.IViewFormlModal
    {

        bool _schedecaricate = false;

        public frmInfoPaziente()
        {
            InitializeComponent();
        }

        #region INTERFACCIA

        public void Carica()
        {

                        this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
            this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_SCHEDA_16);



            switch (this.ucInfoSchede.Sezione)
            {
                case ucInfoSchede.enumInfoSezione.infoPaziente:
                    CaricaDatiPaziente();
                    this.ucEasyTabControl.Tabs["tabPAZ"].Selected = true;
                    break;
                case ucInfoSchede.enumInfoSezione.infoEpisodio:
                    CaricaEpisodi();
                    this.ucEasyTabControl.Tabs["tabEPI"].Selected = true;
                    break;
                default:
                    break;
            }

                        _schedecaricate = false;

            this.StartTimer();

            this.ShowDialog();

            this.StopTimer();

        }

        #endregion

        public ucInfoSchede.enumInfoSezione Sezione
        {
            get
            {
                return this.ucInfoSchede.Sezione;
            }
            set
            {
                this.ucInfoSchede.Sezione = value;
                this.ucEasyTabControl.Tabs["tabPAZ"].Text = "Dati Anagrafici Generali";
                this.ucEasyTabControl.Tabs["tabEPI"].Text = "Episodi ed Eventi";
                switch (value)
                {
                    case ucInfoSchede.enumInfoSezione.infoPaziente:
                        this.ucEasyTabControl.Tabs["tabPAZ"].Visible = true;
                        this.ucEasyTabControl.Tabs["tabEPI"].Visible = false;
                        this.ucEasyTabControl.Tabs["tabSCH"].Text = "Dati Anagrafici Specifici";
                        break;
                    case ucInfoSchede.enumInfoSezione.infoEpisodio:
                        this.ucEasyTabControl.Tabs["tabPAZ"].Visible = false;
                        this.ucEasyTabControl.Tabs["tabEPI"].Visible = true;
                        this.ucEasyTabControl.Tabs["tabSCH"].Text = "Dati di Episodio Specifici";
                        break;
                    default:
                        break;
                }
            }
        }

        #region PRIVATE

        private void CaricaDatiPaziente()
        {
            try
            {
                this.ImpostaCursore(Scci.Enums.enum_app_cursors.WaitCursor);

                if (CoreStatics.CoreApplication.Paziente != null)
                {
                                        CoreStatics.CoreApplication.Paziente.AggiornaDatiSAC();


                                        this.txtCognome.Text = CoreStatics.CoreApplication.Paziente.PazienteSac.Cognome;
                    this.txtNome.Text = CoreStatics.CoreApplication.Paziente.PazienteSac.Nome;
                    this.txtCodiceFiscale.Text = CoreStatics.CoreApplication.Paziente.PazienteSac.CodiceFiscale;
                    this.txtDataNasc.Text = (CoreStatics.CoreApplication.Paziente.PazienteSac.DataNascita > DateTime.MinValue ? CoreStatics.CoreApplication.Paziente.PazienteSac.DataNascita.ToString("dd/MM/yyyy") : "");
                    this.txtSesso.Text = CoreStatics.CoreApplication.Paziente.PazienteSac.Sesso;
                    this.txtComuneNascita.Text = CoreStatics.CoreApplication.Paziente.PazienteSac.ComuneNascita;
                    this.txtProvNascita.Text = CoreStatics.CoreApplication.Paziente.PazienteSac.ProvinciaNascita;
                    this.txtNazionalita.Text = CoreStatics.CoreApplication.Paziente.PazienteSac.Nazionalita;
                    this.txtDataDecesso.Text = (CoreStatics.CoreApplication.Paziente.PazienteSac.DataDecesso > DateTime.MinValue ? CoreStatics.CoreApplication.Paziente.PazienteSac.DataDecesso.ToString("dd/MM/yyyy") : "");


                                        this.txtResCAP.Text = CoreStatics.CoreApplication.Paziente.PazienteSac.CAPResidenza;
                    this.txtResComune.Text = CoreStatics.CoreApplication.Paziente.PazienteSac.ComuneResidenza;
                    this.txtResIndirizzo.Text = CoreStatics.CoreApplication.Paziente.PazienteSac.IndirizzoResidenza;
                    this.txtResProv.Text = CoreStatics.CoreApplication.Paziente.PazienteSac.ProvinciaResidenza;


                                        this.txtDomCAP.Text = CoreStatics.CoreApplication.Paziente.PazienteSac.CAPDomicilio;
                    this.txtDomComune.Text = CoreStatics.CoreApplication.Paziente.PazienteSac.ComuneDomicilio;
                    this.txtDomIndirizzo.Text = CoreStatics.CoreApplication.Paziente.PazienteSac.IndirizzoDomicilio;
                    this.txtDomProv.Text = CoreStatics.CoreApplication.Paziente.PazienteSac.ProvinciaDomicilio;

                                        this.txtTel1.Text = CoreStatics.CoreApplication.Paziente.PazienteSacDatiAggiuntivi.Telefono1;
                    this.txtTel2.Text = CoreStatics.CoreApplication.Paziente.PazienteSacDatiAggiuntivi.Telefono2;
                    this.txtTel3.Text = CoreStatics.CoreApplication.Paziente.PazienteSacDatiAggiuntivi.Telefono3;

                                        this.txtMedCodice.Text = CoreStatics.CoreApplication.Paziente.PazienteSacDatiAggiuntivi.CodiceMedicoDiBase.ToString();
                    this.txtMedCF.Text = CoreStatics.CoreApplication.Paziente.PazienteSacDatiAggiuntivi.CodiceFiscaleMedicoDiBase;
                    this.txtMedCognomeNome.Text = CoreStatics.CoreApplication.Paziente.PazienteSacDatiAggiuntivi.CognomeNomeMedicoDiBase;
                    this.txtMedDistretto.Text = CoreStatics.CoreApplication.Paziente.PazienteSacDatiAggiuntivi.DistrettoMedicoDiBase;
                    this.txtMedData.Text = (CoreStatics.CoreApplication.Paziente.PazienteSacDatiAggiuntivi.DataSceltaMedicoDiBase > DateTime.MinValue ? CoreStatics.CoreApplication.Paziente.PazienteSacDatiAggiuntivi.DataSceltaMedicoDiBase.ToString("dd/MM/yyyy") : "");


                                        DataTable oDt = CoreStatics.CreateDataTable<PazienteSacEsenzioni>();
                    CoreStatics.FillDataTable<PazienteSacEsenzioni>(oDt, CoreStatics.CoreApplication.Paziente.PazienteSacDatiAggiuntivi.Esenzioni);
                    this.ucEasyGridEsenz.DataSource = oDt;

                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            finally
            {
                this.ImpostaCursore(Scci.Enums.enum_app_cursors.DefaultCursor);
            }
        }

        private void CaricaEpisodi()
        {
            try
            {
                this.ImpostaCursore(Scci.Enums.enum_app_cursors.WaitCursor);

                SvuotaDatiEpisodio();

                DataTable dsEpisodi = DBUtils.getEpisodiPazienteDatatable(CoreStatics.CoreApplication.Paziente.CodSAC, DateTime.MinValue, DateTime.MinValue);

                this.ucEasyGridEpisodi.DataSource = dsEpisodi;
                this.ucEasyGridEpisodi.Refresh();

                CaricaDatiEpisodio();
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            finally
            {
                this.ImpostaCursore(Scci.Enums.enum_app_cursors.DefaultCursor);
            }
        }

        private void SvuotaDatiEpisodio()
        {
            try
            {
                this.txtEpiAzienda.Text = "";
                this.txtEpiDataFine.Text = "";
                this.txtEpiDataInizio.Text = "";
                this.txtEpiDiagnosiAcc.Text = "";
                this.txtEpiEpisodio.Text = "";
                this.txtEpiICD9.Text = "";
                this.txtEpiMedicoDim.Text = "";
                this.txtEpiMotivoRic.Text = "";
                this.txtEpiNosologico.Text = "";
                this.txtEpiOrigine.Text = "";
                this.txtEpiProvenienza.Text = "";
                this.txtEpiRepartoDim.Text = "";
                this.txtEpiTipoDim.Text = "";
                this.txtEpiTipoRic.Text = "";
                this.txtEpiUltimoEvento.Text = "";

                this.ucEasyGridEventi.DataSource = null;

            }
            catch (Exception)
            {
            }
        }

        private void CaricaDatiEpisodio()
        {
            bool svuotadati = true;
            try
            {
                if (this.ucEasyGridEpisodi.ActiveRow != null && this.ucEasyGridEpisodi.ActiveRow.IsDataRow)
                {
                    this.ImpostaCursore(Scci.Enums.enum_app_cursors.WaitCursor);

                    RicoveroDWH objRicovero = DBUtils.getRicoveroDWH(this.ucEasyGridEpisodi.ActiveRow.Cells["IDRicovero"].Text);

                    if (objRicovero != null && objRicovero.IDRicovero != null && objRicovero.IDRicovero != string.Empty && objRicovero.IDRicovero.Trim() != "")
                    {
                        svuotadati = false;

                                                this.txtEpiAzienda.Text = objRicovero.AziendaErogante;
                        this.txtEpiNosologico.Text = objRicovero.Nosologico;
                        this.txtEpiEpisodio.Text = objRicovero.DescTipoEpisodio;
                        this.txtEpiDataInizio.Text = (objRicovero.DataInizioRicovero > DateTime.MinValue ? objRicovero.DataInizioRicovero.ToString(@"dd/MM/yyyy HH:mm") : "");
                        this.txtEpiDataFine.Text = (objRicovero.DataFineRicovero > DateTime.MinValue ? objRicovero.DataFineRicovero.ToString(@"dd/MM/yyyy HH:mm") : "");
                        this.txtEpiUltimoEvento.Text = objRicovero.UltimoEvento;
                        this.txtEpiDiagnosiAcc.Text = objRicovero.Diagnosi;
                        this.txtEpiICD9.Text = @"<missing>";
                        this.txtEpiOrigine.Text = objRicovero.EpisodioOrigine;
                        this.txtEpiProvenienza.Text = objRicovero.ProvenienzaPaziente;
                        this.txtEpiMotivoRic.Text = objRicovero.MotivoRicovero;
                        this.txtEpiTipoRic.Text = objRicovero.TipoRicovero;
                        this.txtEpiTipoDim.Text = @"<missing>";
                        this.txtEpiMedicoDim.Text = @"<missing>";

                        if (objRicovero.UltimoEvento.ToUpper().IndexOf(@"DIMISS".ToUpper()) >= 0)
                            this.txtEpiRepartoDim.Text = objRicovero.DescRepartoDimissione;
                        else
                            this.txtEpiRepartoDim.Text = "";


                        
                        DataTable dteventi = CoreStatics.CreateDataTable<EventoDWH>();
                        CoreStatics.FillDataTable<EventoDWH>(dteventi, objRicovero.EventiDWH);

                        this.ucEasyGridEventi.DataSource = dteventi;
                        this.ucEasyGridEventi.Refresh();

                    }
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            finally
            {

                if (svuotadati) SvuotaDatiEpisodio();

                this.ImpostaCursore(Scci.Enums.enum_app_cursors.DefaultCursor);
            }
        }

        private void StartTimer()
        {

            try
            {

                if (CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.TimerRefresh != 0)
                {
                    this.timerRefresh.Interval = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.TimerRefresh;
                    this.timerRefresh.Enabled = true;
                    this.timerRefresh.Start();
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }

        }

        private void StopTimer()
        {

            try
            {

                this.timerRefresh.Stop();
                this.timerRefresh.Enabled = false;

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }

        }

        #endregion

        #region EVENTI

        private void ucEasyGridEsenz_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            try
            {
                                foreach (UltraGridColumn gridcol in e.Layout.Bands[0].Columns)
                {
                    switch (gridcol.Key)
                    {
                        case "CodiceEsenzione":
                            gridcol.Header.Caption = "Cod. Esenzione";
                            break;
                        case "TestoEsenzione":
                            gridcol.Header.Caption = "Esenzione";
                            gridcol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                            break;
                        case "DataInizioValidita":
                            gridcol.Header.Caption = "Inizio Validità";
                            gridcol.Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            gridcol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            gridcol.Format = "dd/MM/yyyy";
                            break;
                        case "DataFineValidita":
                            gridcol.Header.Caption = "Fine Validità";
                            gridcol.Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            gridcol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            gridcol.Format = "dd/MM/yyyy";
                            break;
                        case "CodiceDiagnosi":
                            gridcol.Header.Caption = "Cod. Diagnosi";
                            break;
                        case "DecodificaEsenzioneDiagnosi":
                            gridcol.Header.Caption = "Diagnosi";
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void frmInfoPaziente_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void frmInfoPaziente_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void ucEasyTabControl_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
        {
            if (e.Tab.Key == "tabSCH" && !_schedecaricate)
            {
                try
                {
                    this.ucInfoSchede.Carica();
                    _schedecaricate = true;
                }
                catch (Exception)
                {
                }
            }
        }

        private void ucEasyGridEpisodi_AfterRowActivate(object sender, EventArgs e)
        {
            CaricaDatiEpisodio();
        }

        private void ucEasyGridEpisodi_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                foreach (UltraGridColumn ocol in e.Layout.Bands[0].Columns)
                {
                    switch (ocol.Key)
                    {
                        case "Nosologico":
                            ocol.Hidden = false;
                            ocol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                            break;

                        case "AziendaErogante":
                            ocol.Hidden = false;
                            ocol.Header.Caption = "Azienda";
                            ocol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                            break;

                        case "DescTipoEpisodio":
                            ocol.Hidden = false;
                            ocol.Header.Caption = "Episodio";
                            ocol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                            break;

                        case "DataInizioRicovero":
                            ocol.Hidden = false;
                            ocol.Header.Caption = "Data/ora inizio";
                            ocol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            ocol.Format = @"dd/MM/yyyy HH:mm";
                            ocol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                            break;

                        case "Diagnosi":
                            ocol.Hidden = false;
                            break;

                        case "DescRepartoAmmissione":
                            ocol.Hidden = false;
                            ocol.Header.Caption = "Reparto Accett.";
                            ocol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                            break;

                        case "DataFineRicovero":
                            ocol.Hidden = false;
                            ocol.Header.Caption = "Data/ora fine";
                            ocol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            ocol.Format = @"dd/MM/yyyy HH:mm";
                            ocol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                            break;

                        case "DescRepartoDimissione":
                            ocol.Hidden = false;
                            ocol.Header.Caption = "Ultimo Trasferimento";                             break;

                        default:
                            ocol.Hidden = true;
                            break;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void ucEasyGridEpisodi_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            try
            {
                                foreach (UltraGridCell cell in e.Row.Cells)
                {
                    try
                    {
                        if (cell.Text.Trim().ToUpper().IndexOf(@"01/01/0001") == 0)
                        {
                            cell.Value = System.DBNull.Value;
                        }
                    }
                    catch
                    {
                    }
                }

                            }
            catch
            {
            }
        }

        private void ucEasyGridEventi_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                foreach (UltraGridColumn ocol in e.Layout.Bands[0].Columns)
                {
                    switch (ocol.Key)
                    {

                        case "DescTipoEvento":
                            ocol.Hidden = false;
                            ocol.Header.Caption = "Evento";
                            ocol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                            break;

                        case "DataEvento":
                            ocol.Hidden = false;
                            ocol.Header.Caption = "Data";
                            ocol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            ocol.Format = @"dd/MM/yyyy HH:mm";
                            ocol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                            break;

                        case "DescReparto":
                            ocol.Hidden = false;
                            ocol.Header.Caption = "Reparto Ricovero";
                            ocol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                            break;

                        case "DescSettore":
                            ocol.Hidden = false;
                            ocol.Header.Caption = "Settore Ricovero";
                            break;

                        case "DescLetto":
                            ocol.Hidden = false;
                            ocol.Header.Caption = "Letto";
                            break;

                        default:
                            ocol.Hidden = true;
                            break;
                    }
                }

                int ipos = 0;
                if (e.Layout.Bands[0].Columns.Exists("DescTipoEvento"))
                {
                    e.Layout.Bands[0].Columns["DescTipoEvento"].Header.VisiblePosition = ipos;
                    ipos += 1;
                }
                if (e.Layout.Bands[0].Columns.Exists("DataEvento"))
                {
                    e.Layout.Bands[0].Columns["DataEvento"].Header.VisiblePosition = ipos;
                    ipos += 1;
                }
                if (e.Layout.Bands[0].Columns.Exists("DescReparto"))
                {
                    e.Layout.Bands[0].Columns["DescReparto"].Header.VisiblePosition = ipos;
                    ipos += 1;
                }
                if (e.Layout.Bands[0].Columns.Exists("DescSettore"))
                {
                    e.Layout.Bands[0].Columns["DescSettore"].Header.VisiblePosition = ipos;
                    ipos += 1;
                }
                if (e.Layout.Bands[0].Columns.Exists("DescLetto"))
                {
                    e.Layout.Bands[0].Columns["DescLetto"].Header.VisiblePosition = ipos;
                    ipos += 1;
                }
            }
            catch (Exception)
            {
            }
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {

            try
            {

                this.timerRefresh.Stop();
                this.timerRefresh.Enabled = false;

                if (ucEasyTabControl.SelectedTab.Key == "tabSCH" && _schedecaricate == true)
                {
                    this.ucInfoSchede.Aggiorna();
                }

                this.timerRefresh.Enabled = true;
                this.timerRefresh.Start();

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }

        }

        #endregion

    }
}
