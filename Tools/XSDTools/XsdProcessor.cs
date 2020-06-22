using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Schema;

namespace XSDTools
{
    public class XsdProcessor
    {
        public XmlProcessor XmlProcessor { get; } = new XmlProcessor();
        public ProcessLauncher ProcessLauncher { get; } = new ProcessLauncher();
        public XsdMapper XsdMapper { get; } = new XsdMapper();

        public Task<List<string>> RemoveExternalDependenciesFromFile(string filePath)
        {
            var files = new List<string>
            {
                filePath
            };
            return RemoveExternalDependenciesFromFiles(files);
        }

        public Task<List<string>> RemoveExternalDependenciesFromFiles(string folderPath)
        {
            var files = GetXsdFiles(folderPath);
            return RemoveExternalDependenciesFromFiles(files);
        }

        public async Task<List<string>> RemoveExternalDependenciesFromFiles(List<string> inputFilePaths)
        {
            var files = new List<string>();
            await RemoveExternalDependencies(inputFilePaths, files);
            return files;
        }

        private async Task RemoveExternalDependencies(List<string> inputFilePaths, List<string> outputFilePaths)
        {
            foreach (var inputFilePath in inputFilePaths)
            {
                if (!outputFilePaths.Contains(inputFilePath))
                {
                    outputFilePaths.Add(inputFilePath);
                    var dependencies = XmlProcessor.ReplaceExternalDependencies(inputFilePath);
                    var newDependencies = new List<string>();
                    foreach (var dependency in dependencies)
                    {
                        var targetFilePath = Path.Combine(Path.GetDirectoryName(inputFilePath), dependency.ReplacedWith);
                        if (!outputFilePaths.Contains(targetFilePath))
                        {
                            using (var client = new WebClient())
                            {
                                await client.DownloadFileTaskAsync(dependency.OriginalPath, targetFilePath);
                            }
                        }
                        newDependencies.Add(targetFilePath);
                    }
                    await RemoveExternalDependencies(newDependencies, outputFilePaths);
                }
            }
        }

        public ProcessLauncherOutput CreateModels(string xsdExePath, List<string> inputFilePaths, string modelsFilePath, string modelsNamespace)
        {
            var hackFilePath = string.Empty;
            try
            {
                hackFilePath = CreateHackFile(modelsFilePath);
                inputFilePaths.Add(hackFilePath);
                return ProcessLauncher.RunXsd(xsdExePath, inputFilePaths, Path.GetDirectoryName(modelsFilePath), modelsNamespace);
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

        public LoadXsdData LoadXsd(string filePath, bool compile = true)
        {
            return LoadXsds(new List<string> { filePath }, compile);
        }

        public LoadXsdData LoadXsds(List<string> filePaths, bool compile = true)
        {
            var result = new LoadXsdData();
            var schemaSet = new XmlSchemaSet();
            schemaSet.ValidationEventHandler += (s, e) => result.Data.Add(e);
            foreach (var filePath in filePaths)
            {
                result.Schemas.Add(schemaSet.Add(null, filePath));
            }
            result.SchemaSet = schemaSet;
            if (compile)
            {
                schemaSet.Compile();
            }
            return result;
        }

        public ValidationData ValidateXml(string xmlPath, string xsdPath)
        {
            var result = new ValidationData();
            var loadXsdData = LoadXsd(xsdPath);
            if (loadXsdData.HasErrors)
            {
                return loadXsdData;
            }
            var document = XDocument.Load(xmlPath, LoadOptions.SetLineInfo);
            document.Validate(loadXsdData.SchemaSet, (s, e) => result.Data.Add(e));
            return result;
        }

        public XsdMapData GetXsdMap(string filePath)
        {
            var result = new XsdMapData();
            var loadData = LoadXsd(filePath);
            result.Data = loadData.Data;
            if (loadData.HasErrors)
            {
                return result;
            }
            result.XsdElements = XsdMapper.GetXsdElements(loadData.Schema);
            return result;
        }
    }
}