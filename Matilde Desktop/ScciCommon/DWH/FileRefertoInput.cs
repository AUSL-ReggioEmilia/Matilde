using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnicodeSrl.Scci.DWH
{




    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.progel.it/BT/Dwh/FileRefertoInput/1.0")]
    [System.Xml.Serialization.XmlRootAttribute("Root", Namespace = "http://schemas.progel.it/BT/Dwh/FileRefertoInput/1.0", IsNullable = false)]
    public partial class MessaggioRefertoType
    {

        private System.DateTime dataSequenzaField;

        private int azioneField;

        private RefertoType refertoField;

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public System.DateTime DataSequenza
        {
            get
            {
                return this.dataSequenzaField;
            }
            set
            {
                this.dataSequenzaField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int Azione
        {
            get
            {
                return this.azioneField;
            }
            set
            {
                this.azioneField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public RefertoType Referto
        {
            get
            {
                return this.refertoField;
            }
            set
            {
                this.refertoField = value;
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.progel.it/BT/Dwh/FileRefertoInput/1.0")]
    public partial class RefertoType
    {

        private string idEsternoField;

        private string aziendaEroganteField;

        private string sistemaEroganteField;

        private CodiceDescrizioneType repartoEroganteField;

        private PazienteType pazienteField;

        private System.DateTime dataRefertoField;

        private string numeroRefertoField;

        private string numeroNosologicoField;

        private string numeroPrenotazioneField;

        private string idRichiestaOEField;

        private CodiceDescrizioneType repartoRichiedenteField;

        private StatoRichiestaEnum statoRichiestaField;

        private CodiceDescrizioneType tipoRichiestaField;

        private CodiceDescrizioneType prioritaField;

        private string testoRefertoField;

        private CodiceDescrizioneType medicoRefertanteField;

        private AttributoType[] attributiField;

        private PrestazioneType[] prestazioniField;

        private AllegatoType[] allegatiField;

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string IdEsterno
        {
            get
            {
                return this.idEsternoField;
            }
            set
            {
                this.idEsternoField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string AziendaErogante
        {
            get
            {
                return this.aziendaEroganteField;
            }
            set
            {
                this.aziendaEroganteField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SistemaErogante
        {
            get
            {
                return this.sistemaEroganteField;
            }
            set
            {
                this.sistemaEroganteField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public CodiceDescrizioneType RepartoErogante
        {
            get
            {
                return this.repartoEroganteField;
            }
            set
            {
                this.repartoEroganteField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public PazienteType Paziente
        {
            get
            {
                return this.pazienteField;
            }
            set
            {
                this.pazienteField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public System.DateTime DataReferto
        {
            get
            {
                return this.dataRefertoField;
            }
            set
            {
                this.dataRefertoField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NumeroReferto
        {
            get
            {
                return this.numeroRefertoField;
            }
            set
            {
                this.numeroRefertoField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NumeroNosologico
        {
            get
            {
                return this.numeroNosologicoField;
            }
            set
            {
                this.numeroNosologicoField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NumeroPrenotazione
        {
            get
            {
                return this.numeroPrenotazioneField;
            }
            set
            {
                this.numeroPrenotazioneField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string IdRichiestaOE
        {
            get
            {
                return this.idRichiestaOEField;
            }
            set
            {
                this.idRichiestaOEField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public CodiceDescrizioneType RepartoRichiedente
        {
            get
            {
                return this.repartoRichiedenteField;
            }
            set
            {
                this.repartoRichiedenteField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public StatoRichiestaEnum StatoRichiesta
        {
            get
            {
                return this.statoRichiestaField;
            }
            set
            {
                this.statoRichiestaField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public CodiceDescrizioneType TipoRichiesta
        {
            get
            {
                return this.tipoRichiestaField;
            }
            set
            {
                this.tipoRichiestaField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public CodiceDescrizioneType Priorita
        {
            get
            {
                return this.prioritaField;
            }
            set
            {
                this.prioritaField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string TestoReferto
        {
            get
            {
                return this.testoRefertoField;
            }
            set
            {
                this.testoRefertoField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public CodiceDescrizioneType MedicoRefertante
        {
            get
            {
                return this.medicoRefertanteField;
            }
            set
            {
                this.medicoRefertanteField = value;
            }
        }

        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("Attributo", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        public AttributoType[] Attributi
        {
            get
            {
                return this.attributiField;
            }
            set
            {
                this.attributiField = value;
            }
        }

        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("Prestazione", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        public PrestazioneType[] Prestazioni
        {
            get
            {
                return this.prestazioniField;
            }
            set
            {
                this.prestazioniField = value;
            }
        }

        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("Allegato", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        public AllegatoType[] Allegati
        {
            get
            {
                return this.allegatiField;
            }
            set
            {
                this.allegatiField = value;
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.progel.it/BT/Dwh/FileRefertoInput/1.0")]
    public partial class CodiceDescrizioneType
    {

        private string codiceField;

        private string descrizioneField;

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Codice
        {
            get
            {
                return this.codiceField;
            }
            set
            {
                this.codiceField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Descrizione
        {
            get
            {
                return this.descrizioneField;
            }
            set
            {
                this.descrizioneField = value;
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.progel.it/BT/Dwh/FileRefertoInput/1.0")]
    public partial class AllegatoType
    {

        private string idEsternoField;

        private string nomeFileField;

        private System.DateTime dataFileField;

        private bool dataFileFieldSpecified;

        private string descrizioneField;

        private int posizioneField;

        private bool posizioneFieldSpecified;

        private string mimeTypeField;

        private byte[] mimeDataField;

        private AttributoType[] attributiField;

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string IdEsterno
        {
            get
            {
                return this.idEsternoField;
            }
            set
            {
                this.idEsternoField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NomeFile
        {
            get
            {
                return this.nomeFileField;
            }
            set
            {
                this.nomeFileField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public System.DateTime DataFile
        {
            get
            {
                return this.dataFileField;
            }
            set
            {
                this.dataFileField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DataFileSpecified
        {
            get
            {
                return this.dataFileFieldSpecified;
            }
            set
            {
                this.dataFileFieldSpecified = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Descrizione
        {
            get
            {
                return this.descrizioneField;
            }
            set
            {
                this.descrizioneField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int Posizione
        {
            get
            {
                return this.posizioneField;
            }
            set
            {
                this.posizioneField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PosizioneSpecified
        {
            get
            {
                return this.posizioneFieldSpecified;
            }
            set
            {
                this.posizioneFieldSpecified = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MimeType
        {
            get
            {
                return this.mimeTypeField;
            }
            set
            {
                this.mimeTypeField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "base64Binary")]
        public byte[] MimeData
        {
            get
            {
                return this.mimeDataField;
            }
            set
            {
                this.mimeDataField = value;
            }
        }

        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("Attributo", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        public AttributoType[] Attributi
        {
            get
            {
                return this.attributiField;
            }
            set
            {
                this.attributiField = value;
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.progel.it/BT/Dwh/FileRefertoInput/1.0")]
    public partial class AttributoType
    {

        private string nomeField;

        private string valoreField;

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Nome
        {
            get
            {
                return this.nomeField;
            }
            set
            {
                this.nomeField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Valore
        {
            get
            {
                return this.valoreField;
            }
            set
            {
                this.valoreField = value;
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.progel.it/BT/Dwh/FileRefertoInput/1.0")]
    public partial class PrestazioneType
    {

        private string idEsternoField;

        private System.DateTime dataErogazioneField;

        private bool dataErogazioneFieldSpecified;

        private CodiceDescrizioneType prestazioneField;

        private int prestazionePosizioneField;

        private bool prestazionePosizioneFieldSpecified;

        private CodiceDescrizioneType sezioneField;

        private int sezionePosizioneField;

        private bool sezionePosizioneFieldSpecified;

        private CodiceDescrizioneType gravitaField;

        private string risultatoField;

        private float risultatoNumericoField;

        private bool risultatoNumericoFieldSpecified;

        private object valoriRiferimentoField;

        private string commentiField;

        private AttributoType[] attributiField;

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string IdEsterno
        {
            get
            {
                return this.idEsternoField;
            }
            set
            {
                this.idEsternoField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public System.DateTime DataErogazione
        {
            get
            {
                return this.dataErogazioneField;
            }
            set
            {
                this.dataErogazioneField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DataErogazioneSpecified
        {
            get
            {
                return this.dataErogazioneFieldSpecified;
            }
            set
            {
                this.dataErogazioneFieldSpecified = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public CodiceDescrizioneType Prestazione
        {
            get
            {
                return this.prestazioneField;
            }
            set
            {
                this.prestazioneField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int PrestazionePosizione
        {
            get
            {
                return this.prestazionePosizioneField;
            }
            set
            {
                this.prestazionePosizioneField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PrestazionePosizioneSpecified
        {
            get
            {
                return this.prestazionePosizioneFieldSpecified;
            }
            set
            {
                this.prestazionePosizioneFieldSpecified = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public CodiceDescrizioneType Sezione
        {
            get
            {
                return this.sezioneField;
            }
            set
            {
                this.sezioneField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int SezionePosizione
        {
            get
            {
                return this.sezionePosizioneField;
            }
            set
            {
                this.sezionePosizioneField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SezionePosizioneSpecified
        {
            get
            {
                return this.sezionePosizioneFieldSpecified;
            }
            set
            {
                this.sezionePosizioneFieldSpecified = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public CodiceDescrizioneType Gravita
        {
            get
            {
                return this.gravitaField;
            }
            set
            {
                this.gravitaField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Risultato
        {
            get
            {
                return this.risultatoField;
            }
            set
            {
                this.risultatoField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public float RisultatoNumerico
        {
            get
            {
                return this.risultatoNumericoField;
            }
            set
            {
                this.risultatoNumericoField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool RisultatoNumericoSpecified
        {
            get
            {
                return this.risultatoNumericoFieldSpecified;
            }
            set
            {
                this.risultatoNumericoFieldSpecified = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public object ValoriRiferimento
        {
            get
            {
                return this.valoriRiferimentoField;
            }
            set
            {
                this.valoriRiferimentoField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Commenti
        {
            get
            {
                return this.commentiField;
            }
            set
            {
                this.commentiField = value;
            }
        }

        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("Attributo", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        public AttributoType[] Attributi
        {
            get
            {
                return this.attributiField;
            }
            set
            {
                this.attributiField = value;
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.progel.it/BT/Dwh/FileRefertoInput/1.0")]
    public partial class IndirizzoType
    {

        private string comuneCodiceField;

        private string comuneNomeField;

        private string indirizzoField;

        private string localitaField;

        private string cAPField;

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ComuneCodice
        {
            get
            {
                return this.comuneCodiceField;
            }
            set
            {
                this.comuneCodiceField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ComuneNome
        {
            get
            {
                return this.comuneNomeField;
            }
            set
            {
                this.comuneNomeField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Indirizzo
        {
            get
            {
                return this.indirizzoField;
            }
            set
            {
                this.indirizzoField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Localita
        {
            get
            {
                return this.localitaField;
            }
            set
            {
                this.localitaField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CAP
        {
            get
            {
                return this.cAPField;
            }
            set
            {
                this.cAPField = value;
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.progel.it/BT/Dwh/FileRefertoInput/1.0")]
    public partial class GeneralitaType
    {

        private string codiceFiscaleField;

        private string cognomeField;

        private string nomeField;

        private System.Nullable<System.DateTime> dataNascitaField;

        private bool dataNascitaFieldSpecified;

        private string sessoField;

        private string codiceSanitarioField;

        private string comuneNascitaCodiceField;

        private string comuneNascitaNomeField;

        private string nazionalitaCodiceField;

        private string nazionalitaNomeField;

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CodiceFiscale
        {
            get
            {
                return this.codiceFiscaleField;
            }
            set
            {
                this.codiceFiscaleField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Cognome
        {
            get
            {
                return this.cognomeField;
            }
            set
            {
                this.cognomeField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Nome
        {
            get
            {
                return this.nomeField;
            }
            set
            {
                this.nomeField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public System.Nullable<System.DateTime> DataNascita
        {
            get
            {
                return this.dataNascitaField;
            }
            set
            {
                this.dataNascitaField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DataNascitaSpecified
        {
            get
            {
                return this.dataNascitaFieldSpecified;
            }
            set
            {
                this.dataNascitaFieldSpecified = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Sesso
        {
            get
            {
                return this.sessoField;
            }
            set
            {
                this.sessoField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CodiceSanitario
        {
            get
            {
                return this.codiceSanitarioField;
            }
            set
            {
                this.codiceSanitarioField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ComuneNascitaCodice
        {
            get
            {
                return this.comuneNascitaCodiceField;
            }
            set
            {
                this.comuneNascitaCodiceField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ComuneNascitaNome
        {
            get
            {
                return this.comuneNascitaNomeField;
            }
            set
            {
                this.comuneNascitaNomeField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NazionalitaCodice
        {
            get
            {
                return this.nazionalitaCodiceField;
            }
            set
            {
                this.nazionalitaCodiceField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NazionalitaNome
        {
            get
            {
                return this.nazionalitaNomeField;
            }
            set
            {
                this.nazionalitaNomeField = value;
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.progel.it/BT/Dwh/FileRefertoInput/1.0")]
    public partial class PazienteType
    {

        private string idEsternoField;

        private string codiceAnagraficaEroganteField;

        private string nomeAnagraficaEroganteField;

        private GeneralitaType generalitaField;

        private IndirizzoType residenzaField;

        private IndirizzoType domicilioField;

        private AttributoType[] attributiField;

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string IdEsterno
        {
            get
            {
                return this.idEsternoField;
            }
            set
            {
                this.idEsternoField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CodiceAnagraficaErogante
        {
            get
            {
                return this.codiceAnagraficaEroganteField;
            }
            set
            {
                this.codiceAnagraficaEroganteField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NomeAnagraficaErogante
        {
            get
            {
                return this.nomeAnagraficaEroganteField;
            }
            set
            {
                this.nomeAnagraficaEroganteField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public GeneralitaType Generalita
        {
            get
            {
                return this.generalitaField;
            }
            set
            {
                this.generalitaField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public IndirizzoType Residenza
        {
            get
            {
                return this.residenzaField;
            }
            set
            {
                this.residenzaField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public IndirizzoType Domicilio
        {
            get
            {
                return this.domicilioField;
            }
            set
            {
                this.domicilioField = value;
            }
        }

        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("Attributo", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        public AttributoType[] Attributi
        {
            get
            {
                return this.attributiField;
            }
            set
            {
                this.attributiField = value;
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.progel.it/BT/Dwh/FileRefertoInput/1.0")]
    public enum StatoRichiestaEnum
    {

        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Item0,

        [System.Xml.Serialization.XmlEnumAttribute("1")]
        Item1,

        [System.Xml.Serialization.XmlEnumAttribute("2")]
        Item2,

        [System.Xml.Serialization.XmlEnumAttribute("3")]
        Item3,
    }
}
