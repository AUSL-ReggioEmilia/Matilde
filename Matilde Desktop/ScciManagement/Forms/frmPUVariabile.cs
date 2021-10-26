using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UnicodeSrl.ScciManagement
{
    internal partial class frmPUVariabile : Form, Interfacce.IViewFormPUView
    {
        public frmPUVariabile()
        {
            InitializeComponent();
        }

        #region DECLARE

        private Enums.EnumModalityPopUp _Modality;

        DataTable _dtVariables = null;

        #endregion

        #region Interface

        public PUDataBindings ViewDataBindings
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public Enums.EnumDataNames ViewDataNamePU
        {
            get
            {
                return Enums.EnumDataNames.T_Report;
            }
            set
            {
            }
        }

        public Image ViewImage
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public Enums.EnumModalityPopUp ViewModality
        {
            get
            {
                return _Modality;
            }
            set
            {
                _Modality = value;
            }
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
            try
            {

                this.SuspendLayout();

                MyStatics.SetUltraToolbarsManager(this.ultraToolbarsManager);

                InitGrid();
                BindGrid();

                switch (_Modality)
                {
                    case Enums.EnumModalityPopUp.mpNuovo:
                        this.ubConferma.Enabled = true;
                        break;
                    case Enums.EnumModalityPopUp.mpModifica:
                        this.ubConferma.Enabled = true;
                        break;
                    case Enums.EnumModalityPopUp.mpCancella:
                        this.ubConferma.Enabled = false;
                        break;
                    case Enums.EnumModalityPopUp.mpVisualizza:
                        this.ubConferma.Enabled = false;
                        break;
                    default:
                        break;
                }

                this.ResumeLayout();

            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region PUBLIC

        public DataTable DataTableVariables
        {
            get
            { return _dtVariables; }
            set
            { _dtVariables = value; }
        }

        #endregion

        #region PRIVATE

        private void InitGrid()
        {
            try
            {
                MyStatics.SetUltraGridLayout(ref this.ultraGridVariabili, false, false);
                this.ultraGridVariabili.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
                this.ultraGridVariabili.DisplayLayout.GroupByBox.Hidden = true;
                this.ultraGridVariabili.DisplayLayout.Override.ColumnAutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.VisibleRows;
                this.ultraGridVariabili.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;

                if (_Modality == Enums.EnumModalityPopUp.mpNuovo || _Modality == Enums.EnumModalityPopUp.mpModifica)
                {
                    this.ultraGridVariabili.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.FixedAddRowOnTop;
                    this.ultraGridVariabili.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
                    this.ultraGridVariabili.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
                    this.ultraGridVariabili.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
                    this.ultraGridVariabili.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
                }


            }
            catch (Exception)
            {
            }
        }

        private void BindGrid()
        {
            try
            {
                this.ultraGridVariabili.DataSource = _dtVariables;

                if (this.ultraGridVariabili.DisplayLayout.Bands[0].Columns.Exists("CODICE"))
                {
                    this.ultraGridVariabili.DisplayLayout.Bands[0].Columns["CODICE"].Header.Caption = "Campo";
                }
                if (this.ultraGridVariabili.DisplayLayout.Bands[0].Columns.Exists("DESCRIZIONE"))
                {
                    this.ultraGridVariabili.DisplayLayout.Bands[0].Columns["DESCRIZIONE"].Header.Caption = "Descrizione";
                }

            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region EVENTI

        private void frmPUVariabile_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.ultraGridVariabili != null) this.ultraGridVariabili.Dispose();
        }

        private void ubConferma_Click(object sender, EventArgs e)
        {
            try
            {
                this.ultraGridVariabili.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.CommitRow);

                _dtVariables = this.ultraGridVariabili.DataSource as DataTable;

                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private void ubAnnulla_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion

    }
}
