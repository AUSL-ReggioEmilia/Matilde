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
    public partial class frmInputBox : frmBaseModale
    {
        DialogResult _messageBoxRetun = DialogResult.Cancel;
        private bool _multiLinea = false;

        public string TestoInserito
        {
            get { return this.ucEasyTextBox.Text; }
        }

        public frmInputBox()
        {
            InitializeComponent();
        }

        public DialogResult CaricaInputBox(string messaggio, string caption)
        {
            return CaricaInputBox(messaggio, caption, "");
        }
        public DialogResult CaricaInputBox(string messaggio, string caption, string testoIniziale)
        {
            return CaricaInputBox(messaggio, caption, testoIniziale, false);
        }
        public DialogResult CaricaInputBox(string messaggio, string caption, string testoIniziale, bool multilinea)
        {
            return CaricaInputBox(messaggio, caption, testoIniziale, multilinea, 0);
        }
        public DialogResult CaricaInputBox(string messaggio, string caption, string testoIniziale, bool multilinea, int maxLength)
        {
            return CaricaInputBox(messaggio, caption, testoIniziale, multilinea, maxLength, MessageBoxIcon.Question);
        }
        public DialogResult CaricaInputBox(string messaggio, string caption, string testoIniziale, bool multilinea, int maxLength, MessageBoxIcon icona)
        {
            return CaricaInputBox(messaggio, caption, testoIniziale, multilinea, maxLength, icona, false);
        }
        public DialogResult CaricaInputBox(string messaggio, string caption, string testoIniziale, bool multilinea, int maxLength, MessageBoxIcon icona, bool NascondiTitolo)
        {
            try
            {
                this.Text = caption;

                this.ucEasyLabel.Text = messaggio;
                
                if (maxLength > 0) this.ucEasyTextBox.MaxLength = maxLength;

                if (multilinea)
                    this.ucEasyTextBox.Text = testoIniziale;
                else
                    this.ucEasyTextBox.Text = testoIniziale.Replace(Environment.NewLine, " ");

                _multiLinea = multilinea;
                if (multilinea)
                {
                    this.ucEasyTextBox.Multiline = true;
                    this.ucEasyTextBox.Scrollbars = ScrollBars.Both;
                    this.ucEasyTextBox.AcceptsReturn = true;
                    this.ucEasyTextBox.WordWrap = false;
                                                                                                }
                else
                {
                    this.ucEasyTextBox.Multiline = true;
                    this.ucEasyTextBox.Scrollbars = ScrollBars.Vertical;
                    this.ucEasyTextBox.AcceptsReturn = false;
                    this.ucEasyTextBox.WordWrap = true;
                                                                                                }


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

                this.PulsanteAvantiVisibile = true;
                this.PulsanteAvantiTesto = "AVANTI";
                this.PulsanteIndietroVisibile = true;
                this.PulsanteIndietroTesto = "INDIETRO";

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

        private void frmInputBox_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            _messageBoxRetun = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void frmInputBox_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
                        _messageBoxRetun = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void ucEasyTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (!_multiLinea)
                {
                                        if (e.KeyCode == Keys.Enter) e.SuppressKeyPress = true;
                }
            }
            catch
            {
            }
        }

    }
}
