using System;
using System.Windows.Forms;
using UnicodeSrl.Scci.Model;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.ScciCore.UserControls.ScreenAndTiles;
using UnicodeSrl.Framework.Threading;
using System.Threading;
using UnicodeSrl.Scci.DataContracts;
using System.Data;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using UnicodeSrl.Scci.Enums;
using System.Threading.Tasks;
using static UnicodeSrl.ScciCore.Interfacce;
using UnicodeSrl.ScciCore.Common.Extensions;
using UnicodeSrl.Framework.Diagnostics;
using UnicodeSrl.CdssScreenTiles;
using UnicodeSrl.Framework.UI.Controls;

namespace UnicodeSrl.ScciCore
{
    public partial class ucScreenGridView :
UserControl,
IEasyShortcutMultiKey
    {
        public event EpisodeEventHandler EpisodeSelected;

        private const int C_W_PAD_RIGHT = 0; 
        private const int C_W_FIRSTCELL = 20; 
        private const int C_H_FIRSTROW = 30; 
        private const int C_H_HEADER = 25; 
        private const int C_H_NAVROW = 30;
        private int m_currentPage = 0;
        private int m_pages = 0;

        public ucScreenGridView()
        {
            InitializeComponent();

            this.Selectors = new List<ucTileSelector>();
            this.PvtTokenSource = new CancellationTokenSource();
            this.ScreenColumnsState = new ScreenColumnsState();

            this.CurrentPage = 0;
            this.Pages = 0;

            initShortCuts();

            float fsz = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
            this.cboPages.Font = new Font(this.cboPages.Font.FontFamily, fsz);
        }

        #region     Paging e Dati

        public DataTable DataSource { get; set; }

        public int CurrentPage
        {
            get { return m_currentPage; }
            set
            {
                m_currentPage = value;
                OnPageChange();
            }
        }

        public int Pages
        {
            get { return m_pages; }
            set
            {
                bool changed = (m_pages != value);
                m_pages = value;

                if (changed) OnPagesChange();

                OnPageChange();
            }
        }

        private void OnPagesChange()
        {
            setPageComboState(true);

            List<int> ds = new List<int>();
            for (int i = 1; i <= this.Pages; i++)
            {
                ds.Add(i);
            }
            this.cboPages.DataSource = ds;
            this.cboPages.DropDownWidth = this.cboPages.Width;

            setPageComboState(false);
        }

        private void OnPageChange()
        {
            setPageComboState(true);

            this.cmdMoveFirst.Enabled = ((this.Pages > 0) && (this.CurrentPage > 1));
            this.cmdMovePrev.Enabled = ((this.Pages > 0) && (this.CurrentPage > 1));
            this.cmdMoveNext.Enabled = ((this.Pages > 0) && (this.CurrentPage < this.Pages));
            this.cmdMoveLast.Enabled = ((this.Pages > 0) && (this.CurrentPage < this.Pages));

            if (this.Pages > 1)
                this.tlpMain.RowStyles[2] = new RowStyle(SizeType.Absolute, C_H_NAVROW);
            else
                this.tlpMain.RowStyles[2] = new RowStyle(SizeType.Absolute, 0);

            if (this.Pages == 0)
            {
                this.lblPages.Text = "";
                return;
            }

            if (this.CurrentPage > 0) this.cboPages.Text = this.CurrentPage.ToString();
            this.lblPages.Text = "Pag. " + this.CurrentPage.ToString() + " di " + this.Pages.ToString();

            setPageComboState(false);
        }

        private void cmdMoveFirst_Click(object sender, EventArgs e)
        {
            ShowPage(1);
        }

        private void cmdMoveLast_Click(object sender, EventArgs e)
        {
            ShowPage(this.Pages);
        }

        private void cmdMoveNext_Click(object sender, EventArgs e)
        {
            if (this.CurrentPage <= this.Pages)
            {
                int page = this.CurrentPage + 1;
                ShowPage(page);
            }

        }

        private void cmdMovePrev_Click(object sender, EventArgs e)
        {
            if (this.CurrentPage > 1)
            {
                int page = this.CurrentPage - 1;
                ShowPage(page);
            }
        }

        private void setPageComboState(bool freeze)
        {
            if (freeze)
            {
                this.SkipPageComboEvent = true;
                this.cboPages.Enabled = false;
            }
            else
            {
                this.SkipPageComboEvent = false;
                this.cboPages.Enabled = true;
            }

        }

        private bool SkipPageComboEvent { get; set; }

        private void cboPages_SelectedValueChanged(object sender, EventArgs e)
        {
            if (SkipPageComboEvent) return;

            int page = Convert.ToInt32(this.cboPages.SelectedValue);
            ShowPage(page);
        }

        private void ShowPage(int pageNr)
        {
            try
            {
                if (this.Loading)
                    return;

                this.tlpNav.Enabled = false;
                DataTable screenData = new DataTable();
                screenData = this.DataSource.Clone();
                screenData.Rows.Clear();

                this.CurrentPage = pageNr;

                int rowPerPage = this.ScreenRow.Righe;

                int firstRow = rowPerPage * (pageNr - 1);
                int lastRow = firstRow + rowPerPage;

                if (this.DataSource.Rows.Count < lastRow)
                    lastRow = this.DataSource.Rows.Count;

                for (int i = firstRow; i < lastRow; i++)
                {
                    screenData.ImportRow(this.DataSource.Rows[i]);
                }

                this.DataTable = screenData;

                this.PvtTokenSource = new CancellationTokenSource();

                this.Loading = true;

                this.ThreadPool = new CoreThreadPool
                {
                    SynchronizationContext = SynchronizationContext.Current
                };

                this.ThreadPool.ThreadException +=
                    (s, p) =>
                    {
                        DiagnosticStatics.AddDebugInfo(p.Exception);
                    };

                this.tlpMain.SuspendLayout();

                cleanUp();

                loadUiGrid();

                setTlpMainColumns();

                loadHeaders();

                loadControls();

                this.tlpMain.ResumeLayout();

                if (this.QueueThreadAfter)
                    queueThreads();

                bool selected = false;
                if ((this.SelectedRowObj != null) && (this.SelectedRowObj.Key != null))
                    selected = this.SelectRowByKey(this.SelectedRowObj.Key);

                if ((selected == false) && (this.DataTable.Rows.Count > 0))
                {
                    this.SelectRow(0);
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Loading = false;
                this.tlpNav.Enabled = true;
                setHeaderTableWidthAndPos();
                setTlpMainColumns();
            }

        }


        private void ShowDataRefreshUpdate()
        {
            try
            {
                this.lblRefresh.Text = @"Ultimo aggiornamento dati: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        #endregion  Paging e Dati

        public void LoadData(DataTable sourceData, string codRuolo, T_ScreenRow screenObj, string keyToSel)
        {
            if (this.Loading)
                return;

            this.DataSource = sourceData;

            this.CodRuolo = codRuolo;
            this.ScreenRow = screenObj;

            using (FwDataConnection conn = new FwDataConnection(Database.ConnectionString))
            {
                this.ScreenTiles = conn.MSP_SelScreenTile(this.ScreenRow.Codice);
            }

            decimal pages = Convert.ToDecimal(this.DataSource.Rows.Count) / Convert.ToDecimal(screenObj.Righe);
            if ((pages % 1) == 0)
                this.Pages = Convert.ToInt32(pages);
            else
            {
                pages = Math.Truncate(pages);
                this.Pages = Convert.ToInt32(pages) + 1;
            }


            int page2Load = 1;

            if (String.IsNullOrEmpty(keyToSel) == false)
            {
                string filter = String.Format("IDTrasferimento = '{0}'", keyToSel);
                DataRow[] result = this.DataSource.Select(filter);

                if (result.Length > 0)
                {
                    int idx = this.DataSource.Rows.IndexOf(result[0]);
                    decimal tmpPage = Convert.ToDecimal(idx) / Convert.ToDecimal(screenObj.Righe);

                    page2Load = (int)(tmpPage) + 1;
                }

            }

            this.ShowPage(page2Load);

            this.ShowDataRefreshUpdate();
        }

        public void Abort()
        {
            if (this.ThreadPool == null)
                return;

            if (this.Loading == false)
                return;

            this.PvtTokenSource.Cancel();

            this.ThreadPool.Abort();

        }

        #region     Props


        private bool QueueThreadAfter
        {
            get
            {
                if (this.ScreenRow == null)
                    return true;

                if (this.ScreenRow.CaricaPerRiga.HasValue == false)
                    return true;

                return (this.ScreenRow.CaricaPerRiga == false);
            }
        }

        private CancellationTokenSource PvtTokenSource { get; set; }

        private Boolean Loading { get; set; }

        public DataTable DataTable { get; set; }

        internal CoreThreadPool ThreadPool { get; set; }

        public bool LoadingAborted
        {
            get
            {
                return this.PvtTokenSource.IsCancellationRequested;
            }
        }

        public T_ScreenRow ScreenRow { get; private set; }

        public FwDataBufferedList<T_ScreenTileRow> ScreenTiles { get; private set; }

        private List<T_ScreenTileRow> ScreenTilesNoScroll
        {
            get
            {
                if (this.ScreenTiles == null)
                    return null;

                return this.ScreenTiles.Buffer.Where<T_ScreenTileRow>(t => t.Fissa == true).ToList<T_ScreenTileRow>();
            }
        }

        private List<T_ScreenTileRow> ScreenTilesScroll
        {
            get
            {
                if (this.ScreenTiles == null)
                    return null;

                return this.ScreenTiles.Buffer.Where<T_ScreenTileRow>(t => t.Fissa == false).ToList<T_ScreenTileRow>();
            }
        }

        private ScreenColumnsState ScreenColumnsState
        { get; set; }

        private int WidthNoScroll
        {
            get
            {
                if (this.ScreenTilesNoScroll == null)
                    return 0;

                if (this.ScreenTilesNoScroll.Count == 0)
                    return 0;

                int w = 0;

                T_ScreenTileRow lastCol = this.ScreenTilesNoScroll.OrderByDescending(x => x.Colonna).First();
                w = lastCol.Colonna + lastCol.Larghezza;

                return w;
            }
        }

        private int WidthScroll
        {
            get
            {
                if (this.ScreenTilesScroll == null)
                    return 0;

                if (this.ScreenTilesScroll.Count == 0)
                    return 0;

                T_ScreenTileRow lastCol = this.ScreenTilesScroll.OrderByDescending(x => x.Colonna).First();
                int w = lastCol.Colonna + lastCol.Larghezza;

                return w;
            }
        }

        public string CodRuolo { get; private set; }

        private List<ucTileSelector> Selectors { get; set; }

        public int? SelectedRowIndex { get; private set; }

        public ucTileSelector SelectedRowObj { get; private set; }

        private List<ITileUserCtl> TileControls { get; set; }



        #endregion  Props


        #region     UI

        private void cleanUp()
        {
            this.tlpGrid.AutoScroll = false;

            cleanUpTLP(this.tlpHeaders);
            cleanUpTLP(this.tlpHeadersNoScroll);

            cleanUpTLP(this.tlpGridNoScroll);
            cleanUpTLP(this.tlpGrid);

            this.Selectors.Clear();

            if (this.TileControls != null)
                this.TileControls.Clear();
        }

        private void cleanUpTLP(ucEasyTableLayoutPanel tlp)
        {
            int count = tlp.Controls.Count;

            for (int i = 0; i < count; i++)
            {
                Control ctl = tlp.Controls[0];
                ctl.Dispose();
            }

            if (tlp.Controls.Count > 0)
                tlp.Controls.Clear();

            tlp.RowStyles.Clear();
            tlp.ColumnStyles.Clear();

            tlp.RowCount = 0;
            tlp.ColumnCount = 0;
        }

        private void loadUiGrid()
        {

            this.tlpMain.Visible = false;
            this.SuspendLayout();

            this.pnDataScrollable.VerticalScrollProperties.Value = 0;
            this.pnDataScrollable.AutoScroll = true;
            this.pnDataScrollable.VerticalScrollProperties.SmallChange = 20;
            this.pnDataScrollable.VerticalScrollProperties.LargeChange = 30;

            this.pnDataScrollable.ClientArea.MouseWheel += data_MouseWheel;

            this.tlpHeadersNoScroll.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, C_W_FIRSTCELL));
            this.tlpGridNoScroll.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, C_W_FIRSTCELL));

            this.tlpHeaders.RowStyles.Add(new RowStyle(SizeType.Absolute, C_H_HEADER));
            this.tlpHeaders.RowCount = 1;
            this.tlpHeadersNoScroll.RowStyles.Add(new RowStyle(SizeType.Absolute, C_H_HEADER));
            this.tlpHeadersNoScroll.RowCount = 1;
            RowStyle rs = new RowStyle(SizeType.Absolute, C_H_FIRSTROW);
            this.tlpMain.RowStyles[0] = rs;


            int numRows = this.DataTable.Rows.Count;
            int numColumns = 0;

            numColumns = this.WidthNoScroll;
            loadUiGrid_TLP(this.tlpGridNoScroll, numRows, numColumns);

            loadUiGrid_TLP(this.tlpHeadersNoScroll, 0, numColumns);


            numColumns = this.WidthScroll;
            loadUiGrid_TLP(this.tlpGrid, numRows, numColumns);

            loadUiGrid_TLP(this.tlpHeaders, 0, numColumns);


            this.tlpGridNoScroll.Location = new Point(0, 0);
            this.tlpGrid.Location = new Point(0, 0);
            this.tlpHeaders.Location = new Point(0, 0);

            this.tlpMain.Visible = true;
            this.ResumeLayout();
        }


