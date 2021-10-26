using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using Infragistics.Win.UltraWinToolbars;

namespace UnicodeSrl.ScciCore
{
    public partial class ucRichTextBox : UserControl, Interfacce.IViewRichTextBox
    {
        public ucRichTextBox()
        {
            InitializeComponent();
        }

        #region Declare

        private Font _setFont = null;
        private bool _runtimeCheck = false;

        public event ChangeEventHandler RtfChange;
        public event EventHandler RtfClick;
        public event LinkClickedEventHandler RtfLinkClicked;

        #endregion

        #region Interface

        public Font ViewFont
        {
            get
            {
                return this.rtbRichTextBox.Font;
            }
            set
            {
                _setFont = value;
                this.rtbRichTextBox.Font = value;
            }
        }

        public bool ViewReadOnly
        {
            get
            {
                return this.rtbRichTextBox.ReadOnly;
            }
            set
            {
                this.rtbRichTextBox.ReadOnly = value;
                this.ViewShowToolbar = !value;
                this.UltraToolbarsManager.Enabled = !value;
            }
        }

        public string ViewRtf
        {
            get
            {
                return this.rtbRichTextBox.Rtf;
            }
            set
            {
                this.rtbRichTextBox.Rtf = value;
            }
        }

        public bool ViewShowInsertImage
        {
            get
            {
                return this.UltraToolbarsManager.Tools["Inserisci Immagine"].SharedProps.Visible;
            }
            set
            {
                this.UltraToolbarsManager.Tools["Inserisci Immagine"].SharedProps.Visible = value;
            }
        }

        public bool ViewShowPlainText
        {
            get
            {
                return this.UltraToolbarsManager.Tools["PlainText"].SharedProps.Visible;
            }
            set
            {
                this.UltraToolbarsManager.Tools["PlainText"].SharedProps.Visible = value;
            }
        }

        public bool ViewShowToolbar
        {
            get
            {
                return this.UltraToolbarsManager.Toolbars["UltraToolbarMain"].Visible;
            }
            set
            {
                this.UltraToolbarsManager.Toolbars["UltraToolbarMain"].Visible = value;
            }
        }

        public ToolbarStyle ViewToolbarStyle
        {
            get
            {
                return this.UltraToolbarsManager.Style;
            }
            set
            {
                this.UltraToolbarsManager.Style = value;
            }
        }

        public string ViewText
        {
            get
            {
                return this.rtbRichTextBox.Text;
            }
            set
            {
                this.rtbRichTextBox.Text = value;
            }
        }

        public bool ViewUseLargeImages
        {
            get
            {
                return (this.UltraToolbarsManager.ToolbarSettings.UseLargeImages == Infragistics.Win.DefaultableBoolean.False ? false : true);
            }
            set
            {
                this.UltraToolbarsManager.ToolbarSettings.UseLargeImages = (value == false ? Infragistics.Win.DefaultableBoolean.False : Infragistics.Win.DefaultableBoolean.True);
            }
        }

        public void ViewInit()
        {
            this.InitializeUltraToolbarsManager();
        }

        #endregion

        #region UltraToolBar

