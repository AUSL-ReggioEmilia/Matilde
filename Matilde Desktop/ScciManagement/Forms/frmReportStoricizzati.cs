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
    public partial class frmReportStoricizzati : Form, Interfacce.IViewFormBase
    {

        public frmReportStoricizzati()
        {
            InitializeComponent();
        }

        #region Interface

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

        public Image ViewImage
        {
            get
            {
                return this.PicImage.Image;
            }
            set
            {
                this.PicImage.Image = value;
            }
        }

        public string ViewText
        {
            get
            {
                return this.Text;
            }
            set
            {
                this.Text = value;
            }
        }

        public void ViewInit()
        {

            this.SuspendLayout();

            MyStatics.SetUltraToolbarsManager(this.UltraToolbarsManager);
            this.udteDataNascita.Value = null;
            this.udteDataStampaInizio.Value = DateTime.Now.Date;
            this.udteDataStampaFine.Value = DateTime.Now.Date;
            this.InitializeUltraCombo();
            this.InitializeUltraGrid();

            this.uteCognome.Focus();

            this.ResumeLayout();

        }

        #endregion

        #region UltraComboEditor

        private void InitializeUltraCombo()
        {

            string sSql = string.Empty;
            DataSet oDs = null;

            try
            {

                MyStatics.SetUltraComboEditorLayout(ref this.uceFormatoReport);
                this.uceFormatoReport.ValueMember = "Codice";
                this.uceFormatoReport.DisplayMember = "Descrizione";
                sSql = "Select '" + UnicodeSrl.Scci.Enums.EnumFormatoReport.CAB.ToString() + "' As Codice, 'Cablato' As Descrizione" + Environment.NewLine +
        "UNION" + Environment.NewLine +
        "Select '" + UnicodeSrl.Scci.Enums.EnumFormatoReport.WORD.ToString() + "' As Codice, 'Formato Word 2007/2010' As Descrizione" + Environment.NewLine +
        "Order By Descrizione";
                oDs = DataBase.GetDataSet(sSql);
                this.uceFormatoReport.DataMember = oDs.Tables[0].TableName;
                this.uceFormatoReport.DataSource = oDs;
                this.uceFormatoReport.DataBind();
                this.uceFormatoReport.SelectedIndex = 0;

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "InitializeUltraCombo", this.Name);
            }

        }

        #endregion

        #region UltraGrid

        private void InitializeUltraGrid()
        {

            MyStatics.SetUltraGridLayout(ref this.UltraGrid, true, false);

            this.UltraGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;

        }

        private void LoadUltraGrid()
        {


            string sSqlWhere = string.Empty;
            string sSqlSelect = string.Empty;

            DataSet oDs = null;

            this.Cursor = Cursors.WaitCursor;

            try
            {

                bool bCheckRicerca = false;

                sSqlSelect = "Select MR.ID, P.Cognome, P.Nome, MR.DataEvento, MC.NumeroCartella, MR.CodLogin, MR.NomePC, R.CodFormatoReport, MR.Documento" + Environment.NewLine +
"From T_MovReport MR" + Environment.NewLine +
"Inner Join T_Report R ON MR.CodReport = R.Codice" + Environment.NewLine +
"Inner Join T_Pazienti P ON MR.IDPaziente = P.ID" + Environment.NewLine +
"Left Join T_MovTrasferimenti MT ON MR.IDTrasferimento = MT.ID" + Environment.NewLine +
"Left Join T_MovCartelle MC ON MT.IDCartella = MC.ID" + Environment.NewLine +
"Where R.CodFormatoReport = '" + this.uceFormatoReport.Value.ToString() + "'" + Environment.NewLine;

                if (this.uteCognome.Text != string.Empty)
                {
                    sSqlWhere += "And P.Cognome Like '" + DataBase.Ax2(this.uteCognome.Text) + "%'" + Environment.NewLine;
                    bCheckRicerca = true;
                }
                if (this.uteNome.Text != string.Empty)
                {
                    sSqlWhere += "And P.Nome Like '" + DataBase.Ax2(this.uteNome.Text) + "%'" + Environment.NewLine;
                    bCheckRicerca = true;
                }
                if (this.uteNumeroCartella.Text != string.Empty)
                {
                    sSqlWhere += "And MC.NumeroCartella = '" + DataBase.Ax2(this.uteNumeroCartella.Text) + "'" + Environment.NewLine;
                    bCheckRicerca = true;
                }
                if (this.udteDataNascita.Value != null)
                {
                    sSqlWhere += "And Convert(Date,P.DataNascita) = " + DataBase.SQLDate((DateTime)this.udteDataNascita.Value) + Environment.NewLine;
                    bCheckRicerca = true;
                }
                if (this.udteDataStampaInizio.Value != null)
                {
                    sSqlWhere += "And Convert(Date,MR.DataEvento) >= " + DataBase.SQLDate((DateTime)this.udteDataStampaInizio.Value) + Environment.NewLine;
                    bCheckRicerca = true;
                }
                if (this.udteDataStampaFine.Value != null)
                {
                    sSqlWhere += "And Convert(Date,MR.DataEvento) <= " + DataBase.SQLDate((DateTime)this.udteDataStampaFine.Value) + Environment.NewLine;
                    bCheckRicerca = true;
                }
                if (this.uteUtente.Text != string.Empty)
                {
                    sSqlWhere += "And MR.CodLogin = '" + DataBase.Ax2(this.uteUtente.Text) + "'" + Environment.NewLine;
                    bCheckRicerca = true;
                }
                if (this.uteNomePC.Text != string.Empty)
                {
                    sSqlWhere += "And MR.NomePC = '" + DataBase.Ax2(this.uteNomePC.Text) + "'" + Environment.NewLine;
                    bCheckRicerca = true;
                }

                if (bCheckRicerca == true)
                {

                    oDs = DataBase.GetDataSet(sSqlSelect + sSqlWhere);

                    this.UltraGrid.DataSource = oDs;
                    this.UltraGrid.Refresh();
                    this.UltraGrid.Text = string.Format("{0} ({1:#,##0})", "Ricerca Documenti", this.UltraGrid.Rows.Count);

                    this.UltraGrid.DisplayLayout.PerformAutoResizeColumns(false, Infragistics.Win.UltraWinGrid.PerformAutoSizeType.AllRowsInBand);

                }
                else
                {
                    MessageBox.Show("Inserire almeno 2 parametri di ricerca!", "Ricerca Documenti", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this.uteCognome.Focus();
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            this.Cursor = Cursors.Default;

        }

        #endregion

        #region Events Form

        private void frmReportStoricizzati_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.UltraGrid != null) this.UltraGrid.Dispose();
        }

        #endregion

        #region Events

        private void ubRicerca_Click(object sender, EventArgs e)
        {
            this.LoadUltraGrid();
        }

        private void UltraGrid_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {

            try
            {

                if (e.Row.IsDataRow == true)
                {

                    byte[] documento = (byte[])e.Row.Cells["Documento"].Value;
                    if (documento != null)
                    {

                        string pathnomefile = System.IO.Path.Combine(System.IO.Path.GetTempPath() + "RPT" + DateTime.Now.ToString("yyyyMMddHHmmssfff"));
                        if (e.Row.Cells["CodFormatoReport"].Text == "CAB")
                        {
                            pathnomefile += @".pdf";
                            UnicodeSrl.Framework.Utility.FileSystem.ByteArrayToFile(pathnomefile, ref documento);
                            MyStatics.ApriPDF(pathnomefile);
                        }
                        else if (e.Row.Cells["CodFormatoReport"].Text == "WORD")
                        {
                            pathnomefile += @".docx";
                            UnicodeSrl.Framework.Utility.FileSystem.ByteArrayToFile(pathnomefile, ref documento);
                            MyStatics.ApriDOCX(pathnomefile);
                        }

                    }
                    else
                    {
                        MessageBox.Show("Report NON trovato!");
                    }

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void UltraGrid_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {

                if (e.Layout.Bands[0].Columns.Exists("Documento") == true)
                {
                    e.Layout.Bands[0].Columns["Documento"].Hidden = true;
                }
                if (e.Layout.Bands[0].Columns.Exists("CodFormatoReport") == true)
                {
                    e.Layout.Bands[0].Columns["CodFormatoReport"].Hidden = true;
                }



            }
            catch (Exception)
            {

            }

        }

        #endregion

    }
}
