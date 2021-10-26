using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.ScciResource;
using Infragistics.Win.UltraWinGrid;

using Microsoft.Data.SqlClient;
using System.Globalization;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;

namespace UnicodeSrl.ScciCore
{
    public partial class frmSelezionaCartella : frmBaseModale, Interfacce.IViewFormlModal
    {

        private bool b_manuale = false;

        public frmSelezionaCartella()
        {
            InitializeComponent();
        }

        #region Interface

        public void Carica()
        {

            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_CARTELLACLINICA_256);
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_CARTELLACLINICA_256);

                this.InitializeUltraGrid();
                this.LoadUltraGrid();

                this.ShowDialog();

            }
            catch (Exception)
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }


        }
        public void Carica(DataTable dt)
        {

            b_manuale = true;

            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_CARTELLACLINICA_256);
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_CARTELLACLINICA_256);
                this.PulsanteIndietroVisibile = false;

                this.InitializeUltraGrid();
                this.LoadUltraGrid(dt);

                this.ShowDialog();

            }
            catch (Exception)
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }


        }
        #endregion

        #region UltraGrid

        private void InitializeUltraGrid()
        {
            this.UltraGrid.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Medium;

        }

        private void LoadUltraGrid()
        {

            try
            {

                                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                op.Parametro.Add("FiltroGenerico", CoreStatics.CoreApplication.Sessione.Nosologico);

                                op.Parametro.Add("Ordinamento", "[Data Ingresso Data Ricovero] DESC");


                                                
                                op.Parametro.Add("CodStatoCartella", "AP");                 
                                op.Parametro.Add("SoloUltimoTrasferimentoCartella", "1");

                                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable oDt = Database.GetDataTableStoredProc("MSP_CercaEpisodio", spcoll);

                this.UltraGrid.DataSource = oDt;
                this.UltraGrid.Refresh();

                this.UltraGrid.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);
                this.UltraGrid.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "LoadUltraGrid", this.Name);
            }

        }
        private void LoadUltraGrid(DataTable dt)
        {

            try
            {

                this.UltraGrid.DataSource = dt;
                this.UltraGrid.Refresh();

                this.UltraGrid.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);
                this.UltraGrid.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "LoadUltraGrid", this.Name);
            }

        }

        #endregion

        #region Events

        private void UltraGrid_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {

                if (b_manuale)
                {

                    foreach (Infragistics.Win.UltraWinGrid.UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                    {

                        switch (oCol.Key)
                        {

                            case "NumeroCartella":
                                oCol.Header.Caption = "Numero Cartella";
                                oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                                break;

                            case "DataApertura":
                                oCol.Header.Caption = "Data Apertura";
                                oCol.Format = "dd/MM/yyyy HH:mm";
                                oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                                break;

                            default:
                                oCol.Hidden = true;
                                break;

                        }

                    }

                }
                else
                {

                    foreach (Infragistics.Win.UltraWinGrid.UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                    {

                        switch (oCol.Key)
                        {

                            case "Paziente":
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                                break;

                            case "DescrUA":
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.Header.Caption = "Struttura";
                                oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                                break;

                            case "UO - Settore":
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                                break;

                            case "Data Ingresso Data Ricovero":
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.Header.Caption = @"Data Ingresso/" + Environment.NewLine + @"Data Ricovero";
                                oCol.Format = "dd/MM/yyyy";
                                oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                                break;

                            case "DecrStato":
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.Header.Caption = @"Stato";
                                oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                                break;

                            case "DescStanzaLetto":
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.Header.Caption = @"Stanza / Letto";
                                oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                                break;

                            case "DescEpisodio":
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.Header.Caption = @"Tipo Episodio / " + Environment.NewLine + @"Nosologico";
                                oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                                break;

                            case "NumeroCartella":
                                oCol.Header.Caption = "Cartella";
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                                break;

                            case "NA":
                                oCol.Header.Caption = @"";
                                oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                                oCol.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                                break;

                            case "NAG":
                                oCol.Header.Caption = @"";
                                oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                                oCol.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                                break;

                            case "NEC":
                                oCol.Header.Caption = @"";
                                oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                                oCol.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                                break;

                            case "CIV":
                                oCol.Header.Caption = @"";
                                oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                                oCol.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                                break;

                            default:
                                oCol.Hidden = true;
                                break;

                        }

                    }

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        private void frmSelezionaCartella_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            try
            {

                if (this.UltraGrid.ActiveRow != null)
                {

                    if (!b_manuale) { CoreStatics.CoreApplication.Sessione.NosologicoIndex = this.UltraGrid.ActiveRow.Index; }

                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();

                }
                else
                {
                                        easyStatics.EasyMessageBox("Nessuna Cartella selezionata.", "Collega Cartella", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private void frmSelezionaCartella_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion

    }
}
