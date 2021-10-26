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


namespace UnicodeSrl.ASMN.WebNotificationService.SAC
{
    /// <summary>
    /// Summary description for NotificaPaziente
    /// </summary>
    [WebService(Name = "NotificaPaziente", Namespace = "http://SAC.BT.Paziente.WS.Output.InvioPaziente/v1.0.0.0/")]
    //[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class NotificaPaziente : IBasicHttpBinding_NorificaPaziente
    {

        public void ListenData(SoapPazienteType value)
        {
            try
            {
                DataLog _Dl = new DataLog();
                _Dl.ConnectionString = Common.ConnString;
                _Dl.CodEvento = Common.LogSACNotificaPaziente;
                _Dl.CodUtente = Common.LogUtente;

                if (_Dl.CheckEvento(Common.LogSACNotificaPaziente) == true)
                {
                    string xmlString = null;
                    XmlSerializer serializer = new XmlSerializer(value.GetType());
                    using (StringWriter writer = new StringWriter())
                    {
                        serializer.Serialize(writer, value);
                        xmlString = writer.ToString();
                    }
                    _Dl.EseguiLog(DataLog.enumTipoOperazione.Modifica, Common.LogSACNotificaPaziente, "", xmlString);
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
