using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win.UltraWinTree;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.ScciResource;
using Infragistics.Win.UltraWinGrid;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;

namespace UnicodeSrl.ScciCore
{
    public partial class ucInfoSchede : UserControl, Interfacce.IViewUserControlMiddle
    {
        private UserControl _ucc = null;

        public ucInfoSchede()
        {
            InitializeComponent();

            _ucc = (UserControl)this;
        }

        #region Declare

        bool bModifica = false;
        enumInfoSezione _sezione = enumInfoSezione.infoPaziente;

        public enum enumInfoSezione
        {
            infoPaziente = 0,
            infoEpisodio = 1
        }

        public enumInfoSezione Sezione
        {
            get { return _sezione; }
            set { _sezione = value; }
        }

        #endregion

        #region Interface

        public void Aggiorna()
        {

            try
            {
                this.ResetScheda();
                this.CaricaUltraTreeView();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Aggiorna", this.Name);
            }

        }

        public void Carica()
        {

            CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

            try
            {

                Ruoli r = CoreStatics.CoreApplication.Sessione.Utente.Ruoli;
                bModifica = (r.RuoloSelezionato.Esiste(EnumModules.Schede_Modifica));

                this.ucEasyButtonModifica.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_MODIFICA_256);

                this.ucEasyButtonModifica.PercImageFill = 0.75F;
                this.ucEasyButtonModifica.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.InizializzaUltraTreeView();

                this.Aggiorna();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Carica", this.Name);
            }

            CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);

        }

        public void Ferma()
        {

            try
            {

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Ferma", this.Name);
            }

        }

        #endregion

        #region SubRoutine

        private void ResetScheda()
        {
            this.ucAnteprimaRtfStorico.MovScheda = null;
            this.ucEasyLabelRiga1.Text = "";
            this.ucEasyLabelRiga2.Text = "";
            this.ucEasyLabelRiga3.Text = "";
            this.PictureBox.Image = null;
            ucEasyTableLayoutPanelInfo.Visible = false;
        }

        private void ControllaPulsanti()
        {

            try
            {

                if (this.ucEasyTreeView.ActiveNode == null)
                {
                    this.ucEasyButtonModifica.Enabled = false;
                }
                else
                {
                    if (this.ucEasyTreeView.ActiveNode.Tag.GetType() == typeof(DataRow) || this.ucEasyTreeView.ActiveNode.Tag.GetType() == typeof(DataRowView))
                    {

                        DataRow oDr = getActiveNodeDataRow();

                        if (this.ucAnteprimaRtfStorico.MovScheda != null)
                        {
                            this.ucEasyButtonModifica.Enabled = (bModifica == true && int.Parse(oDr["PermessoModifica"].ToString()) == 1 && !this.ucAnteprimaRtfStorico.Storicizzata);
                        }
                        else
                        {
                            this.ucEasyButtonModifica.Enabled = (bModifica == true && int.Parse(oDr["PermessoModifica"].ToString()) == 1);
                        }
                        if (CoreStatics.CoreApplication.Cartella != null && CoreStatics.CoreApplication.Cartella.CartellaChiusa == true)
                        {
                            this.ucEasyButtonModifica.Enabled = false;
                        }
                    }
                    else
                    {

                        if (this.ucEasyTreeView.ActiveNode.Tag.ToString() == CoreStatics.TV_ROOT)
                        {
                            this.ucEasyButtonModifica.Enabled = false;
                        }
                        else if (this.ucEasyTreeView.ActiveNode.Tag.ToString() == CoreStatics.TV_AGENDE)
                        {
                            this.ucEasyButtonModifica.Enabled = false;
                        }

                    }
                }
            }
            catch (Exception)
            {

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

            UltraTreeNode oNodeParent = null;
            UltraTreeNode oNode = null;

            string oNodeKeySelezionato = string.Empty;

            string sKey = "";

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);
                op.Parametro.Add("DatiEstesi", "0");
                if (_sezione == enumInfoSezione.infoEpisodio)
                    op.Parametro.Add("IDEpisodio", CoreStatics.CoreApplication.Episodio.ID);

                switch (_sezione)
                {
                    case enumInfoSezione.infoPaziente:
                        op.Parametro.Add("CodTipoScheda", UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.TipoSchedaTestataPaziente));
                        op.Parametro.Add("SoloSchedePaziente", "1");
                        break;
                    case enumInfoSezione.infoEpisodio:
                        op.Parametro.Add("CodTipoScheda", UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.TipoSchedaTestataEpisodio));
                        op.Parametro.Add("SoloSchedeEpisodio", "1");
                        break;
                    default:
                        break;
                }

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataSet ds = Database.GetDatasetStoredProc("MSP_SelAlberoSchede", spcoll);

                if (this.ucEasyTreeView.ActiveNode != null) { oNodeKeySelezionato = this.ucEasyTreeView.ActiveNode.Key; }

                this.ucEasyTreeView.Nodes.Clear();

                bool bAddNode = true;
                bool bChild = false;

                switch (_sezione)
                {
                    case enumInfoSezione.infoPaziente:
                        while (bAddNode == true)
                        {

                            bAddNode = false;

                            foreach (DataRow oDr in ds.Tables[0].Rows)
                            {

                                try
                                {
                                    if (bChild == false)
                                    {
                                        if (oDr["IDSchedaPadre"] == DBNull.Value)
                                        {
                                            oNode = new UltraTreeNode(oDr["CodEntita"].ToString() + @"\" + oDr["IDScheda"].ToString(), oDr["Descrizione"].ToString());
                                            oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_SCHEDA_32));
                                            if (int.Parse(oDr["DatiMancanti"].ToString()) == 1) { oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_ALERTALLERGIA_32)); }
                                            oNode.Tag = oDr;
                                            this.ucEasyTreeView.Nodes.Add(oNode);
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
                                                    if (int.Parse(oDr["DatiMancanti"].ToString()) == 1) { oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_ALERTALLERGIA_32)); }
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

                            bChild = true;

                        }

                        break;

                    case enumInfoSezione.infoEpisodio:
                        bAddNode = true;
                        bChild = false;
                        while (bAddNode == true)
                        {

                            bAddNode = false;
                            ds.Tables[2].DefaultView.RowFilter = @"IDEpisodio = '" + CoreStatics.CoreApplication.Episodio.ID + @"'";
                            ds.Tables[2].DefaultView.Sort = @"DataRicovero DESC, Ordine";
                            foreach (DataRowView oDr in ds.Tables[2].DefaultView)
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
                                                oNodeParent.Tag = oDr.Row;
                                                this.ucEasyTreeView.Nodes.Add(oNodeParent);
                                                bAddNode = true;
                                            }

                                            if (oDr["IDScheda"] != DBNull.Value)
                                            {
                                                oNode = new UltraTreeNode(sKey + @"\" + oDr["IDScheda"].ToString(), oDr["Descrizione"].ToString());
                                                oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_SCHEDA_32));
                                                if (int.Parse(oDr["DatiMancanti"].ToString()) == 1) { oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_ALERTALLERGIA_32)); }
                                                oNode.Tag = oDr.Row;
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
                                                    if (int.Parse(oDr["DatiMancanti"].ToString()) == 1) { oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_ALERTALLERGIA_32)); }
                                                    oNode.Tag = oDr.Row;
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

                            bChild = true;

                        }
                        ds.Tables[2].DefaultView.RowFilter = @"";
                        break;

                    default:
                        break;

                }

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
                ControllaPulsanti();
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private DataRow getActiveNodeDataRow()
        {
            return getNodeDataRow(this.ucEasyTreeView.ActiveNode);
        }
        private DataRow getNodeDataRow(UltraTreeNode node)
        {
            DataRow dr = null;
            if (node != null)
            {
                if (node.Tag.GetType() == typeof(DataRow))
                {
                    dr = (DataRow)node.Tag;
                }
                if (node.Tag.GetType() == typeof(DataRowView))
                {
                    dr = ((DataRowView)node.Tag).Row;
                }

            }
            return dr;
        }

        #endregion

        #region Events

        private void ucEasyTreeView_AfterActivate(object sender, NodeEventArgs e)
        {

            try
            {

                DataRow oDr = getNodeDataRow(e.TreeNode);
                if (oDr != null && oDr["IDScheda"] != DBNull.Value)
                {
                    this.ucAnteprimaRtfStorico.MovScheda = new MovScheda(oDr["IDScheda"].ToString(), CoreStatics.CoreApplication.Ambiente);
                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("CodAzioneLock", EnumLock.INFO.ToString());
                    op.TimeStamp.CodEntita = EnumEntita.SCH.ToString();
                    op.TimeStamp.IDEntita = oDr["IDScheda"].ToString();
                    SqlParameterExt[] spcoll = new SqlParameterExt[1];
                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                    DataTable dt = Database.GetDataTableStoredProc("MSP_InfoMovLock", spcoll);
                    if (dt.Rows.Count == 1)
                    {
                        DateTime oDt = DateTime.Parse(dt.Rows[0]["DataUltimaModifica"].ToString());


                        this.ucEasyLabelRiga1.Text = dt.Rows[0]["DescrScheda"].ToString();


                        if ((int)dt.Rows[0]["QtaSchedeTotali"] > 1)
                        {
                            this.ucEasyLabelRiga1.Text += string.Format(" ({0} di {1}",
                                dt.Rows[0]["Numero"].ToString(),
                                dt.Rows[0]["QtaSchedeTotali"].ToString());

                            if ((int)dt.Rows[0]["QtaSchedeAttive"] != (int)dt.Rows[0]["QtaSchedeTotali"])
                            {
                                this.ucEasyLabelRiga1.Text += ", attive " + dt.Rows[0]["QtaSchedeAttive"];
                            }
                            this.ucEasyLabelRiga1.Text += ")";

                            this.ucScrollBarVInfo.Visible = ((int)dt.Rows[0]["QtaSchedeAttive"] > 1 ? true : false);

                        }
                        else
                        {
                            this.ucScrollBarVInfo.Visible = false;
                        }

                        this.ucEasyLabelRiga2.Text = string.Format("Ultima Modifica: {0} Utente: {1}",
CoreStatics.getDateTime(oDt),
dt.Rows[0]["DescrUtenteModifica"].ToString());
                        if (dt.Rows[0]["DataLock"] == DBNull.Value)
                        {
                            this.ucEasyLabelRiga3.Text = "";
                            this.PictureBox.Image = null;
                        }
                        else
                        {
                            oDt = DateTime.Parse(dt.Rows[0]["DataLock"].ToString());
                            this.ucEasyLabelRiga3.Text = string.Format("SCHEDA IN USO DALL'UTENTE {0} DALLE ORE {1}{2}SU {3}",
                                                            dt.Rows[0]["DescrUtenteLock"].ToString(),
                                                            CoreStatics.getDateTime(oDt),
                                                            Environment.NewLine,
                                                            dt.Rows[0]["NomePCLock"].ToString());
                            this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOCHIUSO_32);
                        }
                        ucEasyTableLayoutPanelInfo.Visible = true;
                    }
                    else
                    {
                        this.ucEasyLabelRiga1.Text = "";
                        this.ucEasyLabelRiga2.Text = "";
                        this.ucEasyLabelRiga3.Text = "";
                        this.PictureBox.Image = null;
                        ucEasyTableLayoutPanelInfo.Visible = false;
                    }
                }
                else
                {
                    this.ResetScheda();
                }
                this.ucAnteprimaRtfStorico.RefreshRTF();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyTreeView_AfterActivate", this.Name);
            }

            this.ControllaPulsanti();

        }

        private void ucAnteprimaRtfStorico_StoricoChange(object sender, EventArgs e)
        {
            this.ControllaPulsanti();
        }

        private void ucEasyButtonModifica_Click(object sender, EventArgs e)
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodAzioneLock", EnumLock.LOCK.ToString());
                op.TimeStamp.CodEntita = EnumEntita.SCH.ToString();
                op.TimeStamp.IDEntita = getActiveNodeDataRow()["IDScheda"].ToString();
                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataTable dt = Database.GetDataTableStoredProc("MSP_InfoMovLock", spcoll);

                if (dt.Rows.Count == 1 && int.Parse(dt.Rows[0]["Esito"].ToString()) == 1)
                {
                    CoreStatics.CoreApplication.MovSchedaSelezionata = new MovScheda(getActiveNodeDataRow()["IDScheda"].ToString(), CoreStatics.CoreApplication.Ambiente);
                    CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);
                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.Scheda) == DialogResult.OK)
                    {
                        CoreStatics.CoreApplication.MovSchedaSelezionata.Salva();
                    }
                    CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
                }
                else
                {
                    easyStatics.EasyMessageBox("Scheda bloccata da altro operatore!", "Informazioni Scheda");
                }

                op.Parametro.Remove("CodAzioneLock");
                op.Parametro.Add("CodAzioneLock", EnumLock.UNLOCK.ToString());
                spcoll = new SqlParameterExt[1];
                xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                dt = Database.GetDataTableStoredProc("MSP_InfoMovLock", spcoll);

                this.Aggiorna();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyButtonModifica_Click", "ucInfoSchede");
            }

        }

        private void ucScrollBarVInfo_Button(object sender, ScrollbarEventArgs e)
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.TimeStamp.CodEntita = EnumEntita.SCH.ToString();
                op.TimeStamp.IDEntita = getActiveNodeDataRow()["IDScheda"].ToString();

                switch (e.TypeButton)
                {

                    case ScrollbarEventArgs.EnumTypeButton.Su:
                        op.Parametro.Add("CodAzioneLock", EnumLock.PRECEDENTE.ToString());
                        break;

                    case ScrollbarEventArgs.EnumTypeButton.Giu:
                        op.Parametro.Add("CodAzioneLock", EnumLock.SUCCESSIVA.ToString());
                        break;

                }

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataTable dt = Database.GetDataTableStoredProc("MSP_InfoMovLock", spcoll);
                if (dt.Rows.Count == 1 && dt.Rows[0]["IDSchedaTrovata"].ToString() != op.TimeStamp.IDEntita)
                {
                    switch (_sezione)
                    {
                        case enumInfoSezione.infoPaziente:
                            foreach (UltraTreeNode oNode in this.ucEasyTreeView.Nodes)
                            {
                                if (oNode.Key.Contains(dt.Rows[0]["IDSchedaTrovata"].ToString()) == true)
                                {
                                    oNode.Selected = true;
                                    this.ucEasyTreeView.ActiveNode = oNode;
                                    break;
                                }
                            }
                            break;
                        case enumInfoSezione.infoEpisodio:
                            foreach (UltraTreeNode oNode in this.ucEasyTreeView.ActiveNode.Parent.Nodes)
                            {
                                if (oNode.Key.Contains(dt.Rows[0]["IDSchedaTrovata"].ToString()) == true)
                                {
                                    oNode.Selected = true;
                                    this.ucEasyTreeView.ActiveNode = oNode;
                                    break;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucScrollBarVInfo_Button", "ucInfoSchede");
            }

        }

        #endregion

    }
}
