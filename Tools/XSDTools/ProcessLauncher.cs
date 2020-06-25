using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace XSDTools
{
    public class ProcessLauncher
    {
        public ProcessLauncherOutput RunXsdExe(string xsdExecutablePath, List<string> xsdFiles, string targetFolder, string targetNamespace)
        {
            var arguments = CreateArguments(xsdFiles, targetFolder, targetNamespace);
            var modelsFilePath = Path.Combine(targetFolder, Path.GetFileNameWithoutExtension(xsdFiles.Last()) + ".cs");
            var commandToExecute = $"{xsdExecutablePath} {arguments}";
            return new ProcessLauncherOutput
            {
                Command = commandToExecute,
                Output = StartProcess(xsdExecutablePath, arguments),
                ModelsFilePath = modelsFilePath
            };
        }

        private string CreateArguments(List<string> xsdFiles, string targetFolder, string targetNamespace)
        {
            var args = "/c " + string.Join(" ", xsdFiles.ToArray()) + " ";
            if (!string.IsNullOrEmpty(targetFolder))
            {
                args += "/out:" + targetFolder + " ";
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