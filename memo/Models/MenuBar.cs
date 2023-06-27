using Prism.Mvvm;

namespace Memo.Models
{
    /// <summary>
    /// 系统导航菜单
    /// </summary>
    public class MenuBar : BindableBase
    {

        private string icon;
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get { return icon; } set { icon = value; } }

        private string title;
        /// <summary>
        /// 名称
        /// </summary>
        public string Title { get { return title; } set { title = value; } }

        private string path;
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get { return path; } set { path = value; } }
    }
}
