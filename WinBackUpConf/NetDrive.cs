using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WinBackUpConf
{
    class NetDrive
    {
        private String path = "";
        private String letter = "";

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

        public Boolean IsValid()
        {
            return path != "";
        }

        public void Parse(String line)
        {
            if (line.TrimStart().StartsWith("OK") == false) return;

            char[] s = { ' ' };
            String[] cols = line.Split(s, StringSplitOptions.RemoveEmptyEntries);
            Letter = cols[1];
            Path = cols[2];
        }

        public void Save(XmlDocument xmlDoc, XmlNode rootNode)
        {
            XmlNode userNode = xmlDoc.CreateElement("NetDrive");
            XmlAttribute attribute = xmlDoc.CreateAttribute("path");
            attribute.Value = Path;
            userNode.Attributes.Append(attribute);
            attribute = xmlDoc.CreateAttribute("letter");
            attribute.Value = Letter;
            userNode.Attributes.Append(attribute);
            rootNode.AppendChild(userNode);
        }

        public NetDrive(String line)
        {
            Parse(line);
        }
    }
}
