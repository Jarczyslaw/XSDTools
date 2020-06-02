using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XSDTools
{
    public class XsdProcessor : IDisposable
    {
        private readonly XmlProcessor xmlProcessor = new XmlProcessor();
        private readonly ProcessLauncher processLauncher = new ProcessLauncher();
        private readonly FileDownloader fileDownloader = new FileDownloader();
        private readonly string xsdExePath;

        public XsdProcessor(string xsdExePath)
        {
            this.xsdExePath = xsdExePath;
        }

        public List<string> RemoveExternalDependenciesFromFile(string filePath)
        {
            return RemoveExternalDependenciesFromFiles(Path.GetDirectoryName(filePath), new List<string> { Path.GetFileName(filePath) });
        }

        public List<string> RemoveExternalDependenciesFromFiles(string folderPath)
        {
            return RemoveExternalDependenciesFromFiles(folderPath, GetXsdFileNames(folderPath));
        }

        public List<string> RemoveExternalDependenciesFromFiles(string folderPath, List<string> inputFileNames)
        {
            var files = new List<string>();
            var downloadedFiles = new List<string>();
            RemoveExternalDependencies(folderPath, inputFileNames, downloadedFiles, files);
            return files;
        }

        private void RemoveExternalDependencies(string inputPath, List<string> inputFileNames, List<string> downloadedFiles, List<string> outputFilePaths)
        {
            foreach (var inputFile in inputFileNames)
            {
                var inputFilePath = Path.Combine(inputPath, inputFile);
                outputFilePaths.Add(inputFilePath);
                var dependencies = xmlProcessor.ReplaceExternalDependencies(inputFilePath);
                var newDependencies = new List<string>();
                foreach (var dependency in dependencies)
                {
                    var targetFilePath = Path.Combine(inputPath, dependency.ReplacedWith);
                    if (!downloadedFiles.Contains(targetFilePath))
                    {
                        fileDownloader.DownloadAs(dependency.OriginalPath, targetFilePath);
                        downloadedFiles.Add(targetFilePath);
                    }
                    newDependencies.Add(targetFilePath);
                }
                RemoveExternalDependencies(inputPath, newDependencies, downloadedFiles, outputFilePaths);
            }
        }

        public void CreateModels(List<string> inputFilePaths, string modelsFilePath, string modelsNamespace)
        {
            var hackFilePath = string.Empty;
            try
            {
                hackFilePath = CreateHackFile(modelsFilePath);
                inputFilePaths.Add(hackFilePath);
                processLauncher.RunXsd(xsdExePath, inputFilePaths, Path.GetDirectoryName(modelsFilePath), modelsNamespace);
            }
            finally
            {
                if (File.Exists(hackFilePath))
                {
                    File.Delete(hackFilePath);
                }
            }
        }

        private string CreateHackFile(string modelsFilePath)
        {
            var hackPath = Path.GetDirectoryName(modelsFilePath);
            var hackFileName = Path.GetFileNameWithoutExtension(modelsFilePath);
            var hackFilePath = Path.Combine(hackPath, hackFileName + ".xsd");
            const string hackFileContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xs:schema xmlns:xs=""http://www.w3.org/2001/XMLSchema""/>";
            File.WriteAllText(hackFilePath, hackFileContent);
            return hackFilePath;
        }

        public List<string> GetXsdFiles(string folderPath)
        {
            return Directory.GetFiles(folderPath, "*.xsd")
                .ToList();
        }

        public List<string> GetXsdFileNames(string folderPath)
        {
            return GetXsdFiles(folderPath)
                .Select(Path.GetFileName)
                .ToList();
        }

        public void Dispose()
        {
            fileDownloader.Dispose();
        }
    }
}