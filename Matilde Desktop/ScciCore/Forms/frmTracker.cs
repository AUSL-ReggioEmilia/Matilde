using Infragistics.Win.UltraWinGrid;
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
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.ScciResource;

namespace UnicodeSrl.ScciCore
{
    public partial class frmTracker : frmBaseModale, Interfacce.IViewFormlModal
    {

        #region Declare

        private bool _ambulatoriale = false;

        #endregion

        public frmTracker()
        {
            InitializeComponent();
        }

        #region Interfaccia

        public new void Carica()
        {

            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_TRACKER_256);

                CheckAmbulatoriale();

                this.uchkFiltro.UNCheckedImage = Risorse.GetImageFromResource(Risorse.GC_FILTRO_256);
                this.uchkFiltro.CheckedImage = Risorse.GetImageFromResource(Risorse.GC_FILTROAPPLICATO_256);
                this.uchkFiltro.Checked = false;

                this.chkSoloEpisodio.Visible = !_ambulatoriale;
                this.chkSoloEpisodio.Checked = (CoreStatics.CoreApplication.Episodio == null ? false : true);
                this.lblSoloEpisodio.Visible = !_ambulatoriale;

                this.uchkFiltro.PercImageFill = 0.75F;
                this.uchkFiltro.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;
                this.uchkFiltro.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.ubApplicaFiltro.PercImageFill = 0.75F;

                CoreStatics.SetEasyUltraDockManager(ref this.ultraDockManager);

                this.ucAnteprimaRtf.rtbRichTextBox.ReadOnly = true;

                this.InizializzaFiltri();
                this.InizializzaUltraGridLayout();

                this.drFiltro.Value = ucEasyDateRange.C_RNG_N30G; CoreStatics.SelezionaRigaInGriglia(ref ucEasyGridFiltroStato, "Codice", EnumStatoAppuntamento.PR.ToString());
                this.Aggiorna();

