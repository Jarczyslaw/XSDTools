using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

namespace XSDTools
{
    public class XsdElement
    {
        public XsdElement(XmlSchemaElement xmlSchemaElement)
        {
            XmlSchemaElement = xmlSchemaElement;
        }

        public XmlSchemaElement XmlSchemaElement { get; }

        public string XsdName => XmlSchemaElement.Name;

        public XmlSchemaDatatype XsdDataType => XmlSchemaElement.ElementSchemaType?.Datatype;

        public List<XmlSchemaAttribute> XsdAttributes
        {
            get
            {
                var attributes = new List<XmlSchemaAttribute>();
                if (XmlSchemaElement.ElementSchemaType is XmlSchemaComplexType xmlComplexType)
                {
                    foreach (DictionaryEntry entry in xmlComplexType.AttributeUses)
                    {
                        if (entry.Value is XmlSchemaAttribute attr)
                        {
                            attributes.Add(attr);
                        }
                    }
                }
                return attributes;
            }
        }

        public XmlNode XsdAnnotation
        {
            get
            {
                XmlNode node = null;
                if (XmlSchemaElement.Annotation?.Items?.Count > 0)
                {
                    var documentation = XmlSchemaElement.Annotation.Items[0] as XmlSchemaDocumentation;
                    if (documentation?.Markup?.Length > 0)
                    {
                        node = documentation.Markup[0];
                    }
                }
                return node;
            }
        }

        public List<XsdElement> Children { get; set; } = new List<XsdElement>();

        public override string ToString()
        {
            var result = XsdName;
            if (XsdDataType != null)
            {
                result += $" ({XsdDataType.TypeCode})";
            }
            if (XsdAnnotation != null)
            {
                result += " - " + XsdAnnotation.Value;
            }
            return result;
        }
    }
}