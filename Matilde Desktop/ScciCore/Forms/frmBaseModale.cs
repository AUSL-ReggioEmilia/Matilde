using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.ScciResource;
using Infragistics.Win.UltraWinGrid;
using UnicodeSrl.Scci.Enums;
using static UnicodeSrl.ScciCore.Interfacce;

namespace UnicodeSrl.ScciCore
{
    public partial class frmBaseModale : Form , IViewFormlModal
    {

        public frmBaseModale()
        {
            InitializeComponent();
        }

        #region     IViewFormlModal

                                public void Carica() { }


                                public Object CustomParamaters { get; set; }


        #endregion


        #region Declare

        public event ImmagineTopClickHandler ImmagineClick;

        public event PulsanteBottomClickHandler PulsanteIndietroClick;
        public event PulsanteBottomClickHandler PulsanteAvantiClick;

        private bool _fromevent = false;

                private const uint WM_NCLBUTTONDBLCLK = 0xA3;
        private const uint WM_NCLBUTTONDOWN = 0xA1;

        #endregion

        #region PROPRIETA

        public bool PulsanteAvantiVisibile
        {
            get
            {
                return this.ucBottomModale.ubAvanti.Visible;
            }
            set
            {
                this.ucBottomModale.ubAvanti.Visible = value;
            }
        }

        public bool PulsanteAvantiAbilitato
        {
            get
            {
                return this.ucBottomModale.ubAvanti.Enabled;
            }
            set
            {
                this.ucBottomModale.ubAvanti.Enabled = value;
            }
        }

        public string PulsanteAvantiTesto
        {
            get
            {
                return this.ucBottomModale.ubAvanti.Text;
            }
            set
            {
                this.ucBottomModale.ubAvanti.Text = value;
            }
        }

        public easyStatics.easyRelativeDimensions PulsanteAvantiTestoDimensione
        {
            get
            {
                return this.ucBottomModale.ubAvanti.TextFontRelativeDimension;
            }
            set
            {
                this.ucBottomModale.ubAvanti.TextFontRelativeDimension = value;
            }
        }

        public Keys PulsanteAvantiShortcutKey
        {
            get
            {
                return this.ucBottomModale.ubAvanti.ShortcutKey;
            }
            set
            {
                this.ucBottomModale.ubAvanti.ShortcutKey = value;
            }
        }

        public easyStatics.easyShortcutPosition PulsanteAvantiShortcutPosizione
        {
            get
            {
                return this.ucBottomModale.ubAvanti.ShortcutPosition;
            }
            set
            {
                this.ucBottomModale.ubAvanti.ShortcutPosition = value;
            }
        }

        public easyStatics.easyRelativeDimensions PulsanteAvantiShortcutDimensione
        {
            get
            {
                return this.ucBottomModale.ubAvanti.ShortcutFontRelativeDimension;
            }
            set
            {
                this.ucBottomModale.ubAvanti.ShortcutFontRelativeDimension = value;
            }
        }



        public bool PulsanteIndietroVisibile
        {
            get
            {
                return this.ucBottomModale.ubIndietro.Visible;
            }
            set
            {
                this.ucBottomModale.ubIndietro.Visible = value;
            }
        }

        public bool PulsanteIndietroAbilitato
        {
            get
            {
                return this.ucBottomModale.ubIndietro.Enabled;
            }
            set
            {
                this.ucBottomModale.ubIndietro.Enabled = value;
            }
        }

        public string PulsanteIndietroTesto
        {
            get
            {
                return this.ucBottomModale.ubIndietro.Text;
            }
            set
            {
                this.ucBottomModale.ubIndietro.Text = value;
            }
        }

        public easyStatics.easyRelativeDimensions PulsanteIndietroTestoDimensione
        {
            get
            {
                return this.ucBottomModale.ubIndietro.TextFontRelativeDimension;
            }
            set
            {
                this.ucBottomModale.ubIndietro.TextFontRelativeDimension = value;
            }
        }

        public Keys PulsanteIndietroShortcutKey
        {
            get
            {
                return this.ucBottomModale.ubIndietro.ShortcutKey;
            }
            set
            {
                this.ucBottomModale.ubIndietro.ShortcutKey = value;
            }
        }

