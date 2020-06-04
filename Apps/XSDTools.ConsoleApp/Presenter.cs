using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace XSDTools.ConsoleApp
{
    public class Presenter
    {
        private readonly IView view;
        private readonly AppConfig config;
        private readonly XsdProcessor xsdProcessor;

        public Presenter(IView view, AppConfig config, XsdProcessor xsdProcessor)
        {
            this.view = view;
            this.config = config;
            this.xsdProcessor = xsdProcessor;
        }

        public async Task Run()
        {
            view.ShowMessage($"XSD.exe path: {config.XsdExecutablePath}");
            view.ShowMessage($"XSD source files path: {config.SourceXsdFilesPath}");
            view.ShowMessage($"XSD target files path: {config.TargetXsdFilesPath}");
            view.ShowMessage($"Target file name: {config.TargetFileName}");
            view.ShowMessage($"Target namespace: {config.TargetNamespace}\n");

            try
            {
                var inputFiles = xsdProcessor.GetXsdFiles(config.SourceXsdFilesPath);
                if (inputFiles.Count == 0)
                {
                    view.ShowMessage("No xsd files found in current directory");
                    view.ShowExitMessage();
                    return;
                }
                else
                {
                    view.ShowMessage($"{inputFiles.Count} xsd files found");
                    view.GetInput("Press ENTER to extract xsd dependencies");
                }

                var filesToProcess = new List<string>();
                foreach (var file in inputFiles)
                {
                    var targetFile = Path.Combine(config.TargetXsdFilesPath, Path.GetFileName(file));
                    File.Copy(file, targetFile, true);
                    filesToProcess.Add(targetFile);
                }

                var processedFiles = await xsdProcessor.RemoveExternalDependenciesFromFiles(config.TargetXsdFilesPath, filesToProcess);
                view.ShowMessage($"Total processed files: {processedFiles.Count}");
                foreach (var xsdFile in processedFiles)
                {
                    view.ShowMessage($"\t{xsdFile}");
                }

                view.GetInput("Press ENTER to start generating models");
                var modelsFilePath = Path.Combine(config.TargetXsdFilesPath, config.TargetFileName);
                view.ShowMessage("Running xsd.exe...");
                xsdProcessor.CreateModels(config.XsdExecutablePath, processedFiles, modelsFilePath, config.TargetNamespace);
                view.ShowMessage("Finished!");
            }
            catch (Exception exc)
            {
                view.ShowMessage("Exception occured: " + exc.Message);
                view.ShowMessage(exc.StackTrace);
            }
            view.ShowExitMessage();
        }
    }
}