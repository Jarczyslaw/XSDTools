using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace XSDTools
{
    public class ProcessLauncher
    {
        public ProcessLauncherOutput RunXsd(string xsdExecutablePath, List<string> xsdFiles, string targetPath, string targetNamespace)
        {
            var arguments = CreateArguments(xsdFiles, targetPath, targetNamespace);
            var commandToExecute = $"{xsdExecutablePath} {arguments}";
            return new ProcessLauncherOutput
            {
                Command = commandToExecute,
                Output = StartProcess(xsdExecutablePath, arguments)
            };
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
            return args.Trim();
        }

        private string StartProcess(string exePath, string arguments = null)
        {
            var info = new ProcessStartInfo(exePath, arguments)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            var proc = Process.Start(info);
            var sb = new StringBuilder();
            while (!proc.StandardOutput.EndOfStream)
            {
                sb.AppendLine(proc.StandardOutput.ReadLine());
            }
            proc.WaitForExit();
            return sb.ToString();
        }
    }
}