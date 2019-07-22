using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WinBackUpConf
{
    class Configuration
    {
        List<NetDrive> NetDrives = new List<NetDrive>();

        public Configuration()
        {
            String[] netdrives = runCmd("net", "use");
            foreach(String drive in netdrives)
            {
                NetDrive d = new NetDrive(drive);
                if (d.IsValid())
                    NetDrives.Add(d);
            }
        }

        public Boolean Save(String filename)
        {
            XmlDocument xmlDoc = new XmlDocument();

            XmlNode rootNode = xmlDoc.CreateElement("NetDrives");
            xmlDoc.AppendChild(rootNode);
            foreach (NetDrive d in NetDrives)
                d.Save(xmlDoc, rootNode);
            xmlDoc.Save(filename);

            return true;
        }

        public String[] runCmd(String cmd, String arg)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = cmd;
            startInfo.Arguments = arg;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string errors = process.StandardError.ReadToEnd();

            char[] separator = { '\n' };
            return output.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
