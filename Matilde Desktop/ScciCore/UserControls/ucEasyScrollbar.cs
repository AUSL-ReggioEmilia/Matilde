using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnicodeSrl.ScciResource;


namespace UnicodeSrl.ScciCore
{

    public partial class ucEasyScrollbar : UserControl, Interfacce.IEasyResizableText
    {
        #region DICHIARAZIONI

        private easyStatics.easyRelativeDimensions _textFontRelativeDimension = easyStatics.easyRelativeDimensions._undefined;

        public event ButtonEventHandler ButtonClick;
        public delegate void ButtonEventHandler(object sender, ucEasyScrollbarEventArgs e);

        #endregion

        public ucEasyScrollbar()
        {
            InitializeComponent();

            InizializzaPulsanti();
        }

        #region INTERFACCIA

        public easyStatics.easyRelativeDimensions TextFontRelativeDimension
        {
            get
            {
                return _textFontRelativeDimension;
            }
            set
            {
                _textFontRelativeDimension = value;

                this.ubGiu.TextFontRelativeDimension = _textFontRelativeDimension;
                this.ubPagGiu.TextFontRelativeDimension = _textFontRelativeDimension;
                this.ubPagSu.TextFontRelativeDimension = _textFontRelativeDimension;
                this.ubPrimo.TextFontRelativeDimension = _textFontRelativeDimension;
                this.ubRefresh.TextFontRelativeDimension = _textFontRelativeDimension;
                this.ubSu.TextFontRelativeDimension = _textFontRelativeDimension;
                this.ubUltimo.TextFontRelativeDimension = _textFontRelativeDimension;
            }
        }

        #endregion

        #region PROPRIETA'

        public float PercImageFill
        {
            get
            {
                return this.ubPrimo.PercImageFill;
            }
            set
            {
                this.ubGiu.PercImageFill = value;
                this.ubPagGiu.PercImageFill = value;
                this.ubPagSu.PercImageFill = value;
                this.ubPrimo.PercImageFill = value;
                this.ubRefresh.PercImageFill = value;
                this.ubSu.PercImageFill = value;
                this.ubUltimo.PercImageFill = value;
            }
        }

        #endregion

        #region PRIVATE

        private void InizializzaPulsanti()
        {
            try
            {
                this.ubGiu.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_SB_GIU);
                this.ubSu.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_SB_SU);
                this.ubPagGiu.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_SB_PAGGIU);
                this.ubPagSu.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_SB_PAGSU);
                this.ubPrimo.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_SB_PRIMO);
                this.ubUltimo.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_SB_ULTIMO);
                this.ubRefresh.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_AGGIORNA_256);
            }
            catch
            {
            }
        }

        #endregion

        #region EVENTI

        private void ubButton_Click(object sender, EventArgs e)
        {
            switch (((ucEasyButton)sender).Name)
            {

                case "ubUltimo":
                    if (ButtonClick != null) { ButtonClick(sender, new ucEasyScrollbarEventArgs(ucEasyScrollbarEventArgs.EnumTypeButton.Ultimo)); }
                    break;
                case "ubPagGiu":
                    if (ButtonClick != null) { ButtonClick(sender, new ucEasyScrollbarEventArgs(ucEasyScrollbarEventArgs.EnumTypeButton.PagGiu)); }
                    break;
                case "ubPagSu":
                    if (ButtonClick != null) { ButtonClick(sender, new ucEasyScrollbarEventArgs(ucEasyScrollbarEventArgs.EnumTypeButton.PagSu)); }
                    break;
                case "ubSu":
                    if (ButtonClick != null) { ButtonClick(sender, new ucEasyScrollbarEventArgs(ucEasyScrollbarEventArgs.EnumTypeButton.Su)); }
                    break;
                case "ubPrimo":
                    if (ButtonClick != null) { ButtonClick(sender, new ucEasyScrollbarEventArgs(ucEasyScrollbarEventArgs.EnumTypeButton.Primo)); }
                    break;
                case "ubRefresh":
                    if (ButtonClick != null) { ButtonClick(sender, new ucEasyScrollbarEventArgs(ucEasyScrollbarEventArgs.EnumTypeButton.Refresh)); }
                    break;
                case "ubGiu":
                    if (ButtonClick != null) { ButtonClick(sender, new ucEasyScrollbarEventArgs(ucEasyScrollbarEventArgs.EnumTypeButton.Giu)); }
                    break;

            }
        }

        #endregion

    }


    #region EVENT ARGS

    public class ucEasyScrollbarEventArgs : System.EventArgs
    {
        #region DICHIARAZIONI

        private EnumTypeButton _TypeButton = EnumTypeButton.Primo;

        public enum EnumTypeButton
        {
            Primo = 0,
            PagSu = 1,
            Su = 2,
            Giu = 3,
            PagGiu = 4,
            Ultimo = 5,
            Refresh = 6,
            Indietro = 7,
            Avanti = 8
        }

        #endregion

        public ucEasyScrollbarEventArgs(EnumTypeButton typeButton)
        {
            this._TypeButton = typeButton;
        }

        public EnumTypeButton TypeButton { get { return _TypeButton; } }

    }

    #endregion

}
