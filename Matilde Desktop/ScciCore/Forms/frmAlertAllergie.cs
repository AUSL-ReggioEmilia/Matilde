using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;

using Microsoft.Data.SqlClient;
using System.Globalization;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.ScciResource;
using UnicodeSrl.ScciCore.CustomControls.Infra;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;

namespace UnicodeSrl.ScciCore
{
    public partial class frmAlertAllergie : frmBaseModale, Interfacce.IViewFormlModal
    {
        private ucRichTextBox _ucRichTextBox = null;

        public frmAlertAllergie()
        {
            InitializeComponent();
        }

        #region INTERFACCIA

        public void Carica()
        {
            try
            {
                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_ALERTALLERGIA_16);

                InizializzaControlli();
                InizializzaUltraGridLayout();
                VerificaSicurezza();

                CaricaGriglia();

                this.ShowDialog();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Carica", this.Name);
            }
        }

        #endregion

        #region PRIVATE

        private void InizializzaControlli()
        {

            try
            {
                this.ubAdd.Appearance.Image = Properties.Resources.Aggiungi_256;
                this.chkMostraAnnullati.Checked = false;

                this.ubAdd.PercImageFill = 0.75F;
                this.ubAdd.ShortcutKey = Keys.Add;
                this.ubAdd.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;
                this.ubAdd.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaControlli", this.Name);
            }
        }

        private void InizializzaUltraGridLayout()
        {
            try
            {
                CoreStatics.SetEasyUltraGridLayout(ref this.ucEasyGrid);
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaUltraGridLayout", this.Name);
            }
        }

