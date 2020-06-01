using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace JToolbox.WPF.PrismCore
{
    public abstract class ModuleBase : IModule
    {
        public virtual void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            RegisterRegion(regionManager);
        }

        public virtual void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        public abstract void RegisterRegion(IRegionManager regionManager);
    }
}