using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;

namespace UnicodeSrl.Scci.PluginClient
{

    public class Configurazione
    {

        XmlDocument _XmlDocument = null;

        public Configurazione(string sParametri)
        {
            _XmlDocument = new XmlDocument();
            if (sParametri != string.Empty)
            {
                _XmlDocument.LoadXml(sParametri);
            }
            else
            {
                _XmlDocument.LoadXml("<Parametri></Parametri>");
            }
        }

        public string Parametro(string key)
        {
            return this.Parametro(key, "");
        }
        public string Parametro(string key, string returnvalue)
        {

            string sRet = returnvalue;
            XmlNodeList oXmlNodeList = _XmlDocument.GetElementsByTagName(key);
            if (oXmlNodeList.Count == 1)
            {
                sRet = oXmlNodeList.Item(0).InnerText;
            }
            return sRet;

        }

        public List<string> ParametroRipetibile(string key)
        {
            return ParametroRipetibile(key, false);
        }
        public List<string> ParametroRipetibile(string key, bool onlynextlevel)
        {

            List<string> list_ret = new List<string>();

            XmlNodeList oXmlNodeList = _XmlDocument.GetElementsByTagName(key);

            for (int i = 0; i < oXmlNodeList.Count; i++)
            {
                if (onlynextlevel)
                {
                    if (oXmlNodeList[i].ParentNode.Name == "Parametri")
                    {
                        list_ret.Add(oXmlNodeList[i].InnerXml);
                    }
                }
                else
                {
                    list_ret.Add(oXmlNodeList[i].InnerXml);
                }
            }

            return list_ret;

        }

    }

}
