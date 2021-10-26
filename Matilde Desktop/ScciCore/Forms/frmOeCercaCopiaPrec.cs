using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnicodeSrl.DataModel;
using UnicodeSrl.DatiClinici.DC;
using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Model.Strutture;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.ScciCore.Extensions;

namespace UnicodeSrl.ScciCore
{
    public partial class frmOeCercaCopiaPrec : frmBaseModale
    {
        public frmOeCercaCopiaPrec(MovOrdine movOrdine, string idCampo, DcDecodifiche decodifiche)
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Maximized;

            this.MovOrdine  = movOrdine;
            this.Decodifiche = decodifiche;

                        this.caricaDatoOE(idCampo);

                        this.LoadData();
            this.DisplayUI();

                        setUI();
        }


        #region     Prop

                                public MovOrdine MovOrdine
        { get; private set; }

                                public DcDecodifiche Decodifiche
        { get; private set; }

                                public String IdCampo
        { get; private set; }

                                public int? SequenzaCampo
        { get; private set; }

                                public DcVoce DcVoce
        { get; private set; }

                                public bool VoceRipetibile
        {
            get
            {
                if (this.DcVoce == null) return false ;
                KeyValuePair<String, DcAttributo> kvp = this.DcVoce.Attributi.FirstOrDefault(x => x.Key == "Ripetibile") ;

                return Convert.ToBoolean(kvp.Value.Value);
            }
        }

                                public List<MSP_SelMovOrdini_GridOE> Data
        { get; private set; }

                                public List<SelectionObjectString> SelectionList { get; private set; }


        #endregion  Prop

        #region     UI e Security

                                private void setUI()
        {
                        if (this.gridDC.DataSource is List<SelectionObjectString>)
            {
                List<SelectionObjectString> gridSource = (List<SelectionObjectString>)this.gridDC.DataSource;
                this.SelectionList = gridSource.Where(x => x.Selezionato == true).ToList();
            }
            else
                this.SelectionList = null;

                        if ((this.VoceRipetibile == false) && (this.gridDC.Rows.Count == 1))
            {
                this.gridDC.Rows[0].Height = this.gridDC.Height;
                this.gridDC.DisplayLayout.Bands[0].Columns[0].Hidden = true;
                this.gridDC.DisplayLayout.Bands[0].Columns[1].Hidden = true;
            }

            this.PulsanteAvantiAbilitato = ((this.SelectionList != null ) && (this.SelectionList.Count > 0));           

        }


        #endregion  UI e Security

        #region     Database e Grid

                                public void LoadData()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                StringBuilder builder = new StringBuilder();

                foreach (var erogante in this.MovOrdine.ListaEroganti)
                {
                    builder.Append("'").Append(erogante.CodiceAzienda).Append("|").Append(erogante.Codice).Append("', ");
                }                                

                                string pref = @"OE";
                string idcampoDB = this.IdCampo;

                                if (idcampoDB.StartsWith(pref) == false) idcampoDB = pref + idcampoDB;

                string codUtente = Environment.UserDomainName + @"\" + Environment.UserName;

