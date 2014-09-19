using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace DWAS2.Utilities
{
    enum LogType
    {
        GOOD,
        INFO,
        WARNING,
        ERROR
    }

    class GeneralLogger
    {
        private static GeneralLogger instance = null;
        private string logfile;

        private GeneralLogger(string logfile)
        {
            using (FileStream fs = new FileStream(logfile, FileMode.OpenOrCreate, FileAccess.Write))
            {
                // just test writing
            }
            this.logfile = logfile;
        }

        public static bool Initialize(string logfile)
        {
            if (instance != null) return false;
            else
            {
                try
                {
                    instance = new GeneralLogger(logfile);
                    return true;
                }
                catch(Exception)
                {
                    return false;
                }
            }
        }

        public async static void WriteLog(string content, LogType type)
        {
            if (instance == null)
                throw new InvalidOperationException("logger not initialized.");
            using(StreamWriter sw = new StreamWriter(instance.logfile, append: true))
            {
                await sw.WriteLineAsync(String.Format("{0}[{1}]{2}", DateTime.Now.ToString(), type.ToString(), content));
            }
        }

        public static bool IsInitialized()
        {
            return (instance != null);
        }
    }
}
