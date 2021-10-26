using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace UnicodeSrl.ScciManagement
{
    public class ExportVersion
    {
        public ExportVersion()
        {
            CodVersione = string.Empty;
            Descrizione = string.Empty;
            Fields = new List<ExportField>();
        }

        [XmlElement(ElementName = "Versione")]
        public string CodVersione { get; set; }
        [XmlElement(ElementName = "Descrizione")]
        public string Descrizione { get; set; }
        [XmlElement(ElementName = "Fields")]
        public List<ExportField> Fields { get; set; }
    }

    public class SchedaExportFields
    {

        public SchedaExportFields()
        {
            Fields = new List<ExportField>();
        }

        [XmlElement(ElementName = "Fields")]
        public List<ExportField> Fields { get; set; }
    }

    public class ExportField
    {

        public ExportField()
        {
            Name = string.Empty;
            Value = string.Empty;
            DataType = string.Empty;
        }

        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "Value")]
        public string Value { get; set; }
        [XmlElement(ElementName = "DataType")]
        public string DataType { get; set; }
    }

    public class SchedaExport
    {

        public SchedaExport()
        {
            Codice = string.Empty;
            Descrizione = string.Empty;
            Scheda = new SchedaExportFields();
            Versioni = new List<ExportVersion>();
        }

        [XmlElement(ElementName = "Codice")]
        public string Codice { get; set; }
        [XmlElement(ElementName = "Descrizione")]
        public string Descrizione { get; set; }
        [XmlElement(ElementName = "Scheda")]
        public SchedaExportFields Scheda { get; set; }
        [XmlElement(ElementName = "Versioni")]
        public List<ExportVersion> Versioni { get; set; }
    }

    public class VoceDizionarioExport
    {
        public VoceDizionarioExport()
        {
            Fields = new List<ExportField>();
        }

        [XmlElement(ElementName = "Fields")]
        public List<ExportField> Fields { get; set; }
    }

    public class DizionarioExport
    {

        public DizionarioExport()
        {
            Codice = string.Empty;
            Descrizione = string.Empty;
            Voci = new List<VoceDizionarioExport>();
        }

        [XmlElement(ElementName = "Codice")]
        public string Codice { get; set; }
        [XmlElement(ElementName = "Descrizione")]
        public string Descrizione { get; set; }
        [XmlElement(ElementName = "Voce")]
        public List<VoceDizionarioExport> Voci { get; set; }
    }

    public class DizionariExport
    {
        public DizionariExport()
        {
            Dizionari = new List<DizionarioExport>();
        }

        [XmlElement(ElementName = "Dizionario")]
        public List<DizionarioExport> Dizionari { get; set; }
    }
}