        private void InitializeUltraToolbarsManager()
        {

            try
            {

                var utb = this.UltraToolbarsManager;

                utb.Style = ToolbarStyle.Office2007;
                utb.ToolbarSettings.UseLargeImages = Infragistics.Win.DefaultableBoolean.False;

                utb.Tools["Font"].SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.FontCambia_32;
                utb.Tools["Font"].SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.FontCambia_16;
                utb.Tools["Colore Testo"].SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.FontColor_32;
                utb.Tools["Colore Testo"].SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.FontColor_16;
                utb.Tools["Colore Sfondo"].SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.FontEvidenzia_32;
                utb.Tools["Colore Sfondo"].SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.FontEvidenzia_16;

                utb.Tools["Grassetto"].SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.FontBold_32;
                utb.Tools["Grassetto"].SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.FontBold_16;

                utb.Tools["Italico"].SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.FontItalico_32;
                utb.Tools["Italico"].SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.FontItalico_16;

                utb.Tools["Sottolineato"].SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.FontSottolineato_32;
                utb.Tools["Sottolineato"].SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.FontSottolineato_16;

                utb.Tools["Allineamento Sinistra"].SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.FontAllineaSx_32;
                utb.Tools["Allineamento Sinistra"].SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.FontAllineaSx_16;

                utb.Tools["Allineamento Centro"].SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.FontAllineaCentrato_32;
                utb.Tools["Allineamento Centro"].SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.FontAllineaCentrato_16;

                utb.Tools["Allineamento Destra"].SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.FontAllineaDx_32;
                utb.Tools["Allineamento Destra"].SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.FontAllineaDx_16;

                utb.Tools["AggiungiEP"].SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.FontPunti_32;
                utb.Tools["AggiungiEP"].SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.FontPunti_16;

                utb.Tools["RimuoviEP"].SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.FontPuntiRimuovi_32;
                utb.Tools["RimuoviEP"].SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.FontPuntiRimuovi_16;

                utb.Tools["Indentare Più"].SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.FontIndentaPiu_32;
                utb.Tools["Indentare Più"].SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.FontIndentaPiu_16;

                utb.Tools["Indentare Meno"].SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.FontIndentaMeno_32;
                utb.Tools["Indentare Meno"].SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.FontIndentaMeno_16;

                utb.Tools["Inserisci Immagine"].SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.Immagine_32;
                utb.Tools["Inserisci Immagine"].SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.Immagine_16;

                utb.Tools["Modifica"].SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.Modifica_32;
                utb.Tools["Modifica"].SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.Modifica_16;

                utb.Tools["Annulla"].SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.Annulla_32;
                utb.Tools["Annulla"].SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.Annulla_16;

                utb.Tools["Ripeti"].SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.Ripristina_32;
                utb.Tools["Ripeti"].SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.Ripristina_16;

                utb.Tools["Copia"].SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.Copia_32;
                utb.Tools["Copia"].SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.Copia_16;

                utb.Tools["Incolla"].SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.Incolla_32;
                utb.Tools["Incolla"].SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.Incolla_16;

                utb.Tools["Taglia"].SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.Taglia_32;
                utb.Tools["Taglia"].SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.Taglia_16;

                utb.Tools["Seleziona Tutto"].SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.FontSelezionaTutti_32;
                utb.Tools["Seleziona Tutto"].SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.FontSelezionaTutti_16;


                utb.Tools["PlainText"].SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.RTFCancellaFormato_32;
                utb.Tools["PlainText"].SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.RTFCancellaFormato_16;

            }
            catch (Exception)
            {

            }

        }

