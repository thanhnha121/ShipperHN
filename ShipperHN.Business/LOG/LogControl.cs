using System;
using System.IO;
using System.Web.Configuration;

namespace ShipperHN.Business.LOG
{
    public class LogControl
    {
        public void AddLog(int type, string area, string log)
        {
            string logFilePath = System.AppDomain.CurrentDomain.BaseDirectory + WebConfigurationManager.AppSettings["LogFilePath"];
            if (type == 0)
            {
                string createText = "[HANDLED] [" + GetTime() + "] at [" + area + "] " + log + "\n";
                using (StreamWriter sw = File.AppendText(logFilePath))
                {
                    sw.WriteLine(createText);
                }
            }
            else
            {
                string createText = "[UNHANDLED] [" + GetTime() + "] at [" + area + "] " + log + "\n";
                using (StreamWriter sw = File.AppendText(logFilePath))
                {
                    sw.WriteLine(createText);
                }
            }
        }

        public string GetTime()
        {
            return DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }
    }
}
