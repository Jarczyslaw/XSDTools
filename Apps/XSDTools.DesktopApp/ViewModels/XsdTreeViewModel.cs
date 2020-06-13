using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace XSDTools.DesktopApp.ViewModels
{
    // TODO - dorobić selectedItem, dorobić IsExpanded, dorobić wyłączania zaznaczania
    public class XsdTreeViewModel : BindableBase
    {
        private readonly IDialogService dialogService;
        private bool selectionEnabled;
        private ObservableCollection<XsdTreeNodeViewModel> nodes = new ObservableCollection<XsdTreeNodeViewModel>();
        private XsdTreeNodeViewModel selectedNode;

        public XsdTreeViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;
        }

        public bool SelectionEnabled
        {
            get => selectionEnabled;
            set => SetProperty(ref selectionEnabled, value);
        }

        public ObservableCollection<XsdTreeNodeViewModel> Nodes
        {
            get => nodes;
            set => SetProperty(ref nodes, value);
        }

        public XsdTreeNodeViewModel SelectedNode
        {
            get => selectedNode;
            set => SetProperty(ref selectedNode, value);
        }

        public XsdElement SelectedXsdElement => SelectedNode?.XsdElement;

        public void SetNodes(List<XsdElement> elements, XsdTreeNodeViewModel parentNode = null)
        {
            foreach (var element in elements)
            {
                var newNode = new XsdTreeNodeViewModel
                {
                    Name = element.XsdName,
                    XsdElement = element
                };

                if (parentNode == null)
                {
                    Nodes.Add(newNode);
                }
                else
                {
                    parentNode.Nodes.Add(newNode);
                }
                SetNodes(element.Children, newNode);
            }
        }
    }
}