        private void loadUiGrid_TLP(ucEasyTableLayoutPanel tlp, int numRows, int numColumns)
        {
            RowStyle rs = null;
            ColumnStyle cs = null;

            int wCol = Convert.ToInt32(this.Width * this.ScreenRow.LarghezzaColonnaGrid);
            int hRow = Convert.ToInt32(this.Height * this.ScreenRow.AltezzaRigaGrid);

            if ((numRows > 0) && (this.ScreenRow.AdattaAltezzaRighe.HasValue) && (this.ScreenRow.AdattaAltezzaRighe.Value == true))
                hRow = (this.Height / numRows) - 25;



            if (numRows > 0)
            {
                for (int i = 0; i < numRows; i++)
                {
                    rs = new RowStyle(SizeType.Absolute, hRow);
                    tlp.RowStyles.Add(rs);
                }

                tlp.RowCount = numRows;
            }



            if (numColumns > 0)
            {
                for (int i = 0; i < numColumns; i++)
                {
                    cs = new ColumnStyle(SizeType.Absolute, wCol);

                    tlp.ColumnStyles.Add(cs);
                }

                tlp.ColumnCount = tlp.ColumnStyles.Count;
            }


            tlp.Padding = new Padding(0, 0, C_W_PAD_RIGHT, 0);

        }


