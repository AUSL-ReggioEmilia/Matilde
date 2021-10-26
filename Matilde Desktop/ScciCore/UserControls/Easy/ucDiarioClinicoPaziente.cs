using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;

using Microsoft.Data.SqlClient;
using System.Globalization;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.ScciResource;
using UnicodeSrl.ScciCore.CustomControls.Infra;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.PluginClient;

namespace UnicodeSrl.ScciCore
{
    public partial class ucDiarioClinicoPaziente : UserControl, Interfacce.IViewUserControlMiddle
    {

        #region Declare

        private enum enumRTFRender
        {
            showPopup = 0,
            resizeRow = 1,
            showScrollBars = 2,
            resizeRowAndPopup = 3
        }
        private enumRTFRender _rtfRender = enumRTFRender.showPopup;

        private UserControl _ucc = null;
        private ucRichTextBox _ucRichTextBox = null;

        private Dictionary<int, byte[]> oIcone = new Dictionary<int, byte[]>();

        private bool bInviaConsegna = false;

        #endregion

        public ucDiarioClinicoPaziente()
        {
            InitializeComponent();

            _ucc = (UserControl)this;
        }

        #region INTERFACCIA

        public void Aggiorna()
        {
            if (this.IsDisposed == false)
            {
                CoreStatics.SetNavigazione(false);

                CaricaDati(true, false);


                CoreStatics.SetNavigazione(true);

            }
        }

        public void Carica()
        {
            try
            {

                bInviaConsegna = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.DiarioC_InviaConsegne);
                if (bInviaConsegna)
                {
                    bInviaConsegna = CoreStatics.CheckSelezionaTipoConsegnaPaziente(CoreStatics.CoreApplication.Trasferimento.CodUA,
                                                                                    CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice,
                                                                                    null);
                }

                InizializzaControlli();
                InizializzaUltraGridDiarioLayout();
                VerificaSicurezza();
                InizializzaFiltri();

                CoreStatics.CoreApplication.IDDiarioClinicoSelezionato = "";

                CaricaDati(true, true);
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

                oIcone = new Dictionary<int, byte[]>();

                CoreStatics.SetContesto(EnumEntita.DCL, null);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Ferma", this.Name);
            }

        }

        #endregion

        #region FUNZIONI

