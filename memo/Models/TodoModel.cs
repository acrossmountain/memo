using FreeSql.DataAnnotations;

namespace Memo.Models
{
    [Table(Name = "todos")]
    public class TodoModel : BaseModel
    {
        private string title;
        private string content;
        private int status;


        /// <summary>
        /// 标题
        /// </summary>
        [Column(Name = "title")]
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        /// <summary>
        /// 内容
        /// </summary>
        [Column(Name = "content")]
        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        /// <summary>
        /// 状态
        /// </summary>
        [Column(Name = "status")]
        public int Status
        {
            get { return status; }
            set { status = value; }
        }
    }
}
