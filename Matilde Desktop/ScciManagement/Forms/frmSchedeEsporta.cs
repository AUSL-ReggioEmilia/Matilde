using Infragistics.Documents.Excel;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnicodeSrl.DatiClinici.DC;
using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci;
using UnicodeSrl.ScciCore;
using UnicodeSrl.ScciManagement.Model;
using UnicodeSrl.ScciResource;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmSchedeEsporta : Form, Interfacce.IViewFormBase
    {
        public frmSchedeEsporta()
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

            this.picView.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_SCHEDEESPORTA, Enums.EnumImageSize.isz256));

            MyStatics.SetUltraToolbarsManager(this.UltraToolbarsManager);
            MyStatics.SetUltraGroupBox(this.UltraGroupBox);
            this.InitializeUltraToolbarsManager();
            this.InitializeUltraGrid();
            MyStatics.InitializeSaveFileDialog(ref this.SaveFileDialog);

            this.MultiSelect();
            this.SetTipoState(false);
            this.LoadUltraGrid();

            this.ResumeLayout();

        }

        #endregion

        #region ucMultiSelect

        private void MultiSelect()
        {

            string sSql = "Select Codice, Descrizione + ' [' + Codice + ']' As [Schede] From T_Schede {0} Order By Descrizione ASC";

            this.ucMultiSelectSchede.ViewShowAll = true;
            this.ucMultiSelectSchede.ViewShowFind = true;
            this.ucMultiSelectSchede.ViewDataSetSX = DataBase.GetDataSet(string.Format(sSql, ""));
            this.ucMultiSelectSchede.ViewDataSetDX = DataBase.GetDataSet(string.Format(sSql, "Where 0=1"));
            this.ucMultiSelectSchede.ViewInit();
            this.ucMultiSelectSchede.RefreshData();

        }

        #endregion

        #region UltraToolBar

        private void InitializeUltraToolbarsManager()
        {

            try
            {

                MyStatics.SetUltraToolbarsManager(this.UltraToolbarsManagerGriglia);

                foreach (ToolBase oTool in this.UltraToolbarsManagerGriglia.Tools)
                {
                    oTool.SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(oTool.Key, Enums.EnumImageSize.isz32));
                    oTool.SharedProps.AppearancesSmall.Appearance.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(oTool.Key, Enums.EnumImageSize.isz16));
                }

            }
            catch (Exception)
            {

            }

        }

        private void ActionGridToolClick(ToolBase oTool)
        {

            this.Cursor = Cursors.WaitCursor;

            try
            {

                switch (oTool.Key)
                {

                    case MyStatics.GC_STAMPA:
                        try
                        {
                            this.UltraGrid.PrintPreview(Infragistics.Win.UltraWinGrid.RowPropertyCategories.All);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(@"Si sono verificati errori durante il processo di stampa." + Environment.NewLine + ex.Message, @"Errore di stampa", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;

                    case MyStatics.GC_AGGIORNA:
                        this.UltraGrid.Rows.ColumnFilters.ClearAllFilters();
                        this.UltraGridDecodifiche.Rows.ColumnFilters.ClearAllFilters();
                        this.LoadUltraGrid();

                        break;

                    case MyStatics.GC_ESPORTA:
                        if (this.SaveFileDialog.ShowDialog() == DialogResult.OK)
                        {


                            Workbook wb = new Workbook();

                            Worksheet wsSchede = wb.Worksheets.Add("Schede");
                            this.UltraGridExcelExporter.Export(this.UltraGrid, wsSchede);

                            Worksheet wsDec = wb.Worksheets.Add("Decodifiche");
                            this.UltraGridExcelExporter.Export(this.UltraGridDecodifiche, wsDec);

                            wb.Save(this.SaveFileDialog.FileName);

                        }
                        break;

                    case "Tipo":
                        this.SetTipoCaption();
                        break;

                    default:
                        break;

                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

        }

        private bool GetTipoState()
        {

            StateButtonTool oSbt = this.UltraToolbarsManagerGriglia.Tools["Tipo"] as StateButtonTool;
            return oSbt.Checked;

        }

        private void SetTipoState(bool stato)
        {

            StateButtonTool oSbt = this.UltraToolbarsManagerGriglia.Tools["Tipo"] as StateButtonTool;
            oSbt.Checked = stato;
            this.SetTipoCaption();

        }

        private void SetTipoCaption()
        {

            StateButtonTool oSbt = this.UltraToolbarsManagerGriglia.Tools["Tipo"] as StateButtonTool;
            if (oSbt.Checked)
            {
                oSbt.SharedProps.Caption = "Ultima Versione";
            }
            else
            {
                oSbt.SharedProps.Caption = "Versione Attiva";
            }

        }

        #endregion

        #region UltraGrid

        private void InitializeUltraGrid()
        {
            MyStatics.SetUltraGridLayout(ref this.UltraGrid, true, false);
            MyStatics.SetUltraGridLayout(ref this.UltraGridDecodifiche, true, false);
        }

        private void LoadUltraGrid()
        {

            try
            {

                this.UltraGrid.DataSource = getSchedeSelezionate();
                this.UltraGrid.Refresh();
                this.UltraGrid.Text = string.Format("{0} ({1:#,##0})", this.Text, this.UltraGrid.Rows.Count);

                this.UltraGrid.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);

                this.LoadUltraGridDecodifiche();

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void LoadUltraGridDecodifiche()
        {

            try
            {

                this.UltraGridDecodifiche.DataSource = getDecodificheSelezionate();
                this.UltraGridDecodifiche.Refresh();
                this.UltraGridDecodifiche.Text = string.Format("{0} ({1:#,##0})", "Lista Decodifiche", this.UltraGridDecodifiche.Rows.Count);

                this.UltraGridDecodifiche.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private DataSet getSchedeSelezionate()
        {

            DataSet oDsSchema = getSchemaSchedeSelezionate();

            Gestore oGestore = new Gestore();

            try
            {

                DataSet oDsChange = this.ucMultiSelectSchede.ViewDataSetDX.GetChanges();
                foreach (DataRow oRow in oDsChange.Tables[0].Rows)
                {

                    if (oRow.RowState == DataRowState.Added)
                    {

                        Scheda oScheda = null;

                        if (this.GetTipoState())
                        {
                            int nVersione = int.Parse(DataBase.FindValue("IsNull(MAX(Versione),0)", "T_SchedeVersioni", "CodScheda = '" + oRow["Codice"].ToString() + "'", "0"));
                            oScheda = new Scheda(oRow["Codice"].ToString(), nVersione, DateTime.Now, CoreStatics.CoreApplication.Ambiente);
                        }
                        else
                        {
                            oScheda = new Scheda(oRow["Codice"].ToString(), DateTime.Now, CoreStatics.CoreApplication.Ambiente);
                        }

                        oGestore.SchedaXML = oScheda.StrutturaXML;
                        oGestore.SchedaLayoutsXML = oScheda.LayoutXML;

                        foreach (DcSezione oDcSezione in oGestore.Scheda.Sezioni.Values)
                        {

                            foreach (DcVoce oDcVoce in oDcSezione.Voci.Values)
                            {

                                DataRow oDr = oDsSchema.Tables[0].NewRow();
                                oDr["Codice"] = oScheda.Codice;
                                oDr["Descrizione"] = oScheda.Descrizione;

                                oDr["CodSezione"] = oDcSezione.ID;
                                oDr["Sezione"] = oDcSezione.Descrizione;
                                oDr["OrdineSezione"] = oDcSezione.Ordine;

                                oDr["CodCampo"] = oDcVoce.ID;
                                oDr["Campo"] = oDcVoce.Descrizione;
                                oDr["FormatoCampo"] = oDcVoce.Formato.ToString();
                                oDr["TipoControllo"] = oGestore.SchedaLayouts.Layouts[oDcVoce.Key].TipoVoce.ToString();
                                oDr["Decodifica"] = oDcVoce.Decodifica;
                                oDr["OrdineCampo"] = oDcVoce.Ordine;

                                oDsSchema.Tables[0].Rows.Add(oDr);

                            }

                        }

                    }

                }

            }
            catch (Exception)
            {

            }

            oGestore = null;

            return oDsSchema;

        }

        private DataSet getSchemaSchedeSelezionate()
        {

            DataSet oDs = new DataSet();

            try
            {

                DataTable oDt = new DataTable();
                DataColumn oDc = null;
                oDc = new DataColumn("Codice", typeof(string));
                oDt.Columns.Add(oDc);
                oDc = new DataColumn("Descrizione", typeof(string));
                oDt.Columns.Add(oDc);

                oDc = new DataColumn("CodSezione", typeof(string));
                oDt.Columns.Add(oDc);
                oDc = new DataColumn("Sezione", typeof(string));
                oDt.Columns.Add(oDc);
                oDc = new DataColumn("OrdineSezione", typeof(int));
                oDt.Columns.Add(oDc);

                oDc = new DataColumn("CodCampo", typeof(string));
                oDt.Columns.Add(oDc);
                oDc = new DataColumn("Campo", typeof(string));
                oDt.Columns.Add(oDc);
                oDc = new DataColumn("FormatoCampo", typeof(string));
                oDt.Columns.Add(oDc);
                oDc = new DataColumn("TipoControllo", typeof(string));
                oDt.Columns.Add(oDc);
                oDc = new DataColumn("Decodifica", typeof(string));
                oDt.Columns.Add(oDc);
                oDc = new DataColumn("OrdineCampo", typeof(int));
                oDt.Columns.Add(oDc);

                oDs.Tables.Add(oDt);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            return oDs;

        }

        private FwDataBufferedList<T_DCDecodificheValori> getDecodificheSelezionate()
        {

            try
            {

                List<string> lstDecodifiche = this.UltraGrid.Rows.ToList<UltraGridRow>()
                    .Where(r => r.Cells["Decodifica"].Text != string.Empty)
                    .Select(x => x.Cells["Decodifica"].Text).ToList()
                    .Distinct().ToList();

                List<string> lstOrdinamento = new List<string>() { "CodDec", "Codice", "Ordine" };

                using (FwDataConnection conn = new FwDataConnection(MyStatics.Configurazione.ConnectionString))
                {
                    FwDataBufferedList<T_DCDecodificheValori> oDCDecodificheValori = conn.T_DCDecodificheValori(lstDecodifiche, lstOrdinamento);
                    return oDCDecodificheValori;
                }

            }
            catch (Exception)
            {

            }

            return null;

        }

        #endregion

        #region Events

        private void ucMultiSelectSchede_GridDXInitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {

                if (e.Layout.Bands[0].Columns.Exists("Codice") == true) { e.Layout.Bands[0].Columns["Codice"].Hidden = true; }

            }
            catch (Exception)
            {

            }

        }

        private void ucMultiSelectSchede_GridSXInitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {

                if (e.Layout.Bands[0].Columns.Exists("Codice") == true) { e.Layout.Bands[0].Columns["Codice"].Hidden = true; }

            }
            catch (Exception)
            {

            }


        }

        private void UltraToolbarsManagerGriglia_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {

            try
            {
                ActionGridToolClick(e.Tool);
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void UltraGrid_InitializePrintPreview(object sender, CancelablePrintPreviewEventArgs e)
        {

            try
            {

                e.PrintDocument.PrinterSettings.PrintRange = System.Drawing.Printing.PrintRange.AllPages;
                e.DefaultLogicalPageLayoutInfo.PageHeader = this.Text;
                e.DefaultLogicalPageLayoutInfo.PageHeaderHeight = 20;
                e.DefaultLogicalPageLayoutInfo.PageHeaderAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                e.DefaultLogicalPageLayoutInfo.PageHeaderAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                e.DefaultLogicalPageLayoutInfo.PageHeaderAppearance.FontData.SizeInPoints = 12;

            }
            catch (Exception)
            {

            }

        }

        private void UltraGridDecodifiche_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {

                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {

                    switch (oCol.Key)
                    {
                        case "Icona":
                        case "InfoRTF":
                        case "Path":
                            oCol.Hidden = true;
                            break;


                        default:
                            oCol.Hidden = false;
                            break;
                    }

                }

            }
            catch (Exception)
            {
            }

        }

        private void ubChiudi_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

    }
}
