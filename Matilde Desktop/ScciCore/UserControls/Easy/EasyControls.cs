using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using UnicodeSrl.ScciCore.Extensions;

namespace UnicodeSrl.ScciCore
{

    public class ucEasyButton : Infragistics.Win.Misc.UltraButton, Interfacce.IEasyShortcut, Interfacce.IEasyResizableText
    {

        #region Declare

        private easyStatics.easyRelativeDimensions _shortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
        private easyStatics.easyShortcutPosition _shortcutPosition = easyStatics.easyShortcutPosition.top_right;
        private Keys _shortcutKey = Keys.None;
        private Color _shortcutColor = Color.Black;

        private easyStatics.easyRelativeDimensions _textFontRelativeDimension = easyStatics.easyRelativeDimensions._undefined;

        private float _percImageFill = 0.75F;

        #endregion

        public ucEasyButton()
        {

            this.UseOsThemes = DefaultableBoolean.False;
            this.ButtonStyle = UIElementButtonStyle.Office2003ToolbarButton;

            this.Appearance.TextHAlign = HAlign.Center;
            this.Appearance.TextVAlign = VAlign.Bottom;
            this.Appearance.ImageHAlign = HAlign.Center;
            this.Appearance.ImageVAlign = VAlign.Top;

        }

        #region INTERFACE

        public easyStatics.easyRelativeDimensions ShortcutFontRelativeDimension
        {
            get
            {
                return _shortcutFontRelativeDimension;
            }
            set
            {
                _shortcutFontRelativeDimension = value;
                drawShortcut();
            }
        }

        public easyStatics.easyShortcutPosition ShortcutPosition
        {
            get
            {
                return _shortcutPosition;
            }
            set
            {
                _shortcutPosition = value;
                drawShortcut();
            }
        }

        public Keys ShortcutKey
        {
            get
            {
                return _shortcutKey;
            }
            set
            {
                _shortcutKey = value;
                drawShortcut();
            }
        }

        public easyStatics.easyRelativeDimensions TextFontRelativeDimension
        {
            get
            {
                return _textFontRelativeDimension;
            }
            set
            {
                _textFontRelativeDimension = value;
                setTextFont();
            }
        }

        public Color ShortcutColor
        {
            get
            {
                return _shortcutColor;
            }
            set
            {
                _shortcutColor = value;
            }
        }

        public void PerformActionShortcut()
        {
            this.OnClick(EventArgs.Empty);
        }

        #endregion

        #region PROPERTIES

        public float PercImageFill
        {
            get
            {
                return _percImageFill;
            }
            set
            {
                _percImageFill = value;
                resizeImage();
            }
        }

        #endregion

        #region PRIVATE SUB

        private void drawShortcut()
        {
            try
            {
                if (_shortcutKey == Keys.None)
                {
                    this.Appearance.ImageBackground = null;
                }
                else
                {
                    float fFontSize = this.Appearance.FontData.SizeInPoints;
                    if (_shortcutFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined) fFontSize = easyStatics.getFontSizeInPointsFromRelativeDimension(_shortcutFontRelativeDimension);
                    if (_shortcutColor == null) _shortcutColor = Color.Black;

                    string fontFamily = this.Appearance.FontData.Name;
                    if (fontFamily == null || fontFamily == "") fontFamily = "Calibri";
                    Font f = new Font(fontFamily, fFontSize, FontStyle.Regular, GraphicsUnit.Pixel);

                    this.Appearance.ImageBackground = easyStatics.drawBmpText(this.Size, easyStatics.getTextFromKey(_shortcutKey), _shortcutColor, f, _shortcutPosition);
                }

            }
            catch (Exception)
            {
            }
        }

        private void setTextFont()
        {
            try
            {
                if (_textFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined)
                {
                    this.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(_textFontRelativeDimension);
                }
                else
                    this.Appearance.FontData.SizeInPoints = 0;
            }
            catch (Exception)
            {
            }
        }

        private void resizeImage()
        {
            try
            {
                if (this.Appearance.Image != null && _percImageFill > 0)
                {
                    int iCtrlSize = this.Size.Height;
                    if (iCtrlSize > this.Size.Width) iCtrlSize = this.Size.Width;
                    this.ImageSize = new Size((int)(iCtrlSize * _percImageFill), (int)(iCtrlSize * _percImageFill));
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region OVERRIDE EVENTS

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);


            drawShortcut();
            setTextFont();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            resizeImage();
        }

        #endregion
    }

    public class ucEasyProgressBar : Infragistics.Win.UltraWinProgressBar.UltraProgressBar, Interfacce.IEasyResizableText
    {
        #region Declare

        private easyStatics.easyRelativeDimensions _textFontRelativeDimension = easyStatics.easyRelativeDimensions._undefined;

        #endregion

        public ucEasyProgressBar()
        {
            this.UseOsThemes = DefaultableBoolean.False;
            this.AutoSize = false;
        }

        #region INTERFACE

        public easyStatics.easyRelativeDimensions TextFontRelativeDimension
        {
            get
            {
                return _textFontRelativeDimension;
            }
            set
            {
                _textFontRelativeDimension = value;
                setTextFont();
            }
        }

        #endregion

        #region PRIVATE SUB

        private void setTextFont()
        {
            try
            {
                if (_textFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined)
                {
                    this.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(_textFontRelativeDimension);
                }
                else
                    this.Appearance.FontData.SizeInPoints = 0;
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region OVERRIDE EVENTS

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            setTextFont();
        }

        #endregion

    }

    public class ucEasyTextBox : Infragistics.Win.UltraWinEditors.UltraTextEditor, Interfacce.IEasyResizableText
    {
        #region Declare

        private easyStatics.easyRelativeDimensions _textFontRelativeDimension = easyStatics.easyRelativeDimensions._undefined;

        #endregion

        public ucEasyTextBox()
        {
            this.UseOsThemes = DefaultableBoolean.False;
            this.DisplayStyle = EmbeddableElementDisplayStyle.Office2007;
            this.AutoSize = false;
        }

        #region INTERFACE

        public easyStatics.easyRelativeDimensions TextFontRelativeDimension
        {
            get
            {
                return _textFontRelativeDimension;
            }
            set
            {
                _textFontRelativeDimension = value;
                setTextFont();
            }
        }

        #endregion

        #region PRIVATE SUB

        private void setTextFont()
        {
            try
            {
                if (_textFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined)
                {
                    this.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(_textFontRelativeDimension);
                }
                else
                    this.Appearance.FontData.SizeInPoints = 0;
            }
            catch (Exception)
            {
            }
        }


        #endregion

        #region OVERRIDE EVENTS

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            setTextFont();
        }

        #endregion

    }

    public class ucEasyListBox : Infragistics.Win.UltraWinListView.UltraListView, Interfacce.IEasyResizableText
    {

        #region Declare

        private easyStatics.easyRelativeDimensions _textFontRelativeDimension = easyStatics.easyRelativeDimensions._undefined;

        #endregion

        public ucEasyListBox()
        {
            this.UseOsThemes = DefaultableBoolean.False;
            this.AutoSize = false;
        }

        #region INTERFACE

        public easyStatics.easyRelativeDimensions TextFontRelativeDimension
        {
            get
            {
                return _textFontRelativeDimension;
            }
            set
            {
                _textFontRelativeDimension = value;
                setTextFont();
            }
        }

        #endregion

        #region PRIVATE SUB

        private void setTextFont()
        {
            try
            {
                if (_textFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined)
                {
                    this.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(_textFontRelativeDimension);
                }
                else
                    this.Appearance.FontData.SizeInPoints = 0;
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region OVERRIDE EVENTS

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            setTextFont();
        }

        #endregion
    }

    public class ucEasyLabel : Infragistics.Win.Misc.UltraLabel, Interfacce.IEasyShortcut, Interfacce.IEasyResizableText
    {

        #region Declare

        private easyStatics.easyRelativeDimensions _shortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
        private easyStatics.easyShortcutPosition _shortcutPosition = easyStatics.easyShortcutPosition.top_right;
        private Keys _shortcutKey = Keys.None;
        private Color _shortcutColor = Color.Black;

        private easyStatics.easyRelativeDimensions _textFontRelativeDimension = easyStatics.easyRelativeDimensions._undefined;

        #endregion

        public ucEasyLabel()
        {

            this.Appearance.TextVAlign = VAlign.Middle;


        }

        #region INTERFACE

        public easyStatics.easyRelativeDimensions ShortcutFontRelativeDimension
        {
            get
            {
                return _shortcutFontRelativeDimension;
            }
            set
            {
                _shortcutFontRelativeDimension = value;
                drawShortcut();
            }
        }

        public easyStatics.easyShortcutPosition ShortcutPosition
        {
            get
            {
                return _shortcutPosition;
            }
            set
            {
                _shortcutPosition = value;
                drawShortcut();
            }
        }

        public Keys ShortcutKey
        {
            get
            {
                return _shortcutKey;
            }
            set
            {
                _shortcutKey = value;
                drawShortcut();
            }
        }

