namespace XSDTools.DesktopApp.Services
{
    public class AppSettings : IAppSettings
    {
        private readonly Properties.Settings settings = Properties.Settings.Default;

        public string XsdExePath
        {
            get => settings.XsdExePath;
            set
            {
                settings.XsdExePath = value;
                settings.Save();
            }
        }
    }
}