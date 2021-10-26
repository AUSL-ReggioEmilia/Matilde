using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using UDL;
using UnicodeSrl.Framework.Data;

namespace UnicodeSrl.ScciManagement
{
    public class Configurazione
    {

        public DialogResult OpenConnection()
        {

            try
            {

                ConnectionStringDialog fd = new ConnectionStringDialog();
                fd.Provider = "System.Data.SqlClient";
                fd.ConnectionString = this.ConnectionString;

                if (fd.ShowDialog() == DialogResult.OK)
                {
                    this.ConnectionString = fd.ConnectionString;
                    return DialogResult.OK;
                }
                else
                {
                    return DialogResult.Cancel;
                }

            }
            catch (Exception)
            {
                return DialogResult.Cancel;
            }

        }

        public bool TestConnection()
        {

            try
            {
                string sconnstr = this.ConnectionString;

                using (FwDataConnection fdc = new FwDataConnection(sconnstr))
                {
                    fdc.Open();
                    return true;
                }

            }
            catch (Exception)
            {
                return false;
            }

        }

        internal string FileConfig
        {
            get { return Application.StartupPath + "\\Config.xml"; }
        }

        public string ConnectionString
        {
            get { return this.GetXmlValue(this.FileConfig, "Configurazione", "ConnectionString"); }
            set { this.SetXmlValue(this.FileConfig, "Configurazione", "ConnectionString", value); }
        }

        internal string GetXmlValue(string File, string Section, string Key)
        {

            try
            {

                System.Xml.XmlDocument oXmlDoc = new System.Xml.XmlDocument();
                oXmlDoc.Load(File);
                string nodeKey = "//" + Section + "/add[@key='" + Key + "']/@value";
                if ((oXmlDoc.DocumentElement.SelectSingleNode(nodeKey) != null))
                {
                    Scci.Encryption ocrypt = new UnicodeSrl.Scci.Encryption(Scci.Statics.EncryptionStatics.GC_DECRYPTKEY, Scci.Statics.EncryptionStatics.GC_INITVECTOR);
                    string sreturn = ocrypt.DecryptString(oXmlDoc.DocumentElement.SelectSingleNode(nodeKey).Value);
                    ocrypt = null;
                    return sreturn;
                }
                else
                {
                    return "";
                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "GetXmlValue", "Configurazione.cs");
                return "";
            }

        }

        internal void SetXmlValue(string File, string Section, string Key, string Value)
        {

            try
            {
                string sencryptedvalue = "";
                Scci.Encryption ocrypt = new UnicodeSrl.Scci.Encryption(Scci.Statics.EncryptionStatics.GC_DECRYPTKEY, Scci.Statics.EncryptionStatics.GC_INITVECTOR);
                sencryptedvalue = ocrypt.EncryptString(Value);
                ocrypt = null;

                System.Xml.XmlDocument oXmlDoc = new System.Xml.XmlDocument();
                oXmlDoc.Load(File);
                string nodeKey = "//" + Section + "/add[@key='" + Key + "']/@value";

                if ((oXmlDoc.DocumentElement.SelectSingleNode(nodeKey) != null))
                {
                    oXmlDoc.DocumentElement.SelectSingleNode(nodeKey).Value = sencryptedvalue;
                }
                else
                {
                    System.Xml.XmlElement @add = oXmlDoc.CreateElement("add");
                    System.Xml.XmlAttribute attrkey = oXmlDoc.CreateAttribute("key");
                    attrkey.Value = Key;
                    System.Xml.XmlAttribute val = oXmlDoc.CreateAttribute("value");
                    val.Value = sencryptedvalue;
                    @add.Attributes.Append(attrkey);
                    @add.Attributes.Append(val);
                    oXmlDoc.DocumentElement.SelectSingleNode("//" + Section).AppendChild(@add);
                }
                oXmlDoc.Save(File);

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "SetXmlValue", "Configurazione.cs");
            }

        }

        public string CurrentUser()
        {
            return UnicodeSrl.Framework.Utility.Windows.CurrentUser();
        }

        public string CurrentUserDes()
        {
            return @"Utente : " + CurrentUser();
        }

        public string CurrentPC()
        {
            return Environment.MachineName;
        }

        public string CurrentPCDes()
        {
            return @"NOME PC : " + Environment.MachineName;
        }

        public string GetPropertyConnectionString()
        {

            try
            {

                string sDataSource = "";
                string sInitialCatalog = "";
                string[] sSplit = null;
                sSplit = this.ConnectionString.Split(';');

                for (int x = 0; x <= sSplit.GetLength(0) - 1; x++)
                {
                    if (Convert.ToString(sSplit[x].ToUpper()).IndexOf("Data Source".ToUpper()) != -1)
                    {
                        sDataSource = "Server : " + sSplit[x].Split('=').GetValue(1);
                    }
                    else if (Convert.ToString(sSplit[x].ToUpper()).IndexOf("Initial Catalog".ToUpper()) != -1)
                    {
                        sInitialCatalog = "Database : " + sSplit[x].Split('=').GetValue(1);
                    }
                }

                return "[" + sDataSource + " - " + sInitialCatalog + "]";

            }
            catch (Exception)
            {
                return "";
            }

        }

    }
}
