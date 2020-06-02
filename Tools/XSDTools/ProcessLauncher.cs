using System.Collections.Generic;
using System.Diagnostics;

namespace XSDTools
{
    public class ProcessLauncher
    {
        public string RunXsd(string xsdExecutablePath, List<string> xsdFiles, string targetPath, string targetNamespace)
        {
            var arguments = CreateArguments(xsdFiles, targetPath, targetNamespace);
            var commandToExecute = $"{xsdExecutablePath} {arguments}";
            StartProcess(xsdExecutablePath, arguments);
            return commandToExecute;
        }

        private string CreateArguments(List<string> xsdFiles, string targetPath, string targetNamespace)
        {
            var args = "/c " + string.Join(" ", xsdFiles.ToArray()) + " ";
            if (!string.IsNullOrEmpty(targetPath))
            {
                args += "/out:" + targetPath + " ";
            }
            args += "/l:CS ";
            if (!string.IsNullOrEmpty(targetNamespace))
            {
                args += "/n:" + targetNamespace + " ";
            }
            return string.Format("/c {0} /n:{1}", args, targetNamespace);
        }

        private void StartProcess(string exePath, string arguments = null, bool useShellExecute = false)
        {
            var info = new ProcessStartInfo(exePath, arguments)
            {
                UseShellExecute = useShellExecute
            };
            var proc = Process.Start(info);
            proc.WaitForExit();
        }
    }
}