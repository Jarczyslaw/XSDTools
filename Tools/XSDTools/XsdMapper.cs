using System.Collections.Generic;
using System.Xml.Schema;

namespace XSDTools
{
    public class XsdMapper
    {
        private readonly List<XsdElement> elements = new List<XsdElement>();

        public List<XsdElement> GetXsdElements(XmlSchema schema)
        {
            elements.Clear();
            GetRootXsdElements(schema);
            return elements;
        }

        private void GetRootXsdElements(XmlSchema schema)
        {
            foreach (XmlSchemaObject element in schema.Elements.Values)
            {
                GetXsdElements(element, null);
            }
        }

        private XsdElement AddNewXsdElement(XmlSchemaElement xmlSchemaElement, XsdElement element)
        {
            if (!string.IsNullOrEmpty(xmlSchemaElement.Name))
            {
                var newElement = new XsdElement(xmlSchemaElement);
                if (element == null)
                {
                    elements.Add(newElement);
                }
                else
                {
                    element.Children.Add(newElement);
                }
                return newElement;
            }
            return null;
        }

        private void GetXsdElements(XmlSchemaObject xmlObject, XsdElement parentElement)
        {
            if (xmlObject is XmlSchemaElement xmlElement)
            {
                if (xmlElement.ElementSchemaType is XmlSchemaSimpleType)
                {
                    AddNewXsdElement(xmlElement, parentElement);
                }
                else if (xmlElement.ElementSchemaType is XmlSchemaComplexType xmlComplexType)
                {
                    var element = AddNewXsdElement(xmlElement, parentElement);
                    if (element != null && xmlComplexType.ContentTypeParticle != null)
                    {
                        GetXsdElements(xmlComplexType.ContentTypeParticle, element);
                    }
                }
            }
            else if (xmlObject is XmlSchemaGroupBase xmlGroup)
            {
                foreach (XmlSchemaObject item in xmlGroup.Items)
                {
                    GetXsdElements(item, parentElement);
                }
            }
        }
    }
}