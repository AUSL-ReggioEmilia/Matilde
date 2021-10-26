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
using UnicodeSrl.ScciResource;
using static UnicodeSrl.ScciManagement.Interfacce;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmPUCDSSStrutturaCopia : Form, IViewFormBase
    {
        public frmPUCDSSStrutturaCopia()
        {
            InitializeComponent();
        }

        #region Declare

        private DataSet DSOrigine = null;
        private DataSet DSDestinazione = null;
        const string C_UAORIGINE = @"Origine";
        const string C_UADESTINAZIONE = @"Destinazione";

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
            MyStatics.SetUltraTree(this.UltraTreeOrigine);
            MyStatics.SetUltraGridLayout(ref this.UltraGridOrigine, true, false);
            MyStatics.SetUltraGridLayout(ref this.UltraGridDestinazione, true, false);

            this.picView.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_CDSSSTRUTTURA, Enums.EnumImageSize.isz256));

            this.LoadUltraTreeOrigine();
            this.LoadUltraGridOrigine(C_UAORIGINE);

            this.ResumeLayout();

        }

        #endregion

        #region UltraTree

        private void LoadUltraTreeOrigine()
        {

            this.Cursor = Cursors.WaitCursor;

            UltraTreeNode oNodeRoot = null;
            UltraTreeNode oNodeUA = null;
            UltraTreeNode oNodeAzione = null;
            UltraTreeNode oNodePlugin = null;
            string sKey = @"";

            try
            {

                this.UltraTreeOrigine.Override.NodeStyle = NodeStyle.CheckBox;
                this.UltraTreeOrigine.Nodes.Clear();
                oNodeRoot = new UltraTreeNode(MyStatics.TV_ROOT, "UA di Origine");
                oNodeRoot.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_FOLDER, Enums.EnumImageSize.isz16)));
                oNodeRoot.Override.NodeStyle = NodeStyle.Standard;
                oNodeRoot.Tag = MyStatics.TV_ROOT;
                this.UltraTreeOrigine.Nodes.Add(oNodeRoot);
                string sSql = @"Select S.CodUA, UA.Descrizione + ' [' + S.CodUA + ']' As UA," + Environment.NewLine +
                        "S.CodAzione, A.Descrizione + ' [' + S.CodAzione + ']' AS Azione," + Environment.NewLine +
                        "S.CodPlugin, P.Descrizione + ' [' + S.CodPlugin + ']' AS Plugin" + Environment.NewLine +
                "From T_CDSSStruttura S" + Environment.NewLine +
                "INNER JOIN T_UnitaAtomiche UA ON S.CodUA = UA.Codice" + Environment.NewLine +
                "INNER JOIN T_CDSSAzioni A ON S.CodAzione = A.Codice" + Environment.NewLine +
                "INNER JOIN T_CDSSPlugins P ON S.CodPlugin = P.Codice" + Environment.NewLine +
                "Order By UA, CodAzione, CodPlugin";
                DataSet oDs = DataBase.GetDataSet(sSql);
                foreach (DataRow oDataRow in oDs.Tables[0].Rows)
                {

                    sKey = oDataRow["CodUA"].ToString();
                    oNodeUA = this.UltraTreeOrigine.GetNodeByKey(sKey);
                    if (oNodeUA == null)
                    {
                        oNodeUA = new UltraTreeNode(sKey, oDataRow["UA"].ToString());
                        oNodeUA.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_UNITAATOMICHE, Enums.EnumImageSize.isz16)));
                        oNodeUA.Tag = MyStatics.GC_UNITAATOMICHE;
                        oNodeRoot.Nodes.Add(oNodeUA);
                    }

                    sKey = oNodeUA.Key + "|" + oDataRow["CodAzione"].ToString();
                    oNodeAzione = this.UltraTreeOrigine.GetNodeByKey(sKey);
                    if (oNodeAzione == null)
                    {
                        oNodeAzione = new UltraTreeNode(sKey, oDataRow["Azione"].ToString());
                        oNodeAzione.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_CDSSAZIONI, Enums.EnumImageSize.isz16)));
                        oNodeAzione.Tag = MyStatics.GC_CDSSAZIONI;
                        oNodeUA.Nodes.Add(oNodeAzione);
                    }

                    sKey = oNodeAzione.Key + "|" + oDataRow["CodPlugin"].ToString();
                    oNodePlugin = this.UltraTreeOrigine.GetNodeByKey(sKey);
                    if (oNodePlugin == null)
                    {
                        oNodePlugin = new UltraTreeNode(sKey, oDataRow["Plugin"].ToString());
                        oNodePlugin.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_CDSSPLUGINS, Enums.EnumImageSize.isz16)));
                        oNodePlugin.Tag = MyStatics.GC_CDSSPLUGINS;
                        oNodeAzione.Nodes.Add(oNodePlugin);
                    }

                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "LoadUltraTreeOrigine", this.Text);
            }

            this.UltraTreeOrigine.PerformAction(UltraTreeAction.FirstNode, false, false);
            this.UltraTreeOrigine.PerformAction(UltraTreeAction.ExpandNode, false, false);

            this.Cursor = Cursors.Default;

        }

        private void CheckTreeViewNode(UltraTreeNode node, CheckState checkedstate)
        {

            foreach (UltraTreeNode item in node.Nodes)
            {

                item.CheckedState = checkedstate;

                if (item.Nodes.Count > 0)
                {
                    this.CheckTreeViewNode(item, checkedstate);
                }

            }

        }

        #endregion

        #region UltraGrid

        private void LoadUltraGridOrigine(string tipo)
        {

            try
            {

                if (DSOrigine == null) { DSOrigine = this.SetDatasetSelezione(); }

                this.DeleteDatasetSelezione(tipo);
                this.AddDatasetSelezione(tipo);

                this.UltraGridOrigine.DataSource = null;
                this.UltraGridOrigine.DataSource = DSOrigine;
                this.UltraGridOrigine.DataBind();
                this.UltraGridOrigine.Refresh();
                this.UltraGridOrigine.Text = string.Format("{0} ({1:#,##0})", "UA di Origine e di Destinazione", this.UltraGridOrigine.Rows.Count);

                this.UltraGridOrigine.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);

            }
            catch (Exception)
            {

            }


        }

        private void LoadUltraGridDestinazione()
        {

            try
            {

                DSDestinazione = this.SetDatasetSelezione();

                foreach (UltraGridRow oRow in this.UltraGridOrigine.Rows)
                {

                    if (oRow.Cells["Tipo"].Text == C_UAORIGINE && oRow.Appearance.BackColor != Color.Red)
                    {
                        DataRow dr = DSDestinazione.Tables[0].NewRow();
                        dr["Tipo"] = C_UADESTINAZIONE;
                        dr["CodUA"] = this.uteCodUA.Text;
                        dr["CodAzione"] = oRow.Cells["CodAzione"].Text;
                        dr["CodPlugin"] = oRow.Cells["CodPlugin"].Text;
                        dr["Parametri"] = oRow.Cells["Parametri"].Text;
                        DSDestinazione.Tables[0].Rows.Add(dr);

                    }

                }

                this.UltraGridDestinazione.DataSource = null;
                this.UltraGridDestinazione.DataSource = DSDestinazione;
                this.UltraGridDestinazione.DataBind();
                this.UltraGridDestinazione.Refresh();
                this.UltraGridDestinazione.Text = string.Format("{0} ({1:#,##0})", "UA di Destinazione", this.UltraGridDestinazione.Rows.Count);

                this.UltraGridDestinazione.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        #endregion

        #region Subroutine

        private DataSet SetDatasetSelezione()
        {

            DataSet ods = null;
            try
            {

                ods = new DataSet();
                ods.Tables.Add(new DataTable());
                ods.Tables[0].Columns.Add("Tipo", typeof(string));
                ods.Tables[0].Columns.Add("CodUA", typeof(string));
                ods.Tables[0].Columns.Add("CodAzione", typeof(string));
                ods.Tables[0].Columns.Add("CodPlugin", typeof(string));
                ods.Tables[0].Columns.Add("Parametri", typeof(string));

            }
            catch (Exception)
            {
                ods = null;
            }

            return ods;

        }

        private void AddDatasetSelezione(string tipo)
        {

            try
            {

                switch (tipo)
                {

                    case C_UAORIGINE:
                        this.AddTreeViewNode(this.UltraTreeOrigine.GetNodeByKey(MyStatics.TV_ROOT));
                        break;

                    case C_UADESTINAZIONE:
                        DataSet oDs = DataBase.GetDataSet("Select CodUA, CodAzione, CodPlugin From T_CDSSStruttura Where CodUA = '" + this.uteCodUA.Text + "' Order By CodUA, CodAzione, CodPlugin");
                        foreach (DataRow oRow in oDs.Tables[0].Rows)
                        {

                            DataRow dr = DSOrigine.Tables[0].NewRow();
                            dr["Tipo"] = tipo;
                            dr["CodUA"] = oRow["CodUA"].ToString();
                            dr["CodAzione"] = oRow["CodAzione"].ToString();
                            dr["CodPlugin"] = oRow["CodPlugin"].ToString();
                            DSOrigine.Tables[0].Rows.Add(dr);

                        }
                        break;

                }

            }
            catch (Exception)
            {
                DSOrigine = null;
            }

        }

        private void DeleteDatasetSelezione(string tipo)
        {

            try
            {

                for (int i = DSOrigine.Tables[0].Rows.Count - 1; i >= 0; i--)
                {
                    if (DSOrigine.Tables[0].Rows[i]["Tipo"].ToString() == tipo)
                    {
                        DSOrigine.Tables[0].Rows[i].Delete();
                    }
                }

                DSOrigine.Tables[0].AcceptChanges();

            }
            catch (Exception)
            {
                DSOrigine = null;
            }

        }

        private bool CheckDestinazione()
        {

            bool bRet = true;

            try
            {

                if (bRet && (this.uteCodUA.Text.Trim() == "" || this.lblCodUADes.Text.Trim() == ""))
                {
                    bRet = false;
                    MessageBox.Show("Unità atomica di destinazione NON selezionata!", "Controlli", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

            }
            catch (Exception)
            {

            }

            return bRet;

        }

        private void AddTreeViewNode(UltraTreeNode node)
        {

            foreach (UltraTreeNode item in node.Nodes)
            {

                if (item.Nodes.Count > 0)
                {
                    this.AddTreeViewNode(item);
                }
                if (item.Tag.ToString() == MyStatics.GC_CDSSPLUGINS && item.CheckedState == CheckState.Checked)
                {
                    string[] arrKeys = item.Key.Split('|');

                    DataRow dr = DSOrigine.Tables[0].NewRow();
                    dr["Tipo"] = C_UAORIGINE;
                    dr["CodUA"] = arrKeys[0];
                    dr["CodAzione"] = arrKeys[1];
                    dr["CodPlugin"] = arrKeys[2];
                    dr["Parametri"] = DataBase.FindValue("Parametri", "T_CDSSStruttura", string.Format("CodUA = '{0}' And CodAzione = '{1}' And CodPlugin = '{2}'", arrKeys[0], arrKeys[1], arrKeys[2]), "");
                    DSOrigine.Tables[0].Rows.Add(dr);

                }

            }

        }

        private bool CheckOrigineDoppio(UltraGridRow row)
        {

            bool bRet = false;

            if (bRet == false && DSOrigine.Tables[0].AsEnumerable().Where(c => (c.Field<string>("Tipo").Equals(C_UADESTINAZIONE)
                                            && c.Field<string>("CodUA").Equals(row.Cells["CodUA"].Text)
                                            && c.Field<string>("CodAzione").Equals(row.Cells["CodAzione"].Text)
                                            && c.Field<string>("CodPlugin").Equals(row.Cells["CodPlugin"].Text))
                                            ).Count() > 0)
            {
                bRet = true;
            }

            if (bRet == false && DSOrigine.Tables[0].AsEnumerable().Where(c => (c.Field<string>("Tipo").Equals(C_UADESTINAZIONE)
                                            && c.Field<string>("CodAzione").Equals(row.Cells["CodAzione"].Text)
                                            && c.Field<string>("CodPlugin").Equals(row.Cells["CodPlugin"].Text))
                                            ).Count() > 0)
            {
                bRet = true;
            }

            if (bRet == false && DSOrigine.Tables[0].AsEnumerable().Where(c => (c.Field<string>("Tipo").Equals(C_UAORIGINE)
                                            && c.Field<string>("CodUA") != row.Cells["CodUA"].Text
                                            && c.Field<string>("CodAzione").Equals(row.Cells["CodAzione"].Text)
                                            && c.Field<string>("CodPlugin").Equals(row.Cells["CodPlugin"].Text))
                                            ).Count() > 0)
            {
                bRet = true;
            }

            return bRet;

        }

        private void CopiaCDSSDestinazione()
        {

            try
            {

                string sSql = "Select * From T_CDSSStruttura Where 0=1";

                DataSet oDs = DataBase.GetDataSet(sSql);

                foreach (UltraGridRow oRow in this.UltraGridDestinazione.Rows)
                {

                    DataRow dr = oDs.Tables[0].NewRow();
                    dr["CodUA"] = oRow.Cells["CodUA"].Text;
                    dr["CodAzione"] = oRow.Cells["CodAzione"].Text;
                    dr["CodPlugin"] = oRow.Cells["CodPlugin"].Text;
                    dr["Parametri"] = oRow.Cells["Parametri"].Text;
                    oDs.Tables[0].Rows.Add(dr);

                }

                DataBase.SaveDataSet(oDs, sSql);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        #endregion

        #region Events

        private void UltraTabControl_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
        {

            switch (e.Tab.Index)
            {

                case 0:
                    break;

                case 1:
                    this.LoadUltraGridDestinazione();
                    this.ubConferma.Enabled = (this.UltraGridDestinazione.Rows.Count != 0);
                    break;

            }

        }

        private void UltraTabControl_SelectedTabChanging(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangingEventArgs e)
        {

            switch (e.Tab.Index)
            {

                case 0:
                    break;

                case 1:
                    e.Cancel = !this.CheckDestinazione();
                    break;

            }


        }

        private void UltraTreeOrigine_AfterCheck(object sender, NodeEventArgs e)
        {
            this.UltraTreeOrigine.EventManager.AllEventsEnabled = false;
            this.CheckTreeViewNode(e.TreeNode, e.TreeNode.CheckedState);
            this.UltraTreeOrigine.EventManager.AllEventsEnabled = true;
            this.LoadUltraGridOrigine(C_UAORIGINE);
        }

        private void uteCodUA_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodUA.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_UnitaAtomiche";
                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodUA.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "uteCodScheda5_EditorButtonClick", this.Text);

            }
        }

        private void uteCodUA_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodUADes.Text = DataBase.FindValue("Descrizione", "T_UnitaAtomiche", "Codice = '" + this.uteCodUA.Text + "'", "");
            this.LoadUltraGridOrigine(C_UADESTINAZIONE);
        }

        private void UltraGrid_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {

                if (e.Layout.Bands[0].Columns.Exists("Tipo") == true)
                {
                    e.Layout.Bands[0].Columns["Tipo"].Hidden = true;
                }

                if (e.Layout.Bands[0].Columns.Exists("Parametri") == true)
                {
                    e.Layout.Bands[0].Columns["Parametri"].Hidden = true;
                }

            }
            catch (Exception)
            {

            }

        }

        private void UltraGridOrigine_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {

            try
            {

                if (e.Row.Cells["Tipo"].Text == C_UAORIGINE && this.CheckOrigineDoppio(e.Row))
                {
                    e.Row.Appearance.BackColor = Color.Red;
                }
                else if (e.Row.Cells["Tipo"].Text == C_UADESTINAZIONE)
                {
                    e.Row.Hidden = true;
                }

            }
            catch (Exception)
            {

            }

        }

        private void UltraGridDestinazione_AfterRowActivate(object sender, EventArgs e)
        {

            try
            {

                this.xmlParametri.Text = this.UltraGridDestinazione.ActiveRow.Cells["Parametri"].Text;
                this.ubSalva.Enabled = false;

            }
            catch (Exception)
            {

            }

        }

        private void ubConferma_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show(string.Format("Sei sicuro di voler copiare {0} CDSS sull'Unità Atomica '{1} ({2})'?",
                                                this.UltraGridDestinazione.Rows.Count,
                                                this.lblCodUADes.Text,
                                                this.uteCodUA.Text), "CDSS Struttura Copia", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                this.CopiaCDSSDestinazione();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }

        }

        private void xmlParametri_XMLTextChanged(object sender, EventArgs e)
        {
            this.ubSalva.Enabled = true;
        }

        private void ubSalva_Click(object sender, EventArgs e)
        {

            try
            {

                this.UltraGridDestinazione.ActiveRow.Cells["Parametri"].Value = this.xmlParametri.Text;
                this.UltraGridDestinazione.UpdateData();
                this.ubSalva.Enabled = false;

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
