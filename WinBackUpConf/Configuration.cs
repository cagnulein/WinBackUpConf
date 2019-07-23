using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        public Configuration(String filename)
        {
            Load(filename);
        }

        public Boolean Load(String filename)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filename);
            XmlNode node = xmlDoc.SelectSingleNode("NetDrives");
            foreach (XmlNode item in node.ChildNodes)
            {
                NetDrive n = new NetDrive();
                n.Error += Load_Error;
                n.Output += Load_Output;
                n.Load(item);
                if (n.IsValid())
                    NetDrives.Add(n);
            }
            return true;
        }

        private void Load_Output(object sender, NetDrive.OutputEventArgs e)
        {
            OutputEventArgs o = new OutputEventArgs();
            o.Command = e.Command;
            o.Text = e.Text;
            OnOutput(o);
        }

        private void Load_Error(object sender, NetDrive.ErrorEventArgs e)
        {
            MessageBox.Show("Error loading NetDrive\n" + e.ErrorText);
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

        public class OutputEventArgs : EventArgs
        {
            public String Command { get; set; }
            public String Text { get; set; }
        }

        public event EventHandler<OutputEventArgs> Output;

        protected virtual void OnOutput(OutputEventArgs e)
        {
            EventHandler<OutputEventArgs> handler = Output;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
