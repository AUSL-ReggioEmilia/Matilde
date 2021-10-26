using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;
using System.Drawing;
using UnicodeSrl.Evaluator;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace UnicodeSrl.Scci.RTF
{
    public class RtfFiles
    {

        private const string RTF_HEAD_OPEN = @"{\rtf1\ansi\ansicpg1252\deff0\deflang1040";
        private const string RTF_HEAD_CLOSE = @"}";
        private const string RTF_FONTTABLE = @"{\fonttbl{\f0\fnil\fcharset0 [FONT];}}";
        private const string RTF_TAG_FONT = @"[FONT]";
        private const string RTF_TEXT_HEADER = @"\viewkind4\uc1\pard\f0 ";
        private const string RTF_FONT_EMPTY = @"{\fonttbl}";
        private const string RTF_COLOR_EMPTY = @"{\colortbl}";
        private const string RTF_COLOR_ONE = @"\colortbl\red0\green0\blue0;\red{0}\green{1}\blue{2};";
        private const string RTF_COLOR_SINGLE = @"\red{0}\green{1}\blue{2};";

        public const string RTF_SEZIONE = "SEZIONE";
        public const string RTF_GRIGLIA = "GRIGLIA";
        public const string RTF_DIMENSIONE = "DIMENSIONE";
        public const string RTF_BORDI = "BORDI";
        public const string RTF_FONT = "FONT";
        public const string RTF_VISUALIZZASEMPREETICHETTE = "VISUALIZZASEMPREETICHETTE";
        public const string RTF_NASCONDITITOLO = "NASCONDITITOLO";
        public const string RTF_NASCONDISEPARATOREETICHETTE = "NASCONDISEPARATOREETICHETTE";

        public const string RTF_ETICHETTA = "ETICHETTA";
        public const string RTF_VALORE = "VALORE";
        public const string RTF_OPZIONI = "OPZIONI";
        public const string RTF_NASCOSTO = "NASCOSTO";
        public const string RTF_ELENCO = "ELENCO";
        public const string RTF_VISUALIZZASEMPRE = "#[RTF.Etichetta.VisualizzaSempre()]§";
        public const string RTF_RIGHEPRIMA = "RIGHEPRIMA";
        public const string RTF_RIGHEDOPO = "RIGHEDOPO";

        private const int HMM_PER_INCH = 2540;
        private const int MM_ANISOTROPIC = 8;
        private const int MM_HIENGLISH = 5;
        private const int MM_HIMETRIC = 3;
        private const int MM_ISOTROPIC = 7;
        private const int MM_LOENGLISH = 4;
        private const int MM_LOMETRIC = 2;
        private const int MM_TEXT = 1;
        private const int MM_TWIPS = 6;
        private const int TWIPS_PER_INCH = 1440;

        private enum EmfToWmfBitsFlags
        {
            EmfToWmfBitsFlagsDefault = 0x00000000,

            EmfToWmfBitsFlagsEmbedEmf = 0x00000001,

            EmfToWmfBitsFlagsIncludePlaceable = 0x00000002,

            EmfToWmfBitsFlagsNoXORClip = 0x00000004
        }

        [DllImport("gdiplus.dll")]
        private static extern uint GdipEmfToWmfBits(IntPtr _hEmf, uint _bufferSize, byte[] _buffer, int _mappingMode, EmfToWmfBitsFlags _flags);

        public string rtf_empty
        {
            get
            {
                return RTF_HEAD_OPEN + RTF_FONTTABLE + RTF_TEXT_HEADER + RTF_HEAD_CLOSE;
            }
        }

        public string clearRtfText(string rtfText)
        {

            int pos1 = -1;
            int pos2 = -1;

            string res = string.Empty;

            if (rtfText != "")
            {
                pos1 = rtfText.IndexOf('{', 1);
                if (pos1 > 0)
                {
                    res = rtfText.Substring(pos1);

                    pos2 = res.LastIndexOf('}');

                    if (pos2 > 0)
                        res = res.Substring(0, pos2);
                    else
                        res = rtfText;
                }
                else
                    res = rtfText;
            }

            return res;
        }


        public string joinRtf(string sourceRTF, string destRTF)
        {
            return joinRtf(sourceRTF, destRTF, true);
        }

        public string joinRtf(string sourceRTF, string destRTF, bool checkEmptyColor)
        {
            string resRTF = "";

            string cleanUpSource = clearRtfText(sourceRTF);
            string cleanUpDest = clearRtfText(destRTF);

            string fontTable = RTF_FONT_EMPTY;
            string colorTable = RTF_COLOR_EMPTY;

            if (cleanUpDest != null && cleanUpDest != "")
            {
                cleanUpDest = getFonts(cleanUpDest, ref fontTable);
                if (cleanUpDest.Contains(@"{\colortbl"))
                    cleanUpDest = getColors(cleanUpDest, ref colorTable, checkEmptyColor);
            }

            if (cleanUpSource != null && cleanUpSource != "")
            {
                cleanUpSource = getFonts(cleanUpSource, ref fontTable);
                if (cleanUpSource.Contains(@"{\colortbl"))
                    cleanUpSource = getColors(cleanUpSource, ref colorTable, checkEmptyColor);

                while (cleanUpSource.Contains(@"{\*\shppict{\pict\"))
                {
                    int posI = cleanUpSource.IndexOf(@"{\*\shppict{\pict\");
                    int posF = cleanUpSource.IndexOf(@"}}", posI);
                    if (posI != -1 && posF != -1)
                    {
                        posF += 1;

                        string tagpic = cleanUpSource.Substring(posI, posF - posI + 1);

                        int posIB = tagpic.IndexOf(@"blip");
                        int posFB = tagpic.IndexOf(@"}}", posIB);

                        if (posIB != -1 && posFB != -1)
                        {
                            string bytepic = tagpic.Substring(posIB + 5, posFB - (posIB + 5));

                            int arrayLength = bytepic.Length / 2;
                            byte[] byteArray = new byte[arrayLength];
                            for (int i = 0; i < arrayLength; i++)
                            {
                                byteArray[i] = byte.Parse(bytepic.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
                            }

                            string tagpicnew = InsertImage(byteArray);

                            cleanUpSource = cleanUpSource.Replace(tagpic, tagpicnew);
                        }

                    }

                }

            }

            cleanUpDest = cleanUpDest.Replace(RTF_FONT_EMPTY, "");
            cleanUpDest = cleanUpDest.Replace(RTF_COLOR_EMPTY, "");
            cleanUpSource = cleanUpSource.Replace(RTF_FONT_EMPTY, "");
            cleanUpSource = cleanUpSource.Replace(RTF_COLOR_EMPTY, "");

            if (cleanUpSource != string.Empty && (cleanUpSource.IndexOf("generator Riched20") > 0 || cleanUpSource.IndexOf("generator Msftedit") > 0))
            {
                cleanUpSource = @"{" + cleanUpSource + @"}";
            }

            if (cleanUpDest != string.Empty && (cleanUpDest.IndexOf("generator Riched20") > 0 || cleanUpDest.IndexOf("generator Msftedit") > 0))
            {
                cleanUpDest = @"{" + cleanUpDest + @"}";
            }

            if (fontTable == RTF_FONT_EMPTY)
            {
                if (colorTable == RTF_COLOR_EMPTY)
                    resRTF = RTF_HEAD_OPEN + RTF_FONTTABLE.Replace(RTF_TAG_FONT, "Calibri") + RTF_TEXT_HEADER + RTF_HEAD_CLOSE;
                else
                    resRTF = RTF_HEAD_OPEN + RTF_FONTTABLE.Replace(RTF_TAG_FONT, "Calibri") + colorTable + RTF_TEXT_HEADER + RTF_HEAD_CLOSE;

            }
            else
            {
                if (colorTable == RTF_COLOR_EMPTY)
                    resRTF = RTF_HEAD_OPEN + fontTable + cleanUpDest + cleanUpSource + RTF_HEAD_CLOSE;
                else
                    resRTF = RTF_HEAD_OPEN + fontTable + colorTable + cleanUpDest + cleanUpSource + RTF_HEAD_CLOSE;

            }
            return resRTF;

        }

        public string initRtf(System.Drawing.Font font)
        {
            string rtfData = "";
            string rtfFont = RTF_FONTTABLE.Replace(RTF_TAG_FONT, font.FontFamily.Name);

            rtfData = RTF_HEAD_OPEN + rtfFont + RTF_TEXT_HEADER + RTF_HEAD_CLOSE;

            return rtfData;

        }
        public string initRtf(System.Drawing.Font font, System.Drawing.Color color)
        {
            string rtfData = "";
            string rtfFont = RTF_FONTTABLE.Replace(RTF_TAG_FONT, font.FontFamily.Name);

            rtfData = RTF_HEAD_OPEN +
                        rtfFont +
                        "{" + string.Format(RTF_COLOR_ONE, color.R.ToString(), color.G.ToString(), color.B.ToString()) + "}" +
                        RTF_TEXT_HEADER.Trim() + RTF_HEAD_CLOSE;

            return rtfData;

        }
        public string initRtf(System.Drawing.Font font, System.Drawing.Color[] color)
        {
            string rtfData = "";
            string rtfFont = RTF_FONTTABLE.Replace(RTF_TAG_FONT, font.FontFamily.Name);

            rtfData = RTF_HEAD_OPEN + rtfFont + "{" + string.Format(RTF_COLOR_ONE, color[0].R.ToString(), color[0].G.ToString(), color[0].B.ToString());
            for (int i = 1; i < color.Length; i++)
            {
                rtfData += string.Format(RTF_COLOR_SINGLE, color[i].R.ToString(), color[i].G.ToString(), color[i].B.ToString());
            }
            rtfData += "}" + RTF_TEXT_HEADER.Trim() + RTF_HEAD_CLOSE;

            return rtfData;

        }

        public string BarraTestoRTF(string rtfbox)
        {
            rtfbox = rtfbox.Insert(rtfbox.LastIndexOf(RTF_TEXT_HEADER) + RTF_TEXT_HEADER.Length, @"\strike ");
            if (rtfbox.LastIndexOf("\\tab}") > 0)
            {
                rtfbox = rtfbox.Insert(rtfbox.LastIndexOf("\\tab}", rtfbox.Length), @"\strike0 ");
            }
            else if (rtfbox.LastIndexOf("\\tab }") > 0)
            {
                rtfbox = rtfbox.Insert(rtfbox.LastIndexOf("\\tab }", rtfbox.Length), @"\strike0 ");
            }
            else
            { rtfbox = rtfbox.Insert(rtfbox.LastIndexOf("}", rtfbox.Length), @"\strike0 "); }
            rtfbox = rtfbox.Replace("\\plain", "");

            return rtfbox;

        }

        public string appendRtfText(string rftText, string textToAppend, System.Drawing.Font f)
        {
            string resRTF = "";

            string cleanUpSource = clearRtfText(rftText);
            string formattedText = "";

            if (f != null)
            {
                if (f.Italic)
                    formattedText += @"\i";

                if (f.Bold)
                    formattedText += @"\b";

                if (f.Strikeout)
                    formattedText += @"\strike";

                if (f.Underline)
                    formattedText += @"\ul";

                int szDblPts = Convert.ToInt32(f.Size * 2);
                formattedText += @"\fs" + szDblPts.ToString();
            }

            textToAppend = textToAppend.Replace(@"à".ToString(), @"\'e0");
            textToAppend = textToAppend.Replace(@"è".ToString(), @"\'e8");
            textToAppend = textToAppend.Replace(@"ì".ToString(), @"\'ec");
            textToAppend = textToAppend.Replace(@"ò".ToString(), @"\'f2");
            textToAppend = textToAppend.Replace(@"ù".ToString(), @"\'f9");
            textToAppend = textToAppend.Replace(@"£".ToString(), @"\'a3");
            textToAppend = textToAppend.Replace(@"é".ToString(), @"\'e9");
            textToAppend = textToAppend.Replace(@"{".ToString(), @"\{");
            textToAppend = textToAppend.Replace(@"}".ToString(), @"\}");
            textToAppend = textToAppend.Replace(@"ç".ToString(), @"\'e7");
            textToAppend = textToAppend.Replace(@"§".ToString(), @"\'a7");
            textToAppend = textToAppend.Replace(@"°".ToString(), @"\'b0");

            formattedText += " " + textToAppend.Replace(((char)10).ToString(), @"\par ");
            if (f != null)
            {
                if (f.Italic)
                    formattedText += @"\i0";

                if (f.Bold)
                    formattedText += @"\b0";

                if (f.Strikeout)
                    formattedText += @"\strike0";

                if (f.Underline)
                    formattedText += @"\ul0";

            }

            resRTF = RTF_HEAD_OPEN + cleanUpSource + formattedText + RTF_HEAD_CLOSE;

            return resRTF;

        }
        public string appendRtfText(string rftText, string textToAppend, System.Drawing.Font f, System.Drawing.Color c)
        {

            if (c == Color.Empty)
            {
                return appendRtfText(rftText, textToAppend, f);
            }
            else
            {

                string rtfNew = initRtf(f, c);
                rtfNew = appendRtfText(rtfNew, @"\cf1 " + textToAppend + @"\cf0", f);
                return joinRtf(rtfNew, rftText);

            }

        }

        public string appendRtfImage(string rftText, string textToAppend)
        {
            return appendRtfImage(rftText, textToAppend, System.Drawing.Size.Empty);
        }
        public string appendRtfImage(string rftText, string textToAppend, System.Drawing.Size s)
        {

            string resRTF = "";

            string cleanUpSource = clearRtfText(rftText);
            string formattedText = "";

            formattedText = InsertImage(textToAppend, s);

            resRTF = RTF_HEAD_OPEN + cleanUpSource + formattedText + RTF_HEAD_CLOSE;

            return resRTF;

        }

        internal string InsertImage(byte[] byteArray)
        {
            return InsertImage(byteArray, System.Drawing.Size.Empty);
        }
        internal string InsertImage(byte[] byteArray, System.Drawing.Size s)
        {

            StringBuilder sb = new StringBuilder();

            MemoryStream ms = new MemoryStream(byteArray);
            Image _image = Image.FromStream(ms);
            ms.Close();
            ms.Dispose();

            if (s.IsEmpty == false)
            {

                int w = _image.Width;
                int h = _image.Height;

                if (_image.Width > s.Width)
                {
                    w = s.Width;
                    h = (int)(s.Width * _image.Height / _image.Width);
                    if (h > s.Height)
                    {
                        w = (int)(_image.Width * s.Height / _image.Height);
                        h = s.Height;
                    }
                }
                else if (_image.Height > s.Height)
                {
                    w = (int)(_image.Width * s.Height / _image.Height);
                    h = s.Height;
                    if (w > s.Width)
                    {
                        w = s.Width;
                        h = (int)(s.Width * _image.Height / _image.Width);
                    }
                }

                _image = (Image)(new Bitmap(_image, w, h));

            }

            float xDpi;

            float yDpi;

            using (Graphics graphics = Graphics.FromImage(_image))
            {
                xDpi = graphics.DpiX;
                yDpi = graphics.DpiY;
            }

            sb.Append(WriteImagePrefix(_image, xDpi, yDpi));

            sb.Append(WriteRtfImage(_image));

            sb.Append("}");

            _image.Dispose();
            _image = null;

            return sb.ToString();

        }
        private string InsertImage(string textToAppend)
        {
            byte[] bytes = Convert.FromBase64String(textToAppend);
            return InsertImage(bytes, System.Drawing.Size.Empty);
        }
        private string InsertImage(string textToAppend, System.Drawing.Size s)
        {
            byte[] bytes = Convert.FromBase64String(textToAppend);
            return InsertImage(bytes, s);
        }

        private string WriteImagePrefix(Image _image, float xDpi, float yDpi)
        {

            StringBuilder sb = new StringBuilder();

            int picw = (int)Math.Round((_image.Width / xDpi) * HMM_PER_INCH);

            int pich = (int)Math.Round((_image.Height / yDpi) * HMM_PER_INCH);

            int picwgoal = (int)Math.Round((_image.Width / xDpi) * TWIPS_PER_INCH);

            int pichgoal = (int)Math.Round((_image.Height / yDpi) * TWIPS_PER_INCH);

            sb.Append(@"{\pict\wmetafile8");
            sb.Append(@"\picw");
            sb.Append(picw);
            sb.Append(@"\pich");
            sb.Append(pich);
            sb.Append(@"\picwgoal");
            sb.Append(picwgoal);
            sb.Append(@"\pichgoal");
            sb.Append(pichgoal);
            sb.Append(" ");

            return sb.ToString();

        }
        private string WriteRtfImage(Image _image)
        {

            StringBuilder sb = new StringBuilder();

            MemoryStream _stream = null;

            Graphics _graphics = null;

            Metafile _metaFile = null;

            IntPtr _hdc;

            try
            {
                using (_stream = new MemoryStream())
                {
                    using (_graphics = Graphics.FromImage(_image))
                    {
                        _hdc = _graphics.GetHdc();

                        _metaFile = new Metafile(_stream, _hdc);

                        _graphics.ReleaseHdc(_hdc);
                    }

                    using (_graphics = Graphics.FromImage(_metaFile))
                    {
                        _graphics.DrawImage(_image, new Rectangle(0, 0, _image.Width, _image.Height));
                    }
                    byte[] _buffer = null;
                    using (_metaFile)
                    {
                        IntPtr _hEmf = _metaFile.GetHenhmetafile();

                        uint _bufferSize = GdipEmfToWmfBits(_hEmf, 0, null, MM_ANISOTROPIC, EmfToWmfBitsFlags.EmfToWmfBitsFlagsDefault);

                        _buffer = new byte[_bufferSize];

                        uint _convertedSize = GdipEmfToWmfBits(_hEmf, _bufferSize, _buffer, MM_ANISOTROPIC, EmfToWmfBitsFlags.EmfToWmfBitsFlagsDefault);
                    }
                    for (int i = 0; i < _buffer.Length; ++i)
                    {
                        sb.Append(String.Format("{0:X2}", _buffer[i]));
                    }
                    if (_stream != null)
                    {
                        _stream.Flush();
                        _stream.Close();
                    }
                }
            }
            finally
            {
                if (_graphics != null)
                {
                    _graphics.Dispose();
                }
                if (_metaFile != null)
                {
                    _metaFile.Dispose();
                }
                if (_stream != null)
                {
                    _stream.Flush();
                    _stream.Close();
                }
            }

            return sb.ToString();

        }

        public System.Drawing.Font getFontFromString(string fontString, bool bold, bool italic, bool strike = false)
        {

            string[] fontDivided = fontString.Split(';');

            fontDivided[1] = fontDivided[1].Remove(fontDivided[1].Length - 2).Replace(',', '.');

            TypeConverter converter = TypeDescriptor.GetConverter(typeof(System.Drawing.Font));

            System.Globalization.NumberFormatInfo provider = new System.Globalization.NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";

            if (italic && bold)
                return new System.Drawing.Font(new System.Drawing.FontFamily(fontDivided[0]), float.Parse(fontDivided[1], provider), FontStyle.Italic | FontStyle.Bold);
            if (bold)
                return new System.Drawing.Font(new System.Drawing.FontFamily(fontDivided[0]), float.Parse(fontDivided[1], provider), FontStyle.Bold);

            if (italic)
                return new System.Drawing.Font(new System.Drawing.FontFamily(fontDivided[0]), float.Parse(fontDivided[1], provider), FontStyle.Italic);

            if (strike)
                return new System.Drawing.Font(new System.Drawing.FontFamily(fontDivided[0]), float.Parse(fontDivided[1], provider), FontStyle.Strikeout);

            return new System.Drawing.Font(new System.Drawing.FontFamily(fontDivided[0]), float.Parse(fontDivided[1], provider));

        }

        public Dictionary<string, dynamic> getSezioneResult(Dictionary<string, ParseResult> o_parseresult)
        {

            Dictionary<string, dynamic> dict_Result = new Dictionary<string, dynamic>();

            dict_Result.Add(RTF_NASCONDITITOLO, new Dictionary<string, bool>());
            dict_Result.Add(RTF_DIMENSIONE, new Dictionary<string, int>());
            dict_Result.Add(RTF_BORDI, new Dictionary<string, Rectangle>());
            dict_Result.Add(RTF_VISUALIZZASEMPREETICHETTE, new Dictionary<string, bool>());
            dict_Result.Add(RTF_FONT, new Dictionary<string, int>());
            dict_Result.Add(RTF_NASCONDISEPARATOREETICHETTE, new Dictionary<string, bool>());

            dict_Result[RTF_NASCONDITITOLO].Add(RTF_NASCONDITITOLO, false);
            dict_Result[RTF_DIMENSIONE].Add(RTF_DIMENSIONE, 0);
            dict_Result[RTF_BORDI].Add(RTF_BORDI, new Rectangle(0, 0, 0, 0));
            dict_Result[RTF_VISUALIZZASEMPREETICHETTE].Add(RTF_VISUALIZZASEMPREETICHETTE, false);
            dict_Result[RTF_FONT].Add(RTF_DIMENSIONE, 0);
            dict_Result[RTF_NASCONDISEPARATOREETICHETTE].Add(RTF_NASCONDISEPARATOREETICHETTE, false);

            foreach (ParseResult value in o_parseresult.Values)
            {

                if (value.Ids[0].ToUpper() == "RTF".ToUpper())
                {

                    if (value.Ids[1].ToUpper() == RTF_SEZIONE.ToUpper())
                    {

                        if (value.Ids[2].ToUpper() == RTF_NASCONDITITOLO.ToUpper())
                        {
                            dict_Result[value.Ids[2].ToUpper()][value.Ids[2].ToUpper()] = true;
                        }

                    }
                    else if (value.Ids[1].ToUpper() == RTF_GRIGLIA.ToUpper())
                    {

                        if (value.Ids[2].ToUpper() == RTF_DIMENSIONE.ToUpper())
                        {
                            if (value.Result[value.Ids[2]].Count == 1)
                            {
                                dict_Result[value.Ids[2].ToUpper()][value.Ids[2].ToUpper()] = int.Parse((value.Result[value.Ids[2]][0]));
                            }
                        }
                        else if (value.Ids[2].ToUpper() == RTF_BORDI.ToUpper())
                        {
                            dict_Result[value.Ids[2].ToUpper()][value.Ids[2].ToUpper()] = new Rectangle(int.Parse(value.Result[value.Ids[2]][0]),
                                                                                                        int.Parse(value.Result[value.Ids[2]][1]),
                                                                                                        int.Parse(value.Result[value.Ids[2]][2]),
                                                                                                       int.Parse(value.Result[value.Ids[2]][3]));
                        }
                        else if (value.Ids[2].ToUpper() == RTF_VISUALIZZASEMPREETICHETTE.ToUpper())
                        {
                            dict_Result[value.Ids[2].ToUpper()][value.Ids[2].ToUpper()] = true;
                        }
                        else if (value.Ids[2].ToUpper() == RTF_FONT.ToUpper())
                        {
                            if (value.Ids[3].ToUpper() == RTF_DIMENSIONE.ToUpper())
                            {
                                dict_Result[value.Ids[2].ToUpper()][value.Ids[3].ToUpper()] = int.Parse((value.Result[value.Ids[3]][0]));
                            }
                        }
                        else if (value.Ids[2].ToUpper() == RTF_NASCONDISEPARATOREETICHETTE.ToUpper())
                        {
                            dict_Result[value.Ids[2].ToUpper()][value.Ids[2].ToUpper()] = true;
                        }
                    }
                }

            }

            return dict_Result;

        }

        public Dictionary<string, Font> getFontFromStile(string fontString, Dictionary<string, ParseResult> o_parseresult)
        {

            Dictionary<string, Font> FontResult = new Dictionary<string, Font>();

            string[] fontDivided = fontString.Split(';');
            fontDivided[1] = fontDivided[1].Remove(fontDivided[1].Length - 2).Replace(',', '.');
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(System.Drawing.Font));
            System.Globalization.NumberFormatInfo provider = new System.Globalization.NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";

            FontResult.Add(RTF_ETICHETTA, new System.Drawing.Font(new System.Drawing.FontFamily(fontDivided[0]), float.Parse(fontDivided[1], provider)));
            FontResult.Add(RTF_VALORE, new System.Drawing.Font(new System.Drawing.FontFamily(fontDivided[0]), float.Parse(fontDivided[1], provider)));

            foreach (ParseResult value in o_parseresult.Values)
            {

                if (value.Ids[0].ToUpper() == "RTF".ToUpper())
                {

                    if (value.Ids[1].ToUpper() == RTF_ETICHETTA.ToUpper() || value.Ids[1].ToUpper() == RTF_VALORE.ToUpper())
                    {

                        if (value.Ids[2].ToUpper() == "Font".ToUpper())
                        {

                            if (value.Ids[3].ToUpper() == "Stile".ToUpper())
                            {

                                if (value.Result[value.Ids[3]].Count > 0)
                                {

                                    for (int i = 0; i < value.Result[value.Ids[3]].Count; i++)
                                    {

                                        switch (value.Result[value.Ids[3]][i].ToUpper())
                                        {

                                            case "G":
                                                FontResult[value.Ids[1].ToUpper()] = new System.Drawing.Font(FontResult[value.Ids[1].ToUpper()], FontResult[value.Ids[1].ToUpper()].Style | FontStyle.Bold);
                                                break;

                                            case "S":
                                                FontResult[value.Ids[1].ToUpper()] = new System.Drawing.Font(FontResult[value.Ids[1].ToUpper()], FontResult[value.Ids[1].ToUpper()].Style | FontStyle.Underline);
                                                break;

                                            case "C":
                                                FontResult[value.Ids[1].ToUpper()] = new System.Drawing.Font(FontResult[value.Ids[1].ToUpper()], FontResult[value.Ids[1].ToUpper()].Style | FontStyle.Italic);
                                                break;

                                            case "B":
                                                FontResult[value.Ids[1].ToUpper()] = new System.Drawing.Font(FontResult[value.Ids[1].ToUpper()], FontResult[value.Ids[1].ToUpper()].Style | FontStyle.Strikeout);
                                                break;

                                        }

                                    }

                                }

                            }
                            else if (value.Ids[3].ToUpper() == "Dimensione".ToUpper())
                            {

                                if (value.Result[value.Ids[3]].Count == 1)
                                {
                                    float output;
                                    if (float.TryParse(value.Result[value.Ids[3]][0].ToUpper(), out output))
                                    {
                                        FontResult[value.Ids[1].ToUpper()] = new System.Drawing.Font(FontResult[value.Ids[1].ToUpper()].FontFamily, output, FontResult[value.Ids[1].ToUpper()].Style);
                                    }

                                }

                            }

                        }

                    }

                }

            }

            return FontResult;

        }
        public Dictionary<string, Color> getColorFromStile(Dictionary<string, ParseResult> o_parseresult)
        {

            Dictionary<string, Color> ColorResult = new Dictionary<string, Color>();

            const char HexSeparator = '#';

            ColorResult.Add(RTF_ETICHETTA, Color.Empty);
            ColorResult.Add(RTF_VALORE, Color.Empty);

            foreach (ParseResult value in o_parseresult.Values)
            {

                if (value.Ids[0].ToUpper() == "RTF".ToUpper())
                {

                    if (value.Ids[1].ToUpper() == RTF_ETICHETTA.ToUpper() || value.Ids[1].ToUpper() == RTF_VALORE.ToUpper())
                    {

                        if (value.Ids[2].ToUpper() == "Font".ToUpper())
                        {

                            if (value.Ids[3].ToUpper() == "Colore".ToUpper())
                            {

                                if (value.Result[value.Ids[3]].Count == 1)
                                {

                                    if (value.Result[value.Ids[3]][0].Length > 1 && value.Result[value.Ids[3]][0].StartsWith(@"FF"))
                                    {
                                        ColorResult[value.Ids[1].ToUpper()] = System.Drawing.ColorTranslator.FromHtml(HexSeparator + value.Result[value.Ids[3]][0]);
                                    }
                                    else
                                    {
                                        ColorResult[value.Ids[1].ToUpper()] = Color.FromName(value.Result[value.Ids[3]][0]);
                                    }

                                }
                                else if (value.Result[value.Ids[3]].Count == 3)
                                {

                                    ColorResult[value.Ids[1].ToUpper()] = Color.FromArgb(int.Parse(value.Result[value.Ids[3]][0]),
                                                                                            int.Parse(value.Result[value.Ids[3]][1]),
                                                                                            int.Parse(value.Result[value.Ids[3]][2]));

                                }

                            }

                        }

                    }

                }

            }

            return ColorResult;

        }
        public Dictionary<string, Color> getBackColorFromStile(Dictionary<string, ParseResult> o_parseresult)
        {

            Dictionary<string, Color> ColorResult = new Dictionary<string, Color>();

            const char HexSeparator = '#';

            ColorResult.Add(RTF_ETICHETTA, Color.Empty);
            ColorResult.Add(RTF_VALORE, Color.Empty);

            foreach (ParseResult value in o_parseresult.Values)
            {

                if (value.Ids[0].ToUpper() == "RTF".ToUpper())
                {

                    if (value.Ids[1].ToUpper() == RTF_ETICHETTA.ToUpper() || value.Ids[1].ToUpper() == RTF_VALORE.ToUpper())
                    {

                        if (value.Ids[2].ToUpper() == "Cella".ToUpper())
                        {

                            if (value.Ids[3].ToUpper() == "ColoreSfondo".ToUpper())
                            {

                                if (value.Result[value.Ids[3]].Count == 1)
                                {

                                    if (value.Result[value.Ids[3]][0].Length > 1 && value.Result[value.Ids[3]][0].StartsWith(@"FF"))
                                    {
                                        ColorResult[value.Ids[1].ToUpper()] = System.Drawing.ColorTranslator.FromHtml(HexSeparator + value.Result[value.Ids[3]][0]);
                                    }
                                    else
                                    {
                                        ColorResult[value.Ids[1].ToUpper()] = Color.FromName(value.Result[value.Ids[3]][0]);
                                    }

                                }
                                else if (value.Result[value.Ids[3]].Count == 3)
                                {

                                    ColorResult[value.Ids[1].ToUpper()] = Color.FromArgb(int.Parse(value.Result[value.Ids[3]][0]),
                                                                                            int.Parse(value.Result[value.Ids[3]][1]),
                                                                                            int.Parse(value.Result[value.Ids[3]][2]));

                                }

                            }

                        }

                    }

                }

            }

            return ColorResult;

        }
        public Dictionary<string, List<string>> getOpzioniFromStile(Dictionary<string, ParseResult> o_parseresult)
        {

            Dictionary<string, List<string>> OpzioniResult = new Dictionary<string, List<string>>();
            List<string> ListResul = new List<string>();

            OpzioniResult.Add(RTF_OPZIONI, ListResul);

            foreach (ParseResult value in o_parseresult.Values)
            {

                if (value.Ids[0].ToUpper() == "RTF".ToUpper())
                {

                    if (value.Ids[1].ToUpper() == RTF_OPZIONI.ToUpper())
                    {

                        if (value.Result[value.Ids[1]].Count > 0)
                        {

                            for (int i = 0; i < value.Result[value.Ids[1]].Count; i++)
                            {

                                ListResul.Add(value.Result[value.Ids[1]][i].ToUpper());

                            }

                        }

                    }

                }

            }

            return OpzioniResult;

        }
        public Dictionary<string, Size> getDimensioniFromStile(Dictionary<string, ParseResult> o_parseresult)
        {

            Dictionary<string, Size> SizeResult = new Dictionary<string, Size>();

            SizeResult.Add(RTF_ETICHETTA, Size.Empty);
            SizeResult.Add(RTF_VALORE, Size.Empty);

            foreach (ParseResult value in o_parseresult.Values)
            {

                if (value.Ids[0].ToUpper() == "RTF".ToUpper())
                {

                    if (value.Ids[1].ToUpper() == RTF_ETICHETTA.ToUpper() || value.Ids[1].ToUpper() == RTF_VALORE.ToUpper())
                    {

                        if (value.Ids[2].ToUpper() == "Logo".ToUpper() || value.Ids[2].ToUpper() == "Marker".ToUpper())
                        {

                            if (value.Ids[3].ToUpper() == "Dimensione".ToUpper())
                            {

                                if (value.Result[value.Ids[3]].Count == 2)
                                {

                                    SizeResult[value.Ids[1].ToUpper()] = new Size(int.Parse(value.Result[value.Ids[3]][0]), int.Parse(value.Result[value.Ids[3]][1]));

                                }

                            }

                        }

                    }

                }

            }

            return SizeResult;
        }
        public Dictionary<string, int> getRigheFromStile(Dictionary<string, ParseResult> o_parseresult)
        {

            Dictionary<string, int> RigheResult = new Dictionary<string, int>();

            RigheResult.Add(RTF_RIGHEPRIMA, 0);
            RigheResult.Add(RTF_RIGHEDOPO, 0);

            foreach (ParseResult value in o_parseresult.Values)
            {

                if (value.Ids[0].ToUpper() == "RTF".ToUpper())
                {

                    if (value.Ids[1].ToUpper() == RTF_VALORE.ToUpper())
                    {

                        if (value.Ids[2].ToUpper() == "Righe".ToUpper())
                        {

                            int n = int.Parse(value.Result[value.Ids[2]][0]);
                            if (n > 0)
                            {
                                RigheResult[RTF_RIGHEDOPO] = n;
                            }
                            else if (n < 0)
                            {
                                RigheResult[RTF_RIGHEPRIMA] = Math.Abs(n);
                            }

                        }

                    }

                }

            }

            return RigheResult;

        }

        string getFonts(string rtf, ref string fontTable)
        {
            int fontIndex = 0;
            string fontTemp = "";

            if (fontTable != RTF_FONT_EMPTY)
                do
                {
                    fontIndex++;
                    fontTemp += fontTable.Substring(fontTable.IndexOf(@"{\fonttbl") + 9, fontTable.IndexOf(@";}") - 7);
                    fontTable = fontTable.Replace(fontTable.Substring(fontTable.IndexOf(@"{\fonttbl") + 9, fontTable.IndexOf(@";}") - 7), "");

                }
                while (fontTable != RTF_FONT_EMPTY);


            bool bContinua = true;
            int nstart = 0;
            int nlunghezza = 0;
            do
            {
                nstart = rtf.IndexOf(@"{\fonttbl") + 9;
                nlunghezza = rtf.IndexOf(@";}") - 7;

                if (nlunghezza > 0)
                {
                    string temp = rtf.Substring(nstart, nlunghezza);

                    fontIndex++;


                    rtf = rtf.Replace(temp, "");
                    string fontNumber;
                    if (temp[4].ToString() == @"\")
                        fontNumber = temp[3].ToString();
                    else
                        fontNumber = temp[3].ToString() + temp[4].ToString();

                    rtf = rtf.Replace(@"\f" + fontNumber + " ", @"\f" + (fontIndex - 1).ToString() + " ");
                    temp = temp.Replace(@"\f" + fontNumber + " ", @"\f" + (fontIndex - 1).ToString() + " ");

                    rtf = rtf.Replace(@"\f" + fontNumber + @"\", @"\f" + (fontIndex - 1).ToString() + @"\");
                    temp = temp.Replace(@"\f" + fontNumber + @"\", @"\f" + (fontIndex - 1).ToString() + @"\");
                    fontTemp += temp;
                }
                else
                {
                    bContinua = false;
                }
            }
            while ((rtf.Contains(RTF_FONT_EMPTY) == false) && bContinua == true);

            fontTable = fontTable.Insert(fontTable.Length - 1, fontTemp);

            return rtf;

        }

        string getColors(string rtf, ref string colorTable, bool checkEmptyColor)
        {
            int colorIndex = 0;
            string colorTemp = "";
            string tagunivoco = @"99999";

            if (colorTable != RTF_COLOR_EMPTY)
                do
                {
                    colorIndex++;
                    colorTemp += colorTable.Substring(colorTable.IndexOf(@"{\colortbl") + 10, colorTable.IndexOf(@";") - 9);

                    colorTable = ReplaceFirst(colorTable, colorTable.Substring(colorTable.IndexOf(@"{\colortbl") + 10, colorTable.IndexOf(@";") - 9), "");


                }
                while (colorTable != RTF_COLOR_EMPTY);


            int indice = 0;


            string tempRtf = rtf.Substring(rtf.IndexOf(@"{\colortbl"));
            string splitRtf = rtf.Substring(0, rtf.IndexOf(@"{\colortbl"));
            do
            {

                string temp = tempRtf.Substring(tempRtf.IndexOf(@"{\colortbl") + 10, tempRtf.IndexOf(@";") - 9);

                tempRtf = ReplaceFirst(tempRtf, temp, "");

                if (checkEmptyColor && colorTemp.Trim() != "" && temp.Trim() == @";")
                {
                    tempRtf = tempRtf.Replace(@"\cf" + indice + " ", @"\cf" + tagunivoco + 0.ToString() + " ");
                    tempRtf = tempRtf.Replace(@"\cb" + indice + " ", @"\cb" + tagunivoco + 0.ToString() + " ");
                    tempRtf = tempRtf.Replace(@"\highlight" + indice + " ", @"\highlight" + tagunivoco + 0.ToString() + " ");
                    tempRtf = tempRtf.Replace(@"\cf" + indice + @"\", @"\cf" + tagunivoco + 0.ToString() + @"\");
                    tempRtf = tempRtf.Replace(@"\cb" + indice + @"\", @"\cb" + tagunivoco + 0.ToString() + @"\");
                    tempRtf = tempRtf.Replace(@"\highlight" + indice + @"\", @"\highlight" + tagunivoco + 0.ToString() + @"\");
                    tempRtf = tempRtf.Replace(@"\clcbpat" + indice + @"\", @"\clcbpat" + tagunivoco + 0.ToString() + @"\");

                }
                else
                {
                    colorIndex++;
                    tempRtf = tempRtf.Replace(@"\cf" + indice + " ", @"\cf" + tagunivoco + (colorIndex - 1).ToString() + " ");
                    tempRtf = tempRtf.Replace(@"\cb" + indice + " ", @"\cb" + tagunivoco + (colorIndex - 1).ToString() + " ");
                    tempRtf = tempRtf.Replace(@"\highlight" + indice + " ", @"\highlight" + tagunivoco + (colorIndex - 1).ToString() + " ");
                    tempRtf = tempRtf.Replace(@"\cf" + indice + @"\", @"\cf" + tagunivoco + (colorIndex - 1).ToString() + @"\");
                    tempRtf = tempRtf.Replace(@"\cb" + indice + @"\", @"\cb" + tagunivoco + (colorIndex - 1).ToString() + @"\");
                    tempRtf = tempRtf.Replace(@"\highlight" + indice + @"\", @"\highlight" + tagunivoco + (colorIndex - 1).ToString() + @"\");
                    tempRtf = tempRtf.Replace(@"\clcbpat" + indice + @"\", @"\clcbpat" + tagunivoco + (colorIndex - 1).ToString() + @"\");


                    colorTemp += temp;

                }

                indice++;

            }
            while (!tempRtf.Contains(RTF_COLOR_EMPTY));

            colorTable = colorTable.Insert(colorTable.Length - 1, colorTemp);

            tempRtf = tempRtf.Replace(@"\cf" + tagunivoco, @"\cf");
            tempRtf = tempRtf.Replace(@"\cb" + tagunivoco, @"\cb");
            tempRtf = tempRtf.Replace(@"\highlight" + tagunivoco, @"\highlight");
            tempRtf = tempRtf.Replace(@"\clcbpat" + tagunivoco, @"\clcbpat");
            return splitRtf + tempRtf;

        }

        private static string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

    }
}
