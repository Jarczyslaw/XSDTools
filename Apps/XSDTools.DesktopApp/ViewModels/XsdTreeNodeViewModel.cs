using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace XSDTools.DesktopApp.ViewModels
{
    public class XsdTreeNodeViewModel : BindableBase
    {
        private string name = string.Empty;
        private bool isExpanded;
        private ObservableCollection<XsdTreeNodeViewModel> nodes = new ObservableCollection<XsdTreeNodeViewModel>();

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public ObservableCollection<XsdTreeNodeViewModel> Nodes
        {
            get => nodes;
            set => SetProperty(ref nodes, value);
        }

        public bool IsExpanded
        {
            get => isExpanded;
            set => SetProperty(ref isExpanded, value);
        }

        public XsdElement XsdElement { get; set; }
    }
}