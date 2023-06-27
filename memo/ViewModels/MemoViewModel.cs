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
    public class MemoViewModel : NavigationViewModel
    {
        private readonly IDialogHostService dialogHost;
        private readonly IFreeSql freeSql;

        public MemoViewModel(IContainerProvider provider) : base(provider)
        {
            freeSql = provider.Resolve<IFreeSql>();
            dialogHost = provider.Resolve<IDialogHostService>();
            Memos = new ObservableCollection<MemoModel>();
        }

        #region props

        private bool _isRighteDrawerOpen;
        public bool IsRightDrawerOpen
        {
            get { return _isRighteDrawerOpen; }
            set { _isRighteDrawerOpen = value; RaisePropertyChanged(); }
        }

        private MemoModel _model;
        public MemoModel Model
        {
            get { return _model; }
            set { _model = value; RaisePropertyChanged(); }
        }

        private string _keyword;
        public string Keyword
        {
            get { return _keyword; }
            set { _keyword = value; }
        }

        private ObservableCollection<MemoModel> _memos;
        public ObservableCollection<MemoModel> Memos
        {
            get { return _memos; }
            set { _memos = value; RaisePropertyChanged(); }
        }

        #endregion props

        #region commands
        private DelegateCommand? _addCommand;
        public DelegateCommand AddCommand
        {
            get
            {
                return _addCommand ??= new DelegateCommand(() =>
                {
                    Model = new MemoModel();
                    IsRightDrawerOpen = true;
                });
            }
        }

        private DelegateCommand<MemoModel>? _selectedCommand;
        public DelegateCommand<MemoModel> SelectedCommand
        {
            get
            {
                return _selectedCommand ??= new DelegateCommand<MemoModel>((o) =>
                {
                    var row = freeSql.Select<MemoModel>()
                        .Where(item => item.Id.Equals(o.Id))
                        .OrderBy(item => item.Id)
                        .ToOne();

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
                    Memos.Clear();
                    var result = freeSql.Select<MemoModel>()
                        .Where(item => item.Title.StartsWith(Keyword))
                        .ToList();
                    foreach (var item in result)
                    {
                        Memos.Add(item);
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
                            int rows = freeSql.Update<MemoModel>()
                                                .Set(item => item.Status, Model.Status)
                                                .Set(item => item.Content, Model.Content)
                                                .Set(item => item.Title, Model.Title)
                                                .Where(item => item.Id.Equals(Model.Id))
                                                .ExecuteAffrows();

                            if (rows > 0)
                            {
                                var tmpModel = freeSql.Select<MemoModel>(Model.Id).ToOne();
                                Model.Content = tmpModel.Content;
                                Model.Title = tmpModel.Title;
                            }

                            IsRightDrawerOpen = false;
                        }
                        else
                        {
                            long id = freeSql.Insert<MemoModel>().AppendData(Model).ExecuteIdentity();
                            if (id > 0)
                                Memos.Add(Model);
                            IsRightDrawerOpen = false;
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

        private DelegateCommand<MemoModel>? _deleteCommand;
        public DelegateCommand<MemoModel> DeleteCommand
        {
            get
            {
                return _deleteCommand ??= new DelegateCommand<MemoModel>(async (model) =>
                {
                    try
                    {
                        var dialogResult = await dialogHost.Question("温馨提示", $"确认删除备忘录:{model.Title} ?");
                        if (dialogResult.Result != Prism.Services.Dialogs.ButtonResult.OK) return;

                        Loading(true);
                        var rows = freeSql.Delete<MemoModel>().Where(item => item.Id == model.Id).ExecuteAffrows();
                        if (rows > 0)
                            Memos.Remove(model);
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

        #endregion commands

        #region methods
        private void loadTodos()
        {
            Loading(true);
            Memos.Clear();
            var result = freeSql.Select<MemoModel>().ToList();
            foreach (var item in result)
            {
                Memos.Add(item);
            }
            Loading(false);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            loadTodos();
        }

        #endregion methods
    }
}