        private void ActionToolClick(ToolBase oTool)
        {

            try
            {

                switch (oTool.Key)
                {

                    case "Font":
                        if (this.rtbRichTextBox.SelectionFont != null)
                        {
                            FontDialog.Font = this.rtbRichTextBox.SelectionFont;
                        }
                        else
                        {
                            FontDialog.Font = null;
                        }
                        FontDialog.ShowApply = true;
                        if (FontDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            this.rtbRichTextBox.SelectionFont = FontDialog.Font;
                        }
                        break;

                    case "Colore Testo":
                        ColorDialog.Color = this.rtbRichTextBox.ForeColor;
                        if (ColorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            this.rtbRichTextBox.SelectionColor = ColorDialog.Color;
                        }
                        break;

                    case "Colore Sfondo":
                        ColorDialog.Color = this.rtbRichTextBox.SelectionBackColor;
                        if (ColorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            this.rtbRichTextBox.SelectionBackColor = ColorDialog.Color;
                        }
                        break;

                    case "Grassetto":
                        setFontStyle(oTool.Key);
                        break;

                    case "Italico":
                        setFontStyle(oTool.Key);
                        break;

                    case "Sottolineato":
                        setFontStyle(oTool.Key);
                        break;

                    case "Allineamento Sinistra":
                        setAllineamento();
                        break;

                    case "Allineamento Centro":
                        setAllineamento();
                        break;

                    case "Allineamento Destra":
                        setAllineamento();
                        break;

                    case "Annulla":
                        if (this.rtbRichTextBox.CanUndo) { this.rtbRichTextBox.Undo(); }
                        break;

                    case "Ripeti":
                        if (this.rtbRichTextBox.CanRedo) { this.rtbRichTextBox.Redo(); }
                        break;

                    case "Taglia":
                        this.rtbRichTextBox.Cut();
                        break;

                    case "Copia":
                        this.rtbRichTextBox.Copy();
                        break;

                    case "Incolla":
                        this.rtbRichTextBox.Paste();
                        break;

                    case "Seleziona Tutto":
                        this.rtbRichTextBox.SelectAll();
                        break;

                    case "Inserisci Immagine":
                        OpenFileDialog.Title = "Seleziona Immagine";
                        OpenFileDialog.DefaultExt = "rtf";
                        OpenFileDialog.Filter = "PNG Files|*.png|Bitmap Files|*.bmp|JPEG Files|*.jpg|GIF Files|*.gif";
                        OpenFileDialog.FilterIndex = 1;
                        if (OpenFileDialog.ShowDialog() == DialogResult.OK)
                        {

                            string strImagePath = OpenFileDialog.FileName;
                            Image img;
                            img = Image.FromFile(strImagePath);
                            Clipboard.SetDataObject(img);
                            DataFormats.Format df;
                            df = DataFormats.GetFormat(DataFormats.Bitmap);
                            if (this.rtbRichTextBox.CanPaste(df)) { this.rtbRichTextBox.Paste(df); }

                        }
                        break;

                    case "AggiungiEP":
                        this.rtbRichTextBox.BulletIndent = 10;
                        this.rtbRichTextBox.SelectionBullet = true;
                        break;

                    case "RimuoviEP":
                        this.rtbRichTextBox.SelectionBullet = false;
                        break;

                    case "Indentare Più":
                        this.rtbRichTextBox.SelectionIndent += 5;
                        break;

                    case "Indentare Meno":
                        if (this.rtbRichTextBox.SelectionIndent >= 5)
                        {
                            this.rtbRichTextBox.SelectionIndent -= 5;
                        }
                        else
                        {
                            this.rtbRichTextBox.SelectionIndent = 0;
                        }
                        break;


                    case "PlainText":

                        string sText = this.rtbRichTextBox.Text;
                        this.rtbRichTextBox.Rtf = "";
                        if (_setFont != null) this.rtbRichTextBox.Font = _setFont;
                        this.rtbRichTextBox.Text = sText;
                        rtbRichTextBox_SelectionChanged(this.rtbRichTextBox, new EventArgs());



                        break;

                    default:
                        break;

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        private void setFontStyle(string toolKey)
        {
            try
            {
                if (this.rtbRichTextBox.SelectionFont != null)
                {

                    if (toolKey != null && toolKey.Trim() != "")
                    {
                        System.Drawing.Font currentFont = this.rtbRichTextBox.SelectionFont;
                        System.Drawing.FontStyle newFontStyle = currentFont.Style;

                        switch (toolKey)
                        {
                            case "Grassetto":
                                if (((StateButtonTool)this.UltraToolbarsManager.Tools[toolKey]).Checked)
                                    newFontStyle |= FontStyle.Bold;
                                else
                                    newFontStyle -= FontStyle.Bold;
                                break;

                            case "Italico":
                                if (((StateButtonTool)this.UltraToolbarsManager.Tools[toolKey]).Checked)
                                    newFontStyle |= FontStyle.Italic;
                                else
                                    newFontStyle -= FontStyle.Italic;
                                break;
                            case "Sottolineato":
                                if (((StateButtonTool)this.UltraToolbarsManager.Tools[toolKey]).Checked)
                                    newFontStyle |= FontStyle.Underline;
                                else
                                    newFontStyle -= FontStyle.Underline;
                                break;
                            default:
                                break;
                        }

                        this.rtbRichTextBox.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                    }
                    else
                    {

                        System.Drawing.Font currentFont = this.rtbRichTextBox.SelectionFont;
                        System.Drawing.FontStyle newFontStyle = FontStyle.Regular;

                        if (((StateButtonTool)this.UltraToolbarsManager.Tools["Grassetto"]).Checked) newFontStyle |= FontStyle.Bold;
                        if (((StateButtonTool)this.UltraToolbarsManager.Tools["Italico"]).Checked) newFontStyle |= FontStyle.Italic;
                        if (((StateButtonTool)this.UltraToolbarsManager.Tools["Sottolineato"]).Checked) newFontStyle |= FontStyle.Underline;

                        this.rtbRichTextBox.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                    }

                }
            }
            catch
            {
            }
        }

        private void setAllineamento()
        {
            try
            {
                if (((StateButtonTool)this.UltraToolbarsManager.Tools["Allineamento Centro"]).Checked)
                    this.rtbRichTextBox.SelectionAlignment = HorizontalAlignment.Center;
                else if (((StateButtonTool)this.UltraToolbarsManager.Tools["Allineamento Destra"]).Checked)
                    this.rtbRichTextBox.SelectionAlignment = HorizontalAlignment.Right;
                else
                    this.rtbRichTextBox.SelectionAlignment = HorizontalAlignment.Left;

            }
            catch
            {
            }
        }

        #endregion

        #region Events UserControl

        private void ucRichTextBox_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
                this.InitializeUltraToolbarsManager();
        }

        #endregion

        #region Events

        private void UltraToolbarsManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            if (!_runtimeCheck)
                this.ActionToolClick(e.Tool);
        }

        private void rtbRichTextBox_TextChanged(object sender, EventArgs e)
        {
            if (RtfChange != null) { RtfChange(sender, new EventArgs()); }
        }

        private void rtbRichTextBox_Click(object sender, EventArgs e)
        {
            if (RtfClick != null) { RtfClick(sender, new EventArgs()); }
        }

        private void rtbRichTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (RtfLinkClicked != null) { RtfLinkClicked(sender, new LinkClickedEventArgs(e.LinkText)); }
        }

