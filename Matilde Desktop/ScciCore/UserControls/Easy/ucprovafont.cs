using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Data.SqlClient;
using System.Globalization;
using Infragistics.Win.UltraWinGrid;
using UnicodeSrl.Fw2010.Data;
using UnicodeSrl.ScciResource;
using UnicodeSrl.ScciCore.CustomControls.Infra;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;
using System.IO;

namespace UnicodeSrl.ScciCore
{
    public partial class ucprovafont : UserControl, Interfacce.IViewUserControlMiddle
    {

        private UserControl _ucc = null;
        private easyStatics.easyRelativeDimensions _lastfont = easyStatics.easyRelativeDimensions.XXXHUGE;
        private float _lastfontS = 30F;

        public ucprovafont()
        {
            InitializeComponent();
            _ucc = (UserControl)this;
        }

        #region INTERFACCIA

        public void Aggiorna()
        {

                        CoreStatics.SetNavigazione(false);

            try
            {

                switch (_lastfont)
                {
                    case easyStatics.easyRelativeDimensions.XXXLarge:
                        _lastfont = easyStatics.easyRelativeDimensions.XXXXLarge;
                        break;
                    case easyStatics.easyRelativeDimensions.XXXXLarge:
                        _lastfont = easyStatics.easyRelativeDimensions.HUGE;
                        break;
                    case easyStatics.easyRelativeDimensions.HUGE:
                        _lastfont = easyStatics.easyRelativeDimensions.XHUGE;
                        break;
                    case easyStatics.easyRelativeDimensions.XHUGE:
                        _lastfont = easyStatics.easyRelativeDimensions.XXHUGE;
                        break;
                    case easyStatics.easyRelativeDimensions.XXHUGE:
                        _lastfont = easyStatics.easyRelativeDimensions.XXXHUGE;
                        break;
                    case easyStatics.easyRelativeDimensions.XXXHUGE:
                        _lastfont = easyStatics.easyRelativeDimensions.XXXXHUGE;
                        break;
                    default:
                        _lastfont = easyStatics.easyRelativeDimensions.XXXLarge;
                        break;
                }

                this.lblTEST.TextFontRelativeDimension = _lastfont;
                this.lblfontsize.Text = _lastfont.ToString() + @" [" + easyStatics.getFontSizeInPointsFromRelativeDimension(_lastfont).ToString(@"#,##0.00") + @"]";

                
                
                                
            }
            catch (Exception ex)
            {
                Fw2010.Diagnostics.Statics.AddDebugInfo(ex);
            }

                        CoreStatics.SetNavigazione(true);

        }

        public void Carica()
        {
            try
            {
                InizializzaControlli();
                
                this.Aggiorna();
                this.cmdRicerca.Focus();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Carica", this.Name);
            }

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

        #region FUNZIONI

        private void InizializzaControlli()
        {

            try
            {
                this.lblTEST.TextFontRelativeDimension = easyStatics.easyRelativeDimensions._undefined;
                this.lblTEST.AutoEllipsis = false;

            }
            catch (Exception)
            {
            }

        }


        #endregion

        #region EVENTI

        private void uteRicerca_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                                if (e.KeyCode == Keys.Enter) this.Aggiorna();
            }
            catch (Exception)
            {
            }
        }

        private void cmdRicerca_Click(object sender, EventArgs e)
        {
            this.Aggiorna();
            
        }

        #endregion

    }
}
