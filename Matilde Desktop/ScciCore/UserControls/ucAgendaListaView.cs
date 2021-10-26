using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnicodeSrl.Scci;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci.Enums;
using Infragistics.Win.UltraWinGrid;
using UnicodeSrl.ScciResource;
using UnicodeSrl.ScciCore.CustomControls.Infra;
using System.Threading;

namespace UnicodeSrl.ScciCore
{
    public partial class ucAgendaListaView : UserControl, Interfacce.IViewAgendaLista
    {
        public ucAgendaListaView()
        {
            InitializeComponent();
        }

        #region Declare

        bool _FirstLoad = true;

        ParametriListaAgenda mo_ParametriListaAgenda = new ParametriListaAgenda();
        private string _IDAppSelezionato = string.Empty;

        public event EventHandler UltraGridAfterRowActivate;

        private ucRichTextBox _ucRichTextBox = null;

        #endregion

        #region Interface

        public string ViewCodAgenda { get; set; }

        public string ViewDescrizioneAgenda
        {
            get
            {
                return this.UltraDockManager.ControlPanes[0].Text;
            }
            set
            {
                this.UltraDockManager.ControlPanes[0].Text = value;
            }
        }

        public string ViewParametriLista { get; set; }

        public bool ViewPinnedFiltri
        {
            get { return this.UltraDockManager.ControlPanes[0].Pinned; }
            set
            {
                this.UltraDockManager.ControlPanes[0].Pinned = value;
            }
        }

        public bool ViewVisibleFiltri
        {
            get { return this.UltraDockManager.Visible; }
            set
            {
                this.UltraDockManager.Visible = value;
            }
        }

        public string ViewFiltroGenerico { get; set; }

        public void ViewInit()
        {

            this.InizializzaControlli();
            this.InizializzaFiltri();

        }

        #endregion

        #region private functions

        private void InizializzaControlli()
        {

            if (this.IsDisposed == false)
            {

                try
                {

                    CoreStatics.SetEasyUltraDockManager(ref this.UltraDockManager);

                    this.ubRefresh.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_AGGIORNA_256);
                    this.ubRefresh.PercImageFill = 0.75F;
                    this.ubRefresh.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;
                    this.ubRefresh.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                    CoreStatics.SetEasyUltraGridLayout(ref ugAppuntamenti);

                    ugAppuntamenti.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
                    ugAppuntamenti.DisplayLayout.GroupByBox.Hidden = true;
                    ugAppuntamenti.DisplayLayout.Override.HeaderClickAction = HeaderClickAction.Select;
                    ugAppuntamenti.DisplayLayout.Override.GroupByRowAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                    ugAppuntamenti.FilterRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
                    ugAppuntamenti.GridCaptionFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;
                    ugAppuntamenti.HeaderFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;
                    ugAppuntamenti.ColonnaRTFResize = "AnteprimaRTF";

                }
                catch (Exception ex)
                {
                    CoreStatics.ExGest(ref ex, "InizializzaControlli", this.Name);
                }

            }

        }

