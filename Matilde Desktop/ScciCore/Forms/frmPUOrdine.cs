using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Framework.Data;
using Infragistics.Win.UltraWinListView;
using Infragistics.Win.Misc;
using UnicodeSrl.Scci.PluginClient;
using UnicodeSrl.Framework.Threading;
using System.Threading;
using UnicodeSrl.ScciResource;

namespace UnicodeSrl.ScciCore
{
    public partial class frmPUOrdine : frmBaseModale, Interfacce.IViewFormlModal
    {

        #region Declare

        private bool _bruntime = false;

        private bool _percorsoambulatoriale = false;

        private ucEasyListBox _ucEasyListBox = null;

                private ucSegnalibri _ucSegnalibri = null;

        #endregion

        public frmPUOrdine()
        {
            InitializeComponent();
        }

                                private ClrThreadPool LoadThreadPool { get; set; }

                                private ClrThreadPool LoadThreadPoolGriglieRicerche { get; set; }

                                private ClrThreadPool LoadThreadPoolGriglieAnteprima { get; set; }

                                private ClrThreadPool LoadThreadPoolGrigliaDatiAggiuntivi { get; set; }

        #region Interface

        public void Carica()
        {
            try
            {
                                                                this.LoadThreadPool = new ClrThreadPool(context: SynchronizationContext.Current);
                this.LoadThreadPool.ThreadCompleted += LoadThreadPool_ThreadCompleted;
                this.LoadThreadPool.ThreadException += LoadThreadPool_ThreadException;

                this.LoadThreadPoolGriglieRicerche = new ClrThreadPool(context: SynchronizationContext.Current);
                this.LoadThreadPoolGriglieRicerche.ThreadCompleted += LoadThreadPoolGrigliRicerche_ThreadCompleted;
                this.LoadThreadPoolGriglieRicerche.ThreadException += LoadThreadPoolGrigliRicerche_ThreadException;

                if (this.CustomParamaters != null && this.CustomParamaters.GetType() == typeof(Dictionary<string, List<string>>))
                {
                                        this.LoadThreadPool.QueueWorker(this.CaricamentoOrdineAutomatico_MT, null, "CaricamentoOrdineAutomatico_MT");
                }

                                this.ucEasyGridFiltroTipo.DisplayLayout.Appearance.ImageBackground = null;
                this.ucEasyGridFiltroTipo.DisplayLayout.Appearance.ImageBackground = Risorse.GetImageFromResource(Risorse.GC_WAIT_32);
                this.ucEasyGridFiltroTipo.DisplayLayout.Appearance.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Centered;
                this.ucEasyGridFiltroTipo.Refresh();

                this.LoadThreadPoolGriglieRicerche.QueueWorker(CaricaGrigliaRicerche_MT, null, "CaricaGrigliaRicerche_MT");

                                
                                this.ucEasyTableLayoutNomeProfilo.Visible = false;

                
                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_ORDINE_256);

                
                                this.InizializzaTabControl();

                                this.InizializzaUltraGridLayout();

                                this.InizializzaPulsanti();

                                this.InizializzaCombo();

                                this.CaricaOrdine();

                                this.AbilitaControlli(true);

                                if (this.PercorsoAmbulatoriale)
                {
                    this.lblCodUO.Visible = true;
                    this.lblTitoloCodUO.Visible = true;
                    this.ubZoomUO.Visible = true;
                    this.CaricaUO();
                }
                else
                {
                    this.lblCodUO.Visible = false;
                    this.lblTitoloCodUO.Visible = false;
                    this.ubZoomUO.Visible = false;
                }

                                this.LoadThreadPool.WaitAll();

                if (this.FlagCaricaOrdineAuto)
                {
                                        this.CaricaGrigliaPrestazioniSelezionate();

                                        this.CaricaGrigliaDatiAggiuntivi();

                                        this.AbilitaPulsantiSelezione(true);

                                        this.ucEasyGridPrestazioniDX_AfterSelectChange(ucEasyGridPrestazioniDX, new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventArgs(null));
                }               

                                this.ShowDialog();

                


            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "Carica", this.Text);
            }
        }

        #endregion


        #region         Threading 

                                private DataTable dataTableGrigliaRicerche { get; set; }

                                private DataTable dataTableGruppi { get; set; }

                                private bool FlagCaricaOrdineAuto { get; set; }

                                private void CaricaGrigliaRicerche_MT(object parameter)
        {
                        this.dataTableGrigliaRicerche = new DataTable();
            this.dataTableGrigliaRicerche.Columns.Add("TipoDesc", typeof(string));
            this.dataTableGrigliaRicerche.Columns.Add("TipoValore", typeof(string));
            this.dataTableGrigliaRicerche.Columns.Add("CodErogante", typeof(string));
            this.dataTableGrigliaRicerche.Columns.Add("CodAziendaErogante", typeof(string));

                        this.AddGridRicercheDataRow(this.dataTableGrigliaRicerche, MovOrdine.EnumMovOrdineTipoFiltro.Tutti);

                        this.AddGridRicercheDataRow(this.dataTableGrigliaRicerche, MovOrdine.EnumMovOrdineTipoFiltro.Recenti);

                        this.AddGridRicercheDataRow(this.dataTableGrigliaRicerche, MovOrdine.EnumMovOrdineTipoFiltro.Profili);

                        this.AddGridRicercheDataRow(this.dataTableGrigliaRicerche, MovOrdine.EnumMovOrdineTipoFiltro.ProfiliUtente);

                        List<Scci.DataContracts.OESistemaErogante> eroganti = CoreStatics.CoreApplication.MovOrdineSelezionato.CaricaElencoCompletoEroganti();
            foreach (Scci.DataContracts.OESistemaErogante erogante in eroganti)
            {
                string tipoDesc = erogante.Descrizione;
                string codAziendaErogante = "";

                                if (erogante.CodiceAzienda != null && erogante.CodiceAzienda.Trim() != "")
                {
                    tipoDesc += @" - " + erogante.CodiceAzienda;
                    codAziendaErogante = erogante.CodiceAzienda;
                }

                                this.AddGridRicercheDataRow(this.dataTableGrigliaRicerche, MovOrdine.EnumMovOrdineTipoFiltro.Erogante, tipoDesc, erogante.Codice, codAziendaErogante);
            }

        }

                                private void CaricaGrigliaRicerche_SetUI()
        {
            try
            {
                this.ucEasyGridFiltroTipo.DisplayLayout.Appearance.ImageBackground = null;
                                this.ucEasyGridFiltroTipo.DisplayLayout.Bands[0].Columns.ClearUnbound();
                this.ucEasyGridFiltroTipo.DataSource = this.dataTableGrigliaRicerche;
                this.ucEasyGridFiltroTipo.Refresh();

                if (this.dataTableGrigliaRicerche.Rows.Count > 0)
                {
                    this.ucEasyGridFiltroTipo.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.FirstRowInBand);
                    this.ucEasyGridFiltroTipo.ActiveRow = this.ucEasyGridFiltroTipo.Rows[0];
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }
        }

                                        private void AddGridRicercheDataRow(DataTable sourceTable, MovOrdine.EnumMovOrdineTipoFiltro tipoFiltro, string tipoDesc = "", string codErogante = "", string codAziendaErogante = "")
        {
            DataRow row = sourceTable.NewRow();

            if (tipoDesc == "") tipoDesc = tipoFiltro.ToString();

            row["TipoDesc"] = tipoDesc;
            row["TipoValore"] = tipoFiltro;
            row["CodErogante"] = codErogante;
            row["CodAziendaErogante"] = codAziendaErogante;
            sourceTable.Rows.Add(row);

        }

                                private void CaricaGrigliaGruppi_MT(object parameter)
        {
            this.dataTableGruppi = new DataTable();
            this.dataTableGruppi.Columns.Add("ID", typeof(string));
            this.dataTableGruppi.Columns.Add("Descrizione", typeof(string));

                        List<Scci.DataContracts.OEGruppoPrestazione> gruppi = CoreStatics.CoreApplication.MovOrdineSelezionato.CaricaElencoGruppi();
            foreach (Scci.DataContracts.OEGruppoPrestazione gruppo in gruppi)
            {
                DataRow dataRow = this.dataTableGruppi.NewRow();
                dataRow["ID"] = gruppo.ID;
                dataRow["Descrizione"] = gruppo.Descrizione;
                this.dataTableGruppi.Rows.Add(dataRow);
            }

        }

                                private void CaricaGrigliaGruppi_SetUI()
        {
            this.ucEasyGridFiltroGruppi.DisplayLayout.Appearance.ImageBackground = null;
            this.ucEasyGridFiltroGruppi.DisplayLayout.Bands[0].Columns.ClearUnbound();
            this.ucEasyGridFiltroGruppi.DataSource = this.dataTableGruppi;
            this.ucEasyGridFiltroGruppi.Refresh();

            if (this.dataTableGruppi.Rows.Count > 0)
            {
                this.ucEasyGridFiltroGruppi.Selected.Rows.Clear();
                this.ucEasyGridFiltroGruppi.ActiveRow = null;
            }
        }

                                private void CaricamentoOrdineAutomatico_MT(object parameter)
        {

            this.FlagCaricaOrdineAuto = false;

            List<string> lst_prestazioni = ((Dictionary<string, List<string>>)this.CustomParamaters)["Prestazioni"];
            List<string> lst_profili = ((Dictionary<string, List<string>>)this.CustomParamaters)["Profili"];

                        if ((lst_prestazioni.Count) == 0 && (lst_profili.Count == 0)) return;

                        foreach (string prestazione in lst_prestazioni)
            {
                Configurazione oConfigurazione = new Configurazione("<Parametri>" + prestazione + "</Parametri>");

                                DataTable dt = CoreStatics.CoreApplication.MovOrdineSelezionato.CaricaPrestazioniDaSelezionare(MovOrdine.EnumMovOrdineTipoFiltro.Tutti, "", "", oConfigurazione.Parametro("Codice"));
                List<DataRow> query = (from p in dt.AsEnumerable()
                                       where p.Field<string>("Codice") == oConfigurazione.Parametro("Codice") &&
                                              p.Field<string>("CodErogante") == oConfigurazione.Parametro("CodErogante") &&
                                              p.Field<string>("AziErogante") == oConfigurazione.Parametro("AziErogante") &&
                                              p.Field<string>("Tipo") == "Prestazione"
                                       select p).ToList();

                if (query.Count() == 1)
                {
                                        if (CoreStatics.CoreApplication.MovOrdineSelezionato.InserisciPrestazione(CreaPrestazioneDaRigaDataTable(query[0]))) this.FlagCaricaOrdineAuto = true;
                }

            }

                        foreach (string profilo in lst_profili)
            {

                Configurazione oConfigurazione = new Configurazione("<Parametri>" + profilo + "</Parametri>");

                                DataTable dt = CoreStatics.CoreApplication.MovOrdineSelezionato.CaricaPrestazioniDaSelezionare(MovOrdine.EnumMovOrdineTipoFiltro.Tutti, "", "", oConfigurazione.Parametro("Codice"));
                List<DataRow> query = (from p in dt.AsEnumerable()
                                       where p.Field<string>("Codice") == oConfigurazione.Parametro("Codice") &&
                                              (p.Field<string>("Tipo") == "ProfiloScomponibile" || p.Field<string>("Tipo") == "Profilo")
                                       select p).ToList();
                if (query.Count() == 1)
                {
                                        if (CoreStatics.CoreApplication.MovOrdineSelezionato.InserisciPrestazione(CreaPrestazioneDaRigaDataTable(query[0]))) this.FlagCaricaOrdineAuto = true;
                }

            }


        }

                                                private void LoadThreadPool_ThreadException(object sender, ThreadingExceptionEventArgs args)
        {
            UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(args.Exception);
        }

                                                private void LoadThreadPool_ThreadCompleted(object sender, ThreadingEventArgs args)
        {

        }

        #endregion


        #region Public Properties

        public bool PercorsoAmbulatoriale
        {
            get { return _percorsoambulatoriale; }
            set { _percorsoambulatoriale = value; }
        }

        #endregion

        #region Functions

        private void InizializzaTabControl()
        {
            CoreStatics.SetUltraTabControl(this.utcFunzioniOE);
        }

        private void InizializzaUltraGridLayout()
        {
            try
            {

                CoreStatics.SetEasyUltraGridLayout(ref this.ucEasyGridPrestazioniSX);
                CoreStatics.SetEasyUltraGridLayout(ref this.ucEasyGridPrestazioniDX);

                CoreStatics.SetEasyUltraGridLayout(ref ucEasyGridAnteprimaSX);
                CoreStatics.SetEasyUltraGridLayout(ref ucEasyGridAnteprimaDX);
                CoreStatics.SetEasyUltraGridLayout(ref ucEasyGridFiltroTipo);
                CoreStatics.SetEasyUltraGridLayout(ref ucEasyGridAnteprimaDatiAgg);


            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaUltraGridLayout", this.Name);
            }
        }

        private void InizializzaCombo()
        {

            try
            {
                ucboPriorita.ValueList.ValueListItems.Add(OEPrioritaOrdine.NN, CoreStatics.GetEnumDescription(OEPrioritaOrdine.NN));
                ucboPriorita.ValueList.ValueListItems.Add(OEPrioritaOrdine.O, CoreStatics.GetEnumDescription(OEPrioritaOrdine.O));
                ucboPriorita.ValueList.ValueListItems.Add(OEPrioritaOrdine.P, CoreStatics.GetEnumDescription(OEPrioritaOrdine.P));
                ucboPriorita.ValueList.ValueListItems.Add(OEPrioritaOrdine.U, CoreStatics.GetEnumDescription(OEPrioritaOrdine.U));
                ucboPriorita.ValueList.ValueListItems.Add(OEPrioritaOrdine.U2, CoreStatics.GetEnumDescription(OEPrioritaOrdine.U2));
                ucboPriorita.ValueList.ValueListItems.Add(OEPrioritaOrdine.UD, CoreStatics.GetEnumDescription(OEPrioritaOrdine.UD));
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "InizializzaCombo", this.Text);
            }
        }

        private void InizializzaPulsanti()
        {
            try
            {
                                this.ubScomponiProfilo.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_PROFILO_ESPANDI_256);
                this.ubScomponiProfilo.PercImageFill = 0.75F;

                this.ubSalvaProfilo.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_PROFILO_AGGIUNGI_256);
                this.ubSalvaProfilo.PercImageFill = 0.75F;

                this.ubEliminaProfilo.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_PROFILO_CANCELLA_256);
                this.ubEliminaProfilo.PercImageFill = 0.75F;

                                this.ubAnnullaSalvaProfilo.Appearance.Image = UnicodeSrl.ScciCore.Properties.Resources.Cancella_256;
                this.ubAnnullaSalvaProfilo.PercImageFill = 0.75F;

                this.ubConfermaSalvaProfilo.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_SI_256);
                this.ubConfermaSalvaProfilo.PercImageFill = 0.75F;

                this.ubZoomUO.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_ZOOM_256);
                this.ubZoomUO.PercImageFill = 0.75F;

                                this.udteDataOraProgrammazione.ReadOnly = CoreStatics.CoreApplication.MovOrdineSelezionato.Azione == EnumAzioni.VIS;
                this.ucboPriorita.ReadOnly = CoreStatics.CoreApplication.MovOrdineSelezionato.Azione == EnumAzioni.VIS;

                this.ubInserisci.Enabled = CoreStatics.CoreApplication.MovOrdineSelezionato.Azione != EnumAzioni.VIS;
                this.ubInserisciAll.Enabled = CoreStatics.CoreApplication.MovOrdineSelezionato.Azione != EnumAzioni.VIS;
                this.ubCancellaAll.Enabled = CoreStatics.CoreApplication.MovOrdineSelezionato.Azione != EnumAzioni.VIS;
                this.ubCancella.Enabled = CoreStatics.CoreApplication.MovOrdineSelezionato.Azione != EnumAzioni.VIS;

                this.ubZoomUO.Enabled = CoreStatics.CoreApplication.MovOrdineSelezionato.Azione != EnumAzioni.VIS && this.PercorsoAmbulatoriale;

                this.AbilitaPulsantiProfili(true);
                this.AbilitaPulsantiSelezione(true);
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "InizializzaPulsanti", this.Text);
            }
        }

                                [Obsolete("Utilizzare i nuovi metodi in multithreading", true)]
        private void CaricaGrigliaRicerche()
        {
            DataTable odt = new DataTable();
            DataRow orow = null;

            try
            {
                try
                {
                    odt.Columns.Add("TipoDesc", typeof(string));
                    odt.Columns.Add("TipoValore", typeof(string));
                    odt.Columns.Add("CodErogante", typeof(string));
                    odt.Columns.Add("CodAziendaErogante", typeof(string));

                                        orow = odt.NewRow();
                    orow["TipoDesc"] = MovOrdine.EnumMovOrdineTipoFiltro.Tutti.ToString();
                    orow["TipoValore"] = MovOrdine.EnumMovOrdineTipoFiltro.Tutti;
                    orow["CodErogante"] = string.Empty;
                    orow["CodAziendaErogante"] = string.Empty;
                    odt.Rows.Add(orow);

                                        orow = odt.NewRow();
                    orow["TipoDesc"] = MovOrdine.EnumMovOrdineTipoFiltro.Recenti.ToString();
                    orow["TipoValore"] = MovOrdine.EnumMovOrdineTipoFiltro.Recenti;
                    orow["CodErogante"] = string.Empty;
                    orow["CodAziendaErogante"] = string.Empty;
                    odt.Rows.Add(orow);

                                        orow = odt.NewRow();
                    orow["TipoDesc"] = MovOrdine.EnumMovOrdineTipoFiltro.Profili.ToString();
                    orow["TipoValore"] = MovOrdine.EnumMovOrdineTipoFiltro.Profili;
                    orow["CodErogante"] = string.Empty;
                    orow["CodAziendaErogante"] = string.Empty;
                    odt.Rows.Add(orow);

                                        orow = odt.NewRow();
                    orow["TipoDesc"] = MovOrdine.EnumMovOrdineTipoFiltro.ProfiliUtente.ToString();
                    orow["TipoValore"] = MovOrdine.EnumMovOrdineTipoFiltro.ProfiliUtente;
                    orow["CodErogante"] = "";
                    orow["CodAziendaErogante"] = string.Empty;
                    odt.Rows.Add(orow);

                                        List<Scci.DataContracts.OESistemaErogante> eroganti = CoreStatics.CoreApplication.MovOrdineSelezionato.CaricaElencoCompletoEroganti();
                    foreach (Scci.DataContracts.OESistemaErogante erogante in eroganti)
                    {
                        orow = odt.NewRow();
                        orow["TipoDesc"] = erogante.Descrizione;

                                                if (erogante.CodiceAzienda != null && erogante.CodiceAzienda.Trim() != "")
                        {
                            orow["TipoDesc"] += @" - " + erogante.CodiceAzienda;
                            orow["CodAziendaErogante"] = erogante.CodiceAzienda;
                        }
                        else
                        {
                            orow["CodAziendaErogante"] = string.Empty;
                        }


                        orow["TipoValore"] = MovOrdine.EnumMovOrdineTipoFiltro.Erogante;
                        orow["CodErogante"] = erogante.Codice;
                        odt.Rows.Add(orow);
                    }
                }
                catch (Exception ex)
                {
                    UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                    odt = new DataTable();
                }

                this.ucEasyGridFiltroTipo.DisplayLayout.Bands[0].Columns.ClearUnbound();
                this.ucEasyGridFiltroTipo.DataSource = odt;
                this.ucEasyGridFiltroTipo.Refresh();

                if (odt.Rows.Count > 0)
                {
                    this.ucEasyGridFiltroTipo.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.FirstRowInBand);
                    this.ucEasyGridFiltroTipo.ActiveRow = this.ucEasyGridFiltroTipo.Rows[0];
                }

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "CaricaGrigliaRicerche", this.Text);
            }
            finally
            {
                odt = null;
            }

        }

                                [Obsolete("Utilizzare i nuovi metodi in multithreading", true)]
        private void CaricaGrigliaGruppi()
        {
            DataTable odt = new DataTable();
            DataRow orow = null;

            try
            {
                try
                {
                    odt.Columns.Add("ID", typeof(string));
                    odt.Columns.Add("Descrizione", typeof(string));

                                        List<Scci.DataContracts.OEGruppoPrestazione> gruppi = CoreStatics.CoreApplication.MovOrdineSelezionato.CaricaElencoGruppi();
                    foreach (Scci.DataContracts.OEGruppoPrestazione gruppo in gruppi)
                    {
                        orow = odt.NewRow();
                        orow["ID"] = gruppo.ID;
                        orow["Descrizione"] = gruppo.Descrizione;
                        odt.Rows.Add(orow);
                    }
                }
                catch (Exception ex)
                {
                    UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                    odt = new DataTable();
                }

                this.ucEasyGridFiltroGruppi.DisplayLayout.Bands[0].Columns.ClearUnbound();
                this.ucEasyGridFiltroGruppi.DataSource = odt;
                this.ucEasyGridFiltroGruppi.Refresh();

                if (odt.Rows.Count > 0)
                {
                    this.ucEasyGridFiltroGruppi.Selected.Rows.Clear();
                    this.ucEasyGridFiltroGruppi.ActiveRow = null;
                }

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "CaricaGrigliaGruppi", this.Text);
            }
            finally
            {
                odt = null;
            }

        }


        private void CaricaOrdine()
        {
            try
            {
                                if (CoreStatics.CoreApplication.MovOrdineSelezionato.DataProgrammazione != DateTime.MinValue)
                    this.udteDataOraProgrammazione.Value = CoreStatics.CoreApplication.MovOrdineSelezionato.DataProgrammazione;
                else
                    this.udteDataOraProgrammazione.Value = null;
                this.ucboPriorita.Value = CoreStatics.CoreApplication.MovOrdineSelezionato.Priorita;

                                _bruntime = true;
                this.CaricaGrigliaPrestazioniSelezionate();
                _bruntime = false;

                                this.ucEasyGridPrestazioniSX.DisplayLayout.Bands[0].Columns.ClearUnbound();
                this.ucEasyGridPrestazioniSX.Selected.Rows.Clear();
                this.ucEasyGridPrestazioniSX.ActiveRow = null;

                this.ucEasyGridPrestazioniSX.DataSource = null;
                this.ucEasyGridPrestazioniSX.Refresh();

                                this.CaricaGrigliaDatiAggiuntivi();

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "CaricaOrdine", this.Text);
            }
        }

        private void CaricaGrigliaDatiAggiuntivi()
        {
            this.CaricaDatiAggiuntivi();
        }

        private void CaricaGrigliaPrestazioniSelezionate()
        {

                        if (!_bruntime)
                CoreStatics.CoreApplication.MovOrdineSelezionato.AggiornaElencoPrestazioni();

            this.ucEasyGridPrestazioniDX.DisplayLayout.Bands[0].Columns.ClearUnbound();
            this.ucEasyGridPrestazioniDX.DataSource = null;
            this.ucEasyGridPrestazioniDX.DataSource =
                CoreStatics.CoreApplication.MovOrdineSelezionato.CaricaPrestazioniSelezionate();
            this.ucEasyGridPrestazioniDX.Refresh();

            this.ucEasyGridPrestazioniDX.Selected.Rows.Clear();
            this.ucEasyGridPrestazioniDX.ActiveRow = null;

            this.ucEasyGridAnteprimaDX.DataSource = null;
            this.ucEasyGridAnteprimaDX.Refresh();

        }

        private void AbilitaControlli(bool ControlliAbilitati)
        {
            this.udteDataOraProgrammazione.Enabled = ControlliAbilitati;
            this.ucboPriorita.Enabled = ControlliAbilitati;

            this.ubInserisci.Enabled = ControlliAbilitati;
            this.ubInserisciAll.Enabled = ControlliAbilitati;
            this.ubCancellaAll.Enabled = ControlliAbilitati;
            this.ubCancella.Enabled = ControlliAbilitati;
            this.ubZoomUO.Enabled = ControlliAbilitati;

            this.ucEasyGridFiltroTipo.Enabled = ControlliAbilitati;
            this.ucEasyGridPrestazioniDX.Enabled = ControlliAbilitati;
            this.ucEasyGridPrestazioniSX.Enabled = ControlliAbilitati;

            this.utxtRicercaPrestazione.Enabled = ControlliAbilitati;
            this.ubApplicaFiltroPrestazione.Enabled = ControlliAbilitati;
            this.ubApplicaFiltroGruppi.Enabled = ControlliAbilitati;

            this.ucEasyGridAnteprimaSX.Enabled = ControlliAbilitati;
            this.ucEasyGridAnteprimaDX.Enabled = ControlliAbilitati;
            this.ucEasyGridAnteprimaDatiAgg.Enabled = ControlliAbilitati;

            this.PulsanteAvantiAbilitato = ControlliAbilitati;
            this.PulsanteIndietroAbilitato = ControlliAbilitati;

            this.AbilitaPulsantiProfili(ControlliAbilitati);
            this.AbilitaPulsantiSelezione(ControlliAbilitati);

        }

        private void AbilitaPulsantiProfili(bool PulsantiAbilitati)
        {

            if (PulsantiAbilitati)
            {
                this.ubSalvaProfilo.Enabled = CoreStatics.CoreApplication.MovOrdineSelezionato.Azione != EnumAzioni.VIS &&
                    this.ucEasyGridPrestazioniDX.Rows.Count > 0;

                if (ucEasyGridPrestazioniDX.ActiveRow != null)
                {
                    OEPrestazioneTipo tipo = (OEPrestazioneTipo)Enum.Parse(typeof(OEPrestazioneTipo), ucEasyGridPrestazioniDX.ActiveRow.Cells["Tipo"].Value.ToString());

                    this.ubScomponiProfilo.Enabled =
                        CoreStatics.CoreApplication.MovOrdineSelezionato.Azione != EnumAzioni.VIS &&
                        (tipo == OEPrestazioneTipo.ProfiloScomponibile || tipo == OEPrestazioneTipo.ProfiloUtente);

                    this.ubEliminaProfilo.Enabled = CoreStatics.CoreApplication.MovOrdineSelezionato.Azione != EnumAzioni.VIS &&
                        tipo == OEPrestazioneTipo.ProfiloUtente;
                }
                else
                {
                    this.ubScomponiProfilo.Enabled = false;
                    this.ubEliminaProfilo.Enabled = false;
                }
            }
            else
            {
                this.ubScomponiProfilo.Enabled = PulsantiAbilitati;
                this.ubSalvaProfilo.Enabled = PulsantiAbilitati;
                this.ubEliminaProfilo.Enabled = PulsantiAbilitati;
            }

        }

        private void AbilitaPulsantiSelezione(bool PulsantiAbilitati)
        {
            if (PulsantiAbilitati)
            {

                if (ucEasyGridPrestazioniSX.Rows.Count > 0)
                {
                    this.ubInserisci.Enabled = CoreStatics.CoreApplication.MovOrdineSelezionato.Azione != EnumAzioni.VIS && ucEasyGridPrestazioniSX.ActiveRow != null;
                    this.ubInserisciAll.Enabled = CoreStatics.CoreApplication.MovOrdineSelezionato.Azione != EnumAzioni.VIS;
                }
                else
                {
                    this.ubInserisci.Enabled = false;
                    this.ubInserisciAll.Enabled = false;
                }

                if (ucEasyGridPrestazioniDX.Rows.Count > 0)
                {
                    this.ubCancella.Enabled = CoreStatics.CoreApplication.MovOrdineSelezionato.Azione != EnumAzioni.VIS && ucEasyGridPrestazioniDX.ActiveRow != null;
                    this.ubCancellaAll.Enabled = CoreStatics.CoreApplication.MovOrdineSelezionato.Azione != EnumAzioni.VIS;
                }
                else
                {
                    this.ubCancella.Enabled = false;
                    this.ubCancellaAll.Enabled = false;
                }

            }
            else
            {
                this.ubInserisci.Enabled = PulsantiAbilitati;
                this.ubInserisciAll.Enabled = PulsantiAbilitati;
                this.ubCancella.Enabled = PulsantiAbilitati;
                this.ubCancellaAll.Enabled = PulsantiAbilitati;
            }
        }

        private bool ControllaPrestazioniDuplicateDaRigaGriglia(Infragistics.Win.UltraWinGrid.UltraGridRow rigagriglia, ref UnicodeSrl.ScciCore.ucEasyGrid grigliainserimento)
        {
            bool bRet = true;

            try
            {
                foreach (Infragistics.Win.UltraWinGrid.UltraGridRow riga in grigliainserimento.Rows)
                {
                    if (riga.Cells["CodErogante"].Value.ToString() == rigagriglia.Cells["CodErogante"].Value.ToString() &&
                        riga.Cells["AziErogante"].Value.ToString() == rigagriglia.Cells["AziErogante"].Value.ToString() &&
                        riga.Cells["Codice"].Value.ToString() == rigagriglia.Cells["Codice"].Value.ToString())
                    {
                        bRet = false;
                        break;
                    }
                    else
                    {
                        bRet = true;
                    }
                }
            }
            catch
            {
                bRet = false;
            }

            return bRet;
        }

        private bool ControllaPrestazioniDuplicateDaGriglia(ref UnicodeSrl.ScciCore.ucEasyGrid grigliaselezione, ref UnicodeSrl.ScciCore.ucEasyGrid grigliainserimento)
        {
            bool bRet = true;

            try
            {
                foreach (Infragistics.Win.UltraWinGrid.UltraGridRow rigagriglia in grigliaselezione.Rows)
                {
                    bRet = ControllaPrestazioniDuplicateDaRigaGriglia(rigagriglia, ref grigliainserimento);
                    if (!bRet) break;
                }
            }
            catch
            {
                bRet = false;
            }

            return bRet;
        }

        private OEPrestazione CreaPrestazioneDaRigaGriglia(Infragistics.Win.UltraWinGrid.UltraGridRow rigagriglia)
        {
            OEPrestazione oret = null;
            OESistemaErogante erogante = null;
            try
            {
                erogante = new OESistemaErogante(rigagriglia.Cells["CodErogante"].Value.ToString(), "", rigagriglia.Cells["AziErogante"].Value.ToString(), "");

                oret = new OEPrestazione(rigagriglia.Cells["Codice"].Value.ToString(),
                                         rigagriglia.Cells["Descrizione"].Value.ToString(),
                                         (OEPrestazioneTipo)Enum.Parse(typeof(OEPrestazioneTipo),
                                         rigagriglia.Cells["Tipo"].Value.ToString()), erogante);
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "CreaPrestazioneDaRigaGriglia", this.Text);
                oret = null;
            }

            return oret;

        }

        private OEPrestazione CreaPrestazioneDaRigaDataTable(DataRow riga)
        {
            OEPrestazione oret = null;
            OESistemaErogante erogante = null;
            try
            {
                erogante = new OESistemaErogante(riga["CodErogante"].ToString(), "", riga["AziErogante"].ToString(), "");

                oret = new OEPrestazione(riga["Codice"].ToString(),
                                         riga["Descrizione"].ToString(),
                                         (OEPrestazioneTipo)Enum.Parse(typeof(OEPrestazioneTipo),
                                         riga["Tipo"].ToString()), erogante);
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "CreaPrestazioneDaRigaDataTable", this.Text);
                oret = null;
            }

            return oret;

        }

        private List<OEPrestazione> CreaPrestazioniDaGriglia(ref UnicodeSrl.ScciCore.ucEasyGrid griglia)
        {

            List<OEPrestazione> prestazioni = new List<OEPrestazione>();
            OEPrestazione prestazione = null;
            try
            {
                foreach (Infragistics.Win.UltraWinGrid.UltraGridRow rigagriglia in griglia.Rows)
                {
                    prestazione = CreaPrestazioneDaRigaGriglia(rigagriglia);

                    if (prestazione != null)
                        prestazioni.Add(prestazione);

                }
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "CreaPrestazioniDaGriglia", this.Text);
                prestazioni = new List<OEPrestazione>();
            }

            return prestazioni;

        }

        private bool ControllaDati()
        {
            bool bret = true;

            try
            {
                if (bret)
                {
                    if ((OEPrioritaOrdine)Enum.Parse(typeof(OEPrioritaOrdine), this.ucboPriorita.Value.ToString()) == OEPrioritaOrdine.NN)
                    {
                        easyStatics.EasyMessageBox("Selezionare una priorità per l'ordine.", "Salvataggio Ordine", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.ucboPriorita.Focus();
                        bret = false;
                    }
                }

                if (bret)
                {
                    if (this.PercorsoAmbulatoriale && CoreStatics.CoreApplication.MovOrdineSelezionato.CodUO == string.Empty)
                    {
                        easyStatics.EasyMessageBox("Selezionare l'unità operativa per l'ordine.", "Salvataggio Ordine", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.ubZoomUO.Focus();
                        bret = false;
                    }
                }

                if (bret)
                {
                    if (this.udteDataOraProgrammazione.Value == null)
                    {
                        easyStatics.EasyMessageBox("Selezionare una data di programmazione per l'ordine.", "Salvataggio Ordine", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.udteDataOraProgrammazione.Focus();
                        bret = false;
                    }
                }

                if (bret)
                {
                    if (this.udteDataOraProgrammazione.Value != null && ((DateTime)this.udteDataOraProgrammazione.Value) < new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0))
                    {
                        easyStatics.EasyMessageBox("La data di programmazione non può essere nel passato.", "Salvataggio Ordine", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.udteDataOraProgrammazione.Focus();
                        bret = false;
                    }
                }

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ControllaDati", this.Text);
                bret = false;
            }

            return bret;
        }

        private void CaricaUO()
        {

            string scodUO = CoreStatics.CoreApplication.MovOrdineSelezionato.CodUO;

            Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);

            op.Parametro.Add("CodAzi", Database.GetConfigTable(EnumConfigTable.Azienda));
            op.Parametro.Add("CodUO", scodUO);

            SqlParameterExt[] spcoll = new SqlParameterExt[1];

            string xmlParam = XmlProcs.XmlSerializeToString(op);
            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

            DataTable odt = Database.GetDataTableStoredProc("MSP_SelUO", spcoll);

            if (odt != null && odt.Rows.Count > 0)
            {
                CoreStatics.CoreApplication.MovOrdineSelezionato.CodUO = odt.Rows[0]["CodUO"].ToString();
                this.lblCodUO.Text = odt.Rows[0]["Descrizione"].ToString();
            }
            else
            {
                CoreStatics.CoreApplication.MovOrdineSelezionato.CodUO = string.Empty;
                this.lblCodUO.Text = string.Empty;
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

                                        private bool VerificaESalvaDati()
        {
            bool bReturn = false;
            try
            {
                if (ControllaDati())
                {
                    CoreStatics.CoreApplication.MovOrdineSelezionato.Priorita = (OEPrioritaOrdine)Enum.Parse(typeof(OEPrioritaOrdine), this.ucboPriorita.Value.ToString());
                    if (this.udteDataOraProgrammazione.Value != null)
                        CoreStatics.CoreApplication.MovOrdineSelezionato.DataProgrammazione = (DateTime)this.udteDataOraProgrammazione.Value;

                    if (CoreStatics.CoreApplication.MovOrdineSelezionato.SalvaOrdine())
                    {
                        bReturn = true;
                    }
                    else
                        easyStatics.EasyMessageBox("Errore di salvataggio della testata dell'ordine.", "Salvataggio Ordine", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "VerificaESalvaDati", this.Text);
            }
            return bReturn;
        }

                                [Obsolete("Utilizzare i nuovi metodi in multithreading", true)]
        private void CaricamentoOrdineAutomatico()
        {

            bool bAdd = false;

            try
            {

                                                                List<string> lst_prestazioni = ((Dictionary<string, List<string>>)this.CustomParamaters)["Prestazioni"];
                List<string> lst_profili = ((Dictionary<string, List<string>>)this.CustomParamaters)["Profili"];

                if (lst_prestazioni.Count > 0 || lst_profili.Count > 0)
                {

                                                                                foreach (string prestazione in lst_prestazioni)
                    {

                        Configurazione oConfigurazione = new Configurazione("<Parametri>" + prestazione + "</Parametri>");

                                                DataTable dt = CoreStatics.CoreApplication.MovOrdineSelezionato.CaricaPrestazioniDaSelezionare(MovOrdine.EnumMovOrdineTipoFiltro.Tutti, "", "", oConfigurazione.Parametro("Codice"));
                        List<DataRow> query = (from p in dt.AsEnumerable()
                                               where p.Field<string>("Codice") == oConfigurazione.Parametro("Codice") &&
                                                      p.Field<string>("CodErogante") == oConfigurazione.Parametro("CodErogante") &&
                                                      p.Field<string>("AziErogante") == oConfigurazione.Parametro("AziErogante") &&
                                                      p.Field<string>("Tipo") == "Prestazione"
                                               select p).ToList();
                        if (query.Count() == 1)
                        {
                                                                                                                if (CoreStatics.CoreApplication.MovOrdineSelezionato.InserisciPrestazione(CreaPrestazioneDaRigaDataTable(query[0])))
                            {
                                bAdd = true;
                            }
                        }

                    }

                                                                                foreach (string profilo in lst_profili)
                    {

                        Configurazione oConfigurazione = new Configurazione("<Parametri>" + profilo + "</Parametri>");

                                                DataTable dt = CoreStatics.CoreApplication.MovOrdineSelezionato.CaricaPrestazioniDaSelezionare(MovOrdine.EnumMovOrdineTipoFiltro.Tutti, "", "", oConfigurazione.Parametro("Codice"));
                        List<DataRow> query = (from p in dt.AsEnumerable()
                                               where p.Field<string>("Codice") == oConfigurazione.Parametro("Codice") &&
                                                      (p.Field<string>("Tipo") == "ProfiloScomponibile" || p.Field<string>("Tipo") == "Profilo")
                                               select p).ToList();
                        if (query.Count() == 1)
                        {
                                                                                                                if (CoreStatics.CoreApplication.MovOrdineSelezionato.InserisciPrestazione(CreaPrestazioneDaRigaDataTable(query[0])))
                            {
                                bAdd = true;
                            }
                        }

                    }

                    if (bAdd)
                    {
                                                this.CaricaGrigliaPrestazioniSelezionate();

                                                this.CaricaGrigliaDatiAggiuntivi();

                                                this.AbilitaPulsantiSelezione(true);

                                                this.ucEasyGridPrestazioniDX_AfterSelectChange(ucEasyGridPrestazioniDX, new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventArgs(null));
                    }

                }

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "CaricamentoOrdineAutomatico", this.Text);
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

        #region Events      

        #region eventi interazione selezione prestazioni

        private void ubInserisci_Click(object sender, EventArgs e)
        {

            this.ImpostaCursore(enum_app_cursors.WaitCursor);

            if (this.ControllaPrestazioniDuplicateDaRigaGriglia(this.ucEasyGridPrestazioniSX.ActiveRow, ref this.ucEasyGridPrestazioniDX))
            {
                                if (CoreStatics.CoreApplication.MovOrdineSelezionato.InserisciPrestazione(CreaPrestazioneDaRigaGriglia(this.ucEasyGridPrestazioniSX.ActiveRow)))
                {
                                        this.CaricaGrigliaPrestazioniSelezionate();

                                        this.CaricaGrigliaDatiAggiuntivi();

                                        this.AbilitaPulsantiSelezione(true);

                                        CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGridPrestazioniDX, "Codice", ucEasyGridPrestazioniSX.ActiveRow.Cells["Codice"].Value.ToString());
                    this.ucEasyGridPrestazioniDX_AfterSelectChange(ucEasyGridPrestazioniDX, new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventArgs(null));
                }
                else
                {
                    easyStatics.EasyMessageBox("Errore di inserimento della prestazione selezionata.", "Inserimento Prestazione", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                easyStatics.EasyMessageBox("Prestazione selezionata già presente in elenco.", "Inserimento Prestazione", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.ImpostaCursore(enum_app_cursors.DefaultCursor);

        }

        private void ubInserisciAll_Click(object sender, EventArgs e)
        {

            this.ImpostaCursore(enum_app_cursors.WaitCursor);


            if (this.ControllaPrestazioniDuplicateDaGriglia(ref this.ucEasyGridPrestazioniSX, ref this.ucEasyGridPrestazioniDX))
            {
                                if (CoreStatics.CoreApplication.MovOrdineSelezionato.InserisciPrestazioni(CreaPrestazioniDaGriglia(ref this.ucEasyGridPrestazioniSX)))
                {
                                        this.CaricaGrigliaPrestazioniSelezionate();

                                        this.CaricaGrigliaDatiAggiuntivi();

                                        this.AbilitaPulsantiSelezione(true);
                }
                else
                {
                    easyStatics.EasyMessageBox("Errore di inserimento delle prestazioni selezionate.", "Inserimento Prestazioni", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                easyStatics.EasyMessageBox("L'elenco da inserire contiene una o più prestazioni già presenti.", "Inserimento Prestazione", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.ImpostaCursore(enum_app_cursors.DefaultCursor);

        }

        private void ubCancellaAll_Click(object sender, EventArgs e)
        {

            this.ImpostaCursore(enum_app_cursors.WaitCursor);

                        if (CoreStatics.CoreApplication.MovOrdineSelezionato.CancellaPrestazioni(CreaPrestazioniDaGriglia(ref this.ucEasyGridPrestazioniDX)))
            {
                                this.CaricaGrigliaPrestazioniSelezionate();

                                this.CaricaGrigliaDatiAggiuntivi();

                                this.AbilitaPulsantiSelezione(true);
            }
            else
            {
                easyStatics.EasyMessageBox("Errore di cancellazione delle prestazioni selezionate.", "Cancellazione Prestazioni", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.ImpostaCursore(enum_app_cursors.DefaultCursor);

        }

        private void ubCancella_Click(object sender, EventArgs e)
        {

            this.ImpostaCursore(enum_app_cursors.WaitCursor);

                        if (CoreStatics.CoreApplication.MovOrdineSelezionato.CancellaPrestazione(CreaPrestazioneDaRigaGriglia(this.ucEasyGridPrestazioniDX.ActiveRow)))
            {
                                this.CaricaGrigliaPrestazioniSelezionate();

                                this.CaricaGrigliaDatiAggiuntivi();

                                this.AbilitaPulsantiSelezione(true);
            }
            else
            {
                easyStatics.EasyMessageBox("Errore di cancellazione della prestazione selezionata.", "Cancellazione Prestazione", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.ImpostaCursore(enum_app_cursors.DefaultCursor);

        }

        private void ubScomponiProfilo_Click(object sender, EventArgs e)
        {
            Infragistics.Win.UltraWinGrid.UltraGridRow rigaprofilo = null;
            try
            {
                
                                                
                rigaprofilo = this.ucEasyGridPrestazioniDX.ActiveRow;

                this.ImpostaCursore(enum_app_cursors.WaitCursor);

                                if (CoreStatics.CoreApplication.MovOrdineSelezionato.ScomponiProfilo(CreaPrestazioneDaRigaGriglia(rigaprofilo)))
                {
                                        this.CaricaGrigliaPrestazioniSelezionate();

                                        
                                        this.CaricaGrigliaDatiAggiuntivi();

                                        this.AbilitaPulsantiSelezione(true);

                    this.ucEasyTableLayoutNomeProfilo.Visible = false;
                }
                else
                {
                    easyStatics.EasyMessageBox("Errore di scomposizione del profilo selezionata.", "Scomposizione Profilo Scomponibile", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
                            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ubScomponiProfilo_Click", this.Text);
            }
            finally
            {
                rigaprofilo = null;
            }
        }

        private void ubSalvaProfilo_Click(object sender, EventArgs e)
        {
            this.AbilitaControlli(false);
            this.utxtNomeProfilo.Text = string.Empty;
            this.ucEasyTableLayoutNomeProfilo.Visible = true;
        }

        private void ubEliminaProfilo_Click(object sender, EventArgs e)
        {
            try
            {
                if (easyStatics.EasyMessageBox("Confermi l'eliminazione del profilo utente selezionato ?",
                    "Eliminazione Profilo Utente", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    this.ImpostaCursore(enum_app_cursors.WaitCursor);

                                        if (CoreStatics.CoreApplication.MovOrdineSelezionato.CancellaPrestazione(CreaPrestazioneDaRigaGriglia(this.ucEasyGridPrestazioniDX.ActiveRow)))
                    {
                                                CoreStatics.CoreApplication.MovOrdineSelezionato.EliminaProfiloUtente(ucEasyGridPrestazioniDX.ActiveRow.Cells["Codice"].Value.ToString());

                                                this.CaricaGrigliaPrestazioniSelezionate();

                                                this.ubApplicaFiltroPrestazione_Click(this, new System.EventArgs());

                                                this.CaricaGrigliaDatiAggiuntivi();

                                                this.AbilitaPulsantiSelezione(true);

                        this.ucEasyTableLayoutNomeProfilo.Visible = false;

                    }
                    else
                    {
                        easyStatics.EasyMessageBox("Errore di eliminazione del profilo selezionata.", "Eliminazione Profilo Utente", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    this.ImpostaCursore(enum_app_cursors.DefaultCursor);
                }
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ubEliminaProfilo_Click", this.Text);
            }
        }

        private void ubConfermaSalvaProfilo_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.utxtNomeProfilo.Text != string.Empty)
                {
                    CoreStatics.CoreApplication.MovOrdineSelezionato.SalvaProfiloUtente(this.utxtNomeProfilo.Text);
                    this.ucEasyTableLayoutNomeProfilo.Visible = false;
                    this.AbilitaControlli(true);
                }
                else
                    easyStatics.EasyMessageBox("Nome Profilo non inserito", "Inserimento Profilo Utente", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ubConfermaSalvaProfilo_Click", this.Text);
            }
        }

        private void ubAnnullaSalvaProfilo_Click(object sender, EventArgs e)
        {
            try
            {
                this.utxtNomeProfilo.Text = string.Empty;
                this.ucEasyTableLayoutNomeProfilo.Visible = false;
                this.AbilitaControlli(true);
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ubAnnullaSalvaProfilo_Click", this.Text);
            }
        }

        private void ucEasyGridPrestazioniSX_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            this.ubInserisci_Click(ubInserisci, new System.EventArgs());
        }

        private void ucEasyGridPrestazioniDX_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            this.ubCancella_Click(ubCancella, new System.EventArgs());
        }

        #endregion

        private void udteDataOraProgrammazione_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "Orari" && ((ucEasyDateTimeEditor)sender).Value != null)
            {
                CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainer);
                _ucEasyListBox = GetEasyListBoxForPopupControlContainer(sender);
                this.UltraPopupControlContainer.Show((ucEasyDateTimeEditor)sender);
            }
        }

        private void ubDomaniOre8_Click(object sender, EventArgs e)
        {
            DateTime dtTemp = DateTime.Now.AddDays(1);
            this.udteDataOraProgrammazione.Value = new DateTime(dtTemp.Year, dtTemp.Month, dtTemp.Day, 8, 0, 0);
        }

        private void ubAdesso_Click(object sender, EventArgs e)
        {
            this.udteDataOraProgrammazione.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
        }

        private void ucEasyGridFiltroTipo_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Exists("TipoDesc"))
            {
                e.Layout.Bands[0].Columns["TipoDesc"].Hidden = false;
                e.Layout.Bands[0].Columns["TipoDesc"].Header.Caption = "Tipo";
            }
            if (e.Layout.Bands[0].Columns.Exists("TipoValore"))
            {
                e.Layout.Bands[0].Columns["TipoValore"].Hidden = true;
            }
            if (e.Layout.Bands[0].Columns.Exists("CodErogante"))
            {
                e.Layout.Bands[0].Columns["CodErogante"].Hidden = true;
            }
            if (e.Layout.Bands[0].Columns.Exists("CodAziendaErogante"))
            {
                e.Layout.Bands[0].Columns["CodAziendaErogante"].Hidden = true;
            }
        }

        private void ucEasyGridFiltroGruppi_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Exists("Descrizione"))
            {
                e.Layout.Bands[0].Columns["Descrizione"].Hidden = false;
                e.Layout.Bands[0].Columns["Descrizione"].Header.Caption = "Gruppi";
            }
            if (e.Layout.Bands[0].Columns.Exists("ID"))
            {
                e.Layout.Bands[0].Columns["ID"].Hidden = true;
            }
        }

        private void GridsInitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Exists("Descrizione"))
            {
                e.Layout.Bands[0].Columns["Descrizione"].Hidden = false;

                if (((Control)sender).Name == "ucEasyGridPrestazioniSX")
                    e.Layout.Bands[0].Columns["Descrizione"].Header.Caption = "Prestazioni Disponibili";

                if (((Control)sender).Name == "ucEasyGridPrestazioniDX")
                    e.Layout.Bands[0].Columns["Descrizione"].Header.Caption = "Prestazioni Selezionate";
            }
            if (e.Layout.Bands[0].Columns.Exists("CodErogante"))
            {
                e.Layout.Bands[0].Columns["CodErogante"].Hidden = true;
            }
            if (e.Layout.Bands[0].Columns.Exists("DescErogante"))
            {
                e.Layout.Bands[0].Columns["DescErogante"].Hidden = true;
            }
            if (e.Layout.Bands[0].Columns.Exists("AziErogante"))
            {
                e.Layout.Bands[0].Columns["AziErogante"].Hidden = true;
            }
            if (e.Layout.Bands[0].Columns.Exists("DescAziErogante"))
            {
                e.Layout.Bands[0].Columns["DescAziErogante"].Hidden = true;
            }
            if (e.Layout.Bands[0].Columns.Exists("Codice"))
            {
                e.Layout.Bands[0].Columns["Codice"].Hidden = true;
            }
            if (e.Layout.Bands[0].Columns.Exists("Stato"))
            {
                e.Layout.Bands[0].Columns["Stato"].Hidden = true;
            }
            if (e.Layout.Bands[0].Columns.Exists("Tipo"))
            {
                e.Layout.Bands[0].Columns["Tipo"].Hidden = true;
            }
        }

        private void GridsInitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
                        switch ((OEPrestazioneTipo)Enum.Parse(typeof(OEPrestazioneTipo), e.Row.Cells["Tipo"].Value.ToString()))
            {
                case OEPrestazioneTipo.NN:
                case OEPrestazioneTipo.Prestazione:
                    e.Row.Appearance.BackColor = Color.White;
                    break;

                case OEPrestazioneTipo.Profilo:
                    e.Row.Appearance.BackColor = Color.LightPink;
                    break;

                case OEPrestazioneTipo.ProfiloScomponibile:
                    e.Row.Appearance.BackColor = Color.LightGray;
                    break;

                case OEPrestazioneTipo.ProfiloUtente:
                    e.Row.Appearance.BackColor = Color.LightCyan;
                    break;

            }
        }

        private void ucEasyGridPrestazioniSX_AfterSelectChange(object sender, Infragistics.Win.UltraWinGrid.AfterSelectChangeEventArgs e)
        {
            this.CaricaAnteprimaSX();
        }

        private void ucEasyGridPrestazioniDX_AfterSelectChange(object sender, Infragistics.Win.UltraWinGrid.AfterSelectChangeEventArgs e)
        {
            this.CaricaAnteprimaDX();
        }

        private void ucEasyGridAnteprimaDatiAgg_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Exists("Descrizione"))
            {
                e.Layout.Bands[0].Columns["Descrizione"].Hidden = false;
                e.Layout.Bands[0].Columns["Descrizione"].Header.Caption = "Dati Accessori Richiesti:";
            }
        }

        private void utxtRicercaPrestazione_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                                if (e.KeyCode == Keys.Enter) ubApplicaFiltroPrestazione_Click(this.utxtRicercaPrestazione, new EventArgs());
            }
            catch (Exception)
            {
            }
        }

        private void utxtRicercaGruppi_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                                if (e.KeyCode == Keys.Enter) ubApplicaFiltroGruppi_Click(this.utxtRicercaGruppi, new EventArgs());
            }
            catch (Exception)
            {
            }
        }

        private void ucboPriorita_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.ImpostaCursore(enum_app_cursors.WaitCursor);
                CoreStatics.CoreApplication.MovOrdineSelezionato.Priorita = (OEPrioritaOrdine)Enum.Parse(typeof(OEPrioritaOrdine), this.ucboPriorita.Value.ToString());
                this.ucEasyGridFiltroGruppi.DataSource = null;
                this.ucEasyGridFiltroGruppi.DisplayLayout.Appearance.ImageBackground = Risorse.GetImageFromResource(Risorse.GC_WAIT_32);
                this.ucEasyGridFiltroGruppi.DisplayLayout.Appearance.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Centered;

                this.ucEasyGridFiltroGruppi.Refresh();
                this.LoadThreadPoolGriglieRicerche.QueueWorker(CaricaGrigliaGruppi_MT, null, "CaricaGrigliaGruppi_MT");

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ucboPriorita_ValueChanged", this.Text);
            }
            finally
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }
        }

        private void ubApplicaFiltroPrestazione_Click(object sender, EventArgs e)
        {
            try
            {
                this.ImpostaCursore(enum_app_cursors.WaitCursor);

                                if (this.ucEasyGridFiltroTipo.ActiveRow != null)
                {
                    this.ucEasyGridPrestazioniSX.DisplayLayout.Bands[0].Columns.ClearUnbound();
                    this.ucEasyGridPrestazioniSX.DataSource = null;
                    this.ucEasyGridPrestazioniSX.DataSource =
                        CoreStatics.CoreApplication.MovOrdineSelezionato.CaricaPrestazioniDaSelezionare(
                            (MovOrdine.EnumMovOrdineTipoFiltro)Enum.Parse(typeof(MovOrdine.EnumMovOrdineTipoFiltro),
                            this.ucEasyGridFiltroTipo.ActiveRow.Cells["TipoValore"].Value.ToString()),
                            this.ucEasyGridFiltroTipo.ActiveRow.Cells["CodAziendaErogante"].Value.ToString(),
                            this.ucEasyGridFiltroTipo.ActiveRow.Cells["CodErogante"].Value.ToString(),
                            this.utxtRicercaPrestazione.Text);
                }
                else
                {
                    this.ucEasyGridPrestazioniSX.DisplayLayout.Bands[0].Columns.ClearUnbound();
                    this.ucEasyGridPrestazioniSX.DataSource = null;
                    this.ucEasyGridPrestazioniSX.DataSource =
                        CoreStatics.CoreApplication.MovOrdineSelezionato.CaricaPrestazioniDaSelezionare(MovOrdine.EnumMovOrdineTipoFiltro.Tutti,
                                                                                                        "",
                                                                                                        "",
                                                                                                        this.utxtRicercaPrestazione.Text);
                }
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ubApplicaFiltro_Click", this.Text);
                this.ucEasyGridPrestazioniSX.DataSource = null;
            }
            finally
            {
                this.ucEasyGridPrestazioniSX.Refresh();
                this.ucEasyGridPrestazioniSX.Selected.Rows.Clear();
                this.ucEasyGridPrestazioniSX.ActiveRow = null;

                this.ucEasyGridAnteprimaSX.DataSource = null;
                this.ucEasyGridAnteprimaSX.Refresh();

                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
                this.AbilitaPulsantiProfili(true);
                this.AbilitaPulsantiSelezione(true);
            }
        }

        private void ubApplicaFiltroGruppi_Click(object sender, EventArgs e)
        {
            try
            {
                                if (this.ucEasyGridFiltroGruppi.ActiveRow != null)
                {
                    this.ImpostaCursore(enum_app_cursors.WaitCursor);

                    this.ucEasyGridPrestazioniSX.DisplayLayout.Bands[0].Columns.ClearUnbound();
                    this.ucEasyGridPrestazioniSX.DataSource = null;

                    if (this.PercorsoAmbulatoriale)
                    {
                        this.ucEasyGridPrestazioniSX.DataSource =
                            CoreStatics.CoreApplication.MovOrdineSelezionato.CaricaPrestazioniDaSelezionare
                            (
                                "AMB", (OEPrioritaOrdine)Enum.Parse(typeof(OEPrioritaOrdine), this.ucboPriorita.Value.ToString()),
                                CoreStatics.CoreApplication.MovOrdineSelezionato.CodAzienda, CoreStatics.CoreApplication.MovOrdineSelezionato.CodUO,
                                this.ucEasyGridFiltroGruppi.ActiveRow.Cells["ID"].Value.ToString(), this.utxtRicercaGruppi.Text
                            );
                    }
                    else
                    {
                        this.ucEasyGridPrestazioniSX.DataSource =
                            CoreStatics.CoreApplication.MovOrdineSelezionato.CaricaPrestazioniDaSelezionare
                            (
                                CoreStatics.CoreApplication.Episodio.CodTipoEpisodio, (OEPrioritaOrdine)Enum.Parse(typeof(OEPrioritaOrdine), this.ucboPriorita.Value.ToString()),
                                CoreStatics.CoreApplication.MovOrdineSelezionato.CodAzienda, CoreStatics.CoreApplication.MovOrdineSelezionato.CodUO,
                                this.ucEasyGridFiltroGruppi.ActiveRow.Cells["ID"].Value.ToString(), this.utxtRicercaGruppi.Text
                            );
                    }
                }
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ubApplicaFiltro_Click", this.Text);
                this.ucEasyGridPrestazioniSX.DataSource = null;
            }
            finally
            {
                this.ucEasyGridPrestazioniSX.Refresh();
                this.ucEasyGridPrestazioniSX.Selected.Rows.Clear();
                this.ucEasyGridPrestazioniSX.ActiveRow = null;

                this.ucEasyGridAnteprimaSX.DataSource = null;
                this.ucEasyGridAnteprimaSX.Refresh();

                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
                this.AbilitaPulsantiProfili(false);
                this.AbilitaPulsantiSelezione(true);
            }
        }

        private void ubZoomUO_Click(object sender, EventArgs e)
        {
            try
            {
                                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.Ambulatoriale_SelezioneUOOrdini) == DialogResult.OK)
                    CoreStatics.CoreApplication.MovOrdineSelezionato.CodUO = CoreStatics.CoreApplication.CodUOSelezionata;

                this.CaricaUO();

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ubZoomUO_Click", this.Text);
            }
        }

        private void frmPUOrdine_ImmagineClick(object sender, ImmagineTopClickEventArgs e)
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

        private void frmPUOrdine_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            try
            {
                this.CheckStateThread();
                bool bOK = VerificaESalvaDati();
                if (bOK)
                {
                                        CoreStatics.CoreApplication.MovOrdineSelezionato.SkipDatiAggiuntivi = false;

                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ubApplicaFiltro_Click", this.Text);
            }
        }

        private void frmPUOrdine_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.CheckStateThread();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void ubSalvaBozza_Click(object sender, EventArgs e)
        {
            try
            {
                bool bOK = VerificaESalvaDati();
                if (bOK)
                {
                                        CoreStatics.CoreApplication.MovOrdineSelezionato.SkipDatiAggiuntivi = true;

                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ubSalvaBozza_Click", this.Text);
            }
        }

        private void frmPUOrdine_Shown(object sender, EventArgs e)
        {
                        this.utxtRicercaPrestazione.Focus();
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

        #region Nuovi Threading

        private void CheckStateThread()
        {

                        if (this.LoadThreadPoolGriglieRicerche != null && this.LoadThreadPoolGriglieRicerche.ThreadPoolState == enumThreadState.Running)
            {
                this.LoadThreadPoolGriglieRicerche.Abort();
            }

                        if (this.LoadThreadPoolGrigliaDatiAggiuntivi != null && this.LoadThreadPoolGrigliaDatiAggiuntivi.ThreadPoolState == enumThreadState.Running)
            {
                this.LoadThreadPoolGrigliaDatiAggiuntivi.Abort();
            }

        }

        private void usSelezioni_CollapsedChanged(object sender, EventArgs e)
        {

            if (!this.usSelezioni.Collapsed)
            {

                this.ImpostaCursore(enum_app_cursors.WaitCursor);

                                                                this.ucEasyGridAnteprimaDatiAgg.DataSource = null;

                this.ucEasyGridAnteprimaDatiAgg.DisplayLayout.Appearance.ImageBackground = Risorse.GetImageFromResource(Risorse.GC_WAIT_32);
                this.ucEasyGridAnteprimaDatiAgg.DisplayLayout.Appearance.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Centered;

                this.ucEasyGridAnteprimaDatiAgg.Refresh();
                this.LoadThreadPoolGrigliaDatiAggiuntivi = new ClrThreadPool(context: SynchronizationContext.Current);
                this.LoadThreadPoolGrigliaDatiAggiuntivi.ThreadCompleted += LoadThreadPoolGrigliaDatiAggiuntivi_ThreadCompleted;
                this.LoadThreadPoolGrigliaDatiAggiuntivi.ThreadException += LoadThreadPoolGrigliaDatiAggiuntivi_ThreadException;
                this.LoadThreadPoolGrigliaDatiAggiuntivi.QueueWorker(this.CaricaDatiAggiuntivi_MT, null, "CaricaDatiAggiuntivi_MT");

                                                                this.ucEasyGridAnteprimaSX.DataSource = null;
                this.ucEasyGridAnteprimaDX.DataSource = null;
                this.ucEasyGridAnteprimaSX.DisplayLayout.Appearance.ImageBackground = Risorse.GetImageFromResource(Risorse.GC_WAIT_32);
                this.ucEasyGridAnteprimaSX.DisplayLayout.Appearance.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Centered;

                this.ucEasyGridAnteprimaDX.DisplayLayout.Appearance.ImageBackground = Risorse.GetImageFromResource(Risorse.GC_WAIT_32);
                this.ucEasyGridAnteprimaDX.DisplayLayout.Appearance.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Centered;

                this.ucEasyGridAnteprimaSX.Refresh();
                this.ucEasyGridAnteprimaDX.Refresh();
                this.LoadThreadPoolGriglieAnteprima = new ClrThreadPool(context: SynchronizationContext.Current);
                this.LoadThreadPoolGriglieAnteprima.QueueWorker(this.CaricaAnteprimaSX_MT, null, "CaricaAnteprimaSX_MT");
                this.LoadThreadPoolGriglieAnteprima.QueueWorker(this.CaricaAnteprimaDX_MT, null, "CaricaAnteprimaDX_MT");
                this.LoadThreadPoolGriglieAnteprima.WaitAll();

                this.CaricaAnteprimaSX_UI();
                this.CaricaAnteprimaDX_UI();

                this.ImpostaCursore(enum_app_cursors.DefaultCursor);

            }

        }

        private void CaricaAnteprimaSX()
        {
            this.ImpostaCursore(enum_app_cursors.WaitCursor);
                        this.AbilitaPulsantiProfili(true);
            this.AbilitaPulsantiSelezione(true);
            this.ucEasyGridAnteprimaSX.DataSource = null;
            this.ucEasyGridAnteprimaSX.DisplayLayout.Appearance.ImageBackground = Risorse.GetImageFromResource(Risorse.GC_WAIT_32);
            this.ucEasyGridAnteprimaSX.DisplayLayout.Appearance.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Centered;

            this.ucEasyGridAnteprimaSX.Refresh();
            if (!this.usSelezioni.Collapsed)
            {
                this.CaricaAnteprimaSX_MT(null);
                this.CaricaAnteprimaSX_UI();
            }
            this.ImpostaCursore(enum_app_cursors.DefaultCursor);
        }
        private void CaricaAnteprimaSX_MT(object parameter)
        {

            OEPrestazioneTipo tipo;

            try
            {

                if (ucEasyGridPrestazioniSX.ActiveRow != null)
                {
                    tipo = (OEPrestazioneTipo)Enum.Parse(typeof(OEPrestazioneTipo), ucEasyGridPrestazioniSX.ActiveRow.Cells["Tipo"].Value.ToString());

                    if (tipo == OEPrestazioneTipo.ProfiloScomponibile ||
                        tipo == OEPrestazioneTipo.ProfiloUtente ||
                        tipo == OEPrestazioneTipo.Profilo)
                        ucEasyGridAnteprimaSX.DataSource =
                            CoreStatics.CoreApplication.MovOrdineSelezionato.CaricaAnteprimaEsplosioneProfilo
                            (
                                ucEasyGridPrestazioniSX.ActiveRow.Cells["Codice"].Value.ToString(),
                                ucEasyGridPrestazioniSX.ActiveRow.Cells["CodErogante"].Value.ToString(),
                                ucEasyGridPrestazioniSX.ActiveRow.Cells["AziErogante"].Value.ToString(),
                                tipo
                            );
                    else
                        ucEasyGridAnteprimaSX.DataSource = null;
                }
                else
                {
                    ucEasyGridAnteprimaSX.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ucEasyGridPrestazioniDX_AfterSelectChange", this.Text);
                ucEasyGridAnteprimaSX.DataSource = null;
            }

        }
        private void CaricaAnteprimaSX_UI()
        {
            this.ucEasyGridAnteprimaSX.DisplayLayout.Appearance.ImageBackground = null;
            ucEasyGridAnteprimaSX.Refresh();
        }

        private void CaricaAnteprimaDX()
        {
            this.ImpostaCursore(enum_app_cursors.WaitCursor);
                        this.AbilitaPulsantiProfili(true);
            this.AbilitaPulsantiSelezione(true);
            this.ucEasyGridAnteprimaDX.DataSource = null;
            this.ucEasyGridAnteprimaDX.DisplayLayout.Appearance.ImageBackground = Risorse.GetImageFromResource(Risorse.GC_WAIT_32);
            this.ucEasyGridAnteprimaDX.DisplayLayout.Appearance.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Centered;

            this.ucEasyGridAnteprimaDX.Refresh();
            if (!this.usSelezioni.Collapsed)
            {
                this.CaricaAnteprimaDX_MT(null);
                this.CaricaAnteprimaDX_UI();
            }
            this.ImpostaCursore(enum_app_cursors.DefaultCursor);
        }
        private void CaricaAnteprimaDX_MT(object parameter)
        {

            OEPrestazioneTipo tipo;

            try
            {

                if (ucEasyGridPrestazioniDX.ActiveRow != null)
                {
                    tipo = (OEPrestazioneTipo)Enum.Parse(typeof(OEPrestazioneTipo), ucEasyGridPrestazioniDX.ActiveRow.Cells["Tipo"].Value.ToString());

                    if (tipo == OEPrestazioneTipo.ProfiloScomponibile ||
                        tipo == OEPrestazioneTipo.ProfiloUtente ||
                        tipo == OEPrestazioneTipo.Profilo)
                        ucEasyGridAnteprimaDX.DataSource =
                            CoreStatics.CoreApplication.MovOrdineSelezionato.CaricaAnteprimaEsplosioneProfilo
                            (
                                ucEasyGridPrestazioniDX.ActiveRow.Cells["Codice"].Value.ToString(),
                                ucEasyGridPrestazioniDX.ActiveRow.Cells["CodErogante"].Value.ToString(),
                                ucEasyGridPrestazioniDX.ActiveRow.Cells["AziErogante"].Value.ToString(),
                                tipo
                            );
                    else
                        ucEasyGridAnteprimaDX.DataSource = null;
                }
                else
                    ucEasyGridAnteprimaDX.DataSource = null;
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ucEasyGridPrestazioniDX_AfterSelectChange", this.Text);
                ucEasyGridAnteprimaDX.DataSource = null;
            }

        }
        private void CaricaAnteprimaDX_UI()
        {
            this.ucEasyGridAnteprimaDX.DisplayLayout.Appearance.ImageBackground = null;
            ucEasyGridAnteprimaDX.Refresh();
        }

        private void CaricaDatiAggiuntivi()
        {
            this.ImpostaCursore(enum_app_cursors.WaitCursor);
            if (!this.usSelezioni.Collapsed)
            {
                this.CaricaDatiAggiuntivi_MT(null);
                this.CaricaDatiAggiuntivi_UI();
            }
            this.ImpostaCursore(enum_app_cursors.DefaultCursor);
        }
        private void CaricaDatiAggiuntivi_MT(object parameter)
        {
            this.ucEasyGridAnteprimaDatiAgg.DisplayLayout.Bands[0].Columns.ClearUnbound();
            this.ucEasyGridAnteprimaDatiAgg.DataSource = null;
            this.ucEasyGridAnteprimaDatiAgg.DataSource = CoreStatics.CoreApplication.MovOrdineSelezionato.CaricaAnteprimaDatiAggiuntivi();
        }
        private void CaricaDatiAggiuntivi_UI()
        {
            this.ucEasyGridAnteprimaDatiAgg.DisplayLayout.Appearance.ImageBackground = null;
            this.ucEasyGridAnteprimaDatiAgg.Refresh();
        }
        private void LoadThreadPoolGrigliaDatiAggiuntivi_ThreadException(object sender, ThreadingExceptionEventArgs args)
        {
            UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(args.Exception);
        }
        private void LoadThreadPoolGrigliaDatiAggiuntivi_ThreadCompleted(object sender, ThreadingEventArgs args)
        {
            this.CaricaDatiAggiuntivi_UI();
        }

        private void LoadThreadPoolGrigliRicerche_ThreadException(object sender, ThreadingExceptionEventArgs args)
        {
            UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(args.Exception);
        }
        private void LoadThreadPoolGrigliRicerche_ThreadCompleted(object sender, ThreadingEventArgs args)
        {

            if (args.ThreadName == "CaricaGrigliaRicerche_MT")
            {
                                CaricaGrigliaRicerche_SetUI();
            }
            else if (args.ThreadName == "CaricaGrigliaGruppi_MT")
            {
                                CaricaGrigliaGruppi_SetUI();
            }

        }

        #endregion

     
    }
}
