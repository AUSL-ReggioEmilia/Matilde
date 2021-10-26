using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Data.SqlClient;
using System.Globalization;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.ScciResource;
using Infragistics.Win.UltraWinGrid;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;

namespace UnicodeSrl.ScciCore
{
    public partial class frmIdentificazioneIterataPaziente : frmBaseModale, Interfacce.IViewFormlModal
    {

        #region Dichiarazioni

        DataTable _dtparametrivitali = null;

        #endregion

        public frmIdentificazioneIterataPaziente()
        {
            InitializeComponent();
        }

        #region INTERFACCIA

        public void Carica()
        {

            this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
            this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_PARAMETRIVITALI_16);

            InizializzaControlli();

            CercaPaziente();

            this.ShowDialog();

        }

        #endregion

        #region PRIVATE

        private void InizializzaControlli()
        {
            try
            {
                this.ucEasyGridEpisodi.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
                this.ucEasyGridParametri.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Large;

                                _dtparametrivitali = new DataTable();

                DataColumn colpv = new DataColumn("Icona", typeof(byte[]));
                _dtparametrivitali.Columns.Add(colpv);
                                colpv = new DataColumn("Codice", typeof(string));
                colpv.AllowDBNull = false;
                colpv.Unique = true;
                colpv.MaxLength = 20;
                _dtparametrivitali.Columns.Add(colpv);
                                colpv = new DataColumn("Descrizione", typeof(string));
                colpv.AllowDBNull = true;
                colpv.Unique = false;
                colpv.MaxLength = 200;
                _dtparametrivitali.Columns.Add(colpv);
                                colpv = new DataColumn("CodStato", typeof(string));
                colpv.AllowDBNull = true;
                colpv.Unique = false;
                colpv.MaxLength = 20;
                _dtparametrivitali.Columns.Add(colpv);
                                colpv = new DataColumn("DaRilevare", typeof(bool));
                colpv.AllowDBNull = true;
                colpv.Unique = false;
                _dtparametrivitali.Columns.Add(colpv);
                                colpv = new DataColumn("CodScheda", typeof(string));
                colpv.AllowDBNull = false;
                colpv.Unique = true;
                colpv.MaxLength = 50;
                _dtparametrivitali.Columns.Add(colpv);
                                colpv = new DataColumn("IDMovParametroVitale", typeof(string));
                colpv.AllowDBNull = true;
                colpv.Unique = false;
                colpv.MaxLength = 50;
                _dtparametrivitali.Columns.Add(colpv);

                Dictionary<string, TipoParametroVitale> pvsel = CoreStatics.CoreApplication.ParametriVitaliTrasversali.ListaTipiParametriSelezionati;
                for (int i = 0; i < pvsel.Count; i++)
                {
                    DataRow rowpv = _dtparametrivitali.NewRow();
                    rowpv["Icona"] = System.DBNull.Value;
                    rowpv["Codice"] = pvsel.ElementAt(i).Key;
                    rowpv["Descrizione"] = pvsel.ElementAt(i).Value.Descrizione;
                    rowpv["CodScheda"] = pvsel.ElementAt(i).Value.CodScheda;
                    rowpv["CodStato"] = System.DBNull.Value;
                    rowpv["DaRilevare"] = true;
                    rowpv["IDMovParametroVitale"] = System.DBNull.Value;
                    _dtparametrivitali.Rows.Add(rowpv);
                }

            }
            catch (Exception)
            {
            }
        }

