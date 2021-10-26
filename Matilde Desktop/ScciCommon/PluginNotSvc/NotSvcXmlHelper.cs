using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace UnicodeSrl.Scci.PluginNotSvc
{
    public static class NotSvcXmlHelper
    {
        public static string Serialize<T>(T sourceObject, string newRoot = null, string nsPrefix = null, string nsUrl = null)
        {
            string sret = string.Empty;

            try
            {
                Type tSourceObj = typeof(T);

                XmlSerializer serializer = new XmlSerializer(tSourceObj);
                using (MemoryStream mems = new MemoryStream())
                {
                    serializer.Serialize(mems, sourceObject);
                    mems.Position = 0;

                    XDocument xdocSource = XDocument.Load(mems);

                    if (newRoot != null)
                    {
                        XElement xeWithRoot = new XElement(newRoot, xdocSource.Root);
                        xdocSource = new XDocument(xeWithRoot);
                        xeWithRoot = null;
                    }

                    if ((nsPrefix != null) && (nsUrl != null))
                    {
                        XAttribute xAtt = new XAttribute(XNamespace.Xmlns + nsPrefix, nsUrl);
                        xdocSource.Root.Add(xAtt);
                    }

                    sret = xdocSource.ToString();
                    xdocSource = null;

                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                sret = string.Empty;
            }


            return sret;
        }

        public static string Serialize(object objectInstance)
        {
            Type tSourceObj = objectInstance.GetType();
            XmlSerializer serializer = new XmlSerializer(tSourceObj);
            StringBuilder sb = new StringBuilder();

            using (TextWriter writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, objectInstance);
            }

            return sb.ToString();
        }
    }
}
