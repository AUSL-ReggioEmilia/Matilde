using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.DatiClinici.DC;
using UnicodeSrl.DatiClinici.Common;
using UnicodeSrl.DatiClinici.Interfaces;

namespace UnicodeSrl.Scci
{
    [Serializable()]
    public class Scheda
    {

        private string _codice = string.Empty;
        private string _descrizione = string.Empty;
        private string _note = string.Empty;
        private string _path = string.Empty;
        private string _codtiposcheda = string.Empty;
        private bool _schedasemplice = false;
        private string _codentita = string.Empty;
        private string _ordine = string.Empty;
        private int _numerositaminima = 1;
        private int _numerositamassima = 1;
        private bool _creadefault = false;
        private bool _validabile = false;
        private bool _contenitore = false;
        private bool _alertschedavuota = false;
        private bool _copiarecedenteselezione = false;

        private int _versione = 0;

        private string _strutturaxml = string.Empty;
        private string _layoutxml = string.Empty;
        private DcDecodifiche _DcDecodifiche = null;

        DataContracts.ScciAmbiente _ambiente = new UnicodeSrl.Scci.DataContracts.ScciAmbiente();

        public Scheda(string codice, DateTime data, DataContracts.ScciAmbiente ambiente)
        {
            this._ambiente = ambiente;
            this.Carica(codice, data);
        }
        public Scheda(string codice, int versione, DateTime data, DataContracts.ScciAmbiente ambiente)
        {
            this._ambiente = ambiente;
            this.Carica(codice, versione, data);
        }

        public string Codice
        {
            get { return _codice; }
        }

        public string Descrizione
        {
            get { return _descrizione; }
        }

        public string Note
        {
            get { return _note; }
        }

        public string Path
        {
            get { return _path; }
        }

        public string CodTipoScheda
        {
            get { return _codtiposcheda; }
        }

        public bool SchedaSemplice
        {
            get { return _schedasemplice; }
        }

        public string CodEntita
        {
            get { return _codentita; }
        }

        public string Ordine
        {
            get { return _ordine; }
        }

        public int NumerositaMinima
        {
            get { return _numerositaminima; }
        }

        public int NumerositaMassima
        {
            get { return _numerositamassima; }
        }

        public bool CreaDefault
        {
            get { return _creadefault; }
        }

        public bool Validabile
        {
            get { return _validabile; }
        }

        public bool Contenitore
        {
            get { return _contenitore; }

        }

        public bool AlertSchedaVuota
        {
            get { return _alertschedavuota; }

        }

        public bool CopiaPrecedenteSelezione
        {
            get { return _copiarecedenteselezione; }

        }

        public int Versione
        {
            get { return _versione; }
            set { _versione = value; }
        }

        public string StrutturaXML
        {
            get { return _strutturaxml; }
            set { _strutturaxml = value; }
        }

        public string LayoutXML
        {
            get { return _layoutxml; }
            set { _layoutxml = value; }
        }

        public DcDecodifiche Decodifiche
        {
            get { return _DcDecodifiche; }
            set { _DcDecodifiche = value; }
        }

        public bool CartellaAmbulatorialeCodificata { get; set; }

        public string CodContatore { get; set; }

        private void Carica(string codice, DateTime data)
        {
            try
            {

                Parametri op = new Parametri(this._ambiente);
                op.Parametro.Add("CodScheda", codice);
                op.Parametro.Add("DataRif", Database.dataOra105PerParametri(data));

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelScheda", spcoll);

                if (dt.Rows.Count > 0)
                {

                    _codice = dt.Rows[0]["Codice"].ToString();
                    _descrizione = dt.Rows[0]["Descrizione"].ToString();
                    _note = dt.Rows[0]["Note"].ToString();
                    _path = dt.Rows[0]["Path"].ToString();
                    _codtiposcheda = dt.Rows[0]["CodTipoScheda"].ToString();
                    _schedasemplice = (bool)dt.Rows[0]["SchedaSemplice"];
                    _codentita = dt.Rows[0]["CodEntita"].ToString();
                    _ordine = dt.Rows[0]["Ordine"].ToString();
                    _numerositaminima = (int)dt.Rows[0]["NumerositaMinima"];
                    _numerositamassima = (int)dt.Rows[0]["NumerositaMassima"];
                    _creadefault = (bool)dt.Rows[0]["CreaDefault"];
                    _validabile = (bool)dt.Rows[0]["Validabile"];
                    _contenitore = (bool)dt.Rows[0]["Contenitore"];
                    _alertschedavuota = (bool)dt.Rows[0]["AlertSchedaVuota"];
                    _copiarecedenteselezione = (bool)dt.Rows[0]["CopiaPrecedenteSelezione"];

                    _versione = (int)dt.Rows[0]["Versione"];

                    _strutturaxml = dt.Rows[0]["StrutturaXML"].ToString();
                    _layoutxml = dt.Rows[0]["LayoutXML"].ToString();
                    _DcDecodifiche = null;
                    SetExpandXml(data);

                    this.CartellaAmbulatorialeCodificata = (bool)dt.Rows[0]["CartellaAmbulatorialeCodificata"];
                    this.CodContatore = dt.Rows[0]["CodContatore"].ToString();

                }
            }
            catch (Exception ex)
            {
                this.SvuotaDati();
                throw new Exception(ex.Message, ex);
            }
        }
        private void Carica(string codice, int versione, DateTime data)
        {
            try
            {

                Parametri op = new Parametri(this._ambiente);
                op.Parametro.Add("CodScheda", codice);
                op.Parametro.Add("Versione", versione.ToString());

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelScheda", spcoll);

                if (dt.Rows.Count > 0)
                {

                    _codice = dt.Rows[0]["Codice"].ToString();
                    _descrizione = dt.Rows[0]["Descrizione"].ToString();
                    _note = dt.Rows[0]["Note"].ToString();
                    _path = dt.Rows[0]["Path"].ToString();
                    _codtiposcheda = dt.Rows[0]["CodTipoScheda"].ToString();
                    _schedasemplice = (bool)dt.Rows[0]["SchedaSemplice"];
                    _codentita = dt.Rows[0]["CodEntita"].ToString();
                    _ordine = dt.Rows[0]["Ordine"].ToString();
                    _numerositaminima = (int)dt.Rows[0]["NumerositaMinima"];
                    _numerositamassima = (int)dt.Rows[0]["NumerositaMassima"];
                    _creadefault = (bool)dt.Rows[0]["CreaDefault"];
                    _validabile = (bool)dt.Rows[0]["Validabile"];
                    _contenitore = (bool)dt.Rows[0]["Contenitore"];
                    _alertschedavuota = (bool)dt.Rows[0]["AlertSchedaVuota"];
                    _copiarecedenteselezione = (bool)dt.Rows[0]["CopiaPrecedenteSelezione"];

                    _versione = (int)dt.Rows[0]["Versione"];

                    _strutturaxml = dt.Rows[0]["StrutturaXML"].ToString();
                    _layoutxml = dt.Rows[0]["LayoutXML"].ToString();
                    _DcDecodifiche = null;
                    SetExpandXml(data);

                    this.CartellaAmbulatorialeCodificata = (bool)dt.Rows[0]["CartellaAmbulatorialeCodificata"];
                    this.CodContatore = dt.Rows[0]["CodContatore"].ToString();

                }

            }
            catch (Exception ex)
            {
                this.SvuotaDati();
                throw new Exception(ex.Message, ex);
            }
        }

