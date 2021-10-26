using System;
using System.Data;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnicodeSrl.NotSvc;


namespace UnicodeSrl.ASMN.WebNotificationService.OE
{
    [WebService(Name = "BasicHttpBinding_IService", Namespace = "http://schemas.progel.it/WCF/OE/CmsRichieste/1.0")]
    [ToolboxItem(false)]
    public class NotificaRichiesta : IBasicHttpBinding_INotificaRichiesta
    {

        public void ListenData(RichiestaType value)
        {
            try
            {
                DataLog _Dl = new DataLog();
                _Dl.ConnectionString = Common.ConnString;
                _Dl.CodEvento = Common.LogOENotificaRichiesta;
                _Dl.CodUtente = Common.LogUtente;

                if (_Dl.CheckEvento(Common.LogOENotificaRichiesta) == true)
                {
                    string xmlString = null;
                    XmlSerializer serializer = new XmlSerializer(value.GetType());
                    using (StringWriter writer = new StringWriter())
                    {
                        serializer.Serialize(writer, value);
                        xmlString = writer.ToString();
                    }
                    _Dl.EseguiLog(DataLog.enumTipoOperazione.Modifica, Common.LogOENotificaRichiesta, "", xmlString);
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
