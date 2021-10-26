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
    public partial class frmPUDizionarioWiz : Form, Interfacce.IViewFormPUView
    {

        #region DECLARE

        private PUDataBindings _DataBinds = new PUDataBindings();
        private Enums.EnumModalityPopUp _Modality;
        private Enums.EnumDataNames _ViewDataNamePU;
        private bool _shown = false;

        public string codiceNewDizionario { get; set; }
        public string descrizioneNewDizionario { get; set; }
        public string vociQuickNewDizionario { get; set; }


        internal enum enumDizionarioWiz
        {
            copia = 0,
            quick = 1
        }

        #endregion

        public frmPUDizionarioWiz()
        {
            InitializeComponent();
        }

        #region Interface

        public PUDataBindings ViewDataBindings
        {
            get
            {
                return _DataBinds;
            }
            set
            {
                _DataBinds = value;
            }
        }

        public Enums.EnumDataNames ViewDataNamePU
        {
            get
            {
                return _ViewDataNamePU;
            }
            set
            {
                _ViewDataNamePU = value;
            }
        }

        public Image ViewImage
        {
            get
            {
                return this.picView.Image;
            }
            set
            {
                this.picView.Image = value;
            }
        }

        public Enums.EnumModalityPopUp ViewModality
        {
            get
            {
                return _Modality;
            }
            set
            {
                _Modality = value;
            }
        }

        public Icon ViewIcon
        {
            get
            {
                return this.Icon;
            }
            set
            {
                this.Icon = value;
            }
        }

        public string ViewText
        {
            get
            {
                return (string)this.Tag;
            }
            set
            {
                this.Tag = value;
                this.Text = string.Format("{0} - {1}", MyStatics.GetDataNameModalityFormPU(_Modality), value);
            }
        }

        public void ViewInit()
        {

            this.SuspendLayout();

            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            this.InitializeUltraToolbarsManager();

            loadData();

            this.ResumeLayout();

        }

        internal enumDizionarioWiz ViewDizionarioWiz { get; set; }

        #endregion

        #region UltraToolBar

        private void InitializeUltraToolbarsManager()
        {

            try
            {

                MyStatics.SetUltraToolbarsManager(this.ultraToolbarsManagerForm);

            }
            catch (Exception)
            {

            }

        }

        #endregion

        #region void & functions

        private void loadData()
        {

            try
            {

                this.uteCodice.Visible = true; this.uteDescrizione.Visible = true; this.lblCodice.Visible = true; this.lblDescrizione.Visible = true;
                switch (this.ViewDizionarioWiz)
                {
                    case enumDizionarioWiz.copia:

                        this.utcWiz.Visible = false;
                        this.Height = 240;

                        this.uteCodice.Text = this.codiceNewDizionario;
                        this.uteDescrizione.Text = this.descrizioneNewDizionario;



                        break;
                    case enumDizionarioWiz.quick:

                        this.utcWiz.Visible = true;
                        this.utcWiz.Tabs[0].Visible = true;
                        this.utcWiz.Tabs[1].Visible = false;
                        this.Height = 480;

                        this.uteCodice.Text = this.codiceNewDizionario;
                        this.uteDescrizione.Text = this.descrizioneNewDizionario;

                        break;



                    default:
                        break;
                }


            }
            catch (Exception)
            {
            }

        }

        private bool checkInput()
        {
            bool bReturn = true;
            string sSql = "";

            if (bReturn)
            {
                if (this.uteCodice.Text.Trim() == "")
                {
                    bReturn = false;
                    MessageBox.Show(@"Inserire Codice!", "Copia Dizionario", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.uteCodice.Focus();
                }
            }
            if (bReturn)
            {
                sSql = @"Select Codice From T_DCDecodifiche Where Codice = '" + DataBase.Ax2(this.uteCodice.Text.Trim()) + @"'";
                if (DataBase.GetDataSet(sSql).Tables[0].Rows.Count > 0)
                {
                    bReturn = false;
                    MessageBox.Show(@"Il codice """ + this.uteCodice.Text + @""" è già utilizzato!", "Copia Dizionario", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.uteCodice.Focus();
                }
            }

            if (bReturn)
            {
                if (this.uteDescrizione.Text.Trim() == "")
                {
                    bReturn = false;
                    MessageBox.Show(@"Inserire Descrizione!", "Copia Dizionario", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.uteDescrizione.Focus();
                }
            }

            switch (this.ViewDizionarioWiz)
            {
                case enumDizionarioWiz.quick:
                    if (bReturn)
                    {
                        if (this.uteVoci.Text.Trim().Replace(Environment.NewLine, "") == "")
                        {
                            bReturn = false;
                            MessageBox.Show(@"Inserire Voci!", "Dizionario Quick", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            this.uteVoci.Focus();
                        }
                    }

                    break;



                case enumDizionarioWiz.copia:
                default:
                    break;
            }

            return bReturn;
        }

        #endregion

        #region EVENTI

        private void ubAnnulla_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Hide();
        }

        private void ubConferma_Click(object sender, EventArgs e)
        {
            try
            {

                if (checkInput())
                {

                    this.codiceNewDizionario = this.uteCodice.Text;
                    this.descrizioneNewDizionario = this.uteDescrizione.Text;

                    switch (this.ViewDizionarioWiz)
                    {
                        case enumDizionarioWiz.quick:
                            this.vociQuickNewDizionario = this.uteVoci.Text;
                            break;

                        case enumDizionarioWiz.copia:
                        default:
                            break;
                    }

                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Hide();

                }
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, @"ubConferma_Click", this.Name);
            }
        }

        private void frmPUDizionarioWiz_Shown(object sender, EventArgs e)
        {
            if (!_shown)
            {
                _shown = true;

                switch (this.ViewDizionarioWiz)
                {

                    case enumDizionarioWiz.quick:
                    case enumDizionarioWiz.copia:
                    default:
                        this.uteCodice.Focus();
                        break;
                }
            }
        }

        #endregion

    }
}
