using System;
using System.Drawing;
using System.Windows.Forms;
using static UnicodeSrl.ScciManagement.Interfacce;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmPUProtocolliPrescrizioniInsert : Form, IViewFormBase
    {
        public frmPUProtocolliPrescrizioniInsert()
        {
            InitializeComponent();
        }

        #region Property

        public Icon ViewIcon
        {
            get
            {
                return Icon;
            }
            set
            {
                Icon = value;
            }
        }

        public string ViewText
        {
            get
            {
                return (string)Tag;
            }
            set
            {
                Tag = value;
                Text = value;
            }
        }

        public void ViewInit()
        {

        }

        public string IDPrescrizione
        {
            get { return uteIDrigine.Text; }
            set { uteIDrigine.Text = value; }
        }

        public string Descrizione
        {
            get { return uteDescrizione.Text; }
            set { uteDescrizione.Text = value; }
        }

        public Image ViewImage
        {
            get
            {
                return PicImage.Image;
            }
            set
            {
                PicImage.Image = value;
            }
        }

        #endregion

        #region Methods

        private bool CheckInput()
        {

            bool bRet = true;

            if (bRet && uteIDrigine.Text == string.Empty)
            {
                MessageBox.Show(@"Inserire " + lblIDOrigine.Text + @"!", Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                uteIDrigine.Focus();
                bRet = false;
            }

            if (bRet && uteDescrizione.Text == string.Empty)
            {
                MessageBox.Show(@"Inserire " + lblDescrizione.Text + @"!", Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                uteDescrizione.Focus();
                bRet = false;
            }

            if (bRet && !CheckIDPrescrizione(uteIDrigine.Text))
            {
                MessageBox.Show(lblIDOrigine.Text + @" inserito inesistente !", Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                uteIDrigine.Focus();
                bRet = false;
            }

            return bRet;

        }

        private bool CheckIDPrescrizione(string idPrescrizione)
        {
            string sSQL = string.Empty;

            try
            {
                sSQL = "SELECT ID FROM T_MovPrescrizioni WHERE ID = '" + idPrescrizione + "'";
                return DataBase.GetDataTable(sSQL).Rows.Count == 1;
            }
            catch (Exception ex)
            {
                Framework.Diagnostics.Statics.AddDebugInfo(ex);
                return false;
            }
        }

        #endregion

        #region Events

        private void ubAnnulla_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ubConferma_Click(object sender, EventArgs e)
        {
            if (CheckInput())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        #endregion

    }
}