                using (FwDataConnection fdc = new FwDataConnection(Database.ConnectionString))
                {
                                        FwDataBufferedList < MSP_SelMovOrdini_GridOE > data = fdc.MSP_SelMovOrdini_GridOE(codUtente, this.MovOrdine.IDOrdine, idcampoDB, this.Decodifiche);

                                                            this.Data = (
                                    from x in data.Buffer
                                    where x.DescrizioneDatiOE != ""
                                    orderby x.DataUltimaModifica descending , x.DataProgrammazioneOE descending
                                    select x
                                ).Take(10).ToList();

                }
                


            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Si è verificato un errore nella procedura di caricamento Ordini", "LoadData", "frmOeCercaCopiaPrec");
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }


                                        public void DisplayUI()
        {
            try
            {
                this.grid.Visible = false;

                                this.grid.DataSource = this.Data;

                                grid.DisplayLayout.Bands[0].Override.RowSizing = RowSizing.Fixed;

                grid.DisplayLayout.Bands[0].Override.RowAppearance.BackColor = Color.White;
                grid.DisplayLayout.Bands[0].Override.RowAppearance.ForeColor = Color.Black;

                grid.DisplayLayout.Bands[0].Override.RowAlternateAppearance.BackColor = Color.White;
                grid.DisplayLayout.Bands[0].Override.RowAlternateAppearance.ForeColor = Color.Black;

                grid.DisplayLayout.Bands[0].RowLayoutStyle = RowLayoutStyle.ColumnLayout;

                grid.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
                grid.DisplayLayout.Override.HeaderAppearance.TextHAlign = Infragistics.Win.HAlign.Left;

                grid.DisplayLayout.Override.ActiveCellAppearance.BackColor = Color.FromKnownColor(KnownColor.Window);
                grid.DisplayLayout.Override.ActiveCellAppearance.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
                grid.DisplayLayout.Override.ActiveRowAppearance.BackColor = Color.FromKnownColor(KnownColor.Highlight);
                grid.DisplayLayout.Override.ActiveRowAppearance.ForeColor = Color.FromKnownColor(KnownColor.HighlightText);

                grid.FilterRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
                grid.GridCaptionFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
                grid.HeaderFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
                grid.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;

                                this.grid.HideAllColumns();

                UltraGridBand b0 = this.grid.DisplayLayout.Bands[0];
                b0.DisplayColumn("DescrizioneDatiOE", "DatoOE", 0, 300, 1f, VAlign.Top, HAlign.Left, SortIndicator.None, Infragistics.Win.DefaultableBoolean.True);

                #region old Layout
                                
                
                                                                
                                                                

                                                                                                #endregion old Layout

                                b0.ColHeadersVisible = false;

                                b0.Override.DefaultRowHeight = 15;
                b0.Override.RowSizingArea = RowSizingArea.Default;
                b0.Override.RowSizingAutoMaxLines = 5;
                b0.Override.RowSizing = RowSizing.AutoFixed;

                                this.grid.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);
                this.grid.DisplayLayout.Override.HeaderAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                this.grid.Visible = true;


                                if ((this.Data != null) && (this.Data.Count > 0))
                {
                    this.grid.ActiveRow = this.grid.Rows[0];
                    this.grid.Selected.Rows.Add(this.grid.ActiveRow);
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Si è verificato un errore nella procedura di caricamento Ordini", "LoadData", "frmOeCercaCopiaPrec");
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

        }

                                        private void displayUiDc()
        {
            try
            {
                this.gridDC.Visible = false;
                if ((this.gridDC.DataSource != null) && (this.gridDC.Rows.Count == 0)) return;

                                gridDC.DisplayLayout.Bands[0].Override.RowSizing = RowSizing.Fixed;

                gridDC.DisplayLayout.Bands[0].Override.RowAppearance.BackColor = Color.White;
                gridDC.DisplayLayout.Bands[0].Override.RowAppearance.ForeColor = Color.Black;

                gridDC.DisplayLayout.Bands[0].Override.RowAlternateAppearance.BackColor = Color.White;
                gridDC.DisplayLayout.Bands[0].Override.RowAlternateAppearance.ForeColor = Color.Black;

                gridDC.DisplayLayout.Bands[0].RowLayoutStyle = RowLayoutStyle.ColumnLayout;

                gridDC.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
                gridDC.DisplayLayout.Override.HeaderAppearance.TextHAlign = Infragistics.Win.HAlign.Left;

                gridDC.FilterRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
                gridDC.GridCaptionFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
                gridDC.HeaderFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
                gridDC.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;

                gridDC.DisplayLayout.Override.ActiveRowAppearance.BackColor = Color.White;
                gridDC.DisplayLayout.Override.ActiveRowAppearance.ForeColor = Color.Black;


                                UltraGridBand b0 = this.gridDC.DisplayLayout.Bands[0];

                b0.Columns[0].Width = 50;
                b0.Columns[0].RowLayoutColumnInfo.WeightX = 0f;

                b0.Columns[1].Width = 50;
                b0.Columns[1].CellMultiLine = DefaultableBoolean.True;
                b0.Columns[1].RowLayoutColumnInfo.WeightX = 0f;

                b0.Columns[2].Hidden = true;

                b0.Columns[3].Width = this.gridDC.Width - 100;
                b0.Columns[3].CellMultiLine = DefaultableBoolean.True;
                b0.Columns[3].RowLayoutColumnInfo.WeightX = 1f;                

                                b0.Columns[0].Header.Caption = "Indice";
                b0.Columns[1].Header.Caption = "Selezionato";
                b0.Columns[2].Header.Caption = "Valore";
                b0.Columns[3].Header.Caption = "Descrizione";
                b0.ColHeadersVisible = true;

                                b0.Override.DefaultRowHeight = 15;
                b0.Override.RowSizingArea = RowSizingArea.Default;
                b0.Override.RowSizingAutoMaxLines = 5;
                b0.Override.RowSizing = RowSizing.AutoFree;

                                this.gridDC.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);
                this.gridDC.DisplayLayout.Override.HeaderAppearance.TextHAlign = Infragistics.Win.HAlign.Center;

                                if ((this.gridDC.DataSource != null) && (this.gridDC.Rows.Count > 0))
                {
                    this.gridDC.ActiveRow = this.gridDC.Rows[0];
                    this.gridDC.Selected.Rows.Add(this.gridDC.ActiveRow);
                }

                                if (this.gridDC.Rows.Count == 1)
                {
                    SelectionObjectString selrow = (SelectionObjectString) this.gridDC.ActiveRow.ListObject;
                    selrow.Selezionato = true;
                }


                this.gridDC.Visible = true;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Si è verificato un errore nella procedura di caricamento Ordini", "LoadData", "frmOeCercaCopiaPrec");
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

        }

        #endregion  Database e Grid

        #region     Dati OE

                                private void caricaDatoOE(string idCampo)
        {
            Gestore g = this.MovOrdine.CaricaDatiOE();

                        string dcKey = idCampo.Replace(this.MovOrdine.C_BUTTON_PREFIX_SRC, "");
            String[] keysplitter = dcKey.Split('_');

            if (keysplitter.Length > 1)
            {
                this.IdCampo = keysplitter[0];
                this.SequenzaCampo = Convert.ToInt32(keysplitter[1]);
            }                
            else
            {
                this.IdCampo = dcKey;
                this.SequenzaCampo = null;
            }
              

            this.DcVoce = g.LeggeVoce(this.IdCampo); 
            

            this.lblValore.Text = this.lblValore.Text.Replace(@"<label>", this.DcVoce.Descrizione);

        }

                                private void caricaDatoOrdineSel()
        {
            this.gridDC.DataSource = null;
            if (this.grid.ActiveRow.ListObject == null) return;

                        MSP_SelMovOrdini_GridOE row = (MSP_SelMovOrdini_GridOE)this.grid.ActiveRow.ListObject;
         
            this.gridDC.DataSource = row.GetDatiOE( );

            this.displayUiDc();
            this.setUI();
        }



        #endregion  Dati OE

        private void grid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            try
            {
                                caricaDatoOrdineSel();
                                setUI();
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Si è verificato un errore nella procedura di caricamento Ordini", "grid_AfterSelectChange", "frmOeCercaCopiaPrec");
            }

        }


        private void frmOeCercaCopiaPrec_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void frmOeCercaCopiaPrec_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        private void gridDC_ClickCell_1(object sender, ClickCellEventArgs e)
        {
                        if (this.gridDC.Rows.Count > 1)
            {
                SelectionObjectString selrow = (SelectionObjectString)this.gridDC.ActiveRow.ListObject;
                selrow.Selezionato = !(selrow.Selezionato);
            }

            setUI();
        }

        private void frmOeCercaCopiaPrec_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible == false) return;

            this.setUI();

        }
    }
}
