#pragma warning disable 4014

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Globalization;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.ScciResource;
using Infragistics.Win.UltraWinGrid;

using UnicodeSrl.ScciCore.CustomControls.Infra;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.ScciCore.ThreadingObj;
using System.Threading;
using UnicodeSrl.Framework.Threading;
using UnicodeSrl.Scci.Model;
using UnicodeSrl.ScciCore.Common.Extensions;

namespace UnicodeSrl.ScciCore
{
    public partial class ucRicercaPazienti : UserControl, Interfacce.IViewUserControlMiddle
    {

        private UserControl _ucc = null;
        private ucRichTextBox _ucRichTextBox = null;
        private Dictionary<string, byte[]> oIcone = new Dictionary<string, byte[]>();

        private bool _16_9 = true;

        private Color _colorepazienteseguito = CoreStatics.GetColorFromString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ColorePazienteSeguito));

        private bool FlagSkipLoadEvts { get; set; }

        private bool _skip_FiltroStatoTrasferimento_attivo = false;

        public ucRicercaPazienti()
        {
            InitializeComponent();

            this.cmdRefresh.Visible = false;
            this.tlpCmds.SetColumnSpan(this.esScreen, 2);

            this.UltraGridRicerca.Visible = true;
            this.scvGrid.Visible = false;
            this.UltraGridRicerca.Dock = DockStyle.Fill;

            this.pbWait.Visible = false;

            showCommands(false);

            initScreenViews();

            _ucc = (UserControl)this;

            _16_9 = (easyStatics.aspectRatio() >= 1.5F);
            if (_16_9)
                this.ucEasyTreeViewFiltroStatoTrasferimento.TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
            else
                this.ucEasyTreeViewFiltroStatoTrasferimento.TextFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;
            this.pbSettore.Image = Risorse.GetImageFromResource(Risorse.GC_WAIT_256);
            this.tlpSettore.RowStyles[0].Height = 0;
            this.tlpSettore.RowStyles[1].Height = 100;
            this.pbStanza.Image = Risorse.GetImageFromResource(Risorse.GC_WAIT_256);
            this.tlpStanza.RowStyles[0].Height = 0;
            this.tlpStanza.RowStyles[1].Height = 100;

        }


        #region     Prop

        private bool Interrupted
        {
            get
            {
                return this.scvGrid.LoadingAborted;
            }
        }

        private DataTable DataTableEpisodi { get; set; }

        private string UltimaSelId { get; set; }

        #endregion  Prop

        #region     ScreenView Proc

        private void initScreenViews()
        {
            try
            {
                this.esScreen.Enabled = false;
                this.esScreen.Checked = false;
                this.ScreenRow = null;

                using (FwDataConnection conn = new FwDataConnection(Database.ConnectionString))
                {
                    string codRuolo = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice;

                    FwDataBufferedList<T_ScreenRow> screens = conn.MSP_SelScreen(null, codRuolo, en_TipoScreen.EPIGRID);

                    if (screens.Buffer.Count > 0)
                    {
                        pnCmds.Visible = true;
                        this.tlpMainGrid.SetColumnSpan(this.tlpRicerca, 1);

                        this.esScreen.Enabled = true;
                        this.ScreenRow = screens.Buffer.First();

                        tlpCmds.ColumnStyles[2] = new System.Windows.Forms.ColumnStyle(SizeType.Percent, 34f);
                    }
                    else
                    {
                        pnCmds.Visible = false;
                        this.tlpMainGrid.SetColumnSpan(this.tlpRicerca, 2);

                        tlpCmds.ColumnStyles[2] = new System.Windows.Forms.ColumnStyle(SizeType.Percent, 0f);
                    }

                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }

        }

        public T_ScreenRow ScreenRow { get; private set; }

        private bool ScreenViewActive
        {
            get
            {
                return (this.esScreen.Enabled && this.esScreen.Checked);
            }

        }

        private void cb_MSP_CercaEpisodioScreenView(object dataObject)
        {

            if (dataObject == null)
                return;

            try
            {
                this.DataTableEpisodi = (DataTable)dataObject;

                showUI_ScreenView();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "cb_MSP_CercaEpisodioScreenView", this.Name);
            }
            finally
            {

            }

        }

        private void showUI_ScreenView()
        {
            this.cmdRefresh.Visible = true;
            this.tlpCmds.SetColumnSpan(this.esScreen, 1);

            this.UltraGridRicerca.Visible = false;

            this.scvGrid.Visible = true;
            this.scvGrid.Enabled = true;

            this.scvGrid.Dock = DockStyle.Fill;

            ShowUiResized(true);

            if (this.DataTableEpisodi.Columns.Contains("NA") == false) this.DataTableEpisodi.Columns.Add("NA", typeof(Bitmap));
            if (this.DataTableEpisodi.Columns.Contains("NAG") == false) this.DataTableEpisodi.Columns.Add("NAG", typeof(Bitmap));
            if (this.DataTableEpisodi.Columns.Contains("NEC") == false) this.DataTableEpisodi.Columns.Add("NEC", typeof(Bitmap));
            if (this.DataTableEpisodi.Columns.Contains("CIV") == false) this.DataTableEpisodi.Columns.Add("CIV", typeof(Bitmap));
            if (this.DataTableEpisodi.Columns.Contains("SCC") == false) this.DataTableEpisodi.Columns.Add("SCC", typeof(Bitmap));

            SortedColumnsCollection sorted = this.UltraGridRicerca.DisplayLayout.Bands[0].SortedColumns;

            if ((sorted != null) && (sorted.Count > 0))
            {
                string sortedColumn = sorted[0].Key;
                string sortDirection = "ASC";

                if (sorted[0].SortIndicator == SortIndicator.Descending) sortDirection = "DESC";

                DataView dv = this.DataTableEpisodi.DefaultView;
                dv.Sort = sortedColumn + " " + sortDirection;
                DataTable sortedDT = dv.ToTable();

                this.DataTableEpisodi = sortedDT;
            }


            this.UltraGridRicerca.DataSource = this.DataTableEpisodi;

            List<string> exclusions = new List<string>
            {
                "NA",
                "NAG",
                "NEC",
                "CIV",
                "SCC"
            };

            bool beq = this.scvGrid.DataTable.IsEqual(this.DataTableEpisodi, exclusions);

            if (this.Interrupted)
                beq = false;

            if (beq == false)
            {
                string codRuolo = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice;

                showCommands(true);

                if ((this.UltimaSelId == null) && (this.DataTableEpisodi.Rows.Count > 0) && (this.UltraGridRicerca.ActiveRow != null))
                    this.UltimaSelId = this.UltraGridRicerca.ActiveRow.Cells["IDTrasferimento"].Text;

                this.scvGrid.LoadData(this.DataTableEpisodi, codRuolo, this.ScreenRow, this.UltimaSelId);

                showCommands(false);
            }


            this.scvGrid.Enabled = true;

        }



        internal void ShowUiResized(bool maximized = true)
        {
            Form frm = this.FindForm();
            if (frm != null && frm is Interfacce.IViewFormMain)
            {
                (frm as Interfacce.IViewFormMain).ControlloCentraleMassimizzato = maximized;

                bool bexist = CoreStatics.CoreApplication.Navigazione.Maschere.Elementi.Exists(Maschera => Maschera.ID == EnumMaschere.RicercaPazienti);

                if (bexist)
                {
                    int index = CoreStatics.CoreApplication.Navigazione.Maschere.Elementi.FindIndex(Maschera => Maschera.ID == EnumMaschere.RicercaPazienti);

                    CoreStatics.CoreApplication.Navigazione.Maschere.Elementi[index].Massimizzata = maximized;
                }
            }

            this.SuspendLayout();

            if (maximized)
            {
                this.tlpMainGrid.SetColumnSpan(this.pnGrids, 3);
                this.tlpMainGrid.SetRowSpan(this.pnGrids, 3);

                this.tlpFiltri.Visible = false;
                this.ultraDockManager.Visible = false;
                this.tlpPaziente.Visible = false;

            }
            else
            {
                this.tlpMainGrid.SetColumnSpan(this.pnGrids, 2);
                this.tlpMainGrid.SetRowSpan(this.pnGrids, 1);

                this.tlpFiltri.Visible = true;
                this.ultraDockManager.Visible = true;
                this.tlpPaziente.Visible = true;
            }


            this.ResumeLayout();

            Application.DoEvents();

        }

        private void showCommands(bool showStop)
        {
            if (showStop)
            {
                this.tlpRicerca.Enabled = false;
                this.tlpCmds.Enabled = false;
                this.tlpCmds.Visible = false;
                this.cmdAbort.Enabled = true;
                this.cmdAbort.Visible = true;
                this.cmdAbort.Dock = DockStyle.Fill;
            }
            else
            {
                this.tlpRicerca.Enabled = true;
                this.tlpCmds.Enabled = true;
                this.tlpCmds.Visible = true;
                this.cmdAbort.Enabled = false;
                this.cmdAbort.Visible = false;
                this.tlpCmds.Dock = DockStyle.Fill;
            }
        }

        #endregion  ScreenView Proc

        #region Interface

        public void Aggiorna()
        {

            if (this.ubRicerca.Enabled == true)
            {

                try
                {

                    if (CoreStatics.CoreApplication.Sessione.RicercaPazienti != string.Empty)
                    {
                        this.uteRicerca.Text = CoreStatics.CoreApplication.Sessione.RicercaPazienti;

                        if (this.ucEasyTreeViewFiltroStatoTrasferimento.Nodes.Count > 0
&& this.ucEasyTreeViewFiltroStatoTrasferimento.Nodes.Exists(CoreStatics.GC_TUTTI))
                        {
                            this.ucEasyTreeViewFiltroStatoTrasferimento.EventManager.SetEnabled(Infragistics.Win.UltraWinTree.TreeEventIds.AfterCheck, false);
                            this.ucEasyTreeViewFiltroStatoTrasferimento.Nodes[CoreStatics.GC_TUTTI].CheckedState = CheckState.Unchecked;
                            foreach (Infragistics.Win.UltraWinTree.UltraTreeNode oNode in this.ucEasyTreeViewFiltroStatoTrasferimento.Nodes[CoreStatics.GC_TUTTI].Nodes)
                            {
                                if (oNode.Override.NodeStyle == Infragistics.Win.UltraWinTree.NodeStyle.CheckBox)
                                {
                                    oNode.CheckedState = CheckState.Unchecked;
                                }
                            }
                            this.ucEasyTreeViewFiltroStatoTrasferimento.EventManager.SetEnabled(Infragistics.Win.UltraWinTree.TreeEventIds.AfterCheck, true);
                        }
                        else
                        {
                            _skip_FiltroStatoTrasferimento_attivo = true;
                        }

                    }
                    this.AggiornaGriglia();
                    CoreStatics.CoreApplication.Sessione.RicercaPazienti = string.Empty;


                }
                catch (Exception ex)
                {
                    this.AggiornaPaziente();
                    CoreStatics.ExGest(ref ex, "Aggiorna", "ucRicercapazienti");
                }

            }

        }

        public void Carica()
        {

            try
            {

                CoreStatics.SetEasyUltraDockManager(ref this.ultraDockManager);

                this.ucEasyTableLayoutPanelPP.Visible = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.PazientiSeguiti_Visualizza);

                this.CaricaGriglia();

                this.uteRicerca.Focus();

                this.ubRicerca.Enabled = true;

                this.Aggiorna();

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }

        }

        public void Ferma()
        {

            try
            {

                oIcone = new Dictionary<string, byte[]>();

                this.scvGrid.Abort();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Ferma", this.Name);
            }

        }

        #endregion

        #region UltraGrid

        private async void AggiornaGriglia()
        {
            try
            {
                _colorepazienteseguito = CoreStatics.GetColorFromString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ColorePazienteSeguito));

                string xmlParam = createMainGridParams();

                SendOrPostCallback cb = null;

                if (this.esScreen.Checked == true)
                    cb = cb_MSP_CercaEpisodioScreenView;
                else
                {
                    cb = cb_MSP_CercaEpisodioMainGrid;

                    this.pbWait.Dock = DockStyle.Fill;
                    this.pbWait.Visible = true;

                    this.UltraGridRicerca.Dock = DockStyle.None;
                    this.UltraGridRicerca.Visible = false;
                }

                WorkerRunStoredProcedure wk = CreateWorker("MSP_CercaEpisodio", xmlParam, cb);
                await wk.RunWorkerAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        private async void CaricaGriglia()
        {

            try
            {

                ClrThreadPool pool = new ClrThreadPool
                {
                    SynchronizationContext = SynchronizationContext.Current
                };

                this.FlagSkipLoadEvts = true;

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);

                WorkerRunStoredProcedure wk_SelUODaRuolo = CreateWorker("MSP_SelUODaRuolo", op.ToXmlString(), cb_MSP_SelUODaRuolo);

                op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);

                WorkerRunStoredProcedure wk_SelUADaRuolo = CreateWorker("MSP_SelUADaRuolo", op.ToXmlString(), cb_MSP_SelUADaRuolo);

                op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("DatiEstesi", "0");

                WorkerRunStoredProcedure wk_SelTipoEpisodio = CreateWorker("MSP_SelTipoEpisodio", op.ToXmlString(), cb_MSP_SelTipoEpisodio);

                op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("DatiEstesi", "0");

                WorkerRunStoredProcedure wk_SelStatoTrasferimento = CreateWorker("MSP_SelStatoTrasferimento", op.ToXmlString(), cb_MSP_SelStatoTrasferimento);

                op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("DatiEstesi", "0");

                WorkerRunStoredProcedure wk_SelStatoCartella = CreateWorker("MSP_SelStatoCartella", op.ToXmlString(), cb_MSP_SelStatoCartella);
                wk_SelStatoCartella.Worker(CancellationToken.None);

                op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                op.Parametro.Add("CodTipoFiltroSpeciale", EnumTipoFiltroSpeciale.PAZCAR.ToString());

                WorkerRunStoredProcedure wk_SelFiltriSpeciali = CreateWorker("MSP_SelFiltriSpeciali", op.ToXmlString(), cb_MSP_SelFiltriSpeciali);


                pool.QueueWorker(wk_SelUODaRuolo);
                pool.QueueWorker(wk_SelUADaRuolo);
                pool.QueueWorker(wk_SelTipoEpisodio);
                pool.QueueWorker(wk_SelStatoTrasferimento);
                pool.QueueWorker(wk_SelFiltriSpeciali);

                await pool.WaitAllAsync();


                op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                op.Parametro.Add("DatiEstesi", "1");

                WorkerRunStoredProcedure wk_CercaEpisodio = CreateWorker("MSP_CercaEpisodio", op.ToXmlString(), cb_MSP_CercaEpisodio, true);
                wk_CercaEpisodio.RunWorkerAsync();


                this.ucEasyComboEditorFiltroStatoCartella.Value = EnumStatoCartella.AP.ToString(); this.uchkSoloCartelleInVisione.Checked = false;
                this.uchkSoloPazientiSeguiti.Checked = false;

                this.FlagSkipLoadEvts = false;

                ricercaGridPazienti();

                pool.Dispose();
                pool = null;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        private void AggiornaPaziente()
        {

            try
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);
                if (this.UltraGridRicerca.ActiveRow != null)
                {

                    Paziente oPaz = new Paziente("", this.UltraGridRicerca.ActiveRow.Cells["IDEpisodio"].Text);
                    this.ucEasyPictureBoxPaziente.Image = oPaz.Foto;
                    this.ucEasyLabelPaziente.Text = oPaz.Descrizione;
                    oPaz = null;
                    if (oIcone.ContainsKey(Risorse.GC_INRILIEVO_256) == false)
                    {
                        oIcone.Add(Risorse.GC_INRILIEVO_256, CoreStatics.ImageToByte(Risorse.GetImageFromResource(Risorse.GC_INRILIEVO_256)));
                    }
                    this.ucRtfPaziente.Immagine = CoreStatics.ByteToImage(oIcone[Risorse.GC_INRILIEVO_256]);
                    this.ucRtfPaziente.Titolo = "Dati di Rilievo";

                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("IDEpisodio", this.UltraGridRicerca.ActiveRow.Cells["IDEpisodio"].Text);
                    op.Parametro.Add("Storicizzata", "NO");
                    op.Parametro.Add("SoloRTF", "1");
                    op.Parametro.Add("SoloDatiInRilievoRTF", "1");
                    op.ParametroRipetibile.Add("CodEntita", new string[2] { EnumEntita.PAZ.ToString(), EnumEntita.EPI.ToString() });
                    op.TimeStamp.CodEntita = EnumEntita.CAR.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    FwDataParametersList fplist = new FwDataParametersList
                    {
                        { "xParametri", xmlParam, ParameterDirection.Input, DbType.Xml }
                    };

                    this.ucRtfPaziente.ColonnaRTFResize = "DatiRilievoRTF";
                    this.ucRtfPaziente.FattoreRidimensionamentoRTF = 26;
                    this.ucRtfPaziente.Dati = Database.GetDataTableStoredProc("MSP_SelMovSchedaAvanzato", fplist);

                }
                else
                {
                    this.ucEasyPictureBoxPaziente.Image = null;
                    this.ucEasyLabelPaziente.Text = "";
                    this.ucRtfPaziente.Immagine = null;
                    this.ucRtfPaziente.Titolo = "";
                    this.ucRtfPaziente.Dati = null;

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
            }

        }

        private void InitializeRow(UltraGridRow eRow)
        {
            try
            {
                if (eRow.Cells.Exists("ColoreStatoTrasferimento") && eRow.Cells["ColoreStatoTrasferimento"].Text != "")
                {
                    eRow.Appearance.BackColor = CoreStatics.GetColorFromString(eRow.Cells["ColoreStatoTrasferimento"].Text);
                    eRow.Appearance.ForeColor = Color.Black;
                    foreach (UltraGridCell oCell in eRow.Cells)
                    {
                        if (!oCell.Hidden)
                        {
                            oCell.ActiveAppearance.BackColor = eRow.Appearance.BackColor;
                            oCell.ActiveAppearance.ForeColor = Color.Blue;
                            oCell.ActiveAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                        }
                    }
                }

            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region Events

        private void ubRicerca_Click(object sender, EventArgs e)
        {
            this.UltimaSelId = "";

            ricercaGridPazienti();
        }

        private void ubAzzera_Click(object sender, EventArgs e)
        {
            this.CaricaGriglia();
        }

        private void UltraGrid_AfterSelectChange(object sender, Infragistics.Win.UltraWinGrid.AfterSelectChangeEventArgs e)
        {
            this.UltimaSelId = this.UltraGridRicerca.ActiveRow.Cells["IDTrasferimento"].Text;
        }
        private void UltraGrid_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {

                if (e.Layout.Bands[0].Columns.Exists("Codice") == true)
                {
                    e.Layout.Bands[0].Columns["Codice"].Hidden = true;
                }

                if (e.Layout.Bands[0].Columns.Exists("CodUO") == true)
                {
                    e.Layout.Bands[0].Columns["CodUO"].Hidden = true;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        private void UltraGridRicerca_AfterRowActivate(object sender, EventArgs e)
        {
            this.AggiornaPaziente();
        }
        private void UltraGridRicerca_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {

                bool bSwitchGroupHeaders = false;

                UltraGridGroup grpPaziente = null;
                UltraGridGroup grpStruttura = null;

                int refWidth = (int)(easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XLarge) * 2.4);
                if (_16_9)
                {
                    refWidth = (int)(easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XLarge) * 3.0);
                }

                Graphics g = this.CreateGraphics();
                int refBtnWidth = Convert.ToInt32(CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.UltraGridRicerca.DataRowFontRelativeDimension), g.DpiY) * 2.6F);
                g.Dispose();
                g = null;
                e.Layout.Bands[0].RowLayoutStyle = RowLayoutStyle.GroupLayout;

                e.Layout.Bands[0].ColHeadersVisible = !bSwitchGroupHeaders;
                e.Layout.Bands[0].GroupHeadersVisible = bSwitchGroupHeaders;
                if (bSwitchGroupHeaders)
                {
                    for (int c = e.Layout.Bands[0].Groups.Count - 1; c >= 0; c--)
                    {
                        try
                        {
                            UltraGridGroup grp = e.Layout.Bands[0].Groups[c];
                            e.Layout.Bands[0].Groups.RemoveAt(c);
                            grp.Dispose();
                        }
                        catch
                        {
                        }
                    }
                    e.Layout.Bands[0].Groups.Clear();

                    e.Layout.Bands[0].GroupHeaderLines = 2;
                    grpPaziente = e.Layout.Bands[0].Groups.Add(@"grpPaziente", @"Paziente" + Environment.NewLine + @"Indirizzo");
                    grpStruttura = e.Layout.Bands[0].Groups.Add(@"grpStruttura", @"Struttura" + Environment.NewLine + @"UO - Settore");
                }
                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {

                    switch (oCol.Key)
                    {

                        case "Paziente2":
                            oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Medium) * 0.98F;
                            oCol.CellAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                            oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                            oCol.Header.Caption = "Paziente";
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                if (_16_9)
                                    oCol.MaxWidth = Convert.ToInt32(((float)this.UltraGridRicerca.Width - ((float)refWidth * 7.3F) - ((float)refBtnWidth * 5) - 18F) * 70 / 100);
                                else
                                    oCol.MaxWidth = Convert.ToInt32(((float)this.UltraGridRicerca.Width - ((float)refWidth * 7.3F) - ((float)refBtnWidth * 5) - 18F) * 80 / 100);
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
                            if (bSwitchGroupHeaders)
                            {
                                oCol.RowLayoutColumnInfo.ParentGroup = grpPaziente;
                                oCol.RowLayoutColumnInfo.ParentGroup.Header.Appearance.FontData.SizeInPoints = oCol.Header.Appearance.FontData.SizeInPoints;
                            }
                            break;

                        case "Paziente3":
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                            oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                            oCol.Header.Caption = "Sesso, Luogo e Data Nascita";
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                            }
                            catch (Exception)
                            {
                            }
                            oCol.RowLayoutColumnInfo.OriginX = 0;
                            oCol.RowLayoutColumnInfo.OriginY = 1;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 1;
                            if (bSwitchGroupHeaders)
                            {
                                oCol.RowLayoutColumnInfo.ParentGroup = grpPaziente;
                                oCol.RowLayoutColumnInfo.ParentGroup.Header.Appearance.FontData.SizeInPoints = oCol.Header.Appearance.FontData.SizeInPoints;
                            }
                            break;

                        case "DescrUA":
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                            oCol.Header.Caption = "Struttura";
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                oCol.MaxWidth = (int)(refWidth * 2.0);
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
                            if (bSwitchGroupHeaders)
                            {
                                oCol.RowLayoutColumnInfo.ParentGroup = grpStruttura;
                                oCol.RowLayoutColumnInfo.ParentGroup.Header.Appearance.FontData.SizeInPoints = oCol.Header.Appearance.FontData.SizeInPoints;
                            }
                            break;

                        case "UO - Settore":
                            oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            oCol.Hidden = false;
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                            }
                            catch (Exception)
                            {
                            }
                            oCol.RowLayoutColumnInfo.OriginX = 1;
                            oCol.RowLayoutColumnInfo.OriginY = 1;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 1;
                            if (bSwitchGroupHeaders)
                            {
                                oCol.RowLayoutColumnInfo.ParentGroup = grpStruttura;
                                oCol.RowLayoutColumnInfo.ParentGroup.Header.Appearance.FontData.SizeInPoints = oCol.Header.Appearance.FontData.SizeInPoints;
                            }
                            break;



                        case "DataIngressoGriglia":
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            oCol.Header.Caption = @"Data Ingresso";
                            oCol.Format = "dd/MM/yyyy HH:mm";
                            oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                oCol.MaxWidth = (int)(refWidth * 1.4);
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

                        case "DataRicoveroGriglia":
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            oCol.Header.Caption = @"Data Ricovero";
                            oCol.Format = "dd/MM/yyyy HH:mm";
                            oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                oCol.MaxWidth = (int)(refWidth * 1.4);
                                oCol.MinWidth = oCol.MaxWidth;
                                oCol.Width = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }
                            oCol.RowLayoutColumnInfo.OriginX = 2;
                            oCol.RowLayoutColumnInfo.OriginY = 1;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 1;
                            break;

                        case "DescrStatoGriglia":
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                            oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            oCol.Header.Caption = @"Stato";
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                oCol.MaxWidth = (int)(refWidth * 1.4);
                                oCol.MinWidth = oCol.MaxWidth;
                                oCol.Width = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }
                            oCol.RowLayoutColumnInfo.OriginX = 4;
                            oCol.RowLayoutColumnInfo.OriginY = 0;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 2;
                            if (bSwitchGroupHeaders)
                            {
                                oCol.RowLayoutColumnInfo.ParentGroup = e.Layout.Bands[0].Groups.Add(@"grpStato", oCol.Header.Caption);
                                oCol.RowLayoutColumnInfo.ParentGroup.Header.Appearance.FontData.SizeInPoints = oCol.Header.Appearance.FontData.SizeInPoints;
                            }
                            break;

                        case "DescStanzaLetto":
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            if (_16_9)
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Medium) * 0.94F;
                            else
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                            oCol.Header.Caption = @"Letto / Stanza";
                            oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                if (_16_9)
                                    oCol.MaxWidth = Convert.ToInt32(((float)this.UltraGridRicerca.Width - ((float)refWidth * 7.1F) - ((float)refBtnWidth * 4F) - 18F) * 30 / 100);
                                else
                                    oCol.MaxWidth = Convert.ToInt32(((float)this.UltraGridRicerca.Width - ((float)refWidth * 7.1F) - ((float)refBtnWidth * 4F) - 18F) * 20 / 100);
                                oCol.MinWidth = oCol.MaxWidth;
                                oCol.Width = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }
                            oCol.RowLayoutColumnInfo.OriginX = 5;
                            oCol.RowLayoutColumnInfo.OriginY = 0;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 2;
                            if (bSwitchGroupHeaders)
                            {
                                oCol.RowLayoutColumnInfo.ParentGroup = e.Layout.Bands[0].Groups.Add(@"grpStanza", oCol.Header.Caption);
                                oCol.RowLayoutColumnInfo.ParentGroup.Header.Appearance.FontData.SizeInPoints = oCol.Header.Appearance.FontData.SizeInPoints;
                            }
                            break;

                        case "DescEpisodio":
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            oCol.Header.Caption = @"Tipo Episodio / " + Environment.NewLine + @"Nosologico";
                            oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                oCol.MaxWidth = (int)(refWidth * 1.2);
                                oCol.MinWidth = oCol.MaxWidth;
                                oCol.Width = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }
                            oCol.RowLayoutColumnInfo.OriginX = 6;
                            oCol.RowLayoutColumnInfo.OriginY = 0;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 2;
                            if (bSwitchGroupHeaders)
                            {
                                oCol.RowLayoutColumnInfo.ParentGroup = e.Layout.Bands[0].Groups.Add(@"grpEpisodio", oCol.Header.Caption);
                                oCol.RowLayoutColumnInfo.ParentGroup.Header.Appearance.FontData.SizeInPoints = oCol.Header.Appearance.FontData.SizeInPoints;
                            }
                            break;

                        case "DescrCartellaGriglia":
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            oCol.Header.Caption = "Cartella";
                            oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                oCol.MaxWidth = (int)(refWidth * 1.1);
                                oCol.MinWidth = oCol.MaxWidth;
                                oCol.Width = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }
                            oCol.RowLayoutColumnInfo.OriginX = 7;
                            oCol.RowLayoutColumnInfo.OriginY = 0;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 2;
                            if (bSwitchGroupHeaders)
                            {
                                oCol.RowLayoutColumnInfo.ParentGroup = e.Layout.Bands[0].Groups.Add(@"grpNumCart", oCol.Header.Caption);
                                oCol.RowLayoutColumnInfo.ParentGroup.Header.Appearance.FontData.SizeInPoints = oCol.Header.Appearance.FontData.SizeInPoints;
                            }
                            break;




                        case "NA":
                            oCol.Header.Caption = @"";
                            oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                            oCol.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                oCol.LockedWidth = true;
                                oCol.MaxWidth = refBtnWidth;
                                oCol.Width = oCol.MaxWidth;
                                oCol.MinWidth = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }
                            oCol.RowLayoutColumnInfo.OriginX = 8;
                            oCol.RowLayoutColumnInfo.OriginY = 0;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 2;
                            if (bSwitchGroupHeaders)
                            {
                                oCol.RowLayoutColumnInfo.ParentGroup = e.Layout.Bands[0].Groups.Add(@"grpNA", oCol.Header.Caption);
                                oCol.RowLayoutColumnInfo.ParentGroup.Header.Appearance.FontData.SizeInPoints = oCol.Header.Appearance.FontData.SizeInPoints;
                            }
                            break;

                        case "NAG":
                            oCol.Header.Caption = @"";
                            oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                            oCol.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                oCol.LockedWidth = true;
                                oCol.MaxWidth = refBtnWidth;
                                oCol.Width = oCol.MaxWidth;
                                oCol.MinWidth = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }
                            oCol.RowLayoutColumnInfo.OriginX = 9;
                            oCol.RowLayoutColumnInfo.OriginY = 0;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 2;
                            if (bSwitchGroupHeaders) oCol.RowLayoutColumnInfo.ParentGroup = e.Layout.Bands[0].Groups.Add(@"grpNAG", oCol.Header.Caption);
                            break;

                        case "NEC":
                            oCol.Header.Caption = @"";
                            oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                            oCol.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                oCol.LockedWidth = true;
                                oCol.MaxWidth = refBtnWidth;
                                oCol.Width = oCol.MaxWidth;
                                oCol.MinWidth = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }
                            oCol.RowLayoutColumnInfo.OriginX = 10;
                            oCol.RowLayoutColumnInfo.OriginY = 0;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 2;
                            if (bSwitchGroupHeaders) oCol.RowLayoutColumnInfo.ParentGroup = e.Layout.Bands[0].Groups.Add(@"grpNEC", oCol.Header.Caption);
                            break;

                        case "CIV":
                            oCol.Header.Caption = @"";
                            oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                            oCol.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                oCol.LockedWidth = true;
                                oCol.MaxWidth = refBtnWidth;
                                oCol.Width = oCol.MaxWidth;
                                oCol.MinWidth = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }
                            oCol.RowLayoutColumnInfo.OriginX = 11;
                            oCol.RowLayoutColumnInfo.OriginY = 0;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 2;
                            if (bSwitchGroupHeaders) oCol.RowLayoutColumnInfo.ParentGroup = e.Layout.Bands[0].Groups.Add(@"grpCIV", oCol.Header.Caption);
                            break;

                        case "SCC":
                            oCol.Header.Caption = @"";
                            oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                            oCol.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                oCol.LockedWidth = true;
                                oCol.MaxWidth = refBtnWidth;
                                oCol.Width = oCol.MaxWidth;
                                oCol.MinWidth = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }
                            oCol.RowLayoutColumnInfo.OriginX = 12;
                            oCol.RowLayoutColumnInfo.OriginY = 0;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 2;
                            if (bSwitchGroupHeaders) oCol.RowLayoutColumnInfo.ParentGroup = e.Layout.Bands[0].Groups.Add(@"grpSCC", oCol.Header.Caption);
                            break;

                        default:
                            oCol.Hidden = true;
                            break;

                    }

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }
        private void UltraGridRicerca_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {

            try
            {

                if (e.Row.Cells.Exists("NumAllergie") == true)
                {
                    if ((int)e.Row.Cells["NumAllergie"].Value != 0)
                    {
                        if (oIcone.ContainsKey(Risorse.GC_ALERTALLERGIA_16) == false)
                        {
                            oIcone.Add(Risorse.GC_ALERTALLERGIA_16, CoreStatics.ImageToByte(Risorse.GetImageFromResource(Risorse.GC_ALERTALLERGIA_16)));
                        }
                        e.Row.Cells["NA"].Value = oIcone[Risorse.GC_ALERTALLERGIA_16];
                    }
                }


                if (e.Row.Cells.Exists("NumAlertGenerici") == true)
                {
                    if ((int)e.Row.Cells["NumAlertGenerici"].Value != 0)
                    {
                        if (oIcone.ContainsKey(Risorse.GC_ALERTGENERICO_16) == false)
                        {
                            oIcone.Add(Risorse.GC_ALERTGENERICO_16, CoreStatics.ImageToByte(Risorse.GetImageFromResource(Risorse.GC_ALERTGENERICO_16)));
                        }
                        e.Row.Cells["NAG"].Value = oIcone[Risorse.GC_ALERTGENERICO_16];
                    }
                }

                if (e.Row.Cells.Exists("NumEvidenzaClinica") == true)
                {
                    if ((int)e.Row.Cells["NumEvidenzaClinica"].Value != 0)
                    {
                        if (oIcone.ContainsKey(Risorse.GC_EVIDENZACLINICA_16) == false)
                        {
                            oIcone.Add(Risorse.GC_EVIDENZACLINICA_16, CoreStatics.ImageToByte(Risorse.GetImageFromResource(Risorse.GC_EVIDENZACLINICA_16)));
                        }
                        e.Row.Cells["NEC"].Value = oIcone[Risorse.GC_EVIDENZACLINICA_16];
                    }
                }

                if (e.Row.Cells.Exists("FlagCartellaInVisione") == true && e.Row.Cells.Exists("FlagHoDatoCartellaInVisione") == true)
                {
                    if ((int)e.Row.Cells["FlagCartellaInVisione"].Value != 0)
                    {
                        if (oIcone.ContainsKey(Risorse.GC_OCCHIOAPERTO_16) == false)
                        {
                            oIcone.Add(Risorse.GC_OCCHIOAPERTO_16, CoreStatics.ImageToByte(Risorse.GetImageFromResource(Risorse.GC_OCCHIOAPERTO_16)));
                        }
                        e.Row.Cells["CIV"].Value = oIcone[Risorse.GC_OCCHIOAPERTO_16];
                    }
                    else
                    {
                        if ((int)e.Row.Cells["FlagHoDatoCartellaInVisione"].Value != 0)
                        {
                            if (oIcone.ContainsKey(Risorse.GC_OCCHIOAPERTOFRECCIA_16) == false)
                            {
                                oIcone.Add(Risorse.GC_OCCHIOAPERTOFRECCIA_16, CoreStatics.ImageToByte(Risorse.GetImageFromResource(Risorse.GC_OCCHIOAPERTOFRECCIA_16)));
                            }
                            e.Row.Cells["CIV"].Value = oIcone[Risorse.GC_OCCHIOAPERTOFRECCIA_16];
                        }
                    }

                }
                if (e.Row.Cells.Exists("FlagPazienteSeguito") == true && (int)e.Row.Cells["FlagPazienteSeguito"].Value == 1)
                {
                    e.Row.Appearance.BackColor = _colorepazienteseguito;
                }

                if (e.Row.Cells.Exists("CodStatoConsensoCalcolato") == true)
                {
                    if (e.Row.Cells["CodStatoConsensoCalcolato"].Value.ToString() != "")
                    {
                        if (oIcone.ContainsKey(e.Row.Cells["CodStatoConsensoCalcolato"].Value.ToString()) == false)
                        {
                            oIcone.Add(e.Row.Cells["CodStatoConsensoCalcolato"].Value.ToString(), DBUtils.getIcona16ByTipoStato(EnumEntita.CNC, e.Row.Cells["CodStatoConsensoCalcolato"].Value.ToString(), ""));
                        }

                        if (e.Row.Cells["CodStatoConsensoCalcolato"].Value != null)
                        {
                            var objImageBytes = oIcone[e.Row.Cells["CodStatoConsensoCalcolato"].Value.ToString()];
                            if (objImageBytes != null) e.Row.Cells["SCC"].Value = objImageBytes;
                        }

                    }
                }

                InitializeRow(e.Row);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        private void ucRtfPaziente_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {

                foreach (UltraGridColumn oUgc in e.Layout.Bands[0].Columns)
                {
                    oUgc.Hidden = true;
                }

                if (e.Layout.Bands[0].Columns.Exists("Descrizione") == true)
                {
                    e.Layout.Bands[0].Columns["Descrizione"].Hidden = false;
                    e.Layout.Bands[0].Columns["Descrizione"].CellAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                }

                if (e.Layout.Bands[0].Columns.Exists("DatiRilievoRTF") == true)
                {

                    e.Layout.Bands[0].Columns["DatiRilievoRTF"].CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    RichTextEditor a = new RichTextEditor();
                    e.Layout.Bands[0].Columns["DatiRilievoRTF"].Editor = a;
                    e.Layout.Bands[0].Columns["DatiRilievoRTF"].Hidden = false;


                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }
        private void ucRtfPaziente_InitializeRow(object sender, InitializeRowEventArgs e)
        {

        }

        private void uteRicerca_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (this.ubRicerca.Enabled && e.KeyCode == Keys.Enter)
                {
                    this.UltimaSelId = "";
                    ricercaGridPazienti();
                }

            }
            catch (Exception)
            {
            }
        }

        private void ultraDockManager_InitializePane(object sender, Infragistics.Win.UltraWinDock.InitializePaneEventArgs e)
        {
            e.Pane.Settings.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Medium);
            e.Pane.Settings.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FILTRO_32);

            int filtroWidth = 12 * (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large);
            this.ultraDockManager.ControlPanes[0].FlyoutSize = new Size(filtroWidth, this.ultraDockManager.ControlPanes[0].FlyoutSize.Height);
            this.ultraDockManager.ControlPanes[0].Size = new Size(filtroWidth, this.ultraDockManager.ControlPanes[0].Size.Height);
            this.ultraDockManager.DockAreas[0].Size = new Size(filtroWidth, this.ultraDockManager.DockAreas[0].Size.Height);
            this.ucEasyTableLayoutPanelFiltriAvanzati.Width = filtroWidth;
        }

        private void ucEasyTreeViewFiltroStatoTrasferimento_AfterCheck(object sender, Infragistics.Win.UltraWinTree.NodeEventArgs e)
        {

            try
            {

                if (e.TreeNode.Key == CoreStatics.GC_TUTTI)
                {
                    this.ucEasyTreeViewFiltroStatoTrasferimento.EventManager.SetEnabled(Infragistics.Win.UltraWinTree.TreeEventIds.AfterCheck, false);
                    foreach (Infragistics.Win.UltraWinTree.UltraTreeNode oNode in this.ucEasyTreeViewFiltroStatoTrasferimento.Nodes[CoreStatics.GC_TUTTI].Nodes)
                    {
                        if (oNode.Override.NodeStyle == Infragistics.Win.UltraWinTree.NodeStyle.CheckBox)
                        {
                            oNode.CheckedState = e.TreeNode.CheckedState;
                        }
                    }
                    this.ucEasyTreeViewFiltroStatoTrasferimento.EventManager.SetEnabled(Infragistics.Win.UltraWinTree.TreeEventIds.AfterCheck, true);
                }

                this.Aggiorna();

            }
            catch (Exception)
            {

            }

        }

        private void ucEasyTreeViewFiltroStatoTrasferimento_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Clicks == 1 && e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    Infragistics.Win.UltraWinTree.UltraTreeNode child = ((Infragistics.Win.UltraWinTree.UltraTree)sender).GetNodeFromPoint(e.Location);
                    if (child != null)
                    {
                        int minleftexp = 10;
                        int minleftchk = 30;
                        if (child.Parent != null && child.Parent is Infragistics.Win.UltraWinTree.UltraTreeNode) minleftchk = 46;

                        if (child.NodeStyleResolved == Infragistics.Win.UltraWinTree.NodeStyle.CheckBox)
                        {
                            if (e.Location.X > minleftchk)
                            {
                                if (child.CheckedState == CheckState.Checked)
                                    child.CheckedState = CheckState.Unchecked;
                                else
                                    child.CheckedState = CheckState.Checked;
                            }
                        }
                        else if (child.Nodes.Count > 0)
                        {
                            if (e.Location.X > minleftexp)
                                child.Expanded = !child.Expanded;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void uchkSoloCartelleInVisione_CheckedChanged(object sender, EventArgs e)
        {
            if (this.FlagSkipLoadEvts == false) this.Aggiorna();
        }

        private void uchkSoloPazientiSeguiti_CheckedChanged(object sender, EventArgs e)
        {
            if (this.FlagSkipLoadEvts == false) this.Aggiorna();
        }

        private void ucEasyComboEditorFiltriSpeciali_ValueChanged(object sender, EventArgs e)
        {
            if (this.FlagSkipLoadEvts == false) this.Aggiorna();
        }

        private void ucEasyComboEditorFiltroStatoCartella_ValueChanged(object sender, EventArgs e)
        {
            if (this.FlagSkipLoadEvts == false) this.Aggiorna();
        }

        #endregion

        #region UltraPopupControlContainer

        private void UltraPopupControlContainer_Closed(object sender, EventArgs e)
        {
            _ucRichTextBox.RtfClick -= ucRichTextBox_Click;
        }

        private void UltraPopupControlContainer_Opened(object sender, EventArgs e)
        {
            _ucRichTextBox.RtfClick += ucRichTextBox_Click;
        }

        private void UltraPopupControlContainer_Opening(object sender, CancelEventArgs e)
        {
            Infragistics.Win.Misc.UltraPopupControlContainer popup = sender as Infragistics.Win.Misc.UltraPopupControlContainer;
            popup.PopupControl = _ucRichTextBox;
        }

        private void ucRichTextBox_Click(object sender, EventArgs e)
        {
        }

        private void ucRtfPaziente_ClickCell(object sender, ClickCellEventArgs e)
        {

            Infragistics.Win.UIElement uie;
            Point oPoint;

            try
            {

                switch (e.Cell.Column.Key)
                {

                    case "DatiRilievoRTF":
                        CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainer);
                        _ucRichTextBox = CoreStatics.GetRichTextBoxForPopupControlContainer(e.Cell.Text);

                        uie = e.Cell.GetUIElement();
                        oPoint = new Point(uie.Rect.Left + ((ucEasyGrid)sender).Parent.Parent.Parent.Parent.Location.X, uie.Rect.Top + ((ucEasyGrid)sender).Parent.Parent.Parent.Parent.Location.Y);

                        this.UltraPopupControlContainer.Show((ucEasyGrid)sender, oPoint);
                        break;

                    default:
                        break;

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucRtfPaziente_ClickCell", this.Name);
            }

        }

        #endregion


        private void ricercaGridPazienti()
        {
            CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

            this.Aggiorna();

            CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
        }

        private String createMainGridParams()
        {
            string xml = "";

            Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
            op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);

            if (this.uteRicerca.Text != string.Empty)
            {

                string filtrogenerico = string.Empty;

                string[] ricerche = this.uteRicerca.Text.Split(' ');
                foreach (string ricerca in ricerche)
                {

                    string format = "dd/MM/yyyy";
                    if (DateTime.TryParseExact(ricerca, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
                    {
                        op.Parametro.Add("DataNascita", ricerca);
                    }
                    else
                    {
                        filtrogenerico += ricerca + " ";
                    }

                }

                op.Parametro.Add("FiltroGenerico", filtrogenerico);

            }

            setParamFromGrid(UltraGridStruttura, "CodUA", "Codice", ref op);
            setParamFromGrid(UltraGridTipoEpisodio, "CodTipoEpisodio", "Codice", ref op);

            Dictionary<string, string> listastato = new Dictionary<string, string>();
            string[] codstato = null;

            if ((this.ucEasyTreeViewFiltroStatoTrasferimento.Nodes.Count > 0))
            {
                foreach (Infragistics.Win.UltraWinTree.UltraTreeNode oNode in this.ucEasyTreeViewFiltroStatoTrasferimento.Nodes[CoreStatics.GC_TUTTI].Nodes)
                {
                    if (oNode.Override.NodeStyle == Infragistics.Win.UltraWinTree.NodeStyle.CheckBox && oNode.CheckedState == CheckState.Checked)
                    {
                        listastato.Add(oNode.Key, oNode.Text);
                    }
                }
            }
            else
            {
                if (_skip_FiltroStatoTrasferimento_attivo)
                    _skip_FiltroStatoTrasferimento_attivo = false;
                else
                    listastato.Add(EnumStatoTrasferimento.AT.ToString(), "Attivo");
            }
            codstato = listastato.Keys.ToArray();
            op.ParametroRipetibile.Add("CodStatoTrasferimento", codstato);

            setParamFromCombo(ucEasyComboEditorFiltroStatoCartella, "CodStatoCartella", ref op, EnumStatoCartella.AP.ToString());

            setParamFromGrid(UltraGridUO, "UnitaOperativa", "Descrizione", ref op);
            setParamFromGrid(UltraGridSettore, "Settore", "Descrizione", ref op);
            setParamFromGrid(UltraGridStanza, "Stanza", "Descrizione", ref op);

            op.Parametro.Add("SoloCartelleInVisione", (this.uchkSoloCartelleInVisione.Checked == false ? "0" : "1"));
            op.Parametro.Add("SoloPazientiSeguiti", (this.uchkSoloPazientiSeguiti.Checked == false ? "0" : "1"));

            setParamFromCombo(ucEasyComboEditorFiltriSpeciali, "CodFiltroSpeciale", ref op);

            op.Parametro.Add("Ordinamento", "P.Cognome, P.Nome");

            if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.NumMaxCercaEpi > 0)
                op.Parametro.Add("NumRighe", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.NumMaxCercaEpi.ToString());

            xml = op.ToXmlString();

            return xml;
        }

        private void setParamFromGrid(ucEasyGrid grid, string paramName, string cellName, ref Parametri op)
        {
            if (grid == null)
                return;

            if (grid.ActiveRow == null)
                return;

            if (grid.ActiveRow.Cells[cellName].Text.Contains(CoreStatics.GC_TUTTI) != true)
            {
                op.Parametro.Add(paramName, grid.ActiveRow.Cells[cellName].Text);
            }

        }

        private void setParamFromCombo(ucEasyComboEditor combo, string paramName, ref Parametri op, String defaultValue = null)
        {
            if (combo.Value != null)
            {
                if (combo.Text.Trim() != CoreStatics.GC_TUTTI)
                {
                    op.Parametro.Add(paramName, combo.Value.ToString());
                }
            }
            else
            {
                if (defaultValue != null)
                    op.Parametro.Add(paramName, defaultValue.ToString());
            }
        }

        private void reselRow()
        {

            if (string.IsNullOrEmpty(this.UltimaSelId) == false && this.UltraGridRicerca.Rows.Count > 0)
            {
                for (int iRow = 0; iRow < this.UltraGridRicerca.Rows.Count; iRow++)
                {
                    UltraGridRow item = this.UltraGridRicerca.Rows[iRow];
                    if (item.IsDataRow && !item.IsFilteredOut && item.Cells["IDTrasferimento"].Text.Trim().ToUpper() == this.UltimaSelId.Trim().ToUpper())
                    {
                        this.UltraGridRicerca.ActiveRow = item;

                        break;
                    }
                }
            }
            else
            {
                this.AggiornaPaziente();
            }

            this.uteRicerca.Focus();

        }

        #region     ThreadProc


        private WorkerRunStoredProcedure CreateWorker(string sp, string xmlParam, SendOrPostCallback cb, bool returnDataset = false)
        {
            string cnn = Database.ConnectionString;
            WorkerRunStoredProcedure wk = new WorkerRunStoredProcedure(cnn, sp, xmlParam, cb, SynchronizationContext.Current, returnDataset);

            return wk;
        }

        private void cb_MSP_SelStatoCartella(object dataObject)
        {
            if (dataObject == null)
                return;

            try
            {
                DataTable dt = (DataTable)dataObject;

                this.ucEasyComboEditorFiltroStatoCartella.ValueMember = "Codice";
                this.ucEasyComboEditorFiltroStatoCartella.DisplayMember = "Descrizione";
                this.ucEasyComboEditorFiltroStatoCartella.DataSource = CoreStatics.AggiungiTuttiDataTable(dt, false);
                this.ucEasyComboEditorFiltroStatoCartella.Refresh();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "cb_MSP_SelStatoCartella", this.Name);
            }

        }

        private void cb_MSP_SelUODaRuolo(object dataObject)
        {
            if (dataObject == null)
                return;

            try
            {
                DataTable dt = (DataTable)dataObject;

                this.UltraGridUO.DataSource = CoreStatics.AggiungiTuttiDataTable(dt, false);
                this.UltraGridUO.DisplayLayout.Bands[0].Columns["Descrizione"].Header.Caption = "Unità Operativa";
                this.UltraGridUO.Refresh();


            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "cb_MSP_SelUODaRuolo", this.Name);
            }

        }

        private void cb_MSP_SelUADaRuolo(object dataObject)
        {
            if (dataObject == null)
                return;

            try
            {
                DataTable dt = (DataTable)dataObject;

                this.UltraGridStruttura.DataSource = CoreStatics.AggiungiTuttiDataTable(dt, false);
                this.UltraGridStruttura.DisplayLayout.Bands[0].Columns["Descrizione"].Header.Caption = "Struttura";
                this.UltraGridStruttura.Refresh();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "cb_MSP_SelUADaRuolo", this.Name);
            }

        }

        private void cb_MSP_SelTipoEpisodio(object dataObject)
        {
            if (dataObject == null)
                return;

            try
            {
                DataTable dt = (DataTable)dataObject;

                this.UltraGridTipoEpisodio.DataSource = CoreStatics.AggiungiTuttiDataTable(dt, false);
                this.UltraGridTipoEpisodio.DisplayLayout.Bands[0].Columns["Descrizione"].Header.Caption = "Tipo Episodio";
                this.UltraGridTipoEpisodio.Refresh();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "cb_MSP_SelTipoEpisodio", this.Name);
            }

        }

        private void cb_MSP_SelStatoTrasferimento(object dataObject)
        {
            if (dataObject == null)
                return;

            try
            {
                DataTable dt = (DataTable)dataObject;

                Infragistics.Win.UltraWinTree.UltraTreeNode oNodeRoot = new Infragistics.Win.UltraWinTree.UltraTreeNode(CoreStatics.GC_TUTTI, "Stato");
                oNodeRoot.Override.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
                oNodeRoot.CheckedState = CheckState.Checked;
                foreach (DataRow oDr in dt.Rows)
                {
                    Infragistics.Win.UltraWinTree.UltraTreeNode oNode = new Infragistics.Win.UltraWinTree.UltraTreeNode(oDr["Codice"].ToString(), oDr["Descrizione"].ToString());
                    oNode.Override.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
                    if (oNode.Key == "AT")
                    {
                        oNode.CheckedState = CheckState.Checked;
                    }
                    oNodeRoot.Nodes.Add(oNode);
                }
                this.ucEasyTreeViewFiltroStatoTrasferimento.Nodes.Clear();
                this.ucEasyTreeViewFiltroStatoTrasferimento.Nodes.Add(oNodeRoot);
                this.ucEasyTreeViewFiltroStatoTrasferimento.ExpandAll();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "cb_MSP_SelStatoTrasferimento", this.Name);
            }

        }

        private void cb_MSP_SelFiltriSpeciali(object dataObject)
        {
            if (dataObject == null)
                return;

            try
            {
                DataTable dt = (DataTable)dataObject;

                this.ucEasyComboEditorFiltriSpeciali.ValueMember = "Codice";
                this.ucEasyComboEditorFiltriSpeciali.DisplayMember = "Descrizione";
                this.ucEasyComboEditorFiltriSpeciali.DataSource = CoreStatics.AggiungiTuttiDataTable(dt, false);
                this.ucEasyComboEditorFiltriSpeciali.Refresh();
                this.ucEasyComboEditorFiltriSpeciali.SelectedIndex = 0;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "cb_MSP_SelFiltriSpeciali", this.Name);
            }

        }


        private void cb_MSP_CercaEpisodio(object dataObject)
        {
            if (dataObject == null)
                return;

            try
            {
                DataSet ds = (DataSet)dataObject;

                ds.Tables[0].Columns.Add("NA", typeof(Bitmap));
                ds.Tables[0].Columns.Add("NAG", typeof(Bitmap));
                ds.Tables[0].Columns.Add("NEC", typeof(Bitmap));
                ds.Tables[0].Columns.Add("CIV", typeof(Bitmap));
                ds.Tables[0].Columns.Add("SCC", typeof(Bitmap));


                this.tlpSettore.RowStyles[0].Height = 100;
                this.tlpSettore.RowStyles[1].Height = 0;
                this.pbSettore.Image = null;
                this.UltraGridSettore.DataSource = CoreStatics.AggiungiTuttiDataTable(ds.Tables[2], false);
                this.UltraGridSettore.DisplayLayout.Bands[0].Columns["Descrizione"].Header.Caption = "Settore";
                this.UltraGridSettore.Refresh();

                this.tlpStanza.RowStyles[0].Height = 100;
                this.tlpStanza.RowStyles[1].Height = 0;
                this.pbStanza.Image = null;
                this.UltraGridStanza.DataSource = CoreStatics.AggiungiTuttiDataTable(ds.Tables[3], false);
                this.UltraGridStanza.DisplayLayout.Bands[0].Columns["Descrizione"].Header.Caption = "Stanza";
                this.UltraGridStanza.Refresh();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "cb_MSP_CercaEpisodio", this.Name);
            }

        }

        private void cb_MSP_CercaEpisodioMainGrid(object dataObject)
        {
            if (dataObject == null)
                return;

            try
            {
                this.DataTableEpisodi = (DataTable)dataObject;

                showUI_GridView();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "cb_MSP_CercaEpisodioMainGrid", this.Name);
            }

        }

        private void showUI_GridView()
        {
            try
            {
                this.cmdRefresh.Visible = false;
                this.tlpCmds.SetColumnSpan(this.esScreen, 2);

                this.UltraGridRicerca.Visible = true;
                this.scvGrid.Visible = false;
                this.UltraGridRicerca.Dock = DockStyle.Fill;

                ShowUiResized(false);

                if (this.DataTableEpisodi.Columns.Contains("NA") == false) this.DataTableEpisodi.Columns.Add("NA", typeof(Bitmap));
                if (this.DataTableEpisodi.Columns.Contains("NAG") == false) this.DataTableEpisodi.Columns.Add("NAG", typeof(Bitmap));
                if (this.DataTableEpisodi.Columns.Contains("NEC") == false) this.DataTableEpisodi.Columns.Add("NEC", typeof(Bitmap));
                if (this.DataTableEpisodi.Columns.Contains("CIV") == false) this.DataTableEpisodi.Columns.Add("CIV", typeof(Bitmap));
                if (this.DataTableEpisodi.Columns.Contains("SCC") == false) this.DataTableEpisodi.Columns.Add("SCC", typeof(Bitmap));


                this.UltraGridRicerca.DataSource = this.DataTableEpisodi;
                this.UltraGridRicerca.Refresh();

                if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.NumMaxCercaEpi > 0 &&
                    CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.NumMaxCercaEpi == this.DataTableEpisodi.Rows.Count)
                {
                    this.UltraGridRicerca.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.True;
                    this.UltraGridRicerca.DisplayLayout.CaptionAppearance.ForeColor = Color.Red;
                    this.UltraGridRicerca.Text = "*** Numero casi troppo elevato, visualizzate solo le prime " + CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.NumMaxCercaEpi.ToString() + " righe. Si consiglia di affinare il filtro. *** ";
                }
                else
                {
                    this.UltraGridRicerca.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
                }
                this.UltraGridRicerca.DisplayLayout.Override.AllowColSizing = AllowColSizing.None;

                reselRow();

                if (String.IsNullOrEmpty(this.UltimaSelId) && this.UltraGridRicerca.ActiveRow != null)
                {
                    this.UltimaSelId = this.UltraGridRicerca.ActiveRow.Cells["IDTrasferimento"].Text;
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "showUI_GridView", this.Name);
            }
            finally
            {
                this.pbWait.Dock = DockStyle.None;
                this.pbWait.Visible = false;

                this.UltraGridRicerca.Dock = DockStyle.Fill;
                this.UltraGridRicerca.Visible = true;
            }

        }

        #endregion  ThreadProc

        #region Controls Events

        private void UltraGridUO_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            this.Aggiorna();
        }

        private void UltraGridTipoEpisodio_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            this.Aggiorna();
        }

        private void UltraGridStruttura_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            this.Aggiorna();
        }

        private void UltraGridSettore_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            this.Aggiorna();
        }

        private void UltraGridStanza_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            this.Aggiorna();
        }

        private void esScreen_Click(object sender, EventArgs e)
        {
            if (this.ScreenViewActive)
                showUI_ScreenView();
            else
                showUI_GridView();

        }

        private void scvGrid_EpisodeSelected(object sender, UserControls.ScreenAndTiles.EpisodeEventArgs args)
        {
            if (args.IdTrasferimento != this.UltimaSelId)
            {
                this.UltimaSelId = args.IdTrasferimento;

                UltraGridRow gridRow = this.UltraGridRicerca.Rows.FirstOrDefault(r => r.GetCellValue("IDTrasferimento").ToString() == args.IdTrasferimento);
                if (gridRow != null)
                    this.UltraGridRicerca.ActiveRow = gridRow;

                this.AggiornaPaziente();
            }
        }

        private void cmdAbort_Click(object sender, EventArgs e)
        {
            this.scvGrid.Abort();

            this.ShowUiResized(true);
        }

        private void cmdRefresh_Click(object sender, EventArgs e)
        {
            if (this.ScreenViewActive)
            {

                string codRuolo = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice;

                showCommands(true);

                if ((this.UltimaSelId == null) && (this.DataTableEpisodi.Rows.Count > 0) && (this.UltraGridRicerca.ActiveRow != null))
                    this.UltimaSelId = this.UltraGridRicerca.ActiveRow.Cells["IDTrasferimento"].Text;

                this.scvGrid.LoadData(this.DataTableEpisodi, codRuolo, this.ScreenRow, this.UltimaSelId);

                showCommands(false);
            }
            else
                ricercaGridPazienti();
        }

        #endregion Controls Events
    }
}