        public easyStatics.easyRelativeDimensions TextFontRelativeDimension
        {
            get
            {
                return _textFontRelativeDimension;
            }
            set
            {
                _textFontRelativeDimension = value;
                setTextFont();
            }
        }

        public Color ShortcutColor
        {
            get
            {
                return _shortcutColor;
            }
            set
            {
                _shortcutColor = value;
            }
        }

        public void PerformActionShortcut()
        {
            this.OnClick(EventArgs.Empty);
        }

        #endregion

        #region PRIVATE SUB

        private void drawShortcut()
        {
            try
            {
                if (_shortcutKey == Keys.None)
                {
                    this.Appearance.ImageBackground = null;
                }
                else
                {
                    float fFontSize = this.Appearance.FontData.SizeInPoints;
                    if (_shortcutFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined) fFontSize = easyStatics.getFontSizeInPointsFromRelativeDimension(_shortcutFontRelativeDimension);
                    if (_shortcutColor == null) _shortcutColor = Color.Black;

                    string fontFamily = this.Appearance.FontData.Name;
                    if (fontFamily == null || fontFamily == "") fontFamily = "Calibri";
                    Font f = new Font(fontFamily, fFontSize, FontStyle.Regular, GraphicsUnit.Pixel);

                    this.Appearance.ImageBackground = easyStatics.drawBmpText(this.Size, easyStatics.getTextFromKey(_shortcutKey), _shortcutColor, f, _shortcutPosition);
                }

            }
            catch (Exception)
            {
            }
        }

        private void setTextFont()
        {
            try
            {
                if (_textFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined)
                {
                    this.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(_textFontRelativeDimension);
                }
                else
                    this.Appearance.FontData.SizeInPoints = 0;
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region OVERRIDE EVENTS

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);


            drawShortcut();
            setTextFont();
        }

        #endregion

    }

    public class ucEasyLabelHeader : Infragistics.Win.Misc.UltraLabel, Interfacce.IEasyShortcut, Interfacce.IEasyResizableText
    {

        #region Declare

        private easyStatics.easyRelativeDimensions _shortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
        private easyStatics.easyShortcutPosition _shortcutPosition = easyStatics.easyShortcutPosition.top_right;
        private Keys _shortcutKey = Keys.None;
        private Color _shortcutColor = Color.Black;

        private easyStatics.easyRelativeDimensions _textFontRelativeDimension = easyStatics.easyRelativeDimensions._undefined;

        #endregion

        public ucEasyLabelHeader()
        {

            this.Appearance.TextVAlign = VAlign.Middle;
            this.Appearance.BackColor = Color.FromArgb(193, 217, 240);
            this.Appearance.ForeColor = Color.FromArgb(22, 65, 158);

        }

        #region INTERFACE

        public easyStatics.easyRelativeDimensions ShortcutFontRelativeDimension
        {
            get
            {
                return _shortcutFontRelativeDimension;
            }
            set
            {
                _shortcutFontRelativeDimension = value;
                drawShortcut();
            }
        }

        public easyStatics.easyShortcutPosition ShortcutPosition
        {
            get
            {
                return _shortcutPosition;
            }
            set
            {
                _shortcutPosition = value;
                drawShortcut();
            }
        }

        public Keys ShortcutKey
        {
            get
            {
                return _shortcutKey;
            }
            set
            {
                _shortcutKey = value;
                drawShortcut();
            }
        }

        public easyStatics.easyRelativeDimensions TextFontRelativeDimension
        {
            get
            {
                return _textFontRelativeDimension;
            }
            set
            {
                _textFontRelativeDimension = value;
                setTextFont();
            }
        }

        public Color ShortcutColor
        {
            get
            {
                return _shortcutColor;
            }
            set
            {
                _shortcutColor = value;
            }
        }

        public void PerformActionShortcut()
        {
            this.OnClick(EventArgs.Empty);
        }

        #endregion

        #region PRIVATE SUB

        private void drawShortcut()
        {
            try
            {
                if (_shortcutKey == Keys.None)
                {
                    this.Appearance.ImageBackground = null;
                }
                else
                {
                    float fFontSize = this.Appearance.FontData.SizeInPoints;
                    if (_shortcutFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined) fFontSize = easyStatics.getFontSizeInPointsFromRelativeDimension(_shortcutFontRelativeDimension);
                    if (_shortcutColor == null) _shortcutColor = Color.Black;

                    string fontFamily = this.Appearance.FontData.Name;
                    if (fontFamily == null || fontFamily == "") fontFamily = "Calibri";
                    Font f = new Font(fontFamily, fFontSize, FontStyle.Regular, GraphicsUnit.Pixel);

                    this.Appearance.ImageBackground = easyStatics.drawBmpText(this.Size, easyStatics.getTextFromKey(_shortcutKey), _shortcutColor, f, _shortcutPosition);
                }

            }
            catch (Exception)
            {
            }
        }

        private void setTextFont()
        {
            try
            {
                if (_textFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined)
                {
                    this.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(_textFontRelativeDimension);
                }
                else
                    this.Appearance.FontData.SizeInPoints = 0;
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region OVERRIDE EVENTS

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);


            drawShortcut();
            setTextFont();
        }

        #endregion

    }

    public class ucEasyNumericEditor : Infragistics.Win.UltraWinEditors.UltraNumericEditor, Interfacce.IEasyResizableText
    {
        #region Declare

        private easyStatics.easyRelativeDimensions _textFontRelativeDimension = easyStatics.easyRelativeDimensions._undefined;

        #endregion

        public ucEasyNumericEditor()
        {
            this.UseOsThemes = DefaultableBoolean.False;
            this.DisplayStyle = EmbeddableElementDisplayStyle.Office2007;
            this.AutoSize = false;
        }

        #region INTERFACE

        public easyStatics.easyRelativeDimensions TextFontRelativeDimension
        {
            get
            {
                return _textFontRelativeDimension;
            }
            set
            {
                _textFontRelativeDimension = value;
                setTextFont();
            }
        }

        #endregion

        #region PRIVATE SUB

        private void setTextFont()
        {
            try
            {
                if (_textFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined)
                {
                    this.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(_textFontRelativeDimension);
                }
                else
                    this.Appearance.FontData.SizeInPoints = 0;
            }
            catch (Exception)
            {
            }
        }


        #endregion

        #region OVERRIDE EVENTS

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            setTextFont();
        }

        #endregion

    }

    public class ucEasyMaskedEdit : Infragistics.Win.UltraWinMaskedEdit.UltraMaskedEdit, Interfacce.IEasyResizableText
    {
        #region Declare

        private easyStatics.easyRelativeDimensions _textFontRelativeDimension = easyStatics.easyRelativeDimensions._undefined;

        private int _giorni = 0;
        private int _ore = 0;
        private int _minuti = 0;

        private const string c_separatore = @":";

        #endregion

        public ucEasyMaskedEdit()
        {
            this.UseOsThemes = DefaultableBoolean.False;
            this.DisplayStyle = EmbeddableElementDisplayStyle.Office2007;
            this.AutoSize = false;
            this.InputMask = "nnn" + c_separatore + "hh" + c_separatore + "mm";
            this.SpinButtonDisplayStyle = Infragistics.Win.SpinButtonDisplayStyle.OnRight;
            this.setValue();
        }

        #region INTERFACE

        public easyStatics.easyRelativeDimensions TextFontRelativeDimension
        {
            get
            {
                return _textFontRelativeDimension;
            }
            set
            {
                _textFontRelativeDimension = value;
                setTextFont();
            }
        }

        #endregion

        #region PROPERTIES

        public int Giorni
        {
            get
            {
                this.getValue();
                return _giorni;
            }
            set
            {
                _giorni = value;
                this.setValue();
            }
        }

        public int Ore
        {
            get
            {
                this.getValue();
                return _ore;
            }
            set
            {
                _ore = value;
                this.setValue();
            }
        }

        public int Minuti
        {
            get
            {
                this.getValue();
                return _minuti;
            }
            set
            {
                _minuti = value;
                this.setValue();
            }
        }

        #endregion

        #region PRIVATE SUB

        private void setTextFont()
        {
            try
            {
                if (_textFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined)
                {
                    this.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(_textFontRelativeDimension);
                }
                else
                    this.Appearance.FontData.SizeInPoints = 0;
            }
            catch (Exception)
            {
            }
        }

        private void setValue()
        {
            this.Value = string.Format("{0:000}{3}{1:00}{4}{2:00}", _giorni, _ore, _minuti, c_separatore, c_separatore);
        }

        private void getValue()
        {

            try
            {

                string[] valori = this.Value.ToString().Split(c_separatore.ToCharArray());
                if (valori.Length == 3)
                {
                    _giorni = int.Parse(valori[0]);
                    _ore = int.Parse(valori[1]);
                    _minuti = int.Parse(valori[2]);
                }

            }
            catch (Exception)
            {

            }

        }

        #endregion

        #region PUBLIC SUB

        public void Reset()
        {
            _giorni = 0;
            _ore = 0;
            _minuti = 0;
            this.setValue();
        }

        #endregion

        #region OVERRIDE EVENTS

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            setTextFont();
        }

        #endregion

    }