        private void InizializzaControlli()
        {

            try
            {
                CoreStatics.SetEasyUltraDockManager(ref this.ultraDockManager);
                this.ubAdd.Appearance.Image = Properties.Resources.Aggiungi_256;
                this.ubAdd.PercImageFill = 0.75F;
                this.ubAdd.ShortcutKey = Keys.Add;
                this.ubAdd.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                this.ubAdd.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;

                this.ubValida.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FIRMAMULTIPLA_256);
                this.ubValida.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.uchkFiltro.UNCheckedImage = Risorse.GetImageFromResource(Risorse.GC_FILTRO_256);
                this.uchkFiltro.CheckedImage = Risorse.GetImageFromResource(Risorse.GC_FILTROAPPLICATO_256);
                this.uchkFiltro.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                this.uchkFiltro.PercImageFill = 0.75F;
                this.uchkFiltro.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;

                this.uchkFiltro.Checked = false;

                this.ubValida.PercImageFill = 0.75F;
                this.ubValida.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                this.ubValida.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;

            }
            catch (Exception)
            {
            }
        }

        private void VerificaSicurezza()
        {

            try
            {
                if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.DiarioC_Inserisci))
                    this.ubAdd.Enabled = true;
                else
                    this.ubAdd.Enabled = false;

                if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.DiarioC_Valida))
                    this.ubValida.Enabled = true;
                else
                    this.ubValida.Enabled = false;
                if (CoreStatics.CoreApplication.Cartella != null && CoreStatics.CoreApplication.Cartella.CartellaChiusa == true)
                {
                    this.ubAdd.Enabled = false;
                    this.ubValida.Enabled = false;
                }
            }
            catch (Exception)
            {
            }
        }

        private void InizializzaUltraGridDiarioLayout()
        {
            try
            {
                CoreStatics.SetEasyUltraGridLayout(ref this.ucEasyGrid);
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaUltraGridLayout", this.Name);
            }
        }

        private void InizializzaFiltri()
        {
            if (this.IsDisposed == false)
            {
                try
                {
                    this.uchkInfermieristico.UNCheckedImage = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_DIARIOINFERMIERISTICO_256);
                    this.uchkInfermieristico.CheckedImage = Properties.Resources.Diario_InfermieristicoFiltro_256;
                    this.uchkMedico.UNCheckedImage = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_DIARIOMEDICO_256);
                    this.uchkMedico.CheckedImage = Properties.Resources.Diario_ClinicoFiltro_256;

                    this.ubApplicaFiltro.PercImageFill = 0.75F;
                    this.ubApplicaFiltro.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    this.ubApplicaFiltro.TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Large;

                    this.uchkMedico.Checked = true;
                    this.uchkInfermieristico.Checked = true;

                    this.uchkMedico.PercImageFill = 0.75F;
                    this.uchkMedico.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                    this.uchkInfermieristico.PercImageFill = 0.75F;
                    this.uchkInfermieristico.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                    this.drFiltro.Value = null;
                    this.udteFiltroDA.Value = null;
                    this.udteFiltroA.Value = null;

                    this.utxtFiltroContenuto.Text = "";

                    if (this.ucEasyGridFiltroUtente.Rows.Count > 0)
                    {
                        this.ucEasyGridFiltroUtente.Selected.Rows.Clear();
                        this.ucEasyGridFiltroUtente.ActiveRow = null;
                        CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGridFiltroUtente, "CodUtente", CoreStatics.GC_TUTTI);
                    }
                    if (this.ucEasyTreeViewFiltroTipo.Nodes.Count > 0)
                    {
                        this.ucEasyTreeViewFiltroTipo.EventManager.SetEnabled(Infragistics.Win.UltraWinTree.TreeEventIds.AfterCheck, false);
                        foreach (Infragistics.Win.UltraWinTree.UltraTreeNode oNode in this.ucEasyTreeViewFiltroTipo.Nodes[CoreStatics.GC_TUTTI].Nodes)
                        {
                            if (oNode.Override.NodeStyle == Infragistics.Win.UltraWinTree.NodeStyle.CheckBox)
                            {
                                oNode.CheckedState = CheckState.Checked;
                            }
                        }
                        this.ucEasyTreeViewFiltroTipo.EventManager.SetEnabled(Infragistics.Win.UltraWinTree.TreeEventIds.AfterCheck, true);
                    }
                    if (this.ucEasyGridFiltroStato.Rows.Count > 0)
                    {
                        this.ucEasyGridFiltroStato.Selected.Rows.Clear();
                        this.ucEasyGridFiltroStato.ActiveRow = null;
                        CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGridFiltroStato, this.ucEasyGridFiltroStato.DisplayLayout.Bands[0].Columns[0].Key, CoreStatics.GC_TUTTI);
                    }
                }
                catch (Exception)
                {
                }
            }


        }

        private void CaricaDati(bool ricaricatuttosenzafiltri, bool filtrididetault)
        {

            try
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);
                CoreStatics.SetContesto(EnumEntita.DCL, null);

                DataSet dsDE = null; DataTable dtGriglia = null;

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDEpisodio", CoreStatics.CoreApplication.Episodio.ID);
                if (ricaricatuttosenzafiltri)
                    op.Parametro.Add("DatiEstesi", "1");
                else
                    op.Parametro.Add("DatiEstesi", "0");

                bool bFiltro = false;
                if (!ricaricatuttosenzafiltri)
                {
                    if (this.uchkMedico.Checked && !this.uchkInfermieristico.Checked)
                    {
                        op.Parametro.Add("CodTipoDiario", "M");
                        bFiltro = true;
                    }
                    else if (!this.uchkMedico.Checked && this.uchkInfermieristico.Checked)
                    {
                        op.Parametro.Add("CodTipoDiario", "I");
                        bFiltro = true;
                    }

                    if (this.udteFiltroDA.Value != null)
                    {
                        op.Parametro.Add("DataInizio", CoreStatics.getDateTime((DateTime)this.udteFiltroDA.Value));
                        bFiltro = true;
                    }
                    if (this.udteFiltroA.Value != null)
                    {
                        op.Parametro.Add("DataFine", CoreStatics.getDateTime((DateTime)this.udteFiltroA.Value));
                        bFiltro = true;
                    }

                    if (this.ucEasyGridFiltroStato.ActiveRow != null && this.ucEasyGridFiltroStato.ActiveRow.Cells["Codice"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                    {
                        op.Parametro.Add("CodStatoDiario", this.ucEasyGridFiltroStato.ActiveRow.Cells["Codice"].Text);
                        bFiltro = true;
                    }

                    Dictionary<string, string> listatipo = new Dictionary<string, string>();
                    foreach (Infragistics.Win.UltraWinTree.UltraTreeNode oNode in this.ucEasyTreeViewFiltroTipo.Nodes[CoreStatics.GC_TUTTI].Nodes)
                    {
                        if (oNode.Override.NodeStyle == Infragistics.Win.UltraWinTree.NodeStyle.CheckBox && oNode.CheckedState == CheckState.Checked)
                        {
                            listatipo.Add(oNode.Key, oNode.Text);
                        }
                        else
                        {
                            bFiltro = true;
                        }
                    }
                    string[] codtipo = listatipo.Keys.ToArray();
                    op.ParametroRipetibile.Add("CodTipoVoceDiario", codtipo);

                    if (this.ucEasyGridFiltroUtente.ActiveRow != null && this.ucEasyGridFiltroUtente.ActiveRow.Cells["CodUtente"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                    {
                        op.Parametro.Add("CodUtente", this.ucEasyGridFiltroUtente.ActiveRow.Cells["CodUtente"].Text);
                        bFiltro = true;
                    }

                    if (this.utxtFiltroContenuto.Text.Trim() != "")
                    {
                        op.Parametro.Add("Descrizione", this.utxtFiltroContenuto.Text);
                        bFiltro = true;
                    }

                }
                else if (filtrididetault)
                {
                    this.drFiltro.Value = ucEasyDateRange.C_RNG_30G;
                    bFiltro = true;
                }
                this.uchkFiltro.Checked = bFiltro;

                op.TimeStamp.CodEntita = EnumEntita.DCL.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                dsDE = Database.GetDatasetStoredProc("MSP_SelMovDiarioClinico", spcoll);
                dtGriglia = dsDE.Tables[0];


                if (ricaricatuttosenzafiltri && filtrididetault)
                {
                    if (this.udteFiltroDA.Value != null)
                    {
                        op.Parametro.Add("DataInizio", CoreStatics.getDateTime((DateTime)this.udteFiltroDA.Value));
                        bFiltro = true;
                    }
                    if (this.udteFiltroA.Value != null)
                    {
                        op.Parametro.Add("DataFine", CoreStatics.getDateTime((DateTime)this.udteFiltroA.Value));
                        bFiltro = true;
                    }
                    op.Parametro["DatiEstesi"] = "0";
                    spcoll = new SqlParameterExt[1];
                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                    dtGriglia = Database.GetDataTableStoredProc("MSP_SelMovDiarioClinico", spcoll);
                }

                DataTable dtEdit = dtGriglia.Copy();
                DataColumn colsp = new DataColumn(CoreStatics.C_COL_SPAZIO, typeof(string));
                colsp.AllowDBNull = true;
                colsp.DefaultValue = "";
                colsp.Unique = false;
                dtEdit.Columns.Add(colsp);

                foreach (DataColumn dcCol in dtEdit.Columns)
                {
                    if (dcCol.ColumnName.ToUpper().IndexOf("PERMESSO") == 0
                     || dcCol.ColumnName.ToUpper().IndexOf("ICON") >= 0
                     || dcCol.ColumnName.ToUpper().IndexOf("ANTEPRIMARTF") == 0) dcCol.ReadOnly = false;
                }

                if (ricaricatuttosenzafiltri)
                {

                    if (_rtfRender == enumRTFRender.resizeRow || _rtfRender == enumRTFRender.resizeRowAndPopup)
                    {
                        this.ucEasyGrid.ColonnaRTFResize = "AnteprimaRTF";
                        this.ucEasyGrid.ColonnaRTFControlloContenuto = true;
                        int iFattore = 20;
                        if (UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.FontPredefinitoRTF) != "")
                        {
                            try
                            {
                                Graphics g = this.CreateGraphics();
                                iFattore = CoreStatics.PointToPixel(DrawingProcs.getFontFromString(UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.FontPredefinitoRTF)).SizeInPoints, g.DpiY);
                                g.Dispose();
                                g = null;
                            }
                            catch (Exception)
                            {
                                iFattore = 20;
                            }
                        }
                        this.ucEasyGrid.FattoreRidimensionamentoRTF = iFattore;
                    }
                }

                this.ucEasyGrid.DataSource = null;
                this.ucEasyGrid.DataSource = dtEdit;
                this.ucEasyGrid.Refresh();

                SetIconaValidaTutti();

                if (ricaricatuttosenzafiltri)
                {
                    CaricaDatiEstesi(!ricaricatuttosenzafiltri, ref dsDE);
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaDati()", this.Name);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
            }
        }

        private void CaricaDatiEstesi(bool applicafiltri)
        {
            DataSet ds = null;
            CaricaDatiEstesi(applicafiltri, ref ds);
        }
        private void CaricaDatiEstesi(bool applicafiltri, ref DataSet datasetmovimenti)
        {
            try
            {
                if (datasetmovimenti == null)
                {
                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("IDEpisodio", CoreStatics.CoreApplication.Episodio.ID);
                    op.Parametro.Add("DatiEstesi", "1");

                    op.TimeStamp.CodEntita = EnumEntita.DCL.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                    SqlParameterExt[] spcoll = new SqlParameterExt[1];

                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    datasetmovimenti = Database.GetDatasetStoredProc("MSP_SelMovDiarioClinico", spcoll);
                }

                if (applicafiltri)
                {
                    string codutenteselezionato = "";


                    this.ucEasyGridFiltroUtente.DisplayLayout.Bands[0].Columns.ClearUnbound();
                    if (this.ucEasyGridFiltroUtente.ActiveRow != null && this.ucEasyGridFiltroUtente.ActiveRow.Cells["CodUtente"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                        codutenteselezionato = this.ucEasyGridFiltroUtente.ActiveRow.Cells["CodUtente"].Text;
                    this.ucEasyGridFiltroUtente.DataSource = CoreStatics.AggiungiTuttiDataTable(datasetmovimenti.Tables[2], true);
                    this.ucEasyGridFiltroUtente.Refresh();
                    if (codutenteselezionato != "") CoreStatics.SelezionaRigaInGriglia(ref ucEasyGridFiltroUtente, "CodUtente", codutenteselezionato);
                }
                else
                {

                    this.uchkInfermieristico.Checked = true;
                    this.uchkMedico.Checked = true;

                    this.ucEasyTreeViewFiltroTipo.Nodes.Clear();
                    Infragistics.Win.UltraWinTree.UltraTreeNode oNodeRoot = new Infragistics.Win.UltraWinTree.UltraTreeNode(CoreStatics.GC_TUTTI, "Tipo");
                    oNodeRoot.Override.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
                    oNodeRoot.CheckedState = CheckState.Checked;
                    foreach (DataRow oDr in datasetmovimenti.Tables[1].Rows)
                    {
                        Infragistics.Win.UltraWinTree.UltraTreeNode oNode = new Infragistics.Win.UltraWinTree.UltraTreeNode(oDr["CodTipo"].ToString(), oDr["DescTipo"].ToString());
                        oNode.Override.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
                        oNode.CheckedState = CheckState.Checked;
                        oNodeRoot.Nodes.Add(oNode);
                    }
                    this.ucEasyTreeViewFiltroTipo.Nodes.Add(oNodeRoot);
                    this.ucEasyTreeViewFiltroTipo.ExpandAll();

                    this.ucEasyGridFiltroUtente.DataSource = CoreStatics.AggiungiTuttiDataTable(datasetmovimenti.Tables[2], true);
                    this.ucEasyGridFiltroUtente.DisplayLayout.Bands[0].Columns["DescrUtente"].Header.Caption = "Utente";
                    this.ucEasyGridFiltroUtente.Refresh();


                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("DatiEstesi", "0");

                    SqlParameterExt[] spcoll = new SqlParameterExt[1];

                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    DataSet dsSt = Database.GetDatasetStoredProc("MSP_SelStatoDiario", spcoll);

                    this.ucEasyGridFiltroStato.DataSource = CoreStatics.AggiungiTuttiDataTable(dsSt.Tables[0], false);
                    this.ucEasyGridFiltroStato.DisplayLayout.Bands[0].Columns["Descrizione"].Header.Caption = "Stato";
                    this.ucEasyGridFiltroStato.Refresh();

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaDatiEstesi()", this.Name);
            }
        }

        private void InitializeRow(UltraGridRow eRow)
        {
            try
            {

                if (eRow.Cells.Exists("Icona") == true && eRow.Cells["IDIcona"].Value.ToString() != "")
                {
                    if (oIcone.ContainsKey(Convert.ToInt32(eRow.Cells["IDIcona"].Value)) == false)
                    {
                        oIcone.Add(Convert.ToInt32(eRow.Cells["IDIcona"].Value), CoreStatics.GetImageForGrid(Convert.ToInt32(eRow.Cells["IDIcona"].Value), 256));
                    }
                    eRow.Cells["Icona"].Value = oIcone[Convert.ToInt32(eRow.Cells["IDIcona"].Value)];
                    eRow.Update();
                }


                if (eRow.Cells["PermessoCancella"].Text == "0" && eRow.Cells["PermessoAnnulla"].Text == "0")
                {
                    eRow.Cells[CoreStatics.C_COL_BTN_DEL].CellDisplayStyle = CellDisplayStyle.PlainText;
                    eRow.Cells[CoreStatics.C_COL_BTN_DEL].Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                    eRow.Cells[CoreStatics.C_COL_BTN_DEL].Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.False;
                    eRow.Cells[CoreStatics.C_COL_BTN_DEL].Appearance.TextHAlign = Infragistics.Win.HAlign.Left;
                    eRow.Cells[CoreStatics.C_COL_BTN_DEL].Appearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                    eRow.Cells[CoreStatics.C_COL_BTN_DEL].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Edit;
                    string sStato = "";
                    if (eRow.Cells["CodStato"].Text == "AN")
                    {
                        sStato = eRow.Cells["DescrStato"].Text;
                        if (eRow.Cells["DataAnnullamento"].Value != System.DBNull.Value)
                        {
                            sStato += @":" + Environment.NewLine + ((DateTime)eRow.Cells["DataAnnullamento"].Value).ToString("dd/MM/yyyy") + Environment.NewLine;
                            sStato += ((DateTime)eRow.Cells["DataAnnullamento"].Value).ToString("HH:mm");
                        }
                    }
                    eRow.Cells[CoreStatics.C_COL_BTN_DEL].Value = sStato;
                }

                if (eRow.Cells["PermessoValida"].Text == "0" && eRow.Cells.Exists(CoreStatics.C_COL_BTN_VALID) == true)
                {
                    eRow.Cells[CoreStatics.C_COL_BTN_VALID].CellDisplayStyle = CellDisplayStyle.PlainText;
                    eRow.Cells[CoreStatics.C_COL_BTN_VALID].Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                    eRow.Cells[CoreStatics.C_COL_BTN_VALID].Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.False;
                    eRow.Cells[CoreStatics.C_COL_BTN_VALID].Appearance.TextHAlign = Infragistics.Win.HAlign.Left;
                    eRow.Cells[CoreStatics.C_COL_BTN_VALID].Appearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                    eRow.Cells[CoreStatics.C_COL_BTN_VALID].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Edit;
                    string sStato = "";
                    if (eRow.Cells["CodStato"].Text == "VA")
                    {
                        sStato = eRow.Cells["DescrStato"].Text;
                        if (eRow.Cells["DataValidazione"].Value != System.DBNull.Value)
                        {
                            sStato += @":" + Environment.NewLine + ((DateTime)eRow.Cells["DataValidazione"].Value).ToString("dd/MM/yyyy") + Environment.NewLine;
                            sStato += ((DateTime)eRow.Cells["DataValidazione"].Value).ToString("HH:mm");
                        }
                    }
                    eRow.Cells[CoreStatics.C_COL_BTN_VALID].Value = sStato;
                }
                else
                {
                    if (eRow.Cells.Exists("PermessoUAFirma") && eRow.Cells["PermessoUAFirma"].Text.Trim() == "1")
                        eRow.Cells[CoreStatics.C_COL_BTN_VALID].ButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_TESSERAFIRMA_32);
                    else
                        eRow.Cells[CoreStatics.C_COL_BTN_VALID].ButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_FIRMA_32);
                }

                if (eRow.Cells["PermessoModifica"].Text == "0" && eRow.Cells["PermessoCopia"].Text == "0" && eRow.Cells.Exists(CoreStatics.C_COL_BTN_EDIT) == true)
                {
                    eRow.Cells[CoreStatics.C_COL_BTN_EDIT].CellDisplayStyle = CellDisplayStyle.PlainText;
                    eRow.Cells[CoreStatics.C_COL_BTN_EDIT].Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                    eRow.Cells[CoreStatics.C_COL_BTN_EDIT].Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.False;
                    eRow.Cells[CoreStatics.C_COL_BTN_EDIT].Appearance.TextHAlign = Infragistics.Win.HAlign.Left;
                    eRow.Cells[CoreStatics.C_COL_BTN_EDIT].Appearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                    eRow.Cells[CoreStatics.C_COL_BTN_EDIT].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Edit;
                    string sStato = "";

                    eRow.Cells[CoreStatics.C_COL_BTN_EDIT].Value = sStato;
                }

                if (CoreStatics.CoreApplication.Cartella != null && CoreStatics.CoreApplication.Cartella.CartellaChiusa == true)
                {
                    eRow.Cells[CoreStatics.C_COL_BTN_DEL].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                    eRow.Cells[CoreStatics.C_COL_BTN_VALID].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                    eRow.Cells[CoreStatics.C_COL_BTN_EDIT].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                }

                if (bInviaConsegna)
                {
                    if (eRow.Cells["CodStato"].Text == "VA")
                    {

                    }
                    else
                    {
                        eRow.Cells[CoreStatics.C_COL_BTN_CONSEGNA].CellDisplayStyle = CellDisplayStyle.PlainText;
                        eRow.Cells[CoreStatics.C_COL_BTN_CONSEGNA].Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                        eRow.Cells[CoreStatics.C_COL_BTN_CONSEGNA].Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.False;
                        eRow.Cells[CoreStatics.C_COL_BTN_CONSEGNA].Appearance.TextHAlign = Infragistics.Win.HAlign.Left;
                        eRow.Cells[CoreStatics.C_COL_BTN_CONSEGNA].Appearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                        eRow.Cells[CoreStatics.C_COL_BTN_CONSEGNA].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Edit;
                        eRow.Cells[CoreStatics.C_COL_BTN_CONSEGNA].Value = "";
                    }
                }

            }
            catch (Exception)
            {
            }
        }

        private void SetIconaValidaTutti()
        {
            try
            {
                bool bFirmaDigitale = false;
                for (int r = 0; r < this.ucEasyGrid.Rows.Count; r++)
                {
                    UltraGridRow grow = this.ucEasyGrid.Rows[r];
                    if (grow.Cells["PermessoValida"].Text == "1" && grow.Cells.Exists("PermessoUAFirma") && grow.Cells["PermessoUAFirma"].Text == "1")
                    {
                        bFirmaDigitale = true;
                        r = this.ucEasyGrid.Rows.Count + 1;
                    }
                }
                if (bFirmaDigitale)
                    this.ubValida.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_TESSERAFIRMATUTTI_256);
                else
                    this.ubValida.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FIRMAMULTIPLA_256);
            }
            catch
            {
                this.ubValida.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FIRMAMULTIPLA_256);
            }
        }

        private void setNavigazione(bool enable)
        {
            try
            {
                CoreStatics.SetNavigazione(enable);

                this.ucEasyGrid.Enabled = enable;
                this.ubAdd.Enabled = enable;
                this.uchkFiltro.Enabled = enable;
                this.ubValida.Enabled = enable;

                this.ucEasyTableLayoutPanelFiltro1.Enabled = enable;

            }
            catch
            {
                CoreStatics.SetNavigazione(true);
            }
        }

        #endregion

        #region EVENTI

        private void ubAdd_Click(object sender, EventArgs e)
        {

            CoreStatics.CoreApplication.MovDiarioClinicoDaAnnullare = null;

            MovDiarioClinico movdc = new MovDiarioClinico(CoreStatics.CoreApplication.Trasferimento.CodUA, CoreStatics.CoreApplication.Paziente.ID, CoreStatics.CoreApplication.Episodio.ID, CoreStatics.CoreApplication.Trasferimento.ID, CoreStatics.CoreApplication.Ambiente);
            movdc.Azione = EnumAzioni.INS;
            movdc.DataEvento = DateTime.Now;
            movdc.CodStatoDiario = "VA"; movdc.PermessoUAFirma = 0;
            if (DBUtils.ModuloUAAbilitato(CoreStatics.CoreApplication.Trasferimento.CodUA, EnumUAModuli.FirmaD_Diario)) movdc.PermessoUAFirma = 1;
            CoreStatics.CoreApplication.MovDiarioClinicoSelezionato = movdc;

            if (CoreStatics.CheckSelezionaTipoVoceDiario() == true)
            {
                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingVoceDiDiario, false) == DialogResult.OK)
                {
                    CaricaDati(true, false);
                    if (CoreStatics.CoreApplication.MovDiarioClinicoSelezionato != null) CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.IDMovDiario);
                }
            }
            else
            {
                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezionaTipoVoceDiario, false) == DialogResult.OK)
                {
                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingVoceDiDiario, false) == DialogResult.OK)
                    {
                        CaricaDati(true, false);
                        if (CoreStatics.CoreApplication.MovDiarioClinicoSelezionato != null) CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.IDMovDiario);
                    }
                }
            }

            CoreStatics.SetNavigazione(true);

        }

        private void ubValida_Click(object sender, EventArgs e)
        {
            try
            {

                CoreStatics.CoreApplication.ListaIDMovDiarioClinicoSelezionati.Clear();
                if (this.ucEasyGrid.Rows.Count > 0)
                {
                    for (int iRow = 0; iRow < this.ucEasyGrid.Rows.Count; iRow++)
                    {
                        UltraGridRow ugrRow = this.ucEasyGrid.Rows[iRow];
                        if (ugrRow.IsDataRow && !ugrRow.IsDeleted && ugrRow.Cells["PermessoValida"].Text == "1")
                        {
                            CoreStatics.CoreApplication.ListaIDMovDiarioClinicoSelezionati.Add(ugrRow.Cells["ID"].Text);
                        }
                    }
                }


                if (CoreStatics.CoreApplication.ListaIDMovDiarioClinicoSelezionati.Count <= 0)
                {
                    easyStatics.EasyMessageBox("Non ci sono voci da Validare!", "Validazione Diario Clinico", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.ValidazioneVociDiDiario) == DialogResult.OK)
                        CaricaDati(false, false);
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubValida_Click", this.Name);
            }
        }

        private void ultraDockManager_InitializePane(object sender, Infragistics.Win.UltraWinDock.InitializePaneEventArgs e)
        {
            e.Pane.Settings.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large);
            e.Pane.Settings.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FILTRO_32);

            int filtroWidth = 40 * (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large);
            this.ultraDockManager.ControlPanes[0].FlyoutSize = new Size(filtroWidth, this.ultraDockManager.ControlPanes[0].FlyoutSize.Height);
            this.ultraDockManager.ControlPanes[0].Size = new Size(filtroWidth, this.ultraDockManager.ControlPanes[0].Size.Height);
            this.ultraDockManager.DockAreas[0].Size = new Size(filtroWidth, this.ultraDockManager.DockAreas[0].Size.Height);
            this.pnlFiltro.Width = filtroWidth;

        }

        private void ucEasyGrid_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {
                int refIcoWidth = (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XLarge) * 3;

                Graphics g = this.CreateGraphics();
                int refBtnWidth = Convert.ToInt32(CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.ucEasyGrid.DataRowFontRelativeDimension), g.DpiY) * 4.3);
                g.Dispose();
                g = null;
                e.Layout.Bands[0].HeaderVisible = false;
                e.Layout.Bands[0].ColHeadersVisible = false;
                e.Layout.Bands[0].RowLayoutStyle = RowLayoutStyle.GroupLayout;
                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {
                    try
                    {
                        oCol.SortIndicator = SortIndicator.Disabled;
                        switch (oCol.Key)
                        {
                            case "Icona":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                                oCol.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellAppearance.ImageVAlign = Infragistics.Win.VAlign.Top;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                try
                                {
                                    oCol.LockedWidth = true;
                                    oCol.MaxWidth = refIcoWidth;
                                    oCol.Width = oCol.MaxWidth;
                                    oCol.MinWidth = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 4;

                                break;

                            case "DataEvento":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Red;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.VertScrollBar = false;
                                oCol.Format = "dd/MM/yyyy HH:mm";
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refIcoWidth * 2.5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }


                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;


                                break;

                            case "DescrUtente":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Red;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refIcoWidth * 2.5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 1;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;


                                break;

                            case "DataInserimento":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.FontData.Italic = Infragistics.Win.DefaultableBoolean.True;
                                oCol.CellAppearance.ForeColor = Color.Gray;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.Format = "(dd/MM/yyyy HH:mm)";
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refIcoWidth * 2.5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }


                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 2;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;


                                break;

                            case CoreStatics.C_COL_SPAZIO:
                                oCol.Hidden = false;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XXSmall);
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refIcoWidth * 2.5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.WeightY = 1;
                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 3;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "AnteprimaRTF":
                                oCol.Hidden = false;

                                RichTextEditor a = new RichTextEditor();
                                a.ScrollBars = RichTextBoxScrollBars.Both;
                                oCol.Editor = a;
                                switch (_rtfRender)
                                {
                                    case enumRTFRender.showScrollBars:
                                        oCol.CellClickAction = CellClickAction.EditAndSelectText;
                                        break;
                                    case enumRTFRender.resizeRow:
                                    case enumRTFRender.resizeRowAndPopup:
                                    case enumRTFRender.showPopup:
                                    default:
                                        oCol.CellClickAction = CellClickAction.CellSelect;
                                        break;
                                }
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.VertScrollBar = true;
                                try
                                {
                                    oCol.MaxWidth = this.ucEasyGrid.Width - Convert.ToInt32(refIcoWidth * 3.5) - Convert.ToInt32(refBtnWidth * (bInviaConsegna ? 4.6 : 3.6)) - 30;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 2;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 4;

                                break;

                            default:
                                oCol.Hidden = true;
                                break;
                        }

                    }
                    catch (Exception)
                    {
                    }
                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_EDIT))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_EDIT);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Properties.Resources.Modifica_32;


                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = refBtnWidth;
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;


                    colEdit.RowLayoutColumnInfo.OriginX = 3;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_EDIT + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_EDIT + @"_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = Convert.ToInt32(refBtnWidth * 0.25);
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;
                    colEdit.RowLayoutColumnInfo.OriginX = 4;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_VALID))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_VALID);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_FIRMA_32);

                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = refBtnWidth;
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;


                    colEdit.RowLayoutColumnInfo.OriginX = 5;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_DEL + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_DEL + @"_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = Convert.ToInt32(refBtnWidth * 0.25);
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;
                    colEdit.RowLayoutColumnInfo.OriginX = 6;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_DEL))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_DEL);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Properties.Resources.Cancella_32;


                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = refBtnWidth;
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;


                    colEdit.RowLayoutColumnInfo.OriginX = 7;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }
                if (bInviaConsegna)
                {
                    if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_CONSEGNA + @"_SP"))
                    {
                        UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_CONSEGNA + @"_SP");
                        colEdit.Hidden = false;
                        colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                        colEdit.CellActivation = Activation.Disabled;
                        colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                        try
                        {
                            colEdit.MinWidth = Convert.ToInt32(refBtnWidth * 0.25);
                            colEdit.MaxWidth = colEdit.MinWidth;
                            colEdit.Width = colEdit.MaxWidth;
                        }
                        catch (Exception)
                        {
                        }
                        colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                        colEdit.LockedWidth = true;
                        colEdit.RowLayoutColumnInfo.OriginX = 8;
                        colEdit.RowLayoutColumnInfo.OriginY = 0;
                        colEdit.RowLayoutColumnInfo.SpanX = 1;
                        colEdit.RowLayoutColumnInfo.SpanY = 3;
                    }
                    if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_CONSEGNA))
                    {
                        UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_CONSEGNA);
                        colEdit.Hidden = false;

                        colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                        colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                        colEdit.CellActivation = Activation.AllowEdit;
                        colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                        colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                        colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                        colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                        colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_CONSEGNEPAZIENTE_32);


                        colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                        try
                        {
                            colEdit.MinWidth = refBtnWidth;
                            colEdit.MaxWidth = colEdit.MinWidth;
                            colEdit.Width = colEdit.MaxWidth;
                        }
                        catch (Exception)
                        {
                        }
                        colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                        colEdit.LockedWidth = true;


                        colEdit.RowLayoutColumnInfo.OriginX = 9;
                        colEdit.RowLayoutColumnInfo.OriginY = 0;
                        colEdit.RowLayoutColumnInfo.SpanX = 1;
                        colEdit.RowLayoutColumnInfo.SpanY = 3;
                    }
                }
                if (!e.Layout.Bands[0].Columns.Exists(@"COLFINE_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(@"COLFINE_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = Convert.ToInt32(refBtnWidth * 0.1);
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;
                    colEdit.RowLayoutColumnInfo.OriginX = 10;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }

            }
            catch (Exception ex)
            {
                string aa = ex.Message;
            }
        }

        private void ucEasyGrid_AfterRowActivate(object sender, EventArgs e)
        {
            CoreStatics.CoreApplication.IDDiarioClinicoSelezionato = "";
            CoreStatics.SetContesto(EnumEntita.DCL, this.ucEasyGrid.ActiveRow);
            CoreStatics.CoreApplication.IDDiarioClinicoSelezionato = this.ucEasyGrid.ActiveRow.Cells["ID"].Text;
        }

        private void ucEasyGrid_ClickCellButton(object sender, CellEventArgs e)
        {
            try
            {
                CoreStatics.CoreApplication.MovDiarioClinicoDaAnnullare = null;
                switch (e.Cell.Column.Key)
                {
                    case CoreStatics.C_COL_BTN_EDIT:
                        if (e.Cell.Row.Cells["PermessoModifica"].Text == "1")
                        {

                            MovDiarioClinico movdc = new MovDiarioClinico(e.Cell.Row.Cells["ID"].Text, EnumAzioni.MOD, CoreStatics.CoreApplication.Ambiente);
                            CoreStatics.CoreApplication.MovDiarioClinicoSelezionato = movdc;
                            if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingVoceDiDiario) == DialogResult.OK)
                            {
                                CaricaDati(true, false);

                                if (CoreStatics.CoreApplication.MovDiarioClinicoSelezionato != null) CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.IDMovDiario);
                            }

                        }
                        else
                        {

                            string idOriginale = e.Cell.Row.Cells["ID"].Text;

                            MovDiarioClinico movdcorig = new MovDiarioClinico(idOriginale, CoreStatics.CoreApplication.Ambiente);
                            MovDiarioClinico movdccopia = movdcorig.Copia();
                            if (movdccopia != null)
                            {
                                if (easyStatics.EasyMessageBox(@"Sei sicuro di voler confermare la modifica della voce di diario validata?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                                {

                                    CoreStatics.CoreApplication.MovDiarioClinicoSelezionato = movdccopia;
                                    if (movdcorig.PermessoUAFirma == 1) CoreStatics.CoreApplication.MovDiarioClinicoDaAnnullare = movdcorig; if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingVoceDiDiario) == DialogResult.OK)
                                    {


                                        if (movdcorig.PermessoUAFirma != 1) movdcorig.Annulla();

                                        CaricaDati(true, false);

                                        if (CoreStatics.CoreApplication.MovDiarioClinicoSelezionato != null) CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.IDMovDiario);
                                    }

                                }
                            }

                        }
                        break;

                    case CoreStatics.C_COL_BTN_VALID:
                        {
                            bool bContinua = true;
                            frmSmartCardProgress frmSC = null;
                            MovDiarioClinico movdc = null;
                            try
                            {
                                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

                                if (e.Cell.Row.Cells["PermessoUAFirma"].Text.Trim() == "1")
                                {
                                    bContinua = false;


                                    setNavigazione(false);
                                    frmSC = new frmSmartCardProgress();
                                    frmSC.InizializzaEMostra(0, 4, this);
                                    frmSC.SetCursore(enum_app_cursors.WaitCursor);

                                    frmSC.SetStato(@"Validazione Diario " + e.Cell.Row.Cells["DescrTipoDiario"].Text + @" del " + Convert.ToDateTime(e.Cell.Row.Cells["DataInserimento"].Value).ToString("dd/MM/yyyy HH:mm"));


                                    try
                                    {
                                        frmSC.SetStato(@"Generazione Documento...");

                                        byte[] pdfContent = CoreStatics.GeneraPDFValidazioneDiario(e.Cell.Row.Cells["ID"].Text, true);

                                        if (pdfContent == null || pdfContent.Length <= 0)
                                        {
                                            frmSC.SetLog(@"Errore Generazione documento", true);
                                        }
                                        else
                                        {
                                            bContinua = frmSC.ProvaAFirmare(ref pdfContent, EnumTipoDocumentoFirmato.DCLFM01, "Firma Digitale...", EnumEntita.DCL, e.Cell.Row.Cells["ID"].Text);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        if (frmSC != null) frmSC.SetLog(@"Errore Generazione documento: " + ex.Message, true);
                                        bContinua = false;
                                    }

                                }
                                if (bContinua)
                                {
                                    movdc = new MovDiarioClinico(e.Cell.Row.Cells["ID"].Text, CoreStatics.CoreApplication.Ambiente);
                                    Risposta oRispostaElaboraPrima = new Risposta();
                                    oRispostaElaboraPrima = PluginClientStatics.PluginClient(EnumPluginClient.DCL_VALIDA_PRIMA.ToString(), new object[1] { movdc }, CommonStatics.UAPadri(movdc.CodUA, CoreStatics.CoreApplication.Ambiente));
                                    if (oRispostaElaboraPrima.Successo)
                                    {
                                        bContinua = movdc.Valida();
                                    }
                                    else
                                    {
                                        bContinua = false;
                                        easyStatics.EasyMessageBox(oRispostaElaboraPrima.ex.Message, "Validazione Diario Clinico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                CoreStatics.ExGest(ref ex, "ucEasyGrid_ClickCellButton", this.Name);
                            }
                            finally
                            {
                                if (frmSC != null)
                                {
                                    frmSC.Close();
                                    frmSC.Dispose();
                                }

                                setNavigazione(true);

                                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
                            }

                            if (bContinua && movdc != null)
                            {
                                e.Cell.Row.Cells["CodStato"].Value = movdc.CodStatoDiario;
                                e.Cell.Row.Cells["DescrStato"].Value = movdc.DescrStatoDiario;
                                e.Cell.Row.Cells["DataValidazione"].Value = movdc.DataValidazione;
                                e.Cell.Row.Cells["PermessoValida"].Value = movdc.PermessoValida;
                                e.Cell.Row.Cells["PermessoCopia"].Value = movdc.PermessoCopia;
                                e.Cell.Row.Cells["PermessoModifica"].Value = movdc.PermessoModifica;
                                e.Cell.Row.Cells["PermessoAnnulla"].Value = movdc.PermessoAnnulla;
                                e.Cell.Row.Cells["PermessoCancella"].Value = movdc.PermessoCancella;
                                e.Cell.Row.Cells["PermessoUAFirma"].Value = movdc.PermessoUAFirma;
                                if (movdc.Icona == null)
                                    e.Cell.Row.Cells["Icona"].Value = System.DBNull.Value;
                                else
                                    e.Cell.Row.Cells["Icona"].Value = movdc.Icona;
                                e.Cell.Row.Cells["IDIcona"].Value = movdc.IDIcona;

                                InitializeRow(e.Cell.Row);

                                SetIconaValidaTutti();
                            }


                        }
                        break;


                    case CoreStatics.C_COL_BTN_DEL:
                        if (e.Cell.Row.Cells["PermessoCancella"].Text == "1")
                        {
                            if (easyStatics.EasyMessageBox("Sei sicuro di voler CANCELLARE" + Environment.NewLine + "la voce corrente?", "Cancellazione Diario Clinico", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                MovDiarioClinico movdc = new MovDiarioClinico(e.Cell.Row.Cells["ID"].Text, CoreStatics.CoreApplication.Ambiente);
                                if (movdc.Cancella())
                                {

                                    CaricaDati(true, false);
                                }
                            }
                        }
                        else if (e.Cell.Row.Cells["PermessoAnnulla"].Text == "1")
                        {
                            if (easyStatics.EasyMessageBox("Sei sicuro di voler ANNULLARE" + Environment.NewLine + "la voce corrente?", "Annullamento Diario Clinico", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                MovDiarioClinico movdc = new MovDiarioClinico(e.Cell.Row.Cells["ID"].Text, CoreStatics.CoreApplication.Ambiente);
                                if (movdc.Annulla())
                                {


                                    if (e.Cell.Row.Cells["PermessoUAFirma"].Text.Trim() == "1")
                                    {
                                        bool bContinua = true;
                                        frmSmartCardProgress frmSC = null;

                                        try
                                        {

                                            setNavigazione(false);
                                            frmSC = new frmSmartCardProgress();
                                            frmSC.InizializzaEMostra(0, 3, this);
                                            frmSC.SetCursore(enum_app_cursors.WaitCursor);

                                            try
                                            {
                                                frmSC.SetStato(@"Generazione Documento Annullato...");

                                                byte[] pdfContent = CoreStatics.GeneraPDFValidazioneDiario(e.Cell.Row.Cells["ID"].Text, true);

                                                if (pdfContent == null || pdfContent.Length <= 0)
                                                {
                                                    frmSC.SetLog(@"Errore Generazione Documento Annullato", true);
                                                }
                                                else
                                                {
                                                    bContinua = frmSC.ProvaAFirmare(ref pdfContent, EnumTipoDocumentoFirmato.DCLFM01, "Firma Digitale Documento Annullato...", EnumEntita.DCL, e.Cell.Row.Cells["ID"].Text);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                if (frmSC != null) frmSC.SetLog(@"Errore Generazione Documento Annullato: " + ex.Message, true);
                                                bContinua = false;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            CoreStatics.ExGest(ref ex, "ucEasyGrid_ClickCellButton", this.Name);
                                        }
                                        finally
                                        {
                                            if (frmSC != null)
                                            {
                                                frmSC.Close();
                                                frmSC.Dispose();
                                            }

                                            setNavigazione(true);

                                            CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
                                        }


                                        if (!bContinua)
                                        {
                                            movdc.CodStatoDiario = "VA";
                                            movdc.DataAnnullamento = DateTime.MinValue;
                                            movdc.Azione = EnumAzioni.MOD;
                                            movdc.Salva();


                                        }
                                    }

                                    e.Cell.Row.Cells["CodStato"].Value = movdc.CodStatoDiario;
                                    e.Cell.Row.Cells["DescrStato"].Value = movdc.DescrStatoDiario;
                                    e.Cell.Row.Cells["DataAnnullamento"].Value = movdc.DataValidazione;
                                    e.Cell.Row.Cells["PermessoValida"].Value = movdc.PermessoValida;
                                    e.Cell.Row.Cells["PermessoCopia"].Value = movdc.PermessoCopia;
                                    e.Cell.Row.Cells["PermessoModifica"].Value = movdc.PermessoModifica;
                                    e.Cell.Row.Cells["PermessoAnnulla"].Value = movdc.PermessoAnnulla;
                                    e.Cell.Row.Cells["PermessoCancella"].Value = movdc.PermessoCancella;
                                    e.Cell.Row.Cells["PermessoUAFirma"].Value = movdc.PermessoUAFirma;
                                    if (movdc.Icona == null)
                                        e.Cell.Row.Cells["Icona"].Value = System.DBNull.Value;
                                    else
                                        e.Cell.Row.Cells["Icona"].Value = movdc.Icona;
                                    e.Cell.Row.Cells["IDIcona"].Value = movdc.IDIcona;
                                    e.Cell.Row.Cells["AnteprimaRTF"].Value = movdc.MovScheda.AnteprimaRTF;

                                    InitializeRow(e.Cell.Row);

                                    SetIconaValidaTutti();
                                }
                            }
                        }
                        break;

                    case CoreStatics.C_COL_BTN_CONSEGNA:
                        CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata = new MovConsegnaPaziente(CoreStatics.CoreApplication.Ambiente);
                        CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.IDEpisodio = CoreStatics.CoreApplication.Episodio.ID;
                        CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.AggiungiRuoloCreazione(CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                        if (CoreStatics.CheckSelezionaTipoConsegnaPaziente(CoreStatics.CoreApplication.Trasferimento.CodUA,
                                                                            CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice,
                                                                            new MovDiarioClinico(e.Cell.Row.Cells["ID"].Text, EnumAzioni.MOD, CoreStatics.CoreApplication.Ambiente)))
                        {
                            if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingConsegnaPaziente) == DialogResult.OK)
                            {
                                easyStatics.EasyMessageBox(@"Creazione consegna creata correttamente.", "DIRIO CLINICO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_ClickCellButton", this.Name);
            }
        }

        private void ucEasyGrid_ClickCell(object sender, ClickCellEventArgs e)
        {

            try
            {

                switch (e.Cell.Column.Key)
                {

                    case "AnteprimaRTF":
                        if (_rtfRender == enumRTFRender.showPopup || _rtfRender == enumRTFRender.resizeRowAndPopup)
                        {
                            CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainer);
                            _ucRichTextBox = CoreStatics.GetRichTextBoxForPopupControlContainer(e.Cell.Text);

                            Infragistics.Win.UIElement uie = e.Cell.GetUIElement();
                            Point oPoint = new Point(uie.Rect.Left, uie.Rect.Top);

                            this.UltraPopupControlContainer.Show(this.ucEasyGrid, oPoint);
                        }
                        break;

                    default:
                        break;

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_ClickCell", this.Name);
            }

        }

        private void ucEasyGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            InitializeRow(e.Row);
        }

        private void ucEasyTreeViewFiltroTipo_AfterCheck(object sender, Infragistics.Win.UltraWinTree.NodeEventArgs e)
        {

            try
            {

                if (e.TreeNode.Key == CoreStatics.GC_TUTTI)
                {
                    foreach (Infragistics.Win.UltraWinTree.UltraTreeNode oNode in this.ucEasyTreeViewFiltroTipo.Nodes[CoreStatics.GC_TUTTI].Nodes)
                    {
                        if (oNode.Override.NodeStyle == Infragistics.Win.UltraWinTree.NodeStyle.CheckBox)
                        {
                            oNode.CheckedState = e.TreeNode.CheckedState;
                        }
                    }
                }

            }
            catch (Exception)
            {

            }

        }

        private void uchkFiltro_Click(object sender, EventArgs e)
        {
            if (!this.uchkFiltro.Checked)
            {
                this.InizializzaFiltri();
                this.Aggiorna();
            }
            else
                this.uchkFiltro.Checked = !this.uchkFiltro.Checked;

        }

        private void drFiltro_ValueChanged(object sender, EventArgs e)
        {
            this.udteFiltroDA.Value = drFiltro.DataOraDa;
            this.udteFiltroA.Value = drFiltro.DataOraA;
        }

        private void ubApplicaFiltro_Click(object sender, EventArgs e)
        {
            if (this.ultraDockManager.FlyoutPane != null && !this.ultraDockManager.FlyoutPane.Pinned) this.ultraDockManager.FlyIn();
            CaricaDati(false, false);
            this.ucEasyGrid.Focus();
        }

        private void ucEasyGridFiltro_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Override.HeaderClickAction = HeaderClickAction.Select;
            foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
            {
                if (oCol.Key.ToUpper().IndexOf("DESC") < 0) oCol.Hidden = true;

            }
        }

        private void txtFiltroUtente_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                CoreStatics.SetGridWizardFilter(ref this.ucEasyGridFiltroUtente,
                                                this.ucEasyGridFiltroUtente.DisplayLayout.Bands[0].Columns[1].Key,
                                                this.txtFiltroUtente.Text);
            }
            catch (Exception)
            {
            }
        }

        private void ucDiarioClinicoPaziente_Enter(object sender, EventArgs e)
        {
            try
            {
                this.ucEasyGrid.PerformLayout();
            }
            catch (Exception)
            {
            }
        }

        private void utxtFiltroContenuto_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (this.ubApplicaFiltro.Enabled && e.KeyCode == Keys.Enter) ubApplicaFiltro_Click(this.ubApplicaFiltro, new EventArgs());
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region UltraPopupControlContainer

        private void UltraPopupControlContainer_Closed(object sender, EventArgs e)
        {
            _ucRichTextBox.RtfClick -= ucRichTextBox_Click;
        }

        private void UltraPopupControlContainer_Opened(object sender, EventArgs e)
        {
            _ucRichTextBox.RtfClick += ucRichTextBox_Click;
        }

        private void UltraPopupControlContainer_Opening(object sender, CancelEventArgs e)
        {
            Infragistics.Win.Misc.UltraPopupControlContainer popup = sender as Infragistics.Win.Misc.UltraPopupControlContainer;
            popup.PopupControl = _ucRichTextBox;
        }

        private void ucRichTextBox_Click(object sender, EventArgs e)
        {
        }

        #endregion

    }
}
