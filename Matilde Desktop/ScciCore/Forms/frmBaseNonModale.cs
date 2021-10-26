using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UnicodeSrl.ScciCore
{
    public partial class frmBaseNonModale : Form
    {
        public frmBaseNonModale()
        {
            InitializeComponent();
        }

        #region Declare

        public event PulsanteBottomClickHandler PulsanteIndietroClick;
        public event PulsanteBottomClickHandler PulsanteAvantiClick;

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

        private void frmBaseNonModale_Load(object sender, EventArgs e)
        {

            this.ResizeMain();

                        if (!this.DesignMode)
            {
                string fontFamily = null;
                                fontFamily = CoreStatics.getFontPredefinitoForm();
                if (fontFamily != null && fontFamily != "") this.Font = new Font(fontFamily, this.Font.Size, this.Font.Style);
            }

        }
      
        private void frmBaseNonModale_Resize(object sender, EventArgs e)
        {
            this.ResizeMain();
        }

        private void frmBaseNonModale_KeyDown(object sender, KeyEventArgs e)
        {
            ScciCore.easyStatics.checkShortcutKeyDown(e.KeyCode, this.Controls, false, e.Modifiers);
        }

        private void frmBaseNonModale_TextChanged(object sender, EventArgs e)
        {
                        try
            {
                if (!this.DesignMode)
                {
                    if (this.Text.IndexOf(@" (Versione : ") < 0)
                    {
                        try
                        {
                            this.Text += string.Format(" (Versione : {0})", Application.ProductVersion);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region ucTopModale

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
