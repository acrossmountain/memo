using Memo.Extensions;
using Memo.Models;
using Memo.Services;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;

namespace Memo.ViewModels
{
    class TodoViewModel : NavigationViewModel
    {
        private readonly IDialogHostService dialogHost;
        private readonly IFreeSql freeSql;

        public TodoViewModel(IContainerProvider provider) : base(provider)
        {
            freeSql = provider.Resolve<IFreeSql>();
            dialogHost = provider.Resolve<IDialogHostService>();


        }

        private DelegateCommand? _addCommand;
        public DelegateCommand AddCommand
        {
            get
            {
                return _addCommand ??= new DelegateCommand(() =>
                {
                    Model = new TodoModel();
                    IsRightDrawerOpen = true;
                });
            }
        }

        private DelegateCommand<TodoModel>? _selectedCommand;
        public DelegateCommand<TodoModel> SelectedCommand
        {
            get
            {
                return _selectedCommand ??= new DelegateCommand<TodoModel>((model) =>
                {
                    var row = freeSql.Select<TodoModel>()
                        .Where(item => item.Id.Equals(model.Id))
                        .OrderBy(item => item.Id).ToOne();

                    if (row != null)
                    {
                        Model = row;
                        IsRightDrawerOpen = true;
                    }
                });
            }
        }

        private DelegateCommand? _searchCommand;
        public DelegateCommand SearchCommand
        {
            get
            {
                return _searchCommand ??= new DelegateCommand(() =>
                {
                    Loading(true);
                    Todos.Clear();
                    var result = freeSql.Select<TodoModel>()
                        .Where(item => item.Title.StartsWith(keyword))
                        .WhereIf((selectedIndex == 0 || selectedIndex == 1), item => item.Status.Equals(selectedIndex))
                        .ToList();
                    foreach (var item in result)
                    {
                        Todos.Add(item);
                    }
                    Loading(false);
                });
            }
        }

        private DelegateCommand? _saveCommand;
        public DelegateCommand SaveCommand
        {
            get
            {
                return _saveCommand ??= new DelegateCommand(() =>
                {
                    if (string.IsNullOrWhiteSpace(Model.Title)
                        || string.IsNullOrWhiteSpace(Model.Content))
                        return;

                    Loading(true);
                    try
                    {
                        if (Model.Id > 0)
                        {
                            int rows = freeSql.Update<TodoModel>()
                                                .Set(item => item.Status, Model.Status)
                                                .Set(item => item.Content, Model.Content)
                                                .Set(item => item.Title, Model.Title)
                                                .Where(item => item.Id.Equals(Model.Id))
                                                .ExecuteAffrows();

                            if (rows > 0)
                            {
                                var tmpModel = freeSql.Select<TodoModel>(Model.Id).ToOne();
                                tmpModel.Content = Model.Content;
                                tmpModel.Title = Model.Title;
                                tmpModel.Status = Model.Status;
                            }

                            IsRightDrawerOpen = false;

                        }
                        else
                        {
                            long id = freeSql.Insert<TodoModel>().AppendData(Model).ExecuteIdentity();
                            if (id > 0)
                            {
                                IsRightDrawerOpen = false;
                                Todos.Add(Model);
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        Loading(false);
                    }
                });
            }
        }

        private DelegateCommand<TodoModel>? _deleteCommand;
        public DelegateCommand<TodoModel> DeleteCommand
        {
            get
            {
                return _deleteCommand ??= new DelegateCommand<TodoModel>(async (model) =>
                {
                    try
                    {
                        var dialogResult = await dialogHost.Question("温馨提示", $"确认删除待办事项:{model.Title} ?");
                        if (dialogResult.Result != Prism.Services.Dialogs.ButtonResult.OK) return;

                        Loading(true);
                        var rows = freeSql.Delete<TodoModel>().Where(item => item.Id == model.Id).ExecuteAffrows();
                        if (rows > 0)
                            Todos.Remove(model);
                    }
                    catch (Exception)
                    {

                    }
                    finally
                    {
                        Loading(false);
                    }
                });
            }
        }

        private bool _isRighteDrawerOpen;
        public bool IsRightDrawerOpen
        {
            get { return _isRighteDrawerOpen; }
            set { _isRighteDrawerOpen = value; RaisePropertyChanged(); }
        }

        private TodoModel _model;
        public TodoModel Model
        {
            get { return _model; }
            set { _model = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<TodoModel> todos;
        public ObservableCollection<TodoModel> Todos
        {
            get { return todos; }
            set { todos = value; RaisePropertyChanged(); }
        }

        private string keyword;
        public string Keyword
        {
            get { return keyword; }
            set { keyword = value; }
        }

        private int selectedIndex;

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value; }
        }

        private void loadTodos()
        {
            Loading(true);
            Todos = new ObservableCollection<TodoModel>();
            var result = freeSql.Select<TodoModel>().ToList();
            foreach (var item in result)
            {
                Todos.Add(item);
            }
            Loading(false);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            loadTodos();
        }
    }
}
