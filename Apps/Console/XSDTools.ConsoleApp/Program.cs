namespace XSDTools.ConsoleApp
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var consoleView = new ConsoleView();
            var config = new AppConfig();
            var xsdProcessor = new XsdProcessor();

            var validationResult = config.Validate();
            if (!string.IsNullOrEmpty(config.Validate()))
            {
                consoleView.ShowMessage("Invalid app.config file: " + validationResult);
                return;
            }

            var presenter = new Presenter(consoleView, config, xsdProcessor);
            presenter.Run();
        }
    }
}