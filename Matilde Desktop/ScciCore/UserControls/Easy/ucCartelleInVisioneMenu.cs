using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnicodeSrl.Scci;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using Infragistics.Win.UltraWinGrid;
using System.Globalization;
using UnicodeSrl.ScciResource;

namespace UnicodeSrl.ScciCore
{
    public partial class ucCartelleInVisioneMenu : UserControl, Interfacce.IViewUserControlMiddle
    {
        public ucCartelleInVisioneMenu()
        {
            InitializeComponent();
        }

        #region Declare

        private bool _16_9 = true;

        #endregion

        #region Interface

        public void Aggiorna()
        {

            if (this.IsDisposed == false)
            {

                CoreStatics.GCCollectUltraGrid(ref this.UltraGridCartellePaziente);
                CoreStatics.GCCollectUltraGrid(ref this.UltraGridCartelleAmbulatoriali);

                this.CaricaGrigliaA();
                this.CaricaGrigliaP();

                this.setFocusDefault();

            }

        }

        public void Carica()
        {

            try
            {

                InizializzaControlli();
                InizializzaUltraGridLayout();
                InizializzaFiltri();

                this.CaricaGrigliaA();
                this.CaricaGrigliaP();
                this.setFocusDefault();

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

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Ferma", this.Name);
            }

        }

        #endregion

        #region Properties

        public UltraGridRow RigaPazienteSelezionato
        {
            get
            {

                if (this.UltraGridCartellePaziente.ActiveRow != null)
                {
                    return this.UltraGridCartellePaziente.ActiveRow;
                }
                else if (this.UltraGridCartelleAmbulatoriali.ActiveRow != null)
                {
                    return this.UltraGridCartelleAmbulatoriali.ActiveRow;
                }
                else
                {
                    return null;
                }

            }
        }

        #endregion

        #region Subroutine

        private void setFocusDefault()
        {
            try
            {
                this.uteRicerca.Focus();
                this.uteRicerca.SelectAll();
            }
            catch
            {
            }
        }

        private void InizializzaControlli()
        {

            try
            {

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaControlli", this.Name);
            }

        }

        private void InizializzaUltraGridLayout()
        {

            try
            {

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaUltraGridLayout", this.Name);
            }

        }

        private void InizializzaFiltri()
        {

            Parametri op = null;
            string xmlParam = "";
            FwDataParametersList plist = null;

            if (this.IsDisposed == false)
            {

                try
                {

                    this.uteRicerca.Text = "";

                    op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    plist = new FwDataParametersList();
                    plist.Add("xParametri", xmlParam, ParameterDirection.Input, DbType.Xml);
                    using (FwDataConnection conn = new FwDataConnection(Database.ConnectionString))
                    {

                        DataTable dt = conn.Query<DataTable>("MSP_SelUODaRuolo", plist, CommandType.StoredProcedure);
                        this.UltraGridUO.DataSource = CoreStatics.AggiungiTuttiDataTable(dt, true);
                        this.UltraGridUO.Refresh();
                        if (this.UltraGridUO.Rows.Count > 0)
                        {
                            this.UltraGridUO.ActiveRow = null;
                            this.UltraGridUO.Selected.Rows.Clear();
                            CoreStatics.SelezionaRigaInGriglia(ref UltraGridUO, "CodUO", CoreStatics.GC_TUTTI);
                        }

                    }

                    op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("DatiEstesi", "0");
                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    plist = new FwDataParametersList();
                    plist.Add("xParametri", xmlParam, ParameterDirection.Input, DbType.Xml);
                    using (FwDataConnection conn = new FwDataConnection(Database.ConnectionString))
                    {

                        DataTable dt = conn.Query<DataTable>("MSP_SelStatoTrasferimento", plist, CommandType.StoredProcedure);
                        this.ucEasyTreeViewFiltroStatoTrasferimento.Nodes.Clear();
                        Infragistics.Win.UltraWinTree.UltraTreeNode oNodeRoot = new Infragistics.Win.UltraWinTree.UltraTreeNode(CoreStatics.GC_TUTTI, "Stato");
                        oNodeRoot.Override.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
                        oNodeRoot.CheckedState = CheckState.Unchecked;
                        foreach (DataRow oDr in dt.Rows)
                        {
                            Infragistics.Win.UltraWinTree.UltraTreeNode oNode = new Infragistics.Win.UltraWinTree.UltraTreeNode(oDr["Codice"].ToString(), oDr["Descrizione"].ToString());
                            oNode.Override.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
                            if (oDr["Codice"].ToString() == EnumStatoTrasferimento.DM.ToString() || oDr["Codice"].ToString() == EnumStatoTrasferimento.TR.ToString())
                            {
                                oNode.CheckedState = CheckState.Checked;
                            }
                            else
                            {
                                oNode.CheckedState = CheckState.Unchecked;
                            }
                            oNodeRoot.Nodes.Add(oNode);
                        }
                        this.ucEasyTreeViewFiltroStatoTrasferimento.Nodes.Add(oNodeRoot);
                        this.ucEasyTreeViewFiltroStatoTrasferimento.ExpandAll();

                        if (this.ucEasyTreeViewFiltroStatoTrasferimento.Nodes.Count > 0 && this.ucEasyTreeViewFiltroStatoTrasferimento.Nodes.Exists(CoreStatics.GC_TUTTI))
                        {
                            this.ucEasyTreeViewFiltroStatoTrasferimento.Nodes[CoreStatics.GC_TUTTI].CheckedState = CheckState.Unchecked;
                            this.ucEasyTreeViewFiltroStatoTrasferimento.Nodes[CoreStatics.GC_TUTTI].CheckedState = CheckState.Checked;
                        }

                    }

                }
                catch (Exception ex)
                {
                    CoreStatics.ExGest(ref ex, "InizializzaFiltri", this.Name);
                }

            }

        }

