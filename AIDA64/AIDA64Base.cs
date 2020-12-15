using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace AIDA64
{
    class AIDA64Item
    { 
        public string id { get; }
        public string label { get; }
        public string value { get; set; }

        public AIDA64Item(string aId, string aLabel)
        {
            id    = aId;
            label = aLabel;
        }
    }

    class AIDA64Base : PropertyBase
    {
        public Dictionary<string, Dictionary<string, AIDA64Item>> MainColles { get; } = 
            new Dictionary<string, Dictionary<string, AIDA64Item>>();

        public string Context
        {
            get { return _Context; }
            set { SetProperty(ref _Context, value); }
        }
        string _Context;

        public void Parser(string aRaw)
        {
            var _str = string.Empty;
            var doc  = new XmlDocument();
            doc.LoadXml("<html>" + aRaw + "</html>");

            var root = doc.FirstChild;

            if (root.HasChildNodes)
            {
                foreach (XmlNode rootNode in root.ChildNodes)
                {
                    var nodeName = rootNode.Name;
                    try
                    {
                        if (!MainColles.ContainsKey(nodeName))
                        {
                            MainColles.Add(nodeName, new Dictionary<string, AIDA64Item>());
                        }

                        var currentNodeColles = MainColles[nodeName];

                        if (rootNode.ChildNodes.Count == 3)
                        {
                            var id    = rootNode.ChildNodes[0].InnerText;
                            var label = rootNode.ChildNodes[1].InnerText;
                            var value = rootNode.ChildNodes[2].InnerText;

                            _str = string.Format("{0}#{1,-25}{2,-25} : {3}\r\n", _str, id, label, value);

                            if (!currentNodeColles.ContainsKey(id))
                            {
                                currentNodeColles.Add(id, new AIDA64Item(id, label));
                            }

                           currentNodeColles[id].value = value;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(string.Format("{0} Parser err\r\n{1}\r\n{}", 
                            nodeName,
                            ex.Message,
                            ex.StackTrace));
                    }
                }
            }

            Context = _str;
        }
    }
}
