using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using UnicodeSrl.NotSvc;

namespace NotificationWebSvc.ASMN.OE
{
    /// <summary>
    /// Summary description for NotificaStato
    /// </summary>
    [WebService(Name = "BasicHttpBinding_IService", Namespace = "http://schemas.progel.it/WCF/OE/StatoRichiedenteGenerico/1.0")]
    public class NotificaStato : IBasicHttpBinding_IServiceNS
    {

        public void ListenData(StatoType value)
        {
            try
            {
                DataLog _Dl = new DataLog();
                _Dl.ConnectionString = Common.ConnString;
                _Dl.CodEvento = Common.LogOENotificaStato;
                _Dl.CodUtente = Common.LogUtente;

                if (_Dl.CheckEvento(Common.LogOENotificaStato) == true)
                {
                    string xmlString = null;
                    XmlSerializer serializer = new XmlSerializer(value.GetType());
                    using (StringWriter writer = new StringWriter())
                    {
                        serializer.Serialize(writer, value);
                        xmlString = writer.ToString();
                    }
                    _Dl.EseguiLog(DataLog.enumTipoOperazione.Modifica, Common.LogOENotificaStato, "", xmlString);
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
                throw new SoapException(ex.Message, SoapException.ServerFaultCode, ex);
            }
        }
    }
}
