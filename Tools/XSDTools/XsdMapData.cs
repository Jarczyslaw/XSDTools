using System.Collections.Generic;

namespace XSDTools
{
    public class XsdMapData : ValidationData
    {
        public List<XsdElement> XsdElements { get; set; } = new List<XsdElement>();

        public int XsdElementsCount => GetXsdElementsCount(XsdElements);

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