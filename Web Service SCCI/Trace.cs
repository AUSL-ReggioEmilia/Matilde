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

        private static string TraceFilePath
        {
            get
            {
                string path;
                path = System.IO.Path.GetDirectoryName(
                   System.Reflection.Assembly.GetExecutingAssembly().Location);

                path += @"\Logs\" + DateTime.Now.ToString("yyyyMMdd") + @".uctrace";

                return path;
            }
        }
        
        public static void Add(string info)
        {
            try
            {
                lock (lockObject)
                {
                    string _trace = @"C:\temp\Trace\Log.txt";
                    StringBuilder sb = new StringBuilder();

                    string outp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.ffff") + " - " + info;

                    
                                        
                                        
                                        
                    if (File.Exists(_trace) == false)
                    {
                        using (StreamWriter sw = File.CreateText(_trace))
                        {
                            string startOutp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.ffff") + " - Trace Created ";
                        }
                    }
                    using (StreamWriter sw = File.AppendText(_trace))
                    {
                        sw.WriteLine(outp);
                    }

                }
            }
            catch
            {
            }
        }

        public static void AddMessage(string info)
        {

#if DEBUG
            try
            {
                lock (lockObject)
                {
                    string _trace = TraceFilePath;
                    StringBuilder sb = new StringBuilder();

                    string outp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.ffff") + " - " + info;

                    if (File.Exists(_trace) == false)
                    {
                        using (StreamWriter sw = File.CreateText(_trace))
                        {
                            string startOutp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.ffff") + " - Trace Created ";
                        }
                    }
                    using (StreamWriter sw = File.AppendText(_trace))
                    {
                        sw.WriteLine(outp);
                    }

                }
            }
            catch
            {
            }
#endif
        }

        public static void DeleteTraceFile()
        {
            try
            {
                File.Delete(TraceFilePath);
            }
            catch 
            {
            }
            
        }

    }
}
