using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using UnicodeSrl.DatiClinici.Common;
using UnicodeSrl.DatiClinici.DC;
using UnicodeSrl.Scci;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmPUSchedaEsporta : Form, Interfacce.IViewFormBase
    {
        public frmPUSchedaEsporta()
        {
            InitializeComponent();
        }

        #region Declare

        private const string C_COL_SEL = "SEL";

        #endregion

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

        public string ViewText
        {
            get
            {
                return (string)this.Tag;
            }
            set
            {
                this.Tag = value;
                this.Text = value;
            }
        }

        public void ViewInit()
        {
            this.InitializeSaveFileDialog();
            this.InitializeUltraGrid();

            this.InitializeValues();
        }

        #endregion

        #region Public Properties

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

        public string codiceScheda { get; set; }

        #endregion

        #region Initialize

        private void InitializeSaveFileDialog()
        {
            this.saveFileDialog.FileName = string.Empty;
            this.saveFileDialog.Filter = "File XML (*.xml)|*.xml|Tutti i file|*.*";
            this.saveFileDialog.FilterIndex = 0;
            this.saveFileDialog.CheckPathExists = true;
            this.saveFileDialog.OverwritePrompt = true;
            this.saveFileDialog.Title = "Seleziona il percorso ed il nome file di esportazione";
        }

        private void InitializeUltraGrid()
        {
            MyStatics.SetUltraGridLayout(ref this.UltraGrid, false, false);
            this.UltraGrid.DisplayLayout.GroupByBox.Hidden = true;
            this.UltraGrid.DisplayLayout.Override.RowSelectors = DefaultableBoolean.False;
            this.UltraGrid.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.True;
            this.UltraGrid.UpdateMode = UpdateMode.OnCellChangeOrLostFocus;
        }

        private void InitializeValues()
        {
            this.uteCodice.Text = this.codiceScheda;
            this.uteDescrizione.Text = DataBase.FindValue("Descrizione", "T_Schede", "Codice = '" + this.codiceScheda + "'", string.Empty);

            this.LoadUltraGrid();
        }

        private void LoadUltraGrid()
        {
            DataTable odtTemp = null;
            DataTable odtFinal = null;

            try
            {
                odtTemp = DataBase.GetDataTable("SELECT CodScheda, Versione, Descrizione FROM T_SchedeVersioni WHERE CodScheda = '" + this.codiceScheda + "'");

                if (odtTemp != null && odtTemp.Rows.Count > 0)
                {
                    odtFinal = new DataTable();

                    odtFinal.Columns.Add(C_COL_SEL, typeof(bool));

                    foreach (DataColumn dtc in odtTemp.Columns)
                    {
                        odtFinal.Columns.Add(dtc.ColumnName, dtc.DataType);
                    }

                    foreach (DataRow dtr in odtTemp.Rows)
                    {
                        DataRow dradd = odtFinal.NewRow();

                        dradd[C_COL_SEL] = false;

                        foreach (DataColumn dtc in odtTemp.Columns)
                        {
                            dradd[dtc.ColumnName] = dtr[dtc.ColumnName];
                        }

                        odtFinal.Rows.Add(dradd);
                    }
                }

                this.UltraGrid.DataSource = odtFinal;
                this.UltraGrid.Refresh();
                this.UltraGrid.Text = string.Format("{0} ({1:#,##0})", "Versioni: ", this.UltraGrid.Rows.Count);

                this.UltraGrid.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        #endregion

        #region Subroutine

        private bool CheckInput()
        {

            bool bRet = true;

            if (this.uteFilePath.Text == string.Empty)
            {
                MessageBox.Show(@"Inserire " + this.lblFilePath.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.uteFilePath.Focus();
                bRet = false;
            }

            if (bRet)
            {
                if (!Directory.Exists(Path.GetDirectoryName(this.uteFilePath.Text)))
                {
                    MessageBox.Show(@"Percorso di salvataggio inesistente!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.uteFilePath.Focus();
                    bRet = false;
                }
            }

            if (bRet)
            {
                List<UltraGridRow> rowlist = this.UltraGrid.Rows.ToList<UltraGridRow>().FindAll(r => Boolean.Parse(r.Cells[C_COL_SEL].Value.ToString()) == true);

                if (rowlist == null || rowlist.Count == 0)
                {
                    MessageBox.Show(@"Selezionare almeno una versione per l'esportazione!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.UltraGrid.Focus();
                    bRet = false;
                }
            }

            return bRet;
        }

        private void Export()
        {
            DataTable odttemp = null;
            string ssql = string.Empty;
            List<UltraGridRow> rowlist = null;
            XmlSerializer ser = null;
            TextWriter writer = null;

            string valuefield = string.Empty;

            try
            {

                SchedaExport schedaexport = new SchedaExport();

                ssql = "SELECT * FROM T_Schede WHERE Codice = '" + this.codiceScheda + "'";
                odttemp = DataBase.GetDataTable(ssql);

                schedaexport.Codice = this.uteCodice.Text;
                schedaexport.Descrizione = this.uteDescrizione.Text;

                foreach (DataRow dr in odttemp.Rows)
                {
                    foreach (DataColumn dc in odttemp.Columns)
                    {
                        ExportField f = new ExportField();

                        f.Name = dc.ColumnName;
                        f.DataType = dc.DataType.ToString();

                        switch (dc.ColumnName)
                        {
                            case "Codice":
                                valuefield = System.Security.SecurityElement.Escape(this.uteCodice.Text);
                                break;
                            case "Descrizione":
                                valuefield = System.Security.SecurityElement.Escape(this.uteDescrizione.Text);
                                break;
                            default:
                                valuefield = System.Security.SecurityElement.Escape(dr[dc.ColumnName].ToString());
                                break;
                        }

                        f.Value = valuefield;

                        schedaexport.Scheda.Fields.Add(f);
                    }
                }

                rowlist = this.UltraGrid.Rows.ToList<UltraGridRow>().FindAll(r => Boolean.Parse(r.Cells[C_COL_SEL].Value.ToString()) == true);

                foreach (UltraGridRow row in rowlist)
                {
                    ssql = "select * from T_SchedeVersioni where CodScheda = '" + this.codiceScheda + "' AND Versione = " + row.Cells["Versione"].Value.ToString();
                    odttemp = DataBase.GetDataTable(ssql);

                    foreach (DataRow dr in odttemp.Rows)
                    {
                        ExportVersion v = new ExportVersion();

                        v.CodVersione = odttemp.Rows[0]["Versione"].ToString();
                        v.Descrizione = odttemp.Rows[0]["Descrizione"].ToString();

                        foreach (DataColumn dc in odttemp.Columns)
                        {
                            ExportField f = new ExportField();

                            switch (dc.ColumnName)
                            {
                                case "CodScheda":
                                    valuefield = System.Security.SecurityElement.Escape(this.uteCodice.Text);
                                    break;
                                case "Struttura":
                                case "Struttura_Old":
                                    DcScheda dcsch = null;
                                    try
                                    {
                                        dcsch = (DcScheda)Serializer.FromXmlString(dr[dc.ColumnName].ToString(), typeof(DcScheda));
                                        dcsch.ID = this.uteCodice.Text;
                                        dcsch.Descrizione = this.uteDescrizione.Text;

                                        foreach (DcSezione sez in dcsch.Sezioni.Values)
                                        {
                                            if (sez.Padre.Key == this.codiceScheda) sez.Padre.Key = this.uteCodice.Text;
                                        }

                                        valuefield = Serializer.ToXmlString(dcsch);
                                        valuefield = valuefield.Substring(valuefield.IndexOf("<DcScheda"), valuefield.Length - valuefield.IndexOf("<DcScheda"));
                                        valuefield = System.Security.SecurityElement.Escape(valuefield);
                                    }
                                    catch
                                    {
                                        valuefield = string.Empty;
                                    }
                                    finally
                                    {
                                        if (dcsch != null) dcsch = null;
                                    }
                                    break;

                                case "Layout":
                                    DcSchedaLayouts dclay = null;
                                    try
                                    {
                                        dclay = (DcSchedaLayouts)Serializer.FromXmlString(dr[dc.ColumnName].ToString(), typeof(DcSchedaLayouts));
                                        dclay.ID = this.uteCodice.Text;

                                        valuefield = Serializer.ToXmlString(dclay);
                                        valuefield = valuefield.Substring(valuefield.IndexOf("<DcSchedaLayouts"), valuefield.Length - valuefield.IndexOf("<DcSchedaLayouts"));
                                        valuefield = System.Security.SecurityElement.Escape(valuefield);
                                    }
                                    catch
                                    {
                                        valuefield = string.Empty;
                                    }
                                    finally
                                    {
                                        if (dclay != null) dclay = null;
                                    }
                                    break;

                                default:
                                    valuefield = System.Security.SecurityElement.Escape(dr[dc.ColumnName].ToString());
                                    break;
                            }

                            f.Name = dc.ColumnName;
                            f.DataType = dc.DataType.ToString();
                            f.Value = valuefield;

                            v.Fields.Add(f);
                        }

                        schedaexport.Versioni.Add(v);
                    }
                }

                ser = new XmlSerializer(typeof(SchedaExport));
                writer = new StreamWriter(this.uteFilePath.Text);
                ser.Serialize(writer, schedaexport);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            finally
            {
                if (rowlist != null)
                {
                    rowlist = null;
                }

                if (odttemp != null)
                {
                    odttemp.Dispose();
                    odttemp = null;
                }

                if (writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                    writer = null;
                }

            }
        }

        #endregion

        #region Events

        private void UltraGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].ColHeadersVisible = false;
            foreach (var col in e.Layout.Bands[0].Columns)
            {
                switch (col.Key)
                {
                    case "CodScheda":
                        col.Hidden = true;
                        break;
                    case C_COL_SEL:
                        col.Header.CheckBoxSynchronization = HeaderCheckBoxSynchronization.Band;
                        col.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
                        col.CellClickAction = CellClickAction.Edit;
                        col.CellActivation = Activation.AllowEdit;
                        break;
                }
            }
        }

        private void ubFilePath_Click(object sender, EventArgs e)
        {
            if (this.saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.uteFilePath.Text = this.saveFileDialog.FileName;
            }
        }

        private void ubAnnulla_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void ubConferma_Click(object sender, EventArgs e)
        {
            if (this.CheckInput())
            {
                this.Cursor = Cursors.WaitCursor;
                this.Export();
                this.Cursor = Cursors.Default;
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        #endregion

    }
}
