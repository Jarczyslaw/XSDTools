using System.Configuration;
using System.Runtime.CompilerServices;

namespace XSDTools.ConsoleApp
{
    public class AppConfig
    {
        public string XsdExecutablePath => GetSetting();

        public string TargetNamespace => GetSetting();

        public string TargetFileName
        {
            get
            {
                var value = GetSetting();
                var extension = ".cs";
                if (!value.EndsWith(extension))
                {
                    value = value + extension;
                }
                return value;
            }
        }

        public string SourceXsdFilesPath => GetSetting();

        public string TargetXsdFilesPath => GetSetting();

        public string Validate()
        {
            if (string.IsNullOrEmpty(XsdExecutablePath))
            {
                return "No path to xsd.exe in app.config provided";
            }

            if (string.IsNullOrEmpty(SourceXsdFilesPath))
            {
                return "Insert source files path in app.config file";
            }

            if (string.IsNullOrEmpty(TargetXsdFilesPath))
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