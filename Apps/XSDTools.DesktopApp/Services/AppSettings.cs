using System;
using System.IO;

namespace XSDTools.DesktopApp.Services
{
    public class AppSettings : IAppSettings
    {
        private readonly Properties.Settings settings = Properties.Settings.Default;

        public string XsdExePath
        {
            get
            {
                if (string.IsNullOrEmpty(settings.XsdExePath))
                {
                    return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xsd.exe");
                }
                return settings.XsdExePath;
            }
            set
            {
                settings.XsdExePath = value;
                settings.Save();
            }
        }
    }
}