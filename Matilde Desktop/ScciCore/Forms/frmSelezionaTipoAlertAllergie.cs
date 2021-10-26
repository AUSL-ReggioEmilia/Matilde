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
    public partial class frmSelezionaTipoAlertAllergie : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmSelezionaTipoAlertAllergie()
        {
            InitializeComponent();
        }

        #region Interface

        public void Carica()
        {

            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_ALERTALLERGIA_256);
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_ALERTALLERGIA_16);

                this.InitializeUltraGrid();
                this.LoadUltraGrid();

                this.ShowDialog();
                if (this.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
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
                op.Parametro.Add("DatiEstesi", "1");
                op.Parametro.Add("CodUA", CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.CodUA);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                op.Parametro.Add("CodAzione", CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.Azione.ToString());

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable oDt = Database.GetDataTableStoredProc("MSP_SelTipoAlertAllergiaAnamnesi", spcoll);

                Graphics g = this.CreateGraphics();
                int icondimensions = CoreStatics.PointToPixel(this.UltraGrid.DisplayLayout.Override.CellAppearance.FontData.SizeInPoints, g.DpiX);
                g.Dispose();
                g = null;
                DataTable odtGrid = new DataTable();
                odtGrid.Columns.Add("Codice", typeof(String));
                odtGrid.Columns.Add("IconaSmall", typeof(Byte[]));
                odtGrid.Columns.Add("Descrizione", typeof(String));
                odtGrid.Columns.Add("CodScheda", typeof(String));

                foreach (DataRow odr in oDt.Rows)
                {
                    DataRow odrgrid = odtGrid.NewRow();
                    odrgrid["Codice"] = odr["Codice"];
                    odrgrid["IconaSmall"] = DrawingProcs.GetByteFromImage(DrawingProcs.GetImageFromByte(odr["Icona"], new Size(icondimensions, icondimensions), true)); ;
                    odrgrid["Descrizione"] = odr["Descrizione"];
                    odrgrid["CodScheda"] = odr["CodScheda"];
                    odtGrid.Rows.Add(odrgrid);
                }


                this.UltraGrid.DataSource = odtGrid;
                this.UltraGrid.Refresh();

                this.UltraGrid.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);
                this.UltraGrid.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;

                if (CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.CodTipo != string.Empty && CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.CodTipo != "")
                {
                    var item = this.UltraGrid.Rows.Single(UltraGridRow => UltraGridRow.Cells["Codice"].Text == CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.CodTipo);
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

                        case "IconaSmall":
                            ocol.Hidden = false;
                            ocol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                            ocol.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                            ocol.CellAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
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

        private void frmSelezionaTipoAlertAllergie_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            try
            {

                if (this.UltraGrid.ActiveRow != null)
                {
                    CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.CodTipo = this.UltraGrid.ActiveRow.Cells["Codice"].Text;
                    CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.DescrTipo = this.UltraGrid.ActiveRow.Cells["Descrizione"].Text;
                    CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.CodScheda = this.UltraGrid.ActiveRow.Cells["CodScheda"].Text;
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private void frmSelezionaTipoAlertAllergie_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion

    }
}
