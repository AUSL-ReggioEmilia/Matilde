using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.Scci.Enums;

namespace UnicodeSrl.ScciCore
{
    public partial class frmProgress : Form
    {

        private Scci.Enums.enum_app_cursors _cursore = enum_app_cursors.DefaultCursor;

        public frmProgress()
        {
            InitializeComponent();
        }
       
        #region Metodi

        internal void InizializzaEMostra(int progressValoreMin, int progressValoreMAX, object controlloChiamante)
        {

            this.MyProgressBar.Minimum = progressValoreMin;
            this.MyProgressBar.Maximum = progressValoreMAX;
            this.MyProgressBar.Value = this.MyProgressBar.Minimum;
            this.lblInfo.Text = "";

            this.TopMost = true;
            Screen oScreen = Screen.PrimaryScreen;
            int nHeight = (oScreen.WorkingArea.Height / 100) * 15;
            int nWidth = oScreen.WorkingArea.Width;
            this.Size = new Size(nWidth, nHeight);

            this.Show();
            if (controlloChiamante is frmBaseModale)
                this.Location = new Point(0, (oScreen.Bounds.Height - nHeight));
            else
                this.Location = new Point(0, (oScreen.WorkingArea.Height - nHeight));

            CoreStatics.CoreApplication.Navigazione.Maschere.TracciaNavigazione("Firma Massiva", false);

            Application.DoEvents();

        }

        internal void SetInfo(string info, int indice)
        {
            this.MyProgressBar.Value = indice;
            this.lblInfo.Text = info;
            Application.DoEvents();
        }

        #endregion

    }
}
