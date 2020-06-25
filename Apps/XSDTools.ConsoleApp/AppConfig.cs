using System.Configuration;
using System.Runtime.CompilerServices;

namespace XSDTools.ConsoleApp
{
    public class AppConfig
    {
        public string XsdExecutablePath => GetSetting();

        public string TargetNamespace => GetSetting();

        public string TargetFileName => GetSetting();

        public string SourceXsdFilesFolder => GetSetting();

        public string TargetXsdFilesFolder => GetSetting();

        public string Validate()
        {
            if (string.IsNullOrEmpty(XsdExecutablePath))
            {
                return "No path to xsd.exe in app.config provided";
            }

            if (string.IsNullOrEmpty(SourceXsdFilesFolder))
            {
                return "Insert source files path in app.config file";
            }

            if (string.IsNullOrEmpty(TargetXsdFilesFolder))
            {
                return "Insert target files path in app.config file";
            }

            if (string.IsNullOrEmpty(TargetFileName))
            {
                return "Insert target file name in app.config file";
            }

            return string.Empty;
        }

        private string GetSetting([CallerMemberName] string key = null)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}