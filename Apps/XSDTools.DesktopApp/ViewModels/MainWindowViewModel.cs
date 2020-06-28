using JToolbox.Desktop.Core.Services;
using JToolbox.Desktop.Dialogs;
using Microsoft.Xml.XMLGen;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
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
        private readonly List<DialogFilterPair> xmlFilter = new List<DialogFilterPair> { new DialogFilterPair("xml") };

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
            var sourceFile = SelectXsdFile();
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
                    ShowXsdLoadErrorsDialog();
                    return;
                }

                if (xsdMap.XsdElements.Count == 0)
                {
                    ShowXsdNoElementsDialog();
                    return;
                }

                windowsService.ShowXsdElements(xsdMap.XsdElements, false);
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
            var sourceFile = SelectXsdFile();
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

                if (dialogsService.ShowYesNoQuestion("Do you want to open folder with processed files?"))
                {
                    systemService.OpenFolderLocation(targetFolder);
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

            var files = dialogsService.OpenFiles("Select xsd files...", null, xsdFilter);
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

            StartLog("Creating models from xsd files");
            AddLog("Selected files:");
            foreach (var file in files)
            {
                AddLog("\t" + file);
            }
            AddLog("Target models file: " + Path.Combine(targetFolder, modelsData.FileName + ".cs"));
            AddLog("Target namespace: " + modelsData.Namespace);

            try
            {
                IsBusy = true;
                var output = await Task.Run(() => xsdProcessor.CreateModels(XsdExePath, files, targetFolder, modelsData.FileName, modelsData.Namespace));
                IsBusy = false;

                AddLog("Executed command: " + output.Command);
                AddLog("xsd.exe output:");
                AddLog(output.Output);

                if (output.Valid && dialogsService.ShowYesNoQuestion("Do you want to open models folder?"))
                {
                    systemService.OpenFileLocation(output.ModelsFilePath);
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

        public DelegateCommand GenerateSampleXmlCommand => new DelegateCommand(async () =>
        {
            var xsdFile = SelectXsdFile();
            if (string.IsNullOrEmpty(xsdFile))
            {
                return;
            }

            try
            {
                IsBusy = true;
                var loadData = await Task.Run(() => xsdProcessor.LoadXsd(xsdFile));
                IsBusy = false;

                LogValidationData(loadData);
                if (loadData.HasErrors)
                {
                    ShowXsdLoadErrorsDialog();
                    return;
                }

                IsBusy = true;
                var rootElements = await Task.Run(() => xsdProcessor.XsdMapper.GetRootXsdElements(loadData.Schema));
                IsBusy = false;

                XmlQualifiedName rootElement = null;
                if (rootElements.Count == 0)
                {
                    ShowXsdNoElementsDialog();
                    return;
                }
                else if (rootElements.Count == 1)
                {
                    rootElement = rootElements[0].XmlSchemaElement.QualifiedName;
                }
                else
                {
                    var selectedRootElement = windowsService.ShowXsdElements(rootElements, true);
                    if (selectedRootElement == null)
                    {
                        return;
                    }

                    rootElement = selectedRootElement.XmlSchemaElement.QualifiedName;
                }

                var targetFile = dialogsService.SaveFile("Save XML file...", null, "sample.xml", new DialogFilterPair("xml"));
                if (string.IsNullOrEmpty(targetFile))
                {
                    return;
                }

                StartLog($"Creating sample XML file for {xsdFile} schema");

                IsBusy = true;
                await Task.Run(() => XmlSampleGenerator.CreateSampleFile(xsdFile, rootElement, targetFile));
                IsBusy = false;

                if (File.Exists(targetFile) && dialogsService.ShowYesNoQuestion("File created successfully. Do you want to open its folder?"))
                {
                    systemService.OpenFileLocation(targetFile);
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

        public DelegateCommand ValidateXmlCommand => new DelegateCommand(async () =>
        {
            var xmlFile = dialogsService.OpenFile("Select XML file...", null, xmlFilter);
            if (string.IsNullOrEmpty(xmlFile))
            {
                return;
            }

            ClearLogs();
            try
            {
                xsdProcessor.XmlProcessor.ValidateXmlFile(xmlFile, true);
            }
            catch (Exception exc)
            {
                AddLog("Error while parsing selected XML file: " + exc.Message);
                return;
            }

            var xsdFile = SelectXsdFile();
            if (string.IsNullOrEmpty(xsdFile))
            {
                return;
            }

            try
            {
                StartLog($"Validating {xmlFile} against {xsdFile} schema");

                IsBusy = true;
                var result = await Task.Run(() => xsdProcessor.ValidateXml(xmlFile, xsdFile));
                IsBusy = false;

                LogValidationData(result);
                if (!result.HasData)
                {
                    AddLog("Validation passed correctly!");
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

        private void ShowXsdLoadErrorsDialog()
        {
            dialogsService.ShowError("Errors occurred while loading the xsd file");
        }

        private void ShowXsdNoElementsDialog()
        {
            dialogsService.ShowError("There are no defined elements in selected xsd file");
        }

        private string SelectXsdFile()
        {
            return dialogsService.OpenFile("Select XSD file...", null, xsdFilter);
        }
    }
}