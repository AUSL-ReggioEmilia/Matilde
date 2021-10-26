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
    public partial class frmSelezionaUAConsegne : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmSelezionaUAConsegne()
        {
            InitializeComponent();
        }

        #region Interface

        public void Carica()
        {

            try
            {
                CoreStatics.CoreApplication.ConsegneUACodiceSelezionata = "";
                CoreStatics.CoreApplication.ConsegneUADescrizioneSelezionata = "";
                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_UNITAOPERATIVA_256);
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_UNITAOPERATIVA_16);

                this.InitializeUltraGrid();
                this.LoadUltraGrid();

                                                                if (this.UltraGrid.Rows.Count == 1)
                {
                                        if (CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ID == EnumMaschere.Consegne_SelezioneUA)
                    {
                                                CoreStatics.CoreApplication.ConsegneUACodiceSelezionata = this.UltraGrid.Rows[0].Cells["Codice"].Text;
                        CoreStatics.CoreApplication.ConsegneUADescrizioneSelezionata = this.UltraGrid.Rows[0].Cells["Descrizione"].Text;

                                                CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.ConsegneCodUASelezionata = CoreStatics.CoreApplication.ConsegneUACodiceSelezionata;
                        CoreStatics.CoreApplication.Sessione.Utente.SalvaConfigUtente();
                    }

                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }
                else
                {
                                        try
                    {

                        string sCodUA = string.Empty;

                                                if (CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ID == EnumMaschere.Consegne_SelezioneUA)
                        {

                            if (CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente != null &&
                                CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.ConsegneCodUASelezionata != null &&
                                CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.ConsegneCodUASelezionata.Trim() != "")
                            {
                                sCodUA = CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.ConsegneCodUASelezionata.ToUpper();
                            }
                        }

                                                if (sCodUA != string.Empty)
                        {
                            for (int r = 0; r < this.UltraGrid.Rows.Count; r++)
                            {
                                if (this.UltraGrid.Rows[r].Cells["Codice"].Value.ToString().ToUpper() == sCodUA)
                                {

                                                                        this.UltraGrid.ActiveRow = this.UltraGrid.Rows[r];
                                    this.UltraGrid.Selected.Rows.Clear();

                                    this.UltraGrid.Selected.Rows.Add(this.UltraGrid.Rows[r]);

                                                                        r = this.UltraGrid.Rows.Count + 1;
                                }
                            }
                        }
                    }
                    catch
                    {
                    }

                                        this.ShowDialog();
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

                                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);

                if ((CoreStatics.CoreApplication.MovConsegnaSelezionata != null) &&
                  (CoreStatics.CoreApplication.MovConsegnaSelezionata.CodTipoConsegna != string.Empty))
                {
                    op.Parametro.Add("CodTipoConsegna", CoreStatics.CoreApplication.MovConsegnaSelezionata.CodTipoConsegna);
                }

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
              
                DataTable oDt = Database.GetDataTableStoredProc("MSP_SelUADaRuoloConsegne", spcoll);

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
                CoreStatics.ExGest(ref ex, "UltraGrid_InitializeLayout", this.Name);
            }

        }

        private void frmSelezionaUAConsegne_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            try
            {

                if (this.UltraGrid.ActiveRow != null)
                {
                    if (CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ID == EnumMaschere.Consegne_SelezioneUA)
                    {
                        CoreStatics.CoreApplication.ConsegneUACodiceSelezionata = this.UltraGrid.ActiveRow.Cells["Codice"].Text;
                        CoreStatics.CoreApplication.ConsegneUADescrizioneSelezionata = this.UltraGrid.ActiveRow.Cells["Descrizione"].Text;

                        CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.ConsegneCodUASelezionata = CoreStatics.CoreApplication.ConsegneUACodiceSelezionata;
                        CoreStatics.CoreApplication.Sessione.Utente.SalvaConfigUtente();
                    }

                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "frmSelezionaUA_PulsanteAvantiClick", this.Name);
            }
        }

        private void frmSelezionaUAConsegne_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            CoreStatics.CoreApplication.ConsegneUACodiceSelezionata = string.Empty;
            CoreStatics.CoreApplication.ConsegneUADescrizioneSelezionata = string.Empty;
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion

    }
}
