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
    public partial class frmSelezionaTipoConsegna : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmSelezionaTipoConsegna()
        {
            InitializeComponent();
        }

        #region Interface

        public void Carica()
        {

            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_CONSEGNE_256);
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_CONSEGNE_16);

                setControlliCopiaDaPrecedente(true);

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
                        this.UltraGrid.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Large;
        }

        private void LoadUltraGrid()
        {

            try
            {

                                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodUA", CoreStatics.CoreApplication.MovConsegnaSelezionata.CodUA);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                op.Parametro.Add("CodAzione", CoreStatics.CoreApplication.MovConsegnaSelezionata.Azione.ToString());

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable oDt = Database.GetDataTableStoredProc("MSP_SelTipoConsegna", spcoll);

                this.UltraGrid.DataSource = oDt;
                this.UltraGrid.Refresh();

                this.UltraGrid.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);
                this.UltraGrid.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;

                if (CoreStatics.CoreApplication.MovConsegnaSelezionata.CodTipoConsegna != string.Empty)
                {
                    var item = this.UltraGrid.Rows.Single(UltraGridRow => UltraGridRow.Cells["Codice"].Text == CoreStatics.CoreApplication.MovConsegnaSelezionata.CodTipoConsegna);
                    if (item != null) { this.UltraGrid.ActiveRow = item; }
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void setControlliCopiaDaPrecedente(bool abilita)
        {
            this.uceCopiaDaPrecedente.Checked = abilita;
            this.uceCopiaDaPrecedente.Enabled = abilita;
            this.lblCopiaDaPrecedente.Enabled = this.uceCopiaDaPrecedente.Enabled;

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

        private void frmSelezionaTipConsegna_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {            
            if (this.UltraGrid.ActiveRow != null)
            {

                CoreStatics.CoreApplication.MovConsegnaSelezionata.CodTipoConsegna = this.UltraGrid.ActiveRow.Cells["Codice"].Text;
                CoreStatics.CoreApplication.MovConsegnaSelezionata.DescrTipoConsegna = this.UltraGrid.ActiveRow.Cells["Descrizione"].Text;
                CoreStatics.CoreApplication.MovConsegnaSelezionata.CodScheda = this.UltraGrid.ActiveRow.Cells["CodScheda"].Text;                

                if (this.uceCopiaDaPrecedente.Checked == true)
                {

                    switch (CoreStatics.CoreApplication.MovConsegnaSelezionata.PopolaDaPrecedente())
                    {
                        case MovConsegna.enumPopolaDaPrecedenteReturn.precedente_non_trovato:
                            easyStatics.EasyMessageBox("Non è stata trovata una Consegna precedente da copiare.", "Nuova Consegna", MessageBoxButtons.OK, MessageBoxIcon.Warning);                     
                            break;
                        case MovConsegna.enumPopolaDaPrecedenteReturn.errori:                            
                            break;
                        default:                            
                            break;
                    }

                }

                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();

            }

        }

        private void frmSelezionaTipoConsegna_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion

    }
}