        private void CercaPaziente()
        {

            try
            {

                ClearSelezionePaziente();

                                                                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                
                if (this.txtCercaPaziente.Text != string.Empty)
                {

                    string filtrogenerico = string.Empty;

                    string[] ricerche = this.txtCercaPaziente.Text.Split(' ');
                    foreach (string ricerca in ricerche)
                    {

                        string format = "dd/MM/yyyy";
                        DateTime dateTime;
                        if (DateTime.TryParseExact(ricerca, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                        {
                            op.Parametro.Add("DataNascita", ricerca);
                        }
                        else
                        {
                            filtrogenerico += ricerca + " ";
                        }

                    }

                    op.Parametro.Add("FiltroGenerico", filtrogenerico);

                }

                op.ParametroRipetibile.Add("CodUA", CoreStatics.CoreApplication.ParametriVitaliTrasversali.ListaUASelezionate.Keys.ToArray());

                                                op.Parametro.Add("CodStatoCartella", "AP");                 op.Parametro.Add("CodStatoTrasferimento", "AT"); 
                                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_CercaEpisodio", spcoll);

                dt.DefaultView.Sort = "Paziente";
                this.ucEasyGridEpisodi.DataSource = dt.DefaultView;
                this.ucEasyGridEpisodi.Refresh();
                                this.ucEasyGridEpisodi.DisplayLayout.Override.SelectTypeRow = SelectType.Single;

                if (this.ucEasyGridEpisodi.Rows.Count == 1)
                {
                    this.ucEasyGridEpisodi.Selected.Rows.Clear();
                    this.ucEasyGridEpisodi.ActiveRow = null;
                    this.ucEasyGridEpisodi.Selected.Rows.Add(this.ucEasyGridEpisodi.Rows[0]);
                    this.ucEasyGridEpisodi.Rows[0].Activate();
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CercaPaziente", this.Name);
            }
        }

        private void ClearSelezionePaziente()
        {
            try
            {
                this.lblPaziente.Text = "";
                
                this.ucEasyGridParametri.Selected.Rows.Clear();
                this.ucEasyGridParametri.ActiveRow = null;
                this.ucEasyGridParametri.DataSource = null;

                foreach (DataRow rowpv in _dtparametrivitali.Rows)
                {
                    rowpv["Icona"] = System.DBNull.Value;
                    rowpv["CodStato"] = System.DBNull.Value;
                    rowpv["DaRilevare"] = true;
                    rowpv["IDMovParametroVitale"] = System.DBNull.Value;
                }

            }
            catch (Exception)
            {
            }
        }

        private void SelezionaPaziente()
        {
            ClearSelezionePaziente();
            if (RigaSelezionata(ref this.ucEasyGridEpisodi) != null)
            {

                UltraGridRow growepisodio = RigaSelezionata(ref this.ucEasyGridEpisodi);

                CoreStatics.CoreApplication.Paziente = new Paziente(growepisodio.Cells["IDPaziente"].Text, growepisodio.Cells["IDEpisodio"].Text);
                CoreStatics.CoreApplication.Episodio = new Episodio(growepisodio.Cells["IDEpisodio"].Text);

                                string spaziente = growepisodio.Cells["Paziente"].Text.Trim();
                if (growepisodio.Cells["DescrUA"].Text.Trim() != "") spaziente += Environment.NewLine + growepisodio.Cells["DescrUA"].Text.Trim();
                this.lblPaziente.Text = spaziente;

                                CaricaGrigliaParametri();

                                SelezionaParametroDaInserire();


                                                if (this.txtCercaPaziente.Text.Trim() != "" && this.ucEasyGridEpisodi.Rows.Count == 1 && RigaSelezionata(ref this.ucEasyGridParametri) != null)
                {
                    
                                        UltraGridRow growparam = RigaSelezionata(ref this.ucEasyGridParametri);
                    if (growparam.Cells["CodStato"].Text.Trim() == "") ubRileva_Click(this.ubRileva, new EventArgs());

                }

            }
            else
            {
                CoreStatics.CoreApplication.Paziente = null;
                CoreStatics.CoreApplication.Episodio = null;
            }
        }

        private void CaricaGrigliaParametri()
        {
            try
            {
                UltraGridRow growepisodio = RigaSelezionata(ref this.ucEasyGridEpisodi);
                                                                int iMinuti = 60;
                string sMinuti = UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.MinutiRicercaParametriVitaliTrasversali);
                if (sMinuti.Trim() != "")
                {
                    if (!Int32.TryParse(sMinuti, out iMinuti)) iMinuti = 60;
                }
                
                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDEpisodio", growepisodio.Cells["IDEpisodio"].Text);
                op.Parametro.Add("DatiEstesi", "0");
                op.Parametro.Add("DataInizio", Database.dataOra105PerParametri(DateTime.Now.AddMinutes(-iMinuti)));
                op.Parametro.Add("DataFine", Database.dataOra105PerParametri(DateTime.Now));
                op.ParametroRipetibile.Add("CodTipoParametroVitale", CoreStatics.CoreApplication.ParametriVitaliTrasversali.ListaTipiParametriSelezionati.Keys.ToArray());
                op.ParametroRipetibile.Add("CodStatoParametroVitale", new string[] {"ER", "AN"});
                op.TimeStamp.CodEntita = EnumEntita.PVT.ToString();                 op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();                                 SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                                DataTable dtinseriti = Database.GetDataTableStoredProc("MSP_SelMovParametriVitali", spcoll);
                if (dtinseriti != null)
                {
                    if (dtinseriti.Rows.Count > 0)
                    {
                        dtinseriti.DefaultView.Sort = "DataInserimento ASC, DataEvento ASC";
                        foreach (DataRowView rowpvinserito in dtinseriti.DefaultView)
                        {
                            _dtparametrivitali.DefaultView.RowFilter = @"Codice = '" + Database.testoSQL(rowpvinserito["CodTipo"].ToString()) + @"'";
                            if (_dtparametrivitali.DefaultView.Count == 1)
                            {
                                _dtparametrivitali.DefaultView[0].Row["IDMovParametroVitale"] = rowpvinserito["ID"].ToString();
                                _dtparametrivitali.DefaultView[0].Row["CodStato"] = rowpvinserito["CodStato"];
                                
                                if (_dtparametrivitali.DefaultView[0].Row["CodStato"].ToString() == "AN")
                                    _dtparametrivitali.DefaultView[0].Row["Icona"] = DrawingProcs.GetByteFromImage(ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_NO_32));
                                else
                                    _dtparametrivitali.DefaultView[0].Row["Icona"] = DrawingProcs.GetByteFromImage(ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_SI_32));

                                _dtparametrivitali.DefaultView[0].Row["DaRilevare"] = false;
                            }
                            _dtparametrivitali.DefaultView.RowFilter = @"";
                        }
                    }
                    dtinseriti.Dispose();
                }

                                _dtparametrivitali.DefaultView.Sort = "DaRilevare ASC, Descrizione ASC";
                this.ucEasyGridParametri.DataSource = _dtparametrivitali.DefaultView;
                this.ucEasyGridParametri.DisplayLayout.Override.SelectTypeRow = SelectType.Single;

                this.ucEasyGridParametri.Selected.Rows.Clear();
                this.ucEasyGridParametri.ActiveRow = null;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaGrigliaParametri", this.Name);
            }
        }

