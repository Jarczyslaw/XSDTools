using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;

namespace XSDTools
{
    public partial class XsdProcessor
    {
        public class LoadXsdData : ValidationData
        {
            public List<XmlSchema> Schemas { get; set; } = new List<XmlSchema>();
            public XmlSchemaSet SchemaSet { get; set; }

            public XmlSchema Schema => Schemas.FirstOrDefault();
        }
    }
}