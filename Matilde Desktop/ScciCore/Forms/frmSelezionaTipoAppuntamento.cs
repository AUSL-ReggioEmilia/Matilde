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
    public partial class frmSelezionaTipoAppuntamento : frmBaseModale, Interfacce.IViewFormlModal
    {
                private bool _skipShowForm = false;

        public frmSelezionaTipoAppuntamento()
        {
            InitializeComponent();
        }

                                private List<String> XpFiltro_CodTipoApp { get; set; }

        #region Interface

        public void Carica()
        {

            try
            {
                                if ((this.CustomParamaters != null) && (this.CustomParamaters is List<String>))
                {
                    this.XpFiltro_CodTipoApp = this.CustomParamaters as List<String>;
                }

                _skipShowForm = false;
                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_APPUNTAMENTO_256);
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_APPUNTAMENTO_16);

                this.InitializeUltraGrid();
                this.LoadUltraGrid();

                if (!_skipShowForm) this.ShowDialog();

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
                if (CoreStatics.CoreApplication.Trasferimento != null)
                {
                    op.Parametro.Add("CodUA", CoreStatics.CoreApplication.Trasferimento.CodUA);
                }
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                op.Parametro.Add("CodAzione", CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Azione.ToString());
                if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodAgendaPartenza != string.Empty)
                {
                    op.Parametro.Add("CodAgendaPartenza", CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodAgendaPartenza);
                }
                op.Parametro.Add("DatiEstesi", "1");

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable oDt = Database.GetDataTableStoredProc("MSP_SelTipoAppuntamento", spcoll);

                                Graphics g = this.CreateGraphics();
                int icondimensions = CoreStatics.PointToPixel(this.UltraGrid.DisplayLayout.Override.CellAppearance.FontData.SizeInPoints, g.DpiX);
                g.Dispose();
                g = null;

                DataTable odtGrid = new DataTable();
                odtGrid.Columns.Add("Codice", typeof(String));
                odtGrid.Columns.Add("IconaSmall", typeof(Byte[]));
                odtGrid.Columns.Add("Descrizione", typeof(String));
                odtGrid.Columns.Add("CodScheda", typeof(String));
                odtGrid.Columns.Add("TimeSlotInterval", typeof(int));
                odtGrid.Columns.Add("Multiplo", typeof(bool));
                odtGrid.Columns.Add("SenzaData", typeof(bool));
                odtGrid.Columns.Add("SenzaDataSempre", typeof(bool));
                odtGrid.Columns.Add("Settimanale", typeof(bool));

                bool useParamFilters = ((this.XpFiltro_CodTipoApp != null) && (this.XpFiltro_CodTipoApp.Count > 0));

                foreach (DataRow odr in oDt.Rows)
                {
                    bool filterIsOk = true;

                                                            if (useParamFilters)
                    {
                        filterIsOk = this.XpFiltro_CodTipoApp.Exists(x => x == odr["Codice"].ToString());
                    }

                    if (filterIsOk)
                    {
                        DataRow odrgrid = odtGrid.NewRow();
                        odrgrid["Codice"] = odr["Codice"];
                        odrgrid["IconaSmall"] = DrawingProcs.GetByteFromImage(DrawingProcs.GetImageFromByte(odr["Icona"], new Size(icondimensions, icondimensions), true)); ;
                        odrgrid["Descrizione"] = odr["Descrizione"];
                        odrgrid["CodScheda"] = odr["CodScheda"];
                        odrgrid["TimeSlotInterval"] = odr["TimeSlotInterval"];
                        odrgrid["Multiplo"] = odr["Multiplo"];
                        odrgrid["SenzaData"] = odr["SenzaData"];
                        odrgrid["SenzaDataSempre"] = odr["SenzaDataSempre"];
                        odrgrid["Settimanale"] = odr["Settimanale"];
                        odtGrid.Rows.Add(odrgrid);
                    }

                }

                this.UltraGrid.DataSource = odtGrid;
                this.UltraGrid.Refresh();

                this.UltraGrid.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);
                this.UltraGrid.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;

                                                if (this.UltraGrid.Rows.Count == 1)
                {
                                        this.UltraGrid.Selected.Rows.Clear();
                    this.UltraGrid.Selected.Rows.Add(this.UltraGrid.Rows[0]);
                                        this.UltraGrid.ActiveRow = this.UltraGrid.Rows[0];

                                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodTipoAppuntamento = this.UltraGrid.ActiveRow.Cells["Codice"].Text;
                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DescrTipoAppuntamento = this.UltraGrid.ActiveRow.Cells["Descrizione"].Text;
                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodScheda = this.UltraGrid.ActiveRow.Cells["CodScheda"].Text;
                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.TimeSlotInterval = (int)this.UltraGrid.ActiveRow.Cells["TimeSlotInterval"].Value;
                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Multiplo = (bool)this.UltraGrid.ActiveRow.Cells["Multiplo"].Value;
                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Settimanale = (bool)this.UltraGrid.ActiveRow.Cells["Settimanale"].Value;
                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.SenzaData = (bool)this.UltraGrid.ActiveRow.Cells["SenzaData"].Value;
                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.SenzaDataSempre = (bool)this.UltraGrid.ActiveRow.Cells["SenzaDataSempre"].Value;
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;

                                        _skipShowForm = true;
                }
                else
                {
                    if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodTipoAppuntamento != string.Empty && CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodTipoAppuntamento != "")
                    {
                        var item = this.UltraGrid.Rows.Single(UltraGridRow => UltraGridRow.Cells["Codice"].Text == CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodTipoAppuntamento);
                        if (item != null) { this.UltraGrid.ActiveRow = item; }
                    }
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
                CoreStatics.ExGest(ref ex, "UltraGrid_InitializeLayout", this.Name);
            }

        }

        private void frmSelezionaTipoAppuntamento_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {

            try
            {

                if (this.UltraGrid.ActiveRow != null)
                {
                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodTipoAppuntamento = this.UltraGrid.ActiveRow.Cells["Codice"].Text;
                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DescrTipoAppuntamento = this.UltraGrid.ActiveRow.Cells["Descrizione"].Text;
                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodScheda = this.UltraGrid.ActiveRow.Cells["CodScheda"].Text;
                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.TimeSlotInterval = (int)this.UltraGrid.ActiveRow.Cells["TimeSlotInterval"].Value;
                    if (this.UltraGrid.ActiveRow.Cells["Multiplo"].Text != "") CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Multiplo = (bool)this.UltraGrid.ActiveRow.Cells["Multiplo"].Value;
                    if (this.UltraGrid.ActiveRow.Cells["SenzaData"].Text != "") CoreStatics.CoreApplication.MovAppuntamentoSelezionato.SenzaData = (bool)this.UltraGrid.ActiveRow.Cells["SenzaData"].Value;
                    if (this.UltraGrid.ActiveRow.Cells["SenzaDataSempre"].Text != "") CoreStatics.CoreApplication.MovAppuntamentoSelezionato.SenzaDataSempre = (bool)this.UltraGrid.ActiveRow.Cells["SenzaDataSempre"].Value;
                    if (this.UltraGrid.ActiveRow.Cells["Settimanale"].Text != "") CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Settimanale = (bool)this.UltraGrid.ActiveRow.Cells["Settimanale"].Value;
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void frmSelezionaTipoAppuntamento_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion

    }
}