        private void InizializzaFiltri()
        {

            if (this.IsDisposed == false)
            {

                try
                {

                    this.drFiltro.Value = null;
                    this.drFiltro.DateFuture = false;
                    this.udteFiltroDA.Value = null;
                    this.udteFiltroA.Value = null;

                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("DatiEstesi", "1");
                    op.Parametro.Add("StatiPerFiltro", "1");
                    SqlParameterExt[] spcoll = new SqlParameterExt[1];
                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                    DataTable dt = Database.GetDataTableStoredProc("MSP_SelStatoAppuntamento", spcoll);

                    this.ucEasyTreeViewStato.Nodes.Clear();

                    Infragistics.Win.UltraWinTree.UltraTreeNode oNodeRoot = new Infragistics.Win.UltraWinTree.UltraTreeNode(CoreStatics.GC_TUTTI, "Stato");
                    oNodeRoot.Override.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;

                    oNodeRoot.CheckedState = CheckState.Unchecked;
                    foreach (DataRow oDr in dt.Rows)
                    {

                        if (oDr["Codice"].ToString() != EnumStatoAppuntamento.CA.ToString() &&
    oDr["Codice"].ToString() != EnumStatoAppuntamento.TR.ToString())
                        {
                            Infragistics.Win.UltraWinTree.UltraTreeNode oNode = new Infragistics.Win.UltraWinTree.UltraTreeNode(oDr["Codice"].ToString(), oDr["Descrizione"].ToString());
                            oNode.Override.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;

                            if (oDr["Codice"].ToString() == EnumStatoAppuntamento.DP.ToString() ||
    oDr["Codice"].ToString() == EnumStatoAppuntamento.IC.ToString() ||
    oDr["Codice"].ToString() == EnumStatoAppuntamento.PR.ToString())
                            {
                                oNode.CheckedState = CheckState.Checked;
                            }
                            else
                            {
                                oNode.CheckedState = CheckState.Unchecked;
                            }

                            oNodeRoot.Nodes.Add(oNode);
                        }

                    }
                    this.ucEasyTreeViewStato.Nodes.Add(oNodeRoot);
                    this.ucEasyTreeViewStato.ExpandAll();

                }
                catch (Exception ex)
                {
                    CoreStatics.ExGest(ref ex, "InizializzaFiltri", this.Name);
                }

            }

        }

        #endregion

        #region UltraGrid

