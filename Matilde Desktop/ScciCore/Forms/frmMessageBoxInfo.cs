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
    public partial class frmMessageBoxInfo : frmBaseModale
    {
        public frmMessageBoxInfo()
        {
            InitializeComponent();
        }
        
        DialogResult _messageBoxRetun = DialogResult.Cancel;

        MessageBoxButtons _bottoni = MessageBoxButtons.OK;

        private bool _skipSelectAll = false;

                                                                                                                                public DialogResult CaricaInfoMessageBox(string messaggio,
                                                 string titolo,
                                                 string formcaption,
                                                 MessageBoxButtons bottoni,
                                                 MessageBoxIcon icona,
                                                 string captionAvanti = null,
                                                 string captionIndietro = null,
                                                 easyStatics.easyRelativeDimensions messaggioFontRelativeDimension = easyStatics.easyRelativeDimensions.Medium,
                                                 easyStatics.easyRelativeDimensions titoloFontRelativeDimension = easyStatics.easyRelativeDimensions.XLarge,
                                                 bool bottomHomeHidden = false,
                                                 bool skipSelectAll = false)
        {

            try
            {

                this.Text = formcaption;

                this.ucEasyLabelTitolo.TextFontRelativeDimension = titoloFontRelativeDimension;
                this.ucEasyTextBoxMessaggio.TextFontRelativeDimension = messaggioFontRelativeDimension;

                this.ucEasyLabelTitolo.Text = titolo;
                this.ucEasyTextBoxMessaggio.Text = messaggio;

                _skipSelectAll = skipSelectAll;

                switch (icona)
                {
                                                            case MessageBoxIcon.Stop:
                        this.ucEasyPictureBox.Image = Properties.Resources.msg_error;
                        break;
                                        case MessageBoxIcon.Information:
                        this.ucEasyPictureBox.Image = Properties.Resources.msg_info;
                        break;
                    case MessageBoxIcon.None:
                        this.ucEasyPictureBox.Image = null;
                        break;
                    case MessageBoxIcon.Question:
                        this.ucEasyPictureBox.Image = Properties.Resources.msg_question;
                        break;
                                        case MessageBoxIcon.Warning:
                        this.ucEasyPictureBox.Image = Properties.Resources.msg_warning;
                        break;
                    default:
                        this.ucEasyPictureBox.Image = Properties.Resources.msg_info;
                        break;
                }

                _bottoni = bottoni;
                switch (bottoni)
                {
                    case MessageBoxButtons.OK:
                        this.PulsanteAvantiVisibile = false;
                        this.PulsanteIndietroVisibile = true;
                        if (captionIndietro == null || captionIndietro == "")
                            this.PulsanteIndietroTesto = "INDIETRO";
                        else
                            this.PulsanteIndietroTesto = captionIndietro;
                        break;

                    case MessageBoxButtons.YesNo:
                        this.PulsanteAvantiVisibile = true;
                        if (captionAvanti == null || captionAvanti == "")
                            this.PulsanteAvantiTesto = "SI";
                        else
                            this.PulsanteAvantiTesto = captionAvanti;

                        this.PulsanteIndietroVisibile = true;
                        if (captionIndietro == null || captionIndietro == "")
                            this.PulsanteIndietroTesto = "NO";
                        else
                            this.PulsanteIndietroTesto = captionIndietro;

                        break;

                    default:
                        this.PulsanteAvantiVisibile = true;
                        if (captionAvanti == null || captionAvanti == "")
                            this.PulsanteAvantiTesto = "AVANTI";
                        else
                            this.PulsanteAvantiTesto = captionAvanti;

                        this.PulsanteIndietroVisibile = true;
                        if (captionIndietro == null || captionIndietro == "")
                            this.PulsanteIndietroTesto = "INDIETRO";
                        else
                            this.PulsanteIndietroTesto = captionIndietro;

                        break;
                }

                                                if (bottomHomeHidden)
                    this.ucBottomModale.TableLayoutPanelHome.Visible = false;

                TopMost = true;
                Focus();
                BringToFront();

                this.ShowDialog();

            }
            catch (Exception)
            {
            }

            return _messageBoxRetun;

        }

        private void frmMessageBox_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            _messageBoxRetun = System.Windows.Forms.DialogResult.OK;
            switch (_bottoni)
            {
                case MessageBoxButtons.AbortRetryIgnore:
                    _messageBoxRetun = System.Windows.Forms.DialogResult.Ignore;
                    break;
                case MessageBoxButtons.OK:
                case MessageBoxButtons.OKCancel:
                    _messageBoxRetun = System.Windows.Forms.DialogResult.OK;
                    break;
                case MessageBoxButtons.RetryCancel:
                    _messageBoxRetun = System.Windows.Forms.DialogResult.Retry;
                    break;
                case MessageBoxButtons.YesNo:
                case MessageBoxButtons.YesNoCancel:
                    _messageBoxRetun = System.Windows.Forms.DialogResult.Yes;
                    break;
                default:
                    _messageBoxRetun = System.Windows.Forms.DialogResult.OK;
                    break;
            }
            this.Close();
        }

        private void frmMessageBox_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
                        _messageBoxRetun = System.Windows.Forms.DialogResult.Cancel;
            switch (_bottoni)
            {
                case MessageBoxButtons.AbortRetryIgnore:
                    _messageBoxRetun = System.Windows.Forms.DialogResult.Abort;
                    break;
                case MessageBoxButtons.OK:
                    _messageBoxRetun = System.Windows.Forms.DialogResult.Cancel;
                    break;
                case MessageBoxButtons.OKCancel:
                    _messageBoxRetun = System.Windows.Forms.DialogResult.Cancel;
                    break;
                case MessageBoxButtons.RetryCancel:
                    _messageBoxRetun = System.Windows.Forms.DialogResult.Cancel;
                    break;
                case MessageBoxButtons.YesNo:
                    _messageBoxRetun = System.Windows.Forms.DialogResult.No;
                    break;
                case MessageBoxButtons.YesNoCancel:
                    _messageBoxRetun = System.Windows.Forms.DialogResult.Cancel;
                    break;
                default:
                    _messageBoxRetun = System.Windows.Forms.DialogResult.Cancel;
                    break;
            }
            this.Close();
        }

        private void frmMessageBoxInfo_Shown(object sender, EventArgs e)
        {
            try
            {
                if (_skipSelectAll && this.ucEasyTextBoxMessaggio.Text != string.Empty)
                {
                    this.ucEasyTextBoxMessaggio.Focus();
                    this.ucEasyTextBoxMessaggio.SelectionStart = 0;
                    this.ucEasyTextBoxMessaggio.SelectionLength = 0;
                    this.ucEasyTextBoxMessaggio.SelectedText = "";
                }
            }
            catch
            {
            }
        }
    }
}
