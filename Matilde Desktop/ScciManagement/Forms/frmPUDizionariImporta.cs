using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmPUDizionariImporta : Form, Interfacce.IViewFormBase
    {

        #region Declare

        private const string C_COL_SEL = "SEL";

        private bool _ModalityCSV = false;

        #endregion

        public frmPUDizionariImporta()
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
            this.InitializeOpenFileDialog();
            this.InitializeUltraGrid();
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

        public bool ModalityCSV
        {
            get { return _ModalityCSV; }
            set
            {
                _ModalityCSV = value;
                this.chkCSVIntestazioniColonne.Visible = value;
            }
        }

        #endregion

        #region Initialize

        private void InitializeOpenFileDialog()
        {
            this.openFileDialog.FileName = string.Empty;

            if (this.ModalityCSV)
                this.openFileDialog.Filter = "File CSV (*.csv)|*.csv|Tutti i file|*.*";
            else
                this.openFileDialog.Filter = "File XML (*.xml)|*.xml|Tutti i file|*.*";

            this.openFileDialog.FilterIndex = 0;
            this.openFileDialog.CheckPathExists = true;
            this.openFileDialog.Title = "Seleziona il file di importazione";
        }

        private void InitializeUltraGrid()
        {
            MyStatics.SetUltraGridLayout(ref this.UltraGrid, true, false);
            this.UltraGrid.DisplayLayout.GroupByBox.Hidden = true;
            this.UltraGrid.DisplayLayout.Override.RowSelectors = DefaultableBoolean.False;
            this.UltraGrid.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.True;
            this.UltraGrid.UpdateMode = UpdateMode.OnCellChangeOrLostFocus;
            this.UltraGrid.Text = string.Empty;
        }

        #endregion

        #region Subroutine

        private bool CheckInput()
        {
            bool bret = true;

            return bret;
        }

        private void LoadGrid()
        {
            if (this.ModalityCSV)
                LoadGridCSV();
            else
                LoadGridXML();

        }

        private void LoadGridXML()
        {
            this.Cursor = Cursors.WaitCursor;

            XmlSerializer ser = null;
            XmlReader reader = null;

            DizionariExport dizexp = null;
            DataTable odt = null;

            try
            {
                ser = new XmlSerializer(typeof(DizionariExport));
                reader = XmlReader.Create(this.uteFilePath.Text);

                try
                {
                    dizexp = (DizionariExport)ser.Deserialize(reader);
                }
                catch
                {
                    dizexp = null;
                }

                if (dizexp != null)
                {
                    odt = new DataTable();

                    odt.Columns.Add(C_COL_SEL, typeof(bool));
                    odt.Columns.Add("Codice", typeof(string));
                    odt.Columns.Add("Descrizione", typeof(string));

                    foreach (DizionarioExport diz in dizexp.Dizionari)
                    {
                        DataRow dradd = odt.NewRow();

                        dradd[C_COL_SEL] = false;
                        dradd["Codice"] = diz.Codice;
                        dradd["Descrizione"] = diz.Descrizione;

                        odt.Rows.Add(dradd);
                    }
                }

                this.UltraGrid.DataSource = odt;
                this.UltraGrid.Refresh();
                this.UltraGrid.Text = string.Format("{0} ({1:#,##0})", "Dizionari: ", this.UltraGrid.Rows.Count);

                this.UltraGrid.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }

                if (ser != null) ser = null;
            }

            this.Cursor = Cursors.Default;
        }

        private void LoadGridCSV()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                bool bSvotaGrid = true;

                if (this.uteFilePath.Text.Trim() != "")
                {

                    if (!System.IO.File.Exists(this.uteFilePath.Text))
                    {
                        MessageBox.Show(@"Il file " + this.uteFilePath.Text + @" non esiste o non è raggiungibile!", "Leggi CSV", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                    }
                    else
                    {
                        int itotDiz = 0;
                        DataTable odt = getCSVDataTable(out itotDiz);

                        if (odt != null)
                        {
                            bSvotaGrid = false;
                            this.UltraGrid.DataSource = odt;
                            this.UltraGrid.Refresh();
                            this.UltraGrid.Text = string.Format("Dizionari: {0:#,##0} (Voci: {1:#,##0})", itotDiz, this.UltraGrid.Rows.Count);

                            this.UltraGrid.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);
                        }
                    }

                }

                if (bSvotaGrid)
                {

                    this.UltraGrid.DataSource = null;
                    this.UltraGrid.Refresh();
                    this.UltraGrid.Text = "";
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            this.Cursor = Cursors.Default;
        }

        private DataTable getCSVDataTable(out int totDizionari)
        {
            try
            {
                totDizionari = 0;
                char charSep = ';';
                DataTable odt = null;
                string sCSVFilePathName = this.uteFilePath.Text;

                string[] arrLines = System.IO.File.ReadAllLines(sCSVFilePathName);

                if (arrLines.Length > 0)
                {
                    string[] arrFields = arrLines[0].Split(new char[] { charSep });

                    if (arrFields.Length < 4)
                    {
                        MessageBox.Show(@"File CSV non corretto!", "Carica CSV", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {

                        odt = new DataTable();
                        int itotCols = arrFields.GetLength(0);


                        for (int i = 0; i < itotCols; i++)
                        {
                            if (this.chkCSVIntestazioniColonne.Checked)
                                odt.Columns.Add(arrFields[i].ToLower(), typeof(string));
                            else
                            {
                                string colname = "";
                                switch (i)
                                {
                                    case 0:
                                        colname = @"CodDizionario";
                                        break;

                                    case 1:
                                        colname = @"DescrDizionario";
                                        break;

                                    case 2:
                                        colname = @"CodVoce";
                                        break;

                                    case 3:
                                        colname = @"DescrVoce";
                                        break;

                                    default:
                                        colname = "col" + (i + 1).ToString();
                                        break;
                                }
                                odt.Columns.Add(colname, typeof(string));
                            }

                        }

                        int iFirstRow = 0;
                        if (this.chkCSVIntestazioniColonne.Checked) iFirstRow = 1;

                        string sLastCodDizionario = "";
                        for (int i = iFirstRow; i < arrLines.GetLength(0); i++)
                        {
                            arrFields = arrLines[i].Split(new char[] { charSep });

                            try
                            {
                                if (sLastCodDizionario == "" || sLastCodDizionario.Trim().ToUpper() != arrFields[0].Trim().ToUpper())
                                {
                                    totDizionari += 1;
                                    sLastCodDizionario = arrFields[0];
                                }
                            }
                            catch
                            {
                            }

                            DataRow newRow = odt.NewRow();
                            for (int f = 0; f < itotCols; f++)
                                newRow[f] = arrFields[f];
                            odt.Rows.Add(newRow);
                        }
                    }
                }


                return odt;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private bool Import()
        {
            try
            {
                if (this.ModalityCSV)
                    return ImportCSV();
                else
                    return ImportXML();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Errore importazione", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
        }
        private bool ImportXML()
        {
            bool bRet = false;
            this.Cursor = Cursors.WaitCursor;

            XmlSerializer ser = null;
            XmlReader reader = null;

            DizionariExport dizexp = null;

            List<string> sqlList = null;

            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlTransaction tran = null;

            try
            {
                ser = new XmlSerializer(typeof(DizionariExport));
                reader = XmlReader.Create(this.uteFilePath.Text);

                try
                {
                    dizexp = (DizionariExport)ser.Deserialize(reader);
                }
                catch
                {
                    dizexp = null;
                }

                if (dizexp != null)
                {
                    #region generazione sql per l'importazione dizionari

                    sqlList = new List<string>();

                    List<UltraGridRow> selrows = this.UltraGrid.Rows.ToList<UltraGridRow>().FindAll(r => bool.Parse(r.Cells[C_COL_SEL].Value.ToString()) == true);

                    foreach (UltraGridRow row in selrows)
                    {
                        DizionarioExport diz = dizexp.Dizionari.Find(d => d.Codice == row.Cells["Codice"].Value.ToString());
                        if (diz != null) sqlList = sqlList.Union<string>(this.GetSQLStatement(diz)).ToList<string>();
                    }

                    #endregion

                    #region lancio della insert transazionata

                    if (sqlList.Count > 0)
                    {
                        conn = new SqlConnection(MyStatics.Configurazione.ConnectionString);
                        conn.Open();

                        tran = conn.BeginTransaction("DizionariImportTransaction");

                        cmd = new SqlCommand();

                        cmd.Connection = conn;
                        cmd.Transaction = tran;
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.Text;

                        try
                        {
                            foreach (string ssql in sqlList)
                            {
                                cmd.CommandText = ssql;
                                cmd.ExecuteNonQuery();
                            }

                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                tran.Rollback();
                            }
                            catch (Exception inEx)
                            {
                                ex = new Exception(ex.Message + Environment.NewLine + inEx.Message, inEx);
                            }
                            throw ex;
                        }
                    }

                    #endregion
                }

                MessageBox.Show("Importazione terminata con successo", "Importazione Dizionari", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                MessageBox.Show(ex.Message, "Errore importazione", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                #region chiusura oggetti

                if (reader != null && reader.ReadState != ReadState.Closed)
                {
                    reader.Close();
                    reader = null;
                }

                if (ser != null) ser = null;

                if (dizexp != null) dizexp = null;

                if (tran != null)
                {
                    tran.Dispose();
                    tran = null;
                }

                if (cmd != null)
                {
                    cmd.Dispose();
                    cmd = null;
                }

                if (conn != null && conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Dispose();
                conn = null;

                #endregion
            }

            this.Cursor = Cursors.Default;

            return bRet;
        }
        private bool ImportCSV()
        {
            bool bRet = false;
            this.Cursor = Cursors.WaitCursor;

            try
            {
                DataTable odt = null;

                if (this.uteFilePath.Text.Trim() != "")
                {

                    if (!System.IO.File.Exists(this.uteFilePath.Text))
                    {
                        MessageBox.Show(@"Il file " + this.uteFilePath.Text + @" non esiste o non è raggiungibile!", "Leggi CSV", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                    }
                    else
                    {
                        int itotDiz = 0;
                        odt = getCSVDataTable(out itotDiz);

                    }

                }

                if (odt != null)
                {

                    if (odt.Rows.Count > 0)
                    {
                        string col1 = odt.Columns[0].ColumnName;
                        string col3 = odt.Columns[2].ColumnName;
                        odt.DefaultView.Sort = col1 + @" ASC, " + col3 + @" ASC";

                        string sSqlTrans = "";
                        bool bDizionarioEsisteGia = false;
                        string sCodDizionario = "";
                        string sDescrizioneDizionario = "";
                        bool bInserisciVoce = true;
                        string sCodVoce = "";
                        string sDescrizioneVoce = "";
                        int iOrdine = 1;

                        #region GENERAZIONE SQL

                        for (int i = 0; i < odt.DefaultView.Count; i++)
                        {
                            DataRowView drv = odt.DefaultView[i];
                            if (!drv.Row.IsNull(0) && drv[0].ToString().Trim() != ""
                                && !drv.Row.IsNull(2) && drv[2].ToString().Trim() != "")
                            {


                                if (sCodDizionario.Trim().ToUpper() != drv[0].ToString().Trim().ToUpper())
                                {
                                    iOrdine = 1;
                                    bInserisciVoce = true;
                                    sCodVoce = "";
                                    sDescrizioneVoce = "";
                                    bDizionarioEsisteGia = false;
                                    sCodDizionario = drv[0].ToString().Trim();
                                    sDescrizioneDizionario = drv[1].ToString().Trim();

                                    string find = DataBase.FindValue("IsNull(Count(*),0)", "T_DCDecodifiche", @"Codice = '" + DataBase.Ax2(sCodDizionario) + @"'", "0");
                                    if (find != null && find.Trim() != "" && find.Trim() != "0")
                                    {
                                        bDizionarioEsisteGia = true;
                                        if (this.rbSovrascrivi.Checked)
                                        {
                                            sSqlTrans += @" Delete From T_DcDecodificheValori Where CodDec = '" + DataBase.Ax2(sCodDizionario) + @"' " + Environment.NewLine + Environment.NewLine;
                                            sSqlTrans += @" Delete From T_DCDecodifiche Where Codice = '" + DataBase.Ax2(sCodDizionario) + @"' " + Environment.NewLine + Environment.NewLine;
                                        }
                                    }

                                    if (!bDizionarioEsisteGia || this.rbSovrascrivi.Checked)
                                    {
                                        sSqlTrans += @" Insert Into T_DCDecodifiche (Codice, Descrizione) " + Environment.NewLine;
                                        sSqlTrans += @" Values ('" + DataBase.Ax2(sCodDizionario) + @"', '" + DataBase.Ax2(sDescrizioneDizionario) + @"') " + Environment.NewLine + Environment.NewLine;
                                    }

                                }


                                bInserisciVoce = true;
                                sCodVoce = drv[2].ToString().Trim();
                                sDescrizioneVoce = drv[3].ToString().Trim();

                                if (bDizionarioEsisteGia)
                                {
                                    if (this.rbSovrascrivi.Checked)
                                    {
                                    }
                                    else
                                    {
                                        string find = DataBase.FindValue("IsNull(Count(*),0)", "T_DcDecodificheValori", @"CodDec = '" + DataBase.Ax2(sCodDizionario) + @"' And Codice = '" + DataBase.Ax2(sCodVoce) + @"'", "0");
                                        if (find != null && find.Trim() != "" && find.Trim() != "0")
                                        {
                                            if (this.rbInserisci.Checked)
                                            {
                                                bInserisciVoce = false;
                                            }
                                            else
                                            {
                                                sSqlTrans += @" Delete From T_DcDecodificheValori Where CodDec = '" + DataBase.Ax2(sCodDizionario) + @"' And Codice = '" + DataBase.Ax2(sCodVoce) + @"' " + Environment.NewLine + Environment.NewLine;
                                            }
                                        }
                                    }
                                }

                                if (bInserisciVoce)
                                {
                                    sSqlTrans += @" Insert Into T_DcDecodificheValori (CodDec, Codice, Descrizione, Ordine, DtValI) " + Environment.NewLine;
                                    sSqlTrans += @" Values ('" + DataBase.Ax2(sCodDizionario) + @"', '" + DataBase.Ax2(sCodVoce) + @"', '" + DataBase.Ax2(sDescrizioneVoce) + @"', " + iOrdine.ToString() + @", " + DataBase.SQLDate(DateTime.Now.Date) + @") " + Environment.NewLine + Environment.NewLine;
                                }

                                iOrdine += 1;

                            }

                        }
                        #endregion


                        if (sSqlTrans != string.Empty)
                        {
                            bRet = DataBase.ExecuteSql(sSqlTrans);
                        }
                        else
                            bRet = true;

                    }

                    MessageBox.Show("Importazione terminata con successo", "Importazione Dizionari", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                MessageBox.Show(ex.Message, "Errore importazione", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }


            this.Cursor = Cursors.Default;

            return bRet;
        }

        private List<string> GetSQLStatement(DizionarioExport diz)
        {
            List<string> lsret = new List<string>();
            string sunescval = string.Empty;

            if (this.rbSovrascrivi.Checked)
            {
                lsret.Add(this.GetDeleteDettaglio(diz.Codice));
                lsret.Add(this.GetDeleteTestata(diz.Codice, diz.Descrizione));

                lsret.Add(this.GetInsertTestata(diz.Codice, diz.Descrizione));
                foreach (VoceDizionarioExport v in diz.Voci)
                {
                    lsret.Add(this.GetInsertDettaglio(v));
                }
            }
            else if (this.rbInserisci.Checked)
            {
                lsret.Add(this.GetMergeInsertTestata(diz.Codice, diz.Descrizione));
                foreach (VoceDizionarioExport v in diz.Voci)
                {
                    lsret.Add(this.GetMergeInsertDettaglio(v));
                }
            }
            else if (this.rbSostituisci.Checked)
            {
                lsret.Add(this.GetMergeInsertTestata(diz.Codice, diz.Descrizione));
                foreach (VoceDizionarioExport v in diz.Voci)
                {
                    lsret.Add(this.GetMergeInsertDettaglio(v));
                }

                lsret.Add(this.GetMergeUpdateTestata(diz.Codice, diz.Descrizione));
                foreach (VoceDizionarioExport v in diz.Voci)
                {
                    lsret.Add(this.GetMergeUpdateDettaglio(v));
                }
            }

            return lsret;
        }

        private string GetInsertTestata(string codice, string descrizione)
        {
            string ssqlret = string.Empty;

            try
            {
                ssqlret = "INSERT INTO T_DCDecodifiche ([Codice], [Descrizione]) VALUES ('" + codice + "', '" + UnicodeSrl.Scci.Statics.Database.testoSQL(descrizione) + "')" + Environment.NewLine;
            }
            catch
            {
                ssqlret = string.Empty;
            }

            return ssqlret;
        }

        private string GetDeleteTestata(string codice, string descrizione)
        {
            string ssqlret = string.Empty;

            try
            {
                ssqlret = "DELETE FROM T_DCDecodifiche WHERE [Codice] = '" + codice + "'" + Environment.NewLine;
            }
            catch
            {
                ssqlret = string.Empty;
            }

            return ssqlret;
        }

        private string GetMergeUpdateTestata(string codice, string descrizione)
        {
            string ssqlret = string.Empty;

            try
            {
                ssqlret = @";with data as (select '" + UnicodeSrl.Scci.Statics.Database.testoSQL(codice) + @"' as Codice, '" + UnicodeSrl.Scci.Statics.Database.testoSQL(descrizione) + @"' as Descrizione)
                           merge T_DCDecodifiche t
                           using data s
		                   ON s.Codice = t.Codice
                           when matched then update set Codice = s.Codice, Descrizione = s.Descrizione;" + Environment.NewLine;
            }
            catch
            {
                ssqlret = string.Empty;
            }

            return ssqlret;
        }

        private string GetMergeInsertTestata(string codice, string descrizione)
        {
            string ssqlret = string.Empty;

            try
            {
                ssqlret = @";with data as (select '" + UnicodeSrl.Scci.Statics.Database.testoSQL(codice) +
                           @"' as Codice, '" + UnicodeSrl.Scci.Statics.Database.testoSQL(descrizione) + @"' as Descrizione)
                           merge T_DCDecodifiche t
                           using data s
		                   ON s.Codice = t.Codice
                           when not matched then insert (Codice, Descrizione) values (s.Codice, s.Descrizione);" + Environment.NewLine;
            }
            catch
            {
                ssqlret = string.Empty;
            }

            return ssqlret;
        }

        private string GetInsertDettaglio(VoceDizionarioExport v)
        {
            string ssqlret = string.Empty;

            try
            {

                ssqlret += "INSERT INTO T_DCDecodificheValori (";

                string[] columnnames = (from fld in v.Fields select "[" + fld.Name + "]").ToArray();

                ssqlret += String.Join<string>(", ", columnnames) + ")" + Environment.NewLine + "VALUES" + Environment.NewLine;

                ssqlret += "(" + Environment.NewLine;

                foreach (ExportField fld in v.Fields)
                {
                    ssqlret += GetSQLValue(fld.Value, fld.DataType, true);
                }

                ssqlret = ssqlret.Substring(0, ssqlret.Length - 2);

                ssqlret += ")" + Environment.NewLine;
            }
            catch
            {
                ssqlret = string.Empty;
            }

            return ssqlret;
        }

        private string GetDeleteDettaglio(string coddec)
        {
            string ssqlret = string.Empty;

            try
            {
                ssqlret = "DELETE FROM T_DCDecodificheValori WHERE [CodDec] = '" + coddec + "'" + Environment.NewLine;
            }
            catch
            {
                ssqlret = string.Empty;
            }

            return ssqlret;
        }

        private string GetMergeUpdateDettaglio(VoceDizionarioExport voce)
        {
            string ssqlret = string.Empty;

            try
            {
                ssqlret += @";with data as" + Environment.NewLine;
                ssqlret += @"(" + Environment.NewLine;
                ssqlret += this.GetSelectForMerge(voce.Fields) + Environment.NewLine;
                ssqlret += @")" + Environment.NewLine;
                ssqlret += @"merge T_DCDecodificheValori t
                           using data s
		                   ON s.CodDec = t.CodDec AND s.Codice = t.Codice
                           when matched then " + Environment.NewLine;
                ssqlret += this.GetUpdateForMerge(voce.Fields) + ";" + Environment.NewLine;
            }
            catch
            {
                ssqlret = string.Empty;
            }

            return ssqlret;
        }

        private string GetMergeInsertDettaglio(VoceDizionarioExport voce)
        {
            string ssqlret = string.Empty;
            string coddec = string.Empty;
            string codice = string.Empty;

            try
            {
                ssqlret += @";with data as" + Environment.NewLine;
                ssqlret += @"(" + Environment.NewLine;
                ssqlret += this.GetSelectForMerge(voce.Fields) + Environment.NewLine;
                ssqlret += @")" + Environment.NewLine;
                ssqlret += @"merge T_DCDecodificheValori t
                           using data s
		                   ON s.CodDec = t.CodDec AND s.Codice = t.Codice
                           when not matched then " + Environment.NewLine;
                ssqlret += this.GetInsertForMerge(voce.Fields) + ";" + Environment.NewLine;
            }
            catch
            {
                ssqlret = string.Empty;
            }

            return ssqlret;
        }

        private string GetSelectForMerge(List<ExportField> fields)
        {
            string sret = string.Empty;

            sret += "Select " + Environment.NewLine;

            foreach (ExportField fld in fields)
            {
                sret += GetSQLValue(fld.Value, fld.DataType, false) + " AS " + fld.Name;
                if (fld != fields[fields.Count - 1]) sret += ", ";
            }

            return sret;
        }

        private string GetInsertForMerge(List<ExportField> fields)
        {
            string sret = string.Empty;

            sret += "Insert " + Environment.NewLine;
            sret += "(" + Environment.NewLine;

            foreach (ExportField fld in fields)
            {
                sret += fld.Name;
                if (fld != fields[fields.Count - 1]) sret += ", ";
            }

            sret += ")" + Environment.NewLine;
            sret += "VALUES" + Environment.NewLine;
            sret += "(" + Environment.NewLine;

            foreach (ExportField fld in fields)
            {
                sret += GetSQLValue(fld.Value, fld.DataType, false);
                if (fld != fields[fields.Count - 1]) sret += ", ";
            }

            sret += ")" + Environment.NewLine;

            return sret;
        }

        private string GetUpdateForMerge(List<ExportField> fields)
        {
            string sret = string.Empty;

            sret += "update " + Environment.NewLine;
            sret += "set" + Environment.NewLine;

            foreach (ExportField fld in fields)
            {
                sret += fld.Name + " = " + GetSQLValue(fld.Value, fld.DataType, false);
                if (fld != fields[fields.Count - 1]) sret += ", ";
            }

            sret += Environment.NewLine;

            return sret;
        }

        private string GetSQLValue(string value, string flddatatype, bool closingcomma)
        {
            string sret = string.Empty;
            string sunescval = string.Empty;

            try
            {
                if (System.String.IsNullOrEmpty(value))
                {
                    sret = "NULL";
                }
                else
                {
                    System.Type fldtype = Type.GetType(flddatatype);
                    sunescval = this.UnescapeXML(value);

                    if (fldtype == typeof(System.String))
                    {
                        sret = "'" + UnicodeSrl.Scci.Statics.Database.testoSQL(sunescval) + "'";
                    }
                    else if (fldtype == typeof(System.Int32))
                    {
                        if (Microsoft.VisualBasic.Information.IsNumeric(sunescval))
                            sret = sunescval + "";
                        else
                            sret = "NULL";
                    }
                    else if (fldtype == typeof(System.Boolean))
                    {
                        bool btemp = false;
                        bool.TryParse(sunescval, out btemp);

                        if (btemp)
                            sret = "1";
                        else
                            sret = "0";
                    }
                    else if (fldtype == typeof(System.DateTime))
                    {
                        DateTime dttemp = DateTime.MinValue;
                        DateTime.TryParse(sunescval, out dttemp);

                        if (dttemp != DateTime.MinValue)
                            sret = UnicodeSrl.Scci.Statics.Database.dataOraSQL(dttemp);
                        else
                            sret = "NULL";
                    }
                    else if (fldtype == typeof(System.Byte[]))
                    {
                        if (value != string.Empty)
                            sret = value.ToString();
                        else
                            sret = string.Empty;
                    }
                    else
                    {
                        sret = "NULL";
                    }
                }

                if (closingcomma) sret += ", ";
            }
            catch
            {
                sret = string.Empty;
            }

            return sret;
        }

        private string UnescapeXML(string s)
        {

            string returnString = s;

            if (!string.IsNullOrEmpty(s) || s.Contains('&'))
            {
                returnString = returnString.Replace("&apos;", "'");
                returnString = returnString.Replace("&quot;", "\"");
                returnString = returnString.Replace("&gt;", ">");
                returnString = returnString.Replace("&lt;", "<");
                returnString = returnString.Replace("&amp;", "&");
            }

            return returnString;

        }

        #endregion

        #region Events
        private void chkCSVIntestazioniColonne_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ModalityCSV) LoadGridCSV();
        }

        private void UltraGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            foreach (var col in e.Layout.Bands[0].Columns)
            {
                switch (col.Key)
                {
                    case C_COL_SEL:
                        col.Header.CheckBoxSynchronization = HeaderCheckBoxSynchronization.Band;
                        col.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
                        col.CellClickAction = CellClickAction.Edit;
                        col.CellActivation = Activation.AllowEdit;
                        col.Header.Caption = string.Empty;
                        break;

                    default:
                        col.CellClickAction = CellClickAction.RowSelect;
                        col.CellActivation = Activation.ActivateOnly;
                        break;
                }
            }
        }

        private void ubFilePath_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.uteFilePath.Text = this.openFileDialog.FileName;
                this.Refresh();
                this.LoadGrid();
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
                if (this.Import())
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }
            }
        }

        #endregion

    }
}