        private void CaricaGrigliaP()
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);

                if (this.uteRicerca.Text != string.Empty)
                {

                    string filtrogenerico = string.Empty;

                    string[] ricerche = this.uteRicerca.Text.Split(' ');
                    foreach (string ricerca in ricerche)
                    {

                        string format = "dd/MM/yyyy";
                        DateTime dateTime;
                        if (DateTime.TryParseExact(ricerca, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                        {
                            op.Parametro.Add("DataNascita", ricerca);
                        }
                        else
                        {
                            filtrogenerico += ricerca + " ";
                        }

                    }

                    op.Parametro.Add("FiltroGenerico", filtrogenerico);

                }

                if (this.UltraGridUO.ActiveRow != null && this.UltraGridUO.ActiveRow.Cells["CodUO"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                {
                    op.Parametro.Add("CodUO", this.UltraGridUO.ActiveRow.Cells["CodUO"].Text);
                }

                Dictionary<string, string> listastato = new Dictionary<string, string>();
                string[] codstato = null;
                if ((this.ucEasyTreeViewFiltroStatoTrasferimento.Nodes.Count > 0))
                {
                    foreach (Infragistics.Win.UltraWinTree.UltraTreeNode oNode in this.ucEasyTreeViewFiltroStatoTrasferimento.Nodes[CoreStatics.GC_TUTTI].Nodes)
                    {
                        if (oNode.Override.NodeStyle == Infragistics.Win.UltraWinTree.NodeStyle.CheckBox && oNode.CheckedState == CheckState.Checked)
                        {
                            listastato.Add(oNode.Key, oNode.Text);
                        }
                    }

                }
                if (listastato.Count > 0)
                {
                    codstato = listastato.Keys.ToArray();
                    op.ParametroRipetibile.Add("CodStatoTrasferimento", codstato);
                }

                op.TimeStamp.CodEntita = EnumEntita.CIV.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_CercaCartelleInVisione", spcoll);

                this.UltraGridCartellePaziente.DataSource = dt;
                this.UltraGridCartellePaziente.Refresh();
                this.UltraGridCartellePaziente.Text = string.Format("Cartelle Pazienti ({0}).", dt.Rows.Count);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaGrigliaP", this.Name);
            }

        }

        private void CaricaGrigliaA()
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                op.Parametro.Add("FiltroGenerico", this.uteRicerca.Text);

                op.TimeStamp.CodEntita = EnumEntita.PIV.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_CercaPazienteInVisione", spcoll);

                this.UltraGridCartelleAmbulatoriali.DataSource = dt;
                this.UltraGridCartelleAmbulatoriali.Refresh();
                this.UltraGridCartelleAmbulatoriali.Text = string.Format("Cartelle Ambulatoriali ({0}).", dt.Rows.Count);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaGrigliaA", this.Name);
            }

        }

