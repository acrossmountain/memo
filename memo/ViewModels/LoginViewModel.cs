using Memo.Extensions;
using Memo.Models;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace Memo.ViewModels
{
    public class LoginViewModel : BindableBase, IDialogAware
    {
        public string Title { get; set; } = "Memo";

        public readonly IContainerProvider container;
        private readonly IFreeSql freesql;

        public event Action<IDialogResult> RequestClose;

        public LoginViewModel(IContainerProvider container)
        {
            this.container = container;
            freesql = this.container.Resolve<IFreeSql>();
        }


        private string _username;

        public string Username
        {
            get { return _username; }
            set { _username = value; RaisePropertyChanged(); }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set { _password = value; RaisePropertyChanged(); }
        }

        private int _selectedIndex;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { _selectedIndex = value; RaisePropertyChanged(); }
        }

        private UserModel _model;

        public UserModel Model
        {
            get { return _model; }
            set { _model = value; RaisePropertyChanged(); }
        }


        private DelegateCommand _loginCommand;
        public DelegateCommand LoginCommand
        {
            get
            {
                return _loginCommand ??= new DelegateCommand(() =>
                {
                    if (string.IsNullOrWhiteSpace(Username) ||
                    string.IsNullOrWhiteSpace(Password))
                        return;

                    var user = freesql.Select<UserModel>()
                                .Where(item => item.Username.Equals(Username))
                                .ToOne();

                    // 未找到账号
                    if (user == null)
                        return;

                    // 密码不一致
                    if (user.Password != Password.GetMD5())
                        return;

                    RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
                });
            }
        }

        private DelegateCommand _logoutCommand;
        public DelegateCommand LogoutCommand
        {
            get
            {
                return _logoutCommand ??= new DelegateCommand(() =>
                {
                    RequestClose?.Invoke(new DialogResult(ButtonResult.No));
                });
            }
        }

        private DelegateCommand _registerCommand;
        public DelegateCommand RegisterCommand
        {
            get
            {
                return _registerCommand ??= new DelegateCommand(() =>
                {
                    if (string.IsNullOrWhiteSpace(Model.Username)) return;
                    if (string.IsNullOrWhiteSpace(Model.Password)) return;
                    if (string.IsNullOrWhiteSpace(Model.Nickname)) return;
                    if (string.IsNullOrWhiteSpace(Model.NewPassword)) return;
                    if (Model.Password != Model.NewPassword) return;

                    var user = freesql.Select<UserModel>().Where(item => item.Username.Equals(Model.Username)).ToOne();
                    if (user != null) return;


                    long id = freesql.Insert<UserModel>().AppendData(new UserModel()
                    {
                        Nickname = Model.Nickname,
                        Username = Model.Username,
                        Password = Model.Password.GetMD5(),
                    }).ExecuteIdentity();
                    if (id == 0) return;

                    //SelectedIndex = 0; // 
                });
            }
        }

        private DelegateCommand<string>? _gotoCommand;
        public DelegateCommand<string> GotoCommand
        {
            get
            {
                return _gotoCommand ??= new DelegateCommand<string>((page) =>
                {
                    switch (page)
                    {
                        case "login":
                            SelectedIndex = 0; break;
                        case "register":
                            {
                                Model = new UserModel();
                                SelectedIndex = 1;
                                break;
                            }
                        default:
                            throw new NotImplementedException();
                    }
                });
            }
        }


        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.No));
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}
