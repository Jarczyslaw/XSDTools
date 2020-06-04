using System;

namespace XSDTools.ConsoleApp
{
    public class ConsoleView : IView
    {
        public void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }

        public void ShowExitMessage()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public string GetInput(string message = null)
        {
            if (!string.IsNullOrEmpty(message))
            {
                Console.Write(message);
            }
            return Console.ReadLine();
        }
    }
}