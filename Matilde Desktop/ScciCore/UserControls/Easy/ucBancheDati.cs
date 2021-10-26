using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Data.SqlClient;
using UnicodeSrl.Framework.Data;
using Infragistics.Win.UltraWinGrid;
using System.Collections;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;
using UnicodeSrl.Evaluator;
using Infragistics.Win.UltraWinTree;
using UnicodeSrl.ScciResource;
using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.DatiClinici.DC;
using UnicodeSrl.DatiClinici.Common.Enums;

namespace UnicodeSrl.ScciCore
{
    public partial class ucBancheDati : UserControl, Interfacce.IViewUserControlMiddle
    {

        #region Declare

        Evaluator.Evaluator oEval = new Evaluator.Evaluator();
        Dictionary<string, ParseResult> oParseResult = null;

        UserControl _ucc = null;

        int textboxindex = -1;

        #endregion

        public ucBancheDati()
        {
            InitializeComponent();

        }

        #region Interface

        public void Aggiorna()
        {
            this.CaricaGriglia();
        }

        public void Carica()
        {
            try
            {
                this.InizializzaControlli();
                this.InizializzaUltraGridLayout();
                this.InizializzaUltraTreeView();
                this.CaricaTableLayoutPanel();
                this.CaricaUltraTreeView();

                this.CaricaGriglia();

                _ucc = (UserControl)this;
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Carica", this.Name);
            }
        }

        public void Ferma()
        {

            try
            {

                CoreStatics.SetContesto(EnumEntita.EBM, null);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Ferma", this.Name);
            }

        }

        #endregion

        #region UltraTree

        private void InizializzaUltraTreeView()
        {
            this.ucEasyTreeView.Override.Multiline = Infragistics.Win.DefaultableBoolean.True;
            this.ucEasyTreeView.PerformLayout();
        }

