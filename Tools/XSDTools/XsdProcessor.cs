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
            var files = new List<string>();
            RemoveExternalDependencies(Path.GetFullPath(filePath), new List<string> { filePath }, files);
            return files;
        }

        public List<string> RemoveExternalDependenciesFromFiles(string folderPath)
        {
            var files = new List<string>();
            RemoveExternalDependencies(folderPath, GetXsdFiles(folderPath), files);
            return files;
        }

        private void RemoveExternalDependencies(string targetPath, List<string> inputXsdFilePaths, List<string> outputXsdFilePaths)
        {
            foreach (var inputFile in inputXsdFilePaths)
            {
                outputXsdFilePaths.Add(inputFile);
                var dependencies = xmlProcessor.ReplaceExternalDependencies(inputFile);
                var newDependencies = new List<string>();
                foreach (var dependency in dependencies)
                {
                    var targetFilePath = Path.Combine(targetPath, dependency.ReplacedWith);
                    if (!File.Exists(targetFilePath))
                    {
                        fileDownloader.DownloadAs(dependency.OriginalPath, targetFilePath);
                        newDependencies.Add(targetFilePath);
                    }
                }
                RemoveExternalDependencies(targetPath, newDependencies, outputXsdFilePaths);
            }
        }

        public void CreateModels(List<string> inputXsdFilePaths, string targetPath, string targetNamespace)
        {
            var hackFilePath = string.Empty;
            try
            {
                hackFilePath = CreateHackFile(targetPath, targetNamespace);
                inputXsdFilePaths.Add(hackFilePath);
                processLauncher.RunXsd(xsdExePath, inputXsdFilePaths, targetNamespace);
            }
            finally
            {
                if (File.Exists(hackFilePath))
                {
                    File.Delete(hackFilePath);
                }
            }
        }

        private string CreateHackFile(string targetPath, string targetNamespace)
        {
            var hackFilePath = Path.Combine(targetPath, targetNamespace + ".xsd");
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

        public void Dispose()
        {
            fileDownloader.Dispose();
        }
    }
}