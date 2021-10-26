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
    public partial class frmSelezionaUAUO : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmSelezionaUAUO()
        {
            InitializeComponent();
        }

        #region Interface

        public void Carica()
        {

            try
            {
                CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata = "";
                CoreStatics.CoreApplication.AmbulatorialeUADescrizioneSelezionata = "";
                CoreStatics.CoreApplication.PreTrasferimentoUACodiceSelezionata = "";
                CoreStatics.CoreApplication.PreTrasferimentoUADescrizioneSelezionata = "";
                CoreStatics.CoreApplication.PreTrasferimentoUOCodiceSelezionata = "";
                CoreStatics.CoreApplication.PreTrasferimentoUODescrizioneSelezionata = "";
                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_UNITAOPERATIVA_256);
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_UNITAOPERATIVA_16);

                this.InitializeUltraGrid();
                this.LoadUltraGrid();

                                                                if (this.UltraGrid.Rows.Count == 1)
                {
                                        if (CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ID == EnumMaschere.Ambulatoriale_SelezioneUA)
                    {
                                                CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata = this.UltraGrid.Rows[0].Cells["Codice"].Text;
                        CoreStatics.CoreApplication.AmbulatorialeUADescrizioneSelezionata = this.UltraGrid.Rows[0].Cells["Descrizione"].Text;

                        CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.CodUAAmbulatorialeSelezionata = CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata;
                        CoreStatics.CoreApplication.Sessione.Utente.SalvaConfigUtente();
                    }
                    else if (CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ID == EnumMaschere.PreTrasferimento_SelezioneUAUO)
                    {
                                                CoreStatics.CoreApplication.PreTrasferimentoUACodiceSelezionata = this.UltraGrid.Rows[0].Cells["CodUA"].Text;
                        CoreStatics.CoreApplication.PreTrasferimentoUADescrizioneSelezionata = this.UltraGrid.Rows[0].Cells["DescrizioneUA"].Text;

                        CoreStatics.CoreApplication.PreTrasferimentoUOCodiceSelezionata = this.UltraGrid.Rows[0].Cells["CodUO"].Text;
                        CoreStatics.CoreApplication.PreTrasferimentoUODescrizioneSelezionata = this.UltraGrid.Rows[0].Cells["DescrizioneUO"].Text;
                    }

                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }
                else
                {
                                        try
                    {

                        string sCodUA = string.Empty;

                                                if (CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ID == EnumMaschere.Ambulatoriale_SelezioneUA)
                        {

                            if (CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente != null && CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.CodUAAmbulatorialeSelezionata != null && CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.CodUAAmbulatorialeSelezionata.Trim() != "")
                            {
                                sCodUA = CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.CodUAAmbulatorialeSelezionata.ToUpper();
                            }
                        }

                                                if (sCodUA != string.Empty)
                        {
                            for (int r = 0; r < this.UltraGrid.Rows.Count; r++)
                            {
                                if (this.UltraGrid.Rows[r].Cells["CodUA"].Value.ToString().ToUpper() == sCodUA)
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
                if (CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ID == EnumMaschere.Ambulatoriale_SelezioneUA)
                {
                                        op.Parametro.Add("Accesso", "1");
                }
                else if (CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ID == EnumMaschere.PreTrasferimento_SelezioneUAUO)
                {
                                                            op.Parametro.Add("Accesso", "2");
                }

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable oDt = Database.GetDataTableStoredProc("MSP_SelUAUODaRuolo", spcoll);

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

        private void frmSelezionaUAUO_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            try
            {

                if (this.UltraGrid.ActiveRow != null)
                {
                    if (CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ID == EnumMaschere.Ambulatoriale_SelezioneUA)
                    {
                        CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata = this.UltraGrid.ActiveRow.Cells["Codice"].Text;
                        CoreStatics.CoreApplication.AmbulatorialeUADescrizioneSelezionata = this.UltraGrid.ActiveRow.Cells["Descrizione"].Text;

                        CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.CodUAAmbulatorialeSelezionata = CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata;
                        CoreStatics.CoreApplication.Sessione.Utente.SalvaConfigUtente();
                    }
                    else if (CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ID == EnumMaschere.PreTrasferimento_SelezioneUAUO)
                    {
                        CoreStatics.CoreApplication.PreTrasferimentoUACodiceSelezionata = this.UltraGrid.ActiveRow.Cells["CodUA"].Text;
                        CoreStatics.CoreApplication.PreTrasferimentoUADescrizioneSelezionata = this.UltraGrid.ActiveRow.Cells["DescrizioneUA"].Text;
                                                CoreStatics.CoreApplication.PreTrasferimentoUOCodiceSelezionata = this.UltraGrid.ActiveRow.Cells["CodUO"].Text;
                        CoreStatics.CoreApplication.PreTrasferimentoUODescrizioneSelezionata = this.UltraGrid.ActiveRow.Cells["DescrizioneUO"].Text;

                    }

                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "frmSelezionaUAUO_PulsanteAvantiClick", this.Name);
            }
        }

        private void frmSelezionaUAUO_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata = string.Empty;
            CoreStatics.CoreApplication.AmbulatorialeUADescrizioneSelezionata = string.Empty;
            CoreStatics.CoreApplication.PreTrasferimentoUACodiceSelezionata = string.Empty;
            CoreStatics.CoreApplication.PreTrasferimentoUADescrizioneSelezionata = string.Empty;
            CoreStatics.CoreApplication.PreTrasferimentoUOCodiceSelezionata = string.Empty;
            CoreStatics.CoreApplication.PreTrasferimentoUODescrizioneSelezionata = string.Empty;
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion

    }
}