        private void SelezionaParametroDaInserire()
        {
            try
            {

                this.ucEasyGridParametri.Selected.Rows.Clear();
                this.ucEasyGridParametri.ActiveRow = null;
                for (int iRow = 0; iRow < this.ucEasyGridParametri.Rows.Count; iRow++)
                {
                    if (this.ucEasyGridParametri.Rows[iRow].Cells["CodStato"].Text == "")
                    {
                        this.ucEasyGridParametri.Rows[iRow].Activate();
                        this.ucEasyGridParametri.Selected.Rows.Add(this.ucEasyGridParametri.Rows[iRow]);
                                                iRow = this.ucEasyGridParametri.Rows.Count + 1;
                    }
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "SelezionaParametroDaInserire", this.Name);
            }
        }

        private UltraGridRow RigaSelezionata(ref ucEasyGrid roGrid)
        {
            UltraGridRow ret = null;

            try
            {
                if (roGrid.Selected.Rows.Count == 1 && roGrid.Selected.Rows[0].IsDataRow) ret = roGrid.Selected.Rows[0];
            }
            catch (Exception)
            {
            }

            return ret;
        }

        private void AttivaPulsanti()
        {
            try
            {
                this.ubNonRilevare.Enabled = false;
                this.ubRileva.Enabled = false;

                UltraGridRow rowepisodio = RigaSelezionata(ref this.ucEasyGridEpisodi);
                UltraGridRow rowpv = RigaSelezionata(ref this.ucEasyGridParametri);

                if (rowepisodio != null && rowpv != null)
                {
                    this.ubRileva.Enabled = true;

                    this.ubNonRilevare.Enabled = (rowpv.Cells["CodStato"].Text.Trim().ToUpper() != "AN");
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region EVENTI

        private void frmIdentificazioneIterataPaziente_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
                    }

        private void frmIdentificazioneIterataPaziente_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            CoreStatics.CoreApplication.Paziente = null;
            CoreStatics.CoreApplication.Episodio = null;
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        } 

        private void ucEasyGridEpisodi_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                e.Layout.Override.SelectTypeRow = SelectType.Single;
                foreach (UltraGridColumn col in e.Layout.Bands[0].Columns)
                {
                    switch (col.Key)
                    {
                        case "Paziente":
                            col.Hidden = false;
                            col.Header.VisiblePosition = 0;
                            col.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, AutoResizeColumnWidthOptions.IncludeCells);
                            break;
                        case "NumeroNosologico":
                            col.Hidden = false;
                            col.Header.Caption = "Nosologico";
                            col.Header.VisiblePosition = 1;
                            col.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, AutoResizeColumnWidthOptions.IncludeCells);
                            break;
                                                                                                                                                                        case "UO - Settore":
                            col.Hidden = false;
                            col.Header.VisiblePosition = 2;                            
                                                        break;
                        default:
                            col.Hidden = true;
                            break;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void ucEasyGridParametri_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                e.Layout.Override.SelectTypeRow = SelectType.Single;
                e.Layout.Bands[0].ColHeadersVisible = false;
                foreach (UltraGridColumn col in e.Layout.Bands[0].Columns)
                {
                    switch (col.Key)
                    {
                        case "Icona":
                            col.Hidden = false;
                            col.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                            col.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                            break;
                        case "Descrizione":
                            col.Hidden = false;
                            col.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                            col.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                            break;
                        default:
                            col.Hidden = true;
                            break;
                    }
                }
            }
            catch (Exception )
            {
            }
        }

        private void txtCercaPaziente_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                                if (e.KeyCode == Keys.Enter) CercaPaziente();
            }
            catch (Exception )
            {
            }
        }

        private void frmIdentificazioneIterataPaziente_Activated(object sender, EventArgs e)
        {
            try
            {
                this.txtCercaPaziente.Focus();
            }
            catch (Exception)
            {
            }
        }

        private void controls_Enter(object sender, EventArgs e)
        {
            try
            {
                            }
            catch (Exception)
            {
            }
        }

                                
        private void ucEasyGridEpisodi_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            SelezionaPaziente();
        }

        private void ucEasyGridParametri_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            AttivaPulsanti();
            this.txtCercaPaziente.Focus();
        }

        private void ubRileva_Click(object sender, EventArgs e)
        {
            try
            {
                UltraGridRow rowepisodio = RigaSelezionata(ref this.ucEasyGridEpisodi);
                UltraGridRow rowpv = RigaSelezionata(ref this.ucEasyGridParametri);
                if (rowepisodio != null && rowpv != null)
                {
                    this.ImpostaCursore(Scci.Enums.enum_app_cursors.WaitCursor);

                                        CoreStatics.CoreApplication.MovParametroVitaleSelezionato = new MovParametroVitale(rowepisodio.Cells["CodUA"].Text,
                                                                                                        rowepisodio.Cells["IDPaziente"].Text,
                                                                                                        rowepisodio.Cells["IDEpisodio"].Text,
                                                                                                        rowepisodio.Cells["IDTrasferimento"].Text, CoreStatics.CoreApplication.Ambiente);

                    CoreStatics.CoreApplication.MovParametroVitaleSelezionato.CodTipoParametroVitale = rowpv.Cells["Codice"].Text;
                    CoreStatics.CoreApplication.MovParametroVitaleSelezionato.DescrTipoParametroVitale = rowpv.Cells["Descrizione"].Text;
                    CoreStatics.CoreApplication.MovParametroVitaleSelezionato.CodScheda = rowpv.Cells["CodScheda"].Text;

                    CoreStatics.CoreApplication.MovParametroVitaleSelezionato.Azione = EnumAzioni.INS;
                    CoreStatics.CoreApplication.MovParametroVitaleSelezionato.DataEvento = DateTime.Now;

                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingParametriVitali) == DialogResult.OK)
                    {
                        CaricaGrigliaParametri();
                        SelezionaParametroDaInserire();
                    }

                }
                else
                    easyStatics.EasyMessageBox("Seleziona un Paziente ed un Parametro rilevabile!", "Parametri Vitali", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubRileva_Click", this.Name);
            }
            finally
            {
                CoreStatics.CoreApplication.MovParametroVitaleSelezionato = null;
                this.txtCercaPaziente.Focus();
                this.ImpostaCursore(Scci.Enums.enum_app_cursors.DefaultCursor);
            }
        }

        private void ubNonRilevare_Click(object sender, EventArgs e)
        {
            try
            {
                UltraGridRow rowepisodio = RigaSelezionata(ref this.ucEasyGridEpisodi);
                UltraGridRow rowpv = RigaSelezionata(ref this.ucEasyGridParametri);
                if (rowepisodio != null && rowpv != null)
                {
                    this.ImpostaCursore(Scci.Enums.enum_app_cursors.WaitCursor);

                                        CoreStatics.CoreApplication.MovParametroVitaleSelezionato = new MovParametroVitale(rowepisodio.Cells["CodUA"].Text,
                                                                                    rowepisodio.Cells["IDPaziente"].Text,
                                                                                    rowepisodio.Cells["IDEpisodio"].Text,
                                                                                    rowepisodio.Cells["IDTrasferimento"].Text, CoreStatics.CoreApplication.Ambiente);

                    CoreStatics.CoreApplication.MovParametroVitaleSelezionato.CodTipoParametroVitale = rowpv.Cells["Codice"].Text;
                    CoreStatics.CoreApplication.MovParametroVitaleSelezionato.DescrTipoParametroVitale = rowpv.Cells["Descrizione"].Text;
                    CoreStatics.CoreApplication.MovParametroVitaleSelezionato.CodScheda = rowpv.Cells["CodScheda"].Text;
                    CoreStatics.CoreApplication.MovParametroVitaleSelezionato.CodStatoParametroVitale = "AN";
                    CoreStatics.CoreApplication.MovParametroVitaleSelezionato.Azione = EnumAzioni.INS;
                    CoreStatics.CoreApplication.MovParametroVitaleSelezionato.DataEvento = DateTime.Now;

                    if (CoreStatics.CoreApplication.MovParametroVitaleSelezionato.Salva())
                    {
                        CaricaGrigliaParametri();
                        SelezionaParametroDaInserire();
                    }

                }
                else
                    easyStatics.EasyMessageBox("Seleziona un Paziente ed un Parametro rilevabile!", "Parametri Vitali", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubRileva_Click", this.Name);
            }
            finally
            {
                CoreStatics.CoreApplication.MovParametroVitaleSelezionato = null;
                this.txtCercaPaziente.Focus();
                this.ImpostaCursore(Scci.Enums.enum_app_cursors.DefaultCursor);
            }
        }

                                                                                
        #endregion

    }
}
