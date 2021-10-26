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
    public partial class frmSpostaTaskInAltraCartella : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmSpostaTaskInAltraCartella()
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
                op.Parametro.Add("IDCartella", CoreStatics.CoreApplication.Cartella.ID);
                op.TimeStamp.CodRuolo = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice;
                op.TimeStamp.CodLogin = CoreStatics.CoreApplication.Sessione.Utente.Codice;

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);


                DataTable oDt = Database.GetDataTableStoredProc("MSP_SelMovCartelleDaUtente", spcoll);

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
                            oCol.Header.Caption = "DataTrasferimento";
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
                    if (easyStatics.EasyMessageBox("Confermi lo spostamento nella cartella selezionata?", "Sposta Worklist", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                    {
                                                                        string sNoteXTaskDestinazione = "TASK TRASCRITTO DA CARTELLA";
                        if (CoreStatics.CoreApplication.Cartella != null && CoreStatics.CoreApplication.Cartella.NumeroCartella != null)
                            sNoteXTaskDestinazione += @" N. " + CoreStatics.CoreApplication.Cartella.NumeroCartella;
                        if (CoreStatics.CoreApplication.Trasferimento != null && CoreStatics.CoreApplication.Trasferimento.Descrizione != null)
                            sNoteXTaskDestinazione += @" DI " + CoreStatics.CoreApplication.Trasferimento.Descrizione;
                        if (CoreStatics.CoreApplication.Episodio != null && CoreStatics.CoreApplication.Episodio.NumeroEpisodio != null)
                            sNoteXTaskDestinazione += @", EPISODIO " + CoreStatics.CoreApplication.Episodio.NumeroEpisodio;

                        string sNoteXTaskOrigine = "TASK RINVIATO A CARTELLA N. " + this.UltraGrid.ActiveRow.Cells["NumeroCartella"].Value.ToString() +
                                                        " DI " + this.UltraGrid.ActiveRow.Cells["Struttura"].Value.ToString() +
                                                        ", EPISODIO " + this.UltraGrid.ActiveRow.Cells["NumeroEpisodio"].Value.ToString();

                        if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CopiaInStatoTrasmesso(sNoteXTaskOrigine) == true)
                        {
                                                        CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.IDEpisodio = this.UltraGrid.ActiveRow.Cells["IDEpisodio"].Value.ToString();
                            CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.IDTrasferimento = this.UltraGrid.ActiveRow.Cells["IDTrasferimento"].Value.ToString();

                            if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Note != null && CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Note.Trim() != "")
                                CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Note += Environment.NewLine + sNoteXTaskDestinazione;
                            else
                                CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Note = sNoteXTaskDestinazione;

                            CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Salva();
                        }
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                        this.Close();
                    }
                }
                else
                {
                                        easyStatics.EasyMessageBox("Nessuna cartella di destinazione selezionata.", "Sposta Worklist", MessageBoxButtons.OK, MessageBoxIcon.Stop);
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
