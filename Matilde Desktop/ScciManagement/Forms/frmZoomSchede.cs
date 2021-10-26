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
    public partial class frmZoomSchede : Form, Interfacce.IViewFormZoom
    {
        public frmZoomSchede()
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
            this.UltraGrid.DisplayLayout.GroupByBox.Hidden = true;

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

        private void UltraGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {


                if (e.Layout.Bands[1].Columns.Exists("CodScheda") == true)
                {
                    e.Layout.Bands[1].Columns["CodScheda"].Hidden = true;
                }
                if (e.Layout.Bands[1].Columns.Exists("Versione") == true)
                {
                    e.Layout.Bands[1].Columns["Versione"].Hidden = true;
                }

                if (e.Layout.Bands[2].Columns.Exists("CodScheda") == true)
                {
                    e.Layout.Bands[2].Columns["CodScheda"].Hidden = true;
                }
                if (e.Layout.Bands[2].Columns.Exists("Versione") == true)
                {
                    e.Layout.Bands[2].Columns["Versione"].Hidden = true;
                }
                if (e.Layout.Bands[2].Columns.Exists("IDSezione") == true)
                {
                    e.Layout.Bands[2].Columns["IDSezione"].Hidden = true;
                }

            }
            catch (Exception)
            {

            }

        }

        #endregion

        private void ubAnnulla_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void ubConferma_Click(object sender, EventArgs e)
        {

            if (this.UltraGrid.ActiveRow != null && this.UltraGrid.ActiveRow.IsDataRow && this.UltraGrid.ActiveRow.Band.Index == 1)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Selezionare una riga di sezione!!!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

    }
}
