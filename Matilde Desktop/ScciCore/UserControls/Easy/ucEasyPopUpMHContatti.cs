using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinEditors;

namespace UnicodeSrl.ScciCore
{
    public partial class ucEasyPopUpMHContatti : UserControl
    {

        #region Declare

        public event EventHandler ConfermaClick;
        public event EventHandler AnnullaClick;

        #endregion

        public ucEasyPopUpMHContatti()
        {
            InitializeComponent();
        }

        #region Properties

        public string ID { get; set; }

        public string NumeroTelefono
        {
            get
            {
                return this.txtNumeroTelefono.Text;
            }
            set
            {
                this.txtNumeroTelefono.Text = value;
            }
        }

        public string Descrizione
        {
            get
            {
                return this.txtDescrizione.Text;
            }
            set
            {
                this.txtDescrizione.Text = value;
            }
        }

        #endregion

        #region Subroutine

        private void Check()
        {

            bool bCheck = false;

            if (this.txtNumeroTelefono.Text != string.Empty && this.txtDescrizione.Text != string.Empty)
            {
                bCheck = true;
            }

            this.ubConferma.Enabled = bCheck;

        }

        #endregion

        #region Events

        private void txtNumeroTelefono_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '+')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '+' && (sender as UltraTextEditor).Text.Length > 0)
            {
                e.Handled = true;
            }

            if (e.KeyChar == '+' && (sender as UltraTextEditor).Text.IndexOf('+') > -1)
            {
                e.Handled = true;
            }

        }

        private void txtCampo_ValueChanged(object sender, EventArgs e)
        {
            this.Check();
        }

        private void ubConferma_Click(object sender, EventArgs e)
        {
            if (ConfermaClick != null) { ConfermaClick(sender, e); }

        }

        private void ubAnnulla_Click(object sender, EventArgs e)
        {
            if (AnnullaClick != null) { AnnullaClick(sender, e); }
        }

        #endregion

    }
}
