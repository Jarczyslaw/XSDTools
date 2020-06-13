﻿using Prism.Ioc;
using System.Collections.Generic;
using XSDTools.DesktopApp.Models;
using XSDTools.DesktopApp.ViewModels;
using XSDTools.DesktopApp.Views;

namespace XSDTools.DesktopApp.Services
{
    public class WindowsService : IWindowsService
    {
        private readonly IContainerExtension containerExtension;

        public WindowsService(IContainerExtension containerExtension)
        {
            this.containerExtension = containerExtension;
        }

        public ModelsData GetModelsData()
        {
            return containerExtension.Resolve<ModelsDataWindow>().ShowAsDialog();
        }

        public XsdElement GetXsdElement(List<XsdElement> xsdElements)
        {
            var dataContext = containerExtension.Resolve<XsdTreeViewModel>();
            dataContext.SetNodes(xsdElements);
            var window = containerExtension.Resolve<XsdTreeWindow>();
            window.DataContext = dataContext;
            window.ShowDialog();
            return dataContext.SelectedXsdElement;
        }
    }
}