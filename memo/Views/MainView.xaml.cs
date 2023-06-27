using Memo.Extensions;
using Memo.Services;
using Prism.Events;
using System.Windows;
using System.Windows.Input;

namespace Memo.Views
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window
    {

        public readonly IDialogHostService DialogHostService;

        public MainView(IEventAggregator aggregator, IDialogHostService dialogHostService)
        {
            InitializeComponent();

            this.DialogHostService = dialogHostService;

            aggregator.SubscribeMessage(arg =>
            {
                snackBar.MessageQueue?.Enqueue(arg);
            });

            aggregator.Register(arg =>
            {
                dialogHost.IsOpen = arg.IsOpen;
                if (dialogHost.IsOpen)
                {
                    dialogHost.DialogContent = new LoadingView();
                }
            });

            // basic
            btnMin.Click += (s, e) => { this.WindowState = WindowState.Minimized; };
            btnMax.Click += (s, e) =>
            {
                if (this.WindowState == WindowState.Maximized)
                    this.WindowState = WindowState.Normal;
                else
                    this.WindowState = WindowState.Maximized;
            };
            btnClose.Click += async (s, e) =>
            {
                var dialogResult = await this.DialogHostService.Question("温馨提示", "确认退出系统？");
                if (dialogResult.Result != Prism.Services.Dialogs.ButtonResult.OK) return;
                this.Close();
            };
            colorZone.MouseMove += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    this.DragMove();
            };
            colorZone.MouseDoubleClick += (s, e) =>
            {
                if (this.WindowState == WindowState.Maximized)
                    this.WindowState = WindowState.Normal;
                else
                    this.WindowState = WindowState.Maximized;
            };

            // menuBar
            menuBar.SelectionChanged += (s, e) =>
            {
                drawerHost.IsLeftDrawerOpen = false;
            };
        }

    }
}
