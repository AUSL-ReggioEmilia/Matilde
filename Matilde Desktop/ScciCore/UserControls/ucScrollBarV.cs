using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win.Misc;
using UnicodeSrl.ScciResource;

namespace UnicodeSrl.ScciCore
{
    public partial class ucScrollBarV : UserControl
    {
        public ucScrollBarV()
        {
            InitializeComponent();
        }

        #region Declare

        public event ButtonEventHandler Button;
        public delegate void ButtonEventHandler(object sender, ScrollbarEventArgs e);

        #endregion

        #region Subroutine

        private void SetStyleUcScrollBarV()
        {

            try
            {

                var otlp = this.TableLayoutPanel;
                var oubsu = this.ubSu;
                var oubgiu = this.ubGiu;

                otlp.BackColor = Color.Transparent;

                oubsu.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FRECCIASU_48);
                oubsu.Appearance.ImageBackground = null;
                oubsu.Appearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                oubsu.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                oubsu.Appearance.ThemedElementAlpha = Infragistics.Win.Alpha.Default;
                oubsu.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2007RibbonButton;
                oubsu.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;

                oubgiu.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FRECCIAGIU_48);
                oubgiu.Appearance.ImageBackground = null;
                oubgiu.Appearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                oubgiu.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                oubgiu.Appearance.ThemedElementAlpha = Infragistics.Win.Alpha.Default;
                oubgiu.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2007RibbonButton;
                oubgiu.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;

            }
            catch (Exception)
            {

            }

        }

        private void SetResizeUcScrollBarV()
        {

            try
            {

                this.ResizeImageHW(ref this.ubSu, 20, 20);
                this.ResizeImageHW(ref this.ubGiu, 20, 20);

            }
            catch (Exception)
            {

            }

        }


        private void ResizeImageHW(ref UltraButton oUb, int MarginW, int MarginH)
        {

            try
            {

                if (oUb.Appearance.Image != null)
                {

                    int nH = (oUb.Height - Convert.ToInt32((Convert.ToDouble(oUb.Height) / 100 * Convert.ToDouble(MarginH))));
                    int nW = (oUb.Width - Convert.ToInt32((Convert.ToDouble(oUb.Width) / 100 * Convert.ToDouble(MarginW))));
                    if (nH < nW)
                    {
                        nW = nH;
                    }
                    else
                    {
                        nH = nW;
                    }

                    oUb.ImageSize = new Size(nW, nH);

                }

            }
            catch (Exception)
            {

            }

        }

        #endregion

        #region Events ucScrollBarV

        private void TableLayoutPanel_Resize(object sender, EventArgs e)
        {

            try
            {

                var otlp = this.TableLayoutPanel;
                int nWidth = this.Size.Width;
                if (nWidth > (this.Size.Height / 2)) { nWidth = this.Size.Height / 2; }
                otlp.RowStyles[0].Height = nWidth;
                otlp.RowStyles[otlp.RowCount - 1].Height = nWidth;
                this.SetStyleUcScrollBarV();
                this.SetResizeUcScrollBarV();

            }
            catch (Exception)
            {

            }

        }

        #endregion

        #region Events

        private void UltraButton_Click(object sender, EventArgs e)
        {

            switch (((UltraButton)sender).Name)
            {

                case "ubSu":
                    if (Button != null) { Button(sender, new ScrollbarEventArgs(ScrollbarEventArgs.EnumTypeButton.Su)); }
                    break;

                case "ubGiu":
                    if (Button != null) { Button(sender, new ScrollbarEventArgs(ScrollbarEventArgs.EnumTypeButton.Giu)); }
                    break;

            }

        }

        #endregion

    }
}
