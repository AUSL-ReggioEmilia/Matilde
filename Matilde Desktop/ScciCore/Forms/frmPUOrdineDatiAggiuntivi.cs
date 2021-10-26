using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Scci.Enums;
using Infragistics.Win.UltraWinGrid;
using UnicodeSrl.Framework.Data;
using Infragistics.Win;
using Infragistics.Win.Misc;
using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.Scci.PluginClient;
using UnicodeSrl.DatiClinici.DC;
using UnicodeSrl.Scci.Model.Strutture;

namespace UnicodeSrl.ScciCore
{
    public partial class frmPUOrdineDatiAggiuntivi : frmBaseModale, Interfacce.IViewFormlModal
    {

        #region Declare

                ucDatiAggiuntiviPopUp _datiaggiuntivipopup = new ucDatiAggiuntiviPopUp();
                private ucSegnalibri _ucSegnalibri = null;
        private Graphics g = null;

        #endregion

        public frmPUOrdineDatiAggiuntivi()
        {
            InitializeComponent();

            this.dcv = new WpfControls40.ucDcViewer();
            this.ehViewer.Child = this.dcv;

            this.ShowCpyButton = true;
            this.ShowDatoCpyButton = true;
        }

        #region Properies

                                private bool ShowCpyButton { get; set; }

                                private bool ShowDatoCpyButton { get; set; }

                                public Gestore Gestore { get; private set; }

        public bool PercorsoAmbulatoriale { get; set; }

        #endregion

        #region Interface

