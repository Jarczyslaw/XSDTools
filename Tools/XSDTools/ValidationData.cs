using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;

namespace XSDTools
{
    public class ValidationData
    {
        public List<ValidationEventArgs> Data { get; set; } = new List<ValidationEventArgs>();

        public bool HasData => Data.Count > 0;

        public List<ValidationEventArgs> Errors
        {
            get
            {
                if (Data == null)
                {
                    return Data.Where(d => d.Severity == XmlSeverityType.Error)
                        .ToList();
                }
                return null;
            }
        }

        public bool HasErrors => Errors != null;

        public int ErrorsCount => Errors == null ? 0 : Errors.Count;

        public List<ValidationEventArgs> Warnings
        {
            get
            {
                if (Data == null)
                {
                    return Data.Where(d => d.Severity == XmlSeverityType.Warning)
                        .ToList();
                }
                return null;
            }
        }

        public bool HasWarnings => Warnings != null;

        public int WarningsCount => Warnings == null ? 0 : Warnings.Count;
    }
}