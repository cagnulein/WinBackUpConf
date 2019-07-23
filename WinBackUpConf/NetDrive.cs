using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.VisualBasic;

namespace WinBackUpConf
{
    class NetDrive
    {
        private String path = "";
        private String letter = "";
        private String username = "";
        private String password = "";

        public String Path
        {
            get { return path; }
            set { path = value; }
        }

        public String Letter
        {
            get { return letter; }
            set { letter = value; }
        }

        public String Username { get { return username; } set { username = value; } }
        public String Password { get { return password; } set { password = value; } }

        public Boolean IsValid()
        {
            return path != "";
        }

        public void Parse(String line)
        {
            if (line.TrimStart().StartsWith("OK") == false) return;

            char[] s = { ' ' };
            String[] cols = line.Split(s, StringSplitOptions.RemoveEmptyEntries);
            if(cols[1].Contains(":"))
            {
                Letter = cols[1];
                Path = cols[2];

                // get user
                if (this.IsValid())
                {
                    string[] ret = runCmd("wmic", "netuse where LocalName=\"" + Letter + "\" get UserName /value");
                    if (ret[0].StartsWith("UserName="))
                    {
                        Username = ret[0].Replace("UserName=", "");
                    }
                }
            }                
        }

        public Boolean Save(XmlDocument xmlDoc, XmlNode rootNode)
        {
            XmlNode userNode = xmlDoc.CreateElement("NetDrive");
            XmlAttribute attribute = xmlDoc.CreateAttribute("path");
            attribute.Value = Path;
            userNode.Attributes.Append(attribute);
            attribute = xmlDoc.CreateAttribute("letter");
            attribute.Value = Letter;
            userNode.Attributes.Append(attribute);
            attribute = xmlDoc.CreateAttribute("username");
            attribute.Value = Username;
            userNode.Attributes.Append(attribute);
            attribute = xmlDoc.CreateAttribute("password");
            if(Username != "" && Password == "")
            {
                string password = Interaction.InputBox("If you want, you can insert the password for user " + Username + ". Otherwise leave it empty.", Username + " password for netdrive " + Letter);
                Password = StringCipher.Encrypt(password, Username);
            }
            attribute.Value = Password;
            userNode.Attributes.Append(attribute);
            rootNode.AppendChild(userNode);

            return true;
        }

        public Boolean Load(XmlNode node)
        {
            Path = node.Attributes.GetNamedItem("path").Value;
            Letter = node.Attributes.GetNamedItem("letter").Value;
            Username = node.Attributes.GetNamedItem("username").Value;
            if(node.Attributes.GetNamedItem("password").Value != "")
                Password = StringCipher.Decrypt(node.Attributes.GetNamedItem("password").Value, this.Username);

            String[] ret;
            if (Username == "")
                ret = runCmd("net", "use "+ letter + " " +  path + " /PERSISTENT:YES");
            else
                ret = runCmd("net", "use " + letter + " " + path + " /user:" + Username + " " + Password + " /PERSISTENT:YES");

            return true;
        }

        public NetDrive()
        {

        }

        public NetDrive(XmlNode node)
        {
            Load(node);
        }

        public NetDrive(String line)
        {
            Parse(line);
        }

        public class ErrorEventArgs : EventArgs
        {
            public String Command { get; set; }
            public String ErrorText { get; set; }
        }

        public event EventHandler<ErrorEventArgs> Error;

        protected virtual void OnError(ErrorEventArgs e)
        {
            EventHandler<ErrorEventArgs> handler = Error;
            if (handler != null)
            {
                handler(this, e);
            }
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

            string[] separator = { "\n", "\r" };
            if(errors == "")
            {
                OutputEventArgs o = new OutputEventArgs();
                o.Text = output.Replace("\r", "").Replace("\n","");
                o.Command = cmd + " " + arg;
                OnOutput(o);

                return output.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            }                
            else
            {
                OutputEventArgs o = new OutputEventArgs();
                o.Text = errors.Replace("\r", "").Replace("\n", ""); ;
                o.Command = cmd + " " + arg;
                OnOutput(o);

                ErrorEventArgs e = new ErrorEventArgs();
                e.ErrorText = errors.Replace("\r", "").Replace("\n", "");
                e.Command = cmd + " " + arg;
                OnError(e);
                return errors.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            }                
        }
    }
}
