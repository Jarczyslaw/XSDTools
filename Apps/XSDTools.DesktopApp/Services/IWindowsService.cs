using System.Collections.Generic;
using XSDTools.DesktopApp.Models;

namespace XSDTools.DesktopApp.Services
{
    public interface IWindowsService
    {
        ModelsData GetModelsData();

        XsdElement ShowXsdElements(List<XsdElement> xsdElements, bool selectionEnabled);
    }
}