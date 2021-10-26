using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using System.Xml;
using System.IO;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.DatiClinici.DC;
using UnicodeSrl.DatiClinici.Common;
using UnicodeSrl.DatiClinici.Common.Enums;
using UnicodeSrl.Evaluator;

using UnicodeSrl.RTFLibrary.Core;
using UnicodeSrl.RTFLibrary.Util;

using System.Drawing.Imaging;
using System.Drawing;

using System.ComponentModel;
using System.Threading;

using UnicodeSrl.Scci.RTF;
using UnicodeSrl.Scci.Debugger;
using System.Drawing.Drawing2D;

namespace UnicodeSrl.Scci
{

    [Serializable()]
    public class MovScheda
    {

        private string _idmovscheda = string.Empty;
        private string _codua = string.Empty;
        private string _codentita = string.Empty;
        private string _identita = string.Empty;
        private string _idpaziente = string.Empty;
        private string _idepisodio = string.Empty;
        private string _idtrasferimento = string.Empty;
        private string _codscheda = string.Empty;
        private string _codstatoscheda = string.Empty;
        private string _codstatoschedacalcolato = string.Empty;
        private int _versione = 0;
        private int _numero = 0;
        private string _datixml = string.Empty;
        private string _anteprimartf = string.Empty;
        private string _anteprimatxt = string.Empty;
        private string _datiobbligatorimancantirtf = string.Empty;
        private string _datirilievortf = string.Empty;
        private string _idschedapadre = string.Empty;
        private bool _storicizzata = false;
        private DateTime _datacreazione = DateTime.MinValue;
        private DateTime _dataultimamodifica = DateTime.MinValue;
        private string _codutenterilevazione = string.Empty;
        private string _codutenteultimamodifica = string.Empty;
        private string _descrscheda = string.Empty;
        private string _descrunitaatomica = string.Empty;
        private string _descrutenteultimamodifica = string.Empty;
        private EnumAzioni _azione = EnumAzioni.MOD;
        private bool _inevidenza = false;
        private bool _validabile = false;
        private bool _validata = false;
        private bool _validatanew = false;
        private bool _cartellaambulatorialecodificata = false;
        private string _idcartellaambulatoriale = string.Empty;

        private int _permessoUAFirma = 0;

        private List<MovSchedaAllegato> _movschedaallegati = null;
        private List<List<string>> _movoperazioni = new List<List<string>>();

        private string _idpin = string.Empty;
        private bool _addpin = false;

        private Scheda _scheda = null;
        private DataSet _datasetValori = null;

        private EnumTipoRichiestaAllegatoScheda _tiporichiesta = EnumTipoRichiestaAllegatoScheda.LISTA;

        public MovScheda()
        {
            this.FlagCreaRtfOnSet = true;
        }

        public MovScheda(string idmovscheda, DataContracts.ScciAmbiente ambiente)
            : this()
        {
            this.Ambiente = ambiente;
            this.Carica(idmovscheda);
        }
        public MovScheda(string codentita, string identita, DataContracts.ScciAmbiente ambiente)
            : this()
        {
            this.Ambiente = ambiente;
            this.Carica(codentita, identita);
        }
        public MovScheda(string codscheda, EnumEntita entita, string codua, string idpaziente, string idepisodio, string idtrasferimento, DataContracts.ScciAmbiente ambiente)
            : this()
        {
            this.Ambiente = ambiente;
            _codscheda = codscheda;
            _versione = 0;
            _codentita = entita.ToString();
            _identita = string.Empty;
            _codua = codua;
            _idpaziente = idpaziente;
            _idepisodio = idepisodio;
            _idtrasferimento = idtrasferimento;
            _azione = EnumAzioni.INS;
            this.PermessoUAFirma = getPermessoUAFirma();
        }

        public MovScheda(string codscheda, EnumEntita entita, string codua, string idpaziente, string idepisodio, string idtrasferimento, int versione, DataContracts.ScciAmbiente ambiente)
            : this()
        {
            this.Ambiente = ambiente;
            _codscheda = codscheda;
            _versione = versione;
            _codentita = entita.ToString();
            _identita = string.Empty;
            _codua = codua;
            _idpaziente = idpaziente;
            _idepisodio = idepisodio;
            _idtrasferimento = idtrasferimento;
            _azione = EnumAzioni.INS;
            this.PermessoUAFirma = getPermessoUAFirma();
        }

        public bool FlagCreaRtfOnSet { get; set; }

        public ScciAmbiente Ambiente { get; set; }

        public string IDMovScheda
        {
            get { return _idmovscheda; }
            set { _idmovscheda = value; }
        }

        public string CodUA
        {
            get { return _codua; }
        }

        public string CodEntita
        {
            get { return _codentita; }
        }

        public string IDEntita
        {
            get { return _identita; }
            set { _identita = value; }
        }

        public string IDPaziente
        {
            get { return _idpaziente; }
        }

        public string IDEpisodio
        {
            get { return _idepisodio; }
            set { _idepisodio = value; }
        }

        public string IDTrasferimento
        {
            get { return _idtrasferimento; }
            set { _idtrasferimento = value; }
        }

        public EnumAzioni Azione
        {
            get { return _azione; }
            set { _azione = value; }
        }

        public string CodStatoScheda
        {
            get { return _codstatoscheda; }
            set { _codstatoscheda = value; }
        }

        public string CodStatoSchedaCalcolato
        {
            get { return _codstatoschedacalcolato; }
            set { _codstatoschedacalcolato = value; }
        }

        public string CodScheda
        {
            get { return _codscheda; }
            set { _codscheda = value; }
        }

        public int Versione
        {
            get { return _versione; }
        }

        public int Numero
        {
            get { return _numero; }
        }

        public string DatiXML
        {
            get { return _datixml; }
            set
            {
                if (_datixml != value)
                {
                    _datixml = value;
                    if (this.FlagCreaRtfOnSet)
                        GeneraRTF();
                }
            }
        }

        public string AnteprimaRTF
        {
            get { return _anteprimartf; }
            set { _anteprimartf = value; }
        }
        public string AnteprimaRTFxParamStored
        {
            get { return UnicodeSrl.Scci.Statics.CommonStatics.EncodeBase64StoredParameter(this.AnteprimaRTF); }
        }

        public string AnteprimaTXT
        {
            get { return _anteprimatxt; }
            set { _anteprimatxt = value; }
        }
        public string AnteprimaTXTxParamStored
        {
            get { return UnicodeSrl.Scci.Statics.CommonStatics.EncodeBase64StoredParameter(this.AnteprimaTXT); }
        }

        public string DatiObbligatoriMancantiRTF
        {
            get { return _datiobbligatorimancantirtf; }
            set { _datiobbligatorimancantirtf = value; }
        }
        public string DatiObbligatoriMancantiRTFxParamStored
        {
            get { return UnicodeSrl.Scci.Statics.CommonStatics.EncodeBase64StoredParameter(this.DatiObbligatoriMancantiRTF); }
        }

        public string DatiRilievoRTF
        {
            get { return _datirilievortf; }
            set { _datirilievortf = value; }
        }
        public string DatiRilievoRTFxParamStored
        {
            get { return UnicodeSrl.Scci.Statics.CommonStatics.EncodeBase64StoredParameter(this.DatiRilievoRTF); }
        }

        public string IDSchedaPadre
        {
            get { return _idschedapadre; }
            set { _idschedapadre = value; }
        }

        public bool Storicizzata
        {
            get { return _storicizzata; }
        }

        public DateTime DataCreazione
        {
            get { return _datacreazione; }
        }

        public DateTime DataUltimaModifica
        {
            get { return _dataultimamodifica; }
        }

        public string CodUtenteRilevazione
        {
            get { return _codutenterilevazione; }
        }

        public string CodUtenteUltimaModifica
        {
            get { return _codutenteultimamodifica; }
        }

        public string DescrScheda
        {
            get { return _descrscheda; }
            set { _descrscheda = value; }
        }

        public string DescrUnitaAtomica
        {
            get { return _descrunitaatomica; }
        }

        public string DescUtenteUltimaModifica
        {
            get { return _descrutenteultimamodifica; }
        }

        public string IDPin
        {
            get { return _idpin; }
            set { _idpin = value; }
        }

        public bool AddPin
        {
            get { return _addpin; }
            set { _addpin = value; }
        }

        public Scheda Scheda
        {
            get
            {
                if (_scheda == null)
                {
                    if (_codscheda != string.Empty && _codscheda.Trim() != "")
                    {
                        if (_versione <= 0)
                        {
                            _scheda = new Scheda(_codscheda, DateTime.Now, this.Ambiente);
                            _versione = _scheda.Versione;
                            _descrscheda = _scheda.Descrizione;
                        }
                        else
                        {
                            _scheda = new Scheda(_codscheda, _versione, this.DataCreazione, this.Ambiente);
                        }
                    }
                }
                return _scheda;
            }
        }

        public bool InEvidenza
        {
            get { return _inevidenza; }
            set { _inevidenza = value; }
        }

        public bool Validabile
        {
            get { return _validabile; }
        }

        public bool Validata
        {
            get { return _validata; }
            set { _validata = value; }
        }

        public bool ValidataNew
        {
            get { return _validatanew; }
            set { _validatanew = value; }
        }

        public int PermessoUAFirma
        {
            get { return _permessoUAFirma; }
            set { _permessoUAFirma = value; }
        }

        public bool CartellaAmbulatorialeCodificata
        {
            get { return _cartellaambulatorialecodificata; }
            set { _cartellaambulatorialecodificata = value; }
        }

        public string IDCartellaAmbulatoriale
        {
            get { return _idcartellaambulatoriale; }
            set { _idcartellaambulatoriale = value; }
        }


        public string PathFileTemp { get; set; }

        public long LenDatiObbligatoriMancantiRTF { get; set; }

        public EnumTipoRichiestaAllegatoScheda TipoRichiesta
        {
            get { return _tiporichiesta; }
            set
            {
                _movschedaallegati = null;
                _tiporichiesta = value;
            }
        }

        public List<MovSchedaAllegato> Allegati
        {
            get
            {
                if (_movschedaallegati == null)
                {
                    CaricaAllegati(_tiporichiesta);
                }
                return _movschedaallegati;
            }
            set
            {
                _movschedaallegati = value;
            }
        }

        public List<List<string>> MovOperazioni
        {
            get
            {
                return _movoperazioni;
            }
            set
            {
                _movoperazioni = value;
            }
        }

        public void AggiungiSegnalibro()
        {

            try
            {

                Parametri op = new Parametri(this.Ambiente);

                if (_idpaziente != string.Empty) op.Parametro.Add("IDPaziente", _idpaziente);
                if (_idtrasferimento != string.Empty) op.Parametro.Add("IDTrasferimento", _idtrasferimento);

                op.Parametro.Add("CodEntita", EnumEntita.SCH.ToString());
                op.Parametro.Add("IDEntita", _identita);

                op.Parametro.Add("CodEntitaScheda", _codentita);
                op.Parametro.Add("CodScheda", _codscheda);
                op.Parametro.Add("Numero", _numero.ToString());

                op.TimeStamp.CodAzione = EnumAzioni.INS.ToString();
                op.TimeStamp.CodEntita = EnumEntita.SGL.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                Database.ExecStoredProc("MSP_InsMovSegnalibri", spcoll);

            }
            catch (Exception ex)
            {
                throw new Exception(@"MovScheda.AggiungiSegnalibro()" + Environment.NewLine + ex.Message, ex);
            }

        }

        public void GeneraRTF()
        {

            Gestore oGestore = CommonStatics.GetGestore(this.Ambiente);
            oGestore.SchedaLayoutsXML = this.Scheda.LayoutXML;
            oGestore.Decodifiche = this.Scheda.DizionarioValori();
            oGestore.SchedaXML = this.Scheda.StrutturaXML;
            oGestore.SchedaDatiXML = this.DatiXML;

            if (oGestore.Scheda.Formula != null && oGestore.Scheda.Formula.ToString() != string.Empty)
            {
                _codstatoschedacalcolato = oGestore.Valutatore.Process(oGestore.Scheda.Formula.ToString(), oGestore.Contesto);
            }

            this.GeneraRTF(oGestore);

        }

