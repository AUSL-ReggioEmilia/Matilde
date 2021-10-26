using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using UDL;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Framework.Diagnostics;

namespace UnicodeSrl.ScciEasy
{

    public class Configurazione
    {

        public DialogResult OpenConnection()
        {

            try
            {

                ConnectionStringDialog fd = new ConnectionStringDialog
                {
                    Provider = "Microsoft.Data.SqlClient",
                    ConnectionString = this.ConnectionString
                };

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
                using (FwDataConnection fdc = new FwDataConnection(this.ConnectionString))
                {
                    fdc.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                DiagnosticStatics.AddDebugInfo(ex);
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
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
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
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

    }
}
