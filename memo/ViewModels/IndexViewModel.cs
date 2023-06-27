using Memo.Extensions;
using Memo.Models;
using Memo.Services;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Memo.ViewModels
{
    public class IndexViewModel : NavigationViewModel
    {
        public IndexViewModel(IContainerProvider provider) : base(provider)
        {
            dialog = provider.Resolve<IDialogHostService>();
            freeSql = provider.Resolve<IFreeSql>();
            regionManager = provider.Resolve<IRegionManager>();
        }

        #region props

        private readonly IRegionManager regionManager;
        private readonly IDialogHostService dialog;
        private readonly IFreeSql freeSql;

        private ObservableCollection<TaskBar> _taskBars;
        public ObservableCollection<TaskBar> TaskBars
        {
            get { return _taskBars; }
            set { _taskBars = value; RaisePropertyChanged(); }
        }

        private Summary _summary;
        public Summary Summary
        {
            get { return _summary; }
            set { _summary = value; RaisePropertyChanged(); }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; RaisePropertyChanged(); }
        }


        #endregion props

        #region commands       

        /// <summary>
        /// 新增待办事项
        /// </summary>
        private DelegateCommand<TodoModel>? _addTodoCommand;
        public DelegateCommand<TodoModel> AddTodoCommand
        {
            get
            {
                return _addTodoCommand ??= new DelegateCommand<TodoModel>(async (o) =>
                {
                    DialogParameters param = new DialogParameters();
                    if (o != null)
                        param.Add("Value", o);

                    var dialogResult = await this.dialog.ShowDialog("AddTodoView", param);
                    if (dialogResult.Result == ButtonResult.OK)
                    {
                        try
                        {
                            Loading(true);
                            var data = dialogResult.Parameters.GetValue<TodoModel>("Value");
                            if (data.Id > 0)
                            {
                                freeSql.Update<TodoModel>()
                                                .Set(item => new { data.Title, data.Content, data.Status })
                                                .Where(item => item.Id == data.Id)
                                                .ExecuteAffrows();
                            }
                            else
                            {
                                long id = freeSql.Insert<TodoModel>().AppendData(data).ExecuteIdentity();
                                if (id > 0)
                                {
                                    Summary.TodoCount++;
                                    Summary.TodoCompletedRadio = (Summary.TodoCompletedCount / (double)Summary.TodoCount).ToString("0%");
                                    Summary.Todos.Add(data);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                        finally
                        {
                            Loading(false);
                        }
                    }
                });
            }
        }

        /// <summary>
        /// 新增备忘录
        /// </summary>
        private DelegateCommand<MemoModel>? _addMemoCommand;
        public DelegateCommand<MemoModel> AddMemoCommand
        {
            get
            {
                return _addMemoCommand ??= new DelegateCommand<MemoModel>(async (o) =>
                {
                    DialogParameters param = new DialogParameters();
                    if (o != null)
                        param.Add("Value", o);
                    var dialogResult = await this.dialog.ShowDialog("AddMemoView", param);
                    if (dialogResult.Result == ButtonResult.OK)
                    {
                        try
                        {
                            Loading(true);
                            var data = dialogResult.Parameters.GetValue<MemoModel>("Value");
                            if (data.Id > 0)
                            {
                                freeSql.Update<MemoModel>()
                                                .Set(item => new { data.Title, data.Content, data.Status })
                                                .Where(item => item.Id == data.Id)
                                                .ExecuteAffrows();
                            }
                            else
                            {
                                long id = freeSql.Insert<MemoModel>().AppendData(data).ExecuteIdentity();
                                if (id > 0)
                                {
                                    Summary.Memos.Add(data);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                        finally
                        {
                            Loading(false);
                        }
                    }
                });
            }
        }

        /// <summary>
        /// 完成事项命令
        /// </summary>
        private DelegateCommand<TodoModel>? _completeTodoCommand;
        public DelegateCommand<TodoModel> CompleteTodoCommand
        {
            get
            {
                return _completeTodoCommand ??= new DelegateCommand<TodoModel>((o) =>
                {
                    int rows = freeSql.Update<TodoModel>(o.Id)
                                .Set(item => item.Status == 1)
                                .Where(item => item.Status.Equals(0))
                                .ExecuteAffrows();

                    if (rows > 0)
                    {
                        Summary.TodoCompletedCount++;
                        Summary.TodoCompletedRadio = (Summary.TodoCompletedCount / (double)Summary.TodoCount).ToString("0%");
                        Summary.Todos.Remove(o);
                    }

                    aggregator.PublishMessage("待办完成了");
                });
            }
        }


        private DelegateCommand<TaskBar>? _navigatorCommand;
        public DelegateCommand<TaskBar> NavigatorCommand
        {
            get
            {
                return _navigatorCommand ??= new DelegateCommand<TaskBar>((taskBar) =>
                {
                    if (string.IsNullOrWhiteSpace(taskBar.Target)) return;
                    NavigationParameters parameters = new NavigationParameters();
                    if (taskBar.Title == "已完成")
                    {
                        parameters.Add("status", 1);
                    }

                    regionManager
                        .Regions[PrismManager.MainViewRegionName]
                        .RequestNavigate(taskBar.Target);
                });
            }
        }


        #endregion commands

        #region methods

        private void loadTaskBar()
        {
            TaskBars = new ObservableCollection<TaskBar>
            {
                new TaskBar() { Icon = "ClockFast", Title = "汇总", Target = "TodoView", Color = "#FF0CA0FF" },
                new TaskBar() { Icon = "ClockCheckOutline", Title = "已完成", Target = "TodoView", Color = "#FF1ECA3A" },
                new TaskBar() { Icon = "ChartllineVariant", Title = "完成比例", Target = "TodoView", Color = "#FF02C6DC" },
                new TaskBar() { Icon = "PlaylistStar", Title = "备忘录", Target = "MemoView", Color = "#FFFFA000" }
            };
        }

        private void loadData()
        {

            Summary = new Summary()
            {
                Memos = new ObservableCollection<MemoModel>(),
                Todos = new ObservableCollection<TodoModel>(),
            };

            var todoResult = freeSql.Select<TodoModel>().Where(item => item.Status.Equals(0)).ToList();
            var memoResult = freeSql.Select<MemoModel>().ToList();
            foreach (var item in todoResult)
            {
                Summary.Todos.Add(item);
            }
            foreach (var item in memoResult)
            {
                Summary.Memos.Add(item);
            }

            Summary.TodoCount = (int)freeSql.Select<TodoModel>().Count();
            Summary.MemoCount = (int)freeSql.Select<MemoModel>().Count();
            Summary.TodoCompletedCount = (int)freeSql.Select<TodoModel>().Where(item => item.Status.Equals(1)).Count();
            Summary.TodoCompletedRadio = (Summary.TodoCompletedCount / (double)Summary.TodoCount).ToString("0%");
            Refresh();

            Title = $"你好，下雨 {DateTime.Now.GetDateTimeFormats('D')[1].ToString()}";
        }

        private void Refresh()
        {
            TaskBars[0].Content = Summary.TodoCount.ToString();
            TaskBars[1].Content = Summary.TodoCompletedCount.ToString();
            TaskBars[2].Content = Summary.TodoCompletedRadio;
            TaskBars[3].Content = Summary.MemoCount.ToString();
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            loadTaskBar();
            loadData();
            base.OnNavigatedTo(navigationContext);
        }

        #endregion methods
    }
}
