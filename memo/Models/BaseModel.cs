using FreeSql.DataAnnotations;
using System;

namespace Memo.Models
{
    public class BaseModel
    {
        private int id;
        private DateTime createdAt;
        private DateTime updatedAt;

        /// <summary>
        /// 编号
        /// </summary>
        [Column(Name = "id", IsPrimary = true, IsIdentity = true)]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column(Name = "created_at")]
        public DateTime CreatedAt
        {
            get { return createdAt; }
            set { createdAt = value; }
        }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Column(Name = "updated_at")]
        public DateTime UpdatedAt
        {
            get { return updatedAt; }
            set { updatedAt = value; }
        }
    }
}
