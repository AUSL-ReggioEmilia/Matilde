using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;


using UnicodeSrl.ScciCore.SvcPrescrizioni;
using UnicodeSrl.ScciCore.SvcRicercaSAC;
using UnicodeSrl.ScciCore.SvcConsensiSAC;
using UnicodeSrl.ScciCore.SvcRefertiDWH;
using UnicodeSrl.ScciCore.SvcRicoveriDWH;
using UnicodeSrl.ScciCore.SvcOrderEntry;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci;

namespace UnicodeSrl.ScciCore.WebSvc
{

    public static class ScciSvcRef
    {

        #region Ricerca Prescrizioni

        internal static ScciPrescrizioniClient GetSvcPrescrizioni()
        {

            bool useHttps = Convert.ToBoolean(Convert.ToInt32(Database.GetConfigTable(EnumConfigTable.WebServiceSCCIHTTPS)));
            Uri serviceUri = new Uri(Database.GetConfigTable(EnumConfigTable.WebServiceSCCI) + @"/ScciPrescrizioni.svc");

            EndpointAddress endpointAddress = new EndpointAddress(serviceUri);

            System.ServiceModel.WSHttpBinding binding = CreateBindingInstance(useHttps);

            ScciPrescrizioniClient client = new ScciPrescrizioniClient(binding, endpointAddress);

            if (useHttps)
            {
                string user = "";
                string pasw = "";
                string domain = "";

                readCredentials(out domain, out user, out pasw);

                client.ClientCredentials.Windows.ClientCredential.Domain = domain;
                client.ClientCredentials.Windows.ClientCredential.UserName = user;
                client.ClientCredentials.Windows.ClientCredential.Password = pasw;

            }

            return client;

        }

        #endregion

        #region Ricerca SAC

        internal static ScciRicercaSACClient GetSvcRicercaSAC()
        {
            bool useHttps = Convert.ToBoolean(Convert.ToInt32(Database.GetConfigTable(EnumConfigTable.WebServiceSCCIHTTPS)));
            Uri serviceUri = new Uri(UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSCCI) + @"/ScciRicercaSAC.svc");

            EndpointAddress endpointAddress = new EndpointAddress(serviceUri);

            System.ServiceModel.WSHttpBinding binding = CreateBindingInstance(useHttps);

            ScciRicercaSACClient client = new ScciRicercaSACClient(binding, endpointAddress);

            if (useHttps)
            {
                string user = "";
                string pasw = "";
                string domain = "";

                readCredentials(out domain, out user, out pasw);

                client.ClientCredentials.Windows.ClientCredential.Domain = domain;
                client.ClientCredentials.Windows.ClientCredential.UserName = user;
                client.ClientCredentials.Windows.ClientCredential.Password = pasw;

            }

            return client;

        }

        #endregion

        #region Consensi

        internal static ScciConsensiSACClient GetSvcConsensiSAC()
        {

            bool useHttps = Convert.ToBoolean(Convert.ToInt32(Database.GetConfigTable(EnumConfigTable.WebServiceSCCIHTTPS)));
            Uri serviceUri = new Uri(UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSCCI) + @"/ScciConsensiSAC.svc");

            EndpointAddress endpointAddress = new EndpointAddress(serviceUri);

            System.ServiceModel.WSHttpBinding binding = CreateBindingInstance(useHttps);

            ScciConsensiSACClient client = new ScciConsensiSACClient(binding, endpointAddress);

            if (useHttps)
            {
                string user = "";
                string pasw = "";
                string domain = "";

                readCredentials(out domain, out user, out pasw);

                client.ClientCredentials.Windows.ClientCredential.Domain = domain;
                client.ClientCredentials.Windows.ClientCredential.UserName = user;
                client.ClientCredentials.Windows.ClientCredential.Password = pasw;

            }