        #endregion

        #region Events

        private void uteRicerca_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) ubRicerca_Click(this.ubRicerca, new EventArgs());
        }

        private void ubRicerca_Click(object sender, EventArgs e)
        {

            this.CaricaGrigliaP();
            this.CaricaGrigliaA();
            this.setFocusDefault();

        }

        private void ubAzzera_Click(object sender, EventArgs e)
        {

            this.InizializzaFiltri();

            this.CaricaGrigliaA();
            this.CaricaGrigliaP();

            this.setFocusDefault();

        }

        private void UltraGridUO_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].HeaderVisible = false;
            foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
            {
                switch (oCol.Key)
                {
                    case "Descrizione":
                        oCol.Hidden = false;
                        oCol.Header.Caption = @"Unità Operativa";
                        break;
                    default:
                        oCol.Hidden = true;
                        break;
                }
            }
        }

        private void ucEasyTreeViewFiltroStatoTrasferimento_AfterCheck(object sender, Infragistics.Win.UltraWinTree.NodeEventArgs e)
        {

            try
            {

                if (e.TreeNode.Key == CoreStatics.GC_TUTTI)
                {
                    foreach (Infragistics.Win.UltraWinTree.UltraTreeNode oNode in ((Infragistics.Win.UltraWinTree.UltraTree)sender).Nodes[CoreStatics.GC_TUTTI].Nodes)
                    {
                        if (oNode.Override.NodeStyle == Infragistics.Win.UltraWinTree.NodeStyle.CheckBox)
                        {
                            oNode.CheckedState = e.TreeNode.CheckedState;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void UltraGridCartellePaziente_AfterRowActivate(object sender, EventArgs e)
        {
            this.UltraGridCartelleAmbulatoriali.ActiveRow = null;
            this.UltraGridCartelleAmbulatoriali.Selected.Rows.Clear();
        }

        private void UltraGridCartellePaziente_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {

                bool bSwitchGroupHeaders = false;


                UltraGridGroup grpPaziente = null;
                UltraGridGroup grpStruttura = null;

                int refWidth = (int)(easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XLarge) * 2.4);
                if (_16_9)
                {
                    refWidth = (int)(easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XLarge) * 3.0);
                }

                Graphics g = this.CreateGraphics();
                int refBtnWidth = Convert.ToInt32(CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.UltraGridCartellePaziente.DataRowFontRelativeDimension), g.DpiY) * 2.6F);
                g.Dispose();
                g = null;
                e.Layout.Bands[0].RowLayoutStyle = RowLayoutStyle.GroupLayout;

                e.Layout.Bands[0].ColHeadersVisible = !bSwitchGroupHeaders;
                e.Layout.Bands[0].GroupHeadersVisible = bSwitchGroupHeaders;
                if (bSwitchGroupHeaders)
                {
                    for (int c = e.Layout.Bands[0].Groups.Count - 1; c >= 0; c--)
                    {
                        try
                        {
                            UltraGridGroup grp = e.Layout.Bands[0].Groups[c];
                            e.Layout.Bands[0].Groups.RemoveAt(c);
                            grp.Dispose();
                        }
                        catch
                        {
                        }
                    }
                    e.Layout.Bands[0].Groups.Clear();

                    e.Layout.Bands[0].GroupHeaderLines = 2;
                    grpPaziente = e.Layout.Bands[0].Groups.Add(@"grpPaziente", @"Paziente" + Environment.NewLine + @"Indirizzo");
                    grpStruttura = e.Layout.Bands[0].Groups.Add(@"grpStruttura", @"Struttura" + Environment.NewLine + @"UO - Settore");
                }
                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {

                    switch (oCol.Key)
                    {

                        case "Paziente2":
                            oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Medium) * 0.98F;
                            oCol.CellAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                            oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                            oCol.Header.Caption = "Paziente";
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                if (_16_9)
                                    oCol.MaxWidth = Convert.ToInt32(((float)this.UltraGridCartellePaziente.Width - ((float)refWidth * 7.1F) - ((float)refBtnWidth * 4) - 18F) * 70 / 100);
                                else
                                    oCol.MaxWidth = Convert.ToInt32(((float)this.UltraGridCartellePaziente.Width - ((float)refWidth * 7.1F) - ((float)refBtnWidth * 4) - 18F) * 80 / 100);
                                oCol.MinWidth = oCol.MaxWidth;
                                oCol.Width = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }
                            oCol.RowLayoutColumnInfo.OriginX = 0;
                            oCol.RowLayoutColumnInfo.OriginY = 0;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 1;
                            if (bSwitchGroupHeaders)
                            {
                                oCol.RowLayoutColumnInfo.ParentGroup = grpPaziente;
                                oCol.RowLayoutColumnInfo.ParentGroup.Header.Appearance.FontData.SizeInPoints = oCol.Header.Appearance.FontData.SizeInPoints;
                            }
                            break;

                        case "Paziente3":
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                            oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                            oCol.Header.Caption = "Sesso, Luogo e Data Nascita";
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                            }
                            catch (Exception)
                            {
                            }
                            oCol.RowLayoutColumnInfo.OriginX = 0;
                            oCol.RowLayoutColumnInfo.OriginY = 1;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 1;
                            if (bSwitchGroupHeaders)
                            {
                                oCol.RowLayoutColumnInfo.ParentGroup = grpPaziente;
                                oCol.RowLayoutColumnInfo.ParentGroup.Header.Appearance.FontData.SizeInPoints = oCol.Header.Appearance.FontData.SizeInPoints;
                            }
                            break;

                        case "DescrUA":
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                            oCol.Header.Caption = "Struttura";
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                oCol.MaxWidth = (int)(refWidth * 2.0);
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
                            if (bSwitchGroupHeaders)
                            {
                                oCol.RowLayoutColumnInfo.ParentGroup = grpStruttura;
                                oCol.RowLayoutColumnInfo.ParentGroup.Header.Appearance.FontData.SizeInPoints = oCol.Header.Appearance.FontData.SizeInPoints;
                            }
                            break;

                        case "UO - Settore":
                            oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            oCol.Hidden = false;
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                            }
                            catch (Exception)
                            {
                            }
                            oCol.RowLayoutColumnInfo.OriginX = 1;
                            oCol.RowLayoutColumnInfo.OriginY = 1;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 1;
                            if (bSwitchGroupHeaders)
                            {
                                oCol.RowLayoutColumnInfo.ParentGroup = grpStruttura;
                                oCol.RowLayoutColumnInfo.ParentGroup.Header.Appearance.FontData.SizeInPoints = oCol.Header.Appearance.FontData.SizeInPoints;
                            }
                            break;



                        case "DataIngressoGriglia":
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            oCol.Header.Caption = @"Data Ingresso";
                            oCol.Format = "dd/MM/yyyy HH:mm";
                            oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                oCol.MaxWidth = (int)(refWidth * 1.4);
                                oCol.MinWidth = oCol.MaxWidth;
                                oCol.Width = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }
                            oCol.RowLayoutColumnInfo.OriginX = 2;
                            oCol.RowLayoutColumnInfo.OriginY = 0;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 1;
                            break;

                        case "DataRicoveroGriglia":
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            oCol.Header.Caption = @"Data Ricovero";
                            oCol.Format = "dd/MM/yyyy HH:mm";
                            oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                oCol.MaxWidth = (int)(refWidth * 1.4);
                                oCol.MinWidth = oCol.MaxWidth;
                                oCol.Width = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }
                            oCol.RowLayoutColumnInfo.OriginX = 2;
                            oCol.RowLayoutColumnInfo.OriginY = 1;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 1;
                            break;

                        case "DescrStatoGriglia":
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                            oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            oCol.Header.Caption = @"Stato";
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                oCol.MaxWidth = (int)(refWidth * 1.4);
                                oCol.MinWidth = oCol.MaxWidth;
                                oCol.Width = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }
                            oCol.RowLayoutColumnInfo.OriginX = 4;
                            oCol.RowLayoutColumnInfo.OriginY = 0;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 2;
                            if (bSwitchGroupHeaders)
                            {
                                oCol.RowLayoutColumnInfo.ParentGroup = e.Layout.Bands[0].Groups.Add(@"grpStato", oCol.Header.Caption);
                                oCol.RowLayoutColumnInfo.ParentGroup.Header.Appearance.FontData.SizeInPoints = oCol.Header.Appearance.FontData.SizeInPoints;
                            }
                            break;

                        case "DescStanzaLetto":
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            if (_16_9)
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Medium) * 0.94F;
                            else
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                            oCol.Header.Caption = @"Letto / Stanza";
                            oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                if (_16_9)
                                    oCol.MaxWidth = Convert.ToInt32(((float)this.UltraGridCartellePaziente.Width - ((float)refWidth * 7.1F) - ((float)refBtnWidth * 4F) - 18F) * 30 / 100);
                                else
                                    oCol.MaxWidth = Convert.ToInt32(((float)this.UltraGridCartellePaziente.Width - ((float)refWidth * 7.1F) - ((float)refBtnWidth * 4F) - 18F) * 20 / 100);
                                oCol.MinWidth = oCol.MaxWidth;
                                oCol.Width = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }
                            oCol.RowLayoutColumnInfo.OriginX = 5;
                            oCol.RowLayoutColumnInfo.OriginY = 0;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 2;
                            if (bSwitchGroupHeaders)
                            {
                                oCol.RowLayoutColumnInfo.ParentGroup = e.Layout.Bands[0].Groups.Add(@"grpStanza", oCol.Header.Caption);
                                oCol.RowLayoutColumnInfo.ParentGroup.Header.Appearance.FontData.SizeInPoints = oCol.Header.Appearance.FontData.SizeInPoints;
                            }
                            break;

                        case "DescEpisodio":
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            oCol.Header.Caption = @"Tipo Episodio / " + Environment.NewLine + @"Nosologico";
                            oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                oCol.MaxWidth = (int)(refWidth * 1.2);
                                oCol.MinWidth = oCol.MaxWidth;
                                oCol.Width = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }
                            oCol.RowLayoutColumnInfo.OriginX = 6;
                            oCol.RowLayoutColumnInfo.OriginY = 0;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 2;
                            if (bSwitchGroupHeaders)
                            {
                                oCol.RowLayoutColumnInfo.ParentGroup = e.Layout.Bands[0].Groups.Add(@"grpEpisodio", oCol.Header.Caption);
                                oCol.RowLayoutColumnInfo.ParentGroup.Header.Appearance.FontData.SizeInPoints = oCol.Header.Appearance.FontData.SizeInPoints;
                            }
                            break;

                        case "DescrCartellaGriglia":
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            oCol.Header.Caption = "Cartella";
                            oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                oCol.MaxWidth = (int)(refWidth * 1.1);
                                oCol.MinWidth = oCol.MaxWidth;
                                oCol.Width = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }
                            oCol.RowLayoutColumnInfo.OriginX = 7;
                            oCol.RowLayoutColumnInfo.OriginY = 0;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 2;
                            if (bSwitchGroupHeaders)
                            {
                                oCol.RowLayoutColumnInfo.ParentGroup = e.Layout.Bands[0].Groups.Add(@"grpNumCart", oCol.Header.Caption);
                                oCol.RowLayoutColumnInfo.ParentGroup.Header.Appearance.FontData.SizeInPoints = oCol.Header.Appearance.FontData.SizeInPoints;
                            }
                            break;




                        case "NA":
                            oCol.Header.Caption = @"";
                            oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                            oCol.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                oCol.LockedWidth = true;
                                oCol.MaxWidth = refBtnWidth;
                                oCol.Width = oCol.MaxWidth;
                                oCol.MinWidth = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }
                            oCol.RowLayoutColumnInfo.OriginX = 8;
                            oCol.RowLayoutColumnInfo.OriginY = 0;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 2;
                            if (bSwitchGroupHeaders)
                            {
                                oCol.RowLayoutColumnInfo.ParentGroup = e.Layout.Bands[0].Groups.Add(@"grpNA", oCol.Header.Caption);
                                oCol.RowLayoutColumnInfo.ParentGroup.Header.Appearance.FontData.SizeInPoints = oCol.Header.Appearance.FontData.SizeInPoints;
                            }
                            break;

                        case "NAG":
                            oCol.Header.Caption = @"";
                            oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                            oCol.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                oCol.LockedWidth = true;
                                oCol.MaxWidth = refBtnWidth;
                                oCol.Width = oCol.MaxWidth;
                                oCol.MinWidth = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }
                            oCol.RowLayoutColumnInfo.OriginX = 9;
                            oCol.RowLayoutColumnInfo.OriginY = 0;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 2;
                            if (bSwitchGroupHeaders) oCol.RowLayoutColumnInfo.ParentGroup = e.Layout.Bands[0].Groups.Add(@"grpNAG", oCol.Header.Caption);
                            break;

                        case "NEC":
                            oCol.Header.Caption = @"";
                            oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                            oCol.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                oCol.LockedWidth = true;
                                oCol.MaxWidth = refBtnWidth;
                                oCol.Width = oCol.MaxWidth;
                                oCol.MinWidth = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }
                            oCol.RowLayoutColumnInfo.OriginX = 10;
                            oCol.RowLayoutColumnInfo.OriginY = 0;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 2;
                            if (bSwitchGroupHeaders) oCol.RowLayoutColumnInfo.ParentGroup = e.Layout.Bands[0].Groups.Add(@"grpNEC", oCol.Header.Caption);
                            break;

                        case "CIV":
                            oCol.Header.Caption = @"";
                            oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                            oCol.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                oCol.LockedWidth = true;
                                oCol.MaxWidth = refBtnWidth;
                                oCol.Width = oCol.MaxWidth;
                                oCol.MinWidth = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }
                            oCol.RowLayoutColumnInfo.OriginX = 11;
                            oCol.RowLayoutColumnInfo.OriginY = 0;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 2;
                            if (bSwitchGroupHeaders) oCol.RowLayoutColumnInfo.ParentGroup = e.Layout.Bands[0].Groups.Add(@"grpCIV", oCol.Header.Caption);
                            break;

                        default:
                            oCol.Hidden = true;
                            break;

                    }

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        private void UltraGridCartellePaziente_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {

            try
            {

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        private void UltraGridCartelleAmbulatoriali_AfterRowActivate(object sender, EventArgs e)
        {
            this.UltraGridCartellePaziente.ActiveRow = null;
            this.UltraGridCartellePaziente.Selected.Rows.Clear();
        }

        private void UltraGridCartelleAmbulatoriali_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            foreach (Infragistics.Win.UltraWinGrid.UltraGridColumn oCol in e.Layout.Bands[0].Columns)
            {
                switch (oCol.Key)
                {

                    case @"Paziente":
                        oCol.Header.Caption = "Paziente";
                        oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                        break;

                    case @"Sesso":
                        oCol.Header.Caption = "Sesso";
                        oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                        break;


                    case @"NascitaDescrizione":
                        oCol.Header.Caption = "Data Luogo Nascita";
                        oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                        oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                        oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                        oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                        break;

                    case @"CodiceFiscale":
                        oCol.Header.Caption = "C.F.";
                        oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                        break;


                    case @"ComuneResidenza":
                        oCol.Header.Caption = "Residente a";
                        oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                        break;

                    default:
                        oCol.Hidden = true;
                        break;
                }
            }

            e.Layout.Bands[0].Columns["Paziente"].Header.VisiblePosition = 0;
            e.Layout.Bands[0].Columns["Sesso"].Header.VisiblePosition = 1;
            e.Layout.Bands[0].Columns["NascitaDescrizione"].Header.VisiblePosition = 2;
            e.Layout.Bands[0].Columns["CodiceFiscale"].Header.VisiblePosition = 3;
            e.Layout.Bands[0].Columns["ComuneResidenza"].Header.VisiblePosition = 4;

        }

        private void UltraGridCartelleAmbulatoriali_InitializeRow(object sender, InitializeRowEventArgs e)
        {

        }

        #endregion

    }
}