        private void setHeaderTableWidthAndPos()
        {

            this.tlpHeaders.Location = new Point(this.tlpGrid.Location.X, this.tlpHeaders.Location.Y);
        }

        private void loadSelector(int rowIndex, DataRow row)
        {

            ucTileSelector ts = new ucTileSelector
            {
                Name = "ts_row_" + rowIndex.ToString(),
                Checked = false,

                Tag = rowIndex,
                Key = row["IDTrasferimento"].ToString()
            };

            this.tlpGridNoScroll.Controls.Add(ts, 0, rowIndex);
            this.tlpGridNoScroll.SetRowSpan(ts, 1);
            this.tlpGridNoScroll.SetColumnSpan(ts, 1);

            ts.Dock = System.Windows.Forms.DockStyle.Fill;
            ts.Margin = new System.Windows.Forms.Padding(1);
            ts.Visible = true;
            ts.SelectTileRowCB = this.SelectRow;

            this.Selectors.Add(ts);

        }


        private void loadHeaders()
        {
            int colIdx = 0;

            Action<T_ScreenTileRow, ucTileHeader> setScreenColumnState = new Action<T_ScreenTileRow, ucTileHeader>((tileObject, th) =>
    {
        bool colStateExist = this.ScreenColumnsState.Exist(tileObject);

        if (colStateExist == false)
        {
            if ((tileObject.NonCollassabile == false) && (tileObject.Collassata == true))
            {
                this.ScreenColumnsState.AddOrUpdate(tileObject, en_TileGridColumnState.collapsed, false);
            }
            else
                this.ScreenColumnsState.AddOrUpdate(tileObject, en_TileGridColumnState.normal, true);
        }

        TileColumnState cs = this.ScreenColumnsState.GetTileColumnState(tileObject);

        if (cs.ColumnState == en_TileGridColumnState.collapsed)
        {
            th.Minimize();
            cs.DataLoaded = false;
        }
    }
);

            foreach (T_ScreenTileRow tileObject in this.ScreenTilesNoScroll)
            {
                colIdx = tileObject.Colonna + 1;

                ucTileHeader th = new ucTileHeader(tileObject)
                {
                    LinkedTable = this.tlpGridNoScroll
                };

                this.tlpHeadersNoScroll.Controls.Add(th, colIdx, 0);
                this.tlpHeadersNoScroll.SetColumnSpan(th, tileObject.Larghezza);
                this.tlpHeadersNoScroll.SetRowSpan(th, 1);

                th.Dock = System.Windows.Forms.DockStyle.Fill;
                th.Margin = new System.Windows.Forms.Padding(0);
                th.Visible = true;

                setScreenColumnState(tileObject, th);

                th.ColumnMinimized += Th_ColumnMinimized;

                th.ColumnMaximized += Th_ColumnMaximized;
                th.BeforeColumnMaximized += Th_BeforeColumnMaximized;

            }

            foreach (T_ScreenTileRow tileObject in this.ScreenTilesScroll)
            {
                colIdx = tileObject.Colonna;

                ucTileHeader th = new ucTileHeader(tileObject)
                {
                    LinkedTable = this.tlpGrid
                };

                this.tlpHeaders.Controls.Add(th, colIdx, 0);
                this.tlpHeaders.SetColumnSpan(th, tileObject.Larghezza);
                this.tlpHeaders.SetRowSpan(th, 1);

                th.Dock = System.Windows.Forms.DockStyle.Fill;
                th.Margin = new System.Windows.Forms.Padding(0);
                th.Visible = true;

                setScreenColumnState(tileObject, th);

                th.ColumnMinimized += Th_ColumnMinimized;

                th.ColumnMaximized += Th_ColumnMaximized;
                th.BeforeColumnMaximized += Th_BeforeColumnMaximized;
            }

        }



