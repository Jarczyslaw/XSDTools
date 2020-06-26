using System.Collections.Generic;

namespace XSDTools
{
    public class XsdMapData : ValidationData
    {
        public List<XsdElement> XsdElements { get; set; } = new List<XsdElement>();

        public int XsdElementsCount => GetXsdElementsCount(XsdElements);

        public int XsdRootElementsCount => XsdElements.Count;

        public List<XsdElement> GetXsdElementsByName(string name)
        {
            var result = new List<XsdElement>();
            GetXsdElementsByName(XsdElements, name, result);
            return result;
        }

        private void GetXsdElementsByName(List<XsdElement> elements, string name, List<XsdElement> foundElements)
        {
            foreach (var element in elements)
            {
                if (element.XsdName == name)
                {
                    foundElements.Add(element);
                }
                GetXsdElementsByName(element.Children, name, foundElements);
            }
        }

        private int GetXsdElementsCount(List<XsdElement> elements)
        {
            var count = elements.Count;
            foreach (var element in elements)
            {
                count += GetXsdElementsCount(element.Children);
            }
            return count;
        }
    }
}