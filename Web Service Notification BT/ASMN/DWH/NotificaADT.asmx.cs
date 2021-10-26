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



namespace UnicodeSrl.ASMN.WebNotificationService.DWH
{
    /// <summary>
    /// Summary description for NotificaADT
    /// </summary>
    [WebService(Name = "BasicHttpBinding_IService", Namespace = "http://schemas.progel.it/WCF/Dwh/SoapEventoOutput/1.0")]
    //[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class NotificaADT : INotificaADT
    {

        public void ListenData(SoapEventoType value)
        {
            try
            {
                DataLog _Dl = new DataLog();
                _Dl.ConnectionString = Common.ConnString;
                _Dl.CodEvento = Common.LogDWHNotificaADT;
                _Dl.CodUtente = Common.LogUtente;

                if (_Dl.CheckEvento(Common.LogDWHNotificaADT) == true)
                {
                    string xmlString = null;
                    XmlSerializer serializer = new XmlSerializer(value.GetType());
                    using (StringWriter writer = new StringWriter())
                    {
                        serializer.Serialize(writer, value);
                        xmlString = writer.ToString();
                    }

                    _Dl.EseguiLog(DataLog.enumTipoOperazione.Modifica, Common.LogDWHNotificaADT, "", xmlString);
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