        public void Carica()
        {
            try
            {
                                checkCustomParameters();

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_ORDINE_256);

                
                                this.InizializzaPulsanti();

                                this.InizializzaGriglie();

                                this.CaricaOrdine();

                                this.AbilitaInoltro();


                                this.ShowDialog();
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "Carica", this.Text);
            }
        }

                                private void checkCustomParameters()
        {
            
                        this.ShowCpyButton = true;
            this.ShowDatoCpyButton = true;
                        if (this.CustomParamaters == null) return;

            Array array = this.CustomParamaters as Array;
            if (array == null ) return;

            String[] cpar = (String[])array;
            if (cpar.FirstOrDefault(x => x == MovOrdine.K_OECPY_NOBTN) != null) this.ShowCpyButton = false;
            if (cpar.FirstOrDefault(x => x == MovOrdine.K_OECPY_NOROWBTN) != null) this.ShowDatoCpyButton = false;

        }

        #endregion

        #region Functions

        private void InizializzaPulsanti()
        {
            try
            {
                                this.ubInoltraOrdine.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_FRECCIADX_256);
                this.ubInoltraOrdine.PercImageFill = 0.75F;
                this.ubInoltraOrdine.Visible = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Ordini_Inoltra);

                                if (this.PercorsoAmbulatoriale)
                {
                                        this.ubCopiaDati.Visible = false;
                                        this.tlpDatiAggiuntivi.RowStyles[0].Height = 0F;
                    this.tlpDatiAggiuntivi.RowStyles[1].Height = 92F;
                }
                else
                {
                    this.ubCopiaDati.Visible = this.ShowCpyButton;
                                        if (this.ShowCpyButton)
                    {
                        this.tlpDatiAggiuntivi.RowStyles[0].Height = 8F;
                        this.tlpDatiAggiuntivi.RowStyles[1].Height = 84F;
                    }
                    else
                    {
                        this.tlpDatiAggiuntivi.RowStyles[0].Height = 0F;
                        this.tlpDatiAggiuntivi.RowStyles[1].Height = 92F;
                    }

                }

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "InizializzaPulsanti", this.Text);
            }
        }

        private void InizializzaGriglie()
        {
            CoreStatics.SetEasyUltraGridLayout(ref this.ucEasyGridPrestazioni);
        }

        private void CaricaOrdine()
        {
            try
            {
                                if (CoreStatics.CoreApplication.MovOrdineSelezionato.DataProgrammazione != DateTime.MinValue)
                    this.lblDataOraProgrammazione.Text = CoreStatics.CoreApplication.MovOrdineSelezionato.DataProgrammazione.ToString("dddd dd/MM/yyyy HH:mm");
                else
                    this.lblDataOraProgrammazione.Text = string.Empty;

                this.lblPriorita.Text = CoreStatics.GetEnumDescription(CoreStatics.CoreApplication.MovOrdineSelezionato.Priorita);

                this.lblStatoValidita.Text = CoreStatics.GetEnumDescription(CoreStatics.CoreApplication.MovOrdineSelezionato.StatoValiditaOrdine);

                                this.ucEasyGridPrestazioni.DisplayLayout.Bands[0].Columns.ClearUnbound();
                this.ucEasyGridPrestazioni.DataSource =
                    CoreStatics.CoreApplication.MovOrdineSelezionato.CaricaPrestazioniSelezionate();
                this.ucEasyGridPrestazioni.Refresh();

                                if (g == null) g = this.CreateGraphics();

                                this.ImpostaGroupByGrigliaPrestazioni(ref this.ucEasyGridPrestazioni, ref g, "DescErogante", true);
                this.ucEasyGridPrestazioni.DisplayLayout.Bands[0].SortedColumns["DescErogante"].GroupByRowAppearance.BackGradientStyle = Infragistics.Win.GradientStyle.None;
                this.ucEasyGridPrestazioni.DisplayLayout.Bands[0].Override.ExpansionIndicator = Infragistics.Win.UltraWinGrid.ShowExpansionIndicator.Default;
                this.ucEasyGridPrestazioni.DisplayLayout.Bands[0].Override.GroupByRowExpansionStyle = GroupByRowExpansionStyle.DoubleClick;

                                                                                                
            

                                
                                this.Gestore = CoreStatics.CoreApplication.MovOrdineSelezionato.CaricaDatiOE(this.ShowDatoCpyButton);

                                this.ehViewer.Focus();

                                if (this.ucEasyGridPrestazioni.ActiveRow != null) this.ucEasyGridPrestazioni.ActiveRow = null;
                if (this.ucEasyGridPrestazioni.Selected.Rows.Count > 0) this.ucEasyGridPrestazioni.Selected.Rows.Clear();

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "CaricaOrdine", this.Text);
            }
        }

        private bool SalvaDatiAccessori()
        {
            bool ret = CoreStatics.CoreApplication.MovOrdineSelezionato.SalvaDatiOE(this.Gestore.SchedaDati);

            return ret;
        }

        private void AbilitaInoltro()
        {
                        this.ubInoltraOrdine.Enabled = CoreStatics.CoreApplication.MovOrdineSelezionato.Inoltrabile;

            this.lblStatoValidita.Text = CoreStatics.GetEnumDescription(CoreStatics.CoreApplication.MovOrdineSelezionato.StatoValiditaOrdine);
        }

                                                                private void ImpostaGroupByGrigliaPrestazioni(ref ucEasyGrid griglia, ref Graphics g, string colonnadaraggruppare, bool expandall)
        {

            griglia.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            griglia.DisplayLayout.GroupByBox.Hidden = true;
            if (griglia.DisplayLayout.Bands[0].SortedColumns.Count <= 0)
            {
                try
                {
                    griglia.DisplayLayout.Bands[0].SortedColumns.Add(colonnadaraggruppare, false, true);
                    griglia.DisplayLayout.Bands[0].SortedColumns[colonnadaraggruppare].GroupByRowAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Medium);

                    griglia.DisplayLayout.Bands[0].SortedColumns[colonnadaraggruppare].GroupByRowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Opaque;
                    griglia.DisplayLayout.Bands[0].SortedColumns[colonnadaraggruppare].GroupByRowAppearance.BackColor = Color.FromArgb(219, 207, 233);
                    
                }
                catch (Exception)
                {
                }
            }
            griglia.DisplayLayout.Bands[0].Override.ExpansionIndicator = Infragistics.Win.UltraWinGrid.ShowExpansionIndicator.Never;
            griglia.DisplayLayout.Bands[0].Override.GroupByRowExpansionStyle = GroupByRowExpansionStyle.Disabled;
            griglia.DisplayLayout.Bands[0].Override.GroupByRowPadding = CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XXXSmall), g.DpiY);
            griglia.DisplayLayout.Bands[0].Override.GroupByRowSpacingBefore = CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XXXSmall), g.DpiY);
            griglia.DisplayLayout.Bands[0].Override.GroupByRowDescriptionMask = "[Value]";

            if (expandall)
            {
                griglia.Rows.ExpandAll(true);
            }
            else
            {
                griglia.Rows.CollapseAll(true);
            }

        }

        private void checkAndFixSections()
        {
            Gestore gestSel = this.Gestore;

            foreach (DcSezione dcSez in gestSel.Scheda.Sezioni.Values)
            {
                 if (dcSez.Voci.Count > 0)
                {
                                        DcVoce firstVoce = dcSez.Voci.Values.ToArray()[0];

                                        List<DcDato> listDatiVoce = gestSel.SchedaDati.Dati.Values.Where(x => x.ID == firstVoce.ID).ToList();
                    int min = listDatiVoce.Min(dato => dato.Sequenza);
                    int max = min;

                    if (min > 1)
                    {
                                                max = listDatiVoce.Max(dato => dato.Sequenza);
                        int newIndex = 1;

                        for (int i = min; i <= max; i++)
                        {
                            shiftSequenza(ref gestSel, dcSez, i, newIndex);
                            newIndex++;
                        }
                    } 
                                        min = listDatiVoce.Min(dato => dato.Sequenza);
                    max = listDatiVoce.Max(dato => dato.Sequenza);

                    int newRow = 0;

                    for (int i = min; i <= max; i++)
                    {
                        bool datoExist = listDatiVoce.Exists(dato => dato.Sequenza == i);

                        if (datoExist == false)
                        {
                            if (i == max)
                                                                gestSel.CancellaRigaAt(dcSez.Key, i);
                            else
                            {
                                                                var validRows = listDatiVoce.Where(x => x.Sequenza < (i + 1));
                                newRow = validRows.Max(x => x.Sequenza) + 1;
                                shiftSequenza(ref gestSel, dcSez, i + 1, newRow);
                            }
                        }

                    }

                } 
            } 
                        
            List<DcDato> dati = this.Gestore.SchedaDati.Dati.Values.ToList();

            this.Gestore.SchedaDati.Dati.Clear();

            foreach (DcDato item in dati)
            {
                this.Gestore.SchedaDati.Dati.Add(item);
            }
        }

                                                                private void shiftSequenza(ref Gestore gestSel, DcSezione dcSez, int fromIndex, int toIndex)
        {
                        IEnumerable<DcDato> enDatiToChange = gestSel.SchedaDati.Dati.Values.Where
                (
                    dato => ((dcSez.Voci.Values.FirstOrDefault(v => v.ID == dato.ID) != null) && (dato.Sequenza == fromIndex))
                );

            Dictionary<String, DcDato> keysToUpdate = new Dictionary<string, DcDato>();

            foreach (DcDato datoObj in enDatiToChange)
            {
                string oldKey = datoObj.Key;
                datoObj.Sequenza = toIndex;

                keysToUpdate.Add(oldKey, datoObj);
            }

                        foreach (KeyValuePair<String, DcDato> kvp in keysToUpdate)
            {
                gestSel.SchedaDati.Dati.Remove(kvp.Key);
                gestSel.SchedaDati.Dati.Add(kvp.Value);
            }

        }

        #endregion

        #region Events

        private void frmPUOrdineDatiAggiuntivi_Shown(object sender, EventArgs e)
        {
            this.caricaDatiScheda();
            
            this.dcv.ButtonEvent += dcv_ButtonEvent;
            this.dcv.OnModifiedEvent += dcv_OnModifiedEvent;
        }

        private void ucEasyGridPrestazioni_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            Graphics g = this.CreateGraphics();
            int refWidth = CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.ucEasyGridPrestazioni.DataRowFontRelativeDimension), g.DpiY) * 3;
            g.Dispose();
            g = null;


            foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
            {
                try
                {
                    switch (oCol.Key)
                    {
                        case "DescErogante":
                            oCol.Hidden = false;
                            oCol.Header.Caption = "";
                                                                                                                                                                                                                                                                                        break;

                        case "Descrizione":
                            oCol.Hidden = false;
                            oCol.Header.Caption = "Prestazione";
                                                                                                                                                                                                                                                                                        break;

                        case "Stato":
                            oCol.Hidden = true;
                            try
                            {
                                oCol.MaxWidth = Convert.ToInt32(refWidth * 2);
                                oCol.MinWidth = oCol.MaxWidth;
                                oCol.Width = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }
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

        }

        private void ubInoltraOrdine_Click(object sender, EventArgs e)
        {
            string smsg = string.Empty;
            try
            {
                this.ImpostaCursore(enum_app_cursors.WaitCursor);

                Dictionary<string, object> datiObbli = this.dcv.Gestore.ControlloDatiObbligatori();

                Console.WriteLine("Dcv_OnChangedEvent: " + datiObbli.Count.ToString());

                if (datiObbli.Count != 0)
                {
                                        CoreStatics.CoreApplication.MovOrdineSelezionato.StatoValiditaOrdine = OEValiditaOrdine.NonValido;
                    this.AbilitaInoltro();
                }
                else
                {
                    if (this.SalvaDatiAccessori())
                    {
                        if (CoreStatics.CoreApplication.MovOrdineSelezionato.InoltraOrdine())
                        {

                            if (CoreStatics.CoreApplication.MovOrdineSelezionato != null && CoreStatics.CoreApplication.MovOrdineSelezionato.StatoValiditaOrdine != OEValiditaOrdine.Valido)
                            {
                                easyStatics.EasyMessageBox("Impossibile inoltrare ordine, ritornato stato validazione non valido.", "Inoltro Ordine", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            }
                            else
                            {

                                                                                                                                Risposta oRispostaElaboraPrima = PluginClientStatics.PluginClient(EnumPluginClient.OE_INOLTRA_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovOrdineSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                                if (oRispostaElaboraPrima.Successo == true)
                                {
                                                                    }

                                this.ubInoltraOrdine.Enabled = false;
                                this.dcv.IsEnabled = false;
                                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                                this.Close();
                            }
                        }
                        else
                        {
                            smsg = "Errore nell'inoltro dell'ordine selezionato." + Environment.NewLine +
                                    "Vuoi tornare alla maschera di compilazione dati ?";
                            if (easyStatics.EasyMessageBox(smsg, "Inoltro Ordine", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.No)
                            {
                                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                                this.Close();
                            }
                        }
                    }
                    else
                    {
                        smsg = "Errore nel salvataggio dei dati accessori inseriti." + Environment.NewLine +
                               "Vuoi tornare alla maschera di compilazione dati ?";
                        if (easyStatics.EasyMessageBox(smsg, "Salvataggio Dati Accessori", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.No)
                        {
                            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                            this.Close();
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ubInoltraOrdine_Click", this.Text);
            }
            finally
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }
        }

        private void frmPUOrdineDatiAggiuntivi_ImmagineClick(object sender, ImmagineTopClickEventArgs e)
        {

            try
            {

                switch (e.Tipo)
                {

                    case EnumImmagineTop.Segnalibri:
                        if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Bookmarks > 0)
                        {
                            CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerSegnalibri);
                            _ucSegnalibri = new ucSegnalibri();
                            int iWidth = Convert.ToInt32(easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large)) * 40;
                            int iHeight = Convert.ToInt32((double)iWidth / 1.52D);
                            _ucSegnalibri.Size = new Size(iWidth, iHeight);
                            _ucSegnalibri.ViewInit();
                            _ucSegnalibri.Focus();
                            this.UltraPopupControlContainerSegnalibri.Show();
                        }
                        break;

                    case EnumImmagineTop.SegnalibroAdd:
                        break;

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void frmPUOrdineDatiAggiuntivi_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            try
            {
                this.ImpostaCursore(enum_app_cursors.WaitCursor);

                if (this.SalvaDatiAccessori())
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }
                else
                {
                    string smsg = string.Empty;
                    smsg = "Errore nel salvataggio dei dati accessori inseriti." + Environment.NewLine +
                           "Vuoi tornare alla maschera di compilazione dati ?";
                    if (easyStatics.EasyMessageBox(smsg, "Salvataggio Dati Accessori", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.No)
                    {
                        this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                        this.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "SalvaDatiAccessori", this.Text);
            }
            finally
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }
        }

        private void frmPUOrdineDatiAggiuntivi_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void ucEasyGridPrestazioni_AfterRowActivate(object sender, EventArgs e)
        {
            try
            {
                if (this.ucEasyGridPrestazioni.ActiveRow.IsGroupByRow)
                {
                    this.ucEasyGridPrestazioni.DisplayLayout.Bands[0].Override.SelectedRowAppearance.BackGradientStyle = Infragistics.Win.GradientStyle.None;
                    this.ucEasyGridPrestazioni.DisplayLayout.Bands[0].Override.SelectedRowAppearance.BackColor = this.ucEasyGridPrestazioni.DisplayLayout.Bands[0].SortedColumns[0].GroupByRowAppearance.BackColor;
                    this.ucEasyGridPrestazioni.DisplayLayout.Bands[0].Override.SelectedRowAppearance.ForeColor = System.Drawing.SystemColors.ControlText;
                    this.ucEasyGridPrestazioni.DisplayLayout.Bands[0].Override.SelectedRowAppearance.BorderColor = Color.Blue;
                }
            }
            catch
            {
            }
        }


        #region     Button Events

                                        void dcv_ButtonEvent(string id)
        {

            if (id.Contains(CoreStatics.CoreApplication.MovOrdineSelezionato.C_BUTTON_PREFIX))
            {
                string cmdID = "";

                int index = id.IndexOf(CoreStatics.CoreApplication.MovOrdineSelezionato.C_BUTTON_PREFIX) + CoreStatics.CoreApplication.MovOrdineSelezionato.C_BUTTON_PREFIX.Length;
                cmdID = id.Substring(index);

                btnEvt_Cdss(cmdID);
            }
            else if (id.Contains(CoreStatics.CoreApplication.MovOrdineSelezionato.C_BUTTON_PREFIX_SRC))
            {
                string cmdID = "";

                int index = id.IndexOf(CoreStatics.CoreApplication.MovOrdineSelezionato.C_BUTTON_PREFIX_SRC) + CoreStatics.CoreApplication.MovOrdineSelezionato.C_BUTTON_PREFIX_SRC.Length;
                cmdID = id.Substring(index);

                btnEvt_SearchCopy(cmdID);
            }
            else if (id.Contains(CoreStatics.CoreApplication.MovOrdineSelezionato.C_BUTTON_PREFIX_2))
            {
                string cmdID = "";

                int index = id.IndexOf(CoreStatics.CoreApplication.MovOrdineSelezionato.C_BUTTON_PREFIX_2) + CoreStatics.CoreApplication.MovOrdineSelezionato.C_BUTTON_PREFIX_2.Length;
                cmdID = id.Substring(index);

                btnEvt_Cdss2(cmdID);
            }

        }

                                        private void btnEvt_Cdss(string id)
        {

            string _codua = string.Empty;

            string sNewID = id.Replace(CoreStatics.CoreApplication.MovOrdineSelezionato.C_BUTTON_PREFIX, "");

            try
            {

                                                                this.ImpostaCursore(enum_app_cursors.WaitCursor);
                this.Tag = sNewID;
                if (CoreStatics.CoreApplication.Trasferimento != null)
                {
                    _codua = CoreStatics.CoreApplication.Trasferimento.CodUA;
                }
                else
                {
                    _codua = CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.CodUAAmbulatorialeSelezionata;
                }
                string[] campo = sNewID.Split('_');
                string azione = string.Format("OE{0}.{1}", "Scheda", "Bottone");
                object[] myparam = new object[4] { this, campo[0], int.Parse(campo[1]), this.dcv.Gestore };

                Risposta oRisposta = PluginClientStatics.PluginClient(azione, myparam, CommonStatics.UAPadri(_codua, CoreStatics.CoreApplication.Ambiente));
                if (oRisposta.Successo == true)
                {
                }
                else
                {
                    if (oRisposta.ex != null)
                    {
                        Exception rex = oRisposta.ex;
                        CoreStatics.ExGest(ref rex, @"Si è verificato un errore nell'elaborazione della procedura.", "dcv_ButtonEvent", this.Text, true, this.Name, "Errore", MessageBoxIcon.Warning, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);
                    }
                    else
                    {
                        easyStatics.EasyMessageBox(oRisposta.Parameters[0].ToString(), azione, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                this.Tag = null;


            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "dcv_ButtonEvent", this.Text);
            }
            finally
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }

        }

                                        private void btnEvt_Cdss2(string id)
        {

            string _codua = string.Empty;

            string sNewID = id.Replace(CoreStatics.CoreApplication.MovOrdineSelezionato.C_BUTTON_PREFIX_2, "");

            try
            {

                                                                this.ImpostaCursore(enum_app_cursors.WaitCursor);
                this.Tag = sNewID;
                if (CoreStatics.CoreApplication.Trasferimento != null)
                {
                    _codua = CoreStatics.CoreApplication.Trasferimento.CodUA;
                }
                else
                {
                    _codua = CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.CodUAAmbulatorialeSelezionata;
                }
                string[] campo = sNewID.Split('_');
                string azione = string.Format("OE{0}.{1}", "Scheda", "Bottone2");
                object[] myparam = new object[4] { this, campo[0], int.Parse(campo[1]), this.dcv.Gestore };

                Risposta oRisposta = PluginClientStatics.PluginClient(azione, myparam, CommonStatics.UAPadri(_codua, CoreStatics.CoreApplication.Ambiente));
                if (oRisposta.Successo == true)
                {
                }
                else
                {
                    if (oRisposta.ex != null)
                    {
                        Exception rex = oRisposta.ex;
                        CoreStatics.ExGest(ref rex, @"Si è verificato un errore nell'elaborazione della procedura.", "dcv_ButtonEvent", this.Text, true, this.Name, "Errore", MessageBoxIcon.Warning, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);
                    }
                    else
                    {
                        if (oRisposta.Parameters != null)
                        {
                            easyStatics.EasyMessageBox(oRisposta.Parameters[0].ToString(), azione, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        
                    }
                }
                this.Tag = null;


            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "dcv_ButtonEvent", this.Text);
            }
            finally
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }

        }

                                        private void btnEvt_SearchCopy(string id)
        {
            using (frmOeCercaCopiaPrec f = new frmOeCercaCopiaPrec(CoreStatics.CoreApplication.MovOrdineSelezionato, id, this.Gestore.Decodifiche))
            {
                f.StartPosition = FormStartPosition.CenterParent;
                DialogResult r = f.ShowDialog(this);

                if (r == DialogResult.OK)
                {
                                        updateDcCopia(f.IdCampo, f.SequenzaCampo.Value, f.SelectionList);
                    this.caricaDatiScheda();
                }

            }

        }

        #endregion  Button Events

        private void dcv_OnModifiedEvent(string id)
        {

                        
            Dictionary<string, object> datiObbli = this.dcv.Gestore.ControlloDatiObbligatori();

            
            if (datiObbli.Count == 0)
            {
                                CoreStatics.CoreApplication.MovOrdineSelezionato.StatoValiditaOrdine = OEValiditaOrdine.Valido;
            }
            else
            {
                                CoreStatics.CoreApplication.MovOrdineSelezionato.StatoValiditaOrdine = OEValiditaOrdine.NonValido;
            }

            this.AbilitaInoltro();

        }

        private void ubCopiaDati_Click(object sender, EventArgs e)
        {
                        
                        StringBuilder builder = new StringBuilder();
            string codEroganti = string.Empty;
            string sSql = string.Empty;
            DataTable dtOrdini = null;
            Gestore gestCopia = null;
            bool datiEditati = false;

            string pref = @"OE";

            try
            {
                this.ImpostaCursore(enum_app_cursors.WaitCursor);

                foreach (var erogante in CoreStatics.CoreApplication.MovOrdineSelezionato.ListaEroganti)
                {
                    builder.Append("'").Append(erogante.CodiceAzienda).Append("|").Append(erogante.Codice).Append("', ");
                }

                codEroganti = builder.ToString();
                if (codEroganti.Length > 2) codEroganti = codEroganti.Substring(0, codEroganti.Length - 2);

                                sSql = @"SELECT *
                        FROM T_MovOrdini
                        WHERE 
                            ID IN (SELECT IDOrdine FROM T_MovOrdiniEroganti WHERE CodTipoOrdine IN (" + codEroganti + @")) " + Environment.NewLine;

                sSql += @"  AND IDPaziente = '" + CoreStatics.CoreApplication.MovOrdineSelezionato.Paziente.ID + @"'
                            AND IDEpisodio = '" + CoreStatics.CoreApplication.MovOrdineSelezionato.IDEpisodio + @"'
                            AND IDOrdineOE <> '" + CoreStatics.CoreApplication.MovOrdineSelezionato.IDOrdine + @"'
                            AND CodStatoOrdine <> 'CA'
                           	AND StrutturaDatiAccessori IS NOT NULL
                           	AND LayoutDatiAccessori IS NOT NULL
                           	AND DatiDatiAccessori IS NOT NULL
                         ORDER BY IDNum ASC";

                dtOrdini = Database.GetDatatable(sSql);

                if (dtOrdini != null && dtOrdini.Rows.Count > 0)
                {
                                        foreach (DataRow dr in dtOrdini.Rows)
                    {

                        gestCopia = new Gestore();

                        gestCopia.SchedaXML = dr["StrutturaDatiAccessori"].ToString();
                        gestCopia.SchedaDatiXML = dr["DatiDatiAccessori"].ToString();
                        gestCopia.SchedaLayoutsXML = dr["LayoutDatiAccessori"].ToString();


                        String[] arCheckList = this.Gestore.SchedaDati.Dati.Keys.ToArray();

                        foreach (String dcKey in arCheckList)
                        {
                                                        String[] keysplitter = dcKey.Split('_');
                            string keyToFind = "";
                            string keyToFind_Old = "";

                            if (keysplitter.Length > 1)
                                keyToFind = keysplitter[0] + @"_";
                            else
                                keyToFind = dcKey;

                                                                                                                if (keyToFind.StartsWith(pref))
                            {
                                keyToFind_Old = keyToFind.Substring(2);
                            }

                                                        Func<KeyValuePair<string, DcDato>, bool> fpattern = new Func<KeyValuePair<string, DcDato>, bool>
                                (
                                    x => (
                                                                                                ((x.Key.StartsWith(keyToFind)) && (x.Value.Value != null) && (x.Value.Value.ToString() != ""))
                                            ||
                                                                                                ((x.Key.StartsWith(keyToFind_Old)) && (x.Value.Value != null) && (x.Value.Value.ToString() != ""))
                                         )
                                );

                            List<KeyValuePair<string, DcDato>> listCpy = gestCopia.SchedaDati.Dati.Where(fpattern).ToList();
                            List<KeyValuePair<string, DcDato>> listCpyFixed = new List<KeyValuePair<string, DcDato>>();

                                                                                    foreach (KeyValuePair<string, DcDato> kvp in listCpy)
                            {
                                if (kvp.Value.ID.StartsWith(pref) == false)
                                {
                                                                        string newID = pref + kvp.Value.ID;
                                    DcDato datoFixed = kvp.Value;
                                    datoFixed.ID = newID;
                                    KeyValuePair<string, DcDato> kvpFixed = new KeyValuePair<string, DcDato>(datoFixed.Key, datoFixed);

                                    listCpyFixed.Add(kvpFixed);
                                }
                                else
                                    listCpyFixed.Add(kvp);
                            }


                                                        if (listCpyFixed.Count > 0)
                            {
                                this.Gestore.SchedaDati.Dati.RemoveAll(x => x.Key.StartsWith(keyToFind));

                                                                this.Gestore.SchedaDati.Dati.AddRange(listCpyFixed);

                                datiEditati = true;
                            }

                        }



                        gestCopia = null;
                    }

                    if (datiEditati)
                    {
                                                checkAndFixSections();

                                                dcv_OnModifiedEvent("");

                                                this.caricaDatiScheda();
                    }

                }
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ubCopiaDati_Click", this.Text);
            }
            finally
            {
                this.caricaDatiScheda();
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }
        }

        #endregion

                                private void updateDcCopia (string idCampo, int idSequenza, List<SelectionObjectString> selDatiOE)
        {
            if (selDatiOE == null) return;
            if (selDatiOE.Count == 0) return;
                       
            
            int row = idSequenza;

            DcVoce dcVoce = this.Gestore.LeggeVoce(idCampo);
            int maxSequenze = this.Gestore.LeggeSequenze(idCampo);
                        for (int i = 0; i < selDatiOE.Count; i++)
            {
                                if (row > maxSequenze) this.Gestore.NuovaRiga(dcVoce.Padre.Key, row);

                                this.Gestore.ModificaValore(idCampo, row, selDatiOE[i].Valore);
                row++;
            }


        }

        #region     SchedeDati

                                private void caricaDatiScheda()
        {
            if (this.Gestore.SchedaDati == null) return;
            if (this.Gestore.SchedaDati.Dati.Values == null) return;

            int h = 2;
            int maxChar = 30;

                                    foreach (DcDato dcDato in this.Gestore.SchedaDati.Dati.Values)
            {
                string sval = Convert.ToString(dcDato.Value);
                DcLayout dcLayout = this.Gestore.SchedaLayouts.Layouts[dcDato.ID];

                if ( (dcLayout.TipoVoce == DatiClinici.Common.Enums.enumTipoVoce.Testo) && (sval.Length > maxChar) )
                {
                    this.Gestore.SchedaLayouts.Layouts[dcDato.ID].Attributi["AltezzaTipo"].Value = h.ToString();
                }


            }


            this.dcv.CaricaDati(this.Gestore);
        }

        #endregion  SchedeDati

        #region UltraPopupControlContainerSegnalibri

        private void UltraPopupControlContainerSegnalibri_Closed(object sender, EventArgs e)
        {
            _ucSegnalibri.SegnalibriClick -= UltraPopupControlContainerSegnalibri_ModificaClick;
        }

        private void UltraPopupControlContainerSegnalibri_Opened(object sender, EventArgs e)
        {
            _ucSegnalibri.SegnalibriClick += UltraPopupControlContainerSegnalibri_ModificaClick;
            _ucSegnalibri.Focus();
        }

        private void UltraPopupControlContainerSegnalibri_Opening(object sender, CancelEventArgs e)
        {
            UltraPopupControlContainer popup = sender as UltraPopupControlContainer;
            popup.PopupControl = _ucSegnalibri;
        }

        private void UltraPopupControlContainerSegnalibri_ModificaClick(object sender, SegnalibriClickEventArgs e)
        {

            try
            {

                switch (e.Pulsante)
                {

                    case EnumPulsanteSegnalibri.Modifica:
                        this.UltraPopupControlContainerSegnalibri.Close();
                        this.ucTopModale.Focus();
                        CoreStatics.CaricaPopup(EnumMaschere.Scheda, EnumEntita.SCH, e.ID);
                        break;

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        #endregion

    }
}
