using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win.UltraWinGrid;

namespace UnicodeSrl.ScciCore
{
    public partial class ucMultiSelect : UserControl, Interfacce.IViewMultiSelect
    {
        public ucMultiSelect()
        {
            InitializeComponent();
        }

        #region Declare

        private bool _ViewShowFind = true;
        private bool _ViewShowAll = true;

        public event InitializeLayoutEventHandler GridSXInitializeLayout;
        public event InitializeRowEventHandler GridSXInitializeRow;

        public event InitializeLayoutEventHandler GridDXInitializeLayout;
        public event InitializeRowEventHandler GridDXInitializeRow;
        public event DoubleClickCellEventHandler GridDXDoubleClickCell;

        public event ChangeEventHandler GridChange;

        private string _gridSXCaptionColumnKey = "";
        private string _gridSXCaption = "";
        private int _gridSXFilterColumnIndex = 1;
        private string _gridDXCaptionColumnKey = "";
        private string _gridDXCaption = "";
        private int _gridDXFilterColumnIndex = 1;

        #endregion

        #region Interface

        public bool ViewShowFind
        {
            get
            {
                return _ViewShowFind;
            }
            set
            {
                _ViewShowFind = value;
                this.lblCercaSX.Visible = _ViewShowFind;
                this.uteCercaSX.Visible = _ViewShowFind;
                this.lblCercaDX.Visible = _ViewShowFind;
                this.uteCercaDX.Visible = _ViewShowFind;
                this.TableLayoutPanel.RowStyles[this.TableLayoutPanel.RowCount - 1].Height = (_ViewShowFind == true ? 28 : 0);
            }
        }

        public bool ViewShowAll
        {
            get
            {
                return _ViewShowAll;
            }
            set
            {
                _ViewShowAll = value;
                this.ubInserisciAll.Visible = _ViewShowAll;
                this.ubCancellaAll.Visible = _ViewShowAll;
            }
        }

        public DataSet ViewDataSetSX
        {
            get
            {
                return (DataSet)this.UltraGridSX.DataSource;
            }
            set
            {
                this.UltraGridSX.DataSource = value;
            }
        }

        public DataView ViewDataViewSX
        {
            get
            {
                return (DataView)this.UltraGridSX.DataSource;
            }
            set
            {
                this.UltraGridSX.DataSource = value;
            }
        }

        public DataSet ViewDataSetDX
        {
            get
            {
                return (DataSet)this.UltraGridDX.DataSource;
            }
            set
            {
                this.UltraGridDX.DataSource = value;
            }
        }

        public DataView ViewDataViewDX
        {
            get
            {
                return (DataView)this.UltraGridDX.DataSource;
            }
            set
            {
                this.UltraGridDX.DataSource = value;
            }
        }

        public void ViewInit()
        {

            CoreStatics.SetUltraGridLayout(ref this.UltraGridSX, false, false);
            CoreStatics.SetUltraGridLayout(ref this.UltraGridDX, false, false);

        }

        public string GridSXCaptionColumnKey
        {
            get
            {
                return _gridSXCaptionColumnKey;
            }
            set
            {
                _gridSXCaptionColumnKey = value;
            }
        }

        public string GridSXCaption
        {
            get
            {
                return _gridSXCaption;
            }
            set
            {
                _gridSXCaption = value;
            }
        }

        public int GridSXFilterColumnIndex
        {
            get
            {
                return _gridSXFilterColumnIndex;
            }
            set
            {
                _gridSXFilterColumnIndex = value;
            }
        }

        public string GridDXCaptionColumnKey
        {
            get
            {
                return _gridDXCaptionColumnKey;
            }
            set
            {
                _gridDXCaptionColumnKey = value;
            }
        }

        public string GridDXCaption
        {
            get
            {
                return _gridDXCaption;
            }
            set
            {
                _gridDXCaption = value;
            }
        }

        public int GridDXFilterColumnIndex
        {
            get
            {
                return _gridDXFilterColumnIndex;
            }
            set
            {
                _gridDXFilterColumnIndex = value;
            }
        }

        #endregion

        #region UltraGrid

        public UltraGrid GridDX
        {
            get { return this.UltraGridDX; }
        }

        public UltraGrid GridSX
        {
            get { return this.UltraGridSX; }
        }

