using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.ScciCore;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmAmministrazione : Form, Interfacce.IViewFormBase, Infragistics.Win.IUIElementCreationFilter
    {
        public frmAmministrazione()
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

        public Image ViewImage
        {
            get
            {
                return this.PicImage.Image;
            }
            set
            {
                this.PicImage.Image = value;
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
            this.InitializeUltraGrid();
            this.udteDataNascitaEpi.Value = null;

            CoreStatics.CoreApplication.Ambiente.Codlogin = UnicodeSrl.Framework.Utility.Windows.CurrentUser();
            CoreStatics.CoreApplication.Ambiente.Codruolo = UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.ElabSistemiCodRuolo);

            this.uteCognomeEpi.Focus();

            this.ResumeLayout();

        }

        #endregion

        #region Interface IUIElement

        public void AfterCreateChildElements(UIElement parent)
        {

            Rectangle rc;
            Rectangle childRect;
            ButtonUIElement elementToAdd;

            if (parent.GetType() == typeof(CellUIElement))
            {

                UltraGridColumn column = (UltraGridColumn)parent.GetContext(typeof(UltraGridColumn));

                if (column != null)
                {

                    if (column.Key == "ID" && column.Band.Key == "EpiPaz")
                    {

                        elementToAdd = new ButtonUIElement(parent);
                        elementToAdd.Text = "...";

                        Infragistics.Win.Appearance oAppButton = new Infragistics.Win.Appearance();
                        oAppButton.BackColor = System.Drawing.Color.SteelBlue;
                        oAppButton.BackColor2 = System.Drawing.Color.LightSteelBlue;
                        oAppButton.FontData.Bold = DefaultableBoolean.True;
                        oAppButton.ForeColor = System.Drawing.Color.White;
                        oAppButton.ThemedElementAlpha = Alpha.Transparent;
                        elementToAdd.Appearance = oAppButton;

                        elementToAdd.ElementClick += OnCustomButtonClick;

                        rc = parent.RectInsideBorders;

                        rc.Width = 25;

                        elementToAdd.Rect = rc;

                        foreach (UIElement child in parent.ChildElements)
                        {

                            childRect = child.Rect;

                            if (childRect.Left < rc.Right)
                            {
                                childRect.Width -= rc.Right - childRect.Left;
                                childRect.X += rc.Right - childRect.Left;
                                child.Rect = childRect;
                            }
                        }

                        parent.ChildElements.Add(elementToAdd);

                    }

                }

            }

        }

        public bool BeforeCreateChildElements(UIElement parent)
        {
            return false;
        }

        #endregion

        #region UltraGrid

        private void InitializeUltraGrid()
        {

            MyStatics.SetUltraGridLayout(ref this.UltraGrid, false, false);

            this.UltraGrid.CreationFilter = this;
            this.UltraGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.UltraGrid.DisplayLayout.GroupByBox.Hidden = true;
            this.UltraGrid.DisplayLayout.InterBandSpacing = 10;
            this.UltraGrid.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;

        }

        private void LoadUltraGridEpi()
        {


            string sSql = string.Empty;
            string sSqlEpiWhere = string.Empty;

            DataSet oDs = null;
            DataRelation dr = null;

            this.Cursor = Cursors.WaitCursor;

            try
            {

                bool bCheckRicerca = false;

                sSqlEpiWhere = "Select E.ID" + Environment.NewLine +
"From T_MovEpisodi E" + Environment.NewLine +
"Inner Join T_MovPazienti P On E.ID = P.IDEpisodio" + Environment.NewLine;
                if (this.uteCognomeEpi.Text != string.Empty)
                {
                    if (sSqlEpiWhere != string.Empty) { sSqlEpiWhere += "And "; }
                    sSqlEpiWhere += "P.Cognome Like '" + DataBase.Ax2(this.uteCognomeEpi.Text) + "%'" + Environment.NewLine;
                    bCheckRicerca = true;
                }
                if (this.uteNomeEpi.Text != string.Empty)
                {
                    if (sSqlEpiWhere != string.Empty) { sSqlEpiWhere += "And "; }
                    sSqlEpiWhere += "P.Nome Like '" + DataBase.Ax2(this.uteNomeEpi.Text) + "%'" + Environment.NewLine;
                    bCheckRicerca = true;
                }
                if (this.udteDataNascitaEpi.Value != null)
                {
                    if (sSqlEpiWhere != string.Empty) { sSqlEpiWhere += "And "; }
                    sSqlEpiWhere += "P.DataNascita = " + DataBase.SQLDate((DateTime)this.udteDataNascitaEpi.Value) + Environment.NewLine;
                    bCheckRicerca = true;
                }
                if (this.uteNosologicoEpi.Text != string.Empty)
                {
                    if (sSqlEpiWhere != string.Empty) { sSqlEpiWhere += "And "; }
                    sSqlEpiWhere += "E.NumeroNosologico Like '" + DataBase.Ax2(this.uteNosologicoEpi.Text) + "%'" + Environment.NewLine;
                    bCheckRicerca = true;
                }
                if (this.uteListaAttesaEpi.Text != string.Empty)
                {
                    if (sSqlEpiWhere != string.Empty) { sSqlEpiWhere += "And "; }
                    sSqlEpiWhere += "E.NumeroListaAttesa Like '" + DataBase.Ax2(this.uteListaAttesaEpi.Text) + "%'" + Environment.NewLine;
                    bCheckRicerca = true;
                }

                if (bCheckRicerca == true)
                {
                    sSql = "Select P.Cognome, P.Nome, P.CodiceFiscale As [Codice Fiscale], P.DataNascita As [Data di Nascita], E.*" + Environment.NewLine +
"From T_MovEpisodi E" + Environment.NewLine +
"Inner Join T_MovPazienti P On E.ID = P.IDEpisodio" + Environment.NewLine +
"Where E.ID In (" + sSqlEpiWhere + ")" + Environment.NewLine +
"Order By P.Cognome, P.Nome, P.DataNascita" + Environment.NewLine;

                    sSql += "Select P.*" + Environment.NewLine +
"From T_MovPazienti P" + Environment.NewLine +
"Where P.IDEpisodio In (" + sSqlEpiWhere + ")" + Environment.NewLine +
"Order By P.Cognome, P.Nome, P.DataNascita" + Environment.NewLine;

                    sSql += "Select T.*" + Environment.NewLine +
"From T_MovTrasferimenti T" + Environment.NewLine +
"Where T.IDEpisodio In (" + sSqlEpiWhere + ")" + Environment.NewLine;

                    sSql += "Select C.*, T.ID As IDTrasferimento" + Environment.NewLine +
"From T_MovCartelle C" + Environment.NewLine +
"Inner Join T_MovTrasferimenti T On C.ID = T.IDCartella" + Environment.NewLine +
"Where T.IDEpisodio In (" + sSqlEpiWhere + ")" + Environment.NewLine;

                    sSql += "Select TI.*" + Environment.NewLine +
"From T_MovTaskInfermieristici TI" + Environment.NewLine +
"Where TI.IDEpisodio In (" + sSqlEpiWhere + ")" + Environment.NewLine;

                    sSql += "Select S.*" + Environment.NewLine +
"From T_MovSchede S" + Environment.NewLine +
"Where S.CodEntita = 'WKI'" + Environment.NewLine +
"And S.IDEntita In (" + Environment.NewLine +
"Select TI.ID" + Environment.NewLine +
    "From T_MovTaskInfermieristici TI" + Environment.NewLine +
    "Where TI.IDEpisodio In (" + sSqlEpiWhere + ")" + Environment.NewLine + ")" + Environment.NewLine;

                    sSql += "Select PV.*" + Environment.NewLine +
"From T_MovParametriVitali PV" + Environment.NewLine +
"Where PV.IDEpisodio In (" + sSqlEpiWhere + ")" + Environment.NewLine;

                    sSql += "Select S.*" + Environment.NewLine +
"From T_MovSchede S" + Environment.NewLine +
"Where S.CodEntita = 'PVT'" + Environment.NewLine +
"And S.IDEntita In (" + Environment.NewLine +
"Select PV.ID" + Environment.NewLine +
    "From T_MovParametriVitali PV" + Environment.NewLine +
    "Where PV.IDEpisodio In (" + sSqlEpiWhere + ")" + Environment.NewLine + ")" + Environment.NewLine;

                    sSql += "Select EC.*" + Environment.NewLine +
"From T_MovEvidenzaClinica EC" + Environment.NewLine +
"Where EC.IDEpisodio In (" + sSqlEpiWhere + ")" + Environment.NewLine;


                    sSql += "Select DC.*" + Environment.NewLine +
"From T_MovDiarioClinico DC" + Environment.NewLine +
"Where DC.IDEpisodio In (" + sSqlEpiWhere + ")" + Environment.NewLine;

                    sSql += "Select S.*" + Environment.NewLine +
"From T_MovSchede S" + Environment.NewLine +
"Where S.CodEntita = 'DCL'" + Environment.NewLine +
"And S.IDEntita In (" + Environment.NewLine +
"Select DC.ID" + Environment.NewLine +
    "From T_MovDiarioClinico DC" + Environment.NewLine +
    "Where DC.IDEpisodio In (" + sSqlEpiWhere + ")" + Environment.NewLine + ")" + Environment.NewLine;

                    sSql += "Select APP.*" + Environment.NewLine +
"From T_MovAppuntamenti APP" + Environment.NewLine +
"Where APP.IDEpisodio In (" + sSqlEpiWhere + ")" + Environment.NewLine;

                    sSql += "Select S.*" + Environment.NewLine +
"From T_MovSchede S" + Environment.NewLine +
"Where S.CodEntita = 'APP'" + Environment.NewLine +
"And S.IDEntita In (" + Environment.NewLine +
"Select APP.ID" + Environment.NewLine +
    "From T_MovAppuntamenti APP" + Environment.NewLine +
    "Where APP.IDEpisodio In (" + sSqlEpiWhere + ")" + Environment.NewLine + ")" + Environment.NewLine;

                    oDs = DataBase.GetDataSet(sSql);
                    oDs.Tables[0].TableName = "Episodi";
                    oDs.Tables[1].TableName = "Paziente";
                    oDs.Tables[2].TableName = "Trasferimenti";
                    oDs.Tables[3].TableName = "Cartella";
                    oDs.Tables[4].TableName = "TaskInfermieristici";
                    oDs.Tables[5].TableName = "SchedeTI";
                    oDs.Tables[6].TableName = "ParametriVitali";
                    oDs.Tables[7].TableName = "SchedePV";
                    oDs.Tables[8].TableName = "EvidenzaClinica";
                    oDs.Tables[9].TableName = "DiarioClinico";
                    oDs.Tables[10].TableName = "SchedeDC";
                    oDs.Tables[11].TableName = "Appuntamenti";
                    oDs.Tables[12].TableName = "SchedeAP";

                    dr = new DataRelation("EpiPaz", oDs.Tables["Episodi"].Columns["ID"], oDs.Tables["Paziente"].Columns["IDEpisodio"], false);
                    oDs.Relations.Add(dr);
                    dr = new DataRelation("EpiTra", oDs.Tables["Episodi"].Columns["ID"], oDs.Tables["Trasferimenti"].Columns["IDEpisodio"], false);
                    oDs.Relations.Add(dr);
                    dr = new DataRelation("TraCar", oDs.Tables["Trasferimenti"].Columns["ID"], oDs.Tables["Cartella"].Columns["IDTrasferimento"], false);
                    oDs.Relations.Add(dr);
                    dr = new DataRelation("EpiTi", oDs.Tables["Episodi"].Columns["ID"], oDs.Tables["TaskInfermieristici"].Columns["IDEpisodio"], false);
                    oDs.Relations.Add(dr);
                    dr = new DataRelation("TiSch", oDs.Tables["TaskInfermieristici"].Columns["ID"], oDs.Tables["SchedeTI"].Columns["IDEntita"], false);
                    oDs.Relations.Add(dr);
                    dr = new DataRelation("EpiPv", oDs.Tables["Episodi"].Columns["ID"], oDs.Tables["ParametriVitali"].Columns["IDEpisodio"], false);
                    oDs.Relations.Add(dr);
                    dr = new DataRelation("PvSch", oDs.Tables["ParametriVitali"].Columns["ID"], oDs.Tables["SchedePV"].Columns["IDEntita"], false);
                    oDs.Relations.Add(dr);
                    dr = new DataRelation("EpiEc", oDs.Tables["Episodi"].Columns["ID"], oDs.Tables["EvidenzaClinica"].Columns["IDEpisodio"], false);
                    oDs.Relations.Add(dr);
                    dr = new DataRelation("EpiDc", oDs.Tables["Episodi"].Columns["ID"], oDs.Tables["DiarioClinico"].Columns["IDEpisodio"], false);
                    oDs.Relations.Add(dr);
                    dr = new DataRelation("DcSch", oDs.Tables["DiarioClinico"].Columns["ID"], oDs.Tables["SchedeDC"].Columns["IDEntita"], false);
                    oDs.Relations.Add(dr);
                    dr = new DataRelation("EpiApp", oDs.Tables["Episodi"].Columns["ID"], oDs.Tables["Appuntamenti"].Columns["IDEpisodio"], false);
                    oDs.Relations.Add(dr);
                    dr = new DataRelation("DcApp", oDs.Tables["Appuntamenti"].Columns["ID"], oDs.Tables["SchedeAP"].Columns["IDEntita"], false);
                    oDs.Relations.Add(dr);

                    this.UltraGrid.DataSource = oDs;
                    this.UltraGrid.Refresh();
                    this.UltraGrid.Text = string.Format("{0} ({1:#,##0})", "Ricerca Episodio", this.UltraGrid.Rows.Count);

                    this.UltraGrid.DisplayLayout.PerformAutoResizeColumns(false, Infragistics.Win.UltraWinGrid.PerformAutoSizeType.AllRowsInBand);

                }
                else
                {
                    MessageBox.Show("Inserire almeno un parametro di ricerca!", "Ricerca per Episodio", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this.uteCognomeEpi.Focus();
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            this.Cursor = Cursors.Default;

        }

        #endregion

        #region SubRoutine

        private void AggiornaDatiSAC(string idpaziente, string idepisodio)
        {

            this.Cursor = Cursors.WaitCursor;

            try
            {

                Paziente oPaziente = new Paziente(idpaziente, idepisodio);
                oPaziente.AggiornaDatiSAC();
                oPaziente = null;

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

        }

        #endregion

        #region Events Form

        private void frmAmministrazione_FormClosing(object sender, FormClosingEventArgs e)
        {
            CoreStatics.CoreApplication.Ambiente.Codlogin = string.Empty;
            CoreStatics.CoreApplication.Ambiente.Codruolo = string.Empty;
            if (this.UltraGrid != null) this.UltraGrid.Dispose();
        }

        #endregion

        #region Events

        private void ubRicercaEpi_Click(object sender, EventArgs e)
        {
            this.LoadUltraGridEpi();
        }

        private void UltraGrid_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {

                if (this.UltraTabControlFiltri.ActiveTab.Key == "Episodio")
                {
                    e.Layout.Bands[0].Header.Caption = "Episodi";
                    e.Layout.Bands[1].Header.Caption = "Paziente";
                    e.Layout.Bands[2].Header.Caption = "Trasferimenti";
                    e.Layout.Bands[3].Header.Caption = "Cartella";
                    e.Layout.Bands[4].Header.Caption = "Task Infermieristici";
                    e.Layout.Bands[5].Header.Caption = "Schede Task Infermieristici";
                    e.Layout.Bands[6].Header.Caption = "Parametri Vitali";
                    e.Layout.Bands[7].Header.Caption = "Schede Parametri Vitali";
                    e.Layout.Bands[8].Header.Caption = "Evidenza Clinica";
                    e.Layout.Bands[9].Header.Caption = "Diario Clinico";
                    e.Layout.Bands[10].Header.Caption = "Schede Diario Clinico";
                    e.Layout.Bands[11].Header.Caption = "Appuntamenti";
                    e.Layout.Bands[12].Header.Caption = "Schede Appuntamento";
                }

                for (int x = 0; x <= e.Layout.Bands.Count - 1; x++)
                {
                    e.Layout.Bands[x].Header.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                    e.Layout.Bands[x].Header.Appearance.FontData.SizeInPoints = 12;
                    e.Layout.Bands[x].Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Left;
                    e.Layout.Bands[x].HeaderVisible = true;
                    e.Layout.Bands[x].Indentation = 30;
                }

            }
            catch (Exception)
            {

            }

        }

        #endregion

        #region Events IUIElement

        private void OnCustomButtonClick(object sender, UIElementEventArgs e)
        {

            UltraGridCell cell = (UltraGridCell)e.Element.GetContext(typeof(UltraGridCell));

            if (cell != null)
            {

                switch (cell.Column.Key)
                {

                    case "ID":
                        if (MessageBox.Show($"Vuoi aggiornare l'anagrafica di\n" +
                                            $"{cell.Row.Cells["Cognome"].Value.ToString()} {cell.Row.Cells["Nome"].Value.ToString()}\n" +
                                            $"dal SAC?",
                                            "Aggiornamento anagrafica SAC",
                                            MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        {
                            this.AggiornaDatiSAC(cell.Row.Cells["ID"].Value.ToString(), cell.Row.Cells["IDEpisodio"].Value.ToString());
                            this.LoadUltraGridEpi();
                        }
                        break;

                }

            }

        }

        #endregion

    }
}
