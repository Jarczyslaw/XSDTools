using XSDTools.DesktopApp.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;
using JToolbox.Desktop.Dialogs;
using XSDTools.DesktopApp.Services;

namespace XSDTools.DesktopApp
{
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IDialogsService, DialogsService>();
            containerRegistry.Register<IAppSettings, AppSettings>();
        }
    }
}