            return client;

        }

        #endregion

        #region Ricerca Referti DWH

        public static ScciRefertiDWHClient GetSvcRefertiDWH()
        {
            bool useHttps = Convert.ToBoolean(Convert.ToInt32(Database.GetConfigTable(EnumConfigTable.WebServiceSCCIHTTPS)));
            Uri serviceUri = new Uri(UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSCCI) + @"/ScciRefertiDWH.svc");



            EndpointAddress endpointAddress = new EndpointAddress(serviceUri);

            System.ServiceModel.WSHttpBinding binding = CreateBindingInstance(useHttps);

            ScciRefertiDWHClient client = new ScciRefertiDWHClient(binding, endpointAddress);

            if (useHttps)
            {
                string user = "";
                string pasw = "";
                string domain = "";

                readCredentials(out domain, out user, out pasw);

                client.ClientCredentials.Windows.ClientCredential.Domain = domain;
                client.ClientCredentials.Windows.ClientCredential.UserName = user;
                client.ClientCredentials.Windows.ClientCredential.Password = pasw;

            }

            return client;

        }


        #endregion

        #region Ricerca Ricoveri DWH

        public static ScciRicoveriDWHClient GetSvcRicoveriDWH()
        {
            bool useHttps = Convert.ToBoolean(Convert.ToInt32(Database.GetConfigTable(EnumConfigTable.WebServiceSCCIHTTPS)));
            Uri serviceUri = new Uri(UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSCCI) + @"/ScciRicoveriDWH.svc");

            EndpointAddress endpointAddress = new EndpointAddress(serviceUri);

            System.ServiceModel.WSHttpBinding binding = CreateBindingInstance(useHttps);

            ScciRicoveriDWHClient client = new ScciRicoveriDWHClient(binding, endpointAddress);

            if (useHttps)
            {
                string user = "";
                string pasw = "";
                string domain = "";

                readCredentials(out domain, out user, out pasw);

                client.ClientCredentials.Windows.ClientCredential.Domain = domain;
                client.ClientCredentials.Windows.ClientCredential.UserName = user;
                client.ClientCredentials.Windows.ClientCredential.Password = pasw;

            }

            return client;

        }

        #endregion

        #region Order Entry

        public static ScciOrderEntryClient GetSvcOrderEntry()
        {
            bool useHttps = Convert.ToBoolean(Convert.ToInt32(Database.GetConfigTable(EnumConfigTable.WebServiceSCCIHTTPS)));

            Uri serviceUri = null;

            serviceUri = new Uri(UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSCCI) + @"/ScciOrderEntry.svc");

            EndpointAddress endpointAddress = new EndpointAddress(serviceUri);

            System.ServiceModel.WSHttpBinding binding = CreateBindingInstance(useHttps);

            ScciOrderEntryClient client = new ScciOrderEntryClient(binding, endpointAddress);

            if (useHttps)
            {
                string user = "";
                string pasw = "";
                string domain = "";

                readCredentials(out domain, out user, out pasw);

                client.ClientCredentials.Windows.ClientCredential.Domain = domain;
                client.ClientCredentials.Windows.ClientCredential.UserName = user;
                client.ClientCredentials.Windows.ClientCredential.Password = pasw;

            }

            return client;

        }

        #endregion

        #region Binding

        public static WSHttpBinding CreateBindingInstance(bool useHttps)
        {

            WSHttpBinding binding = new WSHttpBinding();
            binding.Name = "wsSCCI";
            binding.MessageEncoding = WSMessageEncoding.Mtom;
            binding.MaxReceivedMessageSize = 250000000;

            if (useHttps)
                binding.Security.Mode = SecurityMode.TransportWithMessageCredential;
            else
                binding.Security.Mode = SecurityMode.None;

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

        private static void readCredentials(out string domain, out string user, out string pasw)
        {

            string fullUser = Database.GetConfigTable(EnumConfigTable.WebServiceSCCIUserName);
            string[] _user = fullUser.Split(new String[] { @"\" }, StringSplitOptions.None);

            domain = _user[0];
            user = _user[1];

            Encryption enc = new UnicodeSrl.Scci.Encryption(EncryptionStatics.GC_DECRYPTKEY, EncryptionStatics.GC_INITVECTOR);

            pasw = enc.DecryptString(Database.GetConfigTable(EnumConfigTable.WebServiceSCCIPassword));

            enc = null;

        }

        #endregion

    }
}
