using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UnicodeSrl.ASMN.WebNotificationService.DWH
{

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.progel.it/WCF/Dwh/SoapTypesOutput/1.0")]
    public partial class PazienteType
    {

        private string idSacField;

        private string codiceAnagraficaEroganteField;

        private string nomeAnagraficaEroganteField;

        private GeneralitaType generalitaField;

        private IndirizzoType residenzaField;

        private IndirizzoType domicilioField;

        private UslType uslResidenzaField;

        private UslType uslAssistenzaField;

        private MedicoDiBaseType medicoDiBaseField;

        private AttributoType[] attributiField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string IdSac
        {
            get
            {
                return this.idSacField;
            }
            set
            {
                this.idSacField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
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

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
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

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
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

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
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

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
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

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public UslType UslResidenza
        {
            get
            {
                return this.uslResidenzaField;
            }
            set
            {
                this.uslResidenzaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public UslType UslAssistenza
        {
            get
            {
                return this.uslAssistenzaField;
            }
            set
            {
                this.uslAssistenzaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public MedicoDiBaseType MedicoDiBase
        {
            get
            {
                return this.medicoDiBaseField;
            }
            set
            {
                this.medicoDiBaseField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(IsNullable = true)]
        [System.Xml.Serialization.XmlArrayItemAttribute("Attributo")]
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

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.progel.it/WCF/Dwh/SoapTypesOutput/1.0")]
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

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
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

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
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

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
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

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
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

        /// <remarks/>
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

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
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

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
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

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
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

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
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

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
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

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
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

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.progel.it/WCF/Dwh/SoapTypesOutput/1.0")]
    public partial class CodiceDescrizioneType
    {

        private string codiceField;

        private string descrizioneField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
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

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
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

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.progel.it/WCF/Dwh/SoapTypesOutput/1.0")]
    public partial class AttributoType
    {

        private string nomeField;

        private string valoreField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
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

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
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

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.progel.it/WCF/Dwh/SoapTypesOutput/1.0")]
    public partial class MedicoDiBaseType
    {

        private string codiceFiscaleField;

        private string cognomeNomeField;

        private string distrettoField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
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

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string CognomeNome
        {
            get
            {
                return this.cognomeNomeField;
            }
            set
            {
                this.cognomeNomeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Distretto
        {
            get
            {
                return this.distrettoField;
            }
            set
            {
                this.distrettoField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.progel.it/WCF/Dwh/SoapTypesOutput/1.0")]
    public partial class UslType
    {

        private string codiceField;

        private System.Nullable<byte> posizioneAssistitoField;

        private bool posizioneAssistitoFieldSpecified;

        private string regioneCodiceField;

        private string comuneCodiceField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
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

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<byte> PosizioneAssistito
        {
            get
            {
                return this.posizioneAssistitoField;
            }
            set
            {
                this.posizioneAssistitoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PosizioneAssistitoSpecified
        {
            get
            {
                return this.posizioneAssistitoFieldSpecified;
            }
            set
            {
                this.posizioneAssistitoFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string RegioneCodice
        {
            get
            {
                return this.regioneCodiceField;
            }
            set
            {
                this.regioneCodiceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
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
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.progel.it/WCF/Dwh/SoapTypesOutput/1.0")]
    public partial class IndirizzoType
    {

        private string comuneCodiceField;

        private string comuneNomeField;

        private string indirizzoField;

        private string localitaField;

        private string cAPField;

        private System.Nullable<System.DateTime> dataDecorrenzaField;

        private bool dataDecorrenzaFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
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

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
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

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
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

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
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

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
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

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<System.DateTime> DataDecorrenza
        {
            get
            {
                return this.dataDecorrenzaField;
            }
            set
            {
                this.dataDecorrenzaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DataDecorrenzaSpecified
        {
            get
            {
                return this.dataDecorrenzaFieldSpecified;
            }
            set
            {
                this.dataDecorrenzaFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.17929")]
    public delegate void ListenDataCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
}