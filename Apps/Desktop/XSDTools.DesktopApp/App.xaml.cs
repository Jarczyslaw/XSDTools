using XSDTools.DesktopApp.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;
using JToolbox.Desktop.Dialogs;

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
        }
    }
}