        private void loadControls()
        {
            try
            {
                this.TileControls = new List<ITileUserCtl>();

                for (int i = 0; i < this.DataTable.Rows.Count; i++)
                {
                    DataRow row = this.DataTable.Rows[i];
                    int rowIndex = i;

                    AppDataMarshaler appData = loadAppData(row);

                    loadSelector(rowIndex, row);

                    loadControlsRow(rowIndex, appData);

                    Application.DoEvents();

                    if (this.PvtTokenSource.IsCancellationRequested)
                        return;

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }

        }


        private void loadControlsRow(int row, AppDataMarshaler appData)
        {
            foreach (T_ScreenTileRow tileObject in this.ScreenTiles.Buffer)
            {
                if (this.PvtTokenSource.IsCancellationRequested)
                    return;

                TileColumnState columnState = this.ScreenColumnsState.GetTileColumnState(tileObject);
                if (columnState.ColumnState == en_TileGridColumnState.collapsed)
                {
                    loadControlTileLabel(row, tileObject);
                }
                else
                {
                    loadControlTile(row, tileObject, appData);
                }

            }
        }

        private void loadControlTileLabel(int row, T_ScreenTileRow tileObject)
        {
            int col = tileObject.Colonna + 1;

            if ((tileObject.Fissa.HasValue) && (tileObject.Fissa.Value == true))
            {
                this.tlpGridNoScroll.AddTileLabel(tileObject, row, col, 1);
            }
            else
            {
                col = tileObject.Colonna;
                this.tlpGrid.AddTileLabel(tileObject, row, col, 1);
            }
        }

        private void loadControlTile(int row, T_ScreenTileRow tileObject, AppDataMarshaler appData, bool forceLoad = false)
        {
            ITileUserCtl tileControl = TileContolFactory.CreateTileUserCtl(appData, tileObject);
            UserControl ptrUserControl = (UserControl)tileControl;
            ptrUserControl.Visible = false;

            int col = tileObject.Colonna + 1;

            if ((tileObject.Fissa.HasValue) && (tileObject.Fissa.Value == true))
            {
                this.tlpGridNoScroll.AddTileControl(tileControl, row, col, 1, tileObject.Larghezza);
            }
            else
            {
                col = tileObject.Colonna;
                this.tlpGrid.AddTileControl(tileControl, row, col, 1, tileObject.Larghezza);
            }

            this.TileControls.Add(tileControl);

            tileControl.DisplayUiLoading();
            if ((this.QueueThreadAfter == false) || forceLoad)
                this.ThreadPool.QueueWorker(tileControl);
        }

        private void queueThreads()
        {
            foreach (ITileUserCtl ctl in this.TileControls)
            {
                if (this.PvtTokenSource.IsCancellationRequested)
                    return;

                TileColumnState columnState = this.ScreenColumnsState.GetTileColumnState(ctl.ScreenTileRow);

                if (columnState.ColumnState == en_TileGridColumnState.normal) this.ThreadPool.QueueWorker(ctl);

            }
        }

        private void Th_ColumnMaximized(object sender, ScreenGridColumnEventArgs args)
        {
            ucTileHeader head = (ucTileHeader)sender;

            setColumnVisible(head.LinkedTable, args.ColumndIndex);

            setHeaderTableWidthAndPos();

            setTlpMainColumns();
        }

        private void Th_BeforeColumnMaximized(object sender, ScreenGridColumnEventArgs args)
        {
            ucTileHeader head = (ucTileHeader)sender;
            ucEasyTableLayoutPanel tlp = head.LinkedTable;
            int colIndex = args.ColumndIndex;

            List<UniLabelExt> listLabels = tlp.Controls.OfType<UniLabelExt>().Where(c => tlp.GetColumn(c) == colIndex).ToList();

            foreach (UniLabelExt child in listLabels)
            {
                tlp.Controls.Remove(child);
                child.Dispose();
            }

        }

        private void Th_ColumnMinimized(object sender, ScreenGridColumnEventArgs args)
        {
            ucTileHeader head = (ucTileHeader)sender;

            setColumnHidden(head.LinkedTable, args.ColumndIndex);

            setHeaderTableWidthAndPos();

            setTlpMainColumns();

        }

        private void setTlpMainColumns()
        {
            this.tlpHeadMain.ColumnStyles[0].Width = this.tlpGridNoScroll.Width;
            this.tlpGridMain.ColumnStyles[0].Width = this.tlpGridNoScroll.Width;
        }

        private void setColumnVisible(ucEasyTableLayoutPanel tlp, int colIndex)
        {
            tlp.SuspendLayout();

            IEnumerable<ITileUserCtl> tiles = tlp.Controls.OfType<ITileUserCtl>();
            IEnumerable<ITileUserCtl> selColTiles = tiles.Where(c => (c.ParentTableColumn == colIndex));
            T_ScreenTileRow tileObject = null;

            bool isScrollArea = (tlp.Name == this.tlpGrid.Name);

            if (selColTiles.Count() == 0)
            {
                for (int i = 0; i < this.DataTable.Rows.Count; i++)
                {
                    DataRow row = this.DataTable.Rows[i];
                    int rowIndex = i;

                    AppDataMarshaler appData = loadAppData(row);


                    if (isScrollArea)
                        tileObject = this.ScreenTilesScroll.First(c => (c.Colonna == colIndex));
                    else
                        tileObject = this.ScreenTilesNoScroll.First(c => (c.Colonna == (colIndex - 1)));

                    loadControlTile(rowIndex, tileObject, appData, true);

                    if (this.PvtTokenSource.IsCancellationRequested)
                        return;
                }
            }
            else
            {
                tileObject = selColTiles.First().ScreenTileRow;

                for (int i = 0; i < this.DataTable.Rows.Count; i++)
                {
                    Control tilectl = (Control)selColTiles.FirstOrDefault(ctl => ctl.ParentTableRow == i);
                    if (tilectl != null) tilectl.Visible = true;
                }

            }

            TileColumnState columnState = this.ScreenColumnsState.GetTileColumnState(tileObject);
            columnState.ColumnState = en_TileGridColumnState.normal;

            tlp.ResumeLayout();
        }

        private void setColumnHidden(ucEasyTableLayoutPanel tlp, int colIndex)
        {
            tlp.SuspendLayout();

            IEnumerable<ITileUserCtl> tiles = tlp.Controls.OfType<ITileUserCtl>();
            IEnumerable<ITileUserCtl> selColTiles = tiles.Where(c => (c.ParentTableColumn == colIndex));
            T_ScreenTileRow tileObject = selColTiles.First().ScreenTileRow;

            bool isScrollArea = (tlp.Name == this.tlpGrid.Name);

            for (int i = 0; i < this.DataTable.Rows.Count; i++)
            {
                int rowIndex = i;


                loadControlTileLabel(rowIndex, tileObject);

                if (this.PvtTokenSource.IsCancellationRequested)
                    return;
            }

            TileColumnState columnState = this.ScreenColumnsState.GetTileColumnState(tileObject);
            columnState.ColumnState = en_TileGridColumnState.collapsed;

            tlp.ResumeLayout();
        }

        private void scrollToSelector(ucTileSelector sel)
        {
            int topOfControl = sel.Top;
            int bottomOfControl = topOfControl + sel.Height;

            int visibleTop = Math.Abs(this.tlpGridNoScroll.Top);
            int visibleBottom = this.pnDataNotScrollable.Height - visibleTop;


            if ((topOfControl > visibleTop) && (bottomOfControl < visibleBottom))
                return;

            int scroll = 0;

            if (bottomOfControl > visibleBottom)
            {
                scroll = bottomOfControl - this.pnDataNotScrollable.Height;
            }

            if (scroll < 0) scroll = 0;
            if (scroll > this.pnDataScrollable.VerticalScrollProperties.Maximum) scroll = this.pnDataScrollable.VerticalScrollProperties.Maximum;

            this.pnDataScrollable.VerticalScrollProperties.Value = scroll;

            ScrollEventArgs args = new ScrollEventArgs(ScrollEventType.LargeIncrement, scroll, ScrollOrientation.VerticalScroll);
            data_ScrollCB(args);
        }

        public void SelectRow(int row)
        {
            foreach (ucTileSelector sel in this.Selectors)
            {
                if ((int)sel.Tag == row)
                {
                    sel.Checked = true;
                    this.SelectedRowIndex = row;
                    this.SelectedRowObj = sel;
                    scrollToSelector(sel);
                }
                else
                    sel.Checked = false;
            }

            this.OnEpisodeSelected(this.SelectedRowObj.Key);
        }

        public bool SelectRowByKey(String key)
        {
            ucTileSelector sel = this.Selectors.FirstOrDefault(x => x.Key == key);

            if (sel != null)
            {
                this.SelectRow((int)sel.Tag);

                return true;
            }

            return false;
        }

        private void cmdTiles_OpenAll_Click(object sender, EventArgs e)
        {
            var headers = this.tlpHeadersNoScroll.Controls.OfType<ucTileHeader>();

            foreach (ucTileHeader tileHeader in headers)
            {
                TileColumnState tileColumnState = this.ScreenColumnsState.GetTileColumnState(tileHeader.TileObject);
                if (tileColumnState.ColumnState == en_TileGridColumnState.collapsed)
                {
                    tileHeader.Maximize();
                }
            }

            headers = this.tlpHeaders.Controls.OfType<ucTileHeader>();

            foreach (ucTileHeader tileHeader in headers)
            {
                TileColumnState tileColumnState = this.ScreenColumnsState.GetTileColumnState(tileHeader.TileObject);
                if (tileColumnState.ColumnState == en_TileGridColumnState.collapsed)
                {
                    tileHeader.Maximize();
                }
            }

        }

        private void cmdTiles_CloseAll_Click(object sender, EventArgs e)
        {
            Action<IEnumerable<ucTileHeader>> closeHeaders = new Action<IEnumerable<ucTileHeader>>((headers) =>
{
 foreach (ucTileHeader tileHeader in headers)
 {
     TileColumnState tileColumnState = this.ScreenColumnsState.GetTileColumnState(tileHeader.TileObject);
     if ((tileColumnState.ColumnState == en_TileGridColumnState.normal) && (tileHeader.TileObject.NonCollassabile == false))
     {
         tileHeader.Minimize();
     }
 }

});

            var tileHeaders = this.tlpHeadersNoScroll.Controls.OfType<ucTileHeader>();
            closeHeaders(tileHeaders);

            tileHeaders = this.tlpHeaders.Controls.OfType<ucTileHeader>();
            closeHeaders(tileHeaders);
        }

        #endregion  UI

        private AppDataMarshaler loadAppData(DataRow row)
        {
            AppDataMarshaler appData = new AppDataMarshaler
            {
                ScciAmbiente = new ScciAmbiente()
            };
            appData.ScciAmbiente.Codlogin = CoreStatics.CoreApplication.Ambiente.Codlogin;
            appData.ScciAmbiente.Codruolo = this.CodRuolo;
            appData.ScciAmbiente.Contesto = CoreStatics.CoreApplication.Ambiente.Contesto;

            appData.ScciAmbiente.IdCartella = row["IDCartella"].ToString();
            appData.ScciAmbiente.Idepisodio = row["IDEpisodio"].ToString();
            appData.ScciAmbiente.Idpaziente = row["IDPaziente"].ToString();
            appData.ScciAmbiente.IdTrasferimento = row["IDTrasferimento"].ToString();

            appData.ScciAmbiente.Indirizzoip = CoreStatics.CoreApplication.Ambiente.Indirizzoip;
            appData.ScciAmbiente.Nomepc = CoreStatics.CoreApplication.Ambiente.Nomepc;

            appData.CodRuolo = this.CodRuolo;
            appData.CartellaChiusa = (row["CodStatoCartella"].ToString() == EnumStatoCartella.CH.ToString());
            appData.CodUA = row["CodUA"].ToString();

            appData.NumeroCartella = row["NumeroCartella"].ToString();
            appData.CodStatoCartella = row["CodStatoCartella"].ToString();
            appData.DecrStatoCartella = row["DecrStato"].ToString();

            appData.CodStatoTrasferimento = row["CodStatoTrasferimento"].ToString();

            appData.CodUA_Ambulatoriale = CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata;
            appData.CodUO = CoreStatics.CoreApplication.CodUOSelezionata;

            appData.DatiCercaEpisodio = new MSP_CercaEpisodioRow(row);

            return appData;
        }


        private void OnEpisodeSelected(string key)
        {
            if (this.EpisodeSelected != null)
            {
                EpisodeEventArgs args = new EpisodeEventArgs(key);
                this.EpisodeSelected(this, args);
            }

        }

        private void data_MouseWheel(object sender, MouseEventArgs e)
        {

            ScrollEventType sc = ScrollEventType.SmallIncrement;

            if (e.Delta < 0) sc = ScrollEventType.SmallDecrement;

            ScrollEventArgs evt = new ScrollEventArgs(sc, this.pnDataScrollable.VerticalScrollProperties.Value, ScrollOrientation.VerticalScroll);

            data_Scroll(this, evt);

        }

        private void data_Scroll(object sender, ScrollEventArgs e)
        {
            data_ScrollCB(e);
        }

        private void tlpGrid_LocationChanged(object sender, EventArgs e)
        {
            this.setHeaderTableWidthAndPos();
        }

        private void data_ScrollCB(ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                int y = e.NewValue * -1;
                this.tlpGridNoScroll.Location = new Point(0, y);
            }

        }