        private void rtbRichTextBox_SelectionChanged(object sender, EventArgs e)
        {
            _runtimeCheck = true;
            try
            {
                ((StateButtonTool)this.UltraToolbarsManager.Tools["Grassetto"]).Checked = (this.rtbRichTextBox.SelectionFont != null && this.rtbRichTextBox.SelectionFont.Bold);
                ((StateButtonTool)this.UltraToolbarsManager.Tools["Italico"]).Checked = (this.rtbRichTextBox.SelectionFont != null && this.rtbRichTextBox.SelectionFont.Italic);
                ((StateButtonTool)this.UltraToolbarsManager.Tools["Sottolineato"]).Checked = (this.rtbRichTextBox.SelectionFont != null && this.rtbRichTextBox.SelectionFont.Underline);
            }
            catch
            {
            }
            try
            {
                switch (this.rtbRichTextBox.SelectionAlignment)
                {
                    case HorizontalAlignment.Center:
                        ((StateButtonTool)this.UltraToolbarsManager.Tools["Allineamento Centro"]).Checked = true;
                        break;
                    case HorizontalAlignment.Right:
                        ((StateButtonTool)this.UltraToolbarsManager.Tools["Allineamento Destra"]).Checked = true;
                        break;
                    case HorizontalAlignment.Left:
                    default:
                        ((StateButtonTool)this.UltraToolbarsManager.Tools["Allineamento Sinistra"]).Checked = true;
                        break;
                }
            }
            catch
            {
            }
            _runtimeCheck = false;
        }

        #endregion

    }

    static class RichTextExtensions
    {
        public static void ClearAllFormatting(this RichTextBox te, Font font)
        {
            CHARFORMAT2 fmt = new CHARFORMAT2();

            fmt.cbSize = Marshal.SizeOf(fmt);
            fmt.dwMask = CFM_ALL2;
            fmt.dwEffects = CFE_AUTOCOLOR | CFE_AUTOBACKCOLOR;
            fmt.szFaceName = font.FontFamily.Name;

            double size = font.Size;
            size /= 72; size *= 1440.0;
            fmt.yHeight = (int)size; fmt.yOffset = 0;
            fmt.crTextColor = 0;
            fmt.bCharSet = 1; fmt.bPitchAndFamily = 0; fmt.wWeight = 400; fmt.sSpacing = 0;
            fmt.crBackColor = 0;
            fmt.dwMask &= ~CFM_LCID; fmt.dwReserved = 0;
            fmt.sStyle = 0;
            fmt.wKerning = 0;
            fmt.bUnderlineType = 0;
            fmt.bAnimation = 0;
            fmt.bRevAuthor = 0;
            fmt.bReserved1 = 0;

            SendMessage(te.Handle, EM_SETCHARFORMAT, SCF_ALL, ref fmt);
        }

        private const UInt32 WM_USER = 0x0400;
        private const UInt32 EM_GETCHARFORMAT = (WM_USER + 58);
        private const UInt32 EM_SETCHARFORMAT = (WM_USER + 68);
        private const UInt32 SCF_ALL = 0x0004;
        private const UInt32 SCF_SELECTION = 0x0001;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, UInt32 wParam, ref CHARFORMAT2 lParam);

