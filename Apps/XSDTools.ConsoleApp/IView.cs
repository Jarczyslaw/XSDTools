namespace XSDTools.ConsoleApp
{
    public interface IView
    {
        void ShowMessage(string message);

        string GetInput(string message = null);

        void ShowExitMessage();
    }
}