using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win.UltraWinGrid;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmZoom : Form, Interfacce.IViewFormZoom
    {
        public frmZoom()
        {
            InitializeComponent();
        }

        #region Declare

        Sys.Data2008.SqlStruct _SqlStruct = new Sys.Data2008.SqlStruct();
        DataSet _DataSet = null;

        #endregion

        #region Interface

        public Sys.Data2008.SqlStruct ViewSqlStruct
        {
            get
            {
                return _SqlStruct;
            }
            set
            {
                _SqlStruct = value;
            }
        }

        public DataSet ViewDataSet
        {
            get
            {
                return _DataSet;
            }
            set
            {
                _DataSet = value;
            }
        }

        public Infragistics.Win.UltraWinGrid.UltraGridRow ViewActiveRow
        {
            get { return this.UltraGrid.ActiveRow; }
        }

        public Icon ViewIcon
        {
            get
            {
                return this.Icon;
            }
            set
            {
                this.Icon = value;
            }
        }

        public string ViewText
        {
            get
            {
                return this.Text;
            }
            set
            {
                this.Text = value;
            }
        }

        public void ViewInit()
        {

            this.SuspendLayout();

            MyStatics.SetUltraToolbarsManager(this.UltraToolbarsManager);
            MyStatics.SetUltraGridLayout(ref this.UltraGrid, true, false);

            this.LoadUltraGrid();

            this.ResumeLayout();

        }

        #endregion

        #region UltraGrid

        private void LoadUltraGrid()
        {

            try
            {

                if (this.ViewDataSet == null)
                {
                    this.UltraGrid.DataSource = DataBase.GetDataSet(this.ViewSqlStruct.Sql);
                }
                else
                {
                    this.UltraGrid.DataSource = this.ViewDataSet;
                }
                this.UltraGrid.Refresh();
                this.UltraGrid.Text = string.Format("{0} ({1:#,##0})", this.Text, this.UltraGrid.Rows.Count);

                this.UltraGrid.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        #endregion

        #region Events

        private void UltraGrid_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (e.Row.IsDataRow == true)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        #endregion

    }
}
