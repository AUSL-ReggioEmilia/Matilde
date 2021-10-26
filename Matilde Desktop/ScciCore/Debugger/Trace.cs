using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace UnicodeSrl.ScciCore.Debugger
{
    public static class Trace
    {
        private static object lockObject = new object();


        public static void AddMessage(string info)
        {
            string _trace = TraceFilePath;
            StringBuilder sb = new StringBuilder();

            using (StreamWriter sw = File.AppendText(_trace))
            {
                sw.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.ffff")} - {info}");
            }
        }

        private static string TraceFilePath
        {
            get
            {
                string path;
                path = System.IO.Path.GetDirectoryName(
                   System.Reflection.Assembly.GetExecutingAssembly().Location);

                path += @"\Logs\";

                if (Directory.Exists(path) == false)
                    Directory.CreateDirectory(path);

                path += DateTime.Now.ToString("yyyyMMdd") + @".uctrace";

                return path;
            }
        }


    }
}
