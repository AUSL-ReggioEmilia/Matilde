using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using UnicodeSrl.DatiClinici.DC;
using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Scci.Model.Strutture;

namespace UnicodeSrl.DataModel
{

    public static partial class FwDataConnectionExt
    {
        public static FwDataBufferedList<MSP_SelMovOrdini_GridOE> MSP_SelMovOrdini_GridOE(this FwDataConnection cnn,
            string codUtenteIns,
            string idOrdineEscl,
            string idCampoOE,
            DcDecodifiche decodifiche)
        {
            FwDataBufferedList<MSP_SelMovOrdini_GridOE> result = null;

            FwDataParametersList plist = new FwDataParametersList();

            plist.Add("codUtenteIns", codUtenteIns, ParameterDirection.Input, DbType.String, 255);
            plist.Add("idOrdineEsc", idOrdineEscl, ParameterDirection.Input, DbType.String, 50);
            plist.Add("idCampoOE", idCampoOE, ParameterDirection.Input, DbType.String, 255);

            result = cnn.Query<FwDataBufferedList<MSP_SelMovOrdini_GridOE>>("MSP_SelMovOrdini_GridOE", plist, CommandType.StoredProcedure);

            foreach (MSP_SelMovOrdini_GridOE row in result.Buffer)
            {
                row.LoadSchedeDati(decodifiche);
            }

            return result;
        }


    }

    [DataContract()]
    [ModelStoredProcedure("MSP_SelMovOrdini_GridOE")]
    public class MSP_SelMovOrdini_GridOE : FwModelRow<MSP_SelMovOrdini_GridOE>
    {
        private const char C_SEPDECONTROLSDC = '|';

        public MSP_SelMovOrdini_GridOE() : base()
        {
        }

        [DataMember()]
        public Guid ID { get; set; }

        [DataMember()]
        public String IDCampoOEFiltrato { get; set; }

        [DataMember()]
        public String IDOrdineOE { get; set; }

        [DataMember()]
        public String NumeroOrdineOE { get; set; }

        [DataMember()]
        [ValidationStringLenght(8)]
        public DateTime? DataProgrammazioneOE { get; set; }

        [DataMember()]
        public DateTime DataUltimaModifica { get; set; }

        [DataMember()]
        public String Eroganti { get; set; }

        [DataMember()]
        public String Prestazioni { get; set; }

        [DataMember()]
        public String NumeroNosologico { get; set; }

        [DataMember()]
        public String Nome { get; set; }

        [DataMember()]
        public String Cognome { get; set; }

        [DataMember()]
        public String Sesso { get; set; }

        [DataMember()]
        public DateTime? DataNascita { get; set; }

        [DataMember()]
        public Int32? Eta { get; set; }

        [DataMember()]
        public String StrutturaDatiAccessori
        {
            get; set;
        }

        [DataMember()]
        public String DatiDatiAccessori
        {
            get; set;
        }

        [DataMember()]
        public String LayoutDatiAccessori
        {
            get; set;
        }

        [DataFieldIgnore]
        public string DescrizionePaziente
        {
            get
            {
                string nasc = "";
                if (this.DataNascita.HasValue)
                    nasc = this.DataNascita.Value.ToShortDateString();

                string descr = $"{this.Nome} {this.Cognome}, {nasc}, {this.Sesso} ({this.Eta} aa)";
                return descr;
            }
        }

        [DataFieldIgnore]
        private List<SelectionObjectString> DatiOE { get; set; }


        [DataFieldIgnore]
        public string DescrizioneDatiOE
        {
            get
            {
                string ret = "";
                if (this.DatiOE == null) return "";
                if (this.DatiOE.Count == 0) return "";

                foreach (SelectionObjectString dato in DatiOE)
                {
                    ret += $", {dato.Descrizione}";
                }

                ret = ret.Substring(1);

                return ret;
            }
        }

        public List<SelectionObjectString> GetDatiOE()
        {
            return this.DatiOE;
        }

        [DataFieldIgnore]
        private DcDecodifiche Decodifiche { get; set; }

        internal void LoadSchedeDati(DcDecodifiche decodifiche)
        {
            this.DatiOE = new List<SelectionObjectString>();

            this.Decodifiche = decodifiche;

            if ((this.StrutturaDatiAccessori == null) || (this.StrutturaDatiAccessori == "")) return;
            if ((this.DatiDatiAccessori == null) || (this.DatiDatiAccessori == "")) return;
            if ((this.LayoutDatiAccessori == null) || (this.LayoutDatiAccessori == "")) return;

            Gestore gestore = new Gestore();

            gestore.Decodifiche = decodifiche;
            gestore.SchedaXML = this.StrutturaDatiAccessori;
            gestore.SchedaDatiXML = this.DatiDatiAccessori;
            gestore.SchedaLayoutsXML = this.LayoutDatiAccessori;

            string keyToFind = this.IDCampoOEFiltrato + "_";
            string keyToFind_Old = this.IDCampoOEFiltrato.Substring(2) + "_";

            Func<KeyValuePair<string, DcDato>, bool> fpattern = new Func<KeyValuePair<string, DcDato>, bool>
                (
                    x => (
                                ((x.Key.StartsWith(keyToFind)) && (x.Value.Value != null) && (x.Value.Value.ToString() != ""))
                            ||
                                ((x.Key.StartsWith(keyToFind_Old)) && (x.Value.Value != null) && (x.Value.Value.ToString() != ""))
                         )
                );

            List<KeyValuePair<string, DcDato>> listCpy = gestore.SchedaDati.Dati.Where(fpattern).ToList();

            int idx = 0;
            string val = "";
            string descr = "";

            foreach (KeyValuePair<string, DcDato> item in listCpy)
            {
                idx++;

                DcVoce dcVoce = gestore.LeggeVoce(item.Value.ID);
                string decodifica = dcVoce.Decodifica;

                val = Convert.ToString(item.Value.Value);

                if (decodifica != "")
                {
                    string[] arrVals = val.Split(MSP_SelMovOrdini_GridOE.C_SEPDECONTROLSDC);

                    if (arrVals.Length > 1)
                    {
                        foreach (string itemValue in arrVals)
                        {
                            string itemDescr = gestore.DecodificaValore(decodifica, itemValue);
                            if (itemDescr == "") itemDescr = itemValue;
                            descr += @", " + itemDescr;
                        }

                        descr = descr.Substring(2);
                    }
                    else
                    {
                        descr = gestore.DecodificaValore(decodifica, val);
                    }
                }
                else
                    descr = val;

                if (descr == "") descr = val;

                this.DatiOE.Add
                    (
                        new SelectionObjectString(idx, val, descr, false)
                    );
            }


        }

    }
}
