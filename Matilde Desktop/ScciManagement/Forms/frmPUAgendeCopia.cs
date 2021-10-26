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
    public partial class frmPUAgendeCopia : Form, Interfacce.IViewFormPUView
    {

        #region Declare

        private PUDataBindings _DataBinds = new PUDataBindings();
        private Enums.EnumModalityPopUp _Modality;
        private Enums.EnumDataNames _ViewDataNamePU;

        public string codiceNewAgenda { get; set; }

        #endregion

        public frmPUAgendeCopia()
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

                int iMaxL = 0;
                string sNewCodice = _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Codice"].ToString();
                if (_DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"].MaxLength > 0) iMaxL = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"].MaxLength;
                if (sNewCodice.Trim().Length >= iMaxL)
                    sNewCodice = "";
                else
                {
                    const string suff = @"ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    bool bOK = false;
                    for (int i = 0; i < suff.Length; i++)
                    {
                        string sTmp = sNewCodice + suff.Substring(i, 1);
                        if (DataBase.GetDataSet(@"Select Codice From T_Agende Where Codice = '" + DataBase.Ax2(sTmp) + "'").Tables[0].Rows.Count <= 0)
                        {
                            sNewCodice = sTmp;
                            bOK = true;
                            i = suff.Length + 1;
                        }
                    }
                    if (!bOK) sNewCodice = "";
                }

                this.uteCodice.Text = sNewCodice;
                if (!_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("Descrizione"))
                    this.uteDescrizione.Text = @"Copia di " + _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Descrizione"].ToString();

                this.chkCopiaUA.Checked = true;
                this.chkCopiaRuoli.Checked = true;
                this.chkCopiaTA.Checked = true;
                this.chkCopiaPeriodi.Checked = false;

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
                    MessageBox.Show(@"Inserire Codice!", "Copia Agenda", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.uteCodice.Focus();
                }
            }

            if (bReturn)
            {
                sSql = @"Select Codice From T_Agende Where Codice = '" + DataBase.Ax2(this.uteCodice.Text.Trim()) + @"'";
                if (DataBase.GetDataSet(sSql).Tables[0].Rows.Count > 0)
                {
                    bReturn = false;
                    MessageBox.Show(@"Il codice """ + this.uteCodice.Text + @""" è già utilizzato!", "Copia Agenda", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.uteCodice.Focus();
                }
            }

            if (bReturn)
            {
                if (this.uteDescrizione.Text.Trim() == "")
                {
                    bReturn = false;
                    MessageBox.Show(@"Inserire Descrizione!", "Copia Agenda", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.uteDescrizione.Focus();
                }
            }

            if (bReturn)
            {
                string sCheck = "";
                if (!this.chkCopiaUA.Checked)
                {
                    if (sCheck != "") sCheck += Environment.NewLine;
                    sCheck += @"- " + this.chkCopiaUA.Text;
                }
                if (!this.chkCopiaRuoli.Checked)
                {
                    if (sCheck != "") sCheck += Environment.NewLine;
                    sCheck += @"- " + this.chkCopiaRuoli.Text;
                }
                if (!this.chkCopiaTA.Checked)
                {
                    if (sCheck != "") sCheck += Environment.NewLine;
                    sCheck += @"- " + this.chkCopiaTA.Text;
                }
                if (!this.chkCopiaPeriodi.Checked)
                {
                    if (sCheck != "") sCheck += Environment.NewLine;
                    sCheck += @"- " + this.chkCopiaPeriodi.Text;
                }

                if (sCheck != "")
                {
                    if (MessageBox.Show(@"Non hai selezionato:" + Environment.NewLine + sCheck + Environment.NewLine + @"Vuoi proseguire ugualmente con la copia?", "Copia Agenda", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No)
                    {
                        bReturn = false;
                    }
                }
            }

            return bReturn;

        }

        private bool copyAgenda()
        {
            bool bReturn = true;

            try
            {

                string sSql = "";

                sSql += @"INSERT INTO T_Agende (Codice, Descrizione, CodTipoAgenda, Colore, ElencoCampi, IntervalloSlot, OrariLavoro, CodEntita, Ordine, UsaColoreTipoAppuntamento, 
                                                DescrizioneAlternativa, MassimoAnticipoPrenotazione, MassimoRitardoPrenotazione, Lista, ParametriLista, Risorse, EscludiFestivita)
                          SELECT '" + DataBase.Ax2(this.uteCodice.Text.Trim()) + @"', '" + DataBase.Ax2(this.uteDescrizione.Text.Trim()) + @"', CodTipoAgenda, Colore, ElencoCampi, 
                                        IntervalloSlot, OrariLavoro, CodEntita, Ordine, UsaColoreTipoAppuntamento,
                                        DescrizioneAlternativa, MassimoAnticipoPrenotazione, MassimoRitardoPrenotazione, Lista, ParametriLista, Risorse, EscludiFestivita
                          FROM T_Agende
                          Where Codice = '" + DataBase.Ax2(_DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Codice"].ToString()) + @"' " + Environment.NewLine;

                if (this.chkCopiaUA.Checked)
                {
                    sSql += Environment.NewLine;
                    sSql += @"INSERT INTO T_AssUAEntita (CodUA, CodEntita, CodVoce)
                                SELECT CodUA, CodEntita, '" + DataBase.Ax2(this.uteCodice.Text.Trim()) + @"'
                                FROM T_AssUAEntita
                                WHERE CodEntita = 'AGE' And CodVoce = '" + DataBase.Ax2(_DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Codice"].ToString()) + @"'" + Environment.NewLine;
                }

                if (this.chkCopiaRuoli.Checked)
                {
                    sSql += Environment.NewLine;
                    sSql += @"INSERT INTO T_AssRuoliAzioni (CodRuolo, CodEntita, CodVoce, CodAzione)
                                    SELECT CodRuolo, CodEntita, '" + DataBase.Ax2(this.uteCodice.Text.Trim()) + @"', CodAzione
                                    FROM T_AssRuoliAzioni
                                    WHERE CodVoce = '" + DataBase.Ax2(_DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Codice"].ToString()) + @"'" + Environment.NewLine;
                }

                if (this.chkCopiaTA.Checked)
                {
                    sSql += Environment.NewLine;
                    sSql += @"INSERT INTO T_AssAgendeTipoAppuntamenti (CodAgenda, CodTipoApp, EscludiSovrapposizioni)
                                SELECT '" + DataBase.Ax2(this.uteCodice.Text.Trim()) + @"', CodTipoApp, EscludiSovrapposizioni
                                FROM T_AssAgendeTipoAppuntamenti
                                WHERE CodAgenda = '" + DataBase.Ax2(_DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Codice"].ToString()) + @"'" + Environment.NewLine;
                }

                if (this.chkCopiaPeriodi.Checked)
                {
                    sSql += Environment.NewLine;
                    sSql += @"INSERT INTO T_AgendePeriodi (CodAgenda, DataInizio, DataFine, OrariLavoro)
                                SELECT '" + DataBase.Ax2(this.uteCodice.Text.Trim()) + @"', DataInizio, DataFine, OrariLavoro
                                FROM T_AgendePeriodi
                                WHERE CodAgenda = '" + DataBase.Ax2(_DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Codice"].ToString()) + @"'" + Environment.NewLine;
                }

                if (sSql != "")
                {
                    bReturn = DataBase.ExecuteSql(sSql);
                }

            }
            catch (Exception ex)
            {
                bReturn = false;
                MyStatics.ExGest(ref ex, "copyAgenda", this.Name);
            }

            return bReturn;

        }

        #endregion

        #region Eventi

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
                    if (copyAgenda())
                    {
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                        codiceNewAgenda = this.uteCodice.Text;
                        this.Hide();
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
