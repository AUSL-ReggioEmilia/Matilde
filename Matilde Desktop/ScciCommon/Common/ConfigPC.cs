using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace UnicodeSrl.Scci
{
    [Serializable()]
    public class ConfigPC
    {

        public const string SECTION_DEBUG = "01";
        public const string SECTION_RTF = "02";
        public const string SECTION_FONT = "03";
        public const string SECTION_CHIAMATA_NUM = "04";
        public const string SECTION_FILTRO_AGENDE_APP = "05";
        public const string SECTION_IPOVEDENTE = "06";

        public ConfigDebug configDebug { get; set; }
        public ConfigRtf configRtf { get; set; }
        public ConfigFont configFont { get; set; }
        public ConfigChiamataNumeri configChiamataNumeri { get; set; }
        public ConfigIpovedente configIpovedente { get; set; }

        public ConfigPC()
        {
            this.configDebug = new ConfigDebug();
            this.configDebug.EnableTrace = false;
            this.configRtf = new ConfigRtf();
            this.configRtf.Zoom = 1;
            this.configFont = new ConfigFont();
            this.configFont.Coefficiente = 1;
            this.configChiamataNumeri = new ConfigChiamataNumeri();
            this.configChiamataNumeri.CodiceAgenda = "";
            this.configChiamataNumeri.ApriCartellaSuChiamata = false;

            this.configIpovedente = new ConfigIpovedente();
            this.configIpovedente.Ipovedente = false;
        }

    }


    [Serializable()]
    public class ConfigDebug
    {
        public bool EnableTrace { get; set; }

        public ConfigDebug()
        {
            this.EnableTrace = false;
        }

        public string getXML()
        {
            string sXML = "";
            XmlSerializer mySerializer = new XmlSerializer(this.GetType());
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            mySerializer.Serialize(ms, this);
            sXML = System.Text.ASCIIEncoding.UTF8.GetString(ms.ToArray());

            ms.Close();
            ms.Dispose();

            return sXML;
        }

    }

    [Serializable()]
    public class ConfigRtf
    {
        public float Zoom { get; set; }

        public ConfigRtf()
        {
            this.Zoom = 1;
        }

        public string getXML()
        {
            string sXML = "";
            XmlSerializer mySerializer = new XmlSerializer(this.GetType());
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            mySerializer.Serialize(ms, this);
            sXML = System.Text.ASCIIEncoding.UTF8.GetString(ms.ToArray());

            ms.Close();
            ms.Dispose();

            return sXML;
        }

    }

    [Serializable()]
    public class ConfigFont
    {
        public float Coefficiente { get; set; }

        public ConfigFont()
        {
            this.Coefficiente = 1;
        }

        public string getXML()
        {
            string sXML = "";
            XmlSerializer mySerializer = new XmlSerializer(this.GetType());
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            mySerializer.Serialize(ms, this);
            sXML = System.Text.ASCIIEncoding.UTF8.GetString(ms.ToArray());

            ms.Close();
            ms.Dispose();

            return sXML;
        }

    }

    [Serializable()]
    public class ConfigChiamataNumeri
    {
        public string CodiceAgenda { get; set; }
        public bool ApriCartellaSuChiamata { get; set; }

        public ConfigChiamataNumeri()
        {
            this.CodiceAgenda = "";
            this.ApriCartellaSuChiamata = false;
        }

        public string getXML()
        {
            string sXML = "";
            XmlSerializer mySerializer = new XmlSerializer(this.GetType());
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            mySerializer.Serialize(ms, this);
            sXML = System.Text.ASCIIEncoding.UTF8.GetString(ms.ToArray());

            ms.Close();
            ms.Dispose();

            return sXML;
        }

    }

    [Serializable()]
    public class ConfigIpovedente
    {
        public bool Ipovedente { get; set; }

        public ConfigIpovedente()
        {
            this.Ipovedente = false;
        }

        public string getXML()
        {
            string sXML = "";
            XmlSerializer mySerializer = new XmlSerializer(this.GetType());
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            mySerializer.Serialize(ms, this);
            sXML = System.Text.ASCIIEncoding.UTF8.GetString(ms.ToArray());

            ms.Close();
            ms.Dispose();

            return sXML;
        }

    }

}
