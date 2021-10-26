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
    public partial class frmSelezionaUO : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmSelezionaUO()
        {
            InitializeComponent();
        }

        #region Interface

        public void Carica()
        {

            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_UNITAOPERATIVA_256);
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_UNITAOPERATIVA_16);

                this.InitializeUltraGrid();

                if (this.LoadUltraGrid()) this.ShowDialog();

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

        private bool LoadUltraGrid()
        {
            bool bLoaded = true;
            try
            {

                                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodAzi", UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.Azienda));
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);

                                if (CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata != null && CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata.Trim() != "")
                    op.Parametro.Add("CodUA", CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable oDt = Database.GetDataTableStoredProc("MSP_SelUODaRuolo", spcoll);

                this.UltraGrid.DataSource = oDt;
                this.UltraGrid.Refresh();

                this.UltraGrid.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);
                this.UltraGrid.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;

                if (this.UltraGrid.Rows.Count == 1)
                {
                                        this.UltraGrid.ActiveRow = this.UltraGrid.Rows[0];
                    frmSelezionaUO_PulsanteAvantiClick(this, new PulsanteBottomClickEventArgs(EnumPulsanteBottom.Avanti));
                    bLoaded = false;
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "LoadUltraGrid", this.Name);
            }
            return bLoaded;
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

        private void frmSelezionaUO_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            try
            {

                if (this.UltraGrid.ActiveRow != null)
                {
                    CoreStatics.CoreApplication.CodUOSelezionata = this.UltraGrid.ActiveRow.Cells["CodUO"].Text;
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "frmSelezionaUO_PulsanteAvantiClick", this.Name);
            }
        }

        private void frmSelezionaUO_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            CoreStatics.CoreApplication.CodUOSelezionata = string.Empty;
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion

    }
}
