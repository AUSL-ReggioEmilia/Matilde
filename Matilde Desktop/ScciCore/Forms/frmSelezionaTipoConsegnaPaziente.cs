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
    public partial class frmSelezionaTipoConsegnaPaziente : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmSelezionaTipoConsegnaPaziente()
        {
            InitializeComponent();
        }

        #region Interface

        public new void Carica()
        {

            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_CONSEGNEPAZIENTE_256);
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_CONSEGNEPAZIENTE_256);

                this.InitializeUltraGrid();
                this.LoadUltraGrid();

                if (this.UltraGrid.Rows.Count == 1)
                {
                    this.UltraGrid.ActiveRow = this.UltraGrid.Rows[0];
                    frmSelezionaTipoConsegnaPaziente_PulsanteAvantiClick(this, new PulsanteBottomClickEventArgs(EnumPulsanteBottom.Avanti));
                }
                else
                {
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
                op.Parametro.Add("CodUA", CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.CodUA);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                op.Parametro.Add("CodAzione", CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.Azione.ToString());

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                FwDataParametersList procParams = new FwDataParametersList();
                procParams.Add("xParametri", xmlParam, ParameterDirection.Input, DbType.Xml);

                using (FwDataConnection conn = new FwDataConnection(Database.ConnectionString))
                {
                    DataTable oDt = conn.Query<DataTable>("MSP_SelTipoConsegnaPaziente", procParams, CommandType.StoredProcedure);
                    this.UltraGrid.DataSource = oDt;
                }

                this.UltraGrid.Refresh();

                this.UltraGrid.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);
                this.UltraGrid.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
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

        private void frmSelezionaTipoConsegnaPaziente_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {

            if (this.UltraGrid.ActiveRow != null)
            {

                CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.CodTipoConsegnaPaziente = this.UltraGrid.ActiveRow.Cells["Codice"].Text;
                CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.DescrTipoConsegnaPaziente = this.UltraGrid.ActiveRow.Cells["Descrizione"].Text;
                CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.CodScheda = this.UltraGrid.ActiveRow.Cells["CodScheda"].Text;

                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();

            }

        }

        private void frmSelezionaTipoConsegnaPaziente_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion

    }
}
