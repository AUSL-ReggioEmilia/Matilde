using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace UnicodeSrl.ScciCore.Common.ProtocolliPrescrizioni
{
    public static class SerializerProtocolli
    {
        public static string SerializeModelli(List<ModelloPrescrizione> modelli)
        {

            string sXML = string.Empty;

            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(List<ModelloPrescrizione>));
                using (StringWriter sw = new StringWriter())
                {
                    ser.Serialize(sw, modelli);
                    sXML = sw.ToString();
                }
                ser = null;
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                sXML = string.Empty;
            }

            return sXML;
        }

        public static List<ModelloPrescrizione> DeSerializeModelli(string xmlModelli)
        {
            List<ModelloPrescrizione> lRet = new List<ModelloPrescrizione>();

            try
            {
                if (xmlModelli != null && xmlModelli != string.Empty)
                {
                    XmlSerializer ser = new XmlSerializer(typeof(List<ModelloPrescrizione>));
                    using (StringReader sw = new StringReader(xmlModelli))
                    {
                        lRet = (List<ModelloPrescrizione>)ser.Deserialize(sw);
                    }
                    ser = null;
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                lRet = new List<ModelloPrescrizione>();
            }

            return lRet;
        }

    }
}
