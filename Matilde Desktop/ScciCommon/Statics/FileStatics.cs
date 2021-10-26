using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using UnicodeSrl.Scci.Enums;

namespace UnicodeSrl.Scci.Statics
{
    public static class FileStatics
    {
        public static byte[] GetBytesFromFile(string filePath)
        {
            try
            {
                return File.ReadAllBytes(filePath);
            }
            catch
            {
                return null;
            }
        }

        public static byte[] GetBytesFromBase64(char[] base64chararray)
        {
            try
            {
                return Convert.FromBase64CharArray(base64chararray, 0, base64chararray.Length);
            }
            catch
            {
                return null;
            }

        }

        public static string GetSCCITempPath()
        {
            return GetSCCITempPath(true);
        }
        public static string GetSCCITempPath(bool create)
        {
            string stmp = System.IO.Path.GetTempPath();
            stmp = System.IO.Path.Combine(stmp, @"SCCI\");

            if (!System.IO.Directory.Exists(stmp) && create) System.IO.Directory.CreateDirectory(stmp);

            return stmp;
        }

        public static string GetPathSalvataggioScheda(string tipo, string entita, string idEntita, string codscheda, int versione, string utente, string idmovscheda, string idschedapadre)
        {

            string spath = string.Empty;
            string sfile = string.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}.xml", tipo, entita, idEntita, codscheda, versione.ToString(), utente.Replace('\\', '-'), idmovscheda, idschedapadre);

            switch ((EnumTipoSalvataggioScheda)Enum.Parse(typeof(EnumTipoSalvataggioScheda), Database.GetConfigTable(EnumConfigTable.TipoSalvataggioScheda)))
            {

                case UnicodeSrl.Scci.Enums.EnumTipoSalvataggioScheda.N:
                    break;

                case UnicodeSrl.Scci.Enums.EnumTipoSalvataggioScheda.L:
                    spath = Path.Combine(Path.GetTempPath(), sfile);
                    break;

                case UnicodeSrl.Scci.Enums.EnumTipoSalvataggioScheda.R:
                    spath = Path.Combine(Database.GetConfigTable(EnumConfigTable.PathSalvataggioScheda), sfile);
                    break;
            }

            return spath;

        }

        public static bool CheckSalvataggioScheda(string spath)
        {

            bool bret = false;

            try
            {
                if (spath != string.Empty && System.IO.File.Exists(spath) == true)
                {
                    bret = true;
                }
            }
            catch (Exception)
            {
                bret = false;
            }

            return bret;

        }

        public static string ReadSalvataggioScheda(string spath, string sxmloriginale)
        {

            string sxml = string.Empty;

            try
            {
                sxml = System.IO.File.ReadAllText(spath);
            }
            catch (Exception)
            {
                sxml = sxmloriginale;
            }

            return sxml;

        }

        public static void WriteSalvataggioScheda(string spath, string sxml)
        {

            try
            {
                if (spath != string.Empty)
                {
                    System.IO.File.WriteAllText(spath, sxml);
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        public static void DeleteSalvataggioScheda(string spath)
        {

            try
            {
                if (spath != string.Empty && System.IO.File.Exists(spath) == true)
                {
                    System.IO.File.Delete(spath);
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

    }
}
