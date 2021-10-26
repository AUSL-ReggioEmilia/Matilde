using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.ScciResource;

namespace UnicodeSrl.ScciCore
{
    public partial class frmPUOrdineVisualizza : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmPUOrdineVisualizza()
        {
            InitializeComponent();
        }

        public void Carica()
        {
            try
            {
                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_ORDINE_256);

                
                                this.InizializzaUltraGridLayout();

                                CoreStatics.SetEasyUltraDockManager(ref this.ultraDockManager);

                                this.CaricaOrdine();

                                this.ShowDialog();
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "Carica", this.Text);
            }
        }

        #region private functions

        private void InizializzaUltraGridLayout()
        {
            try
            {
                CoreStatics.SetEasyUltraGridLayout(ref this.ucEasyGridPrestazioni);
                CoreStatics.SetEasyUltraGridLayout(ref this.ucEasyGridDatiErogante);
                CoreStatics.SetEasyUltraGridLayout(ref this.ucEasyGridDatiRichiedente);
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaUltraGridLayout", this.Name);
            }
        }

        private void CaricaOrdine()
        {


            try
            {
                                this.lblEroganti.Text = CoreStatics.CoreApplication.MovOrdineSelezionato.Eroganti;

                this.lblStatoErogante.Text = CoreStatics.CoreApplication.MovOrdineSelezionato.StatoOrdine.ToString();

                this.lblNumeroOrdine.Text = CoreStatics.CoreApplication.MovOrdineSelezionato.NumeroOrdine.ToString();

                DateTime dataprogrammazione = DateTime.Parse(CoreStatics.CoreApplication.MovOrdineSelezionato.DataProgrammazione.ToString());
                this.lblDataOraProgrammazione.Text = (dataprogrammazione != DateTime.MinValue ? dataprogrammazione.ToString("dd/MM/yyyy HH:mm") : string.Empty);

                this.lblPriorita.Text = CoreStatics.GetEnumDescription(CoreStatics.CoreApplication.MovOrdineSelezionato.Priorita);

                DateTime datainoltro = DateTime.Parse(CoreStatics.CoreApplication.MovOrdineSelezionato.DataInoltro.ToString());
                this.lblUtenteInoltro.Text = CoreStatics.CoreApplication.MovOrdineSelezionato.UtenteInoltro + " - " +
                                            (datainoltro != DateTime.MinValue ? datainoltro.ToString("dd/MM/yyyy HH:mm") : string.Empty);

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "CaricaOrdine", this.Text);
            }
        }

        private void CaricaGriglie()
        {
            try
            {
                                this.ucEasyGridPrestazioni.DisplayLayout.Bands[0].Columns.ClearUnbound();
                
                                this.ucEasyGridPrestazioni.DataSource =
                    CoreStatics.CoreApplication.MovOrdineSelezionato.CaricaPrestazioniSelezionate(true);

                this.ucEasyGridPrestazioni.Refresh();

                                this.ucEasyGridDatiRichiedente.DisplayLayout.Bands[0].Columns.ClearUnbound();
                this.ucEasyGridDatiRichiedente.DataSource =
                    CoreStatics.CoreApplication.MovOrdineSelezionato.CaricaDatiAggiuntiviStato();
                this.ucEasyGridDatiRichiedente.Refresh();

                                this.ucEasyGridDatiErogante.DisplayLayout.Bands[0].Columns.ClearUnbound();
                this.ucEasyGridDatiErogante.DataSource =
                    CoreStatics.CoreApplication.MovOrdineSelezionato.CaricaDatiAggiuntiviErogante();
                this.ucEasyGridDatiErogante.Refresh();
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "CaricaGriglie", this.Text);
            }
        }

        #endregion

        #region Events

        private void frmPUOrdineVisualizza_Shown(object sender, EventArgs e)
        {
            this.CaricaGriglie();
        }

        private void ultraDockManager_InitializePane(object sender, Infragistics.Win.UltraWinDock.InitializePaneEventArgs e)
        {
            e.Pane.Settings.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large);
            e.Pane.Settings.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_ORDINE_32);

            int filtroWidth = 40 * (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large);
            this.ultraDockManager.ControlPanes[0].FlyoutSize = new Size(filtroWidth, this.ultraDockManager.ControlPanes[0].FlyoutSize.Height);
            this.ultraDockManager.ControlPanes[0].Size = new Size(filtroWidth, this.ultraDockManager.ControlPanes[0].Size.Height);
            this.ultraDockManager.DockAreas[0].Size = new Size(filtroWidth, this.ultraDockManager.DockAreas[0].Size.Height);
            this.pnlDatiErogante.Width = filtroWidth;
        }

        private void ucEasyGridPrestazioni_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            int refWidth = Convert.ToInt32(this.ucEasyGridPrestazioni.Width / 11);

            if (e.Layout.Bands[0].Columns.Exists("CodErogante"))
            {
                e.Layout.Bands[0].Columns["CodErogante"].Hidden = true;
            }
            if (e.Layout.Bands[0].Columns.Exists("DescErogante"))
            {
                e.Layout.Bands[0].Columns["DescErogante"].Hidden = false;
                e.Layout.Bands[0].Columns["DescErogante"].Header.Caption = "Erogante";
                try
                {
                    e.Layout.Bands[0].Columns["DescErogante"].MaxWidth = Convert.ToInt32(refWidth * 2);
                    e.Layout.Bands[0].Columns["DescErogante"].MinWidth = e.Layout.Bands[0].Columns["DescErogante"].MaxWidth;
                    e.Layout.Bands[0].Columns["DescErogante"].Width = e.Layout.Bands[0].Columns["DescErogante"].MaxWidth;
                    e.Layout.Bands[0].Columns["DescErogante"].CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                }
                catch (Exception)
                {
                }
            }
            if (e.Layout.Bands[0].Columns.Exists("Descrizione"))
            {
                e.Layout.Bands[0].Columns["Descrizione"].Hidden = false;
                e.Layout.Bands[0].Columns["Descrizione"].Header.Caption = "Prestazione";
                try
                {
                    e.Layout.Bands[0].Columns["Descrizione"].MaxWidth = Convert.ToInt32(refWidth * 5) - 30;
                    e.Layout.Bands[0].Columns["Descrizione"].MinWidth = e.Layout.Bands[0].Columns["Descrizione"].MaxWidth;
                    e.Layout.Bands[0].Columns["Descrizione"].Width = e.Layout.Bands[0].Columns["Descrizione"].MaxWidth;
                    e.Layout.Bands[0].Columns["Descrizione"].CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                }
                catch (Exception)
                {
                }
            }
            if (e.Layout.Bands[0].Columns.Exists("AziErogante"))
            {
                e.Layout.Bands[0].Columns["AziErogante"].Hidden = true;
            }
            if (e.Layout.Bands[0].Columns.Exists("DescAziErogante"))
            {
                e.Layout.Bands[0].Columns["DescAziErogante"].Hidden = true;
            }
            if (e.Layout.Bands[0].Columns.Exists("Codice"))
            {
                e.Layout.Bands[0].Columns["Codice"].Hidden = true;
            }
            if (e.Layout.Bands[0].Columns.Exists("Tipo"))
            {
                e.Layout.Bands[0].Columns["Tipo"].Hidden = true;
            }
            if (e.Layout.Bands[0].Columns.Exists("Stato"))
            {
                e.Layout.Bands[0].Columns["Stato"].Hidden = false;
                try
                {
                    e.Layout.Bands[0].Columns["Stato"].MaxWidth = Convert.ToInt32(refWidth * 2);
                    e.Layout.Bands[0].Columns["Stato"].MinWidth = e.Layout.Bands[0].Columns["Stato"].MaxWidth;
                    e.Layout.Bands[0].Columns["Stato"].Width = e.Layout.Bands[0].Columns["Stato"].MaxWidth;
                    e.Layout.Bands[0].Columns["Stato"].CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                }
                catch (Exception)
                {
                }
            }

            if (e.Layout.Bands[0].Columns.Exists("DataPianificata"))
            {
                e.Layout.Bands[0].Columns["DataPianificata"].Hidden = false;
                e.Layout.Bands[0].Columns["DataPianificata"].Header.Caption = "Data Pian.";
                e.Layout.Bands[0].Columns["DataPianificata"].Format = @"dd/MM/yyyy HH:mm";
                e.Layout.Bands[0].Columns["DataPianificata"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                try
                {
                    e.Layout.Bands[0].Columns["DataPianificata"].MaxWidth = refWidth*2;
                    e.Layout.Bands[0].Columns["DataPianificata"].MinWidth = e.Layout.Bands[0].Columns["DataPianificata"].MaxWidth;
                    e.Layout.Bands[0].Columns["DataPianificata"].Width = e.Layout.Bands[0].Columns["DataPianificata"].MaxWidth;
                    e.Layout.Bands[0].Columns["Stato"].CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                }
                catch (Exception)
                {
                }
            }
        }

        private void ucEasyGridDatiRichiedente_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            int refWidth = Convert.ToInt32(this.ucEasyGridDatiRichiedente.Width / 10);

            if (e.Layout.Bands[0].Columns.Exists("Codice"))
            {
                e.Layout.Bands[0].Columns["Codice"].Hidden = true;
            }
            if (e.Layout.Bands[0].Columns.Exists("Descrizione"))
            {
                e.Layout.Bands[0].Columns["Descrizione"].Hidden = true;
            }
            if (e.Layout.Bands[0].Columns.Exists("Etichetta"))
            {
                e.Layout.Bands[0].Columns["Etichetta"].Hidden = false;
                try
                {
                    e.Layout.Bands[0].Columns["Etichetta"].MaxWidth = Convert.ToInt32(refWidth * 5);
                    e.Layout.Bands[0].Columns["Etichetta"].MinWidth = e.Layout.Bands[0].Columns["Etichetta"].MaxWidth;
                    e.Layout.Bands[0].Columns["Etichetta"].Width = e.Layout.Bands[0].Columns["Etichetta"].MaxWidth;
                }
                catch (Exception)
                {
                }
            }
            if (e.Layout.Bands[0].Columns.Exists("Gruppo"))
            {
                e.Layout.Bands[0].Columns["Gruppo"].Hidden = true;
            }
            if (e.Layout.Bands[0].Columns.Exists("Obbligatorio"))
            {
                e.Layout.Bands[0].Columns["Obbligatorio"].Hidden = true;
            }
            if (e.Layout.Bands[0].Columns.Exists("Ordinamento"))
            {
                e.Layout.Bands[0].Columns["Ordinamento"].Hidden = true;
            }
            if (e.Layout.Bands[0].Columns.Exists("Ripetibile"))
            {
                e.Layout.Bands[0].Columns["Ripetibile"].Hidden = true;
            }
            if (e.Layout.Bands[0].Columns.Exists("Tipo"))
            {
                e.Layout.Bands[0].Columns["Tipo"].Hidden = true;
            }
            if (e.Layout.Bands[0].Columns.Exists("Valori"))
            {
                e.Layout.Bands[0].Columns["Valori"].Hidden = true;
            }
            if (e.Layout.Bands[0].Columns.Exists("ValidazioneMessaggio"))
            {
                e.Layout.Bands[0].Columns["ValidazioneMessaggio"].Hidden = true;
            }
            if (e.Layout.Bands[0].Columns.Exists("ValidazioneRegEx"))
            {
                e.Layout.Bands[0].Columns["ValidazioneRegEx"].Hidden = true;
            }
            if (e.Layout.Bands[0].Columns.Exists("Testata"))
            {
                e.Layout.Bands[0].Columns["Testata"].Hidden = true;
            }
            if (e.Layout.Bands[0].Columns.Exists("Valore"))
            {
                e.Layout.Bands[0].Columns["Valore"].Hidden = true;
            }
            if (e.Layout.Bands[0].Columns.Exists("DescrizioneValore"))
            {
                e.Layout.Bands[0].Columns["DescrizioneValore"].Hidden = false;
                e.Layout.Bands[0].Columns["DescrizioneValore"].Header.Caption = "Dato";
                try
                {
                    e.Layout.Bands[0].Columns["DescrizioneValore"].MaxWidth = Convert.ToInt32(refWidth * 5) - 30;
                    e.Layout.Bands[0].Columns["DescrizioneValore"].MinWidth = e.Layout.Bands[0].Columns["DescrizioneValore"].MaxWidth;
                    e.Layout.Bands[0].Columns["DescrizioneValore"].Width = e.Layout.Bands[0].Columns["DescrizioneValore"].MaxWidth;
                }
                catch (Exception)
                {
                }
            }
            if (e.Layout.Bands[0].Columns.Exists("Ripetizione"))
            {
                e.Layout.Bands[0].Columns["Ripetizione"].Hidden = true;
            }
        }

        private void ucEasyGridDatiErogante_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            int refWidth = Convert.ToInt32(this.ucEasyGridDatiErogante.Width / 10);

            if (e.Layout.Bands[0].Columns.Exists("Codice"))
            {
                e.Layout.Bands[0].Columns["Codice"].Hidden = false;
                try
                {
                    e.Layout.Bands[0].Columns["Codice"].MaxWidth = Convert.ToInt32(refWidth * 4);
                    e.Layout.Bands[0].Columns["Codice"].MinWidth = e.Layout.Bands[0].Columns["Codice"].MaxWidth;
                    e.Layout.Bands[0].Columns["Codice"].Width = e.Layout.Bands[0].Columns["Codice"].MaxWidth;
                }
                catch (Exception)
                {
                }
            }
            if (e.Layout.Bands[0].Columns.Exists("Valore"))
            {
                e.Layout.Bands[0].Columns["Valore"].Hidden = false;
                try
                {
                    e.Layout.Bands[0].Columns["Valore"].MaxWidth = Convert.ToInt32(refWidth * 5) - 30;
                    e.Layout.Bands[0].Columns["Valore"].MinWidth = e.Layout.Bands[0].Columns["Valore"].MaxWidth;
                    e.Layout.Bands[0].Columns["Valore"].Width = e.Layout.Bands[0].Columns["Valore"].MaxWidth;
                }
                catch (Exception)
                {
                }
            }
            if (e.Layout.Bands[0].Columns.Exists("Ripetizione"))
            {
                e.Layout.Bands[0].Columns["Ripetizione"].Hidden = true;
            }
            if (e.Layout.Bands[0].Columns.Exists("Tipo"))
            {
                e.Layout.Bands[0].Columns["Tipo"].Hidden = true;
            }
            if (e.Layout.Bands[0].Columns.Exists("PDF"))
            {
                e.Layout.Bands[0].Columns["PDF"].Hidden = true;
            }

            if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_GRAPH))
            {
                UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_GRAPH);
                colEdit.Hidden = false;
                colEdit.Header.Caption = string.Empty;

                                colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                colEdit.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
                colEdit.CellActivation = Activation.AllowEdit;
                colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_DOWNLOADDOCUMENTO_32);

                colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                try
                {
                    colEdit.MinWidth = Convert.ToInt32(refWidth);
                    colEdit.MaxWidth = colEdit.MinWidth;
                    colEdit.Width = colEdit.MaxWidth;
                }
                catch (Exception)
                {
                }
                colEdit.LockedWidth = true;
            }

        }

        private void ucEasyGridDatiErogante_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            try
            {
                foreach (UltraGridCell ocell in e.Row.Cells)
                {

                    if (ocell.Column.Key == CoreStatics.C_COL_BTN_GRAPH &&
                        (ocell.Row.Cells["Tipo"].Value.ToString() != "xs:base64Binary" ||
                        ocell.Row.Cells["PDF"].Value.ToString() == string.Empty))
                        ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGridDatiErogante_InitializeRow", this.Name);
            }
        }

        private void ucEasyGridDatiErogante_ClickCellButton(object sender, CellEventArgs e)
        {
            try
            {

                switch (e.Cell.Column.Key)
                {
                    case CoreStatics.C_COL_BTN_GRAPH:
                        
                        byte[] documento = Convert.FromBase64String(e.Cell.Row.Cells["PDF"].Value.ToString());
                        string snewfilename = System.IO.Path.Combine(FileStatics.GetSCCITempPath() + DateTime.Now.ToString("yyyyMMddHHmmss") + @".pdf");

                        if (UnicodeSrl.Framework.Utility.FileSystem.ByteArrayToFile(snewfilename, ref documento))
                        {
                            if (System.IO.File.Exists(snewfilename))
                            {
                                easyStatics.ShellExecute(snewfilename, "");
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGridDatiErogante_ClickCellButton", this.Name);
            }
        }

        private void frmPUOrdineVisualizza_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            try
            {
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                this.Close();
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        #endregion


    }
}
