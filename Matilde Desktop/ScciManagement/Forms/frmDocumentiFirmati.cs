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
    public partial class frmDocumentiFirmati : Form, Interfacce.IViewFormBase
    {
        public frmDocumentiFirmati()
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
            this.InitializeUltraCombo();
            this.udteDataInserimentoI.Value = DateTime.Now.Date;
            this.udteDataInserimentoF.Value = DateTime.Now.Date;
            this.InitializeUltraGrid();

            this.uteID.Focus();

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

                MyStatics.SetUltraComboEditorLayout(ref this.uceCodEntita);
                this.uceCodEntita.ValueMember = "Codice";
                this.uceCodEntita.DisplayMember = "Descrizione";
                sSql = "Select Codice, Descrizione From T_Entita" + Environment.NewLine +
        "UNION" + Environment.NewLine +
        "Select '' As Codice, '' As Descrizione" + Environment.NewLine +
        "Order By Descrizione";
                oDs = DataBase.GetDataSet(sSql);
                this.uceCodEntita.DataMember = oDs.Tables[0].TableName;
                this.uceCodEntita.DataSource = oDs;
                this.uceCodEntita.DataBind();

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

                sSqlSelect = "SELECT ID, IDNum, CodEntita, IDEntita, CodTipoDocumentoFirmato, Numero," + Environment.NewLine +
"CodUtenteInserimento, DataInserimento, DataInserimentoUTC, FlagEsportato, DataEsportazione," + Environment.NewLine +
"CASE WHEN PDFFirmato IS NOT NULL THEN CONVERT(VARCHAR(50),'Doppio click per esportare') ELSE '' END AS EsportaPDFFirmato , NomeFileEsportatoXML, " + Environment.NewLine +
" CASE WHEN PDFNonFirmato IS NOT NULL THEN CONVERT(VARCHAR(50),'Doppio click per esportare') ELSE '' END AS EsportaPDFNonFirmato " + Environment.NewLine +
"FROM T_MovDocumentiFirmati" + Environment.NewLine;
                if (this.uteID.Text != string.Empty)
                {
                    if (sSqlWhere != string.Empty) { sSqlWhere += "And "; }
                    sSqlWhere += "ID = '" + DataBase.Ax2(this.uteID.Text) + "'" + Environment.NewLine;
                    bCheckRicerca = true;
                }
                if (this.udteDataInserimentoI.Value != null)
                {
                    if (sSqlWhere != string.Empty) { sSqlWhere += "And "; }
                    sSqlWhere += "Convert(Date,DataInserimento) >= " + DataBase.SQLDate((DateTime)this.udteDataInserimentoI.Value) + Environment.NewLine;
                    bCheckRicerca = true;
                }
                if (this.udteDataInserimentoF.Value != null)
                {
                    if (sSqlWhere != string.Empty) { sSqlWhere += "And "; }
                    sSqlWhere += "Convert(Date,DataInserimento) <= " + DataBase.SQLDate((DateTime)this.udteDataInserimentoF.Value) + Environment.NewLine;
                    bCheckRicerca = true;
                }
                if (this.uceCodEntita.Text != string.Empty)
                {
                    if (sSqlWhere != string.Empty) { sSqlWhere += "And "; }
                    sSqlWhere += "CodEntita = '" + DataBase.Ax2(this.uceCodEntita.Value.ToString()) + "'" + Environment.NewLine;
                    bCheckRicerca = true;
                }
                if (this.uteIDEntita.Text != string.Empty)
                {
                    if (sSqlWhere != string.Empty) { sSqlWhere += "And "; }
                    sSqlWhere += "IDEntita = '" + DataBase.Ax2(this.uteIDEntita.Text) + "'" + Environment.NewLine;
                    bCheckRicerca = true;
                }

                if (bCheckRicerca == true)
                {

                    oDs = DataBase.GetDataSet(sSqlSelect + "Where " + sSqlWhere);

                    this.UltraGrid.DataSource = oDs;
                    this.UltraGrid.Refresh();
                    this.UltraGrid.Text = string.Format("{0} ({1:#,##0})", "Ricerca Documenti", this.UltraGrid.Rows.Count);

                    this.UltraGrid.DisplayLayout.PerformAutoResizeColumns(false, Infragistics.Win.UltraWinGrid.PerformAutoSizeType.AllRowsInBand);

                }
                else
                {
                    MessageBox.Show("Inserire almeno un parametro di ricerca!", "Ricerca Documenti", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this.uteID.Focus();
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

        private void frmDocumentiFirmati_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.UltraGrid != null) this.UltraGrid.Dispose();
        }

        #endregion

        #region Events

        private void ubRicerca_Click(object sender, EventArgs e)
        {
            this.LoadUltraGrid();
        }

        #endregion

        private void UltraGrid_DoubleClickCell(object sender, Infragistics.Win.UltraWinGrid.DoubleClickCellEventArgs e)
        {

            try
            {

                string sNomeCampoPDF = "";

                if (e.Cell.IsDataCell)
                {

                    this.SaveFileDialog.FilterIndex = 1;
                    this.SaveFileDialog.RestoreDirectory = true;
                    this.SaveFileDialog.Title = "Salva Documento Firmato";
                    this.SaveFileDialog.FileName = (e.Cell.Row.Cells["ID"].Text);

                    switch (e.Cell.Column.Key)
                    {

                        case "EsportaPDFFirmato":
                            this.SaveFileDialog.Filter = @"Documenti Firmati (*.pdf.p7m)|*.pdf.p7m";
                            sNomeCampoPDF = "PDFFirmato";
                            break;

                        case "EsportaPDFNonFirmato":
                            this.SaveFileDialog.Filter = @"Documenti Firmati (*.pdf)|*.pdf";
                            sNomeCampoPDF = "PDFNonFirmato";
                            break;
                    }

                    if (sNomeCampoPDF == string.Empty)
                    {
                        MessageBox.Show("Selezione la cella EsportaPDFFirmato o EsportaPDFNonFirmato per procedere con l'esportazione", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        if (e.Cell.Row.Cells[e.Cell.Column.Key].Text != string.Empty)
                        {
                            if (this.SaveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                object oDocumento = null;
                                oDocumento = DataBase.FindValue(sNomeCampoPDF, "T_MovDocumentiFirmati", "ID = '" + e.Cell.Row.Cells["ID"].Text + "'");

                                if (oDocumento is System.DBNull == false)
                                {
                                    byte[] documentofirmato = (byte[])(oDocumento);
                                    if (documentofirmato != null)
                                    {
                                        UnicodeSrl.Framework.Utility.FileSystem.ByteArrayToFile(this.SaveFileDialog.FileName, ref documentofirmato);
                                        MessageBox.Show("Documento salvato in '" + this.SaveFileDialog.FileName + "'", "File esportato", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Documento non trovato!", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Documento non presente, impossibile esportarlo ", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

    }
}