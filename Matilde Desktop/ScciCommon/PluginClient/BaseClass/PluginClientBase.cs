using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnicodeSrl.Framework.Extension;

namespace UnicodeSrl.Scci.PluginClient
{
    public abstract class PluginClientBase : IPluginClient
    {
        public PluginClientBase()
        {
            this.Risposta = new Risposta();
        }

        public Risposta Risposta { get; protected set; }

        protected Dictionary<string, object> Parameters { get; private set; }

        protected String Modalita { get; private set; }


        public virtual Task<Risposta> AsyncExecute(object[] parameters, string modalita)
        {
            throw new NotImplementedException();
        }

        public virtual Risposta Execute(object[] parameters, string modalita)
        {
            this.SetProperties(parameters, modalita);

            return new Risposta();
        }

        protected void SetProperties(object[] parameters, string modalita)
        {
            if (parameters.Length == 0) throw new Exception("I parametri non sono validi o sono nulli");
            if (parameters[0].GetType() != typeof(Dictionary<string, object>)) throw new Exception("I parametri non sono validi o sono nulli");

            this.Parameters = (Dictionary<string, object>)parameters.First();
            this.Modalita = modalita;

        }

        protected T ValueFromParam<T>(Dictionary<string, object> dicParams, string key, T defaultValue)
        {
            object outvalue = null;
            T retValue = defaultValue;

            dicParams.TryGetValue(key, out outvalue);
            if (outvalue == null) return defaultValue;

            if (typeof(T) == typeof(Boolean))
            {
                bool defBool = Convert.ToBoolean(defaultValue);
                outvalue = this.evaluteBooleanString(Convert.ToString(outvalue), defBool);
            }

            retValue = (T)Convert.ChangeType(outvalue, typeof(T));
            return retValue;
        }

        protected T ValueFromParam<T>(string key, T defaultValue)
        {
            return this.ValueFromParam(this.Parameters, key, defaultValue);
        }

        protected T ValueFromModXml<T>(string key, T defaultValue)
        {
            if (this.Modalita.IsNullOrEmpty()) return defaultValue;

            XDocument doc = XDocument.Parse(this.Modalita);

            T retValue = defaultValue;
            string outvalue = this.GetParamString(doc, key, null);

            if (outvalue == null) return retValue;

            if (typeof(T) == typeof(Boolean))
            {
                bool defBool = Convert.ToBoolean(defaultValue);
                bool result = this.evaluteBooleanString(outvalue, defBool);
                return (T)Convert.ChangeType(result, typeof(T));
            }
            else
            {
                retValue = (T)Convert.ChangeType(outvalue, typeof(T));
                return retValue;
            }
        }

        private string GetParamString(XDocument doc, string paramName, string defaultForNull)
        {
            List<String> values = this.GetParamValues(doc, paramName);

            if (values.Count == 0) return defaultForNull;
            else return values.First();
        }

        private List<String> GetParamValues(XDocument doc, string paramName)
        {

            List<String> valRes = new List<string>();

            List<XElement> elements = doc.Descendants().Where(p => p.Name.LocalName == paramName).ToList();

            foreach (XElement ele in elements)
            {
                valRes.Add(ele.Value);
            }

            if (valRes.Count == 0)
                return null;
            else
                return valRes;
        }

        private bool evaluteBooleanString(string value, bool defaultValue)
        {
            if (value == null) return defaultValue;
            string valueString = value.ToUpper();

            if (valueString == "0") return false;
            if (valueString == "FALSE") return false;
            if (valueString == "FALSO") return false;
            if (valueString == "NO") return false;
            if (valueString == "N") return false;

            if (valueString == "1") return true;
            if (valueString == "TRUE") return true;
            if (valueString == "VERO") return true;
            if (valueString == "SI") return true;
            if (valueString == "S") return true;

            return defaultValue;
        }

    }
}
