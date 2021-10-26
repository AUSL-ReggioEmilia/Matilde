using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.ScciResource;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinListView;
using UnicodeSrl.Framework.Data;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using UnicodeSrl.ScciCore.CustomControls.Infra;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;
using UnicodeSrl.ScciCore.WebSvc;
using UnicodeSrl.Scci.DataContracts;
using System.Globalization;
using UnicodeSrl.ScciCore.SvcOrderEntry;
using UnicodeSrl.Scci.PluginClient;
using DevExpress.XtraTreeList.Internal;

namespace UnicodeSrl.ScciCore
{

    public partial class ucOrderEntryMonitorTrasversale : UserControl, Interfacce.IViewUserControlMiddle
    {

        #region Declare

        private UserControl _ucc = null;

        #endregion

        public ucOrderEntryMonitorTrasversale()
        {
            InitializeComponent();
            _ucc = (UserControl)this;
        }

        #region Interface

        public void Aggiorna()
        {
            if (this.IsDisposed == false)
            {
                this.CaricaGriglia();
            }
        }

        public void Carica()
        {
            try
            {

                this.InizializzaControlli();
                this.InizializzaUltraGridLayout();
                this.InizializzaFiltri();

                this.CaricaGriglia();
                this.ucEasyGrid.Focus();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Carica", this.Name);
            }
        }

        public void Ferma()
        {

            try
            {
                CoreStatics.SetContesto(EnumEntita.OE, null);
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Ferma", this.Name);
            }

        }

        #endregion

        #region private functions

        private void InizializzaControlli()
        {

            try
            {

                CoreStatics.SetEasyUltraDockManager(ref this.ultraDockManager);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaControlli", this.Name);
            }

        }

        private void InizializzaUltraGridLayout()
        {
            try
            {
                CoreStatics.SetEasyUltraGridLayout(ref this.ucEasyGrid);
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaUltraGridLayout", this.Name);
            }
        }

        #region Filtri

        private void InizializzaFiltri()
        {
            if (this.IsDisposed) return;

            try
            {

                this.ubApplicaFiltro.TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Large;
                this.drFiltro.DateFuture = true;
                this.drFiltro.Domani = true;
                this.drFiltro.Value = ucEasyDateRange.C_RNG_OGGI;

                this.CaricaFiltroTipo(true);
                this.CaricaFiltroUO();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaFiltri: Stato", this.Name);
            }
        }

        private void CaricaFiltroTipo(bool onlyall)
        {

            CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

            try
            {

                DataTable dtTipo = new DataTable();
                dtTipo.Columns.Add("TipoDesc", typeof(string));
                dtTipo.Columns.Add("TipoValore", typeof(string));
                dtTipo.Columns.Add("CodErogante", typeof(string));
                dtTipo.Columns.Add("CodAziendaErogante", typeof(string));

                this.AddGridRicercheDataRow(dtTipo, MovOrdine.EnumMovOrdineTipoFiltro.Tutti);

                if (!onlyall)
                {

                    MovOrdine oMov = new MovOrdine(CoreStatics.CoreApplication.Ambiente, "", "", "", "", "", "", "", "", null);
                    List<Scci.DataContracts.OESistemaErogante> eroganti = oMov.CaricaElencoCompletoEroganti2();
                    oMov = null;

                    IOrderedEnumerable<OESistemaErogante> sorted = eroganti.OrderBy(c => c.Descrizione);
                    foreach (OESistemaErogante erogante in sorted)
                    {
                        string tipoDesc = erogante.Descrizione;
                        string codAziendaErogante = "";

                        if (erogante.CodiceAzienda != null && erogante.CodiceAzienda.Trim() != "")
                        {
                            tipoDesc += @" - " + erogante.CodiceAzienda;
                            codAziendaErogante = erogante.CodiceAzienda;
                        }

                        this.AddGridRicercheDataRow(dtTipo, MovOrdine.EnumMovOrdineTipoFiltro.Erogante, tipoDesc, erogante.Codice, codAziendaErogante);
                    }

                }

                this.ucEasyGridFiltroTipo.DisplayLayout.Bands[0].Columns.ClearUnbound();
                this.ucEasyGridFiltroTipo.DataSource = dtTipo;
                this.ucEasyGridFiltroTipo.Refresh();
                if (dtTipo.Rows.Count > 0)
                {
                    this.ucEasyGridFiltroTipo.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.FirstRowInBand);
                    this.ucEasyGridFiltroTipo.ActiveRow = this.ucEasyGridFiltroTipo.Rows[0];
                }

            }
            catch (Exception)
            {

            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
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

        private void CaricaFiltroUO()
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);

