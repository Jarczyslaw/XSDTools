using System.Collections.Generic;
using System.Diagnostics;

namespace XSDTools
{
    public class ProcessLauncher
    {
        public string RunXsd(string xsdExecutablePath, List<string> xsdFiles, string targetNamespace)
        {
            var arguments = CreateArguments(xsdFiles, targetNamespace);
            var commandToExecute = $"{xsdExecutablePath} {arguments}";
            StartProcess(xsdExecutablePath, arguments);
            return commandToExecute;
        }

        private string CreateArguments(List<string> xsdFiles, string targetNamespace)
        {
            var filesArgs = string.Join(" ", xsdFiles.ToArray());
            return string.Format("/c {0} /n:{1}", filesArgs, targetNamespace);
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