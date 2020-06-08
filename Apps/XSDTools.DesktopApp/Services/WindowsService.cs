using Prism.Ioc;
using XSDTools.DesktopApp.Models;
using XSDTools.DesktopApp.Views;

namespace XSDTools.DesktopApp.Services
{
    public class WindowsService : IWindowsService
    {
        private readonly IContainerExtension containerExtension;

        public WindowsService(IContainerExtension containerExtension)
        {
            this.containerExtension = containerExtension;
        }

        public ModelsData GetModelsData()
        {
            return containerExtension.Resolve<ModelsDataWindow>().ShowAsDialog();
        }
    }
}