using System.Collections.Generic;

namespace XSDTools
{
    public class XsdMapData : ValidationData
    {
        public List<XsdElement> XsdElements { get; set; } = new List<XsdElement>();
    }
}