                op.TimeStamp.CodEntita = EnumEntita.OE.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelUOAziendaDaRuolo", spcoll);

                this.ucEasyTreeViewUO.Override.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.SynchronizedCheckBox;
                this.ucEasyTreeViewUO.Nodes.Clear();

                Infragistics.Win.UltraWinTree.UltraTreeNode oNodeRoot = new Infragistics.Win.UltraWinTree.UltraTreeNode(CoreStatics.GC_TUTTI, "Unità Operative");
                oNodeRoot.CheckedState = CheckState.Unchecked;
                foreach (DataRow oDr in ds.Tables[0].Rows)
                {
                    Infragistics.Win.UltraWinTree.UltraTreeNode oNode = new Infragistics.Win.UltraWinTree.UltraTreeNode(oDr["CodAzi"].ToString() + "|" + oDr["CodUO"].ToString(), oDr["Descrizione"].ToString());
                    oNode.CheckedState = CheckState.Unchecked;

                    oNodeRoot.Nodes.Add(oNode);
                }
                this.ucEasyTreeViewUO.Nodes.Add(oNodeRoot);
                this.ucEasyTreeViewUO.ExpandAll();

            }
            catch (Exception)
            {

            }

        }

        #endregion Filtri

        private void CaricaGriglia()
        {

            ScciOrderEntryClient oeclient = null;

            string sCodAziendaSistemaErogante = string.Empty;
            string sCodSistemaErogante = string.Empty;

            try
            {

                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

                if (oeclient == null) oeclient = ScciSvcRef.GetSvcOrderEntry();

                if (this.ucEasyGridFiltroTipo.ActiveRow != null
                    && this.ucEasyGridFiltroTipo.ActiveRow.Cells["TipoValore"].Value.ToString() != MovOrdine.EnumMovOrdineTipoFiltro.Tutti.ToString())
                {
                    sCodAziendaSistemaErogante = this.ucEasyGridFiltroTipo.ActiveRow.Cells["CodAziendaErogante"].Value.ToString();
                    sCodSistemaErogante = this.ucEasyGridFiltroTipo.ActiveRow.Cells["CodErogante"].Value.ToString();
                }

                List<KeyValuePair<string, string>> listauo = new List<KeyValuePair<string, string>>();
                if (this.ucEasyTreeViewUO.Nodes.Exists(CoreStatics.GC_TUTTI) && this.ucEasyTreeViewUO.Nodes[CoreStatics.GC_TUTTI].CheckedState == CheckState.Indeterminate)
                {
                    foreach (Infragistics.Win.UltraWinTree.UltraTreeNode oNode in this.ucEasyTreeViewUO.Nodes[CoreStatics.GC_TUTTI].Nodes)
                    {
                        if (oNode.CheckedState == CheckState.Checked)
                        {
                            string[] codici = oNode.Key.Split('|');
                            listauo.Add(new KeyValuePair<string, string>(codici[0], codici[1]));
                        }
                    }
                }

                OEOrdineTestata[] ordini = oeclient.CercaOrdiniPerStatoPianificato(CoreStatics.CoreApplication.Ambiente.Codlogin,
                                                                                    (DateTime)this.udteFiltroDA.Value, (DateTime)this.udteFiltroA.Value,
                                                                                    (sCodAziendaSistemaErogante == "" ? null : sCodAziendaSistemaErogante),
                                                                                    (sCodSistemaErogante == "" ? null : sCodSistemaErogante),
                                                                                    listauo.ToArray(),
                                                                                    (this.txtNosologico.Text == "" ? null : this.txtNosologico.Text),
                                                                                    (this.txtCognome.Text == "" ? null : this.txtCognome.Text),
                                                                                    (this.txtNome.Text == "" ? null : this.txtNome.Text),
                                                                                    null, 0);

                this.ucEasyGrid.DataSource = null;
                this.ucEasyGrid.DataSource = getDtOrdini(ordini);
                this.ucEasyGrid.Refresh();

                this.setVisualizzaPaziente();

                if (ordini.Length == 100)
                {
                    this.ucEasyGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.True;
                    this.ucEasyGrid.DisplayLayout.CaptionAppearance.ForeColor = Color.Red;
                    this.ucEasyGrid.Text = "*** Numero ordini troppo elevato, visualizzate solo le prime " + ordini.Length.ToString() + " righe. Si consiglia di affinare il filtro. *** ";
                }
                else
                {
                    this.ucEasyGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaGriglia", this.Name);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
                if (oeclient != null) oeclient = null;
            }

        }

        private DataTable getDtOrdini(OEOrdineTestata[] ordini)
        {

            DataTable dt = new DataTable();

            try
            {

                dt.Columns.Add("Paziente", typeof(string));
                dt.Columns.Add("DataProgrammata", typeof(DateTime));
                dt.Columns.Add("Prestazioni", typeof(string));
                dt.Columns.Add("Priorita", typeof(string));
                dt.Columns.Add("SistemaErogante", typeof(string));
                dt.Columns.Add("Stato", typeof(string));
                dt.Columns.Add("StatoIcona", typeof(byte[]));
                dt.Columns.Add("Richiedente", typeof(string));
                dt.Columns.Add("DataOrdine", typeof(DateTime));
                dt.Columns.Add("DataPreferita", typeof(DateTime));
                dt.Columns.Add("NumeroOrdine", typeof(string));
                dt.Columns.Add("IDSac", typeof(string));

                foreach (OEOrdineTestata ordine in ordini)
                {

                    switch (ordine.Stato)
                    {

                        case OEStato.Inserito:
                        case OEStato.Accettato:
                        case OEStato.InCarico:
                        case OEStato.Programmato:
                        case OEStato.Erogato:
                        case OEStato.Errato:

                            if (checkUO(ordine.UnitaOperativaAziendaCodice, ordine.UnitaOperativaCodice))
                            {

                                DataRow dr = dt.NewRow();

                                dr["Paziente"] = $"{ordine.PazienteCognome} {ordine.PazienteNome} ({ordine.PazienteSesso})\n{ordine.PazienteDataNascita.ToString("dd/MM/yyyy")} - {ordine.Nosologico}";

                                if (ordine.DataOraProgrammataCalcolata != DateTime.MinValue)
                                {
                                    dr["DataProgrammata"] = ordine.DataOraProgrammataCalcolata;
                                }

                                if (ordine.DataOraPreferita != DateTime.MinValue)
                                {
                                    dr["DataPreferita"] = ordine.DataOraPreferita;
                                }

                                dr["Prestazioni"] = $"{ordine.AnteprimaPrestazioni}";
                                dr["Priorita"] = $"Priorità: {ordine.PrioritaDesc}";

                                dr["SistemaErogante"] = ordine.Eroganti;
                                dr["Stato"] = ordine.Stato;

                                switch (ordine.Stato)
                                {

                                    case OEStato.Inserito:
                                    case OEStato.Accettato:
                                        dr["StatoIcona"] = CoreStatics.ImageToByte(Risorse.GetImageFromResource(Risorse.GC_INPROGRESS_256));
                                        break;

                                    case OEStato.InCarico:
                                    case OEStato.Programmato:
                                    case OEStato.Erogato:
                                        dr["StatoIcona"] = CoreStatics.ImageToByte(Risorse.GetImageFromResource(Risorse.GC_MODIFICACHECK_256));
                                        break;

                                    case OEStato.Cancellato:
                                    case OEStato.Errato:
                                    case OEStato.Annullato:
                                        dr["StatoIcona"] = CoreStatics.ImageToByte(Risorse.GetImageFromResource(Risorse.GC_ELIMINA_256));
                                        break;

                                }

                                dr["Richiedente"] = $"{ordine.Operatore}\n{ordine.UnitaOperativaDescrizione}";
                                dr["DataOrdine"] = ordine.DataOrdine;

                                dr["NumeroOrdine"] = ordine.NumeroOrdine;
                                dr["IDSac"] = ordine.PazienteId;

                                dt.Rows.Add(dr);

                            }
                            break;

                        case OEStato.Cancellato:
                        case OEStato.Annullato:
                            break;

                    }

                }

            }
            catch (Exception)
            {

            }

            return dt;

        }

        private void setVisualizzaPaziente()
        {

            try
            {

                if (this.chkVisualizzaPaziente.Checked)
                {
                    Graphics g = this.CreateGraphics();
                    CoreStatics.ImpostaGroupByGriglia(ref this.ucEasyGrid, ref g, "Paziente");
                }
                else
                {
                    this.ucEasyGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                }
                this.ucEasyGrid.PerformLayout();

            }
            catch (Exception)
            {

            }

        }

        private bool checkUO(string azienda, string uo)
        {

            bool bret = false;

            try
            {

                string sCodice = $"{azienda}|{uo}";
                bret = this.ucEasyTreeViewUO.Nodes[CoreStatics.GC_TUTTI].Nodes.Exists(sCodice);

            }
            catch (Exception)
            {

            }

            return bret;

        }

        #endregion

        #region Events

        private void ultraDockManager_BeforeShowFlyout(object sender, Infragistics.Win.UltraWinDock.CancelableControlPaneEventArgs e)
        {
            if (this.ucEasyGridFiltroTipo.Rows.Count == 1)
            {
                this.CaricaFiltroTipo(false);
            }
        }

        private void ultraDockManager_InitializePane(object sender, Infragistics.Win.UltraWinDock.InitializePaneEventArgs e)
        {
            e.Pane.Settings.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large);
            e.Pane.Settings.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FILTRO_32);

            int filtroWidth = 20 * (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large);
            this.ultraDockManager.ControlPanes[0].FlyoutSize = new Size(filtroWidth, this.ultraDockManager.ControlPanes[0].FlyoutSize.Height);
            this.ultraDockManager.ControlPanes[0].Size = new Size(filtroWidth, this.ultraDockManager.ControlPanes[0].Size.Height);
            this.ultraDockManager.DockAreas[0].Size = new Size(filtroWidth, this.ultraDockManager.DockAreas[0].Size.Height);
            this.pnlFiltro.Width = filtroWidth;
        }

        private void txtRicerca_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.CaricaGriglia();
                    this.ucEasyGrid.Focus();
                }
            }
            catch (Exception)
            {
            }
        }

        private void chkVisualizzaPaziente_CheckedChanged(object sender, EventArgs e)
        {
            this.setVisualizzaPaziente();
        }

        private void drFiltro_ValueChanged(object sender, EventArgs e)
        {
            this.udteFiltroDA.Value = drFiltro.DataOraDa;
            this.udteFiltroA.Value = drFiltro.DataOraA;
        }

        private void ubApplicaFiltro_Click(object sender, EventArgs e)
        {
            if (ControlloFiltri() == true)
            {
                this.CaricaGriglia();
                this.ucEasyGrid.Focus();
            }
        }

        private bool ControlloFiltri()
        {
            bool bret = true;

            if (this.udteFiltroDA.Value == null || this.udteFiltroA.Value == null)
            {
                easyStatics.EasyMessageBox("Selezionare un periodo temporale.", "Monitor Ordini", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                bret = false;
            }

            return bret;
        }

        private void ucEasyGridFiltroTipo_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Exists("TipoDesc"))
            {
                e.Layout.Bands[0].Columns["TipoDesc"].Hidden = false;
                e.Layout.Bands[0].Columns["TipoDesc"].Header.Caption = "Sistema Erogante";
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

        private void ucEasyGrid_AfterRowActivate(object sender, EventArgs e)
        {
            CoreStatics.SetContesto(EnumEntita.OE, this.ucEasyGrid.ActiveRow);
        }

        private void ucEasyGrid_ClickCellButton(object sender, CellEventArgs e)
        {

            try
            {

                switch (e.Cell.Column.Key)
                {

                    case CoreStatics.C_COL_BTN_VIEW:
                        PazienteSac oPazienteSac = DBUtils.get_RicercaPazientiSACByID(this.ucEasyGrid.ActiveRow.Cells["IDSAC"].Text);
                        CoreStatics.CoreApplication.Paziente = new Paziente(oPazienteSac, CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata, CoreStatics.CoreApplication.AmbulatorialeUADescrizioneSelezionata);
                        CoreStatics.CoreApplication.MovOrdineSelezionato = new MovOrdine(this.ucEasyGrid.ActiveRow.Cells["NumeroOrdine"].Value.ToString(),
                    CoreStatics.CoreApplication.Ambiente,
                    string.Empty,
                    "AMB",
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    CoreStatics.CoreApplication.Paziente);
                        if (CoreStatics.CoreApplication.MovOrdineSelezionato != null)
                        {
                            CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.VisualizzaOrdine);
                            CoreStatics.CoreApplication.MovOrdineSelezionato = null;
                        }
                        CoreStatics.CoreApplication.Paziente = null;
                        break;

                    default:
                        break;

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_ClickCellButton", this.Name);
            }

        }

        private void ucEasyGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {

                int refWidth = (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XLarge) * 3;

                Graphics g = this.CreateGraphics();
                int refBtnWidth = CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.ucEasyGrid.DataRowFontRelativeDimension), g.DpiY) * 4;
                g.Dispose();
                g = null;

                e.Layout.Bands[0].HeaderVisible = false;
                e.Layout.Bands[0].ColHeadersVisible = true;
                e.Layout.Bands[0].RowLayoutStyle = RowLayoutStyle.GroupLayout;

                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {

                    try
                    {

                        oCol.SortIndicator = SortIndicator.Disabled;

                        #region formattazione colonne griglia

                        switch (oCol.Key)
                        {

                            case "StatoIcona":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.VertScrollBar = false;
                                oCol.Header.Caption = "";
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 0.5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 2;
                                break;

                            case "Paziente":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.Header.Caption = "Paziente";
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 2);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 2;
                                break;

                            case "DataProgrammata":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.Format = "dd/MM/yyyy HH:mm";
                                oCol.Header.Caption = "Data Programmata";
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 1.5);
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

                            case "DataPreferita":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.Format = "dd/MM/yyyy HH:mm";
                                oCol.Header.Caption = "Data Preferita";
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 1.5);
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

                            case "Prestazioni":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.Header.Caption = "Prestazioni";
                                try
                                {
                                    oCol.MaxWidth = this.ucEasyGrid.Width - Convert.ToInt32(refWidth * 11) - Convert.ToInt32(refBtnWidth * 1) - 40;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 3;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 2;
                                break;

                            case "Priorita":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.Header.Caption = "Priorità";
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 1.5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 4;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                                break;

                            case "Stato":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.Header.Caption = "Stato";
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 1.5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 4;
                                oCol.RowLayoutColumnInfo.OriginY = 1;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                                break;

                            case "SistemaErogante":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.Header.Caption = "Sistema Erogante";
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 2.5);
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
                                break;

                            case "Richiedente":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.Header.Caption = "Richiedente";
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 1.5);
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
                                break;

                            case "DataOrdine":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.Format = "dd/MM/yyyy HH:mm";
                                oCol.Header.Caption = "Data Ordine";
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 1.5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 7;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                                break;

                            case "NumeroOrdine":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.Header.Caption = "N. Ordine";
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 1.5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 7;
                                oCol.RowLayoutColumnInfo.OriginY = 1;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                                break;

                            default:
                                oCol.Hidden = true;
                                break;

                        }

                        #endregion

                    }
                    catch (Exception)
                    {
                    }

                }

                #region pulsante visualizzazione

                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_VIEW))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_VIEW);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_VISUALIZZA_32);

                    colEdit.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                    colEdit.Header.Caption = "";
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


                    colEdit.RowLayoutColumnInfo.OriginX = 8;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }

                #endregion

            }
            catch
            {
            }

        }

        private void ucEasyGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {

            try
            {

                if (e.Row.Cells["Stato"].Value.ToString() == "Programmato")
                {
                    e.Row.Appearance.BackColor = Color.LightYellow;
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_InitializeRow", this.Name);
            }

        }

        #endregion

    }
}
