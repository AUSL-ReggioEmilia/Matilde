using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTree;
using Infragistics.Win.UltraWinChart;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Scci.Model;

namespace UnicodeSrl.ScciCore
{
    public partial class frmPUGrafico : frmBaseModale, Interfacce.IViewFormlModal
    {

        #region DECLARE

        private const string C_COD_SEZIONE = "XXXNULLOXXX";
        private const string C_DES_SEZIONE = " Sezione NON definita";

        private const string C_COL_SEL = "sel";

        private bool _skipcaricagrafici = false;

        private bool _graphresized = false;

        private bool _from_ucEasyDateRange_ValueChanged = false;

        private ToolboxPerGrafici _defgraf = null;

        private DateTime _cachedatada = DateTime.MinValue;
        private DateTime _cachedataa = DateTime.MinValue;
        private DataTable _DataTableTipiLaboratorioXAsync = null;
        private DataTable _DataTableMovimentiLaboratorioXAsync = null;

        private bool _runtime = false;
        private bool _profilishown = false;

        UnicodeSrl.ScciCore.Common.MT.ScciWebSvcMT _mtLAB = null;

        ucEasyLabel _labelCaricamento = null;

        private DateTime _lastdtDa = DateTime.MinValue;
        private DateTime _lastdtA = DateTime.MinValue;
        private bool _stopDtValueChanged = false;

        private bool _lastLineeGiornaliereChecked = true;

        private Selezione _profiloCorrente = null;
        internal Selezione ProfiloVisualizzazioneCorrente
        {
            get
            {
                return _profiloCorrente;
            }
            set
            {
                _profiloCorrente = value;
            }
        }

        private bool _applicazioneProfiloDaCambioData = false;

        private List<string> _lstPVTSelezionati = new List<string>();


        private List<string> _lstCodPrestLabSelezionati = new List<string>();

        #endregion

        public frmPUGrafico()
        {
            this.InitializingUI = true;

            this.ImageCache = new Dictionary<string, Image>();

            InitializeComponent();
            this.ucEasyGriglione.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.None;

            this.InitializingUI = false;
        }

        #region         Prop

        private List<ucSCCIScatterChart> Grafici = new List<ucSCCIScatterChart>();

        private DateTime DataDa
        {
            get
            {
                if (this.udtFiltroDataDa.Value != null)
                    return getDataDa((DateTime)this.udtFiltroDataDa.Value);
                else
                    return DateTime.MinValue;
            }
        }

        private DateTime DataA
        {
            get
            {
                if (this.udtFiltroDataDa.Value != null)
                    return getDataA((DateTime)this.udtFiltroDataA.Value);
                else
                    return DateTime.MinValue;
            }
        }

        private bool Ambulatoriale
        {
            get
            {
                return String.IsNullOrEmpty(_defgraf.IDEpisodio);
            }
        }

        private bool InitializingUI { get; set; }

        private DataTable DataMarkers { get; set; }

        private List<MSP_SelMovSommGraficoExt> DataTerapie { get; set; }

        private Dictionary<String, Image> ImageCache { get; set; }

        #endregion

        #region INTERFACCIA

        public void Carica()
        {
            try
            {
                _profilishown = false;
                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_PARAMETRIVITALIGRAFICO_16);

                _defgraf = CoreStatics.CoreApplication.DefinizioneGraficoSelezionata;


                if (CoreStatics.CoreApplication.Ambiente.Contesto.ContainsKey(EnumEntita.XXX.ToString()))
                {
                    CoreStatics.CoreApplication.Ambiente.Contesto.Remove(EnumEntita.XXX.ToString());
                }
                CoreStatics.CoreApplication.Ambiente.Contesto.Add(EnumEntita.XXX.ToString(), null);

                this.InitializingUI = true;

                _lstCodPrestLabSelezionati = new List<string>();
                _lstPVTSelezionati = new List<string>();
                if (_defgraf.EntitaIniziale == EnumEntita.PVT && _defgraf.CodTipoIniziale != "") _lstPVTSelezionati.Add(_defgraf.CodTipoIniziale);

                InizializzaControlli();
                CaricaProfiliVisualizzazione("");

                this.InitializingUI = false;

                CaricaDati(true, false);
                _graphresized = false;

                this.CaricaDatiContesto();

                this.ShowDialog();

                if (this.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Carica", this.Name);
            }
        }

        #endregion

        #region VOID & FUNCTIONS

        private void checkLoadProfiloVisualizzazioneCorrente()
        {
            if (_profiloCorrente == null)
            {
                _profiloCorrente = new Selezione(EnumTipoSelezione.GRAF, CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
            }
        }

        private Image getOrAddImage(string idIcona)
        {
            if (this.ImageCache.ContainsKey(idIcona))
            {
                return this.ImageCache[idIcona];
            }
            else
            {
                int size = this.utvFiltroTipoPV.LeftImagesSize.Width;
                byte[] buffer = CoreStatics.GetImageForGrid(Convert.ToInt32(idIcona), size);
                Image img = DrawingProcs.GetImageFromByte(buffer);

                this.ImageCache.Add(idIcona, img);

                return img;
            }
        }

        private void InizializzaControlli()
        {
            try
            {
                _skipcaricagrafici = true;
                this.uchkDati.UNCheckedImage = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_VISUALIZZA_256);
                this.uchkDati.PercImageFill = 0.85F;
                this.uchkDati.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                this.uchkGrafico.UNCheckedImage = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_PARAMETRIVITALIGRAFICO_256);
                this.uchkGrafico.PercImageFill = 0.85F;
                this.uchkGrafico.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.uchkLineeGiornaliere.UNCheckedImage = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_AGENDAGIORNALIERA_256);
                this.uchkLineeGiornaliere.PercImageFill = 0.60F;
                this.uchkLineeGiornaliere.Appearance.ImageVAlign = Infragistics.Win.VAlign.Top;
                setLineeGiornaliereAbilitazione();

                if (CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ID == EnumMaschere.GraficiEvidenzaClinica)
                    this.ucEasyDateRange.PassatoRemoto = true;

                if (_defgraf.DataRicovero > DateTime.MinValue) this.ucEasyDateRange.DataEpisodio = CoreStatics.CoreApplication.Episodio.DataRicovero;
                setDateIniziali();

                this.uchkGrafico.Checked = true; this.uchkDati.Checked = !this.uchkGrafico.Checked;
                switchGriglioneGrafici();


                this.utvFiltroTipoPV.FullRowSelect = true;
                this.utvFiltroTipoPV.HideSelection = true;

                this.utvFiltroTipoLAB.FullRowSelect = true;
                this.utvFiltroTipoLAB.HideSelection = true;
                this.utvFiltroTipoLAB.TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
                this.utvFiltroTipoLAB.Nodes.Override.Sort = SortType.Ascending;

                this.utvFiltroTipoPRF.FullRowSelect = true;
                this.utvFiltroTipoPRF.HideSelection = true;
                this.utvFiltroTipoPRF.Scrollable = Infragistics.Win.UltraWinTree.Scrollbar.ShowIfNeeded;

                InitProfili();

