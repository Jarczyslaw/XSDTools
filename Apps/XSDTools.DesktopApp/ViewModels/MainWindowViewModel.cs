using JToolbox.Desktop.Core.Services;
using JToolbox.Desktop.Dialogs;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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

        public DelegateCommand ShowXsdElementsCommand => new DelegateCommand(async () =>
        {
            var sourceFile = dialogsService.OpenFile("Select XSD file...", null, xsdFilter);
            if (string.IsNullOrEmpty(sourceFile))
            {
                return;
            }

            try
            {
                StartLog($"Getting elements from {sourceFile}");

                IsBusy = true;
                var xsdMap = await Task.Run(() => xsdProcessor.GetXsdMap(sourceFile));
                IsBusy = false;

                LogValidationData(xsdMap);
                if (xsdMap.HasErrors)
                {
                    dialogsService.ShowError("Errors occurred while loading the xsd file");
                    return;
                }

                if (xsdMap.XsdElements.Count == 0)
                {
                    dialogsService.ShowError("There are no defined elements in selected xsd file");
                    return;
                }

                windowsService.GetXsdElement(xsdMap.XsdElements);
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
                StartLog($"Removing external dependencies from {sourceFile}");

                IsBusy = true;
                var targetFile = Path.Combine(targetFolder, Path.GetFileName(sourceFile));
                if (sourceFile != targetFile)
                {
                    File.Copy(sourceFile, targetFile, true);
                }

                var processedFiles = await xsdProcessor.RemoveExternalDependenciesFromFile(targetFile);
                IsBusy = false;

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

        public DelegateCommand GenerateModelsCommand => new DelegateCommand(async () =>
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
                var output = await Task.Run(() => xsdProcessor.CreateModels(XsdExePath, files, targetModelsFile, modelsData.Namespace));
                IsBusy = false;

                AddLog("Executed command: " + output.Command);
                AddLog("xsd.exe output:");
                AddLog(output.Output);

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

        private void LogValidationData(ValidationData data)
        {
            if (data.HasErrors)
            {
                AddLog($"Errors ({data.ErrorsCount}):");
                for (int i = 0; i < data.ErrorsCount; i++)
                {
                    var error = data.Errors[i];
                    AddLog($"\t{i + 1}. {error.Message}");
                }
            }

            if (data.HasWarnings)
            {
                AddLog($"Warnings ({data.WarningsCount}):");
                for (int i = 0; i < data.WarningsCount; i++)
                {
                    var warning = data.Warnings[i];
                    AddLog($"\t{i + 1}. {warning.Message}");
                }
            }
        }
    }
}