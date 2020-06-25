using Prism.Ioc;
using System.Collections.Generic;
using System.Windows;
using XSDTools.DesktopApp.Models;
using XSDTools.DesktopApp.ViewModels;
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

        public XsdElement ShowXsdElements(List<XsdElement> xsdElements, bool selectionEnabled)
        {
            var dataContext = containerExtension.Resolve<XsdTreeViewModel>();
            dataContext.Setup(xsdElements, selectionEnabled);
            var window = new XsdTreeWindow(dataContext)
            {
                Owner = Application.Current.MainWindow
            };
            window.ShowDialog();
            return dataContext.SelectedXsdElement;
        }
    }
}