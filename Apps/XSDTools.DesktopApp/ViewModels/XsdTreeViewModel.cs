using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace XSDTools.DesktopApp.ViewModels
{
    // TODO - dorobić selectedItem, dorobić tooltipy
    public class XsdTreeViewModel : BindableBase
    {
        private ObservableCollection<XsdTreeNodeViewModel> nodes = new ObservableCollection<XsdTreeNodeViewModel>();
        private XsdTreeNodeViewModel selectedNode;

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

        public void Setup(List<XsdElement> elements)
        {
            SetNodes(elements);
            ExpandRootNodes();
        }

        private void SetNodes(List<XsdElement> elements, XsdTreeNodeViewModel parentNode = null)
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

        private void ExpandRootNodes()
        {
            foreach (var node in Nodes)
            {
                node.IsExpanded = true;
            }
        }
    }
}