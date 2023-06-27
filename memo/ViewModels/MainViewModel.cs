using Memo.Extensions;
using Memo.Models;
using Memo.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;

namespace Memo.ViewModels
{
    public class MainViewModel : BindableBase, IConfigure
    {
        private readonly IRegionManager regionManager;
        private IRegionNavigationJournal journal;

        public MainViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        #region commands

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
                        Regions[PrismManager.MainViewRegionName].
                        RequestNavigate(menuBar.Path, target =>
                        {
                            journal = target.Context.NavigationService.Journal;
                        });
                });
            }
        }

        public DelegateCommand? _goBackCommand;
        public DelegateCommand GoBackCommand
        {
            get
            {
                return _goBackCommand ??= new DelegateCommand(() =>
                {
                    if (journal != null && journal.CanGoBack)
                        journal.GoBack();
                });
            }
        }

        public DelegateCommand? _goForwardCommand;
        public DelegateCommand GoForwardCommand
        {
            get
            {
                return _goForwardCommand ??= new DelegateCommand(() =>
                {
                    if (journal != null && journal.CanGoForward)
                        journal.GoForward();
                });
            }
        }

        #endregion commands

        #region props

        private ObservableCollection<MenuBar> _menuBars;
        public ObservableCollection<MenuBar> MenuBars
        {
            get { return _menuBars; }
            set { _menuBars = value; }
        }

        #endregion props

        #region methods

        private void LoadMenuBar()
        {
            MenuBars = new ObservableCollection<MenuBar>
            {
                new MenuBar() { Icon = "Home", Title = "首页", Path = "IndexView" },
                new MenuBar() { Icon = "NotebookOutline", Title = "待办事项", Path = "TodoView" },
                new MenuBar() { Icon = "NotebookPlus", Title = "备忘录", Path = "MemoView" },
                new MenuBar() { Icon = "Cog", Title = "设置", Path = "SettingsView" }
            };
        }

        public void Configure()
        {
            LoadMenuBar();
            regionManager
                .Regions[PrismManager.MainViewRegionName]
                .RequestNavigate("IndexView");
        }

        #endregion
    }
}
