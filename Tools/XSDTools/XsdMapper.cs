using System.Collections.Generic;
using System.Xml.Schema;

namespace XSDTools
{
    public class XsdMapper
    {
        private readonly List<XsdElement> elements = new List<XsdElement>();

        public List<XsdElement> GetXsdElements(XmlSchema schema)
        {
            return StartTraversing(schema, true);
        }

        public List<XsdElement> GetRootXsdElements(XmlSchema schema)
        {
            return StartTraversing(schema, false);
        }

        private List<XsdElement> StartTraversing(XmlSchema schema, bool recursive)
        {
            elements.Clear();
            TraverseXsdElements(schema, recursive);
            return elements;
        }

        private void TraverseXsdElements(XmlSchema schema, bool recursive)
        {
            foreach (XmlSchemaObject element in schema.Elements.Values)
            {
                TraverseXsdElements(element, null, recursive);
            }
        }

        private void TraverseXsdElements(XmlSchemaObject xmlObject, XsdElement parentElement, bool recursive)
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
                    if (element != null && xmlComplexType.ContentTypeParticle != null && recursive)
                    {
                        TraverseXsdElements(xmlComplexType.ContentTypeParticle, element, recursive);
                    }
                }
            }
            else if (xmlObject is XmlSchemaGroupBase xmlGroup && recursive)
            {
                foreach (XmlSchemaObject item in xmlGroup.Items)
                {
                    TraverseXsdElements(item, parentElement, recursive);
                }
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
    }
}