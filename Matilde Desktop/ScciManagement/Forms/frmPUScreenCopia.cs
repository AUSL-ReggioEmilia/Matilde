using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmPUScreenCopia : Form, Interfacce.IViewFormPUView
    {

        #region Declare

        private PUDataBindings _DataBinds = new PUDataBindings();
        private Enums.EnumModalityPopUp _Modality;
        private Enums.EnumDataNames _ViewDataNamePU;

        public string CodiceScreenOld { get; set; }
        public string CodiceScreenNew { get; set; }

        #endregion

        public frmPUScreenCopia()
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

            this.LoadDataBinds();

            this.ResumeLayout();

        }

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

        #region Subroutine

        private void LoadDataBinds()
        {

            try
            {

                CodiceScreenOld = _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Codice"].ToString();

                this.uteCodice.Text = _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Codice"].ToString();
                this.uteDescrizione.Text = @"Copia di " + _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Descrizione"].ToString();

                this.chkCopiaScreenTile.Checked = true;
                this.chkCopiaRuoli.Checked = false;

            }
            catch (Exception)
            {
            }

        }

        private bool CheckInput()
        {

            bool bReturn = true;
            string sSql = "";

            if (bReturn)
            {
                if (this.uteCodice.Text.Trim() == "")
                {
                    bReturn = false;
                    MessageBox.Show(@"Inserire Codice!", "Copia Screen", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.uteCodice.Focus();
                }
            }
            if (bReturn)
            {
                sSql = @"Select Codice From T_Screen Where Codice = '" + DataBase.Ax2(this.uteCodice.Text.Trim()) + @"'";
                if (DataBase.GetDataSet(sSql).Tables[0].Rows.Count > 0)
                {
                    bReturn = false;
                    MessageBox.Show(@"Il codice """ + this.uteCodice.Text + @""" è già utilizzato!", "Copia Screen", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.uteCodice.Focus();
                }
            }

            if (bReturn)
            {
                if (this.uteDescrizione.Text.Trim() == "")
                {
                    bReturn = false;
                    MessageBox.Show(@"Inserire Descrizione!", "Copia Screen", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.uteDescrizione.Focus();
                }
            }

            return bReturn;

        }

        private bool CopyScreen()
        {

            bool bReturn = true;

            try
            {

                string sSql = "";

                sSql += @"INSERT INTO T_Screen (Codice, Descrizione, Attributi, Righe, Colonne, CodTipoScreen, AltezzaRigaGrid, LarghezzaColonnaGrid, CaricaPerRiga, AdattaAltezzaRighe)
                          SELECT '" + DataBase.Ax2(this.uteCodice.Text.Trim()) + @"', '" + DataBase.Ax2(this.uteDescrizione.Text.Trim()) + @"', Attributi, Righe, Colonne, CodTipoScreen, AltezzaRigaGrid, LarghezzaColonnaGrid, CaricaPerRiga, AdattaAltezzaRighe
                          FROM T_Screen
                          Where Codice = '" + DataBase.Ax2(this.CodiceScreenOld) + @"' " + Environment.NewLine;

                if (this.chkCopiaScreenTile.Checked)
                {
                    sSql += Environment.NewLine;
                    sSql += @"INSERT INTO T_ScreenTile (CodScreen, Riga, Colonna, Altezza, Larghezza, InEvidenza, CodPlugin, Attributi, NomeTile, Fissa, NonCollassabile, Collassata)
                                    SELECT '" + DataBase.Ax2(this.uteCodice.Text.Trim()) + @"', Riga, Colonna, Altezza, Larghezza, InEvidenza, CodPlugin, Attributi, NomeTile, Fissa, NonCollassabile, Collassata
                                    FROM T_ScreenTile
                                    WHERE CodScreen = '" + DataBase.Ax2(this.CodiceScreenOld) + @"'" + Environment.NewLine;
                }

                if (this.chkCopiaRuoli.Checked)
                {
                    sSql += Environment.NewLine;
                    sSql += @"INSERT INTO T_AssRuoliAzioni (CodRuolo, CodEntita, CodVoce, CodAzione)
                                    SELECT CodRuolo, CodEntita, '" + DataBase.Ax2(this.uteCodice.Text.Trim()) + @"', CodAzione
                                    FROM T_AssRuoliAzioni
                                    WHERE CodEntita = 'SCR' And CodVoce = '" + DataBase.Ax2(this.CodiceScreenOld) + @"'" + Environment.NewLine;
                }

                if (sSql != "")
                {
                    bReturn = DataBase.ExecuteSql(sSql);
                }

            }
            catch (Exception ex)
            {
                bReturn = false;
                MyStatics.ExGest(ref ex, "CopyScreen", this.Name);
            }

            return bReturn;

        }

        #endregion

        #region Eventi

        private void ubAnnulla_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void ubConferma_Click(object sender, EventArgs e)
        {

            try
            {

                if (CheckInput())
                {

                    if (CopyScreen())
                    {
                        this.CodiceScreenNew = this.uteCodice.Text;
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                        this.Close();
                    }

                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, @"ubConferma_Click", this.Name);
            }

        }

        #endregion

    }
}
