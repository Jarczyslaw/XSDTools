namespace XSDTools
{
    public class ProcessLauncherOutput
    {
        public string Command { get; set; }
        public string Output { get; set; }
        public string ModelsFilePath { get; set; }

        public bool Valid => Output?.Contains("Writing file") == true;
    }
}