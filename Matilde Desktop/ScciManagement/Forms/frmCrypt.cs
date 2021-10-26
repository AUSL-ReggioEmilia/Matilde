using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmCrypt : Form
    {
        public frmCrypt()
        {
            InitializeComponent();
        }

        private void cmdCrypt_Click(object sender, EventArgs e)
        {

            try
            {
                Scci.Encryption ocrypt = new UnicodeSrl.Scci.Encryption(Scci.Statics.EncryptionStatics.GC_DECRYPTKEY, Scci.Statics.EncryptionStatics.GC_INITVECTOR);
                ScriviRisultato(ocrypt.EncryptString(this.txtSource.Text));
                ocrypt = null;
            }
            catch (Exception ex)
            {
                ScriviRisultato("ERRORE <cmdCrypt_Click>:" + Environment.NewLine + ex.Message);
            }
        }

        private void cmdDecrypt_Click(object sender, EventArgs e)
        {
            try
            {
                Scci.Encryption ocrypt = new UnicodeSrl.Scci.Encryption(Scci.Statics.EncryptionStatics.GC_DECRYPTKEY, Scci.Statics.EncryptionStatics.GC_INITVECTOR);
                ScriviRisultato(ocrypt.DecryptString(this.txtSource.Text));
                ocrypt = null;
            }
            catch (Exception ex)
            {
                ScriviRisultato("ERRORE <cmdDecrypt_Click>:" + Environment.NewLine + ex.Message);
            }
        }

        private void ScriviRisultato(string vsText)
        {
            try
            {
                if (this.txtResult.Text.Trim() != "") this.txtResult.Text += Environment.NewLine + Environment.NewLine;
                this.txtResult.Text += vsText;
                this.txtResult.SelectionStart = this.txtResult.Text.Length;
                this.txtResult.ScrollToCaret();
            }
            catch
            {
            }
        }

    }
}