        private void CopyDaA(UltraGrid DaGriglia, UltraGrid AGriglia)
        {

            if (AGriglia.DataSource.GetType() == typeof(DataSet))
            {
                DataSet oDsAGriglia = (DataSet)AGriglia.DataSource;
                while (DaGriglia.Selected.Rows.Count > 0)
                {
                    DataRow oDr = oDsAGriglia.Tables[0].NewRow();
                    for (int x = 0; x <= oDsAGriglia.Tables[0].Columns.Count - 1; x++)
                    {
                        oDr[x] = DaGriglia.Selected.Rows[0].Cells[x].Value;
                    }
                    oDsAGriglia.Tables[0].Rows.Add(oDr);
                    DaGriglia.Selected.Rows[0].Delete(false);
                }
            }
            else if (AGriglia.DataSource.GetType() == typeof(DataView))
            {
                DataView oDsAGriglia = (DataView)AGriglia.DataSource;
                while (DaGriglia.Selected.Rows.Count > 0)
                {
                    DataRowView oDr = oDsAGriglia.AddNew();
                    for (int x = 0; x <= oDsAGriglia.Table.Columns.Count - 1; x++)
                    {
                        oDr[x] = DaGriglia.Selected.Rows[0].Cells[x].Value;
                    }
                    oDr.EndEdit();
                    DaGriglia.Selected.Rows[0].Delete(false);
                }
            }

            AGriglia.Refresh();

            if (AGriglia.Rows.Count > 0)
            {
                AGriglia.Selected.Rows.Clear();
                AGriglia.ActiveRow = AGriglia.Rows.GetRowWithListIndex(AGriglia.Rows.Count - 1);
                AGriglia.ActiveRow.Selected = true;
            }

            this.uteCercaSX_ValueChanged(this.uteCercaSX, new EventArgs());
            this.uteCercaDX_ValueChanged(this.uteCercaDX, new EventArgs());

            if (GridChange != null) { GridChange(DaGriglia, new EventArgs()); }

        }

        #endregion

        #region Events

        private void ubInserisci_Click(object sender, EventArgs e)
        {

            try
            {

                if (this.UltraGridSX.Selected.Rows.Count > 0)
                {

                    CopyDaA(this.UltraGridSX, this.UltraGridDX);

                    if (this.UltraGridSX.Rows.Count > 0)
                    {
                        this.UltraGridSX.Selected.Rows.Clear();
                        this.UltraGridSX.ActiveRow = this.UltraGridSX.Rows.GetRowWithListIndex(0);
                        this.UltraGridSX.ActiveRow.Selected = true;
                    }

                }

            }
            catch (Exception)
            {

            }

        }

        private void ubInserisciAll_Click(object sender, EventArgs e)
        {

            try
            {

                this.UltraGridSX.Selected.Rows.Clear();
                foreach (UltraGridRow oRow in this.UltraGridSX.Rows)
                {
                    if (oRow.IsFilteredOut == false)
                    {
                        this.UltraGridSX.Selected.Rows.Add(oRow);
                    }
                }
                this.ubInserisci_Click(this.ubInserisci, new EventArgs());

            }
            catch (Exception)
            {

            }

        }

        private void ubCancellaAll_Click(object sender, EventArgs e)
        {

            try
            {

                this.UltraGridDX.Selected.Rows.Clear();
                foreach (UltraGridRow oRow in this.UltraGridDX.Rows)
                {
                    if (oRow.IsFilteredOut == false)
                    {
                        this.UltraGridDX.Selected.Rows.Add(oRow);
                    }
                }
                this.ubCancella_Click(this.ubCancella, new EventArgs());

            }
            catch (Exception)
            {

            }

        }

        private void ubCancella_Click(object sender, EventArgs e)
        {

            try
            {

                if (this.UltraGridDX.Selected.Rows.Count > 0)
                {

                    CopyDaA(this.UltraGridDX, this.UltraGridSX);

                    if (this.UltraGridDX.Rows.Count > 0)
                    {
                        this.UltraGridDX.Selected.Rows.Clear();
                        this.UltraGridDX.ActiveRow = this.UltraGridDX.Rows.GetRowWithListIndex(0);
                        this.UltraGridDX.ActiveRow.Selected = true;
                    }

                }

            }
            catch (Exception)
            {

            }

        }

        private void uteCercaSX_ValueChanged(object sender, EventArgs e)
        {

            try
            {

                string gridCaptionColumnKey = _gridSXCaptionColumnKey;
                string gridCaption = _gridSXCaption;

                if (gridCaptionColumnKey.Trim() == "") gridCaptionColumnKey = this.UltraGridSX.DisplayLayout.Bands[0].Columns[1].Key;
                if (gridCaption.Trim() == "") gridCaption = this.UltraGridSX.DisplayLayout.Bands[0].Columns[1].Key;

                string filtercolumnkey = this.UltraGridSX.DisplayLayout.Bands[0].Columns[1].Key;
                if (_gridSXFilterColumnIndex >= 0 && _gridSXFilterColumnIndex < this.UltraGridSX.DisplayLayout.Bands[0].Columns.Count)
                    filtercolumnkey = this.UltraGridSX.DisplayLayout.Bands[0].Columns[_gridSXFilterColumnIndex].Key;

                CoreStatics.SetGridWizardFilter(ref this.UltraGridSX,
                                                filtercolumnkey,
                                                this.uteCercaSX.Text,
                                                gridCaptionColumnKey,
                                                gridCaption);

            }
            catch (Exception)
            {

            }

        }

