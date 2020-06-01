using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace XSDTools
{
    internal class XsdProcessor
    {
        private readonly string schemaLocationAttribute = "schemaLocation";
        private readonly string includeTag = "xsd:include";
        private readonly string importTag = "xsd:import";

        public IEnumerable<string> ExtractSubfiles(string xsdFile)
        {
            var document = new XmlDocument();
            document.Load(xsdFile);

            var result = new List<string>();
            foreach (var node in GetNodes(document))
            {
                var attribute = node.Attributes[schemaLocationAttribute];
                if (attribute != null)
                {
                    var uri = new Uri(attribute.Value, UriKind.RelativeOrAbsolute);
                    if (uri.IsAbsoluteUri && !uri.IsFile)
                    {
                        result.Add(uri.OriginalString);
                        attribute.Value = Path.GetFileName(uri.OriginalString);
                    }
                }
            }

            if (result.Count != 0)
            {
                document.Save(xsdFile);
            }
            return result;
        }

        private IEnumerable<XmlNode> GetNodes(XmlDocument document)
        {
            var nodes = new List<XmlNode>();
            var imports = document.GetElementsByTagName(importTag);
            var includes = document.GetElementsByTagName(includeTag);
            foreach (XmlNode node in imports)
            {
                nodes.Add(node);
            }
            foreach (XmlNode node in includes)
            {
                nodes.Add(node);
            }
            return nodes;
        }
    }
}