        public easyStatics.easyShortcutPosition PulsanteIndietroShortcutPosizione
        {
            get
            {
                return this.ucBottomModale.ubIndietro.ShortcutPosition;
            }
            set
            {
                this.ucBottomModale.ubIndietro.ShortcutPosition = value;
            }
        }

        public easyStatics.easyRelativeDimensions PulsanteIndietroShortcutDimensione
        {
            get
            {
                return this.ucBottomModale.ubIndietro.ShortcutFontRelativeDimension;
            }
            set
            {
                this.ucBottomModale.ubIndietro.ShortcutFontRelativeDimension = value;
            }
        }

        public string CodiceMaschera
        {
            get { return this.ucTopModale.CodiceMaschera; }
            set { this.ucTopModale.CodiceMaschera = value; }
        }

        public Color TopModaleBackColor
        {
            get { return this.ucTopModale.BackColor; }
            set { this.ucTopModale.BackColor = value; }
        }

        public Color BottomBackColor
        {
            get { return this.ucBottomModale.BackColor; }
            set { this.ucBottomModale.BackColor = value; }
        }

        #endregion

        #region Subroutine

        public void ImpostaCursore(Scci.Enums.enum_app_cursors cursortype)
        {
            ImpostaCursore(cursortype, false);
        }
        public void ImpostaCursore(Scci.Enums.enum_app_cursors cursortype, bool skipDoEvents)
        {
            Form frmc = (Form)this;
            CoreStatics.impostaCursore(ref frmc, cursortype);
        }

        private void ResizeMain()
        {

            try
            {

                this.UltraPanelTop.Size = new Size(this.UltraPanelTop.Size.Width, Convert.ToInt16(Convert.ToDouble(this.Height) / 100D * 4D));
                this.UltraPanelBottom.Size = new Size(this.UltraPanelBottom.Size.Width, Convert.ToInt16(Convert.ToDouble(this.Height) / 100D * 4D));

            }
            catch (Exception)
            {

            }

        }

        #endregion

        #region Events Form

        private void frmBaseModale_Load(object sender, EventArgs e)
        {
            this.ResizeMain();

                        if (!this.DesignMode)
            {
                string fontFamily = null;
                                fontFamily = CoreStatics.getFontPredefinitoForm();
                if (fontFamily != null && fontFamily != "") this.Font = new Font(fontFamily, this.Font.Size, this.Font.Style);
            }

        }

        private void frmBaseModale_Resize(object sender, EventArgs e)
        {
            this.ResizeMain();
        }

        private void frmBaseModale_KeyDown(object sender, KeyEventArgs e)
        {
            ScciCore.easyStatics.checkShortcutKeyDown(e.KeyCode, this.Controls, false, e.Modifiers);
        }

        private void frmBaseModale_TextChanged(object sender, EventArgs e)
        {
                        try
            {
                if (!this.DesignMode && !_fromevent)
                {
                    if (this.Text.IndexOf(@" (Versione : ") < 0)
                    {
                        _fromevent = true;
                        try
                        {
                            this.Text += string.Format(" (Versione : {0})", Application.ProductVersion);
                        }
                        catch (Exception)
                        {
                        }
                        _fromevent = false;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

                protected override void WndProc(ref Message m)
        {

            switch ((uint)m.Msg)
            {
                case WM_NCLBUTTONDBLCLK:
                case WM_NCLBUTTONDOWN:
                    return;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        #endregion

        #region ucTopModale

        private void ucTopModale_ImmagineClick(object sender, ImmagineTopClickEventArgs e)
        {
            if (ImmagineClick != null) { ImmagineClick(sender, e); }
        }

        #endregion

        #region ucBottomModale

        private void ucBottomModale_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            if (PulsanteIndietroClick != null) { PulsanteIndietroClick(sender, e); }
        }

        private void ucBottomModale_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            if (PulsanteAvantiClick != null) { PulsanteAvantiClick(sender, e); }
        }

        #endregion

    }
}
