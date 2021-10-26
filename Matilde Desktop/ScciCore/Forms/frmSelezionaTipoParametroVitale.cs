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
    public partial class frmSelezionaTipoParametroVitale : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmSelezionaTipoParametroVitale()
        {
            InitializeComponent();
        }

        #region Interface

        public void Carica()
        {

            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_PARAMETRIVITALI_256);
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_PARAMETRIVITALI_16);

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
                                MovParametroVitale movPvtSel = CoreStatics.CoreApplication.MovParametroVitaleSelezionato;

                                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodUA", movPvtSel.CodUA);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                op.Parametro.Add("CodAzione", movPvtSel.Azione.ToString());
                op.Parametro.Add("DatiEstesi", "1");

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable oDt = Database.GetDataTableStoredProc("MSP_SelTipoParametroVitale", spcoll);

                                List<String> listCodTipo = new List<string>();
                bool bFilterCod = false;

                if ( (movPvtSel.Filtro_CodTipo != null ) && (movPvtSel.Filtro_CodTipo.Length > 0) )
                {
                    bFilterCod = true;
                    listCodTipo.AddRange(movPvtSel.Filtro_CodTipo);
                }                    
                

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
                                        bool bAddRow = (bFilterCod == false ) || (bFilterCod && (listCodTipo.Exists (itm => itm == odr["Codice"].ToString () ) ) )  ;

                    if (bAddRow)
                    {
                        DataRow odrgrid = odtGrid.NewRow();
                        odrgrid["Codice"] = odr["Codice"];
                        odrgrid["IconaSmall"] = DrawingProcs.GetByteFromImage(DrawingProcs.GetImageFromByte(odr["Icona"], new Size(icondimensions, icondimensions), true)); ;
                        odrgrid["Descrizione"] = odr["Descrizione"];
                        odrgrid["CodScheda"] = odr["CodScheda"];
                        odtGrid.Rows.Add(odrgrid);
                    }

                }

                this.UltraGrid.DataSource = odtGrid;
                this.UltraGrid.Refresh();

                this.UltraGrid.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);
                this.UltraGrid.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;

                if (movPvtSel.CodTipoParametroVitale != string.Empty && movPvtSel.CodTipoParametroVitale != "")
                {
                    var item = this.UltraGrid.Rows.Single(UltraGridRow => UltraGridRow.Cells["Codice"].Text == movPvtSel.CodTipoParametroVitale);
                    if (item != null) { this.UltraGrid.ActiveRow = item; }
                }

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

        private void frmSelezionaTipoParametroVitale_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            try
            {

                if (this.UltraGrid.ActiveRow != null)
                {

                    CoreStatics.CoreApplication.MovParametroVitaleSelezionato.CodTipoParametroVitale = this.UltraGrid.ActiveRow.Cells["Codice"].Text;
                    CoreStatics.CoreApplication.MovParametroVitaleSelezionato.DescrTipoParametroVitale = this.UltraGrid.ActiveRow.Cells["Descrizione"].Text;
                    CoreStatics.CoreApplication.MovParametroVitaleSelezionato.CodScheda = this.UltraGrid.ActiveRow.Cells["CodScheda"].Text;

                    if (this.uceCopiaDaPrecedente.Checked == true)
                    {

                                                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);

                        op.Parametro.Add("CodTipoParametroVitale", this.UltraGrid.ActiveRow.Cells["Codice"].Text);
                        op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);

                        SqlParameterExt[] spcoll = new SqlParameterExt[1];
                        string xmlParam = XmlProcs.XmlSerializeToString(op);
                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                        DataTable oDt = Database.GetDataTableStoredProc("MSP_CercaParametroVitalePrecedente", spcoll);

                        if (oDt.Rows.Count == 1 && oDt.Rows[0][0].ToString() != string.Empty)
                        {
                            MovParametroVitale oMovParametroVitale = new MovParametroVitale(oDt.Rows[0][0].ToString(), CoreStatics.CoreApplication.Ambiente);
                            CoreStatics.CoreApplication.MovParametroVitaleSelezionato.MovScheda.Scheda.StrutturaXML = oMovParametroVitale.MovScheda.Scheda.StrutturaXML;
                            CoreStatics.CoreApplication.MovParametroVitaleSelezionato.MovScheda.Scheda.LayoutXML = oMovParametroVitale.MovScheda.Scheda.LayoutXML;
                            CoreStatics.CoreApplication.MovParametroVitaleSelezionato.MovScheda.DatiXML = oMovParametroVitale.MovScheda.DatiXML;
                        }
                        else
                        {
                            if (easyStatics.EasyMessageBox("Non esiste nessun parametro vitale precedente da cui copiare i dati." + Environment.NewLine + "Vuoi continuare ugualmente?", "Copia da parametro vitale precedente", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                            {
                                return;
                            }
                        }

                    }


                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "frmSelezionaTipoParametroVitale_PulsanteAvantiClick", this.Name);
            }
        }

        private void frmSelezionaTipoParametroVitale_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion

    }
}
