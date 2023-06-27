using Memo.Extensions;
using Memo.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;

namespace Memo.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;

        public SettingsViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
            loadMenubar();
        }

        private DelegateCommand<MenuBar>? _navigateCommand;
        public DelegateCommand<MenuBar> NavigateCommand
        {
            get
            {
                return _navigateCommand ??= new DelegateCommand<MenuBar>((menuBar) =>
                {
                    if (menuBar == null || string.IsNullOrWhiteSpace(menuBar.Path))
                        return;

                    regionManager.
                        Regions[PrismManager.SettingsViewRegionName].
                        RequestNavigate(menuBar.Path);
                });
            }
        }

        private ObservableCollection<MenuBar> _menuBars;
        public ObservableCollection<MenuBar> MenuBars
        {
            get { return _menuBars; }
            set { _menuBars = value; RaisePropertyChanged(); }
        }

        void loadMenubar()
        {
            MenuBars = new ObservableCollection<MenuBar>
            {
                new MenuBar() { Icon = "Palette", Title = "个性化", Path = "SkinView" },
                new MenuBar() { Icon = "Cog", Title = "系统设置", Path = "" },
                new MenuBar() { Icon = "Information", Title = "关于更多", Path = "AboutView" }
            };
        }
    }
}