    public class ucEasyCurrencyEditor : Infragistics.Win.UltraWinEditors.UltraCurrencyEditor, Interfacce.IEasyResizableText
    {
        #region Declare

        private easyStatics.easyRelativeDimensions _textFontRelativeDimension = easyStatics.easyRelativeDimensions._undefined;

        #endregion

        public ucEasyCurrencyEditor()
        {
            this.UseOsThemes = DefaultableBoolean.False;
            this.DisplayStyle = EmbeddableElementDisplayStyle.Office2007;
            this.AutoSize = false;
        }

        #region INTERFACE

        public easyStatics.easyRelativeDimensions TextFontRelativeDimension
        {
            get
            {
                return _textFontRelativeDimension;
            }
            set
            {
                _textFontRelativeDimension = value;
                setTextFont();
            }
        }

        #endregion

        #region PRIVATE SUB

        private void setTextFont()
        {
            try
            {
                if (_textFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined)
                {
                    this.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(_textFontRelativeDimension);
                }
                else
                    this.Appearance.FontData.SizeInPoints = 0;
            }
            catch (Exception)
            {
            }
        }


        #endregion

        #region OVERRIDE EVENTS

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            setTextFont();
        }

        #endregion

    }

    public class ucEasyGrid : Infragistics.Win.UltraWinGrid.UltraGrid
    {
        #region Declare

        private easyStatics.easyRelativeDimensions _dataRowFontRelativeDimension = easyStatics.easyRelativeDimensions._undefined;
        private easyStatics.easyRelativeDimensions _gridCaptionFontRelativeDimension = easyStatics.easyRelativeDimensions._undefined;
        private easyStatics.easyRelativeDimensions _headerFontRelativeDimension = easyStatics.easyRelativeDimensions._undefined;
        private easyStatics.easyRelativeDimensions _filterRowFontRelativeDimension = easyStatics.easyRelativeDimensions._undefined;

        private string _colonnaRTFResize = "";
        private int _fattoreRidimensionamentoRTF = 21;
        private bool _colonnaRTFControlloContenuto = false;
        private RichTextBox _rtf = null;
        private Infragistics.Win.UltraWinGrid.AutoFitStyle _defaultAutoFitStyle = AutoFitStyle.ExtendLastColumn;

        #endregion

        public ucEasyGrid()
        {


            initGrid();

        }

        #region PROPERTIES

        public easyStatics.easyRelativeDimensions DataRowFontRelativeDimension
        {
            get
            {
                return _dataRowFontRelativeDimension;
            }
            set
            {
                _dataRowFontRelativeDimension = value;
                setTextFont();
            }
        }

        public easyStatics.easyRelativeDimensions GridCaptionFontRelativeDimension
        {
            get
            {
                if (_gridCaptionFontRelativeDimension == easyStatics.easyRelativeDimensions._undefined && _dataRowFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined)
                    return _dataRowFontRelativeDimension;
                else
                    return _gridCaptionFontRelativeDimension;
            }
            set
            {
                _gridCaptionFontRelativeDimension = value;
                setTextFont();
            }
        }

        public easyStatics.easyRelativeDimensions HeaderFontRelativeDimension
        {
            get
            {
                if (_headerFontRelativeDimension == easyStatics.easyRelativeDimensions._undefined && _dataRowFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined)
                    return _dataRowFontRelativeDimension;
                else
                    return _headerFontRelativeDimension;
            }
            set
            {
                _headerFontRelativeDimension = value;
                setTextFont();
            }
        }

        public easyStatics.easyRelativeDimensions FilterRowFontRelativeDimension
        {
            get
            {
                if (_filterRowFontRelativeDimension == easyStatics.easyRelativeDimensions._undefined && _dataRowFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined)
                    return _dataRowFontRelativeDimension;
                else
                    return _filterRowFontRelativeDimension;
            }
            set
            {
                _filterRowFontRelativeDimension = value;
                setTextFont();
            }
        }

        public bool ShowFilterRow
        {
            get
            {
                return (this.DisplayLayout.Override.AllowRowFiltering == Infragistics.Win.DefaultableBoolean.True && this.DisplayLayout.Override.FilterUIType == FilterUIType.FilterRow);
            }
            set
            {
                setFilterRow(value);
            }
        }

        public bool ShowGroupByBox
        {
            get
            {
                return (!this.DisplayLayout.GroupByBox.Hidden);
            }
            set
            {
                this.DisplayLayout.GroupByBox.Hidden = !value;
            }
        }

        [Obsolete("Ignorato dalla procedura di resize", false)]
        public int FattoreRidimensionamentoRTF
        {
            get
            {
                return _fattoreRidimensionamentoRTF;
            }
            set
            {
                _fattoreRidimensionamentoRTF = value;
            }
        }

        public string ColonnaRTFResize
        {
            get
            {
                return _colonnaRTFResize;
            }
            set
            {
                _colonnaRTFResize = value;
                if (_colonnaRTFResize != "")
                {
                    this.DisplayLayout.Override.RowSizing = RowSizing.Fixed;
                    this.DisplayLayout.Override.RowSizingArea = RowSizingArea.EntireRow;
                }
            }
        }

        public bool ColonnaRTFControlloContenuto
        {
            get
            {
                return _colonnaRTFControlloContenuto;
            }
            set
            {
                _colonnaRTFControlloContenuto = value;
            }
        }



        public Infragistics.Win.UltraWinGrid.AutoFitStyle DefaultAutoFitStyle
        {
            get { return _defaultAutoFitStyle; }
            set { _defaultAutoFitStyle = value; }
        }

        #endregion

        #region PRIVATE SUB

        private void setTextFont()
        {
            try
            {
                if (this.HeaderFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined)
                    this.DisplayLayout.Override.HeaderAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(this.HeaderFontRelativeDimension);
                else
                    this.DisplayLayout.Override.HeaderAppearance.FontData.SizeInPoints = 0;

                if (this.GridCaptionFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined)
                    this.DisplayLayout.CaptionAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(this.GridCaptionFontRelativeDimension);
                else
                    this.DisplayLayout.CaptionAppearance.FontData.SizeInPoints = 0;

                if (this.DataRowFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined)
                    this.DisplayLayout.Override.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(this.DataRowFontRelativeDimension);
                else
                    this.DisplayLayout.Override.CellAppearance.FontData.SizeInPoints = 0;

                if (this.FilterRowFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined)
                    this.DisplayLayout.Override.FilterRowAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(this.FilterRowFontRelativeDimension);
                else
                    this.DisplayLayout.Override.FilterRowAppearance.FontData.SizeInPoints = 0;

            }
            catch (Exception)
            {
            }
        }

        private void initGrid()
        {
            try
            {

                this.DisplayLayout.AutoFitStyle = this.DefaultAutoFitStyle; this.DisplayLayout.Appearance.BackColor = Color.FromKnownColor(KnownColor.Window);
                this.DisplayLayout.Appearance.BorderColor = Color.FromKnownColor(KnownColor.InactiveCaption);


                this.DisplayLayout.GroupByBox.Hidden = true;
                this.DisplayLayout.GroupByBox.Prompt = @"Trascina un'intestazione della colonna qui per raggrupparla.";
                this.DisplayLayout.GroupByBox.Appearance.BackColor = Color.LightSteelBlue;
                this.DisplayLayout.GroupByBox.Appearance.BackColor2 = Color.Lavender;
                this.DisplayLayout.GroupByBox.Appearance.BackGradientStyle = Infragistics.Win.GradientStyle.ForwardDiagonal;
                this.DisplayLayout.GroupByBox.PromptAppearance.BackColor = Color.Lavender;
                this.DisplayLayout.GroupByBox.PromptAppearance.BackColor2 = Color.Lavender;
                this.DisplayLayout.GroupByBox.PromptAppearance.BackGradientStyle = Infragistics.Win.GradientStyle.None;
                this.DisplayLayout.GroupByBox.PromptAppearance.ForeColor = Color.Black;

                this.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
                this.DisplayLayout.GroupByBox.Hidden = true;

                this.DisplayLayout.Override.ActiveCellAppearance.BackColor = Color.FromKnownColor(KnownColor.Window);
                this.DisplayLayout.Override.ActiveCellAppearance.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
                this.DisplayLayout.Override.ActiveRowAppearance.BackColor = Color.FromKnownColor(KnownColor.Highlight);
                this.DisplayLayout.Override.ActiveRowAppearance.ForeColor = Color.FromKnownColor(KnownColor.HighlightText);

                this.DisplayLayout.Override.AllowAddNew = AllowAddNew.No;
                this.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
                this.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
                this.DisplayLayout.Override.AllowColMoving = AllowColMoving.NotAllowed;
                this.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;

                this.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
                this.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;

                this.DisplayLayout.Override.CellAppearance.BorderColor = Color.Silver;
                this.DisplayLayout.Override.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
                this.DisplayLayout.Override.ColumnAutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.SiblingRowsOnly;

                this.DisplayLayout.Override.HeaderAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                this.DisplayLayout.Override.HeaderAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                this.DisplayLayout.Override.HeaderClickAction = HeaderClickAction.SortMulti;
                this.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsVista;

                this.DisplayLayout.Override.RowAlternateAppearance.BackColor = Color.WhiteSmoke;
                this.DisplayLayout.Override.RowAlternateAppearance.ForeColor = Color.DarkBlue;
                this.DisplayLayout.Override.RowAppearance.BackColor = Color.FromKnownColor(KnownColor.Window);
                this.DisplayLayout.Override.RowAppearance.BorderColor = Color.Silver;
                this.DisplayLayout.Override.RowAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;

                this.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
                this.DisplayLayout.Override.RowSizing = Infragistics.Win.UltraWinGrid.RowSizing.AutoFree;

                this.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
                this.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;

                this.DisplayLayout.ScrollBounds = ScrollBounds.ScrollToFill;
                this.DisplayLayout.ScrollStyle = ScrollStyle.Immediate;
                this.DisplayLayout.TabNavigation = Infragistics.Win.UltraWinGrid.TabNavigation.NextControl;

                this.DisplayLayout.ScrollBarLook.ViewStyle = Infragistics.Win.UltraWinScrollBar.ScrollBarViewStyle.Office2010;
                this.DisplayLayout.ViewStyleBand = ViewStyleBand.OutlookGroupBy;

                this.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
                this.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;

                this.DisplayLayout.AutoFitStyle = this.DefaultAutoFitStyle; this.DisplayLayout.Override.CellClickAction = CellClickAction.RowSelect;

                setFilterRow(false);

                setTextFont();

            }
            catch (Exception)
            {
            }
        }

        private void setFilterRow(bool showFilterRow)
        {
            if (showFilterRow)
            {
                this.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
                this.DisplayLayout.Override.RowFilterMode = RowFilterMode.AllRowsInBand;
                this.DisplayLayout.Override.RowFilterMode = RowFilterMode.Default;
                this.DisplayLayout.Override.FilterUIType = FilterUIType.FilterRow;

                this.DisplayLayout.Override.FilterClearButtonLocation = FilterClearButtonLocation.Row;

                this.DisplayLayout.Override.FilterRowAppearance.BackColor = Color.LightYellow;
                this.DisplayLayout.Override.FilterComparisonType = FilterComparisonType.CaseInsensitive;

                this.DisplayLayout.Override.FilterOperatorDefaultValue = FilterOperatorDefaultValue.Contains;
                this.DisplayLayout.Override.FilterOperatorLocation = FilterOperatorLocation.Hidden;
                this.DisplayLayout.Override.FilterOperandStyle = FilterOperandStyle.Edit;

                this.DisplayLayout.Override.FilterRowPrompt = "";
                this.DisplayLayout.Override.FilterRowPromptAppearance.BackColorAlpha = Infragistics.Win.Alpha.Opaque;
                this.DisplayLayout.Override.SpecialRowSeparator = SpecialRowSeparator.FilterRow;
            }
            else
            {
                this.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            }
        }

        #endregion

        #region OVERRIDE EVENTS

        protected override void OnLayout(LayoutEventArgs e)
        {

            this.DisplayLayout.AutoFitStyle = this.DefaultAutoFitStyle; this.DisplayLayout.Override.CellClickAction = CellClickAction.RowSelect;
            this.DisplayLayout.Override.HeaderAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
            this.DisplayLayout.Override.HeaderAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
            this.DisplayLayout.Override.HeaderClickAction = HeaderClickAction.SortSingle;
            this.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsVista;

            setTextFont();

            base.OnLayout(e);
        }

        protected override void OnInitializeRow(InitializeRowEventArgs e)
        {
            try
            {
                if ((_colonnaRTFResize.Trim() != "") &&
                    (e.Row.Cells.Exists(_colonnaRTFResize)) &&
                    (e.Row.Cells[_colonnaRTFResize].Hidden == false) &&
                    (e.Row.Cells[_colonnaRTFResize].Text != "")
                   )
                {
                    int h = 0;
                    int thickness = 0;

                    UltraGridColumn col = this.DisplayLayout.Bands[0].Columns[_colonnaRTFResize];

                    using (RichTextBox rtb = new RichTextBox())
                    {
                        string text = e.Row.Cells[_colonnaRTFResize].Text;

                        if (this.isValidRtf(text))
                            rtb.Rtf = text;
                        else
                            rtb.Text = text;

                        rtb.Width = col.CellSizeResolved.Width;

                        if (rtb.Width == 0) rtb.Width = this.Width;

                        thickness = rtb.Margin.Vertical + SystemInformation.VerticalResizeBorderThickness;

                        h = rtb.CalculateRichTextHeight();

                        rtb.Clear();
                    }

                    int hSpan = 0;
                    if (e.Row.Band.Groups.Count > 0)
                    {
                        UltraGridGroup g = e.Row.Band.Groups[0];
                        int rtfY = e.Row.Cells[_colonnaRTFResize].Column.RowLayoutColumnInfo.OriginY;

                        for (int i = 0; i < g.RowLayoutGroupInfo.SpanY; i++)
                        {
                            if (i != rtfY)
                                hSpan = hSpan + rowRealHeight(i, e.Row);
                        }
                    }

                    e.Row.Height = (h + thickness + hSpan);
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            base.OnInitializeRow(e);
        }

        private bool isValidRtf(string text)
        {
            try
            {
                using (RichTextBox rtb = new RichTextBox())
                {
                    rtb.Rtf = text;
                }
            }
            catch (ArgumentException)
            {
                return false;

            }

            return true;
        }

        private int rowRealHeight(int rowOriginX, UltraGridRow row)
        {
            if (row.Band.Groups.Count == 0) return row.Height;

            int hMax = 0;

            foreach (UltraGridColumn col in row.Band.Columns)
            {
                if ((col.Hidden == false) && (col.RowLayoutColumnInfo.OriginY == rowOriginX))
                {
                    int ch = col.CellSizeResolved.Height;

                    if (ch == 0)
                        ch = row.Height / row.Band.Groups[0].RowLayoutGroupInfo.SpanY;

                    if (ch > hMax)
                        hMax = ch;
                }
            }

            return hMax;
        }




        #endregion

    }

    public class ucEasyPictureBox : System.Windows.Forms.PictureBox, Interfacce.IEasyShortcut
    {
        #region DECLARE

        private easyStatics.easyRelativeDimensions _shortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
        private easyStatics.easyShortcutPosition _shortcutPosition = easyStatics.easyShortcutPosition.top_right;
        private Keys _shortcutKey = Keys.None;
        private Color _shortcutColor = Color.Black;

        #endregion

        public ucEasyPictureBox()
        {
            this.SizeMode = PictureBoxSizeMode.Zoom;
        }

        #region INTERFACE

        public easyStatics.easyRelativeDimensions ShortcutFontRelativeDimension
        {
            get
            {
                return _shortcutFontRelativeDimension;
            }
            set
            {
                _shortcutFontRelativeDimension = value;
                drawShortcut();
            }
        }

        public easyStatics.easyShortcutPosition ShortcutPosition
        {
            get
            {
                return _shortcutPosition;
            }
            set
            {
                _shortcutPosition = value;
                drawShortcut();
            }
        }

        public Keys ShortcutKey
        {
            get
            {
                return _shortcutKey;
            }
            set
            {
                _shortcutKey = value;
                drawShortcut();
            }
        }

        public Color ShortcutColor
        {
            get
            {
                return _shortcutColor;
            }
            set
            {
                _shortcutColor = value;
            }
        }

        public void PerformActionShortcut()
        {
            this.OnClick(EventArgs.Empty);
        }

        #endregion

        #region PRIVATE SUB

        private void drawShortcut()
        {
            try
            {
                if (_shortcutKey == Keys.None)
                {
                    this.BackgroundImage = null;
                }
                else
                {

                    float fFontSize = 8.25F;
                    if (_shortcutFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined) fFontSize = easyStatics.getFontSizeInPointsFromRelativeDimension(_shortcutFontRelativeDimension);
                    if (_shortcutColor == null) _shortcutColor = Color.Black;

                    string fontFamily = null;
                    if (fontFamily == null || fontFamily == "") fontFamily = "Calibri";
                    Font f = new Font(fontFamily, fFontSize, FontStyle.Regular, GraphicsUnit.Pixel);

                    this.BackgroundImage = easyStatics.drawBmpText(this.Size, easyStatics.getTextFromKey(_shortcutKey), _shortcutColor, f, _shortcutPosition);
                }

            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region OVERRIDE EVENTS

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);


            drawShortcut();
        }

        #endregion

    }

    public class ucEasyCalendarCombo : Infragistics.Win.UltraWinSchedule.UltraCalendarCombo, Interfacce.IEasyResizableText
    {
        #region Declare

        private easyStatics.easyRelativeDimensions _textFontRelativeDimension = easyStatics.easyRelativeDimensions._undefined;

        #endregion

        public ucEasyCalendarCombo()
        {
            this.UseOsThemes = DefaultableBoolean.False;
            this.DayDisplayStyle = Infragistics.Win.UltraWinSchedule.DayDisplayStyle.DayNumber;
            this.AutoSize = false;

            this.ButtonStyle = UIElementButtonStyle.Office2003ToolbarButton;

            this.AllowNull = false;

            this.DateButtons.Clear();

            Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton button = new Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton();
            button.Visible = true;
            button.Caption = "Oggi";
            button.Type = Infragistics.Win.UltraWinSchedule.DateButtonType.Today;
            button.Action = Infragistics.Win.UltraWinSchedule.DateButtonAction.SelectDay;
            button.Date = DateTime.Now.Date;

            this.DateButtons.Add(button);

        }

        #region INTERFACE

        public easyStatics.easyRelativeDimensions TextFontRelativeDimension
        {
            get
            {
                return _textFontRelativeDimension;
            }
            set
            {
                _textFontRelativeDimension = value;
                setTextFont();
            }
        }

        #endregion

        #region PRIVATE SUB

        private void setTextFont()
        {
            try
            {
                if (_textFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined)
                {
                    this.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(_textFontRelativeDimension);
                    this.DropDownAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(_textFontRelativeDimension);
                }
                else
                {
                    this.Appearance.FontData.SizeInPoints = 0;
                    this.DropDownAppearance.FontData.SizeInPoints = 0;
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region OVERRIDE EVENTS

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            setTextFont();
        }

        #endregion
    }

    public class ucEasyDateTimeEditor : Infragistics.Win.UltraWinEditors.UltraDateTimeEditor, Interfacce.IEasyResizableText
    {
        #region Declare

        private easyStatics.easyRelativeDimensions _textFontRelativeDimension = easyStatics.easyRelativeDimensions._undefined;

        #endregion

        public ucEasyDateTimeEditor()
        {
            this.UseOsThemes = DefaultableBoolean.False;
            this.DisplayStyle = EmbeddableElementDisplayStyle.Office2007;
            this.AutoSize = false;
        }

        #region INTERFACE

        public easyStatics.easyRelativeDimensions TextFontRelativeDimension
        {
            get
            {
                return _textFontRelativeDimension;
            }
            set
            {
                _textFontRelativeDimension = value;
                setTextFont();
            }
        }

        #endregion

        #region PRIVATE SUB

        private void setTextFont()
        {
            try
            {
                if (_textFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined)
                {
                    this.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(_textFontRelativeDimension);
                    this.DropDownAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(_textFontRelativeDimension);
                }
                else
                {
                    this.Appearance.FontData.SizeInPoints = 0;
                    this.DropDownAppearance.FontData.SizeInPoints = 0;
                }
            }
            catch (Exception)
            {
            }
        }


        #endregion

        #region OVERRIDE EVENTS

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            setTextFont();
        }

        #endregion

    }

    public class ucEasyStateButton : Infragistics.Win.Misc.UltraButton, Interfacce.IEasyShortcut, Interfacce.IEasyResizableText
    {

        #region Declare

        private easyStatics.easyRelativeDimensions _shortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
        private easyStatics.easyShortcutPosition _shortcutPosition = easyStatics.easyShortcutPosition.top_right;
        private Keys _shortcutKey = Keys.None;
        private Color _shortcutColor = Color.Black;

        private easyStatics.easyRelativeDimensions _textFontRelativeDimension = easyStatics.easyRelativeDimensions._undefined;

        private float _percImageFill = 0.75F;

        private bool _checked = false;

        private object _checkedImage = null;
        private object _UNcheckedImage = null;

        #endregion

        public ucEasyStateButton()
        {

            _checked = false;

            this.UseOsThemes = DefaultableBoolean.False;
            this.ButtonStyle = UIElementButtonStyle.Office2003ToolbarButton;

            this.Appearance.TextHAlign = HAlign.Center;
            this.Appearance.TextVAlign = VAlign.Bottom;
            this.Appearance.ImageHAlign = HAlign.Center;
            this.Appearance.ImageVAlign = VAlign.Top;


        }

        #region INTERFACE

        public easyStatics.easyRelativeDimensions ShortcutFontRelativeDimension
        {
            get
            {
                return _shortcutFontRelativeDimension;
            }
            set
            {
                _shortcutFontRelativeDimension = value;
                drawShortcut();
            }
        }

        public easyStatics.easyShortcutPosition ShortcutPosition
        {
            get
            {
                return _shortcutPosition;
            }
            set
            {
                _shortcutPosition = value;
                drawShortcut();
            }
        }

        public Keys ShortcutKey
        {
            get
            {
                return _shortcutKey;
            }
            set
            {
                _shortcutKey = value;
                drawShortcut();
            }
        }

        public easyStatics.easyRelativeDimensions TextFontRelativeDimension
        {
            get
            {
                return _textFontRelativeDimension;
            }
            set
            {
                _textFontRelativeDimension = value;
                setTextFont();
            }
        }

        public Color ShortcutColor
        {
            get
            {
                return _shortcutColor;
            }
            set
            {
                _shortcutColor = value;
            }
        }

        public void PerformActionShortcut()
        {
            this.OnClick(EventArgs.Empty);
        }

        #endregion

        #region PROPERTIES

        public float PercImageFill
        {
            get
            {
                return _percImageFill;
            }
            set
            {
                _percImageFill = value;
                resizeImage();
            }
        }

        public bool Checked
        {
            get
            {
                return _checked;
            }
            set
            {
                _checked = value;
                setCheckedAppearance();
            }
        }

        public object CheckedImage
        {
            get
            {
                if (_checkedImage != null)
                    return _checkedImage;
                else
                    return this.Appearance.Image;
            }
            set
            {
                _checkedImage = value;
                setCheckedAppearance();
            }
        }

        public object UNCheckedImage
        {
            get
            {
                if (_UNcheckedImage != null)
                    return _UNcheckedImage;
                else
                    return this.Appearance.Image;
            }
            set
            {
                _UNcheckedImage = value;
                if (this.Appearance.Image == null) this.Appearance.Image = value;
                setCheckedAppearance();
            }
        }

        #endregion

        #region PRIVATE SUB

        private void drawShortcut()
        {
            try
            {
                if (_shortcutKey == Keys.None)
                {
                    this.Appearance.ImageBackground = null;
                }
                else
                {
                    float fFontSize = this.Appearance.FontData.SizeInPoints;
                    if (_shortcutFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined) fFontSize = easyStatics.getFontSizeInPointsFromRelativeDimension(_shortcutFontRelativeDimension);
                    if (_shortcutColor == null) _shortcutColor = Color.Black;

                    string fontFamily = this.Appearance.FontData.Name;
                    if (fontFamily == null || fontFamily == "") fontFamily = "Calibri";
                    Font f = new Font(fontFamily, fFontSize, FontStyle.Regular, GraphicsUnit.Pixel);

                    this.Appearance.ImageBackground = easyStatics.drawBmpText(this.Size, easyStatics.getTextFromKey(_shortcutKey), _shortcutColor, f, _shortcutPosition);
                }

            }
            catch (Exception)
            {
            }
        }

        private void setTextFont()
        {
            try
            {
                if (_textFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined)
                {
                    this.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(_textFontRelativeDimension);
                }
                else
                    this.Appearance.FontData.SizeInPoints = 0;
            }
            catch (Exception)
            {
            }
        }

        private void resizeImage()
        {
            try
            {
                if ((this.Appearance.Image != null || _checkedImage != null) && _percImageFill > 0)
                {
                    int iCtrlSize = this.Size.Height;
                    if (iCtrlSize > this.Size.Width) iCtrlSize = this.Size.Width;
                    this.ImageSize = new Size((int)(iCtrlSize * _percImageFill), (int)(iCtrlSize * _percImageFill));
                }
            }
            catch (Exception)
            {
            }
        }

        private void setCheckedAppearance()
        {
            try
            {
                if (_checked)
                {
                    if (_checkedImage != null)
                        this.Appearance.Image = this.CheckedImage;

                    this.Appearance.BackGradientStyle = GradientStyle.Vertical;
                    this.Appearance.BackColor = Color.FromArgb(255, 212, 138);
                    this.Appearance.BackColor2 = Color.FromArgb(255, 174, 86);

                    this.Appearance.BorderColor = Color.Black;
                    this.Appearance.BorderAlpha = Alpha.Opaque;

                    this.Appearance.FontData.Bold = DefaultableBoolean.True;
                }
                else
                {
                    if (_UNcheckedImage != null)
                        this.Appearance.Image = _UNcheckedImage;

                    this.Appearance.BackGradientStyle = GradientStyle.Default;
                    this.Appearance.BackColor = Color.Empty;
                    this.Appearance.BackColor2 = Color.Empty;
                    this.Appearance.BorderColor = Color.Empty;
                    this.Appearance.BorderAlpha = Alpha.Default;
                    this.Appearance.FontData.Bold = DefaultableBoolean.Default;
                }
                resizeImage();
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region OVERRIDE EVENTS

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            drawShortcut();
            setTextFont();
        }

        protected override void OnClick(EventArgs e)
        {
            this.Checked = !this.Checked;
            base.OnClick(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            resizeImage();
        }

        #endregion
    }

    public class ucEasyComboEditor : Infragistics.Win.UltraWinEditors.UltraComboEditor, Interfacce.IEasyResizableText
    {
        private easyStatics.easyRelativeDimensions _textFontRelativeDimension = easyStatics.easyRelativeDimensions._undefined;

        public ucEasyComboEditor()
        {
            this.UseOsThemes = DefaultableBoolean.False;
            this.DisplayStyle = EmbeddableElementDisplayStyle.Office2007;
            this.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.AutoSize = false;
        }

        #region PROPERTIES

        public easyStatics.easyRelativeDimensions TextFontRelativeDimension
        {
            get
            {
                return _textFontRelativeDimension;
            }
            set
            {
                _textFontRelativeDimension = value;
                setTextFont();
            }
        }


        #endregion

        #region PRIVATE SUB

        private void setTextFont()
        {
            try
            {
                if (_textFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined)
                {
                    this.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(_textFontRelativeDimension);
                }
                else
                    this.Appearance.FontData.SizeInPoints = 0;
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region OVERRIDE EVENTS

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            setTextFont();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (this.DropDownStyle == Infragistics.Win.DropDownStyle.DropDownList
                && this.Enabled
                && !this.ReadOnly
                && (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back))
                this.Value = null;
        }

        #endregion

    }

    public class ucEasyColorPicker : Infragistics.Win.UltraWinEditors.UltraColorPicker, Interfacce.IEasyResizableText
    {
        private easyStatics.easyRelativeDimensions _textFontRelativeDimension = easyStatics.easyRelativeDimensions._undefined;

        public ucEasyColorPicker()
        {
            this.UseOsThemes = DefaultableBoolean.False;
            this.DisplayStyle = EmbeddableElementDisplayStyle.Office2007;
            this.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.AutoSize = false;
        }

        #region PROPERTIES

        public easyStatics.easyRelativeDimensions TextFontRelativeDimension
        {
            get
            {
                return _textFontRelativeDimension;
            }
            set
            {
                _textFontRelativeDimension = value;
                setTextFont();
            }
        }

        #endregion

        #region PRIVATE SUB

        private void setTextFont()
        {
            try
            {
                if (_textFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined)
                {
                    this.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(_textFontRelativeDimension);
                }
                else
                    this.Appearance.FontData.SizeInPoints = 0;
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region OVERRIDE EVENTS

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            setTextFont();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (this.DropDownStyle == Infragistics.Win.DropDownStyle.DropDownList
                && this.Enabled
                && !this.ReadOnly
                && (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back))
                this.Value = null;
        }

        #endregion

    }

    public class ucEasyDateRange : ucEasyComboEditor
    {

        #region DICHIARAZIONI

        public const string C_RNG_24H = "24H";
        public const string C_RNG_48H = "48H";
        public const string C_RNG_7G = "7G";
        public const string C_RNG_30G = "30G";
        public const string C_RNG_60G = "60G";
        public const string C_RNG_90G = "90G";
        public const string C_RNG_6M = "6M";

        public const string C_RNG_12M = "12M";
        public const string C_RNG_24M = "24M";
        public const string C_RNG_48M = "48M";

        public const string C_RNG_EPI = "EPI";

        public const string C_RNG_OGGI = "OGGI";
        public const string C_RNG_DOMANI = "DMN";
        public const string C_RNG_N24H = "N24H";
        public const string C_RNG_N48H = "N48H";
        public const string C_RNG_N7G = "N7G";
        public const string C_RNG_N30G = "N30G";
        public const string C_RNG_N60G = "N60G";

        public const string C_RNG_N1M = "N1M";
        public const string C_RNG_N6M = "N6M";
        public const string C_RNG_N1Y = "N1Y";
        public const string C_RNG_N5Y = "N5Y";

        private DateTime _dataEpisodio = DateTime.MinValue;

        private bool _futuro = false;
        private bool _domani = false;
        private bool _oggi = true;
        private bool _disponibilitaAgende = false;
        private bool _passatoremoto = false;

        #endregion

        public ucEasyDateRange()
        {
            _futuro = false;
            _disponibilitaAgende = false;
        }

        #region PROPERTIES

        public bool DisponibilitaAgende
        {
            get { return _disponibilitaAgende; }
            set
            {
                if (_disponibilitaAgende != value)
                {
                    _disponibilitaAgende = value;
                    loadItems();
                }
            }
        }

        public DateTime DataEpisodio
        {
            get { return _dataEpisodio; }
            set
            {
                if (_dataEpisodio != value)
                {
                    _dataEpisodio = value;
                    loadItems();
                }
            }
        }

        public bool DateFuture
        {
            get { return _futuro; }
            set
            {
                if (_futuro != value)
                {
                    _futuro = value;
                    loadItems();
                }
            }
        }

        public bool Domani
        {
            get { return _domani; }
            set
            {
                if (_domani != value)
                {
                    _domani = value;
                    loadItems();
                }
            }
        }

        public bool Oggi
        {
            get { return _oggi; }
            set
            {
                if (_oggi != value)
                {
                    _oggi = value;
                    loadItems();
                }
            }
        }

        public bool PassatoRemoto
        {
            get { return _passatoremoto; }
            set
            {
                if (_passatoremoto != value)
                {
                    _passatoremoto = value;
                    loadItems();
                }
            }
        }

        public object DataOraDa
        {
            get
            {
                if (this.Value == null)
                    return null;
                else
                {
                    switch (this.Value.ToString())
                    {
                        case C_RNG_24H:
                            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddDays(-1);
                        case C_RNG_48H:
                            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddDays(-2);
                        case C_RNG_7G:
                            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddDays(-7);
                        case C_RNG_30G:
                            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddDays(-30);
                        case C_RNG_60G:
                            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddDays(-60);
                        case C_RNG_90G:
                            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddDays(-90);
                        case C_RNG_6M:
                            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddMonths(-6);
                        case C_RNG_12M:
                            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddMonths(-12);
                        case C_RNG_24M:
                            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddMonths(-24);
                        case C_RNG_48M:
                            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddMonths(-48);
                        case C_RNG_EPI:
                            return new DateTime(_dataEpisodio.Year, _dataEpisodio.Month, _dataEpisodio.Day, 0, 0, 0);

                        case C_RNG_N24H:
                        case C_RNG_N48H:
                        case C_RNG_N7G:
                        case C_RNG_N30G:
                        case C_RNG_N60G:
                            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

                        case C_RNG_N1M:
                        case C_RNG_N6M:
                        case C_RNG_N1Y:
                        case C_RNG_N5Y:
                            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

                        case C_RNG_DOMANI:
                            DateTime dtdomanistart = DateTime.Now.Date.AddDays(1);
                            dtdomanistart = new DateTime(dtdomanistart.Year, dtdomanistart.Month, dtdomanistart.Day, 0, 0, 0);
                            return dtdomanistart;

                        case C_RNG_OGGI:
                            DateTime dtoggistat = DateTime.Now.Date;
                            dtoggistat = new DateTime(dtoggistat.Year, dtoggistat.Month, dtoggistat.Day, 0, 0, 0);
                            return dtoggistat;
                        default:
                            return null;
                    }
                }
            }
        }


        public object DataOraA
        {
            get
            {
                if (this.Value == null)
                    return null;
                else
                {
                    switch (this.Value.ToString())
                    {
                        case C_RNG_N24H:
                            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).AddDays(1);
                        case C_RNG_N48H:
                            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).AddDays(2);
                        case C_RNG_N7G:
                            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).AddDays(7);
                        case C_RNG_N30G:
                            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).AddDays(30);
                        case C_RNG_N60G:
                            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).AddDays(60);

                        case C_RNG_N1M:
                            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).AddMonths(1);
                        case C_RNG_N6M:
                            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).AddMonths(6);
                        case C_RNG_N1Y:
                            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).AddYears(1);
                        case C_RNG_N5Y:
                            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).AddYears(5);

                        case C_RNG_DOMANI:
                            DateTime dtdomaniend = DateTime.Now.Date.AddDays(1);
                            dtdomaniend = new DateTime(dtdomaniend.Year, dtdomaniend.Month, dtdomaniend.Day, 23, 59, 59);
                            return dtdomaniend;

                        case C_RNG_OGGI:
                            DateTime dtoggiend = DateTime.Now.Date;
                            dtoggiend = new DateTime(dtoggiend.Year, dtoggiend.Month, dtoggiend.Day, 23, 59, 59);
                            return dtoggiend;
                        default:
                            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                    }
                }
            }
        }

        #endregion

        #region SUB

        public static string getRangeDescription(string rangeCode)
        {
            string sRet = "";

            switch (rangeCode)
            {
                case C_RNG_OGGI:
                    sRet = @"Oggi";
                    break;

                case C_RNG_DOMANI:
                    sRet = @"Domani";
                    break;

                case C_RNG_N24H:
                    sRet = @"Prossime 24h";
                    break;
                case C_RNG_N48H:
                    sRet = @"Prossime 48h";
                    break;
                case C_RNG_N7G:
                    sRet = @"Prossimi 7gg";
                    break;
                case C_RNG_N30G:
                    sRet = @"Prossimi 30gg";
                    break;
                case C_RNG_N60G:
                    sRet = @"Prossimi 60gg";
                    break;


                case C_RNG_N1M:
                    sRet = @"Prossimo Mese";
                    break;
                case C_RNG_N6M:
                    sRet = @"Prossimi 6 Mesi";
                    break;
                case C_RNG_N1Y:
                    sRet = @"Prossimi 12 Mesi";
                    break;
                case C_RNG_N5Y:
                    sRet = @"Prossimi 5 Anni";
                    break;


                case C_RNG_24H:
                    sRet = @"Ultime 24h";
                    break;
                case C_RNG_48H:
                    sRet = @"Ultime 48h";
                    break;
                case C_RNG_7G:
                    sRet = @"Ultimi 7gg";
                    break;
                case C_RNG_30G:
                    sRet = @"Ultimi 30gg";
                    break;
                case C_RNG_60G:
                    sRet = @"Ultimi 60gg";
                    break;
                case C_RNG_90G:
                    sRet = @"Ultimi 90gg";
                    break;
                case C_RNG_6M:
                    sRet = @"Ultimi 6 mesi";
                    break;
                case C_RNG_12M:
                    sRet = @"Ultimi 12 mesi";
                    break;
                case C_RNG_24M:
                    sRet = @"Ultimi 24 mesi";
                    break;
                case C_RNG_48M:
                    sRet = @"Ultimi 48 mesi";
                    break;

                case C_RNG_EPI:
                    sRet = @"Episodio";
                    break;

                default:
                    sRet = "";
                    break;
            }

            return sRet;
        }

        private void loadItems()
        {

            this.Items.Clear();

            ValueListItem oVal1 = new ValueListItem(null, " ");
            this.Items.Add(oVal1);

            if (_disponibilitaAgende)
            {
                ValueListItem oVald2 = new ValueListItem(C_RNG_N1M, getRangeDescription(C_RNG_N1M)); this.Items.Add(oVald2);
                ValueListItem oVald3 = new ValueListItem(C_RNG_N6M, getRangeDescription(C_RNG_N6M)); this.Items.Add(oVald3);
                ValueListItem oVald4 = new ValueListItem(C_RNG_N1Y, getRangeDescription(C_RNG_N1Y)); this.Items.Add(oVald4);
                ValueListItem oVald5 = new ValueListItem(C_RNG_N5Y, getRangeDescription(C_RNG_N5Y)); this.Items.Add(oVald5);
            }

            if (_domani)
            {
                ValueListItem oVal22 = new ValueListItem(C_RNG_DOMANI, getRangeDescription(C_RNG_DOMANI)); this.Items.Add(oVal22);
            }

            if (_oggi)
            {
                ValueListItem oVal23 = new ValueListItem(C_RNG_OGGI, getRangeDescription(C_RNG_OGGI)); this.Items.Add(oVal23);
            }

            if (_futuro && !_disponibilitaAgende)
            {

                ValueListItem oVal2 = new ValueListItem(C_RNG_N24H, getRangeDescription(C_RNG_N24H)); this.Items.Add(oVal2);
                ValueListItem oVal3 = new ValueListItem(C_RNG_N48H, getRangeDescription(C_RNG_N48H)); this.Items.Add(oVal3);
                ValueListItem oVal4 = new ValueListItem(C_RNG_N7G, getRangeDescription(C_RNG_N7G)); this.Items.Add(oVal4);
                ValueListItem oVal5 = new ValueListItem(C_RNG_N30G, getRangeDescription(C_RNG_N30G)); this.Items.Add(oVal5);
                ValueListItem oVal6 = new ValueListItem(C_RNG_N60G, getRangeDescription(C_RNG_N60G)); this.Items.Add(oVal6);
            }

            if (!_disponibilitaAgende)
            {
                ValueListItem oVal12 = new ValueListItem(C_RNG_24H, getRangeDescription(C_RNG_24H)); this.Items.Add(oVal12);
                ValueListItem oVal13 = new ValueListItem(C_RNG_48H, getRangeDescription(C_RNG_48H)); this.Items.Add(oVal13);
                ValueListItem oVal14 = new ValueListItem(C_RNG_7G, getRangeDescription(C_RNG_7G)); this.Items.Add(oVal14);
                ValueListItem oVal15 = new ValueListItem(C_RNG_30G, getRangeDescription(C_RNG_30G)); this.Items.Add(oVal15);
                ValueListItem oVal16 = new ValueListItem(C_RNG_60G, getRangeDescription(C_RNG_60G)); this.Items.Add(oVal16);
                ValueListItem oVal19 = new ValueListItem(C_RNG_90G, getRangeDescription(C_RNG_90G)); this.Items.Add(oVal19);
                ValueListItem oVal17 = new ValueListItem(C_RNG_6M, getRangeDescription(C_RNG_6M)); this.Items.Add(oVal17);

                if (_passatoremoto)
                {
                    ValueListItem oVal20 = new ValueListItem(C_RNG_12M, getRangeDescription(C_RNG_12M)); this.Items.Add(oVal20);
                    ValueListItem oVal21 = new ValueListItem(C_RNG_24M, getRangeDescription(C_RNG_24M)); this.Items.Add(oVal21);
                    ValueListItem oVal22 = new ValueListItem(C_RNG_48M, getRangeDescription(C_RNG_48M)); this.Items.Add(oVal22);
                }
            }

            if (_dataEpisodio > DateTime.MinValue)
            {
                ValueListItem oVal18 = new ValueListItem(C_RNG_EPI, getRangeDescription(C_RNG_EPI)); this.Items.Add(oVal18);
            }
        }

        #endregion

        #region OVERRIDE

        protected override void OnEndInit()
        {
            base.OnEndInit();

            loadItems();
        }

        #endregion

    }

    public class ucEasyHourRange : ucEasyComboEditor
    {

        #region DICHIARAZIONI

        private DateTime _datainizio = DateTime.Now;
        private EnumHourRange _rangeore = EnumHourRange.Now;

        public enum EnumHourRange
        {
            [Description("Precedenti 24 ore")]
            Prev24 = -24,
            [Description("Precedenti 18 ore")]
            Prev18 = -18,
            [Description("Precedenti 12 ore")]
            Prev12 = -12,
            [Description("Precedenti 6 ore")]
            Prev6 = -6,
            [Description("Precedenti 6 ore")]
            Prev3 = -3,
            [Description("Solo Ora Attuale")]
            Now = 0,
            [Description("Prossime 3 ore")]
            Next3 = 3,
            [Description("Prossime 6 ore")]
            Next6 = 6,
            [Description("Prossime 12 ore")]
            Next12 = 12,
            [Description("Prossime 18 ore")]
            Next18 = 18,
            [Description("Prossime 24 ore")]
            Next24 = 24
        }

        #endregion

        public ucEasyHourRange()
        {
        }

        #region PROPERTIES

        public DateTime DataInizio
        {
            get { return _datainizio; }
            set
            {
                if (_datainizio != value)
                {
                    _datainizio = value;
                    loadItems();
                }
            }
        }

        public EnumHourRange RangeOre
        {
            get { return _rangeore; }
            set
            {
                if (_rangeore != value)
                {
                    _rangeore = value;
                    loadItems();
                }
            }
        }

        #endregion

        #region PRIVATE SUB

        private void loadItems()
        {

            ValueListItem oVal = null;
            DateTime valoreitem = new DateTime(this.DataInizio.Year, this.DataInizio.Month, this.DataInizio.Day, this.DataInizio.Hour, 0, 0);

            this.Items.Clear();

            if ((int)this.RangeOre > 0)
                for (int i = 1; i <= (int)this.RangeOre; i++)
                {
                    valoreitem = valoreitem.AddHours(1);
                    oVal = new ValueListItem(valoreitem, valoreitem.ToString("dd/MM/yyyy HH:mm"));
                    this.Items.Add(oVal);
                }
            else
                for (int i = -1; i >= (int)this.RangeOre; i--)
                {
                    valoreitem = valoreitem.AddHours(-1);
                    oVal = new ValueListItem(valoreitem, valoreitem.ToString("dd/MM/yyyy HH:mm"));
                    this.Items.Add(oVal);
                }
        }

        #endregion

        #region OVERRIDE

        protected override void OnEndInit()
        {
            base.OnEndInit();

            loadItems();
        }

        #endregion

    }

    public class ucEasyTreeView : Infragistics.Win.UltraWinTree.UltraTree, Interfacce.IEasyResizableText
    {
        private easyStatics.easyRelativeDimensions _textFontRelativeDimension = easyStatics.easyRelativeDimensions._undefined;

        public ucEasyTreeView()
        {
            this.UseOsThemes = DefaultableBoolean.False;
            this.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.DisplayStyle = Infragistics.Win.UltraWinTree.UltraTreeDisplayStyle.WindowsVista;
            this.HideSelection = false;

            this.Override.SelectedNodeAppearance.BackColor = Color.LightYellow;
            this.Override.SelectedNodeAppearance.BackColor2 = Color.Orange;

            this.Override.ActiveNodeAppearance = this.Override.SelectedNodeAppearance;
        }

        #region PROPERTIES

        public easyStatics.easyRelativeDimensions TextFontRelativeDimension
        {
            get
            {
                return _textFontRelativeDimension;
            }
            set
            {
                _textFontRelativeDimension = value;
                setTextFont();
            }
        }

        #endregion

        #region PRIVATE SUB

        private void setTextFont()
        {
            try
            {
                if (_textFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined)
                {
                    this.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(_textFontRelativeDimension);
                }
                else
                {
                    this.Appearance.FontData.SizeInPoints = 0;
                }

                if (this.Override.Multiline == Infragistics.Win.DefaultableBoolean.True)
                {
                    if (_textFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined && easyStatics.getFontSizeInPointsFromRelativeDimension(_textFontRelativeDimension) > 8)
                        this.LeftImagesSize = new Size(Convert.ToInt32(easyStatics.getFontSizeInPointsFromRelativeDimension(_textFontRelativeDimension) * 2.2), Convert.ToInt32(easyStatics.getFontSizeInPointsFromRelativeDimension(_textFontRelativeDimension) * 2.2));
                    else
                        this.LeftImagesSize = new Size(16, 16);
                }
                else
                {
                    if (_textFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined && easyStatics.getFontSizeInPointsFromRelativeDimension(_textFontRelativeDimension) > 16)
                        this.LeftImagesSize = new Size(Convert.ToInt32(easyStatics.getFontSizeInPointsFromRelativeDimension(_textFontRelativeDimension)), Convert.ToInt32(easyStatics.getFontSizeInPointsFromRelativeDimension(_textFontRelativeDimension)));
                    else
                        this.LeftImagesSize = new Size(16, 16);
                }
                this.RightImagesSize = this.LeftImagesSize;
            }
            catch (Exception)
            {
            }
        }


        #endregion

        #region OVERRIDE EVENTS

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            setTextFont();
        }

        #endregion

    }

    public class ucEasyTabControl : Infragistics.Win.UltraWinTabControl.UltraTabControl, Interfacce.IEasyResizableText
    {
        private easyStatics.easyRelativeDimensions _textFontRelativeDimension = easyStatics.easyRelativeDimensions._undefined;

        public ucEasyTabControl()
        {
            this.UseOsThemes = DefaultableBoolean.False;
            this.TabLayoutStyle = Infragistics.Win.UltraWinTabs.TabLayoutStyle.SingleRowSizeToFit;
            this.ViewStyle = Infragistics.Win.UltraWinTabControl.ViewStyle.Office2007;
            this.Appearance.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.SelectedTabAppearance.FontData.Bold = DefaultableBoolean.True;
            this.SelectedTabAppearance.ForeColor = System.Drawing.Color.MidnightBlue;

        }

        #region PROPERTIES

        public easyStatics.easyRelativeDimensions TextFontRelativeDimension
        {
            get
            {
                return _textFontRelativeDimension;
            }
            set
            {
                _textFontRelativeDimension = value;
                setTextFont();
            }
        }

        #endregion

        #region PRIVATE SUB

        private void setTextFont()
        {
            try
            {
                if (_textFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined)
                {
                    this.TabHeaderAreaAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(_textFontRelativeDimension);
                    this.TabSize = new System.Drawing.Size(0, Convert.ToInt32(this.TabHeaderAreaAppearance.FontData.SizeInPoints * 2.2));
                }
                else
                {
                    this.TabHeaderAreaAppearance.FontData.SizeInPoints = 0;
                    this.TabSize = new System.Drawing.Size(0, 0);
                }

            }
            catch (Exception)
            {
            }
        }


        #endregion

        #region OVERRIDE EVENTS

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            setTextFont();
        }

        #endregion

    }

    public class ucEasyGroupBox : Infragistics.Win.Misc.UltraGroupBox, Interfacce.IEasyResizableText
    {
        private easyStatics.easyRelativeDimensions _textFontRelativeDimension = easyStatics.easyRelativeDimensions._undefined;

        public ucEasyGroupBox()
        {
            this.UseOsThemes = DefaultableBoolean.False;
            this.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
        }

        #region PROPERTIES

        public easyStatics.easyRelativeDimensions TextFontRelativeDimension
        {
            get
            {
                return _textFontRelativeDimension;
            }
            set
            {
                _textFontRelativeDimension = value;
                setTextFont();
            }
        }

        #endregion

        #region PRIVATE SUB

        private void setTextFont()
        {
            try
            {
                if (_textFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined)
                {
                    this.HeaderAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(_textFontRelativeDimension);
                }
                else
                {
                    this.HeaderAppearance.FontData.SizeInPoints = 0;
                }

            }
            catch (Exception)
            {
            }
        }


        #endregion

        #region OVERRIDE EVENTS

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            setTextFont();
        }

        #endregion

    }

    public class ucEasyTableLayoutPanel : TableLayoutPanel
    {
        public ucEasyTableLayoutPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
              ControlStyles.OptimizedDoubleBuffer |
              ControlStyles.UserPaint, true);


        }

        public ucEasyTableLayoutPanel(IContainer container)
        {
            container.Add(this);
            SetStyle(ControlStyles.AllPaintingInWmPaint |
              ControlStyles.OptimizedDoubleBuffer |
              ControlStyles.UserPaint, true);
        }

        protected override Point ScrollToControl(Control activeControl)
        {
            return this.DisplayRectangle.Location;
        }

    }

    public class ucEasyFlowLayoutPanel : FlowLayoutPanel
    {
        public ucEasyFlowLayoutPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
              ControlStyles.OptimizedDoubleBuffer |
              ControlStyles.UserPaint, true);


        }

        public ucEasyFlowLayoutPanel(IContainer container)
        {
            container.Add(this);
            SetStyle(ControlStyles.AllPaintingInWmPaint |
              ControlStyles.OptimizedDoubleBuffer |
              ControlStyles.UserPaint, true);
        }

    }

    public class ucEasyCheckEditor : Infragistics.Win.UltraWinEditors.UltraCheckEditor
    {

        public ucEasyCheckEditor()
        {
            this.Appearance.ImageBackground = ScciCore.Properties.Resources.unChecked;
            this.Appearance.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.Appearance.BackColor = Color.Transparent;
            this.Appearance.ThemedElementAlpha = Infragistics.Win.Alpha.Transparent;
            this.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Borderless;
            this.CheckedAppearance.ImageBackground = ScciCore.Properties.Resources.Checked;
            this.CheckedAppearance.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.CheckedAppearance.ThemedElementAlpha = Infragistics.Win.Alpha.Transparent;
            this.Dock = DockStyle.Fill;
            this.Enabled = true;
            this.Style = Infragistics.Win.EditCheckStyle.Button;
            this.Text = string.Empty;
        }

    }

    public class ucEasyOptionSet : Infragistics.Win.UltraWinEditors.UltraOptionSet, Interfacce.IEasyResizableText
    {

        private easyStatics.easyRelativeDimensions _textFontRelativeDimension = easyStatics.easyRelativeDimensions._undefined;

        public ucEasyOptionSet()
        {
            this.Appearance.BackColor = Color.Transparent;
            this.Appearance.ThemedElementAlpha = Infragistics.Win.Alpha.Transparent;
            this.Dock = DockStyle.Fill;
            this.Enabled = true;
            this.Text = string.Empty;
        }

        #region PROPERTIES

        public easyStatics.easyRelativeDimensions TextFontRelativeDimension
        {
            get
            {
                return _textFontRelativeDimension;
            }
            set
            {
                _textFontRelativeDimension = value;
                setTextFont();
            }
        }

        #endregion

        #region PRIVATE SUB

        private void setTextFont()
        {
            try
            {
                if (_textFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined)
                {
                    this.ItemAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(_textFontRelativeDimension);
                }
                else
                {
                    this.ItemAppearance.FontData.SizeInPoints = 0;
                }

            }
            catch (Exception)
            {
            }
        }


        #endregion

        #region OVERRIDE EVENTS

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            setTextFont();
        }

        #endregion

    }

    public class ucEasyToolTipManager : Infragistics.Win.UltraWinToolTip.UltraToolTipManager, Interfacce.IEasyResizableText
    {
        private easyStatics.easyRelativeDimensions _textFontRelativeDimension = easyStatics.easyRelativeDimensions._undefined;

        public ucEasyToolTipManager(System.ComponentModel.IContainer container) : base(container)
        {
            this.DisplayStyle = ToolTipDisplayStyle.Office2007;
        }

        #region PROPERTIES

        public easyStatics.easyRelativeDimensions TextFontRelativeDimension
        {
            get
            {
                return _textFontRelativeDimension;
            }
            set
            {
                _textFontRelativeDimension = value;
                setTextFont();
            }
        }

        #endregion

        #region PRIVATE SUB

        private void setTextFont()
        {
            try
            {
                if (_textFontRelativeDimension != easyStatics.easyRelativeDimensions._undefined)
                {
                    this.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(_textFontRelativeDimension);
                }
                else
                {
                    this.Appearance.FontData.SizeInPoints = 9;
                }

            }
            catch (Exception)
            {
            }
        }

        #region OVERRIDE EVENTS



        #endregion


        #endregion

    }

}
