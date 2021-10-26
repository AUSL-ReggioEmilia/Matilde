using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using UnicodeSrl.Scci;
using UnicodeSrl.ScciCore;

using Infragistics.Win.UltraWinGrid;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmNormalizzazione : Form, Interfacce.IViewFormBase
    {
        public frmNormalizzazione()
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
            this.InitializeUltraComboEditor();
            this.InitializeUltraGrid();
            this.InitializeUltraProgressBar();
            this.ubNormalizza.Enabled = false;

            this.udteDataInizio.Value = DateTime.Now;
            this.udteDataFine.Value = DateTime.Now;

            this.udteDataInizio.Focus();

            this.ResumeLayout();

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


            string sSql = string.Empty;
            string sSqlWhere = string.Empty;

            this.Cursor = Cursors.WaitCursor;

            try
            {

                bool bCheckRicerca = false;

                sSql = "Select * From T_MovSchede MS" + Environment.NewLine;
                if (this.udteDataInizio.Value != null)
                {
                    if (sSqlWhere != string.Empty) { sSqlWhere += "And "; }
                    sSqlWhere += "MS.DataUltimaModifica >= " + DataBase.SQLDate((DateTime)this.udteDataInizio.Value) + Environment.NewLine;
                    bCheckRicerca = true;
                }
                if (this.udteDataFine.Value != null)
                {
                    if (sSqlWhere != string.Empty) { sSqlWhere += "And "; }
                    sSqlWhere += "MS.DataUltimaModifica <= " + DataBase.SQLDate((DateTime)this.udteDataFine.Value) + Environment.NewLine;
                    bCheckRicerca = true;
                }
                if (this.uceEntita.Value != null && this.uceEntita.Value.ToString() != string.Empty)
                {
                    if (sSqlWhere != string.Empty) { sSqlWhere += "And "; }
                    sSqlWhere += "MS.CodEntita = '" + DataBase.Ax2(this.uceEntita.Value.ToString()) + "'" + Environment.NewLine;
                    bCheckRicerca = true;
                }
                if (this.uceScheda.Value != null && this.uceScheda.Value.ToString() != string.Empty)
                {
                    if (sSqlWhere != string.Empty) { sSqlWhere += "And "; }
                    sSqlWhere += "MS.CodScheda = '" + DataBase.Ax2(this.uceScheda.Value.ToString()) + "'" + Environment.NewLine;
                    bCheckRicerca = true;
                }

                if (this.uteIDScheda.Value != null && this.uteIDScheda.Value.ToString() != string.Empty)
                {
                    if (sSqlWhere != string.Empty) { sSqlWhere += "And "; }
                    sSqlWhere += "MS.ID = '" + DataBase.Ax2(this.uteIDScheda.Value.ToString()) + "'" + Environment.NewLine;
                    bCheckRicerca = true;
                }

                if (bCheckRicerca == true)
                {

                    sSql += "Where " + sSqlWhere + Environment.NewLine +
"And MS.Storicizzata = 0" + Environment.NewLine +
"Order By MS.IDNum desc" + Environment.NewLine;

                    this.UltraGrid.DataSource = DataBase.GetDataSet(sSql); ;
                    this.UltraGrid.Refresh();
                    this.UltraGrid.Text = string.Format("{0} ({1:#,##0})", "Normalizzazione Movimenti Schede", this.UltraGrid.Rows.Count);

                    this.UltraGrid.DisplayLayout.PerformAutoResizeColumns(false, Infragistics.Win.UltraWinGrid.PerformAutoSizeType.AllRowsInBand);

                    this.UltraGrid.Selected.Rows.AddRange((UltraGridRow[])this.UltraGrid.Rows.All);

                    this.ubNormalizza.Enabled = (this.UltraGrid.Rows.Count == 0 ? false : true);
                    this.InitializeUltraProgressBar();

                }
                else
                {
                    MessageBox.Show("Inserire almeno un parametro di ricerca!", "Normalizzazione Movimenti Schede", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this.udteDataInizio.Focus();
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            this.Cursor = Cursors.Default;

        }

        #endregion

        #region UltraComboEditor

        private void InitializeUltraComboEditor()
        {

            try
            {

                MyStatics.SetUltraComboEditorLayout(ref this.uceEntita);
                this.uceEntita.ValueMember = "Codice";
                this.uceEntita.DisplayMember = "Descrizione";
                string sSql = "Select Codice, Descrizione From T_Entita Where UsaSchede = 1" + Environment.NewLine +
                "UNION" + Environment.NewLine +
                "Select '' As Codice, '' As Descrizione" + Environment.NewLine +
                "Order By Descrizione";
                DataSet oDs = DataBase.GetDataSet(sSql);
                this.uceEntita.DataMember = oDs.Tables[0].TableName;
                this.uceEntita.DataSource = oDs;
                this.uceEntita.DataBind();

                MyStatics.SetUltraComboEditorLayout(ref this.uceScheda);
                this.uceScheda.ValueMember = "Codice";
                this.uceScheda.DisplayMember = "Descrizione";
                sSql = "Select Codice, Descrizione + ' (' + Codice + ')' As Descrizione From T_Schede" + Environment.NewLine +
                "UNION" + Environment.NewLine +
                "Select '' As Codice, '' As Descrizione" + Environment.NewLine +
                "Order By Descrizione";
                oDs = DataBase.GetDataSet(sSql);
                this.uceScheda.DataMember = oDs.Tables[0].TableName;
                this.uceScheda.DataSource = oDs;
                this.uceScheda.DataBind();

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "InitializeUltraComboEditor", this.Name);
            }

        }

        #endregion

        #region UltraProgressBar

        private void InitializeUltraProgressBar()
        {
            this.UltraProgressBar.Style = Infragistics.Win.UltraWinProgressBar.ProgressBarStyle.Office2007Continuous;
            this.UltraProgressBar.Visible = false;
        }
        private void InitializeUltraProgressBar(int MinValue, int MaxValue)
        {
            this.UltraProgressBar.Style = Infragistics.Win.UltraWinProgressBar.ProgressBarStyle.Office2007Continuous;
            this.UltraProgressBar.Minimum = MinValue;
            this.UltraProgressBar.Maximum = MaxValue;
            this.UltraProgressBar.Value = MinValue;
            this.UltraProgressBar.Visible = true;
        }

        private void UpdateProgressBar()
        {
            if (this.UltraProgressBar.Visible)
            {
                try
                {
                    int pbarnextval = this.UltraProgressBar.Value + 1;

                    if (pbarnextval <= this.UltraProgressBar.Maximum)
                        this.UltraProgressBar.Value = pbarnextval;

                }
                catch
                {
                }

                Application.DoEvents();
            }
        }

        #endregion

        #region Events

        private void frmNormalizzazione_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.UltraGrid != null) this.UltraGrid.Dispose();
        }

        private void ubRicerca_Click(object sender, EventArgs e)
        {
            this.LoadUltraGrid();
        }

        private void UltraGrid_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {

                if (e.Layout.Bands[0].Columns.Exists("Dati") == true) { e.Layout.Bands[0].Columns["Dati"].Hidden = true; }
                if (e.Layout.Bands[0].Columns.Exists("AnteprimaRTF") == true) { e.Layout.Bands[0].Columns["AnteprimaRTF"].Hidden = true; }
                if (e.Layout.Bands[0].Columns.Exists("DatiObbligatoriMancantiRTF") == true) { e.Layout.Bands[0].Columns["DatiObbligatoriMancantiRTF"].Hidden = true; }
                if (e.Layout.Bands[0].Columns.Exists("DatiRilievoRTF") == true) { e.Layout.Bands[0].Columns["DatiRilievoRTF"].Hidden = true; }

            }
            catch (Exception)
            {

            }

        }

        private void ubNormalizza_Click(object sender, EventArgs e)
        {

            string sConnectionString = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.AnalisiConnectionString);
            string sSql = string.Empty;

            try
            {

                if (this.UltraGrid.Selected.Rows.Count > 0)
                {

                    if (MessageBox.Show("Sei sicuro di voler normalizzare le schede selezionate (" + this.UltraGrid.Selected.Rows.Count.ToString() + ") ?",
                                        "Normalizzazione Movimenti Schede",
                                        MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
                    {
                        this.Cursor = Cursors.WaitCursor;

                        this.InitializeUltraProgressBar(0, this.UltraGrid.Selected.Rows.Count);
                        foreach (UltraGridRow oRow in this.UltraGrid.Selected.Rows)
                        {

                            this.UpdateProgressBar();

                            if (oRow.IsDataRow == true)
                            {

                                MovScheda oMovScheda = new MovScheda(oRow.Cells["ID"].Text, CoreStatics.CoreApplication.Ambiente);
                                string sDati = oMovScheda.Normalizza();
                                StringReader oStringReader = new StringReader(sDati);
                                DataSet oDsDati = new DataSet();
                                oDsDati.ReadXml(oStringReader);

                                sSql = "DELETE FROM T_MovSchedeDati" + Environment.NewLine +
        "WHERE IDScheda='" + oRow.Cells["ID"].Value + "'";

                                DataBase.ExecuteSql(sSql, sConnectionString);

                                if (oMovScheda.CodStatoScheda != UnicodeSrl.Scci.Enums.EnumStatoScheda.CA.ToString())
                                {

                                    sSql = "SELECT * FROM T_MovSchedeDati Where 0=1";
                                    DataSet oDsAnalisi = DataBase.GetDataSet(sSql, sConnectionString);

                                    if (oDsDati.Tables.Count > 0)
                                    {
                                        foreach (DataRow oDr in oDsDati.Tables[0].Rows)
                                        {
                                            oDsAnalisi.Tables[0].ImportRow(oDr);
                                        }
                                    }

                                    foreach (DataRow oDr in oDsAnalisi.Tables[0].Rows)
                                    {
                                        oDr["IDScheda"] = oRow.Cells["ID"].Text;
                                        oDr["CodEntita"] = oRow.Cells["CodEntita"].Text;
                                        oDr["IDEntita"] = oRow.Cells["IDEntita"].Text;
                                        oDr["Numero"] = oRow.Cells["Numero"].Value;
                                    }

                                    DataBase.SaveDataSet(oDsAnalisi, sSql, sConnectionString);

                                }

                            }

                        }

                        this.InitializeUltraProgressBar();

                        this.Cursor = Cursors.Default;

                        MessageBox.Show("Fine Normalizzazione!", "Normalizzazione Movimenti Schede", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                else
                {
                    MessageBox.Show("Selezionare almeno una riga!", "Normalizzazione Movimenti Schede", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Cursor = Cursors.Default;
            }

        }

        #endregion

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
