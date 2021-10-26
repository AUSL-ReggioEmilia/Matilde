using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.ScciResource;
using Infragistics.Win.UltraWinTree;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmModuli : Form, Interfacce.IViewFormBase
    {
        public frmModuli()
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

            MyStatics.SetUltraToolbarsManager(this.UltraToolbarsManager);
            MyStatics.SetUltraGroupBox(this.UltraGroupBox);
            MyStatics.SetUltraTree(this.UltraTree);
            this.UltraTree.AllowDrop = true;

            MyStatics.SetUltraGridLayout(ref this.UltraGrid, false, false);
            this.UltraGrid.DisplayLayout.GroupByBox.Hidden = true;
            this.UltraGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            this.UltraGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.UltraGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.UltraGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.UltraGrid.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            this.UltraGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.Edit;

            this.picView.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_MODULI, Enums.EnumImageSize.isz256));

            this.LoadUltraTreePath();
            this.LoadUltraTree();

            this.ResumeLayout();

        }

        #endregion

        #region UltraTree

        private void LoadUltraTreePath()
        {

            this.Cursor = Cursors.WaitCursor;

            try
            {

                UltraTreeNode oNode = null;
                UltraTreeNode oNodeParent = null;
                string sKey = @"";
                this.UltraTree.Nodes.Clear();
                oNode = new UltraTreeNode(MyStatics.TV_ROOT, MyStatics.TV_ROOT);
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_FOLDER, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_ROOT;
                this.UltraTree.Nodes.Add(oNode);
                string sSql = @"Select IsNull(Path,'') As Path From T_Moduli Group By Path Order By Path";
                DataSet oDs = DataBase.GetDataSet(sSql);
                foreach (DataRow oDataRow in oDs.Tables[0].Rows)
                {

                    oNode = this.UltraTree.GetNodeByKey(MyStatics.TV_ROOT);
                    sKey = @"";
                    MyStatics.g_split = MyStatics.SetSplit(oDataRow["Path"].ToString(), @"\");
                    for (int i = 0; i < MyStatics.g_split.Length; i++)
                    {

                        sKey += (sKey != "" ? @"\" : "") + MyStatics.g_split.GetValue(i).ToString();
                        oNodeParent = this.UltraTree.GetNodeByKey(sKey);
                        if (oNodeParent == null)
                        {

                            oNodeParent = new UltraTreeNode(sKey, MyStatics.g_split.GetValue(i).ToString());
                            oNodeParent.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_FOLDER, Enums.EnumImageSize.isz16)));
                            oNodeParent.Tag = MyStatics.TV_PATH;
                            oNode.Nodes.Add(oNodeParent);

                        }

                        oNode = oNodeParent;

                    }

                }

                this.UltraTree.PerformAction(UltraTreeAction.FirstNode, false, false);
                this.UltraTree.PerformAction(UltraTreeAction.ExpandNode, false, false);

                this.Cursor = Cursors.Default;

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "LoadUltraTreePath", this.Text);
            }

            this.Cursor = Cursors.Default;

        }

        private void LoadUltraTree()
        {

            this.Cursor = Cursors.WaitCursor;

            try
            {

                UltraTreeNode oNode = null;
                UltraTreeNode oNodeParent = null;
                string sSql = @"Select * From T_Moduli  Order by Descrizione";
                DataSet oDs = DataBase.GetDataSet(sSql);
                foreach (DataRow oDataRow in oDs.Tables[0].Rows)
                {

                    oNodeParent = this.UltraTree.GetNodeByKey(oDataRow["Path"].ToString());
                    if (oNodeParent != null)
                    {

                        oNode = new UltraTreeNode(oDataRow["Path"].ToString() + @"\" + oDataRow["Codice"].ToString(), oDataRow["Descrizione"].ToString());
                        oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_MODULI, Enums.EnumImageSize.isz16)));
                        oNode.Tag = MyStatics.TV_MODULI;
                        oNodeParent.Nodes.Add(oNode);

                    }

                }

                this.UltraTree.PerformAction(UltraTreeAction.FirstNode, false, false);
                this.UltraTree.PerformAction(UltraTreeAction.ExpandNode, false, false);

                this.Cursor = Cursors.Default;

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "LoadUltraTree", this.Text);
            }

            this.Cursor = Cursors.Default;

        }

        #endregion

        #region UltraGrid

        private void LoadUltraGrid()
        {

            string sSql = @"Select Codice, Descrizione, convert(bit,1) As Diritti From T_Ruoli" + Environment.NewLine +
                            "Where Codice In (Select CodRuolo From T_AssRuoliModuli Where CodModulo = '{0}')" + Environment.NewLine +
                            "Union" + Environment.NewLine +
                            "Select Codice, Descrizione, convert(bit,0) As Diritti From T_Ruoli" + Environment.NewLine +
                            "Where Codice Not In (Select CodRuolo From T_AssRuoliModuli Where CodModulo = '{0}')" + Environment.NewLine +
                            "Order By Codice";

            try
            {

                switch (this.UltraTree.ActiveNode.Tag.ToString())
                {

                    case MyStatics.TV_MODULI:
                        MyStatics.g_split = MyStatics.SetSplit(this.UltraTree.ActiveNode.Key, @"\");
                        sSql = string.Format(sSql, MyStatics.g_split.GetValue(MyStatics.g_split.Length - 1));
                        break;

                    default:
                        sSql = "Select Codice, Descrizione, convert(bit,1) As Diritti From T_Ruoli Where 0=1";
                        break;

                }

                this.UltraGrid.DataSource = DataBase.GetDataSet(sSql);
                this.UltraGrid.Refresh();
                this.UltraGrid.DisplayLayout.PerformAutoResizeColumns(false, Infragistics.Win.UltraWinGrid.PerformAutoSizeType.AllRowsInBand);

            }
            catch (Exception)
            {

            }

        }

        #endregion

        #region Events

        private void frmModuli_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.UltraGrid != null) this.UltraGrid.Dispose();
        }

        private void UltraTree_AfterActivate(object sender, NodeEventArgs e)
        {
            this.LoadUltraGrid();
            this.uteCercaDX_ValueChanged(this.uteCercaDX, new EventArgs());
        }

        private void UltraTree_DragDrop(object sender, DragEventArgs e)
        {

            try
            {

                UltraTreeNode oUltraTreeNode = (UltraTreeNode)e.Data.GetData("Infragistics.Win.UltraWinTree.UltraTreeNode", true);
                UltraTree UltraTree = sender as UltraTree;
                Point ptClient = UltraTree.PointToClient(new Point(e.X, e.Y));
                UltraTreeNode oItem = UltraTree.GetNodeFromPoint(ptClient);

                if (oUltraTreeNode != null && oItem != null)
                {

                    MyStatics.g_split = MyStatics.SetSplit(oUltraTreeNode.Key, @"\");

                    UltraTreeNode oNode = new UltraTreeNode(oItem.Key + @"\" + MyStatics.g_split.GetValue(MyStatics.g_split.Length - 1), oUltraTreeNode.Text);
                    oNode.LeftImages.Add(oUltraTreeNode.LeftImages[0]);
                    oNode.Tag = oUltraTreeNode.Tag;
                    oItem.Nodes.Add(oNode);

                    string sSql = string.Format("Update T_Moduli Set Path = '{0}' Where Codice = '{1}'", oItem.Key, MyStatics.g_split.GetValue(MyStatics.g_split.Length - 1));
                    DataBase.ExecuteSql(sSql);

                    oUltraTreeNode.Remove();

                }

            }
            catch (Exception)
            {

            }

        }

        private void UltraTree_DragOver(object sender, DragEventArgs e)
        {

            try
            {

                UltraTreeNode oUltraTreeNode = (UltraTreeNode)e.Data.GetData("Infragistics.Win.UltraWinTree.UltraTreeNode", true);
                UltraTree UltraTree = sender as UltraTree;
                Point ptClient = UltraTree.PointToClient(new Point(e.X, e.Y));
                UltraTreeNode oItem = UltraTree.GetNodeFromPoint(ptClient);

                if (oUltraTreeNode != null && oItem != null)
                {
                    if (oItem.Tag.ToString() == MyStatics.TV_PATH)
                    {
                        e.Effect = DragDropEffects.Move;
                    }
                    else
                    {
                        e.Effect = DragDropEffects.None;
                    }
                }

            }
            catch (Exception)
            {

            }

        }

        private void UltraTree_MouseMove(object sender, MouseEventArgs e)
        {

            try
            {

                UltraTree UltraTree = sender as UltraTree;
                if (e.Button == MouseButtons.Left && UltraTree.SelectedNodes.Count > 0)
                {
                    if (UltraTree.SelectedNodes[0].Tag.ToString() == MyStatics.TV_MODULI)
                    {
                        UltraTree.DoDragDrop(UltraTree.SelectedNodes[0], DragDropEffects.Move);
                    }
                }

            }
            catch (Exception)
            {

            }

        }

        private void UltraGrid_CellChange(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            this.UltraGrid.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.ExitEditMode, false, false);
        }

        private void UltraGrid_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            this.UltraGrid.UpdateData();
            e.Cell.Row.Refresh(Infragistics.Win.UltraWinGrid.RefreshRow.FireInitializeRow);
        }

        private void UltraGrid_ClickCell(object sender, Infragistics.Win.UltraWinGrid.ClickCellEventArgs e)
        {

            try
            {

                if (e.Cell.Column.Key == "Diritti")
                {

                    MyStatics.g_split = MyStatics.SetSplit(this.UltraTree.ActiveNode.Key, @"\");
                    string sSql = @"";
                    if ((bool)e.Cell.Value == true)
                    {
                        sSql = "Delete From T_AssRuoliModuli Where CodRuolo = '" + e.Cell.Row.Cells["Codice"].Value + "' And CodModulo = '" + MyStatics.g_split.GetValue(MyStatics.g_split.Length - 1) + "'";
                    }
                    else
                    {
                        sSql = "Insert Into T_AssRuoliModuli (CodRuolo, CodModulo) Values ('" + e.Cell.Row.Cells["Codice"].Value + "', '" + MyStatics.g_split.GetValue(MyStatics.g_split.Length - 1) + "')";
                    }
                    DataBase.ExecuteSql(sSql);
                    this.LoadUltraGrid();
                }

            }
            catch (Exception)
            {

            }

        }

        private void UltraGrid_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {

                if (e.Layout.Bands[0].Columns.Exists("Codice") == true) { e.Layout.Bands[0].Columns["Codice"].Hidden = true; }

                if (e.Layout.Bands[0].Columns.Exists("Descrizione") == true) { e.Layout.Bands[0].Columns["Descrizione"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit; }

                if (e.Layout.Bands[0].Columns.Exists("Diritti") == true)
                {
                    e.Layout.Bands[0].Columns["Diritti"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
                }

            }
            catch (Exception)
            {

            }

        }

        private void UltraGrid_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {

            try
            {

                if (e.Row.Cells.Exists("Descrizione") == true)
                {
                    if ((bool)e.Row.Cells["Diritti"].Value == false)
                    {
                        e.Row.Cells["Descrizione"].Appearance.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_SYMBOL_RESTRICTED, Enums.EnumImageSize.isz16));
                    }
                    else
                    {
                        e.Row.Cells["Descrizione"].Appearance.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_SYMBOL_CHECK, Enums.EnumImageSize.isz16));
                    }
                }

            }
            catch (Exception)
            {

            }

        }
        private void uteCercaDX_ValueChanged(object sender, EventArgs e)
        {

            try
            {
                string gridCaptionColumnKey = this.UltraGrid.DisplayLayout.Bands[0].Columns[1].Key;
                string gridCaption = this.UltraGrid.DisplayLayout.Bands[0].Columns[1].Key;
                string filtercolumnkey = this.UltraGrid.DisplayLayout.Bands[0].Columns[1].Key;

                MyStatics.SetGridWizardFilter(ref this.UltraGrid,
                                                filtercolumnkey,
                                                this.uteCercaDX.Text,
                                                gridCaptionColumnKey,
                                                gridCaption);



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
