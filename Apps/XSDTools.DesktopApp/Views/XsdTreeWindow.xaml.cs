using JToolbox.WPF.UI;

namespace XSDTools.DesktopApp.Views
{
    public partial class XsdTreeWindow : WindowBase
    {
        public XsdTreeWindow()
        {
            InitializeComponent();
        }

        public XsdTreeWindow(object dataContext)
            : this()
        {
            DataContext = dataContext;
        }
    }
}