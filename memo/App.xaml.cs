using Memo.Helpers;
using Memo.Services;
using Memo.ViewModels;
using Memo.ViewModels.Dialogs;
using Memo.Views;
using Memo.Views.Dialogs;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Services.Dialogs;
using System.Windows;

namespace Memo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainView>();
        }

        protected override void OnInitialized()
        {
            var dialog = Container.Resolve<IDialogService>();
            dialog.ShowDialog("LoginView", c =>
            {
                if (c.Result != ButtonResult.OK)
                {
                    Application.Current.Shutdown();
                    return;
                }
            });

            var configure = App.Current.MainWindow.DataContext as IConfigure;
            if (configure != null)
            {
                configure.Configure();
            }

            base.OnInitialized();
        }

        protected override void RegisterTypes(IContainerRegistry container)
        {
            container.RegisterInstance<IFreeSql>(DatabaseHelper.Build());

            container.Register<IDialogHostService, DialogHostService>();

            container.RegisterForNavigation<IndexView, IndexViewModel>();
            container.RegisterForNavigation<TodoView, TodoViewModel>();
            container.RegisterForNavigation<MemoView, MemoViewModel>();
            container.RegisterForNavigation<SettingsView, SettingsViewModel>();
            container.RegisterForNavigation<SkinView, SkinViewModel>();
            container.RegisterForNavigation<AboutView, AboutViewModel>();

            container.RegisterForNavigation<AddMemoView, AddMemoViewModel>();
            container.RegisterForNavigation<AddTodoView, AddTodoViewModel>();
            container.RegisterForNavigation<MessageBoxView, MessageBoxViewModel>();

            container.RegisterDialog<LoginView, LoginViewModel>();
        }
    }
}
