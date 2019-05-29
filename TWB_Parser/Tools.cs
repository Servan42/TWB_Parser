using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace TWB_Parser
{
    public static class Tools
    {
        /// <summary>
        /// Very simple XML parser to read the config.
        /// </summary>
        /// <param name="filename">The full path of the XML document + its name.</param>
        /// <returns>A list of strings containing every feilds of the XML document.</returns>
        public static List<String> XmlReader(string filename)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(filename);
            XmlElement elem = xdoc.DocumentElement;
            List<String> list = new List<String>();
            foreach (XmlElement e in elem)
            {
                list.Add(e.InnerText.ToString());
            }
            return list;
        }
    }
}
