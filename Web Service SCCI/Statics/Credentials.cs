using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Scci;
using System.ServiceModel;

namespace WsSCCI
{
    public static class Credentials
    {

        public enum enumTipoCredenziali
        {
            SAC = 1,
            OE = 2,
            DWH = 3
        }

        #region Credenziali per WS ASMX

        public static System.Net.NetworkCredential CredenzialiScci(enumTipoCredenziali tipocredenziali)
        {
            string wsusername = string.Empty;
            string wsuserdomain = string.Empty;
            string wspassword = string.Empty;
            string stemp = string.Empty;

                        Encryption ocrypt = new Encryption(UnicodeSrl.Scci.Statics.EncryptionStatics.GC_DECRYPTKEY, UnicodeSrl.Scci.Statics.EncryptionStatics.GC_INITVECTOR);

            try
            {
                switch (tipocredenziali)
                {
                    case enumTipoCredenziali.DWH:
                        stemp = UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceDWHUserName);
                        wspassword = ocrypt.DecryptString(UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceDWHPassword));
                        break;
                    case enumTipoCredenziali.OE:
                        stemp = UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceOEUserName);
                        wspassword = ocrypt.DecryptString(UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceOEPassword));
                        break;
                    case enumTipoCredenziali.SAC:
                        stemp = UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSACUserName);
                        wspassword = ocrypt.DecryptString(UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSACPassword));
                        break;
                }

                wsusername = stemp.Split(@"\".ToCharArray())[1];
                wsuserdomain = stemp.Split(@"\".ToCharArray())[0];

                return new System.Net.NetworkCredential(wsusername, wspassword, wsuserdomain);

            }
            catch
            {
                return null;
            }
            finally
            {
                ocrypt = null;
            }
        }

        #endregion

        #region Credenziali per WS WCF

        public static CredencialsWCF CredenzialiScciWCF(enumTipoCredenziali tipocredenziali)
        {
            string stemp = string.Empty;

            CredencialsWCF oRet = new CredencialsWCF();

                        Encryption ocrypt = new Encryption(UnicodeSrl.Scci.Statics.EncryptionStatics.GC_DECRYPTKEY, UnicodeSrl.Scci.Statics.EncryptionStatics.GC_INITVECTOR);

            try
            {
                switch (tipocredenziali)
                {
                    case enumTipoCredenziali.DWH:
                        stemp = UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceDWHUserName);
                        oRet.Password = ocrypt.DecryptString(UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceDWHPassword));
                        oRet.Url = UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceDWH);
                        break;
                    case enumTipoCredenziali.OE:
                        stemp = UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceOEUserName);
                        oRet.Password = ocrypt.DecryptString(UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceOEPassword));
                        oRet.Url = UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceOE);
                        break;
                    case enumTipoCredenziali.SAC:
                        stemp = UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSACUserName);
                        oRet.Password = ocrypt.DecryptString(UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSACPassword));
                        oRet.Url = UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSAC);
                        break;
                }

                oRet.UserName = stemp.Split(@"\".ToCharArray())[1];
                oRet.Domain = stemp.Split(@"\".ToCharArray())[0];

            }
            catch
            {
                oRet.Password = string.Empty;
                oRet.UserName = string.Empty;
                oRet.Url = string.Empty;
                oRet.Domain = string.Empty;
            }
            finally
            {
                ocrypt = null;
            }

            return oRet;

        }

                                        internal static BasicHttpBinding CreateBindingInstance(enumTipoCredenziali tipocredenziali)
        {
            BasicHttpBinding binding = new BasicHttpBinding();

            switch (tipocredenziali)
            {
                case enumTipoCredenziali.DWH:
                    break;
                case enumTipoCredenziali.OE:
                    binding.Name = "BasicHttpBinding_IOrderEntryV1";
                    break;
                case enumTipoCredenziali.SAC:
                    break;
            }

            binding.MessageEncoding = WSMessageEncoding.Text;
            binding.MaxReceivedMessageSize = 250000000;
            binding.Security.Mode = BasicHttpSecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;

            binding.CloseTimeout = new TimeSpan(10, 0, 0);
            binding.OpenTimeout = new TimeSpan(10, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(10, 0, 0);
            binding.SendTimeout = new TimeSpan(10, 0, 0);

            binding.ReaderQuotas.MaxDepth = 2147483647;
            binding.ReaderQuotas.MaxStringContentLength = 2147483647;
            binding.ReaderQuotas.MaxArrayLength = 2147483647;
            binding.ReaderQuotas.MaxBytesPerRead = 2147483647;
            binding.ReaderQuotas.MaxNameTableCharCount = 2147483647;

            binding.UseDefaultWebProxy = true;
            return binding;
        }

        #endregion

    }

    public class CredencialsWCF
    {

        public CredencialsWCF()
        {
            UserName = string.Empty;
            Password = string.Empty;
            Domain = string.Empty;
            Url = string.Empty;
        }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Domain { get; set; }

        public string Url { get; set; }

    }

}