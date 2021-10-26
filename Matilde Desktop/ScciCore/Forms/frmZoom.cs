using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnicodeSrl.ScciCore
{
    public partial class frmZoom : frmBaseModale, Interfacce.IViewFormlModal
    {

        #region Declare

        private Object _customparamatersoutput = null;

        #endregion

        public frmZoom()
        {
            InitializeComponent();
        }

        #region Interface

        internal bool MultiSelezione { get; set; }

        internal Object CustomParamatersOutput
        {
            get
            {
                return _customparamatersoutput;
            }
        }

        public new void Carica()
        {

            try
            {

                this.InitializeUltraGrid();
                this.LoadUltraGrid();

                this.ucBottomModale.ubAvanti.Enabled = false;

                this.ShowDialog();

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "Carica", this.Text);
            }

        }

        #endregion

        #region UltraGrid

        private void InitializeUltraGrid()
        {
            if (this.MultiSelezione) { this.UltraGrid.SelectionStrategyFilter = new SelectionStrategyFilter(this.UltraGrid); }
            this.UltraGrid.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Large;
        }

        private void LoadUltraGrid()
        {

            try
            {

                this.UltraGrid.DataSource = CustomParamaters;
                this.UltraGrid.Refresh();

                this.UltraGrid.Selected.Rows.Clear();
                this.UltraGrid.ActiveRow = null;

                this.UltraGrid.SyncWithCurrencyManager = false;
                this.UltraGrid.DisplayLayout.Override.RowSelectors = DefaultableBoolean.False;

                this.UltraGrid.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);
                this.UltraGrid.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "LoadUltraGrid", this.Name);
            }

        }

        #endregion

        #region Events Form

        private void frmZoom_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {

            try
            {

                if (this.UltraGrid.ActiveRow != null)
                {
                    _customparamatersoutput = this.UltraGrid.ActiveRow.ListObject;
                }
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void frmZoom_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            _customparamatersoutput = null;
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion

        #region Events

        private void UltraGrid_AfterRowActivate(object sender, EventArgs e)
        {
        }

        private void UltraGrid_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {

                e.Layout.Bands[0].ColHeadersVisible = false;

                foreach (UltraGridColumn ocol in e.Layout.Bands[0].Columns)
                {

                    switch (ocol.Key)
                    {

                        case "Descrizione":
                            ocol.Hidden = false;
                            ocol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                            ocol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                            break;

                        default:
                            ocol.Hidden = true;
                            break;

                    }

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "UltraGrid_InitializeLayout", this.Name);
            }

        }

        private void UltraGrid_MouseDown(object sender, MouseEventArgs e)
        {

            ucEasyGrid grid = sender as ucEasyGrid;

            if (this.MultiSelezione)
            {
                _customparamatersoutput = grid.Selected.Rows.OfType<UltraGridRow>().Select(r => r.ListObject).ToList();
            }
            else
            {
                _customparamatersoutput = (grid.Selected.Rows.Count == 1 ? grid.Selected.Rows[0].ListObject : null);
            }

            ucBottomModale.ubAvanti.Enabled = (grid.Selected.Rows.Count > 0);

        }

        #endregion

    }
}
