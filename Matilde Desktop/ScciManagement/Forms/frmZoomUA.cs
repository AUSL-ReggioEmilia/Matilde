using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinTree;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.ScciCore;
using UnicodeSrl.ScciResource;
using static UnicodeSrl.ScciManagement.Interfacce;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmZoomUA : Form, IViewFormZoomBase
    {
        public frmZoomUA()
        {
            InitializeComponent();
        }

        #region Declare

        private string _parametro = string.Empty;

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

        public string ViewParametro
        {
            get
            {
                return _parametro;
            }

            set
            {
                _parametro = value;
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

            this.InitializeUltraToolbarsManager();
            MyStatics.SetUltraGroupBox(this.UltraGroupBox);
            MyStatics.SetUltraTree(this.UltraTree);
            MyStatics.SetUltraGridLayout(ref this.UltraGrid, true, false);
            MyStatics.InitializeSaveFileDialog(ref this.SaveFileDialog);

            this.picView.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_UNITAATOMICHE, Enums.EnumImageSize.isz256));

            this.LoadUltraTree();

            this.ResumeLayout();

        }

        #endregion

        #region UltraToolBar

        private void InitializeUltraToolbarsManager()
        {

            try
            {

                MyStatics.SetUltraToolbarsManager(this.UltraToolbarsManager);

                foreach (ToolBase oTool in this.UltraToolbarsManagerGrid.Tools)
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

                    case MyStatics.GC_ESPORTA:
                        if (this.SaveFileDialog.ShowDialog() == DialogResult.OK)
                            this.UltraGridExcelExporter.Export(this.UltraGrid, this.SaveFileDialog.FileName);
                        break;

                    default:
                        break;

                }

            }
            catch (Exception)
            {

            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

        }

        #endregion

        #region UltraTree

        private void LoadUltraTree()
        {

            this.Cursor = Cursors.WaitCursor;

            UltraTreeNode oNodeParent = null;
            UltraTreeNode oNode = null;

            try
            {


                this.UltraTree.Nodes.Clear();
                UltraTreeNode oNodeRoot = new UltraTreeNode(MyStatics.TV_ROOT, MyStatics.TV_ROOT);
                oNodeRoot.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_FOLDER, Enums.EnumImageSize.isz16)));
                oNodeRoot.Tag = MyStatics.TV_ROOT;
                this.UltraTree.Nodes.Add(oNodeRoot);

                Scci.DataContracts.ScciAmbiente ambiente = CoreStatics.CoreApplication.Ambiente;
                ambiente.Codlogin = UnicodeSrl.Framework.Utility.Windows.CurrentUser();
                Parametri op = new Parametri(ambiente);
                op.Parametro.Add("CodUA", this.ViewParametro);
                string xmlParam = UnicodeSrl.Scci.Statics.XmlProcs.XmlSerializeToString(op);
                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataSet oDs = UnicodeSrl.Scci.Statics.Database.GetDatasetStoredProc("MSP_SelUAPadri", spcoll);

                oNodeParent = oNodeRoot;
                foreach (DataRow oDataRow in oDs.Tables[0].Rows)
                {

                    oNode = new UltraTreeNode(oDataRow["Codice"].ToString(), oDataRow["Descrizione"].ToString() + " [" + oDataRow["Codice"].ToString() + "]");
                    oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_UNITAATOMICHE, Enums.EnumImageSize.isz16)));
                    oNode.Tag = MyStatics.TV_UNITAATOMICHE;
                    oNodeParent.Nodes.Add(oNode);
                    oNode.Expanded = true;

                    oNodeParent = oNode;

                }

                this.UltraTree.PerformAction(UltraTreeAction.FirstNode, false, false);
                this.UltraTree.PerformAction(UltraTreeAction.ExpandNode, false, false);

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "LoadUltraTree", this.Text);
            }

            this.Cursor = Cursors.Default;

        }

        #endregion

        #region UltraGrid

        private void LoadUltraGrid(UltraTreeNode oNode)
        {

            try
            {

                this.UltraGrid.DataSource = getEntita(oNode);
                this.UltraGrid.Refresh();
                this.UltraGrid.Text = string.Format("{0} ({1:#,##0})", this.Text, this.UltraGrid.Rows.Count);

                this.UltraGrid.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);

                this.UltraGrid.DisplayLayout.ViewStyleBand = ViewStyleBand.OutlookGroupBy;
                this.UltraGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                this.UltraGrid.DisplayLayout.Bands[0].SortedColumns.Add("CodEntita", false, true);

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "LoadUltraGrid", this.Text);
            }

        }

        #endregion

        #region Subroutine

        private DataSet getEntita(UltraTreeNode oNode)
        {


            DataSet oDs = null;
            string sSql = "Select * From T_AssUAEntita Where CodUA = '{0}' And CodEntita = '{1}' Order By CodVoce";

            sSql = "Select AUA.CodUA, UA.Descrizione as UA, AUA.CodEntita, AUA.CodVoce, DV.Descrizione AS Voce From T_AssUAEntita AUA" + Environment.NewLine +
                        "Inner Join T_UnitaAtomiche UA On AUA.CodUA = UA.Codice" + Environment.NewLine +
                        "Inner Join {2} DV On AUA.CodVoce = DV.Codice" + Environment.NewLine +
                    "Where AUA.CodUA = '{0}' And AUA.CodEntita = '{1}'" + Environment.NewLine +
                    "Order By AUA.CodVoce";

            try
            {

                oDs = DataBase.GetDataSet(string.Format(sSql, oNode.Key, EnumEntita.AGE.ToString(), "T_Agende"));
                oDs.Tables[0].Merge(DataBase.GetDataSet(string.Format(sSql, oNode.Key, EnumEntita.APP.ToString(), "T_TipoAppuntamento")).Tables[0]);
                oDs.Tables[0].Merge(DataBase.GetDataSet(string.Format(sSql, oNode.Key, EnumEntita.DCL.ToString(), "T_TipoVoceDiario")).Tables[0]);
                oDs.Tables[0].Merge(DataBase.GetDataSet(string.Format(sSql, oNode.Key, EnumEntita.PRF.ToString(), "T_TipoPrescrizione")).Tables[0]);
                oDs.Tables[0].Merge(DataBase.GetDataSet(string.Format(sSql, oNode.Key, EnumEntita.PVT.ToString(), "T_TipoParametroVitale")).Tables[0]);
                oDs.Tables[0].Merge(DataBase.GetDataSet(string.Format(sSql, oNode.Key, EnumEntita.RPT.ToString(), "T_Report")).Tables[0]);
                oDs.Tables[0].Merge(DataBase.GetDataSet(string.Format(sSql, oNode.Key, EnumEntita.SCH.ToString(), "T_Schede")).Tables[0]);
                oDs.Tables[0].Merge(DataBase.GetDataSet(string.Format(sSql, oNode.Key, EnumEntita.TST.ToString(), "T_TestiPredefiniti")).Tables[0]);
                oDs.Tables[0].Merge(DataBase.GetDataSet(string.Format(sSql, oNode.Key, EnumEntita.WKI.ToString(), "T_TipoTaskInfermieristico")).Tables[0]);
                oDs.Tables[0].Merge(DataBase.GetDataSet(string.Format(sSql, oNode.Key, EnumEntita.EBM.ToString(), "T_EBM")).Tables[0]);
                oDs.Tables[0].Merge(DataBase.GetDataSet(string.Format(sSql, oNode.Key, EnumEntita.ALG.ToString(), "T_TipoAlertGenerico")).Tables[0]);
                oDs.Tables[0].Merge(DataBase.GetDataSet(string.Format(sSql, oNode.Key, EnumEntita.NWS.ToString(), "T_TipoNews")).Tables[0]);

                if (oNode.Parent != null)
                {
                    UltraTreeNode oNodeActive = oNode;
                    while (oNodeActive.Parent.Tag.ToString() != MyStatics.TV_ROOT)
                    {
                        oDs.Tables[0].Merge(DataBase.GetDataSet(string.Format(sSql, oNodeActive.Parent.Key, EnumEntita.AGE.ToString(), "T_Agende")).Tables[0]);
                        oDs.Tables[0].Merge(DataBase.GetDataSet(string.Format(sSql, oNodeActive.Parent.Key, EnumEntita.APP.ToString(), "T_TipoAppuntamento")).Tables[0]);
                        oDs.Tables[0].Merge(DataBase.GetDataSet(string.Format(sSql, oNodeActive.Parent.Key, EnumEntita.DCL.ToString(), "T_TipoVoceDiario")).Tables[0]);
                        oDs.Tables[0].Merge(DataBase.GetDataSet(string.Format(sSql, oNodeActive.Parent.Key, EnumEntita.PRF.ToString(), "T_TipoPrescrizione")).Tables[0]);
                        oDs.Tables[0].Merge(DataBase.GetDataSet(string.Format(sSql, oNodeActive.Parent.Key, EnumEntita.PVT.ToString(), "T_TipoParametroVitale")).Tables[0]);
                        oDs.Tables[0].Merge(DataBase.GetDataSet(string.Format(sSql, oNodeActive.Parent.Key, EnumEntita.RPT.ToString(), "T_Report")).Tables[0]);
                        oDs.Tables[0].Merge(DataBase.GetDataSet(string.Format(sSql, oNodeActive.Parent.Key, EnumEntita.SCH.ToString(), "T_Schede")).Tables[0]);
                        oDs.Tables[0].Merge(DataBase.GetDataSet(string.Format(sSql, oNodeActive.Parent.Key, EnumEntita.TST.ToString(), "T_TestiPredefiniti")).Tables[0]);
                        oDs.Tables[0].Merge(DataBase.GetDataSet(string.Format(sSql, oNodeActive.Parent.Key, EnumEntita.WKI.ToString(), "T_TipoTaskInfermieristico")).Tables[0]);
                        oDs.Tables[0].Merge(DataBase.GetDataSet(string.Format(sSql, oNodeActive.Parent.Key, EnumEntita.EBM.ToString(), "T_EBM")).Tables[0]);
                        oDs.Tables[0].Merge(DataBase.GetDataSet(string.Format(sSql, oNodeActive.Parent.Key, EnumEntita.ALG.ToString(), "T_TipoAlertGenerico")).Tables[0]);
                        oDs.Tables[0].Merge(DataBase.GetDataSet(string.Format(sSql, oNodeActive.Parent.Key, EnumEntita.NWS.ToString(), "T_TipoNews")).Tables[0]);
                        oNodeActive = oNodeActive.Parent;
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            return oDs;

        }

        #endregion

        #region Events

        private void UltraToolBarManagerGrid_ToolClick(object sender, ToolClickEventArgs e)
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

        private void UltraTree_AfterActivate(object sender, NodeEventArgs e)
        {
            this.LoadUltraGrid(e.TreeNode);
        }

        private void UltraGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {


            }
            catch (Exception)
            {

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
                e.DefaultLogicalPageLayoutInfo.FitWidthToPages = 1;

            }
            catch (Exception)
            {

            }

        }

        private void UltraGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {

            try
            {

                if (e.Row.Cells.Exists("CodUA") && e.Row.Cells["CodUA"].Text != this.UltraTree.ActiveNode.Key)
                {
                    e.Row.Appearance.BackColor = Color.LightYellow;
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