                this.ShowDialog();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Carica", this.Name);
            }

        }

        #endregion

        #region Subroutine

        private void CheckAmbulatoriale()
        {

            _ambulatoriale = false;
            if (CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ID == EnumMaschere.Ambulatoriale_AgendePaziente)
            {
                _ambulatoriale = true;
            }

        }

        private void InizializzaFiltri()
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);
                op.Parametro.Add("DatiEstesi", "1");

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataSet ds = Database.GetDatasetStoredProc("MSP_SelMovAppuntamenti", spcoll);

                this.ucEasyGridFiltroTipo.DataSource = CoreStatics.AggiungiTuttiDataTable(ds.Tables[1], true);
                this.ucEasyGridFiltroTipo.DisplayLayout.Bands[0].Columns["Descrizione"].Header.Caption = "Tipo";
                this.ucEasyGridFiltroTipo.Refresh();

                this.ucEasyGridFiltroStato.DataSource = CoreStatics.AggiungiTuttiDataTable(ds.Tables[2], true);
                this.ucEasyGridFiltroStato.DisplayLayout.Bands[0].Columns["Descrizione"].Header.Caption = "Stato";
                this.ucEasyGridFiltroStato.Refresh();

                this.ucEasyGridFiltroRisorse.DataSource = CoreStatics.AggiungiTuttiDataTable(ds.Tables[3], true);
                this.ucEasyGridFiltroRisorse.DisplayLayout.Bands[0].Columns["Descrizione"].Header.Caption = "Risorse";
                this.ucEasyGridFiltroRisorse.Refresh();

                this.drFiltro.Value = null;
                this.drFiltro.DateFuture = true;
                this.udteFiltroDA.Value = null;
                this.udteFiltroA.Value = null;

                this.uchkFiltro.Checked = false;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaFiltri", this.Name);
            }

        }

        private void Aggiorna()
        {

            try
            {

                this.CaricaUltraGrid();
                this.AggiornaAppuntamento();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Aggiorna", this.Name);
            }

        }

        private void AggiornaAppuntamento()
        {

            try
            {
                if (ucAnteprimaRtf.IsDisposed == false && ucEasyGrid.IsDisposed == false)
                {
                    if (this.ucEasyGrid.ActiveRow != null && this.ucEasyGrid.ActiveRow.IsDataRow == true)
                    {
                        this.ucAnteprimaRtf.rtbRichTextBox.Rtf = this.ucEasyGrid.ActiveRow.Cells["AnteprimaRTF"].Text;
                    }
                    else
                    {
                        this.ucAnteprimaRtf.rtbRichTextBox.Rtf = string.Empty;
                    }
                }



            }
            catch (Exception)
            {
                this.ucAnteprimaRtf.rtbRichTextBox.Rtf = string.Empty;
            }

        }

        #endregion

        #region UltraGrid

        private void InizializzaUltraGridLayout()
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

        private void CaricaUltraGrid()
        {

            bool bFiltro = false;

            try
            {

                CoreStatics.SetContesto(EnumEntita.APP, null);

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);
                op.Parametro.Add("DatiEstesi", "0");

                if (_ambulatoriale == false)
                {
                    op.Parametro.Add("SoloEpisodio", this.chkSoloEpisodio.Checked == false ? "0" : "1");


                    op.Parametro.Add("IgnoraFiltroCartella", this.chkSoloEpisodio.Checked == false ? "0" : "1");

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

                if (this.ucEasyGridFiltroTipo.ActiveRow != null && this.ucEasyGridFiltroTipo.ActiveRow.Cells["Codice"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                {
                    op.Parametro.Add("CodTipoAppuntamento", this.ucEasyGridFiltroTipo.ActiveRow.Cells["Codice"].Text);
                    bFiltro = true;
                }

                if (this.ucEasyGridFiltroStato.ActiveRow != null && this.ucEasyGridFiltroStato.ActiveRow.Cells["Codice"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                {
                    op.Parametro.Add("CodStatoAppuntamento", this.ucEasyGridFiltroStato.ActiveRow.Cells["Codice"].Text);
                    bFiltro = true;
                }

                if (this.ucEasyGridFiltroRisorse.ActiveRow != null && this.ucEasyGridFiltroRisorse.ActiveRow.Cells["Codice"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                {
                    op.Parametro.Add("CodAgenda", this.ucEasyGridFiltroRisorse.ActiveRow.Cells["Codice"].Text);
                    bFiltro = true;
                }

                this.uchkFiltro.Checked = bFiltro;

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovAppuntamenti", spcoll);

                if (this.ucEasyGrid.DisplayLayout.Bands.Count > 0) { this.ucEasyGrid.DataSource = null; }

                this.ucEasyGrid.DataSource = dt;
                this.ucEasyGrid.Refresh();

                this.ucEasyGrid.PerformAction(UltraGridAction.FirstRowInBand, false, false);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaUltraGrid", this.Name);
            }

        }

        #endregion

        #region Events Form

        private void frmTracker_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void frmTracker_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        #endregion

        #region Events

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

        private void ucEasyGrid_AfterRowActivate(object sender, EventArgs e)
        {
            CoreStatics.SetContesto(EnumEntita.APP, this.ucEasyGrid.ActiveRow);
            this.AggiornaAppuntamento();
        }

        private void ucEasyGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {

                int refWidth = (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XLarge) * 3;

                Graphics g = this.CreateGraphics();
                int refBtnWidth = (CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.ucEasyGrid.DataRowFontRelativeDimension), g.DpiY) * 3);
                g.Dispose();
                g = null;
                e.Layout.Bands[0].HeaderVisible = false;
                e.Layout.Bands[0].ColHeadersVisible = false;
                e.Layout.Bands[0].RowLayoutStyle = RowLayoutStyle.GroupLayout;
                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {

                    oCol.SortIndicator = SortIndicator.Disabled;

                    switch (oCol.Key)
                    {

                        case "DescrData":
                            oCol.CellClickAction = CellClickAction.RowSelect;
                            oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            oCol.Width = refWidth * 2;

                            break;

                        case "ElencoRisorse":
                            oCol.CellClickAction = CellClickAction.RowSelect;
                            oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                            try
                            {
                                oCol.MaxWidth = this.ucEasyGrid.Width - (refWidth * 6) - 30;
                                oCol.MinWidth = oCol.MaxWidth;
                                oCol.Width = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }

                            break;


                        case "Oggetto":
                            oCol.CellClickAction = CellClickAction.RowSelect;
                            oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);

                            oCol.Hidden = true;
                            break;

                        case "DescrTipoAppuntamento":
                            oCol.CellClickAction = CellClickAction.RowSelect;
                            oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                            oCol.Width = refWidth * 2;

                            break;

                        case "DescrStatoAppuntamento":
                            oCol.CellClickAction = CellClickAction.RowSelect;
                            oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                            oCol.Width = refWidth * 2;

                            break;

                        default:
                            oCol.Hidden = true;
                            break;

                    }

                }

            }
            catch (Exception)
            {
            }

        }

        private void ucEasyGridFiltro_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            e.Layout.Bands[0].HeaderVisible = false;

            if (e.Layout.Bands[0].Columns.Exists("Codice") == true)
            {
                e.Layout.Bands[0].Columns["Codice"].Hidden = true;
            }

        }

        private void ubApplicaFiltro_Click(object sender, EventArgs e)
        {
            if (this.ultraDockManager.FlyoutPane != null && !this.ultraDockManager.FlyoutPane.Pinned) this.ultraDockManager.FlyIn();
            this.Aggiorna();
            this.ucEasyGrid.Focus();
        }

        private void uchkFiltro_Click(object sender, EventArgs e)
        {
            if (!this.uchkFiltro.Checked)
            {
                this.uchkFiltro.Checked = !this.uchkFiltro.Checked;
                this.InizializzaFiltri();
                this.Aggiorna();
            }
            else
            {
                this.uchkFiltro.Checked = !this.uchkFiltro.Checked;
            }
        }

        private void drFiltro_ValueChanged(object sender, EventArgs e)
        {
            this.udteFiltroDA.Value = drFiltro.DataOraDa;
            this.udteFiltroA.Value = drFiltro.DataOraA;
        }

        #endregion

    }
}
