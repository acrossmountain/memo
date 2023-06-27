using Prism.Mvvm;

namespace Memo.Models
{
    public class TaskBar : BindableBase
    {
        private string icon;

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon
        {
            get { return icon; }
            set { icon = value; }
        }

        private string title;

        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        private string content;

        /// <summary>
        /// 内容
        /// </summary>
        public string Content
        {
            get { return content; }
            set { content = value; }
        }


        private string color;

        /// <summary>
        /// 颜色
        /// </summary>
        public string Color
        {
            get { return color; }
            set { color = value; }
        }

        private string target;

        /// <summary>
        /// 目标
        /// </summary>
        public string Target
        {
            get { return target; }
            set { target = value; }
        }

    }
}
