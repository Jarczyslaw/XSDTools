using JToolbox.Desktop.Core.Services;
using JToolbox.Desktop.Dialogs;
using Prism.Ioc;
using System.Windows;
using XSDTools.DesktopApp.Services;
using XSDTools.DesktopApp.Views;

namespace XSDTools.DesktopApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
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
            containerRegistry.Register<IWindowsService, WindowsService>();
            containerRegistry.Register<ISystemService, SystemService>();
        }
    }
}