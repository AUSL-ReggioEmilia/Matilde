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
    public partial class frmMessageBox : frmBaseModale
    {
        DialogResult _messageBoxRetun = DialogResult.Cancel;

        MessageBoxButtons _bottoni = MessageBoxButtons.OK;

        public frmMessageBox()
        {
            InitializeComponent();
        }

        public DialogResult CaricaMessageBox(string messaggio, string caption)
        {
            return CaricaMessageBox(messaggio, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public DialogResult CaricaMessageBox(string messaggio, string caption, MessageBoxButtons bottoni, MessageBoxIcon icona)
        {
            return CaricaMessageBox(messaggio, caption, bottoni, icona, false);
        }
        public DialogResult CaricaMessageBox(string messaggio, string caption, MessageBoxButtons bottoni, MessageBoxIcon icona, bool NascondiTitolo)
        {
            try
            {
                this.Text = caption;

                this.ucEasyLabel.Text = messaggio;

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
                        this.PulsanteIndietroTesto = "INDIETRO";
                        break;

                    case MessageBoxButtons.YesNo:
                        this.PulsanteAvantiVisibile = true;
                        this.PulsanteAvantiTesto = "SI";
                        this.PulsanteIndietroVisibile = true;
                        this.PulsanteIndietroTesto = "NO";
                        break;

                    default:
                        this.PulsanteAvantiVisibile = true;
                        this.PulsanteAvantiTesto = "AVANTI";
                        this.PulsanteIndietroVisibile = true;
                        this.PulsanteIndietroTesto = "INDIETRO";
                        break;
                }

                if (NascondiTitolo == true)
                {
                    this.ucTopModale.PazienteVisibile = false;
                }
                else
                {
                    this.ucTopModale.PazienteVisibile = true;
                }
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

        public DialogResult CaricaErrorMessageBox(string messaggio, string caption)
        {
            return CaricaErrorMessageBox(messaggio, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public DialogResult CaricaErrorMessageBox(string messaggio, string caption, MessageBoxButtons bottoni, MessageBoxIcon icona)
        {

            try
            {

                this.Text = caption;

                                this.ucEasyLabel.Text = "Matilde ha riscontrato un problema." + Environment.NewLine +
                                        "Vi preghiamo di spegnere il sistema e riavviarlo." + Environment.NewLine +
                                        "Se il problema persiste, contattare il supporto tecnico.";
                this.ucEasyLabelErrore.Text = messaggio;

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
                        this.PulsanteIndietroTesto = "INDIETRO";
                        break;

                    case MessageBoxButtons.YesNo:
                        this.PulsanteAvantiVisibile = true;
                        this.PulsanteAvantiTesto = "SI";
                        this.PulsanteIndietroVisibile = true;
                        this.PulsanteIndietroTesto = "NO";
                        break;

                    default:
                        this.PulsanteAvantiVisibile = true;
                        this.PulsanteAvantiTesto = "AVANTI";
                        this.PulsanteIndietroVisibile = true;
                        this.PulsanteIndietroTesto = "INDIETRO";
                        break;
                }
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
        public DialogResult CaricaErrorMessageBox(string messaggio, string errore, string caption, MessageBoxButtons bottoni, MessageBoxIcon icona)
        {

            try
            {

                this.Text = caption;

                this.ucEasyLabel.Text = errore;
                this.ucEasyLabelErrore.Text = messaggio;

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
                        this.PulsanteIndietroTesto = "INDIETRO";
                        break;

                    case MessageBoxButtons.YesNo:
                        this.PulsanteAvantiVisibile = true;
                        this.PulsanteAvantiTesto = "SI";
                        this.PulsanteIndietroVisibile = true;
                        this.PulsanteIndietroTesto = "NO";
                        break;

                    default:
                        this.PulsanteAvantiVisibile = true;
                        this.PulsanteAvantiTesto = "AVANTI";
                        this.PulsanteIndietroVisibile = true;
                        this.PulsanteIndietroTesto = "INDIETRO";
                        break;
                }
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

    }
}
