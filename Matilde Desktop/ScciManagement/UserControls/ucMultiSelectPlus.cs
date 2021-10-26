using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.ScciCore;
using Infragistics.Win.UltraWinGrid;

namespace UnicodeSrl.ScciManagement
{
    public partial class ucMultiSelectPlus : UserControl
    {
        public ucMultiSelectPlus()
        {
            InitializeComponent();
        }

        #region Declare

        DataSet _ViewDataSetSX = null;
        DataSet _ViewDataSetDX = null;

        public event InitializeLayoutEventHandler GridMasterInitializeLayout;
        public event InitializeRowEventHandler GridMasterInitializeRow;

        public event InitializeLayoutEventHandler GridSXInitializeLayout;
        public event InitializeRowEventHandler GridSXInitializeRow;

        public event InitializeLayoutEventHandler GridDXInitializeLayout;
        public event InitializeRowEventHandler GridDXInitializeRow;

        public event ChangeEventHandler GridChange;

        #endregion

        #region Interface

        public bool ViewShowFind
        {
            get
            {
                return this.ucMultiSelect.ViewShowFind;
            }
            set
            {
                this.ucMultiSelect.ViewShowFind = value;
            }
        }

        public bool ViewShowAll
        {
            get
            {
                return this.ucMultiSelect.ViewShowAll;
            }
            set
            {
                this.ucMultiSelect.ViewShowAll = value;
            }
        }

        public DataSet ViewDataSetMaster
        {
            get
            {
                return (DataSet)this.UltraGridMaster.DataSource;
            }
            set
            {
                this.UltraGridMaster.DataSource = value;
            }
        }

        public DataSet ViewDataSetSX
        {
            get
            {
                return _ViewDataSetSX;
            }
            set
            {
                _ViewDataSetSX = value;
            }
        }

        public DataSet ViewDataSetDX
        {
            get
            {
                return _ViewDataSetDX;
            }
            set
            {
                _ViewDataSetDX = value;
            }
        }

        public void ViewInit()
        {

            MyStatics.SetUltraGridLayout(ref this.UltraGridMaster, false, false);
            this.UltraGridMaster.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            this.UltraGridMaster.DisplayLayout.GroupByBox.Hidden = true;
            this.UltraGridMaster.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            this.ucMultiSelect.ViewInit();

            if (this.UltraGridMaster.Rows.Count > 0)
                this.UltraGridMaster.ActiveRow = this.UltraGridMaster.Rows[0];

        }

        public string GridSXCaptionColumnKey
        {
            get
            {
                return this.ucMultiSelect.GridSXCaptionColumnKey;
            }
            set
            {
                this.ucMultiSelect.GridSXCaptionColumnKey = value;
            }
        }

        public string GridSXCaption
        {
            get
            {
                return this.ucMultiSelect.GridSXCaption;
            }
            set
            {
                this.ucMultiSelect.GridSXCaption = value;
            }
        }

        public int GridSXFilterColumnIndex
        {
            get
            {
                return this.ucMultiSelect.GridSXFilterColumnIndex;
            }
            set
            {
                this.ucMultiSelect.GridSXFilterColumnIndex = value;
            }
        }

        public string GridDXCaptionColumnKey
        {
            get
            {
                return this.ucMultiSelect.GridDXCaptionColumnKey;
            }
            set
            {
                this.ucMultiSelect.GridDXCaptionColumnKey = value;
            }
        }

        public string GridDXCaption
        {
            get
            {
                return this.ucMultiSelect.GridDXCaption;
            }
            set
            {
                this.ucMultiSelect.GridDXCaption = value;
            }
        }

        public int GridDXFilterColumnIndex
        {
            get
            {
                return this.ucMultiSelect.GridDXFilterColumnIndex;
            }
            set
            {
                this.ucMultiSelect.GridDXFilterColumnIndex = value;
            }
        }

        #endregion

        #region UltraGrid

        public UltraGrid GridMaster
        {
            get { return this.UltraGridMaster; }
        }

        public UltraGrid GridDX
        {
            get { return this.ucMultiSelect.GridDX; }
        }

        public UltraGrid GridSX
        {
            get { return this.ucMultiSelect.GridSX; }
        }

        #endregion

        #region Public Method

        public void RefreshData()
        {
            this.ucMultiSelect.RefreshData();
        }

        #endregion

        #region Events

        private void UltraGridMaster_AfterRowActivate(object sender, EventArgs e)
        {

            var strExpr = "CodAzione = '" + this.UltraGridMaster.ActiveRow.Cells["CodAzione"].Value.ToString() + "'";
            _ViewDataSetSX.Tables[0].DefaultView.RowFilter = strExpr;
            _ViewDataSetDX.Tables[0].DefaultView.RowFilter = strExpr;

            this.ucMultiSelect.ViewDataViewSX = _ViewDataSetSX.Tables[0].DefaultView;
            this.ucMultiSelect.ViewDataViewDX = _ViewDataSetDX.Tables[0].DefaultView;

            this.ucMultiSelect.RefreshData();

        }

        private void ucEasyGridMaster_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            if (GridMasterInitializeLayout != null) { GridMasterInitializeLayout(this, e); }
        }

        private void ucEasyGridMaster_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (GridMasterInitializeRow != null) { GridMasterInitializeRow(this, e); }
        }

        private void ucMultiSelect_GridChange(object sender, EventArgs e)
        {
            if (GridChange != null) { GridChange(sender, e); }
        }

        private void ucMultiSelect_GridSXInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            if (GridSXInitializeLayout != null) { GridSXInitializeLayout(this, e); }
        }

        private void ucMultiSelect_GridSXInitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (GridSXInitializeRow != null) { GridSXInitializeRow(this, e); }
        }

        private void ucMultiSelect_GridDXInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            if (GridDXInitializeLayout != null) { GridDXInitializeLayout(this, e); }
        }

        private void ucMultiSelect_GridDXInitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (GridDXInitializeRow != null) { GridDXInitializeRow(this, e); }
        }

        #endregion

    }
}
