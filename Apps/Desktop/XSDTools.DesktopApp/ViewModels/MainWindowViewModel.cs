using JToolbox.Desktop.Dialogs;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.IO;
using XSDTools.DesktopApp.Services;

namespace XSDTools.DesktopApp.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string logs = string.Empty;
        private string xsdExePath = string.Empty;
        private readonly IDialogsService dialogsService;
        private readonly IAppSettings appSettings;

        public MainWindowViewModel(IDialogsService dialogsService, IAppSettings appSettings)
        {
            this.dialogsService = dialogsService;
            this.appSettings = appSettings;

            XsdExePath = appSettings.XsdExePath;
        }

        public DelegateCommand RemoveExternalDependenciesCommand => new DelegateCommand(() =>
        {
        });

        public DelegateCommand GenerateModelsCommand => new DelegateCommand(() =>
        {
            if (!File.Exists(xsdExePath))
            {
                FindXsdExe();
            }

            if (XsdExePathSet)
            {

            }
        });

        public DelegateCommand GenerateSampleXmlCommand => new DelegateCommand(() =>
        {
        });

        public DelegateCommand ValidateXmlCommand => new DelegateCommand(() =>
        {
        });

        public DelegateCommand SetXsdExePathCommand => new DelegateCommand(() => FindXsdExe());

        public string Logs
        {
            get => logs;
            set => SetProperty(ref logs, value);
        }

        public string XsdExePath
        {
            get => xsdExePath;
            set => SetProperty(ref xsdExePath, value);
        }

        public bool XsdExePathSet => File.Exists(XsdExePath);

        private bool FindXsdExe()
        {
            var file = dialogsService.OpenFile("Find xsd.exe file...", null, new List<DialogFilterPair> { new DialogFilterPair("exe") });
            if (!string.IsNullOrEmpty(file))
            {
                XsdExePath = file;
                appSettings.XsdExePath = file;
                return true;
            }
            return false;
        }
    }
}