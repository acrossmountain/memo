using DryIoc;
using Memo.Extensions;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;

namespace Memo.Models
{
    public class NavigationViewModel : BindableBase, INavigationAware
    {
        private readonly IContainerProvider container;
        public readonly IEventAggregator aggregator;

        public NavigationViewModel(IContainerProvider container)
        {
            this.container = container;
            aggregator = container.Resolve<IEventAggregator>();
        }

        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public void Loading(bool IsOpen)
        {
            aggregator.Update(new Events.LoadingModel()
            {
                IsOpen = IsOpen
            });
        }
    }
}