        private void SvuotaDati()
        {

            _codice = string.Empty;
            _descrizione = string.Empty;
            _note = string.Empty;
            _path = string.Empty;
            _codtiposcheda = string.Empty;
            _schedasemplice = false;
            _codentita = string.Empty;
            _ordine = string.Empty;
            _numerositaminima = 1;
            _numerositamassima = 1;
            _creadefault = false;
            _validabile = false;
            _contenitore = false;
            _alertschedavuota = false;
            _copiarecedenteselezione = false;

            _versione = 0;

            _strutturaxml = string.Empty;
            _layoutxml = string.Empty;
            _DcDecodifiche = null;

            this.CartellaAmbulatorialeCodificata = false;
            this.CodContatore = string.Empty;

        }

        public DcDecodifiche DizionarioValori()
        {

            if (_DcDecodifiche == null)
            {

                _DcDecodifiche = new DcDecodifiche();

                try
                {

                    Parametri op = new Parametri(this._ambiente);
                    op.Parametro.Add("CodScheda", this._codice);
                    op.Parametro.Add("Versione", this.Versione.ToString());

                    SqlParameterExt[] spcoll = new SqlParameterExt[1];

                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    DataTable dt = Database.GetDataTableStoredProc("MSP_SelDizionariSchedaValori", spcoll);

                    _DcDecodifiche.DataTable = dt;

                    foreach (DataRow oDr in dt.Rows)
                    {
                        DcObject oDcObject = new DcObject();
                        oDcObject.Key = string.Format("{0}§{1}", oDr["CodTab"].ToString(), oDr["CodValore"].ToString());
                        if (oDr["Icona"] != DBNull.Value)
                        {
                            oDcObject.Value = Convert.ToBase64String((byte[])oDr["Icona"]);
                        }
                        else
                        {
                            oDcObject.Value = oDr["Descrizione"].ToString();
                        }
                        _DcDecodifiche.Add(oDcObject.Key, oDcObject);
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }

            }

            return _DcDecodifiche;

        }

        public void SetExpandXml(DateTime data)
        {

            try
            {

                DcScheda oDcScheda = (DcScheda)Serializer.FromXmlString(_strutturaxml, typeof(DcScheda));
                DcSchedaLayouts oDcSchedaLayouts = (DcSchedaLayouts)Serializer.FromXmlString(_layoutxml, typeof(DcSchedaLayouts));

                this.DizionarioValori();

                foreach (DcSezione oDcSezione in oDcScheda.Sezioni.Values)
                {

                    foreach (DcVoce oDcVoce in oDcSezione.Voci.Values)
                    {

                        if (oDcVoce.Formato == DatiClinici.Common.Enums.enumFormatoVoce.Oggetto)
                        {

                            if (oDcSchedaLayouts.Layouts.ContainsKey(oDcVoce.Key) == true &&
                                oDcSchedaLayouts.Layouts[oDcVoce.Key].TipoVoce == DatiClinici.Common.Enums.enumTipoVoce.Scheda)
                            {

                                Scheda _Scheda = new Scheda(oDcVoce.SottoScheda, data, _ambiente);

                                oDcVoce.Value = (DcScheda)Serializer.FromXmlString(_Scheda.StrutturaXML, typeof(DcScheda));
                                oDcSchedaLayouts.Layouts[oDcVoce.Key].Value = (DcSchedaLayouts)Serializer.FromXmlString(_Scheda.LayoutXML, typeof(DcSchedaLayouts));

                                foreach (DcObject oObject in _Scheda.Decodifiche.Values)
                                {
                                    if (_DcDecodifiche.ContainsKey(oObject.Key) == false)
                                    {
                                        _DcDecodifiche.Add(oObject.Key, oObject);
                                    }
                                }

                                _Scheda = null;

                            }

                        }

                    }

                }

                _strutturaxml = Serializer.ToXmlString(oDcScheda);
                _layoutxml = Serializer.ToXmlString(oDcSchedaLayouts);

            }
            catch (Exception)
            {

            }

        }

    }
}
