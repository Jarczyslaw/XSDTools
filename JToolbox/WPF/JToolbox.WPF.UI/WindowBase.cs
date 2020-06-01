using System.Windows;

namespace JToolbox.WPF.UI
{
    public class WindowBase : Window
    {
        private readonly WindowEvents windowEvents;

        public WindowBase()
        {
            windowEvents = new WindowEvents(this);
            windowEvents.Attach();
        }

        public WindowBase(object dataContext)
            : this()
        {
            DataContext = dataContext;
        }
    }
}