        #region     IEasyShortcutMultiKey + proc shortcut

        private void initShortCuts()
        {
            this.ShortcutKeys = new List<Keys>
            {
                Keys.Up,
                Keys.Down,
                Keys.Left,
                Keys.Right,

                Keys.PageUp,
                Keys.PageDown,

                Keys.Home,
                Keys.End
            };
        }

        public List<Keys> ShortcutKeys
        {
            get;
            private set;
        }

        public void ActionKeyDown(Keys keyCode, Keys modifiers)
        {

            if (this.Loading) return;
            if (this.SelectedRowObj == null) return;

            try
            {
                int row = Convert.ToInt32(this.SelectedRowObj.Tag);

                if (keyCode == Keys.Left)
                {
                    int newValLeft = this.pnDataScrollable.HorizontalScrollProperties.Value - this.pnDataScrollable.HorizontalScrollProperties.LargeChange;
                    if (newValLeft < 0) newValLeft = 0;

                    this.pnDataScrollable.HorizontalScrollProperties.Value = newValLeft;
                }

                if (keyCode == Keys.Right)
                {
                    int newValRight = this.pnDataScrollable.HorizontalScrollProperties.Value + this.pnDataScrollable.HorizontalScrollProperties.LargeChange;
                    if (newValRight > this.pnDataScrollable.HorizontalScrollProperties.Maximum) newValRight = this.pnDataScrollable.HorizontalScrollProperties.Maximum;

                    this.pnDataScrollable.HorizontalScrollProperties.Value = newValRight;
                }

                if (keyCode == Keys.Up)
                {
                    if (row == 0) return; row = row - 1;
                    this.SelectRow(row);
                }

                if (keyCode == Keys.Down)
                {
                    if (row == this.DataTable.Rows.Count - 1) return; row = row + 1;
                    this.SelectRow(row);
                }

                if (keyCode == Keys.PageUp)
                {
                    if (cmdMovePrev.Enabled == false) return;
                    cmdMovePrev.PerformClick();
                }

                if (keyCode == Keys.PageDown)
                {
                    if (cmdMoveNext.Enabled == false) return;
                    cmdMoveNext.PerformClick();
                }

                if (keyCode == Keys.Home)
                {
                    if (cmdMoveFirst.Enabled == false) return;
                    cmdMoveFirst.PerformClick();
                }

                if (keyCode == Keys.End)
                {
                    if (cmdMoveLast.Enabled == false) return;
                    cmdMoveLast.PerformClick();
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }






        #endregion  IEasyShortcutMultiKey + proc shortcut


    }
}