        private void uteCercaDX_ValueChanged(object sender, EventArgs e)
        {

            try
            {
                string gridCaptionColumnKey = _gridDXCaptionColumnKey;
                string gridCaption = _gridDXCaption;

                if (gridCaptionColumnKey.Trim() == "") gridCaptionColumnKey = this.UltraGridDX.DisplayLayout.Bands[0].Columns[1].Key;
                if (gridCaption.Trim() == "") gridCaption = this.UltraGridDX.DisplayLayout.Bands[0].Columns[1].Key;

                string filtercolumnkey = this.UltraGridSX.DisplayLayout.Bands[0].Columns[1].Key;
                if (_gridDXFilterColumnIndex >= 0 && _gridDXFilterColumnIndex < this.UltraGridDX.DisplayLayout.Bands[0].Columns.Count)
                    filtercolumnkey = this.UltraGridDX.DisplayLayout.Bands[0].Columns[_gridDXFilterColumnIndex].Key;

                CoreStatics.SetGridWizardFilter(ref this.UltraGridDX,
                                filtercolumnkey,
                                this.uteCercaDX.Text,
                                gridCaptionColumnKey,
                                gridCaption);

            }
            catch (Exception)
            {

            }

        }

        private void UltraGridSX_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (e.Row.IsDataRow)
            {
                CopyDaA(this.UltraGridSX, this.UltraGridDX);
            }
        }

        private void UltraGridSX_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            if (GridSXInitializeLayout != null) { GridSXInitializeLayout(this, e); }
        }

        private void UltraGridSX_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (GridSXInitializeRow != null) { GridSXInitializeRow(this, e); }
        }

        private void UltraGridDX_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
            if (GridDXDoubleClickCell != null) { GridDXDoubleClickCell(this, e); }
        }

        private void UltraGridDX_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (e.Row.IsDataRow)
            {
                CopyDaA(this.UltraGridDX, this.UltraGridSX);
            }
        }

        private void UltraGridDX_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            if (GridDXInitializeLayout != null) { GridDXInitializeLayout(this, e); }
        }

        private void UltraGridDX_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (GridDXInitializeRow != null) { GridDXInitializeRow(this, e); }
        }

        private void ubButton_Resize(object sender, EventArgs e)
        {
            try
            {
                if (((Infragistics.Win.Misc.UltraButton)sender).Appearance.Image != null)
                {
                    int iCtrlSize = ((Infragistics.Win.Misc.UltraButton)sender).Size.Height;
                    if (iCtrlSize > ((Infragistics.Win.Misc.UltraButton)sender).Size.Width) iCtrlSize = ((Infragistics.Win.Misc.UltraButton)sender).Size.Width;
                    ((Infragistics.Win.Misc.UltraButton)sender).ImageSize = new Size((int)(iCtrlSize * 0.95F), (int)(iCtrlSize * 0.95F));
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region Public Method

        public void RefreshData()
        {

            this.UltraGridSX.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Extended;
            this.UltraGridSX.Refresh();
            this.UltraGridSX.Text = "Elenco (" + this.UltraGridSX.Rows.Count().ToString() + ")";
            if (this.UltraGridSX.Rows.Count > 0)
            {
                this.UltraGridSX.ActiveRow = this.UltraGridSX.Rows.GetRowWithListIndex(0);
                this.UltraGridSX.ActiveRow.Selected = true;
            }
            this.UltraGridSX.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            this.UltraGridSX.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
            this.uteCercaSX_ValueChanged(this.uteCercaSX, new EventArgs());

            this.UltraGridDX.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Extended;
            this.UltraGridDX.Refresh();
            this.UltraGridDX.Text = "Elenco (" + this.UltraGridDX.Rows.Count().ToString() + ")";
            if (this.UltraGridDX.Rows.Count > 0)
            {
                this.UltraGridDX.ActiveRow = this.UltraGridDX.Rows.GetRowWithListIndex(0);
                this.UltraGridDX.ActiveRow.Selected = true;
            }
            this.UltraGridDX.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            this.UltraGridDX.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
            this.uteCercaDX_ValueChanged(this.uteCercaDX, new EventArgs());

        }

        #endregion

    }
}