        private void VerificaSicurezza()
        {

            try
            {
                if (CoreStatics.CoreApplication.Cartella != null && CoreStatics.CoreApplication.Cartella.CartellaChiusa == true)
                {
                    this.ubAdd.Enabled = false;
                }
                else
                {
                    this.ubAdd.Enabled = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.AlertAA_Inserisci);
                }
            }
            catch (Exception)
            {
            }
        }

        private void CaricaGriglia()
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);
                op.Parametro.Add("DatiEstesi", "1");
                if (this.chkMostraAnnullati.Checked)
                {
                    op.Parametro.Add("CodStatoAlert", "Tutti");
                }


                op.TimeStamp.CodEntita = EnumEntita.ALA.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelMovAlertAllergieAnamnesi", spcoll);

                this.ucEasyGrid.DisplayLayout.Bands[0].Columns.ClearUnbound();

                this.ucEasyGrid.ColonnaRTFResize = "AnteprimaRTF";
                int iFattore = 20;
                if (UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.FontPredefinitoRTF) != "")
                {
                    try
                    {
                        Graphics g = this.CreateGraphics();
                        iFattore = CoreStatics.PointToPixel(DrawingProcs.getFontFromString(UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.FontPredefinitoRTF)).SizeInPoints, g.DpiX) + 5;
                        g.Dispose();
                        g = null;
                    }
                    catch (Exception)
                    {
                        iFattore = 20;
                    }
                }
                this.ucEasyGrid.FattoreRidimensionamentoRTF = iFattore;
                DataTable dtEdit = ds.Tables[0].Copy();
                DataColumn colsp = new DataColumn(CoreStatics.C_COL_SPAZIO, typeof(string));
                colsp.AllowDBNull = true;
                colsp.DefaultValue = "";
                colsp.Unique = false;
                dtEdit.Columns.Add(colsp);

                dtEdit.DefaultView.Sort = @"DataEvento DESC";
                this.ucEasyGrid.DataSource = dtEdit.DefaultView;
                this.ucEasyGrid.Refresh();

                this.ucEasyGrid.PerformAction(UltraGridAction.FirstRowInBand, false, false);


            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaGriglia", this.Name);
            }
        }

        #endregion

        #region EVENTI

        private void ucEasyGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {
                int refWidth = (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XLarge) * 3;

                Graphics g = this.CreateGraphics();
                int refBtnWidth = Convert.ToInt32(CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.ucEasyGrid.DataRowFontRelativeDimension), g.DpiY) * 3);
                g.Dispose();
                g = null;
                e.Layout.Bands[0].HeaderVisible = false;
                e.Layout.Bands[0].ColHeadersVisible = false;
                e.Layout.Bands[0].RowLayoutStyle = RowLayoutStyle.GroupLayout;
                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {

                    try
                    {
                        oCol.SortIndicator = SortIndicator.Disabled;
                        switch (oCol.Key)
                        {
                            case "Icona":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                                oCol.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.LockedWidth = true;
                                try
                                {
                                    oCol.MaxWidth = refWidth;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 3;

                                break;

                            case "DataEvento":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Red;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.Format = "dd/MM/yyyy HH:mm";
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 2.5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "DescrUtente":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Red;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 2.5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 1;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case CoreStatics.C_COL_SPAZIO:
                                oCol.Hidden = false;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XXSmall);
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 2.5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.WeightY = 1;
                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 2;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "DescrTipo":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.LockedWidth = true;
                                try
                                {
                                    oCol.MaxWidth = refWidth * 2;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }


                                oCol.RowLayoutColumnInfo.OriginX = 2;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 3;

                                break;

                            case "AnteprimaRTF":
                                oCol.Hidden = false;

                                RichTextEditor a = new RichTextEditor();
                                oCol.Editor = a;
                                oCol.CellActivation = Activation.ActivateOnly;
                                oCol.CellClickAction = CellClickAction.CellSelect;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.VertScrollBar = true;
                                try
                                {
                                    oCol.MaxWidth = this.ucEasyGrid.Width - Convert.ToInt32(refWidth * 5.5) - Convert.ToInt32(refBtnWidth * 2.2) - 30;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 3;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 3;

                                break;

                            default:
                                oCol.Hidden = true;
                                break;

                        }
                    }
                    catch (Exception)
                    {
                    }

                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_EDIT))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_EDIT);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_MODIFICA_32);


                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = refBtnWidth;
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;


                    colEdit.RowLayoutColumnInfo.OriginX = 4;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_EDIT + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_EDIT + @"_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = Convert.ToInt32(refBtnWidth * 0.1);
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;
                    colEdit.RowLayoutColumnInfo.OriginX = 5;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }


                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_ANNULLA))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_ANNULLA);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_ELIMINA_32);


                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = refBtnWidth;
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;


                    colEdit.RowLayoutColumnInfo.OriginX = 6;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_ANNULLA + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_ANNULLA + @"_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = Convert.ToInt32(refBtnWidth * 0.1);
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;
                    colEdit.RowLayoutColumnInfo.OriginX = 7;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }


            }
            catch (Exception)
            {
            }
        }

        private void ucEasyGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            try
            {
                foreach (UltraGridCell ocell in e.Row.Cells)
                {
                    if (CoreStatics.CoreApplication.Cartella != null && CoreStatics.CoreApplication.Cartella.CartellaChiusa == true)
                    {
                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_ANNULLA)
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_EDIT)
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                    }
                    else
                    {
                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_ANNULLA && ocell.Row.Cells["PermessoAnnulla"].Value.ToString() == "0")
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_EDIT && ocell.Row.Cells["Permessomodifica"].Value.ToString() == "0")
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                    }
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_InitializeRow", this.Name);
            }
        }

        private void ucEasyGrid_ClickCellButton(object sender, CellEventArgs e)
        {
            try
            {
                switch (e.Cell.Column.Key)
                {

                    case CoreStatics.C_COL_BTN_ANNULLA:
                        if (e.Cell.Row.Cells["PermessoAnnulla"].Text == "1")
                        {
                            CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato = new MovAlertAllergieAnamnesi(e.Cell.Row.Cells["ID"].Text, e.Cell.Row.Cells["IDPaziente"].Text,
                                                                                                (CoreStatics.CoreApplication.Trasferimento == null ? "" : CoreStatics.CoreApplication.Trasferimento.CodUA),
                                                                                                (CoreStatics.CoreApplication.Episodio == null ? "" : CoreStatics.CoreApplication.Episodio.ID),
                                                                                                (CoreStatics.CoreApplication.Trasferimento == null ? "" : CoreStatics.CoreApplication.Trasferimento.ID),
                                                                                                CoreStatics.CoreApplication.Ambiente);

                            if (CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.CodStato != "AT")
                            {
                                easyStatics.EasyMessageBox("La nota selezionata non è più valida. Alla chiusura del messaggio verranno ricaricati i dati.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                this.CaricaGriglia();
                                return;
                            }

                            if (easyStatics.EasyMessageBox("Sei sicuro di voler ANNULLARE" + Environment.NewLine + "la Nota corrente?", "Annullamento Nota Anamnestica", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                MovAlertAllergieAnamnesi movaa = new MovAlertAllergieAnamnesi(e.Cell.Row.Cells["ID"].Text,
                                                                                                e.Cell.Row.Cells["IDPaziente"].Text,
                                    (CoreStatics.CoreApplication.Trasferimento == null ? "" : CoreStatics.CoreApplication.Trasferimento.CodUA),
                                    (CoreStatics.CoreApplication.Episodio == null ? "" : CoreStatics.CoreApplication.Episodio.ID),
                                    (CoreStatics.CoreApplication.Trasferimento == null ? "" : CoreStatics.CoreApplication.Trasferimento.ID),
                                    CoreStatics.CoreApplication.Ambiente);
                                if (movaa.Annulla())
                                {
                                    CaricaGriglia();
                                }
                            }
                        }
                        break;

                    case CoreStatics.C_COL_BTN_EDIT:
                        if (e.Cell.Row.Cells["PermessoModifica"].Text == "1")
                        {

                            this.ImpostaCursore(Scci.Enums.enum_app_cursors.WaitCursor);

                            CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato = new MovAlertAllergieAnamnesi(e.Cell.Row.Cells["ID"].Text, e.Cell.Row.Cells["IDPaziente"].Text,
                                                                                                (CoreStatics.CoreApplication.Trasferimento == null ? "" : CoreStatics.CoreApplication.Trasferimento.CodUA),
                                                                                                (CoreStatics.CoreApplication.Episodio == null ? "" : CoreStatics.CoreApplication.Episodio.ID),
                                                                                                (CoreStatics.CoreApplication.Trasferimento == null ? "" : CoreStatics.CoreApplication.Trasferimento.ID),
                                                                                                CoreStatics.CoreApplication.Ambiente);

                            if (CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.CodStato != "AT")
                            {
                                easyStatics.EasyMessageBox("La nota selezionata non è più valida. Alla chiusura del messaggio verranno ricaricati i dati.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                this.CaricaGriglia();
                                return;
                            }

                            if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingNoteAnamnesticheAlertAllergie) == DialogResult.OK)
                            {
                                CaricaGriglia();
                            }

                        }
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_ClickCellButton", this.Name);
            }
            finally
            {
                this.ImpostaCursore(Scci.Enums.enum_app_cursors.DefaultCursor);
            }

        }

        private void ubAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string sCodUA = String.Empty;


                this.ImpostaCursore(Scci.Enums.enum_app_cursors.WaitCursor);


                if (CoreStatics.CoreApplication.Trasferimento != null)
                {
                    sCodUA = CoreStatics.CoreApplication.Trasferimento.CodUA;
                }
                else
                {
                    sCodUA = CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata;
                }


                CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato = new MovAlertAllergieAnamnesi(CoreStatics.CoreApplication.Paziente.ID,
                                                                                                                sCodUA,
                                                                                                               (CoreStatics.CoreApplication.Episodio == null ? "" : CoreStatics.CoreApplication.Episodio.ID),
                                                                                                               (CoreStatics.CoreApplication.Trasferimento == null ? "" : CoreStatics.CoreApplication.Trasferimento.ID),
                                                                                                               CoreStatics.CoreApplication.Ambiente);
                CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.Azione = EnumAzioni.INS;
                CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.DataEvento = DateTime.Now;

                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneTipoNoteAnamnesticheAlertAllergie) == DialogResult.OK)
                {
                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingNoteAnamnesticheAlertAllergie) == DialogResult.OK)
                    {
                        CaricaGriglia();
                        if (CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato != null)
                            CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.IDMovAlertAllergieAnamnesi);
                    }
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubAdd_Click", this.Name);
            }
            finally
            {
                this.ImpostaCursore(Scci.Enums.enum_app_cursors.DefaultCursor);
            }
        }

        private void chkMostraAnnullati_CheckedValueChanged(object sender, EventArgs e)
        {
            CaricaGriglia();
        }

        private void frmAlertAllergie_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void frmAlertAllergie_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void UltraPopupControlContainer_Closed(object sender, EventArgs e)
        {
            _ucRichTextBox.RtfClick -= ucRichTextBox_Click;
        }

        private void UltraPopupControlContainer_Opened(object sender, EventArgs e)
        {
            _ucRichTextBox.RtfClick += ucRichTextBox_Click;
        }

        private void UltraPopupControlContainer_Opening(object sender, CancelEventArgs e)
        {
            Infragistics.Win.Misc.UltraPopupControlContainer popup = sender as Infragistics.Win.Misc.UltraPopupControlContainer;
            popup.PopupControl = _ucRichTextBox;
        }

        private void ucEasyGrid_ClickCell(object sender, ClickCellEventArgs e)
        {
            try
            {

                switch (e.Cell.Column.Key)
                {

                    case "AnteprimaRTF":
                        CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainer);
                        _ucRichTextBox = CoreStatics.GetRichTextBoxForPopupControlContainer(e.Cell.Text);

                        Infragistics.Win.UIElement uie = e.Cell.GetUIElement();
                        Point oPoint = new Point(uie.Rect.Left, uie.Rect.Top);

                        this.UltraPopupControlContainer.Show(this.ucEasyGrid, oPoint);
                        break;

                    default:
                        break;

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_ClickCell", this.Name);
            }
        }

        private void ucRichTextBox_Click(object sender, EventArgs e)
        {
        }

        #endregion

    }
}
