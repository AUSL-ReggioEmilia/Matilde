using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnicodeSrl.Scci.Enums;

namespace UnicodeSrl.Scci.Statics
{
    public static class XmlProcs
    {

        public static string CheckXMLEncoding(string vsXmlInputString, EnumXMLEncoding vTargetXmlEncoding)
        {

            String sRet = vsXmlInputString;

            switch (vTargetXmlEncoding)
            {

                case EnumXMLEncoding.UTF_7:
                    sRet = sRet.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", @"<?xml version=""1.0"" encoding=""" + GetEncodingString(vTargetXmlEncoding) + @"""?>");
                    sRet = sRet.Replace(@"<?xml version=""1.0"" encoding=""utf-16""?>", @"<?xml version=""1.0"" encoding=""" + GetEncodingString(vTargetXmlEncoding) + @"""?>");
                    sRet = sRet.Replace(@"<?xml version=""1.0"" encoding=""utf-32""?>", @"<?xml version=""1.0"" encoding=""" + GetEncodingString(vTargetXmlEncoding) + @"""?>");
                    break;

                case EnumXMLEncoding.UTF_8:
                    sRet = sRet.Replace(@"<?xml version=""1.0"" encoding=""utf-7""?>", @"<?xml version=""1.0"" encoding=""" + GetEncodingString(vTargetXmlEncoding) + @"""?>");
                    sRet = sRet.Replace(@"<?xml version=""1.0"" encoding=""utf-16""?>", @"<?xml version=""1.0"" encoding=""" + GetEncodingString(vTargetXmlEncoding) + @"""?>");
                    sRet = sRet.Replace(@"<?xml version=""1.0"" encoding=""utf-32""?>", @"<?xml version=""1.0"" encoding=""" + GetEncodingString(vTargetXmlEncoding) + @"""?>");
                    break;

                case EnumXMLEncoding.UTF_16:
                    sRet = sRet.Replace(@"<?xml version=""1.0"" encoding=""utf-7""?>", @"<?xml version=""1.0"" encoding=""" + GetEncodingString(vTargetXmlEncoding) + @"""?>");
                    sRet = sRet.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", @"<?xml version=""1.0"" encoding=""" + GetEncodingString(vTargetXmlEncoding) + @"""?>");
                    sRet = sRet.Replace(@"<?xml version=""1.0"" encoding=""utf-32""?>", @"<?xml version=""1.0"" encoding=""" + GetEncodingString(vTargetXmlEncoding) + @"""?>");
                    break;

                case EnumXMLEncoding.UTF_32:
                    sRet = sRet.Replace(@"<?xml version=""1.0"" encoding=""utf-7""?>", @"<?xml version=""1.0"" encoding=""" + GetEncodingString(vTargetXmlEncoding) + @"""?>");
                    sRet = sRet.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", @"<?xml version=""1.0"" encoding=""" + GetEncodingString(vTargetXmlEncoding) + @"""?>");
                    sRet = sRet.Replace(@"<?xml version=""1.0"" encoding=""utf-16""?>", @"<?xml version=""1.0"" encoding=""" + GetEncodingString(vTargetXmlEncoding) + @"""?>");
                    break;

            }

            if (sRet.ToUpper().IndexOf(@"<?xml version=""1.0"" encoding=".ToUpper()) < 0) { sRet = @"<?xml version=""1.0"" encoding=""" + GetEncodingString(vTargetXmlEncoding) + @"""?>" + sRet; }

            return sRet;

        }

        public static string GetEncodingString(EnumXMLEncoding vXmlEncoding)
        {

            switch (vXmlEncoding)
            {

                case EnumXMLEncoding.UTF_7:
                    return "utf-7";

                case EnumXMLEncoding.UTF_8:
                    return "utf-8";

                case EnumXMLEncoding.UTF_16:
                    return "utf-16";

                case EnumXMLEncoding.UTF_32:
                    return "utf-32";

                default:
                    return "utf-8";

            }

        }

        public static string CheckXMLDati(string xml)
        {
            string sReturn = xml;

            sReturn = sReturn.Replace(@"&lt;", "<");
            sReturn = sReturn.Replace(@"&gt;", ">");

            sReturn = sReturn.Replace(@"<?xml version=""1.0"" encoding=""utf-7""?>", "");
            sReturn = sReturn.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "");
            sReturn = sReturn.Replace(@"<?xml version=""1.0"" encoding=""utf-16""?>", "");
            sReturn = sReturn.Replace(@"<?xml version=""1.0"" encoding=""utf-32""?>", "");

            return sReturn;
        }

        public static XmlElement GetElement(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc.DocumentElement;
        }

        public class Utf8StringWriter : StringWriter
        {
            public Utf8StringWriter(StringBuilder sb)
                : base(sb)
            {
            }
            public override Encoding Encoding { get { return Encoding.UTF8; } }
        }

        public static string XmlSerializeToString(this object objectInstance)
        {
            var serializer = new XmlSerializer(objectInstance.GetType());
            var sb = new StringBuilder();

            using (TextWriter writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, objectInstance);
            }

            return sb.ToString();
        }

        public static string XmlSerializeToStringUtf8(this object objectInstance)
        {
            var serializer = new XmlSerializer(objectInstance.GetType());
            var sb = new StringBuilder();

            using (TextWriter writer = new Utf8StringWriter(sb))
            {
                serializer.Serialize(writer, objectInstance);
            }

            return sb.ToString();

        }
        public static T XmlDeserializeFromString<T>(string objectData)
        {
            return (T)XmlDeserializeFromString(objectData, typeof(T));
        }

        public static object XmlDeserializeFromString(string objectData, Type type)
        {
            var serializer = new XmlSerializer(type);
            object result;

            using (TextReader reader = new StringReader(objectData))
            {
                result = serializer.Deserialize(reader);
            }

            return result;
        }

    }
}