                _skipcaricagrafici = false;

            }
            catch
            {
            }
        }

        private void setDateIniziali()
        {
            try
            {


                if (_defgraf.IDEpisodio != null && _defgraf.IDEpisodio.Trim() != "" && _defgraf.DataRicovero > DateTime.MinValue && _defgraf.EntitaIniziale != EnumEntita.PVT)
                {


                    this.ucEasyDateRange.Value = ucEasyDateRange.C_RNG_EPI;
                }
                else
                {
                    if (_defgraf.DataDaIniziale <= DateTime.MinValue && _defgraf.DataAIniziale <= DateTime.MinValue)
                    {


                        if (int.Parse(UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.NumeroGiorniEvidenzaClinica)) != 0)
                        {
                            this.udtFiltroDataDa.Value = DateTime.Now.AddDays(-int.Parse(UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.NumeroGiorniEvidenzaClinica)));
                            this.udtFiltroDataA.Value = DateTime.Now.AddDays(1).AddSeconds(-1);
                        }
                        else
                            this.ucEasyDateRange.Value = ucEasyDateRange.C_RNG_6M;

                    }
                    else
                    {


                        if (_defgraf.DataDaIniziale > DateTime.MinValue) this.udtFiltroDataDa.Value = _defgraf.DataDaIniziale;
                        if (_defgraf.DataAIniziale > DateTime.MinValue) this.udtFiltroDataA.Value = _defgraf.DataAIniziale;
                    }
                }
            }
            catch
            {
            }
        }

        private DateTime getDataDa(DateTime vDataDaInput)
        {
            return vDataDaInput.Date;
        }
        private DateTime getDataA(DateTime vDataAInput)
        {
            return vDataAInput.Date.AddDays(1).AddSeconds(-1);
        }

        private void setLineeGiornaliereAbilitazione()
        {
            DateTime datada = DateTime.MinValue;
            DateTime dataa = DateTime.MinValue;

            if (this.udtFiltroDataDa.Value != null) datada = getDataDa((DateTime)this.udtFiltroDataDa.Value);
            if (this.udtFiltroDataA.Value != null) dataa = getDataA((DateTime)this.udtFiltroDataA.Value);

            this.uchkLineeGiornaliere.Enabled = Database.LineeGiornaliereAbilitate(datada, dataa);
            if (!this.uchkLineeGiornaliere.Enabled)
                this.uchkLineeGiornaliere.Checked = false;
            else
                this.uchkLineeGiornaliere.Checked = _lastLineeGiornaliereChecked;
        }
        private bool getLineeGiornaliereVisibili()
        {
            return (this.uchkLineeGiornaliere.Enabled && this.uchkLineeGiornaliere.Checked);
        }

        private void switchGriglioneGrafici()
        {
            if (this.uchkGrafico.Checked)
            {
                this.utabGrafici.Tabs["tabGrafici"].Selected = true;
                this.utabGrafici.Tabs["tabGrafici"].Active = true;
            }
            else
            {
                this.utabGrafici.Tabs["tabGriglione"].Selected = true;
                this.utabGrafici.Tabs["tabGriglione"].Active = true;

            }
        }

        private void SvuotaGrafici()
        {
            SvuotaGrafici(false);
        }
        private void SvuotaGrafici(bool soloLab)
        {
            try
            {
                if (Grafici != null)
                {
                    if (Grafici.Count > 0)
                    {
                        for (int iGr = Grafici.Count - 1; iGr >= 0; iGr--)
                        {
                            if (!soloLab || Grafici[iGr].Entita == EnumEntita.EVC)
                            {
                                this.tabpageGrafici.Controls.Remove(Grafici[iGr]);
                                Grafici[iGr].Dispose();
                                Grafici[iGr] = null;
                                Grafici.RemoveAt(iGr);
                            }
                        }
                    }

                    if (soloLab)
                    {
                        if (Grafici.Count > 0)
                        {
                            RiposizionaGrafici();
                        }
                    }
                    else
                        Grafici.Clear();


                }

            }
            catch (Exception)
            {
            }
        }

        private ucSCCIScatterChart nuovoGrafico(string key, string titolo)
        {
            ucSCCIScatterChart graf = null;

            graf = new ucSCCIScatterChart();
            graf.Visible = false;

            if (key.ToUpper().IndexOf(EnumEntita.EVC.ToString().ToUpper()) == 0)
                graf.Entita = EnumEntita.EVC;
            else
                graf.Entita = EnumEntita.PVT;


            graf.BackColor = System.Drawing.Color.Transparent;
            graf.Name = "ucSCCIChart" + key.Replace("|", "");
            graf.Titolo.Text = titolo;
            graf.TitoloNascosto = (graf.Titolo.Text == "");


            graf.ViewInit();


            return graf;
        }

        private void RiposizionaGrafici()
        {
            try
            {

                if (Grafici.Count > 0)
                {

                    int iTop = 1;
                    int iLeft = 1;
                    int iWidth = 0;
                    int iHeight = 0;

                    switch (Grafici.Count)
                    {
                        case 1:
                            iWidth = this.utabGrafici.Width - 2;
                            iHeight = this.utabGrafici.Height - 2;
                            this.tabpageGrafici.AutoScroll = false;
                            break;

                        case 2:
                            iWidth = this.utabGrafici.Width - 2;
                            iHeight = (this.utabGrafici.Height / 2) - 2;
                            this.tabpageGrafici.AutoScroll = false;
                            break;

                        default:
                            iWidth = this.utabGrafici.Width - 2;
                            iHeight = (this.utabGrafici.Height / 3) - 2;
                            this.tabpageGrafici.AutoScroll = (Grafici.Count > 3);
                            if (Grafici.Count > 3) iWidth -= 20;
                            break;
                    }

                    for (int g = 0; g < Grafici.Count; g++)
                    {
                        try
                        {
                            ucSCCIScatterChart oGraf = Grafici[g];
                            oGraf.Size = new Size(iWidth, iHeight);
                            oGraf.Location = new Point(iLeft, iTop);

                            iTop += iHeight + 1;
                        }
                        catch (Exception)
                        {
                        }
                    }

                }
            }
            catch
            {
            }
        }

        private void CaricaDatiContesto()
        {

            List<object> iList = new List<object>();
            List<object> listparametri = new List<object>();

            listparametri.Add(getDataDa((DateTime)this.udtFiltroDataDa.Value));
            listparametri.Add(getDataA((DateTime)this.udtFiltroDataA.Value));
            iList.Add(listparametri);

            try
            {

                for (int g = 0; g < Grafici.Count; g++)
                {

                    try
                    {

                        ucSCCIScatterChart oGraf = Grafici[g];
                        List<object> listgrafico = new List<object>();
                        listgrafico.Add(oGraf.Titolo.Text);
                        listgrafico.Add(CoreStatics.ImageToBase64(oGraf.Grafico.Image, System.Drawing.Imaging.ImageFormat.Bmp));
                        iList.Add(listgrafico);

                    }
                    catch (Exception)
                    {

                    }

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            finally
            {

                if (!CoreStatics.CoreApplication.Ambiente.Contesto.ContainsKey(EnumEntita.XXX.ToString()))
                    CoreStatics.CoreApplication.Ambiente.Contesto.Add(EnumEntita.XXX.ToString(), null);

                CoreStatics.CoreApplication.Ambiente.Contesto[EnumEntita.XXX.ToString()] = iList;
            }

        }

        #region ASYNC

        private void caricaTerapie()
        {
            List<String> selNodes = new List<string>();

            foreach (UltraTreeNode node in this.utvFiltroTipoPRF.Nodes)
            {
                foreach (UltraTreeNode child in node.Nodes)
                {
                    if ((child.Visible) && (child.CheckedState == CheckState.Checked))
                        selNodes.Add(child.Key);
                }
            }

            this.utvFiltroTipoPRF.Nodes.Clear();
            this.DataMarkers = null;

            this.DataTerapie = _defgraf.GetSelMovSomministrazioniGraficoExt(this.DataDa, this.DataA);

            if (this.DataTerapie == null) return;
            if (this.DataTerapie.Count == 0) return;

            string filter = this.utxtFiltroTipoPRF.Text.ToUpper();

            List<MSP_SelMovSommGraficoExt> tableFiltered = this.DataTerapie
                                                                .Where(row => row.Descrizione.ToUpper().Contains(filter)).ToList();

            var results = from p in tableFiltered
                          group p by p.CodViaSomministrazione into g
                          select new
                          {
                              CodViaSomministrazione = g.Key,
                              ViaSomministrazine = g.Select(item => item.ViaSomministrazione).First(),
                              IDIcona = g.Select(item => item.IDIcona).First(),
                              Prescrizioni = g.Select(item => item)
                          };

            var groupList = results.OrderBy(x => x.ViaSomministrazine).ToList();

            try
            {
                this.InitializingUI = true;

                foreach (var rowViaSomm in groupList)
                {
                    UltraTreeNode oNode = new UltraTreeNode(rowViaSomm.CodViaSomministrazione, rowViaSomm.ViaSomministrazine);
                    oNode.Override.NodeStyle = NodeStyle.CheckBox;

                    if (rowViaSomm.IDIcona > 0)
                    {
                        Image img = this.getOrAddImage(rowViaSomm.IDIcona.ToString());
                        oNode.LeftImages.Add(img);
                        oNode.Override.NodeStyle = NodeStyle.Default;
                    }

                    this.utvFiltroTipoPRF.Nodes.Add(oNode);

                    foreach (MSP_SelMovSommGraficoExt child in rowViaSomm.Prescrizioni)
                    {
                        UltraTreeNode nodeChild = new UltraTreeNode(child.IDPrescrizione.ToString(), child.Descrizione);
                        nodeChild.Override.NodeStyle = NodeStyle.CheckBox;
                        oNode.Nodes.Add(nodeChild);

                        if (selNodes.Exists(x => x == nodeChild.Key))
                            nodeChild.CheckedState = CheckState.Checked;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.InitializingUI = false;
            }


        }

        private void caricaTipiPV()
        {

            DataTable dtpv = _defgraf.DataTableTipiParametriVitali(this.DataDa, this.DataA);

            if (dtpv != null)
            {
                for (int iNode = this.utvFiltroTipoPV.Nodes.Count - 1; iNode >= 0; iNode--)
                {
                    try
                    {
                        dtpv.DefaultView.RowFilter = @"CodTipo = '" + Database.testoSQL(this.utvFiltroTipoPV.Nodes[iNode].Key) + @"'";

                        if (dtpv.DefaultView.Count <= 0)
                            this.utvFiltroTipoPV.Nodes[iNode].Remove();

                        dtpv.DefaultView.RowFilter = @"";
                    }
                    catch (Exception)
                    {
                    }
                }

                foreach (DataRow row in dtpv.Rows)
                {
                    if (!this.utvFiltroTipoPV.Nodes.Exists(row["CodTipo"].ToString()))
                    {
                        UltraTreeNode oNode = new UltraTreeNode(row["CodTipo"].ToString(), row["DescTipo"].ToString());
                        oNode.Override.NodeStyle = NodeStyle.CheckBox;

                        _skipcaricagrafici = true;

                        if (_lstPVTSelezionati != null && _lstPVTSelezionati.Count > 0 && _lstPVTSelezionati.Contains(oNode.Key))
                            oNode.CheckedState = CheckState.Checked;
                        else
                            oNode.CheckedState = CheckState.Unchecked;


                        _skipcaricagrafici = false;

                        if (!row.IsNull("Icona"))
                        {
                            oNode.LeftImages.Add(DrawingProcs.GetImageFromByte((byte[])row["Icona"], this.utvFiltroTipoPV.LeftImagesSize));
                        }

                        this.utvFiltroTipoPV.Nodes.Add(oNode);
                    }







                }
            }



        }


        private void ShowUiTipiPV()
        {

            if (this.Ambulatoriale)
            {
                this.tlpParametri.RowStyles[0].Height = 0;
            }
            else
            {
                this.tlpParametri.RowStyles[0].Height = 50;
            }

            utxtFiltroTipoPV.Enabled = (this.utvFiltroTipoPV.Nodes.Count > 0);
            ubDeselectAll_PV.Enabled = (this.utvFiltroTipoPV.Nodes.Count > 0);
            utvFiltroTipoPV.Enabled = (this.utvFiltroTipoPV.Nodes.Count > 0);


        }

        private void ShowUiTipiTerapie()
        {
            if (this.Ambulatoriale)
            {
                this.utcSelezioni.Tabs["tabTerapie"].Visible = false;
            }
            else
            {
                this.utcSelezioni.Tabs["tabTerapie"].Visible = true;
            }

            utxtFiltroTipoPRF.Enabled = (this.utvFiltroTipoPV.Nodes.Count > 0);
            ubDeselectAll_PRF.Enabled = (this.utvFiltroTipoPV.Nodes.Count > 0);
            utvFiltroTipoPRF.Enabled = (this.utvFiltroTipoPV.Nodes.Count > 0);

        }

        private void ShowUiLAB()
        {
            if (utvFiltroTipoLAB.Nodes.Count > 0)
            {
                tlpTipiLab.Enabled = true;
            }
            else
            {
                tlpTipiLab.Enabled = false;
            }

        }

        private void CaricaDati(bool caricatipi, bool soloLab)
        {
            try
            {
                if (this.InitializingUI)
                    return;

                if (_defgraf == null)
                    return;

                if (_skipcaricagrafici)
                    return;

                this.ImpostaCursore(Scci.Enums.enum_app_cursors.WaitCursor, true);

                SvuotaGrafici(soloLab);

                _skipcaricagrafici = true;


                if (caricatipi)
                {
                    if (this.Ambulatoriale == false)
                    {
                        caricaTipiPV();

                        caricaTerapie();
                    }


                    if (_defgraf.IDSAC != null && _defgraf.IDSAC.Trim() != "")
                    {
                        CaricamentoMovimentiLaboratorioAsync();
                    }
                }

                this.ShowUiTipiPV();
                this.ShowUiTipiTerapie();
                this.ShowUiLAB();

                _skipcaricagrafici = false;
                CaricamentoGraficiXAsync(soloLab);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaDati", this.Name);
            }
            finally
            {
                _skipcaricagrafici = false;
                this.ImpostaCursore(Scci.Enums.enum_app_cursors.DefaultCursor, true);
            }

        }

        private void CaricamentoMovimentiLaboratorioAsync()
        {
            try
            {


                DateTime datada = DateTime.MinValue;
                DateTime dataa = DateTime.MinValue;
                if (this.udtFiltroDataDa.Value != null) datada = getDataDa((DateTime)this.udtFiltroDataDa.Value);
                if (this.udtFiltroDataA.Value != null) dataa = getDataA((DateTime)this.udtFiltroDataA.Value);

                if (_DataTableMovimentiLaboratorioXAsync == null || datada != _cachedatada || dataa != _cachedataa)
                {
                    SetPulsantiProfili(false);
                    chiudiThread(false);

                    if (_DataTableMovimentiLaboratorioXAsync != null)
                    {
                        _DataTableMovimentiLaboratorioXAsync.Dispose();
                        _DataTableMovimentiLaboratorioXAsync = null;
                    }
                    if (_DataTableTipiLaboratorioXAsync != null)
                    {
                        _DataTableTipiLaboratorioXAsync.Dispose();
                        _DataTableTipiLaboratorioXAsync = null;
                    }

                    mostraCaricamento(true);

                    _mtLAB = new Common.MT.ScciWebSvcMT(System.Threading.ThreadPriority.Normal, false, "ThreadLAB1");

                    _mtLAB.DatatableCompleted += mt_DatatableCompleted;
                    _mtLAB.LastThreadException += mt_LastThreadException;

                    _mtLAB.GetRisLabPaziente(_defgraf.IDSAC, datada, dataa);
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricamentoMovimentiLaboratorioAsync", this.Name);
            }
        }

        private void CaricamentoTVLabXAsync()
        {

            bool bEnableDeselectLAB = false;
            try
            {

                if (_DataTableTipiLaboratorioXAsync != null)
                {
                    for (int iSezNode = this.utvFiltroTipoLAB.Nodes.Count - 1; iSezNode >= 0; iSezNode--)
                    {
                        for (int iPNode = this.utvFiltroTipoLAB.Nodes[iSezNode].Nodes.Count - 1; iPNode >= 0; iPNode--)
                        {
                            try
                            {
                                _DataTableTipiLaboratorioXAsync.DefaultView.RowFilter = @"CodSezione = '" + Database.testoSQL((this.utvFiltroTipoLAB.Nodes[iSezNode].Key == C_COD_SEZIONE ? "" : this.utvFiltroTipoLAB.Nodes[iSezNode].Key)) + @"' And CodPrescrizione = '" + Database.testoSQL(this.utvFiltroTipoLAB.Nodes[iSezNode].Nodes[iPNode].Key.Split('|')[1]) + @"'";
                                if (_DataTableTipiLaboratorioXAsync.DefaultView.Count <= 0)
                                    this.utvFiltroTipoLAB.Nodes[iSezNode].Nodes[iPNode].Remove();
                                else
                                    bEnableDeselectLAB = true;
                                _DataTableTipiLaboratorioXAsync.DefaultView.RowFilter = @"";
                            }
                            catch (Exception)
                            {
                            }
                        }

                        try
                        {
                            if (this.utvFiltroTipoLAB.Nodes[iSezNode].Nodes.Count <= 0) this.utvFiltroTipoLAB.Nodes[iSezNode].Remove();
                        }
                        catch (Exception)
                        {
                        }

                    }

                    foreach (DataRow rowLab in _DataTableTipiLaboratorioXAsync.Rows)
                    {

                        string codsezione = (rowLab["CodSezione"].ToString() == string.Empty ? C_COD_SEZIONE : rowLab["CodSezione"].ToString());
                        string descrsezione = (codsezione == C_COD_SEZIONE ? C_DES_SEZIONE : rowLab["DescrSezione"].ToString());

                        UltraTreeNode oNodeSez = null;
                        if (!this.utvFiltroTipoLAB.Nodes.Exists(codsezione))
                        {
                            oNodeSez = new UltraTreeNode(codsezione, descrsezione);
                            oNodeSez.Override.NodeStyle = NodeStyle.Default;

                            oNodeSez.LeftImages.Add(ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_FOLDER_32));

                            this.utvFiltroTipoLAB.Nodes.Add(oNodeSez);
                            oNodeSez.Nodes.Override.Sort = SortType.Ascending;
                        }
                        else
                            oNodeSez = this.utvFiltroTipoLAB.Nodes[codsezione];

                        if (!oNodeSez.Nodes.Exists(oNodeSez.Key + @"|" + rowLab["CodPrescrizione"].ToString()))
                        {
                            UltraTreeNode oNodePr = new UltraTreeNode(oNodeSez.Key + @"|" + rowLab["CodPrescrizione"].ToString(), rowLab["DescPrescrizione"].ToString().Trim());
                            if (!rowLab.IsNull("UM"))
                            {
                                oNodePr.Text += @" [" + rowLab["UM"].ToString() + @"]";
                            }

                            oNodePr.Override.NodeStyle = NodeStyle.CheckBox;

                            oNodePr.LeftImages.Add(ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_PRESCRIZIONE_32));

                            oNodeSez.Nodes.Add(oNodePr);


                            if (_lstCodPrestLabSelezionati != null && _lstCodPrestLabSelezionati.Count > 0 && _lstCodPrestLabSelezionati.Contains(rowLab["CodPrescrizione"].ToString()))
                            {
                                oNodePr.CheckedState = CheckState.Checked;
                                if (!oNodeSez.Expanded) oNodeSez.Expanded = true;
                            }
                            else
                                oNodePr.CheckedState = CheckState.Unchecked;

                        }




                        bEnableDeselectLAB = true;

                    }
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricamentoTVLab", this.Name);
            }
            finally
            {
                this.ubDeselectAll_LAB.Enabled = bEnableDeselectLAB;
            }
        }
       
        private void CaricamentoGraficiXAsync(bool soloLab)
        {
            DateTime datada = this.DataDa;
            DateTime dataa = this.DataA;

            try
            {

                Dictionary<string, string> codicitipiPV = new Dictionary<string, string>();
                Dictionary<string, string> codicitipiLAB = new Dictionary<string, string>();
                
                if (!soloLab)
                {
                    if (_defgraf.IDEpisodio != null && _defgraf.IDEpisodio.Trim() != "" && this.utvFiltroTipoPV.Enabled)
                    {
                        foreach (UltraTreeNode onodepv in this.utvFiltroTipoPV.Nodes)
                        {
                            if (onodepv.Visible && onodepv.CheckedState == CheckState.Checked)
                            {
                                codicitipiPV.Add(onodepv.Key, onodepv.Text);
                            }
                        }
                    }
                }
                
                if (_defgraf.IDSAC != null && _defgraf.IDSAC.Trim() != "" && this.utvFiltroTipoLAB.Enabled && _DataTableTipiLaboratorioXAsync != null)
                {
                    List<string> arrTmpCodPrescr = new List<string>();
                    foreach (UltraTreeNode onodesezlab in this.utvFiltroTipoLAB.Nodes)
                    {
                        foreach (UltraTreeNode onodelab in onodesezlab.Nodes)
                        {
                            if (onodelab.Visible && onodelab.CheckedState == CheckState.Checked)
                            {                         
                                string codprescr = onodelab.Key.Split('|')[1];
                                if (!arrTmpCodPrescr.Contains(codprescr))
                                {
                                    arrTmpCodPrescr.Add(codprescr);
                                    
                                    string elencoSezioni = onodesezlab.Text + @"+";
                                    string codicisezioni = onodelab.Key.Split('|')[0] + @"_";
                                    foreach (UltraTreeNode onodesezlabS in this.utvFiltroTipoLAB.Nodes)
                                    {
                                        if (onodesezlabS.Visible && onodesezlabS.Key.Trim().ToUpper() != onodesezlab.Key.Trim().ToUpper())
                                        {
                                            foreach (UltraTreeNode onodelabS in onodesezlabS.Nodes)
                                            {
                                                if (onodelabS.Visible && onodelabS.Key.Split('|')[1].Trim().ToUpper() == codprescr.Trim().ToUpper())
                                                {
                                                    codicisezioni += onodesezlabS.Key + @"_";
                                                    if (elencoSezioni.Trim().ToUpper().IndexOf(@"+" + onodesezlabS.Text.Trim().ToUpper() + @"+") < 0 && elencoSezioni.Trim().ToUpper().IndexOf(onodesezlabS.Text.Trim().ToUpper() + @"+") != 0)
                                                        elencoSezioni += onodesezlabS.Text + @"+";
                                                }
                                            }
                                        }
                                    }
                                    if (codicisezioni != "") codicisezioni = codicisezioni.Substring(0, codicisezioni.Length - 1);
                                    if (elencoSezioni != "") elencoSezioni = elencoSezioni.Substring(0, elencoSezioni.Length - 1);
                                
                                    codicitipiLAB.Add(codicisezioni + @"|" + codprescr, elencoSezioni + @"|" + onodelab.Text);
                                }

                            }
                        }
                    }

                }

              
                _defgraf.CaricaTuttiIDatiFiltratiXAsync(ref _DataTableMovimentiLaboratorioXAsync, this.DataDa, this.DataA,
                                                        codicitipiPV, codicitipiLAB, soloLab);

                Dictionary<string, DataTable> dictDati = _defgraf.DataTablesPerGrafici;
                Dictionary<string, TipoParametroVitale> dictDimensioniTipoPV = _defgraf.TipiParametroVitale;

                this.ucEasyGriglione.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
                this.ucEasyGriglione.HeaderFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;
                this.ucEasyGriglione.DataSource = null;
                this.ucEasyGriglione.DataSource = _defgraf.DataTableGriglione;
                this.ucEasyGriglione.Refresh();

                if (dictDati.Count > 0)
                {
                    int iWidth = 0;
                    int iHeight = 0;
                    int iTop = 1;
                    int iLeft = 1;


                    Dictionary<string, Dictionary<string, List<string>>> splitGraficiXScale = new Dictionary<string, Dictionary<string, List<string>>>();

                    int iCountGrafici = Grafici.Count + dictDati.Count;
                    for (int iGraf = 0; iGraf < dictDati.Count; iGraf++)
                    {
                        string key = dictDati.ElementAt(iGraf).Key;
                        if (dictDimensioniTipoPV != null && dictDimensioniTipoPV.ContainsKey(key))
                        {
                            TipoParametroVitale tpvpg = dictDimensioniTipoPV[key];
                            Dictionary<string, List<string>> scale = new Dictionary<string, List<string>>();
                            foreach (DimensionePerGrafico dimpv in tpvpg.DimensioniPerGrafico)
                            {
                                string identificativoscala = dimpv.Scala.InizioScala.ToString("0.000000") + @"_" + dimpv.Scala.FineScala.ToString("0.000000");
                                if (scale.Count > 0 && !scale.ContainsKey(identificativoscala)) iCountGrafici += 1;
                                if (!scale.ContainsKey(identificativoscala))
                                {
                                    List<string> listacampi = new List<string>();
                                    listacampi.Add(dimpv.Nome);
                                    scale.Add(identificativoscala, listacampi);
                                }
                                else
                                    scale[identificativoscala].Add(dimpv.Nome);
                            }
                            splitGraficiXScale.Add(key, scale);
                        }
                    }



                    switch (iCountGrafici)
                    {
                        case 1:
                            iWidth = this.utabGrafici.Width - 2;
                            iHeight = this.utabGrafici.Height - 2;
                            this.tabpageGrafici.AutoScroll = false;
                            break;

                        case 2:
                            iWidth = this.utabGrafici.Width - 2;
                            iHeight = (this.utabGrafici.Height / 2) - 2;
                            this.tabpageGrafici.AutoScroll = false;
                            break;

                        default:
                            // >= 3
                            iWidth = this.utabGrafici.Width - 2;
                            iHeight = (this.utabGrafici.Height / 3) - 2;
                            this.tabpageGrafici.AutoScroll = (iCountGrafici > 3);
                            if (iCountGrafici > 3) iWidth -= 20;
                            break;
                    }

                    if (Grafici.Count > 0)
                    {
                        for (int iGr = 0; iGr < Grafici.Count; iGr++)
                        {
                            Grafici[iGr].Width = iWidth;
                            Grafici[iGr].Height = iHeight;
                            Grafici[iGr].Top = iTop;

                            iTop += iHeight + 1;
                        }                        
                    }
                    
                    DateTime dtmin = DateTime.MinValue;
                    DateTime dtMAX = DateTime.MinValue;
                    try
                    {
                        for (int iDT = 0; iDT < dictDati.Count; iDT++)
                        {
                            DataTable dt = dictDati.ElementAt(iDT).Value;
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                foreach (DataRow row in dt.Rows)
                                {
                                    if (!row.IsNull(_defgraf.CampoDataOraPerGrafici))
                                    {
                                        if (dtmin == DateTime.MinValue) dtmin = (DateTime)row[_defgraf.CampoDataOraPerGrafici];
                                        if (dtMAX == DateTime.MinValue) dtMAX = (DateTime)row[_defgraf.CampoDataOraPerGrafici];

                                        if (dtMAX < (DateTime)row[_defgraf.CampoDataOraPerGrafici]) dtMAX = (DateTime)row[_defgraf.CampoDataOraPerGrafici];
                                        if (dtmin > (DateTime)row[_defgraf.CampoDataOraPerGrafici]) dtmin = (DateTime)row[_defgraf.CampoDataOraPerGrafici];
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                    if (!dtmin.Date.Equals(dtMAX.Date))
                    {
                        if (dtmin > datada) dtmin = new DateTime(datada.Year, datada.Month, datada.Day, datada.Hour, datada.Minute, datada.Second);
                        if (dtMAX < dataa) dtMAX = new DateTime(dataa.Year, dataa.Month, dataa.Day, dataa.Hour, dataa.Minute, dataa.Second);
                    }

                    for (int iGraf = 0; iGraf < dictDati.Count; iGraf++)
                    {
                        string key = dictDati.ElementAt(iGraf).Key;
                        DataTable dtGrafico = dictDati.ElementAt(iGraf).Value;
                        
                        string titolo = "titolo di " + key;
                        string[] arrkey = key.Split('|');
                        if (arrkey[0] == EnumEntita.PVT.ToString())
                        {
                            titolo = @"Parametri Vitali: ";
                            for (int iNPV = 0; iNPV < this.utvFiltroTipoPV.Nodes.Count; iNPV++)
                            {
                                if (this.utvFiltroTipoPV.Nodes[iNPV].Visible && this.utvFiltroTipoPV.Nodes[iNPV].Key == arrkey[1])
                                {
                                    titolo += this.utvFiltroTipoPV.Nodes[iNPV].Text;
                                    // exit for
                                    iNPV = this.utvFiltroTipoPV.Nodes.Count + 1;
                                }
                            }
                        }
                        else
                        {
                            titolo = ""; 
                            string nomeprescr = "";
                            for (int iSezLAB = 0; iSezLAB < this.utvFiltroTipoLAB.Nodes.Count; iSezLAB++)
                            {                                
                                if (this.utvFiltroTipoLAB.Nodes[iSezLAB].Visible && arrkey[1].Split('_').Contains<string>(this.utvFiltroTipoLAB.Nodes[iSezLAB].Key))
                                {
                                    for (int iNLAB = 0; iNLAB < this.utvFiltroTipoLAB.Nodes[iSezLAB].Nodes.Count; iNLAB++)
                                    {
                                        if (this.utvFiltroTipoLAB.Nodes[iSezLAB].Nodes[iNLAB].Visible && this.utvFiltroTipoLAB.Nodes[iSezLAB].Nodes[iNLAB].Key.Split('|')[1] == arrkey[2])
                                        {
                                            if (nomeprescr == "")
                                            {
                                                nomeprescr = this.utvFiltroTipoLAB.Nodes[iSezLAB].Nodes[iNLAB].Text;                                              
                                            }
                                            
                                            if (titolo.ToUpper().IndexOf(@"+" + this.utvFiltroTipoLAB.Nodes[iSezLAB].Text.Trim().ToUpper() + @"+") < 0 && titolo.ToUpper().IndexOf(this.utvFiltroTipoLAB.Nodes[iSezLAB].Text.Trim().ToUpper() + @"+") != 0)
                                            {
                                                titolo += this.utvFiltroTipoLAB.Nodes[iSezLAB].Text + @"+";
                                            }

                                            iNLAB = this.utvFiltroTipoLAB.Nodes[iSezLAB].Nodes.Count + 1;
                                        }
                                    }
                                }
                              
                            }
                            if (titolo != "") titolo = titolo.Substring(0, titolo.Length - 1);
                            if (titolo.Trim() != "" && nomeprescr.Trim() != "") titolo += @" - ";
                            if (nomeprescr.Trim() != "") titolo += nomeprescr;
                            titolo = @"Laboratorio: " + titolo;

                        }

                        if (splitGraficiXScale.ContainsKey(key))
                        {
                            Dictionary<string, List<string>> dicScale = splitGraficiXScale[key];
                            for (int iSplit = 0; iSplit < dicScale.Count; iSplit++)
                            {
                                List<string> campidimensione = dicScale.ElementAt(iSplit).Value;

                                // inizio e fine scala
                                float inizioscala = float.MinValue;
                                float finescala = float.MinValue;
                                if (dictDimensioniTipoPV != null && dictDimensioniTipoPV.ContainsKey(key))
                                {
                                    TipoParametroVitale tpvpg = dictDimensioniTipoPV[key];
                                    foreach (DimensionePerGrafico dimpv in tpvpg.DimensioniPerGrafico)
                                    {
                                        if (campidimensione.Contains(dimpv.Nome))
                                        {
                                            if (dimpv.Scala.InizioScala > float.MinValue && (inizioscala == float.MinValue || inizioscala > dimpv.Scala.InizioScala)) inizioscala = dimpv.Scala.InizioScala;
                                            if (dimpv.Scala.FineScala > float.MinValue && (finescala == float.MinValue || finescala < dimpv.Scala.FineScala)) finescala = dimpv.Scala.FineScala;
                                        }
                                    }
                                }

                                ucSCCIScatterChart oGraf = nuovoGrafico(key + @"|" + iSplit.ToString(), titolo);
                                oGraf.Size = new Size(iWidth, iHeight);
                                oGraf.Location = new Point(iLeft, iTop);
                                oGraf.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

                                oGraf.CaricaDati(ref dtGrafico, inizioscala, finescala, dtmin, dtMAX, _defgraf.CampoDataOraPerGrafici, campidimensione, this.getLineeGiornaliereVisibili());

                                this.tabpageGrafici.Controls.Add(oGraf);
                                iTop += iHeight + 1;

                                Grafici.Add(oGraf);

                            }

                        }
                        else
                        {
                            ucSCCIScatterChart oGraf = nuovoGrafico(key, titolo);
                            oGraf.Size = new Size(iWidth, iHeight);
                            oGraf.Location = new Point(iLeft, iTop);
                            List<string> campidimensione = new List<string>();
                            foreach (DataColumn ocol in dtGrafico.Columns)
                            {
                                if (ocol.ColumnName != _defgraf.CampoDataOraPerGrafici && !campidimensione.Contains(ocol.ColumnName))
                                    campidimensione.Add(ocol.ColumnName);
                            }
                            oGraf.CaricaDati(ref dtGrafico, float.MinValue, float.MinValue, dtmin, dtMAX, _defgraf.CampoDataOraPerGrafici, campidimensione, this.getLineeGiornaliereVisibili());

                            this.tabpageGrafici.Controls.Add(oGraf);
                            iTop += iHeight + 1;

                            Grafici.Add(oGraf);

                        } 


                    } 

                    foreach (Control ctrl in Grafici)
                    {
                        ctrl.Visible = true;
                    }
                    

                } 

                this.CaricaDatiContesto();
               
                this.CaricaMarkers();
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricamentoGraficiXAsync", this.Name);
            }
        }

        private void CaricaMarkers()
        {
            List<ChartMarker> markers = new List<ChartMarker>();
            Color colorDefault = Color.Gray;

            try
            {

                if (this.DataMarkers != null)
                {
                    foreach (DataRow row in this.DataMarkers.Rows)
                    {
                        ChartMarker cm = new ChartMarker();
                        cm.DateOnChart = Convert.ToDateTime(row["DataErogazione"]);
                        cm.ToolTip = row["DescrizioneGrafico"].ToString();
                        cm.Tag = row["CodTipoPrescrizione"].ToString();

                        MSP_SelMovSommGraficoExt rowTipo = this.DataTerapie
    .Where(x => x.IDPrescrizione.ToString() == row["IDSistema"].ToString()).First();

                        string colorValue = rowTipo.ColoreGrafico;

                        Color c = colorDefault;

                        if (String.IsNullOrEmpty(colorValue) == false)
                        {
                            c = CoreStatics.GetColorFromString(colorValue);

                        }

                        cm.Background = c;
                        cm.BorderColor = c;

                        markers.Add(cm);
                    }
                }

                foreach (ucSCCIScatterChart chart in this.Grafici)
                {
                    chart.SetMarkers(markers);
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaMarkers", this.Name);
            }
        }

        private void mt_LastThreadException(object sender, Exception ex)
        {
            UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);

        }

        private void mt_DatatableCompleted(object sender, DataTable dt)
        {
            _DataTableMovimentiLaboratorioXAsync = dt;

            _cachedatada = DateTime.MinValue;
            _cachedataa = DateTime.MinValue;
            if (this.udtFiltroDataDa.Value != null) _cachedatada = getDataDa((DateTime)this.udtFiltroDataDa.Value);
            if (this.udtFiltroDataA.Value != null) _cachedataa = getDataA((DateTime)this.udtFiltroDataA.Value);

            _DataTableTipiLaboratorioXAsync = _defgraf.DataTableTipiLaboratorioXAsync(ref _DataTableMovimentiLaboratorioXAsync);

            CaricamentoTVLabXAsync();

            checkParams_AfterDtCompleted();

            SvuotaGrafici(true);
            CaricamentoGraficiXAsync(true);
            this.ShowUiLAB();

            UnicodeSrl.ScciCore.Common.MT.ScciWebSvcMT mt = (UnicodeSrl.ScciCore.Common.MT.ScciWebSvcMT)sender;
            mt.DatatableCompleted -= mt_DatatableCompleted;
            mt.LastThreadException -= mt_LastThreadException;

            try
            {
                _mtLAB.Dispose();
                _mtLAB = null;
            }
            catch
            {
            }
            mostraCaricamento(false);

            SetPulsantiProfili(true);

            _applicazioneProfiloDaCambioData = false;
        }

        private void checkParams_AfterDtCompleted()
        {
            if ((this.CustomParamaters != null) && (this.CustomParamaters is RisultatiLabUM))
            {
                SvcRicoveriDWH.RisultatiLabAll risultatoSel = (SvcRicoveriDWH.RisultatiLabAll)this.CustomParamaters;

                string keySez = risultatoSel.CodSezione;

                UltraTreeNode oNodeSez = null;

                bool bexist = this.utvFiltroTipoLAB.Nodes.Exists(keySez);

                if (bexist)
                {
                    oNodeSez = this.utvFiltroTipoLAB.Nodes[keySez];

                    string keyPrest = keySez + @"|" + risultatoSel.CodPrescrizione;
                    bexist = oNodeSez.Nodes.Exists(keyPrest);

                    if (bexist)
                        oNodeSez.Nodes[keyPrest].CheckedState = CheckState.Checked;
                }

            }
        }

        private void mostraCaricamento(bool mostra)
        {
            try
            {

                if (mostra)
                {
                    if (_labelCaricamento == null)
                    {
                        _labelCaricamento = new ucEasyLabel();
                        _labelCaricamento.Visible = false;


                        Infragistics.Win.Appearance app = new Infragistics.Win.Appearance();
                        app.TextHAlign = Infragistics.Win.HAlign.Center;
                        app.TextVAlign = Infragistics.Win.VAlign.Middle;
                        app.BackColor = Color.Transparent;
                        app.BorderColor = Color.DarkGray;

                        _labelCaricamento.Appearance = app;
                        _labelCaricamento.BorderStyleOuter = Infragistics.Win.UIElementBorderStyle.Solid;
                        _labelCaricamento.Location = new System.Drawing.Point(420, 8);
                        _labelCaricamento.Name = "_labelCaricamento";
                        _labelCaricamento.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
                        _labelCaricamento.ShortcutKey = System.Windows.Forms.Keys.None;
                        _labelCaricamento.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
                        _labelCaricamento.Size = new System.Drawing.Size(this.lblTipiPV.Width, 24);
                        _labelCaricamento.TabIndex = 10;
                        _labelCaricamento.Text = "caricamento...";
                        _labelCaricamento.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
                        _labelCaricamento.Enabled = false;


                    }

                    this.utvFiltroTipoLAB.Visible = false;
                    this.utxtFiltroTipoLAB.Visible = false;

                    this.tlpParametri.Controls.Add(_labelCaricamento, 0, 6);
                    this.tlpParametri.SetColumnSpan(_labelCaricamento, 1);
                    this.tlpParametri.SetRowSpan(_labelCaricamento, 3);

                    _labelCaricamento.Dock = System.Windows.Forms.DockStyle.Fill;
                    _labelCaricamento.Visible = true;

                    this.ubDeselectAll_LAB.Visible = false;
                }
                else
                {
                    if (_labelCaricamento != null)
                    {
                        _labelCaricamento.Visible = false;
                        _labelCaricamento.Dock = System.Windows.Forms.DockStyle.None;
                    }
                    this.utvFiltroTipoLAB.Visible = true;
                    this.utxtFiltroTipoLAB.Visible = true;
                    this.ubDeselectAll_LAB.Visible = true;
                }
            }
            catch
            {
            }
        }

        private void chiudiThread(bool dispose)
        {
            try
            {
                if (_mtLAB != null)
                {
                    _mtLAB.Cancel();
                    if (dispose)
                    {
                        _mtLAB.Dispose();
                        _mtLAB = null;
                    }
                }
            }
            catch
            {
            }
        }

        private void udt_ValueChanged()
        {
            if (!_stopDtValueChanged)
            {
                setLineeGiornaliereAbilitazione();

                CaricaDati(true, false);
                if (!_from_ucEasyDateRange_ValueChanged)
                {
                    bool skipcaricagrafici_orig = _skipcaricagrafici;

                    _skipcaricagrafici = true;
                    _runtime = true;
                    this.ucEasyDateRange.Value = null;
                    _runtime = false;
                    _skipcaricagrafici = skipcaricagrafici_orig;
                }
            }
        }

        private void checkCodPrescrizioneLab(string codPrescrizioneLab, bool setchecked)
        {
            if (!_skipcaricagrafici)
            {
                _skipcaricagrafici = true;
                try
                {
                    for (int iNS = 0; iNS < this.utvFiltroTipoLAB.Nodes.Count; iNS++)
                    {
                        for (int iP = 0; iP < this.utvFiltroTipoLAB.Nodes[iNS].Nodes.Count; iP++)
                        {
                            if (this.utvFiltroTipoLAB.Nodes[iNS].Nodes[iP].Key.Split('|')[1].Trim().ToUpper() == codPrescrizioneLab.Trim().ToUpper())
                            {
                                if (setchecked && this.utvFiltroTipoLAB.Nodes[iNS].Nodes[iP].CheckedState != CheckState.Checked) this.utvFiltroTipoLAB.Nodes[iNS].Nodes[iP].CheckedState = CheckState.Checked;
                                if (!setchecked && this.utvFiltroTipoLAB.Nodes[iNS].Nodes[iP].CheckedState == CheckState.Checked) this.utvFiltroTipoLAB.Nodes[iNS].Nodes[iP].CheckedState = CheckState.Unchecked;
                            }
                        }
                    }
                }
                catch
                {
                }
                _skipcaricagrafici = false;
            }
        }

        #endregion

        #endregion

        #region EVENTI

        private void frmPUGrafico_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void frmPUGrafico_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {

            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void ucEasyDateRange_ValueChanged(object sender, EventArgs e)
        {
            if (!_runtime)
            {
                try
                {
                    _from_ucEasyDateRange_ValueChanged = true;

                    bool originalskipcaricagrafici = _skipcaricagrafici;

                    _skipcaricagrafici = true;

                    DateTime? dataDa = (DateTime?)ucEasyDateRange.DataOraDa;
                    DateTime? dataA = (DateTime?)ucEasyDateRange.DataOraA;

                    if (dataDa.HasValue && dataA.HasValue)
                    {
                        this.udtFiltroDataDa.Value = dataDa.Value;
                        this.udtFiltroDataA.Value = dataA.Value;
                    }

                    _skipcaricagrafici = originalskipcaricagrafici;

                    if (!_skipcaricagrafici)
                    {
                        CaricaDati(true, false);
                    }
                }
                catch
                {
                }
                finally
                {
                    _from_ucEasyDateRange_ValueChanged = false;
                }

            }
        }

        private void uchkDati_Click(object sender, EventArgs e)
        {
            if (!this.uchkDati.Checked)
                this.uchkDati.Checked = true;
            else
            {
                this.uchkGrafico.Checked = !this.uchkDati.Checked;
                switchGriglioneGrafici();
            }
        }

        private void uchkGrafico_Click(object sender, EventArgs e)
        {
            if (!this.uchkGrafico.Checked)
                this.uchkGrafico.Checked = true;
            else
            {
                this.uchkDati.Checked = !this.uchkGrafico.Checked;
                switchGriglioneGrafici();
            }
        }

        private void uchkLineeGiornaliere_Click(object sender, EventArgs e)
        {
            if (this.uchkLineeGiornaliere.Enabled) _lastLineeGiornaliereChecked = this.uchkLineeGiornaliere.Checked;
            if (Grafici != null && Grafici.Count > 0)
            {
                foreach (ucSCCIScatterChart grafico in Grafici)
                {
                    try
                    {
                        grafico.LineeGiornaliereVisibili = getLineeGiornaliereVisibili();
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void utxtFiltroTipo_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                bool blab = false;
                bool breload = false;
                string search = this.utxtFiltroTipoPV.Text;
                UltraTree tv = this.utvFiltroTipoPV;
                if (sender == this.utxtFiltroTipoLAB)
                {
                    search = this.utxtFiltroTipoLAB.Text;
                    tv = this.utvFiltroTipoLAB;
                    blab = true;
                }

                if (blab)
                {
                    foreach (UltraTreeNode nodesez in tv.Nodes)
                    {
                        nodesez.Visible = false;
                        foreach (UltraTreeNode nodepres in nodesez.Nodes)
                        {
                            if (search == "")
                            {
                                if (!nodepres.Visible && nodepres.CheckedState == CheckState.Checked) breload = true;
                                nodepres.Visible = true;
                                nodesez.Visible = true;
                            }
                            else
                            {
                                if (!nodepres.Visible && nodepres.Text.ToUpper().IndexOf(search.ToUpper()) >= 0 && nodepres.CheckedState == CheckState.Checked) breload = true;
                                if (nodepres.Visible && nodepres.Text.ToUpper().IndexOf(search.ToUpper()) < 0 && nodepres.CheckedState == CheckState.Checked) breload = true;
                                nodepres.Visible = (nodepres.Text.ToUpper().IndexOf(search.ToUpper()) >= 0);
                                if (nodepres.Visible && !nodesez.Visible) nodesez.Visible = true;
                            }
                        }
                    }
                }
                else
                {
                    foreach (UltraTreeNode node in tv.Nodes)
                    {
                        if (search == "")
                        {
                            if (!node.Visible && node.CheckedState == CheckState.Checked) breload = true;
                            node.Visible = true;
                        }
                        else
                        {
                            if (!node.Visible && node.Text.ToUpper().IndexOf(search.ToUpper()) >= 0 && node.CheckedState == CheckState.Checked) breload = true;
                            if (node.Visible && node.Text.ToUpper().IndexOf(search.ToUpper()) < 0 && node.CheckedState == CheckState.Checked) breload = true;
                            node.Visible = (node.Text.ToUpper().IndexOf(search.ToUpper()) >= 0);
                        }
                    }
                }

                if (breload) CaricaDati(false, false);
            }
            catch (Exception)
            {
            }
        }

        private void udtFiltroDataA_ValueChanged(object sender, EventArgs e)
        {
            udt_ValueChanged();
        }

        private void udtFiltroDataA_AfterCloseUp(object sender, EventArgs e)
        {
            try
            {
                _stopDtValueChanged = false;
                if ((this.udtFiltroDataA.Value == null && _lastdtA == DateTime.MinValue)
                    || (getDataA((DateTime)this.udtFiltroDataA.Value) != _lastdtA))
                {
                    udt_ValueChanged();
                }
            }
            catch
            {
                _stopDtValueChanged = false;
            }
        }

        private void udtFiltroDataA_AfterDropDown(object sender, EventArgs e)
        {
            _stopDtValueChanged = true;
            if (this.udtFiltroDataA.Value != null)
                _lastdtA = getDataA((DateTime)this.udtFiltroDataA.Value);
            else
                _lastdtA = DateTime.MinValue;
        }

        private void udtFiltroDataDa_ValueChanged(object sender, EventArgs e)
        {
            udt_ValueChanged();
        }

        private void udtFiltroDataDa_AfterCloseUp(object sender, EventArgs e)
        {
            try
            {
                _stopDtValueChanged = false;
                if ((this.udtFiltroDataDa.Value == null && _lastdtDa == DateTime.MinValue)
                    || (getDataDa((DateTime)this.udtFiltroDataDa.Value) != _lastdtDa))
                {
                    udt_ValueChanged();
                }
            }
            catch
            {
                _stopDtValueChanged = false;
            }
        }

        private void udtFiltroDataDa_AfterDropDown(object sender, EventArgs e)
        {
            _stopDtValueChanged = true;
            if (this.udtFiltroDataDa.Value != null)
                _lastdtDa = getDataDa((DateTime)this.udtFiltroDataDa.Value);
            else
                _lastdtDa = DateTime.MinValue;
        }

        private void utvFiltroTipoPV_AfterCheck(object sender, NodeEventArgs e)
        {
            if (e.TreeNode.CheckedState == CheckState.Checked)
            {
                if (!_lstPVTSelezionati.Contains(e.TreeNode.Key)) _lstPVTSelezionati.Add(e.TreeNode.Key);
            }
            else
            {
                if (_lstPVTSelezionati.Contains(e.TreeNode.Key)) _lstPVTSelezionati.Remove(e.TreeNode.Key);
            }

            CaricaDati(false, false);
            this.CaricaDatiContesto();
        }

        private void utvFiltroTipoLAB_AfterCheck(object sender, NodeEventArgs e)
        {
            string codPrest = e.TreeNode.Key.Split('|')[1];
            if (e.TreeNode.CheckedState == CheckState.Checked)
            {
                if (!_lstCodPrestLabSelezionati.Contains(codPrest)) _lstCodPrestLabSelezionati.Add(codPrest);
            }
            else
            {
                if (_lstCodPrestLabSelezionati.Contains(codPrest)) _lstCodPrestLabSelezionati.Remove(codPrest);
            }

            checkCodPrescrizioneLab(e.TreeNode.Key.Split('|')[1], (e.TreeNode.CheckedState == CheckState.Checked));

            CaricaDati(false, true);
            this.CaricaDatiContesto();
        }

        private void utvFiltro_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Clicks == 1 && e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    UltraTreeNode child = ((UltraTree)sender).GetNodeFromPoint(e.Location);
                    if (child != null)
                    {

                        int minleftexp = 14; int minleftchk = 34; if (child.Parent != null && child.Parent is UltraTreeNode) minleftchk = 50;
                        if (child.NodeStyleResolved == NodeStyle.CheckBox)
                        {
                            if (e.Location.X > minleftchk)
                            {
                                if (child.CheckedState == CheckState.Checked)
                                    child.CheckedState = CheckState.Unchecked;
                                else
                                    child.CheckedState = CheckState.Checked;
                            }
                        }
                        else if (child.Nodes.Count > 0)
                        {
                            if (e.Location.X > minleftexp)
                                child.Expanded = !child.Expanded;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void ucEasyGriglione_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {
                int iStart = 6;
                int iCount = iStart;
                for (int i = e.Layout.Bands[0].Columns.Count - 1; i >= iStart; i--)
                {
                    e.Layout.Bands[0].Columns[i].Header.VisiblePosition = iCount;
                    iCount += 1;
                }

            }
            catch
            {
            }

            try
            {
                e.Layout.Override.ActiveRowAppearance.BackColor = Color.FromKnownColor(KnownColor.Window);
                e.Layout.Override.ActiveRowAppearance.ForeColor = Color.FromKnownColor(KnownColor.ControlText);

                e.Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.None;
                e.Layout.Bands[0].Override.HeaderClickAction = HeaderClickAction.Select;
                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {

                    switch (oCol.Key)
                    {
                        case "CodEntita":
                        case "CodTipo":
                        case "CodDimensione":
                            oCol.Hidden = true;
                            break;

                        case "Entita":
                            oCol.Hidden = false;
                            oCol.Header.Caption = "";

                            oCol.MergedCellStyle = MergedCellStyle.Always;
                            oCol.MergedCellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                            oCol.MergedCellContentArea = Infragistics.Win.UltraWinGrid.MergedCellContentArea.VisibleRect;

                            oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                            break;

                        case "DescTipo":
                            oCol.Hidden = false;
                            oCol.Header.Caption = "Tipo";

                            oCol.MergedCellStyle = MergedCellStyle.Always;
                            oCol.MergedCellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                            oCol.MergedCellContentArea = Infragistics.Win.UltraWinGrid.MergedCellContentArea.VisibleRect;

                            oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                            break;

                        case "Dimensione":
                            oCol.Hidden = false;
                            oCol.Header.Caption = "";
                            oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                            break;

                        default:
                            oCol.Format = "#,##0.00#";
                            oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                            oCol.Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            oCol.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                            break;
                    }
                }
            }
            catch (Exception)
            {
            }

        }

        private void frmPUGrafico_Shown(object sender, EventArgs e)
        {
            try
            {
                if (!_graphresized)
                {
                    RiposizionaGrafici();

                    _graphresized = true;
                }
            }
            catch
            {
            }


            try
            {
                foreach (UltraTreeNode node in this.utvFiltroTipoPRF.Nodes)
                {
                    foreach (UltraTreeNode child in node.Nodes)
                    {
                        child.Override.MaxLabelWidth = this.utvFiltroTipoPRF.Width - 56;
                        child.Override.MaxLabelHeight = -1;
                    }
                }
            }
            catch
            {
            }

        }

        private void ubDeselectAll_PV_Click(object sender, EventArgs e)
        {
            try
            {
                bool bReload = false;
                _skipcaricagrafici = true;

                foreach (UltraTreeNode nodePV in this.utvFiltroTipoPV.Nodes)
                {
                    try
                    {
                        if (nodePV.Override.NodeStyle == NodeStyle.CheckBox && nodePV.CheckedState == CheckState.Checked)
                        {
                            nodePV.CheckedState = CheckState.Unchecked;
                            bReload = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                    }
                }

                _skipcaricagrafici = false;
                if (bReload) CaricaDati(false, false);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }
            finally
            {
                _skipcaricagrafici = false;
            }
        }

        private void ubDeselectAll_LAB_Click(object sender, EventArgs e)
        {
            try
            {
                bool bReload = false;
                _skipcaricagrafici = true;

                foreach (UltraTreeNode nodeSez in this.utvFiltroTipoLAB.Nodes)
                {
                    try
                    {
                        if (nodeSez.Nodes.Count > 0)
                        {
                            foreach (UltraTreeNode nodeEsame in nodeSez.Nodes)
                            {
                                try
                                {
                                    if (nodeEsame.Override.NodeStyle == NodeStyle.CheckBox && nodeEsame.CheckedState == CheckState.Checked)
                                    {
                                        nodeEsame.CheckedState = CheckState.Unchecked;
                                        bReload = true;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                    }
                }

                _skipcaricagrafici = false;
                if (bReload) CaricaDati(false, false);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }
            finally
            {
                _skipcaricagrafici = false;
            }
        }

        private void frmPUGrafico_FormClosing(object sender, FormClosingEventArgs e)
        {
            chiudiThread(true);
            if (CoreStatics.CoreApplication.Ambiente.Contesto.ContainsKey(EnumEntita.XXX.ToString()))
            {
                CoreStatics.CoreApplication.Ambiente.Contesto.Remove(EnumEntita.XXX.ToString());
            }
        }

        #endregion

        #region PROFILI VISUALIZZAZIONE

        private void InitProfili()
        {
            try
            {

                CoreStatics.SetUltraTabControl(this.utcSelezioni);

                this.ubProfiliNew.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_NUOVO_32);
                this.ubProfiliNew.PercImageFill = 0.50F;
                this.ubProfiliNew.Appearance.ImageVAlign = Infragistics.Win.VAlign.Top;

                this.ubProfiliSalva.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_SALVA_32);
                this.ubProfiliSalva.PercImageFill = 0.50F;
                this.ubProfiliSalva.Appearance.ImageVAlign = Infragistics.Win.VAlign.Top;

                this.ubProfiliElimina.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_ELIMINA_32);
                this.ubProfiliElimina.PercImageFill = 0.50F;
                this.ubProfiliElimina.Appearance.ImageVAlign = Infragistics.Win.VAlign.Top;

                this.ubProfiliPulisci.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_FILTROAPPLICATO_32);
                this.ubProfiliPulisci.PercImageFill = 0.80F;
                this.ubProfiliPulisci.Appearance.ImageHAlign = Infragistics.Win.HAlign.Left;
                this.ubProfiliPulisci.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }
        }

        private void CaricaProfiliVisualizzazione(string vsCodiceDaSelezionare)
        {
            try
            {
                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodTipoSelezione", EnumTipoSelezione.GRAF.ToString()); op.Parametro.Add("DatiEstesi", "0"); op.Parametro.Add("SoloUtente", "0");
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                UnicodeSrl.Framework.Data.SqlParameterExt[] spcoll = new UnicodeSrl.Framework.Data.SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new UnicodeSrl.Framework.Data.SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable odt = Database.GetDataTableStoredProc("MSP_SelSelezioni", spcoll);


                this.ucEasyGridProfili.DisplayLayout.Bands[0].Columns.ClearUnbound();
                this.ucEasyGridProfili.DataSource = odt;
                this.ucEasyGridProfili.Refresh();

                if (this.ucEasyGridProfili.Rows.Count > 0)
                {
                    this.ucEasyGridProfili.Selected.Rows.Clear();
                    this.ucEasyGridProfili.ActiveRow = null;

                    if (vsCodiceDaSelezionare != null && vsCodiceDaSelezionare != "")
                        CoreStatics.SelezionaRigaInGriglia(ref ucEasyGridProfili, "Codice", vsCodiceDaSelezionare);
                }
                SetPulsantiProfili(true);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }
        }

        private void SetPulsantiProfili(bool bAbilita)
        {
            try
            {
                if (!bAbilita)
                {
                    this.ucEasyGridProfili.Enabled = false;
                    this.ubProfiliNew.Enabled = false;
                    this.ubProfiliElimina.Enabled = false;
                    this.ubProfiliSalva.Enabled = false;
                    this.ubProfiliPulisci.Enabled = false;
                }
                else
                {
                    this.ucEasyGridProfili.Enabled = true;
                    this.ubProfiliNew.Enabled = true;
                    this.ubProfiliElimina.Enabled = false;
                    this.ubProfiliSalva.Enabled = false;
                    this.ubProfiliPulisci.Enabled = false;
                    if (this.ProfiloVisualizzazioneCorrente != null)
                    {
                        this.ubProfiliElimina.Enabled = (this.ProfiloVisualizzazioneCorrente.Azione != EnumAzioni.INS && this.ProfiloVisualizzazioneCorrente.PERMESSOMODIFICA);
                        this.ubProfiliSalva.Enabled = (this.ProfiloVisualizzazioneCorrente.Azione != EnumAzioni.INS && this.ProfiloVisualizzazioneCorrente.PERMESSOMODIFICA);
                        this.ubProfiliPulisci.Enabled = (this.ProfiloVisualizzazioneCorrente.Azione != EnumAzioni.INS);
                    }
                    else
                    {
                        this.ubProfiliElimina.Enabled = false;
                        this.ubProfiliSalva.Enabled = true;
                        this.ubProfiliPulisci.Enabled = false;
                    }
                }

            }
            catch
            {
            }
        }

        private void AggiornaProfiloVisualizzazioneCorrente()
        {
            try
            {

                checkLoadProfiloVisualizzazioneCorrente();

                SelezioniGrafici sel = (SelezioniGrafici)this.ProfiloVisualizzazioneCorrente.Selezioni;
                if (this.ucEasyDateRange.Value == null)
                    sel.RangeDate = "";
                else
                    sel.RangeDate = this.ucEasyDateRange.Value.ToString();
                if (this.udtFiltroDataDa.Value == null)
                    sel.DataDa = DateTime.MinValue;
                else
                    sel.DataDa = (DateTime)this.udtFiltroDataDa.Value;
                if (this.udtFiltroDataA.Value == null)
                    sel.DataA = DateTime.MinValue;
                else
                    sel.DataA = (DateTime)this.udtFiltroDataA.Value;

                sel.CodiciTipoPVT = new List<string>();
                foreach (UltraTreeNode onodepv in this.utvFiltroTipoPV.Nodes)
                {
                    if (onodepv.Visible && onodepv.CheckedState == CheckState.Checked)
                    {
                        sel.CodiciTipoPVT.Add(onodepv.Key);
                    }
                }

                sel.CodiciLAB = new List<string>();
                foreach (UltraTreeNode onodeSezione in this.utvFiltroTipoLAB.Nodes)
                {
                    foreach (UltraTreeNode onodePrest in onodeSezione.Nodes)
                    {
                        if (onodePrest.Visible && onodePrest.CheckedState == CheckState.Checked)
                        {
                            string codPrest = onodePrest.Key.Split('|')[1];
                            if (!sel.CodiciLAB.Contains(codPrest))
                            {
                                sel.CodiciLAB.Add(codPrest);
                            }

                        }
                    }
                }


                this.ProfiloVisualizzazioneCorrente.Selezioni = sel;

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }
        }

        private void ApplicaProfiloVisualizzazione()
        {
            try
            {














                if (this.ProfiloVisualizzazioneCorrente != null)
                {

                    _skipcaricagrafici = true;
                    _runtime = true;

                    _lstPVTSelezionati = new List<string>();
                    _lstCodPrestLabSelezionati = new List<string>();

                    this.utxtFiltroTipoPV.Text = "";
                    foreach (UltraTreeNode onodepv in this.utvFiltroTipoPV.Nodes)
                    {
                        if (onodepv.Visible)
                        {
                            if (((SelezioniGrafici)this.ProfiloVisualizzazioneCorrente.Selezioni).CodiciTipoPVT != null
                                && ((SelezioniGrafici)this.ProfiloVisualizzazioneCorrente.Selezioni).CodiciTipoPVT.Contains(onodepv.Key))
                                onodepv.CheckedState = CheckState.Checked;
                            else
                                onodepv.CheckedState = CheckState.Unchecked;
                        }

                    }

                    this.utxtFiltroTipoLAB.Text = "";
                    foreach (UltraTreeNode onodeSezione in this.utvFiltroTipoLAB.Nodes)
                    {
                        foreach (UltraTreeNode onodePrest in onodeSezione.Nodes)
                        {
                            if (onodePrest.Visible)
                            {
                                string codPrest = onodePrest.Key.Split('|')[1];

                                if (((SelezioniGrafici)this.ProfiloVisualizzazioneCorrente.Selezioni).CodiciLAB != null
                                    && ((SelezioniGrafici)this.ProfiloVisualizzazioneCorrente.Selezioni).CodiciLAB.Contains(codPrest))
                                {
                                    onodePrest.CheckedState = CheckState.Checked;
                                    if (!onodeSezione.Expanded) onodeSezione.ExpandAll();
                                }
                                else
                                    onodePrest.CheckedState = CheckState.Unchecked;
                            }
                        }
                    }

                    _skipcaricagrafici = false;
                    _runtime = false;

                    CaricaDati(false, false);
                    this.CaricaDatiContesto();

                }
            }
            catch (Exception ex)
            {
                _applicazioneProfiloDaCambioData = false;
                _skipcaricagrafici = false;
                _runtime = false;
                CoreStatics.ExGest(ref ex, "ApplicaProfiloVisualizzazione", this.Name);
            }
        }

        private void AnnullaVisualizzazioneProfilo()
        {
            try
            {
                deselezionaTutto();

                this.ProfiloVisualizzazioneCorrente = null;

                if (this.ucEasyGridProfili.Rows.Count > 0)
                {
                    if (this.ucEasyGridProfili.ActiveRow != null)
                    {
                        this.ucEasyGridProfili.Selected.Rows.Clear();
                        this.ucEasyGridProfili.ActiveRow = null;
                    }
                }
                SetPulsantiProfili(true);

                CaricaDati(false, false);
                this.CaricaDatiContesto();
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "AnnullaVisualizzazioneProfilo", this.Name);
            }
        }

        private bool CheckInputSalvaProfili()
        {
            try
            {
                bool bOK = true;
                string sMsg = "";

                if (bOK && this.ProfiloVisualizzazioneCorrente != null)
                {
                    if (((SelezioniGrafici)this.ProfiloVisualizzazioneCorrente.Selezioni).CodiciLAB.Count <= 0
                        && ((SelezioniGrafici)this.ProfiloVisualizzazioneCorrente.Selezioni).CodiciTipoPVT.Count <= 0)
                    {
                        sMsg = @"Non sono stati selezionati Esami e\o Parametri da visualizzare!" + Environment.NewLine;
                        sMsg += @"Vuoi salvare ugualmente il Profilo di Visualizzazione?";

                        if (easyStatics.EasyMessageBox(sMsg, "Salva Profilo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                            bOK = false;
                    }
                }

                return bOK;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string getDescrizioneNuovoProfilo()
        {
            string sNewDescription = "";
            int iAggiunti = 0;
            try
            {
                for (int n = 0; n < this.utvFiltroTipoPV.Nodes.Count; n++)
                {
                    UltraTreeNode onodepv = this.utvFiltroTipoPV.Nodes[n];
                    if (onodepv.Visible && onodepv.CheckedState == CheckState.Checked)
                    {
                        if (iAggiunti >= 1) sNewDescription += @", ";
                        if (iAggiunti < 2)
                            sNewDescription += onodepv.Text.Trim();
                        else
                            sNewDescription += @"...";

                        iAggiunti += 1;

                        if (iAggiunti >= 3) n = this.utvFiltroTipoPV.Nodes.Count + 1;
                    }
                }
                if (iAggiunti < 3)
                {
                    List<string> arrAggiunti = new List<string>();
                    for (int s = 0; s < this.utvFiltroTipoLAB.Nodes.Count; s++)
                    {
                        UltraTreeNode onodeSezione = this.utvFiltroTipoLAB.Nodes[s];
                        for (int n = 0; n < onodeSezione.Nodes.Count; n++)
                        {
                            UltraTreeNode onodePrest = onodeSezione.Nodes[n];
                            if (onodePrest.Visible && onodePrest.CheckedState == CheckState.Checked)
                            {
                                string codPrest = onodePrest.Key.Split('|')[1];
                                if (!arrAggiunti.Contains(codPrest.ToUpper()))
                                {
                                    arrAggiunti.Add(codPrest.ToUpper());

                                    if (iAggiunti >= 1) sNewDescription += @", ";
                                    if (iAggiunti < 2)
                                        sNewDescription += onodePrest.Text.Trim();
                                    else
                                        sNewDescription += @"...";

                                    iAggiunti += 1;

                                    if (iAggiunti >= 3)
                                    {
                                        n = onodeSezione.Nodes.Count + 1;
                                        s = this.utvFiltroTipoLAB.Nodes.Count + 1;
                                    }
                                }

                            }
                        }
                    }
                }



            }
            catch
            {
                sNewDescription = @"Nuovo Profilo [" + CoreStatics.CoreApplication.Sessione.Utente.Descrizione + @" " + DateTime.Now.ToString(@"dd/MM/yyyy HH:mm") + @"]";
            }

            return sNewDescription;
        }

        private void deselezionaTutto()
        {
            try
            {
                _skipcaricagrafici = true;
                _runtime = true;

                this.utxtFiltroTipoPV.Text = "";
                foreach (UltraTreeNode onodepv in this.utvFiltroTipoPV.Nodes)
                {
                    if (onodepv.Visible)
                    {
                        onodepv.CheckedState = CheckState.Unchecked;
                    }

                }

                this.utxtFiltroTipoLAB.Text = "";
                foreach (UltraTreeNode onodeSezione in this.utvFiltroTipoLAB.Nodes)
                {
                    foreach (UltraTreeNode onodePrest in onodeSezione.Nodes)
                    {
                        if (onodePrest.Visible)
                        {
                            onodePrest.CheckedState = CheckState.Unchecked;
                        }
                    }
                }

                _skipcaricagrafici = false;
                _runtime = false;

            }
            catch (Exception ex)
            {

                _skipcaricagrafici = false;
                _runtime = false;
                CoreStatics.ExGest(ref ex, "deselezionaTutto", this.Name);
            }
        }

        #region EVENTI

        private void ucEasyGridProfili_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {

                e.Layout.Override.RowSizing = Infragistics.Win.UltraWinGrid.RowSizing.AutoFixed;

                foreach (UltraGridColumn col in e.Layout.Bands[0].Columns)
                {
                    switch (col.Key)
                    {
                        case "Descrizione":
                            col.Header.Caption = "Profili";
                            col.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            col.Hidden = false;

                            break;
                        default:
                            col.Hidden = true;
                            break;
                    }
                }



            }
            catch
            {
            }
        }

        private void ucEasyGridProfili_ClickCell(object sender, ClickCellEventArgs e)
        {
            try
            {
                this.ProfiloVisualizzazioneCorrente = new Selezione(e.Cell.Row.Cells["Codice"].Text);
                SetPulsantiProfili(true);

                ApplicaProfiloVisualizzazione();
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, @"ucEasyGridProfili_ClickCell", this.Name);
            }
        }

        private void ubProfiliNew_Click(object sender, EventArgs e)
        {
            try
            {
                bool bOK = true;
                string sNewDescription = "";

                AggiornaProfiloVisualizzazioneCorrente();

                bOK = CheckInputSalvaProfili();

                if (bOK)
                {
                    sNewDescription = getDescrizioneNuovoProfilo();

                    if (easyStatics.EasyInputBox(@"Inserisci una descrizione per il nuovo profilo:", @"Nuovo Profilo", sNewDescription, 255, out sNewDescription) == System.Windows.Forms.DialogResult.OK)
                    {
                        if (sNewDescription.Trim() == "")
                        {
                            bOK = false;
                            easyStatics.EasyMessageBox(@"Descrizione Obbligatoria!", "Nuovo Profilo", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                        else
                            bOK = true;
                    }
                    else
                        bOK = false;
                }

                if (bOK)
                {

                    checkLoadProfiloVisualizzazioneCorrente();

                    if (this.ProfiloVisualizzazioneCorrente.Azione == EnumAzioni.INS)
                    {
                        this.ProfiloVisualizzazioneCorrente.Descrizione = sNewDescription;
                        bOK = this.ProfiloVisualizzazioneCorrente.Salva();
                    }
                    else
                    {
                        Selezione newsel = new Selezione(EnumTipoSelezione.GRAF, CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                        newsel.Descrizione = sNewDescription;
                        newsel.FlagSistema = false;
                        newsel.SelezioniXML = this.ProfiloVisualizzazioneCorrente.SelezioniXML;

                        bOK = newsel.Salva();
                        this.ProfiloVisualizzazioneCorrente = newsel;

                    }
                }

                if (bOK)
                {
                    CaricaProfiliVisualizzazione(this.ProfiloVisualizzazioneCorrente.Codice);
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubProfiliNew_Click", this.Name);
            }
        }

        private void ubProfiliSalva_Click(object sender, EventArgs e)
        {
            try
            {

                checkLoadProfiloVisualizzazioneCorrente();

                if (this.ProfiloVisualizzazioneCorrente.PERMESSOMODIFICA)
                {
                    AggiornaProfiloVisualizzazioneCorrente();

                    if (CheckInputSalvaProfili())
                    {

                        bool bSalva = false;
                        string sNewDescription = this.ProfiloVisualizzazioneCorrente.Descrizione; if (sNewDescription.Trim() == "")
                        {
                            sNewDescription = getDescrizioneNuovoProfilo();
                        }

                        if (easyStatics.EasyInputBox(@"Puoi modificare la descrizione del profilo:", @"Salva Profilo", sNewDescription, 255, out sNewDescription) == System.Windows.Forms.DialogResult.OK)
                        {
                            if (sNewDescription.Trim() == "")
                            {
                                bSalva = false;
                                easyStatics.EasyMessageBox(@"Descrizione Obbligatoria!", "Salva Profilo", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            }
                            else
                            {
                                bSalva = true;
                                this.ProfiloVisualizzazioneCorrente.Descrizione = sNewDescription;
                            }
                        }
                        else
                            bSalva = false;

                        if (bSalva)
                        {
                            if (this.ProfiloVisualizzazioneCorrente.Salva())
                            {
                                CaricaProfiliVisualizzazione(this.ProfiloVisualizzazioneCorrente.Codice);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubProfiliSalva_Click", this.Name);
            }
        }

        private void ubProfiliElimina_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.ProfiloVisualizzazioneCorrente != null && this.ProfiloVisualizzazioneCorrente.PERMESSOMODIFICA)
                {
                    if (easyStatics.EasyMessageBox(@"Sei sicuro di voler eliminare il profilo" + Environment.NewLine + @"""" + this.ProfiloVisualizzazioneCorrente.Descrizione + @"""?", "Elimina Profilo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                    {
                        this.ProfiloVisualizzazioneCorrente.Azione = EnumAzioni.CAN;
                        this.ProfiloVisualizzazioneCorrente.Salva();
                        this.ProfiloVisualizzazioneCorrente = null;

                        CaricaProfiliVisualizzazione("");
                    }
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubProfiliElimina_Click", this.Name);
            }
        }

        private void ubProfiliPulisci_Click(object sender, EventArgs e)
        {
            AnnullaVisualizzazioneProfilo();
        }

        private void utcSelezioni_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
        {

            try
            {
                if (e.Tab.Key == "tabProfili")
                {
                    if (!_profilishown)
                    {

                        if (this.ucEasyGridProfili.Rows.Count > 0)
                        {
                            if (this.ucEasyGridProfili.ActiveRow != null)
                            {
                                this.ucEasyGridProfili.Selected.Rows.Clear();
                                this.ucEasyGridProfili.ActiveRow = null;
                            }
                        }
                        _profilishown = true;
                    }
                }
            }
            catch
            {
            }
        }

        #endregion

        #endregion

        private void utxtFiltroTipoPRF_ValueChanged(object sender, EventArgs e)
        {
            bool reload = false;
            string filter = this.utxtFiltroTipoPRF.Text.ToUpper();

            string nodesFilterPre = "";
            string nodesFilterPost = "";

            foreach (UltraTreeNode nodeParent in this.utvFiltroTipoPRF.Nodes)
            {
                foreach (UltraTreeNode nodeChild in nodeParent.Nodes)
                {
                    if (nodeChild.Visible && (nodeChild.CheckedState == CheckState.Checked)) nodesFilterPre += nodeChild.Text;
                }

            }

            foreach (UltraTreeNode nodeParent in this.utvFiltroTipoPRF.Nodes)
            {
                nodeParent.Visible = false;
                foreach (UltraTreeNode nodeChild in nodeParent.Nodes)
                {
                    if (filter == "")
                    {
                        nodeChild.Visible = true;
                        nodeParent.Visible = true;
                    }
                    else
                    {
                        nodeChild.Visible = (nodeChild.Text.ToUpper().IndexOf(filter) >= 0);
                        if (nodeChild.Visible && !nodeParent.Visible) nodeParent.Visible = true;
                    }
                }
            }

            foreach (UltraTreeNode nodeParent in this.utvFiltroTipoPRF.Nodes)
            {
                foreach (UltraTreeNode nodeChild in nodeParent.Nodes)
                {
                    if (nodeChild.Visible && (nodeChild.CheckedState == CheckState.Checked)) nodesFilterPost += nodeChild.Text;
                }

            }
            reload = Convert.ToBoolean(nodesFilterPre != nodesFilterPost);
            if (reload)
            {
                SvuotaGrafici();
                CaricamentoGraficiXAsync(false);
            }

        }

        private void utvFiltroTipoPRF_AfterCheck(object sender, NodeEventArgs e)
        {
            List<String> listTipiSomm = new List<string>();

            try
            {
                foreach (UltraTreeNode node in this.utvFiltroTipoPRF.Nodes)
                {
                    foreach (UltraTreeNode child in node.Nodes)
                    {
                        if (child.Visible && (child.CheckedState == CheckState.Checked))
                        {
                            listTipiSomm.Add(child.Key);
                        }
                    }
                }

                if (listTipiSomm.Count == 0)
                    this.DataMarkers = null;
                else
                    this.DataMarkers = _defgraf.DataTableSelMovSomministrazioniGrafico(this.DataDa, this.DataA, listTipiSomm);

                CaricaMarkers();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "utvFiltroTipoPRF_AfterCheck", this.Name);
            }
        }

        private void ubDeselectAll_PRF_Click(object sender, EventArgs e)
        {
            try
            {
                bool bReload = false;

                this.InitializingUI = true;

                foreach (UltraTreeNode node in this.utvFiltroTipoPRF.Nodes)
                {
                    try
                    {
                        if (node.Override.NodeStyle == NodeStyle.CheckBox && node.CheckedState == CheckState.Checked)
                        {
                            node.CheckedState = CheckState.Unchecked;
                            bReload = true;
                        }

                        foreach (UltraTreeNode child in node.Nodes)
                        {
                            if (child.Override.NodeStyle == NodeStyle.CheckBox && child.CheckedState == CheckState.Checked)
                            {
                                child.CheckedState = CheckState.Unchecked;
                                bReload = true;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                    }
                }

                if (bReload) CaricaDati(false, false);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }
            finally
            {
                this.InitializingUI = false;
            }
        }

    }
}