        private void GeneraRTF(Gestore oGestore)
        {

            RtfFiles rtf = new RtfFiles();

            string rtfAnteprima = "";
            string rtfDatiObbligatoriMancanti = "";
            string rtfDatiRilievo = "";

            DcAttributo DcAttributoRipetibile = new DcAttributo();
            DcAttributo DcAttributoTipoSezione = new DcAttributo();

            Dictionary<string, dynamic> SezioneResult = new Dictionary<string, dynamic>();
            string sGrigliaBordi = string.Empty;

            bool bAttributoRipetibile = false;
            enumTipoSezione enAttributoTipoSezione = enumTipoSezione.Standard;

            bool bPrimaSezione = true;
            bool bSingolaSezione = false;

            bool bIntestazioneRipetuta = false;
            bool bHoScrittoValore = false;
            bool bHoScrittoACapo = false;

            DcAttributo DcAttributoACapo = new DcAttributo();
            DcAttributo DcAttributoStile = new DcAttributo();
            bool bAttributoACapo = false;
            string sAttributoStile = string.Empty;

            Dictionary<string, ParseResult> ParseResult = new Dictionary<string, ParseResult>();

            Dictionary<string, Font> FontResult = new Dictionary<string, Font>();
            Dictionary<string, Color> ColorResult = new Dictionary<string, Color>();
            Dictionary<string, List<string>> OpzioniResult = new Dictionary<string, List<string>>();
            Dictionary<string, Size> DimensioniResult = new Dictionary<string, Size>();
            Dictionary<string, int> RigheResult = new Dictionary<string, int>();
            bool bAttributoBoldEtichetta = true;
            Dictionary<string, Color> BackColorResult = new Dictionary<string, Color>();
            bool bAttributoVisualizzaEtichetta = false;
            const string TAG_GRIDROW_INIZIO = @"\trowd";
            const string TAG_GRIDROW_FINE = @"\row\fs1\pard\par";
            const string TAG_GRIDCEL_BORDER = @"\clbrdr{0}\brdrw{1}\brdrs";
            const string TAG_GRIDCEL_BACKCOLOR = @"\clcbpat";
            const string TAG_GRIDCOL_INIZIO = @"\cellx";
            const string TAG_GRIDVAL_INIZIO = @"\intbl";
            const string TAG_GRIDVAL_FINE = @"\cell";

            string sTagColonne = string.Empty;
            string sTagValori = string.Empty;
            int nWithGridCol = 0;

            string sTagColonneTipoGriglia = string.Empty;
            string sTagValoriTipoGriglia = string.Empty;

            Dictionary<string, Color> ColorResultGriglia = new Dictionary<string, Color>();

            try
            {

                this.AnteprimaRTF = string.Empty;
                this.DatiObbligatoriMancantiRTF = string.Empty;
                this.DatiRilievoRTF = string.Empty;

                System.Drawing.Font f = rtf.getFontFromString(Database.GetConfigTable(EnumConfigTable.FontPredefinitoRTF), false, false);
                System.Drawing.Font f_grid = rtf.getFontFromString(Database.GetConfigTable(EnumConfigTable.FontPredefinitoRTF), false, false);
                rtfAnteprima = rtf.initRtf(f);
                rtfDatiObbligatoriMancanti = rtf.initRtf(f);
                rtfDatiRilievo = rtf.initRtf(f);

                Dictionary<string, object> datiMancanti = oGestore.ControlloDatiObbligatori();
                if (datiMancanti.Count > 0)
                {
                    foreach (KeyValuePair<string, object> datoMancante in datiMancanti)
                    {
                        DcVoce dato = (DcVoce)datoMancante.Value;
                        f = rtf.getFontFromString(Database.GetConfigTable(EnumConfigTable.FontPredefinitoRTF), true, false);
                        rtfDatiObbligatoriMancanti = rtf.appendRtfText(rtfDatiObbligatoriMancanti, dato.Descrizione + @"\line ", f);
                    }
                }

                bPrimaSezione = true;
                bSingolaSezione = (oGestore.Scheda.Sezioni.Values.Count == 1 ? true : false);
                bIntestazioneRipetuta = false;
                bHoScrittoValore = false;


                foreach (DcSezione oDcSezione in oGestore.Scheda.Sezioni.Values)
                {


                    oDcSezione.Attributi.TryGetValue(EnumAttributiSezione.Ripetibile.ToString(), out DcAttributoRipetibile);
                    bAttributoRipetibile = false;
                    if (DcAttributoRipetibile != null) { bAttributoRipetibile = (bool)DcAttributoRipetibile.Value; }
                    if (oGestore.LeggeSequenze(oDcSezione.Voci.Values.First().Key) > 1) { bAttributoRipetibile = true; }

                    oDcSezione.Attributi.TryGetValue(EnumAttributiSezione.TipoSezione.ToString(), out DcAttributoTipoSezione);
                    enAttributoTipoSezione = enumTipoSezione.Standard;
                    if (DcAttributoTipoSezione != null) { enAttributoTipoSezione = (enumTipoSezione)DcAttributoTipoSezione.Value; }

                    oDcSezione.Attributi.TryGetValue(EnumAttributiSezione.Stile.ToString(), out DcAttributoStile);
                    sAttributoStile = string.Empty;
                    if (DcAttributoStile != null) { sAttributoStile = DcAttributoStile.Value.ToString(); }
                    ParseResult = oGestore.Valutatore.Parsing(sAttributoStile);
                    SezioneResult = rtf.getSezioneResult(ParseResult);
                    sGrigliaBordi = string.Empty;
                    if (SezioneResult[RtfFiles.RTF_BORDI][RtfFiles.RTF_BORDI].X != 0)
                    {
                        sGrigliaBordi += string.Format(TAG_GRIDCEL_BORDER, "t", SezioneResult[RtfFiles.RTF_BORDI][RtfFiles.RTF_BORDI].X);
                    }
                    if (SezioneResult[RtfFiles.RTF_BORDI][RtfFiles.RTF_BORDI].Y != 0)
                    {
                        sGrigliaBordi += string.Format(TAG_GRIDCEL_BORDER, "l", SezioneResult[RtfFiles.RTF_BORDI][RtfFiles.RTF_BORDI].Y);
                    }
                    if (SezioneResult[RtfFiles.RTF_BORDI][RtfFiles.RTF_BORDI].Width != 0)
                    {
                        sGrigliaBordi += string.Format(TAG_GRIDCEL_BORDER, "b", SezioneResult[RtfFiles.RTF_BORDI][RtfFiles.RTF_BORDI].Width);
                    }
                    if (SezioneResult[RtfFiles.RTF_BORDI][RtfFiles.RTF_BORDI].Height != 0)
                    {
                        sGrigliaBordi += string.Format(TAG_GRIDCEL_BORDER, "r", SezioneResult[RtfFiles.RTF_BORDI][RtfFiles.RTF_BORDI].Height);
                    }
                    if (SezioneResult[RtfFiles.RTF_FONT][RtfFiles.RTF_DIMENSIONE] != 0)
                    {
                        f_grid = new Font(f_grid.FontFamily, SezioneResult[RtfFiles.RTF_FONT][RtfFiles.RTF_DIMENSIONE]);
                    }

                    if (!bSingolaSezione)
                    {

                        bIntestazioneRipetuta = false;

                        isValidRtf(rtfAnteprima, out string rtextAnteprima);
                        isValidRtf(rtfDatiRilievo, out string rtextDatiRilievo);

                        if (!bPrimaSezione && bHoScrittoValore)
                        {

                            int lenghtAnteprimaSez = rtextAnteprima.Length - 1;
                            int lenghtRilievoSez = rtextDatiRilievo.Length - 1;
                            if (lenghtRilievoSez < 0) lenghtRilievoSez = 0;
                            if (lenghtAnteprimaSez < 0) lenghtAnteprimaSez = 0;

                            if (rtextAnteprima != "")
                                if (rtextAnteprima[lenghtAnteprimaSez] != '\n') rtfAnteprima = rtf.appendRtfText(rtfAnteprima, (enAttributoTipoSezione == enumTipoSezione.Griglia ? @"\par " : @"\line "), f);
                            if (rtextDatiRilievo != "")
                                if (rtextDatiRilievo[lenghtRilievoSez] != '\n') rtfDatiRilievo = rtf.appendRtfText(rtfDatiRilievo, (enAttributoTipoSezione == enumTipoSezione.Griglia ? @"\par " : @"\line "), f);

                            bHoScrittoValore = false;

                        }

                    }

                    bPrimaSezione = false;

                    f = rtf.getFontFromString(Database.GetConfigTable(EnumConfigTable.FontPredefinitoRTF), false, false);

                    if (enAttributoTipoSezione == enumTipoSezione.Griglia)
                    {

                        sTagColonneTipoGriglia = string.Empty;
                        sTagValoriTipoGriglia = string.Empty;
                        nWithGridCol = 0;
                        ColorResultGriglia = new Dictionary<string, Color>();
                        foreach (DcVoce oDcVoce in oDcSezione.Voci.Values)
                        {

                            DcAttributo DcAttributoLarghezzaDes = new DcAttributo();
                            oGestore.SchedaLayouts.Layouts[oDcVoce.Key].Attributi.TryGetValue(EnumAttributiSchedaLayout.LarghezzaDes.ToString(), out DcAttributoLarghezzaDes);
                            int nAttributoLarghezzaDes = 0;
                            if (DcAttributoLarghezzaDes != null) { nAttributoLarghezzaDes = Convert.ToInt32(DcAttributoLarghezzaDes.Value); }

                            string sEtichetta = GeneraRTFDescrizione(oDcVoce, null, bAttributoBoldEtichetta,
                                                (SezioneResult[RtfFiles.RTF_DIMENSIONE][RtfFiles.RTF_DIMENSIONE] == 0 ? false : SezioneResult[RtfFiles.RTF_NASCONDISEPARATOREETICHETTE][RtfFiles.RTF_NASCONDISEPARATOREETICHETTE]));


                            oGestore.SchedaLayouts.Layouts[oDcVoce.Key].Attributi.TryGetValue(EnumAttributiSchedaLayout.Stile.ToString(), out DcAttributoStile);
                            sAttributoStile = string.Empty;
                            if (DcAttributoStile != null) { sAttributoStile = (string)DcAttributoStile.Value; }
                            if (sAttributoStile != string.Empty)
                            {
                                ParseResult = oGestore.Valutatore.Parsing(sAttributoStile);
                            }
                            else
                            {
                                ParseResult = new Dictionary<string, ParseResult>();
                            }
                            FontResult = rtf.getFontFromStile(Database.GetConfigTable(EnumConfigTable.FontPredefinitoRTF), ParseResult);
                            if (Database.GetConfigTable(EnumConfigTable.FontPredefinitoRTF).IndexOf(FontResult[RtfFiles.RTF_ETICHETTA].Size.ToString()) != -1 && SezioneResult[RtfFiles.RTF_FONT][RtfFiles.RTF_DIMENSIONE] != 0)
                            {
                                FontResult = rtf.getFontFromStile(string.Format("{0}; {1}pt", FontResult[RtfFiles.RTF_ETICHETTA].Name, SezioneResult[RtfFiles.RTF_FONT][RtfFiles.RTF_DIMENSIONE]), ParseResult);
                            }
                            if (Database.GetConfigTable(EnumConfigTable.FontPredefinitoRTF).IndexOf(FontResult[RtfFiles.RTF_VALORE].Size.ToString()) != -1 && SezioneResult[RtfFiles.RTF_FONT][RtfFiles.RTF_DIMENSIONE] != 0)
                            {
                                FontResult = rtf.getFontFromStile(string.Format("{0}; {1}pt", FontResult[RtfFiles.RTF_VALORE].Name, SezioneResult[RtfFiles.RTF_FONT][RtfFiles.RTF_DIMENSIONE]), ParseResult);
                            }
                            ColorResult = rtf.getColorFromStile(ParseResult);
                            BackColorResult = rtf.getBackColorFromStile(ParseResult);

                            OpzioniResult = rtf.getOpzioniFromStile(ParseResult);
                            if (OpzioniResult[RtfFiles.RTF_OPZIONI].Contains(RtfFiles.RTF_NASCOSTO) == false)
                            {

                                if (BackColorResult[RtfFiles.RTF_ETICHETTA] != Color.Empty)
                                {
                                    ColorResultGriglia.Add(ColorResultGriglia.Count.ToString(), BackColorResult[RtfFiles.RTF_ETICHETTA]);
                                    sTagColonneTipoGriglia += string.Format(@"{0}{1}", TAG_GRIDCEL_BACKCOLOR, ColorResultGriglia.Count);
                                }

                                nWithGridCol += (SezioneResult[RtfFiles.RTF_DIMENSIONE][RtfFiles.RTF_DIMENSIONE] / 100 * nAttributoLarghezzaDes);
                                sTagColonneTipoGriglia += string.Format("{2}{0}{1}", TAG_GRIDCOL_INIZIO, nWithGridCol.ToString(), sGrigliaBordi);

                                if (ColorResult[RtfFiles.RTF_ETICHETTA] == Color.Empty)
                                {
                                    if (SezioneResult[RtfFiles.RTF_FONT][RtfFiles.RTF_DIMENSIONE] != 0 && SezioneResult[RtfFiles.RTF_FONT][RtfFiles.RTF_DIMENSIONE] != FontResult[RtfFiles.RTF_ETICHETTA].Size)
                                    {
                                        sTagValoriTipoGriglia += string.Format(@"{0}  {1}{2}", TAG_GRIDVAL_INIZIO, getAttributoFont(sEtichetta, FontResult[RtfFiles.RTF_ETICHETTA], SezioneResult), TAG_GRIDVAL_FINE);
                                    }
                                    else
                                    {
                                        sTagValoriTipoGriglia += string.Format(@"{0}  {1}{2}", TAG_GRIDVAL_INIZIO, getAttributoFont(sEtichetta, FontResult[RtfFiles.RTF_ETICHETTA]), TAG_GRIDVAL_FINE);
                                    }
                                }
                                else
                                {
                                    ColorResultGriglia.Add(ColorResultGriglia.Count.ToString(), ColorResult[RtfFiles.RTF_ETICHETTA]);

                                    if (SezioneResult[RtfFiles.RTF_FONT][RtfFiles.RTF_DIMENSIONE] != 0 && SezioneResult[RtfFiles.RTF_FONT][RtfFiles.RTF_DIMENSIONE] != FontResult[RtfFiles.RTF_ETICHETTA].Size)
                                    {
                                        sTagValoriTipoGriglia += string.Format(@"{0}\cf{3}  {1}\cf0{2}", TAG_GRIDVAL_INIZIO, getAttributoFont(sEtichetta, FontResult[RtfFiles.RTF_ETICHETTA], SezioneResult), TAG_GRIDVAL_FINE, ColorResultGriglia.Count);
                                    }
                                    else
                                    {
                                        sTagValoriTipoGriglia += string.Format(@"{0}\cf{3}  {1}\cf0{2}", TAG_GRIDVAL_INIZIO, getAttributoFont(sEtichetta, FontResult[RtfFiles.RTF_ETICHETTA]), TAG_GRIDVAL_FINE, ColorResultGriglia.Count);
                                    }
                                }

                            }

                        }

                    }

                    for (int i = 1; i <= oGestore.LeggeSequenze(oDcSezione.Voci.Values.First().Key); i++)
                    {

                        if (bAttributoRipetibile == true && !bSingolaSezione)
                        {

                            if (bIntestazioneRipetuta == false && oDcSezione.Descrizione != string.Empty)
                            {

                                bool bAnteprima = false;
                                bool bInRilievo = false;

                                foreach (DcVoce oDcVoce in oDcSezione.Voci.Values)
                                {
                                    if (oGestore.LeggeDato(oDcVoce.Key, i) != null)
                                    {
                                        DcDato oDato1 = oGestore.LeggeDato(oDcVoce.Key, i);
                                        if (oDato1.Value != null && oDato1.Value.ToString() != "" && oDato1.Value.ToString() != "False")
                                        {
                                            bAnteprima = true;
                                        }
                                        if (oDato1.InRilievo == true)
                                        {
                                            bInRilievo = true;
                                        }
                                    }
                                    if (bAnteprima == true && bInRilievo == true)
                                    {
                                        break;
                                    }
                                }

                                if (bAnteprima)
                                {
                                    if (SezioneResult[RtfFiles.RTF_NASCONDITITOLO][RtfFiles.RTF_NASCONDITITOLO] == false)
                                    {
                                        rtfAnteprima = rtf.appendRtfText(rtfAnteprima, @"\line \b " + oDcSezione.Descrizione + @" : \b0 \par ", f);
                                        bIntestazioneRipetuta = true;
                                    }
                                    else
                                    {
                                        bIntestazioneRipetuta = true;
                                    }
                                }
                                if (bInRilievo)
                                {
                                    if (SezioneResult[RtfFiles.RTF_NASCONDITITOLO][RtfFiles.RTF_NASCONDITITOLO] == false)
                                    {
                                        rtfDatiRilievo = rtf.appendRtfText(rtfDatiRilievo, @"\line\b " + oDcSezione.Descrizione + @" : \b0 \par ", f);
                                    }
                                }

                            }
                            else
                            {
                                int nline = rtfAnteprima.LastIndexOf(@"\line");
                                int ntrowd = rtfAnteprima.LastIndexOf(@"\trowd");
                                if (nline == -1)
                                {
                                    if (SezioneResult[RtfFiles.RTF_DIMENSIONE][RtfFiles.RTF_DIMENSIONE] == 0 && bAttributoACapo == true)
                                    {
                                        rtfAnteprima = rtf.appendRtfText(rtfAnteprima, @"\line ", f);
                                    }
                                }
                                else
                                {
                                    if (nline < (rtfAnteprima.Length - 7) && ntrowd < nline)
                                    {
                                        if (SezioneResult[RtfFiles.RTF_DIMENSIONE][RtfFiles.RTF_DIMENSIONE] == 0 && bAttributoACapo == true)
                                        {
                                            rtfAnteprima = rtf.appendRtfText(rtfAnteprima, @"\line ", f);
                                        }
                                    }
                                }
                            }

                        }
                        else
                        {
                            int nline = rtfAnteprima.LastIndexOf(@"\line");
                            int ntrowd = rtfAnteprima.LastIndexOf(@"\trowd");
                            if (nline == -1)
                            {
                                if (SezioneResult[RtfFiles.RTF_DIMENSIONE][RtfFiles.RTF_DIMENSIONE] == 0)
                                {
                                    if (!bPrimaSezione && bHoScrittoValore)
                                    {
                                        rtfAnteprima = rtf.appendRtfText(rtfAnteprima, @"\line ", f);
                                    }
                                }
                            }
                            else
                            {
                                if (nline < (rtfAnteprima.Length - 7) && ntrowd < nline)
                                {
                                    if (SezioneResult[RtfFiles.RTF_DIMENSIONE][RtfFiles.RTF_DIMENSIONE] == 0)
                                    {
                                        rtfAnteprima = rtf.appendRtfText(rtfAnteprima, @"\line ", f);

                                        int nlineDatiRilievo = rtfDatiRilievo.LastIndexOf(@"\line");
                                        if (nlineDatiRilievo == -1 && rtfDatiRilievo != rtf.initRtf(f))
                                        {
                                            rtfDatiRilievo = rtf.appendRtfText(rtfDatiRilievo, @"\line ", f);
                                        }

                                    }
                                }
                            }
                        }

                        sTagColonne = string.Empty;
                        sTagValori = string.Empty;
                        nWithGridCol = 0;
                        bHoScrittoValore = false;

                        foreach (DcVoce oDcVoce in oDcSezione.Voci.Values)
                        {
                            if ((oGestore.LeggeDato(oDcVoce.Key, i) != null) && (!oDcVoce.Key.StartsWith("H")))
                            {

                                DcDato oDato1 = oGestore.LeggeDato(oDcVoce.Key, i);

                                oGestore.SchedaLayouts.Layouts[oDcVoce.Key].Attributi.TryGetValue(EnumAttributiSchedaLayout.NuovaRiga.ToString(), out DcAttributoACapo);
                                bAttributoACapo = false;
                                if (DcAttributoACapo != null) { bAttributoACapo = (bool)DcAttributoACapo.Value; }

                                oGestore.SchedaLayouts.Layouts[oDcVoce.Key].Attributi.TryGetValue(EnumAttributiSchedaLayout.Stile.ToString(), out DcAttributoStile);
                                sAttributoStile = string.Empty;
                                if (DcAttributoStile != null) { sAttributoStile = (string)DcAttributoStile.Value; }
                                if (sAttributoStile != string.Empty)
                                {
                                    ParseResult = oGestore.Valutatore.Parsing(sAttributoStile);
                                    if (sAttributoStile.ToUpper() == RtfFiles.RTF_VISUALIZZASEMPRE.ToUpper())
                                    {
                                        bAttributoBoldEtichetta = true;
                                    }
                                    else
                                    {
                                        bAttributoBoldEtichetta = !(sAttributoStile).ToUpper().Contains(RtfFiles.RTF_ETICHETTA);
                                    }
                                    bAttributoVisualizzaEtichetta = sAttributoStile.ToUpper().Contains(RtfFiles.RTF_VISUALIZZASEMPRE.ToUpper());
                                }
                                else
                                {
                                    ParseResult = new Dictionary<string, ParseResult>();
                                    bAttributoBoldEtichetta = true;
                                    bAttributoVisualizzaEtichetta = false;
                                }
                                FontResult = rtf.getFontFromStile(Database.GetConfigTable(EnumConfigTable.FontPredefinitoRTF), ParseResult);
                                if (Database.GetConfigTable(EnumConfigTable.FontPredefinitoRTF).IndexOf(FontResult[RtfFiles.RTF_ETICHETTA].Size.ToString()) != -1 && SezioneResult[RtfFiles.RTF_FONT][RtfFiles.RTF_DIMENSIONE] != 0)
                                {
                                    FontResult = rtf.getFontFromStile(string.Format("{0}; {1}pt", FontResult[RtfFiles.RTF_ETICHETTA].Name, SezioneResult[RtfFiles.RTF_FONT][RtfFiles.RTF_DIMENSIONE]), ParseResult);
                                }
                                if (Database.GetConfigTable(EnumConfigTable.FontPredefinitoRTF).IndexOf(FontResult[RtfFiles.RTF_VALORE].Size.ToString()) != -1 && SezioneResult[RtfFiles.RTF_FONT][RtfFiles.RTF_DIMENSIONE] != 0)
                                {
                                    FontResult = rtf.getFontFromStile(string.Format("{0}; {1}pt", FontResult[RtfFiles.RTF_VALORE].Name, SezioneResult[RtfFiles.RTF_FONT][RtfFiles.RTF_DIMENSIONE]), ParseResult);
                                }
                                ColorResult = rtf.getColorFromStile(ParseResult);
                                BackColorResult = rtf.getBackColorFromStile(ParseResult);
                                OpzioniResult = rtf.getOpzioniFromStile(ParseResult);
                                DimensioniResult = rtf.getDimensioniFromStile(ParseResult);
                                RigheResult = rtf.getRigheFromStile(ParseResult);

                                if (OpzioniResult[RtfFiles.RTF_OPZIONI].Contains(RtfFiles.RTF_NASCOSTO) == false)
                                {

                                    if (bAttributoACapo == true && bHoScrittoACapo == true)
                                    {

                                        bHoScrittoACapo = false;

                                        if (SezioneResult[RtfFiles.RTF_DIMENSIONE][RtfFiles.RTF_DIMENSIONE] != 0)
                                        {
                                            if (sTagColonne != string.Empty && sTagValori != string.Empty && nWithGridCol != 0)
                                            {
                                                if (bHoScrittoValore || SezioneResult[RtfFiles.RTF_VISUALIZZASEMPREETICHETTE][RtfFiles.RTF_VISUALIZZASEMPREETICHETTE] == true)
                                                {
                                                    if (ColorResultGriglia.Count > 0)
                                                    {
                                                        string newrtf = rtf.initRtf(f_grid, ColorResultGriglia.Select(z => z.Value).ToArray());
                                                        if (enAttributoTipoSezione == enumTipoSezione.Griglia && sTagColonneTipoGriglia != string.Empty && sTagValoriTipoGriglia != string.Empty)
                                                        {
                                                            newrtf = rtf.appendRtfText(newrtf, TAG_GRIDROW_INIZIO + sTagColonneTipoGriglia + sTagValoriTipoGriglia + TAG_GRIDROW_FINE, f_grid);
                                                            sTagColonneTipoGriglia = string.Empty;
                                                            sTagValoriTipoGriglia = string.Empty;
                                                        }
                                                        newrtf = rtf.appendRtfText(newrtf, TAG_GRIDROW_INIZIO + sTagColonne + sTagValori + TAG_GRIDROW_FINE, f_grid);
                                                        bHoScrittoValore = false;
                                                        rtfAnteprima = rtf.joinRtf(newrtf, rtfAnteprima);
                                                    }
                                                    else
                                                    {
                                                        if (enAttributoTipoSezione == enumTipoSezione.Griglia && sTagColonneTipoGriglia != string.Empty && sTagValoriTipoGriglia != string.Empty)
                                                        {
                                                            rtfAnteprima = rtf.appendRtfText(rtfAnteprima, TAG_GRIDROW_INIZIO + sTagColonneTipoGriglia + sTagValoriTipoGriglia + TAG_GRIDROW_FINE, f_grid);
                                                            sTagColonneTipoGriglia = string.Empty;
                                                            sTagValoriTipoGriglia = string.Empty;
                                                        }
                                                        rtfAnteprima = rtf.appendRtfText(rtfAnteprima, TAG_GRIDROW_INIZIO + sTagColonne + sTagValori + TAG_GRIDROW_FINE, f_grid);
                                                        bHoScrittoValore = false;
                                                    }
                                                }
                                            }

                                            sTagColonne = string.Empty;
                                            sTagValori = string.Empty;
                                            nWithGridCol = 0;
                                        }

                                        int nline = rtfAnteprima.LastIndexOf(@"\line");
                                        int ntrowd = rtfAnteprima.LastIndexOf(@"\trowd");

                                        if (nline == -1)
                                        {
                                            if (SezioneResult[RtfFiles.RTF_DIMENSIONE][RtfFiles.RTF_DIMENSIONE] == 0 && bAttributoACapo == true)
                                            {
                                                rtfAnteprima = rtf.appendRtfText(rtfAnteprima, @"\line ", f);
                                            }
                                        }
                                        else
                                        {
                                            if (nline < (rtfAnteprima.Length - 7))
                                            {
                                                if (SezioneResult[RtfFiles.RTF_DIMENSIONE][RtfFiles.RTF_DIMENSIONE] == 0 && bAttributoACapo == true)
                                                {
                                                    rtfAnteprima = rtf.appendRtfText(rtfAnteprima, @"\line ", f);
                                                }
                                            }
                                        }

                                        if (rtfDatiRilievo != rtf.initRtf(f))
                                        {
                                            int nlineDatiRilievo = rtfDatiRilievo.LastIndexOf(@"\line");

                                            if ((nlineDatiRilievo == -1) || (nlineDatiRilievo < (rtfDatiRilievo.Length - 7)))
                                            {
                                                if (SezioneResult[RtfFiles.RTF_DIMENSIONE][RtfFiles.RTF_DIMENSIONE] == 0 && bAttributoACapo == true)
                                                {
                                                    rtfDatiRilievo = rtf.appendRtfText(rtfDatiRilievo, @"\line ", f);
                                                }
                                            }
                                        }
                                    }

                                    if (oGestore.SchedaLayouts.Layouts.ContainsKey(oDcVoce.Key) == true)
                                    {
                                        if (oDato1 != null) if (oDato1.Value == null) oDato1.Value = "";
                                        if (oDato1.Value.ToString() != "" || oGestore.SchedaLayouts.Layouts[oDcVoce.Key].TipoVoce == enumTipoVoce.Etichetta
                                                                            || oGestore.SchedaLayouts.Layouts[oDcVoce.Key].TipoVoce == enumTipoVoce.Logo)
                                        {
                                            string sEtichetta = string.Empty;

                                            if (SezioneResult[RtfFiles.RTF_DIMENSIONE][RtfFiles.RTF_DIMENSIONE] == 0)
                                            {
                                                sEtichetta = GeneraRTFDescrizione(oDcVoce, oDato1, bAttributoBoldEtichetta,
                                                    (SezioneResult[RtfFiles.RTF_DIMENSIONE][RtfFiles.RTF_DIMENSIONE] == 0 ? false : SezioneResult[RtfFiles.RTF_NASCONDISEPARATOREETICHETTE][RtfFiles.RTF_NASCONDISEPARATOREETICHETTE]));
                                            }
                                            else
                                            {

                                                sEtichetta = GeneraRTFDescrizione(oDcVoce, null, bAttributoBoldEtichetta,
                                                (SezioneResult[RtfFiles.RTF_DIMENSIONE][RtfFiles.RTF_DIMENSIONE] == 0 ? false : SezioneResult[RtfFiles.RTF_NASCONDISEPARATOREETICHETTE][RtfFiles.RTF_NASCONDISEPARATOREETICHETTE]));
                                            }


                                            if (SezioneResult[RtfFiles.RTF_DIMENSIONE][RtfFiles.RTF_DIMENSIONE] == 0)
                                            {
                                                bHoScrittoValore = true;
                                                bHoScrittoACapo = true;
                                                GeneraRTFCampo(oGestore.SchedaLayouts.Layouts[oDcVoce.Key].TipoVoce, sEtichetta, oDcVoce, oDato1, oDcSezione, i, bAttributoBoldEtichetta, bAttributoVisualizzaEtichetta,
                                                                oGestore, rtf, f,
                                                                FontResult, ColorResult, DimensioniResult, OpzioniResult, RigheResult,
                                                                ref rtfDatiRilievo, ref rtfAnteprima);
                                            }
                                            else
                                            {
                                                if (enAttributoTipoSezione == enumTipoSezione.Standard)
                                                {

                                                    DcAttributo DcAttributoLarghezzaDes = new DcAttributo();
                                                    DcAttributo DcAttributoLarghezzaTipo = new DcAttributo();
                                                    DcAttributo DcAttributoAllineamentoTipo = new DcAttributo();
                                                    oGestore.SchedaLayouts.Layouts[oDcVoce.Key].Attributi.TryGetValue(EnumAttributiSchedaLayout.LarghezzaDes.ToString(), out DcAttributoLarghezzaDes);
                                                    oGestore.SchedaLayouts.Layouts[oDcVoce.Key].Attributi.TryGetValue(EnumAttributiSchedaLayout.LarghezzaTipo.ToString(), out DcAttributoLarghezzaTipo);
                                                    oGestore.SchedaLayouts.Layouts[oDcVoce.Key].Attributi.TryGetValue(EnumAttributiSchedaLayout.AlignTipo.ToString(), out DcAttributoAllineamentoTipo);
                                                    int nAttributoLarghezzaDes = 0;
                                                    int nAttributoLarghezzaTipo = 0;
                                                    HorizontalAlignment enAllineamentoTipo = HorizontalAlignment.Left;
                                                    if (DcAttributoLarghezzaDes != null) { nAttributoLarghezzaDes = Convert.ToInt32(DcAttributoLarghezzaDes.Value); }
                                                    if (DcAttributoLarghezzaTipo != null) { nAttributoLarghezzaTipo = Convert.ToInt32(DcAttributoLarghezzaTipo.Value); }
                                                    if (DcAttributoAllineamentoTipo != null) { enAllineamentoTipo = (HorizontalAlignment)DcAttributoAllineamentoTipo.Value; }

                                                    if (oGestore.SchedaLayouts.Layouts[oDcVoce.Key].TipoVoce == enumTipoVoce.Etichetta)
                                                    {
                                                        nWithGridCol += (SezioneResult[RtfFiles.RTF_DIMENSIONE][RtfFiles.RTF_DIMENSIONE] / 100 * nAttributoLarghezzaDes);
                                                        nWithGridCol += (SezioneResult[RtfFiles.RTF_DIMENSIONE][RtfFiles.RTF_DIMENSIONE] / 100 * nAttributoLarghezzaTipo);

                                                        sTagColonne += string.Format("{2}{0}{1}", TAG_GRIDCOL_INIZIO, nWithGridCol.ToString(), sGrigliaBordi);
                                                        sTagValori += string.Format(@"{0}{3}  {1}{2}", TAG_GRIDVAL_INIZIO, sEtichetta, TAG_GRIDVAL_FINE, getAllineamentoTipo(enAllineamentoTipo));

                                                    }
                                                    else
                                                    {
                                                        string sCampo = GeneraCampo(oGestore.SchedaLayouts.Layouts[oDcVoce.Key].TipoVoce, "", oDcVoce, oDato1, oGestore, OpzioniResult);
                                                        if (sEtichetta == String.Empty)
                                                        {
                                                            nWithGridCol += (SezioneResult[RtfFiles.RTF_DIMENSIONE][RtfFiles.RTF_DIMENSIONE] / 100 * nAttributoLarghezzaDes);
                                                            nWithGridCol += (SezioneResult[RtfFiles.RTF_DIMENSIONE][RtfFiles.RTF_DIMENSIONE] / 100 * nAttributoLarghezzaTipo);
                                                            sTagColonne += string.Format("{2}{0}{1}", TAG_GRIDCOL_INIZIO, nWithGridCol.ToString(), sGrigliaBordi);
                                                            sTagValori += string.Format(@"{0}{3}  {1}{2}", TAG_GRIDVAL_INIZIO, sCampo, TAG_GRIDVAL_FINE, getAllineamentoTipo(enAllineamentoTipo));
                                                            bHoScrittoValore = true;
                                                        }
                                                        else
                                                        {
                                                            if (BackColorResult[RtfFiles.RTF_ETICHETTA] != Color.Empty)
                                                            {
                                                                ColorResultGriglia.Add(ColorResultGriglia.Count.ToString(), BackColorResult[RtfFiles.RTF_ETICHETTA]);
                                                                sTagColonne += string.Format(@"{0}{1}", TAG_GRIDCEL_BACKCOLOR, ColorResultGriglia.Count); ;
                                                            }
                                                            nWithGridCol += (SezioneResult[RtfFiles.RTF_DIMENSIONE][RtfFiles.RTF_DIMENSIONE] / 100 * nAttributoLarghezzaDes);
                                                            sTagColonne += string.Format("{2}{0}{1}", TAG_GRIDCOL_INIZIO, nWithGridCol.ToString(), sGrigliaBordi);
                                                            if (ColorResult[RtfFiles.RTF_ETICHETTA] == Color.Empty)
                                                            {
                                                                if (SezioneResult[RtfFiles.RTF_FONT][RtfFiles.RTF_DIMENSIONE] != 0 && SezioneResult[RtfFiles.RTF_FONT][RtfFiles.RTF_DIMENSIONE] != FontResult[RtfFiles.RTF_ETICHETTA].Size)
                                                                {
                                                                    sTagValori += string.Format(@"{0}  {1}{2}", TAG_GRIDVAL_INIZIO, getAttributoFont(sEtichetta, FontResult[RtfFiles.RTF_ETICHETTA], SezioneResult), TAG_GRIDVAL_FINE);
                                                                }
                                                                else
                                                                {
                                                                    sTagValori += string.Format(@"{0}  {1}{2}", TAG_GRIDVAL_INIZIO, getAttributoFont(sEtichetta, FontResult[RtfFiles.RTF_ETICHETTA]), TAG_GRIDVAL_FINE);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                ColorResultGriglia.Add(ColorResultGriglia.Count.ToString(), ColorResult[RtfFiles.RTF_ETICHETTA]);
                                                                if (SezioneResult[RtfFiles.RTF_FONT][RtfFiles.RTF_DIMENSIONE] != 0 && SezioneResult[RtfFiles.RTF_FONT][RtfFiles.RTF_DIMENSIONE] != FontResult[RtfFiles.RTF_ETICHETTA].Size)
                                                                {
                                                                    sTagValori += string.Format(@"{0}\cf{3}  {1}\cf0{2}", TAG_GRIDVAL_INIZIO, getAttributoFont(sEtichetta, FontResult[RtfFiles.RTF_ETICHETTA], SezioneResult), TAG_GRIDVAL_FINE, ColorResultGriglia.Count);
                                                                }
                                                                else
                                                                {
                                                                    sTagValori += string.Format(@"{0}\cf{3}  {1}\cf0{2}", TAG_GRIDVAL_INIZIO, getAttributoFont(sEtichetta, FontResult[RtfFiles.RTF_ETICHETTA]), TAG_GRIDVAL_FINE, ColorResultGriglia.Count);
                                                                }
                                                            }

                                                            if (BackColorResult[RtfFiles.RTF_VALORE] != Color.Empty)
                                                            {
                                                                ColorResultGriglia.Add(ColorResultGriglia.Count.ToString(), BackColorResult[RtfFiles.RTF_VALORE]);
                                                                sTagColonne += string.Format(@"{0}{1}", TAG_GRIDCEL_BACKCOLOR, ColorResultGriglia.Count); ;
                                                            }
                                                            nWithGridCol += (SezioneResult[RtfFiles.RTF_DIMENSIONE][RtfFiles.RTF_DIMENSIONE] / 100 * nAttributoLarghezzaTipo);
                                                            sTagColonne += string.Format("{2}{0}{1}", TAG_GRIDCOL_INIZIO, nWithGridCol.ToString(), sGrigliaBordi);
                                                            if (ColorResult[RtfFiles.RTF_VALORE] == Color.Empty)
                                                            {
                                                                if (SezioneResult[RtfFiles.RTF_FONT][RtfFiles.RTF_DIMENSIONE] != 0 && SezioneResult[RtfFiles.RTF_FONT][RtfFiles.RTF_DIMENSIONE] != FontResult[RtfFiles.RTF_VALORE].Size)
                                                                {
                                                                    sTagValori += string.Format(@"{0}  {1}{2}", TAG_GRIDVAL_INIZIO, getAttributoFont(sCampo, FontResult[RtfFiles.RTF_VALORE], SezioneResult), TAG_GRIDVAL_FINE);
                                                                }
                                                                else
                                                                {
                                                                    sTagValori += string.Format(@"{0}  {1}{2}", TAG_GRIDVAL_INIZIO, getAttributoFont(sCampo, FontResult[RtfFiles.RTF_VALORE]), TAG_GRIDVAL_FINE);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                ColorResultGriglia.Add(ColorResultGriglia.Count.ToString(), ColorResult[RtfFiles.RTF_VALORE]);
                                                                if (SezioneResult[RtfFiles.RTF_FONT][RtfFiles.RTF_DIMENSIONE] != 0 && SezioneResult[RtfFiles.RTF_FONT][RtfFiles.RTF_DIMENSIONE] != FontResult[RtfFiles.RTF_VALORE].Size)
                                                                {
                                                                    sTagValori += string.Format(@"{0}\cf{3}  {1}\cf0{2}", TAG_GRIDVAL_INIZIO, getAttributoFont(sCampo, FontResult[RtfFiles.RTF_VALORE], SezioneResult), TAG_GRIDVAL_FINE, ColorResultGriglia.Count);
                                                                }
                                                                else
                                                                {
                                                                    sTagValori += string.Format(@"{0}\cf{3}  {1}\cf0{2}", TAG_GRIDVAL_INIZIO, getAttributoFont(sCampo, FontResult[RtfFiles.RTF_VALORE]), TAG_GRIDVAL_FINE, ColorResultGriglia.Count);
                                                                }
                                                            }
                                                            bHoScrittoValore = true;
                                                        }
                                                        if (oDato1.InRilievo)
                                                        {
                                                            sCampo = GeneraCampo(oGestore.SchedaLayouts.Layouts[oDcVoce.Key].TipoVoce, "", oDcVoce, oDato1, oGestore, null);
                                                            rtfDatiRilievo = rtf.appendRtfText(rtfDatiRilievo, sEtichetta + sCampo + @" \tab ", f);
                                                        }
                                                    }

                                                    bHoScrittoACapo = true;

                                                }
                                                else if (enAttributoTipoSezione == enumTipoSezione.Griglia)
                                                {

                                                    string sCampo = GeneraCampo(oGestore.SchedaLayouts.Layouts[oDcVoce.Key].TipoVoce, "", oDcVoce, oDato1, oGestore, OpzioniResult);
                                                    DcAttributo DcAttributoLarghezzaTipo = new DcAttributo();
                                                    oGestore.SchedaLayouts.Layouts[oDcVoce.Key].Attributi.TryGetValue(EnumAttributiSchedaLayout.LarghezzaTipo.ToString(), out DcAttributoLarghezzaTipo);
                                                    int nAttributoLarghezzaTipo = 0;
                                                    if (DcAttributoLarghezzaTipo != null) { nAttributoLarghezzaTipo = Convert.ToInt32(DcAttributoLarghezzaTipo.Value); }

                                                    if (BackColorResult[RtfFiles.RTF_VALORE] != Color.Empty)
                                                    {
                                                        ColorResultGriglia.Add(ColorResultGriglia.Count.ToString(), BackColorResult[RtfFiles.RTF_VALORE]);
                                                        sTagColonne += string.Format(@"{0}{1}", TAG_GRIDCEL_BACKCOLOR, ColorResultGriglia.Count); ;
                                                    }

                                                    nWithGridCol += (SezioneResult[RtfFiles.RTF_DIMENSIONE][RtfFiles.RTF_DIMENSIONE] / 100 * nAttributoLarghezzaTipo);
                                                    sTagColonne += string.Format("{2}{0}{1}", TAG_GRIDCOL_INIZIO, nWithGridCol.ToString(), sGrigliaBordi);
                                                    if (ColorResult[RtfFiles.RTF_VALORE] == Color.Empty)
                                                    {
                                                        if (SezioneResult[RtfFiles.RTF_FONT][RtfFiles.RTF_DIMENSIONE] != 0 && SezioneResult[RtfFiles.RTF_FONT][RtfFiles.RTF_DIMENSIONE] != FontResult[RtfFiles.RTF_VALORE].Size)
                                                        {
                                                            sTagValori += string.Format(@"{0}  {1}{2}", TAG_GRIDVAL_INIZIO, getAttributoFont(sCampo, FontResult[RtfFiles.RTF_VALORE], SezioneResult), TAG_GRIDVAL_FINE);
                                                        }
                                                        else
                                                        {
                                                            sTagValori += string.Format(@"{0}  {1}{2}", TAG_GRIDVAL_INIZIO, getAttributoFont(sCampo, FontResult[RtfFiles.RTF_VALORE]), TAG_GRIDVAL_FINE);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ColorResultGriglia.Add(ColorResultGriglia.Count.ToString(), ColorResult[RtfFiles.RTF_VALORE]);
                                                        if (SezioneResult[RtfFiles.RTF_FONT][RtfFiles.RTF_DIMENSIONE] != 0 && SezioneResult[RtfFiles.RTF_FONT][RtfFiles.RTF_DIMENSIONE] != FontResult[RtfFiles.RTF_VALORE].Size)
                                                        {
                                                            sTagValori += string.Format(@"{0}\cf{3}  {1}\cf0{2}", TAG_GRIDVAL_INIZIO, getAttributoFont(sCampo, FontResult[RtfFiles.RTF_VALORE], SezioneResult), TAG_GRIDVAL_FINE, ColorResultGriglia.Count);
                                                        }
                                                        else
                                                        {
                                                            sTagValori += string.Format(@"{0}\cf{3}  {1}\cf0{2}", TAG_GRIDVAL_INIZIO, getAttributoFont(sCampo, FontResult[RtfFiles.RTF_VALORE]), TAG_GRIDVAL_FINE, ColorResultGriglia.Count);
                                                        }
                                                    }
                                                    if (oDato1.InRilievo)
                                                    {
                                                        sCampo = GeneraCampo(oGestore.SchedaLayouts.Layouts[oDcVoce.Key].TipoVoce, "", oDcVoce, oDato1, oGestore, null);
                                                        rtfDatiRilievo = rtf.appendRtfText(rtfDatiRilievo, sEtichetta + sCampo + @" \tab ", f);
                                                    }
                                                    bHoScrittoValore = true;

                                                }

                                            }

                                        }
                                        else
                                        {
                                            if (SezioneResult[RtfFiles.RTF_DIMENSIONE][RtfFiles.RTF_DIMENSIONE] == 0)
                                            {

                                                if (bAttributoVisualizzaEtichetta)
                                                {

                                                    string sEtichetta = string.Empty;

                                                    sEtichetta = GeneraRTFDescrizione(oDcVoce, oDato1, bAttributoBoldEtichetta,
                                                        (SezioneResult[RtfFiles.RTF_DIMENSIONE][RtfFiles.RTF_DIMENSIONE] == 0 ? false : SezioneResult[RtfFiles.RTF_NASCONDISEPARATOREETICHETTE][RtfFiles.RTF_NASCONDISEPARATOREETICHETTE]));

                                                    bHoScrittoValore = true;
                                                    bHoScrittoACapo = true;
                                                    GeneraRTFCampo(oGestore.SchedaLayouts.Layouts[oDcVoce.Key].TipoVoce, sEtichetta, oDcVoce, oDato1, oDcSezione, i, bAttributoBoldEtichetta, bAttributoVisualizzaEtichetta,
                                                                    oGestore, rtf, f,
                                                                    FontResult, ColorResult, DimensioniResult, OpzioniResult, RigheResult,
                                                                    ref rtfDatiRilievo, ref rtfAnteprima);

                                                }

                                            }
                                            else
                                            {
                                                if (enAttributoTipoSezione == enumTipoSezione.Standard)
                                                {
                                                    DcAttributo DcAttributoLarghezzaDes = new DcAttributo();
                                                    DcAttributo DcAttributoLarghezzaTipo = new DcAttributo();
                                                    oGestore.SchedaLayouts.Layouts[oDcVoce.Key].Attributi.TryGetValue(EnumAttributiSchedaLayout.LarghezzaDes.ToString(), out DcAttributoLarghezzaDes);
                                                    oGestore.SchedaLayouts.Layouts[oDcVoce.Key].Attributi.TryGetValue(EnumAttributiSchedaLayout.LarghezzaTipo.ToString(), out DcAttributoLarghezzaTipo);
                                                    int nAttributoLarghezzaDes = 0;
                                                    int nAttributoLarghezzaTipo = 0;
                                                    if (DcAttributoLarghezzaDes != null) { nAttributoLarghezzaDes = Convert.ToInt32(DcAttributoLarghezzaDes.Value); }
                                                    if (DcAttributoLarghezzaTipo != null) { nAttributoLarghezzaTipo = Convert.ToInt32(DcAttributoLarghezzaTipo.Value); }


                                                    nWithGridCol += (SezioneResult[RtfFiles.RTF_DIMENSIONE][RtfFiles.RTF_DIMENSIONE] / 100 * nAttributoLarghezzaDes);
                                                    nWithGridCol += (SezioneResult[RtfFiles.RTF_DIMENSIONE][RtfFiles.RTF_DIMENSIONE] / 100 * nAttributoLarghezzaTipo);
                                                    sTagColonne += string.Format("{2}{0}{1}", TAG_GRIDCOL_INIZIO, nWithGridCol.ToString(), sGrigliaBordi);
                                                    sTagValori += string.Format(@"{0}  {1}{2}", TAG_GRIDVAL_INIZIO, "", TAG_GRIDVAL_FINE);

                                                    bHoScrittoACapo = true;


                                                }
                                                else if (enAttributoTipoSezione == enumTipoSezione.Griglia)
                                                {

                                                    DcAttributo DcAttributoLarghezzaTipo = new DcAttributo();
                                                    oGestore.SchedaLayouts.Layouts[oDcVoce.Key].Attributi.TryGetValue(EnumAttributiSchedaLayout.LarghezzaTipo.ToString(), out DcAttributoLarghezzaTipo);
                                                    int nAttributoLarghezzaTipo = 0;
                                                    if (DcAttributoLarghezzaTipo != null) { nAttributoLarghezzaTipo = Convert.ToInt32(DcAttributoLarghezzaTipo.Value); }

                                                    nWithGridCol += (SezioneResult[RtfFiles.RTF_DIMENSIONE][RtfFiles.RTF_DIMENSIONE] / 100 * nAttributoLarghezzaTipo);
                                                    sTagColonne += string.Format("{2}{0}{1}", TAG_GRIDCOL_INIZIO, nWithGridCol.ToString(), sGrigliaBordi);
                                                    sTagValori += string.Format(@"{0}  {1}{2}", TAG_GRIDVAL_INIZIO, "", TAG_GRIDVAL_FINE);

                                                }

                                            }

                                        }

                                    }

                                }

                            }



                        }

                        if (SezioneResult[RtfFiles.RTF_DIMENSIONE][RtfFiles.RTF_DIMENSIONE] != 0)
                        {
                            if (sTagColonne != string.Empty && sTagValori != string.Empty && nWithGridCol != 0)
                            {
                                if (bHoScrittoValore || SezioneResult[RtfFiles.RTF_VISUALIZZASEMPREETICHETTE][RtfFiles.RTF_VISUALIZZASEMPREETICHETTE] == true)
                                {

                                    if (ColorResultGriglia.Count > 0)
                                    {
                                        string newrtf = rtf.initRtf(f_grid, ColorResultGriglia.Select(z => z.Value).ToArray());
                                        if (enAttributoTipoSezione == enumTipoSezione.Griglia && sTagColonneTipoGriglia != string.Empty && sTagValoriTipoGriglia != string.Empty)
                                        {
                                            newrtf = rtf.appendRtfText(newrtf, TAG_GRIDROW_INIZIO + sTagColonneTipoGriglia + sTagValoriTipoGriglia + TAG_GRIDROW_FINE, f_grid);
                                            sTagColonneTipoGriglia = string.Empty;
                                            sTagValoriTipoGriglia = string.Empty;
                                        }
                                        newrtf = rtf.appendRtfText(newrtf, TAG_GRIDROW_INIZIO + sTagColonne + sTagValori + TAG_GRIDROW_FINE, f_grid);
                                        bHoScrittoValore = false;
                                        rtfAnteprima = rtf.joinRtf(newrtf, rtfAnteprima);
                                    }
                                    else
                                    {
                                        if (enAttributoTipoSezione == enumTipoSezione.Griglia && sTagColonneTipoGriglia != string.Empty && sTagValoriTipoGriglia != string.Empty)
                                        {
                                            rtfAnteprima = rtf.appendRtfText(rtfAnteprima, TAG_GRIDROW_INIZIO + sTagColonneTipoGriglia + sTagValoriTipoGriglia + TAG_GRIDROW_FINE, f_grid);
                                            sTagColonneTipoGriglia = string.Empty;
                                            sTagValoriTipoGriglia = string.Empty;
                                        }
                                        rtfAnteprima = rtf.appendRtfText(rtfAnteprima, TAG_GRIDROW_INIZIO + sTagColonne + sTagValori + TAG_GRIDROW_FINE, f_grid);
                                        bHoScrittoValore = false;
                                    }

                                }
                            }
                        }

                    }

                }

                if (isValidRtf(rtfAnteprima, out string plainText))
                {
                    this.AnteprimaRTF = rtfAnteprima;
                    this.AnteprimaTXT = plainText;
                }
                else
                {
                    this.AnteprimaRTF = string.Empty;
                    this.AnteprimaTXT = " ";
                }

                if (isValidRtf(rtfDatiObbligatoriMancanti, out string plainTextDati))
                {
                    this.DatiObbligatoriMancantiRTF = (plainTextDati != "" ? rtfDatiObbligatoriMancanti : string.Empty);
                }
                else
                {
                    this.DatiObbligatoriMancantiRTF = string.Empty;
                }

                if (isValidRtf(rtfDatiRilievo, out string plainTextDatiRilievo))
                {
                    this.DatiRilievoRTF = (plainTextDatiRilievo != "" ? rtfDatiRilievo : string.Empty);
                }
                else
                {
                    this.DatiRilievoRTF = string.Empty;
                }

            }
            catch (Exception ex)
            {
                this.AnteprimaRTF = string.Empty;
                this.DatiObbligatoriMancantiRTF = string.Empty;
                this.DatiRilievoRTF = string.Empty;
                throw new Exception(ex.Message, ex);
            }

            oGestore = null;

        }

        private string getAttributoFont(string valore, Font f)
        {
            return getAttributoFont(valore, f, null);
        }
        private string getAttributoFont(string valore, Font f, Dictionary<string, dynamic> sr)
        {

            string s = string.Empty;

            switch (f.Style)
            {

                case FontStyle.Bold:
                    s = @"\b " + valore + @"\b0 ";
                    break;

                case FontStyle.Underline:
                    s = @"\ul " + valore + @"\ul0 ";
                    break;

                case FontStyle.Italic:
                    s = @"\i " + valore + @"\i0 ";
                    break;

                case FontStyle.Strikeout:
                    s = @"\strike " + valore + @"\strike0 ";
                    break;

                default:
                    s = valore;
                    break;

            }

            if (sr != null)
            {
                int szDblPtsPrima = Convert.ToInt32(f.Size * 2);
                int szDblPtsDopo = Convert.ToInt32(sr[RtfFiles.RTF_FONT][RtfFiles.RTF_DIMENSIONE] * 2);
                s = string.Format(@"\fs{0} {1} \fs{2}", szDblPtsPrima.ToString(), s, szDblPtsDopo.ToString());
            }

            return s;

        }

        private string GeneraRTFDescrizione(DcVoce oDcVoce, DcDato oDato, bool bBoldEtichetta, bool bSeparatore)
        {

            string _descrizione = string.Empty;
            string _separatore = (bSeparatore == false ? @": " : @"");

            if (oDato != null && oDato.InRilievo && oDato.Value.ToString() != "")
            {
                if (oDcVoce.Descrizione.Trim().Length > 0)
                {
                    _descrizione = oDcVoce.Descrizione.ToString() + @"(*) " + _separatore;
                }
                else
                {
                    _descrizione = @"(*)";
                }
                if (bBoldEtichetta == true) { _descrizione = @"\b " + _descrizione + @" \b0 "; }
            }
            else
            {
                if (oDcVoce.Descrizione.Trim().Length > 0)
                {
                    _descrizione = oDcVoce.Descrizione.ToString() + _separatore;
                    if (bBoldEtichetta == true) { _descrizione = @"\b " + _descrizione + @" \b0 "; }
                }
                else
                {
                    _descrizione = "";
                }
            };

            return _descrizione;

        }

        private void GeneraRTFCampo(enumTipoVoce enumTP, string s_descrizione, DcVoce oDcVoce, DcDato oDato1, DcSezione oDcSezione, int i, bool bBoldEtichetta, bool bAttributoVisualizzaEtichetta,
                                    Gestore oGestore, RtfFiles rtf, Font f,
                                    Dictionary<string, Font> FontResult, Dictionary<string, Color> ColorResult, Dictionary<string, Size> DimensioniResult,
                                    Dictionary<string, List<string>> OpzioniResult,
                                    Dictionary<string, int> RigheResult,
                                    ref string rtfDatiRilievo, ref string rtfAnteprima)
        {

            if (RigheResult[RtfFiles.RTF_RIGHEPRIMA] > 0)
            {
                for (int a = 1; a <= RigheResult[RtfFiles.RTF_RIGHEPRIMA]; a++)
                {
                    rtfAnteprima = rtf.appendRtfText(rtfAnteprima, @"\line ", f);
                }
            }

            switch (enumTP)
            {

                case DatiClinici.Common.Enums.enumTipoVoce.Testo:
                case DatiClinici.Common.Enums.enumTipoVoce.Numerico:
                case DatiClinici.Common.Enums.enumTipoVoce.Decimale:
                case DatiClinici.Common.Enums.enumTipoVoce.Allegato:
                    if (oDato1.InRilievo)
                    {
                        rtfDatiRilievo = rtf.appendRtfText(rtfDatiRilievo, s_descrizione + oDato1.Value.ToString().Replace(@"\", @"\\") + @"\tab ", f);
                    }
                    rtfAnteprima = rtf.appendRtfText(rtfAnteprima, s_descrizione, FontResult[RtfFiles.RTF_ETICHETTA], ColorResult[RtfFiles.RTF_ETICHETTA]);
                    rtfAnteprima = rtf.appendRtfText(rtfAnteprima, oDato1.Value.ToString().Replace(@"\", @"\\"), FontResult[RtfFiles.RTF_VALORE], ColorResult[RtfFiles.RTF_VALORE]);
                    rtfAnteprima = rtf.appendRtfText(rtfAnteprima, @"\tab", f);
                    break;

                case DatiClinici.Common.Enums.enumTipoVoce.DataOra:
                case DatiClinici.Common.Enums.enumTipoVoce.Ora:
                case DatiClinici.Common.Enums.enumTipoVoce.Data:
                    if (oDato1.InRilievo && oDato1.Value.ToString() != "")
                    {
                        rtfDatiRilievo = rtf.appendRtfText(rtfDatiRilievo, s_descrizione + oDato1.Value + @"\tab ", f);
                    }
                    rtfAnteprima = rtf.appendRtfText(rtfAnteprima, s_descrizione, FontResult[RtfFiles.RTF_ETICHETTA], ColorResult[RtfFiles.RTF_ETICHETTA]);
                    rtfAnteprima = rtf.appendRtfText(rtfAnteprima, oDato1.Value + @"", FontResult[RtfFiles.RTF_VALORE], ColorResult[RtfFiles.RTF_VALORE]);
                    rtfAnteprima = rtf.appendRtfText(rtfAnteprima, @"\tab", f);
                    break;

                case DatiClinici.Common.Enums.enumTipoVoce.Etichetta:
                    bool campiValorizzati = getCampiValorizzati(oDcSezione, oGestore, i);
                    if (campiValorizzati || bAttributoVisualizzaEtichetta)
                    {
                        if (oDato1.InRilievo)
                        {
                            if (bBoldEtichetta == true)
                            {
                                rtfDatiRilievo = rtf.appendRtfText(rtfDatiRilievo, @"\b\i " + oDcVoce.Descrizione.ToString() + @" \i0\b0\tab ", f);
                            }
                            else
                            {
                                rtfDatiRilievo = rtf.appendRtfText(rtfAnteprima, oDcVoce.Descrizione.ToString() + @" ", FontResult[RtfFiles.RTF_ETICHETTA], ColorResult[RtfFiles.RTF_ETICHETTA]);
                                rtfDatiRilievo = rtf.appendRtfText(rtfAnteprima, @"\tab", f);
                            }
                        }
                        if (bBoldEtichetta == true)
                        {
                            rtfAnteprima = rtf.appendRtfText(rtfAnteprima, @"\b\i " + oDcVoce.Descrizione.ToString() + @" \i0\b0\tab ", f);
                        }
                        else
                        {
                            rtfAnteprima = rtf.appendRtfText(rtfAnteprima, oDcVoce.Descrizione.ToString() + @" ", FontResult[RtfFiles.RTF_ETICHETTA], ColorResult[RtfFiles.RTF_ETICHETTA]);
                            rtfAnteprima = rtf.appendRtfText(rtfAnteprima, @"\tab", f);
                        }
                    }
                    break;

                case DatiClinici.Common.Enums.enumTipoVoce.Combo:
                case DatiClinici.Common.Enums.enumTipoVoce.Zoom:
                    string str = oGestore.LeggeTranscodifica(oDato1.Key).ToString();
                    if (oDato1.InRilievo && str != "")
                    {
                        rtfDatiRilievo = rtf.appendRtfText(rtfDatiRilievo, s_descrizione + str.Replace("|", ", ") + @"\tab ", f);
                    }
                    if (str != "")
                    {
                        rtfAnteprima = rtf.appendRtfText(rtfAnteprima, s_descrizione, FontResult[RtfFiles.RTF_ETICHETTA], ColorResult[RtfFiles.RTF_ETICHETTA]);
                        rtfAnteprima = rtf.appendRtfText(rtfAnteprima, str.Replace("|", ", "), FontResult[RtfFiles.RTF_VALORE], ColorResult[RtfFiles.RTF_VALORE]);
                        rtfAnteprima = rtf.appendRtfText(rtfAnteprima, @"\tab", f);
                    }
                    break;

                case DatiClinici.Common.Enums.enumTipoVoce.Multipla:
                case DatiClinici.Common.Enums.enumTipoVoce.ListaSingola:
                    string st = oGestore.LeggeTranscodifica(oDato1.Key).ToString();
                    if (OpzioniResult[RtfFiles.RTF_OPZIONI].Contains(RtfFiles.RTF_ELENCO) == false)
                    {
                        st = st.Replace("|", "; ");
                        if (st != "") st = st.Remove(st.Length - 2, 2);
                        if (oDato1.InRilievo && st != "")
                        {
                            rtfDatiRilievo = rtf.appendRtfText(rtfDatiRilievo, s_descrizione + st + @"\tab ", f);
                        }
                        if (st != "")
                        {
                            rtfAnteprima = rtf.appendRtfText(rtfAnteprima, s_descrizione, FontResult[RtfFiles.RTF_ETICHETTA], ColorResult[RtfFiles.RTF_ETICHETTA]);
                            rtfAnteprima = rtf.appendRtfText(rtfAnteprima, st, FontResult[RtfFiles.RTF_VALORE], ColorResult[RtfFiles.RTF_VALORE]);
                            rtfAnteprima = rtf.appendRtfText(rtfAnteprima, @"\tab", f);
                        }
                    }
                    else
                    {
                        string stinrilievo = "";

                        if (String.IsNullOrEmpty(st) == false)
                        {
                            stinrilievo = st.Replace("|", "; ");

                            if (stinrilievo.Length >= 2)
                                stinrilievo = stinrilievo.Remove(stinrilievo.Length - 2, 2);
                        }

                        if (oDato1.InRilievo && stinrilievo != "")
                        {
                            rtfDatiRilievo = rtf.appendRtfText(rtfDatiRilievo, s_descrizione + stinrilievo + @"\tab ", f);
                        }
                        if (st != "")
                        {
                            rtfAnteprima = rtf.appendRtfText(rtfAnteprima, s_descrizione, FontResult[RtfFiles.RTF_ETICHETTA], ColorResult[RtfFiles.RTF_ETICHETTA]);
                            string[] arvValori = st.Split('|');
                            for (int idx = 0; idx <= arvValori.Length - 1; idx++)
                            {
                                if (arvValori[idx] != string.Empty)
                                {
                                    rtfAnteprima = rtf.appendRtfText(rtfAnteprima, @"\line ", f);
                                    rtfAnteprima = rtf.appendRtfText(rtfAnteprima, @"   ", f);
                                    rtfAnteprima = rtf.appendRtfText(rtfAnteprima, arvValori[idx], FontResult[RtfFiles.RTF_VALORE], ColorResult[RtfFiles.RTF_VALORE]);
                                }
                            }
                            rtfAnteprima = rtf.appendRtfText(rtfAnteprima, @"\tab", f);
                        }
                    }
                    break;

                case DatiClinici.Common.Enums.enumTipoVoce.Immagine:
                    if (oDato1.InRilievo)
                    {
                        rtfDatiRilievo = rtf.appendRtfText(rtfDatiRilievo, s_descrizione, f);
                        rtfDatiRilievo = rtf.appendRtfImage(rtfDatiRilievo, oDato1.Value.ToString());
                        rtfDatiRilievo = rtf.appendRtfText(rtfDatiRilievo, @"\tab", f);
                    }
                    rtfAnteprima = rtf.appendRtfText(rtfAnteprima, s_descrizione, FontResult[RtfFiles.RTF_ETICHETTA], ColorResult[RtfFiles.RTF_ETICHETTA]);
                    rtfAnteprima = rtf.appendRtfImage(rtfAnteprima, oDato1.Value.ToString());
                    rtfAnteprima = rtf.appendRtfText(rtfAnteprima, @"\tab", f);
                    break;

                case DatiClinici.Common.Enums.enumTipoVoce.Logo:
                    if (oDcVoce.Default != "" && oDato1.Abilitato == true)
                    {
                        rtfAnteprima = rtf.appendRtfText(rtfAnteprima, s_descrizione, FontResult[RtfFiles.RTF_ETICHETTA], ColorResult[RtfFiles.RTF_ETICHETTA]);
                        rtfAnteprima = rtf.appendRtfImage(rtfAnteprima, oDcVoce.Default, DimensioniResult[RtfFiles.RTF_ETICHETTA]);
                        rtfAnteprima = rtf.appendRtfText(rtfAnteprima, @"\tab", f);
                    }
                    break;

                case DatiClinici.Common.Enums.enumTipoVoce.TestoRtf:
                    if (oDato1.InRilievo)
                    {
                        rtfDatiRilievo = rtf.appendRtfText(rtfDatiRilievo, s_descrizione, f);
                    }
                    rtfAnteprima = rtf.appendRtfText(rtfAnteprima, s_descrizione, FontResult[RtfFiles.RTF_ETICHETTA], ColorResult[RtfFiles.RTF_ETICHETTA]);
                    if (oDato1.InRilievo)
                    {
                        rtfDatiRilievo = rtf.joinRtf(oDato1.Value.ToString(), rtfDatiRilievo);
                        rtfDatiRilievo = rtf.appendRtfText(rtfDatiRilievo, @" \tab ", f);
                    }
                    rtfAnteprima = rtf.joinRtf(oDato1.Value.ToString(), rtfAnteprima);
                    rtfAnteprima = rtf.appendRtfText(rtfAnteprima, @" \tab ", f);
                    break;

                case DatiClinici.Common.Enums.enumTipoVoce.Domanda:
                    if (oDato1.InRilievo)
                    {
                        if (Convert.ToBoolean(oDato1.Value) == true)
                        {
                            rtfDatiRilievo = rtf.appendRtfText(rtfDatiRilievo, s_descrizione, FontResult[RtfFiles.RTF_ETICHETTA], ColorResult[RtfFiles.RTF_ETICHETTA]);
                            rtfDatiRilievo = rtf.appendRtfText(rtfDatiRilievo, @"Si", FontResult[RtfFiles.RTF_VALORE], ColorResult[RtfFiles.RTF_VALORE]);
                            rtfDatiRilievo = rtf.appendRtfText(rtfDatiRilievo, @"\tab", f);
                        }
                    }
                    if (Convert.ToBoolean(oDato1.Value) == true)
                    {
                        rtfAnteprima = rtf.appendRtfText(rtfAnteprima, s_descrizione, FontResult[RtfFiles.RTF_ETICHETTA], ColorResult[RtfFiles.RTF_ETICHETTA]);
                        rtfAnteprima = rtf.appendRtfText(rtfAnteprima, @"Si", FontResult[RtfFiles.RTF_VALORE], ColorResult[RtfFiles.RTF_VALORE]);
                        rtfAnteprima = rtf.appendRtfText(rtfAnteprima, @"\tab", f);
                    }
                    break;

                case DatiClinici.Common.Enums.enumTipoVoce.Bottone:
                    break;

                case DatiClinici.Common.Enums.enumTipoVoce.Scheda:
                    oGestore.AddGestore(oDato1.Key);
                    MovScheda _MovScheda = new MovScheda("", EnumEntita.SCH, "", "", "", "", this.Ambiente);
                    _MovScheda.GeneraRTF(oGestore.Gestori[oDato1.Key]);
                    if (_MovScheda.DatiRilievoRTF != string.Empty)
                    {
                        rtfDatiRilievo = rtf.joinRtf(rtfDatiRilievo, _MovScheda.DatiRilievoRTF);
                        rtfAnteprima = rtf.appendRtfText(rtfAnteprima, s_descrizione + @"\line ", FontResult[RtfFiles.RTF_ETICHETTA], ColorResult[RtfFiles.RTF_ETICHETTA]);
                    }
                    if (_MovScheda.AnteprimaRTF != string.Empty)
                    {
                        rtfAnteprima = rtf.appendRtfText(rtfAnteprima, s_descrizione + @"\line ", FontResult[RtfFiles.RTF_ETICHETTA], ColorResult[RtfFiles.RTF_ETICHETTA]);
                    }
                    if (oDato1.InRilievo)
                    {
                        rtfDatiRilievo = rtf.joinRtf(_MovScheda.DatiRilievoRTF, rtfDatiRilievo);
                        if (_MovScheda.AnteprimaRTF != string.Empty)
                        {
                            rtfDatiRilievo = rtf.appendRtfText(rtfDatiRilievo, @" \tab ", f);
                        }
                    }
                    rtfAnteprima = rtf.joinRtf(_MovScheda.AnteprimaRTF, rtfAnteprima);
                    if (_MovScheda.AnteprimaRTF != string.Empty)
                    {
                        rtfAnteprima = rtf.appendRtfText(rtfAnteprima, @" \tab ", f);
                    }
                    break;

                case DatiClinici.Common.Enums.enumTipoVoce.Marker:
                    if (oDcVoce.Default != "")
                    {
                        if (oDato1.Value == null) break;

                        if ((oDato1.Value is DcImageElements) || (oDato1.Value is DcMarkers))
                        {
                            if (oDato1.Value is DcImageElements)
                            {
                                DcImageElements tmp = (DcImageElements)oDato1.Value;
                                if ((tmp.Markers.Count == 0) && (tmp.Polylines.Count == 0)) return;
                            }

                            rtfAnteprima = rtf.appendRtfText(rtfAnteprima, s_descrizione, FontResult[RtfFiles.RTF_ETICHETTA], ColorResult[RtfFiles.RTF_ETICHETTA]);

                            Dictionary<string, string> decod = oGestore.DecodificaValori(oDcVoce.Decodifica, false);
                            string imgMarkers = getImageMarkers(oDcVoce.Default, oDato1.Value, decod);

                            rtfAnteprima = rtf.appendRtfImage(rtfAnteprima, imgMarkers, DimensioniResult[RtfFiles.RTF_VALORE]);

                            rtfAnteprima = rtf.appendRtfText(rtfAnteprima, @"\tab", f);
                        }

                    }
                    break;

            }

            if (RigheResult[RtfFiles.RTF_RIGHEDOPO] > 0)
            {
                for (int a = 1; a <= RigheResult[RtfFiles.RTF_RIGHEDOPO]; a++)
                {
                    rtfAnteprima = rtf.appendRtfText(rtfAnteprima, @"\line ", f);
                }
            }

        }

        private string GeneraCampo(enumTipoVoce enumTP, string s_descrizione, DcVoce oDcVoce, DcDato oDato1, Gestore oGestore, Dictionary<string, List<string>> OpzioniResult)
        {

            string sReturn = string.Empty;

            switch (enumTP)
            {

                case DatiClinici.Common.Enums.enumTipoVoce.Testo:
                case DatiClinici.Common.Enums.enumTipoVoce.Numerico:
                case DatiClinici.Common.Enums.enumTipoVoce.Decimale:
                case DatiClinici.Common.Enums.enumTipoVoce.Allegato:
                    sReturn = s_descrizione + oDato1.Value.ToString();
                    break;

                case DatiClinici.Common.Enums.enumTipoVoce.DataOra:
                case DatiClinici.Common.Enums.enumTipoVoce.Ora:
                case DatiClinici.Common.Enums.enumTipoVoce.Data:
                    sReturn = s_descrizione + oDato1.Value.ToString();
                    break;

                case DatiClinici.Common.Enums.enumTipoVoce.Etichetta:
                    sReturn = oDcVoce.Descrizione.ToString();
                    break;

                case DatiClinici.Common.Enums.enumTipoVoce.Combo:
                case DatiClinici.Common.Enums.enumTipoVoce.Zoom:
                    string str = oGestore.LeggeTranscodifica(oDato1.Key).ToString();
                    sReturn = s_descrizione + str.Replace("|", ", ");
                    break;

                case DatiClinici.Common.Enums.enumTipoVoce.Multipla:
                case DatiClinici.Common.Enums.enumTipoVoce.ListaSingola:
                    string st = oGestore.LeggeTranscodifica(oDato1.Key).ToString();
                    if (OpzioniResult != null && OpzioniResult[RtfFiles.RTF_OPZIONI].Contains(RtfFiles.RTF_ELENCO) == true)
                    {
                        if (st != "")
                        {
                            string[] arvValori = st.Split('|');
                            st = string.Empty;
                            for (int idx = 0; idx <= arvValori.Length - 1; idx++)
                            {
                                if (arvValori[idx] != string.Empty)
                                {
                                    if (idx != 0)
                                    {
                                        st += Environment.NewLine + " ";
                                    };
                                    st += arvValori[idx];
                                }
                            }
                            if (oDato1.InRilievo == true)
                            {
                                st += Environment.NewLine;
                            }
                        }
                        sReturn = s_descrizione + st;
                    }
                    else
                    {
                        st = st.Replace("|", "; ");
                        st = st.Remove(st.Length - 2, 2);
                        sReturn = s_descrizione + st;
                    }
                    break;

                case DatiClinici.Common.Enums.enumTipoVoce.Immagine:
                    sReturn = s_descrizione;
                    break;

                case DatiClinici.Common.Enums.enumTipoVoce.Logo:
                    sReturn = s_descrizione;
                    break;

                case DatiClinici.Common.Enums.enumTipoVoce.TestoRtf:
                    sReturn = s_descrizione;
                    break;

                case DatiClinici.Common.Enums.enumTipoVoce.Domanda:
                    if (Convert.ToBoolean(oDato1.Value) == true)
                    {
                        sReturn = s_descrizione + @"Si";
                    }
                    break;

                case DatiClinici.Common.Enums.enumTipoVoce.Bottone:
                    break;

                case DatiClinici.Common.Enums.enumTipoVoce.Scheda:
                    sReturn = s_descrizione;
                    break;

                case DatiClinici.Common.Enums.enumTipoVoce.Marker:
                    if (oDcVoce.Default != "")
                    {
                        if (oDato1.Value.GetType() == typeof(DcMarkers) && ((DcMarkers)oDato1.Value).Count > 0)
                        {
                            sReturn = s_descrizione + getImageMarkers(oDcVoce.Default, oDato1.Value, oGestore.DecodificaValori(oDcVoce.Decodifica, false));
                        }
                    }
                    break;

            }

            if (oDato1.InRilievo == true)
            {
                sReturn += @" (*)";
            }

            return sReturn;

        }

        private bool getCampiValorizzati(DcSezione oDcSezione, Gestore oGestore, int i)
        {

            bool campiValorizzati = false;

            foreach (DcVoce oDcVoceEtichetta in oDcSezione.Voci.Values)
            {
                DcDato oDatoEtichetta = oGestore.LeggeDato(oDcVoceEtichetta.Key, i);

                if (oGestore.SchedaLayouts.Layouts[oDcVoceEtichetta.Key].TipoVoce != DatiClinici.Common.Enums.enumTipoVoce.Etichetta)
                {
                    if (oDatoEtichetta != null && oDatoEtichetta.Value.ToString() != "" && oDatoEtichetta.Value != null)
                    {

                        if (oGestore.SchedaLayouts.Layouts[oDcVoceEtichetta.Key].TipoVoce != enumTipoVoce.Domanda)
                        {
                            campiValorizzati = true;
                        }
                        else
                        {
                            if (Convert.ToBoolean(oDatoEtichetta.Value) == true)
                            {
                                campiValorizzati = true;
                            }
                        }

                    }
                }
            }

            return campiValorizzati;

        }

        private string getImageMarkers(string sDefault, object data, Dictionary<string, string> decodifichevalori)
        {

            const int npersfondo = 50;
            const int nperfont = 20;

            string sReturn = sDefault;

            DcImageElements dataObject = null;

            if (data is DcMarkers)
            {
                dataObject = new DcImageElements();
                DcMarkers marks = (DcMarkers)data;
                dataObject.Polylines = new List<DcPolyline>();
                dataObject.Markers = marks;
            }
            else
                dataObject = (DcImageElements)data;

            try
            {

                MemoryStream ms = new MemoryStream(Convert.FromBase64String(sDefault));
                Image image = Image.FromStream(ms);

                if (dataObject.Markers != null)
                {
                    using (Graphics g = Graphics.FromImage(image))
                    {

                        foreach (DcMarker oMarker in dataObject.Markers.Values)
                        {

                            oMarker.X = oMarker.X * image.Width;
                            oMarker.Y = oMarker.Y * image.Height;

                            MemoryStream newms = new MemoryStream(Convert.FromBase64String(decodifichevalori[oMarker.ID]));
                            Image newimage = Image.FromStream(newms);

                            g.DrawImage(newimage, new System.Drawing.Rectangle((int)oMarker.X, (int)oMarker.Y, newimage.Width, newimage.Height));

                            SolidBrush whiteBrush = new SolidBrush(Color.White);
                            g.FillRectangle(whiteBrush, new System.Drawing.Rectangle((int)oMarker.X, (int)oMarker.Y, (int)((double)newimage.Width / 100 * npersfondo), (int)((double)newimage.Height / 100 * npersfondo)));

                            float size = (float)(((int)((double)newimage.Width / 100 * nperfont)) * (96.0 / 72.0));
                            g.DrawString(oMarker.Numero.ToString(), new Font("Calibri", size, FontStyle.Regular), Brushes.Black, new System.Drawing.Rectangle((int)oMarker.X, (int)oMarker.Y, (int)((double)newimage.Width / 100 * npersfondo), (int)((double)newimage.Height / 100 * npersfondo)));

                            newimage.Dispose();
                            newimage = null;
                            newms.Close();
                            newms.Dispose();

                        }

                    }

                }

                if (dataObject.Polylines != null)
                {
                    foreach (DcPolyline poly in dataObject.Polylines)
                    {
                        this.drawPolylineOnImage(image, poly);
                    }
                }

                sReturn = Serializer.ImageToBase64(image, System.Drawing.Imaging.ImageFormat.Png);

                image.Dispose();
                image = null;

                ms.Close();
                ms.Dispose();

            }
            catch (Exception)
            {
                sReturn = sDefault;
            }

            return sReturn;

        }

        private void drawPolylineOnImage(Image image, DcPolyline polyline)
        {
            using (Graphics g = Graphics.FromImage(image))
            {
                PointF[] gp = polyline.GdiPoints(image.Width, image.Height);
                Pen pen = polyline.GdiPen();

                g.DrawLines(pen, gp);

                pen.Dispose();
            }

        }

        public bool Salva()
        {
            return this.Salva(true);
        }

        public bool Salva(bool RicaricaMovimento)
        {
            return this.Salva(RicaricaMovimento, true);
        }

        public bool Salva(bool RicaricaMovimento, bool FlagGeneraRTF)
        {

            bool bReturn = true;
            Parametri op = null;
            SqlParameterExt[] spcoll;
            string xmlParam = "";

            try
            {

                if (_idmovscheda == string.Empty || _idmovscheda.Trim() == "")
                {
                    if (this.Scheda == null)
                        bReturn = false;
                    else
                    {
                        op = new Parametri(this.Ambiente);
                        op.Parametro.Add("CodUA", _codua);
                        op.Parametro.Add("CodEntita", _codentita);
                        op.Parametro.Add("IDEntita", _identita);
                        op.Parametro.Add("IDPaziente", _idpaziente);
                        op.Parametro.Add("IDEpisodio", _idepisodio);
                        op.Parametro.Add("IDTrasferimento", _idtrasferimento);
                        op.Parametro.Add("IDSchedaPadre", _idschedapadre);

                        op.Parametro.Add("CodScheda", this.Scheda.Codice);
                        op.Parametro.Add("Versione", this.Scheda.Versione.ToString());

                        op.Parametro.Add("Dati", this.DatiXML);
                        if (FlagGeneraRTF == true)
                        {
                            GeneraRTF();
                        }
                        op.Parametro.Add("CodStatoSchedaCalcolato", _codstatoschedacalcolato);
                        op.Parametro.Add("AnteprimaRTF", this.AnteprimaRTFxParamStored);
                        op.Parametro.Add("AnteprimaTXT", this.AnteprimaTXTxParamStored);
                        op.Parametro.Add("DatiObbligatoriMancantiRTF", this.DatiObbligatoriMancantiRTFxParamStored);
                        op.Parametro.Add("DatiRilievoRTF", this.DatiRilievoRTFxParamStored);
                        op.Parametro.Add("InEvidenza", System.Convert.ToInt16(this.InEvidenza).ToString());
                        op.Parametro.Add("IDCartellaAmbulatoriale", _idcartellaambulatoriale);

                        op.TimeStamp.CodEntita = EnumEntita.SCH.ToString();
                        op.TimeStamp.CodAzione = EnumAzioni.INS.ToString();

                        spcoll = new SqlParameterExt[1];

                        xmlParam = XmlProcs.XmlSerializeToString(op);
                        xmlParam = XmlProcs.CheckXMLDati(xmlParam);
                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                        DataTable dt = Database.GetDataTableStoredProc("MSP_InsMovSchede", spcoll);
                        if (dt != null && dt.Rows.Count > 0 && !dt.Rows[0].IsNull(0))
                        {
                            _idmovscheda = dt.Rows[0][0].ToString();
                            SalvaAllegati(RicaricaMovimento);

                            if (RicaricaMovimento == true || _addpin == true) { Carica(_idmovscheda); }
                            if (_addpin == true) { this.AggiungiSegnalibro(); }
                        }
                        else
                        {
                            bReturn = false;
                        }
                    }
                }
                else
                {
                    op = new Parametri(this.Ambiente);
                    op.Parametro.Add("IDScheda", _idmovscheda);
                    op.Parametro.Add("CodUA", _codua);
                    op.Parametro.Add("CodEntita", _codentita);
                    op.Parametro.Add("IDEntita", _identita);
                    op.Parametro.Add("IDPaziente", _idpaziente);
                    op.Parametro.Add("IDEpisodio", _idepisodio);
                    op.Parametro.Add("IDTrasferimento", _idtrasferimento);

                    op.Parametro.Add("CodScheda", this.Scheda.Codice);
                    op.Parametro.Add("Versione", this.Scheda.Versione.ToString());
                    op.Parametro.Add("Numero", _numero.ToString());

                    op.Parametro.Add("Dati", this.DatiXML);
                    op.Parametro.Add("InEvidenza", System.Convert.ToInt16(this.InEvidenza).ToString());

                    if (FlagGeneraRTF == true)
                    {
                        GeneraRTF();
                    }


                    if (_azione == EnumAzioni.CAN)
                    {
                        op.Parametro.Add("CodStatoScheda", "CA");
                        foreach (MovSchedaAllegato oMsa in this.Allegati)
                        {
                            oMsa.Azione = Scci.Enums.EnumAzioni.ANN;
                        }
                    }
                    else if (_azione == EnumAzioni.ANN)
                    {
                        op.Parametro.Add("CodStatoScheda", "AN");
                        if (this.AnteprimaRTF != string.Empty && this.AnteprimaRTF.Trim() != "" && _codentita != EnumEntita.WKI.ToString())
                        {
                            RtfFiles rtf = new RtfFiles();
                            System.Drawing.Font f = rtf.getFontFromString(Database.GetConfigTable(EnumConfigTable.FontPredefinitoRTF), false, false);

                            this.AnteprimaRTF = rtf.BarraTestoRTF(this.AnteprimaRTF);

                        }
                        foreach (MovSchedaAllegato oMsa in this.Allegati)
                        {
                            oMsa.Azione = Scci.Enums.EnumAzioni.ANN;
                        }
                    }
                    else
                    {
                        op.Parametro.Add("CodStatoScheda", _codstatoscheda);
                    }

                    op.Parametro.Add("CodStatoSchedaCalcolato", _codstatoschedacalcolato);
                    op.Parametro.Add("AnteprimaRTF", this.AnteprimaRTFxParamStored);
                    op.Parametro.Add("AnteprimaTXT", this.AnteprimaTXTxParamStored);
                    op.Parametro.Add("DatiObbligatoriMancantiRTF", this.DatiObbligatoriMancantiRTFxParamStored);
                    op.Parametro.Add("DatiRilievoRTF", this.DatiRilievoRTFxParamStored);

                    op.TimeStamp.CodEntita = EnumEntita.SCH.ToString();
                    op.TimeStamp.IDEntita = _idmovscheda;
                    op.TimeStamp.CodAzione = _azione.ToString();

                    spcoll = new SqlParameterExt[1];

                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    xmlParam = XmlProcs.CheckXMLDati(xmlParam);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    DataTable dt = Database.GetDataTableStoredProc("MSP_AggMovSchede", spcoll);

                    SalvaAllegati(RicaricaMovimento);

                    if (RicaricaMovimento) Carica(_idmovscheda);

                }

            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovScheda.Salva()" + Environment.NewLine + ex.Message, ex);
            }

            return bReturn;

        }

        public bool Cancella()
        {

            try
            {
                _codstatoscheda = "CA";
                Azione = EnumAzioni.CAN;

                if (Salva())
                {
                    Azione = EnumAzioni.MOD;
                    Carica(_idmovscheda);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public bool CopiaDaOrigine(MovScheda movschorigine, int numero)
        {
            return CopiaDaOrigine(movschorigine, numero, false);
        }
        public bool CopiaDaOrigine(MovScheda movschorigine, int numero, bool bcheckcampischeda, bool bcopiasempre = false)
        {
            bool bReturn = true;

            try
            {

                if (this.Scheda == null)
                    bReturn = false;
                else
                {

                    _codua = movschorigine.CodUA;
                    _codentita = movschorigine.CodEntita;
                    _identita = movschorigine.IDEntita;
                    _idpaziente = movschorigine.IDPaziente;
                    _idepisodio = movschorigine.IDEpisodio;
                    _idtrasferimento = movschorigine.IDTrasferimento;
                    _codscheda = movschorigine.CodScheda;
                    _codstatoscheda = Enum.GetName(typeof(EnumStatoScheda), EnumStatoScheda.IC);
                    _versione = movschorigine.Versione;
                    _numero = numero;
                    if (bcheckcampischeda)
                    {
                        _datixml = getNuoviDatiXML(this, movschorigine, bcopiasempre);
                    }
                    else
                    {
                        _datixml = movschorigine.DatiXML;
                    }
                    _anteprimartf = movschorigine.AnteprimaRTF;
                    _datiobbligatorimancantirtf = movschorigine.DatiObbligatoriMancantiRTF;
                    _datirilievortf = movschorigine.DatiRilievoRTF;
                    _idschedapadre = movschorigine.IDSchedaPadre;
                    _storicizzata = false;
                    _datacreazione = DateTime.Now;
                    _dataultimamodifica = DateTime.Now;
                    _codutenteultimamodifica = this.Ambiente.Codlogin;
                    _azione = EnumAzioni.INS;
                }

            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovScheda.CopiaDaOrigine()" + Environment.NewLine + ex.Message, ex);
            }

            return bReturn;
        }

        public bool CopiaDaOriginePerConversione(MovScheda movschorigine, int numero)
        {

            bool bReturn = true;

            try
            {

                if (this.Scheda == null)
                    bReturn = false;
                else
                {

                    _codua = movschorigine.CodUA;
                    _codentita = movschorigine.CodEntita;
                    _identita = movschorigine.IDEntita;
                    _idpaziente = movschorigine.IDPaziente;
                    _idepisodio = movschorigine.IDEpisodio;
                    _idtrasferimento = movschorigine.IDTrasferimento;
                    _codscheda = movschorigine.CodScheda;
                    _codstatoscheda = Enum.GetName(typeof(EnumStatoScheda), EnumStatoScheda.IC);
                    _versione = movschorigine.Versione;
                    _numero = numero;
                    _datixml = getNuoviDatiXMLPerConversione(this, movschorigine);
                    _anteprimartf = movschorigine.AnteprimaRTF;
                    _datiobbligatorimancantirtf = movschorigine.DatiObbligatoriMancantiRTF;
                    _datirilievortf = movschorigine.DatiRilievoRTF;
                    _idschedapadre = movschorigine.IDSchedaPadre;
                    _storicizzata = false;
                    _datacreazione = DateTime.Now;
                    _dataultimamodifica = DateTime.Now;
                    _codutenteultimamodifica = this.Ambiente.Codlogin;
                    _azione = EnumAzioni.INS;
                }

            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovScheda.CopiaPerConversione()" + Environment.NewLine + ex.Message, ex);
            }

            return bReturn;

        }

        public string Normalizza()
        {

            string sreturn = string.Empty;

            Gestore oGestore = null;

            try
            {

                oGestore = CommonStatics.GetGestore(this.Ambiente);
                oGestore.SchedaLayoutsXML = this.Scheda.LayoutXML;
                oGestore.Decodifiche = this.Scheda.DizionarioValori();
                oGestore.SchedaXML = this.Scheda.StrutturaXML;

                this.FlagCreaRtfOnSet = false;
                oGestore.SchedaDatiXML = this.DatiXML;
                this.FlagCreaRtfOnSet = true;

                sreturn = oGestore.Normalizza();

            }
            catch (Exception ex)
            {
                throw new Exception(@"MovScheda.Normalizzazione()" + Environment.NewLine + ex.Message, ex);
            }
            finally
            {
                oGestore = null;
            }

            return sreturn;

        }

        public MovScheda Clona()
        {
            try
            {
                EnumEntita ee = (EnumEntita)Enum.Parse(typeof(EnumEntita), this.CodEntita.Trim());

                MovScheda ret = new MovScheda(this.CodScheda,
                                              ee,
                                              this.CodUA,
                                              this.IDPaziente,
                                              this.IDEpisodio,
                                              this.IDTrasferimento,
                                              this.Ambiente);
                ret.IDEntita = this.IDEntita;
                ret.IDSchedaPadre = this.IDSchedaPadre;

                ret.CodStatoScheda = this.CodStatoScheda;
                ret.CodStatoSchedaCalcolato = this.CodStatoSchedaCalcolato;
                ret.DatiObbligatoriMancantiRTF = this.DatiObbligatoriMancantiRTF;
                ret.DatiRilievoRTF = this.DatiRilievoRTF;
                ret.DatiXML = this.DatiXML;

                ret.IDMovScheda = this.IDMovScheda;
                ret.IDPin = this.IDPin;
                ret.IDTrasferimento = this.IDTrasferimento;
                ret.InEvidenza = this.InEvidenza;

                ret.AnteprimaRTF = this.AnteprimaRTF;

                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception(@"MovScheda.Clona()" + Environment.NewLine + ex.Message, ex);
            }

        }

        private void Carica(string idmovscheda)
        {
            try
            {

                Parametri op = new Parametri(this.Ambiente);
                op.Parametro.Add("IDScheda", idmovscheda);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovSchedaBase", spcoll);

                if (dt.Rows.Count > 0)
                {
                    _scheda = null;

                    _idmovscheda = dt.Rows[0]["ID"].ToString();
                    _codua = dt.Rows[0]["CodUA"].ToString();
                    _codentita = dt.Rows[0]["CodEntita"].ToString();
                    _identita = dt.Rows[0]["IDEntita"].ToString();
                    _idpaziente = dt.Rows[0]["IDPaziente"].ToString();
                    _idepisodio = dt.Rows[0]["IDEpisodio"].ToString();
                    _idtrasferimento = dt.Rows[0]["IDTrasferimento"].ToString();
                    _codstatoscheda = dt.Rows[0]["CodStatoScheda"].ToString();
                    _codstatoschedacalcolato = dt.Rows[0]["CodStatoSchedaCalcolato"].ToString();
                    _codscheda = dt.Rows[0]["CodScheda"].ToString();
                    _versione = (int)dt.Rows[0]["Versione"];
                    _numero = (int)dt.Rows[0]["Numero"];
                    _datixml = this.UnescapeXML(dt.Rows[0]["Dati"].ToString());
                    _anteprimartf = dt.Rows[0]["AnteprimaRTF"].ToString();
                    _anteprimatxt = dt.Rows[0]["AnteprimaTXT"].ToString();
                    _datiobbligatorimancantirtf = dt.Rows[0]["DatiObbligatoriMancantiRTF"].ToString();
                    this.LenDatiObbligatoriMancantiRTF = (long)dt.Rows[0]["LenDatiObbligatoriMancantiRTF"];
                    _datirilievortf = dt.Rows[0]["DatiRilievoRTF"].ToString();
                    _idschedapadre = dt.Rows[0]["IDSchedaPadre"].ToString();
                    _storicizzata = (bool)dt.Rows[0]["Storicizzata"];
                    _datacreazione = dt.Rows[0]["DataCreazione"] != DBNull.Value ? (DateTime)dt.Rows[0]["DataCreazione"] : DateTime.MinValue;
                    _dataultimamodifica = dt.Rows[0]["DataUltimaModifica"] != DBNull.Value ? (DateTime)dt.Rows[0]["DataUltimaModifica"] : DateTime.MinValue;
                    _codutenterilevazione = dt.Rows[0]["CodUtenteRilevazione"].ToString();
                    _codutenteultimamodifica = dt.Rows[0]["CodUtenteUltimaModifica"].ToString();
                    _descrscheda = dt.Rows[0]["DescrScheda"].ToString();
                    _descrunitaatomica = dt.Rows[0]["DescrUnitaAtomica"].ToString();
                    _descrutenteultimamodifica = dt.Rows[0]["DescrUtenteUltimaModifica"].ToString();
                    _azione = EnumAzioni.MOD;
                    _inevidenza = dt.Rows[0]["InEvidenza"] != DBNull.Value ? (bool)dt.Rows[0]["InEvidenza"] : false;
                    _validabile = dt.Rows[0]["Validabile"] != DBNull.Value ? (bool)dt.Rows[0]["Validabile"] : false;
                    _validata = dt.Rows[0]["Validata"] != DBNull.Value ? (bool)dt.Rows[0]["Validata"] : false;
                    _validatanew = dt.Rows[0]["Validata"] != DBNull.Value ? (bool)dt.Rows[0]["Validata"] : false;
                    if (dt.Columns.Contains("PermessoUAFirma") && !dt.Rows[0].IsNull("PermessoUAFirma")) _permessoUAFirma = (int)dt.Rows[0]["PermessoUAFirma"];
                    _cartellaambulatorialecodificata = dt.Rows[0]["CartellaAmbulatorialeCodificata"] != DBNull.Value ? (bool)dt.Rows[0]["CartellaAmbulatorialeCodificata"] : false;
                    _idcartellaambulatoriale = dt.Rows[0]["IDCartellaAmbulatoriale"].ToString();

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        private void Carica(string codentita, string identita)
        {
            this.Carica(codentita, identita, 1);
        }
        private void Carica(string codentita, string identita, int numero)
        {
            try
            {

                Parametri op = new Parametri(this.Ambiente);
                op.Parametro.Add("CodEntita", codentita);
                op.Parametro.Add("IDEntita", identita);
                op.Parametro.Add("Numero", numero.ToString());
                op.Parametro.Add("Storicizzata", "NO");

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovSchedaBase", spcoll);

                if (dt.Rows.Count > 0)
                {
                    _scheda = null;

                    _idmovscheda = dt.Rows[0]["ID"].ToString();
                    _codua = dt.Rows[0]["CodUA"].ToString();
                    _codentita = dt.Rows[0]["CodEntita"].ToString();
                    _identita = dt.Rows[0]["IDEntita"].ToString();
                    _idpaziente = dt.Rows[0]["IDPaziente"].ToString();
                    _idepisodio = dt.Rows[0]["IDEpisodio"].ToString();
                    _idtrasferimento = dt.Rows[0]["IDTrasferimento"].ToString();
                    _codstatoscheda = dt.Rows[0]["CodStatoScheda"].ToString();
                    _codstatoschedacalcolato = dt.Rows[0]["CodStatoSchedaCalcolato"].ToString();
                    _codscheda = dt.Rows[0]["CodScheda"].ToString();
                    _versione = (int)dt.Rows[0]["Versione"];
                    _numero = (int)dt.Rows[0]["Numero"];
                    _datixml = this.UnescapeXML(dt.Rows[0]["Dati"].ToString());
                    _anteprimartf = dt.Rows[0]["AnteprimaRTF"].ToString();
                    _anteprimatxt = dt.Rows[0]["AnteprimaTXT"].ToString();
                    _datiobbligatorimancantirtf = dt.Rows[0]["DatiObbligatoriMancantiRTF"].ToString();
                    _datirilievortf = dt.Rows[0]["DatiRilievoRTF"].ToString();
                    _idschedapadre = dt.Rows[0]["IDSchedaPadre"].ToString();
                    _storicizzata = (bool)dt.Rows[0]["Storicizzata"];
                    _datacreazione = dt.Rows[0]["DataCreazione"] != DBNull.Value ? (DateTime)dt.Rows[0]["DataCreazione"] : DateTime.MinValue;
                    _dataultimamodifica = dt.Rows[0]["DataUltimaModifica"] != DBNull.Value ? (DateTime)dt.Rows[0]["DataUltimaModifica"] : DateTime.MinValue;
                    _codutenteultimamodifica = dt.Rows[0]["CodUtenteUltimaModifica"].ToString();
                    _descrscheda = dt.Rows[0]["DescrScheda"].ToString();
                    _descrunitaatomica = dt.Rows[0]["DescrUnitaAtomica"].ToString();
                    _descrutenteultimamodifica = dt.Rows[0]["DescrUtenteUltimaModifica"].ToString();
                    _azione = EnumAzioni.MOD;
                    _inevidenza = dt.Rows[0]["InEvidenza"] != DBNull.Value ? (bool)dt.Rows[0]["InEvidenza"] : false;
                    _validabile = dt.Rows[0]["Validabile"] != DBNull.Value ? (bool)dt.Rows[0]["Validabile"] : false;
                    _validata = dt.Rows[0]["Validata"] != DBNull.Value ? (bool)dt.Rows[0]["Validata"] : false;
                    _validatanew = dt.Rows[0]["Validata"] != DBNull.Value ? (bool)dt.Rows[0]["Validata"] : false;
                    _cartellaambulatorialecodificata = dt.Rows[0]["CartellaAmbulatorialeCodificata"] != DBNull.Value ? (bool)dt.Rows[0]["CartellaAmbulatorialeCodificata"] : false;
                    _idcartellaambulatoriale = dt.Rows[0]["IDCartellaAmbulatoriale"].ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private void CaricaAllegati(EnumTipoRichiestaAllegatoScheda tiporichiesta)
        {
            try
            {

                _movschedaallegati = new List<MovSchedaAllegato>();
                if (_idmovscheda != null && _idmovscheda != string.Empty && _idmovscheda.Trim() != "")
                {
                    Parametri op = new Parametri(this.Ambiente);
                    op.Parametro.Add("IDScheda", _idmovscheda);
                    op.Parametro.Add("TipoRichiesta", EnumTipoRichiestaAllegatoScheda.LISTA.ToString());

                    SqlParameterExt[] spcoll = new SqlParameterExt[1];

                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovSchedaAllegati", spcoll);

                    if (dt != null)
                    {
                        for (int a = 0; a < dt.Rows.Count; a++)
                        {
                            MovSchedaAllegato all = new MovSchedaAllegato(dt.Rows[a]["ID"].ToString(), tiporichiesta, this.Ambiente);
                            _movschedaallegati.Add(all);
                        }

                        dt.Dispose();
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void SalvaAllegati(bool RicaricaMovimento)
        {
            try
            {
                if (this.Allegati != null && this.Allegati.Count > 0)
                {
                    Gestore oGestore = new Gestore();
                    oGestore.SchedaDatiXML = this.DatiXML;
                    if (this.MovOperazioni.Count > 0)
                    {
                        foreach (List<string> oMo in this.MovOperazioni)
                        {
                            switch ((EnumAzioni)Enum.Parse(typeof(EnumAzioni), oMo[2]))
                            {

                                case EnumAzioni.INS:
                                    foreach (MovSchedaAllegato all in _movschedaallegati)
                                    {
                                        if (oMo[0] == all.CodSezione)
                                        {
                                            if (all.SequenzaNuova >= int.Parse(oMo[1]) && all.Azione != EnumAzioni.ANN)
                                            {
                                                all.SequenzaNuova += 1;
                                                Array _arcampo = all.CodCampo.Split('_');
                                                all.CodCampoNuovo = string.Format("{0}_{1}", _arcampo.GetValue(0), all.SequenzaNuova.ToString());
                                                all.Azione = EnumAzioni.INS;
                                            }
                                        }
                                    }
                                    break;

                                case EnumAzioni.CAN:
                                    foreach (MovSchedaAllegato all in _movschedaallegati)
                                    {
                                        if (oMo[0] == all.CodSezione)
                                        {
                                            if (all.SequenzaNuova == int.Parse(oMo[1]) && all.Azione == EnumAzioni.MOD)
                                            {
                                                all.Azione = EnumAzioni.ANN;
                                            }
                                            else if (all.SequenzaNuova > int.Parse(oMo[1]) && all.Azione != EnumAzioni.ANN)
                                            {
                                                all.SequenzaNuova -= 1;
                                                Array _arcampo = all.CodCampo.Split('_');
                                                all.CodCampoNuovo = string.Format("{0}_{1}", _arcampo.GetValue(0), all.SequenzaNuova.ToString());
                                            }
                                        }
                                    }
                                    break;

                            }
                        }
                    }
                    foreach (MovSchedaAllegato all in _movschedaallegati)
                    {
                        DcDato oDato = oGestore.LeggeDato(all.CodCampoNuovo);
                        if (oDato == null || (oDato != null && oDato.Value.ToString() == string.Empty) && all.Azione == EnumAzioni.MOD)
                        {
                            if (all.IDMovSchedaAllegato != string.Empty)
                            {
                                all.Azione = EnumAzioni.ANN;
                                all.Salva(RicaricaMovimento);
                            }
                        }
                        else
                        {
                            if (all.Azione == EnumAzioni.INS)
                            {
                                all.IDMovScheda = this.IDMovScheda;
                                all.CodCampo = all.CodCampoNuovo;
                                all.Sequenza = all.SequenzaNuova;
                                all.Salva(RicaricaMovimento);
                            }
                            else if (all.Azione == EnumAzioni.ANN)
                            {
                                all.Salva(RicaricaMovimento);
                            }
                            else if (all.Azione == EnumAzioni.MOD)
                            {
                                all.CodCampo = all.CodCampoNuovo;
                                all.Sequenza = all.SequenzaNuova;
                                all.Salva(RicaricaMovimento);
                            }
                        }
                    }
                    oGestore = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string UnescapeXML(string s)
        {

            string returnString = s;

            if (!string.IsNullOrEmpty(s) || s.Contains('&'))
            {
                returnString = returnString.Replace("&apos;", "'");
                returnString = returnString.Replace("&quot;", "\"");
                returnString = returnString.Replace("&gt;", ">");
                returnString = returnString.Replace("&lt;", "<");
                returnString = returnString.Replace("&amp;", "&");
            }

            return returnString;

        }
        private string getNuoviDatiXML(MovScheda movScheda, MovScheda oMovSchedaPrecedente, bool bcopiasempre = false)
        {

            string sret = oMovSchedaPrecedente.DatiXML;

            try
            {

                Gestore oGestorePrecedente = CommonStatics.GetGestore(this.Ambiente);
                oGestorePrecedente.SchedaXML = oMovSchedaPrecedente.Scheda.StrutturaXML;
                oGestorePrecedente.SchedaLayoutsXML = oMovSchedaPrecedente.Scheda.LayoutXML;
                oGestorePrecedente.Decodifiche = oMovSchedaPrecedente.Scheda.DizionarioValori();
                oGestorePrecedente.SchedaDatiXML = oMovSchedaPrecedente.DatiXML;

                Gestore oGestoreSelezionata = CommonStatics.GetGestore(this.Ambiente);
                oGestoreSelezionata.SchedaXML = movScheda.Scheda.StrutturaXML;
                oGestoreSelezionata.SchedaLayoutsXML = movScheda.Scheda.LayoutXML;
                oGestoreSelezionata.Decodifiche = movScheda.Scheda.DizionarioValori();

                if (movScheda.DatiXML != string.Empty)
                {
                    oGestoreSelezionata.SchedaDatiXML = movScheda.DatiXML;
                }
                oGestoreSelezionata.NuovaScheda();

                Gestore oGestoreCiclo = CommonStatics.GetGestore(this.Ambiente);
                oGestoreCiclo.SchedaXML = movScheda.Scheda.StrutturaXML;
                oGestoreCiclo.SchedaLayoutsXML = movScheda.Scheda.LayoutXML;
                oGestoreCiclo.Decodifiche = movScheda.Scheda.DizionarioValori();

                if (movScheda.DatiXML != string.Empty)
                {
                    oGestoreCiclo.SchedaDatiXML = movScheda.DatiXML;
                }

                oGestoreCiclo.NuovaScheda();

                foreach (DcDato oDcDatoSelezionata in oGestoreCiclo.SchedaDati.Dati.Values)
                {

                    DcVoce oDcVoceSelezionata = oGestoreSelezionata.LeggeVoce(oDcDatoSelezionata.ID);
                    DcVoce oDcVocePrecedente = oGestorePrecedente.LeggeVoce(oDcDatoSelezionata.ID);
                    if (oDcVoceSelezionata != null && oDcVocePrecedente != null)
                    {

                        bool bRipetibileSelezionata = this.CheckSezioneRipetibile(oGestoreSelezionata.Scheda.Sezioni[oDcVoceSelezionata.Padre.Key]);
                        bool bRipetibilePrecedente = this.CheckSezioneRipetibile(oGestorePrecedente.Scheda.Sezioni[oDcVocePrecedente.Padre.Key]);

                        if (bRipetibileSelezionata == false && bRipetibilePrecedente == false)
                        {

                            DcDato oDcDatoPrecedente = oGestorePrecedente.LeggeDato(oDcDatoSelezionata.Key);
                            if (
                                oDcDatoPrecedente != null
                                && oDcDatoPrecedente.Value.ToString() != string.Empty

                                && (oDcVoceSelezionata.Default == string.Empty || bcopiasempre)

                                && (oDcDatoSelezionata.Value.ToString() == string.Empty || oDcVoceSelezionata.Formato == enumFormatoVoce.Booleano)

                                && oDcVoceSelezionata.Formato == oDcVocePrecedente.Formato

                                && (oGestoreSelezionata.SchedaLayouts.Layouts[oDcVoceSelezionata.ID].TipoVoce == oGestorePrecedente.SchedaLayouts.Layouts[oDcVocePrecedente.ID].TipoVoce
                                || (oGestoreSelezionata.SchedaLayouts.Layouts[oDcVoceSelezionata.ID].TipoVoce == enumTipoVoce.TestoRtf
                                    && oGestorePrecedente.SchedaLayouts.Layouts[oDcVocePrecedente.ID].TipoVoce == enumTipoVoce.Testo)
                                || (oGestoreSelezionata.SchedaLayouts.Layouts[oDcVoceSelezionata.ID].TipoVoce == enumTipoVoce.Testo
                                    && oGestorePrecedente.SchedaLayouts.Layouts[oDcVocePrecedente.ID].TipoVoce == enumTipoVoce.TestoRtf)
                                    )
                                )
                            {
                                if (oGestoreSelezionata.SchedaLayouts.Layouts[oDcVoceSelezionata.ID].TipoVoce == enumTipoVoce.TestoRtf
                                    && oGestorePrecedente.SchedaLayouts.Layouts[oDcVocePrecedente.ID].TipoVoce == enumTipoVoce.Testo)
                                {
                                    oGestoreSelezionata.ModificaValore(oDcDatoSelezionata.Key, getTestoToTestoRtf(oDcDatoPrecedente.Value));
                                }
                                else if (oGestoreSelezionata.SchedaLayouts.Layouts[oDcVoceSelezionata.ID].TipoVoce == enumTipoVoce.Testo
                                    && oGestorePrecedente.SchedaLayouts.Layouts[oDcVocePrecedente.ID].TipoVoce == enumTipoVoce.TestoRtf)
                                {
                                    oGestoreSelezionata.ModificaValore(oDcDatoSelezionata.Key, getTestoRtfToTesto(oDcDatoPrecedente.Value));
                                }
                                else
                                {
                                    oGestoreSelezionata.ModificaValore(oDcDatoSelezionata.Key, oDcDatoPrecedente.Value);
                                }
                            }

                        }
                        else if (bRipetibileSelezionata == true && bRipetibilePrecedente == false)
                        {
                            DcDato oDcDatoPrecedente = oGestorePrecedente.LeggeDato(oDcDatoSelezionata.Key);
                            if (
                                oDcDatoPrecedente != null
                                && oDcDatoPrecedente.Value.ToString() != string.Empty


                                && (oDcVoceSelezionata.Default == string.Empty || bcopiasempre)

                                && (oDcDatoSelezionata.Value.ToString() == string.Empty || oDcVoceSelezionata.Formato == enumFormatoVoce.Booleano)


                                && oDcVoceSelezionata.Formato == oDcVocePrecedente.Formato

                                && (oGestoreSelezionata.SchedaLayouts.Layouts[oDcVoceSelezionata.ID].TipoVoce == oGestorePrecedente.SchedaLayouts.Layouts[oDcVocePrecedente.ID].TipoVoce
                                || (oGestoreSelezionata.SchedaLayouts.Layouts[oDcVoceSelezionata.ID].TipoVoce == enumTipoVoce.TestoRtf
                                    && oGestorePrecedente.SchedaLayouts.Layouts[oDcVocePrecedente.ID].TipoVoce == enumTipoVoce.Testo)
                                || (oGestoreSelezionata.SchedaLayouts.Layouts[oDcVoceSelezionata.ID].TipoVoce == enumTipoVoce.Testo
                                    && oGestorePrecedente.SchedaLayouts.Layouts[oDcVocePrecedente.ID].TipoVoce == enumTipoVoce.TestoRtf)
                                    )
                                )
                            {
                                if (oGestoreSelezionata.SchedaLayouts.Layouts[oDcVoceSelezionata.ID].TipoVoce == enumTipoVoce.TestoRtf
                                    && oGestorePrecedente.SchedaLayouts.Layouts[oDcVocePrecedente.ID].TipoVoce == enumTipoVoce.Testo)
                                {
                                    oGestoreSelezionata.ModificaValore(oDcDatoSelezionata.Key, getTestoToTestoRtf(oDcDatoPrecedente.Value));
                                }
                                else if (oGestoreSelezionata.SchedaLayouts.Layouts[oDcVoceSelezionata.ID].TipoVoce == enumTipoVoce.Testo
                                    && oGestorePrecedente.SchedaLayouts.Layouts[oDcVocePrecedente.ID].TipoVoce == enumTipoVoce.TestoRtf)
                                {
                                    oGestoreSelezionata.ModificaValore(oDcDatoSelezionata.Key, getTestoRtfToTesto(oDcDatoPrecedente.Value));
                                }
                                else
                                {
                                    oGestoreSelezionata.ModificaValore(oDcDatoSelezionata.Key, oDcDatoPrecedente.Value);
                                }
                            }

                        }
                        else if (bRipetibileSelezionata == false && bRipetibilePrecedente == true)
                        {
                        }
                        else if (bRipetibileSelezionata == true && bRipetibilePrecedente == true)
                        {
                            int nSequenzaPrecedente = oGestorePrecedente.LeggeSequenze(oDcDatoSelezionata.ID);
                            for (int i = 1; i <= nSequenzaPrecedente; i++)
                            {

                                DcDato oDcDatoPrecedente = oGestorePrecedente.LeggeDato(oDcDatoSelezionata.ID, i);
                                if (
                                    oDcDatoPrecedente != null

                                    && (oDcVoceSelezionata.Default == string.Empty || bcopiasempre)

                                    && oDcVoceSelezionata.Formato == oDcVocePrecedente.Formato

                                    && (oGestoreSelezionata.SchedaLayouts.Layouts[oDcVoceSelezionata.ID].TipoVoce == oGestorePrecedente.SchedaLayouts.Layouts[oDcVocePrecedente.ID].TipoVoce
                                    || (oGestoreSelezionata.SchedaLayouts.Layouts[oDcVoceSelezionata.ID].TipoVoce == enumTipoVoce.TestoRtf
                                    && oGestorePrecedente.SchedaLayouts.Layouts[oDcVocePrecedente.ID].TipoVoce == enumTipoVoce.Testo)
                                    || (oGestoreSelezionata.SchedaLayouts.Layouts[oDcVoceSelezionata.ID].TipoVoce == enumTipoVoce.Testo
                                    && oGestorePrecedente.SchedaLayouts.Layouts[oDcVocePrecedente.ID].TipoVoce == enumTipoVoce.TestoRtf)
                                    )
                                )
                                {

                                    int nSequenzaSelezionata = oGestoreSelezionata.LeggeSequenze(oDcDatoSelezionata.ID);
                                    if (i > nSequenzaSelezionata)
                                    {
                                        oGestoreSelezionata.NuovaRiga(oDcVoceSelezionata.Padre.Key, i);
                                    }

                                    if (oGestoreSelezionata.LeggeValore(oDcDatoSelezionata.ID, i).ToString() == string.Empty)
                                    {
                                        if (oGestoreSelezionata.SchedaLayouts.Layouts[oDcVoceSelezionata.ID].TipoVoce == enumTipoVoce.TestoRtf
                                            && oGestorePrecedente.SchedaLayouts.Layouts[oDcVocePrecedente.ID].TipoVoce == enumTipoVoce.Testo)
                                        {
                                            oGestoreSelezionata.ModificaValore(oDcDatoSelezionata.ID, i, getTestoToTestoRtf(oDcDatoPrecedente.Value));
                                        }
                                        else if (oGestoreSelezionata.SchedaLayouts.Layouts[oDcVoceSelezionata.ID].TipoVoce == enumTipoVoce.Testo
                                            && oGestorePrecedente.SchedaLayouts.Layouts[oDcVocePrecedente.ID].TipoVoce == enumTipoVoce.TestoRtf)
                                        {
                                            oGestoreSelezionata.ModificaValore(oDcDatoSelezionata.ID, i, getTestoRtfToTesto(oDcDatoPrecedente.Value));
                                        }
                                        else
                                        {
                                            oGestoreSelezionata.ModificaValore(oDcDatoSelezionata.ID, i, oDcDatoPrecedente.Value);
                                        }
                                    }

                                }

                            }

                        }

                    }

                }

                sret = oGestoreSelezionata.SchedaDatiXML;

            }
            catch (Exception ex)
            {
                throw new Exception(@"MovScheda.getNuoviDatiXML()" + Environment.NewLine + ex.Message, ex);
            }

            return sret;

        }

        private string getNuoviDatiXMLPerConversione(MovScheda movScheda, MovScheda oMovSchedaPrecedente)
        {

            string sret = oMovSchedaPrecedente.DatiXML;

            try
            {

                Gestore oGestorePrecedente = CommonStatics.GetGestore(this.Ambiente);
                oGestorePrecedente.SchedaXML = oMovSchedaPrecedente.Scheda.StrutturaXML;
                oGestorePrecedente.SchedaLayoutsXML = oMovSchedaPrecedente.Scheda.LayoutXML;
                oGestorePrecedente.Decodifiche = oMovSchedaPrecedente.Scheda.DizionarioValori();
                oGestorePrecedente.SchedaDatiXML = oMovSchedaPrecedente.DatiXML;

                Gestore oGestoreSelezionata = CommonStatics.GetGestore(this.Ambiente);
                oGestoreSelezionata.SchedaXML = movScheda.Scheda.StrutturaXML;
                oGestoreSelezionata.SchedaLayoutsXML = movScheda.Scheda.LayoutXML;
                oGestoreSelezionata.Decodifiche = movScheda.Scheda.DizionarioValori();
                if (movScheda.DatiXML != string.Empty)
                {
                    oGestoreSelezionata.SchedaDatiXML = movScheda.DatiXML;
                }
                oGestoreSelezionata.NuovaScheda();

                Gestore oGestoreCiclo = CommonStatics.GetGestore(this.Ambiente);
                oGestoreCiclo.SchedaXML = movScheda.Scheda.StrutturaXML;
                oGestoreCiclo.SchedaLayoutsXML = movScheda.Scheda.LayoutXML;
                oGestoreCiclo.Decodifiche = movScheda.Scheda.DizionarioValori();
                if (movScheda.DatiXML != string.Empty)
                {
                    oGestoreCiclo.SchedaDatiXML = movScheda.DatiXML;
                }
                oGestoreCiclo.NuovaScheda();

                foreach (DcDato oDcDatoSelezionata in oGestoreCiclo.SchedaDati.Dati.Values)
                {

                    DcVoce oDcVoceSelezionata = oGestoreSelezionata.LeggeVoce(oDcDatoSelezionata.ID);
                    DcVoce oDcVocePrecedente = oGestorePrecedente.LeggeVoce(oDcDatoSelezionata.ID);
                    if (oDcVoceSelezionata != null && oDcVocePrecedente != null)
                    {

                        bool bRipetibileSelezionata = this.CheckSezioneRipetibile(oGestoreSelezionata.Scheda.Sezioni[oDcVoceSelezionata.Padre.Key]);
                        bool bRipetibilePrecedente = this.CheckSezioneRipetibile(oGestorePrecedente.Scheda.Sezioni[oDcVocePrecedente.Padre.Key]);

                        if (bRipetibileSelezionata == false && bRipetibilePrecedente == false)
                        {

                            DcDato oDcDatoPrecedente = oGestorePrecedente.LeggeDato(oDcDatoSelezionata.Key);
                            if (
                                oDcDatoPrecedente != null
                                && oDcDatoPrecedente.Value.ToString() != string.Empty

                                && oDcVoceSelezionata.Formato == oDcVocePrecedente.Formato

                                && (oGestoreSelezionata.SchedaLayouts.Layouts[oDcVoceSelezionata.ID].TipoVoce == oGestorePrecedente.SchedaLayouts.Layouts[oDcVocePrecedente.ID].TipoVoce
                                || (oGestoreSelezionata.SchedaLayouts.Layouts[oDcVoceSelezionata.ID].TipoVoce == enumTipoVoce.TestoRtf
                                    && oGestorePrecedente.SchedaLayouts.Layouts[oDcVocePrecedente.ID].TipoVoce == enumTipoVoce.Testo)
                                || (oGestoreSelezionata.SchedaLayouts.Layouts[oDcVoceSelezionata.ID].TipoVoce == enumTipoVoce.Testo
                                    && oGestorePrecedente.SchedaLayouts.Layouts[oDcVocePrecedente.ID].TipoVoce == enumTipoVoce.TestoRtf)
                                    )
                                )
                            {
                                if (oGestoreSelezionata.SchedaLayouts.Layouts[oDcVoceSelezionata.ID].TipoVoce == enumTipoVoce.TestoRtf
                                    && oGestorePrecedente.SchedaLayouts.Layouts[oDcVocePrecedente.ID].TipoVoce == enumTipoVoce.Testo)
                                {
                                    oGestoreSelezionata.ModificaValore(oDcDatoSelezionata.Key, getTestoToTestoRtf(oDcDatoPrecedente.Value));
                                }
                                else if (oGestoreSelezionata.SchedaLayouts.Layouts[oDcVoceSelezionata.ID].TipoVoce == enumTipoVoce.Testo
                                    && oGestorePrecedente.SchedaLayouts.Layouts[oDcVocePrecedente.ID].TipoVoce == enumTipoVoce.TestoRtf)
                                {
                                    oGestoreSelezionata.ModificaValore(oDcDatoSelezionata.Key, getTestoRtfToTesto(oDcDatoPrecedente.Value));
                                }
                                else
                                {
                                    oGestoreSelezionata.ModificaValore(oDcDatoSelezionata.Key, oDcDatoPrecedente.Value);
                                }
                            }

                            if (oDcDatoPrecedente != null)
                            {
                                oGestoreSelezionata.ModificaAbilitato(oDcDatoSelezionata.Key, oDcDatoPrecedente.Abilitato);
                                oGestoreSelezionata.ModificaObbligatorio(oDcDatoSelezionata.Key, oDcDatoPrecedente.Obbligatorio);
                                oGestoreSelezionata.ModificaValido(oDcDatoSelezionata.Key, oDcDatoPrecedente.Valido);
                                oGestoreSelezionata.ModificaInRilievo(oDcDatoSelezionata.Key, oDcDatoPrecedente.InRilievo);
                                oGestoreSelezionata.ModificaTranscodifica(oDcDatoSelezionata.Key, oDcDatoPrecedente.Transcodifica);
                            }
                        }
                        else if (bRipetibileSelezionata == true && bRipetibilePrecedente == false)
                        {
                            DcDato oDcDatoPrecedente = oGestorePrecedente.LeggeDato(oDcDatoSelezionata.Key);
                            if (
                                oDcDatoPrecedente != null
                                && oDcDatoPrecedente.Value.ToString() != string.Empty

                                && oDcVoceSelezionata.Formato == oDcVocePrecedente.Formato

                                && (oGestoreSelezionata.SchedaLayouts.Layouts[oDcVoceSelezionata.ID].TipoVoce == oGestorePrecedente.SchedaLayouts.Layouts[oDcVocePrecedente.ID].TipoVoce
                                || (oGestoreSelezionata.SchedaLayouts.Layouts[oDcVoceSelezionata.ID].TipoVoce == enumTipoVoce.TestoRtf
                                    && oGestorePrecedente.SchedaLayouts.Layouts[oDcVocePrecedente.ID].TipoVoce == enumTipoVoce.Testo)
                                || (oGestoreSelezionata.SchedaLayouts.Layouts[oDcVoceSelezionata.ID].TipoVoce == enumTipoVoce.Testo
                                    && oGestorePrecedente.SchedaLayouts.Layouts[oDcVocePrecedente.ID].TipoVoce == enumTipoVoce.TestoRtf)
                                    )
                                )
                            {
                                if (oGestoreSelezionata.SchedaLayouts.Layouts[oDcVoceSelezionata.ID].TipoVoce == enumTipoVoce.TestoRtf
                                    && oGestorePrecedente.SchedaLayouts.Layouts[oDcVocePrecedente.ID].TipoVoce == enumTipoVoce.Testo)
                                {
                                    oGestoreSelezionata.ModificaValore(oDcDatoSelezionata.Key, getTestoToTestoRtf(oDcDatoPrecedente.Value));
                                }
                                else if (oGestoreSelezionata.SchedaLayouts.Layouts[oDcVoceSelezionata.ID].TipoVoce == enumTipoVoce.Testo
                                    && oGestorePrecedente.SchedaLayouts.Layouts[oDcVocePrecedente.ID].TipoVoce == enumTipoVoce.TestoRtf)
                                {
                                    oGestoreSelezionata.ModificaValore(oDcDatoSelezionata.Key, getTestoRtfToTesto(oDcDatoPrecedente.Value));
                                }
                                else
                                {
                                    oGestoreSelezionata.ModificaValore(oDcDatoSelezionata.Key, oDcDatoPrecedente.Value);
                                }
                            }

                            if (oDcDatoPrecedente != null)
                            {
                                oGestoreSelezionata.ModificaAbilitato(oDcDatoSelezionata.Key, oDcDatoPrecedente.Abilitato);
                                oGestoreSelezionata.ModificaObbligatorio(oDcDatoSelezionata.Key, oDcDatoPrecedente.Obbligatorio);
                                oGestoreSelezionata.ModificaValido(oDcDatoSelezionata.Key, oDcDatoPrecedente.Valido);
                                oGestoreSelezionata.ModificaInRilievo(oDcDatoSelezionata.Key, oDcDatoPrecedente.InRilievo);
                                oGestoreSelezionata.ModificaTranscodifica(oDcDatoSelezionata.Key, oDcDatoPrecedente.Transcodifica);
                            }

                        }
                        else if (bRipetibileSelezionata == false && bRipetibilePrecedente == true)
                        {
                        }
                        else if (bRipetibileSelezionata == true && bRipetibilePrecedente == true)
                        {
                            int nSequenzaPrecedente = oGestorePrecedente.LeggeSequenze(oDcDatoSelezionata.ID);
                            for (int i = 1; i <= nSequenzaPrecedente; i++)
                            {

                                DcDato oDcDatoPrecedente = oGestorePrecedente.LeggeDato(oDcDatoSelezionata.ID, i);
                                if (
                                    oDcDatoPrecedente != null

                                    && oDcVoceSelezionata.Formato == oDcVocePrecedente.Formato

                                    && (oGestoreSelezionata.SchedaLayouts.Layouts[oDcVoceSelezionata.ID].TipoVoce == oGestorePrecedente.SchedaLayouts.Layouts[oDcVocePrecedente.ID].TipoVoce
                                    || (oGestoreSelezionata.SchedaLayouts.Layouts[oDcVoceSelezionata.ID].TipoVoce == enumTipoVoce.TestoRtf
                                    && oGestorePrecedente.SchedaLayouts.Layouts[oDcVocePrecedente.ID].TipoVoce == enumTipoVoce.Testo)
                                    || (oGestoreSelezionata.SchedaLayouts.Layouts[oDcVoceSelezionata.ID].TipoVoce == enumTipoVoce.Testo
                                    && oGestorePrecedente.SchedaLayouts.Layouts[oDcVocePrecedente.ID].TipoVoce == enumTipoVoce.TestoRtf)
                                    )
                                )
                                {

                                    int nSequenzaSelezionata = oGestoreSelezionata.LeggeSequenze(oDcDatoSelezionata.ID);
                                    if (i > nSequenzaSelezionata)
                                    {
                                        oGestoreSelezionata.NuovaRiga(oDcVoceSelezionata.Padre.Key, i);
                                    }

                                    if (oGestoreSelezionata.SchedaLayouts.Layouts[oDcVoceSelezionata.ID].TipoVoce == enumTipoVoce.TestoRtf
                                        && oGestorePrecedente.SchedaLayouts.Layouts[oDcVocePrecedente.ID].TipoVoce == enumTipoVoce.Testo)
                                    {
                                        oGestoreSelezionata.ModificaValore(oDcDatoSelezionata.ID, i, getTestoToTestoRtf(oDcDatoPrecedente.Value));
                                    }
                                    else if (oGestoreSelezionata.SchedaLayouts.Layouts[oDcVoceSelezionata.ID].TipoVoce == enumTipoVoce.Testo
                                        && oGestorePrecedente.SchedaLayouts.Layouts[oDcVocePrecedente.ID].TipoVoce == enumTipoVoce.TestoRtf)
                                    {
                                        oGestoreSelezionata.ModificaValore(oDcDatoSelezionata.ID, i, getTestoRtfToTesto(oDcDatoPrecedente.Value));
                                    }
                                    else
                                    {
                                        oGestoreSelezionata.ModificaValore(oDcDatoSelezionata.ID, i, oDcDatoPrecedente.Value);
                                    }

                                }

                                if (oDcDatoPrecedente != null)
                                {
                                    oGestoreSelezionata.ModificaAbilitato(oDcDatoSelezionata.ID, i, oDcDatoPrecedente.Abilitato);
                                    oGestoreSelezionata.ModificaObbligatorio(oDcDatoSelezionata.ID, i, oDcDatoPrecedente.Obbligatorio);
                                    oGestoreSelezionata.ModificaValido(oDcDatoSelezionata.ID, i, oDcDatoPrecedente.Valido);
                                    oGestoreSelezionata.ModificaInRilievo(oDcDatoSelezionata.ID, i, oDcDatoPrecedente.InRilievo);
                                    oGestoreSelezionata.ModificaTranscodifica(oDcDatoSelezionata.ID, i, oDcDatoPrecedente.Transcodifica);
                                }

                            }

                        }

                    }

                }

                sret = oGestoreSelezionata.SchedaDatiXML;

            }
            catch (Exception ex)
            {
                throw new Exception(@"MovScheda.getNuoviDatiXMLPerConversione()" + Environment.NewLine + ex.Message, ex);
            }

            return sret;

        }

        private bool CheckSezioneRipetibile(DcSezione sezione)
        {

            bool bret = false;

            if (sezione.Attributi.ContainsKey(EnumAttributiSezione.Ripetibile.ToString()) == true)
            {
                bret = bool.Parse(((DcAttributo)sezione.Attributi[EnumAttributiSezione.Ripetibile.ToString()]).Value.ToString());
            }

            return bret;

        }

        private object getTestoToTestoRtf(object valore)
        {

            object oRet = valore;

            RtfFiles rtf = new RtfFiles();
            string rtfAnteprima = "";

            try
            {

                System.Drawing.Font f = rtf.getFontFromString(Database.GetConfigTable(EnumConfigTable.FontPredefinitoRTF), false, false);
                rtfAnteprima = rtf.initRtf(f);
                rtfAnteprima = rtf.appendRtfText(rtfAnteprima, valore.ToString(), f);

                oRet = rtfAnteprima;

            }
            catch (Exception)
            {

            }

            return oRet;

        }

        private object getTestoRtfToTesto(object valore)
        {

            object oRet = valore;

            RtfTree Tree = new RtfTree();

            try
            {

                Tree.LoadRtfText(valore.ToString());

                oRet = Tree.Text;

            }
            catch (Exception)
            {

            }

            return oRet;

        }

        private string getAllineamentoTipo(HorizontalAlignment en)
        {

            const string TAG_GRIDVAL_INIZIO_ALIGN_LEFT = @"\ql";
            const string TAG_GRIDVAL_INIZIO_ALIGN_RIGHT = @"\qr";
            const string TAG_GRIDVAL_INIZIO_ALIGN_CENTER = @"\qc";

            string sReturn = TAG_GRIDVAL_INIZIO_ALIGN_LEFT;

            if (en == HorizontalAlignment.Center)
            {
                sReturn = TAG_GRIDVAL_INIZIO_ALIGN_CENTER;
            }
            else if (en == HorizontalAlignment.Right)
            {
                sReturn = TAG_GRIDVAL_INIZIO_ALIGN_RIGHT;
            }

            return sReturn;

        }

        private int getPermessoUAFirma()
        {

            int bret = 0;

            try
            {

                Parametri op = new Parametri(this.Ambiente);
                op.Parametro.Add("CodUA", this.CodUA);
                op.Parametro.Add("CodModulo", "FirmaD_SCH");
                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataTable dt = Database.GetDataTableStoredProc("MSP_SelUAModuli", spcoll);

                if (dt.Rows.Count > 0)
                {
                    bret = 1;
                }

            }
            catch (Exception)
            {

            }

            return bret;

        }

        private bool isValidRtf(string text)
        {
            try
            {
                using (System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox())
                {
                    rtb.Rtf = text;
                }
            }
            catch (ArgumentException)
            {
                return false;

            }

            return true;
        }

        private bool isValidRtf(string text, out string plainText)
        {
            plainText = "";

            try
            {
                using (System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox())
                {
                    rtb.Rtf = text;

                    plainText = rtb.Text;
                    plainText = plainText.Replace("\n", Environment.NewLine);
                }
            }
            catch (ArgumentException)
            {
                return false;

            }

            return true;
        }

    }

}