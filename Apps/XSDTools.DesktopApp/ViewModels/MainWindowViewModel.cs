using JToolbox.Desktop.Core.Services;
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
        private bool isBusy;
        private string logs = string.Empty;
        private string xsdExePath = string.Empty;
        private readonly IDialogsService dialogsService;
        private readonly IAppSettings appSettings;
        private readonly IWindowsService windowsService;
        private readonly ISystemService systemService;
        private readonly XsdProcessor xsdProcessor = new XsdProcessor();

        private readonly List<DialogFilterPair> xsdFilter = new List<DialogFilterPair> { new DialogFilterPair("xsd") };

        public MainWindowViewModel(IDialogsService dialogsService, IAppSettings appSettings,
            IWindowsService windowsService, ISystemService systemService)
        {
            this.dialogsService = dialogsService;
            this.appSettings = appSettings;
            this.windowsService = windowsService;
            this.systemService = systemService;

            XsdExePath = appSettings.XsdExePath;
        }

        public DelegateCommand ShowXsdElementsCommand => new DelegateCommand(() =>
        {
            var sourceFile = dialogsService.OpenFile("Select XSD file...", null, xsdFilter);
            if (string.IsNullOrEmpty(sourceFile))
            {
                return;
            }

            try
            {
                var xsdMap = xsdProcessor.GetXsdMap(sourceFile);
                // TODO - check xsdMap correctness; check is any element in xsdMap
                windowsService.GetXsdElement(xsdMap.XsdElements);
            }
            catch (Exception exc)
            {
                dialogsService.ShowException(exc);
            }
        });

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
                StartLog($"Removing external dependencies from {sourceFile}");

                var targetFile = Path.Combine(targetFolder, Path.GetFileName(sourceFile));
                if (sourceFile != targetFile)
                {
                    File.Copy(sourceFile, targetFile, true);
                }

                var processedFiles = await xsdProcessor.RemoveExternalDependenciesFromFile(targetFile);

                AddLog($"Processed files ({processedFiles.Count}):");
                foreach (var processedFile in processedFiles)
                {
                    AddLog("\t" + processedFile);
                }
                AddLog("Done!");
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

            var files = dialogsService.OpenFiles("Open xsd files...", null, xsdFilter);
            if (files == null || files.Count == 0)
            {
                return;
            }

            var modelsData = windowsService.GetModelsData();
            if (modelsData == null)
            {
                return;
            }

            var targetFolder = dialogsService.OpenFolder("Select target folder...");
            if (string.IsNullOrEmpty(targetFolder))
            {
                return;
            }

            var targetModelsFile = Path.Combine(targetFolder, modelsData.FileName);

            StartLog("Creating models from xsd files");
            AddLog("Selected files:");
            foreach (var file in files)
            {
                AddLog("\t" + file);
            }
            AddLog("Target models file: " + targetModelsFile);
            AddLog("Target namespace: " + modelsData.Namespace);

            try
            {
                IsBusy = true;
                var output = xsdProcessor.CreateModels(XsdExePath, files, targetModelsFile, modelsData.Namespace);
                AddLog("Executed command: " + output.Command);
                AddLog("xsd.exe output:");
                AddLog(output.Output);

                IsBusy = false;

                if (output.Valid && dialogsService.ShowYesNoQuestion("Do you want to open models folder?"))
                {
                    systemService.OpenFileLocation(targetModelsFile + ".cs");
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
            if (string.IsNullOrEmpty(file))
            {
                return false;
            }

            if (Path.GetFileName(file) != "xsd.exe")
            {
                dialogsService.ShowError("Invalid file selected");
                return false;
            }

            XsdExePath = file;
            appSettings.XsdExePath = file;
            return true;
        }

        private void ClearLogs()
        {
            Logs = string.Empty;
        }

        private void AddLog(string message)
        {
            Logs = Logs + message + Environment.NewLine;
        }

        private void StartLog(string message)
        {
            ClearLogs();
            AddLog(message);
        }
    }
}