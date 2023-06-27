using FreeSql.DataAnnotations;

namespace Memo.Models
{
    [Table(Name = "users")]
    public class UserModel : BaseModel
    {
        private string _username;

        [Column(Name = "username")]
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }


        private string _password;

        [Column(Name = "password")]
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }


        private string _nickname;

        [Column(Name = "nickname")]
        public string Nickname
        {
            get { return _nickname; }
            set { _nickname = value; }
        }

        private string _newPassword;
        [Column(IsIgnore = true)]
        public string NewPassword
        {
            get { return _newPassword; }
            set { _newPassword = value; }
        }
    }
}