        private void LoadCalendario()
        {

            try
            {

                mo_ParametriListaAgenda = UnicodeSrl.Scci.Statics.XmlProcs.XmlDeserializeFromString<ParametriListaAgenda>(this.ViewParametriLista);

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodAgenda", this.ViewCodAgenda);
                op.Parametro.Add("DatiEstesi", "0");
                op.Parametro.Add("Lista", "1");
                if (drFiltro.Value != null)
                {
                    DateTime dtI = new DateTime(((DateTime)this.udteFiltroDA.Value).Year, ((DateTime)this.udteFiltroDA.Value).Month, ((DateTime)this.udteFiltroDA.Value).Day,
                                                0, 0, 0);
                    DateTime dtF = new DateTime(((DateTime)this.udteFiltroA.Value).Year, ((DateTime)this.udteFiltroA.Value).Month, ((DateTime)this.udteFiltroA.Value).Day,
                                                23, 59, 59);
                    op.Parametro.Add("DataInserimentoInizio", Database.dataOra105PerParametri(dtI));
                    op.Parametro.Add("DataInserimentoFine", Database.dataOra105PerParametri(dtF));
                }

                Dictionary<string, string> listastato = new Dictionary<string, string>();
                if (this.ucEasyTreeViewStato.Nodes.Exists(CoreStatics.GC_TUTTI) && this.ucEasyTreeViewStato.Nodes[CoreStatics.GC_TUTTI].CheckedState != CheckState.Checked)
                {
                    foreach (Infragistics.Win.UltraWinTree.UltraTreeNode oNode in this.ucEasyTreeViewStato.Nodes[CoreStatics.GC_TUTTI].Nodes)
                    {
                        if (oNode.Override.NodeStyle == Infragistics.Win.UltraWinTree.NodeStyle.CheckBox && oNode.CheckedState == CheckState.Checked)
                        {
                            listastato.Add(oNode.Key, oNode.Text);
                        }
                    }
                }
                string[] codstato = listastato.Keys.ToArray();
                op.ParametroRipetibile.Add("CodStatoAppuntamento", codstato);
                op.Parametro.Add("FiltroGenerico", this.ViewFiltroGenerico);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                this.ugAppuntamenti.SuspendLayout();
                this.ugAppuntamenti.DataSource = Database.GetDataTableStoredProc("MSP_SelMovAppuntamentiAgende", spcoll);
                this.ugAppuntamenti.Refresh();

                if (mo_ParametriListaAgenda.TipoRaggruppamentoAgenda1 != EnumTipoRaggruppamentoAgenda.Nessuno)
                {

                    if (this.ugAppuntamenti.DisplayLayout.Bands[0].Columns.Exists("DescrRaggr1"))
                    {
                        this.ugAppuntamenti.DisplayLayout.Bands[0].SortedColumns.Add(this.ugAppuntamenti.DisplayLayout.Bands[0].Columns["DescrRaggr1"], false, true);
                    }

                    if (mo_ParametriListaAgenda.TipoRaggruppamentoAgenda2 != EnumTipoRaggruppamentoAgenda.Nessuno)
                    {

                        if (this.ugAppuntamenti.DisplayLayout.Bands[0].Columns.Exists("DescrRaggr2"))
                        {
                            this.ugAppuntamenti.DisplayLayout.Bands[0].SortedColumns.Add(this.ugAppuntamenti.DisplayLayout.Bands[0].Columns["DescrRaggr2"], false, true);
                        }

                        if (mo_ParametriListaAgenda.TipoRaggruppamentoAgenda3 != EnumTipoRaggruppamentoAgenda.Nessuno)
                        {

                            if (this.ugAppuntamenti.DisplayLayout.Bands[0].Columns.Exists("DescrRaggr3"))
                            {
                                this.ugAppuntamenti.DisplayLayout.Bands[0].SortedColumns.Add(this.ugAppuntamenti.DisplayLayout.Bands[0].Columns["DescrRaggr3"], false, true);
                            }

                        }

                    }

                }
                this.ugAppuntamenti.DisplayLayout.Bands[0].SortedColumns.Add(this.ugAppuntamenti.DisplayLayout.Bands[0].Columns["DataInserimento"], false, false);

                if (_IDAppSelezionato != string.Empty)
                {
                    this.ugAppuntamenti.ActiveRow = getActiveRow(this.ugAppuntamenti.Rows);
                }
                else
                {
                    this.ugAppuntamenti.ActiveRow = null;
                    if (this.ugAppuntamenti.Rows.Count(r => r.IsGroupByRow == true) == 1 && this.ugAppuntamenti.DisplayLayout.Bands[0].SortedColumns.Count == 2)
                    {
                        this.ugAppuntamenti.Rows.ExpandAll(true);
                    }
                }
                this.ugAppuntamenti.ResumeLayout();

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private UltraGridRow getActiveRow(RowsCollection oRows)
        {

            UltraGridRow oUgrReturn = null;

            foreach (UltraGridRow oUgr in oRows)
            {

                if (oUgr.IsGroupByRow)
                {
                    oUgrReturn = getActiveRow(((UltraGridGroupByRow)oUgr).Rows);
                }
                else
                {
                    if (oUgr.Cells["ID"].Value.ToString() == this._IDAppSelezionato)
                    {
                        oUgrReturn = oUgr;
                    }
                }

                if (oUgrReturn != null)
                {
                    return oUgrReturn;
                }

            }

            return oUgrReturn;

        }

        #endregion

        #region Events

        private void UltraDockManager_InitializePane(object sender, Infragistics.Win.UltraWinDock.InitializePaneEventArgs e)
        {
            e.Pane.Settings.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
            e.Pane.Settings.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FILTRO_32);
        }

        private void drFiltro_ValueChanged(object sender, EventArgs e)
        {
            this.udteFiltroDA.Value = drFiltro.DataOraDa;
            this.udteFiltroA.Value = drFiltro.DataOraA;
            this.RefreshData();
        }

        private void ucEasyTreeViewStato_AfterCheck(object sender, Infragistics.Win.UltraWinTree.NodeEventArgs e)
        {

            if (e.TreeNode.Key == CoreStatics.GC_TUTTI)
            {
                this.ucEasyTreeViewStato.EventManager.SetEnabled(Infragistics.Win.UltraWinTree.TreeEventIds.AfterCheck, false);
                foreach (Infragistics.Win.UltraWinTree.UltraTreeNode oNode in this.ucEasyTreeViewStato.Nodes[CoreStatics.GC_TUTTI].Nodes)
                {
                    if (oNode.Override.NodeStyle == Infragistics.Win.UltraWinTree.NodeStyle.CheckBox)
                    {
                        oNode.CheckedState = e.TreeNode.CheckedState;
                    }
                }
                this.ucEasyTreeViewStato.EventManager.SetEnabled(Infragistics.Win.UltraWinTree.TreeEventIds.AfterCheck, true);
            }

            this.RefreshData();

        }

        private void ubRefresh_Click(object sender, EventArgs e)
        {
            this.RefreshData();
        }

        private void ugAppuntamenti_AfterRowActivate(object sender, EventArgs e)
        {
            if (this.ugAppuntamenti.ActiveRow != null && this.ugAppuntamenti.ActiveRow.IsDataRow)
            {
                this._IDAppSelezionato = this.ugAppuntamenti.ActiveRow.Cells["ID"].Text;
            }
            else
            {
            }
            if (this.UltraGridAfterRowActivate != null) this.UltraGridAfterRowActivate(this, e);
        }

        private void ugAppuntamenti_ClickCell(object sender, ClickCellEventArgs e)
        {

            try
            {

                switch (e.Cell.Column.Key)
                {

                    case "AnteprimaRTF":
                        CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainer);
                        _ucRichTextBox = CoreStatics.GetRichTextBoxForPopupControlContainer(e.Cell.Text);

                        Infragistics.Win.UIElement uie = e.Cell.GetUIElement();
                        Point oPoint = new Point(uie.Rect.Left, uie.Rect.Top);

                        this.UltraPopupControlContainer.Show(this.ugAppuntamenti, oPoint);
                        break;

                    default:
                        break;

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ugAppuntamenti_ClickCell", this.Name);
            }

        }

        private void ugAppuntamenti_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {

                int refWidth = (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small) * 3;

                Graphics g = this.CreateGraphics();
                int refBtnWidth = Convert.ToInt32(CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(((ucEasyGrid)sender).DataRowFontRelativeDimension), g.DpiY) * 3);
                g.Dispose();
                g = null;
                e.Layout.Bands[0].RowLayoutStyle = RowLayoutStyle.GroupLayout;
                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {

                    try
                    {


                        switch (oCol.Key)
                        {

                            case "DataInserimento":
                                oCol.Hidden = false;
                                oCol.Header.Caption = "Data Inserimento";
                                oCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.Format = "dd/MM/yyyy HH:mm";
                                try
                                {
                                    oCol.MaxWidth = (refWidth * 5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }
                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 1;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                                break;

                            case "Oggetto":
                                oCol.Hidden = false;
                                oCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = ((ucEasyGrid)sender).Width - (refWidth * 6);
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

                            case "DescrStatoAppuntamento":
                                oCol.Hidden = false;
                                oCol.Header.Caption = "Stato";
                                oCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.LockedWidth = true;
                                try
                                {
                                    oCol.MaxWidth = (refWidth * 5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }
                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 2;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                                break;

                            case "AnteprimaRTF":
                                oCol.Hidden = false;
                                oCol.Header.Caption = "Descrizione";
                                RichTextEditor a = new RichTextEditor();
                                oCol.Editor = a;
                                oCol.CellActivation = Activation.ActivateOnly;
                                oCol.CellClickAction = CellClickAction.CellSelect;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.VertScrollBar = true;

                                try
                                {
                                    oCol.MaxWidth = ((ucEasyGrid)sender).Width - (refWidth * 6);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }
                                oCol.RowLayoutColumnInfo.OriginX = 2;
                                oCol.RowLayoutColumnInfo.OriginY = 2;
                                oCol.RowLayoutColumnInfo.SpanX = 3;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                                break;

                            case "DescrRaggr1":
                                oCol.Hidden = !(mo_ParametriListaAgenda.TipoRaggruppamentoAgenda1 != EnumTipoRaggruppamentoAgenda.Nessuno);
                                oCol.Hidden = true;
                                if (mo_ParametriListaAgenda.DescrizioneRaggruppamentoAgenda1 != string.Empty)
                                {
                                    oCol.Header.Caption = mo_ParametriListaAgenda.DescrizioneRaggruppamentoAgenda1;
                                }
                                break;

                            case "DescrRaggr2":
                                oCol.Hidden = !(mo_ParametriListaAgenda.TipoRaggruppamentoAgenda2 != EnumTipoRaggruppamentoAgenda.Nessuno);
                                oCol.Hidden = true;
                                if (mo_ParametriListaAgenda.DescrizioneRaggruppamentoAgenda2 != string.Empty)
                                {
                                    oCol.Header.Caption = mo_ParametriListaAgenda.DescrizioneRaggruppamentoAgenda2;
                                }
                                break;

                            case "DescrRaggr3":
                                oCol.Hidden = !(mo_ParametriListaAgenda.TipoRaggruppamentoAgenda3 != EnumTipoRaggruppamentoAgenda.Nessuno);
                                oCol.Hidden = true;
                                if (mo_ParametriListaAgenda.DescrizioneRaggruppamentoAgenda3 != string.Empty)
                                {
                                    oCol.Header.Caption = mo_ParametriListaAgenda.DescrizioneRaggruppamentoAgenda3;
                                }
                                break;

                            default:
                                oCol.Hidden = true;
                                break;

                        }

                    }
                    catch (Exception)
                    {

                    }

                }

            }
            catch (Exception)
            {

            }

        }

        private void ugAppuntamenti_InitializePrintPreview(object sender, CancelablePrintPreviewEventArgs e)
        {

            try
            {

                e.PrintDocument.DocumentName = this.ViewDescrizioneAgenda;

                e.PrintLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;

                e.PrintPreviewSettings.Zoom = -1;

                e.PrintPreviewSettings.DialogLeft = SystemInformation.WorkingArea.X;
                e.PrintPreviewSettings.DialogTop = SystemInformation.WorkingArea.Y;
                e.PrintPreviewSettings.DialogWidth = SystemInformation.WorkingArea.Width;
                e.PrintPreviewSettings.DialogHeight = SystemInformation.WorkingArea.Height;

                e.DefaultLogicalPageLayoutInfo.FitWidthToPages = 1;

                e.DefaultLogicalPageLayoutInfo.PageHeader = this.ViewDescrizioneAgenda;
                e.DefaultLogicalPageLayoutInfo.PageHeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.None;
                e.DefaultLogicalPageLayoutInfo.PageHeaderHeight = 40;
                e.DefaultLogicalPageLayoutInfo.PageHeaderAppearance.FontData.Name = "Thaoma";
                e.DefaultLogicalPageLayoutInfo.PageHeaderAppearance.FontData.SizeInPoints = 14;
                e.DefaultLogicalPageLayoutInfo.PageHeaderAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                e.DefaultLogicalPageLayoutInfo.PageHeaderAppearance.TextHAlign = Infragistics.Win.HAlign.Center;

                e.DefaultLogicalPageLayoutInfo.PageFooter = DateTime.Now + " - Page <#>.";
                e.DefaultLogicalPageLayoutInfo.PageFooterBorderStyle = Infragistics.Win.UIElementBorderStyle.None;
                e.DefaultLogicalPageLayoutInfo.PageFooterHeight = 15;
                e.DefaultLogicalPageLayoutInfo.PageFooterAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.DefaultLogicalPageLayoutInfo.PageHeaderAppearance.FontData.Name = "Thaoma";
                e.DefaultLogicalPageLayoutInfo.PageFooterAppearance.FontData.Italic = Infragistics.Win.DefaultableBoolean.True;

                e.DefaultLogicalPageLayoutInfo.ClippingOverride = Infragistics.Win.UltraWinGrid.ClippingOverride.Yes;

            }
            catch (Exception)
            {

            }

        }

        private void ugAppuntamenti_InitializeRow(object sender, InitializeRowEventArgs e)
        {

            try
            {

                if (e.Row.Cells["Colore"].Value != null)
                {
                    e.Row.Appearance.BackColor = CoreStatics.GetColorFromString(e.Row.Cells["Colore"].Value.ToString());
                }
                e.Row.Height = 64;

            }
            catch (Exception)
            {

            }

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

        #endregion

        #region Public Events

        public void RefreshData()
        {

            try
            {

                if (_FirstLoad == true)
                {
                    this.InizializzaControlli();
                    this.InizializzaFiltri();
                }
                _FirstLoad = false;

                this.LoadCalendario();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        #endregion

    }
}
