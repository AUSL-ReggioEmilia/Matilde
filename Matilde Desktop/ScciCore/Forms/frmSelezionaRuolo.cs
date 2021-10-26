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
using UnicodeSrl.Scci.Enums;
using Infragistics.Win;

namespace UnicodeSrl.ScciCore
{
    public partial class frmSelezionaRuolo : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmSelezionaRuolo()
        {
            InitializeComponent();
        }

        #region Interface

        public void Carica()
        {

            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_RUOLI_256);
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_RUOLI_16);

                this.InitializeUltraGrid();
                this.LoadUltraGrid();

                TopMost = true;
                Focus();
                BringToFront();

                this.ShowDialog();
                if (this.DialogResult == System.Windows.Forms.DialogResult.OK)
                {

                    CoreStatics.CoreApplication.Paziente = null;
                    CoreStatics.CoreApplication.Episodio = null;
                    CoreStatics.CoreApplication.Trasferimento = null;
                    CoreStatics.CoreApplication.Cartella = null;
                    CoreStatics.CoreApplication.Navigazione.Maschere.Reset();
                    CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.MenuPrincipale);

                }

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
                        this.UltraGrid.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Large;
        }

        private void LoadUltraGrid()
        {

            try
            {

                                DataSet oDs = new DataSet();
                DataTable oDt = CoreStatics.CreateDataTable<Ruolo>();
                CoreStatics.FillDataTable<Ruolo>(oDt, CoreStatics.CoreApplication.Sessione.Utente.Ruoli.Elementi);
                oDs.Tables.Add(oDt);

                this.UltraGrid.DataSource = oDt;
                this.UltraGrid.Refresh();

                this.UltraGrid.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);
                this.UltraGrid.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;

                if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato != null)
                {
                    var item = this.UltraGrid.Rows.Single(UltraGridRow => UltraGridRow.Cells["Codice"].Text == CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                    if (item != null) { this.UltraGrid.ActiveRow = item; }
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        #endregion

        #region Events

        private void uteRicerca_ValueChanged(object sender, EventArgs e)
        {

            try
            {

                UltraGridBand band = this.UltraGrid.DisplayLayout.Bands[0];
                band.Override.RowFilterMode = RowFilterMode.AllRowsInBand;

                                band.ColumnFilters["Descrizione"].FilterConditions.Clear();

                if (this.uteRicerca.Text != string.Empty)
                {
                    band.ColumnFilters["Descrizione"].FilterConditions.Add(FilterComparisionOperator.Contains, this.uteRicerca.Text);
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


                e.Layout.Bands[0].ColHeadersVisible = false;

                foreach (UltraGridColumn ocol in e.Layout.Bands[0].Columns)
                {
                    switch (ocol.Key)
                    {
                        case "Descrizione":
                            ocol.Hidden = false;
                            ocol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                            ocol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                            break;

                        default:
                            ocol.Hidden = true;
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        private void frmSelezionaRuolo_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            try
            {

                if (this.UltraGrid.ActiveRow != null)
                {

                    var item = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.Elementi.Single(Ruolo => Ruolo.Codice == this.UltraGrid.ActiveRow.Cells["Codice"].Text);
                    if (item != null)
                    {
                        CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato = item;
                        CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.RuoloSelezionato = item.Codice;
                        CoreStatics.CoreApplication.Ambiente.Codruolo = item.Codice;
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                        this.Close();
                    }

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private void frmSelezionaRuolo_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion

    }
}
