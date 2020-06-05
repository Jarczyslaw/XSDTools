using JToolbox.Desktop.Dialogs;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using XSDTools.DesktopApp.Services;

namespace XSDTools.DesktopApp.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private bool isBusy = false;
        private string logs = string.Empty;
        private string xsdExePath = string.Empty;
        private readonly IDialogsService dialogsService;
        private readonly IAppSettings appSettings;
        private readonly XsdProcessor xsdProcessor = new XsdProcessor();

        private readonly List<DialogFilterPair> xsdFilter = new List<DialogFilterPair> { new DialogFilterPair("xsd") };

        public MainWindowViewModel(IDialogsService dialogsService, IAppSettings appSettings)
        {
            this.dialogsService = dialogsService;
            this.appSettings = appSettings;

            XsdExePath = appSettings.XsdExePath;
        }

        public DelegateCommand RemoveExternalDependenciesCommand => new DelegateCommand(async () =>
        {
            var sourceFile = dialogsService.OpenFile("Select XSD file...", null, xsdFilter);
            if (string.IsNullOrEmpty(sourceFile))
            {
                return;
            }

            var targetFolder = dialogsService.OpenFolder("Select target folder...");
            if (string.IsNullOrEmpty(targetFolder))
            {
                return;
            }

            try
            {
                IsBusy = true;

                var targetFile = Path.Combine(targetFolder, Path.GetFileName(sourceFile));
                if (sourceFile != targetFile)
                {
                    File.Copy(sourceFile, targetFile, true);
                }

                ClearLogs();
                var processedFiles = await xsdProcessor.RemoveExternalDependenciesFromFile(targetFile);
                AddLog($"Processed files ({processedFiles.Count}):");
                foreach (var processedFile in processedFiles)
                {
                    AddLog("\t" + processedFile);
                }
            }
            catch (Exception exc)
            {
                dialogsService.ShowException(exc);
            }
            finally
            {
                IsBusy = false;
            }
        });

        public DelegateCommand GenerateModelsCommand => new DelegateCommand(() =>
        {
            if (!XsdExePathSet && !FindXsdExe())
            {
                return;
            }
        });

        public DelegateCommand GenerateSampleXmlCommand => new DelegateCommand(() =>
        {
        });

        public DelegateCommand ValidateXmlCommand => new DelegateCommand(() =>
        {
        });

        public DelegateCommand SetXsdExePathCommand => new DelegateCommand(() => FindXsdExe());

        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }

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

        private void ClearLogs()
        {
            Logs = string.Empty;
        }

        private void AddLog(string message)
        {
            Logs = Logs + message + Environment.NewLine;
        }
    }
}