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
    public partial class frmSelezionaCartellaCollegabile : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmSelezionaCartellaCollegabile()
        {
            InitializeComponent();
        }

        #region Interface

        public void Carica()
        {

            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_SPOSTA_IN_ALTRA_CARTELLA_256);
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_SPOSTA_IN_ALTRA_CARTELLA_256);

                this.InitializeUltraGrid();
                this.LoadUltraGrid();

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
                                op.TimeStamp.CodRuolo = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice;
                op.TimeStamp.CodLogin = CoreStatics.CoreApplication.Sessione.Utente.Codice;

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);


                DataTable oDt = Database.GetDataTableStoredProc("MSP_SelMovCartelleCollegabili", spcoll);

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

        #endregion

        #region Events

        private void UltraGrid_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            try
            {

                foreach (Infragistics.Win.UltraWinGrid.UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {
                    switch (oCol.Key)
                    {

                        case @"IDEpisodio":
                            oCol.Hidden = true;
                            break;

                        case @"IDTrasferimento":
                            oCol.Hidden = true;
                            break;

                        case @"IDCartella":
                            oCol.Hidden = true;
                            break;

                        case @"IDNum":
                            oCol.Hidden = true;
                            break;

                        case @"NumeroCartella":
                            oCol.Header.Caption = "Numero Cartella";
                            oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                            break;

                        case @"Struttura":
                            oCol.Header.Caption = "Struttura";
                            oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                            break;

                        case @"NumeroEpisodio":
                            oCol.Header.Caption = "Numero Episodio";
                            oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                            break;

                        case @"DataTrasferimento":
                            oCol.Header.Caption = "Data Ingresso";
                            oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                            break;

                        case @"DescrStatoEpisodio":
                            oCol.Header.Caption = "Stato";
                            oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                            break;


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
                    CoreStatics.CoreApplication.EpisodioCollegabileSelezionato = new Episodio(this.UltraGrid.ActiveRow.Cells["IDEpisodio"].Value.ToString());
                    CoreStatics.CoreApplication.TrasferimentoCollegabileSelezionato = new Trasferimento(this.UltraGrid.ActiveRow.Cells["IDTrasferimento"].Value.ToString(), CoreStatics.CoreApplication.Ambiente);
                    CoreStatics.CoreApplication.CartellaCollegabileSelezionata = new Cartella(this.UltraGrid.ActiveRow.Cells["IDCartella"].Value.ToString(), this.UltraGrid.ActiveRow.Cells["NumeroCartella"].Value.ToString(), CoreStatics.CoreApplication.Ambiente);


                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }
                else
                {
                                        easyStatics.EasyMessageBox("Nessuna Cartella Collegabile selezionata.", "Collega Cartella", MessageBoxButtons.OK, MessageBoxIcon.Stop);
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
