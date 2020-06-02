using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace XSDTools
{
    public class XmlProcessor
    {
        private readonly string schemaLocationAttribute = "schemaLocation";
        private readonly string includeTag = "xsd:include";
        private readonly string importTag = "xsd:import";

        public void ForEachDependency(XmlDocument document, Action<XmlAttribute, Uri> action)
        {
            foreach (var node in GetNodesWithDependencies(document))
            {
                var attribute = node.Attributes[schemaLocationAttribute];
                if (attribute != null)
                {
                    var uri = new Uri(attribute.Value, UriKind.RelativeOrAbsolute);
                    if (uri.IsAbsoluteUri && !uri.IsFile)
                    {
                        action(attribute, uri);
                    }
                }
            }
        }

        public List<string> GetAllDependencies(string filePath)
        {
            var document = new XmlDocument();
            document.Load(filePath);
            return GetAllDependencies(document);
        }

        public List<string> GetAllDependencies(XmlDocument document)
        {
            var result = new List<string>();
            ForEachDependency(document, (_, uri) => result.Add(uri.OriginalString));
            return result;
        }

        public List<ReplacedDependencies> ReplaceExternalDependencies(string filePath)
        {
            var document = new XmlDocument();
            document.Load(filePath);
            var replaced = ReplaceExternalDependencies(document);
            if (replaced.Count > 0)
            {
                document.Save(filePath);
            }
            return replaced;
        }

        public List<ReplacedDependencies> ReplaceExternalDependencies(XmlDocument document)
        {
            var result = new List<ReplacedDependencies>();
            ForEachDependency(document, (attribute, uri) =>
            {
                var fileName = Path.GetFileName(uri.OriginalString);
                attribute.Value = fileName;
                result.Add(new ReplacedDependencies
                {
                    OriginalPath = uri.OriginalString,
                    ReplacedWith = fileName
                });
            });
            return result;
        }

        public List<XmlNode> GetNodesWithDependencies(XmlDocument document)
        {
            var result = new List<XmlNode>();
            result.AddRange(GetNodesByTag(document, importTag));
            result.AddRange(GetNodesByTag(document, includeTag));
            return result;
        }

        public List<XmlNode> GetNodesByTag(XmlDocument document, string tag)
        {
            var result = new List<XmlNode>();
            var nodes = document.GetElementsByTagName(tag);
            foreach (XmlNode node in nodes)
            {
                result.Add(node);
            }
            return result;
        }
    }
}