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
    public partial class ucRtf : UserControl, Interfacce.IViewUserControlMiddle
    {
        public ucRtf()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
        }

        #region Declare

        public event ClickCellEventHandler ClickCell;
        public event InitializeLayoutEventHandler InitializeLayout;
        public event InitializeRowEventHandler InitializeRow;

        #endregion

        #region Interface

        public void Aggiorna()
        {

        }

        public void Carica()
        {

        }

        public void Ferma()
        {

            try
            {

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Ferma", this.Name);
            }

        }

        #endregion

        #region Property

        public Image Immagine
        {
            get
            {
                return this.ucEasyPictureBox.Image;
            }
            set
            {
                this.ucEasyPictureBox.Image = value;
            }
        }

        public string Titolo
        {
            get
            {
                return this.ucEasyLabelTitolo.Text;
            }
            set
            {
                this.ucEasyLabelTitolo.Text = value;
            }
        }

        public DataTable Dati
        {
            get
            {
                return (DataTable)this.ucEasyGrid.DataSource;
            }
            set
            {
                this.SuspendLayout();
                this.ucEasyGrid.DataSource = value;
                this.ucEasyGrid.Refresh();
                this.ucEasyGrid.DisplayLayout.Bands[0].ColHeadersVisible = false;
                this.ResumeLayout();
            }
        }

        public string ColonnaRTFResize
        {
            get
            {
                return this.ucEasyGrid.ColonnaRTFResize;
            }
            set
            {
                this.ucEasyGrid.ColonnaRTFResize = value;
            }
        }

        public int FattoreRidimensionamentoRTF
        {
            get
            {
                return this.ucEasyGrid.FattoreRidimensionamentoRTF;
            }
            set
            {
                this.ucEasyGrid.FattoreRidimensionamentoRTF = value;
            }
        }

        #endregion

        #region Events

        private void ucEasyGrid_AfterRowActivate(object sender, EventArgs e)
        {



            bool bcancel = true;

            if (this.ColonnaRTFResize != null
                && this.ColonnaRTFResize != string.Empty
                && this.ColonnaRTFResize.Trim() != ""
                && this.ucEasyGrid.DisplayLayout.Bands[0].Columns.Exists(this.ColonnaRTFResize)
                && this.ucEasyGrid.DisplayLayout.Bands[0].Columns[this.ColonnaRTFResize].CellClickAction == CellClickAction.EditAndSelectText)
                bcancel = false;

            if (bcancel)
                ucEasyGrid.ActiveRow = null;
            else
            {
                try
                {
                    if (this.ucEasyGrid.ActiveRow != null && (this.ucEasyGrid.ActiveRow.Index == 0 || this.ucEasyGrid.ActiveRow.Index % 2 == 0))
                    {
                        this.ucEasyGrid.DisplayLayout.Override.ActiveRowAppearance.BackColor = this.ucEasyGrid.DisplayLayout.Override.RowAppearance.BackColor;
                        this.ucEasyGrid.DisplayLayout.Override.ActiveRowAppearance.BorderColor = this.ucEasyGrid.DisplayLayout.Override.RowAppearance.BorderColor;
                    }
                    else
                    {
                        this.ucEasyGrid.DisplayLayout.Override.ActiveRowAppearance.BackColor = this.ucEasyGrid.DisplayLayout.Override.RowAlternateAppearance.BackColor;
                        this.ucEasyGrid.DisplayLayout.Override.ActiveRowAppearance.BorderColor = this.ucEasyGrid.DisplayLayout.Override.RowAlternateAppearance.BorderColor;
                    }
                    this.ucEasyGrid.DisplayLayout.Override.ActiveRowAppearance.ForeColor = System.Drawing.SystemColors.ControlText;
                    this.ucEasyGrid.DisplayLayout.Override.ActiveCellAppearance.BackColor = this.ucEasyGrid.DisplayLayout.Override.CellAppearance.BackColor;
                    this.ucEasyGrid.DisplayLayout.Override.ActiveCellAppearance.ForeColor = System.Drawing.SystemColors.ControlText;
                    this.ucEasyGrid.DisplayLayout.Override.ActiveCellAppearance.BorderColor = this.ucEasyGrid.DisplayLayout.Override.CellAppearance.BorderColor;
                }
                catch (Exception)
                {
                }
            }
        }

        private void ucEasyGrid_ClickCell(object sender, ClickCellEventArgs e)
        {
            if (ClickCell != null) { ClickCell(sender, e); }
        }

        private void ucEasyGrid_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            if (InitializeLayout != null) { InitializeLayout(sender, e); }
        }

        private void ucEasyGrid_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            if (InitializeRow != null) { InitializeRow(sender, e); }
        }

        #endregion

    }
}