        private void CaricaUltraTreeView()
        {

            UltraTreeNode oNodeRoot = null;
            UltraTreeNode oNodeParent = null;
            UltraTreeNode oNode = null;

            string oNodeKeySelezionato = string.Empty;

            string sKey = "";
            bool bFiltro = false;

            try
            {
                if (CoreStatics.CoreApplication.Paziente != null && this.IsDisposed == false)
                {
                    CoreStatics.SetContesto(EnumEntita.SCH, null);
                    CoreStatics.SetContesto(EnumEntita.APP, null);
                    CoreStatics.SetContesto(EnumEntita.EPI, null);
                    CoreStatics.SetContesto(EnumEntita.PAZ, null);


                    CoreStatics.CoreApplication.IDSchedaSelezionata = "";
                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);
                    op.Parametro.Add("DatiEstesi", "0");

                    SqlParameterExt[] spcoll = new SqlParameterExt[1];
                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                    DataSet ds = Database.GetDatasetStoredProc("MSP_SelAlberoSchede", spcoll);

                    if (!this.ucEasyTreeView.IsDisposed)
                    {

                        if (this.ucEasyTreeView.ActiveNode != null) { oNodeKeySelezionato = this.ucEasyTreeView.ActiveNode.Key; }

                        this.ucEasyTreeView.Nodes.Clear();

                        oNodeRoot = new UltraTreeNode(CoreStatics.TV_ROOT, CoreStatics.CoreApplication.Paziente.Cognome + " " + CoreStatics.CoreApplication.Paziente.Nome);
                        oNodeRoot.LeftImages.Add(CoreStatics.CoreApplication.Paziente.Sesso.ToUpper() == "M" ? Risorse.GetImageFromResource(Risorse.GC_PAZIENTEMASCHIO_32) : Risorse.GetImageFromResource(Risorse.GC_PAZIENTEFEMMINA_32));
                        oNodeRoot.Tag = CoreStatics.TV_ROOT;
                        this.ucEasyTreeView.Nodes.Add(oNodeRoot);

                        bool bAddNode = true;
                        bool bChild = false;
                        while (bAddNode == true)
                        {

                            bAddNode = false;

                            foreach (DataRow oDr in ds.Tables[0].Rows)
                            {

                                if (!this.ucEasyTreeView.IsDisposed)
                                {

                                    try
                                    {
                                        if (bChild == false)
                                        {
                                            if (oDr["IDSchedaPadre"] == DBNull.Value)
                                            {
                                                oNode = new UltraTreeNode(oDr["CodEntita"].ToString() + @"\" + oDr["IDScheda"].ToString(), oDr["Descrizione"].ToString());
                                                oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_SCHEDA_32));
                                                if (int.Parse(oDr["InEvidenza"].ToString()) == 1) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_INRILIEVO_32));
                                                if (int.Parse(oDr["DatiMancanti"].ToString()) == 1) { oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_DATOMANCANTE_32)); }
                                                oNode.Tag = oDr;
                                                oNodeRoot.Nodes.Add(oNode);
                                                bAddNode = true;
                                            }
                                        }
                                        else
                                        {
                                            if (oDr["IDSchedaPadre"] != DBNull.Value)
                                            {
                                                sKey = oDr["CodEntita"].ToString() + @"\" + oDr["IDSchedaPadre"].ToString();
                                                oNodeParent = this.ucEasyTreeView.GetNodeByKey(sKey);
                                                if (oNodeParent != null)
                                                {
                                                    sKey = oDr["CodEntita"].ToString() + @"\" + oDr["IDScheda"].ToString();
                                                    oNode = this.ucEasyTreeView.GetNodeByKey(sKey);
                                                    if (oNode == null)
                                                    {
                                                        oNode = new UltraTreeNode(sKey, oDr["Descrizione"].ToString());
                                                        oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_SCHEDA_32));
                                                        if (int.Parse(oDr["InEvidenza"].ToString()) == 1) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_INRILIEVO_32));
                                                        if (int.Parse(oDr["DatiMancanti"].ToString()) == 1) { oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_DATOMANCANTE_32)); }
                                                        oNode.Tag = oDr;
                                                        oNodeParent.Nodes.Add(oNode);
                                                        bAddNode = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                                        bAddNode = false;
                                    }

                                }

                            }

                            bChild = true;

                        }

                        bAddNode = true;
                        bChild = false;
                        while (bAddNode == true)
                        {

                            bAddNode = false;

                            foreach (DataRow oDr in ds.Tables[1].Rows)
                            {

                                if (!this.ucEasyTreeView.IsDisposed)
                                {

                                    try
                                    {
                                        if (bChild == false)
                                        {
                                            if (oDr["IDSchedaPadre"] == DBNull.Value)
                                            {

                                                sKey = oDr["CodEntita"].ToString() + @"\" + oDr["CodAgenda"].ToString();
                                                oNodeParent = this.ucEasyTreeView.GetNodeByKey(sKey);
                                                if (oNodeParent == null)
                                                {
                                                    oNodeParent = new UltraTreeNode(sKey, oDr["Agenda"].ToString());
                                                    oNodeParent.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_AGENDA_32));
                                                    oNodeParent.Tag = CoreStatics.TV_AGENDE;
                                                    oNodeRoot.Nodes.Add(oNodeParent);
                                                    bAddNode = true;
                                                }
                                                sKey = sKey + @"\" + oDr["IDAppuntamento"].ToString();
                                                oNode = this.ucEasyTreeView.GetNodeByKey(sKey);
                                                if (oNode == null)
                                                {
                                                    oNode = new UltraTreeNode(sKey, oDr["Appuntamento"].ToString());
                                                    oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_APPUNTAMENTO_32));
                                                    oNode.Tag = oDr;
                                                    oNodeParent.Nodes.Add(oNode);
                                                    bAddNode = true;
                                                }
                                                oNodeParent = oNode;

                                                if (oDr["IDScheda"] != DBNull.Value)
                                                {
                                                    oNode = new UltraTreeNode(sKey + @"\" + oDr["IDScheda"].ToString(), oDr["Descrizione"].ToString());
                                                    oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_SCHEDA_32));
                                                    if (int.Parse(oDr["InEvidenza"].ToString()) == 1) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_INRILIEVO_32));
                                                    if (int.Parse(oDr["DatiMancanti"].ToString()) == 1) { oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_DATOMANCANTE_32)); }
                                                    oNode.Tag = oDr;
                                                    oNodeParent.Nodes.Add(oNode);
                                                    bAddNode = true;
                                                }

                                            }
                                        }
                                        else
                                        {
                                            if (oDr["IDSchedaPadre"] != DBNull.Value)
                                            {
                                                sKey = oDr["CodEntita"].ToString() + @"\" + oDr["CodAgenda"].ToString() + @"\" + oDr["IDAppuntamento"].ToString() + @"\" + oDr["IDSchedaPadre"].ToString();
                                                oNodeParent = this.ucEasyTreeView.GetNodeByKey(sKey);
                                                if (oNodeParent != null)
                                                {
                                                    sKey = oDr["CodEntita"].ToString() + @"\" + oDr["CodAgenda"].ToString() + @"\" + oDr["IDAppuntamento"].ToString() + @"\" + oDr["IDScheda"].ToString();
                                                    oNode = this.ucEasyTreeView.GetNodeByKey(sKey);
                                                    if (oNode == null)
                                                    {
                                                        oNode = new UltraTreeNode(sKey, oDr["Descrizione"].ToString());
                                                        oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_SCHEDA_32));
                                                        if (int.Parse(oDr["InEvidenza"].ToString()) == 1) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_INRILIEVO_32));
                                                        if (int.Parse(oDr["DatiMancanti"].ToString()) == 1) { oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_DATOMANCANTE_32)); }
                                                        oNode.Tag = oDr;
                                                        oNodeParent.Nodes.Add(oNode);
                                                        bAddNode = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                                        bAddNode = false;
                                    }

                                }

                            }

                            bChild = true;

                        }

                        bAddNode = true;
                        bChild = false;
                        while (bAddNode == true)
                        {

                            bAddNode = false;

                            foreach (DataRow oDr in ds.Tables[2].Rows)
                            {

                                if (!this.ucEasyTreeView.IsDisposed)
                                {

                                    try
                                    {
                                        if (bChild == false)
                                        {
                                            if (oDr["IDSchedaPadre"] == DBNull.Value)
                                            {

                                                sKey = oDr["CodEntita"].ToString() + @"\" + oDr["IDEpisodio"].ToString();
                                                oNodeParent = this.ucEasyTreeView.GetNodeByKey(sKey);
                                                if (oNodeParent == null)
                                                {
                                                    oNodeParent = new UltraTreeNode(sKey, oDr["Ricovero"].ToString());

                                                    switch (oDr["Dimesso"].ToString())
                                                    {
                                                        case "0":
                                                            oNodeParent.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_LETTO_32));
                                                            break;

                                                        case "1":
                                                            oNodeParent.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_LETTOD_32));
                                                            break;
                                                        case "2":
                                                            oNodeParent.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_LETTODANNULLATO_32));
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                    oNodeParent.Tag = oDr;
                                                    oNodeRoot.Nodes.Add(oNodeParent);
                                                    bAddNode = true;
                                                }

                                                if (oDr["IDScheda"] != DBNull.Value)
                                                {
                                                    oNode = new UltraTreeNode(sKey + @"\" + oDr["IDScheda"].ToString(), oDr["Descrizione"].ToString());
                                                    oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_SCHEDA_32));
                                                    if (int.Parse(oDr["InEvidenza"].ToString()) == 1) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_INRILIEVO_32));
                                                    if (int.Parse(oDr["DatiMancanti"].ToString()) == 1) { oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_DATOMANCANTE_32)); }
                                                    oNode.Tag = oDr;
                                                    oNodeParent.Nodes.Add(oNode);
                                                    bAddNode = true;
                                                }

                                            }
                                        }
                                        else
                                        {
                                            if (oDr["IDSchedaPadre"] != DBNull.Value)
                                            {
                                                sKey = oDr["CodEntita"].ToString() + @"\" + oDr["IDEpisodio"].ToString() + @"\" + oDr["IDSchedaPadre"].ToString();
                                                oNodeParent = this.ucEasyTreeView.GetNodeByKey(sKey);
                                                if (oNodeParent != null)
                                                {
                                                    sKey = oDr["CodEntita"].ToString() + @"\" + oDr["IDEpisodio"].ToString() + @"\" + oDr["IDScheda"].ToString();
                                                    oNode = this.ucEasyTreeView.GetNodeByKey(sKey);
                                                    if (oNode == null)
                                                    {
                                                        oNode = new UltraTreeNode(sKey, oDr["Descrizione"].ToString());
                                                        oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_SCHEDA_32));
                                                        if (int.Parse(oDr["InEvidenza"].ToString()) == 1) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_INRILIEVO_32));
                                                        if (int.Parse(oDr["DatiMancanti"].ToString()) == 1) { oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_DATOMANCANTE_32)); }
                                                        oNode.Tag = oDr;
                                                        oNodeParent.Nodes.Add(oNode);
                                                        bAddNode = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                                        bAddNode = false;
                                    }

                                }

                            }

                            bChild = true;

                        }

                        bAddNode = true;
                        bChild = false;
                        while (bAddNode == true)
                        {

                            bAddNode = false;

                            foreach (DataRow oDr in ds.Tables[3].Rows)
                            {

                                if (!this.ucEasyTreeView.IsDisposed)
                                {

                                    try
                                    {
                                        sKey = oDr["CodEntitaPadre"].ToString() + @"\" + oDr["IDEpisodio"].ToString();
                                        oNodeParent = this.ucEasyTreeView.GetNodeByKey(sKey);
                                        if (oNodeParent != null)
                                        {

                                            if (bChild == false)
                                            {
                                                if (oDr["IDSchedaPadre"] == DBNull.Value)
                                                {

                                                    sKey = oDr["CodEntitaPadre"].ToString() + @"\" + oDr["IDEpisodio"].ToString() + @"\" + oDr["CodAgenda"].ToString();
                                                    oNode = this.ucEasyTreeView.GetNodeByKey(sKey);
                                                    if (oNode == null)
                                                    {
                                                        oNode = new UltraTreeNode(sKey, oDr["Agenda"].ToString());
                                                        oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_AGENDA_32));
                                                        oNode.Tag = CoreStatics.TV_AGENDE;
                                                        oNodeParent.Nodes.Add(oNode);
                                                        bAddNode = true;
                                                    }
                                                    oNodeParent = oNode;

                                                    sKey = sKey + @"\" + oDr["IDAppuntamento"].ToString();
                                                    oNode = this.ucEasyTreeView.GetNodeByKey(sKey);
                                                    if (oNode == null)
                                                    {
                                                        oNode = new UltraTreeNode(sKey, oDr["Appuntamento"].ToString());
                                                        oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_APPUNTAMENTO_32));
                                                        oNode.Tag = oDr;
                                                        oNodeParent.Nodes.Add(oNode);
                                                        bAddNode = true;
                                                    }
                                                    oNodeParent = oNode;

                                                    if (oDr["IDScheda"] != DBNull.Value)
                                                    {
                                                        oNode = new UltraTreeNode(sKey + @"\" + oDr["IDScheda"].ToString(), oDr["Descrizione"].ToString());
                                                        oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_SCHEDA_32));
                                                        if (int.Parse(oDr["InEvidenza"].ToString()) == 1) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_INRILIEVO_32));
                                                        if (int.Parse(oDr["DatiMancanti"].ToString()) == 1) { oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_DATOMANCANTE_32)); }
                                                        oNode.Tag = oDr;
                                                        oNodeParent.Nodes.Add(oNode);
                                                        bAddNode = true;
                                                    }

                                                }
                                            }
                                            else
                                            {
                                                if (oDr["IDSchedaPadre"] != DBNull.Value)
                                                {

                                                    sKey = oDr["CodEntitaPadre"].ToString() + @"\" + oDr["IDEpisodio"].ToString() + @"\" + oDr["CodAgenda"].ToString() + @"\" + oDr["IDAppuntamento"].ToString() + @"\" + oDr["IDSchedaPadre"].ToString();
                                                    oNodeParent = this.ucEasyTreeView.GetNodeByKey(sKey);
                                                    if (oNodeParent != null)
                                                    {
                                                        sKey = oDr["CodEntitaPadre"].ToString() + @"\" + oDr["IDEpisodio"].ToString() + @"\" + oDr["CodAgenda"].ToString() + @"\" + oDr["IDAppuntamento"].ToString() + @"\" + oDr["IDScheda"].ToString();
                                                        oNode = this.ucEasyTreeView.GetNodeByKey(sKey);
                                                        if (oNode == null)
                                                        {
                                                            oNode = new UltraTreeNode(sKey, oDr["Descrizione"].ToString());
                                                            oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_SCHEDA_32));
                                                            if (int.Parse(oDr["InEvidenza"].ToString()) == 1) oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_INRILIEVO_32));
                                                            if (int.Parse(oDr["DatiMancanti"].ToString()) == 1) { oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_DATOMANCANTE_32)); }
                                                            oNode.Tag = oDr;
                                                            oNodeParent.Nodes.Add(oNode);
                                                            bAddNode = true;
                                                        }
                                                    }
                                                }
                                            }

                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                                        bAddNode = false;
                                    }

                                }

                            }

                            bChild = true;

                        }

                        if (!this.ucEasyTreeView.IsDisposed)
                        {

                            this.ucEasyTreeView.PerformAction(UltraTreeAction.FirstNode, false, false);
                            this.ucEasyTreeView.PerformAction(UltraTreeAction.ExpandNode, false, false);

                            if (oNodeKeySelezionato != string.Empty)
                            {
                                oNode = this.ucEasyTreeView.GetNodeByKey(oNodeKeySelezionato);
                                if (oNode != null)
                                {
                                    oNode.Selected = true;
                                    this.ucEasyTreeView.ActiveNode = oNode;
                                    this.ucEasyTreeView.PerformAction(UltraTreeAction.ExpandNode, false, false);
                                }
                            }

                        }
                    }
                }


            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        #endregion

        #region private functions

        private void InizializzaControlli()
        {
            try
            {
                CoreStatics.SetEasyUltraDockManager(ref this.ultraDockManager);

                this.utxtURL.Text = string.Empty;
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaUltraGridLayout", this.Name);
            }
        }

        private void InizializzaUltraGridLayout()
        {
            try
            {
                CoreStatics.SetEasyUltraGridLayout(ref this.ucEasyGridRisorse);
                ucEasyGridRisorse.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;

                CoreStatics.SetEasyUltraGridLayout(ref this.ucEasyGridCampi);
                ucEasyGridCampi.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaUltraGridLayout", this.Name);
            }
        }

        private void CaricaTableLayoutPanel()
        {
            float rowheight = 40;
            this.tlpControlli.SuspendLayout();

            this.tlpControlli.Controls.Clear();
            this.tlpControlli.RowStyles.Clear();
            this.tlpControlli.RowCount = 0;

            if (oParseResult != null)
            {

                for (int i = 0; i <= oParseResult.Count - 1; i++)
                {

                    this.tlpControlli.RowCount = i + 1;
                    this.tlpControlli.RowStyles.Add(new RowStyle(SizeType.Absolute, rowheight));

                    KeyValuePair<string, ParseResult> result = oParseResult.ElementAt(i);

                    ucEasyLabel label = new ucEasyLabel();
                    label.Dock = DockStyle.Fill;
                    label.Appearance.TextVAlign = Infragistics.Win.VAlign.Top;
                    label.Appearance.TextHAlign = Infragistics.Win.HAlign.Left;
                    label.TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
                    label.Name = "label" + i.ToString();
                    label.Text = result.Value.Result.ToList()[0].Key;
                    this.tlpControlli.Controls.Add(label, 0, i);

                    ucEasyTextBox textbox = new ucEasyTextBox();
                    textbox.Dock = DockStyle.Fill;
                    textbox.Appearance.TextVAlign = Infragistics.Win.VAlign.Top;
                    textbox.Appearance.TextHAlign = Infragistics.Win.HAlign.Left;
                    textbox.TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
                    textbox.Name = "textbox" + i.ToString();
                    textbox.Text = string.Empty;
                    textbox.ValueChanged += textbox_ValueChanged;
                    textbox.Enter += textbox_Enter;
                    textbox.Tag = i.ToString();
                    this.tlpControlli.Controls.Add(textbox, 1, i);

                }
            }

            this.tlpControlli.Height = (int)rowheight * this.tlpControlli.RowCount;

            this.tlpControlli.ResumeLayout();

            this.ImpostaURL();

            this.textboxindex = -1;

        }

        void textbox_ValueChanged(object sender, EventArgs e)
        {
            this.ImpostaURL();
        }

        void textbox_Enter(object sender, EventArgs e)
        {
            try
            {
                int.TryParse(((ucEasyTextBox)sender).Tag.ToString(), out this.textboxindex);
            }
            catch
            {
                this.textboxindex = -1;
            }
        }

        private void ImpostaURL()
        {

            string surl = string.Empty;

            if (ucEasyGridRisorse.ActiveRow != null && oParseResult != null)
            {
                surl = ucEasyGridRisorse.ActiveRow.Cells["Url"].Value.ToString();
                surl = surl.Replace(Evaluator.Evaluator.FormulaOpenSeparator.ToString(), string.Empty).Replace(Evaluator.Evaluator.FormulaCloseSeparator.ToString(), string.Empty);

                for (int i = 0; i <= oParseResult.Count - 1; i++)
                {
                    KeyValuePair<string, ParseResult> result = oParseResult.ElementAt(i);
                    ucEasyTextBox textbox = (ucEasyTextBox)this.tlpControlli.Controls["textbox" + i.ToString()];

                    surl = surl.Replace(Evaluator.Evaluator.ReferenceOpenSeparator.ToString() +
                                        result.Value.Result.ToList()[0].Key +
                                        Evaluator.Evaluator.ReferenceCloseSeparator.ToString(), System.Net.WebUtility.HtmlEncode(textbox.Text));
                }

                this.utxtURL.Text = surl;

            }
            else
            {
                this.utxtURL.Text = string.Empty;
            }
        }

        private void CaricaGriglia()
        {

            Parametri op = null;
            string scodUA = string.Empty;
            DataSet ds = null;

            if (CoreStatics.CoreApplication.Trasferimento != null)
                scodUA = CoreStatics.CoreApplication.Trasferimento.CodUA;

            if (CoreStatics.CoreApplication.Paziente != null &&
                CoreStatics.CoreApplication.Paziente.CodUAAmbulatoriale != null &&
                CoreStatics.CoreApplication.Paziente.CodUAAmbulatoriale.Trim() != "")
                scodUA = CoreStatics.CoreApplication.Paziente.CodUAAmbulatoriale;

            if (scodUA != string.Empty)
            {
                op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodUA", scodUA);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                ds = Database.GetDatasetStoredProc("MSP_SelEBM", spcoll);

                if (ds != null)
                {
                    this.ucEasyGridRisorse.DataSource = null;
                    this.ucEasyGridRisorse.DataSource = ds;
                    this.ucEasyGridRisorse.Refresh();

                    this.ucEasyGridRisorse.Selected.Rows.Clear();
                    this.ucEasyGridRisorse.ActiveRow = null;
                    this.ucEasyGridRisorse_AfterRowActivate(this.ucEasyGridRisorse, new EventArgs());
                }

            }
        }

        private string GetValore(ref Gestore oGestore, enumFormatoVoce formatovoce, enumTipoVoce tipovoce, string keyvoce, int sequenzavoce, object valorevoce)
        {

            string sret = string.Empty;

            if (valorevoce != null)
            {
                switch (formatovoce)
                {
                    case enumFormatoVoce.Testo:
                    case enumFormatoVoce.Intero:
                    case enumFormatoVoce.Decimale:
                    case enumFormatoVoce.Data:
                    case enumFormatoVoce.Ora:
                    case enumFormatoVoce.DataOra:
                    case enumFormatoVoce.TestoLungo:
                        switch (tipovoce)
                        {
                            case enumTipoVoce.TestoRtf:
                            case enumTipoVoce.Multipla:
                                break;

                            case enumTipoVoce.Combo:
                            case enumTipoVoce.Zoom:
                                sret = oGestore.LeggeTranscodifica(keyvoce, sequenzavoce);
                                break;

                            default:
                                sret = valorevoce.ToString();
                                break;
                        }
                        break;

                    case DatiClinici.Common.Enums.enumFormatoVoce.Binario:
                    case DatiClinici.Common.Enums.enumFormatoVoce.Oggetto:
                    default:
                        break;
                }
            }

            return sret;

        }

        #endregion

        #region Events

        private void ultraDockManager_InitializePane(object sender, Infragistics.Win.UltraWinDock.InitializePaneEventArgs e)
        {

            e.Pane.Settings.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Medium);
            e.Pane.Settings.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_SCHEDA_32);

            int filtroheight = 12 * (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large);
            this.ultraDockManager.ControlPanes[0].FlyoutSize = new Size(this.ultraDockManager.ControlPanes[0].FlyoutSize.Width, filtroheight);
            this.ultraDockManager.ControlPanes[0].Size = new Size(this.ultraDockManager.ControlPanes[0].FlyoutSize.Width, filtroheight);
            this.ultraDockManager.DockAreas[0].Size = new Size(this.ultraDockManager.ControlPanes[0].FlyoutSize.Width, filtroheight);
            this.tlpSchede.Height = filtroheight;
        }

        private void EasyGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].HeaderVisible = false;
            foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
            {
                switch (oCol.Key)
                {
                    case "Descrizione":
                        oCol.Hidden = false;
                        break;

                    default:
                        oCol.Hidden = true;
                        break;
                }
            }
        }

        private void ucEasyGridRisorse_AfterRowActivate(object sender, EventArgs e)
        {

            if (ucEasyGridRisorse.ActiveRow != null && ucEasyGridRisorse.ActiveRow.Cells["Url"].Value.ToString() != string.Empty)
            {
                oParseResult = oEval.Parsing(ucEasyGridRisorse.ActiveRow.Cells["Url"].Value.ToString());
            }
            else
            {
                oParseResult = null;
            }

            this.CaricaTableLayoutPanel();

        }

        private void ucEasyTreeView_AfterActivate(object sender, NodeEventArgs e)
        {

            DataRow oDr = null;
            MovScheda scheda = null;
            Gestore oGestore = null;

            DataTable odtgrid = null;
            DataRow odtrow = null;

            if (this.IsDisposed == false)
            {

                CoreStatics.SetContesto(EnumEntita.SCH, null);
                CoreStatics.SetContesto(EnumEntita.APP, null);
                CoreStatics.SetContesto(EnumEntita.EPI, null);
                CoreStatics.SetContesto(EnumEntita.PAZ, null);

                try
                {
                    if (e.TreeNode.Tag.GetType() == typeof(DataRow))
                    {
                        oDr = (DataRow)e.TreeNode.Tag;
                    }

                    if (oDr != null)
                    {
                        CoreStatics.SetContesto((EnumEntita)Enum.Parse(typeof(EnumEntita), oDr["CodEntita"].ToString()), oDr["IDEntita"].ToString());
                    }
                    if (oDr != null && oDr["IDScheda"] != DBNull.Value)
                    {
                        CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);
                        this.ubInserisci.Enabled = true;

                        odtgrid = new DataTable();
                        odtgrid.Columns.Add("Codice", typeof(string));
                        odtgrid.Columns.Add("Sequenza", typeof(string));
                        odtgrid.Columns.Add("Descrizione", typeof(string));
                        odtgrid.Columns.Add("Valore", typeof(string));

                        oGestore = CoreStatics.GetGestore();

                        CoreStatics.SetContesto(EnumEntita.SCH, oDr["IDScheda"].ToString());
                        scheda = new MovScheda(oDr["IDScheda"].ToString(), CoreStatics.CoreApplication.Ambiente);

                        oGestore.SchedaXML = scheda.Scheda.StrutturaXML;
                        oGestore.SchedaLayoutsXML = scheda.Scheda.LayoutXML;
                        oGestore.Decodifiche = scheda.Scheda.DizionarioValori();

                        oGestore.SchedaDatiXML = scheda.DatiXML;

                        foreach (DcSezione sezione in oGestore.Scheda.Sezioni.Values)
                        {
                            foreach (DcVoce voce in sezione.Voci.Values)
                            {
                                for (int i = 1; i <= oGestore.LeggeSequenze(voce.Key); i++)
                                {
                                    if (oGestore.SchedaLayouts.Layouts[voce.Key].TipoVoce == enumTipoVoce.Multipla)
                                    {
                                        string[] dati = oGestore.LeggeValore(voce.Key, i).ToString().Split('|');
                                        for (int j = 0; j <= dati.Length - 1; j++)
                                        {
                                            if (dati[j] != string.Empty)
                                            {
                                                odtrow = odtgrid.NewRow();
                                                odtrow["Codice"] = voce.Key;
                                                odtrow["Sequenza"] = i;
                                                odtrow["Descrizione"] = voce.Descrizione;
                                                odtrow["Valore"] = dati[j].ToString();
                                                odtgrid.Rows.Add(odtrow);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        string valore = GetValore(ref oGestore, voce.Formato,
                                                            oGestore.SchedaLayouts.Layouts[voce.Key].TipoVoce, voce.Key, i,
                                                            oGestore.LeggeValore(voce.Key, i));

                                        if (valore != string.Empty)
                                        {
                                            odtrow = odtgrid.NewRow();
                                            odtrow["Codice"] = voce.Key;
                                            odtrow["Sequenza"] = i;
                                            odtrow["Descrizione"] = voce.Descrizione;
                                            odtrow["Valore"] = valore;
                                            odtgrid.Rows.Add(odtrow);
                                        }
                                    }
                                }
                            }
                        }

                        this.ucEasyGridCampi.DataSource = null;
                        this.ucEasyGridCampi.DataSource = odtgrid;
                        this.ucEasyGridCampi.Refresh();

                        this.ucEasyGridCampi.Selected.Rows.Clear();
                        this.ucEasyGridCampi.ActiveRow = null;

                        CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);

                    }
                    else
                    {
                        this.ucEasyGridCampi.DataSource = null;
                        this.ucEasyGridCampi.Refresh();
                        this.utxtValore.Text = string.Empty;
                        this.ubInserisci.Enabled = false;
                    }
                }
                catch (Exception ex)
                {
                    CoreStatics.ExGest(ref ex, "ucEasyTreeView_AfterActivate", this.Name);
                    CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
                }
            }
        }

        private void ucEasyGridCampi_AfterRowActivate(object sender, EventArgs e)
        {
            if (this.ucEasyGridCampi.ActiveRow != null)
            {
                this.utxtValore.Text += this.ucEasyGridCampi.ActiveRow.Cells["Valore"].Value.ToString();
            }
        }

        private void ubBrowse_Click(object sender, EventArgs e)
        {
            CoreStatics.CoreApplication.MovLinkSelezionato = new MovLink(this.utxtURL.Text);
            CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.WebBrowser);
        }

        private void ubInserisci_Click(object sender, EventArgs e)
        {
            if (this.textboxindex >= 0)
            {
                foreach (Control ctl in this.tlpControlli.Controls)
                {
                    if (ctl.GetType() == typeof(ucEasyTextBox))
                    {
                        if (ctl.Tag.ToString() == textboxindex.ToString())
                        {
                            ctl.Text = this.utxtValore.Text;
                            break;
                        }
                    }
                }
            }
            else
            {
                easyStatics.EasyMessageBox("Nessun parametro selezionato per la copia valore!", "Copia Valore", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        #endregion

    }
}
