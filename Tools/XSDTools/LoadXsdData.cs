using System.Xml.Schema;

namespace XSDTools
{
    public partial class XsdProcessor
    {
        public class LoadXsdData : ValidationData
        {
            public XmlSchema Schema { get; set; }
            public XmlSchemaSet SchemaSet { get; set; }
        }
    }
}