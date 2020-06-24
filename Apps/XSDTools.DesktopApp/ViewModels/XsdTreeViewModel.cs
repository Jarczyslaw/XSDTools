using JToolbox.Desktop.Dialogs;
using JToolbox.WPF.Core.Awareness;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace XSDTools.DesktopApp.ViewModels
{
    public class XsdTreeViewModel : BindableBase, ICloseSource
    {
        private readonly IDialogsService dialogsService;
        private ObservableCollection<XsdTreeNodeViewModel> nodes = new ObservableCollection<XsdTreeNodeViewModel>();
        private XsdTreeNodeViewModel selectedNode;

        public XsdTreeViewModel(IDialogsService dialogsService)
        {
            this.dialogsService = dialogsService;
        }

        public DelegateCommand SelectCommand => new DelegateCommand(() =>
        {
            if (SelectedNode == null)
            {
                dialogsService.ShowError("No element selected");
                return;
            }

            SelectedXsdElement = SelectedNode.XsdElement;
            OnClose?.Invoke();
        });

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

        public XsdElement SelectedXsdElement { get; private set; }

        public bool SelectionEnabled { get; private set; }
        public Action OnClose { get; set; }

        public void Setup(List<XsdElement> elements, bool selectionEnabled)
        {
            SelectionEnabled = selectionEnabled;
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