        [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Auto)]
        struct CHARFORMAT2
        {
            public int cbSize;
            public uint dwMask;
            public uint dwEffects;
            public int yHeight;
            public int yOffset;
            public int crTextColor;
            public byte bCharSet;
            public byte bPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szFaceName;
            public short wWeight;
            public short sSpacing;
            public int crBackColor;
            public int lcid;
            public int dwReserved;
            public short sStyle;
            public short wKerning;
            public byte bUnderlineType;
            public byte bAnimation;
            public byte bRevAuthor;
            public byte bReserved1;
        }

        #region CFE_
        const UInt32 CFE_BOLD = 0x0001;
        const UInt32 CFE_ITALIC = 0x0002;
        const UInt32 CFE_UNDERLINE = 0x0004;
        const UInt32 CFE_STRIKEOUT = 0x0008;
        const UInt32 CFE_PROTECTED = 0x0010;
        const UInt32 CFE_LINK = 0x0020;
        const UInt32 CFE_AUTOCOLOR = 0x40000000; const UInt32 CFE_SMALLCAPS = CFM_SMALLCAPS;
        const UInt32 CFE_ALLCAPS = CFM_ALLCAPS;
        const UInt32 CFE_HIDDEN = CFM_HIDDEN;
        const UInt32 CFE_OUTLINE = CFM_OUTLINE;
        const UInt32 CFE_SHADOW = CFM_SHADOW;
        const UInt32 CFE_EMBOSS = CFM_EMBOSS;
        const UInt32 CFE_IMPRINT = CFM_IMPRINT;
        const UInt32 CFE_DISABLED = CFM_DISABLED;
        const UInt32 CFE_REVISED = CFM_REVISED;

        const UInt32 CFE_AUTOBACKCOLOR = CFM_BACKCOLOR;
        #endregion
        #region CFM_
        const UInt32 CFM_BOLD = 0x00000001;
        const UInt32 CFM_ITALIC = 0x00000002;
        const UInt32 CFM_UNDERLINE = 0x00000004;
        const UInt32 CFM_STRIKEOUT = 0x00000008;
        const UInt32 CFM_PROTECTED = 0x00000010;
        const UInt32 CFM_LINK = 0x00000020; const UInt32 CFM_SIZE = 0x80000000;
        const UInt32 CFM_COLOR = 0x40000000;
        const UInt32 CFM_FACE = 0x20000000;
        const UInt32 CFM_OFFSET = 0x10000000;
        const UInt32 CFM_CHARSET = 0x08000000;

        const UInt32 CFM_SMALLCAPS = 0x0040; const UInt32 CFM_ALLCAPS = 0x0080; const UInt32 CFM_HIDDEN = 0x0100; const UInt32 CFM_OUTLINE = 0x0200; const UInt32 CFM_SHADOW = 0x0400; const UInt32 CFM_EMBOSS = 0x0800; const UInt32 CFM_IMPRINT = 0x1000; const UInt32 CFM_DISABLED = 0x2000;
        const UInt32 CFM_REVISED = 0x4000;

        const UInt32 CFM_BACKCOLOR = 0x04000000;
        const UInt32 CFM_LCID = 0x02000000;
        const UInt32 CFM_UNDERLINETYPE = 0x00800000; const UInt32 CFM_WEIGHT = 0x00400000;
        const UInt32 CFM_SPACING = 0x00200000; const UInt32 CFM_KERNING = 0x00100000; const UInt32 CFM_STYLE = 0x00080000; const UInt32 CFM_ANIMATION = 0x00040000; const UInt32 CFM_REVAUTHOR = 0x00008000;

        const UInt32 CFE_SUBSCRIPT = 0x00010000; const UInt32 CFE_SUPERSCRIPT = 0x00020000;
        const UInt32 CFM_SUBSCRIPT = (CFE_SUBSCRIPT | CFE_SUPERSCRIPT);
        const UInt32 CFM_SUPERSCRIPT = CFM_SUBSCRIPT;

        const UInt32 CFM_EFFECTS = (CFM_BOLD | CFM_ITALIC | CFM_UNDERLINE | CFM_COLOR |
                     CFM_STRIKEOUT | CFE_PROTECTED | CFM_LINK);
        const UInt32 CFM_ALL = (CFM_EFFECTS | CFM_SIZE | CFM_FACE | CFM_OFFSET | CFM_CHARSET);

        const UInt32 CFM_EFFECTS2 = (CFM_EFFECTS | CFM_DISABLED | CFM_SMALLCAPS | CFM_ALLCAPS
                            | CFM_HIDDEN | CFM_OUTLINE | CFM_SHADOW | CFM_EMBOSS
                            | CFM_IMPRINT | CFM_DISABLED | CFM_REVISED
                            | CFM_SUBSCRIPT | CFM_SUPERSCRIPT | CFM_BACKCOLOR);

        const UInt32 CFM_ALL2 = (CFM_ALL | CFM_EFFECTS2 | CFM_BACKCOLOR | CFM_LCID
                            | CFM_UNDERLINETYPE | CFM_WEIGHT | CFM_REVAUTHOR
                            | CFM_SPACING | CFM_KERNING | CFM_STYLE | CFM_ANIMATION);
        #endregion
    }

}
